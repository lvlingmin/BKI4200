namespace BioBaseCLIA.Run
{
    partial class frmSampleLoad
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSampleLoad));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fbtnTestResult = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnWorkList = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnReturn = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnLoadSample = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnLoadReagent = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.fbtnRunInfoMody = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnCreatWorkList = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnAddPatient = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnLoadSp = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvSpInfoList = new System.Windows.Forms.DataGridView();
            this.Position = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SampleNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RepeatCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SampleType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TubeType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label17 = new System.Windows.Forms.Label();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.srdReagent = new Disk.SampleReagentDisk();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSpInfoList)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.fbtnTestResult);
            this.panel1.Controls.Add(this.btnWorkList);
            this.panel1.Controls.Add(this.fbtnReturn);
            this.panel1.Controls.Add(this.btnLoadSample);
            this.panel1.Controls.Add(this.btnLoadReagent);
            this.panel1.Name = "panel1";
            // 
            // fbtnTestResult
            // 
            resources.ApplyResources(this.fbtnTestResult, "fbtnTestResult");
            this.fbtnTestResult.BackColor = System.Drawing.Color.Transparent;
            this.fbtnTestResult.EnabledSet = true;
            this.fbtnTestResult.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnTestResult.FlatAppearance.BorderSize = 0;
            this.fbtnTestResult.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnTestResult.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnTestResult.Name = "fbtnTestResult";
            this.fbtnTestResult.UseVisualStyleBackColor = false;
            this.fbtnTestResult.Click += new System.EventHandler(this.fbtnTestResult_Click);
            // 
            // btnWorkList
            // 
            resources.ApplyResources(this.btnWorkList, "btnWorkList");
            this.btnWorkList.BackColor = System.Drawing.Color.Transparent;
            this.btnWorkList.EnabledSet = true;
            this.btnWorkList.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnWorkList.FlatAppearance.BorderSize = 0;
            this.btnWorkList.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnWorkList.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnWorkList.Name = "btnWorkList";
            this.btnWorkList.UseVisualStyleBackColor = false;
            this.btnWorkList.Click += new System.EventHandler(this.btnWorkList_Click);
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
            // btnLoadSample
            // 
            resources.ApplyResources(this.btnLoadSample, "btnLoadSample");
            this.btnLoadSample.BackColor = System.Drawing.Color.Transparent;
            this.btnLoadSample.EnabledSet = true;
            this.btnLoadSample.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnLoadSample.FlatAppearance.BorderSize = 0;
            this.btnLoadSample.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnLoadSample.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnLoadSample.Name = "btnLoadSample";
            this.btnLoadSample.UseVisualStyleBackColor = false;
            // 
            // btnLoadReagent
            // 
            resources.ApplyResources(this.btnLoadReagent, "btnLoadReagent");
            this.btnLoadReagent.BackColor = System.Drawing.Color.Transparent;
            this.btnLoadReagent.EnabledSet = true;
            this.btnLoadReagent.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnLoadReagent.FlatAppearance.BorderSize = 0;
            this.btnLoadReagent.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnLoadReagent.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnLoadReagent.Name = "btnLoadReagent";
            this.btnLoadReagent.UseVisualStyleBackColor = false;
            this.btnLoadReagent.Click += new System.EventHandler(this.btnLoadReagent_Click);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.fbtnRunInfoMody);
            this.groupBox2.Controls.Add(this.btnCreatWorkList);
            this.groupBox2.Controls.Add(this.btnAddPatient);
            this.groupBox2.Controls.Add(this.btnLoadSp);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // fbtnRunInfoMody
            // 
            resources.ApplyResources(this.fbtnRunInfoMody, "fbtnRunInfoMody");
            this.fbtnRunInfoMody.BackColor = System.Drawing.Color.Transparent;
            this.fbtnRunInfoMody.EnabledSet = true;
            this.fbtnRunInfoMody.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnRunInfoMody.FlatAppearance.BorderSize = 0;
            this.fbtnRunInfoMody.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnRunInfoMody.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnRunInfoMody.Name = "fbtnRunInfoMody";
            this.fbtnRunInfoMody.UseVisualStyleBackColor = false;
            this.fbtnRunInfoMody.Click += new System.EventHandler(this.fbtnRunInfoMody_Click);
            // 
            // btnCreatWorkList
            // 
            resources.ApplyResources(this.btnCreatWorkList, "btnCreatWorkList");
            this.btnCreatWorkList.BackColor = System.Drawing.Color.Transparent;
            this.btnCreatWorkList.EnabledSet = true;
            this.btnCreatWorkList.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnCreatWorkList.FlatAppearance.BorderSize = 0;
            this.btnCreatWorkList.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnCreatWorkList.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnCreatWorkList.Name = "btnCreatWorkList";
            this.btnCreatWorkList.UseVisualStyleBackColor = false;
            this.btnCreatWorkList.Click += new System.EventHandler(this.btnCreatWorkList_Click);
            // 
            // btnAddPatient
            // 
            resources.ApplyResources(this.btnAddPatient, "btnAddPatient");
            this.btnAddPatient.BackColor = System.Drawing.Color.Transparent;
            this.btnAddPatient.EnabledSet = true;
            this.btnAddPatient.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnAddPatient.FlatAppearance.BorderSize = 0;
            this.btnAddPatient.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnAddPatient.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnAddPatient.Name = "btnAddPatient";
            this.btnAddPatient.UseVisualStyleBackColor = false;
            this.btnAddPatient.Click += new System.EventHandler(this.btnAddPatient_Click);
            // 
            // btnLoadSp
            // 
            resources.ApplyResources(this.btnLoadSp, "btnLoadSp");
            this.btnLoadSp.BackColor = System.Drawing.Color.Transparent;
            this.btnLoadSp.EnabledSet = true;
            this.btnLoadSp.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnLoadSp.FlatAppearance.BorderSize = 0;
            this.btnLoadSp.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnLoadSp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnLoadSp.Name = "btnLoadSp";
            this.btnLoadSp.UseVisualStyleBackColor = false;
            this.btnLoadSp.Click += new System.EventHandler(this.btnLoadSp_Click);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.dgvSpInfoList);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // dgvSpInfoList
            // 
            resources.ApplyResources(this.dgvSpInfoList, "dgvSpInfoList");
            this.dgvSpInfoList.AllowUserToAddRows = false;
            this.dgvSpInfoList.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSpInfoList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSpInfoList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvSpInfoList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Position,
            this.SampleNo,
            this.dataGridViewTextBoxColumn1,
            this.RepeatCount,
            this.SampleType,
            this.TubeType,
            this.dataGridViewTextBoxColumn2,
            this.Status});
            this.dgvSpInfoList.Name = "dgvSpInfoList";
            this.dgvSpInfoList.ReadOnly = true;
            this.dgvSpInfoList.RowHeadersVisible = false;
            this.dgvSpInfoList.RowTemplate.Height = 23;
            this.dgvSpInfoList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // Position
            // 
            this.Position.DataPropertyName = "Position";
            resources.ApplyResources(this.Position, "Position");
            this.Position.Name = "Position";
            this.Position.ReadOnly = true;
            // 
            // SampleNo
            // 
            this.SampleNo.DataPropertyName = "SampleNo";
            resources.ApplyResources(this.SampleNo, "SampleNo");
            this.SampleNo.Name = "SampleNo";
            this.SampleNo.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "ItemName";
            resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // RepeatCount
            // 
            this.RepeatCount.DataPropertyName = "RepeatCount";
            resources.ApplyResources(this.RepeatCount, "RepeatCount");
            this.RepeatCount.Name = "RepeatCount";
            this.RepeatCount.ReadOnly = true;
            // 
            // SampleType
            // 
            this.SampleType.DataPropertyName = "SampleType";
            resources.ApplyResources(this.SampleType, "SampleType");
            this.SampleType.Name = "SampleType";
            this.SampleType.ReadOnly = true;
            // 
            // TubeType
            // 
            this.TubeType.DataPropertyName = "TubeType";
            resources.ApplyResources(this.TubeType, "TubeType");
            this.TubeType.Name = "TubeType";
            this.TubeType.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Emergency";
            resources.ApplyResources(this.dataGridViewTextBoxColumn2, "dataGridViewTextBoxColumn2");
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // Status
            // 
            this.Status.DataPropertyName = "Status";
            resources.ApplyResources(this.Status, "Status");
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.label17);
            this.groupBox3.Controls.Add(this.textBox8);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.textBox4);
            this.groupBox3.Controls.Add(this.textBox3);
            this.groupBox3.Controls.Add(this.textBox2);
            this.groupBox3.Controls.Add(this.textBox1);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // textBox8
            // 
            resources.ApplyResources(this.textBox8, "textBox8");
            this.textBox8.BackColor = System.Drawing.Color.Purple;
            this.textBox8.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox8.Name = "textBox8";
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
            // textBox4
            // 
            resources.ApplyResources(this.textBox4, "textBox4");
            this.textBox4.BackColor = System.Drawing.Color.White;
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox4.Name = "textBox4";
            // 
            // textBox3
            // 
            resources.ApplyResources(this.textBox3, "textBox3");
            this.textBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox3.Name = "textBox3";
            // 
            // textBox2
            // 
            resources.ApplyResources(this.textBox2, "textBox2");
            this.textBox2.BackColor = System.Drawing.Color.Green;
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Name = "textBox2";
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.BackColor = System.Drawing.Color.Red;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Name = "textBox1";
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.textBox5);
            this.groupBox4.Controls.Add(this.textBox6);
            this.groupBox4.Controls.Add(this.textBox7);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
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
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // textBox5
            // 
            resources.ApplyResources(this.textBox5, "textBox5");
            this.textBox5.BackColor = System.Drawing.Color.White;
            this.textBox5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox5.Name = "textBox5";
            // 
            // textBox6
            // 
            resources.ApplyResources(this.textBox6, "textBox6");
            this.textBox6.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.textBox6.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox6.Name = "textBox6";
            // 
            // textBox7
            // 
            resources.ApplyResources(this.textBox7, "textBox7");
            this.textBox7.BackColor = System.Drawing.Color.Green;
            this.textBox7.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox7.Name = "textBox7";
            // 
            // srdReagent
            // 
            resources.ApplyResources(this.srdReagent, "srdReagent");
            this.srdReagent.BackColor = System.Drawing.Color.Transparent;
            this.srdReagent.BdColor = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White};
            this.srdReagent.BeadsCircleSize = 2F;
            this.srdReagent.BeadsGroupNum = 30;
            this.srdReagent.BeadsHoleR = 11;
            this.srdReagent.BitCircle = null;
            this.srdReagent.LastCircleSize = 3F;
            this.srdReagent.Name = "srdReagent";
            this.srdReagent.RgCircleSize = 1.2F;
            this.srdReagent.RgColor = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.Green,
        System.Drawing.Color.Green,
        System.Drawing.Color.Green,
        System.Drawing.Color.Green,
        System.Drawing.Color.Green,
        System.Drawing.Color.Green,
        System.Drawing.Color.Green,
        System.Drawing.Color.Green,
        System.Drawing.Color.Green,
        System.Drawing.Color.Green,
        System.Drawing.Color.Green,
        System.Drawing.Color.Green,
        System.Drawing.Color.Green,
        System.Drawing.Color.Green,
        System.Drawing.Color.Green,
        System.Drawing.Color.Green,
        System.Drawing.Color.Green,
        System.Drawing.Color.Green};
            this.srdReagent.RgFont = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.srdReagent.RgGroupNum = 30F;
            this.srdReagent.RgName = new string[] {
        "      ",
        "  ",
        "",
        "   ",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        "",
        ""};
            this.srdReagent.RgTestNum = new string[] {
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        "  ",
        " ",
        " ",
        " ",
        " ",
        "",
        " ",
        "  ",
        " ",
        " ",
        " ",
        " ",
        " ",
        " ",
        " "};
            this.srdReagent.SampleHoleR = 11;
            this.srdReagent.SPCircleSize = 1.05F;
            this.srdReagent.SpColor = new System.Drawing.Color[] {
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White,
        System.Drawing.Color.White};
            this.srdReagent.SPGroupNum = 60;
            this.srdReagent.MouseDown += new System.Windows.Forms.MouseEventHandler(this.srdReagent_MouseDown);
            this.srdReagent.MouseUp += new System.Windows.Forms.MouseEventHandler(this.srdReagent_MouseUp);
            // 
            // frmSampleLoad
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.srdReagent);
            this.Name = "frmSampleLoad";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSampleLoad_FormClosed);
            this.Load += new System.EventHandler(this.frmSampleLoad_Load);
            this.SizeChanged += new System.EventHandler(this.frmSampleLoad_SizeChanged);
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSpInfoList)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
        private CustomControl.FunctionButton fbtnTestResult;
        private CustomControl.FunctionButton btnWorkList;
        private CustomControl.FunctionButton fbtnReturn;
        private CustomControl.FunctionButton btnLoadSample;
        private CustomControl.FunctionButton btnLoadReagent;
        private CustomControl.FunctionButton btnAddPatient;
        private CustomControl.FunctionButton btnLoadSp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvSpInfoList;
        private CustomControl.FunctionButton btnCreatWorkList;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.TextBox textBox7;
        private Disk.SampleReagentDisk srdReagent;
        private CustomControl.FunctionButton fbtnRunInfoMody;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Position;
        private System.Windows.Forms.DataGridViewTextBoxColumn SampleNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn RepeatCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn SampleType;
        private System.Windows.Forms.DataGridViewTextBoxColumn TubeType;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
    }
}