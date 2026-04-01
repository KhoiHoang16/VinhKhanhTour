using Microsoft.EntityFrameworkCore;
using VinhKhanhTour.Shared.Data;
using VinhKhanhTour.Shared.Models;

namespace VinhKhanhTour.CMS.Data
{
    public class PostgresPoiRepository : IPoiRepository
    {
        private readonly CmsDbContext _context;

        public PostgresPoiRepository(CmsDbContext context)
        {
            _context = context;
        }

        public async Task<List<Poi>> GetAllPoisAsync()
        {
            return await _context.Pois.ToListAsync();
        }

        public async Task<Poi?> GetPoiAsync(int id)
        {
            return await _context.Pois.FindAsync(id);
        }

        public async Task<int> SavePoiAsync(Poi poi)
        {
            if (poi.Id == 0)
            {
                _context.Pois.Add(poi);
            }
            else
            {
                _context.Pois.Update(poi);
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> AddPoiAsync(Poi poi)
        {
            _context.Pois.Add(poi);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdatePoiAsync(Poi poi)
        {
            _context.Pois.Update(poi);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeletePoiAsync(Poi poi)
        {
            _context.Pois.Remove(poi);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<Tour>> GetAllToursAsync()
        {
            return await _context.Tours.ToListAsync();
        }

        public async Task<int> SaveTourAsync(Tour tour)
        {
            if (tour.Id == 0)
            {
                _context.Tours.Add(tour);
            }
            else
            {
                _context.Tours.Update(tour);
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteTourAsync(Tour tour)
        {
            _context.Tours.Remove(tour);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<UsageHistory>> GetUsageHistoryAsync()
        {
            return await _context.UsageHistories.OrderByDescending(h => h.Timestamp).ToListAsync();
        }

        public async Task<int> RecordUsageAsync(UsageHistory history)
        {
            _context.UsageHistories.Add(history);
            return await _context.SaveChangesAsync();
        }
    }
}
