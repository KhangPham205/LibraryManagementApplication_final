using LibraryManagementApplication.Model.Class;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagementApplication.ViewModel.ClassViewModel
{
    public class NhaXuatBanViewModel : BaseViewModel
    {
        public string MaNXB { get; set; }
        public string TenNXB { get; set; }
        public ObservableCollection<NhaXuatBan> NhaXuatBanList { get; set; }
        private NhaXuatBan _selectedNhaXuatBan;
        public NhaXuatBan SelectedNhaXuatBan
        {
            get { return _selectedNhaXuatBan; }
            set
            {
                if (_selectedNhaXuatBan != value)
                {
                    _selectedNhaXuatBan = value;
                    OnPropertyChanged(nameof(SelectedNhaXuatBan));
                    if (SelectedNhaXuatBan != null)
                        DisplayName = SelectedNhaXuatBan.TenNXB;
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
        private async Task<string> CreateMaNXBAsync()
        {
            // Generate a hash from TenNXB
            using (var context = new LibraryDbContext())
            {
                // Lấy tất cả các mã "MaNXB" hiện tại trong cơ sở dữ liệu có tiền tố "NXB"
                var existingCodes = await context.NhaXuatBans
                                                 .Where(tl => tl.MaNXB.StartsWith("NXB"))
                                                 .Select(tl => tl.MaNXB)
                                                 .ToListAsync();

                var minUnusedNumber = existingCodes
                    .Select(code => int.TryParse(code.Substring(3), out int num) ? num : 0) // Lấy phần số sau "NXB"
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

                // Trả về mã mới với định dạng "NXB" + số có 3 chữ số
                return $"NXB{newCodeNumber:D3}";
            }
        }
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SearchCommand { get; set; }

        public NhaXuatBanViewModel()
        {
            NhaXuatBanList = new ObservableCollection<NhaXuatBan>();
            AddCommand = new RelayCommand<object>((p) => true, async (p) => await AddNhaXuatBan());
            EditCommand = new RelayCommand<object>((p) => SelectedNhaXuatBan != null, async (p) => await EditNhaXuatBan());
            DeleteCommand = new RelayCommand<object>((p) => SelectedNhaXuatBan != null, async (p) => await DeleteNhaXuatBan());
            SearchCommand = new RelayCommand<string>((p) => true, async (p) => await SearchNhaXuatBan(p));

            LoadNhaXuatBanList();
        }

        private async void LoadNhaXuatBanList()
        {
            DisplayName = "";
            NhaXuatBanList.Clear();
            var nhaXuatBans = await GetAllNhaXuatBanAsync();
            foreach (var nxb in nhaXuatBans)
            {
                NhaXuatBanList.Add(nxb);
            }
        }
        private async Task AddNhaXuatBan()
        {
            if (!string.IsNullOrEmpty(DisplayName))
            {
                bool exists = await IsNhaXuatBanExistsAsync(DisplayName);
                if (exists)
                {
                    EXMessagebox.Show("Nhà xuất bản với tên này đã tồn tại.");
                    return;
                }

                NhaXuatBan newNhaXuatBan = new NhaXuatBan()
                {
                    MaNXB = await CreateMaNXBAsync(),
                    TenNXB = DisplayName,
                };

                bool isSuccess = await AddNhaXuatBanToDatabaseAsync(newNhaXuatBan);
                if (!isSuccess)
                    EXMessagebox.Show("Cannot save changes.");
                else
                    NhaXuatBanList.Add(newNhaXuatBan);
                DisplayName = "";
            }
            else
                EXMessagebox.Show("Vui lòng nhập tên nhà xuất bản", "Cảnh báo");
        }
        private async Task EditNhaXuatBan()
        {
            if (SelectedNhaXuatBan != null)
            {
                SelectedNhaXuatBan.TenNXB = DisplayName;
                bool isSuccess = await UpdateNhaXuatBanInDatabaseAsync(SelectedNhaXuatBan);
                if (!isSuccess)
                {
                    EXMessagebox.Show("Error updating Nha xuat ban.");
                }
                LoadNhaXuatBanList();
            }
        }

        private async Task DeleteNhaXuatBan()
        {
            if (SelectedNhaXuatBan != null)
            {
                bool isSuccess = await DeleteNhaXuatBanFromDatabaseAsync(SelectedNhaXuatBan.MaNXB);
                if (isSuccess)
                {
                    NhaXuatBanList.Remove(SelectedNhaXuatBan);
                    DisplayName = "";
                }
            }
        }

        private async Task SearchNhaXuatBan(string keyword)
        {
            var filteredList = await SearchNhaXuatBanInDatabaseAsync(keyword);
            NhaXuatBanList.Clear();
            foreach (var donMuon in filteredList)
            {
                NhaXuatBanList.Add(donMuon);
            }
        }

        #region MethodToDatabase
        private static async Task<bool> IsNhaXuatBanExistsAsync(string tenNXB)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    // Chuyển cả hai về chữ thường trước khi so sánh
                    string normalizedTenNXB = tenNXB.ToLower();
                    return await context.NhaXuatBans
                                        .AnyAsync(nxb => nxb.TenNXB.ToLower() == normalizedTenNXB);
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error checking existence of NhaXuatBan: {ex.Message}");
                return false;
            }
        }
        private static async Task<bool> AddNhaXuatBanToDatabaseAsync(NhaXuatBan nhaXuatBan)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    context.NhaXuatBans.Add(nhaXuatBan);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (DbUpdateException dbEx)
            {
                EXMessagebox.Show($"Database Update Error: {dbEx.Message}\nInner Exception: {dbEx.InnerException?.Message}");
            }
            catch (InvalidOperationException ex)
            {
                EXMessagebox.Show($"Invalid Operation: {ex.Message}");
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Unexpected Error: {ex.Message}");
            }
            return false;
        }

        private static async Task<bool> UpdateNhaXuatBanInDatabaseAsync(NhaXuatBan nhaXuatBan)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    context.NhaXuatBans.Update(nhaXuatBan);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error updating Nha xuat ban: {ex.Message}");
                return false;
            }
        }

        private static async Task<bool> DeleteNhaXuatBanFromDatabaseAsync(string maNXB)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    bool hasRelatedDauSach = await context.DauSachs.AnyAsync(ds => ds.MaNXB == maNXB);
                    if (hasRelatedDauSach)
                    {
                        EXMessagebox.Show("Không thể xóa nhà xuất bản vì có đầu sách liên kết.");
                        return false;
                    }

                    var nhaXuatBanToDelete = await context.NhaXuatBans.FirstOrDefaultAsync(nxb => nxb.MaNXB == maNXB);
                    if (nhaXuatBanToDelete != null)
                    {
                        context.NhaXuatBans.Remove(nhaXuatBanToDelete);
                        await context.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error deleting NhaXuatBan: {ex.Message}");
                return false;
            }
        }

        private static async Task<List<NhaXuatBan>> SearchNhaXuatBanInDatabaseAsync(string keyword)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    var result = await context.NhaXuatBans
                                              .Where(nxb => nxb.MaNXB.Contains(keyword))
                                              .ToListAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error searching DonMuon: {ex.Message}");
                return new List<NhaXuatBan>();
            }
        }

        private static async Task<List<NhaXuatBan>> GetAllNhaXuatBanAsync()
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    var result = await context.NhaXuatBans.ToListAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error getting all DonMuon: {ex.Message}");
                return new List<NhaXuatBan>();
            }
        }

        #endregion
    }
}