using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    public class TermParser
    {
        string original = "";
        List<IElement> elements;
        Dictionary<char, Type> charToElementDictionary;

        public TermParser(string text)
        {
            original = text;
            SetupParseDictionary();
        }
        private void SetupParseDictionary()
        {
            charToElementDictionary = new Dictionary<char, Type>();
            charToElementDictionary.Add('+', typeof(PlusElement));
            charToElementDictionary.Add('-', typeof(MinusElement));
            charToElementDictionary.Add('*', typeof(MultiplyElement));
            charToElementDictionary.Add('/', typeof(DivideElement));
        }

        public List<IElement> Elements
        {
            get
            {
                return elements;
            }
        }

        public void Parse()
        {
            ParseElements();
        }
        private void ParseElements()
        {
            elements = GetElementsFromString(original);
        }
        private List<IElement> GetElementsFromString(string text)
        {
            text = NormalizeString(text);
            List<IElement> elements = new List<IElement>();
            int oldLength = text.Length;
            while (text.Length > 0)
            {
                elements.Add(FindAndDeleteFirstElement(ref text));
                if (oldLength == text.Length) throw new ParseStringException();
                oldLength = text.Length;
            }
            return elements;
        }
        private string NormalizeString(string text)
        {
            text = text.Replace('.', ',');
            text = text.Replace(" ", "");
            return text;
        }
        private IElement FindAndDeleteFirstElement(ref string text)
        {
            // multiple chars elements
            if (Char.IsDigit(text[0]))
            {
                NumberElement element = GetAndDeleteFirstNumberElement(ref text);
                return element;
            }

            // single char elements
            foreach (KeyValuePair<char, Type> pair in charToElementDictionary)
            {
                if (text[0] == pair.Key)
                {
                    text = text.Substring(1);
                    return (IElement)Activator.CreateInstance(pair.Value);
                }
            }

            // variables
            if(Char.IsLetter(text[0]))
            {
                VariableElement element = new VariableElement(text[0]);
                text = text.Substring(1);
                return element;
            }
            return null;
        }
        private NumberElement GetAndDeleteFirstNumberElement(ref string text)
        {
            int endIndex = 0;
            bool dotInside = false;
            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsDigit(text[i]) || (text[i] == ',' && !dotInside)) endIndex = i;
                else break;
                if (text[i] == ',') dotInside = true;
            }
            NumberElement element = new NumberElement(Convert.ToDouble(text.Substring(0, endIndex + 1)));
            text = text.Substring(endIndex + 1);
            return element;
        }
    }

    public class ParseStringException : Exception
    {
    }
}
