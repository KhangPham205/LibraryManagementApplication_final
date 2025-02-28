using LibraryManagementApplication.Model.Class;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LibraryManagementApplication.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        // Properties for binding to the UI
        private string _userID;
        public string UserID
        {
            get { return _userID; }
            set
            {
                if (_userID == "" || _userID != value)
                {
                    _userID = value;
                    OnPropertyChanged(nameof(UserID));
                }
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password == "" || _password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        private bool _isLoginSuccessful = false;
        public bool IsLoginSuccessful
        {
            get { return _isLoginSuccessful; }
            set
            {
                if (_isLoginSuccessful != value)
                {
                    _isLoginSuccessful = value;
                    OnPropertyChanged(nameof(IsLoginSuccessful));
                }
            }
        }

        private TaiKhoan _taiKhoan;
        public TaiKhoan taiKhoan
        {
            get { return _taiKhoan; }
            set
            {
                if (_taiKhoan != value)
                {
                    _taiKhoan = value;
                    OnPropertyChanged(nameof(taiKhoan));
                }
            }
        }


        // Command for login action
        public ICommand LoginCommand { get; set; }
        public ICommand LoginPageCommand { get; set; }
        public ICommand SignupCommand { get; set; }
        public ICommand RecoverCommand { get; set; }
        public LoginViewModel()
        {
            LoginCommand = new RelayCommand<Page>((p) => true, (p) => Login(p));
            LoginPageCommand = new RelayCommand<Frame>((p) => true, (p) => p.Content = new SignInPage());
            SignupCommand = new RelayCommand<Frame>((p) => true, (p) => p.Content = new SignUpPage());
            RecoverCommand = new RelayCommand<Frame>((p) => true, (p) => p.Content = new RecoverPage());
        }

        private void Login(Page p)
        {
            using (var context = new LibraryDbContext())
            {
                Window mainwindow;
                var user = context.TaiKhoans.FirstOrDefault(u => (u.UserID == UserID || u.Email == UserID) && u.Password == Password);
                if (user != null)
                {
                    IsLoginSuccessful = true;
                    GlobalData.LoginUser = user;
                    var mainViewModel = new MainViewModel();

                    if (user.Loai == "AD")
                        mainwindow = new MainWindow1();
                    else
                        mainwindow = new MainWindow2();
                    mainwindow.DataContext = mainViewModel;  // Gán DataContext cho MainWindow
                    mainwindow.Show();
                    Window.GetWindow(p).Close();
                }
                else
                {
                    IsLoginSuccessful = false;
                }

                if (!IsLoginSuccessful)
                    //    MessageBox.Show("Đăng nhập thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    //else
                    EXMessagebox.Show("Đăng nhập thất bại", "Thông báo");
            }
        }
    }

    public static class PasswordBoxHelper
    {
        public static DependencyProperty BindPasswordProperty =
            DependencyProperty.RegisterAttached("BindPassword", typeof(string), typeof(PasswordBoxHelper), new PropertyMetadata(string.Empty, OnBindPasswordChanged));
        public static void SetBindPassword(DependencyObject element, string value)
        {
            element.SetValue(BindPasswordProperty, value);
        }
        public static string GetBindPassword(DependencyObject element)
        {
            return (string)element.GetValue(BindPasswordProperty);
        }
        private static void OnBindPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                // sự kiện PasswordChanged để cập nhật giá trị Password trong ViewModel
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
            }
        }
        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                // Cập nhật giá trị vào thuộc tính BindPassword
                SetBindPassword(passwordBox, passwordBox.Password);
            }
        }
    }
}
