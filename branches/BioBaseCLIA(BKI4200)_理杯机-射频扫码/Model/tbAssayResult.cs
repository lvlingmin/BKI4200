/**  版本信息模板在安装目录下，可自行修改。
* tbAssayResult.cs
*
* 功 能： N/A
* 类 名： tbAssayResult
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2018-01-10 18:15:03   N/A    初版
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
	/// tbAssayResult:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class tbAssayResult
	{
		public tbAssayResult()
		{}
		#region Model
		private int _assayresultid;
		private int? _sampleid;
		private string _itemname;
		private int? _pmtcounter;
		private string _batch;
		private int? _dilutecount;
		private double? _concentration;
		private string _concspec;
		private string _unit;
		private string _range;
		private string _result;
		private string _specification;
		private DateTime? _testdate;
		private int? _status;
		private string _upload;
		/// <summary>
		/// 检测结果ID
		/// </summary>
		public int AssayResultID
		{
			set{ _assayresultid=value;}
			get{return _assayresultid;}
		}
		/// <summary>
		/// 样本ID
		/// </summary>
		public int? SampleID
		{
			set{ _sampleid=value;}
			get{return _sampleid;}
		}
		/// <summary>
		/// 项目名称
		/// </summary>
		public string ItemName
		{
			set{ _itemname=value;}
			get{return _itemname;}
		}
		/// <summary>
		/// 发光值
		/// </summary>
		public int? PMTCounter
		{
			set{ _pmtcounter=value;}
			get{return _pmtcounter;}
		}
		/// <summary>
		/// 试剂批号
		/// </summary>
		public string Batch
		{
			set{ _batch=value;}
			get{return _batch;}
		}
		/// <summary>
		/// 稀释倍数
		/// </summary>
		public int? DiluteCount
		{
			set{ _dilutecount=value;}
			get{return _dilutecount;}
		}
		/// <summary>
		/// 浓度
		/// </summary>
		public double? Concentration
		{
			set{ _concentration=value;}
			get{return _concentration;}
		}
		/// <summary>
		/// 浓度规格
		/// </summary>
		public string ConcSpec
		{
			set{ _concspec=value;}
			get{return _concspec;}
		}
		/// <summary>
		/// 单位
		/// </summary>
		public string Unit
		{
			set{ _unit=value;}
			get{return _unit;}
		}
		/// <summary>
		/// 范围
		/// </summary>
		public string Range
		{
			set{ _range=value;}
			get{return _range;}
		}
		/// <summary>
		/// 结果
		/// </summary>
		public string Result
		{
			set{ _result=value;}
			get{return _result;}
		}
		/// <summary>
		/// 规范
		/// </summary>
		public string Specification
		{
			set{ _specification=value;}
			get{return _specification;}
		}
		/// <summary>
		/// 测试日期
		/// </summary>
		public DateTime? TestDate
		{
			set{ _testdate=value;}
			get{return _testdate;}
		}
		/// <summary>
		/// 测试状态
		/// </summary>
		public int? Status
		{
			set{ _status=value;}
			get{return _status;}
		}
		/// <summary>
		/// 上传
		/// </summary>
		public string Upload
		{
			set{ _upload=value;}
			get{return _upload;}
		}
		#endregion Model

	}
}

