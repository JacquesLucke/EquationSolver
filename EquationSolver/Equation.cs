using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    public class Equation
    {
        Term[] terms;

        public Equation(Term term1, Term term2)
        {
            terms = new Term[] { term1, term2 };
        }

        public static Equation FromString(string s)
        {
            StringtoElementsParser elementsParser = new StringtoElementsParser(s);
            elementsParser.Parse();
            return Equation.FromElements(elementsParser.Elements);
           
        }
        public static Equation FromElements(List<IElement> elements)
        {
            List<IElement>[] parts;
            SplitElementListAtEqualElement(elements, out parts);

            return new Equation(Term.FromElements(parts[0]), Term.FromElements(parts[1]));
        }
        private static void SplitElementListAtEqualElement(List<IElement> elements, out List<IElement>[] parts)
        {
            parts = new List<IElement>[2];
            for(int  i = 0; i<elements.Count; i++)
            {
                if(elements[i] is EqualElement)
                {

                    parts[0] = elements.GetRange(0, i);
                    parts[1] = elements.GetRange(i + 1, elements.Count - i - 1);
                }
            }
        }

        public void Modify(string modification)
        {
            for (int i = 0; i < 2; i++)
                terms[i].Modify(modification);
        }
        public void Add(Term addition)
        {
            for (int i = 0; i < 2; i++)
                terms[i].Add(addition);
        }
        public void Subtract(Term subtraction)
        {
            for (int i = 0; i < 2; i++)
                terms[i].Subtract(subtraction);
        }
        public void Multiply(Term factor)
        {
            for (int i = 0; i < 2; i++)
                terms[i].Multiply(factor);
        }
        public void Divide(Term divisor)
        {
            for (int i = 0; i < 2; i++)
                terms[i].Divide(divisor);
        }

        public void DoModification()
        {
            for (int i = 0; i < 2; i++)
            {
                Term term = terms[i];
                if (term.TopLayer is AddSubtractLayer)
                {
                    AddSubtractLayer layer = (AddSubtractLayer)term.TopLayer;
                    bool didChange = false;
                    foreach (ILayer l in layer.Additions)
                    {
                        if (Layer.ContainsVariables(l))
                        {
                            if (i == 1) { Subtract(new Term(l)); didChange = true; }
                        }
                        else
                        {
                            if (i == 0) { Subtract(new Term(l)); didChange = true; }
                        }
                    }
                    if (!didChange)
                    {
                        foreach (ILayer l in layer.Subtractions)
                        {
                            if (Layer.ContainsVariables(l))
                            {
                                if (i == 1) Add(new Term(l));
                            }
                            else
                            {
                                if (i == 0) Add(new Term(l));
                            }
                        }
                    }
                }
            }
            Simplify();
        }

        public void Simplify()
        {
            for (int i = 0; i < 2; i++)
                terms[i].Simplify();
        }

        public override string ToString()
        {
            return terms[0].ToString() + " = " + terms[1].ToString();
        }
    }
}
