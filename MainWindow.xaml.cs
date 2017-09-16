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
        protected Page PreviousPage { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            LoginPage = new LoginPage(ShowEmailPage, ShowRegistrationPage);
            EmailPage = new EmailPage(ShowLoginPage);
            RegistrationPage = new RegistrationPage(ShowLoginPage);

            ShowLoginPage();
        }

        public void ShowEmailPage()
        {
            Common.Email = LoginPage.Email;
            EmailPage.GetEmail(Common.Email);
            ShowPage(EmailPage);
        }

        public void ShowRegistrationPage()
        {
            ShowPage(RegistrationPage);
        }

        public void ShowLoginPage()
        {
            ShowPage(LoginPage);
        }

        protected void ShowPage(Page page)
        {
            PreviousPage = (Page)PageFrame.Content;
            PageFrame.Content = page;
        }
    }
}
