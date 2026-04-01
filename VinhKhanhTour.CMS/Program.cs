using VinhKhanhTour.CMS.Components;
using VinhKhanhTour.CMS.Services;
using VinhKhanhTour.Shared.Data;
using VinhKhanhTour.Shared.Models;
using VinhKhanhTour.Shared.Services;

using Microsoft.EntityFrameworkCore;
using VinhKhanhTour.CMS.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CmsDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddSingleton<IErrorHandler, CmsErrorHandler>();
builder.Services.AddScoped<IPoiRepository, PostgresPoiRepository>();
builder.Services.AddScoped<PoiService>();
builder.Services.AddScoped<TourService>();
builder.Services.AddScoped<AnalyticsService>();
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
            db.Pois.AddRange(Poi.GetSampleData());
            db.SaveChanges();
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
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
