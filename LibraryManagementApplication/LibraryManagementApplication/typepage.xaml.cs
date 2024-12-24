using LibraryManagementApplication.Model.Class;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace LibraryManagementApplication
{
    /// <summary>
    /// Interaction logic for typepage.xaml
    /// </summary>
    public partial class typepage : Page
    {
        public typepage()
        {
            InitializeComponent();
           
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
                    p.Workbook.Worksheets.Add("Thể loại");
                    // lấy sheet vừa add ra để thao tác
                    ExcelWorksheet ws = p.Workbook.Worksheets["Thể loại"];
                    if (ws == null)
                    {
                        EXMessagebox.Show("Không thể thêm worksheet.");
                        return;
                    }
                    ws.Name = "Thể loại";
                    // fontsize mặc định cho cả sheet
                    ws.Cells.Style.Font.Size = 11;
                    // font family mặc định cho cả sheet
                    ws.Cells.Style.Font.Name = "Calibri";
                    // Tạo danh sách các column header
                    string[] arrColumnHeader = {
                                                "STT",
                                                "Mã thể loại",
                                                "Tên thể loại",
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
                    ws.Cells[3, 1].Value = "Thống kê thông tin Thể Loại";
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
                    List<TheLoai> ExcelList = theloai.ItemsSource.Cast<TheLoai>().ToList();
                    if (theloai.ItemsSource == null)
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
                        ws.Cells[rowIndex, colIndex++].Value = item.MaTL;
                        ws.Cells[rowIndex, colIndex++].Value = item.TenTL;

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

        private void exporttl_Click(object sender, RoutedEventArgs e)
        {
            ExportFile();
        }
    }
}
