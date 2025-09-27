using Microsoft.Data.Sql;
using Microsoft.Data.SqlClient; // Required for ClearAllPools()
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlServerTool
{
    public partial class Form1 : Form
    {
        private SqlManager _sqlManager;
        private List<DatabaseInfo> _databases = new List<DatabaseInfo>();
        private string _sqlShortVersion = "SQL";

        public Form1()
        {
            InitializeComponent();
            this.Text = $"SQL Server Utility v{Assembly.GetExecutingAssembly().GetName().Version}";

            _txtServer.Text = ".\\Sepidar";
            _txtUser.Text = "sa";
            _txtPassword.Text = "1";

            _rbSqlAuth.Checked = true;
            _rbWindowsAuth.CheckedChanged += new EventHandler(_rbAuth_CheckedChanged);
            _rbSqlAuth.CheckedChanged += new EventHandler(_rbAuth_CheckedChanged);

            _progressBar.Style = ProgressBarStyle.Continuous;

            UpdateAuthControls();
            SetDisconnectedState();
            DisplayClientSystemInfo();

            this.Load += new System.EventHandler(this.Form1_Load);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Activate();
        }

        private void DisplayClientSystemInfo()
        {
            try
            {
                var sb = new StringBuilder();

                // Get CPU information
                using (var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor"))
                using (var collection = searcher.Get())
                {
                    using (var cpu = collection.Cast<ManagementObject>().FirstOrDefault())
                    {
                        sb.AppendLine($"CPU: {cpu?["Name"]?.ToString() ?? "N/A"}");
                    }
                }

                // Get Installed RAM
                using (var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem"))
                using (var collection = searcher.Get())
                {
                    using (var system = collection.Cast<ManagementObject>().FirstOrDefault())
                    {
                        if (system != null && ulong.TryParse(system["TotalPhysicalMemory"]?.ToString(), out ulong totalRamBytes))
                        {
                            sb.AppendLine($"Installed RAM: {Math.Round(totalRamBytes / 1073741824.0, 2)} GB");
                        }
                        else
                        {
                            sb.AppendLine("Installed RAM: N/A");
                        }
                    }
                }

                // Get Graphics Card information
                var gpuNames = new List<string>();
                using (var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController"))
                using (var collection = searcher.Get())
                {
                    foreach (var gpu in collection.Cast<ManagementObject>())
                    {
                        using (gpu)
                        {
                            gpuNames.Add(gpu["Name"]?.ToString() ?? "N/A");
                        }
                    }
                }
                sb.AppendLine($"Graphics Card(s): {(gpuNames.Any() ? string.Join(", ", gpuNames) : "N/A")}");


                // Get Storage information
                var storageDetails = new List<string>();
                using (var searcher = new ManagementObjectSearcher("SELECT Model, Size FROM Win32_DiskDrive"))
                using (var collection = searcher.Get())
                {
                    foreach (var drive in collection.Cast<ManagementObject>())
                    {
                        using (drive)
                        {
                            if (double.TryParse(drive["Size"]?.ToString(), out double sizeInBytes))
                            {
                                storageDetails.Add($"{drive["Model"]} ({Math.Round(sizeInBytes / 1073741824.0, 2)} GB)");
                            }
                        }
                    }
                }
                sb.AppendLine($"Storage: {(storageDetails.Any() ? string.Join(" | ", storageDetails) : "N/A")}");


                // Get System Type
                using (var searcher = new ManagementObjectSearcher("SELECT OSArchitecture, Caption FROM Win32_OperatingSystem"))
                using (var collection = searcher.Get())
                {
                    using (var os = collection.Cast<ManagementObject>().FirstOrDefault())
                    {
                        sb.AppendLine($"System Type: {os?["OSArchitecture"]?.ToString() ?? "N/A"}");
                        sb.AppendLine($"OS: {os?["Caption"]?.ToString() ?? "N/A"}");
                    }
                }

                _txtSystemDetails.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                _txtSystemDetails.Text = "Could not retrieve client system information.";
                Log($"Could not get client system info: {ex.Message}");
            }
        }


        private void Log(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            if (_rtbLog.InvokeRequired)
            {
                _rtbLog.Invoke(new Action(() => _rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}")));
            }
            else
            {
                _rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
                _rtbLog.ScrollToCaret();
            }
        }

        private void UpdateAuthControls()
        {
            bool sqlAuthSelected = _rbSqlAuth.Checked;
            _lblUser.Enabled = sqlAuthSelected;
            _txtUser.Enabled = sqlAuthSelected;
            _lblPassword.Enabled = sqlAuthSelected;
            _txtPassword.Enabled = sqlAuthSelected;
        }

        private void ToggleOperationControls(bool isEnabled)
        {
            _tabControl.Enabled = isEnabled;
        }

        private void ShowProgress(string initialMessage)
        {
            Log(initialMessage);
            _progressBar.Value = 0;
            _progressBar.Visible = true;
            this.Cursor = Cursors.WaitCursor;
            ToggleOperationControls(false);
            _gbConnection.Enabled = false;
        }

        private void HideProgress()
        {
            _progressBar.Value = 0;
            _progressBar.Visible = false;
            this.Cursor = Cursors.Default;
            var isConnected = _sqlManager != null;
            ToggleOperationControls(isConnected);
            _gbConnection.Enabled = true;
        }

        private void SetConnectedState()
        {
            _txtServer.Enabled = false;
            _btnBrowseServers.Enabled = false;
            _rbWindowsAuth.Enabled = false;
            _rbSqlAuth.Enabled = false;
            _txtUser.Enabled = false;
            _txtPassword.Enabled = false;
            _btnAbout.Enabled = true;

            _btnConnect.Visible = false;
            _btnDisconnect.Visible = true;

            ToggleOperationControls(true);
        }

        private void SetDisconnectedState()
        {
            // FIX: Clear the connection pool for a definitive disconnect.
            if (_sqlManager != null)
            {
                SqlConnection.ClearAllPools();
            }

            _sqlManager = null;
            _txtServer.Enabled = true;
            _btnBrowseServers.Enabled = true;
            _rbWindowsAuth.Enabled = true;
            _rbSqlAuth.Enabled = true;
            UpdateAuthControls();
            _btnAbout.Enabled = true;

            _btnConnect.Visible = true;
            _btnDisconnect.Visible = false;

            _lblSqlVersion.Text = "SQL Version: N/A";
            _toolTip.SetToolTip(_lblSqlVersion, "");

            // Clear server hardware info but keep client info
            DisplayClientSystemInfo();

            _lbDatabases.DataSource = null;
            _lbFiscalYears.DataSource = null;
            _txtMdfPath.Clear();
            _txtLdfPath.Clear();
            _lblMdfDrive.Text = "";
            _lblLdfDrive.Text = "";
            _lblDbVersion.Text = "Data Version: N/A";
            _lblCompanyName.Text = "Company: N/A";
            _lblDbType.Text = "Type: N/A";
            _lblDbSize.Text = "Size: N/A";
            _lblActivationCode.Text = "Activation Code: N/A";
            _lblUserAccessMode.Text = "Access Mode: N/A";
            _lblUserAccessMode.ForeColor = SystemColors.ControlText;
            _lblServerName.Text = "Server Name: N/A";
            _lblServiceName.Text = "Service Name: N/A";
            _lblConnections.Text = "Connections: N/A";
            _lblTranCount.Text = "Tran Count: N/A";
            _lblLanguage.Text = "Language: N/A";
            _lblCollation.Text = "Collation: N/A";

            _cmbDatabasesQuery.DataSource = null;
            _dgvResults.DataSource = null;
            _txtQuery.Clear();

            ToggleOperationControls(false);
            HideProgress();
            Log("Disconnected.");
        }

        private async void _btnConnect_Click(object sender, EventArgs e)
        {
            string connectionString;
            if (_rbWindowsAuth.Checked)
            {
                connectionString = $"Server={_txtServer.Text};Database=master;Trusted_Connection=True;TrustServerCertificate=True;";
            }
            else
            {
                connectionString = $"Server={_txtServer.Text};Database=master;User Id={_txtUser.Text};Password={_txtPassword.Text};TrustServerCertificate=True;";
            }

            Log($"Connecting to {_txtServer.Text}...");
            this.Cursor = Cursors.WaitCursor;
            _gbConnection.Enabled = false;

            try
            {
                _sqlManager = new SqlManager(connectionString);

                var (version, memory, cpus) = await _sqlManager.GetServerSystemInfoAsync();
                _sqlShortVersion = await _sqlManager.GetSqlShortVersionAsync();

                string singleLineVersion = string.Join(" ", version.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
                _lblSqlVersion.Text = $"SQL Version: {singleLineVersion}";
                _toolTip.SetToolTip(_lblSqlVersion, version);

                string serverInfo = $"Server Hardware: {cpus} CPUs | {memory:N0} MB RAM";

                // Rerun client info to have a clean slate before prepending server info
                DisplayClientSystemInfo();
                _txtSystemDetails.Text = serverInfo + Environment.NewLine + "---" + Environment.NewLine + _txtSystemDetails.Text;

                Log("Connection successful.");

                SetConnectedState();
                await RefreshDatabaseList();
            }
            catch (Exception ex)
            {
                Log($"ERROR: {ex.Message}");
                MessageBox.Show($"Connection Failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetDisconnectedState();
            }
            finally
            {
                this.Cursor = Cursors.Default;
                if (_sqlManager == null)
                {
                    _gbConnection.Enabled = true;
                }
            }
        }


        private void _btnDisconnect_Click(object sender, EventArgs e)
        {
            SetDisconnectedState();
        }

        private async void _btnBrowseServers_Click(object sender, EventArgs e)
        {
            Log("Searching for local and network SQL Server instances...");
            this.Cursor = Cursors.WaitCursor;
            _gbConnection.Enabled = false;

            try
            {
                DataTable sqlSources = await Task.Run(() => SqlDataSourceEnumerator.Instance.GetDataSources());

                if (sqlSources.Rows.Count == 0)
                {
                    MessageBox.Show("No SQL Server instances found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var servers = new List<string>();
                foreach (DataRow row in sqlSources.Rows)
                {
                    string serverName = row["ServerName"].ToString();
                    string instanceName = row["InstanceName"].ToString();
                    if (!string.IsNullOrEmpty(instanceName))
                    {
                        servers.Add($"{serverName}\\{instanceName}");
                    }
                    else
                    {
                        servers.Add(serverName);
                    }
                }

                using (var selectionForm = new Form())
                {
                    selectionForm.Text = "Select SQL Server Instance";
                    selectionForm.StartPosition = FormStartPosition.CenterParent;
                    selectionForm.Size = new System.Drawing.Size(300, 400);

                    var listBox = new ListBox { Dock = DockStyle.Fill };
                    listBox.DataSource = servers;
                    selectionForm.Controls.Add(listBox);

                    var selectButton = new Button { Text = "Select", Dock = DockStyle.Bottom };
                    selectButton.Click += (s, args) =>
                    {
                        if (listBox.SelectedItem != null)
                        {
                            _txtServer.Text = listBox.SelectedItem.ToString();
                        }
                        selectionForm.Close();
                    };
                    selectionForm.Controls.Add(selectButton);
                    selectionForm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                Log($"ERROR browsing for SQL instances: {ex.Message}");
                MessageBox.Show($"Could not retrieve SQL Server instances: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                _gbConnection.Enabled = true;
                Log("Finished searching for SQL Server instances.");
            }
        }

        private async Task RefreshDatabaseList()
        {
            if (_sqlManager == null) return;
            Log("Refreshing database list...");

            var selectedDbName = _lbDatabases.SelectedItem?.ToString();

            _lbDatabases.DataSource = null;
            _lbFiscalYears.DataSource = null;
            _cmbDatabasesQuery.DataSource = null;

            _txtMdfPath.Clear();
            _txtLdfPath.Clear();
            _lblMdfDrive.Text = "";
            _lblLdfDrive.Text = "";
            _lblDbVersion.Text = "Data Version: N/A";
            _lblCompanyName.Text = "Company: N/A";
            _lblDbType.Text = "Type: N/A";
            _lblDbSize.Text = "Size: N/A";
            _lblActivationCode.Text = "Activation Code: N/A";
            _lblUserAccessMode.Text = "Access Mode: N/A";
            _lblUserAccessMode.ForeColor = SystemColors.ControlText;
            _lblServerName.Text = "Server Name: N/A";
            _lblServiceName.Text = "Service Name: N/A";
            _lblConnections.Text = "Connections: N/A";
            _lblTranCount.Text = "Tran Count: N/A";
            _lblLanguage.Text = "Language: N/A";
            _lblCollation.Text = "Collation: N/A";

            _databases = await _sqlManager.GetDatabasesAndFilesAsync();

            foreach (var db in _databases)
            {
                if (!string.IsNullOrEmpty(db.MdfDrive))
                {
                    db.MdfDriveModel = SystemInfoHelper.GetDriveModel(db.MdfDrive[0]);
                }
                if (!string.IsNullOrEmpty(db.LdfDrive))
                {
                    db.LdfDriveModel = SystemInfoHelper.GetDriveModel(db.LdfDrive[0]);
                }
            }

            _lbDatabases.DataSource = _databases;
            _lbDatabases.DisplayMember = "Name";
            _cmbDatabasesQuery.DataSource = new List<DatabaseInfo>(_databases);
            _cmbDatabasesQuery.DisplayMember = "Name";

            if (selectedDbName != null)
            {
                var dbToSelect = _databases.FirstOrDefault(db => db.Name == selectedDbName);
                if (dbToSelect != null)
                {
                    _lbDatabases.SelectedItem = dbToSelect;
                }
            }

            Log($"Found {_databases.Count} databases.");
        }

        private async void _btnBackup_Click(object sender, EventArgs e)
        {
            await PerformBackup(verify: false);
        }

        private async void _btnBackupVerifyChecksum_Click(object sender, EventArgs e)
        {
            await PerformBackup(verify: true);
        }

        private async Task PerformBackup(bool verify)
        {
            if (_lbDatabases.SelectedItem == null)
            {
                MessageBox.Show("Please select a database first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dbInfo = (DatabaseInfo)_lbDatabases.SelectedItem;
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Backup File (*.bak)|*.bak";
                sfd.FileName = $"{dbInfo.Name}_{DateTime.Now:yyyyMMdd_HHmmss}_{_sqlShortVersion}.bak";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string action = verify ? "Backing up and verifying" : "Backing up";
                    var progress = new Progress<int>(percent => { _progressBar.Value = Math.Min(100, percent); });
                    ShowProgress($"{action} {dbInfo.Name} to {sfd.FileName}...");

                    try
                    {
                        await _sqlManager.BackupDatabaseAsync(dbInfo.Name, sfd.FileName, verify, progress);
                        string successMsg = verify ? "Backup and verification completed successfully!" : "Backup completed successfully!";
                        Log(successMsg);
                        MessageBox.Show(successMsg, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        Log($"ERROR: {ex.Message}");
                        MessageBox.Show($"Backup Failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        HideProgress();
                    }
                }
            }
        }

        private async void _btnDelete_Click(object sender, EventArgs e)
        {
            if (_lbDatabases.SelectedItem == null)
            {
                MessageBox.Show("Please select a database first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dbInfo = (DatabaseInfo)_lbDatabases.SelectedItem;

            string prompt = $"This will permanently delete the database '{dbInfo.Name}'. This action cannot be undone.\n\nPlease type 'Delete' into the box below to confirm.";
            string title = "Confirm Delete";
            var (result, confirmation) = InputBoxForm.Show(prompt, title);


            if (result == DialogResult.OK && confirmation.Equals("Delete", StringComparison.OrdinalIgnoreCase))
            {
                ShowProgress($"Deleting database {dbInfo.Name}...");
                try
                {
                    await _sqlManager.DeleteDatabaseAsync(dbInfo.Name);
                    Log("Database deleted successfully.");
                    await RefreshDatabaseList();
                }
                catch (Exception ex)
                {
                    Log($"ERROR: {ex.Message}");
                    MessageBox.Show($"Delete Failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    HideProgress();
                }
            }
            else if (result == DialogResult.OK)
            {
                Log($"Incorrect confirmation text entered. Database '{dbInfo.Name}' was not deleted.");
            }
            else
            {
                Log("Database deletion cancelled by user.");
            }
        }

        private async void _btnCheckDb_Click(object sender, EventArgs e)
        {
            if (_lbDatabases.SelectedItem == null)
            {
                MessageBox.Show("Please select a database first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dbInfo = (DatabaseInfo)_lbDatabases.SelectedItem;
            ShowProgress($"Running DBCC CHECKDB for {dbInfo.Name}. This may take a while...");
            try
            {
                var (output, hasErrors) = await _sqlManager.CheckDatabaseAsync(dbInfo.Name);

                Log($"--- DBCC Results for {dbInfo.Name} ---");
                Log(output);
                Log("--- End of Results ---");

                if (hasErrors)
                {
                    MessageBox.Show($"WARNING: DBCC CHECKDB found errors in '{dbInfo.Name}'. The database may be corrupted. Check the log for details.", "Health Check: FAILED", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show($"SUCCESS: DBCC CHECKDB found no errors in '{dbInfo.Name}'. The database is healthy.", "Health Check: Passed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Log($"ERROR: {ex.Message}");
                MessageBox.Show($"DBCC CHECKDB Failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                HideProgress();
            }
        }

        private async void _btnSchemaCheck_Click(object sender, EventArgs e)
        {
            if (_lbDatabases.SelectedItem == null)
            {
                MessageBox.Show("Please select a database first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dbInfo = (DatabaseInfo)_lbDatabases.SelectedItem;
            ShowProgress($"Checking table schema for {dbInfo.Name}...");
            try
            {
                var (isMatch, message) = await _sqlManager.CheckDatabaseSchemaAsync(dbInfo.Name);
                Log(message.Replace(Environment.NewLine, " "));

                if (isMatch)
                {
                    MessageBox.Show(message, "Schema Check: Passed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(message, "Schema Check: Mismatch Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Log($"ERROR: Schema check failed. {ex.Message}");
                MessageBox.Show($"Schema check failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                HideProgress();
            }
        }

        private async void _btnCheckTriggers_Click(object sender, EventArgs e)
        {
            if (_lbDatabases.SelectedItem == null)
            {
                MessageBox.Show("Please select a database first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dbInfo = (DatabaseInfo)_lbDatabases.SelectedItem;
            ShowProgress($"Checking triggers for {dbInfo.Name}...");
            try
            {
                var (isOk, message) = await _sqlManager.CheckDatabaseTriggersAsync(dbInfo.Name);
                Log(message.Replace(Environment.NewLine, " "));

                if (isOk)
                {
                    MessageBox.Show(message, "Trigger Check: Passed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(message, "Trigger Check: Issues Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Log($"ERROR: Trigger check failed. {ex.Message}");
                MessageBox.Show($"Trigger check failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                HideProgress();
            }
        }

        private async void _btnDetach_Click(object sender, EventArgs e)
        {
            if (_lbDatabases.SelectedItem == null)
            {
                MessageBox.Show("Please select a database first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dbInfo = (DatabaseInfo)_lbDatabases.SelectedItem;

            string prompt = $"This will detach the database '{dbInfo.Name}'. The database files will not be deleted.\n\nPlease type 'Detach' into the box below to confirm.";
            string title = "Confirm Detach";
            var (result, confirmation) = InputBoxForm.Show(prompt, title);

            if (result == DialogResult.OK && confirmation.Equals("Detach", StringComparison.OrdinalIgnoreCase))
            {
                ShowProgress($"Detaching database {dbInfo.Name}...");
                try
                {
                    await _sqlManager.DetachDatabaseAsync(dbInfo.Name);
                    Log("Database detached successfully.");
                    await RefreshDatabaseList();
                }
                catch (Exception ex)
                {
                    Log($"ERROR: {ex.Message}");
                    MessageBox.Show($"Detach Failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    HideProgress();
                }
            }
            else if (result == DialogResult.OK)
            {
                Log($"Incorrect confirmation text entered. Database '{dbInfo.Name}' was not detached.");
            }
            else
            {
                Log("Database detach cancelled by user.");
            }
        }

        private async void _btnRestore_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Backup File (*.bak)|*.bak";
                ofd.Title = "Select a Database Backup File to Restore";
                if (ofd.ShowDialog() != DialogResult.OK) return;

                var progress = new Progress<int>(percent => { _progressBar.Value = Math.Min(100, percent); });
                ShowProgress($"Analyzing backup file: {ofd.FileName}");
                try
                {
                    string originalDbName = await _sqlManager.GetOriginalDatabaseNameAsync(ofd.FileName);
                    if (string.IsNullOrEmpty(originalDbName))
                    {
                        MessageBox.Show("Could not determine the database name from the backup file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        HideProgress();
                        return;
                    }

                    var (result, newDbName) = InputBoxForm.Show("Enter the new name for the restored database:", "Restore Database As", originalDbName);
                    if (result != DialogResult.OK || string.IsNullOrWhiteSpace(newDbName))
                    {
                        Log("Restore cancelled by user.");
                        HideProgress();
                        return;
                    }

                    string destinationFolder;
                    using (var fbd = new FolderBrowserDialog())
                    {
                        fbd.Description = "Select a folder to save the new MDF and LDF database files.";

                        string defaultPath = await _sqlManager.GetDefaultDataPathAsync();
                        if (!string.IsNullOrEmpty(defaultPath) && Directory.Exists(defaultPath))
                        {
                            fbd.SelectedPath = defaultPath;
                        }

                        if (fbd.ShowDialog() != DialogResult.OK)
                        {
                            Log("Restore cancelled by user.");
                            HideProgress();
                            return;
                        }
                        destinationFolder = fbd.SelectedPath;
                    }

                    string newMdfPath = Path.Combine(destinationFolder, $"{newDbName}.mdf");
                    string newLdfPath = Path.Combine(destinationFolder, $"{newDbName}_log.ldf");

                    Log($"Restoring database from {ofd.FileName} to {newDbName}...");
                    Log($"New MDF Path: {newMdfPath}");
                    Log($"New LDF Path: {newLdfPath}");

                    await _sqlManager.RestoreDatabaseAsync(newDbName, ofd.FileName, newMdfPath, newLdfPath, progress);

                    Log("Database restored successfully.");
                    MessageBox.Show("Database restored successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await RefreshDatabaseList();
                }
                catch (Exception ex)
                {
                    Log($"ERROR: {ex.Message}");
                    MessageBox.Show($"Restore Failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    HideProgress();
                }
            }
        }

        private async void _lbDatabases_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_lbDatabases.SelectedItem is DatabaseInfo dbInfo)
            {
                _txtMdfPath.Text = dbInfo.MdfPath;
                _txtLdfPath.Text = dbInfo.LdfPath;
                _lblMdfDrive.Text = $"[{dbInfo.MdfDriveModel}]";
                _lblLdfDrive.Text = $"[{dbInfo.LdfDriveModel}]";
                _lblDbSize.Text = $"Size: {dbInfo.SizeMb:N2} MB ({dbInfo.SizeMb / 1024.0:N2} GB)";

                _lblDbVersion.Text = "Data Version: Loading...";
                _lblCompanyName.Text = "Company: Loading...";
                _lblDbType.Text = "Type: Loading...";
                _lblActivationCode.Text = "Activation Code: Loading...";
                _lblUserAccessMode.Text = "Access Mode: Loading...";
                _lblUserAccessMode.ForeColor = SystemColors.ControlText;
                _lblServerName.Text = "Server Name: Loading...";
                _lblServiceName.Text = "Service Name: Loading...";
                _lblConnections.Text = "Connections: Loading...";
                _lblTranCount.Text = "Tran Count: Loading...";
                _lblLanguage.Text = "Language: Loading...";
                _lblCollation.Text = "Collation: Loading...";
                _lbFiscalYears.DataSource = null;

                try
                {
                    var details = await _sqlManager.GetDatabaseDetailsAsync(dbInfo.Name);
                    _lblDbVersion.Text = $"Data Version: {details.DataVersion}";
                    _lblCompanyName.Text = $"Company: {details.CompanyName}";
                    _lblDbType.Text = $"Type: {details.DbType}";
                    _lblActivationCode.Text = $"Activation Code: {details.ActivationCode}";
                    _lblUserAccessMode.Text = $"Access Mode: {details.UserAccessMode}";
                    _lblServerName.Text = $"Server Name: {details.ServerName}";
                    _lblServiceName.Text = $"Service Name: {details.ServiceName}";
                    _lblConnections.Text = $"Connections: {details.Connections}";
                    _lblTranCount.Text = $"Tran Count: {details.TranCount}";
                    _lblLanguage.Text = $"Language: {details.Language}";
                    _lblCollation.Text = $"Collation: {details.Collation}";

                    switch (details.UserAccessMode.ToUpper())
                    {
                        case "MULTI_USER": _lblUserAccessMode.ForeColor = Color.DarkGreen; break;
                        case "SINGLE_USER": _lblUserAccessMode.ForeColor = Color.OrangeRed; break;
                        case "RESTRICTED_USER": _lblUserAccessMode.ForeColor = Color.DarkOrange; break;
                        default: _lblUserAccessMode.ForeColor = SystemColors.ControlText; break;
                    }

                    _lbFiscalYears.DataSource = details.FiscalYears;
                }
                catch (Exception ex)
                {
                    Log($"ERROR: Could not get database details for {dbInfo.Name}. {ex.Message}");
                    _lblDbVersion.Text = "Data Version: Error";
                    _lblCompanyName.Text = "Company: Error";
                    _lblDbType.Text = "Type: Error";
                    _lblActivationCode.Text = "Activation Code: Error";
                    _lblUserAccessMode.Text = "Access Mode: Error";
                    _lblUserAccessMode.ForeColor = Color.Red;
                    _lblServerName.Text = "Server Name: Error";
                    _lblServiceName.Text = "Service Name: Error";
                    _lblConnections.Text = "Connections: Error";
                    _lblTranCount.Text = "Tran Count: Error";
                    _lblLanguage.Text = "Language: Error";
                    _lblCollation.Text = "Collation: Error";
                }
            }
        }

        private void _btnOpenDir_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_txtMdfPath.Text))
            {
                try
                {
                    string directory = Path.GetDirectoryName(_txtMdfPath.Text);
                    if (Directory.Exists(directory))
                    {
                        Process.Start("explorer.exe", directory);
                    }
                    else
                    {
                        MessageBox.Show($"Directory not found: {directory}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    Log($"ERROR: Could not open directory. {ex.Message}");
                }
            }
        }

        private void _btnBrowseMdf_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "MDF File (*.mdf)|*.mdf";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _txtAttachMdf.Text = ofd.FileName;
                    _txtAttachLdf.Text = Path.ChangeExtension(ofd.FileName, "_log.ldf");
                }
            }
        }

        private void _btnBrowseLdf_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "LDF File (*.ldf)|*.ldf";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _txtAttachLdf.Text = ofd.FileName;
                }
            }
        }

        private async void _btnAttach_Click(object sender, EventArgs e)
        {
            string mdfPath = _txtAttachMdf.Text;
            string ldfPath = _txtAttachLdf.Text;

            if (string.IsNullOrWhiteSpace(mdfPath))
            {
                MessageBox.Show("Please provide the path to the MDF file.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string dbName = Path.GetFileNameWithoutExtension(mdfPath);

            ShowProgress($"Attaching database {dbName}...");
            try
            {
                await _sqlManager.AttachDatabaseAsync(dbName, mdfPath, ldfPath);
                Log("Database attached successfully.");
                MessageBox.Show("Database attached successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await RefreshDatabaseList();
                _txtAttachMdf.Clear();
                _txtAttachLdf.Clear();
            }
            catch (Exception ex)
            {
                Log($"ERROR: {ex.Message}");
                MessageBox.Show($"Attach Failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                HideProgress();
            }
        }

        private void _rbAuth_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAuthControls();
        }

        private void _btnAbout_Click(object sender, EventArgs e)
        {
            using (var about = new AboutBox())
            {
                about.ShowDialog(this);
            }
        }

        // --- QUERY WORKBOOK EVENT HANDLERS ---

        private async void _btnExecuteQuery_Click(object sender, EventArgs e)
        {
            if (_sqlManager == null)
            {
                MessageBox.Show("Please connect to a server first.", "Not Connected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_cmbDatabasesQuery.SelectedItem == null)
            {
                MessageBox.Show("Please select a database to run the query against.", "No Database Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = string.IsNullOrWhiteSpace(_txtQuery.SelectedText) ? _txtQuery.Text : _txtQuery.SelectedText;
            if (string.IsNullOrWhiteSpace(query))
            {
                MessageBox.Show("Please enter a query to execute.", "Empty Query", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dbInfo = (DatabaseInfo)_cmbDatabasesQuery.SelectedItem;
            string dbName = dbInfo.Name;

            Log($"Executing query against {dbName}...");
            this.Cursor = Cursors.WaitCursor;
            _btnExecuteQuery.Enabled = false;
            try
            {
                DataTable result = await _sqlManager.ExecuteQueryAsync(query, dbName);
                _dgvResults.DataSource = result;
                Log($"Query executed successfully. {result.Rows.Count} rows returned.");
            }
            catch (Exception ex)
            {
                Log($"ERROR executing query: {ex.Message}");
                MessageBox.Show($"Query failed: {ex.Message}", "Query Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _dgvResults.DataSource = null;
            }
            finally
            {
                this.Cursor = Cursors.Default;
                _btnExecuteQuery.Enabled = true;
            }
        }

        private void _btnSaveQuery_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtQuery.Text))
            {
                MessageBox.Show("There is no query to save.", "Empty Query", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "SQL Query (*.sql)|*.sql|All Files (*.*)|*.*";
                sfd.Title = "Save Query As...";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(sfd.FileName, _txtQuery.Text);
                        Log($"Query saved to {sfd.FileName}");
                    }
                    catch (Exception ex)
                    {
                        Log($"ERROR saving query: {ex.Message}");
                        MessageBox.Show($"Could not save file: {ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void _btnOpenQuery_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "SQL Query (*.sql)|*.sql|All Files (*.*)|*.*";
                ofd.Title = "Open Query...";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _txtQuery.Text = File.ReadAllText(ofd.FileName);
                        Log($"Query loaded from {ofd.FileName}");
                    }
                    catch (Exception ex)
                    {
                        Log($"ERROR opening query: {ex.Message}");
                        MessageBox.Show($"Could not open file: {ex.Message}", "Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                if (_tabControl.SelectedTab == _tabQueryWorkbook && _btnExecuteQuery.Enabled)
                {
                    _btnExecuteQuery_Click(sender, e);
                    e.Handled = true;
                }
            }
        }
    }

    internal static class SystemInfoHelper
    {
        private static Dictionary<char, string> _driveModelCache = new Dictionary<char, string>();

        public static string GetDriveModel(char driveLetter)
        {
            driveLetter = char.ToUpper(driveLetter);
            if (_driveModelCache.ContainsKey(driveLetter))
            {
                return _driveModelCache[driveLetter];
            }

            try
            {
                string model = "N/A";
                using (var partitionSearcher = new ManagementObjectSearcher($"ASSOCIATORS OF {{Win32_LogicalDisk.DeviceID='{driveLetter}:'}} WHERE AssocClass = Win32_LogicalDiskToPartition"))
                using (var partition = partitionSearcher.Get().Cast<ManagementObject>().FirstOrDefault())
                {
                    if (partition != null)
                    {
                        using (var diskDriveSearcher = new ManagementObjectSearcher($"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partition["DeviceID"]}'}} WHERE AssocClass = Win32_DiskDriveToDiskPartition"))
                        using (var diskDrive = diskDriveSearcher.Get().Cast<ManagementObject>().FirstOrDefault())
                        {
                            if (diskDrive != null)
                            {
                                model = diskDrive["Model"]?.ToString() ?? "N/A";
                            }
                        }
                    }
                }
                _driveModelCache[driveLetter] = model;
                return model;
            }
            catch (Exception)
            {
                return "N/A";
            }
        }
    }
}
