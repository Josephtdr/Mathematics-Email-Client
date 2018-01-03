using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Globalization;
using PdfSharp;

namespace The_Email_Client
{
    /// <summary>
    /// Interaction logic for CalculusPage.xaml
    /// </summary>
    public partial class CalculusPage : Page
    {
        protected Action ShowHomePage { get; set; }
        protected Action ShowIndiciesPage { get; set; }
        protected Action ShowEmailPage { get; set; }
        protected Equation Equ { get; set; }
        private int UsingFractions { get; set; }
        private string TypeofFunction { get; set; }
        private float[] X { get; set; }
        private List<Equation> QuestionsforPDF { get; set; }

        public CalculusPage(Action ShowHomePage, Action ShowIndiciesPage, Action ShowEmailPage) {
            InitializeComponent();
            QuestionsforPDF = new List<Equation>();
            this.ShowHomePage = ShowHomePage;
            this.ShowEmailPage = ShowEmailPage;
            this.ShowIndiciesPage = ShowIndiciesPage;
        }
        
        private void HomeButton_Click(object sender, RoutedEventArgs e) {
            ShowHomePage();
        }

        private void GenerateRandomEquationButton_Click(object sender, RoutedEventArgs e) {
            Equation Equ = CreateRandomEquation();
            if (Equ != null) {
                EquationTextBlock.Text = Equ.ToString();
                DifferentiationTextBlock.Text = Equ.FprimeEquationToString();
                AnswerBox.Clear();
                if((string)PdfButton.Content == "Create PDF") QuestionsforPDF.Add(Equ);
            }
        }

        private Equation CreateRandomEquation() {
            if (!string.IsNullOrWhiteSpace(OrderBox.Text) && !string.IsNullOrWhiteSpace(MagnitudeBox.Text)) {
                switch (TypeofFunction) {
                    case "diferentiation":
                        Equ = new Diferentiation(Convert.ToInt16(OrderBox.Text), Convert.ToInt16(MagnitudeBox.Text), UsingFractions);
                        break;
                    case "integration":
                        Equ = new Integration(Convert.ToInt16(OrderBox.Text), Convert.ToInt16(MagnitudeBox.Text), UsingFractions);
                        break;
                }
                return Equ;
            }
            else {
                MessageBox.Show("Please Enter an order and difficulty value.", "Error!");
                return null;
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e) {
            if (!string.IsNullOrWhiteSpace(AnswerBox.Text)) { 
                MathmaticsCorrectIncorrectWindow MathmaticsCorrectIncorrectWindow;
                if (Equ.VerifyAnswer(AnswerBox.Text)) {
                    Random rnd = new Random();
                    MathmaticsCorrectIncorrectWindow = new MathmaticsCorrectIncorrectWindow(true);
                    MathmaticsCorrectIncorrectWindow.ShowDialog();
                }
                else {
                    MathmaticsCorrectIncorrectWindow = new MathmaticsCorrectIncorrectWindow(false);
                    MathmaticsCorrectIncorrectWindow.ShowDialog();
                }
            }
        }
        
        private void FractionsUseHandleCheck(object sender, RoutedEventArgs e) {
            RadioButton rb = sender as RadioButton;
            switch (rb.Name) {
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

        private void ShowRevealButton_Click(object sender, RoutedEventArgs e) {
            switch ((string)Reveal_AnswerButton.Content) {
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

        private void Cmd_Click(object sender, RoutedEventArgs e) {
            switch (((Button)sender).Name) {
                case "cmdUp":
                    OrderBox.Text = Convert.ToString(Convert.ToInt16(OrderBox.Text) + 1);
                    break;
                case "cmdUp2":
                    MagnitudeBox.Text = Convert.ToString(Convert.ToInt16(MagnitudeBox.Text) + 1);
                    break;
                case "cmdUp3":
                    NumofQuestionsBox.Text = Convert.ToString(Convert.ToInt16(NumofQuestionsBox.Text) + 1);
                    break;
                case "cmdDown":
                    OrderBox.Text = Convert.ToString(Convert.ToInt16(OrderBox.Text) - 1);
                    break;
                case "cmdDown2":
                    MagnitudeBox.Text = Convert.ToString(Convert.ToInt16(MagnitudeBox.Text) - 1);
                    break;
                case "cmdDown3":
                    NumofQuestionsBox.Text = Convert.ToString(Convert.ToInt16(NumofQuestionsBox.Text) - 1);
                    break;
            }
        }

        private void NumberBoxes_TextChanged(object sender, TextChangedEventArgs e) {
            switch (((TextBox)sender).Name) {
                case "OrderBox":
                    if (!string.IsNullOrWhiteSpace(OrderBox.Text))
                        OrderBox.Text = Convert.ToInt16(OrderBox.Text) >= 10 ? "10" : Convert.ToInt16(OrderBox.Text) <= 0 ? "0" : OrderBox.Text;
                    break;
                case "MagnitudeBox":
                    if (!string.IsNullOrWhiteSpace(MagnitudeBox.Text))
                        MagnitudeBox.Text = Convert.ToInt16(MagnitudeBox.Text) >= 25 ? "25" : Convert.ToInt16(MagnitudeBox.Text) <= 0 ? "0" : MagnitudeBox.Text;
                    break;
            }
        }

        
        //Prevents user from typing anything bar numbers, x and operators +,-,^,/ in text boxes this check is applied to.
        private void AnswerBox_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9-/+/^/x]+"); //regex that matches disallowed text
            e.Handled = regex.IsMatch(e.Text);
        }
        //Prevents users from typing anything bar numbers in text boxes this check is applied to.
        private void NumberTextbox_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            e.Handled = regex.IsMatch(e.Text);
        }
        //Prevents users from pasting anything bar numbers into text boxes this check is applied to.
        private void TextBoxPasting(object sender, DataObjectPastingEventArgs e) {
            Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            if (e.DataObject.GetDataPresent(typeof(String))) {
                String text = (String)e.DataObject.GetData(typeof(String));
                if(regex.IsMatch(text)) 
                    e.CancelCommand();
            }
            else 
                e.CancelCommand();
        }
        
        private void PdfButton_Click(object sender, RoutedEventArgs e) {
            switch ((string)PdfButton.Content) {
                case "Start PDF":
                    PdfButton.Content = "Create PDF"; //Indicates user can now create a pdf with saved questions
                    break;
                case "Create PDF":
                    CreatePDFfromList(QuestionsforPDF);
                    QuestionsforPDF = new List<Equation>(); //clears list of current questions
                    PdfButton.Content = "Start PDF"; //Indicated user can start another Pdf now
                    break;
            }
        }
        //Function to create a PDF from a list of Equations
        private void CreatePDFfromList(List<Equation> EquList) {
            PdfDocument document = new PdfDocument();
            PdfPage page;
            XGraphics gfx;
            XFont font = new XFont("Verdana", 20, XFontStyle.Bold); //sets chosen font and style
            int numpages = EquList.Count / 14; //Workout the number of pages which will be required for the number of given questions
            //Creates Question Pages
            for (int i = 0; i < numpages + 1; i++) {
                page = document.AddPage();
                gfx = XGraphics.FromPdfPage(page);
                if (i == 0) gfx.DrawString("Questions:", font, XBrushes.Black, new XRect(0, 10, page.Width, page.Height), XStringFormat.TopCenter);
                for (int j = 0; j < 14; j++) {
                    if (j + (14 * i) < EquList.Count)
                        gfx.DrawString($"{j + 1 + (14 * i)}.\t { Regex.Replace(EquList[j + (14 * i)].ToString(), "[^0-9-/+/^/x ]+", "") }",
                            font, XBrushes.Black, new XRect(10, 10 + ((j + 2) * 50), page.Width, page.Height), XStringFormat.TopLeft);
                }
            }
            //Creates Answer Pages
            for (int i = 0; i < numpages + 1; i++) {
                page = document.AddPage();
                gfx = XGraphics.FromPdfPage(page);
                if (i == 0) gfx.DrawString("Answers:", font, XBrushes.Black, new XRect(0, 10, page.Width, page.Height), XStringFormat.TopCenter);
                for (int j = 0; j < 14; j++) {
                    if (j + (14 * i) < EquList.Count)
                        gfx.DrawString($"{j + 1 + (14 * i)}.\t { Regex.Replace(EquList[j + (14 * i)].FprimeEquationToString(), "[^0-9-/+/^/x ]+", "") }",
                            font, XBrushes.Black, new XRect(10, 10 + ((j + 2) * 50), page.Width, page.Height), XStringFormat.TopLeft);
                }
            }

            string datetime = DateTime.Now.ToString().Replace('/', '-').Replace(':', '.');
            string filename = $"{datetime}.pdf";
            document.Save(filename);
            Process.Start(filename);
        }

        private void EndPdfButton_Click(object sender, RoutedEventArgs e) {
            PdfButton.Content = "Start PDF"; //Indicates user can start a new pdf
            QuestionsforPDF = new List<Equation>(); //Clears list of current equations
        }

        //Creates a random pdf from the users perameters
        private void CreateRanPDFButton_Click(object sender, RoutedEventArgs e) {
            if (Convert.ToInt16(NumofQuestionsBox.Text) > 0) { //Makes sure the user has requested at least 1 question
                List<Equation> EquList = new List<Equation>();
                for (int i = 0; i < Convert.ToInt16(NumofQuestionsBox.Text); i++) {
                    EquList.Add(CreateRandomEquation()); //Adds a random equation to the list 
                }
                CreatePDFfromList(EquList); //creates pdf
            }
        }

        private void PageSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!IsLoaded) {
                return;
            }
            switch (PageSelectionComboBox.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last()) {
                case "Differentiation": //If selected, functions created will be solved with diferentiation
                    TypeofFunction = "diferentiation";
                    break;
                case "Intergration": //If selected, functions created will be solved with intergration
                    TypeofFunction = "integration";
                    break;
                case "Indicies": //If selected, will display the Indicies Page
                    ShowIndiciesPage();
                    break;
                case "Email": //If Selected, will display the Email Page
                    ShowEmailPage();
                    break;
            }
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e) {
            ProfilesWindow profilewindow = new ProfilesWindow();
            profilewindow.ShowDialog();
        }
    }
}
