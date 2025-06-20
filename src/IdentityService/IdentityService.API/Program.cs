using IdentityService.Application.Interfaces;
using IdentityService.Infrastructure.Persistence;
using IdentityService.Infrastructure.Repositories;
using IdentityService.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using IdentityService.Application.Validators;
using Serilog;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// JWT config
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
        };
    });

// Serilog config
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ========== ONLY THIS FOR OPENAPI/SCALAR ==========
builder.Services.AddOpenApi();         // <-- вместо AddSwaggerGen/SwaggerUI
// ================================================

builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IProfileServiceClient, ProfileServiceStub>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        Assembly.Load("IdentityService.Application")
    );
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DeathConfirmedOnly", policy =>
        policy.RequireClaim("DeathConfirmed", "true"));
    options.AddPolicy("AccessAnytimeOnly", policy =>
        policy.RequireClaim("AccessMode", "Anytime"));
});

builder.Services.AddDbContext<MemoriesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ======== Монтируем OpenAPI и Scalar UI ========
if (app.Environment.IsDevelopment())
{
    // OpenAPI JSON: /openapi/v1.json
    app.MapOpenApi();

    // Scalar UI: http://localhost:5035/scalar/v1
    app.MapScalarApiReference();
}
// ==============================================

app.Run();
