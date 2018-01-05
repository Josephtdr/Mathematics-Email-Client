using System;
using System.Collections.Generic;
using System.Data.OleDb;
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

namespace The_Email_Client {
    /// <summary>
    /// Interaction logic for ClassEditerWindow.xaml
    /// </summary>
    public class Student {
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public int ID { get; set; }
        public bool InClass { get; set; }
        
    }
    public partial class ClassEditerWindow : Window {
        private List<string> emaillist = new List<string>();
        private Class editableclass = new Class();
        public ClassEditerWindow(Class editableclass) {
            InitializeComponent();
            this.editableclass = editableclass;
            Title = $"ClassEditerWindow | {this.editableclass.Name}:";
            KeyDown += delegate {
                if (Keyboard.IsKeyDown(Key.Enter)
            && addcontactButton.IsEnabled) Addcontact();
            };
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Escape)) Close(); };
            Updatetable("", "");
        }

        public void Updatetable(string searchemailValue, string searchnameValue) {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try {
                cnctDTB.Open();
                string InsertSql = $"SELECT * FROM Students, Class_Lists WHERE " +
                    $"Students.ID = Class_Lists.Student_ID AND Class_Lists.Class_ID = {editableclass.ID} " +
                    $"AND Students.Email LIKE '%{searchemailValue}%' AND Students.Name LIKE '%{searchnameValue}%';";
                OleDbCommand cmd = new OleDbCommand(InsertSql, cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();

                contactsDataGrid.Items.Clear();
                if (string.IsNullOrWhiteSpace(searchemailValue) && string.IsNullOrWhiteSpace(searchnameValue))
                    emaillist.Clear();
                while (reader.Read()) {
                    contactsDataGrid.Items.Add(new Student { ID = Convert.ToInt16(reader[0]),Name = Common.Cleanstr(reader[1]), EmailAddress = Common.Cleanstr(reader[2]) });
                    if(string.IsNullOrWhiteSpace(searchemailValue) && string.IsNullOrWhiteSpace(searchnameValue))
                        emaillist.Add(Common.Cleanstr(reader[2])); 
                }
            }
            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); }
        }

        private void RemovecontactButton_Click(object sender, RoutedEventArgs e) {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the contact(s).", "Question?", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
                try {
                    cnctDTB.Open();
                    foreach (Student Contact in contactsDataGrid.SelectedItems) {
                        OleDbCommand cmd = new OleDbCommand($"DELETE FROM Class_Lists WHERE Class_ID = {editableclass.ID}" +
                            $" AND Student_ID = {Contact.ID};", cnctDTB);
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = $"SELECT * FROM Class_Lists WHERE Student_ID = {Contact.ID};";
                        OleDbDataReader Reader = cmd.ExecuteReader();
                        if(!Reader.HasRows) {
                            cnctDTB.Close(); cnctDTB.Open();
                            cmd.CommandText = $"DELETE FROM Students WHERE Student_ID = {Contact.ID};";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
                finally { cnctDTB.Close(); }

                searchEmailTextBox.Clear(); searchNameTextBox.Clear();
                Updatetable("", "");
            }
        }
        private void SearchTextBoxes_TextChanged(object sender, TextChangedEventArgs e) {
            Updatetable(searchEmailTextBox.Text, searchNameTextBox.Text);
        }

        private void AddcontactButton_Click(object sender, RoutedEventArgs e) {
            Addcontact();
        }

        private void Addcontact() {
            if (Addcontacterrorchecking(emailtextbox.Text.ToString())) {
                int StudentID = 0; 
                OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
                try {
                    cnctDTB.Open();
                    OleDbCommand cmd = new OleDbCommand($"SELECT ID FROM Students WHERE Email = '{emailtextbox.Text}';", cnctDTB);
                    OleDbDataReader Reader = cmd.ExecuteReader();
                    if (Reader.HasRows) {
                        while(Reader.Read()){
                            StudentID = Convert.ToInt16(Reader[0]);
                        }
                        cmd.CommandText = $"INSERT INTO Class_Lists (Class_ID, Student_ID) VALUES " + 
                            $"({editableclass.ID},{StudentID}) ;";
                        cnctDTB.Close(); cnctDTB.Open();
                        cmd.ExecuteNonQuery();
                    }
                    else {
                        
                        cmd.CommandText = $"INSERT INTO Students (Name, Email) " +
                            $"VALUES ('{ nametextbox.Text }','{ emailtextbox.Text }');";
                        cnctDTB.Close(); cnctDTB.Open();
                        cmd.ExecuteNonQuery();
                        cnctDTB.Close(); cnctDTB.Open();
                        cmd.CommandText = $"SELECT ID FROM Students WHERE Email = '{emailtextbox.Text}'";
                        OleDbDataReader Reader2 = cmd.ExecuteReader();
                        while (Reader2.Read()) {
                            StudentID = Convert.ToInt16(Reader2[0]);
                        }
                        cnctDTB.Close(); cnctDTB.Open();
                        cmd.CommandText = $"INSERT INTO Class_Lists (Class_ID, Student_ID) VALUES " +
                            $"({editableclass.ID},{StudentID}) ;";
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception err) {
                    System.Windows.MessageBox.Show(err.Message);
                }
                finally {
                    cnctDTB.Close();
                }
                emailtextbox.Clear(); searchNameTextBox.Clear();
                nametextbox.Clear(); searchEmailTextBox.Clear();
                Updatetable("", "");
            }
        }

        private bool Addcontacterrorchecking(string email) {
            if (Preexistingemail(email) && Common.Inccorectemailformat(email)) return true;
            else return false;
        }

        private bool Preexistingemail(string email) {
            if (emaillist.Contains(email)) {
                MessageBox.Show("A contact with this email already exists!", "Error");
                return false;
            }
            return true;
        }

        private void ContactsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (contactsDataGrid.SelectedItems.Count == 0) removecontactButton.IsEnabled = false;
            else removecontactButton.IsEnabled = true;
        }

        private void Addcontacttextboxes_TextChanged(object sender, TextChangedEventArgs e) {
            if (emailtextbox.Text == "" || nametextbox.Text == "") addcontactButton.IsEnabled = false;
            else addcontactButton.IsEnabled = true;
        }

        private void browsestudentsButton_Click(object sender, RoutedEventArgs e) {
            StudentsManagerWindow studentsmanagerwindow = new StudentsManagerWindow(editableclass,emaillist);
            studentsmanagerwindow.ShowDialog();
        }
    }
}