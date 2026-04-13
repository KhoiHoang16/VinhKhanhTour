using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace VinhKhanhTour.Services
{
    /// <summary>
    /// Tạo và lưu trữ Device ID duy nhất bằng SecureStorage (encrypted).
    /// - Bền qua app updates
    /// - Mất khi uninstall (iOS) hoặc clear app data (Android)
    /// - Tuân thủ Apple/Google privacy policy (không dùng IMEI/MAC)
    /// </summary>
    public static class DeviceIdentityService
    {
        private const string DeviceIdKey = "vkt_device_id";

        /// <summary>
        /// Lấy hoặc tạo DeviceId duy nhất.
        /// Lưu trong SecureStorage (Keychain trên iOS, EncryptedSharedPreferences trên Android).
        /// </summary>
        public static async Task<string> GetOrCreateDeviceIdAsync()
        {
            try
            {
                var existing = await SecureStorage.Default.GetAsync(DeviceIdKey);
                if (!string.IsNullOrEmpty(existing))
                {
                    Debug.WriteLine($"[DeviceIdentity] Đã có DeviceId: {existing[..12]}...");
                    return existing;
                }

                // Tạo GUID mới = Device ID duy nhất
                var newId = $"VKT-{Guid.NewGuid():N}";
                await SecureStorage.Default.SetAsync(DeviceIdKey, newId);
                Debug.WriteLine($"[DeviceIdentity] Tạo DeviceId mới: {newId[..12]}...");
                return newId;
            }
            catch (Exception ex)
            {
                // Fallback cho emulator, rooted device, hoặc thiết bị không hỗ trợ SecureStorage
                Debug.WriteLine($"[DeviceIdentity] SecureStorage lỗi: {ex.Message}. Dùng fallback.");
                var fallback = $"VKT-FB-{DeviceInfo.Current.Platform}-{Guid.NewGuid().ToString()[..8]}";
                return fallback;
            }
        }
    }
}
