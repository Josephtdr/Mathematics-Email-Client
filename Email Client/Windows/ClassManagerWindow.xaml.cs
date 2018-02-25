using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.OleDb;

namespace The_Email_Client {
    /// <summary>
    /// used to manage the classes in the database
    /// </summary>
    public partial class ClassManagerWindow : Window {
        List<string> ClassNames = new List<string>();
        DataGrid tempclassdatagrid = new DataGrid();
        public ClassManagerWindow() {
            InitializeComponent();
            //allows user to create a new class with the enter key
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Enter)
                && CreateClassButton.IsEnabled) CreateClass(); };
            //allows user to close the window with the escape key
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Escape)) Close(); };
            Updatetable();
        }
        //updates the table of classes
        public void Updatetable() {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);//sets up database connection
            try {
                cnctDTB.Open();//opens connection
                //SQL command to get all records from the Classes table
                OleDbCommand cmd = new OleDbCommand($"SELECT * FROM Classes;", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();
                //clears the table
                classDataGrid.Items.Clear();
                ClassNames.Clear();
                while (reader.Read()) {//loops through each record and adds it to the table along with local values
                    classDataGrid.Items.Add(new Class { Name = Common.Cleanstr(reader[1]), ID = Convert.ToInt16(Common.Cleanstr(reader[0])) });
                    tempclassdatagrid.Items.Add(new Class { Name = Common.Cleanstr(reader[1]), ID = Convert.ToInt16(Common.Cleanstr(reader[0])) });
                    ClassNames.Add(Common.Cleanstr(reader[1]));
                }
            }//informs user of an error in the case of one
            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); }//closes connection
        }
        //Deletes selected classes and any entries in class_Lists relevant to said classes
        private void RemovecontactButton_Click(object sender, RoutedEventArgs e) {
            //Messagebox to make sure they 
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the Class(es).",
                "Question?", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                //sets up database connection
                OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
                OleDbCommand cmd;//Sets up sql command variable
                try {
                    cnctDTB.Open();//opens database connection
                    foreach (Class Class in classDataGrid.SelectedItems) {//loops through each class
                        //SQL command to delete the class
                        cmd = new OleDbCommand($"DELETE FROM Classes WHERE ID = {Class.ID};", cnctDTB);
                        cmd.ExecuteNonQuery();
                        cnctDTB.Close(); cnctDTB.Open();//reopens connection
                        //SQL command to delete now redundant data as to keep referential integrity
                        cmd = new OleDbCommand($"DELETE FROM Class_Lists WHERE Class_ID = {Class.ID};", cnctDTB);
                        cmd.ExecuteNonQuery();
                        cnctDTB.Close(); cnctDTB.Open();
                    }
                }//informs user of an error in the case of one
                catch (Exception err) { MessageBox.Show(err.Message); }
                finally { cnctDTB.Close(); }//closes connection
                searchNameTextBox.Clear();//clears seach boxes
                Updatetable();//updates table
            }
        }
        //Runs whenever the search textbox is changes, 
        //updating the table to only classes which contain the search value
        private void SearchTextBoxes_TextChanged(object sender, TextChangedEventArgs e) {
            SearchTable(searchNameTextBox.Text, classDataGrid);
        }
        //updates the table to only classes which contain the search value
        private void SearchTable(string searchname, DataGrid datagrid) {
            datagrid.Items.Clear();

            for (int i = 0; i < tempclassdatagrid.Items.Count; i++) {
                if (string.IsNullOrWhiteSpace(searchname) || (((Class)tempclassdatagrid.Items[i]).Name.ToLower()).Contains(searchname.ToLower()))
                    datagrid.Items.Add(tempclassdatagrid.Items[i]);
            }
        }
        //creates a new class when button is pressed
        private void CreateClassButton_Click(object sender, RoutedEventArgs e) {
            CreateClass();
        }
        //Creates a new class in the database
        private void CreateClass() {
            //makes sure the name being user doesnt already exist
            if (!ClassNames.Contains(nametextbox.Text)) {
                Class tempclass = new Class();
                //sets up database connction
                OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
                try {
                    cnctDTB.Open();
                    //SQL command to create a new entry within the Classes Table 
                    OleDbCommand cmd = new OleDbCommand($"INSERT INTO Classes (Name) " +
                        $"VALUES ('{ nametextbox.Text }');", cnctDTB);
                    cmd.ExecuteNonQuery();
                    //SQL command to get the ID of the class just created
                    cmd.CommandText = $"SELECT * FROM Classes WHERE Name = '{nametextbox.Text}';";
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read()) {
                        tempclass.ID = Convert.ToInt16(reader[0]);
                        tempclass.Name = (string)reader[1];
                    }
                    //opens a window to edit the class just created
                    ClassEditerWindow classediterwindow = new ClassEditerWindow(tempclass);
                    classediterwindow.ShowDialog();
                }//informs user of an error in the case of one
                catch (Exception err) { MessageBox.Show(err.Message); }
                finally { cnctDTB.Close(); }//closes connection
                searchNameTextBox.Clear();
                nametextbox.Clear();
                Updatetable();//updates table
            }
            else //informs user that they cannot create a class with that name as it already exists
                MessageBox.Show("A Class with that name already exists!", "Error!");
        }
        //enables/disables the edit class button depending upon if only one class is selected
        private void ContactsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (classDataGrid.SelectedItems.Count == 1) EditClassButton.IsEnabled = true;
            else EditClassButton.IsEnabled = false;
        }
        //opens the window to edit the currently selected class
        private void EditClassButton_Click(object sender, RoutedEventArgs e) {
            ClassEditerWindow classediterwindow = new ClassEditerWindow((Class)(classDataGrid.SelectedItem));
            classediterwindow.ShowDialog();
        }
    }
}
