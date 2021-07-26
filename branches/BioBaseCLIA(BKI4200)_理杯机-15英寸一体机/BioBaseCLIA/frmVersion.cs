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
            //label8.Text = AssemblyInfor.AssemblyCompany;
        }
        

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
