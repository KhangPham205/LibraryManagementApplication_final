using LibraryManagementApplication.Model.Class;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace LibraryManagementApplication
{
    /// <summary>
    /// Interaction logic for MainWindow2.xaml
    /// </summary>
    public partial class MainWindow2 : Window
    {
        public MainWindow2()
        {
            InitializeComponent();
            Mainpage.Content = new mainpage();
            mainbutton.IsChecked = true; InitializeComponent();
            Mainpage.Content = new mainpage();
            mainbutton.IsChecked = true;
            profile.Text = $"{GlobalData.LoginUser.UserName}";
            profileemail.Text = $"{GlobalData.LoginUser.Email}";

            pictureprofile.ImageSource = LoadInitialProfileImage(); // Cập nhật ảnh hồ sơ

        }
        private BitmapImage LoadInitialProfileImage()
        {
            try
            {
                if (GlobalData.LoginUser?.ProfileImage != null && GlobalData.LoginUser.ProfileImage.Length > 0)
                {
                    return ConvertByteArrayToBitmapImage(GlobalData.LoginUser.ProfileImage);
                }

                // Trả về ảnh mặc định nếu không có ảnh trong CSDL
                return new BitmapImage(new Uri("pack://application:,,,/Icon/user.png", UriKind.Absolute));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải ảnh mặc định: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
        private void Menu_MouseEnter(object sender, MouseEventArgs e)
        {
            var image = LoadInitialProfileImage();
            if (image != null)
            {
                pictureprofile.ImageSource = image;
            }
        }
        private BitmapImage ConvertByteArrayToBitmapImage(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0) return null;

            using (var memoryStream = new MemoryStream(byteArray))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
    }
}
