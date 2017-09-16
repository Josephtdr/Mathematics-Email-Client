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
    /// <summary>
    /// Interaction logic for RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {

        protected Action ShowEmailPage { get; set; }

        public RegistrationPage(Action ShowEmailPage)
        {
            InitializeComponent();
            this.ShowEmailPage = ShowEmailPage;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void SignUpbutton_Click(object sender, RoutedEventArgs e)
        {
            string Email = EmailTextBox.Text;
            if (Common.inccorectemailformat(Email) && EmailAlreadyExists(Email)
                && Common.inccorectpasswordformat(Passwordbox.Password) && PasswordsMatch()) {

            RegisterUser();
            }
        }

        private bool EmailAlreadyExists(string Email)
        {
            
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try
            {
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand("SELECT * FROM Profiles", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();

                while (reader.Read()) if (Email == Common.Cleanstr(reader[1])) {
                        MessageBox.Show("Email Already Exists!", "Error!");
                        return false; }             
            }
            catch (Exception err)
            {

                System.Windows.MessageBox.Show(err.Message);
            }
            finally
            {
                cnctDTB.Close();
            }
            return true;
        }

        private bool PasswordsMatch()
        {
            if (Passwordbox.Password == PasswordboxCopy.Password) return true;
            else
            {
                MessageBox.Show("Passwords do not Match!", "Error");
                return false;
            }
        }
       
        private void RegisterUser()
        {
            int PassID = 0;
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try
            {
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand($"INSERT INTO Passwords ([Password]) VALUES ('{PasswordHashing.HashPassword(Passwordbox.Password)}');", cnctDTB);
                cmd.ExecuteNonQuery();

                cmd.CommandText = $"SELECT * FROM Passwords WHERE Password ='{PasswordHashing.HashPassword(Passwordbox.Password)}';";
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) PassID = Convert.ToInt16(Common.Cleanstr(reader[0]));

                cnctDTB.Close();
                cnctDTB.Open();

                cmd.CommandText = $"INSERT INTO Profiles ([Email], [Name], [Port], [Server], [Password ID]) VALUES ('{EmailTextBox.Text}','{NameTextBox.Text}',587,'smtp.gmail.com',{PassID});";
                cmd.ExecuteNonQuery();
                MessageBox.Show("Successfully registered account.", "Success!");
                EmailTextBox.Clear(); NameTextBox.Clear();
                ShowEmailPage?.Invoke();
            }
            catch (Exception err)
            {
                MessageBox.Show("Failed to registered account.", "Error!");
                System.Windows.MessageBox.Show(err.Message);
            }
            finally
            {
                cnctDTB.Close();
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EmailTextBox.Clear(); NameTextBox.Clear();
            ShowEmailPage?.Invoke();
        }
    }






    
}
