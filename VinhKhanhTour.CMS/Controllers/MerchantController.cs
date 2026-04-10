using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinhKhanhTour.CMS.Data;
using VinhKhanhTour.Shared.Models;

namespace VinhKhanhTour.CMS.Controllers
{
    [ApiController]
    [Route("api/merchant")]
    public class MerchantController : ControllerBase
    {
        private readonly CmsDbContext _dbContext;

        public MerchantController(CmsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterMerchant([FromBody] MerchantRegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if user with this phone number already exists
            var existingUser = await _dbContext.AppUsers
                .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber || u.Username == request.PhoneNumber);
            
            if (existingUser != null)
            {
                return BadRequest(new { Message = "Số điện thoại đã được đăng ký." });
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // Generate a new AgencyId simply by finding max existing + 1
                // Assuming AgencyId starts from 1
                var maxAgencyIdUser = await _dbContext.AppUsers.MaxAsync(u => (int?)u.AgencyId) ?? 0;
                var maxAgencyIdPoi = await _dbContext.Pois.IgnoreQueryFilters().MaxAsync(p => (int?)p.AgencyId) ?? 0;
                var maxAgencyIdTour= await _dbContext.Tours.IgnoreQueryFilters().MaxAsync(t => (int?)t.AgencyId) ?? 0;
                var maxAgencyIdHistory= await _dbContext.UsageHistories.IgnoreQueryFilters().MaxAsync(t => (int?)t.AgencyId) ?? 0;
                
                int newAgencyId = Math.Max(maxAgencyIdUser, Math.Max(maxAgencyIdPoi, Math.Max(maxAgencyIdTour, maxAgencyIdHistory))) + 1;

                // Create POI
                var newPoi = new Poi
                {
                    Name = request.PoiName,
                    AgencyId = newAgencyId,
                    Description = "Thông tin đang cập nhật",
                    Latitude = 10.76, // Default default center
                    Longitude = 106.70 
                };
                
                _dbContext.Pois.Add(newPoi);
                // Can't save changes yet if we want atomicity for all

                // Hash password "123" with BCrypt
                string passwordHash = BCrypt.Net.BCrypt.HashPassword("123");

                // Create AppUser
                var newUser = new AppUser
                {
                    Username = request.PhoneNumber,
                    PasswordHash = passwordHash,
                    PhoneNumber = request.PhoneNumber,
                    FullName = request.FullName,
                    Email = request.Email,
                    Address = request.Address,
                    AgencyId = newAgencyId,
                    Role = "owner",
                    ForcePasswordChange = true,
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.AppUsers.Add(newUser);

                // Create Audit Log
                var auditLog = new AppAuditLog
                {
                    Timestamp = DateTime.UtcNow,
                    Action = "register_poi",
                    Details = "500000",
                    UserId = newUser.Username
                };

                _dbContext.AppAuditLogs.Add(auditLog);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { Message = "Đăng ký thành công", Username = newUser.Username, AgencyId = newAgencyId });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { Message = "Đã có lỗi xảy ra. Hãy thử lại.", Error = ex.Message });
            }
        }
    }
}
