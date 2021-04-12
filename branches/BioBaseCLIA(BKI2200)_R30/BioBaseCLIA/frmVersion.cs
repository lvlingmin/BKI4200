using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BioBaseCLIA
{
    public partial class frmVersion : frmSmallParent 
    {
        //AssemblyInformation AssemblyInfor = new AssemblyInformation();
        public frmVersion()
        {
            this.ControlBox = false;
            InitializeComponent();
            StartKiller();
            //label5.Text = "CL-BKI2200-V" + AssemblyInfor.AssemblyVersion.Substring(0,5);
            //label8.Text = AssemblyInfor.AssemblyCompany;
        }
        

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void StartKiller()
        {
            Timer timer = new Timer();
            timer.Interval = 5000; //3秒启动 
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            btnOK_Click(null, null);
            //停止Timer 
            ((Timer)sender).Stop();
        }
    }
}
