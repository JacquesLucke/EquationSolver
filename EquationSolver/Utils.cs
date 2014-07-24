using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    public class Utils
    {
        const double error = 0.00000001;

        public static string GetOptimizedTextFromNumber(double number)
        {
            if (number == Math.E) return "E";
            if (number == Math.PI) return "PI";

            double factor = number / Math.E;
            if (Math.Abs(factor - Math.Round(factor)) < error) return Math.Round(factor) + "E";

            factor = number / Math.PI;
            if (Math.Abs(factor - Math.Round(factor)) < error) return Math.Round(factor) + "PI";

            double divisor = Math.E / number;
            if (Math.Abs(divisor - Math.Round(divisor)) < error) return "E/" + Math.Round(divisor);

            divisor = Math.PI / number;
            if (Math.Abs(divisor - Math.Round(divisor)) < error) return "PI/" + Math.Round(divisor);

            string fractionString = GetFractionString(number);
            string normalString = Convert.ToString(number);

            if (fractionString.Length > normalString.Length || fractionString.Length > 6) return normalString;
            else return fractionString;
        }
        private static string GetFractionString(double number)
        {
            int numerator, denominator;
            Utils.GetFractionFromDecimal(number, out numerator, out denominator);
            return numerator + "/" + denominator;
        }
        public static void GetFractionFromDecimal(double number, out int numerator, out int denominator)
        {
            numerator = 0;
            denominator = 1;

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
