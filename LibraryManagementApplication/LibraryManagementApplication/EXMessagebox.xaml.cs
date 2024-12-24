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
    /// Interaction logic for EXMessagebox.xaml
    /// </summary>
    public partial class EXMessagebox : Window
    {
        public MessageBoxResult Result { get; private set; }
        public EXMessagebox(string message, string title = "Message")
        {
            InitializeComponent();
            MessageText.Text = message;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            Close();
        }

        public static MessageBoxResult Show(string message, string title = "Message")
        {
            EXMessagebox box = new EXMessagebox(message, title);
            box.ShowDialog();
            return box.Result;
        }
    }
}
