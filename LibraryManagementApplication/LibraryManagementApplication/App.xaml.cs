using LibraryManagementApplication.ViewModel;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Windows;

namespace LibraryManagementApplication
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Tên tài nguyên nhúng (Namespace + Tên file)
            string resourceName = "LibraryManagementApplication.LibraryDatabase.sqlite";
            string destinationPath = Path.Combine(AppContext.BaseDirectory, "LibraryDatabase.sqlite");

            try
            {
                // Nếu file chưa tồn tại, trích xuất từ tài nguyên nhúng
                if (!File.Exists(destinationPath))
                {
                    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                    using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
                    {
                        stream.CopyTo(fileStream);
                    }
                    MessageBox.Show("Database file has been extracted to the application directory.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi nếu không thể trích xuất file
                MessageBox.Show($"Failed to extract database file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // Tắt ứng dụng nếu không thể tiếp tục
                Shutdown();
            }
        }
    }
}
