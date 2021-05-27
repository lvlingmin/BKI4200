using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Dialogs;
using Common;
using System.IO;
using BioBaseCLIA.Run;
using Maticsoft.DBUtility;

namespace BioBaseCLIA.SysMaintenance
{
    public partial class frmInstruGroupTest : frmParent
    {
        #region 变量
        /// <summary>
        /// 组合测试线程
        /// </summary>
        private Thread GroupTestRun;
        /// <summary>
        /// 移管手测试标志位
        /// </summary>
        bool MoveTubeFlag = false;
        /// <summary>
        /// 加样臂测试标志位
        /// </summary>
        bool AddSampleArmFlag = false;
        /// <summary>
        /// 清洗盘测试标志位
        /// </summary>
        bool WashTrayFlag = false;
        /// <summary>
        /// 报警提示
        /// </summary>
        messageDialog msd = new messageDialog();
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
        /// 清洗盘当前取放管位置孔号
        /// </summary>
        public int currentHoleNum = 1;
        /// <summary>
        /// 空气隔断体积
        /// </summary>
        int AirVol = int.Parse(OperateIniFile.ReadInIPara("Vol", "AirVol"));
        /// <summary>
        /// 样本前吐体积
        /// </summary>
        int SpBeforeVol = int.Parse(OperateIniFile.ReadInIPara("Vol", "SpBeforeVol"));
        /// <summary>
        /// 样本后弃体积
        /// </summary>
        int SpAbandonVol = int.Parse(OperateIniFile.ReadInIPara("Vol", "SpAbandonVol"));
        /// <summary>
        /// 试剂前吐体积
        /// </summary>
        int RgBeforeVol = int.Parse(OperateIniFile.ReadInIPara("Vol", "RgBeforeVol"));
        /// <summary>
        /// 试剂后弃体积
        /// </summary>
        int RgAbandonVol = int.Parse(OperateIniFile.ReadInIPara("Vol", "RgAbandonVol"));
        /// <summary>
        /// 每次洗针时间
        /// </summary>
        int NeedleWashTime = int.Parse(OperateIniFile.ReadInIPara("Time", "NeedleWashTime"));
        /// <summary>
        /// 每次洗针时间
        /// </summary>
        int FirstNeedleWashTime = int.Parse(OperateIniFile.ReadInIPara("Time", "FirstCleanNeedleTime"));
        /// <summary>
        /// 混匀次数
        /// </summary>
        int MixNum = int.Parse(OperateIniFile.ReadInIPara("OtherPara", "MixNum"));
        /// <summary>
        /// 反应盘孔数
        /// </summary>
        int ReactTrayHoleNum = int.Parse(OperateIniFile.ReadInIPara("OtherPara", "ReactTrayHoleNum"));
        /// <summary>
        /// 移管手组合测试信息显示委托
        /// </summary>
        Action<string> MoveGroupTestInfo;
        /// <summary>
        /// 加样臂组合测试信息显示委托
        /// </summary>
        Action<string> ArmGroupTestInfo;
        /// <summary>
        /// 清洗检测盘组合测试信息显示委托
        /// </summary>
        Action<string> WashTrayGroupTestInfo;
        frmMessageShow frmMsgShow = new frmMessageShow();
        /// <summary>
        /// 底物管路
        /// </summary>
        string subpipe = "0";
        #endregion

        public frmInstruGroupTest()
        {
            InitializeComponent();
        }

        private void frmInstruGroupTest_Load(object sender, EventArgs e)
        {
            NetCom3.Instance.ReceiveHandel += new Action<string>(Instance_ReceiveHandel);
            ///通讯连接
            #region 移管手
            chbTakeTube.Checked = true;
            chbPutTube.Checked = true;
            rdbtnTubeRack.Checked = true;
            rdbtnAbandon.Checked = true;

            #endregion

            #region 加样臂
            chbAddSample.Checked = true;
            chbR1.Checked = true;
            chbR2.Checked = true;
            chbBeads.Checked = true;

            cmbReagentPos.SelectedIndex = 0;
            cmbTakeSamPos.SelectedIndex = 0;
            cmbRegentType.SelectedIndex = 0;
            cmbSamCupType.SelectedIndex = 0;
            cmbReactStartPos.Items.Clear();
            for (int i = 1; i <= ReactTrayHoleNum; i++)
            {
                cmbReactStartPos.Items.Add(i);
            }
            cmbReagentPos.Items.Clear();
            for (int i = 1; i <= frmParent.RegentNum; i++)
            {
                cmbReagentPos.Items.Add(i);
            }
            cmbReagentPos.SelectedIndex = 0;
            QueryTubePos();
            #endregion

            #region 清洗检测盘
            chbWash.Checked = true;
            //chbLiquidInjection.Checked = true;
            chbAddSu.Checked = true;
            chbRead.Checked = true;
            cmbPutTube.Enabled = false;
            comboBox2.SelectedIndex = 0;

            #endregion

            MoveGroupTestInfo = new Action<string>((str) =>
            {
                txtMoveShowInfo.AppendText(str + Environment.NewLine);
            });
            ArmGroupTestInfo = new Action<string>((str) =>
            {
                txtArmShowInfo.AppendText(str + Environment.NewLine);
            });
            WashTrayGroupTestInfo = new Action<string>((str) =>
            {
                txtWashShowInfo.AppendText(str + Environment.NewLine);
            });
            #region 控件可用状态
            if (LoginUserType == "0")//admin 用户
            {
                //fbtnInstruMaintenance.Enabled = false;
                fbtnInstruDiagnost.Enabled = false;
                fbtnGroupTest.Enabled = true;
            }
            else if (LoginUserType == "2") // normal 用户
            {
                fbtnInstruDiagnost.Enabled = false;
                fbtnGroupTest.Enabled = false;
            }
            InitControlEnable();
            #endregion
        }
        /// <summary>
        /// 指令返回
        /// </summary>
        string BackObj = "";
        void Instance_ReceiveHandel(string obj)
        {
            if (obj.IsNullOrEmpty())
                return;

            else
            {
                BackObj = obj;
            }

        }
        /// <summary>
        /// 委托用方法，用于接收读数数据
        /// </summary>
        /// <param name="order"></param>
        private void GetReadNum(string order)
        {
            if (order.Contains("EB 90 31 A3"))
            {
                string temp = order.Replace(" ", "");
                int pos = temp.IndexOf("EB9031A3");
                temp = temp.Substring(pos, 32);
                temp = temp.Substring(temp.Length - 8);
                temp = Convert.ToInt64(temp, 16).ToString();
                if (int.Parse(temp) > Math.Pow(10, 5))
                    temp = ((int)GetPMT(double.Parse(temp))).ToString();
                while (!this.IsHandleCreated)//为了防止出现“在创建窗口句柄之前，不能在控件上调用 Invoke 或 BeginInvoke”的错误
                {
                    Thread.Sleep(15);
                }
                BeginInvoke(new Action(() => { txtReadValue.Text = temp; }));
                BeginInvoke(WashTrayGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ": " + "PMT背景值：" + temp });

            }
        }

        /// <summary>
        /// 组合测试方法
        /// </summary>
        void TestRun()
        {
            if (!NetCom3.isConnect)
            {
                frmMsgShow.MessageShow("组合测试", "请进行网络连接！");
                return;
            }
            while (true)
            {
                #region 移管手
                if (MoveTubeFlag)
                {
                    BeginInvoke(MoveGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ": " + "移管手组合测试开始；" });
                    int LoopNum = int.Parse(txtTubeLoopNum.Text);
                    int putTubePos = 1;
                    int takePos = 1;

                    Invoke(new Action(() =>
                    {
                        if (rdbtnReactTray.Checked)
                        {
                            takePos = int.Parse(cmbTakeTubePos.SelectedItem.ToString());
                        }

                        if (rdbtnReactTrayp.Checked)
                        {
                            putTubePos = int.Parse(cmbPutTube.SelectedItem.ToString());
                        }
                    }));
                    if (LoopNum > 0)
                    {
                        BeginInvoke(MoveGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ":  正在进行移管操作；" });
                    }
                    while (LoopNum > 0)
                    {
                        if (rdbtnTubeRack.Checked)
                        {
                            //string takeTubePos = OperateIniFile.ReadIniData("Tube", "TubePos", "1", iniPathSubstrateTube);
                            if (rdbtnReactTrayp.Checked)
                            {
                                #region 管架取管到温育盘                               
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 01 "+ putTubePos.ToString("x2")), 1);
                                if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
                                {
                                    InitControlEnable();
                                    return;
                                }
                                #region 取管成功                             
                                OperateIniFile.WriteIniData("ReactTrayInfo", "no" + putTubePos, "1", iniPathReactTrayInfo);
                                putTubePos++;
                                if (putTubePos == ReactTrayHoleNum + 1)
                                {
                                    putTubePos = 1;
                                }
                                #endregion
                                #endregion
                            }
                            else if (rdbtnWashTrayP.Checked)
                            {
                                #region 管架取管到清洗盘                                                        
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06"), 1);
                                if (!NetCom3.Instance.MoveQuery())
                                {
                                    InitControlEnable();
                                    return;
                                }
                                #region 取管成功
                                List<int> lisTubeNum = new List<int>();
                                lisTubeNum = QueryTubeNum();
                                OperateIniFile.WriteIniData("TubePosition", "No1", "1", iniPathWashTrayInfo);

                                if (LoopNum != 1)
                                {
                                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                                    if (!NetCom3.Instance.WashQuery())
                                    {
                                        InitControlEnable();
                                        return;
                                    }
                                    currentHoleNum = currentHoleNum + 1;
                                    //如果孔号超过30，孔号设为1
                                    if (currentHoleNum > WashTrayNum)
                                    {
                                        currentHoleNum = currentHoleNum - WashTrayNum;
                                    }
                                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                                    DataTable dtWashTrayIni = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
                                    DataTable dtTemp = new DataTable();
                                    dtTemp = dtWashTrayIni.Copy();
                                    //清洗盘状态列表中添加反应盘位置字段，赋值需多赋值一个字段。 
                                    for (int j = 1; j < 2; j++)
                                        dtWashTrayIni.Rows[0][j] = dtTemp.Rows[dtWashTrayIni.Rows.Count - 1][j];
                                    for (int i = 1; i < dtWashTrayIni.Rows.Count; i++)
                                    {
                                        for (int j = 1; j < 2; j++)
                                        {
                                            dtWashTrayIni.Rows[i][j] = dtTemp.Rows[i - 1][j];
                                        }
                                    }

                                    OperateIniFile.WriteConfigToFile("[TubePosition]", iniPathWashTrayInfo, dtWashTrayIni);
                                }
                                #endregion
                                #endregion
                            }
                            else if (rdbtnAbandon.Checked)
                            {
                                #region 暂存盘取管到废弃处
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 07 "), 1);
                                if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                                {
                                    InitControlEnable();
                                    return;
                                }
                                #endregion
                            }
                        }
                        if (rdbtnReactTray.Checked)
                        {
                            if (rdbtnWashTrayP.Checked)
                            {
                                #region 温育盘夹管到清洗盘
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 02 " + takePos.ToString("x2")), 1);
                                if (!NetCom3.Instance.MoveQuery())
                                {
                                    InitControlEnable();
                                    return;
                                }
                                #region 取放管成功
                                OperateIniFile.WriteIniData("ReactTrayInfo", "no" + takePos.ToString(), "0", iniPathReactTrayInfo);
                                takePos++;
                                if (takePos == ReactTrayHoleNum + 1)
                                {
                                    takePos = 1;
                                }
                                //修改清洗盘信息，清洗盘转一位
                                OperateIniFile.WriteIniData("TubePosition", "No1", "1", iniPathWashTrayInfo);
                                if (LoopNum != 1)
                                {
                                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                                    if (!NetCom3.Instance.WashQuery())
                                    {
                                        InitControlEnable();
                                        return;
                                    }
                                    currentHoleNum = currentHoleNum + 1;
                                    //如果孔号超过30，孔号设为1
                                    if (currentHoleNum > WashTrayNum)
                                    {
                                        currentHoleNum = currentHoleNum - WashTrayNum;
                                    }
                                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                                    DataTable dtWashTrayIni = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
                                    DataTable dtTemp = new DataTable();
                                    dtTemp = dtWashTrayIni.Copy();
                                    //清洗盘状态列表中添加反应盘位置字段，赋值需多赋值一个字段。 
                                    for (int j = 1; j < 2; j++)
                                        dtWashTrayIni.Rows[0][j] = dtTemp.Rows[dtWashTrayIni.Rows.Count - 1][j];
                                    for (int i = 1; i < dtWashTrayIni.Rows.Count; i++)
                                    {
                                        for (int j = 1; j < 2; j++)
                                        {
                                            dtWashTrayIni.Rows[i][j] = dtTemp.Rows[i - 1][j];
                                        }
                                    }

                                    OperateIniFile.WriteConfigToFile("[TubePosition]", iniPathWashTrayInfo, dtWashTrayIni);
                                }
                                #endregion
                                #endregion
                            }
                            else if (rdbtnAbandon.Checked)
                            {
                                #region 温育盘扔废管
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 " + takePos.ToString("x2")), 1);
                                if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
                                {
                                    InitControlEnable();
                                    return;
                                }
                                #region 扔废管成功
                                OperateIniFile.WriteIniData("ReactTrayInfo", "no" + takePos.ToString(), "0", iniPathReactTrayInfo);
                                takePos++;
                                if (takePos == ReactTrayHoleNum + 1)
                                {
                                    takePos = 1;
                                }
                                #endregion
                                #endregion
                            }
                        }

                        if (rdbtnWashTray.Checked)
                        {
                            if (rdbtnReactTrayp.Checked)
                            {
                                #region 清洗盘取管到温育盘
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 03 " + putTubePos.ToString("x2") + " 02"), 1);
                                if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
                                {
                                    InitControlEnable();
                                    return;
                                }
                                #region 取放管成功
                                //修改反应盘信息
                                OperateIniFile.WriteIniData("ReactTrayInfo", "no" + putTubePos, "1", iniPathReactTrayInfo);
                                putTubePos++;
                                if (putTubePos == ReactTrayHoleNum + 1)
                                {
                                    putTubePos = 1;
                                }
                                //修改清洗盘信息，清洗盘转一位
                                OperateIniFile.WriteIniData("TubePosition", "No1", "0", iniPathWashTrayInfo);
                                if (LoopNum != 1)
                                {
                                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-1).ToString("X2").Substring(6, 2)), 2);
                                    if (!NetCom3.Instance.WashQuery())
                                    {
                                        InitControlEnable();
                                        return;
                                    }
                                    currentHoleNum = currentHoleNum - 1;
                                    //如果孔号超过30，孔号设为1
                                    if (currentHoleNum <= 0)
                                    {
                                        currentHoleNum = currentHoleNum + WashTrayNum;
                                    }
                                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                                    DataTable dtWashTrayIni = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
                                    DataTable dtTemp = new DataTable();
                                    dtTemp = dtWashTrayIni.Copy();
                                    //清洗盘状态列表中添加反应盘位置字段，赋值需多赋值一个字段。 
                                    for (int j = 1; j < 2; j++)
                                        dtWashTrayIni.Rows[dtWashTrayIni.Rows.Count - 1][j] = dtTemp.Rows[0][j];
                                    for (int i = 0; i < dtWashTrayIni.Rows.Count - 1; i++)
                                    {
                                        for (int j = 1; j < 2; j++)
                                        {
                                            dtWashTrayIni.Rows[i][j] = dtTemp.Rows[i + 1][j];
                                        }
                                    }

                                    OperateIniFile.WriteConfigToFile("[TubePosition]", iniPathWashTrayInfo, dtWashTrayIni);
                                }
                                #endregion
                                #endregion
                            }
                            else if (rdbtnAbandon.Checked)
                            {
                                #region 清洗盘扔废管
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
                                if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
                                {
                                    InitControlEnable();
                                    return;
                                }
                                #region 取放管成功
                                //修改反应盘信息
                                OperateIniFile.WriteIniData("ReactTrayInfo", "no" + putTubePos, "1", iniPathReactTrayInfo);
                                putTubePos++;
                                if (putTubePos == ReactTrayHoleNum + 1)
                                {
                                    putTubePos = 1;
                                }
                                //修改清洗盘信息，清洗盘转一位
                                OperateIniFile.WriteIniData("TubePosition", "No1", "0", iniPathWashTrayInfo);
                                if (LoopNum != 1)
                                {
                                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-1).ToString("X2").Substring(6, 2)), 2);
                                    if (!NetCom3.Instance.WashQuery())
                                    {
                                        InitControlEnable();
                                        return;
                                    }
                                    currentHoleNum = currentHoleNum - 1;
                                    //如果孔号小于等于0
                                    if (currentHoleNum <= 0)
                                    {
                                        currentHoleNum = currentHoleNum + WashTrayNum;
                                    }
                                    OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                                    DataTable dtWashTrayIni = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
                                    DataTable dtTemp = new DataTable();
                                    dtTemp = dtWashTrayIni.Copy();
                                    //清洗盘状态列表中添加反应盘位置字段，赋值需多赋值一个字段。 
                                    for (int j = 1; j < 2; j++)
                                        dtWashTrayIni.Rows[dtWashTrayIni.Rows.Count - 1][j] = dtTemp.Rows[0][j];
                                    for (int i = 0; i < dtWashTrayIni.Rows.Count - 1; i++)
                                    {
                                        for (int j = 1; j < 2; j++)
                                        {
                                            dtWashTrayIni.Rows[i][j] = dtTemp.Rows[i + 1][j];
                                        }
                                    }

                                    OperateIniFile.WriteConfigToFile("[TubePosition]", iniPathWashTrayInfo, dtWashTrayIni);
                                }
                                #endregion
                                #endregion
                            }
                        }
                        LoopNum--;
                        BeginInvoke(new Action(() =>
                   {
                       txtTubeLoopNum.Text = LoopNum.ToString();
                   }));
                    }
                    BeginInvoke(MoveGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ":  移管操作完成！" });
                    BeginInvoke(new Action(() =>
                    {
                        InitControlEnable();
                        cmbPutTube.SelectedItem = putTubePos;
                        cmbTakeTubePos.SelectedItem = takePos;
                    }));
                    MoveTubeFlag = false;
                    break;
                }
                #endregion
                #region 加样臂模块
                if (AddSampleArmFlag)
                {
                    //循环次数
                    int LoopNum = int.Parse(txtArmLoopNum.Text);
                    //取样位置
                    int takeSamplePos = 1;
                    //温育盘位置
                    int InPos = 1;
                    //取试剂位置
                    int takeReagentPos = 1;
                    //杯类型
                    int cupTypeFlag = 0;
                    //试剂类型
                    int ReagentType = 0;
                    int TubeNum = sumReactTubeNum+1;
                    Invoke(new Action(() =>
                    {
                        if (chbAddSample.Checked)
                        {
                            takeSamplePos = int.Parse(cmbTakeSamPos.SelectedItem.ToString());
                            cupTypeFlag = cmbSamCupType.SelectedIndex;
                        }
                        InPos = int.Parse(cmbReactStartPos.SelectedItem.ToString());
                        if (chbR1.Checked || chbR2.Checked || chbBeads.Checked)
                        {
                            takeReagentPos = int.Parse(cmbReagentPos.SelectedItem.ToString());
                            ReagentType = cmbRegentType.SelectedIndex;

                        }
                    }));
                    while (LoopNum > 0)
                    {
                        if (TubeNum <= ReactTrayNum)
                        {
                            //从管架夹新管到反应盘
                            int pos1 = InPos;
                            BeginInvoke(ArmGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ": " + "温育盘放新管" });
                            #region 管架夹管到温育盘
                            //查询配置文件现在管架的取管位置
                            //string takeTubePos = OperateIniFile.ReadIniData("Tube", "TubePos", "1", iniPathSubstrateTube);
                            //int plate = int.Parse(takeTubePos) % 88 == 0 ? int.Parse(takeTubePos) / 88 - 1 : int.Parse(takeTubePos) / 88;//几号板
                            //int column = int.Parse(takeTubePos) % 11 == 0 ? int.Parse(takeTubePos) / 11 - (plate * 8) : int.Parse(takeTubePos) / 11 + 1 - (plate * 8);
                            //int hole = int.Parse(takeTubePos) % 11 == 0 ? 11 : int.Parse(takeTubePos) % 11;
                            string pos = OperateIniFile.ReadIniData("ReactTrayInfo", "no" + pos1, "", iniPathReactTrayInfo);
                            if (pos == "2")
                            {
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 " + pos1.ToString("x2")), 1);
                                if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                                {
                                    InitControlEnable();
                                    return;
                                }
                                else
                                    pos = "0";
                            }
                            if (pos == "0")
                            {
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 01 " + pos1.ToString("x2")), 1);
                                if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                                {
                                    InitControlEnable();
                                    return;
                                }
                            }
                            #region 取放管成功
                            List<int> lisTubeNum = new List<int>();
                            lisTubeNum = QueryTubeNum();
                            /*
                            //移管手要夹的下一个管架位置
                            int NextPos = int.Parse(takeTubePos) + 1;
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
                            int TubeRack = (int.Parse(takeTubePos)) / 88;
                            int curTube = (int.Parse(takeTubePos)) % 88;
                            if (curTube == 0 && int.Parse(takeTubePos) != 0)
                            {
                                TubeRack = TubeRack - 1;
                                curTube = 88;
                            }
                            //那个架子减了一个管
                            OperateIniFile.WriteIniData("Tube", "Pos" + (TubeRack + 1).ToString(), (88 - curTube).ToString(), iniPathSubstrateTube);
                            */
                            OperateIniFile.WriteIniData("ReactTrayInfo", "no" + pos1, "1", iniPathReactTrayInfo);
                            #endregion
                            #endregion
                            TubeNum++;

                        }
                        #region 取样本，加样
                        if (chbAddSample.Checked)
                        {
                            BeginInvoke(ArmGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + "： " + takeSamplePos.ToString()+
                                "号样本正在进行加样。。" });
                            //获取当前反应盘可用位置
                            int pos = InPos;
                            //样本吸液量固定为20
                            int spDistributeVol = 20;//获取分液量
                            ///样本盘转到取样位置SamplePos，加样针加样到反应盘pos位置。
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 01 " + takeSamplePos.ToString("x2") + " " + pos.ToString("x2")
                                + " " + spDistributeVol.ToString("x2")), 0);
                            if (!NetCom3.Instance.SPQuery())
                            {
                                InitControlEnable();
                                return;
                            }
                            //#endregion
                            BeginInvoke(ArmGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + "： "+ takeSamplePos.ToString()+
                                "号样本加样完成。。" });
                            //该位置信息写入配置文件
                            OperateIniFile.WriteIniData("ReactTrayInfo", "no" + pos.ToString(), "2", iniPathReactTrayInfo);
                        }
                        #endregion
                        if (chbR1.Checked)
                        {
                            BeginInvoke(ArmGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ":  正在进行加试剂1。。" });
                            #region 取试剂1，加试剂
                            int rg1DistributeVol = 50;//获取分液量
                            int rgPos = takeReagentPos;
                            int LeftR1 = 0;
                            //获取当前反应盘可用位置
                            int pos = InPos;
                            if (ReagentType != 0)
                            {

                                string RegentName = OperateIniFile.ReadIniData("ReagentPos" + takeReagentPos.ToString(),
                                    "ItemName", "", iniPathReagentTrayInfo);
                                if (RegentName == "" || RegentName == null)
                                {
                                    msd.show("该位置没有试剂，请重新选择试剂位置！");
                                    BeginInvoke(ArmGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + "： "+ takeReagentPos.ToString()+
                                        "号位置没有试剂！" });
                                    InitControlEnable();
                                    return;
                                }
                                LeftR1 = int.Parse(OperateIniFile.ReadIniData("ReagentPos" + takeReagentPos.ToString(),
                                    "LeftReagent1", "", iniPathReagentTrayInfo));
                                if (LeftR1 < LoopNum)
                                {
                                    msd.show("试剂1测数不够，请重新选择试剂位置或者添加试剂");
                                    BeginInvoke(ArmGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ":  试剂1测数不够" });
                                    InitControlEnable();
                                    return;
                                }
                            }
                            string leftR1Vol = (LeftR1 * rg1DistributeVol + (int)(rg1DistributeVol * 0.1)).ToString("x4");
                            //吸取R1加到温育盘
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 03 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                               + " " + rg1DistributeVol.ToString("x2") + " " + leftR1Vol.Substring(0, 2) + " " + leftR1Vol.Substring(2, 2)), 0);
                            if (!NetCom3.Instance.SPQuery())
                            {
                                InitControlEnable();
                                return;
                            }
                            if (ReagentType != 0)
                            {
                                LeftR1--;
                                OperateIniFile.WriteIniData("ReagentPos" + rgPos.ToString(), "LeftReagent1", LeftR1.ToString(), iniPathReagentTrayInfo);
                            }
                            #endregion
                        }
                        if (chbR2.Checked)
                        {
                            BeginInvoke(ArmGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ":  正在进行加试剂2。。" });
                            #region 取试剂2加试剂
                            int rg2DistributeVol = 50;//获取分液量
                            int rgPos = takeReagentPos;
                            int LeftR2 = 0;
                            //获取当前反应盘可用位置
                            int pos = InPos;
                            if (ReagentType != 0)
                            {

                                string RegentName = OperateIniFile.ReadIniData("ReagentPos" + takeReagentPos.ToString(),
                                    "ItemName", "", iniPathReagentTrayInfo);
                                if (RegentName == "" || RegentName == null)
                                {
                                    msd.show("该位置没有试剂，请重新选择试剂位置！");
                                    BeginInvoke(ArmGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") +":  "+ takeReagentPos.ToString()+
                                        "号位置没有试剂！" });
                                    InitControlEnable();
                                    return;
                                }
                                LeftR2 = int.Parse(OperateIniFile.ReadIniData("ReagentPos" + takeReagentPos.ToString(),
                                    "LeftReagent2", "", iniPathReagentTrayInfo));
                                if (LeftR2 < LoopNum)
                                {
                                    msd.show("试剂2测数不够，请重新选择试剂位置或者添加试剂");
                                    BeginInvoke(ArmGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ": " + "试剂2测数不够" });
                                    InitControlEnable();
                                    return;
                                }
                            }
                            string leftR2Vol = (LeftR2 * rg2DistributeVol + (int)(rg2DistributeVol * 0.1)).ToString("x4");
                            //吸取R2加到温育盘
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 04 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                               + " " + rg2DistributeVol.ToString("x2") + " " + leftR2Vol.Substring(0, 2) + " " + leftR2Vol.Substring(2, 2)), 0);
                            if (!NetCom3.Instance.SPQuery())
                            {
                                InitControlEnable();
                                return;
                            }
                            if (ReagentType != 0)
                            {
                                LeftR2--;
                                OperateIniFile.WriteIniData("ReagentPos" + rgPos.ToString(), "LeftReagent2", LeftR2.ToString(), iniPathReagentTrayInfo);
                            }
                            #endregion
                        }
                        if (chbR3.Checked)
                        {
                            BeginInvoke(ArmGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ":  正在进行加试剂3。。" });
                            #region 取试剂2加试剂
                            int rg3DistributeVol = 50;//获取分液量
                            int rgPos = takeReagentPos;
                            int LeftR3 = 0;
                            //获取当前反应盘可用位置
                            int pos = InPos;
                            if (ReagentType != 0)
                            {

                                string RegentName = OperateIniFile.ReadIniData("ReagentPos" + takeReagentPos.ToString(),
                                    "ItemName", "", iniPathReagentTrayInfo);
                                if (RegentName == "" || RegentName == null)
                                {
                                    msd.show("该位置没有试剂，请重新选择试剂位置！");
                                    BeginInvoke(ArmGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") +":  "+ takeReagentPos.ToString()+
                                        "号位置没有试剂！" });
                                    InitControlEnable();
                                    return;
                                }
                                LeftR3 = int.Parse(OperateIniFile.ReadIniData("ReagentPos" + takeReagentPos.ToString(),
                                    "LeftReagent2", "", iniPathReagentTrayInfo));
                                if (LeftR3 < LoopNum)
                                {
                                    msd.show("试剂3测数不够，请重新选择试剂位置或者添加试剂");
                                    BeginInvoke(ArmGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ": " + "试剂3测数不够" });
                                    InitControlEnable();
                                    return;
                                }
                            }
                            string leftR3Vol = (LeftR3 * rg3DistributeVol + (int)(rg3DistributeVol * 0.1)).ToString("x4");
                            //吸取R3加到温育盘
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 05 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                               + " " + rg3DistributeVol.ToString("x2") + " " + leftR3Vol.Substring(0, 2) + " " + leftR3Vol.Substring(2, 2)), 0);
                            if (!NetCom3.Instance.SPQuery())
                            {
                                InitControlEnable();
                                return;
                            }
                            if (ReagentType != 0)
                            {
                                LeftR3--;
                                OperateIniFile.WriteIniData("ReagentPos" + rgPos.ToString(), "LeftReagent3", LeftR3.ToString(), iniPathReagentTrayInfo);
                            }
                            #endregion
                        }
                        if (chbBeads.Checked)
                        {
                            BeginInvoke(ArmGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ": " + "正在进行加磁珠。。" });
                            #region 取磁珠，加磁珠

                            int bdDistributeVol = 100;//获取分液量
                            int rgPos = takeReagentPos;
                            int LeftR4 = 0;
                            //获取当前反应盘可用位置
                            int pos = InPos;
                            if (ReagentType != 0)
                            {

                                string RegentName = OperateIniFile.ReadIniData("ReagentPos" + takeReagentPos.ToString(),
                                    "ItemName", "", iniPathReagentTrayInfo);
                                if (RegentName == "" || RegentName == null)
                                {
                                    msd.show("该位置没有试剂，请重新选择试剂位置！");
                                    BeginInvoke(ArmGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss")+": "  + takeReagentPos.ToString()+
                                        "号位置没有试剂！" });
                                    InitControlEnable();
                                    return;
                                }
                                LeftR4 = int.Parse(OperateIniFile.ReadIniData("ReagentPos" + takeReagentPos.ToString(),
                                   "LeftReagent4", "", iniPathReagentTrayInfo));
                                if (LeftR4 < LoopNum)
                                {
                                    msd.show("磁珠测数不够，请重新选择试剂位置或者添加试剂");
                                    BeginInvoke(ArmGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ": " + "磁珠测数不够" });
                                    InitControlEnable();
                                    return;
                                }
                            }
                            string leftR4Vol = (LeftR4 * bdDistributeVol + (int)(bdDistributeVol * 0.1)).ToString("x4");
                            //吸取磁珠加到温育盘
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 02 07 " + rgPos.ToString("x2") + " " + pos.ToString("x2")
                               + " " + bdDistributeVol.ToString("x2") + " " + leftR4Vol.Substring(0, 2) + " " + leftR4Vol.Substring(2, 2)), 0);
                            if (!NetCom3.Instance.SPQuery())
                            {
                                InitControlEnable();
                                return;
                            }
                            if (ReagentType != 0)
                            {
                                LeftR4--;
                                OperateIniFile.WriteIniData("ReagentPos" + rgPos.ToString(), "LeftReagent4", LeftR4.ToString(), iniPathReagentTrayInfo);
                            }
                            #endregion
                        }
                        
                        //取样位置++
                        InPos = InPos + 1 == ReactTrayNum+1 ? 1 : InPos + 1;
                        takeSamplePos = takeSamplePos+1 == SampleNum+1 ? 1 : takeSamplePos+1;
                        LoopNum--;
                        BeginInvoke(new Action(() =>
                       {
                           txtArmLoopNum.Text = LoopNum.ToString();
                       }));
                    }
                    BeginInvoke(ArmGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ": " + "动作完成。" });
                    AddSampleArmFlag = false;
                    BeginInvoke(new Action(() =>
                    {
                        InitControlEnable();
                    }));
                    break;

                }
                #endregion
                #region 清洗检测盘模块
                if (WashTrayFlag)
                {
                    int LoopNum = int.Parse(txtWashLoopNum.Text);
                    while (LoopNum > 0)
                    {
                        int repeateRead = (int)numericUpDown1.Value;
                        int stepGoon = 0;
                        int currentHoleNum = int.Parse(OperateIniFile.ReadInIPara("OtherPara", "washCurrentHoleNum"));
                        if (chbWash.Checked == true || chbAddSu.Checked == true)
                        {
                            BeginInvoke(WashTrayGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ": " + "管架取反应管放到清洗盘。。" });
                            //string takeTubePos = OperateIniFile.ReadIniData("Tube", "TubePos", "1", iniPathSubstrateTube);
                            #region 管架取管到清洗盘
                            //移管手到取管位置takepos[1]（管架）取管
                            //int plate = int.Parse(takeTubePos) % 88 == 0 ? int.Parse(takeTubePos) / 88 - 1 : int.Parse(takeTubePos) / 88;//几号板
                            //int column = int.Parse(takeTubePos) % 11 == 0 ? int.Parse(takeTubePos) / 11 - (plate * 8) : int.Parse(takeTubePos) / 11 + 1 - (plate * 8);
                            //int hole = int.Parse(takeTubePos) % 11 == 0 ? 11 : int.Parse(takeTubePos) % 11;
                            //到管架takepos[1]位置取管放到温育盘putpos位置
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06"), 1);
                            if (!NetCom3.Instance.MoveQuery())
                            {
                                InitControlEnable();
                                return;
                            }
                            #region 取管成功
                            /*
                            List<int> lisTubeNum = new List<int>();
                            lisTubeNum = QueryTubeNum();
                            //移管手要夹的下一个管架位置
                            int NextPos = int.Parse(takeTubePos) + 1;
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
                            int TubeRack = (int.Parse(takeTubePos)) / 88;
                            int curTube = (int.Parse(takeTubePos)) % 88;
                            if (curTube == 0 && int.Parse(takeTubePos) != 0)
                            {
                                TubeRack = TubeRack - 1;
                                curTube = 88;
                            }
                            //那个架子减了一个管
                            OperateIniFile.WriteIniData("Tube", "Pos" + (TubeRack + 1).ToString(), (88 - curTube).ToString(), iniPathSubstrateTube);
                             */
                            //修改清洗盘信息，清洗盘转一位
                            OperateIniFile.WriteIniData("TubePosition", "No1", "1", iniPathWashTrayInfo);

                            if (LoopNum != 1)
                            {
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                                if (!NetCom3.Instance.WashQuery())
                                {
                                    InitControlEnable();
                                    return;
                                }
                                currentHoleNum = currentHoleNum + 1;
                                //如果孔号超过30
                                if (currentHoleNum > WashTrayNum)
                                {
                                    currentHoleNum = currentHoleNum - WashTrayNum;
                                }
                                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                                DataTable dtWashTrayIni = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
                                DataTable dtTemp = new DataTable();
                                dtTemp = dtWashTrayIni.Copy();
                                //清洗盘状态列表中添加反应盘位置字段，赋值需多赋值一个字段。 
                                for (int j = 1; j < 5; j++)
                                    dtWashTrayIni.Rows[0][j] = dtTemp.Rows[dtWashTrayIni.Rows.Count - 1][j];
                                for (int i = 1; i < dtWashTrayIni.Rows.Count; i++)
                                {
                                    for (int j = 1; j < 5; j++)
                                    {
                                        dtWashTrayIni.Rows[i][j] = dtTemp.Rows[i - 1][j];
                                    }
                                }

                                OperateIniFile.WriteConfigToFile("[TubePosition]", iniPathWashTrayInfo, dtWashTrayIni);
                            }
                            #endregion
                            #endregion
                            BeginInvoke(WashTrayGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ": " + "清洗盘放入反应管" });
                        }
                        if (chbWash.Checked)
                        {
                            BeginInvoke(WashTrayGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ": " + "正在进行清洗。。" });
                            #region 吸液
                            //清洗盘逆时针旋转4个孔位到吸液位置
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 04"), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
                                InitControlEnable();
                                return;
                            }
                            currentHoleNum = currentHoleNum + 4;
                            //如果孔号超过30
                            if (currentHoleNum > WashTrayNum)
                            {
                                currentHoleNum = currentHoleNum - WashTrayNum;
                            }
                            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                            stepGoon = stepGoon + 4;
                            //修改清洗盘配置文件
                            OperateIniFile.WriteIniData("TubePosition", "no1", "0", iniPathWashTrayInfo);
                            OperateIniFile.WriteIniData("TubePosition", "no5", "1", iniPathWashTrayInfo);
                            //修改仪器参数取放管位置当前孔号
                            #region 逆时针转动清洗盘，取放管位置管号变化
                            currentHoleNum = currentHoleNum + 4;
                            if (currentHoleNum > WashTrayNum)
                            {
                                currentHoleNum = currentHoleNum - WashTrayNum;
                            }
                            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                            #endregion
                            //清洗盘吸液
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 01"), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
                                InitControlEnable();
                                return;
                            }
                            #endregion
                            #region 注液
                            //清洗盘逆时针旋转1个孔位到注液位置
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
                                InitControlEnable();
                                return;
                            }
                            currentHoleNum = currentHoleNum + 1;
                            //如果孔号超过30，孔号设为1
                            if (currentHoleNum > WashTrayNum)
                            {
                                currentHoleNum = currentHoleNum - WashTrayNum;
                            }
                            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                            stepGoon = stepGoon + 1;
                            //修改清洗盘配置文件
                            OperateIniFile.WriteIniData("TubePosition", "no5", "0", iniPathWashTrayInfo);
                            OperateIniFile.WriteIniData("TubePosition", "no6", "1", iniPathWashTrayInfo);
                            //修改仪器参数取放管位置当前孔号
                            #region 逆时针转动清洗盘，取放管位置管号变化
                            currentHoleNum = currentHoleNum + 1;
                            if (currentHoleNum > WashTrayNum)
                            {
                                currentHoleNum = currentHoleNum - WashTrayNum;
                            }
                            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                            #endregion
                            //注液
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 10"), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
                                InitControlEnable();
                                return;
                            }
                            #endregion

                            #region 吸液
                            //清洗盘逆时针旋转3个孔位到吸液位置
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 03"), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
                                InitControlEnable();
                                return;
                            }
                            currentHoleNum = currentHoleNum + 3;
                            //如果孔号超过30，孔号设为1
                            if (currentHoleNum > WashTrayNum)
                            {
                                currentHoleNum = currentHoleNum - WashTrayNum;
                            }
                            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                            stepGoon = stepGoon + 3;
                            //修改清洗盘配置文件
                            OperateIniFile.WriteIniData("TubePosition", "no6", "0", iniPathWashTrayInfo);
                            OperateIniFile.WriteIniData("TubePosition", "no9", "1", iniPathWashTrayInfo);
                            //修改仪器参数取放管位置当前孔号
                            #region 逆时针转动清洗盘，取放管位置管号变化
                            currentHoleNum = currentHoleNum + 3;
                            if (currentHoleNum > WashTrayNum)
                            {
                                currentHoleNum = currentHoleNum - WashTrayNum;
                            }
                            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                            #endregion
                            //清洗盘吸液
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 01"), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
                                InitControlEnable();
                                return;
                            }
                            #endregion
                            #region 注液
                            //清洗盘逆时针旋转1个孔位到注液位置
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
                                InitControlEnable();
                                return;
                            }
                            currentHoleNum = currentHoleNum + 1;
                            //如果孔号超过30，孔号设为1
                            if (currentHoleNum > WashTrayNum)
                            {
                                currentHoleNum = currentHoleNum - WashTrayNum;
                            }
                            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                            stepGoon = stepGoon + 1;
                            //修改清洗盘配置文件
                            OperateIniFile.WriteIniData("TubePosition", "no9", "0", iniPathWashTrayInfo);
                            OperateIniFile.WriteIniData("TubePosition", "no10", "1", iniPathWashTrayInfo);
                            //修改仪器参数取放管位置当前孔号
                            #region 逆时针转动清洗盘，取放管位置管号变化
                            currentHoleNum = currentHoleNum + 1;
                            if (currentHoleNum > WashTrayNum)
                            {
                                currentHoleNum = currentHoleNum - WashTrayNum;
                            }
                            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                            #endregion
                            //清洗盘注液
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 01"), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
                                InitControlEnable();
                                return;
                            }
                            #endregion

                            #region 吸液
                            //清洗盘逆时针旋转3个孔位到吸液位置
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 03"), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
                                InitControlEnable();
                                return;
                            }
                            currentHoleNum = currentHoleNum + 3;
                            //如果孔号超过30，孔号设为1
                            if (currentHoleNum > WashTrayNum)
                            {
                                currentHoleNum = currentHoleNum - WashTrayNum;
                            }
                            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                            stepGoon = stepGoon + 3;
                            //修改清洗盘配置文件
                            OperateIniFile.WriteIniData("TubePosition", "no10", "0", iniPathWashTrayInfo);
                            OperateIniFile.WriteIniData("TubePosition", "no13", "1", iniPathWashTrayInfo);
                            //修改仪器参数取放管位置当前孔号
                            #region 逆时针转动清洗盘，取放管位置管号变化
                            currentHoleNum = currentHoleNum + 3;
                            if (currentHoleNum > WashTrayNum)
                            {
                                currentHoleNum = currentHoleNum - WashTrayNum;
                            }
                            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                            #endregion
                            //清洗盘吸液
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 01"), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
                                InitControlEnable();
                                return;
                            }
                            #endregion
                            #region 注液
                            //清洗盘逆时针旋转1个孔位到注液位置
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 01"), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
                                InitControlEnable();
                                return;
                            }
                            currentHoleNum = currentHoleNum + 1;
                            //如果孔号超过30，孔号设为1
                            if (currentHoleNum > WashTrayNum)
                            {
                                currentHoleNum = currentHoleNum - WashTrayNum;
                            }
                            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                            stepGoon = stepGoon + 1;
                            //修改清洗盘配置文件
                            OperateIniFile.WriteIniData("TubePosition", "no13", "0", iniPathWashTrayInfo);
                            OperateIniFile.WriteIniData("TubePosition", "no14", "1", iniPathWashTrayInfo);
                            //修改仪器参数取放管位置当前孔号
                            #region 逆时针转动清洗盘，取放管位置管号变化
                            currentHoleNum = currentHoleNum + 1;
                            if (currentHoleNum > WashTrayNum)
                            {
                                currentHoleNum = currentHoleNum - WashTrayNum;
                            }
                            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                            #endregion
                            //清洗盘注液
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 10"), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
                                InitControlEnable();
                                return;
                            }
                            #endregion

                            #region 吸液
                            //清洗盘逆时针旋转3个孔位到吸液位置
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 03"), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
                                InitControlEnable();
                                return;
                            }
                            currentHoleNum = currentHoleNum + 3;
                            //如果孔号超过30，孔号设为1
                            if (currentHoleNum > WashTrayNum)
                            {
                                currentHoleNum = currentHoleNum - WashTrayNum;
                            }
                            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                            stepGoon = stepGoon + 3;
                            //修改清洗盘配置文件
                            OperateIniFile.WriteIniData("TubePosition", "no14", "0", iniPathWashTrayInfo);
                            OperateIniFile.WriteIniData("TubePosition", "no17", "1", iniPathWashTrayInfo);
                            //修改仪器参数取放管位置当前孔号
                            #region 逆时针转动清洗盘，取放管位置管号变化
                            currentHoleNum = currentHoleNum + 3;
                            if (currentHoleNum > WashTrayNum)
                            {
                                currentHoleNum = currentHoleNum - WashTrayNum;
                            }
                            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                            #endregion
                            //清洗盘吸液
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 01"), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
                                InitControlEnable();
                                return;
                            }
                            #endregion
                            BeginInvoke(WashTrayGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ": " + "清洗完成" });
                        }
                        if (chbAddSu.Checked)
                        {
                            BeginInvoke(WashTrayGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ": " + "正在进行加底物。。" });
                            #region 清洗盘旋转
                            if (stepGoon == 16)
                            {
                                //清洗盘逆时针旋转2位到加底物位置
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 02"), 2);
                                if (!NetCom3.Instance.WashQuery())
                                {
                                    InitControlEnable();
                                    return;
                                }
                                currentHoleNum = currentHoleNum + 2;
                                //如果孔号超过30，孔号设为1
                                if (currentHoleNum > WashTrayNum)
                                {
                                    currentHoleNum = currentHoleNum - WashTrayNum;
                                }
                                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                                stepGoon = stepGoon + 2;
                                //修改清洗盘配置文件
                                OperateIniFile.WriteIniData("TubePosition", "no17", "0", iniPathWashTrayInfo);
                                OperateIniFile.WriteIniData("TubePosition", "no19", "1", iniPathWashTrayInfo);
                                //修改仪器参数取放管位置当前孔号
                                #region 逆时针转动清洗盘，取放管位置管号变化
                                currentHoleNum = currentHoleNum + 2;
                                if (currentHoleNum > WashTrayNum)
                                {
                                    currentHoleNum = currentHoleNum - WashTrayNum;
                                }
                                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                                #endregion

                            }
                            else
                            {
                                //清洗盘逆时针旋转18位到加底物位置
                                int trunHole = 18;
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + trunHole.ToString("x2")), 2);
                                if (!NetCom3.Instance.WashQuery())
                                {
                                    InitControlEnable();
                                    return;
                                }
                                currentHoleNum = currentHoleNum + 18;
                                //如果孔号超过30，孔号设为1
                                if (currentHoleNum > WashTrayNum)
                                {
                                    currentHoleNum = currentHoleNum - WashTrayNum;
                                }
                                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                                stepGoon = stepGoon + 18;
                                //修改清洗盘配置文件
                                OperateIniFile.WriteIniData("TubePosition", "no1", "0", iniPathWashTrayInfo);
                                OperateIniFile.WriteIniData("TubePosition", "no19", "1", iniPathWashTrayInfo);
                                //修改仪器参数取放管位置当前孔号
                                #region 逆时针转动清洗盘，取放管位置管号变化
                                currentHoleNum = currentHoleNum + 18;
                                if (currentHoleNum > WashTrayNum)
                                {
                                    currentHoleNum = currentHoleNum - WashTrayNum;
                                }
                                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                                #endregion
                            }
                            #endregion
                            #region 加底物指令
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 01 " + subpipe + "0"), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
                                InitControlEnable();
                                return;
                            }
                            #endregion
                            BeginInvoke(WashTrayGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ": " + "加底物完成" });

                        }
                        if (chbRead.Checked)
                        {
                            BeginInvoke(WashTrayGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ": " + "正在进行读数。。" });
                            #region 清洗盘旋转
                            //反应管现在在加底物位置
                            if (stepGoon == 18)
                            {
                                //清洗盘逆时针旋转6位到读数位置
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 06"), 2);
                                if (!NetCom3.Instance.WashQuery())
                                {
                                    InitControlEnable();
                                    return;
                                }
                                currentHoleNum = currentHoleNum + 6;
                                //如果孔号超过30，孔号设为1
                                if (currentHoleNum > WashTrayNum)
                                {
                                    currentHoleNum = currentHoleNum - WashTrayNum;
                                }
                                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                                stepGoon = stepGoon + 6;
                                //修改清洗盘配置文件
                                OperateIniFile.WriteIniData("TubePosition", "no19", "0", iniPathWashTrayInfo);
                                OperateIniFile.WriteIniData("TubePosition", "no25", "1", iniPathWashTrayInfo);
                                //修改仪器参数取放管位置当前孔号
                                #region 逆时针转动清洗盘，取放管位置管号变化
                                currentHoleNum = currentHoleNum + 6;
                                if (currentHoleNum > WashTrayNum)
                                {
                                    currentHoleNum = currentHoleNum - WashTrayNum;
                                }
                                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                                #endregion

                            }
                            //反应管现在在最后一次吸液位置
                            else if (stepGoon == 16)
                            {
                                //清洗盘逆时针旋转8位到加底物位置
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 08"), 2);
                                if (!NetCom3.Instance.WashQuery())
                                {
                                    InitControlEnable();
                                    return;
                                }
                                currentHoleNum = currentHoleNum + 8;
                                //如果孔号超过30，孔号设为1
                                if (currentHoleNum > WashTrayNum)
                                {
                                    currentHoleNum = currentHoleNum - WashTrayNum;
                                }
                                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                                stepGoon = stepGoon + 8;
                                //修改清洗盘配置文件
                                OperateIniFile.WriteIniData("TubePosition", "no17", "0", iniPathWashTrayInfo);
                                OperateIniFile.WriteIniData("TubePosition", "no25", "1", iniPathWashTrayInfo);
                                //修改仪器参数取放管位置当前孔号
                                #region 逆时针转动清洗盘，取放管位置管号变化
                                currentHoleNum = currentHoleNum + 8;
                                if (currentHoleNum > WashTrayNum)
                                {
                                    currentHoleNum = currentHoleNum - WashTrayNum;
                                }
                                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                                #endregion
                            }
                            //反应盘在初始位置
                            else
                            {
                                //清洗盘逆时针旋转24位到读数位置
                                int trunHole = 24;
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + trunHole.ToString("x2")), 2);
                                if (!NetCom3.Instance.WashQuery())
                                {
                                    InitControlEnable();
                                    return;
                                }
                                currentHoleNum = currentHoleNum + 24;
                                //如果孔号超过30，孔号设为1
                                if (currentHoleNum > WashTrayNum)
                                {
                                    currentHoleNum = currentHoleNum - WashTrayNum;
                                }
                                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                                stepGoon = stepGoon + 24;
                                //修改清洗盘配置文件
                                OperateIniFile.WriteIniData("TubePosition", "no1", "0", iniPathWashTrayInfo);
                                OperateIniFile.WriteIniData("TubePosition", "no25", "1", iniPathWashTrayInfo);
                                //修改仪器参数取放管位置当前孔号
                                #region 逆时针转动清洗盘，取放管位置管号变化
                                currentHoleNum = currentHoleNum + 24;
                                if (currentHoleNum > WashTrayNum)
                                {
                                    currentHoleNum = currentHoleNum - WashTrayNum;
                                }
                                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                                #endregion
                            }
                            #endregion
                            NetCom3.Instance.ReceiveHandel += GetReadNum;
                            for (int i = 0; i <= repeateRead; i++)
                            {
                                //发送读数指令
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 00 01"), 2);
                                if (!NetCom3.Instance.WashQuery())
                                {
                                    InitControlEnable();
                                    return;
                                }
                                //若读数完毕，发送指令查询读数的数据
                                //string readValue = "10";
                                //BeginInvoke(new Action(() => { txtReadValue.Text = readValue; }));
                                //BeginInvoke(WashTrayGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ": " + "PMT背景值：" + readValue.ToString() });

                            }
                            NetCom3.Instance.ReceiveHandel -= GetReadNum;

                        }
                        if (stepGoon != 0)
                        {
                            #region 清洗盘旋转
                            if (stepGoon == 16)
                            {
                                //清洗盘逆时针旋转14位
                                int turnHoleNum = 14;
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + turnHoleNum.ToString("x2")), 2);
                                if (!NetCom3.Instance.WashQuery())
                                {
                                    InitControlEnable();
                                    return;
                                }
                                currentHoleNum = currentHoleNum + 14;
                                //如果孔号超过30，孔号设为1
                                if (currentHoleNum > WashTrayNum)
                                {
                                    currentHoleNum = currentHoleNum - WashTrayNum;
                                }
                                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                                stepGoon = stepGoon + 14;
                                #region 逆时针转动清洗盘，取放管位置管号变化
                                currentHoleNum = currentHoleNum + 14;
                                if (currentHoleNum >= WashTrayNum)
                                {
                                    currentHoleNum = currentHoleNum - WashTrayNum;
                                }
                                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                                #endregion
                                //修改清洗盘配置文件
                                OperateIniFile.WriteIniData("TubePosition", "no17", "0", iniPathWashTrayInfo);
                                OperateIniFile.WriteIniData("TubePosition", "no1", "1", iniPathWashTrayInfo);
                            }

                            else if (stepGoon == 18)
                            {
                                //清洗盘逆时针旋转12位
                                int turnHoleNum = 12;
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + turnHoleNum.ToString("x2")), 2);
                                if (!NetCom3.Instance.WashQuery())
                                {
                                    InitControlEnable();
                                    return;
                                }
                                currentHoleNum = currentHoleNum + 12;
                                //如果孔号超过30，孔号设为1
                                if (currentHoleNum > WashTrayNum)
                                {
                                    currentHoleNum = currentHoleNum - WashTrayNum;
                                }
                                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                                stepGoon = stepGoon + 12;
                                #region 逆时针转动清洗盘，取放管位置管号变化
                                currentHoleNum = currentHoleNum + 12;
                                if (currentHoleNum >= WashTrayNum)
                                {
                                    currentHoleNum = currentHoleNum - WashTrayNum;
                                }
                                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                                #endregion
                                //修改清洗盘配置文件
                                OperateIniFile.WriteIniData("TubePosition", "no19", "0", iniPathWashTrayInfo);
                                OperateIniFile.WriteIniData("TubePosition", "no1", "1", iniPathWashTrayInfo);
                            }
                            else if (stepGoon == 24)
                            {
                                //清洗盘逆时针旋转6位
                                int turnHoleNum = 6;
                                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + turnHoleNum.ToString("x2")), 2);
                                if (!NetCom3.Instance.WashQuery())
                                {
                                    InitControlEnable();
                                    return;
                                }
                                currentHoleNum = currentHoleNum + 6;
                                //如果孔号超过30，孔号设为1
                                if (currentHoleNum > WashTrayNum)
                                {
                                    currentHoleNum = currentHoleNum - WashTrayNum;
                                }
                                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                                stepGoon = stepGoon + 6;
                                #region 逆时针转动清洗盘，取放管位置管号变化
                                currentHoleNum = currentHoleNum + 6;
                                if (currentHoleNum >= WashTrayNum)
                                {
                                    currentHoleNum = currentHoleNum - WashTrayNum;
                                }
                                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                                #endregion
                                //修改清洗盘配置文件
                                OperateIniFile.WriteIniData("TubePosition", "no25", "0", iniPathWashTrayInfo);
                                OperateIniFile.WriteIniData("TubePosition", "no1", "1", iniPathWashTrayInfo);
                            }
                            #endregion
                            BeginInvoke(WashTrayGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ": " + "清洗盘测试完成反应管夹出，扔到废弃处" });
                            #region 清洗盘扔废管
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
                            if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
                            {
                                InitControlEnable();
                                return;
                            }
                            OperateIniFile.WriteIniData("TubePosition", "no1", "0", iniPathWashTrayInfo);
                            #endregion
                        }
                        LoopNum--;
                        BeginInvoke(new Action(() =>
                        {
                            txtWashLoopNum.Text = LoopNum.ToString();
                        }));
                    }
                    BeginInvoke(WashTrayGroupTestInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ": " + "动作完成！" });
                    WashTrayFlag = false;
                    BeginInvoke(new Action(() =>
                    {
                        InitControlEnable();
                    }));
                    break;
                }
                #endregion

            }
            InitControlEnable();//add y 20180510
        }
        private void rdbtnTubeRack_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbtnTubeRack.Checked)
            {
                cmbTakeTubePos.Enabled = false;
                rdbtnReactTrayp.Enabled = true;
                rdbtnWashTrayP.Enabled = true;
                rdbtnAbandon.Enabled = true;
                //rdbtnReactTray.Enabled = true;
                //rdbtnWashTray.Enabled = true;
            }
            else if (rdbtnReactTray.Checked)
            {
                rdbtnAbandon.Enabled = true;
                rdbtnWashTrayP.Enabled = true;
                if (rdbtnReactTrayp.Checked)
                    rdbtnAbandon.Checked = true;
                rdbtnReactTrayp.Enabled = false;
                //rdbtnWashTray.Enabled = true;
                cmbTakeTubePos.Enabled = true;
                cmbTakeTubePos.Items.Clear();
                for (int i = 1; i <= ReactTrayHoleNum; i++)
                {
                    cmbTakeTubePos.Items.Add(i);
                }
                cmbTakeTubePos.SelectedIndex = 0;

            }
            else if (rdbtnWashTray.Checked)
            {
                rdbtnAbandon.Enabled = true;
                if (rdbtnWashTrayP.Checked)
                    rdbtnAbandon.Checked = true;
                rdbtnWashTrayP.Enabled = false;
                rdbtnReactTrayp.Enabled = true;
                //rdbtnReactTray.Enabled = true;
                cmbTakeTubePos.Enabled = false;
                cmbTakeTubePos.Items.Clear();

            }

        }
        private void rdbtnReactTrayp_CheckedChanged(object sender, EventArgs e)
        {

            if (rdbtnAbandon.Checked)
            {
                cmbPutTube.Enabled = false;
                rdbtnReactTray.Enabled = true;
                rdbtnWashTray.Enabled = true;
                //if (rdbtnTubeRack.Checked)
                //    rdbtnReactTray.Checked = true;
                rdbtnTubeRack.Enabled = true;
            }
            else if (rdbtnReactTrayp.Checked)
            {
                if (rdbtnReactTray.Checked)
                    rdbtnTubeRack.Checked = true;
                rdbtnReactTray.Enabled = false;
                rdbtnWashTray.Enabled = true;
                cmbPutTube.Enabled = true;
                cmbPutTube.Items.Clear();
                for (int i = 1; i <= ReactTrayHoleNum; i++)
                {
                    cmbPutTube.Items.Add(i);
                }
                cmbPutTube.SelectedIndex = 0;
                rdbtnTubeRack.Enabled = true;

            }
            else if (rdbtnWashTrayP.Checked)
            {
                rdbtnReactTray.Enabled = true;
                if (rdbtnWashTray.Checked)
                    rdbtnTubeRack.Checked = true;
                rdbtnWashTray.Enabled = false;
                cmbPutTube.Enabled = false;
                cmbPutTube.Items.Clear();
                rdbtnTubeRack.Enabled = true;
            }
        }
        /// <summary>
        /// 控件初始状态
        /// </summary>
        void InitControlEnable()
        {
            fbtnMoveStart.Enabled = true;
            fbtnMoveStop.Enabled = false;
            fbtnArmStart.Enabled = true;
            fbtnArmStop.Enabled = false;
            fbtnWashStart.Enabled = true;
            fbtnWashStop.Enabled = false;
            fbtnArmClearTube.Enabled = true;
            fbtnMoveClearTube.Enabled = true;
            fbtnWashClearTube.Enabled = true;
            fbtnWashEX.Enabled = true;
        }

        #region 移管手

        private void fbtnMoveStart_Click(object sender, EventArgs e)
        {

            if (!MoveTestCondition())
            {
                return;
            }
            txtMoveShowInfo.Clear();
            fbtnMoveStart.Enabled = false;
            fbtnMoveStop.Enabled = true;
            //fbtnWashEX.Enabled = false;
            //fbtnArmClearTube.Enabled = false;
            //fbtnMoveClearTube.Enabled = false;
            //fbtnWashClearTube.Enabled = false;

            MoveTubeFlag = true;
            AddSampleArmFlag = false;
            WashTrayFlag = false;

            MoveTubeReset();
            GroupTestRun = new Thread(new ThreadStart(TestRun));
            GroupTestRun.IsBackground = true;
            GroupTestRun.Start();
        }
        /// <summary>
        /// 移管手是否可以开始测试
        /// </summary>
        /// <returns></returns>
        bool MoveTestCondition()
        {
            if (!(chbTakeTube.Checked && chbPutTube.Checked))
            {
                if (int.Parse(txtTubeLoopNum.Text) > 1)
                {
                    msd.show("单独取放管时只可循环一次！");
                    txtTubeLoopNum.Clear();
                    txtTubeLoopNum.Focus();
                    return false;
                }
            }
            else if (chbTakeTube.Checked == false && chbPutTube.Checked == false)
            {
                msd.show("请选择需要测试的功能！");
                return false;
            }
            if (chbTakeTube.Checked)
            {
               if (rdbtnReactTray.Checked)
                {
                    //反应盘的个数
                    int TubeNum = QueryReactTrayTubeNum();
                    if (TubeNum == 0)
                    {
                        msd.show("温育盘中无管，无法取管");
                        return false;
                    }


                }
                else if (rdbtnWashTray.Checked)
                {

                    int TubeNum = QueryWashTrayTubeNum();
                    if (TubeNum == 0)
                    {
                        msd.show("清洗盘中无管，无法取管");
                        return false;
                    }
                }
            }
            if (chbPutTube.Checked)
            {

                if (rdbtnReactTrayp.Checked)
                {
                    int TubeNum = QueryReactTrayTubeNum();
                    if (TubeNum != 0)
                    {
                        List<int> reactPutTube = new List<int>();
                        for (int i = 0; i < int.Parse(txtTubeLoopNum.Text); i++)
                        {
                            int posNum = int.Parse(cmbPutTube.SelectedItem.ToString()) + i;
                            if (posNum > ReactTrayNum)
                            {
                                posNum =posNum- ReactTrayNum;
                            }
                            reactPutTube.Add(posNum);
                        }
                        foreach (int a in reactPutTube)
                        {
                            for (int j = 0; j < reactTubePos.Count; j++)
                            {
                                if (a == reactTubePos[j])
                                {
                                    msd.show("温育盘放管位置已有管，请先清空温育盘！");
                                    return false;
                                }
                            }
                        }

                    }
                    if (int.Parse(txtTubeLoopNum.Text) > ReactTrayHoleNum)
                    {
                        msd.show("温育盘的位置不够循环次数，请重新输入循环次数！");
                        txtTubeLoopNum.Clear();
                        txtTubeLoopNum.Focus();
                        return false;
                    }
                }
                else if (rdbtnWashTrayP.Checked)
                {
                    int TubeNum = QueryWashTrayTubeNum();
                    if (TubeNum != 0)
                    {
                        msd.show("请先清空清洗盘中的管再进行放管！");
                        return false;
                    }
                    if (int.Parse(txtTubeLoopNum.Text) > WashTrayNum)
                    {
                        msd.show("清洗盘的位置不够循环次数，请重新输入循环次数！");
                        txtTubeLoopNum.Clear();
                        txtTubeLoopNum.Focus();
                        return false;
                    }
                }
            }
            return true;
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
        /// 温育盘管位置
        /// </summary>
        List<int> reactTubePos = new List<int>();
        /// <summary>
        /// 查询温育盘现在反应管的个数
        /// </summary>
        /// <returns></returns>
        int QueryReactTrayTubeNum()
        {
            #region 检测反应盘管个数
            reactTubePos = new List<int>();
            //查询反应盘管信息
            DataTable dtReactTrayInfo = OperateIniFile.ReadConfig(iniPathReactTrayInfo);
            //反应盘上空反应管个数
            int sumReactTubeNum = 0;
            for (int i = 0; i < dtReactTrayInfo.Rows.Count; i++)
            {
                int TubeValue = int.Parse(dtReactTrayInfo.Rows[i][1].ToString());
                if (TubeValue > 0)
                {
                    reactTubePos.Add(int.Parse(dtReactTrayInfo.Rows[i][0].ToString().Substring(2)));
                    sumReactTubeNum++;
                }
            }
            #endregion
            return sumReactTubeNum;
        }
        /// <summary>
        /// 查询清洗盘中反应管个数
        /// </summary>
        /// <returns></returns>
        int QueryWashTrayTubeNum()
        {
            int sumWashTubeNum = 0;
            DataTable dtWashTrayIni = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
            for (int i = 0; i < dtWashTrayIni.Rows.Count; i++)
            {
                if (dtWashTrayIni.Rows[i][1].ToString() == "1")
                {
                    sumWashTubeNum++;
                }
            }

            return sumWashTubeNum;
        }
        private void fbtnMoveStop_Click(object sender, EventArgs e)
        {
            if (GroupTestRun != null)
                GroupTestRun.Abort();
            InitControlEnable();

        }

        private void fbtnMoveClearTube_Click(object sender, EventArgs e)
        {
            fbtnMoveClearTube.Enabled = false;
            washTrayTubeClear();
            reactTrayTubeClear();
            fbtnMoveClearTube.Enabled = true;
        }
        #endregion

        #region 加样臂
        private void fbtnArmStart_Click(object sender, EventArgs e)
        {

            if (!ArmTestCondition())
            {
                return;
            }
            fbtnArmStart.Enabled = false;
            fbtnArmStop.Enabled = true;
            //fbtnWashEX.Enabled = false;
            //fbtnArmClearTube.Enabled = false;
            //fbtnMoveClearTube.Enabled = false;
            //fbtnWashClearTube.Enabled = false;

            txtArmShowInfo.Clear();
            MoveTubeFlag = false;
            AddSampleArmFlag = true;
            WashTrayFlag = false;
            AddSampleArmReset();
            GroupTestRun = new Thread(new ThreadStart(TestRun));
            GroupTestRun.IsBackground = true;
            GroupTestRun.Start();

        }

        int sumReactTubeNum = 0;
        /// <summary>
        /// 加样臂开始组合测试条件判断
        /// </summary>
        /// <returns></returns>
        bool ArmTestCondition()
        {
            if (chbAddSample.Checked == false && chbAddSu.Checked == false && chbR1.Checked == false
                && chbR2.Checked == false && chbBeads.Checked == false)
            {
                msd.show("请选择需要测试的功能！");
                return false;
            }


            //#region 检测反应盘反应管个数，不足十个补齐，检查是否反应盘满管。
            //查询反应盘管信息
            DataTable dtReactTrayInfo = OperateIniFile.ReadConfig(iniPathReactTrayInfo);
            //反应盘上空反应管个数

            string TrayPos = "";
            for (int i = 0; i < dtReactTrayInfo.Rows.Count; i++)
            {
                if (int.Parse(dtReactTrayInfo.Rows[i][1].ToString()) >=1)
                {
                    sumReactTubeNum++;
                    //后一个值一直覆盖前一个最终的赋值为最后一个位置
                    TrayPos = dtReactTrayInfo.Rows[i][0].ToString();
                }
            }
            QueryTubePos();
            return true;
        }
        /// <summary>
        /// 查询温育盘第一个空反应管位置
        /// </summary>
        void QueryTubePos()
        {
            //查询反应盘管信息
            DataTable dtReactTrayInfo = OperateIniFile.ReadConfig(iniPathReactTrayInfo);
            //反应管的位置
            string TrayPos = "";
            for (int i = 0; i < dtReactTrayInfo.Rows.Count; i++)
            {
                if (int.Parse(dtReactTrayInfo.Rows[i][1].ToString().Trim()) >= 1)
                {
                    //后一个值一直覆盖前一个最终的赋值为最后一个位置
                    TrayPos = dtReactTrayInfo.Rows[i][0].ToString();
                    break;
                }
            }
            if (TrayPos == "")
            {
                cmbReactStartPos.SelectedIndex = 0;
            }
            else
            {
                cmbReactStartPos.SelectedItem = TrayPos.Substring(2);
            }
        }
        private void cmbReactStartPos_SelectedIndexChanged(object sender, EventArgs e)
        {
            //查询反应盘管信息
            DataTable dtReactTrayInfo = OperateIniFile.ReadConfig(iniPathReactTrayInfo);
            //反应管的位置
            string TrayPos = "";
            for (int i = 0; i < dtReactTrayInfo.Rows.Count; i++)
            {
                if (int.Parse(dtReactTrayInfo.Rows[i][1].ToString().Trim()) >= 1)
                {
                    //后一个值一直覆盖前一个最终的赋值为最后一个位置
                    TrayPos = dtReactTrayInfo.Rows[i][0].ToString();
                    break;
                }
            }
            //if (int.Parse(dtReactTrayInfo.Rows[cmbReactStartPos.SelectedIndex][1].ToString().Trim()) == 0 && TrayPos != "")
            //{
            //    msd.show("该位置无反应管,软件会自动选择可加液位置！");
            //    QueryTubePos();
            //}
            if (TrayPos == "")
            {
                QueryTubePos();
            }
        }
        private void fbtnArmStop_Click(object sender, EventArgs e)
        {
            if (GroupTestRun != null)
                GroupTestRun.Abort();
            InitControlEnable();//modify y 20180510
        }

        private void fbtnArmClearTube_Click(object sender, EventArgs e)
        {
            fbtnArmClearTube.Enabled = false;
            reactTrayTubeClear();
            fbtnArmClearTube.Enabled = true;
        }
        #endregion

        #region 清洗检测盘
        private void fbtnWashStart_Click(object sender, EventArgs e)
        {

            if (!washTrayTestCondition())
            {
                return;
            }
            if (chbAddSu.Checked&&cmbSubstrate.SelectedItem == null)
            {
                frmMsgShow.MessageShow("组合测试", "请选择底物管路！");
                cmbSubstrate.Focus();
                return;
            }
            fbtnWashStart.Enabled = false;
            //fbtnWashEX.Enabled = false;
            //fbtnArmClearTube.Enabled = false;
            //fbtnMoveClearTube.Enabled = false;
            //fbtnWashClearTube.Enabled = false;
            fbtnWashStop.Enabled = true;

            txtWashShowInfo.Clear();
            MoveTubeFlag = false;
            AddSampleArmFlag = false;
            WashTrayFlag = true;
            washTrayReset();
            if (chbAddSu.Checked)
            {
                if (cmbSubstrate.SelectedItem.ToString() == "1")
                {
                    subpipe = "1";
                }
                else if (cmbSubstrate.SelectedItem.ToString() == "2")
                {
                    subpipe = "2";
                }
            }
            GroupTestRun = new Thread(new ThreadStart(TestRun));
            GroupTestRun.IsBackground = true;
            GroupTestRun.Start();


        }
        /// <summary>
        /// 清洗检测盘开始组合测试条件判断
        /// </summary>
        /// <returns></returns>
        bool washTrayTestCondition()
        {
            if (chbWash.Checked == false && chbAddSu.Checked == false && chbRead.Checked == false)
            {
                msd.show("请选择测试的功能");
                return false;
            }

            #region 检查管架中的管是否够本次测试使用
            List<int> lisTubeNum = new List<int>();
            lisTubeNum = QueryTubeNum();
            int sumTubeNum = 0;
            for (int i = 0; i < lisTubeNum.Count; i++)
            {
                sumTubeNum = sumTubeNum + lisTubeNum[i];
            }
            if (sumTubeNum < int.Parse(txtWashLoopNum.Text))
            {
                msd.show("管架反应管不够本次测试使用请装载新的反应管！");
                return false;
            }
            #endregion

            #region 查询清洗盘中是否有管
            int washTubeNum = QueryWashTrayTubeNum();
            if (washTubeNum > 0)
            {
                msd.show("清洗盘中有反应管，请先清空清洗盘中的反应管！");
                return false;
            }
            #endregion
            return true;
        }
        private void fbtnWashStop_Click(object sender, EventArgs e)
        {
            if (GroupTestRun != null)
                GroupTestRun.Abort();
            NetCom3.Instance.ReceiveHandel -= GetReadNum;//add y 20180918
            InitControlEnable();//modify y 20180510
        }

        private void fbtnWashClearTube_Click(object sender, EventArgs e)
        {
            fbtnWashClearTube.Enabled = false;
            washTrayTubeClear();
            fbtnWashClearTube.Enabled = true;
        }
        #endregion

        /// <summary>
        /// 移管手相关模块复位
        /// </summary>
        void MoveTubeReset()
        {

        }
        /// <summary>
        /// 加样臂相关模块复位
        /// </summary>
        void AddSampleArmReset()
        {

        }
        /// <summary>
        /// 清洗盘相关模块复位
        /// </summary>
        void washTrayReset()
        {

        }


        /// <summary>
        /// 清洗盘清管
        /// </summary>
        void washTrayTubeClear()
        {
            DataTable dtWashTrayIni = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
            Delegate DLInfo = MoveGroupTestInfo;
            if (!fbtnMoveClearTube.Enabled)
            {
                DLInfo = MoveGroupTestInfo;
            }
            else if (!fbtnArmClearTube.Enabled)
            {
                DLInfo = ArmGroupTestInfo;
            }
            else if (!fbtnWashClearTube.Enabled)
            {
                DLInfo = WashTrayGroupTestInfo;
            }
            int tubeExist = 0;
            for (int i = 0; i < dtWashTrayIni.Rows.Count; i++)
            {
                if (dtWashTrayIni.Rows[i][1].ToString() == "1")
                {
                    tubeExist++;
                }

            }
            //if (tubeExist > 0)
            //{
                BeginInvoke(DLInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ": " + "正在清空清洗盘。。" });
                for (int i = 0; i < WashTrayNum; i++)
                {
                    if (i != 0)
                    {
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-1).ToString("X2").Substring(6, 2)), 2);
                        if (!NetCom3.Instance.WashQuery())
                        {
                            InitControlEnable();
                            return;
                        }
                        currentHoleNum = currentHoleNum - 1;
                        //如果孔号小于等于0
                        if (currentHoleNum <= 0)
                        {
                            currentHoleNum = currentHoleNum + WashTrayNum;
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
                    if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
                    {
                        InitControlEnable();
                        return;
                    }
                    OperateIniFile.WriteIniData("TubePosition", "No1", "0", iniPathWashTrayInfo);
                    #endregion
                }
                BeginInvoke(DLInfo, new object[] { DateTime.Now.ToString("HH-mm-ss") + ": " + "清空清洗盘完成。" });
            //}
            //else
            //{
            //    InitControlEnable();
            //    return;
            //}
           
        }
        /// <summary>
        /// 温育反应盘清管
        /// </summary>
        bool reactTrayTubeClear()
        {
            for (int i = 1; i < ReactTrayNum+1; i++)
            {
                if (isNewWashEnd()) return false;  //lyq add 20190822
                //if (CancellationToken)
                //{
                //    return;
                //}
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 05 " + i.ToString("x2")), 1);
                if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                {
                    MessageBox.Show("抓管出现异常，请确认仪器是否运行故障！");
                    return false;
                }
                //修改反应盘信息
                OperateIniFile.WriteIniData("ReactTrayInfo", "no" + i.ToString(), "0", iniPathReactTrayInfo);
            }
            return true;
        }
        
        private void frmInstruGroupTest_SizeChanged(object sender, EventArgs e)
        {
            formSizeChange(this);
        }

        private void frmInstruGroupTest_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (GroupTestRun != null)
                GroupTestRun.Abort();
            NetCom3.Instance.ReceiveHandel -= new Action<string>(Instance_ReceiveHandel);
        }

        private void fbtnInstruMaintenance_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmInstruMaintenance"))
            {
                frmInstruMaintenance frnIM = new frmInstruMaintenance();
                //this.TopLevel = false;
                frnIM.TopLevel = false;
                frnIM.Parent = this.Parent;
                frnIM.Show();
            }
            else
            {
                frmInstruMaintenance frnIM = (frmInstruMaintenance)Application.OpenForms["frmInstruMaintenance"];
                //frmIM.Activate();
                frnIM.BringToFront();

            }
        }

        private void fbtnInstruDiagnost_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmDiagnost"))
            {
                frmDiagnost frnID = new frmDiagnost();
                //this.TopLevel = false;
                frnID.TopLevel = false;
                frnID.Parent = this.Parent;
                frnID.Show();
            }
            else
            {
                frmDiagnost frnID = (frmDiagnost)Application.OpenForms["frmDiagnost"];
                //frmIM.Activate();
                frnID.BringToFront();

            }
        }

        private void fbtnReturn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chbTakeTube_CheckedChanged(object sender, EventArgs e)
        {
            chbPutTube.Checked = chbTakeTube.Checked;
        }

        private void fbtnWashEX_Click(object sender, EventArgs e)
        {
            if (cmbWashHole.SelectedItem == null)
            {
                frmMsgShow.MessageShow("组合测试", "请选择孔号！");
                return;
            }
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 02 " + int.Parse(cmbWashHole.SelectedItem.ToString()).ToString("X2")), 2);
            NetCom3.Instance.WashQuery();
        }
        bool isNewWashRun = false;
        bool CancellationToken = false;
        private void functionButton2_Click(object sender, EventArgs e)
        {
            panel4.Visible = panel4.Enabled = true;
        }

        private void ReBack_Click(object sender, EventArgs e)
        {
            panel4.Visible = panel4.Enabled = false;
        }
        private void NewWashStar_Click(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
            {
                if (!string.IsNullOrEmpty(numDwPourin.Value.ToString()))
                {
                    if (string.IsNullOrEmpty(OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", iniPathSubstrateTube))) 
                    {
                        MessageBox.Show(" 底物测数不足，请装载");
                        return;
                    }

                    int left = int.Parse(OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", iniPathSubstrateTube));
                    if (left < int.Parse(numDwPourin.Value.ToString()))
                    {
                        MessageBox.Show(" 底物测数不足，请装载");
                        return;
                    }
                }
            }

            if (isNewWashRun)
            {
                MessageBox.Show("正在运行。");
                return;
            }
            //清空，新管，清洗，底物，读数，扔管
            bool[] flow = new bool[6] { checkBox6.Checked, checkBox1.Checked, checkBox2.Checked, checkBox3.Checked, checkBox4.Checked, checkBox5.Checked };
            int starhole;
            int whichPipe = 1;
            if (!(int.TryParse(comboBox2.Text, out starhole)))// && (flow[3] && int.TryParse(comboBox1.Text, out whichPipe) || !flow[3])
            {
                MessageBox.Show("参数不正确");
                return;
            }
            if (flow[2] && !flow[3])
            {
                MessageBox.Show("进行清洗时，必须同时原则加底物。");
                return;
            }
            if (checkBox3.Checked)
            {
                int LeftCount1 = int.Parse(OperateIniFile.ReadIniData("Substrate" + whichPipe + "", "LeftCount", "", iniPathSubstrateTube));
                int CleanTray = 0;
                if (cbCleanTray.Checked)
                {
                    CleanTray = 1;
                }
                if (LeftCount1 < numericUpDown3.Value + CleanTray)
                {
                    MessageBox.Show("底物不足，请及时更换。");
                    return;
                }
            }
            int tubeNum = (int)numericUpDown3.Value;
            int repeatRead = (int)numericUpDown2.Value;

            isNewWashRun = true;
            CancellationToken = false;
            functionButton3.Enabled = false;
            functionButton4.Enabled = false;
            functionButton5.Enabled = true;
            while (!this.IsHandleCreated)
            {
                Thread.Sleep(30);
            }
            textBox1.Invoke(new Action(() => { textBox1.Text = ""; }));
            if (cbClearTray.Checked)
            {
                TExtAppend("开始清空温育盘。。。\n");
                if (!reactTrayTubeClear())
                {
                    NewWashEnd();
                    return;
                }
                TExtAppend("清空温育盘结束。。。\n");
            }
            if (isNewWashEnd()) return;  //lyq add 20190822
            //if (CancellationToken)
            //{
            //    return;
            //}
            if (flow[0])//清空
            {
                TExtAppend("开始清空");
                tubeHoleNum = starhole;
                for (int i = 0; i < WashTrayNum; i++)
                {
                    if (isNewWashEnd()) return;  //lyq add 20190822
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
                    //打印当前取放管孔位到Log。  jun 2019/1/22
                    LogFile.Instance.Write(string.Format("***{0}->:{1}***", "取放管时间: " + DateTime.Now.ToString("HH:mm:ss:fff"), "管孔位置: " + tubeHoleNum));
                    if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
                    {
                        NewWashEnd(1);
                        return;
                    }
                    if (isNewWashEnd()) return;  //lyq add 20190822
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-1).ToString("X2").Substring(6, 2)), 2);
                    //逆时针计数。  Jun 2019/1/22
                    if (tubeHoleNum <= 0)
                    {
                        tubeHoleNum = tubeHoleNum + WashTrayNum;
                    }
                    tubeHoleNum = tubeHoleNum - 1; 
                    if (!NetCom3.Instance.WashQuery())
                    {
                        NewWashEnd(3);
                        return;
                    }
                    if (isNewWashEnd()) return;  //lyq add 20190822
                }
                TExtAppend("清空完成");
            }
            if (checkBox7.Checked)
            {
                SubstratePipeline();
            }
            if (isNewWashEnd()) return;  //lyq add 20190822
            TExtAppend("转到开始孔位"+starhole);
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 02 " + (starhole).ToString("X2")), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                NewWashEnd(3);
                return;
            }
            CleanTray tray = new CleanTray(starhole);
            int temptubenum = tubeNum;
            int tempread = 1;
            int tubecount = 1;
            for (int i = 0; i < WashTrayNum + tubeNum; i++)
            {
                if (isNewWashEnd()) return;
                if (!CheckFormIsOpen("frmInstruGroupTest"))
                {
                    NetCom3.Instance.ReceiveHandel -= GetReadNum2;
                    return;
                }
                if (temptubenum>0)//夹新管
                {
                    if (flow[1])
                    {
                        TExtAppend("正在加第"+(tubeNum - temptubenum+1)+"个新管");
                        //string takeTubePos = OperateIniFile.ReadIniData("Tube", "TubePos", "1", iniPathSubstrateTube);
                        ////移管手到取管位置takepos[1]（管架）取管
                        //int plate = int.Parse(takeTubePos) % 88 == 0 ? int.Parse(takeTubePos) / 88 - 1 : int.Parse(takeTubePos) / 88;//几号板
                        //int column = int.Parse(takeTubePos) % 11 == 0 ? int.Parse(takeTubePos) / 11 - (plate * 8) : int.Parse(takeTubePos) / 11 + 1 - (plate * 8);
                        //int hole = int.Parse(takeTubePos) % 11 == 0 ? 11 : int.Parse(takeTubePos) % 11;
                        //到管架takepos[1]位置取管放到温育盘putpos位置
                        NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06"), 1);
                        if (!NetCom3.Instance.MoveQuery())
                        {
                            NewWashEnd(1);
                            return;
                        }
                        //int nextpos = plate*88 + (column-1)*11 + hole +1;
                        //nextpos = nextpos > 352 ? 1 : nextpos;
                        //OperateIniFile.WriteIniData("Tube", "TubePos", nextpos.ToString(), iniPathSubstrateTube);
                    }
                    tray.pointer[0].Value[1] = 1;
                    temptubenum--;
                }
                if (isNewWashEnd()) return;
                
                if (tray.needStay(flow[2],flow[3],flow[4]))//清洗盘工作
                {
                    //dw2018.12.25 
                    if (flow[4] && tray.pointer[9].Value[1] == 1 && tempread <= tubeNum)
                    //if (flow[4] && tray.pointer[9].Value[1] == 1)
                    {
                        NetCom3.Instance.ReceiveHandel += GetReadNum2;
                        TExtAppend("第"+tempread+"个管正在读数");
                        tempread++;
                    }
                    NetCom3.Instance.Send(NetCom3.Cover(tray.GetWashOrder(flow[2],flow[3],flow[4],whichPipe)), 2);
                    if (!NetCom3.Instance.WashQuery())
                    {
                        NewWashEnd(3);
                        return;
                    }
                    else
                    {
                        if (flow[3] && tray.pointer[8].Value[1] == 1)
                        {
                            string LeftCount1 = OperateIniFile.ReadIniData("Substrate" + whichPipe + "", "LeftCount", "", iniPathSubstrateTube);
                            OperateIniFile.WriteIniData("Substrate" + whichPipe + "", "LeftCount", (int.Parse(LeftCount1) - 1).ToString(), iniPathSubstrateTube);
                            string sbCode1 = OperateIniFile.ReadIniData("Substrate1", "BarCode", "0", iniPathSubstrateTube);
                            string sbNum1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", iniPathSubstrateTube);
                            DbHelperOleDb dbase = new DbHelperOleDb(3);
                            DbHelperOleDb.ExecuteSql(3, @"update tbSubstrate set leftoverTest =" + sbNum1 + " where BarCode = '" + sbCode1 + "'");
                            substrateNum1 = int.Parse(OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", iniPathSubstrateTube));
                        }
                    }
                    if (flow[4] && tray.pointer[9].Value[1] == 1)//重复读数
                    {
                        for (int j = 1; j < repeatRead; j++)
                        {
                            if (isNewWashEnd()) return;  //lyq add 20190822
                            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 00 11"), 2);
                            if (!NetCom3.Instance.WashQuery())
                            {
                                NewWashEnd(3);
                                return;
                            }
                        }
                        Thread.Sleep(500);
                        NetCom3.Instance.ReceiveHandel -= GetReadNum2;
                    }
                }
                if (isNewWashEnd()) return;  //lyq add 20190822
                if (flow[5] && tray.pointer[10].Value[1] == 1)
                {
                    TExtAppend("正在从清洗盘扔第" + tubecount + "个管。");
                    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 01"), 1);
                    if (!NetCom3.Instance.MoveQuery()&&NetCom3.Instance.MoverrorFlag!=(int)ErrorState.IsNull)
                    {
                        NewWashEnd(1);
                        return;
                    }
                    tubecount++;
                    tray.pointer[10].Value[1] = 0;
                }
                if (isNewWashEnd()) return;
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (1).ToString("X2")), 2);
                if (!NetCom3.Instance.WashQuery())
                {
                    NewWashEnd(3);
                    return;
                }
                tray.PoninterMoveOnePace();
                if (chkDelay.Checked)
                    NetCom3.Delay(20000);
                else
                    Thread.Sleep(500);
            }
            if (cbCleanTray.Checked)
            {
                LoopPourinto = 1;
                bLoopRun = true;
                TExtAppend("管路灌注开始。。。\n");
                TestLoopRun();
                //functionButton6_Click(sender, e);
            }
            NewWashEnd();
        }
        /// <summary>
        /// 底物管路灌注  /2019.8.5  zlx  add
        /// </summary>
        void SubstratePipeline()
        {
            if (isNewWashEnd()) return;  //lyq add 20190822
            substrateNum1 = int.Parse(OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", iniPathSubstrateTube));
            TExtAppend("\n底物管路灌注。。。");
            int pos1 = 19;
            //管架取管位置
            //int Num = 20;
            int Num = int.Parse(numDwPourin.Value.ToString());
            if (Num == 0)
                Num = 20;
            string subPipe = "";
            subPipe = "1";
            if (isNewWashEnd()) return;  //lyq add 20190822
            #region 清洗盘顺时针18位，然后放管
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (1 - pos1).ToString("X2").Substring(6, 2)), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                return;
            }
            currentHoleNum = currentHoleNum + (pos1 - 1);
            //若当前管号等于0，说明转过来的孔号为30
            if (currentHoleNum > WashTrayNum)
            {
                currentHoleNum = currentHoleNum - WashTrayNum;
            }
            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
            //OperateIniFile.WriteIniData("TubePosition", "No19", "1", iniPathWashTrayInfo);
            int iNeedCool = 0;
        //夹新管到清洗盘
        AgainNewMove3:
            if (isNewWashEnd()) return;  //lyq add 20190822
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 06 "), 1);
            if (!NetCom3.Instance.MoveQuery())
            {
                if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull)
                {
                    iNeedCool++;
                    if (iNeedCool < 2)
                    {
                        goto AgainNewMove3;
                    }
                    else
                    {
                        frmMsgShow.MessageShow("组合测试", "移管手多次抓空，请装载管架！");
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            #region 取放管成功，相关配置文件修改
            OperateIniFile.WriteIniData("TubePosition", "No19", "1", iniPathWashTrayInfo);
            #endregion
            #endregion
            if (isNewWashEnd()) return;  //lyq add 20190822
            #region 清洗盘逆时针旋转19位，回到19号孔，加底物位置
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (pos1).ToString("X2")), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                return;
            }
            currentHoleNum = currentHoleNum - pos1 + 1;
            //若当前管号等于0，说明转过来的孔号为30
            if (currentHoleNum > WashTrayNum)
            {
                currentHoleNum = currentHoleNum - WashTrayNum;
            }
            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
            #endregion
            while (Num > 0)
            {
                if (isNewWashEnd()) return;  //lyq add 20190822
                #region 底物注液
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 00 00 01 " + subPipe + "0"), 2);
                if (!NetCom3.Instance.WashQuery())
                {
                    //fbtnStart.Enabled = true;
                    //fbtnStop.Enabled = false;
                    return;
                }
                if (subPipe == "1")
                {
                    substrateNum1 = substrateNum1 - 1;
                    OperateIniFile.WriteIniData("Substrate1", "LeftCount", substrateNum1.ToString(), iniPathSubstrateTube);
                    string sbCode1 = OperateIniFile.ReadIniData("Substrate1", "BarCode", "0", iniPathSubstrateTube);
                    string sbNum1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", iniPathSubstrateTube);
                    DbHelperOleDb dbase = new DbHelperOleDb(3);
                    DbHelperOleDb.ExecuteSql(3, @"update tbSubstrate set leftoverTest =" + sbNum1 + " where BarCode = '" + sbCode1 + "'");
                    substrateNum1 = int.Parse(OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", iniPathSubstrateTube));
                }
                else
                {
                    substrateNum2 = substrateNum2 - 1;
                    OperateIniFile.WriteIniData("Substrate1", "LeftCount", substrateNum2.ToString(), iniPathSubstrateTube);
                }
                #endregion
                if (isNewWashEnd()) return;  //lyq add 20190822
                #region 清洗盘顺时针旋转2位，底物吸液位置
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-2).ToString("X2").Substring(6, 2)), 2);
                if (!NetCom3.Instance.WashQuery())
                {
                    return;
                }
                currentHoleNum = currentHoleNum + 2;
                //若当前管号等于0，说明转过来的孔号为30
                if (currentHoleNum > WashTrayNum)
                {
                    currentHoleNum = currentHoleNum - WashTrayNum;
                }
                OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
                #region 吸液
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 03 01"), 2);
                if (!NetCom3.Instance.WashQuery())
                {

                    return;
                }
                #endregion
                #endregion
                if (isNewWashEnd()) return;  //lyq add 20190822
                #region 清洗盘转回底物注液位置，19号位
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (2).ToString("X2")), 2);
                if (!NetCom3.Instance.WashQuery())
                {

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
                numDwPourin.Value = Num;
                Thread.Sleep(500);
            }
            if (isNewWashEnd()) return;  //lyq add 20190822
            #region 加底物位置扔废管
            //清洗盘逆时针旋转12位转到取放管位置
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (11).ToString("X2")), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                return;
            }

            currentHoleNum = currentHoleNum + 18;
            //若当前管号等于0，说明转过来的孔号为30
            if (currentHoleNum > WashTrayNum)
            {
                currentHoleNum = currentHoleNum - WashTrayNum;
            }
            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
            if (!NetCom3.Instance.MoveQuery())
            {
                return;
            }
            OperateIniFile.WriteIniData("TubePosition", "No19", "0", iniPathWashTrayInfo);
            #endregion
            TExtAppend("\n底物管路灌注完成。。。");
        }
        bool isNewWashEnd()
        {
            if (CancellationToken)
            {
                NewWashEnd();
                return true;
            }
            else return false;
        }
        void NewWashEnd(int errorflag=0)
        {
            if (errorflag == 1)
            {
                if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked)
                    TExtAppend("抓手在取管位置发生了撞管。");
                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.putKnocked)
                    TExtAppend("抓手在放管位置发生了撞管。");
                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.LackTube)
                    TExtAppend("理杯机位置缺管。");
                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.OverTime)
                    TExtAppend("移管手指令接收超时。");

            }
            else if (errorflag == 3)
            {
                if (NetCom3.Instance.WasherrorFlag == (int)ErrorState.OverTime)
                    TExtAppend("清洗指令接收超时。");
            }
            else if (errorflag == 2)
            {
                if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.OverTime)
                    TExtAppend("加样指令接收超时。");
                else if (NetCom3.Instance.AdderrorFlag == (int)ErrorState.IsKnocked)
                    TExtAppend("加样针加样时发生撞针。");
            }
            isNewWashRun = false;
            CancellationToken = true; //lyq add 20190822
            //CancellationToken = false;
            functionButton3.Enabled = true;
            NetCom3.Instance.ReceiveHandel -= GetReadNum2;
            TExtAppend("已结束。");
            functionButton4.Enabled = true;
            //functionButton5.Enabled = false;
        }
        void TExtAppend(string text)
        {
            while (!this.IsHandleCreated)
            {
                Thread.Sleep(15);
            }
            textBox1.Invoke(new Action(() => { textBox1.AppendText(Environment.NewLine + text); }));
        }
        private void NewWashEnd_Click(object sender, EventArgs e)
        {
            if (CancellationToken || !isNewWashRun)
            {
                MessageBox.Show("已经准备终止或者实验没有在运行。");
                return;
            }
            CancellationToken = true;
            NewWashEnd();
        }
        private void GetReadNum2(string order)
        {
            if (order.Contains("EB 90 31 A3"))
            {
                string temp = order.Replace(" ", "");
                int pos = temp.IndexOf("EB9031A3");
                temp = temp.Substring(pos, 32);
                temp = temp.Substring(temp.Length - 8);
                temp = Convert.ToInt64(temp, 16).ToString();
                if (int.Parse(temp) > Math.Pow(10, 5))
                    temp = ((int)GetPMT(double.Parse(temp))).ToString();
                TExtAppend(DateTime.Now.ToString("HH-mm-ss") + ": " + "PMT背景值：" + temp );
            }
        }
        class CleanTray
        {
            LinkedList<int[]> tray = new LinkedList<int[]>();
            public List<LinkedListNode<int[]>> pointer{get;set;}
            
            public CleanTray()
            {
                pointer = new List<LinkedListNode<int[]>>(11);
                for (int i = 0; i < WashTrayNum; i++)
                {
                    int[] temp = new int[2] { i + 1, 0 };
                    LinkedListNode<int[]> tempnode = new LinkedListNode<int[]>(temp);
                    tray.AddLast(tempnode);
                    if (i == 0) pointer.Add(tempnode);
                    if (i == 5) pointer.Add(tempnode);
                    if (i == 6) pointer.Add(tempnode);
                    if (i == 9) pointer.Add(tempnode);
                    if (i == 10) pointer.Add(tempnode);
                    if (i == 13) pointer.Add(tempnode);
                    if (i == 14) pointer.Add(tempnode);
                    if (i == 17) pointer.Add(tempnode);
                    if (i == 19) pointer.Add(tempnode);
                    if (i == 24) pointer.Add(tempnode);
                    if (i == 28) pointer.Add(tempnode);
                }
            }
            public CleanTray(int starhole)
                : this()
            {
                while (starhole != pointer[0].Value[0])
                {
                    PoninterMoveOnePace();
                }
            }
            public void PoninterMoveOnePace()
            {
                for (int i = 0; i < 11; i++)
                {
                    pointer[i] = pointer[i].Previous;
                    if (pointer[i] == null) pointer[i] = tray.Last;
                }
            }

            public bool needStay(bool wash, bool addbase, bool read)
            {
                bool stay = false;
                if (wash)
                {
                    for (int i = 1; i < 8; i++)
                    {
                        if (pointer[i].Value[1] == 1)
                        {
                            stay = true;
                            return stay;
                        }
                    }
                }
                if (addbase && pointer[8].Value[1] == 1 || (read && pointer[9].Value[1] == 1))
                {
                    stay = true;
                    return stay;
                }
                return stay;
            }
            public string GetWashOrder(bool wash, bool addbase, bool read,int washPipe)
            {
                StringBuilder order = new StringBuilder(64);
                order.Append("EB 90 31 03 03");
                if (wash && (pointer[1].Value[1] == 1 || pointer[3].Value[1] == 1 || pointer[5].Value[1] == 1 || pointer[7].Value[1] == 1 ))
                {
                    order.Append(" 01 ");
                }
                else order.Append(" 00 ");
                if (wash && pointer[2].Value[1] == 1 )
                {
                    order.Append("1");
                }
                else order.Append("0");
                if (wash && pointer[4].Value[1] == 1)
                {
                    order.Append("1 ");
                }
                else order.Append("0 ");
                if (wash && pointer[6].Value[1] == 1)
                {
                    order.Append("1");
                }
                else order.Append("0");
                if (addbase && pointer[8].Value[1] == 1)
                {
                    order.Append("1 ");
                }
                else order.Append("0 ");
                order.Append(washPipe);
                if (read && pointer[9].Value[1] == 1)
                {
                    order.Append("1");
                }
                else order.Append("0");
                return order.ToString();
            }
        }
        bool isAddBase = false;
        private void functionButton1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(numericUpDown4.Value.ToString()))
            {
                if (string.IsNullOrEmpty(OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", iniPathSubstrateTube)))
                {
                    MessageBox.Show(" 底物测数不足，请装载");
                    return;
                }
                int left = int.Parse(OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", iniPathSubstrateTube));
                if (left < int.Parse(numericUpDown4.Value.ToString()))
                {
                    MessageBox.Show(" 底物测数不足，请装载");
                    return;
                }
            }


            if (isNewWashRun)
            {
                MessageBox.Show("请在仪器空闲时执行此操作");
                return;
            }
            if (numericUpDown4.Value <= 0)
            {
                MessageBox.Show("请输入大于0的数");
                return;
            }
            int basenum = (int)numericUpDown4.Value;
            string LeftCount1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "", iniPathSubstrateTube);
            CancellationToken = false;
            isNewWashRun = true;
            int leftcount1 = int.Parse(LeftCount1);
            if (leftcount1 < basenum)
            {
                MessageBox.Show("底物不足，请及时更换！");
                return;
            }
            functionButton1.Enabled = false;
            isAddBase = true;
            for (int i = 1; i <= basenum; i++)
            {
                if (isNewWashEnd())//lyq add 20190822
                {
                    functionButton1.Enabled = true;
                    return;
                }
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 11 07 07"), 5);
                if (!NetCom3.Instance.SingleQuery() && NetCom3.Instance.errorFlag != (int)ErrorState.ReadySend)
                {
                    frmMsgShow.MessageShow("ERROR", "错误类型为 " + Enum.GetName(typeof(ErrorState), NetCom3.Instance.errorFlag));
                    functionButton1.Enabled = true;
                    return;
                }
                leftcount1 = leftcount1 - 1;
                OperateIniFile.WriteIniData("Substrate1", "LeftCount", leftcount1.ToString(), iniPathSubstrateTube);
                string sbCode1 = OperateIniFile.ReadIniData("Substrate1", "BarCode", "0", iniPathSubstrateTube);
                string sbNum1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", iniPathSubstrateTube);
                DbHelperOleDb dbase = new DbHelperOleDb(3);
                DbHelperOleDb.ExecuteSql(3, @"update tbSubstrate set leftoverTest =" + sbNum1 + " where BarCode = '" + sbCode1 + "'");
                substrateNum1 = int.Parse(OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", iniPathSubstrateTube));
                numericUpDown4.Value = basenum - i;
                Thread.Sleep(500);
            }
            functionButton1.Enabled = true;
            isAddBase = false;
            isNewWashRun = false;
            CancellationToken = true;
        }

        int isNewCleanTray = 1;
        int substrateNum1;
        int substrateNum2;
        Thread Loopthread = null;
        private void functionButton6_Click(object sender, EventArgs e)
        {
            if (functionButton6.Text == "循环灌注")
            {
                //functionButton6.Enabled = false;
                functionButton6.Text = "停止灌注";
                bLoopRun = true;
                CancellationToken = false;
                if (string.IsNullOrEmpty(OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", iniPathSubstrateTube)))
                {
                    MessageBox.Show(" 底物测数不足，请装载");
                    return;
                }
                substrateNum1 = int.Parse(OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", iniPathSubstrateTube));
                //substrateNum2 = int.Parse(OperateIniFile.ReadIniData("Substrate2", "LeftCount", "0", iniPathSubstrateTube));

                if (!bLoopRun) return;
                if (numericUpDown5.Value.ToString().Trim() == "0")
                {
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow("温馨提示", "灌注次数为0，请输入适当循环次数！");
                    functionButton6.Enabled = true;
                    functionButton6.Text = "循环灌注";
                    bLoopRun = true;
                    return;
                }
                if (int.Parse(numericUpDown5.Value.ToString().Trim()) > substrateNum1)
                {
                    frmMessageShow f = new frmMessageShow();
                    f.MessageShow("温馨提示", "底物不足，请重新装载底物！");
                    functionButton6.Enabled = true;
                    functionButton6.Text = "循环灌注";
                    bLoopRun = true;
                    return;
                }
                if (Loopthread==null||Loopthread.ThreadState != ThreadState.Running)
                {
                    LoopPourinto = int.Parse(numericUpDown5.Value.ToString());
                    TExtAppend("循环灌注开始。。。\n");
                    Loopthread = new Thread(new ThreadStart(TestLoopRun));
                    Loopthread.IsBackground = true;
                    Loopthread.Start();
                }
            }
            else
            {               
                //for(int i=0;i<5;i++)
                //{
                    if (Loopthread != null && Loopthread.ThreadState != ThreadState.Stopped)
                    {
                        Loopthread.Abort();
                        //NetCom3.Delay(4000);
                    }
                //}
                functionButton6.Text = "循环灌注";
                bLoopRun = false;
                functionButton6.Enabled = true;
            }         
        }
        bool bLoopRun = false;
        int LoopPourinto = 0;
        void TestLoopRun()
        {
            #region 清空清洗盘
            TExtAppend("清空清洗盘。。。\n");
            for (int i = 0; i < WashTrayNum; i++)
            {
                if (isNewWashEnd()) return;  //lyq add 20190822
                if (!bLoopRun) return;
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
                //打印当前取放管孔位到Log。  jun 2019/1/22
                LogFile.Instance.Write(string.Format("***{0}->:{1}***", "取放管时间: " + DateTime.Now.ToString("HH:mm:ss:fff"), "管孔位置: " + tubeHoleNum));
                if (!NetCom3.Instance.MoveQuery() && NetCom3.Instance.MoverrorFlag != (int)ErrorState.IsNull)
                {
                    return;
                }
                if (isNewWashEnd()) return;  //lyq add 20190822
                NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 03 01 " + (-1).ToString("X2").Substring(6, 2)), 2);
                //逆时针计数。  Jun 2019/1/22
                if (tubeHoleNum <= 0)
                {
                    tubeHoleNum = tubeHoleNum + WashTrayNum;
                }
                tubeHoleNum = tubeHoleNum - 1;
                if (!NetCom3.Instance.WashQuery())
                {
                    functionButton6.Text = "循环灌注";
                    bLoopRun = false;
                    functionButton6.Enabled = true;
                    return;
                }
            }
            #endregion
            TExtAppend("清洗盘清管完成。。。\n");
            if (!bLoopRun)
            {
                functionButton6.Text = "循环灌注";
                bLoopRun = false;
                functionButton6.Enabled = true;
                return;
            }
            if (!AddTubeInCleanTray())
            {
                functionButton6.Text = "循环灌注";
                bLoopRun = false;
                functionButton6.Enabled = true;
                return;
            }
            TExtAppend("夹第1个新管。。。\n");
            CleanTrayMovePace(5);
            if (!bLoopRun)
            {
                functionButton6.Text = "循环灌注";
                bLoopRun = false;
                functionButton6.Enabled = true;
                return;
            }
            if (!AddTubeInCleanTray())
            {
                functionButton6.Text = "循环灌注";
                bLoopRun = false;
                functionButton6.Enabled = true;
                return;
            }
            TExtAppend("夹第2个新管。。。\n");
            CleanTrayMovePace(4);
            if (!bLoopRun)
            {
                functionButton6.Text = "循环灌注";
                bLoopRun = false;
                functionButton6.Enabled = true;
                return;
            }
            if (!AddTubeInCleanTray())
            {
                functionButton6.Text = "循环灌注";
                bLoopRun = false;
                functionButton6.Enabled = true;
                return;
            }
            TExtAppend("夹第3个新管。。。\n");
            CleanTrayMovePace(4);
            if (!bLoopRun)
            {
                functionButton6.Text = "循环灌注";
                bLoopRun = false;
                functionButton6.Enabled = true;
                return;
            }
            if (!AddTubeInCleanTray())
            {
                functionButton6.Text = "循环灌注";
                bLoopRun = false;
                functionButton6.Enabled = true;
                return;
            }
            TExtAppend("夹第4个新管。。。\n");
            CleanTrayMovePace(5 + isNewCleanTray);
            if (!bLoopRun)
            {
                functionButton6.Text = "循环灌注";
                bLoopRun = false;
                functionButton6.Enabled = true;
                return;
            }
            for (int i = 0; i < LoopPourinto; i++)
            {
                if (isNewWashEnd()) return;  //lyq add 20190822
                //底物灌注改为1次
                for (int index = 0; index < 1; index++)
                {
                    if (!bLoopRun)
                    {
                        functionButton6.Text = "循环灌注";
                        bLoopRun = false;
                        functionButton6.Enabled = true;
                        return;
                    }
                    CleanTrayWash(1);
                    CleanTrayMovePace(-1);
                    CleanTrayWash(2);
                    CleanTrayMovePace(-1);
                    CleanTrayWash(2);
                    CleanTrayMovePace(2);
                }
            }
            TExtAppend("灌注完成开始扔管。。。\n");
            if (!bLoopRun)
            {
                functionButton6.Text = "循环灌注";
                bLoopRun = false;
                functionButton6.Enabled = true;
                return;
            }
            CleanTrayMovePace(-5 - isNewCleanTray);
            if (!RemoveTubeOutCleanTray())
            {
                functionButton6.Text = "循环灌注";
                bLoopRun = false;
                functionButton6.Enabled = true;
                return;
            }
            TExtAppend("扔第1个管完成。。。\n");
            if (!bLoopRun)
            {
                functionButton6.Text = "循环灌注";
                bLoopRun = false;
                functionButton6.Enabled = true;
                return;
            }
            CleanTrayMovePace(-4);
            if (!RemoveTubeOutCleanTray())
            {
                functionButton6.Text = "循环灌注";
                bLoopRun = false;
                functionButton6.Enabled = true;
                return;
            }
            TExtAppend("扔第2个管完成。。。\n");
            CleanTrayMovePace(-4);
            if (!bLoopRun)
            {
                functionButton6.Text = "循环灌注";
                bLoopRun = false;
                functionButton6.Enabled = true;
                return;
            }
            if (!RemoveTubeOutCleanTray())
            {
                functionButton6.Text = "循环灌注";
                bLoopRun = false;
                functionButton6.Enabled = true;
                return;
            }
            TExtAppend("扔第3个管完成。。。\n");
            CleanTrayMovePace(-5);
            if (!bLoopRun)
            {
                functionButton6.Text = "循环灌注";
                bLoopRun = false;
                functionButton6.Enabled = true;
                return;
            }
            if (!RemoveTubeOutCleanTray())
            {
                functionButton6.Text = "循环灌注";
                bLoopRun = false;
                functionButton6.Enabled = true;
                return;
            }
            TExtAppend("扔第4个管完成。。。\n");
            //frmMessageShow fm = new frmMessageShow();
            //fm.MessageShow("温馨提示", "循环灌注完成！");
            BeginInvoke(new Action(() => { numericUpDown5.Value = 0;}));
            BeginInvoke(new Action(() => { functionButton6.Text = "循环灌注"; }));
            bLoopRun = false;
            functionButton6.Enabled = true;
            TExtAppend("循环灌注完成。。。\n");
        }
        // Jun Add 20190317 加空管到清洗盘取放管处
        private bool AddTubeInCleanTray(int pos = 0)
        {
            int iNeedCool = 0;
            int IsKnockedCool = 0;
        AgainNewMove:
            if (isNewWashEnd()) return false;  //lyq add 20190822
            string order = "EB 90 31 01 06";
            NetCom3.Instance.Send(NetCom3.Cover(order), 1);
            if (!NetCom3.Instance.MoveQuery())
            {
                #region 发生异常处理
                if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull)
                {
                    iNeedCool++;
                    if (iNeedCool < 2)
                        goto AgainNewMove;
                    else
                    {
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在管架向温育盘抓管时多次抓空!实验停止进行加样！");
                        DialogResult tempresult = MessageBox.Show("移管手抓新管抓空！实验将停止运行！", "移管手错误！", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        return false;
                    }
                }
                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.Sendfailure)
                {
                    if (NetCom3.Instance.waitAndAgainSend != null && NetCom3.Instance.waitAndAgainSend is Thread)
                    {
                        NetCom3.Instance.waitAndAgainSend.Abort();
                    }
                    goto AgainNewMove;
                }
                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked)
                {
                    IsKnockedCool++;
                    if (IsKnockedCool < 2)
                        goto AgainNewMove;
                    else
                    {
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在管架向温育盘抓管时发生撞管！");
                        LogFile.Instance.Write("==============  移管手在管架向清洗盘取放管处抓管发生撞管  " + currentHoleNum);
                        DialogResult tempresult = MessageBox.Show("移管手在管架向清洗盘取放管处抓管发生撞管，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        return false;
                    }

                }
                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.putKnocked)
                {
                    IsKnockedCool++;

                //GAgainMove:
                //    NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 01"), 1);
                //    if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked)
                //    {
                //        IsKnockedCool++;
                //        if (IsKnockedCool < 2)
                //            goto GAgainMove;
                //        else
                //        {
                //            LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手清洗盘扔管时发生撞管！");
                //            LogFile.Instance.Write("==============  移管手在管架向清洗盘扔管时发生撞管  " + currentHoleNum);
                //            DialogResult tempresult = MessageBox.Show("移管手在清洗盘扔管时发生撞管，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                //            return false;
                //        }
                //    }
                    if (IsKnockedCool < 2)
                        goto AgainNewMove;
                    else
                    {
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手清洗盘扔管时发生撞管！");
                        LogFile.Instance.Write("==============  移管手在管架向清洗盘扔管时发生撞管  " + currentHoleNum);
                        DialogResult tempresult = MessageBox.Show("移管手在清洗盘扔管时发生撞管，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        return false;
                    }
                }
                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.OverTime)
                {
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在管架向温育盘抓管时接收数据超时！");
                    DialogResult tempresult = MessageBox.Show("移管手在管架向清洗盘取放管处抓管接收数据超时，实验将进行停止！", "移管手错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    return false;
                }
                #endregion
            }
            return true;
        }
        // Jun Add 20190317  旋转清洗盘相应空位，正数为逆时针旋转
        void CleanTrayMovePace(int pace)
        {
            DataTable dtWashTrayIni = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
            string order;
            if (pace > 0)
            {
                order = "EB 90 31 03 01 " + (pace).ToString("X2");
            }
            else if (pace < 0)
            {
                order = "EB 90 31 03 01 " + (pace).ToString("X2").Substring(6, 2);
            }
            else return;
        AgainNewMove:
            if (isNewWashEnd()) return;  //lyq add 20190822
            NetCom3.Instance.Send(NetCom3.Cover(order), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                if (NetCom3.Instance.WasherrorFlag == (int)ErrorState.Sendfailure)
                    goto AgainNewMove;
                else if (NetCom3.Instance.WasherrorFlag == (int)ErrorState.OverTime)
                {
                    NetCom3.Instance.stopsendFlag = true;
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "清洗指令接收指令超时！");
                    frmMsgShow.MessageShow("清洗指令错误提示", "指令接收超时，实验已终止");
                }
                else
                    return;

            }
            countWashHole(pace);
            currentHoleNum = currentHoleNum - 1;
            if (currentHoleNum > WashTrayNum)
            {
                currentHoleNum = currentHoleNum - WashTrayNum;
            }
            if (currentHoleNum <= 0)
            {
                currentHoleNum = currentHoleNum + WashTrayNum;
            }
            OperateIniFile.WriteIniPara("OtherPara", "washCurrentHoleNum", currentHoleNum.ToString());
            dtWashTrayIni = OperateIniFile.ReadConfig(iniPathWashTrayInfo);
            DataTable dtTemp = new DataTable();
            dtTemp = dtWashTrayIni.Copy();
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
        //Jun Add 20190318 清洗盘清洗1:全部注液，2：全部吸液
        void CleanTrayWash(int oneOrTwo)
        {
            string order;
            if (oneOrTwo == 1)
            {
                substrateNum1 = int.Parse(OperateIniFile.ReadIniData("Substrate1", "LeftCount", "", iniPathSubstrateTube));
                order = "EB 90 31 03 03 00 11 11 10";
            }
            else if (oneOrTwo == 2)
            {
                order = "EB 90 31 03 03 01 00 00 10";
            }
            else
            {
                throw new Exception();
                return;
            }
        AgainNewMove:
            if (isNewWashEnd()) return;  //lyq add 20190822
            NetCom3.Instance.Send(NetCom3.Cover(order), 2);
            if (!NetCom3.Instance.WashQuery())
            {
                if (NetCom3.Instance.WasherrorFlag == (int)ErrorState.Sendfailure)
                    goto AgainNewMove;
                else if (NetCom3.Instance.WasherrorFlag == (int)ErrorState.OverTime)
                {
                    NetCom3.Instance.stopsendFlag = true;
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "清洗指令接收超时！");
                    frmMsgShow.MessageShow("清洗指令错误提示", "指令接收超时，实验已终止");
                }
                else
                    return;
            }
            else
            {
                if (oneOrTwo == 1)
                {
                    OperateIniFile.WriteIniData("Substrate1", "LeftCount", (substrateNum1 - 1).ToString(), iniPathSubstrateTube);
                    substrateNum1--;
                    string sbCode1 = OperateIniFile.ReadIniData("Substrate1", "BarCode", "0", iniPathSubstrateTube);
                    string sbNum1 = OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", iniPathSubstrateTube);
                    DbHelperOleDb dbase = new DbHelperOleDb(3);
                    DbHelperOleDb.ExecuteSql(3, @"update tbSubstrate set leftoverTest =" + sbNum1 + " where BarCode = '" + sbCode1 + "'");
                    substrateNum1 = int.Parse(OperateIniFile.ReadIniData("Substrate1", "LeftCount", "0", iniPathSubstrateTube));
                }
            }
        }

        //Jun Add 20190318  从清洗盘取放管处扔管
        private bool RemoveTubeOutCleanTray()
        {
            int iNeedCool = 0;
            int IsKnockedCool = 0;
        AgainNewMove:
            if (isNewWashEnd()) return false;  //lyq add 20190822
            NetCom3.Instance.Send(NetCom3.Cover("EB 90 31 01 04 06"), 1);
            if (!NetCom3.Instance.MoveQuery())
            {
                #region 发生异常处理
                if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsNull)
                {
                    iNeedCool++;
                    if (iNeedCool < 2)
                        goto AgainNewMove;
                    else
                    {
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘扔废管时多次抓空！");
                        LogFileAlarm.Instance.Write(" *** " + "时间" + DateTime.Now.ToString("HH-mm-ss") + "请洗盘抓空孔位置" + tubeHoleNum + " *** ");
                        DialogResult tempresult = frmMsgShow.MessageShow("移管手错误", "移管手在清洗盘扔废管时多次抓空，实验将进行停止！");
                        return false;
                    }
                }
                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.Sendfailure)
                {
                    if (NetCom3.Instance.waitAndAgainSend != null && NetCom3.Instance.waitAndAgainSend is Thread)
                    {
                        NetCom3.Instance.waitAndAgainSend.Abort();
                    }
                    goto AgainNewMove;
                }
                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.IsKnocked)
                {
                    IsKnockedCool++;
                    if (IsKnockedCool < 2)
                    {
                        goto AgainNewMove;
                    }
                    else
                    {
                        LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘扔废管时发生撞管！");
                        LogFileAlarm.Instance.Write(" *** " + "时间" + DateTime.Now.ToString("HH-mm-ss") + "清洗盘扔废管时发生撞管孔位置" + tubeHoleNum + " *** ");
                        DialogResult tempresult = frmMsgShow.MessageShow("移管手错误", "移管手在清洗盘扔废管时发生撞管，实验将进行停止！");
                        return false;
                    }
                }
                else if (NetCom3.Instance.MoverrorFlag == (int)ErrorState.OverTime)
                {
                    LogFileAlarm.Instance.Write(DateTime.Now.ToString("HH-mm-ss") + " *** " + "错误" + " *** " + "未读" + " *** " + "移管手在清洗盘扔废管时接收数据超时！");
                    DialogResult tempresult = frmMsgShow.MessageShow("移管手错误", "移管手在清洗盘扔废管时接收数据超时，实验将进行停止！");
                    return false;
                }
                #endregion
            }
            return true;
        }
    }
}
