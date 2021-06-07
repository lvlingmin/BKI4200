using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Localization;
using Common;

namespace BioBaseCLIA.InfoSetting
{
    class HL7Analysis
    {
        #region 若有多个消息一起收到，会使用该方法
        /// <summary>
        /// 多个消息字段分割
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>

        //public string[] sliptMessage(string Message)
        //{
        //    string EndString = @"<EB>";
        //    string StartString = @"<SB>";
        //    Regex getStartAreg = new Regex(StartString);
        //    Regex getEndAreg = new Regex(EndString);
        //    MatchCollection mcStart = getStartAreg.Matches(Message);
        //    MatchCollection mcEnd = getEndAreg.Matches(Message);//获取匹配的字符串内容
        //    string[] sMessageLines = new string[mcEnd.Count];
        //    if (mcEnd.Count > 0 && mcStart.Count > 0)
        //    {

        //        #region 获取最后一个清单表 hl7为最后一个清单表内容
        //        string[] getEndStrings = new string[mcEnd.Count];//获取匹配的字符串内容以String格式显示，并把每一个内容单独存放
        //        string[] getStartStrings = new string[mcStart.Count];

        //        int getMSAStringNum = 0;//记录mcMAS循环次数
        //        foreach (Match m in mcEnd)
        //        {
        //            getEndStrings[getMSAStringNum] = m.Value.ToString();
        //            getMSAStringNum = getMSAStringNum + 1;
        //        }
        //        getMSAStringNum = 0;
        //        foreach (Match m1 in mcStart)
        //        {
        //            getStartStrings[getMSAStringNum] = m1.Value.ToString();
        //            getMSAStringNum = getMSAStringNum + 1;
        //        }
        //        int thegetStartindex = 0;
        //        for (int i = 0; i < getEndStrings.Length; i++)
        //        {
        //            int thegetEndindex = Message.IndexOf(getEndStrings[i], thegetStartindex);
        //            sMessageLines[i] = Message.Substring(thegetStartindex, thegetEndindex - thegetStartindex);
        //            thegetStartindex = Message.IndexOf(getStartStrings[i], thegetEndindex);

        //        }
        //        #endregion

        //    }

        //    return sMessageLines;
        //}
        public string[] sliptMessage(string Message)
        {
            return Message.Split(new string[] { "\v" }, StringSplitOptions.RemoveEmptyEntries);
            //return System.Text.RegularExpressions.Regex.Split(Message,"MSH");
        }
        #endregion

        /// <summary>
        /// 单个不同类型消息字段分割
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="MessageType"></param>
        /// <returns></returns>
        //public List<string[]> SplitSegment(string Message, string MessageType)
        //{
        //    string EndString = @"<CR>";
        //    string StartString = @"<SB>";
        //    Regex getStartAreg = new Regex(StartString);
        //    Regex getEndAreg = new Regex(EndString);
        //    int num = 0;
        //    MatchCollection mcStart = getStartAreg.Matches(Message);
        //    MatchCollection mcEnd = getEndAreg.Matches(Message);//获取匹配的字符串内容
        //    string[] sMessageLines = new string[mcEnd.Count];

        //    if (mcEnd.Count > 0 && mcStart.Count > 0)
        //    {
        //        string[] getEndStrings = new string[mcEnd.Count];//获取匹配的字符串内容以String格式显示，并把每一个内容单独存放
        //        string[] getStartStrings = new string[mcStart.Count];

        //        int getMSAStringNum = 0;//记录mcMAS循环次数
        //        foreach (Match m in mcEnd)
        //        {
        //            getEndStrings[getMSAStringNum] = m.Value.ToString();
        //            getMSAStringNum = getMSAStringNum + 1;
        //        }
        //        getMSAStringNum = 0;
        //        foreach (Match m1 in mcStart)
        //        {
        //            getStartStrings[getMSAStringNum] = m1.Value.ToString();
        //            getMSAStringNum = getMSAStringNum + 1;
        //        }
        //        int thegetStartindex = 0;
        //        for (int i = 0; i < getEndStrings.Length; i++)
        //        {

        //            int thegetEndindex = Message.IndexOf(getEndStrings[i], thegetStartindex + 1);
        //            sMessageLines[i] = Message.Substring(thegetStartindex + 4, thegetEndindex - thegetStartindex);
        //            thegetStartindex = thegetEndindex;
        //        }
        //        List<string[]> sFieldss = new List<string[]>();
        //        foreach (string sMessageLine1 in sMessageLines)
        //        {


        //            if (sMessageLine1 != string.Empty)
        //            {
        //                if (sMessageLine1.Contains(MessageType))
        //                {

        //                    string[] sFields = sMessageLine1.Split('|');
        //                    sFieldss.Add(sFields);

        //                    return sFieldss;
        //                }
        //            }
        //        }
        //    }
        //    return null;
        //}

        public List<string[]> SplitSegment(string Message, string MessageType)
        {
            List<string[]> sFieldss = new List<string[]>();
            string[] sMessageLines = Message.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string sMessageLine1 in sMessageLines)
            {
                if (sMessageLine1 != string.Empty)
                {
                    if (sMessageLine1.Contains(MessageType))
                    {

                        string[] sFields = sMessageLine1.Split('|');
                        sFieldss.Add(sFields);

                        return sFieldss;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 接收病人信息时多个DSP信息分割
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public string[] DSPSplit(string message)
        {
            int index = message.IndexOf("DSP");
            string message1 = message.Substring(index);

            return message1.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// MSA字段errocode报错
        /// </summary>
        /// <param name="message1"></param>
        /// <param name="message2"></param>
        /// <returns></returns>
        public string errorInfo(string message1, string message2)
        {
            if (message1 == "AE")
            {
                if (message2 == "100")
                {
                    return LanguageManager.Instance.getLocaltionStr("SegmentSequenceError");

                }
                else if (message2 == "101")
                {
                    return LanguageManager.Instance.getLocaltionStr("RequiredFieldMissing");
                }
                else if (message2 == "102")
                {
                    return LanguageManager.Instance.getLocaltionStr("DataTypeError");
                }
                else if (message2 == "103")
                {
                    return LanguageManager.Instance.getLocaltionStr("TableValueNotFound");
                }
            }
            else if (message1 == "AR")
            {
                if (message2 == "200")
                {
                    return LanguageManager.Instance.getLocaltionStr("UnsupportedMessageType");
                }
                else if (message2 == "201")
                {
                    return LanguageManager.Instance.getLocaltionStr("UnsupportedEventcode");
                }
                else if (message2 == "202")
                {
                    return LanguageManager.Instance.getLocaltionStr("UnsupportedProcessingId");
                }
                else if (message2 == "203")
                {
                    return LanguageManager.Instance.getLocaltionStr("UnsupportedVersionId");
                }
                else if (message2 == "204")
                {
                    return LanguageManager.Instance.getLocaltionStr("UnknownKeyIdentifier");
                }
                else if (message2 == "205")
                {
                    return LanguageManager.Instance.getLocaltionStr("DuplicateKeyIdentifier");
                }
                else if (message2 == "206")
                {
                    return LanguageManager.Instance.getLocaltionStr("ApplicationRecordLocked");
                }
                else if (message2 == "207")
                {
                    return LanguageManager.Instance.getLocaltionStr("ApplicationInternalError");
                }

            }
            return null;
        }

        string currentDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
        /// <summary>
        /// 发送端应用程序
        /// </summary>
        string sendApplication = OperateIniFile.ReadInIPara("LisSet", "SendingApplication");
        /// <summary>
        /// 发送端设备
        /// </summary>
        string sendFacility = OperateIniFile.ReadInIPara("LisSet", "SendingFacility");

        public int mshID { set; get; }
        /// <summary>
        /// 应用程序应答类型(发送实验结果时，一般为0)
        /// </summary>
        public string ApplicationAckType { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public string MessageType { get; set; }
        /// <summary>
        /// MSH字段
        /// </summary>
        /// <returns></returns>
        public string MSH()
        {
            string msh = @"MSH|^~\&|" + sendApplication + "|" + sendFacility + "|||" + currentDateTime + "||" + MessageType + "|"
                          + this.mshID.ToString() + "|P|2.3.1|" + ApplicationAckType + "||Unicode|||" + "\u000d";
            return msh;
        }

        public int pidID { get; set; }
        /// <summary>
        /// 病人住院号
        /// </summary>
        public string PatientID { get; set; }
        /// <summary>
        /// 病人床号
        /// </summary>
        public string BedID { get; set; }
        /// <summary>
        /// 病历号
        /// </summary>
        public string PatienIdentifierID { get; set; }
        /// <summary>
        /// 病人姓名
        /// </summary>
        public string PatientName { get; set; }
        /// <summary>
        /// 病人性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 病人信息字段
        /// </summary>
        /// <returns></returns>
        public string PID()
        {
            string pid = "PID|" + pidID.ToString() + "|" + PatientID + "|" + PatienIdentifierID + "|" + BedID + "|" + PatientName + "|||"
                          + Sex + "|||||||||||||||||||||||" + "\u000d";
            return pid;
        }

        public int obrID { get; set; }
        /// <summary>
        /// 样本条码
        /// </summary>
        public string SamplebarCode { get; set; }
        /// <summary>
        /// 样本编号
        /// </summary>
        public string SampleNo { get; set; }
        /// <summary>
        /// 样本类型，样本来源
        /// </summary>
        public string SampleSource { get; set; }
        /// <summary>
        /// 送检医生
        /// </summary>
        public string doctor { get; set; }
        /// <summary>
        /// 送检时间
        /// </summary>
        public string InspectionTime { get; set; }
        /// <summary>
        /// 送检科室
        /// </summary>
        public string InspectionRoom { get; set; }
        /// <summary>
        /// 检验医生
        /// </summary>
        public string Verifier { get; set; }
        /// <summary>
        /// 审核者
        /// </summary>
        public string Moderator { get; set; }
        /// <summary>
        /// 检验报告相关的医嘱信息
        /// </summary>
        /// <returns></returns>
        public string OBR()
        {
            string obr = "OBR|" + obrID.ToString() + "|" + SamplebarCode + "|" + SampleNo + "|||||||||||" + InspectionTime + "|"
                          + SampleSource + "|" + doctor + "|" + InspectionRoom + "|||" + Verifier + "||||||||||||" + Moderator + "||||||||||||||" + "\u000d";
            return obr;
        }

        public int obxID { get; set; }
        /// <summary>
        /// 项目编号
        /// </summary>
        public string ProNumber { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProName { get; set; }
        /// <summary>
        /// 检验结果值
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// 参考范围
        /// </summary>
        public string Range { get; set; }
        /// <summary>
        /// 检验结果是否正常（描述）
        /// </summary>
        public string resultJuge { get; set; }
        /// <summary>
        /// 定性检验结果值
        /// </summary>
        public string resultValue { get; set; }
        /// <summary>
        /// 项目信息
        /// </summary>
        /// <returns></returns>
        public string OBX()
        {
            string obx = "OBX|" + obxID.ToString() + "|ST|" + ProNumber + "|" + ProName + "|" + resultValue + "|单位|" + Range + "|" + resultJuge
                          + "|" + Result + "||F|||||" + doctor + "||" + "\u000d";
            return obx;
        }

        /// <summary>
        /// 
        /// </summary>
        public string textMessage { get; set; }
        public string errorCode { get; set; }
        public string MSA()
        {
            string msa = "MSA|AA|" + mshID.ToString() + "|" + textMessage + "|||" + errorCode + "|" + "\u000d";
            return msa;
        }
        public int qrdID { get; set; }
        public string QRD()
        {
            string qrd = "QRD|" + currentDateTime + "|R|D|" + qrdID.ToString() + "|||RD|" + SamplebarCode + "|OTH|||T|" + "\u000d";
            return qrd;
        }
        /// <summary>
        /// 设备型号
        /// </summary>
        public string EquipmentType { get; set; }

        /// <summary>
        /// 样本接收之始
        /// </summary>
        public string BegainSampleReceive { get; set; }

        /// <summary>
        /// 样本接收之末
        /// </summary>
        public string EndSampleReceive { get; set; }
        public string QRF()
        {
            string qrf = "QRF|" + EquipmentType + "|" + BegainSampleReceive + "|" + EndSampleReceive + "|||RCT|COR|ALL||" + "\u000d";
            return qrf;
        }
        public string errorPosition { get; set; }
        public string ERR()
        {
            string ord = "ERR|" + errorPosition + "|" + "\u000d";
            return ord;
        }
        public string QueryACKStatus { get; set; }
        public string QAK()
        {
            string qak = "QAK|SR|" + QueryACKStatus + "|" + "\u000d";
            return qak;
        }

        public int dspID { get; set; }
        public string dataLine { get; set; }
        public string DSP()
        {
            string dsp = "DSP|" + dspID.ToString() + "||" + dataLine + "|||" + "\u000d";
            return dsp;
        }

        public int sampleN { get; set; }
        public string DSC()
        {
            string dsc = "DSC|" + sampleN + "|" + "\u000d";
            return dsc;
        }
        public string startMessage = "\u000b";
        public string endMessage = "\u001c\u000d";
        public string sendRMessage()
        {

            return startMessage + MSH() + PID() + OBR() + OBX() + endMessage;
        }
        public string sendQRYMessage()
        {
            return startMessage + MSH() + QRD() + QRF() + endMessage;
        }

        /// <summary>
        /// 收到服务器发出的DSR^R03消息时的应答
        /// </summary>
        /// <returns></returns>
        public string sendDSRMessage()
        {
            return startMessage + MSH() + MSA() + ERR() + endMessage;
        }
        public string sendACKMessage()
        {
            return startMessage + MSH() + MSA() + endMessage;

        }
    }
}
