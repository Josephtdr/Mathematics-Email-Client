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
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Enter)
                && CreateClassButton.IsEnabled) CreateClass(); };
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Escape)) Close(); };
            Updatetable();
        }

        public void Updatetable() {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try {
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand($"SELECT * FROM Classes;", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();

                classDataGrid.Items.Clear();
                ClassNames.Clear();
                while (reader.Read()) {
                    classDataGrid.Items.Add(new Class { Name = Common.Cleanstr(reader[1]), ID = Convert.ToInt16(Common.Cleanstr(reader[0])) });
                    tempclassdatagrid.Items.Add(new Class { Name = Common.Cleanstr(reader[1]), ID = Convert.ToInt16(Common.Cleanstr(reader[0])) });
                    ClassNames.Add(Common.Cleanstr(reader[1]));
                }
            }
            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); }
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
                }
                catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
                finally { cnctDTB.Close(); }
                searchNameTextBox.Clear();
                Updatetable();
            }
        }
        private void SearchTextBoxes_TextChanged(object sender, TextChangedEventArgs e) {
            SearchTable(searchNameTextBox.Text, classDataGrid);
        }

        private void SearchTable(string searchname, DataGrid datagrid) {
            datagrid.Items.Clear();

            for (int i = 0; i < tempclassdatagrid.Items.Count; i++) {
                if (string.IsNullOrWhiteSpace(searchname) || (((Class)tempclassdatagrid.Items[i]).Name.ToLower()).Contains(searchname.ToLower()))
                    datagrid.Items.Add(tempclassdatagrid.Items[i]);
            }
        }

        private void AddcontactButton_Click(object sender, RoutedEventArgs e) {
            CreateClass();

        }

        private void CreateClass() {
            if (!ClassNames.Contains(nametextbox.Text)) {
                Class tempclass = new Class();
                OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
                try {
                    cnctDTB.Open();
                    OleDbCommand cmd = new OleDbCommand($"INSERT INTO Classes (Name) " +
                        $"VALUES ('{ nametextbox.Text }');", cnctDTB);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = $"SELECT * FROM Classes WHERE Name = '{nametextbox.Text}';";
                    OleDbDataReader reader = cmd.ExecuteReader();
                    while (reader.Read()) {
                        tempclass.ID = Convert.ToInt16(reader[0]);
                        tempclass.Name = (string)reader[1];
                    }
                    ClassEditerWindow classediterwindow = new ClassEditerWindow(tempclass);
                    classediterwindow.ShowDialog();
                }
                catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
                finally { cnctDTB.Close(); }
                searchNameTextBox.Clear();
                nametextbox.Clear();
                Updatetable();
            }
            else 
                MessageBox.Show("A Class with that name already exists!", "Error!");
        }
      
        private void ContactsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (classDataGrid.SelectedItems.Count == 1) EditClassButton.IsEnabled = true;
            else EditClassButton.IsEnabled = false;
        }

        private void EditClassButton_Click(object sender, RoutedEventArgs e) {
            ClassEditerWindow classediterwindow = new ClassEditerWindow((Class)(classDataGrid.SelectedItem));
            classediterwindow.ShowDialog();
        }
    }
}
