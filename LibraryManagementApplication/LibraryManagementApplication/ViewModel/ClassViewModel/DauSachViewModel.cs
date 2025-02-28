using LibraryManagementApplication.Model.Class;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LibraryManagementApplication.ViewModel.ClassViewModel
{
    public class DauSachViewModel : BaseViewModel
    {
        private LibraryDbContext dbContext;
        private string maDauSach;
        private string tenDauSach;
        private string tenTG;
        private string ngonNgu;
        private string tenTL;
        private string tenNXB;
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
        public string TenTG
        {
            get => tenTG;
            set
            {
                if (tenTG != value)
                {
                    tenTG = value;
                    OnPropertyChanged();
                }
            }
        }
        public string NgonNgu
        {
            get => ngonNgu;
            set
            {
                if (ngonNgu != value)
                {
                    ngonNgu = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TenTL
        {
            get => tenTL;
            set
            {
                if (tenTL != value)
                {
                    tenTL = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TenNXB
        {
            get => tenNXB;
            set
            {
                if (tenNXB != value)
                {
                    tenNXB = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<ThongTinDauSach> DauSachList { get; set; }
        private DauSach _selectedDauSach;
        public DauSach SelectedDauSach
        {
            get { return _selectedDauSach; }
            set 
            {
                if (_selectedDauSach != value)
                {
                    _selectedDauSach = value;
                    OnPropertyChanged(nameof(SelectedDauSach));
                    if (SelectedDauSach != null)
                    {
                        MaDauSach = SelectedDauSach.MaDauSach;
                        TenDauSach = SelectedDauSach.TenDauSach;
                        TenTG = dbContext.TacGias.Where(t => t.MaTG == SelectedDauSach.MaTG).Select(t => t.TenTG).FirstOrDefault();
                        NgonNgu = SelectedDauSach.NgonNgu;
                        TenTL = dbContext.TheLoais.Where(t => t.MaTL == SelectedDauSach.MaTL).Select(t => t.TenTL).FirstOrDefault();
                        TenNXB = dbContext.NhaXuatBans.Where(t => t.MaNXB == SelectedDauSach.MaNXB).Select(t => t.TenNXB).FirstOrDefault();
                    }
                }
            }
        }
        private ThongTinDauSach _selectedRow;
        public ThongTinDauSach SelectedRow
        {
            get { return _selectedRow; }
            set
            {
                if (_selectedRow != value)
                {
                    _selectedRow = value;
                    OnPropertyChanged(nameof(SelectedRow));
                    if (SelectedRow != null)
                    {
                        MaDauSach = SelectedRow.MaDauSach;
                        TenDauSach = SelectedRow.TenDauSach;
                        TenTG = SelectedRow.TenTG;
                        NgonNgu = SelectedRow.NgonNgu;
                        TenTL = SelectedRow.TenTL;
                        TenNXB = SelectedRow.TenNXB;
                    }
                }
            }
        }
        private async Task<string> CreateMaDSAsync(string TenDauSach)
        {
            // Generate a hash from TenTL
            using (var context = new LibraryDbContext())
            {
                // Lấy tất cả các mã "MaTL" hiện tại trong cơ sở dữ liệu có tiền tố "TL"
                var existingCodes = await context.DauSachs
                                                 .Where(tl => tl.MaDauSach.StartsWith("DS"))
                                                 .Select(tl => tl.MaDauSach)
                                                 .ToListAsync();

                var minUnusedNumber = existingCodes
                    .Select(code => int.TryParse(code.Substring(2), out int num) ? num : 0) // Lấy phần số sau "DS"
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

                // Trả về mã mới với định dạng "DS" + số có 3 chữ số
                return $"DS{newCodeNumber:D3}";
            }
        }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ShowCommand { get; set; }
        public DauSachViewModel()
        {
            dbContext = new LibraryDbContext();

            DauSachList = new ObservableCollection<ThongTinDauSach>();
            AddCommand = new RelayCommand<object>((p) => true, async (p) => await AddDauSach());
            EditCommand = new RelayCommand<object>((p) => SelectedRow != null, async (p) => await EditDauSach());
            DeleteCommand = new RelayCommand<object>((p) => SelectedRow != null, async (p) => await DeleteDauSach());
            SearchCommand = new RelayCommand<string>((p) => true, async (p) => await SearchDauSach());
            ShowCommand = new RelayCommand<object>((p) => true, (p) => ShowDauSach());

            LoadDauSachList();
        }

        private async void LoadDauSachList()
        {
            MaDauSach = TenDauSach = TenTG = TenTL = TenNXB = NgonNgu = "";
            DauSachList.Clear();
            var DauSachs = await GetAllDauSachsAsync();
            ThongTinDauSach thongTinDauSach;
            foreach (var DauSach in DauSachs)
            {
                DauSachList.Add(thongTinDauSach = new ThongTinDauSach()
                {
                    MaDauSach = DauSach.MaDauSach,
                    TenDauSach = DauSach.TenDauSach,
                    TenTG = dbContext.TacGias.FirstOrDefault(t => t.MaTG == DauSach.MaTG).TenTG,
                    NgonNgu = DauSach.NgonNgu,
                    TenTL = dbContext.TheLoais.FirstOrDefault(t => t.MaTL == DauSach.MaTL).TenTL,
                    TenNXB = dbContext.NhaXuatBans.FirstOrDefault(t => t.MaNXB == DauSach.MaNXB).TenNXB,
                });
            }
        }

        private async Task AddDauSach()
        {
            if (!string.IsNullOrEmpty(TenDauSach) && 
                !string.IsNullOrEmpty(TenTG) &&
                !string.IsNullOrEmpty(NgonNgu) &&
                !string.IsNullOrEmpty(TenTL) &&
                !string.IsNullOrEmpty(TenNXB))
            {
                bool exists = await IsDauSachExistsAsync(TenDauSach);
                if (exists)
                {
                    EXMessagebox.Show("Đầu sách này đã tồn tại.");
                    return;
                }

                TacGia tacGia = dbContext.TacGias.FirstOrDefault(t => t.TenTG == TenTG); 
                if (tacGia == null)
                {
                    tacGia = new TacGia()
                    {
                        MaTG = await CreateMaTGAsync(),
                        TenTG = TenTG 
                    }; 
                    // Thêm tacGia mới vào dbContext
                    dbContext.TacGias.Add(tacGia); 
                    await dbContext.SaveChangesAsync(); 
                }

                TheLoai theLoai = dbContext.TheLoais.FirstOrDefault(t => t.TenTL == TenTL);
                if (theLoai == null)
                {
                    theLoai = new TheLoai()
                    {
                        MaTL = await CreateMaTLAsync(),
                        TenTL = TenTL
                    };
                    dbContext.TheLoais.Add(theLoai);
                    await dbContext.SaveChangesAsync();
                }

                NhaXuatBan nhaXuatBan = dbContext.NhaXuatBans.FirstOrDefault(t => t.TenNXB == TenNXB);
                if (nhaXuatBan == null)
                {
                    nhaXuatBan = new NhaXuatBan()
                    {
                        MaNXB = await CreateMaNXBAsync(),
                        TenNXB = TenNXB
                    };
                    dbContext.Add(nhaXuatBan);
                    await dbContext.SaveChangesAsync();
                }

                var newDauSach = new DauSach()
                {
                    MaDauSach = MaDauSach = await CreateMaDSAsync(TenDauSach),
                    TenDauSach = TenDauSach,
                    MaTG = tacGia.MaTG,
                    NgonNgu = NgonNgu,
                    MaTL = theLoai.MaTL,
                    MaNXB = nhaXuatBan.MaNXB,
                };

                ThongTinDauSach thongTinDauSach;
                //EXMessagebox.Show(newDauSach.MaDauSach + " " + newDauSach.TenDauSach + " " + newDauSach.TenTL);
                bool isSuccess = await AddDauSachToDatabaseAsync(newDauSach);
                if (isSuccess)
                {
                    DauSachList.Add(thongTinDauSach = new ThongTinDauSach()
                    {
                        MaDauSach = newDauSach.MaDauSach,
                        TenDauSach = newDauSach.TenDauSach,
                        TenTG = dbContext.TacGias.FirstOrDefault(t => t.MaTG == newDauSach.MaTG).TenTG,
                        NgonNgu = NgonNgu,
                        TenTL = dbContext.TheLoais.FirstOrDefault(t => t.MaTL == newDauSach.MaTL).TenTL,
                        TenNXB = dbContext.NhaXuatBans.FirstOrDefault(t => t.MaNXB == newDauSach.MaNXB).TenNXB,
                    });
                    MaDauSach = TenDauSach = TenTG = TenTL = TenNXB = NgonNgu = "";
                }
                else
                    EXMessagebox.Show("Không thể thêm đầu sách mới");
            }
            else
                EXMessagebox.Show("Vui lòng nhập đầy đủ thông tin", "Cảnh báo");
        }

        private async Task EditDauSach()
        {
            if (SelectedRow != null)
            {
                var ds = await dbContext.DauSachs.FirstOrDefaultAsync(x => x.MaDauSach == SelectedRow.MaDauSach);
                if (ds != null)
                {
                    TacGia tacGia = dbContext.TacGias.FirstOrDefault(t => t.TenTG == TenTG);
                    if (tacGia == null)
                    {
                        tacGia = new TacGia()
                        {
                            MaTG = await CreateMaTGAsync(),
                            TenTG = TenTG
                        };
                        // Thêm tacGia mới vào dbContext
                        dbContext.TacGias.Add(tacGia);
                        await dbContext.SaveChangesAsync();
                    }

                    TheLoai theLoai = dbContext.TheLoais.FirstOrDefault(t => t.TenTL == TenTL);
                    if (theLoai == null)
                    {
                        theLoai = new TheLoai()
                        {
                            MaTL = await CreateMaTLAsync(),
                            TenTL = TenTL
                        };
                        dbContext.TheLoais.Add(theLoai);
                        await dbContext.SaveChangesAsync();
                    }

                    NhaXuatBan nhaXuatBan = dbContext.NhaXuatBans.FirstOrDefault(t => t.TenNXB == TenNXB);
                    if (nhaXuatBan == null)
                    {
                        nhaXuatBan = new NhaXuatBan()
                        {
                            MaNXB = await CreateMaNXBAsync(),
                            TenNXB = TenNXB
                        };
                        dbContext.Add(nhaXuatBan);
                        await dbContext.SaveChangesAsync();
                    }

                    ds.TenDauSach = TenDauSach;
                    ds.MaTG = tacGia.MaTG;
                    ds.NgonNgu = NgonNgu;
                    ds.MaTL = theLoai.MaTL;
                    ds.MaNXB = nhaXuatBan.MaNXB;

                    bool isSuccess = await UpdateDauSachInDatabaseAsync(ds);
                    if (!isSuccess)
                    {
                        EXMessagebox.Show("Cannot edit DauSach");
                    }
                    // Reload list sau khi cập nhật
                    LoadDauSachList();
                }
            }
        }

        private async Task DeleteDauSach()
        {
            if (SelectedRow != null)
            {
                var ds = await dbContext.DauSachs.FirstOrDefaultAsync(t => t.MaDauSach == SelectedRow.MaDauSach);
                if (ds != null)
                {
                    bool isSuccess = await DeleteDauSachFromDatabaseAsync(ds);
                    if (isSuccess)
                    {
                        if (!DauSachList.Remove(SelectedRow))
                            EXMessagebox.Show("Không xóa được phần tử trong danh sách đầu sách");
                        else
                            MaDauSach = TenDauSach = TenTG = TenTL = TenNXB = NgonNgu = "";
                    }
                }                
            }
        }

        private async Task SearchDauSach()
        {
            // Lấy mã tác giả, thể loại, NXB tương ứng nếu có
            var maTG = !string.IsNullOrEmpty(TenTG)
                        ? dbContext.TacGias.Where(t => t.TenTG == TenTG).Select(t => t.MaTG).FirstOrDefault()
                        : null;

            var maTL = !string.IsNullOrEmpty(TenTL)
                        ? dbContext.TheLoais.Where(t => t.TenTL == TenTL).Select(t => t.MaTL).FirstOrDefault()
                        : null;

            var maNXB = !string.IsNullOrEmpty(TenNXB)
                        ? dbContext.NhaXuatBans.Where(t => t.TenNXB == TenNXB).Select(t => t.MaNXB).FirstOrDefault()
                        : null;

            // Tìm kiếm trong CSDL
            var filteredListFromDb = await SearchDauSachInDatabaseAsync(TenDauSach, maTG, NgonNgu, maTL, maNXB);

            // Để tránh nhiều truy vấn lặp lại trong vòng lặp, load toàn bộ dữ liệu liên quan sẵn
            var allTacGias = dbContext.TacGias.ToDictionary(t => t.MaTG, t => t.TenTG);
            var allTheLoais = dbContext.TheLoais.ToDictionary(t => t.MaTL, t => t.TenTL);
            var allNhaXuatBans = dbContext.NhaXuatBans.ToDictionary(t => t.MaNXB, t => t.TenNXB);

            // Xóa danh sách cũ
            DauSachList.Clear();
            foreach (var ds in filteredListFromDb)
            {
                // Lấy tên tương ứng từ dictionary
                var tg = allTacGias.ContainsKey(ds.MaTG) ? allTacGias[ds.MaTG] : null;
                var tl = allTheLoais.ContainsKey(ds.MaTL) ? allTheLoais[ds.MaTL] : null;
                var nxb = allNhaXuatBans.ContainsKey(ds.MaNXB) ? allNhaXuatBans[ds.MaNXB] : null;

                DauSachList.Add(new ThongTinDauSach()
                {
                    MaDauSach = ds.MaDauSach,
                    TenDauSach = ds.TenDauSach,
                    TenTG = tg,
                    NgonNgu = ds.NgonNgu,
                    TenTL = tl,
                    TenNXB = nxb
                });
            }
        }
        private void ShowDauSach()
        {
            LoadDauSachList();
        }
        #region MethodToDatabase
        private static async Task<bool> IsDauSachExistsAsync(string tenDauSach)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    // Chuyển cả hai về chữ thường trước khi so sánh
                    string normalizedTenDS = tenDauSach.ToLower();
                    return await context.DauSachs
                                        .AnyAsync(ds => ds.TenDauSach.ToLower() == normalizedTenDS);
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error checking existence of DauSach: {ex.Message}");
                return false;
            }
        }
        private static async Task<bool> AddDauSachToDatabaseAsync(DauSach DauSach)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    context.DauSachs.Add(DauSach);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (DbUpdateException ex)
            {
                EXMessagebox.Show($"Error adding header book: {ex.InnerException?.Message ?? ex.Message}");
                return false;
            }
            catch (Exception ex) { EXMessagebox.Show(ex.Message); return false; }
        }
        private static async Task<bool> UpdateDauSachInDatabaseAsync(DauSach DauSach)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    context.DauSachs.Update(DauSach);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error updating header book: {ex.Message}");
                return false;
            }
        }
        private static async Task<bool> DeleteDauSachFromDatabaseAsync(DauSach dauSach)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    // Kiểm tra nếu có sách nào tham chiếu đến đầu sách này
                    bool hasRelatedSach = await context.Sachs.AnyAsync(s => s.MaDauSach == dauSach.MaDauSach);
                    if (hasRelatedSach)
                    {
                        EXMessagebox.Show("Không thể xóa đầu sách vì có sách liên kết.");
                        return false;
                    }

                    // Nếu không có sách liên kết, tiến hành xóa đầu sách
                    var DauSachToDelete = await context.DauSachs.FirstOrDefaultAsync(s => s.MaDauSach == dauSach.MaDauSach);
                    if (DauSachToDelete != null)
                    {
                        context.DauSachs.Remove(DauSachToDelete);
                        await context.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error deleting header book: {ex.Message}");
                return false;
            }
        }
        private static async Task<List<DauSach>> SearchDauSachInDatabaseAsync(string tenDauSach, string maTG, string ngonNgu, string maTL, string maNXB)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    var query = context.DauSachs.AsQueryable();

                    // Áp dụng các điều kiện tìm kiếm, chỉ áp dụng nếu các tham số có giá trị.
                    if (!string.IsNullOrEmpty(tenDauSach))
                        query = query.Where(s => s.TenDauSach.Contains(tenDauSach));

                    if (!string.IsNullOrEmpty(maTG))
                        query = query.Where(s => s.MaTG.Contains(maTG));

                    if (!string.IsNullOrEmpty(ngonNgu))
                        query = query.Where(s => s.NgonNgu.Contains(ngonNgu));

                    if (!string.IsNullOrEmpty(maTL))
                        query = query.Where(s => s.MaTL.Contains(maTL));

                    if (!string.IsNullOrEmpty(maNXB))
                        query = query.Where(s => s.MaNXB.Contains(maNXB));

                    var result = await query.ToListAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error searching header books: {ex.Message}");
                return new List<DauSach>();
            }
        }
        private static async Task<List<DauSach>> GetAllDauSachsAsync()
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    var result = await context.DauSachs.ToListAsync();
                    return result;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error getting all dau sach: {ex.Message}");
                return new List<DauSach>();
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
        #endregion
    }

    public partial class ThongTinDauSach
    {
        public string MaDauSach {  get; set; }
        public string TenDauSach { get; set; }
        public string TenTG {  get; set; }
        public string NgonNgu {  get; set; }
        public string TenTL { get; set; }
        public string TenNXB { get; set; }
    }
}
