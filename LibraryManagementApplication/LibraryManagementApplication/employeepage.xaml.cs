using LibraryManagementApplication.Model.Class;
using LibraryManagementApplication.ViewModel;
using LibraryManagementApplication.ViewModel.ClassViewModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace LibraryManagementApplication
{
    /// <summary>
    /// Interaction logic for employeepage.xaml
    /// </summary>
    public partial class employeepage : Page
    {
        public employeepage()
        {
            InitializeComponent();
            loainv.ItemsSource = new List<string>() { "NV", "AD" };
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
                    p.Workbook.Worksheets.Add("Nhân Viên");
                    // lấy sheet vừa add ra để thao tác
                    ExcelWorksheet ws = p.Workbook.Worksheets["Nhân Viên"];
                    if (ws == null)
                    {
                        EXMessagebox.Show("Không thể thêm worksheet.");
                        return;
                    }
                    ws.Name = "Nhân Viên";
                    // fontsize mặc định cho cả sheet
                    ws.Cells.Style.Font.Size = 11;
                    // font family mặc định cho cả sheet
                    ws.Cells.Style.Font.Name = "Calibri";
                    // Tạo danh sách các column header
                    string[] arrColumnHeader = {
                                                "STT",
                                                "Mã nhân viên",
                                                "Tên nhân viên",
                                                "Mật khẩu",
                                                "Email",
                                                "CCCD",
                                                "SDT",
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
                    ws.Cells[3, 1].Value = "Thống kê thông tin Nhân Viên";
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
                    List<TaiKhoan> ExcelList = taikhoan.ItemsSource.Cast<TaiKhoan>().ToList();
                    if (taikhoan.ItemsSource == null)
                    {
                        EXMessagebox.Show("Không có dữ liệu để xuất!");
                        return;
                    }
                    // với mỗi item trong danh sách sẽ ghi trên 1 dòng
                    foreach (var item in ExcelList)
                    {
                        // bắt đầu ghi từ cột 1. Excel bắt đầu từ 1 không phải từ 0
                        colIndex = 1;

                        // rowIndex tương ứng từng dòng dữ liệu
                        rowIndex++;

                        //gán giá trị cho từng cell                      
                        ws.Cells[rowIndex, colIndex++].Value = rowIndex - 4;
                        ws.Cells[rowIndex, colIndex++].Value = item.UserID;
                        ws.Cells[rowIndex, colIndex++].Value = item.UserName;
                        ws.Cells[rowIndex, colIndex++].Value = item.Password;
                        ws.Cells[rowIndex, colIndex++].Value = item.Email;
                        ws.Cells[rowIndex, colIndex++].Value = item.CCCD;
                        ws.Cells[rowIndex, colIndex++].Value = item.SDT;
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
        private void taikhoan_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (taikhoan.SelectedItem != null)
            {
                // Hiển thị nút lịch sử khi có dòng được chọn
                history.Visibility = Visibility.Visible;
            }
            else
            {
                // Ẩn nút lịch sử nếu không có dòng nào được chọn
                history.Visibility = Visibility.Collapsed;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectedEmployee = (TaiKhoan)taikhoan.SelectedItem;

            if (selectedEmployee != null)
            {
                // Tạo cửa sổ chi tiết nhân viên và truyền dữ liệu
                var detailWindow = new employeeinfowindow
                {
                    DataContext = new TaiKhoanViewModel
                    {
                        SelectedTaiKhoan = selectedEmployee
                    }
                };

                // Đăng ký sự kiện Closed để cập nhật danh sách tài khoản sau khi cửa sổ bị đóng
                detailWindow.Closed += (s, args) =>
                {
                    // Làm mới danh sách tài khoản
                    LoadTaiKhoanList();

                    // Ẩn nút lịch sử khi cửa sổ bị đóng
                    history.Visibility = Visibility.Collapsed;
                };

                // Hiển thị cửa sổ chi tiết
                detailWindow.ShowDialog();
            }
        }
        private void LoadTaiKhoanList()
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    // Lấy danh sách tài khoản từ cơ sở dữ liệu
                    var taiKhoanList = context.TaiKhoans.ToList();
                    taikhoan.ItemsSource = taiKhoanList;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Có lỗi khi tải danh sách tài khoản: {ex.Message}");
            }
        }

        private void showsach_Click(object sender, RoutedEventArgs e)
        {
            history.Visibility = Visibility.Collapsed;
        }
    }
}
