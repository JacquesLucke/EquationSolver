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
            set { factors = value; }
        }
        public List<ILayer> Divisors
        {
            get { return divisors; }
            set { divisors = value; }
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
            StrongSimplificationOnChildren();
            GetBetterChildren();
            CalculateNonVariableLayers();
            RemoveOnes();
            LeaveOnlyZeroIfOneFactorIsZero();
            StrongSimplificationOnChildren(); 
            ReduceDuplicatesInFactorsAndDivisors();
        }
        private void GetBetterChildren()
        {
            for (int i = 0; i < factors.Count; i++)
                factors[i] = Layer.GetBetterChild(factors[i]);
            for (int i = 0; i < divisors.Count; i++)
                divisors[i] = Layer.GetBetterChild(divisors[i]);
        }
        private void StrongSimplificationOnChildren()
        {
            foreach (ILayer layer in factors)
                layer.StrongSimplification();
            foreach (ILayer layer in divisors)
                layer.StrongSimplification();
        }
        public void CalculateNonVariableLayers()
        {
            CalculateChildren();

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
        private void CalculateChildren()
        {
            foreach (ILayer layer in factors)
                layer.CalculateNonVariableLayers();
            foreach (ILayer layer in divisors)
                layer.CalculateNonVariableLayers();
        }
        private void RemoveOnes()
        {
            for (int i = 0; i < factors.Count; i++)
            {
                if (factors[i].Calculate(null) == 1)
                {
                    factors.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < divisors.Count; i++)
            {
                if (divisors[i].Calculate(null) == 1)
                {
                    divisors.RemoveAt(i);
                    i--;
                }
            }
        }
        private void LeaveOnlyZeroIfOneFactorIsZero()
        {
            bool isZero = false;
            foreach (ILayer layer in factors)
                if (layer.Calculate(null) == 0) isZero = true;

            if(isZero)
            {
                factors.Clear();
                divisors.Clear();
                factors.Add(new NumberLayer(0));
            }
        }
        private void ReduceDuplicatesInFactorsAndDivisors()
        {
            ILayer[] pair = null;
            for(int i = 0; i <factors.Count; i++)
            {
                for(int j = 0; j < divisors.Count; j++)
                {
                    if (Layer.Compare(factors[i], divisors[j])) pair = new ILayer[] { factors[i], divisors[j] };
                }
            }
            if(pair != null)
            {
                factors.Remove(pair[0]);
                divisors.Remove(pair[1]);
                ReduceDuplicatesInFactorsAndDivisors();
            }
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
            if (s == "") s = "1";
            return s;
        }
    }
}
