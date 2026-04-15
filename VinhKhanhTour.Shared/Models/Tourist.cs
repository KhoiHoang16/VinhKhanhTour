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
        /// Firebase UID — mã định danh duy nhất từ Firebase Authentication
        /// </summary>
        [Required]
        [MaxLength(128)]
        public string SocialId { get; set; } = string.Empty;

        /// <summary>
        /// Email từ tài khoản Google/Apple
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Tên hiển thị từ tài khoản Social
        /// </summary>
        [MaxLength(200)]
        public string? FullName { get; set; }

        /// <summary>
        /// URL ảnh đại diện từ Google/Apple
        /// </summary>
        [MaxLength(500)]
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// Nhà cung cấp xác thực: "google" | "apple"
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string AuthProvider { get; set; } = "google";

        /// <summary>
        /// Trạng thái tài khoản (dùng cho khóa tài khoản tương lai)
        /// </summary>
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;
    }
}
