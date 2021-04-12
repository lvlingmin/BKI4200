namespace BioBaseCLIA.SysMaintenance
{
    partial class frmReconfirm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmReconfirm));
            this.txtUserPassword = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.txtUserName = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.fbtnCancel = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnConfirm = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.SuspendLayout();
            // 
            // txtUserPassword
            // 
            this.txtUserPassword.Location = new System.Drawing.Point(88, 55);
            this.txtUserPassword.Name = "txtUserPassword";
            this.txtUserPassword.PasswordChar = '*';
            this.txtUserPassword.Size = new System.Drawing.Size(150, 21);
            this.txtUserPassword.TabIndex = 9;
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(88, 28);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(150, 21);
            this.txtUserName.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "密  码：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "用户名：";
            // 
            // fbtnCancel
            // 
            this.fbtnCancel.BackColor = System.Drawing.Color.Transparent;
            this.fbtnCancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("fbtnCancel.BackgroundImage")));
            this.fbtnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.fbtnCancel.EnabledSet = true;
            this.fbtnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnCancel.FlatAppearance.BorderSize = 0;
            this.fbtnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fbtnCancel.Location = new System.Drawing.Point(173, 94);
            this.fbtnCancel.Name = "fbtnCancel";
            this.fbtnCancel.Size = new System.Drawing.Size(65, 23);
            this.fbtnCancel.TabIndex = 11;
            this.fbtnCancel.Text = "取消";
            this.fbtnCancel.UseVisualStyleBackColor = false;
            this.fbtnCancel.Click += new System.EventHandler(this.fbtnCancel_Click);
            // 
            // fbtnConfirm
            // 
            this.fbtnConfirm.BackColor = System.Drawing.Color.Transparent;
            this.fbtnConfirm.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("fbtnConfirm.BackgroundImage")));
            this.fbtnConfirm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.fbtnConfirm.EnabledSet = true;
            this.fbtnConfirm.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnConfirm.FlatAppearance.BorderSize = 0;
            this.fbtnConfirm.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnConfirm.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnConfirm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fbtnConfirm.Location = new System.Drawing.Point(88, 94);
            this.fbtnConfirm.Name = "fbtnConfirm";
            this.fbtnConfirm.Size = new System.Drawing.Size(65, 23);
            this.fbtnConfirm.TabIndex = 10;
            this.fbtnConfirm.Text = "确认";
            this.fbtnConfirm.UseVisualStyleBackColor = false;
            this.fbtnConfirm.Click += new System.EventHandler(this.fbtnConfirm_Click);
            // 
            // frmReconfirm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightBlue;
            this.ClientSize = new System.Drawing.Size(280, 142);
            this.Controls.Add(this.fbtnCancel);
            this.Controls.Add(this.fbtnConfirm);
            this.Controls.Add(this.txtUserPassword);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmReconfirm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "配置初始化-请输入确认密码";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmReconfirm_FormClosed);
            this.Load += new System.EventHandler(this.frmReconfirm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CustomControl.FunctionButton fbtnCancel;
        private CustomControl.FunctionButton fbtnConfirm;
        private CustomControl.userTextBoxBase txtUserPassword;
        private CustomControl.userTextBoxBase txtUserName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}