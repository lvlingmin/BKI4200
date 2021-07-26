using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BioBaseCLIA.CustomControl
{
    public partial class defineButton : Button

    {
        public defineButton()
        {
            InitializeComponent();
        }

        public defineButton(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
        protected override bool ShowFocusCues
        {
            get
            {
                return false;
            }
        }

    }
}
