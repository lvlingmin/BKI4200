using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BioBaseCLIA.CalculateCurve;

namespace BioBaseCLIA.CalculateCurve
{
    public class CalculateFactory
    {
        public Calculater getCaler(int calType)
        {
            Calculater er = null;

            switch (calType)
            {

                case 0:
                    er = new FourPL();
                    break;
                case 1:
                    er = new newFourPL();
                    break;
                case 2:
                    er = new newFourPL();
                    break;

            }
                   
            return er;
        }
        public Calculater getCaler( List<double> pars)
        {
            Calculater er = null;
           
           er = new FourPL(pars);//四参数
                    
            return er;
        }
    }
}
