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

            KeyDown += delegate {
                if (Keyboard.IsKeyDown(Key.Enter)) {
                    if (Common.inccorectemailformat(EmailTextBox.Text) && EmailAlreadyExists(EmailTextBox.Text)
                       && UserNameAlreadyExists(UserNameTextBox.Text) && PasswordsMatch())
                        RegisterUser();
                 }
            };
        }
        
        private void SignUpbutton_Click(object sender, RoutedEventArgs e)
        {
            if (Common.inccorectemailformat(EmailTextBox.Text)
                && UserNameAlreadyExists(UserNameTextBox.Text) && PasswordsMatch())
                RegisterUser();
        }

        private bool EmailAlreadyExists(string Email)
        {
        //    OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
        //    try
        //    {
        //        cnctDTB.Open();
        //        OleDbCommand cmd = new OleDbCommand("SELECT Email FROM Profiles", cnctDTB);
        //        OleDbDataReader reader = cmd.ExecuteReader();

        //        while (reader.Read()) if (Email == Common.Cleanstr(reader[0])) {
        //                MessageBox.Show("Email Already Exists!", "Error!");
        //                return false; }             
        //    }
        //    catch (Exception err)
        //    {

        //        System.Windows.MessageBox.Show(err.Message);
        //    }
        //    finally
        //    {
        //        cnctDTB.Close();
        //    }
            return true;
        }

        private bool UserNameAlreadyExists(string UserName)
        {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try
            {
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand("SELECT UserName FROM Profiles", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();

                while (reader.Read()) if (UserName == Common.Cleanstr(reader[0]))
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
            string email = EmailTextBox.Text; 
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            string hashedPassword = Encryption.HashString(Passwordbox.Password;);
            try
            {
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand($"INSERT INTO Profiles ([Name], [Email], [Port], [Server], [Password], [UserName]) VALUES " + 
                    $"('{Encryption.BinaryEncryption(NameTextBox.Text, Password)}','{Encryption.HashString(EmailTextBox.Text)}"+
                    $"',587,'smtp.gmail.com','{hashedPassword}','{UserNameTextBox.Text}');", cnctDTB);
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            EmailTextBox.Clear(); NameTextBox.Clear();
            ShowEmailPage?.Invoke();
        }
    }






    
}
