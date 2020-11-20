/**  版本信息模板在安装目录下，可自行修改。
* tbScalingResult.cs
*
* 功 能： N/A
* 类 名： tbScalingResult
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2018-01-10 18:15:04   N/A    初版
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
	/// 数据访问类:tbScalingResult
	/// </summary>
	public partial class tbScalingResult
	{
		private const int connType = 1;
		public tbScalingResult()
		{}
		#region  BasicMethod

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
		return DbHelperOleDb.GetMaxID(connType, "ScalingResultID", "tbScalingResult"); 
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int ScalingResultID)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from tbScalingResult");
			strSql.Append(" where ScalingResultID=@ScalingResultID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ScalingResultID", OleDbType.Integer,4)
			};
			parameters[0].Value = ScalingResultID;

			return DbHelperOleDb.Exists(connType, strSql.ToString(),parameters);
		}

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool ExistsCurve(string ItemName, string RegentBatch)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from tbScalingResult");
            strSql.Append(" where ItemName=@ItemName and RegentBatch=@RegentBatch");
            OleDbParameter[] parameters = {
					new OleDbParameter("@ItemName", OleDbType.VarChar,30),
					new OleDbParameter("@RegentBatch", OleDbType.VarChar,50)
			};
            parameters[0].Value = ItemName;
            parameters[1].Value = RegentBatch;

            return DbHelperOleDb.Exists(connType, strSql.ToString(), parameters);
        }
		/// <summary>
		/// 增加一条数据
		/// </summary>
		public bool Add(BioBaseCLIA.Model.tbScalingResult model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into tbScalingResult(");
			strSql.Append("ItemName,RegentBatch,ScalingModel,ActiveDate,PointCount,Points,Status,Source)");
			strSql.Append(" values (");
			strSql.Append("@ItemName,@RegentBatch,@ScalingModel,@ActiveDate,@PointCount,@Points,@Status,@Source)");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ItemName", OleDbType.VarChar,30),
					new OleDbParameter("@RegentBatch", OleDbType.VarChar,50),
					new OleDbParameter("@ScalingModel", OleDbType.Integer,4),
					new OleDbParameter("@ActiveDate", OleDbType.Date),
					new OleDbParameter("@PointCount", OleDbType.Integer,4),
					new OleDbParameter("@Points", OleDbType.VarChar,200),
					new OleDbParameter("@Status", OleDbType.Integer,4),
					new OleDbParameter("@Source", OleDbType.Integer,4)};
			parameters[0].Value = model.ItemName;
			parameters[1].Value = model.RegentBatch;
			parameters[2].Value = model.ScalingModel;
			parameters[3].Value = model.ActiveDate;
			parameters[4].Value = model.PointCount;
			parameters[5].Value = model.Points;
			parameters[6].Value = model.Status;
			parameters[7].Value = model.Source;

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
		public bool Update(BioBaseCLIA.Model.tbScalingResult model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update tbScalingResult set ");
			strSql.Append("ItemName=@ItemName,");
			strSql.Append("RegentBatch=@RegentBatch,");
			strSql.Append("ScalingModel=@ScalingModel,");
			strSql.Append("ActiveDate=@ActiveDate,");
			strSql.Append("PointCount=@PointCount,");
			strSql.Append("Points=@Points,");
			strSql.Append("Status=@Status,");
			strSql.Append("Source=@Source");
			strSql.Append(" where ScalingResultID=@ScalingResultID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ItemName", OleDbType.VarChar,30),
					new OleDbParameter("@RegentBatch", OleDbType.VarChar,50),
					new OleDbParameter("@ScalingModel", OleDbType.Integer,4),
					new OleDbParameter("@ActiveDate", OleDbType.Date),
					new OleDbParameter("@PointCount", OleDbType.Integer,4),
					new OleDbParameter("@Points", OleDbType.VarChar,200),
					new OleDbParameter("@Status", OleDbType.Integer,4),
					new OleDbParameter("@Source", OleDbType.Integer,4),
					new OleDbParameter("@ScalingResultID", OleDbType.Integer,4)};
			parameters[0].Value = model.ItemName;
			parameters[1].Value = model.RegentBatch;
			parameters[2].Value = model.ScalingModel;
			parameters[3].Value = model.ActiveDate;
			parameters[4].Value = model.PointCount;
			parameters[5].Value = model.Points;
			parameters[6].Value = model.Status;
			parameters[7].Value = model.Source;
			parameters[8].Value = model.ScalingResultID;

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
		public bool Delete(int ScalingResultID)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from tbScalingResult ");
			strSql.Append(" where ScalingResultID=@ScalingResultID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ScalingResultID", OleDbType.Integer,4)
			};
			parameters[0].Value = ScalingResultID;

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
		public bool DeleteList(string ScalingResultIDlist )
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from tbScalingResult ");
			strSql.Append(" where ScalingResultID in ("+ScalingResultIDlist + ")  ");
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
		public BioBaseCLIA.Model.tbScalingResult GetModel(int ScalingResultID)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select ScalingResultID,ItemName,RegentBatch,ScalingModel,ActiveDate,PointCount,Points,Status,Source from tbScalingResult ");
			strSql.Append(" where ScalingResultID=@ScalingResultID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ScalingResultID", OleDbType.Integer,4)
			};
			parameters[0].Value = ScalingResultID;

			BioBaseCLIA.Model.tbScalingResult model=new BioBaseCLIA.Model.tbScalingResult();
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
		public BioBaseCLIA.Model.tbScalingResult DataRowToModel(DataRow row)
		{
			BioBaseCLIA.Model.tbScalingResult model=new BioBaseCLIA.Model.tbScalingResult();
			if (row != null)
			{
				if(row["ScalingResultID"]!=null && row["ScalingResultID"].ToString()!="")
				{
					model.ScalingResultID=int.Parse(row["ScalingResultID"].ToString());
				}
				if(row["ItemName"]!=null)
				{
					model.ItemName=row["ItemName"].ToString();
				}
				if(row["RegentBatch"]!=null)
				{
					model.RegentBatch=row["RegentBatch"].ToString();
				}
				if(row["ScalingModel"]!=null && row["ScalingModel"].ToString()!="")
				{
					model.ScalingModel=int.Parse(row["ScalingModel"].ToString());
				}
				if(row["ActiveDate"]!=null && row["ActiveDate"].ToString()!="")
				{
					model.ActiveDate=DateTime.Parse(row["ActiveDate"].ToString());
				}
				if(row["PointCount"]!=null && row["PointCount"].ToString()!="")
				{
					model.PointCount=int.Parse(row["PointCount"].ToString());
				}
				if(row["Points"]!=null)
				{
					model.Points=row["Points"].ToString();
				}
				if(row["Status"]!=null && row["Status"].ToString()!="")
				{
					model.Status=int.Parse(row["Status"].ToString());
				}
				if(row["Source"]!=null && row["Source"].ToString()!="")
				{
					model.Source=int.Parse(row["Source"].ToString());
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
			strSql.Append("select ScalingResultID,ItemName,RegentBatch,ScalingModel,ActiveDate,PointCount,Points,Status,Source ");
			strSql.Append(" FROM tbScalingResult ");
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
			strSql.Append("select count(1) FROM tbScalingResult ");
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
				strSql.Append("order by T.ScalingResultID desc");
			}
			strSql.Append(")AS Row, T.*  from tbScalingResult T ");
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
			parameters[0].Value = "tbScalingResult";
			parameters[1].Value = "ScalingResultID";
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

