// src/MemoryArchiveService/MemoryArchiveService.Infrastructure/Services/RabbitMqEventBus.cs
using MemoryArchiveService.Application.Interfaces;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text.Json;

namespace MemoryArchiveService.Infrastructure.Services;

/// <summary>
/// Event Bus for publishing integration events using RabbitMQ (Fanout pattern)
/// </summary>
public class RabbitMqEventBus : IEventBus, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
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

        // Асинхронная версия, но ждем результат синхронно — для конструктора
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(_exchange, ExchangeType.Fanout, durable: true);
    }

    public Task PublishAsync<T>(T @event, CancellationToken ct = default) where T : class
    {
        var body = JsonSerializer.SerializeToUtf8Bytes(@event);

        var props = _channel.CreateBasicProperties();
        props.ContentType = "application/json";
        props.DeliveryMode = 2; // persistent

        _channel.BasicPublish(
            exchange: _exchange,
            routingKey: "",
            basicProperties: props,
            body: body);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        GC.SuppressFinalize(this);
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
    public string? Exchange { get; set; } = "memory-events";
}
