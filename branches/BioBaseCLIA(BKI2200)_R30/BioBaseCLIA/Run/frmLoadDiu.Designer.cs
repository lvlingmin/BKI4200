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
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(40, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 26;
            this.label2.Text = "试剂位置:";
            // 
            // txtRegentPos
            // 
            this.txtRegentPos.Enabled = false;
            this.txtRegentPos.Location = new System.Drawing.Point(106, 43);
            this.txtRegentPos.Name = "txtRegentPos";
            this.txtRegentPos.Size = new System.Drawing.Size(97, 21);
            this.txtRegentPos.TabIndex = 27;
            this.txtRegentPos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cmbDiuPos
            // 
            this.cmbDiuPos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDiuPos.FormattingEnabled = true;
            this.cmbDiuPos.Items.AddRange(new object[] {
            "样本稀释液一",
            "样本稀释液二",
            "样本稀释液三",
            "样本稀释液四",
            "样本稀释液五",
            "样本稀释液六"});
            this.cmbDiuPos.Location = new System.Drawing.Point(106, 77);
            this.cmbDiuPos.Name = "cmbDiuPos";
            this.cmbDiuPos.Size = new System.Drawing.Size(97, 20);
            this.cmbDiuPos.TabIndex = 35;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(28, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 12);
            this.label3.TabIndex = 34;
            this.label3.Text = "稀释液位置:";
            // 
            // functionButton1
            // 
            this.functionButton1.BackColor = System.Drawing.Color.Transparent;
            this.functionButton1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("functionButton1.BackgroundImage")));
            this.functionButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.functionButton1.EnabledSet = true;
            this.functionButton1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.functionButton1.FlatAppearance.BorderSize = 0;
            this.functionButton1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.functionButton1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.functionButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.functionButton1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.functionButton1.Location = new System.Drawing.Point(146, 141);
            this.functionButton1.Name = "functionButton1";
            this.functionButton1.Size = new System.Drawing.Size(57, 27);
            this.functionButton1.TabIndex = 37;
            this.functionButton1.Text = "解绑";
            this.functionButton1.UseVisualStyleBackColor = false;
            this.functionButton1.Click += new System.EventHandler(this.functionButton1_Click);
            // 
            // btnLoadSubstrate
            // 
            this.btnLoadSubstrate.BackColor = System.Drawing.Color.Transparent;
            this.btnLoadSubstrate.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnLoadSubstrate.BackgroundImage")));
            this.btnLoadSubstrate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnLoadSubstrate.EnabledSet = true;
            this.btnLoadSubstrate.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnLoadSubstrate.FlatAppearance.BorderSize = 0;
            this.btnLoadSubstrate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnLoadSubstrate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnLoadSubstrate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadSubstrate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnLoadSubstrate.Location = new System.Drawing.Point(56, 141);
            this.btnLoadSubstrate.Name = "btnLoadSubstrate";
            this.btnLoadSubstrate.Size = new System.Drawing.Size(57, 27);
            this.btnLoadSubstrate.TabIndex = 36;
            this.btnLoadSubstrate.Text = "绑定";
            this.btnLoadSubstrate.UseVisualStyleBackColor = false;
            this.btnLoadSubstrate.Click += new System.EventHandler(this.btnLoadSubstrate_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(40, 115);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 38;
            this.label1.Text = "绑定状态:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(106, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 17);
            this.label4.TabIndex = 39;
            this.label4.Text = "未绑定";
            // 
            // frmLoadDiu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(259, 223);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.functionButton1);
            this.Controls.Add(this.btnLoadSubstrate);
            this.Controls.Add(this.cmbDiuPos);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtRegentPos);
            this.Name = "frmLoadDiu";
            this.Text = "绑定稀释液";
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