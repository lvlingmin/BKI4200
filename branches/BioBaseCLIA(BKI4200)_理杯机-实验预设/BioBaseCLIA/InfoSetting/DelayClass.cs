using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BioBaseCLIA.InfoSetting
{
    class DelayClass
    {
        public EventWaitHandle sign { get; set; }
        public bool Delayed
        {
            get { return _delay; }
        }
        private bool _delay;
        private DateTime endTime, doingTime, PauseTime;
        private bool aborted;
        //private bool Working = false;
        public void Wait(int DalayValue)
        {
            //_delay = false;
            //Working = true;
            endTime = DateTime.Now.AddMilliseconds(DalayValue);
            doingTime = endTime;
            while (DateTime.Now < doingTime)
            {
                if (aborted)
                {
                    //Working = false;
                    return;
                }
                try
                {
                    Thread.CurrentThread.Join(2);
                }
                catch
                {
                }


            }
            _delay = true;
            try
            {
                if (sign != null)
                    while (!sign.Set())
                    {
                        Thread.CurrentThread.Join(1);
                    }
            }
            catch
            {

            }
            //Working = false;
        }
        public void abort()
        {
            aborted = true;
            Thread.CurrentThread.Join(2);
        }
        public void reset()
        {
            aborted = _delay = false;
        }

        public void Pause()
        {
            //if (Working)
            doingTime = DateTime.MaxValue;
            PauseTime = DateTime.Now;
        }
        public void Goon()
        {
            //if (Working)
            doingTime = endTime + (DateTime.Now - PauseTime);
            endTime = doingTime;
        }
        ~DelayClass()
        {
            aborted = true;
        }
    }
}
