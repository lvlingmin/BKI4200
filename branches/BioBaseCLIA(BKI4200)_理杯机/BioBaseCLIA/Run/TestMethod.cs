using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

namespace BioBaseCLIA.Run
{
    /// <summary>
    /// 实验方法类
    /// </summary>
    public class TestMethod
    {
        /// <summary>
        /// 样本ID
        /// </summary>
        public int SampleID { get; set; }
        /// <summary>
        /// 实验号
        /// </summary>
        public int TestID { get; set; }
        /// <summary>
        /// 步骤号
        /// </summary>
        public int StepID { get; set; }

        /// <summary>
        /// 样本编号
        /// </summary>
        public string SampleNo { get; set; }
        /// <summary>
        /// 样本杯类型
        /// </summary>
        public string SampleContainer { get; set; }
        /// <summary>
        /// 样本位置
        /// </summary>
        public int SamplePos { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 步骤加液量
        /// </summary>
        public string AddLiqud { get; set; }

        /// <summary>
        /// 是否急诊,1代表是急诊，0代表不是
        /// </summary>
        public int Emergency { get; set; }

        /// <summary>
        /// 项目方法，1代表只加一次试剂和两次加试剂连续，3代表两次加试剂不连续
        /// </summary>
        public int ProMethod { get; set; }
        /// <summary>
        /// 实验步骤
        /// </summary>
        public ExperimentStep StepName { get; set; }
        /// <summary>
        /// 步骤时间长度
        /// </summary>
        public int timeLength { get; set; }
        /// <summary>
        /// 稀释倍数
        /// </summary>
        public string dilutionTimes { get; set; }




    }
    /// <summary>
    ///实验步骤枚举
    /// </summary>
    public enum ExperimentStep
    {
        /// <summary>
        /// 稀释
        /// </summary>
        Dilution,
        /// <summary>
        /// 加样
        /// </summary>
        AddSample,
        /// <summary>
        /// 加试剂1
        /// </summary>
        AddRegent1,
        /// <summary>
        /// 加试剂2
        /// </summary>
        AddRegent2,
        /// <summary>
        /// 加试剂3
        /// </summary>
        AddRegent3,
        /// <summary>
        /// 加磁珠
        /// </summary>
        AddBeads,
        /// <summary>
        /// 孵育
        /// </summary>
        Incubation,
        /// <summary>
        /// 清洗
        /// </summary>
        Clean,
        /// <summary>
        /// 加底物
        /// </summary>
        AddSubstrate,
        /// <summary>
        /// 读数
        /// </summary>
        Read,
        /// <summary>
        /// 加试剂D
        /// </summary>
        AddRegentD
    }
    /// <summary>
    /// 实验运行状态枚举
    /// </summary>
    public enum RunFlagStart { NoStart = 0, IsRuning = 1, IsStoping = 2, Stoped = 4 }
    class ConvertHelper
    {
        /// <summary> 
        ///  DataTable转换成泛型集合
        /// </summary> 
        /// <typeparam name="T">泛型集合类型</typeparam> 
        /// <param name="dt">DataTable</param> 
        /// <param name="dEnum">字典集合，Key为需要从转换为enum项的DataColumnName，Value为需要转换的枚举的类型</param> 
        /// <returns>以实体类为元素的泛型集合</returns> 
        public static IList<T> DataTableConvertToListGenuric<T>(DataTable dt, Dictionary<string, Type> dEnum) where T : new()
        {
            if (dt.Rows.Count > 0)
            {
                // 定义集合 
                List<T> ts = new List<T>();
                // 获得此模型的类型 
                Type type = typeof(T);
                //定义一个临时变量 
                string tempName = string.Empty;
                //遍历DataTable中所有的数据行  
                foreach (DataRow dr in dt.Rows)
                {
                    T t = new T();
                    //如果T是值类型，则先进行装箱
                    object obj = null;
                    if (!t.GetType().IsClass)
                    {
                        obj = t;
                    }
                    //获得此模型的公共属性 
                    PropertyInfo[] propertys = t.GetType().GetProperties();
                    //遍历该对象的所有属性 
                    foreach (PropertyInfo pi in propertys)
                    {
                        //将属性名称赋值给临时变量   
                        tempName = pi.Name;
                        //检查DataTable是否包含此列（列名==对象的属性名）     
                        if (dt.Columns.Contains(tempName))
                        {
                            // 判断此属性是否有Setter   
                            if (!pi.CanWrite) continue;//该属性不可写，直接跳出   
                            //取值   
                            object value = dr[tempName];
                            //如果非空，则赋给对象的属性   
                            if (value != DBNull.Value)
                            {
                                //如果有枚举项
                                if (dEnum != null)
                                {
                                    var queryResult = from n in dEnum
                                                      where n.Key == tempName
                                                      select n;
                                    //枚举集合中包含与当前属性名相同的项
                                    if (queryResult.Count() > 0)
                                    {
                                        if (obj != null)
                                        {
                                            //将字符串转换为枚举对象
                                            pi.SetValue(obj, Enum.Parse(queryResult.FirstOrDefault().Value, value.ToString()), null);
                                        }
                                        else
                                        {
                                            //将字符串转换为枚举对象
                                            pi.SetValue(t, Enum.Parse(queryResult.FirstOrDefault().Value, value.ToString()), null);
                                        }
                                    }
                                    else
                                    {
                                        if (obj != null)
                                        {
                                            pi.SetValue(obj, value, null);
                                        }
                                        else
                                        {
                                            pi.SetValue(t, value, null);
                                        }
                                    }
                                }
                                else
                                {
                                    if (obj != null)
                                    {
                                        pi.SetValue(obj, value, null);
                                    }
                                    else
                                    {
                                        pi.SetValue(t, value, null);
                                    }
                                }
                            }
                        }
                    }
                    T ta = default(T);
                    //拆箱
                    if (obj != null)
                    {
                        ta = (T)obj;
                    }
                    else
                    {
                        ta = t;
                    }
                    //对象添加到泛型集合中 
                    ts.Add(ta);
                }
                return ts;
            }
            else
            {
                throw new ArgumentNullException("转换的集合为空.");
            }
        }
    }
}
