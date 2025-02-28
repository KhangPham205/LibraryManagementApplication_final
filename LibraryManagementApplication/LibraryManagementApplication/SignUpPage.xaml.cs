using LibraryManagementApplication.ViewModel.ClassViewModel;
using System.Windows;
using System.Windows.Controls;

namespace LibraryManagementApplication
{
    /// <summary>
    /// Interaction logic for SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        public SignUpPage()
        {
            InitializeComponent();
        }
        private void signup_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra nếu tài khoản được thêm thành công
            bool isSuccess = (this.DataContext as TaiKhoanViewModel).IsAdded;

            if (isSuccess)
            {
                // Hiển thị thông báo thành công
                EXMessagebox.Show("Nhân viên đã được thêm thành công.", "Thông báo");

                // Đóng cửa sổ đăng ký
            }
        }
    }
}
