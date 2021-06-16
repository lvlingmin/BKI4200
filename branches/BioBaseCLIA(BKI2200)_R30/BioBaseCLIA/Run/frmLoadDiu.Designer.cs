namespace BioBaseCLIA.Run
{
    partial class frmLoadDiu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLoadDiu));
            this.label2 = new System.Windows.Forms.Label();
            this.txtRegentPos = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.cmbDiuPos = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.functionButton1 = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnLoadSubstrate = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // txtRegentPos
            // 
            resources.ApplyResources(this.txtRegentPos, "txtRegentPos");
            this.txtRegentPos.Name = "txtRegentPos";
            // 
            // cmbDiuPos
            // 
            this.cmbDiuPos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDiuPos.FormattingEnabled = true;
            this.cmbDiuPos.Items.AddRange(new object[] {
            resources.GetString("cmbDiuPos.Items"),
            resources.GetString("cmbDiuPos.Items1"),
            resources.GetString("cmbDiuPos.Items2"),
            resources.GetString("cmbDiuPos.Items3"),
            resources.GetString("cmbDiuPos.Items4"),
            resources.GetString("cmbDiuPos.Items5"),
            resources.GetString("cmbDiuPos.Items6"),
            resources.GetString("cmbDiuPos.Items7"),
            resources.GetString("cmbDiuPos.Items8"),
            resources.GetString("cmbDiuPos.Items9"),
            resources.GetString("cmbDiuPos.Items10"),
            resources.GetString("cmbDiuPos.Items11"),
            resources.GetString("cmbDiuPos.Items12"),
            resources.GetString("cmbDiuPos.Items13"),
            resources.GetString("cmbDiuPos.Items14"),
            resources.GetString("cmbDiuPos.Items15"),
            resources.GetString("cmbDiuPos.Items16")});
            resources.ApplyResources(this.cmbDiuPos, "cmbDiuPos");
            this.cmbDiuPos.Name = "cmbDiuPos";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
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
            // btnLoadSubstrate
            // 
            this.btnLoadSubstrate.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnLoadSubstrate, "btnLoadSubstrate");
            this.btnLoadSubstrate.EnabledSet = true;
            this.btnLoadSubstrate.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnLoadSubstrate.FlatAppearance.BorderSize = 0;
            this.btnLoadSubstrate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnLoadSubstrate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnLoadSubstrate.Name = "btnLoadSubstrate";
            this.btnLoadSubstrate.UseVisualStyleBackColor = false;
            this.btnLoadSubstrate.Click += new System.EventHandler(this.btnLoadSubstrate_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // frmLoadDiu
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.functionButton1);
            this.Controls.Add(this.btnLoadSubstrate);
            this.Controls.Add(this.cmbDiuPos);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtRegentPos);
            this.MaximizeBox = false;
            this.Name = "frmLoadDiu";
            this.Load += new System.EventHandler(this.frmLoadDiu_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private CustomControl.userTextBoxBase txtRegentPos;
        private System.Windows.Forms.ComboBox cmbDiuPos;
        private System.Windows.Forms.Label label3;
        private CustomControl.FunctionButton functionButton1;
        private CustomControl.FunctionButton btnLoadSubstrate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
    }
}