using System.ComponentModel.DataAnnotations;

namespace LibraryManagementApplication.Model.Class
{
    public class Sach
    {
        [Key]
        public string MaDauSach { get; set; }
        public string TenDauSach { get; set; }
        public string ISBN { get; set; }
        public string ViTri { get; set; }
        public string TrangThai { get; set; }
        public int NamXB { get; set; }

        // Navigation property
        public DauSach DauSach { get; set; }

        // Navigation properties for relationships
        public ICollection<CTDM> CTDMs { get; set; }

        public Sach() { }

        public Sach(string maDauSach, string isbn, string viTri, string trangThai, int namXB)
        {
            MaDauSach = maDauSach;
            ISBN = isbn;
            ViTri = viTri;
            TrangThai = trangThai;
            NamXB = namXB;
        }
    }

}
