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
        /// Trạng thái tài khoản (dùng cho khóa tài khoản tương lai)
        /// </summary>
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;
    }
}
