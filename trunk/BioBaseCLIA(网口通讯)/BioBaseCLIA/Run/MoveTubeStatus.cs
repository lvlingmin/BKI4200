using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioBaseCLIA.Run
{
    /// <summary>
    /// 取放管的状态
    /// </summary>
    public class MoveTubeStatus:IEquatable<MoveTubeStatus>
    {
        /// <summary>
        /// 实验号
        /// </summary>
        public int TestId { get; set; }
        /// <summary>
        /// 当前步骤
        /// </summary>
        public int StepNum { get; set; }
        /// <summary>
        /// 取管位置(编写方法0-x),0代表管架，1代表反应盘，2代表清洗盘，x代表取管的位置
        /// </summary>
        public string TakeTubePos{ get; set; }
        /// <summary>
        /// 放管位置(编写方法0-x),0代表废弃处，1代表反应盘，2代表清洗盘，x代表取管的位置
        /// </summary>
        public string putTubePos { get; set; }
        /// <summary>
        /// 是否是没能执行正确的指令
        /// </summary>
        public bool isRetransmit { get; set; }

        public MoveTubeStatus() { }
        public MoveTubeStatus(MoveTubeStatus temp)
        {
            TestId = temp.TestId;
            StepNum = temp.StepNum;
            TakeTubePos = temp.TakeTubePos;
            putTubePos = temp.putTubePos;
            isRetransmit = false;
        }
        public void Copy(MoveTubeStatus temp)
        {
            TestId = temp.TestId;
            StepNum = temp.StepNum;
            TakeTubePos = temp.TakeTubePos;
            putTubePos = temp.putTubePos;
            isRetransmit = false;
        }

        public bool Equals(MoveTubeStatus temp)
        {
            if (temp == null) return false;
            string[] temp1 = TakeTubePos.Split('-');
            string[] temp2 = temp.TakeTubePos.Split('-');
            if (TestId == temp.TestId /*&& StepNum == temp.StepNum */&& temp1[0] == temp2[0] && putTubePos == temp.putTubePos/* && (temp1[0]=="0"?true:temp1[1]==temp2[1])*/) return true;
            else return false;
        }
    }

    public class AddOrder : EventArgs
    {
        public AddOrder(string order, bool isDoubleTimeAndSucceed)
        {
            this.order = order;
            IsDoubleTimeAndSucceed = isDoubleTimeAndSucceed;
        }

        public string order { get; set; }

        public bool IsDoubleTimeAndSucceed { get; set; }
        
    }

}
