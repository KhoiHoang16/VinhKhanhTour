using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VinhKhanhTour.CMS.Data;
using VinhKhanhTour.Shared.Models;

namespace VinhKhanhTour.CMS.Services
{
    public class AppAnalyticsService : IAppAnalyticsService
    {
        private readonly CmsDbContext _dbContext;

        public AppAnalyticsService(CmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DashboardOverviewDto> GetDashboardOverviewAsync()
        {
            // 1. Total Users (Distinct Device IDs from Usage Histories + User Routes)
            var usageUsers = await _dbContext.UsageHistories.Select(x => x.DeviceId).Distinct().ToListAsync();
            var routeUsers = await _dbContext.UserRoutes.Select(x => x.DeviceId).Distinct().ToListAsync();
            var totalUsers = usageUsers.Union(routeUsers).Count();

            // 2. Active Devices (Devices that synced data in the last 7 days)
            var activeThreshold = DateTime.UtcNow.AddDays(-7);
            var activeUsageUsers = await _dbContext.UsageHistories
                .Where(x => x.Timestamp >= activeThreshold)
                .Select(x => x.DeviceId)
                .Distinct()
                .ToListAsync();
            var activeRouteUsers = await _dbContext.UserRoutes
                .Where(x => x.Timestamp >= activeThreshold)
                .Select(x => x.DeviceId)
                .Distinct()
                .ToListAsync();
            var activeDevices = activeUsageUsers.Union(activeRouteUsers).Count();

            // 3. Total Syncs (Count of audit logs tracking "DataSync" or simply sum of records in routes and histories)
            // As an approximation, total sync operations equals how many times data was sent to the server.
            // We can check the AppAuditLogs Action = 'DataSync' or fallback to total AppAuditLog.
            // Let's count "DataSync" actions.
            var totalSyncs = await _dbContext.AppAuditLogs.CountAsync(log => log.Action == "DataSync");

            // 4. Detailed History Separations (Listens vs QR)
            var totalListens = await _dbContext.UsageHistories.CountAsync(x => x.IsQrTriggered == false);
            var totalQrScans = await _dbContext.UsageHistories.CountAsync(x => x.IsQrTriggered == true);

            return new DashboardOverviewDto
            {
                TotalUsers = totalUsers,
                ActiveDevices = activeDevices,
                TotalSyncs = totalSyncs,
                TotalListens = totalListens,
                TotalQrScans = totalQrScans
            };
        }

        public async Task<PagedResult<AppAuditLogDto>> GetAuditLogsAsync(AuditLogFilterDto filter)
        {
            var query = _dbContext.AppAuditLogs.AsQueryable();

            if (!string.IsNullOrEmpty(filter.UserId))
            {
                query = query.Where(l => l.UserId == filter.UserId);
            }

            if (filter.StartDate.HasValue)
            {
                query = query.Where(l => l.Timestamp >= filter.StartDate.Value);
            }

            if (filter.EndDate.HasValue)
            {
                query = query.Where(l => l.Timestamp <= filter.EndDate.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(l => l.Timestamp)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(l => new AppAuditLogDto
                {
                    Id = l.Id,
                    UserId = l.UserId,
                    Action = l.Action,
                    Details = l.Details,
                    Timestamp = l.Timestamp
                })
                .ToListAsync();

            return new PagedResult<AppAuditLogDto>
            {
                Items = items,
                TotalCount = totalCount,
                CurrentPage = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task RecordAuditLogAsync(string userId, string action, string details)
        {
            var log = new AppAuditLog
            {
                UserId = userId,
                Action = action,
                Details = details,
                Timestamp = DateTime.UtcNow
            };
            
            _dbContext.AppAuditLogs.Add(log);
            await _dbContext.SaveChangesAsync();
        }
    }
}
