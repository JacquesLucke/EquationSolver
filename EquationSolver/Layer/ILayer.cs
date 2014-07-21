using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    public interface ILayer
    {
        double Calculate(Dictionary<char, double> variableToNumberDictionary);
    }
}
