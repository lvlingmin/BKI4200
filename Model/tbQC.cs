/**  版本信息模板在安装目录下，可自行修改。
* tbQC.cs
*
* 功 能： N/A
* 类 名： tbQC
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
	/// tbQC:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class tbQC
	{
		public tbQC()
		{}
		#region Model
		private int _qcid;
		private string _qcnumber;
		private string _batch;
		private double? _xvalue;
		private double? _sd;
		private string _status;
		private string _projectname;
		private string _qclevel;
		private string _operatorname;
		private string _adddate;
		private string _validdate;
        private string _qcrules;
		/// <summary>
		/// 质控ID
		/// </summary>
		public int QCID
		{
			set{ _qcid=value;}
			get{return _qcid;}
		}
		/// <summary>
		/// 质控条码
		/// </summary>
		public string QCNumber
		{
			set{ _qcnumber=value;}
			get{return _qcnumber;}
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
		/// 质控靶值
		/// </summary>
		public double? XValue
		{
			set{ _xvalue=value;}
			get{return _xvalue;}
		}
		/// <summary>
		/// 标准差
		/// </summary>
		public double? SD
		{
			set{ _sd=value;}
			get{return _sd;}
		}
		/// <summary>
		/// 质控当前的使用状态
		/// </summary>
		public string Status
		{
			set{ _status=value;}
			get{return _status;}
		}
		/// <summary>
		/// 质控品对应项目名称
		/// </summary>
		public string ProjectName
		{
			set{ _projectname=value;}
			get{return _projectname;}
		}
		/// <summary>
		/// 质控的高中低类别
		/// </summary>
		public string QCLevel
		{
			set{ _qclevel=value;}
			get{return _qclevel;}
		}
		/// <summary>
		/// 质控品录入人姓名
		/// </summary>
		public string OperatorName
		{
			set{ _operatorname=value;}
			get{return _operatorname;}
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
		/// 有效期
		/// </summary>
		public string ValidDate
		{
			set{ _validdate=value;}
			get{return _validdate;}
		}
		/// <summary>
		/// 质控的来源（内部还是外部）改为  质控规则
		/// </summary>
        public string QCRules
		{
            set { _qcrules = value; }
            get { return _qcrules; }
		}
		#endregion Model

	}
}

