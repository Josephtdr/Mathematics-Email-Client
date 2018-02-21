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
        DataGrid tempstudentdatagrid = new DataGrid();
        int Class_ID { get; set; }

        public StudentsManagerWindow(Class editableclass) {
            InitializeComponent();
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Enter) 
                && addcontactButton.IsEnabled) CreateStudent(); };
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Escape)) Close(); };
            Class_ID = editableclass.ID;
            Updatetable();
        }

        private void UpdateClassemailslist() {//updates list of students in the current emaillist
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING); //sets up connection to database
            try {
                cnctDTB.Open(); //opens connection
                //SQL command that fetches emails of all students currently in the chosen class
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

        private void UpdateEmailsList() {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING); //sets up connection to database
            try {
                cnctDTB.Open(); //opens connection
                //SQL command that fetches emails of all students currently in the chosen class
                OleDbCommand cmd = new OleDbCommand($"Select Students.Email FROM Students", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();
                emaillist.Clear();
                while (reader.Read())
                    emaillist.Add(reader[0].ToString());
            }
            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); } //closes connection
        }

        private void Updatetable() {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING); //sets up connection to database
            UpdateClassemailslist();
            UpdateEmailsList();
            try {
                cnctDTB.Open(); //opens connection
                OleDbCommand cmd = new OleDbCommand($"SELECT * FROM Students;", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();
                StudentsDataGrid.Items.Clear();
                while (reader.Read()) {
                    StudentsDataGrid.Items.Add(new Student {
                        ID = Convert.ToInt16(reader[0]),
                        Name = Common.Cleanstr(reader[1]),
                        EmailAddress = Common.Cleanstr(reader[2]),
                        InClass = Classemailslist.Contains(Common.Cleanstr(reader[2]))
                    });
                    tempstudentdatagrid.Items.Add(new Student {
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
                Updatetable();
            }
        }
        //Runs a command whenever the search text boxes text is changed
        private void SearchTextBoxes_TextChanged(object sender, TextChangedEventArgs e) {
            SearchTable(searchNameTextBox.Text, searchEmailTextBox.Text, StudentsDataGrid);
        }
        //Updates the table to only display items which contain the users search term
        private void SearchTable(string searchname, string searchemail, DataGrid datagrid) {
            datagrid.Items.Clear();
            for (int i = 0; i < tempstudentdatagrid.Items.Count; i++) {
                if ((string.IsNullOrWhiteSpace(searchname) || (((Student)tempstudentdatagrid.Items[i]).Name.ToLower()).Contains(searchname.ToLower()))
                    && (string.IsNullOrWhiteSpace(searchemail) || (((Student)tempstudentdatagrid.Items[i]).EmailAddress.ToLower()).Contains(searchemail.ToLower())))
                    datagrid.Items.Add(tempstudentdatagrid.Items[i]);
            }
        }
        //runs whenever the create student button is clicked
        private void CreatestudentButton_Click(object sender, RoutedEventArgs e) {
            CreateStudent();
        }

        private void CreateStudent() {
            if (Addcontacterrorchecking(emailtextbox.Text.ToString())) {//makes sure the student doesnt already exists and has a valid email
                OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING); //sets up connection to database
                try {
                    cnctDTB.Open();//opens connection
                    //SQL command to add a new student to the database
                    OleDbCommand cmd = new OleDbCommand($"INSERT INTO Students (Name, Email) "+
                        $"VALUES ('{ nametextbox.Text }','{ emailtextbox.Text }');", cnctDTB);
                    cmd.ExecuteNonQuery();//creates a new student in the database
                }
                catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
                finally { cnctDTB.Close(); } //closes connection
                emailtextbox.Clear(); searchNameTextBox.Clear();
                nametextbox.Clear(); searchEmailTextBox.Clear(); 
                Updatetable();//Updates table and clears textboxes
            }
        }
        //checks if an email already exists in the list of students 
        private bool Addcontacterrorchecking(string email) {
            return (Common.Preexistingemail(email, emaillist) && Common.Inccorectemailformat(email));
        }
        //Runs whenever the text in the boxes to create a new student is changed
        private void Addcontacttextboxes_TextChanged(object sender, TextChangedEventArgs e) {
            //Will disable the Add student button if both boxes dont have text in them
            addcontactButton.IsEnabled = (!string.IsNullOrWhiteSpace(emailtextbox.Text) &&
                !string.IsNullOrWhiteSpace(nametextbox.Text));
        }
        //Adds selected students to the current class, as long as they are not already within it
        private void addstudentClassButton_Click(object sender, RoutedEventArgs e) {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING); //sets up connection to database
            try {
                cnctDTB.Open(); //opens connection
                foreach (Student student in StudentsDataGrid.SelectedItems) {
                    if (!student.InClass) {
                        //Sql command to add a student to a class in the database
                        OleDbCommand cmd = new OleDbCommand($"INSERT INTO Class_Lists (Class_ID, Student_ID) " +
                            $"VALUES ({Class_ID},{student.ID});", cnctDTB);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); } //closes connection
            Updatetable();//updates the table
        }
        //Removes selected students from the current class
        private void RemoveFromClassButton_Click(object sender, RoutedEventArgs e) {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to remove the students(s).", "Question?", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
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
                Updatetable();//Updates the table
            }
        }
    }
}

