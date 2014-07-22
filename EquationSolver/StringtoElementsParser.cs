using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    public class StringtoElementsParser
    {
        string original = "";
        List<IElement> elements;
        Dictionary<string, Type> stringToElementDictionary;
        Dictionary<char, Type> charToElementDictionary;

        public StringtoElementsParser(string text)
        {
            original = text;
            SetupParseDictionary();
        }
        private void SetupParseDictionary()
        {
            stringToElementDictionary = new Dictionary<string, Type>();
            stringToElementDictionary.Add("sqrt", typeof(SqrtElement));
            stringToElementDictionary.Add("root", typeof(RootElement));
            stringToElementDictionary.Add("log", typeof(LogElement));
            stringToElementDictionary.Add("ln", typeof(LnElement));
            stringToElementDictionary.Add("lb", typeof(LbElement));
            stringToElementDictionary.Add("PI", typeof(PiElement));
            stringToElementDictionary.Add("exp", typeof(ExpElement));

            charToElementDictionary = new Dictionary<char, Type>();
            charToElementDictionary.Add('+', typeof(PlusElement));
            charToElementDictionary.Add('-', typeof(MinusElement));
            charToElementDictionary.Add('*', typeof(MultiplyElement));
            charToElementDictionary.Add('/', typeof(DivideElement));
            charToElementDictionary.Add('^', typeof(PowerElement));
            charToElementDictionary.Add('(', typeof(OpenBracketElement));
            charToElementDictionary.Add(')', typeof(CloseBracketElement));
            charToElementDictionary.Add('_', typeof(UnderscoreElement));
            charToElementDictionary.Add('E', typeof(EElement));
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
            elements = GetElementsFromString(original);
            Cleanup();
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
            if (text == "") text = "0";
            text = text.Replace('.', ',');
            text = text.Replace(" ", "");
            while (true)
            {
                string newText = text.Replace("++", "+");
                newText = newText.Replace("--", "+");
                newText = newText.Replace("+-", "-");
                newText = newText.Replace("-+", "-");
                if (newText == text) break;
                text = newText;
            }
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
            foreach (KeyValuePair<string, Type> pair in stringToElementDictionary)
            {
                if (text.Length < pair.Key.Length) continue;
                if (text.Substring(0, pair.Key.Length) == pair.Key)
                {
                    text = text.Substring(pair.Key.Length);
                    return (IElement)Activator.CreateInstance(pair.Value);
                }
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
            if (Char.IsLetter(text[0]))
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
        private void Cleanup()
        {
            AddMissingBracketsInTheEnd();
            AddMultiplyElements();
        }
        private void AddMissingBracketsInTheEnd()
        {
            int closeBracketsMissing = 0;
            foreach (IElement element in elements)
            {
                if (element is OpenBracketElement) closeBracketsMissing++;
                if (element is CloseBracketElement) closeBracketsMissing--;
            }
            for (int i = 0; i < closeBracketsMissing; i++)
            {
                elements.Add(new CloseBracketElement());
            }
        }
        private void AddMultiplyElements()
        {
            for(int i = 1; i < elements.Count; i++)
            {
                IElement a = elements[i - 1];
                IElement b = elements[i];

                if (a is NumberElement && b is VariableElement) elements.Insert(i, new MultiplyElement());
                if (a is VariableElement && b is VariableElement) elements.Insert(i, new MultiplyElement());
                if (a is NumberElement && b is OpenBracketElement) elements.Insert(i, new MultiplyElement());
                if (a is VariableElement && b is OpenBracketElement) elements.Insert(i, new MultiplyElement());
            }
        }
    }

    public class ParseStringException : Exception
    {
    }
}
