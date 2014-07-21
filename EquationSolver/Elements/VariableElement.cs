using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    public class VariableElement : IElement
    {
        private char name;
        public VariableElement(char name)
        {
            this.name = name;
        }
        public char Name
        {
            get { return name; }
        }
        public override string ToString()
        {
            return Convert.ToString(name);
        }
    }
}
