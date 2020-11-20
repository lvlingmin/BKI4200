/**  版本信息模板在安装目录下，可自行修改。
* tbMainScalCurve.cs
*
* 功 能： N/A
* 类 名： tbMainScalCurve
*
* Ver    变更日期             负责人  变更内容
* ───────────────────────────────────
* V0.01  2018-03-10 9:28:47   N/A    初版
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
	/// 数据访问类:tbMainScalCurve
	/// </summary>
	public partial class tbMainScalCurve
	{
		private const int connType = 1;
		public tbMainScalCurve()
		{}
		#region  BasicMethod

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
		return DbHelperOleDb.GetMaxID(connType, "MainCurveID", "tbMainScalCurve"); 
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int MainCurveID)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from tbMainScalCurve");
			strSql.Append(" where MainCurveID=@MainCurveID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@MainCurveID", OleDbType.Integer,4)
			};
			parameters[0].Value = MainCurveID;

			return DbHelperOleDb.Exists(connType, strSql.ToString(),parameters);
		}


        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool ExistsCurve(string ItemName, string RegentBatch)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from tbMainScalCurve");
            strSql.Append(" where ItemName=@ItemName and RegentBatch=@RegentBatch");
            OleDbParameter[] parameters = {
					new OleDbParameter("@ItemName", OleDbType.VarChar,30),
					new OleDbParameter("@RegentBatch", OleDbType.VarChar,20)
			};
            parameters[0].Value = ItemName;
            parameters[1].Value = RegentBatch;

            return DbHelperOleDb.Exists(connType, strSql.ToString(), parameters);
        }

		/// <summary>
		/// 增加一条数据
		/// </summary>
        public bool Add(BioBaseCLIA.Model.tbMainScalCurve model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into tbMainScalCurve(");
			strSql.Append("ItemName,RegentBatch,Points,ActiveDate,ValidPeriod)");
			strSql.Append(" values (");
			strSql.Append("@ItemName,@RegentBatch,@Points,@ActiveDate,@ValidPeriod)");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ItemName", OleDbType.VarChar,30),
					new OleDbParameter("@RegentBatch", OleDbType.VarChar,20),
					new OleDbParameter("@Points", OleDbType.VarChar,255),
					new OleDbParameter("@ActiveDate", OleDbType.Date),
					new OleDbParameter("@ValidPeriod", OleDbType.Date)};
			parameters[0].Value = model.ItemName;
			parameters[1].Value = model.RegentBatch;
			parameters[2].Value = model.Points;
			parameters[3].Value = model.ActiveDate;
			parameters[4].Value = model.ValidPeriod;

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
        /// 根据批号获得id lyq add 20190815
        /// </summary>
        public int SelectIdAsRegentBatch(string RegentBatch)
        {
			string strSql = "select MainCurveID from tbMainScalCurve where RegentBatch = '" + RegentBatch + "'";
			object temp = DbHelperOleDb.GetSingle(connType, strSql);
			int rBatch = temp == null ? 0 : (int)temp;
			if (rBatch > 0)
			{
				return rBatch;
			}
			else
			{
				return 0;
			}
		}


        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(BioBaseCLIA.Model.tbMainScalCurve model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update tbMainScalCurve set ");
			strSql.Append("ItemName=@ItemName,");
			strSql.Append("RegentBatch=@RegentBatch,");
			strSql.Append("Points=@Points,");
			strSql.Append("ActiveDate=@ActiveDate,");
			strSql.Append("ValidPeriod=@ValidPeriod");
			strSql.Append(" where MainCurveID=@MainCurveID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ItemName", OleDbType.VarChar,30),
					new OleDbParameter("@RegentBatch", OleDbType.VarChar,20),
					new OleDbParameter("@Points", OleDbType.VarChar,255),
					new OleDbParameter("@ActiveDate", OleDbType.Date),
					new OleDbParameter("@ValidPeriod", OleDbType.Date),
					new OleDbParameter("@MainCurveID", OleDbType.Integer,4)};
			parameters[0].Value = model.ItemName;
			parameters[1].Value = model.RegentBatch;
			parameters[2].Value = model.Points;
			parameters[3].Value = model.ActiveDate;
			parameters[4].Value = model.ValidPeriod;
			parameters[5].Value = model.MainCurveID;

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
		public bool Delete(int MainCurveID)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from tbMainScalCurve ");
			strSql.Append(" where MainCurveID=@MainCurveID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@MainCurveID", OleDbType.Integer,4)
			};
			parameters[0].Value = MainCurveID;

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
		public bool DeleteList(string MainCurveIDlist )
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from tbMainScalCurve ");
			strSql.Append(" where MainCurveID in ("+MainCurveIDlist + ")  ");
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
        public BioBaseCLIA.Model.tbMainScalCurve GetModel(int MainCurveID)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select MainCurveID,ItemName,RegentBatch,Points,ActiveDate,ValidPeriod from tbMainScalCurve ");
			strSql.Append(" where MainCurveID=@MainCurveID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@MainCurveID", OleDbType.Integer,4)
			};
			parameters[0].Value = MainCurveID;

            BioBaseCLIA.Model.tbMainScalCurve model = new BioBaseCLIA.Model.tbMainScalCurve();
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
        public BioBaseCLIA.Model.tbMainScalCurve DataRowToModel(DataRow row)
		{
            BioBaseCLIA.Model.tbMainScalCurve model = new BioBaseCLIA.Model.tbMainScalCurve();
			if (row != null)
			{
				if(row["MainCurveID"]!=null && row["MainCurveID"].ToString()!="")
				{
					model.MainCurveID=int.Parse(row["MainCurveID"].ToString());
				}
				if(row["ItemName"]!=null)
				{
					model.ItemName=row["ItemName"].ToString();
				}
				if(row["RegentBatch"]!=null)
				{
					model.RegentBatch=row["RegentBatch"].ToString();
				}
				if(row["Points"]!=null)
				{
					model.Points=row["Points"].ToString();
				}
				if(row["ActiveDate"]!=null && row["ActiveDate"].ToString()!="")
				{
					model.ActiveDate=DateTime.Parse(row["ActiveDate"].ToString());
				}
				if(row["ValidPeriod"]!=null)
				{
					model.ValidPeriod=DateTime.Parse(row["ValidPeriod"].ToString());
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
			strSql.Append("select MainCurveID,ItemName,RegentBatch,Points,ActiveDate,ValidPeriod ");
			strSql.Append(" FROM tbMainScalCurve ");
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
			strSql.Append("select count(1) FROM tbMainScalCurve ");
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
				strSql.Append("order by T.MainCurveID desc");
			}
			strSql.Append(")AS Row, T.*  from tbMainScalCurve T ");
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
			parameters[0].Value = "tbMainScalCurve";
			parameters[1].Value = "MainCurveID";
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

