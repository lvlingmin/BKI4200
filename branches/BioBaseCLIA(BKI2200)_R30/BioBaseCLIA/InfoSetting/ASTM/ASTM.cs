using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioBaseCLIA.InfoSetting
{
    public class ASTM
    {
        protected const string _versionid = "1"; //协议版本
        /// <summary>
        /// 数据结束
        /// </summary>
        public string LF = "\u0000A";
        /// <summary>
        /// 回车
        /// </summary>
        public string CR = "\u000d";
        /// <summary>
        /// 数据开始
        /// </summary>
        public string STX = "\u0002";
        /// <summary>
        /// 数据结束
        /// </summary>
        public string ETX = "\u0003";
        /// <summary>
        /// 中间帧消息结束
        /// </summary>
        public string ETB = "\u0017";
        /// <summary>
        /// 传送结束
        /// </summary>
        public string EOT = "\u0004";
        /// <summary>
        /// 请求
        /// </summary>
        public string ENQ = "\u0005";
        /// <summary>
        /// 确认
        /// </summary>
        public string ACK = "\u0006";
        /// <summary>
        /// 否认
        /// </summary>
        public string NAK = "\u0015";
        /// <summary>
        /// 消息帧帧值
        /// </summary>
        public int FN { get; set; }
        /// <summary>
        /// 校验和，校验高位，低位计算
        /// </summary>
        public string CheckNum(string str,string ecode)
        {
            int count = 0;
            byte[] bytes ;
            switch (ecode)//2018-4-25 zlxadd
            {
                case "ASCII":
                    bytes = Encoding.ASCII.GetBytes(str);
                    break;
                case "UTF8":
                    bytes = Encoding.UTF8.GetBytes(str);
                    break;
                case "UTF32":
                    bytes = Encoding.UTF32.GetBytes(str);
                    break;
                case "UNICODE":
                    bytes = Encoding.Unicode.GetBytes(str);
                    break;
                default:
                    bytes = Encoding.Unicode.GetBytes(str);
                    break;
            }
            //byte[] bytes = ecode.GetBytes(str);
            for (int i = 0; i < bytes.Length; i++)
            {
                count = count + bytes[i];
            }
            return (count % 256).ToString("X2");//校验和
        }
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
        /// 消息类型
        /// PR-病人测试结果
        /// QR-质控测试结果
        /// CR-定标结果
        /// RQ-样本请求查询
        /// QA-样本请求回应
        /// SA-样本申请信息
        /// </summary>
        public string Mtype { get; set; }
        /// <summary>
        /// <summary>
        /// 消息头记录
        /// </summary>
        /// <returns></returns>
        public string MHR()
        {
            string mhr = @"H|\^&|||" + SendFacility + "|||||" + ResiveFacility + "||"+Mtype+"|" + _versionid + "|"+DateTime.Now.ToString("yyyyMMddhhmmss")+"";
            return mhr;
        }
        /// <summary>
        /// 患者信息记录
        /// </summary>
        /// <returns></returns>
        public string PIR(Patient p,ServeyReport rp)
        {
            string pir = @"P|"+rp.ForderNum+"||||"+p.Pname+"|||"+p.Sex+"||||||"+p.Age+"^"+p.Ageunit+"";
            return pir;
        }
        /// <summary>
        /// 测试序列序号
        /// </summary>
        public int TorId { get; set; }
        /// <summary>
        /// 检查项目信息
        /// 项目编号^项目名称^稀释倍数^重复测试次数
        /// </summary>
        public string ProjectInfo {get;set; }
        /// <summary>
        /// 测试顺序
        /// S-打乱顺序
        /// R-正常顺序
        /// </summary>
        public string Priority { get; set; }
        /// <summary>
        /// 报告类型
        /// O-来自Lis请求
        /// Q-查询响应
        /// F-最终结果
        /// </summary>
        public string ReportType { get; set; }
        /// <summary>
        /// 测试序列记录
        /// </summary>
        /// <returns></returns>
        public string TOR(ServeyReport rp)
        {
            string tor = @"O|" + TorId + "|" + rp.PorderNum + "^" + rp.ForderNum + "^" + rp.CollectIdent + "^" + rp.CollectSite + "^" + rp.Bdilute + "|" + rp.RelevantInfo + "^" + rp.OProvider + "|" + ProjectInfo + "|" 
                + Priority + "|"+rp.ReseltDate+"|||||||||"+rp.Source+"||||||||||"+ReportType+"";
            return tor;
        }
        public string ResultId { get; set; }
        /// <summary>
        /// 样本结果记录
        /// </summary>
        /// <returns></returns>
        public string SRR(LabResult r)
        {
            string rr = @"R|" + ResultId + "|" + r.ProjectName + "|" + r.Resultvalue + "|" + r.Unit + "|" + r.Rrs + "|" + r.Abnormalflag + "||" + r.ResultStatus + "||||"+r.Observetime.ToString("yyyyMMddhhmmss") +"";
            return rr;
        }
        /// <summary>
        /// 质控结果记录
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public string QCRR(ServeyReport r)
        {
            string rr = @"R|" + ResultId + "|" + r.RelevantInfo + "|" + r.XdCode + "|"+r.FilletF1+"|" + r.OCallbackNum + "^" + r.SimpleState + "|" + r.FilletF2 + "|" + r.XdCode + "|" + r.ResuleState + "||||" + r.ReseltDate + "";
            return rr;
        }
        public int LrId { get; set; }
        /// <summary>
        /// 消息终止记录
        /// </summary>
        /// <returns></returns>
        public string LR()
        {
            string lr = @"L|" + LrId + "|N|";
            return lr;
        }
        /// <summary>
        /// 请求序列号
        /// </summary>
        public int QrId { get; set; }
        /// <summary>
        /// 查询命令码
        /// O-请求样本查询
        /// A-取消当前的查询请求
        /// </summary>
        public string RequestCode { get; set; }
        /// <summary>
        /// 请求病人信息
        /// 4-样本起始编号 5-样本终止编号
        /// </summary>
        /// <returns></returns>
        public string QR(ServeyReport sp)
        {
            QrId = QrId + 1;
            string qr = @"Q|" + QrId + "|^" + sp.PorderNum + "|" + sp.ForderNum + "|" + sp.ForderNum + "||" + sp.ODate + "|" + sp.OEDate + "|||||" + RequestCode + "|";
            return qr;
        }
        /// <summary>
        /// 发送消息帧
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public string  SendMessage(string message,string encod)
        {
            FN++;
            string sm = STX + FN +  message + ETX + CheckNum(message, encod) +CR+LF;
            return sm;
        }
        /// <summary>
        /// 拆分数据
        /// </summary>
        /// <param name="Message">接收道德信息</param>
        /// <returns></returns>
        public string[] SplitMessage(string Message)
        {
            int IndexofA = Message.IndexOf(STX);
            int IndexofB = Message.IndexOf(ETX);
            if (Message.Length > 0 && IndexofB > 0)
            {
                Message = Message.Substring(IndexofA + STX.Length + 1, IndexofB - ETX.Length-1);
            }
            string[] NewMessage = Message.Split('|');
            return NewMessage;
        }


    }
}
