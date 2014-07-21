using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    public class StringToLayersParser
    {
        string text = "";
        ILayer topLayer;

        public StringToLayersParser(string text)
        {
            this.text = text;
        }

        public void Parse()
        {
            StringtoElementsParser toElementsParser = new StringtoElementsParser(text);
            toElementsParser.Parse();

            ElementsToLayersParser toLayersParser = new ElementsToLayersParser(toElementsParser.Elements);
            toLayersParser.Parse();

            topLayer = toLayersParser.TopLayer;
        }

        public ILayer TopLayer
        {
            get { return topLayer; }
        }
    }
}
