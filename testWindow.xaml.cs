using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// Interaction logic for testWindow.xaml
    /// </summary>
    public partial class testWindow : Window
    {
        public testWindow()
        {
            InitializeComponent();
        }

        public string imagesrc { get; set; }
        private void button_Click(object sender, RoutedEventArgs e)
        {

            string str = $"https://cloud.nawouak.net/run/tex2png--10?{textBox.Text}";
            


            using (WebClient client = new WebClient())
            {
                client.DownloadFile( @str, @"tempLateximage.png");
            }
            

            webbrowser.Source = new Uri(@str);

        }
    }
}
