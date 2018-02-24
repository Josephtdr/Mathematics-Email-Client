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
namespace The_Email_Client {
    /// <summary>
    /// Interaction logic for ForgotPasswordWindow.xaml
    /// </summary>
    public partial class ResettingPasswordWindow : Window {
        //initilises each page
        protected ForgottonPasswordPage ForgottonPasswordPage{ get; set;}
        protected EnterResetCodePage EnterResetCodePage { get; set; }
        protected ResetPasswordPage ResetPasswordPage { get; set; }
        //constructor
        public ResettingPasswordWindow() {
            InitializeComponent();
            //sets up each page with their functions to travel between
            ForgottonPasswordPage = new ForgottonPasswordPage(ShowEnterResetCodePage);
            EnterResetCodePage = new EnterResetCodePage(ShowResetPasswordPage, Close);
            ResetPasswordPage = new ResetPasswordPage(Close);
            ShowForgottonPasswordPage();
        }
        //shows the forgotten password page
        protected void ShowForgottonPasswordPage() {
            ShowPage(ForgottonPasswordPage);
        }
        //show enter reset code page
        protected void ShowEnterResetCodePage() {
            ShowPage(EnterResetCodePage);
        }
        //show reset password page
        protected void ShowResetPasswordPage() {
            ShowPage(ResetPasswordPage);
        }
        //function to show a page
        protected void ShowPage(Page page) {
            PageFrame.Content = page;
        }
    }
}
