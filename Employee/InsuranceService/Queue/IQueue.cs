using RabbitMQ.Client;

namespace InsuranceService.Queue;

public interface IQueue
{
    Task PublishMessage(string key, string data);
    Task ReadMessage(IModel channel, string key, CancellationToken cancellationToken);
}