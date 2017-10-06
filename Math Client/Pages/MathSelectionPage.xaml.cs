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
    /// Interaction logic for MathSelectionPage.xaml
    /// </summary>
    public partial class MathSelectionPage : Page
    {
        protected Action ShowDifferentiationPage { get; set; }
        protected Action ShowIntegrationPage { get; set; }
        protected Action ShowAdditionPage { get; set; }
        protected Action ShowPreviousPage { get; set; }
        public MathSelectionPage(Action ShowDifferentiationPage, Action ShowIntegrationPage, Action ShowAdditionPage, Action ShowPreviousPage)
        {
            this.ShowAdditionPage = ShowAdditionPage;
            this.ShowIntegrationPage = ShowIntegrationPage;
            this.ShowDifferentiationPage = ShowDifferentiationPage;
            this.ShowPreviousPage = ShowPreviousPage;
            InitializeComponent();
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ShowPreviousPage();
        }
        private void MathButton_Click(object sender, RoutedEventArgs e)
        {
            switch ((string)(((System.Windows.Controls.Button)sender).Content))
            {
                case "Differentiation":
                    ShowDifferentiationPage();
                    break;
                case "Integration":
                    ShowIntegrationPage();
                    break;
                case "Addition":
                    ShowAdditionPage();
                    break;
            }
        }
    }
}
