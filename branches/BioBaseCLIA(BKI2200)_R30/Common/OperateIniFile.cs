using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Data;
using System.Windows.Forms;

namespace Common
{
    public class OperateIniFile
    {
        #region API函数声明

        [DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);

        [DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern long GetPrivateProfileString(string section, string key,
            string def, StringBuilder retVal, int size, string filePath);

        #endregion

        #region 读Ini文件
        /// <summary>
        /// 读取ini文件
        /// </summary>
        /// <param name="Section">节，部分一般用来总结ini文件中该section下的值的类型</param>
        /// <param name="Key">键值变量</param>
        /// <param name="NoText">一般为空</param>
        /// <param name="iniFilePath">ini文件地址</param>
        /// <returns>从ini文件中读取的值</returns>
        public static string ReadIniData(string Section, string Key, string NoText, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                //取出的值
                StringBuilder temp = new StringBuilder(1024);
                GetPrivateProfileString(Section, Key, NoText, temp, 1024, iniFilePath);
                return temp.ToString();
            }
            else
            {
                return String.Empty;
            }
        }

        static Object Locker = new object();
        /// <summary>
        /// 读取配置文件的所有属性值
        /// </summary>
        /// <param name="iniFilePath">文件存放路径</param>
        /// <returns>查询到的所有信息的DataTable</returns>
        public static DataTable ReadConfig(string iniFilePath)
        {
            if (!File.Exists(iniFilePath))
            {
                return null;
            }

            using (FileStream wfile = new FileStream(iniFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) 
            {
                StreamReader sr = new StreamReader(wfile);
                try
                {
                    DataTable dtIniInfo; dtIniInfo = new DataTable();

                    if (dtIniInfo.Columns.Count < 1)
                    {
                        dtIniInfo.Columns.Add("Pos", typeof(string));
                        dtIniInfo.Columns.Add("Value", typeof(string));
                    }

                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        line = line.Trim();
                        string cName, cValue;
                        string[] cLine = line.Split('=');
                        if (cLine.Length == 2 && dtIniInfo.Columns.Count == 2)
                        {
                            cName = cLine[0].ToLower();
                            cValue = cLine[1].ToLower();
                            dtIniInfo.Rows.Add(cName, cValue);
                        }
                    }
                    return dtIniInfo;
                }
                catch (Exception e)
                {
                    MessageBox.Show("ReadConfig方法读取" + iniFilePath + "出错,错误原因:" + e.Message);
                    return null;
                }
                finally
                {
                    sr.Close();
                    wfile.Close();
                    wfile.Dispose();
                }
            }
        }

        /// <summary>
        /// 读取仪器参数配置文件中字段的值
        /// </summary>
        /// <param name="Section">节，部分一般用来总结ini文件中该section下的值的类型</param>
        /// <param name="Key">键值变量</param>
        /// <returns></returns>
        public static string ReadInIPara(string Section, string Key)
        {
            string iniFilePath = Directory.GetCurrentDirectory() + "\\InstrumentPara.ini";
            if (File.Exists(iniFilePath))
            {
                //取出的值
                StringBuilder temp = new StringBuilder(1024);
                GetPrivateProfileString(Section, Key, "", temp, 1024, iniFilePath);
                return temp.ToString();
            }
            else
            {
                return String.Empty;
            }

        }
        #endregion
        #region 写Ini文件
        /// <summary>
        /// 写Ini文件
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key">键值变量</param>
        /// <param name="Value">键值</param>
        /// <param name="iniFilePath"></param>
        /// <returns>写入是否成功</returns>
        public static bool WriteIniData(string Section, string Key, string Value, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                long OpStation = WritePrivateProfileString(Section, Key, Value, iniFilePath);
                if (OpStation == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 批量写入数据
        /// </summary>
        /// <param name="iniFilePath">写入文件地址</param>
        /// <returns></returns>
        public static bool WriteConfigToFile(string section, string iniFilePath, DataTable dt)
        {
            using (FileStream wfile = new FileStream(iniFilePath, FileMode.Open, FileAccess.Write, FileShare.Read)) 
            {
                StreamWriter sw = new StreamWriter(wfile);
                try
                {
                    sw.WriteLine(section);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sw.WriteLine("{0}={1}", dt.Rows[i]["Pos"].ToString(), dt.Rows[i]["Value"].ToString());
                    }
                }
                catch
                {
                    return false;
                }
                finally
                {
                    sw.Close();
                    wfile.Close();
                    wfile.Dispose();
                }
                return true;
            }
        }

        /// <summary>
        /// 向仪器参数配置文件写入数值
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <param name="Value">写入值</param>
        /// <returns></returns>
        public static bool WriteIniPara(string Section, string Key, string Value)
        {
            string iniFilePath = Directory.GetCurrentDirectory() + "\\InstrumentPara.ini";
            if (File.Exists(iniFilePath))
            {
                long OpStation = WritePrivateProfileString(Section, Key, Value, iniFilePath);
                if (OpStation == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion
    }

    /// <summary>
    /// 试剂盘配置文件信息
    /// </summary>
    public class ReagentIniInfo
    {
        /// <summary>
        /// 试剂条码
        /// </summary>
        public string BarCode { get; set; }
        /// <summary>
        /// 试剂名称
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 试剂批号
        /// </summary>
        public string BatchNum { get; set; }
        /// <summary>
        /// 试剂总测数
        /// </summary>
        public string TestCount { get; set; }
        /// <summary>
        /// 试剂1剩余测数
        /// </summary>
        public int LeftReagent1 { get; set; }
        /// <summary>
        /// 试剂2剩余测数
        /// </summary>
        public int LeftReagent2 { get; set; }
        /// <summary>
        /// 试剂3剩余测数
        /// </summary>
        public int LeftReagent3 { get; set; }
        /// <summary>
        /// 试剂4剩余测数
        /// </summary>
        public int LeftReagent4 { get; set; }

        /// <summary>
        /// 试剂位置 //2019-02-20
        /// </summary>
        public string Postion { get; set; }
        /// <summary>
        /// 试剂装载日期
        /// </summary>
        public string LoadDate { get; set; }
        /// <summary>
        /// 稀释液剩余体积 //2019-02-20
        /// </summary>
        public int leftDiuVol { get; set; }

    }
}
