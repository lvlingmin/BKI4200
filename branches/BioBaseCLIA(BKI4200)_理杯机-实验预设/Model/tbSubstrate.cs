/**  版本信息模板在安装目录下，可自行修改。
* tbSubstrate.cs
*
* 功 能： N/A
* 类 名： tbSubstrate
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2018-01-10 18:14:49   N/A    初版
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
	/// tbSubstrate:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class tbSubstrate
	{
		public tbSubstrate()
		{}
		#region Model
		private int _substrateid;
		private string _substratenumber;
		private string _batch;
		private string _barcode;
		private string _status;
		private int? _alltestnumber;
		private int? _leftovertest;
		private int? _extratest;
		private string _validdate;
		private string _adddate;
		private string _postion;
		/// <summary>
		/// 底物ID
		/// </summary>
		public int SubstrateID
		{
			set{ _substrateid=value;}
			get{return _substrateid;}
		}
		/// <summary>
		/// 底物条码
		/// </summary>
		public string SubstrateNumber
		{
			set{ _substratenumber=value;}
			get{return _substratenumber;}
		}
		/// <summary>
		/// 底物批号
		/// </summary>
		public string Batch
		{
			set{ _batch=value;}
			get{return _batch;}
		}
		/// <summary>
		/// 底物条码
		/// </summary>
		public string BarCode
		{
			set{ _barcode=value;}
			get{return _barcode;}
		}
		/// <summary>
		/// 底物状态
		/// </summary>
		public string Status
		{
			set{ _status=value;}
			get{return _status;}
		}
		/// <summary>
		/// 底物总测试数
		/// </summary>
		public int? AllTestNumber
		{
			set{ _alltestnumber=value;}
			get{return _alltestnumber;}
		}
		/// <summary>
		/// 剩余的测试数
		/// </summary>
		public int? leftoverTest
		{
			set{ _leftovertest=value;}
			get{return _leftovertest;}
		}
		/// <summary>
		/// 额外的测试数
		/// </summary>
		public int? ExtraTest
		{
			set{ _extratest=value;}
			get{return _extratest;}
		}
		/// <summary>
		/// 有效日期
		/// </summary>
		public string ValidDate
		{
			set{ _validdate=value;}
			get{return _validdate;}
		}
		/// <summary>
		/// 底物添加日期
		/// </summary>
		public string AddDate
		{
			set{ _adddate=value;}
			get{return _adddate;}
		}
		/// <summary>
		/// 底物当前的位置
		/// </summary>
		public string Postion
		{
			set{ _postion=value;}
			get{return _postion;}
		}
		#endregion Model

	}
}

