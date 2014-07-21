using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    public class AddSubtractLayer : ILayer
    {
        List<ILayer> additions;
        List<ILayer> subtractions;

        public AddSubtractLayer()
        {
            additions = new List<ILayer>();
            subtractions = new List<ILayer>();
        }

        public List<ILayer> Additions
        {
            get { return additions; }
        }
        public List<ILayer> Subtractions
        {
            get { return subtractions; }
        }

        public double Calculate(Dictionary<char, double> variableToNumberDictionary)
        {
            double output = 0;

            for (int i = 0; i < additions.Count; i++)
            {
                output += additions[i].Calculate(variableToNumberDictionary);
            }
            for (int i = 0; i < subtractions.Count; i++)
            {
                output -= additions[i].Calculate(variableToNumberDictionary);
            }

            return output;
        }

        public override string ToString()
        {
            string s = "";

            for (int i = 0; i < additions.Count; i++)
            {
                s += additions[i].ToString();
                if (i < additions.Count - 1) s += "+";
            }
            for (int i = 0; i < subtractions.Count; i++)
            {
                s += "-" + subtractions[i].ToString();
            }
            return s;
        }
    }
}
