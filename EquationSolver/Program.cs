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
            try
            {
                Term term = Term.FromString(text);
                term.Simplify();
                Console.WriteLine(term.ToString());
                Console.WriteLine(term.Calculate());
                Console.ReadLine();
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

        private static string GetInputString()
        {
            Console.Write("Term: ");
            return Console.ReadLine();
        }
    }
}
