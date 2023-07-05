using RabbitMQ.Client;

namespace Common.Queue;

public interface IQueueHandler
{
    Task PublishMessage(string key, string data);
    Task ReadMessage(IModel channel,CancellationToken cancellationToken);
}