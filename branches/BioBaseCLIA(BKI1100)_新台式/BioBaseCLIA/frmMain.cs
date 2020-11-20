using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common;
using Maticsoft.DBUtility;
using System.IO;
using System.Threading;
using BioBaseCLIA.Run;
using BioBaseCLIA.DataQuery;
using BioBaseCLIA.InfoSetting;
using BioBaseCLIA.ScalingQC;
using BioBaseCLIA.SysMaintenance;
using System.Runtime.InteropServices;
using System.Timers;
using BioBaseCLIA.User;
using System.Reflection;

namespace BioBaseCLIA
{

    public partial class frmMain : Form
    {
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
        Autosize autosize = new Autosize();
        private float X;//宽度 
        private float Y;//高度
        System.Windows.Forms.Timer time1 = new System.Windows.Forms.Timer();//实时时间显示
        public frmMessageShow frmMsgShow = new frmMessageShow();//提示框
        /// <summary>
        /// 缺液信息状态
        /// </summary>
        public static int[] LackLq;//2018-07-12 zlx mod
        /// <summary>
        /// 缺管信息状态 缺管：0 有管：1
        /// </summary>
        public static int LackTube = -1;
        #region 状态变量错误值
        /// <summary>
        /// 底物警告最小值
        /// </summary>
        int WarnSubstrate = int.Parse(OperateIniFile.ReadInIPara("Limit", "WarnSubstrate"));
        /// <summary>
        /// 底物已经使用完
        /// </summary>
        int ErrorSubstrate = int.Parse(OperateIniFile.ReadInIPara("Limit", "ErrorSubstrate"));
        /// <summary
        /// 试剂警告最小值
        /// </summary>
        int WarnReagent = int.Parse(OperateIniFile.ReadInIPara("Limit", "WarnReagent"));
        /// <summary>
        /// 试剂已经使用完
        /// </summary>
        int ErrorReagent = int.Parse(OperateIniFile.ReadInIPara("Limit", "ErrorReagent"));
        /// <summary>
        /// 管架反应管警告最小值
        /// </summary>
        int WarnTube = int.Parse(OperateIniFile.ReadInIPara("Limit", "WarnTube"));
        /// <summary>
        /// 管架反应管已经使用完
        /// </summary>
        int ErrorTube = int.Parse(OperateIniFile.ReadInIPara("Limit", "ErrorTube"));
        #endregion
        /// <summary>
        /// 点击开始事件
        /// </summary>
        public static event Action<object, EventArgs> btnRunClick;
        /// <summary>
        /// 点击暂停事件
        /// </summary>
        public static event Action btnPauseClick;
        /// <summary>
        /// 点击停止事件
        /// </summary>
        public static event Action btnStopClick;

        /// <summary>
        /// 点击继续事件
        /// </summary>
        public static event Action btnGoonClick;
        /// <summary>
        /// 暂停标志位
        /// </summary>
        public static bool pauseFlag = false;
        /// <summary>
        /// 试剂盘配置文件地址
        /// </summary>
        string iniPathReagentTrayInfo = Directory.GetCurrentDirectory() + "\\ReagentTrayInfo.ini";
        /// <summary>
        /// 底物与管架配置文件地址
        /// </summary>
        string iniPathSubstrateTube = Directory.GetCurrentDirectory() + "\\SubstrateTube.ini";
        /// <summary>
        /// 桶液位是否查询 标志位
        /// </summary>
        public static bool LiquidQueryFlag = true;
        //2018-07-05 zlx add  缺液3分钟暂停加样
        private const int MaxBuffertime = 9;//磁珠清洗液报警最大次数
        private const int MaxWashtime = 9;//探针清洗液报警最大次数
        private const int MaxWastetime = 9;//废液报警最大次数
        private const int MaxWTubetime = 9;//废管报警最大次数
        private int NumWTubettime = 0;//报警指令连续发送废管满的次数
        ////2018-07-11 zlx add
        //string BackObj = "";
        Thread QueryThread;
        /// <summary>
        /// 查询温度
        /// 温育盘，清洗盘，清洗管路，底物管路 2018-07-14
        /// </summary>
        decimal[] Temprrature = new decimal[4];
        /// <summary>
        /// 温育盘温度范围
        /// </summary>
        public decimal[] RangeWY = { 0, 0 };
        /// <summary>
        /// 清洗盘温度范围
        /// </summary>
        public decimal[] RangeWash = { 0, 0 };
        /// <summary>
        /// 底物温度范围
        /// </summary>
        public decimal[] RangeSubstrate = { 0, 0 };
        /// <summary>
        /// 清洗管路温度范围
        /// </summary>
        public decimal[] RangeQXGL = { 0, 0 };
        /// <summary>
        /// 查询信息列表
        /// </summary>
        List<string> Selectlist;
        /// <summary>
        /// 开机时间 
        /// </summary>
        DateTime _BootUpTime;//2018-07-25 zlx add
        //2018-11-14 zlx add
        IntPtr trayHwnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Shell_TrayWnd", null);
        IntPtr hStar = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Button", null);
        /// <summary>
        /// 声音报警启用
        /// </summary>
        int SoundFlag;
        public static int WarnTime = 0;
        public frmMain()
        {
            InitializeComponent();
            X = this.Width;
            Y = this.Height;
            autosize.setTag(this);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            frmWorkList.LiquidLevelDetectionEvent += LiquidLevelDetectionAlarm;
            SoundFlag = (int)SoundFlagStart.IsOpen;
            _BootUpTime = DateTime.Now;//2018-07-25 zlx add
            label2.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            toolTip1.SetToolTip(this.dbtnBuffer, "磁珠清洗液查询");
            toolTip1.SetToolTip(this.dbtnWash, "探针清洗液查询");
            toolTip1.SetToolTip(this.dbtnWaste, "废液查询");
            toolTip1.SetToolTip(this.dbtnSubstract, "底物信息");
            toolTip1.SetToolTip(this.dbtnRegent, "试剂信息");
            toolTip1.SetToolTip(this.dbtnRack, "理杯机信息");
            toolTip1.SetToolTip(this.btnWasteRack, "废管盒信息");
            toolTip1.SetToolTip(this.dbtnLog, "报警信息");//2018-07-19 zlx mod

            if (NetCom3.isConnect)
            {
                dbtnConnect.Enabled = false;
                //dbtnConnect.BackgroundImage = Properties.Resources.已连接;
                toolTip1.SetToolTip(this.dbtnConnect, "网络已连接");
            }
            else
            {
                dbtnConnect.Enabled = false;
                //dbtnConnect.BackgroundImage = Properties.Resources.未连接;
                toolTip1.SetToolTip(this.dbtnConnect, "网络未连接");
                //2018-09-06 zlx add
                fbtnTest.Enabled = false;
                fbtnMaintenance.Enabled = false;
            }
            //2018-07-13 zlx add
            RangeWY[1] = Convert.ToDecimal(OperateIniFile.ReadInIPara("temperature", "MaxTWY"));
            RangeWY[0] = Convert.ToDecimal(OperateIniFile.ReadInIPara("temperature", "MinTWY"));
            RangeWash[1] = Convert.ToDecimal(OperateIniFile.ReadInIPara("temperature", "MaxTWash"));
            RangeWash[0] = Convert.ToDecimal(OperateIniFile.ReadInIPara("temperature", "MinTWash"));
            RangeSubstrate[1] = Convert.ToDecimal(OperateIniFile.ReadInIPara("temperature", "MaxTSubstrate"));
            RangeSubstrate[0] = Convert.ToDecimal(OperateIniFile.ReadInIPara("temperature", "MinTSubstrate"));
            RangeQXGL[1] = Convert.ToDecimal(OperateIniFile.ReadInIPara("temperature", "MaxTQXGL"));
            RangeQXGL[0] = Convert.ToDecimal(OperateIniFile.ReadInIPara("temperature", "MinTQXGL"));



            //frmWorkList.btnLogColor += new Action<int>(LogBtnColorChange);
            frmLogShow.btnLogColor1 += new Action<int>(LogBtnColorChange);
            #region 查询今天错误中是否有未解决的问题，有的话按钮颜色变色
            List<string> lstFiles = GetFiles(Application.StartupPath + @"\Log\AlarmLog", ".txt");
            foreach (string lstFile in lstFiles)
            {
                if (lstFile.Length > 13 || lstFile.Substring(1, 8) != DateTime.Now.ToString("yyyyMMdd"))
                    continue;
                string fileInfo = ReadTxtWarn.ReaderFile(Application.StartupPath + @"\Log\AlarmLog" + "\\" + lstFile);//all text
                if (fileInfo.IndexOf("未读") > -1)
                {
                    dbtnLog.BackgroundImage = Properties.Resources._11感叹号;
                    break;
                }
            }
            #endregion
            int newx = this.Width;
            int warnControlL_Y = (int)pnlbarUP.Size.Height / 5;
            int controlWidth = 100;
            dbtnLog.Location = new Point((newx - controlWidth - 20), warnControlL_Y);
            dbtnRegent.Location = new Point((newx - controlWidth * 2 - 20), warnControlL_Y);
            dbtnRack.Location = new Point((newx - controlWidth * 3 - 20), warnControlL_Y);
            dbtnSubstract.Location = new Point((newx - controlWidth * 4 - 20), warnControlL_Y);
            btnWasteRack.Location = new Point((newx - controlWidth * 5 - 20), warnControlL_Y);
            dbtnWaste.Location = new Point((newx - controlWidth * 6 - 20), warnControlL_Y);
            dbtnWash.Location = new Point((newx - controlWidth * 7 - 20), warnControlL_Y);
            dbtnBuffer.Location = new Point((newx - controlWidth * 8 - 20), warnControlL_Y);
            temperatureButton.Location = new Point((newx - controlWidth * 9 - 20), warnControlL_Y);
            //logo.Location = new Point(defineButton4.Location.X + defineButton4.Size.Width + 10, logo.Location.Y);//y add 20180426;

            LackLq = new int[] { 0, 0, 0, 0 };//2018-07-12 zlx mod
            frmSupplyStatus.btnBtnColor += new Action<int, int, int>(RegenColorChange);
            //frmSampleLoad.btnEmerPause += new Action<object, EventArgs>(defineButton2_Click);
            //2018-07-11 zlx add
            new Thread(new ParameterizedThreadStart((obj) =>
            {
                NetCom3.Instance.ReceiveHandelForQueryTemperatureAndLiquidLevel += new Action<string>(Instance_ReceiveHandel);//y 20180816 更改注册事件
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
            Selectlist = new List<string>();
            QueryThread = new Thread(new ParameterizedThreadStart(Instance_QueryInfo));
            QueryThread.IsBackground = true;
            QueryThread.Start();
            //2018-07-18 zlx mod
            #region 设置按钮控件查询状态timer的属性
            timerStatus.Enabled = true;
            timerStatus.Interval = 20000;
            timerStatus_Tick(null, null);
            timerStatus.Start();

            #endregion
            //2018-11-14 zlx add
            //ShowWindow(trayHwnd, 0);
            //ShowWindow(hStar, 0);
            timeWarnSound.Start();

            //对换软件需要清空温育盘的修改
            //更新到最近一次在本机器关闭软件时的温育盘配置信息
            if (!Directory.Exists(@"C:\temp"))
            {
                Directory.CreateDirectory(@"C:\temp");
            }
            string defaultIniPath = Directory.GetCurrentDirectory() + "\\ReactTrayInfo.ini";//温育盘ini文件地址
            string cTempIniPath = @"C:\temp\ReactTrayInfo.ini";//放在C盘temp文件夹的临时温育盘ini文件地址
            if (File.Exists(cTempIniPath))
            {
                if (OperateIniFile.ReadConfig(cTempIniPath).Rows.Count != 50)
                {
                    File.Delete(cTempIniPath);
                    frmMsgShow.MessageShow("提示", "检测到非正常退出软件，请清空温育盘！");
                    return;
                }
                //删除debug中的ReactTrayInfo.ini
                File.Delete(defaultIniPath);
                //把temp中的ReactTrayInfo.ini剪切过来
                File.Move(cTempIniPath, defaultIniPath);
            }
            else
            {
                //提醒检测到非正常退出，请清空温育盘
                frmMsgShow.MessageShow("提示", "检测到非正常退出软件，请清空温育盘！");
            }

            timerConnect.Enabled = true;
        }
        /// <summary>
        /// 液位检测报警
        /// </summary>
        /// <param name="alarmContent">报警内容</param>
        /// <param name="colorFlag">报警按钮颜色</param>
        private void LiquidLevelDetectionAlarm(string alarmContent, int colorFlag)
        {
            //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "警告" + " *** " + "未读" + " *** " + alarmContent);
            //LogBtnColorChange(colorFlag);
            //NetCom3.Instance.LiquidLevelDetectionFlag = (int)BioBaseCLIA.LiquidLevelDetectionAlarm.Height;
        }
        object locker = new object();
        /// <summary>
        /// 接收返回指令 2018-07-13
        /// </summary>
        /// <param name="obj"></param>
        void Instance_ReceiveHandel(string obj)
        {
            lock (locker)
            {
                if (obj.IsNullOrEmpty())
                {
                    return;
                }
                else
                {
                    string BackObj = obj;
                    if (BackObj.Contains("EB 90 11 AF"))
                    {
                        IntToBool(BackObj);
                    }
                }
            }
        }
        /// <summary>
        /// 发送查询指令  2018-07-13
        /// </summary>
        /// <param name="TubeInfo"></param>
        void Instance_QueryInfo(object TubeInfo)
        {
            bool iswork = false;
            List<string> list;
            bool diagFlag = false;//lyq add 20191230
            while (true)
            {
                Thread.Sleep(100);
                if (Selectlist.Count > 0 && !iswork && LiquidQueryFlag)//2018-08-13 zlx add
                {
                    iswork = true;
                    //for (int i = 0; i < Selectlist.Count; i++)
                    //{
                    //    string[] li = Selectlist[i];
                    //    list.Add(li);
                    //}
                    list = Selectlist.GetRange(0, Selectlist.Count);
                    Selectlist.Clear();
                    while (list.Count > 0)
                    {
                        while (NetCom3.Instance.iapIsRun) //lyq iap 20191130
                            NetCom3.Delay(1000);
                        Thread.Sleep(50);
                        if (NetCom3.isConnect && list[0] != null && NetCom3.isConnect/* && NetCom3.Instance.FReciveCallBack < 3*/)
                        {
                            //2018-07-25                             
                            if (frmWorkList.RunFlag == (int)RunFlagStart.IsRuning)
                            {
                                if (frmWorkList.BQLiquaid)
                                {
                                    while (!NetCom3.totalOrderFlag)
                                        Thread.Sleep(50);
                                    NetCom3.Instance.Send(NetCom3.Cover(list[0]), 5);
                                    NetCom3.Instance.SingleQuery();
                                }
                            }
                            else
                            {
                                Invoke(new Action(() =>
                                {
                                    if (this.ActiveControl != null && this.ActiveControl.Text == "frmDiagnost")//在调试界面返回时立刻执行这句会出现“线程间操作无效”错误
                                    {
                                        if (!frmDiagnost.BQLiquaid)
                                        {
                                            diagFlag = true;
                                        }
                                    }
                                }));
                                if (diagFlag)
                                {
                                    diagFlag = false;
                                    continue;
                                }
                                //if (this.ActiveControl != null && this.ActiveControl.Text == "frmDiagnost")
                                //{
                                //    if (!frmDiagnost.BQLiquaid)
                                //    {
                                //        continue;
                                //    }
                                //}
                                while (!NetCom3.totalOrderFlag)
                                    Thread.Sleep(50);
                                NetCom3.Instance.Send(NetCom3.Cover(list[0]), 5);
                                NetCom3.Instance.SingleQuery();
                            }
                        }
                        list.Remove(list[0]);
                    }
                    list = null;
                    iswork = false;
                }
            }
        }
        /// <summary>
        /// 更改日志按钮颜色
        /// </summary>
        /// <param name="Flag">标志是警告还是错误，0警告变为红色，1错误为黄色</param>
        public void LogBtnColorChange(int Flag)
        {
            if (Flag == 0)
            {
                dbtnLog.BackgroundImage = Properties.Resources._22感叹号;//红色
            }
            else if (Flag == 2)
            {
                dbtnLog.BackgroundImage = Properties.Resources._33感叹号;//蓝色
            }
            else
            {
                dbtnLog.BackgroundImage = Properties.Resources._11感叹号;//黄色
            }
        }
        /// <summary>
        /// 查询试剂盘中试剂的信息
        /// </summary>
        /// <returns></returns>
        List<ReagentIniInfo> QueryReagentIniInfo()
        {
            List<ReagentIniInfo> lisReagentIniInfo = new List<ReagentIniInfo>();
            ReagentIniInfo reagentIniInfo = new ReagentIniInfo();
            for (int i = 1; i <= 10; i++)
            {
                string ReagentType = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "ReagentType", "", iniPathReagentTrayInfo);
                if (ReagentType == "1") continue;
                reagentIniInfo.BarCode = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "BarCode", "", iniPathReagentTrayInfo);
                reagentIniInfo.ItemName = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "ItemName", "", iniPathReagentTrayInfo);
                reagentIniInfo.TestCount = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "TestCount", "", iniPathReagentTrayInfo);
                string leftR1 = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "LeftReagent1", "", iniPathReagentTrayInfo);
                string leftR2 = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "LeftReagent2", "", iniPathReagentTrayInfo);
                string leftR3 = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "LeftReagent3", "", iniPathReagentTrayInfo);
                string leftR4 = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "LeftReagent4", "", iniPathReagentTrayInfo);
                if (leftR1 == "")
                {
                    reagentIniInfo.LeftReagent1 = 0;
                }
                else
                {
                    reagentIniInfo.LeftReagent1 = int.Parse(leftR1);
                }
                if (leftR2 == "")
                {
                    reagentIniInfo.LeftReagent2 = 0;
                }
                else
                {
                    reagentIniInfo.LeftReagent2 = int.Parse(leftR2);
                }
                if (leftR3 == "")
                {
                    reagentIniInfo.LeftReagent3 = 0;
                }
                else
                {
                    reagentIniInfo.LeftReagent3 = int.Parse(leftR3);
                }
                if (leftR4 == "")
                {
                    reagentIniInfo.LeftReagent4 = 0;
                }
                else
                {
                    reagentIniInfo.LeftReagent4 = int.Parse(leftR4);
                }
                reagentIniInfo.LoadDate = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "LoadDate", "", iniPathReagentTrayInfo);
                lisReagentIniInfo.Add(reagentIniInfo);
                reagentIniInfo = new ReagentIniInfo();
            }

            if (lisReagentIniInfo.Count > 0)
            {
                return lisReagentIniInfo;
            }
            else
            {
                return null;
            }
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
        /// 取得指定文件夹下的指定格式的所有文件
        /// </summary>
        /// <param name="folder">指定的文件夹路径</param>
        /// <param name="extension">指定的扩展名</param>
        /// <returns></returns>
        private static List<string> GetFiles(string folder, string extension)
        {
            //若文件夹路径不存在，返回空
            if (!Directory.Exists(folder))
            {
                return null;
            }
            //扩展名必须存在
            if (string.IsNullOrEmpty(extension))
            {
                return null;
            }
            DirectoryInfo dInfo = new DirectoryInfo(folder);
            //文件夹下的所有文件
            FileInfo[] aryFInfo = dInfo.GetFiles();
            List<string> lstRet = new List<string>();
            //将扩展名转化为小写的形式（如“.TXT”与“.txt”其实是相同的），方便后续处理
            extension = extension.ToLower();
            //循环判断每一个文件
            foreach (FileInfo fInfo in aryFInfo)
            {
                //如果当前文件扩展名与指定的相同，则将其加入返回值中
                if (fInfo.Extension.ToLower().Equals(extension))
                {
                    lstRet.Add(fInfo.ToString());
                }
            }
            return lstRet;
        }

        /// <summary>
        /// 运行按钮的状态
        /// </summary>
        void RunBtnStatus()
        {
            defineButton1.Enabled = true;
            defineButton1.BackgroundImage = Properties.Resources.blue_play_128px_569342_easyicon_net;
            defineButton2.BackgroundImage = Properties.Resources.blue_pause_128px_569341_easyicon_net;
            defineButton3.BackgroundImage = Properties.Resources.blue_stop_play_back_128px_569353_easyicon_net;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //2018-08-23 zlx mod
            DialogResult result = MessageBox.Show("是否确定关闭正在运行的系统！", "系统退出警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (result == DialogResult.OK)
            {
                timerStatus.Stop();
                //2018-11-14 zlx add
                //ShowWindow(trayHwnd, 1);
                //ShowWindow(hStar, 1);
                Application.Exit();
                System.Environment.Exit(0);
            }
        }

        private void defineButton1_Click(object sender, EventArgs e)
        {
            //CanCom.Instance.sendStop = false;
            LogFile.Instance.Write(DateTime.Now + "pauseFlag的值为:" + pauseFlag + ",btnRunClick的是否为空:" + (btnRunClick == null ? "NULL" : "NotNUll"));
            if (btnRunClick != null && pauseFlag == false)
            {
                StartFlag = false;
                System.Timers.Timer time = new System.Timers.Timer(5000);//实例化Timer类，设置间隔时间为10000毫秒；
                time.Elapsed += BoolStart;
                time.AutoReset = false;
                time.Start();
                defineButton1.BackgroundImage = Properties.Resources.black_play_128px_569312_easyicon_net;
                defineButton2.BackgroundImage = Properties.Resources.blue_pause_128px_569341_easyicon_net;
                defineButton3.BackgroundImage = Properties.Resources.blue_stop_play_back_128px_569353_easyicon_net;
                defineButton1.Enabled = false;
                frmWorkList.btnRunStatus += new Action(RunBtnStatus);
                frmWorkList.dbtnRackStatus += new Action(dbtnRackStatus);
                btnRunClick(sender, e);
            }
            else if (btnGoonClick != null && pauseFlag)
            {
                defineButton1.BackgroundImage = Properties.Resources.black_play_128px_569312_easyicon_net;
                defineButton2.BackgroundImage = Properties.Resources.blue_pause_128px_569341_easyicon_net;
                defineButton3.BackgroundImage = Properties.Resources.blue_stop_play_back_128px_569353_easyicon_net;
                defineButton2.Enabled = true;
                defineButton1.Enabled = false;
                pauseFlag = false;
                btnGoonClick();
            }
        }
        public static bool StartFlag;//2019-03-02 zlx mod
        void BoolStart(object sender, ElapsedEventArgs e)
        {
            Thread.Sleep(1000);
            if (!StartFlag)
            {
                Action ac = new Action(RunBtnStatus);
                this.Invoke(ac);
                //new Thread(RunBtnStatus).Start();
            }
        }
        private void defineButton2_Click(object sender, EventArgs e)
        {
            if (frmWorkList.RunFlag != (int)RunFlagStart.IsRuning)
            {
                MessageBox.Show("实验未在运行，请勿进行暂停操作！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (pauseFlag == true)
            {
                MessageBox.Show("实验暂停中！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (btnPauseClick != null)
            {
                pauseFlag = true;
                defineButton1.Enabled = true;
                defineButton2.Enabled = false;
                defineButton1.BackgroundImage = Properties.Resources.blue_play_128px_569342_easyicon_net;
                defineButton2.BackgroundImage = Properties.Resources.black_pause_128px_569311_easyicon_net;
                defineButton3.BackgroundImage = Properties.Resources.blue_stop_play_back_128px_569353_easyicon_net;
                btnPauseClick();
            }
        }

        private void defineButton3_Click(object sender, EventArgs e)
        {
            if (btnStopClick != null)
            {
                //2018-07-07 zlx mod 
                DialogResult dr = MessageBox.Show("停止实验会导致此次实验作废，确认是否要停止此次实验！", "信息提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (dr == DialogResult.OK)
                {
                    IniUpdateAccess();
                    defineButton1.BackgroundImage = Properties.Resources.blue_play_128px_569342_easyicon_net;
                    defineButton2.BackgroundImage = Properties.Resources.blue_pause_128px_569341_easyicon_net;
                    defineButton3.BackgroundImage = Properties.Resources.black_stop_play_back_128px_569323_easyicon_net;
                    defineButton1.Enabled = true;
                    btnStopClick();
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 同步试剂、底物配置文件信息到数据库
        /// </summary>
        private void IniUpdateAccess()
        {
            #region 将试剂盘配置文件信息更新到数据库
            List<ReagentIniInfo> lisRIinfo = QueryReagentIniInfo();
            DbHelperOleDb db = new DbHelperOleDb(3);
            if (lisRIinfo.Count > 0)
            {
                foreach (ReagentIniInfo reagentIniInfo in lisRIinfo)
                {
                    db = new DbHelperOleDb(3);
                    DbHelperOleDb.ExecuteSql(3, @"update tbReagent set leftoverTestR1 =" + reagentIniInfo.LeftReagent1 + ",leftoverTestR2 = " + reagentIniInfo.LeftReagent2 +
                                              ",leftoverTestR3 = " + reagentIniInfo.LeftReagent3 + ",leftoverTestR4 = " + reagentIniInfo.LeftReagent4 + " where BarCode = '"
                                                  + reagentIniInfo.BarCode + "' and ReagentName = '" + reagentIniInfo.ItemName + "'");
                }
            }
            #endregion
            #region 将底物配置文件信息更新到数据库
            string sbCode1 = OperateIniFile.ReadIniData("Substrate1", "BarCode", "0", iniPathSubstrateTube);
            string sbNum1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", iniPathSubstrateTube);
            DbHelperOleDb.ExecuteSql(3, @"update tbSubstrate set leftoverTest =" + sbNum1 + " where BarCode = '"
                                                  + sbCode1 + "'");
            //string sbCode2 = OperateIniFile.ReadIniData("Substrate2", "BarCode", "0", iniPathSubstrateTube);
            //string sbNum2 = OperateIniFile.ReadIniData("Substrate2", "LeftCount", "0", iniPathSubstrateTube);
            //DbHelperOleDb.ExecuteSql(@"update tbSubstrate set leftoverTest =" + sbNum2 + " where BarCode = '"
            //                                      + sbCode2 + "'");
            #endregion
        }
        private void defineButton3_MouseLeave(object sender, EventArgs e)
        {
            defineButton3.BackgroundImage = Properties.Resources.blue_stop_play_back_128px_569353_easyicon_net;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            float newx = (this.Width) / X;
            float newy = this.Height / Y;
            autosize.setControls(newx, newy, this);
        }
        private void dbtnBuffer_MouseEnter(object sender, EventArgs e)
        {
            Button button = sender as Button;
            button.FlatStyle = FlatStyle.Popup;
            button.FlatAppearance.BorderSize = 1;
        }

        private void dbtnBuffer_MouseLeave(object sender, EventArgs e)
        {
            Button button = sender as Button;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
        }


        private void fbtnTest_Click(object sender, EventArgs e)
        {
            frmReagentLoad frmRL = new frmReagentLoad();
            frmRL.MdiParent = this;//指定当前窗体为顶级Mdi窗体
            frmRL.Parent = this.pnlPublic;//指定子窗体的父容器为
            frmRL.Show();
        }

        private void fbtnDataQuery_Click(object sender, EventArgs e)
        {
            frmResultQuery frmRQ = new frmResultQuery();
            frmRQ.MdiParent = this;//指定当前窗体为顶级Mdi窗体
            frmRQ.Parent = this.pnlPublic;//指定子窗体的父容器为
            frmRQ.Show();
        }

        private void fbtnSet_Click(object sender, EventArgs e)
        {
            //2018-11-14 zlx mod
            frmInfo frmIF = new frmInfo();
            frmIF.MdiParent = this;//指定当前窗体为顶级Mdi窗体
            frmIF.Parent = this.pnlPublic;//指定子窗体的父容器为
            frmIF.Show();
        }

        private void dbtnSubstract_MouseClick(object sender, MouseEventArgs e)
        {
            if (!CheckFormIsOpen("frmSupplyStatus"))
            {
                frmSupplyStatus frmSS = new frmSupplyStatus();
                frmSS.MdiParent = this;//指定当前窗体为顶级Mdi窗体
                frmSS.Parent = this.pnlPublic;//指定子窗体的父容器为
                frmSS.Show();
            }
            else
            {
                frmSupplyStatus frmSS = (frmSupplyStatus)Application.OpenForms["frmSupplyStatus"];//(frmQCManagement)Application.OpenForms["frmQcM"];
                //frmQcM.Activate();
                frmSS.BringToFront();
            }
        }

        /// <summary>
        /// 判断窗体是否存在
        /// </summary>
        /// <param name="Forms">已存在窗体的类型名</param>
        /// <returns>存在，则返回true</returns>
        public bool CheckFormIsOpen(string Forms)
        {
            bool bResult = false;
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.Name == Forms)
                {
                    bResult = true;
                    break;
                }
            }
            return bResult;
        }
        /// <summary>
        /// 缺液停止标志
        /// </summary>
        public static bool[] StopFlag { get { return _stopflag; } set { _stopflag = value; } }
        private static bool[] _stopflag = new bool[4];
        //报警开启指示
        public static bool BWarn = false;
        /// <summary>
        /// 管架有管数
        /// </summary>
        private int RtSumTubeNum = 0;
        /// <summary>
        /// 底物剩余数
        /// </summary>
        private int RtSubstract = -1;
        /// <summary>
        /// 试剂信息
        /// </summary>
        private List<ReagentIniInfo> RtlisRIinfo = new List<ReagentIniInfo>();
        /// <summary>
        /// 试剂用完位置统计
        /// </summary>
        private List<string> RTReagentPos = new List<string>();
        /// <summary>
        /// 查询各桶状态以及试剂底物管架的状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerStatus_Tick(object sender, EventArgs e)
        {
            //2018-07-31 zlx add
            label2.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            //2018-07-25 zlx mod
            //if (this.ActiveControl == null || this.ActiveControl.Text == "frmDiagnost")//this.ActiveControl == null || this.ActiveControl.Text == "" 
            //{
            if (this.ActiveControl != null && this.ActiveControl.Text == "frmWorkList" && !frmWorkList.BQLiquaid)
            {
                return;
            }
            timerStatus.Enabled = false;
            if (LiquidQueryFlag)
            {
                #region 查询液位信息 zlx add  2018-07-06
                if (!Selectlist.Contains("EB 90 11 09 02"))
                    Selectlist.Add("EB 90 11 09 02");
                #endregion
            }

            #region 检测管架剩余管数状态
            if (frmWorkList.TubeStop || LackTube < 1)
            {
                WarnTime++;
                if (!Selectlist.Contains("EB 90 11 01 06"))
                    Selectlist.Add("EB 90 11 01 06");
            }
            #endregion

            #region 检查底物剩余测数状态
            string LeftCount1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "", iniPathSubstrateTube);
            //string LeftCount2 = OperateIniFile.ReadIniData("Substrate2", "LeftCount", "", iniPathSubstrateTube);
            string LeftCount2 = "0";
            if (LeftCount1 == "" || LeftCount2 == "")//modify by y 20180509
            {
                if (LeftCount1 == "")//add by y 20180509
                    LeftCount1 = "0";//add by y 20180509
                if (LeftCount2 == "")//add by y 20180509
                    LeftCount2 = "0";//add by y 20180509
                                     //return;//y
            }
            if (int.Parse(LeftCount1) + int.Parse(LeftCount2) != RtSubstract)//2018-09-29 zlx add
            {
                if (int.Parse(LeftCount1) + int.Parse(LeftCount2) <= WarnSubstrate)
                {
                    dbtnSubstract.BackgroundImage = Properties.Resources._06;

                    if (int.Parse(LeftCount1) + int.Parse(LeftCount2) <= ErrorSubstrate)
                    {
                        dbtnSubstract.BackgroundImage = Properties.Resources._07;
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "底物剩余测试为"
                        + (int.Parse(LeftCount1) + int.Parse(LeftCount2)).ToString());
                        LogBtnColorChange(0);
                    }
                    else
                    {
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "警告" + " *** " + "未读" + " *** " + "底物剩余测数为"
                       + (int.Parse(LeftCount1) + int.Parse(LeftCount2)).ToString());
                        LogBtnColorChange(1);
                    }
                    WarnTime++;
                }
                else
                {
                    dbtnSubstract.BackgroundImage = Properties.Resources._08;
                }
            }
            RtSubstract = int.Parse(LeftCount1) + int.Parse(LeftCount2);
            #endregion

            #region 检查试剂剩余测数状态
            List<ReagentIniInfo> lisRIinfo = QueryReagentIniInfo();
            #region 删除已经卸载的试剂信息
            List<ReagentIniInfo> RtlisRIinfoC = new List<ReagentIniInfo>();
            foreach (ReagentIniInfo ReagentIniInfo in RtlisRIinfo)
            {
                LogFile.Instance.Write("lisRIinfo的count" + lisRIinfo.Count + ",ReagentIniInfo.ItemName:" + ReagentIniInfo.ItemName);
                List<ReagentIniInfo> list = lisRIinfo.FindAll(ty => ty.ItemName == ReagentIniInfo.ItemName);
                if (list.Count > 0)
                    RtlisRIinfoC.Add(ReagentIniInfo);
            }
            RtlisRIinfo = RtlisRIinfoC;
            #endregion
            dbtnRegent.BackgroundImage = Properties.Resources._14__2_;//2018-09-27 zlx mod
            if (lisRIinfo != null && lisRIinfo.Count > 0)
            {
                foreach (ReagentIniInfo ReagentIniInfo in lisRIinfo)
                {
                    List<string> listItemName = new List<string>();
                    if (ReagentIniInfo.ItemName != "" && !listItemName.Contains(ReagentIniInfo.ItemName))
                    {
                        List<ReagentIniInfo> list = lisRIinfo.FindAll(ty => ty.ItemName == ReagentIniInfo.ItemName);
                        int count = 0;
                        foreach (ReagentIniInfo li in list)
                        {
                            if (li.LeftReagent1 < 1)
                                dbtnRegent.BackgroundImage = Properties.Resources._12__2_;
                            count += li.LeftReagent1;
                        }
                        if (count < ErrorReagent)
                        {
                            dbtnRegent.BackgroundImage = Properties.Resources._13__2_;
                            if (RtlisRIinfo.Find(ty => ty.ItemName == ReagentIniInfo.ItemName) != null && RtlisRIinfo.Find(ty => ty.ItemName == ReagentIniInfo.ItemName).LeftReagent1 == count)
                                continue;
                            LogBtnColorChange(0);
                            LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + ReagentIniInfo.ItemName + "项目试剂剩余测数为" + count.ToString());
                        }
                        listItemName.Add(ReagentIniInfo.ItemName);
                        //2018-09-29 zlx add
                        if (RtlisRIinfo.FindAll(ty => ty.ItemName == ReagentIniInfo.ItemName).Count > 0)
                        {
                            RtlisRIinfo.Find(ty => ty.ItemName == ReagentIniInfo.ItemName).LeftReagent1 = count;
                        }
                        else
                        {
                            ReagentIniInfo info = ReagentIniInfo;
                            info.LeftReagent1 = count;
                            RtlisRIinfo.Add(info);
                        }
                    }
                }
            }
            #endregion


            //2018-08-01 zlx add
            if (this.ActiveControl != null)
            {
                if (this.ActiveControl.Text == "frmReagentLoad")
                {
                    frmSampleLoad frmSL = (frmSampleLoad)Application.OpenForms["frmSampleLoad"];
                    frmSL.LoadData();
                }
                if (this.ActiveControl.Text == "frmSampleLoad")
                {
                    frmSampleLoad frmSL = (frmSampleLoad)Application.OpenForms["frmSampleLoad"];
                    frmSL.LoadData();
                }
                //2018-08-14 zlx add
                if (this.ActiveControl.Text == "frmSupplyStatus")
                {
                    frmSupplyStatus f = (frmSupplyStatus)this.ActiveControl;
                    f.frmSupplyStatus_Load();

                }
            }
            //2018-07-25 zlx mod                
            #region 温度监控 //2018-07-5
            TimeSpan ts = DateTime.Now - _BootUpTime;
            //if (ts.TotalMinutes > 30)
            //{

            //}
            //else
            //{
            //if (Temprrature[0] == 0)
            //    Selectlist.Add(new string[] { "EB 90 11 04 04", "5" });
            //if(Temprrature[1] == 0)
            //    Selectlist.Add(new string[] { "EB 90 11 05 04", "5" });
            //if (Temprrature[2] == 0)
            //    Selectlist.Add(new string[] { "EB 90 11 06 04", "5" });
            //if (Temprrature[3] == 0)
            //    Selectlist.Add(new string[] { "EB 90 11 07 04", "5" });
            if (!Selectlist.Contains("EB 90 11 04 04"))
                Selectlist.Add("EB 90 11 04 04");
            if (!Selectlist.Contains("EB 90 11 05 04"))
                Selectlist.Add("EB 90 11 05 04");
            if (!Selectlist.Contains("EB 90 11 06 04"))
                Selectlist.Add("EB 90 11 06 04");
            if (!Selectlist.Contains("EB 90 11 07 04"))
                Selectlist.Add("EB 90 11 07 04");
            //}
            #endregion
            timerStatus.Enabled = true;
            //}
            #region 网络连接状态
            //if (NetCom3.isConnect)
            //{
            //    dbtnConnect.BackgroundImage = Properties.Resources.已连接;
            //    BeginInvoke(new Action(() => toolTip1.SetToolTip(this.dbtnConnect, "网络已连接")));//2018-07-20 zlx mod
            //}
            //else
            //{
            //    dbtnConnect.BackgroundImage = Properties.Resources.未连接;
            //    BeginInvoke(new Action(() => toolTip1.SetToolTip(this.dbtnConnect, "网络未连接")));//2018-07-20 zlx mod
            //    timerStatus.Enabled = false;
            //}
            #endregion            
        }
        /// <summary>
        /// 处理接收信息信息 2018-07-11
        /// </summary>
        /// <param name="num">查询液位反馈指令</param>
        /// <returns></returns>
        public void IntToBool(string Message)
        {
            string[] dataRecive = Message.Split(' ');
            switch (dataRecive[4])
            {
                case "09":
                    #region 查询液位处理
                    DealLiquid(dataRecive);
                    //recivelist.Remove(recivelist[i]);
                    #endregion
                    break;
                case "04":
                case "05":
                case "06":
                case "07":
                    #region 处理查询的温度信息
                    if (dataRecive[5] == "04")
                    {
                        DealTemperature(dataRecive);
                        alarmOfTemperature(false, dataRecive[4]);
                    }
                    #endregion
                    break;
                case "01":
                    #region 处理缺管查询指令
                    if (dataRecive[6] == "FF")
                    {
                        LackTube = 1;
                        if (frmWorkList.TubeStop)
                        {
                            LackTube = 1;
                            frmWorkList.TubeStop = false;
                        }
                    }
                    else
                        LackTube = 0;
                    Action ac = new Action(dbtnRackStatus);
                    this.Invoke(ac);
                    #endregion
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 管架按钮的状态
        /// </summary>
        void dbtnRackStatus()
        {
            if (frmWorkList.TubeStop || LackTube < 1)
            {
                dbtnRack.BackgroundImage = Properties.Resources._12;//蓝色（红色为_12）
            }
            else
            {
                dbtnRack.BackgroundImage = Properties.Resources._14;//蓝色（红色为_12）
            }
        }
        /// <summary>
        /// 处理查询液位信息 2018-07-13
        /// </summary>
        public void DealLiquid(string[] dataRecive)
        {
            if (dataRecive[5] != "02")
                return;
            int dec = Convert.ToInt32(dataRecive[15], 16);
            string bit = Convert.ToString(dec, 2);
            while (bit.Length < 8)
            {
                bit = "0" + bit;
            }

            if (bit.Substring(0, 1) == "0")
            {
                if (frmWorkList.RunFlag == (int)RunFlagStart.IsRuning)
                    LackLq[0]++;
                else
                    LackLq[0] = 1;
            }
            else
            {
                if (LackLq[0] > 0)
                    LackLq[0] = 0;
            }
            if (bit.Substring(1, 1) == "0")
            {
                if (frmWorkList.RunFlag == (int)RunFlagStart.IsRuning)
                    LackLq[1]++;
                else
                    LackLq[1] = 1;
            }
            else
            {
                if (LackLq[1] > 0)
                    LackLq[1] = 0;
            }
            if (bit.Substring(2, 1) == "0")
            {
                if (frmWorkList.RunFlag == (int)RunFlagStart.IsRuning)
                    LackLq[2]++;
                else
                    LackLq[2] = 1;
            }
            else
            {
                if (LackLq[2] > 0)
                    LackLq[2] = 0;
            }
            if (bit.Substring(3, 1) == "0")
            {
                NumWTubettime++;
                if (NumWTubettime >= 20)
                {
                    if (frmWorkList.RunFlag == (int)RunFlagStart.IsRuning)
                        LackLq[3]++;
                    else
                        LackLq[3] = 1;
                }
                else
                {
                    LackLq[3] = 0;
                }
            }
            else
            {
                NumWTubettime = 0;
                LackLq[3] = 0;
            }
            ShowLiquidInfo();
        }
        //public void DealLiquid(string[] dataRecive)
        //{
        //    if (dataRecive[5] != "02")
        //        return;
        //    int dec = Convert.ToInt32(dataRecive[15], 16);
        //    string bit = Convert.ToString(dec, 2);
        //    while (bit.Length < 8)
        //    {
        //        bit = "0" + bit;
        //    }

        //    if (bit.Substring(0, 1) == "0")
        //    {
        //        if (frmWorkList.RunFlag==(int)RunFlagStart.IsRuning )
        //            LackLq[0]++;
        //        else
        //            LackLq[0] = 1;
        //    }
        //    else
        //    {
        //        if (LackLq[0] > 0)
        //            LackLq[0] = 0;
        //    }
        //    if (bit.Substring(1, 1) == "0")
        //    {
        //        if (frmWorkList.RunFlag == (int)RunFlagStart.IsRuning)
        //            LackLq[1]++;
        //        else
        //            LackLq[1] = 1;
        //    }
        //    else
        //    {
        //        if (LackLq[1] > 0)
        //            LackLq[1] = 0;
        //    }
        //    if (bit.Substring(2, 1) == "0")
        //    {
        //        if (frmWorkList.RunFlag == (int)RunFlagStart.IsRuning)
        //            LackLq[2]++;
        //        else
        //            LackLq[2] = 1;
        //    }
        //    else
        //    {
        //        if (LackLq[2] > 0)
        //            LackLq[2] = 0;
        //    }
        //    if (bit.Substring(3, 1) == "0")
        //    {
        //        if (frmWorkList.RunFlag == (int)RunFlagStart.IsRuning)
        //            LackLq[3]++;
        //        else
        //            LackLq[3] = 1;
        //    }
        //    else
        //    {
        //        if (LackLq[3] > 0)
        //            LackLq[3] = 0;
        //    }
        //    ShowLiquidInfo();
        //}
        /// <summary>
        /// 显示液位信息
        /// </summary>
        public void ShowLiquidInfo()
        {
            if (LackLq[0] > 0)
            {
                if (!BWarn)
                {
                    //NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 09 00"), 5);
                    //NetCom3.Instance.SingleQuery();
                    Selectlist.Add("EB 90 11 09 00");
                    BWarn = true;
                }
                //错误
                //错误存储到Log文件

                if (LackLq[0] > MaxBuffertime)//2018-07-06
                {
                    //2018-09-29 
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "磁珠清洗液为空");
                    dbtnBuffer.BackgroundImage = Properties.Resources._2;//黄色（红色为_2）
                    LogBtnColorChange(0);
                    StopFlag[0] = true;
                }
                else
                {
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "警告" + " *** " + "未读" + " *** " + "磁珠清洗液为空");
                    dbtnBuffer.BackgroundImage = Properties.Resources._3;//黄色（红色为_2）
                    LogBtnColorChange(1);
                }
                WarnTime++;
            }
            else
            {
                //2018-07-26 zlx mod
                if (BWarn && (LackLq[0] == 0 && LackLq[1] == 0 && LackLq[2] == 0 && LackLq[3] == 0))
                {
                    //NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 09 01"), 5);
                    //NetCom3.Instance.SingleQuery();
                    Selectlist.Add("EB 90 11 09 01");
                    BWarn = false;
                }
                dbtnBuffer.BackgroundImage = Properties.Resources._1;//蓝色
                StopFlag[0] = false;//2018-10-09
            }
            if (LackLq[1] > 0)
            {
                if (!BWarn)
                {
                    //NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 09 00"), 5);
                    //NetCom3.Instance.SingleQuery();
                    //Selectlist.Add(new string[] { "EB 90 11 09 00", "5" });
                    Selectlist.Add("EB 90 11 09 00");
                    BWarn = true;
                }
                //错误
                //错误存储到Log文件
                if (LackLq[1] > MaxWashtime)//2018-07-06
                {
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "探针清洗液为空");
                    dbtnWash.BackgroundImage = Properties.Resources._7;//红色（红色为_2）
                    LogBtnColorChange(0);
                    StopFlag[1] = true;
                }
                else
                {
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "警告" + " *** " + "未读" + " *** " + "探针清洗液为空");
                    dbtnWash.BackgroundImage = Properties.Resources._6__2_;//黄色（红色为_7）
                    LogBtnColorChange(1);
                }
                WarnTime++;
            }
            else
            {
                //2018-07-26 zlx mod
                if (BWarn && (LackLq[0] == 0 && LackLq[1] == 0 && LackLq[2] == 0 && LackLq[3] == 0))
                {
                    //NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 09 01"), 5);
                    //NetCom3.Instance.SingleQuery();
                    //Selectlist.Add(new string[] { "EB 90 11 09 01", "5" });
                    Selectlist.Add("EB 90 11 09 01");
                    BWarn = false;
                }
                dbtnWash.BackgroundImage = Properties.Resources._8;//蓝色
                StopFlag[1] = false;//2018-10-09
            }
            if (LackLq[2] > 0)
            {
                //错误
                //错误存储到Log文件
                if (!BWarn)
                {
                    Selectlist.Add("EB 90 11 09 00");
                    //NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 09 00"), 5);
                    //NetCom3.Instance.SingleQuery();
                    BWarn = true;
                }

                if (LackLq[2] > MaxWastetime)//2018-07-05 zlx add
                {
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "废液桶已满");
                    dbtnWaste.BackgroundImage = Properties.Resources._10;//黄色（红色为_10）
                    LogBtnColorChange(0);
                    StopFlag[2] = true;
                }
                else
                {
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "警告" + " *** " + "未读" + " *** " + "废液桶已满");
                    dbtnWaste.BackgroundImage = Properties.Resources._11;//黄色（红色为_10）
                    LogBtnColorChange(1);
                }
                WarnTime++;
            }
            else
            {
                //2018-07-26 zlx add
                if (BWarn && (LackLq[0] == 0 && LackLq[1] == 0 && LackLq[2] == 0 && LackLq[3] == 0))
                {
                    //NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 09 01"), 5);
                    //NetCom3.Instance.SingleQuery();
                    Selectlist.Add("EB 90 11 09 01");
                    BWarn = false;
                }
                dbtnWaste.BackgroundImage = Properties.Resources._9;//蓝色
                StopFlag[2] = false;//2018-10-09
            }
            if (LackLq[3] > 0)
            {
                //错误
                //错误存储到Log文件
                if (LackLq[3] > MaxWTubetime)
                {
                    btnWasteRack.BackgroundImage = Properties.Resources.WasteRack01;//红色
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "废管盒已满");
                    LogBtnColorChange(0);
                    StopFlag[3] = true;
                }
                else
                {
                    btnWasteRack.BackgroundImage = Properties.Resources.WasteRack03;//黄色
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "警告" + " *** " + "未读" + " *** " + "废管盒已满");
                    if (dbtnLog.BackgroundImage != Properties.Resources._22感叹号)//红色
                        LogBtnColorChange(1);
                }
                WarnTime++;
            }
            else
            {
                btnWasteRack.BackgroundImage = Properties.Resources.WasteRack02;//蓝色
                StopFlag[3] = false;
            }
        }
        /// <summary>
        /// 处理查询温度信息 2018-07-14
        /// </summary>
        /// <param name="dataRecive"></param>
        public void DealTemperature(string[] dataRecive)
        {
            decimal readData = Math.Round(Convert.ToDecimal(NetCom3.HexToFloat(dataRecive[12] + dataRecive[13] + dataRecive[14] + dataRecive[15])), 1);
            switch (dataRecive[4])
            {
                case "04":
                    Temprrature[0] = readData;
                    break;
                case "05":
                    Temprrature[1] = readData;
                    break;
                case "06":
                    Temprrature[2] = readData;
                    break;
                case "07":
                    Temprrature[3] = readData;
                    break;
                default:
                    break;
            }
        }

        private void dbtnLog_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmLogShow"))
            {
                frmLogShow frmLS = new frmLogShow();
                frmLS.MdiParent = this;//指定当前窗体为顶级Mdi窗体
                frmLS.Parent = this.pnlPublic;//指定子窗体的父容器为
                frmLS.Show();
            }
            else
            {
                frmLogShow frmLS = (frmLogShow)Application.OpenForms["frmLogShow"];
                //frmQcM.Activate();
                frmLS.BTrefresh_Click();
                frmLS.BringToFront();
            }
        }

        private void dbtnBuffer_Click(object sender, EventArgs e)
        {
            #region 2018-07-17 屏蔽
            /*
            int LackLqValue = 15;
            LackLq = IntToBool(LackLqValue);
            if (LackLq[0])
            {
                //错误
                //错误存储到Log文件
                LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "磁珠清洗液为空");
                dbtnBuffer.BackgroundImage = Properties.Resources._3;//黄色（红色为_2）
                LogBtnColorChange(1);
                frmMsgShow.MessageShow("警告", "磁珠清洗液为空");
            }
            else
            {
                //取消错误
                dbtnBuffer.BackgroundImage = Properties.Resources._1;//蓝色
                frmMsgShow.MessageShow("提示", "磁珠清洗液未空");
            }
            LackLq = new bool[] { false, false, false, false };
             */
            #endregion
            if (LackLq[0] > 0)//2018-07-12 zlx mod
            {
                //错误
                //错误存储到Log文件
                //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "警告" + " *** " + "未读" + " *** " + "磁珠清洗液为空");
                //dbtnBuffer.BackgroundImage = Properties.Resources._3;//黄色（红色为_2）
                //LogBtnColorChange(1);
                //2018-08-13 zlx mod
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow("警告", "磁珠清洗液为空");
                }))
                { IsBackground = true }.Start();
                //frmMsgShow.MessageShow("警告", "磁珠清洗液为空");
            }
            else
            {
                dbtnBuffer.BackgroundImage = Properties.Resources._1;//蓝色
                //取消错误
                //dbtnBuffer.BackgroundImage = Properties.Resources._1;//蓝色
                //2018-08-13 zlx mod
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    frmMessageShow f = new frmMessageShow();
                    //f.ShowInTaskbar = false;
                    f.MessageShow("警告", "磁珠清洗液正常");
                }))
                { IsBackground = true }.Start();
                //frmMsgShow.MessageShow("提示", "磁珠清洗液正常");//2018-07-19 zlx mod
            }
        }

        private void dbtnWash_Click(object sender, EventArgs e)
        {
            #region 2018-07-17 zlx 屏蔽
            /*
            int LackLqValue = 15;
            LackLq = IntToBool(LackLqValue);
            if (LackLq[1])
            {
                //错误
                //错误存储到Log文件
                LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "探针清洗液为空");
                dbtnWash.BackgroundImage = Properties.Resources._6__2_;//黄色（红色为_7）
                LogBtnColorChange(1);
                frmMsgShow.MessageShow("警告", "探针清洗液为空");
            }
            else
            {
                //错误
                dbtnWash.BackgroundImage = Properties.Resources._8;//蓝色
                frmMsgShow.MessageShow("提示", "探针清洗液未空");
            }
            LackLq = new bool[] { false, false, false, false };
             */
            #endregion
            if (LackLq[1] > 0)//2018-07-12 zlx mod
            {
                //错误
                //错误存储到Log文件
                //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "警告" + " *** " + "未读" + " *** " + "探针清洗液为空");
                //dbtnWash.BackgroundImage = Properties.Resources._6__2_;//黄色（红色为_7）
                //LogBtnColorChange(1);
                //2018-08-13 zlx mod
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow("警告", "探针清洗液为空");
                }))
                { IsBackground = true }.Start();
                //frmMsgShow.MessageShow("警告", "探针清洗液为空");
            }
            else
            {
                dbtnWash.BackgroundImage = Properties.Resources._8;//蓝色
                //2018-08-13 zlx mod
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow("警告", "探针清洗液正常");
                }))
                { IsBackground = true }.Start();
                //frmMsgShow.MessageShow("提示", "探针清洗液正常");//2018-07-19 zlx mod
            }
        }

        private void dbtnWaste_Click(object sender, EventArgs e)
        {
            #region 2018-07-17 zlx 屏蔽
            /*
            int LackLqValue = 15;
            LackLq = IntToBool(LackLqValue);
            if (LackLq[3])
            {
                //错误
                dbtnWaste.BackgroundImage = Properties.Resources._9;//蓝色
                frmMsgShow.MessageShow("提示", "废液桶未满");
            }
            else
            {
                //错误
                //错误存储到Log文件
                LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "废液桶已满");
                dbtnWaste.BackgroundImage = Properties.Resources._11;//黄色（红色为_10）
                LogBtnColorChange(1);
                frmMsgShow.MessageShow("警告", "废液桶已满");
            }
            LackLq = new bool[] { false, false, false, false };
             */
            #endregion
            if (LackLq[2] > 0)//2018-07-12 zlx mod
            {
                //错误
                //错误存储到Log文件
                //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "警告" + " *** " + "未读" + " *** " + "废液桶已满");
                //dbtnWaste.BackgroundImage = Properties.Resources._11;//黄色（红色为_10）
                //LogBtnColorChange(1);
                //2018-08-13 zlx mod
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow("警告", "废液桶已满");
                }))
                { IsBackground = true }.Start();
                //frmMsgShow.MessageShow("警告", "废液桶已满");

            }
            else
            {
                dbtnWaste.BackgroundImage = Properties.Resources._9;//蓝色
                //2018-08-13 zlx mod
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow("警告", "废液桶正常");
                }))
                { IsBackground = true }.Start();
                //frmMsgShow.MessageShow("提示", "废液桶正常");//2018-07-19 zlx mod
            }
        }
        /// <summary>
        /// 更改管架、底物、试剂按钮颜色。 LYN add 20171114
        /// </summary>
        /// <param name="rackFlag">管架标志位，0：蓝色，1：黄色，2：红色,3:颜色不变</param>
        /// <param name="subFlag">底物标志位</param>
        /// <param name="regentFlag">试剂标志位</param>
        void RegenColorChange(int rackFlag, int subFlag, int regentFlag)
        {
            if (rackFlag == 0)
            {
                dbtnRack.BackgroundImage = Properties.Resources._14;
            }
            else if (rackFlag == 1)
            {
                dbtnRack.BackgroundImage = Properties.Resources._13;
            }
            else if (rackFlag == 2)
            {
                dbtnRack.BackgroundImage = Properties.Resources._12;
            }

            if (subFlag == 0)
            {
                dbtnSubstract.BackgroundImage = Properties.Resources._08;
            }
            else if (subFlag == 1)
            {
                dbtnSubstract.BackgroundImage = Properties.Resources._06;
            }
            else if (subFlag == 2)
            {
                dbtnSubstract.BackgroundImage = Properties.Resources._07;
            }

            if (regentFlag == 0)
            {
                dbtnRegent.BackgroundImage = Properties.Resources._14__2_;
            }
            else if (regentFlag == 1)
            {
                dbtnRegent.BackgroundImage = Properties.Resources._12__2_;
            }
            else if (regentFlag == 2)
            {
                dbtnRegent.BackgroundImage = Properties.Resources._14__2_;
            }

        }

        private void fbtnScalQc_Click(object sender, EventArgs e)
        {
            frmScaling frmSQ = new frmScaling();
            frmSQ.MdiParent = this;//指定当前窗体为顶级Mdi窗体
            frmSQ.Parent = this.pnlPublic;//指定子窗体的父容器为
            frmSQ.Show();
        }

        private void fbtnMaintenance_Click(object sender, EventArgs e)
        {
            frmInstruMaintenance frmim = new frmInstruMaintenance();
            frmim.MdiParent = this;//指定当前窗体为顶级Mdi窗体
            frmim.Parent = this.pnlPublic;//指定子窗体的父容器为
            frmim.Show();
        }

        /// <summary>
        /// 查询系统温度，改变标签状态，同时更改运行指示参数
        /// </summary>2018-07-14 zlx mod
        /// <param name="bo">是否弹窗提示</param>
        private void alarmOfTemperature(bool bo, string Ttype)
        {
            #region 屏蔽原有查询温度信息 2018-07-17 zlx mod
            /*
            double reagent, wenyu, qingxi, diwu;
            reagent = 18;//后期更换为数据查询
            wenyu = 37;//后期更换为数据查询
            qingxi = 35;//后期更换为数据查询
            diwu = 30;//后期更换为数据查询

            if (reagent > 20 || wenyu < 35 || qingxi < 33 || diwu < 20)//后期需要更改标准,包括下面的部分
            {
                StringBuilder st = new StringBuilder();
                StringBuilder st2 = new StringBuilder();
                st.Append("温度警告：");
                st2.Append("其中,");
                if (reagent > 20)
                {
                    st.Append("试剂盘温度");
                    st2.Append("试剂盘" + reagent.ToString() + "℃");
                }
                if (wenyu < 35)
                {
                    st.Append("、温育盘温度");
                    st2.Append("，温育盘" + wenyu.ToString() + "℃");
                }
                if (qingxi < 33)
                {
                    st.Append("、清洗盘温度");
                    st2.Append("，清洗盘" + qingxi.ToString() + "℃");
                }
                if (diwu < 20)
                {
                    st.Append("、底物环境温度");
                    st2.Append("，底物" + diwu.ToString() + "℃");
                }
                st.Append("未达到标准。");
                temperatureButton.BackgroundImage = Properties.Resources.temperature_2;
                toolTip1.SetToolTip(temperatureButton, "温度警告：\n" + st.ToString());
                //此时停止机器，不允许运行
                if (bo)
                {
                    frmMsgShow.MessageShow("温度警告", st.ToString() + st2.ToString());
                }
                LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + st.ToString() + st2.ToString());
                //LogBtnColorChange(1);
            }
            else
            {
                temperatureButton.BackgroundImage = Properties.Resources.temperature_1;
                toolTip1.SetToolTip(temperatureButton, "系统温度查询");
                //此时更新标志，指示可以运行
                if (bo)
                {
                    frmMsgShow.MessageShow("系统温度查询", "温度已达到运行标准");
                }
            }
             */
            #endregion 
            //2018-08-17 zlx add
            if (Temprrature[0] == 0 || Temprrature[1] == 0 || Temprrature[2] == 0 || Temprrature[3] == 0)
            {
                BeginInvoke(new Action(() => toolTip1.SetToolTip(temperatureButton, "温度警告")));
                return;
            }
            if (Temprrature[0] < RangeWY[0] || Temprrature[0] > RangeWY[1] || Temprrature[1] < RangeWash[0] || Temprrature[1] > RangeWash[1] || Temprrature[3] < RangeSubstrate[0] || Temprrature[3] > RangeSubstrate[1] || Temprrature[2] < RangeQXGL[0] || Temprrature[2] > RangeQXGL[1])//后期需要更改标准,包括下面的部分
            {
                StringBuilder st = new StringBuilder();
                StringBuilder st2 = new StringBuilder();
                st.Append("温度警告：");
                st2.Append("其中");
                int i = st.Length;
                i = st2.Length;
                if (Ttype.Contains("04") && (Temprrature[0] < RangeWY[0] || Temprrature[0] > RangeWY[1]))
                {
                    decimal Temp = Temprrature[0];
                    if (Temp > 55)
                        Temp = 55;
                    st.Append("温育盘温度");
                    st2.Append("温育盘" + Temp.ToString() + "℃");
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "温育盘温度未达到标准：" + Temp.ToString() + "℃");
                }
                if (Ttype.Contains("05") && (Temprrature[1] < RangeWash[0] || Temprrature[1] > RangeWash[1]))
                {
                    decimal Temp = Temprrature[1];
                    if (Temp > 55)
                        Temp = 55;
                    if (st.Length > 5)
                    {
                        st.Append("、清洗盘温度");
                        st2.Append("，清洗盘" + Temp.ToString() + "℃");
                    }
                    else
                    {
                        st.Append("清洗盘温度");
                        st2.Append("清洗盘" + Temp.ToString() + "℃");
                    }
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "清洗盘温度未达到标准：" + Temp.ToString() + "℃");
                }
                if (Ttype.Contains("07") && (Temprrature[3] < RangeSubstrate[0] || Temprrature[3] > RangeSubstrate[1]))
                {
                    decimal Temp = Temprrature[3];
                    if (Temp > 55)
                        Temp = 55;
                    if (st.Length > 5)
                    {
                        st.Append("、底物环境温度");
                        st2.Append("，底物" + Temp.ToString() + "℃");
                    }
                    else
                    {
                        st.Append("底物环境温度");
                        st2.Append("底物" + Temp.ToString() + "℃");
                    }
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "底物温度未达到标准：" + Temp.ToString() + "℃");
                }
                if (Ttype.Contains("06") && (Temprrature[2] < RangeQXGL[0] || Temprrature[2] > RangeQXGL[1]))
                {
                    decimal Temp = Temprrature[2];
                    if (Temp > 55)
                        Temp = 55;
                    if (st.Length > 5)
                    {
                        st.Append("、清洗管路温度");
                        st2.Append("，清洗管路" + Temp.ToString() + "℃");
                    }
                    else
                    {
                        st.Append("清洗管路温度");
                        st2.Append("清洗管路" + Temp.ToString() + "℃");
                    }
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "清洗管路温度未达到标准：" + Temp.ToString() + "℃");
                }
                st.Append("未达到标准。");
                temperatureButton.BackgroundImage = Properties.Resources.temperature_2;
                BeginInvoke(new Action(() => toolTip1.SetToolTip(temperatureButton, "温度警告：\n" + st.ToString())));//2018-07-20 zlx mod
                //此时停止机器，不允许运行
                if (bo)
                {
                    //2018-08-13 zlx mod 
                    new Thread(new ParameterizedThreadStart((obj) =>
                    {
                        frmMessageShow f = new frmMessageShow();
                        f.MessageShow("温度警告", st.ToString() + st2.ToString());
                    }))
                    { IsBackground = true }.Start();

                }
                //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + st.ToString() + st2.ToString());
                //LogBtnColorChange(1);
            }
            else
            {
                temperatureButton.BackgroundImage = Properties.Resources.temperature_1;
                BeginInvoke(new Action(() => toolTip1.SetToolTip(temperatureButton, "系统温度查询")));//2018-07-20 zlx mod
                //此时更新标志，指示可以运行
                if (bo)
                {
                    //2018-08-13 zlx mod
                    new Thread(new ParameterizedThreadStart((obj) =>
                    {
                        frmMessageShow f = new frmMessageShow();
                        f.MessageShow("温度警告", "温度已达到运行标准");
                    }))
                    { IsBackground = true }.Start();

                }
            }
        }

        private void temperatureButton_Click(object sender, EventArgs e)
        {
            //2018-07-25 zlx mod
            TimeSpan ts = DateTime.Now - _BootUpTime;
            if (ts.TotalMinutes < 30 || (Temprrature[0] == 0 && Temprrature[1] == 0 && Temprrature[2] == 0 && Temprrature[3] == 0))
            {
                //if (NetCom3.isConnect)
                //{
                //    NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 04 04"), 5);
                //    NetCom3.Instance.SingleQuery();
                //    NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 05 04"), 5);
                //    NetCom3.Instance.SingleQuery();
                //    NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 06 04"), 5);
                //    NetCom3.Instance.SingleQuery();
                //    NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 07 04"), 5);
                //    NetCom3.Instance.SingleQuery();
                //}
                //2018-08-16 zlx add
                if (!Selectlist.Contains("EB 90 11 04 04") && Temprrature[0] == 0)
                    Selectlist.Add("EB 90 11 04 04");
                if (!Selectlist.Contains("EB 90 11 05 04") && Temprrature[1] == 0)
                    Selectlist.Add("EB 90 11 05 04");
                if (!Selectlist.Contains("EB 90 11 06 04") && Temprrature[2] == 0)
                    Selectlist.Add("EB 90 11 06 04");
                if (!Selectlist.Contains("EB 90 11 07 04") && Temprrature[3] == 0)
                    Selectlist.Add("EB 90 11 07 04");
            }
            Thread.Sleep(100);
            alarmOfTemperature(true, "04,05,06,07");
            //alarmOfTemperature(true,"05");
            //alarmOfTemperature(true,"06");
            //alarmOfTemperature(true,"07");
        }

        private void dbtnConnect_MouseEnter(object sender, EventArgs e)
        {
            Button button = sender as Button;
            button.FlatStyle = FlatStyle.Popup;
            button.FlatAppearance.BorderSize = 1;

        }

        private void dbtnConnect_MouseLeave(object sender, EventArgs e)
        {
            Button button = sender as Button;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            if (NetCom3.isConnect && NetCom3.Instance.isHeartbeatLive)
            {
                dbtnConnect.Enabled = false;
                //dbtnConnect.BackgroundImage = Properties.Resources.已连接;
                toolTip1.SetToolTip(this.dbtnConnect, "网络已连接");
            }
            else
            {
                dbtnConnect.Enabled = true;
                //dbtnConnect.BackgroundImage = Properties.Resources.未连接;
                toolTip1.SetToolTip(this.dbtnConnect, "网络未连接");
            }
        }

        private void dbtnConnect_MouseDown(object sender, MouseEventArgs e)
        {
            if (NetCom3.isConnect && NetCom3.Instance.isHeartbeatLive) return;

            if (NetCom3.Instance.CheckMyIp_Port_Link())
            {
                NetCom3.Instance.close();
                NetCom3.Instance.ConnectServer();

                if (!NetCom3.isConnect)
                {
                    frmMessageShow frmMS = new frmMessageShow();
                    frmMS.MessageShow("系统通讯提示", "无法连接到仪器！");
                    frmMS.Dispose();
                    return;
                }
                else
                {
                    NetCom3.Instance.SendHeartbeat();
                    fbtnTest.Enabled = true;
                    fbtnMaintenance.Enabled = true;
                }
            }
        }

        protected override CreateParams CreateParams//双缓冲，解决界面加载闪烁问题
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        private void btnVersion_Click(object sender, EventArgs e)
        {

            //frmVersion f = new frmVersion();
            //f.ShowDialog();
            //暂时使用调用系统快捷键的方式最小化
            Type tempType = Type.GetTypeFromProgID("Shell.Application");
            object oleObject = System.Activator.CreateInstance(tempType);
            tempType.InvokeMember("ToggleDesktop", BindingFlags.InvokeMethod, null, oleObject, null);

        }

        private void pnlbarUP_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dbtnRack_Click(object sender, EventArgs e)
        {
            //if (!Selectlist.Contains("EB 90 11 01 06"))
            //    Selectlist.Add("EB 90 11 01 06");
            if (LackTube == 0)
            {
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow("警告", "暂存盘位置缺管");
                }))
                { IsBackground = true }.Start();
            }
            else
            {
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow("警告", "暂存盘位置有管");
                }))
                { IsBackground = true }.Start();
            }
        }

        #region 声音报警功能信息 2019-06-29 zlx add
        private void dbtnSound_Click(object sender, EventArgs e)
        {
            if (SoundFlag == (int)SoundFlagStart.isClose)
                SoundFlag = (int)SoundFlagStart.IsOpen;
            else
                SoundFlag = (int)SoundFlagStart.isClose;
            if (SoundFlag == (int)SoundFlagStart.IsOpen)
            {
                dbtnSound.BackgroundImage = Properties.Resources.声音启用;
                timeWarnSound.Start();
            }
            else
            {
                dbtnSound.BackgroundImage = Properties.Resources.声音关闭;
                timeWarnSound.Stop();
            }

        }
        public class BeepUp
        {
            /// <param name="iFrequency">声音频率（从37Hz到32767Hz）。在windows95中忽略</param>  
            /// <param name="iDuration">声音的持续时间，以毫秒为单位。</param>  
            [DllImport("Kernel32.dll")] //引入命名空间 using System.Runtime.InteropServices;  
            public static extern bool Beep(int frequency, int duration);
        }
        /// <summary>
        /// 是否正在报警
        /// </summary>
        private void timeWarnSound_Tick(object sender, EventArgs e)
        {
            if (SoundFlag == (int)SoundFlagStart.isClose) return;
            //Iswarn = true;
            timeWarnSound.Enabled = false;
            bool WarnRgent = false;
            foreach (ReagentIniInfo ReagentIniInfo in RtlisRIinfo)
            {
                if (RtlisRIinfo.Find(ty => ty.ItemName == ReagentIniInfo.ItemName).LeftReagent1 < ErrorReagent)
                    WarnRgent = true;
            }
            if (LackLq[0] > 0 || LackLq[1] > 0 || LackLq[2] > 0 || LackLq[3] > 0 || frmWorkList.TubeStop || (RtSubstract != -1 && RtSubstract < WarnSubstrate))//|| WarnRgent
            {
                BeepUp.Beep(392, 500);
            }
            timeWarnSound.Enabled = true;
            //Iswarn = false;
        }
        #endregion

        private void btnWasteRack_Click(object sender, EventArgs e)
        {
            //2019-09-27 zlx add
            if (LackLq[3] > 0)
            {
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow("警告", "废管盒已满");
                }))
                { IsBackground = true }.Start();

            }
            else
            {
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow("警告", "废管盒正常");
                }))
                { IsBackground = true }.Start();
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            //root权限可以通过帮助进行最小化
            if (frmLogin.LoginUserType != "0")
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            //frmWorkList.btnLogColor -= new Action<int>(LogBtnColorChange);
            //if (CanCom.Instance.IsDev_Connet())
            //{
            //    CanCom.Instance.Dev_Close();
            //}

            if (!Directory.Exists(@"C:\temp"))
            {
                Directory.CreateDirectory(@"C:\temp");
            }
            string defaultIniPath = Directory.GetCurrentDirectory() + "\\ReactTrayInfo.ini";//温育盘ini文件地址
            string cTempIniPath = @"C:\temp\ReactTrayInfo.ini";//放在C盘temp文件夹的临时温育盘ini文件地址
            if (File.Exists(defaultIniPath))
            {
                if (!File.Exists(cTempIniPath))
                {
                    //把默认文件夹中的ReactTrayInfo.ini文件复制过来
                    File.Copy(defaultIniPath, cTempIniPath);
                }
                else
                {
                    File.Delete(cTempIniPath);
                    File.Copy(defaultIniPath, cTempIniPath);
                }
            }
        }

        private void timerConnect_Tick(object sender, EventArgs e)
        {
            timerConnect.Enabled = false;

            if (NetCom3.isConnect && NetCom3.Instance.isHeartbeatLive)
            {
                dbtnConnect.Enabled = false;
            }
            else
            {
                dbtnConnect.Enabled = true;
            }

            timerConnect.Enabled = true;
        }

        private void dbtnConnect_EnabledChanged(object sender, EventArgs e)
        {
            if ((sender as Button).Enabled)
            {
                dbtnConnect.BackgroundImage = Properties.Resources.未连接;
                BeginInvoke(new Action(() => toolTip1.SetToolTip(this.dbtnConnect, "网络未连接")));
                return;
            }

            dbtnConnect.BackgroundImage = Properties.Resources.已连接;
            BeginInvoke(new Action(() => toolTip1.SetToolTip(this.dbtnConnect, "网络未连接")));
        }
    }
    /// <summary>
    /// 声音报警状态 isClose-禁止状态 IsOpen-开启状态 isStarTime=正在报警
    /// </summary>
    public enum SoundFlagStart { isClose = 0, IsOpen = 1, isStarTime = 2 }
}
