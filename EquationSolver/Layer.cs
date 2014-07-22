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
            foreach(ILayer l in layers)
            {
                if(l is T) layersOfType.Add((T)l);
            }
            return layersOfType;
        }
        public static void ReplaceNumbersWithMultiplyLayers(List<ILayer> layers)
        {
            for(int i = 0; i < layers.Count; i++)
            {
                if(layers[i] is NumberLayer)
                {
                    layers[i] = ToMultiplyDivideLayer((NumberLayer)layers[i]);
                }
            }
        }
        public static MultiplyDivideLayer ToMultiplyDivideLayer(NumberLayer nLayer)
        {
            MultiplyDivideLayer layer = new MultiplyDivideLayer();
            layer.Factors.Add(nLayer);
            return layer;
        }
        public static List<ILayer> GetAllFactors(List<MultiplyDivideLayer> layers)
        {
            List<ILayer> subFactors = new List<ILayer>();
            foreach(MultiplyDivideLayer layer in layers)
            {
                subFactors.AddRange(layer.Factors);
            }
            return subFactors;
        }
    }
}
