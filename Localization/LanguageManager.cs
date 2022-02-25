using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.Reflection;

namespace Localization
{
    public class LanguageManager
    {
        public static string LanguageName = "";
        private static LanguageManager _instance;
        public static LanguageManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LanguageManager();
                return _instance;
            }
        }

        private ResourceManager _LocalManager;
        protected ResourceManager LocalManager
        {
            get
            {
                LanguageName = Language.getLanName();
                if (string.IsNullOrEmpty(LanguageName))
                    return null;
                if (_LocalManager == null)
                {
                    Assembly tty = Assembly.Load("Localization");
                    ResourceManager m_ResourceManager = new System.Resources.ResourceManager("Localization." + LanguageName, tty);
                    _LocalManager = m_ResourceManager;
                }
                return _LocalManager;
            }
        }
        public string getLocaltionStr(string strKey)
        {
            string strt = (string)LocalManager.GetObject(strKey);
            if (!string.IsNullOrEmpty(strt))
            {
                strt = strt.Replace(@"\r\n", Environment.NewLine);
                strt = strt.Replace(@"\n", Environment.NewLine);
            }
            return strt;
        }
        public object getLocaltionObj(string strKey)
        {
            return LocalManager.GetObject(strKey);
        }
    }
}
