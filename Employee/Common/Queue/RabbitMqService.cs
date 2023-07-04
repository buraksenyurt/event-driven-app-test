using Common.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace Common.Queue;

public class RabbitMqService
    : BackgroundService
{
    private IModel _channel;
    private IConnection _connection;
    private readonly RabbitMqSettings _rabbitMqSettings;
    private readonly IQueue _queue;
    private readonly IServiceProvider _serviceProvider;
    public RabbitMqService(IServiceProvider serviceProvider, IQueue queue, RabbitMqSettings rabbitMqSettings)
    {
        _serviceProvider = serviceProvider;
        _queue = queue;
        _rabbitMqSettings = rabbitMqSettings;
    }
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqSettings.HostName,
            UserName = _rabbitMqSettings.UserName,
            Password = _rabbitMqSettings.Password,
            DispatchConsumersAsync = true
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        return base.StartAsync(cancellationToken);
    }
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var scopedService = scope.ServiceProvider.GetRequiredService<IRabbitMqScopedService>();
        await _queue.ReadMessage(_channel, cancellationToken);
    }
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        _connection.Close();
    }
}