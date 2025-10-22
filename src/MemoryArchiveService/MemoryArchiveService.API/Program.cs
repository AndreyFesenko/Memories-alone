using System.Text.Json.Serialization;
using MemoryArchiveService.Application;
using MemoryArchiveService.Infrastructure;
using MemoryArchiveService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// MVC / JSON
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

// DI: Application + Infrastructure
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Auth
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = false,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
        options.MapInboundClaims = false;
    });

builder.Services.AddAuthorization();

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

// *** ВАЖНО: аутентификация/авторизация должны быть ДО MapControllers ***
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Простые health endpoints
app.MapGet("/health/live", () => Results.Ok(new { status = "Live" }));
app.MapGet("/health/ready", async (MemoryArchiveDbContext db) =>
{
    var ok = await db.Database.CanConnectAsync();
    return ok ? Results.Ok(new { status = "Ready" }) : Results.StatusCode(503);
});

// OpenAPI JSON + редирект
app.MapOpenApi("/openapi.json");
app.MapGet("/", () => Results.Redirect("/openapi.json"));

// Диагностика внешних подключений (асинхронно, без блокировки старта)
_ = Task.Run(async () =>
{
    using var scope = app.Services.CreateScope();
    var sp = scope.ServiceProvider;
    var cfg = sp.GetRequiredService<IConfiguration>();
    await CloudChecks.RunOnceAsync(sp, cfg, CancellationToken.None);
});

app.Run();

internal static class CloudChecks
{
    public static async Task RunOnceAsync(IServiceProvider sp, IConfiguration cfg, CancellationToken ct)
    {
        Log.Information("=== Cloud connections check started ===");

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

        try
        {
            var s3 = sp.GetRequiredService<Amazon.S3.IAmazonS3>();
            var bucket = cfg["Supabase:S3:Bucket"];
            var resp = await s3.GetBucketLocationAsync(bucket, ct);
            var loc = resp.Location?.Value ?? "";
            Log.Information("Supabase S3: OK (bucket: {Bucket}, location: {Location})", bucket, loc);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Supabase S3: FAILED");
        }

        Log.Information("=== Cloud connections check finished ===");
    }
}
