namespace VinhKhanhTour.Shared.Services
{
    public interface ILocalizationService
    {
        string GetString(string key);
        string CurrentLanguageCode { get; }
    }
}
