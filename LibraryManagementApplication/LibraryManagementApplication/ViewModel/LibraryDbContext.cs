using LibraryManagementApplication.Model.Class;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementApplication.ViewModel
{
    public class LibraryDbContext : DbContext
    {
        public DbSet<DauSach> DauSachs { get; set; }
        public DbSet<Sach> Sachs { get; set; }
        public DbSet<NhaXuatBan> NhaXuatBans { get; set; }
        public DbSet<TheLoai> TheLoais { get; set; }
        public DbSet<TacGia> TacGias { get; set; }
        public DbSet<DocGia> DocGias { get; set; }
        public DbSet<DonMuon> DonMuons { get; set; }
        public DbSet<CTDM> CTDMs { get; set; }
        public DbSet<TaiKhoan> TaiKhoans { get; set; }

        // Cấu hình chuỗi kết nối SQLite
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Tạo file SQLite trong thư mục chạy ứng dụng
            string databasePath = System.IO.Path.Combine(AppContext.BaseDirectory, "LibraryDatabase.sqlite");
            optionsBuilder.UseSqlite($"Data Source={databasePath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure DauSach table
            modelBuilder.Entity<DauSach>(entity =>
            {
                entity.ToTable("DauSach");
                entity.HasKey(e => e.MaDauSach);
                entity.HasOne(e => e.TacGia)
                      .WithMany(t => t.DauSachs)
                      .HasForeignKey(e => e.MaTG)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.TheLoai)
                      .WithMany(t => t.DauSachs)
                      .HasForeignKey(e => e.MaTL)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.NhaXuatBan)
                      .WithMany(n => n.DauSachs)
                      .HasForeignKey(e => e.MaNXB)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Sach table
            modelBuilder.Entity<Sach>(entity =>
            {
                entity.ToTable("Sach");
                entity.HasKey(e => new { e.MaDauSach, e.ISBN });
                entity.Property(e => e.TrangThai).IsRequired();
                entity.HasOne(e => e.DauSach)
                      .WithMany(d => d.Sachs)
                      .HasForeignKey(e => e.MaDauSach);
            });

            // Configure NhaXuatBan table
            modelBuilder.Entity<NhaXuatBan>(entity =>
            {
                entity.ToTable("NhaXuatBan");
                entity.HasKey(e => e.MaNXB);
            });

            // Configure TheLoai table
            modelBuilder.Entity<TheLoai>(entity =>
            {
                entity.ToTable("TheLoai");
                entity.HasKey(e => e.MaTL);
            });

            // Configure TacGia table
            modelBuilder.Entity<TacGia>(entity =>
            {
                entity.ToTable("TacGia");
                entity.HasKey(e => e.MaTG);
            });

            // Configure DocGia table
            modelBuilder.Entity<DocGia>(entity =>
            {
                entity.ToTable("DocGia");
                entity.HasKey(e => e.MaDG);
            });

            // Configure DonMuon table
            modelBuilder.Entity<DonMuon>(entity =>
            {
                entity.ToTable("DonMuon");
                entity.HasKey(e => e.MaMuon);
                entity.HasOne(e => e.DocGia)
                      .WithMany(d => d.DonMuons)
                      .HasForeignKey(e => e.MaDG);
                entity.HasOne(e => e.TaiKhoan)
                      .WithMany(t => t.DonMuons)
                      .HasForeignKey(e => e.MaNV);
            });

            // Configure CTDM table
            modelBuilder.Entity<CTDM>(entity =>
            {
                entity.ToTable("CTDM");
                entity.HasKey(e => new { e.MaMuon, e.MaDauSach, e.ISBN });
                entity.HasOne(e => e.DonMuon)
                      .WithMany(d => d.CTDMs)
                      .HasForeignKey(e => e.MaMuon);
                entity.HasOne(e => e.Sach)
                      .WithMany(s => s.CTDMs)
                      .HasForeignKey(e => new { e.MaDauSach, e.ISBN });
            });

            // Configure TaiKhoan table
            modelBuilder.Entity<TaiKhoan>(entity =>
            {
                entity.ToTable("TaiKhoan");
                entity.HasKey(e => e.UserID);
                entity.Property(e => e.Loai).IsRequired();
            });
        }
    }
}