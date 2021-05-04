using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace BioBaseCLIA.Run
{
    public static class DiuInfo
    {
        public static int[] diuCount = { 1,10,20,50,100,200,500 };
        public static string GetDiuInfo(int diuTime)
        {
            string DiuInfo = "";
            switch(diuTime)
            {
                case 1:
                case 10:
                case 20:
                    DiuInfo = diuTime.ToString();
                    break;
                case 50:
                    DiuInfo = "5;10";
                    break;
                case 100:
                    DiuInfo = "10;10";
                    break;
                case 200:
                    DiuInfo = "10;20";
                    break;
                case 500:
                    DiuInfo = "20;25";
                    break;
            }
            return DiuInfo;
        }
        /// <summary>
        /// 定义稀释的时间
        /// </summary>
        static int diutime1, diutime2, diutime3, diutime4;
        /// <summary>
        /// 获取稀释时间
        /// </summary>
        /// <param name="diuCount">稀释次数</param>
        /// <returns></returns>
        public static int GetDiuTime(int diuCount)
        {
            int diutime = 0;
            switch (diuCount)
            {
                case 1:
                    diutime = diutime1;
                    break;
                case 2:
                    diutime = diutime2;
                    break;
                case 3:
                    diutime = diutime3;
                    break;
                case 4:
                    diutime = diutime4;
                    break;
                default:
                    break;
            }
            return diutime;
        }
        //读取稀释配置时间
        public static void ReadDiuTime()
        {
            diutime1 = int.Parse(OperateIniFile.ReadInIPara("Time", "dilutionTime1"));
            diutime2 = int.Parse(OperateIniFile.ReadInIPara("Time", "dilutionTime2"));
            diutime3 = int.Parse(OperateIniFile.ReadInIPara("Time", "dilutionTime3"));
            diutime4 = int.Parse(OperateIniFile.ReadInIPara("Time", "dilutionTime4"));
        }
        public static string[] DiuProjectName = { "SD1", "SD2", "SD3", "SD4", "SD5", "SD6" };

    }
   
}
