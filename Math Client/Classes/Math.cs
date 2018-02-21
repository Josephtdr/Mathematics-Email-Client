using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace The_Email_Client
{
    public class Fraction
    {
        public long Numerator { get; set; }//Variable for the numerator (top half) of the fraction
        public long Denominator { get; set; }//Variable for the denominator (bottom half) of the fraction
        public float Value { get { //Returns the decimal value of the fraction
                return (float)Numerator / (float)Denominator;
            }
        }
        //Overrides the ToString command such that it displays a fraction correctly when called.
        public override string ToString() {
            bool WholeNumber = (Value == Math.Floor(Value));
            string str = ((WholeNumber ? $"{ Math.Floor(Value) }" : $"{Numerator}/{Denominator}")
                .Replace("-", "")).Replace("+", "");

            return str;
        }
        //Function to return the string which represents this fraction in latex
        public string FractionToLatex() {
            bool WholeNumber = (Value == Math.Floor(Value));//Bool to check if number is a whole number
            string str = ((WholeNumber ? //Creates a latex string depending upon the type, removing any symbols
                $"{ Math.Floor(Value) }" : $"\\frac{{{Numerator}}}{{{Denominator}}}")
                .Replace("-", "")).Replace("+", "");
            return str; //Returns the latex string
        }
        //Function to convert fraction to simplest form
        public void GCD() {
            long Remainder;
            long a = Numerator;
            long b = Denominator;
            //Calculates the greatest common divisor between the Numerator and Denominator
            while (b != 0) {
                Remainder = a % b;
                a = b;
                b = Remainder;
            }
            //Updates values to their simplest form
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

        public static Fraction operator *(Fraction left, Fraction right) {
            Fraction tempfraction = new Fraction {
                Numerator = (left.Numerator * right.Numerator),
                Denominator = (left.Denominator * right.Denominator)
            };
            tempfraction.GCD(); //simplifies fraction
            return tempfraction;
        }

        public static Fraction operator /(Fraction left, Fraction right) {
            Fraction tempfraction = new Fraction {
                Numerator = (left.Numerator * right.Denominator),
                Denominator = (left.Denominator * right.Numerator)
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

        public override string ToString() {
            string coefficient = (Power.Numerator != 0 ? (Coefficient.Value == 1 || Coefficient.Value == -1) ? "" : $"{Coefficient}" : $"{Coefficient}");
            string posetivity = (Coefficient.Value > 0 ? $"+{coefficient}" : $"-{coefficient}");
            return (Coefficient.Numerator == 0 ? "" : $"{posetivity}{X}");
        }
        //Function to crate a latex string to represent the term
        public string TermToLatex() {
            //String which will change depending upon the power of the term
            string LateX = Power.Value == 0 ? "" : Power.Value == 1 ? "x" : Power.Value < 0 ?
                $"x^{{-{Power.FractionToLatex()}}}" : $"x^{{{Power.FractionToLatex()}}}";
            //String which will change depending upon the coefficient of the term
            string coefficient = (Power.Numerator != 0 ? (Coefficient.Value == 1 || Coefficient.Value == -1) ? 
                "" : $"{Coefficient.FractionToLatex()}" : $"{Coefficient.FractionToLatex()}");
            //String which determins the sign of the equation
            string posetivity = (Coefficient.Value > 0 ? $"+{coefficient}" : $"-{coefficient}");
            //Returns the overall latex string 
            return (Coefficient.Numerator == 0 ? "" : $"{posetivity}{LateX}");
        }

        public static Term operator +(Term left, Term right) {
            return new Term {
                Coefficient = left.Coefficient + right.Coefficient,
                Power = left.Power
            };
        }
    }

    public abstract class Equation {//class for each equation
        public static Random Rng = new Random(); //creates an instance of the random class
        public List<Term> Components { get; set; } //stores the equation
        public List<Term> equationAnswer { get; set; } //stores the users answer to said equation
        public List<Term> FprimeComponents { get; set; }//stores the answer to said equation
        public abstract float CalculateAnswer(float x1, float? x2 = null);
        
        public Equation(List<Term> Components) {
            this.Components = Components;
        }
        public static List<Term> ParseString(string stringtoparse) { //Converts a user entered string into a list of terms 
            List<Term> ParsedList = new List<Term>();
            string[] TermList = Regex.Split(Common.RemoveWhitespace(stringtoparse.ToLower()), @"(?<=[^^])(?=[-+])");
            //Splits the string wherever there is a '+' or '-' but not when it is preceded by a '^'

            foreach (string str in TermList)//Loops through each term 
                if (!string.IsNullOrWhiteSpace(str)) {//Checks if the term is empty
                    string tempstring = (str.Replace("^", "")).Replace("+", "");//Removes '+' and '^' from the term
                    string[] components = tempstring.Split('x');//splits the term into the sections before/after the 'x'
                    string[] Coefficient_Fraction = components[0] == "" ? new string[] { "1", "1" } :
                        components[0] == "-" ? new string[] { "-1", "1" } : components[0].Split('/');
                    string[] Power_Fraction = components.Length < 2 ? new string[] { "0", "1" } :
                        components[1] == "" ? new string[] { "1", "1" } : components[1].Split('/');
                    //Creates a Term from the users string of said term 
                    try {//Attempts to converts strings to numbers 
                        ParsedList.Add(new Term {
                            Coefficient = new Fraction {
                                Numerator = Convert.ToInt64(Coefficient_Fraction[0]),
                                Denominator = Coefficient_Fraction.Length > 1 ? Convert.ToInt64(Coefficient_Fraction[1]) : 1
                            },//Creates the fraction representing the coefficient of each term
                            Power = new Fraction {
                                Numerator = Convert.ToInt64(Power_Fraction[0]),
                                Denominator = Power_Fraction.Length > 1 ? Convert.ToInt64(Power_Fraction[1]) : 1
                            }//Creates the fraction representing the power of each term
                        });//Adds the new term to the list of Terms ParsedList 
                    }//If numbers entered by users are too large will display an error informing the user as such
                    catch {
                        MessageBox.Show("Entered Numbers are too large", "Error!");
                        return null;
                    }
                }
            return BubbleSort(ParsedList); //returns a sorted version of ParsedList
        }
        //Creates a random equation based on user perameters. 
        public static List<Term> CreateRandomEquation(int Order, int Magnitude, int UsingFractions) {
            List<Term> RndEqu = new List<Term>();
            for (int i = Order; i > -1; --i) {//Loops n times where n = order which is specified by the user. 
                //Generates random values for the Coefficient of the Value depending upon the users perameters.
                int Coe_numerator = Rng.Next(-((Magnitude + 1) * 3), (Magnitude + 1) * 3);
                //Creates a Denominator of either 1, -1 or a random value depending on the users perameters. 
                int Coe_denominator = UsingFractions == 0 ? Rng.Next(-1, 2) : Rng.Next(-Magnitude, Magnitude + 1);
                Coe_denominator = (Coe_denominator != 0 ? Coe_denominator : 1); //Makes sure the variable is not 0
                Fraction Coefficient = new Fraction { //Creates a new Fraction with the generated values
                    Numerator = Coe_numerator,
                    Denominator = Coe_denominator
                };
                Coefficient.GCD(); //Coverts the fraction to its simplest form
                //Creates a Denominator of either 1, -1 or a random value depending on the users perameters. 
                int Pow_denominator = UsingFractions != 2 ? 1 : Constants.Rnd.Next(-Magnitude, Magnitude + 1); 
                Pow_denominator = (Pow_denominator != 0 ? Pow_denominator : 1);//Makes sure the variable is not 0
                Fraction Power = new Fraction {
                    Numerator = i, //Numerator itterates up with i
                    Denominator = Pow_denominator
                };
                Power.GCD(); //Coverts the fraction to its simplest form
                if (Coe_numerator != 0)
                    RndEqu.Add(
                        new Term {
                            Coefficient = Coefficient,
                            Power = Power
                        }
                );//Adds a new term to the equation
            }
            return BubbleSort(RndEqu); //Returns a sorted version of the list
        } 
        public override string ToString() {
            string Equationstring = "";
            foreach (Term term in Components)
                Equationstring += $"[{term}] ";
            return Equationstring;
        }
        //Returns a latex string of an equation
        public string EquationToLatex() {
            string Equationstring = "";//Creates a blank string
            foreach (Term term in Components)//loops through each term in the equation
                //Adds each term to the string in order
                Equationstring += $"{term.TermToLatex()} ";
            return Equationstring; //returns the latex string
        }
        public string FprimeEquationToString() {
            string tempString = "";
            foreach (Term term in FprimeComponents)
                tempString += $"{term} ";
            return tempString;
        }
        public string FprimeEquationToLatex() {
            string Equationstring = "";
            foreach (Term term in FprimeComponents)
                Equationstring += $"{term.TermToLatex()} ";
            return Equationstring;
        }
        public Tuple<bool, bool> VerifyAnswer(string stringAnswer) {
            //BubbleSort sorts the terms in order of power as so the answers ordering is not punished.
            equationAnswer = ParseString(stringAnswer);
            //Makes sure the user entered a valid value
            if(equationAnswer == null) return new Tuple<bool, bool>(false, false);
            //Checks if the users answer is the correct answer and returns the case
            return new Tuple<bool, bool>( true, 
                (BubbleSort(equationAnswer).ToString() == BubbleSort(FprimeComponents).ToString()));
        }
        public float F(float x) {
            float Fofx = 0;
            foreach (Term term in FprimeComponents)
                Fofx += ((float)(term.Coefficient.Value * (Math.Pow(x, term.Power.Value))));
            return Fofx;
        }//Calculates the gradient of the function F at the given point x
        public static List<Term> BubbleSort(List<Term> termList) {
            Term temp = new Term();

            for (int write = 0; write < termList.Count; write++) {
                for (int sort = 1; sort < termList.Count; sort++) {
                    //if the first term is great than the second term swap the terms positions
                    if (termList[sort].Power.Value > termList[sort - 1].Power.Value) {
                        temp = termList[sort - 1];
                        termList[sort- 1] = termList[sort];
                        termList[sort] = temp;
                    }//if both terms power have the same value which is not 0 combine them
                    if (termList[sort].Coefficient.Value != 0 && termList[sort].Power.Value == termList[sort - 1].Power.Value) {
                        termList[sort] = termList[sort] + termList[sort - 1];//combines terms by adding coefficients together
                        termList[sort - 1] = new Term {//Overites term to be 'null'
                            Coefficient = new Fraction { Numerator = 0, Denominator = 1},
                            Power = new Fraction { Numerator = 0, Denominator = 1 }
                        };
                    }
                }
            }
            termList.RemoveAll(item => item.Coefficient.Value == 0); //Removes all null entires 
            //Moves the term with a power of 0 to the end of the list
            List<Term> list = new List<Term>();
            foreach(Term term in termList) {
                if (term.Power.Value != 0) list.Add(term);
            }
            list.AddRange(termList.Where(item => item.Power.Value == 0).ToList());

            return list;
        }//bubblesort funtion + combing variables of same power
    }

    public class Diferentiation : Equation {
        public Diferentiation(List<Term> Components)  : base(Components) {
            FprimeComponents = new List<Term>(); //Initialises the List
            //Add new elements to the list
            foreach (Term term in Components) {
                if (term.Power.Numerator != 0) {
                    FprimeComponents.Add(new Term {
                        Coefficient = term.Coefficient * term.Power, //Multiplies the coeficient by the power and simplifies
                        Power = new Fraction {//Reduces the magnitude of the power by 1
                            Numerator = term.Power.Numerator - term.Power.Denominator,
                            Denominator = term.Power.Denominator
                        }
                    });
                }
            }
        }
        public override float CalculateAnswer(float x1, float? x2 = null) {
            return F(x1);
        }
    }

    public class Integration : Equation {
        public Integration(List<Term> Components) : base(Components) {
            FprimeComponents = new List<Term>();
            foreach (Term term in Components) {
                //Calculates the new power of the term
                Fraction NewPower = new Fraction {
                    Numerator = term.Power.Numerator + term.Power.Denominator,
                    Denominator = term.Power.Denominator
                };
                NewPower.GCD(); //Converts the fraction to its simplest form
                //Adds the new term to the list
                FprimeComponents.Add(new Term {
                    Coefficient = term.Coefficient / NewPower, //divides the coefficient by the new power and simplifies
                    Power = NewPower
                });
            }
        }
        public override float CalculateAnswer(float x1, float? x2 = null) {
            return F(x1) - F((float)x2);
        }
    }
}
