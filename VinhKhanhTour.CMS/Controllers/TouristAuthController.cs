using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VinhKhanhTour.CMS.Data;
using VinhKhanhTour.CMS.Services;
using VinhKhanhTour.Shared.Models;

namespace VinhKhanhTour.CMS.Controllers
{
    [ApiController]
    [Route("api/tourist-auth")]
    public class TouristAuthController : ControllerBase
    {
        private readonly IDbContextFactory<CmsDbContext> _factory;
        private readonly IFirebaseAuthService _firebaseAuth;
        private readonly ITouristTokenService _tokenService;
        private readonly ILogger<TouristAuthController> _logger;

        public TouristAuthController(
            IDbContextFactory<CmsDbContext> factory,
            IFirebaseAuthService firebaseAuth,
            ITouristTokenService tokenService,
            ILogger<TouristAuthController> logger)
        {
            _factory = factory;
            _firebaseAuth = firebaseAuth;
            _tokenService = tokenService;
            _logger = logger;
        }

        /// <summary>
        /// Đăng nhập Tourist từ Mobile App.
        /// 
        /// Luồng:
        /// 1. Verify Firebase Token
        /// 2. Tìm hoặc tạo Tourist mới
        /// 3. Đồng bộ DevicePurchases ẩn danh → Tourist (Transaction)
        /// 4. Phát hành JWT nội bộ VinhKhanhTour
        /// 
        /// POST /api/tourist-auth/login-mobile
        /// </summary>
        [HttpPost("login-mobile")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginMobile([FromBody] MobileLoginRequest request)
        {
            // === VALIDATION ===
            if (string.IsNullOrWhiteSpace(request.FirebaseToken))
                return BadRequest(new { error = "FirebaseToken là bắt buộc." });

            if (string.IsNullOrWhiteSpace(request.DeviceId))
                return BadRequest(new { error = "DeviceId là bắt buộc." });

            // === STEP 1: VERIFY FIREBASE TOKEN ===
            var firebaseUser = await _firebaseAuth.VerifyTokenAsync(request.FirebaseToken);
            if (firebaseUser == null)
            {
                _logger.LogWarning("[TouristAuth] ❌ Firebase token verification failed for device {DeviceId}",
                    request.DeviceId.Length > 8 ? request.DeviceId[..8] + "..." : request.DeviceId);
                return Unauthorized(new { error = "Token không hợp lệ hoặc đã hết hạn." });
            }

            _logger.LogInformation("[TouristAuth] ✅ Firebase verified: UID={Uid}, Email={Email}",
                firebaseUser.Uid, firebaseUser.Email);

            // === STEP 2 & 3: UPSERT TOURIST + SYNC PURCHASES (TRANSACTION) ===
            using var db = await _factory.CreateDbContextAsync();
            using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                // Tìm Tourist đã tồn tại bằng Firebase UID
                var tourist = await db.Tourists
                    .FirstOrDefaultAsync(t => t.SocialId == firebaseUser.Uid);

                bool isNewUser = false;

                if (tourist == null)
                {
                    // === TOURIST MỚI — INSERT ===
                    isNewUser = true;
                    tourist = new Tourist
                    {
                        SocialId = firebaseUser.Uid,
                        Email = firebaseUser.Email,
                        FullName = firebaseUser.DisplayName,
                        AvatarUrl = firebaseUser.PhotoUrl,
                        AuthProvider = firebaseUser.Provider,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        LastLoginAt = DateTime.UtcNow
                    };

                    db.Tourists.Add(tourist);
                    await db.SaveChangesAsync(); // Cần save để có Tourist.Id cho FK
                    _logger.LogInformation("[TouristAuth] 🆕 Created new Tourist: Id={Id}, Email={Email}",
                        tourist.Id, tourist.Email);
                }
                else
                {
                    // === TOURIST ĐÃ TỒN TẠI — UPDATE LastLoginAt ===
                    tourist.LastLoginAt = DateTime.UtcNow;

                    // Cập nhật thông tin nếu thay đổi từ phía Social
                    if (!string.IsNullOrEmpty(firebaseUser.DisplayName))
                        tourist.FullName = firebaseUser.DisplayName;
                    if (!string.IsNullOrEmpty(firebaseUser.PhotoUrl))
                        tourist.AvatarUrl = firebaseUser.PhotoUrl;

                    _logger.LogInformation("[TouristAuth] 🔄 Existing Tourist login: Id={Id}, Email={Email}",
                        tourist.Id, tourist.Email);
                }

                // === STEP 3: ĐỒNG BỘ DEVICEPURCHASES ẨN DANH ===
                // ⚠️ Dùng IgnoreQueryFilters() vì DevicePurchases không có AgencyId filter,
                // nhưng an toàn hơn khi bypass mọi filter để đảm bảo tầm nhìn toàn cục.
                var anonymousPurchases = await db.DevicePurchases
                    .Where(p => p.DeviceId == request.DeviceId && p.TouristId == null)
                    .ToListAsync();

                int syncedCount = 0;
                if (anonymousPurchases.Any())
                {
                    foreach (var purchase in anonymousPurchases)
                    {
                        purchase.TouristId = tourist.Id;
                    }
                    syncedCount = anonymousPurchases.Count;

                    _logger.LogInformation(
                        "[TouristAuth] 🔗 Synced {Count} anonymous purchases (DeviceId={DeviceId}) → Tourist #{TouristId}",
                        syncedCount,
                        request.DeviceId.Length > 8 ? request.DeviceId[..8] + "..." : request.DeviceId,
                        tourist.Id);
                }

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                // === STEP 4: PHÁT HÀNH JWT NỘI BỘ ===
                var jwtToken = _tokenService.GenerateToken(tourist);

                _logger.LogInformation(
                    "[TouristAuth] 🎫 JWT issued for Tourist #{Id} ({Email}), synced {Count} purchases",
                    tourist.Id, tourist.Email, syncedCount);

                return Ok(new MobileLoginResponse
                {
                    Token = jwtToken,
                    Tourist = new TouristInfo
                    {
                        Id = tourist.Id,
                        Email = tourist.Email,
                        FullName = tourist.FullName,
                        AvatarUrl = tourist.AvatarUrl,
                        AuthProvider = tourist.AuthProvider,
                        IsNewUser = isNewUser
                    },
                    SyncedPurchases = syncedCount
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "[TouristAuth] ❌ Login failed for device {DeviceId}",
                    request.DeviceId.Length > 8 ? request.DeviceId[..8] + "..." : request.DeviceId);

                return StatusCode(500, new { error = "Đã xảy ra lỗi trong quá trình đăng nhập. Vui lòng thử lại." });
            }
        }

        /// <summary>
        /// Lấy thông tin Tourist hiện tại từ JWT Token.
        /// Yêu cầu Tourist đã đăng nhập (Authorization: Bearer {token}).
        /// 
        /// GET /api/tourist-auth/me
        /// </summary>
        [HttpGet("me")]
        [Authorize(Policy = "TouristOnly")]
        public async Task<IActionResult> GetCurrentTourist()
        {
            var touristIdClaim = User.FindFirst("TouristId")?.Value;
            if (string.IsNullOrEmpty(touristIdClaim) || !int.TryParse(touristIdClaim, out int touristId))
                return Unauthorized();

            using var db = await _factory.CreateDbContextAsync();

            var tourist = await db.Tourists.FindAsync(touristId);
            if (tourist == null || !tourist.IsActive)
                return NotFound(new { error = "Tài khoản không tồn tại hoặc đã bị khóa." });

            // Đếm tổng số purchases của Tourist (bao gồm cả đã sync và mua khi đã đăng nhập)
            var purchaseCount = await db.DevicePurchases
                .CountAsync(p => p.TouristId == touristId && p.IsActive);

            return Ok(new
            {
                tourist = new TouristInfo
                {
                    Id = tourist.Id,
                    Email = tourist.Email,
                    FullName = tourist.FullName,
                    AvatarUrl = tourist.AvatarUrl,
                    AuthProvider = tourist.AuthProvider,
                    IsNewUser = false
                },
                totalPurchases = purchaseCount
            });
        }

        /// <summary>
        /// Lấy danh sách Audio đã mua của Tourist.
        /// 
        /// GET /api/tourist-auth/my-purchases
        /// </summary>
        [HttpGet("my-purchases")]
        [Authorize(Policy = "TouristOnly")]
        public async Task<IActionResult> GetMyPurchases()
        {
            var touristIdClaim = User.FindFirst("TouristId")?.Value;
            if (string.IsNullOrEmpty(touristIdClaim) || !int.TryParse(touristIdClaim, out int touristId))
                return Unauthorized();

            using var db = await _factory.CreateDbContextAsync();

            var purchases = await db.DevicePurchases
                .Where(p => p.TouristId == touristId && p.IsActive)
                .OrderByDescending(p => p.PurchasedAt)
                .Select(p => new
                {
                    p.Id,
                    p.PoiId,
                    p.PurchaseType,
                    p.Amount,
                    p.Currency,
                    p.PaymentMethod,
                    p.PurchasedAt,
                    p.DeviceId
                })
                .ToListAsync();

            return Ok(new { purchases, total = purchases.Count });
        }
    }

    // ===== DTOs =====

    public class MobileLoginRequest
    {
        /// <summary>Firebase ID Token từ Google/Apple Sign-in trên Mobile</summary>
        public string FirebaseToken { get; set; } = string.Empty;

        /// <summary>DeviceId hiện tại của thiết bị Mobile (để đồng bộ purchases ẩn danh)</summary>
        public string DeviceId { get; set; } = string.Empty;
    }

    public class MobileLoginResponse
    {
        /// <summary>JWT Token nội bộ VinhKhanhTour — Mobile App dùng để gọi các API protected</summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>Thông tin Tourist</summary>
        public TouristInfo Tourist { get; set; } = new();

        /// <summary>Số lượng giao dịch ẩn danh đã được đồng bộ vào tài khoản</summary>
        public int SyncedPurchases { get; set; }
    }

    public class TouristInfo
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public string AuthProvider { get; set; } = string.Empty;
        public bool IsNewUser { get; set; }
    }
}
