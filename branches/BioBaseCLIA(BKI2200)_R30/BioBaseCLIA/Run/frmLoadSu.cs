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
using System.IO;
using System.Text.RegularExpressions;
using DBUtility;

namespace BioBaseCLIA.Run
{
    public partial class frmLoadSu : frmSmallParent
    {
        /// <summary>
        /// 稀释液装载位置
        /// </summary>
        /// <summary>
        /// 试剂盘配置文件地址
        /// </summary>
        string iniPathReagentTrayInfo = Directory.GetCurrentDirectory() + "\\ReagentTrayInfo.ini";
        frmMessageShow frmMsgShow = new frmMessageShow();
        public int RegentPos { get; set; }
        BLL.tbDilute bllsb = new BLL.tbDilute();
        DataTable dtSb = new DataTable();
        string diuBarCode = "";
        /// <summary>
        /// 稀释液生产日期
        /// </summary>
        DateTime dtime;
        string[] initCon = new string[3];
        bool changeFlag = true;
        bool fillFlag = false;
        /// <summary>
        /// 无焦点获取扫码信息钩子
        /// </summary>
        BarCodeHook barCodeHook = new BarCodeHook();
        public frmLoadSu()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 规格体积
        /// </summary>
        private int AllDiuVol { get; set; }
        /// <summary>
        /// 剩余体积
        /// </summary>
        private int LeftDiuVol { get; set; }
        private void frmLoadSu_Load(object sender, EventArgs e)
        {
            changeFlag = false;

            cmbUnit1.SelectedIndex = cmbUnit2.SelectedIndex = 0;
            dtSb = bllsb.GetList("State=1 and DilutePos = " + RegentPos + "").Tables[0];
            if (dtSb.Rows.Count > 0)
            {
                txtRegentPos.Text = RegentPos.ToString();
                txtDiluteNumber.Text = dtSb.Rows[0]["DiluteNumber"].ToString();
                AllDiuVol = int.Parse(dtSb.Rows[0]["AllDiuVol"].ToString());
                LeftDiuVol = int.Parse(dtSb.Rows[0]["LeftDiuVol"].ToString());
                if (cmbUnit1.SelectedItem.ToString() == "ml")
                    txtSubstrateAllTest.Text = (AllDiuVol / 1000).ToString();
                else
                    txtSubstrateAllTest.Text = AllDiuVol.ToString();
                if (cmbUnit2.SelectedItem.ToString() == "ml")
                    txtSubstrateLastTest.Text = (LeftDiuVol / 1000).ToString();
                else
                    txtSubstrateLastTest.Text = LeftDiuVol.ToString();
                ValidDate.Value = Convert.ToDateTime(dtSb.Rows[0]["ValiData"]);
            }
            else
            {
                txtRegentPos.Text = RegentPos.ToString();
                txtDiluteNumber.Text = "";
                txtSubstrateAllTest.Text = "0";
                txtSubstrateLastTest.Text = "0";
                ValidDate.Value = DateTime.Now.Date.AddMonths(1);
            }
            initCon[0] = txtDiluteNumber.Text;
            initCon[1] = txtSubstrateAllTest.Text;
            initCon[2] = txtSubstrateLastTest.Text;

            changeFlag = true;

            barCodeHook.BarCodeEvent += new BarCodeHook.BarCodeDelegate(BarCode_BarCodeEvent);
            this.txtDiluteNumber.TextChanged += new EventHandler(txtDiluteNumber_TextChanged);
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
                        this.txtDiluteNumber.Text = rgCode;
                    }
                }
            }
        }

        private void btnLoadSubstrate_Click(object sender, EventArgs e)
        {
            frmMessageShow frmMsgShow = new frmMessageShow();
            if (int.Parse(txtSubstrateLastTest.Text) > int.Parse(txtSubstrateAllTest.Text))
            {
                frmMsgShow.MessageShow("稀释液装载", "剩余体积不应该大于规格体积！");
                txtSubstrateLastTest.Focus();
                frmLoadSu_Load(null, null);
                return;
            }
            if (txtDiluteNumber.Text.Trim() == "")
            {
                txtDiluteNumber.Focus();
                frmMsgShow.MessageShow("稀释液装载", "请输入稀释液编码！");
                return;
            }
            if (!judgeDiluteBarCode(txtDiluteNumber.Text.Trim()))
            {
                initContr();
                frmMsgShow.MessageShow("稀释液装载", "未通过条码校验！");
                return;
            }
            if (bllsb.GetList("DilutePos = " + txtRegentPos.Text.Trim() + " or DiluteNumber='" + txtDiluteNumber.Text.Trim() + "' and DilutePos<>null").Tables[0].Rows.Count > 0)
            {
                frmMsgShow.MessageShow("稀释液装载", "稀释液已装载,请先卸载稀释液!");
                return;
            }
            if (!fillFlag)
            {
                txtDiluteNumber.Focus();
                frmMsgShow.MessageShow("稀释液装载", "手动输入条码后，请按回车键解析条码信息！");
                return;
            }
            Model.tbDilute modelSb = new Model.tbDilute();

            DataTable dt = bllsb.GetList("DiluteNumber = '" + txtDiluteNumber.Text.Trim() + "'").Tables[0];
            if (dt.Rows.Count > 0)
            {
                //改数据库
                DbHelperOleDb.ExecuteSql(3, @"update tbDilute set DilutePos=" + RegentPos + "  where DiluteNumber = '" + txtDiluteNumber.Text.Trim() + "'");
                //ini配置文件
                OperateIniFile.WriteIniData("ReagentPos" + RegentPos, "leftDiuVol", dt.Rows[0]["LeftDiuVol"].ToString(), iniPathReagentTrayInfo);

                frmMsgShow.MessageShow("稀释液装载", "装载成功！");
                this.Close();
                return;
            }

            DataTable dtAllSb = bllsb.GetAllList().Tables[0];
            var dr1 = dtAllSb.Select("DiluteNumber='" + txtDiluteNumber.Text.Trim() + "'");
            var dr2 = dtAllSb.Select("DilutePos = '" + RegentPos + "'");
            string[] SuInfo = new string[4];
            if (dr1.Length > 0)//原来数据库是否存在该条码，length大于0，则存在
            {
                frmMsgShow.MessageShow("稀释液装载", "该稀释液编号正在使用，请录入正确的编号！");
                txtSubstrateAllTest.Enabled = txtDiluteNumber.Enabled = txtSubstrateLastTest.Enabled = false;
                btnLoadSubstrate.Enabled = false;
                frmLoadSu_Load(null, null);
                return;
            }
            if (dr2.Length > 0)
            {
                DataTable dt1 = bllsb.GetList("State=1 and DilutePos =" + RegentPos + "").Tables[0];
                if (dt1.Rows.Count > 0)
                    bllsb.Delete(int.Parse(dt1.Rows[0]["DilutePos"].ToString()));
            }
            if (cmbUnit1.SelectedItem.ToString() == "ml")
                AllDiuVol = int.Parse(txtSubstrateAllTest.Text) * 1000;
            else
                AllDiuVol = int.Parse(txtSubstrateAllTest.Text);
            if (cmbUnit2.SelectedItem.ToString() == "ml")
                LeftDiuVol = int.Parse(txtSubstrateLastTest.Text) * 1000;
            else
                LeftDiuVol = int.Parse(txtSubstrateLastTest.Text);
            modelSb.DiluteNumber = txtDiluteNumber.Text;
            modelSb.DilutePos = int.Parse(txtRegentPos.Text);
            modelSb.AllDiuVol = AllDiuVol;
            modelSb.LeftDiuVol = LeftDiuVol;
            modelSb.Unit = "ul";
            modelSb.AddData = DateTime.Now.ToShortDateString();
            modelSb.ValiData = ValidDate.Value.ToShortDateString();
            modelSb.State = 1;
            if (bllsb.Add(modelSb))
            {
                OperateIniFile.WriteIniData("ReagentPos" + RegentPos, "leftDiuVol", LeftDiuVol.ToString(), iniPathReagentTrayInfo);
                frmMsgShow.MessageShow("供应品状态", "稀释液装载成功！");

            }
            txtSubstrateAllTest.Enabled = txtDiluteNumber.Enabled = txtSubstrateLastTest.Enabled = false;
            btnLoadSubstrate.Enabled = false;
            this.Close();
        }

        private void functionButton1_Click(object sender, EventArgs e)
        {
            #region 扫码-卸载 //lyq mod 20201013
            if (dtSb.Rows.Count <= 0)
            {
                frmMsgShow.MessageShow("稀释液装载", "没有检测到已装载试剂，请装载试剂");
                return;
            }
            if (txtDiluteNumber.Text == "")
            {
                frmMsgShow.MessageShow("稀释液装载", "稀释液条码为空，请重新打开本界面");
                return;
            }
            functionButton1.Enabled = false;
            //改数据库
            DbHelperOleDb.ExecuteSql(3, @"update tbDilute set DilutePos=null  where DilutePos =" + dtSb.Rows[0]["DilutePos"].ToString() + " and DiluteNumber = '" + dtSb.Rows[0]["DiluteNumber"].ToString() + "'");
            //清除ini配置文件
            OperateIniFile.WriteIniData("ReagentPos" + RegentPos, "leftDiuVol", "", iniPathReagentTrayInfo);
            Invoke(new Action(() =>
            {
                txtDiluteNumber.Text = "";
                txtSubstrateAllTest.Text = "";
                txtSubstrateLastTest.Text = "";
                ValidDate.Value = DateTime.Now;
            }));
            frmMsgShow.MessageShow("稀释液装载", "卸载成功！");
            functionButton1.Enabled = true;
            this.Close();
            #endregion
        }

        private void chkManualInput_CheckedChanged(object sender, EventArgs e)
        {
            if (chkManualInput.Checked == true)
            {
                initContr();
                txtDiluteNumber.Enabled = true;
            }
            else
            {
                initContr(1);
                txtDiluteNumber.Enabled = false;
            }
        }

        private void txtDiluteNumber_TextChanged(object sender, EventArgs e)
        {
            if (chkManualInput.Checked || functionButton1.Enabled == false || txtDiluteNumber.Text == "" || !changeFlag)
            {
                return;
            }
            string rgCode = txtDiluteNumber.Text.Trim();
            if (!judgeDiluteBarCode(rgCode))
            {
                BeginInvoke(new Action(() =>
                {
                    initContr();
                    frmMsgShow.MessageShow("稀释液装载", "未通过条码校验！");
                }));
                return;
            }

            if (!fillDiluteInfo(rgCode))
            {
                BeginInvoke(new Action(() =>
                {
                    initContr();
                    frmMsgShow.MessageShow("稀释液装载", "稀释液已装载,请先卸载稀释液!");
                }));
            }
        }

        private void frmLoadSu_FormClosed(object sender, FormClosedEventArgs e)
        {
            barCodeHook.Stop();
        }

        private void txtDiluteNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;
            if (txtDiluteNumber.Text.Length != 11 || !judgeDiluteBarCode(txtDiluteNumber.Text.Trim()))
            {
                BeginInvoke(new Action(() =>
                {
                    initContr();
                    frmMsgShow.MessageShow("稀释液装载", "条码校验未通过！请重新输入");
                }));
                return;
            }

            if (!fillDiluteInfo(txtDiluteNumber.Text.Trim()))
            {
                BeginInvoke(new Action(() =>
                {
                    initContr();
                    frmMsgShow.MessageShow("稀释液装载", "稀释液已装载,请先卸载稀释液!");
                }));
            }
        }
        private bool judgeDiluteBarCode(string code)
        {
            string decryption = StringUtils.instance.ToDecryption(code);
            string a1 = decryption.Substring(0, 1);

            if (a1 != "B")
            {
                return false;
            }

            string date = decryption.Substring(1, 3);//生产日期、
            int vol = Convert.ToInt32(decryption.Substring(4, 2), 16);//容量
            int serialNum = Convert.ToInt32(decryption.Substring(6, 4), 16);//流水号

            string year = "", month = "", day = "";
            try
            {
                year = StringUtils.instance.reverseDate(date.Substring(0, 1).ToCharArray()[0]);
                month = StringUtils.instance.reverseDate(date.Substring(1, 1).ToCharArray()[0]);
                day = StringUtils.instance.reverseDate(date.Substring(2, 1).ToCharArray()[0]);
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

            int time = int.Parse(year + month + day);
            int check = (11 + time + vol + serialNum) % 7;

            if (decryption.Substring(10, 1) != check.ToString())
            {
                return false;
            }

            diuBarCode = code.Trim();
            AllDiuVol = vol;
            try
            {
                dtime = DateTime.ParseExact(time.ToString(), "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
            }
            catch
            {
                return false;
            }

            return true;
        }
        private bool fillDiluteInfo(string code)
        {
            if (bllsb.GetList("(DilutePos<>null and DiluteNumber = '" + code + "') or DilutePos = " + RegentPos + "").Tables[0].Rows.Count > 0)
            {
                //frmMsgShow.MessageShow("稀释液装载", "稀释液已装载,请先卸载稀释液!");
                return false;
            }
            fillFlag = false;
            //便利数据库查重
            DataTable dt = bllsb.GetList("State=1 and DiluteNumber = '" + code + "'").Tables[0];

            if (dt.Rows.Count > 0)//重复，已经装载过
            {
                Invoke(new Action(() =>
                {
                    string[] temp = dt.Rows[0]["ValiData"].ToString().Split('/');
                    for (int i = 1; i < temp.Length; i++)
                    {
                        if (temp[i].Length < 2)
                            temp[i] = "0" + temp[i];
                    }
                    temp[0] = temp[0] + temp[1] + temp[2];

                    txtDiluteNumber.Text = code.Trim();
                    txtSubstrateAllTest.Text = dt.Rows[0]["AllDiuVol"].ToString();
                    txtSubstrateLastTest.Text = dt.Rows[0]["LeftDiuVol"].ToString();
                    //对比生产日期后一年 和 数据库的有效期
                    DateTime dt1 = dtime.AddYears(1);
                    DateTime dt2 = DateTime.ParseExact(temp[0], "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
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
                    txtDiluteNumber.Text = code.Trim();
                    txtSubstrateAllTest.Text = AllDiuVol.ToString();
                    txtSubstrateLastTest.Text = AllDiuVol.ToString();
                    //对比生产日期后一年 和 今天装载日期后一月
                    DateTime dt1 = dtime.AddYears(1);
                    DateTime dt2 = DateTime.Now.AddMonths(1);
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
            fillFlag = true;
            txtDiluteNumber.Enabled = false;
            return true;
        }
        /// <summary>
        /// 重置各个控件状态
        /// </summary>
        /// <param name="flag">0=重置，1=恢复</param>
        private void initContr(int state = 0)
        {
            changeFlag = false;
            if (state == 0)
            {
                Invoke(new Action(() =>
                {
                    fillFlag = false;
                    txtDiluteNumber.Text = "";
                    txtSubstrateAllTest.Text = "";
                    txtSubstrateLastTest.Text = "";
                }));
            }
            else if (state == 1)
            {
                Invoke(new Action(() =>
                {
                    fillFlag = false;
                    txtDiluteNumber.Text = initCon[0];
                    txtSubstrateAllTest.Text = initCon[1];
                    txtSubstrateLastTest.Text = initCon[2];
                }));
            }
            changeFlag = true;
        }

        private void txtSubstrateAllTest_TextChanged(object sender, EventArgs e)
        {
            if (txtSubstrateAllTest.Text == "")
                return;
            if (txtSubstrateAllTest.Text.Length > 3)
            {
                if (cmbUnit1.SelectedIndex == 1)
                    return;
                cmbUnit1.SelectedIndex = 1;
                cmbUnit2.SelectedIndex = 1;
            }
            else
            {
                if (cmbUnit1.SelectedIndex == 0)
                    return;
                cmbUnit1.SelectedIndex = 0;
                cmbUnit2.SelectedIndex = 0;
            }
        }
    }
}
