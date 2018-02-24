using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfMath;

namespace The_Email_Client {
    /// <summary>
    /// Interaction logic for CalculusPage.xaml
    /// </summary>
    public partial class CalculusPage : Page
    {
        protected Action ShowHomePage { get; set; } //function to take user to home page
        protected Action ShowEmailPage { get; set; } //function to take user to Email page
        protected Equation Equ { get; set; } //function which stores the current equation in the program
        private int UsingFractions { get; set; } //variable which indicates which type of fraction user is using
        private string TypeofFunction { get; set; } //variable which indicated if user is user integration of differentiation
        private List<Equation> QuestionsforPDF { get; set; } //List to be used for pdf creation when user is creating a pdf
        private TexFormulaParser formulaParser;
        //Constuctor
        public CalculusPage(Action ShowHomePage, Action ShowEmailPage) {
            InitializeComponent();
            this.ShowHomePage = ShowHomePage; //initilises the function from the MainWindow.xaml.cs
            this.ShowEmailPage = ShowEmailPage; //initilises the function from the MainWindow.xaml.cs
            QuestionsforPDF = new List<Equation>(); //sets the list to a blank list
            Initiaize(); //sets up elements of the page for the user
        }
        //Code to set up Page whenever page is loaded
        public void Initiaize() {
            TypeofFunction = "diferentiation";
            PageSelectionComboBox.SelectedIndex = 0; 
            UpdateClassCombobox(); //Gets the classes from the database 
        }

        //takes the user to the home page
        private void HomeButton_Click(object sender, RoutedEventArgs e) {
            ShowHomePage();
        }

        //creates and displays a random equation to the user
        private void GenerateRandomEquationButton_Click(object sender, RoutedEventArgs e) {
            Equation Equ = CreateRandomEquation(); //creates a random equation according to the users perameters
            if (Equ != null) { //checks the equation was succesfully created 
                //Converts the equation to a latex image and displays it to the user
                formulaContainerElement.Visual = RenderLatex(Equ.EquationToLatex());
                //places the answer in a textbox that the user can look at if they wish to
                FprimeTextBlock.Text = Equ.FprimeEquationToString(); 
                AnswerBox.Clear(); //clears the users answer for the previous question
                //if the user is creating a pdf, will add the equation to the list of equations for said pdf
                if ((string)PdfButton.Content == "PDF Started!") QuestionsforPDF.Add(Equ);
            }
        }
        //Converts an equation to a latex render
        private DrawingVisual RenderLatex(string stringtoLatex) {
            //Initialise and create Latex parser
            TexFormulaParser.Initialize();
            formulaParser = new TexFormulaParser();
            //Convert string to latex formula
            TexFormula formula = formulaParser.Parse(stringtoLatex);
            // Render formula to visual.
            var visual = new DrawingVisual();
            var renderer = formula.GetRenderer(TexStyle.Display, 20d);
            var formulaSize = renderer.RenderSize;
            using (var drawingContext = visual.RenderOpen()) {
                renderer.Render(drawingContext, 0, 1);
            }
            return visual; //Returns latex render
        }

        //created a random equation according to the users perameters
        private Equation CreateRandomEquation() {
            if (!string.IsNullOrWhiteSpace(OrderBox.Text) && !string.IsNullOrWhiteSpace(MagnitudeBox.Text)) {
                switch (TypeofFunction) {
                    case "diferentiation": //creates an eqaution to be solved with diferentiation
                        Equ = new Diferentiation(Equation.CreateRandomEquation(Convert.ToInt16(OrderBox.Text)-1, Convert.ToInt16(MagnitudeBox.Text), UsingFractions));
                        break;
                    case "integration": //creates an eqaution to be solved with intergration
                        Equ = new Integration(Equation.CreateRandomEquation(Convert.ToInt16(OrderBox.Text)-1, Convert.ToInt16(MagnitudeBox.Text), UsingFractions));
                        break;
                }
                return Equ;
            }
            else { //error checking to prevent an equation being created with no perametors
                MessageBox.Show("Please Enter an order and difficulty value.", "Error!");
                return null;
            }
        }

        //creates a random pdf according to user perameters
        private void CreateRandomPDF(bool email) {
            if (Convert.ToInt16(NumofQuestionsBox.Text) > 0) { //Makes sure the user has requested at least 1 question
                List<Equation> EquList = new List<Equation>();
                for (int i = 0; i < Convert.ToInt16(NumofQuestionsBox.Text); i++) {
                    EquList.Add(CreateRandomEquation()); //Adds a random equation to the list 
                }
                PDF.CreatePDFfromList(EquList, email, (Class)ClassesCombobox.SelectedItem); //creates pdf
            }
        }

        //checks if what the user has submited is the same as the pre calculated answear
        private void SubmitButton_Click(object sender, RoutedEventArgs e) {
            if (!string.IsNullOrWhiteSpace(AnswerBox.Text)) {
                MathmaticsCorrectIncorrectWindow MathmaticsCorrectIncorrectWindow;
                //Gets values stating if the user is correct or incorrect
                UsersResult Correct = (Equ.VerifyAnswer(AnswerBox.Text));
                //informs the user if they are correct or incorrect
                switch (Correct) {
                    case UsersResult.Correct:
                        MathmaticsCorrectIncorrectWindow =
                            new MathmaticsCorrectIncorrectWindow(true);
                        MathmaticsCorrectIncorrectWindow.ShowDialog();
                        break;
                    case UsersResult.Incorrect:
                        MathmaticsCorrectIncorrectWindow =
                            new MathmaticsCorrectIncorrectWindow(false);
                        MathmaticsCorrectIncorrectWindow.ShowDialog();
                        break;
                    case UsersResult.Error:
                        //Ends function is users answer has an error in it
                        return;
                }
            }
        }
        
        //sets a value indicating which type of fractions the user wishes to use. This value is user later
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

        //allows the user to view the answer to their question and then hide it again
        private void ShowRevealButton_Click(object sender, RoutedEventArgs e) {
            switch ((string)Reveal_AnswerButton.Content) {
                case "Show Answer":
                    Reveal_AnswerButton.Content = "Hide Answer";
                    FprimeTextBlock.Visibility = Visibility.Visible;
                    break;
                case "Hide Answer":
                    Reveal_AnswerButton.Content = "Show Answer";
                    FprimeTextBlock.Visibility = Visibility.Hidden;
                    break;
            }
        }

        //increments values in each respective number box according to name and function. up/down - 1/2/3.
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

        //prevents the user from inputing numbers outside the feasable range set by me
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
                    e.CancelCommand();//Stops user pasting if paste is not only numbers
            }
            else 
                e.CancelCommand();//Stops user pasting if paste is not only numbers
        }
        
        private void PdfButton_Click(object sender, RoutedEventArgs e) {
            PdfButton.Content = "PDF Started!"; //Indicates user can now create a pdf with saved questions
            //Allows user to use buttons relavent to pdf creation
            CreateEmailPDFButton.IsEnabled = true; EndPdfButton.IsEnabled = true; CreatePDFButton.IsEnabled = true;
            PdfButton.IsEnabled = false; 
        }
        
        //cancels the current pdf the user was creating
        private void EndPdfButton_Click(object sender, RoutedEventArgs e) {
            PdfButton.Content = "Start PDF"; //Indicates user can start a new pdf
            QuestionsforPDF = new List<Equation>(); //Clears list of current equations
            ResetPdfButtons(); //resets pdf buttons to indicate a new pdf can be started
        }

        //resets pdf buttons to indicate a new pdf can be started
        private void ResetPdfButtons() {
            PdfButton.Content = "Start PDF"; //Indicated user can start another Pdf now
            //Stops user from using buttons relavent to pdf creation
            CreateEmailPDFButton.IsEnabled = false; EndPdfButton.IsEnabled = false; CreatePDFButton.IsEnabled = false;
            QuestionsforPDF = new List<Equation>(); //Clears current list
            PdfButton.IsEnabled = true; //enables user to start a new pdf
        }

        //Creates a random pdf from the users perameters
        private void CreateRanPDFButton_Click(object sender, RoutedEventArgs e) {
            CreateRandomPDF(false); //created a random pdf according to user specifications
        }

        //runs code depending upon values chosen by user in a combobox
        private void PageSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //if the page is not currently loaded to not run the function
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
                case "Email": //If Selected, will display the Email Page
                    ShowEmailPage();
                    break;
            }
        }
        
        //opens a new instance of the ProfilesWindow
        private void ProfileButton_Click(object sender, RoutedEventArgs e) {
            ProfilesWindow profilewindow = new ProfilesWindow();
            profilewindow.ShowDialog();
        }
        //opens a new instance of the ClassManagerWindow
        private void ClassButton_Click(object sender, RoutedEventArgs e) {
            ClassManagerWindow classmanagerwindow = new ClassManagerWindow();
            classmanagerwindow.ShowDialog();
            UpdateClassCombobox();
        }

        private void CreateRanPDFEmailButton_Click(object sender, RoutedEventArgs e) {
            CreateRandomPDF(true); //created a random pdf according to user specifications
        }

        private void CreateEmailPDFButton_Click(object sender, RoutedEventArgs e) {
            if (PDF.CreatePDFfromList(QuestionsforPDF, true, (Class)ClassesCombobox.SelectedItem)) { //Creates a pdf from the current list of equations 
                QuestionsforPDF = new List<Equation>(); //clears list of current questions
                ResetPdfButtons(); //resets pdf buttons to indicate a new pdf can be started
            }
        }

        private void CreatePDFButton_Click(object sender, RoutedEventArgs e) {
            if (QuestionsforPDF.Count > 0) {
                PDF.CreatePDFfromList(QuestionsforPDF, false, (Class)ClassesCombobox.SelectedItem); //Creates a pdf from the current list of equations 
                QuestionsforPDF = new List<Equation>(); //clears list of current questions
                ResetPdfButtons(); //resets pdf buttons to indicate a new pdf can be started
            }
            else //error message informing user
                MessageBox.Show("Please add at least one question to the PDF.", "Error!");
        }

        //selects all classes from the database and puts them into a combobox. 
        public void UpdateClassCombobox() {
            //sets up connection
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            List<Class> classes = new List<Class>();
            try {
                cnctDTB.Open();//opens connecction
                //sql command to retrieve all classes from the database
                OleDbCommand cmd = new OleDbCommand($"SELECT * FROM Classes;", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {//loops through each class
                    //adds the class to the list of classes
                    classes.Add(new Class { ID = Convert.ToInt16(Common.Cleanstr(reader[0])),
                        Name = Common.Cleanstr(reader[1]) });
                }
                //sets the list of items in the combobox to the list of classes
                ClassesCombobox.ItemsSource = classes;
            }
            //displays error to user if there is one
            catch (Exception err) { MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); }//closes connection
        }
    }
}
