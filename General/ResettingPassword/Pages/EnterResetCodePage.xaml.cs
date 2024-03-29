﻿using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace The_Email_Client {
    /// <summary>
    /// Page in which the user enters the reset code 
    /// </summary>
    public partial class EnterResetCodePage : Page {
        protected Action ShowResetPasswordPage { get; set; }
        protected Action Close { get; set; }
        private int Attempts { get; set; }//stores the number of attempts the user has had
        //constructor
        public EnterResetCodePage(Action ShowResetPasswordPage, Action Close){
            InitializeComponent();
            //sets up actions
            this.ShowResetPasswordPage = ShowResetPasswordPage;
            this.Close = Close;
            KeyDown += delegate {//Allows the user to press enter to check their reset code
                if (Keyboard.IsKeyDown(Key.Enter))
                    SubmitResetCode();
            };
        }
        private void SubmitResetCode() {
            if (Attempts >= 5) {//checks to see how many times the user has entered a code
                //informs user they have had too many attempts and closes
                MessageBox.Show("Too many Attempts, Closing Window", "Error!");
                Close();
            }
            //Checks if the code entered by the user is correct
            int tempnum;
            if (int.TryParse(ResetCodeTextbox.Text, out tempnum) && 
                tempnum == ForgottonPasswordPage.ResetCode)
                ShowResetPasswordPage();//Moves the user to the page in which they can reset their password
            else {
                //informs the user they have entered an incorrect code
                MessageBox.Show("Code is incorrect.", "Error!");
                ++Attempts;//Itterates the number of attempts by the user
            }
        }
        //prevents the user from typing anything but number into the relevant textbox
        private void SubmitResetCodeTextbox_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SumbitResetCode_Click(object sender, RoutedEventArgs e) {
            SubmitResetCode();//Runs the reset code function when the user presses the button
        }
    }
}
