/**  版本信息模板在安装目录下，可自行修改。
* tbQCResult.cs
*
* 功 能： N/A
* 类 名： tbQCResult
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
	/// tbQCResult:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class tbQCResult
	{
		public tbQCResult()
		{}
		#region Model
		private int _qcresultid;
		private int? _qcid;
		private string _itemname;
		private int? _conclevel;
		private int? _source;
		private int? _pmtcounter;
		private string _batch;
		private double? _concentration;
		private string _concspec;
		private string _unit;
		private DateTime? _testdate;
		/// <summary>
		/// 质控结果ID
		/// </summary>
		public int QCResultID
		{
			set{ _qcresultid=value;}
			get{return _qcresultid;}
		}
		/// <summary>
		/// 质控ID
		/// </summary>
		public int? QCID
		{
			set{ _qcid=value;}
			get{return _qcid;}
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
		/// 质控浓度（1,2,3对应高中低浓度）
		/// </summary>
		public int? ConcLevel
		{
			set{ _conclevel=value;}
			get{return _conclevel;}
		}
		/// <summary>
		/// 质控来源(1内部，2外部)
		/// </summary>
		public int? Source
		{
			set{ _source=value;}
			get{return _source;}
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
		/// 质控批号
		/// </summary>
		public string Batch
		{
			set{ _batch=value;}
			get{return _batch;}
		}
		/// <summary>
		/// 浓度(质控测试值)
		/// </summary>
		public double? Concentration
		{
			set{ _concentration=value;}
			get{return _concentration;}
		}
		/// <summary>
		/// 质控规则
		/// </summary>
		public string ConcSPEC
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
		/// 测试日期
		/// </summary>
		public DateTime? TestDate
		{
			set{ _testdate=value;}
			get{return _testdate;}
		}
		#endregion Model

	}
}

