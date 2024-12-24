using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementApplication.Model.Class
{
    public class DonMuon
    {
        [Key]
        public string MaMuon { get; set; }
        public string MaDG { get; set; }
        public string MaNV { get; set; }

        private DateTime _ngayMuon;
        public DateTime NgayMuon
        {
            get => _ngayMuon.Date;  // Trả về chỉ phần ngày
            set => _ngayMuon = value.Date;  // Gán chỉ phần ngày
        }

        private DateTime _ngayTraDK;
        public DateTime NgayTraDK
        {
            get => _ngayTraDK.Date;  // Trả về chỉ phần ngày
            set => _ngayTraDK = value.Date;  // Gán chỉ phần ngày
        }

        private DateTime? _ngayTraTT;
        public DateTime? NgayTraTT
        {
            get => _ngayTraTT?.Date;  // Trả về chỉ phần ngày (nếu không null)
            set => _ngayTraTT = value?.Date;  // Gán chỉ phần ngày (nếu không null)
        }

        public decimal? PhiPhat { get; set; }

        // Navigation properties
        public DocGia DocGia { get; set; }
        public TaiKhoan TaiKhoan { get; set; }
        public ICollection<CTDM> CTDMs { get; set; }

        public DonMuon() { }

        public DonMuon(string maMuon, string maDG, string maNV, DateTime ngayMuon, DateTime ngayTraDK, DateTime? ngayTraTT, decimal phiPhat)
        {
            MaMuon = maMuon;
            MaDG = maDG;
            MaNV = maNV;
            NgayMuon = ngayMuon.Date; // Đảm bảo chỉ phần ngày được lưu
            NgayTraDK = ngayTraDK.Date; // Đảm bảo chỉ phần ngày được lưu
            NgayTraTT = ngayTraTT?.Date; // Đảm bảo chỉ phần ngày được lưu nếu không null
            PhiPhat = phiPhat;
        }
    }
}