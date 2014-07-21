using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            string text = GetInputString();
            List<IElement> elements = new List<IElement>();
            try
            {
                elements = GetElementsFromString(text);
            }
            catch
            {
                Console.WriteLine("Couldn't parse that term");
            }
            Console.ReadLine();
        }

        private static string GetInputString()
        {
            Console.Write("Term: ");
            return Console.ReadLine();
        }
        private static List<IElement> GetElementsFromString(string text)
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
        private static string NormalizeString(string text)
        {
            text = text.Replace('.', ',');
            return text;
        }
        private static IElement FindAndDeleteFirstElement(ref string text)
        {
            // multiple chars elements
            if(Char.IsDigit(text[0]))
            {
                NumberElement element = GetAndDeleteFirstNumberElement(ref text);
                return element;
            }

            // single char elements
            Dictionary<char, Type> charToElementDictionary = new Dictionary<char, Type>();
            charToElementDictionary.Add('+', typeof(PlusElement));
            charToElementDictionary.Add('-', typeof(MinusElement));

            foreach(KeyValuePair<char, Type> pair in charToElementDictionary)
            {
                if (text[0] == pair.Key)
                {
                    text = text.Substring(1);
                    return (IElement)Activator.CreateInstance(pair.Value);
                }
            }
            return null;
        }
        private static NumberElement GetAndDeleteFirstNumberElement(ref string text)
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
