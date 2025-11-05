// src/PublicViewService/Program.cs
using Microsoft.OpenApi.Models;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// ---- Конфиг: ищем appsettings и в текущем каталоге, и в подпапке PublicViewService.API ----
builder.Configuration.Sources.Clear();

var env = builder.Environment;
var basePath = env.ContentRootPath;

var appsettingsRoot = Path.Combine(basePath, "appsettings.json");
var appsettingsEnvRoot = Path.Combine(basePath, $"appsettings.{env.EnvironmentName}.json");

var apiDir = "PublicViewService.API";
var appsettingsApi = Path.Combine(basePath, apiDir, "appsettings.json");
var appsettingsEnvApi = Path.Combine(basePath, apiDir, $"appsettings.{env.EnvironmentName}.json");

// хотя бы один базовый appsettings.json должен существовать
if (!File.Exists(appsettingsRoot) && !File.Exists(appsettingsApi))
    throw new FileNotFoundException(
        $"appsettings.json not found. Looked in:\n - {appsettingsRoot}\n - {appsettingsApi}");

var cfgBuilder = new ConfigurationBuilder().SetBasePath(basePath);

// базовый JSON
if (File.Exists(appsettingsRoot))
    cfgBuilder.AddJsonFile(appsettingsRoot, optional: false, reloadOnChange: false);
else
    cfgBuilder.AddJsonFile(appsettingsApi, optional: false, reloadOnChange: false);

// средовой JSON (опционально)
if (File.Exists(appsettingsEnvRoot))
    cfgBuilder.AddJsonFile(appsettingsEnvRoot, optional: true, reloadOnChange: false);
else if (File.Exists(appsettingsEnvApi))
    cfgBuilder.AddJsonFile(appsettingsEnvApi, optional: true, reloadOnChange: false);

// применяем собранную конфигурацию
builder.Configuration.AddConfiguration(cfgBuilder.Build());

// ---- Сервисы ----
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PublicViewService", Version = "v1" });
});
builder.Services.AddHealthChecks();

var app = builder.Build();

// Swagger только в Dev
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

// Health-check для Render
app.MapHealthChecks("/health");

// Диагностический эндпоинт
app.MapGet("/publicview/api/info", () => Results.Ok(new { message = "Public view ok" }));

app.Run();
