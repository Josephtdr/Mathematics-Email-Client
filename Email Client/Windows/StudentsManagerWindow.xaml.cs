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


namespace The_Email_Client
{
    /// <summary>
    /// Interaction 
    /// 
    /// 
    /// 
    /// 
    /// ic for ContactsManagerWindows.xaml
    /// </summary>
    /// 
    

    public partial class StudentsManagerWindow : Window {
        List<string> Classemailslist = new List<string>();
        List<string> emaillist = new List<string>();
        int Class_ID { get; set; }

        public StudentsManagerWindow(Class editableclass) {
            InitializeComponent();
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Enter) 
                && addcontactButton.IsEnabled) CreateStudent(); };
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Escape)) Close(); };
            Class_ID = editableclass.ID;
            Updatetable("","");
        }

        private void UpdateClassemailslist() {//updates list of students in the current emaillist
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING); //sets up connection to database
            try {
                cnctDTB.Open(); //opens connection
                OleDbCommand cmd = new OleDbCommand($"Select Students.Email FROM Students, Class_Lists WHERE " + 
                    $"Students.ID = Class_Lists.Student_ID AND Class_ID = { Class_ID };", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();
                Classemailslist.Clear();
                while (reader.Read())
                    Classemailslist.Add(reader[0].ToString());
            }
            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); } //closes connection
        }

        private void Updatetable(string searchemailValue, string searchnameValue) {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING); //sets up connection to database
            UpdateClassemailslist();
            try {
                cnctDTB.Open(); //opens connection
                string InsertSql = $"SELECT * FROM Students" +
                    $" WHERE Email LIKE '%{searchemailValue}%' AND Name LIKE '%{searchnameValue}%';";
                OleDbCommand cmd = new OleDbCommand(InsertSql, cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();
                StudentsDataGrid.Items.Clear();
                while (reader.Read()) {
                        StudentsDataGrid.Items.Add(new Student {
                            ID = Convert.ToInt16(reader[0]),
                            Name = Common.Cleanstr(reader[1]),
                            EmailAddress = Common.Cleanstr(reader[2]),
                            InClass = Classemailslist.Contains(Common.Cleanstr(reader[2]))
                    });
                }
            }
            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); } //closes connection
        }

        private void RemovecontactButton_Click(object sender, RoutedEventArgs e) {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the students(s) and remove them from all classes.", "Question?", MessageBoxButton.YesNo);
            if ( result == MessageBoxResult.Yes) {
                OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
                try {
                    cnctDTB.Open();
                    foreach (Student student in StudentsDataGrid.SelectedItems) {
                        OleDbCommand cmd = new OleDbCommand($"DELETE FROM Students WHERE ID ={student.ID};", cnctDTB);
                        cmd.ExecuteNonQuery(); //removes the student from the database
                        cnctDTB.Close(); cnctDTB.Open(); //reopens connection
                        cmd.CommandText = $"DELETE FROM Class_Lists WHERE Student_ID = {student.ID};";
                    }
                }
                catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
                finally { cnctDTB.Close(); }
                searchEmailTextBox.Clear();
                searchNameTextBox.Clear();
                Updatetable("","");
            }
        }
        private void SearchTextBoxes_TextChanged(object sender, TextChangedEventArgs e) {
            Updatetable(searchEmailTextBox.Text, searchNameTextBox.Text);
        }

        private void CreatestudentButton_Click(object sender, RoutedEventArgs e) {
            CreateStudent();
        }

        private void CreateStudent() {
            if (Addcontacterrorchecking(emailtextbox.Text.ToString())) {//makes sure the student doesnt already exists and has a valid email
                OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING); //sets up connection to database
                try {
                    cnctDTB.Open();
                    OleDbCommand cmd = new OleDbCommand($"INSERT INTO Students (Name, Email) "+
                        $"VALUES ('{ nametextbox.Text }','{ emailtextbox.Text }');", cnctDTB);
                    cmd.ExecuteNonQuery();//creates a new student in the database
                }
                catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
                finally { cnctDTB.Close(); } //closes connection
                emailtextbox.Clear(); searchNameTextBox.Clear();
                nametextbox.Clear(); searchEmailTextBox.Clear(); 
                Updatetable("","");
            }
        }

        private bool Addcontacterrorchecking(string email) {
            if (Common.Preexistingemail(email, emaillist) && Common.Inccorectemailformat(email)) return true;
            else return false;
        }

        private void Addcontacttextboxes_TextChanged(object sender, TextChangedEventArgs e) {
            addcontactButton.IsEnabled = (string.IsNullOrWhiteSpace(emailtextbox.Text) ||
                string.IsNullOrWhiteSpace(nametextbox.Text)) ? false : true;
        }

        private void addstudentClassButton_Click(object sender, RoutedEventArgs e) {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING); //sets up connection to database
            try {
                cnctDTB.Open(); //opens connection
                foreach (Student student in StudentsDataGrid.SelectedItems) {
                    if (!student.InClass) {
                        OleDbCommand cmd = new OleDbCommand($"INSERT INTO Class_Lists (Class_ID, Student_ID) " +
                            $"VALUES ({Class_ID},{student.ID});", cnctDTB);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); } //closes connection
            Updatetable("", "");
        }

        private void RemoveFromClassButton_Click(object sender, RoutedEventArgs e) {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try {
                cnctDTB.Open();
                foreach (Student student in StudentsDataGrid.SelectedItems) {
                    if (student.InClass) {
                        OleDbCommand cmd = new OleDbCommand($"DELETE FROM Class_Lists WHERE Class_ID = {Class_ID}" +
                            $" AND Student_ID = {student.ID};", cnctDTB);
                        cmd.ExecuteNonQuery();
                        //cnctDTB.Close(); cnctDTB.Open();
                        //cmd.CommandText = $"SELECT * FROM Class_Lists WHERE Student_ID = {student.ID};";
                        //OleDbDataReader Reader = cmd.ExecuteReader();
                        //if(!Reader.HasRows) {
                        //    cnctDTB.Close(); cnctDTB.Open();
                        //    cmd.CommandText = $"DELETE FROM Students WHERE Student_ID = {student.ID};";
                        //    cmd.ExecuteNonQuery();
                        //} //deleletes students which are in no classes [optional]
                    }
                }
            }
            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); }
            Updatetable("", "");
        }
    }
}

