namespace BioBaseCLIA.DataQuery
{
    partial class frmResultQuery
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmResultQuery));
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.fbtnTestAgain = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnExPort = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnImPort = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnRecalculate = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.tbnSendResult = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnPrint = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnMakeupInfo = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.dgvSampleData = new System.Windows.Forms.DataGridView();
            this.AssayResultID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SampleID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colChioce = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.SampleNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TestDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PMTCounter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Concentration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Unit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Result = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Range = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SampleType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReagentBeach = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvPatientInfo = new System.Windows.Forms.DataGridView();
            this.SampleID1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SendDateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SampleNo1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PatientName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Age = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClinicNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InpatientArea = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Ward = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BedNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MedicaRecordNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Diagnosis = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InspectionItems = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AcquisitionTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbSelect = new System.Windows.Forms.ComboBox();
            this.txtSampleNo = new System.Windows.Forms.TextBox();
            this.fbtnQuery = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fbtnReturn = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnSaveData = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnChoiceCurve = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnDeleteResult = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnModifyResult = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSampleData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPatientInfo)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.fbtnTestAgain);
            this.groupBox2.Controls.Add(this.btnExPort);
            this.groupBox2.Controls.Add(this.btnImPort);
            this.groupBox2.Controls.Add(this.fbtnRecalculate);
            this.groupBox2.Controls.Add(this.tbnSendResult);
            this.groupBox2.Controls.Add(this.fbtnPrint);
            this.groupBox2.Controls.Add(this.fbtnMakeupInfo);
            this.groupBox2.Controls.Add(this.dgvSampleData);
            this.groupBox2.Controls.Add(this.dgvPatientInfo);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // fbtnTestAgain
            // 
            resources.ApplyResources(this.fbtnTestAgain, "fbtnTestAgain");
            this.fbtnTestAgain.BackColor = System.Drawing.Color.Transparent;
            this.fbtnTestAgain.EnabledSet = true;
            this.fbtnTestAgain.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnTestAgain.FlatAppearance.BorderSize = 0;
            this.fbtnTestAgain.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnTestAgain.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnTestAgain.Name = "fbtnTestAgain";
            this.fbtnTestAgain.UseVisualStyleBackColor = false;
            this.fbtnTestAgain.Click += new System.EventHandler(this.fbtnTestAgain_Click);
            // 
            // btnExPort
            // 
            resources.ApplyResources(this.btnExPort, "btnExPort");
            this.btnExPort.BackColor = System.Drawing.Color.Transparent;
            this.btnExPort.EnabledSet = true;
            this.btnExPort.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnExPort.FlatAppearance.BorderSize = 0;
            this.btnExPort.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnExPort.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnExPort.Name = "btnExPort";
            this.btnExPort.UseVisualStyleBackColor = false;
            this.btnExPort.Click += new System.EventHandler(this.btnExPort_Click);
            // 
            // btnImPort
            // 
            resources.ApplyResources(this.btnImPort, "btnImPort");
            this.btnImPort.BackColor = System.Drawing.Color.Transparent;
            this.btnImPort.EnabledSet = true;
            this.btnImPort.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnImPort.FlatAppearance.BorderSize = 0;
            this.btnImPort.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnImPort.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnImPort.Name = "btnImPort";
            this.btnImPort.UseVisualStyleBackColor = false;
            this.btnImPort.Click += new System.EventHandler(this.btnImPort_Click);
            // 
            // fbtnRecalculate
            // 
            resources.ApplyResources(this.fbtnRecalculate, "fbtnRecalculate");
            this.fbtnRecalculate.BackColor = System.Drawing.Color.Transparent;
            this.fbtnRecalculate.EnabledSet = true;
            this.fbtnRecalculate.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnRecalculate.FlatAppearance.BorderSize = 0;
            this.fbtnRecalculate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnRecalculate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnRecalculate.Name = "fbtnRecalculate";
            this.fbtnRecalculate.UseVisualStyleBackColor = false;
            this.fbtnRecalculate.Click += new System.EventHandler(this.fbtnRecalculate_Click);
            // 
            // tbnSendResult
            // 
            resources.ApplyResources(this.tbnSendResult, "tbnSendResult");
            this.tbnSendResult.BackColor = System.Drawing.Color.Transparent;
            this.tbnSendResult.EnabledSet = true;
            this.tbnSendResult.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.tbnSendResult.FlatAppearance.BorderSize = 0;
            this.tbnSendResult.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.tbnSendResult.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.tbnSendResult.Name = "tbnSendResult";
            this.tbnSendResult.UseVisualStyleBackColor = false;
            this.tbnSendResult.Click += new System.EventHandler(this.tbnSendResult_Click);
            // 
            // fbtnPrint
            // 
            resources.ApplyResources(this.fbtnPrint, "fbtnPrint");
            this.fbtnPrint.BackColor = System.Drawing.Color.Transparent;
            this.fbtnPrint.EnabledSet = true;
            this.fbtnPrint.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnPrint.FlatAppearance.BorderSize = 0;
            this.fbtnPrint.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnPrint.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnPrint.Name = "fbtnPrint";
            this.fbtnPrint.UseVisualStyleBackColor = false;
            this.fbtnPrint.MouseDown += new System.Windows.Forms.MouseEventHandler(this.fbtnPrint_MouseDown);
            // 
            // fbtnMakeupInfo
            // 
            resources.ApplyResources(this.fbtnMakeupInfo, "fbtnMakeupInfo");
            this.fbtnMakeupInfo.BackColor = System.Drawing.Color.Transparent;
            this.fbtnMakeupInfo.EnabledSet = true;
            this.fbtnMakeupInfo.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnMakeupInfo.FlatAppearance.BorderSize = 0;
            this.fbtnMakeupInfo.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnMakeupInfo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnMakeupInfo.Name = "fbtnMakeupInfo";
            this.fbtnMakeupInfo.UseVisualStyleBackColor = false;
            this.fbtnMakeupInfo.Click += new System.EventHandler(this.fbtnMakeupInfo_Click);
            // 
            // dgvSampleData
            // 
            resources.ApplyResources(this.dgvSampleData, "dgvSampleData");
            this.dgvSampleData.AllowUserToAddRows = false;
            this.dgvSampleData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSampleData.BackgroundColor = System.Drawing.Color.White;
            this.dgvSampleData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSampleData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AssayResultID,
            this.SampleID,
            this.colChioce,
            this.SampleNo,
            this.ItemName,
            this.TestDate,
            this.PMTCounter,
            this.Concentration,
            this.Unit,
            this.Result,
            this.Range,
            this.SampleType,
            this.Status,
            this.ReagentBeach});
            this.dgvSampleData.Name = "dgvSampleData";
            this.dgvSampleData.RowHeadersVisible = false;
            this.dgvSampleData.RowTemplate.Height = 23;
            this.dgvSampleData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSampleData.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSampleData_CellEndEdit);
            this.dgvSampleData.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSampleData_CellValueChanged);
            this.dgvSampleData.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvSampleData_EditingControlShowing);
            this.dgvSampleData.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvSampleData_RowPostPaint);
            this.dgvSampleData.SelectionChanged += new System.EventHandler(this.dgvSampleData_SelectionChanged);
            // 
            // AssayResultID
            // 
            this.AssayResultID.DataPropertyName = "AssayResultID";
            resources.ApplyResources(this.AssayResultID, "AssayResultID");
            this.AssayResultID.Name = "AssayResultID";
            this.AssayResultID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // SampleID
            // 
            this.SampleID.DataPropertyName = "SampleID";
            resources.ApplyResources(this.SampleID, "SampleID");
            this.SampleID.Name = "SampleID";
            this.SampleID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colChioce
            // 
            resources.ApplyResources(this.colChioce, "colChioce");
            this.colChioce.Name = "colChioce";
            // 
            // SampleNo
            // 
            this.SampleNo.DataPropertyName = "SampleNo";
            resources.ApplyResources(this.SampleNo, "SampleNo");
            this.SampleNo.Name = "SampleNo";
            this.SampleNo.ReadOnly = true;
            this.SampleNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ItemName
            // 
            this.ItemName.DataPropertyName = "ItemName";
            resources.ApplyResources(this.ItemName, "ItemName");
            this.ItemName.Name = "ItemName";
            this.ItemName.ReadOnly = true;
            this.ItemName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // TestDate
            // 
            this.TestDate.DataPropertyName = "TestDate";
            resources.ApplyResources(this.TestDate, "TestDate");
            this.TestDate.Name = "TestDate";
            this.TestDate.ReadOnly = true;
            this.TestDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // PMTCounter
            // 
            this.PMTCounter.DataPropertyName = "PMTCounter";
            resources.ApplyResources(this.PMTCounter, "PMTCounter");
            this.PMTCounter.Name = "PMTCounter";
            this.PMTCounter.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Concentration
            // 
            this.Concentration.DataPropertyName = "Concentration";
            resources.ApplyResources(this.Concentration, "Concentration");
            this.Concentration.Name = "Concentration";
            this.Concentration.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Unit
            // 
            this.Unit.DataPropertyName = "Unit";
            resources.ApplyResources(this.Unit, "Unit");
            this.Unit.Name = "Unit";
            this.Unit.ReadOnly = true;
            this.Unit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Result
            // 
            this.Result.DataPropertyName = "Result";
            resources.ApplyResources(this.Result, "Result");
            this.Result.Name = "Result";
            this.Result.ReadOnly = true;
            this.Result.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Range
            // 
            this.Range.DataPropertyName = "Range";
            resources.ApplyResources(this.Range, "Range");
            this.Range.Name = "Range";
            this.Range.ReadOnly = true;
            this.Range.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // SampleType
            // 
            this.SampleType.DataPropertyName = "SampleType";
            resources.ApplyResources(this.SampleType, "SampleType");
            this.SampleType.Name = "SampleType";
            this.SampleType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Status
            // 
            this.Status.DataPropertyName = "Status";
            resources.ApplyResources(this.Status, "Status");
            this.Status.Name = "Status";
            // 
            // ReagentBeach
            // 
            this.ReagentBeach.DataPropertyName = "Batch";
            resources.ApplyResources(this.ReagentBeach, "ReagentBeach");
            this.ReagentBeach.Name = "ReagentBeach";
            // 
            // dgvPatientInfo
            // 
            resources.ApplyResources(this.dgvPatientInfo, "dgvPatientInfo");
            this.dgvPatientInfo.AllowUserToAddRows = false;
            this.dgvPatientInfo.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPatientInfo.BackgroundColor = System.Drawing.Color.White;
            this.dgvPatientInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPatientInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SampleID1,
            this.SendDateTime,
            this.SampleNo1,
            this.PatientName,
            this.Sex,
            this.Age,
            this.ClinicNo,
            this.InpatientArea,
            this.Ward,
            this.BedNo,
            this.MedicaRecordNo,
            this.Diagnosis,
            this.InspectionItems,
            this.AcquisitionTime});
            this.dgvPatientInfo.Name = "dgvPatientInfo";
            this.dgvPatientInfo.ReadOnly = true;
            this.dgvPatientInfo.RowHeadersVisible = false;
            this.dgvPatientInfo.RowTemplate.Height = 23;
            this.dgvPatientInfo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPatientInfo.SelectionChanged += new System.EventHandler(this.dgvPatientInfo_SelectionChanged);
            // 
            // SampleID1
            // 
            this.SampleID1.DataPropertyName = "SampleID";
            resources.ApplyResources(this.SampleID1, "SampleID1");
            this.SampleID1.Name = "SampleID1";
            this.SampleID1.ReadOnly = true;
            this.SampleID1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // SendDateTime
            // 
            this.SendDateTime.DataPropertyName = "SendDateTimeS";
            resources.ApplyResources(this.SendDateTime, "SendDateTime");
            this.SendDateTime.Name = "SendDateTime";
            this.SendDateTime.ReadOnly = true;
            // 
            // SampleNo1
            // 
            this.SampleNo1.DataPropertyName = "SampleNo";
            resources.ApplyResources(this.SampleNo1, "SampleNo1");
            this.SampleNo1.Name = "SampleNo1";
            this.SampleNo1.ReadOnly = true;
            // 
            // PatientName
            // 
            this.PatientName.DataPropertyName = "PatientName";
            resources.ApplyResources(this.PatientName, "PatientName");
            this.PatientName.Name = "PatientName";
            this.PatientName.ReadOnly = true;
            this.PatientName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Sex
            // 
            this.Sex.DataPropertyName = "Sex";
            resources.ApplyResources(this.Sex, "Sex");
            this.Sex.Name = "Sex";
            this.Sex.ReadOnly = true;
            this.Sex.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Age
            // 
            this.Age.DataPropertyName = "Age";
            resources.ApplyResources(this.Age, "Age");
            this.Age.Name = "Age";
            this.Age.ReadOnly = true;
            this.Age.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ClinicNo
            // 
            this.ClinicNo.DataPropertyName = "ClinicNo";
            resources.ApplyResources(this.ClinicNo, "ClinicNo");
            this.ClinicNo.Name = "ClinicNo";
            this.ClinicNo.ReadOnly = true;
            this.ClinicNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // InpatientArea
            // 
            this.InpatientArea.DataPropertyName = "InpatientArea";
            resources.ApplyResources(this.InpatientArea, "InpatientArea");
            this.InpatientArea.Name = "InpatientArea";
            this.InpatientArea.ReadOnly = true;
            this.InpatientArea.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Ward
            // 
            this.Ward.DataPropertyName = "Ward";
            resources.ApplyResources(this.Ward, "Ward");
            this.Ward.Name = "Ward";
            this.Ward.ReadOnly = true;
            this.Ward.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // BedNo
            // 
            this.BedNo.DataPropertyName = "BedNo";
            resources.ApplyResources(this.BedNo, "BedNo");
            this.BedNo.Name = "BedNo";
            this.BedNo.ReadOnly = true;
            this.BedNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // MedicaRecordNo
            // 
            this.MedicaRecordNo.DataPropertyName = "MedicaRecordNo";
            resources.ApplyResources(this.MedicaRecordNo, "MedicaRecordNo");
            this.MedicaRecordNo.Name = "MedicaRecordNo";
            this.MedicaRecordNo.ReadOnly = true;
            this.MedicaRecordNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Diagnosis
            // 
            this.Diagnosis.DataPropertyName = "Diagnosis";
            resources.ApplyResources(this.Diagnosis, "Diagnosis");
            this.Diagnosis.Name = "Diagnosis";
            this.Diagnosis.ReadOnly = true;
            this.Diagnosis.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // InspectionItems
            // 
            this.InspectionItems.DataPropertyName = "InspectionItems";
            resources.ApplyResources(this.InspectionItems, "InspectionItems");
            this.InspectionItems.Name = "InspectionItems";
            this.InspectionItems.ReadOnly = true;
            // 
            // AcquisitionTime
            // 
            this.AcquisitionTime.DataPropertyName = "AcquisitionTime";
            resources.ApplyResources(this.AcquisitionTime, "AcquisitionTime");
            this.AcquisitionTime.Name = "AcquisitionTime";
            this.AcquisitionTime.ReadOnly = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.cmbSelect);
            this.groupBox1.Controls.Add(this.txtSampleNo);
            this.groupBox1.Controls.Add(this.fbtnQuery);
            this.groupBox1.Controls.Add(this.dtpEndDate);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.dtpStartDate);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // cmbSelect
            // 
            resources.ApplyResources(this.cmbSelect, "cmbSelect");
            this.cmbSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelect.FormattingEnabled = true;
            this.cmbSelect.Items.AddRange(new object[] {
            resources.GetString("cmbSelect.Items"),
            resources.GetString("cmbSelect.Items1")});
            this.cmbSelect.Name = "cmbSelect";
            // 
            // txtSampleNo
            // 
            resources.ApplyResources(this.txtSampleNo, "txtSampleNo");
            this.txtSampleNo.Name = "txtSampleNo";
            // 
            // fbtnQuery
            // 
            resources.ApplyResources(this.fbtnQuery, "fbtnQuery");
            this.fbtnQuery.BackColor = System.Drawing.Color.Transparent;
            this.fbtnQuery.EnabledSet = true;
            this.fbtnQuery.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnQuery.FlatAppearance.BorderSize = 0;
            this.fbtnQuery.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnQuery.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnQuery.Name = "fbtnQuery";
            this.fbtnQuery.UseVisualStyleBackColor = false;
            this.fbtnQuery.Click += new System.EventHandler(this.fbtnQuery_Click);
            // 
            // dtpEndDate
            // 
            resources.ApplyResources(this.dtpEndDate, "dtpEndDate");
            this.dtpEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.ShowCheckBox = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // dtpStartDate
            // 
            resources.ApplyResources(this.dtpStartDate, "dtpStartDate");
            this.dtpStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.ShowCheckBox = true;
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.fbtnReturn);
            this.panel1.Controls.Add(this.fbtnSaveData);
            this.panel1.Controls.Add(this.fbtnChoiceCurve);
            this.panel1.Controls.Add(this.fbtnDeleteResult);
            this.panel1.Controls.Add(this.fbtnModifyResult);
            this.panel1.Name = "panel1";
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
            // fbtnSaveData
            // 
            resources.ApplyResources(this.fbtnSaveData, "fbtnSaveData");
            this.fbtnSaveData.BackColor = System.Drawing.Color.Transparent;
            this.fbtnSaveData.EnabledSet = true;
            this.fbtnSaveData.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnSaveData.FlatAppearance.BorderSize = 0;
            this.fbtnSaveData.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnSaveData.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnSaveData.Name = "fbtnSaveData";
            this.fbtnSaveData.UseVisualStyleBackColor = false;
            this.fbtnSaveData.Click += new System.EventHandler(this.fbtnSaveData_Click);
            // 
            // fbtnChoiceCurve
            // 
            resources.ApplyResources(this.fbtnChoiceCurve, "fbtnChoiceCurve");
            this.fbtnChoiceCurve.BackColor = System.Drawing.Color.Transparent;
            this.fbtnChoiceCurve.EnabledSet = true;
            this.fbtnChoiceCurve.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnChoiceCurve.FlatAppearance.BorderSize = 0;
            this.fbtnChoiceCurve.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnChoiceCurve.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnChoiceCurve.Name = "fbtnChoiceCurve";
            this.fbtnChoiceCurve.UseVisualStyleBackColor = false;
            this.fbtnChoiceCurve.Click += new System.EventHandler(this.fbtnChoiceCurve_Click);
            // 
            // fbtnDeleteResult
            // 
            resources.ApplyResources(this.fbtnDeleteResult, "fbtnDeleteResult");
            this.fbtnDeleteResult.BackColor = System.Drawing.Color.Transparent;
            this.fbtnDeleteResult.EnabledSet = true;
            this.fbtnDeleteResult.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnDeleteResult.FlatAppearance.BorderSize = 0;
            this.fbtnDeleteResult.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnDeleteResult.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnDeleteResult.Name = "fbtnDeleteResult";
            this.fbtnDeleteResult.UseVisualStyleBackColor = false;
            this.fbtnDeleteResult.Click += new System.EventHandler(this.fbtnDeleteResult_Click);
            // 
            // fbtnModifyResult
            // 
            resources.ApplyResources(this.fbtnModifyResult, "fbtnModifyResult");
            this.fbtnModifyResult.BackColor = System.Drawing.Color.Transparent;
            this.fbtnModifyResult.EnabledSet = true;
            this.fbtnModifyResult.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnModifyResult.FlatAppearance.BorderSize = 0;
            this.fbtnModifyResult.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnModifyResult.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnModifyResult.Name = "fbtnModifyResult";
            this.fbtnModifyResult.UseVisualStyleBackColor = false;
            this.fbtnModifyResult.Click += new System.EventHandler(this.fbtnModifyResult_Click);
            // 
            // frmResultQuery
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.LightBlue;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Name = "frmResultQuery";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmResultQuery_Load);
            this.SizeChanged += new System.EventHandler(this.frmResultQuery_SizeChanged);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSampleData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPatientInfo)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private CustomControl.FunctionButton fbtnReturn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.Label label2;
        private CustomControl.FunctionButton fbtnQuery;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgvPatientInfo;
        private CustomControl.FunctionButton fbtnMakeupInfo;
        private CustomControl.FunctionButton fbtnPrint;
        private CustomControl.FunctionButton fbtnDeleteResult;
        private CustomControl.FunctionButton fbtnModifyResult;
        private System.Windows.Forms.Label label6;
        private CustomControl.FunctionButton tbnSendResult;
        private CustomControl.FunctionButton fbtnSaveData;
        private CustomControl.FunctionButton fbtnRecalculate;
        private CustomControl.FunctionButton fbtnChoiceCurve;
        private CustomControl.FunctionButton btnExPort;
        private CustomControl.FunctionButton btnImPort;
        private System.Windows.Forms.TextBox txtSampleNo;
        private System.Windows.Forms.ComboBox cmbSelect;
        private System.Windows.Forms.DataGridView dgvSampleData;
        private CustomControl.FunctionButton fbtnTestAgain;
        private System.Windows.Forms.DataGridViewTextBoxColumn AssayResultID;
        private System.Windows.Forms.DataGridViewTextBoxColumn SampleID;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colChioce;
        private System.Windows.Forms.DataGridViewTextBoxColumn SampleNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn TestDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn PMTCounter;
        private System.Windows.Forms.DataGridViewTextBoxColumn Concentration;
        private System.Windows.Forms.DataGridViewTextBoxColumn Unit;
        private System.Windows.Forms.DataGridViewTextBoxColumn Result;
        private System.Windows.Forms.DataGridViewTextBoxColumn Range;
        private System.Windows.Forms.DataGridViewTextBoxColumn SampleType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReagentBeach;
        private System.Windows.Forms.DataGridViewTextBoxColumn SampleID1;
        private System.Windows.Forms.DataGridViewTextBoxColumn SendDateTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn SampleNo1;
        private System.Windows.Forms.DataGridViewTextBoxColumn PatientName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Sex;
        private System.Windows.Forms.DataGridViewTextBoxColumn Age;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClinicNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn InpatientArea;
        private System.Windows.Forms.DataGridViewTextBoxColumn Ward;
        private System.Windows.Forms.DataGridViewTextBoxColumn BedNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn MedicaRecordNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Diagnosis;
        private System.Windows.Forms.DataGridViewTextBoxColumn InspectionItems;
        private System.Windows.Forms.DataGridViewTextBoxColumn AcquisitionTime;
    }
}