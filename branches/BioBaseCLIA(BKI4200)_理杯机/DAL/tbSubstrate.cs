/**  版本信息模板在安装目录下，可自行修改。
* tbSubstrate.cs
*
* 功 能： N/A
* 类 名： tbSubstrate
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
	/// 数据访问类:tbSubstrate
	/// </summary>
	public partial class tbSubstrate
	{
		private const int connType = 3;
		public tbSubstrate()
		{}
		#region  BasicMethod

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
			return DbHelperOleDb.GetMaxID(connType, "SubstrateID", "tbSubstrate");
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int SubstrateID)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("select count(1) from tbSubstrate");
			strSql.Append(" where SubstrateID=@SubstrateID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@SubstrateID", OleDbType.Integer,4)
			};
			parameters[0].Value = SubstrateID;

			return DbHelperOleDb.Exists(connType, strSql.ToString(), parameters);
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public bool Add(BioBaseCLIA.Model.tbSubstrate model)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("insert into tbSubstrate(");
			strSql.Append("SubstrateNumber,Batch,BarCode,Status,AllTestNumber,leftoverTest,ExtraTest,ValidDate,AddDate,Postion)");
			strSql.Append(" values (");
			strSql.Append("@SubstrateNumber,@Batch,@BarCode,@Status,@AllTestNumber,@leftoverTest,@ExtraTest,@ValidDate,@AddDate,@Postion)");
			OleDbParameter[] parameters = {
					new OleDbParameter("@SubstrateNumber", OleDbType.VarChar,30),
					new OleDbParameter("@Batch", OleDbType.VarChar,30),
					new OleDbParameter("@BarCode", OleDbType.VarChar,30),
					new OleDbParameter("@Status", OleDbType.VarChar,30),
					new OleDbParameter("@AllTestNumber", OleDbType.Integer,4),
					new OleDbParameter("@leftoverTest", OleDbType.Integer,4),
					new OleDbParameter("@ExtraTest", OleDbType.Integer,4),
					new OleDbParameter("@ValidDate", OleDbType.VarChar,30),
					new OleDbParameter("@AddDate", OleDbType.VarChar,30),
					new OleDbParameter("@Postion", OleDbType.VarChar,30)};
			parameters[0].Value = model.SubstrateNumber;
			parameters[1].Value = model.Batch;
			parameters[2].Value = model.BarCode;
			parameters[3].Value = model.Status;
			parameters[4].Value = model.AllTestNumber;
			parameters[5].Value = model.leftoverTest;
			parameters[6].Value = model.ExtraTest;
			parameters[7].Value = model.ValidDate;
			parameters[8].Value = model.AddDate;
			parameters[9].Value = model.Postion;

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
		public bool Update(BioBaseCLIA.Model.tbSubstrate model)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("update tbSubstrate set ");
			strSql.Append("SubstrateNumber=@SubstrateNumber,");
			strSql.Append("Batch=@Batch,");
			strSql.Append("BarCode=@BarCode,");
			strSql.Append("Status=@Status,");
			strSql.Append("AllTestNumber=@AllTestNumber,");
			strSql.Append("leftoverTest=@leftoverTest,");
			strSql.Append("ExtraTest=@ExtraTest,");
			strSql.Append("ValidDate=@ValidDate,");
			strSql.Append("AddDate=@AddDate,");
			strSql.Append("Postion=@Postion");
			strSql.Append(" where SubstrateID=@SubstrateID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@SubstrateNumber", OleDbType.VarChar,30),
					new OleDbParameter("@Batch", OleDbType.VarChar,30),
					new OleDbParameter("@BarCode", OleDbType.VarChar,30),
					new OleDbParameter("@Status", OleDbType.VarChar,30),
					new OleDbParameter("@AllTestNumber", OleDbType.Integer,4),
					new OleDbParameter("@leftoverTest", OleDbType.Integer,4),
					new OleDbParameter("@ExtraTest", OleDbType.Integer,4),
					new OleDbParameter("@ValidDate", OleDbType.VarChar,30),
					new OleDbParameter("@AddDate", OleDbType.VarChar,30),
					new OleDbParameter("@Postion", OleDbType.VarChar,30),
					new OleDbParameter("@SubstrateID", OleDbType.Integer,4)};
			parameters[0].Value = model.SubstrateNumber;
			parameters[1].Value = model.Batch;
			parameters[2].Value = model.BarCode;
			parameters[3].Value = model.Status;
			parameters[4].Value = model.AllTestNumber;
			parameters[5].Value = model.leftoverTest;
			parameters[6].Value = model.ExtraTest;
			parameters[7].Value = model.ValidDate;
			parameters[8].Value = model.AddDate;
			parameters[9].Value = model.Postion;
			parameters[10].Value = model.SubstrateID;

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
		public bool Delete(int SubstrateID)
		{

			StringBuilder strSql = new StringBuilder();
			strSql.Append("delete from tbSubstrate ");
			strSql.Append(" where SubstrateID=@SubstrateID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@SubstrateID", OleDbType.Integer,4)
			};
			parameters[0].Value = SubstrateID;

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
		public bool DeleteList(string SubstrateIDlist)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("delete from tbSubstrate ");
			strSql.Append(" where SubstrateID in (" + SubstrateIDlist + ")  ");
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
		public BioBaseCLIA.Model.tbSubstrate GetModel(int SubstrateID)
		{

			StringBuilder strSql = new StringBuilder();
			strSql.Append("select SubstrateID,SubstrateNumber,Batch,BarCode,Status,AllTestNumber,leftoverTest,ExtraTest,ValidDate,AddDate,Postion from tbSubstrate ");
			strSql.Append(" where SubstrateID=@SubstrateID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@SubstrateID", OleDbType.Integer,4)
			};
			parameters[0].Value = SubstrateID;

			BioBaseCLIA.Model.tbSubstrate model = new BioBaseCLIA.Model.tbSubstrate();
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
		public BioBaseCLIA.Model.tbSubstrate DataRowToModel(DataRow row)
		{
			BioBaseCLIA.Model.tbSubstrate model = new BioBaseCLIA.Model.tbSubstrate();
			if (row != null)
			{
				if (row["SubstrateID"] != null && row["SubstrateID"].ToString() != "")
				{
					model.SubstrateID = int.Parse(row["SubstrateID"].ToString());
				}
				if (row["SubstrateNumber"] != null)
				{
					model.SubstrateNumber = row["SubstrateNumber"].ToString();
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
				if (row["leftoverTest"] != null && row["leftoverTest"].ToString() != "")
				{
					model.leftoverTest = int.Parse(row["leftoverTest"].ToString());
				}
				if (row["ExtraTest"] != null && row["ExtraTest"].ToString() != "")
				{
					model.ExtraTest = int.Parse(row["ExtraTest"].ToString());
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
			strSql.Append("select SubstrateID,SubstrateNumber,Batch,BarCode,Status,AllTestNumber,leftoverTest,ExtraTest,ValidDate,AddDate,Postion ");
			strSql.Append(" FROM tbSubstrate ");
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
			strSql.Append("select count(1) FROM tbSubstrate ");
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
				strSql.Append("order by T.SubstrateID desc");
			}
			strSql.Append(")AS Row, T.*  from tbSubstrate T ");
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
			parameters[0].Value = "tbSubstrate";
			parameters[1].Value = "SubstrateID";
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

