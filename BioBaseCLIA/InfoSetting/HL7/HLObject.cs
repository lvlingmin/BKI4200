using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioBaseCLIA.InfoSetting
{
    public class HLObject
    {
        protected const string _versionid="2.3.1"; //协议版本
        private string _country;//国家代码
        protected  string _character=LisCommunication.Instance.EncodeType;//字符集
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
        /// 国家代码
        /// </summary>
        protected string Country
        {
            get { return _country; }
            set { _country = value; }
        }
        /// <summary>
        /// 消息实时时间
        /// </summary>
        public string GetTime { get { return DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"); } }
        /// <summary>
        /// 发送端应用程序
        /// </summary>
        public string SendApplication { get; set; }
        /// <summary>
        /// 发送端设备
        /// </summary>
        public string SendFacility { get; set; }
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
        /// 应用程序应答类型，发送结果
        /// <param name="0">0病人样本测试结果</param> 
        /// <param name="1">1校准结果</param> 
        /// <param name="2">2质控结果</param> 
        /// </summary>
        public int Sendtype { get; set; }
        /// <summary>
        /// 消息主要语言
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// 消息头
        /// </summary>
        /// <returns></returns>
        public string MSH()
        {
            string msh = @"MSH|^~\&|" + SendApplication + "|" + SendFacility + "|" + ResiveApplication + "|" + ResiveFacility + "|"
                + GetTime + "|" + Security + "|" + Mtype + "|" + ConstrolID + "|P|2.3.1|"
                + Sequence + "|" + Continuation + "|" + AcceptType + "|" + Sendtype + "|" + Country + "|" + _character + "|" + Language + "|" + "\u000d";
            return msh;
        }
        /// <summary>
        /// 接收确认编码
        /// <para name="AA">AA接收</para>
        /// <para name="AE">AE错误</para>
        /// <para name="AR">AR拒绝</para>
        /// <para name="OK">AR查询到数据</para>
        /// <para name="NF">NF没有找到数据</para>
        /// </summary>
        public string Ackcode { get; set; }
        /// <summary>
        /// 日志信息，对事件的文本描述，可写入错误的日志
        /// </summary>
        public string Tmessage { get; set; }
        /// <summary>
        /// 预期序列号
        /// </summary>
        public string Expnem { get; set; }
        /// <summary>
        /// 延迟确认类型
        /// </summary>
        public int Delaytype { get; set; }
        /// <summary>
        /// 错误条件
        /// </summary>
        public string ErrorCode { get; set; }
       /// <summary>
       /// 消息确认
       /// </summary>
       /// <returns></returns>
        public string MSA()
        {
            string msa = @"MSA|" + Ackcode + "|" + ConstrolID + "|" + Tmessage + "|" + Expnem + "|" + Delaytype + "|" + ErrorCode + "\u000d";
            return msa;
        }
        /// <summary>
        /// 不同病人消息
        /// </summary>
        public string Pid
        {
            get;set;
        }
        /// <summary>
        /// 病人信息
        /// </summary>
        /// <param name="p"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public string PID(Patient p)
        {
            string pid = @"PID|" + Pid + "|" + p.Patientid + "|" + p.PIdent + "|" + p.Bedid + "|" + p.Pname + "|" +p.SickArea +"^"+p.SickRoom + "|" + p.Birth + "|" + p.Sex + "|" + p.Blood + "|"
                + p.Race + "|" + p.Address + "|" + p.Post + "|" + p.PhoneNum + "|" + p.Workphone + "|" + PLanguage + "|" + p.Marriage + "|"+p.Region+"|" + p.PatientType + "|" + p.Ybnum + "|" + p.FeeType + "||" + p.National + "|"
                + p.Origo + "|" + p.BirthIndic + "|" + p.BirthOrder + "|" + p.Demo + "|" + p.BmilitaryStatus + "|" + p.Country + "|" + p.Deathtime + "|"
                + p.Bdeath + "\u000d";
            //string pid = @"PID|" + Pid + "|" + p.Patientid + "|" + p.PIdent + "|" + p.Bedid + "|" + p.Pname + "|" + p.SickArea + "^" + p.SickRoom + "|" + p.Birth + "|" + p.Sex + "|" + p.Blood + "|"
            //   + p.Race + "|" + p.Address + "|" + p.Post + "|" + p.PhoneNum + "|" + p.Workphone + "|" + PLanguage + "|" + p.Marriage + "||" + p.PatientType + "|" + p.Ybnum + "|" + p.FeeType + "||" + p.National + "|"
            //   + p.Origo + "|" + p.BirthIndic + "|" + p.BirthOrder + "|" + p.Demo + "|" + p.BmilitaryStatus + "|" + p.Country + "|" + p.Deathtime + "|"
            //   + p.Bdeath + "|" + p.Age + "\u000d";
            return pid;
        }
        /// <summary>
        /// 不同的obr信息
        /// </summary>
        public int obrID { get; set; }
        /// <summary>
        /// 样本编号
        /// </summary>
        public string SampleNo { get; set; }
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
            r.CollectIdent = Collect+"^"+Ident;
            string obr = "OBR|" + obrID + "|" + r.PorderNum + "|" + r.ForderNum + "|" + r.UnserviceId + "|" + r.Priority + "|" + r.RDate + "|" + r.ODate + "|" + r.OEDate + "|"
                + r.CollectV + "|" + r.CollectIdent + "|" + r.SpcCode + "|" + r.Bdilute + "|"+r.RelevantInfo +"|"+r.ReseltDate +"|" + r.Source  + "|" + r.OProvider  + "|"+r.OCallbackNum +"|"+
                r.SimpleState +"|"+r.XdCode+"|"+r.FilletF1 +"|"+r.FilletF2 +"|"+r.ReseltDate +"|"+r.Chargepractice+"||"+r.ResuleState+"||||||||||||||||||||||" + "\u000d";
            //string obr = "OBR|" + obrID.ToString() + "|" + SamplebarCode + "|" + SampleNo + "||||||||||||"+SampleSource+"|"+doctor+"||||||结果报告日期/时间||||||||||||||||||||||||\u000d";
            return obr;

        }
     /// <summary>
     /// 不同的OBX字段
     /// </summary>
        public int SetID { get; set; }
        /// <summary>
        /// 检测结果
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public string OBX(LabResult r) 
        {
            string obx = @"OBX|" + SetID + "|" + r.Obxtype + "|" + r.ProjectId + "|" + r.ProjectName + "^" + r.SubId + "|" + r.Resulttype + "|" + r.Unit + "|" + r.Rrs + "|" + r.Abnormalflag + "||" + r.Abnormaltest + "|" + r.ResultFlag + "|" + r.Resulttype + "|" + r.NormalLTime + "|" + r.Beginresult + "|" + r.Observetime + "||" + r.Doctor + "|" + r.Method + "\u000d";
            return obx;
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
        /// 查询定义
        /// </summary>
        /// <returns></returns>
        public string QRD()
        {
            string qrd = @"QRD|" + GetTime + "|R|D|" + QueryId + "|" + DalayType + "|"+DalayTime +"|RD|" + SampleNo + "|" + FindCintext + "|" + DataCode + "|" + DataCodeValue + "|T" + "\u000d";
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
        /// <summary>
        /// 查询筛选
        /// </summary>
        /// <returns></returns>
        public string QRF()
        {
            string qrf = @"QRF|" + SendFacility + "|" + BeginTime + "|" + EndTime + "|||RCT|COR|ALL|" + "\u000d";
            return qrf;
        }

        /// <summary>
        /// 确定不同的DSP片段
        /// </summary>
        public int DspId { get; set; }
        /// <summary>
        /// 显示级别
        /// </summary>
        public int DLevel{ get; set; }
        /// 显示数据信息
        ///// </summary>
        public string DataLine { get; set; }
        /// <summary>
        /// 结果ID
        /// </summary>
        public string ResultID { get; set; }
        /// <summary>
        /// 逻辑断点
        /// </summary>
        public string BreakPoint { get; set; }
        /// <summary>
        /// 显示查询得到的样本信息和检测项目
        /// </summary>
        /// <returns></returns>
        public string DSP()
        {
            string dsp = @"DSP|" + DspId + "|" + DLevel + "|" + DataLine + "||" + ResultID + "\u000d";
            return dsp;
        }

        public string DSC()
        {
            string dsc=@"DSC||\u000d";
            return dsc;
        }
        /// <summary>
        /// 消息开始标志
        /// </summary>
        //public string startMessage = "\u000b";
        public string startMessage ="\u000b";
        /// <summary>
        /// 消息结束标志
        /// </summary>
        public string endMessage = "\u001c\u000d";
        /// <summary>
        /// 将接受到的消息进行拆分归类
        /// </summary>
        /// <param name="Message">接收到的原始消息</param>
        /// <returns></returns>
        public string[] SplitMessage(string Message)
        {
            int IndexofA = Message.IndexOf("\u000b");
            int IndexofB = Message.IndexOf("\u001c\u000d");
            if (Message.Length > 0 && IndexofB >0)
            {
                Message = Message.Substring(IndexofA + "\u000b".Length, IndexofB - "\u001c\u000d".Length);
            }
            string[] NewMessage = Message.Split(new string[] { "\u000d" }, StringSplitOptions.RemoveEmptyEntries);
            return NewMessage;
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        /// <param name="ackcode">应答类型</param>
        /// <param name="ackcode">应答编码</param>
        /// <returns></returns>
        public string ErrorInfo(string ackType,int ackcode)
        {
            string _errorinfo="";
            if (ackType == "NF")
                _errorinfo = "查询没有找到数据";
            else if (ackType == "AE")
            {
                switch (ackcode)
                { 
                    case 100:
                        _errorinfo = "消息中段的顺序不正确";
                        break;
                    case 101:
                        _errorinfo = "一个段中丢失必须的字段";
                        break;
                    case 102:
                        _errorinfo = "字段的数据类型错误";
                        break;
                    case 103:
                        _errorinfo = "表值为找到";
                        break;
                    default:
                        break;

                }
            }
            else if (ackType == "AR")
            {
                switch (ackcode)
                {
                    case 200:
                        _errorinfo = "消息类型不正确";
                        break;
                    case 201:
                        _errorinfo = "事件代号不支持";
                        break;
                    case 202:
                        _errorinfo = "处理ID不支持";
                        break;
                    case 203:
                        _errorinfo = "版本ID不支持";
                        break;
                    case 204:
                        _errorinfo = "病人信息不存在";
                        break;
                    case 205:
                        _errorinfo = "已存在重复的关键字";
                        break;
                    case 206:
                        _errorinfo = "事务在应用程序存储级不能执行";
                        break;
                    case 207:
                        _errorinfo = "不明的应用程序内部其他错误";
                        break;
                    default:
                        break;

                }
            }
            else
            {
                _errorinfo = "成功";
            }
            return _errorinfo; 
        }
    }
}
