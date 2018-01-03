﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace The_Email_Client
{
    public class Fraction
    {
        public int Numerator { get; set; }//Variable for the numerator (top half) of the fraction
        public int Denominator { get; set; }//Variable for the denominator (bottom half) of the fraction
        public float Value { get { //Returns the decimal value of the fraction
                return (float)Numerator / (float)Denominator;
            }
        }
        //Overrides the ToString command such that it displays a fraction correctly when called.
        public override string ToString() {
            Console.WriteLine(Value);
            bool WholeNumber = (Value == Math.Floor(Value));
            string str = ((WholeNumber ? $"{ Math.Floor(Value) }" : $"{Numerator}/{Denominator}")
                .Replace("-", "")).Replace("+", "");

            return str;
        }
        //Function to convert fraction to simplest form
        public void GCD() {
            int Remainder;
            int a = Numerator;
            int b = Denominator;

            while (b != 0) {
                Remainder = a % b;
                a = b;
                b = Remainder;
            }
            Numerator /= a;
            Denominator /= a;
        }
        public static Fraction operator +(Fraction left, Fraction right) {
            Fraction tempfraction = new Fraction {
                Numerator = ((left.Numerator * right.Denominator) + (right.Numerator * left.Denominator)),
                Denominator = (left.Denominator * right.Denominator)
            };
            tempfraction.GCD(); //simplifies fraction
            return tempfraction;
        }
    }

    public class Term {
        public Fraction Coefficient { get; set; } //Variable for the coefficient of each term
        public Fraction Power { get; set; } //Variable for the coefficient of each term
        public string X { get { //When Term.X is called, will output a string depending on the power value
                return Power.Value == 0 ? "" : Power.Value == 1? "x" : Power.Value < 0 ? $"x^-{Power}" : $"x^{Power}";
            }
        }

        public override string ToString()
        {
            string coefficient = (Power.Numerator != 0 ? (Coefficient.Value == 1 || Coefficient.Value == -1) ? "" : $"{Coefficient}" : $"{Coefficient}");
            string posetivity = (Coefficient.Value > 0 ? $"+{coefficient}" : $"-{coefficient}");
            return (Coefficient.Numerator == 0 ? "" : $"{posetivity}{X}");
        }

        public static Term operator +(Term left, Term right) {
            Fraction tempfraction = new Fraction {
                Numerator = ((left.Coefficient.Numerator * right.Coefficient.Denominator) + (right.Coefficient.Numerator * left.Coefficient.Denominator)),
                Denominator = (left.Coefficient.Denominator * right.Coefficient.Denominator)
            };
            tempfraction.GCD(); //simplifies fraction
            return new Term {
                Coefficient = tempfraction,
                Power = left.Power
            };
        }
    }

    public abstract class Equation {//class for each equation
        public static Random Rng = new Random(); //creates an instance of the random class
        public List<Term> Components { get; set; } //stores the equation
        public List<Term> Answer { get; set; } //stores the users answer to said equation
        public List<Term> FprimeComponents { get; set; }//stores the answer to said equation
        public abstract float CalculateAnswer(float x1, float? x2 = null);
        
        public Equation(int Order, int Magnitude, int UsingFractions) {
            Components = new List<Term>();
            for (int i = Order; i > -1; --i)
            {
                //Generates random values for the Coefficient of the Value.
                int Coe_numerator = Rng.Next(-((Magnitude + 1) * 3), (Magnitude + 1) * 3);
                int Coe_denominator = UsingFractions==0 ? Rng.Next(-1, 2) : Rng.Next(-Magnitude, Magnitude+1);
                Coe_denominator = (Coe_denominator != 0 ? Coe_denominator : 1); //Makes sure the variable is not 0
                Fraction Coefficient = new Fraction { //Creates a new Fraction with the generated values
                    Numerator = Coe_numerator,
                    Denominator = Coe_denominator
                };
                Coefficient.GCD(); //Coverts the fraction to its simplest form

                int Pow_numerator = i;
                int Pow_denominator = UsingFractions != 2 ? 1 : Constants.Rnd.Next(-Magnitude, Magnitude+1);
                Pow_denominator = (Pow_denominator != 0 ? Pow_denominator : 1);
                Fraction Power = new Fraction {
                    Numerator = Pow_numerator,
                    Denominator = Pow_denominator
                };
                Power.GCD();

                if (Coe_numerator != 0)
                    Components.Add(
                        new Term {
                            Coefficient = Coefficient,
                            Power = Power
                    }
                );
            }
            Components = BubbleSort(Components);
        }
        public List<Term> ParseString(string stringtoparse) { //Converts a user entered string into a list of terms 
            stringtoparse.ToLower();
            string[] tempstrings = Regex.Split(Common.RemoveWhitespace(stringtoparse), @"(?<=[^^])(?=[-+])");

            List<Term> ParsedList = new List<Term>();
            foreach (string str in tempstrings) 
                if (!string.IsNullOrWhiteSpace(str)) {
                    string tempstring = (str.Replace("^", "")).Replace("+", "");
                    string[] components = tempstring.Split('x');
                    string[] Coefficient_Fraction = components[0] == "" ? new string[] { "1", "1" } : components[0] == "-" ? new string[] { "-1", "1" } : components[0].Split('/');
                    string[] Power_Fraction = components.Length < 2 ? new string[] { "0", "1" } : components[1] == "" ? new string[] { "1", "1" } : components[1].Split('/');

                    ParsedList.Add(new Term
                    {
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

            return ParsedList;
        }
        public override string ToString() {
            string Equationstring = "";
            foreach (Term term in Components)
            {
                Equationstring += $"[{term}] ";
            }
            return Equationstring;
        }
        public string FprimeEquationToString() {
            string tempString = "";
            foreach (Term term in FprimeComponents)
                tempString += $"{term} ";
            return tempString;
        }
        public bool VerifyAnswer(string answer) {
            //BubbleSort sorts the terms in order of power as so the answers ordering is not punished.
            Answer = BubbleSort(ParseString(answer));
            List<Term> AnswerCheck = BubbleSort(FprimeComponents);
            for (int i = 0; i < FprimeComponents.Count; i++) {
                if (Answer[i].ToString() == null || FprimeComponents[i].ToString() != Answer[i].ToString())
                    return false;
            }
            return true;
        }
        public float Fprime(float x) {
            float Fofx = 0;
            foreach (Term term in FprimeComponents)
                Fofx += ((float)(term.Coefficient.Value * (Math.Pow(x, term.Power.Value))));
            return Fofx;
        }//Calculates the gradient of the function F at the given point x

        public static List<Term> BubbleSort(List<Term> array) {
            Term temp = new Term();

            for (int write = 0; write < array.Count; write++) {
                for (int sort = 1; sort < array.Count; sort++) {
                    if (array[sort].Power.Value > array[sort - 1].Power.Value) {
                        temp = array[sort - 1];
                        array[sort- 1] = array[sort];
                        array[sort] = temp;
                    }
                    if (array[sort].Coefficient.Value != 0 && array[sort].Power.Value == array[sort - 1].Power.Value) {
                        array[sort] = array[sort] + array[sort - 1];
                        array[sort - 1] = new Term {
                            Coefficient = new Fraction { Numerator = 0, Denominator = 1},
                            Power = new Fraction { Numerator = 0, Denominator = 1 }
                        };
                    }
                }
            }
            array.RemoveAll(item => item.Coefficient.Value == 0);

            List<Term> list = new List<Term>();
            foreach(Term term in array) {
                if (term.Power.Value != 0) list.Add(term);
            }
            list.AddRange(array.Where(item => item.Power.Value == 0).ToList());

            return list;
        }
    }

    public class Diferentiation : Equation
    {
        public Diferentiation(int Order, int Magnitude, int UsingFractions)  : base(Order, Magnitude, UsingFractions)
        {
            FprimeComponents = new List<Term>();
            
            foreach (Term term in Components)
            {
                if (term.Power.Numerator != 0) {
                    int Coe_numerator = term.Coefficient.Numerator * term.Power.Numerator;
                    int Coe_denominator = term.Coefficient.Denominator * term.Power.Denominator;
                    Fraction Coefficient = new Fraction {
                        Numerator = Coe_numerator,
                        Denominator = Coe_denominator
                    };
                    Coefficient.GCD();


                    FprimeComponents.Add(new Term {
                        Coefficient = Coefficient,
                        Power = new Fraction {
                            Numerator = term.Power.Numerator- term.Power.Denominator,
                            Denominator = term.Power.Denominator
                        }
                    });
                }
            }
        }
        public override float CalculateAnswer(float x1, float? x2 = null)
        {
            return Fprime(x1);
        }
    }

    public class Integration : Equation
    {
        public Integration(int Order, int Magnitude, int UsingFractions)  : base(Order, Magnitude, UsingFractions)
        {
            FprimeComponents = new List<Term>();
            foreach (Term term in Components)
            {
                    int Coe_numerator = term.Coefficient.Numerator * term.Power.Denominator; int Coe_denominator = term.Coefficient.Denominator * (term.Power.Numerator + term.Power.Denominator);
                    Fraction Coefficient = new Fraction {
                        Numerator = Coe_numerator,
                        Denominator = Coe_denominator
                    };
                    Coefficient.GCD();

                    FprimeComponents.Add(new Term {
                        Coefficient = Coefficient,
                        Power = new Fraction
                        {
                            Numerator = term.Power.Numerator + term.Power.Denominator,
                            Denominator = term.Power.Denominator
                        }
                    });
            }
            
        }
        public override float CalculateAnswer(float x1, float? x2 = null)
        {
            return Fprime(x1) - Fprime((float)x2);
        }
    }
}
