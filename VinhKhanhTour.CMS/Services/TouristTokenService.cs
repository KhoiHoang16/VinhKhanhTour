using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using VinhKhanhTour.Shared.Models;

namespace VinhKhanhTour.CMS.Services
{
    /// <summary>
    /// Phát hành JWT nội bộ VinhKhanhTour cho Tourist.
    /// JWT có claim UserType = "Tourist" để Authorization Policy phân biệt.
    /// 
    /// Config required trong appsettings.json:
    /// {
    ///   "Jwt": {
    ///     "SecretKey": "your-256-bit-secret-key-here-minimum-32-chars",
    ///     "Issuer": "VinhKhanhTour",
    ///     "Audience": "VinhKhanhTourMobileApp",
    ///     "ExpiryDays": 30
    ///   }
    /// }
    /// </summary>
    public class TouristTokenService : ITouristTokenService
    {
        private readonly IConfiguration _config;

        public TouristTokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(Tourist tourist)
        {
            var secretKey = _config["Jwt:SecretKey"]
                ?? throw new InvalidOperationException("Jwt:SecretKey chưa được cấu hình! Kiểm tra appsettings hoặc Environment Variables.");

            var issuer = _config["Jwt:Issuer"] ?? "VinhKhanhTour";
            var audience = _config["Jwt:Audience"] ?? "VinhKhanhTourMobileApp";
            var expiryDays = _config.GetValue<int>("Jwt:ExpiryDays", 30);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                // Subject = TouristId cho identification
                new Claim(JwtRegisteredClaimNames.Sub, tourist.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, tourist.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

                // Custom claims cho Authorization Policy
                new Claim("UserType", "Tourist"),
                new Claim("TouristId", tourist.Id.ToString()),
            };

            // Thêm tên nếu có
            if (!string.IsNullOrEmpty(tourist.FullName))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Name, tourist.FullName));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(expiryDays),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
