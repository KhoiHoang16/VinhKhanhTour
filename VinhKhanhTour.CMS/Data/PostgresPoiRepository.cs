using Microsoft.EntityFrameworkCore;
using VinhKhanhTour.Shared.Data;
using VinhKhanhTour.Shared.Models;

namespace VinhKhanhTour.CMS.Data
{
    public class PostgresPoiRepository : IPoiRepository
    {
        private readonly IDbContextFactory<CmsDbContext> _factory;

        public PostgresPoiRepository(IDbContextFactory<CmsDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<Poi>> GetAllPoisAsync()
        {
            using var context = await _factory.CreateDbContextAsync();
            return await context.Pois.ToListAsync();
        }

        public async Task<Poi?> GetPoiAsync(int id)
        {
            using var context = await _factory.CreateDbContextAsync();
            return await context.Pois.FindAsync(id);
        }

        public async Task<int> SavePoiAsync(Poi poi)
        {
            using var context = await _factory.CreateDbContextAsync();
            if (poi.Id == 0)
            {
                context.Pois.Add(poi);
            }
            else
            {
                context.Pois.Update(poi);
            }
            return await context.SaveChangesAsync();
        }

        public async Task<int> AddPoiAsync(Poi poi)
        {
            using var context = await _factory.CreateDbContextAsync();
            context.Pois.Add(poi);
            return await context.SaveChangesAsync();
        }

        public async Task<int> UpdatePoiAsync(Poi poi)
        {
            using var context = await _factory.CreateDbContextAsync();
            context.Pois.Update(poi);
            return await context.SaveChangesAsync();
        }

        public async Task<int> DeletePoiAsync(Poi poi)
        {
            using var context = await _factory.CreateDbContextAsync();
            context.Pois.Remove(poi);
            return await context.SaveChangesAsync();
        }

        public async Task<List<Tour>> GetAllToursAsync()
        {
            using var context = await _factory.CreateDbContextAsync();
            return await context.Tours.ToListAsync();
        }

        public async Task<int> SaveTourAsync(Tour tour)
        {
            using var context = await _factory.CreateDbContextAsync();
            if (tour.Id == 0)
            {
                context.Tours.Add(tour);
            }
            else
            {
                context.Tours.Update(tour);
            }
            return await context.SaveChangesAsync();
        }

        public async Task<int> DeleteTourAsync(Tour tour)
        {
            using var context = await _factory.CreateDbContextAsync();
            context.Tours.Remove(tour);
            return await context.SaveChangesAsync();
        }

        public async Task<List<UsageHistory>> GetUsageHistoryAsync()
        {
            using var context = await _factory.CreateDbContextAsync();
            return await context.UsageHistories.OrderByDescending(h => h.Timestamp).ToListAsync();
        }

        public async Task<int> RecordUsageAsync(UsageHistory history)
        {
            using var context = await _factory.CreateDbContextAsync();
            context.UsageHistories.Add(history);
            return await context.SaveChangesAsync();
        }

        public async Task<int> DeleteUsageHistoriesAsync(List<UsageHistory> histories)
        {
            using var context = await _factory.CreateDbContextAsync();
            context.UsageHistories.RemoveRange(histories);
            return await context.SaveChangesAsync();
        }

        // --- APPROVAL WORKFLOW METHODS ---

        /// <summary>
        /// Lấy tất cả POI đang chờ duyệt (Pending). Dùng IgnoreQueryFilters để Admin xem được của mọi Agency.
        /// </summary>
        public async Task<List<Poi>> GetPendingPoisAsync()
        {
            using var context = await _factory.CreateDbContextAsync();
            return await context.Pois
                .IgnoreQueryFilters()
                .Where(p => p.ApprovalStatus == ApprovalStatus.Pending && !p.IsDeleted)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy POI theo ID, bỏ qua query filter (cho Admin duyệt POI của Agency khác).
        /// </summary>
        public async Task<Poi?> GetPoiByIdIgnoreFiltersAsync(int id)
        {
            using var context = await _factory.CreateDbContextAsync();
            return await context.Pois
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        /// <summary>
        /// Đếm số POI đang chờ duyệt (badge count cho NavMenu).
        /// </summary>
        public async Task<int> GetPendingCountAsync()
        {
            using var context = await _factory.CreateDbContextAsync();
            return await context.Pois
                .IgnoreQueryFilters()
                .CountAsync(p => p.ApprovalStatus == ApprovalStatus.Pending && !p.IsDeleted);
        }

        // --- NOTIFICATION METHODS ---

        public async Task AddNotificationAsync(AppNotification notification)
        {
            using var context = await _factory.CreateDbContextAsync();
            context.AppNotifications.Add(notification);
            await context.SaveChangesAsync();
        }

        public async Task<List<AppNotification>> GetNotificationsForAgencyAsync(int agencyId)
        {
            using var context = await _factory.CreateDbContextAsync();
            return await context.AppNotifications
                .Where(n => n.RecipientAgencyId == agencyId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(50)
                .ToListAsync();
        }

        public async Task<int> GetUnreadNotificationCountAsync(int agencyId)
        {
            using var context = await _factory.CreateDbContextAsync();
            return await context.AppNotifications
                .CountAsync(n => n.RecipientAgencyId == agencyId && !n.IsRead);
        }
    }
}
