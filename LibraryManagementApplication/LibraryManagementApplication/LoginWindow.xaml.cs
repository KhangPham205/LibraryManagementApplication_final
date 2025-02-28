using System.Windows;

namespace LibraryManagementApplication
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            loginframe.Content = new SignInPage();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            forgot.Visibility = Visibility.Collapsed;
            singin.Visibility = Visibility.Visible;
        }

        private void Hyperlink_Click_1(object sender, RoutedEventArgs e)
        {
            forgot.Visibility = Visibility.Visible;
            singin.Visibility = Visibility.Collapsed;
        }
    }
}
