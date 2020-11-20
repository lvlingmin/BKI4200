namespace BioBaseCLIA.SysMaintenance
{
    partial class frmInstruMaintenance
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInstruMaintenance));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rdbDaily = new System.Windows.Forms.RadioButton();
            this.fbtnStop = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnStart = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.rdbtnCustom = new System.Windows.Forms.RadioButton();
            this.rdbtnGeneral = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbSubstrate = new System.Windows.Forms.ComboBox();
            this.cmbSubPipeCH = new System.Windows.Forms.ComboBox();
            this.lblSubPipe2 = new System.Windows.Forms.Label();
            this.chbClearReactTube = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSubTest = new System.Windows.Forms.TextBox();
            this.txtSubPipeline = new BioBaseCLIA.CustomControl.userNumTextBox();
            this.txtPMT = new System.Windows.Forms.TextBox();
            this.txtWashPipeline = new BioBaseCLIA.CustomControl.userNumTextBox();
            this.txtSamplePipeline = new BioBaseCLIA.CustomControl.userNumTextBox();
            this.chbSubstrateTest = new System.Windows.Forms.CheckBox();
            this.chbSubstrate = new System.Windows.Forms.CheckBox();
            this.chbPmt = new System.Windows.Forms.CheckBox();
            this.chbWashPipeline = new System.Windows.Forms.CheckBox();
            this.chbSamplePipeline = new System.Windows.Forms.CheckBox();
            this.chbClearWashTube = new System.Windows.Forms.CheckBox();
            this.chbInit = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.functionButton1 = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnGroupTest = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnReturn = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnInstruDiagnost = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnInstruMaintenance = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.dfInitializers = new BioBaseCLIA.CustomControl.definePanal(this.components);
            this.pbinitializers = new System.Windows.Forms.ProgressBar();
            this.lainitializers = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.dfInitializers.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtInfo);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // txtInfo
            // 
            resources.ApplyResources(this.txtInfo, "txtInfo");
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ReadOnly = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rdbDaily);
            this.groupBox2.Controls.Add(this.fbtnStop);
            this.groupBox2.Controls.Add(this.fbtnStart);
            this.groupBox2.Controls.Add(this.rdbtnCustom);
            this.groupBox2.Controls.Add(this.rdbtnGeneral);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // rdbDaily
            // 
            resources.ApplyResources(this.rdbDaily, "rdbDaily");
            this.rdbDaily.Name = "rdbDaily";
            this.rdbDaily.TabStop = true;
            this.rdbDaily.UseVisualStyleBackColor = true;
            this.rdbDaily.CheckedChanged += new System.EventHandler(this.RdbDaily_CheckedChanged);
            // 
            // fbtnStop
            // 
            this.fbtnStop.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnStop, "fbtnStop");
            this.fbtnStop.EnabledSet = true;
            this.fbtnStop.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnStop.FlatAppearance.BorderSize = 0;
            this.fbtnStop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnStop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnStop.Name = "fbtnStop";
            this.fbtnStop.UseVisualStyleBackColor = false;
            this.fbtnStop.Click += new System.EventHandler(this.fbtnStop_Click);
            // 
            // fbtnStart
            // 
            this.fbtnStart.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnStart, "fbtnStart");
            this.fbtnStart.EnabledSet = true;
            this.fbtnStart.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnStart.FlatAppearance.BorderSize = 0;
            this.fbtnStart.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnStart.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnStart.Name = "fbtnStart";
            this.fbtnStart.UseVisualStyleBackColor = false;
            this.fbtnStart.Click += new System.EventHandler(this.fbtnStart_Click);
            // 
            // rdbtnCustom
            // 
            resources.ApplyResources(this.rdbtnCustom, "rdbtnCustom");
            this.rdbtnCustom.Name = "rdbtnCustom";
            this.rdbtnCustom.TabStop = true;
            this.rdbtnCustom.UseVisualStyleBackColor = true;
            this.rdbtnCustom.CheckedChanged += new System.EventHandler(this.rdbtnGeneral_CheckedChanged);
            // 
            // rdbtnGeneral
            // 
            resources.ApplyResources(this.rdbtnGeneral, "rdbtnGeneral");
            this.rdbtnGeneral.Name = "rdbtnGeneral";
            this.rdbtnGeneral.TabStop = true;
            this.rdbtnGeneral.UseVisualStyleBackColor = true;
            this.rdbtnGeneral.CheckedChanged += new System.EventHandler(this.rdbtnGeneral_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cmbSubstrate);
            this.groupBox1.Controls.Add(this.cmbSubPipeCH);
            this.groupBox1.Controls.Add(this.lblSubPipe2);
            this.groupBox1.Controls.Add(this.chbClearReactTube);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtSubTest);
            this.groupBox1.Controls.Add(this.txtSubPipeline);
            this.groupBox1.Controls.Add(this.txtPMT);
            this.groupBox1.Controls.Add(this.txtWashPipeline);
            this.groupBox1.Controls.Add(this.txtSamplePipeline);
            this.groupBox1.Controls.Add(this.chbSubstrateTest);
            this.groupBox1.Controls.Add(this.chbSubstrate);
            this.groupBox1.Controls.Add(this.chbPmt);
            this.groupBox1.Controls.Add(this.chbWashPipeline);
            this.groupBox1.Controls.Add(this.chbSamplePipeline);
            this.groupBox1.Controls.Add(this.chbClearWashTube);
            this.groupBox1.Controls.Add(this.chbInit);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // cmbSubstrate
            // 
            this.cmbSubstrate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cmbSubstrate, "cmbSubstrate");
            this.cmbSubstrate.FormattingEnabled = true;
            this.cmbSubstrate.Items.AddRange(new object[] {
            resources.GetString("cmbSubstrate.Items")});
            this.cmbSubstrate.Name = "cmbSubstrate";
            // 
            // cmbSubPipeCH
            // 
            this.cmbSubPipeCH.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cmbSubPipeCH, "cmbSubPipeCH");
            this.cmbSubPipeCH.FormattingEnabled = true;
            this.cmbSubPipeCH.Items.AddRange(new object[] {
            resources.GetString("cmbSubPipeCH.Items"),
            resources.GetString("cmbSubPipeCH.Items1")});
            this.cmbSubPipeCH.Name = "cmbSubPipeCH";
            // 
            // lblSubPipe2
            // 
            resources.ApplyResources(this.lblSubPipe2, "lblSubPipe2");
            this.lblSubPipe2.Name = "lblSubPipe2";
            // 
            // chbClearReactTube
            // 
            resources.ApplyResources(this.chbClearReactTube, "chbClearReactTube");
            this.chbClearReactTube.Name = "chbClearReactTube";
            this.chbClearReactTube.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txtSubTest
            // 
            resources.ApplyResources(this.txtSubTest, "txtSubTest");
            this.txtSubTest.Name = "txtSubTest";
            // 
            // txtSubPipeline
            // 
            this.txtSubPipeline.IsNull = false;
            resources.ApplyResources(this.txtSubPipeline, "txtSubPipeline");
            this.txtSubPipeline.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txtSubPipeline.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtSubPipeline.Name = "txtSubPipeline";
            // 
            // txtPMT
            // 
            resources.ApplyResources(this.txtPMT, "txtPMT");
            this.txtPMT.Name = "txtPMT";
            // 
            // txtWashPipeline
            // 
            this.txtWashPipeline.IsNull = false;
            resources.ApplyResources(this.txtWashPipeline, "txtWashPipeline");
            this.txtWashPipeline.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txtWashPipeline.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtWashPipeline.Name = "txtWashPipeline";
            // 
            // txtSamplePipeline
            // 
            this.txtSamplePipeline.IsNull = false;
            resources.ApplyResources(this.txtSamplePipeline, "txtSamplePipeline");
            this.txtSamplePipeline.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txtSamplePipeline.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtSamplePipeline.Name = "txtSamplePipeline";
            // 
            // chbSubstrateTest
            // 
            resources.ApplyResources(this.chbSubstrateTest, "chbSubstrateTest");
            this.chbSubstrateTest.Name = "chbSubstrateTest";
            this.chbSubstrateTest.UseVisualStyleBackColor = true;
            // 
            // chbSubstrate
            // 
            resources.ApplyResources(this.chbSubstrate, "chbSubstrate");
            this.chbSubstrate.Name = "chbSubstrate";
            this.chbSubstrate.UseVisualStyleBackColor = true;
            // 
            // chbPmt
            // 
            resources.ApplyResources(this.chbPmt, "chbPmt");
            this.chbPmt.Name = "chbPmt";
            this.chbPmt.UseVisualStyleBackColor = true;
            // 
            // chbWashPipeline
            // 
            resources.ApplyResources(this.chbWashPipeline, "chbWashPipeline");
            this.chbWashPipeline.Name = "chbWashPipeline";
            this.chbWashPipeline.UseVisualStyleBackColor = true;
            // 
            // chbSamplePipeline
            // 
            resources.ApplyResources(this.chbSamplePipeline, "chbSamplePipeline");
            this.chbSamplePipeline.Name = "chbSamplePipeline";
            this.chbSamplePipeline.UseVisualStyleBackColor = true;
            // 
            // chbClearWashTube
            // 
            resources.ApplyResources(this.chbClearWashTube, "chbClearWashTube");
            this.chbClearWashTube.Name = "chbClearWashTube";
            this.chbClearWashTube.UseVisualStyleBackColor = true;
            // 
            // chbInit
            // 
            resources.ApplyResources(this.chbInit, "chbInit");
            this.chbInit.Checked = true;
            this.chbInit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbInit.Name = "chbInit";
            this.chbInit.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.functionButton1);
            this.panel1.Controls.Add(this.fbtnGroupTest);
            this.panel1.Controls.Add(this.fbtnReturn);
            this.panel1.Controls.Add(this.fbtnInstruDiagnost);
            this.panel1.Controls.Add(this.fbtnInstruMaintenance);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
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
            // fbtnGroupTest
            // 
            this.fbtnGroupTest.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnGroupTest, "fbtnGroupTest");
            this.fbtnGroupTest.EnabledSet = true;
            this.fbtnGroupTest.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnGroupTest.FlatAppearance.BorderSize = 0;
            this.fbtnGroupTest.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnGroupTest.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnGroupTest.Name = "fbtnGroupTest";
            this.fbtnGroupTest.UseVisualStyleBackColor = false;
            this.fbtnGroupTest.Click += new System.EventHandler(this.fbtnGroupTest_Click);
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
            // fbtnInstruDiagnost
            // 
            this.fbtnInstruDiagnost.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnInstruDiagnost, "fbtnInstruDiagnost");
            this.fbtnInstruDiagnost.EnabledSet = true;
            this.fbtnInstruDiagnost.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnInstruDiagnost.FlatAppearance.BorderSize = 0;
            this.fbtnInstruDiagnost.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnInstruDiagnost.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnInstruDiagnost.Name = "fbtnInstruDiagnost";
            this.fbtnInstruDiagnost.UseVisualStyleBackColor = false;
            this.fbtnInstruDiagnost.Click += new System.EventHandler(this.fbtnInstruDiagnost_Click);
            // 
            // fbtnInstruMaintenance
            // 
            this.fbtnInstruMaintenance.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.fbtnInstruMaintenance, "fbtnInstruMaintenance");
            this.fbtnInstruMaintenance.EnabledSet = true;
            this.fbtnInstruMaintenance.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnInstruMaintenance.FlatAppearance.BorderSize = 0;
            this.fbtnInstruMaintenance.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnInstruMaintenance.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnInstruMaintenance.Name = "fbtnInstruMaintenance";
            this.fbtnInstruMaintenance.UseVisualStyleBackColor = false;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // dfInitializers
            // 
            this.dfInitializers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dfInitializers.Controls.Add(this.pbinitializers);
            this.dfInitializers.Controls.Add(this.lainitializers);
            resources.ApplyResources(this.dfInitializers, "dfInitializers");
            this.dfInitializers.Name = "dfInitializers";
            // 
            // pbinitializers
            // 
            resources.ApplyResources(this.pbinitializers, "pbinitializers");
            this.pbinitializers.Name = "pbinitializers";
            // 
            // lainitializers
            // 
            resources.ApplyResources(this.lainitializers, "lainitializers");
            this.lainitializers.Name = "lainitializers";
            // 
            // frmInstruMaintenance
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.dfInitializers);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.panel1);
            this.Name = "frmInstruMaintenance";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmInstruMaintenance_FormClosed);
            this.Load += new System.EventHandler(this.frmInstruMaintenance_Load);
            this.SizeChanged += new System.EventHandler(this.frmInstruMaintenance_SizeChanged);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.dfInitializers.ResumeLayout(false);
            this.dfInitializers.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private CustomControl.FunctionButton fbtnReturn;
        private CustomControl.FunctionButton fbtnInstruDiagnost;
        private CustomControl.FunctionButton fbtnInstruMaintenance;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rdbtnCustom;
        private System.Windows.Forms.RadioButton rdbtnGeneral;
        private CustomControl.FunctionButton fbtnStart;
        private CustomControl.FunctionButton fbtnStop;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.CheckBox chbSubstrateTest;
        private System.Windows.Forms.CheckBox chbSubstrate;
        private System.Windows.Forms.CheckBox chbPmt;
        private System.Windows.Forms.CheckBox chbWashPipeline;
        private System.Windows.Forms.CheckBox chbSamplePipeline;
        private System.Windows.Forms.CheckBox chbClearWashTube;
        private System.Windows.Forms.CheckBox chbInit;
        private System.Windows.Forms.TextBox txtSubTest;
        private CustomControl.userNumTextBox txtSubPipeline;
        private System.Windows.Forms.TextBox txtPMT;
        private CustomControl.userNumTextBox txtWashPipeline;
        private CustomControl.userNumTextBox txtSamplePipeline;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private CustomControl.FunctionButton fbtnGroupTest;
        private System.Windows.Forms.CheckBox chbClearReactTube;
        private System.Windows.Forms.ComboBox cmbSubPipeCH;
        private System.Windows.Forms.Label lblSubPipe2;
        private CustomControl.FunctionButton functionButton1;
        private CustomControl.definePanal dfInitializers;
        private System.Windows.Forms.ProgressBar pbinitializers;
        private System.Windows.Forms.Label lainitializers;
        private System.Windows.Forms.RadioButton rdbDaily;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbSubstrate;
    }
}