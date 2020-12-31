/**  版本信息模板在安装目录下，可自行修改。
* tbReagent.cs
*
* 功 能： N/A
* 类 名： tbReagent
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
using System.Data;
using System.Text;
using System.Data.OleDb;
using Maticsoft.DBUtility;//Please add references
namespace BioBaseCLIA.DAL
{
	/// <summary>
	/// 数据访问类:tbReagent
	/// </summary>
	public partial class tbReagent
	{
		private const int connType = 3;
		public tbReagent()
		{}
		#region  BasicMethod

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
			return DbHelperOleDb.GetMaxID(connType, "ReagentID", "tbReagent");
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int ReagentID)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("select count(1) from tbReagent");
			strSql.Append(" where ReagentID=@ReagentID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ReagentID", OleDbType.Integer,4)
			};
			parameters[0].Value = ReagentID;

			return DbHelperOleDb.Exists(connType, strSql.ToString(), parameters);
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public bool Add(BioBaseCLIA.Model.tbReagent model)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("insert into tbReagent(");
			strSql.Append("ReagentNumber,ReagentName,Batch,BarCode,Status,AllTestNumber,leftoverTestR1,leftoverTestR2,leftoverTestR3,leftoverTestR4,ValidDate,AddDate,Postion)");
			strSql.Append(" values (");
			strSql.Append("@ReagentNumber,@ReagentName,@Batch,@BarCode,@Status,@AllTestNumber,@leftoverTestR1,@leftoverTestR2,@leftoverTestR3,@leftoverTestR4,@ValidDate,@AddDate,@Postion)");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ReagentNumber", OleDbType.VarChar,30),
					new OleDbParameter("@ReagentName", OleDbType.VarChar,30),
					new OleDbParameter("@Batch", OleDbType.VarChar,30),
					new OleDbParameter("@BarCode", OleDbType.VarChar,30),
					new OleDbParameter("@Status", OleDbType.VarChar,30),
					new OleDbParameter("@AllTestNumber", OleDbType.Integer,4),
					new OleDbParameter("@leftoverTestR1", OleDbType.Integer,4),
					new OleDbParameter("@leftoverTestR2", OleDbType.Integer,4),
					new OleDbParameter("@leftoverTestR3", OleDbType.Integer,4),
					new OleDbParameter("@leftoverTestR4", OleDbType.Integer,4),
					new OleDbParameter("@ValidDate", OleDbType.VarChar,30),
					new OleDbParameter("@AddDate", OleDbType.VarChar,30),
					new OleDbParameter("@Postion", OleDbType.VarChar,30)};
			parameters[0].Value = model.ReagentNumber;
			parameters[1].Value = model.ReagentName;
			parameters[2].Value = model.Batch;
			parameters[3].Value = model.BarCode;
			parameters[4].Value = model.Status;
			parameters[5].Value = model.AllTestNumber;
			parameters[6].Value = model.leftoverTestR1;
			parameters[7].Value = model.leftoverTestR2;
			parameters[8].Value = model.leftoverTestR3;
			parameters[9].Value = model.leftoverTestR4;
			parameters[10].Value = model.ValidDate;
			parameters[11].Value = model.AddDate;
			parameters[12].Value = model.Postion;

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
		public bool Update(BioBaseCLIA.Model.tbReagent model)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("update tbReagent set ");
			strSql.Append("ReagentNumber=@ReagentNumber,");
			strSql.Append("ReagentName=@ReagentName,");
			strSql.Append("Batch=@Batch,");
			strSql.Append("BarCode=@BarCode,");
			strSql.Append("Status=@Status,");
			strSql.Append("AllTestNumber=@AllTestNumber,");
			strSql.Append("leftoverTestR1=@leftoverTestR1,");
			strSql.Append("leftoverTestR2=@leftoverTestR2,");
			strSql.Append("leftoverTestR3=@leftoverTestR3,");
			strSql.Append("leftoverTestR4=@leftoverTestR4,");
			strSql.Append("ValidDate=@ValidDate,");
			strSql.Append("AddDate=@AddDate,");
			strSql.Append("Postion=@Postion");
			strSql.Append(" where ReagentID=@ReagentID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ReagentNumber", OleDbType.VarChar,30),
					new OleDbParameter("@ReagentName", OleDbType.VarChar,30),
					new OleDbParameter("@Batch", OleDbType.VarChar,30),
					new OleDbParameter("@BarCode", OleDbType.VarChar,30),
					new OleDbParameter("@Status", OleDbType.VarChar,30),
					new OleDbParameter("@AllTestNumber", OleDbType.Integer,4),
					new OleDbParameter("@leftoverTestR1", OleDbType.Integer,4),
					new OleDbParameter("@leftoverTestR2", OleDbType.Integer,4),
					new OleDbParameter("@leftoverTestR3", OleDbType.Integer,4),
					new OleDbParameter("@leftoverTestR4", OleDbType.Integer,4),
					new OleDbParameter("@ValidDate", OleDbType.VarChar,30),
					new OleDbParameter("@AddDate", OleDbType.VarChar,30),
					new OleDbParameter("@Postion", OleDbType.VarChar,30),
					new OleDbParameter("@ReagentID", OleDbType.Integer,4)};
			parameters[0].Value = model.ReagentNumber;
			parameters[1].Value = model.ReagentName;
			parameters[2].Value = model.Batch;
			parameters[3].Value = model.BarCode;
			parameters[4].Value = model.Status;
			parameters[5].Value = model.AllTestNumber;
			parameters[6].Value = model.leftoverTestR1;
			parameters[7].Value = model.leftoverTestR2;
			parameters[8].Value = model.leftoverTestR3;
			parameters[9].Value = model.leftoverTestR4;
			parameters[10].Value = model.ValidDate;
			parameters[11].Value = model.AddDate;
			parameters[12].Value = model.Postion;
			parameters[13].Value = model.ReagentID;

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
		/// 更新一条数据中的部分数据
		/// </summary>
		public bool UpdatePart(BioBaseCLIA.Model.tbReagent model)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("update tbReagent set ");
			strSql.Append("ReagentName=@ReagentName,");
			strSql.Append("Batch=@Batch,");
			strSql.Append("BarCode=@BarCode,");
			strSql.Append("Status=@Status,");
			strSql.Append("AllTestNumber=@AllTestNumber,");
			strSql.Append("leftoverTestR1=@leftoverTestR1,");
			strSql.Append("Postion=@Postion");
			strSql.Append(" where ReagentID=@ReagentID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ReagentName", OleDbType.VarChar,30),
					new OleDbParameter("@Batch", OleDbType.VarChar,30),
					new OleDbParameter("@BarCode", OleDbType.VarChar,30),
					new OleDbParameter("@Status", OleDbType.VarChar,30),
					new OleDbParameter("@AllTestNumber", OleDbType.Integer,4),
					new OleDbParameter("@leftoverTestR1", OleDbType.Integer,4),
					new OleDbParameter("@Postion", OleDbType.VarChar,30),
					new OleDbParameter("@ReagentID", OleDbType.Integer,4)};
			parameters[0].Value = model.ReagentName;
			parameters[1].Value = model.Batch;
			parameters[2].Value = model.BarCode;
			parameters[3].Value = model.Status;
			parameters[4].Value = model.AllTestNumber;
			parameters[5].Value = model.leftoverTestR1;
			parameters[6].Value = model.Postion;
			parameters[7].Value = model.ReagentID;

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
		public bool Delete(int ReagentID)
		{

			StringBuilder strSql = new StringBuilder();
			strSql.Append("delete from tbReagent ");
			strSql.Append(" where ReagentID=@ReagentID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ReagentID", OleDbType.Integer,4)
			};
			parameters[0].Value = ReagentID;

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
		public bool DeleteList(string ReagentIDlist)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("delete from tbReagent ");
			strSql.Append(" where ReagentID in (" + ReagentIDlist + ")  ");
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
		public BioBaseCLIA.Model.tbReagent GetModel(int ReagentID)
		{

			StringBuilder strSql = new StringBuilder();
			strSql.Append("select ReagentID,ReagentNumber,ReagentName,Batch,BarCode,Status,AllTestNumber,leftoverTestR1,leftoverTestR2,leftoverTestR3,leftoverTestR4,ValidDate,AddDate,Postion from tbReagent ");
			strSql.Append(" where ReagentID=@ReagentID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ReagentID", OleDbType.Integer,4)
			};
			parameters[0].Value = ReagentID;

			BioBaseCLIA.Model.tbReagent model = new BioBaseCLIA.Model.tbReagent();
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
		public BioBaseCLIA.Model.tbReagent DataRowToModel(DataRow row)
		{
			BioBaseCLIA.Model.tbReagent model = new BioBaseCLIA.Model.tbReagent();
			if (row != null)
			{
				if (row["ReagentID"] != null && row["ReagentID"].ToString() != "")
				{
					model.ReagentID = int.Parse(row["ReagentID"].ToString());
				}
				if (row["ReagentNumber"] != null)
				{
					model.ReagentNumber = row["ReagentNumber"].ToString();
				}
				if (row["ReagentName"] != null)
				{
					model.ReagentName = row["ReagentName"].ToString();
				}
				if (row["Batch"] != null)
				{
					model.Batch = row["Batch"].ToString();
				}
				if (row["BarCode"] != null)
				{
					model.BarCode = row["BarCode"].ToString();
				}
				if (row["Status"] != null)
				{
					model.Status = row["Status"].ToString();
				}
				if (row["AllTestNumber"] != null && row["AllTestNumber"].ToString() != "")
				{
					model.AllTestNumber = int.Parse(row["AllTestNumber"].ToString());
				}
				if (row["leftoverTestR1"] != null && row["leftoverTestR1"].ToString() != "")
				{
					model.leftoverTestR1 = int.Parse(row["leftoverTestR1"].ToString());
				}
				if (row["leftoverTestR2"] != null && row["leftoverTestR2"].ToString() != "")
				{
					model.leftoverTestR2 = int.Parse(row["leftoverTestR2"].ToString());
				}
				if (row["leftoverTestR3"] != null && row["leftoverTestR3"].ToString() != "")
				{
					model.leftoverTestR3 = int.Parse(row["leftoverTestR3"].ToString());
				}
				if (row["leftoverTestR4"] != null && row["leftoverTestR4"].ToString() != "")
				{
					model.leftoverTestR4 = int.Parse(row["leftoverTestR4"].ToString());
				}
				if (row["ValidDate"] != null)
				{
					model.ValidDate = row["ValidDate"].ToString();
				}
				if (row["AddDate"] != null)
				{
					model.AddDate = row["AddDate"].ToString();
				}
				if (row["Postion"] != null)
				{
					model.Postion = row["Postion"].ToString();
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
			strSql.Append("select ReagentID,ReagentNumber,ReagentName,Batch,BarCode,Status,AllTestNumber,leftoverTestR1,leftoverTestR2,leftoverTestR3,leftoverTestR4,ValidDate,AddDate,Postion ");
			strSql.Append(" FROM tbReagent ");
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
			strSql.Append("select count(1) FROM tbReagent ");
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
				strSql.Append("order by T.ReagentID desc");
			}
			strSql.Append(")AS Row, T.*  from tbReagent T ");
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
			parameters[0].Value = "tbReagent";
			parameters[1].Value = "ReagentID";
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

