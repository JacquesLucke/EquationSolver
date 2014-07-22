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
            if (elements.Count == 1)
            {
                if (elements[0] is NumberElement) return typeof(NumberLayer);
                if (elements[0] is VariableElement) return typeof(VariableLayer);
            }

            bool containsPlusOrMinus = false;
            bool containsMultiplyOrDivide = false;
            int deepness = 0;
            foreach (IElement element in elements)
            {
                if (element is OpenBracketElement) deepness++;
                if (element is CloseBracketElement) deepness--;
                if (deepness == 0)
                {
                    if (element is PlusElement || element is MinusElement) containsPlusOrMinus = true;
                    if (element is MultiplyElement || element is DivideElement) containsMultiplyOrDivide = true;
                }
            }

            if (containsPlusOrMinus) return typeof(AddSubtractLayer);
            if (containsMultiplyOrDivide) return typeof(MultiplyDivideLayer);

            if (elements.Count > 0)
            {
                if (elements[0] is SqrtElement) return typeof(RootLayer);
                if (elements[0] is RootElement) return typeof(RootLayer);
            }

            bool containsPowerSymbol = false;
            deepness = 0;
            foreach (IElement element in elements)
            {
                if (element is OpenBracketElement) deepness++;
                if (element is CloseBracketElement) deepness--;
                if (deepness == 0)
                {
                    if (element is PowerElement) containsPowerSymbol = true;
                }
            }
            if (containsPowerSymbol) return typeof(PowerLayer);

            throw new CouldNotFindTopLevelLayerType();
        }

        private NumberLayer ParseNumberLayerFromElement()
        {
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
            List<IElement> els = new List<IElement>(elements);

            while (els.Count > 0)
                AddFirstToLayerAndDeleteFromList(layer, els);

            return layer;
        }
        private RootLayer ParseRootLayerFromElements()
        {
            RootLayer layer = new RootLayer();
            if (elements[0] is SqrtElement)
            {
                layer.NthRoot = new NumberLayer(2);

                elements.RemoveAt(0);
                ElementsToLayersParser parser = new ElementsToLayersParser(new List<IElement>(elements));
                parser.Parse();
                layer.BaseOfRoot = parser.TopLayer;
            }
            if (elements[0] is RootElement)
            {
                elements.RemoveAt(0);
                int indexOfSecondUnderscore = -1;
                for (int i = 1; i < elements.Count; i++)
                {
                    if (elements[i] is UnderscoreElement)
                    {
                        indexOfSecondUnderscore = i;
                        break;
                    }
                }
                if (indexOfSecondUnderscore == -1 || !(elements[0] is UnderscoreElement)) throw new MissingUnderscoreException();
                ElementsToLayersParser parser = new ElementsToLayersParser(elements.GetRange(1, indexOfSecondUnderscore - 1));
                parser.Parse();
                layer.NthRoot = parser.TopLayer;
                elements.RemoveRange(0, indexOfSecondUnderscore + 1);
                parser = new ElementsToLayersParser(new List<IElement>(elements));
                parser.Parse();
                layer.BaseOfRoot = parser.TopLayer;
            }
            return layer;
        }
        private PowerLayer ParsePowerLayerFromElements()
        {
            int indexOfPowerSymbol = -1;
            int deepness = 0;
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i] is OpenBracketElement) deepness++;
                if (elements[i] is CloseBracketElement) deepness--;
                if (elements[i] is PowerElement && deepness == 0)
                {
                    indexOfPowerSymbol = i;
                }
            }

            PowerLayer layer = new PowerLayer();

            ElementsToLayersParser parser = new ElementsToLayersParser(elements.GetRange(0, indexOfPowerSymbol));
            parser.Parse();
            layer.BaseOfPower = parser.TopLayer;

            parser = new ElementsToLayersParser(elements.GetRange(indexOfPowerSymbol + 1, elements.Count - indexOfPowerSymbol - 1));
            parser.Parse();
            layer.Exponent = parser.TopLayer;

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

        // these methods take care of brackets
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
