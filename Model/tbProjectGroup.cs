/**  版本信息模板在安装目录下，可自行修改。
* tbProjectGroup.cs
*
* 功 能： N/A
* 类 名： tbProjectGroup
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2018-01-10 18:14:27   N/A    初版
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
	/// tbProjectGroup:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class tbProjectGroup
	{
		public tbProjectGroup()
		{}
		#region Model
		private int _projectgroupid;
		private string _projectgroupnumber;
		private int? _projectnumber;
		private string _groupcontent;
		/// <summary>
		/// 组合项目ID
		/// </summary>
		public int ProjectGroupID
		{
			set{ _projectgroupid=value;}
			get{return _projectgroupid;}
		}
		/// <summary>
		/// 组合项目名称
		/// </summary>
		public string ProjectGroupNumber
		{
			set{ _projectgroupnumber=value;}
			get{return _projectgroupnumber;}
		}
		/// <summary>
		/// 项目个数
		/// </summary>
		public int? ProjectNumber
		{
			set{ _projectnumber=value;}
			get{return _projectnumber;}
		}
		/// <summary>
		/// 组合项目内容（含有项目名称列表）
		/// </summary>
		public string GroupContent
		{
			set{ _groupcontent=value;}
			get{return _groupcontent;}
		}
		#endregion Model

	}
}

