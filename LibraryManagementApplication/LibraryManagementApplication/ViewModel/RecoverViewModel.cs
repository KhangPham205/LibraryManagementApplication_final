using LibraryManagementApplication.Model.Class;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace LibraryManagementApplication.ViewModel
{
    public class RecoverViewModel : BaseViewModel
    {
        public string email { get; set; }
        public string sdt { get; set; }
        public string cccd { get; set; }
        public string noti {  get; set; }
        public DispatcherTimer dispatcherTimer;
        public int time;
        public ICommand RecoverCommand { get; set; }

        public RecoverViewModel()
        {
            RecoverCommand = new RelayCommand<TextBlock>((p) => true, (p) =>
            {
                recover(p);
            });
        }

        public async void recover(TextBlock countdown)
        {

            try
            {
                using (var context = new LibraryDbContext())
                {
                    // Tìm tài khoản có thông tin khớp với email, số điện thoại và CCCD
                    var taiKhoan = context.TaiKhoans.FirstOrDefault(tk => tk.Email == email && tk.SDT == sdt && tk.CCCD == cccd);

                    if (taiKhoan != null)
                    {
                        time = 3;
                        dispatcherTimer = new DispatcherTimer();
                        dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                        dispatcherTimer.Tick += DispatcherTimer_Tick;
                        noti = $"Mail sẽ được gửi đến bạn trong {time}s";
                        OnPropertyChanged(nameof(noti));
                        countdown.Visibility = Visibility.Visible;
                        dispatcherTimer.Start();
                        // Gửi email chứa mật khẩu
                        await SendEmail(taiKhoan.Email, taiKhoan.Password, taiKhoan.UserName);
                        //EXMessagebox.Show("Mật khẩu đã được gửi qua email của bạn!", "Thông báo");
                        if (time < 0)
                        {
                            dispatcherTimer.Stop();
                            countdown.Visibility = Visibility.Collapsed;
                        }
                        
                    }
                    else
                    {
                        // Nếu không tìm thấy tài khoản, hiển thị thông báo lỗi
                        EXMessagebox.Show("Không tìm thấy tài khoản với thông tin đã cung cấp!", "Lỗi");
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi trong quá trình tương tác với cơ sở dữ liệu hoặc gửi email
                EXMessagebox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi");
            }
        }

        private async Task SendEmail(string recipientEmail, string password, string username)
        {
            try
            {
                // Thông tin tài khoản Gmail
                string senderEmail = "khonggian2k0520@gmail.com";
                string senderPassword = "ttcs ttwh psdn izyw"; // Thay bằng App Password nếu đã tạo

                // Nội dung email
                string subject = "Thông báo khôi phục mật khẩu tài khoản";
                string body = $@"
Kính gửi {username},

Chúng tôi đã nhận được yêu cầu khôi phục mật khẩu từ bạn cho tài khoản của mình trên hệ thống quản lý thư viện.

Dưới đây là thông tin mật khẩu của bạn:
--------------------------------------------------
Mật khẩu: {password}
--------------------------------------------------

Vui lòng sử dụng mật khẩu này để đăng nhập vào hệ thống. Chúng tôi khuyến nghị bạn đổi mật khẩu sau khi đăng nhập để đảm bảo an toàn thông tin tài khoản.

Nếu Quý độc giả không yêu cầu khôi phục mật khẩu, vui lòng bỏ qua email này hoặc liên hệ ngay với chúng tôi qua email hỗ trợ: khonggian2k0520@gmail.com để được hỗ trợ kịp thời.

Chân thành cảm ơn bạn đã tin tưởng và sử dụng dịch vụ của chúng tôi.

Trân trọng,
Đội ngũ hỗ trợ
Thư viện điện tử LibraryService
Email: khonggian2k0520@gmail.com
Điện thoại: 0123 456 789
";


                // Cấu hình SMTP
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };

                // Tạo email
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false // Gửi dạng text
                };
                mail.To.Add(recipientEmail);

                // Gửi email
                await smtpClient.SendMailAsync(mail);
                Console.WriteLine("Email đã được gửi thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gửi email: {ex.Message}");
            }
        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (time >= 1)
            {;
                time--;
                noti = $"Mail sẽ được gửi đến bạn trong {time}s";
                OnPropertyChanged(nameof(noti));
                
            }
            else if (time == 0)
            {
                time--;
                noti = "Mail đã được gửi đến bạn!";
                OnPropertyChanged(nameof(noti));
                
            }
        }

    }
}