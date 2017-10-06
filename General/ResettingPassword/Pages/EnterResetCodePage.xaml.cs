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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace The_Email_Client
{
    /// <summary>
    /// Interaction logic for EnterResetCodePage.xaml
    /// </summary>
    public partial class EnterResetCodePage : Page
    {
        protected Action ShowResetPasswordPage { get; set; }
        public EnterResetCodePage(Action ShowResetPasswordPage)
        {
            InitializeComponent();
            this.ShowResetPasswordPage = ShowResetPasswordPage;
            KeyDown += delegate {
                if (Keyboard.IsKeyDown(Key.Enter))
                    if (Convert.ToInt32(ResetCodeTextbox.Text) == ForgottonPasswordPage.ResetCode)
                        ShowResetPasswordPage();
            };
        }

        private void SumbitResetCode_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(ResetCodeTextbox.Text) == ForgottonPasswordPage.ResetCode)
                ShowResetPasswordPage();
            else MessageBox.Show("Code is incorrect.", "Error!");
        }
    }
}
