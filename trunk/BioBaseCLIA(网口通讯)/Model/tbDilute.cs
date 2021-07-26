/**  版本信息模板在安装目录下，可自行修改。
* tbDilute.cs
*
* 功 能： N/A
* 类 名： tbDilute
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2019/2/19 14:09:33   N/A    初版
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
	/// tbDilute:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class tbDilute
	{
		public tbDilute()
		{}
		#region Model
		private int _id;
		private string _dilutenumber;
		private int? _dilutepos;
		private int? _alldiuvol;
		private int? _leftdiuvol;
		private string _unit;
		private string _adddata;
		private string _validata;
		private int? _state;
		/// <summary>
		/// 
		/// </summary>
		public int ID
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 稀释品编号
		/// </summary>
		public string DiluteNumber
		{
			set{ _dilutenumber=value;}
			get{return _dilutenumber;}
		}
		/// <summary>
		/// 位置
		/// </summary>
		public int? DilutePos
		{
			set{ _dilutepos=value;}
			get{return _dilutepos;}
		}
		/// <summary>
		/// 稀释品总体积
		/// </summary>
		public int? AllDiuVol
		{
			set{ _alldiuvol=value;}
			get{return _alldiuvol;}
		}
		/// <summary>
		/// 剩余体积
		/// </summary>
		public int? LeftDiuVol
		{
			set{ _leftdiuvol=value;}
			get{return _leftdiuvol;}
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
		/// 添加日期
		/// </summary>
		public string AddData
		{
			set{ _adddata=value;}
			get{return _adddata;}
		}
		/// <summary>
		/// 使用有效期
		/// </summary>
		public string ValiData
		{
			set{ _validata=value;}
			get{return _validata;}
		}
		/// <summary>
		/// 稀释品状态
		/// </summary>
		public int? State
		{
			set{ _state=value;}
			get{return _state;}
		}
		#endregion Model

	}
}

