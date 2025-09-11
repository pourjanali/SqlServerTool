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
            this._btnBrowseServers = new System.Windows.Forms.Button();
            this._btnDisconnect = new System.Windows.Forms.Button();
            this._rbWindowsAuth = new System.Windows.Forms.RadioButton();
            this._rbSqlAuth = new System.Windows.Forms.RadioButton();
            this._lblServer = new System.Windows.Forms.Label();
            this._txtServer = new System.Windows.Forms.TextBox();
            this._lblUser = new System.Windows.Forms.Label();
            this._txtUser = new System.Windows.Forms.TextBox();
            this._lblPassword = new System.Windows.Forms.Label();
            this._txtPassword = new System.Windows.Forms.TextBox();
            this._btnConnect = new System.Windows.Forms.Button();
            this._lblVersion = new System.Windows.Forms.Label();
            this._lbDatabases = new System.Windows.Forms.ListBox();
            this._btnBackup = new System.Windows.Forms.Button();
            this._btnDelete = new System.Windows.Forms.Button();
            this._btnCheckDb = new System.Windows.Forms.Button();
            this._btnDetach = new System.Windows.Forms.Button();
            this._rtbLog = new System.Windows.Forms.RichTextBox();
            this._gbFilePaths = new System.Windows.Forms.GroupBox();
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
            this._txtAttachDbName = new System.Windows.Forms.TextBox();
            this._lblAttachDbName = new System.Windows.Forms.Label();
            this._btnRestore = new System.Windows.Forms.Button();
            this._lblDbVersion = new System.Windows.Forms.Label();
            this._lblCompanyName = new System.Windows.Forms.Label();
            this._lbFiscalYears = new System.Windows.Forms.ListBox();
            this._lblFiscalYears = new System.Windows.Forms.Label();
            this._lblDbType = new System.Windows.Forms.Label();
            this._toolTip = new System.Windows.Forms.ToolTip(this.components);
            this._lblDatabases = new System.Windows.Forms.Label();
            this._btnAbout = new System.Windows.Forms.Button();
            this._lblActivationCode = new System.Windows.Forms.Label();
            this._btnBackupVerifyChecksum = new System.Windows.Forms.Button();
            this._progressBar = new System.Windows.Forms.ProgressBar();
            this._gbDbOperations = new System.Windows.Forms.GroupBox();
            this._btnSchemaCheck = new System.Windows.Forms.Button();
            this._gbConnection.SuspendLayout();
            this._gbFilePaths.SuspendLayout();
            this._gbAttach.SuspendLayout();
            this._gbDbOperations.SuspendLayout();
            this.SuspendLayout();
            // 
            // _gbConnection
            // 
            this._gbConnection.Controls.Add(this._btnBrowseServers);
            this._gbConnection.Controls.Add(this._rbWindowsAuth);
            this._gbConnection.Controls.Add(this._rbSqlAuth);
            this._gbConnection.Controls.Add(this._lblServer);
            this._gbConnection.Controls.Add(this._txtServer);
            this._gbConnection.Controls.Add(this._lblUser);
            this._gbConnection.Controls.Add(this._txtUser);
            this._gbConnection.Controls.Add(this._lblPassword);
            this._gbConnection.Controls.Add(this._txtPassword);
            this._gbConnection.Location = new System.Drawing.Point(12, 12);
            this._gbConnection.Name = "_gbConnection";
            this._gbConnection.Size = new System.Drawing.Size(350, 150);
            this._gbConnection.TabIndex = 0;
            this._gbConnection.TabStop = false;
            this._gbConnection.Text = "Connection";
            // 
            // _btnBrowseServers
            // 
            this._btnBrowseServers.Location = new System.Drawing.Point(308, 57);
            this._btnBrowseServers.Name = "_btnBrowseServers";
            this._btnBrowseServers.Size = new System.Drawing.Size(30, 25);
            this._btnBrowseServers.TabIndex = 3;
            this._btnBrowseServers.Text = "...";
            this._toolTip.SetToolTip(this._btnBrowseServers, "Browse for SQL Server instances");
            this._btnBrowseServers.UseVisualStyleBackColor = true;
            this._btnBrowseServers.Click += new System.EventHandler(this._btnBrowseServers_Click);
            // 
            // _btnDisconnect
            // 
            this._btnDisconnect.Location = new System.Drawing.Point(282, 168);
            this._btnDisconnect.Name = "_btnDisconnect";
            this._btnDisconnect.Size = new System.Drawing.Size(80, 28);
            this._btnDisconnect.TabIndex = 9;
            this._btnDisconnect.Text = "Disconnect";
            this._btnDisconnect.UseVisualStyleBackColor = true;
            this._btnDisconnect.Click += new System.EventHandler(this._btnDisconnect_Click);
            // 
            // _rbWindowsAuth
            // 
            this._rbWindowsAuth.AutoSize = true;
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
            this._rbSqlAuth.Location = new System.Drawing.Point(180, 25);
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
            this._lblServer.Location = new System.Drawing.Point(15, 60);
            this._lblServer.Name = "_lblServer";
            this._lblServer.Size = new System.Drawing.Size(42, 15);
            this._lblServer.TabIndex = 2;
            this._lblServer.Text = "Server:";
            // 
            // _txtServer
            // 
            this._txtServer.Location = new System.Drawing.Point(90, 58);
            this._txtServer.Name = "_txtServer";
            this._txtServer.Size = new System.Drawing.Size(212, 23);
            this._txtServer.TabIndex = 2;
            // 
            // _lblUser
            // 
            this._lblUser.AutoSize = true;
            this._lblUser.Location = new System.Drawing.Point(15, 90);
            this._lblUser.Name = "_lblUser";
            this._lblUser.Size = new System.Drawing.Size(33, 15);
            this._lblUser.TabIndex = 4;
            this._lblUser.Text = "User:";
            // 
            // _txtUser
            // 
            this._txtUser.Location = new System.Drawing.Point(90, 88);
            this._txtUser.Name = "_txtUser";
            this._txtUser.Size = new System.Drawing.Size(248, 23);
            this._txtUser.TabIndex = 4;
            // 
            // _lblPassword
            // 
            this._lblPassword.AutoSize = true;
            this._lblPassword.Location = new System.Drawing.Point(15, 120);
            this._lblPassword.Name = "_lblPassword";
            this._lblPassword.Size = new System.Drawing.Size(60, 15);
            this._lblPassword.TabIndex = 6;
            this._lblPassword.Text = "Password:";
            // 
            // _txtPassword
            // 
            this._txtPassword.Location = new System.Drawing.Point(90, 118);
            this._txtPassword.Name = "_txtPassword";
            this._txtPassword.Size = new System.Drawing.Size(248, 23);
            this._txtPassword.TabIndex = 5;
            this._txtPassword.UseSystemPasswordChar = true;
            // 
            // _btnConnect
            // 
            this._btnConnect.Location = new System.Drawing.Point(196, 168);
            this._btnConnect.Name = "_btnConnect";
            this._btnConnect.Size = new System.Drawing.Size(80, 28);
            this._btnConnect.TabIndex = 8;
            this._btnConnect.Text = "Connect";
            this._btnConnect.UseVisualStyleBackColor = true;
            this._btnConnect.Click += new System.EventHandler(this._btnConnect_Click);
            // 
            // _lblVersion
            // 
            this._lblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this._lblVersion.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._lblVersion.Location = new System.Drawing.Point(380, 15);
            this._lblVersion.Name = "_lblVersion";
            this._lblVersion.Size = new System.Drawing.Size(592, 23);
            this._lblVersion.TabIndex = 1;
            this._lblVersion.Text = "Version: N/A";
            this._lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _lbDatabases
            // 
            this._lbDatabases.FormattingEnabled = true;
            this._lbDatabases.ItemHeight = 15;
            this._lbDatabases.Location = new System.Drawing.Point(380, 153);
            this._lbDatabases.Name = "_lbDatabases";
            this._lbDatabases.Size = new System.Drawing.Size(280, 139);
            this._lbDatabases.TabIndex = 2;
            this._lbDatabases.SelectedIndexChanged += new System.EventHandler(this._lbDatabases_SelectedIndexChanged);
            // 
            // _btnBackup
            // 
            this._btnBackup.Location = new System.Drawing.Point(6, 22);
            this._btnBackup.Name = "_btnBackup";
            this._btnBackup.Size = new System.Drawing.Size(110, 30);
            this._btnBackup.TabIndex = 3;
            this._btnBackup.Text = "Backup";
            this._btnBackup.UseVisualStyleBackColor = true;
            this._btnBackup.Click += new System.EventHandler(this._btnBackup_Click);
            // 
            // _btnDelete
            // 
            this._btnDelete.BackColor = System.Drawing.Color.LightCoral;
            this._btnDelete.Location = new System.Drawing.Point(6, 94);
            this._btnDelete.Name = "_btnDelete";
            this._btnDelete.Size = new System.Drawing.Size(342, 30);
            this._btnDelete.TabIndex = 4;
            this._btnDelete.Text = "Delete DB";
            this._btnDelete.UseVisualStyleBackColor = false;
            this._btnDelete.Click += new System.EventHandler(this._btnDelete_Click);
            // 
            // _btnCheckDb
            // 
            this._btnCheckDb.BackColor = System.Drawing.Color.LightSkyBlue;
            this._btnCheckDb.Location = new System.Drawing.Point(122, 58);
            this._btnCheckDb.Name = "_btnCheckDb";
            this._btnCheckDb.Size = new System.Drawing.Size(110, 30);
            this._btnCheckDb.TabIndex = 11;
            this._btnCheckDb.Text = "Check Health";
            this._toolTip.SetToolTip(this._btnCheckDb, "Runs DBCC CHECKDB to verify database integrity.");
            this._btnCheckDb.UseVisualStyleBackColor = false;
            this._btnCheckDb.Click += new System.EventHandler(this._btnCheckDb_Click);
            // 
            // _btnDetach
            // 
            this._btnDetach.BackColor = System.Drawing.Color.Khaki;
            this._btnDetach.Location = new System.Drawing.Point(238, 58);
            this._btnDetach.Name = "_btnDetach";
            this._btnDetach.Size = new System.Drawing.Size(110, 30);
            this._btnDetach.TabIndex = 12;
            this._btnDetach.Text = "Detach DB";
            this._btnDetach.UseVisualStyleBackColor = false;
            this._btnDetach.Click += new System.EventHandler(this._btnDetach_Click);
            // 
            // _rtbLog
            // 
            this._rtbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this._rtbLog.BackColor = System.Drawing.Color.White;
            this._rtbLog.Location = new System.Drawing.Point(12, 532);
            this._rtbLog.Name = "_rtbLog";
            this._rtbLog.ReadOnly = true;
            this._rtbLog.Size = new System.Drawing.Size(960, 73);
            this._rtbLog.TabIndex = 7;
            this._rtbLog.Text = "";
            // 
            // _gbFilePaths
            // 
            this._gbFilePaths.Controls.Add(this._btnOpenDir);
            this._gbFilePaths.Controls.Add(this._txtLdfPath);
            this._gbFilePaths.Controls.Add(this._lblLdf);
            this._gbFilePaths.Controls.Add(this._txtMdfPath);
            this._gbFilePaths.Controls.Add(this._lblMdf);
            this._gbFilePaths.Location = new System.Drawing.Point(12, 340);
            this._gbFilePaths.Name = "_gbFilePaths";
            this._gbFilePaths.Size = new System.Drawing.Size(960, 90);
            this._gbFilePaths.TabIndex = 5;
            this._gbFilePaths.TabStop = false;
            this._gbFilePaths.Text = "Selected Database Files";
            // 
            // _btnOpenDir
            // 
            this._btnOpenDir.Location = new System.Drawing.Point(840, 22);
            this._btnOpenDir.Name = "_btnOpenDir";
            this._btnOpenDir.Size = new System.Drawing.Size(110, 50);
            this._btnOpenDir.TabIndex = 4;
            this._btnOpenDir.Text = "Open Location";
            this._btnOpenDir.UseVisualStyleBackColor = true;
            this._btnOpenDir.Click += new System.EventHandler(this._btnOpenDir_Click);
            // 
            // _txtLdfPath
            // 
            this._txtLdfPath.Location = new System.Drawing.Point(90, 52);
            this._txtLdfPath.Name = "_txtLdfPath";
            this._txtLdfPath.ReadOnly = true;
            this._txtLdfPath.Size = new System.Drawing.Size(740, 23);
            this._txtLdfPath.TabIndex = 3;
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
            this._txtMdfPath.Location = new System.Drawing.Point(90, 22);
            this._txtMdfPath.Name = "_txtMdfPath";
            this._txtMdfPath.ReadOnly = true;
            this._txtMdfPath.Size = new System.Drawing.Size(740, 23);
            this._txtMdfPath.TabIndex = 1;
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
            this._gbAttach.Controls.Add(this._txtAttachDbName);
            this._gbAttach.Controls.Add(this._lblAttachDbName);
            this._gbAttach.Location = new System.Drawing.Point(12, 436);
            this._gbAttach.Name = "_gbAttach";
            this._gbAttach.Size = new System.Drawing.Size(960, 59);
            this._gbAttach.TabIndex = 6;
            this._gbAttach.TabStop = false;
            this._gbAttach.Text = "Attach Database";
            // 
            // _btnAttach
            // 
            this._btnAttach.BackColor = System.Drawing.Color.LightGreen;
            this._btnAttach.Location = new System.Drawing.Point(840, 17);
            this._btnAttach.Name = "_btnAttach";
            this._btnAttach.Size = new System.Drawing.Size(110, 30);
            this._btnAttach.TabIndex = 8;
            this._btnAttach.Text = "Attach Database";
            this._btnAttach.UseVisualStyleBackColor = false;
            this._btnAttach.Click += new System.EventHandler(this._btnAttach_Click);
            // 
            // _btnBrowseLdf
            // 
            this._btnBrowseLdf.Location = new System.Drawing.Point(800, 48);
            this._btnBrowseLdf.Name = "_btnBrowseLdf";
            this._btnBrowseLdf.Size = new System.Drawing.Size(30, 25);
            this._btnBrowseLdf.TabIndex = 7;
            this._btnBrowseLdf.Text = "...";
            this._btnBrowseLdf.UseVisualStyleBackColor = true;
            this._btnBrowseLdf.Visible = false;
            this._btnBrowseLdf.Click += new System.EventHandler(this._btnBrowseLdf_Click);
            // 
            // _txtAttachLdf
            // 
            this._txtAttachLdf.Location = new System.Drawing.Point(461, 22);
            this._txtAttachLdf.Name = "_txtAttachLdf";
            this._txtAttachLdf.Size = new System.Drawing.Size(329, 23);
            this._txtAttachLdf.TabIndex = 6;
            // 
            // _lblAttachLdf
            // 
            this._lblAttachLdf.AutoSize = true;
            this._lblAttachLdf.Location = new System.Drawing.Point(404, 25);
            this._lblAttachLdf.Name = "_lblAttachLdf";
            this._lblAttachLdf.Size = new System.Drawing.Size(51, 15);
            this._lblAttachLdf.TabIndex = 5;
            this._lblAttachLdf.Text = "LDF File:";
            // 
            // _btnBrowseMdf
            // 
            this._btnBrowseMdf.Location = new System.Drawing.Point(800, 17);
            this._btnBrowseMdf.Name = "_btnBrowseMdf";
            this._btnBrowseMdf.Size = new System.Drawing.Size(30, 25);
            this._btnBrowseMdf.TabIndex = 4;
            this._btnBrowseMdf.Text = "...";
            this._btnBrowseMdf.UseVisualStyleBackColor = true;
            this._btnBrowseMdf.Click += new System.EventHandler(this._btnBrowseMdf_Click);
            // 
            // _txtAttachMdf
            // 
            this._txtAttachMdf.Location = new System.Drawing.Point(82, 22);
            this._txtAttachMdf.Name = "_txtAttachMdf";
            this._txtAttachMdf.Size = new System.Drawing.Size(316, 23);
            this._txtAttachMdf.TabIndex = 3;
            // 
            // _lblAttachMdf
            // 
            this._lblAttachMdf.AutoSize = true;
            this._lblAttachMdf.Location = new System.Drawing.Point(15, 25);
            this._lblAttachMdf.Name = "_lblAttachMdf";
            this._lblAttachMdf.Size = new System.Drawing.Size(56, 15);
            this._lblAttachMdf.TabIndex = 2;
            this._lblAttachMdf.Text = "MDF File:";
            // 
            // _txtAttachDbName
            // 
            this._txtAttachDbName.Location = new System.Drawing.Point(120, -4);
            this._txtAttachDbName.Name = "_txtAttachDbName";
            this._txtAttachDbName.Size = new System.Drawing.Size(250, 23);
            this._txtAttachDbName.TabIndex = 1;
            this._txtAttachDbName.Visible = false;
            // 
            // _lblAttachDbName
            // 
            this._lblAttachDbName.AutoSize = true;
            this._lblAttachDbName.Location = new System.Drawing.Point(15, -1);
            this._lblAttachDbName.Name = "_lblAttachDbName";
            this._lblAttachDbName.Size = new System.Drawing.Size(87, 15);
            this._lblAttachDbName.TabIndex = 0;
            this._lblAttachDbName.Text = "New DB Name:";
            this._lblAttachDbName.Visible = false;
            // 
            // _btnRestore
            // 
            this._btnRestore.BackColor = System.Drawing.Color.MediumAquamarine;
            this._btnRestore.Location = new System.Drawing.Point(6, 58);
            this._btnRestore.Name = "_btnRestore";
            this._btnRestore.Size = new System.Drawing.Size(110, 30);
            this._btnRestore.TabIndex = 13;
            this._btnRestore.Text = "Restore DB";
            this._btnRestore.UseVisualStyleBackColor = false;
            this._btnRestore.Click += new System.EventHandler(this._btnRestore_Click);
            // 
            // _lblDbVersion
            // 
            this._lblDbVersion.AutoSize = true;
            this._lblDbVersion.Location = new System.Drawing.Point(380, 45);
            this._lblDbVersion.Name = "_lblDbVersion";
            this._lblDbVersion.Size = new System.Drawing.Size(95, 15);
            this._lblDbVersion.TabIndex = 14;
            this._lblDbVersion.Text = "DataVersion: N/A";
            // 
            // _lblCompanyName
            // 
            this._lblCompanyName.AutoSize = true;
            this._lblCompanyName.Location = new System.Drawing.Point(380, 65);
            this._lblCompanyName.Name = "_lblCompanyName";
            this._lblCompanyName.Size = new System.Drawing.Size(88, 15);
            this._lblCompanyName.TabIndex = 15;
            this._lblCompanyName.Text = "Company: N/A";
            // 
            // _lbFiscalYears
            // 
            this._lbFiscalYears.FormattingEnabled = true;
            this._lbFiscalYears.ItemHeight = 15;
            this._lbFiscalYears.Location = new System.Drawing.Point(670, 153);
            this._lbFiscalYears.Name = "_lbFiscalYears";
            this._lbFiscalYears.Size = new System.Drawing.Size(302, 139);
            this._lbFiscalYears.TabIndex = 16;
            // 
            // _lblFiscalYears
            // 
            this._lblFiscalYears.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._lblFiscalYears.Location = new System.Drawing.Point(670, 133);
            this._lblFiscalYears.Name = "_lblFiscalYears";
            this._lblFiscalYears.Size = new System.Drawing.Size(302, 15);
            this._lblFiscalYears.TabIndex = 17;
            this._lblFiscalYears.Text = "Fiscal Years";
            this._lblFiscalYears.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // _lblDbType
            // 
            this._lblDbType.AutoSize = true;
            this._lblDbType.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._lblDbType.Location = new System.Drawing.Point(380, 85);
            this._lblDbType.Name = "_lblDbType";
            this._lblDbType.Size = new System.Drawing.Size(61, 15);
            this._lblDbType.TabIndex = 18;
            this._lblDbType.Text = "Type: N/A";
            // 
            // _lblDatabases
            // 
            this._lblDatabases.AutoSize = true;
            this._lblDatabases.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._lblDatabases.Location = new System.Drawing.Point(380, 133);
            this._lblDatabases.Name = "_lblDatabases";
            this._lblDatabases.Size = new System.Drawing.Size(64, 15);
            this._lblDatabases.TabIndex = 19;
            this._lblDatabases.Text = "Databases";
            // 
            // _btnAbout
            // 
            this._btnAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnAbout.Location = new System.Drawing.Point(892, 498);
            this._btnAbout.Name = "_btnAbout";
            this._btnAbout.Size = new System.Drawing.Size(80, 30);
            this._btnAbout.TabIndex = 20;
            this._btnAbout.Text = "About";
            this._btnAbout.UseVisualStyleBackColor = true;
            this._btnAbout.Click += new System.EventHandler(this._btnAbout_Click);
            // 
            // _lblActivationCode
            // 
            this._lblActivationCode.AutoSize = true;
            this._lblActivationCode.Location = new System.Drawing.Point(380, 105);
            this._lblActivationCode.Name = "_lblActivationCode";
            this._lblActivationCode.Size = new System.Drawing.Size(112, 15);
            this._lblActivationCode.TabIndex = 21;
            this._lblActivationCode.Text = "ActivationCode: N/A";
            // 
            // _btnBackupVerifyChecksum
            // 
            this._btnBackupVerifyChecksum.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._btnBackupVerifyChecksum.Location = new System.Drawing.Point(122, 22);
            this._btnBackupVerifyChecksum.Name = "_btnBackupVerifyChecksum";
            this._btnBackupVerifyChecksum.Size = new System.Drawing.Size(110, 30);
            this._btnBackupVerifyChecksum.TabIndex = 22;
            this._btnBackupVerifyChecksum.Text = "Backup + Verify";
            this._btnBackupVerifyChecksum.UseVisualStyleBackColor = true;
            this._btnBackupVerifyChecksum.Click += new System.EventHandler(this._btnBackupVerifyChecksum_Click);
            // 
            // _progressBar
            // 
            this._progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this._progressBar.Location = new System.Drawing.Point(12, 501);
            this._progressBar.Name = "_progressBar";
            this._progressBar.Size = new System.Drawing.Size(874, 23);
            this._progressBar.TabIndex = 23;
            // 
            // _gbDbOperations
            // 
            this._gbDbOperations.Controls.Add(this._btnSchemaCheck);
            this._gbDbOperations.Controls.Add(this._btnBackup);
            this._gbDbOperations.Controls.Add(this._btnRestore);
            this._gbDbOperations.Controls.Add(this._btnDelete);
            this._gbDbOperations.Controls.Add(this._btnBackupVerifyChecksum);
            this._gbDbOperations.Controls.Add(this._btnCheckDb);
            this._gbDbOperations.Controls.Add(this._btnDetach);
            this._gbDbOperations.Location = new System.Drawing.Point(12, 202);
            this._gbDbOperations.Name = "_gbDbOperations";
            this._gbDbOperations.Size = new System.Drawing.Size(354, 132);
            this._gbDbOperations.TabIndex = 24;
            this._gbDbOperations.TabStop = false;
            this._gbDbOperations.Text = "Database Operations";
            // 
            // _btnSchemaCheck
            // 
            this._btnSchemaCheck.BackColor = System.Drawing.Color.Thistle;
            this._btnSchemaCheck.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this._btnSchemaCheck.Location = new System.Drawing.Point(238, 22);
            this._btnSchemaCheck.Name = "_btnSchemaCheck";
            this._btnSchemaCheck.Size = new System.Drawing.Size(110, 30);
            this._btnSchemaCheck.TabIndex = 23;
            this._btnSchemaCheck.Text = "Check Schema";
            this._toolTip.SetToolTip(this._btnSchemaCheck, "Compares the database\'s table schemas against the expected standard for its type " +
        "(Dasht or Sepidar).");
            this._btnSchemaCheck.UseVisualStyleBackColor = false;
            this._btnSchemaCheck.Click += new System.EventHandler(this._btnSchemaCheck_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 611);
            this.Controls.Add(this._gbDbOperations);
            this.Controls.Add(this._progressBar);
            this.Controls.Add(this._lblActivationCode);
            this.Controls.Add(this._btnAbout);
            this.Controls.Add(this._lblDatabases);
            this.Controls.Add(this._lblDbType);
            this.Controls.Add(this._lblFiscalYears);
            this.Controls.Add(this._lbFiscalYears);
            this.Controls.Add(this._lblCompanyName);
            this.Controls.Add(this._lblDbVersion);
            this.Controls.Add(this._gbAttach);
            this.Controls.Add(this._gbFilePaths);
            this.Controls.Add(this._rtbLog);
            this.Controls.Add(this._lbDatabases);
            this.Controls.Add(this._lblVersion);
            this.Controls.Add(this._gbConnection);
            this.Controls.Add(this._btnConnect);
            this.Controls.Add(this._btnDisconnect);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SQL Server Utility";
            this._gbConnection.ResumeLayout(false);
            this._gbConnection.PerformLayout();
            this._gbFilePaths.ResumeLayout(false);
            this._gbFilePaths.PerformLayout();
            this._gbAttach.ResumeLayout(false);
            this._gbAttach.PerformLayout();
            this._gbDbOperations.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

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
        private System.Windows.Forms.Label _lblVersion;
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
        private System.Windows.Forms.Label _lblAttachDbName;
        private System.Windows.Forms.TextBox _txtAttachDbName;
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
    }
}

