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

        public HashSet<char> GetVariables()
        {
            HashSet<char> variables = new HashSet<char>();

            foreach (ILayer layer in additions)
                variables.UnionWith(layer.GetVariables());

            foreach (ILayer layer in subtractions)
                variables.UnionWith(layer.GetVariables());

            return variables;
        }
        public void Simplify()
        {
            for (int i = 0; i < additions.Count; i++)
            {
                if (additions[i] is AddSubtractLayer)
                {
                    AddSubtractLayer layer = (AddSubtractLayer)additions[i];
                    additions.AddRange(layer.additions);
                    subtractions.AddRange(layer.subtractions);
                    additions.Remove(layer);
                }
            }
            for (int i = 0; i < subtractions.Count; i++)
            {
                if (subtractions[i] is AddSubtractLayer)
                {
                    AddSubtractLayer layer = (AddSubtractLayer)subtractions[i];
                    subtractions.AddRange(layer.additions);
                    additions.AddRange(layer.subtractions);
                    subtractions.Remove(layer);
                }
            }
            SimplifyChildren();
        }
        private void SimplifyChildren()
        {
            foreach (ILayer layer in additions)
            {
                layer.Simplify();
            }
            foreach (ILayer layer in subtractions)
            {
                layer.Simplify();
            }
        }

        public bool NeedsBrackets()
        {
            return (additions.Count + subtractions.Count) > 1;
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
                output -= subtractions[i].Calculate(variableToNumberDictionary);
            }

            return output;
        }

        public override string ToString()
        {
            string s = "";

            for (int i = 0; i < additions.Count; i++)
            {
                if (additions[i].NeedsBrackets() && additions[i] is AddSubtractLayer) s += "(" + additions[i].ToString() + ")";
                else s += additions[i].ToString();
                if (i < additions.Count - 1) s += "+";
            }
            for (int i = 0; i < subtractions.Count; i++)
            {
                if (subtractions[i].NeedsBrackets() && subtractions[i] is AddSubtractLayer) s += "-(" + subtractions[i].ToString() + ")";
                else s += "-" + subtractions[i].ToString();
            }
            return s;
        }
    }
}
