namespace SqlServerTool
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._gbConnection = new System.Windows.Forms.GroupBox();
            this._btnAbout = new System.Windows.Forms.Button();
            this._btnConnect = new System.Windows.Forms.Button();
            this._btnBrowseServers = new System.Windows.Forms.Button();
            this._rbWindowsAuth = new System.Windows.Forms.RadioButton();
            this._rbSqlAuth = new System.Windows.Forms.RadioButton();
            this._lblServer = new System.Windows.Forms.Label();
            this._txtServer = new System.Windows.Forms.TextBox();
            this._lblUser = new System.Windows.Forms.Label();
            this._txtUser = new System.Windows.Forms.TextBox();
            this._lblPassword = new System.Windows.Forms.Label();
            this._txtPassword = new System.Windows.Forms.TextBox();
            this._btnDisconnect = new System.Windows.Forms.Button();
            this._lbDatabases = new System.Windows.Forms.ListBox();
            this._btnBackup = new System.Windows.Forms.Button();
            this._btnDelete = new System.Windows.Forms.Button();
            this._btnCheckDb = new System.Windows.Forms.Button();
            this._btnDetach = new System.Windows.Forms.Button();
            this._rtbLog = new System.Windows.Forms.RichTextBox();
            this._gbFilePaths = new System.Windows.Forms.GroupBox();
            this._lblLdfDrive = new System.Windows.Forms.Label();
            this._lblMdfDrive = new System.Windows.Forms.Label();
            this._btnOpenDir = new System.Windows.Forms.Button();
            this._txtLdfPath = new System.Windows.Forms.TextBox();
            this._lblLdf = new System.Windows.Forms.Label();
            this._txtMdfPath = new System.Windows.Forms.TextBox();
            this._lblMdf = new System.Windows.Forms.Label();
            this._gbAttach = new System.Windows.Forms.GroupBox();
            this._btnAttach = new System.Windows.Forms.Button();
            this._btnBrowseLdf = new System.Windows.Forms.Button();
            this._txtAttachLdf = new System.Windows.Forms.TextBox();
            this._lblAttachLdf = new System.Windows.Forms.Label();
            this._btnBrowseMdf = new System.Windows.Forms.Button();
            this._txtAttachMdf = new System.Windows.Forms.TextBox();
            this._lblAttachMdf = new System.Windows.Forms.Label();
            this._btnRestore = new System.Windows.Forms.Button();
            this._lblDbVersion = new System.Windows.Forms.Label();
            this._lblCompanyName = new System.Windows.Forms.Label();
            this._lbFiscalYears = new System.Windows.Forms.ListBox();
            this._lblFiscalYears = new System.Windows.Forms.Label();
            this._lblDbType = new System.Windows.Forms.Label();
            this._toolTip = new System.Windows.Forms.ToolTip(this.components);
            this._btnSchemaCheck = new System.Windows.Forms.Button();
            this._btnCheckTriggers = new System.Windows.Forms.Button();
            this._btnOpenQuery = new System.Windows.Forms.Button();
            this._btnSaveQuery = new System.Windows.Forms.Button();
            this._btnExecuteQuery = new System.Windows.Forms.Button();
            this._btnBackupVerifyChecksum = new System.Windows.Forms.Button();
            this._lblDatabases = new System.Windows.Forms.Label();
            this._lblActivationCode = new System.Windows.Forms.Label();
            this._progressBar = new System.Windows.Forms.ProgressBar();
            this._gbDbOperations = new System.Windows.Forms.GroupBox();
            this._lblUserAccessMode = new System.Windows.Forms.Label();
            this._lblServerName = new System.Windows.Forms.Label();
            this._lblServiceName = new System.Windows.Forms.Label();
            this._lblConnections = new System.Windows.Forms.Label();
            this._lblTranCount = new System.Windows.Forms.Label();
            this._lblLanguage = new System.Windows.Forms.Label();
            this._lblCollation = new System.Windows.Forms.Label();
            this._gbDbInfo = new System.Windows.Forms.GroupBox();
            this._lblDbSize = new System.Windows.Forms.Label();
            this._gbServerInfo = new System.Windows.Forms.GroupBox();
            this._tabControl = new System.Windows.Forms.TabControl();
            this._tabDbManager = new System.Windows.Forms.TabPage();
            this._splitDbManager = new System.Windows.Forms.SplitContainer();
            this._flowPanelRight = new System.Windows.Forms.FlowLayoutPanel();
            this._gbFiscalContainer = new System.Windows.Forms.GroupBox();
            this._tabQueryWorkbook = new System.Windows.Forms.TabPage();
            this._splitContainer = new System.Windows.Forms.SplitContainer();
            this._lblQueryDb = new System.Windows.Forms.Label();
            this._cmbDatabasesQuery = new System.Windows.Forms.ComboBox();
            this._txtQuery = new System.Windows.Forms.TextBox();
            this._dgvResults = new System.Windows.Forms.DataGridView();
            this._gbSystemInfo = new System.Windows.Forms.GroupBox();
            this._txtSystemDetails = new System.Windows.Forms.TextBox();
            this._lblSqlVersion = new System.Windows.Forms.Label();
            this._panelTop = new System.Windows.Forms.Panel();
            this._panelBottom = new System.Windows.Forms.Panel();
            this._gbConnection.SuspendLayout();
            this._gbFilePaths.SuspendLayout();
            this._gbAttach.SuspendLayout();
            this._gbDbOperations.SuspendLayout();
            this._gbDbInfo.SuspendLayout();
            this._gbServerInfo.SuspendLayout();
            this._tabControl.SuspendLayout();
            this._tabDbManager.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._splitDbManager)).BeginInit();
            this._splitDbManager.Panel1.SuspendLayout();
            this._splitDbManager.Panel2.SuspendLayout();
            this._splitDbManager.SuspendLayout();
            this._flowPanelRight.SuspendLayout();
            this._gbFiscalContainer.SuspendLayout();
            this._tabQueryWorkbook.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._splitContainer)).BeginInit();
            this._splitContainer.Panel1.SuspendLayout();
            this._splitContainer.Panel2.SuspendLayout();
            this._splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._dgvResults)).BeginInit();
            this._gbSystemInfo.SuspendLayout();
            this._panelTop.SuspendLayout();
            this._panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // _gbConnection
            // 
            this._gbConnection.Controls.Add(this._btnAbout);
            this._gbConnection.Controls.Add(this._btnConnect);
            this._gbConnection.Controls.Add(this._btnBrowseServers);
            this._gbConnection.Controls.Add(this._rbWindowsAuth);
            this._gbConnection.Controls.Add(this._rbSqlAuth);
            this._gbConnection.Controls.Add(this._lblServer);
            this._gbConnection.Controls.Add(this._txtServer);
            this._gbConnection.Controls.Add(this._lblUser);
            this._gbConnection.Controls.Add(this._txtUser);
            this._gbConnection.Controls.Add(this._lblPassword);
            this._gbConnection.Controls.Add(this._txtPassword);
            this._gbConnection.Controls.Add(this._btnDisconnect);
            this._gbConnection.Dock = System.Windows.Forms.DockStyle.Left;
            this._gbConnection.Location = new System.Drawing.Point(5, 5);
            this._gbConnection.Name = "_gbConnection";
            this._gbConnection.Size = new System.Drawing.Size(530, 150);
            this._gbConnection.TabIndex = 0;
            this._gbConnection.TabStop = false;
            this._gbConnection.Text = "Connection Settings";
            // 
            // _btnAbout
            // 
            this._btnAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnAbout.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnAbout.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this._btnAbout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnAbout.Location = new System.Drawing.Point(433, 114);
            this._btnAbout.Name = "_btnAbout";
            this._btnAbout.Size = new System.Drawing.Size(91, 28);
            this._btnAbout.TabIndex = 7;
            this._btnAbout.Text = "About";
            this._btnAbout.UseVisualStyleBackColor = true;
            this._btnAbout.Click += new System.EventHandler(this._btnAbout_Click);
            // 
            // _btnConnect
            // 
            this._btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnConnect.BackColor = System.Drawing.Color.SeaGreen;
            this._btnConnect.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnConnect.FlatAppearance.BorderSize = 0;
            this._btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnConnect.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._btnConnect.ForeColor = System.Drawing.Color.White;
            this._btnConnect.Location = new System.Drawing.Point(433, 22);
            this._btnConnect.Name = "_btnConnect";
            this._btnConnect.Size = new System.Drawing.Size(91, 59);
            this._btnConnect.TabIndex = 5;
            this._btnConnect.Text = "Connect";
            this._btnConnect.UseVisualStyleBackColor = false;
            this._btnConnect.Click += new System.EventHandler(this._btnConnect_Click);
            // 
            // _btnBrowseServers
            // 
            this._btnBrowseServers.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnBrowseServers.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._btnBrowseServers.Location = new System.Drawing.Point(388, 51);
            this._btnBrowseServers.Name = "_btnBrowseServers";
            this._btnBrowseServers.Size = new System.Drawing.Size(30, 25);
            this._btnBrowseServers.TabIndex = 2;
            this._btnBrowseServers.Text = "...";
            this._toolTip.SetToolTip(this._btnBrowseServers, "Browse for SQL Server instances");
            this._btnBrowseServers.UseVisualStyleBackColor = true;
            this._btnBrowseServers.Click += new System.EventHandler(this._btnBrowseServers_Click);
            // 
            // _rbWindowsAuth
            // 
            this._rbWindowsAuth.AutoSize = true;
            this._rbWindowsAuth.Cursor = System.Windows.Forms.Cursors.Hand;
            this._rbWindowsAuth.Location = new System.Drawing.Point(15, 25);
            this._rbWindowsAuth.Name = "_rbWindowsAuth";
            this._rbWindowsAuth.Size = new System.Drawing.Size(156, 19);
            this._rbWindowsAuth.TabIndex = 0;
            this._rbWindowsAuth.TabStop = true;
            this._rbWindowsAuth.Text = "Windows Authentication";
            this._rbWindowsAuth.UseVisualStyleBackColor = true;
            // 
            // _rbSqlAuth
            // 
            this._rbSqlAuth.AutoSize = true;
            this._rbSqlAuth.Cursor = System.Windows.Forms.Cursors.Hand;
            this._rbSqlAuth.Location = new System.Drawing.Point(212, 25);
            this._rbSqlAuth.Name = "_rbSqlAuth";
            this._rbSqlAuth.Size = new System.Drawing.Size(163, 19);
            this._rbSqlAuth.TabIndex = 1;
            this._rbSqlAuth.TabStop = true;
            this._rbSqlAuth.Text = "SQL Server Authentication";
            this._rbSqlAuth.UseVisualStyleBackColor = true;
            // 
            // _lblServer
            // 
            this._lblServer.AutoSize = true;
            this._lblServer.Location = new System.Drawing.Point(15, 55);
            this._lblServer.Name = "_lblServer";
            this._lblServer.Size = new System.Drawing.Size(42, 15);
            this._lblServer.TabIndex = 2;
            this._lblServer.Text = "Server:";
            // 
            // _txtServer
            // 
            this._txtServer.Location = new System.Drawing.Point(90, 52);
            this._txtServer.Name = "_txtServer";
            this._txtServer.Size = new System.Drawing.Size(292, 23);
            this._txtServer.TabIndex = 1;
            // 
            // _lblUser
            // 
            this._lblUser.AutoSize = true;
            this._lblUser.Location = new System.Drawing.Point(15, 88);
            this._lblUser.Name = "_lblUser";
            this._lblUser.Size = new System.Drawing.Size(33, 15);
            this._lblUser.TabIndex = 4;
            this._lblUser.Text = "User:";
            // 
            // _txtUser
            // 
            this._txtUser.Location = new System.Drawing.Point(90, 85);
            this._txtUser.Name = "_txtUser";
            this._txtUser.Size = new System.Drawing.Size(328, 23);
            this._txtUser.TabIndex = 3;
            // 
            // _lblPassword
            // 
            this._lblPassword.AutoSize = true;
            this._lblPassword.Location = new System.Drawing.Point(15, 121);
            this._lblPassword.Name = "_lblPassword";
            this._lblPassword.Size = new System.Drawing.Size(60, 15);
            this._lblPassword.TabIndex = 6;
            this._lblPassword.Text = "Password:";
            // 
            // _txtPassword
            // 
            this._txtPassword.Location = new System.Drawing.Point(90, 118);
            this._txtPassword.Name = "_txtPassword";
            this._txtPassword.Size = new System.Drawing.Size(328, 23);
            this._txtPassword.TabIndex = 4;
            this._txtPassword.UseSystemPasswordChar = true;
            // 
            // _btnDisconnect
            // 
            this._btnDisconnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnDisconnect.BackColor = System.Drawing.Color.IndianRed;
            this._btnDisconnect.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnDisconnect.FlatAppearance.BorderSize = 0;
            this._btnDisconnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnDisconnect.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnDisconnect.ForeColor = System.Drawing.Color.White;
            this._btnDisconnect.Location = new System.Drawing.Point(433, 22);
            this._btnDisconnect.Name = "_btnDisconnect";
            this._btnDisconnect.Size = new System.Drawing.Size(91, 59);
            this._btnDisconnect.TabIndex = 6;
            this._btnDisconnect.Text = "Disconnect";
            this._btnDisconnect.UseVisualStyleBackColor = false;
            this._btnDisconnect.Visible = false;
            this._btnDisconnect.Click += new System.EventHandler(this._btnDisconnect_Click);
            // 
            // _lbDatabases
            // 
            this._lbDatabases.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._lbDatabases.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lbDatabases.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._lbDatabases.FormattingEnabled = true;
            this._lbDatabases.IntegralHeight = false;
            this._lbDatabases.ItemHeight = 17;
            this._lbDatabases.Location = new System.Drawing.Point(0, 25);
            this._lbDatabases.Name = "_lbDatabases";
            this._lbDatabases.Size = new System.Drawing.Size(262, 477);
            this._lbDatabases.TabIndex = 2;
            this._lbDatabases.SelectedIndexChanged += new System.EventHandler(this._lbDatabases_SelectedIndexChanged);
            // 
            // _btnBackup
            // 
            this._btnBackup.BackColor = System.Drawing.Color.WhiteSmoke;
            this._btnBackup.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnBackup.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this._btnBackup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnBackup.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnBackup.Location = new System.Drawing.Point(15, 25);
            this._btnBackup.Name = "_btnBackup";
            this._btnBackup.Size = new System.Drawing.Size(120, 35);
            this._btnBackup.TabIndex = 0;
            this._btnBackup.Text = "Backup";
            this._btnBackup.UseVisualStyleBackColor = false;
            this._btnBackup.Click += new System.EventHandler(this._btnBackup_Click);
            // 
            // _btnDelete
            // 
            this._btnDelete.BackColor = System.Drawing.Color.MistyRose;
            this._btnDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnDelete.FlatAppearance.BorderColor = System.Drawing.Color.Crimson;
            this._btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnDelete.ForeColor = System.Drawing.Color.Crimson;
            this._btnDelete.Location = new System.Drawing.Point(393, 25);
            this._btnDelete.Name = "_btnDelete";
            this._btnDelete.Size = new System.Drawing.Size(120, 35);
            this._btnDelete.TabIndex = 6;
            this._btnDelete.Text = "Delete DB";
            this._btnDelete.UseVisualStyleBackColor = false;
            this._btnDelete.Click += new System.EventHandler(this._btnDelete_Click);
            // 
            // _btnCheckDb
            // 
            this._btnCheckDb.BackColor = System.Drawing.Color.AliceBlue;
            this._btnCheckDb.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnCheckDb.FlatAppearance.BorderColor = System.Drawing.Color.SteelBlue;
            this._btnCheckDb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnCheckDb.ForeColor = System.Drawing.Color.SteelBlue;
            this._btnCheckDb.Location = new System.Drawing.Point(15, 66);
            this._btnCheckDb.Name = "_btnCheckDb";
            this._btnCheckDb.Size = new System.Drawing.Size(120, 35);
            this._btnCheckDb.TabIndex = 2;
            this._btnCheckDb.Text = "Check Health";
            this._toolTip.SetToolTip(this._btnCheckDb, "Runs DBCC CHECKDB to verify database integrity.");
            this._btnCheckDb.UseVisualStyleBackColor = false;
            this._btnCheckDb.Click += new System.EventHandler(this._btnCheckDb_Click);
            // 
            // _btnDetach
            // 
            this._btnDetach.BackColor = System.Drawing.Color.Ivory;
            this._btnDetach.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnDetach.FlatAppearance.BorderColor = System.Drawing.Color.Goldenrod;
            this._btnDetach.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnDetach.ForeColor = System.Drawing.Color.Goldenrod;
            this._btnDetach.Location = new System.Drawing.Point(267, 25);
            this._btnDetach.Name = "_btnDetach";
            this._btnDetach.Size = new System.Drawing.Size(120, 35);
            this._btnDetach.TabIndex = 5;
            this._btnDetach.Text = "Detach DB";
            this._btnDetach.UseVisualStyleBackColor = false;
            this._btnDetach.Click += new System.EventHandler(this._btnDetach_Click);
            // 
            // _rtbLog
            // 
            this._rtbLog.BackColor = System.Drawing.Color.White;
            this._rtbLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rtbLog.Font = new System.Drawing.Font("Consolas", 9F);
            this._rtbLog.Location = new System.Drawing.Point(0, 0);
            this._rtbLog.Name = "_rtbLog";
            this._rtbLog.ReadOnly = true;
            this._rtbLog.Size = new System.Drawing.Size(1064, 85);
            this._rtbLog.TabIndex = 8;
            this._rtbLog.Text = "";
            // 
            // _gbFilePaths
            // 
            this._gbFilePaths.Controls.Add(this._lblLdfDrive);
            this._gbFilePaths.Controls.Add(this._lblMdfDrive);
            this._gbFilePaths.Controls.Add(this._btnOpenDir);
            this._gbFilePaths.Controls.Add(this._txtLdfPath);
            this._gbFilePaths.Controls.Add(this._lblLdf);
            this._gbFilePaths.Controls.Add(this._txtMdfPath);
            this._gbFilePaths.Controls.Add(this._lblMdf);
            this._gbFilePaths.Location = new System.Drawing.Point(3, 237);
            this._gbFilePaths.Name = "_gbFilePaths";
            this._gbFilePaths.Size = new System.Drawing.Size(730, 90);
            this._gbFilePaths.TabIndex = 7;
            this._gbFilePaths.TabStop = false;
            this._gbFilePaths.Text = "Physical File Locations";
            // 
            // _lblLdfDrive
            // 
            this._lblLdfDrive.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblLdfDrive.ForeColor = System.Drawing.Color.DimGray;
            this._lblLdfDrive.Location = new System.Drawing.Point(456, 55);
            this._lblLdfDrive.Name = "_lblLdfDrive";
            this._lblLdfDrive.Size = new System.Drawing.Size(160, 15);
            this._lblLdfDrive.TabIndex = 4;
            this._lblLdfDrive.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _lblMdfDrive
            // 
            this._lblMdfDrive.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblMdfDrive.ForeColor = System.Drawing.Color.DimGray;
            this._lblMdfDrive.Location = new System.Drawing.Point(456, 25);
            this._lblMdfDrive.Name = "_lblMdfDrive";
            this._lblMdfDrive.Size = new System.Drawing.Size(160, 15);
            this._lblMdfDrive.TabIndex = 3;
            this._lblMdfDrive.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _btnOpenDir
            // 
            this._btnOpenDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnOpenDir.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnOpenDir.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._btnOpenDir.Location = new System.Drawing.Point(630, 22);
            this._btnOpenDir.Name = "_btnOpenDir";
            this._btnOpenDir.Size = new System.Drawing.Size(94, 50);
            this._btnOpenDir.TabIndex = 2;
            this._btnOpenDir.Text = "Open Folder";
            this._btnOpenDir.UseVisualStyleBackColor = true;
            this._btnOpenDir.Click += new System.EventHandler(this._btnOpenDir_Click);
            // 
            // _txtLdfPath
            // 
            this._txtLdfPath.BackColor = System.Drawing.Color.White;
            this._txtLdfPath.Location = new System.Drawing.Point(90, 52);
            this._txtLdfPath.Name = "_txtLdfPath";
            this._txtLdfPath.ReadOnly = true;
            this._txtLdfPath.Size = new System.Drawing.Size(360, 23);
            this._txtLdfPath.TabIndex = 1;
            // 
            // _lblLdf
            // 
            this._lblLdf.AutoSize = true;
            this._lblLdf.Location = new System.Drawing.Point(15, 55);
            this._lblLdf.Name = "_lblLdf";
            this._lblLdf.Size = new System.Drawing.Size(57, 15);
            this._lblLdf.TabIndex = 2;
            this._lblLdf.Text = "LDF Path:";
            // 
            // _txtMdfPath
            // 
            this._txtMdfPath.BackColor = System.Drawing.Color.White;
            this._txtMdfPath.Location = new System.Drawing.Point(90, 22);
            this._txtMdfPath.Name = "_txtMdfPath";
            this._txtMdfPath.ReadOnly = true;
            this._txtMdfPath.Size = new System.Drawing.Size(360, 23);
            this._txtMdfPath.TabIndex = 0;
            // 
            // _lblMdf
            // 
            this._lblMdf.AutoSize = true;
            this._lblMdf.Location = new System.Drawing.Point(15, 25);
            this._lblMdf.Name = "_lblMdf";
            this._lblMdf.Size = new System.Drawing.Size(62, 15);
            this._lblMdf.TabIndex = 0;
            this._lblMdf.Text = "MDF Path:";
            // 
            // _gbAttach
            // 
            this._gbAttach.Controls.Add(this._btnAttach);
            this._gbAttach.Controls.Add(this._btnBrowseLdf);
            this._gbAttach.Controls.Add(this._txtAttachLdf);
            this._gbAttach.Controls.Add(this._lblAttachLdf);
            this._gbAttach.Controls.Add(this._btnBrowseMdf);
            this._gbAttach.Controls.Add(this._txtAttachMdf);
            this._gbAttach.Controls.Add(this._lblAttachMdf);
            this._gbAttach.Location = new System.Drawing.Point(3, 451);
            this._gbAttach.Name = "_gbAttach";
            this._gbAttach.Size = new System.Drawing.Size(730, 80);
            this._gbAttach.TabIndex = 6;
            this._gbAttach.TabStop = false;
            this._gbAttach.Text = "Attach New Database";
            // 
            // _btnAttach
            // 
            this._btnAttach.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnAttach.BackColor = System.Drawing.Color.SeaGreen;
            this._btnAttach.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnAttach.FlatAppearance.BorderSize = 0;
            this._btnAttach.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnAttach.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnAttach.ForeColor = System.Drawing.Color.White;
            this._btnAttach.Location = new System.Drawing.Point(604, 22);
            this._btnAttach.Name = "_btnAttach";
            this._btnAttach.Size = new System.Drawing.Size(110, 30);
            this._btnAttach.TabIndex = 4;
            this._btnAttach.Text = "Attach";
            this._btnAttach.UseVisualStyleBackColor = false;
            this._btnAttach.Click += new System.EventHandler(this._btnAttach_Click);
            // 
            // _btnBrowseLdf
            // 
            this._btnBrowseLdf.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnBrowseLdf.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._btnBrowseLdf.Location = new System.Drawing.Point(544, 25);
            this._btnBrowseLdf.Name = "_btnBrowseLdf";
            this._btnBrowseLdf.Size = new System.Drawing.Size(30, 23);
            this._btnBrowseLdf.TabIndex = 3;
            this._btnBrowseLdf.Text = "...";
            this._btnBrowseLdf.UseVisualStyleBackColor = true;
            this._btnBrowseLdf.Click += new System.EventHandler(this._btnBrowseLdf_Click);
            // 
            // _txtAttachLdf
            // 
            this._txtAttachLdf.Location = new System.Drawing.Point(350, 25);
            this._txtAttachLdf.Name = "_txtAttachLdf";
            this._txtAttachLdf.Size = new System.Drawing.Size(188, 23);
            this._txtAttachLdf.TabIndex = 2;
            // 
            // _lblAttachLdf
            // 
            this._lblAttachLdf.AutoSize = true;
            this._lblAttachLdf.Location = new System.Drawing.Point(315, 28);
            this._lblAttachLdf.Name = "_lblAttachLdf";
            this._lblAttachLdf.Size = new System.Drawing.Size(29, 15);
            this._lblAttachLdf.TabIndex = 5;
            this._lblAttachLdf.Text = "LDF:";
            // 
            // _btnBrowseMdf
            // 
            this._btnBrowseMdf.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnBrowseMdf.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this._btnBrowseMdf.Location = new System.Drawing.Point(267, 24);
            this._btnBrowseMdf.Name = "_btnBrowseMdf";
            this._btnBrowseMdf.Size = new System.Drawing.Size(30, 23);
            this._btnBrowseMdf.TabIndex = 1;
            this._btnBrowseMdf.Text = "...";
            this._btnBrowseMdf.UseVisualStyleBackColor = true;
            this._btnBrowseMdf.Click += new System.EventHandler(this._btnBrowseMdf_Click);
            // 
            // _txtAttachMdf
            // 
            this._txtAttachMdf.Location = new System.Drawing.Point(73, 25);
            this._txtAttachMdf.Name = "_txtAttachMdf";
            this._txtAttachMdf.Size = new System.Drawing.Size(188, 23);
            this._txtAttachMdf.TabIndex = 0;
            // 
            // _lblAttachMdf
            // 
            this._lblAttachMdf.AutoSize = true;
            this._lblAttachMdf.Location = new System.Drawing.Point(11, 28);
            this._lblAttachMdf.Name = "_lblAttachMdf";
            this._lblAttachMdf.Size = new System.Drawing.Size(35, 15);
            this._lblAttachMdf.TabIndex = 2;
            this._lblAttachMdf.Text = "MDF:";
            // 
            // _btnRestore
            // 
            this._btnRestore.BackColor = System.Drawing.Color.WhiteSmoke;
            this._btnRestore.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnRestore.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this._btnRestore.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnRestore.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnRestore.Location = new System.Drawing.Point(267, 66);
            this._btnRestore.Name = "_btnRestore";
            this._btnRestore.Size = new System.Drawing.Size(120, 35);
            this._btnRestore.TabIndex = 8;
            this._btnRestore.Text = "Restore DB";
            this._btnRestore.UseVisualStyleBackColor = false;
            this._btnRestore.Click += new System.EventHandler(this._btnRestore_Click);
            // 
            // _lblDbVersion
            // 
            this._lblDbVersion.AutoSize = true;
            this._lblDbVersion.Location = new System.Drawing.Point(15, 25);
            this._lblDbVersion.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this._lblDbVersion.Name = "_lblDbVersion";
            this._lblDbVersion.Size = new System.Drawing.Size(100, 15);
            this._lblDbVersion.TabIndex = 14;
            this._lblDbVersion.Text = "Data Version: N/A";
            // 
            // _lblCompanyName
            // 
            this._lblCompanyName.AutoSize = true;
            this._lblCompanyName.Location = new System.Drawing.Point(15, 46);
            this._lblCompanyName.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this._lblCompanyName.Name = "_lblCompanyName";
            this._lblCompanyName.Size = new System.Drawing.Size(87, 15);
            this._lblCompanyName.TabIndex = 15;
            this._lblCompanyName.Text = "Company: N/A";
            // 
            // _lbFiscalYears
            // 
            this._lbFiscalYears.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._lbFiscalYears.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lbFiscalYears.FormattingEnabled = true;
            this._lbFiscalYears.IntegralHeight = false;
            this._lbFiscalYears.ItemHeight = 15;
            this._lbFiscalYears.Location = new System.Drawing.Point(3, 33);
            this._lbFiscalYears.Name = "_lbFiscalYears";
            this._lbFiscalYears.Size = new System.Drawing.Size(268, 192);
            this._lbFiscalYears.TabIndex = 5;
            // 
            // _lblFiscalYears
            // 
            this._lblFiscalYears.AutoSize = true;
            this._lblFiscalYears.Dock = System.Windows.Forms.DockStyle.Top;
            this._lblFiscalYears.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._lblFiscalYears.Location = new System.Drawing.Point(3, 19);
            this._lblFiscalYears.Name = "_lblFiscalYears";
            this._lblFiscalYears.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this._lblFiscalYears.Size = new System.Drawing.Size(68, 20);
            this._lblFiscalYears.TabIndex = 17;
            this._lblFiscalYears.Text = "Fiscal Years";
            // 
            // _lblDbType
            // 
            this._lblDbType.AutoSize = true;
            this._lblDbType.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblDbType.Location = new System.Drawing.Point(15, 67);
            this._lblDbType.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this._lblDbType.Name = "_lblDbType";
            this._lblDbType.Size = new System.Drawing.Size(61, 15);
            this._lblDbType.TabIndex = 18;
            this._lblDbType.Text = "Type: N/A";
            // 
            // _btnSchemaCheck
            // 
            this._btnSchemaCheck.BackColor = System.Drawing.Color.AliceBlue;
            this._btnSchemaCheck.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnSchemaCheck.FlatAppearance.BorderColor = System.Drawing.Color.SteelBlue;
            this._btnSchemaCheck.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnSchemaCheck.ForeColor = System.Drawing.Color.SteelBlue;
            this._btnSchemaCheck.Location = new System.Drawing.Point(141, 66);
            this._btnSchemaCheck.Name = "_btnSchemaCheck";
            this._btnSchemaCheck.Size = new System.Drawing.Size(120, 35);
            this._btnSchemaCheck.TabIndex = 3;
            this._btnSchemaCheck.Text = "Check Schema";
            this._toolTip.SetToolTip(this._btnSchemaCheck, "Compares the database\'s table schemas against the expected standard for its type " +
        "(Dasht or Sepidar).");
            this._btnSchemaCheck.UseVisualStyleBackColor = false;
            this._btnSchemaCheck.Click += new System.EventHandler(this._btnSchemaCheck_Click);
            // 
            // _btnCheckTriggers
            // 
            this._btnCheckTriggers.BackColor = System.Drawing.Color.Ivory;
            this._btnCheckTriggers.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnCheckTriggers.FlatAppearance.BorderColor = System.Drawing.Color.Goldenrod;
            this._btnCheckTriggers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnCheckTriggers.ForeColor = System.Drawing.Color.Goldenrod;
            this._btnCheckTriggers.Location = new System.Drawing.Point(393, 66);
            this._btnCheckTriggers.Name = "_btnCheckTriggers";
            this._btnCheckTriggers.Size = new System.Drawing.Size(120, 35);
            this._btnCheckTriggers.TabIndex = 7;
            this._btnCheckTriggers.Text = "Check Triggers";
            this._toolTip.SetToolTip(this._btnCheckTriggers, "Checks for unexpected or forbidden database triggers.");
            this._btnCheckTriggers.UseVisualStyleBackColor = false;
            this._btnCheckTriggers.Click += new System.EventHandler(this._btnCheckTriggers_Click);
            // 
            // _btnOpenQuery
            // 
            this._btnOpenQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnOpenQuery.Location = new System.Drawing.Point(747, 3);
            this._btnOpenQuery.Name = "_btnOpenQuery";
            this._btnOpenQuery.Size = new System.Drawing.Size(80, 30);
            this._btnOpenQuery.TabIndex = 3;
            this._btnOpenQuery.Text = "Open";
            this._toolTip.SetToolTip(this._btnOpenQuery, "Open a query from a .sql file");
            this._btnOpenQuery.UseVisualStyleBackColor = true;
            this._btnOpenQuery.Click += new System.EventHandler(this._btnOpenQuery_Click);
            // 
            // _btnSaveQuery
            // 
            this._btnSaveQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnSaveQuery.Location = new System.Drawing.Point(833, 3);
            this._btnSaveQuery.Name = "_btnSaveQuery";
            this._btnSaveQuery.Size = new System.Drawing.Size(80, 30);
            this._btnSaveQuery.TabIndex = 2;
            this._btnSaveQuery.Text = "Save";
            this._toolTip.SetToolTip(this._btnSaveQuery, "Save the current query to a .sql file");
            this._btnSaveQuery.UseVisualStyleBackColor = true;
            this._btnSaveQuery.Click += new System.EventHandler(this._btnSaveQuery_Click);
            // 
            // _btnExecuteQuery
            // 
            this._btnExecuteQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnExecuteQuery.BackColor = System.Drawing.Color.LightGreen;
            this._btnExecuteQuery.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._btnExecuteQuery.Location = new System.Drawing.Point(919, 3);
            this._btnExecuteQuery.Name = "_btnExecuteQuery";
            this._btnExecuteQuery.Size = new System.Drawing.Size(104, 30);
            this._btnExecuteQuery.TabIndex = 1;
            this._btnExecuteQuery.Text = "Execute (F5)";
            this._toolTip.SetToolTip(this._btnExecuteQuery, "Execute the query (F5)");
            this._btnExecuteQuery.UseVisualStyleBackColor = false;
            this._btnExecuteQuery.Click += new System.EventHandler(this._btnExecuteQuery_Click);
            // 
            // _btnBackupVerifyChecksum
            // 
            this._btnBackupVerifyChecksum.BackColor = System.Drawing.Color.WhiteSmoke;
            this._btnBackupVerifyChecksum.Cursor = System.Windows.Forms.Cursors.Hand;
            this._btnBackupVerifyChecksum.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this._btnBackupVerifyChecksum.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnBackupVerifyChecksum.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnBackupVerifyChecksum.Location = new System.Drawing.Point(141, 25);
            this._btnBackupVerifyChecksum.Name = "_btnBackupVerifyChecksum";
            this._btnBackupVerifyChecksum.Size = new System.Drawing.Size(120, 35);
            this._btnBackupVerifyChecksum.TabIndex = 1;
            this._btnBackupVerifyChecksum.Text = "Backup + Verify";
            this._btnBackupVerifyChecksum.UseVisualStyleBackColor = false;
            this._btnBackupVerifyChecksum.Click += new System.EventHandler(this._btnBackupVerifyChecksum_Click);
            // 
            // _lblDatabases
            // 
            this._lblDatabases.BackColor = System.Drawing.Color.Gainsboro;
            this._lblDatabases.Dock = System.Windows.Forms.DockStyle.Top;
            this._lblDatabases.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblDatabases.Location = new System.Drawing.Point(0, 0);
            this._lblDatabases.Name = "_lblDatabases";
            this._lblDatabases.Padding = new System.Windows.Forms.Padding(5);
            this._lblDatabases.Size = new System.Drawing.Size(262, 25);
            this._lblDatabases.TabIndex = 19;
            this._lblDatabases.Text = "Databases";
            // 
            // _lblActivationCode
            // 
            this._lblActivationCode.AutoSize = true;
            this._lblActivationCode.Location = new System.Drawing.Point(15, 88);
            this._lblActivationCode.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this._lblActivationCode.Name = "_lblActivationCode";
            this._lblActivationCode.Size = new System.Drawing.Size(120, 15);
            this._lblActivationCode.TabIndex = 21;
            this._lblActivationCode.Text = "Activation Code: N/A";
            // 
            // _progressBar
            // 
            this._progressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._progressBar.Location = new System.Drawing.Point(0, 85);
            this._progressBar.Name = "_progressBar";
            this._progressBar.Size = new System.Drawing.Size(1064, 15);
            this._progressBar.TabIndex = 23;
            // 
            // _gbDbOperations
            // 
            this._gbDbOperations.Controls.Add(this._btnCheckTriggers);
            this._gbDbOperations.Controls.Add(this._btnSchemaCheck);
            this._gbDbOperations.Controls.Add(this._btnBackup);
            this._gbDbOperations.Controls.Add(this._btnRestore);
            this._gbDbOperations.Controls.Add(this._btnDelete);
            this._gbDbOperations.Controls.Add(this._btnBackupVerifyChecksum);
            this._gbDbOperations.Controls.Add(this._btnCheckDb);
            this._gbDbOperations.Controls.Add(this._btnDetach);
            this._gbDbOperations.Location = new System.Drawing.Point(3, 333);
            this._gbDbOperations.Name = "_gbDbOperations";
            this._gbDbOperations.Size = new System.Drawing.Size(730, 112);
            this._gbDbOperations.TabIndex = 3;
            this._gbDbOperations.TabStop = false;
            this._gbDbOperations.Text = "Database Operations";
            // 
            // _lblUserAccessMode
            // 
            this._lblUserAccessMode.AutoSize = true;
            this._lblUserAccessMode.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._lblUserAccessMode.Location = new System.Drawing.Point(15, 109);
            this._lblUserAccessMode.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this._lblUserAccessMode.Name = "_lblUserAccessMode";
            this._lblUserAccessMode.Size = new System.Drawing.Size(107, 15);
            this._lblUserAccessMode.TabIndex = 25;
            this._lblUserAccessMode.Text = "Access Mode: N/A";
            // 
            // _lblServerName
            // 
            this._lblServerName.AutoSize = true;
            this._lblServerName.Location = new System.Drawing.Point(15, 25);
            this._lblServerName.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this._lblServerName.Name = "_lblServerName";
            this._lblServerName.Size = new System.Drawing.Size(102, 15);
            this._lblServerName.TabIndex = 26;
            this._lblServerName.Text = "Server Name: N/A";
            // 
            // _lblServiceName
            // 
            this._lblServiceName.AutoSize = true;
            this._lblServiceName.Location = new System.Drawing.Point(15, 46);
            this._lblServiceName.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this._lblServiceName.Name = "_lblServiceName";
            this._lblServiceName.Size = new System.Drawing.Size(107, 15);
            this._lblServiceName.TabIndex = 27;
            this._lblServiceName.Text = "Service Name: N/A";
            // 
            // _lblConnections
            // 
            this._lblConnections.AutoSize = true;
            this._lblConnections.Location = new System.Drawing.Point(15, 67);
            this._lblConnections.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this._lblConnections.Name = "_lblConnections";
            this._lblConnections.Size = new System.Drawing.Size(102, 15);
            this._lblConnections.TabIndex = 28;
            this._lblConnections.Text = "Connections: N/A";
            // 
            // _lblTranCount
            // 
            this._lblTranCount.AutoSize = true;
            this._lblTranCount.Location = new System.Drawing.Point(15, 88);
            this._lblTranCount.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this._lblTranCount.Name = "_lblTranCount";
            this._lblTranCount.Size = new System.Drawing.Size(93, 15);
            this._lblTranCount.TabIndex = 29;
            this._lblTranCount.Text = "Tran Count: N/A";
            // 
            // _lblLanguage
            // 
            this._lblLanguage.AutoSize = true;
            this._lblLanguage.Location = new System.Drawing.Point(15, 109);
            this._lblLanguage.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this._lblLanguage.Name = "_lblLanguage";
            this._lblLanguage.Size = new System.Drawing.Size(87, 15);
            this._lblLanguage.TabIndex = 30;
            this._lblLanguage.Text = "Language: N/A";
            // 
            // _lblCollation
            // 
            this._lblCollation.AutoSize = true;
            this._lblCollation.Location = new System.Drawing.Point(15, 130);
            this._lblCollation.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this._lblCollation.Name = "_lblCollation";
            this._lblCollation.Size = new System.Drawing.Size(83, 15);
            this._lblCollation.TabIndex = 31;
            this._lblCollation.Text = "Collation: N/A";
            // 
            // _gbDbInfo
            // 
            this._gbDbInfo.Controls.Add(this._lblDbSize);
            this._gbDbInfo.Controls.Add(this._lblDbVersion);
            this._gbDbInfo.Controls.Add(this._lblCompanyName);
            this._gbDbInfo.Controls.Add(this._lblDbType);
            this._gbDbInfo.Controls.Add(this._lblActivationCode);
            this._gbDbInfo.Controls.Add(this._lblUserAccessMode);
            this._gbDbInfo.Location = new System.Drawing.Point(3, 3);
            this._gbDbInfo.Name = "_gbDbInfo";
            this._gbDbInfo.Size = new System.Drawing.Size(440, 140);
            this._gbDbInfo.TabIndex = 4;
            this._gbDbInfo.TabStop = false;
            this._gbDbInfo.Text = "Selected Database Details";
            // 
            // _lblDbSize
            // 
            this._lblDbSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._lblDbSize.AutoSize = true;
            this._lblDbSize.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._lblDbSize.ForeColor = System.Drawing.Color.DimGray;
            this._lblDbSize.Location = new System.Drawing.Point(260, 25);
            this._lblDbSize.Name = "_lblDbSize";
            this._lblDbSize.Size = new System.Drawing.Size(62, 15);
            this._lblDbSize.TabIndex = 26;
            this._lblDbSize.Text = "Size: N/A";
            // 
            // _gbServerInfo
            // 
            this._gbServerInfo.Controls.Add(this._lblServerName);
            this._gbServerInfo.Controls.Add(this._lblCollation);
            this._gbServerInfo.Controls.Add(this._lblServiceName);
            this._gbServerInfo.Controls.Add(this._lblLanguage);
            this._gbServerInfo.Controls.Add(this._lblConnections);
            this._gbServerInfo.Controls.Add(this._lblTranCount);
            this._gbServerInfo.Location = new System.Drawing.Point(3, 149);
            this._gbServerInfo.Name = "_gbServerInfo";
            this._gbServerInfo.Size = new System.Drawing.Size(440, 82);
            this._gbServerInfo.TabIndex = 5;
            this._gbServerInfo.TabStop = false;
            this._gbServerInfo.Text = "Server && Connection Stats";
            // 
            // _tabControl
            // 
            this._tabControl.Controls.Add(this._tabDbManager);
            this._tabControl.Controls.Add(this._tabQueryWorkbook);
            this._tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tabControl.Location = new System.Drawing.Point(0, 160);
            this._tabControl.Name = "_tabControl";
            this._tabControl.SelectedIndex = 0;
            this._tabControl.Size = new System.Drawing.Size(1064, 592);
            this._tabControl.TabIndex = 24;
            // 
            // _tabDbManager
            // 
            this._tabDbManager.Controls.Add(this._splitDbManager);
            this._tabDbManager.Location = new System.Drawing.Point(4, 24);
            this._tabDbManager.Name = "_tabDbManager";
            this._tabDbManager.Padding = new System.Windows.Forms.Padding(3);
            this._tabDbManager.Size = new System.Drawing.Size(1056, 564);
            this._tabDbManager.TabIndex = 0;
            this._tabDbManager.Text = "Database Manager";
            this._tabDbManager.UseVisualStyleBackColor = true;
            // 
            // _splitDbManager
            // 
            this._splitDbManager.Dock = System.Windows.Forms.DockStyle.Fill;
            this._splitDbManager.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this._splitDbManager.Location = new System.Drawing.Point(3, 3);
            this._splitDbManager.Name = "_splitDbManager";
            // 
            // _splitDbManager.Panel1
            // 
            this._splitDbManager.Panel1.Controls.Add(this._lbDatabases);
            this._splitDbManager.Panel1.Controls.Add(this._lblDatabases);
            // 
            // _splitDbManager.Panel2
            // 
            this._splitDbManager.Panel2.Controls.Add(this._flowPanelRight);
            this._splitDbManager.Size = new System.Drawing.Size(1050, 558);
            this._splitDbManager.SplitterDistance = 262;
            this._splitDbManager.TabIndex = 0;
            // 
            // _flowPanelRight
            // 
            this._flowPanelRight.AutoScroll = true;
            this._flowPanelRight.Controls.Add(this._gbDbInfo);
            this._flowPanelRight.Controls.Add(this._gbFiscalContainer);
            this._flowPanelRight.Controls.Add(this._gbServerInfo);
            this._flowPanelRight.Controls.Add(this._gbFilePaths);
            this._flowPanelRight.Controls.Add(this._gbDbOperations);
            this._flowPanelRight.Controls.Add(this._gbAttach);
            this._flowPanelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this._flowPanelRight.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this._flowPanelRight.Location = new System.Drawing.Point(0, 0);
            this._flowPanelRight.Name = "_flowPanelRight";
            this._flowPanelRight.Size = new System.Drawing.Size(784, 558);
            this._flowPanelRight.TabIndex = 0;
            this._flowPanelRight.WrapContents = false;
            // 
            // _gbFiscalContainer
            // 
            this._gbFiscalContainer.Controls.Add(this._lbFiscalYears);
            this._gbFiscalContainer.Controls.Add(this._lblFiscalYears);
            this._gbFiscalContainer.Location = new System.Drawing.Point(449, 3);
            this._gbFiscalContainer.Name = "_gbFiscalContainer";
            this._gbFiscalContainer.Size = new System.Drawing.Size(274, 228);
            this._gbFiscalContainer.TabIndex = 10;
            this._gbFiscalContainer.TabStop = false;
            // 
            // _tabQueryWorkbook
            // 
            this._tabQueryWorkbook.Controls.Add(this._splitContainer);
            this._tabQueryWorkbook.Location = new System.Drawing.Point(4, 24);
            this._tabQueryWorkbook.Name = "_tabQueryWorkbook";
            this._tabQueryWorkbook.Padding = new System.Windows.Forms.Padding(3);
            this._tabQueryWorkbook.Size = new System.Drawing.Size(1056, 564);
            this._tabQueryWorkbook.TabIndex = 1;
            this._tabQueryWorkbook.Text = "Query Workbook";
            this._tabQueryWorkbook.UseVisualStyleBackColor = true;
            // 
            // _splitContainer
            // 
            this._splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._splitContainer.Location = new System.Drawing.Point(3, 3);
            this._splitContainer.Name = "_splitContainer";
            this._splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // _splitContainer.Panel1
            // 
            this._splitContainer.Panel1.Controls.Add(this._lblQueryDb);
            this._splitContainer.Panel1.Controls.Add(this._cmbDatabasesQuery);
            this._splitContainer.Panel1.Controls.Add(this._btnOpenQuery);
            this._splitContainer.Panel1.Controls.Add(this._btnSaveQuery);
            this._splitContainer.Panel1.Controls.Add(this._btnExecuteQuery);
            this._splitContainer.Panel1.Controls.Add(this._txtQuery);
            // 
            // _splitContainer.Panel2
            // 
            this._splitContainer.Panel2.Controls.Add(this._dgvResults);
            this._splitContainer.Size = new System.Drawing.Size(1050, 558);
            this._splitContainer.SplitterDistance = 262;
            this._splitContainer.TabIndex = 0;
            // 
            // _lblQueryDb
            // 
            this._lblQueryDb.AutoSize = true;
            this._lblQueryDb.Location = new System.Drawing.Point(3, 10);
            this._lblQueryDb.Name = "_lblQueryDb";
            this._lblQueryDb.Size = new System.Drawing.Size(105, 15);
            this._lblQueryDb.TabIndex = 5;
            this._lblQueryDb.Text = "Run query against:";
            // 
            // _cmbDatabasesQuery
            // 
            this._cmbDatabasesQuery.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cmbDatabasesQuery.FormattingEnabled = true;
            this._cmbDatabasesQuery.Location = new System.Drawing.Point(119, 7);
            this._cmbDatabasesQuery.Name = "_cmbDatabasesQuery";
            this._cmbDatabasesQuery.Size = new System.Drawing.Size(273, 23);
            this._cmbDatabasesQuery.TabIndex = 4;
            // 
            // _txtQuery
            // 
            this._txtQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._txtQuery.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._txtQuery.Location = new System.Drawing.Point(3, 39);
            this._txtQuery.Multiline = true;
            this._txtQuery.Name = "_txtQuery";
            this._txtQuery.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._txtQuery.Size = new System.Drawing.Size(1044, 220);
            this._txtQuery.TabIndex = 0;
            // 
            // _dgvResults
            // 
            this._dgvResults.AllowUserToAddRows = false;
            this._dgvResults.AllowUserToDeleteRows = false;
            this._dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dgvResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dgvResults.Location = new System.Drawing.Point(0, 0);
            this._dgvResults.Name = "_dgvResults";
            this._dgvResults.ReadOnly = true;
            this._dgvResults.Size = new System.Drawing.Size(1050, 292);
            this._dgvResults.TabIndex = 0;
            // 
            // _gbSystemInfo
            // 
            this._gbSystemInfo.Controls.Add(this._txtSystemDetails);
            this._gbSystemInfo.Controls.Add(this._lblSqlVersion);
            this._gbSystemInfo.Dock = System.Windows.Forms.DockStyle.Right;
            this._gbSystemInfo.Location = new System.Drawing.Point(541, 5);
            this._gbSystemInfo.Name = "_gbSystemInfo";
            this._gbSystemInfo.Size = new System.Drawing.Size(518, 150);
            this._gbSystemInfo.TabIndex = 25;
            this._gbSystemInfo.TabStop = false;
            this._gbSystemInfo.Text = "System & Server Details";
            // 
            // _txtSystemDetails
            // 
            this._txtSystemDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._txtSystemDetails.BackColor = System.Drawing.SystemColors.Control;
            this._txtSystemDetails.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._txtSystemDetails.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._txtSystemDetails.Location = new System.Drawing.Point(15, 55);
            this._txtSystemDetails.Multiline = true;
            this._txtSystemDetails.Name = "_txtSystemDetails";
            this._txtSystemDetails.ReadOnly = true;
            this._txtSystemDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._txtSystemDetails.Size = new System.Drawing.Size(497, 89);
            this._txtSystemDetails.TabIndex = 1;
            this._txtSystemDetails.Text = "Client Hardware: N/A";
            // 
            // _lblSqlVersion
            // 
            this._lblSqlVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._lblSqlVersion.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblSqlVersion.Location = new System.Drawing.Point(15, 25);
            this._lblSqlVersion.Name = "_lblSqlVersion";
            this._lblSqlVersion.Size = new System.Drawing.Size(490, 30);
            this._lblSqlVersion.TabIndex = 0;
            this._lblSqlVersion.Text = "SQL Version: N/A";
            // 
            // _panelTop
            // 
            this._panelTop.Controls.Add(this._gbConnection);
            this._panelTop.Controls.Add(this._gbSystemInfo);
            this._panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this._panelTop.Location = new System.Drawing.Point(0, 0);
            this._panelTop.Name = "_panelTop";
            this._panelTop.Padding = new System.Windows.Forms.Padding(5);
            this._panelTop.Size = new System.Drawing.Size(1064, 160);
            this._panelTop.TabIndex = 26;
            // 
            // _panelBottom
            // 
            this._panelBottom.Controls.Add(this._rtbLog);
            this._panelBottom.Controls.Add(this._progressBar);
            this._panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._panelBottom.Location = new System.Drawing.Point(0, 752);
            this._panelBottom.Name = "_panelBottom";
            this._panelBottom.Size = new System.Drawing.Size(1064, 100);
            this._panelBottom.TabIndex = 27;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1064, 852);
            this.Controls.Add(this._tabControl);
            this.Controls.Add(this._panelTop);
            this.Controls.Add(this._panelBottom);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeyPreview = true;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SQL Server Utility";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this._gbConnection.ResumeLayout(false);
            this._gbConnection.PerformLayout();
            this._gbFilePaths.ResumeLayout(false);
            this._gbFilePaths.PerformLayout();
            this._gbAttach.ResumeLayout(false);
            this._gbAttach.PerformLayout();
            this._gbDbOperations.ResumeLayout(false);
            this._gbDbInfo.ResumeLayout(false);
            this._gbDbInfo.PerformLayout();
            this._gbServerInfo.ResumeLayout(false);
            this._gbServerInfo.PerformLayout();
            this._tabControl.ResumeLayout(false);
            this._tabDbManager.ResumeLayout(false);
            this._splitDbManager.Panel1.ResumeLayout(false);
            this._splitDbManager.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._splitDbManager)).EndInit();
            this._splitDbManager.ResumeLayout(false);
            this._flowPanelRight.ResumeLayout(false);
            this._gbFiscalContainer.ResumeLayout(false);
            this._gbFiscalContainer.PerformLayout();
            this._tabQueryWorkbook.ResumeLayout(false);
            this._splitContainer.Panel1.ResumeLayout(false);
            this._splitContainer.Panel1.PerformLayout();
            this._splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._splitContainer)).EndInit();
            this._splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._dgvResults)).EndInit();
            this._gbSystemInfo.ResumeLayout(false);
            this._gbSystemInfo.PerformLayout();
            this._panelTop.ResumeLayout(false);
            this._panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _panelTop;
        private System.Windows.Forms.Panel _panelBottom;
        private System.Windows.Forms.GroupBox _gbConnection;
        private System.Windows.Forms.RadioButton _rbWindowsAuth;
        private System.Windows.Forms.RadioButton _rbSqlAuth;
        private System.Windows.Forms.Label _lblServer;
        private System.Windows.Forms.TextBox _txtServer;
        private System.Windows.Forms.Label _lblUser;
        private System.Windows.Forms.TextBox _txtUser;
        private System.Windows.Forms.Label _lblPassword;
        private System.Windows.Forms.TextBox _txtPassword;
        private System.Windows.Forms.Button _btnConnect;
        private System.Windows.Forms.Button _btnDisconnect;
        private System.Windows.Forms.ListBox _lbDatabases;
        private System.Windows.Forms.Button _btnBackup;
        private System.Windows.Forms.Button _btnDelete;
        private System.Windows.Forms.Button _btnCheckDb;
        private System.Windows.Forms.Button _btnDetach;
        private System.Windows.Forms.RichTextBox _rtbLog;
        private System.Windows.Forms.GroupBox _gbFilePaths;
        private System.Windows.Forms.Label _lblMdf;
        private System.Windows.Forms.TextBox _txtMdfPath;
        private System.Windows.Forms.Label _lblLdf;
        private System.Windows.Forms.TextBox _txtLdfPath;
        private System.Windows.Forms.Button _btnOpenDir;
        private System.Windows.Forms.GroupBox _gbAttach;
        private System.Windows.Forms.Label _lblAttachMdf;
        private System.Windows.Forms.TextBox _txtAttachMdf;
        private System.Windows.Forms.Button _btnBrowseMdf;
        private System.Windows.Forms.Label _lblAttachLdf;
        private System.Windows.Forms.TextBox _txtAttachLdf;
        private System.Windows.Forms.Button _btnBrowseLdf;
        private System.Windows.Forms.Button _btnAttach;
        private System.Windows.Forms.Button _btnRestore;
        private System.Windows.Forms.Label _lblDbVersion;
        private System.Windows.Forms.Label _lblCompanyName;
        private System.Windows.Forms.ListBox _lbFiscalYears;
        private System.Windows.Forms.Label _lblFiscalYears;
        private System.Windows.Forms.Label _lblDbType;
        private System.Windows.Forms.ToolTip _toolTip;
        private System.Windows.Forms.Label _lblDatabases;
        private System.Windows.Forms.Button _btnAbout;
        private System.Windows.Forms.Label _lblActivationCode;
        private System.Windows.Forms.Button _btnBackupVerifyChecksum;
        private System.Windows.Forms.ProgressBar _progressBar;
        private System.Windows.Forms.Button _btnBrowseServers;
        private System.Windows.Forms.GroupBox _gbDbOperations;
        private System.Windows.Forms.Button _btnSchemaCheck;
        private System.Windows.Forms.Button _btnCheckTriggers;
        private System.Windows.Forms.Label _lblUserAccessMode;
        private System.Windows.Forms.Label _lblServerName;
        private System.Windows.Forms.Label _lblServiceName;
        private System.Windows.Forms.Label _lblConnections;
        private System.Windows.Forms.Label _lblTranCount;
        private System.Windows.Forms.Label _lblLanguage;
        private System.Windows.Forms.Label _lblCollation;
        private System.Windows.Forms.GroupBox _gbDbInfo;
        private System.Windows.Forms.GroupBox _gbServerInfo;
        private System.Windows.Forms.TabControl _tabControl;
        private System.Windows.Forms.TabPage _tabDbManager;
        private System.Windows.Forms.TabPage _tabQueryWorkbook;
        private System.Windows.Forms.SplitContainer _splitContainer;
        private System.Windows.Forms.TextBox _txtQuery;
        private System.Windows.Forms.DataGridView _dgvResults;
        private System.Windows.Forms.Button _btnExecuteQuery;
        private System.Windows.Forms.Button _btnSaveQuery;
        private System.Windows.Forms.Button _btnOpenQuery;
        private System.Windows.Forms.ComboBox _cmbDatabasesQuery;
        private System.Windows.Forms.Label _lblQueryDb;
        private System.Windows.Forms.GroupBox _gbSystemInfo;
        private System.Windows.Forms.Label _lblSqlVersion;
        private System.Windows.Forms.Label _lblLdfDrive;
        private System.Windows.Forms.Label _lblMdfDrive;
        private System.Windows.Forms.TextBox _txtSystemDetails;
        private System.Windows.Forms.Label _lblDbSize;
        private System.Windows.Forms.SplitContainer _splitDbManager;
        private System.Windows.Forms.FlowLayoutPanel _flowPanelRight;
        private System.Windows.Forms.GroupBox _gbFiscalContainer;
    }
}