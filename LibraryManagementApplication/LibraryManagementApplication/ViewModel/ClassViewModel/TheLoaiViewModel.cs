using LibraryManagementApplication.Model.Class;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LibraryManagementApplication.ViewModel.ClassViewModel
{
    public class TheLoaiViewModel : BaseViewModel
    {
        public string MaTL { get; set; }
        public string TenTL { get; set; }
        public ObservableCollection<TheLoai> TheLoaiList { get; set; }
        private TheLoai _selectedTheLoai;

        public TheLoai SelectedTheLoai
        {
            get => _selectedTheLoai;
            set
            {
                if (_selectedTheLoai != value)
                {
                    _selectedTheLoai = value;
                    OnPropertyChanged();
                    if (SelectedTheLoai != null)
                        DisplayName = SelectedTheLoai.TenTL;
                }
            }
        }
        private String _displayName;
        public String DisplayName
        {
            get => _displayName;
            set
            {
                if (_displayName != value)
                {
                    _displayName = value;
                    OnPropertyChanged();
                }
            }
        }

        private async Task<string> CreateMaTLAsync()
        {
            // Generate a hash from TenTL
            using (var context = new LibraryDbContext())
            {
                // Lấy tất cả các mã "MaTL" hiện tại trong cơ sở dữ liệu có tiền tố "TL"
                var existingCodes = await context.TheLoais
                                                 .Where(tl => tl.MaTL.StartsWith("TL"))
                                                 .Select(tl => tl.MaTL)
                                                 .ToListAsync();

                var minUnusedNumber = existingCodes
                    .Select(code => int.TryParse(code.Substring(2), out int num) ? num : 0) // Lấy phần số sau "TL"
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
                // Trả về mã mới với định dạng "TL" + số có 3 chữ số
                return $"TL{newCodeNumber:D3}";
            }
        }
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ShowCommand { get; set; }

        public TheLoaiViewModel()
        {
            TheLoaiList = new ObservableCollection<TheLoai>();
            AddCommand = new RelayCommand<object>((p) => true, async (p) => await AddTheLoai());
            EditCommand = new RelayCommand<object>((p) => SelectedTheLoai != null, async (p) => await EditTheLoai());
            DeleteCommand = new RelayCommand<object>((p) => SelectedTheLoai != null, async (p) => await DeleteTheLoai());
            SearchCommand = new RelayCommand<object>((p) => true, async (p) => await SearchTheLoai());
            ShowCommand = new RelayCommand<object>((p) => true, (p) => ShowTheLoai());
            LoadTheLoaiList();
        }

        private async void LoadTheLoaiList()
        {
            DisplayName = "";
            TheLoaiList.Clear();
            var theLoais = await GetAllTheLoaisAsync();
            foreach (var theLoai in theLoais)
            {
                TheLoaiList.Add(theLoai);
            }
        }

        private async Task AddTheLoai()
        {
            if (!string.IsNullOrEmpty(DisplayName))
            {
                bool exists = await IsTheLoaiExistsAsync(DisplayName);
                if (exists)
                {
                    EXMessagebox.Show("Thể loại này đã tồn tại.");
                    return;
                }

                TheLoai newTheLoai = new TheLoai()
                {
                    MaTL = await CreateMaTLAsync(), // Generate unique MaTL
                    TenTL = DisplayName
                };

                bool isSuccess = await AddTheLoaiToDatabaseAsync(newTheLoai);
                if (isSuccess)
                    TheLoaiList.Add(newTheLoai);
                else
                    EXMessagebox.Show("Error adding The Loai.");
                DisplayName = "";
            }
            else
                EXMessagebox.Show("Vui lòng nhập tên thể loại", "Cảnh báo");
        }
        private async Task EditTheLoai()
        {
            if (SelectedTheLoai != null)
            {
                SelectedTheLoai.TenTL = DisplayName;
                bool isSuccess = await UpdateTheLoaiInDatabaseAsync(SelectedTheLoai);
                if (!isSuccess)
                {
                    EXMessagebox.Show("Error updating The Loai.");
                }
                LoadTheLoaiList();
            }
        }

        private async Task DeleteTheLoai()
        {
            if (SelectedTheLoai != null)
            {
                bool isSuccess = await DeleteTheLoaiFromDatabaseAsync(SelectedTheLoai);
                if (isSuccess)
                {
                    TheLoaiList.Remove(SelectedTheLoai);
                    DisplayName = "";
                }
            }
        }
        private async Task SearchTheLoai()
        {
            var filteredListFrormDB = await SearchTheLoaiInDatabaseAsync(DisplayName);
            TheLoaiList.Clear();
            foreach (var theloai in filteredListFrormDB)
            {
                TheLoaiList.Add(theloai);
            }
        }
        private void ShowTheLoai()
        {
            LoadTheLoaiList();
        }

        #region MethodToDatabase
        private static async Task<bool> IsTheLoaiExistsAsync(string tenTL)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    // Chuyển cả hai về chữ thường trước khi so sánh
                    string normalizedTenTL = tenTL.ToLower();
                    return await context.TheLoais
                                        .AnyAsync(tl => tl.TenTL.ToLower() == normalizedTenTL);
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error checking existence of TheLoai: {ex.Message}");
                return false;
            }
        }
        private static async Task<bool> AddTheLoaiToDatabaseAsync(TheLoai theLoai)
        {
            //kiểm tra bị trùng
            try
            {
                using (var context = new LibraryDbContext())
                {
                    context.TheLoais.Add(theLoai);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error adding The loai: {ex.Message}");
                return false;
            }
        }
        private static async Task<bool> UpdateTheLoaiInDatabaseAsync(TheLoai theLoai)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    context.TheLoais.Update(theLoai);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error updating The loai: {ex.Message}");
                return false;
            }
        }
        private static async Task<bool> DeleteTheLoaiFromDatabaseAsync(TheLoai theLoai)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    // Kiểm tra nếu có đầu sách nào tham chiếu đến thể loại
                    bool hasRelatedDauSach = await context.DauSachs.AnyAsync(ds => ds.MaTL == theLoai.MaTL);
                    if (hasRelatedDauSach)
                    {
                        EXMessagebox.Show("Không thể xóa thể loại vì có đầu sách liên kết.");
                        return false;
                    }

                    // Nếu không có đầu sách liên kết, tiến hành xóa thể loại
                    var theLoaiToDelete = await context.TheLoais.FirstOrDefaultAsync(tl => tl.MaTL == theLoai.MaTL);
                    if (theLoaiToDelete != null)
                    {
                        context.TheLoais.Remove(theLoaiToDelete);
                        await context.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error deleting TheLoai: {ex.Message}");
                return false;
            }
        }
        private static async Task<List<TheLoai>> SearchTheLoaiInDatabaseAsync(string keyword)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    var result = await context.TheLoais
                                              .Where(s => s.MaTL.Contains(keyword) || s.TenTL.Contains(keyword)) // Allows search by either MaTL or TenTL
                                              .ToListAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error searching The loai: {ex.Message}");
                return new List<TheLoai>();
            }
        }
        private static async Task<List<TheLoai>> GetAllTheLoaisAsync()
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    var result = await context.TheLoais.ToListAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error getting all The loai: {ex.Message}");
                return new List<TheLoai>();
            }
        }
        #endregion
    }
}