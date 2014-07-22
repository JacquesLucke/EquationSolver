﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    public class RootLayer : ILayer
    {
        ILayer nthRoot;
        ILayer baseOfRoot;

        public RootLayer()
        {
            nthRoot = new NumberLayer(2);
            baseOfRoot = new NumberLayer(0);
        }

        public ILayer NthRoot
        {
            get { return nthRoot; }
            set { nthRoot = value; }
        }
        public ILayer BaseOfRoot
        {
            get { return baseOfRoot; }
            set { baseOfRoot = value; }
        }

        public double Calculate(Dictionary<char, double> variableToNumberDictionary)
        {
            return Math.Pow(baseOfRoot.Calculate(variableToNumberDictionary), 1 / nthRoot.Calculate(variableToNumberDictionary));
        }

        public HashSet<char> GetVariables()
        {
            HashSet<char> variables = new HashSet<char>();

            variables.UnionWith(nthRoot.GetVariables());
            variables.UnionWith(baseOfRoot.GetVariables());

            return variables;
        }
        public void StrongSimplification()
        {
            nthRoot.StrongSimplification();
            baseOfRoot.StrongSimplification();

            if (!Double.IsNaN(nthRoot.Calculate(null))) nthRoot = new NumberLayer(nthRoot.Calculate(null));
            if (!Double.IsNaN(baseOfRoot.Calculate(null))) baseOfRoot = new NumberLayer(baseOfRoot.Calculate(null));
        }
        public void Simplify()
        {
            SimplifyChildren();
        }
        private void SimplifyChildren()
        {
            nthRoot.Simplify();
            baseOfRoot.Simplify();
        }

        public bool NeedsBrackets()
        {
            return false;
        }
        public override string ToString()
        {
            string nString = nthRoot.ToString();
            if (nthRoot.NeedsBrackets()) nString = "(" + nString + ")";
            string baseString = baseOfRoot.ToString();
            if (baseOfRoot.NeedsBrackets()) baseString = "(" + baseString + ")";

            return "root_" + nString + "_" + baseString;
        }
    }
}
