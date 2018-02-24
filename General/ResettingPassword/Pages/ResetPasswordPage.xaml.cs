using System;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace The_Email_Client {
    /// <summary>
    /// Page for resetting users password
    /// </summary>
    public partial class ResetPasswordPage : Page {
        protected Action Close { get; set; }
        //constructor
        public ResetPasswordPage(Action Close) {
            InitializeComponent();
            this.Close = Close;
            //allows user to press enter to reset their password
            KeyDown += delegate {
                if (Keyboard.IsKeyDown(Key.Enter))
                    ResetPassword();
            };
        }
        //function to reset a users password
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
        //runs when user presses reset password button
        private void ResetPasswordButton_Click(object sender, RoutedEventArgs e) {
            ResetPassword();
        }
    }
}
