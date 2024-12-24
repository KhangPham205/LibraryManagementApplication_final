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
            forgot.Visibility=Visibility.Visible;
            singin.Visibility=Visibility.Collapsed;
        }
    }
}
