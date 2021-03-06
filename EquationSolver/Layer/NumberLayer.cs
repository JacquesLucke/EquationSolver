﻿using System;
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
            set { this.value = value; }
        }

        public bool NeedsBrackets()
        {
            return value < 0;
        }

        public double Calculate(Dictionary<char, double> variableToNumberDictionary)
        {
            return value;
        }

        public void ReplaceVariableWithLayer(char variable, ILayer layer)
        {
        }

        public HashSet<char> GetVariables()
        {
            return new HashSet<char>();
        }
        public void StrongSimplification()
        { }
        public void CalculateNonVariableLayers()
        { }
        public void Simplify()
        { }

        public override string ToString()
        {
            return Utils.GetOptimizedTextFromNumber(value);
        }
    }
}
