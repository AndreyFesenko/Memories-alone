// src/AccessControlService/AccessControlService.API/Program.cs
using AccessControlService.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// === Конфигурация: ТОЛЬКО JSON (без ENV/CLI override) ===
builder.Configuration.Sources.Clear();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false);

// MVC
builder.Services.AddControllers();

// Health-check для Render
builder.Services.AddHealthChecks();

// === JWT AUTH ===
var jwt = builder.Configuration.GetSection("Jwt");
var issuer = jwt["Issuer"] ?? "Memories.Identity";
var audience = jwt["Audience"] ?? "Memories.Users";
var signingKey = jwt["SigningKey"] ?? throw new InvalidOperationException("Jwt:SigningKey is missing");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // dev
        options.SaveToken = true;
        options.IncludeErrorDetails = true;   // подробности на 401/403 в логах

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,

            ValidateAudience = true,
            ValidAudience = audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2),

            // нормализованные типы клеймов
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.NameIdentifier
        };

        // Подробные логи + нормализация ролей в lowercase
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                Console.WriteLine($"[JWT][AuthN Failed] {ctx.Exception.GetType().Name}: {ctx.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = ctx =>
            {
                var principal = ctx.Principal;
                Console.WriteLine("[JWT][Token Validated]");
                Console.WriteLine($"  IsAuth = {principal?.Identity?.IsAuthenticated}, Name = {principal?.Identity?.Name}");
                Console.WriteLine("  Claims:");
                foreach (var c in principal?.Claims ?? Enumerable.Empty<Claim>())
                    Console.WriteLine($"    {c.Type} = {c.Value}");

                // Дублируем все роли в нижнем регистре => политики с "admin"/"access_manager" всегда совпадут
                if (principal is not null)
                {
                    var add = new ClaimsIdentity();
                    var current = principal.Claims
                        .Where(c => c.Type == ClaimTypes.Role || c.Type.Equals("role", StringComparison.OrdinalIgnoreCase))
                        .Select(c => c.Value)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    foreach (var r in current)
                    {
                        var lower = r.ToLowerInvariant();
                        // если точного такого role-клейма нет — добавим
                        if (!principal.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == lower))
                            add.AddClaim(new Claim(ClaimTypes.Role, lower));
                    }

                    if (add.Claims.Any())
                    {
                        principal.AddIdentity(add);
                        Console.WriteLine($"  Roles (normalized add): {string.Join(",", add.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value))}");
                    }

                    var effectiveRoles = principal.Claims
                        .Where(c => c.Type == ClaimTypes.Role || c.Type.Equals("role", StringComparison.OrdinalIgnoreCase))
                        .Select(c => c.Value);
                    Console.WriteLine($"  Roles (effective): {string.Join(",", effectiveRoles)}");
                }

                return Task.CompletedTask;
            },
            OnChallenge = ctx =>
            {
                Console.WriteLine($"[JWT][Challenge] error={ctx.Error} desc={ctx.ErrorDescription}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    // Политики в lowercase (совпадут с нормализованными ролями)
    options.AddPolicy("AdminsOnly", p => p.RequireRole("admin"));
    options.AddPolicy("AccessManagers", p => p.RequireRole("access_manager"));
});

builder.Services.AddCors(o =>
{
    o.AddPolicy("Dev", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// OpenAPI/Scalar + инфраструктура
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddAccessInfrastructure(builder.Configuration);

var app = builder.Build();

// ===== pipeline =====
app.UseRouting();
app.UseCors("Dev");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();            // /openapi.json
    app.MapScalarApiReference(); // /scalar
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AccessControlService.Infrastructure.AccessDbContext>();
    try
    {
        Console.WriteLine("[DB] Applying migrations for AccessControlService...");
        await db.Database.MigrateAsync();
        Console.WriteLine("[DB] Migrations applied.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("[DB] Migrate failed: " + ex.Message);
        throw;
    }
}

// Порядок важен: аутентификация -> авторизация -> контроллеры
app.UseAuthentication();
app.UseAuthorization();

// Health endpoint для Render
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();
