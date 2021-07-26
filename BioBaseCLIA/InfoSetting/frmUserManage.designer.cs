namespace BioBaseCLIA.InfoSetting
{
    partial class frmUserManage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUserManage));
            this.dgvUserInfo = new System.Windows.Forms.DataGridView();
            this.No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UserName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UserRoleType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Password = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSave = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.txtPassword = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.txtConfirmPassword = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.txtName = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDel = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnAdd = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnModifyPassword = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnUserInfo = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnConnetSet = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnInstrumentPara = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnProInfo = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnReturn = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUserInfo)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvUserInfo
            // 
            resources.ApplyResources(this.dgvUserInfo, "dgvUserInfo");
            this.dgvUserInfo.AllowUserToAddRows = false;
            this.dgvUserInfo.AllowUserToDeleteRows = false;
            this.dgvUserInfo.AllowUserToResizeRows = false;
            this.dgvUserInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUserInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.No,
            this.UserName,
            this.UserRoleType,
            this.Password});
            this.dgvUserInfo.MultiSelect = false;
            this.dgvUserInfo.Name = "dgvUserInfo";
            this.dgvUserInfo.ReadOnly = true;
            this.dgvUserInfo.RowTemplate.Height = 23;
            this.dgvUserInfo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUserInfo.SelectionChanged += new System.EventHandler(this.dgvUserInfo_SelectionChanged);
            // 
            // No
            // 
            this.No.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.No.DataPropertyName = "No";
            this.No.FillWeight = 50F;
            resources.ApplyResources(this.No, "No");
            this.No.Name = "No";
            this.No.ReadOnly = true;
            // 
            // UserName
            // 
            this.UserName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.UserName.DataPropertyName = "UserName";
            resources.ApplyResources(this.UserName, "UserName");
            this.UserName.Name = "UserName";
            this.UserName.ReadOnly = true;
            // 
            // UserRoleType
            // 
            this.UserRoleType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.UserRoleType.DataPropertyName = "UserRoleType";
            resources.ApplyResources(this.UserRoleType, "UserRoleType");
            this.UserRoleType.Name = "UserRoleType";
            this.UserRoleType.ReadOnly = true;
            // 
            // Password
            // 
            this.Password.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Password.DataPropertyName = "UserPassword";
            resources.ApplyResources(this.Password, "Password");
            this.Password.Name = "Password";
            this.Password.ReadOnly = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.dgvUserInfo);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.btnSave);
            this.groupBox2.Controls.Add(this.cmbType);
            this.groupBox2.Controls.Add(this.txtPassword);
            this.groupBox2.Controls.Add(this.txtConfirmPassword);
            this.groupBox2.Controls.Add(this.txtName);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.btnDel);
            this.groupBox2.Controls.Add(this.btnAdd);
            this.groupBox2.Controls.Add(this.btnModifyPassword);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.EnabledSet = true;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cmbType
            // 
            resources.ApplyResources(this.cmbType, "cmbType");
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            resources.GetString("cmbType.Items"),
            resources.GetString("cmbType.Items1")});
            this.cmbType.Name = "cmbType";
            // 
            // txtPassword
            // 
            resources.ApplyResources(this.txtPassword, "txtPassword");
            this.txtPassword.Name = "txtPassword";
            // 
            // txtConfirmPassword
            // 
            resources.ApplyResources(this.txtConfirmPassword, "txtConfirmPassword");
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            // 
            // txtName
            // 
            resources.ApplyResources(this.txtName, "txtName");
            this.txtName.Name = "txtName";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // btnDel
            // 
            resources.ApplyResources(this.btnDel, "btnDel");
            this.btnDel.BackColor = System.Drawing.Color.Transparent;
            this.btnDel.EnabledSet = true;
            this.btnDel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnDel.FlatAppearance.BorderSize = 0;
            this.btnDel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnDel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnDel.Name = "btnDel";
            this.btnDel.UseVisualStyleBackColor = false;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnAdd
            // 
            resources.ApplyResources(this.btnAdd, "btnAdd");
            this.btnAdd.BackColor = System.Drawing.Color.Transparent;
            this.btnAdd.EnabledSet = true;
            this.btnAdd.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnAdd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnModifyPassword
            // 
            resources.ApplyResources(this.btnModifyPassword, "btnModifyPassword");
            this.btnModifyPassword.BackColor = System.Drawing.Color.Transparent;
            this.btnModifyPassword.EnabledSet = true;
            this.btnModifyPassword.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnModifyPassword.FlatAppearance.BorderSize = 0;
            this.btnModifyPassword.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnModifyPassword.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnModifyPassword.Name = "btnModifyPassword";
            this.btnModifyPassword.UseVisualStyleBackColor = false;
            this.btnModifyPassword.Click += new System.EventHandler(this.btnModifyPassword_Click);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.btnUserInfo);
            this.panel1.Controls.Add(this.fbtnConnetSet);
            this.panel1.Controls.Add(this.btnInstrumentPara);
            this.panel1.Controls.Add(this.btnProInfo);
            this.panel1.Controls.Add(this.fbtnReturn);
            this.panel1.Name = "panel1";
            // 
            // btnUserInfo
            // 
            resources.ApplyResources(this.btnUserInfo, "btnUserInfo");
            this.btnUserInfo.BackColor = System.Drawing.Color.Transparent;
            this.btnUserInfo.EnabledSet = true;
            this.btnUserInfo.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnUserInfo.FlatAppearance.BorderSize = 0;
            this.btnUserInfo.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnUserInfo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnUserInfo.Name = "btnUserInfo";
            this.btnUserInfo.UseVisualStyleBackColor = false;
            // 
            // fbtnConnetSet
            // 
            resources.ApplyResources(this.fbtnConnetSet, "fbtnConnetSet");
            this.fbtnConnetSet.BackColor = System.Drawing.Color.Transparent;
            this.fbtnConnetSet.EnabledSet = true;
            this.fbtnConnetSet.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnConnetSet.FlatAppearance.BorderSize = 0;
            this.fbtnConnetSet.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnConnetSet.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnConnetSet.Name = "fbtnConnetSet";
            this.fbtnConnetSet.UseVisualStyleBackColor = false;
            this.fbtnConnetSet.Click += new System.EventHandler(this.fbtnConnetSet_Click);
            // 
            // btnInstrumentPara
            // 
            resources.ApplyResources(this.btnInstrumentPara, "btnInstrumentPara");
            this.btnInstrumentPara.BackColor = System.Drawing.Color.Transparent;
            this.btnInstrumentPara.EnabledSet = true;
            this.btnInstrumentPara.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnInstrumentPara.FlatAppearance.BorderSize = 0;
            this.btnInstrumentPara.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnInstrumentPara.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnInstrumentPara.Name = "btnInstrumentPara";
            this.btnInstrumentPara.UseVisualStyleBackColor = false;
            this.btnInstrumentPara.Click += new System.EventHandler(this.btnInstrumentPara_Click);
            // 
            // btnProInfo
            // 
            resources.ApplyResources(this.btnProInfo, "btnProInfo");
            this.btnProInfo.BackColor = System.Drawing.Color.Transparent;
            this.btnProInfo.EnabledSet = true;
            this.btnProInfo.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnProInfo.FlatAppearance.BorderSize = 0;
            this.btnProInfo.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnProInfo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnProInfo.Name = "btnProInfo";
            this.btnProInfo.UseVisualStyleBackColor = false;
            this.btnProInfo.Click += new System.EventHandler(this.btnProInfo_Click);
            // 
            // fbtnReturn
            // 
            resources.ApplyResources(this.fbtnReturn, "fbtnReturn");
            this.fbtnReturn.BackColor = System.Drawing.Color.Transparent;
            this.fbtnReturn.EnabledSet = true;
            this.fbtnReturn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnReturn.FlatAppearance.BorderSize = 0;
            this.fbtnReturn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnReturn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnReturn.Name = "fbtnReturn";
            this.fbtnReturn.UseVisualStyleBackColor = false;
            this.fbtnReturn.Click += new System.EventHandler(this.fbtnReturn_Click);
            // 
            // frmUserManage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Name = "frmUserManage";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmUserManage_Load);
            this.SizeChanged += new System.EventHandler(this.frmUserManage_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUserInfo)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvUserInfo;
        private System.Windows.Forms.Label label1;
        private CustomControl.FunctionButton btnAdd;
        private CustomControl.FunctionButton btnDel;
        private CustomControl.FunctionButton btnModifyPassword;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cmbType;
        private CustomControl.userTextBoxBase txtPassword;
        private CustomControl.userTextBoxBase txtConfirmPassword;
        private CustomControl.userTextBoxBase txtName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private CustomControl.FunctionButton btnSave;
        private System.Windows.Forms.Panel panel1;
        private CustomControl.FunctionButton btnUserInfo;
        private CustomControl.FunctionButton fbtnConnetSet;
        private CustomControl.FunctionButton btnInstrumentPara;
        private CustomControl.FunctionButton btnProInfo;
        private CustomControl.FunctionButton fbtnReturn;
        private System.Windows.Forms.DataGridViewTextBoxColumn No;
        private System.Windows.Forms.DataGridViewTextBoxColumn UserName;
        private System.Windows.Forms.DataGridViewTextBoxColumn UserRoleType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Password;
    }
}