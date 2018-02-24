using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.OleDb;

namespace The_Email_Client {
    /// <summary>
    /// Page in which the user can register an account
    /// </summary>
    public partial class RegistrationPage : Page {

        protected Action ShowLoginPage { get; set; }

        public RegistrationPage(Action ShowLoginPage) {
            InitializeComponent();
            this.ShowLoginPage = ShowLoginPage;

            //allows the user to sign up with the enter key
            KeyDown += delegate {
                if (Keyboard.IsKeyDown(Key.Enter)) {
                    SignUp(); 
                 }
            };
        }
        //function to sign the user up
        private void SignUp() {
            //checks if the user has entered valid information
            if (UserNameAlreadyExists(UserNameTextBox.Text)
                && NonNullFields()
                && PasswordsMatch()) {
                RegisterUser();//funtion to register user
            }
        }
        private void SignUpbutton_Click(object sender, RoutedEventArgs e) {
            SignUp();//function to sign up user
        }
        //function to check textboxes are not null or empty
        private bool NonNullFields() {
            if (!string.IsNullOrWhiteSpace(EmailTextBox.Text) 
                && !string.IsNullOrWhiteSpace(NameTextBox.Text)
                && !string.IsNullOrWhiteSpace(UserNameTextBox.Text)
                && !string.IsNullOrWhiteSpace(Passwordbox.Password)
                ) return true;
            else {
                //informs user in the case of an error
                MessageBox.Show("No fields can be left empty.","Error!");
                return false;
            }
        }
        //checks if the username a user wishes to use already exists
        private bool UserNameAlreadyExists(string UserName) {
            //opens database connection
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try {
                cnctDTB.Open();//opens database connection
                //SQL statement to find all entries with a username the same as the deisred username
                OleDbCommand cmd = new OleDbCommand($"SELECT UserName FROM Profiles WHERE [UserName]='{UserName}'", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();
                //if the database has rows, returns false as the user cannot have their username
                while (reader.Read()) if (reader.HasRows) {
                        MessageBox.Show("UserName Already Exists!", "Error!");
                        return false;
                    }
            }
            //shows an error to the user in the case of an error
            catch (Exception err) { MessageBox.Show(err.Message); }
            finally { cnctDTB.Close(); }//closes connection
            return true; //returns true as for the code to reach here, it means the user can use that username
        }
        //checks if the two passwords entered by the user match
        private bool PasswordsMatch() {
            if (Passwordbox.Password == PasswordboxCopy.Password) return true;
            else {
                MessageBox.Show("Passwords do not Match!", "Error");
                return false;
            }
        }
       //Function to register the user
        private void RegisterUser() {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try {
                cnctDTB.Open();
                //SQL statement to create a new entry in the database with the users 
                //desired information
                OleDbCommand cmd = 
                    new OleDbCommand($"INSERT INTO Profiles ([Name],[UserName],[Password],[Email],[Settings_ID]) " + 
                    $"VALUES ('{NameTextBox.Text}','{UserNameTextBox.Text}','{Encryption.HashString(Passwordbox.Password)}',"+
                    $"'{Encryption.HashString(EmailTextBox.Text)}', 1);", cnctDTB);
                cmd.ExecuteNonQuery();
                //message box to inform user it was successfull
                MessageBox.Show("Successfully registered account.", "Success!");
                Back(); //function to return to the login page
            }
            catch (Exception err) {
                MessageBox.Show("Failed to registered account.", "Error!");
                System.Windows.MessageBox.Show(err.Message);
            }
            finally { cnctDTB.Close(); }
        }
        //function to return the user to the login page
        private void BackButton_Click(object sender, RoutedEventArgs e) {
            Back(); //function to return to the login page
        }
        //function to return to the login page
        private void Back() {
            EmailTextBox.Clear();
            NameTextBox.Clear();
            UserNameTextBox.Clear();
            ShowLoginPage?.Invoke();
        }
    }
}
