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
        private bool _isDataLoaded = false;

        [ObservableProperty]
        private List<Poi> _pois = [];

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
                Pois = await _poiRepository.GetAllPoisAsync();
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
        private async Task NavigateToMapAsync()
        {
            await Shell.Current.GoToAsync("map");
        }
    }
}