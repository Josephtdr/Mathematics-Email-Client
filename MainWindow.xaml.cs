using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Controls;

namespace The_Email_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        protected EmailPage EmailPage { get; set; }
        protected LoginPage LoginPage { get; set; }
        protected RegistrationPage RegistrationPage { get; set; }
        protected HomePage HomePage { get; set; }
        protected CalculusPage CalculusPage { get; set; }
        protected IndiciesPage IndiciesPage { get; set; }
        protected Page PreviousPage { get; set; }
        public MainWindow() {
            InitializeComponent();
            Common.AttachmentsSource = new List<OpenFileDialog>();
            Common.Attachments = new List<string>();
            Common.Profile = new Profiles();
            HomePage = new HomePage(ShowEmailPage, ShowLoginPage, ShowCalculusPage, ShowIndiciesPage);
            CalculusPage = new CalculusPage(ShowHomePage, ShowIndiciesPage, ShowEmailPage);
            IndiciesPage = new IndiciesPage(ShowHomePage, ShowCalculusPage, ShowEmailPage);
            LoginPage = new LoginPage(LogintoHomePage, ShowRegistrationPage);
            EmailPage = new EmailPage(ShowPreviousPage, ShowHomePage);
            RegistrationPage = new RegistrationPage(ShowLoginPage);
            ShowLoginPage();
        }
        private void ShowIndiciesPage() {
            IndiciesPage.Initialize();
            ShowPage(IndiciesPage);
        }
        private void ShowEmailPage() {
            ShowPage(EmailPage);
        }
        private void ShowHomePage() {
            ShowPage(HomePage);
        }
        private void LogintoHomePage() {
            Common.Profile.GetInfofromDB(Common.Profile, Common.Profile.UserName);
            if (String.IsNullOrWhiteSpace(Common.Profile.Email) || !Common.Inccorectemailformat(Common.Profile.Email)) {
                Common.Profile.Email = Constants.DEFAULTEMAIL;
                Common.Profile.Password = Constants.DEFAULTPASSWORD;
            }
            ShowPage(HomePage);
        }
        private void ShowCalculusPage() {
            CalculusPage.Initiaize();
            ShowPage(CalculusPage);
        }
        private void ShowRegistrationPage() {
            ShowPage(RegistrationPage);
        }
        private void ShowLoginPage() {
            ShowPage(LoginPage);
        }
        private void ShowPreviousPage() {
            switch (PreviousPage.GetType().ToString()) {
                case "CalculusPage":
                    CalculusPage.Initiaize();
                    break;
                case "IndiciesPage":
                    IndiciesPage.Initialize();
                    break;
            }
            ShowPage(PreviousPage);
        }
        private void ShowPage(Page page) {
            PreviousPage = (Page)PageFrame.Content;
            PageFrame.Content = page;
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) {
            this.Left = (SystemParameters.WorkArea.Width - Width) / 2 + SystemParameters.WorkArea.Left;
            this.Top = (SystemParameters.WorkArea.Height - Height) / 2 + SystemParameters.WorkArea.Top;
        }
    }
}
