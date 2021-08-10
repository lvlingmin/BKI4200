using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using Common;
using Localization;

namespace BioBaseCLIA.InfoSetting
{
    public partial class frmInstrumentPara : frmParent
    {
        frmMessageShow frmMsgShow = new frmMessageShow();
        public frmInstrumentPara()
        {
            InitializeComponent();
        }

        private void frmInstrumentPara_Load(object sender, EventArgs e)
        {
            ReadIniToTxt();
        }
        /// <summary>
        /// 控件状态变化
        /// </summary>
        /// <param name="Flag"></param>
        void ControlIsEnable(bool Flag)
        {
            txtAirVol.Enabled = txtBeadsTime.Enabled = txtErrorReagent.Enabled = txtErrorSubstrate.Enabled = txtErrorTube.Enabled
                = txtFirstCleanTime.Enabled = txtMixNum.Enabled = txtNeedleCleanTime.Enabled = txtReadTime.Enabled = txtReagentTime.Enabled
                = txtRgAbandonVol.Enabled = txtRgBeforeVol.Enabled = txtSampleTime.Enabled = txtSpAbandonVol.Enabled = txtSpBeforeVol.Enabled
                = txtSubstrateTime.Enabled = txtWarnReagent.Enabled = txtWarnSubstrate.Enabled = txtWarnTube.Enabled = txtWarnWaitSeconds.Enabled
                = txtTWYMin.Enabled = txtTWYMax.Enabled = txtTWashMax.Enabled = txtTWashMin.Enabled = txtTSubstrateMin.Enabled = txtTSubstrateMax.Enabled//2018-07-13 zlx add
                = txtTSubstrateMin.Enabled = txtTSubstrateMax.Enabled = txtTQXGLMin.Enabled = txtTQXGLMax.Enabled //2018-07-13 zlx add 
                = txtTDarkMin.Enabled = txtTDarkMax.Enabled //暗室
                = txtWashTime.Enabled = txtDiuNum.Enabled = Flag;
        }

        /// <summary>
        /// 检查是否为空
        /// </summary>
        /// <returns>都不为空返回true</returns>
        private bool checknull()
        {
            if (
            txtAirVol.Text.Trim() == "" ||
            txtBeadsTime.Text.Trim() == "" ||
            txtErrorReagent.Text.Trim() == "" ||
            txtErrorSubstrate.Text.Trim() == "" ||
            txtErrorTube.Text.Trim() == "" ||
            txtFirstCleanTime.Text.Trim() == "" ||
            txtMixNum.Text.Trim() == "" ||
            txtNeedleCleanTime.Text.Trim() == "" ||
            txtReadTime.Text.Trim() == "" ||
            txtReagentTime.Text.Trim() == "" ||
            txtRgAbandonVol.Text.Trim() == "" ||
            txtRgBeforeVol.Text.Trim() == "" ||
            txtSampleTime.Text.Trim() == "" ||
            txtSpAbandonVol.Text.Trim() == "" ||
            txtSpBeforeVol.Text.Trim() == "" ||
            txtWarnReagent.Text.Trim() == "" ||
            txtWarnSubstrate.Text.Trim() == "" ||
            txtWarnTube.Text.Trim() == "" ||
            txtWarnWaitSeconds.Text.Trim() == "" ||
            txtWashTime.Text.Trim() == "" ||
            txtSubstrateTime.Text.Trim() == ""
                )
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 读取配置文件中的数据显示到界面控件中
        /// </summary>
        void ReadIniToTxt()
        {
            txtAirVol.Text = OperateIniFile.ReadInIPara("Vol", "AirVol");
            txtBeadsTime.Text = OperateIniFile.ReadInIPara("Time", "beadTime");
            txtErrorReagent.Text = OperateIniFile.ReadInIPara("Limit", "ErrorReagent");
            txtErrorSubstrate.Text = OperateIniFile.ReadInIPara("Limit", "ErrorSubstrate");
            txtErrorTube.Text = OperateIniFile.ReadInIPara("Limit", "ErrorTube");
            txtFirstCleanTime.Text = OperateIniFile.ReadInIPara("Time", "FirstCleanNeedleTime");
            txtMixNum.Text = OperateIniFile.ReadInIPara("OtherPara", "MixNum");
            txtNeedleCleanTime.Text = OperateIniFile.ReadInIPara("Time", "NeedleWashTime");
            txtReadTime.Text = OperateIniFile.ReadInIPara("Time", "readTime");
            txtReagentTime.Text = OperateIniFile.ReadInIPara("Time", "RegentTime");
            txtRgAbandonVol.Text = OperateIniFile.ReadInIPara("Vol", "RgAbandonVol");
            txtRgBeforeVol.Text = OperateIniFile.ReadInIPara("Vol", "RgBeforeVol");
            txtSampleTime.Text = OperateIniFile.ReadInIPara("Time", "sampleTime");
            txtSpAbandonVol.Text = OperateIniFile.ReadInIPara("Vol", "SpAbandonVol");
            txtSpBeforeVol.Text = OperateIniFile.ReadInIPara("Vol", "SpBeforeVol");
            txtSubstrateTime.Text = OperateIniFile.ReadInIPara("Time", "substrateTime");
            txtWarnReagent.Text = OperateIniFile.ReadInIPara("Limit", "WarnReagent");
            txtWarnSubstrate.Text = OperateIniFile.ReadInIPara("Limit", "WarnSubstrate");
            txtWarnTube.Text = OperateIniFile.ReadInIPara("Limit", "WarnTube");
            txtWarnWaitSeconds.Text = OperateIniFile.ReadInIPara("Time", "WarnWaitSeconds");
            txtWashTime.Text = OperateIniFile.ReadInIPara("Time", "washTime");
            txtDiuNum.Text = OperateIniFile.ReadInIPara("OtherPara", "DiuNum ");
            //2018-07-13
            txtTWYMax.Text = OperateIniFile.ReadInIPara("temperature", "MaxTWY");
            txtTWYMin.Text = OperateIniFile.ReadInIPara("temperature", "MinTWY");
            txtTWashMax.Text = OperateIniFile.ReadInIPara("temperature", "MaxTWash");
            txtTWashMin.Text = OperateIniFile.ReadInIPara("temperature", "MinTWash");
            txtTSubstrateMax.Text = OperateIniFile.ReadInIPara("temperature", "MaxTSubstrate");
            txtTSubstrateMin.Text = OperateIniFile.ReadInIPara("temperature", "MinTSubstrate");
            txtTQXGLMax.Text = OperateIniFile.ReadInIPara("temperature", "MaxTQXGL");
            txtTQXGLMin.Text = OperateIniFile.ReadInIPara("temperature", "MinTQXGL");

            //暗室
            txtTDarkMax.Text = OperateIniFile.ReadInIPara("Darkroom", "MaxTDark");
            txtTDarkMin.Text = OperateIniFile.ReadInIPara("Darkroom", "MinTDark");
        }
        /// <summary>
        /// 界面控件中的数据写入ini文件
        /// </summary>
        void WriteTxtToIni()
        {
            OperateIniFile.WriteIniPara("Vol", "AirVol", txtAirVol.Text);
            OperateIniFile.WriteIniPara("Time", "beadTime", txtBeadsTime.Text);
            OperateIniFile.WriteIniPara("Limit", "ErrorReagent", txtErrorReagent.Text);
            OperateIniFile.WriteIniPara("Limit", "ErrorSubstrate", txtErrorSubstrate.Text);
            OperateIniFile.WriteIniPara("Limit", "ErrorTube", txtErrorTube.Text);
            OperateIniFile.WriteIniPara("Time", "FirstCleanNeedleTime", txtFirstCleanTime.Text);
            OperateIniFile.WriteIniPara("OtherPara", "MixNum", txtMixNum.Text);
            OperateIniFile.WriteIniPara("Time", "NeedleWashTime", txtNeedleCleanTime.Text);
            OperateIniFile.WriteIniPara("Time", "readTime", txtReadTime.Text);
            OperateIniFile.WriteIniPara("Time", "RegentTime", txtReagentTime.Text);
            OperateIniFile.WriteIniPara("Vol", "RgAbandonVol", txtRgAbandonVol.Text);
            OperateIniFile.WriteIniPara("Vol", "RgBeforeVol", txtRgBeforeVol.Text);
            OperateIniFile.WriteIniPara("Time", "sampleTime", txtSampleTime.Text);
            OperateIniFile.WriteIniPara("Vol", "SpAbandonVol", txtSpAbandonVol.Text);
            OperateIniFile.WriteIniPara("Vol", "SpBeforeVol", txtSpBeforeVol.Text);
            OperateIniFile.WriteIniPara("Time", "substrateTime", txtSubstrateTime.Text);
            OperateIniFile.WriteIniPara("Limit", "WarnReagent", txtWarnReagent.Text);
            OperateIniFile.WriteIniPara("Limit", "WarnSubstrate", txtWarnSubstrate.Text);
            OperateIniFile.WriteIniPara("Limit", "WarnTube", txtWarnTube.Text);
            OperateIniFile.WriteIniPara("Time", "WarnWaitSeconds", txtWarnWaitSeconds.Text);
            OperateIniFile.WriteIniPara("Time", "washTime", txtWashTime.Text);

            //2018-07-13
            OperateIniFile.WriteIniPara("temperature", "MaxTWY", txtTWYMax.Text);
            OperateIniFile.WriteIniPara("temperature", "MinTWY", txtTWYMin.Text);
            OperateIniFile.WriteIniPara("temperature", "MaxTWash", txtTWashMax.Text);
            OperateIniFile.WriteIniPara("temperature", "MinTWash", txtTWashMin.Text);
            OperateIniFile.WriteIniPara("temperature", "MaxTSubstrate", txtTSubstrateMax.Text);
            OperateIniFile.WriteIniPara("temperature", "MinTSubstrate", txtTSubstrateMin.Text);
            OperateIniFile.WriteIniPara("temperature", "MaxTQXGL", txtTQXGLMax.Text);
            OperateIniFile.WriteIniPara("temperature", "MinTQXGL", txtTQXGLMin.Text);
            OperateIniFile.WriteIniPara("OtherPara", "DiuNum ", txtDiuNum.Text);

            //暗室
            OperateIniFile.WriteIniPara("Darkroom", "MaxTDark", txtTDarkMax.Text);
            OperateIniFile.WriteIniPara("Darkroom", "MinTDark", txtTDarkMin.Text);
        }
        private void fbtnModify_Click(object sender, EventArgs e)
        {
            ControlIsEnable(true);
            fbtnModify.Enabled = false;
            fbtnCancle.Enabled = btnSave.Enabled = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!checknull())
            {
                frmMsgShow.MessageShow(Getstring("InstrumentParaHead"), Getstring("ParaIsNull"));
                return;
            }
            ControlIsEnable(false);
            fbtnCancle.Enabled = btnSave.Enabled = false;
            fbtnModify.Enabled = true;
            WriteTxtToIni();
            WriteDarkroom();              //将暗室值写入下位机
            frmMsgShow.MessageShow(Getstring("InstrumentParaHead"), Getstring("ParaSaveSucess"));//2018-08-16 zlx mod
        }
        private void fbtnCancle_Click(object sender, EventArgs e)
        {
            ControlIsEnable(false);
            fbtnCancle.Enabled = btnSave.Enabled = false;
            fbtnModify.Enabled = true;
            ReadIniToTxt();
            return;
        }
        private void btnUserInfo_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmUserManage"))
            {
                frmUserManage frmUM = new frmUserManage();
                //this.TopLevel = false;
                frmUM.TopLevel = false;
                frmUM.Parent = this.Parent;
                frmUM.Show();
            }
            else
            {
                frmUserManage frmUM = (frmUserManage)Application.OpenForms["frmUserManage"];
                //frmIM.Activate();
                frmUM.BringToFront();

            }
        }

        private void fbtnReturn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmInstrumentPara_SizeChanged(object sender, EventArgs e)
        {
            formSizeChange(this);
        }

        private void btnProInfo_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmInfo"))
            {
                frmInfo frmif = new frmInfo();
                //this.TopLevel = false;
                frmif.TopLevel = false;
                frmif.Parent = this.Parent;
                frmif.Show();
            }
            else
            {
                frmInfo frmif = (frmInfo)Application.OpenForms["frmInfo"];
                //frmIM.Activate();
                frmif.BringToFront();

            }
        }

        private void fbtnConnetSet_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmNetSet"))
            {
                frmNetSet frmNS = new frmNetSet();
                //this.TopLevel = false;
                frmNS.TopLevel = false;
                frmNS.Parent = this.Parent;
                frmNS.Show();
            }
            else
            {
                frmNetSet frmNS = (frmNetSet)Application.OpenForms["frmNetSet"];
                //frmIM.Activate();
                frmNS.BringToFront();

            }
        }
        private string Getstring(string key)
        {
            ResourceManager resManagerA =
                    new ResourceManager("BioBaseCLIA.InfoSetting.frmInstrumentPara", typeof(frmInstrumentPara).Assembly);
            return resManagerA.GetString(key);
        }
        private void functionButton2_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 向下位机写入暗室高值和低值
        /// </summary>
        void WriteDarkroom()
        {
            //SetCultureInfo();
            int MaxDarkroom = int.Parse(OperateIniFile.ReadInIPara("Darkroom", "MaxTDark"));
            int MinDarkroom = int.Parse(OperateIniFile.ReadInIPara("Darkroom", "MinTDark"));
            //string[] s = "EB 90 F1 02 FD FF FF FF FF 00 00 00 00 00 00 4A".Split(' ');
            //string s = MaxDarkroom.ToString("x2");
            if (MaxDarkroom < 256)
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 0D 01 00 " + MaxDarkroom.ToString("X2")), 5);
            else
            {
                string s1 = MaxDarkroom.ToString("X2").Substring(0, 1);
                string s2 = MaxDarkroom.ToString("X2").Substring(1, 2);
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 0D 01 0" + s1 + " " + s2), 5);
            }
            //if (!NetCom3.Instance.SingleQuery())
            //{
            //    //ConnectServer();
            //    keepaliveFlag = false;
            //    isHeartbeatLive = false;
            //}
            if (MinDarkroom < 256)
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 0D 02 00 " + MinDarkroom.ToString("X2")), 5);
            else
            {
                string s1 = MinDarkroom.ToString("X2").Substring(0, 1);
                string s2 = MinDarkroom.ToString("X2").Substring(1, 2);
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 0D 02 0" + s1 + " " + s2), 5);
            }
            //if (!NetCom3.Instance.SingleQuery())
            //{
            //    //ConnectServer();
            //    keepaliveFlag = false;
            //    isHeartbeatLive = false;
            //}
        }
        private string GetCultureInfo()
        {
            if (OperateIniFile.ReadInIPara("CultureInfo", "Culture") == "en")
            {
                return "en";
            }

            return "zh-CN";
        }
        private void SetCultureInfo()
        {
            Language.AppCultureInfo = new System.Globalization.CultureInfo(GetCultureInfo());
            //Language.AppCultureInfo = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentCulture = Language.AppCultureInfo;
            System.Threading.Thread.CurrentThread.CurrentUICulture = Language.AppCultureInfo;
        }

    }
}
