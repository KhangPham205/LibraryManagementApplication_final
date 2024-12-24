using LibraryManagementApplication.Model.Class;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LibraryManagementApplication.ViewModel.ClassViewModel;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Controls;

namespace LibraryManagementApplication.ViewModel.ClassViewModel
{
    public class SachViewModel : BaseViewModel
    {
        private string maDauSach;
        private string tenDauSach;
        private string isbn;
        private string viTri;
        private string trangThai;
        private string namXB;
        public string MaDauSach 
        { 
            get => maDauSach; 
            set
            {
                if (maDauSach != value)
                {
                    maDauSach = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TenDauSach
        {
            get => tenDauSach;
            set
            {
                if (tenDauSach != value)
                {
                    tenDauSach = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ISBN
        {
            get => isbn;
            set
            {
                if (isbn != value)
                {
                    isbn = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ViTri
        {
            get => viTri;
            set
            {
                if (viTri != value)
                {
                    viTri = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TrangThai
        {
            get => trangThai;
            set
            {
                if (trangThai != value)
                {
                    trangThai = value;
                    OnPropertyChanged();
                }
            }
        }
        public string NamXB
        {
            get => namXB;
            set
            {
                if (namXB != value)
                {
                    namXB = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<Sach> SachList { get; set; }
        private Sach _selectedSach;
        public Sach SelectedSach
        {
            get { return _selectedSach; }
            set
            {
                if (_selectedSach != value)
                {
                    _selectedSach = value;
                    OnPropertyChanged(nameof(SelectedSach));
                    if (SelectedSach != null)
                    {
                        MaDauSach = SelectedSach.MaDauSach;
                        TenDauSach = SelectedSach.TenDauSach;
                        ISBN = SelectedSach.ISBN;
                        ViTri = SelectedSach.ViTri;
                        TrangThai = SelectedSach.TrangThai;
                        NamXB = SelectedSach.NamXB.ToString();
                    }
                }
            }
        }
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ShowCommand { get; set; }
        public SachViewModel()
        {
            SachList = new ObservableCollection<Sach>();
            AddCommand = new RelayCommand<object>((p) => true, async (p) => await AddSach());
            EditCommand = new RelayCommand<object>((p) => SelectedSach != null, async (p) => await EditSach());
            DeleteCommand = new RelayCommand<object>((p) => SelectedSach != null, async (p) => await DeleteSach());
            SearchCommand = new RelayCommand<object>((p) => true, async (p) => await SearchSach());
            ShowCommand = new RelayCommand<DataGrid>((p) => true, (p) => ShowDauSach(p));

            LoadSachList();
        }
        private async void LoadSachList()
        {
            MaDauSach = TenDauSach = ISBN = ViTri = NamXB = TrangThai = "";
            SachList.Clear();
            var sachs = await GetAllSachsAsync();
            foreach (var sach in sachs)
            {
                SachList.Add(sach);
            }
        }
        private async Task AddSach()
        {
            try
            {
                // Kiểm tra ISBN định dạng custom: 3-3-3-4
                if (!IsValidCustomISBN(ISBN))
                {
                    EXMessagebox.Show("ISBN không đúng định dạng.");
                    return;
                }

                // Kiểm tra NamXB phải là số và nhỏ hơn hoặc bằng năm hiện tại
                if (!int.TryParse(NamXB, out int namXB) || namXB > DateTime.Now.Year)
                {
                    EXMessagebox.Show("Năm xuất bản không hợp lệ.");
                    return;
                }

                using (var context = new LibraryDbContext())
                {
                    DauSach ds = context.DauSachs.FirstOrDefault(s => s.TenDauSach == TenDauSach);
                    if (ds == null)
                    {
                        EXMessagebox.Show("Không tìm thấy đầu sách.");
                        return;
                    }

                    var newSach = new Sach()
                    {
                        MaDauSach = ds.MaDauSach,
                        TenDauSach = TenDauSach,
                        ISBN = ISBN,
                        ViTri = ViTri,
                        TrangThai = TrangThai,
                        NamXB = namXB
                    };

                    bool isSuccess = await AddSachToDatabaseAsync(newSach);
                    if (!isSuccess)
                        EXMessagebox.Show("Không thể lưu sách mới.");
                    else
                    {
                        MaDauSach = TenDauSach = ISBN = ViTri = NamXB = TrangThai = "";
                        SachList.Add(newSach);
                    }
                }
            }
            catch
            {
                EXMessagebox.Show("Thông tin không hợp lệ!");
            }
        }

        private async Task EditSach()
        {
            try
            {
                // Kiểm tra ISBN định dạng custom: 3-3-3-4
                if (!IsValidCustomISBN(ISBN))
                {
                    EXMessagebox.Show("ISBN không đúng định dạng 3-3-3-4.");
                    return;
                }

                // Kiểm tra NamXB phải là số và nhỏ hơn hoặc bằng năm hiện tại
                if (!int.TryParse(NamXB, out int namXB) || namXB > DateTime.Now.Year)
                {
                    EXMessagebox.Show("Năm xuất bản không hợp lệ.");
                    return;
                }

                if (SelectedSach != null)
                {
                    SelectedSach.TenDauSach = TenDauSach;
                    SelectedSach.ISBN = ISBN;
                    SelectedSach.ViTri = ViTri;
                    SelectedSach.TrangThai = TrangThai;
                    SelectedSach.NamXB = namXB;
                    bool isSuccess = await UpdateSachInDatabaseAsync(SelectedSach);
                    if (!isSuccess)
                    {
                        EXMessagebox.Show("Không thể sửa thông tin sách.");
                    }
                    LoadSachList();
                }
            }
            catch
            {
                EXMessagebox.Show("Thông tin không hợp lệ!");
            }
        }
        private async Task DeleteSach()
        {
            if (SelectedSach != null)
            {
                bool isSuccess = await DeleteSachFromDatabaseAsync(SelectedSach);
                if (isSuccess)
                {
                    MaDauSach = TenDauSach = ISBN = ViTri = NamXB = TrangThai = "";
                    SachList.Remove(SelectedSach);
                }    
                else
                    EXMessagebox.Show("Không thể xóa sách");
            }
        }
        private async Task SearchSach()
        {
            int parsedNamXB;
            if (!int.TryParse(NamXB, out parsedNamXB))
            {
                parsedNamXB = 0; // Default to 0 or another fallback value
            }

            var filteredListFromDb = await SearchSachInDatabaseAsync(TenDauSach, ISBN, ViTri, TrangThai, parsedNamXB);
            SachList.Clear();
            foreach (var sach in filteredListFromDb)
            {
                SachList.Add(sach);
            }
        }
        private void ShowDauSach(DataGrid p)
        {
            LoadSachList();
        }
        #region MethodToDatabase
        private static async Task<bool> AddSachToDatabaseAsync(Sach sach)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    context.Sachs.Add(sach);
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
        private static async Task<bool> UpdateSachInDatabaseAsync(Sach sach)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    context.Sachs.Update(sach);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error updating book: {ex.Message}");
                return false;
            }
        }
        private static async Task<bool> DeleteSachFromDatabaseAsync(Sach sach)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    var sachToDelete = await context.Sachs.FirstOrDefaultAsync(s => s == sach);
                    if (sachToDelete != null)
                    {
                        context.Sachs.Remove(sachToDelete);
                        await context.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error deleting book: {ex.Message}");
                return false;
            }
        }
        private static async Task<List<Sach>> SearchSachInDatabaseAsync(string tenDauSach, string isbn, string viTri, string trangThai, int namXB)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    // Build the query based on the search parameters
                    var query = context.Sachs.AsQueryable();

                    if (!string.IsNullOrEmpty(tenDauSach))
                        query = query.Where(s => s.TenDauSach.Contains(tenDauSach));

                    if (!string.IsNullOrEmpty(isbn))
                        query = query.Where(s => s.ISBN.Contains(isbn));

                    if (!string.IsNullOrEmpty(viTri))
                        query = query.Where(s => s.ViTri.Contains(viTri));

                    if (!string.IsNullOrEmpty(trangThai))
                        query = query.Where(s => s.TrangThai.Contains(trangThai));

                    if (namXB > 0)
                        query = query.Where(s => s.NamXB == namXB);

                    var result = await query.ToListAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error searching books: {ex.Message}");
                return new List<Sach>();
            }
        }
        private static async Task<List<Sach>> GetAllSachsAsync()
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    var result = await context.Sachs.ToListAsync();
                    return result;
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
                EXMessagebox.Show($"Error getting all books: {ex.Message}");
            }
            return new List<Sach>();
        }
        #endregion
        private bool IsValidCustomISBN(string isbn)
        {
            if (string.IsNullOrEmpty(isbn))
                return false;

            var parts = isbn.Split('-');
            if (parts.Length != 4)
                return false;

            return parts[0].Length == 3 && parts[1].Length == 3 && parts[2].Length == 3 && parts[3].Length == 4 &&
                   parts.All(part => part.All(char.IsDigit));
        }
    }
}