using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VinhKhanhTour.CMS.Data;

namespace VinhKhanhTour.CMS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly CmsDbContext _dbContext;

        public AccountController(IConfiguration config, CmsDbContext dbContext)
        {
            _config = config;
            _dbContext = dbContext;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] string? Username, [FromForm] string? Password, [FromForm] string? ReturnUrl)
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                return LocalRedirect($"/login?error=true&returnUrl={Uri.EscapeDataString(ReturnUrl ?? "")}");
            }

            // 1. Try DB Authentication (Merchant/Agency)
            var user = await _dbContext.AppUsers.FirstOrDefaultAsync(u => u.Username == Username);
            if (user != null)
            {
                bool isPasswordValid = false;
                try 
                {
                    isPasswordValid = BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash);
                }
                catch 
                {
                    // Fallback to plain text check just for safety if older users exist, or just fail
                }

                if (isPasswordValid)
                {
                    var dbClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.FullName ?? user.Username),
                        new Claim(ClaimTypes.NameIdentifier, user.Username),
                        new Claim(ClaimTypes.Role, user.Role), // expects "owner" or similar
                        new Claim("agency_id", user.AgencyId?.ToString() ?? ""),
                        new Claim("force_password_change", user.ForcePasswordChange.ToString())
                    };

                    var dbIdentity = new ClaimsIdentity(dbClaims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(dbIdentity),
                        new AuthenticationProperties { IsPersistent = true });

                    string tgt = string.IsNullOrEmpty(ReturnUrl) ? "/" : ReturnUrl;
                    if (!tgt.StartsWith("/")) tgt = "/" + tgt;
                    if (!Url.IsLocalUrl(tgt)) tgt = "/";

                    // Force redirection overriden if pass change needed
                    if (user.ForcePasswordChange)
                    {
                        tgt = "/change-password";
                    }

                    return LocalRedirect(tgt);
                }
            }

            // 2. Fallback to Config Authentication (Super Admin)
            var adminUser = _config["AdminAccount:Username"];
            var adminPass = _config["AdminAccount:Password"];

            if (Username == adminUser && Password == adminPass)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Username),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties { IsPersistent = true });

                string target = string.IsNullOrEmpty(ReturnUrl) ? "/" : ReturnUrl;
                if (!target.StartsWith("/")) target = "/" + target;

                if (!Url.IsLocalUrl(target))
                {
                    target = "/";
                }

                return LocalRedirect(target);
            }

            // 3. Testing account (Keeping it for backwards compatibility if needed, or can remove logic)
            if (Username.ToLower() == "agency" && Password == "agency")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Agency Test User"),
                    new Claim(ClaimTypes.Role, "AgencyClient"),
                    new Claim("agency_id", "1") // Giả lập Data với AgencyId = 1
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties { IsPersistent = true });

                string target = string.IsNullOrEmpty(ReturnUrl) ? "/" : ReturnUrl;
                if (!target.StartsWith("/")) target = "/" + target;

                if (!Url.IsLocalUrl(target)) target = "/";

                return LocalRedirect(target);
            }

            return LocalRedirect($"/login?error=true&returnUrl={Uri.EscapeDataString(ReturnUrl ?? "")}");
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var username = User.Identity?.Name;
            var role = User.FindFirstValue(ClaimTypes.Role);
            
            // Note: username might be FullName depending on claims, let's use Name claim
            // Wait, we mapped Name to FullName. We need to find by AgencyId or something.
            // In Login we mapped ClaimTypes.Name to user.FullName ?? user.Username. This makes it hard to lookup.
            // I should use the agency_id to lookup for uniqueness instead, or update Login to map NameIdentifier.

            var agencyIdString = User.FindFirstValue("agency_id");
            if (string.IsNullOrEmpty(agencyIdString) || !int.TryParse(agencyIdString, out int agencyId))
            {
                return Unauthorized();
            }

            var user = await _dbContext.AppUsers.FirstOrDefaultAsync(u => u.AgencyId == agencyId);
            if (user == null)
            {
                return NotFound();
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.ForcePasswordChange = false;
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/login");
        }
    }

    public class ChangePasswordDto
    {
        public string NewPassword { get; set; } = string.Empty;
    }
}
