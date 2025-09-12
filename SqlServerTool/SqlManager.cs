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
        public string UserAccessMode { get; set; } = "N/A";
    }

    /// <summary>
    /// Represents details of a single SQL trigger.
    /// </summary>
    public class TriggerInfo
    {
        public string Name { get; set; }
        public string Table { get; set; }
        public string Event { get; set; }
        public string Status { get; set; }
        public override string ToString() => $"'{Name}' on table '{Table}' (Event: {Event}, Status: {Status})";
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
            else
            {
                // If not using master, use the specified database context
                builder.InitialCatalog = dbName;
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

                // This block handles setting SINGLE_USER and MULTI_USER modes
                // for operations like DROP, DETACH, and RESTORE
                bool needsSingleUserMode = commandText.Trim().StartsWith("DROP", StringComparison.OrdinalIgnoreCase) ||
                                           commandText.IndexOf("sp_detach_db", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                           (commandText.Trim().StartsWith("RESTORE", StringComparison.OrdinalIgnoreCase) && !commandText.Trim().StartsWith("RESTORE VERIFYONLY", StringComparison.OrdinalIgnoreCase));

                if (needsSingleUserMode)
                {
                    // For drop/detach/restore, we need to be in master context to kick other users off
                    var masterBuilder = new SqlConnectionStringBuilder(_connectionString) { InitialCatalog = "master" };
                    using (var masterConnection = new SqlConnection(masterBuilder.ConnectionString))
                    {
                        await masterConnection.OpenAsync();
                        // Attempt to set to SINGLE_USER. Use a try-catch to ensure we always try to revert.
                        try
                        {
                            using (var singleUserCmd = new SqlCommand($"ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", masterConnection))
                            {
                                await singleUserCmd.ExecuteNonQueryAsync();
                            }
                        }
                        catch (SqlException ex)
                        {
                            // Log or handle if setting to single user fails (e.g., db doesn't exist)
                            // For detach/drop, it might mean the db is already gone.
                            // For restore, it might fail if db doesn't exist for REPLACE.
                            // We'll let the main command try and fail if this was fatal.
                            Console.WriteLine($"Warning: Could not set database {dbName} to SINGLE_USER: {ex.Message}");
                        }
                    }
                }

                using (var command = new SqlCommand(commandText, connection))
                {
                    command.CommandTimeout = 600; // 10 minutes for long operations
                    await command.ExecuteNonQueryAsync();
                }

                // If the command was a RESTORE or a successful DETACH, try to set it back to MULTI_USER
                bool wasRestore = commandText.Trim().StartsWith("RESTORE", StringComparison.OrdinalIgnoreCase) && !commandText.Trim().StartsWith("RESTORE VERIFYONLY", StringComparison.OrdinalIgnoreCase);

                if (wasRestore)
                {
                    var masterBuilder = new SqlConnectionStringBuilder(_connectionString) { InitialCatalog = "master" };
                    using (var masterConnection = new SqlConnection(masterBuilder.ConnectionString))
                    {
                        await masterConnection.OpenAsync();
                        try
                        {
                            using (var multiUserCmd = new SqlCommand($"ALTER DATABASE [{dbName}] SET MULTI_USER", masterConnection))
                            {
                                await multiUserCmd.ExecuteNonQueryAsync();
                            }
                        }
                        catch (SqlException ex)
                        {
                            Console.WriteLine($"Warning: Could not set database {dbName} to MULTI_USER after restore: {ex.Message}");
                        }
                    }
                }
                // For detach, we don't set it back to MULTI_USER because the database is no longer attached.
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
            string commandText;
            // If the LDF path is provided and the file actually exists, we use it.
            if (!string.IsNullOrWhiteSpace(ldfPath) && File.Exists(ldfPath))
            {
                commandText = $"CREATE DATABASE [{dbName}] ON (FILENAME = N'{mdfPath}'), (FILENAME = N'{ldfPath}') FOR ATTACH";
            }
            else
            {
                // If the LDF path is missing, or the file doesn't exist at that path,
                // instruct SQL Server to attach the MDF and build a new log file.
                // This is the standard way to recover from a missing or corrupt log file.
                commandText = $"CREATE DATABASE [{dbName}] ON (FILENAME = N'{mdfPath}') FOR ATTACH_REBUILD_LOG";
            }

            await ExecuteNonQueryAsync(commandText, dbName, useMaster: true);
            return true;
        }

        public async Task<(string Output, bool HasErrors)> CheckDatabaseAsync(string databaseName)
        {
            var output = new StringBuilder();
            bool hasErrors = false;

            var builder = new SqlConnectionStringBuilder(_connectionString) { InitialCatalog = databaseName };
            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.InfoMessage += (sender, e) => {
                    output.AppendLine(e.Message);
                };
                await connection.OpenAsync();
                using (var command = new SqlCommand($"DBCC CHECKDB ('{databaseName}') WITH ALL_ERRORMSGS, NO_INFOMSGS", connection))
                {
                    command.CommandTimeout = 1800; // 30 minutes
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
            var masterBuilder = new SqlConnectionStringBuilder(_connectionString) { InitialCatalog = "master" };

            using (var connection = new SqlConnection(masterBuilder.ConnectionString))
            {
                await connection.OpenAsync();
                try
                {
                    string killConnectionsSql = $"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                    using (var command = new SqlCommand(killConnectionsSql, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }

                    string detachSql = $"EXEC sp_detach_db '{databaseName}', 'true'";
                    using (var command = new SqlCommand(detachSql, connection))
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        string setMultiUserSql = $"ALTER DATABASE [{databaseName}] SET MULTI_USER";
                        using (var command = new SqlCommand(setMultiUserSql, connection))
                        {
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                    catch (Exception multiUserEx)
                    {
                        Console.WriteLine($"Failed to set database back to MULTI_USER mode: {multiUserEx.Message}");
                    }
                    throw;
                }
            }
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

            // Get User Access mode first, as it can be done on the master connection
            try
            {
                details.UserAccessMode = await GetUserAccessModeAsync(databaseName);
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Could not get user access mode for {databaseName}: {ex.Message}");
                details.UserAccessMode = "Error";
            }


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
                catch (SqlException)
                {
                    // Don't pollute console for routine checks, just return details as-is
                }
            }
            return details;
        }

        public async Task<string> GetUserAccessModeAsync(string databaseName)
        {
            const string query = "SELECT DATABASEPROPERTYEX(@dbName, 'UserAccess')";
            using (var connection = await GetOpenConnectionAsync()) // Uses master by default
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@dbName", databaseName);
                var result = await command.ExecuteScalarAsync();
                return result?.ToString() ?? "N/A";
            }
        }

        public async Task<(bool IsMatch, string Message)> CheckDatabaseSchemaAsync(string databaseName)
        {
            var details = await GetDatabaseDetailsAsync(databaseName);
            if (details.DbType == DatabaseType.Unknown)
            {
                return (false, $"Could not determine the database type (Sepidar/Dasht) for '{databaseName}'. Schema check aborted.");
            }

            var dashtSchemas = new HashSet<string> { "ACC", "FMK", "GNR", "JWL", "MSG", "POS", "PROP", "SCD" };
            var sepidarSchemas = new HashSet<string> { "ACC", "AST", "CNT", "DST", "FMK", "GNR", "INV", "MRP", "MSG", "PAY", "POM", "POS", "RPA", "SCD", "SLS", "WKO" };
            var expectedSchemas = details.DbType == DatabaseType.Dasht ? dashtSchemas : sepidarSchemas;

            var actualSchemas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string query = @"
                SELECT t.TABLE_SCHEMA
                FROM INFORMATION_SCHEMA.TABLES AS t
                WHERE t.TABLE_TYPE = 'BASE TABLE'
                GROUP BY t.TABLE_SCHEMA
                ORDER BY t.TABLE_SCHEMA;";

            var builder = new SqlConnectionStringBuilder(_connectionString) { InitialCatalog = databaseName };

            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        actualSchemas.Add(reader.GetString(0));
                    }
                }
            }

            actualSchemas.Remove("INFORMATION_SCHEMA");
            actualSchemas.Remove("sys");


            if (actualSchemas.SetEquals(expectedSchemas))
            {
                return (true, $"Schema for '{databaseName}' ({details.DbType}) is correct.{Environment.NewLine}All {expectedSchemas.Count} expected schemas were found.");
            }
            else
            {
                var missing = expectedSchemas.Except(actualSchemas);
                var extra = actualSchemas.Except(expectedSchemas);
                var message = new StringBuilder();
                message.AppendLine($"Schema mismatch found for '{databaseName}' ({details.DbType}):");
                if (missing.Any())
                {
                    message.AppendLine($"- Missing schemas: {string.Join(", ", missing)}");
                }
                if (extra.Any())
                {
                    message.AppendLine($"- Unexpected extra schemas: {string.Join(", ", extra)}");
                }
                return (false, message.ToString());
            }
        }

        public async Task<(bool IsOk, string Message)> CheckDatabaseTriggersAsync(string databaseName)
        {
            var details = await GetDatabaseDetailsAsync(databaseName);
            if (details.DbType == DatabaseType.Unknown)
            {
                return (false, $"Could not determine the database type (Sepidar/Dasht) for '{databaseName}'. Trigger check aborted.");
            }

            var foundTriggers = new List<TriggerInfo>();
            string query = @"
                SELECT
                    trg.name AS trigger_name,
                    ISNULL(SCHEMA_NAME(tab.schema_id) + '.' + tab.name, 'DATABASE') AS [table],
                    (CASE WHEN OBJECTPROPERTY(trg.object_id, 'ExecIsUpdateTrigger') = 1 THEN 'Update ' ELSE '' END +
                     CASE WHEN OBJECTPROPERTY(trg.object_id, 'ExecIsDeleteTrigger') = 1 THEN 'Delete ' ELSE '' END +
                     CASE WHEN OBJECTPROPERTY(trg.object_id, 'ExecIsInsertTrigger') = 1 THEN 'Insert' ELSE '' END
                    ) AS [event],
                    CASE WHEN is_disabled = 1 THEN 'Disabled' ELSE 'Active' END AS [status]
                FROM sys.triggers trg
                LEFT JOIN sys.objects tab ON trg.parent_id = tab.object_id
                WHERE trg.is_ms_shipped = 0
                ORDER BY trg.name;";

            var builder = new SqlConnectionStringBuilder(_connectionString) { InitialCatalog = databaseName };

            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        foundTriggers.Add(new TriggerInfo
                        {
                            Name = reader["trigger_name"] as string,
                            Table = reader["table"] as string,
                            Event = (reader["event"] as string)?.Trim(),
                            Status = reader["status"] as string
                        });
                    }
                }
            }

            if (details.DbType == DatabaseType.Sepidar)
            {
                if (foundTriggers.Any())
                {
                    var message = new StringBuilder();
                    message.AppendLine($"RED FLAG: Sepidar database '{databaseName}' should not have any triggers, but {foundTriggers.Count} were found:");
                    foreach (var trigger in foundTriggers)
                    {
                        message.AppendLine($"- {trigger}");
                    }
                    return (false, message.ToString());
                }
                else
                {
                    return (true, $"SUCCESS: No triggers found in Sepidar database '{databaseName}', as expected.");
                }
            }
            else // Dasht
            {
                var allowedTriggers = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "ItemChangeVersion",
                    "ItemGroupChangeVersion",
                    "ItemGroupItemChangeVersion",
                    "ItemSalePriceChangeVersion",
                    "ItemSubUnitChangeVersion",
                    "PartyChangeVersion",
                    "UserChangeVersion"
                };

                var unexpectedTriggers = foundTriggers.Where(t => !allowedTriggers.Contains(t.Name)).ToList();

                if (unexpectedTriggers.Any())
                {
                    var message = new StringBuilder();
                    message.AppendLine($"WARNING: Found {unexpectedTriggers.Count} unexpected trigger(s) in Dasht database '{databaseName}':");
                    foreach (var trigger in unexpectedTriggers)
                    {
                        message.AppendLine($"- {trigger}");
                    }
                    return (false, message.ToString());
                }
                else
                {
                    return (true, $"SUCCESS: All {foundTriggers.Count} triggers found in Dasht database '{databaseName}' are standard system triggers.");
                }
            }
        }
    }
}
