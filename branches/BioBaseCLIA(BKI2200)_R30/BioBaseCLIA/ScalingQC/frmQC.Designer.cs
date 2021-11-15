namespace BioBaseCLIA.ScalingQC
{
    partial class frmQC
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmQC));
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.tabControlMy1 = new BioBaseCLIA.CustomControl.TabControlMy();
            this.tabQCManage = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtXValue = new BioBaseCLIA.CustomControl.userNumTextBox();
            this.cmbProName = new System.Windows.Forms.ComboBox();
            this.dtpValidity = new System.Windows.Forms.DateTimePicker();
            this.dtpAddDate = new System.Windows.Forms.DateTimePicker();
            this.cmbBype = new System.Windows.Forms.ComboBox();
            this.btnSaveQC = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnDeleteQC = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnModifyQC = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnAddQC = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chk41s = new System.Windows.Forms.CheckBox();
            this.chk10x = new System.Windows.Forms.CheckBox();
            this.chk22s = new System.Windows.Forms.CheckBox();
            this.chk13s = new System.Windows.Forms.CheckBox();
            this.chk12s = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtOperator = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSD = new BioBaseCLIA.CustomControl.userNumTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBatch = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dgvQCInfo = new System.Windows.Forms.DataGridView();
            this.QCID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.QCNO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.QCBatch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.XValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProjectName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.QCLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.QCRule = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ValidDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OperatorName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AddDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabQCCurve = new System.Windows.Forms.TabPage();
            this.rtxtLoseControl = new System.Windows.Forms.RichTextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.dgvQCValue = new System.Windows.Forms.DataGridView();
            this.QCResultID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Concentration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TestDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dpnlQCcurve = new BioBaseCLIA.CustomControl.definePanal(this.components);
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dpnlQCcurveDay = new BioBaseCLIA.CustomControl.definePanal(this.components);
            this.rbtnRelativeQC = new System.Windows.Forms.RadioButton();
            this.chbVis = new System.Windows.Forms.CheckBox();
            this.rbtnStandardQC = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.fbtnDelete = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnModify = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.txtQCNewValue = new BioBaseCLIA.CustomControl.userNumTextBox();
            this.fbtnAdd = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.txtQCValue = new BioBaseCLIA.CustomControl.userNumTextBox();
            this.lbQCValueNew = new System.Windows.Forms.Label();
            this.lbQCValue = new System.Windows.Forms.Label();
            this.dtpQCTime = new System.Windows.Forms.DateTimePicker();
            this.lbQCTime = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.functionButton1 = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnPrint = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.cmbQClevel = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.dtpEnd = new System.Windows.Forms.DateTimePicker();
            this.textSDc = new BioBaseCLIA.CustomControl.userNumTextBox();
            this.dtpStart = new System.Windows.Forms.DateTimePicker();
            this.txtMean = new BioBaseCLIA.CustomControl.userNumTextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.lbDifferenceValue = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lbAVGValue = new System.Windows.Forms.Label();
            this.cmbQCBatch = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.cmbItem = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fbtnQCQuery = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnReturn = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnScalingQuery = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.tabControlMy1.SuspendLayout();
            this.tabQCManage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvQCInfo)).BeginInit();
            this.tabQCCurve.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvQCValue)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // tabControlMy1
            // 
            resources.ApplyResources(this.tabControlMy1, "tabControlMy1");
            this.tabControlMy1.Controls.Add(this.tabQCManage);
            this.tabControlMy1.Controls.Add(this.tabQCCurve);
            this.tabControlMy1.Name = "tabControlMy1";
            this.tabControlMy1.SelectedIndex = 0;
            this.tabControlMy1.SelectedIndexChanged += new System.EventHandler(this.tabControlMy1_SelectedIndexChanged);
            // 
            // tabQCManage
            // 
            resources.ApplyResources(this.tabQCManage, "tabQCManage");
            this.tabQCManage.BackColor = System.Drawing.Color.LightBlue;
            this.tabQCManage.Controls.Add(this.groupBox1);
            this.tabQCManage.Controls.Add(this.dgvQCInfo);
            this.tabQCManage.Name = "tabQCManage";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.txtXValue);
            this.groupBox1.Controls.Add(this.cmbProName);
            this.groupBox1.Controls.Add(this.dtpValidity);
            this.groupBox1.Controls.Add(this.dtpAddDate);
            this.groupBox1.Controls.Add(this.cmbBype);
            this.groupBox1.Controls.Add(this.btnSaveQC);
            this.groupBox1.Controls.Add(this.btnDeleteQC);
            this.groupBox1.Controls.Add(this.btnModifyQC);
            this.groupBox1.Controls.Add(this.btnAddQC);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtOperator);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtSD);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtBatch);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // txtXValue
            // 
            resources.ApplyResources(this.txtXValue, "txtXValue");
            this.txtXValue.IsDecimal = true;
            this.txtXValue.IsNull = false;
            this.txtXValue.MaxValue = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.txtXValue.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtXValue.Name = "txtXValue";
            // 
            // cmbProName
            // 
            resources.ApplyResources(this.cmbProName, "cmbProName");
            this.cmbProName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProName.FormattingEnabled = true;
            this.cmbProName.Name = "cmbProName";
            // 
            // dtpValidity
            // 
            resources.ApplyResources(this.dtpValidity, "dtpValidity");
            this.dtpValidity.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpValidity.Name = "dtpValidity";
            // 
            // dtpAddDate
            // 
            resources.ApplyResources(this.dtpAddDate, "dtpAddDate");
            this.dtpAddDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpAddDate.Name = "dtpAddDate";
            // 
            // cmbBype
            // 
            resources.ApplyResources(this.cmbBype, "cmbBype");
            this.cmbBype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBype.FormattingEnabled = true;
            this.cmbBype.Items.AddRange(new object[] {
            resources.GetString("cmbBype.Items"),
            resources.GetString("cmbBype.Items1"),
            resources.GetString("cmbBype.Items2")});
            this.cmbBype.Name = "cmbBype";
            // 
            // btnSaveQC
            // 
            resources.ApplyResources(this.btnSaveQC, "btnSaveQC");
            this.btnSaveQC.BackColor = System.Drawing.Color.Transparent;
            this.btnSaveQC.EnabledSet = true;
            this.btnSaveQC.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnSaveQC.FlatAppearance.BorderSize = 0;
            this.btnSaveQC.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnSaveQC.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnSaveQC.Name = "btnSaveQC";
            this.btnSaveQC.UseVisualStyleBackColor = false;
            this.btnSaveQC.Click += new System.EventHandler(this.btnSaveQC_Click);
            // 
            // btnDeleteQC
            // 
            resources.ApplyResources(this.btnDeleteQC, "btnDeleteQC");
            this.btnDeleteQC.BackColor = System.Drawing.Color.Transparent;
            this.btnDeleteQC.EnabledSet = true;
            this.btnDeleteQC.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnDeleteQC.FlatAppearance.BorderSize = 0;
            this.btnDeleteQC.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnDeleteQC.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnDeleteQC.Name = "btnDeleteQC";
            this.btnDeleteQC.UseVisualStyleBackColor = false;
            this.btnDeleteQC.Click += new System.EventHandler(this.btnDeleteQC_Click);
            // 
            // btnModifyQC
            // 
            resources.ApplyResources(this.btnModifyQC, "btnModifyQC");
            this.btnModifyQC.BackColor = System.Drawing.Color.Transparent;
            this.btnModifyQC.EnabledSet = true;
            this.btnModifyQC.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnModifyQC.FlatAppearance.BorderSize = 0;
            this.btnModifyQC.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnModifyQC.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnModifyQC.Name = "btnModifyQC";
            this.btnModifyQC.UseVisualStyleBackColor = false;
            this.btnModifyQC.Click += new System.EventHandler(this.btnModifyQC_Click);
            // 
            // btnAddQC
            // 
            resources.ApplyResources(this.btnAddQC, "btnAddQC");
            this.btnAddQC.BackColor = System.Drawing.Color.Transparent;
            this.btnAddQC.EnabledSet = true;
            this.btnAddQC.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnAddQC.FlatAppearance.BorderSize = 0;
            this.btnAddQC.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnAddQC.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnAddQC.Name = "btnAddQC";
            this.btnAddQC.UseVisualStyleBackColor = false;
            this.btnAddQC.Click += new System.EventHandler(this.btnAddQC_Click);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.chk41s);
            this.groupBox2.Controls.Add(this.chk10x);
            this.groupBox2.Controls.Add(this.chk22s);
            this.groupBox2.Controls.Add(this.chk13s);
            this.groupBox2.Controls.Add(this.chk12s);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // chk41s
            // 
            resources.ApplyResources(this.chk41s, "chk41s");
            this.chk41s.Name = "chk41s";
            this.chk41s.UseVisualStyleBackColor = true;
            // 
            // chk10x
            // 
            resources.ApplyResources(this.chk10x, "chk10x");
            this.chk10x.Name = "chk10x";
            this.chk10x.UseVisualStyleBackColor = true;
            // 
            // chk22s
            // 
            resources.ApplyResources(this.chk22s, "chk22s");
            this.chk22s.Name = "chk22s";
            this.chk22s.UseVisualStyleBackColor = true;
            // 
            // chk13s
            // 
            resources.ApplyResources(this.chk13s, "chk13s");
            this.chk13s.Name = "chk13s";
            this.chk13s.UseVisualStyleBackColor = true;
            // 
            // chk12s
            // 
            resources.ApplyResources(this.chk12s, "chk12s");
            this.chk12s.Name = "chk12s";
            this.chk12s.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // txtOperator
            // 
            resources.ApplyResources(this.txtOperator, "txtOperator");
            this.txtOperator.Name = "txtOperator";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
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
            // txtSD
            // 
            resources.ApplyResources(this.txtSD, "txtSD");
            this.txtSD.IsDecimal = true;
            this.txtSD.IsNull = false;
            this.txtSD.MaxValue = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txtSD.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtSD.Name = "txtSD";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // txtBatch
            // 
            resources.ApplyResources(this.txtBatch, "txtBatch");
            this.txtBatch.Name = "txtBatch";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // dgvQCInfo
            // 
            resources.ApplyResources(this.dgvQCInfo, "dgvQCInfo");
            this.dgvQCInfo.AllowUserToAddRows = false;
            this.dgvQCInfo.AllowUserToDeleteRows = false;
            this.dgvQCInfo.AllowUserToResizeRows = false;
            this.dgvQCInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvQCInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.QCID,
            this.QCNO,
            this.QCBatch,
            this.XValue,
            this.SD,
            this.Status,
            this.ProjectName,
            this.QCLevel,
            this.QCRule,
            this.ValidDate,
            this.OperatorName,
            this.AddDate});
            this.dgvQCInfo.Name = "dgvQCInfo";
            this.dgvQCInfo.ReadOnly = true;
            this.dgvQCInfo.RowHeadersVisible = false;
            this.dgvQCInfo.RowTemplate.Height = 23;
            this.dgvQCInfo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvQCInfo.SelectionChanged += new System.EventHandler(this.dgvQCInfo_SelectionChanged);
            // 
            // QCID
            // 
            this.QCID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.QCID.DataPropertyName = "QCID";
            resources.ApplyResources(this.QCID, "QCID");
            this.QCID.Name = "QCID";
            this.QCID.ReadOnly = true;
            this.QCID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // QCNO
            // 
            this.QCNO.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.QCNO.DataPropertyName = "QCNumber";
            resources.ApplyResources(this.QCNO, "QCNO");
            this.QCNO.Name = "QCNO";
            this.QCNO.ReadOnly = true;
            this.QCNO.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // QCBatch
            // 
            this.QCBatch.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.QCBatch.DataPropertyName = "Batch";
            resources.ApplyResources(this.QCBatch, "QCBatch");
            this.QCBatch.Name = "QCBatch";
            this.QCBatch.ReadOnly = true;
            this.QCBatch.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // XValue
            // 
            this.XValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.XValue.DataPropertyName = "XValue";
            resources.ApplyResources(this.XValue, "XValue");
            this.XValue.Name = "XValue";
            this.XValue.ReadOnly = true;
            this.XValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // SD
            // 
            this.SD.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.SD.DataPropertyName = "SD";
            resources.ApplyResources(this.SD, "SD");
            this.SD.Name = "SD";
            this.SD.ReadOnly = true;
            this.SD.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Status
            // 
            this.Status.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Status.DataPropertyName = "Status";
            resources.ApplyResources(this.Status, "Status");
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            this.Status.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ProjectName
            // 
            this.ProjectName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ProjectName.DataPropertyName = "ProjectName";
            resources.ApplyResources(this.ProjectName, "ProjectName");
            this.ProjectName.Name = "ProjectName";
            this.ProjectName.ReadOnly = true;
            this.ProjectName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // QCLevel
            // 
            this.QCLevel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.QCLevel.DataPropertyName = "QCLevel";
            resources.ApplyResources(this.QCLevel, "QCLevel");
            this.QCLevel.Name = "QCLevel";
            this.QCLevel.ReadOnly = true;
            this.QCLevel.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // QCRule
            // 
            this.QCRule.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.QCRule.DataPropertyName = "QCRules";
            resources.ApplyResources(this.QCRule, "QCRule");
            this.QCRule.Name = "QCRule";
            this.QCRule.ReadOnly = true;
            this.QCRule.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ValidDate
            // 
            this.ValidDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ValidDate.DataPropertyName = "ValidDate";
            this.ValidDate.FillWeight = 120F;
            resources.ApplyResources(this.ValidDate, "ValidDate");
            this.ValidDate.Name = "ValidDate";
            this.ValidDate.ReadOnly = true;
            this.ValidDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // OperatorName
            // 
            this.OperatorName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.OperatorName.DataPropertyName = "OperatorName";
            resources.ApplyResources(this.OperatorName, "OperatorName");
            this.OperatorName.Name = "OperatorName";
            this.OperatorName.ReadOnly = true;
            this.OperatorName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // AddDate
            // 
            this.AddDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.AddDate.DataPropertyName = "AddDate";
            this.AddDate.FillWeight = 120F;
            resources.ApplyResources(this.AddDate, "AddDate");
            this.AddDate.Name = "AddDate";
            this.AddDate.ReadOnly = true;
            this.AddDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // tabQCCurve
            // 
            resources.ApplyResources(this.tabQCCurve, "tabQCCurve");
            this.tabQCCurve.BackColor = System.Drawing.Color.LightBlue;
            this.tabQCCurve.Controls.Add(this.rtxtLoseControl);
            this.tabQCCurve.Controls.Add(this.label15);
            this.tabQCCurve.Controls.Add(this.groupBox5);
            this.tabQCCurve.Controls.Add(this.groupBox4);
            this.tabQCCurve.Controls.Add(this.groupBox3);
            this.tabQCCurve.Controls.Add(this.groupBox6);
            this.tabQCCurve.Name = "tabQCCurve";
            // 
            // rtxtLoseControl
            // 
            resources.ApplyResources(this.rtxtLoseControl, "rtxtLoseControl");
            this.rtxtLoseControl.Name = "rtxtLoseControl";
            this.rtxtLoseControl.ReadOnly = true;
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // groupBox5
            // 
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Controls.Add(this.dgvQCValue);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // dgvQCValue
            // 
            resources.ApplyResources(this.dgvQCValue, "dgvQCValue");
            this.dgvQCValue.AllowUserToAddRows = false;
            this.dgvQCValue.BackgroundColor = System.Drawing.Color.White;
            this.dgvQCValue.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvQCValue.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.QCResultID,
            this.Concentration,
            this.TestDate});
            this.dgvQCValue.Name = "dgvQCValue";
            this.dgvQCValue.ReadOnly = true;
            this.dgvQCValue.RowHeadersVisible = false;
            this.dgvQCValue.RowTemplate.Height = 23;
            this.dgvQCValue.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvQCValue.DataSourceChanged += new System.EventHandler(this.DgvQCValue_DataSourceChanged);
            this.dgvQCValue.SelectionChanged += new System.EventHandler(this.dgvQCValue_SelectionChanged);
            // 
            // QCResultID
            // 
            this.QCResultID.DataPropertyName = "QCResultID";
            resources.ApplyResources(this.QCResultID, "QCResultID");
            this.QCResultID.Name = "QCResultID";
            this.QCResultID.ReadOnly = true;
            // 
            // Concentration
            // 
            this.Concentration.DataPropertyName = "Concentration";
            resources.ApplyResources(this.Concentration, "Concentration");
            this.Concentration.Name = "Concentration";
            this.Concentration.ReadOnly = true;
            this.Concentration.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // TestDate
            // 
            this.TestDate.DataPropertyName = "TestDate";
            resources.ApplyResources(this.TestDate, "TestDate");
            this.TestDate.Name = "TestDate";
            this.TestDate.ReadOnly = true;
            this.TestDate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Controls.Add(this.tabControl1);
            this.groupBox4.Controls.Add(this.rbtnRelativeQC);
            this.groupBox4.Controls.Add(this.chbVis);
            this.groupBox4.Controls.Add(this.rbtnStandardQC);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.TabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Controls.Add(this.dpnlQCcurve);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dpnlQCcurve
            // 
            resources.ApplyResources(this.dpnlQCcurve, "dpnlQCcurve");
            this.dpnlQCcurve.BackColor = System.Drawing.Color.White;
            this.dpnlQCcurve.Name = "dpnlQCcurve";
            // 
            // tabPage2
            // 
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Controls.Add(this.dpnlQCcurveDay);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dpnlQCcurveDay
            // 
            resources.ApplyResources(this.dpnlQCcurveDay, "dpnlQCcurveDay");
            this.dpnlQCcurveDay.BackColor = System.Drawing.Color.White;
            this.dpnlQCcurveDay.Name = "dpnlQCcurveDay";
            // 
            // rbtnRelativeQC
            // 
            resources.ApplyResources(this.rbtnRelativeQC, "rbtnRelativeQC");
            this.rbtnRelativeQC.Name = "rbtnRelativeQC";
            this.rbtnRelativeQC.TabStop = true;
            this.rbtnRelativeQC.UseVisualStyleBackColor = true;
            this.rbtnRelativeQC.CheckedChanged += new System.EventHandler(this.chbVis_CheckedChanged);
            // 
            // chbVis
            // 
            resources.ApplyResources(this.chbVis, "chbVis");
            this.chbVis.Name = "chbVis";
            this.chbVis.UseVisualStyleBackColor = true;
            this.chbVis.CheckedChanged += new System.EventHandler(this.ChbVis_CheckedChanged_1);
            // 
            // rbtnStandardQC
            // 
            resources.ApplyResources(this.rbtnStandardQC, "rbtnStandardQC");
            this.rbtnStandardQC.Name = "rbtnStandardQC";
            this.rbtnStandardQC.TabStop = true;
            this.rbtnStandardQC.UseVisualStyleBackColor = true;
            this.rbtnStandardQC.CheckedChanged += new System.EventHandler(this.chbVis_CheckedChanged);
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.fbtnDelete);
            this.groupBox3.Controls.Add(this.fbtnModify);
            this.groupBox3.Controls.Add(this.txtQCNewValue);
            this.groupBox3.Controls.Add(this.fbtnAdd);
            this.groupBox3.Controls.Add(this.txtQCValue);
            this.groupBox3.Controls.Add(this.lbQCValueNew);
            this.groupBox3.Controls.Add(this.lbQCValue);
            this.groupBox3.Controls.Add(this.dtpQCTime);
            this.groupBox3.Controls.Add(this.lbQCTime);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // fbtnDelete
            // 
            resources.ApplyResources(this.fbtnDelete, "fbtnDelete");
            this.fbtnDelete.BackColor = System.Drawing.Color.Transparent;
            this.fbtnDelete.EnabledSet = true;
            this.fbtnDelete.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnDelete.FlatAppearance.BorderSize = 0;
            this.fbtnDelete.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnDelete.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnDelete.Name = "fbtnDelete";
            this.fbtnDelete.UseVisualStyleBackColor = false;
            this.fbtnDelete.Click += new System.EventHandler(this.fbtnDelete_Click);
            // 
            // fbtnModify
            // 
            resources.ApplyResources(this.fbtnModify, "fbtnModify");
            this.fbtnModify.BackColor = System.Drawing.Color.Transparent;
            this.fbtnModify.EnabledSet = true;
            this.fbtnModify.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnModify.FlatAppearance.BorderSize = 0;
            this.fbtnModify.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnModify.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnModify.Name = "fbtnModify";
            this.fbtnModify.UseVisualStyleBackColor = false;
            this.fbtnModify.Click += new System.EventHandler(this.fbtnModify_Click);
            // 
            // txtQCNewValue
            // 
            resources.ApplyResources(this.txtQCNewValue, "txtQCNewValue");
            this.txtQCNewValue.BackColor = System.Drawing.Color.White;
            this.txtQCNewValue.IsDecimal = true;
            this.txtQCNewValue.IsNull = false;
            this.txtQCNewValue.MaxValue = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.txtQCNewValue.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtQCNewValue.Name = "txtQCNewValue";
            this.txtQCNewValue.ReadOnly = true;
            // 
            // fbtnAdd
            // 
            resources.ApplyResources(this.fbtnAdd, "fbtnAdd");
            this.fbtnAdd.BackColor = System.Drawing.Color.Transparent;
            this.fbtnAdd.EnabledSet = true;
            this.fbtnAdd.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnAdd.FlatAppearance.BorderSize = 0;
            this.fbtnAdd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnAdd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnAdd.Name = "fbtnAdd";
            this.fbtnAdd.UseVisualStyleBackColor = false;
            this.fbtnAdd.Click += new System.EventHandler(this.fbtnAdd_Click);
            // 
            // txtQCValue
            // 
            resources.ApplyResources(this.txtQCValue, "txtQCValue");
            this.txtQCValue.BackColor = System.Drawing.Color.White;
            this.txtQCValue.IsDecimal = true;
            this.txtQCValue.IsNull = false;
            this.txtQCValue.MaxValue = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.txtQCValue.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtQCValue.Name = "txtQCValue";
            this.txtQCValue.ReadOnly = true;
            // 
            // lbQCValueNew
            // 
            resources.ApplyResources(this.lbQCValueNew, "lbQCValueNew");
            this.lbQCValueNew.Name = "lbQCValueNew";
            // 
            // lbQCValue
            // 
            resources.ApplyResources(this.lbQCValue, "lbQCValue");
            this.lbQCValue.Name = "lbQCValue";
            // 
            // dtpQCTime
            // 
            resources.ApplyResources(this.dtpQCTime, "dtpQCTime");
            this.dtpQCTime.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpQCTime.Name = "dtpQCTime";
            // 
            // lbQCTime
            // 
            resources.ApplyResources(this.lbQCTime, "lbQCTime");
            this.lbQCTime.Name = "lbQCTime";
            // 
            // groupBox6
            // 
            resources.ApplyResources(this.groupBox6, "groupBox6");
            this.groupBox6.Controls.Add(this.functionButton1);
            this.groupBox6.Controls.Add(this.fbtnPrint);
            this.groupBox6.Controls.Add(this.cmbQClevel);
            this.groupBox6.Controls.Add(this.label10);
            this.groupBox6.Controls.Add(this.dtpEnd);
            this.groupBox6.Controls.Add(this.textSDc);
            this.groupBox6.Controls.Add(this.dtpStart);
            this.groupBox6.Controls.Add(this.txtMean);
            this.groupBox6.Controls.Add(this.label11);
            this.groupBox6.Controls.Add(this.lbDifferenceValue);
            this.groupBox6.Controls.Add(this.label12);
            this.groupBox6.Controls.Add(this.lbAVGValue);
            this.groupBox6.Controls.Add(this.cmbQCBatch);
            this.groupBox6.Controls.Add(this.label13);
            this.groupBox6.Controls.Add(this.cmbItem);
            this.groupBox6.Controls.Add(this.label14);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.TabStop = false;
            // 
            // functionButton1
            // 
            resources.ApplyResources(this.functionButton1, "functionButton1");
            this.functionButton1.BackColor = System.Drawing.Color.Transparent;
            this.functionButton1.EnabledSet = true;
            this.functionButton1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.functionButton1.FlatAppearance.BorderSize = 0;
            this.functionButton1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.functionButton1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.functionButton1.Name = "functionButton1";
            this.functionButton1.UseVisualStyleBackColor = false;
            this.functionButton1.Click += new System.EventHandler(this.functionButton1_Click);
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
            // cmbQClevel
            // 
            resources.ApplyResources(this.cmbQClevel, "cmbQClevel");
            this.cmbQClevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQClevel.FormattingEnabled = true;
            this.cmbQClevel.Name = "cmbQClevel";
            this.cmbQClevel.SelectedIndexChanged += new System.EventHandler(this.cmbQClevel_SelectedIndexChanged);
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // dtpEnd
            // 
            resources.ApplyResources(this.dtpEnd, "dtpEnd");
            this.dtpEnd.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpEnd.Name = "dtpEnd";
            // 
            // textSDc
            // 
            resources.ApplyResources(this.textSDc, "textSDc");
            this.textSDc.BackColor = System.Drawing.Color.White;
            this.textSDc.IsDecimal = true;
            this.textSDc.IsNull = false;
            this.textSDc.MaxValue = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.textSDc.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.textSDc.Name = "textSDc";
            this.textSDc.ReadOnly = true;
            // 
            // dtpStart
            // 
            resources.ApplyResources(this.dtpStart, "dtpStart");
            this.dtpStart.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpStart.Name = "dtpStart";
            this.dtpStart.ValueChanged += new System.EventHandler(this.chbVis_CheckedChanged);
            // 
            // txtMean
            // 
            resources.ApplyResources(this.txtMean, "txtMean");
            this.txtMean.BackColor = System.Drawing.Color.White;
            this.txtMean.IsDecimal = true;
            this.txtMean.IsNull = false;
            this.txtMean.MaxValue = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.txtMean.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtMean.Name = "txtMean";
            this.txtMean.ReadOnly = true;
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // lbDifferenceValue
            // 
            resources.ApplyResources(this.lbDifferenceValue, "lbDifferenceValue");
            this.lbDifferenceValue.Name = "lbDifferenceValue";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // lbAVGValue
            // 
            resources.ApplyResources(this.lbAVGValue, "lbAVGValue");
            this.lbAVGValue.Name = "lbAVGValue";
            // 
            // cmbQCBatch
            // 
            resources.ApplyResources(this.cmbQCBatch, "cmbQCBatch");
            this.cmbQCBatch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQCBatch.FormattingEnabled = true;
            this.cmbQCBatch.Name = "cmbQCBatch";
            this.cmbQCBatch.SelectedIndexChanged += new System.EventHandler(this.cmbQCBatch_SelectedIndexChanged);
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // cmbItem
            // 
            resources.ApplyResources(this.cmbItem, "cmbItem");
            this.cmbItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItem.FormattingEnabled = true;
            this.cmbItem.Name = "cmbItem";
            this.cmbItem.SelectedIndexChanged += new System.EventHandler(this.cmbItem_SelectedIndexChanged);
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.fbtnQCQuery);
            this.panel1.Controls.Add(this.fbtnReturn);
            this.panel1.Controls.Add(this.fbtnScalingQuery);
            this.panel1.Name = "panel1";
            // 
            // fbtnQCQuery
            // 
            resources.ApplyResources(this.fbtnQCQuery, "fbtnQCQuery");
            this.fbtnQCQuery.BackColor = System.Drawing.Color.Transparent;
            this.fbtnQCQuery.EnabledSet = true;
            this.fbtnQCQuery.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnQCQuery.FlatAppearance.BorderSize = 0;
            this.fbtnQCQuery.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnQCQuery.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnQCQuery.Name = "fbtnQCQuery";
            this.fbtnQCQuery.UseVisualStyleBackColor = false;
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
            // fbtnScalingQuery
            // 
            resources.ApplyResources(this.fbtnScalingQuery, "fbtnScalingQuery");
            this.fbtnScalingQuery.BackColor = System.Drawing.Color.Transparent;
            this.fbtnScalingQuery.EnabledSet = true;
            this.fbtnScalingQuery.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnScalingQuery.FlatAppearance.BorderSize = 0;
            this.fbtnScalingQuery.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnScalingQuery.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnScalingQuery.Name = "fbtnScalingQuery";
            this.fbtnScalingQuery.UseVisualStyleBackColor = false;
            this.fbtnScalingQuery.Click += new System.EventHandler(this.fbtnScalingQuery_Click);
            // 
            // frmQC
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tabControlMy1);
            this.Controls.Add(this.panel1);
            this.Name = "frmQC";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmQC_Load);
            this.SizeChanged += new System.EventHandler(this.frmQC_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.tabControlMy1.ResumeLayout(false);
            this.tabQCManage.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvQCInfo)).EndInit();
            this.tabQCCurve.ResumeLayout(false);
            this.tabQCCurve.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvQCValue)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private CustomControl.FunctionButton fbtnQCQuery;
        private CustomControl.FunctionButton fbtnReturn;
        private CustomControl.FunctionButton fbtnScalingQuery;
        private CustomControl.TabControlMy tabControlMy1;
        private System.Windows.Forms.TabPage tabQCManage;
        private System.Windows.Forms.TabPage tabQCCurve;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dtpValidity;
        private System.Windows.Forms.DateTimePicker dtpAddDate;
        private System.Windows.Forms.ComboBox cmbBype;
        private CustomControl.FunctionButton btnSaveQC;
        private CustomControl.FunctionButton btnDeleteQC;
        private CustomControl.FunctionButton btnModifyQC;
        private CustomControl.FunctionButton btnAddQC;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chk41s;
        private System.Windows.Forms.CheckBox chk10x;
        private System.Windows.Forms.CheckBox chk22s;
        private System.Windows.Forms.CheckBox chk13s;
        private System.Windows.Forms.CheckBox chk12s;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtOperator;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private BioBaseCLIA.CustomControl.userNumTextBox txtXValue;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private BioBaseCLIA.CustomControl.userNumTextBox txtSD;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBatch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dgvQCInfo;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.DataGridView dgvQCValue;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private CustomControl.definePanal dpnlQCcurve;
        private System.Windows.Forms.TabPage tabPage2;
        private CustomControl.definePanal dpnlQCcurveDay;
        private System.Windows.Forms.RadioButton rbtnRelativeQC;
        private System.Windows.Forms.CheckBox chbVis;
        private System.Windows.Forms.RadioButton rbtnStandardQC;
        private System.Windows.Forms.GroupBox groupBox3;
        private CustomControl.FunctionButton fbtnDelete;
        private CustomControl.FunctionButton fbtnModify;
        private CustomControl.FunctionButton fbtnAdd;
        private BioBaseCLIA.CustomControl.userNumTextBox txtQCNewValue;
        private BioBaseCLIA.CustomControl.userNumTextBox txtQCValue;
        private System.Windows.Forms.Label lbQCValueNew;
        private System.Windows.Forms.Label lbQCValue;
        private System.Windows.Forms.DateTimePicker dtpQCTime;
        private System.Windows.Forms.Label lbQCTime;
        private System.Windows.Forms.GroupBox groupBox6;
        private CustomControl.FunctionButton fbtnPrint;
        private System.Windows.Forms.ComboBox cmbQClevel;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DateTimePicker dtpEnd;
        private BioBaseCLIA.CustomControl.userNumTextBox textSDc;
        private System.Windows.Forms.DateTimePicker dtpStart;
        private BioBaseCLIA.CustomControl.userNumTextBox txtMean;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lbDifferenceValue;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lbAVGValue;
        private System.Windows.Forms.ComboBox cmbQCBatch;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cmbItem;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox cmbProName;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.RichTextBox rtxtLoseControl;
        private CustomControl.FunctionButton functionButton1;
        private System.Windows.Forms.DataGridViewTextBoxColumn QCID;
        private System.Windows.Forms.DataGridViewTextBoxColumn QCNO;
        private System.Windows.Forms.DataGridViewTextBoxColumn QCBatch;
        private System.Windows.Forms.DataGridViewTextBoxColumn XValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn SD;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProjectName;
        private System.Windows.Forms.DataGridViewTextBoxColumn QCLevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn QCRule;
        private System.Windows.Forms.DataGridViewTextBoxColumn ValidDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn OperatorName;
        private System.Windows.Forms.DataGridViewTextBoxColumn AddDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn QCResultID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Concentration;
        private System.Windows.Forms.DataGridViewTextBoxColumn TestDate;
    }
}