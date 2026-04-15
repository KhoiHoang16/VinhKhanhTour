using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace VinhKhanhTour.CMS.Services
{
    public interface IEmailService
    {
        Task SendAccountLockedEmailAsync(string email, string name);
    }

    public class MockEmailService : IEmailService
    {
        private readonly ILogger<MockEmailService> _logger;

        public MockEmailService(ILogger<MockEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendAccountLockedEmailAsync(string email, string name)
        {
            var message = $@"
                🔔 [EMAIL NOTIFICATION]
                Tới: {email} ({name})
                Chủ đề: Thông báo khóa tài khoản - VinhKhanhTour
                
                Nội dung: 
                Chào {name},
                Tài khoản của bạn tạm thời bị khóa do vi phạm chính sách hoặc lý do bảo mật. 
                Vui lòng liên hệ hỗ trợ để biết thêm chi tiết.
                -------------------------------------------------
            ";

            _logger.LogWarning(message);
            return Task.CompletedTask;
        }
    }
}
