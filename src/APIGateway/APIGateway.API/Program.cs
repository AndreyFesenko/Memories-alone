//C: \Users\user\Source\Repos\Memories-alone\src\APIGateway\APIGateway.API\Program.cs
using Microsoft.AspNetCore.RateLimiting;
using Scalar.AspNetCore;
using Serilog;
using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

// Подключаем Serilog с выводом в консоль
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // уровень логирования
    .WriteTo.Console()    // вывод в консоль
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Разрешаем все CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowAnyOrigin();
    });
});

// Rate Limiting
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
app.MapReverseProxy();

// Health endpoints
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

// Подключаем статику
app.UseDefaultFiles();
app.UseStaticFiles();

// Scalar UI + OpenAPI
if (app.Environment.IsDevelopment() || true)
{
    app.MapOpenApi("/openapi.json");

    app.MapScalarApiReference(options =>
    {
        options.Title = "Memories API Gateway";
    });
}

app.Run();
