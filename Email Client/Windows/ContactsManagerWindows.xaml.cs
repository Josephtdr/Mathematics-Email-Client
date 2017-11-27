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
    
    public class Contacts
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }
    }

    public partial class ContactsManagerWindows : Window
    {
        List<string> emaillist = new List<string>();
        

        public ContactsManagerWindows()
        {
            InitializeComponent();
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Enter) 
                && addcontactButton.IsEnabled) addcontact(); };
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Escape)) Close(); };
            updatetable("","");
        }

        public void updatetable(string searchemailValue, string searchnameValue)
        {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try
            {
                cnctDTB.Open();
                string InsertSql = $"SELECT * FROM Contacts WHERE Profile_ID={Common.Profile.ID}"+
                    $" AND Email LIKE '%{searchemailValue}%' AND Name LIKE '%{searchnameValue}%';";
                OleDbCommand cmdInsert = new OleDbCommand(InsertSql, cnctDTB);
                OleDbDataReader reader = cmdInsert.ExecuteReader();

                contactsDataGrid.Items.Clear();
                emaillist.Clear();
                while (reader.Read())
                {
                    contactsDataGrid.Items.Add(new Contacts { Name = Common.Cleanstr(reader[0]), EmailAddress = Common.Cleanstr(reader[1]) });
                    emaillist.Add(Common.Cleanstr(reader[1]));
                }
            }
            catch (Exception err)
            {
                
                System.Windows.MessageBox.Show(err.Message);
            }
            finally
            {
                cnctDTB.Close();
            }
        }

        private void removecontactButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the contact(s).", "Question?", MessageBoxButton.YesNo);
            if ( result == MessageBoxResult.Yes)
            {
                OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
                try {
                    cnctDTB.Open();
                    foreach (Contacts contact in contactsDataGrid.SelectedItems)
                    {
                        OleDbCommand cmd = new OleDbCommand($"DELETE FROM Contacts WHERE Email ='{contact.EmailAddress}'"+
                            $" AND Profile_ID={Common.Profile.ID};", cnctDTB);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception err) {
                    System.Windows.MessageBox.Show(err.Message);
                }
                finally {
                    cnctDTB.Close();
                }
                searchEmailTextBox.Clear(); searchNameTextBox.Clear();
                updatetable("","");
            }
        }
        private void searchTextBoxes_TextChanged(object sender, TextChangedEventArgs e)
        {
            updatetable(searchEmailTextBox.Text, searchNameTextBox.Text);
        }

        private void addcontactButton_Click(object sender, RoutedEventArgs e)
        {
            addcontact();
        }

        private void addcontact()
        {
            if (addcontacterrorchecking(emailtextbox.Text.ToString()))
            {
                OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
                try
                {
                    cnctDTB.Open();
                    OleDbCommand cmd = new OleDbCommand($"INSERT INTO Contacts (Name, Email, Profile_ID) "+
                        $"VALUES ('{ nametextbox.Text }','{ emailtextbox.Text }',{Common.Profile.ID});", cnctDTB);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception err)
                {
                    System.Windows.MessageBox.Show(err.Message);
                }
                finally
                {
                    cnctDTB.Close();
                }
                emailtextbox.Clear(); searchNameTextBox.Clear();
                nametextbox.Clear(); searchEmailTextBox.Clear(); 
                updatetable("","");
            }
        }

        private bool addcontacterrorchecking(string email)
        {
            if (preexistingemail(email) && Common.Inccorectemailformat(email)) return true;
            else return false;
        }

        private bool preexistingemail(string email)
        {
            foreach (string preemail in emaillist)
            {
                if (email == preemail) {
                    MessageBox.Show("A contact with this email already exists!", "Error");
                    return false;
                }
            }
            return true;
        }

        private void contactsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contactsDataGrid.SelectedItems.Count == 0) removecontactButton.IsEnabled = false;
            else removecontactButton.IsEnabled = true;
        }

        private void addcontacttextboxes_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (emailtextbox.Text == "" || nametextbox.Text == "") addcontactButton.IsEnabled = false;
            else addcontactButton.IsEnabled = true;
        }
    }
}
