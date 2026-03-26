using SQLite;
using System;

namespace VinhKhanhTour.Shared.Models
{
    public class UsageHistory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string DeviceId { get; set; } = string.Empty;
        public int PoiId { get; set; }
        public int ListenDurationSeconds { get; set; }
        public bool IsQrTriggered { get; set; }
        public double UserLatitude { get; set; }
        public double UserLongitude { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class UserRoute
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string DeviceId { get; set; } = string.Empty;
        // Stores JSON string of ArrayList of route points
        public string RouteDataJson { get; set; } = string.Empty; 
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
