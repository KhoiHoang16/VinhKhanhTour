using System.Threading.Tasks;
using VinhKhanhTour.Shared.Models;

namespace VinhKhanhTour.CMS.Services
{
    public interface IAppAnalyticsService
    {
        Task<DashboardOverviewDto> GetDashboardOverviewAsync();
        Task<PagedResult<AppAuditLogDto>> GetAuditLogsAsync(AuditLogFilterDto filter);
        Task RecordAuditLogAsync(string userId, string action, string details);
    }
}
