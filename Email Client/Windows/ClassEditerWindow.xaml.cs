using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace The_Email_Client {
    /// <summary>
    /// used to manage the students within the class
    /// </summary>
    public partial class ClassEditerWindow : Window {
        private List<string> emaillist = new List<string>();
        private Class editableclass = new Class();
        private DataGrid tempstudentdatagrid = new DataGrid();
        //constructor
        public ClassEditerWindow(Class editableclass) {
            InitializeComponent();
            this.editableclass = editableclass;
            //sets the name of the window to be specific to the class being edited
            Title = $"ClassEditerWindow | {this.editableclass.Name}:";
            //allows user to add a student to the class with the enter key
            KeyDown += delegate {
                if (Keyboard.IsKeyDown(Key.Enter)
            && addstudentButton.IsEnabled) Addstudent();
            };
            //allows user to close the window with the escape key
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Escape)) Close(); };
            Updatetable();
        }
        //updates the table to show the list of students within the class
        public void Updatetable() {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try {
                cnctDTB.Open();
                //SQL command to select all students form within the current class
                string InsertSql = $"SELECT * FROM Students, Class_Lists WHERE " +
                    $"Students.ID = Class_Lists.Student_ID AND Class_Lists.Class_ID = {editableclass.ID};";
                OleDbCommand cmd = new OleDbCommand(InsertSql, cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();

                classStudentsDataGrid.Items.Clear();//clears the table
                emaillist.Clear();
                while (reader.Read()) {//loops through each student and adds them to the table
                    classStudentsDataGrid.Items.Add(new Student { ID = Convert.ToInt16(reader[0]), Name = Common.Cleanstr(reader[1]), EmailAddress = Common.Cleanstr(reader[2]) });
                    tempstudentdatagrid.Items.Add(new Student { ID = Convert.ToInt16(reader[0]), Name = Common.Cleanstr(reader[1]), EmailAddress = Common.Cleanstr(reader[2]) });
                    emaillist.Add(Common.Cleanstr(reader[2])); 
                }
            }
            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); }
        }
        //removes a student from the class and deletes them if they are in no classes
        private void RemovestudentButton_Click(object sender, RoutedEventArgs e) {
            //makes sure the user wishes to remove these students from the class
            MessageBoxResult result = MessageBox.Show("Are you sure you want to remove the students(s).", "Question?", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                //sets up database connection
                OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
                try {
                    cnctDTB.Open();//opens connection
                    foreach (Student student in classStudentsDataGrid.SelectedItems) {//loops through each student
                        //SQL command to delte the student from the list of students within the class
                        OleDbCommand cmd = new OleDbCommand($"DELETE FROM Class_Lists WHERE Class_ID = {editableclass.ID}" +
                            $" AND Student_ID = {student.ID};", cnctDTB);
                        cmd.ExecuteNonQuery();
                        cnctDTB.Close(); cnctDTB.Open();//reopens connection
                        //SQL command to check if student is in any classes
                        cmd.CommandText = $"SELECT * FROM Class_Lists WHERE Student_ID = {student.ID};";
                        OleDbDataReader Reader = cmd.ExecuteReader();
                        if (!Reader.HasRows) {
                            cnctDTB.Close(); cnctDTB.Open();
                            //SQL command to delete the student from the students table
                            cmd.CommandText = $"DELETE FROM Students WHERE Student_ID = {student.ID};";
                            cmd.ExecuteNonQuery();
                        } //deleletes students which are in no classes 
                    }
                }
                catch (Exception err) { MessageBox.Show(err.Message); }
                finally { cnctDTB.Close(); }//closes connection

                searchEmailTextBox.Clear(); searchNameTextBox.Clear();
                Updatetable();//updates table
            }
        }
        //runs whenever the text in the searching text boxes is changed,
        //updates the table to only contain students applicable to the users search
        private void SearchTextBoxes_TextChanged(object sender, TextChangedEventArgs e) {
            SearchTable(searchNameTextBox.Text, searchEmailTextBox.Text, classStudentsDataGrid);
        }

        //updates the table to only contain students applicable to the users search
        private void SearchTable(string searchname, string searchemail, DataGrid datagrid) {
            datagrid.Items.Clear();//clears table
            //adds students which are relavant to users search
            for (int i = 0; i < tempstudentdatagrid.Items.Count; i++) {
                if ((string.IsNullOrWhiteSpace(searchname) || (((Student)tempstudentdatagrid.Items[i]).Name.ToLower()).Contains(searchname.ToLower()))
                    && (string.IsNullOrWhiteSpace(searchemail) || (((Student)tempstudentdatagrid.Items[i]).EmailAddress.ToLower()).Contains(searchemail.ToLower())))
                    datagrid.Items.Add(tempstudentdatagrid.Items[i]);
            }
        }
        //adds a student to the class
        private void AddstudentButton_Click(object sender, RoutedEventArgs e) {
            Addstudent();
        }

        private void Addstudent() {
            //checks if the email entered is a valid email and that a student doesnt already have that email within this class
            if (Addstudenterrorchecking(emailtextbox.Text.ToString())) {
                int StudentID = 0; //variable to store student ID
                //sets up database connection
                OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
                try {
                    cnctDTB.Open();//opens connection
                    //SQL statement to check if a student within the students table exists with said email already
                    OleDbCommand cmd = new OleDbCommand($"SELECT ID FROM Students WHERE Email = '{emailtextbox.Text}';", cnctDTB);
                    OleDbDataReader Reader = cmd.ExecuteReader();
                    if (Reader.HasRows) {//if rows exists then a student with said email already exists
                        while(Reader.Read()){
                            StudentID = Convert.ToInt16(Reader[0]);
                        }
                        cnctDTB.Close(); cnctDTB.Open();//reopens connection
                        //SQL command to insert said student into the list of the class
                        cmd.CommandText = $"INSERT INTO Class_Lists (Class_ID, Student_ID) VALUES " + 
                            $"({editableclass.ID},{StudentID}) ;";
                        cmd.ExecuteNonQuery();
                    }
                    else {
                        cnctDTB.Close(); cnctDTB.Open();//reopens conneciton
                        //SQL command to insert student into students table
                        cmd.CommandText = $"INSERT INTO Students (Name, Email) " +
                            $"VALUES ('{ nametextbox.Text }','{ emailtextbox.Text }');"; 
                        cmd.ExecuteNonQuery();
                        cnctDTB.Close(); cnctDTB.Open();//reopens connection
                        //SQL command to get ID of student just entered into database
                        cmd.CommandText = $"SELECT ID FROM Students WHERE Email = '{emailtextbox.Text}'";
                        OleDbDataReader Reader2 = cmd.ExecuteReader();
                        while (Reader2.Read()) {
                            StudentID = Convert.ToInt16(Reader2[0]);
                        }
                        cnctDTB.Close(); cnctDTB.Open();//reopens connection
                        //SQL command to insert student into class list
                        cmd.CommandText = $"INSERT INTO Class_Lists (Class_ID, Student_ID) VALUES " +
                            $"({editableclass.ID},{StudentID}) ;";
                        cmd.ExecuteNonQuery();
                    }
                }
                //informs user of an error in the case of one
                catch (Exception err) { MessageBox.Show(err.Message); }
                finally { cnctDTB.Close(); }//closes connection
                emailtextbox.Clear(); searchNameTextBox.Clear();
                nametextbox.Clear(); searchEmailTextBox.Clear();
                Updatetable();//updates table
            }
        }
        //function to check if an email is of a valid format and is not already within the class
        private bool Addstudenterrorchecking(string email) {
            return (Preexistingemail(email) && Common.Inccorectemailformat(email));
            
        }
        //function to check if an email is already within the class
        private bool Preexistingemail(string email) {
            if (emaillist.Contains(email)) {
                MessageBox.Show("A student with this email already exists!", "Error");
                return false;
            }
            return true;
        }
        //Enables/disables the add student button depending on if both text boxes have values entered
        private void Addstudenttextboxes_TextChanged(object sender, TextChangedEventArgs e) {
            if (emailtextbox.Text == "" || nametextbox.Text == "") addstudentButton.IsEnabled = false;
            else addstudentButton.IsEnabled = true;
        }
        //opens the browse students window and updates table once it is closed
        private void browsestudentsButton_Click(object sender, RoutedEventArgs e) {
            StudentsManagerWindow studentsmanagerwindow = new StudentsManagerWindow(editableclass);
            studentsmanagerwindow.ShowDialog();
            Updatetable();
        }
    }
}