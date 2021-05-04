namespace BioBaseCLIA.Run
{
    partial class frmWorkList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmWorkList));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.timeReckon = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.TimeLabel3 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TimeLabel2 = new System.Windows.Forms.Label();
            this.TimeLabel1 = new System.Windows.Forms.Label();
            this.fbtnDelTest = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnAddS = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnAddE = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.dgvWorkListData = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fbtnTestResult = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnWorkList = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnReturn = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnLoadSample = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnLoadReagent = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SampleNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Position = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SampleType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Schedule = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TestStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TestTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SampleID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RegentBatch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SubstratePipe = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RegentPos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWorkListData)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timeReckon
            // 
            this.timeReckon.Interval = 10;
            this.timeReckon.Tick += new System.EventHandler(this.timeReckon_Tick);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // TimeLabel3
            // 
            resources.ApplyResources(this.TimeLabel3, "TimeLabel3");
            this.TimeLabel3.Name = "TimeLabel3";
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
            // TimeLabel2
            // 
            resources.ApplyResources(this.TimeLabel2, "TimeLabel2");
            this.TimeLabel2.Name = "TimeLabel2";
            // 
            // TimeLabel1
            // 
            resources.ApplyResources(this.TimeLabel1, "TimeLabel1");
            this.TimeLabel1.Name = "TimeLabel1";
            // 
            // fbtnDelTest
            // 
            resources.ApplyResources(this.fbtnDelTest, "fbtnDelTest");
            this.fbtnDelTest.BackColor = System.Drawing.Color.Transparent;
            this.fbtnDelTest.EnabledSet = true;
            this.fbtnDelTest.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnDelTest.FlatAppearance.BorderSize = 0;
            this.fbtnDelTest.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnDelTest.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnDelTest.Name = "fbtnDelTest";
            this.fbtnDelTest.UseVisualStyleBackColor = false;
            this.fbtnDelTest.Click += new System.EventHandler(this.fbtnDelTest_Click);
            // 
            // fbtnAddS
            // 
            resources.ApplyResources(this.fbtnAddS, "fbtnAddS");
            this.fbtnAddS.BackColor = System.Drawing.Color.Transparent;
            this.fbtnAddS.EnabledSet = true;
            this.fbtnAddS.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnAddS.FlatAppearance.BorderSize = 0;
            this.fbtnAddS.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnAddS.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnAddS.Name = "fbtnAddS";
            this.fbtnAddS.UseVisualStyleBackColor = false;
            this.fbtnAddS.Click += new System.EventHandler(this.fbtnAddS_Click);
            // 
            // fbtnAddE
            // 
            resources.ApplyResources(this.fbtnAddE, "fbtnAddE");
            this.fbtnAddE.BackColor = System.Drawing.Color.Transparent;
            this.fbtnAddE.EnabledSet = true;
            this.fbtnAddE.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnAddE.FlatAppearance.BorderSize = 0;
            this.fbtnAddE.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnAddE.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnAddE.Name = "fbtnAddE";
            this.fbtnAddE.UseVisualStyleBackColor = false;
            this.fbtnAddE.Click += new System.EventHandler(this.fbtnAddE_Click);
            // 
            // dgvWorkListData
            // 
            resources.ApplyResources(this.dgvWorkListData, "dgvWorkListData");
            this.dgvWorkListData.AllowUserToAddRows = false;
            this.dgvWorkListData.AllowUserToResizeColumns = false;
            this.dgvWorkListData.AllowUserToResizeRows = false;
            this.dgvWorkListData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvWorkListData.BackgroundColor = System.Drawing.Color.White;
            this.dgvWorkListData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvWorkListData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.No,
            this.SampleNo,
            this.Position,
            this.SampleType,
            this.ItemName,
            this.Schedule,
            this.TestStatus,
            this.TestTime,
            this.SampleID,
            this.RegentBatch,
            this.SubstratePipe,
            this.RegentPos});
            this.dgvWorkListData.Name = "dgvWorkListData";
            this.dgvWorkListData.ReadOnly = true;
            this.dgvWorkListData.RowHeadersVisible = false;
            this.dgvWorkListData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvWorkListData.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dgvWorkListData_Scroll);
            this.dgvWorkListData.SelectionChanged += new System.EventHandler(this.dgvWorkListData_SelectionChanged);
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
            this.btnLoadSample.Click += new System.EventHandler(this.btnLoadSample_Click);
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
            // No
            // 
            this.No.DataPropertyName = "TestID";
            this.No.FillWeight = 70F;
            resources.ApplyResources(this.No, "No");
            this.No.Name = "No";
            this.No.ReadOnly = true;
            // 
            // SampleNo
            // 
            this.SampleNo.DataPropertyName = "SampleNo";
            this.SampleNo.FillWeight = 120F;
            resources.ApplyResources(this.SampleNo, "SampleNo");
            this.SampleNo.Name = "SampleNo";
            this.SampleNo.ReadOnly = true;
            // 
            // Position
            // 
            this.Position.DataPropertyName = "SamplePos";
            this.Position.FillWeight = 70F;
            resources.ApplyResources(this.Position, "Position");
            this.Position.Name = "Position";
            this.Position.ReadOnly = true;
            // 
            // SampleType
            // 
            this.SampleType.DataPropertyName = "SampleType";
            resources.ApplyResources(this.SampleType, "SampleType");
            this.SampleType.Name = "SampleType";
            this.SampleType.ReadOnly = true;
            // 
            // ItemName
            // 
            this.ItemName.DataPropertyName = "ItemName";
            resources.ApplyResources(this.ItemName, "ItemName");
            this.ItemName.Name = "ItemName";
            this.ItemName.ReadOnly = true;
            // 
            // Schedule
            // 
            this.Schedule.DataPropertyName = "Schedule";
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            this.Schedule.DefaultCellStyle = dataGridViewCellStyle1;
            this.Schedule.FillWeight = 300F;
            resources.ApplyResources(this.Schedule, "Schedule");
            this.Schedule.Name = "Schedule";
            this.Schedule.ReadOnly = true;
            // 
            // TestStatus
            // 
            this.TestStatus.DataPropertyName = "TestStatus";
            this.TestStatus.FillWeight = 120F;
            resources.ApplyResources(this.TestStatus, "TestStatus");
            this.TestStatus.Name = "TestStatus";
            this.TestStatus.ReadOnly = true;
            // 
            // TestTime
            // 
            this.TestTime.DataPropertyName = "TestTime";
            resources.ApplyResources(this.TestTime, "TestTime");
            this.TestTime.Name = "TestTime";
            this.TestTime.ReadOnly = true;
            // 
            // SampleID
            // 
            this.SampleID.DataPropertyName = "SampleID";
            resources.ApplyResources(this.SampleID, "SampleID");
            this.SampleID.Name = "SampleID";
            this.SampleID.ReadOnly = true;
            // 
            // RegentBatch
            // 
            this.RegentBatch.DataPropertyName = "RegentBatch";
            resources.ApplyResources(this.RegentBatch, "RegentBatch");
            this.RegentBatch.Name = "RegentBatch";
            this.RegentBatch.ReadOnly = true;
            // 
            // SubstratePipe
            // 
            this.SubstratePipe.DataPropertyName = "SubstratePipe";
            resources.ApplyResources(this.SubstratePipe, "SubstratePipe");
            this.SubstratePipe.Name = "SubstratePipe";
            this.SubstratePipe.ReadOnly = true;
            // 
            // RegentPos
            // 
            this.RegentPos.DataPropertyName = "RegentPos";
            resources.ApplyResources(this.RegentPos, "RegentPos");
            this.RegentPos.Name = "RegentPos";
            this.RegentPos.ReadOnly = true;
            // 
            // frmWorkList
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TimeLabel3);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TimeLabel2);
            this.Controls.Add(this.TimeLabel1);
            this.Controls.Add(this.fbtnDelTest);
            this.Controls.Add(this.fbtnAddS);
            this.Controls.Add(this.fbtnAddE);
            this.Controls.Add(this.dgvWorkListData);
            this.Controls.Add(this.panel1);
            this.Name = "frmWorkList";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmWorkList_FormClosed);
            this.Load += new System.EventHandler(this.frmWorkList_Load);
            this.SizeChanged += new System.EventHandler(this.frmWorkList_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.dgvWorkListData)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CustomControl.FunctionButton fbtnReturn;
        private System.Windows.Forms.Panel panel1;
        private CustomControl.FunctionButton btnWorkList;
        private CustomControl.FunctionButton btnLoadSample;
        private CustomControl.FunctionButton btnLoadReagent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvWorkListData;
        private CustomControl.FunctionButton fbtnTestResult;
        private System.Windows.Forms.Timer timeReckon;
        private CustomControl.FunctionButton fbtnAddE;
        private CustomControl.FunctionButton fbtnAddS;
        private CustomControl.FunctionButton fbtnDelTest;
        private System.Windows.Forms.Label TimeLabel1;
        private System.Windows.Forms.Label TimeLabel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label TimeLabel3;
        private System.Windows.Forms.DataGridViewTextBoxColumn No;
        private System.Windows.Forms.DataGridViewTextBoxColumn SampleNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Position;
        private System.Windows.Forms.DataGridViewTextBoxColumn SampleType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Schedule;
        private System.Windows.Forms.DataGridViewTextBoxColumn TestStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn TestTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn SampleID;
        private System.Windows.Forms.DataGridViewTextBoxColumn RegentBatch;
        private System.Windows.Forms.DataGridViewTextBoxColumn SubstratePipe;
        private System.Windows.Forms.DataGridViewTextBoxColumn RegentPos;
    }
}