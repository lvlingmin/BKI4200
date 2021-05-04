/**  版本信息模板在安装目录下，可自行修改。
* tbSampleInfo.cs
*
* 功 能： N/A
* 类 名： tbSampleInfo
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2018-01-10 18:15:04   N/A    初版
*
* Copyright (c) 2012 Maticsoft Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：动软卓越（北京）科技有限公司　　　　　　　　　　　　　　│
*└──────────────────────────────────┘
*/
using System;
namespace BioBaseCLIA.Model
{
	/// <summary>
	/// tbSampleInfo:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class tbSampleInfo
	{
		public tbSampleInfo()
		{ }
		#region Model
		private int _sampleid;
		private string _sampleno;
		private string _patientname;
		private string _sex;
		private double? _age;
		private string _sampletype;
		private string _source;
		private string _position;
		private string _samplecontainer;
		private int? _repeatcount;
		private string _regentbatch;
		private string _projectname;
		private int? _emergency;
		private string _clinicno;
		private string _inpatientarea;
		private string _ward;
		private string _bedno;
		private string _medicarecordno;
		private string _diagnosis;
		private string _department;
		private string _senddoctor;
		private DateTime? _senddatetime;
		private string _inspectdoctor;
		private int? _status;
		private string _checkdoctor;//2018-11-12 zlx add
		private string _inspectionitems;//20210420 lyq add
		private DateTime? _acquisitiontime;//20210420 lyq add
		/// <summary>
		/// 
		/// </summary>
		public int SampleID
		{
			set { _sampleid = value; }
			get { return _sampleid; }
		}
		/// <summary>
		/// 样本编号
		/// </summary>
		public string SampleNo
		{
			set { _sampleno = value; }
			get { return _sampleno; }
		}
		/// <summary>
		/// 病人姓名
		/// </summary>
		public string PatientName
		{
			set { _patientname = value; }
			get { return _patientname; }
		}
		/// <summary>
		/// 性别
		/// </summary>
		public string Sex
		{
			set { _sex = value; }
			get { return _sex; }
		}
		/// <summary>
		/// 年龄
		/// </summary>
		public double? Age
		{
			set { _age = value; }
			get { return _age; }
		}
		/// <summary>
		/// 样本类型
		/// </summary>
		public string SampleType
		{
			set { _sampletype = value; }
			get { return _sampletype; }
		}
		/// <summary>
		/// 样本来源
		/// </summary>
		public string Source
		{
			set { _source = value; }
			get { return _source; }
		}
		/// <summary>
		/// 样本位号
		/// </summary>
		public string Position
		{
			set { _position = value; }
			get { return _position; }
		}
		/// <summary>
		/// 样本杯类型
		/// </summary>
		public string SampleContainer
		{
			set { _samplecontainer = value; }
			get { return _samplecontainer; }
		}
		/// <summary>
		/// 重复测试数
		/// </summary>
		public int? RepeatCount
		{
			set { _repeatcount = value; }
			get { return _repeatcount; }
		}
		/// <summary>
		/// 试剂批号
		/// </summary>
		public string RegentBatch
		{
			set { _regentbatch = value; }
			get { return _regentbatch; }
		}
		/// <summary>
		/// 测试项目
		/// </summary>
		public string ProjectName
		{
			set { _projectname = value; }
			get { return _projectname; }
		}
		/// <summary>
		/// 急诊
		/// </summary>
		public int? Emergency
		{
			set { _emergency = value; }
			get { return _emergency; }
		}
		/// <summary>
		/// 门诊号
		/// </summary>
		public string ClinicNo
		{
			set { _clinicno = value; }
			get { return _clinicno; }
		}
		/// <summary>
		/// 病区
		/// </summary>
		public string InpatientArea
		{
			set { _inpatientarea = value; }
			get { return _inpatientarea; }
		}
		/// <summary>
		/// 病房
		/// </summary>
		public string Ward
		{
			set { _ward = value; }
			get { return _ward; }
		}
		/// <summary>
		/// 床号
		/// </summary>
		public string BedNo
		{
			set { _bedno = value; }
			get { return _bedno; }
		}
		/// <summary>
		/// 病历号
		/// </summary>
		public string MedicaRecordNo
		{
			set { _medicarecordno = value; }
			get { return _medicarecordno; }
		}
		/// <summary>
		/// 临床诊断
		/// </summary>
		public string Diagnosis
		{
			set { _diagnosis = value; }
			get { return _diagnosis; }
		}
		/// <summary>
		/// 送检科室
		/// </summary>
		public string Department
		{
			set { _department = value; }
			get { return _department; }
		}
		/// <summary>
		/// 送检医生
		/// </summary>
		public string SendDoctor
		{
			set { _senddoctor = value; }
			get { return _senddoctor; }
		}
		/// <summary>
		/// 送检日期
		/// </summary>
		public DateTime? SendDateTime
		{
			set { _senddatetime = value; }
			get { return _senddatetime; }
		}
		/// <summary>
		/// 检验医生
		/// </summary>
		public string InspectDoctor
		{
			set { _inspectdoctor = value; }
			get { return _inspectdoctor; }
		}
		/// <summary>
		/// 状态
		/// </summary>
		public int? Status
		{
			set { _status = value; }
			get { return _status; }
		}
		/// <summary>
		/// 审核医生 2018-11-12 zlx add
		/// </summary>
		public string CheckDoctor
		{
			get { return _checkdoctor; }
			set { _checkdoctor = value; }
		}
		/// <summary>
		/// 检验项目 20210420 lyq add
		/// </summary>
		public string InspectionItems
		{
			get { return _inspectionitems; }
			set { _inspectionitems = value; }
		}
		/// <summary>
		/// 采集日期 20210420 lyq add
		/// </summary>
		public DateTime? AcquisitionTime
		{
			get { return _acquisitiontime; }
			set { _acquisitiontime = value; }
		}
		#endregion Model

	}
}

