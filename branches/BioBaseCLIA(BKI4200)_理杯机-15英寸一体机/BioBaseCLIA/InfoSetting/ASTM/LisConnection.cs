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
        public event Action <List<string>> ReceiveHandel;
        public DelayClass comDelayer = new DelayClass();
        public EventWaitHandle comWait { get; set; }
        public string EncodeType { get; set; } //2018-4-25 zlxadd
        List<string> _recivelist;
        public CAMessageParser Cmp;

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
      
        public List<string> ReciveList { get { return _recivelist; } set { _recivelist=value; } }
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
                    if (msg == Cmp.ENQ)
                    {
                        write(Cmp.ACK);
                        BWork = true;
                        ReciveList.Clear();
                        if (ReceiveHandel != null)
                            ReceiveHandel -= new Action<List<string>>(AnalysDate);
                        comWait.Set();
                    }
                    else if (msg == Cmp.ACK)
                    {
                        comWait.Set();
                    }
                    else if (msg == Cmp.EOT)
                    {
                        BWork = false;
                        ReceiveHandel += new Action<List<string>>(AnalysDate);
                    }
                    else
                    {
                        if (msg.Length > 1)
                        {
                            ReciveList.Add(msg);
                            write(Cmp.ACK);
                            comWait.Set();
                        }
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
        /// 分析接收数据
        /// </summary>
        /// <param name="message"></param>
        public void AnalysDate(List<string> list)
        {
            Cmp.ReciveInfo(list);
        }
    }

}
