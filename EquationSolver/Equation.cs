using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    public delegate bool TermChange(char variable);
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
            parts[0] = new List<IElement>();
            parts[1] = new List<IElement>();

            for(int  i = 0; i<elements.Count; i++)
            {
                if(elements[i] is EqualElement)
                {

                    parts[0] = elements.GetRange(0, i);
                    parts[1] = elements.GetRange(i + 1, elements.Count - i - 1);
                }
            }
        }

        public Term[] Terms
        {
            get { return terms; }
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
        public void Reciproke()
        {
            for (int i = 0; i < 2; i++)
                terms[i].Reciproke();
        }
        public void Root(Term nthRoot)
        {
            for (int i = 0; i < 2; i++)
                terms[i].Root(nthRoot);
        }
        public void Power(Term exponent)
        {
            for (int i = 0; i < 2; i++)
                terms[i].Power(exponent);
        }

        public void RearrangeToVariable(char variable)
        {
            while (!IsReady(variable))
            {
                if (terms[0].TopLayer is AddSubtractLayer) ((AddSubtractLayer)terms[0].TopLayer).MultiplyChildrenOut();
                Simplify();
                while (DoSuggestedModification(variable))
                { Simplify(); }

                if (terms[0].TopLayer is AddSubtractLayer) ((AddSubtractLayer)terms[0].TopLayer).CombineMultiplyDivideLayers();
                Simplify();
                while (DoSuggestedModification(variable))
                { Simplify(); }
            }
            Simplify();
        }
        private bool IsReady(char variable)
        {
            if (!terms[0].GetVariables().Contains(variable) && !terms[1].GetVariables().Contains(variable)) return true;
            if (!Layer.ContainsVariables(terms[0].TopLayer) && !Layer.ContainsVariables(terms[1].TopLayer) && !Layer.Compare(terms[0].TopLayer, terms[1].TopLayer))
                return true;
            if (terms[0].ToString() == Convert.ToString(variable)) return true;
            if (terms[0].ToString() == "") return true;
            if (terms[0].ToString() == terms[1].ToString()) return true;

            return false;
        }

        public bool DoSuggestedModification(char variable)
        {
            List<TermChange> possibleChanges = new List<TermChange>();
            possibleChanges.Add(MoveVariable);
            possibleChanges.Add(MoveNumber);
            possibleChanges.Add(MoveAdditionsAndSubtractions);
            possibleChanges.Add(MoveFactorsAndDivisors);
            possibleChanges.Add(InvertIfOnlySubtraction);
            possibleChanges.Add(ReciprokeIfOnlyDivision);
            possibleChanges.Add(RootIfOnlyPower);
            possibleChanges.Add(PowerIfOnlyRoot);

            foreach(TermChange change in possibleChanges)
            {
                if (change(variable)) return true;
            }
            return false;
        }
        private bool MoveVariable(char variable)
        {
            if(terms[1].TopLayer is VariableLayer)
            {
                VariableLayer l = (VariableLayer)terms[1].TopLayer;
                if(l.Name == variable)
                {
                    Subtract(Term.FromString(Convert.ToString(variable)));
                    return true;
                }
            }
            return false;
        }
        private bool MoveNumber(char variable)
        {
            if (terms[0].TopLayer is NumberLayer)
            {
                if (terms[0].TopLayer.Calculate(null) != 0)
                {
                    Subtract(new Term(terms[0].TopLayer));
                    return true;
                }
            }
            return false;
        }
        private bool MoveAdditionsAndSubtractions(char variable)
        {
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
                            Subtract(new Term(l));
                            return true;
                        }
                    }
                    foreach (ILayer l in layer.Subtractions)
                    {
                        if (l.GetVariables().Contains(variable) ^ i == 0)
                        {
                            Add(new Term(l));
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        private bool MoveFactorsAndDivisors(char variable)
        {
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
                            Divide(new Term(l));
                            return true;
                        }
                    }
                    foreach (ILayer l in layer.Divisors)
                    {
                        if (l.GetVariables().Contains(variable) ^ i == 0)
                        {
                            Multiply(new Term(l));
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        private bool InvertIfOnlySubtraction(char variable)
        {
            if (Layer.IsOnlySubtraction(terms[0].TopLayer))
            {
                Invert();
                return true;
            }
            return false;
        }
        private bool ReciprokeIfOnlyDivision(char variable)
        {
            if (Layer.IsOnlyDivision(terms[0].TopLayer))
            {
                Reciproke();
                return true;
            }
            return false;
        }
        private bool RootIfOnlyPower(char variable)
        {
            if(terms[0].TopLayer is PowerLayer)
            {
                PowerLayer powerLayer = (PowerLayer)terms[0].TopLayer;
                if(powerLayer.BaseOfPower.GetVariables().Contains(variable))
                {
                    Root(new Term(powerLayer.Exponent));
                    return true;
                }
            }
            return false;
        }
        private bool PowerIfOnlyRoot(char variable)
        {
            if (terms[0].TopLayer is RootLayer)
            {
                RootLayer rootLayer = (RootLayer)terms[0].TopLayer;
                if (rootLayer.BaseOfRoot.GetVariables().Contains(variable))
                {
                    Power(new Term(rootLayer.NthRoot));
                    return true;
                }
            }
            return false;
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
