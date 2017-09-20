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
    public partial class ProfilesWindow : Window
    {
        protected BindingExpression[] expressions;
        public List<string> SettingsResetValues { get; set; }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (Title == "Settings Window - Unsaved")
            {
                switch (MessageBox.Show("Are you sure you want to Quit without Saving?", "Unsaved Data!", MessageBoxButton.YesNo))
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


        public ProfilesWindow()
        {
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Escape)) Close(); };

            SettingsResetValues = new List<string>();
            if (Common.Profile != null) {
                SettingsResetValues.Add(Common.Profile.Name); SettingsResetValues.Add(Common.Profile.Port);
                SettingsResetValues.Add(Common.Profile.Server);
            }
            else {
                MessageBox.Show("Settings could not be found", "Error!");
                Close();
            }
            InitializeComponent();

            DataContext = Common.Profile;
            BindingExpression EmailBind = UserEmailBox.GetBindingExpression(TextBox.TextProperty);
            BindingExpression NameBind = UserNameBox.GetBindingExpression(TextBox.TextProperty);
            BindingExpression PortBind = PortBox.GetBindingExpression(TextBox.TextProperty);
            BindingExpression ServerBind = ServerBox.GetBindingExpression(TextBox.TextProperty);
            expressions = new BindingExpression[] { EmailBind, NameBind, PortBind, ServerBind};
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (BindingExpression bind in expressions) bind.UpdateSource();
            Common.Profile.UpdateDatabasefromSettings(Common.Profile);
            SaveButton.IsEnabled = false; SaveExitButton.IsEnabled = false;
            Title = "Settings Window - Saved";
        }
        
        private void SaveExitButton_Click(object sender, RoutedEventArgs e)
        {
            Title = "Settings Window - Saved";
            foreach (BindingExpression bind in expressions) bind.UpdateSource();
            Common.Profile.UpdateDatabasefromSettings(Common.Profile);
            Close();
        }
        private void TextChanged(object sender, TextChangedEventArgs e)
       {
            bool defaultvalues = true;
            OleDbConnection cnctDTB = new OleDbConnection(Constants.DBCONNSTRING);
            try
            {
                cnctDTB.Open();
                OleDbCommand cmd = new OleDbCommand($"SELECT * FROM Profiles WHERE Email='{Common.Profile.Email}'", cnctDTB);
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

            if(!defaultvalues)
            {
                Title = "Settings Window - Unsaved";
                SaveButton.IsEnabled = true; SaveExitButton.IsEnabled = true;
            }
            else
            {
                Title = "Settings Window";
                SaveButton.IsEnabled = false; SaveExitButton.IsEnabled = false;
            }
            if (UserNameBox.Text != SettingsResetValues[0] || PortBox.Text != SettingsResetValues[1]
               || ServerBox.Text != SettingsResetValues[2]) ResetButton.IsEnabled = true;
            else ResetButton.IsEnabled = false;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            UserNameBox.Text = SettingsResetValues[0];
            PortBox.Text = SettingsResetValues[1];
            ServerBox.Text = SettingsResetValues[2];
        }
    }
    
}
