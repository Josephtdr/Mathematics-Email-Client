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
                return Power == 0 ? "" : Power == 1 ? "x" : $"x^{Power}";
            }
        }
        
        public override string ToString()
        {
            string coeff = (Coefficient > 0 ? $"+{Coefficient}" : $"{Coefficient}");
            return (Coefficient == 0 ? "" : $"{coeff}{X}");
        }
        
    }

    public abstract class Equation
    {
        public List<Term> Components { get; set; }
        
        public Equation(int Order, int Difficulty)
        {
            Components = new List<Term>();
            Random rnd = new Random();
            for (int i = Order - 1; i > -1; --i)
            {
                int coefficient = rnd.Next(-((Difficulty + 1) * 3), (Difficulty + 1) * 3);
                if(coefficient != 0)
                Components.Add(
                    new Term
                    {
                        Coefficient = coefficient,
                        Power = i,
                    }
                );
            }
            
        }

        public override string ToString()
        {
            string Equationstring = "";
            foreach (Term term in Components)
            {
                Equationstring += $"{term} ";
            }
            return Equationstring;
        }

        public abstract bool VerifyAnswer(string answer);

        public abstract string SolvedEquationToString();
        
    }

    public class Diferentiation : Equation
    {

        public List<Term> DifComponents { get; set; }
        public List<Term> Answer { get; set; }

        public Diferentiation(int Order, int Difficulty)  : base(Order,Difficulty)
        {
            DifComponents = new List<Term>();
            foreach (Term term in Components)
            {
                if (term.Power != 0)
                    DifComponents.Add(new Term
                    {
                        Coefficient = term.Coefficient * term.Power,
                        Power = term.Power - 1
                    });
            }
        }

        public override string SolvedEquationToString()
        {
            string DifString = "";
            foreach (Term term in DifComponents)
            {
                DifString += $"{term} ";
            }
            return DifString;
        }

        public override bool VerifyAnswer(string answer)
        {
            string[] tempstrings = Regex.Split(Common.RemoveWhitespace(answer), @"(?=[-+])");
            
            Answer = new List<Term>();
            foreach (string str in tempstrings) {
                if (str != "") { 
                string tempstring = str.Replace("^", ""); tempstring.Replace("+", "");
                string[] stringlist = tempstring.Split('x');
                Answer.Add(new Term {
                    Coefficient = Convert.ToInt16(stringlist[0]),
                    Power = (stringlist.Length < 2 ? 0 : stringlist[1] == "" ? 1 : Convert.ToInt16(stringlist[1]))
                }); }
            }

            for (int i = 0; i < DifComponents.Count; i++) {
                if (DifComponents[i].ToString() != Answer[i].ToString())
                    return false;
            }

            return true;
        }
    }
}
