using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinhKhanhTour.CMS.Data;
using VinhKhanhTour.CMS.Services;

namespace VinhKhanhTour.CMS.Controllers
{
    [Area("Admin")]
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : ControllerBase
    {
        private readonly CmsDbContext _context;
        private readonly IEmailService _emailService;

        public AdminUserController(CmsDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserManagementDto>>> GetUsers([FromQuery] string? search)
        {
            // 1. Lấy AppUsers (Admin/Agency)
            var appUsersQuery = _context.AppUsers.AsNoTracking().AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                appUsersQuery = appUsersQuery.Where(u => u.Username.Contains(search) || (u.Email != null && u.Email.Contains(search)) || (u.FullName != null && u.FullName.Contains(search)));
            }

            var appUsers = await appUsersQuery.Select(u => new UserManagementDto
            {
                Id = u.Id,
                Email = u.Email ?? "",
                DisplayName = u.Username,
                FullName = u.FullName,
                Role = u.Role,
                UserType = "AppUser",
                IsLocked = u.IsLocked,
                LastActiveAt = u.LastActiveAt,
                LastActiveIp = u.LastActiveIp,
                LastActiveDevice = u.LastActiveDevice,
                CreatedAt = u.CreatedAt
            }).ToListAsync();

            // 2. Lấy Tourists
            var touristsQuery = _context.Tourists.AsNoTracking().AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                touristsQuery = touristsQuery.Where(t => t.Email.Contains(search) || (t.FullName != null && t.FullName.Contains(search)));
            }

            var tourists = await touristsQuery.Select(t => new UserManagementDto
            {
                Id = t.Id,
                Email = t.Email,
                DisplayName = t.FullName ?? t.Email,
                FullName = t.FullName,
                Role = "Tourist",
                UserType = "Tourist",
                IsLocked = t.IsLocked,
                LastActiveAt = t.LastActiveAt,
                LastActiveIp = t.LastActiveIp,
                LastActiveDevice = t.LastActiveDevice,
                CreatedAt = t.CreatedAt
            }).ToListAsync();

            // Hợp nhất và trả về
            var result = appUsers.Concat(tourists)
                .OrderByDescending(u => u.LastActiveAt)
                .ThenByDescending(u => u.CreatedAt);

            return Ok(result);
        }

        [HttpPost("{userType}/{id}/toggle-lock")]
        public async Task<IActionResult> ToggleLock(string userType, int id)
        {
            string userEmail = "";
            string userName = "";
            bool isLocking = false;

            if (userType == "Tourist")
            {
                var tourist = await _context.Tourists.FindAsync(id);
                if (tourist == null) return NotFound();
                
                tourist.IsLocked = !tourist.IsLocked;
                isLocking = tourist.IsLocked;
                userEmail = tourist.Email;
                userName = tourist.FullName ?? "Người dùng";
            }
            else
            {
                var appUser = await _context.AppUsers.FindAsync(id);
                if (appUser == null) return NotFound();

                // Không cho phép tự khóa chính mình
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (currentUserId == id.ToString())
                {
                    return BadRequest("Bạn không thể tự khóa tài khoản của chính mình.");
                }

                appUser.IsLocked = !appUser.IsLocked;
                isLocking = appUser.IsLocked;
                userEmail = appUser.Email ?? "";
                userName = appUser.Username;
            }

            await _context.SaveChangesAsync();

            // Nếu là hành động Khóa, gửi Email thông báo
            if (isLocking && !string.IsNullOrEmpty(userEmail))
            {
                try
                {
                    await _emailService.SendAccountLockedEmailAsync(userEmail, userName);
                }
                catch (Exception ex)
                {
                    // Log lỗi gửi mail nhưng vẫn trả về thành công vì DB đã update
                    System.Diagnostics.Debug.WriteLine($"Failed to send lock email: {ex.Message}");
                }
            }

            return Ok(new { isLocked = isLocking });
        }
    }

    public class UserManagementDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string Role { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
        public bool IsLocked { get; set; }
        public DateTime? LastActiveAt { get; set; }
        public string? LastActiveIp { get; set; }
        public string? LastActiveDevice { get; set; }
        public DateTime CreatedAt { get; set; }

        // IsOnline nếu hoạt động trong vòng 15 giây đổ lại (vì Throttling đang set 5 giây)
        public bool IsOnline => LastActiveAt.HasValue && LastActiveAt.Value > DateTime.UtcNow.AddSeconds(-15);
    }
}
