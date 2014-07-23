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
        public static Random Random = new Random();

        static void Main(string[] args)
        {
            try
            {
                while (true)
                {
                    Equation equation = Equation.FromString(GetInputString("Equation"));
                    equation.RearrangeToVariable('x');
                    Console.WriteLine(equation);
                    Console.WriteLine();
                }
            }
            catch (ParseStringException e)
            {
                Console.WriteLine("Couldn't parse that term");
            }
            catch (CouldNotFindTopLevelLayerType e)
            {
                Console.WriteLine("A problem with parsing this string occured");
            }
            catch (MissingUnderscoreException e)
            {
                Console.WriteLine("There is at least one underscore missing");
            }
            Console.ReadLine();
        }

        private static string GetInputString(string text)
        {
            Console.Write(text + ": ");
            return Console.ReadLine();
        }
    }
}
