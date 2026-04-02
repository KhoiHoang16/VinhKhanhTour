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

        public ApiService(VinhKhanhTour.Shared.Data.IPoiRepository poiRepo)
        {
            _poiRepo = poiRepo;
            
            // Dùng trực tiếp Server Render để bạn test luôn nhé
            string baseUrl = "https://vinhkhanhtour.onrender.com";

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromSeconds(5)
            };
        }

        public async Task SyncDatabaseAsync()
        {
            try
            {
                // Fetch the latest POIs from the CMS backend
                var serverPois = await _httpClient.GetFromJsonAsync<List<Poi>>("/api/poi");
                if (serverPois != null && serverPois.Any())
                {
                    // For a simple sync, let's upsert all server POIs into the local database
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

                    // Remove local POIs that no longer exist on the server
                    var localPois = await _poiRepo.GetAllPoisAsync();
                    var serverPoiIds = serverPois.Select(p => p.Id).ToHashSet();
                    
                    foreach (var localPoi in localPois)
                    {
                        if (!serverPoiIds.Contains(localPoi.Id))
                        {
                            await _poiRepo.DeletePoiAsync(localPoi);
                            Debug.WriteLine($"[Sync] Deleted local POI {localPoi.Id} as it was removed from CMS.");
                        }
                    }

                    Debug.WriteLine($"[Sync] Successfully synchronized {serverPois.Count} POIs from the CMS.");
                }

                // 2. Sync (Push) Usage History to CMS
                var localHistories = await _poiRepo.GetUsageHistoryAsync();
                if (localHistories != null && localHistories.Any())
                {
                    var response = await _httpClient.PostAsJsonAsync("/api/analytics/sync-usage", localHistories);
                    if (response.IsSuccessStatusCode)
                    {
                        // Clean up local after success
                        await _poiRepo.DeleteUsageHistoriesAsync(localHistories);
                        
                        // Optional: Record via audit log
                        var auditLog = new AppAuditLogDto
                        {
                            UserId = Microsoft.Maui.Devices.DeviceInfo.Current.Idiom.ToString() + "_" + Guid.NewGuid().ToString().Substring(0, 4),
                            Action = "DataSync",
                            Details = $"Pushed {localHistories.Count} usage history records."
                        };
                        await _httpClient.PostAsJsonAsync("/api/analytics/audit-logs", auditLog);
                        
                        Debug.WriteLine($"[Sync] Successfully pushed {localHistories.Count} Usage History items to CMS and cleared local cache.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Sync Error] Failed to sync with CMS: {ex.Message}");
                // If sync fails (e.g., no internet or CMS offline), we just continue with local SQLite data.
            }
        }
    }
}
