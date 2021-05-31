/********************************************************************************
** Company： biobase.cn
** auth：    jun
** date：    2019/4/10 
** desc：    解密类
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBUtility
{
    public class StringUtils
    {
        public static StringUtils instance = new StringUtils();

        #region 解密算法
        /// <summary>
        /// 将加密的数据还原
        /// </summary>
        /// <returns></returns>
        public string ToDecryption(string content)
        {
            int key = 4;
            char[] chs = content.ToCharArray();
            for (int i = 0; i < chs.Length; i++)
            {
                if (chs[i] >= 'a' && chs[i] <= 'z')
                {
                    chs[i] = (char)((chs[i] - 'a' + (26 - key)) % 26 + 'a');
                }
                else if (chs[i] >= 'A' && chs[i] <= 'Z')
                {
                    chs[i] = (char)((chs[i] - 'A' + (26 - key)) % 26 + 'A');
                }
                else if (chs[i] >= '0' && chs[i] <= '9')
                {
                    chs[i] = (char)((chs[i] - '0' + (10 - key)) % 10 + '0');
                }
                else
                {
                    chs[i] = (char)((chs[i] - '.' + (5 - key)) % 5 + '.');
                }
            }

            // 针对定标浓度和发光值 的小数点转化问题
            string str = new string(chs);
            if (chs[0] >= 3)
            {
                str = str.Replace('V', '.');
            }
            return str;
            //return new string(chs);
        }
        public string reverseDate(char oriDate)
        {
            if(oriDate == '.')
            {
                oriDate = 'V';
            }
            string date = "";
            if (oriDate >= '0' && oriDate <= '9')
            {
                date = oriDate.ToString();
            }
            else
            {
                date = ((oriDate - 'A') + 10).ToString();
            }
            return date;
        }
        #endregion
    }
}
