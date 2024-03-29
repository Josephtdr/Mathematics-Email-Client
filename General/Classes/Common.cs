﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace The_Email_Client {
    /// <summary>
    /// Class for common functions/variables used throughout the code
    /// </summary>
    internal static class Common {//Class for common variables and functions between pages/windows
        public static Profiles Profile { get; set; }//Used to locally store users information

        public static List<string> Attachments { get; set; }//Used to store all current attachemnts
        public static List<OpenFileDialog> AttachmentsSource { get; set; }

        public static long TotalFileLength { get; set; }//Used to store total file length of attachments to an email
        
        public static string Cleanstr(object unregexed) {//Takes a string from DB and returns it without any formating 
            return Regex.Replace(unregexed.ToString(), "<.*?>", String.Empty);
        }
        //Used to remove whitespace from a string
        public static string RemoveWhitespace(this string input) {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }
        //Used to check if an email is of a correct format
        public static bool Inccorectemailformat(string email) {
            if (Regex.IsMatch(email, Constants.VALIDEMAILPATTERN, RegexOptions.IgnoreCase)) return true;
            else {
                System.Windows.MessageBox.Show("Email is of an invalid type", "Error!");
                return false;
            }
        }
        //Used to check if an email already exists within a list
        public static bool Preexistingemail(string email, List<string> emaillist) {
            if (emaillist.Contains(email)) {
                System.Windows.MessageBox.Show("A student with this email already exists!", "Error");
                return false;
            }
            return true;
        }
        //Returns true if a list is not null, not empty and the first element is not blank
        public static bool AnyAndNotNull<T>(this IEnumerable<T> source) {
            if (source != null && source.Any())
                return true;
            else
                return false;
        }
    }
}