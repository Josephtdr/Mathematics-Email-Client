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

namespace The_Email_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class EmailPage : Page
    {
        public List<string> SelectedContacts = new List<string>();
        
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
            Email tempemail = new Email()
            {
                Server = Common.Profile.Server,
                Port = Convert.ToInt16(Common.Profile.Port),
                UserEmail = Common.Profile.Email,
                UserPassword = Common.Profile.Password,
                UserName = Common.Profile.Name,
                Recipients = RecipientsBox.Text.Split(';'),
                CC = CCBox.Text.Split(';'),
                BCC = BCCBox.Text.Split(';'),
                Subject = SubjectBox.Text,
                Body = new TextRange(BodyBox.Document.ContentStart, BodyBox.Document.ContentEnd).Text,
                AttachmentNames = Common.Attachments
            };
            new Thread(new ParameterizedThreadStart(SendEmail)) { IsBackground = true }.Start(tempemail);   
        }

        private void SendEmail(object email) {
            try {
                ((Email)email).Send();
                Dispatcher.Invoke(() => ClearPage());
            }
            catch (Exception error) { System.Windows.Forms.MessageBox.Show(error.Message); }
        }

        private void addemailstotextboxes(Contacts[] contacts, System.Windows.Controls.TextBox textbox) {
            foreach (Contacts contact in contacts)
                if (textbox.Text == null || textbox.Text == "") textbox.Text += (contact.EmailAddress);
                else textbox.Text += (";" + contact.EmailAddress);
        }

        private void addemailTO_CC_BCCbuttons_Click(object sender, RoutedEventArgs e)  {
            SelectedContacts.Clear();
            foreach (string email in (RecipientsBox.Text).Split(';'))
                if (!string.IsNullOrEmpty(email)) SelectedContacts.Add(email);
            foreach (string email in (CCBox.Text).Split(';'))
                if (!string.IsNullOrEmpty(email)) SelectedContacts.Add(email);
            foreach (string email in (BCCBox.Text).Split(';'))
                if (!string.IsNullOrEmpty(email)) SelectedContacts.Add(email);

            selectingcontactWindow selectingcontactWindow = new selectingcontactWindow(SelectedContacts.ToArray());
            selectingcontactWindow.ShowDialog();

            switch ((string)(((System.Windows.Controls.Button)sender).Content)) {
                case "To...":
                    addemailstotextboxes(selectingcontactWindow.SelectedContacts, RecipientsBox);
                    break;
                case "Cc...":
                    addemailstotextboxes(selectingcontactWindow.SelectedContacts, CCBox);
                    break;
                case "BCc...":
                    addemailstotextboxes(selectingcontactWindow.SelectedContacts, BCCBox);
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
