// NotificationService.Infrastructure/Services/TemplateRenderer.cs
using HandlebarsDotNet;

namespace NotificationService.Infrastructure.Services;

public class TemplateRenderer : ITemplateRenderer
{
    public string Render(string template, object model)
    {
        var compile = Handlebars.Compile(template);
        return compile(model);
    }
}
