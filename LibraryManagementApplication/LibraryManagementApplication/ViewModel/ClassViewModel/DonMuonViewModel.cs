using LibraryManagementApplication.Model.Class;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LibraryManagementApplication.ViewModel.ClassViewModel
{
    public class DonMuonViewModel : BaseViewModel
    {
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

        private string maMuon;
        private string maDG;
        private string maNV;
        private string ngayMuon;
        private string ngayTraDK;
        private string ngayTraTT;
        private string phiPhat;
        public string MaMuon 
        { 
            get => maMuon; 
            set
            {
                if (maMuon != value)
                {
                    maMuon = value;
                    OnPropertyChanged(nameof(MaMuon));
                }
            }
        }
        public string MaDG
        {
            get => maDG;
            set
            {
                if (maDG != value)
                {
                    maDG = value;
                    OnPropertyChanged(nameof(MaDG));
                }
            }
        }
        public string MaNV
        {
            get => maNV;
            set
            {
                if (maNV != value)
                {
                    maNV = value;
                    OnPropertyChanged(nameof(MaNV));
                }
            }
        }
        public string NgayMuon
        {
            get => ngayMuon;
            set
            {
                if (ngayMuon != value)
                {
                    ngayMuon = value;
                    OnPropertyChanged(nameof(NgayMuon));
                }
            }
        }
        public string NgayTraDK
        {
            get => ngayTraDK;
            set
            {
                if (ngayTraDK != value)
                {
                    ngayTraDK = value;
                    OnPropertyChanged(nameof(NgayTraDK));
                }
            }
        }
        public string NgayTraTT
        {
            get => ngayTraTT;
            set
            {
                if (ngayTraTT != value)
                {
                    ngayTraTT = value;
                    OnPropertyChanged(nameof(NgayTraTT));
                }
            }
        }
        public string PhiPhat
        {
            get => phiPhat;
            set
            {
                if (phiPhat != value)
                {
                    phiPhat = value;
                    OnPropertyChanged(nameof(PhiPhat));
                }
            }
        }

        // Ngày mượn
        private DateTime? startBorrowDate;
        public DateTime? StartBorrowDate
        {
            get => startBorrowDate;
            set
            {
                if (startBorrowDate != value)
                {
                    startBorrowDate = value;
                    OnPropertyChanged(nameof(StartBorrowDate));
                }
            }
        }

        private DateTime? endBorrowDate;
        public DateTime? EndBorrowDate
        {
            get => endBorrowDate;
            set
            {
                if (endBorrowDate != value)
                {
                    endBorrowDate = value;
                    OnPropertyChanged(nameof(EndBorrowDate));
                }
            }
        }
        // Ngày trả dự kiến
        private DateTime? startReturnDate;
        public DateTime? StartReturnDate
        {
            get => startReturnDate;
            set
            {
                if (startReturnDate != value)
                {
                    startReturnDate = value;
                    OnPropertyChanged(nameof(StartReturnDate));
                }
            }
        }

        private DateTime? endReturnDate;
        public DateTime? EndReturnDate
        {
            get => endReturnDate;
            set
            {
                if (endReturnDate != value)
                {
                    endReturnDate = value;
                    OnPropertyChanged(nameof(EndReturnDate));
                }
            }
        }
        // Ngày trả thực tế
        private DateTime? realStartReturnDate;
        public DateTime? RealStartReturnDate
        {
            get => realStartReturnDate;
            set
            {
                if (realStartReturnDate != value)
                {
                    realStartReturnDate = value;
                    OnPropertyChanged(nameof(RealStartReturnDate));
                }
            }
        }

        private DateTime? realEndReturnDate;
        public DateTime? RealEndReturnDate
        {
            get => realEndReturnDate;
            set
            {
                if (realEndReturnDate != value)
                {
                    realEndReturnDate = value;
                    OnPropertyChanged(nameof(RealEndReturnDate));
                }
            }
        }

        public ObservableCollection<DonMuon> DonMuonList { get; set; }
        public ObservableCollection<DonMuon> DonTraList { get; set; }
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
                        MaMuon = SelectedDonMuon.MaMuon;
                        MaDG = SelectedDonMuon.MaDG;
                        MaNV = SelectedDonMuon.MaNV;
                        NgayMuon = SelectedDonMuon.NgayMuon.ToShortDateString();
                        NgayTraDK = SelectedDonMuon.NgayTraDK.ToShortDateString();
                        if (SelectedDonMuon.NgayTraTT != null)
                        {
                            NgayTraTT = DateTime.Parse(SelectedDonMuon.NgayTraTT.ToString()).ToShortDateString();
                            RealStartReturnDate = RealEndReturnDate = SelectedDonMuon.NgayTraTT;
                        }
                        else
                            NgayTraTT = "";
                        PhiPhat = SelectedDonMuon.PhiPhat.ToString();
                        StartBorrowDate = EndBorrowDate = SelectedDonMuon.NgayMuon;
                        StartReturnDate = EndReturnDate = SelectedDonMuon.NgayTraDK;
                        DanhSachMuon.Clear();
                        foreach (var item in context.CTDMs)
                            if (item != null && item.MaMuon ==  SelectedDonMuon.MaMuon)
                            {
                                Sach sach = context.Sachs.Where(s => s.MaDauSach == item.MaDauSach && s.ISBN == item.ISBN).FirstOrDefault();
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
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SearchDonMuonCommand { get; set; }
        public ICommand SearchDonTraCommand { get; set; }
        public ICommand RestoreCommand { get; set; }
        public ICommand ShowCommand { get; set; }
        private async Task<string> CreateMaDMAsync()
        {
            // Generate a hash from MaMuon
            using (var context = new LibraryDbContext())
            {
                // Lấy tất cả các mã "MaMuon" hiện tại trong cơ sở dữ liệu có tiền tố "MN"
                var existingCodes = await context.DonMuons
                                                 .Where(tl => tl.MaMuon.StartsWith("MN"))
                                                 .Select(tl => tl.MaMuon)
                                                 .ToListAsync();

                var minUnusedNumber = existingCodes
                    .Select(code => int.TryParse(code.Substring(2), out int num) ? num : 0) // Lấy phần số sau "MN"
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

                // Trả về mã mới với định dạng "MN" + số có 3 chữ số
                return $"MN{newCodeNumber:D3}";
            }
        }
        private CTDMViewModel _ctdmViewModel;
        private LibraryDbContext context;
        public DonMuonViewModel()
        {
            _ctdmViewModel = new CTDMViewModel();
            context = new LibraryDbContext();
            DanhSachMuon = new ObservableCollection<BorrowedBook>();

            DonMuonList = new ObservableCollection<DonMuon>();
            DonTraList = new ObservableCollection<DonMuon>();

            AddCommand = new RelayCommand<object>((p) => true, async (p) => await AddDonMuon());
            EditCommand = new RelayCommand<object>((p) => SelectedDonMuon != null, async (p) => await EditDonMuon());
            DeleteCommand = new RelayCommand<object>((p) => SelectedDonMuon != null, async (p) => await DeleteDonMuon());
            SearchDonMuonCommand = new RelayCommand<string>((p) => true, async (p) => await SearchDonMuon());
            SearchDonTraCommand = new RelayCommand<string>((p) => true, async (p) => await SearchDonTra());
            RestoreCommand = new RelayCommand<object>((p) => SelectedDonMuon != null, async (p) => await TraDonMuon());
            ShowCommand = new RelayCommand<object>((p) => true, async (p) => await LoadDonMuonTraList());

            // Initialize the view model
            _ = InitializeAsync();
        }

        #region Method
        private async Task InitializeAsync()
        {
            await LoadDonMuonTraList();
        }

        private async Task LoadDonMuonTraList()
        {
            MaMuon = MaDG = MaNV = PhiPhat = "";
            StartBorrowDate = EndBorrowDate = StartReturnDate = EndReturnDate = RealStartReturnDate = RealEndReturnDate = null;
            DanhSachMuon.Clear();
            var donMuons = await GetAllDonMuonAsync();
            // Xử lý DonMuonList
            DonMuonList.Clear();
            foreach (var item in donMuons.Where(t => t.NgayTraTT == null))
            {
                DonMuonList.Add(item);
            }

            // Xử lý DonTraList
            DonTraList.Clear();
            foreach (var item in donMuons.Where(t => t.NgayTraTT != null))
            {
                DonTraList.Add(item);
            }
        }
        private async Task AddDonMuon()
        {
            MaMuon = MaDG = MaNV = PhiPhat = "";
            StartBorrowDate = EndBorrowDate = StartReturnDate = EndReturnDate = RealStartReturnDate = RealEndReturnDate = null;
            DanhSachMuon.Clear();
            var addborrowwindow = new addborrowwindow();
            addborrowwindow.ShowDialog();
            await LoadDonMuonTraList();
        }

        private async Task TraDonMuon()
        {
            if (SelectedDonMuon != null)
            {
                DateTime ngayTraTT = DateTime.Now.Date; // Ngày trả thực tế
                DateTime ngayTraDK = SelectedDonMuon.NgayTraDK.Date; // Ngày trả dự kiến

                int soNgayTre = (ngayTraTT - ngayTraDK).Days; // Tính số ngày trễ
                decimal phiPhat = 0;

                if (soNgayTre > 0)
                {
                    int soSach = context.CTDMs.Count(ctdm => ctdm.MaMuon == SelectedDonMuon.MaMuon);
                    phiPhat = soNgayTre * soSach * 5000;
                }

                DonMuon donMuon = new DonMuon()
                {
                    MaMuon = SelectedDonMuon.MaMuon,
                    MaDG = context.DonMuons.Where(t => t.MaMuon == SelectedDonMuon.MaMuon).Select(t => t.MaDG).FirstOrDefault(),
                    MaNV = GlobalData.LoginUser.UserID,
                    NgayMuon = SelectedDonMuon.NgayMuon,
                    NgayTraDK = SelectedDonMuon.NgayTraDK,
                    NgayTraTT = DateTime.Now.Date,  // Chỉ lấy phần ngày, tháng, năm
                    PhiPhat = phiPhat
                };

                // Cập nhật trạng thái sách thành "Có sẵn"
                var ctdmsToUpdate = context.CTDMs.Where(ctdm => ctdm.MaMuon == SelectedDonMuon.MaMuon).ToList();

                foreach (var item in ctdmsToUpdate)
                {
                    var sachToUpdate = context.Sachs
                                              .FirstOrDefault(s => s.MaDauSach == item.MaDauSach && s.ISBN == item.ISBN);
                    if (sachToUpdate != null)
                    {
                        sachToUpdate.TrangThai = "Có sẵn"; // Cập nhật trạng thái sách
                    }
                }
                // Lưu thay đổi vào cơ sở dữ liệu
                await context.SaveChangesAsync();

                bool isSuccess = await UpdateDonMuonInDatabaseAsync(donMuon);
                if (!isSuccess)
                    EXMessagebox.Show("Trả đơn thất bại");
                else
                {
                    EXMessagebox.Show("Trả đơn thành công");
                    await LoadDonMuonTraList();
                }
            }
        }

        private async Task EditDonMuon()
        {
            if (SelectedDonMuon != null)
            {
                // Here you would bind the selected object to edit mode, so changes reflect
                bool isSuccess = await UpdateDonMuonInDatabaseAsync(new DonMuon());
                if (!isSuccess)
                {
                    EXMessagebox.Show("Error updating the record.");
                }
            }
        }

        private async Task DeleteDonMuon()
        {
            if (SelectedDonMuon != null)
            {
                bool isSuccess = await DeleteDonMuonFromDatabaseAsync(SelectedDonMuon.MaMuon);
                if (isSuccess)
                {
                    //DonMuonList.Remove(SelectedDonMuon);
                    MaMuon = MaDG = MaNV = PhiPhat = "";
                    StartBorrowDate = EndBorrowDate = StartReturnDate = EndReturnDate = RealStartReturnDate = RealEndReturnDate = null;
                }
            }
        }

        private async Task SearchDonMuon()
        {
            var filteredList = await SearchDonMuonInDatabaseAsync(MaDG, MaNV);

            // Lọc theo ngày mượn
            if (StartBorrowDate.HasValue && EndBorrowDate.HasValue)
            {
                filteredList = filteredList.Where(dm => dm.NgayTraTT == null && dm.NgayMuon >= StartBorrowDate && dm.NgayMuon <= EndBorrowDate).ToList();
            }
            else if (StartBorrowDate.HasValue && !EndBorrowDate.HasValue)
            {
                filteredList = filteredList.Where(dm => dm.NgayTraTT == null && dm.NgayMuon >= StartBorrowDate).ToList();
            }
            else if (EndBorrowDate.HasValue && !StartBorrowDate.HasValue)
            {
                filteredList = filteredList.Where(dm => dm.NgayTraTT == null && dm.NgayMuon <= EndBorrowDate).ToList();
            }

            // Lọc theo ngày trả dự kiến
            if (StartReturnDate.HasValue && EndReturnDate.HasValue)
            {
                filteredList = filteredList.Where(dm => dm.NgayTraTT == null && dm.NgayTraDK >= StartReturnDate && dm.NgayTraDK <= EndReturnDate).ToList();
            }
            else if (StartReturnDate.HasValue && !EndReturnDate.HasValue)
            {
                filteredList = filteredList.Where(dm => dm.NgayTraTT == null && dm.NgayTraDK >= StartReturnDate).ToList();
            }
            else if (EndReturnDate.HasValue && !StartReturnDate.HasValue)
            {
                filteredList = filteredList.Where(dm => dm.NgayTraTT == null && dm.NgayTraDK <= EndReturnDate).ToList();
            }

            // Cập nhật danh sách
            DonMuonList.Clear();
            foreach (var donMuon in filteredList)
            {
                DonMuonList.Add(donMuon);
            }
        }
        private async Task SearchDonTra()
        {
            var filteredList = await SearchDonTraInDatabaseAsync(MaDG, MaNV);

            // Lọc theo ngày trả thực tế
            if (StartReturnDate.HasValue && EndReturnDate.HasValue)
            {
                filteredList = filteredList.Where(dm => dm.NgayTraTT != null && dm.NgayTraTT.HasValue && dm.NgayTraTT >= StartReturnDate && dm.NgayTraTT <= EndReturnDate).ToList();
            }
            else if (StartReturnDate.HasValue && !EndReturnDate.HasValue)
            {
                filteredList = filteredList.Where(dm => dm.NgayTraTT != null && dm.NgayTraTT.HasValue && dm.NgayTraTT >= StartReturnDate).ToList();
            }
            else if (EndReturnDate.HasValue && !StartReturnDate.HasValue)
            {
                filteredList = filteredList.Where(dm => dm.NgayTraTT != null && dm.NgayTraTT.HasValue && dm.NgayTraTT <= EndReturnDate).ToList();
            }

            // Lọc theo ngày trả dự kiến
            if (StartReturnDate.HasValue && EndReturnDate.HasValue)
            {
                filteredList = filteredList.Where(dm => dm.NgayTraTT != null && dm.NgayTraDK >= StartReturnDate && dm.NgayTraDK <= EndReturnDate).ToList();
            }
            else if (StartReturnDate.HasValue && !EndReturnDate.HasValue)
            {
                filteredList = filteredList.Where(dm => dm.NgayTraTT != null && dm.NgayTraDK >= StartReturnDate).ToList();
            }
            else if (EndReturnDate.HasValue && !StartReturnDate.HasValue)
            {
                filteredList = filteredList.Where(dm => dm.NgayTraTT != null && dm.NgayTraDK <= EndReturnDate).ToList();
            }

            // Lọc theo ngày trả thực tế cụ thể (realStartReturnDate và realEndReturnDate)
            if (RealStartReturnDate.HasValue && RealEndReturnDate.HasValue)
            {
                filteredList = filteredList.Where(dm => dm.NgayTraTT >= RealStartReturnDate && dm.NgayTraTT <= RealEndReturnDate).ToList();
            }
            else if (RealStartReturnDate.HasValue && !RealEndReturnDate.HasValue)
            {
                filteredList = filteredList.Where(dm => dm.NgayTraTT >= RealStartReturnDate).ToList();
            }
            else if (!RealStartReturnDate.HasValue && RealEndReturnDate.HasValue)
            {
                filteredList = filteredList.Where(dm => dm.NgayTraTT <= RealEndReturnDate).ToList();
            }

            // Cập nhật danh sách
            DonTraList.Clear();
            foreach (var donTra in filteredList)
            {
                DonTraList.Add(donTra);
            }
        }

        #endregion

        #region MethodToDatabase

        public async Task<bool> AddDonMuonToDatabaseAsync(DonMuon donMuon)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    context.DonMuons.Add(donMuon);
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
                EXMessagebox.Show($"Unexpected Error DonMuon: {ex.Message}");
            }
            return false;
        }

        public async Task<bool> UpdateDonMuonInDatabaseAsync(DonMuon donMuon)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    context.DonMuons.Update(donMuon);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error updating DonMuon: {ex.Message}");
                return false;
            }
        }

        private static async Task<bool> DeleteDonMuonFromDatabaseAsync(string maMuon)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    // Kiểm tra nếu có chi tiết mượn liên kết với đơn mượn
                    bool hasRelatedCTDM = await context.CTDMs.AnyAsync(ctdm => ctdm.MaMuon == maMuon);
                    if (hasRelatedCTDM)
                    {
                        EXMessagebox.Show("Không thể xóa đơn mượn vì có chi tiết mượn liên kết.");
                        return false;
                    }

                    // Nếu không có liên kết, tiến hành xóa đơn mượn
                    var donMuonToDelete = await context.DonMuons.FirstOrDefaultAsync(dm => dm.MaMuon == maMuon);
                    if (donMuonToDelete != null)
                    {
                        context.DonMuons.Remove(donMuonToDelete);
                        await context.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error deleting DonMuon: {ex.Message}");
                return false;
            }
        }

        public async Task<List<DonMuon>> SearchDonMuonInDatabaseAsync(string maDG, string maNV)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    // Truy vấn theo MaMuon, MaDG và MaNV
                    var query = context.DonMuons.AsQueryable();

                    if (!string.IsNullOrEmpty(maDG))
                    {
                        query = query.Where(dm => dm.MaDG.Contains(maDG));
                    }
                    if (!string.IsNullOrEmpty(maNV))
                    {
                        query = query.Where(dm => dm.MaNV.Contains(maNV));
                    }

                    return await query.Where(t => t.NgayTraTT == null).ToListAsync();
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error searching DonMuon: {ex.Message}");
                return new List<DonMuon>();
            }
        }
        public async Task<List<DonMuon>> SearchDonTraInDatabaseAsync(string maDG, string maNV)
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    // Truy vấn theo MaMuon, MaDG và MaNV
                    var query = context.DonMuons.AsQueryable();

                    if (!string.IsNullOrEmpty(maDG))
                    {
                        query = query.Where(dm => dm.MaDG.Contains(maDG));
                    }
                    if (!string.IsNullOrEmpty(maNV))
                    {
                        query = query.Where(dm => dm.MaNV.Contains(maNV));
                    }

                    // Chỉ lấy những đơn đã trả (Ngày trả thực tế không null)
                    query = query.Where(dm => dm.NgayTraTT.HasValue);

                    return await query.ToListAsync();
                }
            }
            catch (Exception ex)
            {
                EXMessagebox.Show($"Error searching DonTra: {ex.Message}");
                return new List<DonMuon>();
            }
        }
        public async Task<List<DonMuon>> GetAllDonMuonAsync()
        {
            try
            {
                using (var context = new LibraryDbContext())
                {
                    return await context.DonMuons.ToListAsync();
                }
            }
            catch
            {
                return new List<DonMuon>();
            }
        }

        #endregion
    }
    public class BorrowedBook
    {
        public string TenDauSach { get; set; }
        public string ISBN { get; set; }
    }
}