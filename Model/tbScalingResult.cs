/**  版本信息模板在安装目录下，可自行修改。
* tbScalingResult.cs
*
* 功 能： N/A
* 类 名： tbScalingResult
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
	/// tbScalingResult:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class tbScalingResult
	{
		public tbScalingResult()
		{}
		#region Model
		private int _scalingresultid;
		private string _itemname;
		private string _regentbatch;
		private int? _scalingmodel;
		private DateTime? _activedate;
		private int? _pointcount;
		private string _points;
		private int? _status;
		private int? _source;
		/// <summary>
		/// 定标结果ID
		/// </summary>
		public int ScalingResultID
		{
			set{ _scalingresultid=value;}
			get{return _scalingresultid;}
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
		/// 试剂批号
		/// </summary>
		public string RegentBatch
		{
			set{ _regentbatch=value;}
			get{return _regentbatch;}
		}
		/// <summary>
		/// 定标模式（两点或六点定标）
		/// </summary>
		public int? ScalingModel
		{
			set{ _scalingmodel=value;}
			get{return _scalingmodel;}
		}
		/// <summary>
		/// 定标曲线激活日期
		/// </summary>
		public DateTime? ActiveDate
		{
			set{ _activedate=value;}
			get{return _activedate;}
		}
		/// <summary>
		/// 定标点个数
		/// </summary>
		public int? PointCount
		{
			set{ _pointcount=value;}
			get{return _pointcount;}
		}
		/// <summary>
		/// 定标点
		/// </summary>
		public string Points
		{
			set{ _points=value;}
			get{return _points;}
		}
		/// <summary>
		/// 定标曲线状态（是否为当前定标）
		/// </summary>
		public int? Status
		{
			set{ _status=value;}
			get{return _status;}
		}
		/// <summary>
		/// 定标来源
		/// </summary>
		public int? Source
		{
			set{ _source=value;}
			get{return _source;}
		}
		#endregion Model

	}
}

