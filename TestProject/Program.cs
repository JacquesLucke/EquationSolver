using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EquationSolver;

namespace TestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            List<KeyValuePair<string, double>> testEquations = new List<KeyValuePair<string, double>>();

            testEquations.Add(new KeyValuePair<string, double>("x+3=10", 7));
            testEquations.Add(new KeyValuePair<string, double>("10+x=20", 10));
            testEquations.Add(new KeyValuePair<string, double>("2+3=x+2", 3));
            testEquations.Add(new KeyValuePair<string, double>("19+x+3=22", 0));
            testEquations.Add(new KeyValuePair<string, double>("88-3+2=x+1", 86));

            testEquations.Add(new KeyValuePair<string, double>("5x+2*3=11", 1));
            testEquations.Add(new KeyValuePair<string, double>("(-3)*2+8=2x", 1));
            testEquations.Add(new KeyValuePair<string, double>("8x+2*4=2x", -4/(double)3));
            testEquations.Add(new KeyValuePair<string, double>("8*2+10x=8x-2", -9));
            testEquations.Add(new KeyValuePair<string, double>("6/(3x)=10", 1/(double)5));

            testEquations.Add(new KeyValuePair<string, double>("2x=14", 7));
            testEquations.Add(new KeyValuePair<string, double>("8-3x=-1", 3));
            testEquations.Add(new KeyValuePair<string, double>("17-4x=1-12x", -2));
            testEquations.Add(new KeyValuePair<string, double>("3(x-2)=5(x-4)", 7));
            testEquations.Add(new KeyValuePair<string, double>("25-(17-2x)=2+(8x-6)", 2));

            bool failed = false;
            foreach(KeyValuePair<string, double> pair in testEquations)
                if (!TestEquation(pair.Key, pair.Value)) failed = true;

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;

            if (failed) 
                Console.WriteLine("test failed");
            else 
                Console.WriteLine("test ok");

            Console.ReadKey();
        }

        static bool TestEquation(string equationString, double answer)
        {
            Equation e = Equation.FromString(equationString);
            e.RearrangeToVariable('x');
            if (Utils.GetOptimizedTextFromNumber(e.Terms[1].Calculate()) == Utils.GetOptimizedTextFromNumber(answer))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("correct   " + equationString + "   x = " + e.Terms[1].ToString());
                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("not correct    " + equationString + "  answer: x = " + e.Terms[1].ToString() + "   correct: x = " + Utils.GetOptimizedTextFromNumber(answer));
                return false;
            }
        }
    }
}
