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
        protected IntegrationPage IntegrationPage { get; set; }
        protected DifferentiationPage DifferentiationPage { get; set; }
        protected AdditionPage AdditionPage { get; set; }
        protected MathSelectionPage MathSelectionPage { get; set; }
        protected Page PreviousPage { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Common.AttachmentsSource = new List<OpenFileDialog>();
            Common.Attachments = new List<string>();
            Common.Profile = new Profiles();
            HomePage = new HomePage(ShowMathSelectionPage, ShowEmailPage, ShowLoginPage);
            IntegrationPage = new IntegrationPage(ShowPreviousPage);
            DifferentiationPage = new DifferentiationPage(ShowPreviousPage);
            AdditionPage = new AdditionPage(ShowPreviousPage);
            LoginPage = new LoginPage(LogintoHomePage, ShowRegistrationPage);
            EmailPage = new EmailPage(ShowPreviousPage);
            RegistrationPage = new RegistrationPage(ShowLoginPage);
            MathSelectionPage = new MathSelectionPage(ShowDifferentiationPage, ShowIntegrationPage, ShowAdditionPage, ShowHomePage);
            ShowLoginPage();
        }

        public void ShowEmailPage()
        {
            ShowPage(EmailPage);
        }
        public void ShowMathSelectionPage()
        {
            ShowPage(MathSelectionPage);
        }
        public void ShowHomePage()
        {
            ShowPage(HomePage);
        }
        public void LogintoHomePage()
        {
            Common.Profile.GetInfofromDB(Common.Profile, Common.Profile.UserName);
            ShowPage(HomePage);
        }
        public void ShowIntegrationPage()
        {
            ShowPage(IntegrationPage);
        }
        public void ShowDifferentiationPage()
        {
            ShowPage(DifferentiationPage);
        }
        public void ShowAdditionPage()
        {
            ShowPage(AdditionPage);
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
