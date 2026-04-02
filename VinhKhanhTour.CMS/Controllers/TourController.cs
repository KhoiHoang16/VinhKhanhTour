using Microsoft.AspNetCore.Mvc;
using VinhKhanhTour.CMS.Services;
using VinhKhanhTour.Shared.Models;

namespace VinhKhanhTour.CMS.Controllers
{
    [ApiController]
    [Route("api/tour")]
    public class TourController : ControllerBase
    {
        private readonly TourService _tourService;
        private readonly ILogger<TourController> _logger;

        public TourController(TourService tourService, ILogger<TourController> logger)
        {
            _tourService = tourService;
            _logger = logger;
        }

        /// <summary>
        /// GET /api/tour — Lấy tất cả Tour (kèm danh sách điểm dừng)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<TourDetailDto>>> GetAll()
        {
            try
            {
                var tours = await _tourService.GetAllToursAsync();
                return Ok(tours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy danh sách Tour");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// GET /api/tour/{id} — Lấy chi tiết 1 Tour
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TourDetailDto>> GetById(int id)
        {
            try
            {
                var tour = await _tourService.GetTourByIdAsync(id);
                if (tour == null) return NotFound(new { error = $"Không tìm thấy Tour có Id={id}" });
                return Ok(tour);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy Tour Id={Id}", id);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// POST /api/tour — Tạo mới Tour + Stops (Transaction)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TourDetailDto>> Create([FromBody] TourCreateUpdateDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.TourName))
                    return BadRequest(new { error = "TourName là trường bắt buộc." });

                var created = await _tourService.CreateTourAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tạo Tour");
                return StatusCode(500, new { error = ex.Message, inner = ex.InnerException?.Message });
            }
        }

        /// <summary>
        /// PUT /api/tour/{id} — Cập nhật Tour + Stops (Transaction)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TourDetailDto>> Update(int id, [FromBody] TourCreateUpdateDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.TourName))
                    return BadRequest(new { error = "TourName là trường bắt buộc." });

                var updated = await _tourService.UpdateTourAsync(id, dto);
                if (updated == null) return NotFound(new { error = $"Không tìm thấy Tour có Id={id}" });
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật Tour Id={Id}", id);
                return StatusCode(500, new { error = ex.Message, inner = ex.InnerException?.Message });
            }
        }

        /// <summary>
        /// DELETE /api/tour/{id} — Xóa Tour (Cascade xóa Stops)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _tourService.DeleteTourAsync(id);
                if (!result) return NotFound(new { error = $"Không tìm thấy Tour có Id={id}" });
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xóa Tour Id={Id}", id);
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
