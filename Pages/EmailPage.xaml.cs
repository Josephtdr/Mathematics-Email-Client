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

namespace The_Email_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class EmailPage : Page
    {
        public List<Contacts> SelectedContacts = new List<Contacts>();

        List<string> Attachments = new List<string>();
        System.Windows.Controls.Button[] buttons;

        long TotalFileLength = 0;
        protected Action ShowLoginPage { get; set; }
        public Settings settings { get; set; }


        public EmailPage(Action ShowLoginPage)
        {
            settings = new Settings();
            this.ShowLoginPage = ShowLoginPage;
            InitializeComponent();
            buttons = new System.Windows.Controls.Button[] { SendButton, SettingsButton, AttachmentButton, ClearAttachments_Button };
            MBValueLable.Foreground = Brushes.Green;
        }

        public void GetEmail(string email)
        {
            settings = settings.UpdateSettingsfromDB(email);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Common.TempPassword = "";
            settings = new Settings();
            ShowLoginPage?.Invoke();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingswindow = new SettingsWindow(settings);
            settingswindow.ShowDialog();
            settings = settingswindow.SettingsObject;
        }

        private void UpdateMBValue(long bytes)
        {
            int MB = (int)(bytes) / (int)(Math.Pow(1024, 2));
            MBValueLable.Content = MB.ToString();
            if (MB < 10) MBValueLable.Foreground = Brushes.Green;
            else if (MB < 20) MBValueLable.Foreground = Brushes.Yellow;
            else if (MB <= 25) MBValueLable.Foreground = Brushes.Red;
        }

        private void AttachmentButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (new FileInfo(openFileDialog.FileName).Length > 25 * Math.Pow(1024, 2) || TotalFileLength + new FileInfo(openFileDialog.FileName).Length > 25 * Math.Pow(1024, 2))
                { System.Windows.Forms.MessageBox.Show("The total size of all included files excedes the limit of 25MB.", "Files too Large"); }
                else
                {
                    TotalFileLength += (new FileInfo(openFileDialog.FileName).Length);
                    Attachments.Add(openFileDialog.FileName);
                    ClearAttachments_Button.IsEnabled = true;
                    UpdateMBValue(TotalFileLength);
                }
            }

        }

        private void ClearAttachments_Button_Click(object sender, RoutedEventArgs e)
        {
            Attachments.Clear();
            ClearAttachments_Button.IsEnabled = false;
            TotalFileLength = 0;
            UpdateMBValue(TotalFileLength);
        }

        private void send_button_Click(object sender, RoutedEventArgs e)
        {
            StatusLabel.Content = "Sending...";

            foreach (System.Windows.Controls.Button Button in buttons) Button.IsEnabled = false;

            try
            {
                Email tempemail = new Email()
                {
                    Server = settings.Server,
                    Port = Convert.ToInt16(settings.Port),
                    UserEmail = settings.Email,
                    UserPassword = Common.TempPassword,
                    UserName = settings.Name,
                    Recipients = RecipientsBox.Text.Split(';'),
                    CC = CCBox.Text.Split(';'),
                    BCC = BCCBox.Text.Split(';'),
                    Subject = SubjectBox.Text,
                    Body = new TextRange(BodyBox.Document.ContentStart, BodyBox.Document.ContentEnd).Text,
                    AttachmentNames = Attachments
                };

                new Thread(new ParameterizedThreadStart(delegate { tempemail.Send(); })) { IsBackground = true }.Start();
            }
            catch (Exception error)
            {
                System.Windows.Forms.MessageBox.Show(error.Message);
            }
            finally
            {
                StatusLabel.Dispatcher.Invoke(delegate
                {
                    StatusLabel.Content = "Sent";
                    foreach (System.Windows.Controls.Button Button in buttons) Button.IsEnabled = true;
                    if (Attachments.Count == 0) ClearAttachments_Button.IsEnabled = false;
                });
            }
        }

        private void addemailstotextboxes(Contacts[] contacts, System.Windows.Controls.TextBox textbox)
        {
            foreach (Contacts contact in contacts)
            {

                if (textbox.Text == null || textbox.Text == "") { textbox.Text += (contact.EmailAddress); }
                else
                {
                    textbox.Text += (";" + contact.EmailAddress);
                }
                SelectedContacts.Add(contact);
            }
        }

        private void addemailTO_CC_BCCbuttons_Click(object sender, RoutedEventArgs e)
        {
            selectingcontactWindow selectingcontactWindow = new selectingcontactWindow(SelectedContacts.ToArray());
            selectingcontactWindow.ShowDialog();

            switch ((string)(((System.Windows.Controls.Button)sender).Content))
            {
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

        private void contactsmanagerbutton_Click(object sender, RoutedEventArgs e)
        {
            ContactsManagerWindows contactsmanagerwindow = new ContactsManagerWindows();
            contactsmanagerwindow.ShowDialog();
        }

    }
}
