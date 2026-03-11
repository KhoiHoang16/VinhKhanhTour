using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using VinhKhanhTour.Models;
using VinhKhanhTour.Services;
using VinhKhanhTour.Utilities;
using System.Diagnostics;

namespace VinhKhanhTour.Pages
{
    public partial class MapPage : ContentPage
    {
        private readonly NarrationEngine _narrationEngine;
        private bool _isTrackingLocation = false;

#if ANDROID || IOS || MACCATALYST
        private Microsoft.Maui.Controls.Maps.Map VinhKhanhMap;
#endif

        private bool _isMapLoaded = false;

        public MapPage(NarrationEngine narrationEngine)
        {
            InitializeComponent();
            _narrationEngine = narrationEngine;
            // Removed LoadPoisToMap() here to prevent blocking the UI during page construction 
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            
            // OnNavigatedTo được gọi SAU KHI animation chuyển trang đã hoàn tất hoàn toàn.
            if (!_isMapLoaded)
            {
                LoadPoisToMap();
                _isMapLoaded = true;
            }
            
            // Đợi thêm 1 chút để UI bản đồ kịp render mượt mà trước khi gọi hộp thoại Quyền GPS
            Task.Delay(500).ContinueWith(async (t) => 
            {
               await MainThread.InvokeOnMainThreadAsync(async () => 
               {
                   await StartLocationTracking();
               });
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            StopLocationTracking();
            _narrationEngine.CancelCurrentNarration();
        }

        private IDispatcherTimer? _locationTimer;

        private async Task StartLocationTracking()
        {
            if (_isTrackingLocation) return;
            
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }
                
                // Cấp quyền Location trên Android 14 đôi lúc rắc rối,
                // chuyển sang phương pháp Polling (hỏi liên tục) thay vì Foreground Service
                if (status == PermissionStatus.Granted)
                {
                    _isTrackingLocation = true;
                    _locationTimer = Dispatcher.CreateTimer();
                    _locationTimer.Interval = TimeSpan.FromSeconds(5); // Quét mỗi 5 giây
                    _locationTimer.Tick += async (s, e) => await CheckLocation();
                    _locationTimer.Start();
                }
                else
                {
                    await DisplayAlert("Lỗi Quyền", "Không thể truy cập vị trí để tự động Thuyết minh.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi bật GPS: {ex.Message}");
            }
        }

        private void StopLocationTracking()
        {
            if (!_isTrackingLocation) return;
            _isTrackingLocation = false;
            
            if (_locationTimer != null)
            {
                _locationTimer.Stop();
                _locationTimer = null;
            }
        }

        private async Task CheckLocation()
        {
            try
            {
                // Bắt buộc Không sử dụng Vị trí đệm (Cache) trên Android Emulator
                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(5));

                // Lấy vị trí trực tiếp từ Cảm biến Máy ảo
                Location? location = await Geolocation.Default.GetLocationAsync(request);

                if (location == null)
                {
                    // Fallback nếu GetLocation xịt
                    location = await Geolocation.Default.GetLastKnownLocationAsync();
                }

                if (location != null)
                {
                    Geolocation_LocationChanged(this, new GeolocationLocationChangedEventArgs(location));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi lấy tọa độ: {ex.Message}");
            }
        }

        private async void Geolocation_LocationChanged(object sender, GeolocationLocationChangedEventArgs e)
        {
            var userLocation = e.Location;
            var pois = Poi.GetSampleData(); // Giả lập quét danh sách 5 quán ốc

            // Tìm quán ốc gần nhất trong bán kính kích hoạt
            Poi? nearestTriggeredPoi = null;
            double minDistance = double.MaxValue;
            double nearestDistance = double.MaxValue;
            string closestPoiName = "Chưa tìm thấy quán ốc";

            foreach (var poi in pois)
            {
                // Engine tính khoảng cách (Geofence Engine)
                double distanceToPoi = LocationHelper.CalculateDistanceInMeters(
                    userLocation.Latitude, userLocation.Longitude,
                    poi.Latitude, poi.Longitude);

                if (distanceToPoi < nearestDistance)
                {
                    nearestDistance = distanceToPoi;
                    closestPoiName = poi.Name;
                }

                if (distanceToPoi <= poi.Radius)
                {
                    // Đạt yêu cầu đồ án: So sánh Priority và Cự ly
                    // Nếu gần hơn, hoặc cùng khoảng cách nhưng ưu tiên cao hơn
                    if (distanceToPoi < minDistance || 
                        (distanceToPoi == minDistance && (nearestTriggeredPoi == null || poi.Priority > nearestTriggeredPoi.Priority)))
                    {
                        nearestTriggeredPoi = poi;
                        minDistance = distanceToPoi;
                    }
                }
            }

            // Cập nhật giao diện Debug để User nhìn thấy
            if (DebugLabel != null)
            {
                MainThread.BeginInvokeOnMainThread(() => 
                {
                    DebugLabel.Text = $"Vị trí của bạn: {userLocation.Latitude:F5}, {userLocation.Longitude:F5}\n" +
                                      $"Gần nhất: {closestPoiName} ({nearestDistance:F0}m)";
                });
            }

            if (nearestTriggeredPoi != null)
            {
                Debug.WriteLine($"[Geofence] Đã rơi vào bán kính quán: {nearestTriggeredPoi.Name} ({minDistance}m)");
                
                if (DebugLabel != null)
                {
                    MainThread.BeginInvokeOnMainThread(() => 
                    {
                        DebugLabel.Text += $"\n🎤 Đang phát âm thanh: {nearestTriggeredPoi.Name}!";
                    });
                }

                // Gọi Động cơ Thuyết minh (Narration Engine)
                await _narrationEngine.PlayPoiNarrationAsync(nearestTriggeredPoi);
            }
        }

        private void LoadPoisToMap()
        {
#if ANDROID || IOS || MACCATALYST
            VinhKhanhMap = new Microsoft.Maui.Controls.Maps.Map
            {
                IsShowingUser = true,
                MapType = MapType.Street
            };
            MapContainer.Children.Add(VinhKhanhMap);

            // 1. Get 5 sample snail places
            var poiList = Poi.GetSampleData();

            // 2. Loop through and create a Pin for each POI
            foreach (var poi in poiList)
            {
                var pin = new Pin
                {
                    Label = poi.Name,
                    Address = $"Bán kính: {poi.Radius}m",
                    Type = PinType.Place,
                    Location = new Location(poi.Latitude, poi.Longitude)
                };

                // Add to Map's Pins collection
                VinhKhanhMap.Pins.Add(pin);
            }

            // 3. Center the map at Vinh Khanh street (e.g., using the first POI as center)
            if (poiList.Any())
            {
                var centerLocation = new Location(poiList[0].Latitude, poiList[0].Longitude);
                var mapSpan = MapSpan.FromCenterAndRadius(centerLocation, Distance.FromKilometers(1));
                
                // Since we are already on the MainThread inside OnAppearing's delay, 
                // we can move directly without another dispatcher.
                try 
                {
                    VinhKhanhMap.MoveToRegion(mapSpan);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Lỗi zoom bản đồ: {ex.Message}");
                }
            }
#else
            MapContainer.Children.Add(new Label 
            {
                Text = "Bản đồ không được hỗ trợ trên Windows. Vui lòng chạy ứng dụng trên thiết bị Android hoặc iOS.",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(20)
            });
#endif
        }
    }
}
