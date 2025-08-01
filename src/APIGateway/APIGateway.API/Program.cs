using Microsoft.AspNetCore.RateLimiting;
using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

// ��������� YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// �������� CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowAnyOrigin();
    });
});

// ��������� Rate Limiting (�� �������)
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
