using System;
using System.ComponentModel.DataAnnotations;

namespace VinhKhanhTour.Shared.Models
{
    /// <summary>
    /// Thông báo nội bộ cho Chủ quán (Agency) khi Admin xử lý POI.
    /// Lưu vào DB để Agency xem lại lịch sử thông báo.
    /// </summary>
    public class AppNotification
    {
        public int Id { get; set; }

        /// <summary>AgencyId của người nhận thông báo.</summary>
        public int RecipientAgencyId { get; set; }

        /// <summary>ID của POI liên quan (nullable).</summary>
        public int? PoiId { get; set; }

        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
