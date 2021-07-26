using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BioBaseCLIA.SysMaintenance
{
    public partial class frmReconfirm : Form
    {
        public static string userType = "";

        private BLL.tbUser bllUser = new BLL.tbUser();
        public frmReconfirm()
        {
            InitializeComponent();
        }

        private void frmReconfirm_Load(object sender, EventArgs e)
        {
            userType = "";
            MessageBox.Show("要初始化配置文件，请重新输入账号密码进行验证。");
        }

        private void fbtnConfirm_Click(object sender, EventArgs e)
        {
            if (txtUserName.Text.Trim() == "" || txtUserPassword.Text.Trim() == "")
            {
                MessageBox.Show("输入项为空！");
                return;
            }
            fbtnConfirm.Enabled = false;
            #region 验证

            string name = txtUserName.Text.Trim();
            string password = txtUserPassword.Text.Trim();
            DataTable dtUser = bllUser.GetList("UserName='" + name + "'").Tables[0];

            if (dtUser.Rows.Count < 1) //账号错误
            {
                MessageBox.Show("账号密码错误！");
                fbtnConfirm.Enabled = true;
                return;
            }
            else
            {
                var dr = dtUser.Select("UserPassword='" + password + "'");
                if (dr.Length < 1) //密码错误
                {
                    MessageBox.Show("账号密码错误！");
                    fbtnConfirm.Enabled = true;
                    return;
                }
                else
                {
                    userType = dr[0]["RoleType"].ToString(); //账号权限
                }
            }
            if (userType != "1") //验证登录账号
            {
                MessageBox.Show("权限不足！");
                fbtnConfirm.Enabled = true;
                return;
            }
            #endregion


            fbtnConfirm.Enabled = true;
            this.Close();
        }

        private void fbtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmReconfirm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (userType != "1")
                userType = "";
        }
    }
}
