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
using System.Data;
using System.Text;
using System.Data.OleDb;
using Maticsoft.DBUtility;//Please add references
namespace BioBaseCLIA.DAL
{
	/// <summary>
	/// 数据访问类:tbProjectGroup
	/// </summary>
	public partial class tbProjectGroup
	{
		private const int connType = 0;
		public tbProjectGroup()
		{}
		#region  BasicMethod

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
			return DbHelperOleDb.GetMaxID(connType, "ProjectGroupID", "tbProjectGroup");
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int ProjectGroupID)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("select count(1) from tbProjectGroup");
			strSql.Append(" where ProjectGroupID=@ProjectGroupID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ProjectGroupID", OleDbType.Integer,4)
			};
			parameters[0].Value = ProjectGroupID;

			return DbHelperOleDb.Exists(connType, strSql.ToString(), parameters);
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public bool Add(BioBaseCLIA.Model.tbProjectGroup model)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("insert into tbProjectGroup(");
			strSql.Append("ProjectGroupNumber,ProjectNumber,GroupContent)");
			strSql.Append(" values (");
			strSql.Append("@ProjectGroupNumber,@ProjectNumber,@GroupContent)");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ProjectGroupNumber", OleDbType.VarChar,30),
					new OleDbParameter("@ProjectNumber", OleDbType.Integer,4),
					new OleDbParameter("@GroupContent", OleDbType.VarChar,250)};
			parameters[0].Value = model.ProjectGroupNumber;
			parameters[1].Value = model.ProjectNumber;
			parameters[2].Value = model.GroupContent;

			int rows = DbHelperOleDb.ExecuteSql(connType, strSql.ToString(), parameters);
			if (rows > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(BioBaseCLIA.Model.tbProjectGroup model)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("update tbProjectGroup set ");
			strSql.Append("ProjectGroupNumber=@ProjectGroupNumber,");
			strSql.Append("ProjectNumber=@ProjectNumber,");
			strSql.Append("GroupContent=@GroupContent");
			strSql.Append(" where ProjectGroupID=@ProjectGroupID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ProjectGroupNumber", OleDbType.VarChar,30),
					new OleDbParameter("@ProjectNumber", OleDbType.Integer,4),
					new OleDbParameter("@GroupContent", OleDbType.VarChar,250),
					new OleDbParameter("@ProjectGroupID", OleDbType.Integer,4)};
			parameters[0].Value = model.ProjectGroupNumber;
			parameters[1].Value = model.ProjectNumber;
			parameters[2].Value = model.GroupContent;
			parameters[3].Value = model.ProjectGroupID;

			int rows = DbHelperOleDb.ExecuteSql(connType, strSql.ToString(), parameters);
			if (rows > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(int ProjectGroupID)
		{

			StringBuilder strSql = new StringBuilder();
			strSql.Append("delete from tbProjectGroup ");
			strSql.Append(" where ProjectGroupID=@ProjectGroupID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ProjectGroupID", OleDbType.Integer,4)
			};
			parameters[0].Value = ProjectGroupID;

			int rows = DbHelperOleDb.ExecuteSql(connType, strSql.ToString(), parameters);
			if (rows > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		/// <summary>
		/// 批量删除数据
		/// </summary>
		public bool DeleteList(string ProjectGroupIDlist)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("delete from tbProjectGroup ");
			strSql.Append(" where ProjectGroupID in (" + ProjectGroupIDlist + ")  ");
			int rows = DbHelperOleDb.ExecuteSql(connType, strSql.ToString());
			if (rows > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public BioBaseCLIA.Model.tbProjectGroup GetModel(int ProjectGroupID)
		{

			StringBuilder strSql = new StringBuilder();
			strSql.Append("select ProjectGroupID,ProjectGroupNumber,ProjectNumber,GroupContent from tbProjectGroup ");
			strSql.Append(" where ProjectGroupID=@ProjectGroupID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ProjectGroupID", OleDbType.Integer,4)
			};
			parameters[0].Value = ProjectGroupID;

			BioBaseCLIA.Model.tbProjectGroup model = new BioBaseCLIA.Model.tbProjectGroup();
			DataSet ds = DbHelperOleDb.Query(connType, strSql.ToString(), parameters);
			if (ds.Tables[0].Rows.Count > 0)
			{
				return DataRowToModel(ds.Tables[0].Rows[0]);
			}
			else
			{
				return null;
			}
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public BioBaseCLIA.Model.tbProjectGroup DataRowToModel(DataRow row)
		{
			BioBaseCLIA.Model.tbProjectGroup model = new BioBaseCLIA.Model.tbProjectGroup();
			if (row != null)
			{
				if (row["ProjectGroupID"] != null && row["ProjectGroupID"].ToString() != "")
				{
					model.ProjectGroupID = int.Parse(row["ProjectGroupID"].ToString());
				}
				if (row["ProjectGroupNumber"] != null)
				{
					model.ProjectGroupNumber = row["ProjectGroupNumber"].ToString();
				}
				if (row["ProjectNumber"] != null && row["ProjectNumber"].ToString() != "")
				{
					model.ProjectNumber = int.Parse(row["ProjectNumber"].ToString());
				}
				if (row["GroupContent"] != null)
				{
					model.GroupContent = row["GroupContent"].ToString();
				}
			}
			return model;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetList(string strWhere)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("select ProjectGroupID,ProjectGroupNumber,ProjectNumber,GroupContent ");
			strSql.Append(" FROM tbProjectGroup ");
			if (strWhere.Trim() != "")
			{
				strSql.Append(" where " + strWhere);
			}
			return DbHelperOleDb.Query(connType, strSql.ToString());
		}

		/// <summary>
		/// 获取记录总数
		/// </summary>
		public int GetRecordCount(string strWhere)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("select count(1) FROM tbProjectGroup ");
			if (strWhere.Trim() != "")
			{
				strSql.Append(" where " + strWhere);
			}
			object obj = DbHelperSQL.GetSingle(connType, strSql.ToString());
			if (obj == null)
			{
				return 0;
			}
			else
			{
				return Convert.ToInt32(obj);
			}
		}
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("SELECT * FROM ( ");
			strSql.Append(" SELECT ROW_NUMBER() OVER (");
			if (!string.IsNullOrEmpty(orderby.Trim()))
			{
				strSql.Append("order by T." + orderby);
			}
			else
			{
				strSql.Append("order by T.ProjectGroupID desc");
			}
			strSql.Append(")AS Row, T.*  from tbProjectGroup T ");
			if (!string.IsNullOrEmpty(strWhere.Trim()))
			{
				strSql.Append(" WHERE " + strWhere);
			}
			strSql.Append(" ) TT");
			strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
			return DbHelperOleDb.Query(connType, strSql.ToString());
		}

		/*
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public DataSet GetList(int PageSize,int PageIndex,string strWhere)
		{
			OleDbParameter[] parameters = {
					new OleDbParameter("@tblName", OleDbType.VarChar, 255),
					new OleDbParameter("@fldName", OleDbType.VarChar, 255),
					new OleDbParameter("@PageSize", OleDbType.Integer),
					new OleDbParameter("@PageIndex", OleDbType.Integer),
					new OleDbParameter("@IsReCount", OleDbType.Boolean),
					new OleDbParameter("@OrderType", OleDbType.Boolean),
					new OleDbParameter("@strWhere", OleDbType.VarChar,1000),
					};
			parameters[0].Value = "tbProjectGroup";
			parameters[1].Value = "ProjectGroupID";
			parameters[2].Value = PageSize;
			parameters[3].Value = PageIndex;
			parameters[4].Value = 0;
			parameters[5].Value = 0;
			parameters[6].Value = strWhere;	
			return DbHelperOleDb.RunProcedure("UP_GetRecordByPage",parameters,"ds");
		}*/

		#endregion  BasicMethod
		#region  ExtensionMethod

		#endregion  ExtensionMethod
	}
}

