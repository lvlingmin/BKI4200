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
using System.Data;
using System.Collections.Generic;
using LTP.Common;
using BioBaseCLIA.Model;
using Maticsoft.DBUtility;
using System.Collections;
namespace BioBaseCLIA.BLL
{
	/// <summary>
	/// tbProject
	/// </summary>
	public partial class tbProject
	{
		private readonly BioBaseCLIA.DAL.tbProject dal=new BioBaseCLIA.DAL.tbProject();
		public tbProject()
		{
        }
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
		public bool Exists(int ProjectID)
		{
			return dal.Exists(ProjectID);
		}

        /// <summary>
        /// 是否存在该记录（ShrotName）
        /// </summary>
        public bool Exists_( string ShrotName)
        {
            return dal.Exists_(ShrotName);
        }

		/// <summary>
		/// 增加一条数据
		/// </summary>
		public bool Add(BioBaseCLIA.Model.tbProject model)
		{
			return dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(BioBaseCLIA.Model.tbProject model)
		{
			return dal.Update(model);
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(int ProjectID)
		{
			
			return dal.Delete(ProjectID);
		}

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete_(string ShortName)
        {

            return dal.Delete_(ShortName);
        }
		/// <summary>
		/// 删除一条数据
		/// </summary>
        //public bool DeleteList(string ProjectIDlist )
        //{
        //    return dal.DeleteList(Maticsoft.Common.PageValidate.SafeLongFilter(ProjectIDlist,0) );
        //}

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public BioBaseCLIA.Model.tbProject GetModel(int ProjectID)
		{
			
			return dal.GetModel(ProjectID);
		}

		/// <summary>
		/// 得到一个对象实体，从缓存中
		/// </summary>
		public BioBaseCLIA.Model.tbProject GetModelByCache(int ProjectID)
		{
			
			string CacheKey = "tbProjectModel-" + ProjectID;
            object objModel = LTP.Common.DataCache.GetCache(CacheKey);
			if (objModel == null)
			{
				try
				{
					objModel = dal.GetModel(ProjectID);
					if (objModel != null)
					{
                        int ModelCache = LTP.Common.ConfigHelper.GetConfigInt("ModelCache");
                        LTP.Common.DataCache.SetCache(CacheKey, objModel, DateTime.Now.AddMinutes(ModelCache), TimeSpan.Zero);
					}
				}
				catch{}
			}
			return (BioBaseCLIA.Model.tbProject)objModel;
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
		public List<BioBaseCLIA.Model.tbProject> GetModelList(string strWhere)
		{
			DataSet ds = dal.GetList(strWhere);
			return DataTableToList(ds.Tables[0]);
		}
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<BioBaseCLIA.Model.tbProject> DataTableToList(DataTable dt)
		{
			List<BioBaseCLIA.Model.tbProject> modelList = new List<BioBaseCLIA.Model.tbProject>();
			int rowsCount = dt.Rows.Count;
			if (rowsCount > 0)
			{
				BioBaseCLIA.Model.tbProject model;
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

