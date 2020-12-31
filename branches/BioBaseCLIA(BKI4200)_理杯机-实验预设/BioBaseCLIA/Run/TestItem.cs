using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioBaseCLIA.Run
{
    /// <summary>
    /// 实验项目
    /// </summary>
   public class TestItem
    {
        /// <summary>
        /// 样本ID
        /// </summary>
       public int SampleID { get; set; }
      /// <summary>
      /// 实验编号
      /// </summary>
       public int TestID { get; set; }
       /// <summary>
       /// 样本编号
       /// </summary>
       public string  SampleNo { get; set; }
       /// <summary>
       /// 样本位置
       /// </summary>
       public int SamplePos { get; set; }
       /// <summary>
       /// 样本类型
       /// </summary>
       public string SampleType { get; set; }
       /// <summary>
       /// 项目名称
       /// </summary>
       public string ItemName { get; set; }

       /// <summary>
       /// 进度
       /// </summary>
       public string Schedule { get; set; }
       /// <summary>
       /// 实验状态
       /// </summary>
       public string TestStatus { get; set; }
       /// <summary>
       /// 实验时间
       /// </summary>
       public string TestTime { get; set; }
       /// <summary>
       /// 试剂批号
       /// </summary>
       public string RegentBatch { get; set; }
       /// <summary>
       /// 使用底物位置 2018-10-17 zlx mod
       /// </summary>
       public string SubstratePipe { get; set; }
       /// <summary>
       /// 使用试剂位置 2019-03-22 zlx mod
       /// </summary>
       public string RegentPos { get; set; }
    }
    /// <summary>
    /// 实验结果
    /// </summary>
   public class TestResult:ICloneable
   {
       /// <summary>
       /// 样本ID
       /// </summary>
       public int SampleID { get; set; }
       /// <summary>
       /// 实验编号
       /// </summary>
       public int TestID { get; set; }
       /// <summary>
       /// 样本编号
       /// </summary>
       public string SampleNo { get; set; }
       /// <summary>
       /// 样本位置
       /// </summary>
       public int SamplePos { get; set; }
       /// <summary>
       /// 样本类型
       /// </summary>
       public string SampleType { get; set; }
       /// <summary>
       /// 项目名称
       /// </summary>
       public string ItemName { get; set; }
       /// <summary>
       /// 发光值
       /// </summary>
       public int PMT { get; set; }
       /// <summary>
       /// 浓度
       /// </summary>
       public string  concentration { get; set; }
       /// <summary>
       /// 实验结果
       /// </summary>
       public string Result { get; set; }
       /// <summary>
       /// S/CO值
       /// </summary>
       public string sco { get; set; }
       /// <summary>
       /// 实验范围1
       /// </summary>
       public string Range1 { get; set; }
       /// <summary>
       /// 实验范围2
       /// </summary>
       public string Range2 { get; set; }
       /// <summary>
       /// 所使用定标信息状态//2018-08-17
       /// </summary>
       public int Status { get; set; }
       /// <summary>
       /// 使用试剂批号  //2018-08-18
       /// </summary>
       public string ReagentBeach { get; set; }
       /// <summary>
       /// 使用底物位置 2018-10-17 zlx mod
       /// </summary>
       public string SubstratePipe { get; set; }
       /// <summary>
       /// 取值单位 2018-11-10 zlx add
       /// </summary>
       public string Unit { get; set; }

       object ICloneable.Clone()
       {
           return this.Clone();
       }
       public TestResult Clone()
       {
           return (TestResult)this.MemberwiseClone();
       } 
   }
    /// <summary>
    /// 定标信息
    /// </summary>
   public class ScalingInfo {
       /// <summary>
       /// 项目名称
       /// </summary>
       public string ItemName { get; set; }
       /// <summary>
       /// 标准品数量
       /// </summary>
       public string Num { get; set; }
       /// <summary>
       /// 试剂批号
       /// </summary>
       public string RegenBatch { get; set; }
       /// <summary>
       /// 实验性质，0为定性实验，1为定量实验
       /// </summary>
       public int testType { get; set; }
       /// <summary>
       /// 定量实验标准品的浓度
       /// </summary>
       public string TestConc { get; set; }
   }

   /// <summary>
   /// 性能测试
   /// </summary>
   public class performanceTest
   {
       /// <summary>
       /// 序号
       /// </summary>
       public int ID { set; get; }
       /// <summary>
       /// 加样位置
       /// </summary>
       public int addSamplePos { set; get; }
       /// <summary>
       /// 取样位置
       /// </summary>
       public int takeSamplePos { set; get; }
   
   }

}
