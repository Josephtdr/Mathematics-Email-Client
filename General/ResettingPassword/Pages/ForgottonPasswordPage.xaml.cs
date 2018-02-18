using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using The_Email_Client;

namespace The_Email_Client
{
    /// <summary>
    /// Interaction logic for ForgottonPasswordPage.xaml
    /// </summary>
    public partial class ForgottonPasswordPage : Page {
        public static int ResetCode { get; set; }
        public static string UserName { get; set; }
        protected Action ShowenterResetCodePage { get; set; }
        public ForgottonPasswordPage(Action ShowenterResetCodePage) {
            InitializeComponent();
            this.ShowenterResetCodePage = ShowenterResetCodePage;
            KeyDown += delegate {
                if (Keyboard.IsKeyDown(Key.Enter))
                    SendResetEmail();
            };
        }

        private void SendResetEmail() {//Verifys a users email, then sends them a reset code.
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
                catch (Exception error) { System.Windows.Forms.MessageBox.Show(error.Message); }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            SendResetEmail(); //Runs the function which will reset the users email
        }
    }
}
