// src/MemoryArchiveService/MemoryArchiveService.Infrastructure/Services/RabbitMqEventBus.cs
using System.Text;
using System.Text.Json;
using MemoryArchiveService.Application.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace MemoryArchiveService.Infrastructure.Services;

public class RabbitMqEventBus : IEventBus, IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly string _exchange;

    public RabbitMqEventBus(IOptions<RabbitMqOptions> options)
    {
        var cfg = options.Value ?? throw new ArgumentNullException(nameof(options));

        _exchange = string.IsNullOrWhiteSpace(cfg.Exchange) ? "memory-events" : cfg.Exchange;

        var factory = BuildFactory(cfg);

        // создаём соединение и канал (async API RabbitMQ.Client 6.x)
        _connection = factory.CreateConnectionAsync(clientProvidedName: "memory-archive-publisher")
                             .GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        // гарантируем, что exchange существует (topic — универсальнее)
        _channel.ExchangeDeclareAsync(_exchange, ExchangeType.Topic, durable: true)
                .GetAwaiter().GetResult();
    }

    private static ConnectionFactory BuildFactory(RabbitMqOptions cfg)
    {
        var factory = new ConnectionFactory
        {
            AutomaticRecoveryEnabled = true
            // ВАЖНО: DispatchConsumersAsync нам не нужен, так как мы не потребляем сообщения здесь
        };

        if (!string.IsNullOrWhiteSpace(cfg.Uri))
        {
            // Предпочтительно: amqps://user:pass@host/vhost
            factory.Uri = new Uri(cfg.Uri);
        }
        else
        {
            factory.HostName = cfg.Host ?? "localhost";
            factory.UserName = cfg.User ?? "guest";
            factory.Password = cfg.Password ?? "guest";
            factory.VirtualHost = string.IsNullOrWhiteSpace(cfg.VirtualHost) ? "/" : cfg.VirtualHost;

            var useTls = cfg.UseTls;
            factory.Port = cfg.Port ?? (useTls ? AmqpTcpEndpoint.UseDefaultPort /* 5671 */ : 5672);
            factory.Ssl = new SslOption
            {
                Enabled = useTls,
                ServerName = factory.HostName
            };
        }

        return factory;
    }

    public async Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : class
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

        var props = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent
        };

        await _channel.BasicPublishAsync(
            exchange: _exchange,
            routingKey: "", // при желании можно указать "memory.created" и т.п.
            mandatory: false,
            basicProperties: props,
            body: body,
            cancellationToken: ct
        );
    }

    public ValueTask DisposeAsync()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}

/// <summary>Опции для RabbitMQ/CloudAMQP.</summary>
public class RabbitMqOptions
{
    public string? Uri { get; set; }

    // Альтернатива вместо Uri:
    public string? Host { get; set; }
    public string? User { get; set; }
    public string? Password { get; set; }
    public int? Port { get; set; }
    public string? VirtualHost { get; set; }

    public string Exchange { get; set; } = "memory-events";
    public string? Queue { get; set; }
    public string? DeadLetterExchange { get; set; }
    public string? DeadLetterQueue { get; set; }
    public bool UseTls { get; set; } = true;
}
