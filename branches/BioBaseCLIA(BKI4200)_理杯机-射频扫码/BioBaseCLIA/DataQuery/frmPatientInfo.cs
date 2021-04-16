using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Maticsoft.DBUtility;
using BioBaseCLIA.InfoSetting;
using Common;
using System.Resources;

namespace BioBaseCLIA.DataQuery
{
    public partial class frmPatientInfo : frmSmallParent
    {
        /// <summary>
        /// 功能简介：全自动化学发光病人信息界面，可录入样本信息。
        /// 完成日期：20170725
        /// 编写人：刘亚男
        /// 版本：1.0
        /// </summary>

        #region 变量及属性
        ///<summary>
        ///样本信息ID，从结果查询界面或样本录入界面传入
        /// </summary>
        public int SampleID { get; set; }
        /// <summary>
        /// 系统使用者名称，从结果查询界面或样本录入界面传入
        /// </summary>
        public string LoginGName { get; set; }
        Model.tbSampleInfo modelSp = new Model.tbSampleInfo();
        BLL.tbSampleInfo bllsp = new BLL.tbSampleInfo();
        public frmMessageShow frmMsg = new frmMessageShow();
        //2018-11-19 zlx add
        BLL.tbDepartment bllDep = new BLL.tbDepartment();
        BLL.tbDoctor bllDoctor = new BLL.tbDoctor();
        DataTable dtDepInfo;//科室列表信息
        #endregion

        public frmPatientInfo()
        {
            InitializeComponent();
        }

        private void frmPatientInfo_Load(object sender, EventArgs e)
        {
            DbHelperOleDb db = new DbHelperOleDb(2);
            dtDepInfo = bllDep.GetAllList().Tables[0];
            cmbDepartment.Items.Clear();
            //2018-11-19 zlx add
            foreach (DataRow dr in dtDepInfo.Rows)
            {
                cmbDepartment.Items.Add(dr["DepartmentName"]);
            }
            db = new DbHelperOleDb(1);
            modelSp = bllsp.GetModel(SampleID);
            txtAge.Text = modelSp.Age.ToString();
            txtBedNo.Text = modelSp.BedNo;
            txtClinicNo.Text = modelSp.ClinicNo;
            //txtInpatientArea.Text = modelSp.InpatientArea;
            txtMedicaRecordNo.Text = modelSp.MedicaRecordNo;
            txtPatientName.Text = modelSp.PatientName;
            //txtWard.Text = modelSp.Ward;
            cmbSex.Text = modelSp.Sex;
            txtDiagnosis.Text = modelSp.Diagnosis;
            //2018-11-19 zlx mod
            cmbDepartment.Text = modelSp.Department;

            cmbSendDoctor.Text = modelSp.SendDoctor;
            dateSendDateTime.Value = (DateTime)modelSp.SendDateTime;
            if (modelSp.InspectDoctor != "")
                txtInspectDoctor.Text = modelSp.InspectDoctor;
            else
                txtInspectDoctor.Text = LoginGName;
            txtCheckDoctor.Text = modelSp.CheckDoctor;
            bool IsLisConnect = bool.Parse(OperateIniFile.ReadInIPara("LisSet", "IsLisConnect"));
            if (!IsLisConnect)
            {
                btnLis.Visible = false;
                return;
            }
        }

        private void fbtnOK_Click(object sender, EventArgs e)
        {
            DbHelperOleDb db = new DbHelperOleDb(1);//2018-11-19 zlx add
            if (txtAge.Text == "")
            {
                modelSp.Age = 0;
            }
            else
            modelSp.Age = int.Parse(txtAge.Text);
            modelSp.BedNo = txtBedNo.Text;
            modelSp.ClinicNo = txtClinicNo.Text;
            modelSp.Diagnosis = txtDiagnosis.Text;
            modelSp.InpatientArea = txtInpatientArea.Text;
            modelSp.MedicaRecordNo = txtMedicaRecordNo.Text;
            modelSp.PatientName = txtPatientName.Text;
            modelSp.Sex = cmbSex.Text;
            modelSp.Ward = txtWard.Text;
            //2018-11-10 zlx mod
            modelSp.Department = cmbDepartment.Text;
            modelSp.SendDoctor = cmbSendDoctor.Text;
            modelSp.SendDateTime = dateSendDateTime.Value;
            modelSp.InspectDoctor = txtInspectDoctor.Text;
            modelSp.CheckDoctor = txtCheckDoctor.Text;
            if (bllsp.Exists(SampleID))
            {
                bllsp.UpdatePatientInfo(modelSp);
            }
            //frmMsg.MessageShow("病人信息", "病人信息录入成功");
            DialogResult = DialogResult.OK;
            Close();
        }

        private void fbtnCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLis_Click(object sender, EventArgs e)
        {
            //2018-4-27 zlx add
            if (modelSp.SampleNo == "") return;


            string CommunicationType = OperateIniFile.ReadInIPara("LisSet", "CommunicationType");
            string ConnectType = OperateIniFile.ReadInIPara("LisSet", "ConnectType");

            if (ConnectType != Getstring("TwoWay"))
            {
                frmMessageShow frmMessage = new frmMessageShow();
                frmMessage.MessageShow( Getstring("MessageHead"), Getstring("ConnMessage"));
                return;
            }
            else//如果与LIS连接，发送查询
            {
                if (CommunicationType == Getstring("NetConn"))
                {
                    if (!LisCommunication.Instance.IsConnect())
                    {
                        MessageBox.Show(Getstring("NoConnMessage"), Getstring("MessageHead"));
                        return;
                    }
                    CMessageParser Cmp = new CMessageParser();
                    Cmp.SelectBySampleNo(modelSp.SampleNo);
                    //LisCommunication.Instance.comWait.WaitOne();
                    bool delay = LisCommunication.Instance.comWait.WaitOne(10000);
                    if (!delay)
                    {
                        LisCommunication.Instance.comWait.Set();
                    }
                    modelSp = Cmp.GetSampleInfo(modelSp);
                }
                else if (CommunicationType == Getstring("SerialConn"))
                {
                    if (!LisConnection.Instance.IsOpen())
                    {
                        MessageBox.Show(Getstring("NoConnMessage"), Getstring("MessageHead"));
                        return;
                    }
                    CAMessageParser CAmp = new CAMessageParser();
                    CAmp.SelectBySampleNo(modelSp.SampleNo);
                    //delay = LisConnection.Instance.SelectWait.WaitOne(10000);
                    //if (!delay)
                    //{
                    //    LisConnection.Instance.SelectWait.Set();
                    //}
                    modelSp = CAmp.GetSampleInfo(modelSp);
                }
                //switch (CommunicationType)
                //{
                //    case "网口通讯":
                //        if (!LisCommunication.Instance.IsConnect())
                //        {
                //            MessageBox.Show(Getstring("NoConnMessage"), Getstring("MessageHead"));
                //            return;
                //        }
                //        CMessageParser Cmp = new CMessageParser();
                //        Cmp.SelectBySampleNo(modelSp.SampleNo);
                //        //LisCommunication.Instance.comWait.WaitOne();
                //        bool delay = LisCommunication.Instance.comWait.WaitOne(10000);
                //        if (!delay)
                //        {
                //            LisCommunication.Instance.comWait.Set();
                //        }
                //        modelSp = Cmp.GetSampleInfo(modelSp);
                //        break;
                //    case "串口通讯":
                //        if (!LisConnection.Instance.IsOpen())
                //        {
                //            MessageBox.Show(Getstring("NoConnMessage"), Getstring("MessageHead"));
                //            return;
                //        }
                //        CAMessageParser CAmp = new CAMessageParser();
                //        CAmp.SelectBySampleNo(modelSp.SampleNo);
                //        //delay = LisConnection.Instance.SelectWait.WaitOne(10000);
                //        //if (!delay)
                //        //{
                //        //    LisConnection.Instance.SelectWait.Set();
                //        //}
                //        modelSp = CAmp.GetSampleInfo(modelSp);
                //        break;
                //    default:
                //        break;
                //}
                txtAge.Text = modelSp.Age.ToString();
                txtBedNo.Text = modelSp.BedNo;
                txtClinicNo.Text = modelSp.ClinicNo;
                //txtInpatientArea.Text = modelSp.InpatientArea;
                txtMedicaRecordNo.Text = modelSp.MedicaRecordNo;
                txtPatientName.Text = modelSp.PatientName;
                //txtWard.Text = modelSp.Ward;
                cmbSex.Text = modelSp.Sex;
                txtDiagnosis.Text = modelSp.Diagnosis;
            }
        }
        private void cmbDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDepartment.SelectedItem.ToString() != "")
            {
                cmbSendDoctor.Items.Clear();
                DataRow[]dr=dtDepInfo.Select("DepartmentName='"+cmbDepartment.SelectedItem+"'");
                if (dr.Length > 0)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 
                {
                    DbHelperOleDb db = new DbHelperOleDb(2);
                    DataTable dtDoctor = bllDoctor.GetList("DepartmentID=" + dr[0]["DepartmentID"] + "").Tables[0];
                    foreach (DataRow docdr in dtDoctor.Rows)
                    {
                        cmbSendDoctor.Items.Add(docdr["DoctorName"]);
                    }
                }
                
            }
        }
        /// <summary>
        /// 重写ProcessDialogKey将回车响应为tab键
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter && !(this.ActiveControl is Button))
            {
                keyData = Keys.Tab;
            }

            return base.ProcessDialogKey(keyData);
        }
        private string Getstring(string key)
        {
            ResourceManager resManagerA =
                    new ResourceManager("BioBaseCLIA.DataQuery.frmPatientInfo", typeof(frmPatientInfo).Assembly);
            return resManagerA.GetString(key);
        }
    }
}
