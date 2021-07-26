using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioBaseCLIA.InfoSetting
{
    public class LabResult
    {
        string _obxtype;
        string _projectId;
        string _subId;
        string _resultvalue;
        string _unit;
        string _rrs;
        string _abnormalflag;
        string _resultFlag;
        string _resulttype;
        DateTime _normalLTime;
        DateTime _observetime;
        string _doctor;
        string _method;
        string _projectName;
        string _abnormaltest;
        string _beginresult;

       
        public LabResult()
        {
            _obxtype="";
            _projectId = "";
            _subId = "";
            _resultvalue = "";
            _unit = "";
            _rrs = "";
            _abnormalflag = "";
            _resultFlag = "";
            _resulttype = "";
            _normalLTime=DateTime.Now ;
            _observetime =DateTime.Now;
            _doctor = "";
            _method = "";
            _projectName = "";
            
        }
        /// <summary>
        /// 原始结果
        /// </summary>
        public string Beginresult
        {
            get { return _beginresult; }
            set { _beginresult = value; }
        }
        /// <summary>
        /// 异常测试原因
        /// </summary>
        public string Abnormaltest
        {
            get { return _abnormaltest; }
            set { _abnormaltest = value; }
        }
        /// <summary>
        /// 测试结果类型 定量NM 定性ST
        /// </summary>
        public string Obxtype
        {
            get { return _obxtype; }
            set { _obxtype = value; }
        }
        /// <summary>
        /// 项目ID号
        /// </summary>
        public string ProjectId
        {
            get { return _projectId; }
            set { _projectId = value; }
        }
        /// <summary>
        /// 项目项目名称
        /// </summary>
        public string ProjectName
        {
            get { return _projectName; }
            set { _projectName = value; }
        }

        /// <summary>
        /// 重复测试次数
        /// </summary>
        public string SubId
        {
            get { return _subId; }
            set { _subId = value; }
        }


        /// <summary>
        /// 检验结果值
        /// </summary>
        public string Resultvalue
        {
            get { return _resultvalue; }
            set { _resultvalue = value; }
        }
        /// <summary>
        /// 检验结果单位
        /// </summary>
        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }
        /// <summary>
        /// 检验结果值正常范围
        /// </summary>
        public string Rrs
        {
            get { return _rrs; }
            set { _rrs = value; }
        }
        /// <summary>
        /// 检验结果异常标志
        /// <para>L-偏低</para>
        /// <para>H-偏高</para>
        /// <para>N-正常</para>
        /// </summary>
        public string Abnormalflag
        {
            get { return _abnormalflag; }
            set { _abnormalflag = value; }
        }
        /// <summary>
        /// 结果标志，F-检测结果，C-复查结果
        /// </summary>
        public string ResultFlag
        {
            get { return _resultFlag; }
            set { _resultFlag = value; }
        }
        /// <summary>
        /// 观察结果浓度
        /// </summary>
        public string Resulttype
        {
            get { return _resulttype; }
            set { _resulttype = value; }
        }
        /// <summary>
        /// 最后观察正常值日期
        /// </summary>
        public DateTime NormalLTime
        {
            get { return _normalLTime; }
            set { _normalLTime = value; }
        }
        /// <summary>
        /// 检测日期时间
        /// </summary>
        public DateTime Observetime
        {
            get { return _observetime; }
            set { _observetime = value; }
        }
        /// <summary>
        /// 检验医生
        /// </summary>
        public string Doctor
        {
            get { return _doctor; }
            set { _doctor = value; }
        }
       /// <summary>
       /// 观察方法
       /// </summary>
        public string Method
        {
            get { return _method; }
            set { _method = value; }
        }
        /// <summary>
        /// 结果状态
        /// 2018-4-28 add
        /// F-完成测试
        /// C-测试中
        /// </summary>
        public string ResultStatus { get; set; }
        public void LoadData(string[] message)
        {
           
        }
    }
}
