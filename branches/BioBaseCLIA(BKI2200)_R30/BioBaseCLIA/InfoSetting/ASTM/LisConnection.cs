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
using System.IO.Ports;

namespace BioBaseCLIA.InfoSetting
{
    /// <summary>
    /// LIS串口连接类
    /// </summary>
    class LisConnection
    {
        SerialPort sp;
        public static readonly LisConnection Instance = new LisConnection();
        public System.Threading.Thread thReceive;//通信线程
        /// <summary>
        /// LIS服务器信息传递事件
        /// </summary>
        public event Action<List<string>> ReceiveHandel;
        public DelayClass comDelayer = new DelayClass();
        public EventWaitHandle comWait { get; set; }
        public string EncodeType { get; set; } //2018-4-25 zlxadd
        List<string> _recivelist;
        public CAMessageParser Cmp;
        public string endLine = "\u000d";
        public LisConnection()
        {
            _recivelist = new List<string>();
            Cmp = new CAMessageParser();
        }
        /// <summary>
        /// 打开串口
        /// </summary>
        public void connect()
        {
            string COM = OperateIniFile.ReadInIPara("LisSet", "IPAddress");
            int ComRate = int.Parse(OperateIniFile.ReadInIPara("LisSet", "Port"));
            sp = new SerialPort();
            sp.PortName = COM;
            sp.BaudRate = ComRate;
            sp.Parity = Parity.None;
            sp.StopBits = StopBits.One;
            sp.DataBits = 8;
            sp.RtsEnable = true;
            sp.DtrEnable = true;
            sp.ReceivedBytesThreshold = 1;
            sp.Encoding = System.Text.Encoding.GetEncoding("GB2312");
            sp.DataReceived -= new SerialDataReceivedEventHandler(CommDataReceived); //设置数据接收事件（监听）
            sp.DataReceived += new SerialDataReceivedEventHandler(CommDataReceived); //设置数据接收事件（监听）
            try
            {
                sp.Open();
            }
            catch (Exception e)
            {

                MessageBox.Show("串口打开失败：" + e.Message);
                return;
            }
        }
        /// <summary>
        /// 关闭串口
        /// </summary>
        public void disconnection()
        {

            if (sp.IsOpen)
            {
                try
                {
                    sp.DataReceived -= new SerialDataReceivedEventHandler(CommDataReceived);
                    sp.Close();
                    IsConnected = false;
                }
                catch
                {
                }
            }
        }
        /// <summary>
        /// 判断串口是否为打开转态
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            if (sp != null && sp.IsOpen)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 判断是否与LIS服务器连接
        /// </summary>
        /// <returns></returns>
        public bool IsConnected { get; set; }

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
                    data = Encoding.ASCII.GetBytes(str);
                    break;
                case "UTF8":
                    data = Encoding.UTF8.GetBytes(str);
                    break;
                case "UTF32":
                    data = Encoding.UTF32.GetBytes(str);
                    break;
                case "Unicode":
                    data = Encoding.Unicode.GetBytes(str);
                    break;
                default:
                    data = Encoding.Unicode.GetBytes(str);
                    break;
            }
            try
            {
                sp.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {

                MessageBox.Show(LanguageManager.Instance.getLocaltionStr("SendMessageFail") + e.Message + "！");
            }
        }
        /// <summary>
        /// 通讯正在工作的标志
        /// </summary>
        public bool BWork { get; set; }

        public List<string> ReciveList { get { return _recivelist; } set { _recivelist = value; } }
        /// <summary>
        /// LIS连接返回信息处理类
        /// </summary> 
        public void CommDataReceived(Object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(500);
            string msg = string.Empty;
            byte[] readBuffer;

            try
            {
                //Comm.BytesToRead中为要读入的字节长度
                readBuffer = new Byte[sp.BytesToRead];
                sp.Read(readBuffer, 0, sp.BytesToRead); //将数据读入缓存
                //处理readBuffer中的数据，自定义处理过程
                switch (EncodeType)//2018-4-25 zlxadd
                {
                    case "ASCII":
                        msg = Encoding.ASCII.GetString(readBuffer, 0, readBuffer.Length);
                        break;
                    case "UTF8":
                        msg = Encoding.UTF8.GetString(readBuffer, 0, readBuffer.Length);
                        break;
                    case "UTF32":
                        msg = Encoding.UTF32.GetString(readBuffer, 0, readBuffer.Length);
                        break;
                    case "UNICODE":
                        msg = Encoding.Unicode.GetString(readBuffer, 0, readBuffer.Length);
                        break;
                    default:
                        msg = Encoding.Unicode.GetString(readBuffer, 0, readBuffer.Length);
                        break;
                }
                if (msg != "")
                {
                    if (msg.Contains("ACK^R01")&&msg.Contains("AA")) 
                    {
                        MessageBox.Show("数据发送成功");
                    }

                    if (msg.Contains("QCK^Q02")) 
                    {
                        
                    }

                    if (msg.Contains("DSR^Q03")) 
                    {
                        SetQryResult(msg);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("提示信息", "接收返回消息异常！具体原因：" + ex.Message);
            }
            if (ReceiveHandel != null)
                foreach (Delegate dele in ReceiveHandel.GetInvocationList())
                    try
                    {
                        ((Action<List<string>>)dele).BeginInvoke(ReciveList, null, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        break;
                    }
        }
        /// <summary>
        /// 设置获取到的病人信息
        /// </summary>
        /// <param name="result"></param>
        private void SetQryResult(string result)
        {
            string[] splidate= result.Split('\u000d');
            CAMessageParser.p = new Patient();
            CAMessageParser.rp = new ServeyReport();
            CAMessageParser.rp.ProjectInfo = new List<string>();
            for (int i = 0; i < splidate.Length; i++)
            {
                if (string.IsNullOrEmpty(splidate[i])||!splidate[i].Contains('|')) continue;

                string[] message = splidate[i].Split('|');
                if (message[0] == "MSH" && message[16] != "")
                {
                    CAMessageParser.rp.TestResult = Convert.ToInt32(message[16]);
                }
                if (message[0] == "DSP")
                {
                    switch (Convert.ToInt32(message[1]))
                    {
                        case 1:
                            CAMessageParser.p.Patientid = message[3];
                            //record.Append("Patientid"+ message[3]+"\n");
                            break;
                        case 2:
                            //p.Bedid = Convert.ToInt32(message[3]);
                            //record.Append("Bedid" + message[3] + "\n");
                            break;
                        case 3:
                            CAMessageParser.p.Pname = message[3];
                            //record.Append("Bedid" + message[3] + "\n");
                            break;
                        case 4:
                            CAMessageParser.p.Birth = message[3];
                            //record.Append("Bedid" + message[3] + "\n");
                            break;
                        case 5:
                            if (message[3] == "M")
                                CAMessageParser.p.Sex = 'M';
                            else if (message[3] == "F")
                                CAMessageParser.p.Sex = 'F';
                            else
                                CAMessageParser.p.Sex = 'O';
                            break;
                        case 6:
                            CAMessageParser.p.Blood = message[3];
                            //record.Append("Blood" + message[3] + "\n");
                            break;
                        case 7:
                            CAMessageParser.p.Race = message[3];
                            //record.Append("Race" + message[3] + "\n");
                            break;
                        case 8:
                            CAMessageParser.p.Address = message[3];
                            //record.Append("Address" + message[3] + "\n");
                            break;
                        case 9:
                            CAMessageParser.p.Post = message[3];
                            //record.Append("Post" + message[3] + "\n");
                            break;
                        case 10:
                            CAMessageParser.p.PhoneNum = message[3];
                            //record.Append("PhoneNum" + message[3] + "\n");
                            break;
                        case 11:
                            CAMessageParser.p.Workphone = message[3];
                            //record.Append("Workphone" + message[3] + "\n");
                            break;
                        case 13:
                            CAMessageParser.p.Marriage = message[3];
                            //record.Append("Marriage" + message[3] + "\n");
                            break;
                        case 14:
                            CAMessageParser.p.Region = message[3];
                            //record.Append("Region" + message[3] + "\n");
                            break;
                        case 15:
                            CAMessageParser.p.PatientType = message[3];
                            //record.Append("Race" + message[3] + "\n");
                            break;
                        case 16:
                            CAMessageParser.p.Ybnum = message[3];
                            //record.Append("PatientType" + message[3] + "\n");
                            break;
                        case 17:
                            CAMessageParser.p.FeeType = message[3];
                            //record.Append("FeeType" + message[3] + "\n");
                            break;
                        case 18:
                            CAMessageParser.p.National = message[3];
                            //record.Append("National" + message[3] + "\n");
                            break;
                        case 19:
                            CAMessageParser.p.Origo = message[3];
                            //record.Append("Origo" + message[3] + "\n");
                            break;
                        //case 20:
                        //    CAMessageParser.Country = message[3];
                            //record.Append("Country" + message[3] + "\n");
                            break;
                        case 21:
                            CAMessageParser.rp.PorderNum = message[3];
                            //record.Append("PorderNum" + message[3] + "\n");
                            break;
                        case 22:
                            CAMessageParser.rp.ForderNum = message[3];
                            //record.Append("ForderNum" + message[3] + "\n");
                            break;
                        case 23:
                            CAMessageParser.rp.ReceiveTime = message[3];
                            //record.Append("ReceiveTime" + message[3] + "\n");
                            break;
                        case 24:
                            CAMessageParser.rp.Priority = (message[3].Contains("N") || message[3].Contains("n")) ? false : true;
                            //record.Append("Priority" + message[3] + "\n");
                            break;
                        case 25:
                            //样本采集量
                            CAMessageParser.rp.CollectV = message[3];
                            //record.Append("CollectV" + message[3] + "\n");
                            break;
                        case 26:
                            //样本类型
                            CAMessageParser.rp.Source = message[3];
                            //record.Append("Source" + message[3] + "\n");
                            break;
                        case 27:
                            CAMessageParser.rp.OProvider = message[3];
                            //record.Append("OProvider" + message[3] + "\n");
                            break;
                        case 28:
                            CAMessageParser.rp.OCallbackNum = message[3];
                            //record.Append("OCallbackNum" + message[3] + "\n");
                            break;
                        case 29:
                            CAMessageParser.rp.ProjectInfo.Add(message[3]);
                            //record.Append("ProjectInfo" + message[3] + "\n");
                            break;
                        case 31:
                            CAMessageParser.rp.OProvider = message[3];
                            //record.Append("ProjectInfo" + message[3] + "\n");
                            break;
                        case 32:
                            CAMessageParser.rp.OCallbackNum = message[3];
                            //record.Append("ProjectInfo" + message[3] + "\n");
                            break;
                        default:
                            break;
                    }

                }
            }
        }

        /// <summary>
        /// 分析接收数据
        /// </summary>
        /// <param name="message"></param>
        public void AnalysDate(List<string> list)
        {
            Cmp.ReciveInfo(list);
        }
    }

}
