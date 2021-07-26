using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;

namespace BioBaseCLIA.InfoSetting
{
    class Inspect//y add 20180419
    {
        /// <summary>
        /// 判断ip地址是否合法
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <returns></returns>//y add 2018.4.19
        public static bool InspectIP(string ip)
        {
            IPAddress IPP;
            bool bo = IPAddress.TryParse(ip, out IPP);
            bo = bo && Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");

            return bo;
        }

        /// <summary>//目前没有用到
        /// 检查姓名是否只由2-10位汉字、字母、星号、"."、下划线组成,可用于姓名、科室名、医院名等。
        /// </summary>
        /// <param name="name">姓名</param>
        /// <returns></returns>
        public static bool NameOnlycharacter(string name)
        {
            return Regex.IsMatch(name, @"^[\u4E00-\u9FA5A-Za-z\*\._]{2,10}$");
        }

        /// <summary>
        /// 检查姓名是否只由2-10位汉字、字母、星号、数字、"."、下划线组成,可用于姓名、科室名、医院名等。
        /// </summary>
        /// <param name="name">姓名</param>
        /// <returns></returns>
        public static bool NameOnlycharacter2(string name)
        {
            return Regex.IsMatch(name, @"^[\u4E00-\u9FA5A-Z0-9a-z\*\._]{2,10}$");
        }

        /// <summary>
        /// 检查输入是否只由至少一位汉字、字母、星号、数字、"."、下划线组成,可用于姓名、科室名、医院名等。
        /// </summary>
        /// <param name="name">姓名</param>
        /// <returns></returns>
        public static bool NameOnlycharacter3(string name)
        {
            return Regex.IsMatch(name, @"^[\u4E00-\u9FA5A-Z0-9a-z\*\._]+$");
        }

        /// <summary>
        /// 检查输入是否存在汉字、字母、星号、数字、"."、下划线以外的符号,可用于姓名、科室名、医院名等。
        /// </summary>
        /// <param name="name">姓名</param>
        /// <returns></returns>
        public static bool PasswordOnlycharacter(string name)//this y add 20180528
        {
            return Regex.IsMatch(name, @"^[\u4E00-\u9FA5A-Z0-9a-z\*\._]*$");
        }

        /// <summary>
        /// 检查账户名只能由三到十位大小写字母、数字、“*“、”_”组成，且不能以数字、“*“、”_”开头，即只能以字母开头。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool InspectID(string name)
        {
            return Regex.IsMatch(name, @"^[a-zA-Z][a-z0-9A-Z_\*]{2,9}$");
        }

        /// <summary>
        /// 验证密码，由任意个大小写字母、数字、“*”、“_”组成，可以为空。
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool InspectPassword(string password)
        {
            return Regex.IsMatch(password, @"^[a-zA-Z0-9_\*]*$");
        }
    }
}
