﻿using LibraryManagementApplication.Model.Class;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using LibraryManagementApplication.ViewModel.ClassViewModel;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Windows.Forms;
using System.IO;

namespace LibraryManagementApplication
{
    /// <summary>
    /// Interaction logic for bookpage.xaml
    /// </summary>
    public partial class headerbookpage : Page
    {
        public headerbookpage()
        {
            InitializeComponent();

            this.Loaded += (s, e) =>
            {
                LoadTheLoai();
            };
        }
        private void LoadTheLoai()
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    theloaitb.ItemsSource = context.TheLoais.Select(t => t.TenTL).ToList();
                    nxbtb.ItemsSource = context.NhaXuatBans.Select(nxb => nxb.TenNXB).ToList();
                    tacgiatb.ItemsSource = context.TacGias.Select(tg => tg.TenTG).ToList();
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Lỗi khi tải dữ liệu: {ex.Message}");
            }
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
                    p.Workbook.Worksheets.Add("Đầu Sách");
                    // lấy sheet vừa add ra để thao tác
                    ExcelWorksheet ws = p.Workbook.Worksheets["Đầu Sách"];
                    if (ws == null)
                    {
                        EXMessagebox.Show("Không thể thêm worksheet.");
                        return;
                    }
                    ws.Name = "Đầu Sách";
                    // fontsize mặc định cho cả sheet
                    ws.Cells.Style.Font.Size = 11;
                    // font family mặc định cho cả sheet
                    ws.Cells.Style.Font.Name = "Calibri";
                    // Tạo danh sách các column header
                    string[] arrColumnHeader = {
                                                "STT",
                                                "Mã đầu sách",
                                                "Tên đầu sách",
                                                "Tác giả",
                                                "Thể loại",
                                                "Nhà xuất bản",
                                                "Ngôn ngữ",
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
                    ws.Cells[3, 1].Value = "Thống kê thông tin Đầu Sách";
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
                    List<ThongTinDauSach> ExcelList = sach.ItemsSource.Cast<ThongTinDauSach>().ToList();
                    if (sach.ItemsSource == null)
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
                        ws.Cells[rowIndex, colIndex++].Value = item.MaDauSach;
                        ws.Cells[rowIndex, colIndex++].Value = item.TenDauSach;
                        ws.Cells[rowIndex, colIndex++].Value = item.TenTG;
                        ws.Cells[rowIndex, colIndex++].Value = item.TenTL;
                        ws.Cells[rowIndex, colIndex++].Value = item.TenNXB;
                        ws.Cells[rowIndex, colIndex++].Value = item.NgonNgu;
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
    
}