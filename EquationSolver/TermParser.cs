﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquationSolver
{
    public class TermParser
    {
        string original = "";
        List<IElement> elements;
        Dictionary<string, Type> stringToElementDictionary;
        Dictionary<char, Type> charToElementDictionary;
        ILayer topLayer;

        public TermParser(string text)
        {
            original = text;
            SetupParseDictionary();
        }
        private void SetupParseDictionary()
        {
            stringToElementDictionary = new Dictionary<string, Type>();
            stringToElementDictionary.Add("sqrt", typeof(SqrtElement));

            charToElementDictionary = new Dictionary<char, Type>();
            charToElementDictionary.Add('+', typeof(PlusElement));
            charToElementDictionary.Add('-', typeof(MinusElement));
            charToElementDictionary.Add('*', typeof(MultiplyElement));
            charToElementDictionary.Add('/', typeof(DivideElement));
            charToElementDictionary.Add('^', typeof(PowerElement));
        }

        public List<IElement> Elements
        {
            get
            {
                return elements;
            }
        }

        public void Parse()
        {
            ParseElements();
            GenerateLayers();
        }
        private void ParseElements()
        {
            elements = GetElementsFromString(original);
        }
        private List<IElement> GetElementsFromString(string text)
        {
            text = NormalizeString(text);
            List<IElement> elements = new List<IElement>();
            int oldLength = text.Length;
            while (text.Length > 0)
            {
                elements.Add(FindAndDeleteFirstElement(ref text));
                if (oldLength == text.Length) throw new ParseStringException();
                oldLength = text.Length;
            }
            return elements;
        }
        private string NormalizeString(string text)
        {
            text = text.Replace('.', ',');
            text = text.Replace(" ", "");
            return text;
        }
        private IElement FindAndDeleteFirstElement(ref string text)
        {
            // multiple chars elements
            if (Char.IsDigit(text[0]))
            {
                NumberElement element = GetAndDeleteFirstNumberElement(ref text);
                return element;
            }
            foreach (KeyValuePair<string, Type> pair in stringToElementDictionary)
            {
                if (text.Length < pair.Key.Length) continue;
                if (text.Substring(0, pair.Key.Length) == pair.Key)
                {
                    text = text.Substring(pair.Key.Length);
                    return (IElement)Activator.CreateInstance(pair.Value);
                }
            }

            // single char elements
            foreach (KeyValuePair<char, Type> pair in charToElementDictionary)
            {
                if (text[0] == pair.Key)
                {
                    text = text.Substring(1);
                    return (IElement)Activator.CreateInstance(pair.Value);
                }
            }

            // variables
            if(Char.IsLetter(text[0]))
            {
                VariableElement element = new VariableElement(text[0]);
                text = text.Substring(1);
                return element;
            }
            return null;
        }
        private NumberElement GetAndDeleteFirstNumberElement(ref string text)
        {
            int endIndex = 0;
            bool dotInside = false;
            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsDigit(text[i]) || (text[i] == ',' && !dotInside)) endIndex = i;
                else break;
                if (text[i] == ',') dotInside = true;
            }
            NumberElement element = new NumberElement(Convert.ToDouble(text.Substring(0, endIndex + 1)));
            text = text.Substring(endIndex + 1);
            return element;
        }

        public void GenerateLayers()
        {
            List<IElement> elements = new List<IElement>(this.elements);
        }
        private Type GetTopLayerType(List<IElement> elements)
        {
            if(elements.Count == 1)
            {
                if (elements[0] is NumberElement) return typeof(NumberLayer);
                if (elements[0] is VariableElement) return typeof(VariableLayer);
            }

            bool containsPlusOrMinus = false;
            bool containsMultiplyOrDivide = false;
            foreach(IElement element in elements)
            {
                if (element is PlusElement || element is MinusElement) containsPlusOrMinus = true;
                if (element is MultiplyElement || element is DivideElement) containsMultiplyOrDivide = true;
            }

            if (containsPlusOrMinus) return typeof(AddSubtractLayer);
            if (containsMultiplyOrDivide) return typeof(MultiplyDivideLayer);

            throw new CouldNotFindTopLevelLayerType();
        }
    }

    public class ParseStringException : Exception
    {
    }
    public class CouldNotFindTopLevelLayerType : Exception
    {
    }
}