using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace VinhKhanhTour.Pages
{
    public partial class LoginPage : ContentPage
    {
        private bool _isPasswordVisible = false;

        public LoginPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            // Auto Login nếu đã có Token trong máy
            var token = await SecureStorage.Default.GetAsync("jwt_token");
            if (!string.IsNullOrEmpty(token))
            {
                await Shell.Current.GoToAsync("//main");
            }
        }

        private void OnTogglePasswordVisibility(object sender, EventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;
            PasswordEntry.IsPassword = !_isPasswordVisible;
            EyeIcon.Text = _isPasswordVisible ? "🙈" : "👁️";
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            // 💡 Lời khuyên 1: Chặn Double-Click
            if (LoginBtn.IsEnabled == false) return;

            string email = EmailEntry.Text?.Trim() ?? "";
            string password = PasswordEntry.Text ?? "";

            // 💡 Lời khuyên 2: Validation
            if (string.IsNullOrEmpty(email))
            {
                await DisplayAlert("Lỗi", "Vui lòng nhập email.", "OK");
                return;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                await DisplayAlert("Lỗi", "Định dạng email không hợp lệ.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Lỗi", "Vui lòng nhập mật khẩu.", "OK");
                return;
            }

            // Bắt đầu trạng thái Loading
            LoginBtn.IsEnabled = false;
            LoginBtn.Text = "Đang xử lý...";
            LoadingSpinner.IsVisible = true;
            LoadingSpinner.IsRunning = true;

            try
            {
                // 💡 Lời khuyên 3: Gọi API Backend
                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("https://vinhkhanhtour.onrender.com");
                
                // Giả lập delay mạng một chút để thấy spinner
                await Task.Delay(500);

                var loginData = new { email = email, password = password };
                var response = await httpClient.PostAsJsonAsync("/api/auth/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    // Lấy token từ kết quả
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (result != null && !string.IsNullOrEmpty(result.Token))
                    {
                        await SecureStorage.Default.SetAsync("jwt_token", result.Token);
                        
                        // Chuyển sang App chính bằng Route tuyệt đối
                        await Shell.Current.GoToAsync("//main");
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await DisplayAlert("Thất bại", "Sai tài khoản hoặc mật khẩu.", "Thử lại");
                }
                else
                {
                    await DisplayAlert("Lỗi", "Hệ thống đang bận. Vui lòng thử lại sau.", "Đóng");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Lỗi kết nối", "Không thể kết nối đến máy chủ. Vui lòng kiểm tra mạng.", "OK");
                System.Diagnostics.Debug.WriteLine($"Login Error: {ex.Message}");
            }
            finally
            {
                // Kết thúc trạng thái Loading
                LoginBtn.IsEnabled = true;
                LoginBtn.Text = "Đăng nhập";
                LoadingSpinner.IsVisible = false;
                LoadingSpinner.IsRunning = false;
            }
        }

        private async void OnForgotPasswordTapped(object sender, EventArgs e)
        {
            await DisplayAlert("Thông báo", "Chức năng khôi phục mật khẩu đang được phát triển.", "OK");
        }

        private async void OnSignUpTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("register");
        }
        
        // Mô hình dữ liệu trả về từ API
        class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}
