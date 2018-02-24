using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.OleDb;

/// <summary>
/// Window for managing the students within the database
/// </summary>
namespace The_Email_Client {
    public partial class StudentsManagerWindow : Window {
        List<string> Classemailslist = new List<string>();
        List<string> emaillist = new List<string>();
        DataGrid tempstudentdatagrid = new DataGrid();
        int Class_ID { get; set; }
        //constuctor
        public StudentsManagerWindow(Class editableclass) {
            InitializeComponent();
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Enter) 
                && addcontactButton.IsEnabled) CreateStudent(); };
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Escape)) Close(); };
            Class_ID = editableclass.ID;//specifies the class window was opened from 
            Updatetable();//updates the table
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
        //function to update table
        private void Updatetable() {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING); //sets up connection to database
            UpdateClassemailslist();//updates the list of emails within the class 
            UpdateEmailsList();//updates the list of all emails
            try {
                cnctDTB.Open(); //opens connection
                //SQL statment to get all records from students
                OleDbCommand cmd = new OleDbCommand($"SELECT * FROM Students;", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();
                //clears tables
                StudentsDataGrid.Items.Clear();
                tempstudentdatagrid.Items.Clear();
                while (reader.Read()) {//loops through each record
                    StudentsDataGrid.Items.Add(new Student {//adds a student to the datagrid
                        ID = Convert.ToInt16(reader[0]),
                        Name = Common.Cleanstr(reader[1]),
                        EmailAddress = Common.Cleanstr(reader[2]),
                        InClass = Classemailslist.Contains(Common.Cleanstr(reader[2]))
                    });
                    tempstudentdatagrid.Items.Add(new Student {//adds a student to the secondary datagrid
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
        //function to create a new entry of a student within the database
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
                    cnctDTB.Open();//opens connection
                    //loops through each selected student
                    foreach (Student student in StudentsDataGrid.SelectedItems) {
                        if (student.InClass) {//checks if the student is in the class before removing them 
                            //SQL statement to remove the student from the class_list
                            OleDbCommand cmd = new OleDbCommand($"DELETE FROM Class_Lists WHERE Class_ID = {Class_ID}" +
                                $" AND Student_ID = {student.ID};", cnctDTB);
                            cmd.ExecuteNonQuery();
                            cnctDTB.Close(); cnctDTB.Open();//reopens connection
                            //SQL statement to see if the student is still in a class
                            cmd.CommandText = $"SELECT * FROM Class_Lists WHERE Student_ID = {student.ID};";
                            OleDbDataReader Reader = cmd.ExecuteReader();
                            if (!Reader.HasRows) {//if statement to see if there are any entries of the student in the table
                                cnctDTB.Close(); cnctDTB.Open();//reopens connection
                                //Sql statement to delete a student from the database
                                cmd.CommandText = $"DELETE FROM Students WHERE Student_ID = {student.ID};";
                                cmd.ExecuteNonQuery();
                            } //deleletes students which are in no classes 
                        }
                    }
                }
                //outputs error to the user
                catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
                finally { cnctDTB.Close(); //closes connection
                Updatetable();//Updates the table
            }
        }
    }
}

