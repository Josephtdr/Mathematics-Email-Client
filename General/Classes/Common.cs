using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.OleDb;
using System.Globalization;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace The_Email_Client
{
    internal static class Common
    {
        public static Profiles Profile { get; set; }

        public static List<string> Attachments { get; set; }
        public static List<OpenFileDialog> AttachmentsSource { get; set; }

        public static long TotalFileLength { get; set; }
        
        public static string Cleanstr(object unregexed)//Takes a string from DB and returns it without any formating
        {
            return Regex.Replace(unregexed.ToString(), "<.*?>", String.Empty);
        }
        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }
        public static bool Inccorectemailformat(string email)
        {
            if (Regex.IsMatch(email, Constants.VALIDEMAILPATTERN, RegexOptions.IgnoreCase)) return true;
            else
            {
                System.Windows.MessageBox.Show("Email is of an invalid type", "Error!");
                return false;
            }
        }

        public static bool Preexistingemail(string email, List<string> emaillist) {
            if (emaillist.Contains(email)) {
                System.Windows.MessageBox.Show("A contact with this email already exists!", "Error");
                return false;
            }
            return true;
        }

        public static bool Inccorectpasswordformat(string password)
        {
            if (Regex.IsMatch(password, Constants.VALIDPASSWORDPATTERN)) return true;
            else
            {
                System.Windows.MessageBox.Show("Invalid Password Format. Must Contain at least 1 captial letter, 1 lowercase letters, 1 digit and be a minimum of 8 characters in length.", "Error!");
                return false;
            }
        }

        

    }
}