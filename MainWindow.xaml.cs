using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Text.RegularExpressions;
/// <summary>
/// This window is used to display the main pages of the program
/// it also provides functionality to switch between pages
/// </summary>
namespace The_Email_Client {
    public partial class MainWindow : Window {
        //Variables initialising each page 
        protected EmailPage EmailPage { get; set; }
        protected LoginPage LoginPage { get; set; }
        protected RegistrationPage RegistrationPage { get; set; }
        protected HomePage HomePage { get; set; }
        protected CalculusPage CalculusPage { get; set; }
        protected Page PreviousPage { get; set; }
        //Constuctor
        public MainWindow() {
            InitializeComponent();
            //Sets up local user profile
            Common.AttachmentsSource = new List<OpenFileDialog>();
            Common.Attachments = new List<string>();
            Common.Profile = new Profiles();
            //Initiliases pages with functions to travel from one to another
            HomePage = new HomePage(ShowEmailPage, ShowLoginPage, ShowCalculusPage);
            CalculusPage = new CalculusPage(ShowHomePage, ShowEmailPage);
            LoginPage = new LoginPage(LogintoHomePage, ShowRegistrationPage);
            EmailPage = new EmailPage(ShowPreviousPage, ShowHomePage);
            RegistrationPage = new RegistrationPage(ShowLoginPage);
            //displays the login page
            ShowLoginPage();
        }
        //Function to display the email page
        private void ShowEmailPage() {
            ShowPage(EmailPage);
        }
        //function to display the home page
        private void ShowHomePage() {
            ShowPage(HomePage);
        }
        //functions used to log into the home page
        private void LogintoHomePage() {
            //Loads the users profile from the database into the local equivalent
            Common.Profile.GetInfofromDB(Common.Profile, Common.Profile.UserName);
            //checks if the user signed in with a valid email
            if (String.IsNullOrWhiteSpace(Common.Profile.Email) || 
                !Regex.IsMatch(Common.Profile.Email, Constants.VALIDEMAILPATTERN, RegexOptions.IgnoreCase)) {
                //updates the users email and password within local variables to be default values
                //this is so they can still use email functionality
                Common.Profile.Email = Constants.DEFAULTEMAIL;
                Common.Profile.Password = Constants.DEFAULTPASSWORD;
            }
            ShowPage(HomePage);
        }
        //function to display the calculus page
        private void ShowCalculusPage() {
            //inisitalised the calculus page such that it is ready for the user
            CalculusPage.Initiaize();
            ShowPage(CalculusPage);
        }
        //function to display the registration page
        private void ShowRegistrationPage() {
            ShowPage(RegistrationPage);
        }
        //function to display the login page
        private void ShowLoginPage() {
            ShowPage(LoginPage);
        }
        //function used to display the last page
        private void ShowPreviousPage() {
            switch (PreviousPage.GetType().ToString()) {
                case "CalculusPage":
                    CalculusPage.Initiaize();
                    break;
            }
            ShowPage(PreviousPage);
        }
        //function used to display a new page
        private void ShowPage(Page page) {
            PreviousPage = (Page)PageFrame.Content;
            PageFrame.Content = page;
        }
        //Moves the window to the center of the screen when page changes
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) {
            this.Left = (SystemParameters.WorkArea.Width - Width) / 2 + SystemParameters.WorkArea.Left;
            this.Top = (SystemParameters.WorkArea.Height - Height) / 2 + SystemParameters.WorkArea.Top;
        }
    }
}
