using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using BioBaseCLIA.Run;

namespace BioBaseCLIA
{
    /// <summary>
    /// 功能简介：全自动化学发光小窗体界面父类窗体。
    /// 完成日期：2017.07.19
    /// 编写人：刘亚男
    /// 版本：1.0
    /// </summary>
    public partial class frmSmallParent : Form
    {
        //添加静态变量用于传值
        public static string concCode = "";
        public static string concValueCode = "";
        public static string testProCode = "";
        public frmSmallParent()
        {
            InitializeComponent();
        }
    }
}
