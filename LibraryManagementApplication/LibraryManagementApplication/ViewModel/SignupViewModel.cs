using System.Windows;
using System.Windows.Input;

namespace LibraryManagementApplication.ViewModel
{
    public class SignupViewModel : BaseViewModel
    {
        public ICommand SignupCommand { get; set; }
        public SignupViewModel()
        {
            SignupCommand = new RelayCommand<Window>((p) => true, (p) => { signup(); p.Close(); });
        }
        public void signup() { }//xu ly dang ky
    }
}