using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VinhKhanhTour.CMS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AccountController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] string? Username, [FromForm] string? Password, [FromForm] string? ReturnUrl)
        {
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
            // Dành cho việc test Account Chủ quán / Đại lý số 1
            if (Username?.ToLower() == "agency" && Password == "agency")
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

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/login");
        }
    }
}
