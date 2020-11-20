using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BioBaseCLIA.CustomControl
{
    public partial class definePanal : Panel
    {
        public definePanal()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);   //   禁止擦除背景.   
            SetStyle(ControlStyles.DoubleBuffer, true);   //   双缓冲 
        }

        public definePanal(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }
    }
}
