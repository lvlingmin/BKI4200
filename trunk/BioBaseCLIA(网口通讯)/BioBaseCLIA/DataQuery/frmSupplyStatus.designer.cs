namespace BioBaseCLIA.DataQuery
{
    partial class frmSupplyStatus
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSupplyStatus));
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.subBottle2 = new SubstrateBottle.SubstrateBottle();
            this.subBottle1 = new SubstrateBottle.SubstrateBottle();
            this.lblSuBottle2 = new System.Windows.Forms.Label();
            this.lblSuBottle1 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.dgvReagentInfo = new System.Windows.Forms.DataGridView();
            this.Postion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RgName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RgCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AllTest = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastTest = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnReturn = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnLoadRackD = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnLoadRackC = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnLoadRackB = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.btnLoadRackA = new BioBaseCLIA.CustomControl.FunctionButton(this.components);
            this.lblRackD = new System.Windows.Forms.Label();
            this.lblRackC = new System.Windows.Forms.Label();
            this.lblRackB = new System.Windows.Forms.Label();
            this.lblRackA = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.MenuSu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ItemUnLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReagentInfo)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.MenuSu.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.subBottle2);
            this.groupBox5.Controls.Add(this.subBottle1);
            this.groupBox5.Controls.Add(this.lblSuBottle2);
            this.groupBox5.Controls.Add(this.lblSuBottle1);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // subBottle2
            // 
            resources.ApplyResources(this.subBottle2, "subBottle2");
            this.subBottle2.BackColor = System.Drawing.Color.LightBlue;
            this.subBottle2.Bottle = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.subBottle2.BottleCap = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.subBottle2.BottleClick = System.Drawing.Color.SlateGray;
            this.subBottle2.CapHeight = 10;
            this.subBottle2.CapWidth = 15;
            this.subBottle2.CapX = 25;
            this.subBottle2.CapY = 10;
            this.subBottle2.Liquid = System.Drawing.Color.RoyalBlue;
            this.subBottle2.Name = "subBottle2";
            this.subBottle2.TestRatio = 0F;
            this.subBottle2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.subBottle2_MouseDown);
            // 
            // subBottle1
            // 
            resources.ApplyResources(this.subBottle1, "subBottle1");
            this.subBottle1.BackColor = System.Drawing.Color.LightBlue;
            this.subBottle1.Bottle = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.subBottle1.BottleCap = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.subBottle1.BottleClick = System.Drawing.Color.SlateGray;
            this.subBottle1.CapHeight = 10;
            this.subBottle1.CapWidth = 15;
            this.subBottle1.CapX = 25;
            this.subBottle1.CapY = 10;
            this.subBottle1.Liquid = System.Drawing.Color.RoyalBlue;
            this.subBottle1.Name = "subBottle1";
            this.subBottle1.TestRatio = 0F;
            this.subBottle1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.subBottle1_MouseDown);
            // 
            // lblSuBottle2
            // 
            resources.ApplyResources(this.lblSuBottle2, "lblSuBottle2");
            this.lblSuBottle2.Name = "lblSuBottle2";
            // 
            // lblSuBottle1
            // 
            resources.ApplyResources(this.lblSuBottle1, "lblSuBottle1");
            this.lblSuBottle1.Name = "lblSuBottle1";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.dgvReagentInfo);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // dgvReagentInfo
            // 
            this.dgvReagentInfo.AllowUserToAddRows = false;
            this.dgvReagentInfo.AllowUserToDeleteRows = false;
            this.dgvReagentInfo.AllowUserToResizeRows = false;
            resources.ApplyResources(this.dgvReagentInfo, "dgvReagentInfo");
            this.dgvReagentInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReagentInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Postion,
            this.RgName,
            this.RgCode,
            this.AllTest,
            this.LastTest});
            this.dgvReagentInfo.Name = "dgvReagentInfo";
            this.dgvReagentInfo.ReadOnly = true;
            this.dgvReagentInfo.RowHeadersVisible = false;
            this.dgvReagentInfo.RowTemplate.Height = 23;
            this.dgvReagentInfo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // Postion
            // 
            this.Postion.DataPropertyName = "Postion";
            resources.ApplyResources(this.Postion, "Postion");
            this.Postion.Name = "Postion";
            this.Postion.ReadOnly = true;
            // 
            // RgName
            // 
            this.RgName.DataPropertyName = "ReagentName";
            resources.ApplyResources(this.RgName, "RgName");
            this.RgName.Name = "RgName";
            this.RgName.ReadOnly = true;
            this.RgName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // RgCode
            // 
            this.RgCode.DataPropertyName = "BarCode";
            resources.ApplyResources(this.RgCode, "RgCode");
            this.RgCode.Name = "RgCode";
            this.RgCode.ReadOnly = true;
            this.RgCode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // AllTest
            // 
            this.AllTest.DataPropertyName = "AllTestNumber";
            resources.ApplyResources(this.AllTest, "AllTest");
            this.AllTest.Name = "AllTest";
            this.AllTest.ReadOnly = true;
            this.AllTest.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // LastTest
            // 
            this.LastTest.DataPropertyName = "leftoverTestR1";
            resources.ApplyResources(this.LastTest, "LastTest");
            this.LastTest.Name = "LastTest";
            this.LastTest.ReadOnly = true;
            this.LastTest.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // btnReturn
            // 
            this.btnReturn.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnReturn, "btnReturn");
            this.btnReturn.EnabledSet = true;
            this.btnReturn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnReturn.FlatAppearance.BorderSize = 0;
            this.btnReturn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnReturn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnReturn.Name = "btnReturn";
            this.btnReturn.UseVisualStyleBackColor = false;
            this.btnReturn.Click += new System.EventHandler(this.btnReturn_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnLoadRackD);
            this.groupBox2.Controls.Add(this.btnLoadRackC);
            this.groupBox2.Controls.Add(this.btnLoadRackB);
            this.groupBox2.Controls.Add(this.btnLoadRackA);
            this.groupBox2.Controls.Add(this.lblRackD);
            this.groupBox2.Controls.Add(this.lblRackC);
            this.groupBox2.Controls.Add(this.lblRackB);
            this.groupBox2.Controls.Add(this.lblRackA);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // btnLoadRackD
            // 
            this.btnLoadRackD.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnLoadRackD, "btnLoadRackD");
            this.btnLoadRackD.EnabledSet = true;
            this.btnLoadRackD.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnLoadRackD.FlatAppearance.BorderSize = 0;
            this.btnLoadRackD.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnLoadRackD.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnLoadRackD.Name = "btnLoadRackD";
            this.btnLoadRackD.UseVisualStyleBackColor = false;
            this.btnLoadRackD.Click += new System.EventHandler(this.btnLoadRackD_Click);
            // 
            // btnLoadRackC
            // 
            this.btnLoadRackC.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnLoadRackC, "btnLoadRackC");
            this.btnLoadRackC.EnabledSet = true;
            this.btnLoadRackC.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnLoadRackC.FlatAppearance.BorderSize = 0;
            this.btnLoadRackC.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnLoadRackC.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnLoadRackC.Name = "btnLoadRackC";
            this.btnLoadRackC.UseVisualStyleBackColor = false;
            this.btnLoadRackC.Click += new System.EventHandler(this.btnLoadRackC_Click);
            // 
            // btnLoadRackB
            // 
            this.btnLoadRackB.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnLoadRackB, "btnLoadRackB");
            this.btnLoadRackB.EnabledSet = true;
            this.btnLoadRackB.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnLoadRackB.FlatAppearance.BorderSize = 0;
            this.btnLoadRackB.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnLoadRackB.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnLoadRackB.Name = "btnLoadRackB";
            this.btnLoadRackB.UseVisualStyleBackColor = false;
            this.btnLoadRackB.Click += new System.EventHandler(this.btnLoadRackB_Click);
            // 
            // btnLoadRackA
            // 
            this.btnLoadRackA.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnLoadRackA, "btnLoadRackA");
            this.btnLoadRackA.EnabledSet = true;
            this.btnLoadRackA.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnLoadRackA.FlatAppearance.BorderSize = 0;
            this.btnLoadRackA.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnLoadRackA.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnLoadRackA.Name = "btnLoadRackA";
            this.btnLoadRackA.UseVisualStyleBackColor = false;
            this.btnLoadRackA.Click += new System.EventHandler(this.btnLoadRackA_Click);
            // 
            // lblRackD
            // 
            resources.ApplyResources(this.lblRackD, "lblRackD");
            this.lblRackD.Name = "lblRackD";
            // 
            // lblRackC
            // 
            resources.ApplyResources(this.lblRackC, "lblRackC");
            this.lblRackC.Name = "lblRackC";
            // 
            // lblRackB
            // 
            resources.ApplyResources(this.lblRackB, "lblRackB");
            this.lblRackB.Name = "lblRackB";
            // 
            // lblRackA
            // 
            resources.ApplyResources(this.lblRackA, "lblRackA");
            this.lblRackA.Name = "lblRackA";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // MenuSu
            // 
            this.MenuSu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ItemUnLoad});
            this.MenuSu.Name = "MenuSu";
            resources.ApplyResources(this.MenuSu, "MenuSu");
            // 
            // ItemUnLoad
            // 
            this.ItemUnLoad.Name = "ItemUnLoad";
            resources.ApplyResources(this.ItemUnLoad, "ItemUnLoad");
            this.ItemUnLoad.Click += new System.EventHandler(this.ItemUnLoad_Click);
            // 
            // frmSupplyStatus
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnReturn);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.label1);
            this.Name = "frmSupplyStatus";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmSupplyStatus_Load);
            this.SizeChanged += new System.EventHandler(this.frmSupplyStatus_SizeChanged);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReagentInfo)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.MenuSu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private CustomControl.FunctionButton btnLoadRackB;
        private CustomControl.FunctionButton btnLoadRackA;
        private System.Windows.Forms.Label lblRackD;
        private System.Windows.Forms.Label lblRackC;
        private System.Windows.Forms.Label lblRackB;
        private System.Windows.Forms.Label lblRackA;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private CustomControl.FunctionButton btnLoadRackD;
        private CustomControl.FunctionButton btnLoadRackC;
        private CustomControl.FunctionButton btnReturn;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.DataGridView dgvReagentInfo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Postion;
        private System.Windows.Forms.DataGridViewTextBoxColumn RgName;
        private System.Windows.Forms.DataGridViewTextBoxColumn RgCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn AllTest;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastTest;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label lblSuBottle1;
        private SubstrateBottle.SubstrateBottle subBottle2;
        private SubstrateBottle.SubstrateBottle subBottle1;
        private System.Windows.Forms.Label lblSuBottle2;
        private System.Windows.Forms.ContextMenuStrip MenuSu;
        private System.Windows.Forms.ToolStripMenuItem ItemUnLoad;
    }
}