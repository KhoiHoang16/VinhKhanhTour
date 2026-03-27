using VinhKhanhTour.CMS.Components;
using VinhKhanhTour.CMS.Services;
using VinhKhanhTour.Shared.Data;
using VinhKhanhTour.Shared.Models;
using VinhKhanhTour.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddControllers();

var dbFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VinhKhanhTour");
Directory.CreateDirectory(dbFolder);
Constants.DatabasePath = Path.Combine(dbFolder, Constants.DatabaseFilename);

builder.Services.AddSingleton<IErrorHandler, CmsErrorHandler>();
builder.Services.AddSingleton<PoiRepository>();
builder.Services.AddScoped<PoiService>();
builder.Services.AddScoped<TourService>();
builder.Services.AddScoped<AnalyticsService>();

VinhKhanhTour.Shared.Models.Poi.LocalizationService = new VinhKhanhTour.CMS.Services.CmsLocalizationService();

var app = builder.Build();

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
