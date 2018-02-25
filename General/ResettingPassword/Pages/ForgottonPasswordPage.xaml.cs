using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace The_Email_Client {
    /// <summary>
    /// Page for user to input email and username to get code to reset password
    /// </summary>
    public partial class ForgottonPasswordPage : Page {
        public static int ResetCode { get; set; }//variable to store reset code
        public static string UserName { get; set; }//variable to store user name of user
        protected Action ShowenterResetCodePage { get; set; }
        //constructor
        public ForgottonPasswordPage(Action ShowenterResetCodePage) {
            InitializeComponent();
            this.ShowenterResetCodePage = ShowenterResetCodePage;
            KeyDown += delegate {
                if (Keyboard.IsKeyDown(Key.Enter))
                    //allows user to send reset email with the enter key
                    SendResetEmail();
            };
        }

        private void SendResetEmail() {//Verifys a users email, then sends them a reset code.
            UserName = UserNameTextBox.Text;//sets this variable to the users username so it can be used later
            //Checks to see if the users entered email corraleates with their entered username.
            if (Encryption.VerifyPasswordorEmail(UserNameTextBox.Text, EmailTextBox.Text, false)) {
                ResetCode = Constants.Rnd.Next(10000000, 99999999);//Creates a random reset code
                try {//Creates a default email to send to the user containing the 
                    //reset code they need to reset their password
                    Email tempemail = new Email() {
                        Server = Constants.DEFAULTSERVER,
                        Port = Constants.DEFAULTPORT,
                        UserEmail = Constants.DEFAULTEMAIL,
                        UserPassword = Constants.DEFAULTPASSWORD,
                        UserName = "MartinsMathematics Client Password Reset",
                        Recipients = new string[] { EmailTextBox.Text },
                        CC = null, BCC = null, AttachmentNames = null,
                        Subject = "Reset Password Request",
                        Body = "Somebody has Requested a Password Reset for this email.\n" +
                        "If this was not you please ignore this email.\n" +
                        $"If this was you, your email reset code is as follows: {ResetCode}.\n" +
                        "Please copy this code down, " +
                        "return to the email client and follow the on screen instructions."
                    };
                    //Opens a new thread to send the email
                    new Thread(new ParameterizedThreadStart(delegate { tempemail.Send(); })) 
                    { IsBackground = true }.Start();
                    //displays the next page to the user where they will input their reset code
                    ShowenterResetCodePage();
                }
                //informs the user if there is an error
                catch (Exception error) { MessageBox.Show(error.Message); }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            SendResetEmail(); //Runs the function which will reset the users email
        }
    }
}
