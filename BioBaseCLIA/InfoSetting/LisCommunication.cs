using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Localization;
using Common;

namespace BioBaseCLIA.InfoSetting
{
    class LisCommunication
    {

        public static readonly LisCommunication Instance = new LisCommunication();
        public System.Threading.Thread thReceive;//通信线程
        /// <summary>
        /// LIS服务器信息传递事件
        /// </summary>
        public event Action<string> ReceiveHandel;
        TcpClient client = new TcpClient();//为TCP网络服务提供客户端连接
        NetworkStream stream;
        public DelayClass comDelayer = new DelayClass();
        public EventWaitHandle comWait { get; set; }
        public string EncodeType { get; set; } //2018-4-25 zlxadd
        //LISLogFile LogFile = new LISLogFile();
        LISLogFile LogFile = LISLogFile.Instance;
        public LisCommunication()
        {
        }
        /// <summary>
        /// LIS服务器连接
        /// </summary>
        public void connect(string IP,int port)
        {

            //string IP = OperateIniFile.ReadInIPara("LisSet", "IPAddress");
            //int port = int.Parse(OperateIniFile.ReadInIPara("LisSet", "Port"));
            try
            {
                client = new TcpClient(IP, port);
                stream = client.GetStream();
                thReceive = new System.Threading.Thread(new System.Threading.ThreadStart(Run));
                thReceive.IsBackground = true;
                thReceive.Start();
            }
            catch (Exception e) {

                MessageBox.Show("Lis服务器无法连接" + "：" + e.Message);
                return;
            }


        }
        /// <summary>
        /// LIS服务器断开连接
        /// </summary>
        public void disconnection() {

            if(IsConnect())
            client.Close();
        
        }

        /// <summary>
        /// 判断是否与LIS服务器连接
        /// </summary>
        /// <returns></returns>
        public bool IsConnect() {

            if (client.Connected)
                return true;
            else
                return false;
        
        
        }

       /// <summary>
       /// 向LIS服务器发送消息
       /// </summary>
       /// <param name="str"></param>
        public void write(string str)
        {
            LISLogFile.Instance.Write("Send:" + str);
            byte[] data;
            switch (EncodeType)//2018-4-25 zlxadd
            {
                case "ASCII":
                    data=Encoding.ASCII.GetBytes(str);
                    break;
                case "UTF8":
                     data=Encoding.UTF8.GetBytes(str);
                    break;
                case "UTF32":
                     data=Encoding.UTF32.GetBytes(str);
                    break;
                case "Unicode":
                     data=Encoding.Unicode.GetBytes(str);
                    break;
                default:
                     data=Encoding.Unicode.GetBytes(str);
                break ;
            }

            //byte[] data = Encoding.Unicode.GetBytes(str);
            try
            {
                stream.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {

                MessageBox.Show("发送消息失败" +e.Message+ "！");
            }



        }
        /// <summary>
        /// LIS连接返回信息处理类
        /// </summary>
        private void Run()
        {
            string msg = string.Empty;
            byte[] data = new byte[8000];


            while (true)
            {
                if (IsConnect())
                {
                    try
                    {
                        if (stream != null)
                        {
                            Int32 bytes = stream.Read(data, 0, data.Length);
                            //msg = Encoding.Unicode.GetString(data, 0, bytes);
                            switch (EncodeType)//2018-4-25 zlxadd
                            {
                                case "ASCII":
                                    msg = Encoding.ASCII.GetString(data, 0, bytes);
                                    break;
                                case "UTF8":
                                    msg = Encoding.UTF8.GetString(data, 0, bytes);
                                    break;
                                case "UTF32":
                                    msg = Encoding.UTF32.GetString(data, 0, bytes);
                                    break;
                                case "UNICODE":
                                    msg = Encoding.Unicode.GetString(data, 0, bytes);
                                    break;
                                default:
                                    msg = Encoding.Unicode.GetString(data, 0, bytes);
                                    break;
                            }
                            if (msg != "" && ReceiveHandel != null)
                            {
                                ReceiveHandel += AnalysDate;
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        break;


                    }

                    if (ReceiveHandel != null)
                        foreach (Delegate dele in ReceiveHandel.GetInvocationList())
                            try
                            {
                                ((Action<string>)dele).BeginInvoke(msg, null, null);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                break;
                            }

                }
            }
        }
       
        /// <summary>
        /// 分析接收数据
        /// </summary>
        /// <param name="message"></param>
        public void AnalysDate(string message)
        {
            if (message == "") return;
            
            CMessageParser Cmp = new CMessageParser();
            string[] spdate = Cmp.SplitMessage(message);
            string[] DMSH = spdate[0].Split('|');
            string[] DMSA = spdate[1].Split('|');
            string _errorinfo = "";
            if (DMSA[6] != "")
            {
                _errorinfo = Cmp.ErrorInfo(DMSA[1], Convert.ToInt32(DMSA[6]));
            }
            else
            {
                _errorinfo = Cmp.ErrorInfo(DMSA[1], 0);
            }
            switch (DMSH[8])
            {
                case "ACK^R01":
                    if (_errorinfo != "")
                    {
                        //MessageBox.Show("编号为" + CMessageParser.ConstrolID + "消息发送结果为:"+ _errorinfo);
                        //LisCommunication.Instance.comWait.Set();
                        LogFile.Write(DateTime.Now + "消息编号为" + CMessageParser.ConstrolID + "的反馈结果为：" + _errorinfo);
                    }
                    break;
                case "QCK^Q02":
                      if (_errorinfo != "")
                        LogFile.Write("编号为" + CMessageParser.ConstrolID + "消息发送结果为:" + _errorinfo);
                        //LogFile.Write(DateTime.Now + "消息编号为" + CMessageParser.ConstrolID + "的反馈结果为：" + _errorinfo);
                    break;
                case "DSR^Q03":
                    if (_errorinfo != "")
                        LogFile.Write("编号为" + CMessageParser.ConstrolID + "消息发送结果为:" + _errorinfo);
                        //LogFile.Write(DateTime.Now + "消息编号为" + CMessageParser.ConstrolID + "的反馈结果为：" + _errorinfo);
                    if (DMSA[1] == "OK")
                        Cmp.ReciveInfo(spdate);
                    Cmp.AcceptType = "AA";
                    Cmp.Mtype = "ACK^Q03";
                    Cmp.SendACK();
                    break;
                default:
                    break;
            }

        }
    }

    /// <summary>
    /// LIS日志
    /// </summary>
    public class LISLogFile
    {
        static object myObject = new object();
        private FileStream SW;
        private static LISLogFile _instance;
        public static LISLogFile Instance
        {
            get
            {
                lock (myObject)
                {
                    return _instance ?? (_instance = new LISLogFile());
                }
            }
        }
        public LISLogFile()
        {
            SW = new FileStream(Application.StartupPath + @"\Log\LisLog\LIS" + DateTime.Now.ToString("yyyyMMdd hhmmss") + ".txt",
                FileMode.Create, FileAccess.Write, FileShare.ReadWrite,100, FileOptions.Asynchronous);//, FileShare.ReadWrite,4096, FileOptions.Asynchronous
            //SW = new FileStream(Application.StartupPath + @"\Log\C" + DateTime.Now.ToString("yyyyMMdd hhmmss") + ".txt");
            //SW.AutoFlush = true;
        }
        public void Write(string str)
        {
            lock (myObject)
            {
                byte[] byteArray = System.Text.Encoding.Default.GetBytes(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + "      " 
                                                                         + str + Environment.NewLine + Environment.NewLine);
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
}
