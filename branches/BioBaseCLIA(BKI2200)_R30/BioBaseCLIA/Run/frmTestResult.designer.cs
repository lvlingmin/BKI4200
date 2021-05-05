namespace BioBaseCLIA.Run
{
    partial class frmTestResult
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTestResult));
            this.label1 = new System.Windows.Forms.Label();
            this.dgvResultData = new System.Windows.Forms.DataGridView();
            this.SampleID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SampleNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Position = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SampleType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PMT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Concentration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Unit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Result = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Range1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Range2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReagentBeach = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sco = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SubstratePipe = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fbtnTestResult = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnWorkList = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnReturn = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnLoadSample = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnLoadReagent = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.functionButton1 = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnTestAgain = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnRCalculatResult = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnRSelectCurve = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnPrint = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnExportData = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnSaveResult = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResultData)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // dgvResultData
            // 
            this.dgvResultData.AllowUserToAddRows = false;
            this.dgvResultData.AllowUserToDeleteRows = false;
            this.dgvResultData.AllowUserToResizeColumns = false;
            this.dgvResultData.AllowUserToResizeRows = false;
            this.dgvResultData.BackgroundColor = System.Drawing.Color.White;
            this.dgvResultData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvResultData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SampleID,
            this.No,
            this.SampleNo,
            this.Position,
            this.SampleType,
            this.ItemName,
            this.PMT,
            this.Concentration,
            this.Unit,
            this.Result,
            this.Range1,
            this.Range2,
            this.ReagentBeach,
            this.Status,
            this.sco,
            this.SubstratePipe});
            resources.ApplyResources(this.dgvResultData, "dgvResultData");
            this.dgvResultData.Name = "dgvResultData";
            this.dgvResultData.ReadOnly = true;
            this.dgvResultData.RowHeadersVisible = false;
            this.dgvResultData.RowTemplate.Height = 23;
            this.dgvResultData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvResultData.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvResultData_RowsAdded);
            // 
            // SampleID
            // 
            this.SampleID.DataPropertyName = "SampleID";
            resources.ApplyResources(this.SampleID, "SampleID");
            this.SampleID.Name = "SampleID";
            this.SampleID.ReadOnly = true;
            this.SampleID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // No
            // 
            this.No.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.No.DataPropertyName = "TestID";
            this.No.FillWeight = 56.87672F;
            resources.ApplyResources(this.No, "No");
            this.No.Name = "No";
            this.No.ReadOnly = true;
            this.No.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // SampleNo
            // 
            this.SampleNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.SampleNo.DataPropertyName = "SampleNo";
            this.SampleNo.FillWeight = 160.1993F;
            resources.ApplyResources(this.SampleNo, "SampleNo");
            this.SampleNo.Name = "SampleNo";
            this.SampleNo.ReadOnly = true;
            this.SampleNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Position
            // 
            this.Position.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Position.DataPropertyName = "SamplePos";
            this.Position.FillWeight = 54.45811F;
            resources.ApplyResources(this.Position, "Position");
            this.Position.Name = "Position";
            this.Position.ReadOnly = true;
            this.Position.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // SampleType
            // 
            this.SampleType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.SampleType.DataPropertyName = "SampleType";
            this.SampleType.FillWeight = 91.20197F;
            resources.ApplyResources(this.SampleType, "SampleType");
            this.SampleType.Name = "SampleType";
            this.SampleType.ReadOnly = true;
            this.SampleType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ItemName
            // 
            this.ItemName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ItemName.DataPropertyName = "ItemName";
            this.ItemName.FillWeight = 88.6668F;
            resources.ApplyResources(this.ItemName, "ItemName");
            this.ItemName.Name = "ItemName";
            this.ItemName.ReadOnly = true;
            this.ItemName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // PMT
            // 
            this.PMT.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.PMT.DataPropertyName = "PMT";
            this.PMT.FillWeight = 77.78355F;
            resources.ApplyResources(this.PMT, "PMT");
            this.PMT.Name = "PMT";
            this.PMT.ReadOnly = true;
            this.PMT.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Concentration
            // 
            this.Concentration.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Concentration.DataPropertyName = "Concentration";
            this.Concentration.FillWeight = 76.11167F;
            resources.ApplyResources(this.Concentration, "Concentration");
            this.Concentration.Name = "Concentration";
            this.Concentration.ReadOnly = true;
            this.Concentration.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Unit
            // 
            this.Unit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Unit.DataPropertyName = "Unit";
            this.Unit.FillWeight = 50.51753F;
            resources.ApplyResources(this.Unit, "Unit");
            this.Unit.Name = "Unit";
            this.Unit.ReadOnly = true;
            this.Unit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Result
            // 
            this.Result.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Result.DataPropertyName = "Result";
            this.Result.FillWeight = 58.18436F;
            resources.ApplyResources(this.Result, "Result");
            this.Result.Name = "Result";
            this.Result.ReadOnly = true;
            this.Result.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Range1
            // 
            this.Range1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Range1.DataPropertyName = "Range1";
            resources.ApplyResources(this.Range1, "Range1");
            this.Range1.Name = "Range1";
            this.Range1.ReadOnly = true;
            this.Range1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Range2
            // 
            this.Range2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Range2.DataPropertyName = "Range2";
            resources.ApplyResources(this.Range2, "Range2");
            this.Range2.Name = "Range2";
            this.Range2.ReadOnly = true;
            this.Range2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ReagentBeach
            // 
            this.ReagentBeach.DataPropertyName = "ReagentBeach";
            resources.ApplyResources(this.ReagentBeach, "ReagentBeach");
            this.ReagentBeach.Name = "ReagentBeach";
            this.ReagentBeach.ReadOnly = true;
            this.ReagentBeach.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Status
            // 
            this.Status.DataPropertyName = "Status";
            resources.ApplyResources(this.Status, "Status");
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            this.Status.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // sco
            // 
            this.sco.DataPropertyName = "sco";
            resources.ApplyResources(this.sco, "sco");
            this.sco.Name = "sco";
            this.sco.ReadOnly = true;
            this.sco.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // SubstratePipe
            // 
            this.SubstratePipe.DataPropertyName = "SubstratePipe";
            resources.ApplyResources(this.SubstratePipe, "SubstratePipe");
            this.SubstratePipe.Name = "SubstratePipe";
            this.SubstratePipe.ReadOnly = true;
            this.SubstratePipe.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.fbtnTestResult);
            this.panel1.Controls.Add(this.btnWorkList);
            this.panel1.Controls.Add(this.fbtnReturn);
            this.panel1.Controls.Add(this.btnLoadSample);
            this.panel1.Controls.Add(this.btnLoadReagent);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // fbtnTestResult
            // 
            this.fbtnTestResult.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnTestResult, "fbtnTestResult");
            this.fbtnTestResult.EnabledSet = true;
            this.fbtnTestResult.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnTestResult.FlatAppearance.BorderSize = 0;
            this.fbtnTestResult.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnTestResult.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnTestResult.Name = "fbtnTestResult";
            this.fbtnTestResult.UseVisualStyleBackColor = false;
            // 
            // btnWorkList
            // 
            this.btnWorkList.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnWorkList, "btnWorkList");
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
            // btnLoadSample
            // 
            this.btnLoadSample.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnLoadSample, "btnLoadSample");
            this.btnLoadSample.EnabledSet = true;
            this.btnLoadSample.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnLoadSample.FlatAppearance.BorderSize = 0;
            this.btnLoadSample.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnLoadSample.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnLoadSample.Name = "btnLoadSample";
            this.btnLoadSample.UseVisualStyleBackColor = false;
            this.btnLoadSample.Click += new System.EventHandler(this.btnLoadSample_Click);
            // 
            // btnLoadReagent
            // 
            this.btnLoadReagent.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnLoadReagent, "btnLoadReagent");
            this.btnLoadReagent.EnabledSet = true;
            this.btnLoadReagent.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnLoadReagent.FlatAppearance.BorderSize = 0;
            this.btnLoadReagent.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnLoadReagent.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnLoadReagent.Name = "btnLoadReagent";
            this.btnLoadReagent.UseVisualStyleBackColor = false;
            this.btnLoadReagent.Click += new System.EventHandler(this.btnLoadReagent_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.functionButton1);
            this.groupBox1.Controls.Add(this.fbtnTestAgain);
            this.groupBox1.Controls.Add(this.fbtnRCalculatResult);
            this.groupBox1.Controls.Add(this.fbtnRSelectCurve);
            this.groupBox1.Controls.Add(this.fbtnPrint);
            this.groupBox1.Controls.Add(this.fbtnExportData);
            this.groupBox1.Controls.Add(this.fbtnSaveResult);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // functionButton1
            // 
            this.functionButton1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.functionButton1, "functionButton1");
            this.functionButton1.EnabledSet = true;
            this.functionButton1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.functionButton1.FlatAppearance.BorderSize = 0;
            this.functionButton1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.functionButton1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.functionButton1.Name = "functionButton1";
            this.functionButton1.UseVisualStyleBackColor = false;
            this.functionButton1.Click += new System.EventHandler(this.functionButton1_Click);
            // 
            // fbtnTestAgain
            // 
            this.fbtnTestAgain.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnTestAgain, "fbtnTestAgain");
            this.fbtnTestAgain.EnabledSet = true;
            this.fbtnTestAgain.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnTestAgain.FlatAppearance.BorderSize = 0;
            this.fbtnTestAgain.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnTestAgain.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnTestAgain.Name = "fbtnTestAgain";
            this.fbtnTestAgain.UseVisualStyleBackColor = false;
            this.fbtnTestAgain.Click += new System.EventHandler(this.fbtnTestAgain_Click);
            // 
            // fbtnRCalculatResult
            // 
            this.fbtnRCalculatResult.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnRCalculatResult, "fbtnRCalculatResult");
            this.fbtnRCalculatResult.EnabledSet = true;
            this.fbtnRCalculatResult.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnRCalculatResult.FlatAppearance.BorderSize = 0;
            this.fbtnRCalculatResult.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnRCalculatResult.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnRCalculatResult.Name = "fbtnRCalculatResult";
            this.fbtnRCalculatResult.UseVisualStyleBackColor = false;
            this.fbtnRCalculatResult.Click += new System.EventHandler(this.fbtnRCalculatResult_Click);
            // 
            // fbtnRSelectCurve
            // 
            this.fbtnRSelectCurve.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnRSelectCurve, "fbtnRSelectCurve");
            this.fbtnRSelectCurve.EnabledSet = true;
            this.fbtnRSelectCurve.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnRSelectCurve.FlatAppearance.BorderSize = 0;
            this.fbtnRSelectCurve.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnRSelectCurve.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnRSelectCurve.Name = "fbtnRSelectCurve";
            this.fbtnRSelectCurve.UseVisualStyleBackColor = false;
            this.fbtnRSelectCurve.Click += new System.EventHandler(this.fbtnRSelectCurve_Click);
            // 
            // fbtnPrint
            // 
            this.fbtnPrint.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnPrint, "fbtnPrint");
            this.fbtnPrint.EnabledSet = true;
            this.fbtnPrint.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnPrint.FlatAppearance.BorderSize = 0;
            this.fbtnPrint.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnPrint.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnPrint.Name = "fbtnPrint";
            this.fbtnPrint.UseVisualStyleBackColor = false;
            this.fbtnPrint.Click += new System.EventHandler(this.fbtnPrint_Click);
            // 
            // fbtnExportData
            // 
            this.fbtnExportData.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnExportData, "fbtnExportData");
            this.fbtnExportData.EnabledSet = true;
            this.fbtnExportData.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnExportData.FlatAppearance.BorderSize = 0;
            this.fbtnExportData.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnExportData.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnExportData.Name = "fbtnExportData";
            this.fbtnExportData.UseVisualStyleBackColor = false;
            this.fbtnExportData.Click += new System.EventHandler(this.fbtnExportData_Click);
            // 
            // fbtnSaveResult
            // 
            this.fbtnSaveResult.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnSaveResult, "fbtnSaveResult");
            this.fbtnSaveResult.EnabledSet = true;
            this.fbtnSaveResult.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnSaveResult.FlatAppearance.BorderSize = 0;
            this.fbtnSaveResult.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnSaveResult.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnSaveResult.Name = "fbtnSaveResult";
            this.fbtnSaveResult.UseVisualStyleBackColor = false;
            this.fbtnSaveResult.Click += new System.EventHandler(this.fbtnSaveResult_Click);
            // 
            // frmTestResult
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvResultData);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmTestResult";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmTestResult_Load);
            this.SizeChanged += new System.EventHandler(this.frmTestResult_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.dgvResultData)).EndInit();
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private CustomControl.FunctionButton fbtnTestResult;
        private CustomControl.FunctionButton btnWorkList;
        private CustomControl.FunctionButton fbtnReturn;
        private CustomControl.FunctionButton btnLoadSample;
        private CustomControl.FunctionButton btnLoadReagent;
        private System.Windows.Forms.DataGridView dgvResultData;
        private System.Windows.Forms.Label label1;
        private CustomControl.FunctionButton fbtnSaveResult;
        private CustomControl.FunctionButton fbtnExportData;
        private CustomControl.FunctionButton fbtnPrint;
        private System.Windows.Forms.GroupBox groupBox1;
        private CustomControl.FunctionButton fbtnRCalculatResult;
        private CustomControl.FunctionButton fbtnRSelectCurve;
        private CustomControl.FunctionButton fbtnTestAgain;
        private CustomControl.FunctionButton functionButton1;
        private System.Windows.Forms.DataGridViewTextBoxColumn SampleID;
        private System.Windows.Forms.DataGridViewTextBoxColumn No;
        private System.Windows.Forms.DataGridViewTextBoxColumn SampleNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Position;
        private System.Windows.Forms.DataGridViewTextBoxColumn SampleType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn PMT;
        private System.Windows.Forms.DataGridViewTextBoxColumn Concentration;
        private System.Windows.Forms.DataGridViewTextBoxColumn Unit;
        private System.Windows.Forms.DataGridViewTextBoxColumn Result;
        private System.Windows.Forms.DataGridViewTextBoxColumn Range1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Range2;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReagentBeach;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn sco;
        private System.Windows.Forms.DataGridViewTextBoxColumn SubstratePipe;
    }
}