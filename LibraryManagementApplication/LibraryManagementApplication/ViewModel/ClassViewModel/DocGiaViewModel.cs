using LibraryManagementApplication.Model.Class;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagementApplication.ViewModel.ClassViewModel
{
    public class DocGiaViewModel : BaseViewModel
    {
        private string maDG;
        private string tenDG;
        private string email;
        private string sdt;
        private string cccd;
        public string MaDG
        {
            get => maDG;
            set
            {
                if (maDG != value)
                {
                    maDG = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TenDG
        {
            get => tenDG;
            set
            {
                if (tenDG != value)
                {
                    tenDG = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Email
        {
            get => email;
            set
            {
                if (email != value)
                {
                    email = value;
                    OnPropertyChanged();
                }
            }
        }
        public string SDT
        {
            get => sdt;
            set
            {
                if (sdt != value)
                {
                    sdt = value;
                    OnPropertyChanged();
                }
            }
        }
        public string CCCD
        {
            get => cccd;
            set
            {
                if (cccd != value)
                {
                    cccd = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<DonMuon> DonMuonList { get; set; }
        public ObservableCollection<DocGia> DocGiaList { get; set; }
        private DocGia _selectedDocGia;
        public DocGia SelectedDocGia
        {
            get { return _selectedDocGia; }
            set
            {
                if (_selectedDocGia != value)
                {
                    _selectedDocGia = value;
                    OnPropertyChanged(nameof(SelectedDocGia));
                    if (SelectedDocGia != null)
                    {
                        TenDG = SelectedDocGia.TenDG;
                        Email = SelectedDocGia.Email;
                        SDT = SelectedDocGia.SDT;
                        CCCD = SelectedDocGia.CCCD;

                        using (var context = new LibraryDbContext())
                        {
                            DonMuonList = new ObservableCollection<DonMuon>(context.DonMuons.Where(t => t.MaDG == SelectedDocGia.MaDG));
                            OnPropertyChanged(nameof(DonMuonList));
                            if (DanhSachMuon == null)
                            {
                                DanhSachMuon = new ObservableCollection<BorrowedBook>();
                            }
                            DanhSachMuon.Clear();
                        }
                    }
                }
            }
        }
        private ObservableCollection<BorrowedBook> _danhSachMuon;
        public ObservableCollection<BorrowedBook> DanhSachMuon
        {
            get { return _danhSachMuon; }
            set
            {
                _danhSachMuon = value;
                OnPropertyChanged(nameof(DanhSachMuon));
            }
        }
        private DonMuon _selectedDonMuon;
        public DonMuon SelectedDonMuon
        {
            get { return _selectedDonMuon; }
            set
            {
                if (_selectedDonMuon != value)
                {
                    _selectedDonMuon = value;
                    OnPropertyChanged(nameof(SelectedDonMuon));
                    if (SelectedDonMuon != null)
                    {
                        using (var context = new LibraryDbContext())
                        {
                            if (DanhSachMuon == null)
                            {
                                DanhSachMuon = new ObservableCollection<BorrowedBook>();
                            }
                            DanhSachMuon.Clear();
                            foreach (var item in context.CTDMs)
                                if (item != null && item.MaMuon == SelectedDonMuon.MaMuon)
                                {
                                    Sach sach = context.Sachs.Where(s => s.MaDauSach == item.MaDauSach && s.ISBN == item.ISBN).FirstOrDefault();
                                    if (sach != null)
                                    {
                                        DanhSachMuon.Add(new BorrowedBook()
                                        {
                                            TenDauSach = sach.TenDauSach,
                                            ISBN = sach.ISBN
                                        });
                                    }
                                }
                        }                            
                    }
                }
            }
        }
        private async Task<string> CreateMaDGAsync()
        {
            // Generate a hash from TenDG
            using (var context = new LibraryDbContext())
            {
                // Lấy tất cả các mã "MaTL" hiện tại trong cơ sở dữ liệu có tiền tố "DG"
                var existingCodes = await context.DocGias
                                                 .Where(tl => tl.MaDG.StartsWith("DG"))
                                                 .Select(tl => tl.MaDG)
                                                 .ToListAsync();

                var minUnusedNumber = existingCodes
                     .Select(code => int.TryParse(code.Substring(2), out int num) ? num : 0) // Lấy phần số sau "DG"
                     .OrderBy(number => number) // Sắp xếp tăng dần
                     .ToList(); // Lấy số lớn nhất trong danh sách mã

                // Tạo mã mới với số tăng dần
                int newCodeNumber = 1;
                foreach (var number in minUnusedNumber)
                {
                    if (number != newCodeNumber)
                    {
                        break; // Nếu phát hiện khoảng trống
                    }
                    newCodeNumber++;
                }

                // Trả về mã mới với định dạng "DG" + số có 3 chữ số
                return $"DG{newCodeNumber:D3}";
            }
        }
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ShowCommand { get; set; }

        public DocGiaViewModel()
        {
            DocGiaList = new ObservableCollection<DocGia>();
            AddCommand = new RelayCommand<object>((p) => true, async (p) => await AddDocGia());
            EditCommand = new RelayCommand<object>((p) => SelectedDocGia != null, async (p) => await EditDocGia());
            DeleteCommand = new RelayCommand<object>((p) => SelectedDocGia != null, async (p) => await DeleteDocGia());
            SearchCommand = new RelayCommand<string>((p) => true, async (p) => await SearchDocGia());
            ShowCommand = new RelayCommand<object>((p) => true, (p) => ShowTheLoai());

            LoadDocGiaList();
        }

        private async void LoadDocGiaList()
        {
            MaDG = TenDG = SDT = Email = CCCD = "";
            DocGiaList.Clear();
            var docGias = await GetAllDocGiasAsync();
            foreach (var docGia in docGias)
            {
                DocGiaList.Add(docGia);
            }
        }

        private async Task AddDocGia()
        {
            if (!string.IsNullOrEmpty(TenDG) &&
                !string.IsNullOrEmpty(Email) &&
                !string.IsNullOrEmpty(SDT) &&
                !string.IsNullOrEmpty(CCCD))
            {
                if (!IsValidEmail(Email))
                {
                    EXMessagebox.Show("Email không hợp lệ.", "Thông báo");
                    return;
                }

                if (!IsCitizenIDValid(CCCD))
                {
                    EXMessagebox.Show("CCCD không hợp lệ.", "Thông báo");
                    return;
                }

                if (!IsPhoneNumberValid(SDT))
                {
                    EXMessagebox.Show("SDT không hợp lệ.", "Thông báo");
                    return;
                }
                bool exists = await IsDocGiaExistsAsync(TenDG, CCCD);
                if (exists)
                {
                    EXMessagebox.Show("Thông tin tên độc giả và căn cước công dân tương ứng đã tồn tại.");
                    return;
                }

                var newDocGia = new DocGia()
                {
                    MaDG = await CreateMaDGAsync(),
                    TenDG = TenDG,
                    Email = Email,
                    SDT = SDT,
                    CCCD = CCCD
                };

                bool isSuccess = await AddDocGiaToDatabaseAsync(newDocGia);
                if (!isSuccess)
                    EXMessagebox.Show("Cannot save changes.");
                else
                    DocGiaList.Add(newDocGia);
                MaDG = TenDG = SDT = Email = CCCD = "";
            }
            else
                EXMessagebox.Show("Vui lòng nhập đầy đủ thông tin của độc giả", "Cảnh báo");
        }
        private async Task EditDocGia()
        {
            if (SelectedDocGia != null)
            {
                if (!IsValidEmail(Email))
                {
                    EXMessagebox.Show("Email không hợp lệ.", "Thông báo");
                    return;
                }

                if (!IsCitizenIDValid(CCCD))
                {
                    EXMessagebox.Show("CCCD không hợp lệ.", "Thông báo");
                    return;
                }

                if (!IsPhoneNumberValid(SDT))
                {
                    EXMessagebox.Show("SDT không hợp lệ.", "Thông báo");
                    return;
                }

                SelectedDocGia.TenDG = TenDG;
                SelectedDocGia.Email = Email;
                SelectedDocGia.SDT = SDT;
                SelectedDocGia.CCCD = CCCD;

                bool isSuccess = await UpdateDocGiaInDatabaseAsync(SelectedDocGia);
                if (!isSuccess)
                    EXMessagebox.Show("Error updating the record.");
                else
                    EXMessagebox.Show("Sửa đổi thông tin độc giả thành công", "Thông báo");
            }
        }
        private async Task DeleteDocGia()
        {
            if (SelectedDocGia != null)
            {
                bool isSuccess = await DeleteDocGiaFromDatabaseAsync(SelectedDocGia.MaDG);
                if (isSuccess)
                    EXMessagebox.Show("Xóa thông tin độc giả thành công");
            }
        }
        private async Task SearchDocGia()
        {
            // Gọi hàm tìm kiếm trong cơ sở dữ liệu với các tham số tìm kiếm
            var filteredList = await SearchDocGiaInDatabaseAsync(TenDG, SDT, CCCD, Email);
            DocGiaList.Clear();
            foreach (var docGia in filteredList)
            {
                DocGiaList.Add(docGia);
            }
        }
        private void ShowTheLoai()
        {
            LoadDocGiaList();
        }

        #region MethodToDatabase
        private static async Task<bool> IsDocGiaExistsAsync(string tenDG, string cccd)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    // Chuyển cả hai về chữ thường trước khi so sánh
                    string normalizedTenTL = tenDG.ToLower();
                    return await context.DocGias
                                        .AnyAsync(dg => dg.TenDG.ToLower() == normalizedTenTL && dg.CCCD == cccd);
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error checking existence of DocGia: {ex.Message}");
                return false;
            }
        }
        public static async Task<bool> AddDocGiaToDatabaseAsync(DocGia docGia)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    context.DocGias.Add(docGia);
                    await context.SaveChangesAsync();
                    EXMessagebox.Show("Đang thêm vào cơ sở dữ liệu.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error adding DocGia: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> UpdateDocGiaInDatabaseAsync(DocGia docGia)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    context.DocGias.Update(docGia);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error updating DocGia: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> DeleteDocGiaFromDatabaseAsync(string maDG)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    var docGiaToDelete = await context.DocGias.Include(d => d.DonMuons)
                                                               .FirstOrDefaultAsync(dg => dg.MaDG == maDG);
                    if (docGiaToDelete != null)
                    {
                        if (docGiaToDelete.DonMuons.Count > 0)
                        {
                            EXMessagebox.Show("Không thể xóa độc giả vì có đơn mượn liên kết", "Lỗi");
                            return false;
                        }

                        context.DocGias.Remove(docGiaToDelete);
                        await context.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error deleting DocGia: {ex.Message}");
                return false;
            }
        }
        public static async Task<List<DocGia>> SearchDocGiaInDatabaseAsync(string tenDG, string sdt, string cccd, string email)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    // Tạo query ban đầu
                    var query = context.DocGias.AsQueryable();

                    // Lọc theo các thuộc tính nếu chúng không rỗng hoặc null
                    if (!string.IsNullOrEmpty(tenDG))
                    {
                        query = query.Where(dg => dg.TenDG.Contains(tenDG));
                    }

                    if (!string.IsNullOrEmpty(sdt))
                    {
                        query = query.Where(dg => dg.SDT.Contains(sdt));
                    }

                    if (!string.IsNullOrEmpty(cccd))
                    {
                        query = query.Where(dg => dg.CCCD.Contains(cccd));
                    }

                    if (!string.IsNullOrEmpty(email))
                    {
                        query = query.Where(dg => dg.Email.Contains(email));
                    }

                    // Thực hiện truy vấn và trả về kết quả
                    return await query.ToListAsync();
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error searching DocGia: {ex.Message}");
                return new List<DocGia>();
            }
        }

        public static async Task<List<DocGia>> GetAllDocGiasAsync()
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    var result = await context.DocGias.ToListAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error getting all DocGia: {ex.Message}");
                return new List<DocGia>();
            }
        }

        #endregion
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Biểu thức chính quy kiểm tra email
                string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                return Regex.IsMatch(email, emailPattern);
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        private bool IsPhoneNumberValid(string phoneNumber)
        {
            var phoneRegex = new Regex(@"^\d{10,11}$");
            return phoneRegex.IsMatch(phoneNumber);
        }
        private bool IsCitizenIDValid(string citizenID)
        {
            var citizenIDRegex = new Regex(@"^\d{12}$");
            return citizenIDRegex.IsMatch(citizenID);
        }
    }
}