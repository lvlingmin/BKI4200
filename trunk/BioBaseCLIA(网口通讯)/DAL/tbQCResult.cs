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
using System.Text;
using System.Data.OleDb;
using Maticsoft.DBUtility;//Please add references
namespace BioBaseCLIA.DAL
{
	/// <summary>
	/// 数据访问类:tbQCResult
	/// </summary>
	public partial class tbQCResult
	{
		private const int connType = 1;
		public tbQCResult()
		{}
		#region  BasicMethod

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
		return DbHelperOleDb.GetMaxID(connType, "QCResultID", "tbQCResult"); 
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int QCResultID)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from tbQCResult");
			strSql.Append(" where QCResultID=@QCResultID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@QCResultID", OleDbType.Integer,4)
			};
			parameters[0].Value = QCResultID;

			return DbHelperOleDb.Exists(connType, strSql.ToString(),parameters);
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public bool Add(BioBaseCLIA.Model.tbQCResult model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into tbQCResult(");
			strSql.Append("QCID,ItemName,ConcLevel,Source,PMTCounter,Batch,Concentration,ConcSPEC,Unit,TestDate)");
			strSql.Append(" values (");
			strSql.Append("@QCID,@ItemName,@ConcLevel,@Source,@PMTCounter,@Batch,@Concentration,@ConcSPEC,@Unit,@TestDate)");
			OleDbParameter[] parameters = {
					new OleDbParameter("@QCID", OleDbType.Integer,4),
					new OleDbParameter("@ItemName", OleDbType.VarChar,30),
					new OleDbParameter("@ConcLevel", OleDbType.Integer,4),
					new OleDbParameter("@Source", OleDbType.Integer,4),
					new OleDbParameter("@PMTCounter", OleDbType.Integer,4),
					new OleDbParameter("@Batch", OleDbType.VarChar,50),
					new OleDbParameter("@Concentration", OleDbType.Double),
					new OleDbParameter("@ConcSPEC", OleDbType.VarChar,64),
					new OleDbParameter("@Unit", OleDbType.VarChar,20),
					new OleDbParameter("@TestDate", OleDbType.Date)};
			parameters[0].Value = model.QCID;
			parameters[1].Value = model.ItemName;
			parameters[2].Value = model.ConcLevel;
			parameters[3].Value = model.Source;
			parameters[4].Value = model.PMTCounter;
			parameters[5].Value = model.Batch;
			parameters[6].Value = model.Concentration;
			parameters[7].Value = model.ConcSPEC;
			parameters[8].Value = model.Unit;
			parameters[9].Value = model.TestDate;

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
		public bool Update(BioBaseCLIA.Model.tbQCResult model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update tbQCResult set ");
			strSql.Append("QCID=@QCID,");
			strSql.Append("ItemName=@ItemName,");
			strSql.Append("ConcLevel=@ConcLevel,");
			strSql.Append("Source=@Source,");
			strSql.Append("PMTCounter=@PMTCounter,");
			strSql.Append("Batch=@Batch,");
			strSql.Append("Concentration=@Concentration,");
			strSql.Append("ConcSPEC=@ConcSPEC,");
			strSql.Append("Unit=@Unit,");
			strSql.Append("TestDate=@TestDate");
			strSql.Append(" where QCResultID=@QCResultID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@QCID", OleDbType.Integer,4),
					new OleDbParameter("@ItemName", OleDbType.VarChar,30),
					new OleDbParameter("@ConcLevel", OleDbType.Integer,4),
					new OleDbParameter("@Source", OleDbType.Integer,4),
					new OleDbParameter("@PMTCounter", OleDbType.Integer,4),
					new OleDbParameter("@Batch", OleDbType.VarChar,50),
					new OleDbParameter("@Concentration", OleDbType.Double),
					new OleDbParameter("@ConcSPEC", OleDbType.VarChar,64),
					new OleDbParameter("@Unit", OleDbType.VarChar,20),
					new OleDbParameter("@TestDate", OleDbType.Date),
					new OleDbParameter("@QCResultID", OleDbType.Integer,4)};
			parameters[0].Value = model.QCID;
			parameters[1].Value = model.ItemName;
			parameters[2].Value = model.ConcLevel;
			parameters[3].Value = model.Source;
			parameters[4].Value = model.PMTCounter;
			parameters[5].Value = model.Batch;
			parameters[6].Value = model.Concentration;
			parameters[7].Value = model.ConcSPEC;
			parameters[8].Value = model.Unit;
			parameters[9].Value = model.TestDate;
			parameters[10].Value = model.QCResultID;

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
		public bool Delete(int QCResultID)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from tbQCResult ");
			strSql.Append(" where QCResultID=@QCResultID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@QCResultID", OleDbType.Integer,4)
			};
			parameters[0].Value = QCResultID;

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
		public bool DeleteList(string QCResultIDlist )
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from tbQCResult ");
			strSql.Append(" where QCResultID in ("+QCResultIDlist + ")  ");
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
		public BioBaseCLIA.Model.tbQCResult GetModel(int QCResultID)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select QCResultID,QCID,ItemName,ConcLevel,Source,PMTCounter,Batch,Concentration,ConcSPEC,Unit,TestDate from tbQCResult ");
			strSql.Append(" where QCResultID=@QCResultID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@QCResultID", OleDbType.Integer,4)
			};
			parameters[0].Value = QCResultID;

			BioBaseCLIA.Model.tbQCResult model=new BioBaseCLIA.Model.tbQCResult();
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
		public BioBaseCLIA.Model.tbQCResult DataRowToModel(DataRow row)
		{
			BioBaseCLIA.Model.tbQCResult model=new BioBaseCLIA.Model.tbQCResult();
			if (row != null)
			{
				if(row["QCResultID"]!=null && row["QCResultID"].ToString()!="")
				{
					model.QCResultID=int.Parse(row["QCResultID"].ToString());
				}
				if(row["QCID"]!=null && row["QCID"].ToString()!="")
				{
					model.QCID=int.Parse(row["QCID"].ToString());
				}
				if(row["ItemName"]!=null)
				{
					model.ItemName=row["ItemName"].ToString();
				}
				if(row["ConcLevel"]!=null && row["ConcLevel"].ToString()!="")
				{
					model.ConcLevel=int.Parse(row["ConcLevel"].ToString());
				}
				if(row["Source"]!=null && row["Source"].ToString()!="")
				{
					model.Source=int.Parse(row["Source"].ToString());
				}
				if(row["PMTCounter"]!=null && row["PMTCounter"].ToString()!="")
				{
					model.PMTCounter=int.Parse(row["PMTCounter"].ToString());
				}
				if(row["Batch"]!=null)
				{
					model.Batch=row["Batch"].ToString();
				}
					//model.Concentration=row["Concentration"].ToString();
				if(row["ConcSPEC"]!=null)
				{
					model.ConcSPEC=row["ConcSPEC"].ToString();
				}
				if(row["Unit"]!=null)
				{
					model.Unit=row["Unit"].ToString();
				}
				if(row["TestDate"]!=null && row["TestDate"].ToString()!="")
				{
					model.TestDate=DateTime.Parse(row["TestDate"].ToString());
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
			strSql.Append("select QCResultID,QCID,ItemName,ConcLevel,Source,PMTCounter,Batch,Concentration,ConcSPEC,Unit,TestDate ");
			strSql.Append(" FROM tbQCResult ");
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
			strSql.Append("select count(1) FROM tbQCResult ");
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
				strSql.Append("order by T.QCResultID desc");
			}
			strSql.Append(")AS Row, T.*  from tbQCResult T ");
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
			parameters[0].Value = "tbQCResult";
			parameters[1].Value = "QCResultID";
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

