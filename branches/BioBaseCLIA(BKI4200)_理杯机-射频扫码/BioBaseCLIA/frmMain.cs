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
using BioBaseCLIA;
using Localization;

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
        ComponentResourceManager resources = new ComponentResourceManager(typeof(frmMain));
        /// <summary>
        /// 缺液信息状态
        /// </summary>
        public static int[] LackLq;
        /// <summary>
        /// 缺管信息状态 缺管：0 有管：1
        /// </summary>
        public static int LackTube=-1;
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
        // 缺液3分钟暂停加样
        private const int MaxBuffertime = 9;//磁珠清洗液报警最大次数
        private const int MaxWashtime = 9;//探针清洗液报警最大次数
        private const int MaxWastetime = 9;//废液报警最大次数
        private const int MaxWTubetime = 9;//废管报警最大次数
        private int NumWTubettime = 0;//报警指令连续发送废管满的次数
        Thread QueryThread;
        /// <summary>
        /// 查询温度
        /// 温育盘，清洗盘，清洗管路，底物管路
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
        DateTime _BootUpTime;
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
            _BootUpTime = DateTime.Now;
            label2.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            toolTip1.SetToolTip(this.dbtnBuffer,GetString("dbtnBuffer.tip"));
            toolTip1.SetToolTip(this.dbtnWash, GetString(" dbtnWash.tip") );
            toolTip1.SetToolTip(this.dbtnWaste, GetString("dbtnWaste.tip") );
            toolTip1.SetToolTip(this.dbtnSubstract, GetString("dbtnSubstract.tip") );
            toolTip1.SetToolTip(this.dbtnRegent, GetString("dbtnRegent.tip") );
            toolTip1.SetToolTip(this.dbtnRack, GetString("dbtnRack.tip") );
            toolTip1.SetToolTip(this.btnWasteRack, GetString("btnWasteRack.tip") );
            toolTip1.SetToolTip(this.dbtnLog, GetString("dbtnLog.tip") );

            if (NetCom3.isConnect)
            {
                dbtnConnect.Enabled = false;
                toolTip1.SetToolTip(this.dbtnConnect, GetString("dbtnConnect.connecttip "));
            }
            else
            {
                dbtnConnect.Enabled = true;
                toolTip1.SetToolTip(this.dbtnConnect, GetString("dbtnConnect.disconnecttip"));
                fbtnTest.Enabled = false;
                fbtnMaintenance.Enabled = false;
            }
            RangeWY[1] = Convert.ToDecimal(OperateIniFile.ReadInIPara("temperature", "MaxTWY"));
            RangeWY[0] = Convert.ToDecimal(OperateIniFile.ReadInIPara("temperature", "MinTWY"));
            RangeWash[1] = Convert.ToDecimal(OperateIniFile.ReadInIPara("temperature", "MaxTWash"));
            RangeWash[0] = Convert.ToDecimal(OperateIniFile.ReadInIPara("temperature", "MinTWash"));
            RangeSubstrate[1] = Convert.ToDecimal(OperateIniFile.ReadInIPara("temperature", "MaxTSubstrate"));
            RangeSubstrate[0] = Convert.ToDecimal(OperateIniFile.ReadInIPara("temperature", "MinTSubstrate"));
            RangeQXGL[1] = Convert.ToDecimal(OperateIniFile.ReadInIPara("temperature", "MaxTQXGL"));
            RangeQXGL[0] = Convert.ToDecimal(OperateIniFile.ReadInIPara("temperature", "MinTQXGL"));

            frmLogShow.btnLogColor1 += new Action<int>(LogBtnColorChange);
            #region 查询今天错误中是否有未解决的问题，有的话按钮颜色变色
            List<string> lstFiles = GetFiles(Application.StartupPath + @"\Log\AlarmLog", ".txt");
            foreach (string lstFile in lstFiles)
            {
                if (lstFile.Length > 13 || lstFile.Substring(1,8)!=DateTime.Now.ToString("yyyyMMdd"))
                    continue;
                string fileInfo = ReadTxtWarn.ReaderFile(Application.StartupPath + @"\Log\AlarmLog" + "\\" + lstFile);//all text
                if (fileInfo.IndexOf(GetString("NotRead")) > -1)
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
            btnWasteRack.Location = new Point((newx - controlWidth*5 - 20), warnControlL_Y);
            dbtnWaste.Location = new Point((newx - controlWidth * 6 - 20), warnControlL_Y);
            dbtnWash.Location = new Point((newx - controlWidth * 7 - 20), warnControlL_Y);
            dbtnBuffer.Location = new Point((newx - controlWidth * 8 - 20), warnControlL_Y);
            temperatureButton.Location = new Point((newx - controlWidth * 9 - 20), warnControlL_Y);
            LackLq = new int[] { 0, 0, 0, 0 };
            frmSupplyStatus.btnBtnColor += new Action<int, int, int>(RegenColorChange);
            new Thread(new ParameterizedThreadStart((obj) =>
            {
                NetCom3.Instance.ReceiveHandelForQueryTemperatureAndLiquidLevel += 
                    new Action<string>(Instance_ReceiveHandel);//更改注册事件
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
            Selectlist = new List<string>();
            QueryThread = new Thread(new ParameterizedThreadStart(Instance_QueryInfo));
            QueryThread.IsBackground = true;
            QueryThread.Start();
            #region 设置按钮控件查询状态timer的属性
            timerStatus.Start();
            #endregion
            timeWarnSound.Start();
            #region 区分出normal、admin 、root权限
            switch (frmParent.LoginUserType) 
            {
                case "0":
                    fbtnSet.Enabled = true;
                    break;
                case "1":
                    fbtnSet.Enabled = true;
                    break;
                case "2":
                    fbtnSet.Enabled = false;
                    break;
            }
            #endregion

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
                if (OperateIniFile.ReadConfig(cTempIniPath).Rows.Count != 80)
                {
                    File.Delete(cTempIniPath);
                    frmMsgShow.MessageShow(GetString("Tips"),GetString("Abnormalexit") );
                    return;
                }
                File.Delete(defaultIniPath);
                File.Move(cTempIniPath, defaultIniPath);
            }
            else
            {
                //提醒检测到非正常退出，请清空温育盘
                frmMsgShow.MessageShow(GetString("Tips"), GetString("Abnormalexit"));
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
        }

        object locker = new object();
        /// <summary>
        /// 接收返回指令
        /// </summary>
        /// <param name="obj"></param>
        void Instance_ReceiveHandel(string obj)
        {
            SetCultureInfo();
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
        /// 发送查询指令  
        /// </summary>
        /// <param name="TubeInfo"></param>
        void Instance_QueryInfo(object TubeInfo)
        {
            bool iswork = false;
            List<string> list;
            bool diagFlag = false;
            while (true)
            {
                Thread.Sleep(100);
                if (Selectlist.Count > 0 && !iswork && LiquidQueryFlag)
                {
                    iswork = true;
                    list = Selectlist.GetRange(0, Selectlist.Count);
                    Selectlist.Clear();
                    while (list.Count>0)
                    {
                        Thread.Sleep(50);
                        while (NetCom3.Instance.iapIsRun) 
                            Thread.Sleep(1000);
                        if (NetCom3.isConnect && list[0] != null && NetCom3.isConnect/* && NetCom3.Instance.FReciveCallBack < 3*/)
                        {
                            if (frmWorkList.RunFlag==(int)RunFlagStart.IsRuning )
                            {
                                if (frmWorkList.BQLiquaid)
                                {
                                    while (!NetCom3.totalOrderFlag)
                                        Thread.Sleep(50);
                                    NetCom3.Instance.Send(NetCom3.Cover(list[0]),5);
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
            while (!this.IsHandleCreated)
            {
                Thread.Sleep(30);
            }
            defineButton1.Enabled = true;
            defineButton1.BackgroundImage = Properties.Resources.blue_play_128px_569342_easyicon_net;
            defineButton2.BackgroundImage = Properties.Resources.blue_pause_128px_569341_easyicon_net;
            defineButton3.BackgroundImage = Properties.Resources.blue_stop_play_back_128px_569353_easyicon_net;
        }
      
        private void button11_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(GetString("ExitTip"), GetString("Tips"), MessageBoxButtons.OKCancel,MessageBoxIcon.Warning);
            if (result == DialogResult.OK)
            {
                timerStatus.Stop();
                Application.Exit();
                System.Environment.Exit(0);
            }
        }

        private void defineButton1_Click(object sender, EventArgs e)
        {
            LogFile.Instance.Write(DateTime.Now + "pauseFlag的值为:" + pauseFlag+",btnRunClick的是否为空:" + (btnRunClick == null?"NULL":"NotNUll"));
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
                frmWorkList.btnRunStatus -= new Action(RunBtnStatus);
                frmWorkList.btnRunStatus += new Action(RunBtnStatus);
                frmWorkList.dbtnRackStatus -= new Action(dbtnRackStatus);
                frmWorkList.dbtnRackStatus += new Action(dbtnRackStatus);

                LogFile.Instance.Write("btnRunClick委托个数:" + btnRunClick.GetInvocationList().Count());
                while (btnRunClick != null && btnRunClick.GetInvocationList().Length > 1)//保证只有一个委托 
                {
                    btnRunClick -= (Action<object, EventArgs>)btnRunClick.GetInvocationList()[0];
                }
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
        public static bool StartFlag;
        void BoolStart(object sender, ElapsedEventArgs e)
        {
            Thread.Sleep(1000);
            if (!StartFlag)
            {
                Action ac = new Action(RunBtnStatus);
                this.Invoke(ac); 
            }
        }
        private void defineButton2_Click(object sender, EventArgs e)
        {
            if (frmWorkList.RunFlag != (int)RunFlagStart.IsRuning)
            {
                MessageBox.Show(GetString("NotRun"), GetString("Tips"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (pauseFlag == true)
            {
                MessageBox.Show(GetString("Paused"), GetString("Tips"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            if (frmWorkList.RunFlag != (int)RunFlagStart.IsRuning)
            {
                MessageBox.Show(GetString("NotRun") , GetString("Tips"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (btnStopClick != null)
            {
                DialogResult dr =MessageBox.Show(GetString("NotRun"), GetString("Tips"), MessageBoxButtons.OKCancel,MessageBoxIcon.Warning);
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
                    DbHelperOleDb.ExecuteSql(3,@"update tbReagent set leftoverTestR1 =" + reagentIniInfo.LeftReagent1 + ",leftoverTestR2 = " + reagentIniInfo.LeftReagent2 +
                                              ",leftoverTestR3 = " + reagentIniInfo.LeftReagent3 + ",leftoverTestR4 = " + reagentIniInfo.LeftReagent4 + " where BarCode = '"
                                                  + reagentIniInfo.BarCode + "' and ReagentName = '" + reagentIniInfo.ItemName + "'");
                }
            }
            #endregion
            #region 将底物配置文件信息更新到数据库
            string sbCode1 = OperateIniFile.ReadIniData("Substrate1", "BarCode", "0", iniPathSubstrateTube);
            string sbNum1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", iniPathSubstrateTube);
            DbHelperOleDb.ExecuteSql(3,@"update tbSubstrate set leftoverTest =" + sbNum1 + " where BarCode = '"
                                                  + sbCode1 + "'");
            #endregion
        }
        private void defineButton3_MouseLeave(object sender, EventArgs e)
        {
            if (defineButton3.BackgroundImage != Properties.Resources.blue_stop_play_back_128px_569353_easyicon_net) 
            {
                defineButton3.BackgroundImage = Properties.Resources.blue_stop_play_back_128px_569353_easyicon_net;
            }
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
            frmInfo frmIF = new frmInfo();
            frmIF.MdiParent = this;//指定当前窗体为顶级Mdi窗体
            frmIF.Parent = this.pnlPublic;//指定子窗体的父容器为
            frmIF.Show();
        }

        private void dbtnSubstract_MouseClick(object sender, MouseEventArgs e)
        {
            SetCultureInfo();
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
        private int RtSumTubeNum=0;
        /// <summary>
        /// 底物剩余数
        /// </summary>
        private int RtSubstract=-1;
        /// <summary>
        /// 试剂信息
        /// </summary>
        private List<ReagentIniInfo> RtlisRIinfo=new List<ReagentIniInfo>();
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
            SetCultureInfo();

            label2.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
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
            string LeftCount2 = "0";
            if (LeftCount1 == "" || LeftCount2 == "")
            {
                if (LeftCount1 == "")
                    LeftCount1 = "0";
                if (LeftCount2 == "")
                    LeftCount2 = "0";
            }
            if (int.Parse(LeftCount1) + int.Parse(LeftCount2) != RtSubstract)
            {
                if (int.Parse(LeftCount1) + int.Parse(LeftCount2) <= WarnSubstrate)
                {
                    dbtnSubstract.BackgroundImage = Properties.Resources._06;
                    if (int.Parse(LeftCount1) + int.Parse(LeftCount2) <= ErrorSubstrate)
                    {
                        dbtnSubstract.BackgroundImage = Properties.Resources._07;
                        string s= GetString("Error");
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + GetString("Error") + " *** " + GetString("NotRead") + " *** " +GetString("Substratesurplus")
                        + (int.Parse(LeftCount1) + int.Parse(LeftCount2)).ToString());
                        LogBtnColorChange(0);
                    }
                    else
                    {
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " +GetString("Warning")  + " *** " + GetString("NotRead") + " *** " + GetString("Substratesurplus")
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
                List<ReagentIniInfo> list = lisRIinfo.FindAll(ty => ty.ItemName == ReagentIniInfo.ItemName);
                if (list.Count > 0)
                    RtlisRIinfoC.Add(ReagentIniInfo);
            }
            RtlisRIinfo = RtlisRIinfoC;
            #endregion
            dbtnRegent.BackgroundImage = Properties.Resources._14__2_;
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
                            count += li.LeftReagent1;
                            if (li.LeftReagent1 == 0 )
                                dbtnRegent.BackgroundImage = Properties.Resources._12__2_;
                        }
                        if (count < ErrorReagent)
                        {
                            dbtnRegent.BackgroundImage = Properties.Resources._13__2_;
                            if (RtlisRIinfo.Find(ty => ty.ItemName == ReagentIniInfo.ItemName) != null && RtlisRIinfo.Find(ty => ty.ItemName == ReagentIniInfo.ItemName).LeftReagent1 == count)
                                continue;
                            LogBtnColorChange(0);
                            LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + GetString("Error") + " *** " + GetString("NotRead") + " *** " + ReagentIniInfo.ItemName +GetString("Lefttests") + count.ToString());
                        }
                        listItemName.Add(ReagentIniInfo.ItemName);
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
                if (this.ActiveControl.Text == "frmSupplyStatus")
                {
                    frmSupplyStatus f = (frmSupplyStatus)this.ActiveControl;
                    f.frmSupplyStatus_Load();
                }
            }
            #region 温度监控 //2018-07-5
            TimeSpan ts = DateTime.Now - _BootUpTime;
            if (!Selectlist.Contains("EB 90 11 04 04"))
                Selectlist.Add("EB 90 11 04 04");
            if (!Selectlist.Contains("EB 90 11 05 04"))
                Selectlist.Add("EB 90 11 05 04");
            if (!Selectlist.Contains("EB 90 11 06 04"))
                Selectlist.Add("EB 90 11 06 04");
            if (!Selectlist.Contains("EB 90 11 07 04"))
                Selectlist.Add("EB 90 11 07 04");
            #endregion
            timerStatus.Enabled = true;
        }
        /// <summary>
        /// 处理接收信息信息 
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
        /// 处理查询液位信息 
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

        /// <summary>
        /// 显示液位信息
        /// </summary>
        public void ShowLiquidInfo()
        {
            if (LackLq[0] > 0)
            {
                if (!BWarn)
                {
                    Selectlist.Add("EB 90 11 09 00");
                    BWarn = true;
                }
                //错误存储到Log文件
                if (LackLq[0] > MaxBuffertime)
                {
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + GetString("Error") + " *** " + GetString("NotRead") + " *** " +GetString("Cleaningfluidempty") );
                    dbtnBuffer.BackgroundImage = Properties.Resources._2;//黄色（红色为_2）
                    LogBtnColorChange(0);
                    StopFlag[0] = true;
                }
                else
                {
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + GetString("Warning") + " *** " + GetString("NotRead") + " *** " + GetString("Cleaningfluidempty"));
                    dbtnBuffer.BackgroundImage = Properties.Resources._3;//黄色（红色为_2）
                    LogBtnColorChange(1);
                }
                WarnTime++;
            }
            else
            {
                if (BWarn && (LackLq[0] == 0 && LackLq[1] == 0 && LackLq[2] == 0 && LackLq[3] == 0))
                {
                    Selectlist.Add("EB 90 11 09 01");
                    BWarn = false;
                }
                dbtnBuffer.BackgroundImage = Properties.Resources._1;//蓝色
                StopFlag[0] = false;
            }
            if (LackLq[1] > 0)
            {
                if (!BWarn)
                {
                    Selectlist.Add("EB 90 11 09 00");
                    BWarn = true;
                }
                //错误存储到Log文件
                if (LackLq[1] > MaxWashtime)
                {
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + GetString("Error") + " *** " + GetString("NotRead") + " *** " + GetString("Probefluidempty"));
                    dbtnWash.BackgroundImage = Properties.Resources._7;//红色（红色为_2）
                    LogBtnColorChange(0);
                    StopFlag[1] = true;
                }
                else
                {
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + GetString("Warning") + " *** " + GetString("NotRead") + " *** " +GetString("Probefluidempty") );
                    dbtnWash.BackgroundImage = Properties.Resources._6__2_;//黄色（红色为_7）
                    LogBtnColorChange(1);
                }
                WarnTime++;
            }
            else
            {
                if (BWarn && (LackLq[0] == 0 && LackLq[1] == 0 && LackLq[2] == 0 && LackLq[3] == 0))
                {
                    Selectlist.Add("EB 90 11 09 01");
                    BWarn = false;
                }
                dbtnWash.BackgroundImage = Properties.Resources._8;//蓝色
                StopFlag[1] = false;
            }
            if (LackLq[2] > 0)
            {
                //错误
                //错误存储到Log文件
                if (!BWarn)
                {
                    Selectlist.Add("EB 90 11 09 00");
                    BWarn = true;
                }

                if (LackLq[2] > MaxWastetime)
                {
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + GetString("Error") + " *** " + GetString("NotRead") + " *** " +GetString("Wastefull") );
                    dbtnWaste.BackgroundImage = Properties.Resources._10;//黄色（红色为_10）
                    LogBtnColorChange(0);
                    StopFlag[2] = true;
                }
                else
                {
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + GetString("Warning") + " *** " + GetString("NotRead") + " *** " + GetString("Wastefull"));
                    dbtnWaste.BackgroundImage = Properties.Resources._11;//黄色（红色为_10）
                    LogBtnColorChange(1);
                }
                WarnTime++;
            }
            else
            {
                if (BWarn && (LackLq[0] == 0 && LackLq[1] == 0 && LackLq[2] == 0 && LackLq[3] == 0))
                {
                    Selectlist.Add("EB 90 11 09 01");
                    BWarn = false;
                }
                dbtnWaste.BackgroundImage = Properties.Resources._9;//蓝色
                StopFlag[2] = false;
            }
            if (LackLq[3] > 0)
            {
                //错误存储到Log文件
                if (LackLq[3] > MaxWTubetime)
                {
                    btnWasteRack.BackgroundImage = Properties.Resources.WasteRack01;//红色
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + GetString("Error") + " *** " + GetString("NotRead") + " *** " +GetString("Wastetubefull") );
                    LogBtnColorChange(0);
                    StopFlag[3] = true;
                }
                else
                {
                    btnWasteRack.BackgroundImage = Properties.Resources.WasteRack03;//黄色
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + GetString("Warning") + " *** " + GetString("NotRead") + " *** " + GetString("Wastetubefull"));
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
        /// 处理查询温度信息
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
                frmLS.BTrefresh_Click();
                frmLS.BringToFront();
            }
        }

        private void dbtnBuffer_Click(object sender, EventArgs e)
        {
            if (LackLq[0] > 0)
            {
                //错误存储到Log文件
                LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + GetString("Warning") + " *** " + GetString("NotRead") + " *** " +GetString("Cleaningfluidempty") );
                dbtnBuffer.BackgroundImage = Properties.Resources._3;//黄色（红色为_2）
                LogBtnColorChange(1);
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    SetCultureInfo();
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow(GetString("Tips") ,GetString("Cleaningfluidempty"));
                }))
                { IsBackground = true }.Start();
            }
            else
            {
                dbtnBuffer.BackgroundImage = Properties.Resources._1;//蓝色
                //取消错误
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    SetCultureInfo();
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow(GetString("Tips"), GetString("Cleaningfluidnormal"));
                }))
                { IsBackground = true }.Start();
            }
        }

        private void dbtnWash_Click(object sender, EventArgs e)
        {
            if (LackLq[1] > 0)
            {
                //错误存储到Log文件
                LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + GetString("Warning") + " *** " + GetString("NotRead") + " *** " + GetString("Probefluidempty"));
                dbtnWash.BackgroundImage = Properties.Resources._6__2_;//黄色（红色为_7）
                LogBtnColorChange(1);
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    SetCultureInfo();
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow(GetString("Tips"),GetString("Probeempty"));
                })) { IsBackground = true }.Start();
            }
            else
            {
                //错误
                dbtnWash.BackgroundImage = Properties.Resources._8;//蓝色
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    SetCultureInfo();
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow(GetString("Tips"), GetString("Probenormal"));
                })) { IsBackground = true }.Start();
            }
        }

        private void dbtnWaste_Click(object sender, EventArgs e)
        {
            if (LackLq[2] > 0)
            {
                //错误存储到Log文件
                LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + GetString("Warning") + " *** " + GetString("NotRead") + " *** " + GetString("Wastefull"));
                dbtnWaste.BackgroundImage = Properties.Resources._11;//黄色（红色为_10）
                LogBtnColorChange(1);
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    SetCultureInfo();
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow(GetString("Tips"), GetString("Wastefull"));
                })) { IsBackground = true }.Start();
            }
            else
            {
                dbtnWaste.BackgroundImage = Properties.Resources._9;//蓝色
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    SetCultureInfo();
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow(GetString("Tips"), GetString("Wastenormal"));
                })) { IsBackground = true }.Start();
            }
        }
        /// <summary>
        /// 更改管架、底物、试剂按钮颜色。 
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
            if (CheckFormIsOpen("frmInstruMaintenance"))
            {
                frmInstruMaintenance frmIM = (frmInstruMaintenance)Application.OpenForms["frmInstruMaintenance"];
                frmIM.Show();
                frmIM.BringToFront();
                return;
            }

            frmInstruMaintenance frmim = new frmInstruMaintenance();
            frmim.MdiParent = this;//指定当前窗体为顶级Mdi窗体
            frmim.Parent = this.pnlPublic;//指定子窗体的父容器为
            frmim.Show();
        }

        /// <summary>
        /// 查询系统温度，改变标签状态，同时更改运行指示参数
        /// </summary>
        /// <param name="bo">是否弹窗提示</param>
        private void alarmOfTemperature(bool bo,string Ttype)
        {
            if (Temprrature[0] == 0 || Temprrature[1] == 0 || Temprrature[2] == 0 || Temprrature[3] == 0)
            {
                BeginInvoke(new Action(() => toolTip1.SetToolTip(temperatureButton, GetString("Temperaturewarning"))));
                return;
            }
            if (Temprrature[0] < RangeWY[0] || Temprrature[0] > RangeWY[1] || Temprrature[1] < RangeWash[0] || Temprrature[1] > RangeWash[1] || Temprrature[3] < RangeSubstrate[0] || Temprrature[3] > RangeSubstrate[1] || Temprrature[2] < RangeQXGL[0] || Temprrature[2] > RangeQXGL[1])//后期需要更改标准,包括下面的部分
            {
                StringBuilder st = new StringBuilder();
                StringBuilder st2 = new StringBuilder();
                st.Append(GetString("Temperaturewarning") +":");
                st2.Append(" ");
                int i = st.Length;
                i = st2.Length;
                if (Ttype.Contains("04")&&(Temprrature[0] < RangeWY[0] || Temprrature[0] > RangeWY[1]))
                {
                    decimal Temp= Temprrature[0];
                    if (Temp > 55)
                        Temp = 55;
                    st.Append(GetString("Incubationtemperature"));
                    st2.Append(GetString("Incubation" + Temp.ToString() +GetString("Temperaturesign")));
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + 
                        " *** " +GetString("Err") + " *** " +GetString("NotRead") + 
                        " *** " + GetString("Incubationtemperature")+ GetString("Notstandardtemperature")+
                        " " + Temp.ToString() +GetString("Temperaturesign"));
                }
                if (Ttype.Contains("05") && (Temprrature[1] < RangeWash[0] || Temprrature[1] > RangeWash[1]))
                {
                    decimal Temp = Temprrature[1];
                    if (Temp > 55)
                        Temp = 55;
                    if (st.Length > 5)
                    {
                        st.Append("、"+  GetString("Cleantemperature") );
                        st2.Append("，" +GetString("Clean") + Temp.ToString() + GetString("Temperaturesign"));
                    }
                    else
                    {
                        st.Append( GetString("Cleantemperature"));
                        st2.Append(GetString("Clean") + Temp.ToString() + GetString("Temperaturesign"));
                    }
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " +
                        GetString("Err") + " *** " + GetString("NotRead") + " *** " +
                        GetString("Cleantemperature") + GetString("Notstandardtemperature") + "：" + Temp.ToString() + GetString("Temperaturesign"));
                }
                if (Ttype.Contains("07") && (Temprrature[3] < RangeSubstrate[0] || Temprrature[3] > RangeSubstrate[1]))
                {
                    decimal Temp = Temprrature[3];
                    if (Temp > 55)
                        Temp = 55;
                    if (st.Length > 5)
                    {
                        st.Append("、" + GetString("Substratetemperature"));
                        st2.Append("，"+ GetString("Substrate") + Temp.ToString() + GetString("Temperaturesign"));
                    }
                    else
                    {
                        st.Append(GetString("Substratetemperature"));
                        st2.Append(GetString("Substrate") + Temp.ToString() + GetString("Temperaturesign"));
                    }
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + GetString("Err") +
                        " *** " + GetString("NotRead") + " *** " + GetString("Substrate") + GetString("Notstandardtemperature") +"：" +
                        Temp.ToString() + GetString("Temperaturesign"));
                }
                if (Ttype.Contains("06") && (Temprrature[2] < RangeQXGL[0] || Temprrature[2] > RangeQXGL[1]))
                {
                    decimal Temp = Temprrature[2];
                    if (Temp > 55)
                        Temp = 55;
                    if (st.Length > 5)
                    {
                        st.Append("、"+ GetString("Pipelinetemperature"));
                        st2.Append("，"+ GetString("Pipeline") + Temp.ToString() + GetString("Temperaturesign"));
                    }
                    else
                    {
                        st.Append(GetString("Pipelinetemperature"));
                        st2.Append(GetString("Pipeline")+ Temp.ToString() + GetString("Temperaturesign"));
                    }
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + GetString("Err") +
                        " *** " + GetString("NotRead") + " *** " + GetString("Pipeline") + GetString("Notstandardtemperature") + "：" +
                        Temp.ToString() + GetString("Temperaturesign"));
                }
                st.Append(GetString("Notstandardtemperature"));
                temperatureButton.BackgroundImage = Properties.Resources.temperature_2;
                BeginInvoke(new Action(() => toolTip1.SetToolTip(temperatureButton,GetString("Temperaturewarning") +"\n" + st.ToString())));
                //此时停止机器，不允许运行
                if (bo)
                {
                    new Thread(new ParameterizedThreadStart((obj) =>
                    {
                        frmMessageShow f = new frmMessageShow();
                        f.MessageShow(GetString("Temperaturewarning"), st.ToString() + st2.ToString());
                    })) { IsBackground = true }.Start();
                    
                }
            }
            else
            {
                temperatureButton.BackgroundImage = Properties.Resources.temperature_1;
                BeginInvoke(new Action(() => toolTip1.SetToolTip(temperatureButton,GetString("temperatureButton.ToolTip"))));//2018-07-20 zlx mod
                //此时更新标志，指示可以运行
                if (bo)
                {
                    new Thread(new ParameterizedThreadStart((obj) =>
                    {
                        frmMessageShow f = new frmMessageShow();
                        f.MessageShow(GetString("Temperaturewarning"),GetString("Standardtemperature"));
                    })) { IsBackground = true }.Start();
                   
                }
            }
        }

        private void temperatureButton_Click(object sender, EventArgs e)
        {
              TimeSpan ts = DateTime.Now - _BootUpTime;
              if (ts.TotalMinutes< 30 || (Temprrature[0] == 0 && Temprrature[1] == 0 && Temprrature[2] == 0 && Temprrature[3] == 0))
              {
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
            alarmOfTemperature(true,"04,05,06,07");
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
            if (NetCom3.isConnect&&NetCom3.Instance.isHeartbeatLive)
            {
                dbtnConnect.Enabled = false;
                toolTip1.SetToolTip(this.dbtnConnect,GetString("Connect") );
            }
            else
            {
                dbtnConnect.Enabled = true;
                toolTip1.SetToolTip(this.dbtnConnect, GetString("Disconnect" ));
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
                    frmMS.MessageShow(GetString("Tips"),GetString("Unableconnect"));
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
            frmVersion frmVersion = new frmVersion();
            frmVersion.Show();
            //Type tempType = Type.GetTypeFromProgID("Shell.Application");
            //object oleObject = System.Activator.CreateInstance(tempType);
            //tempType.InvokeMember("ToggleDesktop", BindingFlags.InvokeMethod, null, oleObject, null);
        }

        private void pnlbarUP_Paint(object sender, PaintEventArgs e)
        {
        }

        private void dbtnRack_Click(object sender, EventArgs e)
        {
            if (LackTube==0)
            {
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    SetCultureInfo();
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow(GetString("Tips"),GetString("Temporarystorageempty"));
                })) { IsBackground = true }.Start();
            }
            else
            {
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    SetCultureInfo();
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow(GetString("Tips"),GetString("Temporarystorage"));
                })) { IsBackground = true }.Start();
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
        bool Iswarn = false;
        private void timeWarnSound_Tick(object sender, EventArgs e)
        {
            if (SoundFlag == (int)SoundFlagStart.isClose) return;
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
        }
        #endregion

        private void btnWasteRack_Click(object sender, EventArgs e)
        {
            if (LackLq[3] > 0)
            {
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    SetCultureInfo();
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow(GetString("Tips"),GetString("Wastepipe") );
                })) { IsBackground = true }.Start();
            }
            else
            {
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    SetCultureInfo();
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow(GetString("Tips"), GetString("Wastepipenormal") );
                })) { IsBackground = true }.Start();
            }
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            //root权限可以通过帮助进行最小化
            if (frmLogin.LoginUserType != "0")
            {
            }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
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
                BeginInvoke(new Action(() => toolTip1.SetToolTip(this.dbtnConnect, GetString("dbtnConnect.connecttip"))));
                return;
            }

            dbtnConnect.BackgroundImage = Properties.Resources.已连接;
            BeginInvoke(new Action(() => toolTip1.SetToolTip(this.dbtnConnect, GetString("dbtnConnect.disconnecttip"))));
        }

        private string GetString(string key)
        {
            return resources.GetString(key);
        }
        /// <summary>
        /// 设置语言环境
        /// </summary>
        private void SetCultureInfo()
        {
            Language.AppCultureInfo = new System.Globalization.CultureInfo(GetCultureInfo());
            //Language.AppCultureInfo = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentCulture = Language.AppCultureInfo;
            System.Threading.Thread.CurrentThread.CurrentUICulture = Language.AppCultureInfo;
        }
        private string GetCultureInfo()
        {
            if (OperateIniFile.ReadInIPara("CultureInfo", "Culture") == "en")
            {
                return "en";
            }

            return "zh-CN";
        }
    }

    /// <summary>
    /// 声音报警状态 isClose-禁止状态 IsOpen-开启状态 isStarTime=正在报警
    /// </summary>
    public enum SoundFlagStart { isClose = 0, IsOpen = 1, isStarTime = 2 }
}
