using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VinhKhanhTour.Data;
using VinhKhanhTour.Models;

namespace VinhKhanhTour.PageModels
{
    public partial class MainPageModel : ObservableObject
    {
        private readonly PoiRepository _poiRepository;
        private readonly IErrorHandler _errorHandler;
        private readonly Services.NarrationEngine _narrationEngine;
        private List<Poi> _allPois = [];
        private bool _isDataLoaded = false;

        [ObservableProperty]
        private List<Poi> _pois = [];

        [ObservableProperty]
        private string _searchText = string.Empty;

        private CancellationTokenSource? _searchCts;
        partial void OnSearchTextChanged(string value)
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            
            var token = _searchCts.Token;
            Task.Delay(300, token).ContinueWith(t => 
            {
                if (!t.IsCanceled)
                {
                    MainThread.BeginInvokeOnMainThread(() => ApplyFilters());
                }
            }, TaskScheduler.Default);
        }

        [ObservableProperty]
        private string _selectedCategory = "All";

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isRefreshing;

        public MainPageModel(PoiRepository poiRepository, IErrorHandler errorHandler, Services.NarrationEngine narrationEngine)
        {
            _poiRepository = poiRepository;
            _errorHandler = errorHandler;
            _narrationEngine = narrationEngine;

            Services.LocalizationResourceManager.Instance.PropertyChanged += (s, e) => 
            {
                ApplyFilters(); // Refresh currently displayed items
            };

            // Lắng nghe GPS liên tục
            Geolocation.Default.LocationChanged += OnLocationChanged;
        }

        private void OnLocationChanged(object? sender, GeolocationLocationChangedEventArgs e)
        {
            var userLocation = e.Location;
            if (userLocation != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    foreach (var poi in _allPois)
                    {
                        poi.DistanceToUser = Utilities.LocationHelper.CalculateDistanceInMeters(
                            userLocation.Latitude, userLocation.Longitude,
                            poi.Latitude, poi.Longitude);
                    }
                });
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                IsBusy = true;
                _allPois = await _poiRepository.GetAllPoisAsync();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void Search(string query)
        {
            SearchText = query;
            ApplyFilters();
        }

        [RelayCommand]
        private void Filter(string category)
        {
            SelectedCategory = category;
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var filtered = _allPois.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(p => p.DisplayName.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase) || 
                                               p.DisplayDescription.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase));
            }

            if (SelectedCategory != "All")
            {
                // In a real app, POI would have a Category property. 
                // For this demo, we'll simulate it by checking keywords in description.
                if (SelectedCategory == "Seafood")
                    filtered = filtered.Where(p => p.DisplayName.Contains("Ốc", StringComparison.InvariantCultureIgnoreCase) || 
                                                   p.DisplayName.Contains("ốc", StringComparison.InvariantCultureIgnoreCase) || 
                                                   p.DisplayDescription.Contains("Ốc", StringComparison.InvariantCultureIgnoreCase) || 
                                                   p.DisplayDescription.Contains("ốc", StringComparison.InvariantCultureIgnoreCase) || 
                                                   p.DisplayDescription.Contains("Hải sản", StringComparison.InvariantCultureIgnoreCase) ||
                                                   p.DisplayDescription.Contains("hải sản", StringComparison.InvariantCultureIgnoreCase) ||
                                                   p.DisplayName.Contains("Snail", StringComparison.InvariantCultureIgnoreCase) ||
                                                   p.DisplayDescription.Contains("Seafood", StringComparison.InvariantCultureIgnoreCase));
                else if (SelectedCategory == "BBQ")
                    filtered = filtered.Where(p => p.DisplayDescription.Contains("nướng", StringComparison.InvariantCultureIgnoreCase) || 
                                                   p.DisplayName.Contains("nướng", StringComparison.InvariantCultureIgnoreCase) || 
                                                   p.DisplayDescription.Contains("grilled", StringComparison.InvariantCultureIgnoreCase) ||
                                                   p.DisplayDescription.Contains("BBQ", StringComparison.InvariantCultureIgnoreCase));
                else if (SelectedCategory == "Noodle")
                    filtered = filtered.Where(p => p.DisplayName.Contains("bún", StringComparison.InvariantCultureIgnoreCase) || 
                                                   p.DisplayDescription.Contains("bún", StringComparison.InvariantCultureIgnoreCase) || 
                                                   p.DisplayName.Contains("lẩu", StringComparison.InvariantCultureIgnoreCase) || 
                                                   p.DisplayDescription.Contains("lẩu", StringComparison.InvariantCultureIgnoreCase) ||
                                                   p.DisplayName.Contains("nước", StringComparison.InvariantCultureIgnoreCase) || 
                                                   p.DisplayDescription.Contains("nước", StringComparison.InvariantCultureIgnoreCase) || 
                                                   p.DisplayDescription.Contains("noodle", StringComparison.InvariantCultureIgnoreCase) ||
                                                   p.DisplayDescription.Contains("hotpot", StringComparison.InvariantCultureIgnoreCase));
                else if (SelectedCategory == "Sushi")
                    filtered = filtered.Where(p => p.DisplayName.Contains("Sushi", StringComparison.InvariantCultureIgnoreCase) ||
                                                   p.DisplayDescription.Contains("Sushi", StringComparison.InvariantCultureIgnoreCase));
            }

            Pois = filtered.ToList();
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            try
            {
                IsRefreshing = true;
                await LoadDataAsync();
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task AppearingAsync()
        {
            if (!_isDataLoaded)
            {
                await RefreshAsync();
                _isDataLoaded = true;
            }

            // Bắt đầu hoặc tiếp tục theo dõi vị trí khi trang hiển thị
            try
            {
                if (!Geolocation.Default.IsListeningForeground)
                {
                    var request = new GeolocationListeningRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(5));
                    await Geolocation.Default.StartListeningForegroundAsync(request);
                }
            }
            catch (Exception ex)
            {
                _errorHandler.HandleError(ex);
            }
        }

        [RelayCommand]
        private async Task StartTourAsync()
        {
            await Shell.Current.GoToAsync("//map");
        }

        [ObservableProperty]
        private Poi? _selectedPoi;

        [ObservableProperty]
        private bool _isPoiDetailVisible;

        [RelayCommand]
        private void GoToPoi(Poi poi)
        {
            if (poi != null)
            {
                SelectedPoi = poi;
                IsPoiDetailVisible = true;
            }
        }

        [RelayCommand]
        private void ClosePoiDetail()
        {
            IsPoiDetailVisible = false;
        }

        [RelayCommand]
        private async Task NavigateToMapAsync()
        {
            if (SelectedPoi != null)
            {
                IsPoiDetailVisible = false;
                await Shell.Current.GoToAsync($"//map?poiId={SelectedPoi.Id}");
            }
        }

        [RelayCommand]
        private async Task PlayNarrationAsync()
        {
            if (SelectedPoi != null)
            {
                await _narrationEngine.PlayPoiNarrationAsync(SelectedPoi, isManual: true);
            }
        }
    }
}