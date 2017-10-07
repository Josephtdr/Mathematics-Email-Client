using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace The_Email_Client
{
    public class Fraction
    {
        public int Numerator { get; set; }
        public int Denominator { get; set; }
        public float Value { get {
                return (float)Numerator / (float)Denominator;
            }
        }

        public override string ToString()
        {
            Console.WriteLine(Value);
            bool WholeNumber = (Value == Math.Floor(Value));
            string str = ((WholeNumber ? $"{ Math.Floor(Value) }" : $"{Numerator}/{Denominator}")
                .Replace("-", "")).Replace("+", "");

            return str;
        }

        public static int GCD(int a, int b)
        {
            int Remainder;

            while (b != 0)
            {
                Remainder = a % b;
                a = b;
                b = Remainder;
            }

            return a;
        }
        
    }

    public class Term
    {
        public Fraction Coefficient { get; set; }
        public Fraction Power { get; set; }
        public string X { get {
                return Power.Value == 0 ? "" : Power.Value == 1 ? "x" : Power.Value < 0 ? $"x^-{Power}" : $"x^{Power}";
            }
        }

        public override string ToString()
        {
            string coeff = (Coefficient.Value > 0 ? $"+{Coefficient}" : $"-{Coefficient}");
            return (Coefficient.Numerator == 0 ? "" : $"{coeff}{X}");
        }

    }

    public abstract class Equation
    {
        public List<Term> Components { get; set; }
        
        public Equation(int Order, int Difficulty, int UsingFractions)
        {
            Components = new List<Term>();
            Random rnd = new Random();
            for (int i = Order; i > -1; --i)
            {
                int Coe_numerator = rnd.Next(-((Difficulty + 1) * 3), (Difficulty + 1) * 3);
                int Coe_denominator = UsingFractions==0 ? rnd.Next(-1, 2) : rnd.Next(-Difficulty, Difficulty+1);
                Coe_denominator = (Coe_denominator != 0 ? Coe_denominator : 1);
                int GCD = Fraction.GCD(Coe_numerator, Coe_denominator); Coe_numerator /= GCD; Coe_denominator /= GCD; //simplifies the fractions

                int Pow_numerator = i;
                int Pow_denominator = UsingFractions != 2 ? 1 : rnd.Next(-Difficulty, Difficulty+1);
                Pow_denominator = (Pow_denominator != 0 ? Pow_denominator : 1);
                GCD = Fraction.GCD(Pow_numerator, Pow_denominator); Pow_numerator /= GCD; Pow_denominator /= GCD; //simplifies the fractions

                if (Coe_numerator != 0)
                Components.Add(
                    new Term
                    {
                        Coefficient = new Fraction {
                            Numerator = Coe_numerator,
                            Denominator = Coe_denominator
                        },
                        Power = new Fraction {
                            Numerator = Pow_numerator,
                            Denominator = Pow_denominator
                        }
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

        public List<Term> SolvedComponents { get; set; }
        public List<Term> Answer { get; set; }

        public Diferentiation(int Order, int Difficulty, int UsingFractions)  : base(Order,Difficulty,UsingFractions)
        {
            SolvedComponents = new List<Term>();
            foreach (Term term in Components)
            {
                if (term.Power.Numerator != 0) {
                    int Coe_numerator = term.Coefficient.Numerator * term.Power.Numerator; int Coe_denominator = term.Coefficient.Denominator * term.Power.Denominator;
                    int GCD = Fraction.GCD(Coe_numerator, Coe_denominator); Coe_numerator /= GCD; Coe_denominator /= GCD;
                    SolvedComponents.Add(new Term
                    {
                        Coefficient = new Fraction
                        {
                            Numerator = Coe_numerator,
                            Denominator = Coe_denominator
                        },
                        Power = new Fraction
                        {
                            Numerator = term.Power.Numerator- term.Power.Denominator,
                            Denominator = term.Power.Denominator
                        }
                    });
                }
            }
        }

        public override string SolvedEquationToString()
        {
            string DifString = "";
            foreach (Term term in SolvedComponents) 
                DifString += $"{term} ";
            return DifString;
        }

        public override bool VerifyAnswer(string answer)
        {
            string[] tempstrings = Regex.Split(Common.RemoveWhitespace(answer), @"(?<=[^^])(?=[-+])");

            Answer = new List<Term>();
            foreach (string str in tempstrings)
            {
                if (str != "")
                {
                    string tempstring = (str.Replace("^", "")).Replace("+", "");
                    string[] components = tempstring.Split('x');
                    string[] Coefficient_Fraction = components[0].Split('/');
                    string[] Power_Fraction = components.Length < 2 ? new string[] { "0", "1" } : components[1] == ""  ? new string[] { "1", "1" } : components[1].Split('/') ;

                    Answer.Add(new Term {
                        Coefficient = new Fraction
                        {
                            Numerator = Convert.ToInt16(Coefficient_Fraction[0]),
                            Denominator = Coefficient_Fraction.Length > 1 ? Convert.ToInt16(Coefficient_Fraction[1]) : 1
                        },
                        Power = new Fraction
                        {
                            Numerator = Convert.ToInt16(Power_Fraction[0]),
                            Denominator = Power_Fraction.Length > 1 ? Convert.ToInt16(Power_Fraction[1]) : 1
                        }
                    });
                }
            }

            for (int i = 0; i < SolvedComponents.Count; i++)
            {
                if (SolvedComponents[i].ToString() != Answer[i].ToString())
                    return false;
            }

            return true;
        }
    }
}
