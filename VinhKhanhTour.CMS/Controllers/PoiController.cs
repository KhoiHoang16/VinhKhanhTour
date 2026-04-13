using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VinhKhanhTour.CMS.Services;
using VinhKhanhTour.Shared.Models;

namespace VinhKhanhTour.CMS.Controllers
{
    [ApiController]
    [Route("api/poi")]
    public class PoiController : ControllerBase
    {
        private readonly PoiService _poiService;
        private readonly ICurrentUserService _currentUserService;

        public PoiController(PoiService poiService, ICurrentUserService currentUserService)
        {
            _poiService = poiService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pois = await _poiService.GetPoisAsync();
            return Ok(pois);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Poi poi)
        {
            await _poiService.CreatePoiAsync(poi);
            return Created($"/api/poi/{poi.Id}", poi);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Poi poi)
        {
            poi.Id = id;
            await _poiService.UpdatePoiAsync(poi);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _poiService.DeletePoiAsync(id);
            return NoContent();
        }

        // ================================================================
        // APPROVAL WORKFLOW ENDPOINTS (Admin-only)
        // ================================================================

        /// <summary>
        /// Lấy danh sách POI đang chờ duyệt. Chỉ Admin mới có quyền.
        /// </summary>
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPending()
        {
            var pois = await _poiService.GetPendingPoisAsync();
            return Ok(pois);
        }

        /// <summary>
        /// Đếm số POI đang chờ duyệt (cho badge count).
        /// </summary>
        [HttpGet("pending/count")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingCount()
        {
            var count = await _poiService.GetPendingCountAsync();
            return Ok(count);
        }

        /// <summary>
        /// Admin phê duyệt POI. Đảm bảo kiểm tra quyền Admin.
        /// Sử dụng SaveChangesAsync nội bộ.
        /// </summary>
        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApprovePoi(int id)
        {
            // Kiểm tra quyền Admin (defense in depth, ngoài [Authorize])
            if (!_currentUserService.IsSystemAdmin)
            {
                return Forbid();
            }

            var result = await _poiService.ApprovePoiAsync(id);
            if (!result)
            {
                return NotFound(new { message = $"Không tìm thấy POI với ID = {id}" });
            }

            return Ok(new { message = $"POI #{id} đã được phê duyệt thành công." });
        }

        /// <summary>
        /// Admin từ chối POI với lý do (AdminNote).
        /// </summary>
        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectPoi(int id, [FromBody] RejectPoiRequest request)
        {
            // Kiểm tra quyền Admin (defense in depth)
            if (!_currentUserService.IsSystemAdmin)
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(request?.Note))
            {
                return BadRequest(new { message = "Lý do từ chối không được để trống." });
            }

            var result = await _poiService.RejectPoiAsync(id, request.Note);
            if (!result)
            {
                return NotFound(new { message = $"Không tìm thấy POI với ID = {id}" });
            }

            return Ok(new { message = $"POI #{id} đã bị từ chối. Lý do: {request.Note}" });
        }
    }

    /// <summary>DTO cho request từ chối POI.</summary>
    public class RejectPoiRequest
    {
        public string Note { get; set; } = string.Empty;
    }
}
