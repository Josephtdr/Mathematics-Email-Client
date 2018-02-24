using System.Windows;
/// <summary>
/// This window is used to display to the user 
/// if they are correct or not when answering questions
/// </summary>
namespace The_Email_Client {
    public partial class MathmaticsCorrectIncorrectWindow : Window {
        //constuctor
        public MathmaticsCorrectIncorrectWindow(bool correct) {
            InitializeComponent();
            //if the user is correct display the correct image
            if (correct) image.Visibility = Visibility.Visible;
            //if the user is incorrect display the incorrect image
            else image2.Visibility = Visibility.Visible;
        }

        private void Okbutton_Click(object sender, RoutedEventArgs e) {
            Close();//closes the window 
        }
    }
}
