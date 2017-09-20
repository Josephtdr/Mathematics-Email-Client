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
        public MainWindow()
        {
            InitializeComponent();
            Common.AttachmentsSource = new List<OpenFileDialog>();
            Common.Attachments = new List<string>();
            Common.Profile = new Profiles();
            LoginPage = new LoginPage(ShowEmailPage, ShowRegistrationPage);
            EmailPage = new EmailPage(ShowLoginPage);
            RegistrationPage = new RegistrationPage(ShowLoginPage);

            ShowLoginPage();
        }

        public void ShowEmailPage()
        {
            //if (Common.Profile.UserName == "admin") Common.Profile.Defaults();
            Common.Profile.UpdateSettingsfromDB(Common.Profile, Common.Profile.UserName);
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
            PageFrame.Content = page;
        }
    }
}
