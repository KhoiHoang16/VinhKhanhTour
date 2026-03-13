namespace VinhKhanhTour.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        
        // Khởi tạo trạng thái ban đầu của Switch dựa trên Theme hiện tại
        ThemeSwitch.IsToggled = Application.Current?.RequestedTheme == AppTheme.Dark;
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
        double rate = Math.Round(e.NewValue, 1);
        SpeechRateLabel.Text = $"Tốc độ: {rate:F1}x";
        
        // TODO: Lưu cài đặt này vào Preferences để sử dụng trong Audio/TTS Service
        // Preferences.Default.Set("TtsSpeechRate", rate);
    }
}
