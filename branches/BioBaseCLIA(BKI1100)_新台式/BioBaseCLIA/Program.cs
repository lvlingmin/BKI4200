using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BioBaseCLIA.User;
using Localization;
using System.IO;
using System.Diagnostics;
//using BioBaseCLIA.Run;


namespace BioBaseCLIA
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //为程序的入口处增加捕获全局异常的功能，增加的内容包括try catch/以及三个捕获异常的方法，并会把信息写到文件中 jun add 20190328
            try
            {
                //只允许存在一个实例
                if ((Process.GetProcesses().Where(x => x.ProcessName == Process.GetCurrentProcess().ProcessName).Select(x => x).ToList().Count > 1))
                {
                    MessageBox.Show("程序正在运行，请勿重复启动!", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Language.AppCultureInfo = new System.Globalization.CultureInfo("zh-CN");
                //Language.AppCultureInfo = new System.Globalization.CultureInfo("en-US");
                System.Threading.Thread.CurrentThread.CurrentCulture = Language.AppCultureInfo;
                System.Threading.Thread.CurrentThread.CurrentUICulture = Language.AppCultureInfo;
                LanguageManager.LanguageName = Localization.Language.getLanName();//当前语言
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                #region 捕获未处理异常 jun add 20190328
                //处理未捕获的异常
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                //处理UI线程异常
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                //处理非UI线程异常
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                #endregion

                frmLogin frmlg = new frmLogin();
                if (frmlg.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new frmMain());
                }
                else
                {
                    Application.Exit();
                }
            }
            catch (Exception ex) 
            {
                string str = "";
                string strDateInfo = "出现应用程序未处理的异常：" + DateTime.Now.ToString() + "\r\n";
                if (ex != null)
                {
                    str = string.Format(strDateInfo + "异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n",
                         ex.GetType().Name, ex.Message, ex.StackTrace);
                }
                else
                {
                    str = string.Format("应用程序线程错误:{0}", ex);
                }
                writeLog(str);
                MessageBox.Show("发生系统错误，请及时联系技术人员！", "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        ///写两个方法，捕获未处理的异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string str = "";
            Exception error = e.ExceptionObject as Exception;
            string strDateInfo = "出现应用程序未处理的异常：" + DateTime.Now.ToString() + "\r\n";
            if (error != null)
            {
                str = string.Format(strDateInfo + "Application UnhandledException:{0};\n\r堆栈信息:{1}", error.Message, error.StackTrace);
            }
            else
            {
                str = string.Format("Application UnhandledError:{0}", e);
            }

            writeLog(str);
            MessageBox.Show("发生系统错误，请及时联系技术人员！", "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string str = "";
            string strDateInfo = "出现应用程序未处理的异常：" + DateTime.Now.ToString() + "\r\n";
            Exception error = e.Exception as Exception;
            if (error != null)
            {
                str = string.Format(strDateInfo + "异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n",
                     error.GetType().Name, error.Message, error.StackTrace);
            }
            else
            {
                str = string.Format("应用程序线程错误:{0}", e);
            }
            writeLog(str);
            MessageBox.Show("发生系统错误，请及时联系技术人员！", "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="str"></param>
        static void writeLog(string str)
        {
            if (!Directory.Exists("ErrLogs"))
            {
                Directory.CreateDirectory("ErrLogs");
            }
            using (StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + @"\Log\ErrLogs\ErrLog.txt", true))
            {
                sw.WriteLine("\n************************************************************************");
                sw.WriteLine(str);
                sw.WriteLine("************************************************************************\n");
                sw.Close();
            }
        }
    }
}
