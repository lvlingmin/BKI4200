/**  版本信息模板在安装目录下，可自行修改。
* tbDoctor.cs
*
* 功 能： N/A
* 类 名： tbDoctor
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2018-01-10 18:13:43   N/A    初版
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
	/// tbDoctor:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class tbDoctor
	{
		public tbDoctor()
		{}
		#region Model
		private int _doctorid;
		private int? _departmentid;
		private string _doctorname;
		private int? _doctortype;
		/// <summary>
		/// 医生ID
		/// </summary>
		public int DoctorID
		{
			set{ _doctorid=value;}
			get{return _doctorid;}
		}
		/// <summary>
		/// 科室ID
		/// </summary>
		public int? DepartmentID
		{
			set{ _departmentid=value;}
			get{return _departmentid;}
		}
		/// <summary>
		/// 医生姓名
		/// </summary>
		public string DoctorName
		{
			set{ _doctorname=value;}
			get{return _doctorname;}
		}
		/// <summary>
		/// 医生类型
		/// </summary>
		public int? DoctorType
		{
			set{ _doctortype=value;}
			get{return _doctortype;}
		}
		#endregion Model

	}
}

