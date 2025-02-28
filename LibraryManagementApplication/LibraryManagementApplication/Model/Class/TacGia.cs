using System.ComponentModel.DataAnnotations;

namespace LibraryManagementApplication.Model.Class
{
    public class TacGia
    {
        [Key]
        public string MaTG { get; set; }  // Mã tác giả (khóa chính)
        public string TenTG { get; set; } // Tên tác giả

        // Navigation properties
        public ICollection<DauSach> DauSachs { get; set; } // Danh sách các đầu sách của tác giả

        public TacGia()
        {
            DauSachs = new HashSet<DauSach>(); // Khởi tạo danh sách để tránh null reference
        }
    }
}
