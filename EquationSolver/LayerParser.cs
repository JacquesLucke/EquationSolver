using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    public class LayerParser
    {
        List<IElement> elements;
        ILayer topLayer;

        public LayerParser(List<IElement> elements)
        {
            this.elements = elements;
        }
        public ILayer TopLayer
        {
            get { return topLayer; }
        }
        public void Parse()
        {
            Type topLayerType = GetTopLayerType();

            if (topLayerType == typeof(NumberLayer))
            {
                topLayer = new NumberLayer(((NumberElement)elements[0]).Number);
            }
            if(topLayerType == typeof(VariableLayer))
            {
                topLayer = new VariableLayer(((VariableElement)elements[0]).Name);
            }
            if(topLayerType == typeof(AddSubtractLayer))
            {
                topLayer = new AddSubtractLayer();
                List<IElement> els = new List<IElement>(elements);

                while(els.Count > 0)
                {
                    AddToLayerAndDeleteFromList((AddSubtractLayer)topLayer, els);
                }
            }
        }

        private Type GetTopLayerType()
        {
            if (elements.Count == 1)
            {
                if (elements[0] is NumberElement) return typeof(NumberLayer);
                if (elements[0] is VariableElement) return typeof(VariableLayer);
            }

            bool containsPlusOrMinus = false;
            bool containsMultiplyOrDivide = false;
            foreach (IElement element in elements)
            {
                if (element is PlusElement || element is MinusElement) containsPlusOrMinus = true;
                if (element is MultiplyElement || element is DivideElement) containsMultiplyOrDivide = true;
            }

            if (containsPlusOrMinus) return typeof(AddSubtractLayer);
            if (containsMultiplyOrDivide) return typeof(MultiplyDivideLayer);

            throw new CouldNotFindTopLevelLayerType();
        }

        private void AddToLayerAndDeleteFromList(AddSubtractLayer layer, List<IElement> els)
        {
            bool isAddition = true;
            if (els[0] is MinusElement) isAddition = false;
            if (els[0] is PlusElement || els[0] is MinusElement) els.RemoveAt(0);

            int length = 0;
            foreach(IElement element in els)
            {
                if (element is PlusElement || element is MinusElement) break;
                length++;
            }

            LayerParser parser = new LayerParser(els.GetRange(0, length));
            parser.Parse();
            if (isAddition) layer.Additions.Add(parser.TopLayer);
            if (!isAddition) layer.Subtractions.Add(parser.TopLayer);

            els.RemoveRange(0, length);
        }
    }
}
