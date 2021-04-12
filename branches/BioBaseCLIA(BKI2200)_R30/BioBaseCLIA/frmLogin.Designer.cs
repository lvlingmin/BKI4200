namespace BioBaseCLIA.User
{
    partial class frmLogin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogin));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.chineseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.engllishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.paProcess = new System.Windows.Forms.Panel();
            this.lblDescribe = new System.Windows.Forms.Label();
            this.progressData = new System.Windows.Forms.ProgressBar();
            this.logo = new System.Windows.Forms.PictureBox();
            this.ChangeLanguage = new System.Windows.Forms.PictureBox();
            this.panellogin = new System.Windows.Forms.Panel();
            this.chkKeepPwd = new System.Windows.Forms.CheckBox();
            this.cmbUserName = new System.Windows.Forms.ComboBox();
            this.btnCancel = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.btnLogin = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.txtUserPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.titleofbio = new System.Windows.Forms.PictureBox();
            this.lbLanguage = new System.Windows.Forms.Label();
            this.cbLanguage = new System.Windows.Forms.ComboBox();
            this.contextMenuStrip1.SuspendLayout();
            this.paProcess.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ChangeLanguage)).BeginInit();
            this.panellogin.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.titleofbio)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chineseToolStripMenuItem,
            this.engllishToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            // 
            // chineseToolStripMenuItem
            // 
            this.chineseToolStripMenuItem.Image = global::BioBaseCLIA.Properties.Resources.china;
            this.chineseToolStripMenuItem.Name = "chineseToolStripMenuItem";
            resources.ApplyResources(this.chineseToolStripMenuItem, "chineseToolStripMenuItem");
            this.chineseToolStripMenuItem.Click += new System.EventHandler(this.chineseToolStripMenuItem_Click);
            // 
            // engllishToolStripMenuItem
            // 
            this.engllishToolStripMenuItem.Image = global::BioBaseCLIA.Properties.Resources.english;
            this.engllishToolStripMenuItem.Name = "engllishToolStripMenuItem";
            resources.ApplyResources(this.engllishToolStripMenuItem, "engllishToolStripMenuItem");
            this.engllishToolStripMenuItem.Click += new System.EventHandler(this.engllishToolStripMenuItem_Click);
            // 
            // paProcess
            // 
            this.paProcess.BackColor = System.Drawing.Color.Transparent;
            this.paProcess.Controls.Add(this.lblDescribe);
            this.paProcess.Controls.Add(this.progressData);
            resources.ApplyResources(this.paProcess, "paProcess");
            this.paProcess.Name = "paProcess";
            // 
            // lblDescribe
            // 
            resources.ApplyResources(this.lblDescribe, "lblDescribe");
            this.lblDescribe.ForeColor = System.Drawing.Color.White;
            this.lblDescribe.Name = "lblDescribe";
            // 
            // progressData
            // 
            resources.ApplyResources(this.progressData, "progressData");
            this.progressData.Name = "progressData";
            // 
            // logo
            // 
            this.logo.BackColor = System.Drawing.Color.Transparent;
            this.logo.BackgroundImage = global::BioBaseCLIA.Properties.Resources.logo;
            resources.ApplyResources(this.logo, "logo");
            this.logo.Name = "logo";
            this.logo.TabStop = false;
            // 
            // ChangeLanguage
            // 
            resources.ApplyResources(this.ChangeLanguage, "ChangeLanguage");
            this.ChangeLanguage.BackColor = System.Drawing.Color.Transparent;
            this.ChangeLanguage.BackgroundImage = global::BioBaseCLIA.Properties.Resources.china;
            this.ChangeLanguage.ContextMenuStrip = this.contextMenuStrip1;
            this.ChangeLanguage.Name = "ChangeLanguage";
            this.ChangeLanguage.TabStop = false;
            // 
            // panellogin
            // 
            this.panellogin.BackColor = System.Drawing.Color.Transparent;
            this.panellogin.BackgroundImage = global::BioBaseCLIA.Properties.Resources.backgroundglass;
            resources.ApplyResources(this.panellogin, "panellogin");
            this.panellogin.Controls.Add(this.cbLanguage);
            this.panellogin.Controls.Add(this.chkKeepPwd);
            this.panellogin.Controls.Add(this.cmbUserName);
            this.panellogin.Controls.Add(this.btnCancel);
            this.panellogin.Controls.Add(this.label2);
            this.panellogin.Controls.Add(this.btnLogin);
            this.panellogin.Controls.Add(this.txtUserPassword);
            this.panellogin.Controls.Add(this.label3);
            this.panellogin.Controls.Add(this.lbLanguage);
            this.panellogin.Name = "panellogin";
            // 
            // chkKeepPwd
            // 
            resources.ApplyResources(this.chkKeepPwd, "chkKeepPwd");
            this.chkKeepPwd.ForeColor = System.Drawing.Color.White;
            this.chkKeepPwd.Name = "chkKeepPwd";
            this.chkKeepPwd.UseVisualStyleBackColor = true;
            // 
            // cmbUserName
            // 
            this.cmbUserName.FormattingEnabled = true;
            resources.ApplyResources(this.cmbUserName, "cmbUserName");
            this.cmbUserName.Name = "cmbUserName";
            this.cmbUserName.SelectedIndexChanged += new System.EventHandler(this.cmbUserName_SelectedIndexChanged);
            this.cmbUserName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbUserName_KeyDown);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.EnabledSet = true;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Name = "label2";
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnLogin, "btnLogin");
            this.btnLogin.EnabledSet = true;
            this.btnLogin.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnLogin.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // txtUserPassword
            // 
            resources.ApplyResources(this.txtUserPassword, "txtUserPassword");
            this.txtUserPassword.Name = "txtUserPassword";
            this.txtUserPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUserPassword_KeyDown);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Name = "label3";
            // 
            // titleofbio
            // 
            this.titleofbio.BackColor = System.Drawing.Color.Transparent;
            this.titleofbio.BackgroundImage = global::BioBaseCLIA.Properties.Resources.title;
            resources.ApplyResources(this.titleofbio, "titleofbio");
            this.titleofbio.Name = "titleofbio";
            this.titleofbio.TabStop = false;
            // 
            // lbLanguage
            // 
            this.lbLanguage.ForeColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.lbLanguage, "lbLanguage");
            this.lbLanguage.Name = "lbLanguage";
            // 
            // cbLanguage
            // 
            this.cbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLanguage.FormattingEnabled = true;
            this.cbLanguage.Items.AddRange(new object[] {
            resources.GetString("cbLanguage.Items"),
            resources.GetString("cbLanguage.Items1")});
            resources.ApplyResources(this.cbLanguage, "cbLanguage");
            this.cbLanguage.Name = "cbLanguage";
            this.cbLanguage.SelectedIndexChanged += new System.EventHandler(this.cbLanguage_SelectedIndexChanged);
            // 
            // frmLogin
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImage = global::BioBaseCLIA.Properties.Resources._1600x900_2_;
            this.Controls.Add(this.paProcess);
            this.Controls.Add(this.logo);
            this.Controls.Add(this.ChangeLanguage);
            this.Controls.Add(this.panellogin);
            this.Controls.Add(this.titleofbio);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Teal;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmLogin";
            this.ShowIcon = false;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Activated += new System.EventHandler(this.FrmLogin_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmLogin_FormClosed);
            this.Load += new System.EventHandler(this.frmLogin_Load);
            this.Leave += new System.EventHandler(this.FrmLogin_Leave);
            this.contextMenuStrip1.ResumeLayout(false);
            this.paProcess.ResumeLayout(false);
            this.paProcess.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ChangeLanguage)).EndInit();
            this.panellogin.ResumeLayout(false);
            this.panellogin.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.titleofbio)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUserPassword;
        private CustomControl.FunctionButton btnCancel;
        private CustomControl.FunctionButton btnLogin;
        private System.Windows.Forms.Panel panellogin;
        private System.Windows.Forms.PictureBox ChangeLanguage;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem chineseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem engllishToolStripMenuItem;
        private System.Windows.Forms.PictureBox logo;
        private System.Windows.Forms.Panel paProcess;
        private System.Windows.Forms.ProgressBar progressData;
        private System.Windows.Forms.Label lblDescribe;
        private System.Windows.Forms.ComboBox cmbUserName;
        private System.Windows.Forms.CheckBox chkKeepPwd;
        private System.Windows.Forms.PictureBox titleofbio;
        private System.Windows.Forms.ComboBox cbLanguage;
        private System.Windows.Forms.Label lbLanguage;
    }
}