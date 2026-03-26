using VinhKhanhTour.CMS.Components;
using VinhKhanhTour.CMS.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register HttpClient for PoiService
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5201") });
builder.Services.AddScoped<PoiService>();

VinhKhanhTour.Shared.Models.Poi.LocalizationService = new VinhKhanhTour.CMS.Services.CmsLocalizationService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

