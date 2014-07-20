using System;
using System.Collections.Generic;
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
            List<IElement> elements = GetElementsFromString(text);
            Console.ReadLine();
        }

        private static string GetInputString()
        {
            Console.Write("Term: ");
            return Console.ReadLine();
        }
        private static List<IElement> GetElementsFromString(string text)
        {
            List<IElement> elements = new List<IElement>();
            while (text.Length > 0)
            {
                elements.Add(FindAndDeleteFirstElement(ref text));
            }
            return elements;
        }
        private static IElement FindAndDeleteFirstElement(ref string text)
        {
            // multiple chars elements
            if(Char.IsDigit(text[0]))
            {
                NumberElement element = GetFirstNumberElement(text);
                text = text.Substring(element.ToString().Length);
                return element;
            }
            // single char elements
            if(text[0] == '+')
            {
                PlusElement element = new PlusElement();
                text = text.Substring(1);
                return element;
            }
            return null;
        }
        private static NumberElement GetFirstNumberElement(string text)
        {
            int endIndex = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsDigit(text[i])) endIndex = i;
                else break;
            }
            return new NumberElement(Convert.ToDouble(text.Substring(0, endIndex + 1)));
        }
    }
}
