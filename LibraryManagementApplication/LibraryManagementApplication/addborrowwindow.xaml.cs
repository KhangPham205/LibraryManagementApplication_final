using LibraryManagementApplication.ViewModel.ClassViewModel;
using LibraryManagementApplication.ViewModel;
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
using System.Collections.ObjectModel;

namespace LibraryManagementApplication
{
    /// <summary>
    /// Interaction logic for addborrowwindow.xaml
    /// </summary>
    public partial class addborrowwindow : Window
    {
        private LibraryDbContext _context;
        public addborrowwindow()
        {
            InitializeComponent();

            _context = new LibraryDbContext();
            LoadBookTitles();
        }

        private void LoadBookTitles()
        {
            tensachtb.ItemsSource = _context.DauSachs.Select(t => t.TenDauSach).ToList();
            tendg.ItemsSource = _context.DocGias.Select(t => t.MaDG).ToList();
        }
        private void Tensachtb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tensachtb.SelectedItem != null)
            {
                string selectedBookTitle = tensachtb.SelectedItem.ToString();
                isbn.ItemsSource = _context.Sachs.Where(t => t.TenDauSach == selectedBookTitle && t.TrangThai == "Có sẵn").Select(t => t.ISBN).ToList();
            }
        }

        private void okeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
