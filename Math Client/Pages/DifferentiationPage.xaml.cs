using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for DifferentiationPage.xaml
    /// </summary>
    public partial class DifferentiationPage : Page
    {
        protected Action ShowHomePage { get; set; }
        protected Equation Dif { get; set; }
        public DifferentiationPage(Action ShowHomePage)
        {
            InitializeComponent();
            this.ShowHomePage = ShowHomePage;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            WebBrowser.Navigate("https://www.wolframalpha.com/input/?i=derivative");
        }

        private void WebBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ShowHomePage();
        }

        private void GenerateRandomEquationButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(OrderBox.Text) && !string.IsNullOrWhiteSpace(DifficultyBox.Text))
            {
                Dif = new Diferentiation(Convert.ToInt16(OrderBox.Text), Convert.ToInt16(DifficultyBox.Text)); 
                EquationLabel.Content = Dif;
                DifferentiationLabel.Content = Dif.SolvedEquationToString();
            }
            else MessageBox.Show("Please Enter an order and difficulty rating", "Error!");
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (Dif.VerifyAnswer(AnswerBox.Text)) MessageBox.Show("Correct btw.", "Well Done!");
            else MessageBox.Show("incorrect btw.", "Not well Done!");
        }

        private void Textbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }
    }
}
