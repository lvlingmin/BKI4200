using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Windows.Forms;

namespace Common
{
    [Serializable]
    public class ProjectXml
    {
        /// <summary>
        /// 项目编号
        /// </summary>
        public string ProjectNumber;
        /// <summary>
        /// 项目简称
        /// </summary>
        public string ShortName;
        /// <summary>
        /// 项目全称
        /// </summary>
        public string FullName;
        /// <summary>
        /// 项目类型
        /// </summary>
        public string ProjectType;
        /// <summary>
        /// 稀释倍数
        /// </summary>
        public int? DiluteCount;
        /// <summary>
        /// 范围精度类型
        /// </summary>
        public string RangeType;
        /// <summary>
        /// 取值范围一
        /// </summary>
        public string ValueRange1;
        /// <summary>
        /// 取值范围二
        /// </summary>
        public string ValueRange2;

        /// <summary>
        /// 范围值的取值单位
        /// </summary>
        public string ValueUnit;
        public double? MinValue;

        public double? MaxValue;
        /// <summary>
        /// 定标点的个数
        /// </summary>
        public int? CalPointNumber;
        /// <summary>
        /// 定标点的各个浓度
        /// </summary>
        public string CalPointConc;
        /// <summary>
        /// 质控点个数
        /// </summary>
        public int? QCPointNumber;
        /// <summary>
        /// 质控点是定标点中的哪几个
        /// </summary>
        public string QCPoints;
        /// <summary>
        /// 实验步骤
        /// </summary>
        public string ProjectProcedure;
        /// <summary>
        /// 定标模式
        /// </summary>
        public int? CalMode;
        /// <summary>
        /// 校准方法
        /// </summary>
        public int? CalMethod;
        /// <summary>
        /// 计算方法（用于定性实验）
        /// </summary>
        public string CalculateMethod;
        /// <summary>
        /// 载入类型
        /// </summary>
        public int? LoadType;
        /// <summary>
        /// 激活状态
        /// </summary>
        public int? ActiveStatus;
        /// <summary>
        /// 稀释液名称，LYN add 20171114
        /// </summary>
        public string DiluteName;
        /// <summary>
        /// 定标有效期  //2018-08-07 
        /// </summary>
        public int ExpiryDate;
        /// <summary>
        /// 取液剩余量  //2018-10-13 zlx add
        /// </summary>
        public string NoUsePro;
        /// <summary>
        /// 取值范围类型 //3019-06-06 zlx add
        /// </summary>
        public string VRangeType;
        public static ProjectXml GetProjectInfo(string projectPath)
        {
            if (System.IO.File.Exists(projectPath))
                return (ProjectXml)SerializerHelper.XMLDeserialize(typeof(ProjectXml), projectPath);
            throw new Exception("没有当前选择的文档！");
        }
        public void SaveToXml()
        {
        
        SerializerHelper.XMLSerialize(this, Application.StartupPath + @"\Programs\" + "模板" + ".xml");

        }
    }
}
