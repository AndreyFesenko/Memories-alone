// NotificationService.Infrastructure/Services/TemplateRenderer.cs
using HandlebarsDotNet;
using NotificationService.Application.Interfaces;

namespace NotificationService.Infrastructure.Services;

public class TemplateRenderer : ITemplateRenderer
{
    private readonly object _lock = new();

    public string Render(string template, object model)
    {
        if (string.IsNullOrWhiteSpace(template))
            return string.Empty;

        lock (_lock) // Handlebars compile is not thread-safe
        {
            var compiled = Handlebars.Compile(template);
            return compiled(model);
        }
    }
}
