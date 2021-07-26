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
        /// 发送实验结果
        /// </summary>
        /// <param name="list"></param>
        public void SendORU(List<TestResult> list)
        {
            if (list.Count == 0) return;
            p = new Patient();
            rp = new ServeyReport();
            Mtype = "PR";
            //string _samplet = list[0].SampleType.Substring(0, 3);
            LrId = 1;
            DbHelperOleDb db = new DbHelperOleDb(0);
            DataTable _dtProject = DbHelperOleDb.Query(0,@"SELECT ProjectNumber, FullName, DiluteCount,ProjectType FROM tbProject").Tables[0];
            if (list[0].SampleType.Trim().Contains("质控品"))
            {
                Mtype = "QR";
                db = new DbHelperOleDb(3);
                DataTable dtQCInfo = DbHelperOleDb.Query(3, @"select QCID,Batch,SD,QCLevel from tbQC where status = '1' and ProjectName = '"
                                                            + list[0].ItemName + "' and Status = '1'").Tables[0];
                db = null;
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
                ReportType = "F";
                Priority = "R";

                string str = "";
                str = ENQ;
                LisConnection.Instance.write(str);
                bool delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
                if (!delay)
                {
                    LisConnection.Instance.comWait.Set();
                    MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LisConnection.Instance.disconnection();
                    return;
                }
                else
                {
                    FN = 0;
                    str = SendMessage(MHR(), LisConnection.Instance.EncodeType);
                    LisConnection.Instance.write(str);
                    delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
                    if (!delay)
                    {
                        LisConnection.Instance.comWait.Set();
                        MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LisConnection.Instance.disconnection();
                        return;
                    }
                    //Thread.Sleep(1000);
                    str = SendMessage(PIR(p, rp), LisConnection.Instance.EncodeType);
                    LisConnection.Instance.write(str);
                    delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
                    if (!delay)
                    {
                        LisConnection.Instance.comWait.Set();
                        MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LisConnection.Instance.disconnection();
                        return;
                    }
                    //Thread.Sleep(1000);

                    str = SendMessage(TOR(rp), LisConnection.Instance.EncodeType);
                    LisConnection.Instance.write(str);
                    delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
                    if (!delay)
                    {
                        LisConnection.Instance.comWait.Set();
                        MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LisConnection.Instance.disconnection();
                        return;
                    }
                    //Thread.Sleep(1000);
                    ResultId = (Convert.ToInt32(ResultId) + 1).ToString();
                    str = SendMessage(QCRR(rp), LisConnection.Instance.EncodeType);
                    LisConnection.Instance.write(str);
                    delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
                    if (!delay)
                    {
                        LisConnection.Instance.comWait.Set();
                        MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LisConnection.Instance.disconnection();
                        return;
                    }
                    //Thread.Sleep(1000);
                    str = SendMessage(LR(), LisConnection.Instance.EncodeType);
                    LisConnection.Instance.write(str);
                    delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
                    if (!delay)
                    {
                        LisConnection.Instance.comWait.Set();
                        MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LisConnection.Instance.disconnection();
                        return;
                    }
                    Thread.Sleep(1000);
                    LrId = LrId + 1;
                    LisConnection.Instance.write(EOT);
                }

            }
            else
            {
                rp.PorderNum = list[0].SampleNo;
                rp.ForderNum = list[0].SampleNo;
                 db = new DbHelperOleDb(1);
                OleDbDataReader read = DbHelperOleDb.ExecuteReader(1,@"SELECT Emergency,SendDateTime,SendDoctor,Department,InspectDoctor,Diagnosis,Source,PatientName,Sex,Age,
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
                        if (read.GetString(8) == "")
                            p.Sex = 'M';
                        else if (read.GetString(8) == "女")
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
                db = null;
                read = null;

                rp.ODate = DateTime.Now.ToShortDateString();
                rp.OEDate = DateTime.Now.ToString();
                rp.CollectV = "1";
                rp.CollectIdent = "^" + list[0].SamplePos;
                rp.ReseltDate = DateTime.Now.ToShortDateString();
                rp.ResuleState = list[0].Result;
                List<string> ResultList = new List<string>();
                DataRow[] _drRow;
                foreach (TestResult li in list)
                {
                    LabResult rs = new LabResult();
                    _drRow = _dtProject.Select("FullName='" + li.ItemName + "'");
                    
                    if (_drRow.Length  > 0)
                    {
                        //int i = Convert.ToInt32(_drRow[0][2]);
                        if ((Convert.ToInt32(_drRow[0][2])) > 1)
                            rp.Bdilute = true;
                        else
                            rp.Bdilute = false;
                        rs.ProjectId = _drRow[0][0].ToString();
                        if (_drRow[0][3].ToString() == "0")
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
                    rs.ResultStatus = "F";
                    ResultList.Add(SRR(rs));
                    db = null;
                    read = null;
                }
                ReportType = "F";
                Priority = "R";
                string str = "";
                str = ENQ;
                LisConnection.Instance.write(str);
                bool delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
                if (!delay)
                {
                    LisConnection.Instance.comWait.Set();
                    MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LisConnection.Instance.disconnection();
                    return;
                }
                else
                {
                    FN = 0;
                    str = SendMessage(MHR(), LisConnection.Instance.EncodeType);
                    LisConnection.Instance.write(str);
                    delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
                    if (!delay)
                    {
                        LisConnection.Instance.comWait.Set();
                        MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LisConnection.Instance.disconnection();
                        return;
                    }
                    str = SendMessage(PIR(p, rp), LisConnection.Instance.EncodeType);
                    LisConnection.Instance.write(str);
                    delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
                    if (!delay)
                    {
                        LisConnection.Instance.comWait.Set();
                        MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LisConnection.Instance.disconnection();
                        return;
                    }
                    //Thread.Sleep(1000);
                    str = SendMessage(TOR(rp), LisConnection.Instance.EncodeType);
                    LisConnection.Instance.write(str);
                    delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
                    if (!delay)
                    {
                        LisConnection.Instance.comWait.Set();
                        MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LisConnection.Instance.disconnection();
                        return;
                    }
                    //Thread.Sleep(1000);
                    for (int i = 0; i < ResultList.Count; i++)
                    {
                        ResultId = i.ToString();
                        str = SendMessage(ResultList[i], LisConnection.Instance.EncodeType);
                        LisConnection.Instance.write(str);
                        delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
                        if (!delay)
                        {
                            LisConnection.Instance.comWait.Set();
                            MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LisConnection.Instance.disconnection();
                            return;
                        }
                        //Thread.Sleep(1000);
                    }
                    
                    str = SendMessage(LR(), LisConnection.Instance.EncodeType);
                    LisConnection.Instance.write(str);
                    delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
                    if (!delay)
                    {
                        LisConnection.Instance.comWait.Set();
                        MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LisConnection.Instance.disconnection();
                        return;
                    }
                    //Thread.Sleep(1000);
                    LrId = LrId + 1;
                    LisConnection.Instance.write(EOT);
                }
                read = null;
            }
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
            ServeyReport rp = new ServeyReport();
            Mtype = "RQ";
            SendFacility = "BIOBASE-BK";
            ResiveFacility = "HOST";
            string str = "";
            str = ENQ;
            LisConnection.Instance.write(str);
            LisConnection.Instance.BWork = true;
            bool delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
            if (!delay)
            {
                LisConnection.Instance.comWait.Set();
                MessageBox.Show("获取信息超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LisConnection.Instance.disconnection();
                return;
            }
            else
            {
                Mtype = "RQ";
                str = MHR();
                str = SendMessage(str, LisConnection.Instance.EncodeType);
                LisConnection.Instance.write(str);
                delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
                if (!delay)
                {
                    LisConnection.Instance.comWait.Set();
                    MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LisConnection.Instance.disconnection();
                    return;
                }
                //Thread.Sleep(1000);
                RequestCode = "O";
                str = QR(rp);
                str = SendMessage(str, LisConnection.Instance.EncodeType);
                LisConnection.Instance.write(str);
                delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
                if (!delay)
                {
                    LisConnection.Instance.comWait.Set();
                    MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LisConnection.Instance.disconnection();
                    return;
                }
                //Thread.Sleep(1000);
                LrId = LrId + 1;
                str = LR();
                str = SendMessage(str, LisConnection.Instance.EncodeType);
                LisConnection.Instance.write(str.ToString());
                delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
                if (!delay)
                {
                    LisConnection.Instance.comWait.Set();
                    MessageBox.Show("消息发送超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LisConnection.Instance.disconnection();
                    return;
                }
                //Thread.Sleep(1000);
                str = EOT;
                LisConnection.Instance.write(str);
                delay = LisConnection.Instance.comWait.WaitOne(Dalaytime);
                if (!delay)
                {
                    LisConnection.Instance.comWait.Set();
                    MessageBox.Show("获取信息超时！通讯已经中断！", "消息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LisConnection.Instance.disconnection();
                    return;
                }
                LisConnection.Instance.BWork = false;
                //Thread.Sleep(1000);
            }
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
