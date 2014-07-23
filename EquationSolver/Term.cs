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

        public void Simplify()
        {
            topLayer.Simplify();
            topLayer = Layer.GetBetterChild(topLayer);
        }
        public void CalculateNonVariableLayers()
        {
            topLayer.CalculateNonVariableLayers();
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
            if (topLayer is AddSubtractLayer) ((AddSubtractLayer)topLayer).Additions.Add(addition.TopLayer);
            else
            {
                AddSubtractLayer newTopLayer = new AddSubtractLayer();
                newTopLayer.Additions.Add(topLayer);
                newTopLayer.Additions.Add(addition.TopLayer);

                topLayer = Layer.GetBetterChild(newTopLayer);
            }
        }
        public void Subtract(Term subtraction)
        {
            if (topLayer is AddSubtractLayer) ((AddSubtractLayer)topLayer).Subtractions.Add(subtraction.TopLayer);
            else
            {
                AddSubtractLayer newTopLayer = new AddSubtractLayer();
                newTopLayer.Additions.Add(topLayer);
                newTopLayer.Subtractions.Add(subtraction.TopLayer);

                topLayer = Layer.GetBetterChild(newTopLayer);
            }
        }
        public void Multiply(Term factor)
        {
            if (topLayer is MultiplyDivideLayer) ((MultiplyDivideLayer)topLayer).Factors.Add(factor.TopLayer);
            else
            {
                MultiplyDivideLayer newTopLayer = new MultiplyDivideLayer();
                newTopLayer.Factors.Add(topLayer);
                newTopLayer.Factors.Add(factor.TopLayer);

                topLayer = Layer.GetBetterChild(newTopLayer);
            }
        }
        public void Divide(Term divisor)
        {
            if (topLayer is MultiplyDivideLayer) ((MultiplyDivideLayer)topLayer).Divisors.Add(divisor.TopLayer);
            else
            {
                MultiplyDivideLayer newTopLayer = new MultiplyDivideLayer();
                newTopLayer.Factors.Add(topLayer);
                newTopLayer.Divisors.Add(divisor.TopLayer);

                topLayer = Layer.GetBetterChild(newTopLayer);
            }
        }
        public void Invert()
        {
            if(topLayer is AddSubtractLayer)
            {
                AddSubtractLayer layer = (AddSubtractLayer)topLayer;
                List<ILayer> additions = new List<ILayer>(layer.Additions);
                List<ILayer> subtractions = new List<ILayer>(layer.Subtractions);
                layer.Additions.Clear();
                layer.Additions.AddRange(subtractions);
                layer.Subtractions.Clear();
                layer.Subtractions.AddRange(additions);
            }
            else if(topLayer is MultiplyDivideLayer)
            {
                ((MultiplyDivideLayer)topLayer).Factors.Add(new NumberLayer(-1));
            }
            else
            {
                MultiplyDivideLayer newTopLayer = new MultiplyDivideLayer();
                newTopLayer.Factors.Add(topLayer);
                newTopLayer.Factors.Add(new NumberLayer(-1));
                topLayer = newTopLayer;
            }
        }

        public override string ToString()
        {
            return topLayer.ToString();
        }
    }
}
