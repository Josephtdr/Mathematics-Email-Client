using System;
using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Net.Mime;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Linq;

namespace The_Email_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class EmailPage : Page
    {
        public List<string> SelectedContacts = new List<string>();
        public List<string> SelectedClasses = new List<string>();
        protected Action ShowPreviousPage { get; set; }
        protected Action ShowHomePage { get; set; }
        public EmailPage(Action ShowPreviousPage, Action ShowHomePage) {
            this.ShowPreviousPage = ShowPreviousPage;
            this.ShowHomePage = ShowHomePage;
            InitializeComponent();
            MBValueLable.Foreground = Brushes.Green;
        }
        

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            ShowPreviousPage?.Invoke();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e) {
            ShowHomePage();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e) {
            ProfilesWindow settingswindow = new ProfilesWindow();
            settingswindow.ShowDialog();
        }

        private void UpdateMBValue(long bytes) {
            int MB = (int)(bytes) / (int)(Math.Pow(1024, 2));
            MBValueLable.Content = MB.ToString();
            if (MB < 10) MBValueLable.Foreground = Brushes.Green;
            else if (MB < 20) MBValueLable.Foreground = Brushes.Yellow;
            else if (MB <= 25) MBValueLable.Foreground = Brushes.Red;
            if (bytes != 0) ClearAttachments_Button.IsEnabled = true;
        }

        private void AttachmentButton_Click(object sender, RoutedEventArgs e) {
            MangerAttachmentsWindow MangerAttachmentsWindow = new MangerAttachmentsWindow();
            MangerAttachmentsWindow.ShowDialog();
            UpdateMBValue(Common.TotalFileLength);
        }

        private void ClearAttachments_Button_Click(object sender, RoutedEventArgs e) {
            Common.Attachments.Clear();
            Common.AttachmentsSource.Clear();
            ClearAttachments_Button.IsEnabled = false;
            Common.TotalFileLength = 0;
            UpdateMBValue(Common.TotalFileLength);
        }

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
        private void send_button_Click(object sender, RoutedEventArgs e) {
            StatusLabel.Content = "Sending...";
            LoadingGif.Visibility = Visibility.Visible;
            List<string> Attachments = Common.Attachments;
            Email tempemail = new Email() {
                Server = Common.Profile.Server,
                Port = Convert.ToInt16(Common.Profile.Port),
                UserEmail = Common.Profile.Email,
                UserPassword = Common.Profile.Password,
                UserName = Common.Profile.Name,
                Recipients = CreateRecipientsList(RecipientsBox.Text.Split(';')),
                CC = CCBox.Text.Split(';'),
                BCC = BCCBox.Text.Split(';'),
                Subject = SubjectBox.Text,
                Body = new TextRange(BodyBox.Document.ContentStart, BodyBox.Document.ContentEnd).Text,
                AttachmentNames = Common.Attachments
            };
            new Thread(new ParameterizedThreadStart(SendEmail)) { IsBackground = true }.Start(tempemail); 
            //runs sending email in the background.
        }

        private string[] CreateRecipientsList(string[] recipients) {
            List<string> templist = new List<string>();
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            foreach (string recipient in recipients) {
                if (!recipient.Contains("}"))
                    templist.Add(recipient);
                else {
                    try {
                        cnctDTB.Open();
                        OleDbCommand cmd = new OleDbCommand($"Select Students.Email FROM Students, Class_Lists, Classes WHERE " +
                            $"Students.ID = Class_Lists.Student_ID AND Classes.ID = Class_Lists.Class_ID "+
                            $"AND Classes.Name = '{ recipient.Replace("{","").Replace("}","") }';", cnctDTB);
                        OleDbDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                            templist.Add(reader[0].ToString());
                    }
                    catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
                    finally { cnctDTB.Close(); }
                }
            }
            return templist.ToArray();
        }

        private void SendEmail(object email) {
            try {
                ((Email)email).Send();
                Dispatcher.Invoke(() => ClearPage()); //calls function in a thread safe manor
            }
            catch (Exception error) { System.Windows.Forms.MessageBox.Show(error.Message); }
        }

        private void addemailstotextboxes(Object[] contacts, System.Windows.Controls.TextBox textbox) {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            if (contacts != null) {
                if(contacts[0].GetType() == typeof(Class)) {
                        foreach (Class Class in contacts)
                            try {
                                cnctDTB.Open();
                                OleDbCommand cmd = new OleDbCommand($"SELECT * FROM Classes WHERE Name = '{Class.Name}';", cnctDTB);
                                OleDbDataReader Reader = cmd.ExecuteReader();
                                while (Reader.Read())
                                    textbox.Text += string.IsNullOrWhiteSpace(textbox.Text) ? $"{{{Reader[1].ToString()}}}" : $";{{{Reader[1].ToString()}}}";
                            }
                            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
                            finally { cnctDTB.Close(); }
                }
                else {
                        foreach (Student student in contacts)
                            textbox.Text += string.IsNullOrWhiteSpace(textbox.Text) ? student.EmailAddress : $";{student.EmailAddress}";
                }
            }
        }

        private void addemailTO_CC_BCCbuttons_Click(object sender, RoutedEventArgs e)  {
            SelectedContacts.Clear(); SelectedClasses.Clear();
            foreach (string email in (((RecipientsBox.Text).Split(';')).Union((CCBox.Text).Split(';'))).Union((BCCBox.Text).Split(';'))) {
                if (!string.IsNullOrEmpty(email) && !email.Contains("{")) SelectedContacts.Add(email);
                else if (!string.IsNullOrEmpty(email)) SelectedClasses.Add(email);
            }
            selectingcontactWindow selectingcontactWindow = new selectingcontactWindow(SelectedContacts.ToArray(), SelectedClasses.ToArray());
            selectingcontactWindow.ShowDialog();

            switch ((string)(((System.Windows.Controls.Button)sender).Content)) {
                case "To...":
                    addemailstotextboxes(selectingcontactWindow.SelectedEmails, RecipientsBox);
                    break;
                case "Cc...":
                    addemailstotextboxes(selectingcontactWindow.SelectedEmails, CCBox);
                    break;
                case "BCc...":
                    addemailstotextboxes(selectingcontactWindow.SelectedEmails, BCCBox);
                    break;
            }
        }
        //opens a new instance of the ClassManagerWindow
        private void classmanagerbutton_Click(object sender, RoutedEventArgs e) {
            ClassManagerWindow classmanagerwindow = new ClassManagerWindow();
            classmanagerwindow.ShowDialog();
            //CalculusPage.UpdateClassCombobox();
        }
        
    }
}
