using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using VinhKhanhTour.CMS.Data;
using Microsoft.Extensions.DependencyInjection;
using VinhKhanhTour.CMS.Services;

namespace VinhKhanhTour.CMS.Middleware
{
    public class UserActivityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly ActiveSessionTracker _tracker;

        public UserActivityMiddleware(RequestDelegate next, IMemoryCache cache, ActiveSessionTracker tracker)
        {
            _next = next;
            _cache = cache;
            _tracker = tracker;
        }

        public async Task InvokeAsync(HttpContext context, IServiceScopeFactory scopeFactory)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userIdStr = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userType = context.User.FindFirstValue("UserType") ?? "AppUser"; // Mặc định là AppUser nếu không có (Cookie Auth)

                if (int.TryParse(userIdStr, out int userId))
                {
                    using var scope = scopeFactory.CreateScope();
                    var dbPostgres = scope.ServiceProvider.GetRequiredService<CmsDbContext>();

                    // 1. Kiểm tra trạng thái Khóa (Security Guard)
                    // Lưu ý: Ta nên check cache cho trạng thái khóa để cực kỳ nhanh, nhưng ở đây ưu tiên tính "Tức thì" nên check DB
                    bool isLocked = false;

                    if (userType == "Tourist")
                    {
                        var tourist = await dbPostgres.Tourists.AsNoTracking()
                            .Where(t => t.Id == userId)
                            .Select(t => new { t.IsLocked })
                            .FirstOrDefaultAsync();
                        
                        isLocked = tourist?.IsLocked ?? false;
                    }
                    else
                    {
                        var appUser = await dbPostgres.AppUsers.AsNoTracking()
                            .Where(u => u.Id == userId)
                            .Select(u => new { u.IsLocked })
                            .FirstOrDefaultAsync();
                        
                        isLocked = appUser?.IsLocked ?? false;
                    }

                    if (isLocked)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Account is locked.");
                        return;
                    }

                    // 2. Tracking Online Status với Throttling (3 phút)
                    string cacheKey = $"heartbeat_{userType}_{userId}";
                    if (!_cache.TryGetValue(cacheKey, out _))
                    {
                        var ip = context.Connection.RemoteIpAddress?.ToString();
                        var device = context.Request.Headers["User-Agent"].ToString();

                        if (userType == "Tourist")
                        {
                            var tourist = await dbPostgres.Tourists.FindAsync(userId);
                            if (tourist != null)
                            {
                                tourist.LastActiveAt = DateTime.UtcNow;
                                tourist.LastActiveIp = ip;
                                tourist.LastActiveDevice = device;
                                await dbPostgres.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            var appUser = await dbPostgres.AppUsers.FindAsync(userId);
                            if (appUser != null)
                            {
                                appUser.LastActiveAt = DateTime.UtcNow;
                                appUser.LastActiveIp = ip;
                                appUser.LastActiveDevice = device;
                                await dbPostgres.SaveChangesAsync();
                            }
                        }

                        // Set cache để 5 giây sau mới update tiếp
                        _cache.Set(cacheKey, true, TimeSpan.FromSeconds(5));
                    }

                    // 3. Update Real-time Tracker (In-Memory, siêu nhẹ nên gọi liên tục được)
                    var ipForTracker = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
                    var deviceForTracker = context.Request.Headers["User-Agent"].ToString();
                    var nameForTracker = context.User.Identity?.Name ?? $"User {userId}";
                    
                    _tracker.Ping(new Services.UserSessionDto
                    {
                        Id = userId,
                        UserType = userType,
                        Identifier = userId.ToString(),
                        DisplayName = nameForTracker,
                        IpAddress = ipForTracker,
                        UserAgent = deviceForTracker
                    });
                }
            }

            await _next(context);
        }
    }
}
