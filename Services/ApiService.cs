using System.Net.Http.Json;
using VinhKhanhTour.Shared.Models;
using System.Diagnostics;
using VinhKhanhTour.Shared.Data;

namespace VinhKhanhTour.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly VinhKhanhTour.Shared.Data.IPoiRepository _poiRepo;

        // Lock để tránh 2 lần SyncAnalytics chạy song song gây trùng lặp
        private static readonly SemaphoreSlim _syncLock = new SemaphoreSlim(1, 1);

        public ApiService(VinhKhanhTour.Shared.Data.IPoiRepository poiRepo)
        {
            _poiRepo = poiRepo;
            
            string baseUrl = "https://vinhkhanhtour.onrender.com";

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromSeconds(60)
            };
        }

        public async Task SyncDatabaseAsync()
        {
            try
            {
                var serverPois = await _httpClient.GetFromJsonAsync<List<Poi>>("/api/poi");
                if (serverPois != null && serverPois.Any())
                {
                    foreach (var poi in serverPois)
                    {
                        var exists = await _poiRepo.GetPoiAsync(poi.Id);
                        if (exists == null)
                        {
                            await _poiRepo.AddPoiAsync(poi);
                        }
                        else
                        {
                            await _poiRepo.UpdatePoiAsync(poi);
                        }
                    }

                    var localPois = await _poiRepo.GetAllPoisAsync();
                    var serverPoiIds = serverPois.Select(p => p.Id).ToHashSet();
                    
                    foreach (var localPoi in localPois)
                    {
                        if (!serverPoiIds.Contains(localPoi.Id))
                        {
                            await _poiRepo.DeletePoiAsync(localPoi);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Sync Error] POI sync failed: {ex.Message}");
            }

            // Luôn thử analytics sync sau POI sync
            await SyncAnalyticsAsync();
        }

        public async Task SyncAnalyticsAsync()
        {
            // Chỉ cho 1 luồng chạy cùng lúc
            if (!await _syncLock.WaitAsync(0))
            {
                Debug.WriteLine("[Sync Analytics] Đã có luồng khác đang sync, bỏ qua.");
                return;
            }

            try
            {
                var localHistories = await _poiRepo.GetUsageHistoryAsync();

                if (localHistories == null || !localHistories.Any())
                {
                    Debug.WriteLine("[Sync Analytics] Không có dữ liệu để đẩy.");
                    return;
                }

                Debug.WriteLine($"[Sync Analytics] Đang gửi {localHistories.Count} bản ghi...");

                var response = await _httpClient.PostAsJsonAsync("/api/analytics/sync-usage", localHistories);

                if (response.IsSuccessStatusCode)
                {
                    await _poiRepo.DeleteUsageHistoriesAsync(localHistories);
                    Debug.WriteLine($"[Sync Analytics] ✅ Đẩy thành công {localHistories.Count} bản ghi!");
                }
                else
                {
                    var body = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"[Sync Analytics] ❌ Server lỗi: {response.StatusCode} - {body}");
                }
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("[Sync Analytics] ⏱️ Timeout - dữ liệu vẫn an toàn trong SQLite.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Sync Analytics] ❌ Lỗi: {ex.Message}");
            }
            finally
            {
                _syncLock.Release();
            }
        }
    }
}
