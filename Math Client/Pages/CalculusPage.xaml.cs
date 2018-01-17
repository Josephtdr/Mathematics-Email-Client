using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace The_Email_Client
{
    /// <summary>
    /// Interaction logic for CalculusPage.xaml
    /// </summary>
    public partial class CalculusPage : Page
    {
        protected Action ShowHomePage { get; set; } //function to take user to home page
        protected Action ShowIndiciesPage { get; set; } //function to take user to Indicies page
        protected Action ShowEmailPage { get; set; } //function to take user to Email page
        protected Equation Equ { get; set; }
        private int UsingFractions { get; set; } //variable which indicates which type of fraction user is using
        private string TypeofFunction { get; set; } //variable which indicated if user is user integration of differentiation
        private float[] X { get; set; } 
        private List<Equation> QuestionsforPDF { get; set; } //List to be used for pdf creation when user is creating a pdf

        public CalculusPage(Action ShowHomePage, Action ShowIndiciesPage, Action ShowEmailPage) {
            InitializeComponent();
            this.ShowHomePage = ShowHomePage; //initilises the function from the MainWindow.xaml.cs
            this.ShowEmailPage = ShowEmailPage; //initilises the function from the MainWindow.xaml.cs
            this.ShowIndiciesPage = ShowIndiciesPage; //initilises the function from the MainWindow.xaml.cs
            QuestionsforPDF = new List<Equation>(); //sets the list to a blank list
            Initiaize();
        }

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
                EquationTextBlock.Text = Equ.ToString(); //shows the question is a text box so the user can see it
                FprimeTextBlock.Text = Equ.FprimeEquationToString(); //places the answer in a textbox that the user can look at if they wish to
                AnswerBox.Clear(); //clears the users answer for the previous question
                //if the user is creating a pdf, will add the equation to the list of equations for said pdf
                if ((string)PdfButton.Content == "PDF Started!") QuestionsforPDF.Add(Equ); 
            }
        }

        //created a random equation according to the users perameters
        private Equation CreateRandomEquation() {
            if (!string.IsNullOrWhiteSpace(OrderBox.Text) && !string.IsNullOrWhiteSpace(MagnitudeBox.Text)) {
                switch (TypeofFunction) {
                    case "diferentiation": //creates an eqaution to be solved with diferentiation
                        Equ = new Diferentiation(Equation.CreateRandomEquation(Convert.ToInt16(OrderBox.Text), Convert.ToInt16(MagnitudeBox.Text), UsingFractions));
                        break;
                    case "integration": //creates an eqaution to be solved with intergration
                        Equ = new Integration(Equation.CreateRandomEquation(Convert.ToInt16(OrderBox.Text), Convert.ToInt16(MagnitudeBox.Text), UsingFractions));
                        break;
                }
                return Equ;
            }
            else { //error checking to prevent an equation being created with no perametors
                MessageBox.Show("Please Enter an order and difficulty value.", "Error!");
                return null;
            }
        }

        //checks if what the user has submited is the same as the pre calculated answear
        private void SubmitButton_Click(object sender, RoutedEventArgs e) {
            if (!string.IsNullOrWhiteSpace(AnswerBox.Text)) { 
                MathmaticsCorrectIncorrectWindow MathmaticsCorrectIncorrectWindow;
                if (Equ.VerifyAnswer(AnswerBox.Text)) {
                    //informs the user they are correct
                    MathmaticsCorrectIncorrectWindow = new MathmaticsCorrectIncorrectWindow(true);
                    MathmaticsCorrectIncorrectWindow.ShowDialog();
                }
                else {
                    //informs the user they are incorrect
                    MathmaticsCorrectIncorrectWindow = new MathmaticsCorrectIncorrectWindow(false);
                    MathmaticsCorrectIncorrectWindow.ShowDialog();
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
                    e.CancelCommand();
            }
            else 
                e.CancelCommand();
        }
        
        private void PdfButton_Click(object sender, RoutedEventArgs e) {
            PdfButton.Content = "PDF Started!"; //Indicates user can now create a pdf with saved questions
            //Allows user to use buttons relavent to pdf creation
            CreateEmailPDFButton.IsEnabled = true; EndPdfButton.IsEnabled = true; CreatePDFButton.IsEnabled = true;
            PdfButton.IsEnabled = false; 
        }

        private List<Tuple<Equation, bool>> SortEquationList(List<Equation> EquList, bool dif) {
            List<Tuple<Equation, bool>> tupleList = new List<Tuple<Equation, bool>>();
            List<Term> tempTermList = new List<Term>();
            int length = 0;
            int itterations = 0;
            foreach (Equation equ in EquList) {
                itterations = 0;
                foreach (Term term in equ.Components) {
                    length += term.ToString().Length;
                    if (length < 37)
                        tempTermList.Add(term);
                    else {
                        if (dif) tupleList.Add(new Tuple<Equation, bool>( new Diferentiation(tempTermList), itterations == 0 ? true : false));
                        else tupleList.Add(new Tuple<Equation, bool>(new Integration(tempTermList), itterations == 0 ? true : false));

                        tempTermList = new List<Term>();
                        tempTermList.Add(term);
                        length = term.ToString().Length;
                        ++itterations;
                    }
                }
                if (tempTermList[0] != null) {
                    if (dif) tupleList.Add(new Tuple<Equation, bool>(new Diferentiation(tempTermList), itterations == 0 ? true : false));
                    else tupleList.Add(new Tuple<Equation, bool>(new Integration(tempTermList), itterations == 0 ? true : false));
                }
            }
            return tupleList;
        }

        private string CreatePDF(List<Equation> EquList, bool dif) { //40 characters?
            PdfDocument document = new PdfDocument();
            PdfPage page;
            XGraphics gfx;
            XFont font = new XFont("Verdana", 20, XFontStyle.Bold); //sets chosen font and style
            List<Tuple<Equation, bool>> TupleList = SortEquationList(EquList, dif);

            int numpages = TupleList.Count / 14; //Workout the number of pages which will be required for the number of given questions
            int QuestionNum = 0;
            //Creates Question Pages
            for (int i = 0; i < numpages + 1; i++) {
                page = document.AddPage();
                gfx = XGraphics.FromPdfPage(page);
                if (i == 0) gfx.DrawString("Questions:", font, XBrushes.Black, new XRect(0, 10, page.Width, page.Height), XStringFormat.TopCenter);
                for (int j = 0; j < 14; j++) {
                    if (j + (14 * i) < TupleList.Count) {
                        QuestionNum += TupleList[j + (14 * i)].Item2 == true ? 1 : 0;
                        gfx.DrawString($"{  (TupleList[j + (14 * i)].Item2 == true ? $"{QuestionNum}." : "") }\t { Regex.Replace(TupleList[j + (14 * i)].Item1.ToString(), "[^0-9-/+/^/x ]+", "") }",
                            font, XBrushes.Black, new XRect(10, 10 + ((j + 2) * 50), page.Width, page.Height), XStringFormat.TopLeft);
                    }
                }
            }
            QuestionNum = 0;
            //Creates Answer Pages
            for (int i = 0; i < numpages + 1; i++) {
                page = document.AddPage();
                gfx = XGraphics.FromPdfPage(page);
                if (i == 0) gfx.DrawString("Answers:", font, XBrushes.Black, new XRect(0, 10, page.Width, page.Height), XStringFormat.TopCenter);
                for (int j = 0; j < 14; j++) {
                    if (j + (14 * i) < TupleList.Count) { 
                        QuestionNum += TupleList[j + (14 * i)].Item2 == true ? 1 : 0;
                        gfx.DrawString($"{ (TupleList[j + (14 * i)].Item2 == true ? $"{QuestionNum}." : "") }\t { Regex.Replace(TupleList[j + (14 * i)].Item1.FprimeEquationToString(), "[^0-9-/+/^/x ]+", "") }",
                            font, XBrushes.Black, new XRect(10, 10 + ((j + 2) * 50), page.Width, page.Height), XStringFormat.TopLeft);
                    }
                }
            }

            string datetime = DateTime.Now.ToString().Replace('/', '-').Replace(':', '.');
            string filename = $"{datetime}.pdf";
            document.Save(filename);
            return filename;
        }


        //Function to create a PDF from a list of Equations
        private bool CreatePDFfromList(List<Equation> EquList, bool email) {
            if (email && ClassesCombobox.SelectedItem == null) {
                MessageBox.Show("You must select a class to email.", "Error!");
                return false;
            }
            string filename = CreatePDF(EquList, EquList[0].GetType() ==  typeof(Diferentiation) ? true : false);
            if (email)
                SendPDFEmail(filename);
            else
                Process.Start(filename);
            return true;
        }

        private void SendPDFEmail(string filename) {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            string emaillist = "";
            Class tempclass = new Class();
            try {
                tempclass = (Class)(ClassesCombobox.SelectedItem);
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand($"SELECT Students.Email FROM Class_Lists, Students WHERE " +
                    $"Class_Lists.Student_ID = Students.ID AND Class_Lists.Class_ID = {tempclass.ID};", cnctDTB); //doesnt work btw
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    emaillist += $"{Common.Cleanstr(reader[0])};";
                }
            }
            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); }

            Email tempemail = new Email {
                Server = Common.Profile.Server,
                Port = Convert.ToInt16(Common.Profile.Port),
                UserEmail = Common.Profile.Email,
                UserPassword = Common.Profile.Password,
                UserName = Common.Profile.Name,
                Recipients = (emaillist.Split(';')).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList().ToArray(),
                Subject = $"{filename} Worksheet for class {4}",
                Body = "",
                AttachmentNames = new List<string>() { filename }
            };
            tempemail.Send();
        }
    

        //creates a random pdf according to user perameters
        private void CreateRandomPDF(bool email) {
            if (Convert.ToInt16(NumofQuestionsBox.Text) > 0) { //Makes sure the user has requested at least 1 question
                List<Equation> EquList = new List<Equation>();
                for (int i = 0; i < Convert.ToInt16(NumofQuestionsBox.Text); i++) {
                    EquList.Add(CreateRandomEquation()); //Adds a random equation to the list 
                }
                CreatePDFfromList(EquList, email); //creates pdf
            }
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
            if (CreatePDFfromList(QuestionsforPDF, true)) { //Creates a pdf from the current list of equations 
                QuestionsforPDF = new List<Equation>(); //clears list of current questions
                ResetPdfButtons(); //resets pdf buttons to indicate a new pdf can be started
            }
        }

        private void CreatePDFButton_Click(object sender, RoutedEventArgs e) {
            CreatePDFfromList(QuestionsforPDF, false); //Creates a pdf from the current list of equations 
            QuestionsforPDF = new List<Equation>(); //clears list of current questions
            ResetPdfButtons(); //resets pdf buttons to indicate a new pdf can be started
        }

        //currently doesnt work in specific cases such as when a new class is created on the email window. 
        public void UpdateClassCombobox() {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            List<Class> classes = new List<Class>();
            try {
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand($"SELECT * FROM Classes;", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    classes.Add(new Class { ID = Convert.ToInt16(Common.Cleanstr(reader[0])), Name = Common.Cleanstr(reader[1]) });
                }
                ClassesCombobox.ItemsSource = classes;
            }
            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); }
        }
    }
}
