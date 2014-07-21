using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    public class NumberLayer : ILayer
    {
        double value;

        public NumberLayer(double value)
        {
            this.value = value;
        }
        public double Value
        {
            get { return value; }
        }

        public bool NeedsBrackets()
        {
            return value < 0;
        }

        public double Calculate(Dictionary<char, double> variableToNumberDictionary)
        {
            return value;
        }

        public override string ToString()
        {
            return Convert.ToString(value);
        }
    }
}
