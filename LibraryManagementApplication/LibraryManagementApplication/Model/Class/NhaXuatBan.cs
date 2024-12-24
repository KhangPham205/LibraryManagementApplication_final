using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementApplication.Model.Class
{
    public class NhaXuatBan
    {
        [Key]
        public string MaNXB { get; set; }  // Mã nhà xuất bản (khóa chính)
        public string TenNXB { get; set; } // Tên nhà xuất bản

        // Navigation properties
        public ICollection<DauSach> DauSachs { get; set; } // Danh sách các đầu sách do nhà xuất bản này xuất bản

        public NhaXuatBan()
        {
            DauSachs = new HashSet<DauSach>(); // Khởi tạo danh sách để tránh null reference
        }
    }
}
