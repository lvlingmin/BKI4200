using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using BioBaseCLIA.CalculateCurve;
using Common;
using System.IO;
using BioBaseCLIA.Run;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Maticsoft.DBUtility;

namespace BioBaseCLIA.SysMaintenance
{
    public partial class frmDiagnost : frmParent
    {
        #region 变量
        frmMessageShow frmMsgShow = new frmMessageShow();//20180524 zlx add
        /// <summary>
        /// 返回指令
        /// </summary>
        string BackObj = "";

        /// <summary>
        /// 清洗盘取放管位置当前孔号
        /// </summary>
        int currentHoleNum = 1;

        /// <summary>
        /// 存储线程，返回按钮结束时使用
        /// </summary>
        List<Thread> threadList = new List<Thread>();
        /// <summary>
        /// 液位开始查询标志
        /// </summary>
        public static bool BQLiquaid = false;
        #region 基础性能变量
        /// <summary>
        /// 反应盘待使用空白反应管个数
        /// </summary>
        int toUsedTube = 10;
        /// <summary>
        /// 底物与管架配置文件地址
        /// </summary>
        string iniPathSubstrateTube = Directory.GetCurrentDirectory() + "\\SubstrateTube.ini";
        /// <summary>
        /// 试剂盘配置文件地址
        /// </summary>
        string iniPathReagentTrayInfo = Directory.GetCurrentDirectory() + "\\ReagentTrayInfo.ini";
        /// <summary>
        /// 反应盘配置文件地址
        /// </summary>
        string iniPathReactTrayInfo = Directory.GetCurrentDirectory() + "\\ReactTrayInfo.ini";
        /// <summary>
        /// 清洗盘配置文件地址
        /// </summary>
        string iniPathWashTrayInfo = Directory.GetCurrentDirectory() + "\\WashTrayInfo.ini";
        /// <summary>
        /// 夹管线程
        /// </summary>
        private Thread MoveTubeThread;
        /// <summary>
        /// 指令正在运行
        /// </summary>
        bool runFlag = false;
        /// <summary>
        /// 存储需进行取放管的反应管信息
        /// </summary>
        List<MoveTubeStatus> lisMoveTube = new List<MoveTubeStatus>();
        MoveTubeStatus moveTube = new MoveTubeStatus();
        /// <summary>
        /// 移管手标志位
        /// </summary>
        bool MoveTubeUseFlag = false;
        /// <summary>
        /// 清洗盘标志位
        /// </summary>
        bool washTrayFlag = false;
        /// <summary>
        /// 反应盘孔数
        /// </summary>
        int ReactTrayHoleNum = int.Parse(OperateIniFile.ReadInIPara("OtherPara", "ReactTrayHoleNum"));
        /// <summary>
        /// 清洗盘信息
        /// </summary>
        DataTable dtWashTrayInfo = new DataTable();
        commands cmd = new commands();

        #endregion

        #region 烧录界面变量
        int selectZhenID = -1;
        int[] receiveFlag = new int[6];
        string iapFilePath = ""; //烧录文件路径
        Thread thFire = null;
        #endregion
        #region 版本号变量
        string[] boardVersion = new string[9];
        #endregion

        #region 规范测试变量
        /// <summary>
        /// 导出信息表
        /// </summary>
        DataTable dtexp = new DataTable();
        /// <summary>
        /// 清洗效果测试20s转1格
        /// </summary>
        DateTime dtimeWash;
        #endregion
        #endregion
        bool bClose = true;
        /// <summary>
        /// 记录单步指令类型，还原信号量时使用
        /// </summary>
        int stepOrderSendFlag = 5;
        #region 老化测试变量
        /// <summary>
        /// 老化测试线程
        /// </summary>
        private Thread AgingTestRun;
        /// <summary>
        /// 老化测试是否正在运行
        /// </summary>
        bool AgingRunFlag = false;
        /// <summary>
        /// 当前管架位置
        /// </summary>
        //int CurrentTubePos = 1;
        /// <summary>
        /// 当前温育盘位置
        /// </summary>
        int CurrentReactPos = 1;
        /// <summary>
        /// 当前取样位置
        /// </summary>
        int CurrentAsPos = 1;
        /// <summary>
        /// 0-准备发送,1-成功 2-发送失败 3-接收失败 4-抓管撞管（撞针） 5-抓空 6-混匀异常 7-放管撞管 8-理杯机缺管 9-发送超时
        /// </summary>
        string[] State = { "准备发送", "成功", "发送失败", "接收失败", "抓管撞管（撞针）", "抓空", "混匀异常", "放管撞管", "理杯机缺管", "发送超时" };
        //txtAgingInfoShow最后一条log
        string showLog = "";
        //暂存盘是否有管
        bool AgingHaveTube = false;
        #endregion
        public frmDiagnost()
        {
            InitializeComponent();
            label98.Location = new Point(22, 80);
            untxtInjection.Location = new Point(89, 77);
        }
        #region 公共方法
        private void frmDiagnost_Load(object sender, EventArgs e)
        {
            bClose = false;
            new Thread(new ParameterizedThreadStart((obj) =>
            {
                NetCom3.Instance.ReceiveHandel += new Action<string>(Instance_ReceiveHandel);
                if (!NetCom3.isConnect)
                {
                    if (NetCom3.Instance.CheckMyIp_Port_Link())
                    {
                        NetCom3.Instance.ConnectServer();

                        if (!NetCom3.isConnect)
                            return;

                    }
                }
            }))
            { IsBackground = true }.Start();
            //this block add y 20180816
            new Thread(new ParameterizedThreadStart((obj) =>
            {
                NetCom3.Instance.ReceiveHandelForQueryTemperatureAndLiquidLevel += new Action<string>(Read);
                if (!NetCom3.isConnect)
                {
                    if (NetCom3.Instance.CheckMyIp_Port_Link())
                    {
                        NetCom3.Instance.ConnectServer();

                        if (!NetCom3.isConnect)
                            return;

                    }
                }
            }))
            { IsBackground = true }.Start();
            //block end
            //查询清洗盘管信息
            dtWashTrayInfo = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
            iniReader();//读取温度监控参数信息
            numOfSample.Value = (decimal)numOfSam;//更新控件的值
            timespanOfSample.Value = timespan;
            numDown.Value = (decimal)num1;
            numUp.Value = (decimal)num2;
            chart1.ChartAreas[0].AxisX.Maximum = numOfSam;//确定图标的属性
            timer1.Interval = Convert.ToInt32(timespan) * 1000;//timer1的间隔时间
            chart1.ChartAreas[0].AxisY.Minimum = num1;
            chart1.ChartAreas[0].AxisY.Maximum = num2;
            saveFileDialog1.InitialDirectory = System.Windows.Forms.Application.StartupPath + @"\daily";//默认路径
            cmbArmRegentPos.SelectedIndex = 0;
            cmbArmRegentPos.Items.Clear();
            List<int>list = GetReagentList();
            foreach (int li in list)
            {
                cmbArmRegentPos.Items.Add(li);
            }
            //zlx add 2018-08-31 zlx add 停止试剂盘旋转
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 09 01"), 0);
            while (!NetCom3.SpReciveFlag)
            {
                NetCom3.Delay(1);
            }
            bClose = true;
            BQLiquaid = false;
            cmbSendDelay.SelectedIndex = 2;
            cmbConnDelay.SelectedIndex = 1;
        }
        /// <summary>
        /// 试剂位置下拉列表
        /// </summary>
        /// <returns></returns>
        private List<int> GetReagentList()
        {
            List<int> list = new List<int>();
            for (int i = 1; i <= frmParent.RegentNum; i++)
            {
                list.Add(i);
            }
            return list;
        }
        void Instance_ReceiveHandel(string obj)
        {
            if (obj.IsNullOrEmpty())
            {
                return;
            }
            else
            {
                BackObj = obj;
            }
        }
        private void frmDiagnost_SizeChanged(object sender, EventArgs e)
        {
            formSizeChange(this);
        }

        private void fbtnInstruMaintenance_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmInstruMaintenance"))
            {
                frmInstruMaintenance frnIM = new frmInstruMaintenance();
                //this.TopLevel = false;
                frnIM.TopLevel = false;
                frnIM.Parent = this.Parent;
                frnIM.Show();
            }
            else
            {
                frmInstruMaintenance frnIM = (frmInstruMaintenance)Application.OpenForms["frmInstruMaintenance"];
                //frmIM.Activate();
                frnIM.BringToFront();

            }
        }
        private void fbtnGroupTest_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmInstruGroupTest"))
            {
                frmInstruGroupTest frnIGT = new frmInstruGroupTest();
                //this.TopLevel = false;
                frnIGT.TopLevel = false;
                frnIGT.Parent = this.Parent;
                frnIGT.Show();
            }
            else
            {
                frmInstruGroupTest frnIGT = (frmInstruGroupTest)Application.OpenForms["frmInstruGroupTest"];
                //frmIM.Activate();
                frnIGT.BringToFront();

            }
        }

        private void fbtnReturn_Click(object sender, EventArgs e)
        {
            if (!bClose)
            {
                return;
            }
            if (needleUpUp == false)
            {
                MessageBox.Show("请检查加样针模块是否全部复位！");
                return;
            }
            timer1.Stop(); //返回就结束timer  jun add 20190426
            NetCom3.Instance.ReceiveHandel -= dealAgingBack;
            foreach (Thread a in threadList)
            {
                if (a != null && a.IsAlive)
                    a.Abort();
            }
            //2018-08-31 ZLX add 试剂盘加载完成
            fbtnReturn.Enabled = false;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 0B B0 00 00"), 0);
            NetCom3.Instance.SPQuery();
            fbtnReturn.Enabled = true;
            this.Close();
        }
        private void frmDiagnost_FormClosed(object sender, FormClosedEventArgs e)
        {
            NetCom3.Instance.ReceiveHandel -= new Action<string>(Instance_ReceiveHandel);
            NetCom3.Instance.ReceiveHandelForQueryTemperatureAndLiquidLevel -= new Action<string>(Read);
        }
        #endregion
        #region 移管手
        private void cmbHandPara_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            cmbHandPara.Enabled = false;
            //暂存盘位置
            if (cmbHandPara.SelectedIndex == 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 01 01"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //温育盘位置
            else if (cmbHandPara.SelectedIndex == 1)
            {
                sendMonitor(NetCom3.Cover("EB 90 01 01 02"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //清洗盘位置
            else if (cmbHandPara.SelectedIndex == 2)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 01 03"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //扔管位置
            else if (cmbHandPara.SelectedIndex == 3)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 01 04"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //理杯块位置
            else if (cmbHandPara.SelectedIndex == 4)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 01 05"), 5);
                NetCom3.Instance.SingleQuery();
            }
            cmbHandPara.Enabled = true;

            UpdateHandSteps();
        }

        private void UpdateHandSteps()
        {
            if (cmbHandPara.SelectedIndex < 0 || cmbHElecMachine.SelectedIndex < 0) return;

            string machine = string.Empty;
            switch (cmbHElecMachine.Text) 
            {
                case "理杯块电机":
                    machine = "01";
                    break;
                case "暂存盘电机":
                    machine = "02";
                    break;
                case "垂直电机":
                    machine = "03";
                    break;
                case "旋转电机":
                    machine = "04";
                    break;
                default:
                    return;
            }

            txtWashCurValue.Text =
                GetCurrentSteps((cmbHandPara.SelectedIndex + 1).ToString().PadLeft(2, '0'),
                machine, "01");
        }
        private void btnHandAdd_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbHElecMachine.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择需调试的电机！");
                return;
            }
            if (txtMoveIncrem.Text == "")
            {
                frmMsgShow.MessageShow("仪器调试", "请输入增量值！");
                txtMoveIncrem.Focus();
                return;
            }
            btnHandAdd.Enabled = false;
            string incream = int.Parse(txtMoveIncrem.Text.Trim()).ToString("x8");
            //暂存盘电机
            if (cmbHElecMachine.SelectedIndex == 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 02 02 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            //垂直电机
            else if (cmbHElecMachine.SelectedIndex == 1)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 02 03 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                     + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            //旋转电机
            else if (cmbHElecMachine.SelectedIndex == 2)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 02 04 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            //理杯块电机
            else if (cmbHElecMachine.SelectedIndex == 3)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 02 01 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            btnHandAdd.Enabled = true;
        }

        private void btnHandSub_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbHElecMachine.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择需调试的电机！");
                return;
            }
            if (txtMoveIncrem.Text == "")
            {
                frmMsgShow.MessageShow("仪器调试", "请输入增量值！");
                txtMoveIncrem.Focus();
                return;
            }
            btnHandSub.Enabled = false;
            string incream = int.Parse("-" + txtMoveIncrem.Text.Trim()).ToString("x8");
            //暂存盘电机
            if (cmbHElecMachine.SelectedIndex == 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 02 02 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            //垂直电机
            else if (cmbHElecMachine.SelectedIndex == 1)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 02 03 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                     + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            //旋转电机
            else if (cmbHElecMachine.SelectedIndex == 2)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 02 04 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            //理杯块电机
            else if (cmbHElecMachine.SelectedIndex == 3)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 02 01 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            btnHandSub.Enabled = true;
        }

        private void btnSaveHand_Click(object sender, EventArgs e)
        {

            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            btnSaveHand.Enabled = false;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 13"), 5);
            NetCom3.Instance.SingleQuery();
            NetCom3.Delay(500);//lyq add 20201016 指令返回太快，增加一个点击停顿
            /*
          if (cmbHElecMachine.SelectedItem == null)
          {
              frmMsgShow.MessageShow("仪器调试", "请选择需调试的电机！");
              return;
          }
          btnSaveHand.Enabled = false;
          if (cmbHElecMachine.SelectedIndex == 0)
          {
              NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 01 13"), 5);
              NetCom3.Instance.SingleQuery();
          }
          else if (cmbHElecMachine.SelectedIndex == 1)
          {
              NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 02 13"), 5);
              NetCom3.Instance.SingleQuery();
          }
          else if (cmbHElecMachine.SelectedIndex == 2)
          {
              NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 03 13"), 5);
              NetCom3.Instance.SingleQuery();
          }
          else if (cmbHElecMachine.SelectedIndex == 3)
          {
              NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 04 13"), 5);
              NetCom3.Instance.SingleQuery();
          }
          //清洗盘电机
          else if (cmbHElecMachine.SelectedIndex == 4)
          {
              NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 01 13"), 5);
              NetCom3.Instance.SingleQuery();
          }
          else if (cmbHElecMachine.SelectedIndex == 5)
          {
              NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 05 13"), 5);
              NetCom3.Instance.SingleQuery();
          }
          else if (cmbHElecMachine.SelectedIndex == 6)
          {
              if (cmbHandPara.SelectedIndex == 8)
              {
                  NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 06 13"), 5);
              }
              else
              {
                  NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 07 13"), 5);
              }
              NetCom3.Instance.SingleQuery();
          }
           */
            btnSaveHand.Enabled = true;
            UpdateHandSteps();
        }

        private void btnHand_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbHandIntegrate.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择旋转动作");
                cmbHandIntegrate.Focus();
                return;
            }
            btnHand.Enabled = false;
            if (cmbHandIntegrate.SelectedIndex == 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 01 01 00 00"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbHandIntegrate.SelectedIndex == 1)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 01 02 00 00"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbHandIntegrate.SelectedIndex == 2)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 01 03 00 00"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbHandIntegrate.SelectedIndex == 3)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 01 04 00 00"), 5);
                NetCom3.Instance.SingleQuery();
            }
            btnHand.Enabled = true;
        }

        private void fbtnINMove_Click(object sender, EventArgs e)
        {

            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            //if (cmbINMovePosition.SelectedItem == null)
            //{
            //    frmMsgShow.MessageShow("仪器调试", "请选择温育盘动作！");
            //    cmbINMovePosition.Focus();
            //    return;
            //}
            fbtnINMove.Enabled = false;
            int HoleNum = int.Parse(txtMoveInHoleNum.Text.Trim());
            sendMonitor(NetCom3.Cover("EB 90 31 04 01 " + HoleNum.ToString("x2") + ""), 0);
            NetCom3.Instance.SPQuery();
            //NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 0a 01 " + HoleNum.ToString("x2") + ""), 5);
            //NetCom3.Instance.SingleQuery();
            //if (cmbINMovePosition.SelectedIndex == 0)
            //{
            //    NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 02 " + HoleNum.ToString("x2") + " "), 5);
            //    NetCom3.Instance.SingleQuery();
            //}
            //else if (cmbINMovePosition.SelectedIndex == 1)
            //{
            //    NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 02 00 " + HoleNum.ToString("x2")), 5);
            //    NetCom3.Instance.SingleQuery();
            //}
            fbtnINMove.Enabled = true;

        }

        private void fbtnHandZ_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (numHole.Value == 0)
            {
                numHole.Focus();
                return;
            }
            fbtnHandZ.Enabled = false;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 05 " + int.Parse(numHole.Value.ToString()).ToString("X2") + ""), 5);
            NetCom3.Instance.SingleQuery();
            fbtnHandZ.Enabled = true;
        }

        private void fbtnMix1Arm_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbMix1Arm.SelectedItem == null)
            {
                cmbMix1Arm.Focus();
                frmMsgShow.MessageShow("仪器调试", "请选择混匀臂动作！");
                return;
            }
            fbtnMix1Arm.Enabled = false;
            if (cmbMix1Arm.SelectedIndex == 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 0a 02 30"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbMix1Arm.SelectedIndex == 1)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 0a 02 31"), 5);
                NetCom3.Instance.SingleQuery();
            }
            fbtnMix1Arm.Enabled = true;
        }

        private void fbtnPressCup_Click(object sender, EventArgs e)
        {

            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbPressCup.SelectedItem == null)
            {
                cmbPressCup.Focus();
                frmMsgShow.MessageShow("仪器调试", "请选择混匀臂动作！");
                return;
            }
            fbtnPressCup.Enabled = false;
            if (cmbPressCup.SelectedIndex == 0)//压杯归零
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 0a 03 30"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbPressCup.SelectedIndex == 1)//压杯
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 0a 03 31"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //if (cmbPressCup.SelectedIndex == 2)//压杯开始
            //{
            //    NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 A1 01 09"), 5);
            //    NetCom3.Instance.SingleQuery();
            //}
            //else if (cmbPressCup.SelectedIndex == 3)//压杯结束
            //{
            //    NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 A1 01 08"), 5);
            //    NetCom3.Instance.SingleQuery();
            //}
            fbtnPressCup.Enabled = true;
        }

        private void btnHandXInit_Click(object sender, EventArgs e)
        {
            //2018-09-30 zlx mod
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            btnHandXInit.Enabled = false;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 03 02"), 5);
            NetCom3.Instance.SingleQuery();
            btnHandXInit.Enabled = true;
        }

        private void btnHandYInit_Click(object sender, EventArgs e)
        {
            //2018-09-30 mod
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            btnHandYInit.Enabled = false;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 03 03"), 5);
            NetCom3.Instance.SingleQuery();
            btnHandYInit.Enabled = true;
        }

        private void btnHandZInit_Click(object sender, EventArgs e)
        {
            //2018-10-06 zlx mod
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            btnHandZInit.Enabled = false;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 03 04"), 5);
            NetCom3.Instance.SingleQuery();
            btnHandZInit.Enabled = true;
        }

        private void btnHandAllInit_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            btnHandAllInit.Enabled = false;
            sendMonitor(NetCom3.Cover("EB 90 01 03 00"), 5);
            NetCom3.Instance.SingleQuery();
            btnHandAllInit.Enabled = true;

            UpdateHandSteps();
        }

        private void btnHandOpen_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            btnHandOpen.Enabled = false;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 03 31"), 5);
            NetCom3.Instance.SingleQuery();
            //if (btnHandOpen.Text == "抓手打开")
            //{
            //    btnHandOpen.Text = "抓手关闭";
            //    NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 03 31"), 5);
            //    NetCom3.Instance.SingleQuery();
            //}
            //else
            //{
            //    btnHandOpen.Text = "抓手打开";
            //    NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 03 30"), 5);
            //    NetCom3.Instance.SingleQuery();
            //}
            btnHandOpen.Enabled = true;
        }


        private void fbtnMix1_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            fbtnMix1.Enabled = false;
            ////2018-09-27 zlx mod
            //NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 07 30"), 5);
            //NetCom3.Instance.SingleQuery();
            if (fbtnMix1.Text == "混匀开始")
            {
                fbtnMix1.Text = "混匀停止";
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 0a 04 30"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else
            {
                fbtnMix1.Text = "混匀开始";
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 0a 04 31"), 5);
                NetCom3.Instance.SingleQuery();
            }
            fbtnMix1.Enabled = true;
        }

        private void fbtnMix1Reset_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            fbtnMix1Reset.Enabled = false;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 05 30"), 5);
            NetCom3.Instance.SingleQuery();
            fbtnMix1Reset.Enabled = true;
        }

        private void fbtnPressCupZero_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            fbtnPressCupZero.Enabled = false;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 04 03 03"), 5);
            NetCom3.Instance.SingleQuery();
            fbtnPressCupZero.Enabled = true;
        }

        private void fbtnInTubeClear_Click(object sender, EventArgs e)
        {
            fbtnInTubeClear.Enabled = false;

            reactTrayTubeClear();

            fbtnInTubeClear.Enabled = true;
        }

        private void fbtnWashTubeClear_Click(object sender, EventArgs e)
        {
            fbtnWashTubeClear.Enabled = false;
            washTrayTubeClear();
            fbtnWashTubeClear.Enabled = true;

        }
        /// <summary>
        /// 清洗盘清管
        /// </summary>
        bool washTrayTubeClear()
        {
            DataTable dtWashTrayIni = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
            //2018-08-20 zlx mod
            for (int i = 0; i < dtWashTrayIni.Rows.Count; i++)
            {
                if (i != 0)
                {
                    //清洗盘顺时针旋转一位
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-1).ToString("X2").Substring(6, 2)), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        fbtnWashTubeClear.Enabled = true;
                        return false;
                    }
                    currentHoleNum = currentHoleNum - 1;
                    //如果孔号小于等于0
                    if (currentHoleNum <= 0)
                    {
                        currentHoleNum = currentHoleNum + 30;
                    }
                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                    dtWashTrayIni = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
                    DataTable dtTemp = new DataTable();
                    dtTemp = dtWashTrayIni.Copy();
                    //清洗盘状态列表中添加反应盘位置字段，赋值需多赋值一个字段。 
                    for (int j = 1; j < 2; j++)
                        dtWashTrayIni.Rows[dtWashTrayIni.Rows.Count - 1][j] = dtTemp.Rows[0][j];
                    for (int k = 0; k < dtWashTrayIni.Rows.Count - 1; k++)
                    {
                        for (int j = 1; j < 2; j++)
                        {
                            dtWashTrayIni.Rows[k][j] = dtTemp.Rows[k + 1][j];
                        }
                    }

                    OperateIniFile.WriteConfigToFile("[TubePosition]", iniPathWashTrayInfo, dtWashTrayIni);
                }
                #region 移管手取放管位置取管扔废管
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
                LogFile.Instance.Write("==============  " + currentHoleNum + "  扔管");
                if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                {
                    fbtnWashTubeClear.Enabled = true;
                    return false;
                }
                OperateIniFile.WriteIniData("TubePosition", "No1", "0", iniPathWashTrayInfo);
                if (LogFileRtest.Instance.Bcount)
                    LogFileRtest.Instance.Testcount++;
                #endregion
            }
            return true;
        }
        /// <summary>
        /// 温育反应盘清管
        /// </summary>
        bool reactTrayTubeClear()
        {
            LogFile.Instance.Write("运行调试界面温育盘清空代码！");
            DataTable dtInTrayIni = OperateIniFile.ReadConfig(iniPathReactTrayInfo);
            for (int i = 0; i < dtInTrayIni.Rows.Count; i++)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 " + int.Parse(dtInTrayIni.Rows[i][0].ToString().Substring(2)).ToString("x2")), 1);
                if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                {
                    fbtnInTubeClear.Enabled = true;
                    return false;
                }
                if (LogFileRtest.Instance.Bcount)
                    LogFileRtest.Instance.Testcount++;
                //修改反应盘信息
                OperateIniFile.WriteIniData("ReactTrayInfo", "no" + int.Parse(dtInTrayIni.Rows[i][0].ToString().Substring(2)).ToString(), "0", iniPathReactTrayInfo);
            }
            OperateIniFile.WriteIniData("Tube", "ReacTrayTub", "", iniPathSubstrateTube);
            
            return true;
        }
        #endregion

        #region 加样臂
        private void cmbASPara_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            cmbASPara.Enabled = false;
            //温育盘
            if (cmbASPara.SelectedIndex == 0)
            {
                sendMonitor(NetCom3.Cover("EB 90 02 A1 01 00"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //清洗杯
            else if (cmbASPara.SelectedIndex == 1)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 A1 01 01"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //样本
            else if (cmbASPara.SelectedIndex == 2)
            {

                NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 A1 01 02"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //稀释液
            else if (cmbASPara.SelectedIndex == 3)
            {
                sendMonitor(NetCom3.Cover("EB 90 02 A1 01 03"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //R3
            else if (cmbASPara.SelectedIndex == 4)
            {
                sendMonitor(NetCom3.Cover("EB 90 02 A1 01 04"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //R2
            else if (cmbASPara.SelectedIndex == 5)
            {
                sendMonitor(NetCom3.Cover("EB 90 02 A1 01 05"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //R1
            else if (cmbASPara.SelectedIndex == 6)
            {
                sendMonitor(NetCom3.Cover("EB 90 02 A1 01 06"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //磁珠
            else if (cmbASPara.SelectedIndex == 7)
            {
                sendMonitor(NetCom3.Cover("EB 90 02 A1 01 07"), 5);
                NetCom3.Instance.SingleQuery();
            }
            cmbASPara.Enabled = true;

            UpdateSampleSteps();
        }

        /// <summary>
        /// 更新加样机步数
        /// </summary>
        private void UpdateSampleSteps() 
        {
            if (cmbASPara.SelectedIndex < 0 || cmbASElecMachine.SelectedIndex < 0) return;
            txtASCurrentPos.Text =
                GetCurrentSteps(cmbASPara.SelectedIndex.ToString().PadLeft(2, '0'),
                (cmbASElecMachine.SelectedIndex + 1).ToString().PadLeft(2, '0'), "0206");
        }

        /// <summary>
        /// 获取步数
        /// </summary>
        /// <param name="parameter">参数名</param>
        /// <param name="machinery">电机</param>
        /// <param name="module">模块标志</param>
        /// <returns></returns>
        private string GetCurrentSteps(string parameter, string machinery, string module)
        {
            if (string.IsNullOrEmpty(parameter) ||
                string.IsNullOrEmpty(machinery)) return string.Empty;

            string steps = string.Empty;
            List<string> results = GetResults(module);

            string stepContent = string.Empty;
            foreach (var item in results)
            {
                if (item.Substring(18).Trim().StartsWith(parameter + " " + machinery))
                {
                    stepContent = item.Substring(24).Trim();
                    break;
                }
            }

            StringBuilder tempSteps = new StringBuilder();
            if (stepContent != string.Empty)
            {
                foreach (var item in stepContent.Split(' '))
                {
                    if (item != "00")
                    {
                        tempSteps.Append(item);
                    }
                }
            }

            if (string.IsNullOrEmpty(tempSteps.ToString())) return tempSteps.ToString();

            return Convert.ToInt64(tempSteps.ToString(), 16).ToString();
        }
        private string GetCurrentWashSteps(string parameter, string machinery, string module)
        {
            if (string.IsNullOrEmpty(parameter) ||
                string.IsNullOrEmpty(machinery)) return string.Empty;

            string steps = string.Empty;
            List<string> results = GetResults(module);

            string stepContent = string.Empty;
            foreach (var item in results)
            {
                if (item.Substring(18).Trim().StartsWith(parameter + " " + machinery))
                {
                    stepContent = item.Substring(24).Trim();
                    break;
                }
            }

            StringBuilder tempSteps = new StringBuilder();
            if (stepContent != string.Empty)
            {
                if ((parameter == "02") && (machinery == "03"))
                {
                    string[] temp=stepContent.Split(' ');

                    for (int i = 0; i < temp.Length; i++)
                    {
                        if (i == 0 || i == 1) continue;
                        if (temp[i] != "00") 
                        {
                            tempSteps.Append(temp[i]);
                        }
                    }
                    goto result;
                }
                foreach (var item in stepContent.Split(' '))
                {
                    if (item != "00")
                    {
                        tempSteps.Append(item);
                    }
                }
            }

            result:
            if (string.IsNullOrEmpty(tempSteps.ToString())) return tempSteps.ToString();

            return Convert.ToInt64(tempSteps.ToString(), 16).ToString();
        }
        /// <summary>
        /// 返回配置文件中当前模块所有数据
        /// </summary>
        /// <param name="module">模块标志</param>
        /// <returns>当前模块数据信息</returns>
        private List<string> GetResults(string module)
        {
            List<string> results = new List<string>();
            if (module == "0206")
            {
                if (GetData("02") != null)
                    results.AddRange(GetData("02"));

                if (GetData("06") != null)
                    results.AddRange(GetData("06"));
            }

            if (module == "01")
            {
                if (GetData("01") != null)
                    results.AddRange(GetData("01"));
            }

            if (module == "04")
            {
                if (GetData("04") != null)
                    results.AddRange(GetData("04"));
            }

            if (module == "03")
            {
                if (GetData("03") != null)
                    results.AddRange(GetData("03"));
            }

            return results;
        }
        private void fbtnASAdd_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbASElecMachine.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择需调试的电机！");
                return;
            }
            if (txtASIncrem.Text.Trim() == "")
            {
                frmMsgShow.MessageShow("仪器调试", "请输入增量值！");
                return;
            }
            fbtnASAdd.Enabled = false;
            string incream = int.Parse(txtASIncrem.Text.Trim()).ToString("x8");
            if (cmbASElecMachine.SelectedIndex == 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 01 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbASElecMachine.SelectedIndex == 1)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 02 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                   + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbASElecMachine.SelectedIndex == 2)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 03 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbASElecMachine.SelectedIndex == 3)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 04 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                   + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            fbtnASAdd.Enabled = true;
        }

        private void fbtnASSub_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbASElecMachine.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择需调试的电机！");
                return;
            }
            if (txtASIncrem.Text.Trim() == "")
            {
                frmMsgShow.MessageShow("仪器调试", "请输入增量值！");
                return;
            }
            fbtnASSub.Enabled = false;
            string incream = int.Parse("-" + txtASIncrem.Text.Trim()).ToString("x8");
            if (cmbASElecMachine.SelectedIndex == 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 01 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbASElecMachine.SelectedIndex == 1)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 02 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                   + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbASElecMachine.SelectedIndex == 2)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 03 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbASElecMachine.SelectedIndex == 3)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 04 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                   + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            fbtnASSub.Enabled = true;
        }

        private void fbtnASSave_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbASElecMachine.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择需调试的电机！");
                return;
            }
            fbtnASSave.Enabled = false;
            if (cmbASElecMachine.SelectedIndex == 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 01 13"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbASElecMachine.SelectedIndex == 1)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 02 13"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbASElecMachine.SelectedIndex == 2)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 03 13"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbASElecMachine.SelectedIndex == 3)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 04 13"), 5);
                NetCom3.Instance.SingleQuery();
            }
            NetCom3.Delay(500);//lyq add 20201016 指令返回太快，增加一个点击停顿
            fbtnASSave.Enabled = true;

            UpdateSampleSteps();
        }

        private void fbtnAsZReset_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            fbtnAsZReset.Enabled = false;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 02 00"), 5);
            NetCom3.Instance.SingleQuery();
            fbtnAsZReset.Enabled = true;
        }

        private void fbtnAsXReset_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            fbtnAsXReset.Enabled = false;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 01 00"), 5);
            NetCom3.Instance.SingleQuery();
            fbtnAsXReset.Enabled = true;
        }

        private void fbtnAsAllReset_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            fbtnAsAllReset.Enabled = false;
            sendMonitor(NetCom3.Cover("EB 90 02 00"), 5);
            NetCom3.Instance.SingleQuery();
            fbtnAsAllReset.Enabled = true;

            UpdateSampleSteps();
        }
        private void fbtnSamReset_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            fbtnSamReset.Enabled = false;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 03 00"), 5);
            NetCom3.Instance.SingleQuery();
            fbtnSamReset.Enabled = true;
        }

        private void fbtnregentReset_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            fbtnregentReset.Enabled = false;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 04 00"), 5);
            NetCom3.Instance.SingleQuery();
            fbtnregentReset.Enabled = true;
        }

        private void fbtnAsMixArm_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            fbtnAsMixArm.Enabled = false;
            if (fbtnAsMixArm.Text == "混匀臂正转")
            {
                fbtnAsMixArm.Text = "混匀臂反转";
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 02 15 40"), 5);
                NetCom3.Instance.SingleQuery();

            }
            else
            {
                fbtnAsMixArm.Text = "混匀臂正转";
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 02 15 41"), 5);
                NetCom3.Instance.SingleQuery();

            }
            fbtnAsMixArm.Enabled = true;
        }

        private void fbtnAdArmZEx_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbAsArmZ.SelectedItem == null)
            {
                cmbAsArmZ.Focus();
                frmMsgShow.MessageShow("仪器调试", "请选择垂直轴轴动作！");
                return;

            }
            fbtnAdArmZEx.Enabled = false;
            if (cmbAsArmZ.SelectedIndex == 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 02 11 40"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbAsArmZ.SelectedIndex == 1)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 02 11 41"), 5);
                NetCom3.Instance.SingleQuery();
            }
            fbtnAdArmZEx.Enabled = true;
        }

        private void fbtnAsArmXEx_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbAsArmX.SelectedItem == null)
            {
                cmbAsArmX.Focus();
                frmMsgShow.MessageShow("仪器调试", "请选择旋转轴动作！");
                return;
            }
            fbtnAsArmXEx.Enabled = false;
            if (cmbAsArmX.SelectedIndex == 0)//温育盘
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 02 12 00"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbAsArmX.SelectedIndex == 1)//洗针
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 02 12 01"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbAsArmX.SelectedIndex == 2)//S
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 02 12 02"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbAsArmX.SelectedIndex == 3)//S
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 02 12 03"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbAsArmX.SelectedIndex == 4)//R3
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 02 12 04"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbAsArmX.SelectedIndex == 5)//R2
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 02 12 05"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbAsArmX.SelectedIndex == 6)//R1
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 02 12 06"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbAsArmX.SelectedIndex == 7)//磁珠
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 02 12 07"), 5);
                NetCom3.Instance.SingleQuery();
            }
            fbtnAsArmXEx.Enabled = true;
        }

        private void btnRegentTrayEx_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (txtHoleNum.Text.Trim() == "")
            {
                frmMsgShow.MessageShow("仪器调试", "请输入孔号");
                txtHoleNum.Focus();
                return;
            }
            if (cmbRegentTray.SelectedItem == null)
            {
                cmbRegentTray.Focus();
                frmMsgShow.MessageShow("仪器调试", "请选择试剂样本盘动作！");
                return;
            }
            btnRegentTrayEx.Enabled = false;
            int holeNum = int.Parse(txtHoleNum.Text);
            if (cmbRegentTray.SelectedIndex == 0)//样本位置
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 02 13 02 " + holeNum.ToString("x2")), 5);
                NetCom3.Instance.SingleQuery();
            }
            if (cmbRegentTray.SelectedIndex == 1)//稀释液位置
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 02 13 03 " + holeNum.ToString("x2")), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbRegentTray.SelectedIndex == 2)//试剂3位置
            {

                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 02 13 04 " + holeNum.ToString("x2")), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbRegentTray.SelectedIndex == 3)//试剂2位置
            {

                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 02 13 05 " + holeNum.ToString("x2")), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbRegentTray.SelectedIndex == 4)//试剂3位置
            {

                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 02 13 06 " + holeNum.ToString("x2")), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbRegentTray.SelectedIndex == 5)//磁珠位置
            {

                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 02 13 07 " + holeNum.ToString("x2")), 5);
                NetCom3.Instance.SingleQuery();
            }
            btnRegentTrayEx.Enabled = true;
        }
        private void fbtnAsPumpEx_Click(object sender, EventArgs e)//this function modify by y in 20180510
        {

            if (txtAsPumpValue.Text.Trim() == "")
            {
                frmMsgShow.MessageShow("仪器调试", "请输入柱塞泵数值！");
                txtAsPumpValue.Focus();
                return;
            }
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            //if (cmbAsPump.SelectedItem == null)
            //{
            //    cmbAsPump.Focus();
            //    frmMsgShow.MessageShow("仪器调试", "请选择柱塞泵动作！");
            //    return;
            //}
            fbtnAsPumpEx.Enabled = false;
            //if (cmbAsPump.SelectedIndex == 0)
            //{
            string temp = int.Parse(txtAsPumpValue.Text).ToString("x4");
            string temp1 = temp.Substring(0, 2);
            string temp2 = temp.Substring(2, 2);
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 02 14 " + temp1 + " " + temp2 + " 00"), 5);
            NetCom3.Instance.SingleQuery();
            //}
            //else
            //{
            //    NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 02 14 00 " + int.Parse(txtAsPumpValue.Text).ToString("x2")), 5);
            //    NetCom3.Instance.SingleQuery();
            //}
            fbtnAsPumpEx.Enabled = true;
        }
        #endregion

        #region 基础性能
        private void tabControlMy1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlMy1.SelectedIndex == 6)
            {
                //cmbWashSubPipe.SelectedIndex = 0;
                //cmbTestSubPipe.SelectedIndex = 0;
                if (!NetCom3.totalOrderFlag)
                {
                    frmMsgShow.MessageShow("仪器调试", "仪器正在运行，请稍后切换！");
                    return;
                }
            }
            else
            {
                if (runFlag)
                {
                    frmMsgShow.MessageShow("仪器调试", "正在执行其他操作，请稍后！");
                    return;
                }
            }

        }
        private void fbtnPerfusionStart_Click(object sender, EventArgs e)
        {
            if (runFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "正在执行其他操作，请稍后！");
                return;
            }
            runFlag = true;
            fbtnPerfusionStart.Enabled = false;
            txtWashTestShow.Clear();
            if (rdbAddSample.Checked)
            {
                #region 加样管路灌注
                txtWashTestShow.AppendText("正在进行加样管路灌注.." + Environment.NewLine + Environment.NewLine);
                int addsamPerfuseNum = int.Parse(txtAddSample.Text.Trim());

                DateTime dtime = DateTime.Now.AddDays(-1);   //lyq add 20190903
                while (addsamPerfuseNum > 0)
                {
                    //lyq add 20190903
                    double dti = DateTime.Now.Subtract(dtime).TotalMilliseconds;  //收到返回指令 与 下一次发送指令的时间差
                    while (dti < 5000)   //收发指令间隔5s
                    {
                        NetCom3.Delay(100);
                        dti = DateTime.Now.Subtract(dtime).TotalMilliseconds;
                    }

                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 08"), 0);
                    if (!NetCom3.Instance.SPQuery())
                    {
                        break;
                    }
                    dtime = DateTime.Now;  // lyq add 20190903 收到返回指令的时间

                    addsamPerfuseNum--;
                    txtAddSample.Text = addsamPerfuseNum.ToString();
                }
                txtWashTestShow.AppendText("加样管路灌注完成" + Environment.NewLine + Environment.NewLine);
                #endregion
            }
            //2019.5.15  hly add
            else if (rdbSubstrate.Checked)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 F1 02"), 5);
                if (!NetCom3.Instance.SingleQuery())
                {
                    return;
                }
                currentHoleNum = int.Parse(OperateIniFile.ReadInIPara("OtherPara", "washCurrentHoleNum"));
                #region 底物管路灌注
                txtWashTestShow.AppendText("正在清空清洗盘.." + Environment.NewLine + Environment.NewLine);
                if (!washTrayTubeClear())
                {
                    runFlag = false;
                    fbtnPerfusionStart.Enabled = true;
                    return;
                }
                txtWashTestShow.AppendText("清洗盘清空完成.." + Environment.NewLine + Environment.NewLine);
                txtWashTestShow.AppendText("正在进行底物管路灌注..." + Environment.NewLine + Environment.NewLine);
                int SubstratePerfuseNum = int.Parse(txtSubstrate.Text.Trim());
                int pos1 = 19;
                string subPipe = "0";
                string LeftCount1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "", iniPathSubstrateTube);
                int leftcount1 = int.Parse(LeftCount1);
                if (leftcount1 < SubstratePerfuseNum)
                {
                    frmMsgShow.MessageShow("仪器调试", "底物不足！");
                    runFlag = false;
                    fbtnPerfusionStart.Enabled = true;
                    return;
                }
                #region 清洗盘顺时针18位，然后放管
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (1 - pos1).ToString("X2").Substring(6, 2)), 2);
                if (!NetCom3.Instance.WashQuery())
                {
                    fbtnPerfusionStart.Enabled = true;
                    runFlag = false;
                    return;
                }
                currentHoleNum = currentHoleNum + (pos1 - 1);
                if (currentHoleNum > 30)
                {
                    currentHoleNum = currentHoleNum - 30;
                }
                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                //夹新管到清洗盘
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06 "), 1);
                if (!NetCom3.Instance.MoveQuery())
                {
                    fbtnPerfusionStart.Enabled = true;
                    return;
                }
                #region 取放管成功，相关配置文件修改
                List<int> lisTubeNum = new List<int>();
                lisTubeNum = QueryTubeNum();
                OperateIniFile.WriteIniData("TubePosition", "No19", "1", iniPathWashTrayInfo);
                #endregion
                #endregion
                #region 清洗盘逆时针旋转19位，回到19号孔，加底物位置
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (pos1).ToString("X2")), 2);
                if (!NetCom3.Instance.WashQuery())
                {
                    fbtnPerfusionStart.Enabled = true;
                    return;
                }

                currentHoleNum = currentHoleNum - pos1 + 1;
                //若当前管号等于0，说明转过来的孔号为30
                if (currentHoleNum > 30)
                {
                    currentHoleNum = currentHoleNum - 30;
                }
                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                #endregion
                while (SubstratePerfuseNum > 0)
                {
                    #region 底物注液
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 01 " + subPipe + "0"), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        runFlag = false;
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    if (subPipe == "1")
                    {
                        leftcount1 = leftcount1 - 1;
                        OperateIniFile.WriteIniData("Substrate1", "LeftCount", leftcount1.ToString(), iniPathSubstrateTube);
                    }
                    //else
                    //{
                    //    leftcount2 = leftcount2 - 1;
                    //    OperateIniFile.WriteIniData("Substrate1", "LeftCount", leftcount2.ToString(), iniPathSubstrateTube);
                    //}
                    #endregion
                    #region 清洗盘顺时针旋转2位，底物吸液位置
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-2).ToString("X2").Substring(6, 2)), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    currentHoleNum = currentHoleNum + 2;
                    //若当前管号等于0，说明转过来的孔号为30
                    if (currentHoleNum > 30)
                    {
                        currentHoleNum = currentHoleNum - 30;
                    }
                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                    #region 吸液
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 01"), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    #endregion
                    #endregion
                    #region 清洗盘转回底物注液位置，19号位
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (2).ToString("X2")), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }

                    currentHoleNum = currentHoleNum - 2;
                    //若当前管号等于0，说明转过来的孔号为30
                    if (currentHoleNum <= 0)
                    {
                        currentHoleNum = currentHoleNum + 30;
                    }
                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                    #endregion
                    SubstratePerfuseNum--;
                    txtSubstrate.Text = SubstratePerfuseNum.ToString();
                }
                #region 扔废管
                #region 清洗盘顺时针旋转10位扔注液3位置反应管
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-pos1).ToString("X2").Substring(6, 2)), 2);
                if (!NetCom3.Instance.WashQuery())
                {
                    runFlag = false;
                    fbtnPerfusionStart.Enabled = true;
                    return;
                }
                currentHoleNum = currentHoleNum + 13;
                if (currentHoleNum > 30)
                {
                    currentHoleNum = currentHoleNum - 30;
                }
                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
                if (!NetCom3.Instance.MoveQuery())
                {
                    runFlag = false;
                    fbtnPerfusionStart.Enabled = true;
                    return;
                }
                OperateIniFile.WriteIniData("TubePosition", "No19", "0", iniPathWashTrayInfo);
                #endregion
                #endregion
                txtWashTestShow.AppendText("底物管路灌注完成..." + Environment.NewLine + Environment.NewLine);
                #endregion
            }
            //2019.5.15  hly add
            else if (rdbWash.Checked)
            {
                #region 清洗管路灌注
                if (cmbwash.SelectedItem == null)
                {
                    frmMsgShow.MessageShow("仪器调试", "请选择清洗管路");
                    runFlag = false;
                    fbtnPerfusionStart.Enabled = true;
                    return;
                }
                txtWashTestShow.AppendText("正在清空清洗盘.." + Environment.NewLine + Environment.NewLine);
                if (!washTrayTubeClear())
                {
                    runFlag = false;
                    fbtnPerfusionStart.Enabled = true;
                    return;
                }
                txtWashTestShow.AppendText("清洗盘清空完成.." + Environment.NewLine + Environment.NewLine);
                txtWashTestShow.AppendText("正在进行清洗管路灌注.." + Environment.NewLine + Environment.NewLine);
                int washPerfuseNum = int.Parse(txtWash.Text.Trim());
                int pos1 = 6;
                int pos2 = 10;
                int pos3 = 14;
                int[] LiquidInjectionFlag = new int[3];
                if (cmbwash.SelectedItem.ToString() == "1")
                {
                    #region 注液1位置放管
                    //NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-6).ToString("X2").Substring(6, 2)), 2);
                    //if (!NetCom3.Instance.WashQuery())
                    //{
                    //    fbtnPerfusionStart.Enabled = true;
                    //    return;
                    //}
                    //currentHoleNum = currentHoleNum - (1 - pos1);
                    //if (currentHoleNum <= 0)
                    //{
                    //    currentHoleNum = 30 + currentHoleNum;
                    //}
                    //OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());

                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06 "), 1);
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        runFlag = false;
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    #region 取放管成功，相关配置文件修改
                    List<int> lisTubeNum = new List<int>();
                    lisTubeNum = QueryTubeNum();
                    OperateIniFile.WriteIniData("TubePosition", "No6", "1", iniPathWashTrayInfo);
                    #endregion
                    #endregion
                    #region 放好管之后逆时针返回
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01" + (pos1).ToString("X2")), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        runFlag = false;
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    currentHoleNum = currentHoleNum - 5;
                    if (currentHoleNum <= 0)
                    {
                        currentHoleNum = currentHoleNum + 30;
                    }
                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                    #endregion
                    LiquidInjectionFlag[0] = 1;
                }
                if (cmbwash.SelectedItem.ToString() == "2")
                {
                    #region 注液2位置放管
                    //NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (pos1 - pos2).ToString("X2").Substring(6, 2)), 2);
                    //if (!NetCom3.Instance.WashQuery())
                    //{
                    //    fbtnPerfusionStart.Enabled = true;
                    //    return;
                    //}
                    //currentHoleNum = currentHoleNum - (pos1 - pos2);
                    //if (currentHoleNum <= 0)
                    //{
                    //    currentHoleNum = 30 + currentHoleNum;
                    //}
                    //OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());

                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06 "), 1);
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        runFlag = false;
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    #region 取放管成功 相关配置文件修改
                    List<int> lisTubeNum = new List<int>();
                    lisTubeNum = QueryTubeNum();
                    OperateIniFile.WriteIniData("TubePosition", "No10", "1", iniPathWashTrayInfo);
                    #endregion
                    #endregion
                    #region 放好管之后逆时针返回
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01" + (pos2).ToString("X2")), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        runFlag = false;
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    currentHoleNum = currentHoleNum - 9;
                    if (currentHoleNum <= 0)
                    {
                        currentHoleNum = currentHoleNum + 30;
                    }
                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                    #endregion
                    LiquidInjectionFlag[1] = 1;
                }
                if (cmbwash.SelectedItem.ToString() == "3")
                {
                    #region 注液3位置放管
                    //NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (pos2 - pos3).ToString("X2").Substring(6, 2)), 2);
                    //if (!NetCom3.Instance.WashQuery())
                    //{
                    //    fbtnPerfusionStart.Enabled = true;
                    //    return;
                    //}
                    //currentHoleNum = currentHoleNum - (pos2 - pos3);
                    //if (currentHoleNum <= 0)
                    //{
                    //    currentHoleNum = 30 + currentHoleNum;
                    //}
                    //OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());

                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06 "), 1);
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        runFlag = false;
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    #region 取放管成功 相关配置文件修改
                    List<int> lisTubeNum = new List<int>();
                    lisTubeNum = QueryTubeNum();
                    OperateIniFile.WriteIniData("TubePosition", "No14", "1", iniPathWashTrayInfo);
                    #endregion
                    #endregion
                    #region 放好管之后逆时针返回
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01" + (pos3).ToString("X2")), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        runFlag = false;
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    currentHoleNum = currentHoleNum - 13;
                    if (currentHoleNum <= 0)
                    {
                        currentHoleNum = currentHoleNum + 30;
                    }
                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                    #endregion
                    LiquidInjectionFlag[2] = 1;
                }
                if (cmbwash.SelectedItem.ToString() == "全部")
                {
                    #region 注液位置放管
                    if (!AddTubeInCleanTray())
                    {
                        runFlag = false;
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    CleanTrayMovePace(pos3 - pos2);
                    if (!AddTubeInCleanTray())
                    {
                        runFlag = false;
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    CleanTrayMovePace(pos2 - pos1);
                    if (!AddTubeInCleanTray())
                    {
                        runFlag = false;
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    CleanTrayMovePace(pos1);
                    LiquidInjectionFlag[0] = LiquidInjectionFlag[1] = LiquidInjectionFlag[2] = 1;
                    #endregion
                }
                while (washPerfuseNum > 0)
                {
                    #region 注液 
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 " + LiquidInjectionFlag[0] + "" + LiquidInjectionFlag[1] + " " + LiquidInjectionFlag[2] + "0"), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        runFlag = false;
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    #endregion
                    #region 清洗盘顺时针旋转1位
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-1).ToString("X2").Substring(6, 2)), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        runFlag = false;
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    currentHoleNum = currentHoleNum + 1;
                    if (currentHoleNum > 30)
                    {
                        currentHoleNum = currentHoleNum - 30;
                    }
                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());

                    #region 吸液
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 01"), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        runFlag = false;
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    #endregion
                    #endregion
                    #region 清洗盘逆时针转回注液位置
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (1).ToString("X2")), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        runFlag = false;
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    currentHoleNum = currentHoleNum - 1;
                    if (currentHoleNum <= 0)
                    {
                        currentHoleNum = currentHoleNum + 30;
                    }
                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                    #endregion
                    washPerfuseNum--;
                    txtWash.Text = washPerfuseNum.ToString();
                }
                if (cmbwash.SelectedItem.ToString() == "1")
                {
                    #region 扔废管
                    #region 清洗盘顺时针旋转6位扔注液3位置反应管
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-6).ToString("X2").Substring(6, 2)), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        runFlag = false;
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    currentHoleNum = currentHoleNum + 13;
                    if (currentHoleNum > 30)
                    {
                        currentHoleNum = currentHoleNum - 30;
                    }
                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        runFlag = false;
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    OperateIniFile.WriteIniData("TubePosition", "No6", "0", iniPathWashTrayInfo);
                    #endregion
                    #endregion
                }
                if (cmbwash.SelectedItem.ToString() == "2")
                {
                    #region 扔废管
                    #region 清洗盘顺时针旋转10位扔注液3位置反应管
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-10).ToString("X2").Substring(6, 2)), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        runFlag = false;
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    currentHoleNum = currentHoleNum + 13;
                    if (currentHoleNum > 30)
                    {
                        currentHoleNum = currentHoleNum - 30;
                    }
                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        runFlag = false;
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    OperateIniFile.WriteIniData("TubePosition", "No10", "0", iniPathWashTrayInfo);
                    #endregion
                    #endregion
                }
                if (cmbwash.SelectedItem.ToString() == "3")
                {
                    #region 扔废管
                    #region 清洗盘顺时针旋转14位扔注液3位置反应管
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-14).ToString("X2").Substring(6, 2)), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        runFlag = false;
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    currentHoleNum = currentHoleNum + 13;
                    if (currentHoleNum > 30)
                    {
                        currentHoleNum = currentHoleNum - 30;
                    }
                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        runFlag = false;
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    OperateIniFile.WriteIniData("TubePosition", "No14", "0", iniPathWashTrayInfo);
                    #endregion
                    #endregion
                }
                if (cmbwash.SelectedItem.ToString() == "全部")
                {
                    CleanTrayMovePace(-pos1);
                    if (!MoveWashTube())
                    {
                        runFlag = false;
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    CleanTrayMovePace(pos1 - pos2);
                    if (!MoveWashTube())
                    {
                        runFlag = false;
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    CleanTrayMovePace(pos2 - pos3);
                    if (!MoveWashTube())
                    {
                        runFlag = false;
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                }
                txtWashTestShow.AppendText("清洗管路灌注完成" + Environment.NewLine + Environment.NewLine);
                #endregion
            }
            else
            {
                frmMsgShow.MessageShow("仪器调试", "请选择需要灌注的管路。");
                runFlag = false;
                fbtnPerfusionStart.Enabled = true;
                return;
            }
            runFlag = false;
            fbtnPerfusionStart.Enabled = true;
        }
        // zlx Add 20190803 加空管到清洗盘取放管处
        private bool AddTubeInCleanTray(int pos = 0)
        {
            int iNeedCool = 0;
            int IsKnockedCool = 0;
            AgainNewMove:
            string order = "EB 90 31 01 06";
            NetCom3.Instance.Send(NetCom3.Cover(order), 1);
            if (!NetCom3.Instance.MoveQuery())
            {
                #region 发生异常处理
                if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull)
                {
                    iNeedCool++;
                    if (iNeedCool > 2)
                    {
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手抓新管多次抓空！实验将停止运行！");
                        DialogResult tempresult = frmMsgShow.MessageShow("移管手错误！", "移管手抓新管多次抓空！实验将停止运行！");
                        return false; ;
                    }
                    else
                    {
                        goto AgainNewMove;
                    }
                }
                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.LackTube)
                {
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "理杯机缺管！实验将停止运行！");
                    DialogResult tempresult = frmMsgShow.MessageShow("移管手错误！", "理杯机缺管！实验将停止运行！");
                    return false; ;
                }
                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.Sendfailure)
                {
                    if (NetCom3.Instance.waitAndAgainSend != null && NetCom3.Instance.waitAndAgainSend is Thread)
                    {
                        NetCom3.Instance.waitAndAgainSend.Abort();
                    }
                    goto AgainNewMove;
                }
                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked)
                {
                    IsKnockedCool++;
                    if (IsKnockedCool < 2)
                        goto AgainNewMove;
                    else
                    {
                        DialogResult tempresult = frmMsgShow.MessageShow("移管手错误", "移管手在管架向清洗盘取放管处抓管发生撞管，实验将进行停止！");
                        return false;
                    }

                }
                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.OverTime)
                {
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在向清洗盘抓新管接收数据超时，实验将停止！！");
                    DialogResult tempresult = frmMsgShow.MessageShow("移管手错误", "移管手在向清洗盘抓新管接收数据超时，实验将停止！");
                    return false;
                }
                #endregion
            }
            return true;
        }
        // zlx Add 20190803   旋转清洗盘相应空位，正数为逆时针旋转
        void CleanTrayMovePace(int pace)
        {
            DataTable dtWashTrayIni = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
            string order;
            if (pace > 0)
            {
                order = "EB 90 31 03 01 " + (pace).ToString("X2");
            }
            else if (pace < 0)
            {
                order = "EB 90 31 03 01 " + (pace).ToString("X2").Substring(6, 2);
            }
            else return;
            AgainNewMove:
            NetCom3.Instance.Send(NetCom3.Cover(order), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                if (NetCom3.Instance.WasherrorFlag == (int)ErrorState.Sendfailure)
                    goto AgainNewMove;
                else if (NetCom3.Instance.WasherrorFlag == (int)ErrorState.OverTime)
                {
                    NetCom3.Instance.stopsendFlag = true;
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "清洗指令接收指令超时！");
                    frmMsgShow.MessageShow("清洗指令错误提示", "指令接收超时，实验已终止");
                }
                else
                    return;

            }
            countWashHole(pace);
            currentHoleNum = currentHoleNum - 1;
            if (currentHoleNum > 30)
            {
                currentHoleNum = currentHoleNum - 30;
            }
            if (currentHoleNum <= 0)
            {
                currentHoleNum = currentHoleNum + 30;
            }
            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
            dtWashTrayIni = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
            DataTable dtTemp = new DataTable();
            dtTemp = dtWashTrayIni.Copy();
            for (int j = 1; j < 2; j++)
                dtWashTrayIni.Rows[dtWashTrayIni.Rows.Count - 1][j] = dtTemp.Rows[0][j];
            for (int k = 0; k < dtWashTrayIni.Rows.Count - 1; k++)
            {
                for (int j = 1; j < 2; j++)
                {
                    dtWashTrayIni.Rows[k][j] = dtTemp.Rows[k + 1][j];
                }
            }
            OperateIniFile.WriteConfigToFile("[TubePosition]", iniPathWashTrayInfo, dtWashTrayIni);
        }
        // zlx Add 20190803  清洗盘扔废管
        bool MoveWashTube()
        {
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
            if (!NetCom3.Instance.MoveQuery())
            {
                return false;
            }
            return true;
        }
        private void fbtnTestStart_Click(object sender, EventArgs e)
        {
            if (runFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "正在执行其他操作，请稍后！");
                return;
            }
            if (cmbTestChioce.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择需要进行的测试！");
                return;
            }

            string LeftCount1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "", iniPathSubstrateTube);
            if (int.Parse(txtTestSubNum.Text) > int.Parse(LeftCount1))
            {
                frmMsgShow.MessageShow("仪器调试", "底物不足！");
                return;
            }
            //if (cmbTestSubPipe.SelectedItem == null)
            //{
            //    frmMsgShow.MessageShow("仪器调试", "请选择底物管路！");
            //    return;
            //}
            txtWashTestShow.Clear();
            fbtnTestStart.Enabled = false;
            runFlag = true;
            //启动移管线程
            MoveTubeThread = new Thread(new ParameterizedThreadStart(MoveTube));
            MoveTubeThread.IsBackground = true;
            MoveTubeThread.Start();
            #region 变量初始化
            //查询反应盘管信息
            dtWashTrayInfo = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
            //样本位置
            string samPos = txtTestPos.Text.Trim();
            //测试次数
            int subNum = int.Parse(txtTestSubNum.Text.Trim());
            //加样体积
            string addSampleVol = txtTestSampleVol.Text.Trim();
            //底物体积
            string subVol = txtTestSubVol.Text.Trim();
            //加样位置
            List<int> lisPos = new List<int>();
            //开始位置索引
            int start = 0;
            #endregion
            #region 检测反应盘空白反应管个数，不足十个补齐。
            //查询反应盘管信息
            DataTable dtReactTrayInfo = OperateIniFile.ReadConfig(iniPathReactTrayInfo);
            //反应盘上空反应管个数
            int sumReactTubeNum = 0;
            //反应管的位置
            string TrayPos = "";

            for (int i = 0; i < dtReactTrayInfo.Rows.Count; i++)
            {
                if (dtReactTrayInfo.Rows[i][1].ToString() == "1")
                {
                    sumReactTubeNum++;
                    //后一个值一直覆盖前一个最终的赋值为最后一个位置
                    TrayPos = dtReactTrayInfo.Rows[i][0].ToString();
                }
            }
            //反应盘空反应管的个数小于10
            if (sumReactTubeNum < toUsedTube)
            {
                txtWashTestShow.AppendText("正在补充空白反应管.." + Environment.NewLine + Environment.NewLine);
                if (TrayPos == "")
                {
                    TrayPos = "NO0";
                }
                int LackTubeNum = toUsedTube - sumReactTubeNum;
                for (int i = 0; i < LackTubeNum; i++)
                {

                    MoveTubeStatus moveTube1 = new MoveTubeStatus();
                    moveTube1.StepNum = 0;
                    moveTube1.putTubePos = "1-" + (int.Parse(TrayPos.Substring(2)) + i + 1).ToString();
                    moveTube1.TestId = 0;
                    moveTube1.TakeTubePos = "0-1";
                    lisMoveTube.Add(moveTube1);
                    while (lisMoveTube.Count != 0)
                    {
                        NetCom3.Delay(10);
                    }

                }
                txtWashTestShow.AppendText("空白反应管补充完成。" + Environment.NewLine + Environment.NewLine);
            }
            #endregion
            if (cmbTestChioce.SelectedIndex == 0)
            {
                #region 加样重复性测试
                DataTable dt = new DataTable();
                dt.Columns.Add("ID", typeof(string));
                dt.Columns.Add("pmt", typeof(string));
                int reactStartPos = 1;
                dtReactTrayInfo = OperateIniFile.ReadConfig(iniPathReactTrayInfo);
                for (int i = 0; i < dtReactTrayInfo.Rows.Count; i++)
                {
                    if (dtReactTrayInfo.Rows[i][1].ToString() == "1")
                    {
                        reactStartPos = int.Parse(dtReactTrayInfo.Rows[i][0].ToString().Substring(2));
                        break;
                    }
                }
                //此次试验使用到的温育盘位置
                for (int i = 0; i < subNum; i++)
                {

                    lisPos.Add((reactStartPos + i) % ReactTrayHoleNum == 0 ? ReactTrayHoleNum : (reactStartPos + i) % ReactTrayHoleNum);
                }

                #region 加样
                while (start < subNum)
                {

                    int addsamplePos = lisPos[start];
                    txtWashTestShow.AppendText("正在取" + samPos.ToString() + "位置的样本加到温育盘" + addsamplePos.ToString() + "位置" + Environment.NewLine + Environment.NewLine);
                    ///在samPos位置吸取addSampleVol体积的样本加到反应盘addsamplePos位置
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 01 " + int.Parse(samPos).ToString("x2") + " " + addsamplePos.ToString("x2")
                                   + " " + int.Parse(addSampleVol).ToString("x2")), 0);
                    if (!NetCom3.Instance.SPQuery())
                    {
                        return;
                    }
                    OperateIniFile.WriteIniData("ReactTrayInfo", "no" + addsamplePos, "2", iniPathReactTrayInfo);
                    ///加样同时进行移管手从TubeRackPos夹管到温育盘
                    MoveTubeStatus moveTube2 = new MoveTubeStatus();
                    moveTube2.StepNum = 0;
                    int pos = (addsamplePos + toUsedTube) % ReactTrayHoleNum;
                    if (pos == 0)
                    {
                        moveTube2.putTubePos = "1-" + ReactTrayHoleNum.ToString();
                    }
                    else
                    {
                        moveTube2.putTubePos = "1-" + pos.ToString();
                    }
                    moveTube2.TestId = 0;
                    moveTube2.TakeTubePos = "0-" + OperateIniFile.ReadIniData("Tube", "TubePos", "1", iniPathSubstrateTube);
                    lisMoveTube.Add(moveTube2);
                    while (lisMoveTube.Count != 0)
                    {
                        NetCom3.Delay(10);
                    }
                    start++;
                }
                #endregion
                start = 0;

                //将温育盘的已经加样的管添加到清洗盘
                /*
                while (start < subNum)
                {
                    MoveTubeStatus moveTube3 = new MoveTubeStatus();
                    moveTube3.putTubePos = "2-1";
                    moveTube3.StepNum = 0;
                    //移管列表加样位置赋值给列表中的反应盘取样位置
                    int addSampos = lisPos[start];
                    moveTube3.TakeTubePos = "1-" + addSampos.ToString();
                    moveTube3.TestId = 0;
                    lisMoveTube.Add(moveTube3);
                    while (lisMoveTube.Count != 0)
                    {
                        NetCom3.Delay(10);
                    }
                    txtWashTestShow.AppendText("清洗盘旋转一位" + Environment.NewLine + Environment.NewLine);
                    ///清洗盘旋转一位(发送指令)
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        return;
                    }
                }
                int leftStartPos = 1;
                for (int i = 0; i < dtWashTrayInfo.Rows.Count; i++)
                {
                    if (dtWashTrayInfo.Rows[i][1].ToString() == "1")
                    {
                        leftStartPos = int.Parse(dtWashTrayInfo.Rows[i][0].ToString().Substring(2));

                    }
                }
               */

                if (subNum < 19)
                {
                    #region 当温育盘上的管夹到清洗盘还未到加底物位置时
                    while (start < subNum)
                    {
                        MoveTubeStatus moveTube3 = new MoveTubeStatus();
                        moveTube3.putTubePos = "2-1";
                        moveTube3.StepNum = 0;
                        //移管列表加样位置赋值给列表中的反应盘取样位置
                        int addSampos = lisPos[start];
                        moveTube3.TakeTubePos = "1-" + addSampos.ToString();
                        moveTube3.TestId = 0;
                        lisMoveTube.Add(moveTube3);
                        while (lisMoveTube.Count != 0)
                        {
                            NetCom3.Delay(10);
                        }
                        txtWashTestShow.AppendText("清洗盘旋转一位" + Environment.NewLine + Environment.NewLine);
                        ///清洗盘旋转一位(发送指令)
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                        if (!NetCom3.Instance.WashQuery())
                        {
                            return;
                        }
                        //旋转一位当前取放管位置的孔号加1
                        currentHoleNum++;
                        //如果孔号超过30，孔号设为1
                        if (currentHoleNum == 31)
                        {
                            currentHoleNum = 1;
                        }
                        LogFile.Instance.Write("==================  当前位置  " + currentHoleNum);
                        OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                        //查询清洗盘信息
                        dtWashTrayInfo = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
                        DataTable dtTemp = new DataTable();
                        dtTemp = dtWashTrayInfo.Copy();
                        //清洗盘状态列表中添加反应盘位置字段
                        dtWashTrayInfo.Rows[0][1] = dtTemp.Rows[dtWashTrayInfo.Rows.Count - 1][1];
                        for (int i = 1; i < dtWashTrayInfo.Rows.Count; i++)
                        {
                            dtWashTrayInfo.Rows[i][1] = dtTemp.Rows[i - 1][1];
                        }
                        OperateIniFile.WriteConfigToFile("[TubePosition]", iniPathWashTrayInfo, dtWashTrayInfo);
                        start++;
                    }
                    //开始位置
                    int leftStartPos = 1;
                    for (int i = 0; i < dtWashTrayInfo.Rows.Count; i++)
                    {
                        if (dtWashTrayInfo.Rows[i][1].ToString() == "1")
                        {
                            leftStartPos = int.Parse(dtWashTrayInfo.Rows[i][0].ToString().Substring(2));

                        }
                    }
                    ///清洗盘旋转18-leftStartPos位
                    txtWashTestShow.AppendText("清洗盘旋转" + (19 - leftStartPos).ToString() + "位。" + Environment.NewLine + Environment.NewLine);
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (19 - leftStartPos).ToString("x2")), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        return;
                    }
                    //旋转18-leftStartPos位当前取放管位置的孔号加1
                    currentHoleNum = currentHoleNum + 19 - leftStartPos;
                    //如果孔号超过30，孔号设为1
                    if (currentHoleNum > 30)
                    {
                        currentHoleNum = currentHoleNum - 30;
                    }
                    LogFile.Instance.Write("==================  当前位置  " + currentHoleNum);
                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                    #region 清洗盘逆时针旋转
                    DataTable dtTemp1 = new DataTable();
                    dtTemp1 = dtWashTrayInfo.Copy();
                    //清洗盘状态列表中添加反应盘位置字段
                    for (int i = 0; i < dtWashTrayInfo.Rows.Count; i++)
                    {
                        if (i - (19 - leftStartPos) < 0)
                        {
                            dtWashTrayInfo.Rows[i][1] = dtTemp1.Rows[i - 19 + leftStartPos + 30][1];
                        }
                        else
                        {
                            dtWashTrayInfo.Rows[i][1] = dtTemp1.Rows[i - 19 + leftStartPos][1];
                        }
                        //for (int j = 1; j < 2; j++)
                        //{
                        //    if (i - (18 - leftStartPos) < 0)
                        //    {
                        //        int temp = i;
                        //        temp = i - (19 - leftStartPos) + 30;
                        //        dtWashTrayInfo.Rows[i][j] = dtTemp1.Rows[temp][j];
                        //    }
                        //    else
                        //    {
                        //        dtWashTrayInfo.Rows[i][j] = dtTemp1.Rows[i - (19 - leftStartPos)+1][j];
                        //    }
                        //}
                    }
                    OperateIniFile.WriteConfigToFile("[TubePosition]", iniPathWashTrayInfo, dtWashTrayInfo);
                    #endregion
                    #endregion
                }
                else
                {
                    #region 夹管完成前面的反应管已加完底物
                    while (start < subNum)
                    {
                        //查询反应盘管信息
                        dtWashTrayInfo = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
                        MoveTubeStatus moveTube3 = new MoveTubeStatus();
                        moveTube3.putTubePos = "2-1";
                        moveTube3.StepNum = 0;
                        //移管列表加样位置赋值给列表中的反应盘取样位置
                        int addSampos = lisPos[start];
                        moveTube3.TakeTubePos = "1-" + addSampos.ToString();
                        moveTube3.TestId = 0;
                        lisMoveTube.Add(moveTube3);
                        while (lisMoveTube.Count != 0)
                        {
                            NetCom3.Delay(10);
                        }
                        txtWashTestShow.AppendText("清洗盘旋转一位" + Environment.NewLine + Environment.NewLine);
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                        if (!NetCom3.Instance.WashQuery())
                        {
                            return;
                        }
                        currentHoleNum = currentHoleNum + 1;
                        //如果孔号超过30，孔号设为1
                        if (currentHoleNum > 30)
                        {
                            currentHoleNum = currentHoleNum - 30;
                        }
                        LogFile.Instance.Write("==================  当前位置  " + currentHoleNum);
                        OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                        DataTable dtTemp = new DataTable();
                        dtTemp = dtWashTrayInfo.Copy();
                        //清洗盘状态列表中添加反应盘位置字段
                        dtWashTrayInfo.Rows[0][1] = dtTemp.Rows[dtWashTrayInfo.Rows.Count - 1][1];
                        for (int i = 1; i < dtWashTrayInfo.Rows.Count; i++)
                        {
                            dtWashTrayInfo.Rows[i][1] = dtTemp.Rows[i - 1][1];
                        }
                        OperateIniFile.WriteConfigToFile("[TubePosition]", iniPathWashTrayInfo, dtWashTrayInfo);
                        string readflag = "0";
                        string subFlag = "0";
                        string subpipe = "0";
                        if (dtWashTrayInfo.Rows[24][1].ToString() == "1")
                        {
                            readflag = "1";
                        }
                        if (dtWashTrayInfo.Rows[19][1].ToString() == "1")
                        {
                            subFlag = "1";
                        }
                        //if (cmbTestSubPipe.SelectedItem.ToString() == "1")
                        //{
                        //    subpipe = "1";
                        //}
                        //else if (cmbTestSubPipe.SelectedItem.ToString() == "2")
                        //{
                        //    subpipe = "2";
                        //}
                        subpipe = "1";
                        ///发送加底物和读数统一的指令
                        if (readflag == "1" || subFlag == "1")
                        {
                            if (readflag == "1")
                            {
                                txtWashTestShow.AppendText("正在读数" + Environment.NewLine + Environment.NewLine);
                                Random rd = new Random();
                                BackObj = rd.Next(1, 100).ToString();
                                dt.Rows.Add(start + 1, int.Parse(BackObj));
                                txtTestValue.Text = BackObj;
                            }
                            else if (subFlag == "1")
                            {
                                txtWashTestShow.AppendText("正在加底物" + Environment.NewLine + Environment.NewLine);
                            }
                            else
                            {
                                txtWashTestShow.AppendText("正在加底物和读数" + Environment.NewLine + Environment.NewLine);
                            }
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 0" + subFlag + " " + subpipe + readflag), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
                                return;
                            }
                        }
                        if (subFlag == "1")
                        {
                            LeftCount1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "", iniPathSubstrateTube);
                            OperateIniFile.WriteIniData("Substrate1", "LeftCount", (int.Parse(LeftCount1) - 1).ToString(), iniPathSubstrateTube);
                        }
                        if (dtWashTrayInfo.Rows[28][1].ToString() == "1")
                        {
                            MoveTubeStatus moveTube = new MoveTubeStatus();
                            //清洗盘扔废管
                            moveTube.putTubePos = "0-0";
                            moveTube.StepNum = 0;
                            moveTube.TakeTubePos = "2-" + dtWashTrayInfo.Rows[28][0].ToString().Substring(2);
                            moveTube.TestId = 0;
                            lisMoveTube.Add(moveTube);
                            moveTube = new MoveTubeStatus();
                            while (lisMoveTube.Count != 0)
                            {
                                NetCom3.Delay(10);
                            }
                        }

                        start++;
                    }
                    #endregion
                }

                while (WashExitsTube())
                {
                    #region 清洗盘中存在反应管
                    if (lisMoveTube.Count == 0)
                        Thread.Sleep(300);
                    txtWashTestShow.AppendText("清洗盘旋转一位" + Environment.NewLine + Environment.NewLine);        ///清洗盘旋转一位
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        return;
                    }
                    currentHoleNum = currentHoleNum + 1;
                    //如果孔号超过30，孔号设为1
                    if (currentHoleNum > 30)
                    {
                        currentHoleNum = currentHoleNum - 30;
                    }
                    LogFile.Instance.Write("==================  当前位置  " + currentHoleNum);
                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                    //查询反应盘管信息
                    DataTable dtTemp = new DataTable();
                    dtTemp = dtWashTrayInfo.Copy();
                    //清洗盘状态列表中添加反应盘位置字段
                    dtWashTrayInfo.Rows[0][1] = dtTemp.Rows[dtWashTrayInfo.Rows.Count - 1][1];
                    for (int i = 1; i < dtWashTrayInfo.Rows.Count; i++)
                    {
                        dtWashTrayInfo.Rows[i][1] = dtTemp.Rows[i - 1][1];
                    }
                    OperateIniFile.WriteConfigToFile("[TubePosition]", iniPathWashTrayInfo, dtWashTrayInfo);
                    string readflag = "0";
                    string subFlag = "0";
                    string subpipe = "0";
                    if (dtWashTrayInfo.Rows[24][1].ToString() == "1")
                    {
                        readflag = "1";
                    }
                    if (dtWashTrayInfo.Rows[19][1].ToString() == "1")
                    {
                        subFlag = "1";
                    }
                    //if (cmbTestSubPipe.SelectedItem.ToString() == "1")
                    //{
                    //    subpipe = "1";
                    //}
                    //else if (cmbTestSubPipe.SelectedItem.ToString() == "2")
                    //{
                    //    subpipe = "2";
                    //}
                    subpipe = "1";
                    ///发送加底物和读数统一的指令
                    if (readflag == "1" || subFlag == "1")
                    {
                        if (readflag == "1")
                        {
                            txtWashTestShow.AppendText("正在读数" + Environment.NewLine + Environment.NewLine);
                            Random rd = new Random();
                            BackObj = rd.Next(1, 100).ToString();
                            dt.Rows.Add(++start, int.Parse(BackObj));
                            txtTestValue.Text = BackObj;
                        }
                        else if (subFlag == "1")
                        {
                            txtWashTestShow.AppendText("正在加底物" + Environment.NewLine + Environment.NewLine);
                        }
                        else
                        {
                            txtWashTestShow.AppendText("正在加底物和读数" + Environment.NewLine + Environment.NewLine);
                        }
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 0" + subFlag + " " + subpipe + readflag), 2);
                        if (!NetCom3.Instance.WashQuery())
                        {
                            return;
                        }
                    }
                    if (dtWashTrayInfo.Rows[28][1].ToString() == "1")
                    {
                        MoveTubeStatus moveTube = new MoveTubeStatus();
                        //清洗盘扔废管
                        moveTube.putTubePos = "0-0";
                        moveTube.StepNum = 0;
                        moveTube.TakeTubePos = "2-" + dtWashTrayInfo.Rows[28][0].ToString().Substring(2);
                        moveTube.TestId = 0;
                        lisMoveTube.Add(moveTube);
                        moveTube = new MoveTubeStatus();
                        while (lisMoveTube.Count != 0)
                        {
                            NetCom3.Delay(10);
                        }

                    }
                    #endregion
                }
                double avg = cmd.AVERAGE(dt);
                double sd = cmd.STDEV(dt);
                double cv = cmd.CV(dt);
                txtTestAV.Text = avg.ToString();
                txtTestSD.Text = sd.ToString();
                txtTestCV.Text = cv.ToString();
                dt.Rows.Add("Avg", txtTestAV.Text.Trim());
                dt.Rows.Add("SD", txtTestSD.Text.Trim());
                dt.Rows.Add("CV", txtTestCV.Text.Trim());
                #region 数据导出
                if (chbTestExport.Checked)
                {
                    string filePath = "";
                    if (cmbTestChioce.SelectedIndex == 0)
                    {
                        filePath = Application.StartupPath + @"\仪器调试\基础性能\加样重复性测试\AddSampleTest_" + DateTime.Now.ToString("yyyyMMdd HHmmss") + ".xls";
                    }
                    else
                    {
                        filePath = Application.StartupPath + @"\仪器调试\基础性能\本底测试\SubstrateTest_" + DateTime.Now.ToString("yyyyMMdd HHmmss") + ".xls";
                    }
                    DataTableExcel.TableToExcel(dt, filePath);
                }
                #endregion
                #endregion
            }
            else
            {
                #region 本底测试
                DataTable dt = new DataTable();
                dt.Columns.Add("ID", typeof(int));
                dt.Columns.Add("pmt", typeof(int));
                dtReactTrayInfo = OperateIniFile.ReadConfig(iniPathReactTrayInfo);
                if (subNum < 19)
                {
                    #region 当管架管夹到清洗盘还未到加底物位置时
                    while (start < subNum)
                    {
                        //int takePos = int.Parse(OperateIniFile.ReadIniData("Tube", "TubePos", "1", iniPathSubstrateTube)) + start;
                        //int plate = takePos % 88 == 0 ? takePos / 88 - 1 : takePos / 88;//几号板
                        //int column = takePos % 11 == 0 ? takePos / 11 - (plate * 8) : takePos / 11 + 1 - (plate * 8);
                        //int hole = takePos % 11 == 0 ? 11 : takePos % 11;
                        txtWashTestShow.AppendText("管架夹管到清洗盘。" + Environment.NewLine + Environment.NewLine);
                        ///从管架takePos位置夹管到清洗盘
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06"), 1);
                        if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                        {
                            return;
                        }
                        #region 取放管成功配置文件修改
                        /*
                        List<int> lisTubeNum = new List<int>();
                        lisTubeNum = QueryTubeNum();
                        //移管手要夹的下一个管架位置
                        int NextPos = takePos + 1;
                        //管架中第一个装载管架的索引
                        int firstTubeIndex = lisTubeNum.FindIndex(ty => ty <= 88 && ty > 0);
                        for (int i = 1; i <= lisTubeNum.Count; i++)
                        {
                            if (NextPos == i * 88 + 1)
                            {
                                NextPos = firstTubeIndex * 88 + (88 - lisTubeNum[firstTubeIndex]) + 1;
                            }
                        }
                        OperateIniFile.WriteIniData("Tube", "TubePos", NextPos.ToString(), iniPathSubstrateTube);
                        int TubeRack = takePos / 88;
                        int curTube = takePos % 88;
                        if (curTube == 0 && takePos != 0)
                        {
                            TubeRack = TubeRack - 1;
                            curTube = 88;
                        }
                        //那个架子减了一个管
                        OperateIniFile.WriteIniData("Tube", "Pos" + (TubeRack + 1).ToString(), (88 - curTube).ToString(), iniPathSubstrateTube);
                        
                         */
                        //清洗盘配置文件更新
                        OperateIniFile.WriteIniData("TubePosition", "No1", "1", iniPathWashTrayInfo);
                        dtWashTrayInfo.Rows[0][1] = "1";
                        #endregion
                        txtWashTestShow.AppendText("清洗盘旋转一位" + Environment.NewLine + Environment.NewLine);
                        ///清洗盘旋转一位
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                        if (!NetCom3.Instance.WashQuery())
                        {
                            return;
                        }
                        currentHoleNum = currentHoleNum + 1;
                        //如果孔号超过30，孔号设为1
                        if (currentHoleNum > 30)
                        {
                            currentHoleNum = currentHoleNum - 30;
                        }
                        LogFile.Instance.Write("==================  当前位置  " + currentHoleNum);
                        OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                        //查询反应盘管信息
                        dtWashTrayInfo = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
                        DataTable dtTemp = new DataTable();
                        dtTemp = dtWashTrayInfo.Copy();
                        //清洗盘状态列表中添加反应盘位置字段
                        dtWashTrayInfo.Rows[0][1] = dtTemp.Rows[dtWashTrayInfo.Rows.Count - 1][1];
                        for (int i = 1; i < dtWashTrayInfo.Rows.Count; i++)
                        {
                            dtWashTrayInfo.Rows[i][1] = dtTemp.Rows[i - 1][1];
                        }
                        OperateIniFile.WriteConfigToFile("[TubePosition]", iniPathWashTrayInfo, dtWashTrayInfo);
                        start++;
                    }
                    //开始位置
                    int leftStartPos = 1;
                    for (int i = 0; i < dtWashTrayInfo.Rows.Count; i++)
                    {
                        if (dtWashTrayInfo.Rows[i][1].ToString() == "1")
                        {
                            leftStartPos = int.Parse(dtWashTrayInfo.Rows[i][0].ToString().Substring(2));
                        }
                    }
                    txtWashTestShow.AppendText("清洗盘旋转" + (19 - leftStartPos).ToString() + "位" + Environment.NewLine + Environment.NewLine);
                    ///清洗盘旋转18-leftStartPos位
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (19 - leftStartPos).ToString("x2")), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        return;
                    }
                    currentHoleNum = currentHoleNum + 19 - leftStartPos;
                    //如果孔号超过30，孔号设为1
                    if (currentHoleNum > 30)
                    {
                        currentHoleNum = currentHoleNum - 30;
                    }
                    LogFile.Instance.Write("==================  当前位置  " + currentHoleNum);
                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                    washTrayFlag = false;
                    #region 清洗盘逆时针旋转
                    DataTable dtTemp1 = new DataTable();
                    dtTemp1 = dtWashTrayInfo.Copy();
                    //清洗盘状态列表中添加反应盘位置字段
                    for (int i = 0; i < dtWashTrayInfo.Rows.Count; i++)
                    {
                        for (int j = 1; j < 2; j++)
                        {
                            if (i - (19 - leftStartPos) < 0)
                            {
                                int temp = i;
                                temp = i - (19 - leftStartPos) + 30;
                                dtWashTrayInfo.Rows[i][j] = dtTemp1.Rows[temp][j];
                            }
                            else
                            {
                                dtWashTrayInfo.Rows[i][j] = dtTemp1.Rows[i - (19 - leftStartPos)][j];
                            }
                        }
                    }
                    OperateIniFile.WriteConfigToFile("[TubePosition]", iniPathWashTrayInfo, dtWashTrayInfo);
                    #endregion

                    #endregion
                }
                else
                {
                    #region 夹管完成前面的反应管已加完底物
                    while (start < subNum)
                    {
                        txtWashTestShow.AppendText("管架夹管到清洗盘。" + Environment.NewLine + Environment.NewLine);
                        ///从管架takePos位置夹管到清洗盘
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06"), 1);
                        if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                        {
                            return;
                        }
                        OperateIniFile.WriteIniData("TubePosition", "No1", "1", iniPathWashTrayInfo);
                        dtWashTrayInfo.Rows[0][1] = "1";
                        //MoveTubeStatus moveTube3 = new MoveTubeStatus();
                        //moveTube3.putTubePos = "2-1";
                        //moveTube3.StepNum = 0;
                        ////移管列表加样位置赋值给列表中的反应盘取样位置
                        //int addSampos = lisPos[start];
                        //moveTube3.TakeTubePos = "1-" + addSampos.ToString();
                        //moveTube3.TestId = 0;
                        //lisMoveTube.Add(moveTube3);
                        //while (lisMoveTube.Count != 0)
                        //{
                        //    NetCom3.Delay(10);
                        //}
                        txtWashTestShow.AppendText("清洗盘旋转一位" + Environment.NewLine + Environment.NewLine);
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                        if (!NetCom3.Instance.WashQuery())
                        {
                            return;
                        }
                        currentHoleNum = currentHoleNum + 1;
                        //如果孔号超过30，孔号设为1
                        if (currentHoleNum > 30)
                        {
                            currentHoleNum = currentHoleNum - 30;
                        }
                        LogFile.Instance.Write("==================  当前位置  " + currentHoleNum);
                        OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                        //查询反应盘管信息
                        dtWashTrayInfo = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
                        DataTable dtTemp = new DataTable();
                        dtTemp = dtWashTrayInfo.Copy();
                        //清洗盘状态列表中添加反应盘位置字段
                        dtWashTrayInfo.Rows[0][1] = dtTemp.Rows[dtWashTrayInfo.Rows.Count - 1][1];
                        for (int i = 1; i < dtWashTrayInfo.Rows.Count; i++)
                        {
                            dtWashTrayInfo.Rows[i][1] = dtTemp.Rows[i - 1][1];
                        }
                        OperateIniFile.WriteConfigToFile("[TubePosition]", iniPathWashTrayInfo, dtWashTrayInfo);
                        string readflag = "0";
                        string subFlag = "0";
                        string subpipe = "0";
                        if (dtWashTrayInfo.Rows[24][1].ToString() == "1")
                        {
                            readflag = "1";
                        }
                        if (dtWashTrayInfo.Rows[19][1].ToString() == "1")
                        {
                            subFlag = "1";
                        }
                        //if (cmbTestSubPipe.SelectedItem.ToString() == "1")
                        //{
                        //    subpipe = "1";
                        //}
                        //else if (cmbTestSubPipe.SelectedItem.ToString() == "2")
                        //{
                        //    subpipe = "2";
                        //}
                        subpipe = "1";
                        ///发送加底物和读数统一的指令
                        if (readflag == "1" || subFlag == "1")
                        {
                            if (readflag == "1")
                            {
                                txtWashTestShow.AppendText("正在读数" + Environment.NewLine + Environment.NewLine);
                                Random rd = new Random();
                                BackObj = rd.Next(1, 100).ToString();
                                dt.Rows.Add(++start, int.Parse(BackObj));
                                txtTestValue.Text = BackObj;
                            }
                            else if (subFlag == "1")
                            {
                                txtWashTestShow.AppendText("正在加底物" + Environment.NewLine + Environment.NewLine);
                            }
                            else
                            {
                                txtWashTestShow.AppendText("正在加底物和读数" + Environment.NewLine + Environment.NewLine);
                            }
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 0" + subFlag + " " + subpipe + readflag), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
                                return;
                            }
                        }
                        if (dtWashTrayInfo.Rows[28][1].ToString() == "1")
                        {
                            MoveTubeStatus moveTube = new MoveTubeStatus();
                            //清洗盘扔废管
                            moveTube.putTubePos = "0-0";
                            moveTube.StepNum = 0;
                            moveTube.TakeTubePos = "2-" + dtWashTrayInfo.Rows[25][0].ToString().Substring(2);
                            moveTube.TestId = 0;
                            lisMoveTube.Add(moveTube);
                            moveTube = new MoveTubeStatus();
                            while (lisMoveTube.Count != 0)
                            {
                                NetCom3.Delay(10);
                            }
                        }
                        start++;
                    }
                    #endregion
                }
                start = 0;
                while (WashExitsTube())
                {
                    #region 清洗盘中存在反应管

                    txtWashTestShow.AppendText("清洗盘旋转一位" + Environment.NewLine + Environment.NewLine);        ///清洗盘旋转一位
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        return;
                    }
                    currentHoleNum = currentHoleNum + 1;
                    //如果孔号超过30，孔号设为1
                    if (currentHoleNum > 30)
                    {
                        currentHoleNum = currentHoleNum - 30;
                    }
                    LogFile.Instance.Write("==================  当前位置  " + currentHoleNum);
                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                    //查询反应盘管信息
                    DataTable dtTemp = new DataTable();
                    dtTemp = dtWashTrayInfo.Copy();
                    //清洗盘状态列表中添加反应盘位置字段
                    dtWashTrayInfo.Rows[0][1] = dtTemp.Rows[dtWashTrayInfo.Rows.Count - 1][1];
                    for (int i = 1; i < dtWashTrayInfo.Rows.Count; i++)
                    {
                        dtWashTrayInfo.Rows[i][1] = dtTemp.Rows[i - 1][1];
                    }
                    OperateIniFile.WriteConfigToFile("[TubePosition]", iniPathWashTrayInfo, dtWashTrayInfo);
                    string readflag = "0";
                    string subFlag = "0";
                    string subpipe = "0";
                    if (dtWashTrayInfo.Rows[24][1].ToString() == "1")
                    {
                        readflag = "1";
                    }
                    if (dtWashTrayInfo.Rows[19][1].ToString() == "1")
                    {
                        subFlag = "1";
                    }
                    //if (cmbTestSubPipe.SelectedItem.ToString() == "1")
                    //{
                    //    subpipe = "1";
                    //}
                    //else if (cmbTestSubPipe.SelectedItem.ToString() == "2")
                    //{
                    //    subpipe = "2";
                    //}
                    subpipe = "1";
                    ///发送加底物和读数统一的指令
                    if (readflag == "1" || subFlag == "1")
                    {
                        if (readflag == "1")
                        {
                            txtWashTestShow.AppendText("正在读数" + Environment.NewLine + Environment.NewLine);
                            Random rd = new Random();
                            BackObj = rd.Next(1, 100).ToString();
                            dt.Rows.Add(++start, int.Parse(BackObj));
                            txtTestValue.Text = BackObj;
                            readflag = "0";
                        }
                        else if (subFlag == "1")
                        {
                            txtWashTestShow.AppendText("正在加底物" + Environment.NewLine + Environment.NewLine);
                            //subFlag = "0";
                            subpipe = "1";
                        }
                        else
                        {
                            txtWashTestShow.AppendText("正在加底物和读数" + Environment.NewLine + Environment.NewLine);
                        }
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 0" + subFlag + " " + subpipe + readflag), 2);
                        if (!NetCom3.Instance.WashQuery())
                        {
                            return;
                        }
                    }
                    if (dtWashTrayInfo.Rows[28][1].ToString() == "1")
                    {
                        MoveTubeStatus moveTube = new MoveTubeStatus();
                        //清洗盘扔废管
                        moveTube.putTubePos = "0-0";
                        moveTube.StepNum = 0;
                        moveTube.TakeTubePos = "2-" + dtWashTrayInfo.Rows[28][0].ToString().Substring(2);
                        moveTube.TestId = 0;
                        lisMoveTube.Add(moveTube);
                        moveTube = new MoveTubeStatus();
                        while (lisMoveTube.Count != 0)
                        {
                            NetCom3.Delay(10);
                        }
                    }
                    #endregion
                }
                double avg = cmd.AVERAGE(dt);
                double sd = cmd.STDEV(dt);
                double cv = cmd.CV(dt);
                txtTestAV.Text = avg.ToString();
                txtTestSD.Text = sd.ToString();
                txtTestCV.Text = cv.ToString();
                #region 数据导出
                if (chbTestExport.Checked)
                {
                    string filePath = "";
                    if (cmbTestChioce.SelectedIndex == 0)
                    {
                        filePath = Application.StartupPath + @"\仪器调试\基础性能\加样重复性测试\AddSampleTest_" + DateTime.Now.ToString("yyyyMMdd HHmmss") + ".xls";
                    }
                    else
                    {
                        filePath = Application.StartupPath + @"\仪器调试\基础性能\本底测试\SubstrateTest_" + DateTime.Now.ToString("yyyyMMdd HHmmss") + ".xls";
                    }
                    DataTableExcel.TableToExcel(dt, filePath);
                }
                #endregion
                #endregion
            }

            runFlag = false;
            fbtnTestStart.Enabled = true;
        }
        /// <summary>
        /// 管架下一个夹管位置
        /// </summary>
        /// <param name="CurrentPos">当前夹管位置</param>
        /// <returns></returns>
        int NextPos(string CurrentPos)
        {
            List<int> lisTubeNum = new List<int>();
            lisTubeNum = QueryTubeNum();
            //移管手要夹的下一个管架位置
            int NextPos = int.Parse(CurrentPos) + 1;
            //管架中第一个装载管架的索引
            int firstTubeIndex = lisTubeNum.FindIndex(ty => ty <= 88 && ty > 0);
            for (int i = 1; i <= lisTubeNum.Count; i++)
            {
                if (NextPos == i * 88 + 1)
                {
                    NextPos = firstTubeIndex * 88 + (88 - lisTubeNum[firstTubeIndex]) + 1;
                }
            }
            return NextPos;
        }
        /// <summary>
        /// 查询四个管架中管的个数
        /// </summary>
        /// <returns></returns>
        List<int> QueryTubeNum()
        {
            List<int> lisTubeNum = new List<int>();
            lisTubeNum.Add(int.Parse(OperateIniFile.ReadIniData("Tube", "Pos1", "", iniPathSubstrateTube)));
            lisTubeNum.Add(int.Parse(OperateIniFile.ReadIniData("Tube", "Pos2", "", iniPathSubstrateTube)));
            lisTubeNum.Add(int.Parse(OperateIniFile.ReadIniData("Tube", "Pos3", "", iniPathSubstrateTube)));
            lisTubeNum.Add(int.Parse(OperateIniFile.ReadIniData("Tube", "Pos4", "", iniPathSubstrateTube)));
            return lisTubeNum;


        }
        /// <summary>
        /// 判断清洗盘是否存在管
        /// </summary>
        /// <returns></returns>
        bool WashExitsTube()
        {
            DataTable tempdtWashTrayInfo = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
            for (int i = 0; i < tempdtWashTrayInfo.Rows.Count; i++)
            {
                if (tempdtWashTrayInfo.Rows[i][1].ToString() == "1")
                {
                    return true;
                }
            }
            return false;
        }
        private void fbtnWashTestStart_Click(object sender, EventArgs e)
        {
            if (runFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "正在执行其他操作，请稍后！");
                return;
            }
            //if (cmbWashSubPipe.SelectedItem == null)
            //{
            //    frmMsgShow.MessageShow("仪器调试", "请选择底物管路。");
            //    return;
            //}

            if (txtWashPosStart.Text.Trim() == "")
            {
                frmMsgShow.MessageShow("仪器调试", "请输入样本号开始位置！");
                txtWashPosEnd.Focus();
                return;
            }
            if (txtWashPosEnd.Text.Trim() == "")
            {
                frmMsgShow.MessageShow("仪器调试", "请输入样本号结束位置！");
                txtWashPosEnd.Focus();
                return;
            }
            if (int.Parse(txtWashPosStart.Text) > int.Parse(txtWashPosEnd.Text))
            {
                frmMsgShow.MessageShow("仪器调试", "样本号开始位置不能大于样本号结束位置。");
                txtWashPosEnd.Focus();
                return;
            }
            #region 变量取值
            //取样开始位置
            int startSamPos = int.Parse(txtWashPosStart.Text.Trim());
            //取样结束位置
            int endSampos = int.Parse(txtWashPosEnd.Text.Trim());
            //重复次数
            int RepeatNum = int.Parse(txtWashRepeatNum.Text.Trim());
            //底物量
            string subVol = txtWashSubVol.Text.Trim();
            //底物管路
            //string subPipe = cmbWashSubPipe.SelectedItem.ToString();
            //样本量
            string samVol = txtWashSamVol.Text.Trim();
            //存储清洗测试的信息
            List<performanceTest> lisPt = new List<performanceTest>();
            performanceTest pt = new performanceTest();
            //序号
            int id = 1;
            //加样位置
            int reactStartPos = 1;

            //开始变量
            int start = 0;
            #endregion
            if ((endSampos - startSamPos) * RepeatNum > ReactTrayNum)
            {
                frmMsgShow.MessageShow("仪器调试", "测试次数不能超过" + ReactTrayNum + "，请重新输入！");
                return;
            }
            string LeftCount1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "", iniPathSubstrateTube);
            if (int.Parse(LeftCount1) < ((endSampos - startSamPos) * RepeatNum))
            {
                frmMsgShow.MessageShow("仪器调试", "底物不足，请核实后继续");
                return;
            }
            txtWashTestShow.Clear();
            fbtnWashTestStart.Enabled = false;
            runFlag = true;
            //启动移管线程
            MoveTubeThread = new Thread(new ParameterizedThreadStart(MoveTube));
            MoveTubeThread.IsBackground = true;
            MoveTubeThread.Start();
            washTrayTubeClear();
            #region 检测反应盘空白反应管个数，不足十个补齐。
            //查询反应盘管信息
            DataTable dtReactTrayInfo = OperateIniFile.ReadConfig(iniPathReactTrayInfo);
            //反应盘上空反应管个数
            int sumReactTubeNum = 0;
            //反应管的位置
            string TrayPos = "";

            for (int i = 0; i < dtReactTrayInfo.Rows.Count; i++)
            {
                if (!runFlag) return;
                if (dtReactTrayInfo.Rows[i][1].ToString() == "1")
                {
                    sumReactTubeNum++;
                    //后一个值一直覆盖前一个最终的赋值为最后一个位置
                    TrayPos = dtReactTrayInfo.Rows[i][0].ToString();
                }
            }
            //反应盘空反应管的个数小于10
            if (sumReactTubeNum < toUsedTube)
            {
                if (TrayPos == "")
                {
                    TrayPos = "NO0";
                }
                int LackTubeNum = toUsedTube - sumReactTubeNum;
                for (int i = 0; i < LackTubeNum; i++)
                {
                    if (!runFlag) return;
                    MoveTubeStatus moveTube1 = new MoveTubeStatus();
                    moveTube1.StepNum = 0;
                    moveTube1.putTubePos = "1-" + (int.Parse(TrayPos.Substring(2)) + i + 1).ToString();
                    moveTube1.TestId = 0;
                    moveTube1.TakeTubePos = "0-1";
                    lisMoveTube.Add(moveTube1);
                    while (lisMoveTube.Count != 0)
                    {
                        NetCom3.Delay(10);
                    }

                }

            }
            #endregion
            dtReactTrayInfo = OperateIniFile.ReadConfig(iniPathReactTrayInfo);
            for (int i = 0; i < dtReactTrayInfo.Rows.Count; i++)
            {
                if (!runFlag) return;
                if (dtReactTrayInfo.Rows[i][1].ToString() == "1")
                {
                    reactStartPos = int.Parse(dtReactTrayInfo.Rows[i][0].ToString().Substring(2));
                    break;
                }
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("pmt", typeof(string));
            txtWashTestShow.AppendText("清洗测试开始。。" + Environment.NewLine + Environment.NewLine);
            //清洗信息赋值
            for (int i = startSamPos; i <= endSampos; i++)
            {
                if (!runFlag) return;
                for (int j = 1; j <= RepeatNum; j++)
                {
                    pt.takeSamplePos = i;
                    pt.ID = id++;
                    pt.addSamplePos = reactStartPos % ReactTrayHoleNum == 0 ? ReactTrayHoleNum : reactStartPos % ReactTrayHoleNum;
                    lisPt.Add(pt);
                    pt = new performanceTest();
                    reactStartPos++;
                    if (reactStartPos == ReactTrayNum)
                        reactStartPos = 1;
                }
            }
            #region 加样
            while (start < lisPt.Count && runFlag)
            {
                if (!runFlag) return;
                int addsamplePos = lisPt[start].addSamplePos;
                int samPos = lisPt[start].takeSamplePos;
                txtWashTestShow.AppendText(lisPt[start].ID + "号反应管正在加样" + Environment.NewLine + Environment.NewLine);
                ///在samPos位置吸取addSampleVol体积的样本加到反应盘addsamplePos位置 
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 01 " + samPos.ToString("x2") + " " + addsamplePos.ToString("x2")
                                 + " " + int.Parse(samVol).ToString("x2")), 0);
                if (!NetCom3.Instance.SPQuery())
                {
                    return;
                }
                //加样同时进行移管手从TubeRackPos夹管到温育盘               
                MoveTubeStatus moveTube2 = new MoveTubeStatus();
                moveTube2.StepNum = 0;
                int pos = (addsamplePos + toUsedTube) % ReactTrayHoleNum;
                if (pos == 0)
                {
                    moveTube2.putTubePos = "1-" + ReactTrayHoleNum.ToString();
                }
                else
                {
                    moveTube2.putTubePos = "1-" + pos.ToString();
                }
                moveTube2.TestId = 0;
                moveTube2.TakeTubePos = "0-" + OperateIniFile.ReadIniData("Tube", "TubePos", "1", iniPathSubstrateTube);
                lisMoveTube.Add(moveTube2);
                while (lisMoveTube.Count != 0)
                {
                    NetCom3.Delay(10);
                }
                start++;
            }
            #endregion
            start = 0;
            #region 取管到清洗盘清洗、加底物及读数
            while (start < lisPt.Count && runFlag)
            {
                MoveTubeStatus moveTube3 = new MoveTubeStatus();
                moveTube3.putTubePos = "2-1";
                moveTube3.StepNum = 0;
                //移管列表加样位置赋值给列表中的反应盘取样位置
                int addSampos = lisPt[start].addSamplePos;
                moveTube3.TakeTubePos = "1-" + addSampos.ToString();
                moveTube3.TestId = 0;
                lisMoveTube.Add(moveTube3);
                while (lisMoveTube.Count > 0)
                {
                    NetCom3.Delay(10);
                }
                do
                {
                    txtWashTestShow.AppendText("清洗盘旋转1位" + Environment.NewLine + Environment.NewLine);
                    ///清洗盘旋转一位
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        return;
                    }
                    currentHoleNum = currentHoleNum + 1;
                    //如果孔号超过30，孔号设为1
                    if (currentHoleNum > 30)
                    {
                        currentHoleNum = currentHoleNum - 30;
                    }
                    LogFile.Instance.Write("==================  当前位置  " + currentHoleNum);
                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                    //查询清洗盘信息
                    dtWashTrayInfo = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
                    DataTable dtTemp = new DataTable();
                    dtTemp = dtWashTrayInfo.Copy();
                    //清洗盘状态列表中添加反应盘位置字段
                    dtWashTrayInfo.Rows[0][1] = dtTemp.Rows[dtWashTrayInfo.Rows.Count - 1][1];
                    for (int i = 1; i < dtWashTrayInfo.Rows.Count; i++)
                    {
                        dtWashTrayInfo.Rows[i][1] = dtTemp.Rows[i - 1][1];
                    }
                    OperateIniFile.WriteConfigToFile("[TubePosition]", iniPathWashTrayInfo, dtWashTrayInfo);
                    //是否进行读数
                    string read = "0";
                    //注液标志位
                    List<string> LiquidInjectionFlag = new List<string>();
                    //吸液标志位
                    List<int> ImbibitionFlag = new List<int>();
                    string Imbibition = "00";
                    //是否加底物标志位
                    string AddSubstrate = "0";
                    //底物管路
                    string substratePipe = "0";
                    //判断吸液位置是否有管
                    if (dtWashTrayInfo.Rows[5][1].ToString() == "1" || dtWashTrayInfo.Rows[9][1].ToString() == "1"
                        || dtWashTrayInfo.Rows[13][1].ToString() == "1" || dtWashTrayInfo.Rows[17][1].ToString() == "1")
                    {
                        Imbibition = "01";
                    }
                    else
                    {
                        Imbibition = "00";
                    }
                    //判断第一次注液位置是否有管
                    if (dtWashTrayInfo.Rows[6][1].ToString() == "1")
                    {
                        LiquidInjectionFlag.Add("1");
                    }
                    else
                    {
                        LiquidInjectionFlag.Add("0");
                    }
                    //判断第二次注液位置是否有管
                    if (dtWashTrayInfo.Rows[10][1].ToString() == "1")
                    {
                        LiquidInjectionFlag.Add("1");
                    }
                    else
                    {
                        LiquidInjectionFlag.Add("0");
                    }
                    //判断第三次注液位置是否有管
                    if (dtWashTrayInfo.Rows[14][1].ToString() == "1")
                    {
                        LiquidInjectionFlag.Add("1");
                    }
                    else
                    {
                        LiquidInjectionFlag.Add("0");
                    }
                    //加底物位置是否有管
                    if (dtWashTrayInfo.Rows[19][1].ToString() == "1")
                    {
                        AddSubstrate = "1";
                    }
                    //读数位置是否有管
                    if (dtWashTrayInfo.Rows[24][1].ToString() == "1")
                    {
                        read = "1";
                    }
                    //吸液、注液或者加底物的位置下面有反应管
                    if (Imbibition == "01" || LiquidInjectionFlag.Contains("1") || AddSubstrate == "1" || read == "1")
                    {
                        //if (AddSubstrate == "1")
                        //{
                        //    if (cmbWashSubPipe.SelectedItem.ToString() == "1")
                        //    {
                        //        substratePipe = "1";
                        //    }
                        //    else if (cmbWashSubPipe.SelectedItem.ToString() == "2")
                        //    {
                        //        substratePipe = "2";
                        //    }

                        //}
                        substratePipe = "1";
                        txtWashTestShow.AppendText("正在清洗、加底物或读数" + Environment.NewLine + Environment.NewLine);
                        ///发送指令
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 " + Imbibition + " " + LiquidInjectionFlag[0]
                        + LiquidInjectionFlag[1] + " " + LiquidInjectionFlag[2] + AddSubstrate + " " + substratePipe + read), 2);
                        if (!NetCom3.Instance.WashQuery())
                        {
                            return;
                        }

                    }
                    if (AddSubstrate == "1")
                    {
                        LeftCount1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "", iniPathSubstrateTube);
                        OperateIniFile.WriteIniData("Substrate1", "LeftCount", (int.Parse(LeftCount1) - 1).ToString(), iniPathSubstrateTube);
                    }
                    if (read == "1")
                    {
                        Random rd = new Random();
                        BackObj = rd.Next(1, 100).ToString();
                        dt.Rows.Add(lisPt[start].ID, int.Parse(BackObj));
                        txtTestValue.Text = BackObj;
                    }
                    if (dtWashTrayInfo.Rows[28][1].ToString() == "1")
                    {
                        MoveTubeStatus moveTube = new MoveTubeStatus();
                        //清洗盘扔废管
                        moveTube.putTubePos = "0-0";
                        moveTube.StepNum = 0;
                        moveTube.TakeTubePos = "2-" + dtWashTrayInfo.Rows[28][0].ToString().Substring(2);
                        moveTube.TestId = 0;
                        lisMoveTube.Add(moveTube);
                        moveTube = new MoveTubeStatus();
                        while (lisMoveTube.Count > 0)
                        {
                            NetCom3.Delay(10);
                        }
                    }
                    //start++;

                } while (WashExitsTube());

                start++;
            }


            #region 数据导出
            if (chbWashExport.Checked)
            {
                string filePath = Application.StartupPath + @"\仪器调试\基础性能\清洗测试\WashTest_" + DateTime.Now.ToString("yyyyMMdd HHmmss") + ".xls";

                DataTableExcel.TableToExcel(dt, filePath);
            }
            #endregion
            #endregion
            while (lisMoveTube.Count > 0)
            {
                NetCom3.Delay(10);
            }
            runFlag = false;
            fbtnWashTestStart.Enabled = true;
        }

        private void cmbTestChioce_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTestChioce.SelectedIndex == 0)
            {
                lblTestSamPos.Visible = true;
                txtTestPos.Visible = true;
                lblTestSamVol.Visible = true;
                txtTestSampleVol.Visible = true;
            }
            else
            {
                lblTestSamPos.Visible = false;
                txtTestPos.Visible = false;
                lblTestSamVol.Visible = false;
                txtTestSampleVol.Visible = false;
            }
        }

        /// <summary>
        /// 清洗盘取放管方法
        /// </summary>
        /// <param name="TubeWashInfo">无用</param>
        void MoveTube(object TubeInfo)
        {

            while (runFlag)
            {
                again:
                //CanCom.Instance.ComWait.WaitOne();
                Thread.Sleep(1);
                if (lisMoveTube.Count > 0)
                {
                    MoveTubeStatus TempMoveStatus = lisMoveTube[0];
                    if (TempMoveStatus == null)
                    {
                        goto again;
                    }
                    string[] takepos = TempMoveStatus.TakeTubePos.Split('-');
                    string[] putpos = TempMoveStatus.putTubePos.Split('-');
                    if (takepos[0] == "0" && MoveTubeUseFlag == false)//管架取管
                    {
                        if (putpos[0] == "1")
                        {
                            ///到管架takepos[1]位置取管放到温育盘putpos位置
                            #region 发送指令及配置文件的实时更改（管架夹新管到反应盘）
                            MoveTubeUseFlag = true;
                            //移管手到取管位置takepos[1]（管架）取管
                            //int plate = int.Parse(takepos[1]) % 88 == 0 ? int.Parse(takepos[1]) / 88 - 1 : int.Parse(takepos[1]) / 88;//几号板
                            //int column = int.Parse(takepos[1]) % 11 == 0 ? int.Parse(takepos[1]) / 11 - (plate * 8) : int.Parse(takepos[1]) / 11 + 1 - (plate * 8);
                            //int hole = int.Parse(takepos[1]) % 11 == 0 ? 11 : int.Parse(takepos[1]) % 11;
                            BeginInvoke(new Action(() =>
                            {
                                txtWashTestShow.AppendText("管架夹管到温育盘。" + Environment.NewLine + Environment.NewLine);
                            }));
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 01 " + int.Parse(putpos[1]).ToString("x2")), 1);
                            if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                            {
                                return;
                            }
                            #region 取管成功
                            /*
                            List<int> lisTubeNum = new List<int>();
                            lisTubeNum = QueryTubeNum();
                            //移管手要夹的下一个管架位置
                            int NextPos = int.Parse(takepos[1]) + 1;
                            //管架中第一个装载管架的索引
                            int firstTubeIndex = lisTubeNum.FindIndex(ty => ty <= 88 && ty > 0);
                            for (int i = 1; i <= lisTubeNum.Count; i++)
                            {
                                if (NextPos == i * 88 + 1)
                                {
                                    NextPos = firstTubeIndex * 88 + (88 - lisTubeNum[firstTubeIndex]) + 1;
                                }
                            }
                            OperateIniFile.WriteIniData("Tube", "TubePos", NextPos.ToString(), iniPathSubstrateTube);
                            int TubeRack = (int.Parse(takepos[1])) / 88;
                            int curTube = (int.Parse(takepos[1])) % 88;
                            if (curTube == 0 && int.Parse(takepos[1]) != 0)
                            {
                                TubeRack = TubeRack - 1;
                                curTube = 88;
                            }
                            //那个架子减了一个管
                            OperateIniFile.WriteIniData("Tube", "Pos" + (TubeRack + 1).ToString(), (88 - curTube).ToString(), iniPathSubstrateTube);
                            */
                            //修改反应盘信息
                            OperateIniFile.WriteIniData("ReactTrayInfo", "no" + putpos[1], "1", iniPathReactTrayInfo);
                            #endregion

                        }
                        lisMoveTube.Remove(TempMoveStatus);
                        MoveTubeUseFlag = false;
                        #endregion

                    }
                    //清洗盘夹管
                    else if (takepos[0] == "2")
                    {
                        //到废弃处
                        if (putpos[0] == "0" && MoveTubeUseFlag == false && !washTrayFlag)
                        {
                            #region 指令发送及相关配置文件更新（清洗盘扔废管）
                            washTrayFlag = true;
                            MoveTubeUseFlag = true;
                            BeginInvoke(new Action(() =>
                            {
                                txtWashTestShow.AppendText("清洗盘扔废管。" + Environment.NewLine + Environment.NewLine);
                            }));
                            ///清洗盘takepos[1]取管扔废管
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 01"), 1);
                            if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                            {
                                return;
                            }
                            #region 取管成功
                            OperateIniFile.WriteIniData("TubePosition", "no" + takepos[1], "0", iniPathWashTrayInfo);
                            LogFile.Instance.Write("==============  " + currentHoleNum + "  扔管");
                            LogFile.Instance.Write("==============  " + takepos[1] + "  扔管");
                            if (dtWashTrayInfo.Rows.Count > 0)
                            {
                                dtWashTrayInfo.Rows[28][1] = "0";

                            }
                            #endregion
                            lisMoveTube.Remove(TempMoveStatus);
                            MoveTubeUseFlag = false;
                            washTrayFlag = false;
                            #endregion

                        }

                    }
                    //反应盘扔废管
                    else if (takepos[0] == "1" && putpos[0] == "0" && MoveTubeUseFlag == false)
                    {
                        #region 指令发送及文件修改（反应盘扔废管）
                        MoveTubeUseFlag = true;
                        BeginInvoke(new Action(() =>
                        {
                            txtWashTestShow.AppendText("温育盘扔废管。" + Environment.NewLine + Environment.NewLine);
                        }));
                        ///温育盘takepos[1]取管扔废管
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 " + int.Parse(takepos[1]).ToString("x2")), 1);
                        if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                        {
                            return;
                        }
                        ///取管成功
                        OperateIniFile.WriteIniData("ReactTrayInfo", "no" + takepos[1], "0", iniPathReactTrayInfo);

                        #endregion
                        lisMoveTube.Remove(TempMoveStatus);
                        MoveTubeUseFlag = false;
                    }
                    //反应盘夹管到清洗盘
                    else if (takepos[0] == "1" && putpos[0] == "2"
                          && MoveTubeUseFlag == false && washTrayFlag == false)///反应盘标志位是否不用判断
                    {
                        #region 指令发送及文件修改（反应盘夹管到清洗盘）
                        MoveTubeUseFlag = true;
                        washTrayFlag = true;
                        #region 指令发送及文件修改（反应盘夹管到清洗盘）
                        BeginInvoke(new Action(() =>
                        {
                            txtWashTestShow.AppendText("温育盘夹管到清洗盘。" + Environment.NewLine + Environment.NewLine);
                        }));
                        ///从反应盘takepos[1]位取管放到清洗盘putpos[1]位置
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 02 " + int.Parse(takepos[1]).ToString("x2")), 1);
                        if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                        {
                            return;
                        }
                        #region 取放管成功
                        OperateIniFile.WriteIniData("ReactTrayInfo", "no" + takepos[1], "0", iniPathReactTrayInfo);
                        OperateIniFile.WriteIniData("TubePosition", "No1", "1", iniPathWashTrayInfo);
                        LogFile.Instance.Write("==============  反应盘向清洗盘  " + currentHoleNum + "  放管");
                        dtWashTrayInfo.Rows[0][1] = "1";
                        #endregion
                        lisMoveTube.Remove(TempMoveStatus);
                        MoveTubeUseFlag = false;
                        washTrayFlag = false;

                        #endregion
                        #endregion

                    }
                }
            }
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable dtTest = new DataTable();
            dtTest.Columns.Add("ID", typeof(string));
            dtTest.Columns.Add("Date", typeof(string));
            Random rd = new Random();
            for (int i = 0; i < 10; i++)
            {

                dtTest.Rows.Add(i + 1, rd.Next(0, 100));
            }
            double avg = cmd.AVERAGE(dtTest);
            double sd = cmd.STDEV(dtTest);
            double cv = cmd.CV(dtTest);
            txtTestAV.Text = avg.ToString();
            txtTestSD.Text = sd.ToString();
            txtTestCV.Text = cv.ToString();
            dtTest.Rows.Add("Avg", txtTestAV.Text.Trim());
            dtTest.Rows.Add("SD", txtTestSD.Text.Trim());
            dtTest.Rows.Add("CV", txtTestCV.Text.Trim());
            string filePath = Application.StartupPath + @"\基础性能\本底测试\SubstrateTest_" + DateTime.Now.ToString("yyyyMMdd HHmmss") + ".xls";
            DataTableExcel.TableToExcel(dtTest, filePath);

        }
        #region 光子板/条码扫描/在线烧录
        private void fbtnRead_Click(object sender, EventArgs e)
        {
            fbtnRead.Enabled = false;
            int Num = int.Parse(txtReadNum.Text);
            int readStart = int.Parse(txtReadStart.Text);
            int readEnd = int.Parse(txtReadEnd.Text);
            //读数数据表
            washTrayTubeClear();
            for (int i = 1; i <= readStart; i++)
            {
                txtReadShow.AppendText("向清洗盘加第" + i + "个管" + Environment.NewLine);
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06"), 1);
                if (!NetCom3.Instance.WashQuery())
                    return;
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                if (!NetCom3.Instance.WashQuery())
                    return;
                Thread.Sleep(500);
            }
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (24 - readStart).ToString("x2") + ""), 2);
            if (!NetCom3.Instance.WashQuery())
                return;

            for (int i = 1; i <= readStart; i++)
            {
                txtReadShow.AppendText("第" + i + "个管读数：" + Environment.NewLine);
                for (int j = 1; j <= Num; j++)
                {
                    BackObj = "";
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 00 01"), 2);
                    if (!NetCom3.Instance.WashQuery())
                        return;
                    else
                    {
                        int delay = 1000;
                        while (!BackObj.Contains("EB 90 31 A3") && delay > 0)
                        {
                            NetCom3.Delay(10);
                            delay = delay - 10;
                        }
                        if (BackObj.Contains("EB 90 31 A3"))
                        {
                            //string temp = BackObj.Replace(" ", "");
                            //int pos = temp.IndexOf("EB9031A3");
                            //temp = temp.Substring(pos, 32);
                            //temp = temp.Substring(temp.Length - 8);
                            string temp = BackObj.Substring(BackObj.Length - 16).Replace(" ", "");
                            temp = Convert.ToInt64(temp, 16).ToString();

                            if (int.Parse(temp) > Math.Pow(10, 5))
                                temp = ((int)GetPMT(double.Parse(temp))).ToString();
                            txtReadShow.AppendText(DateTime.Now.ToString("HH-mm-ss") + ": " + "PMT背景值：" + temp + Environment.NewLine);
                        }
                    }
                }
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                if (!NetCom3.Instance.WashQuery())
                    return;
            }
            /*
            DataTable dtReadData = new DataTable();
            dtReadData.Columns.Add("Pos", typeof(string));
            dtReadData.Columns.Add("Data", typeof(string));
            //读数位置
            List<int> lisReadPos = new List<int>();
            for (int i = readStart; i <= readEnd; i++)
            {
                for (int j = 0; j < Num; j++)
                {
                    lisReadPos.Add(i);
                }
            }
            int start = 0;
            while (start < lisReadPos.Count)
            {
                ///发送读数指令
                dtReadData.Rows.Add(lisReadPos[start].ToString(), BackObj);
                txtReadShow.AppendText(BackObj + Environment.NewLine);
                start++;
            }
            if (chbReadExport.Checked)
            {
                string filePath = Application.StartupPath + @"\仪器调试\光子板\ReadTest_" + DateTime.Now.ToString("yyyyMMdd HHmmss") + ".xls";
                DataTableExcel.TableToExcel(dtReadData, filePath);
            }
             */
            fbtnRead.Enabled = true;
        }

        private void btnScanSpCode_Click(object sender, EventArgs e)
        {
            string SpCodePos = nudSpCodePos.Value.ToString();
            ///发送指令读取SpCodePos位置的条码
            txtSpCode.Text = BackObj;
        }

        private void btnLoadProgram_Click(object sender, EventArgs e)
        {
            if (cmbZhenID.SelectedIndex < 0)
            {
                MessageBox.Show("未选择电控板位，请重新选择！");
                cmbZhenID.Focus();
                return;
            }
            if (txtFilePath.Text.Trim() == "" || !txtFilePath.Text.Trim().Substring(txtFilePath.Text.Trim().LastIndexOf(@"\")).Contains(cmbZhenID.SelectedItem.ToString().Substring(0, 2)))
            {
                MessageBox.Show("文件路径错误，请重新选择与所选电控板位对应的 " + cmbZhenID.SelectedItem.ToString().Substring(0, 2) + "模组 文件");
                txtFilePath.Focus();
                return;
            }
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            iapFilePath = txtFilePath.Text;
            selectZhenID = cmbZhenID.SelectedIndex + 1;  //1-8 计数器 通讯 抓手 加样 清洗 报警 制冷 温育 
            frmMain.LiquidQueryFlag = false;
            cmbZhenID.Enabled = txtFilePath.Enabled = btnSelectBin.Enabled = btnLoadProgram.Enabled = false;
            thFire = new Thread(new ThreadStart(LoadPro));
            thFire.Start();
        }
        private void LoadPro() //IAP
        {
            BeginInvoke(new Action(() =>
            {
                pgbLoad.Value = 1; //初始化进度栏
                lblPercentage.Text = pgbLoad.Value.ToString() + "%"; //进度栏下面打印进度
            }));
            FileStream Myfile;
            int connDelay = 10000;
            int sendDelay = 20;
            try
            {
                Myfile = new FileStream(iapFilePath.Trim(), FileMode.Open, FileAccess.Read);
                if (Myfile.Length < 50)
                {
                    frmMsgShow.MessageShow("ERROR", "这是一个空文件。");
                    goto errorOrEnd;
                }
            }
            catch (System.Exception exp)
            {
                frmMsgShow.MessageShow("ERROR", "获取该文件信息失败!");
                Console.WriteLine(exp.ToString());
                goto errorOrEnd;
            }
            if (selectZhenID == 2)
            {
                //通讯板稳定传输延迟20ms
                Invoke(new Action(() =>
                {
                    cmbSendDelay.Text = "20";
                    cmbConnDelay.Text = "10000";
                    cmbSendDelay.Enabled = false;
                    connDelay = int.Parse(cmbConnDelay.Text);
                    sendDelay = int.Parse(cmbSendDelay.Text);
                }));
            }

            string boardName = "";
            Invoke(new Action(() =>
            {
                boardName = cmbZhenID.SelectedItem.ToString();
            }));
            BinaryReader binreader = new BinaryReader(Myfile);
            int file_len = (int)Myfile.Length;//获取bin文件长度
            int allNumber = file_len + 8 - (file_len % 8 == 0 ? 8 : file_len % 8); //1107mod:EB90后一次发送8字节
            byte[] buff = new byte[allNumber];
            byte[] buff1 = binreader.ReadBytes(file_len);
            for (int i = 0; i < allNumber; i++)
            {
                if (i < file_len)
                {
                    buff[i] = buff1[i];
                }
                else
                {
                    buff[i] = 0;
                }
            }
            Myfile.Dispose();
            NetCom3.Instance.ReceiveHandel += DealReceive;

            //握手
            NetCom3.Instance.iapIsRun = true;
            BeginInvoke(new Action(() =>
            {
                lblDescribe.Text = "上下位机正在握手...";
            }));
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 AA 01"), 5); //通讯板处理
            if (!NetCom3.Instance.SingleQuery())
            {
                frmMsgShow.MessageShow("ERROR", "握手指令发送失败！");
                goto errorOrEnd;
            }
            while (receiveFlag[0] == 0)
            {
                NetCom3.Delay(100);
            }
            //握手 下位机
            BeginInvoke(new Action(() =>
            {
                lblDescribe.Text = "正在跳转 " + boardName;
            }));
            NetCom3.Instance.iapNoBack = true; //A0 01改成不返回了
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 A0 01 0" + selectZhenID), 5); //下位机跳转 
            if (!NetCom3.Instance.SingleQuery())
            {
                frmMsgShow.MessageShow("ERROR", "跳转指令发送失败！");
                goto errorOrEnd;
            }
            NetCom3.Instance.iapNoBack = false;

            if (selectZhenID == 2) //如果烧录通讯板，在这一步通讯板要跳转需要重连
            {
                aabc:
                //NetCom3.Delay(3000);
                NetCom3.Delay(connDelay);//lyq add 20201027
                if (NetCom3.Instance.CheckMyIp_Port_Link())
                {
                    NetCom3.isConnect = false;
                    NetCom3.Instance.iapConnectTwice();
                    if (!NetCom3.isConnect)
                    {
                        frmMsgShow.MessageShow("ERROR", "重新连接失败，正在重新连接。");
                        goto aabc;
                    }
                }
                else
                    goto aabc;
            }

            //NetCom3.Delay(3000);
            NetCom3.Delay(connDelay);//lyq add 20201027
            //握手 下位机IAP
            BeginInvoke(new Action(() =>
            {
                lblDescribe.Text = "正在与 " + boardName + " 的IAP握手";
            }));

            NetCom3.Instance.Send(NetCom3.Cover("EB 90 B0 01 0" + selectZhenID), 5); //与IAP握手
            if (!NetCom3.Instance.SingleQuery())
            {
                frmMsgShow.MessageShow("ERROR", "IAP握手指令发送失败！");
                goto errorOrEnd;
            }
            NetCom3.Delay(1000); //等待1秒
            while (receiveFlag[2] == 0)
            {
                NetCom3.Delay(100);
            }

            //擦除       
            BeginInvoke(new Action(() =>
            {
                pgbLoad.Value = 5;//进度百分之五
                lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                lblDescribe.Text = "正在擦除Flash...";
            }));
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 B0 02"), 5); //擦除
            if (!NetCom3.Instance.SingleQuery())
            {
                frmMsgShow.MessageShow("ERROR", "擦除指令发送失败！");
                goto errorOrEnd;
            }
            while (receiveFlag[3] == 0)
            {
                NetCom3.Delay(100);
            }

            //传输数据长度
            BeginInvoke(new Action(() =>
            {
                pgbLoad.Value = 25;//进度百分之二十五
                lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                lblDescribe.Text = "正在发送文件长度.";
            }));
            string len = file_len.ToString("x8");  //文件长度转化为十六进制八位
            len = len.Substring(0, 2) + " " + len.Substring(2, 2) + " " + len.Substring(4, 2) + " " + len.Substring(6, 2);
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 B0 03 00 00 " + len), 5);
            if (!NetCom3.Instance.SingleQuery())
            {
                frmMsgShow.MessageShow("ERROR", "文件长度发送失败！");
                goto errorOrEnd;
            }
            while (receiveFlag[4] == 0)
            {
                NetCom3.Delay(100);
            }

            //传输数据
            ManualResetEvent loadDataDone = new ManualResetEvent(false);
            NetCom3.Instance.iapNoBack = true;
            BeginInvoke(new Action(() =>
            {
                pgbLoad.Value = 30;//进度百分之30
                lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                lblDescribe.Text = "正在发送数据...";
            }));

            for (int m = 0, k = 1; m < allNumber; m += 8) //通讯指令16进制2位一字节
            {
                //Console.WriteLine(m+"-------"+allNumber);//测试
                loadDataDone.Reset();
                if ((m + 8) >= allNumber)
                    NetCom3.Instance.iapNoBack = false;
                string data = "EB 90 "
                            + buff[m].ToString("x2") + " " + buff[m + 1].ToString("x2") + " "
                            + buff[m + 2].ToString("x2") + " " + buff[m + 3].ToString("x2") + " "
                            + buff[m + 4].ToString("x2") + " " + buff[m + 5].ToString("x2") + " "
                            + buff[m + 6].ToString("x2") + " " + buff[m + 7].ToString("x2");
                NetCom3.Instance.iapSend(NetCom3.Cover(data), 5);
                if (!IapTraQuery())//循环等待变量是延迟1ms
                {
                    frmMsgShow.MessageShow("ERROR", "数据传输失败！");
                    goto errorOrEnd;
                }

                if (m / (allNumber / 50) == k)
                {
                    BeginInvoke(new Action(() =>
                    {
                        pgbLoad.Value = 30 + k;//进度百分之N
                        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                    }));
                    k++;
                }
                //Thread.Sleep(2);
                if (!loadDataDone.WaitOne(sendDelay, false))
                {
                    ;
                }
            }
            while (true)
            {
                if (receiveFlag[5] == 0)
                    NetCom3.Delay(100);
                else if (receiveFlag[5] == 1)
                    break;
                else if (receiveFlag[5] == 2)
                {
                    frmMsgShow.MessageShow("ERROR", "IAP程序烧录失败");
                    goto errorOrEnd;
                }
            }

            NetCom3.Instance.iapNoBack = true;
            BeginInvoke(new Action(() =>
            {
                pgbLoad.Value = 90;//进度百分之90
                lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                lblDescribe.Text = "数据传输完成...";
            }));

            NetCom3.Delay(1000);

            //结束返回软件
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 B0 05 00 00 00 00 80 00"), 5);
            if (!NetCom3.Instance.SingleQuery())
            {
                frmMsgShow.MessageShow("ERROR", "结束指令发送失败！");
                goto errorOrEnd;
            }
            else
            {
                BeginInvoke(new Action(() =>
                {
                    pgbLoad.Value = 100;//进度百分之100
                    lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                    lblDescribe.Text = "程序下载成功！";
                }));
            }

            //通讯板重连
            if (selectZhenID == 2)
            {
                bbcd:
                if (NetCom3.Instance.CheckMyIp_Port_Link())
                {
                    NetCom3.isConnect = false;
                    NetCom3.Instance.iapConnectTwice();
                    if (!NetCom3.isConnect)
                    {
                        frmMsgShow.MessageShow("ERROR", "重新连接失败，正在重新连接。");
                        goto bbcd;
                    }
                }
                else
                    goto bbcd;
            }


            NetCom3.Instance.iapNoBack = false;
            NetCom3.Delay(500);
            NetCom3.Instance.ReceiveHandel -= DealReceive;

            if (selectZhenID == 2)
            {
                frmMsgShow.MessageShow("IAP", "请重启软件和仪器，点击确定自动退出软件");
                Environment.Exit(0);
            }

            #region  烧完程序初始化前先握手
            BackObj = "";
            int[] HandData = new int[16];
            NetCom3.Delay(2000); //预留b0 05跳转时间
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 F1 01"), 5);
            if (!NetCom3.Instance.SingleQuery())
            {
                goto errorOrEnd;
            }
            //判断各个模组是否握手成功            
            while (true)
            {
                if (BackObj.Contains("EB 90 F1 01"))
                {
                    HandData = NetCom3.converTo10(BackObj.Split(' '));
                    break;
                }
            }
            if (HandData[4] != 255)
            {
                MessageBox.Show("计数器模组握手失败！", "上下位机握手");
                goto errorOrEnd;
            }
            if (HandData[5] != 255)
            {
                MessageBox.Show("加样机模组握手失败！", "上下位机握手");
                goto errorOrEnd;
            }
            if (HandData[6] != 255)
            {
                MessageBox.Show("理杯机模组握手失败！", "上下位机握手");
                goto errorOrEnd;
            }
            if (HandData[7] != 255)
            {
                MessageBox.Show("清洗模组握手失败!", "上下位机握手");
                goto errorOrEnd;
            }
            if (HandData[8] != 255)
            {
                MessageBox.Show("报警模组握手失败！", "上下位机握手");
                goto errorOrEnd;
            }
            if (HandData[9] != 255)
            {
                MessageBox.Show("温育盘模组握手失败!", "上下位机握手");
                goto errorOrEnd;
            }
            #endregion
            //传输完毕后初始化仪器
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 F1 02"), 5);
            if (!NetCom3.Instance.SingleQuery())
            {
                goto errorOrEnd;
            }
            //判断是否初始化成功            
            if (NetCom3.Instance.ErrorMessage != null)
            {
                frmMsgShow.MessageShow("仪器初始化", NetCom3.Instance.ErrorMessage);
                goto errorOrEnd;
            }
            frmMsgShow.MessageShow("IAP", "程序下载成功！");

            errorOrEnd:
            Invoke(new Action(() =>
            {
                for (int i = 0; i < receiveFlag.Length; i++)
                {
                    receiveFlag[i] = 0;
                }
                pgbLoad.Value = 0;
                lblPercentage.Text = "0%";
                lblDescribe.Text = "...";
                cmbZhenID.Enabled = txtFilePath.Enabled = btnSelectBin.Enabled = btnLoadProgram.Enabled = true;
                NetCom3.Instance.iapIsRun = false;
                NetCom3.Instance.iapNoBack = false;
                frmMain.LiquidQueryFlag = true;
            }));
            buff = new byte[1];
            buff1 = new byte[1];
            GC.Collect();
            frmReturn();
        }
        private void DealReceive(string order)
        {
            order = order.Substring(6, 8);
            if (order.Contains("55 01"))
            {
                receiveFlag[0] = 1;
            }
            //else if (order.Contains("0A A0 0" + selectZhenID))  //已修改，不返回这条指令
            //{
            //    receiveFlag[1] = 1;
            //}
            else if (order.Contains("6A A6 0" + selectZhenID))
            {
                receiveFlag[2] = 1;
            }
            else if (order.Contains("B0 02 FF"))
            {
                receiveFlag[3] = 1;
            }
            else if (order.Contains("B0 03 FF"))
            {
                receiveFlag[4] = 1;
            }
            else if (order.Contains("B0 04 FF"))//烧录成功
            {
                receiveFlag[5] = 1;
            }
            else if (order.Contains("B0 04 01"))//烧录失败
            {
                receiveFlag[5] = 2;
            }

        }
        private void LoadProgram()
        {
            BeginInvoke(new Action(() =>
            {
                pgbLoad.Value = 1;
                lblPercentage.Text = pgbLoad.Value.ToString() + "%";
            }));
            FileStream Myfile = new FileStream(txtFilePath.Text.Trim(), FileMode.Open, FileAccess.Read);
            BinaryReader binreader = new BinaryReader(Myfile);
            int file_len = (int)Myfile.Length;//获取bin文件长度
            int allNumber = file_len + 7 - (file_len % 7 == 0 ? 7 : file_len % 7);//应发送的最大长度，为7的倍数
            byte[] buff = new byte[allNumber];
            byte[] buff1 = binreader.ReadBytes(file_len);
            for (int i = 0; i < allNumber; i++)
            {
                if (i < file_len)
                {
                    buff[i] = buff1[i];
                }
                else
                {
                    buff[i] = 0;
                }
            }
            if (selectZhenID == 0)//样本盘2
            {
                #region 样本盘程序烧录
                //BeginInvoke(new Action(() => { BeginInvoke(new Action(() => { lblDescribe.Text = "系统正在复位..."; })); }));
                //if (!CanCom.Instance.SpRgTray_Senddata("00 00 00 00 00 00 00", 10))
                //{
                //    MessageBox.Show("系统复位失败！");
                //    return;
                //}

                //BeginInvoke(new Action(() => { BeginInvoke(new Action(() => { lblDescribe.Text = "上下位机正在握手..."; })); }));
                //if (!CanCom.Instance.SpRgTray_Senddata("02 AA 55 00 00 00 00", 10))//判定握手成功与否
                //{
                //    MessageBox.Show("上下位机握手失败，请检测通讯线路！");
                //    return;
                //}
                //BeginInvoke(new Action(() =>
                //{
                //    pgbLoad.Value = 5;//进度百分之五
                //    lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //    lblDescribe.Text = "上下位机握手成功";
                //}));
                //if (CanCom.Instance.SpRgTray_CanSend("02 00 00 00 00 00 00", 10))//判定发送Flash擦除命令成功与否
                //{
                //    BeginInvoke(new Action(() =>
                //    {
                //        lblDescribe.Text = "正在擦除Flash...";
                //    }));
                //    CanCom.Instance.SpRgTray_Recevice(1, 1000);//返回等待
                //    if (!CanCom.Instance.SpRgTray_DoneFlag)
                //    {
                //        MessageBox.Show("Flash擦除失败，请重新操作！");
                //        return;
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 25;//进度百分之二十五
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "Flash擦除成功";
                //    }));
                //    if (!CanCom.Instance.SpRgTray_CanSend("02 01 00 " + file_len.ToString("x8").Substring(0, 2) + " " +
                //        file_len.ToString("x8").Substring(2, 2) + " " + file_len.ToString("x8").Substring(4, 2) + " " +
                //        file_len.ToString("x8").Substring(6, 2), 10))
                //    {
                //        MessageBox.Show("字节长度写入失败，请检查通讯线路！");
                //        return;
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 30;//进度百分之30
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "字节长度写入成功";
                //        Thread.Sleep(100);
                //        lblDescribe.Text = "正在发送数据...";
                //    }));
                //    for (int j = 0; j < allNumber; j += 7)
                //    {
                //        if (!CanCom.Instance.SpRgTray_CanSend(buff[j].ToString("x2") + " "
                //            + buff[j + 1].ToString("x2") + " " + buff[j + 2].ToString("x2") + " "
                //            + buff[j + 3].ToString("x2") + " " + buff[j + 4].ToString("x2") + " "
                //            + buff[j + 5].ToString("x2") + " " + buff[j + 6].ToString("x2"), 10))
                //        {
                //            MessageBox.Show("数据发送失败，请检查通讯线路！");
                //            return;
                //        }
                //        if (j % (allNumber / 50) == 0)
                //        {
                //            BeginInvoke(new Action(() =>
                //            {
                //                pgbLoad.Value = 30 + (j / (allNumber / 50));//进度百分之N
                //                lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //            }));
                //        }
                //        Thread.Sleep(20);
                //    }

                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 90;//进度百分之90
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "数据发送完成，下位机正在写入Flash...";
                //    }));

                //    CanCom.Instance.SpRgTray_Recevice(2, 18000 - 2 * allNumber / 7);//等待时间为3分钟 减去之前的每次发送的 20ms延时。
                //    if (CanCom.Instance.SpRgTrayPara == 1)
                //    {
                //        BeginInvoke(new Action(() =>
                //        {
                //            pgbLoad.Value = 100;//进度百分之100
                //            lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //            lblDescribe.Text = "程序下载成功！";
                //        }));
                //    }
                //    else
                //    {
                //        MessageBox.Show("程序下载失败，请重新烧录！");
                //        return;
                //    }
                //}
                #endregion
            }
            else if (selectZhenID == 1)//加样机3
            {
                #region 加样机程序烧录
                //BeginInvoke(new Action(() => { BeginInvoke(new Action(() => { lblDescribe.Text = "系统正在复位..."; })); }));
                //if (!CanCom.Instance.AddSp_Senddata("00 00 00 00 00 00 00", 10))
                //{
                //    MessageBox.Show("系统复位失败！");
                //    return;
                //}
                //BeginInvoke(new Action(() => { lblDescribe.Text = "上下位机正在握手..."; }));
                //if (!CanCom.Instance.AddSp_Senddata("03 AA 55 00 00 00 00", 10))//判定握手成功与否
                //{
                //    MessageBox.Show("上下位机握手失败，请检测通讯线路！");
                //    return;
                //}
                //BeginInvoke(new Action(() =>
                //{
                //    pgbLoad.Value = 5;//进度百分之五
                //    lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //    lblDescribe.Text = "上下位机握手成功";
                //}));
                //if (CanCom.Instance.AddSp_CanSend("03 00 00 00 00 00 00", 10))//判定发送Flash擦除命令成功与否
                //{
                //    BeginInvoke(new Action(() => { lblDescribe.Text = "正在擦除Flash..."; }));
                //    CanCom.Instance.AddSp_Recevice(1, 1000);//返回等待3秒
                //    if (!CanCom.Instance.AddSp_DoneFlag)
                //    {
                //        MessageBox.Show("Flash擦除失败，请重新操作！");
                //        return;
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 25;//进度百分之二十五
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "Flash擦除成功";
                //    }));
                //    if (!CanCom.Instance.AddSp_CanSend("03 01 00 " + file_len.ToString("x8").Substring(0, 2) + " " +
                //        file_len.ToString("x8").Substring(2, 2) + " " + file_len.ToString("x8").Substring(4, 2) + " " +
                //        file_len.ToString("x8").Substring(6, 2), 10))
                //    {
                //        MessageBox.Show("字节长度写入失败，请检查通讯线路！");
                //        return;
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 30;//进度百分之30
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "字节长度写入成功";
                //        Thread.Sleep(100);
                //        lblDescribe.Text = "正在发送数据...";
                //    }));
                //    for (int j = 0; j < allNumber; j += 7)
                //    {
                //        if (!CanCom.Instance.AddSp_CanSend(buff[j].ToString("x2") + " "
                //            + buff[j + 1].ToString("x2") + " " + buff[j + 2].ToString("x2") + " "
                //            + buff[j + 3].ToString("x2") + " " + buff[j + 4].ToString("x2") + " "
                //            + buff[j + 5].ToString("x2") + " " + buff[j + 6].ToString("x2"), 10))
                //        {
                //            MessageBox.Show("数据发送失败，请检查通讯线路！");
                //            return;
                //        }
                //        if (j % (allNumber / 50) == 0)
                //        {
                //            BeginInvoke(new Action(() =>
                //            {
                //                pgbLoad.Value = 30 + (j / (allNumber / 50));//进度百分之N
                //                lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //            }));
                //        }
                //        Thread.Sleep(20);
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 90;//进度百分之90
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "数据发送完成，下位机正在写入Flash...";
                //    }));
                //    CanCom.Instance.AddSp_Recevice(2, 18000 - 2 * allNumber / 7);//等待时间为3分钟 减去之前的每次发送的 20ms延时。
                //    if (CanCom.Instance.AddSpPara == 1)
                //    {
                //        BeginInvoke(new Action(() =>
                //        {
                //            pgbLoad.Value = 100;//进度百分之100
                //            lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //            lblDescribe.Text = "程序下载成功！";
                //        }));
                //    }
                //    else
                //    {
                //        MessageBox.Show("程序下载失败，请重新烧录！");
                //        return;
                //    }
                //}
                #endregion
            }
            else if (selectZhenID == 2)//取管手4
            {
                #region 取管手程序烧录
                //BeginInvoke(new Action(() => { BeginInvoke(new Action(() => { lblDescribe.Text = "系统正在复位..."; })); }));
                //if (!CanCom.Instance.MoveTube_Senddata("00 00 00 00 00 00 00", 10))
                //{
                //    MessageBox.Show("系统复位失败！");
                //    return;
                //}
                //BeginInvoke(new Action(() => { lblDescribe.Text = "上下位机正在握手..."; }));
                //if (!CanCom.Instance.MoveTube_Senddata("04 AA 55 00 00 00 00", 10))//判定握手成功与否
                //{
                //    MessageBox.Show("上下位机握手失败，请检测通讯线路！");
                //    return;
                //}
                //BeginInvoke(new Action(() =>
                //{
                //    pgbLoad.Value = 5;//进度百分之五
                //    lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //    lblDescribe.Text = "上下位机握手成功";
                //}));
                //if (CanCom.Instance.MoveTube_CanSend("04 00 00 00 00 00 00", 10))//判定发送Flash擦除命令成功与否
                //{
                //    BeginInvoke(new Action(() => { lblDescribe.Text = "正在擦除Flash..."; }));
                //    CanCom.Instance.MoveTube_Recevice(1, 1000);//返回等待3秒
                //    if (!CanCom.Instance.MoveTube_DoneFlag)
                //    {
                //        MessageBox.Show("Flash擦除失败，请重新操作！");
                //        return;
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 25;//进度百分之二十五
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "Flash擦除成功";
                //    }));
                //    if (!CanCom.Instance.MoveTube_CanSend("04 01 00 " + file_len.ToString("x8").Substring(0, 2) + " " +
                //        file_len.ToString("x8").Substring(2, 2) + " " + file_len.ToString("x8").Substring(4, 2) + " " +
                //        file_len.ToString("x8").Substring(6, 2), 10))
                //    {
                //        MessageBox.Show("字节长度写入失败，请检查通讯线路！");
                //        return;
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 30;//进度百分之30
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "字节长度写入成功";
                //        Thread.Sleep(100);
                //        lblDescribe.Text = "正在发送数据...";
                //    }));
                //    for (int j = 0; j < allNumber; j += 7)
                //    {
                //        if (!CanCom.Instance.MoveTube_CanSend(buff[j].ToString("x2") + " "
                //            + buff[j + 1].ToString("x2") + " " + buff[j + 2].ToString("x2") + " "
                //            + buff[j + 3].ToString("x2") + " " + buff[j + 4].ToString("x2") + " "
                //            + buff[j + 5].ToString("x2") + " " + buff[j + 6].ToString("x2"), 10))
                //        {
                //            MessageBox.Show("数据发送失败，请检查通讯线路！");
                //            return;
                //        }
                //        if (j % (allNumber / 50) == 0)
                //        {
                //            BeginInvoke(new Action(() =>
                //            {
                //                pgbLoad.Value = 30 + (j / (allNumber / 50));//进度百分之N
                //                lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //            }));
                //        }
                //        Thread.Sleep(20);
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 90;//进度百分之90
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "数据发送完成，下位机正在写入Flash...";
                //    }));
                //    CanCom.Instance.MoveTube_Recevice(2, 18000 - 2 * allNumber / 7);//等待时间为3分钟 减去之前的每次发送的 20ms延时。
                //    if (CanCom.Instance.MoveTubePara == 1)
                //    {
                //        BeginInvoke(new Action(() =>
                //        {
                //            pgbLoad.Value = 100;//进度百分之100
                //            lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //            lblDescribe.Text = "程序下载成功！";
                //        }));
                //    }
                //    else
                //    {
                //        MessageBox.Show("程序下载失败，请重新烧录！");
                //        return;
                //    }
                //}
                #endregion
            }
            else if (selectZhenID == 3)//温育盘5
            {
                #region 温育反应盘程序烧录
                //BeginInvoke(new Action(() => { BeginInvoke(new Action(() => { lblDescribe.Text = "系统正在复位..."; })); }));
                //if (!CanCom.Instance.ReactTray_Senddata("00 00 00 00 00 00 00", 10))
                //{
                //    MessageBox.Show("系统复位失败！");
                //    return;
                //}
                //BeginInvoke(new Action(() => { lblDescribe.Text = "上下位机正在握手..."; }));
                //if (!CanCom.Instance.ReactTray_Senddata("05 AA 55 00 00 00 00", 10))//判定握手成功与否
                //{
                //    MessageBox.Show("上下位机握手失败，请检测通讯线路！");
                //    return;
                //}
                //BeginInvoke(new Action(() =>
                //{
                //    pgbLoad.Value = 5;//进度百分之五
                //    lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //    lblDescribe.Text = "上下位机握手成功";
                //}));
                //if (CanCom.Instance.ReactTray_CanSend("05 00 00 00 00 00 00", 10))//判定发送Flash擦除命令成功与否
                //{
                //    BeginInvoke(new Action(() => { lblDescribe.Text = "正在擦除Flash..."; }));
                //    CanCom.Instance.ReactTray_Recevice(1, 1000);//返回等待3秒
                //    if (!CanCom.Instance.ReactTray_DoneFlag)
                //    {
                //        MessageBox.Show("Flash擦除失败，请重新操作！");
                //        return;
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 25;//进度百分之二十五
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "Flash擦除成功";
                //    }));
                //    if (!CanCom.Instance.ReactTray_CanSend("05 01 00 " + file_len.ToString("x8").Substring(0, 2) + " " +
                //        file_len.ToString("x8").Substring(2, 2) + " " + file_len.ToString("x8").Substring(4, 2) + " " +
                //        file_len.ToString("x8").Substring(6, 2), 10))
                //    {
                //        MessageBox.Show("字节长度写入失败，请检查通讯线路！");
                //        return;
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 30;//进度百分之30
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "字节长度写入成功";
                //        Thread.Sleep(100);
                //        lblDescribe.Text = "正在发送数据...";
                //    }));
                //    for (int j = 0; j < allNumber; j += 7)
                //    {
                //        if (!CanCom.Instance.ReactTray_CanSend(buff[j].ToString("x2") + " "
                //            + buff[j + 1].ToString("x2") + " " + buff[j + 2].ToString("x2") + " "
                //            + buff[j + 3].ToString("x2") + " " + buff[j + 4].ToString("x2") + " "
                //            + buff[j + 5].ToString("x2") + " " + buff[j + 6].ToString("x2"), 10))
                //        {
                //            MessageBox.Show("数据发送失败，请检查通讯线路！");
                //            return;
                //        }
                //        if (j % (allNumber / 50) == 0)
                //        {
                //            BeginInvoke(new Action(() =>
                //            {
                //                pgbLoad.Value = 30 + (j / (allNumber / 50));//进度百分之N
                //                lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //            }));
                //        }
                //        Thread.Sleep(20);
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 90;//进度百分之90
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "数据发送完成，下位机正在写入Flash...";
                //    }));
                //    CanCom.Instance.ReactTray_Recevice(2, 18000 - 2 * allNumber / 7);//等待时间为3分钟 减去之前的每次发送的 20ms延时。
                //    if (CanCom.Instance.ReactTrayPara == 1)
                //    {
                //        BeginInvoke(new Action(() =>
                //        {
                //            pgbLoad.Value = 100;//进度百分之100
                //            lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //            lblDescribe.Text = "程序下载成功！";
                //        }));
                //    }
                //    else
                //    {
                //        MessageBox.Show("程序下载失败，请重新烧录！");
                //        return;
                //    }
                //}
                #endregion
            }
            else if (selectZhenID == 4)//清洗盘6
            {
                #region 清洗盘程序烧录
                //BeginInvoke(new Action(() => { BeginInvoke(new Action(() => { lblDescribe.Text = "系统正在复位..."; })); }));
                //if (!CanCom.Instance.WashTray_Senddata("00 00 00 00 00 00 00", 10))
                //{
                //    MessageBox.Show("系统复位失败！");
                //    return;
                //}
                //BeginInvoke(new Action(() => { lblDescribe.Text = "上下位机正在握手..."; }));
                //if (!CanCom.Instance.WashTray_Senddata("06 AA 55 00 00 00 00", 10))//判定握手成功与否
                //{
                //    MessageBox.Show("上下位机握手失败，请检测通讯线路！");
                //    return;
                //}
                //BeginInvoke(new Action(() =>
                //{
                //    pgbLoad.Value = 5;//进度百分之五
                //    lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //    lblDescribe.Text = "上下位机握手成功";
                //}));
                //if (CanCom.Instance.WashTray_CanSend("06 00 00 00 00 00 00", 10))//判定发送Flash擦除命令成功与否
                //{
                //    BeginInvoke(new Action(() => { lblDescribe.Text = "正在擦除Flash..."; }));
                //    CanCom.Instance.WashTray_Recevice(1, 1000);//返回等待3秒
                //    if (!CanCom.Instance.WashTray_DoneFlag)
                //    {
                //        MessageBox.Show("Flash擦除失败，请重新操作！");
                //        return;
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 25;//进度百分之二十五
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "Flash擦除成功";
                //    }));
                //    if (!CanCom.Instance.WashTray_CanSend("06 01 00 " + file_len.ToString("x8").Substring(0, 2) + " " +
                //        file_len.ToString("x8").Substring(2, 2) + " " + file_len.ToString("x8").Substring(4, 2) + " " +
                //        file_len.ToString("x8").Substring(6, 2), 10))
                //    {
                //        MessageBox.Show("字节长度写入失败，请检查通讯线路！");
                //        return;
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 30;//进度百分之30
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "字节长度写入成功";
                //        Thread.Sleep(100);
                //        lblDescribe.Text = "正在发送数据...";
                //    }));
                //    for (int j = 0; j < allNumber; j += 7)
                //    {
                //        if (!CanCom.Instance.WashTray_CanSend(buff[j].ToString("x2") + " "
                //            + buff[j + 1].ToString("x2") + " " + buff[j + 2].ToString("x2") + " "
                //            + buff[j + 3].ToString("x2") + " " + buff[j + 4].ToString("x2") + " "
                //            + buff[j + 5].ToString("x2") + " " + buff[j + 6].ToString("x2"), 10))
                //        {
                //            MessageBox.Show("数据发送失败，请检查通讯线路！");
                //            return;
                //        }
                //        if (j % (allNumber / 50) == 0)
                //        {
                //            BeginInvoke(new Action(() =>
                //            {
                //                pgbLoad.Value = 30 + (j / (allNumber / 50));//进度百分之N
                //                lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //            }));
                //        }
                //        Thread.Sleep(20);
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 90;//进度百分之90
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "数据发送完成，下位机正在写入Flash...";
                //    }));
                //    CanCom.Instance.WashTray_Recevice(2, 18000 - 2 * allNumber / 7);//等待时间为3分钟 减去之前的每次发送的 20ms延时。
                //    if (CanCom.Instance.WashTrayPara == 1)
                //    {
                //        BeginInvoke(new Action(() =>
                //        {
                //            pgbLoad.Value = 100;//进度百分之100
                //            lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //            lblDescribe.Text = "程序下载成功！";
                //        }));
                //    }
                //    else
                //    {
                //        MessageBox.Show("程序下载失败，请重新烧录！");
                //        return;
                //    }
                //}
                #endregion
            }
            else if (selectZhenID == 5)//光子板7
            {
                #region 光子板程序烧录
                //BeginInvoke(new Action(() => { BeginInvoke(new Action(() => { lblDescribe.Text = "系统正在复位..."; })); }));
                //if (!CanCom.Instance.Read_Senddata("00 00 00 00 00 00 00", 10))
                //{
                //    MessageBox.Show("系统复位失败！");
                //    return;
                //}
                //BeginInvoke(new Action(() => { lblDescribe.Text = "上下位机正在握手..."; }));
                //if (!CanCom.Instance.Read_Senddata("07 AA 55 00 00 00 00", 10))//判定握手成功与否
                //{
                //    MessageBox.Show("上下位机握手失败，请检测通讯线路！");
                //    return;
                //}
                //BeginInvoke(new Action(() =>
                //{
                //    pgbLoad.Value = 5;//进度百分之五
                //    lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //    lblDescribe.Text = "上下位机握手成功";
                //}));
                //if (CanCom.Instance.Read_CanSend("07 00 00 00 00 00 00", 10))//判定发送Flash擦除命令成功与否
                //{
                //    BeginInvoke(new Action(() => { lblDescribe.Text = "正在擦除Flash..."; }));
                //    CanCom.Instance.Read_Recevice(1, 1000);//返回等待3秒
                //    if (!CanCom.Instance.Read_DoneFlag)
                //    {
                //        MessageBox.Show("Flash擦除失败，请重新操作！");
                //        return;
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 25;//进度百分之二十五
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "Flash擦除成功";
                //    }));
                //    if (!CanCom.Instance.Read_CanSend("07 01 00 " + file_len.ToString("x8").Substring(0, 2) + " " +
                //        file_len.ToString("x8").Substring(2, 2) + " " + file_len.ToString("x8").Substring(4, 2) + " " +
                //        file_len.ToString("x8").Substring(6, 2), 10))
                //    {
                //        MessageBox.Show("字节长度写入失败，请检查通讯线路！");
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 30;//进度百分之30
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "字节长度写入成功";
                //        Thread.Sleep(100);
                //        lblDescribe.Text = "正在发送数据...";
                //    }));
                //    for (int j = 0; j < allNumber; j += 7)
                //    {
                //        if (!CanCom.Instance.Read_CanSend(buff[j].ToString("x2") + " "
                //            + buff[j + 1].ToString("x2") + " " + buff[j + 2].ToString("x2") + " "
                //            + buff[j + 3].ToString("x2") + " " + buff[j + 4].ToString("x2") + " "
                //            + buff[j + 5].ToString("x2") + " " + buff[j + 6].ToString("x2"), 10))
                //        {
                //            MessageBox.Show("数据发送失败，请检查通讯线路！");
                //            return;
                //        }
                //        if (j % (allNumber / 50) == 0)
                //        {
                //            BeginInvoke(new Action(() =>
                //            {
                //                pgbLoad.Value = 30 + (j / (allNumber / 50));//进度百分之N
                //                lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //            }));
                //        }
                //        Thread.Sleep(20);
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 90;//进度百分之90
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "数据发送完成，下位机正在写入Flash...";
                //    }));
                //    CanCom.Instance.Read_Recevice(2, 18000 - 2 * allNumber / 7);//等待时间为3分钟 减去之前的每次发送的 20ms延时。
                //    if (CanCom.Instance.ReadPara == 1)
                //    {
                //        BeginInvoke(new Action(() =>
                //        {
                //            pgbLoad.Value = 100;//进度百分之100
                //            lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //            lblDescribe.Text = "程序下载成功！";
                //        }));
                //    }
                //    else
                //    {
                //        MessageBox.Show("程序下载失败，请重新烧录！");
                //        return;
                //    }
                //}
                #endregion
            }
            else if (selectZhenID == 6)//负压板8
            {
                #region 负压板程序烧录
                //BeginInvoke(new Action(() => { BeginInvoke(new Action(() => { lblDescribe.Text = "系统正在复位..."; })); }));
                //if (!CanCom.Instance.NegPressure_Senddata("00 00 00 00 00 00 00", 10))
                //{
                //    MessageBox.Show("系统复位失败！");
                //    return;
                //}
                //BeginInvoke(new Action(() => { lblDescribe.Text = "上下位机正在握手..."; }));
                //if (!CanCom.Instance.NegPressure_Senddata("08 AA 55 00 00 00 00", 10))//判定握手成功与否
                //{
                //    MessageBox.Show("上下位机握手失败，请检测通讯线路！");
                //    return;
                //}
                //BeginInvoke(new Action(() =>
                //{
                //    pgbLoad.Value = 5;//进度百分之五
                //    lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //    lblDescribe.Text = "上下位机握手成功";
                //}));
                //if (CanCom.Instance.NegPressure_CanSend("08 00 00 00 00 00 00", 10))//判定发送Flash擦除命令成功与否
                //{
                //    BeginInvoke(new Action(() => { lblDescribe.Text = "正在擦除Flash..."; }));
                //    CanCom.Instance.NegPressure_Recevice(1, 1000);//返回等待3秒
                //    if (!CanCom.Instance.NegPressure_DoneFlag)
                //    {
                //        MessageBox.Show("Flash擦除失败，请重新操作！");
                //        return;
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 25;//进度百分之二十五
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "Flash擦除成功";
                //    }));
                //    if (!CanCom.Instance.NegPressure_CanSend("08 01 00 " + file_len.ToString("x8").Substring(0, 2) + " " +
                //        file_len.ToString("x8").Substring(2, 2) + " " + file_len.ToString("x8").Substring(4, 2) + " " +
                //        file_len.ToString("x8").Substring(6, 2), 10))
                //    {
                //        MessageBox.Show("字节长度写入失败，请检查通讯线路！");
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 30;//进度百分之30
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "字节长度写入成功";
                //        Thread.Sleep(100);
                //        lblDescribe.Text = "正在发送数据...";
                //    }));
                //    for (int j = 0; j < allNumber; j += 7)
                //    {
                //        if (!CanCom.Instance.NegPressure_CanSend(buff[j].ToString("x2") + " "
                //            + buff[j + 1].ToString("x2") + " " + buff[j + 2].ToString("x2") + " "
                //            + buff[j + 3].ToString("x2") + " " + buff[j + 4].ToString("x2") + " "
                //            + buff[j + 5].ToString("x2") + " " + buff[j + 6].ToString("x2"), 10))
                //        {
                //            MessageBox.Show("数据发送失败，请检查通讯线路！");
                //            return;
                //        }
                //        if (j % (allNumber / 50) == 0)
                //        {
                //            BeginInvoke(new Action(() =>
                //            {
                //                pgbLoad.Value = 30 + (j / (allNumber / 50));//进度百分之N
                //                lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //            }));
                //        }
                //        Thread.Sleep(20);
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 90;//进度百分之90
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "数据发送完成，下位机正在写入Flash...";
                //    }));
                //    CanCom.Instance.NegPressure_Recevice(2, 18000 - 2 * allNumber / 7);//等待时间为3分钟 减去之前的每次发送的 20ms延时。
                //    if (CanCom.Instance.NegPressurePara == 1)
                //    {
                //        BeginInvoke(new Action(() =>
                //        {
                //            pgbLoad.Value = 100;//进度百分之100
                //            lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //            lblDescribe.Text = "程序下载成功！";
                //        }));
                //    }
                //    else
                //    {
                //        MessageBox.Show("程序下载失败，请重新烧录！");
                //        return;
                //    }
                //}
                #endregion
            }
            else if (selectZhenID == 7)//混匀板9
            {
                #region 混匀板程序烧录
                //BeginInvoke(new Action(() => { BeginInvoke(new Action(() => { lblDescribe.Text = "系统正在复位..."; })); }));
                //if (!CanCom.Instance.WashMix_Senddata("00 00 00 00 00 00 00", 10))
                //{
                //    MessageBox.Show("系统复位失败！");
                //    return;
                //}
                //BeginInvoke(new Action(() => { lblDescribe.Text = "上下位机正在握手..."; }));
                //if (!CanCom.Instance.WashMix_Senddata("09 AA 55 00 00 00 00", 10))//判定握手成功与否
                //{
                //    MessageBox.Show("上下位机握手失败，请检测通讯线路！");
                //    return;
                //}
                //BeginInvoke(new Action(() =>
                //{
                //    pgbLoad.Value = 5;//进度百分之五
                //    lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //    lblDescribe.Text = "上下位机握手成功";
                //}));
                //if (CanCom.Instance.WashMix_CanSend("09 00 00 00 00 00 00", 10))//判定发送Flash擦除命令成功与否
                //{
                //    BeginInvoke(new Action(() => { lblDescribe.Text = "正在擦除Flash..."; }));
                //    CanCom.Instance.WashMix_Recevice(1, 1000);//返回等待3秒
                //    if (!CanCom.Instance.WashMix_DoneFlag)
                //    {
                //        MessageBox.Show("Flash擦除失败，请重新操作！");
                //        return;
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 25;//进度百分之二十五
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "Flash擦除成功";
                //    }));
                //    if (!CanCom.Instance.WashMix_CanSend("09 01 00 " + file_len.ToString("x8").Substring(0, 2) + " " +
                //        file_len.ToString("x8").Substring(2, 2) + " " + file_len.ToString("x8").Substring(4, 2) + " " +
                //        file_len.ToString("x8").Substring(6, 2), 10))
                //    {
                //        MessageBox.Show("字节长度写入失败，请检查通讯线路！");
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 30;//进度百分之30
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "字节长度写入成功";
                //        Thread.Sleep(100);
                //        lblDescribe.Text = "正在发送数据...";
                //    }));
                //    for (int j = 0; j < allNumber; j += 7)
                //    {
                //        if (!CanCom.Instance.WashMix_CanSend(buff[j].ToString("x2") + " "
                //            + buff[j + 1].ToString("x2") + " " + buff[j + 2].ToString("x2") + " "
                //            + buff[j + 3].ToString("x2") + " " + buff[j + 4].ToString("x2") + " "
                //            + buff[j + 5].ToString("x2") + " " + buff[j + 6].ToString("x2"), 10))
                //        {
                //            MessageBox.Show("数据发送失败，请检查通讯线路！");
                //            return;
                //        }
                //        if (j % (allNumber / 50) == 0)
                //        {
                //            BeginInvoke(new Action(() =>
                //            {
                //                pgbLoad.Value = 30 + (j / (allNumber / 50));//进度百分之N
                //                lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //            }));
                //        }
                //        Thread.Sleep(20);
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 90;//进度百分之90
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "数据发送完成，下位机正在写入Flash...";
                //    }));
                //    CanCom.Instance.WashMix_Recevice(2, 18000 - 2 * allNumber / 7);//等待时间为3分钟 减去之前的每次发送的 20ms延时。
                //    if (CanCom.Instance.WashMixPara == 1)
                //    {
                //        BeginInvoke(new Action(() =>
                //        {
                //            pgbLoad.Value = 100;//进度百分之100
                //            lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //            lblDescribe.Text = "程序下载成功！";
                //        }));
                //    }
                //    else
                //    {
                //        MessageBox.Show("程序下载失败，请重新烧录！");
                //        return;
                //    }
                //}
                #endregion
            }
            else if (selectZhenID == 8)//扫码器10
            {
                #region 扫码器程序烧录
                //BeginInvoke(new Action(() => { BeginInvoke(new Action(() => { lblDescribe.Text = "系统正在复位..."; })); }));
                //if (!CanCom.Instance.Scanner_Senddata("00 00 00 00 00 00 00", 10))
                //{
                //    MessageBox.Show("系统复位失败！");
                //    return;
                //}
                //BeginInvoke(new Action(() => { lblDescribe.Text = "上下位机正在握手..."; }));
                //if (!CanCom.Instance.Scanner_Senddata("0a AA 55 00 00 00 00", 10))//判定握手成功与否
                //{
                //    MessageBox.Show("上下位机握手失败，请检测通讯线路！");
                //    return;
                //}
                //BeginInvoke(new Action(() =>
                //{
                //    pgbLoad.Value = 5;//进度百分之五
                //    lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //    lblDescribe.Text = "上下位机握手成功";
                //}));
                //if (CanCom.Instance.Scanner_CanSend("0a 00 00 00 00 00 00", 10))//判定发送Flash擦除命令成功与否
                //{
                //    BeginInvoke(new Action(() => { lblDescribe.Text = "正在擦除Flash..."; }));
                //    CanCom.Instance.Scanner_Recevice(1, 1000);//返回等待3秒
                //    if (!CanCom.Instance.Scanner_DoneFlag)
                //    {
                //        MessageBox.Show("Flash擦除失败，请重新操作！");
                //        return;
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 25;//进度百分之二十五
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "Flash擦除成功";
                //    }));
                //    if (!CanCom.Instance.Scanner_CanSend("0a 01 00 " + file_len.ToString("x8").Substring(0, 2) + " " +
                //        file_len.ToString("x8").Substring(2, 2) + " " + file_len.ToString("x8").Substring(4, 2) + " " +
                //        file_len.ToString("x8").Substring(6, 2), 10))
                //    {
                //        MessageBox.Show("字节长度写入失败，请检查通讯线路！");
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 30;//进度百分之30
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "字节长度写入成功";
                //        Thread.Sleep(100);
                //        lblDescribe.Text = "正在发送数据...";
                //    }));
                //    for (int j = 0; j < allNumber; j += 7)
                //    {
                //        if (!CanCom.Instance.Scanner_CanSend(buff[j].ToString("x2") + " "
                //            + buff[j + 1].ToString("x2") + " " + buff[j + 2].ToString("x2") + " "
                //            + buff[j + 3].ToString("x2") + " " + buff[j + 4].ToString("x2") + " "
                //            + buff[j + 5].ToString("x2") + " " + buff[j + 6].ToString("x2"), 10))
                //        {
                //            MessageBox.Show("数据发送失败，请检查通讯线路！");
                //            return;
                //        }
                //        Thread.Sleep(20);
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 90;//进度百分之90
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "数据发送完成，下位机正在写入Flash...";
                //    }));
                //    CanCom.Instance.Scanner_Recevice(2, 18000 - 2 * allNumber / 7);//等待时间为3分钟 减去之前的每次发送的 20ms延时。
                //    if (CanCom.Instance.ScannerPara == 1)
                //    {
                //        BeginInvoke(new Action(() =>
                //        {
                //            pgbLoad.Value = 100;//进度百分之100
                //            lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //            lblDescribe.Text = "程序下载成功！";
                //        }));
                //    }
                //    else
                //    {
                //        MessageBox.Show("程序下载失败，请重新烧录！");
                //        return;
                //    }
                //}
                #endregion
            }
            else if (selectZhenID == 9)//状态板11
            {
                #region 状态板程序烧录
                //BeginInvoke(new Action(() => { BeginInvoke(new Action(() => { lblDescribe.Text = "系统正在复位..."; })); }));
                //if (!CanCom.Instance.Status_Senddata("00 00 00 00 00 00 00", 10))
                //{
                //    MessageBox.Show("系统复位失败！");
                //    return;
                //}
                //BeginInvoke(new Action(() => { lblDescribe.Text = "上下位机正在握手..."; }));
                //if (!CanCom.Instance.Status_Senddata("0b AA 55 00 00 00 00", 10))//判定握手成功与否
                //{
                //    MessageBox.Show("上下位机握手失败，请检测通讯线路！");
                //    return;
                //}
                //BeginInvoke(new Action(() =>
                //{
                //    pgbLoad.Value = 5;//进度百分之五
                //    lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //    lblDescribe.Text = "上下位机握手成功";
                //}));
                //if (CanCom.Instance.Status_CanSend("0b 00 00 00 00 00 00", 10))//判定发送Flash擦除命令成功与否
                //{
                //    BeginInvoke(new Action(() => { lblDescribe.Text = "正在擦除Flash..."; }));
                //    CanCom.Instance.Status_Recevice(1, 1000);//返回等待10秒
                //    if (!CanCom.Instance.Status_DoneFlag)
                //    {
                //        MessageBox.Show("Flash擦除失败，请重新操作！");
                //        return;
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 25;//进度百分之二十五
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "Flash擦除成功";
                //    }));
                //    if (!CanCom.Instance.Status_CanSend("0b 01 00 " + file_len.ToString("x8").Substring(0, 2) + " " +
                //        file_len.ToString("x8").Substring(2, 2) + " " + file_len.ToString("x8").Substring(4, 2) + " " +
                //        file_len.ToString("x8").Substring(6, 2), 10))
                //    {
                //        MessageBox.Show("字节长度写入失败，请检查通讯线路！");
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 30;//进度百分之30
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "字节长度写入成功";
                //        Thread.Sleep(100);
                //        lblDescribe.Text = "正在发送数据...";
                //    }));
                //    for (int j = 0; j < allNumber; j += 7)
                //    {
                //        if (!CanCom.Instance.Status_CanSend(buff[j].ToString("x2") + " "
                //            + buff[j + 1].ToString("x2") + " " + buff[j + 2].ToString("x2") + " "
                //            + buff[j + 3].ToString("x2") + " " + buff[j + 4].ToString("x2") + " "
                //            + buff[j + 5].ToString("x2") + " " + buff[j + 6].ToString("x2"), 10))
                //        {
                //            MessageBox.Show("数据发送失败，请检查通讯线路！");
                //            return;
                //        }
                //        Thread.Sleep(20);
                //    }
                //    BeginInvoke(new Action(() =>
                //    {
                //        pgbLoad.Value = 90;//进度百分之90
                //        lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //        lblDescribe.Text = "数据发送完成，下位机正在写入Flash...";
                //    }));
                //    CanCom.Instance.Status_Recevice(2, 18000 - 2 * allNumber / 7);//等待时间为3分钟 减去之前的每次发送的 20ms延时。
                //    if (CanCom.Instance.StatusPara == 1)
                //    {
                //        BeginInvoke(new Action(() =>
                //        {
                //            pgbLoad.Value = 100;//进度百分之100
                //            lblPercentage.Text = pgbLoad.Value.ToString() + "%";
                //            lblDescribe.Text = "程序下载成功！";
                //        }));
                //    }
                //    else
                //    {
                //        MessageBox.Show("程序下载失败，请重新烧录！");
                //        return;
                //    }
                //}
                #endregion
            }
            else//选择错误
            {
                MessageBox.Show("电控板位选择错误，请重新选择！");
                return;
            }
            BeginInvoke(new Action(() =>
            {
                cmbZhenID.Enabled = txtFilePath.Enabled = btnSelectBin.Enabled = btnLoadProgram.Enabled = true;
                frmMain.LiquidQueryFlag = true;
            }));
        }
        private void btnSelectBin_Click(object sender, EventArgs e)
        {
            if (cmbZhenID.SelectedIndex < 0)
            {
                MessageBox.Show("未选择电控板位，请重新选择！");
                cmbZhenID.Focus();
                return;
            }
            OpenFileDialog openFile = new OpenFileDialog();
            string strPath;
            if (txtFilePath.Text == "")
            {
                strPath = System.Windows.Forms.Application.StartupPath;
            }
            else
            {
                strPath = txtFilePath.Text.ToString().Substring(0, txtFilePath.Text.LastIndexOf(@"\"));
            }
            //openFile.InitialDirectory = System.Windows.Forms.Application.StartupPath;
            openFile.InitialDirectory = strPath;
            openFile.Filter = "bin文件|*.bin";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = openFile.FileName;
            }
            openFile.Dispose();
        }
        #endregion


        #region 温度监控与校准

        private int GetNumber1()//20-40随机数生成
        {
            Random rd = new Random();
            return rd.Next(20, 41);
        }

        private int GetNumber0()//0-20随机数生成
        {
            Random rd = new Random();
            return rd.Next(0, 21);
        }
        /// <summary>
        /// 试剂盘温度列表
        /// </summary>
        List<decimal> Rlist = new List<decimal>();//试剂盘
        /// <summary>
        /// 温育盘温度列表
        /// </summary>
        List<decimal> Wlist = new List<decimal>();//温育盘
        /// <summary>
        /// 清洗盘温度列表
        /// </summary>
        BindingList<decimal> Qlist = new BindingList<decimal>();//清洗盘
        /// <summary>
        /// 底物管路温度列表
        /// </summary>
        BindingList<decimal> Dlist = new BindingList<decimal>();//底物
        /// <summary>
        /// 清洗管路温度列表
        /// </summary>
        BindingList<decimal> Qglist = new BindingList<decimal>();//底物
        decimal timespan;//时间间隔
        double numOfSam, num1, num2;//采样数、y轴下界、y轴上界

        /// <summary>
        /// 读取配置文件,InstrumentPara,tempreature
        /// </summary>
        private void iniReader()
        {
            numOfSam = double.Parse(Common.OperateIniFile.ReadInIPara("temperature", "numOfSam"));
            timespan = decimal.Parse(Common.OperateIniFile.ReadInIPara("temperature", "timespan"));
            num1 = double.Parse(Common.OperateIniFile.ReadInIPara("temperature", "num1"));
            num2 = double.Parse(Common.OperateIniFile.ReadInIPara("temperature", "num2"));
        }
        /// <summary>
        /// 更改配置文件,InstrumentPara,tempreature
        /// </summary>
        private void iniseter()
        {
            Common.OperateIniFile.WriteIniPara("temperature", "numOfSam", numOfSam.ToString());
            Common.OperateIniFile.WriteIniPara("temperature", "timespan", timespan.ToString());
            Common.OperateIniFile.WriteIniPara("temperature", "num1", num1.ToString());
            Common.OperateIniFile.WriteIniPara("temperature", "num2", num2.ToString());
        }

        /// <summary>
        /// 读数线程
        /// </summary>
        Thread ReadThread;

        private void beginAndStop_Click(object sender, EventArgs e)//开始与暂停按钮点击事件
        {
            if (beginAndStop.Text == "开始")
            {
                suspendAndContinue.Enabled = true;
                beginAndStop.Text = "终止";
                suspendAndContinue.Text = "暂停";
                //ReadThread = new Thread(Read);
                //ReadThread.Start();
                //ReadThread.IsBackground = true;
                timer1.Enabled = true;
                timer1.Start();
            }
            else
            {
                SelectTemFlag = false;
                timer1.Enabled = false;
                timer1.Stop();
                timer1.Enabled = false;
                chart1.Series["reagent"].Points.Clear();
                chart1.Series["wenyu"].Points.Clear();
                chart1.Series["qingxi"].Points.Clear();
                chart1.Series["diwu"].Points.Clear();
                chart1.Series["qxgl"].Points.Clear();//2018-07-05 zlx add
                Rlist.Clear();
                Wlist.Clear();
                Qlist.Clear();
                Dlist.Clear();
                Qglist.Clear();//2018-07-05 zlx add
                suspendAndContinue.Enabled = false;
                beginAndStop.Text = "开始";
            }
        }
        delegate void SetTextCallBack(string text);
        private void SettxtStandard(string text)
        {
            txtStandard.Text = text;
        }

        /// <summary>
        /// 读取返回的温度 2018-07-03 zlx add
        /// </summary>
        void Read(string order)//y mod 20180816
        {
            if (!order.Contains("EB 90 11 AF"))
                return;
            if (order.Contains("EB 90 11 AF 01 06 FF")) //lyq add 1126
                AgingHaveTube = true;
            string[] dataRecive = order.Split(' ');
            //decimal  readData=(decimal)Convert.ToInt32(dataRecive[12], 16)+(decimal )Convert.ToInt32(dataRecive[13], 16)
            //    +(decimal )Convert.ToInt32(dataRecive[14], 16)+(decimal )Convert.ToInt32(dataRecive[15], 16);
            decimal readData = Math.Round(Convert.ToDecimal(NetCom3.HexToFloat(dataRecive[12] + dataRecive[13] + dataRecive[14] + dataRecive[15])), 1);
            #region 读取仪器实际温度
            if (dataRecive[5] == "04")
            {
                if ((beginAndStop.Text != "开始"))
                {
                    switch (dataRecive[4])
                    {
                        case "04":
                            Wlist.Add(readData);
                            while (Wlist.Count > numOfSam)
                            {
                                Wlist.RemoveAt(0);
                            }
                            break;
                        case "05":
                            Qlist.Add(readData);
                            while (Qlist.Count > numOfSam)
                            {
                                Qlist.RemoveAt(0);
                            }
                            break;
                        case "06":
                            Qglist.Add(readData);
                            while (Qglist.Count > numOfSam)
                            {
                                Qglist.RemoveAt(0);
                            }
                            break;
                        case "07":
                            Dlist.Add(readData);
                            while (Dlist.Count > numOfSam)
                            {
                                Dlist.RemoveAt(0);
                            }
                            break;
                        case "08":
                            Rlist.Add(readData);
                            while (Rlist.Count > numOfSam)
                            {
                                Rlist.RemoveAt(0);
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    while (this == null || !this.IsHandleCreated)
                    { }
                    if (!btnSelect.Enabled)
                        this.Invoke(new SetTextCallBack(SettxtStandard), readData.ToString());
                    //this.Invoke(new SetTextCallBack(SettxtStandard), readData.ToString());
                }

            }
            #endregion
            #region 读取仪器校准温度
            if (dataRecive[5] == "06")
            {
                switch (dataRecive[4])
                {
                    case "04":
                        this.Invoke(new SetTextCallBack(SettxtStandard), readData.ToString());
                        break;
                    case "05":
                        this.Invoke(new SetTextCallBack(SettxtStandard), readData.ToString());
                        break;
                    case "06":
                        this.Invoke(new SetTextCallBack(SettxtStandard), readData.ToString());
                        break;
                    case "07":
                        this.Invoke(new SetTextCallBack(SettxtStandard), readData.ToString());
                        break;
                    case "08":
                        this.Invoke(new SetTextCallBack(SettxtStandard), readData.ToString());
                        break;
                    default:
                        break;
                }
            }
            #endregion
        }
        private void suspendAndContinue_Click(object sender, EventArgs e)//暂停与继续按钮点击事件
        {
            if (suspendAndContinue.Text == "暂停")
            {
                timer1.Enabled = false;
                suspendAndContinue.Text = "继续";
            }
            else
            {
                timer1.Enabled = true;
                suspendAndContinue.Text = "暂停";
            }
        }

        private void saveTo_Click(object sender, EventArgs e)//导出按钮点击实现导出功能
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = saveFileDialog1.FileName;

                FileStream file = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
                StreamWriter aw = new StreamWriter(file, Encoding.GetEncoding("GB2312"));
                StringBuilder sb = new StringBuilder();

                sb.Append(DateTime.Now.ToString("F") + Environment.NewLine);
                sb.Append("采样点" + "\t" + "试剂盘温度" + "\t" + "温育盘温度" + "\t" + "清洗盘温度" + "\t" + "底物温度" + "\t" + "清洗管路温度" + Environment.NewLine);

                for (int i = 1; i < Rlist.Count; i++)
                {
                    sb.Append(i.ToString() + "\t" + Rlist[i - 1] + "\t" + Wlist[i - 1] + "\t" + Qlist[i - 1] + "\t" + Dlist[i - 1] + "\t" + Qglist[i - 1] + Environment.NewLine);
                }

                aw.Write(sb.ToString());
                aw.Flush();
                file.Flush();
                aw.Close();
                file.Close();
                aw.Dispose();
                file.Dispose();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            chart1.Series[0].Enabled = chkRegent.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            chart1.Series[1].Enabled = chkWY.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            chart1.Series[2].Enabled = chkQX.Checked;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            chart1.Series[3].Enabled = chkDW.Checked;
        }

        private void saveSetting_Click(object sender, EventArgs e)//更改与保存按钮点击事件
        {
            if (saveSetting.Text == "更改设置")
            {

                saveSetting.Text = "保存设置";
                numDown.Enabled = numUp.Enabled = numOfSample.Enabled = timespanOfSample.Enabled = restore.Enabled = true;
            }
            else
            {

                numOfSam = (double)numOfSample.Value;
                timespan = timespanOfSample.Value;
                num1 = (double)numDown.Value;
                num2 = (double)numUp.Value;
                if (num1 >= num2)
                {
                    frmMsgShow.MessageShow("温度设置警告", "温度范围的选择只能从小到大，最低温度不能高于或等于最高温度");
                    numDown.Focus();
                    return;
                }
                timer1.Interval = Convert.ToInt32(timespan) * 1000;
                chart1.ChartAreas[0].AxisX.Maximum = numOfSam;
                chart1.ChartAreas[0].AxisY.Minimum = num1;
                chart1.ChartAreas[0].AxisY.Maximum = num2;
                iniseter();//保存一次配置文件
                saveSetting.Text = "更改设置";
                numDown.Enabled = numUp.Enabled = numOfSample.Enabled = timespanOfSample.Enabled = restore.Enabled = false;
            }
        }

        private void restore_Click(object sender, EventArgs e)//取消按钮
        {
            if (restore.Enabled == true)
            {
                numOfSample.Value = (decimal)numOfSam;
                timespanOfSample.Value = timespan;
                numDown.Value = (decimal)num1;
                numUp.Value = (decimal)num2;

                saveSetting.Text = "更改设置";
                numDown.Enabled = numUp.Enabled = numOfSample.Enabled = timespanOfSample.Enabled = restore.Enabled = false;
            }
        }
        bool SelectTemFlag;
        private void timer1_Tick(object sender, EventArgs e)
        {
            //2018-07-02 zlx mod
            if (!NetCom3.totalOrderFlag || SelectTemFlag)
            {
                return;
            }
            SelectTemFlag = true;
            if (chkWY.Checked)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 04 04"), 5);
                NetCom3.Instance.SingleQuery();
            }
            if (chkQX.Checked)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 05 04"), 5);
                NetCom3.Instance.SingleQuery();
            }
            if (chkDW.Checked)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 07 04"), 5);
                NetCom3.Instance.SingleQuery();
            }
            if (chkQXGL.Checked)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 06 04"), 5);
                NetCom3.Instance.SingleQuery();
            }
            if (chkRegent.Checked)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 08 04"), 5);
                if (!reagentColdQuery())
                {
                    SelectTemFlag = false;
                    timer1.Enabled = false;
                    timer1.Stop();
                    timer1.Enabled = false;
                    chart1.Series["reagent"].Points.Clear();
                    chart1.Series["wenyu"].Points.Clear();
                    chart1.Series["qingxi"].Points.Clear();
                    chart1.Series["diwu"].Points.Clear();
                    chart1.Series["qxgl"].Points.Clear();//2018-07-05 zlx add
                    Rlist.Clear();
                    Wlist.Clear();
                    Qlist.Clear();
                    Dlist.Clear();
                    Qglist.Clear();//2018-07-05 zlx add                    
                    suspendAndContinue.Enabled = false;

                    frmMsgShow.MessageShow("ERROR", "请检查是否打开制冷开关.");


                    beginAndStop.Text = "开始";
                    return;
                }
                //NetCom3.Instance.SingleQuery();
            }

            NetCom3.Delay(100);
            //Rlist.Add((GetNumber0() + 20) / 2 - 5);//获得温度信息，后期改为指令查询，下同
            //Wlist.Add((GetNumber1() + 40) / 2);
            //Qlist.Add((GetNumber1() + 20) / 2);
            //Dlist.Add((GetNumber1() + 100) / 6);
            //Qglist.Add((GetNumber1() + 60) / 6);
            if (Wlist.Count == 0 && Qlist.Count == 0 && Dlist.Count == 0 && Qglist.Count == 0 && Rlist.Count == 0)
                return;
            //while (Rlist.Count > numOfSam)
            //{
            //    Rlist.RemoveAt(0);
            //}
            if (chart1.IsDisposed || chart1 == null) //防止nullreferencerefresh异常 jun add 20190426
            {
                return;
            }
            try
            {
                chart1.Series["reagent"].Points.DataBindY(Rlist);
                //while (Wlist.Count > numOfSam)
                //{
                //    Wlist.RemoveAt(0);
                //}
                chart1.Series["wenyu"].Points.DataBindY(Wlist);
                //while (Qlist.Count > numOfSam)
                //{
                //    Qlist.RemoveAt(0);
                //}
                chart1.Series["qingxi"].Points.DataBindY(Qlist);
                //while (Dlist.Count > numOfSam)
                //{
                //    Dlist.RemoveAt(0);
                //}
                chart1.Series["diwu"].Points.DataBindY(Dlist);
                //while (Qglist.Count > numOfSam)
                //{
                //    Qglist.RemoveAt(0);
                //}
                chart1.Series["qxgl"].Points.DataBindY(Qglist);
            }
            catch { }
            NetCom3.Delay(10);
            //chart1.Series["reagent"].Points.DataBindY(Rlist);
            //chart1.Series["wenyu"].Points.DataBindY(Wlist);
            //chart1.Series["qingxi"].Points.DataBindY(Qlist);
            //chart1.Series["diwu"].Points.DataBindY(Dlist);
            //chart1.Series["qxgl"].Points.DataBindY(Qglist);
            if (Rlist.Count > 0)
                labreagent.Text = Rlist[Rlist.Count - 1] + "";
            if (Wlist.Count > 0)
                labwenyu.Text = Wlist[Wlist.Count - 1] + "";
            if (Qlist.Count > 0)
                labqingxi.Text = Qlist[Qlist.Count - 1] + "";
            if (Dlist.Count > 0)
                labdiwu.Text = Dlist[Dlist.Count - 1] + "";
            if (Qglist.Count > 0)
                labqxgl.Text = Qglist[Qglist.Count - 1] + "";
            SelectTemFlag = false;
        }

        //private void numDown_ValueChanged(object sender, EventArgs e)//上界值不能小于下界
        //{
        //    numUp.Minimum = numDown.Value;
        //}

        #endregion

        #region 清洗盘
        private void cmbWashPara_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            cmbWashPara.Enabled = false;
            //压杯开始位置
            if (cmbWashPara.SelectedIndex == 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 01 01"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //压杯最低位置
            else if (cmbWashPara.SelectedIndex == 1)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 01 02"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //调整清洗液注液量
            else if (cmbWashPara.SelectedIndex == 2)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 01 03"), 5);
                NetCom3.Instance.SingleQuery();
            }
            /*
            //夹管开始位置
            if (cmbWashPara.SelectedIndex == 0)
            {
                //y decide nouse 20180517
                //cmbWashElecMachine.Items.Clear();
                //cmbWashElecMachine.Items.Add("清洗盘电机");
                //cmbWashElecMachine.Items.Add("Z轴电机");
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 03 31 30"), 5);
                NetCom3.Instance.SingleQuery();

            }
            //夹管最低位置
            else if (cmbWashPara.SelectedIndex == 1)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 03 31 31"), 5);
                NetCom3.Instance.SingleQuery();

            }
            //压杯底部位置
            else if (cmbWashPara.SelectedIndex == 2)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 03 32 31"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //压杯顶部位置
            else if (cmbWashPara.SelectedIndex == 3)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 03 32 30"), 5);
                NetCom3.Instance.SingleQuery();
            }
             */
            cmbWashPara.Enabled = true;

            UpdateWashSteps();
        }

        private void UpdateWashSteps() 
        {
            if (cmbWashPara.SelectedIndex < 0 
                || cmbWashElecMachine.SelectedIndex < 0) return;

            string wash = string.Empty;
            switch (cmbWashElecMachine.Text) 
            {
                case "Z轴电机":
                    wash = "01";
                    break;
                case "清洗盘电机":
                    wash = "02";
                    break;
                case "压杯电机":
                    wash = "03";
                    break;
                default:
                    return;
            }
            txtWashCurrent.Text =
                GetCurrentWashSteps((cmbWashPara.SelectedIndex + 1).ToString().PadLeft(2, '0'),
                wash, "03");
            
        }
        private void fbtnWashAdd_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbWashElecMachine.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择需调试的电机！");
                return;
            }
            if (txtWashIncream.Text == "")
            {
                frmMsgShow.MessageShow("仪器调试", "请输入增量值！");
                txtWashIncream.Focus();
                return;
            }
            fbtnWashAdd.Enabled = false;
            string incream = int.Parse(txtWashIncream.Text.Trim()).ToString("x8");
            //清洗盘电机
            if (cmbWashElecMachine.SelectedIndex == 0)
            {
                //20180524 zlx mod
                //NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 01 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                //    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);

                NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 02 02 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            //Z轴电机
            else if (cmbWashElecMachine.SelectedIndex == 1)
            {
                //20180524 zlx mod
                //NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 02 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                //       + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 02 01 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                       + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            //压杯电机
            else if (cmbWashElecMachine.SelectedIndex == 2)
            {
                //20180524 zlx mod
                //NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 08 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                //   + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 02 03 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                   + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            fbtnWashAdd.Enabled = true;

        }

        private void fbtnWashSub_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbWashElecMachine.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择需调试的电机！");
                return;
            }
            if (txtWashIncream.Text == "")
            {
                frmMsgShow.MessageShow("仪器调试", "请输入增量值！");
                txtWashIncream.Focus();
                return;
            }
            fbtnWashSub.Enabled = false;
            string incream = int.Parse("-" + txtWashIncream.Text.Trim()).ToString("x8");
            //清洗盘电机
            if (cmbWashElecMachine.SelectedIndex == 0)
            {
                //20180524 zlx add
                //NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 01 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                //    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 02 02 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                  + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            //Z轴电机
            else if (cmbWashElecMachine.SelectedIndex == 1)
            {
                //20180524 zlx add
                //NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 02 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                //       + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 02 01 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                       + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            //压杯电机
            else if (cmbWashElecMachine.SelectedIndex == 2)
            {
                //20180524 zlx mod
                //NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 08 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                //   + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 02 03 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                   + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            fbtnWashSub.Enabled = true;
        }

        private void fbtnWashSave_Click(object sender, EventArgs e)
        {

            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            fbtnWashSave.Enabled = false;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 03 00"), 5);
            NetCom3.Instance.SingleQuery();
            NetCom3.Delay(500);//lyq add 20201016 指令返回太快，增加一个点击停顿
            fbtnWashSave.Enabled = true;

            UpdateWashSteps();
            /*
            if (cmbWashPara.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择参数名！");
                return;
            }
            if (cmbWashElecMachine.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择需调试的电机！");
                return;
            }
            //清洗盘电机
            if (cmbWashElecMachine.SelectedIndex == 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 01 13"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //Z轴电机
            else if (cmbWashElecMachine.SelectedIndex == 1)
            {
                //夹管开始位置
                if (cmbWashPara.SelectedIndex == 0)
                {
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 03 13"), 5);
                    NetCom3.Instance.SingleQuery();
                }
                //夹管最低位置
                else if (cmbWashPara.SelectedIndex == 1)
                {
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 04 13"), 5);
                    NetCom3.Instance.SingleQuery();
                }
                //参数名错误提示//this block add y 20180517 
                else
                {
                    frmMsgShow.MessageShow("参数名错误", "为Z轴电机所选的参数名非法，请重新选择。");
                }
            }
            else if (cmbWashElecMachine.SelectedIndex == 2)
            {
                //压杯底部位置
                if (cmbWashPara.SelectedIndex == 2)
                {
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 0A 13"), 5);
                    NetCom3.Instance.SingleQuery();
                }
                //压杯顶部位置
                else if (cmbWashPara.SelectedIndex == 3)
                {
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 09 13"), 5);
                    NetCom3.Instance.SingleQuery();
                }
                //参数名错误提示//this block add y 20180517 
                else
                {
                    frmMsgShow.MessageShow("参数名错误", "为压杯电机所选的参数名非法，请重新选择。");
                }
            }*/
        }



        //20190118 YLS add 清洗盘目标孔位移动功能
        private void fbtnHoleTarget_Click_1(object sender, EventArgs e)
        {
            fbtnHoleTarget.Enabled = false;
            if (!(NetCom3.totalOrderFlag||NetCom3.WashReciveFlag))
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (txtHoleTarget.Text.Trim() == "")
            {
                frmMsgShow.MessageShow("仪器调试", "请输入目标孔位的位置！");
                txtHoleTarget.Focus();
                return;
            }
            int holetarget = int.Parse(txtHoleTarget.Text.Trim());
            string holetargethex = holetarget.ToString("x2");
            if (holetarget == 0 || holetarget == 30)
            {
                frmMsgShow.MessageShow("仪器调试", "当前孔位没有变化！");
                txtHoleTarget.Focus();
                fbtnHoleTarget.Enabled = true;
            }

            else if (holetarget > 0 && holetarget < 30)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 02" + " " + holetargethex.Substring(0, 2)), 2);
                //NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + holetargethex.Substring(0, 2)), 2);
                NetCom3.Instance.WashQuery();
                //frmMsgShow.MessageShow("仪器调试", "清洗盘正在转动，请稍候！");    
                fbtnHoleTarget.Enabled = true;
            }
            else if (holetarget > 30)
            {
                frmMsgShow.MessageShow("仪器调试", "孔位支持移动的范围为0~30！");
                txtHoleTarget.Focus();
            }
            fbtnHoleTarget.Enabled = true;
        }

        private void fbtnPeristalticPSave_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            //20180524 zlx mod
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 03 00"), 5);
            NetCom3.Instance.SingleQuery();
            //if (txtPeristalticPVol.Text.Trim() == "")
            //{
            //    frmMsgShow.MessageShow("仪器调试", "请输入注液泵注液量！");
            //    txtPeristalticPVol.Focus();
            //    return;
            //}
            //fbtnPeristalticPSave.Enabled = false;
            //string pumpVol = int.Parse(txtPeristalticPVol.Text.Trim()).ToString("x8");
            //NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 07 13 " + pumpVol.Substring(0, 2) + " " + pumpVol.Substring(2, 2) + " "
            //        + pumpVol.Substring(4, 2) + " " + pumpVol.Substring(6, 2)), 5);
            //NetCom3.Instance.SingleQuery();
            fbtnPeristalticPSave.Enabled = true;
        }

        private void fbtnWashZEx_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbWashZ.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择Z轴动作！");
            }
            fbtnWashZEx.Enabled = false;
            if (cmbWashZ.SelectedIndex == 0)
            {
                //NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 01 02"), 5);
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 03 31 30"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbWashZ.SelectedIndex == 1)
            {
                //NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 01 01"), 5);
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 03 31 31"), 5);
                NetCom3.Instance.SingleQuery();
            }
            fbtnWashZEx.Enabled = true;
        }

        private void fbtnWashPressCupEx_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbWashPressCup.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择压杯电机动作！");
            }
            fbtnWashPressCupEx.Enabled = false;
            if (cmbWashPressCup.SelectedIndex == 0)
            {
                //NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 01 01"), 5);
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 03 32 30"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbWashPressCup.SelectedIndex == 1)
            {
                //NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 01 02"), 5);
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 03 32 31"), 5);
                NetCom3.Instance.SingleQuery();
            }
            fbtnWashPressCupEx.Enabled = true;
        }

        private void fbtnWashReset_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            fbtnWashReset.Enabled = false;
            //NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 00"), 5);
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 00 00"), 5);// 20180524 zlx add
            NetCom3.Instance.SingleQuery();
            fbtnWashReset.Enabled = true;

            UpdateWashSteps();
        }

        private void fbtnWashTrayReset_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            fbtnWashTrayReset.Enabled = false;
            //NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 01"), 5);
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 00 02"), 5);//20180524 zlx mod
            NetCom3.Instance.SingleQuery();
            fbtnWashTrayReset.Enabled = true;
        }

        private void fbtnWashZReset_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            fbtnWashZReset.Enabled = false;
            // NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 02 00"), 5);
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 00 01"), 5);//20180524 zlx mod
            NetCom3.Instance.SingleQuery();
            fbtnWashZReset.Enabled = true;
        }

        private void fbtnWashPressCupReset_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            fbtnWashPressCupReset.Enabled = false;
            //NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 08 00"), 5);
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 00 03"), 5);//20180524 zlx mod
            NetCom3.Instance.SingleQuery();
            fbtnWashPressCupReset.Enabled = true;
        }

        #endregion


        #region 老化测试
        /// <summary>
        /// 控件初始化状态
        /// </summary>
        void ControlIntit()
        {
            fbtnMoveAgingStart.Enabled = true;
            fbtnMoveAgingStop.Enabled = false;
            fbtnArmAgingStart.Enabled = true;
            fbtnArmAgingStop.Enabled = false;
            fbtnWashAgingStart.Enabled = true;
            fbtnWashAgingStop.Enabled = false;
            showLog = "";
            LogFileRtest.Instance.Write(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "老化测试指令执行次数为：" + LogFileRtest.Instance.Testcount);
            LogFileRtest.Instance.Bcount = false;
            LogFileRtest.Instance.Testcount = 0;
            Invoke(new Action(() =>
            {
                txtmoveNum.ReadOnly = false;
            }));
            threadList.RemoveAll(xy => xy.Name == "AgingTestRun");
            NetCom3.Instance.ReceiveHandel -= dealAgingBack;
        }
        private void fbtnMoveAgingStart_Click(object sender, EventArgs e)
        {
            if (!MoveAgingCondition())
            {
                return;
            }
            txtmoveNum.ReadOnly = true; //lyq add 20190826
            txtAgingInfoShow.Clear();
            txtTestErrorShow.Clear();
            //washTrayTubeClear();
            //reactTrayTubeClear();
            //CurrentTubePos = Convert.ToInt32(cmbRackPos.Text);//2018-08-26 zlx mod
            AgingTestRun = new Thread(new ThreadStart(TestRun));
            AgingTestRun.IsBackground = true;
            AgingTestRun.Start();
        }

        /// <summary>
        /// 移管手老化测试之前的状态
        /// </summary>
        bool MoveAgingCondition()
        {
            if (txtmoveNum.Text.Trim() == "" || txtmoveNum.Text.Trim() == "0")
            {
                frmMsgShow.MessageShow("仪器调试", "请输入测试次数！");
                txtmoveNum.Focus();
                return false;
            }
            #region 控件变量初始化
            //CurrentTubePos = 1;
            CurrentReactPos = 1;
            fbtnMoveAgingStop.Enabled = true;
            fbtnMoveAgingStart.Enabled = fbtnArmAgingStart.Enabled = fbtnArmAgingStop.Enabled
                = fbtnWashAgingStart.Enabled = fbtnWashAgingStop.Enabled = false;
            #endregion
            return true;
        }

        private void fbtnMoveAgingStop_Click(object sender, EventArgs e)
        {
            txtmoveNum.ReadOnly = false;
            NetCom3.Instance.ReceiveHandel -= dealAgingBack;
            if (AgingTestRun != null)
                AgingTestRun.Abort();
            ControlIntit();
        }

        private void fbtnArmAgingStart_Click(object sender, EventArgs e)
        {
            if (!ArmAgingCondition())
            {
                return;
            }
            txtAgingInfoShow.Clear();
            txtTestErrorShow.Clear();
            reactTrayTubeClear();
            AgingTestRun = new Thread(new ThreadStart(TestRun));
            AgingTestRun.IsBackground = true;
            AgingTestRun.Start();

        }

        bool ArmAgingCondition()
        {
            if (cmbArmRegentPos.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择试剂位！");
                return false;
            }
            if (txtArmAgingNum.Text.Trim() == "")
            {
                frmMsgShow.MessageShow("仪器调试", "请输入测试次数！");
                txtArmAgingNum.Focus();
                return false;
            }
            #region 控件变量初始化
            //CurrentTubePos = 1;
            CurrentReactPos = 1;
            CurrentAsPos = 1;
            fbtnArmAgingStop.Enabled = true;
            fbtnMoveAgingStart.Enabled = fbtnArmAgingStart.Enabled = fbtnMoveAgingStop.Enabled
                = fbtnWashAgingStart.Enabled = fbtnWashAgingStop.Enabled = false;
            #endregion
            return true;
        }

        private void fbtnArmAgingStop_Click(object sender, EventArgs e)
        {
            if (AgingTestRun != null)
                AgingTestRun.Abort();
            NetCom3.Instance.ReceiveHandel -= dealAgingBack;
            ControlIntit();
        }

        private void fbtnWashAgingStart_Click(object sender, EventArgs e)
        {
            if (!WashAgingCondition())
            {
                return;
            }
            txtAgingInfoShow.Clear();
            txtTestErrorShow.Clear();
            washTrayTubeClear();
            AgingTestRun = new Thread(new ThreadStart(TestRun));
            AgingTestRun.IsBackground = true;
            AgingTestRun.Start();
        }
        bool WashAgingCondition()
        {
            if (txtAgingWashNum.Text.Trim() == "")
            {
                frmMsgShow.MessageShow("仪器调试", "请输入测试次数！");
                txtAgingWashNum.Focus();
                return false;
            }
            #region 控件变量初始化
            //CurrentTubePos = 1;
            fbtnWashAgingStop.Enabled = true;
            fbtnMoveAgingStart.Enabled = fbtnArmAgingStart.Enabled = fbtnMoveAgingStop.Enabled
                = fbtnWashAgingStart.Enabled = fbtnWashAgingStart.Enabled = fbtnArmAgingStop.Enabled = false;
            #endregion
            return true;
        }
        private void fbtnWashAgingStop_Click(object sender, EventArgs e)
        {
            if (AgingTestRun != null)
                AgingTestRun.Abort();
            NetCom3.Instance.ReceiveHandel -= dealAgingBack;
            ControlIntit();
        }

        /// <summary>
        /// 老化测试运行
        /// </summary>
        void TestRun()
        {
            //监听“31 A1”返回指令
            NetCom3.Instance.ReceiveHandel += dealAgingBack;
            AgingTestRun.Name = "AgingTestRun";
            threadList.Add(AgingTestRun);
            if (fbtnMoveAgingStop.Enabled)
            {
                #region 移管手
                LogFileRtest.Instance.Bcount = true;
                LogFileRtest.Instance.Testcount = 0;
                //总测试数
                int testNum = int.Parse(txtmoveNum.Text);
                //夹管临时计数
                int TubetempNum = 1;
                //总计数
                int SumNum = 1;
                //单次循环数
                int singleLooPNum = testNum;
                //CurrentTubePos = 1;
                CurrentReactPos = 1;
                //int i = CurrentTubePos;
                //lyq add 20190826
                int surplusNum = testNum;
                BeginInvoke(new Action(() =>
                {
                    textSurplusNum.Text = surplusNum.ToString();
                }));
                if (rdbRackIN.Checked)
                {
                    #region 管架与温育盘之间
                    LogFileRtest.Instance.Write(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "移管手老化测试开始，暂存盘与温育盘之间移管测试，测试次数：" + testNum.ToString());
                    if (!reactTrayTubeClear())//2018-09-26
                    {
                        ControlIntit();
                        return;
                    }
                    BeginInvoke(new Action(() =>
                    {
                        txtAgingInfoShow.AppendText("-----移管手老化测试开始，管架与温育盘之间移管测试，测试次数：" + testNum.ToString() + "-----" + Environment.NewLine);
                    }));
                    if (testNum <= ReactTrayNum)
                    {
                        SumNum = 1;
                    }
                    else
                    {
                        SumNum = testNum % ReactTrayNum == 0 ? testNum / ReactTrayNum : testNum / ReactTrayNum + 1;
                        singleLooPNum = ReactTrayNum;
                    }
                    for (int j = 0; j < SumNum; j++)
                    {
                        //if (CurrentTubePos == 353)
                        //{
                        //    CurrentTubePos = 1;
                        //}
                        TubetempNum = 1;
                        CurrentReactPos = 1;
                        while (TubetempNum <= singleLooPNum)
                        {
                            #region 管架夹管到温育盘
                            //int plate = CurrentTubePos % 88 == 0 ? CurrentTubePos / 88 - 1 : CurrentTubePos / 88;//几号板
                            //int column = CurrentTubePos % 11 == 0 ? CurrentTubePos / 11 - (plate * 8) : CurrentTubePos / 11 + 1 - (plate * 8);
                            //int hole = CurrentTubePos % 11 == 0 ? 11 : CurrentTubePos % 11;

                            BeginInvoke(new Action(() =>
                            {
                                txtAgingInfoShow.AppendText("移管手夹到温育盘" + CurrentReactPos.ToString() + "位置" + Environment.NewLine);
                            }));
                            //管架取管放到温育盘
                            ReactNewTube:
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 01 " + CurrentReactPos.ToString("x2")), 1);
                            if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                            {
                                if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.LackTube)
                                {
                                    AgingHaveTube = false;
                                    while (!AgingHaveTube)
                                    {
                                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 06"), 5);
                                        NetCom3.Instance.SingleQuery();

                                        int delay = 1000;
                                        while (!AgingHaveTube && delay > 0)
                                        {
                                            NetCom3.Delay(10);
                                            delay -= 10;
                                        }
                                        if (AgingHaveTube)
                                        {
                                            AgingHaveTube = false;
                                            goto ReactNewTube;
                                        }
                                    }
                                }
                                BeginInvoke(new Action(() =>
                                {
                                    txtAgingInfoShow.AppendText("\n管架取管放到温育盘异常");
                                }));
                                ControlIntit();
                                return;
                            }
                            //NetCom3.Instance.MoveQuery();
                            if (LogFileRtest.Instance.Bcount)
                                LogFileRtest.Instance.Testcount++;
                            #region 取放管成功
                            //移管手要夹的下一个管架位置
                            //CurrentTubePos = CurrentTubePos + 1 == 353 ? 1 : CurrentTubePos + 1;
                            CurrentReactPos++;
                            #endregion
                            #endregion
                            TubetempNum++;
                            //lyq add 20190826
                            surplusNum--;
                            BeginInvoke(new Action(() =>
                            {
                                textSurplusNum.Text = surplusNum.ToString();
                            }));
                        }
                        TubetempNum = 1;
                        //CurrentTubePos = CurrentTubePos - singleLooPNum < 0 ? CurrentTubePos - singleLooPNum + 352 : CurrentTubePos - singleLooPNum;
                        CurrentReactPos = 1;
                        while (CurrentReactPos <= singleLooPNum)
                        {
                            #region 温育盘扔管
                            BeginInvoke(new Action(() =>
                            {
                                txtAgingInfoShow.AppendText("移管手将温育盘" + CurrentReactPos.ToString() + "位置的管扔掉" + Environment.NewLine);
                            }));
                            //温育盘夹管到管架
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 " + CurrentReactPos.ToString("x2")), 1);
                            if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    txtAgingInfoShow.AppendText("\n温育盘夹管到管架异常");
                                }));
                                ControlIntit();
                                return;
                            }
                            /*
                            int plate = CurrentTubePos % 88 == 0 ? CurrentTubePos / 88 - 1 : CurrentTubePos / 88;//几号板
                            int column = CurrentTubePos % 11 == 0 ? CurrentTubePos / 11 - (plate * 8) : CurrentTubePos / 11 + 1 - (plate * 8);
                            int hole = CurrentTubePos % 11 == 0 ? 11 : CurrentTubePos % 11;
                            BeginInvoke(new Action(() =>
                            {
                                txtAgingInfoShow.AppendText("移管手夹温育盘" + CurrentReactPos.ToString() + "位置的管到管架" + (plate + 1).ToString() + "号板，第" + column.ToString() + "列,第"
                                    + hole.ToString() + "孔" + Environment.NewLine);
                            }));
                            //温育盘夹管到管架
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 09 " + plate.ToString("x2") + " " + column.ToString("x2")
                                       + " " + hole.ToString("x2") + " " + CurrentReactPos.ToString("x2")), 1);
                            if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
                            {
                                ControlIntit();
                                return;
                            }                           
                            //移管手要夹的下一个管架位置
                            CurrentTubePos = CurrentTubePos + 1 == 353 ? 1 : CurrentTubePos + 1;
                            CurrentReactPos++;
                            if (CurrentReactPos == ReactTrayHoleNum + 1)
                            {
                                CurrentReactPos = 1;
                            }
                            */
                            #endregion 取放管成功
                            if (LogFileRtest.Instance.Bcount)
                                LogFileRtest.Instance.Testcount++;
                            CurrentReactPos++;
                        }
                        //下一次循环次数
                        singleLooPNum = SumNum - (j + 2) == 0 ? testNum - (j + 1) * ReactTrayNum : ReactTrayNum;
                    }
                    #endregion
                }
                else if (rdbRackWash.Checked)
                {
                    #region 管架与清洗盘之间
                    LogFileRtest.Instance.Write(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "移管手老化测试开始，暂存盘与清洗盘之间移管测试，测试次数：" + testNum.ToString());
                    if (!washTrayTubeClear())//2018-09-26
                    {
                        ControlIntit();
                        return;
                    }
                    BeginInvoke(new Action(() =>
                    {
                        txtAgingInfoShow.AppendText("-----移管手老化测试开始，管架与清洗盘之间移管测试，测试次数：" + testNum.ToString() + "-----" + Environment.NewLine);
                    }));
                    if (testNum <= 30)
                    {
                        SumNum = 1;
                    }
                    else
                    {
                        SumNum = testNum % 30 == 0 ? testNum / 30 : testNum / 30 + 1;
                        singleLooPNum = 30;
                    }
                    for (int j = 0; j < SumNum; j++)
                    {
                        //if (CurrentTubePos == 353)
                        //{
                        //    CurrentTubePos = 1;
                        //}
                        TubetempNum = 1;
                        while (TubetempNum <= singleLooPNum)
                        {
                            #region 管架夹管到清洗盘
                            //int plate = CurrentTubePos % 88 == 0 ? CurrentTubePos / 88 - 1 : CurrentTubePos / 88;//几号板
                            //int column = CurrentTubePos % 11 == 0 ? CurrentTubePos / 11 - (plate * 8) : CurrentTubePos / 11 + 1 - (plate * 8);
                            //int hole = CurrentTubePos % 11 == 0 ? 11 : CurrentTubePos % 11;

                            BeginInvoke(new Action(() =>
                            {
                                txtAgingInfoShow.AppendText(TubetempNum.ToString() + ":移管手夹到清洗盘" + Environment.NewLine);
                            }));
                            //管架取管放到清洗盘
                            WashNewTube:
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06"), 1);
                            if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                            {
                                if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.LackTube)
                                {
                                    AgingHaveTube = false;
                                    while (!AgingHaveTube)
                                    {
                                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 06"), 5);
                                        NetCom3.Instance.SingleQuery();

                                        int delay = 1000;
                                        while (!AgingHaveTube && delay > 0)
                                        {
                                            NetCom3.Delay(10);
                                            delay -= 10;
                                        }
                                        if (AgingHaveTube)
                                        {
                                            AgingHaveTube = false;
                                            goto WashNewTube;
                                        }
                                    }
                                }
                                BeginInvoke(new Action(() =>
                                {
                                    txtAgingInfoShow.AppendText("\n管架取管放到清洗盘异常");
                                }));
                                ControlIntit();
                                return;
                            }
                            //移管手要夹的下一个管架位置
                            //CurrentTubePos = CurrentTubePos + 1 == 353 ? 1 : CurrentTubePos + 1;
                            if (TubetempNum != singleLooPNum)
                            {
                                //清洗盘逆时针旋转一位
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                                if (!NetCom3.Instance.WashQuery())
                                {
                                    BeginInvoke(new Action(() =>
                                    {
                                        txtAgingInfoShow.AppendText("\n清洗盘逆时针旋转异常");
                                    }));
                                    ControlIntit();
                                    return;
                                }
                            }
                            #endregion
                            if (LogFileRtest.Instance.Bcount)
                                LogFileRtest.Instance.Testcount++;
                            TubetempNum++;
                            //lyq add 20190826
                            surplusNum--;
                            BeginInvoke(new Action(() =>
                            {
                                textSurplusNum.Text = surplusNum.ToString();
                            }));
                        }
                        TubetempNum = 1;
                        //CurrentTubePos = CurrentTubePos - singleLooPNum < 0 ? CurrentTubePos - singleLooPNum + 352 : CurrentTubePos - singleLooPNum;
                        while (TubetempNum <= singleLooPNum)
                        {
                            #region 清洗盘扔管
                            BeginInvoke(new Action(() =>
                            {
                                txtAgingInfoShow.AppendText("移管手从清洗盘扔第" + TubetempNum.ToString() + "个管" + Environment.NewLine);
                            }));
                            //清洗盘夹管到管架
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04"), 1);
                            if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    txtAgingInfoShow.AppendText("\n清洗盘夹管到管架异常");
                                }));
                                ControlIntit();
                                return;
                            }
                            /*
                            int plate = CurrentTubePos % 88 == 0 ? CurrentTubePos / 88 - 1 : CurrentTubePos / 88;//几号板
                            int column = CurrentTubePos % 11 == 0 ? CurrentTubePos / 11 - (plate * 8) : CurrentTubePos / 11 + 1 - (plate * 8);
                            int hole = CurrentTubePos % 11 == 0 ? 11 : CurrentTubePos % 11;

                            BeginInvoke(new Action(() =>
                            {
                                txtAgingInfoShow.AppendText(TubetempNum.ToString() + ":移管手夹清洗盘的管到管架" + (plate + 1).ToString() + "号板，第" + column.ToString() + "列,第"
                                    + hole.ToString() + "孔" + Environment.NewLine);
                            }));
                            //清洗盘取管放到管架
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 0a " + plate.ToString("x2") + " " + column.ToString("x2")
                                       + " " + hole.ToString("x2")), 1);
                            if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
                            {
                                ControlIntit();
                                return;
                            }
                            //移管手要夹的下一个管架位置
                            CurrentTubePos = CurrentTubePos + 1 == 353 ? 1 : CurrentTubePos + 1;
                             */
                            if (TubetempNum != singleLooPNum)
                            {
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-1).ToString("x2").Substring(6, 2)), 2);
                                if (!NetCom3.Instance.WashQuery())
                                {
                                    ControlIntit();
                                    return;
                                }
                            }
                            #endregion
                            if (LogFileRtest.Instance.Bcount)
                                LogFileRtest.Instance.Testcount++;
                            TubetempNum++;
                        }
                        //下一次循环次数
                        singleLooPNum = SumNum - (j + 2) == 0 ? testNum - (j + 1) * 30 : 30;
                    }
                    #endregion
                }
                //2019.5.17  hly  add
                else if (rdbRackAbadon.Checked)
                {
                    #region  暂存管取管到废弃处
                    BeginInvoke(new Action(() =>
                    {
                        txtAgingInfoShow.AppendText("-----移管手老化测试开始，管架与废弃处之间移管测试，测试次数：" + testNum.ToString() + "-----" + Environment.NewLine);
                    }));
                    LogFileRtest.Instance.Write(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "移管手老化测试开始，暂存盘与废弃处之间移管测试，测试次数：" + testNum.ToString());
                    int num = 0;
                    while (testNum > 0)
                    {
                        #region
                        BeginInvoke(new Action(() =>
                        {
                            txtAgingInfoShow.AppendText("移管手正在暂存盘扔第" + ++num + "个管" + Environment.NewLine);
                        }));
                        AgainNewMove:
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 07"), 1);
                        if (!NetCom3.Instance.MoveQuery())
                        {
                            #region 异常处理                            
                            if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked)
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    txtAgingInfoShow.AppendText("抓管撞管\n");
                                }));
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 F1 02"), 5);
                                NetCom3.Instance.SingleQuery();
                                goto AgainNewMove;
                            }
                            if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.LackTube)
                            {
                                AgingHaveTube = false;
                                while (!AgingHaveTube)
                                {
                                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 06"), 5);
                                    NetCom3.Instance.SingleQuery();

                                    int delay = 1000;
                                    while (!AgingHaveTube && delay > 0)
                                    {
                                        NetCom3.Delay(10);
                                        delay -= 10;
                                    }
                                    if (AgingHaveTube)
                                    {
                                        AgingHaveTube = false;
                                        goto AgainNewMove;
                                    }
                                }
                            }
                            BeginInvoke(new Action(() =>
                            {
                                txtAgingInfoShow.AppendText("\n管架与废弃处异常");
                            }));
                            #endregion
                            ControlIntit();
                            return;
                        }
                        if (LogFileRtest.Instance.Bcount)
                            LogFileRtest.Instance.Testcount++;
                        #endregion
                        testNum--;
                        //BeginInvoke(new Action(() =>
                        //{
                        //    txtmoveNum.Text = testNum.ToString();
                        //}));
                        surplusNum--;
                        BeginInvoke(new Action(() =>
                        {
                            textSurplusNum.Text = surplusNum.ToString();
                        }));
                    }
                    #endregion
                }
                else if (rdbtnWashIn.Checked)
                {
                    #region 清洗盘与温育盘之间取放管测试
                    LogFileRtest.Instance.Write(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "移管手老化测试开始，清洗盘与温育盘之间移管测试，测试次数：" + testNum.ToString());
                    //2018-09-26
                    if (!washTrayTubeClear())
                    {
                        ControlIntit();
                        return;
                    }
                    if (!reactTrayTubeClear())
                    {
                        ControlIntit();
                        return;
                    }
                    BeginInvoke(new Action(() =>
                    {
                        txtAgingInfoShow.AppendText("-----移管手老化测试开始，温育盘与清洗盘之间移管测试，测试次数：" + testNum.ToString() + "-----" + Environment.NewLine);
                    }));

                    //lyq add 20191010 
                    Random ra = new Random();
                    int random;
                    int[] reactTray = new int[ReactTrayNum];
                    for (int iya = reactTray.Length - 1; iya >= 0; iya--)
                    {
                        reactTray[iya] = 0;
                    }

                    if (testNum <= 30)
                    {
                        SumNum = 1;
                    }
                    else
                    {
                        SumNum = testNum % 30 == 0 ? testNum / 30 : testNum / 30 + 1;
                        singleLooPNum = 30;
                    }
                    #region 管架夹反应管到清洗盘
                    while (TubetempNum <= singleLooPNum)
                    {
                        //int plate = CurrentTubePos % 88 == 0 ? CurrentTubePos / 88 - 1 : CurrentTubePos / 88;//几号板
                        //int column = CurrentTubePos % 11 == 0 ? CurrentTubePos / 11 - (plate * 8) : CurrentTubePos / 11 + 1 - (plate * 8);
                        //int hole = CurrentTubePos % 11 == 0 ? 11 : CurrentTubePos % 11;

                        BeginInvoke(new Action(() =>
                        {
                            txtAgingInfoShow.AppendText(TubetempNum.ToString() + ":移管手夹管到清洗盘" + Environment.NewLine);
                        }));
                        //管架取管放到清洗盘
                        AgainReactAndWash:
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06"), 1);
                        if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                        {
                            if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.LackTube)
                            {
                                AgingHaveTube = false;
                                while (!AgingHaveTube)
                                {
                                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 06"), 5);
                                    NetCom3.Instance.SingleQuery();

                                    int delay = 1000;
                                    while (!AgingHaveTube && delay > 0)
                                    {
                                        NetCom3.Delay(10);
                                        delay -= 10;
                                    }
                                    if (AgingHaveTube)
                                    {
                                        AgingHaveTube = false;
                                        goto AgainReactAndWash;
                                    }
                                }
                            }
                            BeginInvoke(new Action(() =>
                            {
                                txtAgingInfoShow.AppendText("\n管架取管放到清洗盘异常");
                            }));
                            ControlIntit();
                            return;
                        }
                        if (LogFileRtest.Instance.Bcount)
                            LogFileRtest.Instance.Testcount++;
                        //移管手要夹的下一个管架位置
                        //CurrentTubePos = CurrentTubePos + 1 == 353 ? 1 : CurrentTubePos + 1;
                        if (TubetempNum != singleLooPNum)
                        {
                            //清洗盘逆时针旋转一位
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    txtAgingInfoShow.AppendText("\n清洗盘逆时针旋转一位异常");
                                }));
                                ControlIntit();
                                return;
                            }
                        }
                        TubetempNum++;
                    }
                    #endregion
                    for (int j = 0; j < SumNum; j++)
                    {
                        TubetempNum = 1;
                        CurrentReactPos = CurrentReactPos == ReactTrayNum + 1 ? 1 : CurrentReactPos;
                        while (TubetempNum <= singleLooPNum)
                        {
                            #region 清洗盘夹管到温育盘

                            //lyq add 20191010
                            random = ra.Next(1, ReactTrayNum+1);
                            while (reactTray[random - 1] != 0)
                            {
                                random = ra.Next(1, ReactTrayNum + 1);
                            }
                            CurrentReactPos = random;

                            BeginInvoke(new Action(() =>
                            {
                                txtAgingInfoShow.AppendText(TubetempNum.ToString() + ":移管手从清洗盘夹管到温育盘" + CurrentReactPos.ToString() + "位置。" + Environment.NewLine);
                            }));
                            //清洗盘夹管到温育盘
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 03 " + CurrentReactPos.ToString("x2") + " 02"), 1);
                            if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    txtAgingInfoShow.AppendText("\n清洗盘夹管到温育盘异常");
                                }));
                                ControlIntit();
                                return;
                            }
                            if (LogFileRtest.Instance.Bcount)
                                LogFileRtest.Instance.Testcount++;
                            reactTray[random - 1] = 1; //lyq 191010

                            CurrentReactPos = CurrentReactPos + 1 == ReactTrayNum + 1 ? 1 : CurrentReactPos + 1;
                            if (TubetempNum != singleLooPNum)
                            {
                                //清洗盘顺时针旋转一位
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-1).ToString("x2").Substring(6, 2)), 2);
                                if (!NetCom3.Instance.WashQuery())
                                {
                                    BeginInvoke(new Action(() =>
                                    {
                                        txtAgingInfoShow.AppendText("\n清洗盘顺时针旋转一位异常");
                                    }));
                                    ControlIntit();
                                    return;
                                }
                            }
                            #endregion
                            TubetempNum++;
                        }
                        CurrentReactPos = CurrentReactPos - singleLooPNum < 0 ? ReactTrayNum + (CurrentReactPos - singleLooPNum) : CurrentReactPos - singleLooPNum;
                        TubetempNum = 1;
                        while (TubetempNum <= singleLooPNum)
                        {
                            #region 温育盘夹管到清洗盘

                            //lyq 20191010
                            random = ra.Next(1, ReactTrayNum + 1);
                            while (reactTray[random - 1] != 1)
                            {
                                random = ra.Next(1, ReactTrayNum + 1);
                            }
                            CurrentReactPos = random;

                            BeginInvoke(new Action(() =>
                            {
                                txtAgingInfoShow.AppendText(TubetempNum.ToString() + ":移管手从温育盘" + CurrentReactPos.ToString() + "位置夹管到清洗盘" + Environment.NewLine);
                            }));
                            //温育盘夹管到清洗盘
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 02 " + CurrentReactPos.ToString("x2")), 1);
                            if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    txtAgingInfoShow.AppendText("\n温育盘夹管到清洗盘异常");
                                }));
                                ControlIntit();
                                return;
                            }
                            if (LogFileRtest.Instance.Bcount)
                                LogFileRtest.Instance.Testcount++;
                            reactTray[random - 1] = 0; //lyq 191010

                            CurrentReactPos = CurrentReactPos + 1 == ReactTrayNum + 1 ? 1 : CurrentReactPos + 1;
                            if (TubetempNum != singleLooPNum)
                            {
                                //清洗盘逆时时针旋转一位
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                                if (!NetCom3.Instance.WashQuery())
                                {
                                    BeginInvoke(new Action(() =>
                                    {
                                        txtAgingInfoShow.AppendText("\n清洗盘逆时时针旋转一位异常");
                                    }));
                                    ControlIntit();
                                    return;
                                }
                            }
                            #endregion
                          
                            TubetempNum++;
                            //lyq add 20190826
                            surplusNum--;
                            BeginInvoke(new Action(() =>
                            {
                                textSurplusNum.Text = surplusNum.ToString();
                            }));
                        }
                        //下一次循环次数
                        singleLooPNum = SumNum - (j + 2) == 0 ? testNum - (j + 1) * 30 : 30;
                    }
                    #endregion
                }
                #endregion
            }
            else if (fbtnArmAgingStop.Enabled)
            {
                //试剂盘位号
                int regentPos = 1;
                #region 加样臂
                Invoke(new Action(() =>
                {
                    //试剂盘位号
                    regentPos = int.Parse(cmbArmRegentPos.SelectedItem.ToString());
                }));
                //总测试数
                int testNum = int.Parse(txtArmAgingNum.Text);
                //加样臂动作临时计数
                int ArmtempNum = 1;
                //夹管数量
                int sumNum = testNum;
                if (testNum < ReactTrayNum)
                {
                    sumNum = testNum;
                }
                else
                {
                    sumNum = ReactTrayNum;
                }
                BeginInvoke(new Action(() =>
                {
                    txtAgingInfoShow.AppendText("-----加样臂老化测试开始，测试次数：" + testNum.ToString() + "-----" + Environment.NewLine);
                }));
                while (ArmtempNum <= testNum)
                {
                    if (chbArmIsTube.Checked)
                    {
                        BeginInvoke(new Action(() =>
                        {
                            txtAgingInfoShow.AppendText("管架夹管到温育盘" + CurrentReactPos.ToString() + "位置。" + Environment.NewLine);
                        }));
                        //int plate = CurrentTubePos % 88 == 0 ? CurrentTubePos / 88 - 1 : CurrentTubePos / 88;//几号板
                        //int column = CurrentTubePos % 11 == 0 ? CurrentTubePos / 11 - (plate * 8) : CurrentTubePos / 11 + 1 - (plate * 8);
                        //int hole = CurrentTubePos % 11 == 0 ? 11 : CurrentTubePos % 11;
                        //管架取管放到温育盘
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 01 " + CurrentReactPos.ToString("x2")), 1);
                        if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                        {
                            BeginInvoke(new Action(() =>
                            {
                                txtAgingInfoShow.AppendText("\n管架取管放到温育盘异常");
                            }));
                            ControlIntit();
                            return;
                        }
                    }
                    #region 加样
                    BeginInvoke(new Action(() =>
                    {
                        txtAgingInfoShow.AppendText(CurrentAsPos.ToString() + "号样本加到到温育盘" + CurrentReactPos.ToString() + "位置。" + Environment.NewLine);
                    }));
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 01 " + CurrentAsPos.ToString("x2") + " " + CurrentReactPos.ToString("x2")
                              + " 14"), 0);
                    if (!NetCom3.Instance.SPQuery())
                    {
                        BeginInvoke(new Action(() =>
                        {
                            txtAgingInfoShow.AppendText("\n加样异常");
                        }));
                        ControlIntit();
                        return;
                    }
                    #endregion

                    #region  加R1
                    BeginInvoke(new Action(() =>
                    {
                        txtAgingInfoShow.AppendText(regentPos.ToString() + "号试剂位R1加到到温育盘" + CurrentReactPos.ToString() + "位置。" + Environment.NewLine);
                    }));
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 03 " + regentPos.ToString("x2") + " " + CurrentReactPos.ToString("x2")
                              + " 14 05 12"), 0);
                    if (!NetCom3.Instance.SPQuery())
                    {
                        BeginInvoke(new Action(() =>
                        {
                            txtAgingInfoShow.AppendText("\n加R1异常");
                        }));
                        ControlIntit();
                        return;
                    }
                    #endregion

                    #region  加R2
                    BeginInvoke(new Action(() =>
                    {
                        txtAgingInfoShow.AppendText(regentPos.ToString() + "号试剂位R2加到到温育盘" + CurrentReactPos.ToString() + "位置。" + Environment.NewLine);
                    }));
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 04 " + regentPos.ToString("x2") + " " + CurrentReactPos.ToString("x2")
                              + " 14 05 12"), 0);
                    if (!NetCom3.Instance.SPQuery())
                    {
                        BeginInvoke(new Action(() =>
                        {
                            txtAgingInfoShow.AppendText("\n加R2异常");
                        }));
                        ControlIntit();
                        return;
                    }
                    #endregion

                    #region  加R3
                    BeginInvoke(new Action(() =>
                    {
                        txtAgingInfoShow.AppendText(regentPos.ToString() + "号试剂位R3加到到温育盘" + CurrentReactPos.ToString() + "位置。" + Environment.NewLine);
                    }));
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 05 " + regentPos.ToString("x2") + " " + CurrentReactPos.ToString("x2")
                              + " 14 05 12"), 0);
                    if (!NetCom3.Instance.SPQuery())
                    {
                        BeginInvoke(new Action(() =>
                        {
                            txtAgingInfoShow.AppendText("\n加R3异常");
                        }));
                        ControlIntit();
                        return;
                    }
                    #endregion

                    #region  加稀释液
                    /*
                    BeginInvoke(new Action(() =>
                    {
                        txtAgingInfoShow.AppendText(regentPos.ToString() + "号试剂位稀释液加到到温育盘" + CurrentReactPos.ToString() + "位置。" + Environment.NewLine);
                    }));
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 06 " + regentPos.ToString("x2") + " " + CurrentReactPos.ToString("x2")
                              + " 14"), 0);
                    if (!NetCom3.Instance.SPQuery())
                    {
                        ControlIntit();
                        return;
                    }
                    */
                    #endregion

                    #region  加磁珠
                    BeginInvoke(new Action(() =>
                    {
                        txtAgingInfoShow.AppendText(regentPos.ToString() + "号试剂位磁珠加到到温育盘" + CurrentReactPos.ToString() + "位置。" + Environment.NewLine);
                    }));
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 07 " + regentPos.ToString("x2") + " " + CurrentReactPos.ToString("x2")
                              + " 14 05 12"), 0);
                    if (!NetCom3.Instance.SPQuery())
                    {
                        BeginInvoke(new Action(() =>
                        {
                            txtAgingInfoShow.AppendText("\n加磁珠异常");
                        }));
                        ControlIntit();
                        return;
                    }
                    #endregion

                    CurrentReactPos = CurrentReactPos + 1 == ReactTrayNum + 1 ? 1 : CurrentReactPos + 1;
                    CurrentAsPos = CurrentAsPos + 1 == SampleNum + 1 ? 1 : CurrentAsPos + 1;
                    ArmtempNum++;
                }
                #region 清空温育盘反应管到管架               
                BeginInvoke(new Action(() =>
                {
                    txtAgingInfoShow.AppendText("将反应管丢弃！" + Environment.NewLine);
                }));
                if (chbArmIsTube.Checked)
                {
                    //CurrentTubePos = 1;
                    CurrentReactPos = 1;
                    for (int i = 0; i < sumNum; i++)
                    {
                        //int plate = CurrentTubePos % 88 == 0 ? CurrentTubePos / 88 - 1 : CurrentTubePos / 88;//几号板
                        //int column = CurrentTubePos % 11 == 0 ? CurrentTubePos / 11 - (plate * 8) : CurrentTubePos / 11 + 1 - (plate * 8);
                        //int hole = CurrentTubePos % 11 == 0 ? 11 : CurrentTubePos % 11;
                        //温育盘夹管到管架
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 " + CurrentReactPos.ToString("x2")), 1);
                        if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                        {
                            BeginInvoke(new Action(() =>
                            {
                                txtAgingInfoShow.AppendText("\n温育盘夹管到管架异常");
                            }));
                            ControlIntit();
                            return;
                        }
                        #region 取放管成功
                        //移管手要夹的下一个管架位置
                        //CurrentTubePos++;
                        CurrentReactPos++;
                        #endregion
                    }
                }
                #endregion
                #endregion
            }
            else if (fbtnWashAgingStop.Enabled)
            {
                #region 清洗盘

                //总测试数
                int testNum = int.Parse(txtAgingWashNum.Text);
                //加样臂动作临时计数
                int WashtempNum = 1;
                BeginInvoke(new Action(() =>
                {
                    txtAgingInfoShow.AppendText("-----清洗盘老化测试开始，测试次数：" + testNum.ToString() + "-----" + Environment.NewLine);
                }));

                #region 夹新管到清洗盘
                if (chbWashIsTube.Checked)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        BeginInvoke(new Action(() =>
                        {
                            txtAgingInfoShow.AppendText((i + 1).ToString() + ": 夹新管到清洗盘。" + Environment.NewLine);
                        }));
                        //int plate = CurrentTubePos % 88 == 0 ? CurrentTubePos / 88 - 1 : CurrentTubePos / 88;//几号板
                        //int column = CurrentTubePos % 11 == 0 ? CurrentTubePos / 11 - (plate * 8) : CurrentTubePos / 11 + 1 - (plate * 8);
                        //int hole = CurrentTubePos % 11 == 0 ? 11 : CurrentTubePos % 11;
                        //管架取管放到清洗盘
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06"), 1);
                        if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                        {
                            BeginInvoke(new Action(() =>
                            {
                                txtAgingInfoShow.AppendText("\n管架取管放到清洗盘异常");
                            }));
                            ControlIntit();
                            return;
                        }
                        //移管手要夹的下一个管架位置
                        //CurrentTubePos++;
                        if (i != 29)
                        {
                            //清洗盘逆时针旋转一位
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    txtAgingInfoShow.AppendText("\n清洗盘逆时针旋转一位异常");
                                }));
                                ControlIntit();
                                return;
                            }
                        }
                    }
                }
                #endregion

                while (WashtempNum <= testNum)
                {
                    BeginInvoke(new Action(() =>
                    {
                        txtAgingInfoShow.AppendText((WashtempNum).ToString() + ": 清洗盘旋转1位。" + Environment.NewLine);
                    }));
                    //清洗盘逆时针旋转一位
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        BeginInvoke(new Action(() =>
                        {
                            txtAgingInfoShow.AppendText("\n清洗盘逆时针旋转一位异常");
                        }));
                        ControlIntit();
                        return;
                    }
                    BeginInvoke(new Action(() =>
                    {
                        txtAgingInfoShow.AppendText((WashtempNum).ToString() + ": 清洗盘吸液、注液、加底物、读数。" + Environment.NewLine);
                    }));
                    //吸液、注液、加底物、读数
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 01 11 11 11"), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        BeginInvoke(new Action(() =>
                        {
                            txtAgingInfoShow.AppendText("\n吸液、注液、加底物、读数异常");
                        }));
                        ControlIntit();
                        return;
                    }
                    WashtempNum++;
                }

                #region 清洗盘夹回管架

                if (chbWashIsTube.Checked)
                {
                    //CurrentTubePos = 1;
                    for (int i = 0; i < 30; i++)
                    {
                        BeginInvoke(new Action(() =>
                        {
                            txtAgingInfoShow.AppendText((i + 1).ToString() + ": 清洗盘扔废管" + Environment.NewLine);
                        }));
                        //int plate = CurrentTubePos % 88 == 0 ? CurrentTubePos / 88 - 1 : CurrentTubePos / 88;//几号板
                        //int column = CurrentTubePos % 11 == 0 ? CurrentTubePos / 11 - (plate * 8) : CurrentTubePos / 11 + 1 - (plate * 8);
                        //int hole = CurrentTubePos % 11 == 0 ? 11 : CurrentTubePos % 11;
                        //管架取管放到清洗盘
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04"), 1);
                        if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                        {
                            BeginInvoke(new Action(() =>
                            {
                                txtAgingInfoShow.AppendText("\n管架取管放到清洗盘异常");
                            }));
                            ControlIntit();
                            return;
                        }
                        //移管手要夹的下一个管架位置
                        //CurrentTubePos++;
                        if (i != 29)
                        {
                            //清洗盘逆时针旋转一位
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    txtAgingInfoShow.AppendText("\n清洗盘逆时针旋转一位异常");
                                }));
                                ControlIntit();
                                return;
                            }
                        }
                    }
                }

                #endregion
                #endregion
            }
            BeginInvoke(new Action(() =>
            {
                txtAgingInfoShow.AppendText("--------------------------测试完成--------------------------" + Environment.NewLine);
                txtmoveNum.ReadOnly = false;
            }));
            ControlIntit();
            NetCom3.Instance.ReceiveHandel -= dealAgingBack;
        }

        #endregion
        private void label46_Click(object sender, EventArgs e)
        {
        }

        private void timespanOfSample_ValueChanged(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbModelName.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择要查询的温控模块！");
                return;
            }

        }

        private void makeStandard_Click(object sender, EventArgs e)
        {
            //2018-07-02 zlx add
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbModelName.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择要调试的温控模块！");
                return;
            }
            if (cmbStep.Text == "")
            {
                frmMsgShow.MessageShow("仪器调试", "请选择要调试的操作内容！");
                txtMoveIncrem.Focus();
                return;
            }
            btnmakeStandard.Enabled = false;
            switch (cmbModelName.SelectedItem.ToString())
            {
                case "温育盘":
                    switch (cmbStep.SelectedItem.ToString())
                    {
                        case "加热打开":
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 04 00"), 5);
                            break;
                        case "加热关闭":
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 04 01"), 5);
                            break;
                        case "设置校准值":
                            if (txtStandard.Text == "")
                            {
                                frmMsgShow.MessageShow("仪器调试", "请录入要校准的温度值！");
                                return;
                            }
                            //string jzwendu = (float.Parse(txtStandard.Text)).ToString("x4");
                            string jzwendu = NetCom3.FloatToHex(float.Parse(txtStandard.Text));
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 04 03 " + jzwendu), 5);
                            break;
                        default:
                            break;
                    }
                    break;
                case "清洗盘":
                    switch (cmbStep.Text)
                    {
                        case "加热打开":
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 05 00"), 5);
                            break;
                        case "加热关闭":
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 05 01"), 5);
                            break;
                        case "设置校准值":
                            if (txtStandard.Text == "")
                            {
                                frmMsgShow.MessageShow("仪器调试", "请录入要校准的温度值！");
                                return;
                            }
                            //string jzwendu = (float.Parse(txtStandard.Text)).ToString("x4");
                            string jzwendu = NetCom3.FloatToHex(float.Parse(txtStandard.Text));
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 05 03 " + jzwendu), 5);
                            break;
                        default:
                            break;
                    }
                    break;
                case "清洗管路":
                    switch (cmbStep.Text)
                    {
                        case "加热打开":
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 06 00"), 5);
                            break;
                        case "加热关闭":
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 06 01"), 5);
                            break;
                        case "设置校准值":
                            if (txtStandard.Text == "")
                            {
                                frmMsgShow.MessageShow("仪器调试", "请录入要校准的温度值！");
                                return;
                            }
                            //string jzwendu = (float.Parse(txtStandard.Text)).ToString("x4");
                            string jzwendu = NetCom3.FloatToHex(float.Parse(txtStandard.Text));
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 06 03 " + jzwendu), 5);
                            break;
                        default:
                            break;
                    }
                    break;
                case "底物管路":
                    switch (cmbStep.Text)
                    {
                        case "加热打开":
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 07 00"), 5);
                            break;
                        case "加热关闭":
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 07 01"), 5);
                            break;
                        case "设置校准值":
                            if (txtStandard.Text == "")
                            {
                                frmMsgShow.MessageShow("仪器调试", "请录入要校准的温度值！");
                                return;
                            }
                            //string jzwendu = (float.Parse(txtStandard.Text)).ToString("x4");
                            string jzwendu = NetCom3.FloatToHex(float.Parse(txtStandard.Text));
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 07 03 " + jzwendu), 5);
                            break;
                        default:
                            break;
                    }
                    break;
                case "试剂盘":
                    if (cmbStep.Text.ToString() == "设置校准值")
                    {
                        if (txtStandard.Text == "")
                        {
                            frmMsgShow.MessageShow("仪器调试", "请录入要校准的温度值！");
                            break;
                        }
                        string sjwendu = NetCom3.FloatToHex(float.Parse(txtStandard.Text));
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 08 03 " + sjwendu), 5);
                        if (!reagentColdQuery())
                        {
                            frmMsgShow.MessageShow("ERROR", "请检查是否打开制冷开关");
                        }
                    }
                    break;
                default:
                    break;
            }
            NetCom3.Instance.SingleQuery();
            btnmakeStandard.Enabled = true;
        }

        private void functionButton1_Click(object sender, EventArgs e)
        {

            //2018-07-02 zlx add
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            btnQuatoState.Enabled = false;
            if (rbtnRunLightOpen.Checked)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 08 00"), 5);
            }
            else if (rbtnRunLightClose.Checked)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 08 01"), 5);
            }
            else if (rbtnWainOpen.Checked)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 09 00"), 5);
            }
            else if (rbtnWainOpen.Checked)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 09 01"), 5);
            }
            NetCom3.Instance.SingleQuery();
            btnQuatoState.Enabled = true;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            txtStandard.Text = "";
            //2018-07-03 zlx add
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbModelName.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择要调试的温控模块！");
                return;
            }
            if (cmbStep.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择要查询的温控模块！");
                return;
            }
            else if (cmbStep.SelectedItem.ToString() != "查询校准值" && cmbStep.SelectedItem.ToString() != "查询温度")
            {
                LogFile.Instance.Write(cmbStep.SelectedItem.ToString());
                frmMsgShow.MessageShow("仪器调试", "选择查询的温控模块有误，请重新选择！");
                return;
            }
            btnSelect.Enabled = false;
            switch (cmbModelName.SelectedItem.ToString())
            {
                case "温育盘":
                    if (cmbStep.SelectedItem.ToString() == "查询校准值")
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 04 06"), 5);
                    else if (cmbStep.SelectedItem.ToString() == "查询温度")
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 04 04"), 5);
                    break;
                case "清洗盘":
                    if (cmbStep.SelectedItem.ToString() == "查询校准值")
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 05 06"), 5);
                    else if (cmbStep.SelectedItem.ToString() == "查询温度")
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 05 04"), 5);
                    break;
                case "清洗管路":
                    if (cmbStep.SelectedItem.ToString() == "查询校准值")
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 06 06"), 5);
                    else if (cmbStep.SelectedItem.ToString() == "查询温度")
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 06 04"), 5);
                    break;
                case "底物管路":
                    if (cmbStep.SelectedItem.ToString() == "查询校准值")
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 07 06"), 5);
                    else if (cmbStep.SelectedItem.ToString() == "查询温度")
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 07 04"), 5);
                    break;
                case "试剂盘":
                    if (cmbStep.SelectedItem.ToString() == "查询校准值")
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 08 06"), 5);
                    else if (cmbStep.SelectedItem.ToString() == "查询温度")
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 08 04"), 5);
                    if (!reagentColdQuery())
                    {
                        frmMsgShow.MessageShow("ERROR", "请检查是否打开制冷开关.");
                    }
                    break;
            }
            NetCom3.Instance.SingleQuery();
            btnSelect.Enabled = true;
            //ReadThread.Abort();
        }

        private void chkQXGL_CheckedChanged(object sender, EventArgs e)
        {
            chart1.Series[5].Enabled = chkQXGL.Checked;
        }

        /// <summary>
        /// 增加清洗盘转动按钮的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void functionButton1_Click_1(object sender, EventArgs e)
        {
            string holetargethex = "";
            functionButton1.Enabled = false;
            if (!(NetCom3.totalOrderFlag || NetCom3.WashReciveFlag))
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (textBox1.Text.Trim() == "")
            {
                frmMsgShow.MessageShow("仪器调试", "请输入转动个数！");
                textBox1.Focus();
                return;
            }
            try
            {
                int holetarget = int.Parse(textBox1.Text.Trim());
                if (holetarget > 0)
                {
                    holetargethex = holetarget.ToString("x2");
                }
                else if (holetarget < 0)
                {
                    holetargethex = holetarget.ToString("X2").Substring(6, 2);
                }
                if (holetarget == 0 || holetarget == 30 || holetarget == -30)
                {
                    frmMsgShow.MessageShow("仪器调试", "当前孔位没有变化！");
                    textBox1.Focus();
                    functionButton1.Enabled = true;
                }
                else if (holetarget > -30 && holetarget < 30)
                {
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01" + " " + holetargethex.Substring(0, 2)), 2);
                    //NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + holetargethex.Substring(0, 2)), 2);
                    NetCom3.Instance.WashQuery();
                    //frmMsgShow.MessageShow("仪器调试", "清洗盘正在转动，请稍候！");    
                    functionButton1.Enabled = true;
                }
                else if (holetarget > 30 || holetarget < -30)
                {
                    frmMsgShow.MessageShow("仪器调试", "孔位支持移动的范围为-30~30！");
                    textBox1.Focus();
                }
                functionButton1.Enabled = true;
            }
            catch (Exception exp)
            {
                MessageBox.Show("请输入数字！");
            }
            finally
            {
                functionButton1.Enabled = true;
            }

        }

        private void btnHandvertical_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbHandvertical.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择旋转动作");
                cmbHandIntegrate.Focus();
                return;
            }
            btnHandvertical.Enabled = false;
            if (cmbHandvertical.SelectedIndex == 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 02 30 00 00"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbHandvertical.SelectedIndex == 1)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 02 31 00 00"), 5);
                NetCom3.Instance.SingleQuery();
            }
            btnHandvertical.Enabled = true;
        }

        private void btnCupMake_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbCupMake.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择理杯块运行动作");
                cmbHandIntegrate.Focus();
                return;
            }
            btnCupMake.Enabled = false;
            if (cmbCupMake.SelectedIndex == 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 04 30 00 00"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbCupMake.SelectedIndex == 1)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 04 31 00 00"), 5);
                NetCom3.Instance.SingleQuery();
            }
            btnCupMake.Enabled = true;

        }

        private void CmbIpara_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            CmbIpara.Enabled = false;
            //温育盘位置
            if (CmbIpara.SelectedIndex == 2)
            {
                sendMonitor(NetCom3.Cover("EB 90 04 01 03"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //混匀臂位置
            //else if (cmbHandPara.SelectedIndex == 1)
            //{
            //    NetCom3.Instance.Send(NetCom3.Cover("EB 90 04 01 02"), 5);
            //    NetCom3.Instance.SingleQuery();
            //}
            //压杯结束位置
            else if (CmbIpara.SelectedIndex == 1)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 04 01 02"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //压杯开始位置
            else if (CmbIpara.SelectedIndex == 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 04 01 01"), 5);
                NetCom3.Instance.SingleQuery();
            }
            CmbIpara.Enabled = true;

            UpdateIncubationSteps();
        }

        private void UpdateIncubationSteps()
        {
            if (CmbIpara.SelectedIndex < 0 || cmbIElecMachine.SelectedIndex < 0) return;
            textBox2.Text =
                GetCurrentSteps((CmbIpara.SelectedIndex + 1).ToString().PadLeft(2, '0'),
                (cmbIElecMachine.SelectedIndex + 1).ToString().PadLeft(2, '0'), "04");
        }

        private void btnIAdd_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbIElecMachine.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择需调试的电机！");
                return;
            }
            if (txtIIncrem.Text == "")
            {
                frmMsgShow.MessageShow("仪器调试", "请输入增量值！");
                txtIIncrem.Focus();
                return;
            }
            btnIAdd.Enabled = false;
            string incream = int.Parse(txtIIncrem.Text.Trim()).ToString("x8");
            //温育盘电机
            if (cmbIElecMachine.SelectedIndex == 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 04 02 01 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            //垂直电机
            else if (cmbIElecMachine.SelectedIndex == 1)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 04 02 02 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                     + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            //压杯电机
            else if (cmbIElecMachine.SelectedIndex == 2)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 04 02 03 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            btnIAdd.Enabled = true;
        }

        private void btnISub_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbIElecMachine.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择需调试的电机！");
                return;
            }
            if (txtIIncrem.Text == "")
            {
                frmMsgShow.MessageShow("仪器调试", "请输入增量值！");
                txtIIncrem.Focus();
                return;
            }
            btnISub.Enabled = false;
            string incream = int.Parse("-" + txtIIncrem.Text.Trim()).ToString("x8");
            //温育盘电机
            if (cmbIElecMachine.SelectedIndex == 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 04 02 01 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            //垂直电机
            else if (cmbIElecMachine.SelectedIndex == 1)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 04 02 02 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                     + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            //压杯电机
            else if (cmbIElecMachine.SelectedIndex == 2)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 04 02 03 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            btnISub.Enabled = true;
        }

        private void btnISave_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            btnISave.Enabled = false;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 04 13"), 5);
            NetCom3.Instance.SingleQuery();
            NetCom3.Delay(500);//lyq add 20201016 指令返回太快，增加一个点击停顿
            btnISave.Enabled = true;

            UpdateIncubationSteps();
        }

        private void btnIAllInit_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            btnIAllInit.Enabled = false;
            sendMonitor(NetCom3.Cover("EB 90 04 03 00"), 5);
            NetCom3.Instance.SingleQuery();
            btnIAllInit.Enabled = true;

            UpdateIncubationSteps();
        }

        private void fbtnInTubeReset_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            fbtnInTubeReset.Enabled = false;
            sendMonitor(NetCom3.Cover("EB 90 04 03 01"), 5);
            NetCom3.Instance.SingleQuery();
            fbtnInTubeReset.Enabled = true;
        }

        private void btnIYInit_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            btnIYInit.Enabled = false;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 04 03 02"), 5);
            NetCom3.Instance.SingleQuery();
            btnIYInit.Enabled = true;
        }

        private void cmbStep_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbStep.SelectedItem.ToString() == "查询温度")
                label10.Text = " 温度：";
            else
                label10.Text = "校准值：";
        }

        private void btnZx_Click(object sender, EventArgs e)
        {
            int pos = int.Parse(numXz.Text);
            if (pos > 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (pos).ToString("X2")), 2);
            }
            else
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (0 + pos).ToString("X2").Substring(6, 2)), 2);
            }
        }

        private void btnReadNum_Click(object sender, EventArgs e)
        {
            textReadShow.Clear();
            btnReadNum.Enabled = false;
            int Num = int.Parse(numRepeat.Text);
            NetCom3.Instance.ReceiveHandel += GetReadNum2;
            for (int i = 1; i <= Num; i++)
            {
                //BackObj = "";
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 00 01"), 2);
                if (!NetCom3.Instance.WashQuery())
                    return;
                //else
                //{
                //    int delay = 1000;
                //    while (!BackObj.Contains("EB 90 31 A3") && delay > 0)
                //    {
                //        NetCom3.Delay(10);
                //        delay = delay - 10;
                //    }
                //    if (BackObj.Contains("EB 90 31 A3"))
                //    {
                //        //string temp = BackObj.Replace(" ", "");
                //        //int pos = temp.IndexOf("EB9031A3");
                //        //temp = temp.Substring(pos, 32);
                //        //temp = temp.Substring(temp.Length - 8);
                //        string temp = BackObj.Substring(BackObj.Length - 16).Replace(" ", "");
                //        temp = Convert.ToInt64(temp, 16).ToString();
                //        if (int.Parse(temp) > Math.Pow(10, 5))
                //            temp = ((int)GetPMT(double.Parse(temp))).ToString();
                //        textReadShow.AppendText(DateTime.Now.ToString("HH-mm-ss") + ": " + "PMT背景值：" + temp + Environment.NewLine);
                //    }
                //}
            }
            Thread.Sleep(500);
            NetCom3.Instance.ReceiveHandel -= GetReadNum2;
            TExtAppend("已完成" + Environment.NewLine);
            //textReadShow.AppendText("已完成" + Environment.NewLine);
            btnReadNum.Enabled = true;
        }

        //2019.5.30 hly add
        private void fbtnPeristalticPEx_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            fbtnPeristalticPEx.Enabled = false;
            if (cmbPeristalticPVol.SelectedItem.ToString() == "归零")
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 03 14"), 5);
                //NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 0B 00 01"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbPeristalticPVol.SelectedItem.ToString() == "350")
            {
                string temp = 350.ToString("x4");
                string temp1 = temp.Substring(0, 2);
                string temp2 = temp.Substring(2, 2);
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 03 14 " + temp1 + " " + temp2 + " 00"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //if (cmbPeristalticPVol.SelectedItem.ToString() == "1")
            //{
            //    NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 0B 00 01"), 5);
            //    NetCom3.Instance.SingleQuery();
            //}
            //else if (cmbPeristalticPVol.SelectedItem.ToString() == "2")
            //{
            //    NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 0B 00 02"), 5);
            //    NetCom3.Instance.SingleQuery();
            //}
            //else if (cmbPeristalticPVol.SelectedItem.ToString() == "3")
            //{
            //    NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 0B 00 03"), 5);
            //    NetCom3.Instance.SingleQuery();
            //}
            fbtnPeristalticPEx.Enabled = true;
        }

        private void btnPutCupInit_Click(object sender, EventArgs e)
        {
            //2019-06-06 zlx add
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            btnPutCupInit.Enabled = false;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 03 01"), 5);
            NetCom3.Instance.SingleQuery();
            btnPutCupInit.Enabled = true;
        }

        private void BtnOrderSend_Click(object sender, EventArgs e)
        {
            if (textStepOrder.Text.Trim() == "")
            {
                frmMsgShow.MessageShow("提示", "请输入通讯命令！");
                return;
            }

            string order = textStepOrder.Text.ToString().Trim().Replace("eb", "EB").Replace("eB", "EB").Replace("Eb", "EB");

            if (order.Substring(0, 5) != "EB 90")
            {
                frmMsgShow.MessageShow("提示", "未识别的指令！");
                return;
            }
            if (!Regex.IsMatch(order.Replace(" ", ""), @"^[0-9a-fA-F]{1,}$"))
            {
                frmMsgShow.MessageShow("提示", "未识别的指令！");
                return;
            }
            foreach (string temp in order.Split(' '))
            {
                if (temp.Length != 2)
                {
                    frmMsgShow.MessageShow("提示", "未识别的指令！");
                    return;
                }
            }

            btnOrderSend.Enabled = false;
            btnOrderCancel.Enabled = true;

            if (order.Contains("EB 90 31 01"))
            {
                stepOrderSendFlag = 1;
                NetCom3.Instance.Send(NetCom3.Cover(order), 1);
                NetCom3.Instance.MoveQuery();
            }
            else if (order.Contains("EB 90 31 02") || order.Contains("EB 90 31 04"))
            {
                stepOrderSendFlag = 0;
                NetCom3.Instance.Send(NetCom3.Cover(order), 0);
                NetCom3.Instance.SPQuery();
            }
            else if (order.Contains("EB 90 31 03"))
            {
                stepOrderSendFlag = 2;
                NetCom3.Instance.Send(NetCom3.Cover(order), 2);
                NetCom3.Instance.WashQuery();
            }
            else
            {
                stepOrderSendFlag = 5;
                NetCom3.Instance.Send(NetCom3.Cover(order), 5);
                NetCom3.Instance.SingleQuery();
            }

            btnOrderSend.Enabled = true;
            btnOrderCancel.Enabled = false;
            LogFile.Instance.Write(DateTime.Now.ToString("HH-mm-ss"));
        }

        private void btnWashMix_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbWashMix.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择压杯电机动作！");
            }
            btnWashMix.Enabled = false;
            if (cmbWashMix.SelectedIndex == 0)
            {
                //NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 01 01"), 5);
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 03 33 01"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbWashMix.SelectedIndex == 1)
            {
                //NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 01 02"), 5);
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 03 33 02"), 5);
                NetCom3.Instance.SingleQuery();
            }
            btnWashMix.Enabled = true;
        }

        private void fbtnStchDiskTurn_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            fbtnStchDiskTurn.Enabled = false;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 05 01"), 5);
            NetCom3.Instance.SingleQuery();
            fbtnStchDiskTurn.Enabled = true;
        }

        private void BtnVersion_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            TxtVersionClear();

            btnVersion.Enabled = false;

            //userTextBoxBase[] a = new userTextBoxBase[8];

            NetCom3.Instance.ReceiveHandel += GetVersionNum;
            for (int i = 1; i <= 8; i++)
            {
                string boardId = i.ToString("x2");

                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 FF " + boardId), 5);
                VersionQuery();
                //NetCom3.Instance.SingleQuery();                

            }
            NetCom3.Instance.ReceiveHandel -= GetVersionNum;

            for (int i = 1; i <= 8; i++)
            {
                if (i == 1)
                {
                    txtPhotonVersion.Text = boardVersion[i];
                }
                else if (i == 2)
                {
                    txtCommunicationVersion.Text = boardVersion[i];
                }
                else if (i == 3)
                {
                    txtGripperVersion.Text = boardVersion[i];
                }
                else if (i == 4)
                {
                    txtSimpleVersion.Text = boardVersion[i];
                }
                else if (i == 5)
                {
                    txtWashVersion.Text = boardVersion[i];
                }
                else if (i == 6)
                {
                    txtAlertVersion.Text = boardVersion[i];
                }
                else if (i == 7)
                {
                    txtRefrigerationVersion.Text = boardVersion[i];
                }
                else if (i == 8)
                {
                    txtTrayVersion.Text = boardVersion[i];
                }
            }

            btnVersion.Enabled = true;
        }

        private void GetVersionNum(string order)
        {
            if (order.Contains("EB 90 11 FF"))
            {
                string bVersion;//版本号字符串
                int MM; //大版本号
                int NN; //小版本号
                string version = order.Replace(" ", "");
                int pos = version.IndexOf("EB9011FF");
                version = version.Substring(pos, 32);
                version = version.Substring(8);
                string tag = version.Substring(0, 2); //0X 选择板子                
                MM = Convert.ToInt32(version.Substring(2, 2), 16);
                NN = Convert.ToInt32(version.Substring(4, 2), 16);

                bVersion = "V" + tag.Substring(1, 1) + "." + MM + "." + NN;

                boardVersion[int.Parse(tag)] = bVersion;
            }
        }
        private void TxtVersionClear()
        {
            Invoke(new Action(() =>
            {
                txtPhotonVersion.Clear();
                txtCommunicationVersion.Clear();
                txtGripperVersion.Clear();
                txtSimpleVersion.Clear();

                txtWashVersion.Clear();
                txtAlertVersion.Clear();
                txtRefrigerationVersion.Clear();
                txtTrayVersion.Clear();
            }));
        }
        /// <summary>
        /// 版本号超时默认查询
        /// </summary>
        /// <returns></returns>
        public bool VersionQuery()
        {
            DateTime vsDt = DateTime.Now;
            while (!NetCom3.totalOrderFlag)
            {
                if (DateTime.Now.Subtract(vsDt).TotalMilliseconds > 2000)
                {
                    NetCom3.Instance.errorFlag = (int)ErrorState.OverTime;
                    NetCom3.totalOrderFlag = true;
                    NetCom3.DiagnostDone.Set();
                }
                NetCom3.Delay(10);
            }
            if (NetCom3.Instance.errorFlag != (int)ErrorState.Success)
            {
                LogFile.Instance.Write("MoverrorFlag = ： " + NetCom3.Instance.MoverrorFlag + "  *****当前 " + DateTime.Now.ToString("HH - mm - ss"));
                return false;
            }
            else
            {
                return true;
            }
        }

        private void TxtTestErrorShow_TextChanged(object sender, EventArgs e)
        {
            txtTestErrorShow.ScrollToCaret();
        }

        private void TxtAgingInfoShow_TextChanged(object sender, EventArgs e)
        {
            int index = txtAgingInfoShow.Lines.Count();
            showLog = txtAgingInfoShow.Lines.ElementAtOrDefault(index - 1);
            if (showLog == "")
                showLog = txtAgingInfoShow.Lines.ElementAtOrDefault(index - 2);
            txtAgingInfoShow.ScrollToCaret();
        }
        private void dealAgingBack(string order)
        {
            if (showLog == "" || showLog == null)
                return;
            if (order.Contains("EB 90 31 A1"))
            {
                string log = showLog;
                int tempInt = order.IndexOf("EB 90 31 A1 ");
                string temp = order.Substring(tempInt + 12, 2);
                Byte bit = Convert.ToByte(temp, 16);
                int flag = -1;
                if (bit != Byte.MaxValue)
                {
                    temp = Convert.ToString(bit, 2);
                    while (temp.Length < 8)
                    {
                        temp = "0" + temp;
                    }
                    if (temp.Substring(0, 1) == "0")//为空
                    {
                        flag = 5;
                    }
                    if (temp.Substring(1, 1) == "0")//取管撞管
                    {
                        flag = 4;
                    }
                    if (temp.Substring(2, 1) == "0")//放管撞管
                    {
                        flag = 7;
                    }
                    if (temp.Substring(3, 1) == "0")//盘通讯异常
                    {
                        flag = 9;
                    }
                    if (temp.Substring(4, 1) == "0")//理杯机缺管
                    {
                        flag = 8;
                    }
                    if (flag != -1)
                        BeginInvoke(new Action(() =>
                        {
                            txtTestErrorShow.AppendText(DateTime.Now.ToString("HH:mm:ss->") + log + "——" + State[flag] + Environment.NewLine);

                        }));
                }
            }
            else if (order.Contains("EB 90 31 A2"))
            {
                string log = showLog;
                int tempInt = order.IndexOf("EB 90 31 A1 ");
                string temp = order.Substring(tempInt + 12, 2);
                if (temp == "7F")//加液针撞针
                {
                    BeginInvoke(new Action(() =>
                    {
                        txtTestErrorShow.AppendText(DateTime.Now.ToString("HH:mm:ss->") + log + "——" + State[4] + Environment.NewLine);

                    }));
                }
            }

        }

        private void CmbTestName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTestName.SelectedItem.ToString() == "加样准确性测试")
            {
                untxtInjection.Visible = label98.Visible = false;//清洗注液测试

                label87.Visible = untxtReadNum.Visible = false;
                label82.Visible = untxtSamplePos2.Visible = false;
                label73.Visible = txtbox2.Visible = false;

                label84.Visible = label85.Visible = untxtSamplePos.Visible = untxtSampleVol.Visible = true;

                label88.Visible = label89.Visible = untxtPos2.Visible = untxtPos1.Visible = true; //温育盘放管孔位
            }
            else if (cmbTestName.SelectedItem.ToString() == "酶活性测试")
            {
                untxtInjection.Visible = label98.Visible = false;//清洗注液测试

                label84.Visible = label85.Visible = untxtSamplePos.Visible = untxtSampleVol.Visible = false;
                label82.Visible = untxtSamplePos2.Visible = false;
                label73.Visible = txtbox2.Visible = false;

                label87.Visible = untxtReadNum.Visible = true;

                label88.Visible = label89.Visible = untxtPos2.Visible = untxtPos1.Visible = true; //温育盘放管孔位
            }
            else if (cmbTestName.SelectedItem.ToString() == "清洗效果测试")
            {
                untxtInjection.Visible = label98.Visible = false;//清洗注液测试

                label84.Visible = untxtSampleVol.Visible = true;
                label82.Visible = untxtSamplePos2.Visible = false;
                label73.Visible = txtbox2.Visible = true;

                label87.Visible = untxtReadNum.Visible = true;
                label85.Visible = untxtSamplePos.Visible = true;

                label88.Visible = label89.Visible = untxtPos2.Visible = untxtPos1.Visible = false; //温育盘放管孔位
            }
            else if (cmbTestName.SelectedItem.ToString() == "酶活测试-加样底物读数")
            {
                untxtInjection.Visible = label98.Visible = false;//清洗注液测试

                label87.Visible = untxtReadNum.Visible = true;
                label73.Visible = txtbox2.Visible = true;

                label88.Visible = label89.Visible = untxtPos2.Visible = untxtPos1.Visible = false; //温育盘放管孔位

                label82.Visible = untxtSamplePos2.Visible = true;
                label84.Visible = label85.Visible = untxtSamplePos.Visible = untxtSampleVol.Visible = true;
            }
            else if (cmbTestName.SelectedItem.ToString() == "清洗注液测试")
            {
                untxtInjection.Visible = label98.Visible = true; //清洗注液测试

                label87.Visible = untxtReadNum.Visible = false;
                label73.Visible = txtbox2.Visible = false;

                label88.Visible = label89.Visible = untxtPos2.Visible = untxtPos1.Visible = false; //温育盘放管孔位

                label82.Visible = untxtSamplePos2.Visible = false;
                label84.Visible = label85.Visible = untxtSamplePos.Visible = untxtSampleVol.Visible = false;
            }
        }

        private void CbChange_CheckedChanged(object sender, EventArgs e)
        {
            if (cbChange.Checked)
            {
                untxtPos1.Enabled = true;
                untxtPos2.Enabled = true;
                untxtSampleVol.Enabled = true;
                untxtSamplePos.Enabled = true;
                untxtReadNum.Enabled = true;
                untxtPos2.MaxValue = frmParent.ReactTrayNum;
            }
            else
            {
                untxtPos1.Enabled = false;
                untxtPos2.Enabled = false;
                untxtSampleVol.Enabled = false;
                untxtSamplePos.Enabled = false;
                untxtReadNum.Enabled = false;
            }
        }

        private void FbtnTestsStop_Click(object sender, EventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                textBox3.AppendText(Environment.NewLine + DateTime.Now.ToString("HH-mm-ss") + ": 测试结束。" + Environment.NewLine + Environment.NewLine);

                threadList.Find(xy => xy.Name == "specificationTest").Abort();
                threadList.RemoveAll(xy => xy.Name == "specificationTest");
                fbtnTestS.Enabled = true;
                cmbTestName.Enabled = true;
                fbtnTestsStop.Enabled = false;
            }));
        }

        private void FbtnTestS_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbTestName.SelectedIndex < 0)
            {
                frmMsgShow.MessageShow("仪器调试", "没有选择具体测试项目，请重试！");
                return;
            }
            if (untxtSamplePos2.Text == "" || untxtSamplePos.Text == "" || untxtSampleVol.Text == "" || untxtPos1.Text == "" || untxtPos2.Text == "" || untxtReadNum.Text == "" || untxtInjection.Text == "")
            {
                frmMsgShow.MessageShow("仪器调试", "存在输入框为空，请重新输入！");
                return;
            }
            //if (int.Parse(untxtSamplePos.Text) > int.Parse(untxtSamplePos2.Text) && cmbTestName.SelectedIndex != 2)
            //{
            //    frmMsgShow.MessageShow("仪器调试", "样本位输入不正确，请重新输入！");
            //    untxtSamplePos.Text = untxtSamplePos2.Text = "1";
            //    untxtSamplePos.Focus();
            //    return;
            //}
            if (int.Parse(untxtPos1.Text) > int.Parse(untxtPos2.Text))
            {
                frmMsgShow.MessageShow("仪器调试", "温育盘放管孔位输入不正确，请重新输入！");
                untxtPos1.Text = untxtPos2.Text = "1";
                untxtPos1.Focus();
                return;
            }
            Thread run = new Thread(specificationTestRun);
            run.Name = "specificationTest";
            run.IsBackground = true;
            run.Start();
            threadList.Add(run);
        }

        private void specificationTestRun()
        {
            //温育盘开始放管孔位
            int reactTrayStartPos = int.Parse(untxtPos1.Text);
            //温育盘最后放管孔位
            int reactTrayEndPos = int.Parse(untxtPos2.Text);
            //测试次数
            int repeat = int.Parse(untxtPos2.Text) - int.Parse(untxtPos1.Text) + 1;
            //读数次数
            int readNum = int.Parse(untxtReadNum.Text);
            //加液量
            int sampleVol = int.Parse(untxtSampleVol.Text);
            //样本位号
            string samplePos = untxtSamplePos.Text;
            //选择的测试
            int testIndex = 0;
            //选择项目名称
            string testName = "";
            //清洗注液测试，测试组数
            int InjectionNum = int.Parse(untxtInjection.Text);

            Invoke(new Action(() =>
            {
                testName = cmbTestName.SelectedItem.ToString();
                testIndex = cmbTestName.SelectedIndex;
                textBox3.Clear();
                fbtnTestS.Enabled = false;
                cmbTestName.Enabled = false;
                fbtnTestsStop.Enabled = true;
                cbChange.Checked = false;
            }));

            //重置导出信息表
            if (dtexp.Rows.Count > 0)
            {
                dtexp.Clear();
                dtexp.Columns.RemoveAt(0);
                dtexp.Columns.RemoveAt(0);
            }
            dtexp.Columns.Add(testName, typeof(string));
            dtexp.Columns.Add("PMT", typeof(string));

            //加样准确性测试
            if (testIndex == 0)
            {
                DialogResult r = MessageBox.Show("请确认在温育盘 " + reactTrayStartPos + " 号到 " + reactTrayEndPos + " 号孔位是否放好反应管，点击确定马上开始测试。", "仪器调试", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (r == DialogResult.Cancel)
                {
                    goto enzymaticActivityTestEnd;
                }
                for (int i = 0; i < repeat; i++)
                {
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 01 " + int.Parse(samplePos).ToString("x2") + " " + reactTrayStartPos.ToString("x2")
                                 + " " + sampleVol.ToString("x2")), 0);
                    if (!NetCom3.Instance.SPQuery())
                    {
                        goto enzymaticActivityTestEnd;
                    }
                    reactTrayStartPos++;
                    NetCom3.Delay(1000);
                }
                NetCom3.Delay(1000);
            }
            //酶活性测试
            else if (testIndex == 1)
            {
                if (repeat > 10)
                {
                    frmMsgShow.MessageShow("仪器调试", "目前最多只允许10个管进行测试，请重新输入！");
                    goto enzymaticActivityTestEnd;
                }

                DialogResult r = MessageBox.Show("请确认在温育盘 " + reactTrayStartPos + " 号到 " + reactTrayEndPos + " 号孔位是否放好反应管，是否加好试剂，点击确定马上开始测试。", "仪器调试", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (r == DialogResult.Cancel)
                {
                    goto enzymaticActivityTestEnd;
                }

                BeginInvoke(new Action(() =>
                {
                    textBox3.AppendText(DateTime.Now.ToString("HH-mm-ss") + ": 测试开始。" + Environment.NewLine);
                }));

                for (int i = 0; i < repeat; i++)
                {
                    //抓一个管从温育盘到清洗盘
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 02 " + reactTrayStartPos.ToString("x2")), 1);
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        goto enzymaticActivityTestEnd;
                    }
                    reactTrayStartPos++;

                    //清洗盘顺时针旋转一位
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-1).ToString("X2").Substring(6, 2)), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        goto enzymaticActivityTestEnd;
                    }
                    NetCom3.Delay(500);
                }

                //循环加底物读数
                int turnToSub = 20 - (31 - repeat); //清洗盘20加底物
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + turnToSub.ToString("x2").Substring(6, 2)), 2); //顺时针转到20
                if (!NetCom3.Instance.WashQuery())
                {
                    goto enzymaticActivityTestEnd;
                }
                int num = 1; //第num个管在读数
                for (int i = 0; i < repeat; i++)
                {
                    NetCom3.Delay(1000);
                    //加底物
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 01 10"), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        goto enzymaticActivityTestEnd;
                    }

                    //逆时针旋转5位
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 05"), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        goto enzymaticActivityTestEnd;
                    }

                    //读数发光 ,循环读数次数次                    
                    BeginInvoke(new Action(() =>
                    {
                        textBox3.AppendText(Environment.NewLine + Environment.NewLine + DateTime.Now.ToString("HH-mm-ss") + ": 第 " + num++ + " 个管正在读数。" + Environment.NewLine);
                    }));
                    for (int j = 0; j < readNum; j++)
                    {
                        BackObj = "";
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 00 11"), 2);
                        if (!NetCom3.Instance.WashQuery())
                        {
                            goto enzymaticActivityTestEnd;
                        }
                        else
                        {
                            int delay = 1000;
                            while (!BackObj.Contains("EB 90 31 A3") && delay > 0)
                            {
                                NetCom3.Delay(10);
                                delay = delay - 10;
                            }
                            if (BackObj.Contains("EB 90 31 A3"))
                            {
                                string temp = BackObj.Substring(BackObj.Length - 16).Replace(" ", "");
                                temp = Convert.ToInt64(temp, 16).ToString();

                                if (int.Parse(temp) > Math.Pow(10, 5))
                                    temp = ((int)GetPMT(double.Parse(temp))).ToString();
                                BeginInvoke(new Action(() =>
                                {
                                    textBox3.AppendText(DateTime.Now.ToString("HH-mm-ss") + ": " + "PMT背景值:" + temp + Environment.NewLine);
                                }));

                            }
                        }
                        NetCom3.Delay(500);
                    }

                    if (i != repeat - 1)
                    {
                        //顺时针旋转6位
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-6).ToString("X2").Substring(6, 2)), 2);
                        if (!NetCom3.Instance.WashQuery())
                        {
                            goto enzymaticActivityTestEnd;
                        }
                    }
                    else
                    {
                        //逆时针转到30位
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 05"), 2);
                        if (!NetCom3.Instance.WashQuery())
                        {
                            goto enzymaticActivityTestEnd;
                        }
                    }
                }
                //循环扔管
                for (int i = 0; i < repeat; i++)
                {
                    //逆时针转1位
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        goto enzymaticActivityTestEnd;
                    }
                    //在取放管处扔管
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        goto enzymaticActivityTestEnd;
                    }
                }
                NetCom3.Delay(1000);
            }
            //清洗效果测试
            else if (testIndex == 2)
            {
                repeat = int.Parse(txtbox2.Text);//每个样本杯加样次数
                reactTrayStartPos = 1;

                if (repeat < 1)
                {
                    MessageBox.Show("请输入大于0的测试次数");
                    goto enzymaticActivityTestEnd;
                }
                DialogResult r = MessageBox.Show("请确认准备完毕，点击确定马上开始测试。", "仪器调试", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (r == DialogResult.Cancel)
                {
                    goto enzymaticActivityTestEnd;
                }
                BeginInvoke(new Action(() =>
                {
                    textBox3.AppendText(DateTime.Now.ToString("HH-mm-ss") + ": 测试开始。" + Environment.NewLine);
                }));

                int num = 0; //第num个管在读数
                int[] washPos = new int[31]; //清洗盘放管位置计数 0为执行结果标志位
                int cycleTimes = repeat % 5 == 0 ? (repeat / 5) : (repeat / 5 + 1); //清洗盘循环一次5个管 ，清洗盘一共循环几次
                int oneTimes; //第一次循环几个管

                #region 夹新管
                //检测1-repeat孔位是否有管，清掉
                DataTable dtInTrayIni = OperateIniFile.ReadConfig(iniPathReactTrayInfo);
                for (int i = 0; i < repeat; i++)
                {
                    if (int.Parse(dtInTrayIni.Rows[i][1].ToString()) >= 1)//有管
                    {
                        //有管扔掉
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 " + (i + 1).ToString("X2")), 1);
                        if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                        {
                            MessageBox.Show("异常，请重试");
                            goto enzymaticActivityTestEnd;
                        }
                        //修改反应盘信息
                        OperateIniFile.WriteIniData("ReactTrayInfo", "no" + (i + 1), "0", iniPathReactTrayInfo);
                    }
                }
                //夹新管
                for (int i = 0; i < repeat; i++)
                {
                    int second = 0;
                    againInjection:
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 01 " + (i + 1).ToString("X2")), 1);
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull || NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked)
                        {
                            second++;
                            if (second < 2)
                                goto againInjection;
                            else
                            {
                                MessageBox.Show("异常，请重试");
                                goto enzymaticActivityTestEnd;
                            }
                        }
                        else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.putKnocked)
                        {
                            second++;
                            if (second < 2)
                            {
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 " + (i + 1).ToString("X2")), 1);
                                if (!NetCom3.Instance.MoveQuery())
                                {
                                    MessageBox.Show("异常，请重试");
                                    goto enzymaticActivityTestEnd;
                                }
                                goto againInjection;
                            }
                            else
                            {
                                MessageBox.Show("异常，请重试");
                                goto enzymaticActivityTestEnd;
                            }
                        }
                        else
                        {
                            MessageBox.Show("异常，请重试");
                            goto enzymaticActivityTestEnd;
                        }
                    }
                    //修改反应盘信息
                    OperateIniFile.WriteIniData("ReactTrayInfo", "no" + (i + 1), "1", iniPathReactTrayInfo);
                }
                #endregion
                //加样
                for (int i = 0; i < repeat; i++)
                {
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 01 " + int.Parse(samplePos).ToString("X2") + " " + (i + 1).ToString("X2") + " " + sampleVol.ToString("X2")), 0);
                    if (!NetCom3.Instance.SPQuery())
                    {
                        MessageBox.Show("异常，请重试");
                        goto enzymaticActivityTestEnd;
                    }
                }
                for (int i = 0; i < cycleTimes; i++)
                {
                    oneTimes = repeat / 5 == 0 ? repeat : 5; //第一次循环几个管

                    int j = 0;
                    //越过第一根抽液针 的开始动作
                    for (j = 0; j < oneTimes; j++)
                    {
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 02 " + reactTrayStartPos.ToString("x2")), 1);  //夹管
                        if (!NetCom3.Instance.MoveQuery())
                        {
                            goto enzymaticActivityTestEnd;
                        }
                        //修改反应盘信息
                        OperateIniFile.WriteIniData("ReactTrayInfo", "no" + reactTrayStartPos, "0", iniPathReactTrayInfo);

                        reactTrayStartPos++;

                        //清洗盘逆时针旋转6位 跳过第一个抽液针，到第一个注液针处
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (6).ToString("X2")), 2);
                        if (!NetCom3.Instance.WashQuery())
                        {
                            goto enzymaticActivityTestEnd;
                        }
                        string strWash = "EB 90 31 03 03 ";

                        if (j == 0)
                        {
                            strWash += "00 10";
                        }
                        else if (j == 1)
                        {
                            strWash += "00 10";
                        }
                        else if (j == 2)
                        {
                            strWash += "00 10";
                        }
                        else if (j == 3)
                        {
                            strWash += "01 10";
                        }
                        else if (j == 4)
                        {
                            strWash += "01 11";
                        }
                        //抽液注液指令
                        NetCom3.Instance.Send(NetCom3.Cover(strWash), 2);
                        if (!NetCom3.Instance.WashQuery())
                        {
                            goto enzymaticActivityTestEnd;
                        }
                        if ((j + 1) < oneTimes) //如果还要夹下一个管
                        {
                            //转到2号位 配合1号位夹新管保持好队形
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-5).ToString("X2").Substring(6, 2)), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
                                goto enzymaticActivityTestEnd;
                            }
                        }
                    }

                    for (int k = 7; k < oneTimes + 7; k++)
                    {
                        washPos[k] = 1;
                    }
                    //清洗盘指令
                    while (washPos[0] == 0)
                    {
                        washPos = specificationTestWash(washPos, readNum);

                        if (washPos[24] == 1)
                        {
                            num++;
                            BeginInvoke(new Action(() =>
                            {
                                textBox3.AppendText(Environment.NewLine + Environment.NewLine + DateTime.Now.ToString("HH-mm-ss") + ": 第 " + num + " 个管正在读数。" + Environment.NewLine);
                            }));
                        }
                        while (DateTime.Now.Subtract(dtimeWash).TotalMilliseconds < 20000) //20s一格20000
                            NetCom3.Delay(100);
                    }

                    //扔管
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (6 - oneTimes).ToString("X2")), 2); //转到30
                    if (!NetCom3.Instance.WashQuery())
                    {
                        goto enzymaticActivityTestEnd;
                    }
                    for (int m = 0; m < oneTimes; m++)//循环扔管
                    {
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2); //转1位到1
                        if (!NetCom3.Instance.WashQuery())
                        {
                            goto enzymaticActivityTestEnd;
                        }
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);//取放管处扔管
                        if (!NetCom3.Instance.MoveQuery())
                        {
                            goto enzymaticActivityTestEnd;
                        }
                    }
                    for (int n = 0; n < washPos.Length; n++)
                        washPos[n] = 0;
                    repeat = repeat - oneTimes;
                }

            }
            #region 清洗效果测试一次一个 注释
            //else if (testIndex == 2)
            //{
            //    frmMsgShow.MessageShow("仪器调试", "请确认在温育盘 " + reactTrayStartPos + " 号到 " + reactTrayEndPos + " 号孔位是否放好反应管，是否加好酶，点击确定马上开始测试。");

            //    BeginInvoke(new Action(() =>
            //    {
            //        textBox3.AppendText(DateTime.Now.ToString("HH-mm-ss") + ": 测试开始。" + Environment.NewLine);
            //    }));

            //    int num = 1; //第num个管在读数
            //    for (int i = 0; i < repeat; i++)
            //    {
            //        //抓一个管从温育盘到清洗盘
            //        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 02 " + reactTrayStartPos.ToString("x2")), 1);
            //        if (!NetCom3.Instance.MoveQuery())
            //        {
            //            frmMsgShow.MessageShow("ERROR", "移管错误，错误类型：" + NetCom3.Instance.MoverrorFlag);
            //            goto enzymaticActivityTestEnd;
            //        }
            //        reactTrayStartPos++;

            //        //清洗盘逆时针旋转6位 跳过第一个抽液针，到第一个注液针处
            //        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (6).ToString("X2")), 2);
            //        if (!NetCom3.Instance.WashQuery())
            //        {
            //            goto enzymaticActivityTestEnd;
            //        }

            //        //注液-1
            //        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 10"), 2);
            //        if (!NetCom3.Instance.WashQuery())
            //        {
            //            goto enzymaticActivityTestEnd;
            //        }
            //        NetCom3.Delay(100);

            //        //清洗盘逆时针旋转3位 到抽液
            //        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (3).ToString("X2")), 2);
            //        if (!NetCom3.Instance.WashQuery())
            //        {
            //            goto enzymaticActivityTestEnd;
            //        }

            //        //抽液-2
            //        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 01"), 2);
            //        if (!NetCom3.Instance.WashQuery())
            //        {
            //            goto enzymaticActivityTestEnd;
            //        }
            //        NetCom3.Delay(100);

            //        //旋转一位
            //        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (1).ToString("X2")), 2);
            //        if (!NetCom3.Instance.WashQuery())
            //        {
            //            goto enzymaticActivityTestEnd;
            //        }

            //        //注液-2
            //        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 01"), 2);
            //        if (!NetCom3.Instance.WashQuery())
            //        {
            //            goto enzymaticActivityTestEnd;
            //        }
            //        NetCom3.Delay(100);

            //        //-------------------------
            //        //清洗盘逆时针旋转3位 到抽液
            //        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (3).ToString("X2")), 2);
            //        if (!NetCom3.Instance.WashQuery())
            //        {
            //            goto enzymaticActivityTestEnd;
            //        }

            //        //抽液-3
            //        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 01"), 2);
            //        if (!NetCom3.Instance.WashQuery())
            //        {
            //            goto enzymaticActivityTestEnd;
            //        }
            //        NetCom3.Delay(100);

            //        //旋转一位
            //        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (1).ToString("X2")), 2);
            //        if (!NetCom3.Instance.WashQuery())
            //        {
            //            goto enzymaticActivityTestEnd;
            //        }

            //        //注液-3
            //        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 10"), 2);
            //        if (!NetCom3.Instance.WashQuery())
            //        {
            //            goto enzymaticActivityTestEnd;
            //        }
            //        NetCom3.Delay(100);

            //        //清洗盘逆时针旋转3位 到抽液
            //        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (3).ToString("X2")), 2);
            //        if (!NetCom3.Instance.WashQuery())
            //        {
            //            goto enzymaticActivityTestEnd;
            //        }

            //        //抽液-4
            //        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 01"), 2);
            //        if (!NetCom3.Instance.WashQuery())
            //        {
            //            goto enzymaticActivityTestEnd;
            //        }
            //        NetCom3.Delay(100);

            //        //清洗盘逆时针旋转2位 到加底物
            //        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (2).ToString("X2")), 2);
            //        if (!NetCom3.Instance.WashQuery())
            //        {
            //            goto enzymaticActivityTestEnd;
            //        }

            //        //加底物
            //        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 01 10"), 2);
            //        if (!NetCom3.Instance.WashQuery())
            //        {
            //            goto enzymaticActivityTestEnd;
            //        }
            //        NetCom3.Delay(100);

            //        //旋转5位 到读数
            //        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (5).ToString("X2")), 2);
            //        if (!NetCom3.Instance.WashQuery())
            //        {
            //            goto enzymaticActivityTestEnd;
            //        }

            //        //读数
            //        BeginInvoke(new Action(() =>
            //        {
            //            textBox3.AppendText(Environment.NewLine + Environment.NewLine + DateTime.Now.ToString("HH-mm-ss") + ": 第 " + num++ + " 个管正在读数。" + Environment.NewLine);
            //        }));
            //        for (int j = 0; j < readNum; j++)
            //        {
            //            BackObj = "";
            //            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 00 01"), 2);
            //            if (!NetCom3.Instance.WashQuery())
            //            {
            //                goto enzymaticActivityTestEnd;
            //            }
            //            else
            //            {
            //                int delay = 1000;
            //                while (!BackObj.Contains("EB 90 31 A3") && delay > 0)
            //                {
            //                    NetCom3.Delay(10);
            //                    delay = delay - 10;
            //                }
            //                if (BackObj.Contains("EB 90 31 A3"))
            //                {
            //                    string temp = BackObj.Substring(BackObj.Length - 16).Replace(" ", "");
            //                    temp = Convert.ToInt64(temp, 16).ToString();

            //                    if (int.Parse(temp) > Math.Pow(10, 5))
            //                        temp = ((int)GetPMT(double.Parse(temp))).ToString();
            //                    BeginInvoke(new Action(() =>
            //                    {
            //                        textBox3.AppendText(DateTime.Now.ToString("HH-mm-ss") + ": " + "PMT背景值:" + temp + Environment.NewLine);
            //                    }));

            //                }
            //            }
            //            NetCom3.Delay(100);
            //        }

            //        //旋转到取放管处
            //        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (6).ToString("X2")), 2);
            //        if (!NetCom3.Instance.WashQuery())
            //        {
            //            goto enzymaticActivityTestEnd;
            //        }

            //        //在取放管处扔管
            //        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
            //        if (!NetCom3.Instance.MoveQuery())
            //        {
            //            goto enzymaticActivityTestEnd;
            //        }
            //        NetCom3.Delay(500);
            //    }
            //}
            #endregion
            else if (testIndex == 3)
            {
                int repeat2 = int.Parse(txtbox2.Text);//每个样本杯加样次数
                int sampleTubeNum = int.Parse(untxtSamplePos2.Text) - int.Parse(untxtSamplePos.Text) + 1;
                if (repeat2 < 1)
                {
                    MessageBox.Show("请输入大于0的测试次数");
                    goto enzymaticActivityTestEnd;
                }
                DialogResult r = MessageBox.Show("请确认准备完毕，点击确定马上开始测试。", "仪器调试", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (r == DialogResult.Cancel)
                {
                    goto enzymaticActivityTestEnd;
                }
                BeginInvoke(new Action(() =>
                {
                    textBox3.AppendText(DateTime.Now.ToString("HH-mm-ss") + ": 测试开始。" + Environment.NewLine);
                }));

                //判断温育盘1号位有无反应管
                DataTable dtInTrayIni = OperateIniFile.ReadConfig(iniPathReactTrayInfo);
                if (int.Parse(dtInTrayIni.Rows[0][1].ToString()) >= 1)
                {
                    //有管扔掉
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 01"), 1);
                    if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                    {
                        MessageBox.Show("异常，请重试");
                        goto enzymaticActivityTestEnd;
                    }
                    //修改反应盘信息
                    OperateIniFile.WriteIniData("ReactTrayInfo", "no1", "0", iniPathReactTrayInfo);
                }

                samplePos = (int.Parse(samplePos) - 1).ToString();
                for (int samp = 1; samp <= sampleTubeNum; samp++)
                {
                    samplePos = (int.Parse(samplePos) + 1).ToString();
                    int num = 1; //第samp个样本杯，第num个管在读数
                    for (int i = 0; i < repeat2; i++)
                    {
                        //夹新管到温育盘1号位
                        addTubeAgain:
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 01 01"), 1);
                        if (!NetCom3.Instance.MoveQuery())
                        {
                            if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.putKnocked)
                            {
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 01"), 1);
                                if (!NetCom3.Instance.MoveQuery())
                                {
                                    MessageBox.Show("异常，请重试");
                                    goto enzymaticActivityTestEnd;
                                }
                                goto addTubeAgain;
                            }
                            else
                            {
                                MessageBox.Show("异常，请重试");
                                goto enzymaticActivityTestEnd;
                            }
                        }
                        //更新配置文件
                        OperateIniFile.WriteIniData("ReactTrayInfo", "no1", "1", iniPathReactTrayInfo);
                        //加样  
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 01 " + int.Parse(samplePos).ToString("X2")
                            + " 01 " + sampleVol.ToString("X2")), 0);
                        if (!NetCom3.Instance.SPQuery())
                        {
                            goto enzymaticActivityTestEnd;
                        }
                        //移管到清洗盘
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 02 01"), 1);
                        if (!NetCom3.Instance.MoveQuery())
                        {
                            MessageBox.Show("异常，请重试");
                            goto enzymaticActivityTestEnd;
                        }
                        //修改反应盘信息
                        OperateIniFile.WriteIniData("ReactTrayInfo", "no1", "0", iniPathReactTrayInfo);
                        //旋转到20格
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 13"), 2);
                        if (!NetCom3.Instance.WashQuery())
                        {
                            goto enzymaticActivityTestEnd;
                        }
                        //加底物
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 01 10"), 2);
                        if (!NetCom3.Instance.WashQuery())
                        {
                            goto enzymaticActivityTestEnd;
                        }
                        //旋转到25格
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 05"), 2);
                        if (!NetCom3.Instance.WashQuery())
                        {
                            goto enzymaticActivityTestEnd;
                        }
                        //循环读数                  
                        BeginInvoke(new Action(() =>
                        {
                            textBox3.AppendText(Environment.NewLine + Environment.NewLine + DateTime.Now.ToString("HH-mm-ss") + ": 第 " + samp + " 个样本杯，第 " + num++ + " 个管正在读数。" + Environment.NewLine);
                        }));
                        for (int j = 0; j < readNum; j++)
                        {
                            BackObj = "";
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 00 11"), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
                                goto enzymaticActivityTestEnd;
                            }
                            else
                            {
                                int delay = 1000;
                                while (!BackObj.Contains("EB 90 31 A3") && delay > 0)
                                {
                                    NetCom3.Delay(10);
                                    delay = delay - 10;
                                }
                                if (BackObj.Contains("EB 90 31 A3"))
                                {
                                    string temp = BackObj.Substring(BackObj.Length - 16).Replace(" ", "");
                                    temp = Convert.ToInt64(temp, 16).ToString();

                                    if (int.Parse(temp) > Math.Pow(10, 5))
                                        temp = ((int)GetPMT(double.Parse(temp))).ToString();
                                    BeginInvoke(new Action(() =>
                                    {
                                        textBox3.AppendText(DateTime.Now.ToString("HH-mm-ss") + ": " + "PMT背景值:" + temp + Environment.NewLine);
                                    }));
                                }
                            }
                            NetCom3.Delay(500);
                        }
                        //旋转到1号位
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 06"), 2);
                        if (!NetCom3.Instance.WashQuery())
                        {
                            goto enzymaticActivityTestEnd;
                        }
                        //清洗盘扔管
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
                        if (!NetCom3.Instance.MoveQuery())
                        {
                            MessageBox.Show("异常，请重试");
                            goto enzymaticActivityTestEnd;
                        }
                    }
                }
            }
            //清洗注液测试
            else if (testIndex == 4)
            {
                DialogResult r = MessageBox.Show("请确认准备完毕，点击确定马上开始测试。", "仪器调试", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (r == DialogResult.Cancel)
                {
                    goto enzymaticActivityTestEnd;
                }
                if (!washTrayTubeClear())
                {
                    MessageBox.Show("清空清洗盘失败，请重试");
                    goto enzymaticActivityTestEnd;
                }

                int needPos = InjectionNum * 4;//一组需要4个孔位

                //检测1-needPos孔位是否有管，清掉
                DataTable dtInTrayIni = OperateIniFile.ReadConfig(iniPathReactTrayInfo);
                for (int i = 0; i < needPos; i++)
                {
                    if (int.Parse(dtInTrayIni.Rows[i][1].ToString()) >= 1)//有管
                    {
                        //有管扔掉
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 " + (i + 1).ToString("X2")), 1);
                        if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                        {
                            MessageBox.Show("异常，请重试");
                            goto enzymaticActivityTestEnd;
                        }
                        //修改反应盘信息
                        OperateIniFile.WriteIniData("ReactTrayInfo", "no" + (i + 1), "0", iniPathReactTrayInfo);
                    }
                }
                //夹新管
                for (int i = 0; i < needPos; i++)
                {
                    if (i % 4 == 3)
                    {
                        continue;
                    }
                    int second = 0;
                    againInjection:
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 01 " + (i + 1).ToString("X2")), 1);
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull || NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked)
                        {
                            second++;
                            if (second < 2)
                                goto againInjection;
                            else
                            {
                                MessageBox.Show("异常，请重试");
                                goto enzymaticActivityTestEnd;
                            }
                        }
                        else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.putKnocked)
                        {
                            second++;
                            if (second < 2)
                            {
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 " + (i + 1).ToString("X2")), 1);
                                if (!NetCom3.Instance.MoveQuery())
                                {
                                    MessageBox.Show("异常，请重试");
                                    goto enzymaticActivityTestEnd;
                                }
                                goto againInjection;
                            }
                            else
                            {
                                MessageBox.Show("异常，请重试");
                                goto enzymaticActivityTestEnd;
                            }
                        }
                        else
                        {
                            MessageBox.Show("异常，请重试");
                            goto enzymaticActivityTestEnd;
                        }
                    }
                    //修改反应盘信息
                    OperateIniFile.WriteIniData("ReactTrayInfo", "no" + (i + 1), "1", iniPathReactTrayInfo);
                }
                //for循环，温育盘-》清洗盘（旋转），注液，清洗盘=》温育盘
                for (int i = 0; i < InjectionNum; i++)
                {
                    int actPos = 4 * i + 1;//1 5 9
                    int second = 0;

                    actPos += 2;
                    //加第一个管
                    moveAgain:
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 02 " + (actPos).ToString("X2")), 1);
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull || NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked)
                        {
                            second++;
                            if (second < 2)
                                goto moveAgain;
                            else
                            {
                                MessageBox.Show("异常，请重试");
                                goto enzymaticActivityTestEnd;
                            }
                        }
                        else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.putKnocked)
                        {
                            second++;
                            if (second < 2)
                            {
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
                                if (!NetCom3.Instance.MoveQuery())
                                {
                                    MessageBox.Show("异常，请重试");
                                    goto enzymaticActivityTestEnd;
                                }
                                goto moveAgain;
                            }
                            else
                            {
                                MessageBox.Show("异常，请重试");
                                goto enzymaticActivityTestEnd;
                            }
                        }
                        else
                        {
                            MessageBox.Show("异常，请重试");
                            goto enzymaticActivityTestEnd;
                        }
                    }
                    //修改反应盘信息
                    OperateIniFile.WriteIniData("ReactTrayInfo", "no" + (actPos--), "0", iniPathReactTrayInfo);
                    //逆时针旋转4格
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (4).ToString("X2")), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        MessageBox.Show("异常，请重试");
                        goto enzymaticActivityTestEnd;
                    }
                    //加第二个管
                    second = 0;
                    moveAgain2:
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 02 " + (actPos).ToString("X2")), 1);
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull || NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked)
                        {
                            second++;
                            if (second < 2)
                                goto moveAgain2;
                            else
                            {
                                MessageBox.Show("异常，请重试");
                                goto enzymaticActivityTestEnd;
                            }
                        }
                        else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.putKnocked)
                        {
                            second++;
                            if (second < 2)
                            {
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
                                if (!NetCom3.Instance.MoveQuery())
                                {
                                    MessageBox.Show("异常，请重试");
                                    goto enzymaticActivityTestEnd;
                                }
                                goto moveAgain2;
                            }
                            else
                            {
                                MessageBox.Show("异常，请重试");
                                goto enzymaticActivityTestEnd;
                            }
                        }
                        else
                        {
                            MessageBox.Show("异常，请重试");
                            goto enzymaticActivityTestEnd;
                        }
                    }
                    //修改反应盘信息
                    OperateIniFile.WriteIniData("ReactTrayInfo", "no" + (actPos--), "0", iniPathReactTrayInfo);
                    //逆时针旋转4格
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (4).ToString("X2")), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        MessageBox.Show("异常，请重试");
                        goto enzymaticActivityTestEnd;
                    }
                    //加第三个管
                    second = 0;
                    moveAgain3:
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 02 " + (actPos).ToString("X2")), 1);
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull || NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked)
                        {
                            second++;
                            if (second < 2)
                                goto moveAgain3;
                            else
                            {
                                MessageBox.Show("异常，请重试");
                                goto enzymaticActivityTestEnd;
                            }
                        }
                        else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.putKnocked)
                        {
                            second++;
                            if (second < 2)
                            {
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
                                if (!NetCom3.Instance.MoveQuery())
                                {
                                    MessageBox.Show("异常，请重试");
                                    goto enzymaticActivityTestEnd;
                                }
                                goto moveAgain3;
                            }
                            else
                            {
                                MessageBox.Show("异常，请重试");
                                goto enzymaticActivityTestEnd;
                            }
                        }
                        else
                        {
                            MessageBox.Show("异常，请重试");
                            goto enzymaticActivityTestEnd;
                        }
                    }
                    //修改反应盘信息
                    OperateIniFile.WriteIniData("ReactTrayInfo", "no" + (actPos--), "0", iniPathReactTrayInfo);
                    //逆时针旋转6格
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (6).ToString("X2")), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        MessageBox.Show("异常，请重试");
                        goto enzymaticActivityTestEnd;
                    }
                    //三个管路注液
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 11 10"), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        MessageBox.Show("异常，请重试");
                        goto enzymaticActivityTestEnd;
                    }
                    //清洗盘=》温育盘，放回
                    //逆时针旋转16格
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (16).ToString("X2")), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        MessageBox.Show("异常，请重试");
                        goto enzymaticActivityTestEnd;
                    }
                    //放回--actPos
                    second = 0;
                    actPos += 4;
                    moveback1:
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 03 " + (--actPos).ToString("X2") + " 02"), 1);
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull || NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked || NetCom3.Instance.MoverrorFlag == (int)ErrorState.putKnocked)
                        {
                            second++;
                            if (second < 2)
                                goto moveback1;
                            else
                            {
                                MessageBox.Show("异常，请重试");
                                goto enzymaticActivityTestEnd;
                            }
                        }
                        else
                        {
                            MessageBox.Show("异常，请重试");
                            goto enzymaticActivityTestEnd;
                        }
                    }
                    //修改反应盘信息
                    OperateIniFile.WriteIniData("ReactTrayInfo", "no" + (actPos), "1", iniPathReactTrayInfo);
                    //逆时针旋转4格
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (4).ToString("X2")), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        MessageBox.Show("异常，请重试");
                        goto enzymaticActivityTestEnd;
                    }
                    //放回--actPos
                    second = 0;
                    moveback2:
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 03 " + (--actPos).ToString("X2") + " 02"), 1);
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull || NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked || NetCom3.Instance.MoverrorFlag == (int)ErrorState.putKnocked)
                        {
                            second++;
                            if (second < 2)
                                goto moveback2;
                            else
                            {
                                MessageBox.Show("异常，请重试");
                                goto enzymaticActivityTestEnd;
                            }
                        }
                        else
                        {
                            MessageBox.Show("异常，请重试");
                            goto enzymaticActivityTestEnd;
                        }
                    }
                    //修改反应盘信息
                    OperateIniFile.WriteIniData("ReactTrayInfo", "no" + (actPos), "1", iniPathReactTrayInfo);
                    //逆时针旋转4格
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (4).ToString("X2")), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        MessageBox.Show("异常，请重试");
                        goto enzymaticActivityTestEnd;
                    }
                    //放回--actPos
                    second = 0;
                    moveback3:
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 03 " + (--actPos).ToString("X2") + " 02"), 1);
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull || NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked || NetCom3.Instance.MoverrorFlag == (int)ErrorState.putKnocked)
                        {
                            second++;
                            if (second < 2)
                                goto moveback3;
                            else
                            {
                                MessageBox.Show("异常，请重试");
                                goto enzymaticActivityTestEnd;
                            }
                        }
                        else
                        {
                            MessageBox.Show("异常，请重试");
                            goto enzymaticActivityTestEnd;
                        }
                    }
                    //修改反应盘信息
                    OperateIniFile.WriteIniData("ReactTrayInfo", "no" + (actPos), "1", iniPathReactTrayInfo);
                }

            }

            //正常结束测试执行初始化命令
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 F1 02"), 5);
            if (!NetCom3.Instance.SingleQuery())
            {
                if (NetCom3.Instance.ErrorMessage != null)
                    MessageBox.Show(NetCom3.Instance.ErrorMessage, "仪器初始化");
            }

            //测试结束
            enzymaticActivityTestEnd:
            BeginInvoke(new Action(() =>
            {
                textBox3.AppendText(Environment.NewLine + DateTime.Now.ToString("HH-mm-ss") + ": 测试结束。" + Environment.NewLine + Environment.NewLine);
                threadList.RemoveAll(xy => xy.Name == "specificationTest");
                fbtnTestS.Enabled = true;
                cmbTestName.Enabled = true;
                fbtnTestsStop.Enabled = false;
            }));
        }


        /// <summary>
        /// 规范测试清洗效果测试 清洗盘指令
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="readFrequency">循环几次读数</param>
        /// <returns></returns>
        private int[] specificationTestWash(int[] pos, int readFrequency)
        {
            pos[0] = 0;
            string order = "EB 90 31 03 03 ";

            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2); //清洗盘旋转1位
            if (!NetCom3.Instance.WashQuery())
            {
                goto enzymaticActivityTestEnd;
            }
            dtimeWash = DateTime.Now;
            //旋转1位后重置清洗盘放管状态
            int[] temp = new int[31];
            for (int i = 1; i <= 29; i++)
            {
                temp[i + 1] = pos[i];
            }
            temp[1] = pos[30];
            pos = temp;

            //判断是否需要执行指令
            if (pos[10] == 0 && pos[11] == 0 && pos[14] == 0 && pos[15] == 0 && pos[18] == 0 && pos[20] == 0 && pos[25] == 0)
            {
                return pos;
            }

            if (pos[10] == 1 || pos[14] == 1 || pos[18] == 1)//不经过6号位第一根抽液针，其他3根抽液针下面有管则抽液
            {
                order += "01 ";
            }
            else
                order += "00 ";

            //3个注液指令
            if (pos[7] == 1)
            {
                order += "1"; //不可能执行
            }
            else
            {
                order += "0";
            }
            if (pos[11] == 1)
            {
                order += "1 ";
            }
            else
            {
                order += "0 ";
            }
            if (pos[15] == 1)
            {
                order += "1";
            }
            else
            {
                order += "0";
            }

            //加底物
            if (pos[20] == 1)
            {
                order += "1 1";
            }
            else
            {
                order += "0 0";
            }

            //读数指令
            if (pos[25] == 1)
            {
                order += "1";
            }
            else
            {
                order += "0";
            }


            if (pos[25] == 1) //因为是一次循环5个管，所以读数的时候其他位置不可能有管。
            {
                BackObj = "";
                //发送指令
                for (int i = 0; i < readFrequency; i++)
                {
                    NetCom3.Instance.Send(NetCom3.Cover(order), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        goto enzymaticActivityTestEnd;
                    }
                    else
                    {
                        int delay = 1000;
                        while (!BackObj.Contains("EB 90 31 A3") && delay > 0)
                        {
                            NetCom3.Delay(10);
                            delay = delay - 10;
                        }
                        if (BackObj.Contains("EB 90 31 A3"))
                        {
                            string readTemp = BackObj.Substring(BackObj.Length - 16).Replace(" ", "");
                            readTemp = Convert.ToInt64(readTemp, 16).ToString();

                            if (int.Parse(readTemp) > Math.Pow(10, 5))
                                readTemp = ((int)GetPMT(double.Parse(readTemp))).ToString();
                            BeginInvoke(new Action(() =>
                            {
                                textBox3.AppendText(DateTime.Now.ToString("HH-mm-ss") + ": " + "PMT背景值:" + readTemp + Environment.NewLine);
                            }));

                        }
                    }
                }
            }
            else
            {
                NetCom3.Instance.Send(NetCom3.Cover(order), 2);
                if (!NetCom3.Instance.WashQuery())
                {
                    goto enzymaticActivityTestEnd;
                }
            }

            for (int i = 7; i <= 24; i++) //判断是否是最后一次
            {
                if (pos[i] == 1) //有管就还没有结束
                    break;
                if (i == 24) //走到了这里说明前面都没有管了
                    goto enzymaticActivityTestEnd;
            }

            return pos;

            enzymaticActivityTestEnd:
            pos[0] = 1; //因为到这一步pos[0]不可能是1，所以暂时当作一个结束的标志位

            return pos;
        }

        private void Fbtnexp_Click(object sender, EventArgs e)
        {
            if (dtexp.Rows.Count == 0)
            {
                MessageBox.Show("没有实验结果。", "null");
                return;
            }
            try
            {
                #region 数据导出                  
                string filePath = "";
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "请选择保存文件夹";
                DialogResult flag = dialog.ShowDialog();
                if (flag != DialogResult.OK)
                    return;

                filePath = dialog.SelectedPath + @"\" + dtexp.Columns[0].ColumnName.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd HHmmss") + ".xls";
                DataTableExcel.TableToExcel(dtexp, filePath);
                frmMsgShow.MessageShow("SUCCESS", "导出成功。");

                #endregion
            }
            catch (System.Exception ex)
            {
                frmMsgShow.MessageShow("ERROR", "导出失败。\n" + ex.Message);
            }
        }

        private void TextBox3_TextChanged(object sender, EventArgs e)
        {
            int index = textBox3.Lines.Count();
            if (index == 0)
                return;
            string str = textBox3.Lines.ElementAtOrDefault(index--);
            while (str == null || str == "")
            {
                str = textBox3.Lines.ElementAtOrDefault(index--);
            }
            if (str.Contains("PMT"))
            {
                string[] s = str.Trim().Split(':');
                dtexp.Rows.Add(s[0] + ":" + s[1], s[2]);
            }
            else if (str != null)
            {
                dtexp.Rows.Add(str, "");
            }
        }

        private void FbtnExportToOldFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            //dialog.Title = "请选择更新目标文件";
            dialog.Title = "请选择相应文件添加当前测试记录";
            dialog.Filter = "xls文件|*.xls";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string path = dialog.FileName;
                    DataTable dt = OperateExcel.ImPortExcel(path);

                    if (dt.Rows[0][0] != DBNull.Value)
                    {
                        dt.Rows.Add("", "");
                        dt.Rows.Add("", "");
                        dt.Rows.Add(dtexp.Columns[0].ColumnName, "PMT");
                        foreach (DataRow dar in dtexp.Rows)
                        {
                            dt.Rows.Add(dar[0], dar[1]);

                        }

                        if (File.Exists(path))
                            File.Delete(path);
                        DataTableExcel.TableToExcel(dt, path);
                    }
                    else
                    {
                        if (DialogResult.OK != MessageBox.Show("该excel表格不符合规定，是否清空重新加载。", "WARNING", MessageBoxButtons.OKCancel))
                            return;
                        if (File.Exists(path))
                            File.Delete(path);

                        DataTableExcel.TableToExcel(dtexp, path);
                    }

                    frmMsgShow.MessageShow("SUCCESS", "添加记录成功。");
                }
                catch (System.Exception ex)
                {
                    frmMsgShow.MessageShow("ERROR", "添加记录失败。\n" + ex.Message);
                }

            }
        }

        private void btnHandClose_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            btnHandClose.Enabled = false;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 03 30"), 5);
            NetCom3.Instance.SingleQuery();
            //if (btnHandOpen.Text == "抓手打开")
            //{
            //    btnHandOpen.Text = "抓手关闭";
            //    NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 03 31"), 5);
            //    NetCom3.Instance.SingleQuery();
            //}
            //else
            //{
            //    btnHandOpen.Text = "抓手打开";
            //    NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 03 30"), 5);
            //    NetCom3.Instance.SingleQuery();
            //}
            btnHandClose.Enabled = true;
        }

        private void btnBQLiquaid_Click(object sender, EventArgs e)
        {
            btnBQLiquaid.Enabled = false;
            if (btnBQLiquaid.Text.Trim() == "打开报警查询")
            {
                BQLiquaid = true;
                btnBQLiquaid.Text = "关闭报警查询";
            }
            else
            {
                BQLiquaid = false;
                btnBQLiquaid.Text = "打开报警查询";
            }
            btnBQLiquaid.Enabled = true;
        }

        private void GetReadNum2(string order)
        {
            if (order.Contains("EB 90 31 A3"))
            {
                string temp = order.Replace(" ", "");
                int pos = temp.IndexOf("EB9031A3");
                temp = temp.Substring(pos, 32);
                temp = temp.Substring(temp.Length - 8);
                temp = Convert.ToInt64(temp, 16).ToString();
                if (int.Parse(temp) > Math.Pow(10, 5))
                    temp = ((int)GetPMT(double.Parse(temp))).ToString();
                TExtAppend(DateTime.Now.ToString("HH-mm-ss") + ": " + "PMT背景值：" + temp);
            }
        }

        private void TExtAppend(string text)
        {
            while (!this.IsHandleCreated)
            {
                Thread.Sleep(15);
            }
            textReadShow.Invoke(new Action(() => { textReadShow.AppendText(Environment.NewLine + text); }));
        }

        private bool IapTraQuery()
        {
            int sendFlag = 0;
            aabc:
            while (!NetCom3.totalOrderFlag || !NetCom3.callBack)
            {
                //Thread.Sleep(1);
            }
            if (NetCom3.Instance.errorFlag != (int)ErrorState.Success)
            {
                if (++sendFlag < 2)
                {
                    LogFile.Instance.Write("errorFlag = ： " + NetCom3.Instance.errorFlag + "  *****当前 " + DateTime.Now.ToString("HH - mm - ss") + "  *****IAP数据传输，正在等待连接 ");
                    
                    NetCom3.Delay(5000);
                    if (NetCom3.isConnect)//如果连接上了，goto继续
                    {
                        goto aabc;
                    }
                }
                LogFile.Instance.Write("MoverrorFlag = ： " + NetCom3.Instance.MoverrorFlag + "  *****当前 " + DateTime.Now.ToString("HH - mm - ss"));
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool needleUpUp = true;//加样针在上
        private bool handUpUp = true;

        string needleDownR1 = "EB 90 02 A1 01 06";//针在试剂盘R1位置
        string needleDownR2 = "EB 90 02 A1 01 05";//针在试剂盘R2位置
        string needleDownR3 = "EB 90 02 A1 01 04";//针在试剂盘R3位置
        string needleDownDilute = "EB 90 02 A1 01 03";//针在试剂盘稀释位置
        string needleDownBeads = "EB 90 02 A1 01 07";//针在试剂盘磁珠位置

        string needleDownTray = "EB 90 02 A1 01 00";//针在温育盘
        string needleModuleReset = "EB 90 02 00";//加样针模块全部复位
        string handDownTray = "EB 90 01 01 02";//抓手在温育盘
        string handModuleReset = "EB 90 01 03 00";//抓手模块全部复位
        string reactTrayReset = "EB 90 04 03 01";//温育盘复位
        string reactTrayModuleReset = "EB 90 04 03 00";//温育盘模块全部复位
        string mixHole = "EB 90 31 04 01";//混匀某孔位
        Thread Loopthread;
        bool bLoopRun = false;
        private void btnLoopTurn_Click(object sender, EventArgs e)
        {
            btnLoopTurn.Enabled = false;
            if (!(NetCom3.totalOrderFlag || NetCom3.WashReciveFlag))
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (textBox1.Text.Trim() == "")
            {
                frmMsgShow.MessageShow("仪器调试", "请输入转动个数！");
                textBox1.Focus();
                return;
            }
            if (bLoopRun)
            {
                bLoopRun = false;
                btnLoopTurn.Enabled = true;
                btnLoopTurn.Text = "循环执行";
            }
            else
            {
                string holetargethex = "";
                int holetarget = int.Parse(textBox1.Text.Trim());
                if (holetarget > 0)
                {
                    holetargethex = holetarget.ToString("x2");
                }
                else if (holetarget < 0)
                {
                    holetargethex = holetarget.ToString("X2").Substring(6, 2);
                }
                if (holetarget == 0 || holetarget == 30 || holetarget == -30)
                {
                    frmMsgShow.MessageShow("仪器调试", "当前孔位没有变化！");
                    textBox1.Focus();
                    btnLoopTurn.Enabled = true;
                }
                else if (holetarget > 30 || holetarget < -30)
                {
                    frmMsgShow.MessageShow("仪器调试", "孔位支持移动的范围为-30~30！");
                    textBox1.Focus();
                    btnLoopTurn.Enabled = true;
                }
                else if (holetarget > -30 && holetarget < 30)
                {
                    bLoopRun = true;
                    Loopthread = new Thread(new ThreadStart(TestLoopRun));
                    Loopthread.IsBackground = true;
                    Loopthread.Start();
                    btnLoopTurn.Enabled = true;
                    btnLoopTurn.Text = "停止循环";
                }
            }
        }
        void TestLoopRun()
        {
            string holetargethex = "";
            int holetarget = int.Parse(textBox1.Text.Trim());
            holetargethex = holetarget.ToString("x2");
            while (bLoopRun)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01" + " " + holetargethex.Substring(0, 2)), 2);
                NetCom3.Instance.WashQuery();
                Thread.Sleep(2000);
            }

        }

        private void cmbModelName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbModelName.SelectedItem.ToString() != "试剂盘")
            {
                if (cmbStep.Items[0].ToString() == "加热打开")
                {
                    return;
                }

                cmbStep.Items.Clear();
                object[] ob = new string[5] { "加热打开", "加热关闭", "设置校准值", "查询校准值", "查询温度" };
                cmbStep.Items.AddRange(ob);
            }
            else
            {
                if (cmbStep.Items[0].ToString() == "设置校准值")
                {
                    return;
                }

                cmbStep.Items.Clear();
                object[] ob = new string[3] { "设置校准值", "查询校准值", "查询温度" };
                cmbStep.Items.AddRange(ob);
            }
        }

        string trayResetPosition = "EB 90 04 01 03";//温育盘初始位置

        private void fbtnASSave2_Click(object sender, EventArgs e)
        {

        }

        private void fbtnASSave2_Click_1(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }

            fbtnASSave2.Enabled = false;
            List<string> data = GetData("02");
            if (data == null) data = new List<string>();
            List<string> data2 = GetData("06");

            if (data2 != null && data2.Count() > 0)
            {
                data.AddRange(data2);
            }
            if (data.Count() < 1)
            {
                MessageBox.Show("数据不存在");
            }
            foreach (string item in data)
            {
                string s = ("EB 90 " + item.Substring(0, 5) + " 13 " + item.Substring(18)).TrimEnd();
                NetCom3.Instance.Send(NetCom3.Cover(s), 5);
                NetCom3.Instance.SingleQuery();
            }
            fbtnASSave2.Enabled = true;
        }

        private void btnSaveHand2_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }

            btnSaveHand2.Enabled = false;
            List<string> data = GetData("01");

            if (data.Count() < 1)
            {
                MessageBox.Show("数据不存在");
            }
            foreach (string item in data)
            {
                string s = ("EB 90 " + item.Substring(0, 2) + " 13 " + item.Substring(18)).TrimEnd();
                NetCom3.Instance.Send(NetCom3.Cover(s), 5);
                NetCom3.Instance.SingleQuery();
            }
            btnSaveHand2.Enabled = true;
        }

        private void btnISave2_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }

            btnISave2.Enabled = false;
            List<string> data = GetData("04");

            if (data.Count() < 1)
            {
                MessageBox.Show("数据不存在");
            }
            foreach (string item in data)
            {
                string s = ("EB 90 " + item.Substring(0, 2) + " 13 " + item.Substring(18)).TrimEnd();
                NetCom3.Instance.Send(NetCom3.Cover(s), 5);
                NetCom3.Instance.SingleQuery();
            }
            btnISave2.Enabled = true;
        }

        private void fbtnWashSave2_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }

            fbtnWashSave2.Enabled = false;
            List<string> data = GetData("03");

            if (data.Count() < 1)
            {
                MessageBox.Show("数据不存在");
            }
            foreach (string item in data)
            {
                string s = ("EB 90 " + item.Substring(0, 5) + " 00 " + item.Substring(18)).TrimEnd();
                NetCom3.Instance.Send(NetCom3.Cover(s), 5);
                NetCom3.Instance.SingleQuery();
            }
            fbtnWashSave2.Enabled = true;
        }
        private List<string> GetData(string state)
        {
            string path = Directory.GetCurrentDirectory() + "\\LocationData.ini";
            if (!File.Exists(path))
            {
                MessageBox.Show("在指定目录下不存在数据文件文件，请确认！");
                return new List<string>();
            }

            List<string> data = new List<string>();
            using (StreamReader sr = new StreamReader(path, Encoding.Default))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("="))
                        data.Add(line);
                }
            }

            return data.Where(item => item.Substring(0, 2) == state).ToList();
        }
        private void sendMonitor(string order, int orderType)
        {
            if (order.Contains(needleDownTray))//针在温育盘
            {
                needleUpUp = false;

            }
            else if (order.Contains(needleDownR1) || order.Contains(needleDownR2) || order.Contains(needleDownR3)
               || order.Contains(needleDownDilute) || order.Contains(needleDownBeads))//针在试剂盘
            {
                needleUpUp = false;
            }
            else if (order.Contains(handDownTray))//抓手在温育盘
            {
                handUpUp = false;
            }
            else if (order.Contains(needleModuleReset))//针全部复位
            {
                needleUpUp = true;
            }
            else if (order.Contains(handModuleReset))//抓手全部复位
            {
                handUpUp = true;
            }
            else if (order.Contains(reactTrayReset) || order.Contains(reactTrayModuleReset) || order.Contains(mixHole) || order.Contains(trayResetPosition))
            {
                if (needleUpUp == false || handUpUp == false)
                {
                    frmMsgShow.MessageShow("Warn", "请检查 \"加样针模组\" 或 \"抓手模组\" 是否全部复位，避免撞针.");
                    return;
                }
            }
            ////////////
            //////////
            ////////////

            NetCom3.Instance.Send(order, orderType);
        }

        private void cmbASElecMachine_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSampleSteps();
        }

        private void cmbIElecMachine_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateIncubationSteps();
        }

        private void cmbHElecMachine_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateHandSteps();
        }

        private void cmbWashElecMachine_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWashSteps();
        }

        private void fbtnInitIni_Click(object sender, EventArgs e)
        {
            fbtnInitIni.Enabled = false;

            #region 重新输入账号密码 重新确认
            frmReconfirm fr = new frmReconfirm();
            fr.ShowDialog(this);
            if (frmReconfirm.userType == "" || frmReconfirm.userType != "1")
            {
                MessageBox.Show("初始化配置文件失败！");
                goto errorEnd;
            }
            #endregion

            #region 清空 ReagentTrayInfo.ini
            for (int i = 1; i <= RegentNum; i++)
            {
                OperateIniFile.WriteIniData("ReagentPos" + i.ToString(), "BarCode", "", iniPathReagentTrayInfo);
                OperateIniFile.WriteIniData("ReagentPos" + i.ToString(), "ItemName", "", iniPathReagentTrayInfo);
                OperateIniFile.WriteIniData("ReagentPos" + i.ToString(), "BachNum", "", iniPathReagentTrayInfo);
                OperateIniFile.WriteIniData("ReagentPos" + i.ToString(), "TestCount", "", iniPathReagentTrayInfo);
                OperateIniFile.WriteIniData("ReagentPos" + i.ToString(), "LeftReagent1", "", iniPathReagentTrayInfo);
                OperateIniFile.WriteIniData("ReagentPos" + i.ToString(), "LeftReagent2", "", iniPathReagentTrayInfo);
                OperateIniFile.WriteIniData("ReagentPos" + i.ToString(), "LeftReagent3", "", iniPathReagentTrayInfo);
                OperateIniFile.WriteIniData("ReagentPos" + i.ToString(), "LeftReagent4", "", iniPathReagentTrayInfo);
                OperateIniFile.WriteIniData("ReagentPos" + i.ToString(), "leftDiuVol", "", iniPathReagentTrayInfo);
                OperateIniFile.WriteIniData("ReagentPos" + i.ToString(), "LoadDate", "", iniPathReagentTrayInfo);
            }
            #endregion
            #region 清空 ReactTrayInfo.ini
            for (int i = 1; i <= ReactTrayNum; i++)
            {
                OperateIniFile.WriteIniData("ReactTrayInfo", "no" + i, "0", iniPathReactTrayInfo);
            }
            #endregion
            #region 清空 WashTrayInfo.ini
            for (int i = 1; i <= WashTrayNum; i++)
            {
                OperateIniFile.WriteIniData("TubePosition", "no" + i, "0", iniPathWashTrayInfo);
            }
            #endregion
            #region 清空 SubstrateTube.ini

            for (int i = 1; i <= 2; i++)
            {
                OperateIniFile.WriteIniData("Substrate" + i, "BarCode", "", iniPathSubstrateTube);
                OperateIniFile.WriteIniData("Substrate" + i, "TestCount", "", iniPathSubstrateTube);
                OperateIniFile.WriteIniData("Substrate" + i, "LeftCount", "", iniPathSubstrateTube);
                OperateIniFile.WriteIniData("Substrate" + i, "LoadDate", "", iniPathSubstrateTube);
                OperateIniFile.WriteIniData("Substrate" + i, "ValidDate", DateTime.Now.AddYears(1).ToString("yyyy-MM-dd"), iniPathSubstrateTube);
                if(i == 2)
                {
                    OperateIniFile.WriteIniData("Substrate" + i, "ValidDate", "", iniPathSubstrateTube);
                }
            }

            OperateIniFile.WriteIniData("Tube", "Pos1", "0", iniPathSubstrateTube);
            OperateIniFile.WriteIniData("Tube", "Pos2", "0", iniPathSubstrateTube);
            OperateIniFile.WriteIniData("Tube", "Pos3", "0", iniPathSubstrateTube);
            OperateIniFile.WriteIniData("Tube", "Pos4", "0", iniPathSubstrateTube);
            OperateIniFile.WriteIniData("Tube", "TubeX", "0", iniPathSubstrateTube);
            OperateIniFile.WriteIniData("Tube", "TubeY", "0", iniPathSubstrateTube);
            OperateIniFile.WriteIniData("Tube", "TubePos", "1", iniPathSubstrateTube);
            OperateIniFile.WriteIniData("Tube", "ReacTrayTub", "", iniPathSubstrateTube);

            #endregion
            #region 清空 ReportSort.ini
            OperateIniFile.WriteIniData("RpSort", null, null, Directory.GetCurrentDirectory() + "\\ReportSort.ini");
            #endregion

            try
            {
                #region 清空BaseInfo.mdb
                DbHelperOleDb.ExecuteSql(2, "delete from tbDepartment where DepartmentID > 0");
                DbHelperOleDb.ExecuteSql(2, "delete from tbDoctor where DoctorID > 0");

                #endregion
                #region 清空 ProjectInfo.mdb
                DbHelperOleDb.ExecuteSql(0, "delete from tbProject where ProjectID > 0");

                #endregion
                #region 清空 SupplyStatus.mdb
                DbHelperOleDb.ExecuteSql(3, "delete from tbQC where QCID > 0");
                DbHelperOleDb.ExecuteSql(3, "delete from tbReagent where ReagentID > 0");
                DbHelperOleDb.ExecuteSql(3, "delete from tbSubstrate where SubstrateID > 0");
                DbHelperOleDb.ExecuteSql(3, "delete from tbDilute where ID > 0");

                #endregion
                #region 清空 TestResult.mdb
                DbHelperOleDb.ExecuteSql(1, "delete from tbSampleInfo where SampleID > 0");
                DbHelperOleDb.ExecuteSql(1, "delete from tbAssayResult where AssayResultID > 0");
                DbHelperOleDb.ExecuteSql(1, "delete from tbQCResult where QCResultID > 0");
                DbHelperOleDb.ExecuteSql(1, "delete from tbScalingResult where ScalingResultID > 0");
                DbHelperOleDb.ExecuteSql(1, "delete from tbMainScalCurve where MainCurveID > 0");

                #endregion
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("初始化数据库失败，请重试！" + ex.ToString());
                goto errorEnd;
            }

            //重启软件
            MessageBox.Show("初始化完成，点击确定后退出软件！", "配置文件&数据库初始化");
            fbtnInitIni.Enabled = true;
            Environment.Exit(0);

            errorEnd:
            fbtnInitIni.Enabled = true;
        }

        private void fbtnInitSpReadCard_Click(object sender, EventArgs e)
        {
            fbtnInitSpReadCard.Enabled = false;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 CA F1 01"), 5);
            NetCom3.Instance.SingleQuery();
            fbtnInitSpReadCard.Enabled = true;
        }

        private void btnOrderCancel_Click(object sender, EventArgs e)
        {
            btnOrderCancel.Enabled = false;

            if (stepOrderSendFlag == 0)//判断指令标志
            {
                NetCom3.Instance.AdderrorFlag = (int)ErrorState.OverTime;
                NetCom3.SpSendFlag = true;
                NetCom3.SpReciveFlag = true;
                NetCom3.totalOrderFlag = true;
                NetCom3.spreceiveDone.Set();
            }
            else if (stepOrderSendFlag == 1)
            {
                NetCom3.Instance.MoverrorFlag = (int)ErrorState.OverTime;
                NetCom3.MoveSendFlag = true;
                NetCom3.MoveReciveFlag = true;
                NetCom3.totalOrderFlag = true;
                NetCom3.movereceiveDone.Set();

            }
            else if (stepOrderSendFlag == 2)
            {
                NetCom3.Instance.WasherrorFlag = (int)ErrorState.OverTime;
                NetCom3.WashSendFlag = true;
                NetCom3.WashReciveFlag = true;
                NetCom3.totalOrderFlag = true;
                NetCom3.washreceiveDone.Set();
            }
            else
            {
                NetCom3.Instance.errorFlag = (int)ErrorState.OverTime;
                NetCom3.totalOrderFlag = true;
                NetCom3.DiagnostDone.Set();
            }
            NetCom3.Delay(500);
            btnOrderSend.Enabled = true;
        }
        /// <summary>
        /// 磁珠抽液泵执行标志
        /// </summary>
        bool bDrainageB = false;
        private void startDrainageB_Click(object sender, EventArgs e)
        {
            int num = int.Parse(numDrainageB.Value.ToString());

            bDrainageB = true;
            startDrainageB.Enabled = false;
            endDrainageB.Enabled = true;
            while (num > 0)
            {
                if (bDrainageB)
                {
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 0B 00 00"), 5);
                    if (!NetCom3.Instance.SingleQuery() && NetCom3.Instance.errorFlag != (int)ErrorState.ReadySend)
                    {
                        StopDrainageB();
                        return;
                    }
                    num--;
                    BeginInvoke(new Action(() => { numDrainageB.Value = num; }));
                }
                else
                {
                    StopDrainageB();
                    return;
                }
            }
            StopDrainageB();
        }

        private void endDrainageB_Click(object sender, EventArgs e)
        {
            bDrainageB = false;
        }
        void StopDrainageB()
        {
            bDrainageB = false;
            startDrainageB.Enabled = true;
            endDrainageB.Enabled = false;
        }
        /// <summary>
        /// 试剂盘温度超时默认查询
        /// </summary>
        /// <returns></returns>
        public bool reagentColdQuery()
        {
            DateTime vsDt = DateTime.Now;
            while (!NetCom3.totalOrderFlag)
            {
                if (DateTime.Now.Subtract(vsDt).TotalMilliseconds > 2000)
                {
                    NetCom3.Instance.errorFlag = (int)ErrorState.OverTime;
                    NetCom3.totalOrderFlag = true;
                    NetCom3.DiagnostDone.Set();
                }
                NetCom3.Delay(10);
            }
            if (NetCom3.Instance.errorFlag != (int)ErrorState.Success)
            {
                LogFile.Instance.Write("MoverrorFlag = ： " + NetCom3.Instance.MoverrorFlag + "  *****当前 " + DateTime.Now.ToString("HH - mm - ss"));
                return false;
            }
            else
            {
                return true;
            }
        }

        ////添加一个清洗盘个数移动，TextChanged验证是否是数字  jun add，2019/2/18
        //private string pattern = @"^[\-]?[0-9]*$";
        //private string temp = String.Empty;
        //private void txtInput_TextChanged(object sender, EventArgs e)
        //{
        //    Match m = Regex.Match(this.textBox1.Text, pattern);   // 匹配正则表达式

        //    if (!m.Success)   // 输入的不是数字
        //    {
        //        this.textBox1.Text = temp;   // textBox内容不变
        //        textBox1.Focus();
        //    }
        //    else   // 输入的是数字
        //    {
        //        temp = this.textBox1.Text;   // 将现在textBox的值保存下来
        //    }
        //}

        private void frmReturn()
        {
            if (!bClose)
            {
                return;
            }
            timer1.Stop(); //返回就结束timer  jun add 20190426
            NetCom3.Instance.ReceiveHandel -= dealAgingBack;
            foreach (Thread a in threadList)
            {
                if (a != null && a.IsAlive)
                    a.Abort();
            }
            //2018-08-31 ZLX add 试剂盘加载完成
            fbtnReturn.Enabled = false;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 0B B0 00 00"), 0);
            NetCom3.Instance.SPQuery();
            fbtnReturn.Enabled = true;
            Invoke(new Action(() =>
            {
                this.Close();
            }));
        }
    }
}
