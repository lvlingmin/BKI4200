namespace BioBaseCLIA.SysMaintenance
{
    partial class frmBarrelevel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBarrelevel));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblSystem = new System.Windows.Forms.Label();
            this.lblPipeline = new System.Windows.Forms.Label();
            this.lblCold = new System.Windows.Forms.Label();
            this.lblWaste = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // lblSystem
            // 
            resources.ApplyResources(this.lblSystem, "lblSystem");
            this.lblSystem.Name = "lblSystem";
            // 
            // lblPipeline
            // 
            resources.ApplyResources(this.lblPipeline, "lblPipeline");
            this.lblPipeline.Name = "lblPipeline";
            // 
            // lblCold
            // 
            resources.ApplyResources(this.lblCold, "lblCold");
            this.lblCold.Name = "lblCold";
            // 
            // lblWaste
            // 
            resources.ApplyResources(this.lblWaste, "lblWaste");
            this.lblWaste.Name = "lblWaste";
            // 
            // frmBarrelevel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.lblWaste);
            this.Controls.Add(this.lblCold);
            this.Controls.Add(this.lblPipeline);
            this.Controls.Add(this.lblSystem);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "frmBarrelevel";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmBarrelevel_FormClosed);
            this.Load += new System.EventHandler(this.frmBarrelevel_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblSystem;
        private System.Windows.Forms.Label lblPipeline;
        private System.Windows.Forms.Label lblCold;
        private System.Windows.Forms.Label lblWaste;
    }
}