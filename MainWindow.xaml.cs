using System;
using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Net.Mime;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace The_Email_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected EmailPage EmailPage { get; set; }
        protected LoginPage LoginPage { get; set; }
        protected RegistrationPage RegistrationPage { get; set; }
        protected HomePage HomePage { get; set; }
        protected CalculusPage CalculusPage { get; set; }
        protected IndiciesPage IndiciesPage { get; set; }
        protected Page PreviousPage { get; set; }
        public MainWindow()
        {
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
        public void ShowIndiciesPage() {
            ShowPage(IndiciesPage);
        }
        public void ShowEmailPage() {
            ShowPage(EmailPage);
        }
        public void ShowHomePage()
        {
            ShowPage(HomePage);
        }
        public void LogintoHomePage()
        {
            Common.Profile.GetInfofromDB(Common.Profile, Common.Profile.UserName);
            if (String.IsNullOrWhiteSpace(Common.Profile.Email) || !Common.Inccorectemailformat(Common.Profile.Email)) {
                Common.Profile.Email = Constants.DEFAULTEMAIL;
                Common.Profile.Password = Constants.DEFAULTPASSWORD;
            }
            ShowPage(HomePage);
        }
        public void ShowCalculusPage()
        {
            ShowPage(CalculusPage);
        }
        public void ShowRegistrationPage()
        {
            ShowPage(RegistrationPage);
        }
        public void ShowLoginPage()
        {
            ShowPage(LoginPage);
        }
        public void ShowPreviousPage()
        {
            ShowPage(PreviousPage);
        }
        protected void ShowPage(Page page)
        {
            PreviousPage = (Page)PageFrame.Content;
            PageFrame.Content = page;
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Left = (SystemParameters.WorkArea.Width - Width) / 2 + SystemParameters.WorkArea.Left;
            this.Top = (SystemParameters.WorkArea.Height - Height) / 2 + SystemParameters.WorkArea.Top;
        }
    }
}
