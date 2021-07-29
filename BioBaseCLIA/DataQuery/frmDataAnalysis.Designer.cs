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
            this.No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dPanal1 = new BioBaseCLIA.CustomControl.definePanal(this.components);
            this.fbtnInfo1Delete = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnInfo1Up = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.dgvReleaseInfo1 = new System.Windows.Forms.DataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Result = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.ratio1 = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.ratio2 = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.ratio3 = new BioBaseCLIA.CustomControl.userTextBoxBase();
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
            this.chkInsChart.AutoSize = true;
            this.chkInsChart.Location = new System.Drawing.Point(108, 14);
            this.chkInsChart.Name = "chkInsChart";
            this.chkInsChart.Size = new System.Drawing.Size(78, 16);
            this.chkInsChart.TabIndex = 2;
            this.chkInsChart.Text = "INS释放图";
            this.chkInsChart.UseVisualStyleBackColor = true;
            // 
            // chkCpChart
            // 
            this.chkCpChart.AutoSize = true;
            this.chkCpChart.Location = new System.Drawing.Point(8, 14);
            this.chkCpChart.Name = "chkCpChart";
            this.chkCpChart.Size = new System.Drawing.Size(78, 16);
            this.chkCpChart.TabIndex = 3;
            this.chkCpChart.Text = "C-P释放图";
            this.chkCpChart.UseVisualStyleBackColor = true;
            // 
            // chkPsaRatio
            // 
            this.chkPsaRatio.AutoSize = true;
            this.chkPsaRatio.Location = new System.Drawing.Point(208, 14);
            this.chkPsaRatio.Name = "chkPsaRatio";
            this.chkPsaRatio.Size = new System.Drawing.Size(90, 16);
            this.chkPsaRatio.TabIndex = 4;
            this.chkPsaRatio.Text = "f-PSA/t-PSA";
            this.chkPsaRatio.UseVisualStyleBackColor = true;
            // 
            // chkFshLhRatio
            // 
            this.chkFshLhRatio.AutoSize = true;
            this.chkFshLhRatio.Location = new System.Drawing.Point(308, 14);
            this.chkFshLhRatio.Name = "chkFshLhRatio";
            this.chkFshLhRatio.Size = new System.Drawing.Size(60, 16);
            this.chkFshLhRatio.TabIndex = 5;
            this.chkFshLhRatio.Text = "FSH/LH";
            this.chkFshLhRatio.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkPGIPGIIRatio);
            this.groupBox1.Controls.Add(this.chkInsChart);
            this.groupBox1.Controls.Add(this.chkCpChart);
            this.groupBox1.Controls.Add(this.chkPsaRatio);
            this.groupBox1.Controls.Add(this.chkFshLhRatio);
            this.groupBox1.Location = new System.Drawing.Point(126, 85);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(626, 36);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "打印信息";
            // 
            // chkPGIPGIIRatio
            // 
            this.chkPGIPGIIRatio.AutoSize = true;
            this.chkPGIPGIIRatio.Location = new System.Drawing.Point(374, 14);
            this.chkPGIPGIIRatio.Name = "chkPGIPGIIRatio";
            this.chkPGIPGIIRatio.Size = new System.Drawing.Size(72, 16);
            this.chkPGIPGIIRatio.TabIndex = 6;
            this.chkPGIPGIIRatio.Text = "PGI/PGII";
            this.chkPGIPGIIRatio.UseVisualStyleBackColor = true;
            // 
            // dgvReleaseInfo2
            // 
            this.dgvReleaseInfo2.AllowUserToAddRows = false;
            this.dgvReleaseInfo2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReleaseInfo2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.No,
            this.Value,
            this.Time});
            this.dgvReleaseInfo2.Location = new System.Drawing.Point(6, 20);
            this.dgvReleaseInfo2.Name = "dgvReleaseInfo2";
            this.dgvReleaseInfo2.RowTemplate.Height = 23;
            this.dgvReleaseInfo2.Size = new System.Drawing.Size(296, 146);
            this.dgvReleaseInfo2.TabIndex = 3;
            // 
            // No
            // 
            this.No.DataPropertyName = "No";
            this.No.FillWeight = 15F;
            this.No.HeaderText = "No.";
            this.No.Name = "No";
            this.No.ReadOnly = true;
            this.No.Width = 38;
            // 
            // Value
            // 
            this.Value.DataPropertyName = "Value";
            this.Value.FillWeight = 25F;
            this.Value.HeaderText = "结果";
            this.Value.Name = "Value";
            this.Value.Width = 63;
            // 
            // Time
            // 
            this.Time.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Time.DataPropertyName = "Time";
            this.Time.FillWeight = 60F;
            this.Time.HeaderText = "时间";
            this.Time.Name = "Time";
            this.Time.ReadOnly = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dPanal1);
            this.groupBox2.Controls.Add(this.fbtnInfo1Delete);
            this.groupBox2.Controls.Add(this.fbtnInfo1Up);
            this.groupBox2.Controls.Add(this.dgvReleaseInfo1);
            this.groupBox2.Location = new System.Drawing.Point(126, 127);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(308, 331);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "C-P释放信息";
            // 
            // dPanal1
            // 
            this.dPanal1.Location = new System.Drawing.Point(8, 198);
            this.dPanal1.Name = "dPanal1";
            this.dPanal1.Size = new System.Drawing.Size(294, 131);
            this.dPanal1.TabIndex = 8;
            // 
            // fbtnInfo1Delete
            // 
            this.fbtnInfo1Delete.BackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo1Delete.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("fbtnInfo1Delete.BackgroundImage")));
            this.fbtnInfo1Delete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.fbtnInfo1Delete.EnabledSet = true;
            this.fbtnInfo1Delete.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnInfo1Delete.FlatAppearance.BorderSize = 0;
            this.fbtnInfo1Delete.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo1Delete.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo1Delete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fbtnInfo1Delete.Location = new System.Drawing.Point(210, 172);
            this.fbtnInfo1Delete.Name = "fbtnInfo1Delete";
            this.fbtnInfo1Delete.Size = new System.Drawing.Size(75, 23);
            this.fbtnInfo1Delete.TabIndex = 7;
            this.fbtnInfo1Delete.Text = "移除";
            this.fbtnInfo1Delete.UseVisualStyleBackColor = false;
            this.fbtnInfo1Delete.Click += new System.EventHandler(this.fbtnInfo1Delete_Click);
            // 
            // fbtnInfo1Up
            // 
            this.fbtnInfo1Up.BackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo1Up.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("fbtnInfo1Up.BackgroundImage")));
            this.fbtnInfo1Up.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.fbtnInfo1Up.EnabledSet = true;
            this.fbtnInfo1Up.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnInfo1Up.FlatAppearance.BorderSize = 0;
            this.fbtnInfo1Up.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo1Up.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo1Up.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fbtnInfo1Up.Location = new System.Drawing.Point(26, 172);
            this.fbtnInfo1Up.Name = "fbtnInfo1Up";
            this.fbtnInfo1Up.Size = new System.Drawing.Size(75, 23);
            this.fbtnInfo1Up.TabIndex = 6;
            this.fbtnInfo1Up.Text = "上移";
            this.fbtnInfo1Up.UseVisualStyleBackColor = false;
            this.fbtnInfo1Up.Click += new System.EventHandler(this.fbtnInfo1Up_Click);
            // 
            // dgvReleaseInfo1
            // 
            this.dgvReleaseInfo1.AllowUserToAddRows = false;
            this.dgvReleaseInfo1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReleaseInfo1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.Result,
            this.Date});
            this.dgvReleaseInfo1.Location = new System.Drawing.Point(6, 20);
            this.dgvReleaseInfo1.Name = "dgvReleaseInfo1";
            this.dgvReleaseInfo1.RowTemplate.Height = 23;
            this.dgvReleaseInfo1.Size = new System.Drawing.Size(296, 146);
            this.dgvReleaseInfo1.TabIndex = 4;
            // 
            // Id
            // 
            this.Id.DataPropertyName = "Id";
            this.Id.FillWeight = 15F;
            this.Id.HeaderText = "No.";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            this.Id.Width = 38;
            // 
            // Result
            // 
            this.Result.DataPropertyName = "Result";
            this.Result.FillWeight = 25F;
            this.Result.HeaderText = "结果";
            this.Result.Name = "Result";
            this.Result.Width = 63;
            // 
            // Date
            // 
            this.Date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Date.DataPropertyName = "Date";
            this.Date.FillWeight = 60F;
            this.Date.HeaderText = "时间";
            this.Date.Name = "Date";
            this.Date.ReadOnly = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.dPanal2);
            this.groupBox3.Controls.Add(this.fbtnInfo2Delete);
            this.groupBox3.Controls.Add(this.fbtnInfo2Up);
            this.groupBox3.Controls.Add(this.dgvReleaseInfo2);
            this.groupBox3.Location = new System.Drawing.Point(444, 127);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(308, 331);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "INS释放信息";
            // 
            // dPanal2
            // 
            this.dPanal2.Location = new System.Drawing.Point(8, 198);
            this.dPanal2.Name = "dPanal2";
            this.dPanal2.Size = new System.Drawing.Size(294, 131);
            this.dPanal2.TabIndex = 9;
            // 
            // fbtnInfo2Delete
            // 
            this.fbtnInfo2Delete.BackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo2Delete.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("fbtnInfo2Delete.BackgroundImage")));
            this.fbtnInfo2Delete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.fbtnInfo2Delete.EnabledSet = true;
            this.fbtnInfo2Delete.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnInfo2Delete.FlatAppearance.BorderSize = 0;
            this.fbtnInfo2Delete.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo2Delete.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo2Delete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fbtnInfo2Delete.Location = new System.Drawing.Point(210, 172);
            this.fbtnInfo2Delete.Name = "fbtnInfo2Delete";
            this.fbtnInfo2Delete.Size = new System.Drawing.Size(75, 23);
            this.fbtnInfo2Delete.TabIndex = 5;
            this.fbtnInfo2Delete.Text = "移除";
            this.fbtnInfo2Delete.UseVisualStyleBackColor = false;
            this.fbtnInfo2Delete.Click += new System.EventHandler(this.fbtnInfo2Delete_Click);
            // 
            // fbtnInfo2Up
            // 
            this.fbtnInfo2Up.BackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo2Up.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("fbtnInfo2Up.BackgroundImage")));
            this.fbtnInfo2Up.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.fbtnInfo2Up.EnabledSet = true;
            this.fbtnInfo2Up.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnInfo2Up.FlatAppearance.BorderSize = 0;
            this.fbtnInfo2Up.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo2Up.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnInfo2Up.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fbtnInfo2Up.Location = new System.Drawing.Point(26, 172);
            this.fbtnInfo2Up.Name = "fbtnInfo2Up";
            this.fbtnInfo2Up.Size = new System.Drawing.Size(75, 23);
            this.fbtnInfo2Up.TabIndex = 4;
            this.fbtnInfo2Up.Text = "上移";
            this.fbtnInfo2Up.UseVisualStyleBackColor = false;
            this.fbtnInfo2Up.Click += new System.EventHandler(this.fbtnInfo2Up_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.flowLayoutPanel1);
            this.groupBox4.Controls.Add(this.fbtnSearch);
            this.groupBox4.Controls.Add(this.cmbSelectRange);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Location = new System.Drawing.Point(126, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(626, 75);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "病人信息";
            // 
            // flowLayoutPanel1
            // 
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
            this.flowLayoutPanel1.Location = new System.Drawing.Point(6, 40);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(614, 27);
            this.flowLayoutPanel1.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 8, 0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "姓名：";
            // 
            // txtPatientName
            // 
            this.txtPatientName.Location = new System.Drawing.Point(44, 3);
            this.txtPatientName.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.txtPatientName.Name = "txtPatientName";
            this.txtPatientName.Size = new System.Drawing.Size(79, 21);
            this.txtPatientName.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(126, 8);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 8, 0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "性别：";
            // 
            // cmbPatientSex
            // 
            this.cmbPatientSex.FormattingEnabled = true;
            this.cmbPatientSex.Items.AddRange(new object[] {
            "男",
            "女"});
            this.cmbPatientSex.Location = new System.Drawing.Point(167, 3);
            this.cmbPatientSex.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.cmbPatientSex.Name = "cmbPatientSex";
            this.cmbPatientSex.Size = new System.Drawing.Size(37, 20);
            this.cmbPatientSex.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(207, 8);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 8, 0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "年龄：";
            // 
            // txtPatientOld
            // 
            this.txtPatientOld.IsNull = false;
            this.txtPatientOld.Location = new System.Drawing.Point(248, 3);
            this.txtPatientOld.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
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
            this.txtPatientOld.Size = new System.Drawing.Size(40, 21);
            this.txtPatientOld.TabIndex = 11;
            this.txtPatientOld.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(291, 8);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 8, 0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "病历号：";
            // 
            // txtMedicaRecordNo
            // 
            this.txtMedicaRecordNo.Location = new System.Drawing.Point(344, 3);
            this.txtMedicaRecordNo.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.txtMedicaRecordNo.Name = "txtMedicaRecordNo";
            this.txtMedicaRecordNo.Size = new System.Drawing.Size(106, 21);
            this.txtMedicaRecordNo.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(453, 8);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 8, 0, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "门诊号：";
            // 
            // txtClinicNo
            // 
            this.txtClinicNo.Location = new System.Drawing.Point(506, 3);
            this.txtClinicNo.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.txtClinicNo.Name = "txtClinicNo";
            this.txtClinicNo.Size = new System.Drawing.Size(106, 21);
            this.txtClinicNo.TabIndex = 13;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(3, 35);
            this.label11.Margin = new System.Windows.Forms.Padding(3, 8, 3, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(41, 12);
            this.label11.TabIndex = 14;
            this.label11.Text = "日期：";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(50, 30);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 21);
            this.dateTimePicker1.TabIndex = 15;
            // 
            // fbtnSearch
            // 
            this.fbtnSearch.BackColor = System.Drawing.Color.Transparent;
            this.fbtnSearch.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("fbtnSearch.BackgroundImage")));
            this.fbtnSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.fbtnSearch.EnabledSet = true;
            this.fbtnSearch.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnSearch.FlatAppearance.BorderSize = 0;
            this.fbtnSearch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnSearch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fbtnSearch.Location = new System.Drawing.Point(208, 11);
            this.fbtnSearch.Name = "fbtnSearch";
            this.fbtnSearch.Size = new System.Drawing.Size(75, 23);
            this.fbtnSearch.TabIndex = 16;
            this.fbtnSearch.Text = "查询";
            this.fbtnSearch.UseVisualStyleBackColor = false;
            this.fbtnSearch.Click += new System.EventHandler(this.fbtnSearch_Click);
            // 
            // cmbSelectRange
            // 
            this.cmbSelectRange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelectRange.FormattingEnabled = true;
            this.cmbSelectRange.Items.AddRange(new object[] {
            "样本编号",
            "病人信息"});
            this.cmbSelectRange.Location = new System.Drawing.Point(50, 14);
            this.cmbSelectRange.Name = "cmbSelectRange";
            this.cmbSelectRange.Size = new System.Drawing.Size(121, 20);
            this.cmbSelectRange.TabIndex = 15;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 17);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 12);
            this.label10.TabIndex = 14;
            this.label10.Text = "范围：";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.fbtnListRemove);
            this.groupBox5.Controls.Add(this.listBoxSampleNum);
            this.groupBox5.Location = new System.Drawing.Point(9, 4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(111, 485);
            this.groupBox5.TabIndex = 10;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "样本编号";
            // 
            // fbtnListRemove
            // 
            this.fbtnListRemove.BackColor = System.Drawing.Color.Transparent;
            this.fbtnListRemove.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("fbtnListRemove.BackgroundImage")));
            this.fbtnListRemove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.fbtnListRemove.EnabledSet = true;
            this.fbtnListRemove.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnListRemove.FlatAppearance.BorderSize = 0;
            this.fbtnListRemove.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnListRemove.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnListRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fbtnListRemove.Location = new System.Drawing.Point(19, 454);
            this.fbtnListRemove.Name = "fbtnListRemove";
            this.fbtnListRemove.Size = new System.Drawing.Size(75, 23);
            this.fbtnListRemove.TabIndex = 17;
            this.fbtnListRemove.Text = "移除";
            this.fbtnListRemove.UseVisualStyleBackColor = false;
            this.fbtnListRemove.Click += new System.EventHandler(this.fbtnListRemove_Click);
            // 
            // listBoxSampleNum
            // 
            this.listBoxSampleNum.BackColor = System.Drawing.Color.PowderBlue;
            this.listBoxSampleNum.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBoxSampleNum.FormattingEnabled = true;
            this.listBoxSampleNum.ItemHeight = 12;
            this.listBoxSampleNum.Location = new System.Drawing.Point(6, 17);
            this.listBoxSampleNum.Name = "listBoxSampleNum";
            this.listBoxSampleNum.Size = new System.Drawing.Size(99, 420);
            this.listBoxSampleNum.TabIndex = 11;
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
            this.fbtnConfirm.Location = new System.Drawing.Point(587, 464);
            this.fbtnConfirm.Name = "fbtnConfirm";
            this.fbtnConfirm.Size = new System.Drawing.Size(75, 23);
            this.fbtnConfirm.TabIndex = 11;
            this.fbtnConfirm.Text = "确定";
            this.fbtnConfirm.UseVisualStyleBackColor = false;
            this.fbtnConfirm.Click += new System.EventHandler(this.fbtnConfirm_Click);
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
            this.fbtnCancel.Location = new System.Drawing.Point(671, 464);
            this.fbtnCancel.Name = "fbtnCancel";
            this.fbtnCancel.Size = new System.Drawing.Size(75, 23);
            this.fbtnCancel.TabIndex = 12;
            this.fbtnCancel.Text = "取消";
            this.fbtnCancel.UseVisualStyleBackColor = false;
            this.fbtnCancel.Click += new System.EventHandler(this.fbtnCancel_Click);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.label6);
            this.flowLayoutPanel2.Controls.Add(this.ratio1);
            this.flowLayoutPanel2.Controls.Add(this.label7);
            this.flowLayoutPanel2.Controls.Add(this.ratio2);
            this.flowLayoutPanel2.Controls.Add(this.label8);
            this.flowLayoutPanel2.Controls.Add(this.ratio3);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(126, 464);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(455, 23);
            this.flowLayoutPanel2.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 5);
            this.label6.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 12);
            this.label6.TabIndex = 18;
            this.label6.Text = "f-PSA/t-PSA:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(147, 5);
            this.label7.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 12);
            this.label7.TabIndex = 19;
            this.label7.Text = "FSH/LH:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(261, 5);
            this.label8.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 12);
            this.label8.TabIndex = 20;
            this.label8.Text = "PGI/PGII:";
            // 
            // ratio1
            // 
            this.ratio1.Location = new System.Drawing.Point(86, 3);
            this.ratio1.Name = "ratio1";
            this.ratio1.Size = new System.Drawing.Size(55, 21);
            this.ratio1.TabIndex = 21;
            // 
            // ratio2
            // 
            this.ratio2.Location = new System.Drawing.Point(200, 3);
            this.ratio2.Name = "ratio2";
            this.ratio2.Size = new System.Drawing.Size(55, 21);
            this.ratio2.TabIndex = 22;
            // 
            // ratio3
            // 
            this.ratio3.Location = new System.Drawing.Point(326, 3);
            this.ratio3.Name = "ratio3";
            this.ratio3.Size = new System.Drawing.Size(55, 21);
            this.ratio3.TabIndex = 23;
            // 
            // frmDataAnalysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(764, 493);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.fbtnCancel);
            this.Controls.Add(this.fbtnConfirm);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmDataAnalysis";
            this.Text = "打印范围";
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
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn Result;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn No;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;
        private CustomControl.userTextBoxBase ratio1;
        private CustomControl.userTextBoxBase ratio2;
        private CustomControl.userTextBoxBase ratio3;
    }
}