using Microsoft.EntityFrameworkCore;
using VinhKhanhTour.CMS.Data;
using VinhKhanhTour.Shared.Models;

namespace VinhKhanhTour.CMS.Services
{
    public class TourService
    {
        private readonly CmsDbContext _context;

        public TourService(CmsDbContext context)
        {
            _context = context;
        }

        // GET: Lấy tất cả Tour (kèm danh sách Stops)
        public async Task<List<TourDetailDto>> GetAllToursAsync()
        {
            var tours = await _context.Tours
                .Include(t => t.Stops.OrderBy(s => s.OrderIndex))
                .OrderBy(t => t.Id)
                .ToListAsync();

            return tours.Select(MapToDetailDto).ToList();
        }

        // GET: Lấy chi tiết 1 Tour (kèm danh sách Stops sắp xếp theo OrderIndex)
        public async Task<TourDetailDto?> GetTourByIdAsync(int id)
        {
            var tour = await _context.Tours
                .Include(t => t.Stops.OrderBy(s => s.OrderIndex))
                .FirstOrDefaultAsync(t => t.Id == id);

            return tour == null ? null : MapToDetailDto(tour);
        }

        // POST: Tạo mới Tour + Stops trong 1 Transaction
        public async Task<TourDetailDto> CreateTourAsync(TourCreateUpdateDto dto)
        {
            // Sử dụng Transaction để đảm bảo tính toàn vẹn dữ liệu
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Bước 1: Tạo Tour trước để lấy Id
                var tour = new Tour
                {
                    TourName = dto.TourName,
                    Description = dto.Description,
                    ImageUrl = dto.ImageUrl,
                    StartPoint = dto.StartPoint,
                    EndPoint = dto.EndPoint,
                    TotalDistance = dto.TotalDistance
                };

                _context.Tours.Add(tour);
                await _context.SaveChangesAsync(); // tour.Id đã được PostgreSQL sinh ra

                // Bước 2: Gán TourId cho từng Stop rồi lưu
                if (dto.Stops != null && dto.Stops.Any())
                {
                    var stops = dto.Stops.Select(s => new TourStop
                    {
                        TourId = tour.Id, // Gán khóa ngoại
                        PoiId = s.PoiId,
                        PoiName = s.PoiName,
                        Description = s.Description,
                        Latitude = s.Latitude,
                        Longitude = s.Longitude,
                        OrderIndex = s.OrderIndex
                    }).ToList();

                    _context.TourStops.AddRange(stops);
                    await _context.SaveChangesAsync();
                    tour.Stops = stops;
                }

                // Commit Transaction nếu thành công
                await transaction.CommitAsync();
                return MapToDetailDto(tour);
            }
            catch
            {
                // Rollback toàn bộ nếu có lỗi
                await transaction.RollbackAsync();
                throw;
            }
        }

        // PUT: Cập nhật Tour + Stops trong 1 Transaction
        public async Task<TourDetailDto?> UpdateTourAsync(int id, TourCreateUpdateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var tour = await _context.Tours
                    .Include(t => t.Stops)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (tour == null) return null;

                // Bước 1: Cập nhật thông tin Tour
                tour.TourName = dto.TourName;
                tour.Description = dto.Description;
                tour.ImageUrl = dto.ImageUrl;
                tour.StartPoint = dto.StartPoint;
                tour.EndPoint = dto.EndPoint;
                tour.TotalDistance = dto.TotalDistance;

                // Bước 2: Xóa toàn bộ Stops cũ
                _context.TourStops.RemoveRange(tour.Stops);

                // Bước 3: Thêm danh sách Stops mới
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

                    _context.TourStops.AddRange(newStops);
                    tour.Stops = newStops;
                }

                await _context.SaveChangesAsync();
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
            var tour = await _context.Tours.FindAsync(id);
            if (tour == null) return false;

            _context.Tours.Remove(tour);
            await _context.SaveChangesAsync();
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
