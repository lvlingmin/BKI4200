using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BioBaseCLIA.SysMaintenance
{
    public partial class frmBarrelevel : frmSmallParent
    {
        bool[] LackLq;
        public frmBarrelevel()
        {
            InitializeComponent();
        }

        private void frmBarrelevel_Load(object sender, EventArgs e)
        {

            LackLq = new bool[] { false, false, false, false };
          
            int LackLqValue = 15;
            LackLq = IntToBool(LackLqValue);
            if (LackLq[0])
            {
                lblSystem.Text = "已空";
            }
            else
            {
                lblSystem.Text = "正常";
            }
            if (LackLq[1])
            {
                lblPipeline.Text = "已空";
            }
            else
            {
                lblPipeline.Text = "正常";
            }
            if (LackLq[2])
            {
                lblCold.Text = "已空";
            }
            else
            {
                lblCold.Text = "正常";
            }
            if (LackLq[3])
            {
                lblWaste.Text = "未满";
            }
            else
            {
                lblWaste.Text = "已满";
            }

        }
        
        public bool[] IntToBool(int num)
        {
            bool[] lck = new bool[4] { false, false, false, false };
            if (num / 8 >= 1)
            {
                lck[0] = true;
            }
            if ((num % 8) / 4 >= 1)
            {
                lck[1] = true;
            }
            if (((num % 8) % 4) / 2 >= 1)
            {
                lck[2] = true;
            }
            if ((((num % 8) % 4) % 2) == 1)
            {
                lck[3] = true;
            }
            return lck;
        }

        private void frmBarrelevel_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }
    }
}
