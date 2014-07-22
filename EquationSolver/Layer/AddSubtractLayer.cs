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
            StrongSimplificationOnChildren();
            CombineMultiplyDivideLayers();
            StrongSimplificationOnChildren();
            GetBetterChildrenLayers();
        }
        private void StrongSimplificationOnChildren()
        {
            foreach (ILayer layer in additions)
                layer.StrongSimplification();
            foreach (ILayer layer in subtractions)
                layer.StrongSimplification();
        }
        private void CalculateNonVariableTerms()
        {
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
        private void GetBetterChildrenLayers()
        {
            for (int i = 0; i < additions.Count; i++)
                additions[i] = Layer.GetBetterChild(additions[i]);
            for (int i = 0; i < subtractions.Count; i++)
                subtractions[i] = Layer.GetBetterChild(subtractions[i]);
        }

        private void CombineMultiplyDivideLayers()
        {
            Layer.ReplaceLayersWithMultiplyLayers(additions);
            Layer.ReplaceLayersWithMultiplyLayers(subtractions);

            List<MultiplyDivideLayer> all = new List<MultiplyDivideLayer>();
            all.AddRange(Layer.GetAllOfType<MultiplyDivideLayer>(additions));
            all.AddRange(Layer.GetAllOfType<MultiplyDivideLayer>(subtractions));

            List<ILayer> subFactors = Layer.GetAllFactors(all);
            List<ILayer[]> equalPairs = Layer.FindEqualPairs(subFactors);
            RemovePairsInSameLayer(all, equalPairs);

            if (equalPairs.Count > 0)
            {
                CombinePair(equalPairs[0], all);
                GetBetterChildrenLayers();
                StrongSimplification();
            }
        }
        private void RemovePairsInSameLayer(List<MultiplyDivideLayer> layers, List<ILayer[]> pairs)
        {
            for (int i = 0; i < pairs.Count; i++)
            {
                for (int j = 0; j < layers.Count; j++)
                {
                    if (layers[j].Factors.Contains(pairs[i][0]) && layers[j].Factors.Contains(pairs[i][1]) || pairs[i][0].GetVariables().Count == 0)
                    {
                        pairs.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }
        }
        private AddSubtractLayer CreateNonCombinedLayerPart(ILayer[] pair, MultiplyDivideLayer[] parents)
        {
            AddSubtractLayer layerPart = new AddSubtractLayer();

            MultiplyDivideLayer layer0 = Layer.GetMultiplyDivideLayerWithoutOne(parents[0], pair[0]);
            if (additions.Contains(parents[0])) layerPart.Additions.Add(layer0);
            else layerPart.Subtractions.Add(layer0);

            MultiplyDivideLayer layer1 = Layer.GetMultiplyDivideLayerWithoutOne(parents[1], pair[1]);
            if (additions.Contains(parents[1])) layerPart.Additions.Add(layer1);
            else layerPart.Subtractions.Add(layer1);

            return layerPart;
        }
        private void CombinePair(ILayer[] pair, List<MultiplyDivideLayer> all)
        {
            MultiplyDivideLayer[] parents = new MultiplyDivideLayer[2];
            List<ILayer> tmp = new List<ILayer>(all);
            parents[0] = (MultiplyDivideLayer)Layer.FindParentLayer(pair[0], tmp);
            parents[1] = (MultiplyDivideLayer)Layer.FindParentLayer(pair[1], tmp);

            MultiplyDivideLayer newLayer = new MultiplyDivideLayer();
            newLayer.Factors.Add(CreateNonCombinedLayerPart(pair, parents));
            newLayer.Factors.Add(pair[0]);
            additions.Add(newLayer);

            RemoveFromLayers(parents[0]);
            RemoveFromLayers(parents[1]);
        }
        private MultiplyDivideLayer FindParentLayer(ILayer child, List<MultiplyDivideLayer> possibleParents)
        {
            foreach(MultiplyDivideLayer layer in possibleParents)
            {
                if (layer.Factors.Contains(child)) return layer;
            }
            return null;
        }
        private void RemoveFromLayers(ILayer layer)
        {
            additions.Remove(layer);
            subtractions.Remove(layer);
        }

        public void Simplify()
        {
            SimplifyChildren();
            RemoveZeros();

            for (int i = 0; i < additions.Count; i++)
            {
                if (additions[i] is AddSubtractLayer)
                {
                    AddSubtractLayer layer = (AddSubtractLayer)additions[i];
                    additions.AddRange(layer.additions);
                    subtractions.AddRange(layer.subtractions);
                    additions.Remove(layer);
                    i--;
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
                    i--;
                }
            }
        }
        public void RemoveZeros()
        {
            for (int i = 0; i < additions.Count; i++)
            {
                if (additions[i].Calculate(null) == 0)
                {
                    additions.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < subtractions.Count; i++)
            {
                if (subtractions[i].Calculate(null) == 1)
                {
                    subtractions.RemoveAt(i);
                    i--;
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
