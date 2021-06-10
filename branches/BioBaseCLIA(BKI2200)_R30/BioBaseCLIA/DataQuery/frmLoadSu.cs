using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Maticsoft.DBUtility;
using Common;
using DBUtility;
using System.Text.RegularExpressions;
using System.Threading;
using System.Resources;

namespace BioBaseCLIA.DataQuery
{
    public partial class frmLoadSu : frmSmallParent
    {
        /// <summary>
        /// 更改主界面底物瓶颜色
        /// </summary>
        public static event Action<int, int> suTestRatio;
        /// <summary>
        /// 底物瓶瓶号
        /// </summary>
        public static int bootleNum { get; set; }
        BLL.tbSubstrate bllsb = new BLL.tbSubstrate();
        /// <summary>
        /// 条码获取测试次数
        /// </summary>
        string testNum = "";
        /// <summary>
        /// 条码获取生产日期
        /// </summary>
        DateTime dtime;
        /// <summary>
        /// 已装载正常状态的底物
        /// </summary>
        DataTable dtSb = new DataTable();
        string[] initCon = new string[3];
        bool changeFlag = true;
        bool fillFlag = false;
        /// <summary>
        /// 更改主界面底物管架试剂按钮颜色.LYN add 20171114
        /// </summary>
        public static event Action<int, int, int> btnBtnColor;
        frmMessageShow frmMsgShow = new frmMessageShow();
        /// <summary>
        /// 无焦点获取扫码信息钩子
        /// </summary>
        BarCodeHook barCodeHook = new BarCodeHook();
        public frmLoadSu()
        {
            InitializeComponent();
        }

        private void frmLoadSu_Load(object sender, EventArgs e)
        {
            changeFlag = false;
            dtSb = bllsb.GetList("Status='正常'and SubstrateNumber = '" + bootleNum + "'").Tables[0];
            if (dtSb.Rows.Count > 0)
            {
                txtSubstrateCode.Text = dtSb.Rows[0]["BarCode"].ToString();
                txtSubstrateAllTest.Text = dtSb.Rows[0]["AllTestNumber"].ToString();
                txtSubstrateLastTest.Text = dtSb.Rows[0]["leftoverTest"].ToString();
                ValidDate.Value = Convert.ToDateTime(dtSb.Rows[0]["ValidDate"]);

            }
            else
            {
                txtSubstrateCode.Text = "";
                txtSubstrateAllTest.Text = "0";
                txtSubstrateLastTest.Text = "0";
                ValidDate.Value = DateTime.Now.Date.AddMonths(1);
            }

            initCon[0] = txtSubstrateCode.Text;
            initCon[1] = txtSubstrateAllTest.Text;
            initCon[2] = txtSubstrateLastTest.Text;

            btnLoadSubstrate.Enabled = true;
            changeFlag = true;

            barCodeHook.BarCodeEvent += new BarCodeHook.BarCodeDelegate(BarCode_BarCodeEvent);
            this.txtSubstrateCode.TextChanged += new EventHandler(txtSubstrateCode_TextChanged);
            barCodeHook.Start();
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
                    string rgCode = Regex.Replace(barCode.BarCode, @"\s", "");
                    if (rgCode != null && rgCode != "")
                    {
                        this.txtSubstrateCode.Text = rgCode;
                    }
                }
            }
        }
        private void btnDelSubstrate_Click(object sender, EventArgs e)
        {
            #region 扫码-卸载 //lyq mod 20201012
            if (dtSb.Rows.Count <= 0)
            {
                frmMsgShow.MessageShow(Getstring("MessageCaption"), Getstring("NoSubstrate"));
                return;
            }
            if (txtSubstrateCode.Text == "")
            {
                frmMsgShow.MessageShow(Getstring("MessageCaption"), Getstring("SubstrateBarcodeEmpty"));
                return;
            }
            btnDelSubstrate.Enabled = false;
            //把数据库中已装载试剂状态正常改为卸载
            DbHelperOleDb.ExecuteSql(3, @"update tbSubstrate set Status='卸载' where Status ='正常' and BarCode = '" + dtSb.Rows[0]["BarCode"].ToString() + "'");
            //清除ini配置文件
            deleteSuIni();
            frmMsgShow.MessageShow(Getstring("MessageCaption"), Getstring("UnloadSubstrateSuccess"));
            btnDelSubstrate.Enabled = true;
            if (suTestRatio != null)
            {
                suTestRatio(0, 500);
            }
            this.Close();
            #endregion
        }

        private void btnLoadSubstrate_Click(object sender, EventArgs e)
        {
            if (int.Parse(txtSubstrateLastTest.Text) > int.Parse(txtSubstrateAllTest.Text))
            {
                frmMsgShow.MessageShow(Getstring("MessageCaption"), Getstring("LeftTestLessAllTest") );
                txtSubstrateLastTest.Focus();
                frmLoadSu_Load(null, null);
                return;
            }
            if (txtSubstrateCode.Text.Trim() == "")
            {
                txtSubstrateCode.Focus();
                frmMsgShow.MessageShow(Getstring("MessageCaption"), Getstring("InputBarcode") );
                return;
            }
            if (!judgeSubBarCode(txtSubstrateCode.Text.Trim()))
            {
                initContr();
                frmMsgShow.MessageShow(Getstring("MessageCaption"), Getstring("CheckBarcode") );
                return;
            }
            if (bllsb.GetList("Status='正常'").Tables[0].Rows.Count > 0)
            {
                frmMsgShow.MessageShow(Getstring("MessageCaption"), Getstring("AlreadySubstate") );
                return;
            }
            if (!fillFlag)
            {
                txtSubstrateCode.Focus();
                frmMsgShow.MessageShow(Getstring("MessageCaption"), Getstring("InputBarcodeEnter") );
                return;
            }
            Model.tbSubstrate modelSb = new Model.tbSubstrate();
            DataTable dt = bllsb.GetList("Status='卸载'and BarCode ='" + txtSubstrateCode.Text.Trim() + "'").Tables[0];
            if (dt.Rows.Count > 0)
            {
                //把数据库中已装载试剂状态卸载改为正常
                DbHelperOleDb.ExecuteSql(3, @"update tbSubstrate set Status='正常' where Status ='卸载' and BarCode = '" + txtSubstrateCode.Text.Trim() + "'");
                //更改ini配置文件
                string[] SuInfo = new string[4];
                SuInfo[0] = dt.Rows[0]["BarCode"].ToString();
                SuInfo[1] = dt.Rows[0]["AllTestNumber"].ToString();
                SuInfo[2] = dt.Rows[0]["leftoverTest"].ToString();
                //SuInfo[3] = modelSb.AddDate;
                SuInfo[3] = dt.Rows[0]["ValidDate"].ToString();
                ModifySuIni(SuInfo, dt.Rows[0]["AddDate"].ToString().Replace(@"/", "-"));
                frmMsgShow.MessageShow(Getstring("MessageCaption"), Getstring("SubstrateLoadSuccess"));
                if (suTestRatio != null)
                {
                    suTestRatio(int.Parse(SuInfo[2]), int.Parse(SuInfo[1]));
                }
                this.Close();
                return;
            }

            #region 装载底物
            modelSb = new Model.tbSubstrate();
            modelSb.Postion = bootleNum.ToString();
            modelSb.SubstrateNumber = bootleNum.ToString();
            modelSb.BarCode = txtSubstrateCode.Text.Trim();
            modelSb.AllTestNumber = int.Parse(txtSubstrateAllTest.Text.Trim());
            modelSb.AddDate = DateTime.Now.Date.ToShortDateString();
            modelSb.ExtraTest = 100;
            modelSb.leftoverTest = int.Parse(txtSubstrateLastTest.Text.Trim());
            modelSb.Batch = "0001";
            modelSb.ValidDate = ValidDate.Value.ToString("yyyy-MM-dd");
            modelSb.Status = "正常";
            if (bllsb.Add(modelSb))
            {
                if (btnBtnColor != null)
                {
                    this.BeginInvoke(new Action(() => { btnBtnColor(3, 0, 3); }));
                }
                txtSubstrateAllTest.Text = modelSb.AllTestNumber.ToString();
                txtSubstrateLastTest.Text = modelSb.leftoverTest.ToString();
                string[] SuInfo = new string[4];
                SuInfo[0] = modelSb.BarCode;
                SuInfo[1] = modelSb.AllTestNumber.ToString();
                SuInfo[2] = modelSb.leftoverTest.ToString();
                //SuInfo[3] = modelSb.AddDate;
                SuInfo[3] = modelSb.ValidDate;
                ModifySuIni(SuInfo);
                if (suTestRatio != null)
                {
                    suTestRatio(int.Parse(SuInfo[2]), int.Parse(SuInfo[1]));
                }
                if (btnBtnColor != null)
                {
                    this.BeginInvoke(new Action(() => { btnBtnColor(3, 0, 3); }));
                }
                frmMsgShow.MessageShow(Getstring("MessageCaption"), Getstring("SubstrateLoadSuccess") );
                this.Close();
            }
            #endregion
            txtSubstrateAllTest.Enabled = txtSubstrateCode.Enabled = txtSubstrateLastTest.Enabled = false;
            btnLoadSubstrate.Enabled = false;
            btnDelSubstrate.Enabled = true;
            btnDelSubstrate.Text = Getstring("LoadSubstrate");
        }
        void ModifySuIni(string[] suInfo, string loadData = "")
        {
            OperateIniFile.WriteIniData("Substrate" + bootleNum.ToString(), "BarCode", suInfo[0], Application.StartupPath + "//SubstrateTube.ini");
            OperateIniFile.WriteIniData("Substrate" + bootleNum.ToString(), "TestCount", suInfo[1], Application.StartupPath + "//SubstrateTube.ini");
            OperateIniFile.WriteIniData("Substrate" + bootleNum.ToString(), "LeftCount", suInfo[2], Application.StartupPath + "//SubstrateTube.ini");
            if (loadData == "")
                OperateIniFile.WriteIniData("Substrate" + bootleNum.ToString(), "LoadDate", DateTime.Now.ToString("yyyy-MM-dd"), Application.StartupPath + "//SubstrateTube.ini");
            else
                OperateIniFile.WriteIniData("Substrate" + bootleNum.ToString(), "LoadDate", loadData, Application.StartupPath + "//SubstrateTube.ini");
            //2018-10-17 zlx add
            OperateIniFile.WriteIniData("Substrate" + bootleNum.ToString(), "ValidDate", suInfo[3], Application.StartupPath + "//SubstrateTube.ini");
        }
        void deleteSuIni()
        {
            Invoke(new Action(() =>
            {
                txtSubstrateCode.Text = "";
                txtSubstrateAllTest.Text = "";
                txtSubstrateLastTest.Text = "";
                ValidDate.Value = DateTime.Now;
            }));

            OperateIniFile.WriteIniData("Substrate" + bootleNum.ToString(), "BarCode", "", Application.StartupPath + "//SubstrateTube.ini");
            OperateIniFile.WriteIniData("Substrate" + bootleNum.ToString(), "TestCount", "", Application.StartupPath + "//SubstrateTube.ini");
            OperateIniFile.WriteIniData("Substrate" + bootleNum.ToString(), "LeftCount", "", Application.StartupPath + "//SubstrateTube.ini");
            OperateIniFile.WriteIniData("Substrate" + bootleNum.ToString(), "LoadDate", "", Application.StartupPath + "//SubstrateTube.ini");
            //2018-10-17 zlx add
            OperateIniFile.WriteIniData("Substrate" + bootleNum.ToString(), "ValidDate", "", Application.StartupPath + "//SubstrateTube.ini");
        }

        private void chkManualInput_CheckedChanged(object sender, EventArgs e)
        {
            if (chkManualInput.Checked == true)
            {
                initContr();
                txtSubstrateCode.Enabled = true;
            }
            else
            {
                initContr(1);
                txtSubstrateCode.Enabled = false;
            }
        }

        private void txtSubstrateCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;
            if ((txtSubstrateCode.Text.Length != 12 && txtSubstrateCode.Text.Length != 15) || !judgeSubBarCode(txtSubstrateCode.Text.Trim()))
            {
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    frmMessageShow fr = new frmMessageShow();
                    fr.MessageShow(Getstring("MessageCaption"), Getstring("CheckBarcode"));
                }))
                { IsBackground = true }.Start();
                Invoke(new Action(() =>
                {
                    initContr();
                    //frmMsgShow.MessageShow("底物装载", "条码校验未通过！请重新输入");
                }));
                return;
            }

            if (!fillSubInfo(txtSubstrateCode.Text.Trim()))
            {
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    frmMessageShow fr = new frmMessageShow();
                    fr.MessageShow(Getstring("MessageCaption"), Getstring("AlreadySubstate"));
                }))
                { IsBackground = true }.Start();
                Invoke(new Action(() =>
                {
                    initContr();
                    //frmMsgShow.MessageShow("底物装载", "已装载底物,请先卸载底物!");
                }));
            }
        }
        private bool judgeSubBarCode(string subCode)
        {
            string decryption = StringUtils.instance.ToDecryption(subCode);
            string a1 = decryption.Substring(0, 1);

            if (a1 != "A")
            {
                return false;
            }
            if (subCode.Length == 15)
            {
                string batchDate = decryption.Substring(1, 3);//批号日期
                string productDate = decryption.Substring(4, 3);//生产日期
                int testN = Convert.ToInt32(decryption.Substring(7, 3), 16);//测试次数
                int serialNum = Convert.ToInt32(decryption.Substring(10, 4), 16);//流水号
                string year = "", month = "", day = "";
                string year2 = "", month2 = "", day2 = "";
                try
                {
                    year = StringUtils.instance.reverseDate(batchDate.Substring(0, 1).ToCharArray()[0]);
                    month = StringUtils.instance.reverseDate(batchDate.Substring(1, 1).ToCharArray()[0]);
                    day = StringUtils.instance.reverseDate(batchDate.Substring(2, 1).ToCharArray()[0]);

                    year2 = StringUtils.instance.reverseDate(productDate.Substring(0, 1).ToCharArray()[0]);
                    month2 = StringUtils.instance.reverseDate(productDate.Substring(1, 1).ToCharArray()[0]);
                    day2 = StringUtils.instance.reverseDate(productDate.Substring(2, 1).ToCharArray()[0]);
                }
                catch
                {
                    return false;
                }
                while (year.Length < 4)
                {
                    year = year.Insert(0, "20");
                }
                while (month.Length < 2)
                {
                    month = month.Insert(0, "0");
                }
                while (day.Length < 2)
                {
                    day = day.Insert(0, "0");
                }
                while (year2.Length < 4)
                {
                    year2 = year2.Insert(0, "20");
                }
                while (month2.Length < 2)
                {
                    month2 = month2.Insert(0, "0");
                }
                while (day2.Length < 2)
                {
                    day2 = day2.Insert(0, "0");
                }

                int batchTime = int.Parse(year + month + day);
                int productTime = int.Parse(year2 + month2 + day2);
                int check = (10 + batchTime + productTime + testN + serialNum) % 7;
                if (decryption.Substring(14, 1) != check.ToString())
                {
                    return false;
                }

                testNum = testN.ToString();
                try
                {
                    dtime = DateTime.ParseExact(productTime.ToString(), "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
                }
                catch
                {
                    return false;
                }
            }
            else if (subCode.Length == 12)
            {
                string batchDate = decryption.Substring(1, 3);//批号日期
                int testN = Convert.ToInt32(decryption.Substring(4, 3), 16);//测试次数
                string productDate = decryption.Substring(7, 1);//生产日期
                string serialNum = decryption.Substring(8, 3);//流水号
                //int serialNum = Convert.ToInt32(decryption.Substring(8, 3), 16);//流水号

                string year = "", month = "", day = "";
                try
                {
                    year = StringUtils.instance.reverseDate(batchDate.Substring(0, 1).ToCharArray()[0]);
                    month = StringUtils.instance.reverseDate(batchDate.Substring(1, 1).ToCharArray()[0]);
                    day = StringUtils.instance.reverseDate(batchDate.Substring(2, 1).ToCharArray()[0]);
                }
                catch
                {
                    return false;
                }
                while (year.Length < 4)
                {
                    year = year.Insert(0, "20");
                }
                while (month.Length < 2)
                {
                    month = month.Insert(0, "0");
                }
                while (day.Length < 2)
                {
                    day = day.Insert(0, "0");
                }
                int batchTime = int.Parse(year + month + day);

                #region productDate
                if (productDate == "0")//旧版没有生产日期
                {
                    serialNum = Convert.ToInt32(serialNum, 16).ToString();
                }
                else//添加了生产日期
                {
                    if (productDate == "z")
                    {
                        productDate = "0";
                    }
                    else
                    {
                        productDate = StringUtils.instance.reverseDate(productDate.ToCharArray()[0]);
                    }
                    
                    //添加生产日期后的流水号
                    string tempNum1 = serialNum.Substring(0, 1);
                    string tempNum2 = serialNum.Substring(1, 1);
                    string tempNum3 = serialNum.Substring(2, 1);
                    tempNum1 = (int.Parse(StringUtils.instance.reverseDate(tempNum1.ToCharArray()[0])) * Math.Pow(62, 2)).ToString();
                    tempNum2 = (int.Parse(StringUtils.instance.reverseDate(tempNum2.ToCharArray()[0])) * Math.Pow(62, 1)).ToString();
                    tempNum3 = (int.Parse(StringUtils.instance.reverseDate(tempNum3.ToCharArray()[0])) * Math.Pow(62, 0)).ToString();
                    serialNum = (int.Parse(tempNum1) + int.Parse(tempNum2) + int.Parse(tempNum3)).ToString();
                }
                #endregion

                int check = (10 + batchTime + testN + int.Parse(productDate) +int.Parse(serialNum)) % 7;
                if (decryption.Substring(11, 1) != check.ToString())
                {
                    return false;
                }
                productDate = DateTime.Parse(year + "/" + month + "/" + day).AddDays(double.Parse(productDate)).ToString("yyyyMMdd");
                testNum = testN.ToString();
                try
                {
                    dtime = DateTime.ParseExact(productDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
                }
                catch
                {
                    return false;
                }
            }
            else
                return false;
            
            return true;
        }
        private bool fillSubInfo(string subCode)
        {
            if (bllsb.GetList("Status='正常'and SubstrateNumber = '" + bootleNum + "'").Tables[0].Rows.Count > 0)
            {
                return false;
            }
            fillFlag = false;
            //便利数据库查重
            DataTable dt = bllsb.GetList("Status='卸载'and BarCode ='" + subCode + "'").Tables[0];

            if (dt.Rows.Count > 0)//重复，已经装载过
            {
                Invoke(new Action(() =>
                {
                    txtSubstrateCode.Text = subCode;
                    txtSubstrateAllTest.Text = dt.Rows[0]["AllTestNumber"].ToString();
                    txtSubstrateLastTest.Text = dt.Rows[0]["leftoverTest"].ToString();
                    //对比生产日期后一年 和 数据库的有效期
                    DateTime dt1 = dtime.AddYears(1).AddDays(-1);
                    DateTime dt2 = DateTime.ParseExact(dt.Rows[0]["ValidDate"].ToString().Replace("-", ""), "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
                    if (DateTime.Compare(dt1, dt2) <= 0)//使用两个最小的作为有效期
                    {
                        ValidDate.Value = dt1;
                    }
                    else
                    {
                        ValidDate.Value = dt2;
                    }
                }));

            }
            else//首次装载
            {
                Invoke(new Action(() =>
                {
                    txtSubstrateCode.Text = subCode;
                    txtSubstrateAllTest.Text = testNum;
                    txtSubstrateLastTest.Text = testNum;
                    //对比生产日期后一年 和 今天装载日期后一月
                    DateTime dt1 = dtime.AddYears(1).AddDays(-1);
                    ValidDate.Value = dt1;
                    #region 暂使用试剂盒有效期
                    //DateTime dt2 = DateTime.Now.AddMonths(1);
                    //if (DateTime.Compare(dt1, dt2) <= 0)//使用两个最小的作为有效期
                    //{
                    //    ValidDate.Value = dt1;
                    //}
                    //else
                    //{
                    //    ValidDate.Value = dt2;
                    //}
                    #endregion
                }));
            }
            fillFlag = true;
            txtSubstrateCode.Enabled = false;
            return true;
        }

        private void txtSubstrateCode_TextChanged(object sender, EventArgs e)
        {
            if (chkManualInput.Checked || btnDelSubstrate.Enabled == false || txtSubstrateCode.Text == "" || !changeFlag)
            {
                return;
            }
            string rgCode = txtSubstrateCode.Text.Trim();
            if (!judgeSubBarCode(rgCode))
            {
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    frmMessageShow fr = new frmMessageShow();
                    fr.MessageShow(Getstring("MessageCaption"), Getstring("CheckBarcodeFail"));
                }))
                { IsBackground = true }.Start();
                BeginInvoke(new Action(() =>
                {
                    initContr(1);
                    //frmMsgShow.MessageShow("底物装载", "未通过条码校验！");
                }));
                return;
            }
            if (!fillSubInfo(rgCode))
            {
                new Thread(new ParameterizedThreadStart((obj) =>
                {
                    frmMessageShow fr = new frmMessageShow();
                    fr.MessageShow(Getstring("MessageCaption"), Getstring("AlreadySubstate") );
                }))
                { IsBackground = true }.Start();
                BeginInvoke(new Action(() =>
                {
                    initContr(1);
                    //frmMsgShow.MessageShow("底物装载", "已装载底物,请先卸载底物!");
                }));
            }
        }

        private void frmLoadSu_FormClosed(object sender, FormClosedEventArgs e)
        {
            barCodeHook.Stop();
        }
        private void initContr(int state = 0)
        {
            changeFlag = false;
            if (state == 0)
            {
                Invoke(new Action(() =>
                {
                    fillFlag = false;
                    txtSubstrateCode.Text = "";
                    txtSubstrateAllTest.Text = "";
                    txtSubstrateLastTest.Text = "";
                }));
            }
            else if (state == 1)
            {
                Invoke(new Action(() =>
                {
                    fillFlag = false;
                    txtSubstrateCode.Text = initCon[0];
                    txtSubstrateAllTest.Text = initCon[1];
                    txtSubstrateLastTest.Text = initCon[2];
                }));
            }
            changeFlag = true;
        }

        private string Getstring(string key)
        {
            ResourceManager resManagerA =
                    new ResourceManager("BioBaseCLIA.DataQuery.frmLoadSu", typeof(frmLoadSu).Assembly);
            return resManagerA.GetString(key);
        }

    }
}
