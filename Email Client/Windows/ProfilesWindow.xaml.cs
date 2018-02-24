using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.ComponentModel;

namespace The_Email_Client {
    /// <summary>
    /// Used to manage the profile of the user
    /// </summary>
    public partial class ProfilesWindow : Window {
        protected BindingExpression[] expressions;
        private List<string> ProfileResetValues { get; set; }
        private List<string> EditableSettings { get; set; }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (Title == "Profile Window - Unsaved") {
                switch (MessageBox.Show("Are you sure you want to Quit without Saving?", "Unsaved Data!", MessageBoxButton.YesNo)){
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


        public ProfilesWindow() {
            KeyDown += delegate { if (Keyboard.IsKeyDown(Key.Escape)) Close(); };
            
            ProfileResetValues = new List<string>();
            EditableSettings = new List<string>();
            if (Common.Profile != null) {
                ProfileResetValues.Add(Common.Profile.Name); ProfileResetValues.Add(Common.Profile.Port);
                ProfileResetValues.Add(Common.Profile.Server); ProfileResetValues.Add(Common.Profile.UserName);
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

            if (Common.Profile.Email == Constants.DEFAULTEMAIL)
                ResetPasswordButton.IsEnabled = false;
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e) {
            foreach (BindingExpression bind in expressions) bind.UpdateSource();
            Common.Profile.UpdateDatabasefromProfile(Common.Profile);
            SaveButton.IsEnabled = false; SaveExitButton.IsEnabled = false;
            Title = "Profile Window - Saved";
            EditableSettings.Clear();
            EditableSettings.Add(UserNameBox.Text); EditableSettings.Add(PortBox.Text);
            EditableSettings.Add(ServerBox.Text); EditableSettings.Add(UserUserNameBox.Text);
        }
        
        private void SaveExitButton_Click(object sender, RoutedEventArgs e)
        {
            Title = "Profile Window - Saved";
            foreach (BindingExpression bind in expressions) bind.UpdateSource();
            Common.Profile.UpdateDatabasefromProfile(Common.Profile);
            Close();
        }
        private void TextChanged(object sender, TextChangedEventArgs e) {
            bool defaultvalues = true;
            if (EditableSettings[1] != PortBox.Text || EditableSettings[2] != ServerBox.Text
                || EditableSettings[0] != UserNameBox.Text || EditableSettings[3] != UserUserNameBox.Text)
               defaultvalues = false;
            
            if (!defaultvalues) {
                Title = "Profile Window - Unsaved";
                SaveButton.IsEnabled = true; SaveExitButton.IsEnabled = true;
            }
            else {
                Title = "Profile Window";
                SaveButton.IsEnabled = false; SaveExitButton.IsEnabled = false;
            }
            if (UserNameBox.Text != ProfileResetValues[0] || PortBox.Text != ProfileResetValues[1]
               || ServerBox.Text != ProfileResetValues[2] || UserUserNameBox.Text != ProfileResetValues[3])
                ResetButton.IsEnabled = true;

            else ResetButton.IsEnabled = false;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e) {
            UserNameBox.Text = ProfileResetValues[0];
            PortBox.Text = ProfileResetValues[1];
            ServerBox.Text = ProfileResetValues[2];
            UserUserNameBox.Text = ProfileResetValues[3];
        }

        private void ResetPasswordButton_Click(object sender, RoutedEventArgs e) {
            ResettingPasswordWindow ForgotPasswordWindow = new ResettingPasswordWindow();
            ForgotPasswordWindow.ShowDialog();
        }
    }
    
}
