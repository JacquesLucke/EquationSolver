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
                Equation equation = Equation.FromString("3+x-7+x=5+5+x+x+x+x");
                equation.DoModification();
                equation.DoModification();
                equation.DoModification();
                Console.WriteLine(equation);

                while (true)
                {
                    string text = GetInputString("Modification");
                    equation.Modify(text);
                    equation.Simplify();
                    Console.WriteLine(equation);
                    Console.WriteLine();
                }

                //Term term = Term.FromString("100");

                //while (true)
                //{
                //    string text = GetInputString("Modificition");
                //    term.Modify(text);
                //    term.Simplify();
                //    Console.WriteLine("Simplification: " + term.ToString());
                //    HashSet<char> s = term.GetVariables();
                //    if (s.Count == 0) Console.WriteLine("Result: " + term.Calculate());
                //    Console.WriteLine();
                //}
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
