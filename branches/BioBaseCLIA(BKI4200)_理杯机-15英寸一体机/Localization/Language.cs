using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Localization
{
    public class Language
    {
        public static CultureInfo AppCultureInfo;
        public static bool isValid()
        {
            switch (System.Globalization.CultureInfo.CurrentCulture.ToString())
            {
                case "en":
                case "en-029":
                case "en-AU":
                case "en-BZ":
                case "en-CA":
                case "en-GB":
                case "en-IE":
                case "en-JM":
                case "en-NZ":
                case "en-PH":
                case "en-TT":
                case "en-US":
                case "en-ZA":
                case "en-ZW":
                case "zh-CHS":
                case "zh-CHT":
                case "zh-CN":
                case "zh-HK":
                case "zh-MO":
                case "zh-SG":
                case "zh-TW":
                    return true;
                default:
                    return false;
            }
        }
        public static string getLanName()
        {
            switch (System.Globalization.CultureInfo.CurrentCulture.ToString())
            {
                case "en":
                case "en-029":
                case "en-AU":
                case "en-BZ":
                case "en-CA":
                case "en-GB":
                case "en-IE":
                case "en-JM":
                case "en-NZ":
                case "en-PH":
                case "en-TT":
                case "en-US":
                case "en-ZA":
                case "en-ZW":
                    return "en-US";
                case "zh-CHS":
                case "zh-CHT":
                case "zh-CN":
                case "zh-HK":
                case "zh-MO":
                case "zh-SG":
                case "zh-TW":
                    return "zh-CN";
            }
            return "zh-CN";
        }
    }
}
