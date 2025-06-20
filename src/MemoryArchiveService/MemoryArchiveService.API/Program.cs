using MemoryArchiveService.Application;
using MemoryArchiveService.Infrastructure;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Scalar + OpenAPI
builder.Services.AddOpenApi();
builder.Services.AddAuthorization();

// Подключаем DI из Application/Infrastructure
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Swagger (по желанию)
// builder.Services.AddSwaggerGen(c => { ... });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();         // отдаёт OpenAPI JSON
    app.MapScalarApiReference(); // Scalar UI по /scalar/v1
    // app.UseSwagger(); app.UseSwaggerUI(); // если нужно
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
