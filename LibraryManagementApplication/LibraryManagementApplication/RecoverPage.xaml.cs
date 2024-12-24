using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace LibraryManagementApplication
{
    /// <summary>
    /// Interaction logic for RecoverPage.xaml
    /// </summary>
    public partial class RecoverPage : Page
    {
        DispatcherTimer dispatcherTimer;
        int time = 6;
        public RecoverPage()
        {
            InitializeComponent();
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (time > 0)
            {
                time--;
                countdown.Text = $"Mail sẽ được gửi đến bạn trong {time}s";
            }
            else if (time == -3)
            {
                countdown.Visibility = Visibility.Collapsed;
                dispatcherTimer.Stop();
            }
            else
            {
                time--;
                countdown.Text = "Mail đã được gửi đến bạn!";
            }
        }

        private void recover_Click(object sender, RoutedEventArgs e)
        {
            countdown.Visibility = Visibility.Visible;
            dispatcherTimer.Start();
        }
    }
}
