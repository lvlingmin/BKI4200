using Common;
using Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows.Forms;

namespace BioBaseCLIA
{
    public  partial class frmMessageShow : frmSmallParent
    {
        delegate void SetTextCallBack(string text);//修改控件属性块 20180524 zlx add
        public frmMessageShow()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            
            this.Close();
        }
        /// <summary>
        /// 自定义Dialog的Show()方法
        /// </summary>
        /// <param name="title">Dialog标题</param>
        /// <param name="content">内容</param>
        /// <returns>指示标志符以指示对话框的返回值</returns>
        public DialogResult MessageShow(string title, string content)
        {
            //20180524 zlx mod
            //this.Text = title;
            //this.lblMessage.Text = content;
            string culture = OperateIniFile.ReadInIPara("CultureInfo", "Culture");
            int maxllenth = 0;
            string newcontent = "";
            if (this.IsHandleCreated)
            {
                this.Invoke(new SetTextCallBack(SetText), content);
                maxllenth = content.Length;
            }
            else
            {

                if (culture == "en")
                {
                    maxllenth = 125;
                    if (content.Length > maxllenth)
                    {
                        int beginPos = 0;
                        while (beginPos < content.Length)
                        {
                            int endpos = beginPos + maxllenth;
                            if (endpos > content.Length)
                            {
                                endpos = content.Length;
                                newcontent = newcontent + content.Substring(beginPos, endpos - beginPos) + "\r";
                            }
                            else
                            {
                                string str = content.Substring(beginPos, endpos - beginPos);
                                endpos = str.LastIndexOf(' ') + beginPos;
                                newcontent = newcontent + content.Substring(beginPos, endpos - beginPos) + "\r";

                            }
                            beginPos = endpos;
                        }
                    }
                    else
                    {
                        newcontent = content;
                        maxllenth = content.Length;
                    }
                }
                if (culture == "zh-CN")
                {
                    maxllenth = 50;
                    if (content.Length > maxllenth)
                    {
                        int beginPos = 0;
                        while (beginPos < content.Length)
                        {
                            int endpos = beginPos + maxllenth;
                            if (endpos > content.Length)
                                endpos = content.Length;
                            newcontent = newcontent + content.Substring(beginPos, endpos - beginPos) + "\r";
                            beginPos = endpos;
                        }
                    }
                    else
                    {
                        newcontent = content;
                        maxllenth = content.Length;
                    }
                    maxllenth = maxllenth * 2;
                }

                this.Text = title;
                this.lblMessage.Text = newcontent;
                this.Width = 6 * maxllenth + 80;
            }
            //根据提示信息长短确定弹窗大小。
            //this.Width = 8 * maxllenth+80;
            //this.Height = ;
            this.btnOK.Left = this.Width / 2 - 37;//设置弹窗中按钮的位置
            //2018-11-23 zlx add
            if (this.Text.Contains(getString("keywordText.Warning1")) || this.Text.Contains(getString("keywordText.Warning2")))//lyq
                StartKiller();
            DialogResult dr = this.ShowDialog();//增加DialogResult返回值
            return dr;
        }
        public void MessageWrite(string title, string content)
        {
            Invoke(new SetTextCallBack(SetText), content);
        }
        /// <summary>
        /// 更改控件信息 20180524 zlx add
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        private void SetText(string content)
        {
            this.lblMessage.Text = content;
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
        //public DialogResult MessageConfirm(string title, string content)
        //{
        //    this.Text = title;
        //    this.lblMessage.Text = content;
        //    this.Width = 12 * content.Length + 80;//根据提示信息长短确定弹窗大小。
        //    //this.Height = ;
        //    this.btnOK.Left = this.Width / 2 - 37;//设置弹窗中按钮的位置
        //    return MessageBox.Show(MessageBoxButtons.OKCancel, MessageBoxIcon.Question); ;
        //}

        internal System.Windows.Forms.DialogResult MessageShow(string p, string p_2, MessageBoxButtons messageBoxButtons, MessageBoxIcon messageBoxIcon)
        {
            throw new NotImplementedException();
        }

        internal void MessageShow(string p)
        {
            throw new NotImplementedException();
        }
        private string getString(string key)
        {
            ResourceManager resManager = new ResourceManager(typeof(frmMessageShow));
            return resManager.GetString(key).Replace(@"\n", "\n").Replace(@"\t", "\t");
        }
    }
}
