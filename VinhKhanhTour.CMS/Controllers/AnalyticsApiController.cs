using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VinhKhanhTour.CMS.Services;
using VinhKhanhTour.Shared.Data;
using VinhKhanhTour.Shared.Models;

namespace VinhKhanhTour.CMS.Controllers
{
    [ApiController]
    [Route("api/analytics")]
    public class AnalyticsApiController : ControllerBase
    {
        private readonly IAppAnalyticsService _analyticsService;
        private readonly IPoiRepository _poiRepository;
        private readonly ILogger<AnalyticsApiController> _logger;

        public AnalyticsApiController(IAppAnalyticsService analyticsService, IPoiRepository poiRepository, ILogger<AnalyticsApiController> logger)
        {
            _analyticsService = analyticsService;
            _poiRepository = poiRepository;
            _logger = logger;
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
            if (filter.Page <= 0) filter.Page = 1;
            if (filter.PageSize <= 0) filter.PageSize = 20;

            var result = await _analyticsService.GetAuditLogsAsync(filter);
            return Ok(result);
        }

        [HttpPost("audit-logs")]
        public async Task<IActionResult> RecordAuditLog([FromBody] AppAuditLogDto record)
        {
            try
            {
                if (record == null || string.IsNullOrEmpty(record.UserId) || string.IsNullOrEmpty(record.Action))
                {
                    return BadRequest(new { error = "UserId and Action are required fields." });
                }

                await _analyticsService.RecordAuditLogAsync(record.UserId, record.Action, record.Details ?? "");
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording audit log");
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpPost("sync-usage")]
        public async Task<IActionResult> SyncUsageHistory([FromBody] List<UsageHistory> histories)
        {
            try
            {
                _logger.LogInformation("=== SYNC-USAGE CALLED === Count: {Count}", histories?.Count ?? 0);

                if (histories == null || !histories.Any())
                {
                    return Ok(new { syncedCount = 0, message = "No data received" });
                }

                // Log chi tiết từng bản ghi nhận được
                foreach (var h in histories)
                {
                    _logger.LogInformation("Received: Id={Id}, PoiId={PoiId}, QR={QR}, Device={Device}, Time={Time}",
                        h.Id, h.PoiId, h.IsQrTriggered, h.DeviceId, h.Timestamp);
                }

                int savedCount = 0;
                foreach (var history in histories)
                {
                    // Reset ID để PostgreSQL tự sinh ID mới
                    history.Id = 0;
                    var result = await _poiRepository.RecordUsageAsync(history);
                    savedCount += result;
                    _logger.LogInformation("Saved record for PoiId={PoiId}, DB result={Result}", history.PoiId, result);
                }

                _logger.LogInformation("=== SYNC-USAGE DONE === Saved: {Count}", savedCount);
                return Ok(new { syncedCount = histories.Count, savedCount = savedCount });
            }
            catch (Exception ex)
            {
                // Trả về JSON lỗi chi tiết thay vì HTML Error page
                _logger.LogError(ex, "!!! SYNC-USAGE FAILED !!!");
                return StatusCode(500, new { 
                    error = ex.Message, 
                    innerError = ex.InnerException?.Message,
                    type = ex.GetType().Name,
                    stackTrace = ex.StackTrace 
                });
            }
        }
    }
}
