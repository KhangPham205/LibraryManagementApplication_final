using System.Windows;
using System.Windows.Controls;

namespace LibraryManagementApplication
{
    /// <summary>
    /// Interaction logic for SignInPage.xaml
    /// </summary>
    public partial class SignInPage : Page
    {
        public SignInPage()
        {
            InitializeComponent();
        }
        private void see_Checked(object sender, RoutedEventArgs e)
        {
            visiblepassword.Text = password.Password;
            password.Visibility = Visibility.Collapsed;
            visiblepassword.Visibility = Visibility.Visible;
        }

        private void see_Unchecked(object sender, RoutedEventArgs e)
        {
            if (visiblepassword.Visibility == Visibility.Visible)
            {
                password.Password = visiblepassword.Text;
                password.Visibility = Visibility.Visible;
                visiblepassword.Visibility = Visibility.Collapsed;
            }
        }
    }
}
