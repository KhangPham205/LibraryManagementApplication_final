using Microsoft.Win32;
using LibraryManagementApplication.Model.Class;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using OfficeOpenXml.Table.PivotTable;
using System.Diagnostics.Eventing.Reader;

namespace LibraryManagementApplication.ViewModel.ClassViewModel
{
    public class TaiKhoanViewModel : BaseViewModel
    {
        private string userName;
        private string password;
        private string loaiNV;
        private string email;
        private string sdt;
        private string cccd;
        private BitmapImage _profileImage;
        public string UserName
        {
            get => userName;
            set
            {
                if (userName != value)
                {
                    userName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Password
        {
            get => password;
            set
            {
                if (password != value)
                {
                    password = value;
                    OnPropertyChanged();
                }
            }
        }
        public string LoaiNV
        {
            get => loaiNV;
            set
            {
                if (loaiNV != value)
                {
                    loaiNV = value;
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
        public BitmapImage ProfileImage
        {
            get { return _profileImage; }
            set
            {
                if (_profileImage != value)
                {
                    _profileImage = value;
                    OnPropertyChanged(nameof(ProfileImage));
                }
            }
        }

        // Mật khẩu mới
        private string newPassWord;
        public string NewPassWord
        {
            get => newPassWord;
            set
            {
                if (newPassWord != value)
                {
                    newPassWord = value; 
                    OnPropertyChanged();
                }
            }
        }
        private string newPassWord2;
        public string NewPassWord2
        {
            get => newPassWord2;
            set
            {
                if (newPassWord2 != value)
                {
                    newPassWord2 = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsAdded;
        public ObservableCollection<TaiKhoan> TaiKhoanList { get; set; }
        private TaiKhoan _selectedTaiKhoan;
        public ObservableCollection<DonMuon> DonMuonList { get; set; }
        public TaiKhoan SelectedTaiKhoan
        {
            get { return _selectedTaiKhoan; }
            set
            {
                if (_selectedTaiKhoan != value)
                {
                    _selectedTaiKhoan = value;
                    OnPropertyChanged(nameof(SelectedTaiKhoan));
                    if (SelectedTaiKhoan != null)
                    {
                        UserName = SelectedTaiKhoan.UserName;
                        Password = SelectedTaiKhoan.Password;
                        LoaiNV = SelectedTaiKhoan.Loai;
                        Email = SelectedTaiKhoan.Email;
                        SDT = SelectedTaiKhoan.SDT;
                        CCCD = SelectedTaiKhoan.CCCD;

                        using (var context = new LibraryDbContext())
                        {
                            DonMuonList = new ObservableCollection<DonMuon>(context.DonMuons.Where(t => t.MaNV == SelectedTaiKhoan.UserID));
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
        private async Task<string> CreateMaTKAsync()
        {
            using (var context = new LibraryDbContext())
            {
                // Lấy tất cả các mã UserID hiện tại trong cơ sở dữ liệu có tiền tố LoaiNV
                var existingCodes = await context.TaiKhoans
                                                 .Where(tl => tl.UserID.StartsWith(LoaiNV))
                                                 .Select(tl => tl.UserID)
                                                 .ToListAsync();

                // Lọc và chuyển đổi mã sang số, bỏ qua các giá trị không hợp lệ hoặc bằng 0
                var minUnusedNumber = existingCodes
                    .Select(code => int.TryParse(code.Substring(LoaiNV.Length), out int num) ? num : -1) // Lấy phần số sau tiền tố
                    .Where(num => num > 0) // Chỉ lấy các số lớn hơn 0
                    .OrderBy(number => number) // Sắp xếp tăng dần
                    .ToList();

                // Tìm số mới nhỏ nhất chưa được sử dụng
                int newCodeNumber = 1;
                foreach (var number in minUnusedNumber)
                {
                    if (number != newCodeNumber)
                    {
                        break; // Nếu phát hiện khoảng trống
                    }
                    newCodeNumber++;
                }

                // Trả về mã mới với định dạng "LoaiNV" + số có 3 chữ số
                return $"{LoaiNV}{newCodeNumber:D3}";
            }
        }
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ShowCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }

        public TaiKhoanViewModel()
        {
            TaiKhoanList = new ObservableCollection<TaiKhoan>();
            AddCommand = new RelayCommand<object>((p) => true, async (p) => await AddTaiKhoan());
            EditCommand = new RelayCommand<object>((p) => SelectedTaiKhoan != null, async (p) => await EditTaiKhoan());
            DeleteCommand = new RelayCommand<object>((p) => SelectedTaiKhoan != null, async (p) => await DeleteTaiKhoan());
            SearchCommand = new RelayCommand<string>((p) => true, async (p) => await SearchTaiKhoan());
            ShowCommand = new RelayCommand<DataGrid>((p) => true, (p) => ShowTaiKhoan());
            ChangePasswordCommand = new RelayCommand<DataGrid>((p) => true, async (p) => await ChangePassWord());

            if (GlobalData.LoginUser != null && GlobalData.LoginUser.ProfileImage != null)
            {
                ProfileImage = ConvertByteArrayToBitmapImage(GlobalData.LoginUser.ProfileImage);
            }

            LoadTaiKhoanList();
        }

        private async void LoadTaiKhoanList()
        {
            UserName = Password = Email = SDT = CCCD = LoaiNV = "";
            TaiKhoanList.Clear();
            var taiKhoans = await GetAllTaiKhoansAsync();
            foreach (var taiKhoan in taiKhoans)
            {
                TaiKhoanList.Add(taiKhoan);
            }
        }

        private async Task<bool> AddTaiKhoan()
        {
            if (!string.IsNullOrEmpty(UserName) && 
                !string.IsNullOrEmpty(Password) && 
                !string.IsNullOrEmpty(Email) && 
                !string.IsNullOrEmpty(SDT) &&
                !string.IsNullOrEmpty(LoaiNV) &&
                !string.IsNullOrEmpty(CCCD))
            {
                if (!IsValidEmail(Email))
                {
                    EXMessagebox.Show("Email không hợp lệ.", "Thông báo");
                    return false;
                }
                
                if (!IsCitizenIDValid(CCCD))
                {
                    EXMessagebox.Show("CCCD không hợp lệ.", "Thông báo");
                    return false;
                }

                if (!IsPhoneNumberValid(SDT))
                {
                    EXMessagebox.Show("SDT không hợp lệ.", "Thông báo");
                    return false;
                }

                bool exists = await IsTaiKhoanExistsAsync(CCCD, Email);
                if (exists)
                {
                    EXMessagebox.Show("Nhân viên với email hoặc căn cước công dân đã được sử dụng.", "Thông báo");
                    return false;
                }

                TaiKhoan newTaiKhoan = new TaiKhoan()
                {
                    UserID = await CreateMaTKAsync(),
                    UserName = UserName,
                    Password = Password,
                    Loai = LoaiNV,
                    Email = Email,
                    SDT = SDT,
                    CCCD = CCCD
                };

                bool isSuccess = await AddTaiKhoanToDatabaseAsync(newTaiKhoan);
                IsAdded = isSuccess;
                if (isSuccess)
                    TaiKhoanList.Add(newTaiKhoan);
                else
                    EXMessagebox.Show("Error adding Tai Khoan.");
                UserName = Password = Email = SDT = CCCD = LoaiNV = "";
                return isSuccess;
            }
            else
            {
                EXMessagebox.Show("Vui lòng nhập thông tin", "Cảnh báo");
                return false;
            }    
        }

        private async Task EditTaiKhoan()
        {
            if (SelectedTaiKhoan != null)
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

                SelectedTaiKhoan.UserName = UserName;
                SelectedTaiKhoan.Password = Password;
                SelectedTaiKhoan.SDT = SDT;
                SelectedTaiKhoan.Email = Email;
                SelectedTaiKhoan.CCCD = CCCD;
                bool isSuccess = await UpdateTaiKhoanInDatabaseAsync(SelectedTaiKhoan);
                if (!isSuccess)
                {
                    EXMessagebox.Show("Error updating Tai Khoan.");
                }
                else
                    EXMessagebox.Show("Sửa thông tin tài khoản thành công", "Thông báo");
            }
        }

        private async Task DeleteTaiKhoan()
        {
            if (SelectedTaiKhoan != null)
            {
                bool isSuccess = await DeleteTaiKhoanFromDatabaseAsync(SelectedTaiKhoan.UserID);
                if (isSuccess)
                    EXMessagebox.Show("Xóa tài khoản thành công", "Thông báo");
            }
        }

        private async Task SearchTaiKhoan()
        {
            // Gọi hàm tìm kiếm trong cơ sở dữ liệu với các tham số tìm kiếm
            var filteredList = await SearchTaiKhoanInDatabaseAsync(UserName, Password, LoaiNV, Email, SDT, CCCD);
            TaiKhoanList.Clear();
            foreach (var taiKhoan in filteredList)
            {
                TaiKhoanList.Add(taiKhoan);
            }
        }
        private void ShowTaiKhoan()
        {           
            LoadTaiKhoanList();
        }
        private async Task ChangePassWord()
        {
            if (!string.IsNullOrEmpty(Password) &&
                !string.IsNullOrEmpty(NewPassWord) &&
                !string.IsNullOrEmpty(NewPassWord2) &&
                NewPassWord.Equals(NewPassWord2))
            {
                using (var context = new LibraryDbContext())
                {
                    TaiKhoan taiKhoan = context.TaiKhoans.Where(tk => tk.SDT == SDT && tk.Email == Email && tk.CCCD == CCCD && tk.Password == Password).FirstOrDefault();
                    if (taiKhoan != null)
                    {
                        taiKhoan.Password = NewPassWord;
                        bool isSuccess = await UpdateTaiKhoanInDatabaseAsync(taiKhoan);
                        if (!isSuccess)
                            EXMessagebox.Show("Error updating Tai Khoan.");
                        else
                            EXMessagebox.Show("Cập nhật mật khẩu thành công", "Thông báo");
                    }
                    else
                        EXMessagebox.Show("Mật khẩu cũ sai hoặc thông tin không trùng khớp", "Thông báo");
                }
            }
            else
                EXMessagebox.Show("Vui lòng điền lại mật khẩu", "Thông báo");
        }
        #region MethodToDatabase
        private static async Task<bool> IsTaiKhoanExistsAsync(string cccd, string email)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    // Kiểm tra xem có tồn tại tài khoản với CCCD hoặc email đã sử dụng hay không
                    return await context.TaiKhoans
                        .AnyAsync(tk => tk.CCCD == cccd ||
                                        tk.Email.ToLower() == email.ToLower());
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error checking existence of Tai Khoan: {ex.Message}");
                return false;
            }
        }
        public static async Task<bool> AddTaiKhoanToDatabaseAsync(TaiKhoan taiKhoan)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    context.TaiKhoans.Add(taiKhoan);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error adding Tai khoan: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> UpdateTaiKhoanInDatabaseAsync(TaiKhoan taiKhoan)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    context.TaiKhoans.Update(taiKhoan);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error updating Tai khoan: {ex.Message}");
                return false;
            }
        }
        private static async Task<bool> DeleteTaiKhoanFromDatabaseAsync(string userID)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    // Kiểm tra nếu tài khoản có liên kết với các đơn mượn
                    bool hasRelatedDonMuon = await context.DonMuons.AnyAsync(dm => dm.MaNV == userID);
                    if (hasRelatedDonMuon)
                    {
                        EXMessagebox.Show("Không thể xóa tài khoản vì có liên kết với đơn mượn.");
                        return false;
                    }

                    // Nếu không có liên kết, tiến hành xóa tài khoản
                    var taiKhoanToDelete = await context.TaiKhoans.FirstOrDefaultAsync(tk => tk.UserID == userID);
                    if (taiKhoanToDelete != null)
                    {
                        context.TaiKhoans.Remove(taiKhoanToDelete);
                        await context.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error deleting Tai khoan: {ex.Message}");
                return false;
            }
        }
        public static async Task<List<TaiKhoan>> SearchTaiKhoanInDatabaseAsync(string userName, string password, string loaiNV, string email, string sdt, string cccd)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    // Tạo query ban đầu
                    var query = context.TaiKhoans.AsQueryable();

                    // Lọc theo các thuộc tính nếu chúng không rỗng hoặc null
                    if (!string.IsNullOrEmpty(userName))
                    {
                        query = query.Where(tk => tk.UserName.Contains(userName));
                    }

                    if (!string.IsNullOrEmpty(password))
                    {
                        query = query.Where(tk => tk.Password.Contains(password));
                    }

                    if (!string.IsNullOrEmpty(loaiNV))
                    {
                        query = query.Where(tk => tk.Loai.Contains(loaiNV));
                    }

                    if (!string.IsNullOrEmpty(email))
                    {
                        query = query.Where(tk => tk.Email.Contains(email));
                    }

                    if (!string.IsNullOrEmpty(sdt))
                    {
                        query = query.Where(tk => tk.SDT.Contains(sdt));
                    }

                    if (!string.IsNullOrEmpty(cccd))
                    {
                        query = query.Where(tk => tk.CCCD.Contains(cccd));
                    }

                    // Thực hiện truy vấn và trả về kết quả
                    return await query.ToListAsync();
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error searching Tai khoan: {ex.Message}");
                return new List<TaiKhoan>();
            }
        }

        public static async Task<List<TaiKhoan>> GetAllTaiKhoansAsync()
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    var result = await context.TaiKhoans.ToListAsync();
                    return result.Where(tk => tk != null).ToList(); // Loại bỏ NULL nếu cần
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error getting all Tai khoan: {ex.Message}");
                return new List<TaiKhoan>();
            }
        }

        #endregion

        private byte[] ConvertBitmapImageToByteArray(BitmapImage bitmapImage)
        {
            using (var memoryStream = new MemoryStream())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(memoryStream);
                return memoryStream.ToArray();
            }
        }

        // Convert byte[] to BitmapImage
        private BitmapImage ConvertByteArrayToBitmapImage(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0) return null;

            using (var memoryStream = new MemoryStream(byteArray))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
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