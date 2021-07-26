using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common;
using System.Threading;
using System.IO;
using BioBaseCLIA.Run;

namespace BioBaseCLIA.SysMaintenance
{
    public partial class frmInstruMaintenance : frmParent
    {
        #region 变量
        /// <summary>
        /// 底物灌注次数
        /// </summary>
        public int subPerfusionNum = int.Parse(OperateIniFile.ReadInIPara("MaintenancePara", "subPerfusionNum"));
        /// <summary>
        /// 加样管路灌注次数
        /// </summary>
        public int samPerfusionNum = int.Parse(OperateIniFile.ReadInIPara("MaintenancePara", "samPerfusionNum"));
        /// <summary>
        /// 清洗管路灌注次数
        /// </summary>
        public int washPerfusionNum = int.Parse(OperateIniFile.ReadInIPara("MaintenancePara", "washPerfusionNum"));
        /// <summary>
        /// PMT背景值检测对比值
        /// </summary>
        public int PmtCompareValue = int.Parse(OperateIniFile.ReadInIPara("MaintenancePara", "PmtCompareValue"));
        /// <summary>
        /// 底物有效性检测对比值
        /// </summary>
        public int subCompareValue = int.Parse(OperateIniFile.ReadInIPara("MaintenancePara", "subCompareValue"));
        /// <summary>
        /// 清洗盘取放管位置当前孔号
        /// </summary>
        int currentHoleNum = 1;

        /// <summary>
        /// 维护开始线程
        /// </summary>
        private Thread StartThread;

        /// <summary>
        /// 底物与管架配置文件地址
        /// </summary>
        string iniPathSubstrateTube = Directory.GetCurrentDirectory() + "\\SubstrateTube.ini";
        /// <summary>
        /// 试剂盘配置文件地址
        /// </summary>
        string iniPathReagentTrayInfo = Directory.GetCurrentDirectory() + "\\ReagentTrayInfo.ini";
        /// <summary>
        /// 反应盘配置文件地址
        /// </summary>
        string iniPathReactTrayInfo = Directory.GetCurrentDirectory() + "\\ReactTrayInfo.ini";
        /// <summary>
        /// 清洗盘配置文件地址
        /// </summary>
        string iniPathWashTrayInfo = Directory.GetCurrentDirectory() + "\\WashTrayInfo.ini";

        /// <summary>
        /// 初始洗针时间
        /// </summary>
        int FirstNeedleWashTime = int.Parse(OperateIniFile.ReadInIPara("Time", "FirstCleanNeedleTime"));

        /// <summary>
        /// 报警时更改主界面日志按钮的颜色
        /// </summary>
        //public static event Action<int> btnLogColor;
        frmMessageShow frmMsgShow = new frmMessageShow();
        /// <summary>
        /// 指令返回
        /// </summary>
        string BackObj = "";
        /// <summary>
        /// 下位机返回数据
        /// </summary>
        string[] dataRecive = new string[16];
        int substrateNum1;
        int substrateNum2;
        #endregion

        public frmInstruMaintenance()
        {
            InitializeComponent();
        }

        private void frmInstruMaintenance_Load(object sender, EventArgs e)
        {
            NetCom3.Instance.ReceiveHandel += new Action<string>(Instance_ReceiveHandel);
            rdbtnGeneral.Checked = true;
            fbtnStart.Enabled = true;
            fbtnStop.Enabled = false;
        }

        void Instance_ReceiveHandel(string obj)
        {
            if (obj.IsNullOrEmpty())
                return;

            else
            {
                BackObj = obj;
            }
        }
        void ControlEnable(bool Flag)
        {
            chbClearWashTube.Enabled = chbPmt.Enabled = chbSamplePipeline.Enabled = chbSubstrate.Enabled = chbSubstrateTest.Enabled
                = chbWashPipeline.Enabled = txtSamplePipeline.Enabled = txtSubPipeline.Enabled = txtWashPipeline.Enabled = chbClearReactTube.Enabled
            = cmbSubPipeCH.Enabled = cmbSubstrate.Enabled = lblSubPipe1.Enabled = lblSubPipe2.Enabled = Flag;
        }
        private void rdbtnGeneral_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbtnGeneral.Checked)
            {
                ControlEnable(false);
                txtSamplePipeline.Text = samPerfusionNum.ToString();
                txtSubPipeline.Text = subPerfusionNum.ToString();
                txtWashPipeline.Text = washPerfusionNum.ToString();
                chbClearWashTube.Checked = true;
                chbInit.Checked = true;
                chbPmt.Checked = true;
                chbSamplePipeline.Checked = true;
                chbSubstrate.Checked = true;
                chbSubstrateTest.Checked = true;
                chbWashPipeline.Checked = true;
            }
            else
            {
                ControlEnable(true);
                chbClearWashTube.Checked = false;
                //chbInit.Checked = true;
                chbPmt.Checked = false;
                chbSamplePipeline.Checked = false;
                chbSubstrate.Checked = false;
                chbSubstrateTest.Checked = false;
                chbWashPipeline.Checked = false;
            }
        }
        private void fbtnStart_Click(object sender, EventArgs e)
        {
            if (!ControlInit())
                return;
            fbtnStart.Enabled = false;
            fbtnStop.Enabled = true;
            groupBox1.Enabled = false;//add y 20180510
            rdbtnGeneral.Enabled = rdbtnCustom.Enabled = false;//add y 20180510

            StartThread = new Thread(new ParameterizedThreadStart(MaintenanceStart));// GaTestRun  TestRun
            StartThread.IsBackground = true;
            StartThread.Start();

        }
        /// <summary>
        /// 维护开始之前各个控件初始化
        /// </summary>
        bool ControlInit()//this function modify y 20180510
        {
            if (!NetCom3.isConnect)
            {
                frmMsgShow.MessageShow("仪器维护", "网口未连接，请连接网络！");
                return false;
            }
           substrateNum1 = int.Parse(OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", iniPathSubstrateTube));
           substrateNum2 = int.Parse(OperateIniFile.ReadIniData("Substrate2", "LeftCount", "0", iniPathSubstrateTube));
            if (rdbtnGeneral.Checked)
            {
                if (substrateNum1 > 0)
                {
                    cmbSubstrate.SelectedIndex = 0;
                    cmbSubPipeCH.SelectedIndex = 0;
                }
                else
                {
                    cmbSubstrate.SelectedIndex = 1;
                    cmbSubPipeCH.SelectedIndex = 1;
                }
            }
            else
            {
                if (chbSamplePipeline.Checked)//this block add y 20180510
                {
                    if (txtSamplePipeline.Text.Trim() == "")
                    {
                        frmMsgShow.MessageShow("仪器维护", "请选择加样管路灌注功能的次数！");
                        txtSamplePipeline.Focus();
                        return false;
                    }
                }//this block end
                if (chbWashPipeline.Checked)//this block add y 20180510
                {
                    if (txtWashPipeline.Text.Trim() == "")
                    {
                        frmMsgShow.MessageShow("仪器维护", "请选择清洗管路灌注功能的次数！");
                        txtWashPipeline.Focus();
                        return false;
                    }
                }//this block end
                if (chbPmt.Checked)//this block add y 20180510
                {
                    if (txtPMT.Text.Trim() == "")
                    {
                        frmMsgShow.MessageShow("仪器维护", "请选择PMT背景检测功能的参数！");
                        txtPMT.Focus();
                        return false;
                    }
                }//this block end

                if (chbSubstrate.Checked)
                {
                    if (txtSubPipeline.Text.Trim() == "")//this block add y 20180510
                    {
                        frmMsgShow.MessageShow("仪器维护", "请选择底物管路灌注功能的次数！");
                        txtSubPipeline.Focus();
                        return false;
                    }//this block end
                    if (cmbSubstrate.SelectedItem == null)
                    {
                        frmMsgShow.MessageShow("仪器维护", "请选择底物管路灌注功能的底物管路！");
                        cmbSubstrate.Focus();
                        return false;
                    }
                    if (cmbSubstrate.SelectedIndex == 0 && substrateNum1 == 0)
                    {
                        frmMsgShow.MessageShow("仪器维护", "管路1无底物，请装载！");
                        return false;
                    }
                    if (cmbSubstrate.SelectedIndex == 1 && substrateNum2 == 0)
                    {
                        frmMsgShow.MessageShow("仪器维护", "管路2无底物，请装载！");
                        return false;
                    }
                }
                if (chbSubstrateTest.Checked)
                {
                    //if (txtSubTest.Text.Trim() == "")//this block add y 20180510
                    //{
                    //    frmMsgShow.MessageShow("仪器维护", "请选择底物有效性检测功能的参数！");
                    //    txtSubTest.Focus();
                    //    return false;
                    //}//this block end
                    if (cmbSubPipeCH.SelectedItem == null)
                    {
                        frmMsgShow.MessageShow("仪器维护", "请选择底物有效性检测功能的底物管路！");
                        cmbSubPipeCH.Focus();
                        return false;
                    }
                    if (cmbSubstrate.SelectedIndex == 0 && substrateNum1 == 0)
                    {
                        frmMsgShow.MessageShow("仪器维护", "管路1无底物，请装载！");
                        return false;
                    }
                    if (cmbSubstrate.SelectedIndex == 1 && substrateNum2 == 0)
                    {
                        frmMsgShow.MessageShow("仪器维护", "管路2无底物，请装载！");
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 仪器开始维护具体方法
        /// </summary>
        /// <param name="obj"></param>
        void MaintenanceStart(object obj)
        {
            if (rdbtnGeneral.Checked)
            {
                if (InstruInit())
                {
                    this.BeginInvoke(new Action(() => { txtInfo.AppendText("仪器初始化完成。。。" + Environment.NewLine); }));
                }
                else
                {
                    this.BeginInvoke(new Action(() => { txtInfo.AppendText("仪器初始化失败。。。" + Environment.NewLine); }));
                }
                Thread.Sleep(1000);
                washTrayTubeClear();
                Thread.Sleep(1000);
                ClearReactTube();
                Thread.Sleep(1000);
                SamplePipeline();
                Thread.Sleep(1000);
                WashPipeline();
                Thread.Sleep(1000);
                SubstratePipeline();
                Thread.Sleep(1000);
                PMTTest();
                Thread.Sleep(1000);
                SubstrateTest();
            }
            else if (rdbtnCustom.Checked)
            {
                //仪器是否初始化
                if (chbInit.Checked)
                {
                    if (InstruInit())
                    {
                        this.BeginInvoke(new Action(() => { txtInfo.AppendText("仪器初始化完成。" + Environment.NewLine); }));
                    }
                    else
                    {
                        this.BeginInvoke(new Action(() => { txtInfo.AppendText("仪器初始化失败。" + Environment.NewLine); }));
                    }
                }
                //清空清洗盘反应管
                if (chbClearWashTube.Checked)
                {
                    washTrayTubeClear();
                }
                //清空反应盘反应管
                if (chbClearReactTube.Checked)
                {
                    ClearReactTube();
                }
                //是否进行加样管路灌注
                if (chbSamplePipeline.Checked)
                {
                    SamplePipeline();
                }
                //是否进行清洗管路灌注
                if (chbWashPipeline.Checked)
                {
                    WashPipeline();
                }
                //是否进行底物管路灌注
                if (chbSubstrate.Checked)
                {
                    SubstratePipeline();
                }
                //是否进行PMT背景检测
                if (chbPmt.Checked)
                {
                    PMTTest();
                }
                //是否进行底物有效性检测
                if (chbSubstrateTest.Checked)
                {
                    SubstrateTest();
                }
            }
            else if (rdbDaily.Checked)
            {
                //仪器初始化
                if (chbInit.Checked)
                {
                    if (InstruInit())
                    {
                        this.BeginInvoke(new Action(() => { txtInfo.AppendText("仪器初始化完成。" + Environment.NewLine); }));
                    }
                    else
                    {
                        this.BeginInvoke(new Action(() => { txtInfo.AppendText("仪器初始化失败。" + Environment.NewLine); }));
                    }
                }
                //清空清洗盘反应管
                if (chbClearWashTube.Checked)
                {
                    washTrayTubeClear();
                }
                //进行底物管路灌注
                if (chbSubstrate.Checked)
                {
                    SubstratePipeline();
                }
                //进行底物有效性检测
                if (chbSubstrateTest.Checked)
                {
                    SubstrateTest();
                }
            }
            BeginInvoke(new Action(() =>
            {
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                txtInfo.AppendText("----------维护操作已结束----------" + Environment.NewLine);//add y 20180510
                groupBox1.Enabled = true;//add y 20180510
                rdbtnGeneral.Enabled = rdbtnCustom.Enabled = true;//add y 20180510
            }));
        }
        /// <summary>
        /// 仪器初始化
        /// </summary>
        bool InstruInit()
        {
            this.BeginInvoke(new Action(() => { txtInfo.AppendText("仪器正在初始化。。。" + Environment.NewLine); }));
            //仪器初始化
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 F1 02"), 5);
            if (!NetCom3.Instance.SingleQuery())
            {
                return false;
            }
            #region 判断各个模组是否初始化成功
            if (NetCom3.Instance.ErrorMessage != null)
            {
                //2018-09-06 zlx mod
                DialogResult r = frmMsgShow.MessageShow("仪器初始化", NetCom3.Instance.ErrorMessage);
                return false;
            }
            /*
            int[] HandData = new int[16];
            while (dataRecive[0] == null)
            {
                Thread.Sleep(10);
            }
            HandData = NetCom3.converTo10(dataRecive);
            if (HandData[4] != 255)
            {
                frmMsgShow.MessageShow("仪器初始化", "计数器模组初始化失败！");
                return false;
            }
            if (HandData[5] != 255)
            {
                frmMsgShow.MessageShow("仪器初始化", "抓手模组初始化失败！");
                return false;
            }
            if (HandData[6] != 255)
            {
                frmMsgShow.MessageShow("仪器初始化", "加样机模组初始化失败！");
                return false;
            }
            if (HandData[7] != 255)
            {
                frmMsgShow.MessageShow("仪器初始化", "清洗模组初始化失败！");
                return false;
            }
             */
            #endregion
            //currentHoleNum = int.Parse(OperateIniFile.ReadInIPara("OtherPara", "washCurrentHoleNum"));
            //currentHoleNum孔转到清洗盘取放管位置
            //NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 02 " + currentHoleNum.ToString("x2")), 2);
            //if (!NetCom3.Instance.WashQuery())
            //{
             //   return false;
            //}
            return true;

        }
        /// <summary>
        /// 清空清洗盘的反应管
        /// </summary>
        //2019.5.16  hly modify
        bool washTrayTubeClear()
        {
            this.BeginInvoke(new Action(() => { txtInfo.AppendText("清空清洗盘的反应管。。。" + Environment.NewLine); }));
            DataTable dtWashTrayIni = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
            for (int i = 0; i < dtWashTrayIni.Rows.Count; i++)
            {
                if (i != 0)
                {
                    //清洗盘顺时针旋转一位
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-1).ToString("X2").Substring(6, 2)), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        fbtnStart.Enabled = true;
                        fbtnStop.Enabled = false;
                        return false;
                    }
                    currentHoleNum = currentHoleNum - 1;
                    if (currentHoleNum <= 0)
                    {
                        currentHoleNum = currentHoleNum + 30;
                    }
                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                    dtWashTrayIni = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
                    DataTable dtTemp = new DataTable();
                    dtTemp = dtWashTrayIni.Copy();
                    //清洗盘状态列表中添加反应盘位置字段，赋值需多赋值一个字段。 
                    for (int j = 1; j < 2; j++)
                        dtWashTrayIni.Rows[dtWashTrayIni.Rows.Count - 1][j] = dtTemp.Rows[0][j];
                    for (int k = 0; k < dtWashTrayIni.Rows.Count - 1; k++)
                    {
                        for (int j = 1; j < 2; j++)
                        {
                            dtWashTrayIni.Rows[k][j] = dtTemp.Rows[k + 1][j];
                        }
                    }

                    OperateIniFile.WriteConfigToFile("[TubePosition]", iniPathWashTrayInfo, dtWashTrayIni);
                }
                #region 移管手取放管位置取管扔废管
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
                LogFile.Instance.Write("==============  " + currentHoleNum + "  扔管");
                if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                {
                    fbtnStart.Enabled = true;
                    fbtnStop.Enabled = false;
                    return false;
                }
                OperateIniFile.WriteIniData("TubePosition", "No1", "0", iniPathWashTrayInfo);
                #endregion
            }
            this.BeginInvoke(new Action(() => { txtInfo.AppendText("清洗盘反应管清空完成。。。" + Environment.NewLine); }));
            return true;
        }
        /// <summary>
        /// 清空反应盘中的反应管
        /// </summary>
        void ClearReactTube()
        {
            this.BeginInvoke(new Action(() => { txtInfo.AppendText("清空温育盘使用过的反应管。。。" + Environment.NewLine); }));
            DataTable dtReactTrayIni = OperateIniFile.ReadConfig(iniPathReactTrayInfo);
            #region 反应盘清除使用过的反应管
            for (int i = 0; i < dtReactTrayIni.Rows.Count; i++)
            {
                if (int.Parse(dtReactTrayIni.Rows[i][1].ToString()) > 1)
                {
                    int pos = int.Parse(dtReactTrayIni.Rows[i][0].ToString().Substring(2));
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 " + pos.ToString("x2")), 1);
                    if (!NetCom3.Instance.MoveQuery())
                    {
                        fbtnStart.Enabled = true;
                        fbtnStop.Enabled = false;
                        return;
                    }
                    //配置文件修改
                    OperateIniFile.WriteIniData("ReactTrayInfo", "no" + pos, "0", iniPathReactTrayInfo);
                }
            }
            #endregion
            this.BeginInvoke(new Action(() => { txtInfo.AppendText("清空温育盘使用过的反应管完成。。。" + Environment.NewLine); }));
        }
        /// <summary>
        /// 加样管路灌注
        /// </summary>
        void SamplePipeline()
        {
            this.BeginInvoke(new Action(() => { txtInfo.AppendText("加样管路灌注。。。" + Environment.NewLine); }));
            int Num = int.Parse(txtSamplePipeline.Text.Trim());
            while (Num > 0)
            {

                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 08"), 0);
                if (!NetCom3.Instance.SPQuery())
                {
                    fbtnStart.Enabled = true;
                    fbtnStop.Enabled = false;
                    return;
                }
                Num--;
            }
            this.BeginInvoke(new Action(() => { txtInfo.AppendText("加样管路灌注完成。。。" + Environment.NewLine); }));
        }

        /// <summary>
        /// 清洗管路灌注
        /// </summary>
        //2019.5.8  hly  modify
        void WashPipeline()
        {
            washTrayTubeClear();
            this.BeginInvoke(new Action(() => { txtInfo.AppendText("清洗管路灌注。。。" + Environment.NewLine); }));
            int Num = int.Parse(txtWashPipeline.Text.Trim());
            //注液1位置
            int pos1 = 6;
            int pos2 = 10;
            int pos3 = 14;
            //管架取管位置
            int getTubePos;
            int plate;
            int column;
            int hole;

            #region 注液1位置放管,在6号孔
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-6).ToString("X2").Substring(6, 2)), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }
            currentHoleNum = currentHoleNum - (1 - pos1);
            if (currentHoleNum <= 0)
            {
                currentHoleNum = 30 + currentHoleNum;
            }
            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
            getTubePos = int.Parse(OperateIniFile.ReadIniData("Tube", "TubePos", "", iniPathSubstrateTube));         
            plate = getTubePos % 88 == 0 ? getTubePos / 88 - 1 : getTubePos / 88;//几号板
            column = getTubePos % 11 == 0 ? getTubePos / 11 - (plate * 8) : getTubePos / 11 + 1 - (plate * 8);
            hole = getTubePos % 11 == 0 ? 11 : getTubePos % 11;
            int iNeedCool = 0; 
         AgainNewMove:          
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06 " + plate.ToString("x2") + " " + column.ToString("x2") + " " + hole.ToString("x2")), 1);
            if (!NetCom3.Instance.MoveQuery())
            {
                #region
                if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull)
                {
                    iNeedCool++;
                    bool isMoevtoNextBoard;
                    if (iNeedCool < 12)
                    {
                        if (iNeedCool == 3 || iNeedCool == 6 || iNeedCool == 9)
                        {
                            isMoevtoNextBoard = true;
                            getTubePos = BoardNextPos(getTubePos, true, out isMoevtoNextBoard);
                        }
                        else
                        {
                            isMoevtoNextBoard = false;
                            getTubePos = BoardNextPos(getTubePos, false, out isMoevtoNextBoard);
                        }

                        plate = getTubePos % 88 == 0 ? getTubePos / 88 - 1 : getTubePos / 88;//几号板
                        column = getTubePos % 11 == 0 ? getTubePos / 11 - (plate * 8) : getTubePos / 11 + 1 - (plate * 8);
                        hole = getTubePos % 11 == 0 ? 11 : getTubePos % 11;
                        goto AgainNewMove;
                    }
                    else
                    {
                        frmMsgShow.MessageShow("组合测试", "移管手多次抓空，请装载管架！");
                        //NewWashEnd();
                        return;
                    }
                }
                else
                {
                    return;
                }
                #endregion
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }
            OperateIniFile.WriteIniData("TubePosition", "No6", "1", iniPathWashTrayInfo);
            #region 取放管成功，相关配置文件修改
            List<int> lisTubeNum = new List<int>();
            lisTubeNum = QueryTubeNum();
            //移管手要夹的下一个管架位置
            int NextPos = getTubePos + 1;
            //管架中第一个装载管架的索引
            int firstTubeIndex = lisTubeNum.FindIndex(ty => ty <= 88 && ty > 0);
            for (int i = 1; i <= lisTubeNum.Count; i++)
            {
                if (NextPos == i * 88 + 1)
                {
                    NextPos = firstTubeIndex * 88 + (88 - lisTubeNum[firstTubeIndex]) + 1;
                }
            }
            OperateIniFile.WriteIniData("Tube", "TubePos", NextPos.ToString(), iniPathSubstrateTube);
            int TubeRack = (getTubePos) / 88;
            int curTube = (getTubePos) % 88;
            if (curTube == 0 && getTubePos != 0)
            {
                TubeRack = TubeRack - 1;
                curTube = 88;
            }
            //那个架子减了一个管
            OperateIniFile.WriteIniData("Tube", "Pos" + (TubeRack + 1).ToString(), (88 - curTube).ToString(), iniPathSubstrateTube);
            //清洗盘配置文件修改
            //OperateIniFile.WriteIniData("TubePosition", "No1", "1", iniPathWashTrayInfo);
            #endregion
            #endregion

            #region 注液2位置放管,在10号孔
            //清洗盘注液2位置转到取放管位置(顺时针旋转)，4个位置
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (pos1 - pos2).ToString("X2").Substring(6, 2)), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }
            currentHoleNum = currentHoleNum - (pos1 - pos2);
            //若当前管号等于0，说明转过来的孔号为30
            if (currentHoleNum <= 0)
            {
                currentHoleNum = 30 + currentHoleNum;
            }
            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
            getTubePos = int.Parse(OperateIniFile.ReadIniData("Tube", "TubePos", "", iniPathSubstrateTube));
            plate = getTubePos % 88 == 0 ? getTubePos / 88 - 1 : getTubePos / 88;//几号板
            column = getTubePos % 11 == 0 ? getTubePos / 11 - (plate * 8) : getTubePos / 11 + 1 - (plate * 8);
            hole = getTubePos % 11 == 0 ? 11 : getTubePos % 11;
            //管架取管到清洗盘
            AgainNewMove1:
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06 " + plate.ToString("x2") + " " + column.ToString("x2") + " " + hole.ToString("x2")), 1);
            if (!NetCom3.Instance.MoveQuery())
            {
                #region
                if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull)
                {
                    iNeedCool++;
                    bool isMoevtoNextBoard;
                    if (iNeedCool < 12)
                    {
                        if (iNeedCool == 3 || iNeedCool == 6 || iNeedCool == 9)
                        {
                            isMoevtoNextBoard = true;
                            getTubePos = BoardNextPos(getTubePos, true, out isMoevtoNextBoard);
                        }
                        else
                        {
                            isMoevtoNextBoard = false;
                            getTubePos = BoardNextPos(getTubePos, false, out isMoevtoNextBoard);
                        }

                        plate = getTubePos % 88 == 0 ? getTubePos / 88 - 1 : getTubePos / 88;//几号板
                        column = getTubePos % 11 == 0 ? getTubePos / 11 - (plate * 8) : getTubePos / 11 + 1 - (plate * 8);
                        hole = getTubePos % 11 == 0 ? 11 : getTubePos % 11;
                        goto AgainNewMove1;
                    }
                    else
                    {
                        frmMsgShow.MessageShow("组合测试", "移管手多次抓空，请装载管架！");
                        //NewWashEnd();
                        return;
                    }
                }
                else
                {
                    return;
                }
                #endregion
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }
            OperateIniFile.WriteIniData("TubePosition", "No10", "1", iniPathWashTrayInfo);
            #region 取放管成功 相关配置文件修改
            lisTubeNum = new List<int>();
            lisTubeNum = QueryTubeNum();
            //移管手要夹的下一个管架位置
            NextPos = getTubePos + 1;
            //管架中第一个装载管架的索引
            firstTubeIndex = lisTubeNum.FindIndex(ty => ty <= 88 && ty > 0);
            for (int i = 1; i <= lisTubeNum.Count; i++)
            {
                if (NextPos == i * 88 + 1)
                {
                    NextPos = firstTubeIndex * 88 + (88 - lisTubeNum[firstTubeIndex]) + 1;
                }
            }
            OperateIniFile.WriteIniData("Tube", "TubePos", NextPos.ToString(), iniPathSubstrateTube);
            TubeRack = (getTubePos) / 88;
            curTube = (getTubePos) % 88;
            if (curTube == 0 && getTubePos != 0)
            {
                TubeRack = TubeRack - 1;
                curTube = 88;
            }
            //那个架子减了一个管
            OperateIniFile.WriteIniData("Tube", "Pos" + (TubeRack + 1).ToString(), (88 - curTube).ToString(), iniPathSubstrateTube);
            //清洗盘配置文件修改
            //OperateIniFile.WriteIniData("TubePosition", "No1", "1", iniPathWashTrayInfo);
            #endregion
            #endregion

            #region 注液3位置放管，在14号孔
            //清洗盘注液2位置转到取放管位置(顺时针旋转)
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (pos2 - pos3).ToString("X2").Substring(6, 2)), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }
            currentHoleNum = currentHoleNum - (pos2 - pos3);
            if (currentHoleNum <= 0)
            {
                currentHoleNum = 30 + currentHoleNum;
            }
            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());           
            getTubePos = int.Parse(OperateIniFile.ReadIniData("Tube", "TubePos", "", iniPathSubstrateTube));
            plate = getTubePos % 88 == 0 ? getTubePos / 88 - 1 : getTubePos / 88;//几号板
            column = getTubePos % 11 == 0 ? getTubePos / 11 - (plate * 8) : getTubePos / 11 + 1 - (plate * 8);
            hole = getTubePos % 11 == 0 ? 11 : getTubePos % 11;
            AgainNewMove2:
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06 " + plate.ToString("x2") + " " + column.ToString("x2") + " " + hole.ToString("x2")), 1);
            if (!NetCom3.Instance.MoveQuery())
            {
                #region
                if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull)
                {
                    iNeedCool++;
                    bool isMoevtoNextBoard;
                    if (iNeedCool < 12)
                    {
                        if (iNeedCool == 3 || iNeedCool == 6 || iNeedCool == 9)
                        {
                            isMoevtoNextBoard = true;
                            getTubePos = BoardNextPos(getTubePos, true, out isMoevtoNextBoard);
                        }
                        else
                        {
                            isMoevtoNextBoard = false;
                            getTubePos = BoardNextPos(getTubePos, false, out isMoevtoNextBoard);
                        }

                        plate = getTubePos % 88 == 0 ? getTubePos / 88 - 1 : getTubePos / 88;//几号板
                        column = getTubePos % 11 == 0 ? getTubePos / 11 - (plate * 8) : getTubePos / 11 + 1 - (plate * 8);
                        hole = getTubePos % 11 == 0 ? 11 : getTubePos % 11;
                        goto AgainNewMove2;
                    }
                    else
                    {
                        frmMsgShow.MessageShow("组合测试", "移管手多次抓空，请装载管架！");
                        //NewWashEnd();
                        return;
                    }
                }
                else
                {
                    return;
                }
                #endregion
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }
            OperateIniFile.WriteIniData("TubePosition", "No14", "1", iniPathWashTrayInfo);
            #region 取放管成功 相关配置文件修改
            lisTubeNum = new List<int>();
            lisTubeNum = QueryTubeNum();
            //移管手要夹的下一个管架位置
            NextPos = getTubePos + 1;
            //管架中第一个装载管架的索引
            firstTubeIndex = lisTubeNum.FindIndex(ty => ty <= 88 && ty > 0);
            for (int i = 1; i <= lisTubeNum.Count; i++)
            {
                if (NextPos == i * 88 + 1)
                {
                    NextPos = firstTubeIndex * 88 + (88 - lisTubeNum[firstTubeIndex]) + 1;
                }
            }
            OperateIniFile.WriteIniData("Tube", "TubePos", NextPos.ToString(), iniPathSubstrateTube);
            TubeRack = (getTubePos) / 88;
            curTube = (getTubePos) % 88;
            if (curTube == 0 && getTubePos != 0)
            {
                TubeRack = TubeRack - 1;
                curTube = 88;
            }
            //那个架子减了一个管
            OperateIniFile.WriteIniData("Tube", "Pos" + (TubeRack + 1).ToString(), (88 - curTube).ToString(), iniPathSubstrateTube);
            //清洗盘配置文件修改
            //OperateIniFile.WriteIniData("TubePosition", "No1", "1", iniPathWashTrayInfo);
            #endregion
            #endregion

            #region 清洗盘回到原来的位置，逆时针旋转14个孔位

            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (pos3).ToString("X2")), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }

            currentHoleNum = currentHoleNum - pos3 + 1;
            //若当前管号等于0，说明转过来的孔号为30
            if (currentHoleNum > 30)
            {
                currentHoleNum = currentHoleNum - 30;
            }
            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
            #endregion

            while (Num > 0)
            {
                #region 注液
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 11 10"), 2);
                if (!NetCom3.Instance.WashQuery())
                {
                    fbtnStart.Enabled = true;
                    fbtnStop.Enabled = false;
                    return;
                }

                #endregion
                #region 清洗盘顺时针旋转1位
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-1).ToString("X2").Substring(6, 2)), 2);
                if (!NetCom3.Instance.WashQuery())
                {
                    fbtnStart.Enabled = true;
                    fbtnStop.Enabled = false;
                    return;
                }

                currentHoleNum = currentHoleNum + 1;
                if (currentHoleNum > 30)
                {
                    currentHoleNum = currentHoleNum - 30;
                }
                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                
                #region  吸液
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 01"), 2);
                if (!NetCom3.Instance.WashQuery())
                {
                    fbtnStart.Enabled = true;
                    fbtnStop.Enabled = false;
                    return;
                }
                #endregion
                #endregion

                #region 清洗盘逆时针转回注液位置
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (1).ToString("X2")), 2);
                if (!NetCom3.Instance.WashQuery())
                {
                    fbtnStart.Enabled = true;
                    fbtnStop.Enabled = false;
                    return;
                }

                currentHoleNum = currentHoleNum - 1;
                if (currentHoleNum <= 0)
                {
                    currentHoleNum = currentHoleNum + 30;
                }
                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());               
                #endregion
                Num--;
            }
            #region 扔废管

            #region 清洗盘逆时针旋转16位扔注液3位置反应管
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (16).ToString("X2")), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }

            currentHoleNum = currentHoleNum + 13;
            if (currentHoleNum > 30)
            {
                currentHoleNum = currentHoleNum - 30;
            }
            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());           
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
            if (!NetCom3.Instance.MoveQuery())
            {
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }
            OperateIniFile.WriteIniData("TubePosition", "No14", "0", iniPathWashTrayInfo);
            #endregion
            #region 清洗盘逆时针旋转4位扔注液2位置反应管
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (4).ToString("X2")), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }

            currentHoleNum = currentHoleNum - 4;
            if (currentHoleNum > 30)
            {
                currentHoleNum = currentHoleNum - 30;
            }
            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());         
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
            if (!NetCom3.Instance.MoveQuery())
            {
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }
            OperateIniFile.WriteIniData("TubePosition", "No10", "0", iniPathWashTrayInfo);
            #endregion
            #region 清洗盘逆时针旋转4位扔注液1位置反应管
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (4).ToString("X2")), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }
            currentHoleNum = currentHoleNum - 4;
            if (currentHoleNum > 30)
            {
                currentHoleNum = currentHoleNum - 30;
            }
            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());           
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
            if (!NetCom3.Instance.MoveQuery())
            {
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }
            OperateIniFile.WriteIniData("TubePosition", "No6", "0", iniPathWashTrayInfo);
            #endregion
            #endregion
            this.BeginInvoke(new Action(() => { txtInfo.AppendText("清洗管路灌注完成。" + Environment.NewLine); }));
        }
        /// <summary>
        /// 查询四个管架中管的个数
        /// </summary>
        /// <returns></returns>
        List<int> QueryTubeNum()
        {
            List<int> lisTubeNum = new List<int>();
            lisTubeNum.Add(int.Parse(OperateIniFile.ReadIniData("Tube", "Pos1", "", iniPathSubstrateTube)));
            lisTubeNum.Add(int.Parse(OperateIniFile.ReadIniData("Tube", "Pos2", "", iniPathSubstrateTube)));
            lisTubeNum.Add(int.Parse(OperateIniFile.ReadIniData("Tube", "Pos3", "", iniPathSubstrateTube)));
            lisTubeNum.Add(int.Parse(OperateIniFile.ReadIniData("Tube", "Pos4", "", iniPathSubstrateTube)));
            return lisTubeNum;
        } 
        /// <summary>
        /// 底物管路灌注
        /// </summary>
        //2019.5.9  hly  modify
        void SubstratePipeline()
        {
            this.BeginInvoke(new Action(() => { txtInfo.AppendText("底物管路灌注。。。" + Environment.NewLine); }));
            int pos1 = 19;
            //管架取管位置
            int getTubePos;
            int plate;
            int column;
            int hole;
            int Num = int.Parse(txtSubPipeline.Text.Trim());
            string subPipe = "0";
            Invoke(new Action(() =>
            {
                if (cmbSubstrate.SelectedItem.ToString() == "1")
                {
                    subPipe = "1";
                }
                else if (cmbSubstrate.SelectedItem.ToString() == "2")
                {
                    subPipe = "2";
                }
            }));
            #region 清洗盘顺时针18位，然后放管
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (1 - pos1).ToString("X2").Substring(6,2)), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }
            currentHoleNum = currentHoleNum + (pos1 - 1);
            //若当前管号等于0，说明转过来的孔号为30
            if (currentHoleNum > 30)
            {
                currentHoleNum = currentHoleNum - 30;
            }
            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
            //OperateIniFile.WriteIniData("TubePosition", "No19", "1", iniPathWashTrayInfo);
            getTubePos = int.Parse(OperateIniFile.ReadIniData("Tube", "TubePos", "", iniPathSubstrateTube));
            plate = getTubePos % 88 == 0 ? getTubePos / 88 - 1 : getTubePos / 88;//几号板
            column = getTubePos % 11 == 0 ? getTubePos / 11 - (plate * 8) : getTubePos / 11 + 1 - (plate * 8);
            hole = getTubePos % 11 == 0 ? 11 : getTubePos % 11;
            int iNeedCool = 0;
            //夹新管到清洗盘
            AgainNewMove3:
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06 " + plate.ToString("x2") + " " + column.ToString("x2") + " " + hole.ToString("x2")), 1);
            if (!NetCom3.Instance.MoveQuery())
            {
                if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull)
                {
                    iNeedCool++;
                    bool isMoevtoNextBoard;
                    if (iNeedCool < 12)
                    {
                        if (iNeedCool == 3 || iNeedCool == 6 || iNeedCool == 9)
                        {
                            isMoevtoNextBoard = true;
                            getTubePos = BoardNextPos(getTubePos, true, out isMoevtoNextBoard);
                        }
                        else
                        {
                            isMoevtoNextBoard = false;
                            getTubePos = BoardNextPos(getTubePos, false, out isMoevtoNextBoard);
                        }

                        plate = getTubePos % 88 == 0 ? getTubePos / 88 - 1 : getTubePos / 88;//几号板
                        column = getTubePos % 11 == 0 ? getTubePos / 11 - (plate * 8) : getTubePos / 11 + 1 - (plate * 8);
                        hole = getTubePos % 11 == 0 ? 11 : getTubePos % 11;
                        goto AgainNewMove3;
                    }
                    else
                    {
                        frmMsgShow.MessageShow("组合测试", "移管手多次抓空，请装载管架！");
                        //NewWashEnd();
                        return;
                    }
                }
                else
                {
                    return;
                }
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }
            OperateIniFile.WriteIniData("TubePosition", "No19", "1", iniPathWashTrayInfo);
            #region 取放管成功，相关配置文件修改
            List<int> lisTubeNum = new List<int>();
            lisTubeNum = QueryTubeNum();
            //移管手要夹的下一个管架位置
            int NextPos = getTubePos + 1;
            //管架中第一个装载管架的索引
            int firstTubeIndex = lisTubeNum.FindIndex(ty => ty <= 88 && ty > 0);
            for (int i = 1; i <= lisTubeNum.Count; i++)
            {
                if (NextPos == i * 88 + 1)
                {
                    NextPos = firstTubeIndex * 88 + (88 - lisTubeNum[firstTubeIndex]) + 1;
                }
            }

            OperateIniFile.WriteIniData("Tube", "TubePos", NextPos.ToString(), iniPathSubstrateTube);
            int TubeRack = (getTubePos) / 88;
            int curTube = (getTubePos) % 88;
            if (curTube == 0 && getTubePos != 0)
            {
                TubeRack = TubeRack - 1;
                curTube = 88;
            }
            //那个架子减了一个管
            OperateIniFile.WriteIniData("Tube", "Pos" + (TubeRack + 1).ToString(), (88 - curTube).ToString(), iniPathSubstrateTube);
            //清洗盘配置文件修改
            //OperateIniFile.WriteIniData("TubePosition", "No1", "1", iniPathWashTrayInfo);
            #endregion
            #endregion
            #region 清洗盘逆时针旋转19位，回到19号孔，加底物位置
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (pos1).ToString("X2")), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }

            currentHoleNum = currentHoleNum - pos1 + 1;
            //若当前管号等于0，说明转过来的孔号为30
            if (currentHoleNum > 30)
            {
                currentHoleNum = currentHoleNum - 30;
            }
            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
            #endregion
            while (Num > 0)
            {
                #region 底物注液
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 01 " + subPipe + "0"), 2);
                if (!NetCom3.Instance.WashQuery())
                {
                    fbtnStart.Enabled = true;
                    fbtnStop.Enabled = false;
                    return;
                }
                if (subPipe == "1")
                {
                    substrateNum1 = substrateNum1 - 1;
                    OperateIniFile.WriteIniData("Substrate1", "LeftCount", substrateNum1.ToString(), iniPathSubstrateTube);
                }
                else
                {
                    substrateNum2 = substrateNum2 - 1;
                    OperateIniFile.WriteIniData("Substrate1", "LeftCount", substrateNum2.ToString(), iniPathSubstrateTube);
                }
                #endregion
                #region 清洗盘顺时针旋转2位，底物吸液位置
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-2).ToString("X2").Substring(6, 2)), 2);
                if (!NetCom3.Instance.WashQuery())
                {
                    fbtnStart.Enabled = true;
                    fbtnStop.Enabled = false;
                    return;
                }
                currentHoleNum = currentHoleNum + 2;
                //若当前管号等于0，说明转过来的孔号为30
                if (currentHoleNum > 30)
                {
                    currentHoleNum = currentHoleNum - 30;
                }
                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());               
                #region 吸液
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 01"), 2);
                if (!NetCom3.Instance.WashQuery())
                {
                    fbtnStart.Enabled = true;
                    fbtnStop.Enabled = false;
                    return;
                }
                #endregion
                #endregion
                #region 清洗盘转回底物注液位置，19号位
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (2).ToString("X2")), 2);
                if (!NetCom3.Instance.WashQuery())
                {
                    fbtnStart.Enabled = true;
                    fbtnStop.Enabled = false;
                    return;
                }

                currentHoleNum = currentHoleNum - 2;
                //若当前管号等于0，说明转过来的孔号为30
                if (currentHoleNum <= 0)
                {
                    currentHoleNum = currentHoleNum + 30;
                }
                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());   
                #endregion
                Num--;
                BeginInvoke(new Action(() =>
                {
                    txtSubPipeline.Text = Num.ToString();
                }));
            }
            #region 加底物位置扔废管
            //清洗盘逆时针旋转12位转到取放管位置
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (11).ToString("X2")), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }

            currentHoleNum = currentHoleNum + 18;
            //若当前管号等于0，说明转过来的孔号为30
            if (currentHoleNum > 30)
            {
                currentHoleNum = currentHoleNum - 30;
            }
            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
            if (!NetCom3.Instance.MoveQuery())
            {
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }
            OperateIniFile.WriteIniData("TubePosition", "No19", "0", iniPathWashTrayInfo);
            #endregion
            this.BeginInvoke(new Action(() => { txtInfo.AppendText("底物管路灌注完成。。。" + Environment.NewLine); }));
        }
        /// <summary>
        /// PMT背景值检测
        /// </summary>
        void PMTTest()
        {
            this.BeginInvoke(new Action(() => { txtInfo.AppendText("正在进行PMT背景值检测。。。" + Environment.NewLine); }));
            //int PMT = 0;
            BackObj = "";
            //发送单独的读数指令
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 00 01"), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }
            else
            {
                int delay = 1000;
                while (!BackObj.Contains("EB 90 31 A3") && delay > 0)
                {
                    NetCom3.Delay(10);
                    delay = delay - 10;
                }
                if (BackObj.Contains("EB 90 31 A3"))
                {
                    string temp = BackObj.Substring(BackObj.Length - 16).Replace(" ", "");
                    temp = Convert.ToInt64(temp, 16).ToString();
                    if (double.Parse(temp) > Math.Pow(10, 5))
                        temp = ((int)GetPMT(double.Parse(temp))).ToString();
                    //textReadShow.AppendText(DateTime.Now.ToString("HH-mm-ss") + ": " + "PMT背景值：" + temp + Environment.NewLine);
                    this.BeginInvoke(new Action(() => { txtPMT.Text = temp; }));
                }
            }
            //PMT = int.Parse(BackObj);
            //if (double.Parse(PMT.ToString()) > Math.Pow(10, 5))
            //    PMT = (int)GetPMT(double.Parse(PMT.ToString()));
            //BeginInvoke(new Action(() =>
            //{
            //    txtPMT.Text = PMT.ToString();
            //}));
            this.BeginInvoke(new Action(() => { txtInfo.AppendText("PMT背景值检测完成。。。" + Environment.NewLine); }));
        }
        /// <summary>
        /// 底物有效性检测
        /// </summary>
        //2019.5.10  hly  modify
        void SubstrateTest()
        {
            this.BeginInvoke(new Action(() => { txtInfo.AppendText("正在进行底物有效性检测。。。" + Environment.NewLine); }));
            //int subPMT = 0;
            int pos1 = 19;
            //管架取管位置
            int getTubePos;
            int plate;
            int column;
            int hole;
            string subPipe = "0";
            Invoke(new Action(() =>
            {
                if (cmbSubPipeCH.SelectedItem.ToString() == "1")
                {
                    subPipe = "1";
                }
                else if (cmbSubPipeCH.SelectedItem.ToString() == "2")
                {
                    subPipe = "2";
                }
            }));
            #region 管架取管到清洗盘
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (1 - pos1).ToString("X2").Substring(6, 2)), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }
            currentHoleNum = currentHoleNum + (pos1 - 1);
            //若当前管号等于0，说明转过来的孔号为30
            if (currentHoleNum > 30)
            {
                currentHoleNum = currentHoleNum - 30;
            }
            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
            getTubePos = int.Parse(OperateIniFile.ReadIniData("Tube", "TubePos", "", iniPathSubstrateTube));
            plate = getTubePos % 88 == 0 ? getTubePos / 88 - 1 : getTubePos / 88;//几号板
            column = getTubePos % 11 == 0 ? getTubePos / 11 - (plate * 8) : getTubePos / 11 + 1 - (plate * 8);
            hole = getTubePos % 11 == 0 ? 11 : getTubePos % 11;
            int iNeedCool = 0;
            //夹新管到清洗盘
            AgainNewMove:
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06 " + plate.ToString("x2") + " " + column.ToString("x2") + " " + hole.ToString("x2")), 1);
            if (!NetCom3.Instance.MoveQuery())
            {
                if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull)
                {
                    iNeedCool++;
                    bool isMoevtoNextBoard;
                    if (iNeedCool < 12)
                    {
                        if (iNeedCool == 3 || iNeedCool == 6 || iNeedCool == 9)
                        {
                            isMoevtoNextBoard = true;
                            getTubePos = BoardNextPos(getTubePos, true, out isMoevtoNextBoard);
                        }
                        else
                        {
                            isMoevtoNextBoard = false;
                            getTubePos = BoardNextPos(getTubePos, false, out isMoevtoNextBoard);
                        }

                        plate = getTubePos % 88 == 0 ? getTubePos / 88 - 1 : getTubePos / 88;//几号板
                        column = getTubePos % 11 == 0 ? getTubePos / 11 - (plate * 8) : getTubePos / 11 + 1 - (plate * 8);
                        hole = getTubePos % 11 == 0 ? 11 : getTubePos % 11;
                        goto AgainNewMove;
                    }
                    else
                    {
                        frmMsgShow.MessageShow("组合测试", "移管手多次抓空，请装载管架！");
                        //NewWashEnd();
                        return;
                    }
                }
                else
                {
                    return;
                }
                //fbtnStart.Enabled = true;
                //fbtnStop.Enabled = false;
                //return;
            }
            OperateIniFile.WriteIniData("TubePosition", "No19", "1", iniPathWashTrayInfo);
            #region 取放管成功，相关配置文件修改
            List<int> lisTubeNum = new List<int>();
            lisTubeNum = QueryTubeNum();
            //移管手要夹的下一个管架位置
            int NextPos = getTubePos + 1;
            //管架中第一个装载管架的索引
            int firstTubeIndex = lisTubeNum.FindIndex(ty => ty <= 88 && ty > 0);
            for (int i = 1; i <= lisTubeNum.Count; i++)
            {
                if (NextPos == i * 88 + 1)
                {
                    NextPos = firstTubeIndex * 88 + (88 - lisTubeNum[firstTubeIndex]) + 1;
                }
            }
            OperateIniFile.WriteIniData("Tube", "TubePos", NextPos.ToString(), iniPathSubstrateTube);
            int TubeRack = (getTubePos) / 88;
            int curTube = (getTubePos) % 88;
            if (curTube == 0 && getTubePos != 0)
            {
                TubeRack = TubeRack - 1;
                curTube = 88;
            }
            //那个架子减了一个管
            OperateIniFile.WriteIniData("Tube", "Pos" + (TubeRack + 1).ToString(), (88 - curTube).ToString(), iniPathSubstrateTube);
            //清洗盘配置文件修改
            //OperateIniFile.WriteIniData("TubePosition", "No1", "1", iniPathWashTrayInfo);
            #endregion
            #endregion
            #region 清洗盘逆时针旋转19位，回到19号孔，加底物位置
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (pos1).ToString("X2")), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }

            currentHoleNum = currentHoleNum - pos1 + 1;
            //若当前管号等于0，说明转过来的孔号为30
            if (currentHoleNum > 30)
            {
                currentHoleNum = currentHoleNum - 30;
            }
            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
            #endregion
            #region 底物灌注
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 01 " + subPipe + "0"), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }
            if (subPipe == "1")
            {
                substrateNum1 = substrateNum1 - 1;
                OperateIniFile.WriteIniData("Substrate1", "LeftCount", substrateNum1.ToString(), iniPathSubstrateTube);
            }
            else
            {
                substrateNum2 = substrateNum2 - 1;
                OperateIniFile.WriteIniData("Substrate1", "LeftCount", substrateNum2.ToString(), iniPathSubstrateTube);
            }
            #endregion
            #region 清洗盘旋转到读数位置读数
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (6).ToString("X2")), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }
            currentHoleNum = currentHoleNum + 6;
            //若当前管号等于0，说明转过来的孔号为30
            if (currentHoleNum > 30)
            {
                currentHoleNum = currentHoleNum - 30;
            }
            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
            #endregion
            //OperateIniFile.WriteIniData("TubePosition", "No19", "0", iniPathWashTrayInfo);
            //OperateIniFile.WriteIniData("TubePosition", "No25", "1", iniPathWashTrayInfo);
            //发送单独的读数指令
            BackObj = "";
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 00 01"), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }
            else
            {
                int delay = 1000;
                while (!BackObj.Contains("EB 90 31 A3") && delay > 0)
                {
                    NetCom3.Delay(10);
                    delay = delay - 10;
                }
                if (BackObj.Contains("EB 90 31 A3"))
                {
                    string temp = BackObj.Substring(BackObj.Length - 16).Replace(" ", "");
                    temp = Convert.ToInt64(temp, 16).ToString();
                    if (double.Parse(temp) > Math.Pow(10, 5))
                        temp = ((int)GetPMT(double.Parse(temp))).ToString();
                    //textReadShow.AppendText(DateTime.Now.ToString("HH-mm-ss") + ": " + "PMT背景值：" + temp + Environment.NewLine);
                    this.BeginInvoke(new Action(() => { txtSubTest.Text = temp; }));
                }
            }
            #region 读数完成25号位置扔废管
            //清洗盘逆时针旋转5位转到取放管位置
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (5).ToString("X2")), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }

            currentHoleNum = currentHoleNum + 6;
            //若当前管号等于0，说明转过来的孔号为30
            if (currentHoleNum > 30)
            {
                currentHoleNum = currentHoleNum - 30;
            }
            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
            //OperateIniFile.WriteIniData("TubePosition", "No25", "0", iniPathWashTrayInfo);
            //OperateIniFile.WriteIniData("TubePosition", "No1", "1", iniPathWashTrayInfo);
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
            if (!NetCom3.Instance.MoveQuery())
            {
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                return;
            }
            OperateIniFile.WriteIniData("TubePosition", "No19", "0", iniPathWashTrayInfo);
            #endregion
            this.BeginInvoke(new Action(() => { txtInfo.AppendText("底物有效性检测完成。。。" + Environment.NewLine); }));



        }
        private void fbtnStop_Click(object sender, EventArgs e)
        {
            StartThread.Abort();
            fbtnStart.Enabled = true;//add y 20180510
            fbtnStop.Enabled = false;//add y 20180510
            txtInfo.AppendText("----------维护操作已提前终止----------" + Environment.NewLine);//add y 20180510
            groupBox1.Enabled = true;//add y 20180510
            rdbtnGeneral.Enabled = rdbtnCustom.Enabled = true;//add y 20180510
        }
        private void fbtnReturn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void fbtnInstruDiagnost_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmDiagnost"))
            {
                frmDiagnost frnID = new frmDiagnost();
                frnID.TopLevel = false;
                frnID.Parent = this.Parent;
                frnID.Show();
            }
            else
            {
                frmDiagnost frnID = (frmDiagnost)Application.OpenForms["frmDiagnost"];
                frnID.BringToFront();

            }
        }
        private void fbtnGroupTest_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmInstruGroupTest"))
            {
                frmInstruGroupTest frnIGT = new frmInstruGroupTest();
                frnIGT.TopLevel = false;
                frnIGT.Parent = this.Parent;
                frnIGT.Show();
            }
            else
            {
                frmInstruGroupTest frnIGT = (frmInstruGroupTest)Application.OpenForms["frmInstruGroupTest"];
                frnIGT.BringToFront();

            }
        }
        private void frmInstruMaintenance_SizeChanged(object sender, EventArgs e)
        {
            formSizeChange(this);
        }

        private void frmInstruMaintenance_FormClosed(object sender, FormClosedEventArgs e)
        {
            NetCom3.Instance.ReceiveHandel -= new Action<string>(Instance_ReceiveHandel);
        }

        private void functionButton1_Click(object sender, EventArgs e)//20180516 y 仪器初始化点击事件
        {
            int X = Convert.ToInt32((this.Width - dfInitializers.Width) / 2);
            int Y = Convert.ToInt32((this.Height - dfInitializers.Height)/2);
            dfInitializers.Location = new Point(X, Y);
            dfInitializers.Visible = true;
            lainitializers.Visible = true;

            //上下位机连接
            BeginInvoke(new Action(() =>
            {
                pbinitializers.Value = 20;
                lainitializers.Text = "上下位机连接..." + " " + pbinitializers.Value.ToString() + "%";
            }));
            if (!NetCom3.isConnect)
            {
                if (NetCom3.Instance.CheckMyIp_Port_Link())
                {
                    NetCom3.Instance.ConnectServer();

                    if (!NetCom3.isConnect)
                        goto complete;

                }
            }
            int[] HandData = new int[16];
            //仪器初始化
            BeginInvoke(new Action(() =>
            {
                pbinitializers.Value = 50;
                lainitializers.Text = "仪器初始化..." + " " + pbinitializers.Value.ToString() + "%";
            }));
            Array.Clear(dataRecive, 0, 15);//2018-09-17
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 F1 02"), 5);
            if (!NetCom3.Instance.SingleQuery())
            {
                goto complete;
            }
            #region 判断各个模组是否初始化成功
            //while (dataRecive[0] == null)
            //{
            //    Thread.Sleep(500);
            //}
            //HandData = NetCom3.converTo10(dataRecive);
            
            //if (HandData[4] != 255)
            //{
            //    frmMsgShow.MessageShow("仪器初始化", "计数器模组初始化失败！");
            //    goto complete;
            //}
            //if (HandData[5] != 255)
            //{
            //    frmMsgShow.MessageShow("仪器初始化", "抓手模组初始化失败！");
            //    goto complete;
            //}
            //if (HandData[6] != 255)
            //{
            //    frmMsgShow.MessageShow("仪器初始化", "加样机模组初始化失败！");
            //    goto complete;
            //}
            //if (HandData[7] != 255)
            //{
            //    frmMsgShow.MessageShow("仪器初始化", "清洗模组初始化失败！");
            //    goto complete;
            //}
             
            
            if (NetCom3.Instance.ErrorMessage != null)
            {
                //2018-09-06 zlx mod
                frmMsgShow.MessageShow("仪器初始化",NetCom3.Instance.ErrorMessage);
                goto complete;  
            }
            #endregion
            currentHoleNum = int.Parse(OperateIniFile.ReadInIPara("OtherPara", "washCurrentHoleNum"));
            //currentHoleNum孔转到清洗盘取放管位置
            //NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 02 " + currentHoleNum.ToString("x2")), 2);
            BeginInvoke(new Action(() =>
            {
                pbinitializers.Value = 90;
                lainitializers.Text = "仪器初始化..." + " " + pbinitializers.Value.ToString() + "%";
            }));
            //if (!NetCom3.Instance.WashQuery())
            //{
            //    goto complete;
            //}
            BeginInvoke(new Action(() =>
            {
                pbinitializers.Value = 100;
                lainitializers.Text = "仪器初始化..." + " " + pbinitializers.Value.ToString() + "%";
            }));
            Thread.Sleep(2000);
            complete:
            dfInitializers.Visible = false;
            lainitializers.Visible = false;
        }

        
        private int BoardNextPos(int pos, bool moveToNextBoard, out bool isRemoveBoarf)//add y 20180727  返回下一个管位置
        {
            if (pos == 0)
            {
                int boardPos = int.Parse(OperateIniFile.ReadIniData("Tube", "TubePos", "1", System.IO.Directory.GetCurrentDirectory() + "\\SubstrateTube.ini"));
                OperateIniFile.WriteIniData("Tube", "TubePos", ((boardPos + 1) > 352 ? 1 : (boardPos + 1)).ToString(), iniPathSubstrateTube);
                isRemoveBoarf = moveToNextBoard;
                int platee = boardPos % 88 == 0 ? boardPos / 88 - 1 : boardPos / 88;//2018-09-03 zlx mod
                int number = int.Parse(Common.OperateIniFile.ReadIniData("Tube", "Pos" + (platee + 1).ToString(), "1", System.IO.Directory.GetCurrentDirectory() + "\\SubstrateTube.ini"));
                if (number < 1)
                {
                    number = 1;
                }
                OperateIniFile.WriteIniData("Tube", "Pos" + (platee + 1).ToString(), (number - 1).ToString(), iniPathSubstrateTube);
                return boardPos;
            }
            isRemoveBoarf = moveToNextBoard;
            int plate = pos % 88 == 0 ? pos / 88 - 1 : pos / 88;//几号板
            int column = pos % 11 == 0 ? pos / 11 - (plate * 8) : pos / 11 + 1 - (plate * 8);
            int hole = pos % 11 == 0 ? 11 : pos % 11;
            if (moveToNextBoard)
            {
                plate++;
                if (plate > 3)
                {
                    plate = 0;
                }
                column = hole = 1;
            }
            else
            {
                hole++;
                if (hole > 11)
                {
                    hole = 1;
                    column++;
                    if (column > 8)
                    {
                        column = 1;
                        plate++;
                        isRemoveBoarf = true;
                        if (plate > 3)
                        {
                            plate = 0;
                        }
                    }
                }
            }
            if (isRemoveBoarf)
            {
                OperateIniFile.WriteIniData("Tube", "Pos" + (plate == 0 ? 4 : plate).ToString(), (0).ToString(), iniPathSubstrateTube);
            }
            OperateIniFile.WriteIniData("Tube", "TubePos", (plate * 88 + (column - 1) * 11 + hole + 1).ToString(), iniPathSubstrateTube);
            int count = int.Parse(Common.OperateIniFile.ReadIniData("Tube", "Pos" + (plate + 1).ToString(), "1", System.IO.Directory.GetCurrentDirectory() + "\\SubstrateTube.ini"));
            if (count < 1) count = 1;
            OperateIniFile.WriteIniData("Tube", "Pos" + (plate + 1).ToString(), (count - 1).ToString(), iniPathSubstrateTube);
            return plate * 88 + (column - 1) * 11 + hole;
        }

        private void RdbDaily_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbDaily.Checked)
            {
                chbClearWashTube.Checked = true;
                chbClearWashTube.Enabled = false;
                chbClearReactTube.Enabled = false;
                chbSamplePipeline.Enabled = false;
                txtSamplePipeline.Enabled = false;
                chbWashPipeline.Enabled = false;
                txtWashPipeline.Enabled = false;
                chbPmt.Enabled = false;
                txtPMT.Enabled = false;
                chbSubstrate.Checked = true;
                chbSubstrate.Enabled = false;
                txtSubPipeline.Text = "15";
                txtSubPipeline.Enabled = false;
                cmbSubstrate.SelectedIndex = 0;
                cmbSubstrate.Enabled = false;
                chbSubstrateTest.Checked = true;
                chbSubstrateTest.Enabled = false;
                //txtSubTest.Enabled = false;
                cmbSubPipeCH.SelectedIndex = 0;
                cmbSubPipeCH.Enabled = false;
                fbtnStart.Enabled = true;
                fbtnStop.Enabled = false;
                rdbtnGeneral.Enabled = rdbtnCustom.Enabled = true;
            }
            else
            {
                ControlEnable(true);
                txtSubPipeline.Text = "5";
            }
        }
    }
}
