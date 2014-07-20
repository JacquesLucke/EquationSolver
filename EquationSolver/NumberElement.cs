using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    public class NumberElement : IElement
    {
        private double number = 0;
        public NumberElement(double number)
        {
            this.number = number;
        }

        public double Number
        {
            get { return number; }
        }

        public override string ToString()
        {
            return "" + number;
        }
    }
}
