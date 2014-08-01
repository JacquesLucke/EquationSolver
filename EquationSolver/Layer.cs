using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    public class Layer
    {
        public static bool Compare(ILayer l1, ILayer l2)
        {
            HashSet<char> var1 = l1.GetVariables();
            HashSet<char> var2 = l2.GetVariables();
            if (!var1.SetEquals(var2)) return false;

            const double epsilon = 0.0001;
            for (int i = 0; i < 3; i++)
            {
                Dictionary<char, double> variableTable = CreateRandomVariableTable(var1);
                if (Math.Abs(l1.Calculate(variableTable) - l2.Calculate(variableTable)) > epsilon) return false;
            }

            return true;
        }
        private static Dictionary<char, double> CreateRandomVariableTable(HashSet<char> variables)
        {
            Dictionary<char, double> variableTable = new Dictionary<char, double>();
            foreach (char c in variables)
            {
                variableTable.Add(c, Program.Random.NextDouble());
            }
            return variableTable;
        }

        public static List<T> GetAllOfType<T>(List<ILayer> layers)
        {
            List<T> layersOfType = new List<T>();
            foreach (ILayer l in layers)
            {
                if (l is T) layersOfType.Add((T)l);
            }
            return layersOfType;
        }
        public static void ReplaceLayersWithMultiplyLayers(List<ILayer> layers)
        {
            for (int i = 0; i < layers.Count; i++)
            {
                if (!(layers[i] is MultiplyDivideLayer))
                    layers[i] = ToMultiplyDivideLayer(layers[i]);
            }
        }
        public static MultiplyDivideLayer ToMultiplyDivideLayer(ILayer nLayer)
        {
            MultiplyDivideLayer layer = new MultiplyDivideLayer();
            layer.Factors.Add(nLayer);
            layer.Factors.Add(new NumberLayer(1));
            return layer;
        }
        public static List<ILayer> GetAllFactors(List<MultiplyDivideLayer> layers)
        {
            List<ILayer> subFactors = new List<ILayer>();
            foreach (MultiplyDivideLayer layer in layers)
            {
                subFactors.AddRange(layer.Factors);
            }
            return subFactors;
        }

        public static List<ILayer[]> FindEqualPairs(List<ILayer> layers)
        {
            List<ILayer[]> pairs = new List<ILayer[]>();
            for (int i = 0; i < layers.Count - 1; i++)
            {
                for (int j = i + 1; j < layers.Count; j++)
                {
                    if (Layer.Compare(layers[i], layers[j]))
                    {
                        pairs.Add(new ILayer[] { layers[i], layers[j] });
                    }
                }
            }
            return pairs;
        }
        public static ILayer FindParentLayer(ILayer child, List<ILayer> possibleParents)
        {
            foreach (ILayer layer in possibleParents)
            {
                if (layer is AddSubtractLayer)
                    if (((AddSubtractLayer)layer).Additions.Contains(child) || ((AddSubtractLayer)layer).Subtractions.Contains(child))
                        return layer;
                if (layer is MultiplyDivideLayer)
                    if (((MultiplyDivideLayer)layer).Factors.Contains(child) || ((MultiplyDivideLayer)layer).Divisors.Contains(child))
                        return layer;
                if (layer is LogarithmLayer)
                    if (((LogarithmLayer)layer).BaseOfLogarithm == child || ((LogarithmLayer)layer).Number == child)
                        return layer;
                if (layer is PowerLayer)
                    if (((PowerLayer)layer).BaseOfPower == child || ((PowerLayer)layer).Exponent == child)
                        return layer;
                if (layer is RootLayer)
                    if (((RootLayer)layer).BaseOfRoot == child || ((RootLayer)layer).NthRoot == child)
                        return layer;
            }
            throw new Exception("Layer not found");
        }

        public static MultiplyDivideLayer GetMultiplyDivideLayerWithoutOne(MultiplyDivideLayer layer, ILayer removeLayer)
        {
            MultiplyDivideLayer m = new MultiplyDivideLayer();
            m.Factors = new List<ILayer>(layer.Factors);
            m.Divisors = new List<ILayer>(layer.Divisors);

            m.Factors.Remove(removeLayer);
            m.Factors.Remove(removeLayer);

            return m;
        }

        public static ILayer GetBetterChild(ILayer childNow)
        {
            ILayer newChild = childNow;
            if(!ContainsVariables(childNow))
            {
                newChild = new NumberLayer(childNow.Calculate(null));
            }
            if (childNow is AddSubtractLayer)
            {
                AddSubtractLayer child = (AddSubtractLayer)childNow;
                if (child.Additions.Count + child.Subtractions.Count == 1)
                {
                    if(child.Additions.Count == 1)
                    {
                        newChild = child.Additions[0];
                    }
                }
            }
            if (childNow is MultiplyDivideLayer)
            {
                MultiplyDivideLayer child = (MultiplyDivideLayer)childNow;
                if (child.Factors.Count + child.Divisors.Count == 1)
                {
                    if (child.Factors.Count == 1)
                    {
                        newChild = child.Factors[0];
                    }
                }
            }
            if(childNow is RootLayer)
            {
                RootLayer child = (RootLayer)childNow;
                if(child.BaseOfRoot is PowerLayer)
                {
                    PowerLayer powerLayer = (PowerLayer)child.BaseOfRoot;
                    if (Compare(child.NthRoot, powerLayer.Exponent)) newChild = powerLayer.BaseOfPower;
                    else
                    {
                        MultiplyDivideLayer newNthRoot = new MultiplyDivideLayer();
                        newNthRoot.Factors.Add(child.NthRoot);
                        newNthRoot.Divisors.Add(powerLayer.Exponent);
                        child.NthRoot = newNthRoot;
                        child.BaseOfRoot = powerLayer.BaseOfPower;
                    }
                }
            }
            if(childNow is PowerLayer)
            {
                PowerLayer child = (PowerLayer)childNow;
                if(child.BaseOfPower is RootLayer)
                {
                    RootLayer sqrtLayer = (RootLayer)child.BaseOfPower;
                    if (Compare(child.Exponent, sqrtLayer.NthRoot)) newChild = sqrtLayer.BaseOfRoot;
                    else
                    {
                        MultiplyDivideLayer newExponent = new MultiplyDivideLayer();
                        newExponent.Factors.Add(child.Exponent);
                        newExponent.Divisors.Add(sqrtLayer.NthRoot);
                        child.Exponent = newExponent;
                        child.BaseOfPower = sqrtLayer.BaseOfRoot;
                    }
                }
            }
            return newChild;
        }

        public static ILayer MultiplyOut(MultiplyDivideLayer original)
        {
            AddSubtractLayer multiplyOutLayer = null;
            for (int i = 0; i < original.Factors.Count; i++)
            {
                if(original.Factors[i] is AddSubtractLayer)
                {
                    AddSubtractLayer layer = (AddSubtractLayer)original.Factors[i];
                    if (layer.Additions.Count + layer.Subtractions.Count > 1)
                    {
                        multiplyOutLayer = layer;
                        break;
                    }
                }
            }

            if (multiplyOutLayer != null)
            {
                MultiplyDivideLayer m = new MultiplyDivideLayer();
                m.Factors.AddRange(original.Factors);
                m.Divisors.AddRange(original.Divisors);
                m.Factors.Remove(multiplyOutLayer);
                m.Divisors.Remove(multiplyOutLayer);

                AddSubtractLayer newLayer = new AddSubtractLayer();
                foreach(ILayer layer in multiplyOutLayer.Additions)
                {
                    MultiplyDivideLayer part = new MultiplyDivideLayer();
                    part.Factors.Add(layer);
                    part.Factors.AddRange(m.Factors);
                    part.Divisors.AddRange(m.Divisors);
                    newLayer.Additions.Add(part);
                }
                foreach (ILayer layer in multiplyOutLayer.Subtractions)
                {
                    MultiplyDivideLayer part = new MultiplyDivideLayer();
                    part.Factors.Add(layer);
                    part.Factors.AddRange(m.Factors);
                    part.Divisors.AddRange(m.Divisors);
                    newLayer.Subtractions.Add(part);
                }
                return newLayer;
            }
            else return original;
        }

        public static bool ContainsVariables(ILayer layer)
        {
            return layer.GetVariables().Count != 0;
        }

        public static bool IsOnlySubtraction(ILayer layer)
        {
            if(layer is AddSubtractLayer)
            {
                AddSubtractLayer l = (AddSubtractLayer)layer;
                if (l.Additions.Count == 0 && l.Subtractions.Count == 1)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsOnlyDivision(ILayer layer)
        {
            if (layer is MultiplyDivideLayer)
            {
                MultiplyDivideLayer l = (MultiplyDivideLayer)layer;
                if (l.Factors.Count == 0 && l.Divisors.Count == 1)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsConstantLayer(ILayer layer, char variable)
        {
            return GetDegreeOfLayer(layer, variable) == 0;
        }
        public static bool IsLinearLayer(ILayer layer, char variable)
        {
            return GetDegreeOfLayer(layer, variable) == 1;
        }
        public static bool IsQuadraticLayer(ILayer layer, char variable)
        {
            return GetDegreeOfLayer(layer, variable) == 2;
        }
        public static int GetDegreeOfLayer(ILayer layer, char variable)
        {
            HashSet<char> variables = layer.GetVariables();
            Dictionary<char, double> variableTable = CreateRandomVariableTable(variables);
            variableTable[variable] = 50000;
            if (Double.IsNaN(layer.Calculate(variableTable))) return -1;
            variableTable[variable] = -50000;
            if (Double.IsNaN(layer.Calculate(variableTable))) return -1;

            double[] functionValues = GetFunctionValues(layer, variable, 130, 6, 1);
            for(int i = 0; i < 4; i++)
            {
                if (AreValuesEqual(functionValues)) return i;
                functionValues = GetDifferences(functionValues);
            }
            return -1;
        }
        private static double[] GetFunctionValues(ILayer layer, char variable, double begin, int stepAmount, double stepSize)
        {
            double[] functionValues = new double[stepAmount];
            HashSet<char> variables = layer.GetVariables();
            Dictionary<char, double> variableTable = CreateRandomVariableTable(variables);
            for (int i = 0; i < functionValues.Length; i++)
            {
                variableTable[variable] = begin + i * stepSize;
                functionValues[i] = layer.Calculate(variableTable);
            }
            return functionValues;
        }
        private static double[] GetDifferences(double[] functionValues)
        {
            double[] differences = new double[functionValues.Length - 1];
            for (int i = 0; i < differences.Length; i++)
            {
                differences[i] = functionValues[i + 1] - functionValues[i];
            }
            return differences;
        }
        private static bool AreValuesEqual(double[] functionValues)
        {
            double error = 0.0000001;
            for (int i = 1; i < functionValues.Length; i++)
            {
                if (Math.Abs(functionValues[i] - functionValues[i - 1]) > error) return false;
            }
            return true;
        }
    }
}
