using Microsoft.Extensions.Logging;

namespace VinhKhanhTour.CMS.Services
{
    /// <summary>
    /// Mock implementation của IFirebaseAuthService cho môi trường Development/Testing.
    /// Khi có Firebase Admin SDK thật, tạo class FirebaseAuthService thay thế.
    /// 
    /// ⚠️ CHỈ DÙNG CHO DEV — KHÔNG BAO GIỜ DÙNG TRÊN PRODUCTION.
    /// </summary>
    public class MockFirebaseAuthService : IFirebaseAuthService
    {
        private readonly ILogger<MockFirebaseAuthService> _logger;

        public MockFirebaseAuthService(ILogger<MockFirebaseAuthService> logger)
        {
            _logger = logger;
        }

        public Task<FirebaseUserInfo?> VerifyTokenAsync(string idToken)
        {
            _logger.LogWarning("⚠️ [MockFirebase] Đang dùng Mock Firebase Auth — KHÔNG dùng Production!");

            if (string.IsNullOrWhiteSpace(idToken))
            {
                _logger.LogError("[MockFirebase] Token rỗng, trả về null.");
                return Task.FromResult<FirebaseUserInfo?>(null);
            }

            // Trong mock: mô phỏng decode token bằng cách dùng token value làm seed
            // Format mock token: "mock_<email>" hoặc bất kỳ chuỗi nào
            var mockEmail = idToken.StartsWith("mock_")
                ? idToken.Replace("mock_", "") + "@gmail.com"
                : "tourist@vinhkhanhtour.com";

            var result = new FirebaseUserInfo
            {
                Uid = $"firebase_uid_{idToken.GetHashCode():X8}",
                Email = mockEmail,
                DisplayName = "Tourist Test User",
                PhotoUrl = null,
                Provider = "google"
            };

            _logger.LogInformation("[MockFirebase] ✅ Verified mock token → UID={Uid}, Email={Email}",
                result.Uid, result.Email);

            return Task.FromResult<FirebaseUserInfo?>(result);
        }
    }

    // =========================================================================
    // 📦 KHI CÓ FIREBASE ADMIN SDK — BỎ COMMENT PHẦN NÀY VÀ ĐĂNG KÝ THAY THẾ
    // =========================================================================
    //
    // using FirebaseAdmin;
    // using FirebaseAdmin.Auth;
    // using Google.Apis.Auth.OAuth2;
    //
    // public class FirebaseAuthService : IFirebaseAuthService
    // {
    //     private readonly ILogger<FirebaseAuthService> _logger;
    //
    //     public FirebaseAuthService(ILogger<FirebaseAuthService> logger)
    //     {
    //         _logger = logger;
    //
    //         if (FirebaseApp.DefaultInstance == null)
    //         {
    //             // Cách 1: Từ file JSON
    //             // FirebaseApp.Create(new AppOptions
    //             // {
    //             //     Credential = GoogleCredential.FromFile("firebase-admin.json")
    //             // });
    //             
    //             // Cách 2: Từ biến môi trường GOOGLE_APPLICATION_CREDENTIALS
    //             FirebaseApp.Create(new AppOptions
    //             {
    //                 Credential = GoogleCredential.GetApplicationDefault()
    //             });
    //         }
    //     }
    //
    //     public async Task<FirebaseUserInfo?> VerifyTokenAsync(string idToken)
    //     {
    //         try
    //         {
    //             var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
    //             
    //             return new FirebaseUserInfo
    //             {
    //                 Uid = decoded.Uid,
    //                 Email = decoded.Claims.GetValueOrDefault("email")?.ToString() ?? "",
    //                 DisplayName = decoded.Claims.GetValueOrDefault("name")?.ToString(),
    //                 PhotoUrl = decoded.Claims.GetValueOrDefault("picture")?.ToString(),
    //                 Provider = decoded.Claims.GetValueOrDefault("firebase", new Dictionary<string, object>()) 
    //                     is Dictionary<string, object> fb && fb.ContainsKey("sign_in_provider")
    //                     ? fb["sign_in_provider"].ToString() ?? "google"
    //                     : "google"
    //             };
    //         }
    //         catch (Exception ex)
    //         {
    //             _logger.LogError(ex, "[Firebase] Token verification failed.");
    //             return null;
    //         }
    //     }
    // }
}
