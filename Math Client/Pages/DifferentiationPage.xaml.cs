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
        protected Equation Equ { get; set; }
        private int UsingFractions { get; set; }
        private string TypeofFunction { get; set; }
        private float[] x { get; set; }

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
            if (!string.IsNullOrWhiteSpace(OrderBox.Text) && !string.IsNullOrWhiteSpace(MagnitudeBox.Text))
            {
                switch (TypeofFunction)
                {
                    case "diferentiation":
                        Equ = new Diferentiation(Convert.ToInt16(OrderBox.Text), Convert.ToInt16(MagnitudeBox.Text), UsingFractions);
                        break;
                    case "integration":
                        Equ = new Integration(Convert.ToInt16(OrderBox.Text), Convert.ToInt16(MagnitudeBox.Text), UsingFractions);
                        break;
                }
                EquationTextBlock.Text = Equ.ToString();
                DifferentiationTextBlock.Text = Equ.SolvedEquationToString();
                AnswerBox.Clear(); FanswerBox.Clear();
                FanswerBox.Visibility = Visibility.Hidden; FsubmitButton.Visibility = Visibility.Hidden;
                FtextBlock.Visibility = Visibility.Hidden;
            }
            else MessageBox.Show("Please Enter an order and difficulty value.", "Error!");
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            MathmaticsCorrectIncorrectWindow MathmaticsCorrectIncorrectWindow;
            if (Equ.VerifyAnswer(AnswerBox.Text))
            {
                Random rnd = new Random();
                MathmaticsCorrectIncorrectWindow = new MathmaticsCorrectIncorrectWindow(true);
                MathmaticsCorrectIncorrectWindow.ShowDialog();
                FanswerBox.Visibility = Visibility.Visible; FsubmitButton.Visibility = Visibility.Visible;
                FtextBlock.Visibility = Visibility.Visible;
                switch (TypeofFunction)
                {
                    case "diferentiation":
                        x = new float[] { (rnd.Next(-10, 10)) }; 
                        FtextBlock.Text = $"Find the gradient of the curve f(x) at the point x={x[0]}.";
                        break;
                    case "integration":
                        float x1 = (rnd.Next(-10, 10)); float x2 = (rnd.Next(-10, 10));
                        x1 = x1 == x2 ? x1 + 1 : x1;
                        x = new float[] { x1 , x2 };
                        FtextBlock.Text = $"Find the are bound by the curve between x={x[0]} and x={x[1]}. This Doesnt work btw lul.";
                        break;
                }
            }
            else {
                MathmaticsCorrectIncorrectWindow = new MathmaticsCorrectIncorrectWindow(false);
                MathmaticsCorrectIncorrectWindow.ShowDialog();
            }
        }
        
        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            switch (rb.Name)
            {
                case "no":
                    UsingFractions = 0;
                    break;
                case "half":
                    UsingFractions = 1;
                    break;
                case "full":
                    UsingFractions = 2;
                    break;
            }
        }

        private void ShowRevealButton_Click(object sender, RoutedEventArgs e)
        {
            switch ((string)Reveal_AnswerButton.Content)
            {
                case "Show Answer":
                    Reveal_AnswerButton.Content = "Hide Answer";
                    DifferentiationTextBlock.Visibility = Visibility.Visible;
                    break;
                case "Hide Answer":
                    Reveal_AnswerButton.Content = "Show Answer";
                    DifferentiationTextBlock.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void TypeHandleChecked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            switch (rb.Name)
            {
                case "diferentiation":
                    TypeofFunction = "diferentiation";
                    break;
                case "integration":
                    TypeofFunction = "integration";
                    break;
            }
        }

        private void Cmd_Click(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "cmdUp":
                    OrderBox.Text = Convert.ToString(Convert.ToInt16(OrderBox.Text) + 1);
                    break;
                case "cmdUp2":
                    MagnitudeBox.Text = Convert.ToString(Convert.ToInt16(MagnitudeBox.Text) + 1);
                    break;
                case "cmdDown":
                    OrderBox.Text = Convert.ToString(Convert.ToInt16(OrderBox.Text) - 1);
                    break;
                case "cmdDown2":
                    MagnitudeBox.Text = Convert.ToString(Convert.ToInt16(MagnitudeBox.Text) - 1);
                    break;
            }
            
        }

        private void NumberBoxes_TextChanged(object sender, TextChangedEventArgs e)
        {
            switch (((TextBox)sender).Name)
            {
                case "OrderBox":
                    if (!string.IsNullOrWhiteSpace(OrderBox.Text))
                        OrderBox.Text = Convert.ToInt16(OrderBox.Text) >= 99 ? "99" : Convert.ToInt16(OrderBox.Text) <= 0 ? "0" : OrderBox.Text;
                    break;
                case "MagnitudeBox":
                    if (!string.IsNullOrWhiteSpace(MagnitudeBox.Text))
                        MagnitudeBox.Text = Convert.ToInt16(MagnitudeBox.Text) >= 99 ? "99" : Convert.ToInt16(MagnitudeBox.Text) <= 0 ? "0" : MagnitudeBox.Text;
                    break;
            }
            
        }

        private void FsubmitButton_Click(object sender, RoutedEventArgs e)
        {
            MathmaticsCorrectIncorrectWindow MathmaticsCorrectIncorrectWindow;
            switch (TypeofFunction) {
                case "diferentiation":
                    if (Convert.ToInt16(FanswerBox.Text) == Convert.ToInt16(Equ.CalculateAnswer(x[0]))) {
                        MathmaticsCorrectIncorrectWindow = new MathmaticsCorrectIncorrectWindow(true);
                        MathmaticsCorrectIncorrectWindow.ShowDialog();
                    }
                    else {
                        MathmaticsCorrectIncorrectWindow = new MathmaticsCorrectIncorrectWindow(false);
                        MathmaticsCorrectIncorrectWindow.ShowDialog();
                    }
                    break;
                case "integration":
                    if (Convert.ToInt16(FanswerBox.Text) == Convert.ToInt16(Equ.CalculateAnswer(x[0], x[1]))) {
                        MathmaticsCorrectIncorrectWindow = new MathmaticsCorrectIncorrectWindow(true);
                        MathmaticsCorrectIncorrectWindow.ShowDialog();
                    }
                    else {
                        MathmaticsCorrectIncorrectWindow = new MathmaticsCorrectIncorrectWindow(false);
                        MathmaticsCorrectIncorrectWindow.ShowDialog();
                    }
                    break;
            }
        }

        private void AnswerBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9-/+/^/x]+"); //regex that matches disallowed text
            e.Handled = regex.IsMatch(e.Text);
        }
        
        private void NumberTextbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //e.Handled = !Common.IsTextAllowed(e.Text);
            Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            e.Handled = regex.IsMatch(e.Text);

        }
    }
}
