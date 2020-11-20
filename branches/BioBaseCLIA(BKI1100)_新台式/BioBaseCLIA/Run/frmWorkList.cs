using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using BioBaseCLIA.CalculateCurve;
using BioBaseCLIA.InfoSetting;
using Common;
using Maticsoft.DBUtility;

namespace BioBaseCLIA.Run
{
    public partial class frmWorkList : frmParent
    {
        #region 界面显示与进度计算相关变量
        /// <summary>
        /// 根据不同情况更改主界面运行按钮状态
        /// </summary>
        public static event Action btnRunStatus;
        /// <summary>
        /// 根据不同情况更改管架按钮颜色显示 2019-03-12 zlx add
        /// </summary>
        public static event Action dbtnRackStatus;
        /// <summary>
        /// 样本试剂盘自定义控件更新
        /// </summary>
        public static event Action SpDiskUpdate;
        /// <summary>
        /// 报警时更改主界面日志按钮的颜色
        /// </summary>
        //public static event Action<int> btnLogColor;
        /// <summary>
        /// 实验每个步骤的信息
        /// </summary>
        List<TestMethod> lisTestMethod = new List<TestMethod>();
        /// <summary>
        /// TestMethod 新建一个实例
        /// </summary>
        TestMethod testmethod = new TestMethod();
        /// <summary>
        /// 各个步骤实验进度,最终结果按照实验运行顺序排序
        /// </summary>
        List<TestSchedule> lisTestSchedule = new List<TestSchedule>();

        /// <summary>
        /// TestSchedule 新建一个实例
        /// </summary>
        TestSchedule testschedule = new TestSchedule();
        /// <summary>
        /// 用于datagridview控件绑定的集合
        /// </summary>
        static IList<TestItem> lisTestItem = new List<TestItem>();

        /// <summary>
        /// 绑定类型的列表，便于作为datagridview的数据源时进行增删改查
        /// </summary>
        static BindingList<TestItem> BTestItem = new BindingList<TestItem>(lisTestItem);
        /// <summary>
        /// 新建一个所做实验项目的实例
        /// </summary>
        TestItem testItem = new TestItem();
        /// <summary>
        /// 进度条控件序列
        /// </summary>
        List<Bar.ProgressBar> lisProBar = new List<Bar.ProgressBar>();

        Bar.ProgressBar proBar = new Bar.ProgressBar();
        /// <summary>
        /// 添加控件事件
        /// </summary>
        public event Action<bool> EventControlAdd;

        Action<string, int> TestStatusInfo;
        /// <summary>
        /// 正在做的实验步骤
        /// </summary>
        TestSchedule DoingTestStep = new TestSchedule();
        /// <summary>
        /// 实验完成加底物数量
        /// </summary>
        private int completeTestNums = 0;
        /// <summary>
        /// 记录上一次加试剂时间
        /// </summary>
        int lastAddLiquidTime = 0;
        /// <summary>
        /// 实验总时间
        /// </summary>
        int sumTime = 0;
        /// <summary>
        /// 当前步骤的时间
        /// </summary>
        int stepTime = 0;
        /// <summary>
        /// 实际步骤运行时间是否有延迟
        /// </summary>
        bool DalayFlag = false;
        /// <summary>
        /// 模拟运行时间间隔，1000代表1s
        /// </summary>
        int timeInterval = 1000;
        /// <summary>
        /// 当前可做样本的数量
        /// </summary>
        int SampleNumCurrent = 0;
        /// <summary>
        ///定义实验结果的List列表
        /// </summary>
        public static List<TestResult> ITestResult = new List<TestResult>();
        /// <summary>
        /// 存储显示在实验结果界面的数据
        /// </summary>
        public static BindingCollection<TestResult> BTestResult = new BindingCollection<TestResult>();
        /// <summary>
        /// 存储实验结果数据
        /// </summary>
        public static BindingCollection<TestResult> TemporaryTestResult = new BindingCollection<TestResult>();
        /// <summary>
        /// 实验结果
        /// </summary>
        public static TestResult testResult = new TestResult();
        /// <summary>
        /// 已经结束的实验项目
        /// </summary>
        List<TestItem> lisTiEnd = new List<TestItem>();
        /// <summary>
        /// 加样时间
        /// </summary>
        int SampleTime = int.Parse(OperateIniFile.ReadInIPara("Time", "sampleTime"));
        /// <summary>
        /// 加试剂时间
        /// </summary>
        int RegentTime = int.Parse(OperateIniFile.ReadInIPara("Time", "RegentTime"));
        /// <summary>
        /// 加磁珠时间
        /// </summary>
        int beadTime = int.Parse(OperateIniFile.ReadInIPara("Time", "beadTime"));
        /// <summary>
        /// 清洗时间
        /// </summary>
        int washTime = int.Parse(OperateIniFile.ReadInIPara("Time", "washTime"));
        /// <summary>
        /// 加底物时间
        /// </summary>
        int substrateTime = int.Parse(OperateIniFile.ReadInIPara("Time", "substrateTime"));
        /// <summary>
        /// 读数时间
        /// </summary>
        int readTime = int.Parse(OperateIniFile.ReadInIPara("Time", "readTime"));
        /// <summary>
        /// 稀释时间。LYN add 20171114
        /// </summary>
        int dilutionTime =0;
        /// <summary>
        /// 底物剩余测数
        /// </summary>
        int substrateNum1 = 0;
        int substrateNum2 = 0;
        /// <summary>
        /// 弹窗提示实例
        /// </summary>
        frmMessageShow frmMsgShow = new frmMessageShow();
        /// <summary>
        /// 额外稀释体积，LYN add 20171114
        /// </summary>
        int extraDilutionVol = 100;
        /// <summary>
        /// 后弃体积
        /// </summary>
        int AbandSampleVol = 10;
        /// <summary>
        /// 温育盘起始位置。LYN add 20171114
        /// </summary>
        int ReactStartPos = 0;
        #endregion

        #region 实验运行相关变量
        /// <summary>
        /// 运行线程
        /// </summary>
        private Thread RunThread;
        /// <summary>
        /// 加液线程
        /// </summary>
        private Thread AddLiquidThread;
        /// <summary>
        /// 是否是新的清洗盘，新清洗盘的抽液和注液位置均往后一个位置，是为1，否为0
        /// </summary>
        int isNewCleanTray = 1;
        /// <summary>
        /// 已停止的实验编号
        /// </summary>
        List<string> StopList = new List<string>();
        /// <summary>
        /// 实验项目名字
        /// </summary>
        List<string> ProjectName = new List<string>();
        #region 公共
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
        //lyn add 20180611
        /// <summary>
        /// R1后弃比例
        /// </summary>
        int abanR1Pro = 10;
        /// <summary>
        /// R2后弃比例
        /// </summary>
        int abanR2Pro = 10;
        /// <summary>
        /// R3后弃比例
        /// </summary>
        int abanR3Pro = 10;
        /// <summary>
        /// 稀释液后弃比例
        /// </summary>
        int abanDiuPro = 10;
        /// <summary>
        /// 磁珠后弃比例
        /// </summary>
        int abanBeadPro =10;
        /// <summary>
        /// 20个位置的稀释液体积
        /// </summary>
        string[] diuleftVol = new string[20];
        /// <summary>
        /// 移管手标志位，是否正在使用，TRUE为正在使用，false未在使用
        /// </summary>
        bool MoveTubeUseFlag = false;
        /// <summary>
        /// 清洗盘标志位，是否正在使用，TRUE为正在使用，false未在使用
        /// </summary>
        bool WashTrayUseFlag = false;
        /// <summary>
        /// 清洗盘是否正在旋转
        /// </summary>
        bool WashTurnFlag = false;
        /// <summary>
        /// 正在加样标志位
        /// </summary>
        public static bool AddingSampleFlag = false;
        /// <summary>
        /// 实验是否正在运行
        /// </summary>
        public static int RunFlag =(int)RunFlagStart.NoStart;
       
        /// <summary>
        /// 反应盘孔数
        /// </summary>
        int ReactTrayHoleNum = int.Parse(OperateIniFile.ReadInIPara("OtherPara", "ReactTrayHoleNum"));
        /// <summary>
        /// 最大稀释倍数（针最小吸液量为5，反应管最大体积为600）
        /// </summary>
        int DiuMaxTimes = 10; //2018 zlx add
        /// <summary>
        /// 最小吸液量
        /// </summary>
        int ImbibitionMin = 5;
        /// <summary>
        /// 稀释总体积 2018-06-01 zlx add
        /// </summary>
        int _diuSumVol = 100;
        /// <summary>
        /// 稀释液获取不到的体积/ul 2019-04-12 zlx add
        /// </summary>
        int DiuNoUsePro = 500;
        /// <summary>
        /// 稀释样本后弃体积
        /// </summary>
        int DiuLeftVol = 40;
        /// <summary>
        /// 试剂获取不到的体积/ul 2019-04-12 zlx add
        /// </summary>
        int RegentNoUsePro = 200;
        /// <summary>
        /// 反应盘待使用空白反应管个数
        /// </summary>
        int toUsedTube = 5;//modify y 20180522 10=>15
        /// <summary>
        /// 未开始进行实验的反应管的id
        /// </summary>
        int NoStartTestId = 1;
        /// <summary>
        /// 是否添加急诊
        /// </summary>
        public static bool EmergencyFlag = false;
        /// <summary>
        /// 是否加普通样本
        /// </summary>
        public static bool addOrdinaryFlag = false;
        /// <summary>
        /// 实验运行的下一步骤
        /// </summary>
        TestSchedule TestStep;
        #endregion
        #region 清洗相关
        /// <summary>
        /// 清洗线程timer
        /// </summary>
        System.Timers.Timer timer = new System.Timers.Timer();
        /// <summary>
        /// 清洗盘是否放入第一个反应管
        /// </summary>
        bool FirstTubeWash = false;
        /// <summary>
        /// 存储清洗盘反应管状态
        /// </summary>
        DataTable dtWashTrayTubeStatus = new DataTable();
        /// <summary>
        /// 存储需进行取放管的反应管信息
        /// </summary>
        List<MoveTubeStatus> lisMoveTube = new List<MoveTubeStatus>();
        MoveTubeStatus moveTube = new MoveTubeStatus();
        /// <summary>
        /// 夹管线程
        /// </summary>
        private Thread MoveTubeThread;
        /// <summary>
        /// 读数线程
        /// </summary>
        private Thread ReadThread;
        /// <summary>
        /// 清洗线程
        /// </summary>
        private Thread washThread;
        /// <summary>
        /// 第一次清洗的结束时间
        /// </summary>
        List<int> W1EndTime = new List<int>();
        /// <summary>
        /// 清洗盘取放管位置当前孔号
        /// </summary>
        int currentHoleNum = 1;
        /// 稀释使用反应管数。LYN add 20171114
        int DiuTubeNum = 0;
        /// <summary>
        /// 下位机返回数据
        /// </summary>
        string[] dataRecive = new string[16];
        /// <summary>
        /// 暂时存储未处理的读数数据
        /// </summary>
        Queue<string> DataReciveNumberRead = new Queue<string>();
        #endregion
        #endregion
        #region 结果计算
        /// <summary>
        /// 吸光度
        /// </summary>
        int PMT = 0;
        /// <summary>
        /// 浓度
        /// </summary>
        string concentration = "";
        /// <summary>
        /// 实验结果
        /// </summary>
        string result;

        string sco = "";
        /// <summary>
        /// 实验计算线程
        /// </summary>
        private Thread CaculateThread;
        /// <summary>
        /// 标准品定标液的初始信息
        /// </summary>
        public static List<ScalingInfo> lisScalingInfo;//= new List<ScalingInfo>();
        /// <summary>
        /// 标准品信息实例
        /// </summary>
        ScalingInfo scalingInfo = new ScalingInfo();
        /// <summary>
        /// 存储标准品或定标液的吸光度值
        /// </summary>
        DataTable dtScalingPMT;
        /// <summary>
        /// 标准品或定标液计算结果
        /// </summary>
        DataTable dtScalCacResult;
        #endregion

        #region 数据处理相关变量
        /// <summary>
        /// 读数下位机返回数据
        /// </summary>
        string ReadbackObj = "";
        #endregion

        #region 标志位
        /// <summary>
        /// 仪器运行指示灯标志位
        /// </summary>
        bool RunLightFlag = false;
        /// <summary>
        /// 液位开始查询标志
        /// </summary>
        public static bool BQLiquaid = false;
        /// <summary>
        /// 缺管标志  
        /// </summary>
        public static bool TubeStop = false;
        /// <summary>
        /// 向温育盘夹管失败的位置
        /// </summary>
        List<int> AddTubeStop = new List<int>();
        /// <summary>
        /// 底物缺少标志 
        /// </summary>
        public static  bool SubstrateStop = false;
        /// <summary>
        /// 温育盘满盘标志
        /// </summary>
        private bool BFullReactTray = false;
        /// <summary>
        /// 抓管出现问题标志
        /// </summary>
        private bool TubeProblemFlag = false;
        /// <summary>
        /// 是否允许清洗盘转动
        /// </summary>
        bool NoTateFlag = false;
        /// <summary>
        /// 往清洗盘抓空
        /// </summary>
        bool moveNullTubeProblemFlag = false;
        /// <summary>
        /// 是否进入实验流程
        /// </summary>
        bool EnterRun = false;
        #endregion

        /// <summary>
        /// 液位检测报警事件
        /// </summary>
        public static event Action<string, int> LiquidLevelDetectionEvent;
        public frmWorkList()
        {
            InitializeComponent();
         
            dtWashTrayTubeStatus.Columns.Add("Pos", typeof(string));
            dtWashTrayTubeStatus.Columns.Add("Value", typeof(string));
            dtWashTrayTubeStatus.Columns.Add("TestId", typeof(int));
            dtWashTrayTubeStatus.Columns.Add("StepNum", typeof(int));
            //清洗盘状态表中新添加一个字段，存放反应管在反应盘的位置。LYN add 20171114
            dtWashTrayTubeStatus.Columns.Add("reactTrayPos", typeof(int));
            //加载数据
            foreach (TestResult item in ITestResult)
            {
                BTestResult.Add(item);
            }
            if (dgvWorkListData.RowCount > 0)//2019-01-08 zlx mod
                dgvWorkListData.DataSource = null;
        }

        private void frmWorkList_Load(object sender, EventArgs e)
        {
            //2018-08-13 zlx add
            if (!frmMain.LiquidQueryFlag)
                frmMain.LiquidQueryFlag = true;
            if (!NetCom3.isConnect)
            {
                if (NetCom3.Instance.CheckMyIp_Port_Link())
                {
                    NetCom3.Instance.ConnectServer();
                }
            }
            if (NetCom3.Instance.stopsendFlag)
                NetCom3.Instance.stopsendFlag = false;
            if (!NetCom3.isConnect)
            {
                frmMsgShow.MessageShow("上下位机连接", "系统与仪器连接失败，请查看连接设置！");
                return;
            }

            #region 数据初始化及变量赋值
            lisTestMethod = new List<TestMethod>();
            lisTestSchedule = new List<TestSchedule>();
            //NetCom3.Instance.ReceiveHandel += new Action<string>(Instance_ReceiveHandel);
            #endregion
            if (BTestResult != null)
                if (BTestResult.Count > 0)
                {
                    for (int i = BTestResult.Count - 1; i >= 0; i--)//modify y 20180512
                    {
                        //BTestResult.RemoveAt(i);
                    }
                }
            if (TemporaryTestResult != null) 
            {
                TemporaryTestResult.Clear();
            }
            if (BTestItem != null)
                if (BTestItem.Count > 0)
                {
                    for (int i = BTestItem.Count - 1; i >= 0; i--)//modify y 20180512
                    {
                        BTestItem.RemoveAt(i);
                    }
                }
            DiuInfo.ReadDiuTime();
            frmMain.btnRunClick -= new Action<object, EventArgs>(TestRun);
            frmMain.btnRunClick += new Action<object, EventArgs>(TestRun);
            frmMain.btnPauseClick -= new Action(AllPause);
            frmMain.btnPauseClick += new Action(AllPause);
            frmMain.btnStopClick -= new Action(AllStop);
            frmMain.btnStopClick += new Action(AllStop);
            frmMain.btnGoonClick -= new Action(Goon);
            frmMain.btnGoonClick += new Action(Goon);
            frmSampleLoad.EmergencySample -= new Action(EmergencySampleSch);
            frmSampleLoad.EmergencySample += new Action(EmergencySampleSch);
            frmWarn.btnPauseClick += new Action(AllPause);
            frmWarn.btnGoonClick += new Action(Goon);
            NetCom3.Instance.EventStop += new Action(AllStop);

            //NetCom3.MoveTubeError += this.DisposeMoveAndAddError;//y add 20180723
            AddResetEvent.Set();//add y 20180727

        }
        /// <summary>
        /// 数据接收
        /// </summary>
        /// <param name="obj"></param>
        void Instance_ReceiveHandel(string obj)
        {
            if (obj.IsNullOrEmpty())
                return;
            else
            {
                lock (dataLocker)
                {
                    dataRecive = obj.Split(' ');
                    if (dataRecive[0] != null && dataRecive[3] == "A3" && (dataRecive[15] != "00" || dataRecive[14] != "00" || dataRecive[13] != "00"))
                    {
                        string readData = dataRecive[12] + dataRecive[13] + dataRecive[14] + dataRecive[15];
                        lock (DataReciveNumberRead)
                        {
                            DataReciveNumberRead.Enqueue(readData);
                        }
                        dataRecive[0] = null;
                    }
                }
            }
        }

        #region 进度生成

        /// <summary>
        /// 急诊样本进度生成
        /// </summary>
        /// <param name="dtEmergencySample"></param>
        void EmergencySampleSch()
        {
            BLL.tbSampleInfo bllsampleinfo = new BLL.tbSampleInfo();
            BLL.tbProject bllproject = new BLL.tbProject();
            LoadingHelper.ShowLoadingScreen();
            DataTable dtSampleInfo = new DataTable();
            dtSampleInfo.Columns.Add("SampleID", typeof(int));
            dtSampleInfo.Columns.Add("SampleType", typeof(string));
            dtSampleInfo.Columns.Add("SampleNo", typeof(string));
            dtSampleInfo.Columns.Add("RepeatCount", typeof(int));
            dtSampleInfo.Columns.Add("Position", typeof(int));
            dtSampleInfo.Columns.Add("SampleContainer", typeof(string));
            dtSampleInfo.Columns.Add("Emergency", typeof(string));
            dtSampleInfo.Columns.Add("ProjectProcedure", typeof(string));
            dtSampleInfo.Columns.Add("ShortName", typeof(string));
            #region 生成实验进度
            if (RunFlag == (int)RunFlagStart.IsRuning)
            {
                //运行中加急诊样本
                if (EmergencyFlag)
                {
                    #region 急诊样本
                    #region 获取样本信息
                    //查询添加的急诊样本
                    DbHelperOleDb db = new DbHelperOleDb(1);
                    //DataTable dtprosample = bllsampleinfo.GetList(" SendDateTime >=#" + DateTime.Now.ToString("yyyy-MM-dd")
                    //    + "#and SendDateTime <#" + DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")
                    //    + "# and Status = 0  and Emergency = 2 order by CInt([Position])").Tables[0];
                    DataTable dtprosample = bllsampleinfo.GetList(" SendDateTime >=#" + DateTime.Now.ToString("yyyy-MM-dd")
                        + "#and SendDateTime <#" + DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")
                        + "# and Status = 0  and Emergency = 2 order by  SampleNo").Tables[0];
                    db = new DbHelperOleDb(1);
                    DbHelperOleDb.ExecuteSql(1,@"update tbSampleInfo set Emergency = 3 where Status = 0 and Emergency = 2 and SendDateTime >=#"
                                      + DateTime.Now.ToString("yyyy-MM-dd") + "#and SendDateTime <#"
                                      + DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + "#");
                    db = new DbHelperOleDb(0);
                    DataTable dtproinfo = bllproject.GetList("").Tables[0];
                    for (int i = 0; i < dtprosample.Rows.Count; i++)
                    {
                        //  int count = new List<TestItem>((BindingList<TestItem>)this.dgvWorkListData.DataSource).FindAll(ty =>
                        //ty.SamplePos == Convert.ToInt32(dtprosample.Rows[i]["Position"])).Count;
                        int count = new List<TestItem>((BindingList<TestItem>)this.dgvWorkListData.DataSource).FindAll(ty =>
                            ty.SampleNo == dtprosample.Rows[i]["SampleNo"].ToString()).Count;
                        if (count != 0)
                            continue;
                        string[] projectName = dtprosample.Rows[i]["ProjectName"].ToString().TrimEnd().Split(' ');
                        for (int j = 0; j < projectName.Length; j++)
                        {
                            DataRow[] dr = dtproinfo.Select("ShortName ='" + projectName[j] + "'");
                            string projectProcedure = (string)dr[0]["ProjectProcedure"];
                            dtSampleInfo.Rows.Add(dtprosample.Rows[i]["SampleID"], dtprosample.Rows[i]["SampleType"],
                                dtprosample.Rows[i]["SampleNo"], dtprosample.Rows[i]["RepeatCount"], dtprosample.Rows[i]["Position"],
                                dtprosample.Rows[i]["SampleContainer"], 4, projectProcedure, projectName[j]);
                        }
                    }
                    #endregion
                    if (dtSampleInfo.Rows.Count == 0 || dtSampleInfo == null)
                    {
                        LoadingHelper.CloseForm();
                        EmergencyFlag = false;
                        frmAddSample.newSample = false;
                        Goon();//2018-10-18 zlx mod
                        return;
                    }
                    #region 将dataTable转换为list<TestMethod>类型
                    Dictionary<string, Type> dEnum = new Dictionary<string, Type>();
                    dEnum.Add("StepName", typeof(ExperimentStep));
                    List<TestMethod> templisTestMethod = (List<TestMethod>)ConvertHelper.DataTableConvertToListGenuric<TestMethod>(MethodInit(dtSampleInfo), dEnum);
                    #endregion
                    //合并加液步骤，对进度进行赋值
                    List<TestSchedule> templisTestSchedule = MethodMergeSchedule(templisTestMethod);
                    //对进度列表进行排序，并按照所排的顺序对TestID按照顺序赋值
                    templisTestSchedule = orderModifyTestID(templisTestSchedule);
                    int tempSamNum = templisTestSchedule.FindAll(tx => tx.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddLiquidTube).Count;
                    //查询还未开始运行的标准品和定标液数量或急诊样本数量
                    int NoStartScalNum = new List<TestItem>((BindingList<TestItem>)this.dgvWorkListData.DataSource).FindAll(ty =>
                        ty.TestID >= NoStartTestId && (ty.SampleType.Contains("标准品") || ty.SampleType.Contains("定标液"))).Count;

                    int NoStartTestID = NoStartTestId + NoStartScalNum;
                    //被延后的反应管的所在位置
                    int OldAddSamPos = lisTestSchedule.Find(ty => ty.TestID == NoStartTestID - 1 &&
                        ty.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddLiquidTube).AddSamplePos;
                    //急诊样本testID赋值
                    for (int j = 0; j < templisTestSchedule.Count; j++)
                    {
                        TestSchedule TempTestSchedule = templisTestSchedule[j];
                        TempTestSchedule.TestID = TempTestSchedule.TestID + NoStartTestID - 1;
                        if (TempTestSchedule.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddLiquidTube)
                        {
                            //稀释实验赋值
                            if (int.Parse(TempTestSchedule.dilutionTimes) > 1)
                            {
                                string[] diupos = TempTestSchedule.dilutionPos.Split('-');
                                if (diupos.Length > 1)
                                {
                                    string newpos = "";
                                    for (int i = 0; i < diupos.Length; i++)
                                    {
                                        if (newpos == "")
                                        {
                                            newpos = "1";
                                        }
                                        else
                                        {
                                            newpos += "-" + (i + 1).ToString();
                                        }
                                    }
                                    TempTestSchedule.dilutionPos = newpos;
                                }
                                else
                                {
                                    TempTestSchedule.dilutionPos = "1";
                                }
                                string[] temppos = TempTestSchedule.dilutionPos.Split('-');
                                TempTestSchedule.getSamplePos = "R" + temppos[temppos.Length - 1];
                                OldAddSamPos = OldAddSamPos + 1;
                                if (OldAddSamPos > ReactTrayHoleNum)
                                {
                                    OldAddSamPos = 4;
                                }
                                TempTestSchedule.AddSamplePos = OldAddSamPos;
                            }
                            else
                            {
                                OldAddSamPos = OldAddSamPos + 1;
                                if (OldAddSamPos > ReactTrayHoleNum)
                                {
                                    OldAddSamPos = 4;
                                }
                                TempTestSchedule.AddSamplePos = OldAddSamPos;
                                TempTestSchedule.getSamplePos = "S" + TempTestSchedule.samplePos;
                            }
                        }
                    }
                    //已生成进度条testid修改值
                    for (int i = 0; i < lisTestSchedule.Count; i++)
                    {
                        if (lisTestSchedule[i].TestID >= NoStartTestID)
                        {
                            TestSchedule TempTestSchedule = lisTestSchedule[i];
                            //还存在问题
                            TempTestSchedule.TestID = templisTestSchedule[templisTestSchedule.Count - 1].TestID - NoStartTestID + 1 + TempTestSchedule.TestID;
                            if (TempTestSchedule.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddLiquidTube)
                            {
                                if (int.Parse(TempTestSchedule.dilutionTimes) > 1)
                                {
                                    string[] diupos = TempTestSchedule.dilutionPos.Split('-');
                                    if (diupos.Length > 1)
                                    {
                                        //2018-06-09 zlx  mod
                                        string newpos = "";
                                        for (int j = 0; j < diupos.Length; j++)
                                        {
                                            if (newpos == "")
                                            {
                                                newpos = "1";
                                            }
                                            else
                                            {
                                                newpos += "-" + (j + 1).ToString();
                                            }
                                        }
                                        TempTestSchedule.dilutionPos = newpos;
                                    }
                                    else
                                    {
                                        TempTestSchedule.dilutionPos = "1";
                                    }
                                    string[] temppos = TempTestSchedule.dilutionPos.Split('-');
                                    TempTestSchedule.getSamplePos = "R" + temppos[temppos.Length - 1];
                                    OldAddSamPos = OldAddSamPos + 1;
                                    if (OldAddSamPos > ReactTrayHoleNum)
                                    {
                                        OldAddSamPos = 4;
                                    }
                                    TempTestSchedule.AddSamplePos = OldAddSamPos;
                                }
                                else
                                {
                                    OldAddSamPos = OldAddSamPos + 1;
                                    if (OldAddSamPos > ReactTrayHoleNum)
                                    {
                                        OldAddSamPos = 4;
                                    }
                                    TempTestSchedule.AddSamplePos = OldAddSamPos;
                                    TempTestSchedule.getSamplePos = "S" + TempTestSchedule.samplePos;
                                }
                            }
                        }
                    }
                    //急诊样本之前的标准品还有未运行的
                    NoStartTestID = NoStartTestID - NoStartScalNum;
                    //获取添加新的急诊样本后的当前样本数量
                    SampleNumCurrent = SampleNumCurrent + tempSamNum;
                    //templisTestSchedule.Sort(new SortRun());//按照步骤开始顺序排序
                    //将实验运行时的样本合并到已经生成进度的列表中
                    lisTestSchedule.AddRange(templisTestSchedule);
                    //按照testid进行排序
                    lisTestSchedule.Sort(new SortEmergency());
                    //进度计算
                    #region 控件清空还未开始运行的进度条控件
                    while (dgvWorkListData.Controls.Count > 2)
                    {
                        this.dgvWorkListData.Controls.Clear();//清除已有的控件
                    }
                    for (int i = BTestItem.Count - 1; i >= NoStartTestID - 1; i--)
                    {
                        BTestItem.Remove(BTestItem[i]);
                    }
                    //清空之前获取已经开始实验的进度条控件
                    for (int i = lisProBar.Count - 1; i >= NoStartTestID - 1; i--)
                    {
                        lisProBar.Remove(lisProBar[i]);
                    }

                    //JUN Add 2019-3-6 确保列表中没有已经完成的实验
                    if (RunFlag != (int)RunFlagStart.IsRuning)
                    {
                        while (dgvWorkListData.Controls.Count > 2)
                        {
                            this.dgvWorkListData.Controls.Clear();//清除已有的控件
                        }
                        for (int i = BTestItem.Count - 1; i >= 0; i--)
                        {
                            BTestItem.Remove(BTestItem[i]);
                        }
                        //清空之前获取已经开始实验的进度条控件
                        for (int i = lisProBar.Count - 1; i >= 0; i--)
                        {
                            lisProBar.Remove(lisProBar[i]);
                        }
                    }
                    #endregion
                    //将新添加的急诊进度和未开始运行的样本进度附加到datagridview中
                    BindData(lisTestSchedule.FindAll(tx => tx.TestID >= NoStartTestID), NoStartTestID);
                    #region 获取已经开始实验的样本的空闲时间
                    //获取已经开始运行的样本的进度表
                    List<TestSchedule> RunSchedule = lisTestSchedule.FindAll(tx => tx.TestID < NoStartTestID);
                    List<string> runFreeTime = GetFreeTime(RunSchedule);
                    #endregion
                    //未运行的样本进度计算
                    List<TestSchedule> lisTestNoRun = ExperimentalScheduleAlgorithm(lisTestSchedule.FindAll(tx => tx.TestID >= NoStartTestID), runFreeTime);
                    //清空之前获取已经开始实验的进度条控件
                    lisTestSchedule = lisTestSchedule.FindAll(tx => tx.TestID < NoStartTestID);
                    lisTestSchedule.AddRange(lisTestNoRun);
                    lisTestSchedule.Sort(new SortRun());
                    #endregion
                }
                else
                {
                    #region 追加普通样本
                    #region 获取样本信息
                    //查询添加的急诊样本
                    DbHelperOleDb db = new DbHelperOleDb(1);
                    //DataTable dtprosample = bllsampleinfo.GetList(" SendDateTime >=#" + DateTime.Now.ToString("yyyy-MM-dd")
                    //    + "#and SendDateTime <#" + DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")
                    //    + "# and Status = 0  and Emergency = 0 order by CInt([Position])").Tables[0];
                    DataTable dtprosample = bllsampleinfo.GetList(" SendDateTime >=#" + DateTime.Now.ToString("yyyy-MM-dd")
                        + "#and SendDateTime <#" + DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")
                        + "# and Status = 0  and Emergency = 0 order by SampleNo").Tables[0];
                    db = new DbHelperOleDb(1);
                    DbHelperOleDb.ExecuteSql(1,@"update tbSampleInfo set Emergency = 1 where Status = 0 and Emergency = 0 and SendDateTime >=#"
                                      + DateTime.Now.ToString("yyyy-MM-dd") + "#and SendDateTime <#"
                                      + DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + "#");
                    db = new DbHelperOleDb(0);
                    DataTable dtproinfo = bllproject.GetList("").Tables[0];
                    for (int i = 0; i < dtprosample.Rows.Count; i++)
                    {
                        //20180528 zlx add
                        //int count = new List<TestItem>((BindingList<TestItem>)this.dgvWorkListData.DataSource).FindAll(ty =>
                        //  ty.SamplePos == Convert.ToInt32(dtprosample.Rows[i]["Position"])).Count;
                        //Jun mod
                        int count = new List<TestItem>((BindingList<TestItem>)this.dgvWorkListData.DataSource).FindAll(ty =>
                      ty.SampleNo == dtprosample.Rows[i]["SampleNo"].ToString()).Count;
                        if (count != 0)
                            continue;
                        string[] projectName = dtprosample.Rows[i]["ProjectName"].ToString().TrimEnd().Split(' ');
                        for (int j = 0; j < projectName.Length; j++)
                        {
                            DataRow[] dr = dtproinfo.Select("ShortName ='" + projectName[j] + "'");
                            string projectProcedure = (string)dr[0]["ProjectProcedure"];
                            dtSampleInfo.Rows.Add(dtprosample.Rows[i]["SampleID"], dtprosample.Rows[i]["SampleType"],
                                dtprosample.Rows[i]["SampleNo"], dtprosample.Rows[i]["RepeatCount"], dtprosample.Rows[i]["Position"],
                                dtprosample.Rows[i]["SampleContainer"], 4, projectProcedure, projectName[j]);
                        }
                    }
                    #endregion
                    if (dtSampleInfo.Rows.Count == 0 || dtSampleInfo == null)
                    {
                        LoadingHelper.CloseForm();
                        addOrdinaryFlag = false;
                        frmAddSample.newSample = false;
                        Goon();
                        return;
                    }
                    #region 将dataTable转换为list<TestMethod>类型
                    Dictionary<string, Type> dEnum = new Dictionary<string, Type>();
                    dEnum.Add("StepName", typeof(ExperimentStep));
                    List<TestMethod> templisTestMethod = (List<TestMethod>)ConvertHelper.DataTableConvertToListGenuric<TestMethod>(MethodInit(dtSampleInfo), dEnum);
                    #endregion
                    //合并加液步骤，对进度进行赋值
                    List<TestSchedule> templisTestSchedule = MethodMergeSchedule(templisTestMethod);
                    //对进度列表进行排序，并按照所排的顺序对TestID按照顺序赋值
                    templisTestSchedule = orderModifyTestID(templisTestSchedule);
                    int tempSamNum = templisTestSchedule.FindAll(tx => tx.TestScheduleStep ==
                        TestSchedule.ExperimentScheduleStep.AddLiquidTube).Count;
                    int NoStartTestID = dgvWorkListData.Rows.Count + 1;
                    //被延后的反应管的所在位置
                    int OldAddSamPos = lisTestSchedule.Find(ty => ty.TestID == NoStartTestID - 1 && ((
                        ty.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddLiquidTube) ||
                        (ty.TestScheduleStep == TestSchedule.ExperimentScheduleStep.DoNotTakeCareThis))).AddSamplePos;
                    //追加样本testID赋值
                    for (int j = 0; j < templisTestSchedule.Count; j++)
                    {
                        TestSchedule TempTestSchedule = templisTestSchedule[j];
                        TempTestSchedule.TestID = TempTestSchedule.TestID + NoStartTestID - 1;
                        if (TempTestSchedule.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddLiquidTube)
                        {
                            //稀释实验赋值
                            if (int.Parse(TempTestSchedule.dilutionTimes) > 1)
                            {
                                string[] diupos = TempTestSchedule.dilutionPos.Split('-');
                                if (diupos.Length > 1)
                                {
                                    string newpos = "";
                                    for (int i = 0; i < diupos.Length; i++)
                                    {
                                        if (newpos == "")
                                        {
                                            newpos = "1";
                                        }
                                        else
                                        {
                                            newpos += "-" + (i + 1).ToString();
                                        }
                                    }
                                    TempTestSchedule.dilutionPos = newpos;
                                }
                                else
                                {
                                    TempTestSchedule.dilutionPos = "1";
                                }
                                string[] temppos = TempTestSchedule.dilutionPos.Split('-');
                                TempTestSchedule.getSamplePos = "R" + temppos[temppos.Length - 1];
                                OldAddSamPos = OldAddSamPos + 1;
                                if (OldAddSamPos > ReactTrayHoleNum)
                                {
                                    OldAddSamPos = 4;
                                }
                                TempTestSchedule.AddSamplePos = OldAddSamPos;
                            }
                            else
                            {
                                OldAddSamPos = OldAddSamPos + 1;
                                if (OldAddSamPos > ReactTrayHoleNum)
                                {
                                    OldAddSamPos = 4;
                                }
                                TempTestSchedule.AddSamplePos = OldAddSamPos;
                                TempTestSchedule.getSamplePos = "S" + TempTestSchedule.samplePos;
                            }
                        }
                    }
                    //追加样本之前还有未运行的样本
                    NoStartTestID = NoStartTestId;
                    //获取添加新的普通样本后的当前样本数量
                    SampleNumCurrent = SampleNumCurrent + tempSamNum;//2018-07-11 lyn mod
                    //将实验运行时的样本合并到已经生成进度的列表中
                    lisTestSchedule.AddRange(templisTestSchedule);
                    //按照testid进行排序
                    lisTestSchedule.Sort(new SortEmergency());
                    //进度计算
                    #region 控件清空还未开始运行的进度条控件
                    while (dgvWorkListData.Controls.Count > 2)
                    {
                        this.dgvWorkListData.Controls.Clear();//清除已有的控件
                    }
                    for (int i = BTestItem.Count - 1; i >= NoStartTestID - 1; i--)
                    {
                        BTestItem.Remove(BTestItem[i]);
                    }
                    //清空之前获取已经开始实验的进度条控件
                    for (int i = lisProBar.Count - 1; i >= NoStartTestID - 1; i--)
                    {
                        lisProBar.Remove(lisProBar[i]);
                    }
                    #endregion
                    //将新添加的急诊进度和未开始运行的样本进度附加到datagridview中
                    BindData(lisTestSchedule.FindAll(tx => tx.TestID >= NoStartTestID), NoStartTestID);
                    #region 获取已经开始实验的样本的空闲时间
                    //获取已经开始运行的样本的进度表
                    List<TestSchedule> RunSchedule = lisTestSchedule.FindAll(tx => tx.TestID < NoStartTestID);
                    List<string> runFreeTime = GetFreeTime(RunSchedule);
                    #endregion
                    //未运行的样本进度计算
                    List<TestSchedule> lisTestNoRun = ExperimentalScheduleAlgorithm(lisTestSchedule.FindAll(tx => tx.TestID >= NoStartTestID), runFreeTime);
                    //清空之前获取已经开始实验的进度条控件
                    lisTestSchedule = lisTestSchedule.FindAll(tx => tx.TestID < NoStartTestID);
                    lisTestSchedule.AddRange(lisTestNoRun);
                    lisTestSchedule.Sort(new SortRun());
                    #endregion
                }
            }
            else
            {
                LogFile.Instance.Write("开始进行时间计算 " + DateTime.Now.ToString("mm:ss:ms"));
                #region 获取需要实验的样本
                DbHelperOleDb db = new DbHelperOleDb(1);
                DbHelperOleDb.ExecuteSql(1,@"update tbSampleInfo set Emergency = 3 where Status = 0 and Emergency = 2 and SendDateTime >=#"
                                        + DateTime.Now.ToString("yyyy-MM-dd") + "#and SendDateTime <#"
                                        + DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + "#");
                //查询添加的急诊样本  //tbSampleInfo.Emergency DESC;             
                //DataTable dtprosample = bllsampleinfo.GetList(" SendDateTime >=#" + DateTime.Now.ToString("yyyy-MM-dd")
                //    + "#and SendDateTime <#" + DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")
                //    + "# and Status = 0 order by Emergency DESC,CInt([Position]) ASC").Tables[0];
                //Jun mod
                db = new DbHelperOleDb(1);
                DataTable dtprosample = bllsampleinfo.GetList(" SendDateTime >=#" + DateTime.Now.ToString("yyyy-MM-dd")
                    + "#and SendDateTime <#" + DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")
                    + "# and Status = 0 order by Emergency DESC,SampleNo ASC").Tables[0];
                db = new DbHelperOleDb(0);
                DataTable dtproinfo = bllproject.GetList("").Tables[0];
                for (int i = 0; i < dtprosample.Rows.Count; i++)
                {
                    string[] projectName = dtprosample.Rows[i]["ProjectName"].ToString().TrimEnd().Split(' ');
                    for (int j = 0; j < projectName.Length; j++)
                    {
                        DataRow[] dr = dtproinfo.Select("ShortName ='" + projectName[j] + "'");
                        string projectProcedure = (string)dr[0]["ProjectProcedure"];
                       string dilutimes = dtprosample.Rows[i]["RepeatCount"].ToString();
                        dtSampleInfo.Rows.Add(dtprosample.Rows[i]["SampleID"], dtprosample.Rows[i]["SampleType"],
                            dtprosample.Rows[i]["SampleNo"], dtprosample.Rows[i]["RepeatCount"], dtprosample.Rows[i]["Position"],
                            dtprosample.Rows[i]["SampleContainer"], dtprosample.Rows[i]["Emergency"],
                            projectProcedure, projectName[j]);
                    }
                }
                #endregion
                if (dtSampleInfo.Rows.Count == 0 || dtSampleInfo == null)
                {
                    while (LoadingHelper.loadingForm == null)
                    {
                        Thread.Sleep(30);
                    }
                    LoadingHelper.CloseForm();
                    return;
                }
                #region 将dataTable转换为list<TestMethod>类型
                Dictionary<string, Type> dEnum = new Dictionary<string, Type>();
                dEnum.Add("StepName", typeof(ExperimentStep));
                lisTestMethod = (List<TestMethod>)ConvertHelper.DataTableConvertToListGenuric<TestMethod>(MethodInit(dtSampleInfo), dEnum);
                #endregion
                //合并加液步骤，对进度进行赋值
                lisTestSchedule = MethodMergeSchedule(lisTestMethod);
                //对进度列表进行排序，并按照所排的顺序对TestID按照顺序赋值
                lisTestSchedule = orderModifyTestID(lisTestSchedule);
                //dgvWorkListData控件上绑定进度排好的数据
                cleanDgvShowData(1);
                BindData(lisTestSchedule, 1);
                //进度计算
                lisTestSchedule = ExperimentalScheduleAlgorithm(lisTestSchedule, new List<string>() { "0-1000000" });
                lisTestSchedule.Sort(new SortRun());
                LogFile.Instance.Write("结束进行时间计算 " + DateTime.Now.ToString("mm:ss:ms"));

            }
            #endregion
            if (EmergencyFlag || addOrdinaryFlag)
            {
                List<TestSchedule> tss = lisTestSchedule.FindAll(tx => tx.StartTime <= sumTime);
                _GaDoingOne = tss[tss.Count - 1];
                TestStep = GaNextOne();
            }
            frmAddSample.newSample = false;
            LoadingHelper.CloseForm();
            NetCom3.ComWait.Set();
            frmSampleLoad.CaculatingFlag = false;
            EmergencyFlag = false;
            addOrdinaryFlag = false;
            if (frmMain.pauseFlag == true)
            {
                GetNoStartList(); //lyq 20200824
            }

            StopWatchAddOrSubtractEmergency();//增加倒计时的值
        }
        #region 清除工作界面展示数据
        /// <summary>
        /// 清除工作界面展示数据，JUN add 2019-03-06
        /// </summary>
        /// <returns></returns>
        /// <summary>
        void cleanDgvShowData(int NoStartTestID)
        {
            while (dgvWorkListData.Controls.Count > 2)
            {
                this.dgvWorkListData.Controls.Clear();//清除已有的控件
            }
            for (int i = BTestItem.Count - 1; i >= NoStartTestID - 1; i--)
            {
                BTestItem.Remove(BTestItem[i]);
            }
            for (int i = lisProBar.Count - 1; i >= NoStartTestID - 1; i--)
            {
                lisProBar.Remove(lisProBar[i]);
            }
        }
        #endregion
        System.Timers.Timer stopTimer = new System.Timers.Timer();//倒计时timer
        int LastMaxTime = 0;//记录加急诊前的试验时间最大值
        double MaxTime = 0;//记录实验剩余时间
        int LastSumTime = 0;//记录上一次sunmtime的值
        const float PiusTimes = 1.1f;//增长的beilv
        void SetStopWatch()//实验倒计时,初始化并开始
        {
            MaxTime = lisTestSchedule.Select(it => it.EndTime).ToList<int>().Max();
            LastMaxTime = (int)MaxTime;
            MaxTime = MaxTime - sumTime + 180 * 2;
            MaxTime *= PiusTimes;
            LastSumTime = 0;
            TimeSpan span = new TimeSpan(0, 0, Convert.ToInt32(MaxTime));
            while (!this.IsHandleCreated)
            {
                Thread.Sleep(30);
            }
            LogFile.Instance.Write("**********时间更新1：" + DateTime.Now.ToString("HH-mm-ss") +
                " H :" + ((int)span.TotalHours).ToString("00") +
                "M :" + span.Minutes.ToString("00") + "**********");
            TimeLabel2.Invoke(new Action(() =>
            {
                TimeLabel2.Text = ((int)span.TotalHours).ToString("00");
                TimeLabel3.Text = span.Minutes.ToString("00");
                TimeLabel2.Visible = TimeLabel1.Visible = TimeLabel3.Visible = label2.Visible = label3.Visible = true;
            }));
            stopTimer.Elapsed -= StopWatchDecrease;
            stopTimer.Elapsed += StopWatchDecrease;
            stopTimer.Interval = 60000;
            stopTimer.Start();
        }

        void StopStopWatch()//结束并清理倒计时
        {
            stopTimer.Stop();
            LogFile.Instance.Write("**********时间更新4：" + DateTime.Now.ToString("HH-mm-ss") +
                " StopStopWatch");
            while (!TimeLabel2.IsHandleCreated)
            {
                Thread.Sleep(30);
            }
            LogFile.Instance.Write("**********时间更新5：" + DateTime.Now.ToString("HH-mm-ss"));
            lock (TimeLabel2)
            {
                TimeLabel2.Invoke(new Action(() =>
                {
                    TimeLabel2.Visible = TimeLabel1.Visible = TimeLabel3.Visible = label2.Visible = label3.Visible = false;
                    return;
                }));
            }
        }

        void StopWatchAddOrSubtractEmergency()//加急诊之后增加倒计时,或者减去试验后减少倒计时
        {
            int temp = lisTestSchedule.Select(it => it.EndTime).ToList<int>().Max();
            int margin = temp - LastMaxTime;
            LastMaxTime = temp;
            MaxTime = MaxTime + (margin * PiusTimes);
        }

        void StopWatchDecrease(object sender, ElapsedEventArgs e)//倒计时计时器每分钟引发的事件
        {
            LogFile.Instance.Write("**********时间更新2：" + DateTime.Now.ToString("HH-mm-ss") +
               " RunFlag :" + (int)RunFlagStart.IsRuning + " MaxTime :"+ MaxTime + "**********");
            if (RunFlag == (int)RunFlagStart.IsRuning)
            {
                if (sumTime == 0)
                {
                    MaxTime -= 60;
                }
                else
                {
                    if (_GaDoingOne == null)
                    {
                        MaxTime -= 60;
                    }
                    else
                    {
                        double temp = (sumTime - LastSumTime) * PiusTimes;
                        MaxTime -= temp;
                        LastSumTime = sumTime;
                    }
                }
                if (MaxTime <= 0)
                {
                    MaxTime = 0;
                }
                TimeSpan span = new TimeSpan(0, 0, Convert.ToInt32(MaxTime));
                while (!this.IsHandleCreated)
                {
                    Thread.Sleep(30);
                }
                LogFile.Instance.Write("**********时间更新3：" + DateTime.Now.ToString("HH-mm-ss") +
                " H :" + ((int)span.TotalHours).ToString("00") +
                "M :" + span.Minutes.ToString("00") + "**********");
                TimeLabel2.Invoke(new Action(() =>
                {
                    TimeLabel2.Text = ((int)span.TotalHours).ToString("00");
                    TimeLabel3.Text = span.Minutes.ToString("00");
                }));
            }
            else
            {
                StopStopWatch();
            }
        }

        /// <summary>
        /// 实验步骤初始化
        /// </summary>
        /// <param name="dtSampleInfo">读取的符合条件的样本信息，可以实验运行之前生成的样本信息也可以时实验运行中添加的急诊样本信息</param>
        /// <returns></returns>

        public DataTable MethodInit(DataTable dtSampleInfo)
        {
            #region 从数据库中读取样本信息，初始化实验步骤存储到DataTable

            //新建datatable保存实验的每个步骤
            DataTable dtStepInfo = new DataTable();
            dtStepInfo.Columns.Add("SampleID", typeof(int));
            dtStepInfo.Columns.Add("TestID", typeof(int));
            dtStepInfo.Columns.Add("StepID", typeof(int));
            dtStepInfo.Columns.Add("SampleNo", typeof(string));
            dtStepInfo.Columns.Add("SampleContainer", typeof(string));
            dtStepInfo.Columns.Add("SamplePos", typeof(int));
            dtStepInfo.Columns.Add("ItemName", typeof(string));
            dtStepInfo.Columns.Add("AddLiqud", typeof(string));
            dtStepInfo.Columns.Add("Emergency", typeof(int));
            dtStepInfo.Columns.Add("StepName", typeof(string));
            dtStepInfo.Columns.Add("ProMethod", typeof(int));
            dtStepInfo.Columns.Add("timeLength", typeof(int));
            //清洗盘状态表中新添加一个字段，存放稀释倍数。LYN add 20171114
            dtStepInfo.Columns.Add("dilutionTimes", typeof(string));

            int testID = 1;
            int stepID = 1;
            DbHelperOleDb db = new DbHelperOleDb(1);
            DataTable dtTestData = new BLL.tbAssayResult().GetList("").Tables[0];
            for (int i = 0; i < dtSampleInfo.Rows.Count; i++)
            {
                //string str = "select tbAssayResult.AssayResultID,tbAssayResult.SampleID,tbSampleInfo.SampleNo,"
                //   + "tbAssayResult.ItemName,tbAssayResult.TestDate,tbAssayResult.PMTCounter,"
                //   + "tbAssayResult.Concentration,tbAssayResult.Unit,tbAssayResult.Result,tbAssayResult.Range,tbSampleInfo.SampleType,tbAssayResult.Status,tbAssayResult.Batch from " //2018-08-17  zlx 添加tbAssayResult.Status
                //   + "tbAssayResult INNER JOIN tbSampleInfo on tbAssayResult.SampleID = tbSampleInfo.SampleID "
                //   + "where tbAssayResult.SampleID=" + SampleID + "";

                //DbHelperOleDb DB = new DbHelperOleDb(1);//2018-5-9 zlx add tbSampleInfo.SampleType
                //DataTable dtTestData = DbHelperOleDb.Query(@str).Tables[0];
                DataRow[] drTestData = null;
                string Emergency = dtSampleInfo.Rows[i]["Emergency"].ToString();
                if (Emergency != "4" || Emergency != "5")
                {
                    drTestData = dtTestData.Select("SampleID="
                          + dtSampleInfo.Rows[i]["SampleID"] + " AND ItemName='"
                          + dtSampleInfo.Rows[i]["ShortName"].ToString() + "'AND Status >= 0");
                    //dtTestData = new BLL.tbAssayResult().GetList("tbAssayResult.SampleID=" +
                    //    dtSampleInfo.Rows[i]["SampleID"] + " AND tbAssayResult.ItemName='" +
                    //    dtSampleInfo.Rows[i]["ShortName"].ToString() + "'AND Status >= 0").Tables[0];
                }
                if (drTestData != null && drTestData.Length > 0)
                {
                    if (drTestData.Length >= int.Parse(dtSampleInfo.Rows[i]["RepeatCount"].ToString()))
                        continue;
                    else
                        dtSampleInfo.Rows[i]["RepeatCount"] = int.Parse(dtSampleInfo.Rows[i]["RepeatCount"].
                            ToString()) - drTestData.Length;
                }
                string ProjectProcedure = dtSampleInfo.Rows[i]["ProjectProcedure"].ToString();
                string[] step = ProjectProcedure.Split(';');
                int tempProMethod = 1;
                //if (step.Length == 8 || step.Length == 9)
                //    tempProMethod = 1;
                //else
                //    tempProMethod = 2;
                for (int j = 0; j < int.Parse(dtSampleInfo.Rows[i]["RepeatCount"].ToString()); j++)
                {
                    #region 稀释步骤添加 LYN add 20171114
                    string name = dtSampleInfo.Rows[i]["ShortName"].ToString();
                    //从已有的样本信息表中获取当前样本的稀释信息
                    DataRow[] rows = dtSampleRunInfo.Select("ItemName='" + name + "' and SampleNo='"
                        + dtSampleInfo.Rows[i]["SampleNo"].ToString() + "'");
                    int DiuTimes = 1;
                    //string Emergency = dtSampleInfo.Rows[i]["Emergency"].ToString();
                    if (rows.Length == 0)
                    {
                        DiuTimes = 1;
                    }

                    else
                    {
                        if (rows[0]["SampleType"].ToString().Contains("标准品") || rows[0]["SampleType"].ToString().Contains("定标液") || rows[0]["SampleType"].ToString().Contains("校准品"))
                        {
                            Emergency = "5";
                        }
                        else if (rows[0]["SampleType"].ToString().Contains("质控品"))
                        {
                            Emergency = "4";
                        }
                        else if (rows[0]["SampleType"].ToString().Contains("交叉污染"))//lyq add 20190830
                        {
                            //Emergency = (int.Parse(dtSampleInfo.Rows[i]["RepeatCount"].ToString()) - j -1).ToString() ;   
                            //Emergency = (int.Parse(dtSampleInfo.Rows[i]["RepeatCount"].ToString()) - j - 1 - 1).ToString();
                            Emergency = (int.Parse(dtSampleInfo.Rows[i]["RepeatCount"].ToString()) - j - 6).ToString();
                        }
                        else
                        {
                            //获取急诊信息
                            Emergency = rows[0]["Emergency"].ToString() == "是" ?
                                ((frmWorkList.RunFlag == (int)RunFlagStart.IsRuning) ? "2" : "3") :
                                (frmWorkList.RunFlag == (int)RunFlagStart.IsRuning ? "0" : "1");
                        }
                        //2019-02-28 zlx mod
                        if ((rows.Length > 0 && !(rows[0]["SampleType"].ToString().Contains("标准品")) || rows[0]["SampleType"].ToString().Contains("校准品") ||
                            rows[0]["SampleType"].ToString().Contains("质控品") ) )
                        {
                            //获取稀释倍数
                            DiuTimes = int.Parse(rows[0]["DilutionTimes"].ToString());
                        }
                    }
                    //加液量
                    string[] Addliqud = step[0].Split('-');
                    if (DiuTimes > 1)//稀释倍数大于1
                    {
                        //2018-06-05 zlx mod
                        //dtStepInfo.Rows.Add(dtSampleInfo.Rows[i]["SampleID"], testID, stepID, dtSampleInfo.Rows[i]["SampleNo"], dtSampleInfo.Rows[i]["SampleContainer"], dtSampleInfo.Rows[i]["tbSampleInfo.Position"],
                        //        dtSampleInfo.Rows[i]["ShortName"], Addliqud[1], Emergency, ExperimentStep.Dilution, 1, dilutionTime, DiuTimes);
                        dtStepInfo.Rows.Add(dtSampleInfo.Rows[i]["SampleID"], testID, stepID,
                            dtSampleInfo.Rows[i]["SampleNo"], dtSampleInfo.Rows[i]["SampleContainer"],
                            dtSampleInfo.Rows[i]["Position"],
                                dtSampleInfo.Rows[i]["ShortName"], Addliqud[1], Emergency,
                                ExperimentStep.Dilution, 1, dilutionTime, DiuTimes);
                        stepID++;
                    }
                    #endregion
                    for (int k = 0; k < step.Length; k++)
                    {
                        string[] AddLiqud = step[k].Split('-');
                        if (step[k].Contains("S"))
                        {
                            dtStepInfo.Rows.Add(dtSampleInfo.Rows[i]["SampleID"], testID, stepID,
                                dtSampleInfo.Rows[i]["SampleNo"], dtSampleInfo.Rows[i]["SampleContainer"],
                                dtSampleInfo.Rows[i]["Position"], dtSampleInfo.Rows[i]["ShortName"], AddLiqud[1],
                                Emergency, ExperimentStep.AddSample, 1, SampleTime, DiuTimes);
                            stepID++;
                        }
                        else if (step[k].Contains("R1"))
                        {//加试剂1步骤
                            if (k < 3)
                            {
                                tempProMethod = 1;
                            }
                            else
                            {
                                tempProMethod = 2;
                            }
                            dtStepInfo.Rows.Add(dtSampleInfo.Rows[i]["SampleID"], testID, stepID, dtSampleInfo.Rows[i]["SampleNo"], dtSampleInfo.Rows[i]["SampleContainer"], dtSampleInfo.Rows[i]["Position"],
                                   dtSampleInfo.Rows[i]["ShortName"], AddLiqud[1], Emergency, ExperimentStep.AddRegent1, tempProMethod, RegentTime, 1);
                            stepID++;
                        }
                        else if (step[k].Contains("R2"))
                        {//加试剂2步骤
                            if (k < 3)
                            {
                                tempProMethod = 1;
                            }
                            else
                            {
                                tempProMethod = 2;
                            }
                            dtStepInfo.Rows.Add(dtSampleInfo.Rows[i]["SampleID"], testID, stepID, dtSampleInfo.Rows[i]["SampleNo"], dtSampleInfo.Rows[i]["SampleContainer"], dtSampleInfo.Rows[i]["Position"],
                                dtSampleInfo.Rows[i]["ShortName"], AddLiqud[1], Emergency, ExperimentStep.AddRegent2, tempProMethod, RegentTime, 1);
                            stepID++;
                        }
                        else if (step[k].Contains("R3"))
                        {//加试剂3步骤
                            if (k < 3)
                            {
                                tempProMethod = 1;
                            }
                            else
                            {
                                tempProMethod = 2;
                            }
                            dtStepInfo.Rows.Add(dtSampleInfo.Rows[i]["SampleID"], testID, stepID, dtSampleInfo.Rows[i]["SampleNo"], dtSampleInfo.Rows[i]["SampleContainer"], dtSampleInfo.Rows[i]["Position"],
                                dtSampleInfo.Rows[i]["ShortName"], AddLiqud[1], Emergency, ExperimentStep.AddRegent3, tempProMethod, RegentTime, 1);
                            stepID++;
                        }
                        else if (step[k].Contains("H"))
                        {//孵育

                            string[] incubationTime = step[k].Split('-');
                            dtStepInfo.Rows.Add(dtSampleInfo.Rows[i]["SampleID"], testID, stepID, dtSampleInfo.Rows[i]["SampleNo"], dtSampleInfo.Rows[i]["SampleContainer"], dtSampleInfo.Rows[i]["Position"],
                                dtSampleInfo.Rows[i]["ShortName"], 0, Emergency, ExperimentStep.Incubation, 0, int.Parse(incubationTime[1]) * 60, 1);
                            stepID++;

                        }
                        else if (step[k].Contains("B"))
                        {//加磁珠
                            dtStepInfo.Rows.Add(dtSampleInfo.Rows[i]["SampleID"], testID, stepID, dtSampleInfo.Rows[i]["SampleNo"], dtSampleInfo.Rows[i]["SampleContainer"], dtSampleInfo.Rows[i]["Position"],
                               dtSampleInfo.Rows[i]["ShortName"], AddLiqud[1], Emergency, ExperimentStep.AddBeads, 0, beadTime, 1);
                            stepID++;
                        }
                        else if (step[k].Contains("W"))
                        {//清洗
                            dtStepInfo.Rows.Add(dtSampleInfo.Rows[i]["SampleID"], testID, stepID, dtSampleInfo.Rows[i]["SampleNo"], dtSampleInfo.Rows[i]["SampleContainer"], dtSampleInfo.Rows[i]["Position"],
                              dtSampleInfo.Rows[i]["ShortName"], AddLiqud[1], Emergency, ExperimentStep.Clean, 0, washTime, 1);
                            stepID++;
                        }
                        else if (step[k].Contains("T"))
                        {//加底物
                            dtStepInfo.Rows.Add(dtSampleInfo.Rows[i]["SampleID"], testID, stepID, dtSampleInfo.Rows[i]["SampleNo"], dtSampleInfo.Rows[i]["SampleContainer"], dtSampleInfo.Rows[i]["Position"],
                               dtSampleInfo.Rows[i]["ShortName"], AddLiqud[1], Emergency, ExperimentStep.AddSubstrate, 0, substrateTime, 1);
                            stepID++;
                        }
                        else if (step[k].Contains("D"))
                        {//读数
                            dtStepInfo.Rows.Add(dtSampleInfo.Rows[i]["SampleID"], testID, stepID, dtSampleInfo.Rows[i]["SampleNo"], dtSampleInfo.Rows[i]["SampleContainer"], dtSampleInfo.Rows[i]["Position"],
                              dtSampleInfo.Rows[i]["ShortName"], 0, Emergency, ExperimentStep.Read, 0, readTime, 1);
                            stepID++;
                        }
                    }
                    stepID = 1;
                    testID++;
                }
            }
            #endregion
            return dtStepInfo;
        }

        /// <summary>
        /// 加液步骤按顺序进行合并加液的步骤，对进度进行赋值
        /// </summary>
        /// <param name="lisTM">各个样本的步骤方法</param>
        /// <returns></returns>
        public List<TestSchedule> MethodMergeSchedule(List<TestMethod> lisTM)
        {
            List<TestSchedule> TEMPlisTestSchedule = new List<TestSchedule>();
            int temp = 0;
            if (lisTM.Count == 0 || lisTM == null)
            {
                return TEMPlisTestSchedule;
            }
            DbHelperOleDb db = new DbHelperOleDb(0);
            DataTable dtProject = DbHelperOleDb.Query(0,@"select * from tbProject").Tables[0];
            for (int j = 1; j <= lisTM[lisTM.Count - 1].TestID; j++)
            {
                int testTimeLength = 0;
                //一个实验中该步骤在第几个运行
                int stepTime = 1;
                //获取相同项目和相同样本步骤数量
                int numItem = lisTM.Count(ty => ty.TestID == j);
                #region 加液步骤合并
                //判断合并的步骤已有几个
                int flagstep = 0;
                //获取当前试剂的索引
                int tempRegent = 0;
                for (int i = temp; i < temp + numItem; i++)
                {
                    switch (lisTM[i].StepName)
                    {
                        #region 稀释步骤合并到加液步骤中。LYN add 20171114
                        case ExperimentStep.Dilution:
                            //20180601 zlx add
                            testschedule.singleStep += "D-";
                            #region 新修改的稀释过程
                            DataRow[] drProject = dtProject.Select("ShortName = '" + lisTM[i].ItemName + "'");
                            object DiluteCount = drProject[0]["DiluteCount"];
                            int ExtraDiluteCount = int.Parse(lisTM[i].dilutionTimes) / int.Parse(DiluteCount.ToString());
                            int diucount = 0;
                            if (ExtraDiluteCount > 1)
                            {
                                string diuInfo = DiuInfo.GetDiuInfo(ExtraDiluteCount);
                                foreach (string diu in diuInfo.Split(';'))
                                {
                                    if (diu != "")
                                    {
                                        DiuTubeNum = DiuTubeNum + 1;
                                        testTimeLength += dilutionTime;
                                        if (testschedule.dilutionPos == null)
                                        {
                                            ReactStartPos = ReactStartPos + 1 > ReactTrayHoleNum ? (ReactStartPos + 1) % ReactTrayHoleNum : ReactStartPos + 1;
                                            testschedule.dilutionPos = (ReactStartPos).ToString();
                                        }
                                        else
                                        {
                                            ReactStartPos = ReactStartPos + 1 > ReactTrayHoleNum ? (ReactStartPos + 1) % ReactTrayHoleNum : ReactStartPos + 1;
                                            testschedule.dilutionPos += "-" + ((ReactStartPos).ToString());
                                        }
                                    }
                                    diucount = diucount + 1;
                                }
                            }
                            if (int.Parse(DiluteCount.ToString()) > 1)
                            {
                                object DiluteName = drProject[0]["DiluteName"];
                                foreach (string diu in DiluteName.ToString().Split(';'))
                                {
                                    if (diu != "")
                                    {
                                        DiuTubeNum = DiuTubeNum + 1;
                                        //testTimeLength += dilutionTime;
                                        if (testschedule.dilutionPos == null)
                                        {
                                            ReactStartPos = ReactStartPos + 1 > ReactTrayHoleNum ? (ReactStartPos + 1) % ReactTrayHoleNum : ReactStartPos + 1;
                                            testschedule.dilutionPos = (ReactStartPos).ToString();
                                        }
                                        else
                                        {
                                            ReactStartPos = ReactStartPos + 1 > ReactTrayHoleNum ? (ReactStartPos + 1) % ReactTrayHoleNum : ReactStartPos + 1;
                                            testschedule.dilutionPos += "-" + ((ReactStartPos).ToString());
                                        }
                                    }
                                    diucount = diucount + 1;
                                }
                            }
                            testschedule.dilutionTimes = lisTM[i].dilutionTimes;
                            testschedule.getSamplePos = "R" + (ReactStartPos).ToString();
                            #endregion
                            testTimeLength = DiuInfo.GetDiuTime(diucount);
                            //testTimeLength = int.Parse(OperateIniFile.ReadInIPara("Time", "dilutionTime" + diucount + ""));
                            testschedule.AddLiqud += lisTM[i].AddLiqud + "-";
                            ReactStartPos = ReactStartPos + 1 > ReactTrayHoleNum ? (ReactStartPos + 1) % ReactTrayHoleNum : ReactStartPos + 1;
                            testschedule.AddSamplePos = ReactStartPos;
                            break;

                        #endregion
                        case ExperimentStep.AddSample:
                            testTimeLength += SampleTime;
                            flagstep = ++flagstep;
                            testschedule.singleStep += "S";
                            testschedule.AddLiqud += lisTM[i].AddLiqud;
                            break;
                        case ExperimentStep.AddRegent1:

                            if (lisTM[i].ProMethod == 1)
                            {
                                testschedule.singleStep += "-R1";
                                testTimeLength += RegentTime;
                                testschedule.AddLiqud += "-" + lisTM[i].AddLiqud;
                                tempRegent = i;
                            }
                            break;
                        case ExperimentStep.AddRegent2:

                            if (lisTM[i].ProMethod == 1)
                            {
                                testschedule.singleStep += "-R2";
                                testTimeLength += RegentTime;
                                testschedule.AddLiqud += "-" + lisTM[i].AddLiqud;
                                tempRegent = i;
                            }
                            break;
                        case ExperimentStep.AddRegent3:

                            if (lisTM[i].ProMethod == 1)
                            {
                                testschedule.singleStep += "-R3";
                                testTimeLength += RegentTime;
                                testschedule.AddLiqud += "-" + lisTM[i].AddLiqud;
                                tempRegent = i;
                            }
                            break;
                    }
                    testschedule.SampleNo = lisTM[i].SampleNo;
                    testschedule.SampleID = lisTM[i].SampleID;
                    testschedule.ItemName = lisTM[i].ItemName;
                    testschedule.TestID = lisTM[i].TestID;
                    testschedule.samplePos = lisTM[i].SamplePos;
                    testschedule.SampleContainer = lisTM[i].SampleContainer;
                    testschedule.TimePro = ItemProportion(lisTM[i].ItemName, lisTM[i].SamplePos, lisTM);
                    testschedule.ProMethod = lisTM[i].ProMethod;
                    testschedule.StartTime = 0;
                    testschedule.TimeLengh = testTimeLength;
                    testschedule.EndTime = testschedule.StartTime + testschedule.TimeLengh;
                    testschedule.TestScheduleStep = TestSchedule.ExperimentScheduleStep.AddLiquidTube;
                    testschedule.Emergency = lisTM[i].Emergency;
                }
                //如果未稀释，如下赋值 LYN add 20171114
                if (testschedule.dilutionPos == null)
                {
                    ReactStartPos = ReactStartPos + 1 > ReactTrayHoleNum ? (ReactStartPos + 1) % ReactTrayHoleNum : ReactStartPos + 1;
                    testschedule.AddSamplePos = ReactStartPos;
                    testschedule.dilutionTimes = "1";
                    testschedule.getSamplePos = "S" + testschedule.samplePos.ToString();
                }
                testschedule.stepNum = stepTime++;
                TEMPlisTestSchedule.Add(testschedule);
                testschedule = new TestSchedule();
                #endregion

                #region 余下步骤依次赋值
                for (int i = temp; i < temp + numItem; i++)
                {
                    switch (lisTM[i].StepName)
                    {
                        case ExperimentStep.AddBeads:
                            testschedule.SampleNo = lisTM[i].SampleNo;
                            testschedule.SampleID = lisTM[i].SampleID;
                            testschedule.ItemName = lisTM[i].ItemName;
                            testschedule.TestID = lisTM[i].TestID;
                            testschedule.samplePos = lisTM[i].SamplePos;
                            testschedule.SampleContainer = lisTM[i].SampleContainer;
                            testschedule.TimePro = ItemProportion(lisTM[i].ItemName, lisTM[i].SamplePos, lisTM);
                            testschedule.StartTime = 0;
                            testschedule.TimeLengh = beadTime;
                            testschedule.stepNum = 3;
                            testschedule.stepNum = stepTime++;
                            testschedule.AddLiqud = lisTM[i].AddLiqud;
                            testschedule.EndTime = testschedule.StartTime + testschedule.TimeLengh;
                            testschedule.TestScheduleStep = TestSchedule.ExperimentScheduleStep.AddBeads;
                            testschedule.ProMethod = lisTM[i].ProMethod;
                            testschedule.Emergency = lisTM[i].Emergency;
                            //稀释相关字段赋值。 LYN add 20171114
                            testschedule.dilutionPos = "";
                            testschedule.AddSamplePos = 0;
                            testschedule.dilutionTimes = "1";
                            testschedule.getSamplePos = "";
                            TEMPlisTestSchedule.Add(testschedule);
                            testschedule = new TestSchedule();
                            break;
                        case ExperimentStep.Incubation:
                            testschedule.SampleNo = lisTM[i].SampleNo;
                            testschedule.SampleID = lisTM[i].SampleID;
                            testschedule.ItemName = lisTM[i].ItemName;
                            testschedule.TestID = lisTM[i].TestID;
                            testschedule.samplePos = lisTM[i].SamplePos;
                            testschedule.SampleContainer = lisTM[i].SampleContainer;
                            testschedule.TimePro = ItemProportion(lisTM[i].ItemName, lisTM[i].SamplePos, lisTM);
                            testschedule.StartTime = 0;
                            testschedule.stepNum = stepTime++;
                            testschedule.TimeLengh = lisTM[i].timeLength;
                            testschedule.AddLiqud = lisTM[i].AddLiqud;
                            testschedule.EndTime = testschedule.StartTime + testschedule.TimeLengh;
                            testschedule.TestScheduleStep = TestSchedule.ExperimentScheduleStep.Incubation;
                            testschedule.Emergency = lisTM[i].Emergency;
                            testschedule.ProMethod = lisTM[i].ProMethod;
                            //稀释相关字段赋值。 LYN add 20171114
                            testschedule.dilutionPos = "";
                            testschedule.AddSamplePos = 0;
                            testschedule.dilutionTimes = "1";
                            testschedule.getSamplePos = "";
                            TEMPlisTestSchedule.Add(testschedule);
                            testschedule = new TestSchedule();
                            break;
                        case ExperimentStep.AddRegent1:
                            if (lisTM[i].ProMethod == 2)
                            {
                                testschedule.SampleNo = lisTM[i].SampleNo;
                                testschedule.SampleID = lisTM[i].SampleID;
                                testschedule.ItemName = lisTM[i].ItemName;
                                testschedule.TestID = lisTM[i].TestID;
                                testschedule.samplePos = lisTM[i].SamplePos;
                                testschedule.SampleContainer = lisTM[i].SampleContainer;
                                testschedule.TimePro = ItemProportion(lisTM[i].ItemName, lisTM[i].SamplePos, lisTM);
                                testschedule.AddLiqud = lisTM[i].AddLiqud;
                                testschedule.StartTime = 0;
                                testschedule.TimeLengh = RegentTime;
                                testschedule.stepNum = stepTime++;
                                testschedule.EndTime = testschedule.StartTime + testschedule.TimeLengh;
                                testschedule.TestScheduleStep = TestSchedule.ExperimentScheduleStep.AddSingleR;
                                testschedule.Emergency = lisTM[i].Emergency;
                                testschedule.ProMethod = lisTM[i].ProMethod;
                                //稀释相关字段赋值。 LYN add 20171114
                                testschedule.dilutionPos = "";
                                testschedule.AddSamplePos = 0;
                                testschedule.dilutionTimes = "1";
                                testschedule.getSamplePos = "";
                                testschedule.singleStep = "R1";
                                TEMPlisTestSchedule.Add(testschedule);
                                testschedule = new TestSchedule();

                            }
                            break;
                        case ExperimentStep.AddRegent2:
                            if (lisTM[i].ProMethod == 2)
                            {
                                testschedule.SampleNo = lisTM[i].SampleNo;
                                testschedule.SampleID = lisTM[i].SampleID;
                                testschedule.ItemName = lisTM[i].ItemName;
                                testschedule.TestID = lisTM[i].TestID;
                                testschedule.samplePos = lisTM[i].SamplePos;
                                testschedule.SampleContainer = lisTM[i].SampleContainer;
                                testschedule.TimePro = ItemProportion(lisTM[i].ItemName, lisTM[i].SamplePos, lisTM);
                                testschedule.AddLiqud = lisTM[i].AddLiqud;
                                testschedule.StartTime = 0;
                                testschedule.TimeLengh = RegentTime;
                                testschedule.stepNum = stepTime++;
                                testschedule.EndTime = testschedule.StartTime + testschedule.TimeLengh;
                                testschedule.TestScheduleStep = TestSchedule.ExperimentScheduleStep.AddSingleR;
                                testschedule.Emergency = lisTM[i].Emergency;
                                testschedule.ProMethod = lisTM[i].ProMethod;
                                //稀释相关字段赋值。 LYN add 20171114
                                testschedule.dilutionPos = "";
                                testschedule.AddSamplePos = 0;
                                testschedule.dilutionTimes = "1";
                                testschedule.getSamplePos = "";
                                testschedule.singleStep = "R2";
                                TEMPlisTestSchedule.Add(testschedule);
                                testschedule = new TestSchedule();

                            }
                            break;
                        case ExperimentStep.AddRegent3:
                            if (lisTM[i].ProMethod == 2)
                            {
                                testschedule.SampleNo = lisTM[i].SampleNo;
                                testschedule.SampleID = lisTM[i].SampleID;
                                testschedule.ItemName = lisTM[i].ItemName;
                                testschedule.TestID = lisTM[i].TestID;
                                testschedule.samplePos = lisTM[i].SamplePos;
                                testschedule.SampleContainer = lisTM[i].SampleContainer;
                                testschedule.TimePro = ItemProportion(lisTM[i].ItemName, lisTM[i].SamplePos, lisTM);
                                testschedule.AddLiqud = lisTM[i].AddLiqud;
                                testschedule.StartTime = 0;
                                testschedule.TimeLengh = RegentTime;
                                testschedule.stepNum = stepTime++;
                                testschedule.EndTime = testschedule.StartTime + testschedule.TimeLengh;
                                testschedule.TestScheduleStep = TestSchedule.ExperimentScheduleStep.AddSingleR;
                                testschedule.Emergency = lisTM[i].Emergency;
                                testschedule.ProMethod = lisTM[i].ProMethod;
                                //稀释相关字段赋值。 LYN add 20171114
                                testschedule.dilutionPos = "";
                                testschedule.AddSamplePos = 0;
                                testschedule.dilutionTimes = "1";
                                testschedule.getSamplePos = "";
                                testschedule.singleStep = "R3";
                                TEMPlisTestSchedule.Add(testschedule);
                                testschedule = new TestSchedule();

                            }
                            break;
                        case ExperimentStep.Clean:
                            if (i < temp + numItem - 3)
                            {
                                testschedule.SampleNo = lisTM[i].SampleNo;
                                testschedule.SampleID = lisTM[i].SampleID;
                                testschedule.ItemName = lisTM[i].ItemName;
                                testschedule.TestID = lisTM[i].TestID;
                                testschedule.samplePos = lisTM[i].SamplePos;
                                testschedule.SampleContainer = lisTM[i].SampleContainer;
                                testschedule.AddLiqud = lisTM[i].AddLiqud;
                                testschedule.TimePro = ItemProportion(lisTM[i].ItemName, lisTM[i].SamplePos, lisTM);
                                testschedule.StartTime = 0;
                                testschedule.stepNum = stepTime++;
                                testschedule.TimeLengh = lisTM[i].timeLength + 100;
                                testschedule.EndTime = testschedule.StartTime + testschedule.TimeLengh;
                                testschedule.TestScheduleStep = TestSchedule.ExperimentScheduleStep.Wash1;
                                testschedule.ProMethod = lisTM[i].ProMethod;
                                testschedule.Emergency = lisTM[i].Emergency;
                                //稀释相关字段赋值。 LYN add 20171114
                                testschedule.dilutionPos = "";
                                testschedule.AddSamplePos = 0;
                                testschedule.dilutionTimes = "1";
                                testschedule.getSamplePos = "";
                                TEMPlisTestSchedule.Add(testschedule);
                                testschedule = new TestSchedule();
                            }
                            break;
                    }

                }
                #region 清洗步骤合并
                for (int i = temp + numItem - 3; i < temp + numItem; i++)
                {

                    switch (lisTM[i].StepName)
                    {
                        case ExperimentStep.Clean:
                            testTimeLength += washTime;
                            testschedule.singleStep += "W2";
                            testschedule.AddLiqud += lisTM[i].AddLiqud;
                            break;
                        case ExperimentStep.AddSubstrate:
                            testTimeLength += substrateTime;
                            testschedule.singleStep += "-Su";
                            testschedule.AddLiqud += "-" + lisTM[i].AddLiqud;

                            break;
                        case ExperimentStep.Read:
                            testTimeLength += readTime;
                            testschedule.singleStep += "-R";
                            testschedule.AddLiqud += "-" + lisTM[i].AddLiqud;
                            break;

                    }
                    testschedule.SampleNo = lisTM[i].SampleNo;
                    testschedule.SampleID = lisTM[i].SampleID;
                    testschedule.ItemName = lisTM[i].ItemName;
                    testschedule.TestID = lisTM[i].TestID;
                    testschedule.samplePos = lisTM[i].SamplePos;
                    testschedule.SampleContainer = lisTM[i].SampleContainer;
                    testschedule.TimePro = ItemProportion(lisTM[i].ItemName, lisTM[i].SamplePos, lisTM);
                    testschedule.ProMethod = lisTM[i].ProMethod;
                    testschedule.StartTime = 0;

                    testschedule.TimeLengh = testTimeLength;
                    testschedule.EndTime = testschedule.StartTime + testschedule.TimeLengh;
                    testschedule.TestScheduleStep = TestSchedule.ExperimentScheduleStep.WashTray;
                    testschedule.Emergency = lisTM[i].Emergency;
                }
                //稀释相关字段赋值。 LYN add 20171114
                testschedule.dilutionPos = "";
                testschedule.AddSamplePos = 0;
                testschedule.dilutionTimes = "1";
                testschedule.getSamplePos = "";
                testschedule.stepNum = stepTime++;
                TEMPlisTestSchedule.Add(testschedule);
                testschedule = new TestSchedule();
                #endregion
                temp += numItem;
                #endregion
            }
            return TEMPlisTestSchedule;
        }

        /// <summary>
        /// 计算相同项目所占比重
        /// </summary>
        /// <param name="ItemName">项目名称</param>
        /// <param name="sampos">样本位号</param>
        /// <param name="lisTM">各个样本的步骤方法</param>
        /// <returns></returns>
        double ItemProportion(string ItemName, int sampos, List<TestMethod> lisTM)
        {
            double sumTime = 0;
            //取出相同样本号的项目
            List<TestMethod> tempSameTestMethod = lisTM.FindAll(ty => ty.SamplePos == sampos);
            List<ItemNameTime> lisItemNameTime = new List<ItemNameTime>();
            ItemNameTime itemNameTime = new ItemNameTime();
            for (int j = tempSameTestMethod[0].TestID; j <= tempSameTestMethod[tempSameTestMethod.Count - 1].TestID; j++)
            {
                //是否循环到第一次温育
                bool loopFlag = false;
                //取出相同样本号的步骤
                List<TestMethod> tempIDTestMethod = tempSameTestMethod.FindAll(ty => ty.TestID == j);
                double testTime = 0;
                for (int k = 0; k < tempIDTestMethod.Count; k++)
                {
                    switch (tempIDTestMethod[k].StepName)
                    {
                        //稀释时间添加. LYN add 20171114
                        case ExperimentStep.Dilution:
                        case ExperimentStep.AddSample:
                        case ExperimentStep.AddRegent1:
                        case ExperimentStep.AddRegent2:
                        case ExperimentStep.AddRegent3:
                            testTime += tempIDTestMethod[k].timeLength;
                            break;
                        case ExperimentStep.Incubation:
                            loopFlag = true;
                            break;
                    }
                    if (loopFlag)
                    {
                        break;
                    }
                }
                itemNameTime.ItemName = tempIDTestMethod[0].ItemName;
                itemNameTime.TestTime = testTime;
                lisItemNameTime.Add(itemNameTime);
                itemNameTime = new ItemNameTime();
            }
            for (int i = 0; i < tempSameTestMethod.Count; i++)
            {
                switch (tempSameTestMethod[i].StepName)
                {
                    //稀释时间添加. LYN add 20171114
                    case ExperimentStep.Dilution:
                    case ExperimentStep.AddSample:
                    case ExperimentStep.AddRegent1:
                    case ExperimentStep.AddRegent2:
                    case ExperimentStep.AddRegent3:
                    case ExperimentStep.Incubation:
                    case ExperimentStep.AddBeads:
                        sumTime += tempSameTestMethod[i].timeLength;
                        break;
                }
            }
            foreach (ItemNameTime inTime in lisItemNameTime)
            {
                if (inTime.ItemName == ItemName)
                {
                    return inTime.TestTime / sumTime;
                }
            }//求出每个项目的实验时间占所有项目实验时间的比重
            return 0;
        }

        /// <summary>
        /// 对进度列表进行排序，并按照所排的顺序对TestID按照顺序赋值
        /// </summary>
        /// <param name="lisTS">合并步骤后的实验进度</param>
        /// <returns></returns>
        public List<TestSchedule> orderModifyTestID(List<TestSchedule> lisTS)
        {
            lisTS.Sort(new SortSchedule());

            #region 反应盘开始放管位置赋值
            DataTable dtReactTrayInfo = OperateIniFile.ReadConfig(iniPathReactTrayInfo);
            //第一个有管的位置
            string FirstPos = "";
            DataRow[] dr = dtReactTrayInfo.Select("Pos='no" + ReactTrayNum.ToString() + "'");
            if (RunFlag == (int)RunFlagStart.IsRuning && NoStartTestId > 1) //lyq 20200328
            {
                FirstPos = GetRuningFirstPos(dtReactTrayInfo);
            }
            else
            {
                _GaDoingOne = null; //lyq 20200328
                FirstPos = GetFisrtPos(dtReactTrayInfo);
            }
            if (FirstPos == "")
                ReactStartPos = 3;
            else
                ReactStartPos = int.Parse(FirstPos.Substring(2)) - 1;
            LogFile.Instance.Write(ReactStartPos + "      ==========       ");
            #endregion

            #region 对稀释位置、加样位置、取样位置进行赋值. LYN add 20171114
            List<TestSchedule> tempTestSchedule = new List<TestSchedule>();
            //获取加液步骤的列表
            tempTestSchedule = lisTS.FindAll(tx => tx.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddLiquidTube);
            foreach (TestSchedule Ts in tempTestSchedule)
            {
                //稀释倍数大于1
                if (int.Parse(Ts.dilutionTimes.Trim()) > 1)
                {
                    //20180601 zlx add
                    string[] AddLiqud = Ts.AddLiqud.Split('-');
                    //稀释使用多个管
                    string[] spos = Ts.dilutionPos.Split('-');
                    Ts.dilutionPos = "";
                    for (int i = 0; i < spos.Length; i++)
                    {
                        if (Ts.dilutionPos == "")
                        {
                            Ts.dilutionPos = "1";
                        }
                        else
                        {
                            Ts.dilutionPos += "-" + ((i + 1) % 4 != 0 ? (i + 1) % 4 : 1).ToString();
                        }
                    }
                    #region
                    /*
                    if (int.Parse(Ts.dilutionTimes.Trim()) > DiuMaxTimes)
                    {
                        string[] spos = Ts.dilutionPos.Split('-');
                        Ts.dilutionPos = "";
                        for (int i = 0; i < spos.Length; i++)
                        {

                            if (Ts.dilutionPos == "")
                            {
                                Ts.dilutionPos = "1";
                            }
                            else
                            {
                                Ts.dilutionPos += "-" + (i + 1).ToString();
                            }


                        }

                    }
                    else
                    {
                        Ts.dilutionPos = "1";
                    }
                    */
                    #endregion
                    string[] diupos = Ts.dilutionPos.Split('-');
                    Ts.getSamplePos = "R" + diupos[diupos.Length - 1];
                    ReactStartPos = ReactStartPos + 1;
                    if (ReactStartPos > ReactTrayHoleNum)
                    {
                        ReactStartPos = 4;
                    }
                    Ts.AddSamplePos = ReactStartPos;
                }
                else
                {
                    ReactStartPos = ReactStartPos + 1;
                    if (ReactStartPos > ReactTrayHoleNum)
                    {
                        ReactStartPos = 4;
                    }
                    Ts.AddSamplePos = ReactStartPos;
                    Ts.getSamplePos = "S" + Ts.samplePos.ToString();
                }
            }
            #endregion
            //处理过的列表数量
            int tempScheduleNum = 0;
            int testIDNum = 1;
            while (tempScheduleNum < lisTS.Count)
            {
                List<TestSchedule> tempLis = new List<TestSchedule>();
                for (int i = tempScheduleNum; i < lisTS.Count; i++)
                {
                    tempLis.Add(lisTS[i]);
                }
                int SameTestNum = tempLis.Count(ty => ty.samplePos == lisTS[tempScheduleNum].samplePos
                    && ty.ItemName == lisTS[tempScheduleNum].ItemName && ty.TestID == lisTS[tempScheduleNum].TestID);
                for (int i = tempScheduleNum; i < tempScheduleNum + SameTestNum; i++)
                {
                    TestSchedule testCurrentSchedule = lisTS[i];
                    testCurrentSchedule.TestID = testIDNum;
                }
                testIDNum++;
                tempScheduleNum = tempScheduleNum + SameTestNum;
            }
            return lisTS;
        }

        /// <summary>
        /// 运行中温育第一个加管位置
        /// </summary>
        /// <param name="dtReactTrayInfo"></param>
        /// <returns></returns>
        string GetRuningFirstPos(DataTable dtReactTrayInfo)
        {
            string FirstPos = "";
            int OldAddSamPos =
                lisTestSchedule.Find(ty => ty.TestID == (NoStartTestId - 1) && ((
                ty.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddLiquidTube) ||
                (ty.TestScheduleStep == TestSchedule.ExperimentScheduleStep.DoNotTakeCareThis))).AddSamplePos;
            if (OldAddSamPos == dtReactTrayInfo.Rows.Count)
            {
                FirstPos = "";
            }
            else
            {
                FirstPos = "no" + (OldAddSamPos + 1);
            }
            return FirstPos;
        }

        /// <summary>
        /// 温育第一个加管位置
        /// </summary>
        /// <param name="dtReactTrayInfo"></param>
        /// <returns></returns>
        string GetFisrtPos(DataTable dtReactTrayInfo)
        {
            string FirstPos = "";
            DataRow[] dr = dtReactTrayInfo.Select("Pos='no" + ReactTrayNum.ToString() + "'");
            if (Convert.ToInt32(dr[0][1]) == 1)
            {
                for (int i = dtReactTrayInfo.Rows.Count - 1; i >= 0; i--)
                {
                    if (dtReactTrayInfo.Rows[i][1].ToString() != "1")
                    {
                        FirstPos = dtReactTrayInfo.Rows[i + 1][0].ToString();
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < dtReactTrayInfo.Rows.Count; i++)
                {
                    if (dtReactTrayInfo.Rows[i][1].ToString() == "1")
                    {
                        //后一个值一直覆盖前一个最终的赋值为最后一个位置
                        FirstPos = dtReactTrayInfo.Rows[i][0].ToString();
                        if (FirstPos == "no1" || FirstPos == "no2" || FirstPos == "no3")
                            continue;
                        if (FirstPos == "no4")
                        {
                            for (int num = dtReactTrayInfo.Rows.Count - 1; num > 0; num--)
                            {
                                if (dtReactTrayInfo.Rows[num][1].ToString() != "0")
                                {
                                    if (num == ReactTrayHoleNum - 1)
                                        FirstPos = dtReactTrayInfo.Rows[3][0].ToString();
                                    else
                                    {
                                        if (num + 1 - toUsedTube > 4)
                                            FirstPos = dtReactTrayInfo.Rows[num + 1][0].ToString();
                                    }
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
            }
            return FirstPos;
        }

        /// <summary>
        /// 实验运行调度
        /// </summary>
        /// <param name="lisTS">赋值testid后的实验进度</param>
        /// <returns></returns>
        public List<TestSchedule> ExperimentalScheduleAlgorithm(List<TestSchedule> lisTS, List<string> freeTime)
        {
            LogFile.Instance.Write("**********" + "计算调度" + "**********");
            lastAddLiquidTime = 0;
            int beforeTime = 0;
            for (int i = 0; i < lisTS.Count; i++)
            {
                TestSchedule testSchedule = lisTS[i];
                switch (lisTS[i].TestScheduleStep)
                {
                    case TestSchedule.ExperimentScheduleStep.AddLiquidTube:
                        List<TestSchedule> lisTestid = lisTS.FindAll(tx => tx.TestID == testSchedule.TestID);
                        int addLiquidStartTime = AllowTime(freeTime, lisTestid);
                        testSchedule.StartTime = addLiquidStartTime;
                        testSchedule.EndTime = testSchedule.StartTime + testSchedule.TimeLengh;
                        beforeTime = testSchedule.EndTime;
                        ChangFreeTime(freeTime, testSchedule.StartTime, testSchedule.TimeLengh);
                        //LogFile.Instance.Write("TESTID " + testSchedule.TestID + " 开始时间  ： " + addLiquidStartTime);
                        break;
                    case TestSchedule.ExperimentScheduleStep.AddBeads:
                        testSchedule.StartTime = beforeTime;
                        testSchedule.EndTime = testSchedule.StartTime + testSchedule.TimeLengh;
                        beforeTime = testSchedule.EndTime;
                        ChangFreeTime(freeTime, testSchedule.StartTime, testSchedule.TimeLengh);
                        break;
                    case TestSchedule.ExperimentScheduleStep.AddSingleR:
                        testSchedule.StartTime = beforeTime;
                        testSchedule.EndTime = testSchedule.StartTime + testSchedule.TimeLengh;
                        beforeTime = testSchedule.EndTime;
                        ChangFreeTime(freeTime, testSchedule.StartTime, testSchedule.TimeLengh);
                        break;
                    case TestSchedule.ExperimentScheduleStep.Incubation:
                        testSchedule.StartTime = beforeTime;
                        testSchedule.EndTime = testSchedule.StartTime + testSchedule.TimeLengh;
                        beforeTime = testSchedule.EndTime;
                        break;
                    case TestSchedule.ExperimentScheduleStep.Wash1:
                        testSchedule.StartTime = beforeTime;
                        testSchedule.EndTime = testSchedule.StartTime + testSchedule.TimeLengh;
                        beforeTime = testSchedule.EndTime;
                        //ChangFreeTime(freeTime, testSchedule.StartTime, 5);
                        //ChangFreeTime(freeTime, testSchedule.EndTime, 5);
                        break;
                    case TestSchedule.ExperimentScheduleStep.WashTray:
                        testSchedule.StartTime = beforeTime;
                        testSchedule.EndTime = testSchedule.StartTime + testSchedule.TimeLengh;
                        beforeTime = testSchedule.EndTime;
                        //ChangFreeTime(freeTime, testSchedule.StartTime, 5);
                        break;
                }
            }
            return lisTS;
        }

        /// <summary>
        /// 获取空闲时间
        /// </summary>
        /// <returns></returns>
        private List<string> GetFreeTime(List<TestSchedule> RunSchedule)
        {
            List<string> runFreeTime = new List<string>() { (sumTime + 2).ToString() + "-1000000" };
            for (int i = 0; i < RunSchedule.Count; i++)
            {
                TestSchedule ts = RunSchedule[i];
                for (int j = 0; j < runFreeTime.Count; j++)
                {
                    string[] minMaxTime = runFreeTime[j].Split('-');
                    int minTime = int.Parse(minMaxTime[0]);
                    int maxTime = int.Parse(minMaxTime[1]);
                    if (minTime == maxTime)
                    {
                        runFreeTime.Remove(runFreeTime[j]);
                        j = j - 1;
                        continue;
                    }
                    if (ts.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddBeads ||
                        ts.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddLiquidTube ||
                        ts.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddSingleR)
                    {
                        #region 计算空闲 加磁珠/加液/单独加试剂
                        if (ts.StartTime <= minTime)
                        {
                            if (ts.EndTime < minTime)
                            {
                                continue;
                            }
                            else if (ts.EndTime == minTime || (ts.EndTime < maxTime && ts.EndTime > minTime))
                            {
                                runFreeTime[j] = (ts.EndTime).ToString() + "-" + maxTime;
                                break;
                            }
                            else if (ts.EndTime >= maxTime)
                            {
                                runFreeTime.RemoveAt(j);
                                j = j - 1;
                                break;
                            }
                        }
                        else if (ts.StartTime > minTime && ts.StartTime < maxTime)
                        {
                            if (ts.EndTime > minTime && ts.EndTime < maxTime)
                            {
                                int index = 0;
                                runFreeTime[j] = (minTime).ToString() + "-" + (ts.StartTime);
                                index = j + 1;
                                runFreeTime.Insert(index, (ts.EndTime).ToString() + "-" + maxTime);
                                break;
                            }
                            else if (ts.EndTime >= maxTime)
                            {
                                runFreeTime[j] = (minTime).ToString() + "-" + (ts.StartTime);
                                break;
                            }

                        }
                        else if (ts.StartTime == maxTime)
                        {
                            if (ts.EndTime >= maxTime)
                            {
                                runFreeTime[j] = (minTime).ToString() + "-" + (maxTime);
                                break;
                            }
                        }
                        else if (ts.StartTime > maxTime)
                        {
                            continue;
                        }
                        #endregion
                        //ChangFreeTime(runFreeTime, ts.StartTime, ts.TimeLengh);
                    }
                    if (ts.TestScheduleStep == TestSchedule.ExperimentScheduleStep.Wash1)
                    {
                        //#region 计算空闲 到清洗盘
                        //if (ts.StartTime <= minTime)
                        //{
                        //    if (ts.StartTime + 5 < minTime)
                        //    {
                        //        continue;
                        //    }
                        //    else if (ts.StartTime + 5 == minTime || (ts.StartTime + 5 < maxTime && ts.StartTime + 5 > minTime))
                        //    {
                        //        runFreeTime[j] = (ts.StartTime + 5).ToString() + "-" + maxTime;
                        //        break;
                        //    }
                        //    else if (ts.StartTime + 5 >= maxTime)
                        //    {
                        //        runFreeTime.RemoveAt(j);
                        //        j = j - 1;
                        //        break;
                        //    }
                        //}
                        //else if (ts.StartTime > minTime && ts.StartTime < maxTime)
                        //{
                        //    if (ts.StartTime + 5 > minTime && ts.StartTime + 5 < maxTime)
                        //    {
                        //        int index = 0;
                        //        runFreeTime[j] = (minTime).ToString() + "-" + (ts.StartTime);
                        //        index = j + 1;
                        //        runFreeTime.Insert(index, (ts.StartTime + 5).ToString() + "-" + maxTime);
                        //        break;
                        //    }
                        //    else if (ts.StartTime + 5 >= maxTime)
                        //    {
                        //        runFreeTime[j] = (minTime).ToString() + "-" + (ts.StartTime);
                        //        break;
                        //    }

                        //}
                        //else if (ts.StartTime == maxTime)
                        //{
                        //    if (ts.StartTime + 5 >= maxTime)
                        //    {
                        //        runFreeTime[j] = (minTime).ToString() + "-" + (maxTime);
                        //        break;
                        //    }
                        //}
                        //else if (ts.StartTime > maxTime)
                        //{
                        //    continue;
                        //}
                        //#endregion

                        #region 计算空闲 到温育盘
                        if (ts.EndTime <= minTime)
                        {
                            if (ts.EndTime + 5 < minTime)
                            {
                                continue;
                            }
                            else if (ts.EndTime + 5 == minTime || (ts.EndTime + 5 < maxTime && ts.EndTime + 5 > minTime))
                            {
                                runFreeTime[j] = (ts.EndTime + 5).ToString() + "-" + maxTime;
                                break;
                            }
                            else if (ts.EndTime + 5 >= maxTime)
                            {
                                runFreeTime.RemoveAt(j);
                                j = j - 1;
                                break;
                            }
                        }
                        else if (ts.EndTime > minTime && ts.EndTime < maxTime)
                        {
                            if (ts.EndTime + 5 > minTime && ts.EndTime + 5 < maxTime)
                            {
                                int index = 0;
                                runFreeTime[j] = (minTime).ToString() + "-" + (ts.EndTime);
                                index = j + 1;
                                runFreeTime.Insert(index, (ts.EndTime + 5).ToString() + "-" + maxTime);
                                break;
                            }
                            else if (ts.EndTime + 5 >= maxTime)
                            {
                                runFreeTime[j] = (minTime).ToString() + "-" + (ts.EndTime);
                                break;
                            }

                        }
                        else if (ts.EndTime == maxTime)
                        {
                            if (ts.EndTime + 5 >= maxTime)
                            {
                                runFreeTime[j] = (minTime).ToString() + "-" + (maxTime);
                                break;
                            }
                        }
                        else if (ts.EndTime > maxTime)
                        {
                            continue;
                        }
                        #endregion
                    }
                    if (ts.TestScheduleStep == TestSchedule.ExperimentScheduleStep.WashTray)
                    {
                        //#region 计算空闲 到清洗盘
                        //if (ts.StartTime <= minTime)
                        //{
                        //    if (ts.StartTime + 5 < minTime)
                        //    {
                        //        continue;
                        //    }
                        //    else if (ts.StartTime + 5 == minTime || (ts.StartTime + 5 < maxTime && ts.StartTime + 5 > minTime))
                        //    {
                        //        runFreeTime[j] = (ts.StartTime + 5).ToString() + "-" + maxTime;
                        //        break;
                        //    }
                        //    else if (ts.StartTime + 5 >= maxTime)
                        //    {
                        //        runFreeTime.RemoveAt(j);
                        //        j = j - 1;
                        //        break;
                        //    }
                        //}
                        //else if (ts.StartTime > minTime && ts.StartTime < maxTime)
                        //{
                        //    if (ts.StartTime + 5 > minTime && ts.StartTime + 5 < maxTime)
                        //    {
                        //        int index = 0;
                        //        runFreeTime[j] = (minTime).ToString() + "-" + (ts.StartTime);
                        //        index = j + 1;
                        //        runFreeTime.Insert(index, (ts.StartTime + 5).ToString() + "-" + maxTime);
                        //        break;
                        //    }
                        //    else if (ts.StartTime + 5 >= maxTime)
                        //    {
                        //        runFreeTime[j] = (minTime).ToString() + "-" + (ts.StartTime);
                        //        break;
                        //    }

                        //}
                        //else if (ts.StartTime == maxTime)
                        //{
                        //    if (ts.StartTime + 5 >= maxTime)
                        //    {
                        //        runFreeTime[j] = (minTime).ToString() + "-" + (maxTime);
                        //        break;
                        //    }
                        //}
                        //else if (ts.StartTime > maxTime)
                        //{
                        //    continue;
                        //}
                        //#endregion
                        //ChangFreeTime(runFreeTime, ts.StartTime, 5);
                    }
                }
            }
            return runFreeTime;
        }
        /// <summary>
        /// 改变空闲时间
        /// </summary>
        /// <param name="freeTime"></param>
        /// <param name="startTime"></param>
        /// <param name="continueTime"></param>
        private void ChangFreeTime(List<string> freeTime, int startTime, int continueTime)
        {
            for (int i = 0; i < freeTime.Count; i++)
            {
                string[] minMaxTime = freeTime[i].Split('-');
                int minTime = int.Parse(minMaxTime[0]);
                int maxTime = int.Parse(minMaxTime[1]);
                if (minTime == maxTime)
                {
                    freeTime.Remove(freeTime[i]);
                    i = i - 1;
                    continue;
                }
                if (startTime >= minTime && startTime + continueTime <= maxTime)
                {
                    if ((startTime + continueTime) == maxTime && (startTime == minTime))
                    {
                        freeTime.Remove(freeTime[i]);
                    }
                    else
                    {
                        freeTime[i] = (startTime + continueTime).ToString() + "-" + maxTime;
                        freeTime.Insert(i, minTime.ToString() + "-" + startTime);
                    }
                    return;
                }
            }
        }
        /// <summary>
        /// 产生开始加液的位置
        /// </summary>
        /// <param name="freeTime">空闲时间</param>
        /// <param name="lisTS">当前实验步骤</param>
        /// <returns>开始加样时间</returns>
        public int AllowTime(List<string> freeTime, List<TestSchedule> lisTS)
        {
            int addLiquidStartTime = lastAddLiquidTime;
            bool isAllowFlag = false;
            int beforeTime = 0;
            int firstTime = 0;
            sequel:
            if (firstTime != 0)
            {
                addLiquidStartTime += 1;
            }
            for (int index = 0; index < lisTS.Count; index++)
            {
                firstTime++;
                switch (lisTS[index].TestScheduleStep)
                {
                    case TestSchedule.ExperimentScheduleStep.AddLiquidTube:
                        for (int i = 0; i < freeTime.Count; i++)
                        {
                            string[] minMaxTime = freeTime[i].Split('-');
                            int minTime = int.Parse(minMaxTime[0]);
                            if (minTime < addLiquidStartTime)
                            {
                                continue;
                            }
                            int maxTime = int.Parse(minMaxTime[1]);
                            if (addLiquidStartTime >= minTime && addLiquidStartTime + lisTS[index].TimeLengh <= maxTime
                                && addLiquidStartTime >= lastAddLiquidTime)
                            {
                                isAllowFlag = true;
                                break;
                            }
                            else
                            {
                                isAllowFlag = false;
                                continue;
                            }
                        }
                        if (isAllowFlag == false)
                        {
                            goto sequel;
                        }
                        //LogFile.Instance.Write("TESTID " + lisTS[index].TestID + " 上次时间  ： " + addLiquidStartTime);
                        beforeTime = addLiquidStartTime + lisTS[index].TimeLengh;
                        break;
                    case TestSchedule.ExperimentScheduleStep.AddBeads:
                        for (int i = 0; i < freeTime.Count; i++)
                        {
                            string[] minMaxTime = freeTime[i].Split('-');
                            int minTime = int.Parse(minMaxTime[0]);
                            if (minTime < addLiquidStartTime)
                            {
                                continue;
                            }
                            int maxTime = int.Parse(minMaxTime[1]);
                            if (beforeTime >= minTime && beforeTime + lisTS[index].TimeLengh <= maxTime)
                            {
                                isAllowFlag = true;
                                break;
                            }
                            else
                            {
                                isAllowFlag = false;
                                continue;
                            }
                        }
                        if (isAllowFlag == false)
                        {
                            goto sequel;
                        }
                        beforeTime = beforeTime + lisTS[index].TimeLengh;
                        break;
                    case TestSchedule.ExperimentScheduleStep.Incubation:
                        beforeTime = beforeTime + lisTS[index].TimeLengh;
                        break;
                    case TestSchedule.ExperimentScheduleStep.Wash1:
                        //for (int i = 0; i < freeTime.Count; i++)
                        //{
                        //    string[] minMaxTime = freeTime[i].Split('-');
                        //    int minTime = int.Parse(minMaxTime[0]);
                        //    int maxTime = int.Parse(minMaxTime[1]);
                        //    if (beforeTime >= minTime && beforeTime + 5 <= maxTime)
                        //    {
                        //        isAllowFlag = true;
                        //        break;
                        //    }
                        //    else
                        //    {
                        //        isAllowFlag = false;
                        //        continue;
                        //    }
                        //}
                        //if (isAllowFlag == false)
                        //{
                        //    goto sequel;
                        //}
                        beforeTime = beforeTime + lisTS[index].TimeLengh;
                        break;
                    case TestSchedule.ExperimentScheduleStep.AddSingleR:
                        for (int i = 0; i < freeTime.Count; i++)
                        {
                            string[] minMaxTime = freeTime[i].Split('-');
                            int minTime = int.Parse(minMaxTime[0]);
                            if (minTime < addLiquidStartTime)
                            {
                                continue;
                            }
                            int maxTime = int.Parse(minMaxTime[1]);
                            if (beforeTime >= minTime && beforeTime + lisTS[index].TimeLengh <= maxTime)
                            {
                                beforeTime = beforeTime + lisTS[index].TimeLengh;
                                isAllowFlag = true;
                                break;
                            }
                            else
                            {
                                isAllowFlag = false;
                                continue;
                            }
                        }
                        if (isAllowFlag == false)
                        {
                            goto sequel;
                        }
                        beforeTime = beforeTime + lisTS[index].TimeLengh;
                        break;
                    case TestSchedule.ExperimentScheduleStep.WashTray:
                        //for (int i = 0; i < freeTime.Count; i++)
                        //{
                        //    string[] minMaxTime = freeTime[i].Split('-');
                        //    int minTime = int.Parse(minMaxTime[0]);
                        //    int maxTime = int.Parse(minMaxTime[1]);
                        //    if (beforeTime >= minTime && beforeTime + 5 <= maxTime)
                        //    {
                        //        isAllowFlag = true;
                        //        break;
                        //    }
                        //    else
                        //    {
                        //        isAllowFlag = false;
                        //        continue;
                        //    }
                        //}
                        //if (isAllowFlag == false)
                        //{
                        //    goto sequel;
                        //}
                        lastAddLiquidTime = addLiquidStartTime;
                        beforeTime = beforeTime + lisTS[index].TimeLengh;
                        break;
                }
            }
            return addLiquidStartTime;
        }
        /// <summary>
        /// 将数据绑定到datagridview控件
        /// </summary>
        void BindData(List<TestSchedule> lisTS, int nostartid)
        {
            EventControlAdd += new Action<bool>(AddSchedule);
            EventControlAdd(false);
            if (lisTS.Count == 0 || lisTS == null)
            {
                return;
            }
            int tempTestBegain = 0;
            DbHelperOleDb db = new DbHelperOleDb(1);
            BLL.tbSampleInfo bllsp = new BLL.tbSampleInfo();
            DataTable dtSample = bllsp.GetList(" SendDateTime  >=#"
                      + DateTime.Now.ToString("yyyy-MM-dd") + "#and SendDateTime <#"
                      + DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + "# and Status = 0 order by SampleNo").Tables[0];
            for (int i = nostartid; i <= lisTS[lisTS.Count - 1].TestID; i++)
            {
                //实验项目步骤数量
                int ItemStepNum = lisTS.Count(ty => ty.TestID == i);
                int testTime = 0;
                string mergeSteps = "";
                for (int j = tempTestBegain; j < ItemStepNum + tempTestBegain; j++)
                {
                    switch (lisTS[j].TestScheduleStep)
                    {
                        case TestSchedule.ExperimentScheduleStep.AddLiquidTube:
                            mergeSteps += lisTS[j].singleStep;
                            testTime += lisTS[j].TimeLengh;
                            break;
                        case TestSchedule.ExperimentScheduleStep.AddBeads:
                            mergeSteps += "-B";
                            testTime += lisTS[j].TimeLengh;
                            break;
                        case TestSchedule.ExperimentScheduleStep.AddSingleR:
                            if (lisTS[j].singleStep == "R1")
                            {
                                mergeSteps += "-R1";
                            }
                            else if (lisTS[j].singleStep == "R2")
                            {
                                mergeSteps += "-R2";
                            }
                            else if (lisTS[j].singleStep == "R3")
                            {
                                mergeSteps += "-R3";
                            }
                            testTime += lisTS[j].TimeLengh; ;
                            break;
                        case TestSchedule.ExperimentScheduleStep.Wash1:
                            mergeSteps += "-W1";
                            testTime += lisTS[j].TimeLengh;
                            break;
                        case TestSchedule.ExperimentScheduleStep.Incubation:
                            mergeSteps += "-I";
                            testTime += lisTS[j].TimeLengh;
                            break;
                        case TestSchedule.ExperimentScheduleStep.WashTray:
                            mergeSteps += "-" + lisTS[j].singleStep;
                            testTime += lisTS[j].TimeLengh;
                            break;
                    }
                }
                TimeSpan ts = new TimeSpan(0, 0, testTime);
                testItem.SampleNo = lisTS[tempTestBegain].SampleNo;
                testItem.SampleID = lisTS[tempTestBegain].SampleID;
                testItem.ItemName = lisTS[tempTestBegain].ItemName;
                testItem.SamplePos = lisTS[tempTestBegain].samplePos;
                //频繁操作数据库浪费了大量的时间，改成一次查出来，然后从DataTable中查数据
                DataRow[] dr = dtSample.Select("Position ='" + lisTS[tempTestBegain].samplePos + "'");
                testItem.SampleType = dr[0]["SampleType"].ToString();
                testItem.RegentBatch = dr[0]["RegentBatch"].ToString();
                testItem.Schedule = mergeSteps;
                testItem.TestID = lisTS[tempTestBegain].TestID;
                testItem.TestStatus = "";
                testItem.TestTime = ts.ToString();
                testItem.SubstratePipe = "";//2018-10-17
                testItem.RegentPos = "";//2018-10-17
                BTestItem.Add(testItem);
                testItem = new TestItem();
                tempTestBegain = tempTestBegain + ItemStepNum;
            }
            this.dgvWorkListData.DataSource = BTestItem;//绑定数据源
            proBarInit(nostartid);
            EventControlAdd += new Action<bool>(AddSchedule);
            EventControlAdd(false);
        }
        /// <summary>
        /// 对该界面datagridview控件使用的自定义进度条控件赋值到list列表中
        /// </summary>
        void proBarInit(int nostartId)
        {
            for (int i = nostartId - 1; i < dgvWorkListData.Rows.Count; i++)
            {
                proBar = new Bar.ProgressBar();
                proBar.Visible = false;
                string steps = dgvWorkListData.Rows[i].Cells[6].Value.ToString();
                string[] step = steps.Split('-');
                proBar.BarNumber = step.Length;
                proBar.ValueChange();
                //EventControlAdd += new Action<bool>(AddSchedule);
                TestSchedule tempTs = new TestSchedule();
                for (int k = 0; k < step.Length; k++)
                {
                    switch (step[k])
                    {
                        //稀释步骤控件属性赋值。 LYN add 20171114
                        case "D":
                            proBar.BarColor[k] = Color.BlueViolet;
                            proBar.BarWidth[k] = 20;
                            break;
                        case "S":
                            proBar.BarColor[k] = Color.Red;
                            proBar.BarWidth[k] = 20;
                            break;
                        case "R2":
                            proBar.BarColor[k] = Color.Red;
                            proBar.BarWidth[k] = 13;
                            break;
                        case "R1":
                            proBar.BarColor[k] = Color.Red;
                            proBar.BarWidth[k] = 13;
                            break;
                        case "R3":
                            proBar.BarColor[k] = Color.Red;
                            proBar.BarWidth[k] = 13;
                            break;
                        case "I":
                            proBar.BarColor[k] = Color.Green;
                            proBar.BarWidth[k] = 80;
                            break;
                        case "B":
                            proBar.BarColor[k] = Color.Black;
                            proBar.BarWidth[k] = 15;
                            break;
                        case "Su":
                            proBar.BarColor[k] = Color.Pink;
                            proBar.BarWidth[k] = 10;
                            break;
                        case "W1":
                        case "W2":
                            proBar.BarColor[k] = Color.Blue;
                            proBar.BarWidth[k] = 30;
                            break;
                        case "R":
                            proBar.BarColor[k] = Color.Orange;
                            proBar.BarWidth[k] = 10;
                            break;
                    }
                }
                lisProBar.Add(proBar);
                //proBar = new Bar.ProgressBar();
            }
        }
        #endregion
        /// <summary>
        /// 运行状态改变影响相关按钮
        /// </summary>
        /// <param name="Flag"></param>
        void buttonEnableRun(bool Flag)
        {
            fbtnAddE.Enabled = fbtnAddS.Enabled = fbtnDelTest.Enabled = Flag;//2018-11-29 zlx mod
            btnLoadReagent.Enabled = btnLoadSample.Enabled = fbtnReturn.Enabled = !Flag;//2018-11-29 zlx mod
        }
        #region 实验方法
        /// <summary>
        /// 稀释液体积初始化 lyn add 20180611
        /// </summary>
        void diuVolInit()
        {
            for (int i = 9; i <= 10; i++)
            {
                diuleftVol[i - 1] = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "leftDiuVol", "", iniPathReagentTrayInfo);
            }
            //for (int i = 1; i <= 20; i++)
            //{
            //    diuleftVol[i - 1] = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "leftDiuVol", "", iniPathReagentTrayInfo);
            //}
        }
        /// <summary>
        /// 实验运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TestRun(object sender, EventArgs e)
        {
            if (NetCom3.Instance.stopsendFlag)
                NetCom3.Instance.stopsendFlag = false;
            //2018-09-25 zlx add
            frmMain.StartFlag = true;
            if (dgvWorkListData.RowCount == 0)
            {
                btnRunStatus();
                return;
            }
            //if (NetCom3.Instance.FReciveCallBack >= 3)//2018-10-24
            //    NetCom3.Instance.FReciveCallBack = 0;
            SetStopWatch();//倒计时功能 y add 0809
            TrayRemoveAllTube = false;//y add 抓空标志位，确定是否触发抓空异常
            isCleanPipeLineNow = false;//add y 20180727 是否在清洗清洗盘管路标志位
            //2018-07-23
            if (BQLiquaid)
                BQLiquaid = false;
            //2018-07-18
            #region 缺液提示
            if (frmMain.LackLq[0] > 0 || frmMain.LackLq[1] > 0 || frmMain.LackLq[2] > 0 || frmMain.LackLq[3] > 0)
            {
                //bool BStopFlag = false;
                StringBuilder st = new StringBuilder();
                if (frmMain.LackLq[0] > 0)
                {
                    st.Append("磁珠清洗液缺液！");
                }
                if (frmMain.LackLq[1] > 0)
                {
                    if (st.Length == 0)
                        st.Append("探针清洗液缺液！");
                    else
                        st.Append("\n探针清洗液缺液！");
                }
                if (frmMain.LackLq[2] > 0)
                {
                    if (st.Length == 0)
                        st.Append("废液桶已满！");
                    else
                        st.Append("\n废液桶已满！");
                }
                if (frmMain.LackLq[3] > 0)
                {
                    if (st.Length == 0)
                        st.Append("废管盒已满！");
                    else
                        st.Append("\n废管盒已满！");
                }
                if ((frmMain.StopFlag[0] || frmMain.StopFlag[1] || frmMain.StopFlag[2] || frmMain.StopFlag[3]))
                {
                    st.Append("\n实验无法开始，请添加清洗液！");
                    MessageBox.Show(st.ToString(), "缺液警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (btnRunStatus != null)
                    {
                        btnRunStatus();
                    }
                    buttonEnableRun(false);
                    return;
                }
                else
                {
                    st.Append("\n将会影响到此次实验,请确认是否继续？");
                    DialogResult dr = MessageBox.Show(st.ToString(), "缺液警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (dr != DialogResult.OK)
                    {
                        if (btnRunStatus != null)
                        {
                            btnRunStatus();
                        }
                        buttonEnableRun(false);
                        return;
                    }
                }

            }
            #endregion
            //开启仪器运行指示灯 2018-07-07
            //NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 08 00"), 5);
            //NetCom3.Instance.SingleQuery();
            RunFlag = (int)RunFlagStart.IsRuning;
            frmTestResult.BRun = true;//2018-08-15 zlx add
            btnLoadReagent.Enabled = btnLoadSample.Enabled = fbtnReturn.Enabled = fbtnReturn.Enabled = false;//2018-07-26 zlx add
            lisSavedId = new List<int>();
            MoveTubeThread = new Thread(new ParameterizedThreadStart(MoveTube));
            MoveTubeThread.IsBackground = true;
            MoveTubeThread.Start();
            EnterRun= false;
            if (!MachineInit())
            {
                //diuVolInit();
                //释放已经开启的移管线程
                if (MoveTubeThread != null)
                {
                    MoveTubeThread.Abort();
                }
                if (btnRunStatus != null)
                {
                    btnRunStatus();
                }
                RunFlag = (int)RunFlagStart.Stoped;
                buttonEnableRun(false);
                fbtnReturn.Enabled = true;//2018-07-31 zlx add
                return;
            }
            //diuVolInit();//20180614 zlx mod
            dtScalingPMT = new DataTable();
            dtScalingPMT.Columns.Add("PMT", typeof(int));
            dtScalingPMT.Columns.Add("Conc", typeof(double));
            dtScalingPMT.Columns.Add("ItemName", typeof(string));
            dtScalingPMT.Columns.Add("ItemType", typeof(int));
            dtScalingPMT.Columns.Add("RegentBatch", typeof(string));

            dtScalCacResult = new DataTable();
            dtScalCacResult.Columns.Add("ItemName", typeof(string));
            dtScalCacResult.Columns.Add("ItemType", typeof(int));
            dtScalCacResult.Columns.Add("Result", typeof(string));
            dtScalCacResult.Columns.Add("RegentBatch", typeof(string));
            while (washTrayTube() || lisMoveTube.Count > 0)
            {
                NetCom3.Delay(10);
            }
           
            #region 清洗盘反应管状态表
            DataTable dtIniWashTray = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
            for (int i = 0; i < dtIniWashTray.Rows.Count; i++)
            {
                //清洗盘添加反应盘位置字段，初始赋值该字段也需赋值。 LYN add 20171114
                dtWashTrayTubeStatus.Rows.Add(dtIniWashTray.Rows[i][0], dtIniWashTray.Rows[i][1], 0, 0, 0);
            }
            #endregion
            FirstTubeWash = true;//实验开始第一次清洗反应管放入
            lisTestSchedule.Sort(new SortRun());//按照步骤开始顺序排序
            TestStatusInfo = new Action<string, int>((str, index) =>
            {
                dgvWorkListData.Rows[index - 1].Cells["TestStatus"].Value = str;
            });//实验的实时运行实验状态
            lisSavedId = new List<int>();//2018-08-21 zlx add
            buttonEnableRun(true);//2018-11-29 zlx mod
            EnterRun = true;
            RunThread = new Thread(new ParameterizedThreadStart(GaTestRun));// GaTestRun  TestRun
            RunThread.IsBackground = true;
            RunThread.Start();

            timeReckon.Enabled = true;
            timeReckon.Interval = timeInterval;
            timeReckon.Start();

        }
        /// <summary>
        /// 设备各部件复位
        /// </summary>
        /// <returns>复位成功，返回true;否则返回false</returns>
        bool MachineInit()
        {
            //2018-08-16 zlx mod
            if (!RunConditionJudge())
            {
                return false;
            }
            NetCom3.Instance.ReceiveHandel += new Action<string>(Instance_ReceiveHandel);
            //仪器初始化
            lock (dataLocker)
            {
                Array.Clear(dataRecive, 0, 15);//2018-07-05 zlx add
            }
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 F1 02"), 5);
            if (!NetCom3.Instance.SingleQuery())
            {
                return false;
            }
            #region 判断各个模组是否初始化成功
            //int[] HandData = new int[16];
            //while (dataRecive[0] == null || dataRecive[2] != "F1")//2018-07-18 zlx add
            //{
            //    NetCom3.Delay(10);//2018-07-01
            //}
            //HandData = NetCom3.converTo10(dataRecive);
            /*
            if (HandData[4] != 255)
            {
                frmMsgShow.MessageShow("仪器初始化", "计数器模组初始化失败！");
                return false;
            }
            if (HandData[5] != 255)
            {
                frmMsgShow.MessageShow("仪器初始化", "抓手模组初始化失败！");
                return false;
            }
            if (HandData[6] != 255)
            {
                frmMsgShow.MessageShow("仪器初始化", "加样机模组初始化失败！");
                return false;
            }
            if (HandData[7] != 255)
            {
                frmMsgShow.MessageShow("仪器初始化", "清洗模组初始化失败！");
                return false;
            }
             */
            if (NetCom3.Instance.ErrorMessage != null)
            {
                //2018-09-06 zlx mod
                MessageBox.Show(NetCom3.Instance.ErrorMessage, "仪器初始化");
                return false;
            }
            BQLiquaid = true;//2018-07-21
            #endregion
            currentHoleNum = 1;
            washCountNum = 1;
            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
            LogFile.Instance.Write("==============复位成功，当前位置  " + washCountNum);
            #region 查询仪器中是否有使用过的反应管，若有夹出

            if (!washTrayTubeClear(null))
            {
                MessageBox.Show("清空清洗盘故障");
                return false;
            }
            if (RunFlag != (int)RunFlagStart.IsRuning) return false;
            if (!CleanTrayWashPipeline()) 
            {
                MessageBox.Show("清洗盘清洗故障");
                return false;
            }
            //if (RunFlag != (int)RunFlagStart.IsRuning) return false;
            if (!reactTrayTubeClear(null)) 
            {
                MessageBox.Show("反应盘清空故障");
                return false;
            }
            if (RunFlag != (int)RunFlagStart.IsRuning) return false;
            #endregion
            #region 检查温育盘信息
            DataTable dtReactTrayInfo = OperateIniFile.ReadConfig(iniPathReactTrayInfo);
            //反应盘上空反应管个数
            int sumReactTubeNum = 0;
            //反应管的位置
            string TrayPos = "";
            sumReactTubeNum = dtReactTrayInfo.Select("Value=1").Length;
            LogFile.Instance.Write("1温育盘有管数量" + sumReactTubeNum);
            if (sumReactTubeNum == 0)
            {
                OperateIniFile.WriteIniData("Tube", "ReacTrayTub", "", iniPathSubstrateTube);
            }
            else
            {
                string ReacTrayTub = OperateIniFile.ReadIniData("Tube", "ReacTrayTub", "", iniPathSubstrateTube);
                if (ReacTrayTub != "")
                {
                    string[] ReacTrayTubSplit = ReacTrayTub.Split(';');
                    foreach (string ReacTub in ReacTrayTubSplit)
                    {
                        if (TubeStop) return false;
                        if (ReacTub != "" && (OperateIniFile.ReadIniData("ReactTrayInfo", "No" + ReacTub, "", iniPathReactTrayInfo)) == "0")
                        {
                            rackToReact(int.Parse(ReacTub));
                        }
                    }
                }
            }
            dtReactTrayInfo = OperateIniFile.ReadConfig(iniPathReactTrayInfo);
            sumReactTubeNum = dtReactTrayInfo.Select("Value=1").Length;
            LogFile.Instance.Write("2温育盘有管数量" + sumReactTubeNum);
            if (sumReactTubeNum < toUsedTube)
            {
                DataRow[] dr = dtReactTrayInfo.Select("Pos='no50'");
                if (Convert.ToInt32(dr[0][1]) == 1)
                {
                    for (int i = 3; i < dtReactTrayInfo.Rows.Count; i++)
                    {
                        if (dtReactTrayInfo.Rows[i][1].ToString() == "1")
                        {
                            TrayPos = dtReactTrayInfo.Rows[i][0].ToString();
                        }
                        else
                            break;
                    }
                }
                else
                {
                    for (int i = 0; i < dtReactTrayInfo.Rows.Count; i++)
                    {
                        if (dtReactTrayInfo.Rows[i][1].ToString() == "1")
                        {
                            TrayPos = dtReactTrayInfo.Rows[i][0].ToString();
                        }
                    }
                }
            }


            //for (int i = 0; i < dtReactTrayInfo.Rows.Count; i++)
            //{
            //    if (dtReactTrayInfo.Rows[i][1].ToString() == "1")
            //    {
            //        sumReactTubeNum++;
            //        //后一个值一直覆盖前一个最终的赋值为最后一个位置
            //        TrayPos = dtReactTrayInfo.Rows[i][0].ToString();
            //    }
            //}
            //反应盘空反应管的个数小于15
            if (sumReactTubeNum < toUsedTube)
            {
                if (TrayPos == "")
                {
                    TrayPos = "NO3";
                }
                int LackTubeNum = toUsedTube - sumReactTubeNum;
                int OnePos = int.Parse(OperateIniFile.ReadIniData("Tube", "TubePos", "1", iniPathSubstrateTube)) - 1; //2018-09-28
                for (int i = 0; i < LackTubeNum; i++)
                {
                    if (TubeStop) return false;
                    MoveTubeStatus moveTube1 = new MoveTubeStatus();
                    moveTube1.StepNum = 0;
                    int putPos = int.Parse(TrayPos.Substring(2)) + i + 1;
                    if (putPos > ReactTrayHoleNum)
                        putPos = putPos - ReactTrayHoleNum + 3;
                    moveTube1.putTubePos = "1-" + putPos.ToString();
                    moveTube1.TestId = 0;
                    //管架夹新管未夹位置统计
                    //int tubeNum = 0;
                    //for (int j = 0; j < lisMoveTube.Count; j++)
                    //{
                    //    if (lisMoveTube[j].TakeTubePos.Split('-')[0] == "0")
                    //    {
                    //        tubeNum++;
                    //    }
                    //}
                    //int OnePos = int.Parse(OperateIniFile.ReadIniData("Tube", "TubePos", "1", iniPathSubstrateTube)) + tubeNum;
                    bool b;
                    //OnePos = BoardNextPos(OnePos, false, out b);
                    //moveTube1.TakeTubePos = "0-" + OnePos.ToString();
                    moveTube1.TakeTubePos = "0-" + 1;
                    lock (locker2)
                    {
                        lisMoveTube.Add(moveTube1);
                        //int ii = lisMoveTube.FindAll(tx => tx.TakeTubePos == "0-" + OnePos.ToString()+"").Count;
                    }
                }

            }
            #endregion
            return true;
        }


        /// <summary>
        /// 实验运行之间进行是否能运行的判断
        /// </summary>
        /// <returns>可以继续运行返回true，反之返回false</returns>
        bool RunConditionJudge()
        {
            lisScalingInfo = new List<ScalingInfo>();
            BLL.tbMainScalCurve bllMainCurve = new BLL.tbMainScalCurve();
            #region 从数据库中读取样本信息

            if (dtSampleRunInfo.Rows.Count == 0 || dtSampleRunInfo == null)
            {
                frmMsgShow.MessageShow("工作列表", "请重新装载样本！");
                return false;
            }
            #endregion

            //将所做的实验项目转化为List类型
            List<TestItem> lisItem = new List<TestItem>((BindingList<TestItem>)this.dgvWorkListData.DataSource).ToList();
            //对项目名称进行分组
            var ItemNames = lisItem.GroupBy(x => x.ItemName).Where(x => x.Count() >= 1).ToList();
            //查询试剂信息
            List<ReagentIniInfo> lisRIinfo = QueryReagentIniInfo();
            //所有的标准品或定标液
            //List<TestItem> lisAllScaling;
            //相同项目的样本
            List<TestItem> lisSameItem = new List<TestItem>();
            List<string> lisRegentBatch = new List<string>();
            foreach (var item in ItemNames)
            {
                List<ReagentIniInfo> itemRiInfo = lisRIinfo.FindAll(ty => (ty.ItemName == item.Key));
                #region 判断是否需要稀释，稀释液是否够用
                List<TestSchedule> list = lisTestSchedule.FindAll(ty => (ty.ItemName == item.Key && int.Parse(ty.dilutionTimes) > 1));
                DbHelperOleDb db = new DbHelperOleDb(0);
                if (list.Count > 0)
                {
                    BLL.tbDilute tbDilute = new BLL.tbDilute();
                    db = new DbHelperOleDb(0);
                    string DiluteCount = DbHelperOleDb.GetSingle(0,@"select DiluteCount from tbProject where ShortName ='" + item.Key + "'").ToString();
                    db = new DbHelperOleDb(0);
                    string DiluteName = DbHelperOleDb.GetSingle(0,@"select DiluteName from tbProject where ShortName ='" + item.Key + "'").ToString();
                    int DiuVolLeftCount = 0;
                    foreach (ReagentIniInfo RiInfo in itemRiInfo)
                    {
                        db = new DbHelperOleDb(3);
                        string pos = new BLL.tbReagent().GetModelList("BarCode='" + RiInfo.BarCode + "'")[0].Postion;
                        db = new DbHelperOleDb(3);
                        List<Model.tbDilute> ListDilute = tbDilute.GetModelList("DilutePos=" + int.Parse(pos) + "");
                        if (ListDilute.Count > 0)
                        {
                            string ValiData = ListDilute[0].ValiData;
                            if (ValiData != "" && Convert.ToDateTime(ValiData) < DateTime.Now.Date)
                            {
                                frmMsgShow.MessageShow("工作列表", pos + "试剂位稀释液过期，请及时进行更换");
                                return false;
                            }
                        }
                        DiuVolLeftCount = DiuVolLeftCount + RiInfo.leftDiuVol;
                    }
                    int SumDiluteVol = 0;
                    foreach (TestSchedule AddTest in list)
                    {
                        int GetSampleV = int.Parse(AddTest.AddLiqud.Split('-')[0]);
                        if (int.Parse(DiluteCount) > 1)
                        {
                            List<string> diuList = GetDiuVol(GetSampleV, DiluteName);
                            foreach (string duitec in diuList)
                            {
                                SumDiluteVol = SumDiluteVol + int.Parse(duitec.Split(';')[1]) + abanDiuPro;
                            }
                            GetSampleV = int.Parse(diuList[0].Split(';')[0]);
                        }

                        int ExtraDiuCount = int.Parse(AddTest.dilutionTimes) / (int.Parse(DiluteCount));
                        if (ExtraDiuCount > 1)
                        {
                            string diuInfo = DiuInfo.GetDiuInfo(ExtraDiuCount);
                            List<string> diuList = GetDiuVol(GetSampleV, diuInfo);
                            foreach (string duitec in diuList)
                            {
                                SumDiluteVol = SumDiluteVol + int.Parse(duitec.Split(';')[1]) + abanDiuPro;
                            }

                        }
                    }
                    if (DiuVolLeftCount < SumDiluteVol)
                    {
                        frmMsgShow.MessageShow("工作列表", item.Key + "项目稀释液不足，请及时进行更换！");
                        return false;
                    }
                }
                #endregion
               
                #region 屏蔽
                /*
                DbHelperOleDb db = new DbHelperOleDb(0);
                string DiluteName = DbHelperOleDb.GetSingle(@"select DiluteName from tbProject where ShortName ='" + item.Key + "'").ToString();
                //string NoUsePro = DbHelperOleDb.GetSingle(@"select NoUsePro from tbProject where ShortName ='" + item.Key + "'").ToString();
                if (DiluteName != "1")
                {
                    string[] DiluteCounts = DiluteName.Split(';');
                    BLL.tbDilute tbDilute = new BLL.tbDilute();
                    db = new DbHelperOleDb(3);
                    foreach (ReagentIniInfo RiInfo in itemRiInfo)
                    {
                        string ReagentType = OperateIniFile.ReadIniData("ReagentPos" + RiInfo.Postion, "ReagentType", "0", iniPathReagentTrayInfo);
                        if (ReagentType != "1") continue;
                        DataTable  ListDilute = tbDilute.GetList("DilutePos like '" + RiInfo.Postion + "-*' AND State=1").Tables[0];
                        for (int i = 0; i < ListDilute.Rows.Count; i++)
                        {
                            string ValiData = ListDilute.Rows[i]["ValiData"].ToString();
                            if (ValiData != "" && Convert.ToDateTime(ValiData) < DateTime.Now.Date)
                            {
                                frmMsgShow.MessageShow("工作列表", ListDilute.Rows[i]["DilutePos"] + "试剂位稀释液过期，请及时进行更换");
                                return false;
                            }
                        }
                    }
                    //DataTable dtDilute = tbDilute.GetModelList("DilutePos=" + itemRiInfo[0].+ "");
                    //查询所做实验中该项目的信息
                    lisSameItem = lisItem.FindAll(ty => (ty.ItemName == item.Key && !ty.SampleType.Contains("标准品") && !ty.SampleType.Contains("质控品")));
                    List<int> listDiuInfo = new List<int>();
                    decimal DiluteVol = 0;
                    for (int i = 0; i < DiluteCounts.Length; i++)
                    {
                        DiluteVol = DiluteVol + getDiuVol(_diuSumVol, int.Parse(DiluteCounts[i]))[1] + abanDiuPro;
                        Thread.Sleep(1);
                    }
                    //DiluteVol =DiluteVol+getDiuVol(_diuSumVol, int.Parse(DiluteCount))[1] + abanDiuPro;
                    decimal SumDiluteVol = DiluteVol * lisSameItem.Count;

                    int DiuVolLeftCount = 0;
                    if (itemRiInfo.Count > 0)
                    {
                        for (int i = 0; i < itemRiInfo.Count; i++)
                        {
                            string ReagentType = OperateIniFile.ReadIniData("ReagentPos" + itemRiInfo[i].Postion, "ReagentType", "0", iniPathReagentTrayInfo);
                            if (ReagentType != "1") continue;
                            if (itemRiInfo[i].LeftReagent1 > DiuNoUsePro)
                                DiuVolLeftCount = DiuVolLeftCount + itemRiInfo[i].LeftReagent1 -DiuNoUsePro;
                            if (itemRiInfo[i].LeftReagent2 > DiuNoUsePro)
                                DiuVolLeftCount = DiuVolLeftCount + itemRiInfo[i].LeftReagent2 -DiuNoUsePro;
                            if (itemRiInfo[i].LeftReagent3 >DiuNoUsePro)
                                DiuVolLeftCount = DiuVolLeftCount + itemRiInfo[i].LeftReagent3 -DiuNoUsePro;
                        }
                    }
                    if (DiuVolLeftCount < SumDiluteVol)
                    {
                        frmMsgShow.MessageShow("工作列表", item.Key + "项目稀释液不足，请及时进行更换！");
                        return false;
                    }
                */
                #endregion

                #region 检测试剂剩余测数是否够本次实验使用
                int itemLeftCount = 0;
                //查询所做实验中该项目的信息
                lisSameItem = lisItem.FindAll(ty => ty.ItemName == item.Key);
                //查询配置文件此项目的信息
                //List<ReagentIniInfo> itemRiInfo = lisRIinfo.FindAll(ty => ty.ItemName == item.Key);
                if (itemRiInfo.Count > 0)
                {
                    #region 判断是否有足够的试剂进行定标实验
                    List<TestItem> ScaSameItem = lisSameItem.FindAll(ty => (ty.SampleType.Contains("标准品")
                    /*|| ty.SampleType.Contains("质控品")*/));
                    if (ScaSameItem.Count > 0)
                    {
                        var scaBeach = ScaSameItem.GroupBy(x => x.RegentBatch).Where(x => x.Count() >= 1).ToList();
                        int scallLeftCount = 0;
                        foreach (var beach in scaBeach)
                        {
                            for (int i = 0; i < itemRiInfo.Count; i++)
                            {
                                string ReagentType = OperateIniFile.ReadIniData("ReagentPos" + itemRiInfo[i].Postion, "ReagentType", "0", iniPathReagentTrayInfo);
                                if (ReagentType == "1") continue;
                                if (itemRiInfo[i].BatchNum == beach.Key.ToString())
                                {
                                    scallLeftCount = scallLeftCount + itemRiInfo[i].LeftReagent1;
                                }
                            }
                            if (ScaSameItem.FindAll(ty => ty.RegentBatch == beach.Key.ToString()).Count > scallLeftCount)
                            {
                                if (itemLeftCount < lisSameItem.Count)
                                {
                                    frmMsgShow.MessageShow("工作列表", beach.Key + "批次的" + item.Key + "试剂不够本次定标(质控)测试！");
                                    return false;
                                }
                            }
                        }
                    }
                    #endregion
                    for (int i = 0; i < itemRiInfo.Count; i++)
                    {
                        string ReagentType = OperateIniFile.ReadIniData("ReagentPos" + itemRiInfo[i].Postion, "ReagentType", "0", iniPathReagentTrayInfo);
                        if (ReagentType == "1") continue;
                        itemLeftCount = itemLeftCount + itemRiInfo[i].LeftReagent1;
                    }
                    if (itemLeftCount < lisSameItem.Count)
                    {
                        frmMsgShow.MessageShow("工作列表", item.Key + "项目不够本次实验测试，请装载此项目");
                        return false;
                    }

                }
                else
                {
                    frmMsgShow.MessageShow("工作列表", "无" + item.Key + "项目，请装载此项目");
                    return false;
                }

                #endregion
                #region 查询是否有已有定标，并且对标准品浓度和吸光度进行保存
                db = new DbHelperOleDb(0);
                //DataTable dtItemInfo = DbHelperOleDb.Query(@"select ProjectType,CalPointConc,CalPointNumber from tbProject where ShortName ='" + item.Key + "'").Tables[0];
                //2018-07-31  zlx add ExpiryDate
                DataTable dtItemInfo = DbHelperOleDb.Query(0,@"select ProjectType,CalPointConc,CalPointNumber,ExpiryDate from tbProject where ShortName ='" + item.Key + "'").Tables[0];
                DataTable dtScal = new DataTable();
                dtScal.Columns.Add("SampleType", typeof(string));
                dtScal.Columns.Add("Num", typeof(int));
                dtScal.Columns.Add("Conc", typeof(string));
                //对装载试剂的试剂批号进行分组
                var regenBatchNums = lisRIinfo.FindAll(tx => tx.ItemName == item.Key)
                    .GroupBy(x => x.BatchNum).Where(x => x.Count() >= 1).ToList();
                if (regenBatchNums.Count > 0)
                {
                    foreach (var reBNum in regenBatchNums)
                    {
                        if (reBNum.Key.ToString() == "") continue;
                        //2018-08-25 zlx add
                        dtScal.Clear();

                        //查询该试剂批号的项目是否有历史定标
                        //object obPoints = DbHelperOleDb.GetSingle(@"select Points from tbScalingResult where ItemName ='"
                        //    + item.Key + "'and RegentBatch = '" + reBNum.Key + "' and status = 1");
                        //string points = obPoints == null ? "" : obPoints.ToString();
                        //2018-07-31 zlx mod
                        db = new DbHelperOleDb(1);
                        DataTable tbScalingResult = DbHelperOleDb.Query(1,@"select Points,ActiveDate from tbScalingResult where ItemName ='"
                        + item.Key + "'and RegentBatch = '" + reBNum.Key + "' and status = 1").Tables[0];
                        string points = null;
                        string ActiveDate = null;
                        foreach (DataRow dr in tbScalingResult.Rows)
                        {
                            if (dr["Points"].ToString() != "")
                                points = dr["Points"].ToString();
                            if (dr["ActiveDate"].ToString() != "")
                                ActiveDate = dr["ActiveDate"].ToString();
                        }
                        if (int.Parse(dtItemInfo.Rows[0][0].ToString()) == 1)
                        {
                            #region 定量实验
                            for (int i = 0; i < int.Parse(dtItemInfo.Rows[0][2].ToString()); i++)
                            {
                                switch (i)
                                {
                                    case 0:
                                        dtScal.Rows.Add("标准品A", lisItem.FindAll(ty => ty.SampleType == "标准品A"
                                            && ty.RegentBatch == reBNum.Key).Count);
                                        break;
                                    case 1:
                                        dtScal.Rows.Add("标准品B", lisItem.FindAll(ty => ty.SampleType == "标准品B"
                                            && ty.RegentBatch == reBNum.Key).Count);
                                        break;
                                    case 2:
                                        dtScal.Rows.Add("标准品C", lisItem.FindAll(ty => ty.SampleType == "标准品C"
                                        && ty.RegentBatch == reBNum.Key).Count);
                                        break;
                                    case 3:
                                        dtScal.Rows.Add("标准品D", lisItem.FindAll(ty => ty.SampleType == "标准品D"
                                        && ty.RegentBatch == reBNum.Key).Count);
                                        break;
                                    case 4:
                                        dtScal.Rows.Add("标准品E", lisItem.FindAll(ty => ty.SampleType == "标准品E"
                                        && ty.RegentBatch == reBNum.Key).Count);
                                        break;
                                    case 5:
                                        dtScal.Rows.Add("标准品F", lisItem.FindAll(ty => ty.SampleType == "标准品F"
                                        && ty.RegentBatch == reBNum.Key).Count);
                                        break;
                                    case 6:
                                        dtScal.Rows.Add("标准品G", lisItem.FindAll(ty => ty.SampleType == "标准品G"
                                        && ty.RegentBatch == reBNum.Key).Count);
                                        break;
                                }
                            }
                            int sNum = 0;
                            for (int i = 0; i < dtScal.Rows.Count; i++)
                            {
                                if (int.Parse(dtScal.Rows[i][1].ToString()) == 0)
                                {                                    
                                    sNum++;
                                }
                            }

                            ////lyq add 20190831
                            //if (lisItem.FindAll(ty => ty.SampleType.Contains("交叉污染")).Count > 0)
                            //    sNum = 0;

                            if (sNum > 0)//六个标准品中存在1个个数为0
                            {
                                if (int.Parse(dtScal.Rows[2][1].ToString()) > 0 && int.Parse(dtScal.Rows[4][1].ToString()) > 0)//C.E两点数量不为0
                                {
                                    db = new DbHelperOleDb(1);
                                    bool ExitsMainCurve = bllMainCurve.ExistsCurve(item.Key, reBNum.Key);
                                    if (ExitsMainCurve) //有主曲线
                                    {

                                        if (lisSameItem.FindAll(ty => (ty.SampleType.Contains("标准品"))).Count == 0)
                                        {
                                            #region 判断定标曲线是否可以使用
                                            string[] scpoint = points.Split(';');
                                            DataTable tempdt = new DataTable();
                                            tempdt.Columns.Add("Conc", typeof(float));
                                            tempdt.Columns.Add("PMT", typeof(float));
                                            DataTable DataMain = new DataTable();
                                            DataMain.Columns.Add("Conc", typeof(float));
                                            DataMain.Columns.Add("PMT", typeof(float));
                                            for (int i = 0; i < scpoint.Length; i++)
                                            {
                                                if (scpoint[i] != "")
                                                {
                                                    string[] pointinfo = scpoint[i].Replace("(", "").Replace(")", "").Split(',');
                                                    tempdt.Rows.Add(double.Parse(pointinfo[0]), double.Parse(pointinfo[1]));
                                                }
                                            }
                                            if (tempdt.Rows.Count == 2)
                                            {
                                                db = new DbHelperOleDb(1);
                                                DataTable _DataMain = bllMainCurve.GetList("ItemName='" + item.Key + "' AND RegentBatch='" + reBNum.Key + "'").Tables[0];
                                                if (_DataMain.Rows.Count > 0)
                                                {
                                                    string[] mainPoint = _DataMain.Rows[0]["Points"].ToString().Split(';');
                                                    for (int i = 0; i < mainPoint.Length; i++)
                                                    {
                                                        string[] pointinfo = mainPoint[i].Replace("(", "").Replace(")", "").Split(',');
                                                        if (pointinfo.Length == 2)
                                                            DataMain.Rows.Add(float.Parse(pointinfo[0]), float.Parse(pointinfo[1]));
                                                    }
                                                }
                                            }
                                            Calculater er = GetCalculater(DataMain, tempdt, item.Key);
                                            if (er.R2 < 0.99)
                                            {
                                                frmMsgShow.MessageShow("工作列表", "批号为：" + reBNum.Key + " 项目名称为：" + item.Key + "的试剂定标曲线相关性较差，请进行更换或重新定标！");
                                                return false;
                                            }
                                            #endregion
                                        }
                                        scalingInfo.ItemName = item.Key;
                                        scalingInfo.RegenBatch = reBNum.Key;
                                        scalingInfo.Num = dtScal.Rows[2][1].ToString() + "," + dtScal.Rows[4][1].ToString();
                                        scalingInfo.TestConc = dtItemInfo.Rows[0][1].ToString().Split(',')[2] + ","
                                            + dtItemInfo.Rows[0][1].ToString().Split(',')[4];
                                        scalingInfo.testType = int.Parse(dtItemInfo.Rows[0][0].ToString());
                                        lisScalingInfo.Add(scalingInfo);
                                        scalingInfo = new ScalingInfo();
                                    }
                                    else//无主曲线
                                    {
                                        //判断是否有历史定标
                                        if (points == null || points == "")
                                        {
                                            frmMsgShow.MessageShow("工作列表", "无主曲线无法两点校准进行计算！");
                                            return false;
                                        }
                                        else if (DateTime.Now.Date.AddDays(-Convert.ToInt32(dtItemInfo.Rows[0][3])).Date > Convert.ToDateTime(ActiveDate))
                                        {
                                            //2018-07-31 zlx add
                                            frmMsgShow.MessageShow("工作列表", "无主曲线已有的历史定标已经过期，请更换定标信息！");
                                            return false;
                                        }
                                        else
                                        {
                                            if (lisSameItem.FindAll(ty => (ty.SampleType.Contains("标准品"))).Count == 0)
                                            {
                                                #region 判断定标曲线是否可以使用
                                                string[] scpoint = points.Split(';');
                                                DataTable tempdt = new DataTable();
                                                tempdt.Columns.Add("Conc", typeof(float));
                                                tempdt.Columns.Add("PMT", typeof(float));
                                                DataTable DataMain = new DataTable();
                                                DataMain.Columns.Add("Conc", typeof(float));
                                                DataMain.Columns.Add("PMT", typeof(float));
                                                for (int i = 0; i < scpoint.Length; i++)
                                                {
                                                    if (scpoint[i] != "")
                                                    {
                                                        string[] pointinfo = scpoint[i].Replace("(", "").Replace(")", "").Split(',');
                                                        tempdt.Rows.Add(double.Parse(pointinfo[0]), double.Parse(pointinfo[1]));
                                                    }
                                                }
                                                if (tempdt.Rows.Count == 2)
                                                {
                                                    db = new DbHelperOleDb(1);
                                                    DataTable _DataMain = bllMainCurve.GetList("ItemName='" + item.Key + "' AND RegentBatch='" + reBNum.Key + "'").Tables[0];
                                                    if (_DataMain.Rows.Count > 0)
                                                    {
                                                        string[] mainPoint = _DataMain.Rows[0]["Points"].ToString().Split(';');
                                                        for (int i = 0; i < mainPoint.Length; i++)
                                                        {
                                                            string[] pointinfo = mainPoint[i].Replace("(", "").Replace(")", "").Split(',');
                                                            if (pointinfo.Length == 2)
                                                                DataMain.Rows.Add(float.Parse(pointinfo[0]), float.Parse(pointinfo[1]));
                                                        }
                                                    }
                                                }
                                                Calculater er = GetCalculater(DataMain, tempdt, item.Key);
                                                if (er.R2 < 0.99)
                                                {
                                                    frmMsgShow.MessageShow("工作列表", "批号为：" + reBNum.Key + " 项目名称为：" + item.Key + "的试剂定标曲线相关性较差，请进行更换或重新定标！");
                                                    return false;
                                                }
                                                #endregion
                                            }
                                            scalingInfo.ItemName = item.Key;
                                            scalingInfo.RegenBatch = reBNum.Key;
                                            scalingInfo.Num = "0";
                                            scalingInfo.TestConc = dtItemInfo.Rows[0][1].ToString();
                                            scalingInfo.testType = int.Parse(dtItemInfo.Rows[0][0].ToString());
                                            lisScalingInfo.Add(scalingInfo);
                                            scalingInfo = new ScalingInfo();
                                        }

                                    }
                                }
                                else
                                {
                                    //判断是否有历史定标
                                    if (points == null || points == "")
                                    {
                                        frmMsgShow.MessageShow("工作列表", "请对批号为：" + reBNum.Key + " 项目名称为：" + item.Key + "的试剂进行定标");
                                        return false;
                                    }
                                    else if (DateTime.Now.Date.AddDays(-Convert.ToInt32(dtItemInfo.Rows[0][3])).Date > Convert.ToDateTime(ActiveDate))
                                    {
                                        //2018-07-31 zlx add
                                        frmMsgShow.MessageShow("工作列表", "批号为：" + reBNum.Key + " 项目名称为：" + item.Key + "的试剂定标信息已经过期，请进行更换或重新定标！");
                                        return false;
                                    }
                                    else
                                    {
                                        if (lisSameItem.FindAll(ty => (ty.SampleType.Contains("标准品"))).Count == 0)
                                        {
                                            #region 判断定标曲线是否可以使用
                                            string[] scpoint = points.Split(';');
                                            DataTable tempdt = new DataTable();
                                            tempdt.Columns.Add("Conc", typeof(float));
                                            tempdt.Columns.Add("PMT", typeof(float));
                                            DataTable DataMain = new DataTable();
                                            DataMain.Columns.Add("Conc", typeof(float));
                                            DataMain.Columns.Add("PMT", typeof(float));
                                            for (int i = 0; i < scpoint.Length; i++)
                                            {
                                                if (scpoint[i] != "")
                                                {
                                                    string[] pointinfo = scpoint[i].Replace("(", "").Replace(")", "").Split(',');
                                                    tempdt.Rows.Add(double.Parse(pointinfo[0]), double.Parse(pointinfo[1]));
                                                }
                                            }
                                            if (tempdt.Rows.Count == 2)
                                            {
                                                db = new DbHelperOleDb(1);
                                                DataTable _DataMain = bllMainCurve.GetList("ItemName='" + item.Key + "' AND RegentBatch='" + reBNum.Key + "'").Tables[0];
                                                if (_DataMain.Rows.Count > 0)
                                                {
                                                    string[] mainPoint = _DataMain.Rows[0]["Points"].ToString().Split(';');
                                                    for (int i = 0; i < mainPoint.Length; i++)
                                                    {
                                                        string[] pointinfo = mainPoint[i].Replace("(", "").Replace(")", "").Split(',');
                                                        if (pointinfo.Length == 2)
                                                            DataMain.Rows.Add(float.Parse(pointinfo[0]), float.Parse(pointinfo[1]));
                                                    }
                                                }
                                            }
                                            Calculater er = GetCalculater(DataMain, tempdt, item.Key);
                                            if (er.R2 < 0.99)
                                            {
                                                frmMsgShow.MessageShow("工作列表", "批号为：" + reBNum.Key + " 项目名称为：" + item.Key + "的试剂定标曲线相关性较差，请进行更换或重新定标！");
                                                return false;
                                            }
                                            #endregion
                                        }
                                        scalingInfo.ItemName = item.Key;
                                        scalingInfo.RegenBatch = reBNum.Key;
                                        scalingInfo.Num = "0";
                                        scalingInfo.TestConc = dtItemInfo.Rows[0][1].ToString();
                                        scalingInfo.testType = int.Parse(dtItemInfo.Rows[0][0].ToString());
                                        lisScalingInfo.Add(scalingInfo);
                                        scalingInfo = new ScalingInfo();
                                    }
                                }
                            }
                            else//六个标准品都不为0
                            {

                                if (lisSameItem.FindAll(ty => (ty.SampleType.Contains("标准品"))).Count == 0)
                                {
                                    #region 判断定标曲线是否可以使用
                                    string[] scpoint = points.Split(';');
                                    DataTable tempdt = new DataTable();
                                    tempdt.Columns.Add("Conc", typeof(float));
                                    tempdt.Columns.Add("PMT", typeof(float));
                                    DataTable DataMain = new DataTable();
                                    DataMain.Columns.Add("Conc", typeof(float));
                                    DataMain.Columns.Add("PMT", typeof(float));
                                    for (int i = 0; i < scpoint.Length; i++)
                                    {
                                        if (scpoint[i] != "")
                                        {
                                            string[] pointinfo = scpoint[i].Replace("(", "").Replace(")", "").Split(',');
                                            tempdt.Rows.Add(double.Parse(pointinfo[0]), double.Parse(pointinfo[1]));
                                        }
                                    }
                                    if (tempdt.Rows.Count == 2)
                                    {
                                        db = new DbHelperOleDb(1);
                                        DataTable _DataMain = bllMainCurve.GetList("ItemName='" + item.Key + "' AND RegentBatch='" + reBNum.Key + "'").Tables[0];
                                        if (_DataMain.Rows.Count > 0)
                                        {
                                            string[] mainPoint = _DataMain.Rows[0]["Points"].ToString().Split(';');
                                            for (int i = 0; i < mainPoint.Length; i++)
                                            {
                                                string[] pointinfo = mainPoint[i].Replace("(", "").Replace(")", "").Split(',');
                                                if (pointinfo.Length == 2)
                                                    DataMain.Rows.Add(float.Parse(pointinfo[0]), float.Parse(pointinfo[1]));
                                            }
                                        }
                                    }
                                    Calculater er = GetCalculater(DataMain, tempdt, item.Key);
                                    if (er.R2 < 0.99)
                                    {
                                        frmMsgShow.MessageShow("工作列表", "批号为：" + reBNum.Key + " 项目名称为：" + item.Key + "的试剂定标曲线相关性较差，请进行更换或重新定标！");
                                        return false;
                                    }
                                    #endregion
                                }
                                scalingInfo.ItemName = item.Key;
                                scalingInfo.RegenBatch = reBNum.Key;
                                if (dtScal.Rows.Count == 7)
                                {
                                    scalingInfo.Num = dtScal.Rows[0][1].ToString() + "," + dtScal.Rows[1][1].ToString() + "," +
                                    dtScal.Rows[2][1].ToString() + "," + dtScal.Rows[3][1].ToString() + "," +
                                    dtScal.Rows[4][1].ToString() + "," + dtScal.Rows[5][1].ToString() + "," + dtScal.Rows[6][1].ToString();
                                }
                                else
                                {
                                    scalingInfo.Num = dtScal.Rows[0][1].ToString() + "," + dtScal.Rows[1][1].ToString() + "," +
                                        dtScal.Rows[2][1].ToString() + "," + dtScal.Rows[3][1].ToString() + "," +
                                        dtScal.Rows[4][1].ToString() + "," + dtScal.Rows[5][1].ToString();
                                }
                                scalingInfo.TestConc = dtItemInfo.Rows[0][1].ToString();
                                scalingInfo.testType = int.Parse(dtItemInfo.Rows[0][0].ToString());
                                lisScalingInfo.Add(scalingInfo);
                                scalingInfo = new ScalingInfo();
                            }                            
                            #endregion
                        }
                        else
                        {
                            #region 定性实验
                            dtScal.Rows.Add("定标液", lisItem.FindAll(ty => ty.SampleType == "定标液" && ty.RegentBatch == reBNum.Key).Count);
                            if (int.Parse(dtScal.Rows[0][1].ToString()) == 0)//实验运行中未装载定标液
                            {
                                if (points == "")
                                {
                                    frmMsgShow.MessageShow("工作列表", "无CUTOFF值，无法计算样本结果！");
                                    return false;
                                }
                                else
                                {
                                    scalingInfo.ItemName = item.Key;
                                    scalingInfo.RegenBatch = reBNum.Key;
                                    scalingInfo.Num = "0";
                                    scalingInfo.TestConc = dtItemInfo.Rows[0][1].ToString();
                                    scalingInfo.testType = int.Parse(dtItemInfo.Rows[0][0].ToString());
                                    lisScalingInfo.Add(scalingInfo);
                                    scalingInfo = new ScalingInfo();
                                }
                            }
                            else
                            {
                                scalingInfo.ItemName = item.Key;
                                scalingInfo.RegenBatch = reBNum.Key;
                                scalingInfo.Num = dtScal.Rows[0][1].ToString();
                                scalingInfo.TestConc = dtItemInfo.Rows[0][1].ToString();
                                scalingInfo.testType = int.Parse(dtItemInfo.Rows[0][0].ToString());
                                lisScalingInfo.Add(scalingInfo);
                                scalingInfo = new ScalingInfo();
                            }
                            #endregion
                        }
                    }
                }
                #endregion
            }
            #region 检测底物测数是否够本次实验使用
            string ValidDate1 = OperateIniFile.ReadIniData("Substrate1", "ValidDate", "", iniPathSubstrateTube);//2018-10-17 zlx add
            //string ValidDate2 = OperateIniFile.ReadIniData("Substrate2", "ValidDate", "", iniPathSubstrateTube);//2018-10-17 zlx add
            substrateNum1 = int.Parse(OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", iniPathSubstrateTube));
            substrateNum2 = 0;// int.Parse(OperateIniFile.ReadIniData("Substrate2", "LeftCount", "0", iniPathSubstrateTube));
            if (Convert.ToDateTime(ValidDate1) < DateTime.Now.Date)
            {
                string OverInfo = "";
                if (Convert.ToDateTime(ValidDate1) < DateTime.Now.Date)
                    OverInfo = "底物";
                LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "警告" + " *** " + "未读" + " *** " + "底物1已经过期");
                DialogResult r = MessageBox.Show("" + OverInfo + "已过期需更换。本次仍然继续测试？", "底物检查信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (DialogResult.OK != r)
                    return false;
            }
            /*
            if (Convert.ToDateTime(ValidDate1) < DateTime.Now.Date || Convert.ToDateTime(ValidDate2) < DateTime.Now.Date)
            {
                string OverInfo = "";
                if (Convert.ToDateTime(ValidDate1) < DateTime.Now.Date)
                    OverInfo = "底物1";
                //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "警告" + " *** " + "未读" + " *** " + "底物1已经过期");
                if (Convert.ToDateTime(ValidDate2) < DateTime.Now.Date)
                {
                    if (OverInfo != "")
                        OverInfo = OverInfo + "和底物2";
                    else
                        OverInfo = "底物2";
                }
                //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "警告" + " *** " + "未读" + " *** " + "底物2已经过期");
                DialogResult r = MessageBox.Show("" + OverInfo + "已过期需更换。本次仍然继续测试？", "底物检查信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (DialogResult.OK != r)
                    return false;
            }
             */
            if (substrateNum1 + substrateNum2 < BTestItem.Count)
            {
                frmMsgShow.MessageShow("工作列表", "底物测数不够本次实验测试，请装载底物！");
                return false;
            }
            #endregion
            List<TestItem> QCList = lisItem.FindAll(x => x.SampleType.Contains("质控品"));
            foreach (TestItem item in QCList)
            {
                string QCLevel;
                if (item.SampleType == "质控品H")
                {
                    QCLevel = "0";
                }
                else if (item.SampleType == "质控品M")
                {
                    QCLevel = "1";
                }
                else
                {
                    QCLevel = "2";
                }
                DbHelperOleDb db = new DbHelperOleDb(3);
                DataTable dtQCInfo = DbHelperOleDb.Query(3,@"select QCID,Batch,QCLevel from tbQC where status = '1' and ProjectName = '"
                                                            + item.ItemName + "'and QCLevel = '" + QCLevel + "' and Status = '1'").Tables[0];
                if (dtQCInfo == null || dtQCInfo.Rows.Count == 0)
                {
                    frmMsgShow.MessageShow("工作列表", "查找不到项目名称为:" + item.ItemName + ",质控类别与:" + item.SampleType + "相对应的相关质控的信息！");
                    return false;
                }
            }

            #region 检测管架剩余管数是否够本次实验使用
            //查询反应盘管信息
            /*
            DataTable dtReactTrayInfo = OperateIniFile.ReadConfig(iniPathReactTrayInfo);
            //反应盘上空反应管个数
            int sumReactTubeNum = 0;
            //反应管的位置
            //string TrayPos = "";
            for (int i = 0; i < dtReactTrayInfo.Rows.Count; i++)
            {
                if (dtReactTrayInfo.Rows[i][1].ToString() == "1")
                {
                    sumReactTubeNum++;
                    ////后一个值一直覆盖前一个最终的赋值为最后一个位置
                    //TrayPos = dtReactTrayInfo.Rows[i][0].ToString();
                }
            }
            List<int> lisTubeNum = new List<int>();
            lisTubeNum = QueryTubeNum();
            int sumTubeNum = 0;
            for (int i = 0; i < lisTubeNum.Count; i++)
            {
                sumTubeNum = sumTubeNum + lisTubeNum[i];
            }
            //添加稀释所占反应管的计算。 LYN add 20171114
            if (sumTubeNum + sumReactTubeNum < BTestItem.Count + DiuTubeNum + toUsedTube + 4)//2018-06-23 zlx mod//y add4because of clean tray clean pipe
            {
                frmMsgShow.MessageShow("工作列表", "管架反应管不够请装载新的反应管！");
                return false;
            }
             */
            #endregion
            return true;
        }
        /// <summary>
        /// 获取定标公式信息
        /// </summary>
        private Calculater GetCalculater(DataTable dtMain, DataTable dtCaculate, string projectName)
        {
            //新建定标方程类的实例
            CalculateFactory ft = new CalculateFactory();
            List<Data_Value> CurveData = new List<Data_Value>();
            //取到定标计算公式
            DbHelperOleDb db = new DbHelperOleDb(0);
            //数据库项目表中查询定标方程选择字段
            int calMode = int.Parse(DbHelperOleDb.GetSingle(0, @"select CalMode from tbProject where ShortName = '"
                                                            + projectName + "'").ToString());
            Calculater er = ft.getCaler(calMode);
            if (dtCaculate.Rows.Count == 2)
            {
                //两点定标数据存储
                DataTable dt = GetCorrectedPoints(dtCaculate, dtMain);
                if (dt == null) return null;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    CurveData.Add(new Data_Value()
                    {
                        Data = double.Parse(dt.Rows[i]["Conc"].ToString()),
                        DataValue = double.Parse(dt.Rows[i]["PMT"].ToString())
                    });
                }
            }
            else
            {
                for (int j = 0; j < dtCaculate.Rows.Count; j++)
                {
                    CurveData.Add(new Data_Value()
                    {
                        Data = double.Parse(dtCaculate.Rows[j]["Conc"].ToString()),
                        DataValue = double.Parse(dtCaculate.Rows[j]["PMT"].ToString())
                    });
                }
            }
            if (CurveData.Count > 0)//ltData标准品
            {
                for (int i = 0; i < CurveData.Count; i++)
                {
                    if (CurveData[i].Data == 0)
                    {
                        CurveData[i].Data = 0.0001;
                    }
                    if (CurveData[i].Data == 1)
                    {
                        CurveData[i].Data = 0.999999;
                    }
                    if (CurveData[i].DataValue == 0)
                    {
                        CurveData[i].DataValue = 0.0001;
                    }
                }
                for (int i = 0; i < CurveData.Count; i++)
                {
                    //对处理过的数据进行纠错
                    if (double.IsNaN(CurveData[i].DataValue) || double.IsNaN(CurveData[i].Data))
                    {
                        MessageBox.Show("函数计算错误，可能该数据不适合此回归模型", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                }
                //计算定标曲线的系数
                //for (int i = 0; i < CurveData.Count; i++)
                //{
                er.AddData(CurveData);
                er.Fit();
                //}
            }
            foreach (double par in er._pars)
            {
                if (double.IsNaN(par) || double.IsInfinity(par))
                {
                    //dtScalCacResult.Rows.Add(dtCaculate.Rows[0]["ItemName"].ToString(), 1, "");
                    MessageBox.Show(dtCaculate.Rows[0]["ItemName"].ToString() + "项目回归计算时出现运算错误，可能该数据不适合此回归模型", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
            return er;
        }

        /// <summary>
        /// 清洗盘清管
        /// </summary>
        bool washTrayTubeClear(object obj)
        {
            LogFile.Instance.Write("======>" + "清洗盘开始清管");
            DataTable dtWashTrayIni = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
            //TrayRemoveAllTube = true;//y add 抓空标志位，保证不触发抓空异常
            for (int i = 0; i < 30; i++)
            {
                if (RunFlag != (int)RunFlagStart.IsRuning || NetCom3.Instance.stopsendFlag)
                {
                    return false;
                }
                if (i != 0)
                {
                    //顺时针旋转1个孔位
                    AgainSend:
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-1).ToString("X2").Substring(6, 2)), 2);
                    //NetCom3.Instance.WashQuery();
                    if (!NetCom3.Instance.WashQuery())
                    {
                        if (NetCom3.Instance.WasherrorFlag == (int)ErrorState.Sendfailure)
                            goto AgainSend;
                        else if (NetCom3.Instance.WasherrorFlag == (int)ErrorState.OverTime)
                        {
                            NetCom3.Instance.stopsendFlag = true;
                            ShowWarnInfo("清洗盘旋转指令接收超时", "清洗", 1);
                            //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘夹管到温育盘时发生撞管！");
                            //MessageBox.Show("指令接收超时，实验已终止", "清洗指令错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //addLiquiding = false;
                            //AllStop();
                            return false;
                        }
                    }
                    currentHoleNum = currentHoleNum - 1;
                    //如果孔号小于等于0
                    if (currentHoleNum <= 0)
                    {
                        currentHoleNum = currentHoleNum + 30;
                    }
                    countWashHole(-1);
                    //LogFile.Instance.Write("==================  当前位置  " + currentHoleNum);
                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());

                    //顺时针应该是加的 jun add 
                    tubeHoleNum = tubeHoleNum + 1;
                    if (tubeHoleNum >= 31)
                    {
                        tubeHoleNum = tubeHoleNum - 30;
                    }
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
                MoveTubeUseFlag = true;
                WashTrayUseFlag = true;
                int IsKnockedCool = 0;
                AgainNewMove:
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
                LogFile.Instance.Write("==================  位置  " + washCountNum + "  扔管");
                //NetCom3.Instance.MoveQuery();
                if (!NetCom3.Instance.MoveQuery())
                {
                    #region 异常处理
                    if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.Sendfailure)
                        goto AgainNewMove;
                    else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked)
                    {
                        IsKnockedCool++;
                        if (IsKnockedCool < 2)
                        {
                            goto AgainNewMove;
                        }
                        else
                        {
                            setmainformbutten();
                            LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘扔废管时取管撞管,撞管位置为：" + tubeHoleNum);
                            //LogFileAlarm.Instance.Write(" *** " + "时间" + DateTime.Now.ToString("HH-mm-ss") + "洗盘扔废管时发生撞管孔位置" + tubeHoleNum + " *** ");
                            return false;
                        }
                    }
                    else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.OverTime)
                    {
                        setmainformbutten();
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在温育盘扔废管时接收数据超时！");
                        return false;
                    }
                    #endregion
                }
                LogFile.Instance.Write("==================  位置  " + washCountNum + "  扔管");
                OperateIniFile.WriteIniData("TubePosition", "No1", "0", iniPathWashTrayInfo);
                MoveTubeUseFlag = false;
                WashTrayUseFlag = false;
                #endregion
            }
            //TrayRemoveAllTube = false;//y add 抓空标志位
            LogFile.Instance.Write("======>" + "清洗盘清管结束");
            isHavedCount = true;
            return true;
        }
        void washTrayTubeClear()
        {
            LogFile.Instance.Write("======>" + "清洗盘开始清管");
            DataTable dtWashTrayIni = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
            //TrayRemoveAllTube = true;//y add 抓空标志位，保证不触发抓空异常
            for (int i = 0; i < 30; i++)
            {
                if (RunFlag != (int)RunFlagStart.IsRuning || NetCom3.Instance.stopsendFlag)
                {
                    return;
                }
                if (i != 0)
                {
                //顺时针旋转1个孔位
                AgainSend:
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-1).ToString("X2").Substring(6, 2)), 2);
                    //NetCom3.Instance.WashQuery();
                    if (!NetCom3.Instance.WashQuery())
                    {
                        if (NetCom3.Instance.WasherrorFlag == (int)ErrorState.Sendfailure)
                            goto AgainSend;
                        else if (NetCom3.Instance.WasherrorFlag == (int)ErrorState.OverTime)
                        {
                            NetCom3.Instance.stopsendFlag = true;
                            ShowWarnInfo("清洗盘旋转指令接收超时", "清洗", 1);
                            //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘夹管到温育盘时发生撞管！");
                            //MessageBox.Show("指令接收超时，实验已终止", "清洗指令错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //addLiquiding = false;
                            AllStop();
                        }
                    }
                    currentHoleNum = currentHoleNum - 1;
                    //如果孔号小于等于0
                    if (currentHoleNum <= 0)
                    {
                        currentHoleNum = currentHoleNum + 30;
                    }
                    countWashHole(-1);
                    //LogFile.Instance.Write("==================  当前位置  " + currentHoleNum);
                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());

                    //顺时针应该是加的 jun add 
                    tubeHoleNum = tubeHoleNum + 1;
                    if (tubeHoleNum >= 31)
                    {
                        tubeHoleNum = tubeHoleNum - 30;
                    }
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
                MoveTubeUseFlag = true;
                WashTrayUseFlag = true;
                int IsKnockedCool = 0;
            AgainNewMove:
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
                LogFile.Instance.Write("==================  位置  " + washCountNum + "  扔管");
                //NetCom3.Instance.MoveQuery();
                if (!NetCom3.Instance.MoveQuery())
                {
                    #region 异常处理
                    if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.Sendfailure)
                        goto AgainNewMove;
                    else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked)
                    {
                        IsKnockedCool++;
                        if (IsKnockedCool < 2)
                        {
                            goto AgainNewMove;
                        }
                        else
                        {
                            setmainformbutten();
                            LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘扔废管时取管撞管,撞管位置为：" + tubeHoleNum);
                            //LogFileAlarm.Instance.Write(" *** " + "时间" + DateTime.Now.ToString("HH-mm-ss") + "洗盘扔废管时发生撞管孔位置" + tubeHoleNum + " *** ");
                            return;
                        }
                    }
                    else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.OverTime)
                    {
                        setmainformbutten();
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘扔废管时接收数据超时！");
                        return;
                    }
                    #endregion
                }
                LogFile.Instance.Write("==================  位置  " + washCountNum + "  扔管");
                OperateIniFile.WriteIniData("TubePosition", "No1", "0", iniPathWashTrayInfo);
                MoveTubeUseFlag = false;
                WashTrayUseFlag = false;
                #endregion
            }
            //TrayRemoveAllTube = false;//y add 抓空标志位
            LogFile.Instance.Write("======>" + "清洗盘清管结束");
            isHavedCount = true;
        }
        public static bool isCleanPipeLineNow = false;//是否正在进行清洗管路
        private bool CleanTrayWashPipeline()//实验开始前清洗注液管路
        {
            isCleanPipeLineNow = true;
            if (!AddTubeInCleanTray())
                return false;
            CleanTrayMovePace(5);
            if (!AddTubeInCleanTray())
                return false;
            CleanTrayMovePace(4);
            if (RunFlag == (int)RunFlagStart.IsRuning||
                RunFlag == (int)RunFlagStart.IsStoping)
            {
                if (!AddTubeInCleanTray())
                    return false;
                CleanTrayMovePace(4);
                if (!AddTubeInCleanTray())
                    return false;
                CleanTrayMovePace(5 + isNewCleanTray);
                for (int i = 0; i < 4; i++)
                {
                    if (RunFlag != (int)RunFlagStart.IsRuning &&
                        RunFlag != (int)RunFlagStart.IsStoping || NetCom3.Instance.stopsendFlag) break;
                    CleanTrayWash(1);
                    CleanTrayMovePace(-1);
                    CleanTrayWash(2);
                    CleanTrayMovePace(-1);
                    CleanTrayWash(2);
                    CleanTrayMovePace(2);
                }
                CleanTrayMovePace(-5 - isNewCleanTray);
                if (!RemoveTubeOutCleanTray())
                    return false;
                CleanTrayMovePace(-4);
                if (!RemoveTubeOutCleanTray())
                    return false;
            }
            CleanTrayMovePace(-4);
            if (!RemoveTubeOutCleanTray())
                return false;
            CleanTrayMovePace(-5);
            if (!RemoveTubeOutCleanTray())
                return false;
            isCleanPipeLineNow = false;
            return true;
        }
        void CleanTrayWash(int oneOrTwo)//清洗盘清洗1:全部注液，2：全部吸液
        {
            string order;
            string substratePipe = "";
            if (oneOrTwo == 1)
            {
                //if (substrateNum1 != 0)
                //{
                //    substratePipe = "1";
                //}
                //else
                //{
                //    substratePipe = "2";
                //}
                //2018-10-17 zlx mod
                string LeftCount1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "", iniPathSubstrateTube);
                /*
                string LeftCount2 = OperateIniFile.ReadIniData("Substrate2", "LeftCount", "", iniPathSubstrateTube);
                if (int.Parse(LeftCount1) > 0)//substrateNum1
                {
                    if (int.Parse(LeftCount1) > int.Parse(LeftCount2) && int.Parse(LeftCount2)>0)
                        substratePipe = "2";
                    else
                        substratePipe = "1";
                }
                else
                {
                    substratePipe = "2";
                }
                 */
                substratePipe = "1";
                order = "EB 90 31 03 03 00 11 11 " + substratePipe + "0";//全部注液
            }
            else if (oneOrTwo == 2)
            {
                order = "EB 90 31 03 03 01 00 00 10";//全部吸液
            }
            else
            {
                throw new Exception();
            }
            AgainNewMove:
            NetCom3.Instance.Send(NetCom3.Cover(order), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                if (NetCom3.Instance.WasherrorFlag == (int)ErrorState.Sendfailure)
                    goto AgainNewMove;
                else if (NetCom3.Instance.WasherrorFlag == (int)ErrorState.OverTime)
                {
                    NetCom3.Instance.stopsendFlag = true;
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "清洗盘在清洗时接收数据超时！");
                    MessageBox.Show("指令接收超时，实验已终止", "清洗指令错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //addLiquiding = false;
                    AllStop();
                }
                else
                    return;
            }
            else
            {
                if (oneOrTwo == 1)
                {
                    OperateIniFile.WriteIniData("Substrate1", "LeftCount", (substrateNum1 - 1).ToString(), iniPathSubstrateTube);
                    substrateNum1--;
                }
                /*
                if (oneOrTwo == 1)
                {
                    if (substratePipe == "1")
                    {
                        OperateIniFile.WriteIniData("Substrate1", "LeftCount", (substrateNum1 - 1).ToString(), iniPathSubstrateTube);
                        substrateNum1--;
                    }
                    else
                    {
                        OperateIniFile.WriteIniData("Substrate2", "LeftCount", (substrateNum2 - 1).ToString(), iniPathSubstrateTube);
                        substrateNum2--;
                    }
                */
            }
        }
        private bool AddTubeInCleanTray(int pos = 0)//加空管到清洗盘取放管处
        {
            /*
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
            //MoveTubeUseFlag = true;
            //WashTrayUseFlag = true;
            //WashTurnFlag = true;
            int plate = boardPos % 88 == 0 ? boardPos / 88 - 1 : boardPos / 88;//几号板
            int column = boardPos % 11 == 0 ? boardPos / 11 - (plate * 8) : boardPos / 11 + 1 - (plate * 8);
            int hole = boardPos % 11 == 0 ? 11 : boardPos % 11;
             */
            int iNeedCool = 0;
            int IsKnockedCool = 0;
            AgainNewMove:
            //string order = "EB 90 31 01 06 " + plate.ToString("x2") + " " + column.ToString("x2") + " " + hole.ToString("x2");
            //NetCom3.Instance.Send(NetCom3.Cover(order), 1);
            string order = "EB 90 31 01 06";
            NetCom3.Instance.Send(NetCom3.Cover(order), 1);
            LogFile.Instance.Write("==============  加空管到清洗盘  " + washCountNum);
            if (!NetCom3.Instance.MoveQuery())
            {
                #region 发生异常处理
                if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull)
                {
                    iNeedCool++;
                    if (iNeedCool < 2)
                        goto AgainNewMove;
                    else
                    {
                        setmainformbutten();
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在暂存盘向清洗盘抓管时多次抓空!实验暂停加样！");
                        DialogResult tempresult = MessageBox.Show("移管手抓新管抓空！实验将停止运行！", "移管手错误！", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        return false;
                    }
                }
                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.LackTube)
                {
                    setmainformbutten();
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "理杯机缺管实验暂停加样！");
                    frmMessageShow message = new frmMessageShow();
                    message.MessageShow("移管手错误！", "理杯机缺管！");
                    //DialogResult tempresult = MessageBox.Show("移管手抓新管抓空！实验将停止运行！", "移管手错误！", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return false;
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
                        setmainformbutten();
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在暂存盘向清洗盘抓管时取管撞管！");
                        LogFile.Instance.Write("==============  移管手在暂存盘向清洗盘取放管处抓管取管撞管  " + currentHoleNum);
                        DialogResult tempresult = MessageBox.Show("移管手在暂存盘向清洗盘取放管处抓管取管撞管，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        return false;
                    }

                }
                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.putKnocked)
                {
                    IsKnockedCool++;
                    GAgainMove:
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
                    if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked)
                    {
                        IsKnockedCool++;
                        if (IsKnockedCool < 2)
                            goto GAgainMove;
                        else
                        {
                            setmainformbutten();
                            LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘扔管时取管撞管！");
                            LogFile.Instance.Write("==============  移管手在暂存盘向清洗盘扔管时取管撞管  " + currentHoleNum);
                            DialogResult tempresult = MessageBox.Show("移管手在清洗盘扔管时取管撞管，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                    goto AgainNewMove;
                }
                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.OverTime)
                {
                    setmainformbutten();
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在暂存盘向清洗盘抓管时接收数据超时！");
                    DialogResult tempresult = MessageBox.Show("移管手在暂存盘向清洗盘取放管处抓管接收数据超时，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return false;
                }
                #endregion
            }
            return true;
            //if (pos == 0)
            //{
            //    NetCom3.Instance.Send(NetCom3.Cover(order), 1);
            //    NetCom3.Instance.MoveQuery();
            //}
            //else
            //{
            //    NetCom3.Instance.Send(NetCom3.Cover(order), 4);
            //    //NetCom3.Instance.tempMoveOrder = NetCom3.Cover(order);
            //}
            //WashTurnFlag = false;
            //WashTrayUseFlag = false;
            //MoveTubeUseFlag = false;
        }


        private bool RemoveTubeOutCleanTray()//从清洗盘取放管处扔管
        {
            MoveTubeUseFlag = true;
            WashTrayUseFlag = true;
            //WashTurnFlag = true;
            int iNeedCool = 0;
            int IsKnockedCool = 0;
            AgainNewMove:
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
            LogFile.Instance.Write("==============  从清洗盘取放管处扔管  " + washCountNum);
            if (!NetCom3.Instance.MoveQuery())
            {
                #region 发生异常处理
                if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull)
                {
                    iNeedCool++;
                    if (iNeedCool < 2)
                    {
                        AgainReSetSend:
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 08"), 1);
                        if (!NetCom3.Instance.MoveQuery())
                        {
                            if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.Sendfailure)
                            {
                                LogFile.Instance.Write(DateTime.Now + "清洗盘复位返回指令重新发送！");
                                goto AgainReSetSend;
                            }
                            else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.OverTime)
                            {
                                NetCom3.Instance.stopsendFlag = true;
                                ShowWarnInfo("清洗盘复位返回指令接收超时", "清洗", 1);
                                AllStop();
                            }
                        }
                        goto AgainNewMove;
                    }
                    else
                    {
                        setmainformbutten();
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘扔废管时多次抓空！抓空位置为" + tubeHoleNum);
                        //LogFileAlarm.Instance.Write(" *** " + "时间" + DateTime.Now.ToString("HH-mm-ss") + "请洗盘抓空孔位置" + tubeHoleNum + " *** ");
                        DialogResult tempresult = MessageBox.Show("移管手在清洗盘扔废管时多次抓空，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        return false;
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
                    {
                        goto AgainNewMove;
                    }
                    else
                    {
                        setmainformbutten();
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘扔废管时取管撞管！撞管位置为：" + tubeHoleNum);
                        //LogFileAlarm.Instance.Write(" *** " + "时间" + DateTime.Now.ToString("HH-mm-ss") + "清洗盘扔废管时发生撞管孔位置" + tubeHoleNum + " *** ");
                        LogFile.Instance.Write("==============  移管手在清洗盘扔废管时取管撞管  " + washCountNum + "取扔管");
                        DialogResult tempresult = MessageBox.Show("移管手在清洗盘扔废管时取管撞管，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        return false;
                    }
                }
                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.OverTime)
                {
                    setmainformbutten();
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘扔废管时接收数据超时！");
                    DialogResult tempresult = MessageBox.Show("移管手在清洗盘扔废管时接收数据超时，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return false;
                }
                #endregion
            }
            //WashTurnFlag = false;
            MoveTubeUseFlag = false;
            WashTrayUseFlag = false;
            return true;
        }
        void CleanTrayMovePace(int pace)//旋转清洗盘相应空位，正数为逆时针旋转
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
                    ShowWarnInfo("清洗盘旋转指令接收超时", "清洗", 1);
                    //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘夹管到温育盘时发生撞管！");
                    //MessageBox.Show("指令接收超时，实验已终止", "清洗指令错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //addLiquiding = false;
                    AllStop();
                }
                else
                    return;
            }
            countWashHole(pace);
            currentHoleNum = currentHoleNum - 1;
            //如果孔号小于等于0
            if (currentHoleNum > 30)
            {
                currentHoleNum = currentHoleNum - 30;
            }
            if (currentHoleNum <= 0)
            {
                currentHoleNum = currentHoleNum + 30;
            }
            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
            //LogFile.Instance.Write("==================  当前位置  " + currentHoleNum);
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
        /// <summary>
        /// 温育反应盘清管
        /// </summary>
        bool reactTrayTubeClear(object obj)
        {
            DataTable dtInTrayIni = OperateIniFile.ReadConfig(iniPathReactTrayInfo);
            TrayRemoveAllTube = true;//y add 抓空标志位，保证不触发抓空异常
            for (int i = 0; i < dtInTrayIni.Rows.Count; i++)
            {
                if (NetCom3.Instance.stopsendFlag) return false;
                if (i == 0 || i == 1 || i == 2)
                {
                    if (int.Parse(dtInTrayIni.Rows[i][1].ToString()) >= 1)
                    {
                        int IsKnockedCool = 0;
                        AgainNewMove:
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 " + int.Parse(dtInTrayIni.Rows[i][0].ToString().Substring(2)).ToString("x2")), 1);
                        if (!NetCom3.Instance.MoveQuery())
                        {
                            #region 发生异常处理
                            if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.Sendfailure)
                            {
                                //if (NetCom3.Instance.waitAndAgainSend != null && NetCom3.Instance.waitAndAgainSend is Thread)
                                //{
                                //    NetCom3.Instance.waitAndAgainSend.Abort();
                                //}
                                goto AgainNewMove;
                            }
                            else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked)
                            {
                                IsKnockedCool++;
                                if (IsKnockedCool < 2)
                                {
                                    goto AgainNewMove;
                                }
                                else
                                {
                                    setmainformbutten();
                                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在温育盘扔废管时取管撞管！");
                                    return false ;
                                }
                            }
                            else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.OverTime)
                            {
                                setmainformbutten();
                                LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在温育盘扔废管时接收数据超时！");
                                return false;
                            }
                            #endregion
                        }
                        //修改反应盘信息
                        OperateIniFile.WriteIniData("ReactTrayInfo", "no" + int.Parse(dtInTrayIni.Rows[i][0].ToString().Substring(2)).ToString(), "0", iniPathReactTrayInfo);
                    }
                }
                else
                {
                    if (int.Parse(dtInTrayIni.Rows[i][1].ToString()) > 1)
                    {
                        int IsKnockedCool = 0;
                        AgainNewMove:
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 " + int.Parse(dtInTrayIni.Rows[i][0].ToString().Substring(2)).ToString("x2")), 1);
                        if (!NetCom3.Instance.MoveQuery())
                        {
                            #region 发生异常处理
                            if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.Sendfailure)
                            {
                                //if (NetCom3.Instance.waitAndAgainSend != null && NetCom3.Instance.waitAndAgainSend is Thread)
                                //{
                                //    NetCom3.Instance.waitAndAgainSend.Abort();
                                //}
                                goto AgainNewMove;
                            }
                            else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked)
                            {
                                IsKnockedCool++;
                                if (IsKnockedCool < 2)
                                {
                                    goto AgainNewMove;
                                }
                                else
                                {
                                    setmainformbutten();
                                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在温育盘扔废管时取管撞管！");
                                    return false;
                                }
                            }
                            else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.OverTime)
                            {
                                setmainformbutten();
                                LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在温育盘扔废管时接收数据超时！");
                                return false;
                            }
                            #endregion
                            //return;
                        }
                        //修改反应盘信息
                        OperateIniFile.WriteIniData("ReactTrayInfo", "no" + int.Parse(dtInTrayIni.Rows[i][0].ToString().Substring(2)).ToString(), "0", iniPathReactTrayInfo);
                    }
                }
            }
            TrayRemoveAllTube = false;//y add 抓空标志位
            return true;
        }
        void reactTrayTubeClear()
        {
            DataTable dtInTrayIni = OperateIniFile.ReadConfig(iniPathReactTrayInfo);
            TrayRemoveAllTube = true;//y add 抓空标志位，保证不触发抓空异常
            for (int i = 0; i < dtInTrayIni.Rows.Count; i++)
            {
                if (NetCom3.Instance.stopsendFlag) break;
                if (i == 0 || i == 1 || i == 2)
                {
                    if (int.Parse(dtInTrayIni.Rows[i][1].ToString()) >= 1)
                    {
                        int IsKnockedCool = 0;
                    AgainNewMove:
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 " + int.Parse(dtInTrayIni.Rows[i][0].ToString().Substring(2)).ToString("x2")), 1);
                        if (!NetCom3.Instance.MoveQuery())
                        {
                            #region 发生异常处理
                            if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.Sendfailure)
                            {
                                //if (NetCom3.Instance.waitAndAgainSend != null && NetCom3.Instance.waitAndAgainSend is Thread)
                                //{
                                //    NetCom3.Instance.waitAndAgainSend.Abort();
                                //}
                                goto AgainNewMove;
                            }
                            else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked)
                            {
                                IsKnockedCool++;
                                if (IsKnockedCool < 2)
                                {
                                    goto AgainNewMove;
                                }
                                else
                                {
                                    setmainformbutten();
                                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在温育盘扔废管时取管撞管！");
                                    return;
                                }
                            }
                            else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.OverTime)
                            {
                                setmainformbutten();
                                LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在温育盘扔废管时接收数据超时！");
                                return;
                            }
                            #endregion
                        }
                        //修改反应盘信息
                        OperateIniFile.WriteIniData("ReactTrayInfo", "no" + int.Parse(dtInTrayIni.Rows[i][0].ToString().Substring(2)).ToString(), "0", iniPathReactTrayInfo);
                    }
                }
                else
                {
                    if (int.Parse(dtInTrayIni.Rows[i][1].ToString()) > 1)
                    {
                        int IsKnockedCool = 0;
                    AgainNewMove:
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 " + int.Parse(dtInTrayIni.Rows[i][0].ToString().Substring(2)).ToString("x2")), 1);
                        if (!NetCom3.Instance.MoveQuery())
                        {
                            #region 发生异常处理
                            if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.Sendfailure)
                            {
                                //if (NetCom3.Instance.waitAndAgainSend != null && NetCom3.Instance.waitAndAgainSend is Thread)
                                //{
                                //    NetCom3.Instance.waitAndAgainSend.Abort();
                                //}
                                goto AgainNewMove;
                            }
                            else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked)
                            {
                                IsKnockedCool++;
                                if (IsKnockedCool < 2)
                                {
                                    goto AgainNewMove;
                                }
                                else
                                {
                                    setmainformbutten();
                                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在温育盘扔废管时取管撞管！");
                                    return;
                                }
                            }
                            else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.OverTime)
                            {
                                setmainformbutten();
                                LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在温育盘扔废管时接收数据超时！");
                                return;
                            }
                            #endregion
                            //return;
                        }
                        //修改反应盘信息
                        OperateIniFile.WriteIniData("ReactTrayInfo", "no" + int.Parse(dtInTrayIni.Rows[i][0].ToString().Substring(2)).ToString(), "0", iniPathReactTrayInfo);
                    }
                }
            }
            TrayRemoveAllTube = false;//y add 抓空标志位
        }
        /// <summary>
        /// 查询试剂盘中试剂的信息
        /// </summary>
        /// <returns></returns>
        List<ReagentIniInfo> QueryReagentIniInfo()
        {
            List<ReagentIniInfo> lisReagentIniInfo = new List<ReagentIniInfo>();
            ReagentIniInfo reagentIniInfo = new ReagentIniInfo();
            for (int i = 1; i <= RegentNum; i++)
            {
                reagentIniInfo.Postion = i.ToString();
                reagentIniInfo.BarCode = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "BarCode", "", iniPathReagentTrayInfo);
                reagentIniInfo.ItemName = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "ItemName", "", iniPathReagentTrayInfo);
                reagentIniInfo.TestCount = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "TestCount", "", iniPathReagentTrayInfo);
                reagentIniInfo.BatchNum = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "BachNum", "", iniPathReagentTrayInfo);
                string leftR1 = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "LeftReagent1", "", iniPathReagentTrayInfo);
                string leftR2 = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "LeftReagent2", "", iniPathReagentTrayInfo);
                string leftR3 = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "LeftReagent3", "", iniPathReagentTrayInfo);
                string leftR4 = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "LeftReagent4", "", iniPathReagentTrayInfo);
                string leftDiuVol = OperateIniFile.ReadIniData("ReagentPos" + i.ToString(), "leftDiuVol", "", iniPathReagentTrayInfo);
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
                if (leftDiuVol == "")
                {
                    reagentIniInfo.leftDiuVol = 0;
                }
                else
                {
                    reagentIniInfo.leftDiuVol = int.Parse(leftDiuVol);
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
        /// 反应盘夹到清洗盘标志位1
        /// </summary>
        bool ValueFlag3 = false;
        /// <summary>
        /// 反应盘夹到清洗盘标志位2
        /// </summary>
        bool ValueFlag4 = false;
        /// <summary>
        /// 实验运行
        /// </summary>
        /// <param name="obj"></param>
        void GaTestRun(object obj)
        {
            sumTime = 0;
            completeTestNums = 0;
            try
            {
                if (lisTestSchedule.Count > 0)
                {
                    ///开启仪器运行指示灯    
                    //NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 08 00"), 5);
                    //NetCom3.Instance.SingleQuery();
                    RunLightFlag = true;
                    SampleNumCurrent = dgvWorkListData.Rows.Count;
                    //while (SampleNumCurrent > StopList.Count && RunFlag == (int)RunFlagStart.IsRuning)
                    while (SampleNumCurrent >= StopList.Count
                        && SampleNumCurrent != 0
                        && RunFlag == (int)RunFlagStart.IsRuning)
                    {
                        Thread.Sleep(20);
                        NetCom3.ComWait.WaitOne();
                        TestStep = GaNextOne();
                        StartRun:
                        //if (TestStep == null)
                        //{
                        //    while (SampleNumCurrent > StopList.Count)//2018-07-12
                        //    {
                        //        NetCom3.Delay(10);
                        //        if (TestStep != null)
                        //            goto StartRun;
                        //    }
                        //    break;
                        //}
                        //2018-09-30 zlx add
                        DalayFlag = false;
                        waitTime:
                        if (RunFlag != (int)RunFlagStart.IsRuning)//增加一个停止按钮的过程中防止标志位被改写！！！！
                        {
                            continue;
                        }
                        //CanCom.Instance.ComWait.WaitOne();
                        #region 供应品状态变化
                        if (!frmMain.pauseFlag)
                        {
                            if ((frmMain.StopFlag[0] || frmMain.StopFlag[1] || frmMain.StopFlag[2]||frmMain.StopFlag[3])|| BFullReactTray)
                            {
                                AllPause();
                                GetNoStartList();
                            }
                            if (TubeStop)
                            {
                                if (!frmMain.pauseFlag)
                                {
                                    AllPause();
                                    GetNoStartList();
                                }
                                OperateIniFile.WriteIniData("Tube", "TubePos", "1", Application.StartupPath + "//SubstrateTube.ini");
                            }
                            if (SubstrateStop)
                            {
                                if (!frmMain.pauseFlag)
                                {
                                    AllPause();
                                    GetNoStartList();
                                }
                                
                            }
                        }
                        else
                        {
                            string LeftCount1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "", iniPathSubstrateTube);
                            if ((!frmMain.StopFlag[0] && !frmMain.StopFlag[1] && !frmMain.StopFlag[2] && !frmMain.StopFlag[3]) && StopList.Count > 0 && !TubeStop && (int.Parse(LeftCount1) > SampleNumCurrent) && !BFullReactTray ) 
                            {
                                if (SubstrateStop)
                                    SubstrateStop = false;
                                StopList.Clear();
                                Goon();
                                frmMain.pauseFlag = false;

                            }
                        }
                        #endregion
                        #region 更换管完成，去除缺管状态
                        if (AddTubeStop.Count > 0 && !TubeStop)
                        {
                            List<int> AddTubeStopCopy = AddTubeStop.GetRange(0, AddTubeStop.Count);
                            for (int i = 0; i < AddTubeStopCopy.Count; i++)
                            {
                                //int OnePos = int.Parse(OperateIniFile.ReadIniData("Tube", "TubePos", "1", iniPathSubstrateTube));
                                string ss = OperateIniFile.ReadIniData("ReactTrayInfo", "no" + AddTubeStopCopy[i], "", iniPathReactTrayInfo);
                                if (ss == "0" && !TubeStop)
                                {
                                    rackToReact(AddTubeStopCopy[i]);
                                    if (int.Parse(OperateIniFile.ReadIniData("ReactTrayInfo", "no" + AddTubeStopCopy[i], "", iniPathReactTrayInfo)) == 1)
                                    {
                                        lock (AddTubeStop)
                                        {
                                            AddTubeStop.Remove(AddTubeStopCopy[i]);
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        if (NetCom3.Instance.stopsendFlag)
                            AllStop();
                        Thread.Sleep(30);
                        if (TestStep == null)
                        {
                            Thread.Sleep(100);
                            if (((SampleNumCurrent >= StopList.Count)
                                && SampleNumCurrent != 0 && !SubstrateStop)
                                || (EmergencyFlag || addOrdinaryFlag))//2018-07-12
                            {
                                //NetCom3.Delay(10);
                                //if (TestStep != null)
                                goto StartRun;
                            }
                            else if ((TestStep != null) && !EmergencyFlag && !addOrdinaryFlag)
                            {
                                goto StartRun;
                            }
                            else
                            {
                                break;
                            }
                            //break;
                        }
                        #region 实验流程
                        switch (TestStep.TestScheduleStep)
                        {
                            case TestSchedule.ExperimentScheduleStep.DoNotTakeCareThis://this part add y 20180727
                                if (sumTime != TestStep.StartTime)
                                {
                                    if (sumTime > TestStep.StartTime)
                                    {
                                        sumTime = TestStep.StartTime + 10;
                                        stepTime = TestStep.EndTime;
                                        continue;
                                    }
                                    else
                                    {
                                        goto waitTime;
                                    }
                                }
                                break;
                            case TestSchedule.ExperimentScheduleStep.AddLiquidTube:
                                //2018-07-10 zlx add 
                                //if (frmMain.StopFlag[0] || frmMain.StopFlag[1] || frmMain.StopFlag[2])
                                //{
                                //    if (!StopList.Contains(TestStep.TestID.ToString()))
                                //        StopList.Add(TestStep.TestID.ToString());
                                //    //lisTestSchedule.RemoveAll(tx => (tx.TestID == TestStep.TestID));
                                //    break;
                                //}
                                while (addLiquiding)
                                    goto waitTime;
                                if (sumTime != TestStep.StartTime)
                                {
                                    if (sumTime > TestStep.StartTime)
                                    {
                                        sumTime = TestStep.StartTime;
                                    }
                                    else
                                    {
                                        goto waitTime;
                                    }
                                }
                                #region 判断是否暂停加样
                                //如果正在加急诊，未开始运行的不允许开始加样
                                if (EmergencyFlag || addOrdinaryFlag || frmMain.pauseFlag)
                                {
                                    break;
                                }
                                #region 稀释液试剂是否够用判断
                                //是否进行稀释
                                bool IsDiu = false;
                                int DiuNum = 0;
                                if (int.Parse(TestStep.dilutionTimes) > 0)
                                {
                                    DbHelperOleDb db = new DbHelperOleDb(0);
                                    string DiluteCount = DbHelperOleDb.GetSingle(0,@"select DiluteCount from tbProject where ShortName ='" + TestStep.ItemName + "'").ToString();
                                    db = new DbHelperOleDb(0);
                                    string DiluteName = DbHelperOleDb.GetSingle(0,@"select DiluteName from tbProject where ShortName ='" + TestStep.ItemName + "'").ToString();
                                    int GetSampleV = int.Parse(TestStep.AddLiqud.Split('-')[0]);
                                    if (DiluteName != "1")
                                    {
                                        List<string> diuList = GetDiuVol(GetSampleV, DiluteName);
                                        foreach (string duitec in diuList)
                                        {
                                            DiuNum = DiuNum + int.Parse(duitec.Split(';')[1]) + abanDiuPro;
                                        }
                                        GetSampleV = int.Parse(diuList[0].Split(';')[0]);
                                    }
                                    int ExDilute = int.Parse(TestStep.dilutionTimes) / int.Parse(DiluteCount);
                                    if (ExDilute > 1)
                                    {
                                        string diuInfo = DiuInfo.GetDiuInfo(ExDilute);
                                        List<string> diuList = GetDiuVol(int.Parse(TestStep.AddLiqud.Split('-')[0]), diuInfo);
                                        foreach (string duitec in diuList)
                                        {
                                            DiuNum = DiuNum + int.Parse(duitec.Split(';')[1]) + abanDiuPro;
                                        }
                                    }
                                    IsDiu = true;
                                }
                                DataRow[] drRg = dtRgInfo.Select("RgName='" + TestStep.ItemName.ToString() + "'");
                                int Rgcounum = 0;
                                List<int> DiuNumList = new List<int>();
                                foreach (var rowRg in drRg)
                                {
                                    Rgcounum = Rgcounum + Convert.ToInt32(rowRg["leftoverTestR1"].ToString());
                                    string diulest = OperateIniFile.ReadIniData("ReagentPos" + rowRg["Postion"].ToString(), "leftDiuVol", "", iniPathReagentTrayInfo);
                                    if (IsDiu && diulest!="" && int.Parse(diulest) - DiuNoUsePro > DiuNum)
                                        IsDiu = false;
                                }
                                if ((int.Parse(TestStep.dilutionTimes) > 1 && IsDiu) || Rgcounum <= 0)
                                {
                                    if (int.Parse(TestStep.dilutionTimes) > 1 && IsDiu)
                                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "" + TestStep.ItemName + "项目稀释液已用完，将停止后续的" + TestStep.ItemName + "实验！");
                                    if (Rgcounum <= 0)
                                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "" + TestStep.ItemName + "项目试剂已用完，将停止后续的" + TestStep.ItemName + "实验！");
                                    GetNoStartList(TestStep.ItemName, IsDiu);
                                    setmainformbutten();
                                    break;
                                }
                                #endregion
                                #region 底物是否够用判断
                                string LeftCount1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "", iniPathSubstrateTube);
                                List<TestSchedule> list = lisTestSchedule.FindAll(tx => (tx.TestID < NoStartTestId && tx.StartTime > 0));
                                int Sampleing = 0;
                                List<string> stoplist = new List<string>();
                                foreach (TestSchedule li in list)
                                {
                                    if (!stoplist.Contains(li.TestID.ToString()))
                                    {
                                        Sampleing++;
                                        stoplist.Add(li.TestID.ToString());
                                    }
                                }
                                if (int.Parse(LeftCount1) <= Sampleing - completeTestNums)
                                {
                                    SubstrateStop = true;
                                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "底物缺少，将暂停加样！");
                                    setmainformbutten();
                                    break;
                                }
                                #endregion
                                int movepos = TestStep.AddSamplePos + toUsedTube;
                                if (movepos > ReactTrayNum)
                                    movepos = movepos - ReactTrayNum + 3;
                                //string ss = OperateIniFile.ReadIniData("ReactTrayInfo", "no" + movepos, "", iniPathReactTrayInfo);
                                string ss = OperateIniFile.ReadIniData("ReactTrayInfo", "no" + TestStep.AddSamplePos, "", iniPathReactTrayInfo);
                                if (int.Parse(ss) > 1)
                                {
                                    BFullReactTray = true;
                                    break;
                                }
                                else if (int.Parse(ss) == 0)
                                {
                                    if (!AddTubeStop.Contains(TestStep.AddSamplePos))
                                    {
                                        AddTubeStop.Add(TestStep.AddSamplePos);
                                    }
                                    //TubeProblemFlag = true;
                                    goto waitTime;
                                }
                                #endregion
                                stepTime = TestStep.EndTime;
                                NoStartTestId++;
                                AddLiquidThread = new Thread(new ParameterizedThreadStart(addLiquid));
                                AddLiquidThread.IsBackground = true;
                                AddLiquidThread.Start(TestStep);
                                break;
                            case TestSchedule.ExperimentScheduleStep.AddSingleR:
                                //if (frmMain.StopFlag[0] || frmMain.StopFlag[1] || frmMain.StopFlag[2])//2018-07-10 zlx add
                                //{
                                //    if (!StopList.Contains(TestStep.TestID.ToString()))
                                //        StopList.Add(TestStep.TestID.ToString());
                                //    //lisTestSchedule.RemoveAll(tx => (tx.TestID == TestStep.TestID));
                                //    break;
                                //}
                                while (addLiquiding)
                                    goto waitTime;
                                if (sumTime != TestStep.StartTime)
                                {
                                    if (sumTime > TestStep.StartTime)
                                    {
                                        sumTime = TestStep.StartTime;

                                    }
                                    else
                                    {
                                        goto waitTime;
                                    }
                                }
                                if (NoStartTestId <= TestStep.TestID && (EmergencyFlag || addOrdinaryFlag || frmMain.pauseFlag))
                                {
                                    break;
                                }
                                stepTime = TestStep.EndTime;
                                int addSampos = lisTestSchedule.Find(ty => ty.ItemName == TestStep.ItemName && ty.SampleNo == TestStep.SampleNo
                                    && ty.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddLiquidTube && ty.TestID == TestStep.TestID).AddSamplePos;
                                TestStep.AddSamplePos = addSampos;
                                AddLiquidThread = new Thread(new ParameterizedThreadStart(addLiquid));
                                AddLiquidThread.IsBackground = true;
                                AddLiquidThread.Start(TestStep);
                                break;
                            case TestSchedule.ExperimentScheduleStep.AddBeads:
                                //if (frmMain.StopFlag[0] || frmMain.StopFlag[1] || frmMain.StopFlag[2])//2018-07-10 zlx add
                                //{
                                //    if (!StopList.Contains(TestStep.TestID.ToString()))
                                //        StopList.Add(TestStep.TestID.ToString());
                                //    //lisTestSchedule.RemoveAll(tx => (tx.TestID == TestStep.TestID));
                                //    break;
                                //}
                                while (addLiquiding)
                                    goto waitTime;
                                if (sumTime != TestStep.StartTime)
                                {
                                    if (sumTime > TestStep.StartTime)
                                    {
                                        sumTime = TestStep.StartTime;
                                    }
                                    else
                                    {
                                        goto waitTime;
                                    }
                                }
                                if (NoStartTestId <= TestStep.TestID && (EmergencyFlag || addOrdinaryFlag || frmMain.pauseFlag))
                                {
                                    break;
                                }
                                stepTime = TestStep.EndTime;
                                addSampos = lisTestSchedule.Find(ty => ty.ItemName == TestStep.ItemName && ty.SampleNo == TestStep.SampleNo
                                     && ty.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddLiquidTube && ty.TestID == TestStep.TestID).AddSamplePos;
                                TestStep.AddSamplePos = addSampos;
                                AddLiquidThread = new Thread(new ParameterizedThreadStart(addLiquid));
                                AddLiquidThread.IsBackground = true;
                                AddLiquidThread.Start(TestStep);
                                break;
                            case TestSchedule.ExperimentScheduleStep.Incubation:
                                //if (StopList.Contains(TestStep.TestID.ToString()))//2018-07-10 zlx add
                                //    break;
                                if (sumTime < TestStep.StartTime)
                                {
                                    goto waitTime;
                                }
                                if (NoStartTestId <= TestStep.TestID && (EmergencyFlag || addOrdinaryFlag || frmMain.pauseFlag))
                                {
                                    break;
                                }
                                if (this.IsHandleCreated)
                                {
                                    BeginInvoke(TestStatusInfo, new object[] { "正在温育", TestStep.TestID });
                                }
                                break;
                            case TestSchedule.ExperimentScheduleStep.Wash1:
                                //if (frmMain.StopFlag[1] || StopList.Contains(TestStep.TestID.ToString()))//2018-07-10 zlx add
                                //{
                                //    if (!StopList.Contains(TestStep.TestID.ToString()))
                                //        StopList.Add(TestStep.TestID.ToString());
                                //    break;
                                //}
                                if (sumTime != TestStep.StartTime)
                                {
                                    if (sumTime > TestStep.StartTime)
                                    {
                                        sumTime = TestStep.StartTime;

                                    }
                                    else
                                    {
                                        goto waitTime;
                                    }
                                }
                                if (NoStartTestId <= TestStep.TestID && (EmergencyFlag || addOrdinaryFlag || frmMain.pauseFlag))
                                {
                                    break;
                                }
                                W1EndTime.Add(TestStep.EndTime);
                                while (ValueFlag3)
                                {
                                    Thread.Sleep(30);
                                }
                                ValueFlag3 = true;
                                MoveTubeStatus moveTube4 = new MoveTubeStatus();
                                moveTube4.putTubePos = "2-1";
                                moveTube4.StepNum = TestStep.stepNum;
                                //移管列表加样位置赋值给列表中的反应盘取样位置。 LYN add 20171114
                                addSampos = lisTestSchedule.Find(ty => ty.ItemName == TestStep.ItemName && ty.SampleNo == TestStep.SampleNo
                                    && ty.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddLiquidTube && ty.TestID == TestStep.TestID).AddSamplePos;
                                moveTube4.TakeTubePos = "1-" + addSampos.ToString();
                                moveTube4.TestId = TestStep.TestID;
                                lock (locker2)
                                {
                                    lisMoveTube.Add(moveTube4);
                                }
                                ValueFlag3 = false;
                                if (FirstTubeWash)
                                {
                                    FirstTubeWash = false;
                                    washThread = new Thread(new ParameterizedThreadStart(washTray));
                                    washThread.IsBackground = true;
                                    washThread.Start();
                                }
                                break;
                            case TestSchedule.ExperimentScheduleStep.WashTray:
                                //2018-07-10 zlx add
                                //if (frmMain.StopFlag[0] || frmMain.StopFlag[2] || StopList.Contains(TestStep.TestID.ToString()))
                                //{
                                //    if (!StopList.Contains(TestStep.TestID.ToString()))
                                //        StopList.Add(TestStep.TestID.ToString());
                                //    break;
                                //}
                                if (sumTime != TestStep.StartTime)
                                {
                                    if (sumTime > TestStep.StartTime)
                                    {
                                        sumTime = TestStep.StartTime;

                                    }
                                    else
                                    {
                                        goto waitTime;
                                    }
                                }
                                if (NoStartTestId <= TestStep.TestID && (EmergencyFlag || addOrdinaryFlag || frmMain.pauseFlag))
                                {
                                    break;
                                }
                                while (ValueFlag4)
                                {
                                    Thread.Sleep(30);
                                }
                                ValueFlag4 = true;
                                MoveTubeStatus moveTube5 = new MoveTubeStatus();
                                moveTube5.putTubePos = "2-1";
                                moveTube5.StepNum = TestStep.stepNum;
                                //移管列表加样位置赋值给列表中的反应盘取样位置。 LYN add 20171114
                                addSampos = lisTestSchedule.Find(ty => ty.ItemName == TestStep.ItemName && ty.SampleNo == TestStep.SampleNo
                                    && ty.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddLiquidTube && ty.TestID == TestStep.TestID).AddSamplePos;
                                moveTube5.TakeTubePos = "1-" + addSampos.ToString();
                                moveTube5.TestId = TestStep.TestID;
                                lock (locker2)
                                {
                                    lisMoveTube.Add(moveTube5);
                                }
                                ValueFlag4 = false;
                                if (FirstTubeWash)
                                {
                                    FirstTubeWash = false;
                                    washThread = new Thread(new ParameterizedThreadStart(washTray));
                                    washThread.IsBackground = true;
                                    washThread.Start();
                                }
                                break;
                        }
                        #endregion
                    }
                    #region 停止实验流程及相关线程
                    while (RunFlag == (int)RunFlagStart.IsRuning)
                    {
                        //BTestResult.Count == dgvWorkListData.Rows.Count 
                        if (((SampleNumCurrent <= 0 && BTestResult.Count != 0) || (SampleNumCurrent == StopList.Count && StopList.Count > 0)) && MoveTubeUseFlag == false && !washTrayTube() && !ReactTrayTube())//2018-10-09 zlx mod
                        {
                            RunFlag = (int)RunFlagStart.IsStoping;
                            if (SpDiskUpdate != null)
                            {
                                this.BeginInvoke(new Action(() => { SpDiskUpdate(); }));
                            }
                            //稀释管数初始化。 LYN add 20171114
                            DiuTubeNum = 1;
                            ////将清洗盘当前取放管位置的孔号保存到配置文件中
                            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                            IniUpdateAccess();//由配置文件同步试剂与底物信息到数据库
                            if (MoveTubeThread != null)
                            {
                                MoveTubeThread.Abort();
                            }
                            if (washThread != null)
                            {
                                washThread.Abort();
                            }
                            if (AddLiquidThread != null)
                            {
                                AddLiquidThread.Abort();
                            }
                            if (CaculateThread != null)
                            {
                                CaculateThread.Abort();
                            }
                            if (ReadThread != null)
                            {
                                ReadThread.Abort();
                            }
                            timeReckon.Stop();
                            timer.Enabled = false;
                            #region 实验完成进行仪器初始化 2018-07-04 zlx add
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 F1 02"), 5);
                            NetCom3.Instance.SingleQuery();
                            washCountNum = 1;

                            #endregion
                            //关闭仪器运行指示灯 2018-07-07
                            //NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 08 01"), 5);
                            //NetCom3.Instance.SingleQuery();

                            if (!CleanTrayWashPipeline())
                            {
                                MessageBox.Show("实验结束清洗故障，请在稍后检查并清空清洗盘");
                            }


                            StopStopWatch();//终止倒计时
                            if (btnRunStatus != null)
                            {
                                this.BeginInvoke(new Action(() =>
                                {
                                    btnRunStatus();
                                }));
                            }
                            
                            if (StopList.Count > 0)
                            {
                                if (frmMain.StopFlag[0] || frmMain.StopFlag[1] || frmMain.StopFlag[2] || frmMain.StopFlag[3])
                                {
                                    string message = "";
                                    if (frmMain.StopFlag[0] || frmMain.StopFlag[1])
                                        message = "仪器缺液";
                                    if (frmMain.StopFlag[2])
                                    {
                                        if (message == "")
                                            message = "仪器废液桶已满";
                                        else
                                            message = message + ",废液桶已满";
                                    }
                                    if (frmMain.StopFlag[3])
                                    {
                                        if (message == "")
                                            message = "仪器废管盒已满";
                                        else
                                            message = message + ",废管盒已满";
                                    }
                                    MessageBox.Show(message+"，部分实验已完成！详情请在报警信息查看！", "实验状态", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                                }
                                else
                                    MessageBox.Show("供应品缺少，部分实验已完成！详情请在报警信息查看！", "实验状态", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                            }
                            else
                                MessageBox.Show("实验完成！", "实验状态", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);//2018-07-13 zlx mod
                            if (frmMain.pauseFlag)
                                frmMain.pauseFlag = false;
                            RunFlag = (int)RunFlagStart.Stoped;
                            RunLightFlag = false;
                            buttonEnableRun(false);
                            fbtnReturn.Enabled = true;//全部完成后才允许返回               
                            break;
                        }
                        Thread.Sleep(50);
                    }
                    #endregion
                }
            }
            catch (ThreadAbortException ex)
            {
                frmMsgShow.MessageShow("工作列表", ex.Message);
                IniUpdateAccess();
            }
            finally { }
        }
        /// <summary>
        /// 判断清洗盘是否有管，LYN add 20171114
        /// </summary>
        /// <returns></returns>
        bool washTrayTube()
        {
            DataTable dtWashTrayIni = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
            for (int i = 0; i < dtWashTrayIni.Rows.Count; i++)
            {
                if (dtWashTrayIni.Rows[i][1].ToString() == "1")
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 判断温育盘是否有正在进行的实验 2010-10-09 zlx add
        /// </summary>
        /// <returns></returns>
        bool ReactTrayTube()
        {
            DataTable dtReactTrayIni = OperateIniFile.ReadConfig(iniPathReactTrayInfo);
            for (int i = 0; i < dtReactTrayIni.Rows.Count; i++)
            {
                if (dtReactTrayIni.Rows[i][1].ToString() == "2")
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 获取未开始的实验编号
        /// </summary>
        public void GetNoStartList()
        {
            List<TestSchedule> list = lisTestSchedule.FindAll(tx => (tx.TestID >= NoStartTestId && tx.StartTime > 0));
            foreach (TestSchedule li in list)
            {
                if (!StopList.Contains(li.TestID.ToString()))
                {
                    StopList.Add(li.TestID.ToString());
                }
            }
        }
        public void GetNoStartList(string projectName, bool IsDiu)
        {
            NetCom3.ComWait.Reset();
            List<string> stopList = new List<string>();
            List<TestSchedule> Temp = lisTestSchedule.FindAll(tx => (tx.TestID >= NoStartTestId && tx.ItemName == projectName));
            foreach (var item in Temp)
            {
                item.TestScheduleStep = TestSchedule.ExperimentScheduleStep.DoNotTakeCareThis;
                if (!stopList.Contains(item.TestID.ToString()))
                {
                    stopList.Add(item.TestID.ToString());
                    foreach (DataGridViewRow dr in dgvWorkListData.Rows)
                    {
                        if (int.Parse(dr.Cells[1].Value.ToString()) == item.TestID)
                        {
                            if (IsDiu)
                                dr.Cells["TestStatus"].Value = "稀释液已用完";
                            else
                                dr.Cells["TestStatus"].Value = "试剂用完";
                            dr.DefaultCellStyle.BackColor = Color.Gray;
                        }
                    }
                }
            }
            SampleNumCurrent = SampleNumCurrent - stopList.Count;
        }
        /// <summary>
        /// 正在执行的实验步骤
        /// </summary>
        TestSchedule _GaDoingOne;
        /// <summary>
        /// 获取下一个实验步骤
        /// </summary>
        /// <returns></returns>
        TestSchedule GaNextOne()
        {
            int temp = 0;
            if (_GaDoingOne == null)
            {
                _GaDoingOne = lisTestSchedule[0];
                return _GaDoingOne;
            }
            foreach (TestSchedule wp in lisTestSchedule)
            {
                temp++;
                if (_GaDoingOne == wp)
                    break;
            }
            if (temp <= lisTestSchedule.Count - 1)
            {
                TestSchedule thisone = null;
                thisone = lisTestSchedule[temp];
                _GaDoingOne = thisone;
                return _GaDoingOne;
            }
            else
            {
                _GaDoingOne = null;
                return null;
            }
        }

        /// <summary>
        /// 获取稀释吸液量 2018-06-01
        /// 样本量/稀释液量
        /// </summary>
        /// <param name="Times"></param>
        /// <returns></returns>
        int[] getDiuVol(int SumVol, int Times)
        {
            int[] DiuVoInfo = new int[2];//稀释信息 样本量/稀释液量
            if (Times != 0)
            {
                DiuVoInfo[0] = int.Parse((Math.Round(Convert.ToDouble(SumVol) / Times)).ToString());
                DiuVoInfo[1] = SumVol - DiuVoInfo[0];
            }
            return DiuVoInfo;
        }
        /// <summary>
        /// 获取稀释加样信息
        /// </summary>
        /// <param name="AddLiquidVol">样本使用量</param>
        /// <param name="DiutimeInfo">稀释倍数信息</param>
        /// <returns>稀释加样信息</returns>
        List<string> GetDiuVol(int AddLiquidVol, string DiutimeInfo)
        {
            List<string> DiuVoInfo = new List<string>();
            string[] Diutimes = DiutimeInfo.Split(';');
            for (int i = Diutimes.Length; i > 0; i--)
            {
                if (Diutimes[i - 1] != "" && int.Parse(Diutimes[i - 1]) != 1)
                {
                    //获取稀释完成最少体积、
                    int MinSunDiuV = AddLiquidVol + DiuLeftVol;
                    float AddSample = float.Parse(MinSunDiuV.ToString()) / float.Parse(Diutimes[i - 1]);
                    int AddSampleV = 0;
                    int AddDiuV = 0;
                    if (AddSample < 5)
                        AddSampleV = 5;
                    else
                    {
                        if ((AddSample - (int)AddSample) != 0)
                        {
                            AddSampleV = (int)AddSample + 1;
                        }
                        else
                            AddSampleV = (int)AddSample;
                    }
                    AddDiuV = AddSampleV * int.Parse(Diutimes[i - 1]) - AddSampleV;
                    DiuVoInfo.Insert(0, AddSampleV.ToString() + ";" + AddDiuV);
                    AddLiquidVol = AddSampleV;
                }
            }
            return DiuVoInfo;
        }
        private bool addLiquiding=false ;
        /// <summary>
        /// 实验运行中加液步骤
        /// </summary>
        /// <param name="testS">当前步骤对象</param>
        private void addLiquid(object testS)
        {
            addLiquiding = true;
            //lock (addLiquiding)
            //{
            //NetCom3.ComWait.WaitOne(-1);
            //frmMain.barrelFlag = true;
            //将object形转换为TestSchedule形
            TestSchedule testTempS = testS as TestSchedule;
            //获取试剂盘上对应实验名称的试剂信息
            IniUpdateAccess();
            DbHelperOleDb dba = new DbHelperOleDb(3);
            DataTable dtRegent = new BLL.tbReagent().GetList("ReagentName='" + testTempS.ItemName.ToString() + "' AND Status='正常' AND leftoverTestR1>=0 ").Tables[0];
            DataRow[] drRg;
            //DataRow[] drRg = dtRgInfo.Select("RgName='" + testTempS.ItemName.ToString() + "' AND leftoverTestR1>0");
            foreach (DataRow dr in dtRegent.Rows)
            {
                drRg = dtRgInfo.Select("Postion='" + dr["Postion"] + "'");
                if (drRg.Length > 0)
                {
                    drRg[0]["leftoverTestR1"] = dr["leftoverTestR1"].ToString();
                    drRg[0]["leftoverTestR2"] = dr["leftoverTestR2"].ToString();
                    drRg[0]["leftoverTestR3"] = dr["leftoverTestR3"].ToString();
                    drRg[0]["leftoverTestR4"] = dr["leftoverTestR4"].ToString();
                }
            }
            //drRg = dtRgInfo.Select("RgName='" + testTempS.ItemName.ToString() + "' AND leftoverTestR1>0").OrderBy(x => int.Parse(x["leftoverTestR1"].ToString())).ToArray();
            //DataRow[] 
            drRg = dtRgInfo.Select("RgName='" + testTempS.ItemName.ToString() + "' AND leftoverTestR1>0")
                .OrderBy(x => int.Parse(x["leftoverTestR1"].ToString())).ToArray();
            int rgPos = 0;
            int rgindex = -1;
            //int NoUsePro = 0;//2018-10-13 zlx add
            int AddErrorCount;
            switch (testTempS.TestScheduleStep)
            {
                case TestSchedule.ExperimentScheduleStep.AddLiquidTube:
                    string[] TestStepSingle = testTempS.singleStep.Split('-');
                    string[] LiquidVol = testTempS.AddLiqud.Split('-');
                    for (int j = 0; j < TestStepSingle.Length; j++)
                    {
                        //稀释步骤添加。 LYN add 20171114
                        if (TestStepSingle[j] == "D")
                        {
                            AddingSampleFlag = true;
                            while (!this.IsHandleCreated)//为了防止出现“在创建窗口句柄之前，不能在控件上调用 Invoke 或 BeginInvoke”的错误
                            {
                                Thread.Sleep(30);
                            }
                            BeginInvoke(TestStatusInfo, new object[] { "正在稀释", testTempS.TestID });

                            lisProBar[testTempS.TestID - 1].BarColor[StepIndex(dgvWorkListData.Rows[testTempS.TestID - 1].Cells[6].Value.ToString(), "D")]
                                = Color.Yellow;
                            lisProBar[testTempS.TestID - 1].Invalidate();
                            string[] diupos = testTempS.dilutionPos.Split('-');
                            DbHelperOleDb db = new DbHelperOleDb(0);//2019-02-28 zlx add
                            string DiluteCount = DbHelperOleDb.GetSingle(0,@"select DiluteCount from tbProject where ShortName 
                                                                             = '" + testTempS.ItemName + "'").ToString();
                            db = new DbHelperOleDb(0);
                            string DiluteName = DbHelperOleDb.GetSingle(0,@"select DiluteName from tbProject where ShortName ='" + testTempS.ItemName + "'").ToString();
                            int ExtraDiluteC = int.Parse(testTempS.dilutionTimes) / int.Parse(DiluteCount);
                            if (ExtraDiluteC > 1)
                            {
                                if (DiluteName == "1")
                                    DiluteName = DiuInfo.GetDiuInfo(ExtraDiluteC);
                                else
                                    DiluteName = DiuInfo.GetDiuInfo(ExtraDiluteC) + ";" + DiluteName;
                            }
                            #region 稀释
                            if (DiluteName != "1")
                            {
                                List<string> diuList = GetDiuVol(int.Parse(LiquidVol[j]), DiluteName);
                                string Pos = "";
                                for (int i = 0; i < diuList.Count; i++)
                                {
                                    int SampleVol = int.Parse(diuList[i].Split(';')[0]);
                                    int DiuVol = int.Parse(diuList[i].Split(';')[1]);
                                    #region 获取稀释液位置
                                    List<ReagentIniInfo> currentReagent = QueryReagentIniInfo().Where(reagent => reagent.ItemName == testTempS.ItemName.ToString()
                                        && reagent.leftDiuVol > 0).ToList();
                                    if (currentReagent.Count > 0)
                                    {
                                        foreach (var reagent in currentReagent)
                                        {
                                            string diulest = OperateIniFile.ReadIniData("ReagentPos" +
                                                reagent.Postion, "leftDiuVol", "", iniPathReagentTrayInfo);
                                            if ((!string.IsNullOrEmpty(diulest)) && (int.Parse(diulest) > DiuVol + abanDiuPro + DiuNoUsePro))
                                            {
                                                rgPos = int.Parse(reagent.Postion);//获取该试剂位置编号
                                                //rgindex = g;
                                                break;
                                            }
                                        }
                                    }
                                    #endregion
                                    if (rgPos > 0)
                                    {
                                        //新管位置
                                        int pos = int.Parse(diupos[i]);
                                        #region 加新管
                                        while (MoveTubeUseFlag)
                                            NetCom3.Delay(10);
                                        rackToReact(pos);
                                        if (TubeStop)//加新管时管架为空 2019-02-22
                                        {
                                            RemoveTestList(testTempS,"稀释样品时暂存盘缺管");
                                            #region 稀释不成功扔管
                                            for (int index = 1; index < 4; index++)
                                            {
                                                string exitStatus = OperateIniFile.ReadIniData("ReactTrayInfo",
                                                "no" + index, "", iniPathReactTrayInfo);
                                                if (exitStatus == "1" || exitStatus == "2")
                                                {
                                                    ReactToAband(index);
                                                }
                                            }
                                            #endregion
                                            goto outAddLiquidTube;
                                        }
                                        #endregion
                                        #region 加稀释液
                                        AddErrorCount = 0;
                                        AddErrorCount = AddLiquid(rgPos, pos, DiuVol);
                                        if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.OverTime)
                                        {
                                            NetCom3.Instance.stopsendFlag = true;
                                            ShowWarnInfo("加稀释液指令接收超时", "加样", 1);
                                            AllStop();
                                        }
                                        if (AddErrorCount > 0)
                                        {
                                            if (AddErrorCount > 1)
                                            {
                                                NetCom3.Instance.stopsendFlag = true;
                                                ShowWarnInfo("加样针加稀释液时撞针", "加样", 1);
                                                AllStop();
                                            }
                                            else
                                            {
                                                MoveTubeListAddTubeDispose(pos);
                                                RemoveTestList(testTempS , "加样针加稀释液时撞针");
                                            }
                                            break;
                                        }
                                        #endregion
                                        #region 加需稀释的样本
                                        int samplePos;//获取样本位置。
                                        if (i == 0)
                                            samplePos = testTempS.samplePos;
                                        else
                                            samplePos = int.Parse(diupos[i - 1]);
                                        AddErrorCount = 0;
                                        if (i == 0)
                                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 01 " + samplePos.ToString("x2") + " " + pos.ToString("x2")
                                                + " " + SampleVol.ToString("x2")), 0);
                                        else
                                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 02 " + samplePos.ToString("x2") + " " + pos.ToString("x2")
                                                + " " + SampleVol.ToString("x2")), 0);
                                        if (!NetCom3.Instance.SPQuery())
                                        {
                                            #region 异常处理
                                            Again:
                                            string againSend = "";
                                            if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.IsKnocked && AddErrorCount < 2)
                                            {
                                                AddErrorCount++;
                                                //重新发送指令
                                                if (againSend == "")
                                                    againSend = "EB 90 31 02 01 " + samplePos.ToString("x2") + " " + pos.ToString("x2")
                                                    + " 00";
                                            }
                                            else if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                                            {
                                                //重新发送指令
                                                if (againSend == "")
                                                    againSend = "EB 90 31 02 01 " + samplePos.ToString("x2") + " " + pos.ToString("x2")
                                                    + " " + SampleVol.ToString("x2");
                                            }
                                            else if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.OverTime)
                                            {
                                                NetCom3.Instance.stopsendFlag = true;
                                                ShowWarnInfo("加样针加样本时指令接收超时", "加样", 1);
                                                AllStop();
                                            }
                                            int sendFlag = SendAgain(againSend, 0);
                                            if (sendFlag == (int)ErrorState.Sendfailure)
                                                goto Again;
                                            else if (sendFlag == (int)ErrorState.IsKnocked)
                                                AddErrorCount++;
                                            #endregion
                                        }
                                        if (NetCom3.Instance.LiquidLevelDetectionFlag == (int)LiquidLevelDetectionAlarm.Low &&
                                            LiquidLevelDetectionEvent != null &&
                                            (!(dgvWorkListData.Rows[testTempS.TestID - 1].Cells["SampleType"].Value.ToString().Contains("标准品"))))
                                            LiquidLevelDetectionEvent("样本位置" + samplePos + "：样本出现凝块或样本量不足", 2);
                                        //配置文件进行修改
                                        //diupos[0] = pos.ToString();
                                        OperateIniFile.WriteIniData("ReactTrayInfo", "no" + diupos[0], "2", iniPathReactTrayInfo);
                                        if (AddErrorCount > 0)
                                        {
                                            if (AddErrorCount > 1)
                                            {
                                                NetCom3.Instance.stopsendFlag = true;
                                                ShowWarnInfo("加样针加样本时发生撞针", "加样", 1);
                                                AllStop();
                                            }
                                            else
                                            {
                                                MoveTubeListAddTubeDispose(pos);
                                                RemoveTestList(testTempS , "加样针加样本时发生撞针");
                                            }
                                            break;
                                        }
                                        //混匀
                                        AgainMix:
                                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 04 01 " + pos.ToString("x2")), 0);
                                        if (!NetCom3.Instance.SPQuery())//NetCom3.Instance.MoveQuery()
                                        {
                                            #region 异常处理

                                            if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                                            {
                                                goto AgainMix;
                                            }
                                            else
                                            {
                                                NetCom3.Instance.stopsendFlag = true;
                                                ShowWarnInfo("混匀" + pos + "位置指令接收异常", "加样", 1);
                                                AllStop();
                                                break;
                                            }
                                            #endregion
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        RemoveTestList(testTempS, "当前项目稀释液不足");
                                        return;
                                    }
                                    #region 扔上一次稀释的管
                                    if (i > 0)
                                    {
                                        ReactToAband(int.Parse(diupos[i - 1]));
                                    }
                                    #endregion
                                }
                            }
                            #endregion
                            lisProBar[testTempS.TestID - 1].BarColor[StepIndex(dgvWorkListData.Rows[testTempS.TestID - 1].Cells[6].Value.ToString(), "D")]
                                = Color.Gray;
                            toGray(testTempS.TestID - 1, StepIndex(dgvWorkListData.Rows[testTempS.TestID - 1].Cells[6].Value.ToString(), "D"));
                            lisProBar[testTempS.TestID - 1].Invalidate();
                            AddingSampleFlag = false;
                        }
                        else if (TestStepSingle[j] == "S")
                        {
                            AddingSampleFlag = true;
                            while (!this.IsHandleCreated)//为了防止出现“在创建窗口句柄之前，不能在控件上调用 Invoke 或 BeginInvoke”的错误
                            {
                                Thread.Sleep(30);
                            }
                            BeginInvoke(TestStatusInfo, new object[] { "正在加样", testTempS.TestID });
                            lisProBar[testTempS.TestID - 1].BarColor[StepIndex(dgvWorkListData.Rows[testTempS.TestID - 1].Cells[6].Value.ToString(), "S")]
                                = Color.Yellow;
                            lisProBar[testTempS.TestID - 1].Invalidate();
                            #region 加样操作
                            int samplePos = int.Parse(testTempS.getSamplePos.Substring(1));//获取样本位置。
                                                                                           //获取当前反应盘可用位置
                            int pos = testTempS.AddSamplePos;


                            string cupType = testTempS.SampleContainer;//获取样本杯类型
                            if (testTempS.getSamplePos.Contains("R"))
                            {
                                while (lisMoveTube.Count != 0)//查询反应盘是否空闲，如不空闲，持续等待
                                {
                                    Thread.Sleep(100);
                                }
                                AddErrorCount = 0;
                                ///反应盘转到取样位置SamplePos，加样针加样到反应盘pos位置。
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 02 " + samplePos.ToString("x2") + " " + pos.ToString("x2")
                                    + " " + int.Parse(LiquidVol[j].Trim()).ToString("x2")), 0);
                                if (!NetCom3.Instance.SPQuery())
                                {
                                    #region 异常处理
                                    Again:
                                    string againSend = "";
                                    if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.IsKnocked && AddErrorCount < 2)
                                    {
                                        AddErrorCount++;
                                        //重新发送指令
                                        if (againSend == "")
                                            againSend = "EB 90 31 02 02 " + samplePos.ToString("x2") + " " + pos.ToString("x2")
                                  + " 00";
                                    }
                                    if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                                    {
                                        //重新发送指令
                                        if (againSend == "")
                                            againSend = "EB 90 31 02 02 " + samplePos.ToString("x2") + " " + pos.ToString("x2")
                                    + " " + int.Parse(LiquidVol[j].Trim()).ToString("x2");
                                    }
                                    else if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.OverTime)
                                    {
                                        NetCom3.Instance.stopsendFlag = true;
                                        ShowWarnInfo("加样针加样本时指令接收异常", "加样", 1);
                                        //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘夹管到温育盘时发生撞管！");
                                        //MessageBox.Show("指令接收超时，实验已终止", "加样错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        //addLiquiding = false;
                                        AllStop();
                                    }
                                    int sendFlag = SendAgain(againSend, 0);
                                    if (sendFlag == (int)ErrorState.Sendfailure)
                                        goto Again;
                                    else if (sendFlag == (int)ErrorState.IsKnocked)
                                        AddErrorCount++;
                                    #endregion
                                }
                                if (NetCom3.Instance.LiquidLevelDetectionFlag == (int)LiquidLevelDetectionAlarm.Low &&
                                   LiquidLevelDetectionEvent != null &&
                                   (!(dgvWorkListData.Rows[testTempS.TestID - 1].Cells["SampleType"].Value.ToString().Contains("标准品"))))
                                    LiquidLevelDetectionEvent("样本位置" + samplePos + "：样本出现凝块或样本量不足", 2);
                                //该位置信息写入配置文件
                                OperateIniFile.WriteIniData("ReactTrayInfo", "no" + pos.ToString(), "2", iniPathReactTrayInfo);
                                if (AddErrorCount > 0)
                                {
                                    if (AddErrorCount > 1)
                                    {
                                        NetCom3.Instance.stopsendFlag = true;
                                        ShowWarnInfo("加样针加样本时发生撞针", "加样", 1);
                                        AllStop();
                                    }
                                    else
                                    {
                                        MoveTubeListAddTubeDispose(pos);
                                        MoveTubeListAddTubeDispose(samplePos);
                                        RemoveTestList(testTempS , "加样针加样本时发生撞针");
                                    }
                                    break;
                                }
                            }
                            else
                            {
                                AddErrorCount = 0;
                                ///样本盘转到取样位置SamplePos，加样针加样到反应盘pos位置。
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 01 " + samplePos.ToString("x2") + " " + pos.ToString("x2")
                                    + " " + int.Parse(LiquidVol[j].Trim()).ToString("x2")), 0);
                                if (!NetCom3.Instance.SPQuery())
                                {
                                    #region 异常处理
                                    Again:
                                    string againSend = "";
                                    if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.IsKnocked && AddErrorCount < 2)
                                    {
                                        AddErrorCount++;
                                        //重新发送指令
                                        if (againSend == "")
                                            againSend = "EB 90 31 02 01 " + samplePos.ToString("x2") + " " + pos.ToString("x2")
                                  + " 00";
                                    }
                                    else if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                                    {
                                        //重新发送指令
                                        if (againSend == "")
                                            againSend = "EB 90 31 02 01 " + samplePos.ToString("x2") + " " + pos.ToString("x2")
                                    + " " + int.Parse(LiquidVol[j].Trim()).ToString("x2");
                                    }
                                    else if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.OverTime)
                                    {
                                        NetCom3.Instance.stopsendFlag = true;
                                        ShowWarnInfo("加样针加样本时指令接收异常", "加样", 1);
                                        //MessageBox.Show("指令接收超时，实验已终止", "加样错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        //addLiquiding = false;
                                        AllStop();
                                    }
                                    int sendFlag = SendAgain(againSend, 0);
                                    if (sendFlag == (int)ErrorState.Sendfailure)
                                        goto Again;
                                    else if (sendFlag == (int)ErrorState.IsKnocked)
                                        AddErrorCount++;
                                    #endregion
                                }
                                if (NetCom3.Instance.LiquidLevelDetectionFlag == (int)LiquidLevelDetectionAlarm.Low &&
                                   LiquidLevelDetectionEvent != null &&
                                   (!(dgvWorkListData.Rows[testTempS.TestID - 1].Cells["SampleType"].Value.ToString().Contains("标准品"))))
                                    LiquidLevelDetectionEvent("样本位置" + samplePos + "：样本出现凝块或样本量不足", 2);
                                //该位置信息写入配置文件
                                OperateIniFile.WriteIniData("ReactTrayInfo", "no" + pos.ToString(), "2", iniPathReactTrayInfo);
                                if (AddErrorCount > 0)
                                {
                                    if (AddErrorCount > 1)
                                    {
                                        NetCom3.Instance.stopsendFlag = true;
                                        ShowWarnInfo("加样针加样时发生撞针", "加样", 1);
                                        //MessageBox.Show("加样针撞针未能修复，实验已终止", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        //addLiquiding = false;
                                        AllStop();
                                    }
                                    else
                                    {
                                        MoveTubeListAddTubeDispose(pos);
                                        RemoveTestList(testTempS , "加样针加样本时发生撞针");
                                    }
                                    break;
                                }
                            }

                            #region 扔第二次稀释的管
                            if (testTempS.getSamplePos.Contains("R"))
                            {
                                ReactToAband(samplePos);
                            }
                            #endregion

                            #endregion
                            lisProBar[testTempS.TestID - 1].BarColor[StepIndex(dgvWorkListData.
                                Rows[testTempS.TestID - 1].Cells[6].Value.ToString(), "S")]
                                = Color.Gray;
                            lisProBar[testTempS.TestID - 1].Invalidate();
                            AddingSampleFlag = false;
                        }
                        else if (TestStepSingle[j] == "R1")
                        {
                            int index = TestStepSingle.ToList().IndexOf("R1");
                            while (!this.IsHandleCreated)//为了防止出现“在创建窗口句柄之前，不能在控件上调用 Invoke 或 BeginInvoke”的错误
                            {
                                Thread.Sleep(30);
                            }
                            BeginInvoke(TestStatusInfo, new object[] { "正在加试剂1", testTempS.TestID });
                            lisProBar[testTempS.TestID - 1].BarColor[StepIndex(dgvWorkListData.Rows[testTempS.TestID - 1]
                                .Cells[6].Value.ToString(), "R1")]
                                = Color.Yellow;
                            lisProBar[testTempS.TestID - 1].Invalidate();
                            #region 加R1操作
                            if (drRg.Length > 0)
                            {
                                for (int g = 0; g < drRg.Length; g++)
                                {
                                    //标准品、质控品以及其他只能使用它们自身的试剂 2018-08-27 添加
                                    if (dgvWorkListData.Rows[testTempS.TestID - 1].Cells["SampleType"].
                                        Value.ToString().Contains("标准品"))
                                    //|| dgvWorkListData.Rows[testTempS.TestID - 1].
                                    //Cells["SampleType"].Value.ToString().Contains("质控品"))
                                    {
                                        if (drRg[g]["Batch"].ToString() != dgvWorkListData.Rows[testTempS.TestID - 1]
                                            .Cells["RegentBatch"].Value.ToString() || int.Parse(drRg[g]["leftoverTestR1"].ToString()) <= 0)
                                            continue;
                                    }
                                    if (int.Parse(drRg[g]["leftoverTestR1"].ToString()) > 0)//判定试剂剩余量是否大于0
                                    {
                                        rgPos = int.Parse(drRg[g]["Postion"].ToString());//获取该试剂位置编号
                                        if (dgvWorkListData != null && dgvWorkListData.Rows.Count != 0)
                                        {
                                            this.BeginInvoke(new Action(() =>
                                            {
                                                dgvWorkListData.Rows[testTempS.TestID - 1].Cells["RegentBatch"].Value =
                                                    drRg[g]["Batch"].ToString();
                                                dgvWorkListData.Rows[testTempS.TestID - 1].Cells["RegentPos"].Value = rgPos;
                                            }));
                                        }
                                        rgindex = g;
                                        break;
                                    }
                                }
                            }
                            if (rgPos > 0)
                            {
                                //获取当前反应盘可用位置
                                int pos = testTempS.AddSamplePos;
                                //剩余R1体积
                                int leftR1 = int.Parse(OperateIniFile.ReadIniData("ReagentPos" + rgPos.ToString(),
                                    "LeftReagent1", "", iniPathReagentTrayInfo));
                                //string leftR1Vol = (leftR1 * int.Parse(LiquidVol[j].Trim()) + (int)(int.Parse(LiquidVol[j].Trim()) * abanR1Pro)).ToString("x4");
                                string leftR1Vol = (RegentNoUsePro + leftR1 * int.Parse(LiquidVol[j].Trim()) + 
                                    leftR1 * abanR1Pro).ToString("x4");//2018-10-13  zlx mod
                                AddErrorCount = 0;
                                ///取试剂盘rgPos位置试剂加到反应盘Pos位置///洗针
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 03 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                                    + " " + int.Parse(LiquidVol[j].Trim()).ToString("x2") + " " + 
                                    leftR1Vol.Substring(0, 2) + " " + leftR1Vol.Substring(2, 2)), 0);
                                if (!NetCom3.Instance.SPQuery())
                                {
                                    #region 异常处理
                                    Again:
                                    string againSend = "";
                                    if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.IsKnocked && AddErrorCount < 2)
                                    {
                                        AddErrorCount++;
                                        //重新发送指令
                                        if (againSend == "")
                                            againSend = againSend = "EB 90 31 02 03 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                                    + " 00 " + leftR1Vol.Substring(0, 2) + " " + leftR1Vol.Substring(2, 2);
                                    }
                                    else if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                                    {
                                        //重新发送指令
                                        if (againSend == "")
                                            againSend = "EB 90 31 02 03 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                                    + " " + int.Parse(LiquidVol[j].Trim()).ToString("x2") + " " + leftR1Vol.Substring(0, 2) + " " + leftR1Vol.Substring(2, 2);
                                    }
                                    else if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.OverTime)
                                    {
                                        NetCom3.Instance.stopsendFlag = true;
                                        ShowWarnInfo("加样针加试剂时发生指令接收异常", "加试剂", 1);
                                        //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘夹管到温育盘时发生撞管！");
                                        //MessageBox.Show("指令接收超时，实验已终止", "加样错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        //addLiquiding = false;
                                        AllStop();
                                    }
                                    int sendFlag = SendAgain(againSend, 0);
                                    if (sendFlag == (int)ErrorState.Sendfailure)
                                        goto Again;
                                    else if (sendFlag == (int)ErrorState.IsKnocked)
                                        AddErrorCount++;
                                    #endregion
                                }
                                //以下设置moveTube，即反应盘对应需加新管位置（testTempS.TestID+DValue+10）到放管位置
                                //drRg[rgindex]["leftoverTestR1"] = leftR1 - 1;
                                for (int i = 0; i < drRg.Length; i++)
                                {
                                    if (int.Parse(drRg[i]["Postion"].ToString()) == rgPos)
                                        drRg[i]["leftoverTestR1"] = leftR1 - 1;
                                }
                                OperateIniFile.WriteIniData("ReagentPos" + rgPos.ToString(), "LeftReagent1",
                                    (leftR1 - 1).ToString(), iniPathReagentTrayInfo);
                                if (AddErrorCount > 0)
                                {
                                    if (AddErrorCount > 1)
                                    {
                                        NetCom3.Instance.stopsendFlag = true;
                                        ShowWarnInfo("加样针加试剂时发生撞针", "加试剂", 1);
                                        //MessageBox.Show("加样针撞针未能修复，实验已终止", "加样错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        //addLiquiding = false;
                                        AllStop();
                                    }
                                    else
                                    {
                                        MoveTubeListAddTubeDispose(pos);
                                        RemoveTestList(testTempS , "加样针加试剂1时发生撞针");
                                    }
                                    break;
                                }
                                if (TestStepSingle[TestStepSingle.Length - 1] == "R1")
                                {
                                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 04 01 " + pos.ToString("x2")), 0);
                                    if (!NetCom3.Instance.SPQuery())
                                    {
                                        #region 异常处理
                                        string againSend = "";
                                        Again:
                                        if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                                        {
                                            //重新发送指令
                                            if (againSend == "")
                                                againSend = "EB 90 31 04 01 " + pos.ToString("x2");
                                            if (SendAgain(againSend, 0) == (int)ErrorState.Sendfailure)
                                                goto Again;
                                        }
                                        else
                                        {
                                            NetCom3.Instance.stopsendFlag = true;
                                            //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "混匀异常！");
                                            ShowWarnInfo("混匀异常", "加样混匀", 1);
                                            //MessageBox.Show("混匀异常！", "加样错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            //addLiquiding = false;
                                            AllStop();
                                            break;
                                        }
                                        #endregion
                                    }
                                }
                            }
                            #endregion
                            lisProBar[testTempS.TestID - 1].BarColor[StepIndex(dgvWorkListData.Rows[testTempS.TestID - 1].
                                Cells[6].Value.ToString(), "R1")]
                                = Color.Gray;
                            lisProBar[testTempS.TestID - 1].Invalidate();
                        }
                        else if (TestStepSingle[j] == "R2")
                        {
                            int index = TestStepSingle.ToList().IndexOf("R2");
                            while (!this.IsHandleCreated)//为了防止出现“在创建窗口句柄之前，不能在控件上调用 Invoke 或 BeginInvoke”的错误
                            {
                                Thread.Sleep(30);
                            }
                            BeginInvoke(TestStatusInfo, new object[] { "正在加试剂2", testTempS.TestID });
                            lisProBar[testTempS.TestID - 1].BarColor[StepIndex(dgvWorkListData.Rows[testTempS.TestID - 1].
                                Cells[6].Value.ToString(), "R2")]
                                = Color.Yellow;
                            lisProBar[testTempS.TestID - 1].Invalidate();
                            #region 加R2过程
                            if (dgvWorkListData.Rows[testTempS.TestID - 1].Cells["RegentPos"].Value.ToString() != "")
                                rgPos = int.Parse(dgvWorkListData.Rows[testTempS.TestID - 1].Cells["RegentPos"].Value.ToString());
                            else
                            {
                                if (drRg.Length > 0)
                                {
                                    for (int g = 0; g < drRg.Length; g++)
                                    {
                                        //标准品、质控品以及其他只能使用它们自身的试剂 2018-08-27 zlx add
                                        if (dgvWorkListData.Rows[testTempS.TestID - 1].Cells["SampleType"].Value.ToString().Contains("标准品") )
                                            //|| dgvWorkListData.Rows[testTempS.TestID - 1].Cells["SampleType"].Value.ToString().Contains("质控品"))
                                        {
                                            if (drRg[g]["Batch"].ToString() != dgvWorkListData.Rows[testTempS.TestID - 1].Cells["RegentBatch"].Value.ToString())
                                                continue;
                                        }
                                        if (int.Parse(drRg[g]["leftoverTestR2"].ToString()) > 0)//判定试剂剩余量是否大于0
                                        {
                                            rgPos = int.Parse(drRg[g]["Postion"].ToString());//获取该试剂位置编号
                                            rgindex = g;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (rgPos > 0)
                            {
                                //获取当前反应盘可用位置
                                int pos = (testTempS.AddSamplePos) % ReactTrayHoleNum == 0 ? ReactTrayHoleNum : (testTempS.AddSamplePos) % ReactTrayHoleNum;
                                //剩余R2体积
                                int leftR2 = int.Parse(OperateIniFile.ReadIniData("ReagentPos" + rgPos.ToString(), "LeftReagent2", "", iniPathReagentTrayInfo));
                                string leftR2Vol = (RegentNoUsePro + leftR2 * int.Parse(LiquidVol[j].Trim()) + leftR2 * abanR2Pro).ToString("x4");//2018-10-13
                                                                                                                                                  //string leftR2Vol = (leftR2 * int.Parse(LiquidVol[j].Trim()) + (int)(int.Parse(LiquidVol[j].Trim()) * abanR2Pro)).ToString("x4");
                                AddErrorCount = 0;
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 04 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                                        + " " + int.Parse(LiquidVol[j].Trim()).ToString("x2") + " " + leftR2Vol.Substring(0, 2) + " " + leftR2Vol.Substring(2, 2)), 0);
                                if (!NetCom3.Instance.SPQuery())
                                {
                                    #region 异常处理
                                    Again:
                                    string againSend = "";
                                    if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.IsKnocked && AddErrorCount < 2)
                                    {
                                        AddErrorCount++;
                                        //重新发送指令
                                        if (againSend == "")
                                            againSend = "EB 90 31 02 04 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                                       + " 00 " + leftR2Vol.Substring(0, 2) + " " + leftR2Vol.Substring(2, 2);
                                    }
                                    else if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                                    {
                                        //重新发送指令
                                        if (againSend == "")
                                            againSend = "EB 90 31 02 04 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                                        + " " + int.Parse(LiquidVol[j].Trim()).ToString("x2") + " " + leftR2Vol.Substring(0, 2) + " " + leftR2Vol.Substring(2, 2);
                                    }
                                    else if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.OverTime)
                                    {
                                        NetCom3.Instance.stopsendFlag = true;
                                        ShowWarnInfo("加样针加试剂时发生指令接收异常", "加样", 1);
                                        //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘夹管到温育盘时发生撞管！");
                                        //MessageBox.Show("指令接收超时，实验已终止", "加样错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        //addLiquiding = false;
                                        AllStop();
                                    }
                                    int sendFlag = SendAgain(againSend, 0);
                                    if (sendFlag == (int)ErrorState.Sendfailure)
                                        goto Again;
                                    else if (sendFlag == (int)ErrorState.IsKnocked)
                                        AddErrorCount++;
                                    #endregion
                                }
                                //修改试剂配制信息
                                //drRg[rgindex]["leftoverTestR2"] = leftR2 - 1;
                                for (int i = 0; i < drRg.Length; i++)
                                {
                                    if (int.Parse(drRg[i]["Postion"].ToString()) == rgPos)
                                        drRg[i]["leftoverTestR2"] = leftR2 - 1;
                                }
                                OperateIniFile.WriteIniData("ReagentPos" + rgPos.ToString(), "LeftReagent2", (leftR2 - 1).ToString(), iniPathReagentTrayInfo);
                                if (AddErrorCount > 0)
                                {
                                    if (AddErrorCount > 1)
                                    {
                                        NetCom3.Instance.stopsendFlag = true;
                                        ShowWarnInfo("加样针加试剂时发生撞针", "加样", 1);
                                        //MessageBox.Show("加样针撞针未能修复，实验已终止", "加样错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        //addLiquiding = false;
                                        AllStop();
                                    }
                                    else
                                    {
                                        MoveTubeListAddTubeDispose(pos);
                                        RemoveTestList(testTempS , "加样针加试剂2时发生撞针");
                                    }
                                    break;
                                }
                                if (TestStepSingle[TestStepSingle.Length - 1] == "R2")
                                {

                                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 04 01 " + pos.ToString("x2")), 0);//add y 20180524混匀
                                    if (!NetCom3.Instance.SPQuery())//add y 20180524混匀
                                    {
                                        #region 异常处理
                                        string againSend = "";
                                        Again:
                                        if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                                        {
                                            //重新发送指令
                                            if (againSend == "")
                                                againSend = "EB 90 31 04 01 " + pos.ToString("x2");
                                            if (SendAgain(againSend, 0) == (int)ErrorState.Sendfailure)
                                                goto Again;
                                        }
                                        else
                                        {
                                            NetCom3.Instance.stopsendFlag = true;
                                            //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "混匀" + pos + "位置指令接收异常！");
                                            ShowWarnInfo("混匀" + pos + "位置指令接收异常", "加样", 1);
                                            //MessageBox.Show("混匀异常！", "加样错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            //addLiquiding = false;
                                            AllStop();
                                            break;
                                        }
                                        #endregion
                                    }

                                }

                            }
                            #endregion
                            lisProBar[testTempS.TestID - 1].BarColor[StepIndex(dgvWorkListData.Rows[testTempS.TestID - 1].
                                Cells[6].Value.ToString(), "R2")]
                                = Color.Gray;
                            lisProBar[testTempS.TestID - 1].Invalidate();
                        }
                        else if (TestStepSingle[j] == "R3")
                        {
                            int index = TestStepSingle.ToList().IndexOf("R3");
                            while (!this.IsHandleCreated)//为了防止出现“在创建窗口句柄之前，不能在控件上调用 Invoke 或 BeginInvoke”的错误
                            {
                                Thread.Sleep(30);
                            }
                            BeginInvoke(TestStatusInfo, new object[] { "正在加试剂3", testTempS.TestID });
                            lisProBar[testTempS.TestID - 1].BarColor[StepIndex(dgvWorkListData.Rows[testTempS.TestID - 1].
                                Cells[6].Value.ToString(), "R3")]
                                = Color.Yellow;
                            lisProBar[testTempS.TestID - 1].Invalidate();
                            #region 加R3过程
                            if (dgvWorkListData.Rows[testTempS.TestID - 1].Cells["RegentPos"].Value.ToString() != "")
                                rgPos = int.Parse(dgvWorkListData.Rows[testTempS.TestID - 1].Cells["RegentPos"].Value.ToString());
                            else
                            {
                                if (drRg.Length > 0)
                                {
                                    for (int g = 0; g < drRg.Length; g++)
                                    {
                                        //2018-08-27 zlx add
                                        if (dgvWorkListData.Rows[testTempS.TestID - 1].Cells["SampleType"].Value.ToString().Contains("标准品")) 
                                            //|| dgvWorkListData.Rows[testTempS.TestID - 1].Cells["SampleType"].Value.ToString().Contains("质控品"))
                                        {
                                            if (drRg[g]["Batch"].ToString() != dgvWorkListData.Rows[testTempS.TestID - 1].Cells["RegentBatch"].Value.ToString())
                                                continue;
                                        }
                                        if (int.Parse(drRg[g]["leftoverTestR3"].ToString()) > 0)//判定试剂剩余量是否大于0
                                        {
                                            rgPos = int.Parse(drRg[g]["Postion"].ToString());//获取该试剂位置编号
                                            rgindex = g;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (rgPos > 0)
                            {
                                //获取当前反应盘可用位置
                                int pos = (testTempS.AddSamplePos) % ReactTrayHoleNum == 0 ? ReactTrayHoleNum : (testTempS.AddSamplePos) % ReactTrayHoleNum;
                                //剩余R3体积
                                int leftR3 = int.Parse(OperateIniFile.ReadIniData("ReagentPos" + rgPos.ToString(), "LeftReagent3", "", iniPathReagentTrayInfo));
                                string leftR3Vol = (RegentNoUsePro + leftR3 * int.Parse(LiquidVol[j].Trim()) + leftR3 * abanR3Pro).ToString("x4");//2018-10-13 zlx mod
                                                                                                                                                  //string leftR3Vol = (leftR3 * int.Parse(LiquidVol[j].Trim()) + (int)(int.Parse(LiquidVol[j].Trim()) * abanR3Pro)).ToString("x4");
                                AddErrorCount = 0;
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 05 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                                        + " " + int.Parse(LiquidVol[j].Trim()).ToString("x2") + " " + leftR3Vol.Substring(0, 2) + " " + leftR3Vol.Substring(2, 2)), 0);
                                if (!NetCom3.Instance.SPQuery())
                                {
                                    #region 异常处理
                                    Again:
                                    string againSend = "";
                                    if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.IsKnocked && AddErrorCount < 2)
                                    {
                                        AddErrorCount++;
                                        //重新发送指令
                                        if (againSend == "")
                                            againSend = "EB 90 31 02 05 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                                        + " 00 " + leftR3Vol.Substring(0, 2) + " " + leftR3Vol.Substring(2, 2);
                                    }
                                    else if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                                    {
                                        //重新发送指令
                                        if (againSend == "")
                                            againSend = "EB 90 31 02 05 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                                        + " " + int.Parse(LiquidVol[j].Trim()).ToString("x2") + " " + leftR3Vol.Substring(0, 2) + " " + leftR3Vol.Substring(2, 2);
                                    }
                                    else if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.OverTime)
                                    {
                                        NetCom3.Instance.stopsendFlag = true;
                                        ShowWarnInfo("加样针加试剂时指令接收超时", "加样", 1);
                                        //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘夹管到温育盘时发生撞管！");
                                        //MessageBox.Show("指令接收超时，实验已终止", "加样错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        //addLiquiding = false;
                                        AllStop();
                                    }
                                    int sendFlag = SendAgain(againSend, 0);
                                    if (sendFlag == (int)ErrorState.Sendfailure)
                                        goto Again;
                                    else if (sendFlag == (int)ErrorState.IsKnocked)
                                        AddErrorCount++;
                                    #endregion
                                }
                                //drRg[rgindex]["leftoverTestR3"] = leftR3 - 1;
                                for (int i = 0; i < drRg.Length; i++)
                                {
                                    if (int.Parse(drRg[i]["Postion"].ToString()) == rgPos)
                                        drRg[i]["leftoverTestR3"] = leftR3 - 1;
                                }
                                OperateIniFile.WriteIniData("ReagentPos" + rgPos.ToString(), "LeftReagent3", (leftR3 - 1).ToString(), iniPathReagentTrayInfo);
                                if (AddErrorCount > 0)
                                {
                                    if (AddErrorCount > 1)
                                    {
                                        NetCom3.Instance.stopsendFlag = true;
                                        //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "加样针加试剂时发生撞针！");
                                        ShowWarnInfo("加样针加试剂时发生撞针", "加样", 1);
                                        //MessageBox.Show("加样针撞针未能修复，实验已终止", "加样错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        //addLiquiding = false;
                                        AllStop();
                                    }
                                    else
                                    {
                                        MoveTubeListAddTubeDispose(pos);
                                        RemoveTestList(testTempS , "加样针加试剂3时发生撞针");
                                    }
                                    break;
                                }
                                if (TestStepSingle[TestStepSingle.Length - 1] == "R3")
                                {
                                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 04 01 " + pos.ToString("x2")), 0);
                                    if (!NetCom3.Instance.SPQuery())
                                    {
                                        #region 异常处理
                                        string againSend = "";
                                        Again:
                                        if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                                        {
                                            //重新发送指令
                                            if (againSend == "")
                                                againSend = "EB 90 31 04 01 " + pos.ToString("x2");
                                            if (SendAgain(againSend, 0) == (int)ErrorState.Sendfailure)
                                                goto Again;
                                        }
                                        else
                                        {
                                            NetCom3.Instance.stopsendFlag = true;
                                            //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "混匀" + pos + "位置指令接收超时！");
                                            ShowWarnInfo("混匀" + pos + "位置指令接收超时", "加样", 1);
                                            //MessageBox.Show("混匀异常！", "加样错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            AllStop();
                                            break;
                                        }
                                        #endregion
                                    }
                                }

                            }
                            #endregion
                            lisProBar[testTempS.TestID - 1].BarColor[StepIndex(dgvWorkListData.Rows[testTempS.TestID - 1].
                                Cells[6].Value.ToString(), "R3")]
                                = Color.Gray;
                            lisProBar[testTempS.TestID - 1].Invalidate();
                        }
                    }
                    #region 夹新管
                    MoveTubeStatus moveTube6 = new MoveTubeStatus();
                    int pos1 = testTempS.AddSamplePos + toUsedTube;
                    if (pos1 > ReactTrayHoleNum)
                    {
                        pos1 = pos1 - ReactTrayHoleNum + 3;
                    }
                    string ss = OperateIniFile.ReadIniData("ReactTrayInfo", "no" + pos1, "", iniPathReactTrayInfo);
                    if ((ss == "2"|| ss == "9") && !AddTubeStop.Contains(pos1))
                        AddTubeStop.Add(pos1);
                    else
                    {
                        moveTube6.StepNum = testTempS.stepNum;
                        moveTube6.putTubePos = "1-" + pos1.ToString();
                        moveTube6.TestId = testTempS.TestID;
                        //管架夹新管未夹位置统计
                        int tubeNum1 = 0;
                        for (int i = 0; i < lisMoveTube.Count; i++)
                        {
                            if (lisMoveTube[i].TakeTubePos.Split('-')[0] == "0")
                            {
                                tubeNum1++;
                            }
                        }
                        moveTube6.TakeTubePos = "0-" + (int.Parse(OperateIniFile.ReadIniData("Tube", "TubePos", "1", iniPathSubstrateTube))
                            + tubeNum1).ToString();
                        lock (locker2)
                        {
                            lisMoveTube.Add(moveTube6);
                        }
                    }
                    #endregion
                    outAddLiquidTube:
                    DalayFlag = false;
                    stepTime = 0;
                    break;
                case TestSchedule.ExperimentScheduleStep.AddBeads:
                   try
                    {
                        lisProBar[testTempS.TestID - 1].BarColor[StepIndex(dgvWorkListData.Rows[testTempS.TestID - 1].Cells[6].Value.ToString(), "B") - 1]
                        = Color.Gray;
                    }
                    catch (Exception e)
                    {
                        //NetCom3.Instance.writeLog(e);
                        LogFile.Instance.Write(e.ToString());
                    }
                    lisProBar[testTempS.TestID - 1].Invalidate();
                    while (!this.IsHandleCreated)//为了防止出现“在创建窗口句柄之前，不能在控件上调用 Invoke 或 BeginInvoke”的错误
                    {
                        Thread.Sleep(30);
                    }
                    BeginInvoke(TestStatusInfo, new object[] { "正在加磁珠", testTempS.TestID });
                    lisProBar[testTempS.TestID - 1].BarColor[StepIndex(dgvWorkListData.Rows[testTempS.TestID - 1].Cells[6].Value.ToString(), "B")]
                        = Color.Yellow;
                    lisProBar[testTempS.TestID - 1].Invalidate();
                    #region 加磁珠过程
                    if (dgvWorkListData.Rows[testTempS.TestID - 1].Cells["RegentPos"].Value.ToString() != "")
                        rgPos = int.Parse(dgvWorkListData.Rows[testTempS.TestID - 1].Cells["RegentPos"].Value.ToString());
                    else
                    {
                        if (drRg.Length > 0)
                        {
                            for (int g = 0; g < drRg.Length; g++)
                            {
                                //2018-08-27 zlx add
                                if (dgvWorkListData.Rows[testTempS.TestID - 1].Cells["SampleType"].Value.ToString().Contains("标准品") 
                                    /*|| dgvWorkListData.Rows[testTempS.TestID - 1].Cells["SampleType"].Value.ToString().Contains("质控品")*/)
                                {
                                    if (drRg[g]["Batch"].ToString() != dgvWorkListData.Rows[testTempS.TestID - 1].Cells["RegentBatch"].Value.ToString())
                                        continue;
                                }
                                if (int.Parse(drRg[g]["leftoverTestR4"].ToString()) > 0)//判定试剂剩余量是否大于0
                                {
                                    rgPos = int.Parse(drRg[g]["Postion"].ToString());//获取该试剂位置编号
                                    rgindex = g;
                                    break;
                                }
                            }
                        }
                    }
                    if (rgPos > 0)
                    {
                        //获取当前反应盘可用位置
                        int pos = (testTempS.AddSamplePos) % ReactTrayHoleNum == 0 ? ReactTrayHoleNum : (testTempS.AddSamplePos) % ReactTrayHoleNum;
                        //剩余R4体积
                        int leftR4 = int.Parse(OperateIniFile.ReadIniData("ReagentPos" + rgPos.ToString(), "LeftReagent4", "", iniPathReagentTrayInfo));
                        string leftR4Vol = (RegentNoUsePro + leftR4 * int.Parse(testTempS.AddLiqud) + leftR4 * abanR1Pro).ToString("x4");//2018-10-13 zlx mod
                                                                                                                                         //string leftR4Vol = (leftR4 * int.Parse(testTempS.AddLiqud) + (int)(int.Parse(testTempS.AddLiqud) * abanR1Pro)).ToString("x4");
                                                                                                                                         ///取试剂盘rgPos位置试剂加到反应盘Pos位置///洗针
                        AddErrorCount = 0;
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 07 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                            + " " + int.Parse(testTempS.AddLiqud).ToString("x2") + " " + leftR4Vol.Substring(0, 2) + " " + leftR4Vol.Substring(2, 2)), 0);
                        if (!NetCom3.Instance.SPQuery())
                        {
                            #region 异常处理
                            Again:
                            string againSend = "";
                            if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.IsKnocked && AddErrorCount < 2)
                            {
                                AddErrorCount++;
                                //重新发送指令
                                if (againSend == "")
                                    againSend = "EB 90 31 02 07 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                            + " 00 " + leftR4Vol.Substring(0, 2) + " " + leftR4Vol.Substring(2, 2);
                            }
                            else if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                            {
                                //重新发送指令
                                if (againSend == "")
                                    againSend = "EB 90 31 02 07 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                            + " " + int.Parse(testTempS.AddLiqud).ToString("x2") + " " + leftR4Vol.Substring(0, 2) + " " + leftR4Vol.Substring(2, 2);
                            }
                            else if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.OverTime)
                            {
                                NetCom3.Instance.stopsendFlag = true;
                                ShowWarnInfo("加样针加磁珠时指令接收超时，实验已终止", "加样", 1);
                                //MessageBox.Show("指令接收超时，实验已终止", "加样错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                //addLiquiding = false;
                                AllStop();
                            }
                            int sendFlag = SendAgain(againSend, 0);
                            if (sendFlag == (int)ErrorState.Sendfailure)
                                goto Again;
                            else if (sendFlag == (int)ErrorState.IsKnocked)
                                AddErrorCount++;
                            #endregion
                        }
                        for (int i = 0; i < drRg.Length; i++)
                        {
                            if (int.Parse(drRg[i]["Postion"].ToString()) == rgPos)
                                drRg[i]["leftoverTestR4"] = leftR4 - 1;
                        }
                        OperateIniFile.WriteIniData("ReagentPos" + rgPos.ToString(), "LeftReagent4", (leftR4 - 1).ToString(), iniPathReagentTrayInfo);
                        if (AddErrorCount > 0)
                        {
                            if (AddErrorCount > 1)
                            {
                                NetCom3.Instance.stopsendFlag = true;
                                ShowWarnInfo("加样针加磁珠撞针，未能修复，实验已终止", "加样", 1);
                                AllStop();
                            }
                            else
                            {
                                MoveTubeListAddTubeDispose(pos);
                                RemoveTestList(testTempS , "加样针加磁珠时发生撞针");
                            }
                            break;
                        }

                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 04 01 " + pos.ToString("x2")), 0);//add y 20180524混匀
                        if (!NetCom3.Instance.SPQuery())//add y 20180524混匀
                        {
                            #region 异常处理
                            string againSend = "";
                            Again:
                            if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                            {
                                //重新发送指令
                                if (againSend == "")
                                    againSend = "EB 90 31 04 01 " + pos.ToString("x2");
                                if (SendAgain(againSend, 0) == (int)ErrorState.Sendfailure)
                                    goto Again;
                            }
                            else
                            {
                                NetCom3.Instance.stopsendFlag = true;
                                ShowWarnInfo("混匀" + pos + "位置时指令接收超时", "加样", 1);
                                //MessageBox.Show("混匀异常！", "加样错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                //addLiquiding = false;
                                AllStop();
                                break;
                            }
                            #endregion
                        }
                        //drRg = dtRgInfo.Select("RgName='" + testTempS.ItemName.ToString() + "'");

                    }
                    #endregion
                    lisProBar[testTempS.TestID - 1].BarColor[StepIndex(dgvWorkListData.Rows[testTempS.TestID - 1].Cells[6].Value.ToString(), "B")]
                        = Color.Gray;
                    lisProBar[testTempS.TestID - 1].Invalidate();
                    DalayFlag = false;
                    stepTime = 0;
                    break;
                case TestSchedule.ExperimentScheduleStep.AddSingleR:
                    if (testTempS.singleStep == "R1")
                    {
                        lisProBar[testTempS.TestID - 1].BarColor[StepIndex(dgvWorkListData.Rows[testTempS.TestID - 1].Cells[6].Value.ToString(), "R1") - 1]
                        = Color.Gray;
                        lisProBar[testTempS.TestID - 1].Invalidate();
                        while (!this.IsHandleCreated)//为了防止出现“在创建窗口句柄之前，不能在控件上调用 Invoke 或 BeginInvoke”的错误
                        {
                            Thread.Sleep(30);
                        }
                        BeginInvoke(TestStatusInfo, new object[] { "正在加试剂1", testTempS.TestID });
                        lisProBar[testTempS.TestID - 1].BarColor[StepIndex(dgvWorkListData.Rows[testTempS.TestID - 1].Cells[6].Value.ToString(), "R1")]
                            = Color.Yellow;
                        lisProBar[testTempS.TestID - 1].Invalidate();
                        #region 加R过程
                        if (dgvWorkListData.Rows[testTempS.TestID - 1].Cells["RegentPos"].Value.ToString() != "")
                            rgPos = int.Parse(dgvWorkListData.Rows[testTempS.TestID - 1].Cells["RegentPos"].Value.ToString());
                        else
                        {
                            if (drRg.Length > 0)
                            {
                                for (int g = 0; g < drRg.Length; g++)
                                {
                                    //标准品、质控品以及其他只能使用它们自身的试剂 2018-08-27 zlx add
                                    if (dgvWorkListData.Rows[testTempS.TestID - 1].Cells["SampleType"].Value.ToString().Contains("标准品") 
                                        /*|| dgvWorkListData.Rows[testTempS.TestID - 1].Cells["SampleType"].Value.ToString().Contains("质控品")*/)
                                    {
                                        if (drRg[g]["Batch"].ToString() != dgvWorkListData.Rows[testTempS.TestID - 1].Cells["RegentBatch"].Value.ToString())
                                            continue;
                                    }
                                    if (int.Parse(drRg[g]["leftoverTestR1"].ToString()) > 0)//判定试剂剩余量是否大于0
                                    {
                                        rgPos = int.Parse(drRg[g]["Postion"].ToString());//获取该试剂位置编号
                                        dgvWorkListData.Rows[testTempS.TestID - 1].Cells["RegentPos"].Value = rgPos;
                                        rgindex = g;
                                        break;
                                    }
                                }
                            }
                        }
                        if (rgPos > 0)
                        {
                            //获取当前反应盘可用位置
                            int pos = (testTempS.AddSamplePos) % ReactTrayHoleNum == 0 ? ReactTrayHoleNum : (testTempS.AddSamplePos) % ReactTrayHoleNum;
                            //剩余R1体积
                            int leftR1 = int.Parse(OperateIniFile.ReadIniData("ReagentPos" + rgPos.ToString(), "LeftReagent1", "", iniPathReagentTrayInfo));
                            string leftR1Vol = (RegentNoUsePro + leftR1 * int.Parse(testTempS.AddLiqud) + leftR1 * abanR1Pro).ToString("x4");//2018-10-13 zlx mod
                                                                                                                                             //string leftR1Vol = (leftR1 * int.Parse(testTempS.AddLiqud) + (int)(int.Parse(testTempS.AddLiqud) * abanR1Pro)).ToString("x4");
                                                                                                                                             ///取试剂盘rgPos位置试剂加到反应盘Pos位置///洗针
                            AddErrorCount = 0;
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 03 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                                + " " + int.Parse(testTempS.AddLiqud).ToString("x2") + " " + leftR1Vol.Substring(0, 2) + " " + leftR1Vol.Substring(2, 2)), 0);
                            if (!NetCom3.Instance.SPQuery())
                            {
                                #region 异常处理
                                Again:
                                string againSend = "";
                                if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.IsKnocked && AddErrorCount < 2)
                                {
                                    AddErrorCount++;
                                    //重新发送指令
                                    if (againSend == "")
                                        againSend = "EB 90 31 02 03 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                              + " 00 " + leftR1Vol.Substring(0, 2) + " " + leftR1Vol.Substring(2, 2);
                                }
                                else if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                                {
                                    //重新发送指令
                                    if (againSend == "")
                                        againSend = "EB 90 31 02 03 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                                + " " + int.Parse(testTempS.AddLiqud).ToString("x2") + " " + leftR1Vol.Substring(0, 2) + " " + leftR1Vol.Substring(2, 2);
                                }
                                else if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.OverTime)
                                {
                                    NetCom3.Instance.stopsendFlag = true;
                                    ShowWarnInfo("加样针加试剂时指令接收超时", "加样", 1);
                                    //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘夹管到温育盘时发生撞管！");
                                    //MessageBox.Show("指令接收超时，实验已终止", "加样错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    //addLiquiding = false;
                                    AllStop();
                                }
                                int sendFlag = SendAgain(againSend, 0);
                                if (sendFlag == (int)ErrorState.Sendfailure)
                                    goto Again;
                                else if (sendFlag == (int)ErrorState.IsKnocked)
                                    AddErrorCount++;
                                #endregion
                            }
                            for (int i = 0; i < drRg.Length; i++)
                            {
                                if (int.Parse(drRg[i]["Postion"].ToString()) == rgPos)
                                    drRg[i]["leftoverTestR1"] = leftR1 - 1;
                            }
                            OperateIniFile.WriteIniData("ReagentPos" + rgPos.ToString(), "LeftReagent1", (leftR1 - 1).ToString(), iniPathReagentTrayInfo);
                            if (AddErrorCount > 0)
                            {
                                if (AddErrorCount > 1)
                                {
                                    NetCom3.Instance.stopsendFlag = true;
                                    ShowWarnInfo("加样针加试剂时撞针未能修复！", "加样", 1);
                                    AllStop();
                                }
                                else
                                {
                                    RemoveTestList(testTempS , "加样针加试剂1时发生撞针");
                                    MoveTubeListAddTubeDispose(pos);
                                }
                                break;
                            }

                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 04 01 " + pos.ToString("x2")), 0);
                            if (!NetCom3.Instance.SPQuery())
                            {
                                #region 异常处理
                                string againSend = "";
                                Again:
                                if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                                {
                                    //重新发送指令
                                    if (againSend == "")
                                        againSend = "EB 90 31 04 01 " + pos.ToString("x2");
                                    if (SendAgain(againSend, 0) == (int)ErrorState.Sendfailure)
                                        goto Again;
                                }
                                else
                                {
                                    NetCom3.Instance.stopsendFlag = true;
                                    ShowWarnInfo("混匀" + pos + "位置时指令接收异常", "加样", 1);
                                    //MessageBox.Show("混匀异常！", "加样错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    //addLiquiding = false;
                                    AllStop();
                                    break;
                                }
                                #endregion
                            }


                        }
                        #endregion
                        lisProBar[testTempS.TestID - 1].BarColor[StepIndex(dgvWorkListData.Rows[testTempS.TestID - 1].Cells[6].Value.ToString(), "R1")]
                            = Color.Gray;
                    }
                    else if (testTempS.singleStep == "R2")
                    {
                        lisProBar[testTempS.TestID - 1].BarColor[StepIndex(dgvWorkListData.Rows[testTempS.TestID - 1].Cells[6].Value.ToString(), "R2") - 1]
                        = Color.Gray;
                        lisProBar[testTempS.TestID - 1].Invalidate();
                        while (!this.IsHandleCreated)//为了防止出现“在创建窗口句柄之前，不能在控件上调用 Invoke 或 BeginInvoke”的错误
                        {
                            Thread.Sleep(30);
                        }
                        BeginInvoke(TestStatusInfo, new object[] { "正在加试剂2", testTempS.TestID });
                        lisProBar[testTempS.TestID - 1].BarColor[StepIndex(dgvWorkListData.Rows[testTempS.TestID - 1].Cells[6].Value.ToString(), "R2")]
                            = Color.Yellow;
                        lisProBar[testTempS.TestID - 1].Invalidate();
                        #region 加R过程
                        if (dgvWorkListData.Rows[testTempS.TestID - 1].Cells["RegentPos"].Value.ToString() != "")
                            rgPos = int.Parse(dgvWorkListData.Rows[testTempS.TestID - 1].Cells["RegentPos"].Value.ToString());
                        else
                        {
                            if (drRg.Length > 0)
                            {
                                for (int g = 0; g < drRg.Length; g++)
                                {
                                    //标准品、质控品以及其他只能使用它们自身的试剂 2018-08-27 zlx add
                                    if (dgvWorkListData.Rows[testTempS.TestID - 1].Cells["SampleType"].Value.ToString().Contains("标准品")
                                        /*|| dgvWorkListData.Rows[testTempS.TestID - 1].Cells["SampleType"].Value.ToString().Contains("质控品")*/)
                                    {
                                        if (drRg[g]["Batch"].ToString() != dgvWorkListData.Rows[testTempS.TestID - 1].Cells["RegentBatch"].Value.ToString())
                                            continue;
                                    }
                                    if (int.Parse(drRg[g]["leftoverTestR2"].ToString()) > 0)//判定试剂剩余量是否大于0
                                    {
                                        rgPos = int.Parse(drRg[g]["Postion"].ToString());//获取该试剂位置编号
                                        rgindex = g;
                                        break;
                                    }
                                }
                            }
                        }
                        if (rgPos > 0)
                        {
                            //获取当前反应盘可用位置
                            int pos = (testTempS.AddSamplePos) % ReactTrayHoleNum == 0 ? ReactTrayHoleNum : (testTempS.AddSamplePos) % ReactTrayHoleNum;
                            //剩余R2体积
                            int leftR2 = int.Parse(OperateIniFile.ReadIniData("ReagentPos" + rgPos.ToString(), "LeftReagent2", "", iniPathReagentTrayInfo));
                            string leftR2Vol = (RegentNoUsePro + leftR2 * int.Parse(testTempS.AddLiqud) + leftR2 * abanR2Pro).ToString("x4");//2018-10-13 zlx mod
                            AddErrorCount = 0;
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 04 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                                + " " + int.Parse(testTempS.AddLiqud).ToString("x2") + " " + leftR2Vol.Substring(0, 2) + " " + leftR2Vol.Substring(2, 2)), 0);
                            if (!NetCom3.Instance.SPQuery())
                            {
                                #region 异常处理
                                Again:
                                string againSend = "";
                                if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.IsKnocked && AddErrorCount < 2)
                                {
                                    AddErrorCount++;
                                    //重新发送指令
                                    if (againSend == "")
                                        againSend = "EB 90 31 02 04 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                                + " 00 " + leftR2Vol.Substring(0, 2) + " " + leftR2Vol.Substring(2, 2);
                                }
                                else if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                                {
                                    //重新发送指令
                                    if (againSend == "")
                                        againSend = "EB 90 31 02 04 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                                + " " + int.Parse(testTempS.AddLiqud).ToString("x2") + " " + leftR2Vol.Substring(0, 2) + " " + leftR2Vol.Substring(2, 2);
                                }
                                else if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.OverTime)
                                {
                                    NetCom3.Instance.stopsendFlag = true;
                                    ShowWarnInfo("加样针加试剂时指令接收超时", "加样", 1);
                                    //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘夹管到温育盘时发生撞管！");
                                    //MessageBox.Show("指令接收超时，实验已终止", "加样错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    //addLiquiding = false;
                                    AllStop();
                                }
                                int sendFlag = SendAgain(againSend, 0);
                                if (sendFlag == (int)ErrorState.Sendfailure)
                                    goto Again;
                                else if (sendFlag == (int)ErrorState.IsKnocked)
                                    AddErrorCount++;
                                #endregion
                            }
                            for (int i = 0; i < drRg.Length; i++)
                            {
                                if (int.Parse(drRg[i]["Postion"].ToString()) == rgPos)
                                    drRg[i]["leftoverTestR2"] = leftR2 - 1;
                            }
                            OperateIniFile.WriteIniData("ReagentPos" + rgPos.ToString(), "LeftReagent2", (leftR2 - 1).ToString(), iniPathReagentTrayInfo);
                            if (AddErrorCount > 0)
                            {
                                if (AddErrorCount > 1)
                                {
                                    NetCom3.Instance.stopsendFlag = true;
                                    ShowWarnInfo("加样针加试剂时撞针未能修复", "加样", 1);
                                    AllStop();
                                }
                                else
                                {
                                    MoveTubeListAddTubeDispose(pos);
                                    RemoveTestList(testTempS , "加样针加试剂2时发生撞针");
                                }
                                break;
                            }

                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 04 01 " + pos.ToString("x2")), 0);
                            if (!NetCom3.Instance.SPQuery())
                            {
                                #region 异常处理
                                string againSend = "";
                                Again:
                                if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                                {
                                    //重新发送指令
                                    if (againSend == "")
                                        againSend = "EB 90 31 04 01 " + pos.ToString("x2");
                                    if (SendAgain(againSend, 0) == (int)ErrorState.Sendfailure)
                                        goto Again;
                                }
                                else
                                {
                                    NetCom3.Instance.stopsendFlag = true;
                                    ShowWarnInfo("混匀" + pos + "位置时指令接收异常！", "加样", 1);
                                    //MessageBox.Show("混匀异常！", "加样错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    AllStop();
                                    break;
                                }
                                #endregion
                            }

                        }
                        #endregion
                        lisProBar[testTempS.TestID - 1].BarColor[StepIndex(dgvWorkListData.Rows[testTempS.TestID - 1].Cells[6].Value.ToString(), "R2")]
                            = Color.Gray;
                    }
                    else if (testTempS.singleStep == "R3")
                    {
                        lisProBar[testTempS.TestID - 1].BarColor[StepIndex(dgvWorkListData.Rows[testTempS.TestID - 1].Cells[6].Value.ToString(), "R3") - 1]
                        = Color.Gray;
                        lisProBar[testTempS.TestID - 1].Invalidate();
                        while (!this.IsHandleCreated)//为了防止出现“在创建窗口句柄之前，不能在控件上调用 Invoke 或 BeginInvoke”的错误
                        {
                            Thread.Sleep(30);
                        }
                        BeginInvoke(TestStatusInfo, new object[] { "正在加试剂3", testTempS.TestID });
                        lisProBar[testTempS.TestID - 1].BarColor[StepIndex(dgvWorkListData.Rows[testTempS.TestID - 1].Cells[6].Value.ToString(), "R3")]
                            = Color.Yellow;
                        lisProBar[testTempS.TestID - 1].Invalidate();
                        #region 加R过程
                        if (dgvWorkListData.Rows[testTempS.TestID - 1].Cells["RegentPos"].Value.ToString() != "")
                            rgPos = int.Parse(dgvWorkListData.Rows[testTempS.TestID - 1].Cells["RegentPos"].Value.ToString());
                        else
                        {
                            if (drRg.Length > 0)
                            {
                                for (int g = 0; g < drRg.Length; g++)
                                {
                                    //标准品、质控品以及其他只能使用它们自身的试剂 2018-08-27 zlx add
                                    if (dgvWorkListData.Rows[testTempS.TestID - 1].Cells["SampleType"].Value.ToString().Contains("标准品")
                                        /*|| dgvWorkListData.Rows[testTempS.TestID - 1].Cells["SampleType"].Value.ToString().Contains("质控品")*/)
                                    {
                                        if (drRg[g]["Batch"].ToString() != dgvWorkListData.Rows[testTempS.TestID - 1].Cells["RegentBatch"].Value.ToString())
                                            continue;
                                    }
                                    if (int.Parse(drRg[g]["leftoverTestR3"].ToString()) > 0)//判定试剂剩余量是否大于0
                                    {
                                        rgPos = int.Parse(drRg[g]["Postion"].ToString());//获取该试剂位置编号
                                        rgindex = g;
                                        break;
                                    }
                                }
                            }
                        }
                        if (rgPos > 0)
                        {
                            //获取当前反应盘可用位置
                            int pos = (testTempS.AddSamplePos) % ReactTrayHoleNum == 0 ? ReactTrayHoleNum : (testTempS.AddSamplePos) % ReactTrayHoleNum;
                            //剩余R3体积
                            int leftR3 = int.Parse(OperateIniFile.ReadIniData("ReagentPos" + rgPos.ToString(), "LeftReagent3", "", iniPathReagentTrayInfo));
                            string leftR3Vol = (RegentNoUsePro + leftR3 * int.Parse(testTempS.AddLiqud) + leftR3 * abanR3Pro).ToString("x4");//2018-10-13 zlx mod
                            AddErrorCount = 0;
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 05 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                                + " " + int.Parse(testTempS.AddLiqud).ToString("x2") + " " + leftR3Vol.Substring(0, 2) + " " + leftR3Vol.Substring(2, 2)), 0);
                            if (!NetCom3.Instance.SPQuery())
                            {
                                #region 异常处理
                                Again:
                                string againSend = "";
                                if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.IsKnocked && AddErrorCount < 2)
                                {
                                    AddErrorCount++;
                                    //重新发送指令
                                    if (againSend == "")
                                        againSend = "EB 90 31 02 05 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                                + " 00 " + leftR3Vol.Substring(0, 2) + " " + leftR3Vol.Substring(2, 2);
                                }
                                else if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                                {
                                    //重新发送指令
                                    if (againSend == "")
                                        againSend = "EB 90 31 02 05 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                                + " " + int.Parse(testTempS.AddLiqud).ToString("x2") + " " + leftR3Vol.Substring(0, 2) + " " + leftR3Vol.Substring(2, 2);
                                }
                                else if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.OverTime)
                                {
                                    NetCom3.Instance.stopsendFlag = true;
                                    ShowWarnInfo("加样针加试剂时指令接收超时！", "加样", 1);
                                    //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘夹管到温育盘时发生撞管！");
                                    //MessageBox.Show("指令接收超时，实验已终止", "加样错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    //addLiquiding = false;
                                    AllStop();
                                }
                                int sendFlag = SendAgain(againSend, 0);
                                if (sendFlag == (int)ErrorState.Sendfailure)
                                    goto Again;
                                else if (sendFlag == (int)ErrorState.IsKnocked)
                                    AddErrorCount++;
                                #endregion
                            }
                            //drRg[rgindex]["leftoverTestR3"] = leftR3 - 1;
                            for (int i = 0; i < drRg.Length; i++)
                            {
                                if (int.Parse(drRg[i]["Postion"].ToString()) == rgPos)
                                    drRg[i]["leftoverTestR3"] = leftR3 - 1;
                            }
                            OperateIniFile.WriteIniData("ReagentPos" + rgPos.ToString(), "LeftReagent3", (leftR3 - 1).ToString(), iniPathReagentTrayInfo);
                            if (AddErrorCount > 0)
                            {
                                if (AddErrorCount > 1)
                                {
                                    NetCom3.Instance.stopsendFlag = true;
                                    ShowWarnInfo("加样针加试剂时撞针！", "加样", 1);
                                    //MessageBox.Show("加样针撞针未能修复，实验已终止", "加样错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    AllStop();
                                }
                                else
                                {
                                    MoveTubeListAddTubeDispose(pos);
                                    RemoveTestList(testTempS , "加样针加试剂3时发生撞针");
                                }
                                break;
                            }

                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 04 01 " + pos.ToString("x2")), 0);
                            if (!NetCom3.Instance.SPQuery())
                            {
                                #region 异常处理
                                string againSend = "";
                                Again:
                                if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                                {
                                    //重新发送指令
                                    if (againSend == "")
                                        againSend = "EB 90 31 04 01 " + pos.ToString("x2");
                                    if (SendAgain(againSend, 0) == (int)ErrorState.Sendfailure)
                                        goto Again;
                                }
                                else
                                {
                                    NetCom3.Instance.stopsendFlag = true;
                                    ShowWarnInfo("混匀" + pos + "位置时指令接收超时！", "加样", 1);
                                    //MessageBox.Show("混匀异常！", "加样错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    AllStop();
                                    break;
                                }
                                #endregion
                            }

                        }
                        #endregion
                        lisProBar[testTempS.TestID - 1].BarColor[StepIndex(dgvWorkListData.Rows[testTempS.TestID - 1].Cells[6].Value.ToString(), "R3")]
                            = Color.Gray;
                    }
                    lisProBar[testTempS.TestID - 1].Invalidate();
                    DalayFlag = false;
                    stepTime = 0;
                    break;
            }
            drRg = null;
            addLiquiding = false;
            //}
        }

        /// <summary>
        /// 加稀释液方法
        /// </summary>
        /// <param name="rgPos">试剂位置</param>
        /// <param name="pos">加样位置</param>
        /// <param name="leftdiuVol">加样体积</param>
        private int AddLiquid(int rgPos, int pos, int FirstDiu)
        {
            int AddErrorCount = 0;
            int LeftdiuVol = int.Parse(OperateIniFile.ReadIniData("ReagentPos" + rgPos.ToString(), "leftDiuVol", "", iniPathReagentTrayInfo));
            string  strLeftdiuVol = LeftdiuVol.ToString("x2");
            int len = strLeftdiuVol.Trim().Length;
            if (len < 4)
            {
                for (int i = len; i < 4; i++)
                {
                    strLeftdiuVol = "0" + strLeftdiuVol;
                }
            }
            LogFile.Instance.Write(DateTime.Now + ":strLeftdiuVol的值为"+ strLeftdiuVol); 
            string Order = "EB 90 31 02 06 ";
            NetCom3.Instance.Send(NetCom3.Cover(Order + rgPos.ToString("x2") + " " + pos.ToString("x2")
                                          + " " + FirstDiu.ToString("x2") + " " + strLeftdiuVol.Substring(0, 2) + " " + strLeftdiuVol.Substring(2, 2)), 0);
            //指令未执行完成进行等待

            if (!NetCom3.Instance.SPQuery())
            {
                #region 异常处理
                Again:
                string againSend = "";
                if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.IsKnocked && AddErrorCount < 2)
                {
                    AddErrorCount++;
                    //重新发送指令
                    if (againSend == "")
                        againSend = Order + rgPos.ToString("x2") + " " + pos.ToString("x2")
                        + " 00 " + strLeftdiuVol.Substring(0, 2) + " " + strLeftdiuVol.Substring(2, 2);
                }
                else if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.Sendfailure)
                {
                    //重新发送指令
                    if (againSend == "")
                        againSend = Order + rgPos.ToString("x2") + " " + pos.ToString("x2")
                        + " " + FirstDiu.ToString("x2") + " " + strLeftdiuVol.Substring(0, 2) + " " + strLeftdiuVol.Substring(2, 2);
                }
                else if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.OverTime)
                {
                    NetCom3.Instance.stopsendFlag = true;
                    //MessageBox.Show("指令接收超时，实验已终止", "加样错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //addLiquiding = false;
                    //AllStop();
                }
                int sendFlag = SendAgain(againSend, 0);
                if (sendFlag == (int)ErrorState.Sendfailure)
                    goto Again;
                else if (sendFlag == (int)ErrorState.IsKnocked)
                    AddErrorCount++;
                #endregion
            }
            #region 体积修改 lyn add 20180611
            DataRow[] drRg = dtRgInfo.Select("Postion=" + rgPos + "");
            OperateIniFile.WriteIniData("ReagentPos" + rgPos.ToString(), "leftDiuVol", (LeftdiuVol - (int)(FirstDiu + abanDiuPro)).ToString(), iniPathReagentTrayInfo);
            #endregion
            return AddErrorCount;
        }
        /// <summary>
        /// 异常消息显示
        /// </summary>
        /// <param name="threadid">动作执行者</param>
        /// <param name="state">异常状态</param>
        /// <param name="moving">执行动作</param>
        /// <param name="stopLog">实验是否停止</param>
        private void ShowWarnInfo(string message, string state, int stopLog)
        {
            setmainformbutten();
            string stopMessage = "";
            if (stopLog == 1) stopMessage = ",实验已停止！";
            LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + message);
            MessageBox.Show(message + stopMessage, state + "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        /// <summary>
        /// 将本步骤之前的步骤对应的进度条变灰
        /// </summary>
        /// <param name="testTempS">实验步骤</param>
        /// <param name="step">本步骤对应字符串</param>
        /// <returns></returns>
        private void toGray(int TestID, int index)
        {
            for (int i = index - 1; i >= 0; i--)
            {
                lisProBar[TestID].BarColor[i] = Color.Gray;
            }
        }

        /// <summary>
        /// 重复发送指令
        /// </summary>
        /// <param name="order">需要发送的指令</param>
        /// <param name="sengType">发送类型</param>
        /// <returns></returns>
        private int SendAgain(string order,int sendType)
        {
             NetCom3.Instance.Send(NetCom3.Cover(order), sendType);
             if (sendType == 0)//加样
             {
                 NetCom3.Instance.SPQuery();
                 return (int)NetCom3.Instance.AdderrorFlag;
             }
             if (sendType == 1)
             {
                 NetCom3.Instance.SPQuery();//移管
                 return (int)NetCom3.Instance.AdderrorFlag;
             }
             if (sendType == 2)
             {
                 NetCom3.Instance.SPQuery();//清洗
                 return (int)NetCom3.Instance.WasherrorFlag;
             }
             return -1;
        }
        /// <summary>
        /// 获取各个步骤在当前实验的索引
        /// </summary>
        /// <param name="steps">所有步骤</param>
        /// <param name="currentStep">当前步骤</param>
        /// <returns></returns>
        int StepIndex(string steps, string currentStep)
        {
            string[] step = steps.Split('-');
            return step.ToList().IndexOf(currentStep);
        }
        /// <summary>
        /// 管架取管到温育盘
        /// </summary>
        /// <param name="RackPos">管架位置</param>
        /// <param name="ReactPos">温育盘位置</param>
        /// <returns></returns>
        void rackToReact(int ReactPos)
        {
            #region 发送指令及配置文件的实时更改（管架夹新管到反应盘）
            /*
            while (MoveTubeUseFlag)
            {
                NetCom3.Delay(10);
            }
            MoveTubeUseFlag = true;
            //移管手到取管位置takepos[1]（管架）取管
            int plate = RackPos % 88 == 0 ? RackPos / 88 - 1 : RackPos / 88;//几号板
            int column = RackPos % 11 == 0 ? RackPos / 11 - (plate * 8) : RackPos / 11 + 1 - (plate * 8);
            int hole = RackPos % 11 == 0 ? 11 : RackPos % 11;
        
            #region 取管成功
            int TubeRack = RackPos / 88;
            int curTube = RackPos % 88;
            if (curTube == 0 && RackPos != 0)
            {
                TubeRack = TubeRack - 1;
                curTube = 88;
            }
            //那个架子减了一个管
            OperateIniFile.WriteIniData("Tube", "Pos" + (TubeRack + 1).ToString(), (88 - curTube).ToString(), iniPathSubstrateTube);
            List<int> lisTubeNum = new List<int>();
            lisTubeNum = QueryTubeNum();
            //移管手要夹的下一个管架位置
            int NextPos = RackPos + 1;
            //管架中第一个装载管架的索引
            int firstTubeIndex = lisTubeNum.FindIndex(ty => ty <= 88 && ty > 0);
            if (firstTubeIndex < 0)
            {
                setmainformbutten();
                TubeStop = true;
                LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "异常" + " *** " + "未读" + " *** " + "管架为空，将停止加样！");
            }
            for (int i = 1; i <= lisTubeNum.Count; i++)
            {
                if (NextPos == i * 88 + 1)
                {
                    NextPos = firstTubeIndex * 88 + (88 - lisTubeNum[firstTubeIndex]) + 1;
                }
            }
          
            OperateIniFile.WriteIniData("Tube", "TubePos", NextPos.ToString(), iniPathSubstrateTube);
             */
            while (MoveTubeUseFlag && !NetCom3.Instance.stopsendFlag)
            {
                Thread.Sleep(30);
            }
            if (NetCom3.Instance.stopsendFlag)
            {
                return;
            }
            MoveTubeUseFlag = true;
            int iNeedCool = 0;
            int IsKnockedCool = 0;
            AgainNewMove:
            //到管架takepos[1]位置取管放到温育盘putpos位置
            //NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 01 " + plate.ToString("x2") + " " + column.ToString("x2")
            //    + " " + hole.ToString("x2") + " " + ReactPos.ToString("x2")), 1);

            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 01 " + ReactPos.ToString("x2")), 1);
            if (!NetCom3.Instance.MoveQuery())
            {
                TubeProblemFlag = true;
                //TubeProblemFlag = true;
                #region 发生异常处理
                if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull)
                {
                    iNeedCool++;
                    if (iNeedCool < 3)
                    {
                        goto AgainNewMove;
                    }
                    else
                    {
                        TubeStop = true;
                        if ((ReactPos != 1 && ReactPos != 2 && ReactPos != 3) && !AddTubeStop.Contains(ReactPos))
                        {
                            OperateIniFile.ReadIniData("ReactTrayInfo", "no" + ReactPos, "", iniPathReactTrayInfo);
                            lock (AddTubeStop)
                            {
                                AddTubeStop.Add(ReactPos);

                            }
                        }
                        dbtnRackStatus();
                        setmainformbutten();
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在暂存盘向温育盘抓新管时多次抓空!实验暂停加样！");
                        TubeProblemFlag = false; //2019-08-24 ZLX add
                    }

                }
                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.LackTube)
                {
                    TubeStop = true;
                    if ((ReactPos != 1 && ReactPos != 2 && ReactPos != 3) && !AddTubeStop.Contains(ReactPos))
                    {
                        OperateIniFile.ReadIniData("ReactTrayInfo", "no" + ReactPos, "", iniPathReactTrayInfo);
                        lock (AddTubeStop)
                        {
                            AddTubeStop.Add(ReactPos);
                        }
                    }
                    dbtnRackStatus();
                    setmainformbutten();
                    TubeProblemFlag = false;//2019-08-24 ZLX add

                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "理杯机缺管!实验暂停加样！");
                    LogFile.Instance.Write("理杯机缺管!TubeProblemFlag的值为：" + TubeProblemFlag);
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
                    {
                        goto AgainNewMove;
                    }
                    else
                    {
                        setmainformbutten();
                        NetCom3.Instance.stopsendFlag = true;
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手向温育盘抓新管时取管撞管！");
                        DialogResult tempresult = MessageBox.Show("移管手向温育盘抓新管时取管撞管，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        AllStop();
                    }
                }
                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.putKnocked)
                {
                    IsKnockedCool++;
                    GAgainNewMove:
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 " + ReactPos.ToString("x2")), 1);
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        #region 异常处理
                        if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked)
                        {
                            IsKnockedCool++;
                            if (IsKnockedCool < 2)
                            {
                                goto GAgainNewMove;
                            }
                            else
                            {
                                setmainformbutten();
                                NetCom3.Instance.stopsendFlag = true;
                                LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在温育盘扔废管时取管撞管！");
                                DialogResult tempresult = MessageBox.Show("移管手在温育盘扔废管时取管撞管！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                AllStop();
                            }
                        }
                        #endregion
                    }
                    if (IsKnockedCool < 2)
                    {
                        goto AgainNewMove;
                    }
                    else
                    {
                        setmainformbutten();
                        NetCom3.Instance.stopsendFlag = true;
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手向温育盘抓新管时放管撞管！");
                        DialogResult tempresult = MessageBox.Show("移管手向温育盘抓新管时放管撞管，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        AllStop();
                    }
                }
                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.OverTime)
                {
                    setmainformbutten();
                    NetCom3.Instance.stopsendFlag = true;
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在暂存盘向温育盘抓新管时接收数据超时！");
                    AllStop();
                    DialogResult tempresult = MessageBox.Show("移管手在暂存盘向温育盘抓新管时接收数据超时，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                }
                #endregion
            }
            else
            {
                TubeProblemFlag = false;
                OperateIniFile.WriteIniData("ReactTrayInfo", "no" + ReactPos, "1", iniPathReactTrayInfo);
            }
            MoveTubeUseFlag = false;
            #endregion
            #endregion
        }
        /// <summary>
        /// 温育盘扔废管
        /// </summary>
        /// <param name="takepos"></param>
        void ReactToAband(int takepos)
        {

            MoveTubeUseFlag = true;
            //NetCom3.IncubateTray.WaitOne(15000);//add y 20180524
            int iNeedCool = 0;
            int IsKnockedCool = 0;
            AgainNewMove:
            ///温育盘takepos[1]取管扔废管
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 " + takepos.ToString("x2")), 1);
            if (!NetCom3.Instance.MoveQuery())
            {
                #region 发生异常处理
                if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull)
                {
                    iNeedCool++;
                    if (iNeedCool < 2)
                        goto AgainNewMove;
                    else
                    {
                        setmainformbutten();
                        NetCom3.Instance.stopsendFlag = true;
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在温育盘扔废管时多次抓空！");
                        DialogResult tempresult = MessageBox.Show("移管手在温育盘扔废管时多次抓空，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        AllStop();
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
                    {
                        goto AgainNewMove;
                    }
                    else
                    {
                        setmainformbutten();
                        NetCom3.Instance.stopsendFlag = true;
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在温育盘扔废管时取管撞管！");
                        DialogResult tempresult = MessageBox.Show("移管手在温育盘扔废管时取管撞管，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        AllStop();
                    }
                }
                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.OverTime)
                {
                    setmainformbutten();
                    NetCom3.Instance.stopsendFlag = true;
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在温育盘扔废管时接收数据超时！");
                    DialogResult tempresult = MessageBox.Show("移管手在温育盘扔废管时接收数据超时，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    AllStop();
                }
                #endregion
            }
            else
            {
                ///取管成功
                OperateIniFile.WriteIniData("ReactTrayInfo", "no" + takepos.ToString(), "0", iniPathReactTrayInfo);
            }
            MoveTubeUseFlag = false;

        }

        /// <summary>
        /// 运行时间计时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timeReckon_Tick(object sender, EventArgs e)
        {
            while (frmSampleLoad.CaculatingFlag ||
                (TubeProblemFlag && (RunFlag == (int)RunFlagStart.IsRuning)))
            {
                Thread.Sleep(30);
            }
            if (W1EndTime.Count == 0)
            {
                if ((sumTime == stepTime && sumTime != 0))
                    DalayFlag = true;
            }
            else
            {
                if ((sumTime == stepTime && sumTime != 0) || (sumTime != 0 && sumTime == W1EndTime[0]))
                {
                    DalayFlag = true;
                }
            }
            sumTime++;
        }
        #region 清洗相关方法
        /// <summary>
        /// 清洗盘中是否存在即将夹出的反应管
        /// </summary>
        /// <returns></returns>
        bool ClamptubeWash()
        {
            //2018-08-25 zlx mod
            if (lisMoveTube.Count >0 && lisMoveTube.Exists(ty => ty.TakeTubePos != null && ty.TakeTubePos.Split('-')[0] == "2"))
                return true;
            else
                return false;
        }
        private void timerWash_Tick(object sender, EventArgs e)
        {
            if (RunFlag != (int)RunFlagStart.IsRuning || NetCom3.Instance.stopsendFlag)//2018-5-22 zlx add
            {
                return;
            }
            DalayFlag = false;//20180525 y zhushidiao
            if (WashTrayUseFlag)
            {
                DalayFlag = true;
            }
            timer.Enabled = false;//20180525 y add
            while (WashTrayUseFlag || WashTurnFlag || NoTateFlag || NetCom3.Instance.stopsendFlag || ClamptubeWash())
            {
                Thread.Sleep(100);
            }
            WashTrayUseFlag = true;//20180526 y move
            WashTurnFlag = true;
            #region 清洗盘旋转一位
            ///清洗盘逆时针旋转一位(发送指令)
            AgainSend:
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);           
            timer.Enabled = true;//20180525 y add
            if (!NetCom3.Instance.WashQuery())//2018-10-13 zlx mod
            {
                if (NetCom3.Instance.WasherrorFlag == (int)ErrorState.Sendfailure)
                {
                    LogFile.Instance.Write(DateTime.Now+"清洗盘旋转指令重新发送！");
                    goto AgainSend;
                }
                else if (NetCom3.Instance.WasherrorFlag == (int)ErrorState.OverTime)
                {
                    NetCom3.Instance.stopsendFlag = true;
                    ShowWarnInfo("清洗盘旋转指令接收超时","清洗",1);
                    //MessageBox.Show("指令接收超时，实验已终止", "清洗指令错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //addLiquiding = false;
                    AllStop();
                }
            }
            countWashHole(1);
            currentHoleNum++;
            //如果孔号超过30，孔号设为1
            if (currentHoleNum == 31)
            {
                currentHoleNum = 1;
            }
            //LogFile.Instance.Write("==================  当前位置  " + currentHoleNum ); 
            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
            //旋转一位当前取放管位置的孔号加1
            DataTable dtTemp = new DataTable();
            dtTemp = dtWashTrayTubeStatus.Copy();
            //清洗盘状态列表中添加反应盘位置字段，赋值需多赋值一个字段。 LYN add 20171114
            for (int j = 1; j < 5; j++)
                dtWashTrayTubeStatus.Rows[0][j] = dtTemp.Rows[dtWashTrayTubeStatus.Rows.Count - 1][j];
            for (int i = 1; i < dtWashTrayTubeStatus.Rows.Count; i++)
            {
                for (int j = 1; j < 5; j++)
                {
                    dtWashTrayTubeStatus.Rows[i][j] = dtTemp.Rows[i - 1][j];
                }
            }
            OperateIniFile.WriteConfigToFile("[TubePosition]", iniPathWashTrayInfo, dtWashTrayTubeStatus);
            WashTurnFlag = false;
            washStep();
            WashTrayUseFlag = false;
            #endregion

        }
        private void setmainformbutten()//更改主界面按钮颜色
        {
            if (this.Parent.Parent is frmMain)
            {
                ((frmMain)this.Parent.Parent).LogBtnColorChange(1);
            }
        }
        private Object locker2 = new object();//锁，用于移管手列表，lisMovetube的修改操作
        MoveTubeStatus[] MoveTubeError = new MoveTubeStatus[15];//当移管手指令出错时，暂时存储对应的移管手MoveTubeStatus，用于计算同一个指令出错的次数
        int ipostemp = 0;//数组MoveTubeError的当前可以存储的项的下标，相当于指针
        bool CatchVacancyThisTime = false;//是否抓空，根据此来决定如何修改清洗盘状态表
        ManualResetEvent MoveTubeManualReset = new ManualResetEvent(true);//当移管手出错时，阻止lisMoveTube列表继续被提取并被发送相关的移管手指令
        public static bool IsDoubleTime = false;//加液针是否是第二次出错，根据此来决定处理加液针出错的行为
        public static ManualResetEvent AddResetEvent = new ManualResetEvent(true);//当加液针出错时，阻止相关加液指令被发送
        public static bool TrayRemoveAllTube = false;//y add 抓空标志位，确定是否触发抓空异常
        MoveTubeStatus tempMoveTubeStatus;
        void SetItemInMoveTubeError(MoveTubeStatus moveTube)//将一个MoveTubeStatus存入MoveTubeError
        {
            MoveTubeError[ipostemp] = moveTube;
            ipostemp++;
            if (ipostemp == 15) ipostemp = 0;
        }
        #region 屏蔽异常处理
        /*
        private void DisposeMoveAndAddError(Object obj,EventArgs args)//返回移管手或加液臂执行出错的指令时，对移管进度指令进行调整
        {
            int Type = (int)obj;//出错的类型
            if (Type < 6)
            {
                if ((Type == 3 || Type == 2) && TrayRemoveAllTube) return;//如果是清洗盘清空或者温育盘清空，则不触发异常处理
                if ((Type == 1) && TrayRemoveAllTube)//清空时撞管
                {
                    string order = ((AddOrder)args).order;
                    MoveTubeStatus status = new MoveTubeStatus() { putTubePos = "0-" + order + currentHoleNum, TakeTubePos = "2-" + currentHoleNum };
                    int temptemp = 0;
                    foreach (var item in MoveTubeError)
                    {
                        if (status.Equals(item)) temptemp++;
                    }
                    if (temptemp > 0)
                    {
                        MessageBox.Show("清空清洗盘或温育盘时多次撞管，请进行维修。实验已终止。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "清空清洗盘或温育盘时多次撞管，请进行维修。实验已终止。");
                        setmainformbutten();
                        AllStop();
                        return;
                    }
                    else
                    {
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "警告" + " *** " + "未读" + " *** " + "清空清洗盘或温育盘时撞管。");
                        setmainformbutten();
                    }
                    SetItemInMoveTubeError(status);
                    NetCom3.Instance.Send(order, 4);
                    return;
                }
                if ((Type == 1 || Type == 3)&& isCleanPipeLineNow)//清洗管路时在清洗盘撞针或抓空
                {
                    string order = ((AddOrder)args).order;
                    MoveTubeStatus status = new MoveTubeStatus() { putTubePos = "0-" + order + currentHoleNum, TakeTubePos = "2-" + currentHoleNum };
                    int temptemp = 0;
                    foreach (var item in MoveTubeError)
                    {
                        if (status.Equals(item)) temptemp++;
                    }
                    if (temptemp > 0)
                    {
                        MessageBox.Show("移管手多次撞管，请进行维修。实验已终止。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);//"+ (Type == 1 ?"撞针":"在清洗盘抓空多次")+ "
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手" + (Type == 1 ? "撞针" : "在清洗盘抓空多次") + "，请进行维修。实验已终止。");
                        setmainformbutten();
                        AllStop();
                        return;
                    }
                    else
                    {
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "警告" + " *** " + "未读" + " *** " + "移管手" + (Type == 1 ? "撞针" : "抓空") + "。");
                        setmainformbutten();
                    }
                    SetItemInMoveTubeError(status);
                    NetCom3.Instance.Send(order, 4);
                    return;
                }
                MoveTubeStatus moveTubeError = tempMoveTubeStatus ?? (lisMoveTube.Count>0? lisMoveTube[0]:null);
                MoveTubeStatus moveTubeCopy = MoveTubeListAddOne(moveTubeError);
                if (moveTubeError == null || moveTubeCopy == null) return;
                lock (locker2)
                {
                    moveTubeError.isRetransmit = true;
                }

                int iNeedCool = 0;//此前有多少个相同的出错类型
                foreach (var item in MoveTubeError)
                {
                    if (moveTubeError.Equals(item)) iNeedCool++;
                }
                SetItemInMoveTubeError(moveTubeError);
                if (Type == 1)//撞管
                {
                    if (iNeedCool > 0)
                    {
                        MessageBox.Show("移管手撞管，请进行维修。实验已终止。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ThreadPool.QueueUserWorkItem(new WaitCallback((ob) => { NetCom3.Instance.CleanAllReactTray(); }), null);
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "温育盘移管手多次撞针，实验已停止。");
                        setmainformbutten();
                        AllStop();
                    }
                    else
                    {
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "警告" + " *** " + "未读" + " *** " + "温育盘移管手撞针。");
                        setmainformbutten();
                    }
                }
                else
                {
                    if (Type == 2)//温育盘抓空
                    {
                        if (iNeedCool > 0)
                        {
                            MessageBox.Show("移管手多次抓空，请进行维修,实验已终止。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ThreadPool.QueueUserWorkItem(new WaitCallback((ob) => { NetCom3.Instance.CleanAllReactTray(); }), null);
                            LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "温育盘移管手多次抓空，实验已停止。");
                            setmainformbutten();
                            AllStop();
                        }
                        else
                        {
                            LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "警告" + " *** " + "未读" + " *** " + "温育盘移管手抓空。");
                            setmainformbutten();
                        }
                        CatchVacancyThisTime = true;
                    }
                    else if (Type == 3)//清洗盘抓空
                    {
                        if (iNeedCool > 0)
                        {
                            MessageBox.Show("移管手多次抓空，请进行维修,实验已终止。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            ThreadPool.QueueUserWorkItem(new WaitCallback((ob) => { NetCom3.Instance.CleanAllReactTray(); }), null);
                            LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "清洗盘移管手多次抓空，实验已停止。");
                            setmainformbutten();
                            AllStop();
                        }
                        else
                        {
                            LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "警告" + " *** " + "未读" + " *** " + "清洗盘移管手抓空。");
                            setmainformbutten();
                        }
                        CatchVacancyThisTime = true;
                    }
                    else if (Type == 4)//管架抓空
                    {
                        string[] takepos = moveTubeError.TakeTubePos.Split('-');
                        bool isMoevtoNextBoard;
                        int nextPos = 0;
                        if (iNeedCool == 3 || iNeedCool == 6 || iNeedCool == 9)
                        {
                            nextPos = BoardNextPos(int.Parse(takepos[1]), true, out isMoevtoNextBoard);
                        }
                        else
                        {
                            nextPos = BoardNextPos(int.Parse(takepos[1]), false, out isMoevtoNextBoard);
                            if (isMoevtoNextBoard)
                            {
                                int intTemp = 3 - (iNeedCool % 3);
                                iNeedCool += intTemp;
                                for (int i = 0; i < intTemp; i++)
                                {
                                    SetItemInMoveTubeError(moveTubeError);
                                }
                            }
                        }
                        int j = 0;
                        for (; j < lisMoveTube.Count; j++)
                        {
                            if (lisMoveTube[j] == moveTubeCopy) break;
                        }
                        if (iNeedCool > 11)
                        {
                            SortMoveTubeList(1,j);//整理顺序
                            LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "管架移管手多次抓空");
                            setmainformbutten();
                            DialogResult tempresult = MessageBox.Show("移管手多次抓空，请进行维修,实验已暂停。" + Environment.NewLine
                                + "点击“确定”实验继续，并视为一号暂存盘装载完成。" + Environment.NewLine + "点击“取消”终止实验", "错误"
                                , MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                            if (tempresult == System.Windows.Forms.DialogResult.Cancel)
                            {
                                AllStop();
                            }
                        }
                        else
                        {
                            SortMoveTubeList(nextPos,j);
                            LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "警告" + " *** " + "未读" + " *** " + "暂存盘移管手抓空");
                            setmainformbutten();
                        }
                    }
                }
            }
            else
            {
                if (Type == 6)//从暂存盘抓新管到清洗盘，抓空处理
                {
                    string order = ((AddOrder)args).order;
                    string[] OrderPart = order.Split(' ');
                    int plate = int.Parse(OrderPart[5]);
                    int colum = int.Parse(OrderPart[6]);
                    int hole = int.Parse(OrderPart[7]);
                    int ciunt = plate * 88 + (colum - 1) * 11 + hole;
                    MoveTubeStatus moveTube = new MoveTubeStatus();
                    moveTube.TakeTubePos = "0-" + ciunt;
                    moveTube.putTubePos = "2-" + currentHoleNum;
                    moveTube.StepNum = -1;
                    moveTube.TestId = -1;

                    int iNeedCool = 0;
                    foreach (var item in MoveTubeError)
                    {
                        if (moveTube.Equals(item)) iNeedCool++;
                    }
                    SetItemInMoveTubeError(moveTube);

                    bool isMoevtoNextBoard;
                    int nextPos = 0;
                    if (iNeedCool == 3 || iNeedCool == 6 || iNeedCool == 9)
                    {
                        nextPos = BoardNextPos(ciunt, true, out isMoevtoNextBoard);
                    }
                    else
                    {
                        nextPos = BoardNextPos(ciunt, false, out isMoevtoNextBoard);
                        if (isMoevtoNextBoard)
                        {
                            int intTemp = 3 - (iNeedCool % 3);
                            iNeedCool += intTemp;
                            for (int i = 0; i < intTemp; i++)
                            {
                                SetItemInMoveTubeError(moveTube);
                            }
                        }
                    }
                    if (iNeedCool > 11)
                    {
                        SortMoveTubeList(1);//整理顺序
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "暂存盘移管手多次抓空");
                        setmainformbutten();
                        DialogResult tempresult = MessageBox.Show("移管手多次抓空，请进行维修,实验已暂停。" + Environment.NewLine
                            + "点击“确定”实验继续，并视为暂存盘装载完成。" + Environment.NewLine + "点击“取消”终止实验", "错误"
                            , MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        if (tempresult == System.Windows.Forms.DialogResult.Cancel)
                        {
                            AllStop();
                        }
                    }
                    else
                    {
                        AddTubeInCleanTray(nextPos);
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "警告" + " *** " + "未读" + " *** " + "暂存盘移管手抓空");
                        setmainformbutten();
                    }
                }
                if (Type == 11)//加液针撞针
                {
                    string order = ((AddOrder)args).order;
                    bool IsDouble = ((AddOrder)args).IsDoubleTimeAndSucceed;
                    string[] orderPart = order.Split(' ');
                    if (IsDouble)
                    {
                        IsDoubleTime = false;
                        MoveTubeManualReset.Reset();

                        bool isDuilte = false;
                        //int pos = Convert.ToInt32(orderPart[6], 16);
                        //int posTwo = Convert.ToInt32(orderPart[6], 16);
                        //if (orderPart[4] == "02")
                        //{
                        //    pos = Convert.ToInt32(orderPart[5], 16);
                        //    isDuilte = true;
                        //}
                        int pos = Convert.ToInt32(orderPart[5], 16);
                        int posTwo = Convert.ToInt32(orderPart[5], 16);
                        MoveTubeListAddTubeDispose(pos);
                        if (isDuilte)
                        {
                            MoveTubeListAddTubeDispose(posTwo);
                        }
                        MoveTubeManualReset.Set();
                        
                        RemoveTestList(pos, posTwo, isDuilte);

                        AddResetEvent.Set();
                        NetCom3.ComWait.Set();
                        //2018-12-03 zlx add
                        string step = "";
                        if (orderPart[5] == "01")
                            step = "样本";
                        else if (orderPart[5] == "02")
                            step = "稀释样本";
                        else if (orderPart[5] == "03")
                            step = "试剂1";
                        else if (orderPart[5] == "04")
                            step = "试剂2";
                        else if (orderPart[5] == "05")
                            step = "试剂3";
                        else if (orderPart[5] == "06")
                            step = "稀释液";
                        else if (orderPart[5] == "07")
                            step = "磁珠";
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "加液针在加入" + step + "撞针，对应实验已经移除");
                        //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "加液针发生多次撞针，在位置" + pos + (isDuilte ? "和" + posTwo.ToString() : "") + "，对应实验已经移除");
                        //2018-10-25 zlx add
                        int TestID = lisTestSchedule.Find(ty => ty.AddSamplePos == pos).TestID;
                        foreach (DataGridViewRow dr in dgvWorkListData.Rows)
                        {
                            if (int.Parse(dr.Cells[1].Value.ToString()) == TestID)
                            {
                                dr.Cells["TestStatus"].Value = "实验废弃";
                                //BeginInvoke(TestStatusInfo, new object[] { "实验废弃！", dr.Cells[1].Value.ToString() });
                                dr.DefaultCellStyle.BackColor = Color.Gray;
                                SampleNumCurrent = SampleNumCurrent - 1;
                                break;
                            }
                        }
                        setmainformbutten();
                    }
                    else
                    {
                        NetCom3.ComWait.Reset();
                        AddResetEvent.Reset();
                        IsDoubleTime = true;
                        orderPart[7] = "00";
                        String tempstring = null;
                        foreach (var item in orderPart)
                        {
                            tempstring += item + " ";
                        }
                        tempstring = tempstring.TrimEnd();
                        NetCom3.Instance.Send(tempstring, 4);
                        //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "警告" + " *** " + "未读" + " *** " + "加液针撞针一次。");
                        setmainformbutten();
                    }
                    
                }
            }
        }
        private MoveTubeStatus MoveTubeListAddOne(MoveTubeStatus moveTube)//add y 20180727  复制lisMoveTube中出错的那一项，然后将此复制元素增加到lisMoveTube列表头。
        {
            if (moveTube == null)
            {
                return null;
            }
            MoveTubeStatus tubeStatus = new MoveTubeStatus(moveTube);
            lock (locker2)
            {
                if (lisMoveTube.Count == 0)
                {
                    lisMoveTube.Insert(0, tubeStatus);
                }
                else
                {
                    lisMoveTube.Insert(1, tubeStatus);
                }
            }
             return tubeStatus;
        }
        private void SortMoveTubeList(int FirstPos , int starPos = 1)//add y 20180727  整理movetubelis
        {
            lock (locker2)
            {
                int pos = FirstPos;//取管位置
                for (int j = starPos; j < lisMoveTube.Count; j++)//从某个位置开始
                {
                    var item = lisMoveTube[j];
                    String[] TakeTubeAtring = item.TakeTubePos.Split('-');
                    if (TakeTubeAtring[0] == "0")
                    {
                        item.TakeTubePos = "0-" + pos;
                        pos++;
                        if (pos > 352)
                        {
                            int tary = -1;
                            int[] PosTemp = new int[4];
                            PosTemp[0] = int.Parse(Common.OperateIniFile.ReadIniData("Tube", "Pos1", "1", System.IO.Directory.GetCurrentDirectory() + "\\SubstrateTube.ini")) == 88 ? 0 : -1;
                            PosTemp[1] = int.Parse(Common.OperateIniFile.ReadIniData("Tube", "Pos2", "1", System.IO.Directory.GetCurrentDirectory() + "\\SubstrateTube.ini")) == 88 ? 1 : -1;
                            PosTemp[2] = int.Parse(Common.OperateIniFile.ReadIniData("Tube", "Pos3", "1", System.IO.Directory.GetCurrentDirectory() + "\\SubstrateTube.ini")) == 88 ? 2 : -1;
                            PosTemp[3] = int.Parse(Common.OperateIniFile.ReadIniData("Tube", "Pos4", "1", System.IO.Directory.GetCurrentDirectory() + "\\SubstrateTube.ini")) == 88 ? 3 : -1;
                            for (int i = 0; i < 4; i++)
                            {
                                if (PosTemp[i] != -1)
                                {
                                    tary = PosTemp[i];
                                    break;
                                }
                            }
                            if (tary == -1)
                            {
                                AllPause();
                                MessageBox.Show("缺少反应管，实验已暂停，请装填管架，然后稍后重新开始", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                pos = 1;
                            }
                            else pos = tary * 88 + 1;
                        }
                    }
                }
            }
        }
        
       
        private int BoardNextPos(int pos,bool moveToNextBoard, out bool isRemoveBoarf)//add y 20180727  返回下一个管位置
        {
            if (pos == 0)
            {
                int boardPos = int.Parse(OperateIniFile.ReadIniData("Tube", "TubePos", "1", System.IO.Directory.GetCurrentDirectory() + "\\SubstrateTube.ini"));
                OperateIniFile.WriteIniData("Tube", "TubePos", ((boardPos+1)>352 ? 1: (boardPos + 1)).ToString(), iniPathSubstrateTube);
                isRemoveBoarf = moveToNextBoard;
                int platee = boardPos % 88 == 0 ? boardPos / 88 - 1 : boardPos / 88;//2018-09-03 zlx mod
                int number = int.Parse(Common.OperateIniFile.ReadIniData("Tube", "Pos"+(platee+1).ToString(), "1", System.IO.Directory.GetCurrentDirectory() + "\\SubstrateTube.ini"));
                if (number <1)
                {
                    number = 1;
                }
                OperateIniFile.WriteIniData("Tube", "Pos" + (platee + 1).ToString(), (number-1).ToString(), iniPathSubstrateTube);
                return boardPos;
            }
            isRemoveBoarf = moveToNextBoard;
            int plate = pos % 88 == 0 ? pos / 88 - 1 : pos / 88;//几号板
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
                OperateIniFile.WriteIniData("Tube", "Pos" + (plate==0?4:plate).ToString(), (0).ToString(), iniPathSubstrateTube);
            }
            OperateIniFile.WriteIniData("Tube", "TubePos", (plate * 88 + (column - 1) * 11 + hole + 1).ToString(), iniPathSubstrateTube);
            int count = int.Parse(Common.OperateIniFile.ReadIniData("Tube", "Pos" + (plate + 1).ToString(), "1", System.IO.Directory.GetCurrentDirectory() + "\\SubstrateTube.ini"));
            if (count < 1) count = 1;
            OperateIniFile.WriteIniData("Tube", "Pos" + (plate + 1).ToString(), (count-1).ToString(), iniPathSubstrateTube);
            return plate * 88 + (column - 1) * 11 + hole;
        }
         */
        #endregion
        private void MoveTubeListAddTubeDispose(int pos)//add y 20180727  丢掉不用的管,加液针撞针需要的处理
        {
            MoveTubeStatus moveTubeStatus = new MoveTubeStatus();
            moveTubeStatus.TakeTubePos = "1-" + pos;
            moveTubeStatus.putTubePos = "0";
            lock (locker2)
            {
                lisMoveTube.Add(moveTubeStatus);
            }
        }
        private void  RemoveTestList(TestSchedule testSchedule , string endReason)//add y 20180727  整理testschould列表，将进度中撞针的实验设置为跳过，以实现可以继续剩下的实验
        {
            LogFile.Instance.Write("调用了RemoveTestList(TestSchedule testSchedule)方法！");
            string TestnameTemp = testSchedule.TestID.ToString();
            Thread thread;//2019-2-21 zlx mod
            if(TubeStop)
                thread= new Thread(new ThreadStart(() => { MessageBox.Show("编号为:" + TestnameTemp + " 的测试因稀释缺管已废弃，请稍后重新运行此测试", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information); }));
            else
                thread = new Thread(new ThreadStart(() => { MessageBox.Show("编号为:" + TestnameTemp + " 的测试因发生故障已废弃，请稍后重新运行此测试", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information); }));
            if (thread!=null)
            {
                
                thread.IsBackground = true;
                thread.Start();
            }
            LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "编号为:" + TestnameTemp + " 的实验因发生故障已废弃,废弃实验在温育盘" + testSchedule.AddSamplePos + "号位置" + "," + endReason);
            foreach (DataGridViewRow dr in dgvWorkListData.Rows)
            {
                if (int.Parse(dr.Cells[1].Value.ToString()) == testSchedule.TestID)
                {
                    dr.Cells["TestStatus"].Value = "实验废弃";
                    //BeginInvoke(TestStatusInfo, new object[] { "实验废弃！", dr.Cells[1].Value.ToString() });
                    dr.DefaultCellStyle.BackColor = Color.Gray;
                    SampleNumCurrent = SampleNumCurrent - 1;
                    if ((StopList.Count > 0) &&
                              StopList.Contains(dr.Cells["No"].Value.ToString()))
                    {
                        StopList.Remove(dr.Cells["No"].Value.ToString());
                    }
                    break;
                }
            }
            List<TestSchedule> Temp = lisTestSchedule.FindAll(ty => ty.TestID == testSchedule.TestID);
            foreach (var item in Temp)
            {
                item.TestScheduleStep = TestSchedule.ExperimentScheduleStep.DoNotTakeCareThis;
            }
            //SampleNumCurrent = SampleNumCurrent - 1;
            LoadingHelper.CloseForm();
        }
        private void RemoveTestList(int pos, int posTwo, bool isDuilte)//add y 20180727  整理testschould列表，将进度中撞针的实验设置为跳过，以实现可以继续剩下的实验
        {
            LogFile.Instance.Write("调用了RemoveTestList(int pos, int posTwo, bool isDuilte)方法！");
            LoadingHelper.ShowLoadingScreen();
            TestSchedule testSchedule = null;
            //int pointer = 0;
            //foreach (var item in lisTestSchedule)
            //{
            //    if (item == _GaDoingOne) break;
            //    pointer++;
            //}
            //if ((pointer - 1) <= lisTestSchedule.Count - 1)
            //{
            //    TestSchedule thisone = null;
            //    thisone = lisTestSchedule[pointer - 1];
            //    _GaDoingOne = thisone;
            //}
            //for (int i = pointer; i >= 0; i--)
            //{
            //    if (isDuilte && lisTestSchedule[i].AddSamplePos == posTwo || lisTestSchedule[i].AddSamplePos == pos || isDuilte && int.Parse(lisTestSchedule[i].dilutionPos) == posTwo)
            //    {
            //        testSchedule = lisTestSchedule[i];
            //        break;
            //    }
            //}
            
            //if (testSchedule == null) MessageBox.Show("未能找到该实验项目");

            string TestnameTemp = testSchedule.TestID.ToString();
            Thread thread = new Thread(new ThreadStart(() => { MessageBox.Show("编号为:" + TestnameTemp + " 的实验因为发生撞针，无法继续，已经排除，请在稍后重新运行此实验", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information); }));
            //2018-10-16 zlx mod
            foreach (DataGridViewRow dr in dgvWorkListData.Rows)
            {
                if (int.Parse(dr.Cells[1].Value.ToString()) == testSchedule.TestID)
                {
                    dr.Cells["TestStatus"].Value = "实验废弃";
                    //BeginInvoke(TestStatusInfo, new object[] { "实验废弃！", dr.Cells[1].Value.ToString() });
                    dr.DefaultCellStyle.BackColor = Color.Gray;
                    SampleNumCurrent = SampleNumCurrent - 1;
                    if ((StopList.Count > 0) &&
                              StopList.Contains(dr.Cells["No"].Value.ToString()))
                    {
                        StopList.Remove(dr.Cells["No"].Value.ToString());
                    }
                    break;
                }
            }
            //foreach (DataGridViewRow dr in dgvWorkListData.Rows)
            //{
            //    if (dr.Cells[3].Value.Equals(pos))
            //    {
            //        BeginInvoke(TestStatusInfo, new object[] { "实验废弃！", dr.Cells[1].Value.ToString() });
            //        dr.DefaultCellStyle.BackColor = Color.Gray;
            //    }
            //}
            thread.IsBackground = true;
            thread.Start();

            List<TestSchedule> Temp = lisTestSchedule.FindAll(ty => ty.TestID == testSchedule.TestID);
            foreach (var item in Temp)
            {
                item.TestScheduleStep = TestSchedule.ExperimentScheduleStep.DoNotTakeCareThis;
            }
            SampleNumCurrent = SampleNumCurrent - 1;
            LoadingHelper.CloseForm();

        }
        private void DisposeMoveAndAddError(string errorname,int errorState)
        { 
            
        }

        /// <summary>
        /// 清洗盘取放管方法
        /// </summary>
        /// <param name="TubeWashInfo">无用</param>
        void MoveTube(object TubeInfo)
        {
            while (true)
            {
                again:
                Thread.Sleep(30);
                if (lisMoveTube.Count > 0)
                {
                    MoveTubeManualReset.WaitOne();//y add 20170724
                    MoveTubeStatus TempMoveStatus = lisMoveTube[0];
                    tempMoveTubeStatus = TempMoveStatus;//y add 20180814
                    if (TempMoveStatus == null)
                    {
                        goto again;
                    }
                    string[] takepos = TempMoveStatus.TakeTubePos.Split('-');
                    string[] putpos = TempMoveStatus.putTubePos.Split('-');
                    TestSchedule ts = lisTestSchedule.Find(ty => ty.TestID == TempMoveStatus.TestId && ty.stepNum == TempMoveStatus.StepNum);
                    string step = "";
                    if (ts != null)
                        step = ts.TestScheduleStep.ToString();

                    if (takepos[0] == "0" && MoveTubeUseFlag == false)//管架取管
                    {
                        if (putpos[0] == "1")
                        {
                            #region 发送指令及配置文件的实时更改（暂存盘夹新管到反应盘）
                            if (TubeStop)
                            {
                                if ((int.Parse(putpos[1]) != 1 || int.Parse(putpos[1]) != 2 || int.Parse(putpos[1]) != 3) && !AddTubeStop.Contains(int.Parse(putpos[1])))
                                    lock (AddTubeStop)
                                    {
                                        AddTubeStop.Add(int.Parse(putpos[1]));
                                    }
                                lisMoveTube.Remove(TempMoveStatus);
                                continue;
                            }
                            MoveTubeUseFlag = true;
                            int iNeedCool = 0;
                            int IsKnockedCool = 0;
                            #region 屏蔽
                            /*
                            //移管手到取管位置takepos[1]（管架）取管
                            takepos[1] = OperateIniFile.ReadIniData("Tube", "TubePos", "1", iniPathSubstrateTube);
                            //takepos[1] = (int.Parse(OperateIniFile.ReadIniData("Tube", "TubePos", "1", iniPathSubstrateTube)) - 1).ToString(); //2018-09-28
                            int plate = int.Parse(takepos[1]) % 88 == 0 ? int.Parse(takepos[1]) / 88 - 1 : int.Parse(takepos[1]) / 88;//几号板
                            int column = int.Parse(takepos[1]) % 11 == 0 ? int.Parse(takepos[1]) / 11 - (plate * 8) : int.Parse(takepos[1]) / 11 + 1 - (plate * 8);
                            int hole = int.Parse(takepos[1]) % 11 == 0 ? 11 : int.Parse(takepos[1]) % 11;
                            //更改了取官成功的位置，防止发生处理出现的抓空等问题时，修改完配置文件又被此处覆盖
                            #region 取管成功
                            int TubeRack = (int.Parse(takepos[1])) / 88;
                            int curTube = (int.Parse(takepos[1])) % 88;
                            if (curTube == 0 && int.Parse(takepos[1]) != 0)
                            {
                                TubeRack = TubeRack - 1;
                                curTube = 88;
                            }
                            //那个架子减了一个管
                            OperateIniFile.WriteIniData("Tube", "Pos" + (TubeRack + 1).ToString(), (88 - curTube).ToString(), iniPathSubstrateTube);
                            List<int> lisTubeNum = new List<int>();
                            lisTubeNum = QueryTubeNum();
                            //移管手要夹的下一个管架位置
                            int NextPos = int.Parse(takepos[1]) + 1;
                            //管架中第一个装载管架的索引
                            int firstTubeIndex = lisTubeNum.FindIndex(ty => ty <= 88 && ty > 0);
                            if (firstTubeIndex < 0)
                            {
                                setmainformbutten();
                                TubeStop = true;
                                LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "异常" + " *** " + "未读" + " *** " + "管架为空，将停止加样！");
                            }
                            for (int i = 1; i <= lisTubeNum.Count; i++)
                            {
                                if (NextPos == i * 88 + 1)
                                {
                                    NextPos = firstTubeIndex * 88 + (88 - lisTubeNum[firstTubeIndex]) + 1;
                                }
                            }
                            OperateIniFile.WriteIniData("Tube", "TubePos", NextPos.ToString(), iniPathSubstrateTube);
                             */
                            AgainNewMove:
                            ///到管架takepos[1]位置取管放到温育盘putpos位置
                            //NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 01 " + plate.ToString("x2") + " " + column.ToString("x2")
                            //    + " " + hole.ToString("x2") + " " + int.Parse(putpos[1]).ToString("x2")), 1);
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 01 " + int.Parse(putpos[1]).ToString("x2")), 1);
                            if (!NetCom3.Instance.MoveQuery())
                            {
                                TubeProblemFlag = true;
                                #region 发生异常处理
                                if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull)
                                {
                                    iNeedCool++;
                                    if (iNeedCool < 3)
                                        goto AgainNewMove;
                                    else
                                    {
                                        TubeStop = true;
                                        if ((int.Parse(putpos[1]) != 1 || int.Parse(putpos[1]) != 2 || int.Parse(putpos[1]) != 3) && !AddTubeStop.Contains(int.Parse(putpos[1])))
                                        {
                                            lock (AddTubeStop)
                                            {
                                                AddTubeStop.Add(int.Parse(putpos[1]));
                                            }
                                        }
                                        dbtnRackStatus();
                                        setmainformbutten();
                                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在暂存盘向温育盘抓管时多次抓空!实验暂停加样！");
                                        TubeProblemFlag = false;//2019-08-24 ZLX add
                                    }
                                }
                                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.LackTube)
                                {
                                    TubeStop = true;
                                    if ((int.Parse(putpos[1]) != 1 || int.Parse(putpos[1]) != 2 || int.Parse(putpos[1]) != 3) && !AddTubeStop.Contains(int.Parse(putpos[1])))
                                    {
                                        lock (AddTubeStop)
                                        {
                                            AddTubeStop.Add(int.Parse(putpos[1]));
                                        }
                                    }
                                    dbtnRackStatus();
                                    setmainformbutten();
                                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "理杯机缺管实验暂停加样！");
                                    TubeProblemFlag = false;//2019-08-24 ZLX add
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
                                        setmainformbutten();
                                        NetCom3.Instance.stopsendFlag = true;
                                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在暂存盘向温育盘抓管时取管撞管！");
                                        DialogResult tempresult = MessageBox.Show("移管手在暂存盘向温育盘抓管时发生撞管，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                        AllStop();
                                    }
                                }
                                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.putKnocked)
                                {
                                    GAgainNewMove:
                                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 " + int.Parse(putpos[1]).ToString("x2")), 1);
                                    if (!NetCom3.Instance.MoveQuery())
                                    {
                                        #region 异常处理
                                        if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked)
                                        {
                                            IsKnockedCool++;
                                            if (IsKnockedCool < 2)
                                            {
                                                goto GAgainNewMove;
                                            }
                                            else
                                            {
                                                setmainformbutten();
                                                NetCom3.Instance.stopsendFlag = true;
                                                LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在温育盘扔废管时取管撞管！");
                                                DialogResult tempresult = MessageBox.Show("移管手扔废管时发生撞管！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                                AllStop();
                                            }
                                        }
                                        #endregion
                                    }
                                    IsKnockedCool++;
                                    if (IsKnockedCool < 2)
                                    {
                                        goto AgainNewMove;
                                    }
                                    else
                                    {
                                        setmainformbutten();
                                        NetCom3.Instance.stopsendFlag = true;
                                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手向温育盘抓新管时放管撞管！");
                                        DialogResult tempresult = MessageBox.Show("移管手向温育盘抓新管时发生撞管，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                        AllStop();
                                    }
                                }
                                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.OverTime)
                                {
                                    setmainformbutten();
                                    NetCom3.Instance.stopsendFlag = true;
                                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在暂存盘向温育盘抓管时接收数据超时！");
                                    DialogResult tempresult = MessageBox.Show("移管手在暂存盘向温育盘抓管时接收数据超时，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                    AllStop();
                                }
                                #endregion
                            }
                            else
                            {
                                TubeProblemFlag = false;
                                OperateIniFile.WriteIniData("ReactTrayInfo", "no" + putpos[1], "1", iniPathReactTrayInfo);
                            }
                            #endregion
                            //NetCom3.Instance.MoveQuery();
                            #endregion
                            lock (locker2)
                            {
                                lisMoveTube.Remove(TempMoveStatus);
                            }
                            MoveTubeUseFlag = false;
                        }

                    }
                    //清洗盘夹管
                    else if (takepos[0] == "2" && WashTrayUseFlag == false)
                    {
                        if (putpos[0] == "1" && MoveTubeUseFlag == false)
                        {
                            #region 指令发送及相关文件修改（清洗盘夹管到反应盘）
                            MoveTubeUseFlag = true;
                            WashTrayUseFlag = true;
                            int iNeedCool = 0;
                            int IsKnockedCool = 0;
                            AgainNewMove:
                            //NetCom3.IncubateTray.WaitOne(15000);//add y 20180524
                            //LogFile.Instance.Write(string.Format("{0}<-:{1}", DateTime.Now.ToString("HH:mm:ss"), "清洗盘夹管到温育盘 movetube  WashTrayUseFlag = true"));
                            ///清洗盘取管放到温育盘putpos[1]位置
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 03 " + int.Parse(putpos[1]).ToString("x2") + " 01"), 1);
                            if (!NetCom3.Instance.MoveQuery())
                            {
                                #region 发生异常处理
                                if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull)
                                {
                                    iNeedCool++;
                                    if (iNeedCool < 2)
                                    {
                                        AgainReSetSend:
                                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 08"), 1);
                                        if (!NetCom3.Instance.MoveQuery())
                                        {
                                            if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.Sendfailure)
                                            {
                                                LogFile.Instance.Write(DateTime.Now + "清洗盘复位返回指令重新发送！");
                                                goto AgainReSetSend;
                                            }
                                            else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.OverTime)
                                            {
                                                NetCom3.Instance.stopsendFlag = true;
                                                ShowWarnInfo("清洗盘复位返回指令接收超时", "清洗", 1);
                                                AllStop();
                                            }
                                        }
                                        goto AgainNewMove;
                                    }
                                    else
                                    {
                                        setmainformbutten();
                                        NetCom3.Instance.stopsendFlag = true;
                                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘夹管到温育盘时多次抓空！");
                                        DialogResult tempresult = MessageBox.Show("移管手在清洗盘夹管到温育盘时多次抓空，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                        AllStop();
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
                                        setmainformbutten();
                                        NetCom3.Instance.stopsendFlag = true;
                                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘夹管到温育盘时取管撞管！");
                                        DialogResult tempresult = MessageBox.Show("移管手在清洗盘夹管到温育盘时发生撞管，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                        AllStop();
                                    }
                                }
                                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.putKnocked)
                                {

                                    int CIsKnockedCool = 0;
                                    #region 清洗盘向温育盘放管时发生撞管
                                    OperateIniFile.WriteIniData("TubePosition", "no" + takepos[1], "0", iniPathWashTrayInfo);
                                    LogFile.Instance.Write("==============清洗盘夹管  " + washCountNum);
                                    if (dtWashTrayTubeStatus.Rows.Count > 0)
                                    {
                                        dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][1] = "0";
                                        dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][2] = 0;
                                        dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][3] = 0;
                                        dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][4] = 0;
                                    }

                                    #endregion

                                    ClearMove:
                                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 " + int.Parse(putpos[1]).ToString("x2")), 1);
                                    if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked)
                                    {
                                        CIsKnockedCool++;
                                        if (CIsKnockedCool < 2)
                                            goto ClearMove;
                                        else
                                        {
                                            setmainformbutten();
                                            NetCom3.Instance.stopsendFlag = true;
                                            LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在温育盘扔废管时取管撞管！撞管位置为：" + tubeHoleNum);
                                            //LogFileAlarm.Instance.Write(" *** " + "时间" + DateTime.Now.ToString("HH-mm-ss") + "移管手在温育盘扔废管时发生撞管孔位置" + tubeHoleNum + " *** ");
                                            DialogResult tempresult = MessageBox.Show("移管手在温育盘扔废管时发生撞管，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                            AllStop();
                                        }
                                    }
                                    else
                                    {
                                        List<TestSchedule> TestSchedule = lisTestSchedule.FindAll(ty => ty.TestID == TempMoveStatus.TestId);
                                        if (TestSchedule.Count > 0)
                                            RemoveTestList(TestSchedule[0] , "移管手在清洗盘夹管向温育盘时放管撞管");
                                        TempMoveStatus.isRetransmit = false;
                                        lock (locker2)
                                        {
                                            ///取放管失败
                                            lisMoveTube.Remove(TempMoveStatus);
                                        }
                                        OperateIniFile.WriteIniData("ReactTrayInfo", "no" + putpos[1], "0", iniPathReactTrayInfo);
                                    }
                                }
                                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.OverTime)
                                {
                                    setmainformbutten();
                                    NetCom3.Instance.stopsendFlag = true;
                                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘夹管到温育盘时接收数据超时！");
                                    DialogResult tempresult = MessageBox.Show("移管手在清洗盘夹管到温育盘时接收数据超时，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                    AllStop();
                                }
                                #endregion
                            }
                            else
                            {
                                #region 取放管成功
                                OperateIniFile.WriteIniData("TubePosition", "no" + takepos[1], "0", iniPathWashTrayInfo);
                                LogFile.Instance.Write("==============清洗盘夹管  " + washCountNum);
                                //LogFile.Instance.Write("==============夹管到温育盘  " + takepos[1] + "  扔管");
                                //if (!CatchVacancyThisTime)
                                //{
                                if (dtWashTrayTubeStatus.Rows.Count > 0)
                                {
                                    dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][1] = "0";
                                    dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][2] = 0;
                                    dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][3] = 0;
                                    dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][4] = 0;
                                }
                                //}
                                //CatchVacancyThisTime = false;
                                OperateIniFile.WriteIniData("ReactTrayInfo", "no" + putpos[1], "2", iniPathReactTrayInfo);
                                #endregion
                            }
                            //WashTurnFlag = false;//2018-10-24 zlx mod
                            ///取放管失败
                            if (!TempMoveStatus.isRetransmit)
                            {
                                W1EndTime.RemoveAt(0);
                            }
                            DalayFlag = false;
                            #endregion
                            lock (locker2)
                            {
                                lisMoveTube.Remove(TempMoveStatus);
                            }
                            WashTrayUseFlag = false;
                            MoveTubeUseFlag = false;
                        }
                        //到废弃处
                        else if (putpos[0] == "0" && MoveTubeUseFlag == false && WashTrayUseFlag == false)
                        {
                            #region 指令发送及相关配置文件更新（清洗盘扔废管）
                            MoveTubeUseFlag = true;
                            WashTrayUseFlag = true;
                            //WashTurnFlag = true;
                            int iNeedCool = 0;
                            int IsKnockedCool = 0;
                            AgainNewMove:
                            //LogFile.Instance.Write(string.Format("{0}<-:{1}", DateTime.Now.ToString("HH:mm:ss"), "到废弃处 movetube  WashTrayUseFlag = true"));
                            ///清洗盘takepos[1]取管扔废管
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 01"), 1);
                            //LogFile.Instance.Write("  ***  " + "当前时间" + DateTime.Now + "清洗盘takepos[1]取管扔废管取放管位置" + tubeHoleNum + "  ***  ");
                            if (!NetCom3.Instance.MoveQuery())
                            {
                                #region 发生异常处理
                                if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull)
                                {
                                    iNeedCool++;
                                    if (iNeedCool < 2)
                                    {
                                        AgainReSetSend:
                                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 08"), 1);
                                        if (!NetCom3.Instance.MoveQuery())
                                        {
                                            if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.Sendfailure)
                                            {
                                                LogFile.Instance.Write(DateTime.Now + "清洗盘复位返回指令重新发送！");
                                                goto AgainReSetSend;
                                            }
                                            else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.OverTime)
                                            {
                                                NetCom3.Instance.stopsendFlag = true;
                                                ShowWarnInfo("清洗盘复位返回指令接收超时", "清洗", 1);
                                                AllStop();
                                            }
                                        }
                                        goto AgainNewMove;
                                    }
                                    else
                                    {
                                        setmainformbutten();
                                        NetCom3.Instance.stopsendFlag = true;
                                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘扔废管时多次抓空！抓空位置为：" + tubeHoleNum);
                                        //LogFileAlarm.Instance.Write(" *** " + "时间" + DateTime.Now.ToString("HH-mm-ss") + "清洗盘扔废管时多次抓空孔位置" + tubeHoleNum + " *** ");
                                        DialogResult tempresult = MessageBox.Show("移管手在清洗盘扔废管时多次抓空，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                        AllStop();
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
                                        setmainformbutten();
                                        NetCom3.Instance.stopsendFlag = true;
                                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘扔废管时取管撞管！撞管位置为：" + tubeHoleNum);
                                        //LogFileAlarm.Instance.Write(" *** " + "时间" + DateTime.Now.ToString("HH-mm-ss") + "移管手在清洗盘扔废管时发生撞管孔位置" + tubeHoleNum + " *** ");
                                        DialogResult tempresult = MessageBox.Show("移管手在清洗盘扔废管时发生撞管，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                        AllStop();
                                    }
                                }
                                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.OverTime)
                                {
                                    setmainformbutten();
                                    NetCom3.Instance.stopsendFlag = true;
                                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘扔废管时接收数据超时！");
                                    DialogResult tempresult = MessageBox.Show("移管手在清洗盘扔废管时接收数据超时，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                    AllStop();
                                }
                                #endregion
                            }
                            else
                            {
                                #region 取管成功
                                OperateIniFile.WriteIniData("TubePosition", "no" + takepos[1], "0", iniPathWashTrayInfo);
                                LogFile.Instance.Write("==============  " + washCountNum + "  扔管");
                                //LogFile.Instance.Write("==============  " + currentHoleNum + "  扔管");
                                //LogFile.Instance.Write("==============  " + takepos[1] + "  扔管");
                                //if (!CatchVacancyThisTime)
                                //{
                                if (dtWashTrayTubeStatus.Rows.Count > 0)
                                {
                                    dtWashTrayTubeStatus.Rows[28][1] = "0";
                                    dtWashTrayTubeStatus.Rows[28][2] = 0;
                                    dtWashTrayTubeStatus.Rows[28][3] = 0;
                                    dtWashTrayTubeStatus.Rows[28][4] = 0;
                                }
                                //}
                                //CatchVacancyThisTime = false;
                                #endregion
                                lock (locker2)
                                {
                                    lisMoveTube.Remove(TempMoveStatus);
                                }
                            }
                            #endregion
                            //WashTurnFlag = false;//2018-11-28 zlx mod
                            MoveTubeUseFlag = false;
                            WashTrayUseFlag = false;
                        }

                    }
                    //反应盘扔废管
                    else if (takepos[0] == "1" && putpos[0] == "0" && MoveTubeUseFlag == false)
                    {
                        #region 指令发送及文件修改（反应盘扔废管）
                        MoveTubeUseFlag = true;
                        int iNeedCool = 0;
                        int IsKnockedCool = 0;
                        AgainNewMove:
                        //NetCom3.IncubateTray.WaitOne(15000);//add y 20180524
                        ///温育盘takepos[1]取管扔废管
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 " + int.Parse(takepos[1]).ToString("x2")), 1);
                        if (!NetCom3.Instance.MoveQuery())
                        {
                            #region 发生异常处理
                            if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull)
                            {
                                iNeedCool++;
                                if (iNeedCool < 2)
                                {
                                    goto AgainNewMove;
                                }
                                else
                                {
                                    setmainformbutten();
                                    //NetCom3.Instance.stopsendFlag = true;
                                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在反应盘扔废管时多次抓空！");
                                    //DialogResult tempresult = MessageBox.Show("移管手在反应盘扔废管时多次抓空，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                    //AllStop();
                                    //break;
                                    RemoveLeftTestList();
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
                                    setmainformbutten();
                                    NetCom3.Instance.stopsendFlag = true;
                                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在温育盘扔废管时取管撞管！");
                                    DialogResult tempresult = MessageBox.Show("移管手在温育盘扔废管时发生撞管，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                    AllStop();
                                }

                            }
                            else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.OverTime)
                            {
                                setmainformbutten();
                                NetCom3.Instance.stopsendFlag = true;
                                LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在温育盘扔废管时接收数据超时！");
                                DialogResult tempresult = MessageBox.Show("移管手在温育盘扔废管时接收数据超时，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                AllStop();
                                break;
                            }
                            #endregion
                        }
                        else
                        {
                            ///取管成功
                            OperateIniFile.WriteIniData("ReactTrayInfo", "no" + takepos[1], "0", iniPathReactTrayInfo);
                            if (BFullReactTray)
                                BFullReactTray = false;
                        }
                        #endregion
                        lock (locker2)
                        {
                            lisMoveTube.Remove(TempMoveStatus);
                        }
                        MoveTubeUseFlag = false;

                    }
                    //反应盘夹管到清洗盘
                    //else if (takepos[0] == "1" && dtWashTrayTubeStatus.Rows[0][1].ToString() == "0" && putpos[0] == "2"
                    //    && WashTurnFlag == false && MoveTubeUseFlag == false && WashTrayUseFlag==false)///反应盘标志位是否不用判断
                    else if (takepos[0] == "1" && dtWashTrayTubeStatus.Rows[0][1].ToString() == "0" && putpos[0] == "2"
                        && MoveTubeUseFlag == false && WashTurnFlag == false)///反应盘标志位是否不用判断
                    {
                        NoTateFlag = true;
                        MoveTubeUseFlag = true;
                        WashTrayUseFlag = true;
                        //WashTurnFlag = true;
                        int iNeedCool = 0;
                        int IsKnockedCool = 0;
                        #region 指令发送及文件修改（反应盘夹管到清洗盘）
                        //NetCom3.IncubateTray.WaitOne(15000);//add y 20180524
                        AgainNewMove:
                        ///从反应盘takepos[1]位取管放到清洗盘putpos[1]位置
                        LogFile.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + "取管位置:" + TempMoveStatus.TakeTubePos + ",放管位置:" + TempMoveStatus.putTubePos);
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 02 " + int.Parse(takepos[1]).ToString("x2")), 1);
                        if (!NetCom3.Instance.MoveQuery())
                        {
                            LogFile.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + "EB 90 31 01 02重发，MoverrorFlag为:" + NetCom3.Instance.MoverrorFlag);
                            #region 发生异常处理
                            if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull)
                            {
                                iNeedCool++;
                                if (iNeedCool < 3)
                                {
                                    goto AgainNewMove;
                                }
                                else
                                {
                                    setmainformbutten();
                                    //NetCom3.Instance.stopsendFlag = true;
                                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在温育盘夹管到清洗盘时多次抓空！");
                                    //DialogResult tempresult = MessageBox.Show("移管手在反应盘夹管到清洗盘时多次抓空，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                    //AllStop();

                                    #region  当前实验废弃，复位后扔掉 
                                    moveNullTubeProblemFlag = true;
                                    lisMoveTube.Remove(TempMoveStatus);
                                    OperateIniFile.WriteIniData("TubePosition", "No1", "0", iniPathWashTrayInfo);
                                    dtWashTrayTubeStatus.Rows[0][1] = "0";
                                    dtWashTrayTubeStatus.Rows[0][2] = 0;
                                    dtWashTrayTubeStatus.Rows[0][3] = 0;
                                    dtWashTrayTubeStatus.Rows[0][4] = 0;
                                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 01 10 00 00"), 5);
                                    NetCom3.Instance.SingleQuery();
                                    MoveTubeListAddTubeDispose(Convert.ToInt32(int.Parse(takepos[1])));
                                    List<TestSchedule> TestSchedule = lisTestSchedule.FindAll(ty => ty.TestID == TempMoveStatus.TestId);
                                    if (TestSchedule.Count > 0)
                                        RemoveTestList(TestSchedule[0]  , "移管手在温育盘夹管向清洗盘时多次抓空");
                                    moveNullTubeProblemFlag = false;
                                    #endregion

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
                                    setmainformbutten();
                                    NetCom3.Instance.stopsendFlag = true;
                                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在温育盘夹管到清洗盘时取管撞管！撞管位置为：" + tubeHoleNum);
                                    //LogFileAlarm.Instance.Write(" *** " + "时间" + DateTime.Now.ToString("HH-mm-ss") + "移管手在反应盘夹管到清洗盘时发生撞管孔位置" + tubeHoleNum + " *** ");
                                    DialogResult tempresult = MessageBox.Show("移管手在温育盘夹管到清洗盘时发生撞管，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                    AllStop();
                                }
                            }
                            else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.putKnocked)
                            {
                                int CIsKnockedCool = 0;
                                OperateIniFile.WriteIniData("ReactTrayInfo", "no" + takepos[1], "0", iniPathReactTrayInfo);
                                ClearMove:
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
                                if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked)
                                {
                                    CIsKnockedCool++;
                                    if (CIsKnockedCool < 2)
                                        goto ClearMove;
                                    else
                                    {
                                        setmainformbutten();
                                        NetCom3.Instance.stopsendFlag = true;
                                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘扔废管时取管撞管！撞管位置为：" + tubeHoleNum);
                                        //LogFileAlarm.Instance.Write(" *** " + "时间" + DateTime.Now.ToString("HH-mm-ss") + "移管手在清洗盘扔废管时发生撞管孔位置" + tubeHoleNum + " *** ");
                                        DialogResult tempresult = MessageBox.Show("移管手在清洗盘扔废管时发生撞管，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                        AllStop();
                                    }
                                }
                                else
                                {
                                    List<TestSchedule> TestSchedule = lisTestSchedule.FindAll(ty => ty.TestID == TempMoveStatus.TestId);
                                    if (TestSchedule.Count > 0)
                                        RemoveTestList(TestSchedule[0] , "移管手在温育盘夹管到清洗盘时放管撞管");
                                    step = "";
                                    lock (locker2)
                                    {
                                        ///取放管失败
                                        lisMoveTube.Remove(TempMoveStatus);
                                    }
                                }

                            }
                            else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.OverTime)
                            {
                                setmainformbutten();
                                NetCom3.Instance.stopsendFlag = true;
                                LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在温育盘夹管到清洗盘时接收数据超时！");
                                DialogResult tempresult = MessageBox.Show("移管手在温育盘夹管到清洗盘时接收数据超时，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                AllStop();
                                break;
                            }
                            #endregion
                        }
                        else
                        {
                            lock (locker2)
                            {
                                ///取放管失败
                                lisMoveTube.Remove(TempMoveStatus);
                            }
                            #region 取放管成功
                            //LogFile.Instance.Write("==============  温育向清洗盘放管  " + currentHoleNum);
                            LogFile.Instance.Write("==============  反应盘向清洗盘放管  " + washCountNum);
                            if (step == "Wash1")//2018-06-04 两步法移管留位置 zlx add
                                OperateIniFile.WriteIniData("ReactTrayInfo", "no" + takepos[1], "9", iniPathReactTrayInfo);
                            else
                            {
                                OperateIniFile.WriteIniData("ReactTrayInfo", "no" + takepos[1], "0", iniPathReactTrayInfo);
                                if (BFullReactTray)
                                    BFullReactTray = false;
                            }
                            //OperateIniFile.WriteIniData("ReactTrayInfo", "no" + takepos[1], "0", iniPathReactTrayInfo);
                            OperateIniFile.WriteIniData("TubePosition", "No1", "1", iniPathWashTrayInfo);
                            //if (!CatchVacancyThisTime)
                            //{
                            dtWashTrayTubeStatus.Rows[0][1] = "1";
                            dtWashTrayTubeStatus.Rows[0][2] = TempMoveStatus.TestId;
                            dtWashTrayTubeStatus.Rows[0][3] = TempMoveStatus.StepNum;
                            dtWashTrayTubeStatus.Rows[0][4] = takepos[1];
                            //}
                            //CatchVacancyThisTime = false;
                            #endregion
                        }

                        //LogFile.Instance.Write(string.Format("{0}<-:{1}", DateTime.Now.ToString("HH:mm:ss"), "反应盘夹管到清洗盘 movetube  WashTrayUseFlag = false"));

                        if (step == "WashTray")
                        {

                            lisProBar[TempMoveStatus.TestId - 1].BarColor[StepIndex(dgvWorkListData.Rows[TempMoveStatus.TestId - 1].Cells[6].Value.ToString(), "W2") - 1]
                                   = Color.Gray;
                            lisProBar[TempMoveStatus.TestId - 1].Invalidate();

                            BeginInvoke(TestStatusInfo, new object[] { "正在清洗", TempMoveStatus.TestId });

                            lisProBar[TempMoveStatus.TestId - 1].BarColor[StepIndex(dgvWorkListData.Rows[TempMoveStatus.TestId - 1].Cells[6].Value.ToString(), "W2")]
                                = Color.Yellow;
                            lisProBar[TempMoveStatus.TestId - 1].Invalidate();

                        }
                        if (step == "Wash1")
                        {

                            lisProBar[TempMoveStatus.TestId - 1].BarColor[StepIndex(dgvWorkListData.Rows[TempMoveStatus.TestId - 1].Cells[6].Value.ToString(), "W1") - 1]
                                = Color.Gray;
                            lisProBar[TempMoveStatus.TestId - 1].Invalidate();

                            BeginInvoke(TestStatusInfo, new object[] { "正在清洗", TempMoveStatus.TestId });

                            lisProBar[TempMoveStatus.TestId - 1].BarColor[StepIndex(dgvWorkListData.Rows[TempMoveStatus.TestId - 1].Cells[6].Value.ToString(), "W1")]
                                = Color.Yellow;
                            lisProBar[TempMoveStatus.TestId - 1].Invalidate();
                        }
                        WashTrayUseFlag = false;
                        MoveTubeUseFlag = false;
                        //WashTurnFlag = false;
                        #endregion
                        NoTateFlag = false;
                    }
                }
            }
        }

        /// <summary>
        /// 剩余实验废弃
        /// </summary>
        /// <param name="testSchedule"></param>
        private void RemoveLeftTestList()
        {
            LogFile.Instance.Write("调用了RemoveLeftTestList(TestSchedule testSchedule)方法！");
            Thread thread;
            thread = new Thread(new ThreadStart(() => { MessageBox.Show("剩余的实验因为发生故障，无法继续，请在稍后重新运行此实验", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information); }));
            if (thread != null)
            {
                thread.IsBackground = true;
                thread.Start();
            }
            foreach (DataGridViewRow dr in dgvWorkListData.Rows)
            {
                if (int.Parse(dr.Cells[1].Value.ToString()) >= NoStartTestId)
                {
                    dr.Cells["TestStatus"].Value = "实验废弃";
                    dr.DefaultCellStyle.BackColor = Color.Gray;
                    SampleNumCurrent = SampleNumCurrent - 1;
                    if ((StopList.Count > 0) &&
                              StopList.Contains(dr.Cells["No"].Value.ToString()))
                    {
                        StopList.Remove(dr.Cells["No"].Value.ToString());
                    }
                }
            }
            List<TestSchedule> Temp = lisTestSchedule.FindAll(ty => ty.TestID >= NoStartTestId);
            foreach (var item in Temp)
            {
                item.TestScheduleStep = TestSchedule.ExperimentScheduleStep.DoNotTakeCareThis;
            }
            LoadingHelper.CloseForm();
        }

        void washTray(object TubeInfo)
        {
            timer.Interval = 20000 / (1000 / timeInterval);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timerWash_Tick);
            timer.Enabled = true;
        }
        /// <summary>
        /// 清洗盘各个实验步骤
        /// </summary>
        void washStep()
        {

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
            #region 读数
            if (dtWashTrayTubeStatus.Rows[24][1].ToString() == "1")
            {
                read = "1";
            }
            #endregion

            #region 清洗和加底物
            //判断第一次吸液位置是否有管
            if (dtWashTrayTubeStatus.Rows[4 + isNewCleanTray][1].ToString() == "1")
            {
                //if (frmMain.StopFlag[3])//2018-07-11 zlx add
                //{
                //    TestSchedule Schedule = lisTestSchedule.Find(ty => ty.TestID == int.Parse(dtWashTrayTubeStatus.Rows[4 + isNewCleanTray][2].ToString()));
                //    if (!StopList.Contains(Schedule.TestID.ToString()))
                //        StopList.Add(Schedule.TestID.ToString());
                //}
                ImbibitionFlag.Add(1);
            }
            else
            {
                ImbibitionFlag.Add(0);
            }
            //吸液步骤完成后是否将管放回反应盘
            bool takeTubeFlag = false;

            //判断第三次吸液位置是否有管
            if (dtWashTrayTubeStatus.Rows[8 + isNewCleanTray][1].ToString() == "1")
            {
                //if (frmMain.StopFlag[3])//2018-07-11 zlx add
                //{
                //    TestSchedule Schedule = lisTestSchedule.Find(ty => ty.TestID == int.Parse(dtWashTrayTubeStatus.Rows[8 + isNewCleanTray][2].ToString()));
                //    if (!StopList.Contains(Schedule.TestID.ToString()))
                //        StopList.Add(Schedule.TestID.ToString());
                //}
                ImbibitionFlag.Add(1);
            }
            else
            {
                ImbibitionFlag.Add(0);
            }

            //判断第二次吸液位置是否有管
            if (dtWashTrayTubeStatus.Rows[12 + isNewCleanTray][1].ToString() == "1")
            {
                //if (frmMain.StopFlag[3])//2018-07-11 zlx add
                //{
                //    TestSchedule Schedule = lisTestSchedule.Find(ty => ty.TestID == int.Parse(dtWashTrayTubeStatus.Rows[12 + isNewCleanTray][2].ToString()));
                //    if (!StopList.Contains(Schedule.TestID.ToString()))
                //        StopList.Add(Schedule.TestID.ToString());
                //}
                ImbibitionFlag.Add(1);
              
            }
            else
            {
                ImbibitionFlag.Add(0);
            }
            //判断第四次吸液位置是否有管
            if (dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][1].ToString() == "1")
            {
                string CurrentStep = lisTestSchedule.Find(ty => ty.stepNum == int.Parse(dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][3].ToString())
                                                         && ty.TestID == int.Parse(dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][2].ToString()))
                                                         .TestScheduleStep.ToString();
                ImbibitionFlag.Add(1);
                if (CurrentStep == BioBaseCLIA.Run.TestSchedule.ExperimentScheduleStep.Wash1.ToString())
                {
                    takeTubeFlag = true;
                }
            }
            else
            {
                ImbibitionFlag.Add(0);
            }
            //判断第一次注液位置是否有管
            if (dtWashTrayTubeStatus.Rows[5 + isNewCleanTray][1].ToString() == "1")
            {
                //2018-07-10 zlx add
                //if (frmMain.StopFlag[0])
                //{
                //    TestSchedule Schedule = lisTestSchedule.Find(ty => ty.TestID == int.Parse(dtWashTrayTubeStatus.Rows[5 + isNewCleanTray][2].ToString()));
                //    if (!StopList.Contains(Schedule.TestID.ToString()))
                //        StopList.Add(Schedule.TestID.ToString());
                //}
                LiquidInjectionFlag.Add("1");
            }
            else
            {
                LiquidInjectionFlag.Add("0");
            }
            //判断第二次注液位置是否有管
            if (dtWashTrayTubeStatus.Rows[9 + isNewCleanTray][1].ToString() == "1")
            {
                //2018-07-10 zlx add 
                //if (frmMain.StopFlag[0])
                //{
                //    TestSchedule Schedule = lisTestSchedule.Find(ty => ty.TestID == int.Parse(dtWashTrayTubeStatus.Rows[9 + isNewCleanTray][2].ToString()));
                //    if (!StopList.Contains(Schedule.TestID.ToString()))
                //        StopList.Add(Schedule.TestID.ToString());
                //}
                LiquidInjectionFlag.Add("1");
            }
            else
            {
                LiquidInjectionFlag.Add("0");
            }
            //判断第三次注液位置是否有管
            if (dtWashTrayTubeStatus.Rows[13 + isNewCleanTray][1].ToString() == "1")
            {
                //2018-07-10 zlx add
                //if (frmMain.StopFlag[0])
                //{
                //    TestSchedule Schedule = lisTestSchedule.Find(ty => ty.TestID == int.Parse(dtWashTrayTubeStatus.Rows[13 + isNewCleanTray][2].ToString()));
                //    if (!StopList.Contains(Schedule.TestID.ToString()))
                //        StopList.Add(Schedule.TestID.ToString());
                //}
                LiquidInjectionFlag.Add("1");
            }
            else
            {
                LiquidInjectionFlag.Add("0");
            }
            //加底物位置是否有管
            if (dtWashTrayTubeStatus.Rows[18 + isNewCleanTray][1].ToString() == "1")
            {
                AddSubstrate = "1";
            }
            //吸液、注液或者加底物的位置下面有反应管
            if (ImbibitionFlag.Contains(1) || LiquidInjectionFlag.Contains("1") || AddSubstrate == "1" || read == "1")
            {
                if (ImbibitionFlag.Contains(1))
                {
                    Imbibition = "01";
                }
                if (AddSubstrate == "1")
                {
                    //2018-10-17 zlx mod
                    /*
                   string LeftCount1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "", iniPathSubstrateTube);
                   string LeftCount2 = OperateIniFile.ReadIniData("Substrate2", "LeftCount", "", iniPathSubstrateTube);
                   if (int.Parse(LeftCount1)>0)//substrateNum1
                   {
                       if (int.Parse(LeftCount1) >int.Parse(LeftCount2)&& int.Parse(LeftCount2)>0)
                           substratePipe = "2";
                       else
                           substratePipe = "1";
                   }
                   else
                   {
                       substratePipe = "2";
                   }
                    */
                    substratePipe = "1";
                }
                #region 指令发送
                if (read == "1")
                {
                    lock (listestid)
                    {
                        listestid.Enqueue(int.Parse(dtWashTrayTubeStatus.Rows[24][2].ToString()));
                    }
                    ReadThread = new Thread(new ParameterizedThreadStart(Read));
                    ReadThread.IsBackground = true;
                    ReadThread.Start();
                }
                AgainSend:
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 " + Imbibition + " " + LiquidInjectionFlag[0] +
                    LiquidInjectionFlag[1] + " " + LiquidInjectionFlag[2] + AddSubstrate + " " + substratePipe + read), 2);
                if (!NetCom3.Instance.WashQuery())
                {
                    if (NetCom3.Instance.WasherrorFlag == (int)ErrorState.Sendfailure)
                        goto AgainSend;
                    else if (NetCom3.Instance.WasherrorFlag == (int)ErrorState.OverTime)
                    {
                        NetCom3.Instance.stopsendFlag = true;
                        ShowWarnInfo("清洗盘清洗灌注指令接收超时", "清洗", 1);
                        //LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘夹管到温育盘时发生撞管！");
                        //MessageBox.Show("指令接收超时，实验已终止", "清洗指令错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //addLiquiding = false;
                        AllStop();
                    }
                }
                #endregion
            }
            #region 指令完成后界面信息改变
            if (read == "1")
            {
                //ReadThread = new Thread(new ParameterizedThreadStart(Read));
                //ReadThread.IsBackground = true;
                //ReadThread.Start();
                if (dgvWorkListData.RowCount == 0) return;
                lisProBar[int.Parse(dtWashTrayTubeStatus.Rows[24][2].ToString()) - 1].
                BarColor[StepIndex(dgvWorkListData.Rows[int.Parse(dtWashTrayTubeStatus.Rows[24][2].ToString()) - 1].
                Cells[6].Value.ToString(), "R")]
               = Color.Gray;
                lisProBar[int.Parse(dtWashTrayTubeStatus.Rows[24][2].ToString()) - 1].Invalidate();
                BeginInvoke(TestStatusInfo, new object[] { "实验完成", int.Parse(dtWashTrayTubeStatus.Rows[24][2].ToString()) });
                //completeTestNums++;
            }
            if (ImbibitionFlag.Contains(1))
            {
                if (ImbibitionFlag[3] == 1 && takeTubeFlag == false)
                {
                    if (dgvWorkListData.RowCount == 0) return;
                    lisProBar[int.Parse(dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][2].ToString()) - 1].BarColor
                            [StepIndex(dgvWorkListData.Rows[int.Parse(dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][2].ToString()) - 1]
                            .Cells[6].Value.ToString(), "W2")] = Color.Gray;
                    lisProBar[int.Parse(dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][2].ToString()) - 1].Invalidate();
                    BeginInvoke(TestStatusInfo, new object[] { "正在加底物", int.Parse(dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][2].ToString()) });
                    lisProBar[int.Parse(dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][2].ToString()) - 1].
                        BarColor[StepIndex(dgvWorkListData.Rows[int.Parse(dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][2].ToString()) - 1].
                        Cells[6].Value.ToString(), "Su")] = Color.Yellow;
                    lisProBar[int.Parse(dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][2].ToString()) - 1].Invalidate();

                }
                if (takeTubeFlag)
                {
                    if (dgvWorkListData.RowCount == 0) return;
                    lisProBar[int.Parse(dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][2].ToString()) - 1].BarColor
                        [StepIndex(dgvWorkListData.Rows[int.Parse(dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][2].ToString()) - 1]
                        .Cells[6].Value.ToString(), "W1")] = Color.Gray;
                    lisProBar[int.Parse(dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][2].ToString()) - 1].Invalidate();
                    int pos = (int.Parse(dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][4].ToString())) % ReactTrayHoleNum;
                    MoveTubeStatus moveTube7 = new MoveTubeStatus();
                    if (pos == 0)
                    {
                        moveTube7.putTubePos = "1-" + ReactTrayHoleNum.ToString();
                    }
                    else
                    {
                        moveTube7.putTubePos = "1-" + pos.ToString();
                    }
                    moveTube7.StepNum = int.Parse(dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][3].ToString());
                    moveTube7.TakeTubePos = "2-" + dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][0].ToString().Substring(2);
                    moveTube7.TestId = int.Parse(dtWashTrayTubeStatus.Rows[16 + isNewCleanTray][2].ToString());
                    lock (locker2)
                    {
                        if (lisMoveTube.Count > 0)
                        {
                            lisMoveTube.Insert(0, moveTube7);
                        }
                        else
                        {
                            lisMoveTube.Add(moveTube7);
                        }
                    }
                }
            }

            if (AddSubstrate == "1")
            {
                string LeftCount1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "", iniPathSubstrateTube);
                OperateIniFile.WriteIniData("Substrate1", "LeftCount", (int.Parse(LeftCount1) - 1).ToString(), iniPathSubstrateTube);
                /*
                string LeftCount2 = OperateIniFile.ReadIniData("Substrate2", "LeftCount", "", iniPathSubstrateTube);
                if (int.Parse(substratePipe) == 1)//2018-10-17 zlx mod
                {
                    OperateIniFile.WriteIniData("Substrate1", "LeftCount", (int.Parse(LeftCount1)-1).ToString(), iniPathSubstrateTube);
                    substrateNum1 = int.Parse(LeftCount1) - 1;
                    //OperateIniFile.WriteIniData("Substrate1", "LeftCount", (substrateNum1 - 1).ToString(), iniPathSubstrateTube);
                    //substrateNum1--;
                }
                else
                {
                    OperateIniFile.WriteIniData("Substrate2", "LeftCount", (int.Parse(LeftCount2) - 1).ToString(), iniPathSubstrateTube);
                    substrateNum2 = int.Parse(LeftCount2) - 1;
                    //OperateIniFile.WriteIniData("Substrate2", "LeftCount", (substrateNum2 - 1).ToString(), iniPathSubstrateTube);
                    //substrateNum2--;
                }
                 */
                //2018-10-17 zlx add
                if (dgvWorkListData.RowCount == 0) return;
                dgvWorkListData.Rows[int.Parse(dtWashTrayTubeStatus.Rows[18 + isNewCleanTray][2].ToString()) - 1].Cells["SubstratePipe"].Value = substratePipe;
                lisProBar[int.Parse(dtWashTrayTubeStatus.Rows[18 + isNewCleanTray][2].ToString()) - 1].
                    BarColor[StepIndex(dgvWorkListData.Rows[int.Parse(dtWashTrayTubeStatus.Rows[18 + isNewCleanTray][2].ToString()) - 1].
                    Cells[6].Value.ToString(), "Su")] = Color.Gray;
                lisProBar[int.Parse(dtWashTrayTubeStatus.Rows[18 + isNewCleanTray][2].ToString()) - 1].Invalidate();
                BeginInvoke(TestStatusInfo, new object[] { "正在读数", int.Parse(dtWashTrayTubeStatus.Rows[18 + isNewCleanTray][2].ToString()) });
                lisProBar[int.Parse(dtWashTrayTubeStatus.Rows[18 + isNewCleanTray][2].ToString()) - 1].
                    BarColor[StepIndex(dgvWorkListData.Rows[int.Parse(dtWashTrayTubeStatus.Rows[18 + isNewCleanTray][2].ToString()) - 1].
                    Cells[6].Value.ToString(), "R")]
                    = Color.Yellow;
                lisProBar[int.Parse(dtWashTrayTubeStatus.Rows[18 + isNewCleanTray][2].ToString()) - 1].Invalidate();
                completeTestNums++;
            }
            #endregion
            #region 读数完成将管取出
            if (dtWashTrayTubeStatus.Rows[28][1].ToString() == "1")
            {
                moveTube.putTubePos = "0-0";
                moveTube.StepNum = int.Parse(dtWashTrayTubeStatus.Rows[28][3].ToString());
                moveTube.TakeTubePos = "2-" + dtWashTrayTubeStatus.Rows[28][0].ToString().Substring(2);
                moveTube.TestId = int.Parse(dtWashTrayTubeStatus.Rows[28][2].ToString());
                lock (locker2)
                {
                    if (lisMoveTube.Count > 0)
                    {
                        lisMoveTube.Insert(0, moveTube);
                    }
                    else
                    {
                        lisMoveTube.Add(moveTube);
                    }
                }
                moveTube = new MoveTubeStatus();
            }
            #endregion
            #endregion

        }
        /// <summary>
        /// 正在计算标志位
        /// </summary>
        bool calNowFlag = false;
        bool ValueFlag = false;
        Queue<int> listestid = new Queue<int>();
        object dataLocker = new object();
        object readLocker = new object();
        void Read(object readInfo)
        {
            //while (dataRecive[0] == null || dataRecive[3] != "A3" || dataRecive[15] == "00" )
            //{
            //    Thread.Sleep(5);
            //}
            //string readData = dataRecive[12] + dataRecive[13] + dataRecive[14] + dataRecive[15];
            //lock (dataLocker)
            //{
            //    dataRecive[0] = null;
            //}
           string readData;
           lock (readLocker)
           {
               while (DataReciveNumberRead.Count<1)
               {
                   Thread.Sleep(30);
               }
               lock (DataReciveNumberRead)
               {
                   readData = DataReciveNumberRead.Dequeue();
               }
           }
           //注释掉这部分，会导致结果显示不全
           //while (calNowFlag && RunFlag == (int)RunFlagStart.IsRuning)
           //{
           //    Thread.Sleep(30);
           //}
           calNowFlag = true;
          
           int testid;
           lock (readLocker)
           {
               while (listestid.Count<1)
               {
                   Thread.Sleep(30);
               }
               lock (listestid)
               {
                   testid = listestid.Dequeue();
               }
           }
           double PMT = double.Parse(System.Convert.ToInt32("0x" + readData, 16).ToString());
           PMT = GetPMT(PMT);
           int pmt = (int)PMT;
           LogFile.Instance.Write(string.Format("{0}<-:{1}", DateTime.Now.ToString("HH:mm:ss:fff"), "实验" + testid + "读数发光值为:" + pmt));
           CalculatResult(testid, pmt);
            /*
            int testid;
            lock (readLocker)
            {
                while (listestid.Count < 1)
                {
                    Thread.Sleep(5);
                }
                lock (listestid)
                {
                    testid = listestid.Dequeue();
                }
            }
            //int[] PMTS = { 1000, 4728, 4828, 4928,  110760, 120760, 110790,247240, 248240, 257240, 737016, 738016, 727016, 1744336, 1754336, 1755336, 5844848, 5854848, 5824848, 10824848, 10524848, 10864848, 4728, 110760, 247240, 737016, 1754336, 5844848, 4728, 110760, 247240, 737016, 1754336, 5844848, 4728, 110760, 247240, 737016, 1754336, 5844848, 4728, 110760, 247240, 737016, 1754336, 5844848, 4728, 110760, 247240, 737016, 1754336, 5844848 };
            int[] PMTS = {110760, 120760, 110790, 247240, 248240, 257240, 737016, 738016, 727016, 1744336, 1754336, 1755336, 5844848, 5854848, 5824848, 10824848, 10524848, 10864848, 4728, 110760, 247240, 737016, 1754336, 5844848, 4728, 110760, 247240, 737016, 1754336, 5844848, 4728, 110760, 247240, 737016, 1754336, 5844848, 4728, 110760, 247240, 737016, 1754336, 5844848, 4728, 110760, 247240, 737016, 1754336, 5844848 };
            //int[] PMTS = { 12192792,12192792, 11413104, 6525264, 6129800, 5071456, 4804736, 2964480, 3239216, 2481136, 2281352, 1829592, 1691824 };
            // if (PMTS[testid] > Math.Pow(10, 5))
            //    PMTS[testid] =(int)GetPMT(PMTS[testid]);
            ////CalculatResult(testid, 110760);
            //if (testid < 5)
            //    CalculatResult(testid, 108);
            //else if (testid < 10)
            //    CalculatResult(testid, 908990);
            //else if (testid < 15)
            //    CalculatResult(testid, 2908990);
            //else if (testid < 20)
            //    CalculatResult(testid, 6908990);
            //else
            //    CalculatResult(testid, 1107602999);
            CalculatResult(testid, PMTS[testid]);
             */
        }
        /// <summary>
        /// 反应管完成结果计算
        /// </summary>
        /// <param name="testid">测试序号</param>
        /// <param name="pmt">光子值</param>
        void CalculatResult(int testid, int pmt)
        {
            #region 当前项目的所有样本完成时更新样本状态
            TestItem ti = new TestItem();
            ti.ItemName = dgvWorkListData.Rows[testid - 1].Cells["ItemName"].Value.ToString();
            ti.RegentBatch = dgvWorkListData.Rows[testid - 1].Cells["RegentBatch"].Value.ToString();
            ti.SampleID = int.Parse(dgvWorkListData.Rows[testid - 1].Cells["SampleID"].Value.ToString());
            ti.SampleNo = dgvWorkListData.Rows[testid - 1].Cells["SampleNo"].Value.ToString();
            ti.SamplePos = int.Parse(dgvWorkListData.Rows[testid - 1].Cells["Position"].Value.ToString());
            ti.SampleType = dgvWorkListData.Rows[testid - 1].Cells["SampleType"].Value.ToString();
            ti.Schedule = dgvWorkListData.Rows[testid - 1].Cells["Schedule"].Value.ToString();
            ti.TestID = int.Parse(dgvWorkListData.Rows[testid - 1].Cells["No"].Value.ToString());
            ti.TestStatus = dgvWorkListData.Rows[testid - 1].Cells["TestStatus"].Value.ToString();
            ti.TestTime = dgvWorkListData.Rows[testid - 1].Cells["TestTime"].Value.ToString();
            lisTiEnd.Add(ti);
            //已经结束的反应管列表中查询当前testid的
            List<TestItem> lis1 = lisTiEnd.FindAll(tx => tx.SamplePos ==
                int.Parse(dgvWorkListData.Rows[testid - 1].Cells["Position"].Value.ToString()));
            List<TestItem> BToListTi = new List<TestItem>((BindingList<TestItem>)this.dgvWorkListData.DataSource);
            //总共的反应管查询当前testid的
            List<TestItem> lis2 = BToListTi.FindAll(tx => tx.SamplePos ==
                int.Parse(dgvWorkListData.Rows[testid - 1].Cells["Position"].Value.ToString()));
            DbHelperOleDb db = new DbHelperOleDb(1);
            if (lis1.Count == lis2.Count)
            {
                db = new DbHelperOleDb(1);
                DbHelperOleDb.ExecuteSql(1,@"update tbSampleInfo set Status = 1 where SampleID = " + int.Parse
                           (dgvWorkListData.Rows[testid - 1].Cells["SampleID"].Value.ToString()));
                for (int i = 0; i < dtSpInfo.Rows.Count; i++)
                {
                    try
                    {
                        if (int.Parse(dtSpInfo.Rows[i]["Position"].ToString())
                        == int.Parse(dgvWorkListData.Rows[testid - 1].Cells["Position"].Value.ToString()))
                        {
                            dtSpInfo.Rows[i]["Status"] = "1";
                        }
                    }
                    catch (Exception ex)
                    {
                        LogFile.Instance.Write("记录添加样本操作相同数据时出现的异常信息，暂时不需要进行处理" + ex.Message + "\n" + ex.StackTrace);
                    }
                }
            }

            #endregion

            #region 初始化变量
            result = "";
            SampleNumCurrent--;
            int NumResult = 0;//实验结果精度 2018-11-05 zlx add
            string Unit = "";//实验结果单位 2018-11-10 zlx add
            //获取当前读数完成的反应管的项目名称
            string ItemName = dgvWorkListData.Rows[testid - 1].Cells["ItemName"].Value.ToString();
            //获取当前读数完成的反应管使用试剂的批号
            string Batch = dgvWorkListData.Rows[testid - 1].Cells["RegentBatch"].Value.ToString();
            //获取当前反应管样本的类型
            string sampleType = dgvWorkListData.Rows[testid - 1].Cells["SampleType"].Value.ToString();
            db = new DbHelperOleDb(0);
            //查询当前项目范围
            //2018-08-03 zlx mod
            DataTable tbtbProject = DbHelperOleDb.Query(0,@"select ValueUnit,RangeType,ExpiryDate,VRangeType,ValueRange1,ValueRange2 from tbProject where ShortName = '" + ItemName + "'").Tables[0];
            string Range1 = "";
            string Range2 = "";
            double MaxValue = 0;
            double MinValue = 0;
            int ExpiryDate = 0;
            string VRangeType = "";
            foreach (DataRow dr in tbtbProject.Rows)
            {
                if (dr != null)
                {
                    Range1 = dr["ValueRange1"].ToString();
                    Range2 = dr["ValueRange2"].ToString();
                    ExpiryDate = Convert.ToInt32(dr["ExpiryDate"]);
                    //2018-11-24 zlx mod
                    if (dr["RangeType"].ToString()!="")
                      NumResult = int.Parse(dr["RangeType"].ToString());
                    else
                      NumResult = 0;
                    Unit = dr["ValueUnit"].ToString();
                    VRangeType = dr["VRangeType"].ToString();
                }
            }
            //object ob = DbHelperOleDb.GetSingle(@"select ValueRange1 from tbProject where ShortName = '" + ItemName + "'");
            //string Range1 = ob == null ? "" : ob.ToString();
            //ob = DbHelperOleDb.GetSingle(@"select ValueRange2 from tbProject where ShortName = '" + ItemName + "'");
            //string Range2 = ob == null ? "" : ob.ToString();
            //当前反应管使用过的项目定标信息
            ScalingInfo CurrentScal = lisScalingInfo.Find(ty => ty.ItemName == ItemName && ty.RegenBatch == Batch);
            //2018-08-17  zlx add
            int ScalingState = 0;
            if (CurrentScal.testType == 0)//定性实验
            {
                #region 定性实验
                if (CurrentScal.Num == "0")//实验中无正在做的标准品或定标液，使用历史定标
                {
                    db = new DbHelperOleDb(1);
                    //2018-08-03 zlx mod
                    DataTable tbScalingResult = DbHelperOleDb.Query(1,@"select Points,ActiveDate from tbScalingResult where ItemName = '" + ItemName
                        + "' and RegentBatch = '" + Batch + "' and Status = 1").Tables[0];
                    string points = "";
                    string ActiveDate = "";
                    foreach (DataRow dr in tbScalingResult.Rows)
                    {
                        if (dr != null)
                        {
                            points = dr["Points"].ToString();
                            ActiveDate = dr["ActiveDate"].ToString();
                        }

                    }
                    
                    if (Convert.ToDateTime(ActiveDate).AddDays(ExpiryDate).Date < DateTime.Now.Date)
                    {
                        //2017-08-17 zlx mod
                        ScalingState = 1;
                    }
                    //string points = DbHelperOleDb.GetSingle(@"select Points from tbScalingResult where ItemName = '" + ItemName
                    //    + "' and RegentBatch = '" + Batch + "' and Status = 1").ToString();
                    dtScalCacResult.Rows.Add(ItemName, 0, points, Batch);
                }

                if (sampleType.Contains("定标液"))
                {
                    dtScalingPMT.Rows.Add(PMT, 0, ItemName, 0, Batch);
                    //从dtScalingPMT中查询定性的当前项目的定标液信息
                    DataRow[] rows = dtScalingPMT.Select("ItemName = '" + ItemName + "' and RegentBatch = '" + Batch + "'");
                    if (rows.Length == int.Parse(CurrentScal.Num))
                    {
                        DataTable dtCacuData = new DataTable();
                        dtCacuData = dtScalingPMT.Clone(); // 克隆dtScalingPMT 的结构，包括所有 dtScalingPMT 架构和约束,并无数据；
                        foreach (DataRow row in rows)  // 将查询的结果添加到dtCacuData中；
                        {
                            dtCacuData.Rows.Add(row.ItemArray); //符合条件的所有数据
                        }
                        //  开启读数计算线程
                        CaculateThread = new Thread(new ParameterizedThreadStart(Calculate));
                        CaculateThread.IsBackground = true;
                        CaculateThread.Start(dtCacuData);
                    }

                }
                else//未知品质控品计算
                {
                    DataRow[] drCaluPara = dtScalCacResult.Select("ItemName = '" + ItemName + "' and RegentBatch = '" + Batch + "'");
                    while (drCaluPara.Length == 0)
                    {
                        drCaluPara = dtScalCacResult.Select("ItemName = '" + ItemName + "' and RegentBatch = '" + Batch + "'");
                        Thread.Sleep(30);
                    }
                    if (drCaluPara.Length > 0)
                    {
                        double scoValue = pmt / double.Parse(drCaluPara[0]["Result"].ToString());
                        concentration = "";
                        sco = scoValue.ToString();
                        if (scoValue <= 1)
                        {
                            result = "阳性";
                        }
                        else if (scoValue > 1)
                        {
                            result = "阴性";
                        }
                        else
                        {
                            result = "无法计算";
                        }
                    }
                }
                #endregion
            }
            else
            {
                #region 定量实验
                if (CurrentScal.Num == "0")//无标准品使用历史定标
                {
                    db = new DbHelperOleDb(1);
                //2018-08-03 zlx mod
                    DataTable tbScalingResult = DbHelperOleDb.Query(1,@"select Points,ActiveDate from tbScalingResult where ItemName = '" + ItemName
                        + "' and RegentBatch = '" + Batch + "' and Status = 1").Tables[0];
                    string points = "";
                    string ActiveDate = "";
                    foreach (DataRow dr in tbScalingResult.Rows)
                    {
                        if (dr != null)
                        {
                            points = dr["Points"].ToString();
                            ActiveDate = dr["ActiveDate"].ToString();
                        }
                    }
                    if (Convert.ToDateTime(ActiveDate).AddDays(ExpiryDate).Date < DateTime.Now.Date)
                    {
                        //2018-08-17 zlx mod
                        ScalingState = 1;
                    }
                    //string points = DbHelperOleDb.GetSingle(@"select Points from tbScalingResult where ItemName = '" + ItemName +
                    //    "' and RegentBatch= '" + Batch + "' and Status = 1").ToString();
                    if (points != "")
                    {
                        //获取定标点
                        string[] curvePoints = points.Split(';');
                        if (curvePoints.Length == 0)
                        {
                            return;
                        }
                        DataTable dtCacuData = new DataTable();
                        dtCacuData = dtScalingPMT.Clone();
                        for (int i = 0; i < curvePoints.Length; i++)
                        {
                            if (curvePoints[i] == "")
                            {
                                continue;
                            }
                            //将每个定标点的浓度和RLU分开放到数组中
                            string[] pointsData = curvePoints[i].Split(',');
                            dtCacuData.Rows.Add(pointsData[1].Substring(0, pointsData[1].IndexOf(")")), pointsData[0].Substring(1), ItemName, 1, Batch);
                        }
                        //  开启读数计算线程
                        CaculateThread = new Thread(new ParameterizedThreadStart(Calculate));
                        CaculateThread.IsBackground = true;
                        CaculateThread.Start(dtCacuData);
                    }
                }
                if (sampleType.Contains("标准品"))
                {
                    if (CurrentScal.Num.Split(',').Length >= 6)
                    {
                        #region 六点定标
                        result = "";
                        double conc = 0;
                        string[] concs = CurrentScal.TestConc.Split(',');
                        string[] Num = CurrentScal.Num.Split(',');
                        if (sampleType == "标准品A")
                        {
                            conc = double.Parse(concs[0]);
                        }
                        else if (sampleType == "标准品B")
                        {
                            conc = double.Parse(concs[1]);
                        }
                        else if (sampleType == "标准品C")
                        {
                            conc = double.Parse(concs[2]);
                        }
                        else if (sampleType == "标准品D")
                        {
                            conc = double.Parse(concs[3]);
                        }
                        else if (sampleType == "标准品E")
                        {
                            conc = double.Parse(concs[4]);
                        }
                        else if (sampleType == "标准品F")
                        {
                            conc = double.Parse(concs[5]);
                        }
                        else if (sampleType == "标准品G")
                        {
                            conc = double.Parse(concs[6]);
                        }
                        dtScalingPMT.Rows.Add(pmt, conc, ItemName, 1, Batch);
                        concentration = conc.ToString();
                        result = "";

                        //从dtScalingPMT中查询定性的当前项目的定标液信息
                        DataRow[] rows = dtScalingPMT.Select("ItemName = '" + ItemName + "' and RegentBatch= '" + Batch + "'");
                        int ScalSumNum = 0;
                        //计算该项目该批号的所有标准品数量
                        foreach (string N in Num)
                        {
                            ScalSumNum += int.Parse(N);
                        }
                        if (rows.Length == ScalSumNum)
                        {
                            DataTable dtCacuData = new DataTable();
                            dtCacuData = dtScalingPMT.Clone(); // 克隆dtScalingPMT 的结构，包括所有 dtScalingPMT 架构和约束,并无数据；
                            //存放需要取均值的标准品
                            string[] PmtAvg = new string[Num.Length];
                            for (int i = 0; i < concs.Length; i++)
                            {
                                for (int j = 0; j < rows.Length; j++)
                                {
                                    if (double.Parse(rows[j]["Conc"].ToString()) == double.Parse(concs[i]))
                                    {
                                        PmtAvg[i] += rows[j]["PMT"].ToString() + ",";
                                    }
                                }
                            }
                            for (int k = 0; k < PmtAvg.Length; k++)
                            {
                                dtCacuData.Rows.Add(AVG(PmtAvg[k].Split(',')), concs[k], ItemName, 1, Batch);
                            }
                            CaculateThread = new Thread(new ParameterizedThreadStart(Calculate));
                            CaculateThread.IsBackground = true;
                            CaculateThread.Start(dtCacuData);
                        }

                        #endregion
                    }
                    else
                    {
                        #region 两点校准
                        DataTable dtCacuD = new DataTable();
                        DataTable dtCacuData = new DataTable();
                        #region 将校准品的浓度与发光值保存到dtScalingPMT
                        result = "";
                        double conc = 0;
                        string[] concs = CurrentScal.TestConc.Split(',');
                        string[] Num = CurrentScal.Num.Split(',');
                        if (sampleType == "标准品C")
                        {
                            conc = double.Parse(concs[0]);//默认为标准品C点浓度
                        }
                        else if (sampleType == "标准品E")
                        {
                            conc = double.Parse(concs[1]);//默认为标准品E点浓度
                        }
                        dtScalingPMT.Rows.Add(pmt, conc, ItemName, 1, Batch);
                        concentration = conc.ToString();
                        result = "";
                        #endregion
                        //从dtScalingPMT中查询当前项目的校准品信息
                        DataRow[] rows = dtScalingPMT.Select("ItemName = '" + ItemName
                            + "' and RegentBatch= '" + Batch + "'");
                        int ScalSumNum = 0;
                        //计算该项目该批号的所有标准品数量
                        foreach (string N in Num)
                        {
                            ScalSumNum += int.Parse(N);
                        }
                        if (rows.Length == ScalSumNum)//两点定标只有两点
                        {
                            #region 获取原始定标的浓度与发光值
                            db = new DbHelperOleDb(1);
                            string points = DbHelperOleDb.GetSingle(1,@"select Points from tbMainScalCurve where ItemName = '"
                                + ItemName + "' and RegentBatch = '" + Batch + "'").ToString();
                            if (points != "")
                            {
                                //获取定标点
                                string[] curvePoints = points.Split(';');
                                if (curvePoints.Length == 0)
                                {
                                    return;
                                }

                                dtCacuD = dtScalingPMT.Clone();
                                for (int i = 0; i < curvePoints.Length; i++)
                                {
                                    if (curvePoints[i] == "")
                                    {
                                        continue;
                                    }
                                    //将每个定标点的浓度和RLU分开放到数组中
                                    string[] pointsData = curvePoints[i].Split(',');
                                    dtCacuD.Rows.Add(pointsData[1].Substring(0, pointsData[1].IndexOf(")")), pointsData[0].Substring(1), ItemName, 1, Batch);
                                }
                            }
                            #endregion
                            #region 取出校准品浓度与发光值存储在变量中

                            dtCacuData = dtScalingPMT.Clone(); // 克隆dtScalingPMT 的结构，包括所有 dtScalingPMT 架构和约束,并无数据；
                            //存放需要取均值的标准品
                            string[] PmtAvg = new string[Num.Length];
                            for (int i = 0; i < concs.Length; i++)
                            {
                                for (int j = 0; j < rows.Length; j++)
                                {
                                    double dtest = double.Parse(rows[j]["Conc"].ToString());
                                    if (double.Parse(rows[j]["Conc"].ToString()) == double.Parse(concs[i]))
                                    {
                                        PmtAvg[i] += rows[j]["PMT"].ToString() + ",";
                                    }
                                }
                            }
                            for (int k = 0; k < PmtAvg.Length; k++)
                            {
                                dtCacuData.Rows.Add(AVG(PmtAvg[k].Split(',')), concs[k], ItemName, 1, Batch);
                            }
                            #endregion
                            #region 根据校准品浓度与PMT，利用计算公式计算原始定标中各个浓度校正后的PMT，形成新的定标信息表
                            DataTable dtCorrectedCacuData = new DataTable();
                            dtCorrectedCacuData = dtScalingPMT.Clone();
                            dtCorrectedCacuData = GetCorrectedPoints(dtCacuData, dtCacuD);
                            #endregion
                            //  开启读数计算线程
                            CaculateThread = new Thread(new ParameterizedThreadStart(Calculate));
                            CaculateThread.IsBackground = true;
                            CaculateThread.Start(dtCorrectedCacuData);
                        }
                        #endregion
                    }
                }
                else
                {
                    #region 非标准品、非校准品 结果计算
                    DataRow[] drCaluPara = dtScalCacResult.Select("ItemName = '" + ItemName + "' and RegentBatch = '" + Batch + "'");
                    while (drCaluPara.Length == 0)
                    {
                        drCaluPara = dtScalCacResult.Select("ItemName = '" + ItemName + "' and RegentBatch = '" + Batch + "'");
                        Thread.Sleep(10);
                    }
                    string strPars = drCaluPara[0]["Result"].ToString();
                    string[] pars = strPars.Split('|');
                    List<double> datas = new List<double>();
                    foreach (string strPar in pars)
                    {
                        datas.Add(double.Parse(strPar));
                    }
                    #region 浓度和结果显示
                    double[] dbpars = datas.ToArray();
                    db = new DbHelperOleDb(0);
                    //线性范围最小值 2018-10-20 zlx mod
                    MinValue = double.Parse(DbHelperOleDb.GetSingle(0,@"select MinValue from tbProject where ShortName = '" + ItemName + "'").ToString());
                    //线性范围最大值 2018-10-20 zlx mod
                    db = new DbHelperOleDb(0);//2018-09-04 zlx add
                    MaxValue = double.Parse(DbHelperOleDb.GetSingle(0,@"select MaxValue from tbProject where ShortName = '" + ItemName + "'").ToString());
                    //正则表达式 表示汉字范围;
                    Regex cn = new Regex("[\u4e00-\u9fa5]+");
                    //根据线性范围决定显示类型
                    concentration = CalculationConcentration(ItemName, Batch, pmt).ToString("#0.000000");
                    //concentration = GetResultInverse(dbpars, pmt).ToString("#0.000000");//对得出的浓度进行小数点保留 lyn modify 20171118
                    LogFile.Instance.Write(DateTime.Now + "浓度：" + concentration + "");
                    #region 若用户对样本因结果不在线性范围内进行稀释
                    if (dtSampleRunInfo.Rows.Count > 0)
                    {
                        db = new DbHelperOleDb(0);
                        //项目中的稀释倍数
                        int DiuTimes = int.Parse(DbHelperOleDb.GetSingle(0,@"select DiluteCount from tbProject where ShortName = '" + ItemName + "'").ToString());
                        //该样本编号
                        string SampleNo = dgvWorkListData.Rows[testid - 1].Cells["SampleNo"].Value.ToString();
                        DataRow[] rows = dtSampleRunInfo.Select("ItemName='" + ItemName + "' and SampleNo='"
                        + SampleNo + "'");
                        string newDiuTimes = rows[0]["DilutionTimes"].ToString();
                        if (DiuTimes <int.Parse(newDiuTimes))
                        {
                            double DiuProportion = double.Parse(newDiuTimes) / DiuTimes;
                            LogFile.Instance.Write(DateTime.Now + "不固定稀释倍数：" + DiuProportion + "");
                            concentration = (double.Parse(concentration) * DiuProportion).ToString();
                        }
                    }
                    #endregion
                    if (double.Parse(concentration) < MinValue)
                    {
                        //concentration = "<" + MinValue;
                        concentration = (MinValue + 0.001).ToString();
                        result = "不在线性范围之内";
                    }
                    else if (double.Parse(concentration) > MaxValue * 1.2)
                    {
                        concentration = ">" + MaxValue * 1.2;
                        result = "不在线性范围之内";
                    }
                    else if (VRangeType != "" && int.Parse(VRangeType) > 0)
                        result = "";
                    else
                    {
                        if (cn.IsMatch(Range1))//range1字符串中有中文
                        {
                            result = "";
                        }
                        else if (Regex.Matches(Range1, "[a-zA-Z]").Count > 0)//range1字符串中有英文
                        {
                            result = "";
                        }
                        else
                        {
                            if (Range1.Contains("-"))
                            {
                                string[] ranges = Range1.Split('-');
                                if (double.Parse(concentration) < double.Parse(ranges[0]))
                                {
                                    result = "↓";
                                }
                                else if (double.Parse(concentration) > double.Parse(ranges[1]))
                                {
                                    result = "↑";
                                }
                                else
                                    result = "正常";
                            }
                            else if (Range1.Contains("<"))
                            {
                                if (double.Parse(concentration) >= double.Parse(Range1.Substring(1)))
                                {
                                    result = "↑";
                                }
                                else
                                {
                                    result = "正常";
                                }
                            }
                            else if (Range1.Contains("<="))
                            {
                                if (double.Parse(concentration) > double.Parse(Range1.Substring(2)))
                                {
                                    result = "↑";
                                }
                                else
                                {
                                    result = "正常";
                                }
                            }
                            else if (Range1.Contains(">"))
                            {
                                if (double.Parse(concentration) <= double.Parse(Range1.Substring(1)))
                                {
                                    result = "↓";
                                }
                                else
                                {
                                    result = "正常";
                                }

                            }
                            else if (Range1.Contains(">="))
                            {
                                if (double.Parse(concentration) < double.Parse(Range1.Substring(2)))
                                {
                                    result = "↓";
                                }
                                else
                                {
                                    result = "正常";
                                }
                            }
                        }
                    }
                    #endregion
                    #endregion
                }
                #endregion
            }
            #endregion
            testResult.SampleID = int.Parse
                (dgvWorkListData.Rows[testid - 1].Cells["SampleID"].Value.ToString());
            testResult.TestID = testid;
            testResult.SampleNo = (dgvWorkListData.Rows[testid - 1].Cells["SampleNo"].Value.ToString());
            testResult.SamplePos = int.Parse(dgvWorkListData.Rows[testid - 1].Cells["Position"].Value.ToString());
            testResult.SampleType = sampleType;
            testResult.ItemName = ItemName;
            testResult.PMT = pmt;
            //testResult.concentration = concentration;
            if (concentration != "" && !(concentration.Contains(">") || concentration.Contains("<")))
                testResult.concentration = Convert.ToDouble(concentration).ToString("F" + NumResult + "");//2018-11-07 zlx mod
            else
                testResult.concentration = concentration;
            testResult.Result = result;
            testResult.Range1 = Range1;
            testResult.Range2 = Range2;
            testResult.sco = sco;
            //2018-08-17 zlx add
            testResult.Status = ScalingState;
            //2018-08-17 zlx add
            testResult.ReagentBeach = dgvWorkListData.Rows[testid - 1].Cells["RegentBatch"].Value.ToString();
            //2018-10-17 zlx mod
            testResult.SubstratePipe = dgvWorkListData.Rows[testid - 1].Cells["SubstratePipe"].Value.ToString();
            testResult.Unit = Unit;//2018-11-10 zlx add

            SaveTestResultData(BToListTi, lis1, lis2);

            calNowFlag = false;
        }

        /// <summary>
        /// 保存实验结果
        /// </summary>
        /// <param name="BToListTi"></param>
        /// <param name="lis1"></param>
        /// <param name="lis2"></param>
        void SaveTestResultData(List<TestItem> BToListTi, List<TestItem> lis1, List<TestItem> lis2)
        {
            if (lisTiEnd.Count == BToListTi.Count)
                frmTestResult.BRun = false;
            LogFile.Instance.Write("*********  发光值  ： " + testResult.PMT + "  **********");
            BTestResult.Add(testResult);
            TemporaryTestResult.Add(testResult);
            if (testResult.SampleType.Contains("标准品"))
            {
                GC.KeepAlive(testResult);//防止被回收               
                List<TestItem> BToList = BToListTi.FindAll(tx => (tx.ItemName == testResult.ItemName && tx.SampleType.Contains("标准品") && tx.RegentBatch == dgvWorkListData.Rows[testResult.TestID - 1].Cells["RegentBatch"].Value.ToString()));
                List<TestItem> ENDList = lisTiEnd.FindAll(tx => (tx.ItemName == testResult.ItemName && tx.SampleType.Contains("标准品")));
                if (BToList.Count == ENDList.Count)
                {
                    //List<TestResult> ScalingResult = new List<TestResult>(BTestResult).FindAll(tx => (tx.ItemName == testResult.ItemName && testResult.SampleType.Contains("标准品")));
                    //frmTestResult f = new frmTestResult();
                    List<TestResult> ScalingResult = new List<TestResult>(TemporaryTestResult).FindAll(tx => (tx.ItemName == testResult.ItemName && testResult.SampleType.Contains("标准品")));

                    Invoke(new Action(() =>
                    {
                        frmTestResult f;
                        if (!CheckFormIsOpen("frmTestResult"))
                        {
                            f = new frmTestResult();
                        }
                        else
                        {
                            f = (frmTestResult)Application.OpenForms["frmTestResult"];
                        }
                        List<TestResult> SScalingResult = new List<TestResult>();
                        foreach (TestResult tr in ScalingResult)
                        {
                            SScalingResult.Add(tr);
                        }
                        f.SaveStandardResult(ScalingResult);
                    }));
                }
            }
            else
                SaveResultDate(testResult);
            SendLisData(lis1, lis2);
            testResult = new TestResult();
        }

        /// <summary>
        /// 向lis系统发送实验结果
        /// </summary>
        /// <param name="lis1"></param>
        /// <param name="lis2"></param>
        void SendLisData(List<TestItem> lis1, List<TestItem> lis2)
        {
            string CommunicationType = OperateIniFile.ReadInIPara("LisSet", "CommunicationType");
            bool IsTrueTimeTran = bool.Parse(OperateIniFile.ReadInIPara("LisSet", "IsTrueTimeTran"));
            string tranInfo = OperateIniFile.ReadInIPara("LisSet", "TransInfo");
            string SendingApplication = OperateIniFile.ReadInIPara("LisSet", "SendingApplication");
            string SendingFacility = OperateIniFile.ReadInIPara("LisSet", "SendingFacility");
            switch (CommunicationType)
            {
                case "网口通讯":
                    if (LisCommunication.Instance.IsConnect() && IsTrueTimeTran && lis1.Count == lis2.Count)
                    {
                        CMessageParser Cmp = new CMessageParser();
                        Cmp.SendApplication = SendingApplication;
                        Cmp.SendFacility = SendingFacility;
                        List<TestResult> list = new List<TestResult>();
                        for (int i = 0; i < BTestResult.Count; i++)
                        {
                            if (BTestResult[i].SampleNo == testResult.SampleNo)
                                list.Add(BTestResult[i]);
                        }
                        Cmp.SendORU(list);
                    }
                    break;
                case "串口通讯":
                    if (LisConnection.Instance.IsOpen() && IsTrueTimeTran && lis1.Count == lis2.Count)
                    {
                        CAMessageParser Cmp = new CAMessageParser();
                        Cmp.SendApplication = SendingApplication;
                        Cmp.SendFacility = SendingFacility;
                        List<TestResult> list = new List<TestResult>();
                        for (int i = 0; i < BTestResult.Count; i++)
                        {
                            if (BTestResult[i].SampleNo == testResult.SampleNo)
                                list.Add(BTestResult[i]);
                        }
                        Cmp.SendORU(list);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 保存单个实验结果 2018-07-19 zlx add
        /// </summary>
        /// <param name="result"></param>
        public void SaveResultDate(TestResult result)
        {
            Model.tbAssayResult modelAssayResult = new Model.tbAssayResult();
            Model.tbQCResult modelQCResult = new Model.tbQCResult();
           
            //存储质控实验结果到数据库
            if (result.SampleType.Contains("质控品"))
            {
                string QCLevel;
                if (result.SampleType == "质控品H")
                {
                    QCLevel = "0";
                }
                else if (result.SampleType == "质控品M")
                {
                    QCLevel = "1";
                }
                else
                {
                    QCLevel = "2";
                }
                DbHelperOleDb db = new DbHelperOleDb(3);
                DataTable dtQCInfo = DbHelperOleDb.Query(3,@"select QCID,Batch,QCLevel from tbQC where status = '1' and ProjectName = '"
                                                            + result.ItemName + "'and QCLevel = '" + QCLevel + "' and Status = '1'").Tables[0];
                if (dtQCInfo == null || dtQCInfo.Rows.Count == 0)
                {
                    //frmMsgShow.MessageShow("实验结果", "查找不到相关质控的信息！");
                    return;
                }
                modelQCResult.Batch = dtQCInfo.Rows[0][1].ToString();
                if (result.concentration == "")
                {
                    modelQCResult.Concentration = 0;
                }
                else if (result.concentration.Contains(">") || result.concentration.Contains("<"))
                    modelQCResult.Concentration = double.Parse(result.concentration.Substring(1, result.concentration.Length - 1));
                else
                {
                    modelQCResult.Concentration = double.Parse(result.concentration);
                }
                //modelQCResult.Concentration = double.Parse(result.concentration);
                modelQCResult.ConcLevel = int.Parse(dtQCInfo.Rows[0][2].ToString());
                modelQCResult.ConcSPEC = "";
                modelQCResult.ItemName = result.ItemName;
                modelQCResult.PMTCounter = result.PMT;
                modelQCResult.QCID = int.Parse(dtQCInfo.Rows[0][0].ToString());
                modelQCResult.Source = 0;
                modelQCResult.TestDate = DateTime.Now;
                modelQCResult.Unit = "";
                db = new DbHelperOleDb(1);
                new BLL.tbQCResult().Add(modelQCResult);
            }
            //存储样本实验结果到数据库
            else
            {
                modelAssayResult.SampleID = result.SampleID;
                modelAssayResult.Batch = "";
                if (result.concentration == "")
                {
                    modelAssayResult.Concentration = 0;
                }
                else if (result.concentration.Contains(">") || result.concentration.Contains("<"))
                    modelAssayResult.Concentration = double.Parse(result.concentration.Substring(1, result.concentration.Length - 1));
                else
                {
                    modelAssayResult.Concentration = double.Parse(result.concentration);
                }
                modelAssayResult.ConcSpec = "";
                modelAssayResult.DiluteCount = 0;
                modelAssayResult.ItemName = result.ItemName;
                modelAssayResult.PMTCounter = result.PMT;
                modelAssayResult.Range = result.Range1;
                modelAssayResult.Result = result.Result;
                modelAssayResult.Specification = "";
                //modelAssayResult.Status = 0;
                modelAssayResult.TestDate = DateTime.Now;
                modelAssayResult.Unit = "";
                modelAssayResult.Upload = "";
                modelAssayResult.Status = testResult.Status;//2018-07-17 zlx mod
                modelAssayResult.Batch = testResult.ReagentBeach;//2017-08-18 zlx add
                modelAssayResult.Unit = result.Unit;//2018-11-10 zlx mod
                DbHelperOleDb db = new DbHelperOleDb(1);
                new BLL.tbAssayResult().Add(modelAssayResult);
            }
            lisSavedId.Add(result.TestID);
        }
        /// <summary>
        /// 对一组可以转换成int类型的数组取均值
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        int AVG(string[] s)
        {
            int sum = 0;
            int emptyNum = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == "")
                {
                    emptyNum++;
                    continue;
                }
                sum += int.Parse(s[i]);
            }

            return sum / (s.Length - emptyNum);
        }
        /// <summary>
        /// 获取两点校正后的六个点坐标值（浓度与发光值）
        /// </summary>
        /// <param name="dtNew">两点校准的浓度与发光值信息表</param>
        /// <param name="dtOld">原始定标的浓度与发光值信息表</param>
        /// <returns>校正以后的定标信息表</returns>
        public DataTable GetCorrectedPoints(DataTable dtNew, DataTable dtOld)
        {
            double k, b, x, y, x1, y1;
            x = double.Parse(dtNew.Rows[0]["Conc"].ToString());
            x1 = double.Parse(dtNew.Rows[1]["Conc"].ToString());
            //注：当前默认校准点浓度为标准品C、E两点浓度，故不需要比对浓度，直接计算即可。
            //y为同浓度点的吸光度差值占原始浓度的百分比，即y等于新测试吸光度的值减去原始吸光度的值，而后再除以原始吸光度的值
            //y = (double.Parse(dtNew.Rows[0]["PMT"].ToString()) - double.Parse(dtOld.Rows[2]["PMT"].ToString())) / double.Parse(dtOld.Rows[2]["PMT"].ToString());
            //y1 = (double.Parse(dtNew.Rows[1]["PMT"].ToString()) - double.Parse(dtOld.Rows[4]["PMT"].ToString())) / double.Parse(dtOld.Rows[4]["PMT"].ToString());
            //modify 20181026 y
            y = double.Parse(dtNew.Rows[0]["PMT"].ToString()) / double.Parse(dtOld.Rows[2]["PMT"].ToString());
            y1 = double.Parse(dtNew.Rows[1]["PMT"].ToString()) / double.Parse(dtOld.Rows[4]["PMT"].ToString());

            k = (y1 - y) / (x1 - x);
            b = y - k * x;
            for (int i = 0; i < dtOld.Rows.Count; i++)
            {
                if (i == 2)
                {
                    dtOld.Rows[2]["PMT"] = dtNew.Rows[0]["PMT"].ToString();
                }
                else if (i == 4)
                {
                    dtOld.Rows[4]["PMT"] = dtNew.Rows[1]["PMT"].ToString();
                }
                else
                {
                    //除了C、E两点之外，其他点的PMT值为原始浓度乘以（1+吸光度变化百分比）注意（此百分比可正可负）
                    //dtOld.Rows[i]["PMT"] = double.Parse(dtOld.Rows[i]["PMT"].ToString()) * (1 + (k * double.Parse(dtOld.Rows[i]["Conc"].ToString()) + b));
                    //modify 20181026 y
                    dtOld.Rows[i]["PMT"] = double.Parse(dtOld.Rows[i]["PMT"].ToString()) * (k * double.Parse(dtOld.Rows[i]["Conc"].ToString()) + b);
                }
            }
            return dtOld;
        }

        #region 计算浓度
        /// <summary>
        /// 根据发光值计算浓度
        /// </summary>
        /// <param name="name">项目名称</param>
        /// <param name="reagentBatch">试剂批号</param>
        /// <param name="yValue">发光值</param>
        /// <returns></returns>
        private double CalculationConcentration(string name, string reagentBatch, double yValue)
        {
            if (yValue < 0)
                yValue = 0;

            Calculater er = GetCalculater(name);
            List<Data_Value> scalingResult = GetScalingResult(name, reagentBatch);

            if (scalingResult == null)
                return 0;

            if (scalingResult[0].Data == 0)
                scalingResult[0].Data = 0.0001;

            #region 超出范围
            if (scalingResult[0].DataValue < scalingResult[1].DataValue)
            {
                if (yValue < scalingResult[0].DataValue)
                    return scalingResult[0].Data - 0.1;

                //if (yValue > scalingResult[scalingResult.Count() - 1].DataValue)
                //    return scalingResult[scalingResult.Count() - 1].Data;
            }

            if (scalingResult[0].DataValue > scalingResult[1].DataValue)
            {
                if (yValue > scalingResult[0].DataValue)
                    return scalingResult[0].Data - 0.1;

                //if (yValue < scalingResult[scalingResult.Count() - 1].DataValue)
                //    return scalingResult[scalingResult.Count() - 1].Data;
            }
            #endregion

            er.AddData(scalingResult);
            er.Fit();

            return er.GetResultInverse(yValue);
        }

        /// <summary>
        /// 获取拟合算法实例
        /// </summary>
        /// <param name="name">项目名称</param>
        /// <returns></returns>
        private Calculater GetCalculater(string name)
        {
            DbHelperOleDb db = new DbHelperOleDb(0);
            int calMode = int.Parse(DbHelperOleDb.GetSingle(0,
                @"select CalMode from tbProject where ShortName = '" + name + "'").ToString());

            CalculateFactory calculate = new CalculateFactory();
            Calculater er = calculate.getCaler(calMode);

            return er;
        }

        /// <summary>
        /// 获取定标结果
        /// </summary>
        /// <param name="name">项目名称</param>
        /// <param name="reagentBatch">试剂批号</param>
        /// <returns></returns>
        private List<Data_Value> GetScalingResult(string name, string reagentBatch)
        {
            BLL.tbScalingResult bllscalResult = new BLL.tbScalingResult();
            DbHelperOleDb db = new DbHelperOleDb(1);
            DataTable scalingResultTemp = bllscalResult.GetList("ItemName='" + name + "' AND RegentBatch='" + reagentBatch + "' AND Status= 1").Tables[0];

            List<Data_Value> scalingResult = new List<Data_Value>();

            if (scalingResultTemp.Rows.Count > 0)
            {
                string[] mainPoint = scalingResultTemp.Rows[0]["Points"].ToString().Replace("(", "").Replace(")", "").Split(';');
                for (int i = 0; i < mainPoint.Length; i++)
                {
                    string[] pointinfo = mainPoint[i].Split(',');
                    if (pointinfo.Length == 2)
                        scalingResult.Add(new Data_Value() { Data = double.Parse(pointinfo[0]), DataValue = double.Parse(pointinfo[1]) });
                }

                return scalingResult;
            }

            return null;
        }
        #endregion

        /// <summary>
        /// 计算定量实验中普通样本的浓度
        /// </summary>
        /// <param name="_pars">拟合公式系数</param>
        /// <param name="yValue">吸光度值</param>
        /// <returns></returns>
        [Obsolete("废弃")]
        public double GetResultInverse(double[] _pars, double yValue)//计算浓度
        {
            #region 浓度计算之前处理
            if (yValue < 0)
                yValue = Math.Abs(yValue);
            if (yValue < _pars[3])
                return -1;
            double d = 0.0001;
             double b = 1;
             if (_pars[1] != 0)
             {
                 b = (1 / _pars[1]);
             }
            Random rd = new Random();
            foreach (double par in _pars)
            {
                if (double.IsInfinity(par))
                {
                    return 0;
                }
            }
            if ((yValue - _pars[3]) == 0)
            {
                d = ((double)rd.Next(10, 90)) / 10000;
            }
            else
            {
                d = (yValue - _pars[3]);
            }
            //if (m < d)
            //{
            //    if (_pars[1] > 0)
            //    {
            //        d = m;
            //    }
            //    else
            //    {
            //        d = m * 0.99;
            //    }
            //}
            #region 根据幂函数的性质以及不同情况下的定义域进行处理
            double[] fractions = decimalToFractions(b);
            double molecule = Math.Abs(fractions[0]);
            double denominator =Math.Abs( fractions[1]);
            double fourlPart = ((_pars[0] - _pars[3]) / d) - 1;
            if (denominator != 0)
            {
                if (fractions[1] != 0)
                {
                    if (fractions[0] / fractions[1] < 0)
                    {
                        if (molecule % 2 != 0 && denominator % 2 != 0 ||
                            molecule % 2 == 0 && denominator % 2 != 0)//分子和分母都为奇数或者分子为偶数，分母为奇数
                        {
                            if (fourlPart == 0)
                            {
                                fourlPart = ((double)rd.Next(10, 90)) / 10000;
                            }
                        }
                        else if (molecule % 2 != 0 && denominator % 2 == 0)//
                        {
                            if (fourlPart <= 0)
                            {
                                fourlPart = ((double)rd.Next(10, 90)) / 10000;
                            }
                        }
                    }
                    else
                    {
                        if (molecule % 2 != 0 && denominator % 2 == 0)
                        {
                            if (fourlPart < 0)
                            {
                                fourlPart = 0;
                            }
                        }
                    }
                }
            }
            #endregion
            #endregion
            double conc = _pars[2] * (Math.Pow(fourlPart, b));
            return conc;
        }
        /// <summary>
        /// 小数转分数
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        double[] decimalToFractions(double d)
        {
            double[] f = new double[2];
            try
            {
                bool fushuFlag = false;
                double inputnum = Convert.ToDouble(d);
                if (inputnum < 0)
                {
                    fushuFlag = true;
                    inputnum = Math.Abs(inputnum);
                }
                string[] array = inputnum.ToString().Split('.');
                int len = array[1].Length;
                if (len > 9)
                {
                    len = 9;
                    array[1] = double.Parse(array[1]).ToString("f9");
                }
                int num = Convert.ToInt32(Math.Pow(10, len));
                int value = Convert.ToInt32(inputnum * num);
                int a = value;
                int b = num;
                while (a != b)
                {
                    if (a > b)
                        a = a - b;
                    else
                        b = b - a;
                }
                value = value / a;
                num = num / a;
                if (fushuFlag)
                {
                    value = int.Parse("-" + value.ToString());
                }
                f[0] = value;
                f[1] = num;
            }
            catch (Exception)
            {
                f[0] = d;
                f[1] = 0;
            }
            return f;

        }
        object locker = new object();
        /// <summary>
        /// 结果计算
        /// </summary>
        /// <param name="obCaculate"></param>
        void Calculate(object obCaculate)
        {
            Thread.Sleep(10);
            lock (locker)
            {
                DataTable dtCaculate = new DataTable();
                dtCaculate = dtScalingPMT.Clone();
                dtCaculate = obCaculate as DataTable;
                if (dtCaculate.Rows.Count == 0 || dtCaculate == null)
                {
                    return;
                }
                DbHelperOleDb db = new DbHelperOleDb(0);
                #region 定性实验计算
                if (dtCaculate.Rows[0]["ItemType"].ToString() == "0")
                {
                    db = new DbHelperOleDb(0);
                    //计算参数
                    double calulatePara = double.Parse(DbHelperOleDb.GetSingle(0, @"select CalculateMethod from tbProject where ShortName = '"
                                                                                + dtCaculate.Rows[0]["ItemName"].ToString() + "'").ToString()) / 100;
                    //项目cutoff值
                    double cutoffValue = 0;
                    //吸光度和
                    double sumPMT = 0;
                    //吸光度均值
                    double avgPMT = 0;
                    for (int i = 0; i < dtCaculate.Rows.Count; i++)
                    {
                        sumPMT += double.Parse(dtCaculate.Rows[i][4].ToString());
                    }
                    avgPMT = sumPMT / dtCaculate.Rows.Count;
                    cutoffValue = avgPMT * calulatePara;
                    dtScalCacResult.Rows.Add(dtCaculate.Rows[0]["ItemName"].ToString(), 0, cutoffValue.ToString(), dtCaculate.Rows[0]["RegentBatch"].ToString());
                }
                #endregion
                #region 定量实验计算
                else
                {
                    //新建定标方程类的实例
                    CalculateFactory ft = new CalculateFactory();
                    List<Data_Value> CurveData = new List<Data_Value>();
                    db = new DbHelperOleDb(0);
                    //数据库项目表中查询定标方程选择字段
                    int CalMode = int.Parse(DbHelperOleDb.GetSingle(0, @"select CalMode from tbProject where ShortName = '"
                                                                    + dtCaculate.Rows[0]["ItemName"].ToString() + "'").ToString());
                    //取到定标计算公式
                    Calculater er = ft.getCaler(CalMode);
                    //2018-12-05 zlx 
                    for (int j = 0; j < dtCaculate.Rows.Count; j++)
                    {
                        CurveData.Add(new Data_Value()
                        {
                            Data = double.Parse(dtCaculate.Rows[j]["Conc"].ToString()),
                            DataValue = double.Parse(dtCaculate.Rows[j]["PMT"].ToString())
                        });
                    }

                    //排序
                    CurveData.Sort(new Data_ValueDataAsc());
                    ////////////////////y Mod This Block 20181114

                    //相同浓度的吸光度值两两求均值
                    //for (int i = CurveData.Count - 2; i >= 0; i--)
                    //{
                    //    Data_Value v2 = CurveData[i + 1];
                    //    Data_Value v1 = CurveData[i];
                    //    if (v2.Data == v1.Data)
                    //    {
                    //        v1.DataValue = (v1.DataValue + v2.DataValue) / 2;
                    //        CurveData.RemoveAt(i + 1);
                    //    }
                    //}

                    //2018-12-06 zlx 屏蔽
                    List<Data_Value> dataTableTemp = new List<Data_Value>();
                    for (int i = 0; i < CurveData.Count; i++)
                    {
                        double Data = CurveData[i].Data;
                        bool isHave = false;
                        foreach (var item in dataTableTemp)
                        {
                            if (item.Data == Data)
                            {
                                isHave = true;
                                break;
                            }
                        }
                        if (isHave) continue;
                        else
                        {
                            int num = 0;
                            double Sum = 0;
                            List<Data_Value> ListItem = CurveData.FindAll(tx => tx.Data == Data);
                            for (int j = 0; j < ListItem.Count; j++)
                            {
                                if (ListItem[j].Data == Data)
                                {
                                    Sum += ListItem[j].DataValue;
                                    num++;
                                }
                            }
                            Sum = Sum / num;
                            dataTableTemp.Add(new Data_Value() { Data = Data, DataValue = Sum });
                        }
                    }
                    CurveData = dataTableTemp;

                    ////////////////////////This Block End
                    if (CurveData.Count > 0)//ltData标准品
                    {
                        for (int i = 0; i < CurveData.Count; i++)
                        {
                            if (CurveData[i].Data == 0)
                            {
                                CurveData[i].Data = 0.0001;
                            }
                            if (CurveData[i].Data == 1)
                            {
                                CurveData[i].Data = 0.999999;
                            }
                            if (CurveData[i].DataValue == 0)
                            {
                                CurveData[i].DataValue = 0.0001;
                            }
                        }
                        for (int i = 0; i < CurveData.Count; i++)
                        {
                            //对处理过的数据进行纠错
                            if (double.IsNaN(CurveData[i].DataValue) || double.IsNaN(CurveData[i].Data))
                            {
                                MessageBox.Show("函数计算错误，可能该数据不适合此回归模型", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }

                        //计算定标曲线的系数
                        //for (int i = 0; i < CurveData.Count; i++)
                        //{
                        er.AddData(CurveData);
                        er.Fit();
                        //}
                    }

                    foreach (double par in er._pars)
                    {
                        if (double.IsNaN(par) || double.IsInfinity(par))
                        {
                            //dtScalCacResult.Rows.Add(dtCaculate.Rows[0]["ItemName"].ToString(), 1, "");
                            MessageBox.Show(dtCaculate.Rows[0]["ItemName"].ToString() + "项目回归计算时出现运算错误，可能该数据不适合此回归模型", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    dtScalCacResult.Rows.Add(dtCaculate.Rows[0]["ItemName"].ToString(), 1, er.StrPars, dtCaculate.Rows[0]["RegentBatch"]);

                }
                #endregion
            }
        }

        #endregion

        /// <summary>
        /// 实验暂停
        /// </summary>
        void AllPause()
        {
            frmMain.pauseFlag = true;
            stopTimer.Stop();//倒计时暂停
        }
        /// <summary>
        /// 实验停止
        /// </summary>
        void AllStop()
        {
            if (RunFlag == (int)RunFlagStart.IsRuning)
            {
                NetCom3.Instance.stopsendFlag = true;
                RunFlag = (int)RunFlagStart.IsStoping;
                if (AddingSampleFlag)
                {
                    AddingSampleFlag = false;
                    //NetCom3.Delay(10);//如果正在加样步骤，暂时先不会弹出样本装载界面
                }
                LogFile.Instance.Write(DateTime.Now + "实验调用了终止实验的程序！");
                StopStopWatch();//终止倒计时
                BQLiquaid = false;//2018-09-14
                buttonEnableRun(false);
                frmAddSample.newSample = true;
                if (btnRunStatus != null)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        btnRunStatus();
                    }));
                }
                if (SpDiskUpdate != null)
                {
                    this.BeginInvoke(new Action(() => { SpDiskUpdate(); }));
                }
                //将清洗盘当前取放管位置的孔号保存到配置文件中
                //OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                IniUpdateAccess();//由配置文件同步试剂与底物信息到数据库
                try
                {
                    //if (NetCom3.Instance.MoverrorFlag != (int)ErrorState.Success)
                    //    MoveTubeUseFlag = false; 
                    //else
                    //{
                    //    while (MoveTubeUseFlag)
                    //        Thread.Sleep(10);
                    //}
                    while ((MoveTubeThread != null && MoveTubeThread.ThreadState != ThreadState.Stopped) && MoveTubeThread.IsAlive)//MoveTubeUseFlag||
                    {
                        MoveTubeThread.Abort();
                        MoveTubeThread.Join();
                        Thread.Sleep(100);
                    }
                    MoveTubeThread = null;
                    while ((washThread != null && washThread.ThreadState != ThreadState.Stopped) && washThread.IsAlive)//WashTrayUseFlag || 
                    {
                        washThread.Abort();
                        washThread.Join();
                        Thread.Sleep(100);
                    }
                    washThread = null;
                    while ((AddLiquidThread != null && AddLiquidThread.ThreadState != ThreadState.Stopped) && AddLiquidThread.IsAlive)//addLiquiding || 
                    {
                        AddLiquidThread.Abort();
                        AddLiquidThread.Join();
                        Thread.Sleep(100);
                    }
                    AddLiquidThread = null;
                    while (ReadThread != null && ReadThread.ThreadState != ThreadState.Stopped && ReadThread.IsAlive)
                    {
                        ReadThread.Abort();
                        ReadThread.Join();
                        Thread.Sleep(100);
                    }
                    while (calNowFlag)
                        Thread.Sleep(10);
                    while ((CaculateThread != null && CaculateThread.ThreadState != ThreadState.Stopped) && CaculateThread.IsAlive)//calNowFlag||
                    {
                        CaculateThread.Abort();
                        CaculateThread.Join();
                        Thread.Sleep(100);
                    }
                    CaculateThread = null;
                    timeReckon.Stop();
                    timer.Enabled = false;
                }
                catch (ThreadAbortException e)
                {
                    frmMsgShow.MessageShow("工作列表", e.Message);
                }
                finally
                {
                    RunFlag = (int)RunFlagStart.Stoped;
                    if (NetCom3.isConnect)
                        NetCom3.Instance.stopsendFlag = false;
                    if (frmMain.pauseFlag)
                        frmMain.pauseFlag = false;
                    //this.Close();
                    if (CheckFormIsOpen("frmWorkList"))
                    {
                        this.BeginInvoke(new Action(() => { Close(); }));
                    }
                }
            }
        }
        /// <summary>
        /// 实验继续进行
        /// </summary>
        void Goon()
        {
            if (EmergencyFlag || addOrdinaryFlag)
            {
                frmMsgShow.MessageShow("工作列表", "正在添加急诊或普通样本，请在样本装载界面生成工作列表运行！");
                return;
            }
            LoadingHelper.ShowLoadingScreen();
            frmSampleLoad.CaculatingFlag = true;
            NetCom3.ComWait.Reset();
            #region 进度重新计算
            //获取已经开始运行的样本的进度表
            List<TestSchedule> RunSchedule = lisTestSchedule.FindAll(tx => tx.TestID < NoStartTestId );
            List<string> runFreeTime = new List<string>() { (sumTime + 2).ToString() + "-1000000" };
            for (int i = 0; i < RunSchedule.Count; i++)
            {
                TestSchedule ts = RunSchedule[i];
                for (int j = 0; j < runFreeTime.Count; j++)
                {
                    string[] minMaxTime = runFreeTime[j].Split('-');
                    //本次空闲时间段的最小值
                    int minTime = int.Parse(minMaxTime[0]);
                    //本次空闲时间段的最大值
                    int maxTime = int.Parse(minMaxTime[1]);
                    if (ts.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddBeads ||
                        ts.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddLiquidTube ||
                        ts.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddSingleR)
                    {
                        if (ts.StartTime <= minTime)
                        {
                            if (ts.EndTime < minTime)
                            {
                                continue;
                            }
                            else if (ts.EndTime == minTime || (ts.EndTime < maxTime && ts.EndTime > minTime))
                            {
                                runFreeTime[j] = (ts.EndTime + 1).ToString() + "-" + maxTime;
                                break;
                            }
                            else if (ts.EndTime >= maxTime)
                            {
                                runFreeTime.RemoveAt(j);
                                j = j - 1;
                                break;
                            }
                        }
                        else if (ts.StartTime > minTime && ts.StartTime < maxTime)
                        {
                            if (ts.EndTime > minTime && ts.EndTime < maxTime)
                            {
                                int index = 0;
                                runFreeTime[j] = (minTime).ToString() + "-" + (ts.StartTime - 1);
                                index = j + 1;
                                runFreeTime.Insert(index, (ts.EndTime + 1).ToString() + "-" + maxTime);
                                break;
                            }
                            else if (ts.EndTime >= maxTime)
                            {
                                runFreeTime[j] = (minTime).ToString() + "-" + (ts.StartTime - 1);
                                break;
                            }
                        }
                        else if (ts.StartTime == maxTime)
                        {
                            if (ts.EndTime >= maxTime)
                            {
                                runFreeTime[j] = (minTime).ToString() + "-" + (maxTime - 1);
                                break;
                            }
                        }
                        else if (ts.StartTime > maxTime)
                        {
                            continue;
                        }
                    }
                }
            }
            //未运行的样本进度计算
            List<TestSchedule> lisTestNoRun = ExperimentalScheduleAlgorithm(orderModifyTestID(lisTestSchedule.FindAll(tx => tx.TestID >= NoStartTestId)), runFreeTime);
            foreach (TestSchedule TestS in lisTestNoRun)
            {
                lisTestSchedule.Remove(TestS);
                TestS.TestID = TestS.TestID + NoStartTestId - 1;
            }
            lisTestSchedule.AddRange(lisTestNoRun);
            lisTestSchedule.Sort(new SortRun());
            #endregion
            List<TestSchedule> tss = lisTestSchedule.FindAll(tx => tx.StartTime <= sumTime);//2019-04-08 删除等于号
            if (tss.Count > 1)
                _GaDoingOne = tss[tss.Count - 1];
            if (EnterRun)//在进行实验调度前点暂停，防止第一个实验不加样 2020/08/13
                TestStep = GaNextOne();
            LoadingHelper.CloseForm();
            NetCom3.ComWait.Set();
            frmSampleLoad.CaculatingFlag = false;
            MaxTime = lisTestSchedule.Select(it => it.EndTime).ToList<int>().Max();
            LastMaxTime = (int)MaxTime;
            MaxTime = MaxTime - sumTime;
            MaxTime *= PiusTimes;
            LastSumTime = sumTime;
            TimeSpan span = new TimeSpan(0, 0, Convert.ToInt32(MaxTime));
            while (!this.IsHandleCreated)
            {
                Thread.Sleep(30);
            }
            TimeLabel2.Invoke(new Action(() =>
            {
                TimeLabel2.Text = ((int)span.TotalHours).ToString("00");
                TimeLabel3.Text = span.Minutes.ToString("00");
                TimeLabel2.Visible = TimeLabel1.Visible = TimeLabel3.Visible = label2.Visible = label3.Visible = true;
            }));
            stopTimer.Start();//倒计时继续
        }

        #region 控件窗体
        /// <summary>
        /// 刷新列表中的进度条控件，重新显示
        /// </summary>
        /// <param name="runflag">实验是否正在运行</param>
        void dgvRefresh(bool runflag)
        {

            EventControlAdd += new Action<bool>(AddSchedule);
            EventControlAdd(runflag);

        }
        private void frmWorkList_SizeChanged(object sender, EventArgs e)
        {
            formSizeChange(this);
        }

        private void fbtnReturn_Click(object sender, EventArgs e)
        {
            if (RunFlag == (int)RunFlagStart.IsRuning)
            {
                this.Hide();
            }
            else
            {
                frmAddSample.newSample = true;
                this.Close();
            }
        }

        private void btnLoadSample_Click(object sender, EventArgs e)
        {

            if (!CheckFormIsOpen("frmSampleLoad"))
            {
                frmSampleLoad frmSL = new frmSampleLoad();
                frmSL.TopLevel = false;
                frmSL.Parent = this.Parent;
                frmSL.Show();
            }
            else
            {
                frmSampleLoad frmSL = (frmSampleLoad)Application.OpenForms["frmSampleLoad"];
                frmSL.Show();
                frmSL.BringToFront(); 
            }
            if (RunFlag == (int)RunFlagStart.Stoped || RunFlag ==(int)RunFlagStart.NoStart)//move y 20180528 更改此代码的位置，不然窗体close后，后边的代码执行不到。
            {
                frmAddSample.newSample = true; //2018-08-02 zlx add
                this.Close();
            }
        }

        private void btnLoadReagent_Click(object sender, EventArgs e)
        {

            if (!CheckFormIsOpen("frmReagentLoad"))
            {
                frmReagentLoad frmRL = new frmReagentLoad();
                frmRL.TopLevel = false;
                frmRL.Parent = this.Parent;
                frmRL.Show();
            }
            else
            {
                frmReagentLoad frmRL = (frmReagentLoad)Application.OpenForms["frmReagentLoad"];
                frmRL.Show();
                frmRL.BringToFront(); 
            }
            if (RunFlag == (int)RunFlagStart.Stoped||RunFlag==(int)RunFlagStart.NoStart)//move y 20180528 更改此代码的位置，不然窗体close后，后边的代码执行不到。
            {
                frmAddSample.newSample = true; //2018-08-02 zlx add
                this.Close();
            }
          
        }
        private void fbtnTestResult_Click(object sender, EventArgs e)
        {
            //20180506 zlx mod
            if (!CheckFormIsOpen("frmTestResult"))
            {
                frmTestResult frmTR = new frmTestResult();
                frmTR.TopLevel = false;
                frmTR.Parent = this.Parent;
                frmTR.Show();
            }
            else
            {
                frmTestResult frmTR = (frmTestResult)Application.OpenForms["frmTestResult"];
                frmTR.Show();
                frmTR.BringToFront(); 
            }
            if (RunFlag == (int)RunFlagStart.Stoped || RunFlag==(int)RunFlagStart.NoStart)//move y 20180528 更改此代码的位置，不然窗体close后，后边的代码执行不到。
            {
                this.Close();
            }
        }
        /// <summary>
        /// 添加实验进度控件
        /// </summary>
        /// <param name="runFlag">是否正在运行</param>
        private void AddSchedule(bool runFlag)
        {

            //获取datagridview显示的第一行的索引
            int firstViewRowIndex = 0;
            firstViewRowIndex = this.dgvWorkListData.FirstDisplayedScrollingRowIndex;
            //获取向用户显示的datagridview控件的行数
            int viewRows = this.dgvWorkListData.DisplayedRowCount(true);
            for (int i = firstViewRowIndex; i < firstViewRowIndex + viewRows; i++)
            {

                proBar = lisProBar[i];
                #region 获取显示在界面上的进度列表单元格的大小并赋值给进度控件
                proBar.Tag = dgvWorkListData.Rows[i].Cells[6].Tag;
                Rectangle rec = this.dgvWorkListData.GetCellDisplayRectangle(6, i, false);
                proBar.Left = rec.X + 2;
                proBar.Top = rec.Y + 4;
                proBar.Width = rec.Width - 5;
                proBar.Height = rec.Height - 8;
                proBar.Visible = true;
                #endregion
                //添加进度控件
                this.dgvWorkListData.Controls.Add(lisProBar[i]);
            }
            EventControlAdd -= new Action<bool>(AddSchedule);

        }

        private void dgvWorkListData_Scroll(object sender, ScrollEventArgs e)
        {

            while (dgvWorkListData.Controls.Count > 2)
            {
                this.dgvWorkListData.Controls.Clear();//清除已有的控件
            }
            dgvRefresh(false);
        }
        private void frmWorkList_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (RunLightFlag)
            {
                //关闭仪器运行指示灯

                RunLightFlag = false;
            }
            frmMain.btnRunClick -= new Action<object, EventArgs>(TestRun);
            frmSampleLoad.EmergencySample -= new Action(EmergencySampleSch);
            frmMain.btnGoonClick -= new Action(Goon);
            frmMain.btnPauseClick -= new Action(AllPause);
            frmMain.btnStopClick -= new Action(AllStop);
            NetCom3.Instance.ReceiveHandel -= new Action<string>(Instance_ReceiveHandel);
            NetCom3.Instance.EventStop -= new Action(AllStop);
            //NetCom3.MoveTubeError -= this.DisposeMoveAndAddError;//add y 20180723
            AddResetEvent.Set();//add y 20180727
            timeReckon.Stop();
            timer.Enabled = false;
            try
            {
                if (RunFlag == (int)RunFlagStart.IsRuning)
                {
                    if (MoveTubeThread != null)
                    {
                        MoveTubeThread.Abort();

                    }
                    if (washThread != null)
                    {
                        washThread.Abort();
                    }
                    if (AddLiquidThread != null)
                    {
                        AddLiquidThread.Abort();
                    }

                    if (CaculateThread != null)
                    {
                        CaculateThread.Abort();
                    }
                    if (ReadThread != null)
                    {
                        ReadThread.Abort();
                    }
                }
            }
            catch
            {

            }
            finally { RunFlag = (int)RunFlagStart.Stoped; }

        }
        /// <summary>
        /// 同步试剂、底物配置文件信息到数据库
        /// </summary>
        private void IniUpdateAccess()
        {
            #region 将试剂盘配置文件信息更新到数据库
            List<ReagentIniInfo> lisRIinfo = QueryReagentIniInfo();
            if (lisRIinfo.Count > 0)
            {
                foreach (ReagentIniInfo reagentIniInfo in lisRIinfo)
                {
                    //2018-09-04 zlx  mod
                    DbHelperOleDb db = new DbHelperOleDb(3);
                    DbHelperOleDb.ExecuteSql(3,@"update tbReagent set leftoverTestR1 =" + reagentIniInfo.LeftReagent1 + ",leftoverTestR2 = " + reagentIniInfo.LeftReagent2 +
                                              ",leftoverTestR3 = " + reagentIniInfo.LeftReagent3 + ",leftoverTestR4 = " + reagentIniInfo.LeftReagent4 + " where BarCode = '"
                                                  + reagentIniInfo.BarCode + "' and ReagentName = '" + reagentIniInfo.ItemName + "'");
                }
            }
            #endregion
            #region 将底物配置文件信息更新到数据库
            string sbCode1 = OperateIniFile.ReadIniData("Substrate1", "BarCode", "0", iniPathSubstrateTube);
            string sbNum1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", iniPathSubstrateTube);
            DbHelperOleDb dbase = new DbHelperOleDb(3);
            DbHelperOleDb.ExecuteSql(3,@"update tbSubstrate set leftoverTest =" + sbNum1 + " where BarCode = '"
                                                  + sbCode1 + "'");
            //string sbCode2 = OperateIniFile.ReadIniData("Substrate2", "BarCode", "0", iniPathSubstrateTube);
            //string sbNum2 = OperateIniFile.ReadIniData("Substrate2", "LeftCount", "0", iniPathSubstrateTube);
            //DbHelperOleDb.ExecuteSql(@"update tbSubstrate set leftoverTest =" + sbNum2 + " where BarCode = '"
            //                                      + sbCode2 + "'");
            #endregion
            string AddTubeString = "";
            if (AddTubeStop.Count>0)
            {
                foreach (int AddTube in AddTubeStop)
                {
                    AddTubeString = AddTubeString+AddTube + ";";
                }
            }
            OperateIniFile.WriteIniData("Tube", "ReacTrayTub", AddTubeString, iniPathSubstrateTube);
        }
        #endregion

        private void fbtnAddE_Click(object sender, EventArgs e)
        {
            #region 提示
            if (Convert.ToInt16(TimeLabel2.Text) * 60 + Convert.ToInt16(TimeLabel3.Text) < 8)
            {
                MessageBox.Show("实验即将结束，请稍后再继续添加实验！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //if (frmMain.pauseFlag)
            //{
            //    MessageBox.Show("当前供应品缺乏，请保证供应品充足后在进行实验操作！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            if (frmMain.StopFlag[0] || frmMain.StopFlag[1] || frmMain.StopFlag[2] || frmMain.StopFlag[3] || TubeStop || SubstrateStop)
            {
                MessageBox.Show("当前供应品缺乏，请保证供应品充足后在进行实验操作！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            #endregion

            fbtnAddE.Enabled = false;
            while (AddingSampleFlag)
            {
                NetCom3.Delay(10);//如果正在加样步骤，暂时先不会弹出样本装载界面
            }
            EmergencyFlag = true;//2018-10-15 
            frmSampleLoad.DtItemInfoNoStat = GetNoAddLiquid().Copy();
            if (!CheckFormIsOpen("frmSampleLoad"))
            {
                frmSampleLoad frmSL = new frmSampleLoad();
                frmSL.TopLevel = false;
                frmSL.Parent = this.Parent;
                frmSL.Show();
            }
            else
            {
                frmSampleLoad frmSL = (frmSampleLoad)Application.OpenForms["frmSampleLoad"];
                frmSL.BringToFront();
            }
            fbtnAddE.Enabled = true;
        }

        private void fbtnAddS_Click(object sender, EventArgs e)
        {
            #region 提示
            if (Convert.ToInt16(TimeLabel2.Text) * 60 + Convert.ToInt16(TimeLabel3.Text) < 8)
            {
                MessageBox.Show("实验即将结束，请稍后再继续添加实验！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //if (frmMain.pauseFlag)
            //{
            //    MessageBox.Show("当前供应品缺乏，请保证供应品充足后在进行实验操作！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            if (frmMain.StopFlag[0] || frmMain.StopFlag[1] || frmMain.StopFlag[2] || frmMain.StopFlag[3] || TubeStop || SubstrateStop)
            {
                MessageBox.Show("当前供应品缺乏，请保证供应品充足后在进行实验操作！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            #endregion

            fbtnAddS.Enabled = false;
            while (AddingSampleFlag)
            {
                NetCom3.Delay(10);//如果正在加样步骤，暂时先不会弹出样本装载界面
            }
            addOrdinaryFlag = true;//2018-10-18 zlx mod
            frmSampleLoad.DtItemInfoNoStat = GetNoAddLiquid().Copy();
            if (!CheckFormIsOpen("frmSampleLoad"))
            {
                frmSampleLoad frmSL = new frmSampleLoad();
                frmSL.TopLevel = false;
                frmSL.Parent = this.Parent;
                frmSL.Show();
            }
            else
            {
                frmSampleLoad frmSL = (frmSampleLoad)Application.OpenForms["frmSampleLoad"];
                frmSL.BringToFront();
            }
            fbtnAddS.Enabled = true;
        }
        /// <summary>
        /// 获取未开始的实验需要的试剂和样本
        /// </summary>
        /// <returns></returns>
        public DataTable GetNoAddLiquid()
        {
            DataTable dtAddliquid = frmSampleLoad.DtItemInfoNoStat.Clone();
            List<TestSchedule> list = lisTestSchedule.FindAll(tx => (tx.TestID >= NoStartTestId && tx.StartTime > 0 && tx.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddLiquidTube));
            var ItemNames = list.GroupBy(x => x.ItemName).Where(x => x.Count() >= 1).ToList();
            for (int i = 0; i < ItemNames.Count; i++)
            {
                string ItemName = ItemNames[i].Key.ToString();
                DbHelperOleDb db = new DbHelperOleDb(0);
                DataTable dtDilute = DbHelperOleDb.Query(0,@"select DiluteCount,DiluteName from tbProject where ShortName ='" + ItemName + "'").Tables[0];
                List<TestSchedule> ItemNamelist = list.FindAll(tx => (tx.ItemName == ItemName));
                int LeftDiu = 0;
                int leftRg = 0;
                int DiluteCount = int.Parse(dtDilute.Rows[0]["DiluteCount"].ToString());
                string DiluteName = dtDilute.Rows[0]["DiluteName"].ToString();
                foreach (TestSchedule test in ItemNamelist)
                {
                    leftRg++;
                    DataRow[] drRunInfo = dtSampleRunInfo.Select("SampleNo='" + test.SampleNo + "' AND ItemName='" + ItemName + "'");
                    if (!drRunInfo[0]["SampleType"].ToString().Contains("标准品") && !drRunInfo[0]["SampleType"].ToString().Contains("质控品"))
                    {
                        if (int.Parse(test.dilutionTimes) > 1)
                        {
                            int ExtraDiluteC = int.Parse(test.dilutionTimes) / DiluteCount;
                            if (DiluteCount == 1)
                                DiluteName = DiuInfo.GetDiuInfo(ExtraDiluteC);
                            else
                                DiluteName = DiuInfo.GetDiuInfo(ExtraDiluteC) + ";" + DiluteName;
                            List<string> diuList = GetDiuVol(int.Parse(test.AddLiqud.Split('-')[0].ToString()), DiluteName);
                            for (int j = 0; j < diuList.Count; j++)
                            {
                                LeftDiu = LeftDiu + int.Parse(diuList[j].Split(';')[1]);
                            }
                        }
                    }
                }
                DataRow dr = dtAddliquid.NewRow();
                dr["RgName"] = ItemName;
                dr["TestRg"] = leftRg;
                dr["TestDiu"] = LeftDiu;
                dtAddliquid.Rows.Add(dr);
            }
            return dtAddliquid;
        }
        #region 删除生成的工作列表 lyn add 2018.06.16
        private void fbtnDelTest_Click(object sender, EventArgs e)
        {
            NetCom3.ComWait.Reset();
            if (AllowDel())
            {
                LoadingHelper.ShowLoadingScreen();
                //#region 原有数据删除
                //string[] dgvSelectedID = new string[dgvWorkListData.SelectedRows.Count];
                //for (int m = 0; m < dgvWorkListData.SelectedRows.Count; m++)
                //{
                //    dgvSelectedID[m] = dgvWorkListData.SelectedRows[m].Cells["No"].Value.ToString();
                //}
                #region 原有数据删除
                string[] dgvSelectedID = new string[dgvWorkListData.SelectedRows.Count];
                //Jun add
                DbHelperOleDb db = new DbHelperOleDb(1);
                for (int m = 0; m < dgvWorkListData.SelectedRows.Count; m++)
                {
                    string dgvSelectedid = dgvWorkListData.SelectedRows[m].Cells["No"].Value.ToString();
                    string SampleNo = dgvWorkListData.SelectedRows[m].Cells["SampleNo"].Value.ToString();
                    string ItemName = dgvWorkListData.SelectedRows[m].Cells["ItemName"].Value.ToString();
                    lisTestSchedule.RemoveAll(ty => ty.TestID == int.Parse(dgvSelectedid));
                    List<TestSchedule> LeftSchedule = lisTestSchedule.FindAll(ty => ty.SampleNo == SampleNo);
                    if (LeftSchedule.Count > 0)
                    {
                        if (LeftSchedule.FindAll(ty => ty.ItemName == ItemName).Count > 0)
                        {
                            SampleNumCurrent--; //lyq 191021
                            if ((StopList.Count > 0) &&
                                StopList.Contains(dgvWorkListData.SelectedRows[m].Cells["No"].Value.ToString()))
                            {
                                StopList.Remove(dgvWorkListData.SelectedRows[m].Cells["No"].Value.ToString());
                            }
                            continue;
                        }
                        else
                        {
                            var drr = dtSampleRunInfo.Select("SampleNo='" + SampleNo + "'AND ItemName='" + ItemName + "'");
                            if (drr.Length > 0)
                            {
                                dtSampleRunInfo.Rows.Remove(drr[0]);
                            }
                        }
                    }
                    else
                    {
                        var drr = dtSampleRunInfo.Select("SampleNo='" + SampleNo + "'AND ItemName='" + ItemName + "'");
                        if (drr.Length > 0)
                        {
                            dtSampleRunInfo.Rows.Remove(drr[0]);
                        }
                        db = new DbHelperOleDb(1);
                        int bDelete = DbHelperOleDb.ExecuteSql(1,@"delete from tbSampleInfo where Status =0 AND SampleNo='" + SampleNo + "'");
                        if (bDelete > 0)
                        {
                            var ddr = dtSpInfo.Select("SampleNo='" + SampleNo + "'");
                            if (ddr.Length > 0)
                            {
                                dtSpInfo.Rows.Remove(ddr[0]);
                            }
                        }
                        //var drRun = frmParent.dtSampleRunInfo.Select("SampleNo='" + dgvWorkListData.SelectedRows[m].Cells["No"].Value + "'");
                    }
                    //var drRun = frmParent.dtSampleRunInfo.Select("SampleNo='" + dgvWorkListData.SelectedRows[m].Cells["No"].Value + "'AND ItemName=");
                    SampleNumCurrent--;
                    if ((StopList.Count > 0) &&
                                StopList.Contains(dgvWorkListData.SelectedRows[m].Cells["No"].Value.ToString()))
                    {
                        StopList.Remove(dgvWorkListData.SelectedRows[m].Cells["No"].Value.ToString());
                    }
                    #region 屏蔽原有代码
                    /*
                    int bDelete = 0;
                    dgvSelectedID[m] = dgvWorkListData.SelectedRows[m].Cells["No"].Value.ToString();

                    bDelete = DbHelperOleDb.ExecuteSql(@"delete from tbSampleInfo where Status =0 AND SampleNo='" + dgvWorkListData.SelectedRows[m].Cells["SampleNo"].Value + "'");

                    //bDelete = DbHelperOleDb.ExecuteSql(@"update tbSampleInfo set Status = 2  where SampleNo='" + dgvWorkListData.SelectedRows[m].Cells["SampleNo"].Value + "'");

                    if (bDelete > 0)
                    {

                        string temp = dgvWorkListData.SelectedRows[m].Cells["SampleNo"].Value.ToString();
                        var drr = dtSpInfo.Select("SampleNo='" + temp + "'");
                        if (drr.Length > 0)
                        {
                            dtSpInfo.Rows.Remove(drr[0]);
                        }
                        var drRun = frmParent.dtSampleRunInfo.Select("SampleNo='" + temp + "'");
                        foreach (DataRow d in drRun)
                        {
                            frmParent.dtSampleRunInfo.Rows.Remove(d);
                        }
                    }
                     */
                    #endregion
                }
                //for (int n = 0; n < dgvSelectedID.Length; n++)
                //{
                //    lisTestSchedule.RemoveAll(ty => ty.TestID == int.Parse(dgvSelectedID[n]));
                //    SampleNumCurrent--;
                //}

                #endregion
                #region testid和温育盘位置重新赋值
                //按照testid进行排序
                lisTestSchedule.Sort(new SortEmergency());
                int OldAddSamPos = 3;
                DataTable dtReactTrayInfo = OperateIniFile.ReadConfig(iniPathReactTrayInfo);
                //第一个有管的位置
                string FirstPos = "";
                //2018-08-30 zlx mod
                DataRow[] dr = dtReactTrayInfo.Select("Pos='no" + ReactTrayNum.ToString() + "'");
                if (Convert.ToInt32(dr[0][1]) == 1)
                {
                    for (int i = dtReactTrayInfo.Rows.Count - 1; i >= 0; i--)
                    {
                        if (dtReactTrayInfo.Rows[i][1].ToString() != "1")
                        {
                            FirstPos = dtReactTrayInfo.Rows[i + 1][0].ToString();
                            break;
                        }

                    }
                }
                else
                {
                    for (int i = 0; i < dtReactTrayInfo.Rows.Count; i++)
                    {
                        if (dtReactTrayInfo.Rows[i][1].ToString() == "1")
                        {
                            //后一个值一直覆盖前一个最终的赋值为最后一个位置
                            FirstPos = dtReactTrayInfo.Rows[i][0].ToString();
                            if (FirstPos == "no1" || FirstPos == "no2" || FirstPos == "no3")
                                continue;
                            //2018-06-12 zlx mod
                            if (FirstPos == "no4")
                            {
                                for (int num = dtReactTrayInfo.Rows.Count - 1; num > 0; num--)
                                {
                                    if (dtReactTrayInfo.Rows[num][1].ToString() != "0")
                                    {
                                        if (num == ReactTrayHoleNum - 1)
                                            FirstPos = dtReactTrayInfo.Rows[3][0].ToString();
                                        else
                                        {
                                            if (num + 1 - toUsedTube > 4)//dtReactTrayInfo.Rows[num + 1 - 15][0].ToString()!= FirstPos
                                                FirstPos = dtReactTrayInfo.Rows[num + 1][0].ToString();
                                        }
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    }
                }

                //for (int i = 0; i < dtReactTrayInfo.Rows.Count; i++)
                //{
                //    if (dtReactTrayInfo.Rows[i][1].ToString() == "1")
                //    {
                //        //后一个值一直覆盖前一个最终的赋值为最后一个位置
                //        FirstPos = dtReactTrayInfo.Rows[i][0].ToString();
                //        if (FirstPos == "no4")
                //        {
                //            for (int num = dtReactTrayInfo.Rows.Count - 1; num > 0; num--)
                //            {
                //                if (dtReactTrayInfo.Rows[num][1].ToString() == "0")
                //                {
                //                    if (num == ReactTrayHoleNum - 1)
                //                        FirstPos = dtReactTrayInfo.Rows[3][0].ToString();
                //                    else
                //                        FirstPos = dtReactTrayInfo.Rows[num + 1][0].ToString();
                //                    break;
                //                }
                //            }
                //        }
                //        break;
                //    }
                //}
                if (FirstPos == "")
                    OldAddSamPos = 3;
                else
                    OldAddSamPos = int.Parse(FirstPos.Substring(2)) - 1;
                if (NoStartTestId != 1)
                {
                    //被延后的反应管的所在位置
                    OldAddSamPos = lisTestSchedule.Find(ty => ty.TestID == NoStartTestId - 1 &&
                        ty.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddLiquidTube).AddSamplePos;

                }
                List<TestSchedule> lisNoStartSchedule = lisTestSchedule.FindAll(tx => tx.TestID >= NoStartTestId);
                lisTestSchedule.RemoveAll(tx => tx.TestID >= NoStartTestId);
                //testid赋值
                int initID = NoStartTestId;
                for (int i = 1; i <= dgvWorkListData.Rows.Count; i++)
                {
                    List<TestSchedule> TempTestSchedule = lisNoStartSchedule.FindAll(tx => tx.TestID == i);
                    if (TempTestSchedule.Count == 0)
                    {
                        continue;
                    }
                    for (int j = 0; j < TempTestSchedule.Count; j++)
                    {
                        TempTestSchedule[j].TestID = initID;
                    }
                    initID++;
                }
                //温育盘位置赋值
                for (int i = 0; i < lisNoStartSchedule.Count; i++)
                {
                    TestSchedule TempTestSchedule = lisNoStartSchedule[i];
                    if (TempTestSchedule.TestScheduleStep == TestSchedule.ExperimentScheduleStep.AddLiquidTube)
                    {
                        //稀释实验赋值
                        if (int.Parse(TempTestSchedule.dilutionTimes) > 1)
                        {
                            string[] diupos = TempTestSchedule.dilutionPos.Split('-');
                            if (diupos.Length > 1)
                            {
                                string newpos = "";
                                for (int j = 0; j < diupos.Length; j++)
                                {
                                    if (newpos == "")
                                    {
                                        newpos = "1";
                                    }
                                    else
                                    {
                                        newpos += "-" + (j + 1).ToString(); ;
                                    }
                                }
                                TempTestSchedule.dilutionPos = newpos;

                            }
                            else
                            {
                                TempTestSchedule.dilutionPos = "1";
                            }
                            string[] temppos = TempTestSchedule.dilutionPos.Split('-');
                            TempTestSchedule.getSamplePos = "R" + temppos[temppos.Length - 1];
                            OldAddSamPos = OldAddSamPos + 1;
                            if (OldAddSamPos > ReactTrayHoleNum)
                            {
                                OldAddSamPos = 4;
                            }
                            TempTestSchedule.AddSamplePos = OldAddSamPos;
                        }
                        else
                        {
                            OldAddSamPos = OldAddSamPos + 1;
                            if (OldAddSamPos > ReactTrayHoleNum)
                            {
                                OldAddSamPos = 4;
                            }
                            TempTestSchedule.AddSamplePos = OldAddSamPos;
                            TempTestSchedule.getSamplePos = "S" + TempTestSchedule.samplePos;
                        }
                    }
                }
                #endregion
                #region 控件清空还未开始运行的进度条控件
                while (dgvWorkListData.Controls.Count > 2)
                {
                    this.dgvWorkListData.Controls.Clear();//清除已有的控件
                }
                for (int i = BTestItem.Count - 1; i >= NoStartTestId - 1; i--)
                {
                    BTestItem.Remove(BTestItem[i]);
                }
                //清空之前获取已经开始实验的进度条控件
                for (int i = lisProBar.Count - 1; i >= NoStartTestId - 1; i--)
                {
                    lisProBar.Remove(lisProBar[i]);
                }
                #endregion
                //将删除后未开始运行的样本进度附加到datagridview中
                BindData(lisNoStartSchedule, NoStartTestId);
                #region 进度重新计算
                //获取已经开始运行的样本的进度表
                List<TestSchedule> RunSchedule = lisTestSchedule;
                List<string> runFreeTime = GetFreeTime(RunSchedule);
                //未运行的样本进度计算
                List<TestSchedule> lisTestNoRun = ExperimentalScheduleAlgorithm(lisNoStartSchedule, runFreeTime);
                lisTestSchedule.AddRange(lisTestNoRun);
                lisTestSchedule.Sort(new SortRun());
                #endregion
            }
            #region 重新为倒计时进行赋值
            List<TestSchedule> tss = lisTestSchedule.FindAll(tx => tx.StartTime <= sumTime);
            if (tss.Count != 0)
            {
                _GaDoingOne = tss[tss.Count - 1];
                TestStep = GaNextOne();
            }
            LoadingHelper.CloseForm();
            NetCom3.ComWait.Set();
            MaxTime = lisTestSchedule.Select(it => it.EndTime).ToList<int>().Max();
            LastMaxTime = (int)MaxTime;
            MaxTime = MaxTime - sumTime;
            MaxTime *= PiusTimes;
            LastSumTime = sumTime;
            TimeSpan span = new TimeSpan(0, 0, Convert.ToInt32(MaxTime));
            while (!this.IsHandleCreated)
            {
                Thread.Sleep(30);
            }
            TimeLabel2.Invoke(new Action(() =>
            {
                TimeLabel2.Text = ((int)span.TotalHours).ToString("00");
                TimeLabel3.Text = span.Minutes.ToString("00");
                TimeLabel2.Visible = TimeLabel1.Visible = TimeLabel3.Visible = label2.Visible = label3.Visible = true;
            }));
            stopTimer.Start();//倒计时继续
            #endregion
            //StopWatchAddOrSubtractEmergency();//加急诊之后增加倒计时,或者减去试验后减少倒计时
        }

        private void dgvWorkListData_SelectionChanged(object sender, EventArgs e)
        {
            //if (dgvWorkListData.SelectedRows.Count > 0)
            //{
            //    fbtnDelTest.Enabled = true;
            //}
            //else
            //{
            //    fbtnDelTest.Enabled = false;
            //}
        }
        /// <summary>
        /// 是否允许删除选中的行
        /// </summary>
        /// <returns>true为允许；false为不允许</returns>
        bool AllowDel()
        {
            for (int i = 0; i < dgvWorkListData.SelectedRows.Count; i++)
            {
                if (dgvWorkListData.SelectedRows[i].Cells["SampleType"].Value.ToString().Contains("标准品")
                    || dgvWorkListData.SelectedRows[i].Cells["SampleType"].Value.ToString().Contains("定标液"))
                {
                    frmMsgShow.MessageShow("工作列表", "选中的实验中存在标准品，请重新选择！");
                    return false;
                }
                if (dgvWorkListData.SelectedRows[i].Cells["TestStatus"].Value.ToString() != "")
                {
                    frmMsgShow.MessageShow("工作列表", "选中的实验中存在已经开始运行的实验，请重新选择！");
                    return false;
                }
            }
            return true;
        }
        #endregion

        private void btnWorkList_Click(object sender, EventArgs e)
        {

        }

    }
    public class TestResultInfo
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int TestID { get; set; }
        /// <summary>
        /// 光子值
        /// </summary>
        public int PMT { get; set; }
    }
}
