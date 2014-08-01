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
                    Equation e1 = Equation.FromString(GetInputString("Equation 1"));
                    Equation e2 = Equation.FromString(GetInputString("Equation 2"));

                    e1.RearrangeToVariable('x');
                    e2.ReplaceVariableWithLayer('x', e1.Terms[1].TopLayer);
                    e2.RearrangeToVariable('y');
                    e1.ReplaceVariableWithLayer('y', e2.Terms[1].TopLayer);
                    e1.RearrangeToVariable('x');

                    Console.WriteLine("x = " + e1.Terms[1].ToString());
                    Console.WriteLine("y = " + e2.Terms[1].ToString());
                    Console.WriteLine();
                }

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
