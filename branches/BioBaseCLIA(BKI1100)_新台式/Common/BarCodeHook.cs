/********************************************************************************
** Company： biobase.cn
** auth：    jun
** date：    2019/4/10 
** desc：    无焦点获取扫码信息的钩子类
** Ver.:     V1.0.0
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;

namespace Common
{
    /// <summary>
    /// 无焦点获取扫码信息的钩子类
    /// </summary> 
    public class BarCodeHook
    {
        public delegate void BarCodeDelegate(BarCodes barCode);
        public event BarCodeDelegate BarCodeEvent;

        /// <summary>
        /// 扫描信息
        /// </summary> 
        public struct BarCodes
        {
            public int VirtKey;      //虚拟码  
            public int ScanCode;     //扫描码  
            public string BarCode;   //条码信息  
            public bool IsValid;     //条码是否有效  
            public DateTime Time;    //扫描时间  
        }
        /// <summary>
        /// 事件信息
        /// </summary> 
        private struct EventMsg  
        {
            public int message;
            public int paramL;
            public int paramH;
            public int Time;
            public int hwnd;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);

        BarCodes barCode = new BarCodes();
        int hKeyboardHook = 0;
        public List<char> _barcode = new List<char>(100);
        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            if (nCode == 0)
            {
                EventMsg msg = (EventMsg)Marshal.PtrToStructure(lParam, typeof(EventMsg));

                if (wParam == 0x100)   //WM_KEYDOWN = 0x100  
                {
                    barCode.VirtKey = msg.message & 0xff;  //虚拟码  
                    barCode.ScanCode = msg.paramL & 0xff;  //扫描码  

                    if (DateTime.Now.Subtract(barCode.Time).TotalMilliseconds > 100)
                    {
                        _barcode.Clear();
                    }
                    else
                    {
                        if ((msg.message & 0xff) == 13 && _barcode.Count > 0)   //回车  
                        {
                            barCode.BarCode = new String(_barcode.ToArray());
                            barCode.IsValid = true;
                            _barcode.Clear();
                        }
                    }

                    barCode.Time = DateTime.Now;
                    if (BarCodeEvent != null) BarCodeEvent(barCode);    //触发事件  
                    barCode.IsValid = false;
                    _barcode.Add(Convert.ToChar(msg.message & 0xff));
                }
            }
            return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
        }

        private static HookProc hookproc;
         
        /// <summary>
        /// 安装钩子
        /// </summary> 
        public bool Start()
        {
            if (hKeyboardHook == 0)
            {
                hookproc = new HookProc(KeyboardHookProc);
                //WH_KEYBOARD_LL = 13 

                hKeyboardHook = SetWindowsHookEx(13, hookproc, GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);   //.net4可用
                //hKeyboardHook = SetWindowsHookEx(13, hookproc, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);  //仅.net3.5可以用
                GC.KeepAlive(hookproc);//增加一个保持不被垃圾回收的机制
            }
            return (hKeyboardHook != 0);
        }
 
        /// <summary>
        /// 卸载钩子
        /// </summary> 
        public bool Stop()
        {
            if (hKeyboardHook != 0)
            {
                bool ret = UnhookWindowsHookEx(hKeyboardHook);
                if (ret) hKeyboardHook = 0;
                //return UnhookWindowsHookEx(hKeyboardHook);
            }
            //return true;
            return hKeyboardHook == 0;
        }

        ~BarCodeHook()//为类增加一个析构函数，资源的及时回收
        {
            Stop();
        }
    }
}
