// C:\_C_Sharp\MyOtus_Prof\Memories_alone\src\APIGateway\APIGateway.API\Program.cs
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Serilog.Events;
using Yarp.ReverseProxy;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---------- Serilog в консоль ----------
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Override("Yarp", LogEventLevel.Warning)
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
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
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

// ---------- Http Logging (без тел, чтобы не ловить бинарь) ----------
builder.Services.AddHttpLogging(o =>
{
    o.LoggingFields =
        HttpLoggingFields.RequestPropertiesAndHeaders |
        HttpLoggingFields.ResponsePropertiesAndHeaders;
    // тела отключены тут — ниже есть мини-middleware для текстовых ответов
    o.RequestBodyLogLimit = 0;
    o.ResponseBodyLogLimit = 0;
});

var app = builder.Build();

// ---------- Проксируемые сценарии требуют: ----------
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    // KnownProxies/KnownNetworks можно добавить при деплое за реальным прокси
});

app.UseSerilogRequestLogging(); // красивый сводный лог по запросам
app.UseHttpLogging();           // заголовки запр/ответа
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

// ---------- (Опционально) логируем ТОЛЬКО текстовые ответы, если они не были сжаты ----------
static bool IsTextContent(string? ct) =>
    !string.IsNullOrWhiteSpace(ct) &&
    (ct.StartsWith("text/", StringComparison.OrdinalIgnoreCase) ||
     ct.Contains("json", StringComparison.OrdinalIgnoreCase) ||
     ct.Contains("xml", StringComparison.OrdinalIgnoreCase) ||
     ct.Contains("html", StringComparison.OrdinalIgnoreCase));

app.Use(async (ctx, next) =>
{
    // пусть downstream сформирует ответ в буфер
    var originalBody = ctx.Response.Body;
    await using var buffer = new MemoryStream();
    ctx.Response.Body = buffer;

    try
    {
        await next();
    }
    finally
    {
        // если ответ НЕ сжат и похож на текст — логируем начало тела (до 4 КБ)
        var hasEncoding = ctx.Response.Headers.ContainsKey("Content-Encoding");
        var ct = ctx.Response.ContentType;

        if (!hasEncoding && IsTextContent(ct))
        {
            buffer.Position = 0;
            using var reader = new StreamReader(buffer, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            var max = Math.Min(body.Length, 4096);
            if (max > 0)
            {
                Log.Information("ResponseBody (first {Len} chars) for {Method} {Path}: {Body}",
                    max, ctx.Request.Method, ctx.Request.Path, body[..max]);
            }
        }

        buffer.Position = 0;
        await buffer.CopyToAsync(originalBody);
        ctx.Response.Body = originalBody;
    }
});

// ---------- Прокси ----------
app.MapReverseProxy();

// ---------- Статика (docs) ----------
app.UseDefaultFiles();   // index.html по умолчанию
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
