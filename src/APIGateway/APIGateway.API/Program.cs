using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

// ---------- Serilog в консоль ----------
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// ---------- YARP из appsettings ----------
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// ---------- CORS (пока всё разрешаем) ----------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// ---------- Rate Limiting (глобально, мягко) ----------
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("global", opts =>
    {
        opts.PermitLimit = 200;
        opts.Window = TimeSpan.FromMinutes(1);
    });
});

// ---------- Response Compression ----------
builder.Services.AddResponseCompression();

// ---------- Http Logging (диагностика прокси) ----------
builder.Services.AddHttpLogging(o =>
{
    o.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders |
                      HttpLoggingFields.ResponsePropertiesAndHeaders |
                      HttpLoggingFields.RequestBody |
                      HttpLoggingFields.ResponseBody;
    o.RequestBodyLogLimit = 1024 * 64;
    o.ResponseBodyLogLimit = 1024 * 64;
});

var app = builder.Build();

// ---------- Проксируемые сценарии требуют: ----------
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    // при необходимости можно указать KnownProxies/KnownNetworks
});

app.UseSerilogRequestLogging();
app.UseHttpLogging();
app.UseResponseCompression();
app.UseCors("AllowAll");

// WebSockets (для NotificationService/SignalR через гейтвей)
app.UseWebSockets();

// Rate Limiter до прокси
app.UseRateLimiter();

// ---------- Health ----------
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

// ---------- Прокси ----------
app.MapReverseProxy();

// ---------- Статика (docs) ----------
app.UseDefaultFiles();   // подключает index.html по умолчанию для / и /api-docs, если настроен default file
app.UseStaticFiles();    // раздаёт /wwwroot

// Явный маршрут на /api-docs -> наш single-page тестер
app.MapGet("/api-docs", async context =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync("wwwroot/index.html");
});

// Маршрут на OpenAPI агрегат (статический файл)
app.MapGet("/openapi.json", async context =>
{
    context.Response.ContentType = "application/json; charset=utf-8";
    await context.Response.SendFileAsync("wwwroot/openapi.json");
});

// Полезный fallback на корень
app.MapGet("/", () => Results.Redirect("/api-docs"));

app.Run();
