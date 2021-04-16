/**  版本信息模板在安装目录下，可自行修改。
* tbMainScalCurve.cs
*
* 功 能： N/A
* 类 名： tbMainScalCurve
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2018-03-10 9:28:46   N/A    初版
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
	/// tbMainScalCurve:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class tbMainScalCurve
	{
		public tbMainScalCurve()
		{}
		#region Model
		private int _maincurveid;
		private string _itemname;
		private string _regentbatch;
		private string _points;
		private DateTime? _activedate;
        private DateTime? _validperiod;
		/// <summary>
		/// 索引
		/// </summary>
		public int MainCurveID
		{
			set{ _maincurveid=value;}
			get{return _maincurveid;}
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
		/// 定标点
		/// </summary>
		public string Points
		{
			set{ _points=value;}
			get{return _points;}
		}
		/// <summary>
		/// 激活日期
		/// </summary>
		public DateTime? ActiveDate
		{
			set{ _activedate=value;}
			get{return _activedate;}
		}
		/// <summary>
		/// 有效期限
		/// </summary>
        public DateTime? ValidPeriod
		{
			set{ _validperiod=value;}
			get{return _validperiod;}
		}
		#endregion Model

	}
}

