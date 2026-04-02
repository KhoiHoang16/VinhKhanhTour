using System.Diagnostics;
using VinhKhanhTour.Models;

namespace VinhKhanhTour.Services
{
    public class NarrationEngine
    {
        private int _lastPlayedPoiId = -1;
        private DateTime _lastPlayedTime = DateTime.MinValue;

        // Cấu hình chống spam (Debounce / Cooldown)
        // Không đọc lại cùng 1 điểm nếu chưa trôi qua 2 phút
        private readonly TimeSpan _cooldown = TimeSpan.FromMinutes(2);
        private readonly VinhKhanhTour.Shared.Data.IPoiRepository _poiRepository;
        private readonly VinhKhanhTour.Services.ApiService _apiService;
        private CancellationTokenSource? _ttsCts;

        public NarrationEngine(VinhKhanhTour.Shared.Data.IPoiRepository poiRepository, VinhKhanhTour.Services.ApiService apiService)
        {
            _poiRepository = poiRepository;
            _apiService = apiService;
        }

        public async Task PlayPoiNarrationAsync(Poi poi, bool isManual = false, bool isQrTriggered = false)
        {
            // Logic chống spam đọc đè liên tục (bỏ qua nếu là click thủ công)
            if (!isManual && poi.Id == _lastPlayedPoiId && DateTime.Now - _lastPlayedTime < _cooldown)
            {
                Debug.WriteLine($"[NarrationEngine] Đang trong thời gian Cooldown. Bỏ qua: {poi.Name}");
                return;
            }

            try
            {
                // Nếu đang phát dở một âm thanh khác thì hủy luôn cái cũ
                CancelCurrentNarration();

                _ttsCts = new CancellationTokenSource();

                Debug.WriteLine($"[NarrationEngine] Đã kích hoạt kịch bản: {poi.Name}");
                _lastPlayedPoiId = poi.Id;
                _lastPlayedTime = DateTime.Now;

                // Các tùy chọn nâng cao cho Text-To-Speech (mặc định giọng hệ thống)
                var options = new SpeechOptions
                {
                    Pitch = 1.0f, // Độ thanh
                    Volume = 1.0f // Âm lượng
                };

                // Cấu hình ngôn ngữ TTS
                var currentCode = Services.LocalizationResourceManager.Instance.CurrentLanguageCode;
                var locales = await TextToSpeech.Default.GetLocalesAsync();
                var localesList = locales?.ToList() ?? new List<Locale>();
                
                // IN RA CONSOLE TẤT CẢ GIỌNG ĐỌC MÀ HỆ ĐIỀU HÀNH HIỆN CÓ
                Debug.WriteLine($"[NarrationEngine] Đang tìm kiếm giọng đọc cho mã: '{currentCode}'...");
                Debug.WriteLine($"[NarrationEngine] --- DANH SÁCH GIỌNG ĐỌC CỦA MÁY ({localesList.Count}) ---");
                foreach (var loc in localesList)
                {
                    Debug.WriteLine($"   Id: {loc.Id} | Lang: {loc.Language} | Name: {loc.Name}");
                }
                Debug.WriteLine($"[NarrationEngine] ---------------------------------------------");

                // Sử dụng CultureInfo để tự động lấy mã 2 chữ, 3 chữ và tên tiếng Anh
                System.Globalization.CultureInfo cultureInfo;
                try
                {
                    cultureInfo = new System.Globalization.CultureInfo(currentCode);
                }
                catch
                {
                    cultureInfo = new System.Globalization.CultureInfo("en");
                }

                var twoLetter = cultureInfo.TwoLetterISOLanguageName;       // VD: "vi"
                var threeLetter = cultureInfo.ThreeLetterISOLanguageName;   // VD: "vie"
                var englishName = cultureInfo.EnglishName;                  // VD: "Vietnamese"
                int spaceIndex = englishName.IndexOf(' ');
                var searchName = spaceIndex > 0 ? englishName.Substring(0, spaceIndex) : englishName;

                // TÌM KIẾM THUẬT TOÁN THÁC NƯỚC (CASCADING) CẢI TIẾN
                // 1. Khớp mã 2 chữ ("vi", "es") hoặc mã 3 chữ ("vie", "spa")
                var selectedLocale = localesList.FirstOrDefault(l => 
                    l.Language != null && (
                        l.Language.Equals(twoLetter, StringComparison.OrdinalIgnoreCase) ||
                        l.Language.Equals(threeLetter, StringComparison.OrdinalIgnoreCase) ||
                        l.Language.Equals(currentCode, StringComparison.OrdinalIgnoreCase)
                    ));

                // 2. Khớp chuỗi bắt đầu mã Language hợp lệ (VD: "vi-VN", "es-ES", "zh-CN")
                selectedLocale ??= localesList.FirstOrDefault(l => 
                    l.Language != null && (
                        l.Language.StartsWith(twoLetter + "-", StringComparison.OrdinalIgnoreCase) || 
                        l.Language.StartsWith(twoLetter + "_", StringComparison.OrdinalIgnoreCase) ||
                        l.Language.StartsWith(threeLetter + "-", StringComparison.OrdinalIgnoreCase) ||
                        l.Language.StartsWith(threeLetter + "_", StringComparison.OrdinalIgnoreCase) ||
                        l.Language.StartsWith(currentCode + "-", StringComparison.OrdinalIgnoreCase) ||
                        l.Language.StartsWith(currentCode + "_", StringComparison.OrdinalIgnoreCase)
                    )
                );

                // 3. Khớp tên tiếng Anh (VD: "Vietnamese", "Spanish", "Japanese")
                // Google Speech Services thường có tên kiểu "Vietnamese (Vietnam)"
                selectedLocale ??= localesList.FirstOrDefault(l => 
                    l.Name != null && l.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase)
                );

                // 4. Fallback đặc biệt cho các ngôn ngữ khi Google trả về ID đặc biệt
                selectedLocale ??= localesList.FirstOrDefault(l =>
                    l.Id != null && l.Id.Contains(twoLetter, StringComparison.OrdinalIgnoreCase)
                );

                if (selectedLocale != null)
                {
                    options.Locale = selectedLocale;
                }

                // Yêu cầu đồ án 3: TTS Đa ngôn ngữ, Dung lượng nhẹ
                if (options.Locale != null)
                {
                    Debug.WriteLine($"[NarrationEngine] Phát bằng ngôn ngữ: {options.Locale.Language} - {options.Locale.Name}");
                }
                else
                {
                    Debug.WriteLine($"[NarrationEngine] Không tìm thấy locale phù hợp cho '{currentCode}', dùng mặc định.");
                }

                await TextToSpeech.Default.SpeakAsync(poi.DisplayTtsScript, options, cancelToken: _ttsCts.Token);
                
                // Thu thập Lịch sử sử dụng ghi nhận vào DB cục bộ
                try 
                {
                    await _poiRepository.RecordUsageAsync(new VinhKhanhTour.Shared.Models.UsageHistory 
                    {
                        PoiId = poi.Id,
                        ListenDurationSeconds = 10, // Giả lập độ dài nghe (Có thể lấy từ AudioLength nếu có)
                        IsQrTriggered = isQrTriggered,
                        Timestamp = DateTime.UtcNow,
                        DeviceId = Microsoft.Maui.Devices.DeviceInfo.Current.Idiom.ToString() + "_" + Guid.NewGuid().ToString().Substring(0, 4)
                    });
                    
                    // Fire-and-forget logic để lập tức gọi đẩy API luôn, không đợi lần mở app sau
                    _ = Task.Run(async () => 
                    {
                        try 
                        {
                            await _apiService.SyncDatabaseAsync();
                        }
                        catch { }
                    });
                } 
                catch (Exception ex)
                {
                    Debug.WriteLine($"[NarrationEngine] Lỗi ghi nhận lịch sử: {ex.Message}");
                }

                Debug.WriteLine($"[NarrationEngine] Đã đọc xong kịch bản ({currentCode}): {poi.Name}");
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("[NarrationEngine] TTS bị hủy bỏ vì có yêu cầu mới tới cắn ngang.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[NarrationEngine] Lỗi phát TTS: {ex.Message}");
            }
        }

        public void CancelCurrentNarration()
        {
            if (_ttsCts?.IsCancellationRequested == false)
            {
                _ttsCts.Cancel();
                _ttsCts.Dispose();
                _ttsCts = null;
            }
        }
    }
}
