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

namespace The_Email_Client
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        protected Action ShowMathSelectionPage { get; set; }
        protected Action ShowEmailPage { get; set; }
        protected Action ShowLoginPage { get; set; }
        public HomePage(Action ShowMathSelectionPage, Action ShowEmailPage, Action ShowLoginPage)
        {
            InitializeComponent();
            this.ShowEmailPage = ShowEmailPage;
            this.ShowMathSelectionPage = ShowMathSelectionPage;
            this.ShowLoginPage = ShowLoginPage;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch ((string)(((System.Windows.Controls.Button)sender).Content))
            {
                case "Email":
                    if (String.IsNullOrWhiteSpace(Common.Profile.Email) || !Common.Inccorectemailformat(Common.Profile.Email)) {
                        Common.Profile.Email = Constants.DEFAULTEMAIL;
                        Common.Profile.Password = Constants.DEFAULTPASSWORD;
                    }
                    ShowEmailPage();
                    break;
                case "Maths!":
                    ShowMathSelectionPage();
                    break;
            }
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            Common.Profile = new Profiles();
            ShowLoginPage();
        }
    }
}
