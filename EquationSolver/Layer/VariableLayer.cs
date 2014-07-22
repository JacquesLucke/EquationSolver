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
            double value = Double.NaN;
            if (variableToNumberDictionary != null)
                if (variableToNumberDictionary.ContainsKey(name))
                    value = variableToNumberDictionary[name];
            return value;
        }

        public HashSet<char> GetVariables()
        {
            HashSet<char> variables = new HashSet<char>();
            variables.Add(name);
            return variables;
        }
        public void Simplify()
        { }

        public override string ToString()
        {
            return Convert.ToString(name);
        }
    }
}
