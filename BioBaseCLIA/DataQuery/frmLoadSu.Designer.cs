namespace BioBaseCLIA.DataQuery
{
    partial class frmLoadSu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLoadSu));
            this.btnLoadSubstrate = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnChangeSubstrate = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.label8 = new System.Windows.Forms.Label();
            this.txtSubstrateCode = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.txtSubstrateLastTest = new BioBaseCLIA.CustomControl.userNumTextBox();
            this.txtSubstrateAllTest = new BioBaseCLIA.CustomControl.userNumTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ValidDate = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // btnLoadSubstrate
            // 
            this.btnLoadSubstrate.BackColor = System.Drawing.Color.Transparent;
            this.btnLoadSubstrate.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnLoadSubstrate.BackgroundImage")));
            this.btnLoadSubstrate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnLoadSubstrate.Enabled = false;
            this.btnLoadSubstrate.EnabledSet = true;
            this.btnLoadSubstrate.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnLoadSubstrate.FlatAppearance.BorderSize = 0;
            this.btnLoadSubstrate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnLoadSubstrate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnLoadSubstrate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadSubstrate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnLoadSubstrate.Location = new System.Drawing.Point(206, 172);
            this.btnLoadSubstrate.Name = "btnLoadSubstrate";
            this.btnLoadSubstrate.Size = new System.Drawing.Size(114, 37);
            this.btnLoadSubstrate.TabIndex = 21;
            this.btnLoadSubstrate.Text = "保存";
            this.btnLoadSubstrate.UseVisualStyleBackColor = false;
            this.btnLoadSubstrate.Click += new System.EventHandler(this.btnLoadSubstrate_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(52, 75);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 15;
            this.label7.Text = "总测数：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(40, 40);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 14;
            this.label6.Text = "底物条码：";
            // 
            // btnChangeSubstrate
            // 
            this.btnChangeSubstrate.BackColor = System.Drawing.Color.Transparent;
            this.btnChangeSubstrate.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnChangeSubstrate.BackgroundImage")));
            this.btnChangeSubstrate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnChangeSubstrate.EnabledSet = true;
            this.btnChangeSubstrate.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnChangeSubstrate.FlatAppearance.BorderSize = 0;
            this.btnChangeSubstrate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnChangeSubstrate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnChangeSubstrate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChangeSubstrate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnChangeSubstrate.Location = new System.Drawing.Point(12, 172);
            this.btnChangeSubstrate.Name = "btnChangeSubstrate";
            this.btnChangeSubstrate.Size = new System.Drawing.Size(113, 37);
            this.btnChangeSubstrate.TabIndex = 20;
            this.btnChangeSubstrate.Text = "装载底物";
            this.btnChangeSubstrate.UseVisualStyleBackColor = false;
            this.btnChangeSubstrate.Click += new System.EventHandler(this.btnChangeSubstrate_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(40, 110);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 16;
            this.label8.Text = "剩余测数：";
            // 
            // txtSubstrateCode
            // 
            this.txtSubstrateCode.Enabled = false;
            this.txtSubstrateCode.Location = new System.Drawing.Point(126, 37);
            this.txtSubstrateCode.Name = "txtSubstrateCode";
            this.txtSubstrateCode.Size = new System.Drawing.Size(163, 21);
            this.txtSubstrateCode.TabIndex = 17;
            this.txtSubstrateCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtSubstrateLastTest
            // 
            this.txtSubstrateLastTest.Enabled = false;
            this.txtSubstrateLastTest.IsNull = false;
            this.txtSubstrateLastTest.Location = new System.Drawing.Point(126, 107);
            this.txtSubstrateLastTest.MaxValue = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.txtSubstrateLastTest.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtSubstrateLastTest.Name = "txtSubstrateLastTest";
            this.txtSubstrateLastTest.Size = new System.Drawing.Size(163, 21);
            this.txtSubstrateLastTest.TabIndex = 19;
            this.txtSubstrateLastTest.Text = "0";
            // 
            // txtSubstrateAllTest
            // 
            this.txtSubstrateAllTest.Enabled = false;
            this.txtSubstrateAllTest.IsNull = false;
            this.txtSubstrateAllTest.Location = new System.Drawing.Point(126, 72);
            this.txtSubstrateAllTest.MaxValue = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.txtSubstrateAllTest.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtSubstrateAllTest.Name = "txtSubstrateAllTest";
            this.txtSubstrateAllTest.Size = new System.Drawing.Size(163, 21);
            this.txtSubstrateAllTest.TabIndex = 18;
            this.txtSubstrateAllTest.Text = "1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(28, 143);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 22;
            this.label1.Text = "使用有效期：";
            // 
            // ValidDate
            // 
            this.ValidDate.Location = new System.Drawing.Point(126, 137);
            this.ValidDate.Name = "ValidDate";
            this.ValidDate.Size = new System.Drawing.Size(163, 21);
            this.ValidDate.TabIndex = 23;
            this.ValidDate.Value = new System.DateTime(2018, 10, 17, 0, 0, 0, 0);
            // 
            // frmLoadSu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(339, 235);
            this.Controls.Add(this.ValidDate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSubstrateAllTest);
            this.Controls.Add(this.txtSubstrateLastTest);
            this.Controls.Add(this.btnLoadSubstrate);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnChangeSubstrate);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtSubstrateCode);
            this.Name = "frmLoadSu";
            this.Text = "底物装载";
            this.Load += new System.EventHandler(this.frmLoadSu_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CustomControl.FunctionButton btnLoadSubstrate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private CustomControl.FunctionButton btnChangeSubstrate;
        private System.Windows.Forms.Label label8;
        private CustomControl.userTextBoxBase txtSubstrateCode;
        private CustomControl.userNumTextBox txtSubstrateLastTest;
        private CustomControl.userNumTextBox txtSubstrateAllTest;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker ValidDate;
    }
}