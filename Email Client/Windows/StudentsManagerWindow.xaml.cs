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
        List<string> emaillist = new List<string>();
        int Class_ID { get; set; }

        public StudentsManagerWindow(Class editableclass, List<string> EmailList) {
            InitializeComponent();
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Enter) 
                && addcontactButton.IsEnabled) Addcontact(); };
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Escape)) Close(); };
            Class_ID = editableclass.ID;
            emaillist = EmailList;
            Updatetable("","");
        }
        protected override void OnClosed(EventArgs e) {
            //overite to ask if they want to save data etc...........
            base.OnClosed(e);
        }

        public void Updatetable(string searchemailValue, string searchnameValue) {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try {
                cnctDTB.Open();
                string InsertSql = $"SELECT * FROM Students" +
                    $" WHERE Email LIKE '%{searchemailValue}%' AND Name LIKE '%{searchnameValue}%';";
                OleDbCommand cmd = new OleDbCommand(InsertSql, cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();

                StudentsDataGrid.Items.Clear();
                bool inclass = false;
                while (reader.Read()) {
                    if (emaillist.Contains(reader[2])) inclass = true;
                    else inclass = false;
                    StudentsDataGrid.Items.Add(new Student {
                        ID = Convert.ToInt16(reader[0]),
                        Name = Common.Cleanstr(reader[1]),
                        EmailAddress = Common.Cleanstr(reader[2]),
                        InClass = inclass
                    });
                }
            }
            catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); }
        }

        private void RemovecontactButton_Click(object sender, RoutedEventArgs e) {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the students(s).", "Question?", MessageBoxButton.YesNo);
            if ( result == MessageBoxResult.Yes) {
                OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
                try {
                    cnctDTB.Open();
                    foreach (Student student in StudentsDataGrid.SelectedItems) {
                        OleDbCommand cmd = new OleDbCommand($"DELETE FROM Students WHERE ID ='{student.ID}';", cnctDTB);
                        cmd.ExecuteNonQuery();
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

        private void AddcontactButton_Click(object sender, RoutedEventArgs e) {
            Addcontact();
        }

        private void Addcontact() {
            if (Addcontacterrorchecking(emailtextbox.Text.ToString())) {
                OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
                try {
                    cnctDTB.Open();
                    OleDbCommand cmd = new OleDbCommand($"INSERT INTO Students (Name, Email) "+
                        $"VALUES ('{ nametextbox.Text }','{ emailtextbox.Text }');", cnctDTB);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception err) { System.Windows.MessageBox.Show(err.Message); }
                finally { cnctDTB.Close(); }
                emailtextbox.Clear(); searchNameTextBox.Clear();
                nametextbox.Clear(); searchEmailTextBox.Clear(); 
                Updatetable("","");
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
            if (StudentsDataGrid.SelectedItems.Count == 0) removecontactButton.IsEnabled = false;
            else removecontactButton.IsEnabled = true;
        }

        private void Addcontacttextboxes_TextChanged(object sender, TextChangedEventArgs e) {
            if (string.IsNullOrWhiteSpace(emailtextbox.Text) || string.IsNullOrWhiteSpace(nametextbox.Text))
                addcontactButton.IsEnabled = false;
            else addcontactButton.IsEnabled = true;
        }

        private void UpdateDBButton_Click(object sender, RoutedEventArgs e) {

        }
    }
}
