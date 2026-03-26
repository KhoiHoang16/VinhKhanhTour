using VinhKhanhTour.Shared.Services;

namespace VinhKhanhTour.Services
{
    public class AppLocalizationService : ILocalizationService
    {
        public string GetString(string key)
        {
            return LocalizationResourceManager.Instance[key]?.ToString() ?? key;
        }

        public string CurrentLanguageCode => LocalizationResourceManager.Instance.CurrentLanguageCode;
    }
}
