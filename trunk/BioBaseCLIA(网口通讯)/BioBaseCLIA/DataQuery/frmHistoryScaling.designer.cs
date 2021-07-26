namespace BioBaseCLIA.DataQuery
{
    partial class frmHistoryScaling
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmHistoryScaling));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgvExistScal = new System.Windows.Forms.DataGridView();
            this.dPnlCurve = new BioBaseCLIA.CustomControl.definePanal(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.fbtnChoice = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.ScalingResultID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActiveDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExistScal)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgvExistScal);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // dgvExistScal
            // 
            this.dgvExistScal.AllowUserToAddRows = false;
            this.dgvExistScal.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvExistScal.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ScalingResultID,
            this.ItemName,
            this.ActiveDate,
            this.Status});
            resources.ApplyResources(this.dgvExistScal, "dgvExistScal");
            this.dgvExistScal.Name = "dgvExistScal";
            this.dgvExistScal.ReadOnly = true;
            this.dgvExistScal.RowTemplate.Height = 23;
            this.dgvExistScal.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvExistScal.SelectionChanged += new System.EventHandler(this.dgvExistScal_SelectionChanged);
            // 
            // dPnlCurve
            // 
            resources.ApplyResources(this.dPnlCurve, "dPnlCurve");
            this.dPnlCurve.Name = "dPnlCurve";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // fbtnChoice
            // 
            this.fbtnChoice.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnChoice, "fbtnChoice");
            this.fbtnChoice.EnabledSet = true;
            this.fbtnChoice.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnChoice.FlatAppearance.BorderSize = 0;
            this.fbtnChoice.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnChoice.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnChoice.Name = "fbtnChoice";
            this.fbtnChoice.UseVisualStyleBackColor = false;
            this.fbtnChoice.Click += new System.EventHandler(this.fbtnChoice_Click);
            // 
            // ScalingResultID
            // 
            this.ScalingResultID.DataPropertyName = "ScalingResultID";
            resources.ApplyResources(this.ScalingResultID, "ScalingResultID");
            this.ScalingResultID.Name = "ScalingResultID";
            this.ScalingResultID.ReadOnly = true;
            // 
            // ItemName
            // 
            this.ItemName.DataPropertyName = "ItemName";
            resources.ApplyResources(this.ItemName, "ItemName");
            this.ItemName.Name = "ItemName";
            this.ItemName.ReadOnly = true;
            this.ItemName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ActiveDate
            // 
            this.ActiveDate.DataPropertyName = "ActiveDate";
            resources.ApplyResources(this.ActiveDate, "ActiveDate");
            this.ActiveDate.Name = "ActiveDate";
            this.ActiveDate.ReadOnly = true;
            this.ActiveDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Status
            // 
            this.Status.DataPropertyName = "bstatus";
            resources.ApplyResources(this.Status, "Status");
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            // 
            // frmHistoryScaling
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.fbtnChoice);
            this.Controls.Add(this.dPnlCurve);
            this.Controls.Add(this.groupBox2);
            this.Name = "frmHistoryScaling";
            this.Load += new System.EventHandler(this.frmHistoryScaling_Load);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvExistScal)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgvExistScal;
        private CustomControl.definePanal dPnlCurve;
        private CustomControl.FunctionButton fbtnChoice;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ScalingResultID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ActiveDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
    }
}