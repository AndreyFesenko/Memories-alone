using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Services;

public class RabbitMqOptions
{
    public string Host { get; set; } = "localhost";
    public string User { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public int? Port { get; set; } = 5672;
    public string Exchange { get; set; } = "notifications";
    public string Queue { get; set; } = "notifications.queue";
    public string DeadLetterExchange { get; set; } = "notifications.dlx";
    public string DeadLetterQueue { get; set; } = "notifications.dlq";
}
