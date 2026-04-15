using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using VinhKhanhTour.CMS.Data;
using VinhKhanhTour.Shared.Models;

namespace VinhKhanhTour.CMS.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class MobileAuthController : ControllerBase
    {
        private readonly CmsDbContext _context;
        private readonly IConfiguration _configuration;

        public MobileAuthController(CmsDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { error = "Email và mật khẩu là bắt buộc." });

            var tourist = await _context.Tourists.FirstOrDefaultAsync(t => t.Email == request.Email);
            if (tourist == null)
                return Unauthorized(new { error = "Sai tài khoản hoặc mật khẩu." });

            if (!VerifyPassword(request.Password, tourist.PasswordHash))
                return Unauthorized(new { error = "Sai tài khoản hoặc mật khẩu." });

            if (tourist.IsLocked)
                return StatusCode(403, new { error = "Tài khoản của bạn đã bị khóa." });

            tourist.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(tourist);
            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { error = "Email và mật khẩu là bắt buộc." });

            if (await _context.Tourists.AnyAsync(t => t.Email == request.Email))
                return BadRequest(new { error = "Email này đã được đăng ký." });

            var newTourist = new Tourist
            {
                Email = request.Email,
                FullName = request.FullName,
                PasswordHash = HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                IsLocked = false
            };

            _context.Tourists.Add(newTourist);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(newTourist);
            return Ok(new { token });
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            return HashPassword(inputPassword) == storedHash;
        }

        private string GenerateJwtToken(Tourist tourist)
        {
            var jwtSecretKey = _configuration["Jwt:SecretKey"]
                ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                ?? "VinhKhanhTour-Default-Dev-Key-Change-In-Production-Min32Chars!";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("TouristId", tourist.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, tourist.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, tourist.Email),
                new Claim("UserType", "Tourist"), // Theo đúng logic TouristOnly ở Program.cs
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var issuer = _configuration["Jwt:Issuer"] ?? "VinhKhanhTour";
            var audience = _configuration["Jwt:Audience"] ?? "VinhKhanhTourMobileApp";

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("me")]
        [Microsoft.AspNetCore.Authorization.Authorize(Policy = "TouristOnly")]
        public async Task<IActionResult> GetCurrentTourist()
        {
            var touristIdClaim = User.FindFirst("TouristId")?.Value;
            if (string.IsNullOrEmpty(touristIdClaim) || !int.TryParse(touristIdClaim, out int touristId))
                return Unauthorized();

            var tourist = await _context.Tourists.FindAsync(touristId);
            if (tourist == null || tourist.IsLocked)
                return NotFound(new { error = "Tài khoản không tồn tại hoặc đã bị khóa." });

            var purchaseCount = await _context.DevicePurchases
                .CountAsync(p => p.TouristId == touristId && p.IsActive);

            return Ok(new
            {
                tourist = new
                {
                    Id = tourist.Id,
                    Email = tourist.Email,
                    FullName = tourist.FullName,
                    AvatarUrl = tourist.AvatarUrl,
                    IsNewUser = false
                },
                totalPurchases = purchaseCount
            });
        }

        [HttpGet("my-purchases")]
        [Microsoft.AspNetCore.Authorization.Authorize(Policy = "TouristOnly")]
        public async Task<IActionResult> GetMyPurchases()
        {
            var touristIdClaim = User.FindFirst("TouristId")?.Value;
            if (string.IsNullOrEmpty(touristIdClaim) || !int.TryParse(touristIdClaim, out int touristId))
                return Unauthorized();

            var purchases = await _context.DevicePurchases
                .Where(p => p.TouristId == touristId && p.IsActive)
                .OrderByDescending(p => p.PurchasedAt)
                .Select(p => new
                {
                    p.Id,
                    p.PoiId,
                    p.PurchaseType,
                    p.Amount,
                    p.Currency,
                    p.PaymentMethod,
                    p.PurchasedAt,
                    p.DeviceId
                })
                .ToListAsync();

            return Ok(new { purchases, total = purchases.Count });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? FullName { get; set; }
    }
}
