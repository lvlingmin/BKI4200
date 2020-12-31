/**  版本信息模板在安装目录下，可自行修改。
* tbReagent.cs
*
* 功 能： N/A
* 类 名： tbReagent
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
	/// tbReagent:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class tbReagent
	{
		public tbReagent()
		{}
		#region Model
		private int _reagentid;
		private string _reagentnumber;
		private string _reagentname;
		private string _batch;
		private string _barcode;
		private string _status;
		private int? _alltestnumber;
		private int? _leftovertestr1;
		private int? _leftovertestr2;
		private int? _leftovertestr3;
		private int? _leftovertestr4;
		private string _validdate;
		private string _adddate;
		private string _postion;
		/// <summary>
		/// 试剂ID
		/// </summary>
		public int ReagentID
		{
			set{ _reagentid=value;}
			get{return _reagentid;}
		}
		/// <summary>
		/// 试剂条码
		/// </summary>
		public string ReagentNumber
		{
			set{ _reagentnumber=value;}
			get{return _reagentnumber;}
		}
		/// <summary>
		/// 试剂名称
		/// </summary>
		public string ReagentName
		{
			set{ _reagentname=value;}
			get{return _reagentname;}
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
		/// 试剂条码
		/// </summary>
		public string BarCode
		{
			set{ _barcode=value;}
			get{return _barcode;}
		}
		/// <summary>
		/// 试剂状态
		/// </summary>
		public string Status
		{
			set{ _status=value;}
			get{return _status;}
		}
		/// <summary>
		/// 总测试数
		/// </summary>
		public int? AllTestNumber
		{
			set{ _alltestnumber=value;}
			get{return _alltestnumber;}
		}
		/// <summary>
		/// R1剩余测试数
		/// </summary>
		public int? leftoverTestR1
		{
			set{ _leftovertestr1=value;}
			get{return _leftovertestr1;}
		}
		/// <summary>
		/// R2剩余测试数
		/// </summary>
		public int? leftoverTestR2
		{
			set{ _leftovertestr2=value;}
			get{return _leftovertestr2;}
		}
		/// <summary>
		/// R3剩余测试数
		/// </summary>
		public int? leftoverTestR3
		{
			set{ _leftovertestr3=value;}
			get{return _leftovertestr3;}
		}
		/// <summary>
		/// R4剩余测试数
		/// </summary>
		public int? leftoverTestR4
		{
			set{ _leftovertestr4=value;}
			get{return _leftovertestr4;}
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
		/// 添加日期
		/// </summary>
		public string AddDate
		{
			set{ _adddate=value;}
			get{return _adddate;}
		}
		/// <summary>
		/// 试剂当前的位置
		/// </summary>
		public string Postion
		{
			set{ _postion=value;}
			get{return _postion;}
		}
		#endregion Model

	}
}

