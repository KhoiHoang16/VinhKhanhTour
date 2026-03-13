using System;
using System.Linq;
using Microsoft.Maui.Controls;
using VinhKhanhTour.Models;
using VinhKhanhTour.PageModels;

namespace VinhKhanhTour.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is MainPageModel model)
            {
                await model.AppearingCommand.ExecuteAsync(null);
            }
        }

        private void OnPoiSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Poi selectedPoi)
            {
                if (BindingContext is MainPageModel vm)
                {
                    vm.GoToPoiCommand.Execute(selectedPoi);
                }
                
                // Trả về null để khi quay lại bấm vẫn ăn
                if (sender is CollectionView cv)
                {
                    cv.SelectedItem = null;
                }
            }
        }
    }
}