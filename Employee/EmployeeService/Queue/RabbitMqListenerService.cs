namespace EmployeeService.Queue;

public class RabbitMqListenerService
    : BackgroundService
{
    private readonly IQueue _queue;
    public RabbitMqListenerService(IQueue queue)
    {
        _queue = queue;
    }
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        return base.StartAsync(cancellationToken);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _queue.ReadMessage("insurance.contract", stoppingToken);
    }
}