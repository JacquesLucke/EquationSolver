using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    public class LogarithmLayer : ILayer
    {
        ILayer baseOfLogarithm;
        ILayer number;

        public LogarithmLayer()
        {
            baseOfLogarithm = new NumberLayer(Math.E);
            number = new NumberLayer(1);
        }

        public ILayer BaseOfLogarithm
        {
            get { return baseOfLogarithm; }
            set { baseOfLogarithm = value; }
        }
        public ILayer Number
        {
            get { return number; }
            set { number = value; }
        }

        public double Calculate(Dictionary<char, double> variableToNumberDictionary)
        {
            return Math.Log(number.Calculate(variableToNumberDictionary), baseOfLogarithm.Calculate(variableToNumberDictionary));
        }

        public void Simplify()
        {
            SimplifyChildren();
        }
        private void SimplifyChildren()
        {
            baseOfLogarithm.Simplify();
            number.Simplify();
        }

        public bool NeedsBrackets()
        {
            return false;
        }
        public override string ToString()
        {
            string numberString = number.ToString();
            if (number.NeedsBrackets()) numberString = "(" + numberString + ")";
            string baseString = baseOfLogarithm.ToString();
            if (baseOfLogarithm.NeedsBrackets()) baseString = "(" + baseString + ")";

            return "log_" + baseString + "_" + numberString;
        }
    }
}
