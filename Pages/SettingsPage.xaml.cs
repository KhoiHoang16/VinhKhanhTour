namespace VinhKhanhTour.Pages;

public partial class SettingsPage : ContentPage
{
    private bool _isInitializing = true;
    private readonly Services.ApiService _apiService;

    public SettingsPage(Services.ApiService apiService)
    {
        _apiService = apiService;
        InitializeComponent();
        
        // Khởi tạo trạng thái ban đầu của Switch dựa trên Theme hiện tại
        ThemeSwitch.IsToggled = Application.Current?.RequestedTheme == AppTheme.Dark;
        
        // Khởi tạo ngôn ngữ LanguagePicker
        var currentLang = Services.LocalizationResourceManager.Instance.CurrentLanguageCode;
        LanguagePicker.SelectedIndex = currentLang switch
        {
            "en" => 1,
            "es" => 2,
            "fr" => 3,
            "de" => 4,
            "zh" => 5,
            "ja" => 6,
            "ko" => 7,
            "ru" => 8,
            "it" => 9,
            "pt" => 10,
            _ => 0
        };

        _isInitializing = false;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUserProfile();
    }

    private async Task LoadUserProfile()
    {
        try
        {
            var profile = await _apiService.GetTouristProfileAsync();
            if (profile != null)
            {
                UserNameLabel.Text = profile.FullName ?? "Người dùng";
                UserEmailLabel.Text = profile.Email;
            }
            else
            {
                UserNameLabel.Text = "Khách tham quan";
                UserEmailLabel.Text = "Chưa đăng nhập";
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading profile: {ex.Message}");
            UserNameLabel.Text = "Lỗi tải thông tin";
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Xác nhận", "Bạn có chắc chắn muốn đăng xuất không?", "Đăng xuất", "Hủy");
        if (confirm)
        {
            SecureStorage.Default.Remove("jwt_token");
            
            // Quay lại màn hình đăng nhập (Xóa stack điều hướng)
            await Shell.Current.GoToAsync("//login");
        }
    }

    private void OnLanguageChanged(object sender, EventArgs e)
    {
        if (_isInitializing) return;
        if (LanguagePicker.SelectedIndex == -1) return;
        
        string selectedLang = LanguagePicker.SelectedIndex switch
        {
            1 => "en",
            2 => "es",
            3 => "fr",
            4 => "de",
            5 => "zh",
            6 => "ja",
            7 => "ko",
            8 => "ru",
            9 => "it",
            10 => "pt",
            _ => "vi"
        };

        string cultureString = selectedLang switch
        {
            "en" => "en-US",
            "es" => "es-ES",
            "fr" => "fr-FR",
            "de" => "de-DE",
            "zh" => "zh-CN",
            "ja" => "ja-JP",
            "ko" => "ko-KR",
            "ru" => "ru-RU",
            "it" => "it-IT",
            "pt" => "pt-PT",
            _ => "vi-VN"
        };
        
        // Lưu cài đặt vào Preferences
        Preferences.Default.Set("AppLanguage", selectedLang);
        
        // Chỉnh sửa runtime
        Services.LocalizationResourceManager.Instance.SetCulture(new System.Globalization.CultureInfo(cultureString));

        // Buộc tải lại toàn bộ Ứng dụng để Áp dụng Ngôn ngữ mới trọn vẹn
        if (Application.Current != null)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Application.Current.MainPage = new AppShell();
            });
        }
    }

    private void OnThemeSwitchToggled(object sender, ToggledEventArgs e)
    {
        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
        }
    }

    private void OnSpeechRateChanged(object sender, ValueChangedEventArgs e)
    {
        UpdateSpeechRateLabel(e.NewValue);
    }

    private void UpdateSpeechRateLabel(double rawValue)
    {
        double rate = Math.Round(rawValue, 1);
        string prefix = Services.LocalizationResourceManager.Instance["Tốc độ"];
        string normalStr = Services.LocalizationResourceManager.Instance["Bình thường"];
        SpeechRateLabel.Text = rate == 1.0 ? $"{normalStr} (1.0x)" : $"{prefix}: {rate:F1}x";
    }
}
