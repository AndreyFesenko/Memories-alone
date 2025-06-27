// src/MemoryArchiveService/MemoryArchiveService.Infrastructure/Services/RabbitMqEventBus.cs
using MemoryArchiveService.Application.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MemoryArchiveService.Infrastructure.Services;

public class RabbitMqEventBus : IEventBus, IAsyncDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly string _exchange;

    public RabbitMqEventBus(IOptions<RabbitMqOptions> options)
    {
        var config = options.Value ?? throw new ArgumentNullException(nameof(options));
        _exchange = string.IsNullOrWhiteSpace(config.Exchange) ? "memory-events" : config.Exchange;

        var factory = new ConnectionFactory
        {
            HostName = config.Host ?? "localhost",
            UserName = config.User ?? "guest",
            Password = config.Password ?? "guest",
            Port = config.Port ?? AmqpTcpEndpoint.UseDefaultPort
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();


        // Создаём exchange (fanout, durable)
        Task.Run(async () => {
            await _channel.ExchangeDeclareAsync(_exchange, ExchangeType.Fanout, durable: true);
        }).GetAwaiter().GetResult();
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
            routingKey: "",
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

/// <summary>
/// Options for configuring RabbitMQ connection (DI via IOptions)
/// </summary>
public class RabbitMqOptions
{
    public string Host { get; set; } = "localhost";
    public string User { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public int? Port { get; set; }
    public string Exchange { get; set; } = "memory-events";
}
