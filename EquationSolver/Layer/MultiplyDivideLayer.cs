using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    public class MultiplyDivideLayer : ILayer
    {
        List<ILayer> factors;
        List<ILayer> divisors;

        public MultiplyDivideLayer()
        {
            factors = new List<ILayer>();
            divisors = new List<ILayer>();
        }

        public List<ILayer> Factors
        {
            get { return factors; }
        }
        public List<ILayer> Divisors
        {
            get { return divisors; }
        }

        public HashSet<char> GetVariables()
        {
            HashSet<char> variables = new HashSet<char>();

            foreach (ILayer layer in factors)
                variables.UnionWith(layer.GetVariables());

            foreach (ILayer layer in divisors)
                variables.UnionWith(layer.GetVariables());

            return variables;
        }

        public void StrongSimplification()
        {
            CalculateNonVariableTerms();
        }
        private void CalculateNonVariableTerms()
        {
            foreach (ILayer layer in factors)
                layer.StrongSimplification();
            foreach (ILayer layer in divisors)
                layer.StrongSimplification();

            NumberLayer newNumber = new NumberLayer(1);
            for (int i = 0; i < factors.Count; i++)
            {
                if (!Double.IsNaN(factors[i].Calculate(null)))
                {
                    newNumber.Value *= factors[i].Calculate(null);
                    factors.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < divisors.Count; i++)
            {
                if (!Double.IsNaN(divisors[i].Calculate(null)))
                {
                    newNumber.Value /= divisors[i].Calculate(null);
                    divisors.RemoveAt(i);
                    i--;
                }
            }
            if(newNumber.Value != 1) factors.Add(newNumber);
        }

        public void Simplify()
        {
            SimplifyChildren();

            for (int i = 0; i < factors.Count; i++)
            {
                if (factors[i] is MultiplyDivideLayer)
                {
                    MultiplyDivideLayer layer = (MultiplyDivideLayer)factors[i];
                    factors.AddRange(layer.factors);
                    divisors.AddRange(layer.divisors);
                    factors.Remove(layer);
                }
            }
            for (int i = 0; i < divisors.Count; i++)
            {
                if (divisors[i] is MultiplyDivideLayer)
                {
                    MultiplyDivideLayer layer = (MultiplyDivideLayer)divisors[i];
                    divisors.AddRange(layer.factors);
                    factors.AddRange(layer.divisors);
                    divisors.Remove(layer);
                }
            }
        }
        private void SimplifyChildren()
        {
            foreach (ILayer layer in factors)
            {
                layer.Simplify();
            }
            foreach (ILayer layer in divisors)
            {
                layer.Simplify();
            }
        }

        public bool NeedsBrackets()
        {
            return (factors.Count + divisors.Count) > 1;
        }

        public double Calculate(Dictionary<char, double> variableToNumberDictionary)
        {
            double output = 1;

            for (int i = 0; i < factors.Count; i++)
            {
                output *= factors[i].Calculate(variableToNumberDictionary);
            }
            for (int i = 0; i < divisors.Count; i++)
            {
                output /= divisors[i].Calculate(variableToNumberDictionary);
            }

            return output;
        }

        public override string ToString()
        {
            string s = "";

            for (int i = 0; i < factors.Count; i++)
            {
                if (factors[i].NeedsBrackets()) s += "(" + factors[i].ToString() + ")";
                else s += factors[i].ToString();
                if (i < factors.Count - 1) s += "*";
            }
            for (int i = 0; i < divisors.Count; i++)
            {
                if (divisors[i].NeedsBrackets()) s += "/(" + divisors[i].ToString() + ")";
                else s += "/" + divisors[i].ToString();
            }
            return s;
        }
    }
}
