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
            else MessageBox.Show("Please Enter an order and difficulty value.", "Error!");
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (Dif.VerifyAnswer(AnswerBox.Text)) MessageBox.Show("Correct btw.", "Well Done!");
            else MessageBox.Show("incorrect btw.", "Not well Done!");
        }

        private void Textbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Common.IsTextAllowed(e.Text);
        }
        
    }
}
