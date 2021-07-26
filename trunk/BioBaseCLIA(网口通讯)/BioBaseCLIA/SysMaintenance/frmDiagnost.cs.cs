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

namespace BioBaseCLIA.SysMaintenance
{
    public partial class frmDiagnost : frmParent
    {
        #region 变量
        frmMessageShow frmMsgShow=new frmMessageShow();//20180524 zlx add
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
        /// <summary>
        ///
        /// </summary>
        int selectZhenID = -1;
        #endregion
        #endregion

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
        int CurrentTubePos = 1;
        /// <summary>
        /// 当前温育盘位置
        /// </summary>
        int CurrentReactPos = 1;
        /// <summary>
        /// 当前取样位置
        /// </summary>
        int CurrentAsPos = 1;
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
        public frmDiagnost()
        {
            InitializeComponent();
        }
        #region 公共方法
        private void frmDiagnost_Load(object sender, EventArgs e)
        {
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
          })) { IsBackground = true }.Start();
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
            })){ IsBackground = true }.Start();
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
            //zlx add 2018-08-31 zlx add 停止试剂盘旋转
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 09 01"), 0);
            while (!NetCom3.SpReciveFlag)
            {
                NetCom3.Delay(1);
            }
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
            this.timer1.Stop();//报了一个chart1的Nullreferencerefresh的异常，返回的时候就停止timer jun add 20190426
            fbtnReturn.Enabled = false;
            foreach (Thread a in threadList)
            {
                if (a != null && a.IsAlive)
                    a.Abort();
            }
            //2018-08-31 ZLX add 试剂盘加载完成
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
            //管架A首孔
            if (cmbHandPara.SelectedIndex == 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 A1 01 00"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //管架B首孔
            else if (cmbHandPara.SelectedIndex == 1)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 A1 01 01"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //管架C首孔
            else if (cmbHandPara.SelectedIndex == 2)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 A1 01 02"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //管架D首孔
            else if (cmbHandPara.SelectedIndex == 3)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 A1 01 03"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //温育盘位置
            else if (cmbHandPara.SelectedIndex == 4)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 A1 01 04"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //弃管位置
            else if (cmbHandPara.SelectedIndex == 5)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 A1 01 05"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //清洗盘位置
            else if (cmbHandPara.SelectedIndex == 6)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 A1 01 06"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //混匀臂垂直
            else if (cmbHandPara.SelectedIndex == 7)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 A1 01 07"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //压杯结束
            else if (cmbHandPara.SelectedIndex == 8)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 A1 01 08"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //压杯开始
            else if (cmbHandPara.SelectedIndex == 9)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 A1 01 09"), 5);
                NetCom3.Instance.SingleQuery();
            }
            cmbHandPara.Enabled = true;
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
            if (cmbHElecMachine.SelectedIndex == 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 01 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbHElecMachine.SelectedIndex == 1)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 02 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                     + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbHElecMachine.SelectedIndex == 2)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 03 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbHElecMachine.SelectedIndex == 3)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 04 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            //清洗盘电机
            else if (cmbHElecMachine.SelectedIndex == 4)
            {
                //20180524 zlx mod
                //NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 01 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                //   + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 02 02 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                 + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbHElecMachine.SelectedIndex == 5)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 05 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                   + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbHElecMachine.SelectedIndex == 6)
            {
                if (cmbHandPara.SelectedIndex == 8)
                {
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 06 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                       + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                }
                else
                {
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 07 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                       + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                }
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
            if (cmbHElecMachine.SelectedIndex == 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 01 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbHElecMachine.SelectedIndex == 1)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 02 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                     + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbHElecMachine.SelectedIndex == 2)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 03 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbHElecMachine.SelectedIndex == 3)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 04 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            //清洗盘电机
            else if (cmbHElecMachine.SelectedIndex == 4)
            {
                //20180524 zlx add
                //NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 01 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                //   + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 02 02 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                   + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbHElecMachine.SelectedIndex == 5)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 05 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                   + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbHElecMachine.SelectedIndex == 6)
            {
                if (cmbHandPara.SelectedIndex == 8)
                {
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 06 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                       + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                }
                else
                {
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 07 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                       + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);
                }
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
            btnSaveHand.Enabled = true;
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
                frmMsgShow.MessageShow("仪器调试", "请选择移管手动作");
                cmbHandIntegrate.Focus();
                return;
            }
            btnHand.Enabled = false;
            if (cmbHandIntegrate.SelectedIndex == 0)
            {
                if (txtHandValue.Text.Trim() == "")
                {
                    frmMsgShow.MessageShow("仪器调试", "请输入需要移动到反应管的位置！");
                    txtHandValue.Focus();
                    return;
                }
                string[] num = txtHandValue.Text.Trim().Split('-');
                //this block add y 20180601
                int[] temp = new int[3];
                for (int i = 0; i < 3; i++)
                {
                    if (!int.TryParse(num[i], out temp[i]))
                    {
                        frmMsgShow.MessageShow("仪器调试", "请输入数字！");
                        txtHandValue.Focus();
                        return;
                    }
                }
                if (temp[0] > 4 || temp[0] < 1)
                {
                    frmMsgShow.MessageShow("仪器调试", "请输入正确范围的板号！");
                    txtHandValue.Focus();
                    return;
                }
                if (temp[1] > 11 || temp[1] < 1)
                {
                    frmMsgShow.MessageShow("仪器调试", "请输入正确范围的行号！");
                    txtHandValue.Focus();
                    return;
                }
                if (temp[2] > 8 || temp[2] < 1)
                {
                    frmMsgShow.MessageShow("仪器调试", "请输入正确范围的列号！");
                    txtHandValue.Focus();
                    return;
                }
                temp[0]--;
                num[0] = temp[0].ToString();
                //this block end
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 01 " + int.Parse(num[0]).ToString("x2") + " "
                    + int.Parse(num[1]).ToString("x2") + " " + int.Parse(num[2]).ToString("x2")), 5);
                NetCom3.Instance.SingleQuery();
            }
            //else if (cmbHandIntegrate.SelectedIndex == 1)
            //{ 
            //    NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 01 " + int.Parse(num[0]).ToString("x2") + " "
            //        + int.Parse(num[1]).ToString("x2") + " " + int.Parse(num[2]).ToString("x2")), 5);
            //    NetCom3.Instance.SingleQuery();
            //}
            //}
            //else if (cmbHandIntegrate.SelectedIndex == 2)
            //{ }
            //else if (cmbHandIntegrate.SelectedIndex == 3)
            //{ }
            btnHand.Enabled = true;
        }

        private void fbtnINMove_Click(object sender, EventArgs e)
        {

            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbINMovePosition.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择温育盘动作！");
                cmbINMovePosition.Focus();
                return;
            }
            fbtnINMove.Enabled = false;
            int HoleNum = int.Parse(txtMoveInHoleNum.Text.Trim());
            if (cmbINMovePosition.SelectedIndex == 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 02 30" + HoleNum.ToString("x2") + " "), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbINMovePosition.SelectedIndex == 1)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 02 31 " + HoleNum.ToString("x2")), 5);
                NetCom3.Instance.SingleQuery();
            }
            fbtnINMove.Enabled = true;

        }

        private void fbtnHandZ_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbHandZ.SelectedItem == null)
            {
                cmbHandZ.Focus();
                frmMsgShow.MessageShow("仪器调试", "请选择Z轴动作！");
                return;
            }
            fbtnHandZ.Enabled = false;
            if (cmbHandZ.SelectedIndex == 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 04 30"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbHandZ.SelectedIndex == 1)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 04 31"), 5);
                NetCom3.Instance.SingleQuery();
            }
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
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 05 30"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbMix1Arm.SelectedIndex == 1)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 05 31"), 5);
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
            if (cmbPressCup.SelectedIndex == 0)//压杯垂直上
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 06 30"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbPressCup.SelectedIndex == 1)//压杯垂直下
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 06 31"), 5);
                NetCom3.Instance.SingleQuery();
            }
            if (cmbPressCup.SelectedIndex == 2)//压杯开始
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 A1 01 09"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbPressCup.SelectedIndex == 3)//压杯结束
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 A1 01 08"), 5);
                NetCom3.Instance.SingleQuery();
            }
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
            btnHandZInit.Enabled = false;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 01 00"), 5);
            NetCom3.Instance.SingleQuery();
            btnHandZInit.Enabled = true;
        }

        private void btnHandYInit_Click(object sender, EventArgs e)
        {
            //2018-09-30 mod
             if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            btnHandZInit.Enabled = false;
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 02 00"), 5);
            NetCom3.Instance.SingleQuery();
            btnHandZInit.Enabled = true;
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
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 03 00"), 5);
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
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 01 00"), 5);
            NetCom3.Instance.SingleQuery();
            btnHandAllInit.Enabled = true;
        }

        private void btnHandOpen_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            btnHandOpen.Enabled = false;
            if (btnHandOpen.Text == "抓手打开")
            {
                btnHandOpen.Text = "抓手关闭";
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 03 31"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else
            {
                btnHandOpen.Text = "抓手打开";
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 03 30"), 5);
                NetCom3.Instance.SingleQuery();
            }
            btnHandOpen.Enabled = true;//2018-09-30 zlx mod
        }


        private void fbtnMix1_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            fbtnMix1.Enabled = false;
            //2018-09-27 zlx mod
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 07 30"), 5);
            NetCom3.Instance.SingleQuery();
            //if (fbtnMix1.Text == "混匀开始")
            //{
            //    fbtnMix1.Text = "混匀停止";
            //    NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 07 30"), 5);
            //    NetCom3.Instance.SingleQuery();
            //}
            //else
            //{
            //    fbtnMix1.Text = "混匀开始";
            //    NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 07 31"), 5);
            //    NetCom3.Instance.SingleQuery();
            //}
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
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 06 30"), 5);
            NetCom3.Instance.SingleQuery();
            fbtnPressCupZero.Enabled = true;
        }

        private void fbtnInTubeClear_Click(object sender, EventArgs e)
        {
            fbtnInTubeClear.Enabled = false;
            reactTrayTubeClear();
            OperateIniFile.WriteIniData("Tube", "ReacTrayTub", "", iniPathSubstrateTube);
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
        void washTrayTubeClear()
        {
            DataTable dtWashTrayIni = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
            //int tubeExist = 0;
            //for (int i = 0; i < dtWashTrayIni.Rows.Count; i++)
            //{
            //    if (dtWashTrayIni.Rows[i][1].ToString() == "1")
            //    {
            //        tubeExist++;
            //    }

            //}
            //if (tubeExist > 0)
            //{
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
                            return;
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
                    if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
                    {
                        fbtnWashTubeClear.Enabled = true;
                        return;
                    }
                    OperateIniFile.WriteIniData("TubePosition", "No1", "0", iniPathWashTrayInfo);
                    #endregion
                }

            //}
            //else
            //{
            //    fbtnWashTubeClear.Enabled = true;
            //    return;
            //}

        }
        /// <summary>
        /// 温育反应盘清管
        /// </summary>
        void reactTrayTubeClear()
        {
            DataTable dtInTrayIni = OperateIniFile.ReadConfig(iniPathReactTrayInfo);
            for (int i = 0; i < dtInTrayIni.Rows.Count; i++)
            {
                 //2018-08-20 zlx mod
                //if (dtInTrayIni.Rows[i][1].ToString() == "1")
                //{
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 " + int.Parse(dtInTrayIni.Rows[i][0].ToString().Substring(2)).ToString("x2")), 1);
                    if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
                    {
                        fbtnInTubeClear.Enabled = true;
                        return;
                    }
                    //修改反应盘信息
                    OperateIniFile.WriteIniData("ReactTrayInfo", "no" + int.Parse(dtInTrayIni.Rows[i][0].ToString().Substring(2)).ToString(), "0", iniPathReactTrayInfo);
                //}

            }
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
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 A1 01"), 5);
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
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 A1 01 03"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //R3
            else if (cmbASPara.SelectedIndex == 4)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 A1 01 04"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //R2
            else if (cmbASPara.SelectedIndex == 5)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 A1 01 05"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //R1
            else if (cmbASPara.SelectedIndex == 6)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 A1 01 06"), 5);
                NetCom3.Instance.SingleQuery();
            }
            //磁珠
            else if (cmbASPara.SelectedIndex == 7)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 A1 01 07"), 5);
                NetCom3.Instance.SingleQuery();
            }
            cmbASPara.Enabled = true;
        }

        private void fbtnASAdd_Click(object sender, EventArgs e)
        {
            if (!NetCom3.totalOrderFlag)   //发送成功但没有运行完毕，totalOrderFlag没有改为true
            {
                frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            if (cmbASElecMachine.SelectedItem == null)    //cmbASElecMachine 是控件名
            {
                frmMsgShow.MessageShow("仪器调试", "请选择需调试的电机！");
                return;
            }
            if (txtASIncrem.Text.Trim() == "")       //txtASIncrem 是控件名
            {
                frmMsgShow.MessageShow("仪器调试", "请输入增量值！");
                return;
            }
            fbtnASAdd.Enabled = false;      //fbtnASAdd 控件名
            fbtnASAdd.ForeColor = Color.Red;
            string incream = int.Parse(txtASIncrem.Text.Trim()).ToString("x8");
            if (cmbASElecMachine.SelectedIndex == 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 01 10 " + incream.Substring(0, 2) + " " + incream.Substring(2, 2) + " "
                    + incream.Substring(4, 2) + " " + incream.Substring(6, 2)), 5);    //5是ordertype 代表调试   Instance是NetCom3命名空间里的一个静态对象
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
            fbtnASAdd.ForeColor = Color.Black;
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
            fbtnASSave.Enabled = true;
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
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 02 00"), 5);
            NetCom3.Instance.SingleQuery();
            fbtnAsAllReset.Enabled = true;
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
                cmbWashSubPipe.SelectedIndex = 0;
                cmbTestSubPipe.SelectedIndex = 0;
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

            }
            //2019.5.16  hly  modify
            else if (rdbSubstrate.Checked)
            {
                if (cmbSubstrate.SelectedItem == null)
                {
                    frmMsgShow.MessageShow("仪器调试", "请选择底物管路");
                    runFlag = false;
                    fbtnPerfusionStart.Enabled = true;
                    return;
                }
                cmbSubstrate.Enabled = false;
                txtWashTestShow.AppendText("正在进行底物管路灌注.." + Environment.NewLine + Environment.NewLine);
                int SubstratePerfuseNum = int.Parse(txtSubstrate.Text.Trim());
                int pos1 = 19;
                string subPipe = "0";
                string LeftCount1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "", iniPathSubstrateTube);
                string LeftCount2 = OperateIniFile.ReadIniData("Substrate2", "LeftCount", "", iniPathSubstrateTube);
                int leftcount1 = int.Parse(LeftCount1);
                int leftcount2 = int.Parse(LeftCount2);
                //管架取管位置
                int getTubePos;
                int plate;
                int column;
                int hole;
                int iNeedCool = 0;
                Invoke(new Action(() =>
                {
                    if (cmbSubstrate.SelectedItem.ToString() == "1")
                    {
                        subPipe = "1";
                    }
                    else if (cmbSubstrate.SelectedItem.ToString() == "2")
                    {
                        subPipe = "2";
                    }
                }));
                #region 清洗盘顺时针18位，然后放管
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (1 - pos1).ToString("X2").Substring(6, 2)), 2);
                if (!NetCom3.Instance.WashQuery())
                {
                    fbtnPerfusionStart.Enabled = true;
                    return;
                }
                currentHoleNum = currentHoleNum + (pos1 - 1);
                if (currentHoleNum > 30)
                {
                    currentHoleNum = currentHoleNum - 30;
                }
                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                getTubePos = int.Parse(OperateIniFile.ReadIniData("Tube", "TubePos", "", iniPathSubstrateTube));
                plate = getTubePos % 88 == 0 ? getTubePos / 88 - 1 : getTubePos / 88;//几号板
                column = getTubePos % 11 == 0 ? getTubePos / 11 - (plate * 8) : getTubePos / 11 + 1 - (plate * 8);
                hole = getTubePos % 11 == 0 ? 11 : getTubePos % 11;
                AgainNewMove3:
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06 " + plate.ToString("x2") + " " + column.ToString("x2") + " " + hole.ToString("x2")), 1);
                if (!NetCom3.Instance.MoveQuery())
                {
                    if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull)
                    {
                        iNeedCool++;
                        bool isMoevtoNextBoard;
                        if (iNeedCool < 12)
                        {
                            if (iNeedCool == 3 || iNeedCool == 6 || iNeedCool == 9)
                            {
                                isMoevtoNextBoard = true;
                                getTubePos = BoardNextPos(getTubePos, true, out isMoevtoNextBoard);
                            }
                            else
                            {
                                isMoevtoNextBoard = false;
                                getTubePos = BoardNextPos(getTubePos, false, out isMoevtoNextBoard);
                            }
                            plate = getTubePos % 88 == 0 ? getTubePos / 88 - 1 : getTubePos / 88;//几号板
                            column = getTubePos % 11 == 0 ? getTubePos / 11 - (plate * 8) : getTubePos / 11 + 1 - (plate * 8);
                            hole = getTubePos % 11 == 0 ? 11 : getTubePos % 11;
                            goto AgainNewMove3;
                        }
                        else
                        {
                            frmMsgShow.MessageShow("仪器调试", "移管手多次抓空，请装载管架！");
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                    fbtnPerfusionStart.Enabled = true;
                    return;
                }
                OperateIniFile.WriteIniData("TubePosition", "No19", "1", iniPathWashTrayInfo);
                #region 取放管成功，相关配置文件修改
                List<int> lisTubeNum = new List<int>();
                lisTubeNum = QueryTubeNum();
                //移管手要夹的下一个管架位置
                int NextPos = getTubePos + 1;
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
                int TubeRack = (getTubePos) / 88;
                int curTube = (getTubePos) % 88;
                if (curTube == 0 && getTubePos != 0)
                {
                    TubeRack = TubeRack - 1;
                    curTube = 88;
                }
                //那个架子减了一个管
                OperateIniFile.WriteIniData("Tube", "Pos" + (TubeRack + 1).ToString(), (88 - curTube).ToString(), iniPathSubstrateTube);
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
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    if (subPipe == "1")
                    {
                        leftcount1 = leftcount1 - 1;
                        OperateIniFile.WriteIniData("Substrate1", "LeftCount", leftcount1.ToString(), iniPathSubstrateTube);
                    }
                    else
                    {
                        leftcount2 = leftcount2 - 1;
                        OperateIniFile.WriteIniData("Substrate1", "LeftCount", leftcount2.ToString(), iniPathSubstrateTube);
                    }
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
                txtWashTestShow.AppendText("底物管路灌注完成" + Environment.NewLine + Environment.NewLine);
                cmbSubstrate.Enabled = true;
            }
            //2019.5.16  hly  modify
            else if (rdbWash.Checked)
            {
                if (cmbwash.SelectedItem == null)
                {
                    frmMsgShow.MessageShow("仪器调试", "请选择清洗管路");
                    runFlag = false;
                    fbtnPerfusionStart.Enabled = true;
                    return;
                }
                txtWashTestShow.AppendText("正在清空请洗盘.." + Environment.NewLine + Environment.NewLine);
                washTrayTubeClear();
                txtWashTestShow.AppendText("清洗盘清空完成.." + Environment.NewLine + Environment.NewLine);
                txtWashTestShow.AppendText("正在进行清洗管路灌注.." + Environment.NewLine + Environment.NewLine);
                int washPerfuseNum = int.Parse(txtWash.Text.Trim());
                //注液位置
                int pos1 = 6;
                int pos2 = 10;
                int pos3 = 14;
                //管架取管位置
                int getTubePos;
                int plate;
                int column;
                int hole;
                int iNeedCool = 0;
                if (cmbwash.SelectedItem.ToString() == "1")
                {
                    #region 注液1位置放管,在6号孔
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-6).ToString("X2").Substring(6, 2)), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    currentHoleNum = currentHoleNum - (1 - pos1);
                    if (currentHoleNum <= 0)
                    {
                        currentHoleNum = 30 + currentHoleNum;
                    }
                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                    getTubePos = int.Parse(OperateIniFile.ReadIniData("Tube", "TubePos", "", iniPathSubstrateTube));
                    plate = getTubePos % 88 == 0 ? getTubePos / 88 - 1 : getTubePos / 88;//几号板
                    column = getTubePos % 11 == 0 ? getTubePos / 11 - (plate * 8) : getTubePos / 11 + 1 - (plate * 8);
                    hole = getTubePos % 11 == 0 ? 11 : getTubePos % 11;
                AgainNewMove:
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06 " + plate.ToString("x2") + " " + column.ToString("x2") + " " + hole.ToString("x2")), 1);
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        #region
                        if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull)
                        {
                            iNeedCool++;
                            bool isMoevtoNextBoard;
                            if (iNeedCool < 12)
                            {
                                if (iNeedCool == 3 || iNeedCool == 6 || iNeedCool == 9)
                                {
                                    isMoevtoNextBoard = true;
                                    getTubePos = BoardNextPos(getTubePos, true, out isMoevtoNextBoard);
                                }
                                else
                                {
                                    isMoevtoNextBoard = false;
                                    getTubePos = BoardNextPos(getTubePos, false, out isMoevtoNextBoard);
                                }

                                plate = getTubePos % 88 == 0 ? getTubePos / 88 - 1 : getTubePos / 88;//几号板
                                column = getTubePos % 11 == 0 ? getTubePos / 11 - (plate * 8) : getTubePos / 11 + 1 - (plate * 8);
                                hole = getTubePos % 11 == 0 ? 11 : getTubePos % 11;
                                goto AgainNewMove;
                            }
                            else
                            {
                                frmMsgShow.MessageShow("仪器调试", "移管手多次抓空，请装载管架！");
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                        #endregion
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    OperateIniFile.WriteIniData("TubePosition", "No6", "1", iniPathWashTrayInfo);
                    #region 取放管成功，相关配置文件修改
                    List<int> lisTubeNum = new List<int>();
                    lisTubeNum = QueryTubeNum();
                    //移管手要夹的下一个管架位置
                    int NextPos = getTubePos + 1;
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
                    int TubeRack = (getTubePos) / 88;
                    int curTube = (getTubePos) % 88;
                    if (curTube == 0 && getTubePos != 0)
                    {
                        TubeRack = TubeRack - 1;
                        curTube = 88;
                    }
                    //那个架子减了一个管
                    OperateIniFile.WriteIniData("Tube", "Pos" + (TubeRack + 1).ToString(), (88 - curTube).ToString(), iniPathSubstrateTube);
                    //清洗盘配置文件修改
                    //OperateIniFile.WriteIniData("TubePosition", "No1", "1", iniPathWashTrayInfo);
                    #endregion
                    #endregion
                    #region 放好管之后逆时针返回
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01" + (6).ToString("X2")), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
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
                }
                if (cmbwash.SelectedItem.ToString() == "2")
                {
                    #region 注液2位置放管,在10号孔
                    //清洗盘注液2位置转到取放管位置(顺时针旋转)，4个位置
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (pos1 - pos2).ToString("X2").Substring(6, 2)), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    currentHoleNum = currentHoleNum - (pos1 - pos2);
                    if (currentHoleNum <= 0)
                    {
                        currentHoleNum = 30 + currentHoleNum;
                    }
                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                    getTubePos = int.Parse(OperateIniFile.ReadIniData("Tube", "TubePos", "", iniPathSubstrateTube));
                    plate = getTubePos % 88 == 0 ? getTubePos / 88 - 1 : getTubePos / 88;//几号板
                    column = getTubePos % 11 == 0 ? getTubePos / 11 - (plate * 8) : getTubePos / 11 + 1 - (plate * 8);
                    hole = getTubePos % 11 == 0 ? 11 : getTubePos % 11;
                    AgainNewMove1:
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06 " + plate.ToString("x2") + " " + column.ToString("x2") + " " + hole.ToString("x2")), 1);
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        #region
                        if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull)
                        {
                            iNeedCool++;
                            bool isMoevtoNextBoard;
                            if (iNeedCool < 12)
                            {
                                if (iNeedCool == 3 || iNeedCool == 6 || iNeedCool == 9)
                                {
                                    isMoevtoNextBoard = true;
                                    getTubePos = BoardNextPos(getTubePos, true, out isMoevtoNextBoard);
                                }
                                else
                                {
                                    isMoevtoNextBoard = false;
                                    getTubePos = BoardNextPos(getTubePos, false, out isMoevtoNextBoard);
                                }

                                plate = getTubePos % 88 == 0 ? getTubePos / 88 - 1 : getTubePos / 88;//几号板
                                column = getTubePos % 11 == 0 ? getTubePos / 11 - (plate * 8) : getTubePos / 11 + 1 - (plate * 8);
                                hole = getTubePos % 11 == 0 ? 11 : getTubePos % 11;
                                goto AgainNewMove1;
                            }
                            else
                            {
                                frmMsgShow.MessageShow("仪器调试", "移管手多次抓空，请装载管架！");
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                        #endregion
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    OperateIniFile.WriteIniData("TubePosition", "No10", "1", iniPathWashTrayInfo);
                    #region 取放管成功 相关配置文件修改
                    List<int> lisTubeNum = new List<int>();
                    lisTubeNum = QueryTubeNum();
                    //移管手要夹的下一个管架位置
                    int NextPos = getTubePos + 1;
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
                    int TubeRack = (getTubePos) / 88;
                    int curTube = (getTubePos) % 88;
                    if (curTube == 0 && getTubePos != 0)
                    {
                        TubeRack = TubeRack - 1;
                        curTube = 88;
                    }
                    //那个架子减了一个管
                    OperateIniFile.WriteIniData("Tube", "Pos" + (TubeRack + 1).ToString(), (88 - curTube).ToString(), iniPathSubstrateTube);
                    //清洗盘配置文件修改
                    //OperateIniFile.WriteIniData("TubePosition", "No1", "1", iniPathWashTrayInfo);
                    #endregion
                    #endregion
                    #region 放好管之后逆时针返回
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01" + (pos2).ToString("X2")), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
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
                }
                if (cmbwash.SelectedItem.ToString() == "3")
                {
                    #region 注液3位置放管，在14号孔
                    //清洗盘注液2位置转到取放管位置(顺时针旋转)
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (pos2 - pos3).ToString("X2").Substring(6, 2)), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    currentHoleNum = currentHoleNum - (pos2 - pos3);
                    if (currentHoleNum <= 0)
                    {
                        currentHoleNum = 30 + currentHoleNum;
                    }
                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                    getTubePos = int.Parse(OperateIniFile.ReadIniData("Tube", "TubePos", "", iniPathSubstrateTube));
                    plate = getTubePos % 88 == 0 ? getTubePos / 88 - 1 : getTubePos / 88;//几号板
                    column = getTubePos % 11 == 0 ? getTubePos / 11 - (plate * 8) : getTubePos / 11 + 1 - (plate * 8);
                    hole = getTubePos % 11 == 0 ? 11 : getTubePos % 11;
                    AgainNewMove2:
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06 " + plate.ToString("x2") + " " + column.ToString("x2") + " " + hole.ToString("x2")), 1);
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        #region
                        if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull)
                        {
                            iNeedCool++;
                            bool isMoevtoNextBoard;
                            if (iNeedCool < 12)
                            {
                                if (iNeedCool == 3 || iNeedCool == 6 || iNeedCool == 9)
                                {
                                    isMoevtoNextBoard = true;
                                    getTubePos = BoardNextPos(getTubePos, true, out isMoevtoNextBoard);
                                }
                                else
                                {
                                    isMoevtoNextBoard = false;
                                    getTubePos = BoardNextPos(getTubePos, false, out isMoevtoNextBoard);
                                }

                                plate = getTubePos % 88 == 0 ? getTubePos / 88 - 1 : getTubePos / 88;//几号板
                                column = getTubePos % 11 == 0 ? getTubePos / 11 - (plate * 8) : getTubePos / 11 + 1 - (plate * 8);
                                hole = getTubePos % 11 == 0 ? 11 : getTubePos % 11;
                                goto AgainNewMove2;
                            }
                            else
                            {
                                frmMsgShow.MessageShow("仪器调试", "移管手多次抓空，请装载管架！");
                                //NewWashEnd();
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                        #endregion
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    OperateIniFile.WriteIniData("TubePosition", "No14", "1", iniPathWashTrayInfo);
                    #region 取放管成功 相关配置文件修改
                    List<int> lisTubeNum = new List<int>();
                    lisTubeNum = QueryTubeNum();
                    //移管手要夹的下一个管架位置
                    int NextPos = getTubePos + 1;
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
                    int TubeRack = (getTubePos) / 88;
                    int curTube = (getTubePos) % 88;
                    if (curTube == 0 && getTubePos != 0)
                    {
                        TubeRack = TubeRack - 1;
                        curTube = 88;
                    }
                    //那个架子减了一个管
                    OperateIniFile.WriteIniData("Tube", "Pos" + (TubeRack + 1).ToString(), (88 - curTube).ToString(), iniPathSubstrateTube);
                    //清洗盘配置文件修改
                    //OperateIniFile.WriteIniData("TubePosition", "No1", "1", iniPathWashTrayInfo);
                    #endregion
                    #endregion
                    #region 放好管之后逆时针返回
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01" + (pos3).ToString("X2")), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
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
                }
                while (washPerfuseNum > 0)
                {
                    #region 注液
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 11 10"), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    #endregion

                    #region 清洗盘顺时针旋转1位
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-1).ToString("X2").Substring(6, 2)), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
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
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    #endregion
                    #endregion

                    #region 清洗盘逆时针转回注液位置
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (1).ToString("X2")), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
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
                        fbtnPerfusionStart.Enabled = true;
                        return;
                    }
                    OperateIniFile.WriteIniData("TubePosition", "No14", "0", iniPathWashTrayInfo);
                    #endregion
                    #endregion
                }
                txtWashTestShow.AppendText("清洗管路灌注完成" + Environment.NewLine + Environment.NewLine);
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
            if (cmbTestSubPipe.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择底物管路！");
                return;
            }
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
                    moveTube1.TakeTubePos = "0-" + OperateIniFile.ReadIniData("Tube", "TubePos", "1", iniPathSubstrateTube);
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
                        if (cmbTestSubPipe.SelectedItem.ToString() == "1")
                        {
                            subpipe = "1";
                        }
                        else if (cmbTestSubPipe.SelectedItem.ToString() == "2")
                        {
                            subpipe = "2";
                        }
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
                    if (cmbTestSubPipe.SelectedItem.ToString() == "1")
                    {
                        subpipe = "1";
                    }
                    else if (cmbTestSubPipe.SelectedItem.ToString() == "2")
                    {
                        subpipe = "2";
                    }
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
                        int takePos = int.Parse(OperateIniFile.ReadIniData("Tube", "TubePos", "1", iniPathSubstrateTube));//+ start
                        int plate = takePos % 88 == 0 ? takePos / 88 - 1 : takePos / 88;//几号板
                        int column = takePos % 11 == 0 ? takePos / 11 - (plate * 8) : takePos / 11 + 1 - (plate * 8);
                        int hole = takePos % 11 == 0 ? 11 : takePos % 11;
                        txtWashTestShow.AppendText("管架夹管到清洗盘。" + Environment.NewLine + Environment.NewLine);
                        ///从管架takePos位置夹管到清洗盘
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06 " + plate.ToString("x2") + " " + column.ToString("x2")
                                + " " + hole.ToString("x2")), 1);
                        if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
                        {
                            return;
                        }
                        #region 取放管成功配置文件修改
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
                        /*
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
                         */
                        int takePos = int.Parse(OperateIniFile.ReadIniData("Tube", "TubePos", "1", iniPathSubstrateTube));//+ start
                        int plate = takePos % 88 == 0 ? takePos / 88 - 1 : takePos / 88;//几号板
                        int column = takePos % 11 == 0 ? takePos / 11 - (plate * 8) : takePos / 11 + 1 - (plate * 8);
                        int hole = takePos % 11 == 0 ? 11 : takePos % 11;
                        txtWashTestShow.AppendText("管架夹管到清洗盘。" + Environment.NewLine + Environment.NewLine);
                        ///从管架takePos位置夹管到清洗盘
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06 " + plate.ToString("x2") + " " + column.ToString("x2")
                                + " " + hole.ToString("x2")), 1);
                        if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                        {
                            return;
                        }
                        #region 取放管成功配置文件修改
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
                        //清洗盘配置文件更新
                        OperateIniFile.WriteIniData("TubePosition", "No1", "1", iniPathWashTrayInfo);
                        dtWashTrayInfo.Rows[0][1] = "1";
                        #endregion
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
                        if (cmbTestSubPipe.SelectedItem.ToString() == "1")
                        {
                            subpipe = "1";
                        }
                        else if (cmbTestSubPipe.SelectedItem.ToString() == "2")
                        {
                            subpipe = "2";
                        }

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
                    if (cmbTestSubPipe.SelectedItem.ToString() == "1")
                    {
                        subpipe = "1";
                    }
                    else if (cmbTestSubPipe.SelectedItem.ToString() == "2")
                    {
                        subpipe = "2";
                    }
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
            if (cmbWashSubPipe.SelectedItem == null)
            {
                frmMsgShow.MessageShow("仪器调试", "请选择底物管路。");
                return;
            }
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
            string subPipe = cmbWashSubPipe.SelectedItem.ToString();
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
            if ((endSampos - startSamPos) * RepeatNum > 80)
            {
                frmMsgShow.MessageShow("仪器调试", "测试次数不能超过80，请重新输入！");
                return;
            }
            txtWashTestShow.Clear();
            fbtnWashTestStart.Enabled = false;
            runFlag = true;
            //启动移管线程
            MoveTubeThread = new Thread(new ParameterizedThreadStart(MoveTube));
            MoveTubeThread.IsBackground = true;
            MoveTubeThread.Start();
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
                    moveTube1.TakeTubePos = "0-" + OperateIniFile.ReadIniData("Tube", "TubePos", "1", iniPathSubstrateTube);
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

                for (int j = 1; j <= RepeatNum; j++)
                {
                    pt.takeSamplePos = i;
                    pt.ID = id++;
                    pt.addSamplePos = reactStartPos % ReactTrayHoleNum == 0 ? ReactTrayHoleNum : reactStartPos % ReactTrayHoleNum;
                    lisPt.Add(pt);
                    pt = new performanceTest();
                    reactStartPos++;
                    if (reactStartPos == 80)
                        reactStartPos = 1;
                }
            }
            #region 加样
            while (start < lisPt.Count)
            {
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
                else
                //修改反应盘信息
                    OperateIniFile.WriteIniData("ReactTrayInfo", "no" + addsamplePos, "2", iniPathReactTrayInfo);
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
            while (start < lisPt.Count)
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
                        if (AddSubstrate == "1")
                        {
                            if (cmbWashSubPipe.SelectedItem.ToString() == "1")
                            {
                                substratePipe = "1";
                            }
                            else if (cmbWashSubPipe.SelectedItem.ToString() == "2")
                            {
                                substratePipe = "2";
                            }

                        }
                        txtWashTestShow.AppendText("正在清洗、加底物或读数" + Environment.NewLine + Environment.NewLine);
                        ///发送指令
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 " + Imbibition + " " + LiquidInjectionFlag[0]
                        + LiquidInjectionFlag[1] + " " + LiquidInjectionFlag[2] + AddSubstrate + " " + substratePipe + read), 2);
                        if (!NetCom3.Instance.WashQuery())
                        {
                            return;
                        }

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

                } while ((WashExitsTube()));

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
                            int plate = int.Parse(takepos[1]) % 88 == 0 ? int.Parse(takepos[1]) / 88 - 1 : int.Parse(takepos[1]) / 88;//几号板
                            int column = int.Parse(takepos[1]) % 11 == 0 ? int.Parse(takepos[1]) / 11 - (plate * 8) : int.Parse(takepos[1]) / 11 + 1 - (plate * 8);
                            int hole = int.Parse(takepos[1]) % 11 == 0 ? 11 : int.Parse(takepos[1]) % 11;
                            BeginInvoke(new Action(() =>
                            {
                                txtWashTestShow.AppendText("管架夹管到温育盘。" + Environment.NewLine + Environment.NewLine);
                            }));
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 01 " + plate.ToString("x2") + " " + column.ToString("x2")
                               + " " + hole.ToString("x2") + " " + int.Parse(putpos[1]).ToString("x2")), 1);
                            if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
                            {
                                return;
                            }
                            #region 取管成功
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
                            if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
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
                        if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
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
                        if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
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
            txtReadShow.Clear();
            int Num = int.Parse(txtReadNum.Text);
            int readStart = int.Parse(txtReadStart.Text);
            washTrayTubeClear();
            for (int i = 1; i <= readStart; i++)
            {
                txtReadShow.AppendText("向清洗盘加第" + i + "个管" + Environment.NewLine);
                if (!AddTubeInCleanTray())
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
                txtReadShow.AppendText("第"+i+"个管读数：" + Environment.NewLine);
                for (int j = 1; j <= Num; j++)
                {
                    BackObj = "";
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 00 01"), 2);
                    if (!NetCom3.Instance.WashQuery())
                        return;
                    else
                    {
                        int delay=1000;
                        while (!BackObj.Contains("EB 90 31 A3") && delay>0)
                        {
                            NetCom3.Delay(10);
                            delay = delay-10;
                        }
                        if (BackObj.Contains("EB 90 31 A3"))
                        {
                            //string temp = BackObj.Replace(" ", "");
                            //int pos = temp.IndexOf("EB9031A3");
                            //temp = temp.Substring(pos, 32);
                            //temp = temp.Substring(temp.Length - 8);
                            string temp = BackObj.Substring(BackObj.Length - 16).Replace(" ", "");
                            temp = Convert.ToInt64(temp, 16).ToString();
                            if (double.Parse(temp) > Math.Pow(10, 5))
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
                int readEnd = int.Parse(txtReadEnd.Text);
                //读数数据表
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
        /// <summary>
        /// 加新管到清洗盘 2019-04-24 ZLX add
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private bool AddTubeInCleanTray(int pos = 0)
        {
            bool noUse;
            int boardPos;
            if (pos == 0)
            {
                boardPos = BoardNextPos(pos, false, out noUse);
            }
            else
            {
                boardPos = pos;
            }
            int plate = boardPos % 88 == 0 ? boardPos / 88 - 1 : boardPos / 88;//几号板
            int column = boardPos % 11 == 0 ? boardPos / 11 - (plate * 8) : boardPos / 11 + 1 - (plate * 8);
            int hole = boardPos % 11 == 0 ? 11 : boardPos % 11;
            int iNeedCool = 0;
            int IsKnockedCool = 0;
        AgainNewMove:
            string order = "EB 90 31 01 06 " + plate.ToString("x2") + " " + column.ToString("x2") + " " + hole.ToString("x2");
            NetCom3.Instance.Send(NetCom3.Cover(order), 1);
            if (!NetCom3.Instance.MoveQuery())
            {
                #region 发生异常处理
                if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull)
                {
                    iNeedCool++;
                    if (iNeedCool == 11)
                    {
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在管架向温育盘抓管时多次抓空!实验停止进行加样！");
                        DialogResult tempresult = MessageBox.Show("移管手抓新管多次抓空！实验将停止运行！", "移管手错误！", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        return false; ;
                    }
                    else if (iNeedCool < 11)
                    {
                        if (iNeedCool == 3 || iNeedCool == 6 || iNeedCool == 9)
                        {
                            boardPos = BoardNextPos(boardPos, true, out noUse);
                        }
                        else
                        {
                            boardPos = BoardNextPos(boardPos, false, out noUse);
                        }
                        plate = boardPos % 88 == 0 ? boardPos / 88 - 1 : boardPos / 88;
                        column = boardPos % 11 == 0 ? boardPos / 11 - (plate * 8) : boardPos / 11 + 1 - (plate * 8);
                        hole = boardPos % 11 == 0 ? 11 : boardPos % 11;
                        goto AgainNewMove;
                    }
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
                        DialogResult tempresult = MessageBox.Show("移管手在管架向清洗盘取放管处抓管发生撞管，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        return false;
                    }

                }
                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.OverTime)
                {
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在管架向温育盘抓管时接收数据超时！");
                    DialogResult tempresult = MessageBox.Show("移管手在管架向清洗盘取放管处抓管接收数据超时，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return false;
                }
                #endregion
            }
            return true;
        }
        /// <summary>
        /// 返回下一个有管位置
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="moveToNextBoard"></param>
        /// <param name="isRemoveBoarf"></param>
        /// <returns></returns>
        private int BoardNextPos(int pos, bool moveToNextBoard, out bool isRemoveBoarf)
        {
            if (pos == 0)
            {
                int boardPos = int.Parse(OperateIniFile.ReadIniData("Tube", "TubePos", "1", System.IO.Directory.GetCurrentDirectory() + "\\SubstrateTube.ini"));
                OperateIniFile.WriteIniData("Tube", "TubePos", ((boardPos + 1) > 352 ? 1 : (boardPos + 1)).ToString(), iniPathSubstrateTube);
                isRemoveBoarf = moveToNextBoard;
                int platee = boardPos % 88 == 0 ? boardPos / 88 - 1 : boardPos / 88;//2018-09-03 zlx mod
                int number = int.Parse(Common.OperateIniFile.ReadIniData("Tube", "Pos" + (platee + 1).ToString(), "1", System.IO.Directory.GetCurrentDirectory() + "\\SubstrateTube.ini"));
                if (number < 1)
                {
                    number = 1;
                }
                OperateIniFile.WriteIniData("Tube", "Pos" + (platee + 1).ToString(), (number - 1).ToString(), iniPathSubstrateTube);
                return boardPos;
            }
            isRemoveBoarf = moveToNextBoard;
            int plate = pos % 88 == 0 ? pos / 88 - 1 : pos / 88;
            int column = pos % 11 == 0 ? pos / 11 - (plate * 8) : pos / 11 + 1 - (plate * 8);
            int hole = pos % 11 == 0 ? 11 : pos % 11;
            if (moveToNextBoard)
            {
                plate++;
                if (plate > 3)
                {
                    plate = 0;
                }
                column = hole = 1;
            }
            else
            {
                hole++;
                if (hole > 11)
                {
                    hole = 1;
                    column++;
                    if (column > 8)
                    {
                        column = 1;
                        plate++;
                        isRemoveBoarf = true;
                        if (plate > 3)
                        {
                            plate = 0;
                        }
                    }
                }
            }
            if (isRemoveBoarf)
            {
                OperateIniFile.WriteIniData("Tube", "Pos" + (plate == 0 ? 4 : plate).ToString(), (0).ToString(), iniPathSubstrateTube);
            }
            OperateIniFile.WriteIniData("Tube", "TubePos", (plate * 88 + (column - 1) * 11 + hole + 1).ToString(), iniPathSubstrateTube);
            int count = int.Parse(Common.OperateIniFile.ReadIniData("Tube", "Pos" + (plate + 1).ToString(), "1", System.IO.Directory.GetCurrentDirectory() + "\\SubstrateTube.ini"));
            if (count < 1) count = 1;
            OperateIniFile.WriteIniData("Tube", "Pos" + (plate + 1).ToString(), (count - 1).ToString(), iniPathSubstrateTube);
            return plate * 88 + (column - 1) * 11 + hole;
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
                frmMsgShow.MessageShow("信息提示","未选择电控板位，请重新选择！");
                cmbZhenID.Focus();
                return;
            }
            if (txtFilePath.Text.Trim() == "")
            {
                frmMsgShow.MessageShow("信息提示","未选择文件路径，请重新选择！");
                txtFilePath.Focus();
                return;
            }
            selectZhenID = cmbZhenID.SelectedIndex;
            frmMain.LiquidQueryFlag = false;
            cmbZhenID.Enabled = txtFilePath.Enabled = btnSelectBin.Enabled = btnLoadProgram.Enabled = false;
            new Thread(new ThreadStart(LoadProgram)).Start();
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
                frmMsgShow.MessageShow("信息提示","电控板位选择错误，请重新选择！");
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
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = openFile.FileName;

            }
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
            string[] dataRecive = order.Split(' ');
            //decimal  readData=(decimal)Convert.ToInt32(dataRecive[12], 16)+(decimal )Convert.ToInt32(dataRecive[13], 16)
            //    +(decimal )Convert.ToInt32(dataRecive[14], 16)+(decimal )Convert.ToInt32(dataRecive[15], 16);
            decimal readData =Math.Round(Convert.ToDecimal(NetCom3.HexToFloat(dataRecive[12] + dataRecive[13] + dataRecive[14] + dataRecive[15])),1);
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
                        default:
                            break;
                    }
                }
                else
                {
                    while (this == null || !this.IsHandleCreated)
                    { }
                    this.Invoke(new SetTextCallBack(SettxtStandard), readData.ToString());
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
                sb.Append("采样点" + "\t" + "试剂盘温度" + "\t" + "温育盘温度" + "\t" + "清洗盘温度" + "\t" + "底物温度" + Environment.NewLine);

                for (int i = 1; i < Rlist.Count; i++)
                {
                    sb.Append(i.ToString() + "\t" + Rlist[i - 1] + "\t" + Wlist[i - 1] + "\t" + Qlist[i - 1] + "\t" + Dlist[i - 1] + Environment.NewLine);
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
                if (num1 >= num2)
                {
                    frmMsgShow.MessageShow("温度设置警告", "温度范围的选择只能从小到大，最低温度不能高于或等于最高温度");
                    numDown.Focus();
                    return;
                }
                numOfSam = (double)numOfSample.Value;
                timespan = timespanOfSample.Value;
                num1 = (double)numDown.Value;
                num2 = (double)numUp.Value;

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
                NetCom3.Instance.SingleQuery ();
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

            NetCom3.Delay(100);
            //Rlist.Add((GetNumber0() + 20) / 2 - 5);//获得温度信息，后期改为指令查询，下同
            //Wlist.Add((GetNumber1() + 40) / 2);
            //Qlist.Add((GetNumber1() + 20) / 2);
            //Dlist.Add((GetNumber1() + 100) / 6);
            //Qglist.Add((GetNumber1() + 60) / 6);
            if (Wlist.Count == 0 && Qlist.Count == 0 && Dlist.Count == 0 && Qglist.Count == 0)
                return;
            //while (Rlist.Count > numOfSam)
            //{
            //    Rlist.RemoveAt(0);
            //}
            if (chart1.IsDisposed || chart1 == null) //防止chart1 的 null异常 jun add 20190426
            {
                return;
            }
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
            NetCom3.Delay(10);
            //chart1.Series["reagent"].Points.DataBindY(Rlist);
            //chart1.Series["wenyu"].Points.DataBindY(Wlist);
            //chart1.Series["qingxi"].Points.DataBindY(Qlist);
            //chart1.Series["diwu"].Points.DataBindY(Dlist);
            //chart1.Series["qxgl"].Points.DataBindY(Qglist);
            if (Rlist.Count>0)
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
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 03 00"), 5);
            NetCom3.Instance.SingleQuery();
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

        //private void fbtnPeristalticPEx_Click(object sender, EventArgs e)
        //{
        //    if (!NetCom3.totalOrderFlag)
        //    {
        //        frmMsgShow.MessageShow("仪器调试", "仪器正在运动，请稍等！");
        //        return;
        //    }
        //    if (txtPeristalticPVol.Text.Trim() == "")
        //    {
        //        frmMsgShow.MessageShow("仪器调试", "请输入注液泵注液量！");
        //        txtPeristalticPVol.Focus();
        //        return;
        //    }
        //    fbtnPeristalticPEx.Enabled = false;
        //    //20180524 zlx mod
        //    NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 01 03"), 5);
        //    NetCom3.Instance.SingleQuery();
        //    string pumpVol = int.Parse(txtPeristalticPVol.Text.Trim()).ToString("x8");
        //    //NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 07 10 " + pumpVol.Substring(0, 2) + " " + pumpVol.Substring(2, 2) + " "
        //    //        + pumpVol.Substring(4, 2) + " " + pumpVol.Substring(6, 2)), 5);
        //    NetCom3.Instance.Send(NetCom3.Cover("EB 90 03 02 04" + pumpVol.Substring(0, 2) + " " + pumpVol.Substring(2, 2) + " "
        //            + pumpVol.Substring(4, 2) + " " + pumpVol.Substring(6, 2)), 5);
        //    NetCom3.Instance.SingleQuery();
        //    fbtnPeristalticPEx.Enabled = true;
        //}
        

        //20190118 YLS add 清洗盘目标孔位移动功能
        private void fbtnHoleTarget_Click_1(object sender, EventArgs e)
        {
            fbtnHoleTarget.Enabled = false;
            if (!NetCom3.totalOrderFlag)
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
                NetCom3.Instance.SingleQuery();
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
        }
        private void fbtnMoveAgingStart_Click(object sender, EventArgs e)
        {
            txtmoveNum.Enabled = false;
            if (!MoveAgingCondition())
            {
                return;
            }            
            txtAgingInfoShow.Clear();
            //washTrayTubeClear();
            //reactTrayTubeClear();
            if (cmbRackPos.Text == "管架A首孔位置")
                CurrentTubePos = 1;
            else if (cmbRackPos.Text == "管架B首孔位置")
                CurrentTubePos = 89;
            else if (cmbRackPos.Text == "管架C首孔位置")
                CurrentTubePos = 177;
            else if (cmbRackPos.Text == "管架D首孔位置")
                CurrentTubePos = 265;
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
            CurrentTubePos = 1;
            CurrentReactPos = 1;
            fbtnMoveAgingStop.Enabled = true;
            fbtnMoveAgingStart.Enabled = fbtnArmAgingStart.Enabled = fbtnArmAgingStop.Enabled
                = fbtnWashAgingStart.Enabled = fbtnWashAgingStop.Enabled = false;
            #endregion
            return true;
        }

        private void fbtnMoveAgingStop_Click(object sender, EventArgs e)
        {
            txtmoveNum.Enabled = true;
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
            CurrentTubePos = 1;
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
            ControlIntit();
        }

        private void fbtnWashAgingStart_Click(object sender, EventArgs e)
        {
            if (!WashAgingCondition())
            {
                return;
            }
            txtAgingInfoShow.Clear();
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
            CurrentTubePos = 1;
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
            ControlIntit();
        }
        
        /// <summary>
        /// 老化测试运行
        /// </summary>
        void TestRun()
        {
            if (fbtnMoveAgingStop.Enabled)
            {
                #region 移管手
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
                int i = CurrentTubePos;
                //lyq add 20190826
                int surplusNum = testNum;
                BeginInvoke(new Action(() =>
                {
                    textSurplusNum.Text = surplusNum.ToString();
                }));
                if (rdbRackIN.Checked)
                {
                    #region 管架与温育盘之间
                    reactTrayTubeClear();//2018-09-26
                    BeginInvoke(new Action(() =>
                    {
                        txtAgingInfoShow.AppendText("-----移管手老化测试开始，管架与温育盘之间移管测试，测试次数：" + testNum.ToString() + "-----" + Environment.NewLine);
                    }));
                    if (testNum <= 80)
                    {
                        SumNum = 1;
                    }
                    else
                    {
                        SumNum = testNum % 80 == 0 ? testNum / 80 : testNum / 80 + 1;
                        singleLooPNum = 80;

                    }
                    for (int j = 0; j < SumNum; j++)
                    {
                        if (CurrentTubePos == 353)
                        {
                            CurrentTubePos = 1;
                        }
                        TubetempNum = 1;
                        CurrentReactPos = 1;
                        while (TubetempNum <= singleLooPNum)
                        {
                            #region 管架夹管到温育盘
                            int plate = CurrentTubePos % 88 == 0 ? CurrentTubePos / 88 - 1 : CurrentTubePos / 88;//几号板
                            int column = CurrentTubePos % 11 == 0 ? CurrentTubePos / 11 - (plate * 8) : CurrentTubePos / 11 + 1 - (plate * 8);
                            int hole = CurrentTubePos % 11 == 0 ? 11 : CurrentTubePos % 11;

                            BeginInvoke(new Action(() =>
                            {
                                txtAgingInfoShow.AppendText("移管手夹管架" + (plate + 1).ToString() + "号板，第" + column.ToString() + "列,第"
                                    + hole.ToString() + "孔的管到温育盘" + CurrentReactPos.ToString() + "位置" + Environment.NewLine);
                            }));
                            //管架取管放到温育盘
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 01 " + plate.ToString("x2") + " " + column.ToString("x2")
                                       + " " + hole.ToString("x2") + " " + CurrentReactPos.ToString("x2")), 1);
                            if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
                            {
                                ControlIntit();
                                return;
                            }
                            #region 取放管成功
                            //移管手要夹的下一个管架位置
                            CurrentTubePos = CurrentTubePos + 1 == 353 ? 1 : CurrentTubePos + 1;
                            CurrentReactPos++;
                            #endregion
                            #endregion
                            TubetempNum++;
                        }
                        TubetempNum = 1;
                        CurrentTubePos = CurrentTubePos - singleLooPNum < 0 ? CurrentTubePos - singleLooPNum + 352 : CurrentTubePos - singleLooPNum;

                        CurrentReactPos = 1;
                        while (TubetempNum <= singleLooPNum)
                        {
                            #region 温育盘夹回到管架
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

                            #region 取放管成功
                            //移管手要夹的下一个管架位置
                            CurrentTubePos = CurrentTubePos + 1 == 353 ? 1 : CurrentTubePos + 1;
                            CurrentReactPos++;
                            if (CurrentReactPos == ReactTrayHoleNum + 1)
                            {
                                CurrentReactPos = 1;
                            }
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
                        //下一次循环次数
                        singleLooPNum = SumNum - (j + 2) == 0 ? testNum - (j + 1) * 80 : 80;
                    }
                    #endregion
                }
                else if (rdbRackWash.Checked)
                {
                    #region 管架与清洗盘之间
                    washTrayTubeClear();//2018-09-26
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
                        if (CurrentTubePos == 353)
                        {
                            CurrentTubePos = 1;
                        }
                        TubetempNum = 1;
                        while (TubetempNum <= singleLooPNum)
                        {
                            #region 管架夹管到清洗盘
                            int plate = CurrentTubePos % 88 == 0 ? CurrentTubePos / 88 - 1 : CurrentTubePos / 88;//几号板
                            int column = CurrentTubePos % 11 == 0 ? CurrentTubePos / 11 - (plate * 8) : CurrentTubePos / 11 + 1 - (plate * 8);
                            int hole = CurrentTubePos % 11 == 0 ? 11 : CurrentTubePos % 11;

                            BeginInvoke(new Action(() =>
                            {
                                txtAgingInfoShow.AppendText(TubetempNum.ToString() + ":移管手夹管架" + (plate + 1).ToString() + "号板，第" + column.ToString() + "列,第"
                                    + hole.ToString() + "孔的管到清洗盘" + Environment.NewLine);
                            }));
                            //管架取管放到清洗盘
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06 " + plate.ToString("x2") + " " + column.ToString("x2")
                                       + " " + hole.ToString("x2")), 1);
                            if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
                            {
                                ControlIntit();
                                return;
                            }
                            //移管手要夹的下一个管架位置
                            CurrentTubePos = CurrentTubePos + 1 == 353 ? 1 : CurrentTubePos + 1;
                            if (TubetempNum != singleLooPNum)
                            {
                                //清洗盘逆时针旋转一位
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                                if (!NetCom3.Instance.WashQuery())
                                {
                                    ControlIntit();
                                    return;
                                }
                            }
                            #endregion
                            TubetempNum++;
                        }
                        TubetempNum = 1;
                        CurrentTubePos = CurrentTubePos - singleLooPNum < 0 ? CurrentTubePos - singleLooPNum + 352 : CurrentTubePos - singleLooPNum;
                        while (TubetempNum <= singleLooPNum)
                        {
                            #region 清洗盘夹管到管架
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
                else if (rdbtnWashIn.Checked)
                {
                    #region 清洗盘与温育盘之间取放管测试
                    //2018-09-26
                    washTrayTubeClear();
                    reactTrayTubeClear();
                    BeginInvoke(new Action(() =>
                    {
                        txtAgingInfoShow.AppendText("-----移管手老化测试开始，温育盘与清洗盘之间移管测试，测试次数：" + testNum.ToString() + "-----" + Environment.NewLine);
                    }));

                    //lyq add 20191010 
                    Random ra = new Random();
                    int random;
                    int[] reactTray = new int[80];
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
                        int plate = CurrentTubePos % 88 == 0 ? CurrentTubePos / 88 - 1 : CurrentTubePos / 88;//几号板
                        int column = CurrentTubePos % 11 == 0 ? CurrentTubePos / 11 - (plate * 8) : CurrentTubePos / 11 + 1 - (plate * 8);
                        int hole = CurrentTubePos % 11 == 0 ? 11 : CurrentTubePos % 11;

                        BeginInvoke(new Action(() =>
                        {
                            txtAgingInfoShow.AppendText(TubetempNum.ToString() + ":移管手夹管架" + (plate + 1).ToString() + "号板，第" + column.ToString() + "列,第"
                                + hole.ToString() + "孔的管到清洗盘" + Environment.NewLine);
                        }));
                        //管架取管放到清洗盘
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06 " + plate.ToString("x2") + " " + column.ToString("x2")
                                   + " " + hole.ToString("x2")), 1);
                        if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
                        {
                            ControlIntit();
                            return;
                        }
                        //移管手要夹的下一个管架位置
                        CurrentTubePos = CurrentTubePos + 1 == 353 ? 1 : CurrentTubePos + 1;
                        if (TubetempNum != singleLooPNum)
                        {
                            //清洗盘逆时针旋转一位
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
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
                        CurrentReactPos = CurrentReactPos == 81 ? 1 : CurrentReactPos;
                        while (TubetempNum <= singleLooPNum)
                        {
                            #region 清洗盘夹管到温育盘

                            //lyq add 20191010
                            random = ra.Next(1, 81);
                            while (reactTray[random - 1] != 0)
                            {
                                random = ra.Next(1, 81);
                            }
                            CurrentReactPos = random;

                            BeginInvoke(new Action(() =>
                            {
                                txtAgingInfoShow.AppendText(TubetempNum.ToString() + ":移管手从清洗盘夹管到温育盘" + CurrentReactPos.ToString() + "位置。" + Environment.NewLine);
                            }));
                            //清洗盘夹管到温育盘
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 03 " + CurrentReactPos.ToString("x2") + " 02"), 1);
                            if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
                            {
                                ControlIntit();
                                return;
                            }

                            reactTray[random - 1] = 1; //lyq 191010

                            CurrentReactPos = CurrentReactPos + 1 == 81 ? 1 : CurrentReactPos + 1;
                            if (TubetempNum != singleLooPNum)
                            {
                                //清洗盘顺时针旋转一位
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-1).ToString("x2").Substring(6, 2)), 2);
                                if (!NetCom3.Instance.WashQuery())
                                {
                                    ControlIntit();
                                    return;
                                }
                            }
                            #endregion
                            TubetempNum++;
                        }
                        CurrentReactPos = CurrentReactPos - singleLooPNum < 0 ? 80 + (CurrentReactPos - singleLooPNum) : CurrentReactPos - singleLooPNum;
                        TubetempNum = 1;
                        while (TubetempNum <= singleLooPNum)
                        {
                            #region 温育盘夹管到清洗盘

                            //lyq 20191010
                            random = ra.Next(1, 81);
                            while (reactTray[random - 1] != 1)
                            {
                                random = ra.Next(1, 81);
                            }
                            CurrentReactPos = random;

                            BeginInvoke(new Action(() =>
                            {
                                txtAgingInfoShow.AppendText(TubetempNum.ToString() + ":移管手从温育盘" + CurrentReactPos.ToString() + "位置夹管到清洗盘" + Environment.NewLine);
                            }));
                            //温育盘夹管到清洗盘
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 02 " + CurrentReactPos.ToString("x2")), 1);
                            if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
                            {
                                ControlIntit();
                                return;
                            }

                            reactTray[random - 1] = 0; //lyq 191010

                            CurrentReactPos = CurrentReactPos + 1 == 81 ? 1 : CurrentReactPos + 1;
                            if (TubetempNum != singleLooPNum)
                            {
                                //清洗盘逆时时针旋转一位
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                                if (!NetCom3.Instance.WashQuery())
                                {
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
                if (testNum < 80)
                {
                    sumNum = testNum;
                }
                else
                {
                    sumNum = 80;
                }
                BeginInvoke(new Action(() =>
                {
                    txtAgingInfoShow.AppendText("-----加样臂老化测试开始，测试次数：" + testNum.ToString() + "-----" + Environment.NewLine);
                }));

                while (ArmtempNum <= testNum)
                {
                    if (CurrentTubePos <= sumNum && chbArmIsTube.Checked)
                    {
                        BeginInvoke(new Action(() =>
                        {
                            txtAgingInfoShow.AppendText("管架夹管到温育盘" + CurrentReactPos.ToString() + "位置。" + Environment.NewLine);
                        }));
                        int plate = CurrentTubePos % 88 == 0 ? CurrentTubePos / 88 - 1 : CurrentTubePos / 88;//几号板
                        int column = CurrentTubePos % 11 == 0 ? CurrentTubePos / 11 - (plate * 8) : CurrentTubePos / 11 + 1 - (plate * 8);
                        int hole = CurrentTubePos % 11 == 0 ? 11 : CurrentTubePos % 11;
                        //管架取管放到温育盘
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 01 " + plate.ToString("x2") + " " + column.ToString("x2")
                                   + " " + hole.ToString("x2") + " " + CurrentReactPos.ToString("x2")), 1);
                        if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
                        {
                            ControlIntit();
                            return;
                        }
                        #region 取放管成功
                        //移管手要夹的下一个管架位置
                        CurrentTubePos = CurrentTubePos + 1;
                        #endregion
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
                        ControlIntit();
                        return;
                    }
                    #endregion

                    #region  加稀释液
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
                        ControlIntit();
                        return;
                    }
                    #endregion

                    CurrentReactPos = CurrentReactPos + 1 == 81 ? 1 : CurrentReactPos + 1;
                    CurrentAsPos = CurrentAsPos + 1 == 61 ? 1 : CurrentAsPos + 1;
                    ArmtempNum++;
                }

                #region 清空温育盘反应管到管架
                BeginInvoke(new Action(() =>
                {

                    txtAgingInfoShow.AppendText("将反应管夹回管架！" + Environment.NewLine);
                }));

                if (chbArmIsTube.Checked)
                {
                    CurrentTubePos = 1;
                    CurrentReactPos = 1;
                    for (int i = 0; i < sumNum; i++)
                    {
                        int plate = CurrentTubePos % 88 == 0 ? CurrentTubePos / 88 - 1 : CurrentTubePos / 88;//几号板
                        int column = CurrentTubePos % 11 == 0 ? CurrentTubePos / 11 - (plate * 8) : CurrentTubePos / 11 + 1 - (plate * 8);
                        int hole = CurrentTubePos % 11 == 0 ? 11 : CurrentTubePos % 11;
                        //温育盘夹管到管架
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 09 " + plate.ToString("x2") + " " + column.ToString("x2")
                                   + " " + hole.ToString("x2") + " " + CurrentReactPos.ToString("x2")), 1);
                        if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
                        {
                            ControlIntit();
                            return;
                        }

                        #region 取放管成功
                        //移管手要夹的下一个管架位置
                        CurrentTubePos++;
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
                            txtAgingInfoShow.AppendText((i+1).ToString()+": 夹新管到清洗盘。"+ Environment.NewLine);
                        }));
                        int plate = CurrentTubePos % 88 == 0 ? CurrentTubePos / 88 - 1 : CurrentTubePos / 88;//几号板
                        int column = CurrentTubePos % 11 == 0 ? CurrentTubePos / 11 - (plate * 8) : CurrentTubePos / 11 + 1 - (plate * 8);
                        int hole = CurrentTubePos % 11 == 0 ? 11 : CurrentTubePos % 11;
                        //管架取管放到清洗盘
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06 " + plate.ToString("x2") + " " + column.ToString("x2")
                                   + " " + hole.ToString("x2")), 1);
                        if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
                        {
                            ControlIntit();
                            return;
                        }
                        //移管手要夹的下一个管架位置
                        CurrentTubePos++;
                        if (i != 29)
                        {
                            //清洗盘逆时针旋转一位
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
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
                        ControlIntit();
                        return;
                    }
                    WashtempNum++;

                }

                #region 清洗盘夹回管架
                if (chbWashIsTube.Checked)
                {
                    CurrentTubePos = 1;
                    for (int i = 0; i < 30; i++)
                    {
                        BeginInvoke(new Action(() =>
                        {
                            txtAgingInfoShow.AppendText((i + 1).ToString() + ": 清洗盘夹回管架" + Environment.NewLine);
                        }));
                        int plate = CurrentTubePos % 88 == 0 ? CurrentTubePos / 88 - 1 : CurrentTubePos / 88;//几号板
                        int column = CurrentTubePos % 11 == 0 ? CurrentTubePos / 11 - (plate * 8) : CurrentTubePos / 11 + 1 - (plate * 8);
                        int hole = CurrentTubePos % 11 == 0 ? 11 : CurrentTubePos % 11;
                        //管架取管放到清洗盘
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 0a " + plate.ToString("x2") + " " + column.ToString("x2")
                                   + " " + hole.ToString("x2")), 1);
                        if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
                        {
                            ControlIntit();
                            return;
                        }
                        //移管手要夹的下一个管架位置
                        CurrentTubePos++;
                        if (i != 29)
                        {
                            //清洗盘逆时针旋转一位
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
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
                txtmoveNum.Enabled = true;
            }));            
            ControlIntit();
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
                        case"加热打开":
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 04 00"), 5);
                            break;
                        case"加热关闭":
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 04 01"), 5);
                            break;
                        case"设置校准值":
                            if (txtStandard.Text == "")
                            {
                                frmMsgShow.MessageShow("仪器调试", "请录入要校准的温度值！");
                                return;
                            }
                            //string jzwendu = (float.Parse(txtStandard.Text)).ToString("x4");
                            string jzwendu = NetCom3.FloatToHex(float.Parse(txtStandard.Text));
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 04 03 " + jzwendu), 5);
                            break;
                        default :
                            break;
                    }
                    break;
                case"清洗盘":
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
                case"清洗管路":
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
                case"底物管路":
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
                default :
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
            else if(rbtnWainOpen.Checked )
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
            else if (cmbStep.SelectedItem != "查询校准值" && cmbStep.SelectedItem != "查询温度")
            {
                frmMsgShow.MessageShow("仪器调试", "选择查询的温控模块有误，请重新选择！");
                return;
            }
            btnSelect.Enabled = false;
            switch (cmbModelName.SelectedItem.ToString() )
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
            }
            NetCom3.Instance.SingleQuery();
            btnSelect.Enabled = true;
            //ReadThread.Abort();
        }

        private void chkQXGL_CheckedChanged(object sender, EventArgs e)
        {
            chart1.Series[5].Enabled = chkQXGL.Checked;
        }

        //增加清洗盘转动按钮的点击事件，jun add，2019/2/18
        private void functionButton1_Click_1(object sender, EventArgs e)
        {
            string holetargethex = "";
            functionButton1.Enabled = false;
            if (!NetCom3.totalOrderFlag)
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
                    NetCom3.Instance.SingleQuery();
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
            catch(Exception exp)
            {
                frmMsgShow.MessageShow("信息提示","请输入数字！");
            }
            finally 
            {
                functionButton1.Enabled = true;
            }
           
        }

        private void fbtnMixInArm_Click(object sender, EventArgs e)
        {
            //增加一个位号温育混匀 jun add 20190410
            if (!NetCom3.totalOrderFlag)
            {
                frmMessageShow frmMsg = new frmMessageShow();
                frmMsg.MessageShow("仪器调试", "仪器正在运动，请稍等！");
                return;
            }
            fbtnMixInArm.Enabled = false;
            try
            {
                int inNum = int.Parse(mixInNum.Text);
                if (inNum > 80) 
                {
                    frmMessageShow frmMsg = new frmMessageShow();
                    frmMsg.MessageShow("温馨提示", "请输入正确位号！");
                    mixInNum.Focus();
                    fbtnMixInArm.Enabled = true;
                    return;
                }
            }
            catch
            {
                frmMessageShow frmMsg = new frmMessageShow();
                frmMsg.MessageShow("温馨提示", "请输入正确位号！");
                mixInNum.Focus();
                fbtnMixInArm.Enabled = true;
                return;
            }
            string order = "EB 90 31 01 0B " + int.Parse(mixInNum.Text).ToString("x2")+" ";
            NetCom3.Instance.Send(NetCom3.Cover(order), 1);
            NetCom3.Instance.SingleQuery();

            fbtnMixInArm.Enabled = true;
        }


       //2019.5.27  hly add
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
                    //    int delay=1000;
                    //    while (!BackObj.Contains("EB 90 31 A3") && delay>0)
                    //    {
                    //        NetCom3.Delay(10);
                    //        delay = delay-10;
                    //    }
                    //    if (BackObj.Contains("EB 90 31 A3"))
                    //    {
                    //        string temp = BackObj.Substring(BackObj.Length - 16).Replace(" ", "");
                    //        temp = Convert.ToInt64(temp, 16).ToString();
                    //        if (double.Parse(temp) >Math.Pow(10, 5))
                    //            temp = ((int)GetPMT(double.Parse(temp))).ToString();
                    //        textReadShow.AppendText(DateTime.Now.ToString("HH-mm-ss") + ": " + "PMT背景值：" + temp + Environment.NewLine);
                    //    }
                    //}
            }
            Thread.Sleep(500);
            NetCom3.Instance.ReceiveHandel -= GetReadNum2;
            TExtAppend("已完成" + Environment.NewLine);
            //textReadShow.AppendText("已完成"+ Environment.NewLine);
            btnReadNum.Enabled = true;
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
                frmMsgShow.MessageShow("仪器调试", "请选择混匀电机动作！");
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

        private void FunctionButton2_Click(object sender, EventArgs e)
        {
            functionButton2.ForeColor = Color.Red;
            functionButton2.Enabled = false;
            Thread.Sleep(500);
            functionButton2.ForeColor = Color.Black;
        }

        private void LButton1_Click(object sender, EventArgs e)
        {
            if (textStepOrder.Text.Trim() == "")       //控件名
            {
                MessageBox.Show("请输入通讯命令！");
                return;
            }
            btnOrderSend.Enabled = false;

            string order = textStepOrder.Text.ToString();

            //NetCom3.Instance.Send(NetCom3.Cover(order), 5);  //发送套接字

            if (order.Contains("EB 90 31 01"))
            {
                NetCom3.Instance.Send(NetCom3.Cover(order), 1);
                NetCom3.Instance.MoveQuery();
            }
            else if (order.Contains("EB 90 31 02") || order.Contains("EB 90 31 04"))
            {
                NetCom3.Instance.Send(NetCom3.Cover(order), 0);
                NetCom3.Instance.SPQuery();
            }
            else if (order.Contains("EB 90 31 03"))
            {
                NetCom3.Instance.Send(NetCom3.Cover(order), 2);
                NetCom3.Instance.WashQuery();
            }
            else
            {
                NetCom3.Instance.Send(NetCom3.Cover(order), 5);
                NetCom3.Instance.SingleQuery();
            }

            btnOrderSend.Enabled = true;   //返回套接字后按钮可用

            LogFile.Instance.Write(DateTime.Now.ToString("HH-mm-ss"));
        }

        //2019.5.27  hly  add
        private void btnZx_Click(object sender, EventArgs e)
        {
            int pos = int.Parse(numXz.Text);
            if (pos > 0)
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (pos).ToString("X2")), 2);
            }
            else
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (0 + pos).ToString("X2").Substring(6,2)), 2);
            }
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
            if (cmbPeristalticPVol.SelectedItem.ToString() == "1")
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 0B 00 01"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbPeristalticPVol.SelectedItem.ToString() == "2")
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 0B 00 02"), 5);
                NetCom3.Instance.SingleQuery();
            }
            else if (cmbPeristalticPVol.SelectedItem.ToString() == "3")
            {
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 0B 00 03"), 5);
                NetCom3.Instance.SingleQuery();
            }
            fbtnPeristalticPEx.Enabled = true;
        }

        private void CmbTestName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTestName.SelectedItem.ToString() == "加样准确性测试")
            {
                label87.Visible = untxtReadNum.Visible = false;

                label84.Visible = label85.Visible = untxtSamplePos.Visible = untxtSampleVol.Visible = true;
            }
            else if (cmbTestName.SelectedItem.ToString() == "酶活性测试")
            {
                label84.Visible = label85.Visible = untxtSamplePos.Visible = untxtSampleVol.Visible = false;

                label87.Visible = untxtReadNum.Visible = true;
            }
            else if (cmbTestName.SelectedItem.ToString() == "清洗效果测试")
            {

                label84.Visible = label85.Visible = untxtSamplePos.Visible = untxtSampleVol.Visible = false;
                label87.Visible = untxtReadNum.Visible = true;
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

        private void CbChange_CheckedChanged(object sender, EventArgs e)
        {
            if (cbChange.Checked)
            {
                untxtSampleVol.Enabled = true;
                untxtSamplePos.Enabled = true;
                untxtReadNum.Enabled = true;

                txtTubeNum.Enabled = true;
                cmbRackStart.Enabled = true;
                txtColumnStart.Enabled = true;
                txtHoleStart.Enabled = true;

            }
            else
            {
                untxtSampleVol.Enabled = false;
                untxtSamplePos.Enabled = false;
                untxtReadNum.Enabled = false;

                txtTubeNum.Enabled = false;
                cmbRackStart.Enabled = false;
                txtColumnStart.Enabled = false;
                txtHoleStart.Enabled = false;
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
            if (untxtSamplePos.Text == "" || untxtSampleVol.Text == "")
            {
                frmMsgShow.MessageShow("仪器调试", "存在输入框为空，请重新输入！");
                return;
            }
            if (txtTubeNum.Text == "" || cmbRackStart.SelectedIndex < 0 || txtColumnStart.Text == "" || txtHoleStart.Text == "" || untxtReadNum.Text == "")
            {
                frmMsgShow.MessageShow("仪器调试", "存在输入框为空，请重新输入！");
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
            //开始初始化管架位置变量
            //板(0x00-0x03) 4
            int plate = 0;
            Invoke(new Action(() =>
            {
                plate = cmbRackStart.SelectedIndex;
            }));
            //列(0x01-0x08) 8
            int column = int.Parse(txtColumnStart.Text);
            //孔(0x01-0x0b) 11
            int hole = int.Parse(txtHoleStart.Text);
            //测试次数
            int repeat = int.Parse(txtTubeNum.Text);
            //温育盘开始放管孔位
            int reactTrayPos = 1;
            //管架管号
            int rackTubePos = plate * 88 + (column - 1) * 11 + hole;
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
                frmMsgShow.MessageShow("仪器调试", "请确认在 " + (plate + 1) + " 号管架 第 " + column + " 列的 " + hole + " 号位后是否依次放好 " + repeat + " 个反应管，点击确定马上开始测试。");

                //从管架移管到温育盘
                reactTrayPos = 1;
                int tempRackTubePos = rackTubePos;
                int tempPlate = plate;
                int tempColumn = column;
                int tempHole = hole;
                for (int i = 0; i < repeat; i++)
                {
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 01 " + tempPlate.ToString("X2") + " " + tempColumn.ToString("X2")
                        + " " + tempHole.ToString("X2") + " " + reactTrayPos.ToString("X2")), 1); //aa bb cc dd//板 列 孔 温育盘位置
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        goto enzymaticActivityTestEnd;
                    }
                    reactTrayPos++;//下一个温育盘放管位置
                    tempRackTubePos++;//管架下一个位置
                    tempPlate = tempRackTubePos % 88 == 0 ? tempRackTubePos / 88 - 1 : tempRackTubePos / 88; //板
                    tempColumn = tempRackTubePos % 11 == 0 ? tempRackTubePos / 11 - (tempPlate * 8) : tempRackTubePos / 11 + 1 - (tempPlate * 8); // 列
                    tempHole = tempRackTubePos % 11 == 0 ? 11 : tempRackTubePos % 11; //孔
                }
                //循环加液
                for (int i = 0; i < repeat; i++)
                {
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 01 " + int.Parse(samplePos).ToString("X2") + " " + (i + 1).ToString("X2")
                                 + " " + sampleVol.ToString("X2")), 0); //aa bb cc //样本盘孔号、温育盘盘孔号、样本体积
                    if (!NetCom3.Instance.SPQuery())
                    {
                        MessageBox.Show(NetCom3.Instance.AdderrorFlag.ToString());
                        goto enzymaticActivityTestEnd;
                    }
                    NetCom3.Delay(300);
                }
                //从温育盘夹管到管架
                reactTrayPos = 1;
                tempRackTubePos = rackTubePos;
                tempPlate = plate;
                tempColumn = column;
                tempHole = hole;
                for (int i = 0; i < repeat; i++)
                {
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 09 " + tempPlate.ToString("X2") + " " + tempColumn.ToString("X2")
                        + " " + tempHole.ToString("X2") + " " + reactTrayPos.ToString("X2")), 1); //aa bb cc dd//板 列 孔 温育盘位置
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        goto enzymaticActivityTestEnd;
                    }
                    reactTrayPos++;//下一个温育盘放管位置
                    tempRackTubePos++;//管架下一个位置
                    tempPlate = tempRackTubePos % 88 == 0 ? tempRackTubePos / 88 - 1 : tempRackTubePos / 88; //板
                    tempColumn = tempRackTubePos % 11 == 0 ? tempRackTubePos / 11 - (tempPlate * 8) : tempRackTubePos / 11 + 1 - (tempPlate * 8); // 列
                    tempHole = tempRackTubePos % 11 == 0 ? 11 : tempRackTubePos % 11; //孔
                }
                NetCom3.Delay(1000);
            }
            //酶活性测试
            else if (testIndex == 1)
            {
                if (repeat > 10)
                {
                    frmMsgShow.MessageShow("仪器调试", "目前单次最多只允许10个管进行测试，请重新输入！");
                    goto enzymaticActivityTestEnd;
                }

                frmMsgShow.MessageShow("仪器调试", "请确认在 " + (plate + 1) + " 号管架 第 " + column + " 列的 " + hole + " 号位后是否依次放好 " + repeat + " 个反应管，点击确定马上开始测试。");

                Invoke(new Action(() =>
                {
                    textBox3.AppendText(DateTime.Now.ToString("HH-mm-ss") + ": 测试开始。" + Environment.NewLine);
                }));

                //从管架到清洗盘
                for (int i = 0; i < repeat; i++)
                {
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06 " + plate.ToString("X2") + " " + column.ToString("X2")
                        + " " + hole.ToString("X2")), 1); // aa bb cc（板列孔）
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        goto enzymaticActivityTestEnd;
                    }
                    rackTubePos++;
                    plate = rackTubePos % 88 == 0 ? rackTubePos / 88 - 1 : rackTubePos / 88;//板
                    column = rackTubePos % 11 == 0 ? rackTubePos / 11 - (plate * 8) : rackTubePos / 11 + 1 - (plate * 8);//列
                    hole = rackTubePos % 11 == 0 ? 11 : rackTubePos % 11;//孔

                    //清洗盘顺时针旋转一位
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-1).ToString("X2").Substring(6, 2)), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        goto enzymaticActivityTestEnd;
                    }
                }

                //循环加底物读数
                int turnToSub = 20 - (31 - repeat); //清洗盘20加底物
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + turnToSub.ToString("X2").Substring(6, 2)), 2); //顺时针转到20
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
                frmMsgShow.MessageShow("仪器调试", "请确认在 " + (plate + 1) + " 号管架 第 " + column + " 列的 " + hole + " 号位后是否依次放好 " + repeat + " 个反应管，点击确定马上开始测试。");
                BeginInvoke(new Action(() =>
                {
                    textBox3.AppendText(DateTime.Now.ToString("HH-mm-ss") + ": 测试开始。" + Environment.NewLine);
                }));

                int num = 0; //第num个管在读数
                int[] washPos = new int[31]; //清洗盘放管位置计数 0为执行结果标志位
                int cycleTimes = repeat % 5 == 0 ? (repeat / 5) : (repeat / 5 + 1); //清洗盘循环一次5个管 ，清洗盘一共循环几次
                int oneTimes; //第一次循环几个管

                for (int i = 0; i < cycleTimes; i++)
                {
                    oneTimes = repeat / 5 == 0 ? repeat : 5; //第一次循环几个管

                    int j = 0;
                    //越过第一根抽液针 的开始动作
                    for (j = 0; j < oneTimes; j++)
                    {
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06 " + plate.ToString("X2") + " " + column.ToString("X2")
                       + " " + hole.ToString("X2")), 1); // aa bb cc（板列孔）
                        if (!NetCom3.Instance.MoveQuery())
                        {
                            goto enzymaticActivityTestEnd;
                        }
                        rackTubePos++;
                        plate = rackTubePos % 88 == 0 ? rackTubePos / 88 - 1 : rackTubePos / 88;//板
                        column = rackTubePos % 11 == 0 ? rackTubePos / 11 - (plate * 8) : rackTubePos / 11 + 1 - (plate * 8);//列
                        hole = rackTubePos % 11 == 0 ? 11 : rackTubePos % 11;//孔

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
                        //while (DateTime.Now.Subtract(dtimeWash).TotalMilliseconds < 20000) //20s一格
                        //    NetCom3.Delay(100);
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

        private void GetReadNum2(string order)
        {
            if (order.Contains("EB 90 31 A3"))
            {
                string temp = order.Replace(" ", "");
                int pos = temp.IndexOf("EB9031A3");
                temp = temp.Substring(pos, 32);
                temp = temp.Substring(temp.Length - 8);
                temp = Convert.ToInt64(temp, 16).ToString();
                if (double.Parse(temp) > Math.Pow(10, 5))
                    temp = ((int)GetPMT(double.Parse(temp))).ToString();
                TExtAppend(DateTime.Now.ToString("HH-mm-ss") + ": " + "PMT背景值：" + temp);
            }
        }

        void TExtAppend(string text)
        {
            while (!this.IsHandleCreated)
            {
            }
            textReadShow.Invoke(new Action(() => { textReadShow.AppendText(Environment.NewLine + text); }));
        }
    }
}
