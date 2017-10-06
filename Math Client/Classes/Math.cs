using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace The_Email_Client
{
    public class Term
    {
        public int Coefficient { get; set; }
        public int Power { get; set; }
        public string X { get {
                return Power == 0 ? " " : Power == 1 ? "x" : $"x^{Power}";
            }
        }
        
        public override string ToString()
        {
            string coeff = (Coefficient > 0 ? $"+{Coefficient}" : $"{Coefficient}");
            return (Coefficient == 0 ? "" : $"{coeff}{X}");
        }
        
    }

    public class Equation
    {
        public List<Term> Components { get; set; }
        public List<Term> difComponents { get; set; }
        
        public void GenerateEquation(int Order, int Difficulty)
        {
            Components = new List<Term>();
            difComponents = new List<Term>();
            Random rnd = new Random();
            for (int i = Order-1; i > -1; --i)
            {
                Components.Add(
                    new Term {
                        Coefficient = rnd.Next(-((Difficulty + 1) * 3),(Difficulty+1)*3),
                        Power = i,
                    }
                );
            }
            foreach (Term term in Components)
            {
                if(term.Power != 0)
                difComponents.Add(new Term {
                    Coefficient = term.Coefficient * term.Power,
                    Power = term.Power - 1
                });
                else
                difComponents.Add(new Term {
                    Coefficient = 0,
                    Power = 0
                });
            }
        }

        public string ReturnEquationString()
        {
            string Equationstring = "";
            foreach (Term term in Components) {
                Equationstring += $"{term} ";
            }
            return Equationstring;
        }

        public string ReturnDifString()
        {
            string DifString = "";
            foreach (Term term in difComponents) {
                DifString += $"{term} ";
            }
            return DifString;
        }

        public bool VerifyAnswer(string answer)
        {
            string[] tempstrings = Regex.Split(Common.RemoveWhitespace(answer), @"(\+|\-)");
            return true;
        }
        
    }

}
