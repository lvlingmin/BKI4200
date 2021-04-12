using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace BioBaseCLIA.Run
{
    class LogOfAlarm
    {
        public Image Bit { get; set; }

        readonly string date;
        public string Date { get { return date; } }

        readonly string text;
        public string Text { get { return text; } }

        readonly LinkLabel butt;
        public LinkLabel Butt { get { return butt; } }

        
        //readonly string module;
        //public string Module { get { return module; } }


        public LogOfAlarm(Image bit, string date, string text, LinkLabel butt)
        {
            this.date = date;
            this.text = text;
            Bit = bit;
            this.butt = butt;
        }

        public override string ToString()
        {
            return string.Format("日期:{0}类型:{1}文本:{2}",date,Bit.ToString(),text);
        }

    }
}
