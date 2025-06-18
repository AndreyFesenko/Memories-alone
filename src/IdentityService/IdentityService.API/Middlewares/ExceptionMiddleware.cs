using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access");
            await WriteError(context, HttpStatusCode.Unauthorized, "Неверный логин или пароль");
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Not found");
            await WriteError(context, HttpStatusCode.NotFound, "Ресурс не найден");
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed");
            await WriteError(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning(ex, "FluentValidation failed");
            await WriteError(context, HttpStatusCode.BadRequest, string.Join("; ", ex.Errors.Select(e => e.ErrorMessage)));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Bad argument");
            await WriteError(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteError(context, HttpStatusCode.InternalServerError, "Произошла внутренняя ошибка на сервере");
        }
    }

    private async Task WriteError(HttpContext context, HttpStatusCode code, string message)
    {
        context.Response.StatusCode = (int)code;
        context.Response.ContentType = "application/json";
        var json = JsonSerializer.Serialize(new { error = message });
        await context.Response.WriteAsync(json);
    }
}
