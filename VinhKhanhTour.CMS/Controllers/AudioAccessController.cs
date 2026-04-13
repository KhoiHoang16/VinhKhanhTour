using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinhKhanhTour.CMS.Data;
using VinhKhanhTour.CMS.Services;
using VinhKhanhTour.Shared.Models;

namespace VinhKhanhTour.CMS.Controllers
{
    [ApiController]
    [Route("api/audio")]
    [AllowAnonymous]
    public class AudioAccessController : ControllerBase
    {
        private readonly IDbContextFactory<CmsDbContext> _factory;
        private readonly IConfiguration _config;
        private readonly MoMoPaymentService _momoService;
        private readonly VNPayPaymentService _vnpayService;

        public AudioAccessController(
            IDbContextFactory<CmsDbContext> factory,
            IConfiguration config,
            MoMoPaymentService momoService,
            VNPayPaymentService vnpayService)
        {
            _factory = factory;
            _config = config;
            _momoService = momoService;
            _vnpayService = vnpayService;
        }

        /// <summary>
        /// Kiểm tra du khách đã mua quyền nghe Audio của POI này chưa.
        /// GET /api/audio/check-access?deviceId=xxx&poiId=5
        /// </summary>
        [HttpGet("check-access")]
        public async Task<IActionResult> CheckAccess([FromQuery] string deviceId, [FromQuery] int poiId)
        {
            if (string.IsNullOrWhiteSpace(deviceId) || poiId <= 0)
                return BadRequest(new { error = "deviceId và poiId là bắt buộc." });

            using var db = await _factory.CreateDbContextAsync();

            var purchase = await db.DevicePurchases
                .Where(p => p.DeviceId == deviceId && p.IsActive)
                .Where(p => p.PoiId == poiId || p.PurchaseType == "all_access")
                .Where(p => p.ExpiresAt == null || p.ExpiresAt > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (purchase != null)
            {
                return Ok(new
                {
                    hasAccess = true,
                    purchaseType = purchase.PurchaseType,
                    purchasedAt = purchase.PurchasedAt
                });
            }

            // Chưa mua → trả giá
            var price = _config.GetValue<int>("AudioPaywall:DefaultPriceVND", 20000);
            var currency = _config["AudioPaywall:Currency"] ?? "VND";

            var poi = await db.Pois.IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == poiId && !p.IsDeleted);

            return Ok(new
            {
                hasAccess = false,
                price,
                currency,
                poiName = poi?.Name ?? "Unknown"
            });
        }

        /// <summary>
        /// Tạo link thanh toán MoMo.
        /// POST /api/audio/create-momo-payment
        /// </summary>
        [HttpPost("create-momo-payment")]
        public async Task<IActionResult> CreateMoMoPayment([FromBody] PaymentCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.DeviceId) || request.PoiId <= 0)
                return BadRequest(new { error = "Dữ liệu không hợp lệ." });

            var price = _config.GetValue<int>("AudioPaywall:DefaultPriceVND", 20000);
            var orderId = $"VKT_{request.PoiId}_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid().ToString()[..6]}";
            var orderInfo = $"Mở khóa Audio POI #{request.PoiId}";

            // extraData chứa DeviceId + PoiId để xử lý trong IPN callback
            var extraData = Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes($"{request.DeviceId}|{request.PoiId}|{request.RecoveryContact ?? ""}")
            );

            var result = await _momoService.CreatePaymentAsync(orderId, price, orderInfo, extraData);

            if (result == null || result.ResultCode != 0)
            {
                return StatusCode(500, new
                {
                    error = "Không thể tạo link thanh toán MoMo.",
                    detail = result?.Message ?? "Unknown error"
                });
            }

            return Ok(new
            {
                payUrl = result.PayUrl,
                qrCodeUrl = result.QrCodeUrl,
                orderId
            });
        }

        /// <summary>
        /// Tạo URL thanh toán VNPay.
        /// POST /api/audio/create-vnpay-payment
        /// </summary>
        [HttpPost("create-vnpay-payment")]
        public IActionResult CreateVNPayPayment([FromBody] PaymentCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.DeviceId) || request.PoiId <= 0)
                return BadRequest(new { error = "Dữ liệu không hợp lệ." });

            var price = _config.GetValue<int>("AudioPaywall:DefaultPriceVND", 20000);
            // Encode DeviceId + PoiId vào orderId để trích xuất khi callback
            var orderId = $"VKT{request.PoiId}T{DateTime.UtcNow:yyyyMMddHHmmss}";
            var orderInfo = $"Mo khoa Audio POI {request.PoiId} - Device {request.DeviceId}";
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

            // Lưu tạm mapping orderId → deviceId vào memory (hoặc DB)
            // Trong production, nên lưu vào DB pending_payments table
            PendingPayments.TryAdd(orderId, new PendingPaymentInfo
            {
                DeviceId = request.DeviceId,
                PoiId = request.PoiId,
                RecoveryContact = request.RecoveryContact
            });

            var paymentUrl = _vnpayService.CreatePaymentUrl(orderId, price, orderInfo, ipAddress);

            return Ok(new { payUrl = paymentUrl, orderId });
        }

        /// <summary>
        /// MoMo IPN Callback — MoMo gọi server khi thanh toán thành công.
        /// POST /api/audio/momo-ipn
        /// </summary>
        [HttpPost("momo-ipn")]
        public async Task<IActionResult> MoMoIpn([FromBody] MoMoIpnRequest ipn)
        {
            // Xác thực chữ ký
            if (!_momoService.VerifyIpnSignature(ipn))
            {
                return BadRequest(new { message = "Invalid signature" });
            }

            // resultCode = 0 nghĩa là thanh toán thành công
            if (ipn.ResultCode != 0)
            {
                return Ok(new { message = "Payment not successful, ignored." });
            }

            // Parse extraData để lấy DeviceId, PoiId
            try
            {
                var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(ipn.ExtraData));
                var parts = decoded.Split('|');
                if (parts.Length < 2) return BadRequest(new { message = "Invalid extraData" });

                var deviceId = parts[0];
                var poiId = int.Parse(parts[1]);
                var recovery = parts.Length > 2 ? parts[2] : null;

                await RecordPurchase(deviceId, poiId, "momo", ipn.TransId.ToString(),
                    ipn.Amount, string.IsNullOrEmpty(recovery) ? null : recovery);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MoMo IPN] Error: {ex.Message}");
                return StatusCode(500);
            }

            return NoContent(); // MoMo expects 204
        }

        /// <summary>
        /// VNPay Return URL — User được redirect về sau thanh toán.
        /// GET /api/audio/vnpay-return
        /// </summary>
        [HttpGet("vnpay-return")]
        public async Task<IActionResult> VNPayReturn()
        {
            // Xác thực chữ ký
            if (!_vnpayService.ValidateSignature(Request.Query))
            {
                return BadRequest("Chữ ký không hợp lệ.");
            }

            var result = _vnpayService.ParseReturn(Request.Query);

            if (!result.IsSuccess)
            {
                // Redirect về trang lỗi
                return Redirect($"/listen/payment-failed?reason={result.ResponseCode}");
            }

            // Tìm pending payment info
            if (PendingPayments.TryRemove(result.TxnRef, out var info))
            {
                await RecordPurchase(info.DeviceId, info.PoiId, "vnpay",
                    result.TransactionNo, result.Amount, info.RecoveryContact);

                // Redirect về ListenApp
                return Redirect($"/listen/{info.PoiId}?unlocked=true");
            }

            return Redirect("/listen/payment-failed?reason=order_not_found");
        }

        /// <summary>
        /// MoMo Return URL — User được redirect về sau thanh toán MoMo.
        /// GET /api/audio/momo-return
        /// </summary>
        [HttpGet("momo-return")]
        public IActionResult MoMoReturn(
            [FromQuery] string orderId,
            [FromQuery] int resultCode,
            [FromQuery] string extraData)
        {
            if (resultCode == 0 && !string.IsNullOrEmpty(extraData))
            {
                try
                {
                    var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(extraData));
                    var parts = decoded.Split('|');
                    if (parts.Length >= 2)
                    {
                        var poiId = int.Parse(parts[1]);
                        return Redirect($"/listen/{poiId}?unlocked=true");
                    }
                }
                catch { }
            }

            return Redirect("/listen/payment-failed?reason=momo_error");
        }

        /// <summary>
        /// API unlock thủ công (cho admin hoặc test).
        /// POST /api/audio/unlock
        /// </summary>
        [HttpPost("unlock")]
        public async Task<IActionResult> ManualUnlock([FromBody] UnlockRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.DeviceId) || request.PoiId <= 0)
                return BadRequest(new { error = "Dữ liệu không hợp lệ." });

            await RecordPurchase(request.DeviceId, request.PoiId,
                request.PaymentMethod, request.TransactionId,
                request.Amount, request.RecoveryContact);

            return Created("", new
            {
                success = true,
                message = "Mở khóa thành công! Bạn có thể nghe Audio vĩnh viễn."
            });
        }

        /// <summary>
        /// Khôi phục giao dịch bằng Email/SĐT khi đổi thiết bị.
        /// POST /api/audio/recover
        /// </summary>
        [HttpPost("recover")]
        public async Task<IActionResult> RecoverPurchases([FromBody] RecoverRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NewDeviceId) || string.IsNullOrWhiteSpace(request.RecoveryContact))
                return BadRequest(new { error = "Thiếu thông tin khôi phục." });

            using var db = await _factory.CreateDbContextAsync();

            var oldPurchases = await db.DevicePurchases
                .Where(p => p.RecoveryContact == request.RecoveryContact && p.IsActive)
                .ToListAsync();

            if (!oldPurchases.Any())
            {
                return NotFound(new { error = "Không tìm thấy giao dịch nào với thông tin này." });
            }

            // Tạo bản sao cho DeviceId mới
            foreach (var old in oldPurchases)
            {
                // Kiểm tra đã có chưa
                var exists = await db.DevicePurchases.AnyAsync(p =>
                    p.DeviceId == request.NewDeviceId && p.PoiId == old.PoiId && p.IsActive);

                if (!exists)
                {
                    db.DevicePurchases.Add(new DevicePurchase
                    {
                        DeviceId = request.NewDeviceId,
                        PoiId = old.PoiId,
                        PurchaseType = old.PurchaseType,
                        Amount = 0, // Khôi phục — không tính tiền
                        PaymentMethod = "recovery",
                        TransactionId = $"RECOVER_{old.Id}",
                        RecoveryContact = request.RecoveryContact,
                        PurchasedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }
            }

            await db.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                restoredCount = oldPurchases.Count,
                message = $"Đã khôi phục {oldPurchases.Count} giao dịch thành công!"
            });
        }

        // ===== PRIVATE HELPERS =====

        private async Task RecordPurchase(string deviceId, int poiId, string method,
            string? transactionId, decimal amount, string? recovery)
        {
            using var db = await _factory.CreateDbContextAsync();

            // Chống duplicate transaction
            if (!string.IsNullOrEmpty(transactionId))
            {
                var dup = await db.DevicePurchases.AnyAsync(p => p.TransactionId == transactionId);
                if (dup) return;
            }

            db.DevicePurchases.Add(new DevicePurchase
            {
                DeviceId = deviceId,
                PoiId = poiId,
                PurchaseType = "single",
                Amount = amount,
                PaymentMethod = method,
                TransactionId = transactionId,
                RecoveryContact = recovery,
                PurchasedAt = DateTime.UtcNow,
                IsActive = true
            });

            await db.SaveChangesAsync();
            Console.WriteLine($"[AudioAccess] ✅ Unlocked POI#{poiId} for device {deviceId[..8]}... via {method}");
        }

        // In-memory pending payments (cho VNPay flow)
        // Trong production, nên dùng DB hoặc Redis
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, PendingPaymentInfo>
            PendingPayments = new();
    }

    // ===== DTOs =====

    public class PaymentCreateRequest
    {
        public string DeviceId { get; set; } = string.Empty;
        public int PoiId { get; set; }
        public string? RecoveryContact { get; set; }
    }

    public class RecoverRequest
    {
        public string NewDeviceId { get; set; } = string.Empty;
        public string RecoveryContact { get; set; } = string.Empty;
    }

    public class PendingPaymentInfo
    {
        public string DeviceId { get; set; } = string.Empty;
        public int PoiId { get; set; }
        public string? RecoveryContact { get; set; }
    }
}
