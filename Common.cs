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

namespace The_Email_Client
{
    internal class Common
    {
        public static string Email { get; set; }

        public static string Cleanstr(object unregexed)//Takes a string from DB and returns it without any formating
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

        public static string HashPassword(string password)
        {
            //Create the salt value with a cryptographic PRNG
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            //Create the Rfc2898DeriveBytes and get the hash value
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            //Combine the salt and password bytes for later use
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            //Turn the combined salt+hash into a string for storage
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }

        public static bool VerifyPassword(string email, string password)
        {
            //Fetch the stored value
            string savedPasswordHash = "sXgXELuRUY1AA+zdIJhemHznP9tWElMqyC93jBCcpD1TsPh5";
            int passID = 0;
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try
            {
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand($"SELECT * FROM Profiles WHERE Email='{email}';", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) { passID = Convert.ToInt16(Common.Cleanstr(reader[5])); }

                cnctDTB.Close();
                cnctDTB.Open();
                cmd.CommandText = $"SELECT Password FROM Passwords WHERE ID={passID};";
                reader = cmd.ExecuteReader();
                while (reader.Read()) savedPasswordHash = Common.Cleanstr(reader[0]);
            }
            catch (Exception err) { MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); }

            //Extract the bytes
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            //Get the salt
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            //Compute the hash on the password the user entered
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            //Compare the results
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i]) {
                    MessageBox.Show("Incorrect Password", "Error!");
                    return false; }

            return true;

            }
    }
}