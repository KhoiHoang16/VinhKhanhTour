using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VinhKhanhTour.Shared.Data;
using VinhKhanhTour.Shared.Models;
using VinhKhanhTour.Shared.Services;
using VinhKhanhTour.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Ensure it listens to Render's PORT environment variable
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Configure Database Path to be a local file for the API if not set
var dbFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VinhKhanhTour");
Directory.CreateDirectory(dbFolder);
Constants.DatabasePath = Path.Combine(dbFolder, Constants.DatabaseFilename);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<IErrorHandler, ApiErrorHandler>();
builder.Services.AddSingleton<IPoiRepository, SqlitePoiRepository>();

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var isRender = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RENDER"));
if (!isRender)
{
    app.UseHttpsRedirection();
}

app.UseForwardedHeaders(new Microsoft.AspNetCore.Builder.ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
});

// Map Poi Endpoints
var poiApi = app.MapGroup("/api/poi");

poiApi.MapGet("/", async (IPoiRepository repo) =>
{
    var pois = await repo.GetAllPoisAsync();
    return Results.Ok(pois);
});

poiApi.MapPost("/", async (IPoiRepository repo, Poi poi) => 
{
    await repo.SavePoiAsync(poi);
    return Results.Created($"/api/poi/{poi.Id}", poi);
});

poiApi.MapPut("/{id}", async (int id, IPoiRepository repo, Poi poi) =>
{
    poi.Id = id;
    await repo.SavePoiAsync(poi);
    return Results.NoContent();
});

poiApi.MapDelete("/{id}", async (int id, IPoiRepository repo) =>
{
    var pois = await repo.GetAllPoisAsync();
    var poi = pois.FirstOrDefault(p => p.Id == id);
    if (poi != null)
    {
        await repo.DeletePoiAsync(poi);
        return Results.NoContent();
    }
    return Results.NotFound();
});

app.Run();
