/**  版本信息模板在安装目录下，可自行修改。
* tbProject.cs
*
* 功 能： N/A
* 类 名： tbProject
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
	/// tbProject:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class tbProject
	{
		public tbProject()
		{}
		#region Model
		private int _projectid;
		private string _projectnumber;
		private string _shortname;
		private string _fullname;
		private string _projecttype;
		private string _dilutename;
		private int? _dilutecount;
		private string _rangetype;
		private string _valuerange1;
		private string _valuerange2;
		private string _valueunit;
		private double? _minvalue;
		private double? _maxvalue;
		private int? _calpointnumber;
		private string _calpointconc;
		private int? _qcpointnumber;
		private string _qcpoints;
		private string _projectprocedure;
		private int? _calmode;
		private int? _calmethod;
		private string _calculatemethod;
		private int? _loadtype;
		private int? _activestatus;
        private int _expiryDate; //2018-07-30 zlx add
        private string _noUsePro;//2018-10-13 zlx add
        private string _vRangeType;//2019-06-06 zlx mod
        /// <summary>
        /// 实验项目ID
        /// </summary>
        public int ProjectID
		{
			set{ _projectid=value;}
			get{return _projectid;}
		}
		/// <summary>
		/// 项目编号
		/// </summary>
		public string ProjectNumber
		{
			set{ _projectnumber=value;}
			get{return _projectnumber;}
		}
		/// <summary>
		/// 项目简称
		/// </summary>
		public string ShortName
		{
			set{ _shortname=value;}
			get{return _shortname;}
		}
		/// <summary>
		/// 项目全称
		/// </summary>
		public string FullName
		{
			set{ _fullname=value;}
			get{return _fullname;}
		}
		/// <summary>
		/// 项目类型
		/// </summary>
		public string ProjectType
		{
			set{ _projecttype=value;}
			get{return _projecttype;}
		}
		/// <summary>
		/// 稀释名称
		/// </summary>
		public string DiluteName
		{
			set{ _dilutename=value;}
			get{return _dilutename;}
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
		/// 范围类型
		/// </summary>
		public string RangeType
		{
			set{ _rangetype=value;}
			get{return _rangetype;}
		}
		/// <summary>
		/// 取值范围一
		/// </summary>
		public string ValueRange1
		{
			set{ _valuerange1=value;}
			get{return _valuerange1;}
		}
		/// <summary>
		/// 取值范围二
		/// </summary>
		public string ValueRange2
		{
			set{ _valuerange2=value;}
			get{return _valuerange2;}
		}
		/// <summary>
		/// 范围值的取值单位
		/// </summary>
		public string ValueUnit
		{
			set{ _valueunit=value;}
			get{return _valueunit;}
		}
		/// <summary>
		/// 线性范围最小值
		/// </summary>
		public double? MinValue
		{
			set{ _minvalue=value;}
			get{return _minvalue;}
		}
		/// <summary>
		/// 线性范围最大值
		/// </summary>
		public double? MaxValue
		{
			set{ _maxvalue=value;}
			get{return _maxvalue;}
		}
		/// <summary>
		/// 定标点的个数
		/// </summary>
		public int? CalPointNumber
		{
			set{ _calpointnumber=value;}
			get{return _calpointnumber;}
		}
		/// <summary>
		/// 定标点的各个浓度
		/// </summary>
		public string CalPointConc
		{
			set{ _calpointconc=value;}
			get{return _calpointconc;}
		}
		/// <summary>
		/// 质控点个数
		/// </summary>
		public int? QCPointNumber
		{
			set{ _qcpointnumber=value;}
			get{return _qcpointnumber;}
		}
		/// <summary>
		/// 质控点是定标点中的哪几个
		/// </summary>
		public string QCPoints
		{
			set{ _qcpoints=value;}
			get{return _qcpoints;}
		}
		/// <summary>
		/// 实验步骤
		/// </summary>
		public string ProjectProcedure
		{
			set{ _projectprocedure=value;}
			get{return _projectprocedure;}
		}
		/// <summary>
		/// 定标模式
		/// </summary>
		public int? CalMode
		{
			set{ _calmode=value;}
			get{return _calmode;}
		}
		/// <summary>
		/// 校准方法
		/// </summary>
		public int? CalMethod
		{
			set{ _calmethod=value;}
			get{return _calmethod;}
		}
		/// <summary>
		/// 计算方法（用于定性实验）
		/// </summary>
		public string CalculateMethod
		{
			set{ _calculatemethod=value;}
			get{return _calculatemethod;}
		}
		/// <summary>
		/// 载入类型
		/// </summary>
		public int? LoadType
		{
			set{ _loadtype=value;}
			get{return _loadtype;}
		}
		/// <summary>
		/// 激活状态
		/// </summary>
		public int? ActiveStatus
		{
			set{ _activestatus=value;}
			get{return _activestatus;}
		}
        /// <summary>
        /// 定标有效期 2018-07-30 zlx add
        /// </summary>
        public int ExpiryDate
        {
            get { return _expiryDate; }
            set { _expiryDate = value; }
        }
        /// <summary>
        /// 剩余体积 2018-10-13 zlx add
        /// </summary>
        public string NoUsePro
        {
            get { return _noUsePro; }
            set { _noUsePro = value; }
        }
        /// <summary>
        /// 参考值范围类型
        /// </summary>
        public string VRangeType
        {
            get { return _vRangeType; }
            set { _vRangeType = value;}
        }
        #endregion Model

    }
}

