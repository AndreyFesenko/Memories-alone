// src/ModerationService/Program.cs
using Microsoft.OpenApi.Models;
using ModerationService.Repositories;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// ---- Конфиг: ищем appsettings и в текущем каталоге, и в подпапке ModerationService.API ----
builder.Configuration.Sources.Clear();

var env = builder.Environment;
var basePath = env.ContentRootPath;

// кандидаты: корень текущего веб-проекта и подпапка ModerationService.API
var appsettingsRoot = Path.Combine(basePath, "appsettings.json");
var appsettingsEnvRoot = Path.Combine(basePath, $"appsettings.{env.EnvironmentName}.json");
var apiDir = "ModerationService.API";
var appsettingsApi = Path.Combine(basePath, apiDir, "appsettings.json");
var appsettingsEnvApi = Path.Combine(basePath, apiDir, $"appsettings.{env.EnvironmentName}.json");

// обязательно должен существовать хотя бы один из appsettings.json
if (!File.Exists(appsettingsRoot) && !File.Exists(appsettingsApi))
    throw new FileNotFoundException(
        $"appsettings.json not found. Looked in:\n - {appsettingsRoot}\n - {appsettingsApi}");

var configBuilder = new ConfigurationBuilder()
    .SetBasePath(basePath);

// базовый appsettings.json
if (File.Exists(appsettingsRoot))
    configBuilder.AddJsonFile(appsettingsRoot, optional: false, reloadOnChange: false);
else
    configBuilder.AddJsonFile(appsettingsApi, optional: false, reloadOnChange: false);

// appsettings.{Environment}.json (опционально)
if (File.Exists(appsettingsEnvRoot))
    configBuilder.AddJsonFile(appsettingsEnvRoot, optional: true, reloadOnChange: false);
else if (File.Exists(appsettingsEnvApi))
    configBuilder.AddJsonFile(appsettingsEnvApi, optional: true, reloadOnChange: false);

// применяем собранную конфигурацию
var configuration = configBuilder.Build();
builder.Configuration.AddConfiguration(configuration);

// ---- Опции ----
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.Configure<ModerationOptions>(builder.Configuration.GetSection("Moderation"));
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));

// ---- Сервисы ----
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ModerationService", Version = "v1" });
});
builder.Services.AddHealthChecks();

// Репозиторий модерации (пока InMemory; позже можно подменить на внешний провайдер)
builder.Services.AddSingleton<IModerationRepository, InMemoryModerationRepository>();

var app = builder.Build();

// Swagger только в Development
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

// Render health-check
app.MapHealthChecks("/health");

// Простой диагностический эндпоинт
app.MapGet("/moderation/api/info", () => Results.Ok(new { message = "Moderation ok" }));

app.Run();

// ===== Вспомогательные классы опций =====
public sealed class ConnectionStrings { public string Default { get; set; } = string.Empty; }
public sealed class ModerationOptions
{
    public string Provider { get; set; } = "Local";
    public string ApiKey { get; set; } = string.Empty;
    public double Threshold { get; set; } = 0.8;
}
public sealed class RabbitMqOptions
{
    public string Host { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Exchange { get; set; } = "notifications";
    public string Queue { get; set; } = "audit-log-queue";
    public string DeadLetterExchange { get; set; } = "notifications.dlx";
    public string DeadLetterQueue { get; set; } = "notifications.dlq";
    public string VirtualHost { get; set; } = "/";
}
