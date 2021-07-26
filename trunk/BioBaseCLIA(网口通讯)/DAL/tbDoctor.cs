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
using System.Data;
using System.Text;
using System.Data.OleDb;
using Maticsoft.DBUtility;//Please add references
namespace BioBaseCLIA.DAL
{
	/// <summary>
	/// 数据访问类:tbDoctor
	/// </summary>
	public partial class tbDoctor
	{
		private const int connType = 2;
		public tbDoctor()
		{
        }
		#region  BasicMethod

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
		return DbHelperOleDb.GetMaxID(connType, "DoctorID", "tbDoctor"); 
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int DoctorID)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from tbDoctor");
			strSql.Append(" where DoctorID=@DoctorID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@DoctorID", OleDbType.Integer,4)
			};
			parameters[0].Value = DoctorID;

			return DbHelperOleDb.Exists(connType, strSql.ToString(),parameters);
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public bool Add(BioBaseCLIA.Model.tbDoctor model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into tbDoctor(");
			strSql.Append("DepartmentID,DoctorName,DoctorType)");
			strSql.Append(" values (");
			strSql.Append("@DepartmentID,@DoctorName,@DoctorType)");
			OleDbParameter[] parameters = {
					new OleDbParameter("@DepartmentID", OleDbType.Integer,4),
					new OleDbParameter("@DoctorName", OleDbType.VarChar,30),
					new OleDbParameter("@DoctorType", OleDbType.Integer,4)};
			parameters[0].Value = model.DepartmentID;
			parameters[1].Value = model.DoctorName;
			parameters[2].Value = model.DoctorType;

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
		public bool Update(BioBaseCLIA.Model.tbDoctor model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update tbDoctor set ");
			strSql.Append("DepartmentID=@DepartmentID,");
			strSql.Append("DoctorName=@DoctorName,");
			strSql.Append("DoctorType=@DoctorType");
			strSql.Append(" where DoctorID=@DoctorID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@DepartmentID", OleDbType.Integer,4),
					new OleDbParameter("@DoctorName", OleDbType.VarChar,30),
					new OleDbParameter("@DoctorType", OleDbType.Integer,4),
					new OleDbParameter("@DoctorID", OleDbType.Integer,4)};
			parameters[0].Value = model.DepartmentID;
			parameters[1].Value = model.DoctorName;
			parameters[2].Value = model.DoctorType;
			parameters[3].Value = model.DoctorID;

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
		public bool Delete(int DoctorID)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from tbDoctor ");
			strSql.Append(" where DoctorID=@DoctorID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@DoctorID", OleDbType.Integer,4)
			};
			parameters[0].Value = DoctorID;

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
		public bool DeleteList(string DoctorIDlist )
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from tbDoctor ");
			strSql.Append(" where DoctorID in ("+DoctorIDlist + ")  ");
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
		public BioBaseCLIA.Model.tbDoctor GetModel(int DoctorID)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select DoctorID,DepartmentID,DoctorName,DoctorType from tbDoctor ");
			strSql.Append(" where DoctorID=@DoctorID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@DoctorID", OleDbType.Integer,4)
			};
			parameters[0].Value = DoctorID;

			BioBaseCLIA.Model.tbDoctor model=new BioBaseCLIA.Model.tbDoctor();
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
		public BioBaseCLIA.Model.tbDoctor DataRowToModel(DataRow row)
		{
			BioBaseCLIA.Model.tbDoctor model=new BioBaseCLIA.Model.tbDoctor();
			if (row != null)
			{
				if(row["DoctorID"]!=null && row["DoctorID"].ToString()!="")
				{
					model.DoctorID=int.Parse(row["DoctorID"].ToString());
				}
				if(row["DepartmentID"]!=null && row["DepartmentID"].ToString()!="")
				{
					model.DepartmentID=int.Parse(row["DepartmentID"].ToString());
				}
				if(row["DoctorName"]!=null)
				{
					model.DoctorName=row["DoctorName"].ToString();
				}
				if(row["DoctorType"]!=null && row["DoctorType"].ToString()!="")
				{
					model.DoctorType=int.Parse(row["DoctorType"].ToString());
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
			strSql.Append("select DoctorID,DepartmentID,DoctorName,DoctorType ");
			strSql.Append(" FROM tbDoctor ");
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
			strSql.Append("select count(1) FROM tbDoctor ");
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
				strSql.Append("order by T.DoctorID desc");
			}
			strSql.Append(")AS Row, T.*  from tbDoctor T ");
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
			parameters[0].Value = "tbDoctor";
			parameters[1].Value = "DoctorID";
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

