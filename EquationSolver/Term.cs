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
        public static Term FromElements(List<IElement> elements)
        {
            ElementsToLayersParser parser = new ElementsToLayersParser(elements);
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

        public void Modify(string modification)
        {
            StringtoElementsParser stringParser = new StringtoElementsParser(modification);
            stringParser.Parse();
            List<IElement> elements = stringParser.Elements;
            IElement operation = elements[0];
            elements.RemoveAt(0);
            Term term = Term.FromElements(elements);

            if (operation is PlusElement) Add(term);
            if (operation is MinusElement) Subtract(term);
            if (operation is MultiplyElement) Multiply(term);
            if (operation is DivideElement) Divide(term);
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
        public void Divide(Term divisor)
        {
            MultiplyDivideLayer newTopLayer = new MultiplyDivideLayer();
            newTopLayer.Factors.Add(topLayer);
            newTopLayer.Divisors.Add(divisor.TopLayer);

            topLayer = Layer.GetBetterChild(newTopLayer);
        }

        public override string ToString()
        {
            return topLayer.ToString();
        }
    }
}
