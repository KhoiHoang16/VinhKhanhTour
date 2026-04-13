using SQLite;
using System;

namespace VinhKhanhTour.Shared.Models
{
    public class DevicePurchase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string DeviceId { get; set; } = string.Empty;
        public int PoiId { get; set; }
        public string PurchaseType { get; set; } = "single";       // single | bundle | all_access
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "VND";
        public string PaymentMethod { get; set; } = string.Empty;  // momo | vnpay
        public string? TransactionId { get; set; }
        public string? RecoveryContact { get; set; }               // Email/SĐT optional cho khôi phục
        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }                   // null = vĩnh viễn
        public bool IsActive { get; set; } = true;
    }

    // DTO cho API unlock request
    public class UnlockRequest
    {
        public string DeviceId { get; set; } = string.Empty;
        public int PoiId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string? RecoveryContact { get; set; }
    }
}
