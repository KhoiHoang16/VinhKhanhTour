using System.Collections.Generic;
using System.Threading.Tasks;
using VinhKhanhTour.Shared.Models;

namespace VinhKhanhTour.Shared.Data
{
    public interface IPoiRepository
    {
        // POI
        Task<List<Poi>> GetAllPoisAsync();
        Task<Poi?> GetPoiAsync(int id);
        Task<int> SavePoiAsync(Poi poi);
        Task<int> AddPoiAsync(Poi poi);
        Task<int> UpdatePoiAsync(Poi poi);
        Task<int> DeletePoiAsync(Poi poi);

        // Tour
        Task<List<Tour>> GetAllToursAsync();
        Task<int> SaveTourAsync(Tour tour);
        Task<int> DeleteTourAsync(Tour tour);

        // Usage History
        Task<List<UsageHistory>> GetUsageHistoryAsync();
        Task<int> RecordUsageAsync(UsageHistory history);
        Task<int> DeleteUsageHistoriesAsync(List<UsageHistory> histories);
    }
}
