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
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtSubstrateCode = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.txtSubstrateLastTest = new BioBaseCLIA.CustomControl.userNumTextBox();
            this.txtSubstrateAllTest = new BioBaseCLIA.CustomControl.userNumTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ValidDate = new System.Windows.Forms.DateTimePicker();
            this.btnLoadSubstrate = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnChangeSubstrate = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.SuspendLayout();
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // txtSubstrateCode
            // 
            resources.ApplyResources(this.txtSubstrateCode, "txtSubstrateCode");
            this.txtSubstrateCode.Name = "txtSubstrateCode";
            // 
            // txtSubstrateLastTest
            // 
            resources.ApplyResources(this.txtSubstrateLastTest, "txtSubstrateLastTest");
            this.txtSubstrateLastTest.IsNull = false;
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
            // 
            // txtSubstrateAllTest
            // 
            resources.ApplyResources(this.txtSubstrateAllTest, "txtSubstrateAllTest");
            this.txtSubstrateAllTest.IsNull = false;
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
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // ValidDate
            // 
            resources.ApplyResources(this.ValidDate, "ValidDate");
            this.ValidDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.ValidDate.Name = "ValidDate";
            this.ValidDate.Value = new System.DateTime(2018, 10, 17, 0, 0, 0, 0);
            // 
            // btnLoadSubstrate
            // 
            resources.ApplyResources(this.btnLoadSubstrate, "btnLoadSubstrate");
            this.btnLoadSubstrate.BackColor = System.Drawing.Color.Transparent;
            this.btnLoadSubstrate.EnabledSet = true;
            this.btnLoadSubstrate.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnLoadSubstrate.FlatAppearance.BorderSize = 0;
            this.btnLoadSubstrate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnLoadSubstrate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnLoadSubstrate.Name = "btnLoadSubstrate";
            this.btnLoadSubstrate.UseVisualStyleBackColor = false;
            this.btnLoadSubstrate.Click += new System.EventHandler(this.btnLoadSubstrate_Click);
            // 
            // btnChangeSubstrate
            // 
            resources.ApplyResources(this.btnChangeSubstrate, "btnChangeSubstrate");
            this.btnChangeSubstrate.BackColor = System.Drawing.Color.Transparent;
            this.btnChangeSubstrate.EnabledSet = true;
            this.btnChangeSubstrate.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnChangeSubstrate.FlatAppearance.BorderSize = 0;
            this.btnChangeSubstrate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnChangeSubstrate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnChangeSubstrate.Name = "btnChangeSubstrate";
            this.btnChangeSubstrate.UseVisualStyleBackColor = false;
            this.btnChangeSubstrate.Click += new System.EventHandler(this.btnChangeSubstrate_Click);
            // 
            // frmLoadSu
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
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