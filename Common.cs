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

namespace The_Email_Client
{
    internal class Common
    {
        public static string Email { get; set; }

        public static string Cleanstr(object unregexed)
        {
            return Regex.Replace(unregexed.ToString(), "<.*?>", String.Empty);
        }

        public static bool inccorectemailformat(string email)
        {
            if (Regex.IsMatch(email, Constants.VALIDEMAILPATTERN, RegexOptions.IgnoreCase)) return true;
            else
            {
                MessageBox.Show("Invalid Email Format", "Error!");
                return false;
            }
        }

        public static bool inccorectpasswordformat(string password)
        {
            if (Regex.IsMatch(password, Constants.VALIDPASSWORDPATTERN)) return true;
            else
            {
                MessageBox.Show("Invalid Password Format. Must Contain at least 1 captial letter, 1 lowercase letters, 1 digit and be a minimum of 8 characters in length.", "Error!");
                return false;
            }
        }
    }
}