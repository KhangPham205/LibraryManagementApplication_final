using LibraryManagementApplication.Model.Class;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace LibraryManagementApplication.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
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
        #region commands
        public ICommand signincommand { get; set; }
        public ICommand signoutcommand { get; set; }
        public ICommand mainpagecommand { get; set; }
        public ICommand bookpagecommand { get; set; }
        public ICommand borrowpagecommand { get; set; }
        public ICommand readerpagecommand { get; set; }
        public ICommand employeepagecommand { get; set; }
        public ICommand writerpagecommand { get; set; }
        public ICommand typepagecommand { get; set; }
        public ICommand publisherpagecommand { get; set; }
        public ICommand lendpagecommand { get; set; }
        public ICommand headerbookpagecommand { get; set; }
        public ICommand infopagecommand { get; set; }
        #endregion
        public MainViewModel()
        {
            signoutcommand = new RelayCommand<Page>((p) => { return true; }, (p) => { logOut(p); });
            mainpagecommand = new RelayCommand<Frame>((p) => { return p == null || !(p.Content is mainpage); }, (p) => { p.Content = new mainpage(); });
            bookpagecommand = new RelayCommand<Frame>((p) => { return p == null || !(p.Content is bookpage); }, (p) => { p.Content = new bookpage(); });
            borrowpagecommand = new RelayCommand<Frame>((p) => { return p == null || !(p.Content is borrowpage); }, (p) => { p.Content = new borrowpage(); });
            readerpagecommand = new RelayCommand<Frame>((p) => { return p == null || !(p.Content is readerpage); }, (p) => { p.Content = new readerpage(); });
            employeepagecommand = new RelayCommand<Frame>((p) => { return p == null || !(p.Content is employeepage); }, (p) => { p.Content = new employeepage(); });
            writerpagecommand = new RelayCommand<Frame>((p) => { return p == null || !(p.Content is writerpage); }, (p) => { p.Content = new writerpage(); });
            typepagecommand = new RelayCommand<Frame>((p) => { return p == null || !(p.Content is typepage); }, (p) => { p.Content = new typepage(); });
            publisherpagecommand = new RelayCommand<Frame>((p) => { return p == null || !(p.Content is publisherpage); }, (p) => { p.Content = new publisherpage(); });
            headerbookpagecommand = new RelayCommand<Frame>((p) => { return p == null || !(p.Content is headerbookpage); }, (p) => { p.Content = new headerbookpage(); });
            infopagecommand = new RelayCommand<Frame>((p) => { return p == null || !(p.Content is infoxaml); }, (p) => { p.Content = new infoxaml(); });
            lendpagecommand = new RelayCommand<Frame>((p) => { return p == null || !(p.Content is lendpagexaml); }, (p) => { p.Content = new lendpagexaml(); });

            if (GlobalData.LoginUser != null && GlobalData.LoginUser.ProfileImage != null)
            {
                ProfileImage = ConvertByteArrayToBitmapImage(GlobalData.LoginUser.ProfileImage);
            }
        }
        public void logOut(Page p)
        {
            Window window = new LoginWindow();
            window.Show();
            Window.GetWindow(p).Close();
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
