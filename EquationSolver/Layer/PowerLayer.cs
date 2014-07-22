﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    public class PowerLayer : ILayer
    {
        ILayer baseOfPower;
        ILayer exponent;

        public PowerLayer()
        {
            baseOfPower = new NumberLayer(0);
            exponent = new NumberLayer(1);
        }

        public ILayer BaseOfPower
        {
            get { return baseOfPower; }
            set { baseOfPower = value; }
        }
        public ILayer Exponent
        {
            get { return exponent; }
            set { exponent = value; }
        }

        public void Simplify()
        {
            SimplifyChildren();
        }
        private void SimplifyChildren()
        {
            baseOfPower.Simplify();
            exponent.Simplify();
        }

        public double Calculate(Dictionary<char, double> variableToNumberDictionary)
        {
            return Math.Pow(baseOfPower.Calculate(variableToNumberDictionary), exponent.Calculate(variableToNumberDictionary));
        }

        public override string ToString()
        {
            string exponentString = exponent.ToString();
            if (exponent.NeedsBrackets()) exponentString = "(" + exponentString + ")";
            string baseString = baseOfPower.ToString();
            if (baseOfPower.NeedsBrackets()) baseString = "(" + baseString + ")";

            return baseString + "^" + exponentString;
        }
        public bool NeedsBrackets()
        {
            return false;
        }
    }
}