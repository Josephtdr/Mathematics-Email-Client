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
        public Object[] SelectedRecipients { get; protected set; }
        //lists to store emails/classes already in email page
        private string[] preexistingcontacts;
        private string[] preexistingclasses;
        //tables used for searching 
        DataGrid tempclassdatagrid = new DataGrid();
        DataGrid tempstudentdatagrid = new DataGrid();
        //constructor
        public selectingcontactWindow(string[] preexistingcontacts, string[] preexistingclasses) {
            InitializeComponent();
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Escape)) Close(); };
            SelectedRecipients = null;
            this.preexistingclasses = preexistingclasses;
            this.preexistingcontacts = preexistingcontacts;
            //updates both tables
            updatetable("Students", studentsDataGrid);
            updatetable("Classes", classDataGrid);
        }
        //function to clear two datagrids [tables]
        private void ClearDataGrids(DataGrid dataGrid1, DataGrid dataGrid2) {
            dataGrid1.Items.Clear();
            dataGrid2.Items.Clear();
        }
        //funmction to update either table depending upon which is specified
        private void updatetable(string DBTable, DataGrid datagrid) {
            //sets up database connection
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try {
                bool check = (datagrid.Name == "studentsDataGrid");
                cnctDTB.Open();//opens connection
                OleDbCommand cmd = new OleDbCommand("", cnctDTB);
                if (check) {
                    //SQL command to get all students
                    cmd.CommandText = $"SELECT * FROM {DBTable};";
                    ClearDataGrids(datagrid, tempstudentdatagrid);
                }
                else {
                    //SQL statement to get all classes
                    cmd.CommandText = $"SELECT * FROM {DBTable};";
                    ClearDataGrids(datagrid, tempclassdatagrid);
                }
                
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    //checks if the email/class is already within a field on the email page
                    if (!((check && preexistingcontacts.Contains(reader[2])) || preexistingclasses.Contains($"{{{reader[1]}}}"))) {
                        if (check) {//specifies if adding a student or class
                            datagrid.Items.Add(new Student { Name = Common.Cleanstr(reader[1]), EmailAddress = Common.Cleanstr(reader[2]) });
                            tempstudentdatagrid.Items.Add(new Student { Name = Common.Cleanstr(reader[1]), EmailAddress = Common.Cleanstr(reader[2]) });
                        }
                        else {
                            datagrid.Items.Add(new Class { Name = Common.Cleanstr(reader[1]) });
                            tempclassdatagrid.Items.Add(new Class { Name = Common.Cleanstr(reader[1]) });
                        }
                    }
                }
            }//informs user in case of an error
            catch (Exception err) { MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); }//closes connection
        }
        //adds a class/email to the 
        private void addcontacttoemailButton_Click(object sender, RoutedEventArgs e) {
            List<object> Recipieints = new List<object>();
            //loops through each selected item and adds it to the list to be added to the email page
            foreach(var recipient in (((Button)sender).Name == "addstudenstoemailButton" ? studentsDataGrid.SelectedItems : classDataGrid.SelectedItems) )
                Recipieints.Add(recipient);
            
            SelectedRecipients = Recipieints.ToArray();
            Close();//closes the window
        }
        //function used to search either table
        private void SearchTable(string searchname, DataGrid datagrid, string searchemail = "") {            
            datagrid.Items.Clear();//clears the table
            if (datagrid.Name == "studentsDataGrid") {//checks which datagrid is being searched
                //loops through each student and adds it to the table if it is applicable
                for (int i = 0; i < tempstudentdatagrid.Items.Count; i++) {
                    if((string.IsNullOrWhiteSpace(searchname) || (((Student)tempstudentdatagrid.Items[i]).Name.ToLower()).Contains(searchname.ToLower())) 
                        && (string.IsNullOrWhiteSpace(searchemail) || (((Student)tempstudentdatagrid.Items[i]).EmailAddress.ToLower()).Contains(searchemail.ToLower())))
                        datagrid.Items.Add(tempstudentdatagrid.Items[i]);
                }
            }
            else//loops through each class and adds it to the table if it is applicable
                for (int i = 0; i < tempclassdatagrid.Items.Count; i++) {
                    if (string.IsNullOrWhiteSpace(searchname) || (((Class)tempclassdatagrid.Items[i]).Name.ToLower()).Contains(searchname.ToLower()))
                        datagrid.Items.Add(tempclassdatagrid.Items[i]);
                }
        }
        //fucntions runs whenever any of the searching textboxes text is changed
        private void searchTextBoxes_TextChanged(object sender, TextChangedEventArgs e) {
            switch (((TextBox)sender).Name) {//switch statement to see which table is being searched
                case "searchclassNameTextBox":
                    SearchTable(searchclassNameTextBox.Text, classDataGrid);
                    break;
                case "searchNameTextBox": case "searchEmailTextBox":
                    SearchTable(searchNameTextBox.Text, studentsDataGrid, searchEmailTextBox.Text);
                    break;
            }
        }
    }
}
