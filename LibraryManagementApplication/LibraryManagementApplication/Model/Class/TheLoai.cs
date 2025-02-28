using System.ComponentModel.DataAnnotations;

namespace LibraryManagementApplication.Model.Class
{
    public class TheLoai
    {
        [Key]
        public string MaTL { get; set; }  // Mã thể loại (khóa chính)
        public string TenTL { get; set; } // Tên thể loại

        // Navigation properties
        public ICollection<DauSach> DauSachs { get; set; } // Danh sách các đầu sách thuộc thể loại

        public TheLoai()
        {
            DauSachs = new HashSet<DauSach>(); // Khởi tạo danh sách để tránh null reference
        }
    }
}
