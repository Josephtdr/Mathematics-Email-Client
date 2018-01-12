using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace The_Email_Client {
    /// <summary>
    /// Interaction logic for IndiciesPage.xaml
    /// </summary>
    public partial class IndiciesPage : Page {
        private Action ShowHomePage { get; set; }
        private Action ShowEmailPage { get; set; }
        private Action ShowCalculusPage { get; set; }
        public IndiciesPage(Action ShowHomePage, Action ShowCalculusPage, Action ShowEmailPage) {
            InitializeComponent();
            this.ShowCalculusPage = ShowCalculusPage;
            this.ShowHomePage = ShowHomePage;
            this.ShowEmailPage = ShowEmailPage;
        }
        
        public void Initialize() {
            PageSelectionComboBox.SelectedIndex = 1;
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e) {
            ShowHomePage();
        }

        private void PageSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!IsLoaded) {
                return;
            }
            switch (PageSelectionComboBox.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last()) {
                case "Calculus":
                    ShowCalculusPage();
                    break;
                case "Email": //If Selected, will display the Email Page
                    ShowEmailPage();
                    break;
            }
        }
        private void SettingsButton_Click(object sender, RoutedEventArgs e) {
            ProfilesWindow settingswindow = new ProfilesWindow();
            settingswindow.ShowDialog();
        }

        //Prevents user from typing anything bar numbers, x and operators +,-,^,/ in text boxes this check is applied to.
        private void AnswerBox_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9-/+/^/x]+"); //regex that matches disallowed text
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SumbitButton_Click(object sender, RoutedEventArgs e) {
            List<Term> ParsedString = Equation.ParseString(AnswerBox.Text);
            Diferentiation DifEqu = new Diferentiation(ParsedString);
            Integration IntEqu = new Integration(ParsedString);
            DifAnswerBlock.Text = DifEqu.FprimeEquationToString();
            IntAnswerBlock.Text = IntEqu.FprimeEquationToString();
        }
    }
}
