using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BioBaseCLIA.Run
{
    class LoadingHelper
    {
        #region 相关变量定义
        /// <summary>
        /// 定义委托进行窗口关闭
        /// </summary>
        private delegate void CloseDelegate();
        public static LoaderForm loadingForm;
        private static readonly Object syncLock = new Object();  //加锁使用

        #endregion

        private LoadingHelper()
        {

        }

        /// <summary>
        /// 显示loading框
        /// </summary>
        public static void ShowLoadingScreen()
        {
            // Make sure it is only launched once.
            if (loadingForm != null)
                return;
            Thread thread = new Thread(new ThreadStart(LoadingHelper.ShowForm));
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        private static void ShowForm()
        {
            if (loadingForm != null && loadingForm.IsHandleCreated)
            {
                LoadingHelper.CloseFormInternal();
                //lock (syncLock)
                //{
                //    loadingForm.closeOrder();
                //    loadingForm = null;
                //}
                //loadingForm.Invoke(new CloseDelegate(LoadingHelper.CloseFormInternal));

            }
            loadingForm = new LoaderForm();
            loadingForm.TopMost = true;
            loadingForm.ShowDialog();
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public static void CloseForm()
        {
            Thread.Sleep(100); //可能到这里线程还未起来，所以进行延时，可以确保线程起来，彻底关闭窗口
            if (loadingForm != null && loadingForm.IsHandleCreated)
            {
                loadingForm.Invoke(new CloseDelegate(LoadingHelper.CloseFormInternal));
                //lock (syncLock)
                //{
                    //Thread.Sleep(100);
                    //if (loadingForm != null)
                    //{
                        //Thread.Sleep(100);  //通过三次延时，确保可以彻底关闭窗口
                        //2018-12-07 zlx mod
                    //if (loadingForm.IsHandleCreated)
                    //{
                    //    loadingForm.Invoke(new CloseDelegate(LoadingHelper.CloseFormInternal));
                    //    loadingForm = null;
                    //}
                    //}
                //}
            }
        }

        /// <summary>
        /// 关闭窗口，委托中使用
        /// </summary>
        private static void CloseFormInternal()
        {
            lock (syncLock)
            {
                if (loadingForm != null)
                {
                    loadingForm.closeOrder();
                    loadingForm = null;
                }
            }

        }

    }
}
