using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using Maticsoft.DBUtility;

namespace BioBaseCLIA.InfoSetting
{
    public partial class frmUserManage : frmParent
    {
        BLL.tbUser bllUser = new BLL.tbUser();
        DataTable dtData = new DataTable();
        DataTable dtUser = new DataTable();
        Model.tbUser modelUser = new Model.tbUser();
        frmMessageShow frmMsgShow = new frmMessageShow();
        public frmUserManage()
        {
            InitializeComponent();
        }

        private void frmUserManage_SizeChanged(object sender, EventArgs e)
        {
            formSizeChange(this);
        }

        private void fbtnReturn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //2018-08-04 zlx mod
        private void DataTableChange(int RoleType)
        {
            dtData.Rows.Clear();
            for (int i = 0; i < dtUser.Rows.Count; i++)
            {
                if (Convert.ToInt32(dtUser.Rows[i]["RoleType"]) > 1)
                {
                    if (Convert.ToInt32(LoginUserType) == 9)
                        dtData.Rows.Add((i + 1).ToString(), dtUser.Rows[i]["UserName"].ToString(), Getstring("AppDevelop"), dtUser.Rows[i]["UserPassword"].ToString());
                }
                else
                {
                    //dtData.Rows.Add((i + 1).ToString(), dtUser.Rows[i]["UserName"].ToString(), dtUser.Rows[i]["RoleType"].ToString() == "1" ? "管理员" : "普通用户", dtUser.Rows[i]["UserPassword"].ToString());
                    dtData.Rows.Add((dtData.Rows.Count + 1).ToString(), dtUser.Rows[i]["UserName"].ToString(), dtUser.Rows[i]["RoleType"].ToString() == "1" ? Getstring("Administrator") : Getstring("Personal"), dtUser.Rows[i]["UserPassword"].ToString());
                }
            }
        }

        DataTable temp = new DataTable();
        private void frmUserManage_Load(object sender, EventArgs e)
        {
            //2018-08-04  zlx add
            if (Convert.ToInt32(LoginUserType) == 0)
            {
                btnAdd.Enabled = false;
                btnDel.Enabled = false;
            }
            DbHelperOleDb db = new DbHelperOleDb(2);
            dtUser = bllUser.GetAllList().Tables[0];
            dtData.Columns.Add("No", typeof(string));
            dtData.Columns.Add("UserName", typeof(string));
            dtData.Columns.Add("UserRoleType", typeof(string));
            dtData.Columns.Add("UserPassword", typeof(string));
            //2018-08-04 zlx mod
            DataTableChange(Convert.ToInt32(LoginUserType));
            dgvUserInfo.SelectionChanged -= dgvUserInfo_SelectionChanged;
            dgvUserInfo.DataSource = dtData;
            dgvUserInfo.SelectionChanged += dgvUserInfo_SelectionChanged;
            txtName.Enabled = txtPassword.Enabled = txtConfirmPassword.Enabled = cmbType.Enabled = btnSave.Enabled= false;

            temp.Columns.Add("No", typeof(string));
            temp.Columns.Add("UserName", typeof(string));
            temp.Columns.Add("UserRoleType", typeof(string));
            temp.Columns.Add("UserPassword", typeof(string));
            temp.Rows.Clear();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (btnAdd.Text.Trim() == Getstring("AddUser"))
            {
                txtName.Enabled = txtPassword.Enabled = txtConfirmPassword.Enabled = cmbType.Enabled = btnSave.Enabled = true;
                txtName.Text = txtPassword.Text = txtConfirmPassword.Text = "";
                btnDel.Enabled = btnModifyPassword.Enabled = false;
                cmbType.SelectedIndex = 0;
                btnAdd.Text = Getstring("cancel");
            }
            else
            {
                btnAdd.Text = Getstring("AddUser");
                dgvUserInfo_SelectionChanged(sender, e);
                txtName.Enabled = txtPassword.Enabled = txtConfirmPassword.Enabled = cmbType.Enabled = btnSave.Enabled = false;
                if(dgvUserInfo.Rows.Count>0)
                    btnDel.Enabled = btnModifyPassword.Enabled = true;
                if (dgvUserInfo.SelectedRows.Count > 0)//this block add y 20180510
                {
                    txtName.Text = dgvUserInfo.SelectedRows[0].Cells["UserName"].Value.ToString();
                    cmbType.Text = dgvUserInfo.SelectedRows[0].Cells["UserRoleType"].Value.ToString();
                    //2018-08-04 zlx mod
                    //txtPassword.Text = txtConfirmPassword.Text = dgvUserInfo.SelectedRows[0].Cells["Password"].Value.ToString();
                }
                else
                {
                    txtName.Text = "";
                    txtPassword.Text = txtConfirmPassword.Text = "";
                }//this block end
            }
        }

        private void btnModifyPassword_Click(object sender, EventArgs e)
        {
            if (dgvUserInfo.SelectedRows.Count == 0)
                return;
            if (btnModifyPassword.Text.Trim() == Getstring("ChangePass"))
            {
                txtPassword.Enabled = txtConfirmPassword.Enabled = btnSave.Enabled = true;
                btnModifyPassword.Text = Getstring("cancel");
                btnDel.Enabled = btnAdd.Enabled = false;
            }
            else
            {
                btnModifyPassword.Text = Getstring("ChangePass");
                //2018-08-04 zlx mod
                if (Convert.ToInt32(LoginUserType) == 0)
                    btnModifyPassword.Enabled = true;
                else
                {
                    txtPassword.Enabled = txtConfirmPassword.Enabled = btnSave.Enabled = false;
                    if (dgvUserInfo.SelectedRows.Count != 0)//add y 20180510
                        btnDel.Enabled = true;
                    btnAdd.Enabled = true;//modify y 20180510
                }
                if (dgvUserInfo.SelectedRows.Count > 0)//this block add y 20180510
                {
                    txtName.Text = dgvUserInfo.SelectedRows[0].Cells["UserName"].Value.ToString();
                    cmbType.Text = dgvUserInfo.SelectedRows[0].Cells["UserRoleType"].Value.ToString();
                    //2018-08-04 zlx mod
                    //txtPassword.Text = txtConfirmPassword.Text = dgvUserInfo.SelectedRows[0].Cells["Password"].Value.ToString();
                }
                else
                {
                    txtName.Text = "";
                    txtPassword.Text = txtConfirmPassword.Text = "";
                }//this block end
            }
        }

        private void dgvUserInfo_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUserInfo.SelectedRows.Count > 0)
            {
                int index = int.Parse(dgvUserInfo.SelectedRows[0].Cells[0].Value.ToString()) - 1;
                txtName.Text = dtData.Rows[index]["UserName"].ToString();
                cmbType.Text = dtData.Rows[index]["UserRoleType"].ToString();
                //2018-08-04 zlx mod
                //txtPassword.Text = txtConfirmPassword.Text = dtData.Rows[index]["UserPassword"].ToString();
                //2018-08-04 zlx add
                if (txtName.Text != LoginUserName)
                    btnModifyPassword.Enabled = false;
                else
                    btnModifyPassword.Enabled = true;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                frmMsgShow.MessageShow(Getstring("UserInfoSet"), Getstring("DifPassWord"));
                txtConfirmPassword.Text = "";
                txtConfirmPassword.Focus();
                return;
            }
            if (txtName.Text.Trim() == "")
            {
                frmMsgShow.MessageShow(Getstring("UserInfoSet"), Getstring("NoUserName"));
                txtName.Focus();
                return;
            }

            if (!Inspect.NameOnlycharacter3(txtName.Text.Trim()))//y add 20180419
            {
                frmMsgShow.MessageShow(Getstring("UserInfoSet"), Getstring("UserNameSet"));//y modify 20180427
                txtName.Focus();
                return;
            }
            if (txtPassword.Text.Trim() =="")//20181129 zlx mod
            {
                frmMsgShow.MessageShow(Getstring("UserInfoSet"), Getstring("NullPassWord"));
                txtPassword.Focus();
                return;
            }//this end
            if (!Inspect.PasswordOnlycharacter(txtPassword.Text.Trim()))//this y add 20180528
            {
                frmMsgShow.MessageShow(Getstring("UserInfoSet"), Getstring("PassWordSet"));//y modify 20180427
                txtPassword.Focus();
                return;
            }//this end

            var dr = dtUser.Select("UserName='"+txtName.Text.Trim()+"'");
            if (dr.Length < 1)
            {
                modelUser.UserName=txtName.Text.Trim();
                modelUser.UserPassword=txtPassword.Text.Trim();
                modelUser.RoleType=cmbType.SelectedIndex;
                modelUser.defaultValue=0;

                if (bllUser.Add(modelUser))
                {
                    dtData.Rows.Add(dtData.Rows.Count+1,modelUser.UserName, modelUser.RoleType==0? Getstring("Personal"):Getstring("Administrator"), modelUser.UserPassword);
                    txtName.Enabled = txtPassword.Enabled = txtConfirmPassword.Enabled = cmbType.Enabled = btnSave.Enabled = false;
                    btnAdd.Text = Getstring("AddUser");
                    if (dgvUserInfo.Rows.Count != 0)//add y 20180510
                        btnDel.Enabled = btnModifyPassword.Enabled = true;
                    frmMsgShow.MessageShow(Getstring("UserInfoSet"), Getstring("AddSucess"));
                    dtUser = bllUser.GetAllList().Tables[0];              
                }
            }
            else
            {
                if (btnAdd.Text == Getstring("cancel") && btnModifyPassword.Text ==Getstring("ChangePass"))//add y 20180510
                {
                    frmMsgShow.MessageShow(Getstring("UserInfoSet"), Getstring("SameUser"));//add y 20180510
                }
                modelUser.UserID =int.Parse(dr[0]["UserID"].ToString());
                modelUser.UserName = dr[0]["UserName"].ToString();
                modelUser.UserPassword = txtPassword.Text.Trim();
                modelUser.RoleType = int.Parse(dr[0]["RoleType"].ToString());
                modelUser.defaultValue = 0;

                if (bllUser.Update(modelUser))
                {
                    btnModifyPassword.Text = Getstring("ChangePass");
                    txtName.Enabled = txtPassword.Enabled = txtConfirmPassword.Enabled = cmbType.Enabled = btnSave.Enabled = false;
                    btnAdd.Text = Getstring("AddUser");
                    //2018-08-04 zlx mod
                    if (Convert.ToInt32(LoginUserType) == 0)
                        btnModifyPassword.Enabled = true;
                    else
                        btnDel.Enabled = btnAdd.Enabled = btnModifyPassword.Enabled = true;

                    dtUser = bllUser.GetAllList().Tables[0];
                    DataTableChange(Convert.ToInt32(LoginUserType));//2018-08-04 zlx mod

                    frmMsgShow.MessageShow(Getstring("UserInfoSet"), Getstring("ChangePassSucess"));
                }
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            var dr = dtUser.Select("UserName='" + txtName.Text + "'");
            if (dr.Length > 0)
            {
                if (bllUser.Delete(int.Parse(dr[0]["UserID"].ToString())))
                {
                    dtUser.Rows.Remove(dr[0]);
                    DataTableChange(Convert.ToInt32(LoginUserType));//2018-08-04 zlx mod
                    frmMsgShow.MessageShow(Getstring("UserInfoSet"), Getstring("DeleteSucess"));
                }
                if (dgvUserInfo.Rows.Count == 0)
                {
                    txtName.Text = "";
                    txtPassword.Text = txtConfirmPassword.Text = "";
                }
                else
                {
                    foreach (DataGridViewRow a in dgvUserInfo.SelectedRows)
                    {
                        a.Selected = false;
                    }
                }
            }
            else
            {
                frmMsgShow.MessageShow(Getstring("UserInfoSet"), Getstring("DeleteNoSelect"));
            }
            dtUser = bllUser.GetAllList().Tables[0];
            if (dgvUserInfo.Rows.Count == 0)
            {
                btnDel.Enabled = btnModifyPassword.Enabled = false;
            }
        }

        private void btnInstrumentPara_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmInstrumentPara"))
            {
                frmInstrumentPara frmIP = new frmInstrumentPara();
                //this.TopLevel = false;
                frmIP.TopLevel = false;
                frmIP.Parent = this.Parent;
                frmIP.Show();
            }
            else
            {
                frmInstrumentPara frmIP = (frmInstrumentPara)Application.OpenForms["frmInstrumentPara"];
                //frmIM.Activate();
                frmIP.BringToFront();

            }
        }

        private void btnProInfo_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmInfo"))
            {
                frmInfo frmif = new frmInfo();
                //this.TopLevel = false;
                frmif.TopLevel = false;
                frmif.Parent = this.Parent;
                frmif.Show();
            }
            else
            {
                frmInfo frmif = (frmInfo)Application.OpenForms["frmInfo"];
                //frmIM.Activate();
                frmif.BringToFront();

            }
        }

        private void fbtnConnetSet_Click(object sender, EventArgs e)
        {
            if (!CheckFormIsOpen("frmNetSet"))
            {
                frmNetSet frmNS = new frmNetSet();
                //this.TopLevel = false;
                frmNS.TopLevel = false;
                frmNS.Parent = this.Parent;
                frmNS.Show();
            }
            else
            {
                frmNetSet frmNS = (frmNetSet)Application.OpenForms["frmNetSet"];
                //frmIM.Activate();
                frmNS.BringToFront();

            }
        }
        private string Getstring(string key)
        {
            ResourceManager resManagerA =
                    new ResourceManager("BioBaseCLIA.InfoSetting.frmUserManage", typeof(frmUserManage).Assembly);
            return resManagerA.GetString(key);
        }

    }
}
