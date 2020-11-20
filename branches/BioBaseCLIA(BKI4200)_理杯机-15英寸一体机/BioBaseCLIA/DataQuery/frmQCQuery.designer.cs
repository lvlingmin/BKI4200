namespace BioBaseCLIA.DataQuery
{
    partial class frmQCQuery
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmQCQuery));
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.dgvQCValue = new System.Windows.Forms.DataGridView();
            this.QCResultID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Concentration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQCRules = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.fbtnAdd = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.txtQCNewValue = new System.Windows.Forms.TextBox();
            this.txtQCValue = new System.Windows.Forms.TextBox();
            this.lbQCValueNew = new System.Windows.Forms.Label();
            this.lbQCValue = new System.Windows.Forms.Label();
            this.dtpQCTime = new System.Windows.Forms.DateTimePicker();
            this.lbQCTime = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.fbtnPrint = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.cmbQClevel = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.dtpEnd = new System.Windows.Forms.DateTimePicker();
            this.txtSD = new System.Windows.Forms.TextBox();
            this.dtpStart = new System.Windows.Forms.DateTimePicker();
            this.txtMean = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lbDifferenceValue = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbAVGValue = new System.Windows.Forms.Label();
            this.cmbQCBatch = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbItem = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fbtnReturn = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnScalingQuery = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnResultQuery = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnQCQuery = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvQCValue)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
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
            this.colQCRules,
            this.TestDate});
            this.dgvQCValue.Name = "dgvQCValue";
            this.dgvQCValue.ReadOnly = true;
            this.dgvQCValue.RowHeadersVisible = false;
            this.dgvQCValue.RowTemplate.Height = 23;
            this.dgvQCValue.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
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
            // colQCRules
            // 
            this.colQCRules.DataPropertyName = "QCRules";
            resources.ApplyResources(this.colQCRules, "colQCRules");
            this.colQCRules.Name = "colQCRules";
            this.colQCRules.ReadOnly = true;
            this.colQCRules.Resizable = System.Windows.Forms.DataGridViewTriState.True;
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
            this.chbVis.CheckedChanged += new System.EventHandler(this.chbVis_CheckedChanged);
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
            this.groupBox3.Controls.Add(this.fbtnAdd);
            this.groupBox3.Controls.Add(this.txtQCNewValue);
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
            this.fbtnModify.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnModify.FlatAppearance.BorderSize = 0;
            this.fbtnModify.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnModify.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnModify.Name = "fbtnModify";
            this.fbtnModify.UseVisualStyleBackColor = false;
            this.fbtnModify.Click += new System.EventHandler(this.fbtnModify_Click);
            // 
            // fbtnAdd
            // 
            resources.ApplyResources(this.fbtnAdd, "fbtnAdd");
            this.fbtnAdd.BackColor = System.Drawing.Color.Transparent;
            this.fbtnAdd.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnAdd.FlatAppearance.BorderSize = 0;
            this.fbtnAdd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnAdd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnAdd.Name = "fbtnAdd";
            this.fbtnAdd.UseVisualStyleBackColor = false;
            this.fbtnAdd.Click += new System.EventHandler(this.fbtnAdd_Click);
            // 
            // txtQCNewValue
            // 
            resources.ApplyResources(this.txtQCNewValue, "txtQCNewValue");
            this.txtQCNewValue.BackColor = System.Drawing.Color.White;
            this.txtQCNewValue.Name = "txtQCNewValue";
            this.txtQCNewValue.ReadOnly = true;
            // 
            // txtQCValue
            // 
            resources.ApplyResources(this.txtQCValue, "txtQCValue");
            this.txtQCValue.BackColor = System.Drawing.Color.White;
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
            this.dtpQCTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpQCTime.Name = "dtpQCTime";
            // 
            // lbQCTime
            // 
            resources.ApplyResources(this.lbQCTime, "lbQCTime");
            this.lbQCTime.Name = "lbQCTime";
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.fbtnPrint);
            this.groupBox2.Controls.Add(this.cmbQClevel);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.dtpEnd);
            this.groupBox2.Controls.Add(this.txtSD);
            this.groupBox2.Controls.Add(this.dtpStart);
            this.groupBox2.Controls.Add(this.txtMean);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.lbDifferenceValue);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.lbAVGValue);
            this.groupBox2.Controls.Add(this.cmbQCBatch);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.cmbItem);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // fbtnPrint
            // 
            resources.ApplyResources(this.fbtnPrint, "fbtnPrint");
            this.fbtnPrint.BackColor = System.Drawing.Color.Transparent;
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
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // dtpEnd
            // 
            resources.ApplyResources(this.dtpEnd, "dtpEnd");
            this.dtpEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEnd.Name = "dtpEnd";
            this.dtpEnd.ValueChanged += new System.EventHandler(this.dtpStart_ValueChanged);
            // 
            // txtSD
            // 
            resources.ApplyResources(this.txtSD, "txtSD");
            this.txtSD.BackColor = System.Drawing.Color.White;
            this.txtSD.Name = "txtSD";
            this.txtSD.ReadOnly = true;
            // 
            // dtpStart
            // 
            resources.ApplyResources(this.dtpStart, "dtpStart");
            this.dtpStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStart.Name = "dtpStart";
            this.dtpStart.ValueChanged += new System.EventHandler(this.chbVis_CheckedChanged);
            // 
            // txtMean
            // 
            resources.ApplyResources(this.txtMean, "txtMean");
            this.txtMean.BackColor = System.Drawing.Color.White;
            this.txtMean.Name = "txtMean";
            this.txtMean.ReadOnly = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // lbDifferenceValue
            // 
            resources.ApplyResources(this.lbDifferenceValue, "lbDifferenceValue");
            this.lbDifferenceValue.Name = "lbDifferenceValue";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
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
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // cmbItem
            // 
            resources.ApplyResources(this.cmbItem, "cmbItem");
            this.cmbItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItem.FormattingEnabled = true;
            this.cmbItem.Name = "cmbItem";
            this.cmbItem.SelectedIndexChanged += new System.EventHandler(this.cmbItem_SelectedIndexChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.fbtnReturn);
            this.panel1.Controls.Add(this.fbtnScalingQuery);
            this.panel1.Controls.Add(this.fbtnResultQuery);
            this.panel1.Controls.Add(this.fbtnQCQuery);
            this.panel1.Name = "panel1";
            // 
            // fbtnReturn
            // 
            resources.ApplyResources(this.fbtnReturn, "fbtnReturn");
            this.fbtnReturn.BackColor = System.Drawing.Color.Transparent;
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
            this.fbtnScalingQuery.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnScalingQuery.FlatAppearance.BorderSize = 0;
            this.fbtnScalingQuery.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnScalingQuery.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnScalingQuery.Name = "fbtnScalingQuery";
            this.fbtnScalingQuery.UseVisualStyleBackColor = false;
            this.fbtnScalingQuery.Click += new System.EventHandler(this.fbtnScalingQuery_Click);
            // 
            // fbtnResultQuery
            // 
            resources.ApplyResources(this.fbtnResultQuery, "fbtnResultQuery");
            this.fbtnResultQuery.BackColor = System.Drawing.Color.Transparent;
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
            resources.ApplyResources(this.fbtnQCQuery, "fbtnQCQuery");
            this.fbtnQCQuery.BackColor = System.Drawing.Color.Transparent;
            this.fbtnQCQuery.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnQCQuery.FlatAppearance.BorderSize = 0;
            this.fbtnQCQuery.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnQCQuery.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnQCQuery.Name = "fbtnQCQuery";
            this.fbtnQCQuery.UseVisualStyleBackColor = false;
            // 
            // frmQCQuery
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.panel1);
            this.Name = "frmQCQuery";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmQCQuery_Load);
            this.SizeChanged += new System.EventHandler(this.frmQCQuery_SizeChanged);
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvQCValue)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private CustomControl.FunctionButton fbtnReturn;
        private CustomControl.FunctionButton fbtnScalingQuery;
        private CustomControl.FunctionButton fbtnResultQuery;
        private CustomControl.FunctionButton fbtnQCQuery;
        private System.Windows.Forms.DateTimePicker dtpEnd;
        private System.Windows.Forms.DateTimePicker dtpStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbItem;
        private System.Windows.Forms.ComboBox cmbQCBatch;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSD;
        private System.Windows.Forms.TextBox txtMean;
        private System.Windows.Forms.Label lbDifferenceValue;
        private System.Windows.Forms.Label lbAVGValue;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbtnRelativeQC;
        private System.Windows.Forms.RadioButton rbtnStandardQC;
        private System.Windows.Forms.CheckBox chbVis;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label lbQCValueNew;
        private System.Windows.Forms.Label lbQCValue;
        private System.Windows.Forms.DateTimePicker dtpQCTime;
        private System.Windows.Forms.Label lbQCTime;
        private System.Windows.Forms.TextBox txtQCNewValue;
        private System.Windows.Forms.TextBox txtQCValue;
        private System.Windows.Forms.GroupBox groupBox5;
        private CustomControl.FunctionButton fbtnDelete;
        private CustomControl.FunctionButton fbtnModify;
        private CustomControl.FunctionButton fbtnAdd;
        private CustomControl.FunctionButton fbtnPrint;
        private System.Windows.Forms.DataGridView dgvQCValue;
        private System.Windows.Forms.ComboBox cmbQClevel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private CustomControl.definePanal dpnlQCcurveDay;
        private CustomControl.definePanal dpnlQCcurve;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DataGridViewTextBoxColumn QCResultID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Concentration;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQCRules;
        private System.Windows.Forms.DataGridViewTextBoxColumn TestDate;
    }
}