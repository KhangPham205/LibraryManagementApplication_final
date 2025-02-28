using System.ComponentModel.DataAnnotations;

namespace LibraryManagementApplication.Model.Class
{
    public class TaiKhoan
    {
        [Key]
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Loai { get; set; }
        public string Email { get; set; }
        public string SDT { get; set; }
        public string CCCD { get; set; }
        public byte[]? ProfileImage { get; set; } // Dữ liệu ảnh

        // Navigation properties
        public ICollection<DonMuon> DonMuons { get; set; }

        public TaiKhoan() { }

        public TaiKhoan(string userID, string userName, string password, string loai, string email, string sdt, string cccd)
        {
            UserID = userID;
            UserName = userName;
            Password = password;
            Loai = loai;
            Email = email;
            SDT = sdt;
            CCCD = cccd;
        }
    }
}
