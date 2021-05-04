using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Maticsoft.DBUtility;
using BioBaseCLIA.ScalingQC;
using System.Text.RegularExpressions;
using DBUtility;
using Common;
using System.Resources;

namespace BioBaseCLIA.Run
{
    public partial class frmAddScaling : frmSmallParent
    {
        private DataTable dtConcValue = new DataTable();
        BLL.tbMainScalCurve bllmsc = new BLL.tbMainScalCurve();
        Model.tbMainScalCurve modelMainScalcurve = new Model.tbMainScalCurve();
        public frmMessageShow frmMsg = new frmMessageShow();
        BLL.tbProject bllprp = new BLL.tbProject();
        Model.tbProject modelProject = new Model.tbProject();
        /// <summary>
        /// 无焦点获取键盘事件钩子
        /// </summary>
        private BarCodeHook barCodeHook = new BarCodeHook();
        /// <summary>
        /// 存储实验流程 jun add 20190409
        /// </summary>
        private DataTable dtTestPro = new DataTable();
        /// <summary>
        /// 项目名称
        /// </summary>
        private string itemName = "";
        /// <summary>
        /// 试剂批号
        /// </summary>
        private string regentBatch = "";
        /// <summary>
        /// 激活日期
        /// </summary>
        private string activeDate = "";
        /// <summary>
        /// 定标点数
        /// </summary>
        private string calPointNumber = "";
        /// <summary>
        /// 有效期
        /// </summary>
        private string validDate = "";
        /// <summary>
        /// 项目更新确定按钮测试用
        /// </summary>
        private int mainCurveID = 0;
        bool haveItem = false;
        /// <summary>
        /// 添加主曲线事件
        /// </summary>
        public static event Action AddMainCurve;
        public frmAddScaling()
        {
            InitializeComponent();
            dtConcValue.Columns.Add("No", typeof(string));
            dtConcValue.Columns.Add("Conc", typeof(string));
            dtConcValue.Columns.Add("Value", typeof(string));

            //为实验流程列表增加source jun add 20190409
            dtTestPro.Columns.Add(getString("keywordText.Step"), typeof(string));
            dtTestPro.Columns.Add(getString("keywordText.Parameter"), typeof(string));
            dtTestPro.Columns.Add(getString("keywordText.Unit"), typeof(string));

        }
        public frmAddScaling(string name, string batch, string activedate, string validdate)//增加一个项目名称参数 jun add 20190409
        {
            InitializeComponent();
            dtConcValue.Columns.Add("No", typeof(string));
            dtConcValue.Columns.Add("Conc", typeof(string));
            dtConcValue.Columns.Add("Value", typeof(string));
            itemName = name;
            regentBatch = batch;
            activeDate = activedate;
            validDate = validdate;

            //为实验流程列表增加source jun add 20190409
            dtTestPro.Columns.Add(getString("keywordText.Step"), typeof(string));
            dtTestPro.Columns.Add(getString("keywordText.Parameter"), typeof(string));
            dtTestPro.Columns.Add(getString("keywordText.Unit"), typeof(string));


        }
        private void frmAddScaling_Load(object sender, EventArgs e)
        {
            txtRegentBatch.Text = regentBatch;
            BLL.tbProject bllProject = new BLL.tbProject();
            DbHelperOleDb db = new DbHelperOleDb(0);
            DataTable dt = bllProject.GetList(" ShortName='" + itemName + "'").Tables[0];

            if (dt.Rows.Count < 1 || regentBatch=="")
                return;
            haveItem = true;
            //lyq add 20190814 根据几点定标，来生成几行
            string calPonitNum = dt.Rows[0]["CalPointNumber"].ToString();
            calPointNumber = calPonitNum;
            int calPN = Convert.ToInt32(calPonitNum);
            for (int i = 0; i < calPN; i++)
            {
                dtConcValue.Rows.Add(i + 1, dt.Rows[0]["CalPointConc"].ToString().Split(',')[i], "");
            }

            //2018-11-14 zlx mod
            db = new DbHelperOleDb(1);
            dt = bllmsc.GetList(" ItemName = '" + itemName + "' and RegentBatch = '" + regentBatch + "'").Tables[0];
            if (dt.Rows.Count > 0)
            {
                string[] splitPoint = dt.Rows[0]["Points"].ToString().Replace("(", "").Replace(")", "").Split(';');//主曲线表的 浓度，发光值
                int ig = 0;
                foreach (string pointinfo in splitPoint)
                {
                    string[] point = pointinfo.Split(',');  //浓度和发光值   
                    //lyq mod 20190820
                    dtConcValue.Rows[ig][1] = point[0];
                    dtConcValue.Rows[ig][2] = point[1];
                    ig++;
                    if (ig == splitPoint.Length - 1)
                            break;
                    
                    //for (int i = 0; i < dtConcValue.Rows.Count; i++)
                    //{
                    //    if (dtConcValue.Rows[i][1].ToString() == point[0].ToString())
                    //    {
                    //        dtConcValue.Rows[i][2] = point[1];
                    //        //continue;
                    //    }
                    //}
                }
            }
            dgvScaling.DataSource = dtConcValue;

            //将原有流程查询并展示  jun add 20190409
            tbxTestPro.Text = itemName;
            string sql = "select ProjectProcedure from tbProject where ShortName = '" + itemName + "'";
            DbHelperOleDb db0 = new DbHelperOleDb(0);
            DataTable dtTemp = DbHelperOleDb.Query(0,sql).Tables[0];
            string[] steps = dtTemp.Rows[0][0].ToString().Split(';');
            if (steps.Length < 1 || steps[0] == "")
            {
                frmMessageShow frm = new frmMessageShow();
                frm.MessageShow(getString("keywordText.Tips"), getString("keywordText.TipsNone"));
                return;
            }
            foreach (string step in steps)
            {
                string[] s = step.Split('-');
                dtTestPro.Rows.Add(s[0], s[1], s[2]);
            }
            //设置dgv数据源和列宽
            dgvTestPro.DataSource = dtTestPro;
            dgvTestPro.Columns[0].ReadOnly = true;
            dgvTestPro.Columns[0].Width = 51;  //50
            dgvTestPro.Columns[1].Width = 80;
            dgvTestPro.Columns[2].ReadOnly = true;
            dgvTestPro.Columns[2].Width = 65;  //50

            //为扫码委托增加一个钩子回调方法
            barCodeHook.BarCodeEvent += new BarCodeHook.BarCodeDelegate(BarCode_BarCodeEvent);
            barCodeHook.Start();//打开钩子
        }
        /// <summary>
        /// 钩子回调方法 j
        /// </summary>
        /// <param name="barCode">条码</param>
        void BarCode_BarCodeEvent(BarCodeHook.BarCodes barCode)
        {
            HandleBarCode(barCode);
        }
        private delegate void ShowInfoDelegate(BarCodeHook.BarCodes barCode);
        /// <summary>
        /// 扫码信息处理函数 j
        /// </summary>
        /// <param name="barCode">条码</param>
        private void HandleBarCode(BarCodeHook.BarCodes barCode)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowInfoDelegate(HandleBarCode), new object[] { barCode });
            }
            else
            {
                if (barCode.IsValid)
                {
                    //使用一个正则，使得里面的空格，制表符等去除,把信息写到条码框里

                    string rgCode = Regex.Replace(barCode.BarCode, @"\s", "");  //扫描得到条码

                    string decryption = StringUtils.instance.ToDecryption(rgCode); //得到解密后条码
                    string signChar = decryption.Substring(0, 1);  //四种条码的首位是序号，分别是1 2 3 4                    
                    string sign2 = decryption.Substring(2);  //定标浓度条码使用，去掉前两位
                    //string sign13 = decryption.Substring(1, 3);

                    string sign17;   //发光值条码使用，第一个发光值的7位
                    string sign8;    //发光值条码使用，第二个发光值剩余7位                    

                    if (rgCode != null && rgCode != "")
                    {
                        switch (signChar)
                        {
                            case "2":  //实验流程的条码
                                DbHelperOleDb db0 = new DbHelperOleDb(0);  //连接数据库ProjectInfo
                                fillDgvProcess(rgCode);   //填充实验流程

                                #region 注释掉
                                //lyq add 2090820
                                //string sql = "select ShortName from tbProject where ShortName = '" + itemName + "'";
                                //DataTable dtTemp = DbHelperOleDb.Query(sql).Tables[0];
                                //string shortName = dtTemp.Rows[0][0].ToString();

                                //判断该条码是否是已经存在项目
                                //string sql = "select ProjectNumber from tbProject where ShortName = '" + itemName + "'";
                                //DataTable dtTemp = DbHelperOleDb.Query(sql).Tables[0];
                                //string projectNumber = dtTemp.Rows[0][0].ToString();
                                //if (projectNumber == int.Parse(sign13).ToString())
                                //{
                                //    fillDgvProcess(rgCode);
                                //}
                                //else
                                //{
                                //    frmMsg.MessageShow("该条码与此项目在数据库的编号不匹配！");
                                //    return;
                                //}
                                #endregion
                                break;
                            case "3":  //定标浓度第一个条码，包含4个浓度
                                string[] decryArray = sign2.Split(new char[3] { 'B', 'C', 'D' }); //得到四个浓度
                                for (int i = 0; i < decryArray.Length; i++)  //填充GridView
                                {
                                    dtConcValue.Rows[i][1] = decryArray[i].Substring(0, decryArray[i].Length - 1);
                                }
                                break;
                            case "4":  //定标浓度第二个条码，包含两个或三个浓度
                                string[] decryArray2 = sign2.Split(new char[2] { 'F', 'G' }); //得到几个浓度
                                //扫描枪得到的rgCode不知道什么问题，全是大写字母了
                                if (decryArray2.Length >= 4) //如果是7点定标
                                {
                                    for (int i = 0; i < decryArray2.Length; i++)
                                    {
                                        if (i > 0)
                                        {
                                            if (i == 2)   // split后 ，FG连一块，第三个string块是空的，第四个才是G后浓度
                                            {
                                                dtConcValue.Rows[6][1] = decryArray2[3];  //填充第七个浓度
                                                break;
                                            }
                                            else
                                            {
                                                dtConcValue.Rows[i + 4][1] = decryArray2[i]; //填充第六个浓度
                                            }
                                        }
                                        else  //i==0，去掉尾部标志，填充第五个浓度
                                        {
                                            dtConcValue.Rows[i + 4][1] = decryArray2[i].Substring(0, decryArray2[i].Length - 1); 
                                        }
                                    }
                                }
                                else  //六点定标
                                {
                                    for (int i = 0; i < 2; i++)
                                    {
                                        if (i == 0) //填充第五个浓度
                                        {
                                            dtConcValue.Rows[i + 4][1] = decryArray2[i].Substring(0, decryArray2[i].Length - 1);
                                        }
                                        else  //填充第六个浓度
                                        {
                                            dtConcValue.Rows[i + 4][1] = decryArray2[i];
                                        }
                                    }
                                }
                                
                                break;
                            case "5":  //发光值第一个条码，两个发光值
                                sign17 = decryption.Substring(1, 7); //发光值条码，第一个发光值的7位
                                sign8 = decryption.Substring(8, 7);  //发光值条码，第二个发光值7位
                                if (decryption.Contains("."))  //两个参数中有小数
                                {
                                    string[] strSign17 = sign17.Split('.');
                                    string[] strSign8 = sign8.Split('.');
                                    if (strSign17.Length == 2) //第一个发光值是小数
                                    {
                                        sign17 = Convert.ToInt32(strSign17[0], 16).ToString() + "." + Convert.ToInt32(strSign17[1], 16).ToString();
                                    }
                                    else if (strSign17.Length == 1)  //整数
                                    {
                                        sign17 = Convert.ToInt32(sign17, 16).ToString();
                                    }
                                    if (strSign8.Length == 2) //第二个发光值是小数
                                    {
                                        sign8 = Convert.ToInt32(strSign8[0], 16).ToString() + "." + Convert.ToInt32(strSign8[1], 16).ToString();
                                    }
                                    else if (strSign8.Length == 1) //整数
                                    {
                                        sign8 = Convert.ToInt32(sign8, 16).ToString();
                                    }
                                }
                                else  //两个发光值都是整数
                                {
                                    sign17 = Convert.ToInt32(sign17, 16).ToString();
                                    sign8 = Convert.ToInt32(sign8, 16).ToString();
                                }
                                dtConcValue.Rows[0][2] = sign17;
                                dtConcValue.Rows[1][2] = sign8;
                                //dtConcValue.Rows[0][2] = double.Parse(sign17);//去掉前面的0                                                                                            
                                //dtConcValue.Rows[1][2] = double.Parse(sign8);
                                break;
                            case "6":  //发光值第2个条码，两个发光值
                                sign17 = decryption.Substring(1, 7);
                                sign8 = decryption.Substring(8, 7);
                                if (decryption.Contains("."))
                                {
                                    string[] strSign17 = sign17.Split('.');
                                    string[] strSign8 = sign8.Split('.');
                                    if (strSign17.Length == 2) //小数
                                    {
                                        sign17 = Convert.ToInt32(strSign17[0], 16).ToString() + "." + Convert.ToInt32(strSign17[1], 16).ToString();
                                    }
                                    else if (strSign17.Length == 1)  //整数
                                    {
                                        sign17 = Convert.ToInt32(sign17, 16).ToString();
                                    }
                                    if (strSign8.Length == 2)
                                    {
                                        sign8 = Convert.ToInt32(strSign8[0], 16).ToString() + "." + Convert.ToInt32(strSign8[1], 16).ToString();
                                    }
                                    else if (strSign8.Length == 1)
                                    {
                                        sign8 = Convert.ToInt32(sign8, 16).ToString();
                                    }
                                }
                                else
                                {
                                    sign17 = Convert.ToInt32(sign17, 16).ToString();
                                    sign8 = Convert.ToInt32(sign8, 16).ToString();
                                }
                                dtConcValue.Rows[2][2] = sign17;
                                dtConcValue.Rows[3][2] = sign8;
                                //dtConcValue.Rows[2][2] = double.Parse(sign17);
                                //dtConcValue.Rows[3][2] = double.Parse(sign8);
                                break;
                            case "7":  //发光值第3个条码，两个发光值
                                sign17 = decryption.Substring(1, 7);
                                sign8 = decryption.Substring(8, 7);
                                if (decryption.Contains("."))
                                {
                                    string[] strSign17 = sign17.Split('.');
                                    string[] strSign8 = sign8.Split('.');
                                    if (strSign17.Length == 2) //小数
                                    {
                                        sign17 = Convert.ToInt32(strSign17[0], 16).ToString() + "." + Convert.ToInt32(strSign17[1], 16).ToString();
                                    }
                                    else if (strSign17.Length == 1)  //整数
                                    {
                                        sign17 = Convert.ToInt32(sign17, 16).ToString();
                                    }
                                    if (strSign8.Length == 2)
                                    {
                                        sign8 = Convert.ToInt32(strSign8[0], 16).ToString() + "." + Convert.ToInt32(strSign8[1], 16).ToString();
                                    }
                                    else if (strSign8.Length == 1)
                                    {
                                        sign8 = Convert.ToInt32(sign8, 16).ToString();
                                    }
                                }
                                else
                                {
                                    sign17 = Convert.ToInt32(sign17, 16).ToString();
                                    sign8 = Convert.ToInt32(sign8, 16).ToString();
                                }
                                dtConcValue.Rows[4][2] = sign17;
                                dtConcValue.Rows[5][2] = sign8;
                                //dtConcValue.Rows[4][2] = double.Parse(sign17);
                                //dtConcValue.Rows[5][2] = double.Parse(sign8);
                                break;
                            case "8":  //发光值第4个条码，1个发光值
                                sign17 = decryption.Substring(1, 7);
                                if (decryption.Contains("."))
                                {
                                    string[] strSign17 = sign17.Split('.');
                                    if (strSign17.Length == 2) //小数
                                    {
                                        sign17 = Convert.ToInt32(strSign17[0], 16).ToString() + "." + Convert.ToInt32(strSign17[1], 16).ToString();
                                    }
                                    else if (strSign17.Length == 1)  //整数
                                    {
                                        sign17 = Convert.ToInt32(sign17, 16).ToString();
                                    }
                                }
                                else
                                {
                                    sign17 = Convert.ToInt32(sign17, 16).ToString();
                                }
                                dtConcValue.Rows[6][2] = sign17;
                                //dtConcValue.Rows[6][2] = double.Parse(sign17);
                                break;
                            default:
                                frmMsg.MessageShow("项目更新", getString("keywordText.ScanByStandard"));
                                //Console.WriteLine("请按标准扫描本公司条码");
                                break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 填充流程列表
        /// </summary>
        /// <param name="rgcode"></param>
        private void fillDgvProcess(string rgcode)
        {
            string decryption = StringUtils.instance.ToDecryption(rgcode);

            if ((decryption.Substring(4, 1) == "f") || (decryption.Substring(4, 1) == "F"))//之所以加一个大写，是因为扫码设备通过钩子全是大写，后期会改进
            {
                //明文字符串删掉已经用完的字符
                decryption = decryption.Substring(5);
                if (dgvTestPro.Rows.Count < 1)
                {
                    return;
                }
                //获取当前实验的流程，并根据流程取不同位数的值
                for (int i = 0; i < dgvTestPro.Rows.Count; i++)
                {
                    switch (dgvTestPro.Rows[i].Cells[0].Value.ToString())  //这是通过判断界面 gridView中的流程每一项，再更新条码中特定位置的数。  这是更新值，不是更新流程。
                    {
                        case "S":                                          //所以需要条码生成中用到的数据库 中的项目流程，和软件数据库中的项目流程 顺序保持一致
                            dgvTestPro.Rows[i].Cells[1].Value = int.Parse(decryption.Substring(0, 2)) * 5;
                            decryption = decryption.Substring(2);
                            break;
                        case "R1":
                            dgvTestPro.Rows[i].Cells[1].Value = int.Parse(decryption.Substring(0, 2)) * 10;
                            decryption = decryption.Substring(2);
                            break;
                        case "R2":
                            dgvTestPro.Rows[i].Cells[1].Value = int.Parse(decryption.Substring(0, 2)) * 10;
                            decryption = decryption.Substring(2);
                            break;
                        case "R3":
                            dgvTestPro.Rows[i].Cells[1].Value = int.Parse(decryption.Substring(0, 2)) * 10;
                            decryption = decryption.Substring(2);
                            break;
                        case "R4":
                            dgvTestPro.Rows[i].Cells[1].Value = int.Parse(decryption.Substring(0, 2)) * 10;
                            decryption = decryption.Substring(2);
                            break;
                        case "H":   //lyq mod 20190816
                            string time16 = decryption.Substring(0, 3);
                            int timeSecond10 = Convert.ToInt32(time16, 16);
                            int timeMin = timeSecond10 / 60;
                            dgvTestPro.Rows[i].Cells[1].Value = timeMin;
                            decryption = decryption.Substring(3);
                            break;
                        case "B":
                            dgvTestPro.Rows[i].Cells[1].Value = int.Parse(decryption.Substring(0, 1)) * 10;
                            decryption = decryption.Substring(1);
                            break;
                        case "W":
                            dgvTestPro.Rows[i].Cells[1].Value = int.Parse(decryption.Substring(0, 1)) * 100;
                            decryption = decryption.Substring(1);
                            break;
                        case "T":
                            dgvTestPro.Rows[i].Cells[1].Value = int.Parse(decryption.Substring(0, 1)) * 100;
                            decryption = decryption.Substring(1);
                            break;
                        case "D":
                            dgvTestPro.Rows[i].Cells[1].Value = int.Parse(decryption.Substring(0, 1)) * 10;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!haveItem)
                return;
            //注释这部分的判断，使得可以更新某个项目的信息 jun mod 20190409
            
            ////查询是否存在相应项目名称和试剂批号的主曲线
            //DataTable dt = bllmsc.GetList(" ItemName = '" + itemName + "' and RegentBatch = '" + regentBatch + "'").Tables[0];
            //if (dt.Rows.Count > 0)
            //{
            //    frmMsg.MessageShow("添加曲线", "该批号试剂主曲线已输入过！");
            //    Close();
            //    return;
            //}
            //定标点数
            string pt = "";
            //吸光度值
            string values = "";
            for (int i = 0; i < dtConcValue.Rows.Count; i++)
            {
                pt += "(" + dtConcValue.Rows[i]["Conc"].ToString() + "," + dtConcValue.Rows[i]["Value"].ToString() + ");";
                values += dtConcValue.Rows[i]["Value"].ToString() + ",";
            }
            string[] value = values.Split(',');
            for (int i = 0; i < value.Length - 1; i++)
            {
                if (value[i] == "")
                {
                    frmMsg.MessageShow(getString("keywordText.AddCurve"), getString("keywordText.ValueNone"));
                    return;
                }
            }
            //lyq add 20190816 
            //在主曲线表，查找id根据批号，如果没有则添加记录
            int mCurve = 0;
            DbHelperOleDb db ;
            mCurve = bllmsc.SelectIdAsRegentBatch(regentBatch);
            if (mCurve == 0)
            {
                Model.tbMainScalCurve modelMSC = new Model.tbMainScalCurve();
                modelMSC.ItemName = itemName;
                modelMSC.RegentBatch = regentBatch;
                modelMSC.Points = pt;
                modelMSC.ValidPeriod = validDate == "" ? DateTime.Now.AddDays(365) : DateTime.Parse(validDate);//更改为365天 jun mod 20190409
                modelMSC.ActiveDate = DateTime.Now;
                activeDate = DateTime.Now.ToString();
                if (!bllmsc.Add(modelMSC))
                {
                    throw new Exception();
                    return;
                }
                mCurve = bllmsc.SelectIdAsRegentBatch(regentBatch);
            }
            if (mCurve == 0)
            {
                frmMsg.MessageShow("项目更新", getString("keywordText.FindNone"));
                return;
            }

            modelMainScalcurve.MainCurveID = mCurve;            
            //frmMsg.MessageShow(modelMainScalcurve.MainCurveID+"");
            modelMainScalcurve.ItemName = itemName;
            modelMainScalcurve.RegentBatch = regentBatch;
            modelMainScalcurve.Points = pt;
            modelMainScalcurve.ValidPeriod = validDate == "" ? DateTime.Now.AddDays(365) : DateTime.Parse(validDate);//更改为365天 jun mod 20190409
            modelMainScalcurve.ActiveDate = activeDate == "" ? DateTime.Now : DateTime.Parse(activeDate);
            
            //直接用sql更新一条修改的数据 jun add 20190409
            //更新流程
            string steps = "";
            StringBuilder sbSteps = new StringBuilder();
            for (int i = 0; i < dgvTestPro.Rows.Count; i++)
            {
                if (i == dgvTestPro.Rows.Count - 1)
                {
                    sbSteps.Append(dgvTestPro.Rows[i].Cells[0].Value.ToString());
                    sbSteps.Append("-" + dgvTestPro.Rows[i].Cells[1].Value.ToString() + "-");
                    sbSteps.Append(dgvTestPro.Rows[i].Cells[2].Value.ToString());
                    break;
                }
                sbSteps.Append(dgvTestPro.Rows[i].Cells[0].Value.ToString());
                sbSteps.Append("-" + dgvTestPro.Rows[i].Cells[1].Value.ToString() + "-");
                sbSteps.Append(dgvTestPro.Rows[i].Cells[2].Value.ToString() + ";");                
            }

            //lyq add 20190821 把浓度更新到项目信息表
            string ptTotbPro = ""; //浓度
            for (int i = 0; i < dtConcValue.Rows.Count; i++)
            {
                if (i == dtConcValue.Rows.Count - 1)
                {
                    ptTotbPro += dtConcValue.Rows[i]["Conc"].ToString();
                    break;
                }
                ptTotbPro += dtConcValue.Rows[i]["Conc"].ToString() + ",";
            }
            string ptTosql = ",CalPointConc ='" + ptTotbPro + "'";

            steps = sbSteps.ToString();    
            string sql = "update tbProject set ProjectProcedure ='" + steps + "'"+ ptTosql +"where ShortName = '"+itemName+"'";   //lyq mod 20190815 
            db = new DbHelperOleDb(0); 
            int rows = DbHelperOleDb.ExecuteSql(0,sql);//更新流程到数据库
            if (rows > 0)
            {
                frmMsg.MessageShow("项目更新",getString("keywordText.UpdateAucceeded"));
            }
            else
            {
                frmMsg.MessageShow("项目更新", getString("keywordText.UpdateFailed"));
            }

            //更新曲线
            db = new DbHelperOleDb(1);
            //if (bllmsc.Add(modelMainScalcurve))
            if (bllmsc.Update(modelMainScalcurve))//更新，不是添加 jun mod 20190409
            {
                frmMsg.MessageShow(getString("keywordText.AddCurve"), getString("keywordText.AddCurveAucceeded"));
                if (AddMainCurve != null)
                {
                    AddMainCurve();
                }
                Close();
            }
            else
            {
                frmMsg.MessageShow(getString("keywordText.AddCurve"), getString("keywordText.AddCurveFailed"));
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            barCodeHook.Stop(); //lyq add 190815
            this.Close();
        }

        /// <summary>
        /// 添加主曲线时控制发光值的输入 
        /// 2018-4-19 zlx add
        /// </summary>
        TextBox control;
        bool _bevent = false;
        private void dgvScaling_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            control = (TextBox)e.Control;
            if (control.Text == "" && !_bevent)
            {
                control.KeyPress += new KeyPressEventHandler(control_KeyPress);
                _bevent = true;
            }
        }
        void control_KeyPress(object sender, KeyPressEventArgs e)
        {
            //限制只能输入1-9的数字和退格键
            if (dgvScaling.CurrentCell.ColumnIndex == 2)
            {
                if (((int)e.KeyChar >= 48 && (int)e.KeyChar <= 57) || e.KeyChar == 8)
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                    frmMsg.MessageShow(getString("keywordText.AddCurve"), getString("keywordText.OnlyNum"));
                }
            }

        }

        private void showScanConc_Click(object sender, EventArgs e)
        {
            ////点击扫描浓度 jun add 20190408
            // frmScanConc frmSC = new frmScanConc();
            // DialogResult dr = frmSC.ShowDialog();
            // string decryption="";
            // if (dr != DialogResult.OK)
            // {
            //     return;
            // }
            // if (dr == DialogResult.OK) 
            // {
            //     decryption = StringUtils.instance.ToDecryption(concCode);                
            // }
            // if (decryption == "" || decryption == null)
            // {
            //     return;
            // }
            // string[] decryArray;
            // if(decryption.Substring(0,1)=="3")
            // {
            //     decryArray = decryption.Substring(2).Split(new char[3] {  'B', 'C', 'D' });
            //     for (int i = 0; i < decryArray.Length; i++)
            //     {
            //         dtConcValue.Rows[i][1] = decryArray[i].Substring(0,decryArray[i].Length-1);
            //     }
            // }
            // else if (decryption.Substring(0, 1) == "4")
            // {
            //     decryArray = decryption.Substring(2).Split(new char[1] { 'F' });
            //     for (int i = 0; i < decryArray.Length; i++)
            //     {
            //         dtConcValue.Rows[i + 4][1] = decryArray[i].Substring(0, decryArray[i].Length - 1);
            //     }
            // }

        }

        private void functionButton2_Click(object sender, EventArgs e)
        {
            ////扫描发光值 jun add 20190408
            //frmSacanConcValue frmSCV = new frmSacanConcValue();
            //DialogResult dr = frmSCV.ShowDialog();
            //string decryption = "";
            //if (dr != DialogResult.OK)
            //{
            //    return;
            //}
            //if (dr == DialogResult.OK)
            //{
            //    decryption = StringUtils.instance.ToDecryption(concValueCode);
            //}
            //if (decryption.Substring(0, 1) == "5")
            //{

            //    dtConcValue.Rows[0][2] = double.Parse(decryption.Substring(1, 7));//去掉前面的0
            //    dtConcValue.Rows[1][2] = double.Parse(decryption.Substring(8));
            //}
            //else if (decryption.Substring(0, 1) == "6")
            //{
            //    dtConcValue.Rows[2][2] = double.Parse(decryption.Substring(1, 7));
            //    dtConcValue.Rows[3][2] = double.Parse(decryption.Substring(8));
            //}
            //else if (decryption.Substring(0, 1) == "7")
            //{
            //    dtConcValue.Rows[4][2] = double.Parse(decryption.Substring(1, 7));
            //    dtConcValue.Rows[5][2] = double.Parse(decryption.Substring(8));
            //}
        }

        private void btnScanTestPro_Click(object sender, EventArgs e)
        {
            //扫描实验流程 jun add 20190408
            //frmScanTestPro frmSTP = new frmScanTestPro();
            //DialogResult dr = frmSTP.ShowDialog();
            //string decryption = "";
            //if (dr != DialogResult.OK)
            //{
            //    return;
            //}
            //if (dr == DialogResult.OK)
            //{
            //    decryption = StringUtils.instance.ToDecryption(testProCode);
            //}
            //if (decryption.Substring(4, 1) == "f")
            //{
            //    //明文字符串删掉已经用完的字符
            //    decryption = decryption.Substring(5);
            //    if (dgvTestPro.Rows.Count <1) 
            //    {
            //        return;
            //    }
            //    //获取当前实验的流程，并根据流程取不同位数的值
            //    for (int i = 0; i < dgvTestPro.Rows.Count; i++) 
            //    {
            //        switch (dgvTestPro.Rows[i].Cells[0].Value.ToString())
            //        {
            //            case "S":
            //                dgvTestPro.Rows[i].Cells[1].Value = int.Parse( decryption.Substring(0, 2))*5;
            //                decryption = decryption.Substring(2);
            //                break;
            //            case "R1":
            //                dgvTestPro.Rows[i].Cells[1].Value = int.Parse( decryption.Substring(0, 2))*10;
            //                decryption = decryption.Substring(2);
            //                break;
            //            case "R2":
            //                dgvTestPro.Rows[i].Cells[1].Value = int.Parse( decryption.Substring(0, 2))*10;
            //                decryption = decryption.Substring(2);
            //                break;
            //            case "R3":
            //                dgvTestPro.Rows[i].Cells[1].Value = int.Parse( decryption.Substring(0, 2))*10;
            //                decryption = decryption.Substring(2);
            //                break;
            //            case "R4":
            //                dgvTestPro.Rows[i].Cells[1].Value = int.Parse( decryption.Substring(0, 2))*10;
            //                decryption = decryption.Substring(2);
            //                break;
            //            case "H":
            //                dgvTestPro.Rows[i].Cells[1].Value = int.Parse( decryption.Substring(0, 2))*100;
            //                decryption = decryption.Substring(2);
            //                break;
            //            case "B":
            //                dgvTestPro.Rows[i].Cells[1].Value = int.Parse( decryption.Substring(0,1))*10;
            //                decryption = decryption.Substring(1);
            //                break;
            //            case "W":
            //                dgvTestPro.Rows[i].Cells[1].Value = int.Parse( decryption.Substring(0, 1))*100;
            //                decryption = decryption.Substring(1);
            //                break;
            //            case "T":
            //                dgvTestPro.Rows[i].Cells[1].Value = int.Parse( decryption.Substring(0, 1))*100;
            //                decryption = decryption.Substring(1);
            //                break;
            //            case "D":
            //                dgvTestPro.Rows[i].Cells[1].Value = int.Parse( decryption.Substring(0, 1))*10;
            //                break;
            //            default :
            //                frmMsg.MessageShow("出现字典没有的字符串");
            //                break;
            //        }
            //    }
            //}
        }

        private void frmAddScaling_FormClosing(object sender, FormClosingEventArgs e)
        {
            barCodeHook.Stop();
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                dgvTestPro.Enabled = true;
                dgvScaling.Enabled = true;
                barCodeHook.Stop();
                groupBox4.Enabled = true;
            }
            else
            {
                dgvTestPro.Enabled = false;
                dgvScaling.Enabled = false;
                barCodeHook.Start();
                groupBox4.Enabled = false;
            }
        }

        //lyq add 190816
        private void FunctionButton1_Click(object sender, EventArgs e) //有焦点扫码输入或手动输入
        {
            if (!haveItem)
                return;
            string rgCode = codeTextBox.Text.Trim();
            if (rgCode == null || rgCode == "")
            {
                return;
            }

            string decryption = StringUtils.instance.ToDecryption(rgCode);
            string signChar = decryption.Substring(0, 1);
            //string sign13 = decryption.Substring(1, 3);
            string sign2 = decryption.Substring(2);

            string sign17;
            string sign8;

            try
            {
                switch (signChar)
                {
                    case "2":  //实验流程的条码
                        DbHelperOleDb db0 = new DbHelperOleDb(0);  //连接数据库ProjectInfo
                        fillDgvProcess(rgCode);   //填充实验流程

                        #region 注释掉
                        //lyq add 2090820
                        //string sql = "select ShortName from tbProject where ShortName = '" + itemName + "'";
                        //DataTable dtTemp = DbHelperOleDb.Query(sql).Tables[0];
                        //string shortName = dtTemp.Rows[0][0].ToString();

                        //判断该条码是否是已经存在项目
                        //string sql = "select ProjectNumber from tbProject where ShortName = '" + itemName + "'";
                        //DataTable dtTemp = DbHelperOleDb.Query(sql).Tables[0];
                        //string projectNumber = dtTemp.Rows[0][0].ToString();
                        //if (projectNumber == int.Parse(sign13).ToString())
                        //{
                        //    fillDgvProcess(rgCode);
                        //}
                        //else
                        //{
                        //    frmMsg.MessageShow("该条码与此项目在数据库的编号不匹配！");
                        //    return;
                        //}
                        #endregion
                        codeTextBox.Text = "";
                        codeTextBox.Focus();
                        break;
                    case "3":
                        string[] decryArray = sign2.Split(new char[3] { 'B', 'C', 'D' });
                        for (int i = 0; i < decryArray.Length; i++)
                        {
                            dtConcValue.Rows[i][1] = decryArray[i].Substring(0, decryArray[i].Length - 1);
                        }
                        codeTextBox.Text = "";
                        codeTextBox.Focus();
                        break;
                    case "4"://lyq add 20190814 扫描枪得到的rgCode不知道什么问题，全是大写字母了
                        if (rgCode.Contains('x'))
                        {
                            string[] decryArray2 = sign2.Split(new char[2] { 'F', 'G' });
                            for (int i = 0; i < decryArray2.Length; i++)
                            {
                                dtConcValue.Rows[i + 4][1] = decryArray2[i].Substring(0, decryArray2.Length); ;
                            }
                        }
                        else
                        {
                            string[] decryArray2 = sign2.Split(new char[2] { 'F', 'G' });
                            if (decryArray2.Length >= 4)
                            {
                                for (int i = 0; i < decryArray2.Length; i++)
                                {
                                    if (i > 0)
                                    {
                                        if (i == 2)   // split后 ，FG连一块，第三个string块是空的，第四个才是G后浓度
                                        {
                                            dtConcValue.Rows[6][1] = decryArray2[3];
                                            break;
                                        }
                                        else
                                        {
                                            dtConcValue.Rows[i + 4][1] = decryArray2[i];
                                        }
                                    }
                                    else
                                    {
                                        dtConcValue.Rows[i + 4][1] = decryArray2[i].Substring(0, decryArray2[i].Length - 1);
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    if (i == 0)
                                    {
                                        dtConcValue.Rows[i + 4][1] = decryArray2[i].Substring(0, decryArray2[i].Length - 1);
                                    }
                                    else
                                    {
                                        dtConcValue.Rows[i + 4][1] = decryArray2[i];
                                    }
                                }
                            }
                        }
                        codeTextBox.Text = "";
                        codeTextBox.Focus();
                        break;
                    case "5":
                        sign17 = decryption.Substring(1, 7);
                        sign8 = decryption.Substring(8, 7);
                        if (decryption.Contains("."))  //两个参数中有小数
                        {
                            string[] strSign17 = sign17.Split('.');
                            string[] strSign8 = sign8.Split('.');
                            if (strSign17.Length == 2) //小数
                            {
                                sign17 = Convert.ToInt32(strSign17[0], 16).ToString() + "." + Convert.ToInt32(strSign17[1], 16).ToString();
                            }
                            else if (strSign17.Length == 1)  //整数
                            {
                                sign17 = Convert.ToInt32(sign17, 16).ToString();
                            }
                            if (strSign8.Length == 2)
                            {
                                sign8 = Convert.ToInt32(strSign8[0], 16).ToString() + "." + Convert.ToInt32(strSign8[1], 16).ToString();
                            }
                            else if (strSign8.Length == 1)
                            {
                                sign8 = Convert.ToInt32(sign8, 16).ToString();
                            }
                        }
                        else
                        {
                            sign17 = Convert.ToInt32(sign17, 16).ToString();
                            sign8 = Convert.ToInt32(sign8, 16).ToString();
                        }
                        dtConcValue.Rows[0][2] = sign17;
                        dtConcValue.Rows[1][2] = sign8;
                        //dtConcValue.Rows[0][2] = double.Parse(sign17);//去掉前面的0                                                                                            
                        //dtConcValue.Rows[1][2] = double.Parse(sign8);
                        break;
                    case "6":
                        sign17 = decryption.Substring(1, 7);
                        sign8 = decryption.Substring(8, 7);
                        if (decryption.Contains("."))
                        {
                            string[] strSign17 = sign17.Split('.');
                            string[] strSign8 = sign8.Split('.');
                            if (strSign17.Length == 2) //小数
                            {
                                sign17 = Convert.ToInt32(strSign17[0], 16).ToString() + "." + Convert.ToInt32(strSign17[1], 16).ToString();
                            }
                            else if (strSign17.Length == 1)  //整数
                            {
                                sign17 = Convert.ToInt32(sign17, 16).ToString();
                            }
                            if (strSign8.Length == 2)
                            {
                                sign8 = Convert.ToInt32(strSign8[0], 16).ToString() + "." + Convert.ToInt32(strSign8[1], 16).ToString();
                            }
                            else if (strSign8.Length == 1)
                            {
                                sign8 = Convert.ToInt32(sign8, 16).ToString();
                            }
                        }
                        else
                        {
                            sign17 = Convert.ToInt32(sign17, 16).ToString();
                            sign8 = Convert.ToInt32(sign8, 16).ToString();
                        }
                        dtConcValue.Rows[2][2] = sign17;
                        dtConcValue.Rows[3][2] = sign8;
                        //dtConcValue.Rows[2][2] = double.Parse(sign17);
                        //dtConcValue.Rows[3][2] = double.Parse(sign8);
                        break;
                    case "7":
                        sign17 = decryption.Substring(1, 7);
                        sign8 = decryption.Substring(8, 7);
                        if (decryption.Contains("."))
                        {
                            string[] strSign17 = sign17.Split('.');
                            string[] strSign8 = sign8.Split('.');
                            if (strSign17.Length == 2) //小数
                            {
                                sign17 = Convert.ToInt32(strSign17[0], 16).ToString() + "." + Convert.ToInt32(strSign17[1], 16).ToString();
                            }
                            else if (strSign17.Length == 1)  //整数
                            {
                                sign17 = Convert.ToInt32(sign17, 16).ToString();
                            }
                            if (strSign8.Length == 2)
                            {
                                sign8 = Convert.ToInt32(strSign8[0], 16).ToString() + "." + Convert.ToInt32(strSign8[1], 16).ToString();
                            }
                            else if (strSign8.Length == 1)
                            {
                                sign8 = Convert.ToInt32(sign8, 16).ToString();
                            }
                        }
                        else
                        {
                            sign17 = Convert.ToInt32(sign17, 16).ToString();
                            sign8 = Convert.ToInt32(sign8, 16).ToString();
                        }
                        dtConcValue.Rows[4][2] = sign17;
                        dtConcValue.Rows[5][2] = sign8;
                        //dtConcValue.Rows[4][2] = double.Parse(sign17);
                        //dtConcValue.Rows[5][2] = double.Parse(sign8);
                        break;
                    case "8":
                        sign17 = decryption.Substring(1, 7);
                        if (decryption.Contains("."))
                        {
                            string[] strSign17 = sign17.Split('.');
                            if (strSign17.Length == 2) //小数
                            {
                                sign17 = Convert.ToInt32(strSign17[0], 16).ToString() + "." + Convert.ToInt32(strSign17[1], 16).ToString();
                            }
                            else if (strSign17.Length == 1)  //整数
                            {
                                sign17 = Convert.ToInt32(sign17, 16).ToString();
                            }
                        }
                        else
                        {
                            sign17 = Convert.ToInt32(sign17, 16).ToString();
                        }
                        dtConcValue.Rows[6][2] = sign17;
                        //dtConcValue.Rows[6][2] = double.Parse(sign17);
                        break;
                    default:
                        frmMsg.MessageShow("项目更新", getString("keywordText.ScanByStandard"));
                        break;
                }
            }
            catch(System.Exception ex)
            {
                ;
            }
            codeTextBox.Text = "";
            codeTextBox.Focus();
        }
        private string getString(string key)
        {
            ResourceManager resManager = new ResourceManager(typeof(frmAddScaling));
            return resManager.GetString(key);
        }
    }
}
