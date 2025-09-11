using Microsoft.Data.Sql;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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
            _txtUser.Text = "";
            _txtPassword.Text = "";

            _rbSqlAuth.Checked = true;
            _rbWindowsAuth.CheckedChanged += new EventHandler(_rbAuth_CheckedChanged);
            _rbSqlAuth.CheckedChanged += new EventHandler(_rbAuth_CheckedChanged);

            _progressBar.Style = ProgressBarStyle.Continuous;
            _progressBar.ForeColor = Color.Green;

            UpdateAuthControls();
            SetDisconnectedState();
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
            _gbDbOperations.Enabled = isEnabled;
            _gbAttach.Enabled = isEnabled;
            _lbDatabases.Enabled = isEnabled;
        }

        private void ShowProgress(string initialMessage)
        {
            Log(initialMessage);
            _progressBar.Value = 0;
            _progressBar.Visible = true;
            this.Cursor = Cursors.WaitCursor;
            ToggleOperationControls(false);
        }

        private void HideProgress()
        {
            _progressBar.Value = 0;
            _progressBar.Visible = false;
            this.Cursor = Cursors.Default;
            var isConnected = _sqlManager != null;
            ToggleOperationControls(isConnected);
        }

        // #6: Fix Connect/Disconnect button usability
        private void SetConnectedState()
        {
            // Instead of disabling the whole groupbox, disable individual controls
            _txtServer.Enabled = false;
            _btnBrowseServers.Enabled = false;
            _rbWindowsAuth.Enabled = false;
            _rbSqlAuth.Enabled = false;
            _txtUser.Enabled = false;
            _txtPassword.Enabled = false;
            _btnConnect.Enabled = false;

            _btnDisconnect.Enabled = true; // This button will now be usable
            ToggleOperationControls(true);
        }

        // #6: Fix Connect/Disconnect button usability
        private void SetDisconnectedState()
        {
            _sqlManager = null;
            // Explicitly enable connection controls
            _txtServer.Enabled = true;
            _btnBrowseServers.Enabled = true;
            _rbWindowsAuth.Enabled = true;
            _rbSqlAuth.Enabled = true;
            UpdateAuthControls(); // This correctly handles enabling/disabling user/pass fields based on auth type
            _btnConnect.Enabled = true;

            _btnDisconnect.Enabled = false;

            _lbDatabases.DataSource = null;
            _lbFiscalYears.DataSource = null;
            _txtMdfPath.Clear();
            _txtLdfPath.Clear();
            _lblVersion.Text = "Version: N/A";
            _toolTip.SetToolTip(_lblVersion, "");
            _lblDbVersion.Text = "DataVersion: N/A";
            _lblCompanyName.Text = "Company: N/A";
            _lblDbType.Text = "Type: N/A";
            _lblActivationCode.Text = "ActivationCode: N/A";

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
            _btnConnect.Enabled = false;
            _btnDisconnect.Enabled = false;

            try
            {
                _sqlManager = new SqlManager(connectionString);

                string version = await _sqlManager.GetSqlServerVersionAsync();
                _sqlShortVersion = await _sqlManager.GetSqlShortVersionAsync();
                _lblVersion.Text = $"Version: {version}";
                _toolTip.SetToolTip(_lblVersion, version);
                Log("Connection successful.");

                SetConnectedState(); // Set state before refreshing list
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
            }
        }


        private void _btnDisconnect_Click(object sender, EventArgs e)
        {
            SetDisconnectedState();
        }

        // #15: Add button to browse for SQL servers
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

                // Use a simple selection form
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
            _lbDatabases.DataSource = null;
            _lbFiscalYears.DataSource = null;
            _txtMdfPath.Clear();
            _txtLdfPath.Clear();
            _lblDbVersion.Text = "DataVersion: N/A";
            _lblCompanyName.Text = "Company: N/A";
            _lblDbType.Text = "Type: N/A";
            _lblActivationCode.Text = "ActivationCode: N/A";
            _databases = await _sqlManager.GetDatabasesAndFilesAsync();
            _lbDatabases.DataSource = _databases;
            _lbDatabases.DisplayMember = "Name";
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
            var result = MessageBox.Show($"Are you sure you want to permanently delete the database '{dbInfo.Name}'?\n\nTHIS ACTION CANNOT BE UNDONE.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
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

        // #18: Schema Check button handler
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
                Log(message.Replace(Environment.NewLine, " ")); // Log as a single line

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

        private async void _btnDetach_Click(object sender, EventArgs e)
        {
            if (_lbDatabases.SelectedItem == null)
            {
                MessageBox.Show("Please select a database first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dbInfo = (DatabaseInfo)_lbDatabases.SelectedItem;
            var result = MessageBox.Show($"Are you sure you want to detach the database '{dbInfo.Name}'?\nThe database files will NOT be deleted.", "Confirm Detach", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
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
                        return;
                    }

                    string newDbName = Interaction.InputBox("Enter the new name for the restored database:", "Restore Database As", originalDbName);
                    if (string.IsNullOrWhiteSpace(newDbName))
                    {
                        Log("Restore cancelled by user.");
                        return;
                    }

                    string destinationFolder;
                    using (var fbd = new FolderBrowserDialog())
                    {
                        fbd.Description = "Select a folder to save the new MDF and LDF database files.";
                        if (fbd.ShowDialog() != DialogResult.OK)
                        {
                            Log("Restore cancelled by user.");
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

                _lblDbVersion.Text = "DataVersion: Loading...";
                _lblCompanyName.Text = "Company: Loading...";
                _lblDbType.Text = "Type: Loading...";
                _lblActivationCode.Text = "ActivationCode: Loading...";
                _lbFiscalYears.DataSource = null;

                try
                {
                    var details = await _sqlManager.GetDatabaseDetailsAsync(dbInfo.Name);
                    _lblDbVersion.Text = $"DataVersion: {details.DataVersion}";
                    _lblCompanyName.Text = $"Company: {details.CompanyName}";
                    _lblDbType.Text = $"Type: {details.DbType}";
                    _lblActivationCode.Text = $"ActivationCode: {details.ActivationCode}";
                    _lbFiscalYears.DataSource = details.FiscalYears;
                }
                catch (Exception ex)
                {
                    Log($"ERROR: Could not get database details for {dbInfo.Name}. {ex.Message}");
                    _lblDbVersion.Text = "DataVersion: Error";
                    _lblCompanyName.Text = "Company: Error";
                    _lblDbType.Text = "Type: Error";
                    _lblActivationCode.Text = "ActivationCode: Error";
                }
            }
            else
            {
                _txtMdfPath.Clear();
                _txtLdfPath.Clear();
                _lblDbVersion.Text = "DataVersion: N/A";
                _lblCompanyName.Text = "Company: N/A";
                _lblDbType.Text = "Type: N/A";
                _lblActivationCode.Text = "ActivationCode: N/A";
                _lbFiscalYears.DataSource = null;
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
                    _txtAttachDbName.Text = Path.GetFileNameWithoutExtension(ofd.FileName);
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
            string dbName = Path.GetFileNameWithoutExtension(_txtAttachMdf.Text);
            string mdfPath = _txtAttachMdf.Text;
            string ldfPath = _txtAttachLdf.Text;

            if (string.IsNullOrWhiteSpace(mdfPath) || string.IsNullOrWhiteSpace(ldfPath))
            {
                MessageBox.Show("Please provide an MDF path and LDF path.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ShowProgress($"Attaching database {dbName}...");
            try
            {
                await _sqlManager.AttachDatabaseAsync(dbName, mdfPath, ldfPath);
                Log("Database attached successfully.");
                MessageBox.Show("Database attached successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await RefreshDatabaseList();
                _txtAttachDbName.Clear();
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
            AboutBox about = new AboutBox();
            about.ShowDialog(this);
        }
    }
}

