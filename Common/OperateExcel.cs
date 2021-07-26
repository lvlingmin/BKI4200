using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.IO;

namespace Common
{
    public class OperateExcel
    {
        OleDbConnection conn;//excel 连接
        public static DataTable ExcelTable { get; set; }
        /// <summary>
        /// 建立连接
        /// </summary>
        /// <param name="inPath">要连接Excel表格的地址</param>
        /// <returns></returns>
        public static OleDbConnection GetConn(string inPath)
        {
            string filePath = inPath;
            FileInfo fileInfo = new FileInfo(filePath);
            string directory = fileInfo.DirectoryName;
            string strConn2 = @"Provider=microsoft.jet.oledb.4.0;Data Source='{0}';Extended Properties='Excel 8.0;HDR=Yes;IMEX=0;'";
            string strConnection = string.Format(strConn2, inPath);
            OleDbConnection conn = new OleDbConnection(strConnection);
            return conn;
        }
        /// <summary>
        /// 从Excel表向系统导入数据
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static DataTable ImPortExcel(string path)
        {
            DataTable dtOld = new DataTable();
            String sConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=2'";
            using (OleDbConnection ole_conn = new OleDbConnection(sConnectionString))
            {
                ole_conn.Open();
                using (OleDbCommand ole_cmd = ole_conn.CreateCommand())
                {
                    String tableName = null;
                    DataTable dt = ole_conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    tableName = dt.Rows[0][2].ToString().Trim();
                    //类似SQL的查询语句这个[Sheet1$对应Excel文件中的一个工作表]
                    ole_cmd.CommandText = @"select * from [" + tableName + "]";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(ole_cmd);
                    adapter.Fill(dtOld);
                }
            }
            return dtOld;
        }
        /// <summary>
        /// 由系统向外部导出数据
        /// </summary>
        /// <param name="OleConn"></param>
        /// <param name="writeDataTb"></param>
        public static void ExPortToExcel(string path)
        {
            string sConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=Excel 8.0;";
             using (OleDbConnection ole_conn = new OleDbConnection(sConnectionString))
             {
                ole_conn.Open();
                using (OleDbCommand ole_cmd = ole_conn.CreateCommand())
                {
                    System.Data.DataTable dt = ole_conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    string tableName = dt.Rows[0][2].ToString().Trim();
                    foreach (DataRow dr in ExcelTable.Rows)
                    {
                        ole_cmd.CommandText = @"insert into [" + tableName + "](样本编号 ,项目名称,实验时间 ,发光值,浓度 ,单位 ,结果 ,参考值 )values('" + dr[2] + "','" + dr[3] + "','" + dr[4] + "','" + dr[5] + "','" + dr[6] + "','" + dr[7] + "','" + dr[8] + "','" + dr[9] + "')";
                        ole_cmd.ExecuteNonQuery();
                    }
                }
             }
        }
        /// <summary>
        /// 创建新的Excel表格
        /// </summary>
        /// <param name="OleConn"></param>
        /// <param name="listColum"></param>
        public static void CreatExcel(string path)
        {
            
            String sConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=Excel 8.0;";
            //实例化一个Oledbconnection类(实现了IDisposable,要using)
            using (OleDbConnection ole_conn = new OleDbConnection(sConnectionString))
            {
                ole_conn.Open();
                using (OleDbCommand ole_cmd = ole_conn.CreateCommand())
                {
                    
                    //DataTable dt = ole_conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    //if (dt.Rows.Count > 0)
                    //{
                    //    File.Delete(path);
                    //    //ole_cmd.CommandText = "DROP TABLE " + ExcelTable.TableName + "";
                    //    //ole_cmd.ExecuteNonQuery();
                    //}
                    ole_cmd.CommandText = "CREATE TABLE " + ExcelTable.TableName + "([样本编号] VarChar,[项目名称] VarChar,[实验时间] VarChar,[发光值] VarChar,[浓度] VarChar,[单位] VarChar,[结果] VarChar,[参考值] VarChar)";
                    ole_cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
