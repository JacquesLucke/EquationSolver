using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    public class VariableLayer : ILayer
    {
        char name;

        public VariableLayer(char name)
        {
            this.name = name;
        }

        public char Name
        {
            get { return name; }
        }

        public bool NeedsBrackets()
        {
            return false;
        }

        public double Calculate(Dictionary<char, double> variableToNumberDictionary)
        {
            return variableToNumberDictionary[name];
        }

        public override string ToString()
        {
            return Convert.ToString(name);
        }
    }
}
