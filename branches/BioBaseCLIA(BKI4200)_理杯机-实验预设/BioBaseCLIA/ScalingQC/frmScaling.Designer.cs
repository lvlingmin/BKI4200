namespace BioBaseCLIA.ScalingQC
{
    partial class frmScaling
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmScaling));
            this.label6 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fbtnScalingQuery = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnReturn = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnQCQuery = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.dgvScalData = new System.Windows.Forms.DataGridView();
            this.colItemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRegentBatch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colISMainCurve = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIsScal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCalType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActiveDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colValidDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ExpiryDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fbtnAddMainCurve = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.fbtnReset = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnSelectCurve = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnPrintCurve = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgvScalingData = new System.Windows.Forms.DataGridView();
            this.colScalPoint = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colConcentration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRLU = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chbShowMainCurve = new System.Windows.Forms.CheckBox();
            this.lblR = new System.Windows.Forms.Label();
            this.lblEquation = new System.Windows.Forms.Label();
            this.definePanal1 = new BioBaseCLIA.CustomControl.definePanal(this.components);
            this.MenuCurve = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.itemSave = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvScalData)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvScalingData)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.MenuCurve.SuspendLayout();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Bold);
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(409, 7);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(73, 29);
            this.label6.TabIndex = 45;
            this.label6.Text = "定标";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.fbtnScalingQuery);
            this.panel1.Controls.Add(this.fbtnReturn);
            this.panel1.Controls.Add(this.fbtnQCQuery);
            this.panel1.Location = new System.Drawing.Point(844, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(175, 520);
            this.panel1.TabIndex = 46;
            // 
            // fbtnScalingQuery
            // 
            this.fbtnScalingQuery.BackColor = System.Drawing.Color.Transparent;
            this.fbtnScalingQuery.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("fbtnScalingQuery.BackgroundImage")));
            this.fbtnScalingQuery.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.fbtnScalingQuery.Enabled = false;
            this.fbtnScalingQuery.EnabledSet = true;
            this.fbtnScalingQuery.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnScalingQuery.FlatAppearance.BorderSize = 0;
            this.fbtnScalingQuery.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnScalingQuery.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnScalingQuery.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fbtnScalingQuery.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold);
            this.fbtnScalingQuery.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.fbtnScalingQuery.Location = new System.Drawing.Point(24, 40);
            this.fbtnScalingQuery.Name = "fbtnScalingQuery";
            this.fbtnScalingQuery.Size = new System.Drawing.Size(130, 60);
            this.fbtnScalingQuery.TabIndex = 22;
            this.fbtnScalingQuery.Text = "定标";
            this.fbtnScalingQuery.UseVisualStyleBackColor = false;
            // 
            // fbtnReturn
            // 
            this.fbtnReturn.BackColor = System.Drawing.Color.Transparent;
            this.fbtnReturn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("fbtnReturn.BackgroundImage")));
            this.fbtnReturn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.fbtnReturn.EnabledSet = true;
            this.fbtnReturn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnReturn.FlatAppearance.BorderSize = 0;
            this.fbtnReturn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnReturn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnReturn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fbtnReturn.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold);
            this.fbtnReturn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.fbtnReturn.Location = new System.Drawing.Point(24, 423);
            this.fbtnReturn.Name = "fbtnReturn";
            this.fbtnReturn.Size = new System.Drawing.Size(130, 60);
            this.fbtnReturn.TabIndex = 21;
            this.fbtnReturn.Text = "返回";
            this.fbtnReturn.UseVisualStyleBackColor = false;
            this.fbtnReturn.Click += new System.EventHandler(this.fbtnReturn_Click);
            // 
            // fbtnQCQuery
            // 
            this.fbtnQCQuery.BackColor = System.Drawing.Color.Transparent;
            this.fbtnQCQuery.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("fbtnQCQuery.BackgroundImage")));
            this.fbtnQCQuery.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.fbtnQCQuery.EnabledSet = true;
            this.fbtnQCQuery.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnQCQuery.FlatAppearance.BorderSize = 0;
            this.fbtnQCQuery.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnQCQuery.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnQCQuery.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fbtnQCQuery.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold);
            this.fbtnQCQuery.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.fbtnQCQuery.Location = new System.Drawing.Point(24, 146);
            this.fbtnQCQuery.Name = "fbtnQCQuery";
            this.fbtnQCQuery.Size = new System.Drawing.Size(130, 60);
            this.fbtnQCQuery.TabIndex = 20;
            this.fbtnQCQuery.Text = "质控";
            this.fbtnQCQuery.UseVisualStyleBackColor = false;
            this.fbtnQCQuery.Click += new System.EventHandler(this.fbtnQCQuery_Click);
            // 
            // dgvScalData
            // 
            this.dgvScalData.AllowUserToAddRows = false;
            this.dgvScalData.AllowUserToDeleteRows = false;
            this.dgvScalData.AllowUserToResizeRows = false;
            this.dgvScalData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvScalData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colItemName,
            this.colRegentBatch,
            this.colISMainCurve,
            this.colIsScal,
            this.colCalType,
            this.colActiveDate,
            this.colValidDate,
            this.ExpiryDate});
            this.dgvScalData.Location = new System.Drawing.Point(150, 20);
            this.dgvScalData.Name = "dgvScalData";
            this.dgvScalData.ReadOnly = true;
            this.dgvScalData.RowTemplate.Height = 23;
            this.dgvScalData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvScalData.Size = new System.Drawing.Size(673, 174);
            this.dgvScalData.TabIndex = 0;
            this.dgvScalData.SelectionChanged += new System.EventHandler(this.dgvScalData_SelectionChanged);
            // 
            // colItemName
            // 
            this.colItemName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colItemName.DataPropertyName = "ItemName";
            this.colItemName.FillWeight = 80F;
            this.colItemName.HeaderText = "项目名称";
            this.colItemName.Name = "colItemName";
            this.colItemName.ReadOnly = true;
            // 
            // colRegentBatch
            // 
            this.colRegentBatch.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colRegentBatch.DataPropertyName = "RegentBatch";
            this.colRegentBatch.HeaderText = "试剂批号";
            this.colRegentBatch.Name = "colRegentBatch";
            this.colRegentBatch.ReadOnly = true;
            // 
            // colISMainCurve
            // 
            this.colISMainCurve.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colISMainCurve.DataPropertyName = "MainCurve";
            this.colISMainCurve.FillWeight = 95F;
            this.colISMainCurve.HeaderText = "主曲线(Y/N)";
            this.colISMainCurve.Name = "colISMainCurve";
            this.colISMainCurve.ReadOnly = true;
            // 
            // colIsScal
            // 
            this.colIsScal.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colIsScal.DataPropertyName = "Scaling";
            this.colIsScal.FillWeight = 90F;
            this.colIsScal.HeaderText = "定标(Y/N)";
            this.colIsScal.Name = "colIsScal";
            this.colIsScal.ReadOnly = true;
            // 
            // colCalType
            // 
            this.colCalType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colCalType.DataPropertyName = "CalType";
            this.colCalType.FillWeight = 80F;
            this.colCalType.HeaderText = "校准方式";
            this.colCalType.Name = "colCalType";
            this.colCalType.ReadOnly = true;
            // 
            // colActiveDate
            // 
            this.colActiveDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colActiveDate.DataPropertyName = "ActiveDate";
            this.colActiveDate.HeaderText = "定标日期";
            this.colActiveDate.Name = "colActiveDate";
            this.colActiveDate.ReadOnly = true;
            // 
            // colValidDate
            // 
            this.colValidDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colValidDate.DataPropertyName = "ValidDate";
            this.colValidDate.HeaderText = "有效期";
            this.colValidDate.Name = "colValidDate";
            this.colValidDate.ReadOnly = true;
            this.colValidDate.Visible = false;
            // 
            // ExpiryDate
            // 
            this.ExpiryDate.DataPropertyName = "ExpiryDate";
            this.ExpiryDate.HeaderText = "定标有效期";
            this.ExpiryDate.Name = "ExpiryDate";
            this.ExpiryDate.ReadOnly = true;
            // 
            // fbtnAddMainCurve
            // 
            this.fbtnAddMainCurve.BackColor = System.Drawing.Color.Transparent;
            this.fbtnAddMainCurve.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("fbtnAddMainCurve.BackgroundImage")));
            this.fbtnAddMainCurve.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.fbtnAddMainCurve.EnabledSet = true;
            this.fbtnAddMainCurve.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnAddMainCurve.FlatAppearance.BorderSize = 0;
            this.fbtnAddMainCurve.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnAddMainCurve.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnAddMainCurve.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fbtnAddMainCurve.Location = new System.Drawing.Point(9, 20);
            this.fbtnAddMainCurve.Name = "fbtnAddMainCurve";
            this.fbtnAddMainCurve.Size = new System.Drawing.Size(120, 38);
            this.fbtnAddMainCurve.TabIndex = 47;
            this.fbtnAddMainCurve.Text = "添加主曲线";
            this.fbtnAddMainCurve.UseVisualStyleBackColor = false;
            this.fbtnAddMainCurve.Click += new System.EventHandler(this.fbtnAddMainCurve_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.fbtnReset);
            this.groupBox1.Controls.Add(this.fbtnSelectCurve);
            this.groupBox1.Controls.Add(this.dgvScalData);
            this.groupBox1.Controls.Add(this.fbtnPrintCurve);
            this.groupBox1.Controls.Add(this.fbtnAddMainCurve);
            this.groupBox1.Location = new System.Drawing.Point(3, 36);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(835, 200);
            this.groupBox1.TabIndex = 48;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "数据显示";
            // 
            // fbtnReset
            // 
            this.fbtnReset.BackColor = System.Drawing.Color.Transparent;
            this.fbtnReset.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("fbtnReset.BackgroundImage")));
            this.fbtnReset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.fbtnReset.EnabledSet = true;
            this.fbtnReset.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnReset.FlatAppearance.BorderSize = 0;
            this.fbtnReset.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnReset.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fbtnReset.Location = new System.Drawing.Point(9, 64);
            this.fbtnReset.Name = "fbtnReset";
            this.fbtnReset.Size = new System.Drawing.Size(120, 42);
            this.fbtnReset.TabIndex = 51;
            this.fbtnReset.Text = "重新加载数据";
            this.fbtnReset.UseVisualStyleBackColor = false;
            this.fbtnReset.Click += new System.EventHandler(this.FbtnReset_Click);
            // 
            // fbtnSelectCurve
            // 
            this.fbtnSelectCurve.BackColor = System.Drawing.Color.Transparent;
            this.fbtnSelectCurve.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("fbtnSelectCurve.BackgroundImage")));
            this.fbtnSelectCurve.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.fbtnSelectCurve.EnabledSet = true;
            this.fbtnSelectCurve.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnSelectCurve.FlatAppearance.BorderSize = 0;
            this.fbtnSelectCurve.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnSelectCurve.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnSelectCurve.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fbtnSelectCurve.Location = new System.Drawing.Point(9, 112);
            this.fbtnSelectCurve.Name = "fbtnSelectCurve";
            this.fbtnSelectCurve.Size = new System.Drawing.Size(120, 38);
            this.fbtnSelectCurve.TabIndex = 50;
            this.fbtnSelectCurve.Text = "选择历史定标";
            this.fbtnSelectCurve.UseVisualStyleBackColor = false;
            this.fbtnSelectCurve.Click += new System.EventHandler(this.fbtnSelectCurve_Click);
            // 
            // fbtnPrintCurve
            // 
            this.fbtnPrintCurve.BackColor = System.Drawing.Color.Transparent;
            this.fbtnPrintCurve.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("fbtnPrintCurve.BackgroundImage")));
            this.fbtnPrintCurve.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.fbtnPrintCurve.EnabledSet = true;
            this.fbtnPrintCurve.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnPrintCurve.FlatAppearance.BorderSize = 0;
            this.fbtnPrintCurve.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnPrintCurve.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnPrintCurve.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fbtnPrintCurve.Location = new System.Drawing.Point(9, 156);
            this.fbtnPrintCurve.Name = "fbtnPrintCurve";
            this.fbtnPrintCurve.Size = new System.Drawing.Size(120, 38);
            this.fbtnPrintCurve.TabIndex = 48;
            this.fbtnPrintCurve.Text = "打印定标曲线";
            this.fbtnPrintCurve.UseVisualStyleBackColor = false;
            this.fbtnPrintCurve.MouseDown += new System.Windows.Forms.MouseEventHandler(this.fbtnPrintCurve_MouseDown);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgvScalingData);
            this.groupBox2.Location = new System.Drawing.Point(6, 243);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(257, 292);
            this.groupBox2.TabIndex = 49;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "当前定标点";
            // 
            // dgvScalingData
            // 
            this.dgvScalingData.AllowUserToAddRows = false;
            this.dgvScalingData.AllowUserToDeleteRows = false;
            this.dgvScalingData.AllowUserToResizeRows = false;
            this.dgvScalingData.BackgroundColor = System.Drawing.Color.White;
            this.dgvScalingData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvScalingData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colScalPoint,
            this.colConcentration,
            this.colRLU});
            this.dgvScalingData.Location = new System.Drawing.Point(6, 20);
            this.dgvScalingData.Name = "dgvScalingData";
            this.dgvScalingData.ReadOnly = true;
            this.dgvScalingData.RowHeadersVisible = false;
            this.dgvScalingData.RowTemplate.Height = 23;
            this.dgvScalingData.Size = new System.Drawing.Size(240, 266);
            this.dgvScalingData.TabIndex = 1;
            // 
            // colScalPoint
            // 
            this.colScalPoint.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colScalPoint.HeaderText = "定标点";
            this.colScalPoint.Name = "colScalPoint";
            this.colScalPoint.ReadOnly = true;
            // 
            // colConcentration
            // 
            this.colConcentration.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colConcentration.HeaderText = "浓度";
            this.colConcentration.Name = "colConcentration";
            this.colConcentration.ReadOnly = true;
            this.colConcentration.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colRLU
            // 
            this.colRLU.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colRLU.HeaderText = "RLU";
            this.colRLU.Name = "colRLU";
            this.colRLU.ReadOnly = true;
            this.colRLU.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chbShowMainCurve);
            this.groupBox3.Controls.Add(this.lblR);
            this.groupBox3.Controls.Add(this.lblEquation);
            this.groupBox3.Controls.Add(this.definePanal1);
            this.groupBox3.Location = new System.Drawing.Point(269, 244);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(569, 292);
            this.groupBox3.TabIndex = 50;
            this.groupBox3.TabStop = false;
            // 
            // chbShowMainCurve
            // 
            this.chbShowMainCurve.AutoSize = true;
            this.chbShowMainCurve.Location = new System.Drawing.Point(10, 21);
            this.chbShowMainCurve.Name = "chbShowMainCurve";
            this.chbShowMainCurve.Size = new System.Drawing.Size(84, 16);
            this.chbShowMainCurve.TabIndex = 4;
            this.chbShowMainCurve.Text = "显示主曲线";
            this.chbShowMainCurve.UseVisualStyleBackColor = true;
            this.chbShowMainCurve.CheckedChanged += new System.EventHandler(this.chbShowMainCurve_CheckedChanged);
            // 
            // lblR
            // 
            this.lblR.AutoSize = true;
            this.lblR.Location = new System.Drawing.Point(6, 269);
            this.lblR.Name = "lblR";
            this.lblR.Size = new System.Drawing.Size(53, 12);
            this.lblR.TabIndex = 3;
            this.lblR.Text = "相关系数";
            // 
            // lblEquation
            // 
            this.lblEquation.AutoSize = true;
            this.lblEquation.Location = new System.Drawing.Point(6, 242);
            this.lblEquation.Name = "lblEquation";
            this.lblEquation.Size = new System.Drawing.Size(41, 12);
            this.lblEquation.TabIndex = 2;
            this.lblEquation.Text = "公式：";
            // 
            // definePanal1
            // 
            this.definePanal1.BackColor = System.Drawing.Color.White;
            this.definePanal1.Location = new System.Drawing.Point(6, 41);
            this.definePanal1.Name = "definePanal1";
            this.definePanal1.Size = new System.Drawing.Size(551, 188);
            this.definePanal1.TabIndex = 1;
            this.definePanal1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.definePanal1_MouseDown);
            // 
            // MenuCurve
            // 
            this.MenuCurve.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemSave});
            this.MenuCurve.Name = "MenuCurve";
            this.MenuCurve.Size = new System.Drawing.Size(125, 26);
            // 
            // itemSave
            // 
            this.itemSave.Name = "itemSave";
            this.itemSave.Size = new System.Drawing.Size(124, 22);
            this.itemSave.Text = "保存图像";
            this.itemSave.Click += new System.EventHandler(this.itemSave_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick_1);
            // 
            // frmScaling
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1024, 547);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmScaling";
            this.Text = "frmScaling";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmScaling_FormClosed);
            this.Load += new System.EventHandler(this.frmScaling_Load);
            this.SizeChanged += new System.EventHandler(this.frmScaling_SizeChanged);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvScalData)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvScalingData)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.MenuCurve.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel1;
        private CustomControl.FunctionButton fbtnScalingQuery;
        private CustomControl.FunctionButton fbtnReturn;
        private CustomControl.FunctionButton fbtnQCQuery;
        private System.Windows.Forms.DataGridView dgvScalData;
        private CustomControl.FunctionButton fbtnAddMainCurve;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private CustomControl.FunctionButton fbtnPrintCurve;
        private System.Windows.Forms.DataGridView dgvScalingData;
        private CustomControl.definePanal definePanal1;
        private System.Windows.Forms.Label lblR;
        private System.Windows.Forms.Label lblEquation;
        private System.Windows.Forms.CheckBox chbShowMainCurve;
        private System.Windows.Forms.ContextMenuStrip MenuCurve;
        private System.Windows.Forms.ToolStripMenuItem itemSave;
        private System.Windows.Forms.DataGridViewTextBoxColumn colScalPoint;
        private System.Windows.Forms.DataGridViewTextBoxColumn colConcentration;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRLU;
        private CustomControl.FunctionButton fbtnSelectCurve;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRegentBatch;
        private System.Windows.Forms.DataGridViewTextBoxColumn colISMainCurve;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIsScal;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCalType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colActiveDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValidDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn ExpiryDate;
        private System.Windows.Forms.Timer timer1;
        private CustomControl.FunctionButton fbtnReset;
    }
}