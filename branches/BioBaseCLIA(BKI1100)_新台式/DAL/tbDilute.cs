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
    public partial class tbDilute
    {
        private const int connType = 3;
        public tbDilute()
        { }
        #region  BasicMethod

        /// <summary>
        /// 得到最大ID
        /// </summary>
        public int GetMaxId()
        {
            return DbHelperOleDb.GetMaxID(connType, "DiluteNumber", "tbDilute");
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int DiluteNumber)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from tbDilute");
            strSql.Append(" where DiluteNumber=@DiluteNumber");
            OleDbParameter[] parameters = {
					new OleDbParameter("@DiluteNumber", OleDbType.Integer,4)
			};
            parameters[0].Value = DiluteNumber;

            return DbHelperOleDb.Exists(connType, strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(BioBaseCLIA.Model.tbDilute model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into tbDilute(");
            strSql.Append("DiluteNumber,DilutePos,AllDiuVol,LeftDiuVol,Unit,AddData,ValiData ,State)");
            strSql.Append(" values (");
            strSql.Append("@DiluteNumber,@DilutePos,@AllDiuVol,@LeftDiuVol,@Unit,@AddData,@ValiData ,@State)");
            OleDbParameter[] parameters = {
					new OleDbParameter("@DiluteNumber", OleDbType.VarChar,30),
					new OleDbParameter("@DilutePos", OleDbType.Integer,4),
					new OleDbParameter("@AllDiuVol", OleDbType.Integer,4),
					new OleDbParameter("@LeftDiuVol", OleDbType.Integer,4),
					new OleDbParameter("@Unit", OleDbType.VarChar,30),
					new OleDbParameter("@AddData", OleDbType.VarChar,30),
					new OleDbParameter("@ValiData", OleDbType.VarChar,30),
					new OleDbParameter("@State", OleDbType.Integer,4)};
            parameters[0].Value = model.DiluteNumber;
            parameters[1].Value = model.DilutePos;
            parameters[2].Value = model.AllDiuVol;
            parameters[3].Value = model.LeftDiuVol;
            parameters[4].Value = model.Unit;
            parameters[5].Value = model.AddData;
            parameters[6].Value = model.ValiData;
            parameters[7].Value = model.State;
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
        public bool Update(BioBaseCLIA.Model.tbDilute model)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("update tbDilute set ");
            strSql.Append("DiluteNumber=@DiluteNumber,");
            strSql.Append("DilutePos=@DilutePos,");
            strSql.Append("AllDiuVol=@AllDiuVol,");
            strSql.Append("LeftDiuVol=@LeftDiuVol,");
            strSql.Append("Unit=@Unit,");
            strSql.Append("AddData=@AddData,");
            strSql.Append("ValiData=@ValiData,");
            strSql.Append("State=@State");
            strSql.Append(" where ID=@ID");
            OleDbParameter[] parameters = {
				new OleDbParameter("@DiluteNumber", OleDbType.VarChar,30),
					new OleDbParameter("@DilutePos", OleDbType.Integer,4),
					new OleDbParameter("@AllDiuVol", OleDbType.Integer,4),
					new OleDbParameter("@LeftDiuVol", OleDbType.Integer,4),
					new OleDbParameter("@Unit", OleDbType.VarChar,30),
					new OleDbParameter("@AddData", OleDbType.VarChar,30),
					new OleDbParameter("@ValiData", OleDbType.VarChar,30),
					new OleDbParameter("@State", OleDbType.Integer,4),
                    new OleDbParameter("@ID", OleDbType.Integer,10)};
            parameters[0].Value = model.DiluteNumber;
            parameters[1].Value = model.DilutePos;
            parameters[2].Value = model.AllDiuVol;
            parameters[3].Value = model.LeftDiuVol;
            parameters[4].Value = model.Unit;
            parameters[5].Value = model.AddData;
            parameters[6].Value = model.ValiData;
            parameters[7].Value = model.State;
            parameters[8].Value = model.ID;

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
        public bool Delete(int DilutePos)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from tbDilute ");
            strSql.Append(" where DilutePos=@DilutePos");
            OleDbParameter[] parameters = {
					new OleDbParameter("@DilutePos", OleDbType.Integer,30)
			};
            parameters[0].Value = DilutePos;

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
            strSql.Append("delete from tbDilute ");
            strSql.Append(" where DiluteNumber in (" + SubstrateIDlist + ")  ");
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
        public BioBaseCLIA.Model.tbDilute GetModel(int DiluteNumber)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select DiluteNumber,DilutePos,AllDiuVol,LeftDiuVol,Unit,AddData,ValiData ,State from tbDilute ");
            strSql.Append(" where DiluteNumber=@DiluteNumber");
            OleDbParameter[] parameters = {
					new OleDbParameter("@DiluteNumber", OleDbType.Integer,4)
			};
            parameters[0].Value = DiluteNumber;

            BioBaseCLIA.Model.tbDilute model = new BioBaseCLIA.Model.tbDilute();
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
        public BioBaseCLIA.Model.tbDilute DataRowToModel(DataRow row)
        {
            //DiluteNumber,DilutePos,AllDiuVol,LeftDiuVol,Unit,AddData,ValiData ,State
            BioBaseCLIA.Model.tbDilute model = new BioBaseCLIA.Model.tbDilute();
            if (row != null)
            {
                if (row["DiluteNumber"] != null && row["DiluteNumber"].ToString() != "")
                {
                    model.DiluteNumber = row["DiluteNumber"].ToString();
                }
                if (row["DilutePos"] != null)
                {
                    model.DilutePos = int.Parse(row["DilutePos"].ToString());
                }
                if (row["AllDiuVol"] != null)
                {
                    model.AllDiuVol = int.Parse(row["AllDiuVol"].ToString());
                }
                if (row["LeftDiuVol"] != null)
                {
                    model.LeftDiuVol = int.Parse(row["LeftDiuVol"].ToString());
                }
                if (row["Unit"] != null)
                {
                    model.Unit = row["Unit"].ToString();
                }
                if (row["AddData"] != null && row["AddData"].ToString() != "")
                {
                    model.AddData = row["AddData"].ToString();
                }
                if (row["ValiData"] != null && row["ValiData"].ToString() != "")
                {
                    model.ValiData = row["ValiData"].ToString();
                }
                if (row["State"] != null && row["State"].ToString() != "")
                {
                    model.State = int.Parse(row["State"].ToString());
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
            strSql.Append("select  DiluteNumber,DilutePos,AllDiuVol,LeftDiuVol,Unit,AddData,ValiData ,State ");
            strSql.Append(" FROM tbDilute ");
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
            strSql.Append("select count(1) FROM tbDilute ");
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
                strSql.Append("order by T.ID desc");
            }
            strSql.Append(")AS Row, T.*  from tbDilute T ");
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
