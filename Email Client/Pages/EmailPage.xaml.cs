using System;
using System.Windows;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Media;
using System.Data.OleDb;
using System.Windows.Controls;
using System.Linq;

namespace The_Email_Client {
    /// <summary>
    /// Page to send emails from
    /// </summary>
    public partial class EmailPage : Page {
        public List<string> SelectedContacts = new List<string>();
        public List<string> SelectedClasses = new List<string>();
        protected Action ShowPreviousPage { get; set; }
        protected Action ShowHomePage { get; set; }
        //constructor
        public EmailPage(Action ShowPreviousPage, Action ShowHomePage) {
            this.ShowPreviousPage = ShowPreviousPage;
            this.ShowHomePage = ShowHomePage;
            InitializeComponent();
            MBValueLable.Foreground = Brushes.Green;
        }
        //takes the user to the previous page
        private void BackButton_Click(object sender, RoutedEventArgs e) {
            ShowPreviousPage?.Invoke();
        }
        //takes the user to the home page
        private void HomeButton_Click(object sender, RoutedEventArgs e) {
            ShowHomePage();
        }
        //opens the profile window
        private void ProfileButton_Click(object sender, RoutedEventArgs e) {
            ProfilesWindow profileswindow = new ProfilesWindow();
            profileswindow.ShowDialog();
        }
        //function to update the MB indicator to the current value
        private void UpdateMBValue(long bytes) {
            int MB = (int)(bytes) / (int)(Math.Pow(1024, 2));
            MBValueLable.Content = MB.ToString();
            if (MB < 10) MBValueLable.Foreground = Brushes.Green;
            else if (MB < 20) MBValueLable.Foreground = Brushes.Yellow;
            else if (MB <= 25) MBValueLable.Foreground = Brushes.Red;
            if (bytes != 0) ClearAttachments_Button.IsEnabled = true;
        }//updates the displayed MB value to show the user the number of MB of files currently attached to their email

        private void AttachmentButton_Click(object sender, RoutedEventArgs e) {
            MangerAttachmentsWindow MangerAttachmentsWindow = new MangerAttachmentsWindow();
            MangerAttachmentsWindow.ShowDialog();
            UpdateMBValue(Common.TotalFileLength);
        }//opens the manage attachment window 

        //function to clear all attachments
        private void ClearAttachments_Button_Click(object sender, RoutedEventArgs e) {
            Common.Attachments.Clear();
            Common.AttachmentsSource.Clear();
            ClearAttachments_Button.IsEnabled = false;
            Common.TotalFileLength = 0;
            UpdateMBValue(Common.TotalFileLength);
        }
        //functions to clear all elements on the page
        private void ClearPage() {
            BCCBox.Clear(); CCBox.Clear(); RecipientsBox.Clear();
            BodyBox.Document.Blocks.Clear(); SubjectBox.Clear(); 
            Common.Attachments.Clear();
            Common.AttachmentsSource.Clear();
            ClearAttachments_Button.IsEnabled = false;
            Common.TotalFileLength = 0;
            UpdateMBValue(Common.TotalFileLength);
            StatusLabel.Content = "Sent";
            LoadingGif.Visibility = Visibility.Hidden;
        }   
        //Procedure which runs when the users wishes to send an email
        private void Send_button_Click(object sender, RoutedEventArgs e) {
            //Both Used to indicate to the user the email is sending
            StatusLabel.Content = "Sending...";
            LoadingGif.Visibility = Visibility.Visible;
            
            //Gets all applicable values and creates a new Email
            Email tempemail = new Email() {
                Server = Common.Profile.Server,
                Port = Convert.ToInt16(Common.Profile.Port),
                UserEmail = Common.Profile.Email,
                UserPassword = Common.Profile.Password,
                UserName = Common.Profile.Name,
                Recipients = CreateEmailsList(RecipientsBox.Text.Split(';')),
                CC = CreateEmailsList(CCBox.Text.Split(';')),
                BCC = CreateEmailsList(BCCBox.Text.Split(';')),
                Subject = SubjectBox.Text,
                Body = new TextRange(BodyBox.Document.ContentStart, BodyBox.Document.ContentEnd).Text,
                AttachmentNames = Common.Attachments
            };
            new Thread(new ParameterizedThreadStart(SendEmail)) { IsBackground = true }.Start(tempemail); 
            //runs SendEmail in the background.
        }

        //Takes a users string of desired recipients and returns a formatted list for the Gamil API
        private string[] CreateEmailsList(string[] rawEmails) {
            List<string> RecipientsEmailList = new List<string>();//list which stores all the emails found in rawEmail 
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            foreach (string Recipients in rawEmails) {//loops through each set of recipients in rawEmails
                if (!Recipients.Contains("}"))//checks if recipiets is a singular email or the name of a list of emails
                    RecipientsEmailList.Add(Recipients);//if a singular email, adds to the list of emails
                else {
                    try {
                        cnctDTB.Open();//opens a connection to the database
                        //SQL statment finding all relavent emails to said list of recipients
                        OleDbCommand cmd = new OleDbCommand($"Select Students.Email FROM Students, Class_Lists, Classes WHERE " +
                            $"Students.ID = Class_Lists.Student_ID AND Classes.ID = Class_Lists.Class_ID "+
                            $"AND Classes.Name = '{ Recipients.Replace("{","").Replace("}","") }';", cnctDTB);
                        OleDbDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())//loops through each relavent item in the database and adds them to the list of emails
                            RecipientsEmailList.Add(reader[0].ToString());
                    }
                    catch (Exception err) { MessageBox.Show(err.Message); } //displays error to user
                    finally { cnctDTB.Close(); } //closes connection to database
                }
            }
            return RecipientsEmailList.ToArray();//Returns the list of emails in the form of an array
        }
        //function to send a users email
        private void SendEmail(object email) {
            try {
                ((Email)email).Send();//Sends the users email
                //Indicates to the user the email has succesfully sent
                Dispatcher.Invoke(() => ClearPage()); //calls function in a thread safe manor
            }
            //Informs user in the case of an error
            catch (Exception error) { MessageBox.Show(error.Message); }
        }
        //Adds emails to the specified textbox
        private void Addemailstotextboxes(Object[] contacts, TextBox textbox) {
            //sets up database connection
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            if (contacts != null) {
                if(contacts[0].GetType() == typeof(Class)) {//checks if the user is adding classes or students
                        foreach (Class Class in contacts)//loops through each class
                            try {
                                cnctDTB.Open();//opens conenction
                            //SQL statement to select all emails from the class
                                OleDbCommand cmd = new OleDbCommand($"SELECT * FROM Classes WHERE Name = '{Class.Name}';", cnctDTB);
                                OleDbDataReader Reader = cmd.ExecuteReader();
                                while (Reader.Read())//adds each email to the specified textbox
                                    textbox.Text += string.IsNullOrWhiteSpace(textbox.Text) ? $"{{{Reader[1].ToString()}}}" : $";{{{Reader[1].ToString()}}}";
                            }//informs the user in the case of an error
                            catch (Exception err) { MessageBox.Show(err.Message); }
                            finally { cnctDTB.Close(); }
                }
                else {
                        foreach (Student student in contacts)
                            textbox.Text += string.IsNullOrWhiteSpace(textbox.Text) ? student.EmailAddress : $";{student.EmailAddress}";
                }
            }
        }
        //function which is used to add students/classes from database to recipients
        private void AddemailTO_CC_BCCbuttons_Click(object sender, RoutedEventArgs e)  {
            SelectedContacts.Clear(); SelectedClasses.Clear();
            //loops through each email and class in the recipients, Cc, and BCc textboxes
            foreach (string email in (((RecipientsBox.Text).Split(';')).Union((CCBox.Text).Split(';'))).Union((BCCBox.Text).Split(';'))) {
                if(!string.IsNullOrEmpty(email))//makes sure email is not null or empty
                    //checks if the email is an email or class name and adds it to the apropriate list
                    if (!email.Contains("{")) SelectedContacts.Add(email);
                    else SelectedClasses.Add(email);
            }
            //opens the window for the user to select the classes/emails to add to the correct textbox
            selectingcontactWindow selectingcontactWindow = new selectingcontactWindow(SelectedContacts.ToArray(), SelectedClasses.ToArray());
            selectingcontactWindow.ShowDialog();

            //checks which textbox the email/class is being added to and adds it to it
            switch ((string)(((Button)sender).Content)) {
                case "To...":
                    Addemailstotextboxes(selectingcontactWindow.SelectedRecipients, RecipientsBox);
                    break;
                case "Cc...":
                    Addemailstotextboxes(selectingcontactWindow.SelectedRecipients, CCBox);
                    break;
                case "BCc...":
                    Addemailstotextboxes(selectingcontactWindow.SelectedRecipients, BCCBox);
                    break;
            }
        }
        //opens a new instance of the ClassManagerWindow
        private void Classmanagerbutton_Click(object sender, RoutedEventArgs e) {
            ClassManagerWindow classmanagerwindow = new ClassManagerWindow();
            classmanagerwindow.ShowDialog();
            //CalculusPage.UpdateClassCombobox();
        }
        
    }
}
