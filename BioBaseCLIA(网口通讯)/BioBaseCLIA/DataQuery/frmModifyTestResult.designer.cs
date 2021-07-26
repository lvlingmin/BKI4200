namespace BioBaseCLIA.DataQuery
{
    partial class frmModifyTestResult
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmModifyTestResult));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtRange = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.label7 = new System.Windows.Forms.Label();
            this.txtUnit = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.label4 = new System.Windows.Forms.Label();
            this.txtConcentra = new BioBaseCLIA.CustomControl.userNumTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPMTCount = new BioBaseCLIA.CustomControl.userNumTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtTestDate = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.label3 = new System.Windows.Forms.Label();
            this.txtItemName = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSampleNo = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.label1 = new System.Windows.Forms.Label();
            this.fbtnCancle = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnOK = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtRange);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtUnit);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtConcentra);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtPMTCount);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtTestDate);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtItemName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtSampleNo);
            this.groupBox1.Controls.Add(this.label1);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // txtRange
            // 
            resources.ApplyResources(this.txtRange, "txtRange");
            this.txtRange.Name = "txtRange";
            this.txtRange.ReadOnly = true;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // txtUnit
            // 
            resources.ApplyResources(this.txtUnit, "txtUnit");
            this.txtUnit.Name = "txtUnit";
            this.txtUnit.ReadOnly = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // txtConcentra
            // 
            this.txtConcentra.IsDecimal = true;
            this.txtConcentra.IsNull = false;
            resources.ApplyResources(this.txtConcentra, "txtConcentra");
            this.txtConcentra.MaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtConcentra.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtConcentra.Name = "txtConcentra";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // txtPMTCount
            // 
            this.txtPMTCount.IsNull = false;
            resources.ApplyResources(this.txtPMTCount, "txtPMTCount");
            this.txtPMTCount.MaxValue = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.txtPMTCount.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtPMTCount.Name = "txtPMTCount";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // txtTestDate
            // 
            resources.ApplyResources(this.txtTestDate, "txtTestDate");
            this.txtTestDate.Name = "txtTestDate";
            this.txtTestDate.ReadOnly = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // txtItemName
            // 
            resources.ApplyResources(this.txtItemName, "txtItemName");
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.ReadOnly = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // txtSampleNo
            // 
            resources.ApplyResources(this.txtSampleNo, "txtSampleNo");
            this.txtSampleNo.Name = "txtSampleNo";
            this.txtSampleNo.ReadOnly = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // fbtnCancle
            // 
            this.fbtnCancle.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnCancle, "fbtnCancle");
            this.fbtnCancle.EnabledSet = true;
            this.fbtnCancle.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnCancle.FlatAppearance.BorderSize = 0;
            this.fbtnCancle.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnCancle.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnCancle.Name = "fbtnCancle";
            this.fbtnCancle.UseVisualStyleBackColor = false;
            this.fbtnCancle.Click += new System.EventHandler(this.fbtnCancle_Click);
            // 
            // fbtnOK
            // 
            this.fbtnOK.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnOK, "fbtnOK");
            this.fbtnOK.EnabledSet = true;
            this.fbtnOK.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnOK.FlatAppearance.BorderSize = 0;
            this.fbtnOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnOK.Name = "fbtnOK";
            this.fbtnOK.UseVisualStyleBackColor = false;
            this.fbtnOK.Click += new System.EventHandler(this.fbtnOK_Click);
            // 
            // frmModifyTestResult
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.fbtnCancle);
            this.Controls.Add(this.fbtnOK);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmModifyTestResult";
            this.Load += new System.EventHandler(this.frmModifyTestResult_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private CustomControl.userTextBoxBase txtSampleNo;
        private System.Windows.Forms.Label label1;
        private CustomControl.userTextBoxBase txtTestDate;
        private System.Windows.Forms.Label label3;
        private CustomControl.userTextBoxBase txtItemName;
        private System.Windows.Forms.Label label2;
        private CustomControl.userTextBoxBase txtRange;
        private System.Windows.Forms.Label label7;
        private CustomControl.userTextBoxBase txtUnit;
        private System.Windows.Forms.Label label4;
        private CustomControl.userNumTextBox txtConcentra;
        private System.Windows.Forms.Label label5;
        private CustomControl.userNumTextBox txtPMTCount;
        private System.Windows.Forms.Label label6;
        private CustomControl.FunctionButton fbtnCancle;
        private CustomControl.FunctionButton fbtnOK;
    }
}