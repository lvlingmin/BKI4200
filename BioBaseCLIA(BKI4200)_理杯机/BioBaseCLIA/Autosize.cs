using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;


namespace BioBaseCLIA
{
    class Autosize
    {
        public void setTag(Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                con.Tag = con.Width + ":" + con.Height + ":" + con.Left + ":" + con.Top + ":" + con.Font.Size;
                if (con.Controls.Count > 0) setTag(con);
            }
        }
        string[] mytag;
        public void setControls(float newx, float newy, Control cons)
        {
            foreach (Control con in cons.Controls)
            {

                if (con.Tag != null && !(con is CustomControl.defineButton) && !(con is Disk.SampleReagentDisk))
                {
                    mytag = con.Tag.ToString().Split(new char[] { ':' });
                    float a = Convert.ToSingle(mytag[0]) * newx; con.Width = (int)a;
                    a = Convert.ToSingle(mytag[1]) * newy; con.Height = (int)(a);
                    a = Convert.ToSingle(mytag[2]) * newx;
                    con.Left = (int)(a);
                    a = Convert.ToSingle(mytag[3]) * newy; con.Top = (int)(a);
                    Single currentSize = Convert.ToSingle(mytag[4]) * Math.Min(newx, newy);
                    con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit); 
                }
                if (con is Disk.SampleReagentDisk)
                {
                    con.Left =(int)((con.Left+con.Width) * newx/2-con.Width/2);
                }
                if (con is Bar.ProgressBar)
                { 
                
                }
                if (con.Controls.Count > 0)
                {
                    setControls(newx, newy, con);
                }
                if (con is DataGridView)
                {
                    DataGridView dgv = con as DataGridView;
                    Cursor.Current = Cursors.WaitCursor;

                    int widths = 0;
                    for (int i = 0; i < dgv.Columns.Count; i++)
                    {
                        dgv.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.AllCells);  // 自动调整列宽  
                        widths += dgv.Columns[i].Width;   // 计算调整列后单元列的宽度和                       
                    }
                    //if (widths >= con.Size.Width)  // 如果调整列的宽度大于设定列宽  
                    //    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;  // 调整列的模式 自动  
                    //else
                    //    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;  // 如果小于 则填充  

                    Cursor.Current = Cursors.Default;
                }
            }
        }
    }
}
