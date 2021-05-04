using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using BioBaseCLIA.CalculateCurve;
using System.IO;
using Common;
using BioBaseCLIA.User;
using NPOI.Util;
using Res = BioBaseCLIA.Resources.String.NetCom3;

namespace BioBaseCLIA
{
    class NetCom3
    {
        //public static event EventHandler MoveTubeError;//add y 20180720 
        #region 基础参数
        //frmMessageShow frmMS = new frmMessageShow();
        commands cmd = new commands();
        public static readonly NetCom3 Instance = new NetCom3();
        /// <summary>
        /// IP地址
        /// </summary>
        IPAddress ipAddress = IPAddress.Parse(OperateIniFile.ReadInIPara("NetSet", "IPAdress"));
        /// <summary>
        /// 服务器端口号
        /// </summary>     
        private static int port = Convert.ToInt32(OperateIniFile.ReadInIPara("NetSet", "Port"));
        IPEndPoint remoteEP;
        /// <summary>
        /// 创建客户端Socket
        /// </summary>
        Socket client;
        /// <summary>
        /// 返回的消息
        /// </summary>   
        private static String response = String.Empty;
        /// <summary>
        /// 创建锁
        /// </summary>
        static object locker = new object();
        /// <summary>
        ///指令接收数组
        /// </summary>
        public string[] ReciveData = new string[16];
        public string ErrorMessage=null;//2019-02-12 zlx add
        #endregion
        #region 线程、事件
        /// <summary>
        /// 数据接收事件
        /// </summary>
        public event Action<string> ReceiveHandel;
        public event Action<string> ReceiveHandelForQueryTemperatureAndLiquidLevel;
        /// <summary>
        /// 加样系统数据接收线程
        /// </summary>
        Thread SPReciveThread;
        /// <summary>
        /// 移管系统
        /// </summary>
        Thread MOVEReciveThread;
        /// <summary>
        /// 清洗系统
        /// </summary>
        Thread WASHReciveThread;
        /// <summary>
        /// 保持连接线程
        /// </summary>
        Thread KeepAliveThread;
        Thread thDataHandle = null;
        public event Action EventStop;//Stop事件
        #endregion
        #region 各模块信号变量

        /// <summary>
        /// 连接是否完成的信号实例
        /// </summary>  
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        /// <summary>
        /// 加样系统接收是否完成的信号实例
        /// </summary>  
        public static ManualResetEvent spreceiveDone = new ManualResetEvent(false);
        /// <summary>
        /// 移管系统接收是否完成的信号实例
        /// </summary>  
        public static ManualResetEvent movereceiveDone = new ManualResetEvent(false);
        /// <summary>
        /// 清洗系统接收是否完成的信号实例
        /// </summary>  
        public static ManualResetEvent washreceiveDone = new ManualResetEvent(false);
        /// <summary>
        /// 调试系统接收是否完成的信号实例
        /// </summary>  
        public static ManualResetEvent DiagnostDone = new ManualResetEvent(false);
        ///// <summary>
        ///// 温育盘是否被占用（避免加R2和混匀之间插入有关移管手有关需要温育盘的指令，导致温育盘被抢占，混匀错了反应管）
        ///// </summary>
        public static ManualResetEvent ComWait = new ManualResetEvent(true);
        //2018-09-04 zlx add
        public int FReciveCallBack = 0;             
        #endregion
        #region 各模块标志位
        /// <summary>
        /// 测试是否连接服务器端
        /// </summary>
        public static bool isConnect = false;
        /// <summary>
        /// 加样指令发送标志位
        /// </summary>
        public static bool SpSendFlag = true;
        /// <summary>
        /// 加样指令接收标志位
        /// </summary>
        public static bool SpReciveFlag = true;
        /// <summary>
        /// 清洗指令发送标志位
        /// </summary>
        public static bool WashSendFlag = true;
        /// <summary>
        /// 清洗指令接收标志位
        /// </summary>
        public static bool WashReciveFlag = true;
        /// <summary>
        /// 移管指令发送标志位
        /// </summary>
        public static bool MoveSendFlag = true;
        /// <summary>
        /// 移管指令接收标志位
        /// </summary>
        public static bool MoveReciveFlag = true;
        /// <summary>
        /// 是否收到已经发送成功的指令
        /// </summary>
        public static bool totalOrderFlag = true;
        /// <summary>
        /// 其他消息报错状态
        /// </summary>
        public int errorFlag=0;
        /// <summary>
        /// 移管消息报错状态
        /// </summary>
        public int MoverrorFlag=0;
        /// <summary>
        /// 加样消息报错状态
        /// </summary>
        public int AdderrorFlag = 0;
        /// <summary>
        /// 液位探测状态
        /// </summary>
        public Int32 LiquidLevelDetectionFlag = (int)LiquidLevelDetectionAlarm.Height;
        /// <summary>
        /// 清洗消息报错状态
        /// </summary>
        public int WasherrorFlag = 0;
        /// <summary>
        /// 保持连接指令
        /// </summary>
        public bool keepaliveFlag = false;
        /// <summary>
        /// 心跳链接指令是否能够发送成功
        /// </summary>
        public bool isHeartbeatLive = true;
        /// <summary>
        /// 停止发送指令标志 2018-12-04
        /// </summary>
        public bool stopsendFlag = false;
        /// <summary>
        /// IAP流程开始运行、不在循环发送温度缺管查询等指令
        /// </summary>
        public bool iapIsRun = false;
        /// <summary> 
        /// iap此次指令不需要返回
        /// </summary>
        public bool iapNoBack = false;
        public static bool callBack = false;
        #endregion

        public NetCom3()
        {
            //cleanReactionTray = new Thread(new ThreadStart(CleanAllReactTray)) { IsBackground = true };
            remoteEP = new IPEndPoint(ipAddress, port);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            KeepAliveThread = new Thread(new ParameterizedThreadStart(KeepAlive));
            KeepAliveThread.IsBackground = true;
            KeepAliveThread.Start();
            //frmMS = new frmMessageShow();
        }
        #region 服务器连接与判断
        /// <summary>
        /// 获得本机的IP地址
        /// </summary>
        /// <returns></returns>
        public string GetIP()
        {
            string hostNameOrIP = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(hostNameOrIP);
            string sIP = string.Empty;
            if (ipEntry.AddressList.Length > 0)
            {
                foreach (IPAddress addr in ipEntry.AddressList)
                {
                    if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        sIP = addr.ToString();
                        break;
                    }
                }
            }
            return sIP;
        }
        //检测有效的服务器
        public bool CheckNetWorkLink()
        {
            Ping p = new Ping();
            try
            {
                PingReply pr = p.Send(ipAddress);
                if (pr.Status != IPStatus.Success)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 检查端口号是否开启
        /// </summary>
        /// <param name="myip">本机IP地址</param>
        /// <param name="myport">本机端口</param>
        /// <returns></returns>
        public bool CheckPort(string myip, string myport)
        {
            bool tcpListen = false;
            bool udpListen = false;//设定端口状态标识位
            System.Net.IPAddress myIpAddress = IPAddress.Parse(myip);
            System.Net.IPEndPoint myIpEndPoint = new IPEndPoint(myIpAddress, Convert.ToInt32(myport));
            try
            {
                System.Net.Sockets.TcpClient tcpClient = new System.Net.Sockets.TcpClient();
                tcpClient.Connect(myIpEndPoint);//对远程计算机的指定端口提出TCP连接请求搜索//////////////////***********标记：此处可能会引发多线程异步冲突
                tcpListen = true;
            }
            catch(Exception e) 
            {
                writeLog(e);
            }
            try
            {
                System.Net.Sockets.UdpClient udpClient = new UdpClient();
                udpClient.Connect(myIpEndPoint);//对远程计算机的指定端口提出UDP连接请求
                udpListen = true;
            }
            catch(Exception e) 
            {
                writeLog(e);
            }
            if (tcpListen == false && udpListen == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        //联机检测
        public bool CheckMyIp_Port_Link()
        {
            string myip = GetIP();
            //string[] myiparray = myip.Split('.');
            if (!CheckNetWorkLink())
            {
                frmMessageShow frmMS = new frmMessageShow();
                frmMS.MessageShow("", Res.Networkunavailable);
                frmMS.Dispose();
                return false;
            }
            if (!CheckPort(myip, "5000"))
            {
                frmMessageShow frmMS = new frmMessageShow();
                frmMS.MessageShow("", Res.Portclosed);
                frmMS.Dispose();
                return false;
            }
            return true;
        }
        /// <summary>
        /// 连接服务器方法
        /// </summary>
        public void ConnectServer()
        {
            try
            {
                if (!client.Connected)
                {
                    remoteEP = new IPEndPoint(ipAddress, port);
                    client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                    connectDone.Reset();
                    if (connectDone.WaitOne(6000, false))
                    {
                        isConnect = true;
                        totalOrderFlag = true;
                        stopsendFlag = false;
                    }
                    else
                    {
                        isConnect = false;
                        //frmMessageShow frmMS = new frmMessageShow();
                        //frmMS.MessageShow("", "无法连接到服务器！");
                        //frmMS.Dispose();
                        return;

                    }
                }
            }
            catch(Exception e) 
            {
                writeLog(e);
                isConnect = false;
                return;
            }
        }
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                //从状态对象检索套接字  
                Socket client = (Socket)ar.AsyncState;
                // 完成连接    
                client.EndConnect(ar);
                //连接成功
                isConnect = true;
                // 将连接信号置为终止状态  
                connectDone.Set();
            }
            catch(Exception e) 
            {
                writeLog(e);
                isConnect = false;
                // 将连接信号置为终止状态  
                connectDone.Set();
                return;
            }
        }
        /// <summary>
        /// 保持连接
        /// </summary>
        /// <param name="obj"></param>
        void KeepAlive(object obj)
        {
            while (true)
            {
                Thread.Sleep(15);
                if (isConnect)
                {
                    keepaliveFlag = false;
                    Thread.Sleep(20000);
                    while (iapIsRun) //lyq iap 191130
                        Delay(1000);
                    if (SpSendFlag && SpReciveFlag && WashSendFlag && WashReciveFlag && MoveSendFlag && MoveReciveFlag && totalOrderFlag)
                    {
                        keepaliveFlag = true;
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 F1 03"), 5);
                        if (!NetCom3.Instance.SingleQuery())
                        {
                            //ConnectServer();
                            keepaliveFlag = false;
                            isHeartbeatLive = false;
                        }
                        else
                        {
                            isHeartbeatLive = true;
                        }
                    }
                }
                /*
                else
                {
                    if (NetCom3.Instance.CheckMyIp_Port_Link())
                    {
                        NetCom3.Instance.ConnectServer();
                    }
                    if (isConnect)
                        return;
                    else 
                    {
                        frmMessageShow frmMS = new frmMessageShow();
                        frmMS.MessageShow("连接服务器消息", "服务器断开重新连接失败！");
                        frmMS.Dispose();
                    }
                }
                 */
            }
        }

        /// <summary>
        /// 发送心跳包
        /// </summary>
        public void SendHeartbeat()
        {
            if (isConnect)
            {
                keepaliveFlag = false;
                if (SpSendFlag && SpReciveFlag && WashSendFlag && WashReciveFlag && MoveSendFlag && MoveReciveFlag && totalOrderFlag)
                {
                    keepaliveFlag = true;
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 F1 03"), 5);
                    if (!NetCom3.Instance.SingleQuery())
                    {
                        keepaliveFlag = false;
                        isHeartbeatLive = false;
                    }
                    else
                    {
                        isHeartbeatLive = true;
                    }
                }
            }
        }
        #endregion
        #region 公共方法
        public static void Delay(int mm)
        {
            DateTime current = DateTime.Now;
            while (current.AddMilliseconds(mm) > DateTime.Now)
            {
                Thread.Sleep(15);
                Application.DoEvents();
            }
            return;
        }
        /// <summary>
        /// 指令不足16位补位
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Cover(string s)
        {
            string order = s;
            string[] tempOrder = s.Split(' ');
            if (tempOrder.Length < 16)
            {
                for (int i = 0; i < 16 - tempOrder.Length; i++)
                {
                    order += " 00";
                }
            }
            return order;
        }

        /// <summary>
        /// 十六进制转为十进制
        /// </summary>
        /// <param name="reciveData"></param>
        /// <returns></returns>
        public static int[] converTo10(string[] reciveData)
        {
            int[] data = new int[16];
            for (int i = 0; i < 16; i++)
            {
                data[i] = System.Convert.ToInt32("0x" + reciveData[i], 16);
            }
            return data;
        }

        /// <summary>
        /// 单精度类型转为16进制 2018-07-04 zlx add
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FloatToHex(float f)
        {
            var b = BitConverter.GetBytes(f);
            string hex = BitConverter.ToString(b.Reverse().ToArray()).Replace("-", " ");
            //string hex = BitConverter.ToString(b.Reverse().ToArray()).Replace("-", "");
            return hex;
        }
        /// <summary>
        /// 16进制转为单精度类型 2018-07-04 zlx add
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static float HexToFloat(string hex)
        {
            uint num = uint.Parse(hex, System.Globalization.NumberStyles.AllowHexSpecifier);
            byte[] floatVals = BitConverter.GetBytes(num);
            float f = BitConverter.ToSingle(floatVals, 0);
            return f;
        }
        #endregion
        public Thread waitAndAgainSend;//实验的指令重发线程
        #region 发送方法
        /// <summary>
        /// 指令发送方法
        /// </summary>
        /// <param name="order">发送指令</param>
        /// <param name="orderType">发送指令类型，加样：0，移管：1，清洗：2,仪器调试：5</param>
        public void Send(String order, int orderType)
        { 
            //2018-02-21 zlx add
            while (stopsendFlag)
            {
                Delay(100);
                order = "";
            }
            if (order == "") return;
            if (orderType == 0)
            {
                while (!SpReciveFlag || !SpSendFlag)
                {
                    Delay(100);
                }
                SpSendFlag = false;
                SpReciveFlag = false;
                spreceiveDone.Reset();
                AdderrorFlag = (int)ErrorState.ReadySend;
            }
            else if (orderType == 1)
            {
                while (!MoveReciveFlag || !MoveSendFlag)
                {
                    Delay(100);
                }
                MoveSendFlag = false;
                MoveReciveFlag = false;
                movereceiveDone.Reset();
                MoverrorFlag = (int)ErrorState.ReadySend;
            }
            else if (orderType == 2)
            {
                while (!WashReciveFlag || !WashSendFlag)
                {
                    Delay(100);
                }
                WashSendFlag = false;
                WashReciveFlag = false;
                washreceiveDone.Reset();
                WasherrorFlag = (int)ErrorState.ReadySend;
            }
            else if (orderType == 5)
            {
                //DiagnostNum = 16;
                errorFlag = (int)ErrorState.ReadySend;
            }
            lock (this)
            {
                // 使用ASCII编码将字符串数据转换为字节数据   
                byte[] byteData = cmd.HexStringToByteArray(order);
                try
                {
                    while (!totalOrderFlag)
                    {
                        Delay(100);
                    }
                    totalOrderFlag = false;
                    //DiagnostDone.Reset();
                    //add y 20180927 定时接收不到返回的接受指令进行指令重发⬇
                    if (!order.Contains("EB 90"))//2018-01-11 zlx add
                    {
                        switch (orderType)
                        {
                            case 0:
                                if (AdderrorFlag == (int)ErrorState.ReadySend)
                                {
                                    AdderrorFlag = (int)ErrorState.Sendfailure;
                                    SpSendFlag = true;
                                    SpReciveFlag = true;
                                    totalOrderFlag = true;
                                }
                                break;
                            case 1:
                                if (MoverrorFlag == (int)ErrorState.ReadySend)
                                {
                                    MoverrorFlag = (int)ErrorState.Sendfailure;
                                    MoveSendFlag = true;
                                    MoveReciveFlag = true;
                                    totalOrderFlag = true;
                                }
                                break;
                            case 2:
                                if (WasherrorFlag == (int)ErrorState.ReadySend)
                                {
                                    WasherrorFlag = (int)ErrorState.Sendfailure;
                                    WashSendFlag = true;
                                    WashReciveFlag = true;
                                    totalOrderFlag = true;
                                }
                                break;
                            default :
                                if (errorFlag == (int)ErrorState.ReadySend)
                                {
                                    errorFlag = (int)ErrorState.Sendfailure;
                                    totalOrderFlag = true;
                                }
                                break;
                        }
                        return;
                    }
                    if (order.Contains("EB 90 31") || order.Contains("eb 90 31") || order.Contains("Eb 90 31"))
                    {
                        waitAndAgainSend = new Thread(new ParameterizedThreadStart((object obj) =>
                        {
                            string waitOrder = obj as string;
                            if (waitOrder == null || waitOrder == string.Empty) return;
                            for (int i = 0; i < 61; i++)
                            {
                                Thread.Sleep(100);
                            }
                            if (!totalOrderFlag)
                            {
                                switch (orderType)
                                {
                                    case 0:
                                        if (AdderrorFlag == (int)ErrorState.ReadySend)
                                        {
                                            AdderrorFlag = (int)ErrorState.Sendfailure;
                                            SpSendFlag = true;
                                            SpReciveFlag = true;
                                            totalOrderFlag = true;
                                        }
                                        break ;
                                    case 1:
                                        if (MoverrorFlag == (int)ErrorState.ReadySend)
                                        {
                                            MoverrorFlag = (int)ErrorState.Sendfailure;
                                            MoveSendFlag = true;
                                            MoveReciveFlag = true;
                                            totalOrderFlag = true;
                                        }
                                        break;
                                    case 2:
                                        if (WasherrorFlag == (int)ErrorState.ReadySend)
                                        {
                                            WasherrorFlag = (int)ErrorState.Sendfailure;
                                            WashSendFlag = true;
                                            WashReciveFlag = true;
                                            totalOrderFlag = true;
                                        }
                                        break;
                                }
                            }
                            //if (!totalOrderFlag)
                            //{
                            //    byte[] byteData2 = cmd.HexStringToByteArray(waitOrder);
                            //    client.BeginSend(byteData2, 0, byteData2.Length, 0, new AsyncCallback(OtherSendCallback), client);
                            //    LogFile.Instance.Write(string.Format("{0}->:{1}", DateTime.Now.ToString("HH:mm:ss:fff"), order));
                            //}
                        }));
                        waitAndAgainSend.IsBackground = true;
                        waitAndAgainSend.Start(order);
                    }
                    //⬆
                    LogFile.Instance.Write(string.Format("{0}->:{1}", DateTime.Now.ToString("HH:mm:ss:fff"), order));
                    switch (orderType)
                    {
                        case 0:
                            NetCom3.Instance.LiquidLevelDetectionFlag = (int)LiquidLevelDetectionAlarm.Height;//运行前标准置高
                            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(AddArmSendCallback), client);
                            //LogFile.Instance.Write(string.Format("{0}->:{1}", DateTime.Now.ToString("HH:mm:ss:fff"), order));
                            break;
                        case 1:
                            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(MoveSendCallback), client);
                            //LogFile.Instance.Write(string.Format("{0}->:{1}", DateTime.Now.ToString("HH:mm:ss:fff"), order));
                            break;
                        case 2:
                            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(WashSendCallback), client);
                            //LogFile.Instance.Write(string.Format("{0}->:{1}", DateTime.Now.ToString("HH:mm:ss:fff"), order));
                            break;
                        case 5:
                            DiagnostDone.Reset();//modify 20181009 y
                            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(DiagnostSendCallback), client);
                            //LogFile.Instance.Write(string.Format("{0}->:{1}", DateTime.Now.ToString("HH:mm:ss:fff"), order));
                            break;
                        default:
                            DiagnostDone.Reset();
                            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(OtherSendCallback), client);
                            //LogFile.Instance.Write(string.Format("{0}->:{1}", DateTime.Now.ToString("HH:mm:ss:fff"), order));
                            break;
                    }
                    //client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
                    // 开始将数据发送到下位机。
                }
                catch (Exception ex)
                {
                    LogFile.Instance.Write(DateTime.Now + "Send调用了EventStop");
                    writeLog(ex);
                    stopsendFlag = true;
                    if (!totalOrderFlag)
                    {
                        switch (orderType)
                    {
                            case 0:
                                if (AdderrorFlag == (int)ErrorState.ReadySend)
                                {
                                    AdderrorFlag = (int)ErrorState.Sendfailure;
                                    SpSendFlag = true;
                                    SpReciveFlag = true;
                                }
                                break;
                            case 1:
                                if (MoverrorFlag == (int)ErrorState.ReadySend)
                                {
                                    MoverrorFlag = (int)ErrorState.Sendfailure;
                                    MoveSendFlag = true;
                                    MoveReciveFlag = true;
                                }
                                break;
                            case 2:
                                if (WasherrorFlag == (int)ErrorState.ReadySend)
                                {
                                    WasherrorFlag = (int)ErrorState.Sendfailure;
                                    WashSendFlag = true;
                                    WashReciveFlag = true;
                                }
                                break;
                            default:
                                if (errorFlag == (int)ErrorState.ReadySend)
                                {
                                    errorFlag = (int)ErrorState.Sendfailure;
                                }
                                break;
                        }
                    }
                    totalOrderFlag = true;
                    frmMain.LiquidQueryFlag = false;//2018-08-13 zlx add
                    if (!keepaliveFlag)
                    {
                        frmMessageShow frmMS = new frmMessageShow();
                        frmMS.MessageShow(Res.Sendfailed+ " "+ orderType + "：", ex.Message);
                        frmMS.Dispose();
                    }
                }
            }

        }
        public void iapSend(String order, int orderType)
        {
            //2018-02-21 zlx add
            while (stopsendFlag)
            {
                Delay(100);
                order = "";
            }
            if (order == "") return;
            if (orderType == 5)
            {
                if (stopsendFlag)
                    return;
                callBack = false;
                errorFlag = (int)ErrorState.ReadySend;
            }
            lock (this)
            {
                byte[] byteData = cmd.HexStringToByteArray(order);
                try
                {
                    while (!totalOrderFlag)
                    {
                        if (stopsendFlag)
                            return;
                        Delay(100);
                    }
                    totalOrderFlag = false;

                    LogFile.Instance.Write(string.Format("{0}->:{1}", DateTime.Now.ToString("HH:mm:ss:fff"), order));
                    switch (orderType)
                    {
                        case 5:
                            DiagnostDone.Reset();
                            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(DiagnostSendCallback), client);
                            break;
                        default:
                            DiagnostDone.Reset();
                            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(OtherSendCallback), client);
                            break;
                    }
                    callBack = true;
                }
                catch (Exception ex)
                {
                    LogFile.Instance.Write(DateTime.Now + "Send调用了EventStop");
                    stopsendFlag = true;
                    if (!totalOrderFlag)
                    {
                        switch (orderType)
                        {
                            default:
                                if (errorFlag == (int)ErrorState.ReadySend)
                                {
                                    errorFlag = (int)ErrorState.Sendfailure;
                                }
                                break;
                        }
                    }
                    totalOrderFlag = true;
                    if (!keepaliveFlag)
                    {
                        MessageBox.Show( Res.Sendfailed + orderType + "：" + ex.Message, "");
                    }
                }
            }
        }
        private void AddArmSendCallback(IAsyncResult ar)
        {
            try
            {
                // 从状态对象检索套接字。    
                Socket client = (Socket)ar.AsyncState;
                // 完成向下位机发送数据     
                int bytesSent = client.EndSend(ar);
                //SpOrderNum = 1;
                SpSendFlag = true;
                SPReciveThread = new Thread(new ParameterizedThreadStart(SPReciveMessage));
                SPReciveThread.IsBackground = true;
                SPReciveThread.Start();
            }
            catch (Exception e)
            {
                stopsendFlag = true;
                writeLog(e);
                if (!totalOrderFlag && AdderrorFlag == (int)ErrorState.ReadySend)
                {
                    AdderrorFlag = (int)ErrorState.Sendfailure;
                }
                totalOrderFlag = true;
                SpSendFlag = true;
                SpReciveFlag = true;
                if (!keepaliveFlag)
                {
                    LogFile.Instance.Write(DateTime.Now + "AddArmSendCallback调用了EventStop");
                    if (EventStop != null)
                        EventStop();
                    frmMessageShow frmMS = new frmMessageShow();
                    frmMS.MessageShow("",Res.Sendfailed+":" + e.Message);
                    frmMS.Dispose();
                }
            }
        }
        private void WashSendCallback(IAsyncResult ar)
        {
            try
            {
                // 从状态对象检索套接字。    
                Socket client = (Socket)ar.AsyncState;
                // 完成向下位机发送数据     
                int bytesSent = client.EndSend(ar);
                //WashOrderNum = 1;
                WashSendFlag = true;
                WASHReciveThread = new Thread(new ParameterizedThreadStart(WASHReciveMessage));
                WASHReciveThread.IsBackground = true;
                WASHReciveThread.Start();
            }
            catch (Exception e)
            {
                stopsendFlag = true;
                writeLog(e);
                if (!totalOrderFlag && WasherrorFlag == (int)ErrorState.ReadySend)
                    WasherrorFlag = (int)ErrorState.Sendfailure;
                totalOrderFlag = true;
                WashSendFlag = true;
                WashReciveFlag = true;
                if (!keepaliveFlag)
                {
                    LogFile.Instance.Write(DateTime.Now + "WashSendCallback调用了EventStop");
                    if (EventStop != null)
                        EventStop();
                    frmMessageShow frmMS = new frmMessageShow();
                    frmMS.MessageShow("",Res.Sendfailed+ "：" + e.Message);
                    frmMS.Dispose();
                }
            }
        }
        private void MoveSendCallback(IAsyncResult ar)
        {
            try
            {
                // 从状态对象检索套接字。    
                Socket client = (Socket)ar.AsyncState;
                // 完成向下位机发送数据     
                int bytesSent = client.EndSend(ar);
                //MoveOrderNum = 1;
                MoveSendFlag = true;
                MOVEReciveThread = new Thread(new ParameterizedThreadStart(MOVEReciveMessage));
                MOVEReciveThread.IsBackground = true;
                MOVEReciveThread.Start();
            }
            catch (Exception e)
            {
                stopsendFlag = true;
                writeLog(e);
                if (!totalOrderFlag && MoverrorFlag == (int)ErrorState.ReadySend)
                    MoverrorFlag = (int)ErrorState.Sendfailure;
                totalOrderFlag = true;
                MoveSendFlag = true;
                MoveReciveFlag = true;
                if (!keepaliveFlag)
                {
                    LogFile.Instance.Write(DateTime.Now + "MoveSendCallback调用了EventStop");
                    if (EventStop != null)
                        EventStop();
                    frmMessageShow frmMS = new frmMessageShow();
                    frmMS.MessageShow("",Res.Sendfailed+ "：" + e.Message);
                    frmMS.Dispose();
                }
            }
        }
        private void DiagnostSendCallback(IAsyncResult ar)
        {
            try
            {
                // 从状态对象检索套接字。    
                Socket client = (Socket)ar.AsyncState;
                // 完成向下位机发送数据     
                int bytesSent = client.EndSend(ar);
                if (iapNoBack)
                {
                    totalOrderFlag = true;
                    errorFlag = (int)ErrorState.Success;
                    LogFile.Instance.Write(DateTime.Now.ToString("hh:mm:ss:fff") + "DiagnostSendCallback,order no back！");
                    return;
                }
                try
                {
                    StateObject state = new StateObject();
                    state.workSocket = client;
                    // 开始从服务器接收数据
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                    if (!DiagnostDone.WaitOne(60000, false))
                    {
                        LogFile.Instance.Write(DateTime.Now.ToString("hh:mm:ss:fff") + "DiagnostSendCallback调试指令接收出现异常！");
                        errorFlag = (int)ErrorState.OverTime;
                        totalOrderFlag = true;
                        frmMessageShow frmMS = new frmMessageShow();
                        frmMS.MessageShow("",Res.communicationfail);
                        frmMS.Dispose();
                        EventStop.Invoke();
                    }
                }
                catch (Exception ex)
                {
                    errorFlag =(int)ErrorState.Recivefailure;
                    writeLog(ex);
                    if (!keepaliveFlag)
                    {
                        if (!ex.Message.Contains("正在中止线程"))
                        {
                            totalOrderFlag = true;
                            frmMessageShow frmMS = new frmMessageShow();
                            frmMS.MessageShow("","DiagnostSendCallback:"+ex.Message);
                            frmMS.Dispose();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                writeLog(e);
                stopsendFlag = true;
                if (!totalOrderFlag)
                    errorFlag = (int)ErrorState.Sendfailure;
                totalOrderFlag = true;
                if (!keepaliveFlag)
                {
                    LogFile.Instance.Write(DateTime.Now + "DiagnostSendCallback调用了EventStop");
                    if (EventStop != null)
                        EventStop();
                    frmMessageShow frmMS = new frmMessageShow();
                    frmMS.MessageShow("",Res.Sendfailed+"：" + e.Message);
                    frmMS.Dispose();
                }
            }
        }
        private void OtherSendCallback(IAsyncResult ar)
        {
            try
            {
                //dw2018.12.24
                // 从状态对象检索套接字。    
                Socket client = (Socket)ar.AsyncState;
                // 完成向下位机发送数据     
                int bytesSent = client.EndSend(ar);
                if (iapNoBack)
                {
                    totalOrderFlag = true;
                    errorFlag = (int)ErrorState.Success;
                    LogFile.Instance.Write(DateTime.Now.ToString("hh:mm:ss:fff") + "DiagnostSendCallback,order no back！");
                    return;
                }
                //dw2018.12.24
                StateObject state = new StateObject();
                state.workSocket = client;
                // 开始从服务器接收数据
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                if (!DiagnostDone.WaitOne(60000, false))
                {
                    LogFile.Instance.Write(DateTime.Now.ToString("hh:mm:ss:fff") + "系统查询指令接收出现异常！");
                    FReciveCallBack++;
                    errorFlag = (int)ErrorState.OverTime;
                    totalOrderFlag = true;
                  
                }
            }
            catch (Exception e)
            {
                errorFlag = (int)ErrorState.Recivefailure;
                totalOrderFlag = true;
                writeLog(e);
                if (!keepaliveFlag)
                {
                    LogFile.Instance.Write(DateTime.Now + "OtherSendCallback调用了EventStop");
                }
            }
        }
        #endregion

        #region 查询方法
        /// <summary>
        /// 加样系统方法查询
        /// </summary>
        /// <returns></returns>
        public bool SPQuery()
        {
            while (!SpReciveFlag)
            {
                Delay(10);
            }
            if (AdderrorFlag !=(int)ErrorState.Success )
            {
                LogFile.Instance.Write("MoverrorFlag = ： " + MoverrorFlag+ " *****当前 " + DateTime.Now.ToString("HH - mm - ss"));
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 移管系统方法查询
        /// </summary>
        /// <returns></returns>
        public bool MoveQuery()
        {
            while (!MoveReciveFlag)
            {
                Delay(10);
            }
            if (MoverrorFlag != (int)ErrorState.Success)
            {
                LogFile.Instance.Write("MoverrorFlag = ： "+ MoverrorFlag+ "***** 当前 " + DateTime.Now.ToString("HH - mm - ss"));
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 清洗系统方法查询
        /// </summary>
        /// <returns></returns>
        public bool WashQuery()
        {
            while (!WashReciveFlag)
            {
                Delay(10);
            }
            if (WasherrorFlag != (int)ErrorState.Success)
            {
                LogFile.Instance.Write("MoverrorFlag = ： " + MoverrorFlag+ " *****当前 " + DateTime.Now.ToString("HH - mm - ss"));
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 单模块系统方法查询
        /// </summary>
        /// <returns></returns>
        public bool SingleQuery()
        {
            while (!totalOrderFlag)
            {
                Delay(10);
            }
            if (errorFlag != (int)ErrorState.Success)
            {
                LogFile.Instance.Write("MoverrorFlag = ： " + MoverrorFlag + "  *****当前 " + DateTime.Now.ToString("HH - mm - ss"));
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region 接收方法
        /// <summary>
        /// 加样模块数据接收方法
        /// </summary>
        /// <param name="obj"></param>
        void SPReciveMessage(object obj)
        {
            Thread.Sleep(10);
            try
            {
                StateObject state = new StateObject();
                state.workSocket = client;
                // 开始从服务器接收数据
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                if (!spreceiveDone.WaitOne(100000, false))
                {
                    LogFile.Instance.Write(DateTime.Now+"加样系统接收数据超时");
                    AdderrorFlag = (int)ErrorState.OverTime;
                    SpReciveFlag = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                AdderrorFlag = (int)ErrorState.Recivefailure;
                SpReciveFlag = true;
                //LogFile.Instance.Write(DateTime.Now + "SPReciveMessage:"+ex.Message);
                writeLog(ex);
                return;
            }
        }
        /// <summary>
        /// 移管模块数据接收方法
        /// </summary>
        /// <param name="obj"></param>
        void MOVEReciveMessage(object obj)
        {
            Thread.Sleep(10);
            try
            {
                StateObject state = new StateObject();
                state.workSocket = client;
                // 开始从服务器接收数据
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                if (!movereceiveDone.WaitOne(100000, false))
                {
                    LogFile.Instance.Write(DateTime.Now+"移管手通讯接收数据超时");
                    MoverrorFlag = (int)ErrorState.OverTime;
                    MoveReciveFlag=true;
                    return;
                }
            }
            catch (Exception ex)
            {
                MoverrorFlag = (int)ErrorState.Recivefailure;
                MoveReciveFlag=true;
                LogFile.Instance.Write(DateTime.Now + "MOVEReciveMessage:" + ex.Message);
                writeLog(ex);
                return;
            }
        }
        /// <summary>
        /// 清洗模块数据接收方法
        /// </summary>
        /// <param name="obj"></param>
        void WASHReciveMessage(object obj)
        {
            Thread.Sleep(10);
            try
            {
                StateObject state = new StateObject();
                state.workSocket = client;
                // 开始从服务器接收数据
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                if (!washreceiveDone.WaitOne(100000, false))
                {
                    WasherrorFlag = (int)ErrorState.OverTime;
                    WashReciveFlag = true;
                    LogFile.Instance.Write(DateTime.Now + "清洗系统接收数据超时");
                    return;
                }
            }
            catch (Exception ex)
            {
                WasherrorFlag = (int)ErrorState.Recivefailure;
                WashReciveFlag = true;
                LogFile.Instance.Write(DateTime.Now + "WASHReciveMessage:" + ex.Message);
                writeLog(ex);
                return;
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            lock (locker)
            {
                if (!isConnect)
                {
                    //dw2018.12.24
                    MessageBox.Show("ReceiveCallback isConnect "+Res.Disconnect);
                    LogFile.Instance.Write(string.Format("{0}<-:{1}", DateTime.Now.ToString("HH:mm:ss:fff"), "isConnect退出"));
                    //dw2018.12.24
                    return;
                }
                StateObject state = (StateObject)ar.AsyncState;
                try
                {
                    if ((state.workSocket == null) || (!state.workSocket.Connected))
                    {
                        //dw2018.12.24
                        MessageBox.Show("ReceiveCallback state.workSocket " + Res.Disconnect);
                        LogFile.Instance.Write(string.Format("{0}<-:{1}", DateTime.Now.ToString("HH:mm:ss:fff"), "state.workSocket为空退出"));
                        //dw2018.12.24
                        return;
                    }
                    Socket client = state.workSocket;
                    // 读取下位机返回的字节数 
                    int bytesRead=0;
                    try
                    {
                        //保证数据接收完成 Jun add
                        //Thread.Sleep(800);
                        bytesRead = client.EndReceive(ar);
                    }
                    catch (Exception e)
                    {
                        LogFile.Instance.Write(DateTime.Now + "client.EndReceive接收数据异常:" + e.Message);
                        writeLog(e);
                    }
                    if ( bytesRead> 0)
                    {
                        //2018-07-20 zlx add 追踪异常
                        try
                        {
                            Array.Resize(ref state.buffer, bytesRead);
                        }
                        catch (Exception e)
                        {
                            writeLog(e);
                            //MessageBox.Show("代码Array.Resize出现异常："+e.Message);
                        }
                        try
                        {
                            state.sb.Append(cmd.ByteArrayToHexString(state.buffer));
                        }
                        catch (Exception e)
                        {
                            //MessageBox.Show("代码 state.sb.Append出现异常：" + e.Message);
                            writeLog(e);
                        }
                        response = response + state.sb;
                        LogFile.Instance.Write(string.Format("{0}<-:{1}", DateTime.Now.ToString("HH:mm:ss:fff"), state.sb.ToString()));
                        //2018-10-11 zlx mod
                        ReciveData = new string[state.sb.Length / 48];
                        //for (int i = 0; i < state.sb.Length / 48; i++)
                        //{
                        //    ReciveData[i] = response.Substring(i * 48, 48);
                        //}
                        if (response.Contains("EB 90 CA"))
                        {
                            if (response.Contains("EB 90 CA A1 00 00 00") || response.Contains("EB 90 CA A2 00 00 00"))
                            {
                                ReciveData = new string[state.sb.Length / 48];
                                for (int i = 0; i < state.sb.Length / 48; i++)
                                {
                                    ReciveData[i] = response.Substring(i * 48, 48);
                                }
                            }
                            else if (response.Contains("EB 90 CA F1 FF"))
                            {
                                ReciveData = new string[state.sb.Length / 48];
                                for (int i = 0; i < state.sb.Length / 48; i++)
                                {
                                    ReciveData[i] = response.Substring(i * 48, 48);
                                }
                            }
                            else
                            {
                                int len = Convert.ToInt32(response.Substring(response.IndexOf("EB 90 CA"), 14).Substring(12, 2), 16);
                                if (len < 11)
                                    len = 11;
                                len = (len + 5) * 3;

                                ReciveData = new string[state.sb.Length / (len - 1)];
                                //ReciveData[0] = response.Substring(0, len);
                                for (int i = 0; i < state.sb.Length / (len - 1); i++)
                                {
                                    ReciveData[i] = response.Substring(i * len, len);
                                }
                                //MessageBox.Show("state.sb.Length:" + state.sb.Length + "----len:" + len + "\nReciveData[0]:" + ReciveData[0] + "\nReciveData[1]" + ReciveData[1]);
                            }
                        }
                        else
                        {
                            ReciveData = new string[state.sb.Length / 48];
                            for (int i = 0; i < state.sb.Length / 48; i++)
                            {
                                ReciveData[i] = response.Substring(i * 48, 48);
                            }
                        }
                        response = string.Empty;
                        foreach (string tempResponse in ReciveData)
                        {
                            byte WhereToReceive = 1;

                            if (tempResponse.ToString().IndexOf("EB 90") > -1)
                            {
                                string orderTemp = tempResponse.Substring(tempResponse.ToString().IndexOf("EB 90") + 6, 5);     //"xx xx"
                                string orderTemp2 = tempResponse.Substring(tempResponse.ToString().IndexOf("EB 90") + 6, 4); //"XX X"
                                string orderTemp3 = tempResponse.Substring(tempResponse.ToString().IndexOf("EB 90") + 6, 8); //"XX XX XX"
                                if (orderTemp.Substring(0, 2) == "CA" || orderTemp == "CA F1" || orderTemp == "11 FF" || orderTemp == "11 AF" || orderTemp == "01 A0" || orderTemp == "11 A0" //"CA F1"射频读卡器初始化返回//"11 FF"版本号返回指令//仪器调教指令处理//仪器调试收到查询温度
                                    || orderTemp == "A1 03" || orderTemp == "F1 01" || orderTemp == "F1 02" || orderTemp == "F1 03")//心跳包上下位机握手动作完毕//仪器初始化完毕   y modify 20180802  zlx mod 2018-08-16
                                {
                                    if (orderTemp == "01 A0") 
                                    {
                                        HandleLocationData(tempResponse);
                                    }

                                    //dw 2018.12.18
                                    #region 初始化检测模块光电信号开关
                                    if (orderTemp == "F1 02")
                                    {
                                        int tempInt;
                                        //发送的指令EB 90 F1 02，下位机返回的指令中是EB 90 F1 02 FF FF FF FF，需要进行修改具体每一位
                                        //待修改-现在下位机初始化返回的数据还是EB 90 F1 02 FF打头，在下位机初始化返回数据修改后也就是返回数据以EB 90 F1 02 00打头，就可以进行模块光电信号开关的检测
                                        //此部分为光电信号的检测，把所有检测到未开的开关输出到一个弹框内
                                        if (tempResponse.Contains("EB 90 F1 02 FF"))
                                        {
                                            tempInt = tempResponse.IndexOf("EB 90 F1 02 FF ");
                                        }
                                        else
                                        {
                                            tempInt = tempResponse.IndexOf("EB 90 F1 02 00 ");
                                        }
                                        //int tempInt = tempResponse.IndexOf("EB 90 F1 02 00 ");
                                        //初始化检测II：加样模块光电信号
                                        string tempII = tempResponse.Substring(tempInt + 15, 2);
                                        //判断II位为1F
                                        frmMessageShow frmMS;
                                        ErrorMessage = null;
                                        if (tempII != "1F")
                                        {

                                            Byte bit = Convert.ToByte(tempII, 16);
                                            tempII = Convert.ToString(bit, 2);
                                            while (tempII.Length < 8)
                                            {
                                                tempII = "0" + tempII;
                                            }
                                            if (tempII[7] != '1')
                                            {
                                                ErrorMessage = ErrorMessage + Res.Sampleabnormal;
                                            }
                                            if (tempII[6] != '1')
                                            {
                                                ErrorMessage = ErrorMessage + Res.Reagentabnormal;
                                            }
                                            if (tempII[5] != '1')
                                            {
                                                ErrorMessage = ErrorMessage + Res.Verticalanomal;
                                            }
                                            if (tempII[4] != '1')
                                            {
                                                ErrorMessage = ErrorMessage + Res.Rotationabnormal;
                                            }
                                            if (tempII[3] != '1')
                                            {
                                                ErrorMessage = ErrorMessage + Res.Plungerabnormal;
                                            }
                                        }

                                        //初始化检测JJ：抓手模块光电信号
                                        string tempJJ = tempResponse.Substring(tempInt + 18, 2);
                                        //判断JJ位为FF
                                        if (tempJJ != "3F")
                                        {
                                            Byte bit = Convert.ToByte(tempJJ, 16);
                                            tempJJ = Convert.ToString(bit, 2);
                                            while (tempJJ.Length < 8)
                                            {
                                                tempJJ = "0" + tempJJ;
                                            }
                                            if (tempJJ[7] != '1')
                                            {
                                                ErrorMessage = ErrorMessage + Res.Cupabnormal ;
                                            }
                                            if (tempJJ[6] != '1')
                                            {
                                                ErrorMessage = ErrorMessage + Res.Temporaryabnormal ;
                                            }
                                            if (tempJJ[5] != '1')
                                            {
                                                ErrorMessage = ErrorMessage + Res.Verticalphotoelectricabnormal ;
                                            }
                                            if (tempJJ[4] != '1')
                                            {
                                                ErrorMessage = ErrorMessage + Res.Rotatingphotoelectricabnormal ;
                                            }
                                            if (tempJJ[3] != '1')
                                            {
                                                ErrorMessage = ErrorMessage + Res.Gripperabnormal;
                                            }
                                            if (tempJJ[2] != '1')
                                            {
                                                ErrorMessage = ErrorMessage + Res.Emptyabnormal;
                                            }

                                        }
                                        //初始化检测KK：清洗模块光电信号
                                        string tempKK = tempResponse.Substring(tempInt + 21, 2);
                                        if (tempKK != "F")
                                        {
                                            Byte bit = Convert.ToByte(tempKK, 16);
                                            tempKK = Convert.ToString(bit, 2);
                                            while (tempKK.Length < 8)
                                            {
                                                tempKK = "0" + tempKK;
                                            }

                                            if (tempKK[7] != '1')
                                            {
                                                ErrorMessage = ErrorMessage + Res.Cleanabnormal ;
                                            }
                                            if (tempKK[6] != '1')
                                            {
                                                ErrorMessage = ErrorMessage + Res.Pressureabnormal ;
                                            }
                                            if (tempKK[5] != '1')
                                            {
                                                ErrorMessage = ErrorMessage + Res.Cleanverticalabnormal ;
                                            }
                                            if (tempKK[4] != '1')
                                            {
                                                ErrorMessage = ErrorMessage + Res.Measureabnormal ;
                                            }
                                        }

                                        //初始化检测KK：温育盘模块光电信号
                                        string tempMM = tempResponse.Substring(tempInt + 24, 2);
                                        if (tempMM != "7")
                                        {
                                            Byte bit = Convert.ToByte(tempMM, 16);
                                            tempMM = Convert.ToString(bit, 2);
                                            while (tempMM.Length < 8)
                                            {
                                                tempMM = "0" + tempMM;
                                            }

                                            if (tempMM[7] != '1')
                                            {
                                                ErrorMessage = ErrorMessage + Res.Incubateabnormal ;
                                            }
                                            if (tempMM[6] != '1')
                                            {
                                                ErrorMessage = ErrorMessage + Res.Incubateverticalabnormal;
                                            }
                                            if (tempMM[5] != '1')
                                            {
                                                ErrorMessage = ErrorMessage + Res.Incubatepressureabnormal ;
                                            }
                                        }
                                    }
                                    //dw 2018.12.18
                                    #endregion

                                    LogFile.Instance.Write(string.Format("{0}<-:{1}", DateTime.Now.ToString("HH:mm:ss:fff"), "DiagnostDone.Set释放信号之前"));
                                    totalOrderFlag = true;
                                    errorFlag = (int)ErrorState.Success;
                                    DiagnostDone.Set();
                                    if (orderTemp == "11 AF") WhereToReceive = 2;
                                }
                                //下位机收到上位机指令
                                else if (orderTemp == "00 00")//y modify 20180802
                                {
                                    if (waitAndAgainSend is Thread && waitAndAgainSend != null)
                                    {
                                        totalOrderFlag = true;
                                        waitAndAgainSend.Abort();
                                    }
                                }
                                
                                //移管手模块动作执行完毕
                                else if (orderTemp == "31 A1")//20180717 y 增加了撞针等出错处理
                                {
                                    int tempInt = tempResponse.IndexOf("EB 90 31 A1 ");
                                    string temp = tempResponse.Substring(tempInt + 12, 2);
                                    Byte bit = Convert.ToByte(temp, 16);
                                    if (bit != Byte.MaxValue)
                                    {
                                        temp = Convert.ToString(bit, 2);
                                        while (temp.Length < 8)
                                        {
                                            temp = "0" + temp;
                                        }
                                        if (temp.Substring(0, 1) == "0")//为空
                                        {
                                            MoverrorFlag = (int)ErrorState.IsNull;
                                        }
                                        if (temp.Substring(1, 1) == "0")//取管撞管
                                        {
                                            MoverrorFlag = (int)ErrorState.IsKnocked;
                                        }
                                        if (temp.Substring(2, 1) == "0")//放管撞管
                                        {
                                            MoverrorFlag = (int)ErrorState.putKnocked;
                                        }
                                        if (temp.Substring(4, 1) == "0")//理杯机缺管
                                        {
                                            MoverrorFlag = (int)ErrorState.LackTube;
                                        }
                                    }
                                    else
                                    {
                                        MoverrorFlag = (int)ErrorState.Success;//成功
                                    }
                                    MoveReciveFlag = true;
                                    movereceiveDone.Set();
                                }
                                //加样系统动作执行完毕
                                else if (orderTemp == "31 A2")//20180717 y 增加了撞针等出错处理
                                {
                                    int tempInt = tempResponse.IndexOf("EB 90 31 A2 ");
                                    string temp = tempResponse.Substring(tempInt + 12, 2);
                                    if (temp == "7F")//加液针撞针
                                    {
                                        AdderrorFlag = (int)ErrorState.IsKnocked;
                                    }
                                    else
                                    {
                                        AdderrorFlag = (int)ErrorState.Success;
                                    }
                                    LiquidLevelDetectionFlag = (int)((Convert.ToInt32(tempResponse.Substring(tempInt + 15, 2) +
                                        tempResponse.Substring(tempInt + 18, 2), 16) > 0) ? LiquidLevelDetectionAlarm.Height : LiquidLevelDetectionAlarm.Low);
                                    SpReciveFlag = true;
                                    spreceiveDone.Set();
                                }
                                else if (orderTemp == "31 A4")//混匀完成指令
                                {
                                    AdderrorFlag = (int)ErrorState.Success;//成功
                                    SpReciveFlag = true;
                                    spreceiveDone.Set();
                                }
                                //清洗系统动作执行完毕
                                else if (orderTemp == "31 A3")
                                {
                                    WasherrorFlag = (int)ErrorState.Success;
                                    WashReciveFlag = true;
                                    washreceiveDone.Set();
                                }
                                //IAP过程相关指令返回
                                else if (orderTemp == "55 01" || orderTemp == "0A A0" || orderTemp == "6A A6" || orderTemp2 == "B0 0")
                                {
                                    if (orderTemp3 == "B0 03 FF") //开始传输IAP文件
                                    {
                                        LogFile.Instance.Write(string.Format("{0}<-:{1}", DateTime.Now.ToString("HH:mm:ss:fff"), "IAP开始进行文件传输烧录"));

                                    }
                                    else if (orderTemp3 == "B0 04 FF")
                                    {
                                        LogFile.Instance.Write(string.Format("{0}<-:{1}", DateTime.Now.ToString("HH:mm:ss:fff"), "IAP文件已传输完毕"));
                                    }
                                    else if (orderTemp3 == "B0 04 01")
                                    {
                                        LogFile.Instance.Write(String.Format("{0}<-:{1}", DateTime.Now.ToString("HH:mm:ss:fff"), "IAP程序烧录失败"));
                                    }
                                    LogFile.Instance.Write(string.Format("{0}<-:{1}", DateTime.Now.ToString("HH:mm:ss:fff"), "DiagnostDone.Set释放信号之前"));
                                    totalOrderFlag = true;
                                    errorFlag = (int)ErrorState.Success;
                                    DiagnostDone.Set();
                                }
                                if (WhereToReceive == 1)
                                {
                                    thDataHandle = new Thread(new ParameterizedThreadStart(HandleMessage));
                                    thDataHandle.IsBackground = true;
                                    thDataHandle.Start(tempResponse);
                                }
                                else if (WhereToReceive == 2)
                                {
                                    thDataHandle = new Thread(new ParameterizedThreadStart(HandleMessageForTemperatureAndLiquidLevel));
                                    thDataHandle.IsBackground = true;
                                    thDataHandle.Start(tempResponse);
                                }
                                //response = string.Empty;
                                if (orderTemp == "00 00")
                                {
                                    state.sb.Remove(0, state.sb.Length);
                                    state.buffer = new byte[StateObject.BufferSize];
                                    if (client.Connected)
                                    {
                                        // 获取其余的数据  
                                        client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                                        //Thread.Sleep(300);
                                    }
                                    LogFile.Instance.Write(DateTime.Now + ":0");
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogFile.Instance.Write(DateTime.Now + "ReceiveCallback调用了EventStop,异常：" + e.Message);
                    writeLog(e);
                    stopsendFlag = true;
                    if (waitAndAgainSend is Thread && waitAndAgainSend != null)
                    {
                        totalOrderFlag = true;
                        waitAndAgainSend.Abort();
                    }
                    if (state.sb.Length > 0)
                    {
                        state.sb.Remove(0, state.sb.Length);
                        state.buffer = new byte[StateObject.BufferSize];
                        if (client.Connected)
                        {
                            //获取其余的数据  
                            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                        }
                    }
                    close();
                }
            }

        }

        /// <summary>
        /// 保存下位机返回位置数据
        /// </summary>
        /// <param name="receiveData">返回位置数据</param>
        private void HandleLocationData(string receiveData) 
        {
            string stateData = receiveData.Substring(18, 5);

            if (stateData == "00 00") return;

            string saveData = receiveData.Substring(18);
            string saveDataSection = stateData.Substring(0, 2);
            string saveKey = receiveData.Substring(18, 11);

            OperateIniFile.WriteIniData(saveDataSection, saveKey, receiveData.Substring(18),
                Directory.GetCurrentDirectory() + "\\LocationData.ini");
        }

        object lockHand = new object();
        /// <summary>
        /// 处理接收到的数据
        /// </summary>
        private void HandleMessage(object message)
        {
            lock (lockHand)
            {
                //将接收的数据传到相应的界面
                if (ReceiveHandel != null)
                    foreach (Delegate dele in ReceiveHandel.GetInvocationList())
                        try
                        {
                            ((Action<string>)dele).BeginInvoke(message.ToString(), null, null);
                        }
                        catch (Exception ex)
                        {
                            writeLog(ex);
                            frmMessageShow frmMS = new frmMessageShow();
                            frmMS.MessageShow("", ex.Message);
                            frmMS.Dispose();
                            break;
                        }
            }
        }
        object locker1 = new object();
        private void HandleMessageForTemperatureAndLiquidLevel(object message)
        {
            lock (locker1)
            {
                if (ReceiveHandelForQueryTemperatureAndLiquidLevel != null)
                {
                    foreach (Delegate item in ReceiveHandelForQueryTemperatureAndLiquidLevel.GetInvocationList())
                    {
                        ((Action<string>)item).BeginInvoke(message.ToString(), null, null);
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// 断开连接
        /// </summary>
        public void DisConnect()
        {
            if (client != null)
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                isConnect = false;
            }
        }
        /// <summary>
        /// 断开连接停止相应线程
        /// </summary>
        public void close()
        {
            if (client != null)
            {
                DisConnect();
                isConnect = false;
            }
        }
        /// <summary>
        /// 清空温育盘所有管
        /// </summary>
        public void CleanAllReactTray()
        {
            string iniPathReactTrayInfo = Directory.GetCurrentDirectory() + "\\ReactTrayInfo.ini";
            Run.frmWorkList.TrayRemoveAllTube = true;//y add 抓空标志位，保证不触发抓空异常
            for (int i = 1; i < 81; i++)
            {
                Send(NetCom3.Cover("EB 90 31 01 05 " + i.ToString("x2")), 1);
                //修改反应盘信息
                OperateIniFile.WriteIniData("ReactTrayInfo", "no" + i, "0", iniPathReactTrayInfo);
                MoveQuery();
            }
            Run.frmWorkList.TrayRemoveAllTube = false;//y add 抓空标志位，保证不触发抓空异常
        }
        /// <summary>
        /// IAP重连
        /// </summary>
        public void iapConnectTwice()
        {
            try
            {
                DisConnect();
                Delay(3000);
                Socket sc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                if (!sc.Connected)
                {
                    remoteEP = new IPEndPoint(ipAddress, port);
                    sc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    sc.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), sc);

                    if (connectDone.WaitOne(6000, false))
                    {
                        isConnect = true;
                        totalOrderFlag = true;
                    }
                    else
                    {
                        isConnect = false;
                        return;
                    }
                }
                client = sc;
            }
            catch (Exception e)
            {
                LogFile.Instance.Write(DateTime.Now + "connectCatchENd");
                writeLog(e);
                isConnect = false;
                return;
            }
        }
        /// <summary>
        /// 写程序异常文件
        /// </summary>
        /// <param name="str"></param>
        public void writeLog(Exception ex)
        {
            string str = string.Format("异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n",
                        ex.GetType().Name, ex.Message, ex.StackTrace);
            LogFile.Instance.Write(DateTime.Now + str);
        }
    }
    // State object for receiving data from remote device. 
    /// <summary>
    /// 信息传输错误状态
    /// 0-准备发送,1-成功 2-发送失败 3-接收失败 4-抓管撞管（撞针） 5-抓空 6-混匀异常 7-放管撞管 8-理杯机缺管 9-发送超时
    /// </summary>
    public enum ErrorState { ReadySend = 0, Success = 1, Sendfailure = 2, Recivefailure = 3, IsKnocked = 4, IsNull = 5, BlendUnusua = 6, putKnocked=7,LackTube=8,OverTime = 9 }
    /// <summary>
    /// 液位检测状态
    /// >100  Height
    /// <=100 Low
    /// </summary>
    public enum LiquidLevelDetectionAlarm :int { Low = 0, Height = 1 };
    public class StateObject
    {
        // Client socket.     
        public Socket workSocket = null;
        // Size of receive buffer.     
        public const int BufferSize = 1024;
        // Receive buffer.     
        public byte[] buffer = new byte[BufferSize];
        // Received data string.     
        // Received data string.     
        public StringBuilder Spsb = new StringBuilder();
        public StringBuilder Movesb = new StringBuilder();
        public StringBuilder Washsb = new StringBuilder();
        public StringBuilder sb = new StringBuilder();
    }
    /// <summary>
    /// 日志
    /// </summary>
    public class LogFile
    {
        static object myObject = new object();
        private FileStream SW;
        private static LogFile _instance;
        public static LogFile Instance
        {
            get
            {
                lock (myObject)
                {
                    return _instance ?? (_instance = new LogFile());
                }
            }
        }
        public LogFile()
        {
            SW = new FileStream(Application.StartupPath + @"\Log\NetLog\C" + DateTime.Now.ToString("yyyyMMdd HHmmss") + ".txt", FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 100, FileOptions.Asynchronous);
            //SW = new FileStream(Application.StartupPath + @"\Log\C" + DateTime.Now.ToString("yyyyMMdd hhmmss") + ".txt");
            //SW.AutoFlush = true;
        }
        public void Write(string str)
        {
            lock (myObject)
            {
                byte[] byteArray = System.Text.Encoding.Default.GetBytes(str + Environment.NewLine);
                SW.BeginWrite(byteArray, 0, byteArray.Length, null, null);
                //SW.WriteLine(str);
                SW.Flush();
            }
        }
        public void Close()
        {
            SW.Flush();
            SW.Close();
        }
    }
    /// <summary>
    /// 报警信息记录
    /// </summary>
    public class LogFileAlarm
    {
        static object myObject = new object();
        private FileStream Fs;
        private static LogFileAlarm _instance;
        public static LogFileAlarm Instance
        {
            get
            {
                lock (myObject)
                {
                    return _instance = new LogFileAlarm();
                }
            }
        }
        public LogFileAlarm()
        {
            if (File.Exists(Application.StartupPath + @"\Log\AlarmLog\I" + DateTime.Now.ToString("yyyyMMdd") + ".txt"))
            {
                Fs = new FileStream(Application.StartupPath + @"\Log\AlarmLog\I" + DateTime.Now.ToString("yyyyMMdd") + ".txt", FileMode.Append, FileAccess.Write, FileShare.Read);
                //Fs = new FileStream(Application.StartupPath + @"\Log\AlarmLog\I" + DateTime.Now.ToString("yyyyMMdd") + ".txt", FileMode.Open,FileAccess.ReadWrite,FileShare.Read );
            }
            else
            {
                Fs = new FileStream(Application.StartupPath + @"\Log\AlarmLog\I" + DateTime.Now.ToString("yyyyMMdd") + ".txt", FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 100, FileOptions.Asynchronous);
            }
        }
        public void Write(string str)
        {
            lock (myObject)
            {
                StreamWriter sw = new StreamWriter(Fs, System.Text.Encoding.Default);//转码
                sw.WriteLine(str);
                sw.Flush();
                sw.Close();
                Fs.Close();
            }
        }
        public void Close()
        {
            Fs.Flush();
            Fs.Close();
        }
    }
}