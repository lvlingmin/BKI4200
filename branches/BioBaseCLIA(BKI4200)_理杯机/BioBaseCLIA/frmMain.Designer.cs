namespace BioBaseCLIA
{
    partial class frmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.pnlPublic = new System.Windows.Forms.Panel();
            this.pnlSidebar = new System.Windows.Forms.Panel();
            this.fbtnScalQc = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnSet = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnDataQuery = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnMaintenance = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.fbtnTest = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.timerStatus = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pnlbarDown = new System.Windows.Forms.Panel();
            this.dbtnSound = new BioBaseCLIA.CustomControl.defineButton(this.components);
            this.btnVersion = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.dbtnConnect = new BioBaseCLIA.CustomControl.defineButton(this.components);
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.pnlbarUP = new System.Windows.Forms.Panel();
            this.btnWasteRack = new BioBaseCLIA.CustomControl.defineButton(this.components);
            this.logo = new System.Windows.Forms.PictureBox();
            this.temperatureButton = new BioBaseCLIA.CustomControl.defineButton(this.components);
            this.defineButton1 = new BioBaseCLIA.CustomControl.defineButton(this.components);
            this.defineButton3 = new BioBaseCLIA.CustomControl.defineButton(this.components);
            this.defineButton2 = new BioBaseCLIA.CustomControl.defineButton(this.components);
            this.dbtnLog = new BioBaseCLIA.CustomControl.defineButton(this.components);
            this.dbtnRack = new BioBaseCLIA.CustomControl.defineButton(this.components);
            this.dbtnRegent = new BioBaseCLIA.CustomControl.defineButton(this.components);
            this.dbtnSubstract = new BioBaseCLIA.CustomControl.defineButton(this.components);
            this.dbtnWaste = new BioBaseCLIA.CustomControl.defineButton(this.components);
            this.dbtnWash = new BioBaseCLIA.CustomControl.defineButton(this.components);
            this.dbtnBuffer = new BioBaseCLIA.CustomControl.defineButton(this.components);
            this.timeWarnSound = new System.Windows.Forms.Timer(this.components);
            this.timerConnect = new System.Windows.Forms.Timer(this.components);
            this.pnlPublic.SuspendLayout();
            this.pnlSidebar.SuspendLayout();
            this.pnlbarDown.SuspendLayout();
            this.pnlbarUP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logo)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlPublic
            // 
            resources.ApplyResources(this.pnlPublic, "pnlPublic");
            this.pnlPublic.BackColor = System.Drawing.Color.LightBlue;
            this.pnlPublic.Controls.Add(this.pnlSidebar);
            this.pnlPublic.Name = "pnlPublic";
            this.toolTip1.SetToolTip(this.pnlPublic, resources.GetString("pnlPublic.ToolTip"));
            // 
            // pnlSidebar
            // 
            resources.ApplyResources(this.pnlSidebar, "pnlSidebar");
            this.pnlSidebar.Controls.Add(this.fbtnScalQc);
            this.pnlSidebar.Controls.Add(this.fbtnSet);
            this.pnlSidebar.Controls.Add(this.fbtnDataQuery);
            this.pnlSidebar.Controls.Add(this.fbtnMaintenance);
            this.pnlSidebar.Controls.Add(this.fbtnTest);
            this.pnlSidebar.Name = "pnlSidebar";
            this.toolTip1.SetToolTip(this.pnlSidebar, resources.GetString("pnlSidebar.ToolTip"));
            // 
            // fbtnScalQc
            // 
            resources.ApplyResources(this.fbtnScalQc, "fbtnScalQc");
            this.fbtnScalQc.BackColor = System.Drawing.Color.Transparent;
            this.fbtnScalQc.EnabledSet = true;
            this.fbtnScalQc.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnScalQc.FlatAppearance.BorderSize = 0;
            this.fbtnScalQc.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnScalQc.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnScalQc.Name = "fbtnScalQc";
            this.toolTip1.SetToolTip(this.fbtnScalQc, resources.GetString("fbtnScalQc.ToolTip"));
            this.fbtnScalQc.UseVisualStyleBackColor = false;
            this.fbtnScalQc.Click += new System.EventHandler(this.fbtnScalQc_Click);
            // 
            // fbtnSet
            // 
            resources.ApplyResources(this.fbtnSet, "fbtnSet");
            this.fbtnSet.BackColor = System.Drawing.Color.Transparent;
            this.fbtnSet.EnabledSet = true;
            this.fbtnSet.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnSet.FlatAppearance.BorderSize = 0;
            this.fbtnSet.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnSet.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnSet.Name = "fbtnSet";
            this.toolTip1.SetToolTip(this.fbtnSet, resources.GetString("fbtnSet.ToolTip"));
            this.fbtnSet.UseVisualStyleBackColor = false;
            this.fbtnSet.Click += new System.EventHandler(this.fbtnSet_Click);
            // 
            // fbtnDataQuery
            // 
            resources.ApplyResources(this.fbtnDataQuery, "fbtnDataQuery");
            this.fbtnDataQuery.BackColor = System.Drawing.Color.Transparent;
            this.fbtnDataQuery.EnabledSet = true;
            this.fbtnDataQuery.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnDataQuery.FlatAppearance.BorderSize = 0;
            this.fbtnDataQuery.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnDataQuery.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnDataQuery.Name = "fbtnDataQuery";
            this.toolTip1.SetToolTip(this.fbtnDataQuery, resources.GetString("fbtnDataQuery.ToolTip"));
            this.fbtnDataQuery.UseVisualStyleBackColor = false;
            this.fbtnDataQuery.Click += new System.EventHandler(this.fbtnDataQuery_Click);
            // 
            // fbtnMaintenance
            // 
            resources.ApplyResources(this.fbtnMaintenance, "fbtnMaintenance");
            this.fbtnMaintenance.BackColor = System.Drawing.Color.Transparent;
            this.fbtnMaintenance.EnabledSet = true;
            this.fbtnMaintenance.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnMaintenance.FlatAppearance.BorderSize = 0;
            this.fbtnMaintenance.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnMaintenance.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnMaintenance.Name = "fbtnMaintenance";
            this.toolTip1.SetToolTip(this.fbtnMaintenance, resources.GetString("fbtnMaintenance.ToolTip"));
            this.fbtnMaintenance.UseVisualStyleBackColor = false;
            this.fbtnMaintenance.Click += new System.EventHandler(this.fbtnMaintenance_Click);
            // 
            // fbtnTest
            // 
            resources.ApplyResources(this.fbtnTest, "fbtnTest");
            this.fbtnTest.BackColor = System.Drawing.Color.Transparent;
            this.fbtnTest.EnabledSet = true;
            this.fbtnTest.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.fbtnTest.FlatAppearance.BorderSize = 0;
            this.fbtnTest.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.fbtnTest.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.fbtnTest.Name = "fbtnTest";
            this.toolTip1.SetToolTip(this.fbtnTest, resources.GetString("fbtnTest.ToolTip"));
            this.fbtnTest.UseVisualStyleBackColor = false;
            this.fbtnTest.Click += new System.EventHandler(this.fbtnTest_Click);
            // 
            // timerStatus
            // 
            this.timerStatus.Interval = 20000;
            this.timerStatus.Tick += new System.EventHandler(this.timerStatus_Tick);
            // 
            // toolTip1
            // 
            this.toolTip1.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip1_Popup);
            // 
            // pnlbarDown
            // 
            resources.ApplyResources(this.pnlbarDown, "pnlbarDown");
            this.pnlbarDown.BackColor = System.Drawing.Color.Transparent;
            this.pnlbarDown.BackgroundImage = global::BioBaseCLIA.Properties.Resources.其他界面按钮22;
            this.pnlbarDown.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlbarDown.Controls.Add(this.dbtnSound);
            this.pnlbarDown.Controls.Add(this.btnVersion);
            this.pnlbarDown.Controls.Add(this.label2);
            this.pnlbarDown.Controls.Add(this.dbtnConnect);
            this.pnlbarDown.Controls.Add(this.btnHelp);
            this.pnlbarDown.Controls.Add(this.btnExit);
            this.pnlbarDown.Name = "pnlbarDown";
            this.toolTip1.SetToolTip(this.pnlbarDown, resources.GetString("pnlbarDown.ToolTip"));
            // 
            // dbtnSound
            // 
            resources.ApplyResources(this.dbtnSound, "dbtnSound");
            this.dbtnSound.BackgroundImage = global::BioBaseCLIA.Properties.Resources.声音启用;
            this.dbtnSound.FlatAppearance.BorderSize = 0;
            this.dbtnSound.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.dbtnSound.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.dbtnSound.Name = "dbtnSound";
            this.toolTip1.SetToolTip(this.dbtnSound, resources.GetString("dbtnSound.ToolTip"));
            this.dbtnSound.UseVisualStyleBackColor = true;
            this.dbtnSound.Click += new System.EventHandler(this.dbtnSound_Click);
            // 
            // btnVersion
            // 
            resources.ApplyResources(this.btnVersion, "btnVersion");
            this.btnVersion.BackColor = System.Drawing.Color.Transparent;
            this.btnVersion.BackgroundImage = global::BioBaseCLIA.Properties.Resources.主界面按钮;
            this.btnVersion.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnVersion.FlatAppearance.BorderSize = 0;
            this.btnVersion.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnVersion.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnVersion.Name = "btnVersion";
            this.toolTip1.SetToolTip(this.btnVersion, resources.GetString("btnVersion.ToolTip"));
            this.btnVersion.UseVisualStyleBackColor = false;
            this.btnVersion.Click += new System.EventHandler(this.btnVersion_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.toolTip1.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // dbtnConnect
            // 
            resources.ApplyResources(this.dbtnConnect, "dbtnConnect");
            this.dbtnConnect.BackgroundImage = global::BioBaseCLIA.Properties.Resources.未连接;
            this.dbtnConnect.FlatAppearance.BorderSize = 0;
            this.dbtnConnect.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.dbtnConnect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.dbtnConnect.Name = "dbtnConnect";
            this.toolTip1.SetToolTip(this.dbtnConnect, resources.GetString("dbtnConnect.ToolTip"));
            this.dbtnConnect.UseVisualStyleBackColor = true;
            this.dbtnConnect.EnabledChanged += new System.EventHandler(this.dbtnConnect_EnabledChanged);
            this.dbtnConnect.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dbtnConnect_MouseDown);
            this.dbtnConnect.MouseEnter += new System.EventHandler(this.dbtnConnect_MouseEnter);
            this.dbtnConnect.MouseLeave += new System.EventHandler(this.dbtnConnect_MouseLeave);
            // 
            // btnHelp
            // 
            resources.ApplyResources(this.btnHelp, "btnHelp");
            this.btnHelp.BackColor = System.Drawing.Color.Transparent;
            this.btnHelp.BackgroundImage = global::BioBaseCLIA.Properties.Resources.主界面按钮;
            this.btnHelp.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnHelp.FlatAppearance.BorderSize = 0;
            this.btnHelp.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnHelp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnHelp.Name = "btnHelp";
            this.toolTip1.SetToolTip(this.btnHelp, resources.GetString("btnHelp.ToolTip"));
            this.btnHelp.UseVisualStyleBackColor = false;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            this.btnHelp.MouseEnter += new System.EventHandler(this.dbtnBuffer_MouseEnter);
            this.btnHelp.MouseLeave += new System.EventHandler(this.dbtnBuffer_MouseLeave);
            // 
            // btnExit
            // 
            resources.ApplyResources(this.btnExit, "btnExit");
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.BackgroundImage = global::BioBaseCLIA.Properties.Resources.主界面按钮;
            this.btnExit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnExit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnExit.Name = "btnExit";
            this.toolTip1.SetToolTip(this.btnExit, resources.GetString("btnExit.ToolTip"));
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.button11_Click);
            this.btnExit.MouseEnter += new System.EventHandler(this.dbtnBuffer_MouseEnter);
            this.btnExit.MouseLeave += new System.EventHandler(this.dbtnBuffer_MouseLeave);
            // 
            // pnlbarUP
            // 
            resources.ApplyResources(this.pnlbarUP, "pnlbarUP");
            this.pnlbarUP.BackColor = System.Drawing.Color.Transparent;
            this.pnlbarUP.BackgroundImage = global::BioBaseCLIA.Properties.Resources.其他界面按钮2;
            this.pnlbarUP.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlbarUP.Controls.Add(this.btnWasteRack);
            this.pnlbarUP.Controls.Add(this.logo);
            this.pnlbarUP.Controls.Add(this.temperatureButton);
            this.pnlbarUP.Controls.Add(this.defineButton1);
            this.pnlbarUP.Controls.Add(this.defineButton3);
            this.pnlbarUP.Controls.Add(this.defineButton2);
            this.pnlbarUP.Controls.Add(this.dbtnLog);
            this.pnlbarUP.Controls.Add(this.dbtnRack);
            this.pnlbarUP.Controls.Add(this.dbtnRegent);
            this.pnlbarUP.Controls.Add(this.dbtnSubstract);
            this.pnlbarUP.Controls.Add(this.dbtnWaste);
            this.pnlbarUP.Controls.Add(this.dbtnWash);
            this.pnlbarUP.Controls.Add(this.dbtnBuffer);
            this.pnlbarUP.Name = "pnlbarUP";
            this.toolTip1.SetToolTip(this.pnlbarUP, resources.GetString("pnlbarUP.ToolTip"));
            this.pnlbarUP.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlbarUP_Paint);
            // 
            // btnWasteRack
            // 
            resources.ApplyResources(this.btnWasteRack, "btnWasteRack");
            this.btnWasteRack.BackColor = System.Drawing.Color.Transparent;
            this.btnWasteRack.BackgroundImage = global::BioBaseCLIA.Properties.Resources.WasteRack02;
            this.btnWasteRack.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnWasteRack.FlatAppearance.BorderSize = 0;
            this.btnWasteRack.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.btnWasteRack.FlatAppearance.MouseDownBackColor = System.Drawing.Color.CornflowerBlue;
            this.btnWasteRack.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DodgerBlue;
            this.btnWasteRack.ForeColor = System.Drawing.Color.Transparent;
            this.btnWasteRack.Name = "btnWasteRack";
            this.toolTip1.SetToolTip(this.btnWasteRack, resources.GetString("btnWasteRack.ToolTip"));
            this.btnWasteRack.UseVisualStyleBackColor = false;
            this.btnWasteRack.Click += new System.EventHandler(this.btnWasteRack_Click);
            this.btnWasteRack.MouseEnter += new System.EventHandler(this.dbtnBuffer_MouseEnter);
            this.btnWasteRack.MouseLeave += new System.EventHandler(this.dbtnBuffer_MouseLeave);
            // 
            // logo
            // 
            resources.ApplyResources(this.logo, "logo");
            this.logo.BackColor = System.Drawing.Color.Transparent;
            this.logo.BackgroundImage = global::BioBaseCLIA.Properties.Resources.logo;
            this.logo.Name = "logo";
            this.logo.TabStop = false;
            this.toolTip1.SetToolTip(this.logo, resources.GetString("logo.ToolTip"));
            // 
            // temperatureButton
            // 
            resources.ApplyResources(this.temperatureButton, "temperatureButton");
            this.temperatureButton.BackColor = System.Drawing.Color.Transparent;
            this.temperatureButton.BackgroundImage = global::BioBaseCLIA.Properties.Resources.temperature_1;
            this.temperatureButton.FlatAppearance.BorderSize = 0;
            this.temperatureButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.CornflowerBlue;
            this.temperatureButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DodgerBlue;
            this.temperatureButton.ForeColor = System.Drawing.Color.Transparent;
            this.temperatureButton.Name = "temperatureButton";
            this.toolTip1.SetToolTip(this.temperatureButton, resources.GetString("temperatureButton.ToolTip"));
            this.temperatureButton.UseVisualStyleBackColor = false;
            this.temperatureButton.Click += new System.EventHandler(this.temperatureButton_Click);
            this.temperatureButton.MouseEnter += new System.EventHandler(this.dbtnBuffer_MouseEnter);
            this.temperatureButton.MouseLeave += new System.EventHandler(this.dbtnBuffer_MouseLeave);
            // 
            // defineButton1
            // 
            resources.ApplyResources(this.defineButton1, "defineButton1");
            this.defineButton1.BackColor = System.Drawing.Color.Transparent;
            this.defineButton1.BackgroundImage = global::BioBaseCLIA.Properties.Resources.blue_play_128px_569342_easyicon_net;
            this.defineButton1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.defineButton1.FlatAppearance.BorderSize = 0;
            this.defineButton1.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.defineButton1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.defineButton1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.defineButton1.ForeColor = System.Drawing.Color.Transparent;
            this.defineButton1.Name = "defineButton1";
            this.toolTip1.SetToolTip(this.defineButton1, resources.GetString("defineButton1.ToolTip"));
            this.defineButton1.UseVisualStyleBackColor = false;
            this.defineButton1.Click += new System.EventHandler(this.defineButton1_Click);
            this.defineButton1.MouseEnter += new System.EventHandler(this.dbtnBuffer_MouseEnter);
            this.defineButton1.MouseLeave += new System.EventHandler(this.dbtnBuffer_MouseLeave);
            // 
            // defineButton3
            // 
            resources.ApplyResources(this.defineButton3, "defineButton3");
            this.defineButton3.BackColor = System.Drawing.Color.Transparent;
            this.defineButton3.BackgroundImage = global::BioBaseCLIA.Properties.Resources.blue_stop_play_back_128px_569353_easyicon_net;
            this.defineButton3.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.defineButton3.FlatAppearance.BorderSize = 0;
            this.defineButton3.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.defineButton3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.defineButton3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.defineButton3.ForeColor = System.Drawing.Color.Transparent;
            this.defineButton3.Name = "defineButton3";
            this.toolTip1.SetToolTip(this.defineButton3, resources.GetString("defineButton3.ToolTip"));
            this.defineButton3.UseVisualStyleBackColor = false;
            this.defineButton3.Click += new System.EventHandler(this.defineButton3_Click);
            this.defineButton3.MouseEnter += new System.EventHandler(this.dbtnBuffer_MouseEnter);
            this.defineButton3.MouseLeave += new System.EventHandler(this.dbtnBuffer_MouseLeave);
            // 
            // defineButton2
            // 
            resources.ApplyResources(this.defineButton2, "defineButton2");
            this.defineButton2.BackColor = System.Drawing.Color.Transparent;
            this.defineButton2.BackgroundImage = global::BioBaseCLIA.Properties.Resources.blue_pause_128px_569341_easyicon_net;
            this.defineButton2.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.defineButton2.FlatAppearance.BorderSize = 0;
            this.defineButton2.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.defineButton2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.defineButton2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.defineButton2.ForeColor = System.Drawing.Color.Transparent;
            this.defineButton2.Name = "defineButton2";
            this.toolTip1.SetToolTip(this.defineButton2, resources.GetString("defineButton2.ToolTip"));
            this.defineButton2.UseVisualStyleBackColor = false;
            this.defineButton2.Click += new System.EventHandler(this.defineButton2_Click);
            this.defineButton2.MouseEnter += new System.EventHandler(this.dbtnBuffer_MouseEnter);
            this.defineButton2.MouseLeave += new System.EventHandler(this.dbtnBuffer_MouseLeave);
            // 
            // dbtnLog
            // 
            resources.ApplyResources(this.dbtnLog, "dbtnLog");
            this.dbtnLog.BackColor = System.Drawing.Color.Transparent;
            this.dbtnLog.BackgroundImage = global::BioBaseCLIA.Properties.Resources._33感叹号;
            this.dbtnLog.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.dbtnLog.FlatAppearance.BorderSize = 0;
            this.dbtnLog.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.dbtnLog.FlatAppearance.MouseDownBackColor = System.Drawing.Color.CornflowerBlue;
            this.dbtnLog.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DodgerBlue;
            this.dbtnLog.ForeColor = System.Drawing.Color.Transparent;
            this.dbtnLog.Name = "dbtnLog";
            this.toolTip1.SetToolTip(this.dbtnLog, resources.GetString("dbtnLog.ToolTip"));
            this.dbtnLog.UseVisualStyleBackColor = false;
            this.dbtnLog.Click += new System.EventHandler(this.dbtnLog_Click);
            this.dbtnLog.MouseEnter += new System.EventHandler(this.dbtnBuffer_MouseEnter);
            this.dbtnLog.MouseLeave += new System.EventHandler(this.dbtnBuffer_MouseLeave);
            // 
            // dbtnRack
            // 
            resources.ApplyResources(this.dbtnRack, "dbtnRack");
            this.dbtnRack.BackColor = System.Drawing.Color.Transparent;
            this.dbtnRack.BackgroundImage = global::BioBaseCLIA.Properties.Resources._14;
            this.dbtnRack.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.dbtnRack.FlatAppearance.BorderSize = 0;
            this.dbtnRack.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.dbtnRack.FlatAppearance.MouseDownBackColor = System.Drawing.Color.CornflowerBlue;
            this.dbtnRack.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DodgerBlue;
            this.dbtnRack.ForeColor = System.Drawing.Color.Transparent;
            this.dbtnRack.Name = "dbtnRack";
            this.toolTip1.SetToolTip(this.dbtnRack, resources.GetString("dbtnRack.ToolTip"));
            this.dbtnRack.UseVisualStyleBackColor = false;
            this.dbtnRack.Click += new System.EventHandler(this.dbtnRack_Click);
            this.dbtnRack.MouseEnter += new System.EventHandler(this.dbtnBuffer_MouseEnter);
            this.dbtnRack.MouseLeave += new System.EventHandler(this.dbtnBuffer_MouseLeave);
            // 
            // dbtnRegent
            // 
            resources.ApplyResources(this.dbtnRegent, "dbtnRegent");
            this.dbtnRegent.BackColor = System.Drawing.Color.Transparent;
            this.dbtnRegent.BackgroundImage = global::BioBaseCLIA.Properties.Resources._14__2_;
            this.dbtnRegent.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.dbtnRegent.FlatAppearance.BorderSize = 0;
            this.dbtnRegent.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.dbtnRegent.FlatAppearance.MouseDownBackColor = System.Drawing.Color.CornflowerBlue;
            this.dbtnRegent.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DodgerBlue;
            this.dbtnRegent.ForeColor = System.Drawing.Color.Transparent;
            this.dbtnRegent.Name = "dbtnRegent";
            this.toolTip1.SetToolTip(this.dbtnRegent, resources.GetString("dbtnRegent.ToolTip"));
            this.dbtnRegent.UseVisualStyleBackColor = false;
            this.dbtnRegent.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dbtnSubstract_MouseClick);
            this.dbtnRegent.MouseEnter += new System.EventHandler(this.dbtnBuffer_MouseEnter);
            this.dbtnRegent.MouseLeave += new System.EventHandler(this.dbtnBuffer_MouseLeave);
            // 
            // dbtnSubstract
            // 
            resources.ApplyResources(this.dbtnSubstract, "dbtnSubstract");
            this.dbtnSubstract.BackColor = System.Drawing.Color.Transparent;
            this.dbtnSubstract.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.dbtnSubstract.FlatAppearance.BorderSize = 0;
            this.dbtnSubstract.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.dbtnSubstract.FlatAppearance.MouseDownBackColor = System.Drawing.Color.CornflowerBlue;
            this.dbtnSubstract.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DodgerBlue;
            this.dbtnSubstract.ForeColor = System.Drawing.Color.Transparent;
            this.dbtnSubstract.Name = "dbtnSubstract";
            this.toolTip1.SetToolTip(this.dbtnSubstract, resources.GetString("dbtnSubstract.ToolTip"));
            this.dbtnSubstract.UseVisualStyleBackColor = false;
            this.dbtnSubstract.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dbtnSubstract_MouseClick);
            this.dbtnSubstract.MouseEnter += new System.EventHandler(this.dbtnBuffer_MouseEnter);
            this.dbtnSubstract.MouseLeave += new System.EventHandler(this.dbtnBuffer_MouseLeave);
            // 
            // dbtnWaste
            // 
            resources.ApplyResources(this.dbtnWaste, "dbtnWaste");
            this.dbtnWaste.BackColor = System.Drawing.Color.Transparent;
            this.dbtnWaste.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.dbtnWaste.FlatAppearance.BorderSize = 0;
            this.dbtnWaste.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.dbtnWaste.FlatAppearance.MouseDownBackColor = System.Drawing.Color.CornflowerBlue;
            this.dbtnWaste.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DodgerBlue;
            this.dbtnWaste.ForeColor = System.Drawing.Color.Transparent;
            this.dbtnWaste.Name = "dbtnWaste";
            this.toolTip1.SetToolTip(this.dbtnWaste, resources.GetString("dbtnWaste.ToolTip"));
            this.dbtnWaste.UseVisualStyleBackColor = false;
            this.dbtnWaste.Click += new System.EventHandler(this.dbtnWaste_Click);
            this.dbtnWaste.MouseEnter += new System.EventHandler(this.dbtnBuffer_MouseEnter);
            this.dbtnWaste.MouseLeave += new System.EventHandler(this.dbtnBuffer_MouseLeave);
            // 
            // dbtnWash
            // 
            resources.ApplyResources(this.dbtnWash, "dbtnWash");
            this.dbtnWash.BackColor = System.Drawing.Color.Transparent;
            this.dbtnWash.BackgroundImage = global::BioBaseCLIA.Properties.Resources._8;
            this.dbtnWash.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.dbtnWash.FlatAppearance.BorderSize = 0;
            this.dbtnWash.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.dbtnWash.FlatAppearance.MouseDownBackColor = System.Drawing.Color.CornflowerBlue;
            this.dbtnWash.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DodgerBlue;
            this.dbtnWash.ForeColor = System.Drawing.Color.Transparent;
            this.dbtnWash.Name = "dbtnWash";
            this.toolTip1.SetToolTip(this.dbtnWash, resources.GetString("dbtnWash.ToolTip"));
            this.dbtnWash.UseVisualStyleBackColor = false;
            this.dbtnWash.Click += new System.EventHandler(this.dbtnWash_Click);
            this.dbtnWash.MouseEnter += new System.EventHandler(this.dbtnBuffer_MouseEnter);
            this.dbtnWash.MouseLeave += new System.EventHandler(this.dbtnBuffer_MouseLeave);
            // 
            // dbtnBuffer
            // 
            resources.ApplyResources(this.dbtnBuffer, "dbtnBuffer");
            this.dbtnBuffer.BackColor = System.Drawing.Color.Transparent;
            this.dbtnBuffer.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.dbtnBuffer.FlatAppearance.BorderSize = 0;
            this.dbtnBuffer.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.dbtnBuffer.FlatAppearance.MouseDownBackColor = System.Drawing.Color.CornflowerBlue;
            this.dbtnBuffer.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DodgerBlue;
            this.dbtnBuffer.ForeColor = System.Drawing.Color.Transparent;
            this.dbtnBuffer.Name = "dbtnBuffer";
            this.toolTip1.SetToolTip(this.dbtnBuffer, resources.GetString("dbtnBuffer.ToolTip"));
            this.dbtnBuffer.UseVisualStyleBackColor = false;
            this.dbtnBuffer.Click += new System.EventHandler(this.dbtnBuffer_Click);
            this.dbtnBuffer.MouseEnter += new System.EventHandler(this.dbtnBuffer_MouseEnter);
            this.dbtnBuffer.MouseLeave += new System.EventHandler(this.dbtnBuffer_MouseLeave);
            // 
            // timeWarnSound
            // 
            this.timeWarnSound.Interval = 1000;
            this.timeWarnSound.Tick += new System.EventHandler(this.timeWarnSound_Tick);
            // 
            // timerConnect
            // 
            this.timerConnect.Interval = 3000;
            this.timerConnect.Tick += new System.EventHandler(this.timerConnect_Tick);
            // 
            // frmMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pnlPublic);
            this.Controls.Add(this.pnlbarDown);
            this.Controls.Add(this.pnlbarUP);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.IsMdiContainer = true;
            this.Name = "frmMain";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.pnlPublic.ResumeLayout(false);
            this.pnlSidebar.ResumeLayout(false);
            this.pnlbarDown.ResumeLayout(false);
            this.pnlbarDown.PerformLayout();
            this.pnlbarUP.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.logo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlbarUP;
        private System.Windows.Forms.Button btnExit;
        private CustomControl.defineButton defineButton1;
        private CustomControl.defineButton defineButton2;
        private CustomControl.defineButton dbtnLog;
        private CustomControl.defineButton dbtnRack;
        private CustomControl.defineButton dbtnRegent;
        private CustomControl.defineButton dbtnSubstract;
        private CustomControl.defineButton dbtnWaste;
        private CustomControl.defineButton dbtnWash;
        private CustomControl.defineButton dbtnBuffer;
        private CustomControl.defineButton defineButton3;
        private System.Windows.Forms.Panel pnlbarDown;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Panel pnlPublic;
        private System.Windows.Forms.Panel pnlSidebar;
        private CustomControl.FunctionButton fbtnTest;
        private CustomControl.FunctionButton fbtnSet;
        private CustomControl.FunctionButton fbtnMaintenance;
        private CustomControl.FunctionButton fbtnDataQuery;
        private System.Windows.Forms.Timer timerStatus;
        private System.Windows.Forms.ToolTip toolTip1;
        private CustomControl.FunctionButton fbtnScalQc;
        private CustomControl.defineButton temperatureButton;
        private CustomControl.defineButton dbtnConnect;
        private System.Windows.Forms.PictureBox logo;
        private System.Windows.Forms.Button btnVersion;
        private System.Windows.Forms.Label label2;
        private CustomControl.defineButton dbtnSound;
        private System.Windows.Forms.Timer timeWarnSound;
        private CustomControl.defineButton btnWasteRack;
        private System.Windows.Forms.Timer timerConnect;
    }
}

