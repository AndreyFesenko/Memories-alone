using Microsoft.AspNetCore.RateLimiting;
using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

// Добавляем YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Включаем CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowAnyOrigin();
    });
});

// Добавляем Rate Limiting (по желанию)
builder.Services.AddRateLimiter(_ =>
{
    _.AddFixedWindowLimiter("global", options =>
    {
        options.PermitLimit = 100;
        options.Window = TimeSpan.FromMinutes(1);
    });
});

var app = builder.Build();

app.UseCors("AllowAll");
app.UseRateLimiter();
app.MapReverseProxy();

app.Run();
