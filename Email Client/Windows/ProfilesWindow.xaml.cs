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
        private List<string> SettingsResetValues { get; set; }
        private List<string> EditableSettings { get; set; }

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
            EditableSettings = new List<string>();
            if (Common.Profile != null) {
                SettingsResetValues.Add(Common.Profile.Name); SettingsResetValues.Add(Common.Profile.Port);
                SettingsResetValues.Add(Common.Profile.Server); SettingsResetValues.Add(Common.Profile.UserName);
                EditableSettings.Add(Common.Profile.Name); EditableSettings.Add(Common.Profile.Port);
                EditableSettings.Add(Common.Profile.Server); EditableSettings.Add(Common.Profile.UserName);
            }
            else {
                MessageBox.Show("Settings could not be found", "Error!");
                Close();
            }
            InitializeComponent();

            DataContext = Common.Profile;
            BindingExpression EmailBind = UserEmailBox.GetBindingExpression(TextBox.TextProperty);
            BindingExpression UserNameBind = UserUserNameBox.GetBindingExpression(TextBox.TextProperty);
            BindingExpression NameBind = UserNameBox.GetBindingExpression(TextBox.TextProperty);
            BindingExpression PortBind = PortBox.GetBindingExpression(TextBox.TextProperty);
            BindingExpression ServerBind = ServerBox.GetBindingExpression(TextBox.TextProperty);
            expressions = new BindingExpression[] { EmailBind, UserNameBind, NameBind, PortBind, ServerBind};
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (BindingExpression bind in expressions) bind.UpdateSource();
            Common.Profile.UpdateDatabasefromSettings(Common.Profile);
            SaveButton.IsEnabled = false; SaveExitButton.IsEnabled = false;
            Title = "Settings Window - Saved";
            EditableSettings.Clear();
            EditableSettings.Add(UserNameBox.Text); EditableSettings.Add(PortBox.Text);
            EditableSettings.Add(ServerBox.Text); EditableSettings.Add(UserUserNameBox.Text);
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
            if (EditableSettings[1] != PortBox.Text || EditableSettings[2] != ServerBox.Text
                || EditableSettings[0] != UserNameBox.Text || EditableSettings[3] != UserUserNameBox.Text)
               defaultvalues = false;
            
            if (!defaultvalues)
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
               || ServerBox.Text != SettingsResetValues[2] || UserUserNameBox.Text != SettingsResetValues[3])
                ResetButton.IsEnabled = true;

            else ResetButton.IsEnabled = false;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            UserNameBox.Text = SettingsResetValues[0];
            PortBox.Text = SettingsResetValues[1];
            ServerBox.Text = SettingsResetValues[2];
            UserUserNameBox.Text = SettingsResetValues[3];
        }

        private void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            ResettingPasswordWindow ForgotPasswordWindow = new ResettingPasswordWindow();
            ForgotPasswordWindow.ShowDialog();
        }
    }
    
}
