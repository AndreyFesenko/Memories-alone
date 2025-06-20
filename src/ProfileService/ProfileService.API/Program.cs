using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ProfileService.Application;
using ProfileService.Application.Interfaces;
using ProfileService.Infrastructure;
using ProfileService.Infrastructure.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Application & Infrastructure DI
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Controllers
builder.Services.AddControllers();

// OpenAPI Generation (Swagger)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "ProfileService API", Version = "v1" });
});

// Scalar/OpenApi
builder.Services.AddOpenApi(); // Для генерации openapi.json
builder.Services.AddAuthorization();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Сервис отдаёт OpenAPI json по пути /openapi/v1.json
    app.UseSwagger(options => options.RouteTemplate = "/openapi/{documentName}.json");
    app.MapOpenApi(); // JSON OpenAPI по маршруту
    app.MapScalarApiReference(); // UI Scalar по http://localhost:5228/scalar/v1
}

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
