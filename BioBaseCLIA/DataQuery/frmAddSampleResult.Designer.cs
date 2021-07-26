namespace BioBaseCLIA.Run
{
    partial class frmAddSampleResult
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddSampleResult));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbItemName = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbSampleType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbResult = new System.Windows.Forms.ComboBox();
            this.txtDilutionRatio = new BioBaseCLIA.CustomControl.userNumTextBox();
            this.txtConcentration = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.cmbRgBatch = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.fbtnSubmit = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnBack = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.label12 = new System.Windows.Forms.Label();
            this.txtSampleNum = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.fbtnPatientInfo = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // cmbItemName
            // 
            resources.ApplyResources(this.cmbItemName, "cmbItemName");
            this.cmbItemName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItemName.FormattingEnabled = true;
            this.cmbItemName.Name = "cmbItemName";
            this.cmbItemName.SelectedIndexChanged += new System.EventHandler(this.cmbItemName_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.cmbSampleType);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.cmbResult);
            this.groupBox1.Controls.Add(this.txtDilutionRatio);
            this.groupBox1.Controls.Add(this.txtConcentration);
            this.groupBox1.Controls.Add(this.cmbRgBatch);
            this.groupBox1.Controls.Add(this.cmbItemName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // cmbSampleType
            // 
            resources.ApplyResources(this.cmbSampleType, "cmbSampleType");
            this.cmbSampleType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSampleType.FormattingEnabled = true;
            this.cmbSampleType.Items.AddRange(new object[] {
            resources.GetString("cmbSampleType.Items"),
            resources.GetString("cmbSampleType.Items1"),
            resources.GetString("cmbSampleType.Items2")});
            this.cmbSampleType.Name = "cmbSampleType";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // cmbResult
            // 
            resources.ApplyResources(this.cmbResult, "cmbResult");
            this.cmbResult.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbResult.FormattingEnabled = true;
            this.cmbResult.Items.AddRange(new object[] {
            resources.GetString("cmbResult.Items"),
            resources.GetString("cmbResult.Items1"),
            resources.GetString("cmbResult.Items2"),
            resources.GetString("cmbResult.Items3")});
            this.cmbResult.Name = "cmbResult";
            // 
            // txtDilutionRatio
            // 
            resources.ApplyResources(this.txtDilutionRatio, "txtDilutionRatio");
            this.txtDilutionRatio.IsNull = false;
            this.txtDilutionRatio.MaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtDilutionRatio.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtDilutionRatio.Name = "txtDilutionRatio";
            // 
            // txtConcentration
            // 
            resources.ApplyResources(this.txtConcentration, "txtConcentration");
            this.txtConcentration.Name = "txtConcentration";
            // 
            // cmbRgBatch
            // 
            resources.ApplyResources(this.cmbRgBatch, "cmbRgBatch");
            this.cmbRgBatch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRgBatch.FormattingEnabled = true;
            this.cmbRgBatch.Name = "cmbRgBatch";
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
            // fbtnSubmit
            // 
            resources.ApplyResources(this.fbtnSubmit, "fbtnSubmit");
            this.fbtnSubmit.BackColor = System.Drawing.Color.Transparent;
            this.fbtnSubmit.EnabledSet = true;
            this.fbtnSubmit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnSubmit.FlatAppearance.BorderSize = 0;
            this.fbtnSubmit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnSubmit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnSubmit.Name = "fbtnSubmit";
            this.fbtnSubmit.UseVisualStyleBackColor = false;
            this.fbtnSubmit.Click += new System.EventHandler(this.fbtnSubmit_Click);
            // 
            // fbtnBack
            // 
            resources.ApplyResources(this.fbtnBack, "fbtnBack");
            this.fbtnBack.BackColor = System.Drawing.Color.Transparent;
            this.fbtnBack.EnabledSet = true;
            this.fbtnBack.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnBack.FlatAppearance.BorderSize = 0;
            this.fbtnBack.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnBack.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnBack.Name = "fbtnBack";
            this.fbtnBack.UseVisualStyleBackColor = false;
            this.fbtnBack.Click += new System.EventHandler(this.fbtnBack_Click);
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // txtSampleNum
            // 
            resources.ApplyResources(this.txtSampleNum, "txtSampleNum");
            this.txtSampleNum.Name = "txtSampleNum";
            this.txtSampleNum.ReadOnly = true;
            // 
            // fbtnPatientInfo
            // 
            resources.ApplyResources(this.fbtnPatientInfo, "fbtnPatientInfo");
            this.fbtnPatientInfo.BackColor = System.Drawing.Color.Transparent;
            this.fbtnPatientInfo.EnabledSet = true;
            this.fbtnPatientInfo.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnPatientInfo.FlatAppearance.BorderSize = 0;
            this.fbtnPatientInfo.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnPatientInfo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnPatientInfo.Name = "fbtnPatientInfo";
            this.fbtnPatientInfo.UseVisualStyleBackColor = false;
            this.fbtnPatientInfo.Click += new System.EventHandler(this.fbtnPatientInfo_Click);
            // 
            // frmAddSampleResult
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.fbtnPatientInfo);
            this.Controls.Add(this.txtSampleNum);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.fbtnBack);
            this.Controls.Add(this.fbtnSubmit);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmAddSampleResult";
            this.Load += new System.EventHandler(this.frmAddSampleResult_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbItemName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbRgBatch;
        private CustomControl.userTextBoxBase txtConcentration;
        private CustomControl.userNumTextBox txtDilutionRatio;
        private System.Windows.Forms.ComboBox cmbResult;
        private System.Windows.Forms.Label label9;
        private CustomControl.FunctionButton fbtnSubmit;
        private CustomControl.FunctionButton fbtnBack;
        private System.Windows.Forms.ComboBox cmbSampleType;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label12;
        private CustomControl.userTextBoxBase txtSampleNum;
        private CustomControl.FunctionButton fbtnPatientInfo;
    }
}