using System;
using System.ComponentModel.DataAnnotations;

namespace VinhKhanhTour.Shared.Models
{
    /// <summary>
    /// Tài khoản Khách du lịch (Tourist) — hoàn toàn độc lập với AppUser (CMS Admin/Agency).
    /// Đăng nhập qua Firebase Authentication (Google/Apple Sign-in) từ Mobile App.
    /// </summary>
    public class Tourist
    {
        public int Id { get; set; }

        /// <summary>
        /// Mật khẩu đã được mã hóa (Hashed)
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Email của người dùng di động
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Tên hiển thị
        /// </summary>
        [MaxLength(200)]
        public string? FullName { get; set; }

        /// <summary>
        /// URL ảnh đại diện
        /// </summary>
        [MaxLength(500)]
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// Trạng thái khóa tài khoản
        /// </summary>
        public bool IsLocked { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Thời điểm hoạt động cuối cùng
        /// </summary>
        public DateTime? LastActiveAt { get; set; }

        /// <summary>
        /// Địa chỉ IP cuối cùng truy cập
        /// </summary>
        [MaxLength(50)]
        public string? LastActiveIp { get; set; }

        /// <summary>
        /// Tên thiết bị/Trình duyệt cuối cùng truy cập
        /// </summary>
        [MaxLength(200)]
        public string? LastActiveDevice { get; set; }
    }
}
