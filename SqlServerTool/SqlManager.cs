using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlServerTool
{
    public enum DatabaseType { Unknown, Sepidar, Dasht }

    /// <summary>
    /// A container for all retrieved details about a specific database.
    /// </summary>
    public class DbDetails
    {
        public string DataVersion { get; set; } = "N/A";
        public string CompanyName { get; set; } = "N/A";
        public DatabaseType DbType { get; set; } = DatabaseType.Unknown;
        public List<string> FiscalYears { get; set; } = new List<string>();
        public string ActivationCode { get; set; } = "N/A";
    }


    /// <summary>
    /// Core class for interacting with a SQL Server instance.
    /// Handles all database operations.
    /// </summary>
    public class SqlManager
    {
        private readonly string _connectionString;

        public SqlManager(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));
            _connectionString = connectionString;
        }

        private async Task<SqlConnection> GetOpenConnectionAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        private async Task ExecuteNonQueryAsync(string commandText, string dbName, bool useMaster = false, IProgress<int> progress = null)
        {
            var builder = new SqlConnectionStringBuilder(_connectionString);
            if (useMaster)
            {
                builder.InitialCatalog = "master";
            }

            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                if (progress != null)
                {
                    connection.InfoMessage += (sender, e) =>
                    {
                        var message = e.Message;
                        if (message.EndsWith(" percent processed."))
                        {
                            var percentStr = message.Split(' ')[0];
                            if (int.TryParse(percentStr, out int percent))
                            {
                                progress.Report(percent);
                            }
                        }
                    };
                }

                await connection.OpenAsync();

                if (commandText.Trim().StartsWith("DROP", StringComparison.OrdinalIgnoreCase) ||
                    commandText.Contains("sp_detach_db"))
                {
                    using (var singleUserCmd = new SqlCommand($"ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", connection))
                    {
                        await singleUserCmd.ExecuteNonQueryAsync();
                    }
                }

                using (var command = new SqlCommand(commandText, connection))
                {
                    command.CommandTimeout = 600; // 10 minutes for long operations
                    await command.ExecuteNonQueryAsync();
                }

                if (commandText.Trim().StartsWith("RESTORE", StringComparison.OrdinalIgnoreCase) && !commandText.Trim().StartsWith("RESTORE VERIFYONLY", StringComparison.OrdinalIgnoreCase))
                {
                    using (var multiUserCmd = new SqlCommand($"ALTER DATABASE [{dbName}] SET MULTI_USER", connection))
                    {
                        await multiUserCmd.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the full SQL Server version string, including the Edition.
        /// </summary>
        /// <returns>A formatted string with the server edition and version number.</returns>
        public async Task<string> GetSqlServerVersionAsync()
        {
            string edition = "N/A";
            string version = "N/A";

            const string query = "SELECT SERVERPROPERTY('Edition'), @@VERSION";

            using (var connection = await GetOpenConnectionAsync())
            using (var command = new SqlCommand(query, connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    edition = reader.GetString(0);
                    version = reader.GetString(1).Split('\n')[0].Trim();
                }
            }
            return $"{edition} - {version}";
        }

        /// <summary>
        /// Gets a simplified SQL Server version string for filenames.
        /// </summary>
        /// <returns>A short string like "SQL2019".</returns>
        public async Task<string> GetSqlShortVersionAsync()
        {
            const string query = "SELECT @@VERSION";
            string versionString = "";
            using (var connection = await GetOpenConnectionAsync())
            using (var command = new SqlCommand(query, connection))
            {
                versionString = (await command.ExecuteScalarAsync()) as string ?? "";
            }

            if (versionString.Contains("2022")) return "SQL2022";
            if (versionString.Contains("2019")) return "SQL2019";
            if (versionString.Contains("2017")) return "SQL2017";
            if (versionString.Contains("2016")) return "SQL2016";
            if (versionString.Contains("2014")) return "SQL2014";
            if (versionString.Contains("2012")) return "SQL2012";
            if (versionString.Contains("2008 R2")) return "SQL2008R2";
            if (versionString.Contains("2008")) return "SQL2008";
            if (versionString.Contains("2005")) return "SQL2005";

            return "SQL"; // Default fallback
        }

        public async Task<List<DatabaseInfo>> GetDatabasesAndFilesAsync()
        {
            var databases = new List<DatabaseInfo>();
            const string query = "SELECT d.name, mf.physical_name FROM sys.databases d JOIN sys.master_files mf ON d.database_id = mf.database_id WHERE d.database_id > 4 ORDER BY d.name, mf.type_desc DESC;";

            using (var connection = await GetOpenConnectionAsync())
            using (var command = new SqlCommand(query, connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                DatabaseInfo currentDb = null;
                while (await reader.ReadAsync())
                {
                    string dbName = reader.GetString(0);
                    if (currentDb == null || currentDb.Name != dbName)
                    {
                        currentDb = new DatabaseInfo { Name = dbName };
                        databases.Add(currentDb);
                    }
                    if (currentDb.MdfPath == "N/A") currentDb.MdfPath = reader.GetString(1);
                    else currentDb.LdfPath = reader.GetString(1);
                }
            }
            return databases;
        }

        public async Task<bool> BackupDatabaseAsync(string databaseName, string backupFilePath, bool verify, IProgress<int> progress = null)
        {
            progress?.Report(5); // Initial progress
            string commandText = $"BACKUP DATABASE [{databaseName}] TO DISK = N'{backupFilePath}' WITH CHECKSUM, NOFORMAT, INIT, NAME = N'{databaseName}-Full Database Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10";
            await ExecuteNonQueryAsync(commandText, databaseName, useMaster: true, progress);

            if (verify)
            {
                progress?.Report(95); // Progress before starting verification
                string verifyCommand = $"RESTORE VERIFYONLY FROM DISK = N'{backupFilePath}' WITH CHECKSUM";
                await ExecuteNonQueryAsync(verifyCommand, databaseName, useMaster: true, null); // Verify doesn't report granular progress
            }
            progress?.Report(100);
            return true;
        }

        public async Task<bool> DeleteDatabaseAsync(string databaseName)
        {
            string commandText = $"DROP DATABASE [{databaseName}]";
            await ExecuteNonQueryAsync(commandText, databaseName, useMaster: true);
            return true;
        }

        public async Task<bool> AttachDatabaseAsync(string dbName, string mdfPath, string ldfPath)
        {
            string commandText = $"CREATE DATABASE [{dbName}] ON (FILENAME = N'{mdfPath}'), (FILENAME = N'{ldfPath}') FOR ATTACH";
            await ExecuteNonQueryAsync(commandText, dbName, useMaster: true);
            return true;
        }

        public async Task<(string Output, bool HasErrors)> CheckDatabaseAsync(string databaseName)
        {
            var output = new StringBuilder();
            bool hasErrors = false;

            using (var connection = await GetOpenConnectionAsync())
            {
                connection.InfoMessage += (sender, e) => {
                    output.AppendLine(e.Message);
                };

                using (var command = new SqlCommand($"DBCC CHECKDB ('{databaseName}') WITH ALL_ERRORMSGS, NO_INFOMSGS", connection))
                {
                    command.CommandTimeout = 1800;
                    await command.ExecuteNonQueryAsync();
                }
            }

            string resultText = output.ToString();
            if (string.IsNullOrWhiteSpace(resultText) || resultText.Contains("0 allocation errors and 0 consistency errors"))
            {
                resultText = $"DBCC CHECKDB for '{databaseName}' completed without errors.";
                hasErrors = false;
            }
            else
            {
                hasErrors = true;
            }

            return (resultText, hasErrors);
        }

        public async Task<bool> DetachDatabaseAsync(string databaseName)
        {
            string commandText = $"EXEC sp_detach_db '{databaseName}', 'true'";
            await ExecuteNonQueryAsync(commandText, databaseName, useMaster: true);
            return true;
        }

        public async Task<bool> RestoreDatabaseAsync(string databaseName, string backupFilePath, string newMdfPath, string newLdfPath, IProgress<int> progress = null)
        {
            progress?.Report(5);
            var fileList = await GetBackupFileListAsync(backupFilePath);
            if (fileList == null || fileList.Count < 2)
            {
                throw new InvalidOperationException("Could not read the logical file names from the backup set.");
            }

            var mdfLogicalName = fileList.First(f => f.Type == "D").LogicalName;
            var ldfLogicalName = fileList.First(f => f.Type == "L").LogicalName;

            string commandText = $@"
                RESTORE DATABASE [{databaseName}]
                FROM DISK = N'{backupFilePath}'
                WITH
                    MOVE N'{mdfLogicalName}' TO N'{newMdfPath}',
                    MOVE N'{ldfLogicalName}' TO N'{newLdfPath}',
                    REPLACE,
                    STATS = 10";

            await ExecuteNonQueryAsync(commandText, databaseName, useMaster: true, progress);
            progress?.Report(100);
            return true;
        }

        public async Task<string> GetOriginalDatabaseNameAsync(string backupFilePath)
        {
            const string query = "RESTORE HEADERONLY FROM DISK = @backupFilePath";
            using (var connection = await GetOpenConnectionAsync())
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@backupFilePath", backupFilePath);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return reader["DatabaseName"] as string;
                    }
                }
            }
            return null;
        }

        public async Task<List<(string LogicalName, string Type)>> GetBackupFileListAsync(string backupFilePath)
        {
            var fileList = new List<(string, string)>();
            string query = $"RESTORE FILELISTONLY FROM DISK = N'{backupFilePath}'";

            using (var connection = await GetOpenConnectionAsync())
            using (var command = new SqlCommand(query, connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    fileList.Add((reader["LogicalName"].ToString(), reader["Type"].ToString()));
                }
            }
            return fileList;
        }

        public async Task<DbDetails> GetDatabaseDetailsAsync(string databaseName)
        {
            var details = new DbDetails();
            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                InitialCatalog = databaseName
            };

            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    string versionQuery = "SELECT TOP 1 CAST(Major AS VARCHAR) + '.' + CAST(Minor AS VARCHAR) + '.' + CAST(Build AS VARCHAR) AS Result FROM fmk.Version ORDER BY VersionID DESC;";
                    using (var versionCmd = new SqlCommand(versionQuery, connection))
                    {
                        var result = await versionCmd.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                        {
                            details.DataVersion = result.ToString();
                        }
                    }

                    string companyQuery = "SELECT Value FROM fmk.Configuration WHERE [Key] = N'CompanyName'";
                    using (var companyCmd = new SqlCommand(companyQuery, connection))
                    {
                        var result = await companyCmd.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                        {
                            details.CompanyName = result.ToString();
                        }
                    }

                    // #9 Get ActivationCode
                    string activationCodeQuery = "SELECT value FROM sys.extended_properties where name = 'ActivationCode'";
                    using (var activationCmd = new SqlCommand(activationCodeQuery, connection))
                    {
                        var result = await activationCmd.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                        {
                            details.ActivationCode = result.ToString();
                        }
                    }

                    try
                    {
                        string sepidarQuery = "SELECT Title FROM fmk.fiscalyear";
                        using (var fiscalCmd = new SqlCommand(sepidarQuery, connection))
                        using (var reader = await fiscalCmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                details.FiscalYears.Add(reader.GetString(0));
                            }
                            details.DbType = DatabaseType.Sepidar;
                        }
                    }
                    catch (SqlException)
                    {
                        try
                        {
                            string dashtQuery = "SELECT Title FROM fmk.fiscalperiod";
                            using (var fiscalCmd = new SqlCommand(dashtQuery, connection))
                            using (var reader = await fiscalCmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    details.FiscalYears.Add(reader.GetString(0));
                                }
                                details.DbType = DatabaseType.Dasht;
                            }
                        }
                        catch (SqlException)
                        {
                            details.DbType = DatabaseType.Unknown;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"INFO: Could not retrieve details for {databaseName}. Error: {ex.Message}");
                }
            }

            return details;
        }
    }
}
