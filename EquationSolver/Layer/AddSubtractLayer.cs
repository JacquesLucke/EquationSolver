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

        public void StrongSimplification()
        {
            CalculateNonVariableTerms();
            CombineMultiplyDivideLayers();
        }
        private void CalculateNonVariableTerms()
        {
            foreach (ILayer layer in additions)
                layer.StrongSimplification();
            foreach (ILayer layer in subtractions)
                layer.StrongSimplification();

            NumberLayer newNumber = new NumberLayer(0);
            for (int i = 0; i < additions.Count; i++)
            {
                if (!Double.IsNaN(additions[i].Calculate(null)))
                {
                    newNumber.Value += additions[i].Calculate(null);
                    additions.RemoveAt(i);
                    i--;
                }
            }
            for (int i = 0; i < subtractions.Count; i++)
            {
                if (!Double.IsNaN(subtractions[i].Calculate(null)))
                {
                    newNumber.Value -= subtractions[i].Calculate(null);
                    subtractions.RemoveAt(i);
                    i--;
                }
            }
            if (newNumber.Value > 0) additions.Add(newNumber);
            if (newNumber.Value < 0)
            {
                newNumber.Value = -newNumber.Value;
                subtractions.Add(newNumber);
            }
        }
        
        private void CombineMultiplyDivideLayers()
        {
            Layer.ReplaceNumbersWithMultiplyLayers(additions);
            Layer.ReplaceNumbersWithMultiplyLayers(subtractions);

            List<MultiplyDivideLayer> all = new List<MultiplyDivideLayer>();
            all.AddRange(Layer.GetAllOfType<MultiplyDivideLayer>(additions));
            all.AddRange(Layer.GetAllOfType<MultiplyDivideLayer>(subtractions));

            List<ILayer> subFactors = Layer.GetAllFactors(all);

            List<KeyValuePair<ILayer, ILayer>> pairs = new List<KeyValuePair<ILayer, ILayer>>();
            for (int i = 0; i < subFactors.Count - 1; i++)
            {
                for (int j = i + 1; j < subFactors.Count; j++)
                {
                    if (Layer.Compare(subFactors[i], subFactors[j]))
                    {
                        pairs.Add(new KeyValuePair<ILayer, ILayer>(subFactors[i], subFactors[j]));
                    }
                }
            }

            for (int i = 0; i < pairs.Count; i++)
            {
                for (int j = 0; j < all.Count; j++)
                {
                    if (all[j].Factors.Contains(pairs[i].Key) && all[j].Factors.Contains(pairs[i].Value) || pairs[i].Key.GetVariables().Count == 0)
                    {
                        pairs.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }

            if (pairs.Count > 0)
            {
                KeyValuePair<ILayer, ILayer> pair = pairs[0];
                KeyValuePair<MultiplyDivideLayer, MultiplyDivideLayer> parents;
                parents = new KeyValuePair<MultiplyDivideLayer, MultiplyDivideLayer>(FindParentLayer(pair.Key, all), FindParentLayer(pair.Value, all));

                MultiplyDivideLayer newLayer = new MultiplyDivideLayer();
                newLayer.Factors.Add(pair.Key);
                AddSubtractLayer layerPart = new AddSubtractLayer();

                MultiplyDivideLayer l1 = new MultiplyDivideLayer();
                foreach (ILayer layer in parents.Key.Factors)
                    if (layer != pair.Key) l1.Factors.Add(layer);
                foreach (ILayer layer in parents.Key.Divisors)
                    if (layer != pair.Key) l1.Divisors.Add(layer);

                MultiplyDivideLayer l2 = new MultiplyDivideLayer();
                foreach (ILayer layer in parents.Value.Factors)
                    if (layer != pair.Value) l2.Factors.Add(layer);
                foreach (ILayer layer in parents.Value.Divisors)
                    if (layer != pair.Value) l2.Divisors.Add(layer);

                if (additions.Contains(parents.Key)) layerPart.Additions.Add(l1);
                else layerPart.Subtractions.Add(l1);
                if (additions.Contains(parents.Value)) layerPart.Additions.Add(l2);
                else layerPart.Subtractions.Add(l2);

                newLayer.Factors.Add(layerPart);

                if (additions.Contains(parents.Key)) additions.Remove(parents.Key);
                if (additions.Contains(parents.Value)) additions.Remove(parents.Value);
                if (subtractions.Contains(parents.Key)) subtractions.Remove(parents.Key);
                if (subtractions.Contains(parents.Value)) subtractions.Remove(parents.Value);

                additions.Add(newLayer);

                StrongSimplification();
            }
        }
        private MultiplyDivideLayer FindParentLayer(ILayer child, List<MultiplyDivideLayer> possibleParents)
        {
            foreach(MultiplyDivideLayer layer in possibleParents)
            {
                if (layer.Factors.Contains(child)) return layer;
            }
            return null;
        }

        public void Simplify()
        {
            SimplifyChildren();

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
