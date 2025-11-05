// src/APIGateway/APIGateway.API/Program.cs
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Serilog.Events;
using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

// === Конфиг: ТОЛЬКО JSON (без ENV/CLI override) ===
builder.Configuration.Sources.Clear();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false);

// Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Override("Yarp", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// CORS
builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// Rate limiting
builder.Services.AddRateLimiter(o =>
{
    o.AddFixedWindowLimiter("global", opts =>
    {
        opts.PermitLimit = 200;
        opts.Window = TimeSpan.FromMinutes(1);
    });
});

// Compression
builder.Services.AddResponseCompression();

// Http logging (без тел)
builder.Services.AddHttpLogging(o =>
{
    o.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders |
                      HttpLoggingFields.ResponsePropertiesAndHeaders;
    o.RequestBodyLogLimit = 0;
    o.ResponseBodyLogLimit = 0;
});

// Health-check для Render
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseSerilogRequestLogging();
app.UseHttpLogging();
app.UseResponseCompression();
app.UseCors("AllowAll");
app.UseWebSockets();
app.UseRateLimiter();

// Статика (до прокси, чтобы /, /api-docs, /openapi.json и ассеты отдавались локально)
app.UseDefaultFiles();
app.UseStaticFiles();

// Прокси
app.MapReverseProxy();

// Health endpoints
app.MapGet("/health/live", () => Results.Ok(new { status = "Live" }));
app.MapGet("/health/ready", () => Results.Ok(new { status = "Ready" }));
app.MapHealthChecks("/health");

// Явные маршруты для удобства
app.MapGet("/api-docs", async ctx =>
{
    ctx.Response.ContentType = "text/html; charset=utf-8";
    await ctx.Response.SendFileAsync("wwwroot/index.html");
});

app.MapGet("/openapi.json", async ctx =>
{
    ctx.Response.ContentType = "application/json; charset=utf-8";
    await ctx.Response.SendFileAsync("wwwroot/openapi.json");
});

app.MapGet("/", () => Results.Redirect("/api-docs"));

app.Run();
