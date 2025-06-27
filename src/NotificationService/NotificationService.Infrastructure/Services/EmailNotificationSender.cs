using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using System.Net.Mail;
using Microsoft.Extensions.Logging;

namespace NotificationService.Infrastructure.Services;

public class EmailNotificationSender : IEmailSender
{
    private readonly ILogger<EmailNotificationSender> _logger;

    public EmailNotificationSender(ILogger<EmailNotificationSender> logger)
    {
        _logger = logger;
    }

    public async Task SendAsync(NotificationMessage message, CancellationToken ct = default)
    {
        try
        {
            using var mail = new MailMessage("noreply@memories.com", message.Recipient, message.Subject, message.Body);
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
