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

namespace The_Email_Client
{
    /// <summary>
    /// Interaction logic for ForgotPasswordWindow.xaml
    /// </summary>
    public partial class ForgotPasswordWindow : Window
    {
        protected ForgottonPasswordPage forgottonPasswordPage{ get; set;}
        protected EnterResetCodePage enterResetCodePage { get; set; }
        protected ResetPasswordPage resetPasswordPage { get; set; }

        public ForgotPasswordWindow()
        {
            InitializeComponent();
            forgottonPasswordPage = new ForgottonPasswordPage(ShowenterResetCodePage);
            enterResetCodePage = new EnterResetCodePage(ShowResetPasswordPage);
            resetPasswordPage = new ResetPasswordPage(Close);
            ShowforgottonPasswordPage();
        }

        protected void ShowforgottonPasswordPage()
        {
            ShowPage(forgottonPasswordPage);
        }

        protected void ShowenterResetCodePage()
        {
            ShowPage(enterResetCodePage);
        }
        protected void ShowResetPasswordPage()
        {
            ShowPage(resetPasswordPage);
        }


        protected void ShowPage(Page page)
        {
            PageFrame.Content = page;
        }
    }
}
