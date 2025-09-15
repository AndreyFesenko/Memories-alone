// src/IdentityService/IdentityService.API/Program.cs
using System.Text;
using System.Text.Json.Serialization;
using IdentityService.Application;
using IdentityService.Infrastructure;
using IdentityService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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
var key = jwt["Key"] ?? "super-secret-dev-key-change-it";
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

// ---- Controllers + JSON ----
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(); // только JSON, без SwaggerUI

// ---- CORS (dev) ----
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p => p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

// ---- AuthN/AuthZ ----
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false; // локально
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DeathConfirmedOnly", policy => policy.RequireClaim("DeathConfirmed", "true"));
    options.AddPolicy("AccessAnytimeOnly", policy => policy.RequireClaim("AccessMode", "Anytime"));
});

// ---- DI из слоёв ----
builder.Services.AddApplicationServices(cfg);
builder.Services.AddInfrastructureServices(cfg);

// (необязательно) миграции при старте — если нужно
builder.Services.AddHostedService<DbInitHostedService>();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseCors("AllowAll");

// порядок важен: AuthN -> AuthZ
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health (минимально, без внешних пакетов)
app.MapGet("/health/live", () => Results.Ok(new { status = "Live" }));
app.MapGet("/health/ready", async (MemoriesDbContext db) =>
{
    // Быстрый ping к БД как readiness
    var canConnect = await db.Database.CanConnectAsync();
    return canConnect ? Results.Ok(new { status = "Ready" }) : Results.StatusCode(503);
});

// OpenAPI JSON + Scalar (если хочешь, можно включить всегда)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi.json");
    app.MapScalarApiReference(options =>
    {
        options.Title = "Identity Service";
        // Scalar сам возьмёт /openapi.json по умолчанию
    });
}

app.Run();

/// <summary>
/// Опционально: инициализация БД/миграции при старте приложения.
/// </summary>
public sealed class DbInitHostedService : IHostedService
{
    private readonly IServiceProvider _sp;

    public DbInitHostedService(IServiceProvider sp) => _sp = sp;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MemoriesDbContext>();
        await db.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
