using LibraryManagementApplication.Model.Class;
using LibraryManagementApplication.ViewModel;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
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
    /// Interaction logic for mainpage.xaml
    /// </summary>
    public partial class mainpage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public LibraryDbContext context = new LibraryDbContext();
        public int availableBooksCount { get; set; }
        public int borrowedBooksCount { get; set; }
        public int lostBooksCount { get; set; }
        public Func<ChartPoint, string> PointLabel { get; set; }
        public SeriesCollection Series { get; set; }
        public List<string> Labels { get; set; }
        public string[] Months { get; set; }
        public Func<double, string> Formatter { get; set; }
        private string _namthongke;
        public string namthongke
        {
            get => _namthongke;
            set
            {
                if (_namthongke != value)
                {
                    _namthongke = value;
                    OnPropertyChanged();
                }
            }
        }
        private SeriesCollection _lineSeries;
        public SeriesCollection lineSeries
        {
            get => _lineSeries;
            set
            {
                if (_lineSeries != value)
                {
                    _lineSeries = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _yearthongke;
        public int yearthongke
        {
            get => _yearthongke;
            set
            {
                if (_yearthongke != value)
                {
                    _yearthongke = value;
                    namthongke = $"Năm {value}";
                    lineSeries = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Số sách mượn",
                        Values = new ChartValues<int>(GetBooksBorrowedByMonth())
                    }
                };
                    OnPropertyChanged();
                }
            }
        }
        private DateTime? _startdate;
        public DateTime? startdate
        {
            get { return _startdate; }
            set
            {
                if (_startdate != value)
                {
                    _startdate = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? _enddate;
        public DateTime? enddate
        {
            get { return _enddate; }
            set
            {
                if (_enddate != value)
                {
                    _enddate = value;
                    OnPropertyChanged();
                }
            }
        }

        public mainpage()
        {
            InitializeComponent();
            SoDauSachTextBox.Text = $"{context.DauSachs.Count()}";
            SoSachTextBox.Text = $"{context.Sachs.Count()}";
            SoDocGiaTextBox.Text = $"{context.DocGias.Count()}";
            SoDonMuonTextBox.Text = $"{context.DonMuons.Count()}";
            _yearthongke = int.Parse(DateTime.Now.Year.ToString());
            _namthongke = $"Năm {_yearthongke.ToString()}";
            datagridDSMuon.ItemsSource = context.DonMuons.ToList();
            
            // Gán danh sách dữ liệu cho DataGrid
            PointLabel = chartPoint => string.Format("{0},({1:p0})", chartPoint.Y, chartPoint.Participation);
            DataContext = this;
            //Biểu đồ cột
            Series = new SeriesCollection()
            {
                new RowSeries
                {
                    Title = "Được mượn:",
                    Values = new ChartValues<int>(Top5TheLoaiSachs().Select(t => t.SoLuongMuon))
                }
            };
            Labels = Top5TheLoaiSachs().Select(t => t.TenTL).ToList();

            // Biểu đồ đường
            UpdateLineSeries();
            Months = new[] { "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12" };
            Formatter = value => value.ToString();

            // Biểu đồ tròn
            foreach (var item in context.Sachs.ToList())
            {
                if (item.TrangThai == "Có sẵn")
                    availableBooksCount++;
                else if (item.TrangThai == "Được mượn")
                    borrowedBooksCount++;
                else
                    lostBooksCount++;
            }

            PieSeriesAvailable.Values = new ChartValues<int> { availableBooksCount };
            PieSeriesBorrowed.Values = new ChartValues<int> { borrowedBooksCount };
            PieSeriesLost.Values = new ChartValues<int> { lostBooksCount };
        }
        public List<TheLoaiReport> Top5TheLoaiSachs()
        {
            // Kết nối các bảng: CTDM, Sach, DauSach, TheLoai
            var query = from ctdm in context.CTDMs
                        join sach in context.Sachs on new { ctdm.MaDauSach, ctdm.ISBN } equals new { sach.MaDauSach, sach.ISBN }
                        join dausach in context.DauSachs on sach.MaDauSach equals dausach.MaDauSach
                        join theloai in context.TheLoais on dausach.MaTL equals theloai.MaTL
                        group theloai by theloai.MaTL into grouped
                        select new
                        {
                            TheLoai = grouped.Key,
                            SoLuongMuon = grouped.Count() // Đếm số lần thể loại này được mượn
                        };

            // Sắp xếp theo số lượng mượn giảm dần và lấy top 5
            var top5TheLoai = query.OrderBy(x => x.SoLuongMuon)
                                   .Take(5)
                                   .Select(x => new TheLoaiReport
                                   {
                                       TenTL = context.TheLoais.FirstOrDefault(t => t.MaTL == x.TheLoai).TenTL,
                                       SoLuongMuon = x.SoLuongMuon // Trả về số lượng mượn cùng với tên thể loại
                                   })
                                   .ToList();

            return top5TheLoai;
        }
        public List<int> GetBooksBorrowedByMonth()
        {
            var borrowData = new List<int>(new int[12]); // Mảng lưu số lượng mượn theo tháng

            // Lấy dữ liệu từ bảng DonMuon và nhóm theo tháng
            var query = from donMuon in context.DonMuons
                        join ctdm in context.CTDMs on new {donMuon.MaMuon} equals new {ctdm.MaMuon}
                        where donMuon.NgayMuon != null && donMuon.NgayMuon.Year == _yearthongke
                        group donMuon by donMuon.NgayMuon.Month into grouped
                        select new
                        {
                            Month = grouped.Key,
                            Count = grouped.Count()
                        };

            // Cập nhật số lượng sách mượn vào từng tháng trong mảng borrowData
            foreach (var data in query)
            {
                borrowData[data.Month - 1] = data.Count; // Gán số sách mượn vào đúng tháng
            }

            return borrowData;
        }
       

        public void backyear_Click(object sender, RoutedEventArgs e)
        {
            yearthongke--;
            UpdateLineSeries();

        }

        public void nextyear_Click(object sender, RoutedEventArgs e)
        {
            yearthongke++;
            UpdateLineSeries();

        }
        private void UpdateLineSeries()
        {
            lineSeries = new SeriesCollection()
            {
                new LineSeries
                {
                    Title = "Số sách mượn",
                    Values = new ChartValues<int>(GetBooksBorrowedByMonth()),
                }
            };
        }
       protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void show_Click(object sender, RoutedEventArgs e)
        {
            if (startdate.HasValue && enddate.HasValue) 
            datagridDSMuon.ItemsSource= context.DonMuons
                .Where(d => d.NgayMuon >= startdate.Value && d.NgayMuon <= enddate.Value)
                .ToList();
            else if (startdate.HasValue && !enddate.HasValue) 
                datagridDSMuon.ItemsSource = context.DonMuons
                .Where(d => d.NgayMuon >= startdate.Value)
                .ToList();
            else 
                datagridDSMuon.ItemsSource = context.DonMuons
                .Where(d => d.NgayMuon <= enddate.Value)
                .ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            startdate = null;
            enddate = null;
            datagridDSMuon.ItemsSource = context.DonMuons.ToList();
        }
    }

    public class TheLoaiReport
    {
        public string TenTL { get; set; }  // Tên thể loại
        public int SoLuongMuon { get; set; } // Số lượng sách mượn
    }
}
