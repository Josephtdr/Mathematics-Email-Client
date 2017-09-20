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
    /// Interaction logic for MangerAttachmentsWindow.xaml
    /// </summary>
    public class File
    {
        public string FileName { get; set; }
        public long FileLength { get; set; }
    }
    public partial class MangerAttachmentsWindow : Window
    {

        public MangerAttachmentsWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
        private void updateTable()
        {
            contactsDataGrid.Items.Clear();
            foreach (OpenFileDialog attachment in Common.AttachmentsSource)
            {
                contactsDataGrid.Items.Add(new File
                {
                    FileName = attachment.FileName,
                    FileLength = new FileInfo(attachment.FileName).Length
                });
            }
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == (DialogResult)1)
            {
                if (new FileInfo(openFileDialog.FileName).Length > 25 * Math.Pow(1024, 2) || Common.TotalFileLength + new FileInfo(openFileDialog.FileName).Length > 25 * Math.Pow(1024, 2))
                { System.Windows.MessageBox.Show("The total size of all included files excedes the limit of 25MB.", "Files too Large"); }
                else
                {
                    Common.AttachmentsSource.Add(openFileDialog);
                    Common.TotalFileLength += (new FileInfo(openFileDialog.FileName).Length);
                    Common.Attachments.Add(openFileDialog.FileName);
                    RemoveAllButton.IsEnabled = true;
                    UpdateMBValue(Common.TotalFileLength);
                    updateTable();
                }
            }
        }

        private void UpdateMBValue(long bytes)
        {
            int MB = (int)(bytes) / (int)(Math.Pow(1024, 2));
            MBValueLable.Content = MB.ToString();
            if (MB < 10) MBValueLable.Foreground = Brushes.Green;
            else if (MB < 20) MBValueLable.Foreground = Brushes.Yellow;
            else if (MB <= 25) MBValueLable.Foreground = Brushes.Red;
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveAllButton_Click(object sender, RoutedEventArgs e)
        {
            Common.Attachments.Clear();
            Common.AttachmentsSource.Clear();
            RemoveAllButton.IsEnabled = false;
            Common.TotalFileLength = 0;
            UpdateMBValue(Common.TotalFileLength);
            updateTable();
        }
    }
}
