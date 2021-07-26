using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
                        dtData.Rows.Add((i + 1).ToString(), dtUser.Rows[i]["UserName"].ToString(), "程序开发", dtUser.Rows[i]["UserPassword"].ToString());
                }
                else
                {
                    //dtData.Rows.Add((i + 1).ToString(), dtUser.Rows[i]["UserName"].ToString(), dtUser.Rows[i]["RoleType"].ToString() == "1" ? "管理员" : "普通用户", dtUser.Rows[i]["UserPassword"].ToString());
                    dtData.Rows.Add((dtData.Rows.Count + 1).ToString(), dtUser.Rows[i]["UserName"].ToString(), dtUser.Rows[i]["RoleType"].ToString() == "1" ? "管理员" : "普通用户", dtUser.Rows[i]["UserPassword"].ToString());
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
            if (btnAdd.Text.Trim() == "添加用户")
            {
                txtName.Enabled = txtPassword.Enabled = txtConfirmPassword.Enabled = cmbType.Enabled = btnSave.Enabled = true;
                txtName.Text = txtPassword.Text = txtConfirmPassword.Text = "";
                btnDel.Enabled = btnModifyPassword.Enabled = false;
                cmbType.SelectedIndex = 0;
                btnAdd.Text = "取消";
            }
            else
            {
                btnAdd.Text = "添加用户";
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
            if (btnModifyPassword.Text.Trim() == "修改密码")
            {
                txtPassword.Enabled = txtConfirmPassword.Enabled = btnSave.Enabled = true;
                btnModifyPassword.Text = "取消";
                btnDel.Enabled = btnAdd.Enabled = false;
            }
            else
            {
                btnModifyPassword.Text = "修改密码";
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
                frmMsgShow.MessageShow("用户信息设置", "确认密码与用户密码不一致，请重新输入！");
                txtConfirmPassword.Text = "";
                txtConfirmPassword.Focus();
                return;
            }
            if (txtName.Text.Trim() == "")
            {
                frmMsgShow.MessageShow("用户信息设置", "未输入用户名，请输入！");
                txtName.Focus();
                return;
            }

            if (!Inspect.NameOnlycharacter3(txtName.Text.Trim()))//y add 20180419
            {
                frmMsgShow.MessageShow("用户信息设置", "用户名至少由一位汉字、字母、星号、数字、\".\"、下划线组成。");//y modify 20180427
                txtName.Focus();
                return;
            }
            if (txtPassword.Text.Trim() =="")//20181129 zlx mod
            {
                frmMsgShow.MessageShow("用户设置", "用户密码不能为空。");
                txtPassword.Focus();
                return;
            }//this end
            if (!Inspect.PasswordOnlycharacter(txtPassword.Text.Trim()))//this y add 20180528
            {
                frmMsgShow.MessageShow("用户信息设置", "密码不能存在汉字、字母、星号、数字、\".\"、下划线以外的符号。");//y modify 20180427
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
                    dtData.Rows.Add(dtData.Rows.Count+1,modelUser.UserName, modelUser.RoleType==0?"普通用户":"管理员", modelUser.UserPassword);
                    txtName.Enabled = txtPassword.Enabled = txtConfirmPassword.Enabled = cmbType.Enabled = btnSave.Enabled = false;
                    btnAdd.Text = "添加用户";
                    if (dgvUserInfo.Rows.Count != 0)//add y 20180510
                        btnDel.Enabled = btnModifyPassword.Enabled = true;
                    frmMsgShow.MessageShow("用户信息设置", "添加成功！");
                    dtUser = bllUser.GetAllList().Tables[0];              
                }
            }
            else
            {
                if (btnAdd.Text == "取消" && btnModifyPassword.Text == "修改密码")//add y 20180510
                {
                    frmMsgShow.MessageShow("用户信息设置", "已有同名帐号，自动更新其密码。");//add y 20180510
                }
                modelUser.UserID =int.Parse(dr[0]["UserID"].ToString());
                modelUser.UserName = dr[0]["UserName"].ToString();
                modelUser.UserPassword = txtPassword.Text.Trim();
                modelUser.RoleType = int.Parse(dr[0]["RoleType"].ToString());
                modelUser.defaultValue = 0;

                if (bllUser.Update(modelUser))
                {
                    btnModifyPassword.Text = "修改密码";
                    txtName.Enabled = txtPassword.Enabled = txtConfirmPassword.Enabled = cmbType.Enabled = btnSave.Enabled = false;
                    btnAdd.Text = "添加用户";
                    //2018-08-04 zlx mod
                    if (Convert.ToInt32(LoginUserType) == 0)
                        btnModifyPassword.Enabled = true;
                    else
                        btnDel.Enabled = btnAdd.Enabled = btnModifyPassword.Enabled = true;

                    dtUser = bllUser.GetAllList().Tables[0];
                    DataTableChange(Convert.ToInt32(LoginUserType));//2018-08-04 zlx mod

                    frmMsgShow.MessageShow("用户信息设置", "密码修改成功！");
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
                    frmMsgShow.MessageShow("用户信息设置", "删除成功！");
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
                frmMsgShow.MessageShow("用户信息设置", "删除的用户名不存在或未选中行，请重新选择！");
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

        
    }
}
