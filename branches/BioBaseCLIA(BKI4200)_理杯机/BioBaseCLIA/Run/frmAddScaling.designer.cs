namespace BioBaseCLIA.Run
{
    partial class frmAddScaling
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddScaling));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvScaling = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtRegentBatch = new System.Windows.Forms.TextBox();
            this.btnCancel = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnOK = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.showScanConc = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.functionButton2 = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.dgvTestPro = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.tbxTestPro = new System.Windows.Forms.TextBox();
            this.btnScanTestPro = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.functionButton1 = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.codeTextBox = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Conc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvScaling)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTestPro)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvScaling
            // 
            resources.ApplyResources(this.dgvScaling, "dgvScaling");
            this.dgvScaling.AllowUserToAddRows = false;
            this.dgvScaling.AllowUserToDeleteRows = false;
            this.dgvScaling.AllowUserToResizeRows = false;
            this.dgvScaling.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvScaling.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.No,
            this.Conc,
            this.Value});
            this.dgvScaling.Name = "dgvScaling";
            this.dgvScaling.RowTemplate.Height = 23;
            this.dgvScaling.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvScaling_EditingControlShowing);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.dgvScaling);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txtRegentBatch
            // 
            resources.ApplyResources(this.txtRegentBatch, "txtRegentBatch");
            this.txtRegentBatch.Name = "txtRegentBatch";
            this.txtRegentBatch.ReadOnly = true;
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.EnabledSet = true;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.BackColor = System.Drawing.Color.Transparent;
            this.btnOK.EnabledSet = true;
            this.btnOK.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnOK.FlatAppearance.BorderSize = 0;
            this.btnOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // showScanConc
            // 
            resources.ApplyResources(this.showScanConc, "showScanConc");
            this.showScanConc.BackColor = System.Drawing.Color.Transparent;
            this.showScanConc.EnabledSet = true;
            this.showScanConc.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.showScanConc.FlatAppearance.BorderSize = 0;
            this.showScanConc.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.showScanConc.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.showScanConc.Name = "showScanConc";
            this.showScanConc.UseVisualStyleBackColor = false;
            this.showScanConc.Click += new System.EventHandler(this.showScanConc_Click);
            // 
            // functionButton2
            // 
            resources.ApplyResources(this.functionButton2, "functionButton2");
            this.functionButton2.BackColor = System.Drawing.Color.Transparent;
            this.functionButton2.EnabledSet = true;
            this.functionButton2.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.functionButton2.FlatAppearance.BorderSize = 0;
            this.functionButton2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.functionButton2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.functionButton2.Name = "functionButton2";
            this.functionButton2.UseVisualStyleBackColor = false;
            this.functionButton2.Click += new System.EventHandler(this.functionButton2_Click);
            // 
            // dgvTestPro
            // 
            resources.ApplyResources(this.dgvTestPro, "dgvTestPro");
            this.dgvTestPro.AllowUserToAddRows = false;
            this.dgvTestPro.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTestPro.Name = "dgvTestPro";
            this.dgvTestPro.RowTemplate.Height = 23;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // tbxTestPro
            // 
            resources.ApplyResources(this.tbxTestPro, "tbxTestPro");
            this.tbxTestPro.Name = "tbxTestPro";
            this.tbxTestPro.ReadOnly = true;
            // 
            // btnScanTestPro
            // 
            resources.ApplyResources(this.btnScanTestPro, "btnScanTestPro");
            this.btnScanTestPro.BackColor = System.Drawing.Color.Transparent;
            this.btnScanTestPro.EnabledSet = true;
            this.btnScanTestPro.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnScanTestPro.FlatAppearance.BorderSize = 0;
            this.btnScanTestPro.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnScanTestPro.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnScanTestPro.Name = "btnScanTestPro";
            this.btnScanTestPro.UseVisualStyleBackColor = false;
            this.btnScanTestPro.Click += new System.EventHandler(this.btnScanTestPro_Click);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.dgvTestPro);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.CheckBox1_CheckedChanged);
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.btnCancel);
            this.groupBox3.Controls.Add(this.btnOK);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // functionButton1
            // 
            resources.ApplyResources(this.functionButton1, "functionButton1");
            this.functionButton1.BackColor = System.Drawing.Color.Transparent;
            this.functionButton1.EnabledSet = true;
            this.functionButton1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.functionButton1.FlatAppearance.BorderSize = 0;
            this.functionButton1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.functionButton1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.functionButton1.Name = "functionButton1";
            this.functionButton1.UseVisualStyleBackColor = false;
            this.functionButton1.Click += new System.EventHandler(this.FunctionButton1_Click);
            // 
            // codeTextBox
            // 
            resources.ApplyResources(this.codeTextBox, "codeTextBox");
            this.codeTextBox.Name = "codeTextBox";
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Controls.Add(this.codeTextBox);
            this.groupBox4.Controls.Add(this.functionButton1);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // No
            // 
            this.No.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.No.DataPropertyName = "No";
            resources.ApplyResources(this.No, "No");
            this.No.Name = "No";
            this.No.ReadOnly = true;
            // 
            // Conc
            // 
            this.Conc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Conc.DataPropertyName = "Conc";
            resources.ApplyResources(this.Conc, "Conc");
            this.Conc.Name = "Conc";
            // 
            // Value
            // 
            this.Value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Value.DataPropertyName = "Value";
            dataGridViewCellStyle1.Format = "N0";
            dataGridViewCellStyle1.NullValue = null;
            this.Value.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.Value, "Value");
            this.Value.Name = "Value";
            // 
            // frmAddScaling
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnScanTestPro);
            this.Controls.Add(this.functionButton2);
            this.Controls.Add(this.tbxTestPro);
            this.Controls.Add(this.showScanConc);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtRegentBatch);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmAddScaling";
            this.Load += new System.EventHandler(this.frmAddScaling_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvScaling)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTestPro)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvScaling;
        private System.Windows.Forms.GroupBox groupBox1;
        private CustomControl.FunctionButton btnOK;
        private CustomControl.FunctionButton btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtRegentBatch;
        private CustomControl.FunctionButton showScanConc;
        private CustomControl.FunctionButton functionButton2;
        private System.Windows.Forms.DataGridView dgvTestPro;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbxTestPro;
        private CustomControl.FunctionButton btnScanTestPro;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private CustomControl.FunctionButton functionButton1;
        private CustomControl.userTextBoxBase codeTextBox;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.DataGridViewTextBoxColumn No;
        private System.Windows.Forms.DataGridViewTextBoxColumn Conc;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
    }
}