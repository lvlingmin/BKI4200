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

namespace BioBaseCLIA
{
    class NetCom3
    {

        #region 基础参数
        //frmMessageShow frmMS = new frmMessageShow();
        commands cmd = new commands();
        public static readonly NetCom3 Instance = new NetCom3();
        //IPAddress ipAddress = IPAddress.Parse("192.168.1.146");
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        IPEndPoint remoteEP;
   
        private const int port = 8087;
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
        public string ErrorMessage = null;//2019-02-12 zlx add
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
        public int errorFlag = 0;
        /// <summary>
        /// 移管消息报错状态
        /// </summary>
        public int MoverrorFlag = 0;
        /// <summary>
        /// 加样消息报错状态
        /// </summary>
        public int AdderrorFlag = 0;
        /// <summary>
        /// 清洗消息报错状态
        /// </summary>
        public int WasherrorFlag = 0;
        /// <summary>
        /// 保持连接指令
        /// </summary>
        public bool keepaliveFlag = false;
        /// <summary>
        /// 停止发送指令标志 2018-12-04
        /// </summary>
        public bool stopsendFlag = false;
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
            catch { }
            try
            {
                System.Net.Sockets.UdpClient udpClient = new UdpClient();
                udpClient.Connect(myIpEndPoint);//对远程计算机的指定端口提出UDP连接请求
                udpListen = true;
            }
            catch { }
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
                frmMS.MessageShow("信息提示", "网络连接不可用！");
                frmMS.Dispose();
                return false;
            }
            if (!CheckPort(myip, "5000"))
            {
                frmMessageShow frmMS = new frmMessageShow();
                frmMS.MessageShow("信息提示", "端口关闭！");
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
                    if (connectDone.WaitOne(6000, false))
                    {
                        isConnect = true;
                        totalOrderFlag = true;
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
            catch (Exception e)
            {
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
            catch (Exception e)
            {
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
                Thread.Sleep(50);
                if (isConnect)
                {
                    keepaliveFlag = false;
                    Thread.Sleep(20000);
                    if (SpSendFlag && SpReciveFlag && WashSendFlag && WashReciveFlag && MoveSendFlag && MoveReciveFlag && totalOrderFlag)
                    {
                        keepaliveFlag = true;
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 F1 03"), 5);
                        if (!NetCom3.Instance.SingleQuery())
                        {
                            ConnectServer();
                            keepaliveFlag = false;
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
                */
            }
        }
        #endregion
        #region 公共方法
        public static void Delay(int mm)
        {
            DateTime current = DateTime.Now;
            while (current.AddMilliseconds(mm) > DateTime.Now)
            {
                Thread.Sleep(30);
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
                            default:
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
                        frmMS.MessageShow("消息发送失败提示，Send" + orderType + "：", ex.Message);
                        frmMS.Dispose();
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
                    frmMS.MessageShow("", "指令发送失败,AddArmSendCallback函数：" + e.Message);
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
                    frmMS.MessageShow("", "指令发送失败，WashSendCallback：" + e.Message);
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
                    frmMS.MessageShow("", "指令发送失败，MoveSendCallback：" + e.Message);
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
                        frmMS.MessageShow("", "调试系统通讯故障！");
                        frmMS.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    errorFlag = (int)ErrorState.Recivefailure;
                    if (!keepaliveFlag)
                    {
                        if (!ex.Message.Contains("正在中止线程"))
                        {
                            totalOrderFlag = true;
                            frmMessageShow frmMS = new frmMessageShow();
                            frmMS.MessageShow("", "DiagnostSendCallback:" + ex.Message);
                            frmMS.Dispose();
                        }
                    }

                }
            }
            catch (Exception e)
            {
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
                    frmMS.MessageShow("", "DiagnostSendCallback指令发送失败：" + e.Message);
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
                if (!keepaliveFlag)
                {
                    LogFile.Instance.Write(DateTime.Now + "OtherSendCallback调用了EventStop");
                    //    if (EventStop != null)
                    //        EventStop();
                    //    frmMessageShow frmMS = new frmMessageShow();
                    //    frmMS.MessageShow("", "指令发送失败：" + e.Message);
                    //    frmMS.Dispose();
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
            if (AdderrorFlag != (int)ErrorState.Success)
            {
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
                    LogFile.Instance.Write(DateTime.Now + "加样系统接收数据超时");
                    AdderrorFlag = (int)ErrorState.OverTime;
                    SpReciveFlag = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                AdderrorFlag = (int)ErrorState.Recivefailure;
                SpReciveFlag = true;
                LogFile.Instance.Write(DateTime.Now + "SPReciveMessage:" + ex.Message);
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
                    LogFile.Instance.Write(DateTime.Now + "移管手通讯接收数据超时");
                    MoverrorFlag = (int)ErrorState.OverTime;
                    MoveReciveFlag = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                MoverrorFlag = (int)ErrorState.Recivefailure;
                MoveReciveFlag = true;
                LogFile.Instance.Write(DateTime.Now + "MOVEReciveMessage:" + ex.Message);
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
                return;
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            frmMessageShow frmMsgShow = new frmMessageShow();
            lock (locker)
            {
                if (!isConnect)
                {
                    //dw2018.12.24
                    frmMsgShow.MessageShow("信息提示", "ReceiveCallback isConnect 断开连接！");
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
                        frmMsgShow.MessageShow("信息提示","ReceiveCallback state.workSocket 断开连接！");
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
                        bytesRead = client.EndReceive(ar);
                    }
                    catch (Exception e)
                    {
                        LogFile.Instance.Write(string.Format("{0}<-:{1}", DateTime.Now.ToString("HH:mm:ss:fff"), "client.EndReceive接收异常，异常:"+e.Message));
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
                            frmMsgShow.MessageShow("信息提示","代码Array.Resize出现异常：" + e.Message);
                        }
                        //2018-07-20 zlx add 追踪异常
                        try
                        {
                            state.sb.Append(cmd.ByteArrayToHexString(state.buffer));
                        }
                        catch (Exception e)
                        {
                            frmMsgShow.MessageShow("信息提示","代码 state.sb.Append出现异常：" + e.Message);
                        }
                        response = response + state.sb;
                        LogFile.Instance.Write(string.Format("{0}<-:{1}", DateTime.Now.ToString("HH:mm:ss:fff"), state.sb.ToString()));
                        //2018-10-11 zlx mod
                        ReciveData = new string[state.sb.Length / 48];
                        for (int i = 0; i < state.sb.Length / 48; i++)
                        {
                            ReciveData[i] = response.Substring(i * 48, 48);
                        }
                        response = string.Empty;
                        //int i = response.IndexOf("EB 90 ");//add y 20180917

                        //if (i > -1 && response.Substring(i).Length >= 47)//modify y 20180917
                        //{
                        //    response = response.Substring(i);
                        //    string tempResponse = response.Substring(0, 47);
                        //    response = response.Substring(47);
                        foreach (string tempResponse in ReciveData)
                        {
                            byte WhereToReceive = 1;

                            if (tempResponse.ToString().IndexOf("EB 90") > -1)
                            {
                                string orderTemp = tempResponse.Substring(tempResponse.ToString().IndexOf("EB 90")+6,5);
                                if (orderTemp == "11 AF" || orderTemp == "01 A0" || orderTemp == "11 A0" //仪器调教指令处理//仪器调试收到查询温度
                                    || orderTemp == "A1 03" || orderTemp == "F1 01" || orderTemp == "F1 02" || orderTemp == "F1 03")//心跳包上下位机握手动作完毕//仪器初始化完毕   y modify 20180802  zlx mod 2018-08-16
                                {
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
                                                //bit0:样品盘光电开关未开
                                                //frmMS = new frmMessageShow();
                                                //frmMS.MessageShow("初始化检测", "加样模块样品盘光电开关未开！");
                                                //frmMS.Dispose();
                                                ErrorMessage = ErrorMessage + "加样模块样品盘光耦异常!\n";
                                            }
                                            if (tempII[6] != '1')
                                            {
                                                //bit1:试剂盘光电开关未开
                                                //frmMS = new frmMessageShow();
                                                //frmMS.MessageShow("初始化检测", "加样模块试剂盘光电开关未开！");
                                                //frmMS.Dispose();
                                                ErrorMessage = ErrorMessage + "加样模块试剂盘光耦异常!\n";
                                            }
                                            if (tempII[5] != '1')
                                            {
                                                //bit2:垂直臂光电开关未开
                                                //frmMS = new frmMessageShow();
                                                //frmMS.MessageShow("初始化检测", "加样模块垂直臂光电开关未开！");
                                                //frmMS.Dispose();
                                                ErrorMessage = ErrorMessage + "加样模块垂直臂光耦异常!\n";
                                            }
                                            if (tempII[4] != '1')
                                            {
                                                //bit3:旋转臂光电开关未开
                                                //frmMS = new frmMessageShow();
                                                //frmMS.MessageShow("初始化检测", "加样模块旋转臂光电开关未开！");
                                                //frmMS.Dispose();
                                                ErrorMessage = ErrorMessage + "加样模块旋转臂光耦异常!\n";
                                            }
                                            if (tempII[3] != '1')
                                            {
                                                //bit4:柱塞泵光电开关未开
                                                //frmMS = new frmMessageShow();
                                                //frmMS.MessageShow("初始化检测", "加样模块柱塞泵光电开关未开！");
                                                //frmMS.Dispose();
                                                ErrorMessage = ErrorMessage + "加样模块柱塞泵光耦异常!\n";
                                            }
                                        }

                                        //初始化检测JJ：抓手模块光电信号
                                        string tempJJ = tempResponse.Substring(tempInt + 18, 2);
                                        //判断JJ位为FF
                                        if (tempJJ != "FF")
                                        {
                                            Byte bit = Convert.ToByte(tempJJ, 16);
                                            tempJJ = Convert.ToString(bit, 2);
                                            while (tempJJ.Length < 8)
                                            {
                                                tempJJ = "0" + tempJJ;
                                            }
                                            if (tempJJ[7] != '1')
                                            {
                                                //bit0：温育盘光电开关未开
                                                //frmMS = new frmMessageShow();
                                                //frmMS.MessageShow("初始化检测", "抓手模块温育盘光电开关未开！");
                                                //frmMS.Dispose();
                                                ErrorMessage = ErrorMessage + "抓手模块温育盘光耦异常!\n";
                                            }
                                            if (tempJJ[6] != '1')
                                            {
                                                //bit1：X轴光电开关未开
                                                //frmMS = new frmMessageShow();
                                                //frmMS.MessageShow("初始化检测", "抓手模块X轴光电开关未开！");
                                                //frmMS.Dispose();
                                                ErrorMessage = ErrorMessage + "抓手模块X轴光耦异常!\n";
                                            }
                                            if (tempJJ[5] != '1')
                                            {
                                                //bit2：Y轴光电开关未开
                                                //frmMS = new frmMessageShow();
                                                //frmMS.MessageShow("初始化检测", "抓手模块Y轴光电开关未开！");
                                                //frmMS.Dispose();
                                                ErrorMessage = ErrorMessage + "抓手模块Y轴光耦异常!\n";
                                            }
                                            if (tempJJ[4] != '1')
                                            {
                                                //bit3：Z轴光电开关未开
                                                //frmMS = new frmMessageShow();
                                                //frmMS.MessageShow("初始化检测", "抓手模块Z轴光电开关未开！");
                                                //frmMS.Dispose();
                                                ErrorMessage = ErrorMessage + "抓手模块Z轴光耦异常!\n";
                                            }
                                            if (tempJJ[3] != '1')
                                            {
                                                //bit4：压杯光电开关未开
                                                //frmMS = new frmMessageShow();
                                                //frmMS.MessageShow("初始化检测", "抓手模块压杯光电开关未开！");
                                                //frmMS.Dispose();
                                                ErrorMessage = ErrorMessage + "抓手模块压杯光耦异常!\n";
                                            }
                                            if (tempJJ[2] != '1')
                                            {
                                                //bit5：垂直光电开关未开
                                                //frmMS = new frmMessageShow();
                                                //frmMS.MessageShow("初始化检测", "抓手模块垂直光电开关未开！");
                                                //frmMS.Dispose();
                                                ErrorMessage = ErrorMessage + "抓手模块垂直光耦异常!\n";
                                            }
                                            if (tempJJ[1] != '1')
                                            {
                                                //bit6：抓手光电开关未开
                                                //frmMS = new frmMessageShow();
                                                //frmMS.MessageShow("初始化检测", "抓手模块抓手光电开关未开！");
                                                //frmMS.Dispose();
                                                ErrorMessage = ErrorMessage + "抓手模块抓手光耦异常!\n";
                                            }
                                            if (tempJJ[0] != '1')
                                            {
                                                //bit7：抓空光电开关未开
                                                //frmMS = new frmMessageShow();
                                                //frmMS.MessageShow("初始化检测", "抓手模块抓空光电开关未开！");
                                                //frmMS.Dispose();
                                                ErrorMessage = ErrorMessage + "抓手模块抓空光耦异常!\n";
                                            }

                                        }
                                        //初始化检测KK：清洗模块光电信号
                                        string tempKK = tempResponse.Substring(tempInt + 21, 2);
                                        if (tempKK != "07")
                                        {
                                            Byte bit = Convert.ToByte(tempKK, 16);
                                            tempKK = Convert.ToString(bit, 2);
                                            while (tempKK.Length < 8)
                                            {
                                                tempKK = "0" + tempKK;
                                            }

                                            if (tempKK[7] != '1')
                                            {
                                                //bit0:清洗盘光电开关未开
                                                //frmMS = new frmMessageShow();
                                                //frmMS.MessageShow("初始化检测", "清洗模块清洗盘光电开关未开！");
                                                //frmMS.Dispose();
                                                ErrorMessage = ErrorMessage + "清洗模块清洗盘光耦异常!\n";
                                            }
                                            if (tempKK[6] != '1')
                                            {
                                                //bit1:压杯光电开关未开
                                                //frmMS = new frmMessageShow();
                                                //frmMS.MessageShow("初始化检测", "清洗模块压杯光电开关未开！");
                                                //frmMS.Dispose();
                                                ErrorMessage = ErrorMessage + "清洗模块压杯光耦异常!\n";
                                            }
                                            if (tempKK[5] != '1')
                                            {
                                                //bit2:垂直光电开关未开
                                                //frmMS = new frmMessageShow();
                                                //frmMS.MessageShow("初始化检测", "清洗模块垂直光电开关未开！");
                                                //frmMS.Dispose();
                                                ErrorMessage = ErrorMessage + "清洗模块垂直光耦异常!\n";
                                            }
                                            if (tempKK[4] != '1')
                                            {
                                                //bit3: 柱塞泵光电开关未开;
                                                //frmMS = new frmMessageShow();
                                                //frmMS.MessageShow("初始化检测", "清洗模块柱塞泵光电开关未开！");
                                                //frmMS.Dispose();
                                                ErrorMessage = ErrorMessage + "清洗模块柱塞泵光耦异常!\n";
                                            }
                                        }
                                        //if (ErrorMessage != null)
                                        //{
                                        //    //frmMS = new frmMessageShow();
                                        //    //frmMS.MessageShow("初始化检测结果", ErrorMessage);
                                        //    //frmMS.Dispose();
                                        //    MessageBox.Show(ErrorMessage, "初始化检测故障");
                                        //    ErrorMessage = null;
                                        //}
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
                                        if (temp.Substring(0, 1) != "1")//撞管
                                        {
                                            MoverrorFlag = (int)ErrorState.IsKnocked;
                                        }
                                        if (temp.Substring(1, 1) != "1")//抓空
                                        {
                                            MoverrorFlag = (int)ErrorState.IsNull;
                                        }
                                        if (temp.Substring(3, 1) != "1")//混匀异常
                                        {
                                            MoverrorFlag = (int)ErrorState.IsKnocked;
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
                finally
                {
                   
                }
            }

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
            try
            {

            }
            catch 
            {

            }

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
            /// <summary>
            /// 反应盘配置文件地址
            /// </summary>
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
    }
    // State object for receiving data from remote device. 
    /// <summary>
    /// 信息传输错误状态
    /// 0-正常发送,1-成功 2-发送失败 3-撞针 4-抓空 5-混匀异常 9-发送超时
    /// </summary>
    public enum ErrorState {ReadySend=0, Success = 1, Sendfailure = 2, Recivefailure = 3,IsKnocked = 4, IsNull = 5, BlendUnusua = 6,OverTime=9}
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
                //Fs = new FileStream(Application.StartupPath + @"\Log\AlarmLog\I" + DateTime.Now.ToString("yyyyMMdd") + ".txt", FileMode.Open,FileAccess.ReadWrite,FileShare.Read);
                Fs = new FileStream(Application.StartupPath + @"\Log\AlarmLog\I" + DateTime.Now.ToString("yyyyMMdd") + ".txt", FileMode.Append,FileAccess.Write, FileShare.None);
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
                //byte[] byteArray = System.Text.Encoding.Default.GetBytes(str + Environment.NewLine);
                StreamWriter sw = new StreamWriter(Fs, System.Text.Encoding.Default);// 转码
                sw.Write(str + Environment.NewLine);
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