using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maticsoft.DBUtility;
using BioBaseCLIA.Model;
using BioBaseCLIA.Run;
using System.Data.OleDb;
using System.Data;
using System.Windows.Forms;
using System.Threading;
using Common;
namespace BioBaseCLIA.InfoSetting
{
    /// <summary>
    /// 串口Lis处理
    /// </summary>
    public class CAMessageParser : ASTM
    {
        public static Patient p { get; set; }
        public static ServeyReport rp { get; set; }
        int _dalaytime = 6000;
        /// <summary>
        /// 发送延迟
        /// </summary>
        public int Dalaytime
        {
            get { return _dalaytime; }
            set { _dalaytime = value; }
        }
        /// <summary>
        /// 样本编号
        /// </summary>
        public string SampleNo { get; set; }
        /// <summary>
        /// 应用程序应答类型，发送结果
        /// <param name="0">0病人样本测试结果</param> 
        /// <param name="1">1校准结果</param> 
        /// <param name="2">2质控结果</param> 
        /// </summary>
        public int Sendtype { get; set; }
        /// <summary>
        /// 消息开始标志
        /// </summary>
        //public string startMessage = "\u000b";
        public string startMessage = "\u000b";
        /// <summary>
        /// 消息结束标志
        /// </summary>
        public string endMessage = "\u001c\u000d";
        /// <summary>
        ///接收端应用程序
        /// </summary>
        public string ResiveApplication { get; set; }
        /// <summary>
        /// 接收端设备
        /// </summary>
        public string ResiveFacility { get; set; }
        /// <summary>
        /// 消息控制ID
        /// </summary>
        public static int ConstrolID { get; set; }

        /// <summary>
        /// 安全性
        /// </summary>
        public string Security { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public string Mtype { get; set; }
        /// <summary>
        /// 序列号
        /// </summary>
        public string Sequence { get; set; }
        /// <summary>
        /// 连续指针
        /// </summary>
        public string Continuation { get; set; }
        /// <summary>
        /// 接收应答类型
        /// </summary>
        public string AcceptType { get; set; }
        /// <summary>
        /// 消息主要语言
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// 不同的OBX字段
        /// </summary>
        public int SetID { get; set; }
        /// <summary>
        /// 消息实时时间
        /// </summary>
        public string GetTime { get { return DateTime.Now.ToString("yyyyMMddhhmmss"); } }
        private string _pLanguage;//主要语言
        /// <summary>
        /// 主要语言
        /// </summary>
        protected string PLanguage
        {
            get { return _pLanguage; }
            set { _pLanguage = value; }
        }
        /// <summary>
        /// 发送实验结果
        /// </summary>
        /// <param name="list"></param>
        //public void SendORU(List<TestResult> list)
        //{
        //    if (list.Count == 0) return;
        //    p = new Patient();
        //    rp = new ServeyReport();
        //    Mtype = "PR";
        //    //string _samplet = list[0].SampleType.Substring(0, 3);
        //    LrId = 1;
        //    DbHelperOleDb db = new DbHelperOleDb(0);
        //    DataTable _dtProject = DbHelperOleDb.Query(0,@"SELECT ProjectNumber, FullName, DiluteCount,ProjectType FROM tbProject").Tables[0];
        //    if (list[0].SampleType.Trim().Contains("质控品"))
        //    {
        //        Mtype = "QR";
        //        db = new DbHelperOleDb(3);
        //        DataTable dtQCInfo = DbHelperOleDb.Query(3, @"select QCID,Batch,SD,QCLevel from tbQC where status = '1' and ProjectName = '"
        //                                                    + list[0].ItemName + "' and Status = '1'").Tables[0];
        //        db = null;
        //        rp.PorderNum = list[0].SampleID.ToString();
        //        rp.ForderNum = list[0].SampleNo;
        //        rp.ODate = DateTime.Now.ToShortDateString();
        //        rp.OEDate = DateTime.Now.ToString();
        //        rp.CollectIdent = "^" + list[0].SamplePos;
        //        rp.SpcCode = "1";
        //        rp.RelevantInfo = list[0].ItemName;
        //        rp.ReseltDate = DateTime.Now.ToShortDateString();
        //        rp.Source = "0";
        //        rp.OProvider = dtQCInfo.Rows[0][1].ToString();
        //        rp.OCallbackNum = list[0].PMT.ToString();
        //        rp.SimpleState = dtQCInfo.Rows[0][2].ToString();
        //        rp.XdCode = list[0].concentration;
        //        rp.FilletF1 = "";
        //        //rp.FilletF2=;
        //        rp.ResuleState = list[0].Result;
        //        ReportType = "F";
        //        Priority = "R";

        //        string str = "";
        //        str = ENQ;
        //        LisConnection.Instance.write(str);
        //        bool delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
        //        if (!delay)
        //        {
        //            LisConnection.Instance.comWait.Set();
        //            MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            LisConnection.Instance.disconnection();
        //            return;
        //        }
        //        else
        //        {
        //            FN = 0;
        //            str = SendMessage(MHR(), LisConnection.Instance.EncodeType);
        //            LisConnection.Instance.write(str);
        //            delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
        //            if (!delay)
        //            {
        //                LisConnection.Instance.comWait.Set();
        //                MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                LisConnection.Instance.disconnection();
        //                return;
        //            }
        //            //Thread.Sleep(1000);
        //            str = SendMessage(PIR(p, rp), LisConnection.Instance.EncodeType);
        //            LisConnection.Instance.write(str);
        //            delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
        //            if (!delay)
        //            {
        //                LisConnection.Instance.comWait.Set();
        //                MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                LisConnection.Instance.disconnection();
        //                return;
        //            }
        //            //Thread.Sleep(1000);

        //            str = SendMessage(TOR(rp), LisConnection.Instance.EncodeType);
        //            LisConnection.Instance.write(str);
        //            delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
        //            if (!delay)
        //            {
        //                LisConnection.Instance.comWait.Set();
        //                MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                LisConnection.Instance.disconnection();
        //                return;
        //            }
        //            //Thread.Sleep(1000);
        //            ResultId = (Convert.ToInt32(ResultId) + 1).ToString();
        //            str = SendMessage(QCRR(rp), LisConnection.Instance.EncodeType);
        //            LisConnection.Instance.write(str);
        //            delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
        //            if (!delay)
        //            {
        //                LisConnection.Instance.comWait.Set();
        //                MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                LisConnection.Instance.disconnection();
        //                return;
        //            }
        //            //Thread.Sleep(1000);
        //            str = SendMessage(LR(), LisConnection.Instance.EncodeType);
        //            LisConnection.Instance.write(str);
        //            delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
        //            if (!delay)
        //            {
        //                LisConnection.Instance.comWait.Set();
        //                MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                LisConnection.Instance.disconnection();
        //                return;
        //            }
        //            Thread.Sleep(1000);
        //            LrId = LrId + 1;
        //            LisConnection.Instance.write(EOT);
        //        }

        //    }
        //    else
        //    {
        //        rp.PorderNum = list[0].SampleNo;
        //        rp.ForderNum = list[0].SampleNo;
        //         db = new DbHelperOleDb(1);
        //        OleDbDataReader read = DbHelperOleDb.ExecuteReader(1,@"SELECT Emergency,SendDateTime,SendDoctor,Department,InspectDoctor,Diagnosis,Source,PatientName,Sex,Age,
        //                ClinicNo,InpatientArea,Ward,BedNo,MedicaRecordNo  FROM tbSampleInfo WHERE SampleID=" + list[0].SampleID + " AND SampleNo='" + list[0].SampleNo + "'");
        //        if (read.Read())
        //        {
        //            if (read.GetInt32(0) == 3)
        //                rp.Priority = true;
        //            else
        //                rp.Priority = false;
        //            rp.ReceiveTime = read.GetDateTime(1).ToString();
        //            //rp.SpcCode;
        //            rp.Source = read.GetString(6);
        //            try
        //            {
        //                rp.OProvider = read.GetString(2);
        //            }
        //            catch
        //            {
        //                rp.OProvider = "";
        //            }
        //            try
        //            {
        //                rp.OCallbackNum = read.GetString(3);
        //            }
        //            catch
        //            {
        //                rp.OCallbackNum = "";
        //            }
        //            //rp.SimpleState;
        //            //rp.XdCode;
        //            try
        //            {
        //                rp.FilletF1 = read.GetString(4);
        //            }
        //            catch
        //            {
        //                rp.FilletF1 = "";
        //            }
        //            //rp.FilletF2;
        //            try
        //            {
        //                rp.RelevantInfo = read.GetString(5);
        //            }
        //            catch
        //            {
        //                rp.RelevantInfo = "";
        //            }
        //            try
        //            {
        //                p.Pname = read.GetString(7);
        //            }
        //            catch
        //            {
        //                p.Pname = "";
        //            }
        //            try
        //            {
        //                if (read.GetString(8) == "")
        //                    p.Sex = 'M';
        //                else if (read.GetString(8) == "女")
        //                    p.Sex = 'F';
        //                else
        //                    p.Sex = 'O';
        //            }
        //            catch
        //            {
        //                p.Sex = 'O';
        //            }
        //            if (read.GetDouble(9).ToString() != null)
        //            {
        //                p.Age = read.GetDouble(9).ToString();
        //            }
        //            //double  d = read.GetDouble(9);
        //            try
        //            {
        //                p.Patientid = read.GetString(10);
        //            }
        //            catch
        //            {
        //                p.Patientid = "";
        //            }
        //            try
        //            {
        //                p.SickArea = read.GetString(11);
        //            }
        //            catch
        //            {
        //                p.SickArea = "";
        //            }
        //            try
        //            {
        //                p.SickRoom = read.GetString(12);
        //            }
        //            catch
        //            {
        //                p.SickRoom = "";
        //            }
        //            try
        //            {
        //                p.Bedid = Convert.ToInt32(read.GetString(13));
        //            }
        //            catch
        //            {
        //                p.Bedid = 0;
        //            }
        //            try
        //            {
        //                p.PIdent = read.GetString(14);
        //            }
        //            catch
        //            {
        //                p.PIdent = "";
        //            }
        //        }
        //        db = null;
        //        read = null;

        //        rp.ODate = DateTime.Now.ToShortDateString();
        //        rp.OEDate = DateTime.Now.ToString();
        //        rp.CollectV = "1";
        //        rp.CollectIdent = "^" + list[0].SamplePos;
        //        rp.ReseltDate = DateTime.Now.ToShortDateString();
        //        rp.ResuleState = list[0].Result;
        //        List<string> ResultList = new List<string>();
        //        DataRow[] _drRow;
        //        foreach (TestResult li in list)
        //        {
        //            LabResult rs = new LabResult();
        //            _drRow = _dtProject.Select("FullName='" + li.ItemName + "'");

        //            if (_drRow.Length  > 0)
        //            {
        //                //int i = Convert.ToInt32(_drRow[0][2]);
        //                if ((Convert.ToInt32(_drRow[0][2])) > 1)
        //                    rp.Bdilute = true;
        //                else
        //                    rp.Bdilute = false;
        //                rs.ProjectId = _drRow[0][0].ToString();
        //                if (_drRow[0][3].ToString() == "0")
        //                    rs.Obxtype = "ST";
        //                else
        //                    rs.Obxtype = "NM";
        //            }
        //            rs.ProjectName = li.ItemName;
        //            rs.Resulttype = li.concentration;
        //            //rs.Unit=Result.
        //            rs.Rrs = li.Range1;
        //            rs.Abnormalflag = li.Result;
        //            rs.ResultFlag = "F";
        //            rs.Resulttype = li.concentration;
        //            rs.Observetime = DateTime.Now;
        //            rs.Method = "";
        //            rs.Doctor = rp.FilletF1;
        //            rs.ResultStatus = "F";
        //            ResultList.Add(SRR(rs));
        //            db = null;
        //            read = null;
        //        }
        //        ReportType = "F";
        //        Priority = "R";
        //        string str = "";
        //        str = ENQ;
        //        LisConnection.Instance.write(str);
        //        bool delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
        //        if (!delay)
        //        {
        //            LisConnection.Instance.comWait.Set();
        //            MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            LisConnection.Instance.disconnection();
        //            return;
        //        }
        //        else
        //        {
        //            FN = 0;
        //            str = SendMessage(MHR(), LisConnection.Instance.EncodeType);
        //            LisConnection.Instance.write(str);
        //            delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
        //            if (!delay)
        //            {
        //                LisConnection.Instance.comWait.Set();
        //                MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                LisConnection.Instance.disconnection();
        //                return;
        //            }
        //            str = SendMessage(PIR(p, rp), LisConnection.Instance.EncodeType);
        //            LisConnection.Instance.write(str);
        //            delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
        //            if (!delay)
        //            {
        //                LisConnection.Instance.comWait.Set();
        //                MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                LisConnection.Instance.disconnection();
        //                return;
        //            }
        //            //Thread.Sleep(1000);
        //            str = SendMessage(TOR(rp), LisConnection.Instance.EncodeType);
        //            LisConnection.Instance.write(str);
        //            delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
        //            if (!delay)
        //            {
        //                LisConnection.Instance.comWait.Set();
        //                MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                LisConnection.Instance.disconnection();
        //                return;
        //            }
        //            //Thread.Sleep(1000);
        //            for (int i = 0; i < ResultList.Count; i++)
        //            {
        //                ResultId = i.ToString();
        //                str = SendMessage(ResultList[i], LisConnection.Instance.EncodeType);
        //                LisConnection.Instance.write(str);
        //                delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
        //                if (!delay)
        //                {
        //                    LisConnection.Instance.comWait.Set();
        //                    MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                    LisConnection.Instance.disconnection();
        //                    return;
        //                }
        //                //Thread.Sleep(1000);
        //            }

        //            str = SendMessage(LR(), LisConnection.Instance.EncodeType);
        //            LisConnection.Instance.write(str);
        //            delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
        //            if (!delay)
        //            {
        //                LisConnection.Instance.comWait.Set();
        //                MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                LisConnection.Instance.disconnection();
        //                return;
        //            }
        //            //Thread.Sleep(1000);
        //            LrId = LrId + 1;
        //            LisConnection.Instance.write(EOT);
        //        }
        //        read = null;
        //    }
        //}
        /// <summary>
        /// 不同的obr信息
        /// </summary>
        public int obrID { get; set; }
        public void SendORU(List<TestResult> list)
        {
            //TestResult Result,
            if (list.Count == 0) return;
            p = new Patient();
            rp = new ServeyReport();
            Mtype = "ORU^R01";

            string Message = "";
            string _samplet = list[0].SampleType.Trim();
            if (list[0].SampleType.Contains("质控品"))
            {
                ConstrolID = ConstrolID + 1;
                DbHelperOleDb db = new DbHelperOleDb(3);
                DataTable dtQCInfo = DbHelperOleDb.Query(3, @"select QCID,Batch,SD,QCLevel from tbQC where status = '1' and ProjectName = '"
                                                            + list[0].ItemName + "' and Status = '1'").Tables[0];
                Sendtype = 2;
                obrID = ConstrolID;
                rp.PorderNum = list[0].SampleID.ToString();
                rp.ForderNum = list[0].SampleNo;
                rp.ODate = DateTime.Now.ToShortDateString();
                rp.OEDate = DateTime.Now.ToString();
                rp.CollectIdent = "^" + list[0].SamplePos;
                rp.SpcCode = "1";
                rp.RelevantInfo = list[0].ItemName;
                rp.ReseltDate = DateTime.Now.ToShortDateString();
                rp.Source = "0";
                rp.OProvider = dtQCInfo.Rows[0][1].ToString();
                rp.OCallbackNum = list[0].PMT.ToString();
                rp.SimpleState = dtQCInfo.Rows[0][2].ToString();
                rp.XdCode = list[0].concentration;
                rp.FilletF1 = "";
                //rp.FilletF2=;
                rp.ResuleState = list[0].Result;
                Message = startMessage + MSH() + OBR(rp) + endMessage;
            }
            else
            {
                ConstrolID = ConstrolID + 1;
                Sendtype = 0;
                obrID = ConstrolID;
                rp.PorderNum = list[0].SampleNo;
                rp.ForderNum = list[0].SampleNo;
                DbHelperOleDb db = new DbHelperOleDb(1);
                OleDbDataReader read = DbHelperOleDb.ExecuteReader(1, @"SELECT Emergency,SendDateTime,SendDoctor,Department,InspectDoctor,Diagnosis,Source,PatientName,Sex,Age,
                        ClinicNo,InpatientArea,Ward,BedNo,MedicaRecordNo  FROM tbSampleInfo WHERE SampleID=" + list[0].SampleID + " AND SampleNo='" + list[0].SampleNo + "'");

                if (read.Read())
                {
                    if (read.GetInt32(0) == 3)
                        rp.Priority = true;
                    else
                        rp.Priority = false;
                    rp.ReceiveTime = read.GetDateTime(1).ToString();
                    //rp.SpcCode;
                    rp.Source = read.GetString(6);
                    try
                    {
                        rp.OProvider = read.GetString(2);
                    }
                    catch
                    {
                        rp.OProvider = "";
                    }
                    try
                    {
                        rp.OCallbackNum = read.GetString(3);
                    }
                    catch
                    {
                        rp.OCallbackNum = "";
                    }
                    //rp.SimpleState;
                    //rp.XdCode;
                    try
                    {
                        rp.FilletF1 = read.GetString(4);
                    }
                    catch
                    {
                        rp.FilletF1 = "";
                    }
                    //rp.FilletF2;
                    try
                    {
                        rp.RelevantInfo = read.GetString(5);
                    }
                    catch
                    {
                        rp.RelevantInfo = "";
                    }
                    try
                    {
                        p.Pname = read.GetString(7);
                    }
                    catch
                    {
                        p.Pname = "";
                    }
                    try
                    {
                        if (read.GetString(8) == "男" || read.GetString(8) == "M" || read.GetString(8) == "m")
                            p.Sex = 'M';
                        else if (read.GetString(8) == "女" || read.GetString(8) == "F" || read.GetString(8) == "F")
                            p.Sex = 'F';
                        else
                            p.Sex = 'O';
                    }
                    catch
                    {
                        p.Sex = 'O';
                    }
                    if (read.GetDouble(9).ToString() != null)
                    {
                        p.Age = read.GetDouble(9).ToString();
                    }
                    //double  d = read.GetDouble(9);
                    try
                    {
                        p.Patientid = read.GetString(10);
                    }
                    catch
                    {
                        p.Patientid = "";
                    }
                    try
                    {
                        p.SickArea = read.GetString(11);
                    }
                    catch
                    {
                        p.SickArea = "";
                    }
                    try
                    {
                        p.SickRoom = read.GetString(12);
                    }
                    catch
                    {
                        p.SickRoom = "";
                    }
                    try
                    {
                        p.Bedid = Convert.ToInt32(read.GetString(13));
                    }
                    catch
                    {
                        p.Bedid = 0;
                    }
                    try
                    {
                        p.PIdent = read.GetString(14);
                    }
                    catch
                    {
                        p.PIdent = "";
                    }
                }
                read = null;

                rp.ODate = DateTime.Now.ToString("yyyyMMdd");
                rp.OEDate = DateTime.Now.ToString("yyyyMMddhhmmss");
                rp.CollectV = "1";
                rp.CollectIdent = "^" + list[0].SamplePos;
                rp.ReseltDate = DateTime.Now.ToString("yyyyMMdd");
                rp.ResuleState = list[0].Result;
                Message = startMessage + MSH() + PID(p) + OBR(rp);
                SetID = 1;
                foreach (TestResult li in list)
                {
                    LabResult rs = new LabResult();
                    db = new DbHelperOleDb(0);
                    read = DbHelperOleDb.ExecuteReader(0, @"SELECT ProjectNumber, FullName, DiluteCount,ProjectType FROM tbProject WHERE FullName='" + li.ItemName + "'");
                    if (read.Read())
                    {
                        if (read.GetInt32(2) > 1)
                            rp.Bdilute = true;
                        else
                            rp.Bdilute = false;
                        rs.ProjectId = read.GetString(0);
                        if (read.GetString(3) == "0")
                            rs.Obxtype = "ST";
                        else
                            rs.Obxtype = "NM";
                    }
                    rs.ProjectName = li.ItemName;
                    rs.Resulttype = li.concentration;
                    //rs.Unit=Result.
                    rs.Rrs = li.Range1;
                    rs.Abnormalflag = li.Result;
                    rs.ResultFlag = "F";
                    rs.Resulttype = li.concentration;
                    rs.Observetime = DateTime.Now;
                    rs.Method = "";
                    rs.Doctor = rp.FilletF1;
                    Message = Message + OBX(rs);
                    SetID = SetID + 1;
                }
                Message = Message + endMessage;
                read = null;
            }
            if (Message != "")
                LisConnection.Instance.write(Message);
        }
        /// <summary>
        /// 病人信息
        /// </summary>
        /// <param name="p"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public string PID(Patient p)
        {
            string pid = @"PID|" + ConstrolID + "|" + p.Patientid + "|" + p.PIdent + "|" + p.Bedid + "|" + p.Pname + "|" +
                (string.IsNullOrEmpty(p.SickArea) ? "" : (p.SickArea + "^")) + p.SickRoom + "|" + p.Age + "|" + p.Sex + "|" + p.Blood + "|"
                + p.Race + "|" + p.Address + "|" + p.Post + "|" + p.PhoneNum + "|" + p.Workphone + "|" + PLanguage + "|" + p.Marriage + "|" + p.Region + "|" + p.PatientType + "|" + p.Ybnum + "|" + p.FeeType + "||" + p.National + "|"
                + p.Origo + "|" + p.BirthIndic + "|" + p.BirthOrder + "|" + p.Demo + "|" + p.BmilitaryStatus + "|" + p.Country + "|" + p.Deathtime + "|"
                + p.Bdeath + "\u000d";
            //string pid = @"PID|" + Pid + "|" + p.Patientid + "|" + p.PIdent + "|" + p.Bedid + "|" + p.Pname + "|" + p.SickArea + "^" + p.SickRoom + "|" + p.Birth + "|" + p.Sex + "|" + p.Blood + "|"
            //   + p.Race + "|" + p.Address + "|" + p.Post + "|" + p.PhoneNum + "|" + p.Workphone + "|" + PLanguage + "|" + p.Marriage + "||" + p.PatientType + "|" + p.Ybnum + "|" + p.FeeType + "||" + p.National + "|"
            //   + p.Origo + "|" + p.BirthIndic + "|" + p.BirthOrder + "|" + p.Demo + "|" + p.BmilitaryStatus + "|" + p.Country + "|" + p.Deathtime + "|"
            //   + p.Bdeath + "|" + p.Age + "\u000d";
            return pid;
        }
        /// <summary>
        /// 样本架
        /// </summary>
        public string Collect { get; set; }
        /// <summary>
        /// 样本位置
        /// </summary>
        public string Ident { get; set; }
        /// <summary>
        /// 观察报告
        /// </summary>
        /// <returns></returns>
        public string OBR(ServeyReport r)
        {
            r.SpcCode = SampleNo;
            r.CollectIdent = Collect + "^" + Ident;
            string obr = "OBR|" + obrID + "|" + r.PorderNum + "|" + r.ForderNum + "|" + r.UnserviceId + "|" + (r.Priority == true ? "Y" : "N") + "|" + r.RDate + "|" + r.ODate + "|" + r.OEDate + "|"
                + r.CollectV + "|" + r.CollectIdent + "|" + r.SpcCode + "|" + r.Bdilute + "|" + r.RelevantInfo + "|" + r.ReseltDate + "|" + r.Source + "|" + r.OProvider + "|" + r.OCallbackNum + "|" +
                r.SimpleState + "|" + r.XdCode + "|" + r.FilletF1 + "|" + r.FilletF2 + "|" + r.ReseltDate + "|" + r.Chargepractice + "||" + r.ResuleState + "||||||||||||||||||||||" + "\u000d";
            //string obr = "OBR|" + obrID.ToString() + "|" + SamplebarCode + "|" + SampleNo + "||||||||||||"+SampleSource+"|"+doctor+"||||||结果报告日期/时间||||||||||||||||||||||||\u000d";
            return obr;

        }
        /// <summary>
        /// 检测结果
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public string OBX(LabResult r)
        {
            string obx = @"OBX|" + SetID + "|" + r.Obxtype + "|" + r.ProjectId + "|" + r.ProjectName + "|" + r.Resulttype + "|" + r.Unit + "|" + r.Rrs + "|" + r.Abnormalflag + "||" + r.Abnormaltest + "|" + r.ResultFlag + "|" + r.Resulttype + "|" + r.NormalLTime.ToString("yyyyMMddhhmmss") + "|" + r.Beginresult + "|" + r.Observetime.ToString("yyyyMMddhhmmss") + "||" + r.Doctor + "|" + r.Method + "\u000d";
            return obx;
        }
        /// <summary>
        /// 根据选择发送样本结果
        /// </summary>
        /// <param name="Result">实验结果</param>
        /// <param name="tranInfo">选择结果发送请求</param>
        public void SendSample(TestResult Result, string tranInfo)
        {
            LabResult rs = new LabResult();
            p = new Patient();
            rp = new ServeyReport();
            Mtype = "PR";
            string _samplet = Result.SampleType.Substring(0, 3);
            LrId = 1;
            DbHelperOleDb db = new DbHelperOleDb(3);
            OleDbDataReader read = DbHelperOleDb.ExecuteReader(3,@"SELECT Emergency,SendDateTime,SendDoctor,Department,InspectDoctor,Diagnosis,Source,PatientName,Sex,Age,
                        ClinicNo,InpatientArea,Ward,BedNo,MedicaRecordNo  FROM tbSampleInfo WHERE SampleID=" + Result.SampleID + " AND SampleNo=" + Result.SampleNo + "");
            if (read.Read())
            {
                string[] spstr = tranInfo.Split(',');
                for (int i = 0; i < spstr.Length; i++)
                {
                    switch (spstr[0])
                    {
                        case "01":
                            rp.ForderNum = Result.SampleNo;
                            break;
                        case "02":
                            p.Pname = read.GetString(7);
                            break;
                        case "03":
                            if (read.GetString(8) == "男")
                                p.Sex = 'M';
                            else if (read.GetString(8) == "女")
                                p.Sex = 'F';
                            else
                                p.Sex = 'O';
                            break;
                        case "04":
                            if (read.GetDouble(9).ToString() != null)
                            {
                                p.Age = read.GetDouble(9).ToString();
                            }
                            break;
                        case "05":
                            rp.OProvider = read.GetString(2);
                            break;
                        case "07":
                            p.Bedid = Convert.ToInt32(read.GetString(13));
                            break;
                        case "08":
                            rp.OCallbackNum = read.GetString(3);
                            break;
                        case "09":
                            rp.RelevantInfo = read.GetString(5);
                            break;
                        case "11":
                            rs.ProjectName = Result.ItemName;
                            break;
                        case "13":
                            rs.Resulttype = Result.concentration;
                            break;
                        case "14":
                            rs.Rrs = Result.Range1;
                            break;
                        case "15":
                            rp.ReceiveTime = read.GetString(1);
                            break;
                        case "16":
                            rp.FilletF1 = read.GetString(4);
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// 根据样品编号查询病人信息
        /// </summary>
        /// <param name="SampleNo"></param>
        public void SelectBySampleNo(string sampleNo)
        {
            SendApplication = OperateIniFile.ReadInIPara("LisSet", "SendingApplication");
            SendFacility = OperateIniFile.ReadInIPara("LisSet", "SendingFacility");
            Mtype = "QRY^Q02";
            Sendtype = 0;
            SampleNo = sampleNo;
            SendQry();

            //SendApplication = OperateIniFile.ReadInIPara("LisSet", "SendingApplication");
            //SendFacility = OperateIniFile.ReadInIPara("LisSet", "SendingFacility");
            //ServeyReport rp = new ServeyReport();
            //Mtype = "RQ";
            //ResiveFacility = "HOST";
            //string str = "";
            //str = ENQ;
            //LisConnection.Instance.write(str);
            //LisConnection.Instance.BWork = true;
            //bool delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
            //if (!delay)
            //{
            //    LisConnection.Instance.comWait.Set();
            //    MessageBox.Show("获取信息超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    LisConnection.Instance.disconnection();
            //    return;
            //}
            //else
            //{
            //    Mtype = "RQ";
            //    str = MHR();
            //    str = SendMessage(str, LisConnection.Instance.EncodeType);
            //    LisConnection.Instance.write(str);
            //    delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
            //    if (!delay)
            //    {
            //        LisConnection.Instance.comWait.Set();
            //        MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        LisConnection.Instance.disconnection();
            //        return;
            //    }
            //    //Thread.Sleep(1000);
            //    RequestCode = "O";
            //    str = QR(rp);
            //    str = SendMessage(str, LisConnection.Instance.EncodeType);
            //    LisConnection.Instance.write(str);
            //    delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
            //    if (!delay)
            //    {
            //        LisConnection.Instance.comWait.Set();
            //        MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        LisConnection.Instance.disconnection();
            //        return;
            //    }
            //    //Thread.Sleep(1000);
            //    LrId = LrId + 1;
            //    str = LR();
            //    str = SendMessage(str, LisConnection.Instance.EncodeType);
            //    LisConnection.Instance.write(str.ToString());
            //    delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
            //    if (!delay)
            //    {
            //        LisConnection.Instance.comWait.Set();
            //        MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        LisConnection.Instance.disconnection();
            //        return;
            //    }
            //    //Thread.Sleep(1000);
            //    str = EOT;
            //    LisConnection.Instance.write(str);
            //    delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
            //    if (!delay)
            //    {
            //        LisConnection.Instance.comWait.Set();
            //        MessageBox.Show("获取信息超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        LisConnection.Instance.disconnection();
            //        return;
            //    }
            //    LisConnection.Instance.BWork = false;
            //    //Thread.Sleep(1000);
            //}
        }
        public void SendQry()
        {
            string Message = startMessage + MSH() + QRD() + QRF() + endMessage;

            LogFile.Instance.Write("发送查询：\n" + Message + "\n");
            p = new Patient();
            rp = new ServeyReport();

            LisConnection.Instance.write(Message);
        }
        /// <summary>
        /// 查询方式，SN(样本编号方式)BC(样本条码方式)
        /// </summary>
        //public string QueryCode { get; set; }
        /// <summary>
        /// 查询ID，随查询数目递增
        /// </summary>
        public static int QueryId { get; set; }
        /// <summary>
        /// 查询内容
        /// </summary>
        public string FindCintext { get; set; }
        /// <summary>
        /// 部分数据代码
        /// </summary>
        public string DataCode { get; set; }
        /// <summary>
        /// 数据代码值限定
        /// </summary>
        public string DataCodeValue { get; set; }
        /// <summary>
        /// 延迟相应类型
        /// </summary>
        public string DalayType { get; set; }
        /// <summary>
        /// 延迟相应日期/时间
        /// </summary>
        public string DalayTime { get; set; }
        /// <summary>
        /// 消息头
        /// </summary>
        /// <returns></returns>
        public string MSH()
        {
            string msh = @"MSH|^~\&|" + SendApplication + "|" + SendFacility + "|" + ResiveApplication + "|" + ResiveFacility + "|"
                + GetTime + "|" + Security + "|" + Mtype + "|" + (ConstrolID++) + "|P|2.3.1|"
                + Sequence + "|" + Continuation + "|" + AcceptType + "|||" + /*"ASCII" + ""*/ /*+ Country*//*_character*/ "Unicode" + "|" + "|"  /*Language*/ + "|" + "\u000d";
            return msh;
        }
        public string QRD()
        {
            //string qrd = @"QRD|" + GetTime + "|R|D|" + QueryId + "|" + DalayType + "|"+DalayTime +"|RD|" + SampleNo + "|" + FindCintext + "|" + DataCode + "|" + DataCodeValue + "|T" + "\u000d";
            string qrd = @"QRD|" + GetTime + "|R|D|" + QueryId + "|" + DalayType + "|" + DalayTime + "|RD|" + SampleNo + "|" + "OTH" + "|" + DataCode + "|" + DataCodeValue + "|T|" + "\u000d";
            return qrd;
        }
        /// <summary>
        /// 记录开始日期/时间
        /// </summary>
        public string BeginTime { get; set; }
        /// <summary>
        /// 记录结束日期/时间
        /// </summary>
        public string EndTime { get; set; }
        public string RecordStartTime
        {
            get
            {
                return DateTime.Now.ToString("yyyyMMdd") + "000000";
            }
        }
        public string RecordEndTime
        {
            get
            {
                return DateTime.Now.ToString("yyyyMMddhhmmss");
            }
        }
        public string QRF()
        {
            //string qrf = @"QRF|" + SendFacility + "|" + BeginTime + "|" + EndTime + "|||RCT|COR|ALL||" + "\u000d";
            string qrf = @"QRF|" + SendFacility + "|" + RecordStartTime + "|" + RecordEndTime + "|||RCT|COR|ALL||" + "\u000d";
            return qrf;
        }
        private static tbSampleInfo _sampleInfo = new tbSampleInfo();
        /// <summary>
        /// 获取病人样本信息
        /// </summary>
        /// <returns></returns>
        public tbSampleInfo GetSampleInfo(tbSampleInfo modelSp)
        {
            if (p != null)
            {
                _sampleInfo = modelSp;
                try
                {
                    _sampleInfo.Age = Convert.ToDouble(p.Age);
                }
                catch
                {
                    _sampleInfo.Age = 0;
                }
                _sampleInfo.BedNo = p.Bedid.ToString();
                _sampleInfo.ClinicNo = p.Patientid;
                _sampleInfo.InpatientArea = p.SickArea;
                _sampleInfo.MedicaRecordNo = p.PIdent;
                _sampleInfo.PatientName = p.Pname;
                if (p.Sex == 'M')
                    _sampleInfo.Sex = "男";
                else if (p.Sex == 'F')
                    _sampleInfo.Sex = "女";
                else
                    _sampleInfo.Sex = "其他";
                _sampleInfo.Ward = p.SickRoom;
            }
            if (rp != null)
            {
                _sampleInfo.InspectDoctor = rp.FilletF1;
                _sampleInfo.SendDoctor = rp.OProvider;
                _sampleInfo.Department = rp.OCallbackNum;
                /*暂时屏蔽了更新项目信息
                if (rp.ProjectInfo != null && rp.ProjectInfo.Count > 0)
                {
                    for (int i = 0; i < rp.ProjectInfo.Count; i++)
                    {
                        string[] info = rp.ProjectInfo[i].Split('^');
                        _sampleInfo.ProjectName = info[1] + " ";
                    }
                }
                */
            }

            return _sampleInfo;
        }

        /// <summary>
        /// 处理从LIs接收的数据
        /// </summary>
        /// <param name="list"></param>
        public void ReciveInfo(List<string> list)
        {
            p = new Patient();
            rp = new ServeyReport();
            rp.ProjectInfo = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                string[] split = SplitMessage(list[i]);
                if (split[0] == "P")
                {
                    rp.ForderNum = split[1];
                    p.Pname = split[5];
                    p.Sex = Convert.ToChar(split[8]);
                    string[] ageinfo = split[14].Split('^');
                    if (ageinfo[0]!=null)
                    p.Age = ageinfo[0];
                    if (ageinfo[1] != null)
                    p.Ageunit = ageinfo[1];
                    p.Pname = split[5];
                }
                if (split[0] == "O")
                {
                    string[]spinfo = split[2].Split('^');
                    if (spinfo[0]!=null)
                        rp.PorderNum = spinfo[0];
                    if (spinfo[1] != null)
                        rp.ForderNum = spinfo[1];
                    if (spinfo[2] != null)
                        rp.CollectIdent = spinfo[2];
                    if (spinfo[3] != null)
                        rp.CollectSite = spinfo[3];
                    rp.Bdilute =Convert.ToBoolean(spinfo[4]);
                    spinfo = null;
                    spinfo = split[3].Split('^');
                    rp.RelevantInfo = spinfo[0];
                    rp.OProvider = spinfo[1];
                    rp.ProjectInfo.Add(split[4]);
                    rp.ReseltDate = split[6];
                    rp.Source = split[15];
                }
            }
            LisConnection.Instance.comWait.Set();
        }
    }
}
