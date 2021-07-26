using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace BioBaseCLIA.InfoSetting
{
    public class ServeyReport
    {
        string _porderNum, _forderNum, _unserviceId,  _rDate, _oDate, _oEDate,  _collectIdent, _spcCode, _relevantInfo, _receiveTime, _source;
        string _oProvider, _oCallbackNum, _simpleState, _xdCode, _filletF1, _filletF2, _reseltDate, _resuleState,_collectV;

        bool _priority, _bdilute;
        int _testResult;
        string _collectSite;//样本位置 2018-4-28 add
        string _chargepractice;//质控规则   2018-5-17

      
        List<string> _projectInfo;
        public ServeyReport()
        {
            _testResult = -1;
            _porderNum  = "";
            _forderNum = "";
            _unserviceId = "";
            _priority = false;
            _rDate  = "";
            _oDate  = "";
            _oEDate  = "";
            _collectV = "";
            _collectIdent  = "";
            _collectSite = "";
            _spcCode  = "";
            _relevantInfo  = "";
            _receiveTime  = "";
            _source = "";;
            _oProvider = "";
            _oCallbackNum = "";
            _simpleState  = "";
            _xdCode  = "";
            _filletF1 = "";
            _filletF2 = "";
            _reseltDate  = "";
            _resuleState = "";
            _bdilute = false;
        }
        /// <summary>
        /// 质控-质控规则
        /// </summary>
        public string Chargepractice
        {
            get { return _chargepractice; }
            set { _chargepractice = value; }
        }
        /// <summary>
        /// 测试结果类型 
        /// <para>0病人样本测试结果</para>
        /// <para>2质控结果</para>
        /// </summary>
        public int TestResult
        {
            get { return _testResult; }
            set { _testResult = value; }
        }
        /// <summary>
        /// 样本-样本条码号
        /// 质控-项目编号
        /// </summary>
        public string PorderNum
        {
            get { return _porderNum; }
            set { _porderNum = value; }
        }
        /// <summary>
        ///样本- 样本编号
        ///质控-项目名称
        /// </summary>
        public string ForderNum
        {
            get { return _forderNum; }
            set { _forderNum = value; }
        }
        /// <summary>
        /// 通用服务标识符，用作仪器型号
        /// </summary>
        public string UnserviceId
        {
            get { return _unserviceId; }
            set { _unserviceId = value; }
        }
        /// <summary>
        /// 样本-急诊标识
        /// 质控-优先级
        /// </summary>
        public bool Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }
        /// <summary>
        /// 请求时间/日期
        /// </summary>
        public string RDate
        {
            get { return _rDate; }
            set { _rDate = value; }
        }
        /// <summary>
        /// 观察时间日期
        /// </summary>
        public string ODate
        {
            get { return _oDate; }
            set { _oDate = value; }
        }
        /// <summary>
        /// 观察结束时间日期
        /// </summary>
        public string OEDate
        {
            get { return _oEDate; }
            set { _oEDate = value; }
        }
        /// <summary>
        /// (样本)重复测试次数,样本采集量
        /// </summary>
        public string CollectV
        {
            get { return _collectV; }
            set { _collectV = value; }
        }
        /// <summary>
        /// 样本架
        /// </summary>
        public string CollectIdent
        {
            get { return _collectIdent; }
            set { _collectIdent = value; }
        }
        /// <summary>
        /// 样本位置
        /// </summary>
        public string CollectSite
        {
            get { return _collectSite; }
            set { _collectSite = value; }
        }
        /// <summary>
        /// （样本）样本处理代码 （质控）质控次数
        /// </summary>
        public string SpcCode
        {
            get { return _spcCode; }
            set { _spcCode = value; }
        }
       
        /// <summary>
        /// （样本）稀释标志(质控)质控编号 
        /// </summary>
        public bool Bdilute
        {
            get { return _bdilute; }
            set { _bdilute = value; }
        }
        /// <summary>
        /// (样本)临床诊断信息(质控)质控名称
        /// </summary>
        public string RelevantInfo
        {
            get { return _relevantInfo; }
            set { _relevantInfo = value; }
        }
        /// <summary>
        /// 送检日期时间
        /// </summary>
        public string ReceiveTime
        {
            get { return _receiveTime; }
            set { _receiveTime = value; }
        }
        /// <summary>
        /// 样本来源，样本类型
        /// <para>0-血清</para>
        /// <para >1-尿液</para>
        /// <para >2-血浆</para>
        /// <para>3-脑脊液</para>
        /// <para>4-胸腹水</para>
        /// <para>5-全血</para>
        /// <para>6-其他</para>
        /// 质控-质控液有效期
        /// </summary>
        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }
        /// <summary>
        /// (样本)送检医生（质控）质控批号
        /// </summary>
        public string OProvider
        {
            get { return _oProvider; }
            set { _oProvider = value; }
        }
        /// <summary>
        /// (样本)送检科室
        /// （质控）质控液水平 H-高 M-中 L-低
        /// </summary>
        public string OCallbackNum
        {
            get { return _oCallbackNum; }
            set { _oCallbackNum = value; }
        }
        /// <summary>
        /// (样本)样本性状(质控)标准差
        /// </summary>
        public string SimpleState
        {
            get { return _simpleState; }
            set { _simpleState = value; }
        }
        /// <summary>
        /// (样本)血袋编号(质控)质控结果
        /// </summary>
        public string XdCode
        {
            get { return _xdCode; }
            set { _xdCode = value; }
        }
        /// <summary>
        /// (样本)检验医生(质控)结果单位
        /// </summary>
        public string FilletF1
        {
            get { return _filletF1; }
            set { _filletF1 = value; }
        }
        /// <summary>
        /// (样本)治疗科室(质控)结果标志(质控规则)
        /// </summary>
        public string FilletF2
        {
            get { return _filletF2; }
            set { _filletF2 = value; }
        }
        /// <summary>
        /// 结果报告日期时间
        /// </summary>
        public string ReseltDate
        {
            get { return _reseltDate; }
            set { _reseltDate = value; }
        }
        /// <summary>
        /// 结果状态
        /// F-完成测试
        /// C-测试中
        /// </summary>
        public string ResuleState
        {
            get { return _resuleState; }
            set { _resuleState = value; }
        }
        //检查项目列表
        public List<string> ProjectInfo
        {
            get { return _projectInfo; }
            set { _projectInfo = value; }
        }
        public void LoadData(string[] info)
        {
            PorderNum = info[2];
            ForderNum = info[3];
            UnserviceId = info[4];
            if (info[5] != "")
            {
                Priority = Convert.ToBoolean(info[5]);
            }
            RDate = info[6];
            ODate = info[7];
            OEDate = info[8];
            CollectV = info[9];
            CollectIdent = info[10];
            SpcCode = info[11];
            RelevantInfo = info[13];
            ReceiveTime = info[14];
            Source = info[15];
            OProvider = info[16];
            OCallbackNum = info[17];
            SimpleState = info[18];
            XdCode = info[19];
            FilletF1 = info[20];
            FilletF2 = info[21];
            ReseltDate = info[22];
            ResuleState = info[25];
            if (info[12] != "")
            {
                Bdilute = Convert.ToBoolean(info[12]);
            }
        } 

    }
}
