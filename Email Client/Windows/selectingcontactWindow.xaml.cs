using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.OleDb;
using System.Globalization;
using System.Text.RegularExpressions;


namespace The_Email_Client {
    /// <summary>
    /// Interaction logic for ContactsManagerWindows.xaml
    /// </summary>
    /// 
    
    public partial class selectingcontactWindow : Window {
        public Object[] SelectedEmails { get; protected set; }

        public string[] preexistingcontacts;
        public string[] preexistingclasses;

        public selectingcontactWindow(string[] preexistingcontacts, string[] preexistingclasses) {
            InitializeComponent();
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Escape)) Close(); };
            SelectedEmails = null;
            this.preexistingclasses = preexistingclasses;
            this.preexistingcontacts = preexistingcontacts;
            updatetable("","", "Students", true, studensDataGrid);
            updatetable("", "", "Classes", false, classDataGrid);
        }
        public void updatetable(string searchemailValue, string searchnameValue, string DBTable, bool Nameandemail, DataGrid datagrid) {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try {
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand("", cnctDTB);
                if (Nameandemail)
                    cmd.CommandText = $"SELECT * FROM {DBTable} WHERE Email LIKE '%{searchemailValue}%' AND Name LIKE '%{searchnameValue}%';";
                else
                    cmd.CommandText = $"SELECT * FROM {DBTable} WHERE Name LIKE '%{searchnameValue}%';";

                OleDbDataReader reader = cmd.ExecuteReader();
                datagrid.Items.Clear();

                bool includecontact = true;
                while (reader.Read()) {
                    includecontact = true;
                    if ((Nameandemail && preexistingcontacts.Contains(reader[2])) || preexistingclasses.Contains($"{{{reader[1]}}}"))
                        includecontact = false;

                    if (includecontact) {
                        if (Nameandemail)
                            datagrid.Items.Add(new Student { Name = Common.Cleanstr(reader[1]), EmailAddress = Common.Cleanstr(reader[2]) });
                        else
                            datagrid.Items.Add(new Class { Name = Common.Cleanstr(reader[1]) });
                    }
                }
            }
            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); }
        }

        private void addcontacttoemailButton_Click(object sender, RoutedEventArgs e) {
            List<object> emails = new List<object>();
            foreach(var contact in (((Button)sender).Name == "addstudenstoemailButton" ? studensDataGrid.SelectedItems : classDataGrid.SelectedItems) )
                emails.Add(contact);
            
            SelectedEmails = emails.ToArray();
            Close();
        }
        
        private void searchTextBoxes_TextChanged(object sender, TextChangedEventArgs e) {
            switch (((TextBox)sender).Name) {
                case "searchclassNameTextBox":
                    updatetable("", searchclassNameTextBox.Text, "Classes", false, studensDataGrid);
                    break;
                case "searchNameTextBox":
                    updatetable(searchEmailTextBox.Text, searchNameTextBox.Text, "Students", true, classDataGrid);
                    break;
                case "searchEmailTextBox":
                    updatetable(searchEmailTextBox.Text, searchNameTextBox.Text, "Students", true, classDataGrid);
                    break;
            }
        }
    }
}
