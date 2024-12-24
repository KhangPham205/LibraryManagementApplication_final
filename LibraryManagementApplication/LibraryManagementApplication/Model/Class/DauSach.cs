using System.Collections.Generic;

namespace LibraryManagementApplication.Model.Class
{
    public class DauSach
    {
        // Các thuộc tính cơ bản
        public string MaDauSach { get; set; }  // Mã đầu sách (Khóa chính)
        public string TenDauSach { get; set; } // Tên đầu sách
        public string MaTG { get; set; }       // Mã tác giả (Khóa ngoại)
        public string NgonNgu { get; set; }    // Ngôn ngữ của đầu sách
        public string MaTL { get; set; }       // Mã thể loại (Khóa ngoại)
        public string MaNXB { get; set; }      // Mã nhà xuất bản (Khóa ngoại)

        // Các thuộc tính điều hướng (Navigation Properties)
        public TacGia TacGia { get; set; }     // Liên kết đến bảng TacGia
        public TheLoai TheLoai { get; set; }   // Liên kết đến bảng TheLoai
        public NhaXuatBan NhaXuatBan { get; set; } // Liên kết đến bảng NhaXuatBan

        public ICollection<Sach> Sachs { get; set; } // Danh sách các sách thuộc đầu sách

        // Constructor
        public DauSach()
        {
            Sachs = new HashSet<Sach>(); // Khởi tạo danh sách sách để tránh null reference
        }
    }
}
