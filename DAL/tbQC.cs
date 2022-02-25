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
using System.Data;
using System.Text;
using System.Data.OleDb;
using Maticsoft.DBUtility;//Please add references
namespace BioBaseCLIA.DAL
{
	/// <summary>
	/// 数据访问类:tbQC
	/// </summary>
	public partial class tbQC
	{
		private const int connType = 3;
		public tbQC()
		{}
		#region  BasicMethod

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
			return DbHelperOleDb.GetMaxID(connType, "QCID", "tbQC");
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int QCID)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("select count(1) from tbQC");
			strSql.Append(" where QCID=@QCID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@QCID", OleDbType.Integer,4)
			};
			parameters[0].Value = QCID;

			return DbHelperOleDb.Exists(connType, strSql.ToString(), parameters);
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public bool Add(BioBaseCLIA.Model.tbQC model)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("insert into tbQC(");
			strSql.Append("QCNumber,Batch,XValue,SD,Status,ProjectName,QCLevel,OperatorName,AddDate,ValidDate,QCRules)");
			strSql.Append(" values (");
			strSql.Append("@QCNumber,@Batch,@XValue,@SD,@Status,@ProjectName,@QCLevel,@OperatorName,@AddDate,@ValidDate,@QCRules)");
			OleDbParameter[] parameters = {
					new OleDbParameter("@QCNumber", OleDbType.VarChar,30),
					new OleDbParameter("@Batch", OleDbType.VarChar,30),
					new OleDbParameter("@XValue", OleDbType.Double),
					new OleDbParameter("@SD", OleDbType.Double),
					new OleDbParameter("@Status", OleDbType.VarChar,30),
					new OleDbParameter("@ProjectName", OleDbType.VarChar,30),
					new OleDbParameter("@QCLevel", OleDbType.VarChar,30),
					new OleDbParameter("@OperatorName", OleDbType.VarChar,30),
					new OleDbParameter("@AddDate", OleDbType.VarChar,30),
					new OleDbParameter("@ValidDate", OleDbType.VarChar,30),
					new OleDbParameter("@QCRules", OleDbType.VarChar,30)};
			parameters[0].Value = model.QCNumber;
			parameters[1].Value = model.Batch;
			parameters[2].Value = model.XValue;
			parameters[3].Value = model.SD;
			parameters[4].Value = model.Status;
			parameters[5].Value = model.ProjectName;
			parameters[6].Value = model.QCLevel;
			parameters[7].Value = model.OperatorName;
			parameters[8].Value = model.AddDate;
			parameters[9].Value = model.ValidDate;
			parameters[10].Value = model.QCRules;

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
		public bool Update(BioBaseCLIA.Model.tbQC model)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("update tbQC set ");
			strSql.Append("QCNumber=@QCNumber,");
			strSql.Append("Batch=@Batch,");
			strSql.Append("XValue=@XValue,");
			strSql.Append("SD=@SD,");
			strSql.Append("Status=@Status,");
			strSql.Append("ProjectName=@ProjectName,");
			strSql.Append("QCLevel=@QCLevel,");
			strSql.Append("OperatorName=@OperatorName,");
			strSql.Append("AddDate=@AddDate,");
			strSql.Append("ValidDate=@ValidDate,");
			strSql.Append("QCRules=@QCRules");
			strSql.Append(" where QCID=@QCID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@QCNumber", OleDbType.VarChar,30),
					new OleDbParameter("@Batch", OleDbType.VarChar,30),
					new OleDbParameter("@XValue", OleDbType.Double),
					new OleDbParameter("@SD", OleDbType.Double),
					new OleDbParameter("@Status", OleDbType.VarChar,30),
					new OleDbParameter("@ProjectName", OleDbType.VarChar,30),
					new OleDbParameter("@QCLevel", OleDbType.VarChar,30),
					new OleDbParameter("@OperatorName", OleDbType.VarChar,30),
					new OleDbParameter("@AddDate", OleDbType.VarChar,30),
					new OleDbParameter("@ValidDate", OleDbType.VarChar,30),
					new OleDbParameter("@QCRules", OleDbType.VarChar,30),
					new OleDbParameter("@QCID", OleDbType.Integer,4)};
			parameters[0].Value = model.QCNumber;
			parameters[1].Value = model.Batch;
			parameters[2].Value = model.XValue;
			parameters[3].Value = model.SD;
			parameters[4].Value = model.Status;
			parameters[5].Value = model.ProjectName;
			parameters[6].Value = model.QCLevel;
			parameters[7].Value = model.OperatorName;
			parameters[8].Value = model.AddDate;
			parameters[9].Value = model.ValidDate;
			parameters[10].Value = model.QCRules;
			parameters[11].Value = model.QCID;

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
		public bool Delete(int QCID)
		{

			StringBuilder strSql = new StringBuilder();
			strSql.Append("delete from tbQC ");
			strSql.Append(" where QCID=@QCID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@QCID", OleDbType.Integer,4)
			};
			parameters[0].Value = QCID;

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
		public bool DeleteList(string QCIDlist)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("delete from tbQC ");
			strSql.Append(" where QCID in (" + QCIDlist + ")  ");
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
		public BioBaseCLIA.Model.tbQC GetModel(int QCID)
		{

			StringBuilder strSql = new StringBuilder();
			strSql.Append("select QCID,QCNumber,Batch,XValue,SD,Status,ProjectName,QCLevel,OperatorName,AddDate,ValidDate,QCRules from tbQC ");
			strSql.Append(" where QCID=@QCID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@QCID", OleDbType.Integer,4)
			};
			parameters[0].Value = QCID;

			BioBaseCLIA.Model.tbQC model = new BioBaseCLIA.Model.tbQC();
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
		public BioBaseCLIA.Model.tbQC DataRowToModel(DataRow row)
		{
			BioBaseCLIA.Model.tbQC model = new BioBaseCLIA.Model.tbQC();
			if (row != null)
			{
				if (row["QCID"] != null && row["QCID"].ToString() != "")
				{
					model.QCID = int.Parse(row["QCID"].ToString());
				}
				if (row["QCNumber"] != null)
				{
					model.QCNumber = row["QCNumber"].ToString();
				}
				if (row["Batch"] != null)
				{
					model.Batch = row["Batch"].ToString();
				}
				//model.XValue=row["XValue"].ToString();
				//model.SD=row["SD"].ToString();
				if (row["Status"] != null)
				{
					model.Status = row["Status"].ToString();
				}
				if (row["ProjectName"] != null)
				{
					model.ProjectName = row["ProjectName"].ToString();
				}
				if (row["QCLevel"] != null)
				{
					model.QCLevel = row["QCLevel"].ToString();
				}
				if (row["OperatorName"] != null)
				{
					model.OperatorName = row["OperatorName"].ToString();
				}
				if (row["AddDate"] != null)
				{
					model.AddDate = row["AddDate"].ToString();
				}
				if (row["ValidDate"] != null)
				{
					model.ValidDate = row["ValidDate"].ToString();
				}
				if (row["QCRules"] != null)
				{
					model.QCRules = row["QCRules"].ToString();
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
			strSql.Append("select QCID,QCNumber,Batch,XValue,SD,Status,ProjectName,QCLevel,OperatorName,AddDate,ValidDate,QCRules ");
			strSql.Append(" FROM tbQC ");
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
			strSql.Append("select count(1) FROM tbQC ");
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
				strSql.Append("order by T.QCID desc");
			}
			strSql.Append(")AS Row, T.*  from tbQC T ");
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
			parameters[0].Value = "tbQC";
			parameters[1].Value = "QCID";
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

