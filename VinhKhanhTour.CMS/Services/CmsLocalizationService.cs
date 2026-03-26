using VinhKhanhTour.Shared.Services;

namespace VinhKhanhTour.CMS.Services
{
    public class CmsLocalizationService : ILocalizationService
    {
        public string CurrentLanguageCode => "vi";

        public string GetString(string key)
        {
            return key;
        }
    }
}
