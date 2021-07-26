using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Reflection;

namespace BioBaseCLIA.CustomControl
{
    public class userTextBoxBase : TextBox
    {
        #region 变量
        /// <summary>
        ///  释 放一个键
        /// </summary>
        const int WM_CHAR = 0x0102;
        #endregion
        public userTextBoxBase()
        {
            InitializeComponent();
        }

        

        #region 事件
        //重写enable事件
        protected override void OnEnabledChanged(EventArgs e)
        {
            if (Enabled == false)
            {
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

            }
            else
            {
                SetStyle(ControlStyles.UserPaint, false);
            }
            base.OnEnabledChanged(e);
        }
        //重写OnPaint   
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            if (Enabled == false)
            {
                StringFormat strF = new StringFormat();
                strF.Alignment = StringAlignment.Near;
                strF.LineAlignment = StringAlignment.Center;
                pe.Graphics.DrawString(this.Text, this.Font, Brushes.Black, this.ClientRectangle, strF);
            }
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_CHAR)
            {
                char c = (char)m.WParam;
                if (c == 12288)
                {
                    c = (char)32;
                }
                else if (c > 65280 && c < 65375)
                    c = (char)(c - 65248);
                m.WParam = new IntPtr((int)c);
                base.WndProc(ref m);
            }
            else
            {
                base.WndProc(ref m);
            }
        }
        #endregion

      

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }      
       
    }
    
}
