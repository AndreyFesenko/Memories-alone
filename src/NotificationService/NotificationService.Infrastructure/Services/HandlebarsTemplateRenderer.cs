﻿using HandlebarsDotNet;
using NotificationService.Application.Interfaces;

namespace NotificationService.Infrastructure.Services;

public class HandlebarsTemplateRenderer : ITemplateRenderer
{
    public string Render(string template, object model)
    {
        var compiled = Handlebars.Compile(template);
        return compiled(model);
    }
}
