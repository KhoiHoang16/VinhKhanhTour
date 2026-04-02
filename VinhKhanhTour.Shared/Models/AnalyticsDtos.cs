using System;
using System.Collections.Generic;

namespace VinhKhanhTour.Shared.Models
{
    public class DashboardOverviewDto
    {
        public int TotalUsers { get; set; }
        public int ActiveDevices { get; set; }
        public int TotalSyncs { get; set; }
        public int TotalListens { get; set; }
        public int TotalQrScans { get; set; }
    }

    public class AuditLogFilterDto
    {
        public string? UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / (PageSize > 0 ? PageSize : 1));
        public int PageSize { get; set; } = 20;
        public int CurrentPage { get; set; } = 1;
    }

    public class AppAuditLogDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
