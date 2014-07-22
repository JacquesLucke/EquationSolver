using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    public class ElementsToLayersParser
    {
        List<IElement> elements;
        ILayer topLayer;

        public ElementsToLayersParser(List<IElement> elements)
        {
            this.elements = elements;
        }
        public ILayer TopLayer
        {
            get { return topLayer; }
        }
        public void Parse()
        {
            DeleteSurroundingBrackets();

            Type topLayerType = GetTopLayerType();

            if (topLayerType == typeof(NumberLayer))
                topLayer = ParseNumberLayerFromElement();

            if (topLayerType == typeof(VariableLayer))
                topLayer = ParseVariableLayerFromElement();

            if (topLayerType == typeof(AddSubtractLayer))
                topLayer = ParseAddSubtractLayerFromElements();

            if (topLayerType == typeof(MultiplyDivideLayer))
                topLayer = ParseMultiplyDivideLayerFromElements();

            if (topLayerType == typeof(RootLayer))
                topLayer = ParseRootLayerFromElements();

            if (topLayerType == typeof(PowerLayer))
                topLayer = ParsePowerLayerFromElements();

            if (topLayerType == typeof(LogarithmLayer))
                topLayer = ParseLogarithmLayerFromElements();
        }
        private void DeleteSurroundingBrackets()
        {
            int oldLength = 0;
            while (oldLength != elements.Count)
            {
                oldLength = elements.Count;
                if (CheckIfInBrackets())
                {
                    elements.RemoveAt(0);
                    elements.RemoveAt(elements.Count - 1);
                }
            }
        }
        private bool CheckIfInBrackets()
        {
            if (elements.Count < 2) return false;
            int deepness = 0;
            if (elements[0] is OpenBracketElement) deepness = 1;
            else return false;
            for (int i = 1; i < elements.Count - 1; i++)
            {
                if (elements[i] is OpenBracketElement) deepness++;
                if (elements[i] is CloseBracketElement) deepness--;
                if (deepness == 0) return false;
            }
            return true;
        }

        // takes care of brackets
        private Type GetTopLayerType()
        {
            if (ContainsType(elements, typeof(PlusElement), typeof(MinusElement))) return typeof(AddSubtractLayer);

            if (ContainsType(elements, typeof(MultiplyElement), typeof(DivideElement))) return typeof(MultiplyDivideLayer);

            if (elements.Count > 0)
            {
                if (elements[0] is SqrtElement) return typeof(RootLayer);
                if (elements[0] is RootElement) return typeof(RootLayer);

                if (elements[0] is LogElement) return typeof(LogarithmLayer);
                if (elements[0] is LnElement) return typeof(LogarithmLayer);
                if (elements[0] is LbElement) return typeof(LogarithmLayer);
            }

            if (ContainsType(elements, typeof(PowerElement))) return typeof(PowerLayer);

            if(ContainsType(elements, typeof(ExpElement))) return typeof(MultiplyDivideLayer);

            if (elements.Count == 1)
            {
                if (elements[0] is NumberElement) return typeof(NumberLayer);
                if (elements[0] is EElement) return typeof(NumberLayer);
                if (elements[0] is PiElement) return typeof(NumberLayer);
                if (elements[0] is VariableElement) return typeof(VariableLayer);
            }

            throw new CouldNotFindTopLevelLayerType();
        }

        private NumberLayer ParseNumberLayerFromElement()
        {
            if (elements[0] is EElement) return new NumberLayer(Math.E);
            if (elements[0] is PiElement) return new NumberLayer(Math.PI);
            return new NumberLayer(((NumberElement)elements[0]).Number);
        }
        private VariableLayer ParseVariableLayerFromElement()
        {
            return new VariableLayer(((VariableElement)elements[0]).Name); ;
        }
        private AddSubtractLayer ParseAddSubtractLayerFromElements()
        {
            AddSubtractLayer layer = new AddSubtractLayer();
            List<IElement> els = new List<IElement>(elements);

            while (els.Count > 0)
                AddFirstToLayerAndDeleteFromList(layer, els);

            return layer;
        }
        private MultiplyDivideLayer ParseMultiplyDivideLayerFromElements()
        {
            MultiplyDivideLayer layer = new MultiplyDivideLayer();
            if (ContainsType(elements, typeof(MultiplyElement), typeof(DivideElement)))
            {
                List<IElement> els = new List<IElement>(elements);

                while (els.Count > 0)
                    AddFirstToLayerAndDeleteFromList(layer, els);
                return layer;
            }
            if(ContainsType(elements, typeof(ExpElement)))
            {
                ILayer factor, exponent;
                ParseMiddleOperatorType(new List<IElement>(elements), typeof(ExpElement), out factor, out exponent);

                PowerLayer powerLayer = new PowerLayer();
                powerLayer.BaseOfPower = new NumberLayer(10);
                powerLayer.Exponent = exponent;

                layer.Factors.Add(factor);
                layer.Factors.Add(powerLayer);
                return layer;
            }
            return layer;
        }
        private RootLayer ParseRootLayerFromElements()
        {
            RootLayer layer = new RootLayer();
            if (elements[0] is SqrtElement)
            {
                layer.NthRoot = new NumberLayer(2);

                ILayer baseOfRoot;
                ParseBeginWithoutParameterType(new List<IElement>(elements), out baseOfRoot);
                layer.BaseOfRoot = baseOfRoot;
            }
            if (elements[0] is RootElement)
            {
                ILayer nthRoot, baseOfRoot;
                ParseBeginWithParameterType(new List<IElement>(elements), out nthRoot, out baseOfRoot);
                layer.NthRoot = nthRoot;
                layer.BaseOfRoot = baseOfRoot;
            }
            return layer;
        }
        private PowerLayer ParsePowerLayerFromElements()
        {
            PowerLayer layer = new PowerLayer();

            ILayer baseOfPower, exponent;
            ParseMiddleOperatorType(new List<IElement>(elements), typeof(PowerElement), out baseOfPower, out exponent);
            layer.BaseOfPower = baseOfPower;
            layer.Exponent = exponent;

            return layer;
        }
        private LogarithmLayer ParseLogarithmLayerFromElements()
        {
            LogarithmLayer layer = new LogarithmLayer();
            if (elements[1] is UnderscoreElement)
            {
                ILayer baseOfLogarithm, number;
                ParseBeginWithParameterType(new List<IElement>(elements), out baseOfLogarithm, out number);
                layer.BaseOfLogarithm = baseOfLogarithm;
                layer.Number = number;
            }
            else
            {
                if (elements[0] is LogElement) layer.BaseOfLogarithm = new NumberLayer(10);
                if (elements[0] is LnElement) layer.BaseOfLogarithm = new NumberLayer(Math.E);
                if (elements[0] is LbElement) layer.BaseOfLogarithm = new NumberLayer(2);

                ILayer number;
                ParseBeginWithoutParameterType(new List<IElement>(elements), out number);
                layer.Number = number;
            }
            return layer;
        }

        private void AddFirstToLayerAndDeleteFromList(AddSubtractLayer layer, List<IElement> els)
        {
            bool isAddition = true;
            if (els[0] is MinusElement) isAddition = false;
            if (els[0] is PlusElement || els[0] is MinusElement) els.RemoveAt(0);

            int length = GetFirstIndexOrCount(els, typeof(PlusElement), typeof(MinusElement));

            ElementsToLayersParser parser = new ElementsToLayersParser(els.GetRange(0, length));
            parser.Parse();
            if (isAddition) layer.Additions.Add(parser.TopLayer);
            if (!isAddition) layer.Subtractions.Add(parser.TopLayer);

            els.RemoveRange(0, length);
        }
        private void AddFirstToLayerAndDeleteFromList(MultiplyDivideLayer layer, List<IElement> els)
        {
            bool isFactor = true;
            if (els[0] is DivideElement) isFactor = false;
            if (els[0] is MultiplyElement || els[0] is DivideElement) els.RemoveAt(0);

            int length = GetFirstIndexOrCount(els, typeof(MultiplyElement), typeof(DivideElement));

            ElementsToLayersParser parser = new ElementsToLayersParser(els.GetRange(0, length));
            parser.Parse();
            if (isFactor) layer.Factors.Add(parser.TopLayer);
            if (!isFactor) layer.Divisors.Add(parser.TopLayer);

            els.RemoveRange(0, length);
        }

        private void ParseBeginWithParameterType(List<IElement> els, out ILayer parameter, out ILayer layer)
        {
            els.RemoveAt(0);
            int indexOfSecondUnderscore = 1 + GetFirstIndexOrCount(els.GetRange(1, els.Count - 1), typeof(UnderscoreElement));
            if (indexOfSecondUnderscore == els.Count || !(els[0] is UnderscoreElement)) throw new MissingUnderscoreException();

            ElementsToLayersParser parser = new ElementsToLayersParser(els.GetRange(1, indexOfSecondUnderscore - 1));
            parser.Parse();
            parameter = parser.TopLayer;
            els.RemoveRange(0, indexOfSecondUnderscore + 1);

            parser = new ElementsToLayersParser(new List<IElement>(els));
            parser.Parse();
            layer = parser.TopLayer;
        }
        private void ParseBeginWithoutParameterType(List<IElement> els, out ILayer layer)
        {
            els.RemoveAt(0);
            ElementsToLayersParser parser = new ElementsToLayersParser(els);
            parser.Parse();
            layer = parser.TopLayer;
        }
        private void ParseMiddleOperatorType(List<IElement> els, Type operatorType, out ILayer before, out ILayer after)
        {
            int indexOfPowerSymbol = GetFirstIndexOrCount(els, operatorType);

            ElementsToLayersParser parser = new ElementsToLayersParser(els.GetRange(0, indexOfPowerSymbol));
            parser.Parse();
            before = parser.TopLayer;

            parser = new ElementsToLayersParser(els.GetRange(indexOfPowerSymbol + 1, els.Count - indexOfPowerSymbol - 1));
            parser.Parse();
            after = parser.TopLayer;
        }

        // these methods take care of brackets
        private bool ContainsType(List<IElement> els, params Type[] search)
        {
            return GetFirstIndexOrCount(els, search) != els.Count;
        }
        private int GetFirstIndexOrCount(List<IElement> els, params Type[] search)
        {
            int index = els.Count;
            foreach (Type type in search)
            {
                int indexOfElement = GetFirstIndexOfType(els, type);
                if (indexOfElement != -1) index = Math.Min(index, indexOfElement);
            }
            return index;
        }
        private int GetFirstIndexOfType(List<IElement> els, Type type)
        {
            int index = -1;
            int deepness = 0;
            for (int i = 0; i < els.Count; i++)
            {
                if (els[i] is OpenBracketElement) deepness++;
                if (els[i] is CloseBracketElement) deepness--;
                if (els[i].GetType() == type && deepness == 0)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
    }

    public class CouldNotFindTopLevelLayerType : Exception
    {
    }
    public class MissingUnderscoreException : Exception
    {
    }
}
