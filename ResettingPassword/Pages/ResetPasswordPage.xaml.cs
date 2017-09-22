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

namespace The_Email_Client
{
    /// <summary>
    /// Interaction logic for ResetPasswordPage.xaml
    /// </summary>
    public partial class ResetPasswordPage : Page
    {
        protected Action Close { get; set; }
        public ResetPasswordPage(Action Close)
        {
            InitializeComponent();
            this.Close = Close;
            KeyDown += delegate {
                if (Keyboard.IsKeyDown(Key.Enter))
                    ResetPassword();
            };
        }
        private void ResetPassword()
        {
            if (PasswordBox.Password == PasswordConfirmationBox.Password)
            {
                bool finished = false;
                OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
                try
                {
                    cnctDTB.Open();
                    OleDbCommand cmd = new OleDbCommand($"UPDATE Profiles SET [Password] ='{ Encryption.HashString(PasswordBox.Password) }' Where UserName='{ ForgottonPasswordPage.UserName }';", cnctDTB);
                    cmd.ExecuteNonQuery();
                    finished = true;
                }
                catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
                finally { cnctDTB.Close(); }
                if (finished) Close();
            }
            else MessageBox.Show("Passwords Do not Match", "Error!");
        }
        private void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            ResetPassword();
        }
    }
}
