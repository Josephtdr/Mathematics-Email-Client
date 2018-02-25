using System;
using System.Windows;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media;

namespace The_Email_Client
{
    /// <summary>
    /// Manage attachemnts to the email
    /// </summary>
    //Class to store file datails
    public class File {
        public string FileName { get; set; }
        public long FileLength { get; set; }
    }
    public partial class MangerAttachmentsWindow : Window {
        //constuctor
        public MangerAttachmentsWindow() {
            InitializeComponent();
            DataContext = this;
            //updates the indicator of how much data is currently attached
            UpdateMBValue(Common.TotalFileLength);
            //updates the table to display attached items
            updateTable();
        }
        //updates the table to display attached items
        private void updateTable() {
            fileDataGrid.Items.Clear();//Clears the table
            //loops through each attachment
            foreach (OpenFileDialog attachment in Common.AttachmentsSource) {
                fileDataGrid.Items.Add(new File {//adds a the file to the table
                    FileName = attachment.FileName,
                    FileLength = (long)(Convert.ToInt32(new FileInfo(attachment.FileName).Length) / (Math.Pow(1024, 1))) + 1
                });
            }
        }

        //Adds a new attachment to the list of attachemtns and table
        private void AddButton_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();//opens file explorer
            if (openFileDialog.ShowDialog() == (DialogResult)1) {//checks user chose a file
                //makes sure the file is not too large to exceed the file size limit
                if (Common.TotalFileLength + new FileInfo(openFileDialog.FileName).Length > 25 * Math.Pow(1024, 2)) 
                    //informs user their file was too large
                    System.Windows.MessageBox.Show("The total size of all included files excedes the limit of 25MB.", "Files too Large");
                else {
                    //Adds the file to the list of files
                    Common.AttachmentsSource.Add(openFileDialog);
                    Common.TotalFileLength += (new FileInfo(openFileDialog.FileName).Length);
                    Common.Attachments.Add(openFileDialog.FileName);
                    RemoveAllButton.IsEnabled = true;
                    UpdateMBValue(Common.TotalFileLength);//increments the total size of attached files
                    //updates the table
                    updateTable();
                }
            }
        }
        //updates the on screen display of overall file size
        private void UpdateMBValue(long bytes) {
            //Converts the file size to MB
            int MB = (int)(bytes) / (int)(Math.Pow(1024, 2));
            MBValueLable.Content = MB.ToString();
            //changes the colour depending upon how close to the limit the value is
            if (MB < 10) MBValueLable.Foreground = Brushes.Green;
            else if (MB < 20) MBValueLable.Foreground = Brushes.Orange;
            else if (MB <= 25) MBValueLable.Foreground = Brushes.Red;
        }
        //removes selected files from the table and list of files
        private void RemoveButton_Click(object sender, RoutedEventArgs e) {
            List<OpenFileDialog> FilesforDeletion = new List<OpenFileDialog>();
            foreach (File file in fileDataGrid.SelectedItems) {
                foreach (OpenFileDialog File in Common.AttachmentsSource)
                    if (file.FileName == File.FileName)
                        FilesforDeletion.Add(File);  
                Common.TotalFileLength -= (new FileInfo(file.FileName).Length);
                Common.Attachments.Remove(file.FileName);
            }
            foreach(OpenFileDialog File in FilesforDeletion)
                Common.AttachmentsSource.Remove(File);
            //updates on screen indicater
            UpdateMBValue(Common.TotalFileLength);
            //updates table
            updateTable();
         }
        //clears all attachments
        private void RemoveAllButton_Click(object sender, RoutedEventArgs e) {
            Common.Attachments.Clear();
            Common.AttachmentsSource.Clear();
            RemoveAllButton.IsEnabled = false;
            Common.TotalFileLength = 0;
            UpdateMBValue(Common.TotalFileLength);
            updateTable();
        }
    }
}
