using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VinhKhanhTour.Data;
using VinhKhanhTour.Models;

namespace VinhKhanhTour.PageModels
{
    public partial class MainPageModel : ObservableObject
    {
        private readonly PoiRepository _poiRepository;
        private readonly Services.IErrorHandler _errorHandler;
        private List<Poi> _allPois = [];
        private bool _isDataLoaded = false;

        [ObservableProperty]
        private List<Poi> _pois = [];

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isRefreshing;

        public MainPageModel(PoiRepository poiRepository, Services.IErrorHandler errorHandler)
        {
            _poiRepository = poiRepository;
            _errorHandler = errorHandler;
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
            // Placeholder logic for category filtering
            // For now, let's just use it to show we can filter
            ApplyFilters(category);
        }

        private void ApplyFilters(string category = "Tất cả")
        {
            var filtered = _allPois.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(p => p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || 
                                               p.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            if (category != "Tất cả")
            {
                // In a real app, POI would have a Category property. 
                // For this demo, we'll simulate it by checking keywords in description.
                if (category == "Ốc & Hải sản")
                    filtered = filtered.Where(p => p.Name.Contains("Ốc") || p.Description.Contains("Ốc") || p.Description.Contains("Hải sản"));
                else if (category == "Đồ nướng")
                    filtered = filtered.Where(p => p.Description.Contains("nướng"));
                else if (category == "Món nước")
                    filtered = filtered.Where(p => p.Description.Contains("bún") || p.Description.Contains("lẩu"));
                else if (category == "Sushi")
                    filtered = filtered.Where(p => p.Name.Contains("Sushi"));
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
        }

        [RelayCommand]
        private async Task StartTourAsync()
        {
            await Shell.Current.GoToAsync("//map");
        }

        [RelayCommand]
        private async Task GoToPoiAsync(Poi poi)
        {
            if (poi != null)
            {
                await Shell.Current.GoToAsync($"//map?poiId={poi.Id}");
            }
        }
    }
}