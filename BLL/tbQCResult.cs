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
using System.Data;
using System.Collections.Generic;
using LTP.Common;
using BioBaseCLIA.Model;
namespace BioBaseCLIA.BLL
{
	/// <summary>
	/// tbQCResult
	/// </summary>
	public partial class tbQCResult
	{
		private readonly BioBaseCLIA.DAL.tbQCResult dal=new BioBaseCLIA.DAL.tbQCResult();
		public tbQCResult()
		{}
		#region  BasicMethod

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
			return dal.GetMaxId();
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int QCResultID)
		{
			return dal.Exists(QCResultID);
		}

		/// <summary>
		/// 增加一条数据
		/// </summary>
		public bool Add(BioBaseCLIA.Model.tbQCResult model)
		{
			return dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(BioBaseCLIA.Model.tbQCResult model)
		{
			return dal.Update(model);
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(int QCResultID)
		{
			
			return dal.Delete(QCResultID);
		}
        ///// <summary>
        ///// 删除一条数据
        ///// </summary>
        //public bool DeleteList(string QCResultIDlist )
        //{
        //    return dal.DeleteList(Maticsoft.Common.PageValidate.SafeLongFilter(QCResultIDlist,0) );
        //}

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public BioBaseCLIA.Model.tbQCResult GetModel(int QCResultID)
		{
			
			return dal.GetModel(QCResultID);
		}

		/// <summary>
		/// 得到一个对象实体，从缓存中
		/// </summary>
		public BioBaseCLIA.Model.tbQCResult GetModelByCache(int QCResultID)
		{
			
			string CacheKey = "tbQCResultModel-" + QCResultID;
            object objModel = LTP.Common.DataCache.GetCache(CacheKey);
			if (objModel == null)
			{
				try
				{
					objModel = dal.GetModel(QCResultID);
					if (objModel != null)
					{
                        int ModelCache = LTP.Common.ConfigHelper.GetConfigInt("ModelCache");
                        LTP.Common.DataCache.SetCache(CacheKey, objModel, DateTime.Now.AddMinutes(ModelCache), TimeSpan.Zero);
					}
				}
				catch{}
			}
			return (BioBaseCLIA.Model.tbQCResult)objModel;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetList(string strWhere)
		{
			return dal.GetList(strWhere);
		}
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<BioBaseCLIA.Model.tbQCResult> GetModelList(string strWhere)
		{
			DataSet ds = dal.GetList(strWhere);
			return DataTableToList(ds.Tables[0]);
		}
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<BioBaseCLIA.Model.tbQCResult> DataTableToList(DataTable dt)
		{
			List<BioBaseCLIA.Model.tbQCResult> modelList = new List<BioBaseCLIA.Model.tbQCResult>();
			int rowsCount = dt.Rows.Count;
			if (rowsCount > 0)
			{
				BioBaseCLIA.Model.tbQCResult model;
				for (int n = 0; n < rowsCount; n++)
				{
					model = dal.DataRowToModel(dt.Rows[n]);
					if (model != null)
					{
						modelList.Add(model);
					}
				}
			}
			return modelList;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetAllList()
		{
			return GetList("");
		}

		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public int GetRecordCount(string strWhere)
		{
			return dal.GetRecordCount(strWhere);
		}
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
		{
			return dal.GetListByPage( strWhere,  orderby,  startIndex,  endIndex);
		}
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		//public DataSet GetList(int PageSize,int PageIndex,string strWhere)
		//{
			//return dal.GetList(PageSize,PageIndex,strWhere);
		//}

		#endregion  BasicMethod
		#region  ExtensionMethod

		#endregion  ExtensionMethod
	}
}

