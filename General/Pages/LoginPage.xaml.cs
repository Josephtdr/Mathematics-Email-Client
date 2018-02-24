using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace The_Email_Client {
    /// <summary>
    /// Page used to log into the program
    /// </summary>
    public partial class LoginPage : Page {
        protected Action ShowHomePage { get; set; }
        protected Action ShowRegistationPage { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public LoginPage(Action ShowHomePage, Action ShowRegistationPage) {
            this.ShowHomePage = ShowHomePage;
            this.ShowRegistationPage = ShowRegistationPage;
            InitializeComponent();
            DataContext = this; //enables databinding

            //alows the user to login with the enter key
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Enter))
                { VerifyLogIn(); }
            };
        }
        //function to log into the program
        private void VerifyLogIn() {
            Password = Passwordbox.Password; UserName = UserNameTextBox.Text; Email = EmailTextBox.Text;
            //checks if the users password is correct
            if (Encryption.VerifyPasswordorEmail(UserName, Password, true)) {
                //checks if the user entered an email
                if (String.IsNullOrWhiteSpace(Email)) {
                    UserNameTextBox.Clear();
                    ShowHomePage?.Invoke();
                }
                //checks if the users email is correct
                else if (Encryption.VerifyPasswordorEmail(UserName, Email, false)) {
                    UserNameTextBox.Clear(); EmailTextBox.Clear();
                    ShowHomePage?.Invoke();
                }
            }
        }
        //button to log into the program
        private void SubmitButton_Click(object sender, RoutedEventArgs e) {
            VerifyLogIn();
        }
        //button to open the registration page
        private void Registrationbutton_Click(object sender, RoutedEventArgs e) {
            ShowRegistationPage?.Invoke();
        }
        //button to open the forgotten password window 
        private void ForgottenPassword_Click(object sender, RoutedEventArgs e) {
            ResettingPasswordWindow forgotPasswordWindow = new ResettingPasswordWindow();
            forgotPasswordWindow.ShowDialog();
        }
    }
}
