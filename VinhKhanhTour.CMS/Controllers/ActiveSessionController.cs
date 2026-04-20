using Microsoft.AspNetCore.Mvc;
using VinhKhanhTour.CMS.Services;

namespace VinhKhanhTour.CMS.Controllers
{
    [ApiController]
    [IgnoreAntiforgeryToken]
    public class ActiveSessionController : ControllerBase
    {
        private readonly ActiveSessionTracker _tracker;

        public ActiveSessionController(ActiveSessionTracker tracker)
        {
            _tracker = tracker;
        }

        // Endpoint GET cho Admin Dashboard (UserManagement.razor)
        [HttpGet("api/admin/active-sessions")]
        public IActionResult GetActiveSessions()
        {
            // Lấy danh sách những người hoạt động trong vòng 5 phút (300s)
            var sessions = _tracker.GetActiveSessions(300);
            return Ok(sessions);
        }

        // Endpoint POST cho các Client (như ListenApp) ping để báo trạng thái online
        [HttpPost("api/tracking/ping")]
        public IActionResult Ping([FromBody] PingRequest req)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
            var ua = Request.Headers["User-Agent"].ToString();

            var session = new UserSessionDto
            {
                Id = req.Id,
                UserType = req.UserType ?? "Guest",
                Identifier = req.Identifier ?? "Unknown",
                DisplayName = req.DisplayName ?? (req.UserType == "Guest" ? "Khách vãng lai" : "Người dùng"),
                Avatar = req.Avatar ?? "",
                IpAddress = ip,
                UserAgent = ua
            };

            _tracker.Ping(session);
            return Ok();
        }
    }

    public class PingRequest
    {
        public int Id { get; set; }
        public string? UserType { get; set; }
        public string? Identifier { get; set; }
        public string? DisplayName { get; set; }
        public string? Avatar { get; set; }
    }
}
