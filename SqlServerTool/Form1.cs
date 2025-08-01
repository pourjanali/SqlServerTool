using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlServerTool
{
    public partial class Form1 : Form
    {
        private SqlManager _sqlManager;
        private List<DatabaseInfo> _databases = new List<DatabaseInfo>();

        public Form1()
        {
            InitializeComponent();

            _txtServer.Text = ".\\Sepidar";
            _txtUser.Text = "";
            _txtPassword.Text = "";

            _rbSqlAuth.Checked = true;
            _rbWindowsAuth.CheckedChanged += new EventHandler(_rbAuth_CheckedChanged);
            _rbSqlAuth.CheckedChanged += new EventHandler(_rbAuth_CheckedChanged);
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

        private void SetConnectedState()
        {
            _gbConnection.Enabled = false;
            _btnConnect.Enabled = false;
            _btnDisconnect.Enabled = true;

            _btnBackup.Enabled = true;
            _btnBackupAndVerify.Enabled = true;
            _btnDelete.Enabled = true;
            _btnCheckDb.Enabled = true;
            _btnDetach.Enabled = true;
            _gbAttach.Enabled = true;
            _btnRestore.Enabled = true;
        }

        private void SetDisconnectedState()
        {
            _sqlManager = null;
            _gbConnection.Enabled = true;
            _btnConnect.Enabled = true;
            _btnDisconnect.Enabled = false;

            _lbDatabases.DataSource = null;
            _lbFiscalYears.DataSource = null;
            _txtMdfPath.Clear();
            _txtLdfPath.Clear();
            _lblVersion.Text = "Version: N/A";
            _lblDbVersion.Text = "DataVersion: N/A";
            _lblCompanyName.Text = "Company: N/A";
            _lblDbType.Text = "Type: N/A";


            _btnBackup.Enabled = false;
            _btnBackupAndVerify.Enabled = false;
            _btnDelete.Enabled = false;
            _btnCheckDb.Enabled = false;
            _btnDetach.Enabled = false;
            _gbAttach.Enabled = false;
            _btnRestore.Enabled = false;
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
            _btnConnect.Enabled = false;
            try
            {
                _sqlManager = new SqlManager(connectionString);

                // Get the full version string including the edition
                string version = await _sqlManager.GetSqlServerVersionAsync();
                _lblVersion.Text = $"Version: {version}";
                Log("Connection successful.");

                await RefreshDatabaseList();
                SetConnectedState();
            }
            catch (Exception ex)
            {
                Log($"ERROR: {ex.Message}");
                MessageBox.Show($"Connection Failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetDisconnectedState();
            }
        }

        private void _btnDisconnect_Click(object sender, EventArgs e)
        {
            SetDisconnectedState();
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
            _databases = await _sqlManager.GetDatabasesAndFilesAsync();
            _lbDatabases.DataSource = _databases;
            _lbDatabases.DisplayMember = "Name";
            Log($"Found {_databases.Count} databases.");
        }

        private async void _btnBackup_Click(object sender, EventArgs e)
        {
            await PerformBackup(verify: false);
        }

        private async void _btnBackupAndVerify_Click(object sender, EventArgs e)
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
                sfd.FileName = $"{dbInfo.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string action = verify ? "Backing up and verifying" : "Backing up";
                    Log($"{action} {dbInfo.Name} to {sfd.FileName}...");
                    try
                    {
                        await _sqlManager.BackupDatabaseAsync(dbInfo.Name, sfd.FileName, verify);
                        string successMsg = verify ? "Backup and verification completed successfully!" : "Backup completed successfully!";
                        Log(successMsg);
                        MessageBox.Show(successMsg, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        Log($"ERROR: {ex.Message}");
                        MessageBox.Show($"Backup Failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                Log($"Deleting database {dbInfo.Name}...");
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
            Log($"Running DBCC CHECKDB for {dbInfo.Name}. This may take a while...");
            this.Cursor = Cursors.WaitCursor;
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
                this.Cursor = Cursors.Default;
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
                Log($"Detaching database {dbInfo.Name}...");
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
            }
        }

        private async void _btnRestore_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Backup File (*.bak)|*.bak";
                ofd.Title = "Select a Database Backup File to Restore";
                if (ofd.ShowDialog() != DialogResult.OK) return;

                this.Cursor = Cursors.WaitCursor;
                try
                {
                    Log($"Analyzing backup file: {ofd.FileName}");
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

                    await _sqlManager.RestoreDatabaseAsync(newDbName, ofd.FileName, newMdfPath, newLdfPath);

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
                    this.Cursor = Cursors.Default;
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
                _lbFiscalYears.DataSource = null;

                try
                {
                    var details = await _sqlManager.GetDatabaseDetailsAsync(dbInfo.Name);
                    _lblDbVersion.Text = $"DataVersion: {details.DataVersion}";
                    _lblCompanyName.Text = $"Company: {details.CompanyName}";
                    _lblDbType.Text = $"Type: {details.DbType}";
                    _lbFiscalYears.DataSource = details.FiscalYears;
                }
                catch (Exception ex)
                {
                    Log($"ERROR: Could not get database details for {dbInfo.Name}. {ex.Message}");
                    _lblDbVersion.Text = "DataVersion: Error";
                    _lblCompanyName.Text = "Company: Error";
                    _lblDbType.Text = "Type: Error";
                }
            }
            else
            {
                _txtMdfPath.Clear();
                _txtLdfPath.Clear();
                _lblDbVersion.Text = "DataVersion: N/A";
                _lblCompanyName.Text = "Company: N/A";
                _lblDbType.Text = "Type: N/A";
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
                    _txtAttachLdf.Text = Path.ChangeExtension(ofd.FileName, ".ldf");
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
            string dbName = _txtAttachDbName.Text;
            string mdfPath = _txtAttachMdf.Text;
            string ldfPath = _txtAttachLdf.Text;

            if (string.IsNullOrWhiteSpace(dbName) || string.IsNullOrWhiteSpace(mdfPath) || string.IsNullOrWhiteSpace(ldfPath))
            {
                MessageBox.Show("Please provide a database name, MDF path, and LDF path.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Log($"Attaching database {dbName}...");
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
        }

        private void _rbAuth_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAuthControls();
        }
    }
}
