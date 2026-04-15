namespace VinhKhanhTour.CMS.Services
{
    /// <summary>
    /// DTO chứa thông tin user sau khi verify Firebase Token thành công
    /// </summary>
    public class FirebaseUserInfo
    {
        public string Uid { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? PhotoUrl { get; set; }
        public string Provider { get; set; } = "google";
    }

    /// <summary>
    /// Interface cho dịch vụ xác thực Firebase.
    /// Tách interface để dễ mock/test và chuyển sang Firebase Admin SDK thật khi sẵn sàng.
    /// </summary>
    public interface IFirebaseAuthService
    {
        /// <summary>
        /// Xác thực Firebase ID Token và trích xuất thông tin người dùng.
        /// </summary>
        /// <param name="idToken">Firebase ID Token từ Mobile App</param>
        /// <returns>Thông tin user nếu token hợp lệ, null nếu không</returns>
        Task<FirebaseUserInfo?> VerifyTokenAsync(string idToken);
    }
}
