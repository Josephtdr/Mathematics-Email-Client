using System;
using System.Collections.Generic;
using System.Data.OleDb;
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

namespace The_Email_Client {
    /// <summary>
    /// Interaction logic for ResetPasswordPage.xaml
    /// </summary>
    public partial class ResetPasswordPage : Page {
        protected Action Close { get; set; }
        public ResetPasswordPage(Action Close) {
            InitializeComponent();
            this.Close = Close;
            //allows user to press enter to reset their password
            KeyDown += delegate {
                if (Keyboard.IsKeyDown(Key.Enter))
                    ResetPassword();
            };
        }
        private void ResetPassword() {
            //Checks both passwords match and are not null
            if (!string.IsNullOrWhiteSpace(PasswordBox.Password) &&
                PasswordBox.Password == PasswordConfirmationBox.Password) {
                bool finished = false;
                OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
                try {
                    cnctDTB.Open();//opens database connection
                    //Updates password within database to be a hashed version of their entered string
                    OleDbCommand cmd = new OleDbCommand($"UPDATE Profiles SET [Password] " +
                        $"='{ Encryption.HashString(PasswordBox.Password) }'" +
                        $" Where UserName='{ ForgottonPasswordPage.UserName }';", cnctDTB);
                    cmd.ExecuteNonQuery();//Runs the command
                    finished = true;//Updates the variable to indicate the SQL ran successfully
                }
                catch (Exception err) { MessageBox.Show(err.Message); }
                finally { cnctDTB.Close(); }
                if (finished) Close();
            }
            //informs user their passwords do not match
            else MessageBox.Show("Passwords Do not Match", "Error!");
        }
        private void ResetPasswordButton_Click(object sender, RoutedEventArgs e) {
            ResetPassword();
        }
    }
}
