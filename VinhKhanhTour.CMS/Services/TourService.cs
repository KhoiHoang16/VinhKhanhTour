using VinhKhanhTour.Shared.Data;
using VinhKhanhTour.Shared.Models;

namespace VinhKhanhTour.CMS.Services
{
    public class TourService
    {
        private readonly PoiRepository _repo;

        public TourService(PoiRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Tour>> GetToursAsync() => await _repo.GetAllToursAsync();
        
        public async Task<Tour?> GetTourAsync(int id)
        {
            var tours = await GetToursAsync();
            return tours.FirstOrDefault(t => t.Id == id);
        }

        public async Task SaveTourAsync(Tour tour) => await _repo.SaveTourAsync(tour);
        
        public async Task DeleteTourAsync(int id)
        {
            var tour = await GetTourAsync(id);
            if (tour != null) await _repo.DeleteTourAsync(tour);
        }
    }
}
