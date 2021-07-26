using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common;

namespace BioBaseCLIA.Run
{
    public partial class frmWarn : frmSmallParent
    {
        /// <summary>
        /// 报警信息
        /// </summary>
        public string WarnInfo { get; set; }
        /// <summary>
        /// 按钮显示忽略还是停止，忽略为1；停止为0
        /// </summary>
        public int ShowFlag { get; set; }
        /// <summary>
        /// 等待时间
        /// </summary>
        public int WaitSeconds ;

        /// <summary>
        /// 点击暂停事件
        /// </summary>
        public static event Action btnPauseClick;

        /// <summary>
        /// 点击继续事件
        /// </summary>
        public static event Action btnGoonClick;

        public static bool warnFlag = true;
        public frmWarn()
        {
            InitializeComponent();
        }

        private void frmWarn_Load(object sender, EventArgs e)
        {
            warnFlag = true;
            WaitSeconds = int.Parse(OperateIniFile.ReadInIPara("Time", "WarnWaitSeconds"));
            lblWarnInfo.Text = WarnInfo;
            if (ShowFlag == 0)
            {
                fbtnStopIgnore.Text = "停止";
            }
            else
            {
                fbtnStopIgnore.Text = "忽略";
            }
            TimeSpan sp = TimeSpan.FromSeconds(WaitSeconds);
            lblTime.Text = string.Format("{0}分{1}秒" , sp.Minutes, sp.Seconds);
            timerCount.Enabled = true;
            timerCount.Start();
        }

        private void timerCount_Tick(object sender, EventArgs e)
        {
            WaitSeconds--;
            TimeSpan sp = TimeSpan.FromSeconds(WaitSeconds);
            lblTime.Text = string.Format("{0}分{1}秒", sp.Minutes, sp.Seconds);
            if (WaitSeconds == 0)
            {

                //取消报警
                if (warnFlag)
                {
                    warnFlag = false;
                }
                if (ShowFlag == 0)
                {
                    //停止
                    DialogResult = DialogResult.Abort;
                }
                else
                {
                    //忽略
                    DialogResult = DialogResult.Ignore;
                }
                Close();
            }
        }

        private void fbtnStopIgnore_Click(object sender, EventArgs e)
        {
            //取消报警
            if (warnFlag)
            {
                warnFlag = false;
            }
            timerCount.Stop();
            if (fbtnStopIgnore.Text == "停止")
            {
                DialogResult = DialogResult.Abort;
            }
            else 
            {
                DialogResult = DialogResult.Ignore;
                if (waitForFlag && btnGoonClick != null)
                {
                    btnGoonClick();
                    waitForFlag = false;
                }
               
            
            }
            Close();
        }

        private void fbtnRetry_Click(object sender, EventArgs e)
        {
            timerCount.Stop();
            //取消报警
            if (warnFlag)
            {
                warnFlag = false;
            }
            DialogResult = DialogResult.Retry;
            if (waitForFlag && btnGoonClick != null)
            {
                btnGoonClick();
                waitForFlag = false;
            }         
            Close();
        }

      public static  bool waitForFlag = false;
        private void fbtnWait_Click(object sender, EventArgs e)
        {
            timerCount.Stop();
            waitForFlag = true;
            //取消报警
            if (warnFlag)
            {
                warnFlag = false;
            }
            if (btnPauseClick != null)
            {
                btnPauseClick();
            }
           
        }

        private void frmWarn_FormClosed(object sender, FormClosedEventArgs e)
        {
            //取消报警
            if (warnFlag)
            {
                warnFlag = false;
            }
        }
    }
}
