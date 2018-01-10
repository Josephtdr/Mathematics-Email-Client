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
    /// Interaction logic for ClassManagerWindow.xaml
    /// </summary>
public partial class ClassManagerWindow : Window
    {
        //List<string> ClassNames = new List<string>();

        public ClassManagerWindow()
        {
            InitializeComponent();
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Enter) 
                && CreateClassButton.IsEnabled) CreateClass(); };
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Escape)) Close(); };
            Updatetable("");
        }

        public void Updatetable(string searchnameValue) {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try {
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand($"SELECT * FROM Classes " +
                    $"WHERE Name LIKE '%{searchnameValue}%';", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();

                classDataGrid.Items.Clear();
                //ClassNames.Clear();
                while (reader.Read()) {
                    classDataGrid.Items.Add(new Class { Name = Common.Cleanstr(reader[1]), ID = Convert.ToInt16(Common.Cleanstr(reader[0])) });
                    //ClassNames.Add(Common.Cleanstr(reader[1]));
                }
            }
            catch (Exception err) {  System.Windows.MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); }
        }

        private void RemovecontactButton_Click(object sender, RoutedEventArgs e) {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the Class('s).", "Question?", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
                OleDbCommand cmd;
                try {
                    cnctDTB.Open();
                    foreach (Class Class in classDataGrid.SelectedItems) {
                        cmd = new OleDbCommand($"DELETE FROM Classes WHERE ID = {Class.ID};", cnctDTB);
                        cmd.ExecuteNonQuery();
                        cnctDTB.Close(); cnctDTB.Open();
                        cmd = new OleDbCommand($"DELETE FROM Class_Lists WHERE Class_ID = {Class.ID};", cnctDTB);
                        cmd.ExecuteNonQuery();
                        cnctDTB.Close(); cnctDTB.Open();
                    }
                }
                catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
                finally { cnctDTB.Close(); }
                searchNameTextBox.Clear();
                Updatetable("");
            }
        }
        private void SearchTextBoxes_TextChanged(object sender, TextChangedEventArgs e) {
            Updatetable(searchNameTextBox.Text);
        }

        private void AddcontactButton_Click(object sender, RoutedEventArgs e) {
            CreateClass();
            
        }

        private void CreateClass() {
            Class tempclass = new Class();
            bool set = false;
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
                set = true;
            }
            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); }
            if (set) {
                ClassEditerWindow classediterwindow = new ClassEditerWindow(tempclass);
                classediterwindow.ShowDialog();
            }
            searchNameTextBox.Clear();
            nametextbox.Clear(); 
            Updatetable("");
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
