using RabbitMQ.Client;

namespace EmployeeService.Queue;

public class RabbitMqListenerService
    : BackgroundService
{
    private readonly IQueue _queue;
    private IModel _channel;
    private IConnection _connection;
    public RabbitMqListenerService(IQueue queue)
    {
        _queue = queue;
    }
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = "127.0.0.1",
            UserName = "scoth",
            Password = "tiger",
            DispatchConsumersAsync = true // Bu özelliği etkinleştirmezsek, topic'leri dinyelen async operasyonlar işletilmez
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        return base.StartAsync(cancellationToken);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _queue.ReadMessage(_channel, "insurance.contract", stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        _connection.Close();
    }
}