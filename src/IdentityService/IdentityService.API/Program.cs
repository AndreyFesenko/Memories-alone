// src/IdentityService/IdentityService.API/Program.cs
using IdentityService.Application;
using IdentityService.Infrastructure;
using IdentityService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// === Конфиг: только JSON (без ENV/CLI override) ===
builder.Configuration.Sources.Clear();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false);

// ---- Serilog ----
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

// ---- Config ----
var cfg = builder.Configuration;

var jwt = cfg.GetSection("Jwt");
var issuer = jwt["Issuer"] ?? "memories-issuer";
var audience = jwt["Audience"] ?? "memories-audience";
var key = jwt["Key"] ?? "supersecretkey_should_be_env_or_user_secret";
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

// ---- Controllers + JSON ----
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(); // только OpenAPI JSON

// ---- CORS (Dev) ----
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p => p
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin());
});

// ---- AuthN/AuthZ ----
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true,

            // ВАЖНО: говорим, какие клеймы считать "именем" и "ролью"
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DeathConfirmedOnly", p => p.RequireClaim("DeathConfirmed", "true"));
    options.AddPolicy("AccessAnytimeOnly", p => p.RequireClaim("AccessMode", "Anytime"));
});

// ---- DI из ваших слоёв ----
builder.Services.AddApplicationServices(cfg);
builder.Services.AddInfrastructureServices(cfg);

// ---- Миграции при старте ----
builder.Services.AddHostedService<DbInitHostedService>();

// ---- Health ----
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseCors("AllowAll");

// AuthN → AuthZ
app.UseAuthentication();
app.UseAuthorization();

// Контроллеры
app.MapControllers();

// ---- Health endpoints ----
app.MapHealthChecks("/health");
app.MapGet("/health/live", () => Results.Ok(new { status = "Live" }));
app.MapGet("/health/ready", async (MemoriesDbContext db) =>
{
    var ok = await db.Database.CanConnectAsync();
    return ok ? Results.Ok(new { status = "Ready" }) : Results.StatusCode(503);
});

// ---- OpenAPI + Scalar (Dev) ----
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi.json");
    app.MapScalarApiReference(opts =>
    {
        opts.Title = "Identity Service";
    });
}

app.Run();

/// <summary>
/// Инициализация БД/миграции при старте приложения.
/// </summary>
public sealed class DbInitHostedService : IHostedService
{
    private readonly IServiceProvider _sp;
    public DbInitHostedService(IServiceProvider sp) => _sp = sp;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MemoriesDbContext>();
        try
        {
            Console.WriteLine("[DB] Applying migrations for IdentityService...");
            await db.Database.MigrateAsync(cancellationToken);
            Console.WriteLine("[DB] Migrations applied.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("[DB] Migration failed: " + ex.Message);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
