using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        ///  报告指定 Unicode 字符在此字符串中的第no个匹配项的索引。
        /// </summary>
        /// <param name="content">原始字符串内容</param>
        /// <param name="no">序列字符串排列</param>
        /// <param name="value">要查找的字符串资源</param>
        /// <returns></returns>
        public static int NumberIndexOf(this string content, int no, char value)
        {
            if (string.IsNullOrEmpty(content)) throw new ArgumentNullException("参数为null");

            int index = -1;
            int charindex = 0;

            foreach (var item in content)
            {
                index++;
                if (item == value)
                {
                    charindex++;
                }

                if (no == charindex) return index;
            }

            return -1;
        }
    }
}
