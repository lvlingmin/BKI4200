using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common;
using System.Threading;
using System.IO.Ports;
using Microsoft.Win32;
using System.Resources;

namespace BioBaseCLIA.InfoSetting
{
    public partial class frmNetSet : frmParent
    {
        frmMessageShow frmMsgShow = new frmMessageShow();
        DelayClass delayer = new DelayClass();//延时类
        public EventWaitHandle wait = new AutoResetEvent(false);
        string CommunicationType;//通讯类型 2018-5-14 zlx add
        bool IsLisConnect;//是否进行Lis通讯  2018-5-14zlx add
        public frmNetSet()
        {
            InitializeComponent();
            IsLisConnect = bool.Parse(OperateIniFile.ReadInIPara("LisSet", "IsLisConnect"));
        }
        private void frmNetSet_Load(object sender, EventArgs e)
        {
            #region 通讯设置
            ShowNetPara();
            #endregion
            if ((!IsLisConnect) || (CommunicationType == ""))
            {
                tabControlMy1.TabPages.Remove(tabLisSet);
                tabControlMy1.TabPages.Remove(tabLisSetCK);
            }
            else
            {
                tabControlMy1.TabPages.Remove(tabLisSet);
                tabControlMy1.TabPages.Remove(tabLisSetCK);
                if(CommunicationType == "NetConn")
                    tabControlMy1.TabPages.Add(tabLisSet);
                else if((CommunicationType == "SerialConn"))
                    tabControlMy1.TabPages.Add(tabLisSetCK);
                //switch (CommunicationType)
                //{
                //    case "网口通讯":
                //        tabControlMy1.TabPages.Add(tabLisSet);
                //        break;
                //    case "串口通讯":
                //        tabControlMy1.TabPages.Add(tabLisSetCK);
                //        break;
                //}
            }
            #region 获取可用端口 2018-5-14 zlxadd
            string[] str = SerialPort.GetPortNames();
            cmbCom.Items.Clear();
            List<string> list = GetComlist(false);
            for (int i = 0; i < list.Count; i++)
            {
                cmbCom.Items.Add(list[i]);
            }
            #endregion 
            lisParaShow();
            cmbLisType.SelectedItem =Getstring(CommunicationType);
            if (CommunicationType == "NetConn")
            {
                if (LisCommunication.Instance.IsConnect())
                {
                    labStatus.Text = Getstring("Connceted");
                    fbtnLisCon.Enabled = false;
                    fbtnLISClose.Enabled = true;
                }
                else
                {
                    labStatus.Text = Getstring("NotConnceted");
                    fbtnLisCon.Enabled = true;
                    fbtnLISClose.Enabled = false;
                }
            }
            else if ((CommunicationType == "SerialConn"))
            {
                if (LisConnection.Instance.IsOpen())
                {
                    labSStatus.Text = Getstring("Open");
                    fbtnComOpen.Enabled = false;
                    fbtnComClose.Enabled = true;
                }
                else
                {
                    labSStatus.Text = Getstring("Close");
                    fbtnComOpen.Enabled = true;
                    fbtnComClose.Enabled = false;
                }
            }
            //switch (CommunicationType)//2018-5-7 zlx mod
            //{
            //    case "网口通讯":
            //        if (LisCommunication.Instance.IsConnect())
            //        {
            //            labStatus.Text = "已连接";
            //            fbtnLisCon.Enabled = false;
            //            fbtnLISClose.Enabled = true;
            //        }
            //        else
            //        {
            //            labStatus.Text = "未连接";
            //            fbtnLisCon.Enabled = true;
            //            fbtnLISClose.Enabled = false;
            //        }
            //        break;
            //    case "串口通讯":
            //        if (LisConnection.Instance.IsOpen())
            //        {
            //            labSStatus.Text = "打开";
            //            fbtnComOpen.Enabled = false;
            //            fbtnComClose.Enabled = true;
            //        }
            //        else
            //        {
            //            labSStatus.Text = "关闭";
            //            fbtnComOpen.Enabled = true;
            //            fbtnComClose.Enabled = false;
            //        }
            //        break;
            //    default:
            //        break;
            //}
        }

        private void frmNetSet_SizeChanged(object sender, EventArgs e)
        {
            formSizeChange(this);
        }

        private void btnProInfo_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmInfo"))
            {
                frmInfo frmif = new frmInfo();
                frmif.TopLevel = false;
                frmif.Parent = this.Parent;
                frmif.Show();
            }
            else
            {
                frmInfo frmif = (frmInfo)Application.OpenForms["frmInfo"];
                frmif.BringToFront();

            }
        }

        private void btnUserInfo_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmUserManage"))
            {
                frmUserManage frmUM = new frmUserManage();
                frmUM.TopLevel = false;
                frmUM.Parent = this.Parent;
                frmUM.Show();
            }
            else
            {
                frmUserManage frmUM = (frmUserManage)Application.OpenForms["frmUserManage"];
                frmUM.BringToFront();

            }
        }

        private void btnInstrumentPara_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmInstrumentPara"))
            {
                frmInstrumentPara frmIP = new frmInstrumentPara();
                frmIP.TopLevel = false;
                frmIP.Parent = this.Parent;
                frmIP.Show();
            }
            else
            {
                frmInstrumentPara frmIP = (frmInstrumentPara)Application.OpenForms["frmInstrumentPara"];
                frmIP.BringToFront();

            }
        }

        private void fbtnReturn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region 通讯设置

        void ShowNetPara()
        {
            txtNetIPAdress.Text = OperateIniFile.ReadInIPara("NetSet", "IPAdress");
            txtNetPort.Text = OperateIniFile.ReadInIPara("NetSet", "Port");
        }
        private void fbtnNetSave_Click(object sender, EventArgs e)
        {
            if (txtNetPort.Text.Trim() == "")//y modify 2018.4.19/*
            {
                frmMsgShow.MessageShow(Getstring("ConnectSet"), Getstring("NullPort"));
                return;
            }
            if (chkIsLisConn.Checked)
            { 
                if(cmbLisType.SelectedItem ==null)
                    frmMsgShow.MessageShow(Getstring("ConnectSet"), Getstring("LisSelect"));
            }
            if (Inspect.InspectIP(txtNetIPAdress.Text.Trim()))//y modify 2018.4.19*/
            {
                OperateIniFile.WriteIniPara("NetSet", "IPAdress", txtNetIPAdress.Text.Trim());
                OperateIniFile.WriteIniPara("NetSet", "Port", txtNetPort.Text.Trim());
                OperateIniFile.WriteIniPara("LisSet", "CommunicationType", cmbLisConType.Text );
                OperateIniFile.WriteIniPara("LisSet", "IsLisConnect", chkIsLisConn.Checked.ToString());
                frmMsgShow.MessageShow(Getstring("ConnectSet"),Getstring("NetSaveSucess"));
            }
            else
            {
                frmMsgShow.MessageShow(Getstring("ConnectSet"), Getstring("IPErrorMesage"));//y modify 2018.4.19
            }
            
        }
        #endregion
        

        #region LIS设置
        void lisParaShow()
        {

            CommunicationType =OperateIniFile.ReadInIPara("LisSet", "CommunicationType");
            string IPAddress = OperateIniFile.ReadInIPara("LisSet", "IPAddress");
            string Port = OperateIniFile.ReadInIPara("LisSet", "Port");
            string ConnectType = OperateIniFile.ReadInIPara("LisSet", "ConnectType");
            string LisCodeType = OperateIniFile.ReadInIPara("LisSet", "LisCodeType");
            bool IsTrueTimeTran = bool.Parse(OperateIniFile.ReadInIPara("LisSet", "IsTrueTimeTran"));
            string transinfo = OperateIniFile.ReadInIPara("LisSet", "TransInfo");
            if (CommunicationType == "NetConn")
            {
                txtLISIPAddress.Text = IPAddress;
                txtLISPort.Text = Port;
                cmbLisConType.SelectedItem = ConnectType;
                chkIsLisConn.Checked = chISLis.Checked = IsLisConnect;
                cmbLisCodeType.Text = LisCodeType;
                chISDataSend.Checked = IsTrueTimeTran;
                if (transinfo != "")
                {
                    string[] arry = transinfo.Split(',');
                    foreach (string num in arry)
                    {
                        if (num != "")
                            lisCheckNum.Items.Add(num);
                    }
                }
                tabControlMy1.TabPages.Remove(tabLisSetCK);
            }
            else if (CommunicationType == "SerialConn")
            {
                cmbCom.SelectedItem = IPAddress;
                cmbBaud.SelectedItem = Port;
                CmdConnType.SelectedItem = ConnectType;
                chkIsLisConn.Checked = chkISLis.Checked = IsLisConnect;
                comEncodType.Text = LisCodeType;
                chkISDataSend.Checked = IsTrueTimeTran;
                if (transinfo != "")
                {
                    string[] arry = transinfo.Split(',');
                    foreach (string num in arry)
                    {
                        if (num != "")
                            lisCheckNumS.Items.Add(num);
                    }
                }
                tabControlMy1.TabPages.Remove(tabLisSet);
            }
            //switch (CommunicationType)
            //{
            //    case "网口通讯":
            //        txtLISIPAddress.Text = IPAddress;
            //        txtLISPort.Text = Port;
            //        cmbLisConType.SelectedItem = ConnectType;
            //        chkIsLisConn.Checked  = chISLis.Checked = IsLisConnect;
            //        cmbLisCodeType.Text = LisCodeType;
            //        chISDataSend.Checked = IsTrueTimeTran;
            //        if (transinfo != "")
            //        {
            //            string[] arry = transinfo.Split(',');
            //            foreach (string num in arry)
            //            {
            //                if (num != "")
            //                    lisCheckNum.Items.Add(num);
            //            }
            //        }
            //        tabControlMy1.TabPages.Remove(tabLisSetCK);
            //        break;
            //    case "串口通讯":
            //        cmbCom.SelectedItem = IPAddress;
            //        cmbBaud.SelectedItem = Port;
            //        CmdConnType.SelectedItem = ConnectType;
            //        chkIsLisConn.Checked = chkISLis.Checked = IsLisConnect;
            //        comEncodType.Text = LisCodeType;
            //        chkISDataSend.Checked = IsTrueTimeTran;
            //        if (transinfo != "")
            //        {
            //            string[] arry = transinfo.Split(',');
            //            foreach (string num in arry)
            //            {
            //                if (num != "")
            //                    lisCheckNumS.Items.Add(num);
            //            }
            //        }
            //        tabControlMy1.TabPages.Remove(tabLisSet);
            //        break;
            //    default:
            //        break;
            //}
        }
        private void fbtnLisCon_Click(object sender, EventArgs e)
        {
            if (cmbLisConType.SelectedItem == null)
            {
                frmMsgShow.MessageShow(Getstring("ConnectSet"), Getstring("LisSelect"));
                return;
            }

            if (txtLISPort.Text.Trim() == "")//y modify 2018.4.19/*
            {
                frmMsgShow.MessageShow(Getstring("ConnectSet"), Getstring("NullPort"));
                return;
            }
            
            if (!Inspect.InspectIP(txtLISIPAddress.Text.Trim()))
            {
                frmMsgShow.MessageShow(Getstring("ConnectSet"), Getstring("IPErrorMesage"));
                return;
            }//y modify 2018.4.19*/
            
            wait.Reset();
            LisCommunication.Instance.comDelayer = delayer;
            LisCommunication.Instance.comWait = wait;

            delayer.sign = wait;
            LisCommunication.Instance.ReceiveHandel += new Action<string>(InstanceLIS_ReceiveHandel);
            if (!LisCommunication.Instance.IsConnect())
            {
                string ip = txtLISIPAddress.Text.ToString().Trim();
                int port = int.Parse(txtLISPort.Text.Trim());
                LisCommunication.Instance.connect(ip, port);
            }
            if (cmbLisCodeType.Text == null)//2018-4-25 zlxadd
                LisCommunication.Instance.EncodeType = "Unicode";
            else
                LisCommunication.Instance.EncodeType = cmbLisCodeType.Text.ToString();
            if (LisCommunication.Instance.IsConnect())
            {
                if(CommunicationType == "NetConn")
                    OperateIniFile.WriteIniPara("LisSet", "CommunicationType", Getstring("NetConn"));//2018-5-14 zlx add
                else if(CommunicationType == "SerialConn")
                    OperateIniFile.WriteIniPara("LisSet", "CommunicationType", Getstring("SerialConn"));//2018-5-14 zlx add
                OperateIniFile.WriteIniPara("LisSet", "IPAddress", txtLISIPAddress.Text.Trim());
                OperateIniFile.WriteIniPara("LisSet", "Port", txtLISPort.Text.Trim());
                OperateIniFile.WriteIniPara("LisSet", "ConnectType", cmbLisConType.SelectedItem.ToString());
                OperateIniFile.WriteIniPara("LisSet", "LisCodeType", LisCommunication.Instance.EncodeType);
                labStatus.Text = Getstring("Connceted");
                fbtnLisCon.Enabled = false;
                fbtnLISClose.Enabled = true;
            }
        }
        void InstanceLIS_ReceiveHandel(string obj)
        {
            byte[] data = new byte[8000];
            try
            {


                if (IsDisposed || !this.Parent.IsHandleCreated) return;
                data = new byte[8000];
            }
            catch (Exception e)
            {
                frmMsgShow.MessageShow("", e.Message);
            }


        }
        private void btnLeft_Click(object sender, EventArgs e)
        {
            if (lisName.SelectedItems.Count == 0) return;
            for (int i = 0; i < lisName.SelectedItems.Count; i++)
            {
                object obj = lisName.SelectedItems[i];
                string[] arryNum = obj.ToString().Split('、','.');
                if (lisCheckNum.Items.Contains(arryNum[0]))
                {
                    frmMsgShow.MessageShow(Getstring("LisSet"), Getstring("AddReData"));
                    return;
                }

                else
                {
                    lisCheckNum.Items.Add(arryNum[0]);
                }
            }

            StringBuilder strbud = new StringBuilder();
            for (int j = 0; j < lisCheckNum.Items.Count; j++)
            {
                if (j < lisCheckNum.Items.Count - 1)
                    strbud.Append(lisCheckNum.Items[j] + ",");
                else if (j == lisCheckNum.Items.Count - 1)
                    strbud.Append(lisCheckNum.Items[j]);
            }
            OperateIniFile.WriteIniPara("LisSet", "TransInfo", strbud.ToString());
        }

        private void fbtnLISClose_Click(object sender, EventArgs e)
        {
            if (LisCommunication.Instance.thReceive != null)
                LisCommunication.Instance.thReceive.Abort();
            if (LisCommunication.Instance.IsConnect())
            {
                LisCommunication.Instance.disconnection();
                LisCommunication.Instance.ReceiveHandel -= new Action<string>(InstanceLIS_ReceiveHandel);
            }
            if (!LisCommunication.Instance.IsConnect())
            {
                labStatus.Text = Getstring("NotConnceted");
                fbtnLisCon.Enabled = true;
                fbtnLISClose.Enabled = false;
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (lisCheckNum.SelectedItems.Count == 0) return;
            StringBuilder strbud = new StringBuilder();
            for (int i = lisCheckNum.SelectedItems.Count - 1; i >= 0; i--)
            {

                lisCheckNum.Items.Remove(lisCheckNum.SelectedItems[i]);


            }
            for (int j = 0; j < lisCheckNum.Items.Count; j++)
            {

                strbud.Append(lisCheckNum.Items[j] + ",");

            }
            OperateIniFile.WriteIniPara("LisSet", "TransInfo", strbud.ToString());
        }

        private void frmNetSet_FormClosed(object sender, FormClosedEventArgs e)
        {
            //LisCommunication.Instance.ReceiveHandel -= new Action<string>(InstanceLIS_ReceiveHandel);
        }

        private void chISLis_CheckedChanged(object sender, EventArgs e)
        {
            OperateIniFile.WriteIniPara("LisSet", "IsLisConnect", chISLis.Checked.ToString());

        }

        private void chISDataSend_CheckedChanged(object sender, EventArgs e)
        {
            OperateIniFile.WriteIniPara("LisSet", "IsTrueTimeTran", chISDataSend.Checked.ToString());
        }
        #endregion

        private void cmbLisCodeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //2018-4-26 zlx add
            if (cmbLisCodeType.Text == null)
                LisCommunication.Instance.EncodeType = "Unicode";
            else
                LisCommunication.Instance.EncodeType = cmbLisCodeType.Text.ToString();
            OperateIniFile.WriteIniPara("LisSet", "LisCodeType", LisCommunication.Instance.EncodeType);
        }

        private void fbtnComOpen_Click(object sender, EventArgs e)
        {
            //2018-5-7 zlx add
            if (cmbCom.SelectedItem == null)
            {
                frmMsgShow.MessageShow(Getstring("SerialConnSet"),Getstring("NullPort"));
                return;
            }

            if (cmbBaud.SelectedItem == null)
            {
                frmMsgShow.MessageShow(Getstring("SerialConnSet"),Getstring("NullBaud"));
                return;
            }
            wait.Reset();
            LisConnection.Instance.comDelayer = delayer;
            LisConnection.Instance.comWait = wait;
            delayer.sign = wait;
            // LisConnection.Instance.ReceiveHandel += new Action<string>(InstanceLIS_ReceiveHandel);
            if (cmbLisCodeType.Text == null)
                LisConnection.Instance.EncodeType = "Unicode";
            else
                LisConnection.Instance.EncodeType = cmbLisCodeType.Text.ToString();
            if (CmdConnType.Text == "")
                cmbLisConType.SelectedIndex = 0;
            if (!LisConnection.Instance.IsOpen())
            {
                if(CommunicationType==Getstring("SerialConn"))
                    OperateIniFile.WriteIniPara("LisSet", "CommunicationType",Getstring("SerialConn"));
                else
                    OperateIniFile.WriteIniPara("LisSet", "CommunicationType", Getstring("NetConn"));
                OperateIniFile.WriteIniPara("LisSet", "IPAddress", cmbCom.SelectedItem.ToString());
                OperateIniFile.WriteIniPara("LisSet", "Port", cmbBaud.SelectedItem.ToString());
                OperateIniFile.WriteIniPara("LisSet", "ConnectType", CmdConnType.SelectedItem.ToString());
                OperateIniFile.WriteIniPara("LisSet", "LisCodeType", LisConnection.Instance.EncodeType);
                LisConnection.Instance.connect();
            }
            if (LisConnection.Instance.IsOpen())
            {

                labSStatus.Text = Getstring("Open");
                fbtnComOpen.Enabled = false;
                fbtnComClose.Enabled = true;
            }

        }

        private void fbtnComClose_Click(object sender, EventArgs e)
        {
            //2018-5-7 add
            if (LisConnection.Instance.IsOpen())
            {
                LisConnection.Instance.disconnection();
            }
            if (!LisConnection.Instance.IsOpen())
            {
                labSStatus.Text = Getstring("Close");
                fbtnComOpen.Enabled = true;
                fbtnComClose.Enabled = false;
            }
        }
        /// <summary>
        /// 获取本机串口信息
        /// </summary>
        /// <param name="isUseReg"></param>
        /// <returns></returns>
        private List<string> GetComlist(bool isUseReg)
        {
            //2018-5-7 添加
            List<string> list = new List<string>();
            try
            {
                if (isUseReg)
                {
                    RegistryKey RootKey = Registry.LocalMachine;
                    RegistryKey Comkey = RootKey.OpenSubKey(@"HARDWARE\DEVICEMAP\SERIALCOMM");

                    String[] ComNames = Comkey.GetValueNames();

                    foreach (String ComNamekey in ComNames)
                    {
                        string TemS = Comkey.GetValue(ComNamekey).ToString();
                        list.Add(TemS);
                    }
                }
                else
                {
                    foreach (string com in SerialPort.GetPortNames())  //自动获取串行口名称  
                        list.Add(com);
                }
            }
            catch
            {
                MessageBox.Show(Getstring("SerialConnSet"), Getstring("SerialConnError"));
                System.Environment.Exit(0); //彻底退出应用程序   
            }
            return list;
        }

        private void chkISLis_CheckedChanged(object sender, EventArgs e)
        {
            OperateIniFile.WriteIniPara("LisSet", "IsLisConnect", chkISLis.Checked.ToString());
        }

        private void chkISDataSend_CheckedChanged(object sender, EventArgs e)
        {
            OperateIniFile.WriteIniPara("LisSet", "IsTrueTimeTran", chkISDataSend.Checked.ToString());
        }

        private void chkIsLisConn_CheckedChanged(object sender, EventArgs e)
        {
            IsLisConnect = chkIsLisConn.Checked;
            OperateIniFile.WriteIniPara("LisSet", "IsLisConnect", chkIsLisConn.Checked.ToString());
            frmNetSet_Load(sender,e);
            //OnLoad(null);
            
        }

        private void cmbLisType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLisType.SelectedItem == null)
                return;
            if (cmbLisType.SelectedItem.ToString() == Getstring("NetConn"))
                CommunicationType = "NetConn";
            else
                CommunicationType = "SerialConn";
            //CommunicationType = cmbLisType.SelectedItem.ToString();
            OperateIniFile.WriteIniPara("LisSet", "CommunicationType", CommunicationType);
            frmNetSet_Load(sender, e);
            //OnLoad(null);
        }
         public Control getControl()
        {
            Control _control=null;
            if (IsLisConnect)
            {
                if(CommunicationType == "NetConn")
                    _control = tabLisSet;
                else 
                    _control = tabLisSetCK;

                //switch (CommunicationType)
                //{
                //    case "网口通讯":
                //        _control = tabLisSet;
                //        break;
                //    case "串口通讯":
                //        _control = tabLisSetCK;
                //        break;
                //    default:
                //        break;
                //}
            }
            return _control;
        }
        private void comEncodType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //2018-4-26 zlx add
            if (comEncodType.Text == null)
                LisCommunication.Instance.EncodeType = "Unicode";
            else
                LisCommunication.Instance.EncodeType = comEncodType.Text.ToString();
            OperateIniFile.WriteIniPara("LisSet", "LisCodeType", LisCommunication.Instance.EncodeType);
        }
        private string Getstring(string key)
        {
            ResourceManager resManagerA =
                    new ResourceManager("BioBaseCLIA.InfoSetting.frmNetSet", typeof(frmNetSet).Assembly);
            return resManagerA.GetString(key);
        }
    }
}
