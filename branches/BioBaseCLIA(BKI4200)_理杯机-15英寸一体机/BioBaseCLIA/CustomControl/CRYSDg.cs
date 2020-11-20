using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace BioBaseCLIA.CustomControl
{
    public partial class CRYSDg : DataGridView
    {
        private string _ColumnNames = "";
        private bool _enAbleSum = false; private bool isSum = false;
        [DefaultValue(""), Description("设定按钮渐变的前景色"), Category("自定义")]
        public string ColumnNames
        {
            get
            {
                return _ColumnNames;
            }
            set
            {
                _ColumnNames = value;
            }
        }
        [DefaultValue(false), Description("设定是否启用合计"), Category("自定义")]
        public bool EnAbleSum
        {
            get
            {
                return _enAbleSum;
            }
            set
            {
                _enAbleSum = value;
            }
        }
        public CRYSDg()
        {
            BackgroundColor = Color.White;
            RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            AllowUserToResizeColumns = false;
            AllowUserToResizeRows = false;
            //InitializeComponent();
        }
        public CRYSDg(IContainer container)
        {
            container.Add(this);

            BackgroundColor = Color.White;
            //InitializeComponent();

        }
        protected override void InitLayout()
        {
            if (EnAbleSum)
            {
                isSum = true;
                NewText(-1);//设置RowHeader对应的TextBox;
                this.HorizontalScrollBar.VisibleChanged += new EventHandler(HorizontalScrollBar_VisibleChanged);//水平
                this.VerticalScrollBar.VisibleChanged += new EventHandler(VerticalScrollBar_VisibleChanged);      //竖直
                this.HorizontalScrollBar.ValueChanged += new EventHandler(HorizontalScrollBar_ValueChanged);
                for (int i = 0; i < Columns.Count; i++)
                    NewText(i);
            }
            ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            base.InitLayout();
            //AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
        }
        //重画的事件有 OnRowPrePaint OnCellPainting OnRowPostPaint OnPaintBackground
        protected override void OnPaint(PaintEventArgs e)
        {
            //if (this.Rows.Count != 0)
            //{
            //    for (int i = 0; i < this.Rows.Count; i++)
            //    {
            //        if ((i % 2) == 1)
            //        {
            //            this.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(237, 249, 249);
            //        }
            //        else
            //        {
            //            this.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.White;
            //        }
            //    }
            //}
            base.OnPaint(e);
        }
        protected override void PaintBackground(Graphics graphics, Rectangle clipBounds, Rectangle gridBounds)
        {
            base.PaintBackground(graphics, clipBounds, gridBounds);
            drawline(graphics);
        }
        private void drawline(Graphics e)
        {
            int tt = 0;
            Pen drawPen = new Pen(Color.FromArgb(189, 193, 194), 1);
            if (RowHeadersVisible)
                e.DrawLine(drawPen, new Point(RowHeadersWidth, ColumnHeadersHeight), new Point(RowHeadersWidth, Height));
            tt += RowHeadersWidth;
            for (int i = 0; i < Columns.Count; i++)
            {
                if (Columns[i].Visible == true)
                {
                    if (getRight(i) > RowHeadersWidth)
                        e.DrawLine(drawPen, new Point(getRight(i), ColumnHeadersHeight), new Point(getRight(i), Height));
                }
            }
        }
        protected override void OnDataBindingComplete(DataGridViewBindingCompleteEventArgs e)
        {
            base.OnDataBindingComplete(e);
            // AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
        }
        //===============================================================================

        //===============================================================================
        void HorizontalScrollBar_ValueChanged(object sender, EventArgs e)
        {
            SetTxtLeft();
        }
        protected override void OnScroll(ScrollEventArgs e)
        {
            base.OnScroll(e);
            if (EnAbleSum)
                if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                    SetTxtLeft();
        }
        void VerticalScrollBar_VisibleChanged(object sender, EventArgs e)
        {
            if (VerticalScrollBar.Visible)
                this.VerticalScrollBar.Maximum = this.VerticalScrollBar.Maximum + findTextBox("txt").Width;
        }

        void HorizontalScrollBar_VisibleChanged(object sender, EventArgs e)
        {
            SetTxtTop();
        }
        //求和
        protected override void OnCellEndEdit(DataGridViewCellEventArgs e)
        {
            base.OnCellEndEdit(e);
            if (EnAbleSum)
                columnSum(e.ColumnIndex);
        }
        private void columnSum(int colIndex)
        {
            decimal cellValue = 0; decimal curCellValue = 0;
            string[] columnSums = ColumnNames.Split(',');
            if (columnSums != null)
                for (int tt = 0; tt < columnSums.Length - 1; tt++)
                {
                    for (int i = 0; i < Rows.Count; i++)
                    {
                        try
                        {
                            curCellValue = (decimal)double.Parse(Rows[i].Cells[columnSums[tt]].Value.ToString());
                        }
                        catch
                        {
                            curCellValue = 0;
                        }
                        cellValue += curCellValue;
                    }
                    findTextBox("txt" + Columns[columnSums[tt]].Index.ToString()).Text = cellValue.ToString();
                }
        }
        //所有列求和
        protected override void OnRowsRemoved(DataGridViewRowsRemovedEventArgs e)
        {
            base.OnRowsRemoved(e);
            if (EnAbleSum)
                columnSum(e.RowIndex);
        }

        protected override void OnRowHeadersWidthChanged(EventArgs e)
        {
            if (isSum)
                if (EnAbleSum)
                {
                    findTextBox("txt").Width = this.RowHeadersWidth;
                    for (int i = 0; i < this.ColumnCount; i++)
                    {
                        findTextBox("txt" + i.ToString()).Left = getLeft(i);
                    }
                }
            base.OnRowHeadersWidthChanged(e);
        }
        protected override void OnColumnWidthChanged(DataGridViewColumnEventArgs e)
        {
            if (isSum)
                if (EnAbleSum)
                {
                    findTextBox("txt" + e.Column.Index.ToString()).Width = e.Column.Width;
                    for (int i = e.Column.Index; i < this.ColumnCount; i++)
                    {
                        findTextBox("txt" + i.ToString()).Left = getLeft(i);
                    }
                }
            base.OnColumnWidthChanged(e);
        }

        protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
        {
            //this.RowTemplate.Height = 18;
            if (EnAbleSum)
                NewText(e.Column.Index);
            base.OnColumnAdded(e);
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            if (isSum)
                if (EnAbleSum)
                {
                    SetTxtTop();
                    SetTxtLeft();
                }
            base.OnSizeChanged(e);
        }

        //设置TextBox的Top，使之保持在DagtaGridView最下方；
        private void SetTxtTop()
        {
            int topTxt = 0;
            if (HorizontalScrollBar.Visible)
                topTxt = this.Height - this.HorizontalScrollBar.Height;
            else
                topTxt = this.Height;
            TextBox t = findTextBox("txt");
            t.Top = topTxt - t.Height;
            for (int i = 0; i < this.ColumnCount; i++)
            {
                TextBox t1 = findTextBox("txt" + i.ToString());
                t1.Top = topTxt - t1.Height;
            }
        }

        //根据TextBox的名字找到TextBox;
        private TextBox findTextBox(string txtName)
        {

            foreach (Control ctl in this.Controls)
            {
                if (ctl.Name == txtName)
                    return ctl as TextBox;
            }
            return null;
        }

        //设置TextBox的Left，使之与对应的Column的Left对应
        private void SetTxtLeft()
        {
            for (int i = 0; i < this.FirstDisplayedScrollingColumnIndex; i++)
            {
                findTextBox("txt" + i.ToString()).Visible = false;
            }
            int firstIndex = FirstDisplayedScrollingColumnIndex > 0 ? FirstDisplayedScrollingColumnIndex : 0;
            for (int j = firstIndex; j < this.ColumnCount; j++)
            {
                TextBox t = findTextBox("txt" + j.ToString());
                t.Visible = true;
                t.Left = getLeft(j);
            }
        }
        //得到Column的Left
        private int getLeft(int ColumnIndex)
        {
            if (ColumnIndex < 0) return 0;
            int left = this.RowHeadersWidth - this.FirstDisplayedScrollingColumnHiddenWidth;
            int firstIndex = FirstDisplayedScrollingColumnIndex > 0 ? FirstDisplayedScrollingColumnIndex : 0;
            for (int i = firstIndex; i < ColumnIndex; i++)
            {
                left += Columns[i].Width;
            }
            return left;
        }
        //得到Column的Right
        private int getRight(int ColumnIndex)
        {
            int tt = RowHeadersWidth;
            if (ColumnIndex < 0) return 0;
            if (Columns[ColumnIndex].Frozen == true)
            {
                for (int i = 0; i < ColumnIndex; i++)
                {
                    if (Columns[i].Visible)
                        tt += Columns[i].Width;
                }
                if (RowHeadersVisible)
                    return tt + Columns[ColumnIndex].Width;
                else
                    return tt + Columns[ColumnIndex].Width - RowHeadersWidth;
            }
            else
            {
                int temp = 0;
                for (int i = 0; i < Columns.Count; i++)
                {
                    if (Columns[i].Frozen && Columns[i].Visible)
                        temp += Columns[i].Width;
                }
                int left = this.RowHeadersWidth - this.FirstDisplayedScrollingColumnHiddenWidth;
                int firstIndex = FirstDisplayedScrollingColumnIndex > 0 ? FirstDisplayedScrollingColumnIndex : 0;
                for (int i = firstIndex; i < ColumnIndex; i++)
                {
                    if (Columns[i].Visible)
                        left += Columns[i].Width;
                }
                if (RowHeadersVisible)
                    return temp + left + Columns[ColumnIndex].Width;
                else
                    return temp + left + Columns[ColumnIndex].Width - RowHeadersWidth;
            }
        }
        //生成TextBox;
        private void NewText(int ColumnIndex)
        {
            TextBox t = new TextBox();
            t.ReadOnly = true;
            if (ColumnIndex < 0)
            {
                t.Name = "txt";
                t.Width = this.RowHeadersWidth;
                t.Text = "Sum";
            }
            else
            {
                t.Name = "txt" + ColumnIndex.ToString();
                t.Width = Columns[ColumnIndex].Width;
            }
            t.BackColor = System.Drawing.Color.LightBlue;
            t.Top = this.Height - t.Height;
            t.Left = getLeft(ColumnIndex);

            if (findTextBox(t.Name) == null)//这里谢谢　网友:wayit,这他对这里进行了修改。
            {
                this.Controls.Add(t);
                t.Show();
            }
        }
    }
}
