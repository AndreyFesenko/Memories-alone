// C:\Users\user\Source\Repos\Memories-alone\src\APIGateway\APIGateway.API\Program.cs
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

// Serilog в консоль
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// CORS (разрешаем всё)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowAnyOrigin());
});

// Rate Limiting (по желанию)
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("global", opts =>
    {
        opts.PermitLimit = 100;
        opts.Window = TimeSpan.FromMinutes(1);
    });
});

var app = builder.Build();

app.UseCors("AllowAll");
app.UseRateLimiter();

// Отдаём статику (openapi.json и swagger-ui) ДО прокси
app.UseDefaultFiles();   // ищет index.* в wwwroot и подпапках при прямом заходе на папку
app.UseStaticFiles();

// Health
app.MapGet("/health/live", () =>
{
    Log.Information("Health check: Live");
    return Results.Ok(new { status = "Live" });
});

app.MapGet("/health/ready", () =>
{
    Log.Information("Health check: Ready");
    return Results.Ok(new { status = "Ready" });
});

// Редирект /api-docs -> /api-docs/
app.MapGet("/api-docs", ctx =>
{
    ctx.Response.Redirect("/api-docs/", permanent: false);
    return Task.CompletedTask;
});

// Прокси (последним)
app.MapReverseProxy();

app.Run();
