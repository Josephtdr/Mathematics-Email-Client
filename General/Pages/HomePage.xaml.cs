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
namespace The_Email_Client {
    public partial class HomePage : Page {
        protected Action ShowCalculusPage { get; set; }
        protected Action ShowEmailPage { get; set; }
        protected Action ShowLoginPage { get; set; }
        public HomePage(Action ShowEmailPage, Action ShowLoginPage, Action ShowCalculusPage) {
            InitializeComponent();
            this.ShowEmailPage = ShowEmailPage;
            this.ShowLoginPage = ShowLoginPage;
            this.ShowCalculusPage = ShowCalculusPage;
        }

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

        private void LogOutButton_Click(object sender, RoutedEventArgs e) {
            Common.Profile = new Profiles();
            ShowLoginPage();
        }
    }
}
