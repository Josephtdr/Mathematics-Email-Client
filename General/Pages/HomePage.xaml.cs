using System;
using System.Windows;
using System.Windows.Controls;
namespace The_Email_Client {
    /// <summary>
    /// Page used to naviate the program and log out
    /// </summary>
    public partial class HomePage : Page {
        protected Action ShowCalculusPage { get; set; }
        protected Action ShowEmailPage { get; set; }
        protected Action ShowLoginPage { get; set; }
        //constructor
        public HomePage(Action ShowEmailPage, Action ShowLoginPage, Action ShowCalculusPage) {
            InitializeComponent();
            this.ShowEmailPage = ShowEmailPage;
            this.ShowLoginPage = ShowLoginPage;
            this.ShowCalculusPage = ShowCalculusPage;
        }
        //checks which button the user pressed and takes them to the respective page
        private void Button_Click(object sender, RoutedEventArgs e) {
            switch ((string)(((System.Windows.Controls.Button)sender).Content)) {
                case "Email":
                    ShowEmailPage();
                    break;
                case "Calculus":
                    ShowCalculusPage();
                    break;
            }
        }
        //button to log out the user
        private void LogOutButton_Click(object sender, RoutedEventArgs e) {
            Common.Profile = new Profiles();//deletes their local reccords
            ShowLoginPage();
        }
    }
}
