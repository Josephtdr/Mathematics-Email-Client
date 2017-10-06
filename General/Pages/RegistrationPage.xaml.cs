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

        protected Action ShowLoginPage { get; set; }

        public RegistrationPage(Action ShowLoginPage)
        {
            InitializeComponent();
            this.ShowLoginPage = ShowLoginPage;

            KeyDown += delegate {
                if (Keyboard.IsKeyDown(Key.Enter)) {
                    if (
                        UserNameAlreadyExists(UserNameTextBox.Text) && PasswordsMatch()
                        && NonNullFields()) RegisterUser();
                 }
            };
        }
        
        private void SignUpbutton_Click(object sender, RoutedEventArgs e)
        {
            if (Common.Inccorectemailformat(EmailTextBox.Text)
                && UserNameAlreadyExists(UserNameTextBox.Text) && PasswordsMatch()
                && NonNullFields()) RegisterUser();
        }

        private bool NonNullFields()
        {
            if (!string.IsNullOrWhiteSpace(EmailTextBox.Text) && !string.IsNullOrWhiteSpace(NameTextBox.Text)
                && !string.IsNullOrWhiteSpace(UserNameTextBox.Text)) return true;
            else {
                MessageBox.Show("No fields can be left empty.","Error!");
                return false; }
        }

        private bool EmailAlreadyExists(string Email)
        {
            //OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            //try
            //{
            //    cnctDTB.Open();
            //    OleDbCommand cmd = new OleDbCommand("SELECT Email FROM Profiles", cnctDTB);
            //    OleDbDataReader reader = cmd.ExecuteReader();

            //    while (reader.Read()) if (Email == Common.Cleanstr(reader[0])) {
            //            MessageBox.Show("Email Already Exists!", "Error!");
            //            return false; }             
            //}
            //catch (Exception err)
            //{

            //    System.Windows.MessageBox.Show(err.Message);
            //}
            //finally
            //{
            //    cnctDTB.Close();
            //}
            return true;
        }

        private bool UserNameAlreadyExists(string UserName)
        {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try
            {
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand($"SELECT UserName FROM Profiles WHERE [UserName]='{UserName}'", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();

                while (reader.Read()) if (reader.HasRows)
                    {
                        MessageBox.Show("UserName Already Exists!", "Error!");
                        return false;
                    }
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
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            string hashedPassword = Encryption.HashString(Passwordbox.Password);
            int userID = 0;
            try
            {
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand($"INSERT INTO Profiles ([Name],[UserName],[Password],[Email]) VALUES " + 
                    $"('{NameTextBox.Text}','{UserNameTextBox.Text}','{hashedPassword}','{Encryption.HashString(EmailTextBox.Text)}');", cnctDTB);
                cmd.ExecuteNonQuery();

                cnctDTB.Close();
                cnctDTB.Open();

                cmd = new OleDbCommand($"Select ID from Profiles WHERE UserName='{UserNameTextBox.Text}';",cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) userID = (int)reader[0];

                cnctDTB.Close();
                cnctDTB.Open();
                cmd = new OleDbCommand($"INSERT INTO Settings ([Profile_ID],[Port],[Server]) VALUES "+
                $"({userID},587,'smtp.gmail.com');", cnctDTB);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Successfully registered account.", "Success!");
                EmailTextBox.Clear(); NameTextBox.Clear();
                ShowLoginPage?.Invoke();
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            EmailTextBox.Clear(); NameTextBox.Clear();
            ShowLoginPage?.Invoke();
        }
    }






    
}
