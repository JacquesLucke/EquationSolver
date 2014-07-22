using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    public class Term
    {
        ILayer topLayer;

        public Term(ILayer layer)
        {
            this.topLayer = layer;
        }
        public static Term FromString(string s)
        {
            StringToLayersParser parser = new StringToLayersParser(s);
            parser.Parse();
            return new Term(parser.TopLayer);
        }

        public double Calculate()
        {
            return topLayer.Calculate(null);
        }
        public double Calculate(Dictionary<char, double> variableToNumberDictionary)
        {
            return topLayer.Calculate(variableToNumberDictionary);
        }

        public void StrongSimplification()
        {
            for (int i = 0; i < 5; i++)
            {
                Simplify();
                topLayer.StrongSimplification();
                Simplify();
                topLayer = Layer.GetBetterChild(topLayer);
            }
        }
        public void Simplify()
        {
            topLayer.Simplify();
        }
        public HashSet<char> GetVariables()
        {
            return topLayer.GetVariables();
        }

        public ILayer TopLayer
        {
            get { return topLayer; }
        }

        public void Add(Term addition)
        {
            AddSubtractLayer newTopLayer = new AddSubtractLayer();
            newTopLayer.Additions.Add(topLayer);
            newTopLayer.Additions.Add(addition.TopLayer);

            topLayer = Layer.GetBetterChild(newTopLayer);
        }
        public void Subtract(Term subtraction)
        {
            AddSubtractLayer newTopLayer = new AddSubtractLayer();
            newTopLayer.Additions.Add(topLayer);
            newTopLayer.Subtractions.Add(subtraction.TopLayer);

            topLayer = Layer.GetBetterChild(newTopLayer);
        }
        public void Multiply(Term factor)
        {
            MultiplyDivideLayer newTopLayer = new MultiplyDivideLayer();
            newTopLayer.Factors.Add(topLayer);
            newTopLayer.Factors.Add(factor.TopLayer);

            topLayer = Layer.GetBetterChild(newTopLayer);
        }

        public override string ToString()
        {
            return topLayer.ToString();
        }
    }
}
