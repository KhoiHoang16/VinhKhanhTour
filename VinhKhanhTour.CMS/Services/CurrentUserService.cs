using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace VinhKhanhTour.CMS.Services
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        bool IsSystemAdmin { get; }
        int? AgencyId { get; }
    }

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        
        public bool IsSystemAdmin => _httpContextAccessor.HttpContext?.User?.IsInRole("Admin") ?? false;
        
        public int? AgencyId 
        {
            get 
            {
                var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("agency_id");
                if (claim != null && int.TryParse(claim.Value, out int agencyId))
                {
                    return agencyId;
                }
                return null;
            }
        }
    }
}
