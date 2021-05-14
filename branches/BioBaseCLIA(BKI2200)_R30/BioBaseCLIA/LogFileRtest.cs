using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BioBaseCLIA
{
    class LogFileRtest
    {
        static object myObject = new object();
        private FileStream SW;
        private static LogFileRtest _instance;
        private int testcount;
        private bool bcount = false;
        private string fileName;
        public static LogFileRtest Instance
        {
            get
            {
                lock (myObject)
                {
                    return _instance ?? (_instance = new LogFileRtest());
                }
            }
        }

        public int Testcount
        {
            get => testcount;
            set => testcount = value;
        }
        public bool Bcount
        {
            get => bcount;
            set => bcount = value;
        }
        public string FileName
        {
            get => fileName;
            set => fileName = value;
        }

        public LogFileRtest()
        {
            //SW = new FileStream(Application.StartupPath + @"\Log\NetLog\C" + DateTime.Now.ToString("yyyyMMdd HHmmss") + ".txt", FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 100, FileOptions.Asynchronous);
            SW = new FileStream(Application.StartupPath + @"\仪器调试\老化测试\移管手\M" + DateTime.Now.ToString("yyyyMMdd") + ".txt", FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 100, FileOptions.Asynchronous);
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
}
