// src/QRCodeService/Program.cs
using Microsoft.OpenApi.Models;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// ---- Конфиг: ищем appsettings и в текущем каталоге, и в подпапке QRCodeService.API ----
builder.Configuration.Sources.Clear();

var env = builder.Environment;
var basePath = env.ContentRootPath;

// кандидаты путей
var appsettingsRoot = Path.Combine(basePath, "appsettings.json");
var appsettingsEnvRoot = Path.Combine(basePath, $"appsettings.{env.EnvironmentName}.json");
var apiDir = "QRCodeService.API";
var appsettingsApi = Path.Combine(basePath, apiDir, "appsettings.json");
var appsettingsEnvApi = Path.Combine(basePath, apiDir, $"appsettings.{env.EnvironmentName}.json");

// проверка наличия
if (!File.Exists(appsettingsRoot) && !File.Exists(appsettingsApi))
    throw new FileNotFoundException(
        $"appsettings.json not found. Looked in:\n - {appsettingsRoot}\n - {appsettingsApi}");

var configBuilder = new ConfigurationBuilder()
    .SetBasePath(basePath);

// базовый JSON
if (File.Exists(appsettingsRoot))
    configBuilder.AddJsonFile(appsettingsRoot, optional: false, reloadOnChange: false);
else
    configBuilder.AddJsonFile(appsettingsApi, optional: false, reloadOnChange: false);

// средовой JSON
if (File.Exists(appsettingsEnvRoot))
    configBuilder.AddJsonFile(appsettingsEnvRoot, optional: true, reloadOnChange: false);
else if (File.Exists(appsettingsEnvApi))
    configBuilder.AddJsonFile(appsettingsEnvApi, optional: true, reloadOnChange: false);

// применяем конфигурацию
var configuration = configBuilder.Build();
builder.Configuration.AddConfiguration(configuration);

// ---- Опции ----
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.Configure<QrCodeOptions>(builder.Configuration.GetSection("QrCode"));

// ---- Базовые сервисы ----
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "QRCodeService", Version = "v1" });
});
builder.Services.AddHealthChecks();

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

// ---- Health-check для Render ----
app.MapHealthChecks("/health");

// ---- Диагностический эндпоинт ----
app.MapGet("/qrcode/api/info", () => Results.Ok(new { message = "QR code service ok" }));

app.Run();


// ===== Опции =====
public sealed class ConnectionStrings
{
    public string Default { get; set; } = string.Empty;
}

public sealed class QrCodeOptions
{
    public int DefaultSize { get; set; } = 512;        // px
    public string ErrorCorrection { get; set; } = "M"; // L/M/Q/H
    public int Margin { get; set; } = 2;               // quiet zone
}
