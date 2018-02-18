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
        protected Action ShowHomePage { get; set; }
        protected Action ShowRegistationPage { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public LoginPage(Action ShowHomePage, Action ShowRegistationPage) {
            this.ShowHomePage = ShowHomePage;
            this.ShowRegistationPage = ShowRegistationPage;
            InitializeComponent();
            
            DataContext = this;
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Enter))
                { Password = Passwordbox.Password; UserName = UserNameTextBox.Text; Email = EmailTextBox.Text;
                    if (Encryption.VerifyPasswordorEmail(UserName, Password, true)) {
                        if (String.IsNullOrWhiteSpace(Email)) {
                            UserNameTextBox.Clear();
                            ShowHomePage?.Invoke();
                        }
                        else if (Encryption.VerifyPasswordorEmail(UserName, Email, false)) {
                            UserNameTextBox.Clear(); EmailTextBox.Clear();
                            ShowHomePage?.Invoke();
                        }
                    }
                }
                
            };
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e) {
            Password = Passwordbox.Password; UserName = UserNameTextBox.Text; Email = EmailTextBox.Text;
            if (Encryption.VerifyPasswordorEmail(UserName, Password, true)) {
                if (String.IsNullOrWhiteSpace(Email)) {
                    UserNameTextBox.Clear();
                    ShowHomePage?.Invoke();
                }
                else if (Encryption.VerifyPasswordorEmail(UserName, Email, false)) {
                    UserNameTextBox.Clear(); EmailTextBox.Clear();
                    ShowHomePage?.Invoke();
                }
            }
        }

        private void Registrationbutton_Click(object sender, RoutedEventArgs e) {
            ShowRegistationPage?.Invoke();
        }

        private void ForgottenPassword_Click(object sender, RoutedEventArgs e) {
            ResettingPasswordWindow forgotPasswordWindow = new ResettingPasswordWindow();
            forgotPasswordWindow.ShowDialog();
        }
    }
}
