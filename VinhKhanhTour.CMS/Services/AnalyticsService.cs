using VinhKhanhTour.Shared.Data;
using VinhKhanhTour.Shared.Models;

namespace VinhKhanhTour.CMS.Services
{
    public class AnalyticsService
    {
        private readonly IPoiRepository _repo;

        public AnalyticsService(IPoiRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<UsageHistory>> GetUsageHistoryAsync() => await _repo.GetUsageHistoryAsync();

        // Sample method to generate random mock data if history is empty (for demo purposes)
        public async Task GenerateMockDataAsync()
        {
            var existing = await GetUsageHistoryAsync();
            if (existing.Count > 0)
            {
                await _repo.DeleteUsageHistoriesAsync(existing);
            }

            var random = new Random();
            for (int i = 0; i < 50; i++)
            {
                await _repo.RecordUsageAsync(new UsageHistory
                {
                    DeviceId = Guid.NewGuid().ToString().Substring(0, 8),
                    PoiId = random.Next(1, 6),
                    ListenDurationSeconds = random.Next(10, 180),
                    IsQrTriggered = random.Next(10) > 7,
                    Timestamp = DateTime.UtcNow.AddHours(-random.Next(1, 168)),
                    // Tọa độ bám sát Vĩnh Khánh (10.7615, 106.7022) 
                    // Độ lệch nhỏ (.0015 đến .002) để dữ liệu tập trung thành dải thay vì rải rác rộng
                    UserLatitude = 10.761 + (random.NextDouble() * 0.0015),
                    UserLongitude = 106.7015 + (random.NextDouble() * 0.0018)
                });
            }
        }
    }
}
