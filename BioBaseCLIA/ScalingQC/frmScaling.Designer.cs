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
            this.fbtnSave = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
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
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.fbtnScalingQuery);
            this.panel1.Controls.Add(this.fbtnReturn);
            this.panel1.Controls.Add(this.fbtnQCQuery);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // fbtnScalingQuery
            // 
            this.fbtnScalingQuery.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnScalingQuery, "fbtnScalingQuery");
            this.fbtnScalingQuery.EnabledSet = true;
            this.fbtnScalingQuery.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnScalingQuery.FlatAppearance.BorderSize = 0;
            this.fbtnScalingQuery.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnScalingQuery.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnScalingQuery.Name = "fbtnScalingQuery";
            this.fbtnScalingQuery.UseVisualStyleBackColor = false;
            // 
            // fbtnReturn
            // 
            this.fbtnReturn.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnReturn, "fbtnReturn");
            this.fbtnReturn.EnabledSet = true;
            this.fbtnReturn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnReturn.FlatAppearance.BorderSize = 0;
            this.fbtnReturn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnReturn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnReturn.Name = "fbtnReturn";
            this.fbtnReturn.UseVisualStyleBackColor = false;
            this.fbtnReturn.Click += new System.EventHandler(this.fbtnReturn_Click);
            // 
            // fbtnQCQuery
            // 
            this.fbtnQCQuery.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnQCQuery, "fbtnQCQuery");
            this.fbtnQCQuery.EnabledSet = true;
            this.fbtnQCQuery.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnQCQuery.FlatAppearance.BorderSize = 0;
            this.fbtnQCQuery.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnQCQuery.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnQCQuery.Name = "fbtnQCQuery";
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
            resources.ApplyResources(this.dgvScalData, "dgvScalData");
            this.dgvScalData.Name = "dgvScalData";
            this.dgvScalData.ReadOnly = true;
            this.dgvScalData.RowHeadersVisible = false;
            this.dgvScalData.RowTemplate.Height = 23;
            this.dgvScalData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvScalData.SelectionChanged += new System.EventHandler(this.dgvScalData_SelectionChanged);
            // 
            // colItemName
            // 
            this.colItemName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colItemName.DataPropertyName = "ItemName";
            this.colItemName.FillWeight = 80F;
            resources.ApplyResources(this.colItemName, "colItemName");
            this.colItemName.Name = "colItemName";
            this.colItemName.ReadOnly = true;
            // 
            // colRegentBatch
            // 
            this.colRegentBatch.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colRegentBatch.DataPropertyName = "RegentBatch";
            resources.ApplyResources(this.colRegentBatch, "colRegentBatch");
            this.colRegentBatch.Name = "colRegentBatch";
            this.colRegentBatch.ReadOnly = true;
            // 
            // colISMainCurve
            // 
            this.colISMainCurve.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colISMainCurve.DataPropertyName = "MainCurve";
            this.colISMainCurve.FillWeight = 95F;
            resources.ApplyResources(this.colISMainCurve, "colISMainCurve");
            this.colISMainCurve.Name = "colISMainCurve";
            this.colISMainCurve.ReadOnly = true;
            // 
            // colIsScal
            // 
            this.colIsScal.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colIsScal.DataPropertyName = "Scaling";
            this.colIsScal.FillWeight = 90F;
            resources.ApplyResources(this.colIsScal, "colIsScal");
            this.colIsScal.Name = "colIsScal";
            this.colIsScal.ReadOnly = true;
            // 
            // colCalType
            // 
            this.colCalType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colCalType.DataPropertyName = "CalType";
            this.colCalType.FillWeight = 70F;
            resources.ApplyResources(this.colCalType, "colCalType");
            this.colCalType.Name = "colCalType";
            this.colCalType.ReadOnly = true;
            // 
            // colActiveDate
            // 
            this.colActiveDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colActiveDate.DataPropertyName = "ActiveDate";
            this.colActiveDate.FillWeight = 70F;
            resources.ApplyResources(this.colActiveDate, "colActiveDate");
            this.colActiveDate.Name = "colActiveDate";
            this.colActiveDate.ReadOnly = true;
            // 
            // colValidDate
            // 
            this.colValidDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colValidDate.DataPropertyName = "ValidDate";
            resources.ApplyResources(this.colValidDate, "colValidDate");
            this.colValidDate.Name = "colValidDate";
            this.colValidDate.ReadOnly = true;
            // 
            // ExpiryDate
            // 
            this.ExpiryDate.DataPropertyName = "ExpiryDate";
            resources.ApplyResources(this.ExpiryDate, "ExpiryDate");
            this.ExpiryDate.Name = "ExpiryDate";
            this.ExpiryDate.ReadOnly = true;
            // 
            // fbtnAddMainCurve
            // 
            this.fbtnAddMainCurve.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnAddMainCurve, "fbtnAddMainCurve");
            this.fbtnAddMainCurve.EnabledSet = true;
            this.fbtnAddMainCurve.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnAddMainCurve.FlatAppearance.BorderSize = 0;
            this.fbtnAddMainCurve.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnAddMainCurve.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnAddMainCurve.Name = "fbtnAddMainCurve";
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
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // fbtnReset
            // 
            this.fbtnReset.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnReset, "fbtnReset");
            this.fbtnReset.EnabledSet = true;
            this.fbtnReset.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnReset.FlatAppearance.BorderSize = 0;
            this.fbtnReset.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnReset.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnReset.Name = "fbtnReset";
            this.fbtnReset.UseVisualStyleBackColor = false;
            this.fbtnReset.Click += new System.EventHandler(this.FbtnReset_Click);
            // 
            // fbtnSelectCurve
            // 
            this.fbtnSelectCurve.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnSelectCurve, "fbtnSelectCurve");
            this.fbtnSelectCurve.EnabledSet = true;
            this.fbtnSelectCurve.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnSelectCurve.FlatAppearance.BorderSize = 0;
            this.fbtnSelectCurve.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnSelectCurve.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnSelectCurve.Name = "fbtnSelectCurve";
            this.fbtnSelectCurve.UseVisualStyleBackColor = false;
            this.fbtnSelectCurve.Click += new System.EventHandler(this.fbtnSelectCurve_Click);
            // 
            // fbtnPrintCurve
            // 
            this.fbtnPrintCurve.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnPrintCurve, "fbtnPrintCurve");
            this.fbtnPrintCurve.EnabledSet = true;
            this.fbtnPrintCurve.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnPrintCurve.FlatAppearance.BorderSize = 0;
            this.fbtnPrintCurve.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnPrintCurve.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnPrintCurve.Name = "fbtnPrintCurve";
            this.fbtnPrintCurve.UseVisualStyleBackColor = false;
            this.fbtnPrintCurve.MouseDown += new System.Windows.Forms.MouseEventHandler(this.fbtnPrintCurve_MouseDown);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.fbtnSave);
            this.groupBox2.Controls.Add(this.dgvScalingData);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // fbtnSave
            // 
            this.fbtnSave.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnSave, "fbtnSave");
            this.fbtnSave.EnabledSet = true;
            this.fbtnSave.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnSave.FlatAppearance.BorderSize = 0;
            this.fbtnSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnSave.Name = "fbtnSave";
            this.fbtnSave.UseVisualStyleBackColor = false;
            this.fbtnSave.Click += new System.EventHandler(this.fbtnSave_Click);
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
            resources.ApplyResources(this.dgvScalingData, "dgvScalingData");
            this.dgvScalingData.Name = "dgvScalingData";
            this.dgvScalingData.RowHeadersVisible = false;
            this.dgvScalingData.RowTemplate.Height = 23;
            // 
            // colScalPoint
            // 
            this.colScalPoint.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colScalPoint.FillWeight = 60F;
            resources.ApplyResources(this.colScalPoint, "colScalPoint");
            this.colScalPoint.Name = "colScalPoint";
            // 
            // colConcentration
            // 
            this.colConcentration.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.colConcentration, "colConcentration");
            this.colConcentration.Name = "colConcentration";
            this.colConcentration.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colRLU
            // 
            this.colRLU.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.colRLU, "colRLU");
            this.colRLU.Name = "colRLU";
            this.colRLU.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chbShowMainCurve);
            this.groupBox3.Controls.Add(this.lblR);
            this.groupBox3.Controls.Add(this.lblEquation);
            this.groupBox3.Controls.Add(this.definePanal1);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // chbShowMainCurve
            // 
            resources.ApplyResources(this.chbShowMainCurve, "chbShowMainCurve");
            this.chbShowMainCurve.Name = "chbShowMainCurve";
            this.chbShowMainCurve.UseVisualStyleBackColor = true;
            this.chbShowMainCurve.CheckedChanged += new System.EventHandler(this.chbShowMainCurve_CheckedChanged);
            // 
            // lblR
            // 
            resources.ApplyResources(this.lblR, "lblR");
            this.lblR.Name = "lblR";
            // 
            // lblEquation
            // 
            resources.ApplyResources(this.lblEquation, "lblEquation");
            this.lblEquation.Name = "lblEquation";
            // 
            // definePanal1
            // 
            this.definePanal1.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.definePanal1, "definePanal1");
            this.definePanal1.Name = "definePanal1";
            this.definePanal1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.definePanal1_MouseDown);
            // 
            // MenuCurve
            // 
            this.MenuCurve.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemSave});
            this.MenuCurve.Name = "MenuCurve";
            resources.ApplyResources(this.MenuCurve, "MenuCurve");
            // 
            // itemSave
            // 
            this.itemSave.Name = "itemSave";
            resources.ApplyResources(this.itemSave, "itemSave");
            this.itemSave.Click += new System.EventHandler(this.itemSave_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick_1);
            // 
            // frmScaling
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmScaling";
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
        private CustomControl.FunctionButton fbtnSelectCurve;
        private System.Windows.Forms.Timer timer1;
        private CustomControl.FunctionButton fbtnReset;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRegentBatch;
        private System.Windows.Forms.DataGridViewTextBoxColumn colISMainCurve;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIsScal;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCalType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colActiveDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValidDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn ExpiryDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colScalPoint;
        private System.Windows.Forms.DataGridViewTextBoxColumn colConcentration;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRLU;
        private CustomControl.FunctionButton fbtnSave;
    }
}