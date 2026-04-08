using VinhKhanhTour.CMS.Components;
using VinhKhanhTour.CMS.Services;
using VinhKhanhTour.Shared.Data;
using VinhKhanhTour.Shared.Models;
using VinhKhanhTour.Shared.Services;

using Microsoft.EntityFrameworkCore;
using VinhKhanhTour.CMS.Data;

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

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth_token";
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/Error";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });
builder.Services.AddAuthorization();

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
        SslMode = Npgsql.SslMode.Require,
        TrustServerCertificate = true
    };
    connectionString = builderDb.ToString();
}
builder.Services.AddDbContext<CmsDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddSingleton<IErrorHandler, CmsErrorHandler>();
builder.Services.AddScoped<IPoiRepository, PostgresPoiRepository>();
builder.Services.AddScoped<PoiService>();
builder.Services.AddScoped<TourService>();
builder.Services.AddScoped<AnalyticsService>();
builder.Services.AddScoped<IAppAnalyticsService, AppAnalyticsService>();
builder.Services.AddScoped<GeminiTranslationService>();

VinhKhanhTour.Shared.Models.Poi.LocalizationService = new VinhKhanhTour.CMS.Services.CmsLocalizationService();

var app = builder.Build();

// Auto-migrate and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CmsDbContext>();
    try
    {
        db.Database.Migrate();

        if (!db.Pois.Any())
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
            var maxId = db.Pois.Max(p => (int?)p.Id) ?? 0;
            if (maxId > 0)
            {
                db.Database.ExecuteSqlRaw($"SELECT setval(pg_get_serial_sequence('\"Pois\"', 'Id'), {maxId});");
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
app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
