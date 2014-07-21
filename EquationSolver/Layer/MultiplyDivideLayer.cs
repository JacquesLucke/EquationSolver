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
                s += factors[i].ToString();
                if (i < factors.Count - 1) s += "*";
            }
            for (int i = 0; i < divisors.Count; i++)
            {
                s += "/" + divisors[i].ToString();
            }
            return s;
        }
    }
}
