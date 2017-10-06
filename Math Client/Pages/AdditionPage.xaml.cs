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
    /// Interaction logic for AdditionPage.xaml
    /// </summary>
    public partial class AdditionPage : Page
    {
        protected Action ShowHomePage { get; set; }
        public AdditionPage(Action ShowHomePage)
        {
            InitializeComponent();
            this.ShowHomePage = ShowHomePage;
        }
    }
}
