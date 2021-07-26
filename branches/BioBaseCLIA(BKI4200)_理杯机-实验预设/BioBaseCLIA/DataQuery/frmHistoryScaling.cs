using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Maticsoft.DBUtility;
using BioBaseCLIA.CalculateCurve;

namespace BioBaseCLIA.DataQuery
{
    public partial class frmHistoryScaling : frmSmallParent
    {
        public string tempItemName { get; set; }
        public string RegentBatch {get;set; }//2018-08-25 zlx add
        public int scalingResultID { get; set; }
        public Calculater Caler { get; set; }
        Calculater er = null;
        frmMessageShow fms = new frmMessageShow();
        public frmHistoryScaling()
        {
            InitializeComponent();

        }

        private void frmHistoryScaling_Load(object sender, EventArgs e)
        {
            DbHelperOleDb db = new DbHelperOleDb(1);
            //2018-08-25 zlx mod
            DataTable dtScalingResult = DbHelperOleDb.Query(1,@"select ScalingResultID,ItemName,ActiveDate,iif(Status=1, '是', '') AS bstatus from tbScalingResult 
                                                              where ItemName = '" + tempItemName + "' AND RegentBatch='" + RegentBatch + "'").Tables[0];
            dgvExistScal.DataSource = dtScalingResult;

        }

        private void dgvExistScal_SelectionChanged(object sender, EventArgs e)
        {
            List<Data_Value> CurveData = new List<Data_Value>();
            CalculateFactory ft = new CalculateFactory();

            drawCurve dc = new drawCurve();
            #region 查询选择项目的定标数据
            BLL.tbScalingResult bllscalingresult = new BLL.tbScalingResult();
            Model.tbScalingResult mdscalingresult = new Model.tbScalingResult();
            string ScalingResultID = dgvExistScal.CurrentRow.Cells["ScalingResultID"].Value.ToString();
            if (ScalingResultID == "")
            {
                return;
            }
            DbHelperOleDb db = new DbHelperOleDb(1);
            //获取scalingResult的实体
            mdscalingresult = bllscalingresult.GetModel(int.Parse(ScalingResultID));
            //获取定标点
            string[] curvePoints = mdscalingresult.Points.Split(';');
            if (curvePoints.Length == 0)
            {
                return;
            }
            #endregion
            db = new DbHelperOleDb(0);
            //数据库项目表中查询定标方程选择字段
            int CalMode = int.Parse(DbHelperOleDb.GetSingle(0, @"select CalMode from tbProject where ShortName = '"
                                                            + tempItemName + "'").ToString());
            //取到定标计算公式
            er = ft.getCaler(CalMode);
            #region 获取定标数据,对CurveData进行赋值
            for (int i = 0; i < curvePoints.Length; i++)
            {
                if (curvePoints[i] != "")
                {
                    //将每个定标点的浓度和RLU分开放到数组中
                    string[] pointsData = curvePoints[i].Split(',');
                    CurveData.Add(new Data_Value()
                    {
                        Data = double.Parse(pointsData[0].Substring(1)),
                        DataValue = double.Parse(pointsData[1].Substring(0, pointsData[1].IndexOf(")")))
                    });
                }
            }
            #endregion

            #region 对CurveData的数据进行处理
            for (int i = CurveData.Count - 2; i >= 0; i--)
            {
                Data_Value v2 = CurveData[i + 1];
                Data_Value v1 = CurveData[i];
                if (v2.Data == v1.Data)
                {
                    v1.DataValue = (v1.DataValue + v2.DataValue) / 2;
                    CurveData.RemoveAt(i + 1);
                }

            }
            //定义变量储存画曲线用的点的数据
            DataTable dt = new DataTable();
            dt.Columns.Add("consistence", typeof(float));
            dt.Columns.Add("PMT", typeof(float));//2018-07-21 zlx mod
            for (int i = 0; i < CurveData.Count; i++)
            {

                if (CurveData[i].Data == 0)
                {
                    CurveData[i].Data = 0.0001;
                }
                if (CurveData[i].Data == 1)
                {

                    CurveData[i].Data = 0.999999;
                }
                if (CurveData[i].DataValue == 0)
                {
                    CurveData[i].DataValue = 0.0001;
                }
            }
            for (int i = 0; i < CurveData.Count; i++)
            {
                //对处理过的数据进行纠错
                if (double.IsNaN(CurveData[i].DataValue) || double.IsNaN(CurveData[i].Data))
                {
                    fms.MessageShow("历史定标", "函数计算错误，可能该数据不适合此回归模型");
                    return;

                }
            }
            //将画定标曲线的点赋值给dt
            foreach (Data_Value dv in CurveData)
            {
                dt.Rows.Add(dv.Data, dv.DataValue);
            }

            //计算定标曲线的系数
            //for (int i = 0; i < CurveData.Count; i++)
            //{

            er.AddData(CurveData);
            er.Fit();
            //}

            #endregion
            string strTitle = "R^2:" + Convert.ToDouble(er.R2).ToString("0.###");
            dc.paintEliseScaling(dPnlCurve, dt, dt, er.GetResult, er.GetResult, strTitle, CalMode);
        }

        private void fbtnChoice_Click(object sender, EventArgs e)
        {
            Caler = er;
            if (dgvExistScal.RowCount > 0)//2018-07-23 zlx add
            {
                DbHelperOleDb db = new DbHelperOleDb(1);
                DbHelperOleDb.ExecuteSql(1,@"update tbScalingResult set Status=1 where ItemName = '" + dgvExistScal.CurrentRow.Cells["ItemName"].Value + "'AND ScalingResultID = "
                                                       + Convert.ToInt32(dgvExistScal.CurrentRow.Cells["ScalingResultID"].Value) + "").ToString();
                DbHelperOleDb.ExecuteSql(1,@"update tbScalingResult set Status=0 where ItemName = '" + dgvExistScal.CurrentRow.Cells["ItemName"].Value + "'AND ScalingResultID <> "
                                                       + Convert.ToInt32(dgvExistScal.CurrentRow.Cells["ScalingResultID"].Value) + " AND RegentBatch='" + RegentBatch + "'").ToString();
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}     

