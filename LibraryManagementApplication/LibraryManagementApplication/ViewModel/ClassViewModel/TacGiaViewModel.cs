using LibraryManagementApplication.Model.Class;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LibraryManagementApplication.ViewModel.ClassViewModel
{
    public class TacGiaViewModel : BaseViewModel
    {
        public string MaTG { get; set; }
        public string TenTG { get; set; }
        public ObservableCollection<TacGia> TacGiaList { get; set; }
        private TacGia _selectedTacGia;
        public TacGia SelectedTacGia
        {
            get { return _selectedTacGia; }
            set
            {
                if (_selectedTacGia != value)
                {
                    _selectedTacGia = value;
                    OnPropertyChanged(nameof(SelectedTacGia));
                    if (SelectedTacGia != null)
                        DisplayName = SelectedTacGia.TenTG;
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
        private async Task<string> CreateMaTGAsync()
        {
            // Generate a hash from TenTL
            using (var context = new LibraryDbContext())
            {
                // Lấy tất cả các mã "MaTG" hiện tại trong cơ sở dữ liệu có tiền tố "TG"
                var existingCodes = await context.TacGias
                                                 .Where(tl => tl.MaTG.StartsWith("TG"))
                                                 .Select(tl => tl.MaTG)
                                                 .ToListAsync();

                var minUnusedNumber = existingCodes
                    .Select(code => int.TryParse(code.Substring(2), out int num) ? num : 0) // Lấy phần số sau "TG"
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
                // Trả về mã mới với định dạng "TG" + số có 3 chữ số
                return $"TG{newCodeNumber:D3}";
            }
        }
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ShowCommand { get; set; }
        public TacGiaViewModel()
        {
            TacGiaList = new ObservableCollection<TacGia>();
            AddCommand = new RelayCommand<object>((p) => true, async (p) => await AddTacGia());
            EditCommand = new RelayCommand<object>((p) => SelectedTacGia != null, async (p) => await EditTacGia());
            DeleteCommand = new RelayCommand<object>((p) => SelectedTacGia != null, async (p) => await DeleteTacGia());
            SearchCommand = new RelayCommand<string>((p) => true, async (p) => await SearchTacGia());
            ShowCommand = new RelayCommand<object>((p) => true, (p) => ShowTacGia());

            LoadTacGiaList();
        }

        private async void LoadTacGiaList()
        {
            DisplayName = "";
            TacGiaList.Clear();
            var tacGias = await GetAllTacGiasAsync();
            foreach (var tacGia in tacGias)
            {
                TacGiaList.Add(tacGia);
            }
        }

        private async Task AddTacGia()
        {
            if (!string.IsNullOrEmpty(DisplayName))
            {
                bool exists = await IsTacGiaExistsAsync(DisplayName);
                if (exists)
                {
                    EXMessagebox.Show("Tác giả đã tồn tại.");
                    return;
                }

                TacGia newTacGia = new TacGia()
                {
                    MaTG = await CreateMaTGAsync(),
                    TenTG = DisplayName
                };

                bool isSuccess = await AddTacGiaToDatabaseAsync(newTacGia);
                if (!isSuccess)
                    EXMessagebox.Show("Cannot save changes.");
                else
                {
                    TacGiaList.Add(newTacGia);
                    DisplayName = "";
                }
            }
        }
        private async Task EditTacGia()
        {
            if (SelectedTacGia != null)
            {
                SelectedTacGia.TenTG = DisplayName;
                bool isSuccess = await UpdateTacGiaInDatabaseAsync(SelectedTacGia);
                if (!isSuccess)
                {
                    EXMessagebox.Show("Error updating the record.");
                }
                LoadTacGiaList();
            }
        }
        private async Task DeleteTacGia()
        {
            if (SelectedTacGia != null)
            {
                bool isSuccess = await DeleteTacGiaFromDatabaseAsync(SelectedTacGia.MaTG);
                if (isSuccess)
                {
                    TacGiaList.Remove(SelectedTacGia);
                    DisplayName = "";
                }
            }
        }
        private async Task SearchTacGia()
        {
            var filteredList = await SearchTacGiaInDatabaseAsync(DisplayName);
            TacGiaList.Clear();
            foreach (var tacGia in filteredList)
            {
                TacGiaList.Add(tacGia);
            }
        }

        private void ShowTacGia()
        {
            LoadTacGiaList();
        }
        #region MethodToDatabase
        private static async Task<bool> IsTacGiaExistsAsync(string tenTG)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    // Chuyển cả hai về chữ thường trước khi so sánh
                    string normalizedTenTG = tenTG.ToLower();
                    return await context.TacGias
                                        .AnyAsync(tg => tg.TenTG.ToLower() == normalizedTenTG);
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error checking existence of TcaGia: {ex.Message}");
                return false;
            }
        }
        private static async Task<bool> AddTacGiaToDatabaseAsync(TacGia tacGia)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    context.TacGias.Add(tacGia);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error adding Tac gia: {ex.Message}");
                return false;
            }
        }

        private static async Task<bool> UpdateTacGiaInDatabaseAsync(TacGia tacGia)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    context.TacGias.Update(tacGia);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error updating Tac gia: {ex.Message}");
                return false;
            }
        }
        private static async Task<bool> DeleteTacGiaFromDatabaseAsync(string maTG)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    bool hasRelatedDauSach = await context.DauSachs.AnyAsync(ds => ds.MaTG == maTG);
                    if (hasRelatedDauSach)
                    {
                        EXMessagebox.Show("Không thể xóa tác giả vì có đầu sách liên kết.");
                        return false;
                    }

                    var tacGiaToDelete = await context.TacGias.FirstOrDefaultAsync(tg => tg.MaTG == maTG);
                    if (tacGiaToDelete != null)
                    {
                        context.TacGias.Remove(tacGiaToDelete);
                        await context.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Lỗi xóa tác giả: {ex.Message}");
                return false;
            }
        }
        private static async Task<List<TacGia>> SearchTacGiaInDatabaseAsync(string keyword)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    var result = await context.TacGias
                                              .Where(s => s.TenTG.Contains(keyword))
                                              .ToListAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error searching Tac gia: {ex.Message}");
                return new List<TacGia>();
            }
        }
        private static async Task<List<TacGia>> GetAllTacGiasAsync()
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    var result = await context.TacGias.ToListAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error getting all Tac gia: {ex.Message}");
                return new List<TacGia>();
            }
        }

        #endregion
    }
}