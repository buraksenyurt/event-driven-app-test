namespace InsuranceService.Queue;

using RabbitMQ.Client;
using System.Text;
public class RabbitQueue
    : IQueue
{
    public async Task PublishMessage(string key, string data)
    {
        var factory = new ConnectionFactory
        {
            HostName = "127.0.0.1",
            UserName = "scoth",
            Password = "tiger"
        };

        var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        var body = Encoding.UTF8.GetBytes(data);
        channel.BasicPublish(
            exchange: "sales.exchange",
            routingKey: key,
            basicProperties: null,
            body: body);

        await Task.CompletedTask;
    }
}