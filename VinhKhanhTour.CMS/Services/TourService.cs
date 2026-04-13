using Microsoft.EntityFrameworkCore;
using VinhKhanhTour.CMS.Data;
using VinhKhanhTour.Shared.Models;

namespace VinhKhanhTour.CMS.Services
{
    public class TourService
    {
        private readonly IDbContextFactory<CmsDbContext> _factory;

        public TourService(IDbContextFactory<CmsDbContext> factory)
        {
            _factory = factory;
        }

        // GET: Lấy tất cả Tour (kèm danh sách Stops)
        public async Task<List<TourDetailDto>> GetAllToursAsync()
        {
            using var context = await _factory.CreateDbContextAsync();
            var tours = await context.Tours
                .Include(t => t.Stops.OrderBy(s => s.OrderIndex))
                .OrderBy(t => t.Id)
                .ToListAsync();

            return tours.Select(MapToDetailDto).ToList();
        }

        // GET: Lấy chi tiết 1 Tour (kèm danh sách Stops sắp xếp theo OrderIndex)
        public async Task<TourDetailDto?> GetTourByIdAsync(int id)
        {
            using var context = await _factory.CreateDbContextAsync();
            var tour = await context.Tours
                .Include(t => t.Stops.OrderBy(s => s.OrderIndex))
                .FirstOrDefaultAsync(t => t.Id == id);

            return tour == null ? null : MapToDetailDto(tour);
        }

        // POST: Tạo mới Tour + Stops trong 1 Transaction
        public async Task<TourDetailDto> CreateTourAsync(TourCreateUpdateDto dto)
        {
            using var context = await _factory.CreateDbContextAsync();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var tour = new Tour
                {
                    TourName = dto.TourName,
                    Description = dto.Description,
                    ImageUrl = dto.ImageUrl,
                    StartPoint = dto.StartPoint,
                    EndPoint = dto.EndPoint,
                    TotalDistance = dto.TotalDistance
                };

                context.Tours.Add(tour);
                await context.SaveChangesAsync();

                if (dto.Stops != null && dto.Stops.Any())
                {
                    var stops = dto.Stops.Select(s => new TourStop
                    {
                        TourId = tour.Id,
                        PoiId = s.PoiId,
                        PoiName = s.PoiName,
                        Description = s.Description,
                        Latitude = s.Latitude,
                        Longitude = s.Longitude,
                        OrderIndex = s.OrderIndex
                    }).ToList();

                    context.TourStops.AddRange(stops);
                    await context.SaveChangesAsync();
                    tour.Stops = stops;
                }

                await transaction.CommitAsync();
                return MapToDetailDto(tour);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // PUT: Cập nhật Tour + Stops trong 1 Transaction
        public async Task<TourDetailDto?> UpdateTourAsync(int id, TourCreateUpdateDto dto)
        {
            using var context = await _factory.CreateDbContextAsync();
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var tour = await context.Tours
                    .Include(t => t.Stops)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (tour == null) return null;

                tour.TourName = dto.TourName;
                tour.Description = dto.Description;
                tour.ImageUrl = dto.ImageUrl;
                tour.StartPoint = dto.StartPoint;
                tour.EndPoint = dto.EndPoint;
                tour.TotalDistance = dto.TotalDistance;

                context.TourStops.RemoveRange(tour.Stops);

                if (dto.Stops != null && dto.Stops.Any())
                {
                    var newStops = dto.Stops.Select(s => new TourStop
                    {
                        TourId = tour.Id,
                        PoiId = s.PoiId,
                        PoiName = s.PoiName,
                        Description = s.Description,
                        Latitude = s.Latitude,
                        Longitude = s.Longitude,
                        OrderIndex = s.OrderIndex
                    }).ToList();

                    context.TourStops.AddRange(newStops);
                    tour.Stops = newStops;
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return MapToDetailDto(tour);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // DELETE: Xóa Tour (Cascade sẽ tự xóa Stops)
        public async Task<bool> DeleteTourAsync(int id)
        {
            using var context = await _factory.CreateDbContextAsync();
            var tour = await context.Tours.FindAsync(id);
            if (tour == null) return false;

            context.Tours.Remove(tour);
            await context.SaveChangesAsync();
            return true;
        }

        // Helper: Map Entity -> DTO
        private static TourDetailDto MapToDetailDto(Tour tour)
        {
            return new TourDetailDto
            {
                Id = tour.Id,
                TourName = tour.TourName,
                Description = tour.Description,
                ImageUrl = tour.ImageUrl,
                StartPoint = tour.StartPoint,
                EndPoint = tour.EndPoint,
                TotalDistance = tour.TotalDistance,
                Stops = tour.Stops?.OrderBy(s => s.OrderIndex).Select(s => new TourStopDto
                {
                    PoiId = s.PoiId,
                    PoiName = s.PoiName,
                    Description = s.Description,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    OrderIndex = s.OrderIndex
                }).ToList() ?? new()
            };
        }
    }
}
