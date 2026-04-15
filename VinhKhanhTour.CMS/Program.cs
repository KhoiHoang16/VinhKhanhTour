using VinhKhanhTour.CMS.Components;
using VinhKhanhTour.CMS.Services;
using VinhKhanhTour.Shared.Data;
using VinhKhanhTour.Shared.Models;
using VinhKhanhTour.Shared.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VinhKhanhTour.CMS.Data;

// Workaround for SIGSEGV 139 on Render/Linux during file watching
Environment.SetEnvironmentVariable("DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE", "false");
Environment.SetEnvironmentVariable("DOTNET_USE_POLLING_FILE_WATCHER", "true");

var builder = WebApplication.CreateBuilder(args);

// Ensure it listens to Render's PORT environment variable
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IEmailService, MockEmailService>();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth_token";
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/Error";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    })
    // JWT Bearer scheme cho Mobile Tourist Authentication
    .AddJwtBearer("TouristJwt", options =>
    {
        var jwtSecretKey = builder.Configuration["Jwt:SecretKey"]
            ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
            ?? "VinhKhanhTour-Default-Dev-Key-Change-In-Production-Min32Chars!";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "VinhKhanhTour",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "VinhKhanhTourMobileApp",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
        };

        // Không redirect khi API trả 401 — thay vì redirect về /login
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                // Prevent redirect, return 401 JSON instead
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"error\": \"Unauthorized. Token không hợp lệ hoặc đã hết hạn.\"}");
            }
        };
    });

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Policy cho Tourist API — yêu cầu JWT với claim UserType = Tourist
    options.AddPolicy("TouristOnly", policy =>
        policy.AddAuthenticationSchemes("TouristJwt")
              .RequireAuthenticatedUser()
              .RequireClaim("UserType", "Tourist"));
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
}

// Chuyển đổi định dạng postgresql:// của Render thành định dạng chuẩn cho EF Core
if (!string.IsNullOrEmpty(connectionString) && (connectionString.StartsWith("postgres://") || connectionString.StartsWith("postgresql://")))
{
    var uri = new Uri(connectionString);
    var userInfo = uri.UserInfo.Split(':');
    var builderDb = new Npgsql.NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.IsDefaultPort ? 5432 : uri.Port,
        Username = userInfo.Length > 0 ? userInfo[0] : "",
        Password = userInfo.Length > 1 ? userInfo[1] : "",
        Database = uri.LocalPath.TrimStart('/'),
        SslMode = Npgsql.SslMode.Require
    };
    connectionString = builderDb.ToString();
}
builder.Services.AddDbContextFactory<CmsDbContext>(options =>
    options.UseNpgsql(connectionString), ServiceLifetime.Scoped);

builder.Services.AddSingleton<IErrorHandler, CmsErrorHandler>();
builder.Services.AddScoped<IPoiRepository, PostgresPoiRepository>();
builder.Services.AddScoped<PostgresPoiRepository>();
builder.Services.AddScoped<PoiService>();
builder.Services.AddScoped<TourService>();
builder.Services.AddScoped<AnalyticsService>();
builder.Services.AddScoped<IAppAnalyticsService, AppAnalyticsService>();
builder.Services.AddScoped<GeminiTranslationService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Payment Services
builder.Services.AddHttpClient("MoMo");
builder.Services.AddSingleton<MoMoPaymentService>();
builder.Services.AddSingleton<VNPayPaymentService>();

// Tourist Auth Services
builder.Services.AddScoped<IFirebaseAuthService, MockFirebaseAuthService>(); // ⚠️ Swap sang FirebaseAuthService khi có credentials
builder.Services.AddScoped<ITouristTokenService, TouristTokenService>();

VinhKhanhTour.Shared.Models.Poi.LocalizationService = new VinhKhanhTour.CMS.Services.CmsLocalizationService();

var app = builder.Build();

// Auto-migrate and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CmsDbContext>();
    try
    {
        db.Database.Migrate();

        var allPois = db.Pois.IgnoreQueryFilters().ToList();
        if (!allPois.Any())
        {
            var sampleData = Poi.GetSampleData();
            // Force Identity Insert for auto-increment to work properly
            foreach (var poi in sampleData) { poi.Id = 0; }
            db.Pois.AddRange(sampleData);
            db.SaveChanges();
        }

        // Đảm bảo chỉ mục ID liên tục cập nhật trên PostgreSQL (Fix the Insert error)
        if (db.Database.IsNpgsql())
        {
            var maxId = db.Pois.IgnoreQueryFilters().Max(p => (int?)p.Id) ?? 0;
            if (maxId > 0)
            {
                db.Database.ExecuteSqlRaw("SELECT setval(pg_get_serial_sequence('\"Pois\"', 'Id'), {0});", maxId);
            }
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found");

var isRender = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RENDER"));
if (!isRender)
{
    app.UseHttpsRedirection();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
});

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<VinhKhanhTour.CMS.Middleware.UserActivityMiddleware>();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
