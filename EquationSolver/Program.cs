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
                TermParser parser = new TermParser(text);
                parser.Parse();
                elements = parser.Elements;
                parser.TopLayer.Calculate(null);
            }
            catch (ParseStringException e)
            {
                Console.WriteLine("Couldn't parse that term");
            }
            Console.ReadLine();
            Console.ReadLine();
        }

        private static string GetInputString()
        {
            Console.Write("Term: ");
            return Console.ReadLine();
        }
    }
}
