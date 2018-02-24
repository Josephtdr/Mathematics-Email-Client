using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.OleDb;

namespace The_Email_Client {
    /// <summary>
    /// used to imput emails and classes into the To, Cc, BCc boxes in the email page
    /// </summary>
    
    public partial class selectingcontactWindow : Window {
        public Object[] SelectedEmails { get; protected set; }
        private string[] preexistingcontacts;
        private string[] preexistingclasses;
        DataGrid tempclassdatagrid = new DataGrid();
        DataGrid tempstudentdatagrid = new DataGrid();

        public selectingcontactWindow(string[] preexistingcontacts, string[] preexistingclasses) {
            InitializeComponent();
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Escape)) Close(); };
            SelectedEmails = null;
            this.preexistingclasses = preexistingclasses;
            this.preexistingcontacts = preexistingcontacts;
            updatetable("Students", studentsDataGrid);
            updatetable("Classes", classDataGrid);
        }

        private void ClearDataGrids(DataGrid dataGrid1, DataGrid dataGrid2) {
            dataGrid1.Items.Clear();
            dataGrid2.Items.Clear();
        }
        private void updatetable(string DBTable, DataGrid datagrid) {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try {
                bool check = (datagrid.Name == "studentsDataGrid");
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand("", cnctDTB);
                if (check) {
                    cmd.CommandText = $"SELECT * FROM {DBTable};";
                    ClearDataGrids(datagrid, tempstudentdatagrid);
                }
                else {
                    cmd.CommandText = $"SELECT * FROM {DBTable};";
                    ClearDataGrids(datagrid, tempclassdatagrid);
                }
                
                OleDbDataReader reader = cmd.ExecuteReader();
                bool includecontact = true;
                while (reader.Read()) {
                    includecontact = true;
                    if ((check && preexistingcontacts.Contains(reader[2])) || preexistingclasses.Contains($"{{{reader[1]}}}"))
                        includecontact = false;

                    if (includecontact) {
                        if (check) {
                            datagrid.Items.Add(new Student { Name = Common.Cleanstr(reader[1]), EmailAddress = Common.Cleanstr(reader[2]) });
                            tempstudentdatagrid.Items.Add(new Student { Name = Common.Cleanstr(reader[1]), EmailAddress = Common.Cleanstr(reader[2]) });
                        }
                        else {
                            datagrid.Items.Add(new Class { Name = Common.Cleanstr(reader[1]) });
                            tempclassdatagrid.Items.Add(new Class { Name = Common.Cleanstr(reader[1]) });
                        }
                    }
                }
            }
            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); }
        }

        private void addcontacttoemailButton_Click(object sender, RoutedEventArgs e) {
            List<object> emails = new List<object>();
            foreach(var contact in (((Button)sender).Name == "addstudenstoemailButton" ? studentsDataGrid.SelectedItems : classDataGrid.SelectedItems) )
                emails.Add(contact);
            
            SelectedEmails = emails.ToArray();
            Close();
        }
        
        private void SearchTable(string searchname, string searchemail, DataGrid datagrid) {            
            datagrid.Items.Clear();
            if (datagrid.Name == "studentsDataGrid") {
                for (int i = 0; i < tempstudentdatagrid.Items.Count; i++) {
                    if((string.IsNullOrWhiteSpace(searchname) || (((Student)tempstudentdatagrid.Items[i]).Name.ToLower()).Contains(searchname.ToLower())) 
                        && (string.IsNullOrWhiteSpace(searchemail) || (((Student)tempstudentdatagrid.Items[i]).EmailAddress.ToLower()).Contains(searchemail.ToLower())))
                        datagrid.Items.Add(tempstudentdatagrid.Items[i]);
                }
            }
            else
                for (int i = 0; i < tempclassdatagrid.Items.Count; i++) {
                    if (string.IsNullOrWhiteSpace(searchname) || (((Class)tempclassdatagrid.Items[i]).Name.ToLower()).Contains(searchname.ToLower()))
                        datagrid.Items.Add(tempclassdatagrid.Items[i]);
                }
        }

        private void searchTextBoxes_TextChanged(object sender, TextChangedEventArgs e) {
            switch (((TextBox)sender).Name) {
                case "searchclassNameTextBox":
                    SearchTable(searchclassNameTextBox.Text, "", classDataGrid);
                    break;
                case "searchNameTextBox": case "searchEmailTextBox":
                    SearchTable(searchNameTextBox.Text, searchEmailTextBox.Text, studentsDataGrid);
                    break;
            }
        }
    }
}
