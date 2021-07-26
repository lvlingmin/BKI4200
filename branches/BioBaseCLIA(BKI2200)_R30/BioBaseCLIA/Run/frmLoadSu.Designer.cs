namespace BioBaseCLIA.Run
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
            this.txtDiluteNumber = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.txtSubstrateLastTest = new BioBaseCLIA.CustomControl.userNumTextBox();
            this.txtSubstrateAllTest = new BioBaseCLIA.CustomControl.userNumTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ValidDate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.txtRegentPos = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.btnLoadSubstrate = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.cmbUnit1 = new System.Windows.Forms.ComboBox();
            this.cmbUnit2 = new System.Windows.Forms.ComboBox();
            this.functionButton1 = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.chkManualInput = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(76, 86);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 12);
            this.label7.TabIndex = 15;
            this.label7.Text = "规格:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(76, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 12);
            this.label6.TabIndex = 14;
            this.label6.Text = "条码:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(64, 118);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 12);
            this.label8.TabIndex = 16;
            this.label8.Text = "剩余量:";
            // 
            // txtDiluteNumber
            // 
            this.txtDiluteNumber.Enabled = false;
            this.txtDiluteNumber.Location = new System.Drawing.Point(117, 50);
            this.txtDiluteNumber.Name = "txtDiluteNumber";
            this.txtDiluteNumber.Size = new System.Drawing.Size(147, 21);
            this.txtDiluteNumber.TabIndex = 17;
            this.txtDiluteNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtDiluteNumber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDiluteNumber_KeyDown);
            // 
            // txtSubstrateLastTest
            // 
            this.txtSubstrateLastTest.Enabled = false;
            this.txtSubstrateLastTest.IsNull = false;
            this.txtSubstrateLastTest.Location = new System.Drawing.Point(117, 114);
            this.txtSubstrateLastTest.MaxValue = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.txtSubstrateLastTest.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtSubstrateLastTest.Name = "txtSubstrateLastTest";
            this.txtSubstrateLastTest.Size = new System.Drawing.Size(72, 21);
            this.txtSubstrateLastTest.TabIndex = 19;
            this.txtSubstrateLastTest.Text = "0";
            // 
            // txtSubstrateAllTest
            // 
            this.txtSubstrateAllTest.Enabled = false;
            this.txtSubstrateAllTest.IsNull = false;
            this.txtSubstrateAllTest.Location = new System.Drawing.Point(117, 82);
            this.txtSubstrateAllTest.MaxValue = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.txtSubstrateAllTest.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtSubstrateAllTest.Name = "txtSubstrateAllTest";
            this.txtSubstrateAllTest.Size = new System.Drawing.Size(72, 21);
            this.txtSubstrateAllTest.TabIndex = 18;
            this.txtSubstrateAllTest.Text = "25";
            this.txtSubstrateAllTest.TextChanged += new System.EventHandler(this.txtSubstrateAllTest_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(52, 152);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 22;
            this.label1.Text = "有效期至:";
            // 
            // ValidDate
            // 
            this.ValidDate.Location = new System.Drawing.Point(117, 146);
            this.ValidDate.Name = "ValidDate";
            this.ValidDate.Size = new System.Drawing.Size(147, 21);
            this.ValidDate.TabIndex = 23;
            this.ValidDate.Value = new System.DateTime(2018, 10, 17, 0, 0, 0, 0);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(52, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 24;
            this.label2.Text = "试剂位置:";
            // 
            // txtRegentPos
            // 
            this.txtRegentPos.Enabled = false;
            this.txtRegentPos.Location = new System.Drawing.Point(117, 18);
            this.txtRegentPos.Name = "txtRegentPos";
            this.txtRegentPos.Size = new System.Drawing.Size(147, 21);
            this.txtRegentPos.TabIndex = 25;
            this.txtRegentPos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnLoadSubstrate
            // 
            this.btnLoadSubstrate.BackColor = System.Drawing.Color.Transparent;
            this.btnLoadSubstrate.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnLoadSubstrate.BackgroundImage")));
            this.btnLoadSubstrate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnLoadSubstrate.EnabledSet = true;
            this.btnLoadSubstrate.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnLoadSubstrate.FlatAppearance.BorderSize = 0;
            this.btnLoadSubstrate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnLoadSubstrate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnLoadSubstrate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadSubstrate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnLoadSubstrate.Location = new System.Drawing.Point(117, 196);
            this.btnLoadSubstrate.Name = "btnLoadSubstrate";
            this.btnLoadSubstrate.Size = new System.Drawing.Size(57, 27);
            this.btnLoadSubstrate.TabIndex = 21;
            this.btnLoadSubstrate.Text = "保存";
            this.btnLoadSubstrate.UseVisualStyleBackColor = false;
            this.btnLoadSubstrate.Click += new System.EventHandler(this.btnLoadSubstrate_Click);
            // 
            // cmbUnit1
            // 
            this.cmbUnit1.FormattingEnabled = true;
            this.cmbUnit1.Items.AddRange(new object[] {
            "ml",
            "ul"});
            this.cmbUnit1.Location = new System.Drawing.Point(195, 82);
            this.cmbUnit1.Name = "cmbUnit1";
            this.cmbUnit1.Size = new System.Drawing.Size(42, 20);
            this.cmbUnit1.TabIndex = 26;
            // 
            // cmbUnit2
            // 
            this.cmbUnit2.FormattingEnabled = true;
            this.cmbUnit2.Items.AddRange(new object[] {
            "ml",
            "ul"});
            this.cmbUnit2.Location = new System.Drawing.Point(195, 114);
            this.cmbUnit2.Name = "cmbUnit2";
            this.cmbUnit2.Size = new System.Drawing.Size(42, 20);
            this.cmbUnit2.TabIndex = 27;
            // 
            // functionButton1
            // 
            this.functionButton1.BackColor = System.Drawing.Color.Transparent;
            this.functionButton1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("functionButton1.BackgroundImage")));
            this.functionButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.functionButton1.EnabledSet = true;
            this.functionButton1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.functionButton1.FlatAppearance.BorderSize = 0;
            this.functionButton1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.functionButton1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.functionButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.functionButton1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.functionButton1.Location = new System.Drawing.Point(207, 196);
            this.functionButton1.Name = "functionButton1";
            this.functionButton1.Size = new System.Drawing.Size(57, 27);
            this.functionButton1.TabIndex = 28;
            this.functionButton1.Text = "卸载";
            this.functionButton1.UseVisualStyleBackColor = false;
            this.functionButton1.Click += new System.EventHandler(this.functionButton1_Click);
            // 
            // chkManualInput
            // 
            this.chkManualInput.AutoSize = true;
            this.chkManualInput.Location = new System.Drawing.Point(192, 173);
            this.chkManualInput.Name = "chkManualInput";
            this.chkManualInput.Size = new System.Drawing.Size(72, 16);
            this.chkManualInput.TabIndex = 30;
            this.chkManualInput.Text = "手动输入";
            this.chkManualInput.UseVisualStyleBackColor = true;
            this.chkManualInput.CheckedChanged += new System.EventHandler(this.chkManualInput_CheckedChanged);
            // 
            // frmLoadSu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(339, 235);
            this.Controls.Add(this.chkManualInput);
            this.Controls.Add(this.functionButton1);
            this.Controls.Add(this.cmbUnit2);
            this.Controls.Add(this.cmbUnit1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtRegentPos);
            this.Controls.Add(this.ValidDate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSubstrateAllTest);
            this.Controls.Add(this.txtSubstrateLastTest);
            this.Controls.Add(this.btnLoadSubstrate);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtDiluteNumber);
            this.Name = "frmLoadSu";
            this.Text = "稀释液装载";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmLoadSu_FormClosed);
            this.Load += new System.EventHandler(this.frmLoadSu_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CustomControl.FunctionButton btnLoadSubstrate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private CustomControl.userTextBoxBase txtDiluteNumber;
        private CustomControl.userNumTextBox txtSubstrateLastTest;
        private CustomControl.userNumTextBox txtSubstrateAllTest;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker ValidDate;
        private System.Windows.Forms.Label label2;
        private CustomControl.userTextBoxBase txtRegentPos;
        private System.Windows.Forms.ComboBox cmbUnit1;
        private System.Windows.Forms.ComboBox cmbUnit2;
        private CustomControl.FunctionButton functionButton1;
        private System.Windows.Forms.CheckBox chkManualInput;
    }
}