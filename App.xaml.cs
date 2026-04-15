namespace VinhKhanhTour
{
    public partial class App : Application
    {
        private readonly Services.ApiService _apiService;
        private IDispatcherTimer? _heartbeatTimer;

        public App(Services.ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());
            
            window.Created += (s, e) => StartHeartbeat();
            window.Resumed += (s, e) => StartHeartbeat();
            window.Stopped += (s, e) => StopHeartbeat();

            return window;
        }

        private void StartHeartbeat()
        {
            if (_heartbeatTimer == null)
            {
                _heartbeatTimer = Application.Current?.Dispatcher.CreateTimer();
                if (_heartbeatTimer != null)
                {
                    _heartbeatTimer.Interval = TimeSpan.FromSeconds(5);
                    _heartbeatTimer.Tick += async (s, e) => await DoHeartbeatAsync();
                }
            }
            
            if (_heartbeatTimer != null && !_heartbeatTimer.IsRunning)
            {
                _heartbeatTimer.Start();
            }
        }

        private void StopHeartbeat()
        {
            if (_heartbeatTimer?.IsRunning == true)
            {
                _heartbeatTimer.Stop();
            }
        }

        private async Task DoHeartbeatAsync()
        {
            bool isAlive = await _apiService.SendHeartbeatAsync();
            if (!isAlive)
            {
                // Nhận được 401 -> Tài khoản bị Admin khóa hoặc mất quyền -> Ép đăng xuất lập tức
                SecureStorage.Default.Remove("jwt_token");
                StopHeartbeat();
                
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Shell.Current.GoToAsync("//login");
                    if (App.Current?.MainPage != null)
                    {
                        await App.Current.MainPage.DisplayAlert("Phiên hết hạn", "Tài khoản của bạn đã bị khóa hoặc phiên đăng nhập đã hết. Vui lòng đăng nhập lại.", "Đã hiểu");
                    }
                });
            }
        }
    }
}