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
    public partial class FunctionButton : Button
    {

       
        //private
        public FunctionButton()
        {
            InitializeComponent();
        }

        public FunctionButton(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            
        }


        

        /// <summary>  
        /// 此组件的前景色，用于显示文本  
        /// </summary>  
        [Browsable(true), Category("外观"), Description("此组件的前景色，用于显示文本")]
        public new Color ForeColor
        {
            get { return base.ForeColor; }
            set
            {
                if (this.enabled)
                    base.ForeColor = value;
            }
        }  

        private bool enabled = true; 
        /// <summary>  
        /// 指示是否已启用该控件，如果要使用原有的Enabled禁用控件，需要设置EnabledSet达到目的，EnabledSet级别高于此属性级别  
        /// </summary>  
        [Browsable(true), Category("行为"), Description("指示是否已启用该控件")]
        public new bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                if (value == false)
                {
                    base.ForeColor = Color.Black;
                    base.BackgroundImage = Properties.Resources.灰显1;
                }
                else
                {
                    this.ForeColor = Color.Black;
                    base.BackgroundImage = Properties.Resources.主界面按钮;
                }
            }
        }

        /// <summary>  
        /// Enabled其否启用该控件  
        /// </summary>  
        public bool EnabledSet
        {
            get { return base.Enabled; }
            set { base.Enabled = value; }
        }  

        protected override bool ShowFocusCues
        {
            get
            {
                return false;
            }
        }
        public new System.Drawing.Image BackgroundImage
        {
           
            get
            {
                return base.BackgroundImage;
            }
            set
            {

                if (this.enabled)
                {
                    base.BackgroundImage = value;
                }
   
            }
           
        }

        public override System.Drawing.Color BackColor
        {
            get
            {
                return System.Drawing.Color.Transparent;
            }
        }



        private void FunctionButton_MouseEnter(object sender, EventArgs e)
        {
            Button button = sender as Button;
            button.FlatStyle = FlatStyle.Popup;
            button.FlatAppearance.BorderSize = 1;
        }

        private void FunctionButton_MouseLeave(object sender, EventArgs e)
        {
            Button button = sender as Button;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
        }
        protected override void OnClick(EventArgs e)
        {
            if (this.enabled)
                base.OnClick(e);
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            if (this.enabled)
                base.OnDoubleClick(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (this.enabled)
                base.OnMouseClick(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (this.enabled)
                base.OnMouseDoubleClick(e);
        }

        /// <summary>  
        /// 鼠标进入变换背景图片  
        /// </summary>  
        /// <param name="e"></param>  
        protected override void OnMouseEnter(EventArgs e)
        {
            if (this.enabled)
            {
                base.OnMouseEnter(e);
                //this.BackgroundImage = WApp.util.Ui.assistant.UIResurece.Own.ImgButtonSelected;  
            }
        }

        /// <summary>  
        /// 鼠标离开变换背景图片  
        /// </summary>  
        /// <param name="e"></param>  
        protected override void OnMouseLeave(EventArgs e)
        {
            if (this.enabled)
            {
                base.OnMouseLeave(e);
                //this.BackgroundImage = WApp.util.Ui.assistant.UIResurece.Own.ImgButtonNormal;  
            }
        }  

    }
}
