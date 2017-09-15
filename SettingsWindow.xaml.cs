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
using System.ComponentModel;

namespace The_Email_Client
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        protected BindingExpression[] expressions;
        public Settings SettingsObject { get; set; }
        
        protected override void OnClosing(CancelEventArgs e)
        {
            if (Title == "Settings Window - Unsaved")
            {
                switch (MessageBox.Show("Are you sure you want to Quit without Saving?", "??", MessageBoxButton.YesNo))
                {
                    case MessageBoxResult.Yes:
                        break;
                    case MessageBoxResult.No:
                        e.Cancel = true;
                        break;
                    default:
                        break;
                }
            }
        }


        public SettingsWindow(Settings settings)
        {
            SettingsObject = new Settings();
            if (settings != null) SettingsObject = settings;
            else SettingsObject = SettingsObject.defaults();

            InitializeComponent();

            DataContext = SettingsObject;
            BindingExpression EmailBind = UserEmailBox.GetBindingExpression(TextBox.TextProperty);
            BindingExpression NameBind = UserNameBox.GetBindingExpression(TextBox.TextProperty);
            BindingExpression PortBind = PortBox.GetBindingExpression(TextBox.TextProperty);
            BindingExpression ServerBind = ServerBox.GetBindingExpression(TextBox.TextProperty);
            expressions = new BindingExpression[] { EmailBind, NameBind, PortBind, ServerBind};
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (BindingExpression bind in expressions) bind.UpdateSource();
            SettingsObject.UpdateDatabasefromSettings(SettingsObject);
            SaveButton.IsEnabled = false;
            Title = "Settings Window - Saved";
        }
        
        private void SaveExitButton_Click(object sender, RoutedEventArgs e)
        {
            Title = "Settings Window - Saved";
            foreach (BindingExpression bind in expressions) bind.UpdateSource();
            SettingsObject.UpdateDatabasefromSettings(SettingsObject);
            Close();
        }
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            bool defaultvalues = true;
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try
            {  
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand($"SELECT * FROM Profiles WHERE Email='{Common.Email}'", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader[2].ToString() != UserNameBox.Text || reader[3].ToString() != PortBox.Text
                        || reader[4].ToString() != ServerBox.Text) defaultvalues = false;
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

            if (!defaultvalues)
            {
                Title = "Settings Window - Unsaved";
                SaveButton.IsEnabled = true;
            }
            else Title = "Settings Window";
        }
        
        protected bool visible = false;
        
    }
    public class Settings 
    {
        public string Email { get; set; }
        public string Port { get; set; }
        public string Server { get; set; }
        public string PasswordID { get; set; }
        public string Name { get; set; }

        public Settings defaults()
        {
            Settings Defaults = new Settings()
            {
                Email = "testofcsharperinoemailerino@gmail.com",
                PasswordID = "1",
                Name = "Stool",
                Port = "587",
                Server = "smtp.gmail.com"
            };
            return Defaults;
        }

        public Settings UpdateSettingsfromDB(string email)
        {
            Settings newsettings = new Settings();

            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try
            {
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand($"SELECT * FROM Profiles WHERE Email='{email}'", cnctDTB);
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    newsettings = new Settings
                    {
                        Email = Common.Cleanstr((string)reader[1]),
                        PasswordID = Common.Cleanstr(reader[5].ToString()),
                        Name = Common.Cleanstr((string)reader[2]),
                        Port = Common.Cleanstr(reader[3].ToString()),
                        Server = Common.Cleanstr((string)reader[4])
                    };
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
            return newsettings;
        }

        public void UpdateDatabasefromSettings(Settings settings)
        {
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try
            {
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand($"UPDATE Profiles SET Name ='{ settings.Name }', Port ={ Convert.ToInt16(settings.Port)}, Server='{ settings.Server }' Where Email='{ Common.Email }'", cnctDTB);
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
        }
    }
}
