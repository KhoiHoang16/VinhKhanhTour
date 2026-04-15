using System;
using System.ComponentModel.DataAnnotations;

namespace VinhKhanhTour.Shared.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "Client";

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? FullName { get; set; }

        public int? AgencyId { get; set; }

        public bool ForcePasswordChange { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Trạng thái khóa tài khoản
        /// </summary>
        public bool IsLocked { get; set; } = false;

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
