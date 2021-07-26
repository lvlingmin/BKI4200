using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Dialogs
{
    public class messageDialog
    {
        public static string applicationName = "全自动化学发光控制软件";
        public DialogResult Confirm(string str)
        {
            return MessageBox.Show(str, applicationName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
        }
        public void erroring(string str)
        {
            MessageBox.Show(str, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }
        public void warning(string str)
        {
            MessageBox.Show(str, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
        }
        public void show(string str)
        {
            MessageBox.Show(str, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public DialogResult DefaultCancle(string str)
        {
            return MessageBox.Show(str, applicationName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        }
        public void show1(string str)
        {
            MessageBox.Show(str, applicationName);
        }
    }
}
