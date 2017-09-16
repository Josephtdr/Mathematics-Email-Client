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
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        protected Action ShowEmailPage { get; set; }
        protected Action ShowRegistationPage { get; set; }
        public string Email { get; set; }
        protected string Password { get; set; }
        private int passID { get; set; }
        public LoginPage(Action ShowEmailPage, Action ShowRegistationPage)
        {
            this.ShowEmailPage = ShowEmailPage;
            this.ShowRegistationPage = ShowRegistationPage;
            InitializeComponent();
            DataContext = this;
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Enter))
                { if (EmailExists()) if (passcorrect(Email)) ShowEmailPage?.Invoke(); }
            };
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {

            if (EmailExists()) if(passcorrect(Email)) ShowEmailPage?.Invoke();
            
        }

        private bool EmailExists()
        {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try
            {
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand("SELECT * FROM Profiles", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (Email == Common.Cleanstr((string)reader[1])) {
                        passID = Convert.ToInt16(Common.Cleanstr(reader[5].ToString()));
                        return true; } 
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
            MessageBox.Show("Email does not exist!", "Error!");
            return false;
        }

        private bool passcorrect(string Email)
        {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try
            {
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand($"SELECT Password FROM Passwords WHERE ID={passID}", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (Common.Cleanstr(reader[0].ToString()) == Passwordbox.Password) return true;
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
            MessageBox.Show("Incorrect Password","Error!");
            return false;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Registrationbutton_Click(object sender, RoutedEventArgs e)
        {
            ShowRegistationPage?.Invoke();
        }
    }
}
