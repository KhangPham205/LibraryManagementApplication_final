using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementApplication.Model.Class
{
    public class DocGia
    {
        [Key]
        public string MaDG { get; set; }
        public string TenDG { get; set; }
        public string Email { get; set; }
        public string SDT { get; set; }
        public string CCCD { get; set; }

        // Navigation properties
        public ICollection<DonMuon> DonMuons { get; set; }

        public DocGia() { }

        public DocGia(string maDG, string tenDG, string email, string sdt, string cccd)
        {
            MaDG = maDG;
            TenDG = tenDG;
            Email = email;
            SDT = sdt;
            CCCD = cccd;
        }
    }
}
