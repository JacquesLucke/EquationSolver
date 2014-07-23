using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    public delegate void Modification(Term term);
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
        public void Invert()
        {
            for (int i = 0; i < 2; i++)
                terms[i].Invert();
        }

        public void RearrangeToVariable(char variable)
        {
            while (terms[0].ToString() != Convert.ToString(variable) || terms[0].ToString() == "")
            {
                if (terms[0].TopLayer is AddSubtractLayer) ((AddSubtractLayer)terms[0].TopLayer).MultiplyChildrenOut();
                Simplify();
                while (DoSuggestedModification(variable))
                { }

                if (terms[0].TopLayer is AddSubtractLayer) ((AddSubtractLayer)terms[0].TopLayer).CombineMultiplyDivideLayers();
                Simplify();
                while (DoSuggestedModification(variable))
                { }
            }
        }
        public bool DoSuggestedModification(char variable)
        {
            Modification modification;
            Term term;
            SuggestNextModification(out modification, out term, variable);
            if (modification != null && term != null)
            {
                DoModification(modification, term);
                return true;
            }
            return false;
        }
        public void DoModification(Modification modification, Term term)
        {
            modification(term);
            Simplify();
        }
        public void SuggestNextModification(out Modification modification, out Term changeTerm, char variable)
        {
            modification = null;
            changeTerm = null;

            for (int i = 0; i < 2; i++)
            {
                Term term = terms[i];
                if (term.TopLayer is AddSubtractLayer)
                {
                    AddSubtractLayer layer = (AddSubtractLayer)term.TopLayer;
                    foreach (ILayer l in layer.Additions)
                    {
                        if (l.GetVariables().Contains(variable) ^ i == 0)
                        {
                            modification = Subtract;
                            changeTerm = new Term(l);
                            return;
                        }
                    }
                    foreach (ILayer l in layer.Subtractions)
                    {
                        if (l.GetVariables().Contains(variable) ^ i == 0)
                        {
                            modification = Add;
                            changeTerm = new Term(l);
                            return;
                        }
                    }
                }
            }

            for (int i = 0; i < 2; i++)
            {
                Term term = terms[i];
                if (term.TopLayer is MultiplyDivideLayer)
                {
                    MultiplyDivideLayer layer = (MultiplyDivideLayer)term.TopLayer;
                    foreach (ILayer l in layer.Factors)
                    {
                        if (l.GetVariables().Contains(variable) ^ i == 0)
                        {
                            modification = Divide;
                            changeTerm = new Term(l);
                            return;
                        }
                    }
                    foreach (ILayer l in layer.Divisors)
                    {
                        if (l.GetVariables().Contains(variable) ^ i == 0)
                        {
                            modification = Multiply;
                            changeTerm = new Term(l);
                            return;
                        }
                    }
                }
            }
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
