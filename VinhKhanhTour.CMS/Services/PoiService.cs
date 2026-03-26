using VinhKhanhTour.Shared.Data;
using VinhKhanhTour.Shared.Models;

namespace VinhKhanhTour.CMS.Services
{
    public class PoiService
    {
        private readonly PoiRepository _repo;

        public PoiService(PoiRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Poi>> GetPoisAsync()
        {
            return await _repo.GetAllPoisAsync();
        }

        public async Task<Poi?> GetPoiAsync(int id)
        {
            var pois = await GetPoisAsync();
            return pois.FirstOrDefault(p => p.Id == id);
        }

        public async Task CreatePoiAsync(Poi poi)
        {
            await _repo.SavePoiAsync(poi);
        }

        public async Task UpdatePoiAsync(Poi poi)
        {
            await _repo.SavePoiAsync(poi);
        }

        public async Task DeletePoiAsync(int id)
        {
            var pois = await GetPoisAsync();
            var poi = pois.FirstOrDefault(p => p.Id == id);
            if (poi != null)
            {
                await _repo.DeletePoiAsync(poi);
            }
        }
    }
}
