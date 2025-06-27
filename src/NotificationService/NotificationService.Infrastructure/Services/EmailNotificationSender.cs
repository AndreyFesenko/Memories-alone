using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net;

namespace NotificationService.Infrastructure.Services;

public class EmailNotificationSender : INotificationSender
{
    private readonly SmtpClient _smtp;
    private readonly ILogger<EmailNotificationSender> _logger;

    public EmailNotificationSender(SmtpClient smtp, ILogger<EmailNotificationSender> logger)
    {
        _smtp = smtp;
        _logger = logger;
    }

    public async Task SendAsync(NotificationMessage notification, CancellationToken ct = default)
    {
        try
        {
            var msg = new MailMessage("no-reply@yourdomain.com", notification.Recipient, notification.Subject ?? "(No subject)", notification.Message)
            {
                IsBodyHtml = true
            };
            await _smtp.SendMailAsync(msg, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка отправки email");
            throw;
        }
    }
}
