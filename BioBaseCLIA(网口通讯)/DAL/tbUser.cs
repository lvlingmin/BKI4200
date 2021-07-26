/**  版本信息模板在安装目录下，可自行修改。
* tbUser.cs
*
* 功 能： N/A
* 类 名： tbUser
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
using System.Data;
using System.Text;
using System.Data.OleDb;
using Maticsoft.DBUtility;//Please add references
namespace BioBaseCLIA.DAL
{
	/// <summary>
	/// 数据访问类:tbUser
	/// </summary>
	public partial class tbUser
	{
		private const int connType = 2;
		public tbUser()
		{}
		#region  BasicMethod

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
		return DbHelperOleDb.GetMaxID(connType, "UserID", "tbUser"); 
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int UserID)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from tbUser");
			strSql.Append(" where UserID=@UserID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@UserID", OleDbType.Integer,4)
			};
			parameters[0].Value = UserID;

			return DbHelperOleDb.Exists(connType, strSql.ToString(),parameters);
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public bool Add(BioBaseCLIA.Model.tbUser model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into tbUser(");
			strSql.Append("UserName,UserPassword,RoleType,defaultValue)");
			strSql.Append(" values (");
			strSql.Append("@UserName,@UserPassword,@RoleType,@defaultValue)");
			OleDbParameter[] parameters = {
					new OleDbParameter("@UserName", OleDbType.VarChar,30),
					new OleDbParameter("@UserPassword", OleDbType.VarChar,30),
					new OleDbParameter("@RoleType", OleDbType.Integer,4),
					new OleDbParameter("@defaultValue", OleDbType.Integer,4)};
			parameters[0].Value = model.UserName;
			parameters[1].Value = model.UserPassword;
			parameters[2].Value = model.RoleType;
			parameters[3].Value = model.defaultValue;

			int rows=DbHelperOleDb.ExecuteSql(connType, strSql.ToString(),parameters);
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
		public bool Update(BioBaseCLIA.Model.tbUser model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update tbUser set ");
			strSql.Append("UserName=@UserName,");
			strSql.Append("UserPassword=@UserPassword,");
			strSql.Append("RoleType=@RoleType,");
			strSql.Append("defaultValue=@defaultValue");
			strSql.Append(" where UserID=@UserID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@UserName", OleDbType.VarChar,30),
					new OleDbParameter("@UserPassword", OleDbType.VarChar,30),
					new OleDbParameter("@RoleType", OleDbType.Integer,4),
					new OleDbParameter("@defaultValue", OleDbType.Integer,4),
					new OleDbParameter("@UserID", OleDbType.Integer,4)};
			parameters[0].Value = model.UserName;
			parameters[1].Value = model.UserPassword;
			parameters[2].Value = model.RoleType;
			parameters[3].Value = model.defaultValue;
			parameters[4].Value = model.UserID;

			int rows=DbHelperOleDb.ExecuteSql(connType, strSql.ToString(),parameters);
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
		public bool Delete(int UserID)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from tbUser ");
			strSql.Append(" where UserID=@UserID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@UserID", OleDbType.Integer,4)
			};
			parameters[0].Value = UserID;

			int rows=DbHelperOleDb.ExecuteSql(connType, strSql.ToString(),parameters);
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
		public bool DeleteList(string UserIDlist )
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from tbUser ");
			strSql.Append(" where UserID in ("+UserIDlist + ")  ");
			int rows=DbHelperOleDb.ExecuteSql(connType, strSql.ToString());
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
		public BioBaseCLIA.Model.tbUser GetModel(int UserID)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select UserID,UserName,UserPassword,RoleType,defaultValue from tbUser ");
			strSql.Append(" where UserID=@UserID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@UserID", OleDbType.Integer,4)
			};
			parameters[0].Value = UserID;

			BioBaseCLIA.Model.tbUser model=new BioBaseCLIA.Model.tbUser();
			DataSet ds=DbHelperOleDb.Query(connType, strSql.ToString(),parameters);
			if(ds.Tables[0].Rows.Count>0)
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
		public BioBaseCLIA.Model.tbUser DataRowToModel(DataRow row)
		{
			BioBaseCLIA.Model.tbUser model=new BioBaseCLIA.Model.tbUser();
			if (row != null)
			{
				if(row["UserID"]!=null && row["UserID"].ToString()!="")
				{
					model.UserID=int.Parse(row["UserID"].ToString());
				}
				if(row["UserName"]!=null)
				{
					model.UserName=row["UserName"].ToString();
				}
				if(row["UserPassword"]!=null)
				{
					model.UserPassword=row["UserPassword"].ToString();
				}
				if(row["RoleType"]!=null && row["RoleType"].ToString()!="")
				{
					model.RoleType=int.Parse(row["RoleType"].ToString());
				}
				if(row["defaultValue"]!=null && row["defaultValue"].ToString()!="")
				{
					model.defaultValue=int.Parse(row["defaultValue"].ToString());
				}
			}
			return model;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetList(string strWhere)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select UserID,UserName,UserPassword,RoleType,defaultValue ");
			strSql.Append(" FROM tbUser ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			return DbHelperOleDb.Query(connType, strSql.ToString());
		}

		/// <summary>
		/// 获取记录总数
		/// </summary>
		public int GetRecordCount(string strWhere)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) FROM tbUser ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
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
			StringBuilder strSql=new StringBuilder();
			strSql.Append("SELECT * FROM ( ");
			strSql.Append(" SELECT ROW_NUMBER() OVER (");
			if (!string.IsNullOrEmpty(orderby.Trim()))
			{
				strSql.Append("order by T." + orderby );
			}
			else
			{
				strSql.Append("order by T.UserID desc");
			}
			strSql.Append(")AS Row, T.*  from tbUser T ");
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
			parameters[0].Value = "tbUser";
			parameters[1].Value = "UserID";
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

