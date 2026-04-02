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
                // Render free tier cần 30-60s để cold start, 5s là quá ngắn
                Timeout = TimeSpan.FromSeconds(60)
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
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Sync Error] Failed to sync POIs: {ex.Message}");
            }

            // Luôn thử analytics sync sau POI sync, dù POI sync lỗi
            await SyncAnalyticsAsync();
        }

        public async Task SyncAnalyticsAsync()
        {
            // Dùng SemaphoreSlim để đảm bảo chỉ 1 luồng chạy sync tại 1 thời điểm
            if (!await _syncLock.WaitAsync(0))
            {
                Debug.WriteLine("[Sync Analytics] Đã có luồng khác đang sync, bỏ qua.");
                return;
            }

            try
            {
                var localHistories = await _poiRepo.GetUsageHistoryAsync();
                Debug.WriteLine($"[Sync Analytics] Tìm thấy {localHistories?.Count ?? 0} bản ghi cục bộ chờ đẩy lên CMS.");

                if (localHistories == null || !localHistories.Any())
                {
                    Debug.WriteLine("[Sync Analytics] Không có dữ liệu để đẩy.");
                    return;
                }

                Debug.WriteLine($"[Sync Analytics] Đang gửi {localHistories.Count} bản ghi lên {_httpClient.BaseAddress}api/analytics/sync-usage ...");

                var response = await _httpClient.PostAsJsonAsync("/api/analytics/sync-usage", localHistories);
                
                Debug.WriteLine($"[Sync Analytics] Server trả về: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    // Xóa dữ liệu cục bộ đã gửi thành công
                    await _poiRepo.DeleteUsageHistoriesAsync(localHistories);
                    
                    Debug.WriteLine($"[Sync Analytics] ✅ Đẩy thành công {localHistories.Count} bản ghi lên CMS!");

                    // Hiện thông báo trên App
                    MainThread.BeginInvokeOnMainThread(async () => 
                    {
                        try
                        {
                            if (Application.Current?.Windows?.FirstOrDefault()?.Page != null)
                            {
                                await Application.Current.Windows.First().Page!.DisplayAlert(
                                    "✅ Đã gửi CMS", 
                                    $"Ghi nhận thành công {localHistories.Count} lượt tương tác lên máy chủ!", 
                                    "OK");
                            }
                        }
                        catch { }
                    });
                }
                else
                {
                    var body = await response.Content.ReadAsStringAsync();
                    // Nếu server trả HTML, chỉ lấy 200 ký tự đầu
                    if (body.Contains("<!DOCTYPE") || body.Contains("<html"))
                    {
                        body = $"Server trả về HTML Error Page (HTTP {(int)response.StatusCode}). Có thể code CMS chưa được cập nhật trên Render.";
                    }
                    else if (body.Length > 300)
                    {
                        body = body.Substring(0, 300) + "...";
                    }
                    
                    Debug.WriteLine($"[Sync Analytics] ❌ Server từ chối: {response.StatusCode} - {body}");
                    
                    MainThread.BeginInvokeOnMainThread(async () => 
                    {
                        try
                        {
                            if (Application.Current?.Windows?.FirstOrDefault()?.Page != null)
                            {
                                await Application.Current.Windows.First().Page!.DisplayAlert(
                                    "❌ Lỗi Server", 
                                    body, 
                                    "Đóng");
                            }
                        }
                        catch { }
                    });
                }
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("[Sync Analytics] ⏱️ TIMEOUT! Server Render chưa kịp phản hồi.");
                MainThread.BeginInvokeOnMainThread(async () => 
                {
                    try
                    {
                        if (Application.Current?.Windows?.FirstOrDefault()?.Page != null)
                        {
                            await Application.Current.Windows.First().Page!.DisplayAlert(
                                "⏱️ Timeout", 
                                "Server Render đang khởi động lại (cold start). Dữ liệu đã lưu offline, sẽ tự đồng bộ lần sau.", 
                                "OK");
                        }
                    }
                    catch { }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Sync Analytics] ❌ Lỗi: {ex.GetType().Name} - {ex.Message}");
                MainThread.BeginInvokeOnMainThread(async () => 
                {
                    try
                    {
                        if (Application.Current?.Windows?.FirstOrDefault()?.Page != null)
                        {
                            await Application.Current.Windows.First().Page!.DisplayAlert(
                                "❌ Lỗi Đồng Bộ", 
                                $"{ex.GetType().Name}: {ex.Message}", 
                                "Đóng");
                        }
                    }
                    catch { }
                });
            }
            finally
            {
                _syncLock.Release();
            }
        }
    }
}
