using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Data;
using System.Text.RegularExpressions;
using Maticsoft.DBUtility;
using Common;

namespace BioBaseCLIA.Run
{
    static class XmlOperation
    {
        const string path = "实验预设.Xml";
        static XmlDocument document = null;
        static Random random = null;
        /// <summary>
        /// 录入实验预设前缀名
        /// </summary>
        public const string PredictStart = "yb";//2019-02-26 zlx add
        /// <summary>
        /// 得到对应的Xml文件，如果没有就创建一个新的，然后加载
        /// </summary>
        public static void GetXmlFile()
        {
            document = new XmlDocument();
            if (!File.Exists(path))
            {
                CreateNewXmlFile(document);
            }
            document.Load(path);
        }
        /// <summary>
        /// 操作结束后关闭
        /// </summary>
        public static void EndXmlFile()
        {
            document = null;
        }

        /// <summary>
        /// xml文档插入一个新的实验结点
        /// </summary>
        /// <param name="ExperimentName"></param>
        /// <returns></returns>
        public static void InsertExperiment(string ExperimentName)
        {
            if (!CheckXmlName(ExperimentName)) return;
            if (document == null) GetXmlFile();
            if (document.DocumentElement.GetElementsByTagName(ExperimentName).Count == 0)
            {
                document.DocumentElement.AppendChild(CreateExperimentNode(document, ExperimentName));
                document.Save(path);
            }
        }
        /// <summary>
        /// xml文档在对应的实验项目结点下插入一个新的样品结点
        /// </summary>
        /// <param name="SampleName">样品名</param>
        /// <param name="ExperimentName">实验名</param>
        public static void InsertSample(string ExperimentName, string SampleName)
        {
            if (!(CheckXmlName(ExperimentName))) return;//&& CheckXmlName(SampleName)
            if (document == null) GetXmlFile();
            XmlNodeList nodeList = document.DocumentElement.GetElementsByTagName(ExperimentName);
            if (nodeList.Count == 0) InsertExperiment(ExperimentName);
            string PredictNum = OperateIniFile.ReadInIPara("TestResultSet", "PredictNum");
            if (PredictNum != "" || int.Parse(PredictNum) > 0)
            {
                SampleName = PredictStart + (int.Parse(SampleName.Substring(SampleName.Length - 3, 3)) % int.Parse(PredictNum));
            }
            else
            {
                SampleName = XmlOperation.PredictStart + int.Parse(SampleName.Substring(SampleName.Length - 3, 3));
            }
            if (((XmlElement)nodeList[0]).GetElementsByTagName(SampleName).Count!=0) return;
            if (nodeList.Count > 1)//数目异常处理
            {
                for (int i = 1; i < nodeList.Count; i++)
                {
                    XmlNodeList tempList = nodeList[i].ChildNodes;
                    foreach (var item in tempList)
                    {
                        if (item is XmlElement)
                        {
                            XmlElement temp = (XmlElement)item;
                            if (((XmlElement)nodeList[0]).GetElementsByTagName(temp.Name).Count == 0)
                            {
                                ((XmlElement)nodeList[0]).InsertBefore(temp, ((XmlElement)nodeList[0]).LastChild);
                            }
                        }
                    }
                    document.DocumentElement.RemoveChild(nodeList[i]);
                }
            }
            ((XmlElement)nodeList[0]).InsertBefore(CreatOneXmlElement(document, SampleName), ((XmlElement)nodeList[0]).LastChild);
            document.Save(path);
        }
        /// <summary>
        /// 查找有关实验的值
        /// </summary>
        /// <param name="experimentName">实验项目名称</param>
        /// <param name="sampleName">样本编号或者样本类型</param>
        /// <param name="IsPMT">是否要得到发光值，true：得到发光值;false:得到浓度值。</param>
        /// <returns></returns>
        public static string FindTestNumber(string experimentName,string sampleName)
        {
            try
            {
                if (!(CheckXmlName(experimentName) && CheckXmlName(sampleName))) return "-1";
                if (document == null) GetXmlFile();
                XmlNodeList list1 = document.DocumentElement.GetElementsByTagName(experimentName);
                if (list1.Count == 0) return "-1";
                XmlElement experiment = (XmlElement)list1[0];
                XmlNodeList list2 = experiment.GetElementsByTagName(sampleName);
                if (list2.Count == 0) return "-1";
                XmlElement sample = (XmlElement)list2[0];
                string[] tempNum = sample.InnerText.Trim().Split(';');
                tempNum[0] = tempNum[0].Trim();
                tempNum[1] = tempNum[1].Trim();

                if (tempNum[0] == "-1") return "-1";
                if (tempNum[1] == "-1" || tempNum[1] == "0") return tempNum[0];

                return RandomNumber(tempNum[0], tempNum[1]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.Source + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.TargetSite);
                return "-1";
            }
        }
        /// <summary>
        /// Xml字符串验证
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool CheckXmlName(string name)
        {
            bool temp = true;
            if (name.Length >= 3 && (name.Substring(0, 3) == "xml" || name.Substring(0, 3) == "Xml" || name.Substring(0, 3) == "XML")) temp = false;
            if (!Regex.IsMatch(name, @"^[\u4E00-\u9FA5A-Za-z]+[\u4E00-\u9FA5A-Za-z0-9-]*$")) temp = false;
            if (!temp)
            {
                MessageBox.Show("实验和样品名称只能包含英文字母、汉字或数字，且不能以数字、“xml”、“Xml”或“XML”开头。");
            }
            return temp;
        }
        /// <summary>
        /// 进行随机处理，返回原始值的随机范围内的一个值
        /// </summary>
        /// <param name="originalNum">靶值（原始值）</param>
        /// <param name="RandomRange">随机区间，一个表示百分比的值</param>
        /// <returns></returns>
        private static string RandomNumber(string originalNum, string RandomRange)
        {
            decimal origin;
            string[] tempOriange = originalNum.Split('.');
            string[] tempOriange2 = new string[2]{"",""};
            tempOriange2[0] = Regex.Replace(tempOriange[0], @"[^0-9]+", "");
            if (tempOriange.Length >= 2) tempOriange2[1] = Regex.Replace(tempOriange[1], @"[^0-9]+", "");
            if (!decimal.TryParse(tempOriange2[0]+"."+tempOriange2[1],out origin))
            {
                MessageBox.Show("数值转换错误", "来自XmlOperation.RandomNumber()//decimal");
                return originalNum;
            }

            int range;
            tempOriange = RandomRange.Split('.');
            tempOriange2[0] = Regex.Replace(tempOriange[0], @"[^0-9]+", "");
            if (!int.TryParse(tempOriange2[0],out range))
            {
                MessageBox.Show("数值转换错误", "来自XmlOperation.RandomNumber()//int");
                return originalNum;
            }
            if (range == 0 || range == -1) return origin.ToString("####0.###");
            if (range < 0) range = Math.Abs(range);

            decimal result;
            if (random == null) random = new Random();
            result = random.Next(-range, range);
            result += (decimal)random.NextDouble();
            result = result / (decimal)100 + (decimal)1;
            result = origin * (decimal)result;
            return result.ToString("####0.###");
        }
        private static void CreateNewXmlFile(XmlDocument document)//创建xml
        {
            document.AppendChild(document.CreateXmlDeclaration("1.0", "UTF-8", null));
            document.AppendChild(document.CreateComment("-1表示按照原始读数显示，不进行读数控制。分号之前的是靶值，应输入正数或零值表示要展示的值。"));
            document.AppendChild(document.CreateComment("对于各种定标液质控液来说，靶值代表发光值，其他是浓度。"));
            document.AppendChild(document.CreateComment("分号后的是变化百分比，应该是一个整数，表示一个百分数，是靶值的正负变化范围。"));
            document.AppendChild(document.CreateElement("实验预设"));

            DbHelperOleDb db = new DbHelperOleDb(0);
            BLL.tbProject bllPj = new BLL.tbProject();
            DataTable dtProject = bllPj.GetList("ActiveStatus=1").Tables[0];
            for (int i = 0; i < dtProject.Rows.Count; i++)
            {
                document.DocumentElement.AppendChild(CreateExperimentNode(document, dtProject.Rows[i][2].ToString()));
            }
            document.Save(path);
        }
        private static XmlElement CreateExperimentNode(XmlDocument document, string ExperimentName)// 创建一个新的实验结点，其中包含各个标准品质控品的模版
        {
            if (ExperimentName.Contains("xmlns")|| ExperimentName.Contains(":"))
            {
                MessageBox.Show("实验名称不应该包含“xmlns”或者“：”冒号", "来自方法CreateNode的警告");
                return null;
            }
            XmlElement experiment = document.CreateElement(ExperimentName);
            experiment.AppendChild(CreatOneXmlElement(document, "标准品A"));
            experiment.AppendChild(CreatOneXmlElement(document, "标准品B"));
            experiment.AppendChild(CreatOneXmlElement(document, "标准品C"));
            experiment.AppendChild(CreatOneXmlElement(document, "标准品D"));
            experiment.AppendChild(CreatOneXmlElement(document, "标准品E"));
            experiment.AppendChild(CreatOneXmlElement(document, "标准品F"));
            experiment.AppendChild(CreatOneXmlElement(document, "标准品G"));
            experiment.AppendChild(CreatOneXmlElement(document, "质控品L"));
            experiment.AppendChild(CreatOneXmlElement(document, "质控品M"));
            experiment.AppendChild(CreatOneXmlElement(document, "质控品H"));
            experiment.AppendChild(CreatOneXmlElement(document, "样品编号示例"));
            return experiment;
        }
        private static XmlElement CreatOneXmlElement(XmlDocument document, string Name)
        {
            XmlElement temp = document.CreateElement(Name);
            temp.AppendChild(document.CreateTextNode("-1;-1"));

            //XmlElement tempC = document.CreateElement("浓度");

            //XmlElement tempCCoreNum = document.CreateElement("靶值");
            //tempCCoreNum.AppendChild(document.CreateTextNode("-1"));
            //tempC.AppendChild(tempCCoreNum);

            //XmlElement tempCRange = document.CreateElement("变化百分比");
            //tempCRange.AppendChild(document.CreateTextNode("-1"));
            //tempC.AppendChild(tempCRange);

            //temp.AppendChild(tempC);
            return temp;
        }

    }
}
