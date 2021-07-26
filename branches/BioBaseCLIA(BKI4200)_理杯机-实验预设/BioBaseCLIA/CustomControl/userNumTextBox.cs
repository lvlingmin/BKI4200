using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BioBaseCLIA.CustomControl
{
    public partial class userNumTextBox : userTextBoxBase
    {
        frmMessageShow fms = new frmMessageShow();
        /// <summary>
        /// 小数
        /// </summary>
        private bool _isDecimal = false;
        /// <summary>
        /// 负数
        /// </summary>
        private bool _isNegative = false;
        /// <summary>
        /// 最小值
        /// </summary>
        private decimal _minValue = 0;
        /// <summary>
        /// 最大值
        /// </summary>
        private decimal _maxValue = 100;
        public userNumTextBox()
        {
            InitializeComponent();
        }
        #region 属性
        private bool _isNull = false;
        [Browsable(true)]
        [Description("是否允许为空"), Category("自定义")]
        public bool IsNull
        {
            get { return _isNull; }
            set
            {
                _isNull = value;
            }
        }

        private string _inputtext = "1";
        [Browsable(true)]
        [Description("与控件关联的文本"), Category("自定义")]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                Changed(value);
                base.Text = _inputtext;

            }
        }

        [RefreshProperties(RefreshProperties.Repaint)]
        [Browsable(true)]
        [Description("输入数字时最小值"), Category("自定义")]
        public decimal MinValue
        {
            get
            {

                return _minValue;
            }
            set
            {
                _minValue = (decimal)double.Parse(value.ToString());
            }
        }
        [Browsable(true)]
        [Description("输入数字时最大值"), Category("自定义")]
        public decimal MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = (decimal)double.Parse(value.ToString());
            }
        }

        [Browsable(true)]
        [Description("是否可输入小数"), Category("自定义"), DefaultValue(false)]
        public bool IsDecimal
        {
            get { return _isDecimal; }
            set
            {
                _isDecimal = value;
            }
        }

        [Browsable(true)]
        [Description("是否可输入负数"), Category("自定义"), DefaultValue(false)]
        public bool IsNegative
        {
            get { return _isNegative; }
            set
            {
                _isNegative = value;
            }
        }
        #endregion
        /// <summary>
        /// string转换decimal方法
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        Decimal ConvertToDcm(string strValue)
        {
            try
            {
                return Decimal.Parse(strValue);
            }
            catch
            {
                return MinValue;
            }
        }
        public void Changed(string value)
        {
            if (!IsNull)
            {
                if (string.IsNullOrEmpty(value))
                    _inputtext = MinValue.ToString();
                else if (ConvertToDcm(value) < MinValue)
                {
                    _inputtext = MinValue.ToString();
                }
                else if (ConvertToDcm(value) > MaxValue)
                {
                    _inputtext = MaxValue.ToString();
                }
                else
                    _inputtext = ConvertToDcm(value).ToString();
            }



        }
        protected override void OnValidating(System.ComponentModel.CancelEventArgs e)
        {
            base.OnValidating(e);
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (!IsDecimal)
                if (e.KeyChar == '.')
                    e.KeyChar = (char)Keys.None;
            if (!IsNegative)
                if (e.KeyChar == '-')
                    e.KeyChar = (char)Keys.None;
            if (SelectionStart > 0)
            {
                if (e.KeyChar == '.')
                {
                    if (Text.Substring((SelectionStart - 1), 1) == "-")
                    {
                        e.Handled = true;
                        return;
                    }
                    if (Text.IndexOf('.') != -1)
                    {
                        e.Handled = true;
                        return;
                    }
                }
                if (e.KeyChar == '-')
                    e.Handled = true;
                //return;
            }
            else
            {
                if (SelectionStart == 0)
                    if (e.KeyChar == '.')
                    {
                        e.Handled = true;
                        return;
                    }
            }
            if ((e.KeyChar > 65280) && (e.KeyChar < 65375))
                e.KeyChar = (char)(e.KeyChar - 65248);
            string temp = "-0123456789.";
            if (temp.IndexOf(e.KeyChar) == -1 && Convert.ToInt32(e.KeyChar) != 8)
            {
                e.Handled = true;

            }
            else
            {
                e.Handled = false;
            }

            base.OnKeyPress(e);
        }

        private void userNumTextBox_TextChanged_1(object sender, EventArgs e)
        {
            if (ConvertToDcm(Text) < MinValue || ConvertToDcm(Text) > MaxValue)
            {
                fms.MessageShow("", "请输入" + MinValue.ToString() + "-" + MaxValue.ToString() + "的值");
                this.Clear();
                this.Focus();
            }
        }
    }
}
