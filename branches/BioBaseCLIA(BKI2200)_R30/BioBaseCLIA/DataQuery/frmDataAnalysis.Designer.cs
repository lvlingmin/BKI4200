namespace BioBaseCLIA.DataQuery
{
    partial class frmDataAnalysis
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDataAnalysis));
            this.chkInsChart = new System.Windows.Forms.CheckBox();
            this.chkCpChart = new System.Windows.Forms.CheckBox();
            this.chkPsaRatio = new System.Windows.Forms.CheckBox();
            this.chkFshLhRatio = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkPGIPGIIRatio = new System.Windows.Forms.CheckBox();
            this.dgvReleaseInfo2 = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dPanal1 = new BioBaseCLIA.CustomControl.definePanal(this.components);
            this.fbtnInfo1Delete = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnInfo1Up = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.dgvReleaseInfo1 = new System.Windows.Forms.DataGridView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dPanal2 = new BioBaseCLIA.CustomControl.definePanal(this.components);
            this.fbtnInfo2Delete = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnInfo2Up = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPatientName = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbPatientSex = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPatientOld = new BioBaseCLIA.CustomControl.userNumTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMedicaRecordNo = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.label5 = new System.Windows.Forms.Label();
            this.txtClinicNo = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.label11 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.fbtnSearch = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.cmbSelectRange = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.fbtnListRemove = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.listBoxSampleNum = new System.Windows.Forms.ListBox();
            this.fbtnConfirm = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnCancel = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.ratio1 = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.label7 = new System.Windows.Forms.Label();
            this.ratio2 = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.label8 = new System.Windows.Forms.Label();
            this.ratio3 = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Result = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReleaseInfo2)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReleaseInfo1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkInsChart
            // 
            resources.ApplyResources(this.chkInsChart, "chkInsChart");
            this.chkInsChart.Name = "chkInsChart";
            this.chkInsChart.UseVisualStyleBackColor = true;
            // 
            // chkCpChart
            // 
            resources.ApplyResources(this.chkCpChart, "chkCpChart");
            this.chkCpChart.Name = "chkCpChart";
            this.chkCpChart.UseVisualStyleBackColor = true;
            // 
            // chkPsaRatio
            // 
            resources.ApplyResources(this.chkPsaRatio, "chkPsaRatio");
            this.chkPsaRatio.Name = "chkPsaRatio";
            this.chkPsaRatio.UseVisualStyleBackColor = true;
            // 
            // chkFshLhRatio
            // 
            resources.ApplyResources(this.chkFshLhRatio, "chkFshLhRatio");
            this.chkFshLhRatio.Name = "chkFshLhRatio";
            this.chkFshLhRatio.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.chkPGIPGIIRatio);
            this.groupBox1.Controls.Add(this.chkInsChart);
            this.groupBox1.Controls.Add(this.chkCpChart);
            this.groupBox1.Controls.Add(this.chkPsaRatio);
            this.groupBox1.Controls.Add(this.chkFshLhRatio);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // chkPGIPGIIRatio
            // 
            resources.ApplyResources(this.chkPGIPGIIRatio, "chkPGIPGIIRatio");
            this.chkPGIPGIIRatio.Name = "chkPGIPGIIRatio";
            this.chkPGIPGIIRatio.UseVisualStyleBackColor = true;
            // 
            // dgvReleaseInfo2
            // 
            resources.ApplyResources(this.dgvReleaseInfo2, "dgvReleaseInfo2");
            this.dgvReleaseInfo2.AllowUserToAddRows = false;
            this.dgvReleaseInfo2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReleaseInfo2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.No,
            this.Value,
            this.Time});
            this.dgvReleaseInfo2.Name = "dgvReleaseInfo2";
            this.dgvReleaseInfo2.RowTemplate.Height = 23;
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.dPanal1);
            this.groupBox2.Controls.Add(this.fbtnInfo1Delete);
            this.groupBox2.Controls.Add(this.fbtnInfo1Up);
            this.groupBox2.Controls.Add(this.dgvReleaseInfo1);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // dPanal1
            // 
            resources.ApplyResources(this.dPanal1, "dPanal1");
            this.dPanal1.Name = "dPanal1";
            // 
            // fbtnInfo1Delete
            // 
            resources.ApplyResources(this.fbtnInfo1Delete, "fbtnInfo1Delete");
            this.fbtnInfo1Delete.BackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo1Delete.EnabledSet = true;
            this.fbtnInfo1Delete.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnInfo1Delete.FlatAppearance.BorderSize = 0;
            this.fbtnInfo1Delete.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo1Delete.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo1Delete.Name = "fbtnInfo1Delete";
            this.fbtnInfo1Delete.UseVisualStyleBackColor = false;
            this.fbtnInfo1Delete.Click += new System.EventHandler(this.fbtnInfo1Delete_Click);
            // 
            // fbtnInfo1Up
            // 
            resources.ApplyResources(this.fbtnInfo1Up, "fbtnInfo1Up");
            this.fbtnInfo1Up.BackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo1Up.EnabledSet = true;
            this.fbtnInfo1Up.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnInfo1Up.FlatAppearance.BorderSize = 0;
            this.fbtnInfo1Up.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo1Up.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo1Up.Name = "fbtnInfo1Up";
            this.fbtnInfo1Up.UseVisualStyleBackColor = false;
            this.fbtnInfo1Up.Click += new System.EventHandler(this.fbtnInfo1Up_Click);
            // 
            // dgvReleaseInfo1
            // 
            resources.ApplyResources(this.dgvReleaseInfo1, "dgvReleaseInfo1");
            this.dgvReleaseInfo1.AllowUserToAddRows = false;
            this.dgvReleaseInfo1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReleaseInfo1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.Result,
            this.Date});
            this.dgvReleaseInfo1.Name = "dgvReleaseInfo1";
            this.dgvReleaseInfo1.RowTemplate.Height = 23;
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.dPanal2);
            this.groupBox3.Controls.Add(this.fbtnInfo2Delete);
            this.groupBox3.Controls.Add(this.fbtnInfo2Up);
            this.groupBox3.Controls.Add(this.dgvReleaseInfo2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // dPanal2
            // 
            resources.ApplyResources(this.dPanal2, "dPanal2");
            this.dPanal2.Name = "dPanal2";
            // 
            // fbtnInfo2Delete
            // 
            resources.ApplyResources(this.fbtnInfo2Delete, "fbtnInfo2Delete");
            this.fbtnInfo2Delete.BackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo2Delete.EnabledSet = true;
            this.fbtnInfo2Delete.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnInfo2Delete.FlatAppearance.BorderSize = 0;
            this.fbtnInfo2Delete.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo2Delete.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo2Delete.Name = "fbtnInfo2Delete";
            this.fbtnInfo2Delete.UseVisualStyleBackColor = false;
            this.fbtnInfo2Delete.Click += new System.EventHandler(this.fbtnInfo2Delete_Click);
            // 
            // fbtnInfo2Up
            // 
            resources.ApplyResources(this.fbtnInfo2Up, "fbtnInfo2Up");
            this.fbtnInfo2Up.BackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo2Up.EnabledSet = true;
            this.fbtnInfo2Up.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnInfo2Up.FlatAppearance.BorderSize = 0;
            this.fbtnInfo2Up.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo2Up.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo2Up.Name = "fbtnInfo2Up";
            this.fbtnInfo2Up.UseVisualStyleBackColor = false;
            this.fbtnInfo2Up.Click += new System.EventHandler(this.fbtnInfo2Up_Click);
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Controls.Add(this.flowLayoutPanel1);
            this.groupBox4.Controls.Add(this.fbtnSearch);
            this.groupBox4.Controls.Add(this.cmbSelectRange);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.txtPatientName);
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.cmbPatientSex);
            this.flowLayoutPanel1.Controls.Add(this.label3);
            this.flowLayoutPanel1.Controls.Add(this.txtPatientOld);
            this.flowLayoutPanel1.Controls.Add(this.label4);
            this.flowLayoutPanel1.Controls.Add(this.txtMedicaRecordNo);
            this.flowLayoutPanel1.Controls.Add(this.label5);
            this.flowLayoutPanel1.Controls.Add(this.txtClinicNo);
            this.flowLayoutPanel1.Controls.Add(this.label11);
            this.flowLayoutPanel1.Controls.Add(this.dateTimePicker1);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txtPatientName
            // 
            resources.ApplyResources(this.txtPatientName, "txtPatientName");
            this.txtPatientName.Name = "txtPatientName";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // cmbPatientSex
            // 
            resources.ApplyResources(this.cmbPatientSex, "cmbPatientSex");
            this.cmbPatientSex.FormattingEnabled = true;
            this.cmbPatientSex.Items.AddRange(new object[] {
            resources.GetString("cmbPatientSex.Items"),
            resources.GetString("cmbPatientSex.Items1")});
            this.cmbPatientSex.Name = "cmbPatientSex";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // txtPatientOld
            // 
            resources.ApplyResources(this.txtPatientOld, "txtPatientOld");
            this.txtPatientOld.IsNull = false;
            this.txtPatientOld.MaxValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtPatientOld.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtPatientOld.Name = "txtPatientOld";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // txtMedicaRecordNo
            // 
            resources.ApplyResources(this.txtMedicaRecordNo, "txtMedicaRecordNo");
            this.txtMedicaRecordNo.Name = "txtMedicaRecordNo";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // txtClinicNo
            // 
            resources.ApplyResources(this.txtClinicNo, "txtClinicNo");
            this.txtClinicNo.Name = "txtClinicNo";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // dateTimePicker1
            // 
            resources.ApplyResources(this.dateTimePicker1, "dateTimePicker1");
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Name = "dateTimePicker1";
            // 
            // fbtnSearch
            // 
            resources.ApplyResources(this.fbtnSearch, "fbtnSearch");
            this.fbtnSearch.BackColor = System.Drawing.Color.Transparent;
            this.fbtnSearch.EnabledSet = true;
            this.fbtnSearch.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnSearch.FlatAppearance.BorderSize = 0;
            this.fbtnSearch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnSearch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnSearch.Name = "fbtnSearch";
            this.fbtnSearch.UseVisualStyleBackColor = false;
            this.fbtnSearch.Click += new System.EventHandler(this.fbtnSearch_Click);
            // 
            // cmbSelectRange
            // 
            resources.ApplyResources(this.cmbSelectRange, "cmbSelectRange");
            this.cmbSelectRange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelectRange.FormattingEnabled = true;
            this.cmbSelectRange.Items.AddRange(new object[] {
            resources.GetString("cmbSelectRange.Items"),
            resources.GetString("cmbSelectRange.Items1")});
            this.cmbSelectRange.Name = "cmbSelectRange";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // groupBox5
            // 
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Controls.Add(this.fbtnListRemove);
            this.groupBox5.Controls.Add(this.listBoxSampleNum);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // fbtnListRemove
            // 
            resources.ApplyResources(this.fbtnListRemove, "fbtnListRemove");
            this.fbtnListRemove.BackColor = System.Drawing.Color.Transparent;
            this.fbtnListRemove.EnabledSet = true;
            this.fbtnListRemove.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnListRemove.FlatAppearance.BorderSize = 0;
            this.fbtnListRemove.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnListRemove.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnListRemove.Name = "fbtnListRemove";
            this.fbtnListRemove.UseVisualStyleBackColor = false;
            this.fbtnListRemove.Click += new System.EventHandler(this.fbtnListRemove_Click);
            // 
            // listBoxSampleNum
            // 
            resources.ApplyResources(this.listBoxSampleNum, "listBoxSampleNum");
            this.listBoxSampleNum.BackColor = System.Drawing.Color.PowderBlue;
            this.listBoxSampleNum.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBoxSampleNum.FormattingEnabled = true;
            this.listBoxSampleNum.Name = "listBoxSampleNum";
            // 
            // fbtnConfirm
            // 
            resources.ApplyResources(this.fbtnConfirm, "fbtnConfirm");
            this.fbtnConfirm.BackColor = System.Drawing.Color.Transparent;
            this.fbtnConfirm.EnabledSet = true;
            this.fbtnConfirm.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnConfirm.FlatAppearance.BorderSize = 0;
            this.fbtnConfirm.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnConfirm.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnConfirm.Name = "fbtnConfirm";
            this.fbtnConfirm.UseVisualStyleBackColor = false;
            this.fbtnConfirm.Click += new System.EventHandler(this.fbtnConfirm_Click);
            // 
            // fbtnCancel
            // 
            resources.ApplyResources(this.fbtnCancel, "fbtnCancel");
            this.fbtnCancel.BackColor = System.Drawing.Color.Transparent;
            this.fbtnCancel.EnabledSet = true;
            this.fbtnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnCancel.FlatAppearance.BorderSize = 0;
            this.fbtnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnCancel.Name = "fbtnCancel";
            this.fbtnCancel.UseVisualStyleBackColor = false;
            this.fbtnCancel.Click += new System.EventHandler(this.fbtnCancel_Click);
            // 
            // flowLayoutPanel2
            // 
            resources.ApplyResources(this.flowLayoutPanel2, "flowLayoutPanel2");
            this.flowLayoutPanel2.Controls.Add(this.label6);
            this.flowLayoutPanel2.Controls.Add(this.ratio1);
            this.flowLayoutPanel2.Controls.Add(this.label7);
            this.flowLayoutPanel2.Controls.Add(this.ratio2);
            this.flowLayoutPanel2.Controls.Add(this.label8);
            this.flowLayoutPanel2.Controls.Add(this.ratio3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // ratio1
            // 
            resources.ApplyResources(this.ratio1, "ratio1");
            this.ratio1.Name = "ratio1";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // ratio2
            // 
            resources.ApplyResources(this.ratio2, "ratio2");
            this.ratio2.Name = "ratio2";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // ratio3
            // 
            resources.ApplyResources(this.ratio3, "ratio3");
            this.ratio3.Name = "ratio3";
            // 
            // Id
            // 
            this.Id.DataPropertyName = "Id";
            this.Id.FillWeight = 15F;
            resources.ApplyResources(this.Id, "Id");
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            // 
            // Result
            // 
            this.Result.DataPropertyName = "Result";
            this.Result.FillWeight = 25F;
            resources.ApplyResources(this.Result, "Result");
            this.Result.Name = "Result";
            // 
            // Date
            // 
            this.Date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Date.DataPropertyName = "Date";
            this.Date.FillWeight = 60F;
            resources.ApplyResources(this.Date, "Date");
            this.Date.Name = "Date";
            this.Date.ReadOnly = true;
            // 
            // No
            // 
            this.No.DataPropertyName = "No";
            this.No.FillWeight = 15F;
            resources.ApplyResources(this.No, "No");
            this.No.Name = "No";
            this.No.ReadOnly = true;
            // 
            // Value
            // 
            this.Value.DataPropertyName = "Value";
            this.Value.FillWeight = 25F;
            resources.ApplyResources(this.Value, "Value");
            this.Value.Name = "Value";
            // 
            // Time
            // 
            this.Time.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Time.DataPropertyName = "Time";
            this.Time.FillWeight = 60F;
            resources.ApplyResources(this.Time, "Time");
            this.Time.Name = "Time";
            this.Time.ReadOnly = true;
            // 
            // frmDataAnalysis
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.fbtnCancel);
            this.Controls.Add(this.fbtnConfirm);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmDataAnalysis";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmDataAnalysis_FormClosed);
            this.Load += new System.EventHandler(this.frmDataAnalysis_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReleaseInfo2)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReleaseInfo1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.CheckBox chkInsChart;
        private System.Windows.Forms.CheckBox chkCpChart;
        private System.Windows.Forms.CheckBox chkPsaRatio;
        private System.Windows.Forms.CheckBox chkFshLhRatio;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvReleaseInfo2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private CustomControl.FunctionButton fbtnInfo2Delete;
        private CustomControl.FunctionButton fbtnInfo2Up;
        private System.Windows.Forms.GroupBox groupBox4;
        private CustomControl.FunctionButton fbtnInfo1Delete;
        private CustomControl.FunctionButton fbtnInfo1Up;
        private System.Windows.Forms.DataGridView dgvReleaseInfo1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ListBox listBoxSampleNum;
        private CustomControl.FunctionButton fbtnConfirm;
        private CustomControl.FunctionButton fbtnCancel;
        private CustomControl.definePanal dPanal1;
        private CustomControl.definePanal dPanal2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private CustomControl.userTextBoxBase txtPatientName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbPatientSex;
        private System.Windows.Forms.Label label3;
        private CustomControl.userNumTextBox txtPatientOld;
        private System.Windows.Forms.Label label4;
        private CustomControl.userTextBoxBase txtMedicaRecordNo;
        private System.Windows.Forms.Label label5;
        private CustomControl.userTextBoxBase txtClinicNo;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private CustomControl.FunctionButton fbtnSearch;
        private System.Windows.Forms.ComboBox cmbSelectRange;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private CustomControl.FunctionButton fbtnListRemove;
        private System.Windows.Forms.CheckBox chkPGIPGIIRatio;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private CustomControl.userTextBoxBase ratio1;
        private CustomControl.userTextBoxBase ratio2;
        private CustomControl.userTextBoxBase ratio3;
        private System.Windows.Forms.DataGridViewTextBoxColumn No;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn Result;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
    }
}