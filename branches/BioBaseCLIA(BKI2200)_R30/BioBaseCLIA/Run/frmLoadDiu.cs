using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
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
                label4.Text = getString("keywordText.Bound");
            }
            else
                label4.Text = getString("keywordText.Unbound");
        }
        private void btnLoadSubstrate_Click(object sender, EventArgs e)
        {
            if(cmbDiuPos.SelectedItem!=null && cmbDiuPos.SelectedItem.ToString()!="")
            {
                OperateIniFile.WriteIniData("ReagentPos" + RegentPos, "DiuPos", cmbDiuPos.SelectedItem.ToString(), iniPathReagentTrayInfo);
                label4.Text = getString("keywordText.Bound");
                frmMessageShow frmMessage = new frmMessageShow();
                frmMessage.MessageShow(getString("keywordText.BoundDiluent"), getString("keywordText.BindingSuccee"));
            }
                
        }

        private void functionButton1_Click(object sender, EventArgs e)
        {
            OperateIniFile.WriteIniData("ReagentPos" + RegentPos, "DiuPos", "", iniPathReagentTrayInfo);
            label4.Text = getString("keywordText.Unbound");
            frmMessageShow frmMessage = new frmMessageShow();
            frmMessage.MessageShow(getString("keywordText.Unbundling"), getString("keywordText.UnbindSuccess"));
        }

        private string getString(string key)
        {
            ResourceManager resManager = new ResourceManager(typeof(frmLoadDiu));
            return resManager.GetString(key).Replace(@"\n", "\n").Replace(@"\t", "\t");
        }
    }
}
