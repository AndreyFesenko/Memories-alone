using ModerationService.Repositories;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSingleton<IModerationRepository, InMemoryModerationRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "ModerationService", Version = "v1" }); });
var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseDeveloperExceptionPage(); app.UseSwagger(); app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ModerationService v1")); }
app.UseRouting(); app.UseAuthorization(); app.MapControllers(); app.Run();
