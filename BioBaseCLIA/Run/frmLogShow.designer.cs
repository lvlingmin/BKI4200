namespace BioBaseCLIA.Run
{
    partial class frmLogShow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLogShow));
            this.lblDate = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SelectDate = new System.Windows.Forms.DateTimePicker();
            this.onlynoread = new System.Windows.Forms.CheckBox();
            this.CBmodule = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.BTfliter = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.notrepeat = new System.Windows.Forms.CheckBox();
            this.CBDateShow = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.DGVLog = new System.Windows.Forms.DataGridView();
            this.Bit = new System.Windows.Forms.DataGridViewImageColumn();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Text = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Butt = new System.Windows.Forms.DataGridViewLinkColumn();
            this.logOfAlarmBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.fbtnReturn = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.BTrefresh = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGVLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.logOfAlarmBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // lblDate
            // 
            resources.ApplyResources(this.lblDate, "lblDate");
            this.lblDate.Name = "lblDate";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.SelectDate);
            this.groupBox1.Controls.Add(this.onlynoread);
            this.groupBox1.Controls.Add(this.CBmodule);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.BTfliter);
            this.groupBox1.Controls.Add(this.notrepeat);
            this.groupBox1.Controls.Add(this.lblDate);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // SelectDate
            // 
            resources.ApplyResources(this.SelectDate, "SelectDate");
            this.SelectDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.SelectDate.Name = "SelectDate";
            this.SelectDate.ShowCheckBox = true;
            this.SelectDate.ValueChanged += new System.EventHandler(this.SelectedIndexChanged);
            // 
            // onlynoread
            // 
            resources.ApplyResources(this.onlynoread, "onlynoread");
            this.onlynoread.Name = "onlynoread";
            this.onlynoread.UseVisualStyleBackColor = true;
            this.onlynoread.CheckedChanged += new System.EventHandler(this.SelectedIndexChanged);
            // 
            // CBmodule
            // 
            resources.ApplyResources(this.CBmodule, "CBmodule");
            this.CBmodule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CBmodule.FormattingEnabled = true;
            this.CBmodule.Items.AddRange(new object[] {
            resources.GetString("CBmodule.Items"),
            resources.GetString("CBmodule.Items1"),
            resources.GetString("CBmodule.Items2")});
            this.CBmodule.Name = "CBmodule";
            this.CBmodule.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // BTfliter
            // 
            resources.ApplyResources(this.BTfliter, "BTfliter");
            this.BTfliter.BackColor = System.Drawing.Color.Transparent;
            this.BTfliter.EnabledSet = true;
            this.BTfliter.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BTfliter.FlatAppearance.BorderSize = 0;
            this.BTfliter.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.BTfliter.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.BTfliter.Name = "BTfliter";
            this.BTfliter.UseVisualStyleBackColor = false;
            this.BTfliter.Click += new System.EventHandler(this.BTfliter_Click);
            // 
            // notrepeat
            // 
            resources.ApplyResources(this.notrepeat, "notrepeat");
            this.notrepeat.Name = "notrepeat";
            this.notrepeat.UseVisualStyleBackColor = true;
            this.notrepeat.CheckedChanged += new System.EventHandler(this.SelectedIndexChanged);
            // 
            // CBDateShow
            // 
            resources.ApplyResources(this.CBDateShow, "CBDateShow");
            this.CBDateShow.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CBDateShow.FormattingEnabled = true;
            this.CBDateShow.Name = "CBDateShow";
            this.CBDateShow.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.DGVLog);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // DGVLog
            // 
            this.DGVLog.AllowUserToAddRows = false;
            this.DGVLog.AllowUserToDeleteRows = false;
            this.DGVLog.AllowUserToResizeRows = false;
            resources.ApplyResources(this.DGVLog, "DGVLog");
            this.DGVLog.AutoGenerateColumns = false;
            this.DGVLog.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.DGVLog.BackgroundColor = System.Drawing.Color.DarkSeaGreen;
            this.DGVLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DGVLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGVLog.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Bit,
            this.Date,
            this.Text,
            this.Butt});
            this.DGVLog.DataSource = this.logOfAlarmBindingSource;
            this.DGVLog.Name = "DGVLog";
            this.DGVLog.ReadOnly = true;
            this.DGVLog.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.DGVLog.RowTemplate.Height = 23;
            this.DGVLog.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.DGVLog_ColumnWidthChanged);
            this.DGVLog.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.DGVLog_DataBindingComplete);
            this.DGVLog.RowHeightChanged += new System.Windows.Forms.DataGridViewRowEventHandler(this.DGVLog_RowHeightChanged);
            this.DGVLog.Scroll += new System.Windows.Forms.ScrollEventHandler(this.DGVLog_Scroll);
            // 
            // Bit
            // 
            this.Bit.DataPropertyName = "Bit";
            resources.ApplyResources(this.Bit, "Bit");
            this.Bit.Name = "Bit";
            this.Bit.ReadOnly = true;
            // 
            // Date
            // 
            this.Date.DataPropertyName = "Date";
            resources.ApplyResources(this.Date, "Date");
            this.Date.Name = "Date";
            this.Date.ReadOnly = true;
            // 
            // Text
            // 
            this.Text.DataPropertyName = "Text";
            resources.ApplyResources(this.Text, "Text");
            this.Text.Name = "Text";
            this.Text.ReadOnly = true;
            // 
            // Butt
            // 
            this.Butt.DataPropertyName = "Butt";
            resources.ApplyResources(this.Butt, "Butt");
            this.Butt.Name = "Butt";
            this.Butt.ReadOnly = true;
            this.Butt.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Butt.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Butt.Text = "";
            // 
            // logOfAlarmBindingSource
            // 
            this.logOfAlarmBindingSource.DataSource = typeof(BioBaseCLIA.Run.LogOfAlarm);
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
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "wrong.jpg");
            this.imageList1.Images.SetKeyName(1, "warning.jpg");
            this.imageList1.Images.SetKeyName(2, "right.jpg");
            this.imageList1.Images.SetKeyName(3, "spilt.jpg");
            // 
            // BTrefresh
            // 
            this.BTrefresh.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.BTrefresh, "BTrefresh");
            this.BTrefresh.EnabledSet = true;
            this.BTrefresh.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BTrefresh.FlatAppearance.BorderSize = 0;
            this.BTrefresh.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.BTrefresh.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.BTrefresh.Name = "BTrefresh";
            this.BTrefresh.UseVisualStyleBackColor = false;
            this.BTrefresh.Click += new System.EventHandler(this.BTrefresh_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 20000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmLogShow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.BTrefresh);
            this.Controls.Add(this.CBDateShow);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.fbtnReturn);
            this.Controls.Add(this.groupBox2);
            this.Name = "frmLogShow";
            this.TransparencyKey = System.Drawing.Color.White;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmLogShow_Load);
            this.SizeChanged += new System.EventHandler(this.frmLogShow_SizeChanged);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DGVLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.logOfAlarmBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDate;
        private CustomControl.FunctionButton fbtnReturn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox notrepeat;
        private CustomControl.FunctionButton BTfliter;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.DataGridView DGVLog;
        private System.Windows.Forms.BindingSource logOfAlarmBindingSource;
        private System.Windows.Forms.ComboBox CBDateShow;
        private System.Windows.Forms.ComboBox CBmodule;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox onlynoread;
        private CustomControl.FunctionButton BTrefresh;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.DataGridViewImageColumn Bit;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn Text;
        private System.Windows.Forms.DataGridViewLinkColumn Butt;
        private System.Windows.Forms.DateTimePicker SelectDate;
    }
}