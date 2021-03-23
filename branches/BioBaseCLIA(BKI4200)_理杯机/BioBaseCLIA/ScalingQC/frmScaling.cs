using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Maticsoft.DBUtility;
using BioBaseCLIA.Run;
using System.Threading;
using BioBaseCLIA.CalculateCurve;
using BioBaseCLIA.DataQuery;
using System.Timers; //lyq add 20190828
using System.Resources;

namespace BioBaseCLIA.ScalingQC
{
    public partial class frmScaling : frmParent
    {
        BLL.tbProject blltbproject = new BLL.tbProject();
        BLL.tbReagent bllregent = new BLL.tbReagent();
        BLL.tbMainScalCurve bllMainCurve = new BLL.tbMainScalCurve();
        BLL.tbScalingResult bllscalResult = new BLL.tbScalingResult();
        DataTable dtProject = new DataTable();
        DataTable dtScalInfo = new DataTable();
        DataTable dtScalResult = new DataTable();
        /// <summary>
        /// 定标曲线显示点
        /// </summary>
        List<Data_Value> ltData = new List<Data_Value>();
        /// <summary>
        /// 定标主曲线显示点
        /// </summary>
        List<Data_Value> MainltData = new List<Data_Value>();
        /// <summary>
        /// 新建定标方程类的实例
        /// </summary>
        CalculateFactory ft = new CalculateFactory();
        /// <summary>
        /// 新建变量用于计算定标曲线系数
        /// </summary>
        Calculater er = null;
        /// <summary>
        /// 新建画定标曲线的实例
        /// </summary>
        drawCurve dc = new drawCurve();
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
        frmMessageShow frmMS = new frmMessageShow();

        //lyq add 20190828
        Thread th;
        public frmScaling()
        {
            InitializeComponent();
            dtScalInfo.Columns.Add("ItemName", typeof(string));
            dtScalInfo.Columns.Add("RegentBatch", typeof(string));
            dtScalInfo.Columns.Add("MainCurve", typeof(string));
            dtScalInfo.Columns.Add("Scaling", typeof(string));
            dtScalInfo.Columns.Add("CalType", typeof(string));
            dtScalInfo.Columns.Add("ActiveDate", typeof(string));
            dtScalInfo.Columns.Add("ValidDate", typeof(string));
            dtScalInfo.Columns.Add("ExpiryDate", typeof(int));//2018-07-30 zlx add
        }

        private void frmScaling_Load(object sender, EventArgs e)
        {
            frmAddScaling.AddMainCurve += new Action(RefreshUI);

            //lyq mod 20190828
            th = new Thread(new ParameterizedThreadStart((obj) =>
            {
                RefreshUI();
            }));
            th.IsBackground = true;
            th.Start();
            //new Thread(new ParameterizedThreadStart((obj) =>
            //{
            //   RefreshUI();
            //})) { IsBackground = true }.Start();

        }

        public void RefreshUI()
        {
            //lyq mod 20191009
            dtScalInfo.Clear();
            DbHelperOleDb db = new DbHelperOleDb(0);
            dtProject = blltbproject.GetList(" ProjectType = '1'").Tables[0];
            db = new DbHelperOleDb(1);
            dtScalResult = bllscalResult.GetList("").Tables[0];
            DataTable dtMainScalCurve = bllMainCurve.GetAllList().Tables[0];
            db = new DbHelperOleDb(3);
            DataTable dtt = bllregent.GetList("Status= '1'").Tables[0]; //获得数据库该表全部数据            
            if (dtScalResult.Rows.Count > 0)
            {
                foreach (DataRow drScal in dtScalResult.Rows)
                {
                    DataRow[] drProject = dtProject.Select("ShortName='" + drScal["ItemName"] + "'");
                    if (dtProject.Select("ShortName='" + drScal["ItemName"] + "'").Length > 0 && drScal["Status"].ToString() == "1")
                    {
                        bool ExitsMainCurve = dtMainScalCurve.Select("ItemName='" + drScal["ItemName"] + " ' AND RegentBatch='" + drScal["RegentBatch"] + "'").Length > 0;
                        int expiryDate = int.Parse(drProject[0]["ExpiryDate"].ToString());
                        DataRow dr = dtScalInfo.NewRow();
                        dr["ItemName"] = drScal["ItemName"];
                        dr["RegentBatch"] = drScal["RegentBatch"];
                        dr["MainCurve"] = ExitsMainCurve ? "Y" : "N";
                        dr["Scaling"] = "Y";
                        dr["CalType"] = drScal["ScalingModel"].ToString() == "6" ? getString("keywordText.SixPoint") : getString("keywordText.TwoPoint");
                        dr["ActiveDate"] = Convert.ToDateTime(drScal["ActiveDate"]).ToString("yyyy-MM-dd");
                        dr["ValidDate"] = (Convert.ToDateTime(drScal["ActiveDate"]).AddDays(expiryDate)).ToString();
                        dr["ExpiryDate"] = drProject[0]["ExpiryDate"];
                        dtScalInfo.Rows.Add(dr);
                    }
                }
            }
            if (dtScalInfo.Rows.Count > 1)
                dtScalInfo = Distinct(dtScalInfo, "ItemName", "RegentBatch");

            //lyq add 20190911
            DataView dv = dtScalInfo.DefaultView;
            dv.Sort = "ActiveDate DESC";
            dtScalInfo = dv.ToTable();

            while (!this.IsHandleCreated)//2018-11-24 zlx add
            {
                NetCom3.Delay(100);
            }
            Invoke(new Action(() =>
            {
                dgvScalData.DataSource = dtScalInfo;
                //dgvScalData.Rows[0].Selected = true;
            }));
        }
        /// <summary>
        /// 去除表中的重复元素
        /// </summary>
        /// <param name="dt">需要处理的表</param>
        /// <param name="str1">列1</param>
        /// <param name="str2">列2</param>
        /// <returns></returns>
        public static DataTable Distinct(DataTable dt, string str1, string str2)
        {
            for (int i = dt.Rows.Count - 2; i > 0; i--)
            {
                DataRow[] rows = dt.Select(str1 + "= '" + dt.Rows[i][str1] + "' and " + str2 + "= '" + dt.Rows[i][str2] + "'");
                if (rows.Length > 1)
                {
                    dt.Rows.RemoveAt(i);
                }
            }
            return dt;
        }
        private void frmScaling_SizeChanged(object sender, EventArgs e)
        {
            formSizeChange(this);
        }

        private void fbtnAddMainCurve_Click(object sender, EventArgs e)
        {
            if (dgvScalData.CurrentRow == null)
            {
                return;
            }
            string ItemName = dgvScalData.CurrentRow.Cells["colItemName"].Value.ToString();
            string Batch = dgvScalData.CurrentRow.Cells["colRegentBatch"].Value.ToString();
            string validdate = dgvScalData.CurrentRow.Cells["colValidDate"].Value.ToString();
            string activedate = dgvScalData.CurrentRow.Cells["colActiveDate"].Value.ToString();
            frmAddScaling frmAS = new frmAddScaling(ItemName, Batch, activedate, validdate);
            frmAS.Show();
        }

        private void dgvScalData_SelectionChanged(object sender, EventArgs e)
        {
            //2018-08-29 zlx add
            clearData();
            if (dgvScalData.CurrentRow == null)
            {
                return;
            }
            string ItemName = dgvScalData.CurrentRow.Cells["colItemName"].Value.ToString();
            string Batch = dgvScalData.CurrentRow.Cells["colRegentBatch"].Value.ToString();
            ShowScalCurve(ItemName, Batch);

        }
        /// <summary>
        /// 显示定标曲线
        /// </summary>
        void ShowScalCurve(string itemname, string batch)
        {
            DataRow[] ScalCurve = dtScalResult.Select("ItemName = '" + itemname
                                    + "' and RegentBatch = '" + batch + "' and Status = 1");

            int ScalingMod = 0;
            bool ScalingModIsSix = false;
            if (ScalCurve.Length != 0)
                ScalingModIsSix = int.TryParse(ScalCurve[0]["ScalingModel"].ToString(), out ScalingMod);
            if (ScalingMod < 6)//2019-02-12 zlx mod
            {
                ScalingModIsSix = false;
            }
            DbHelperOleDb db = new DbHelperOleDb(0);
            #region 定标曲线计算变量
            ft = new CalculateFactory();
            er = null;
            ltData = new List<Data_Value>();
            //定义变量储存画曲线用的点的数据
            DataTable dt = new DataTable();
            dt.Columns.Add("consistence", typeof(float));
            dt.Columns.Add("PMT", typeof(float));
            #endregion
            #region 主曲线计算变量
            Calculater er1 = null;
            DataTable dtMain = new DataTable();
            dtMain.Columns.Add("consistence", typeof(float));
            dtMain.Columns.Add("PMT", typeof(float));
            MainltData = new List<Data_Value>();
            #endregion
            dc = new drawCurve();
            //数据库项目表中查询定标方程选择字段
            int CalMode = int.Parse(DbHelperOleDb.GetSingle(0, @"select CalMode from tbProject where ShortName = '" + itemname + "'").ToString());
            er = ft.getCaler(CalMode);
            //定义主曲线变量储存画曲线用的点的数据
            er1 = ft.getCaler(CalMode);
            #region 获取主曲线数据
            db = new DbHelperOleDb(1);
            DataTable dt1 = bllMainCurve.GetList(" ItemName='" + itemname + "' and RegentBatch = '" + batch + "'").Tables[0];
            if (dt1.Rows.Count == 0)
            {
                if (ScalingModIsSix)
                {
                    goto noNeedMainScal;
                }
                return;
            }
            string[] curvePointsMain = dt1.Rows[0]["Points"].ToString().Split(';');
            //2017-4-19 zlx add 修改点击没有主曲线的数据报错。
            if (curvePointsMain.Length == 0)
            {
                if (ScalingModIsSix)
                {
                    goto noNeedMainScal;
                }
                return;
            }
            for (int i = 0; i < curvePointsMain.Length; i++)
            {
                if (curvePointsMain[i] == "")
                    continue;
                //将每个定标点的浓度和RLU分开放到数组中
                string[] pointsData = curvePointsMain[i].Split(',');
                MainltData.Add(new Data_Value()
                {
                    Data = double.Parse(pointsData[0].Substring(1)),
                    DataValue = double.Parse(pointsData[1].Substring(0, pointsData[1].IndexOf(")")))
                });
            }
            MainltData.Sort(new Data_ValueDataAsc());
            #region 主曲线显示
            //if (chbShowMainCurve.Checked)
            //{

            for (int i = MainltData.Count - 2; i >= 0; i--)
            {
                Data_Value v2 = MainltData[i + 1];
                Data_Value v1 = MainltData[i];
                if (v2.Data == v1.Data)
                {
                    v1.DataValue = (v1.DataValue + v2.DataValue) / 2;
                    MainltData.RemoveAt(i + 1);
                }

            }

            if (MainltData.Count > 0)//ltData标准品
            {
                for (int i = 0; i < MainltData.Count; i++)
                {

                    if (MainltData[i].Data == 0)
                    {
                        MainltData[i].Data = 0.0001;
                    }
                    if (MainltData[i].Data == 1)
                    {
                        MainltData[i].Data = 0.999999;
                    }
                    if (MainltData[i].DataValue == 0)
                    {
                        MainltData[i].DataValue = 0.0001;
                    }
                }
                for (int i = 0; i < MainltData.Count; i++)
                {
                    //对处理过的数据进行纠错
                    if (double.IsNaN(MainltData[i].Data) || double.IsNaN(MainltData[i].DataValue))
                    {
                        MessageBox.Show(getString("keywordText.FuncCalcError"), getString("keywordText.Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;

                    }
                }
                //将画定标曲线的点赋值给dt
                foreach (Data_Value dv in MainltData)
                {
                    dtMain.Rows.Add(dv.Data, dv.DataValue);
                }

                //计算定标曲线的系数
                //for (int i = 0; i < MainltData.Count; i++)
                //{

                er1.AddData(MainltData);
                er1.Fit();
                //}
                foreach (double par in er1._pars)
                {
                    if (double.IsNaN(par) || double.IsInfinity(par))
                    {
                        MessageBox.Show(getString("keywordText.RegressionCalcError"), getString("keywordText.Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }

            }
        //}
        #endregion
        #endregion
        noNeedMainScal:
            if (ScalCurve.Length == 0)
            {
                clearData();
            }
            else
            {
                #region 显示定标数据
                string[] curvePoints = ScalCurve[0]["Points"].ToString().Split(';');
                dgvScalingData.Rows.Clear();
                if (curvePoints.Length == 0)
                {
                    return;
                }
                for (int i = 0; i < curvePoints.Length; i++)
                {
                    if (curvePoints[i] == "")
                        continue;
                    //将每个定标点的浓度和RLU分开放到数组中
                    string[] pointsData = curvePoints[i].Split(',');
                    dgvScalingData.Rows.Add("S" + (i+1).ToString(), pointsData[0].Substring(1), pointsData[1].Substring(0, pointsData[1].IndexOf(")")));
                }
                #endregion
                if (dgvScalingData.Rows.Count == 2)
                {
                    //两点定标数据存储
                    DataTable tempdt = new DataTable();
                    tempdt.Columns.Add("consistence", typeof(float));
                    tempdt.Columns.Add("PMT", typeof(float));
                    for (int i = 0; i < dgvScalingData.Rows.Count; i++)
                    {
                        tempdt.Rows.Add(double.Parse(dgvScalingData[1, i].Value.ToString()), double.Parse(dgvScalingData[2, i].Value.ToString()));
                    }
                    dt = GetCorrectedPoints(tempdt, dtMain);
                    if (dt == null) return;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ltData.Add(new Data_Value()
                        {
                            Data = double.Parse(dt.Rows[i]["consistence"].ToString()),
                            DataValue = double.Parse(dt.Rows[i]["PMT"].ToString())
                        });
                    }
                }
                else
                {
                    int ScalingDataCount = 0;
                    for (int i = 0; i < dgvScalingData.Rows.Count; i++)
                    {
                        if (dgvScalingData[0, i].Value != null && dgvScalingData[1, i].Value != null)
                            ScalingDataCount++;
                    }
                    for (int j = 0; j < ScalingDataCount; j++)
                    {
                        ltData.Add(new Data_Value()
                        {
                            Data = double.Parse(dgvScalingData[1, j].Value.ToString()),
                            DataValue = double.Parse(dgvScalingData[2, j].Value.ToString())
                        });
                    }
                }
                ltData.Sort(new Data_ValueDataAsc());
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
                    for (int i = 0; i < ltData.Count; i++)
                    {
                        //对处理过的数据进行纠错
                        if (double.IsNaN(ltData[i].Data) || double.IsNaN(ltData[i].DataValue))
                        {
                            MessageBox.Show(getString("keywordText.FuncCalcError"), getString("keywordText.Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;

                        }
                    }
                    //将画定标曲线的点赋值给dt
                    foreach (Data_Value dv in ltData)
                    {
                        dt.Rows.Add(dv.Data, dv.DataValue);
                    }

                    //计算定标曲线的系数
                    //for (int i = 0; i < ltData.Count; i++)
                    //{
                    er.AddData(ltData);
                    er.Fit();
                    //}
                    foreach (double par in er._pars)
                    {
                        if (double.IsNaN(par) || double.IsInfinity(par))
                        {
                            MessageBox.Show(getString("keywordText.RegressionCalcError"), getString("keywordText.Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }
                    }
                    EquationCurve(er, dt, CalMode);
                }
            }
            //在definePanal1中画出定标曲线
            dc.paintEliseScaling(definePanal1, dt, chbShowMainCurve.Checked ? dtMain : new DataTable(), er.GetResult, er1.GetResult, "", 0);
        }
        /// <summary>
        /// 获取两点校正后的六个点坐标值（浓度与发光值）
        /// </summary>
        /// <param name="dtNew">两点校准的浓度与发光值信息表</param>
        /// <param name="dtOld">原始定标的浓度与发光值信息表</param>
        /// <returns>校正以后的定标信息表</returns>
        public DataTable GetCorrectedPoints(DataTable dtNew, DataTable dtOld)
        {
            double k, b, x, y, x1, y1;
            x = double.Parse(dtNew.Rows[0]["consistence"].ToString());
            x1 = double.Parse(dtNew.Rows[1]["consistence"].ToString());
            //注：当前默认校准点浓度为标准品C、E两点浓度，故不需要比对浓度，直接计算即可。
            //y为同浓度点的吸光度差值占原始浓度的百分比，即y等于新测试吸光度的值减去原始吸光度的值，而后再除以原始吸光度的值
            //y = (double.Parse(dtNew.Rows[0]["PMT"].ToString()) - double.Parse(dtOld.Rows[2]["PMT"].ToString())) / double.Parse(dtOld.Rows[2]["PMT"].ToString());
            //y1 = (double.Parse(dtNew.Rows[1]["PMT"].ToString()) - double.Parse(dtOld.Rows[4]["PMT"].ToString())) / double.Parse(dtOld.Rows[4]["PMT"].ToString());
            //modify 20181026 y
            if (dtOld.Rows.Count < 4) return null;
            y = double.Parse(dtNew.Rows[0]["PMT"].ToString()) / double.Parse(dtOld.Rows[2]["PMT"].ToString());
            y1 = double.Parse(dtNew.Rows[1]["PMT"].ToString()) / double.Parse(dtOld.Rows[4]["PMT"].ToString());

            k = (y1 - y) / (x1 - x);
            b = y - k * x;
            for (int i = 0; i < dtOld.Rows.Count; i++)
            {
                if (i == 2)
                {
                    dtOld.Rows[2]["PMT"] = dtNew.Rows[0]["PMT"].ToString();
                }
                else if (i == 4)
                {
                    dtOld.Rows[4]["PMT"] = dtNew.Rows[1]["PMT"].ToString();
                }
                else
                {
                    //除了C、E两点之外，其他点的PMT值为原始浓度乘以（1+吸光度变化百分比）注意（此百分比可正可负）
                    //dtOld.Rows[i]["PMT"] = double.Parse(dtOld.Rows[i]["PMT"].ToString()) * (1 + (k * double.Parse(dtOld.Rows[i]["consistence"].ToString()) + b));
                    //modify 20181026 y
                    dtOld.Rows[i]["PMT"] = double.Parse(dtOld.Rows[i]["PMT"].ToString()) * (k * double.Parse(dtOld.Rows[i]["consistence"].ToString()) + b);
                }
            }
            return dtOld;
        }
        /// <summary>
        /// 清空界面上的曲线和数据
        /// </summary>
        public void clearData()
        {
            dgvScalingData.Rows.Clear();
            definePanal1.BackgroundImage = null;
            lblEquation.Text = getString("keywordText.Formula");
            lblR.Text = getString("keywordText.CorrelationCoefficient");
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
            ////在definePanal1中画出定标曲线
            //dc.paintEliseScaling(definePanal1,dt, er.GetResult, "", 1);//mdscalingInfo.fittingResult.IsNullOrEmpty() ? "" : "R: " + Convert.ToDouble(mdscalingInfo.fittingResult).ToString("0.###")
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
                    case 2:
                        lblEquation.Text = getString("keywordText.Formula");
                        Furmula = getString("keywordText.Formula") + " y = (" + double.Parse(strpar[0]).ToString("0.00") + " - " + double.Parse(strpar[3]).ToString("0.00") +
                            ") / [1 + (x/" + double.Parse(strpar[2]).ToString("0.00") + ")^" + double.Parse(strpar[1]).ToString("0.00") + "] + "
                            + double.Parse(strpar[3]).ToString("0.00");
                        lblEquation.Text = Furmula;
                        lblR.Text = getString("keywordText.CorrelationCoefficient") + er.R2;
                        break;
                    case 1:
                        lblEquation.Text = getString("keywordText.Formula");
                        Furmula = getString("keywordText.Equation") + " y=" + double.Parse(strpar[0]).ToString("0.00") + "+" + double.Parse(strpar[1]).ToString("0.00")
                            + "/(1+exp(-(" + double.Parse(strpar[2]).ToString("0.00") + "+" + double.Parse(strpar[3]).ToString("0.00") + "*ln(x))))";
                        lblEquation.Text = Furmula;
                        lblR.Text = getString("keywordText.CorrelationCoefficient") + er.R2;
                        break;
                }
            }
            #endregion


        }
        #endregion
        /// <summary>
        /// 保存图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        private void fbtnQCQuery_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmQC"))
            {
                th.Abort(); //lyq add 20190828
                frmQC frmQC = new frmQC();
                frmQC.TopLevel = false;
                frmQC.Parent = this.Parent;
                frmQC.Show();
            }
            else
            {
                frmQC frmQC = (frmQC)Application.OpenForms["frmQC"];
                frmQC.BringToFront(); ;

            }
        }

        private void fbtnReturn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void chbShowMainCurve_CheckedChanged(object sender, EventArgs e)
        {
            dgvScalData_SelectionChanged(null, null);
        }
        private void frmScaling_FormClosed(object sender, FormClosedEventArgs e)
        {
            frmAddScaling.AddMainCurve -= new Action(RefreshUI);
        }

        private void definePanal1_MouseDown(object sender, MouseEventArgs e)
        {
            //鼠标右键显示保存图像的菜单
            if (e.Button == MouseButtons.Right)
            {
                Point p = new Point(e.X, e.Y);
                MenuCurve.Show(definePanal1, p);
            }
        }

        private void fbtnPrintCurve_MouseDown(object sender, MouseEventArgs e)
        {
            if (dgvScalData.CurrentRow == null)
            {
                return;
            }
            if (dgvScalData.CurrentRow.Cells["colIsScal"].Value.ToString() == "N")
            {
                frmMS.MessageShow(getString("keywordText.Calibration"), getString("keywordText.NoInputCurve"));
                return;
            }
            string ItemName = dgvScalData.CurrentRow.Cells["colItemName"].Value.ToString();
            //定标曲线浓度和吸光度数据
            DataTable dt = new DataTable();
            dt.Columns.Add("consistence", typeof(float));
            dt.Columns.Add("absorbency", typeof(float));
            //将画定标曲线的点赋值给dt
            foreach (Data_Value dv in ltData)
            {
                dt.Rows.Add(dv.Data, dv.DataValue);
            }
            reportCurve reportscaling = new reportCurve(dt, er, ItemName);
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
        private void fbtnSelectCurve_Click(object sender, EventArgs e)
        {
            //2018-07-21 zlx add
            frmHistoryScaling frmHS = new frmHistoryScaling();
            if (dgvScalData.CurrentRow == null)
            { return; }
            frmHS.tempItemName = dgvScalData.CurrentRow.Cells["colItemName"].Value.ToString();
            //2018-08-25 zlx add
            frmHS.RegentBatch = dgvScalData.CurrentRow.Cells["colRegentBatch"].Value.ToString();
            if (frmHS.ShowDialog() == DialogResult.OK)
            {
                RefreshUI();
                string ItemName = dgvScalData.CurrentRow.Cells["colItemName"].Value.ToString();
                string Batch = dgvScalData.CurrentRow.Cells["colRegentBatch"].Value.ToString();
                ShowScalCurve(ItemName, Batch);
            }
        }
        //lyq add 20190828
        bool bReset = true;
        private void resetIsReady(object sender, EventArgs e)
        {
            if (bReset && th.IsAlive)
            {
                bReset = false;
                timer1.Enabled = true;
                timer1.Interval = 500;
                fbtnAddMainCurve.Enabled = false; 
                fbtnSelectCurve.Enabled = false;
                fbtnPrintCurve.Enabled = false;
                fbtnReset.Enabled = false;
                chbShowMainCurve.Enabled = false;
            }
            if (!th.IsAlive)
            {
                bReset = true;
                timer1.Enabled = false;
                timer1.Stop();
                fbtnAddMainCurve.Enabled = true;
                fbtnSelectCurve.Enabled = true;
                fbtnPrintCurve.Enabled = true;
                fbtnReset.Enabled = true;
                chbShowMainCurve.Enabled = true;
            }
        }   
        private void FbtnReset_Click(object sender, EventArgs e)
        {
            if (bReset)
            {
                DataTable dttemp = dtScalInfo.Clone(); //lyq add 20191221
                dgvScalData.DataSource = dttemp;

                th.Abort();
                th = new Thread(new ParameterizedThreadStart((obj) =>
                {
                    RefreshUI();
                }));
                th.IsBackground = true;
                th.Start();
                resetIsReady(sender, e);
            }
        }
        private void Timer1_Tick_1(object sender, EventArgs e)
        {
            if (!th.IsAlive)
                resetIsReady(sender, e);
        }
        private string getString(string key)
        {
            ResourceManager resManager = new ResourceManager(typeof(frmScaling));
            return resManager.GetString(key);
        }
    }
}