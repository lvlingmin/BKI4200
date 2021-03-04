namespace BioBaseCLIA.DataQuery
{
    partial class frmPatientInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPatientInfo));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtWard = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.label4 = new System.Windows.Forms.Label();
            this.txtInpatientArea = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbSendDoctor = new System.Windows.Forms.ComboBox();
            this.cmbDepartment = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.dateSendDateTime = new System.Windows.Forms.DateTimePicker();
            this.txtCheckDoctor = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.label14 = new System.Windows.Forms.Label();
            this.txtInspectDoctor = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.label13 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.cmbSex = new System.Windows.Forms.ComboBox();
            this.txtDiagnosis = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.label7 = new System.Windows.Forms.Label();
            this.txtMedicaRecordNo = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.label8 = new System.Windows.Forms.Label();
            this.txtBedNo = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.label9 = new System.Windows.Forms.Label();
            this.txtAge = new BioBaseCLIA.CustomControl.userNumTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPatientName = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.label2 = new System.Windows.Forms.Label();
            this.txtClinicNo = new BioBaseCLIA.CustomControl.userTextBoxBase();
            this.label1 = new System.Windows.Forms.Label();
            this.fbtnOK = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnLis = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.txtWard);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtInpatientArea);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cmbSendDoctor);
            this.groupBox1.Controls.Add(this.cmbDepartment);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.dateSendDateTime);
            this.groupBox1.Controls.Add(this.txtCheckDoctor);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.txtInspectDoctor);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.cmbSex);
            this.groupBox1.Controls.Add(this.txtDiagnosis);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtMedicaRecordNo);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtBedNo);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtAge);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtPatientName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtClinicNo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // txtWard
            // 
            resources.ApplyResources(this.txtWard, "txtWard");
            this.txtWard.Name = "txtWard";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // txtInpatientArea
            // 
            resources.ApplyResources(this.txtInpatientArea, "txtInpatientArea");
            this.txtInpatientArea.Name = "txtInpatientArea";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // cmbSendDoctor
            // 
            resources.ApplyResources(this.cmbSendDoctor, "cmbSendDoctor");
            this.cmbSendDoctor.FormattingEnabled = true;
            this.cmbSendDoctor.Name = "cmbSendDoctor";
            // 
            // cmbDepartment
            // 
            resources.ApplyResources(this.cmbDepartment, "cmbDepartment");
            this.cmbDepartment.FormattingEnabled = true;
            this.cmbDepartment.Name = "cmbDepartment";
            this.cmbDepartment.SelectedIndexChanged += new System.EventHandler(this.cmbDepartment_SelectedIndexChanged);
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // dateSendDateTime
            // 
            resources.ApplyResources(this.dateSendDateTime, "dateSendDateTime");
            this.dateSendDateTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateSendDateTime.Name = "dateSendDateTime";
            this.dateSendDateTime.Value = new System.DateTime(2018, 11, 10, 10, 54, 0, 0);
            // 
            // txtCheckDoctor
            // 
            resources.ApplyResources(this.txtCheckDoctor, "txtCheckDoctor");
            this.txtCheckDoctor.Name = "txtCheckDoctor";
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // txtInspectDoctor
            // 
            resources.ApplyResources(this.txtInspectDoctor, "txtInspectDoctor");
            this.txtInspectDoctor.Name = "txtInspectDoctor";
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // cmbSex
            // 
            resources.ApplyResources(this.cmbSex, "cmbSex");
            this.cmbSex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSex.FormattingEnabled = true;
            this.cmbSex.Items.AddRange(new object[] {
            resources.GetString("cmbSex.Items"),
            resources.GetString("cmbSex.Items1")});
            this.cmbSex.Name = "cmbSex";
            // 
            // txtDiagnosis
            // 
            resources.ApplyResources(this.txtDiagnosis, "txtDiagnosis");
            this.txtDiagnosis.Name = "txtDiagnosis";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // txtMedicaRecordNo
            // 
            resources.ApplyResources(this.txtMedicaRecordNo, "txtMedicaRecordNo");
            this.txtMedicaRecordNo.Name = "txtMedicaRecordNo";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // txtBedNo
            // 
            resources.ApplyResources(this.txtBedNo, "txtBedNo");
            this.txtBedNo.Name = "txtBedNo";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // txtAge
            // 
            resources.ApplyResources(this.txtAge, "txtAge");
            this.txtAge.IsNull = false;
            this.txtAge.MaxValue = new decimal(new int[] {
            150,
            0,
            0,
            0});
            this.txtAge.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.txtAge.Name = "txtAge";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // txtPatientName
            // 
            resources.ApplyResources(this.txtPatientName, "txtPatientName");
            this.txtPatientName.Name = "txtPatientName";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // txtClinicNo
            // 
            resources.ApplyResources(this.txtClinicNo, "txtClinicNo");
            this.txtClinicNo.Name = "txtClinicNo";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // fbtnOK
            // 
            resources.ApplyResources(this.fbtnOK, "fbtnOK");
            this.fbtnOK.BackColor = System.Drawing.Color.Transparent;
            this.fbtnOK.EnabledSet = true;
            this.fbtnOK.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnOK.FlatAppearance.BorderSize = 0;
            this.fbtnOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnOK.Name = "fbtnOK";
            this.fbtnOK.UseVisualStyleBackColor = false;
            this.fbtnOK.Click += new System.EventHandler(this.fbtnOK_Click);
            // 
            // btnLis
            // 
            resources.ApplyResources(this.btnLis, "btnLis");
            this.btnLis.BackColor = System.Drawing.Color.Transparent;
            this.btnLis.EnabledSet = true;
            this.btnLis.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnLis.FlatAppearance.BorderSize = 0;
            this.btnLis.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnLis.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnLis.Name = "btnLis";
            this.btnLis.UseVisualStyleBackColor = false;
            this.btnLis.Click += new System.EventHandler(this.btnLis_Click);
            // 
            // frmPatientInfo
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.btnLis);
            this.Controls.Add(this.fbtnOK);
            this.Controls.Add(this.groupBox1);
            this.Name = "frmPatientInfo";
            this.Load += new System.EventHandler(this.frmPatientInfo_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private CustomControl.userTextBoxBase txtClinicNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private CustomControl.userTextBoxBase txtPatientName;
        private System.Windows.Forms.Label label2;
        private CustomControl.userTextBoxBase txtBedNo;
        private System.Windows.Forms.Label label9;
        private CustomControl.userTextBoxBase txtWard;
        private System.Windows.Forms.Label label4;
        private CustomControl.userTextBoxBase txtInpatientArea;
        private System.Windows.Forms.Label label5;
        private CustomControl.userNumTextBox txtAge;
        private System.Windows.Forms.Label label6;
        private CustomControl.FunctionButton fbtnOK;
        private System.Windows.Forms.ComboBox cmbSex;
        private CustomControl.FunctionButton btnLis;
        private System.Windows.Forms.DateTimePicker dateSendDateTime;
        private CustomControl.userTextBoxBase txtCheckDoctor;
        private System.Windows.Forms.Label label14;
        private CustomControl.userTextBoxBase txtInspectDoctor;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label15;
        private CustomControl.userTextBoxBase txtDiagnosis;
        private System.Windows.Forms.Label label7;
        private CustomControl.userTextBoxBase txtMedicaRecordNo;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmbSendDoctor;
        private System.Windows.Forms.ComboBox cmbDepartment;
    }
}