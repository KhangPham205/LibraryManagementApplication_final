using LibraryManagementApplication.Model.Class;
using LibraryManagementApplication.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace LibraryManagementApplication
{
    /// <summary>
    /// Interaction logic for infoxaml.xaml
    /// </summary>
    public partial class infoxaml : Page, INotifyPropertyChanged
    {
        // Triển khai sự kiện PropertyChanged như yêu cầu của INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        // Phương thức để gọi sự kiện PropertyChanged
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        private LibraryDbContext context;
        private BitmapImage _profileImage;
        public BitmapImage ProfileImage
        {
            get { return _profileImage; }
            set
            {
                if (_profileImage != value)
                {
                    _profileImage = value;
                    OnPropertyChanged();
                }
            }
        }
        public infoxaml()
        {
            context = new LibraryDbContext();

            InitializeComponent();
            DataContext = GlobalData.LoginUser;
            pictureprofile.ImageSource = LoadInitialProfileImage(); // Tải ảnh ban đầu từ CSDL
        }
        private BitmapImage LoadInitialProfileImage()
        {
            if (GlobalData.LoginUser?.ProfileImage != null && GlobalData.LoginUser.ProfileImage.Length > 0)
            {
                return ConvertByteArrayToBitmapImage(GlobalData.LoginUser.ProfileImage);
            }

            // Trả về ảnh mặc định nếu không có ảnh trong CSDL
            return new BitmapImage(new Uri("pack://application:,,,/Icon/user.png", UriKind.Absolute));
        }


        private void changemk_Click(object sender, RoutedEventArgs e)
        {
            ChangePassword changePassword = new ChangePassword();
            changePassword.ShowDialog();
        }

        private void changeprofile_Click(object sender, RoutedEventArgs e)
        {
            SelectImage();
        }

        private void SelectImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() != true)
            {
                EXMessagebox.Show("Bạn chưa chọn tệp nào.", "Thông báo");
                return;
            }

            try
            {
                var selectedFilePath = openFileDialog.FileName;

                // Kiểm tra nếu tệp không tồn tại
                if (!File.Exists(selectedFilePath))
                {
                    throw new FileNotFoundException("Tệp đã chọn không tồn tại.");
                }

                // Kiểm tra kích thước tệp (giới hạn kích thước nếu cần)
                var fileInfo = new FileInfo(selectedFilePath);
                if (fileInfo.Length > 5 * 1024 * 1024) // Ví dụ: Giới hạn 5MB
                {
                    throw new Exception("Tệp quá lớn. Vui lòng chọn ảnh có dung lượng nhỏ hơn 5MB.");
                }

                // Tải ảnh vào `BitmapImage`
                var bitmapImage = new BitmapImage();
                using (var stream = new FileStream(selectedFilePath, FileMode.Open, FileAccess.Read))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.EndInit();
                }

                // Cập nhật thuộc tính `ProfileImage`
                ProfileImage = bitmapImage;

                // Chuyển ảnh sang byte[] để lưu vào CSDL
                byte[] imageData = ConvertBitmapImageToByteArray(ProfileImage);

                // Lưu vào cơ sở dữ liệu
                using (var context = new LibraryDbContext())
                {
                    TaiKhoan taiKhoan = context.TaiKhoans.FirstOrDefault(t => t.UserID == GlobalData.LoginUser.UserID);
                    if (taiKhoan != null)
                    {
                        taiKhoan.ProfileImage = imageData;
                        context.TaiKhoans.Update(taiKhoan);
                        context.SaveChanges();
                        GlobalData.LoginUser.ProfileImage = imageData;
                    }
                    if (GlobalData.LoginUser != null)
                    {
                        ProfileImage = ConvertByteArrayToBitmapImage(GlobalData.LoginUser.ProfileImage);
                    }
                }

                EXMessagebox.Show("Ảnh đã được chọn và lưu thành công!", "Thông báo");
                
                pictureprofile.ImageSource = ProfileImage;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    MessageBox.Show($"Lỗi nội bộ: {ex.InnerException.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException}");
                }
                else
                {
                    MessageBox.Show($"Lỗi không xác định: {ex.Message}\nChi tiết: {ex.StackTrace}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private byte[] ConvertBitmapImageToByteArray(BitmapImage bitmapImage)
        {
            using (var memoryStream = new MemoryStream())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(memoryStream);
                return memoryStream.ToArray();
            }
        }

        // Convert byte[] to BitmapImage
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
