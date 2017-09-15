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

        public static string Cleanstr(string unregexed)
        {
            return Regex.Replace(unregexed, "<.*?>", String.Empty);
        }
    }
}