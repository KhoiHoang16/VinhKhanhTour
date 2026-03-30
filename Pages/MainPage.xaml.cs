using System;
using System.Linq;
using Microsoft.Maui.Controls;
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is MainPageModel model)
            {
                // Fire-and-forget to prevent blocking the UI thread (ANR)
                _ = model.AppearingCommand.ExecuteAsync(null);
            }
        }
    }
}