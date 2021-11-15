/**  版本信息模板在安装目录下，可自行修改。
* tbProject.cs
*
* 功 能： N/A
* 类 名： tbProject
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
	/// 数据访问类:tbProject
	/// </summary>
	public partial class tbProject
	{
		private const int connType = 0;
		public tbProject()
		{
        
        }
		#region  BasicMethod

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
			return DbHelperOleDb.GetMaxID(connType, "ProjectID", "tbProject");
		}

		/// <summary>
		/// 是否存在该记录（ShrotName）
		/// </summary>
		public bool Exists_(string ShrotName)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("select count(1) from tbProject");
			strSql.Append(" where ShortName =@ShortName");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ShortName", OleDbType.VarChar,30)
			};
			parameters[0].Value = ShrotName;
			return DbHelperOleDb.Exists(connType, strSql.ToString(), parameters);
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int ProjectID)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("select count(1) from tbProject");
			strSql.Append(" where ProjectID=@ProjectID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ProjectID", OleDbType.Integer,4)
			};
			parameters[0].Value = ProjectID;

			return DbHelperOleDb.Exists(connType, strSql.ToString(), parameters);
		}

		/// <summary>
		/// 增加一条数据
		/// </summary>
		public bool Add(BioBaseCLIA.Model.tbProject model)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("insert into tbProject(");
			strSql.Append("ProjectNumber,ShortName,FullName,ProjectType,DiluteName,DiluteCount,RangeType,ValueRange1,ValueRange2,ValueUnit,MinValue,MaxValue," +
				"CalPointNumber,CalPointConc,QCPointNumber,QCPoints,ProjectProcedure,CalMode,CalMethod,CalculateMethod,LoadType,ActiveStatus,ExpiryDate,NoUsePro,VRangeType )");
			strSql.Append(" values (");
			strSql.Append("@ProjectNumber,@ShortName,@FullName,@ProjectType,@DiluteName,@DiluteCount,@RangeType,@ValueRange1,@ValueRange2,@ValueUnit,@MinValue,@MaxValue," +
				"@CalPointNumber,@CalPointConc,@QCPointNumber,@QCPoints,@ProjectProcedure,@CalMode,@CalMethod,@CalculateMethod,@LoadType,@ActiveStatus,@ExpiryDate,@NoUsePro,@VRangeType)");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ProjectNumber", OleDbType.VarChar,30),
					new OleDbParameter("@ShortName", OleDbType.VarChar,30),
					new OleDbParameter("@FullName", OleDbType.VarChar,255),
					new OleDbParameter("@ProjectType", OleDbType.VarChar,30),
					new OleDbParameter("@DiluteName", OleDbType.VarChar,30),
					new OleDbParameter("@DiluteCount", OleDbType.Integer,4),
					new OleDbParameter("@RangeType", OleDbType.VarChar,30),
					new OleDbParameter("@ValueRange1", OleDbType.VarChar,250),
					new OleDbParameter("@ValueRange2", OleDbType.VarChar,250),
					new OleDbParameter("@ValueUnit", OleDbType.VarChar,30),
					new OleDbParameter("@MinValue", OleDbType.Double),
					new OleDbParameter("@MaxValue", OleDbType.Double),
					new OleDbParameter("@CalPointNumber", OleDbType.Integer,4),
					new OleDbParameter("@CalPointConc", OleDbType.VarChar,50),
					new OleDbParameter("@QCPointNumber", OleDbType.Integer,4),
					new OleDbParameter("@QCPoints", OleDbType.VarChar,20),
					new OleDbParameter("@ProjectProcedure", OleDbType.VarChar,250),
					new OleDbParameter("@CalMode", OleDbType.Integer,4),
					new OleDbParameter("@CalMethod", OleDbType.Integer,4),
					new OleDbParameter("@CalculateMethod", OleDbType.VarChar,30),
					new OleDbParameter("@LoadType", OleDbType.Integer,4),
					new OleDbParameter("@ActiveStatus", OleDbType.Integer,4),
					new OleDbParameter("@ExpiryDate", OleDbType.Integer,4),//2018-08-02 zlx add
                    new OleDbParameter("@NoUsePro", OleDbType.VarChar,30),//2018-10-13 zlx add 
                    new OleDbParameter("@VRangeType", OleDbType.VarChar,30)};
			parameters[0].Value = model.ProjectNumber;
			parameters[1].Value = model.ShortName;
			parameters[2].Value = model.FullName;
			parameters[3].Value = model.ProjectType;
			parameters[4].Value = model.DiluteName;
			parameters[5].Value = model.DiluteCount;
			parameters[6].Value = model.RangeType;
			parameters[7].Value = model.ValueRange1;
			parameters[8].Value = model.ValueRange2;
			parameters[9].Value = model.ValueUnit;
			parameters[10].Value = model.MinValue;
			parameters[11].Value = model.MaxValue;
			parameters[12].Value = model.CalPointNumber;
			parameters[13].Value = model.CalPointConc;
			parameters[14].Value = model.QCPointNumber;
			parameters[15].Value = model.QCPoints;
			parameters[16].Value = model.ProjectProcedure;
			parameters[17].Value = model.CalMode;
			parameters[18].Value = model.CalMethod;
			parameters[19].Value = model.CalculateMethod;
			parameters[20].Value = model.LoadType;
			parameters[21].Value = model.ActiveStatus;
			parameters[22].Value = model.ExpiryDate;//2018-08-02 zlx add
			parameters[23].Value = model.NoUsePro;//2018-10-13 zlx mod
			parameters[24].Value = model.VRangeType;//2018-10-13 zlx add 
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
		public bool Update(BioBaseCLIA.Model.tbProject model)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("update tbProject set ");
			strSql.Append("ProjectNumber=@ProjectNumber,");
			strSql.Append("ShortName=@ShortName,");
			strSql.Append("FullName=@FullName,");
			strSql.Append("ProjectType=@ProjectType,");
			strSql.Append("DiluteName=@DiluteName,");
			strSql.Append("DiluteCount=@DiluteCount,");
			strSql.Append("RangeType=@RangeType,");
			strSql.Append("ValueRange1=@ValueRange1,");
			strSql.Append("ValueRange2=@ValueRange2,");
			strSql.Append("ValueUnit=@ValueUnit,");
			strSql.Append("MinValue=@MinValue,");
			strSql.Append("MaxValue=@MaxValue,");
			strSql.Append("CalPointNumber=@CalPointNumber,");
			strSql.Append("CalPointConc=@CalPointConc,");
			strSql.Append("QCPointNumber=@QCPointNumber,");
			strSql.Append("QCPoints=@QCPoints,");
			strSql.Append("ProjectProcedure=@ProjectProcedure,");
			strSql.Append("CalMode=@CalMode,");
			strSql.Append("CalMethod=@CalMethod,");
			strSql.Append("CalculateMethod=@CalculateMethod,");
			strSql.Append("LoadType=@LoadType,");
			strSql.Append("ActiveStatus=@ActiveStatus");
			strSql.Append(" where ProjectID=@ProjectID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ProjectNumber", OleDbType.VarChar,30),
					new OleDbParameter("@ShortName", OleDbType.VarChar,30),
					new OleDbParameter("@FullName", OleDbType.VarChar,255),
					new OleDbParameter("@ProjectType", OleDbType.VarChar,30),
					new OleDbParameter("@DiluteName", OleDbType.VarChar,30),//2018-07-30 
					new OleDbParameter("@DiluteCount", OleDbType.Integer,4),
					new OleDbParameter("@RangeType", OleDbType.VarChar,30),
					new OleDbParameter("@ValueRange1", OleDbType.VarChar,250),
					new OleDbParameter("@ValueRange2", OleDbType.VarChar,250),
					new OleDbParameter("@ValueUnit", OleDbType.VarChar,30),
					new OleDbParameter("@MinValue", OleDbType.Double),
					new OleDbParameter("@MaxValue", OleDbType.Double),
					new OleDbParameter("@CalPointNumber", OleDbType.Integer,4),
					new OleDbParameter("@CalPointConc", OleDbType.VarChar,50),
					new OleDbParameter("@QCPointNumber", OleDbType.Integer,4),
					new OleDbParameter("@QCPoints", OleDbType.VarChar,20),
					new OleDbParameter("@ProjectProcedure", OleDbType.VarChar,250),
					new OleDbParameter("@CalMode", OleDbType.Integer,4),
					new OleDbParameter("@CalMethod", OleDbType.Integer,4),
					new OleDbParameter("@CalculateMethod", OleDbType.VarChar,30),
					new OleDbParameter("@LoadType", OleDbType.Integer,4),
					new OleDbParameter("@ActiveStatus", OleDbType.Integer,4),
					new OleDbParameter("@ProjectID", OleDbType.Integer,4)};
			parameters[0].Value = model.ProjectNumber;
			parameters[1].Value = model.ShortName;
			parameters[2].Value = model.FullName;
			parameters[3].Value = model.ProjectType;
			parameters[4].Value = model.DiluteName;
			parameters[5].Value = model.DiluteCount;
			parameters[6].Value = model.RangeType;
			parameters[7].Value = model.ValueRange1;
			parameters[8].Value = model.ValueRange2;
			parameters[9].Value = model.ValueUnit;
			parameters[10].Value = model.MinValue;
			parameters[11].Value = model.MaxValue;
			parameters[12].Value = model.CalPointNumber;
			parameters[13].Value = model.CalPointConc;
			parameters[14].Value = model.QCPointNumber;
			parameters[15].Value = model.QCPoints;
			parameters[16].Value = model.ProjectProcedure;
			parameters[17].Value = model.CalMode;
			parameters[18].Value = model.CalMethod;
			parameters[19].Value = model.CalculateMethod;
			parameters[20].Value = model.LoadType;
			parameters[21].Value = model.ActiveStatus;
			parameters[22].Value = model.ProjectID;

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
		public bool Delete(int ProjectID)
		{

			StringBuilder strSql = new StringBuilder();
			strSql.Append("delete from tbProject ");
			strSql.Append(" where ProjectID=@ProjectID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ProjectID", OleDbType.Integer,4)
			};
			parameters[0].Value = ProjectID;

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
		public bool Delete_(string ShortName)
		{

			StringBuilder strSql = new StringBuilder();
			strSql.Append("delete from tbProject ");
			strSql.Append(" where ShortName=@ShortName");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ShortName", OleDbType.VarChar,30)
			};
			parameters[0].Value = ShortName;

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
		public bool DeleteList(string ProjectIDlist)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("delete from tbProject ");
			strSql.Append(" where ProjectID in (" + ProjectIDlist + ")  ");
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
		public BioBaseCLIA.Model.tbProject GetModel(int ProjectID)
		{

			StringBuilder strSql = new StringBuilder();
			strSql.Append("select ProjectID,ProjectNumber,ShortName,FullName,ProjectType,DiluteName,DiluteCount,RangeType,ValueRange1,ValueRange2,ValueUnit,MinValue,MaxValue,CalPointNumber,CalPointConc,QCPointNumber,QCPoints,ProjectProcedure,CalMode,CalMethod,CalculateMethod,LoadType,ActiveStatus from tbProject ");
			strSql.Append(" where ProjectID=@ProjectID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@ProjectID", OleDbType.Integer,4)
			};
			parameters[0].Value = ProjectID;

			BioBaseCLIA.Model.tbProject model = new BioBaseCLIA.Model.tbProject();
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
		public BioBaseCLIA.Model.tbProject DataRowToModel(DataRow row)
		{
			BioBaseCLIA.Model.tbProject model = new BioBaseCLIA.Model.tbProject();
			if (row != null)
			{
				if (row["ProjectID"] != null && row["ProjectID"].ToString() != "")
				{
					model.ProjectID = int.Parse(row["ProjectID"].ToString());
				}
				if (row["ProjectNumber"] != null)
				{
					model.ProjectNumber = row["ProjectNumber"].ToString();
				}
				if (row["ShortName"] != null)
				{
					model.ShortName = row["ShortName"].ToString();
				}
				if (row["FullName"] != null)
				{
					model.FullName = row["FullName"].ToString();
				}
				if (row["ProjectType"] != null)
				{
					model.ProjectType = row["ProjectType"].ToString();
				}
				if (row["DiluteName"] != null)
				{
					model.DiluteName = row["DiluteName"].ToString();
				}
				if (row["DiluteCount"] != null && row["DiluteCount"].ToString() != "")
				{
					model.DiluteCount = int.Parse(row["DiluteCount"].ToString());
				}
				if (row["RangeType"] != null)
				{
					model.RangeType = row["RangeType"].ToString();
				}
				if (row["ValueRange1"] != null)
				{
					model.ValueRange1 = row["ValueRange1"].ToString();
				}
				if (row["ValueRange2"] != null)
				{
					model.ValueRange2 = row["ValueRange2"].ToString();
				}
				if (row["ValueUnit"] != null)
				{
					model.ValueUnit = row["ValueUnit"].ToString();
				}
				//model.MinValue=row["MinValue"].ToString();
				//model.MaxValue=row["MaxValue"].ToString();
				if (row["CalPointNumber"] != null && row["CalPointNumber"].ToString() != "")
				{
					model.CalPointNumber = int.Parse(row["CalPointNumber"].ToString());
				}
				if (row["CalPointConc"] != null)
				{
					model.CalPointConc = row["CalPointConc"].ToString();
				}
				if (row["QCPointNumber"] != null && row["QCPointNumber"].ToString() != "")
				{
					model.QCPointNumber = int.Parse(row["QCPointNumber"].ToString());
				}
				if (row["QCPoints"] != null)
				{
					model.QCPoints = row["QCPoints"].ToString();
				}
				if (row["ProjectProcedure"] != null)
				{
					model.ProjectProcedure = row["ProjectProcedure"].ToString();
				}
				if (row["CalMode"] != null && row["CalMode"].ToString() != "")
				{
					model.CalMode = int.Parse(row["CalMode"].ToString());
				}
				if (row["CalMethod"] != null && row["CalMethod"].ToString() != "")
				{
					model.CalMethod = int.Parse(row["CalMethod"].ToString());
				}
				if (row["CalculateMethod"] != null)
				{
					model.CalculateMethod = row["CalculateMethod"].ToString();
				}
				if (row["LoadType"] != null && row["LoadType"].ToString() != "")
				{
					model.LoadType = int.Parse(row["LoadType"].ToString());
				}
				if (row["ActiveStatus"] != null && row["ActiveStatus"].ToString() != "")
				{
					model.ActiveStatus = int.Parse(row["ActiveStatus"].ToString());
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
			strSql.Append("select ProjectID,ProjectNumber,ShortName,FullName,ProjectType,DiluteName,DiluteCount,RangeType,ValueRange1,ValueRange2," +
				"ValueUnit,MinValue,MaxValue,CalPointNumber,CalPointConc,QCPointNumber,QCPoints,ProjectProcedure,CalMode,CalMethod,CalculateMethod," +
				"LoadType,ActiveStatus,ExpiryDate,NoUsePro,VRangeType ");//2018-10-13 zlx mod
			strSql.Append(" FROM tbProject ");
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
			strSql.Append("select count(1) FROM tbProject ");
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
				strSql.Append("order by T.ProjectID desc");
			}
			strSql.Append(")AS Row, T.*  from tbProject T ");
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
			parameters[0].Value = "tbProject";
			parameters[1].Value = "ProjectID";
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

