using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VinhKhanhTour.CMS.Services;
using VinhKhanhTour.Shared.Models;

namespace VinhKhanhTour.CMS.Controllers
{
    [ApiController]
    [Route("api/analytics")]
    public class AnalyticsApiController : ControllerBase
    {
        private readonly IAppAnalyticsService _analyticsService;

        public AnalyticsApiController(IAppAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("overview")]
        public async Task<ActionResult<DashboardOverviewDto>> GetDashboardOverview()
        {
            var overview = await _analyticsService.GetDashboardOverviewAsync();
            return Ok(overview);
        }

        [HttpGet("audit-logs")]
        public async Task<ActionResult<PagedResult<AppAuditLogDto>>> GetAuditLogs([FromQuery] AuditLogFilterDto filter)
        {
            // Set defaults if missing
            if (filter.Page <= 0) filter.Page = 1;
            if (filter.PageSize <= 0) filter.PageSize = 20;

            var result = await _analyticsService.GetAuditLogsAsync(filter);
            return Ok(result);
        }

        [HttpPost("audit-logs")]
        public async Task<IActionResult> RecordAuditLog([FromBody] AppAuditLogDto record)
        {
            if (string.IsNullOrEmpty(record.UserId) || string.IsNullOrEmpty(record.Action))
            {
                return BadRequest("UserId and Action are required fields.");
            }

            await _analyticsService.RecordAuditLogAsync(record.UserId, record.Action, record.Details ?? "");
            return Ok();
        }
    }
}
