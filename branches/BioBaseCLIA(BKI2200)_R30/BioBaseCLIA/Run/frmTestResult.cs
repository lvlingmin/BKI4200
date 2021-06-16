using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Maticsoft.DBUtility;
using BioBaseCLIA.DataQuery;
using Common;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.Resources;

namespace BioBaseCLIA.Run
{
    public partial class frmTestResult : frmParent
    {
        #region 变量和实例
        /// <summary>
        /// 实验结果表的model实例
        /// </summary>
        Model.tbAssayResult modelAssayResult = new Model.tbAssayResult();
        /// <summary>
        /// 实验结果表的bll实例
        /// </summary>
        BLL.tbAssayResult bllAssayResult = new BLL.tbAssayResult();
        /// <summary>
        /// 质控结果表的model实例
        /// </summary>
        Model.tbQCResult modelQCResult = new Model.tbQCResult();
        /// <summary>
        ///  质控结果表的bll实例
        /// </summary>
        BLL.tbQCResult bllQCResult = new BLL.tbQCResult();
        /// <summary>
        /// 定标结果表的model实例
        /// </summary>
        Model.tbScalingResult modelScalingResult = new Model.tbScalingResult();
        /// <summary>
        /// 定标结果表的bll实例
        /// </summary>
        BLL.tbScalingResult bllScalingResult = new BLL.tbScalingResult();
        /// <summary>
        /// 样本信息表的bll实例  2019-01-10 zlx add
        /// </summary>
        Model.tbSampleInfo modelSp = new Model.tbSampleInfo();
        /// <summary>
        /// 样本信息表表的bll实例 2019-01-10 zlx add
        /// </summary>
        BLL.tbSampleInfo bllsp = new BLL.tbSampleInfo();
        frmMessageShow frmMsgShow = new frmMessageShow();
        #endregion
        CalculateCurve.Calculater er = null;//2018-07-23 zlx add
        public static bool BRun = true;//2018-08-15 zlx add
        List<int> OverSubpos = new List<int>();//2018-10-17 
        /// <summary>
        /// 底物与管架配置文件地址 2018-10-17 zlx add
        /// </summary>
        string iniPathSubstrateTube = Directory.GetCurrentDirectory() + "\\SubstrateTube.ini";
        public frmTestResult()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            dgvResultData.AutoGenerateColumns = false;
        }

        private void frmTestResult_Load(object sender, EventArgs e)
        {
            //2018-08-15 zlx add
            
            if (BRun)
                btnLoadReagent.Enabled = btnLoadSample.Enabled = false;
            else
                btnLoadReagent.Enabled = btnLoadSample.Enabled = true;
            //2018-10-17 zlx add
            string ValidDate1 = OperateIniFile.ReadIniData("Substrate1", "ValidDate", "", iniPathSubstrateTube);//2018-10-17 zlx add
            //string ValidDate2 = OperateIniFile.ReadIniData("Substrate2", "ValidDate", "", iniPathSubstrateTube);//2018-10-17 zlx add
            if (ValidDate1!="" && Convert.ToDateTime(ValidDate1) < DateTime.Now.Date)
                OverSubpos.Add(1);
            //if (Convert.ToDateTime(ValidDate2) < DateTime.Now.Date)
            //    OverSubpos.Add(2);
            #region 界面控件及数据初始化
            //dgvResultData.Columns[0].Width = 60;
            //dgvResultData.Columns[1].Width = 120;
            //dgvResultData.Columns[2].Width = 60;
            //dgvResultData.Columns[3].Width = 100;
            //dgvResultData.Columns[4].Width = 100;
            //dgvResultData.Columns[5].Width = 100;
            //dgvResultData.Columns[6].Width = 100;
            //dgvResultData.Columns[7].Width = 100;
            #endregion
            if (frmWorkList.ITestResult == null)
                return;
             
            //绑定工作列表界面得出的实验结果
            //dgvResultData.Rows.Add(1, "TestID", "SampleNo", "1", "1", "ItemName", 4470, 1.5, "Result", "0-1000", "0-10","","","","","ul");
            dgvResultData.DataSource = frmWorkList.BTestResult;
            GetDataGridColor(); //2018-08-18 zlx add
        }
        public void GetDataGridColor()
        {
          
            for (int i = 0; i < dgvResultData.Rows.Count; i++)
            {
                //2018-08-17
                if (Convert.ToInt32(dgvResultData.Rows[i].Cells["Status"].Value) == 1)
                {
                    object id = dgvResultData.Rows[i].Cells["SampleID"].Value;
                    dgvResultData.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                }
                //2018-08-18
                DataRow[] dr = dtRgInfo.Select("Batch='" + dgvResultData.Rows[i].Cells["ReagentBeach"].Value + "'");
                if (dr.Length > 0)
                {
                    if (Convert.ToDateTime(dr[0]["ValidDate"]) < Convert.ToDateTime(dgvResultData.Rows[i].Cells["ResultDatetime"].Value))
                    {
                        dgvResultData.Rows[i].Cells["ItemName"].Style.BackColor = Color.Pink;
                    }
                }
                //2018-10-17 zlx add
                if (OverSubpos.Contains(Convert.ToInt32(dgvResultData.Rows[i].Cells["SubstratePipe"].Value)))
                    dgvResultData.Rows[i].Cells["Result"].Style.BackColor = Color.Gray;
                //if (OverSubpos.Contains(Convert.ToInt32(dgvResultData.Rows[i].Cells["SubstratePipe"].Value)))
                //    dgvResultData.Rows[i].Cells["Result"].Style.BackColor = Color.Green;
            }
        }
        private void frmTestResult_SizeChanged(object sender, EventArgs e)
        {
            formSizeChange(this);
        }
        /// <summary>
        /// 将BindingList类型转换为IList类型
        /// </summary>
        /// <returns>返回IList<TestResult>的值</returns>
        private IList<TestResult> BindingListToList()
        {
            IList<TestResult> list = new List<TestResult>((BindingList<TestResult>)this.dgvResultData.DataSource);
            return list;

        }
        #region 界面其他功能按钮
       
        private void fbtnSaveResult_Click(object sender, EventArgs e)
        {
            List<TestResult> listResult = BindingListToList().ToList();

            ////lyq add 20190831
            //for (int i = 0; i < listResult.Count; i++)
            //{
            //    if (listResult[i].SampleType.Contains("交叉污染"))
            //    {
            //        MessageBox.Show("交叉污染不支持保存到数据库");
            //        return;
            //    }                    
            //}

            for (int i = 0; i < listResult.Count; i++)
            {
                //已经保存过该项数据
                if (lisSavedId.Count != 0 && lisSavedId.Exists(ty=>ty==listResult[i].TestID))
                {
                    continue;
                }
              
                //存储质控实验结果到数据库
                if (listResult[i].SampleType.Contains(getString("keywordText.ControlPlasmids")))
                {
                    string QCLevel;
                    if (listResult[i].SampleType == getString("keywordText.ControlHigh"))
                    {
                        QCLevel = "0";
                    }
                    else if (listResult[i].SampleType == getString("keywordText.ControlMiddle"))
                    {
                        QCLevel = "1";
                    }
                    else
                    {
                        QCLevel = "2";
                    }
                    DbHelperOleDb db = new DbHelperOleDb(3);
                    DataTable dtQCInfo = DbHelperOleDb.Query(3,@"select QCID,Batch,QCLevel from tbQC where status = '1' and ProjectName = '"
                                                                + listResult[i].ItemName + "'and QCLevel = '" + QCLevel + "' and Status = '1'").Tables[0];
                    if (dtQCInfo == null || dtQCInfo.Rows.Count == 0)
                    {
                        frmMsgShow.MessageShow(getString("keywordText.ExperimentalResult"), getString("keywordText.NotFindInformationOfControl"));
                        return;
                    }
                    modelQCResult.Batch = dtQCInfo.Rows[0][1].ToString();
                    modelQCResult.Concentration = double.Parse(listResult[i].concentration);
                    modelQCResult.ConcLevel = int.Parse(dtQCInfo.Rows[0][2].ToString());
                    modelQCResult.ConcSPEC = "";
                    modelQCResult.ItemName = listResult[i].ItemName;
                    modelQCResult.PMTCounter = listResult[i].PMT;
                    modelQCResult.QCID = int.Parse(dtQCInfo.Rows[0][0].ToString());
                    modelQCResult.Source = 0;
                    modelQCResult.TestDate = DateTime.Now;
                    modelQCResult.Unit = "";
                    db = new DbHelperOleDb(1);
                    bllQCResult.Add(modelQCResult);
                }
                //存储样本实验结果到数据库
                else
                {
                    modelAssayResult.SampleID = listResult[i].SampleID;
                    modelAssayResult.Batch = "";
                    if (listResult[i].concentration == "")
                    {
                        modelAssayResult.Concentration = 0;
                    }
                    else
                    {
                        modelAssayResult.Concentration = double.Parse(listResult[i].concentration);
                    }
                    modelAssayResult.ConcSpec = "";
                    modelAssayResult.DiluteCount = 0;
                    modelAssayResult.ItemName = listResult[i].ItemName;
                    modelAssayResult.PMTCounter = listResult[i].PMT;
                    modelAssayResult.Range = listResult[i].Range1;
                    modelAssayResult.Result = listResult[i].Result;
                    modelAssayResult.Specification = "";
                    modelAssayResult.Status = listResult[i].Status;//2018-07-17 zlx mod
                    modelAssayResult.TestDate = DateTime.Now;
                    modelAssayResult.Unit = listResult[i].Unit;//2018-11-10 zlx mod
                    modelAssayResult.Upload = "";
                    modelAssayResult.Batch = listResult[i].ReagentBeach;//2018-08-18 
                    DbHelperOleDb db = new DbHelperOleDb(1);
                    bllAssayResult.Add(modelAssayResult);
                }
                lisSavedId.Add(listResult[i].TestID);
            }
            //存储定标结果
            SaveStandardResult(listResult);
            #region 存储定性实验定标液结果
            //查询已经装载的项目
            DbHelperOleDb db1 = new DbHelperOleDb(3);
            //2018-08-28 zlx mod
            DataTable UsingItem = DbHelperOleDb.Query(3, @"select ReagentName,Batch from tbReagent where Status = '正常'").Tables[0];
            //DataTable UsingItem = DbHelperOleDb.Query(3,@"select ReagentName,Batch from tbReagent where Status = '"+ getString("keywordText.Normal") +"'").Tables[0];
            if (UsingItem == null || UsingItem.Rows.Count == 0)
            {
                frmMsgShow.MessageShow(getString("keywordText.ExperimentalResult"), getString("keywordText.HaveNotBeUsedItem"));
                return;
            }
            //2018-08-28 zlx mod
            DataTable UsingItemNew = FilterDT(UsingItem, 0,1);
            for (int j = 0; j < UsingItemNew.Rows.Count; j++)
            {
                //取出定性实验的定标液结果
                List<TestResult> lisCutoffResult = listResult.FindAll(ty => ty.ItemName == UsingItemNew.Rows[j][0].ToString()
                                                                          && ty.SampleType.Contains(getString("keywordText.CalibrationSolution")));

                if (lisCutoffResult.Count == 0)
                {
                    continue;
                }
                //计算参数
               db1 = new DbHelperOleDb(0); 
                double calulatePara = double.Parse(DbHelperOleDb.GetSingle(0,@"select CalculateMethod from tbProject where ShortName = '"
                                                                            + lisCutoffResult[0].ItemName + "'").ToString()) / 100;
                //项目cutoff值
                double cutoffValue = 0;
                ///吸光度和
                double sumPMT = 0;
                //吸光度均值
                double avgPMT = 0;
                for (int i = 0; i < lisCutoffResult.Count; i++)
                {
                    sumPMT += lisCutoffResult[i].PMT;
                }
                avgPMT = sumPMT / lisCutoffResult.Count;
                cutoffValue = avgPMT * calulatePara;
                //查询当前项目试剂批号
                db1 = new DbHelperOleDb(3); 
                string ReagentBatch = DbHelperOleDb.GetSingle(3,@"select Batch from tbReagent where ReagentName = '"
                                                              + UsingItemNew.Rows[j][0].ToString() + "'").ToString();
                db1 = new DbHelperOleDb(1); 
                //该项目的历史定标
                int sameScaling = int.Parse(DbHelperOleDb.GetSingle(1,@"select count(*) from tbScalingResult where ItemName = '"
                                                           + lisCutoffResult[0].ItemName + "'").ToString());
                //将已有定标的状态设为非正在使用的状态
                if (sameScaling > 0)
                {
                    db1 = new DbHelperOleDb(1); 
                    DbHelperOleDb.ExecuteSql(1,@"update tbScalingResult set Status=0 where ItemName = '"
                                                           + lisCutoffResult[0].ItemName + "'").ToString();
                }

                modelScalingResult.ActiveDate = DateTime.Now;
                modelScalingResult.ItemName = lisCutoffResult[0].ItemName;
                modelScalingResult.PointCount = 1;
                modelScalingResult.Points = cutoffValue.ToString();
                modelScalingResult.RegentBatch = ReagentBatch;
                modelScalingResult.ScalingModel = null;
                modelScalingResult.Source = 1;
                modelScalingResult.Status = 1;
                bllScalingResult.Add(modelScalingResult);
            }
            #endregion
            //2018-07-24 zlx add
           frmMsgShow.MessageShow(getString("keywordText.SaveResult"), getString("keywordText.SavedSuccessfully"));
        }

        /// <summary>
        /// 保存定标结果
        /// </summary>
        /// <param name="IlistResult"></param>
        public void SaveStandardResult(List<TestResult> listResult)
        {
            //查询已经装载的项目
            DbHelperOleDb db1 = new DbHelperOleDb(3);
            DataTable UsingItem = DbHelperOleDb.Query(3, @"select ReagentName,Batch from tbReagent where Status ='正常'").Tables[0];
            //DataTable UsingItem = DbHelperOleDb.Query(3,@"select ReagentName,Batch from tbReagent where Status ='"+getString("keywordText.Normal")+"'").Tables[0];
            if (UsingItem == null || UsingItem.Rows.Count == 0)
            {
                frmMsgShow.MessageShow(getString("keywordText.ExperimentalResult"), getString("keywordText.HaveNotBeUsedItem"));
                return;
            }
            //2018-08-28 zlx mod
            DataTable UsingItemNew = FilterDT(UsingItem, 0,1);
            for (int j = 0; j < UsingItemNew.Rows.Count; j++)
            {
                //取出实验结果中相同项目名称的标准品
                List<TestResult> FilistStandardResult = listResult.FindAll(ty => ty.ItemName == UsingItemNew.Rows[j][0].ToString()
                                                                          && ty.SampleType.Contains(getString("keywordText.StandardPlasmids")) && ty.ReagentBeach == UsingItemNew.Rows[j][1].ToString());
                if (FilistStandardResult.Count == 0)
                {
                    continue;
                }
                List<TestResult> ilistStandardResult = new List<TestResult>(FilistStandardResult.ToArray());
                db1 = new DbHelperOleDb(0);
                //取出项目中试剂标准品的浓度点 2018-08-27 
                string calpointsConc = DbHelperOleDb.GetSingle(0,@"select CalPointConc from tbProject where ShortName = '"
                                                               + ilistStandardResult[0].ItemName + "'").ToString();
                //将项目的浓度放在数组中
                string[] pointConc = calpointsConc.Split(',');

                StringBuilder points = new StringBuilder();
                int PointsNum = pointConc.Length;

                //this block add by y in 2018/04/28
                //对相同浓度的标准品取均值,然后移除相同浓度的重复标准品
                //string ReagentBatch = ilistStandardResult[0].ReagentBeach;//2018-08-25 zlx add
                #region 计算结果
                /*
                List<TestResult> T1 = new List<TestResult>(ilistStandardResult);
                ilistStandardResult.Clear();
                List<TestResult> T2 = new List<TestResult>();
                while (T1.Count != 0)
                {
                    TestResult temp = T1[T1.Count - 1];
                    long avg = temp.PMT;
                    int division = 1;
                    //T1.RemoveAt(T1.Count - 1);
                    for (int i = T1.Count - 1; i >= 0; i--)
                    {
                        if (T1[i].SampleType == temp.SampleType)//原本是.SamplePos改为浓度.concentration，现用样本类型.SampleType
                        {
                            avg += T1[i].PMT;
                            division++;
                            T1.RemoveAt(i);
                        }
                    }
                    temp.PMT = Convert.ToInt32(avg / division);
                    ilistStandardResult.Add(temp);
                }
                //this block end

                //this block add by y in 2018/04/29
                //对标准品按照SampleType进行排序
                T1.Clear();
                T1.AddRange(ilistStandardResult);
                T1.Reverse();
                */
               
                List<TestResult> T1 = new List<TestResult>();
                foreach (TestResult result in ilistStandardResult)
                {
                    if (T1.FindAll(ty => ty.SampleType == result.SampleType).Count > 0)
                        continue;
                    List<TestResult> T2 = ilistStandardResult.FindAll(ty => ty.SampleType == result.SampleType);
                    long avg = 0;
                    int division = 0;
                    TestResult temp = new TestResult();
                    temp = (TestResult)result.Clone();
                    foreach (TestResult tresult in T2)
                    {
                        avg += tresult.PMT;
                        division++;
                    }
                    temp.PMT = Convert.ToInt32(avg / division);
                    T1.Add(temp);
                }
                #endregion
                //ilistStandardResult.Clear();
                int[] posit = new int[T1.Count];
                for(int i = 0;i<T1.Count;i++)
                {
                    posit[i] = ((int)Convert.ToChar( T1[i].SampleType.Substring(T1[i].SampleType.Length-1)))-65;//根据样品类型中的字母ascii码判断该条应该在的行
                }
                int a =0,b=posit[0];
                while (b != 100)
                {
                    for (int i = 0; i < T1.Count; i++)
                    {
                        if (posit[i] < b)
                        {
                            b = posit[i];
                            a = i;
                        }
                    }
                    if (b == 100)
                        break;
                    posit[a] = 100;
                    ilistStandardResult.Add(T1[a]);
                    b = 101;
                }
                bool hasWrongPointer = false;
                bool hasPointerC = false, hasPoniterE = false;
                foreach (var item in ilistStandardResult)
                {
                    if (item.SampleType == getString("keywordText.StandardC")) hasPointerC = true;
                    if (item.SampleType == getString("keywordText.StandardE")) hasPoniterE = true;
                }
                if (!hasPoniterE && !hasPointerC)
                {
                    hasWrongPointer = true;
                }
                //计算实验的标准品浓度种类、数量是否满足对应实验项目的要求
                if (hasWrongPointer)//PointsNum > ilistStandardResult.Count &&
                {
                    frmMsgShow.MessageShow(getString("keywordText.ExperimentalResult"), getString("keywordText.InsufficientStandardData"));
                    continue;
                }
                else
                {
                    //for (int i = ilistStandardResult.Count - 2; i >= 0; i--)//decide not use by y
                    //{
                    //    TestResult T22 = ilistStandardResult[i + 1];
                    //    TestResult T11 = ilistStandardResult[i];
                    //    if (T22.SamplePos == T11.SamplePos)
                    //    {
                    //        T11.PMT = (T11.PMT + T22.PMT) / 2;
                    //        ilistStandardResult.RemoveAt(i + 1);
                    //    }
                    //}//对相同浓度的标准品两两取均值

                    //浓度值赋值
                    string[] tempConc = new string[7];
                    for (int i = 0; i < ilistStandardResult.Count; i++)
                    {
                        if (ilistStandardResult[i].SampleType == getString("keywordText.StandardA"))
                        {
                            //points.Append("(" + pointConc[i] + "," + ilistStandardResult[i].PMT + ")");
                            tempConc[0] = "(" + pointConc[0] + "," + ilistStandardResult[i].PMT + ")";
                        }
                        else if (ilistStandardResult[i].SampleType == getString("keywordText.StandardB"))
                        {
                            //points.Append("(" + pointConc[i] + "," + ilistStandardResult[i].PMT + ")");
                            tempConc[1] = "(" + pointConc[1] + "," + ilistStandardResult[i].PMT + ")";
                        }
                        else if (ilistStandardResult[i].SampleType == getString("keywordText.StandardC"))
                        {
                            //points.Append(";(" + pointConc[i] + "," + ilistStandardResult[i].PMT + ")");
                            tempConc[2] = "(" + pointConc[2] + "," + ilistStandardResult[i].PMT + ")";
                        }
                        else if (ilistStandardResult[i].SampleType == getString("keywordText.StandardD"))
                        {
                            //points.Append(";(" + pointConc[i] + "," + ilistStandardResult[i].PMT + ")");
                            tempConc[3] = "(" + pointConc[3] + "," + ilistStandardResult[i].PMT + ")";
                        }
                        else if (ilistStandardResult[i].SampleType == getString("keywordText.StandardE"))
                        {
                            //points.Append(";(" + pointConc[i] + "," + ilistStandardResult[i].PMT + ")");
                            tempConc[4] = "(" + pointConc[4] + "," + ilistStandardResult[i].PMT + ")";
                        }
                        else if (ilistStandardResult[i].SampleType == getString("keywordText.StandardF"))
                        {
                            //points.Append(";(" + pointConc[i] + "," + ilistStandardResult[i].PMT + ")");
                            tempConc[5] = "(" + pointConc[5] + "," + ilistStandardResult[i].PMT + ")";
                        }
                        else if (ilistStandardResult[i].SampleType == getString("keywordText.StandardG"))
                        {
                            tempConc[6] = "(" + pointConc[6] + "," + ilistStandardResult[i].PMT + ")";
                        }
                    }
                    int pointerNumber = 0;
                    for (int i = 0; i < 7; i++)
                    {
                        if (tempConc[i] != null && tempConc[i] != "")
                        {
                            points.Append(tempConc[i] + ";");
                            pointerNumber++;
                        }
                    }
                    points.Remove(points.Length - 1, 1);
                    //db1 = new DbHelperOleDb(3);
                    ////查询当前项目试剂批号 2018-08-27 zlx mod
                    //string ReagentBatch = DbHelperOleDb.GetSingle(@"select Batch from tbReagent where ReagentName = '"
                    //                                              + UsingItemNew.Rows[j][0].ToString() + "' AND RegentBatch='" + ilistStandardResult[0].ReagentBeach + "'").ToString();
                    string ReagentBatch = ilistStandardResult[0].ReagentBeach;
                    db1 = new DbHelperOleDb(0);
                    //查询当前项目的定标方法 2018-11-24 zlx mod
                    object ScalingModel = DbHelperOleDb.GetSingle(0,@"select CalMethod from tbProject where ShortName = '"
                                                               + ilistStandardResult[0].ItemName + "' ");
                    if (ScalingModel == null)
                        ScalingModel = ilistStandardResult.Count;
                    bool ExitsMainCurve = false;
                    if (pointerNumber < PointsNum)
                    {
                        db1 = new DbHelperOleDb(1);
                        ExitsMainCurve = new BLL.tbMainScalCurve().ExistsCurve(ilistStandardResult[0].ItemName, ilistStandardResult[0].ReagentBeach);
                    }
                   
                    //该项目的历史定标  2018-08-27 zlx mod
                    if (ExitsMainCurve || pointerNumber == PointsNum)
                    {
                        db1 = new DbHelperOleDb(1);
                        int sameScaling = int.Parse(DbHelperOleDb.GetSingle(1,@"select count(*) from tbScalingResult where ItemName = '"
                                                                   + ilistStandardResult[0].ItemName + "' AND RegentBatch='" + ilistStandardResult[0].ReagentBeach + "'").ToString());
                        //将已有定标的状态设为非正在使用的状态
                        if (sameScaling > 0)
                        {
                            db1 = new DbHelperOleDb(1);//2018-09-05 zlx add
                            //2018-08-27 zlx mod
                            DbHelperOleDb.ExecuteSql(1,@"update tbScalingResult set Status=0 where ItemName = '"
                                                                   + ilistStandardResult[0].ItemName + "' AND RegentBatch='" + ilistStandardResult[0].ReagentBeach + "'").ToString();
                        }
                        modelScalingResult.Status = 1;
                    }
                    else
                    {
                        modelScalingResult.Status = 0;
                    }

                    modelScalingResult.ActiveDate = DateTime.Now;
                    modelScalingResult.ItemName = ilistStandardResult[0].ItemName;
                    modelScalingResult.PointCount = pointerNumber;
                    modelScalingResult.Points = points.ToString();
                    modelScalingResult.RegentBatch = ReagentBatch;
                    modelScalingResult.ScalingModel = int.Parse(ScalingModel.ToString());//2018-11-24 zlx mod
                    modelScalingResult.Source = 1;

                    db1 = new DbHelperOleDb(1);//2018-09-05
                    bool saveresult = bllScalingResult.Add(modelScalingResult);
                   

                }

            }//for cycle end

        }


        /// <summary>
        /// 移除datatable中同一列中相同的元素 2018-08-28 zlx mod
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public DataTable FilterDT(DataTable dt, int i,int ii)//i表示判断第几列重复
        {
            DataTable _dtClone=dt.Clone();
            //List<string[dt.Rows.Count]> key = new List<string[dt.Rows.Count]>();//唯一键值列表

            for (int j = 0; j < dt.Rows.Count; j++)
            {
                string temp = dt.Rows[j][i].ToString();
                string beach= dt.Rows[j][ii].ToString();
                if (_dtClone.Select("ReagentName='" + temp + "'AND Batch='" + beach + "'").Length > 0)
                {
                    dt.Rows.Remove(dt.Rows[j]);//移除该行
                    j = j - 1;
                }
                else
                {
                    _dtClone.Rows.Add(temp, beach);
                }
                //if (key.Count(x => (x == temp &&x == beach))> 0)//有重复
                //{
                //    dt.Rows.Remove(dt.Rows[j]);//移除该行
                //    j = j - 1;
                //}
                //else
                //{
                //    key.Add(temp);//重复键值添加到唯一列表
                //}
            }
            return dt;
        }
        private void fbtnExportData_Click(object sender, EventArgs e)
        {
            if (dgvResultData.Rows.Count == 0)
            {
                frmMessageShow fmessage = new frmMessageShow();
                fmessage.MessageShow(getString("keywordText.TipsOfExport"), getString("keywordText.CanNotExport"));
                return;
            }
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "xls"+getString("keywordText.File") +"|*.xls";
            string FileName = dialog.FileName = DateTime.Now.ToString("yyyyMMdd") + "_" + getString("keywordText.ExperimentalResult");
            IWorkbook excel = new HSSFWorkbook();//创建.xls文件
            ISheet sheet = excel.CreateSheet(FileName); //创建sheet
            if (dialog.ShowDialog() != DialogResult.OK)
                return;
            else
            {
                IRow row = sheet.CreateRow(0);//创建行对象，填充表头
                for (int i = 0; i < dgvResultData.Columns.Count - 1; i++)
                {
                    if (dgvResultData.Columns[i].Visible == false)
                    {
                        continue;
                    }
                    row.CreateCell(i - 1).SetCellValue(dgvResultData.Columns[i].HeaderText);
                    LogFile.Instance.Write(dgvResultData.Columns[i].HeaderText);
                    //sheet.AutoSizeColumn(i);
                }
                //填充内容，j从1开始，屏蔽掉第一列，循环读取
                for (int i = 0; i < dgvResultData.Rows.Count; i++)
                {
                    row = sheet.CreateRow(i + 1);
                    for (int j = 1; j < dgvResultData.Columns.Count - 1; j++)
                    {
                        if (dgvResultData.Columns[j].Visible == false)
                        {
                            continue;
                        }
                        #region 导出 lyq 20190812 add 
                        if (dgvResultData.Columns[j].HeaderText.ToString().Equals("PMT"))
                        {
                            row.CreateCell(j - 1).SetCellValue(double.Parse(dgvResultData.Rows[i].Cells[j].Value.ToString()));
                        }
                        else
                        {
                            row.CreateCell(j - 1).SetCellValue(dgvResultData.Rows[i].Cells[j].Value.ToString());
                        }
                        #endregion
                        //row.CreateCell(j - 1).SetCellValue(dgvResultData.Rows[i].Cells[j].Value.ToString());
                        // sheet.AutoSizeColumn(j - 1);//列宽自适应
                    }
                }
            }
            try
            {
                FileStream file = new FileStream(dialog.FileName.ToString(), FileMode.Create);
                excel.Write(file);
                file.Close();
            }
            catch (Exception exc)
            {
                frmMessageShow fmessage = new frmMessageShow();
                fmessage.MessageShow(getString("keywordText.TipsOfExport"), getString("keywordText.ExportFailedMaybeInOperation"));
                return;
            }
            frmMessageShow frmessage = new frmMessageShow();
            frmessage.MessageShow(getString("keywordText.TipsOfExport"), getString("keywordText.ExportedSuccessfully"));
        }


        private void fbtnPrint_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region 界面右侧按钮
        private void fbtnReturn_Click(object sender, EventArgs e)
        {
            if (CheckFormIsOpen("frmWorkList"))
            {
                frmWorkList frmWL = (frmWorkList)Application.OpenForms["frmWorkList"];
                frmWL.Show();
                frmWL.BringToFront();
                return;
            }
            this.Close();
        }

        private void btnLoadSample_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmSampleLoad"))
            {
                frmSampleLoad frmSL = new frmSampleLoad();
                frmSL.TopLevel = false;
                frmSL.Parent = this.Parent;
                frmSL.Show();
            }
            else
            {
                frmSampleLoad frmSL = (frmSampleLoad)Application.OpenForms["frmSampleLoad"];
                frmSL.Show();
                frmSL.LoadData();//2018-01-10 zlx add
                frmSL.BringToFront(); ;
            }
            if (CheckFormIsOpen("frmWorkList") && (frmWorkList.RunFlag == (int)RunFlagStart.Stoped || frmWorkList.RunFlag == (int)RunFlagStart.NoStart))//2019-01-11 zlx add
            {
                frmWorkList frmWL = (frmWorkList)Application.OpenForms["frmWorkList"];
                frmWL.Close();
            }
        }

        private void btnWorkList_Click(object sender, EventArgs e)
        {

            if (!CheckFormIsOpen("frmWorkList"))
            {
                frmWorkList frmWL = new frmWorkList();
                frmWL.TopLevel = false;
                frmWL.Parent = this.Parent;
                frmWL.Show();
            }
            else
            {
                frmWorkList frmWL = (frmWorkList)Application.OpenForms["frmWorkList"];
                frmWL.Show();
                frmWL.BringToFront(); 

            }
        }

        private void btnLoadReagent_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmReagentLoad"))
            {
                frmReagentLoad frmRL = new frmReagentLoad();
                frmRL.TopLevel = false;
                frmRL.Parent = this.Parent;
                frmRL.Show();
            }
            else
            {
                frmReagentLoad frmRL = (frmReagentLoad)Application.OpenForms["frmReagentLoad"];
                frmRL.Show();
                frmRL.BringToFront(); ;
                frmRL.LoadData();//2018-11-14 zlx add
            }
            if (CheckFormIsOpen("frmWorkList") && (frmWorkList.RunFlag == (int)RunFlagStart.Stoped || frmWorkList.RunFlag == (int)RunFlagStart.NoStart))//2019-01-11 zlx add
            {
                frmWorkList frmWL = (frmWorkList)Application.OpenForms["frmWorkList"];
                frmWL.Close();
            }
        }

        #endregion

        //2018-07-21 zlx add 
        private void fbtnRSelectCurve_Click(object sender, EventArgs e)
        {
            frmHistoryScaling frmHS = new frmHistoryScaling();
            if (dgvResultData.CurrentRow == null)
            { return; }
            frmHS.tempItemName = dgvResultData.CurrentRow.Cells["ItemName"].Value.ToString();
            if (frmHS.ShowDialog() == DialogResult.OK)
                er = frmHS.Caler;
        }

        private void fbtnRCalculatResult_Click(object sender, EventArgs e)
        {
            //2018-07-23 zlx add
            if (dgvResultData.CurrentRow == null)
            { return; };
            if (er == null)
            {
                return;
            }
            else
            {
                for (int i = 0; i < dgvResultData.SelectedRows.Count; i++)
                {
                    string concentration = er.GetResultInverse(Convert.ToDouble(
                   dgvResultData.SelectedRows[i].Cells[6].Value.ToString())).ToString("0.###");
                    if (concentration == getString("keywordText.NonNumeric"))
                    {
                        dgvResultData.SelectedRows[i].Cells[7].Value = 0;
                    }
                    else
                    {
                        dgvResultData.SelectedRows[i].Cells[7].Value = concentration;
                    }
                }
            }
        }

        //2018-08-15 zlx add
        private void dgvResultData_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
           
            if (!BRun)
                btnLoadReagent.Enabled = btnLoadSample.Enabled = true;
            else
                btnLoadReagent.Enabled = btnLoadSample.Enabled = false;
            
            //2018-08-17 zlx add
            if (Convert.ToInt32(dgvResultData.Rows[dgvResultData.Rows.Count - 1].Cells["Status"].Value) == 1)
                dgvResultData.Rows[dgvResultData.Rows.Count - 1].DefaultCellStyle.BackColor = Color.Yellow;
            //2018-10-17 zlx add
            if (OverSubpos.Contains(Convert.ToInt32(dgvResultData.Rows[dgvResultData.Rows.Count - 1].Cells["SubstratePipe"].Value)))
                dgvResultData.Rows[dgvResultData.Rows.Count - 1].Cells["Result"].Style.BackColor = Color.Gray;
            //2018-08-18 zlx add
            DataRow[] dr = dtRgInfo.Select("Batch='" + dgvResultData.Rows[dgvResultData.Rows.Count - 1].Cells["ReagentBeach"].Value + "'");
            if (dr.Length > 0)
            {
                if (Convert.ToDateTime(dr[0]["ValidDate"]) < Convert.ToDateTime(dgvResultData.Rows[dgvResultData.Rows.Count - 1].Cells["ResultDatetime"].Value))
                {
                    dgvResultData.Rows[dgvResultData.Rows.Count - 1].Cells["ItemName"].Style.BackColor = Color.Pink;
                }
            }
             
        }

        //2018-01-10 zlx add
        private void fbtnTestAgain_Click(object sender, EventArgs e)
        {
            if (dgvResultData.SelectedRows.Count == 0||frmWorkList.RunFlag==(int)RunFlagStart.IsRuning) return;

            if (dgvResultData.SelectedRows[0].Cells["SampleType"].Value.ToString().Contains(getString("keywordText.Standard"))||
                dgvResultData.SelectedRows[0].Cells["SampleType"].Value.ToString().Contains(getString("keywordText.Calibrator"))||
                dgvResultData.SelectedRows[0].Cells["SampleType"].Value.ToString().Contains(getString("keywordText.Control"))) 
            {
                MessageBox.Show(getString("keywordText.NotSerumAndPleaseAddManually"),getString("keywordText.Tips"), MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }

                bool _boolAgain = false;
            DbHelperOleDb db = new DbHelperOleDb(1);
            DataTable _dtSP = bllsp.GetList("SampleID=" + dgvResultData.SelectedRows[0].Cells["SampleID"].Value + "").Tables[0];
            if (_dtSP.Rows.Count > 0)
            {
                if (Convert.ToInt32(_dtSP.Rows[0]["Status"]) == 1)
                {
                    for (int i = 0; i < dtSpInfo.Rows.Count; i++)
                    {
                        if (dtSpInfo.Rows[i]["SampleNo"].ToString().Equals(_dtSP.Rows[0]["SampleNo"].ToString()))
                        {
                            if (DbHelperOleDb.ExecuteSql(1,@"update tbSampleInfo set Status = 0  where SampleID=" + dgvResultData.SelectedRows[0].Cells["SampleID"].Value + "") > 0)
                            {
                                dtSpInfo.Rows[i]["Status"] = 0;
                                _dtSP.Rows[0]["Status"] = 0;
                            }
                        }
                    }
                }
                if (Convert.ToInt32(_dtSP.Rows[0]["Status"]) == 0)
                    _boolAgain = true;
            }
            int bUpdate = 0;
                if(_boolAgain)
                    bUpdate = DbHelperOleDb.ExecuteSql(1,@"update tbAssayResult set Status = -1  where SampleID=" + dgvResultData.SelectedRows[0].Cells["SampleID"].Value + " AND ItemName='" + dgvResultData.SelectedRows[0].Cells["ItemName"].Value + "'AND PMTCounter=" + dgvResultData.SelectedRows[0].Cells["PMT"].Value + "");
            if (bUpdate > 0)
                MessageBox.Show(getString("keywordText.SettingSuccessfully"));
        }

        private void functionButton1_Click(object sender, EventArgs e)
        {
            if (frmWorkList.RunFlag == (int)RunFlagStart.IsRuning)
            {
                MessageBox.Show(getString("keywordText.PleaseEmptyAfterTheExperiment"), getString("keywordText.Tips"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            frmWorkList.BTestResult.Clear();
        }
        private string getString(string key)
        {
            ResourceManager resManager = new ResourceManager(typeof(frmTestResult));
            return resManager.GetString(key).Replace(@"\n", "\n").Replace(@"\t", "\t");
        }
    }
}
