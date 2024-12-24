using LibraryManagementApplication.Model.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
namespace LibraryManagementApplication.ViewModel
{
    public class SignupViewModel : BaseViewModel
    {
        public ICommand SignupCommand { get; set; }
        public SignupViewModel()
        {
            SignupCommand = new RelayCommand<Window>((p) => true, (p)=> {signup(); p.Close();  });
        }
        public void signup() { }//xu ly dang ky
    }
}