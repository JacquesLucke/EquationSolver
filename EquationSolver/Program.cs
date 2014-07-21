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
                StringToLayersParser parser = new StringToLayersParser(text);
                parser.Parse();
                Console.WriteLine(parser.TopLayer.Calculate(null));
            }
            catch (ParseStringException e)
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
    }
}
