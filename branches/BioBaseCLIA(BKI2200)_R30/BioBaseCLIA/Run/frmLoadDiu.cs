using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BioBaseCLIA.Run
{
    public partial class frmLoadDiu : frmSmallParent
    {
        /// <summary>
        /// 试剂位置
        /// </summary>
        public int RegentPos { get; set; }
        /// <summary>
        /// 试剂盘配置文件地址
        /// </summary>
        string iniPathReagentTrayInfo = Directory.GetCurrentDirectory() + "\\ReagentTrayInfo.ini";
        public frmLoadDiu()
        {
            InitializeComponent();
            
        }
        private void frmLoadDiu_Load(object sender, EventArgs e)
        {
            txtRegentPos.Text = RegentPos.ToString();
            cmbDiuPos.DataSource = frmParent.DiuPosList;
            string DiuPos = OperateIniFile.ReadIniData("ReagentPos" + RegentPos, "DiuPos", "", iniPathReagentTrayInfo);
            if (DiuPos != "")
            {
                cmbDiuPos.Text = DiuPos;
                label4.Text = "已绑定";
            }
            else
                label4.Text = "未绑定";
        }
        private void btnLoadSubstrate_Click(object sender, EventArgs e)
        {
            if(cmbDiuPos.SelectedItem!=null && cmbDiuPos.SelectedItem.ToString()!="")
            {
                OperateIniFile.WriteIniData("ReagentPos" + RegentPos, "DiuPos", cmbDiuPos.SelectedItem.ToString(), iniPathReagentTrayInfo);
                label4.Text = "已绑定";
                frmMessageShow frmMessage = new frmMessageShow();
                frmMessage.MessageShow("绑定稀释液","绑定稀释液信息成功！");
            }
                
        }

        private void functionButton1_Click(object sender, EventArgs e)
        {
            OperateIniFile.WriteIniData("ReagentPos" + RegentPos, "DiuPos", "", iniPathReagentTrayInfo);
            label4.Text = "未绑定";
            frmMessageShow frmMessage = new frmMessageShow();
            frmMessage.MessageShow("解绑稀释液","稀释液信息成功解绑！");
        }

       
    }
}
