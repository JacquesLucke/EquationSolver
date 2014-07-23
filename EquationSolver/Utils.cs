using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    public class Utils
    {
        public static void GetFractionFromDecimal(double number, out int numerator, out int denominator)
        {
            const double error = 0.00000001;
            numerator = 1;
            denominator = 2;

            double fractionPart = number - Math.Floor(number);

            while (Math.Abs(numerator / (double)denominator - fractionPart) > error)
            {
                if (numerator / (double)denominator < fractionPart) numerator++;
                else denominator++;
            }

            numerator += (int)Math.Floor(number) * denominator;
        }
    }
}
