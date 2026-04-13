using VinhKhanhTour.Shared.Data;
using VinhKhanhTour.Shared.Models;
using VinhKhanhTour.CMS.Data;

namespace VinhKhanhTour.CMS.Services
{
    public class PoiService
    {
        private readonly PostgresPoiRepository _repo;

        public PoiService(PostgresPoiRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Poi>> GetPoisAsync()
        {
            return await _repo.GetAllPoisAsync();
        }

        public async Task<Poi?> GetPoiAsync(int id)
        {
            var pois = await GetPoisAsync();
            return pois.FirstOrDefault(p => p.Id == id);
        }

        public async Task CreatePoiAsync(Poi poi)
        {
            await _repo.SavePoiAsync(poi);
        }

        public async Task UpdatePoiAsync(Poi poi)
        {
            await _repo.SavePoiAsync(poi);
        }

        public async Task DeletePoiAsync(int id)
        {
            var pois = await GetPoisAsync();
            var poi = pois.FirstOrDefault(p => p.Id == id);
            if (poi != null)
            {
                await _repo.DeletePoiAsync(poi);
            }
        }

        // --- APPROVAL WORKFLOW ---

        /// <summary>
        /// Lấy danh sách POI đang chờ duyệt (Admin-only).
        /// </summary>
        public async Task<List<Poi>> GetPendingPoisAsync()
        {
            return await _repo.GetPendingPoisAsync();
        }

        /// <summary>
        /// Đếm số POI đang chờ duyệt (cho badge count NavMenu).
        /// </summary>
        public async Task<int> GetPendingCountAsync()
        {
            return await _repo.GetPendingCountAsync();
        }

        /// <summary>
        /// Admin duyệt POI. Set trạng thái Approved và tạo thông báo cho Agency.
        /// </summary>
        public async Task<bool> ApprovePoiAsync(int id)
        {
            var poi = await _repo.GetPoiByIdIgnoreFiltersAsync(id);
            if (poi == null) return false;

            poi.ApprovalStatus = ApprovalStatus.Approved;
            poi.AdminNote = string.Empty;
            await _repo.UpdatePoiAsync(poi);

            // Tạo thông báo cho Agency
            if (poi.AgencyId.HasValue)
            {
                await _repo.AddNotificationAsync(new AppNotification
                {
                    RecipientAgencyId = poi.AgencyId.Value,
                    PoiId = poi.Id,
                    Title = "✅ POI đã được duyệt",
                    Message = $"Địa điểm \"{poi.Name}\" đã được Admin phê duyệt và hiển thị công khai.",
                    CreatedAt = DateTime.UtcNow
                });
            }

            return true;
        }

        /// <summary>
        /// Admin từ chối POI. Set trạng thái Rejected, ghi AdminNote, và tạo thông báo.
        /// </summary>
        public async Task<bool> RejectPoiAsync(int id, string note)
        {
            var poi = await _repo.GetPoiByIdIgnoreFiltersAsync(id);
            if (poi == null) return false;

            poi.ApprovalStatus = ApprovalStatus.Rejected;
            poi.AdminNote = note;
            await _repo.UpdatePoiAsync(poi);

            // Tạo thông báo cho Agency
            if (poi.AgencyId.HasValue)
            {
                await _repo.AddNotificationAsync(new AppNotification
                {
                    RecipientAgencyId = poi.AgencyId.Value,
                    PoiId = poi.Id,
                    Title = "❌ POI bị từ chối",
                    Message = $"Địa điểm \"{poi.Name}\" bị từ chối. Lý do: {note}",
                    CreatedAt = DateTime.UtcNow
                });
            }

            return true;
        }

        // --- NOTIFICATIONS ---

        public async Task<List<AppNotification>> GetNotificationsForAgencyAsync(int agencyId)
        {
            return await _repo.GetNotificationsForAgencyAsync(agencyId);
        }

        public async Task<int> GetUnreadNotificationCountAsync(int agencyId)
        {
            return await _repo.GetUnreadNotificationCountAsync(agencyId);
        }
    }
}
