/**  版本信息模板在安装目录下，可自行修改。
* tbAssayResult.cs
*
* 功 能： N/A
* 类 名： tbAssayResult
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
using System.Text;
using System.Data.OleDb;
using Maticsoft.DBUtility;//Please add references
namespace BioBaseCLIA.DAL
{
	/// <summary>
	/// 数据访问类:tbAssayResult
	/// </summary>
	public partial class tbAssayResult
	{
		private const int connType = 1;
		public tbAssayResult()
		{}
		#region  BasicMethod

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
			return DbHelperOleDb.GetMaxID(connType, "AssayResultID", "tbAssayResult");
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int AssayResultID)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("select count(1) from tbAssayResult");
			strSql.Append(" where AssayResultID=@AssayResultID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@AssayResultID", OleDbType.Integer,4)
			};
			parameters[0].Value = AssayResultID;

			return DbHelperOleDb.Exists(connType, strSql.ToString(), parameters);
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public bool Add(BioBaseCLIA.Model.tbAssayResult model)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("insert into tbAssayResult(");
			strSql.Append("SampleID,ItemName,PMTCounter,Batch,DiluteCount,Concentration,ConcSpec,Unit,Range,Result,Specification,TestDate,Status,Upload)");
			strSql.Append(" values (");
			strSql.Append("@SampleID,@ItemName,@PMTCounter,@Batch,@DiluteCount,@Concentration,@ConcSpec,@Unit,@Range,@Result,@Specification,@TestDate,@Status,@Upload)");
			OleDbParameter[] parameters = {
					new OleDbParameter("@SampleID", OleDbType.Integer,4),
					new OleDbParameter("@ItemName", OleDbType.VarChar,30),
					new OleDbParameter("@PMTCounter", OleDbType.Integer,4),
					new OleDbParameter("@Batch", OleDbType.VarChar,50),//2018-08-20 zlx mod
					new OleDbParameter("@DiluteCount", OleDbType.Integer,4),
					new OleDbParameter("@Concentration", OleDbType.VarChar,20),
					new OleDbParameter("@ConcSpec", OleDbType.VarChar,64),
					new OleDbParameter("@Unit", OleDbType.VarChar,20),
					new OleDbParameter("@Range", OleDbType.VarChar,255),
					new OleDbParameter("@Result", OleDbType.VarChar,50),
					new OleDbParameter("@Specification", OleDbType.VarChar,255),
					new OleDbParameter("@TestDate", OleDbType.Date),
					new OleDbParameter("@Status", OleDbType.Integer,4),
					new OleDbParameter("@Upload", OleDbType.VarChar,255)};
			parameters[0].Value = model.SampleID;
			parameters[1].Value = model.ItemName;
			parameters[2].Value = model.PMTCounter;
			parameters[3].Value = model.Batch;
			parameters[4].Value = model.DiluteCount;
			parameters[5].Value = model.Concentration;
			parameters[6].Value = model.ConcSpec;
			parameters[7].Value = model.Unit;
			parameters[8].Value = model.Range;
			parameters[9].Value = model.Result;
			parameters[10].Value = model.Specification;
			parameters[11].Value = model.TestDate;
			parameters[12].Value = model.Status;
			parameters[13].Value = model.Upload;

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
		public bool Update(BioBaseCLIA.Model.tbAssayResult model)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("update tbAssayResult set ");
			strSql.Append("SampleID=@SampleID,");
			strSql.Append("ItemName=@ItemName,");
			strSql.Append("PMTCounter=@PMTCounter,");
			strSql.Append("Batch=@Batch,");
			strSql.Append("DiluteCount=@DiluteCount,");
			strSql.Append("Concentration=@Concentration,");
			strSql.Append("ConcSpec=@ConcSpec,");
			strSql.Append("Unit=@Unit,");
			strSql.Append("Range=@Range,");
			strSql.Append("Result=@Result,");
			strSql.Append("Specification=@Specification,");
			strSql.Append("TestDate=@TestDate,");
			strSql.Append("Status=@Status,");
			strSql.Append("Upload=@Upload");
			strSql.Append(" where AssayResultID=@AssayResultID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@SampleID", OleDbType.Integer,4),
					new OleDbParameter("@ItemName", OleDbType.VarChar,30),
					new OleDbParameter("@PMTCounter", OleDbType.Integer,4),
					new OleDbParameter("@Batch", OleDbType.VarChar,10),
					new OleDbParameter("@DiluteCount", OleDbType.Integer,4),
					new OleDbParameter("@Concentration", OleDbType.VarChar,20),
					new OleDbParameter("@ConcSpec", OleDbType.VarChar,64),
					new OleDbParameter("@Unit", OleDbType.VarChar,20),
					new OleDbParameter("@Range", OleDbType.VarChar,255),
					new OleDbParameter("@Result", OleDbType.VarChar,50),
					new OleDbParameter("@Specification", OleDbType.VarChar,255),
					new OleDbParameter("@TestDate", OleDbType.Date),
					new OleDbParameter("@Status", OleDbType.Integer,4),
					new OleDbParameter("@Upload", OleDbType.VarChar,255),
					new OleDbParameter("@AssayResultID", OleDbType.Integer,4)};
			parameters[0].Value = model.SampleID;
			parameters[1].Value = model.ItemName;
			parameters[2].Value = model.PMTCounter;
			parameters[3].Value = model.Batch;
			parameters[4].Value = model.DiluteCount;
			parameters[5].Value = model.Concentration;
			parameters[6].Value = model.ConcSpec;
			parameters[7].Value = model.Unit;
			parameters[8].Value = model.Range;
			parameters[9].Value = model.Result;
			parameters[10].Value = model.Specification;
			parameters[11].Value = model.TestDate;
			parameters[12].Value = model.Status;
			parameters[13].Value = model.Upload;
			parameters[14].Value = model.AssayResultID;

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
		public bool Delete(int AssayResultID)
		{

			StringBuilder strSql = new StringBuilder();
			strSql.Append("delete from tbAssayResult ");
			strSql.Append(" where AssayResultID=@AssayResultID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@AssayResultID", OleDbType.Integer,4)
			};
			parameters[0].Value = AssayResultID;

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
		public bool DeleteList(string AssayResultIDlist)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("delete from tbAssayResult ");
			strSql.Append(" where AssayResultID in (" + AssayResultIDlist + ")  ");
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
		public BioBaseCLIA.Model.tbAssayResult GetModel(int AssayResultID)
		{

			StringBuilder strSql = new StringBuilder();
			strSql.Append("select AssayResultID,SampleID,ItemName,PMTCounter,Batch,DiluteCount,Concentration,ConcSpec,Unit,Range,Result,Specification,TestDate,Status,Upload from tbAssayResult ");
			strSql.Append(" where AssayResultID=@AssayResultID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@AssayResultID", OleDbType.Integer,4)
			};
			parameters[0].Value = AssayResultID;

			BioBaseCLIA.Model.tbAssayResult model = new BioBaseCLIA.Model.tbAssayResult();
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
		public BioBaseCLIA.Model.tbAssayResult DataRowToModel(DataRow row)
		{
			BioBaseCLIA.Model.tbAssayResult model = new BioBaseCLIA.Model.tbAssayResult();
			if (row != null)
			{
				if (row["AssayResultID"] != null && row["AssayResultID"].ToString() != "")
				{
					model.AssayResultID = int.Parse(row["AssayResultID"].ToString());
				}
				if (row["SampleID"] != null && row["SampleID"].ToString() != "")
				{
					model.SampleID = int.Parse(row["SampleID"].ToString());
				}
				if (row["ItemName"] != null)
				{
					model.ItemName = row["ItemName"].ToString();
				}
				if (row["PMTCounter"] != null && row["PMTCounter"].ToString() != "")
				{
					model.PMTCounter = int.Parse(row["PMTCounter"].ToString());
				}
				if (row["Batch"] != null)
				{
					model.Batch = row["Batch"].ToString();
				}
				if (row["DiluteCount"] != null && row["DiluteCount"].ToString() != "")
				{
					model.DiluteCount = int.Parse(row["DiluteCount"].ToString());
				}
				//2018-4-20 zlx modified
				if (row["Concentration"] != null && row["Concentration"].ToString() != "")
				{
					model.Concentration = row["Concentration"].ToString();
				}
				if (row["ConcSpec"] != null)
				{
					model.ConcSpec = row["ConcSpec"].ToString();
				}
				if (row["Unit"] != null)
				{
					model.Unit = row["Unit"].ToString();
				}
				if (row["Range"] != null)
				{
					model.Range = row["Range"].ToString();
				}
				if (row["Result"] != null)
				{
					model.Result = row["Result"].ToString();
				}
				if (row["Specification"] != null)
				{
					model.Specification = row["Specification"].ToString();
				}
				if (row["TestDate"] != null && row["TestDate"].ToString() != "")
				{
					model.TestDate = DateTime.Parse(row["TestDate"].ToString());
				}
				if (row["Status"] != null && row["Status"].ToString() != "")
				{
					model.Status = int.Parse(row["Status"].ToString());
				}
				if (row["Upload"] != null)
				{
					model.Upload = row["Upload"].ToString();
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
			strSql.Append("select AssayResultID,SampleID,ItemName,PMTCounter,Batch,DiluteCount,Concentration,ConcSpec,Unit,Range,Result,Specification,TestDate,Status,Upload ");
			strSql.Append(" FROM tbAssayResult ");
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
			strSql.Append("select count(1) FROM tbAssayResult ");
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
				strSql.Append("order by T.AssayResultID desc");
			}
			strSql.Append(")AS Row, T.*  from tbAssayResult T ");
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
			parameters[0].Value = "tbAssayResult";
			parameters[1].Value = "AssayResultID";
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

