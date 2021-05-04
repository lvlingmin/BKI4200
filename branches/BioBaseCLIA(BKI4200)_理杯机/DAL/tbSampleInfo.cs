/**  版本信息模板在安装目录下，可自行修改。
* tbSampleInfo.cs
*
* 功 能： N/A
* 类 名： tbSampleInfo
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
	/// 数据访问类:tbSampleInfo
	/// </summary>
	public partial class tbSampleInfo
	{
		private const int connType = 1;
		public tbSampleInfo()
		{}
		#region  BasicMethod

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
			return DbHelperOleDb.GetMaxID(connType, "SampleID", "tbSampleInfo");
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int SampleID)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("select count(1) from tbSampleInfo");
			strSql.Append(" where SampleID=@SampleID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@SampleID", OleDbType.Integer,4)
			};
			parameters[0].Value = SampleID;

			return DbHelperOleDb.Exists(connType, strSql.ToString(), parameters);
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public bool Add(BioBaseCLIA.Model.tbSampleInfo model)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("insert into tbSampleInfo(");
			strSql.Append("SampleNo,PatientName,Sex,Age,SampleType,Source,[Position],SampleContainer,RepeatCount,RegentBatch,ProjectName,Emergency,ClinicNo,InpatientArea,Ward,BedNo,MedicaRecordNo,Diagnosis,Department,SendDoctor,SendDateTime,InspectDoctor,Status,CheckDoctor,InspectionItems,AcquisitionTime)");//2018-11-12 zlx add CheckDoctor
			strSql.Append(" values (");
			strSql.Append("@SampleNo,@PatientName,@Sex,@Age,@SampleType,@Source,@Position,@SampleContainer,@RepeatCount,@RegentBatch,@ProjectName,@Emergency,@ClinicNo,@InpatientArea,@Ward,@BedNo,@MedicaRecordNo,@Diagnosis,@Department,@SendDoctor,@SendDateTime,@InspectDoctor,@Status,@CheckDoctor,@InspectionItems,@AcquisitionTime)");
			OleDbParameter[] parameters = {
					new OleDbParameter("@SampleNo", OleDbType.VarChar,30),
					new OleDbParameter("@PatientName", OleDbType.VarChar,20),
					new OleDbParameter("@Sex", OleDbType.VarChar,10),
					new OleDbParameter("@Age", OleDbType.Double),
					new OleDbParameter("@SampleType", OleDbType.VarChar,30),
					new OleDbParameter("@Source", OleDbType.VarChar,30),
					new OleDbParameter("@Position", OleDbType.VarChar,20),
					new OleDbParameter("@SampleContainer", OleDbType.VarChar,20),
					new OleDbParameter("@RepeatCount", OleDbType.Integer,4),
					new OleDbParameter("@RegentBatch",OleDbType.VarChar,50),
					new OleDbParameter("@ProjectName", OleDbType.VarChar,255),
					new OleDbParameter("@Emergency", OleDbType.Integer,4),
					new OleDbParameter("@ClinicNo", OleDbType.VarChar,30),
					new OleDbParameter("@InpatientArea", OleDbType.VarChar,20),
					new OleDbParameter("@Ward", OleDbType.VarChar,20),
					new OleDbParameter("@BedNo", OleDbType.VarChar,20),
					new OleDbParameter("@MedicaRecordNo", OleDbType.VarChar,20),
					new OleDbParameter("@Diagnosis", OleDbType.VarChar,50),
					new OleDbParameter("@Department", OleDbType.VarChar,30),
					new OleDbParameter("@SendDoctor", OleDbType.VarChar,30),
					new OleDbParameter("@SendDateTime", OleDbType.Date),
					new OleDbParameter("@InspectDoctor", OleDbType.VarChar,30),
					new OleDbParameter("@Status", OleDbType.Integer,4),
					new OleDbParameter("@CheckDoctor", OleDbType.VarChar,30),
					new OleDbParameter("@InspectionItems", OleDbType.VarChar,255),
					new OleDbParameter("@AcquisitionTime", OleDbType.Date)};
			parameters[0].Value = model.SampleNo;
			parameters[1].Value = model.PatientName;
			parameters[2].Value = model.Sex;
			parameters[3].Value = model.Age;
			parameters[4].Value = model.SampleType;
			parameters[5].Value = model.Source;
			parameters[6].Value = model.Position;
			parameters[7].Value = model.SampleContainer;
			parameters[8].Value = model.RepeatCount;
			parameters[9].Value = model.RegentBatch;
			parameters[10].Value = model.ProjectName;
			parameters[11].Value = model.Emergency;
			parameters[12].Value = model.ClinicNo;
			parameters[13].Value = model.InpatientArea;
			parameters[14].Value = model.Ward;
			parameters[15].Value = model.BedNo;
			parameters[16].Value = model.MedicaRecordNo;
			parameters[17].Value = model.Diagnosis;
			parameters[18].Value = model.Department;
			parameters[19].Value = model.SendDoctor;
			parameters[20].Value = model.SendDateTime;
			parameters[21].Value = model.InspectDoctor;
			parameters[22].Value = model.Status;
			parameters[23].Value = model.CheckDoctor;
			parameters[24].Value = model.InspectionItems;
			parameters[25].Value = model.AcquisitionTime;

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
		public bool Update(BioBaseCLIA.Model.tbSampleInfo model)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("update tbSampleInfo set ");
			strSql.Append("SampleNo=@SampleNo,");
			strSql.Append("PatientName=@PatientName,");
			strSql.Append("Sex=@Sex,");
			strSql.Append("Age=@Age,");
			strSql.Append("SampleType=@SampleType,");
			strSql.Append("Source=@Source,");
			strSql.Append("[Position]=@Position,");
			strSql.Append("SampleContainer=@SampleContainer,");
			strSql.Append("RepeatCount=@RepeatCount,");
			strSql.Append("RegentBatch=@RegentBatch,");
			strSql.Append("ProjectName=@ProjectName,");
			strSql.Append("Emergency=@Emergency,");
			strSql.Append("ClinicNo=@ClinicNo,");
			strSql.Append("InpatientArea=@InpatientArea,");
			strSql.Append("Ward=@Ward,");
			strSql.Append("BedNo=@BedNo,");
			strSql.Append("MedicaRecordNo=@MedicaRecordNo,");
			strSql.Append("Diagnosis=@Diagnosis,");
			strSql.Append("Department=@Department,");
			strSql.Append("SendDoctor=@SendDoctor,");
			strSql.Append("SendDateTime=@SendDateTime,");
			strSql.Append("InspectDoctor=@InspectDoctor,");
			strSql.Append("Status=@Status,");
			strSql.Append("CheckDoctor=@CheckDoctor,");
			strSql.Append("InspectionItems=@InspectionItems");
			strSql.Append(" where SampleID=@SampleID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@SampleNo", OleDbType.VarChar,30),
					new OleDbParameter("@PatientName", OleDbType.VarChar,20),
					new OleDbParameter("@Sex", OleDbType.VarChar,10),
					new OleDbParameter("@Age", OleDbType.Double),
					new OleDbParameter("@SampleType", OleDbType.VarChar,30),
					new OleDbParameter("@Source", OleDbType.VarChar,30),
					new OleDbParameter("@Position", OleDbType.VarChar,20),
					new OleDbParameter("@SampleContainer", OleDbType.VarChar,20),
					new OleDbParameter("@RepeatCount", OleDbType.Integer,4),
					new OleDbParameter("@RegentBatch",OleDbType.VarChar,50),
					new OleDbParameter("@ProjectName", OleDbType.VarChar,255),
					new OleDbParameter("@Emergency", OleDbType.Integer,4),
					new OleDbParameter("@ClinicNo", OleDbType.VarChar,30),
					new OleDbParameter("@InpatientArea", OleDbType.VarChar,20),
					new OleDbParameter("@Ward", OleDbType.VarChar,20),
					new OleDbParameter("@BedNo", OleDbType.VarChar,20),
					new OleDbParameter("@MedicaRecordNo", OleDbType.VarChar,20),
					new OleDbParameter("@Diagnosis", OleDbType.VarChar,50),
					new OleDbParameter("@Department", OleDbType.VarChar,30),
					new OleDbParameter("@SendDoctor", OleDbType.VarChar,30),
					new OleDbParameter("@SendDateTime", OleDbType.Date),
					new OleDbParameter("@InspectDoctor", OleDbType.VarChar,30),
					new OleDbParameter("@Status", OleDbType.Integer,4),
					new OleDbParameter("@CheckDoctor", OleDbType.VarChar,30),//2018-11-12 zlx add
					new OleDbParameter("@InspectionItems", OleDbType.VarChar,255),
					new OleDbParameter("@SampleID", OleDbType.Integer,4)
			};
			parameters[0].Value = model.SampleNo;
			parameters[1].Value = model.PatientName;
			parameters[2].Value = model.Sex;
			parameters[3].Value = model.Age;
			parameters[4].Value = model.SampleType;
			parameters[5].Value = model.Source;
			parameters[6].Value = model.Position;
			parameters[7].Value = model.SampleContainer;
			parameters[8].Value = model.RepeatCount;
			parameters[9].Value = model.RegentBatch;
			parameters[10].Value = model.ProjectName;
			parameters[11].Value = model.Emergency;
			parameters[12].Value = model.ClinicNo;
			parameters[13].Value = model.InpatientArea;
			parameters[14].Value = model.Ward;
			parameters[15].Value = model.BedNo;
			parameters[16].Value = model.MedicaRecordNo;
			parameters[17].Value = model.Diagnosis;
			parameters[18].Value = model.Department;
			parameters[19].Value = model.SendDoctor;
			parameters[20].Value = model.SendDateTime;
			parameters[21].Value = model.InspectDoctor;
			parameters[22].Value = model.Status;
			parameters[23].Value = model.CheckDoctor;//2018-11-12 zlx add
			parameters[24].Value = model.InspectionItems;
			parameters[25].Value = model.SampleID;

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
		public bool UpdatePatientInfo(BioBaseCLIA.Model.tbSampleInfo model)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("update tbSampleInfo set ");
			strSql.Append("PatientName=@PatientName,");
			strSql.Append("Sex=@Sex,");
			strSql.Append("Age=@Age,");
			strSql.Append("ClinicNo=@ClinicNo,");
			strSql.Append("InpatientArea=@InpatientArea,");
			strSql.Append("Ward=@Ward,");
			strSql.Append("BedNo=@BedNo,");
			strSql.Append("MedicaRecordNo=@MedicaRecordNo,");
			strSql.Append("Diagnosis=@Diagnosis,");
			//2018-11-10 zlx add
			strSql.Append("Department=@Department,");
			strSql.Append("SendDoctor=@SendDoctor,");
			strSql.Append("SendDateTime=@SendDateTime,");
			strSql.Append("InspectDoctor=@InspectDoctor,");
			strSql.Append("CheckDoctor=@CheckDoctor,");
			strSql.Append("AcquisitionTime=@AcquisitionTime");
			strSql.Append(" where SampleID=@SampleID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@PatientName", OleDbType.VarChar,20),
					new OleDbParameter("@Sex", OleDbType.VarChar,10),
					new OleDbParameter("@Age", OleDbType.Double),
					new OleDbParameter("@ClinicNo", OleDbType.VarChar,30),
					new OleDbParameter("@InpatientArea", OleDbType.VarChar,20),
					new OleDbParameter("@Ward", OleDbType.VarChar,20),
					new OleDbParameter("@BedNo", OleDbType.VarChar,20),
					new OleDbParameter("@MedicaRecordNo", OleDbType.VarChar,20),
					new OleDbParameter("@Diagnosis", OleDbType.VarChar,50),
                    //2018-11-10 zlx add
                    new OleDbParameter("@Department", OleDbType.VarChar,20),
					new OleDbParameter("@SendDoctor", OleDbType.VarChar,20),
					new OleDbParameter("@SendDateTime", OleDbType.VarChar,50),
					new OleDbParameter("@InspectDoctor", OleDbType.VarChar,50),
					new OleDbParameter("@CheckDoctor", OleDbType.VarChar,50),
					new OleDbParameter("@AcquisitionTime", OleDbType.VarChar,50),
					new OleDbParameter("@SampleID", OleDbType.Integer,4)
					};
			parameters[0].Value = model.PatientName;
			parameters[1].Value = model.Sex;
			parameters[2].Value = model.Age;
			parameters[3].Value = model.ClinicNo;
			parameters[4].Value = model.InpatientArea;
			parameters[5].Value = model.Ward;
			parameters[6].Value = model.BedNo;
			parameters[7].Value = model.MedicaRecordNo;
			parameters[8].Value = model.Diagnosis;
			parameters[9].Value = model.Department;//2018-11-10 zlx add
			parameters[10].Value = model.SendDoctor;
			parameters[11].Value = model.SendDateTime;
			parameters[12].Value = model.InspectDoctor;
			parameters[13].Value = model.CheckDoctor;
			parameters[14].Value = model.AcquisitionTime;
			parameters[15].Value = model.SampleID;


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
		public bool Delete(string SampleNo)
		{

			StringBuilder strSql = new StringBuilder();
			strSql.Append("delete from tbSampleInfo ");
			strSql.Append(" where SampleNo=@SampleNo");
			OleDbParameter[] parameters = {
					new OleDbParameter("@SampleNo", OleDbType.VarChar,30)
			};
			parameters[0].Value = SampleNo;

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
		public bool DeleteOne(int SampleID)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("delete from tbSampleInfo ");
			strSql.Append(" where SampleID=@SampleID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@SampleID", OleDbType.Integer,4)
			};
			parameters[0].Value = SampleID;

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
		public bool DeleteList(string SampleIDlist)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("delete from tbSampleInfo ");
			strSql.Append(" where SampleID in (" + SampleIDlist + ")  ");
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
		public BioBaseCLIA.Model.tbSampleInfo GetModel(int SampleID)
		{

			StringBuilder strSql = new StringBuilder();
			strSql.Append("select SampleID,SampleNo,PatientName,Sex,Age,SampleType,Source,[Position],SampleContainer,RepeatCount,RegentBatch,ProjectName,Emergency,ClinicNo,InpatientArea,Ward,BedNo,MedicaRecordNo,Diagnosis,Department,SendDoctor,SendDateTime,InspectDoctor,Status,CheckDoctor,InspectionItems,AcquisitionTime from tbSampleInfo ");//2018-11-12 zlx add,CheckDoctor
			strSql.Append(" where SampleID=@SampleID");
			OleDbParameter[] parameters = {
					new OleDbParameter("@SampleID", OleDbType.Integer,4)
			};
			parameters[0].Value = SampleID;

			BioBaseCLIA.Model.tbSampleInfo model = new BioBaseCLIA.Model.tbSampleInfo();
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
		public BioBaseCLIA.Model.tbSampleInfo DataRowToModel(DataRow row)
		{
			BioBaseCLIA.Model.tbSampleInfo model = new BioBaseCLIA.Model.tbSampleInfo();
			if (row != null)
			{
				if (row["SampleID"] != null && row["SampleID"].ToString() != "")
				{
					model.SampleID = int.Parse(row["SampleID"].ToString());
				}
				if (row["SampleNo"] != null)
				{
					model.SampleNo = row["SampleNo"].ToString();
				}
				if (row["PatientName"] != null)
				{
					model.PatientName = row["PatientName"].ToString();
				}
				if (row["Sex"] != null)
				{
					model.Sex = row["Sex"].ToString();
				}
				if (row["Age"] != null)
				{
					model.Age = Convert.ToDouble(row["Age"]);
				}
				//model.Age=row["Age"].ToString();
				if (row["SampleType"] != null)
				{
					model.SampleType = row["SampleType"].ToString();
				}
				if (row["Source"] != null)
				{
					model.Source = row["Source"].ToString();
				}
				if (row["Position"] != null)
				{
					model.Position = row["Position"].ToString();
				}
				if (row["SampleContainer"] != null)
				{
					model.SampleContainer = row["SampleContainer"].ToString();
				}
				if (row["RepeatCount"] != null && row["RepeatCount"].ToString() != "")
				{
					model.RepeatCount = int.Parse(row["RepeatCount"].ToString());
				}
				if (row["RegentBatch"] != null)
				{
					model.RegentBatch = row["RegentBatch"].ToString();
				}
				if (row["ProjectName"] != null)
				{
					model.ProjectName = row["ProjectName"].ToString();
				}
				if (row["Emergency"] != null && row["Emergency"].ToString() != "")
				{
					model.Emergency = int.Parse(row["Emergency"].ToString());
				}
				if (row["ClinicNo"] != null)
				{
					model.ClinicNo = row["ClinicNo"].ToString();
				}
				if (row["InpatientArea"] != null)
				{
					model.InpatientArea = row["InpatientArea"].ToString();
				}
				if (row["Ward"] != null)
				{
					model.Ward = row["Ward"].ToString();
				}
				if (row["BedNo"] != null)
				{
					model.BedNo = row["BedNo"].ToString();
				}
				if (row["MedicaRecordNo"] != null)
				{
					model.MedicaRecordNo = row["MedicaRecordNo"].ToString();
				}
				if (row["Diagnosis"] != null)
				{
					model.Diagnosis = row["Diagnosis"].ToString();
				}
				if (row["Department"] != null)
				{
					model.Department = row["Department"].ToString();
				}
				if (row["SendDoctor"] != null)
				{
					model.SendDoctor = row["SendDoctor"].ToString();
				}
				if (row["SendDateTime"] != null && row["SendDateTime"].ToString() != "")
				{
					model.SendDateTime = DateTime.Parse(row["SendDateTime"].ToString());
				}
				if (row["InspectDoctor"] != null)
				{
					model.InspectDoctor = row["InspectDoctor"].ToString();
				}
				if (row["Status"] != null && row["Status"].ToString() != "")
				{
					model.Status = int.Parse(row["Status"].ToString());
				}
				//2018-11-12 zlx mod
				if (row["CheckDoctor"] != null && row["CheckDoctor"].ToString() != "")
				{
					model.CheckDoctor = row["CheckDoctor"].ToString();
				}
				if (row["InspectionItems"] != null && row["InspectionItems"].ToString() != "")
				{
					model.InspectionItems = row["InspectionItems"].ToString();
				}
				if (row["AcquisitionTime"] != null && row["AcquisitionTime"].ToString() != "")
				{
					model.AcquisitionTime = DateTime.Parse(row["AcquisitionTime"].ToString());
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
			strSql.Append("select SampleID,SampleNo,PatientName,Sex,Age,SampleType,Source,[Position],SampleContainer,RepeatCount,RegentBatch,ProjectName,Emergency,ClinicNo,InpatientArea,Ward,BedNo,MedicaRecordNo,Diagnosis,Department,SendDoctor,SendDateTime,InspectDoctor,Status,CheckDoctor,InspectionItems,AcquisitionTime ");//2018-11-12 zlx add,CheckDoctor
			strSql.Append(" FROM tbSampleInfo ");
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
			strSql.Append("select count(1) FROM tbSampleInfo ");
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
				strSql.Append("order by T.SampleID desc");
			}
			strSql.Append(")AS Row, T.*  from tbSampleInfo T ");
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
			parameters[0].Value = "tbSampleInfo";
			parameters[1].Value = "SampleID";
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

