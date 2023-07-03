namespace EmployeeService.Queue;

public interface IQueue
{
    Task PublishMessage(string key, string data);
    Task ReadMessage(string key, CancellationToken cancellationToken);
}