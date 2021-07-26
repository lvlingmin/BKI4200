namespace BioBaseCLIA.Run
{
    partial class frmWarn
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmWarn));
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblWarnInfo = new System.Windows.Forms.Label();
            this.timerCount = new System.Windows.Forms.Timer(this.components);
            this.fbtnStopIgnore = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnRetry = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnWait = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            resources.ApplyResources(this.lblTitle, "lblTitle");
            this.lblTitle.Name = "lblTitle";
            // 
            // lblTime
            // 
            resources.ApplyResources(this.lblTime, "lblTime");
            this.lblTime.Name = "lblTime";
            // 
            // lblWarnInfo
            // 
            resources.ApplyResources(this.lblWarnInfo, "lblWarnInfo");
            this.lblWarnInfo.Name = "lblWarnInfo";
            // 
            // timerCount
            // 
            this.timerCount.Interval = 1000;
            this.timerCount.Tick += new System.EventHandler(this.timerCount_Tick);
            // 
            // fbtnStopIgnore
            // 
            this.fbtnStopIgnore.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnStopIgnore, "fbtnStopIgnore");
            this.fbtnStopIgnore.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnStopIgnore.FlatAppearance.BorderSize = 0;
            this.fbtnStopIgnore.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnStopIgnore.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnStopIgnore.Name = "fbtnStopIgnore";
            this.fbtnStopIgnore.UseVisualStyleBackColor = false;
            this.fbtnStopIgnore.Click += new System.EventHandler(this.fbtnStopIgnore_Click);
            // 
            // fbtnRetry
            // 
            this.fbtnRetry.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnRetry, "fbtnRetry");
            this.fbtnRetry.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnRetry.FlatAppearance.BorderSize = 0;
            this.fbtnRetry.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnRetry.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnRetry.Name = "fbtnRetry";
            this.fbtnRetry.UseVisualStyleBackColor = false;
            this.fbtnRetry.Click += new System.EventHandler(this.fbtnRetry_Click);
            // 
            // fbtnWait
            // 
            this.fbtnWait.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnWait, "fbtnWait");
            this.fbtnWait.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnWait.FlatAppearance.BorderSize = 0;
            this.fbtnWait.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnWait.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnWait.Name = "fbtnWait";
            this.fbtnWait.UseVisualStyleBackColor = false;
            this.fbtnWait.Click += new System.EventHandler(this.fbtnWait_Click);
            // 
            // frmWarn
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.lblWarnInfo);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.fbtnStopIgnore);
            this.Controls.Add(this.fbtnRetry);
            this.Controls.Add(this.fbtnWait);
            this.Name = "frmWarn";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmWarn_FormClosed);
            this.Load += new System.EventHandler(this.frmWarn_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CustomControl.FunctionButton fbtnWait;
        private CustomControl.FunctionButton fbtnRetry;
        private CustomControl.FunctionButton fbtnStopIgnore;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Label lblWarnInfo;
        private System.Windows.Forms.Timer timerCount;
    }
}