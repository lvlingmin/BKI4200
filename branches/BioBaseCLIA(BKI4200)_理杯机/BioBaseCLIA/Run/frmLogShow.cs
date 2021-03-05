using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Common;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Xml;
using System.Threading;
using Common.Extensions;

namespace BioBaseCLIA.Run
{
    public partial class frmLogShow : frmParent
    {
        /// <summary>
        /// 报警时更改主界面日志按钮的颜色
        /// </summary>
        public static event Action<int> btnLogColor1;
        public frmLogShow()
        {
            InitializeComponent();
        }

        String filePath = Application.StartupPath + @"\Log\AlarmLog";
        List<LogOfAlarm> lOrigin = new List<LogOfAlarm>();//
        List<LogOfAlarm> lShadow = new List<LogOfAlarm>();
        List<string> dateselect = new List<string>();
        frmMessageShow frmMsg = new frmMessageShow();
        ComponentResourceManager resources = new ComponentResourceManager(typeof(frmLogShow));

        public void ToEmpty()
        {
            lOrigin.Clear();
            lShadow.Clear();
            dateselect.Clear();
            //dateselect.Add("全部");
        }

        /// <summary>
        /// 从文件读取刷新原始数据链表和日期选择列表，并更改文件内容，标识是否已读
        /// </summary>
        public void toRefreshOriginAndDate()
        {
            ToEmpty();
            string filename = "I" + SelectDate.Value.ToString("yyyyMMdd") + ".txt";
            List<string> lstFiles = GetFiles(filePath, filename);
            //List<string> lstFiles = GetFiles(filePath, ".txt");
            lstFiles.Sort();
            lstFiles.Reverse();
            foreach (string lstFile in lstFiles)
            {
                if (lstFile.Length > 13)
                    continue;
                string newdate = lstFile.Substring(1, 4) +GetString("Year") + lstFile.Substring(5, 2) + GetString("Month") + lstFile.Substring(7, 2)+GetString("Date");
                dateselect.Add(newdate);
                lOrigin.Add(new LogOfAlarm(imageList1.Images[3], newdate, "----", null));
                string fileInfo = ReadTxtWarn.ReaderFile(filePath +"\\"+ lstFile);//all text
                if (lstFile.Contains(DateTime.Now.ToString("yyyyMMdd")))
                {
                    string fileinto = fileInfo.Replace(GetString("Notread") ,GetString("Haveread") );
                    ReadTxtWarn.WriteFile(filePath + "\\" + lstFile, fileinto);//将配置文本信息里的未读替换为已读
                }
                //string fileinto = fileInfo.Replace("未读", "已读");
                //ReadTxtWarn.WriteFile(filePath + "\\" + lstFile, fileinto);//将配置文本信息里的未读替换为已读
                string time;
                string module;
                string text;
                string[] allofthis = fileInfo.Split('\n');
                for (int i = allofthis.Count() - 1; i >= 0; i--)
                {
                    if (allofthis[i].Length < 27) continue;
                    time = allofthis[i].Substring(0, 8).Replace('-', ':');
                    module = allofthis[i].Substring(allofthis[i].IndexOf('*') + 3, 
                        allofthis[i].NumberIndexOf(4,'*')-(allofthis[i].IndexOf('*') + 3));
                    text = allofthis[i].Substring(allofthis[i].LastIndexOf('*') + 1);
                    Image bit = imageList1.Images[0];
                    if (module.Contains( GetString("Warning"))) bit = imageList1.Images[1];
                    if ((allofthis[i].Substring(allofthis[i].NumberIndexOf(6, '*'),
                        allofthis[i].NumberIndexOf(7, '*') - allofthis[i].NumberIndexOf(6, '*'))).Contains(GetString("Notread")))
                        bit.Tag = GetString("Notread");
                    else bit.Tag = GetString("Haveread");

                    lOrigin.Add(new LogOfAlarm(bit, time, text, null));//2018-11-02 zlx mod
                }
               
            }
            dateselect.Add(GetString("All"));
            if (!dateselect.Contains(DateTime.Now.ToString("yyyy"))&&
                !dateselect.Contains(DateTime.Now.ToString("MM"))&&
                !dateselect.Contains(DateTime.Now.ToString("dd")))
                dateselect.Insert(0, DateTime.Now.ToString("yyyy.MM.dd"));
            CBDateShow.DataSource = null;
            CBDateShow.DataSource = dateselect;
        }

        /// <summary>
        /// 日期过滤器：根据选择的日期，对数据进行筛选
        /// </summary>
        /// <param name="datefliter">待过滤的数据链表</param>
        /// <returns>过滤后的数据链表</returns>
        private List<LogOfAlarm> fliterOfdate(List<LogOfAlarm> datefliter)//filter of date
        {
            List<LogOfAlarm> lTemporary = new List<LogOfAlarm>();

            string datesele = SelectDate.Value.ToString("yyyy年MM月dd日");
            string datesele2 = SelectDate.Value.ToString("yyyy.MM.dd");
            bool putin = true;
            foreach (var a in datefliter)
            {
                //if (putin)
                //{
                //    if (a.Text != "----") continue;
                //}
                var y=DateTime.Now.ToString("yyyy");
                var y2=DateTime.Now.ToString("MM");
                var y3=DateTime.Now.ToString("dd");
                if ((a.Date.Contains(DateTime.Now.ToString("yyyy")) &&
                    a.Date.Contains(DateTime.Now.ToString("MM")))&&
                    a.Date.Contains(DateTime.Now.ToString("dd"))/*&& putin*/)
                {
                    putin = false;
                    lTemporary.Add(a);
                    continue;
                }
                if (!putin)
                {
                    if (a.Text == "----")
                    {
                        break;
                    }
                    lTemporary.Add(a);
                }
            }

            return lTemporary;
        }

        /// <summary>
        /// 去重过滤器：根据是否选定“去重”，确定是否筛除重复数据
        /// </summary>
        /// <param name="inter">待过滤的数据链表</param>
        /// <returns>过滤后的数据链表</returns>
        private List<LogOfAlarm> fliterOfRepeat(List<LogOfAlarm> inter)
        {
            if (notrepeat.Checked == false) return inter;
            else
            {
                List<LogOfAlarm> lTemporary = new List<LogOfAlarm>();
                foreach (var a in inter)
                {
                    if (a.Text == "----")
                    {
                        lTemporary.Add(a);
                        continue;
                    }
                    bool bo = true;
                    for (int temp = 0; temp < lTemporary.Count; temp++)//int temp = num+1; temp < inter.Count; temp++
                    {
                        if (a.Text == lTemporary[temp].Text && a.Bit.Tag == lTemporary[temp].Bit.Tag)//a.Text == inter[temp].Text
                        {
                            bo = false;
                            continue;
                        }
                    }
                    if (bo)
                    {
                        lTemporary.Add(a);
                    }
                }
            return lTemporary;
            }
        }

        /// <summary>
        /// 未读过滤器：根据是否选择“只看未读”，决定是否显示已读数据
        /// </summary>
        /// <param name="inter">待过滤的数据链表</param>
        /// <returns>过滤后的数据链表</returns>
        private List<LogOfAlarm> fliterOfonlyonread(List<LogOfAlarm> inter)
        {
            if (onlynoread.Checked == false) return inter;
            else
            {
                List<LogOfAlarm> lTemporary = new List<LogOfAlarm>();
                foreach (var a in inter)
                {
                    if (a.Bit.Tag == null)//this block modify by y
                    {
                        lTemporary.Add(a);
                        continue;
                    }
                    if (a.Bit.Tag.ToString() ==GetString("Notread") )//this block modify end
                    {
                        lTemporary.Add(a); 
                    }
                }
                return lTemporary;
            }
        }

        /// <summary>
        /// 类型过滤器：根据选择的类型，决定筛选后保留什么类型的数据
        /// </summary>
        /// <param name="inter">待过滤的数据链表</param>
        /// <returns>过滤后的数据链表</returns>
        private List<LogOfAlarm> fliterOfmodule(List<LogOfAlarm> inter)
        {
            if (CBmodule.Text ==GetString("All") ) return inter;
            else
            {
                List<LogOfAlarm> lTemporary = new List<LogOfAlarm>();
                if (CBmodule.Text ==GetString("CBmodule.Items1") )
                {
                    foreach (var a in inter)
                    {
                        if (CheckImg(a.Bit, imageList1.Images[1]) || CheckImg(a.Bit, imageList1.Images[3]))
                        {
                            lTemporary.Add(a);
                        }
                    }
                }
                if (CBmodule.Text ==GetString("CBmodule.Items2"))
                {
                    foreach (var a in inter)
                    {
                        if (CheckImg(a.Bit, imageList1.Images[0]) || CheckImg(a.Bit, imageList1.Images[3]))
                        {
                            lTemporary.Add(a);
                        }
                    }
                }
                return lTemporary;
            }
        }

        /// <summary>
        /// 空日期栏过滤器：去除没有日志信息的日期数据条。优先级5（最后）
        /// </summary>
        /// <param name="inter">待过滤的数据链表</param>
        /// <returns>过滤后的数据链表</returns>
        private List<LogOfAlarm> fliterOfnulldate(List<LogOfAlarm> inter)
        {
            List<LogOfAlarm> lTemporary = new List<LogOfAlarm>();
            for (int i = 0; i < inter.Count-1; i++)
            {
                if (inter[i].Text == "----" && inter[i + 1].Text == "----")
                {
                    continue;
                }
                lTemporary.Add(inter[i]);
            }
            if (inter.Count > 0 && inter[inter.Count - 1].Text != "----") lTemporary.Add(inter[inter.Count - 1]);///////////////////////////////////////
            return lTemporary;
        }

        /// <summary>
        /// 定位“详情”帮助连接linklable控件在表格中的位置，并进行控件添加。
        /// </summary>
        private void setLocation()//int? t=null
        {
            //if(t!=null)
            //this.DGVLog.Controls.Clear();
            foreach (var a in DGVLog.Controls.OfType<Control>().ToList<Control>())
            {
                DGVLog.Controls.Remove(a);
            }
            //int CellWidth = this.DGVLog.GetCellDisplayRectangle(this.DGVLog.CurrentCell.ColumnIndex, this.DGVLog.CurrentCell.RowIndex, true).Width;
            //int CellHeight = this.DGVLog.GetCellDisplayRectangle(this.DGVLog.CurrentCell.ColumnIndex, this.DGVLog.CurrentCell.RowIndex, true).Height;
            //int CellBottom = this.DGVLog.GetCellDisplayRectangle(this.DGVLog.CurrentCell.ColumnIndex, this.DGVLog.CurrentCell.RowIndex, true).Bottom;
            //int CellRight = this.DGVLog.GetCellDisplayRectangle(this.DGVLog.CurrentCell.ColumnIndex, this.DGVLog.CurrentCell.RowIndex, true).Right;
            int i = 0;
            foreach (var a in lShadow)
            {
                if (a.Butt != null)
                {
                    int Cellx = this.DGVLog.GetCellDisplayRectangle(3, i, true).Left;
                    int Celly = this.DGVLog.GetCellDisplayRectangle(3, i, true).Top;
                    if (Cellx == 0)
                    {
                        i++;
                        continue;
                    }
                    a.Butt.Location = new System.Drawing.Point(Cellx, Celly);
                    this.DGVLog.Controls.Add(a.Butt);
                }
                i++;
            }
        }

        /// <summary>
        /// 改变未读信息的字体
        /// </summary>
        private void setTheRowfont()
        {
            int i = 0;
            foreach (var a in lShadow)
            {
                if (GetString("Notread") ==(string)a.Bit.Tag)
                {
                    DGVLog.Rows[i].DefaultCellStyle.Font = new System.Drawing.Font("宋体",12f, FontStyle.Bold);
                }
                i++;
            }
        }

        /// <summary>
        /// 从原始数据链表读取数据，经过调用各个过滤器，形成用于与表格关联显示的shadow数据链表.并且显示到表格，同时修改表格样式。
        /// </summary>
        public void toRefreshShadowAndGrid()
        {
            lShadow = fliterOfnulldate(fliterOfRepeat(fliterOfdate(fliterOfmodule(fliterOfonlyonread(lOrigin)))));

            int width = DGVLog.Width;
            DGVLog.Columns["Bit"].Width = 50;
            DGVLog.Columns["Bit"].HeaderText =GetString("Type");
            DGVLog.Columns["Date"].Width = 150;
            DGVLog.Columns["Date"].HeaderText =GetString("Timeordate");
            int textwidth = width - DGVLog.Columns["Bit"].Width - DGVLog.Columns["Date"].Width - 125;
            if (textwidth < 1) textwidth = 1;
            DGVLog.Columns["Text"].Width = textwidth;
            DGVLog.Columns["Text"].HeaderText =GetString("Content") ;
            DGVLog.Columns["Butt"].Width = 65;
            DGVLog.Columns["Butt"].HeaderText = GetString("Help") ;

            DGVLog.DataSource = null;
            DGVLog.DataSource = lShadow;//数据源改变时触发事件
        }

        private void frmLogShow_Load(object sender, EventArgs e)
        {
            CBmodule.Text =GetString("All");
            toRefreshOriginAndDate();
            CBDateShow.SelectedIndex=0;
            toRefreshShadowAndGrid();
            SelectDate.MaxDate = DateTime.Now;
            this.groupBox1.Focus();
            if (btnLogColor1 != null)
            {
                this.BeginInvoke(new Action(() => { btnLogColor1(2); }));
            }
            timer1.Start();//2018-08-14 zlx  add
        }

        /// <summary>
        /// 取得指定文件夹下的指定格式的所有文件
        /// </summary>
        /// <param name="folder">指定的文件夹路径</param>
        /// <param name="extension">指定的扩展名</param>
        /// <returns></returns>
        private static List<string> GetFiles(string folder, string extension)
        {
            //若文件夹路径不存在，返回空
            if (!Directory.Exists(folder))
            {
                return null;
            }
            //扩展名必须存在
            if (string.IsNullOrEmpty(extension))
            {
                return null;
            }

            DirectoryInfo dInfo = new DirectoryInfo(folder);
            //文件夹下的所有文件
            FileInfo[] aryFInfo = dInfo.GetFiles(extension);
            //FileInfo[] aryFInfo = dInfo.GetFiles("I20201028.txt");
            List<string> lstRet = new List<string>();
            //将扩展名转化为小写的形式（如“.TXT”与“.txt”其实是相同的），方便后续处理
            //extension = extension.ToLower();
            //循环判断每一个文件
            foreach (FileInfo fInfo in aryFInfo)
            {
                lstRet.Add(fInfo.ToString());
            }
            return lstRet;
        }

        /// <summary>
        /// 比较两幅图片是否相同
        /// </summary>
        /// <param name="a1">第一幅图片</param>
        /// <param name="a2">第二幅图片</param>
        /// <returns>是或否</returns>
        public bool CheckImg(Image a1, Image a2)
        {
            Bitmap firstImage = (Bitmap)a1;
            Bitmap secondImage = (Bitmap)a2;

            bool flag = true;
            string firstPixel;
            string secondPixel;

            if (firstImage.Width == secondImage.Width
                && firstImage.Height == secondImage.Height)
            {
                for (int i = 0; i < firstImage.Width; i++)
                {
                    for (int j = 0; j < firstImage.Height; j++)
                    {
                        firstPixel = firstImage.GetPixel(i, j).ToString();
                        secondPixel = secondImage.GetPixel(i, j).ToString();
                        if (firstPixel != secondPixel)
                        {
                            flag = false;
                            break;
                        }
                        j++;
                    }
                    i++;
                }

                if (flag == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }  
        }


        private void frmLogShow_SizeChanged(object sender, EventArgs e)
        {
            formSizeChange(this);
        }

        private void fbtnReturn_Click(object sender, EventArgs e)
        {
            timer1.Stop();//2018-08-14 zlx add
            Close();
        }

        /// <summary>
        /// 帮助栏超链接“详情”linkclicked控件的事件点击方法,用于点击执行显示故障排除的信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel temp = (LinkLabel)sender;
            string position = (string)temp.Tag;

            //Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(@"..\..\Run\App.config");
            ////根据Key读取<add>元素的Value
            //string text = config.AppSettings.Settings[position].Value;
            //string text = ReadInIhelpoflog("guard", position);
            XmlDocument doc = new XmlDocument();

            //加载根目录下XML文件

            doc.Load("LogHelp.xml");

            //获取根节点

            XmlElement root = doc.DocumentElement;

            //获取<学生>子节点

            XmlNodeList personNodes = root.GetElementsByTagName("帮助信息");

            //使用foreach循环读出集合

            foreach (XmlNode node in personNodes)
            {

                string text = ((XmlElement)node).GetElementsByTagName(position.Substring(0,4))[0].InnerText;
                frmMsg.MessageShow( GetString("label1.Text") , text);
            }
        }

        private void BTfliter_Click(object sender, EventArgs e)
        {
            toRefreshShadowAndGrid();
        }

        /// <summary>
        /// 全部加载按钮点击事件，重新加载数据并复位页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BTrefresh_Click(object sender=null, EventArgs e=null)
        {
            onlynoread.CheckedChanged -= SelectedIndexChanged;
            notrepeat.CheckedChanged -= SelectedIndexChanged;
            CBDateShow.SelectedIndexChanged -= SelectedIndexChanged;
            CBmodule.SelectedIndexChanged -= SelectedIndexChanged;

            CBDateShow.Text =GetString("All");
            CBmodule.Text = GetString("All") ;
            onlynoread.Checked = false;
            notrepeat.Checked = false;

            toRefreshOriginAndDate();
            toRefreshShadowAndGrid();

            onlynoread.CheckedChanged += SelectedIndexChanged;
            notrepeat.CheckedChanged += SelectedIndexChanged;
            CBDateShow.SelectedIndexChanged += SelectedIndexChanged;
            CBmodule.SelectedIndexChanged += SelectedIndexChanged;

            this.groupBox1.Focus();
            if (btnLogColor1 != null)
            {
                this.BeginInvoke(new Action(() => { btnLogColor1(2); }));
            }
        }

        /// <summary>
        /// 表格滚动条滚动事件，更改linklable控件的位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DGVLog_Scroll(object sender, ScrollEventArgs e)
        {
            if (DGVLog.DataSource == lShadow)
            {
                setLocation();
            }
        }

        /// <summary>
        /// 表格栏宽度更改事件，更改linklable控件的位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DGVLog_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (DGVLog.DataSource == lShadow)
            {
                setLocation();
            }
        }

        /// <summary>
        /// 表格行宽度更改事件，更改linklable控件的位置。虽然表格已设定不允许更改行。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DGVLog_RowHeightChanged(object sender, DataGridViewRowEventArgs e)
        {
            if (DGVLog.DataSource == lShadow)
            {
                setLocation();
            }
        }

        /// <summary>
        /// 表格的数据源改变完成时触发的事件，确定linklable控件的位置并更改，加粗未读内容。确保数据都加载到表格后时再执行，这样可以避免意外。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DGVLog_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (DGVLog.DataSource == lShadow)
            {
                setLocation();
                setTheRowfont();
            }
        }

        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            toRefreshOriginAndDate();
            toRefreshShadowAndGrid();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int i = dateselect.Count;
            toRefreshOriginAndDate();
            toRefreshShadowAndGrid();

            this.groupBox1.Focus();
            if (btnLogColor1 != null)
            {
                this.BeginInvoke(new Action(() => { btnLogColor1(2); }));
            }
        }
        private string GetString(string key)
        {
            return resources.GetString(key);
        }
    }
}
