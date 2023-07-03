using RabbitMQ.Client;

namespace EmployeeService.Queue;

public interface IQueue
{
    Task ReadMessage(IModel channel, string key, CancellationToken cancellationToken);
}