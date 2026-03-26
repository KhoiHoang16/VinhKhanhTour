using System.Net.Http.Json;
using VinhKhanhTour.Shared.Models;

namespace VinhKhanhTour.CMS.Services
{
    public class PoiService
    {
        private readonly HttpClient _http;

        public PoiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Poi>> GetPoisAsync()
        {
            return await _http.GetFromJsonAsync<List<Poi>>("api/poi") ?? new List<Poi>();
        }

        public async Task<Poi?> GetPoiAsync(int id)
        {
            var pois = await GetPoisAsync();
            return pois.FirstOrDefault(p => p.Id == id);
        }

        public async Task CreatePoiAsync(Poi poi)
        {
            await _http.PostAsJsonAsync("api/poi", poi);
        }

        public async Task UpdatePoiAsync(Poi poi)
        {
            await _http.PutAsJsonAsync($"api/poi/{poi.Id}", poi);
        }

        public async Task DeletePoiAsync(int id)
        {
            await _http.DeleteAsync($"api/poi/{id}");
        }
    }
}
