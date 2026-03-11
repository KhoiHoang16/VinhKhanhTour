using System;
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
    }
}