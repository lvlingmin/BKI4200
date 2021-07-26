using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Maticsoft.DBUtility;
using System.Drawing.Printing;
using System.Threading;
using BioBaseCLIA.CalculateCurve;

namespace BioBaseCLIA.DataQuery
{
    /// <summary>
    /// 功能简介：全自动化学发光定标查询界面，设置查询定标数据和曲线。
    /// 完成日期：20170721
    /// 编写人：刘亚男
    /// 版本：1.0
    /// </summary>
    public partial class frmScalingQuery : frmParent
    {
        #region 定义属性和对象
        /// <summary>
        /// 定义tbScalingResult表的model实例，BLL实例
        /// </summary>
        private Model.tbScalingResult modelscalingResult = new Model.tbScalingResult();
        private BLL.tbScalingResult bllscalingResult = new BLL.tbScalingResult();
        /// <summary>
        /// 新建画定标曲线的实例
        /// </summary>
        drawCurve dc = new drawCurve();
        /// <summary>
        /// 定标曲线画图显示点
        /// </summary>
        List<Data_Value> ltData = new List<Data_Value>();
        /// <summary>
        /// 定标曲线计算点
        /// </summary>
        List<Data_Value> CurveData = new List<Data_Value>();
        /// <summary>
        /// 新建定标方程类的实例
        /// </summary>
        CalculateFactory ft = new CalculateFactory();
        /// <summary>
        /// 新建变量用于计算定标曲线系数
        /// </summary>
        Calculater er = null;
        /// <summary>
        /// 中间变量，存储处理过的图片保存路径
        /// </summary>
        string temp = string.Empty;
        /// <summary>
        /// 存储图片保存文件名
        /// </summary>
        string fileNameExt = string.Empty;
        /// <summary>
        /// 存储图片保存路径
        /// </summary>
        string localFilePath = string.Empty;
        #endregion

        public frmScalingQuery()
        {
            InitializeComponent();

        }
        private void frmScalingQuery_Load(object sender, EventArgs e)
        {

            #region 查询并显示所有的项目名称
            DbHelperOleDb db = new DbHelperOleDb(0);
            new Thread(new ParameterizedThreadStart((obj) =>
            {
                DataTable dtItemName = DbHelperOleDb.Query(@"select * from tbProject where ProjectType = '1'").Tables[0];
                if (dtItemName.Rows.Count == 0)
                {
                    return;
                }
              
                foreach (DataRow row in dtItemName.Rows)
                {
                      Invoke(new Action(() =>
                                        {
                                            
                                            cmbItem.Items.Add(row["ShortName"].ToString());
                                        }));
                }

                Invoke(new Action(() =>
                {
                    cmbItem.SelectedIndex = 0;
                    cmbItem.Focus();
                }));
            })) { IsBackground = true }.Start();
            
            #endregion
        }


        private void frmScalingQuery_SizeChanged(object sender, EventArgs e)
        {
            formSizeChange(this);
        }

        #region 右侧按钮点击方法
        private void fbtnResultQuery_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmResultQuery"))
            {
                frmResultQuery frmRQ = new frmResultQuery();
                frmRQ.TopLevel = false;
                frmRQ.Parent = this.Parent;
                frmRQ.Show();
            }
            else
            {
                frmResultQuery frmRQ = (frmResultQuery)Application.OpenForms["frmResultQuery"];
                frmRQ.BringToFront(); ;

            }
        }

        private void fbtnQCQuery_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmQCQuery"))
            {
                frmQCQuery frmQC = new frmQCQuery();
                frmQC.TopLevel = false;
                frmQC.Parent = this.Parent;
                frmQC.Show();
            }
            else
            {
                frmQCQuery frmQC = (frmQCQuery)Application.OpenForms["frmQCQuery"];
                frmQC.BringToFront(); ;

            }
        }

        private void fbtnReturn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 窗体中其他功能按钮
        private void fbtnCurrentCals_Click(object sender, EventArgs e)
        {
            frmTowPointsCal frmTPC = new frmTowPointsCal();
            frmTPC.Show();
        }
        /// <summary>
        /// 点击设置当前选中的定标曲线为当前定标，且其他相同项目的定标曲线设置为非当前定标曲线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fbtnCurrentCurve_Click(object sender, EventArgs e)
        {
            DbHelperOleDb db = new DbHelperOleDb(1);
            DbHelperOleDb.ExecuteSql("update tbScalingResult set Status=1 where ScalingResultID ="
                                          + int.Parse(dgvExistScal.CurrentRow.Cells["ScalingResultID"].Value.ToString()));
            db = new DbHelperOleDb(1);
            DbHelperOleDb.ExecuteSql("update tbScalingResult set Status=0 where ScalingResultID <>"
                                          + int.Parse(dgvExistScal.CurrentRow.Cells["ScalingResultID"].Value.ToString())
                                          + "and ItemName = '" + cmbItem.SelectedItem + "'");
            fbtnCurrentCurve.Enabled = false;
            txtStatus.Text = "正在使用";
        }
        #region 定标曲线及公式等显示、计算及打印
        private void fbtnShowCurve_Click(object sender, EventArgs e)
        {
            if (dgvScalingData.Rows.Count == 0)
            {
                return;
            }
            DbHelperOleDb db = new DbHelperOleDb(0);
            ltData = new List<Data_Value>();
            CurveData = new List<Data_Value>();
            //数据库项目表中查询定标方程选择字段
            int CalMode = int.Parse(DbHelperOleDb.GetSingle(@"select CalMode from tbProject where ShortName = '"
                                                            + cmbItem.SelectedItem + "'").ToString());
            //if (CalMode == null)
            //{
            //    return;
            //}
            //取到定标计算公式
            er = ft.getCaler(CalMode);
            #region 获取定标数据,对ltData、CurveData进行赋值
            //定标点数（定标数据行数）
            int ScalingDataCount = 0;
            for (int i = 0; i < dgvScalingData.Rows.Count; i++)
            {
                if (dgvScalingData[0, i].Value != null && dgvScalingData[1, i].Value != null)
                    ScalingDataCount++;
            }
            for (int j = 0; j < ScalingDataCount; j++)
            {
                ltData.Add(new Data_Value() { Data = double.Parse(dgvScalingData[0, j].Value.ToString()), DataValue = double.Parse(dgvScalingData[1, j].Value.ToString()) });
                CurveData.Add(new Data_Value() { Data = double.Parse(dgvScalingData[0, j].Value.ToString()), DataValue = double.Parse(dgvScalingData[1, j].Value.ToString()) });
            }
            #endregion

            #region 对ltData、CurveData的数据进行处理
            ltData.Sort(new Data_ValueDataAsc());
            CurveData.Sort(new Data_ValueDataAsc());
            for (int i = ltData.Count - 2; i >= 0; i--)
            {
                Data_Value v2 = ltData[i + 1];
                Data_Value v1 = ltData[i];
                if (v2.Data == v1.Data)
                {
                    v1.DataValue = (v1.DataValue + v2.DataValue) / 2;
                    ltData.RemoveAt(i + 1);
                }

            }
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
            dt.Columns.Add("absorbency", typeof(float));
            if (ltData.Count > 0)//ltData标准品
            {

                for (int i = 0; i < ltData.Count; i++)
                {

                    if (ltData[i].Data == 0)
                    {
                        ltData[i].Data = 0.0001;
                    }
                    if (ltData[i].Data == 1)
                    {
                        ltData[i].Data = 0.999999;
                    }
                    if (ltData[i].DataValue == 0)
                    {
                        ltData[i].DataValue = 0.0001;
                    }
                }
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
                for (int i = 0; i < ltData.Count; i++)
                {
                    //对处理过的数据进行纠错
                    if (double.IsNaN(ltData[i].Data) || double.IsNaN(ltData[i].DataValue) || double.IsNaN(CurveData[i].DataValue) || double.IsNaN(CurveData[i].Data))
                    {
                        MessageBox.Show("函数计算错误，可能该数据不适合此回归模型", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;

                    }
                }
                //将画定标曲线的点赋值给dt
                foreach (Data_Value dv in ltData)
                {
                    dt.Rows.Add(dv.Data, dv.DataValue);
                }

                //计算定标曲线的系数
                for (int i = 0; i < CurveData.Count; i++)
                {

                    er.AddData(CurveData);
                    er.Fit();
                }
            }


            #endregion

            foreach (double par in er._pars)
            {
                if (double.IsNaN(par) || double.IsInfinity(par))
                {
                    MessageBox.Show("回归计算时出现运算错误，可能该数据不适合此回归模型", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            EquationCurve(er, dt, CalMode);
        }

        #region 方程及曲线显示
        /// <summary>
        /// 定标曲线及方程显示
        /// </summary>
        /// <param name="er"></param>
        /// <param name="dt"></param>
        /// <param name="calMode"></param>
        public void EquationCurve(Calculater er, DataTable dt, int calMode)
        {
            //定义变量用来存储定标曲线系数由string类型转换为double类型的数据
            double tempCal = 0;
            //存储定标曲线处理后的系数
            string newPars = string.Empty;
            foreach (string par in er.StrPars.Split('|'))
            {
                if (double.TryParse(par, out tempCal))
                {
                    if (double.IsInfinity(tempCal) || double.IsNaN(tempCal))
                    {

                        newPars += "0|";
                    }
                    else
                    {
                        newPars += par + "|";
                    }
                }
                else
                {

                    newPars += par + "0|";
                }
            }
            //在definePanal1中画出定标曲线
            dc.paintEliseScaling(definePanal1, dt, er.GetResult, "", calMode);//mdscalingInfo.fittingResult.IsNullOrEmpty() ? "" : "R: " + Convert.ToDouble(mdscalingInfo.fittingResult).ToString("0.###")
            #region 获取方程参数
            //存储定标曲线系数
            string[] strpar = er.StrPars.Split('|');
            //存储定标曲线方程
            string Furmula = string.Empty;
            if (strpar.Length > 0)
            {
                switch (calMode)
                {
                    case 0:
                        txtFormulaInfo.Clear();
                        Furmula = "方程式： y = (A - D) / [1 + (x/C)^B] + D";
                        txtFormulaInfo.AppendText("四参数Logistic曲线拟合：" + "\r\n" + Furmula + "\r\n" + "A: " + strpar[0] + "\r\n" + "B: " + strpar[1] + "\r\n" + "C: " + strpar[2] + "\r\n" + "D: " + strpar[3] + "\r\n" + "R^2: " + er.R2);
                        break;
                    case 1:
                        txtFormulaInfo.Clear();
                        Furmula = "方程式： y=A+B/(1+exp(-(C+D*ln(x))))";
                        txtFormulaInfo.AppendText("四参数Logistic曲线拟合：" + "\r\n" + Furmula + "\r\n" + "A: " + strpar[0] + "\r\n" + "B: " + strpar[1] + "\r\n" + "C: " + strpar[2] + "\r\n" + "D: " + strpar[3] + "\r\n" + "R^2: " + er.R2);
                        break;

                }
            }
            #endregion


        }
        #endregion
        #region 右击保存定标曲线图片

        private void definePanal1_MouseDown(object sender, MouseEventArgs e)
        {
            //鼠标右键显示保存图像的菜单
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(e.X, e.Y);
                MenuCurve.Show(definePanal1, p);
            }
        }


        private void itemSave_Click(object sender, EventArgs e)
        {

            Graphics gSrc = definePanal1.CreateGraphics();
            Bitmap bmSave = new Bitmap(this.definePanal1.Width, this.definePanal1.Height);
            SaveFileDialog path = new SaveFileDialog();
            path.Filter = "jpg文件(*.JPEG)|*.JPEG|All File(*.*)|*.*";
            path.AddExtension = true;
            path.OverwritePrompt = true;
            if (path.ShowDialog() == DialogResult.OK)
            {
                //获得文件路径 
                localFilePath = path.FileName.ToString();
                //获取文件名，不带路径
                fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1);

            }
            if (localFilePath == "")
                return;

            else
            {
                temp = localFilePath.Substring(0, localFilePath.LastIndexOf("\\"));
                definePanal1.DrawToBitmap(bmSave, new Rectangle(0, 0, bmSave.Width, bmSave.Height));
                bmSave.Save(@temp + "\\" + fileNameExt);
            }
        }
        #endregion

        private void fbtnPrintCurve_MouseDown(object sender, MouseEventArgs e)
        {
            //定标曲线浓度和吸光度数据
            DataTable dt = new DataTable();
            dt.Columns.Add("consistence", typeof(float));
            dt.Columns.Add("absorbency", typeof(float));
            //将画定标曲线的点赋值给dt
            foreach (Data_Value dv in ltData)
            {
                dt.Rows.Add(dv.Data, dv.DataValue);
            }
            reportCurve reportscaling = new reportCurve(dt, er, cmbItem.SelectedItem.ToString());
            //打印定标报告
            if (e.Button == MouseButtons.Left)
            {
                reportscaling.print();
            }
            //预览定标曲线报告
            else if (e.Button == MouseButtons.Right)
            {
                reportscaling.Preview();
            }
        }
        #endregion
        #endregion


        private void cmbItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            #region 根据项目选择的不同，显示已有定标
            if (cmbItem.SelectedItem == null)
            {
                return;
            }
            DbHelperOleDb db = new DbHelperOleDb(1);
            DataTable dtScalingResult = DbHelperOleDb.Query(@"select ScalingResultID,ActiveDate from tbScalingResult 
                                                              where ItemName = '" + cmbItem.SelectedItem + "'").Tables[0];
            dgvExistScal.DataSource = dtScalingResult;
            if (dtScalingResult.Rows.Count==0)
            {
                clearData();                
            }
            #endregion

        }

        /// <summary>
        /// 清空界面上的曲线和数据
        /// </summary>
        public void clearData()
        {
            dgvScalingData.Rows.Clear();
            definePanal1.BackgroundImage = null;
            txtFormulaInfo.Text = "";
            txtItemName.Text = "";
            txtActiveTime.Text = "";
            txtRegentBatch.Text = "";
            txtSource.Text = "";
            txtStatus.Text = "";
        }

        private void dgvExistScal_SelectionChanged(object sender, EventArgs e)
        {
            #region 根据选择的已有定标不同定标信息和数据
            if (dgvExistScal.CurrentRow == null)
            {
                return;
            }
            //获取当前选中的一列的ScalingResultID值
            string ScalingResultID = dgvExistScal.CurrentRow.Cells["ScalingResultID"].Value.ToString();
            if (ScalingResultID == "")
            {
                return;
            }
            DbHelperOleDb db = new DbHelperOleDb(1);
            //获取scalingResult的实体
            modelscalingResult = bllscalingResult.GetModel(int.Parse(ScalingResultID));
            #region 显示定标信息
            txtItemName.Text = modelscalingResult.ItemName;
            txtActiveTime.Text = modelscalingResult.ActiveDate.ToString();
            txtRegentBatch.Text = modelscalingResult.RegentBatch;
            if (modelscalingResult.Source == 1)
            {
                txtSource.Text = "内部";
            }
            else
            {
                txtSource.Text = "外部";
            }
            if (modelscalingResult.Status == 1)
            {
                txtStatus.Text = "正在使用";
                fbtnCurrentCurve.Enabled = false;
            }
            else
            {
                txtStatus.Text = "暂未使用";
                fbtnCurrentCurve.Enabled = true;
            }
            if (modelscalingResult.ScalingModel == 6)
            {
                chkTwoPoints.Checked = false;
            }
            else
            {
                chkTwoPoints.Checked = true;
            }
            #endregion

            #region 显示定标数据
            //获取定标点
            string[] curvePoints = modelscalingResult.Points.Split(';');
            if (curvePoints.Length == 0)
            {
                return;
            }
            if (dgvScalingData.Rows.Count < curvePoints.Length)
            dgvScalingData.Rows.Add(curvePoints.Length);
            
            for (int i = 0; i < curvePoints.Length; i++)
            {
                if (curvePoints[i] == "")
                    continue;
                //将每个定标点的浓度和RLU分开放到数组中
                string[] pointsData = curvePoints[i].Split(',');
                dgvScalingData[0, i].Value = pointsData[0].Substring(1);
                dgvScalingData[1, i].Value = pointsData[1].Substring(0, pointsData[1].IndexOf(")"));

            }
            #endregion

            #region 显示定标曲线和方程
            fbtnShowCurve_Click(null, null);
            #endregion
            #endregion
        }
        /// <summary>
        /// 根据两点校准的选择不同，实时更新数据库中的定标模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkTwoPoints_CheckedChanged(object sender, EventArgs e)
        {
            DbHelperOleDb db = new DbHelperOleDb(1);
            if (chkTwoPoints.Checked)
            {
                fbtnCurrentCals.Enabled = true;
                DbHelperOleDb.ExecuteSql("update tbScalingResult set ScalingModel=2 where ScalingResultID ="
                                          + int.Parse(dgvExistScal.CurrentRow.Cells["ScalingResultID"].Value.ToString()));
            }
            else
            {
                fbtnCurrentCals.Enabled = false;
                DbHelperOleDb.ExecuteSql("update tbScalingResult set ScalingModel=6 where ScalingResultID ="
                                          + int.Parse(dgvExistScal.CurrentRow.Cells["ScalingResultID"].Value.ToString()));
            }
        }





    }
}
