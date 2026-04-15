using VinhKhanhTour.Shared.Models;

namespace VinhKhanhTour.CMS.Services
{
    /// <summary>
    /// Interface phát hành JWT nội bộ VinhKhanhTour cho Tourist sau khi verify Firebase thành công.
    /// </summary>
    public interface ITouristTokenService
    {
        /// <summary>
        /// Tạo JWT Token nội bộ cho Tourist đã xác thực.
        /// Token có claim UserType = "Tourist" để phân biệt với CMS Admin/Agency.
        /// </summary>
        string GenerateToken(Tourist tourist);
    }
}
