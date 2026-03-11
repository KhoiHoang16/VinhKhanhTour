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

        private CancellationTokenSource? _ttsCts;

        public async Task PlayPoiNarrationAsync(Poi poi)
        {
            // Logic chống spam đọc đè liên tục
            if (poi.Id == _lastPlayedPoiId && DateTime.Now - _lastPlayedTime < _cooldown)
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

                // Yêu cầu đồ án 3: TTS Đa ngôn ngữ, Dung lượng nhẹ
                await TextToSpeech.Default.SpeakAsync(poi.TtsScript, options, cancelToken: _ttsCts.Token);
                
                Debug.WriteLine($"[NarrationEngine] Đã đọc xong kịch bản: {poi.Name}");
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
