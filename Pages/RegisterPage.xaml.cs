using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace VinhKhanhTour.Pages
{
    public partial class RegisterPage : ContentPage
    {
        private bool _isPasswordVisible = false;
        private bool _isConfirmPasswordVisible = false;

        public RegisterPage()
        {
            InitializeComponent();
        }

        private void OnTogglePasswordVisibility(object sender, EventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;
            PasswordEntry.IsPassword = !_isPasswordVisible;
            EyeIcon.Text = _isPasswordVisible ? "🙈" : "👁️";
        }

        private void OnToggleConfirmPasswordVisibility(object sender, EventArgs e)
        {
            _isConfirmPasswordVisible = !_isConfirmPasswordVisible;
            ConfirmPasswordEntry.IsPassword = !_isConfirmPasswordVisible;
            ConfirmEyeIcon.Text = _isConfirmPasswordVisible ? "🙈" : "👁️";
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            if (RegisterBtn.IsEnabled == false) return;

            string fullName = FullNameEntry.Text?.Trim() ?? "";
            string email = EmailEntry.Text?.Trim() ?? "";
            string password = PasswordEntry.Text ?? "";
            string confirmPassword = ConfirmPasswordEntry.Text ?? "";

            // Validation
            if (string.IsNullOrEmpty(fullName))
            {
                await DisplayAlert("Lỗi", "Vui lòng nhập họ và tên.", "OK");
                return;
            }

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

            if (password.Length < 6)
            {
                await DisplayAlert("Lỗi", "Mật khẩu phải có ít nhất 6 ký tự.", "OK");
                return;
            }

            if (password != confirmPassword)
            {
                await DisplayAlert("Lỗi", "Mật khẩu xác nhận không khớp.", "OK");
                return;
            }

            // Start Loading
            RegisterBtn.IsEnabled = false;
            RegisterBtn.Text = "Đang xử lý...";
            LoadingSpinner.IsVisible = true;
            LoadingSpinner.IsRunning = true;

            try
            {
                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("https://vinhkhanhtour.onrender.com");

                var registerData = new 
                { 
                    fullName = fullName,
                    email = email, 
                    password = password 
                };

                var response = await httpClient.PostAsJsonAsync("/api/auth/register", registerData);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<RegisterResponse>();
                    if (result != null && !string.IsNullOrEmpty(result.Token))
                    {
                        await SecureStorage.Default.SetAsync("jwt_token", result.Token);
                        
                        await DisplayAlert("Thành công", "Tài khoản của bạn đã được tạo thành công!", "Bắt đầu khám phá");
                        
                        // Chuyển hướng vào App chính
                        await Shell.Current.GoToAsync("//main");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    // Thử parse json error nếu có
                    await DisplayAlert("Thất bại", "Đăng ký không thành công. Email có thể đã tồn tại.", "Thử lại");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Lỗi kết nối", "Không thể kết nối đến máy chủ. Vui lòng kiểm tra mạng.", "OK");
                System.Diagnostics.Debug.WriteLine($"Register Error: {ex.Message}");
            }
            finally
            {
                RegisterBtn.IsEnabled = true;
                RegisterBtn.Text = "Tạo tài khoản";
                LoadingSpinner.IsVisible = false;
                LoadingSpinner.IsRunning = false;
            }
        }

        private async void OnBackToLoginTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        class RegisterResponse
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}
