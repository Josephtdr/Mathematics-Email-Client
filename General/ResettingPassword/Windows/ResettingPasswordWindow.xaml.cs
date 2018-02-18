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

namespace The_Email_Client {
    /// <summary>
    /// Interaction logic for ForgotPasswordWindow.xaml
    /// </summary>
    public partial class ResettingPasswordWindow : Window {
        protected ForgottonPasswordPage ForgottonPasswordPage{ get; set;}
        protected EnterResetCodePage EnterResetCodePage { get; set; }
        protected ResetPasswordPage ResetPasswordPage { get; set; }

        public ResettingPasswordWindow() {
            InitializeComponent();
            ForgottonPasswordPage = new ForgottonPasswordPage(ShowEnterResetCodePage);
            EnterResetCodePage = new EnterResetCodePage(ShowResetPasswordPage, Close);
            ResetPasswordPage = new ResetPasswordPage(Close);
            ShowForgottonPasswordPage();
        }
        protected void ShowForgottonPasswordPage() {
            ShowPage(ForgottonPasswordPage);
        }
        protected void ShowEnterResetCodePage() {
            ShowPage(EnterResetCodePage);
        }
        protected void ShowResetPasswordPage() {
            ShowPage(ResetPasswordPage);
        }
        protected void ShowPage(Page page) {
            PageFrame.Content = page;
        }
    }
}
