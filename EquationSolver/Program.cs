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
            Console.ReadLine();
        }

        private static string GetInputString()
        {
            Console.Write("Term: ");
            return Console.ReadLine();
        }
    }
}
