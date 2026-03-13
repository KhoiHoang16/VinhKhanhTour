using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using VinhKhanhTour.Models;
using VinhKhanhTour.Services;
using VinhKhanhTour.Utilities;
using System.Diagnostics;

namespace VinhKhanhTour.Pages
{
    [QueryProperty(nameof(SelectedPoiId), "poiId")]
    public partial class MapPage : ContentPage
    {
        private readonly NarrationEngine _narrationEngine;
        private bool _isTrackingLocation = false;

        private int _selectedPoiId = 0;
        public int SelectedPoiId
        {
            get => _selectedPoiId;
            set
            {
                _selectedPoiId = value;
                if (_isMapLoaded)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        UpdateMapCirclesState();
                        CenterOnSelectedPoi();
                    });
                }
            }
        }

        private int _playingPoiId = 0; // Thêm ID quán đang phát thuyết minh

#if ANDROID || IOS || MACCATALYST
        private Microsoft.Maui.Controls.Maps.Map VinhKhanhMap;
        private Dictionary<int, Microsoft.Maui.Controls.Maps.Circle> _poiCircles = new();
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

#if ANDROID
                    // Start Background Service
                    if (Platform.CurrentActivity is MainActivity mainActivity)
                    {
                        mainActivity.StartLocationService();
                        VinhKhanhTour.Platforms.Android.BackgroundLocationTracker.LocationChanged += OnBackgroundLocationChanged;
                    }
#endif
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

        private void OnBackgroundLocationChanged(object? sender, Location location)
        {
            MainThread.BeginInvokeOnMainThread(() => 
            {
                Geolocation_LocationChanged(this, new GeolocationLocationChangedEventArgs(location));
            });
        }

        private void StopLocationTracking()
        {
            if (!_isTrackingLocation) return;
            _isTrackingLocation = false;

#if ANDROID
            if (Platform.CurrentActivity is MainActivity mainActivity)
            {
                mainActivity.StopLocationService();
                VinhKhanhTour.Platforms.Android.BackgroundLocationTracker.LocationChanged -= OnBackgroundLocationChanged;
            }
#endif
            
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

        private Location? _currentUserLocation;

        private void OnMyLocationClicked(object sender, EventArgs e)
        {
#if ANDROID || IOS || MACCATALYST
            if (_currentUserLocation != null && VinhKhanhMap != null)
            {
                VinhKhanhMap.MoveToRegion(MapSpan.FromCenterAndRadius(_currentUserLocation, Distance.FromKilometers(0.5)));
            }
#endif
        }

        private void OnZoomInClicked(object sender, EventArgs e)
        {
#if ANDROID || IOS || MACCATALYST
            if (VinhKhanhMap != null)
            {
                var currentSpan = VinhKhanhMap.VisibleRegion;
                if (currentSpan != null)
                {
                    VinhKhanhMap.MoveToRegion(MapSpan.FromCenterAndRadius(currentSpan.Center, Distance.FromKilometers(currentSpan.Radius.Kilometers / 2)));
                }
            }
#endif
        }

        private void OnZoomOutClicked(object sender, EventArgs e)
        {
#if ANDROID || IOS || MACCATALYST
            if (VinhKhanhMap != null)
            {
                var currentSpan = VinhKhanhMap.VisibleRegion;
                if (currentSpan != null)
                {
                    VinhKhanhMap.MoveToRegion(MapSpan.FromCenterAndRadius(currentSpan.Center, Distance.FromKilometers(currentSpan.Radius.Kilometers * 2)));
                }
            }
#endif
        }

        private bool _isPlaying = false;
        private void OnPlayPauseClicked(object sender, EventArgs e)
        {
            _isPlaying = !_isPlaying;
            PlayPauseButton.Text = _isPlaying ? "⏸" : "▶";
            
            if (!_isPlaying)
            {
                _narrationEngine.CancelCurrentNarration();
            }
        }

        private async void Geolocation_LocationChanged(object sender, GeolocationLocationChangedEventArgs e)
        {
            _currentUserLocation = e.Location;
            var pois = Poi.GetSampleData(); 

            // Tìm quán ốc gần nhất trong bán kính kích hoạt
            Poi? nearestTriggeredPoi = null;
            double minDistance = double.MaxValue;
            double nearestDistance = double.MaxValue;
            string closestPoiName = "Chưa tìm thấy quán ốc";

            foreach (var poi in pois)
            {
                double distanceToPoi = LocationHelper.CalculateDistanceInMeters(
                    _currentUserLocation.Latitude, _currentUserLocation.Longitude,
                    poi.Latitude, poi.Longitude);

                if (distanceToPoi < nearestDistance)
                {
                    nearestDistance = distanceToPoi;
                    closestPoiName = poi.Name;
                }

                if (distanceToPoi <= poi.Radius)
                {
                    if (distanceToPoi < minDistance || 
                        (distanceToPoi == minDistance && (nearestTriggeredPoi == null || poi.Priority > nearestTriggeredPoi.Priority)))
                    {
                        nearestTriggeredPoi = poi;
                        minDistance = distanceToPoi;
                    }
                }
            }

            // Cập nhật giao diện chuyên nghiệp
            MainThread.BeginInvokeOnMainThread(() => 
            {
                DebugLabel.Text = $"GPS: {_currentUserLocation.Latitude:F5}, {_currentUserLocation.Longitude:F5}";
                DistanceLabel.Text = $"Cách quán gần nhất: {nearestDistance:F0}m";
                
                if (nearestTriggeredPoi != null)
                {
                    PoiNameLabel.Text = nearestTriggeredPoi.Name;
                }
                else
                {
                    PoiNameLabel.Text = closestPoiName;
                }
            });

            if (nearestTriggeredPoi != null)
            {
                Debug.WriteLine($"[Geofence] Đã rơi vào bán kính quán: {nearestTriggeredPoi.Name} ({minDistance}m)");
                
                // Gọi Động cơ Thuyết minh (Narration Engine)
                if (_isPlaying)
                {
                    _playingPoiId = nearestTriggeredPoi.Id;
                    MainThread.BeginInvokeOnMainThread(() => UpdateMapCirclesState());
                    await _narrationEngine.PlayPoiNarrationAsync(nearestTriggeredPoi);
                }
                else
                {
                    _playingPoiId = 0;
                    MainThread.BeginInvokeOnMainThread(() => UpdateMapCirclesState());
                }
            }
            else
            {
                if (_playingPoiId != 0)
                {
                    _playingPoiId = 0;
                    MainThread.BeginInvokeOnMainThread(() => UpdateMapCirclesState());
                }
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

                var circle = new Microsoft.Maui.Controls.Maps.Circle
                {
                    Center = new Location(poi.Latitude, poi.Longitude),
                    Radius = Distance.FromMeters(poi.Radius),
                    StrokeColor = Color.FromArgb("#808080"), // Xám mặc định
                    StrokeWidth = 2,
                    FillColor = Color.FromRgba(128, 128, 128, 50)
                };
                VinhKhanhMap.MapElements.Add(circle);
                _poiCircles[poi.Id] = circle;
            }

            UpdateMapCirclesState();
            CenterOnSelectedPoi();
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

        private void UpdateMapCirclesState()
        {
#if ANDROID || IOS || MACCATALYST
            foreach (var kvp in _poiCircles)
            {
                int poiId = kvp.Key;
                var circle = kvp.Value;

                if (poiId == _playingPoiId)
                {
                    // Đang phát (Màu Vàng)
                    circle.StrokeColor = Color.FromArgb("#FFD700");
                    circle.FillColor = Color.FromRgba(255, 215, 0, 80);
                }
                else if (poiId == _selectedPoiId)
                {
                    // Được chọn từ list (Màu Xanh Cyan/Lục)
                    circle.StrokeColor = Color.FromArgb("#00CED1");
                    circle.FillColor = Color.FromRgba(0, 206, 209, 80);
                }
                else
                {
                    // Chưa đụng tới (Xám)
                    circle.StrokeColor = Color.FromArgb("#A9A9A9");
                    circle.FillColor = Color.FromRgba(169, 169, 169, 40);
                }
            }
#endif
        }

        private void CenterOnSelectedPoi()
        {
#if ANDROID || IOS || MACCATALYST
            if (VinhKhanhMap == null) return;
            var poiList = Poi.GetSampleData();
            Poi? targetPoi = null;
            
            if (_selectedPoiId > 0)
            {
                targetPoi = poiList.FirstOrDefault(p => p.Id == _selectedPoiId);
            }
            
            if (targetPoi == null && poiList.Any())
            {
                targetPoi = poiList[0];
            }
            
            if (targetPoi != null)
            {
                var centerLocation = new Location(targetPoi.Latitude, targetPoi.Longitude);
                var radius = _selectedPoiId > 0 ? Distance.FromKilometers(0.2) : Distance.FromKilometers(1);
                var mapSpan = MapSpan.FromCenterAndRadius(centerLocation, radius);
                
                try 
                {
                    VinhKhanhMap.MoveToRegion(mapSpan);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Lỗi zoom bản đồ: {ex.Message}");
                }
            }
#endif
        }
    }
}
