// src/MemoryArchiveService/MemoryArchiveService.API/Program.cs
using System.Text.Json.Serialization;
using Amazon.S3;
using MemoryArchiveService.Application;
using MemoryArchiveService.Infrastructure;
using MemoryArchiveService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ---- Serilog ----
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// ---- MVC / JSON ----
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// multipart ограничения
builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 1024L * 1024 * 200; // 200 MB
    o.ValueLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = 1024 * 64;
});

// CORS (dev)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDev", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// Health + OpenAPI JSON
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(); // /openapi.json

// DI из Application/Infrastructure
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Http logging
builder.Services.AddHttpLogging(o =>
{
    o.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders |
                      HttpLoggingFields.ResponsePropertiesAndHeaders;
    o.RequestBodyLogLimit = 0;
    o.ResponseBodyLogLimit = 0;
});

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseHttpLogging();
app.UseCors("AllowDev");

app.MapControllers();

// Простые health endpoints
app.MapGet("/health/live", () => Results.Ok(new { status = "Live" }));
app.MapGet("/health/ready", async (MemoryArchiveDbContext db) =>
{
    var ok = await db.Database.CanConnectAsync();
    return ok ? Results.Ok(new { status = "Ready" }) : Results.StatusCode(503);
});

// OpenAPI JSON
app.MapOpenApi("/openapi.json");

// Корень -> openapi
app.MapGet("/", () => Results.Redirect("/openapi.json"));

// ---------- Разовый диагностический прогон подключений на старте ----------
_ = Task.Run(async () =>
{
    using var scope = app.Services.CreateScope();
    var sp = scope.ServiceProvider;
    var cfg = sp.GetRequiredService<IConfiguration>();
    await CloudChecks.RunOnceAsync(sp, cfg, CancellationToken.None);
});

app.Run();

/// <summary>
/// Утилиты проверки внешних подключений (консольные логи)
/// </summary>
internal static class CloudChecks
{
    public static async Task RunOnceAsync(IServiceProvider sp, IConfiguration cfg, CancellationToken ct)
    {
        Log.Information("=== Cloud connections check started ===");

        await CheckPostgresAsync(sp, ct);
        await CheckRabbitMqAsync(cfg, ct);
        await CheckSupabaseS3Async(sp, cfg, ct);

        Log.Information("=== Cloud connections check finished ===");
    }

    private static async Task CheckPostgresAsync(IServiceProvider sp, CancellationToken ct)
    {
        try
        {
            var db = sp.GetRequiredService<MemoryArchiveDbContext>();
            var ok = await db.Database.CanConnectAsync(ct);
            Log.Information("Postgres (Supabase): {Status}", ok ? "OK" : "FAILED");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Postgres (Supabase): FAILED");
        }
    }

    private static async Task CheckRabbitMqAsync(IConfiguration cfg, CancellationToken ct)
    {
        try
        {
            var mq = cfg.GetSection("RabbitMq");
            var factory = new ConnectionFactory
            {
                HostName = mq["Host"] ?? "localhost",
                UserName = mq["User"] ?? "guest",
                Password = mq["Password"] ?? "guest",
                Port = int.TryParse(mq["Port"], out var p) ? p : 5672
            };

            await using var conn = await factory.CreateConnectionAsync(clientProvidedName: "memory-archive-health", cancellationToken: ct);
            await using var ch = await conn.CreateChannelAsync();

            // пассивная проверка стандартного обменника
            await ch.ExchangeDeclarePassiveAsync("amq.direct");

            Log.Information("RabbitMQ: OK (host {Host}, port {Port})", factory.HostName, factory.Port);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "RabbitMQ: FAILED");
        }
    }

    private static async Task CheckSupabaseS3Async(IServiceProvider sp, IConfiguration cfg, CancellationToken ct)
    {
        try
        {
            var s3 = sp.GetRequiredService<IAmazonS3>();
            var bucket = cfg["Supabase:S3:Bucket"];

            // легкая операция — запрос локации бакета
            var resp = await s3.GetBucketLocationAsync(bucket, ct);
            Log.Information("Supabase S3: OK (bucket: {Bucket}, location: {Location})",
                bucket, resp.Location?.Value ?? "(unknown)");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Supabase S3: FAILED");
        }
    }
}
