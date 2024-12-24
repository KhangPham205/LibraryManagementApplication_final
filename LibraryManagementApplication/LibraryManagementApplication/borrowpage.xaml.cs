using LibraryManagementApplication.Model.Class;
using LibraryManagementApplication.ViewModel;
using LibraryManagementApplication.ViewModel.ClassViewModel;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace LibraryManagementApplication
{
    /// <summary>
    /// Interaction logic for muonpage.xaml
    /// </summary>
    public partial class borrowpage : Page
    {
        private ObservableCollection<ThongTinDonMuon> ttdonmuons;
        private LibraryDbContext _context;
        public borrowpage()
        {
            InitializeComponent();

            ttdonmuons = new ObservableCollection<ThongTinDonMuon>();
            _context = new LibraryDbContext();

            //_ = LoadThongTin();
        }

        private void adddon_Click(object sender, RoutedEventArgs e)
        {
            var addborrowwindow = new addborrowwindow();
            addborrowwindow.ShowDialog();
        }
        private void ExportFile()
        {
            string filePath = "";
            // tạo SaveFileDialog để lưu file excel
            SaveFileDialog dialog = new SaveFileDialog();

            // chỉ lọc ra các file có định dạng Excel
            dialog.Filter = "Excel | *.xlsx | Excel 2003 | *.xls";

            // Nếu mở file và chọn nơi lưu file thành công sẽ lưu đường dẫn lại dùng
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                filePath = dialog.FileName;
            }

            // nếu đường dẫn null hoặc rỗng thì báo không hợp lệ và return hàm
            if (string.IsNullOrEmpty(filePath))
            {
                EXMessagebox.Show("Đường dẫn báo cáo không hợp lệ");
                return;
            }


            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage p = new ExcelPackage())
                {
                    //Tạo một sheet để làm việc trên đó
                    p.Workbook.Worksheets.Add("Đơn Mượn");
                    // lấy sheet vừa add ra để thao tác
                    ExcelWorksheet ws = p.Workbook.Worksheets["Đơn Mượn"];
                    if (ws == null)
                    {
                        EXMessagebox.Show("Không thể thêm worksheet.");
                        return;
                    }
                    ws.Name = "Đơn Mượn";
                    // fontsize mặc định cho cả sheet
                    ws.Cells.Style.Font.Size = 11;
                    // font family mặc định cho cả sheet
                    ws.Cells.Style.Font.Name = "Calibri";
                    // Tạo danh sách các column header
                    string[] arrColumnHeader = {
                                                "STT",
                                                "Mã Đơn Mượn",
                                                "Mã độc giả",
                                                "Mã nhân viên",
                                                "Ngày mượn",
                                                "Ngày trả dự kiến",
                                                "Sách",
                                                "ISBN",
                                                "Ghi chú"
                    };

                    // lấy ra số lượng cột cần dùng dựa vào số lượng header
                    var countColHeader = arrColumnHeader.Count();

                    int colIndex = 1;
                    int rowIndex = 4;

                    ws.Cells[1, 1].Value = "Ngày xuất báo cáo:";
                    ws.Cells[1, 2].Value = DateTime.Now.ToString();
                    ws.Cells[2, 1].Value = "Người xuất báo cáo:";
                    ws.Cells[2, 2].Value = GlobalData.LoginUser.UserName;

                    // merge các column lại từ column 1 đến số column header
                    ws.Cells[3, 1].Value = "Thống kê thông tin Đơn Mượn";
                    ws.Cells[3, 1, 3, countColHeader].Merge = true;
                    // in đậm
                    ws.Cells[3, 1, 3, countColHeader].Style.Font.Bold = true;
                    // căn giữa
                    ws.Cells[3, 1, 3, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    //tạo các header từ column header đã tạo từ bên trên
                    foreach (var item in arrColumnHeader)
                    {
                        var cell = ws.Cells[rowIndex, colIndex];

                        //set màu 
                        var fill = cell.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);

                        //căn chỉnh các border
                        var border = cell.Style.Border;
                        border.Bottom.Style =
                            border.Top.Style =
                            border.Left.Style =
                            border.Right.Style = ExcelBorderStyle.Thin;

                        //gán giá trị
                        cell.Value = item;

                        colIndex++;
                    }

                    // lấy ra danh sách UserInfo từ ItemSource của DataGrid
                    List<DonMuon> ExcelList = datagridMuon.ItemsSource.Cast<DonMuon>().ToList();
                    if (datagridMuon.ItemsSource == null)
                    {
                        EXMessagebox.Show("Không có dữ liệu để xuất!");
                        return;
                    }
                    rowIndex++;
                    int count = 1;
                    // với mỗi item trong danh sách sẽ ghi trên 1 dòng
                    foreach (var item in ExcelList)
                    {
                        // bắt đầu ghi từ cột 1. Excel bắt đầu từ 1 không phải từ 0
                        colIndex = 1;
                        List<BorrowedBook> ctdmList = new List<BorrowedBook> { };
                        foreach (var ctdms in _context.CTDMs)
                            if (ctdms != null && ctdms.MaMuon == item.MaMuon)
                            {
                                Sach sach = _context.Sachs.Where(s => s.MaDauSach == ctdms.MaDauSach && s.ISBN == ctdms.ISBN).FirstOrDefault();
                                ctdmList.Add(new BorrowedBook()
                                {
                                    TenDauSach = sach.TenDauSach,
                                    ISBN = sach.ISBN
                                });
                            }
                        var rowctdm = ctdmList.Count - 1;
                        //gán giá trị cho từng cell                      
                        ws.Cells[rowIndex, colIndex, rowIndex + rowctdm, colIndex].Merge = true;
                        ws.Cells[rowIndex, colIndex, rowIndex + rowctdm, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws.Cells[rowIndex, colIndex, rowIndex + rowctdm, colIndex++].Value = count++;
                        ws.Cells[rowIndex, colIndex, rowIndex + rowctdm, colIndex].Merge = true;
                        ws.Cells[rowIndex, colIndex, rowIndex + rowctdm, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws.Cells[rowIndex, colIndex, rowIndex + rowctdm, colIndex++].Value = item.MaMuon;
                        ws.Cells[rowIndex, colIndex, rowIndex + rowctdm, colIndex].Merge = true;
                        ws.Cells[rowIndex, colIndex, rowIndex + rowctdm, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws.Cells[rowIndex, colIndex, rowIndex + rowctdm, colIndex++].Value = item.MaDG;
                        ws.Cells[rowIndex, colIndex, rowIndex + rowctdm, colIndex].Merge = true;
                        ws.Cells[rowIndex, colIndex, rowIndex + rowctdm, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws.Cells[rowIndex, colIndex, rowIndex + rowctdm, colIndex++].Value = item.MaNV;
                        ws.Cells[rowIndex, colIndex, rowIndex + rowctdm, colIndex].Merge = true;
                        ws.Cells[rowIndex, colIndex, rowIndex + rowctdm, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws.Cells[rowIndex, colIndex, rowIndex + rowctdm, colIndex++].Value = item.NgayMuon.ToShortDateString();
                        ws.Cells[rowIndex, colIndex, rowIndex + rowctdm, colIndex].Merge = true;
                        ws.Cells[rowIndex, colIndex, rowIndex + rowctdm, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws.Cells[rowIndex, colIndex, rowIndex + rowctdm, colIndex++].Value = item.NgayTraDK.ToShortDateString();
                        ws.Cells[rowIndex, colIndex, rowIndex + rowctdm, colIndex].Merge = true;
                        ws.Cells[rowIndex, colIndex, rowIndex + rowctdm, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        int colstart = colIndex;
                        foreach (var ctdm in ctdmList)
                        {
                            colIndex = colstart;
                            ws.Cells[rowIndex, colIndex++].Value = ctdm.TenDauSach;
                            ws.Cells[rowIndex, colIndex++].Value = ctdm.ISBN;
                            rowIndex++;
                        }
                    }
                    ws.Cells.AutoFitColumns();
                    Byte[] bin = p.GetAsByteArray();
                    File.WriteAllBytes(filePath, bin);
                }
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = filePath,
                        UseShellExecute = true
                    });
                }
                catch (Exception openEx)
                {
                    EXMessagebox.Show($"File đã được lưu nhưng không thể mở: {openEx.Message}");
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Có lỗi khi lưu file!. {ex.Message}");
            }
        }

        private void export_Click(object sender, RoutedEventArgs e)
        {
            ExportFile();
        }
    }

    public class ThongTinDonMuon
    {
        public string MaMuon { get; set; }
        public string TenDG { get; set; }
        public string TenNV { get; set; }
        public string NgayMuon { get; set; }
        public string NgayTraDK { get; set; }
    }
}
