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
    /// Interaction logic for MathmaticsCorrectIncorrectWindow.xaml
    /// </summary>
    public partial class MathmaticsCorrectIncorrectWindow : Window
    {
        public MathmaticsCorrectIncorrectWindow(bool correct)
        {
            InitializeComponent();
            if (correct) image.Visibility = Visibility.Visible;
            else image2.Visibility = Visibility.Visible;
        }

        private void Okbutton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
