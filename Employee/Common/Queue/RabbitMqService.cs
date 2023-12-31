using Common.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Common.Queue;

public class RabbitMqService
    : BackgroundService
{
    private IModel _channel;
    private IConnection _connection;
    private ILogger _logger;
    private readonly RabbitMqSettings _rabbitMqSettings;
    private readonly IQueueHandler _queue;
    private readonly IServiceProvider _serviceProvider;
    public RabbitMqService(IServiceProvider serviceProvider, IQueueHandler queue, IOptions<RabbitMqSettings> rmqSettings, ILogger<RabbitMqService> logger)
    {
        _serviceProvider = serviceProvider;
        _queue = queue;
        _rabbitMqSettings = rmqSettings.Value;
        _logger = logger;
    }
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqSettings.HostName,
            UserName = _rabbitMqSettings.UserName,
            Password = _rabbitMqSettings.Password,
            Port = _rabbitMqSettings.Port,
            DispatchConsumersAsync = true
        };
        _logger.LogWarning($"{factory.Endpoint}, {factory.HostName}, {factory.Port}");
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