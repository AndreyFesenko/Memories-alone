using RabbitMQ.Client;

public class RabbitTest
{
    public void Foo()
    {
        var factory = new ConnectionFactory();
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.ExchangeDeclare("test", ExchangeType.Fanout);
    }
}
