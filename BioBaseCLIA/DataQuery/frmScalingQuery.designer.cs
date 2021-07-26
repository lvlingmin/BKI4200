namespace BioBaseCLIA.DataQuery
{
    partial class frmScalingQuery
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmScalingQuery));
            this.MenuCurve = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.itemSave = new System.Windows.Forms.ToolStripMenuItem();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fbtnReturn = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnScalingQuery = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnResultQuery = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnQCQuery = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tabEquation = new System.Windows.Forms.TabControl();
            this.tabCurve = new System.Windows.Forms.TabPage();
            this.definePanal1 = new BioBaseCLIA.CustomControl.definePanal(this.components);
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txtFormulaInfo = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.fbtnShowCurve = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.dgvExistScal = new System.Windows.Forms.DataGridView();
            this.ScalingResultID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActiveDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fbtnPrintCurve = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnCurrentCurve = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnCurrentCals = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvScalingData = new System.Windows.Forms.DataGridView();
            this.colConcentration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRLU = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbxScalInfo = new System.Windows.Forms.GroupBox();
            this.chkTwoPoints = new System.Windows.Forms.CheckBox();
            this.txtItemName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtActiveTime = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.txtRegentBatch = new System.Windows.Forms.TextBox();
            this.lblRegentBatch = new System.Windows.Forms.Label();
            this.lblTestItem = new System.Windows.Forms.Label();
            this.cmbItem = new System.Windows.Forms.ComboBox();
            this.MenuCurve.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabEquation.SuspendLayout();
            this.tabCurve.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExistScal)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvScalingData)).BeginInit();
            this.gbxScalInfo.SuspendLayout();
            this.SuspendLayout();
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
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.fbtnReturn);
            this.panel1.Controls.Add(this.fbtnScalingQuery);
            this.panel1.Controls.Add(this.fbtnResultQuery);
            this.panel1.Controls.Add(this.fbtnQCQuery);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // fbtnReturn
            // 
            this.fbtnReturn.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnReturn, "fbtnReturn");
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
            this.fbtnScalingQuery.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnScalingQuery, "fbtnScalingQuery");
            this.fbtnScalingQuery.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnScalingQuery.FlatAppearance.BorderSize = 0;
            this.fbtnScalingQuery.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnScalingQuery.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnScalingQuery.Name = "fbtnScalingQuery";
            this.fbtnScalingQuery.UseVisualStyleBackColor = false;
            // 
            // fbtnResultQuery
            // 
            this.fbtnResultQuery.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnResultQuery, "fbtnResultQuery");
            this.fbtnResultQuery.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnResultQuery.FlatAppearance.BorderSize = 0;
            this.fbtnResultQuery.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnResultQuery.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnResultQuery.Name = "fbtnResultQuery";
            this.fbtnResultQuery.UseVisualStyleBackColor = false;
            this.fbtnResultQuery.Click += new System.EventHandler(this.fbtnResultQuery_Click);
            // 
            // fbtnQCQuery
            // 
            this.fbtnQCQuery.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnQCQuery, "fbtnQCQuery");
            this.fbtnQCQuery.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnQCQuery.FlatAppearance.BorderSize = 0;
            this.fbtnQCQuery.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnQCQuery.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnQCQuery.Name = "fbtnQCQuery";
            this.fbtnQCQuery.UseVisualStyleBackColor = false;
            this.fbtnQCQuery.Click += new System.EventHandler(this.fbtnQCQuery_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.LightBlue;
            this.groupBox3.Controls.Add(this.tabEquation);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // tabEquation
            // 
            this.tabEquation.Controls.Add(this.tabCurve);
            this.tabEquation.Controls.Add(this.tabPage2);
            resources.ApplyResources(this.tabEquation, "tabEquation");
            this.tabEquation.Name = "tabEquation";
            this.tabEquation.SelectedIndex = 0;
            // 
            // tabCurve
            // 
            this.tabCurve.Controls.Add(this.definePanal1);
            resources.ApplyResources(this.tabCurve, "tabCurve");
            this.tabCurve.Name = "tabCurve";
            this.tabCurve.UseVisualStyleBackColor = true;
            // 
            // definePanal1
            // 
            resources.ApplyResources(this.definePanal1, "definePanal1");
            this.definePanal1.Name = "definePanal1";
            this.definePanal1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.definePanal1_MouseDown);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.txtFormulaInfo);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // txtFormulaInfo
            // 
            resources.ApplyResources(this.txtFormulaInfo, "txtFormulaInfo");
            this.txtFormulaInfo.Name = "txtFormulaInfo";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.fbtnShowCurve);
            this.groupBox2.Controls.Add(this.dgvExistScal);
            this.groupBox2.Controls.Add(this.fbtnPrintCurve);
            this.groupBox2.Controls.Add(this.fbtnCurrentCurve);
            this.groupBox2.Controls.Add(this.fbtnCurrentCals);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // fbtnShowCurve
            // 
            this.fbtnShowCurve.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnShowCurve, "fbtnShowCurve");
            this.fbtnShowCurve.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnShowCurve.FlatAppearance.BorderSize = 0;
            this.fbtnShowCurve.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnShowCurve.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnShowCurve.Name = "fbtnShowCurve";
            this.fbtnShowCurve.UseVisualStyleBackColor = false;
            this.fbtnShowCurve.Click += new System.EventHandler(this.fbtnShowCurve_Click);
            // 
            // dgvExistScal
            // 
            this.dgvExistScal.AllowUserToAddRows = false;
            this.dgvExistScal.BackgroundColor = System.Drawing.Color.White;
            this.dgvExistScal.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvExistScal.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ScalingResultID,
            this.ActiveDate});
            resources.ApplyResources(this.dgvExistScal, "dgvExistScal");
            this.dgvExistScal.Name = "dgvExistScal";
            this.dgvExistScal.ReadOnly = true;
            this.dgvExistScal.RowHeadersVisible = false;
            this.dgvExistScal.RowTemplate.Height = 23;
            this.dgvExistScal.SelectionChanged += new System.EventHandler(this.dgvExistScal_SelectionChanged);
            // 
            // ScalingResultID
            // 
            this.ScalingResultID.DataPropertyName = "ScalingResultID";
            resources.ApplyResources(this.ScalingResultID, "ScalingResultID");
            this.ScalingResultID.Name = "ScalingResultID";
            this.ScalingResultID.ReadOnly = true;
            this.ScalingResultID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ActiveDate
            // 
            this.ActiveDate.DataPropertyName = "ActiveDate";
            resources.ApplyResources(this.ActiveDate, "ActiveDate");
            this.ActiveDate.Name = "ActiveDate";
            this.ActiveDate.ReadOnly = true;
            this.ActiveDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // fbtnPrintCurve
            // 
            this.fbtnPrintCurve.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnPrintCurve, "fbtnPrintCurve");
            this.fbtnPrintCurve.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnPrintCurve.FlatAppearance.BorderSize = 0;
            this.fbtnPrintCurve.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnPrintCurve.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnPrintCurve.Name = "fbtnPrintCurve";
            this.fbtnPrintCurve.UseVisualStyleBackColor = false;
            this.fbtnPrintCurve.MouseDown += new System.Windows.Forms.MouseEventHandler(this.fbtnPrintCurve_MouseDown);
            // 
            // fbtnCurrentCurve
            // 
            this.fbtnCurrentCurve.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnCurrentCurve, "fbtnCurrentCurve");
            this.fbtnCurrentCurve.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnCurrentCurve.FlatAppearance.BorderSize = 0;
            this.fbtnCurrentCurve.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnCurrentCurve.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnCurrentCurve.Name = "fbtnCurrentCurve";
            this.fbtnCurrentCurve.UseVisualStyleBackColor = false;
            this.fbtnCurrentCurve.Click += new System.EventHandler(this.fbtnCurrentCurve_Click);
            // 
            // fbtnCurrentCals
            // 
            this.fbtnCurrentCals.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnCurrentCals, "fbtnCurrentCals");
            this.fbtnCurrentCals.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnCurrentCals.FlatAppearance.BorderSize = 0;
            this.fbtnCurrentCals.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnCurrentCals.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnCurrentCals.Name = "fbtnCurrentCals";
            this.fbtnCurrentCals.UseVisualStyleBackColor = false;
            this.fbtnCurrentCals.Click += new System.EventHandler(this.fbtnCurrentCals_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvScalingData);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // dgvScalingData
            // 
            this.dgvScalingData.AllowUserToAddRows = false;
            this.dgvScalingData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvScalingData.BackgroundColor = System.Drawing.Color.White;
            this.dgvScalingData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvScalingData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colConcentration,
            this.colRLU});
            resources.ApplyResources(this.dgvScalingData, "dgvScalingData");
            this.dgvScalingData.Name = "dgvScalingData";
            this.dgvScalingData.ReadOnly = true;
            this.dgvScalingData.RowHeadersVisible = false;
            this.dgvScalingData.RowTemplate.Height = 23;
            // 
            // colConcentration
            // 
            this.colConcentration.FillWeight = 98.47715F;
            resources.ApplyResources(this.colConcentration, "colConcentration");
            this.colConcentration.Name = "colConcentration";
            this.colConcentration.ReadOnly = true;
            this.colConcentration.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colRLU
            // 
            this.colRLU.FillWeight = 101.5228F;
            resources.ApplyResources(this.colRLU, "colRLU");
            this.colRLU.Name = "colRLU";
            this.colRLU.ReadOnly = true;
            this.colRLU.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // gbxScalInfo
            // 
            this.gbxScalInfo.Controls.Add(this.chkTwoPoints);
            this.gbxScalInfo.Controls.Add(this.txtItemName);
            this.gbxScalInfo.Controls.Add(this.label1);
            this.gbxScalInfo.Controls.Add(this.txtSource);
            this.gbxScalInfo.Controls.Add(this.label3);
            this.gbxScalInfo.Controls.Add(this.txtActiveTime);
            this.gbxScalInfo.Controls.Add(this.label2);
            this.gbxScalInfo.Controls.Add(this.txtStatus);
            this.gbxScalInfo.Controls.Add(this.lblStatus);
            this.gbxScalInfo.Controls.Add(this.txtRegentBatch);
            this.gbxScalInfo.Controls.Add(this.lblRegentBatch);
            resources.ApplyResources(this.gbxScalInfo, "gbxScalInfo");
            this.gbxScalInfo.Name = "gbxScalInfo";
            this.gbxScalInfo.TabStop = false;
            // 
            // chkTwoPoints
            // 
            resources.ApplyResources(this.chkTwoPoints, "chkTwoPoints");
            this.chkTwoPoints.Name = "chkTwoPoints";
            this.chkTwoPoints.UseVisualStyleBackColor = true;
            this.chkTwoPoints.CheckedChanged += new System.EventHandler(this.chkTwoPoints_CheckedChanged);
            // 
            // txtItemName
            // 
            resources.ApplyResources(this.txtItemName, "txtItemName");
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.ReadOnly = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txtSource
            // 
            resources.ApplyResources(this.txtSource, "txtSource");
            this.txtSource.Name = "txtSource";
            this.txtSource.ReadOnly = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // txtActiveTime
            // 
            resources.ApplyResources(this.txtActiveTime, "txtActiveTime");
            this.txtActiveTime.Name = "txtActiveTime";
            this.txtActiveTime.ReadOnly = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // txtStatus
            // 
            resources.ApplyResources(this.txtStatus, "txtStatus");
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            // 
            // lblStatus
            // 
            resources.ApplyResources(this.lblStatus, "lblStatus");
            this.lblStatus.Name = "lblStatus";
            // 
            // txtRegentBatch
            // 
            resources.ApplyResources(this.txtRegentBatch, "txtRegentBatch");
            this.txtRegentBatch.Name = "txtRegentBatch";
            this.txtRegentBatch.ReadOnly = true;
            // 
            // lblRegentBatch
            // 
            resources.ApplyResources(this.lblRegentBatch, "lblRegentBatch");
            this.lblRegentBatch.Name = "lblRegentBatch";
            // 
            // lblTestItem
            // 
            resources.ApplyResources(this.lblTestItem, "lblTestItem");
            this.lblTestItem.Name = "lblTestItem";
            // 
            // cmbItem
            // 
            this.cmbItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItem.FormattingEnabled = true;
            resources.ApplyResources(this.cmbItem, "cmbItem");
            this.cmbItem.Name = "cmbItem";
            this.cmbItem.SelectedIndexChanged += new System.EventHandler(this.cmbItem_SelectedIndexChanged);
            // 
            // frmScalingQuery
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightBlue;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbxScalInfo);
            this.Controls.Add(this.lblTestItem);
            this.Controls.Add(this.cmbItem);
            this.DoubleBuffered = true;
            this.Name = "frmScalingQuery";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmScalingQuery_Load);
            this.SizeChanged += new System.EventHandler(this.frmScalingQuery_SizeChanged);
            this.MenuCurve.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.tabEquation.ResumeLayout(false);
            this.tabCurve.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvExistScal)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvScalingData)).EndInit();
            this.gbxScalInfo.ResumeLayout(false);
            this.gbxScalInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxScalInfo;
        private System.Windows.Forms.TextBox txtRegentBatch;
        private System.Windows.Forms.Label lblRegentBatch;
        private System.Windows.Forms.ComboBox cmbItem;
        private System.Windows.Forms.Label lblTestItem;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtActiveTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.CheckBox chkTwoPoints;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvScalingData;
        private System.Windows.Forms.TextBox txtItemName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TabControl tabEquation;
        private System.Windows.Forms.TabPage tabCurve;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txtFormulaInfo;
        private CustomControl.definePanal definePanal1;
        private System.Windows.Forms.Panel panel1;
        private CustomControl.FunctionButton fbtnReturn;
        private CustomControl.FunctionButton fbtnScalingQuery;
        private CustomControl.FunctionButton fbtnResultQuery;
        private CustomControl.FunctionButton fbtnQCQuery;
        private CustomControl.FunctionButton fbtnPrintCurve;
        private CustomControl.FunctionButton fbtnCurrentCals;
        private CustomControl.FunctionButton fbtnCurrentCurve;
        private System.Windows.Forms.DataGridView dgvExistScal;
        private CustomControl.FunctionButton fbtnShowCurve;
        private System.Windows.Forms.ContextMenuStrip MenuCurve;
        private System.Windows.Forms.ToolStripMenuItem itemSave;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DataGridViewTextBoxColumn ScalingResultID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ActiveDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colConcentration;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRLU;
    }
}