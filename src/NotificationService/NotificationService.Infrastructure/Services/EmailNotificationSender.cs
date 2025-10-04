using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using System.Net.Mail;
using Microsoft.Extensions.Logging;

namespace NotificationService.Infrastructure.Services;


public class EmailNotificationSender : IEmailSender
{
    private readonly ILogger<EmailNotificationSender> _logger;
    private readonly ITemplateRenderer _templateRenderer;

    public EmailNotificationSender(ITemplateRenderer templateRenderer, ILogger<EmailNotificationSender> logger)
    {
        _templateRenderer = templateRenderer;
        _logger = logger;
    }

    // Основной метод интерфейса (без шаблона)
    public async Task SendAsync(NotificationMessage message, CancellationToken ct = default)
    {
        await SendAsync(message, message.Body, new { }, ct);
    }

    //Расширенный метод с шаблоном
    public async Task SendAsync(NotificationMessage message, string template, object data, CancellationToken ct = default)
    {
        try
        {
            var renderedBody = _templateRenderer.Render(template, data);

            using var mail = new MailMessage("noreply@memories.com", message.Recipient ?? throw new ArgumentNullException(nameof(message.Recipient)))
            {
                Subject = message.Subject,
                Body = renderedBody,
                IsBodyHtml = true
            };

            using var smtp = new SmtpClient("localhost"); // TODO: SMTP настройки
            await smtp.SendMailAsync(mail, ct);

            _logger.LogInformation("Email sent to {To} (Subject: {Subject})", message.Recipient, message.Subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", message.Recipient);
            throw;
        }
    }
}
