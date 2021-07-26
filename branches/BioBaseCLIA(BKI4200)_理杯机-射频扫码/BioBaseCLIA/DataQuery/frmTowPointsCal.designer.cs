namespace BioBaseCLIA.DataQuery
{
    partial class frmTowPointsCal
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTowPointsCal));
            this.dgvCEData = new System.Windows.Forms.DataGridView();
            this.fbtnOK = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnSetEnable = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnSetDefault = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.colItemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCPoint = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEPoint = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTestTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCEData)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvCEData
            // 
            resources.ApplyResources(this.dgvCEData, "dgvCEData");
            this.dgvCEData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCEData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colItemName,
            this.colCPoint,
            this.colEPoint,
            this.colStatus,
            this.colTestTime});
            this.dgvCEData.Name = "dgvCEData";
            this.dgvCEData.RowTemplate.Height = 23;
            // 
            // fbtnOK
            // 
            resources.ApplyResources(this.fbtnOK, "fbtnOK");
            this.fbtnOK.BackColor = System.Drawing.Color.Transparent;
            this.fbtnOK.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnOK.FlatAppearance.BorderSize = 0;
            this.fbtnOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnOK.Name = "fbtnOK";
            this.fbtnOK.UseVisualStyleBackColor = false;
            // 
            // fbtnSetEnable
            // 
            resources.ApplyResources(this.fbtnSetEnable, "fbtnSetEnable");
            this.fbtnSetEnable.BackColor = System.Drawing.Color.Transparent;
            this.fbtnSetEnable.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnSetEnable.FlatAppearance.BorderSize = 0;
            this.fbtnSetEnable.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnSetEnable.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnSetEnable.Name = "fbtnSetEnable";
            this.fbtnSetEnable.UseVisualStyleBackColor = false;
            // 
            // fbtnSetDefault
            // 
            resources.ApplyResources(this.fbtnSetDefault, "fbtnSetDefault");
            this.fbtnSetDefault.BackColor = System.Drawing.Color.Transparent;
            this.fbtnSetDefault.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnSetDefault.FlatAppearance.BorderSize = 0;
            this.fbtnSetDefault.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnSetDefault.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnSetDefault.Name = "fbtnSetDefault";
            this.fbtnSetDefault.UseVisualStyleBackColor = false;
            // 
            // colItemName
            // 
            resources.ApplyResources(this.colItemName, "colItemName");
            this.colItemName.Name = "colItemName";
            this.colItemName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colCPoint
            // 
            resources.ApplyResources(this.colCPoint, "colCPoint");
            this.colCPoint.Name = "colCPoint";
            this.colCPoint.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colEPoint
            // 
            resources.ApplyResources(this.colEPoint, "colEPoint");
            this.colEPoint.Name = "colEPoint";
            this.colEPoint.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colStatus
            // 
            resources.ApplyResources(this.colStatus, "colStatus");
            this.colStatus.Name = "colStatus";
            this.colStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colTestTime
            // 
            resources.ApplyResources(this.colTestTime, "colTestTime");
            this.colTestTime.Name = "colTestTime";
            this.colTestTime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // frmTowPointsCal
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PowderBlue;
            this.Controls.Add(this.fbtnOK);
            this.Controls.Add(this.fbtnSetEnable);
            this.Controls.Add(this.fbtnSetDefault);
            this.Controls.Add(this.dgvCEData);
            this.Name = "frmTowPointsCal";
            ((System.ComponentModel.ISupportInitialize)(this.dgvCEData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvCEData;
        private CustomControl.FunctionButton fbtnSetDefault;
        private CustomControl.FunctionButton fbtnSetEnable;
        private CustomControl.FunctionButton fbtnOK;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCPoint;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEPoint;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTestTime;
    }
}