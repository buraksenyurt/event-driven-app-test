namespace InsuranceService.Queue;

using InsuranceService.Data;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Common.Queue;
using Common.Config;
using Microsoft.Extensions.Options;

public class RabbitMqHandler
    : IQueueHandler
{
    private readonly IServiceScopeFactory _serviceFactory;
    private readonly RabbitMqSettings _rmqSettings;
    private readonly ILogger _logger;
    public RabbitMqHandler(IServiceScopeFactory serviceFactory, IOptions<RabbitMqSettings> rmqSettings, ILogger<RabbitMqHandler> logger)
    {
        _serviceFactory = serviceFactory;
        _rmqSettings = rmqSettings.Value;
        _logger = logger;
    }
    public async Task PublishMessage(string key, string data)
    {
        var factory = new ConnectionFactory
        {
            HostName = _rmqSettings.HostName,
            UserName = _rmqSettings.UserName,
            Password = _rmqSettings.Password,
            Port = _rmqSettings.Port
        };
        _logger.LogWarning($"{factory.Endpoint}, {factory.HostName}, {factory.Port}");

        var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        var body = Encoding.UTF8.GetBytes(data);
        _logger.LogWarning($"Exchange Topic {_rmqSettings.TopicExchange}, Routing Key {key}, Body : {body}");
        channel.BasicPublish(
            exchange: _rmqSettings.TopicExchange,
            routingKey: key,
            basicProperties: null,
            body: body);

        await Task.CompletedTask;
    }

    public async Task ReadMessage(IModel channel, CancellationToken cancellationToken)
    {
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (model, args) =>
        {
            var body = Encoding.UTF8.GetString(args.Body.ToArray());
            _logger.LogWarning($"Gelen mesaj {body}");
            using var scope = _serviceFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ContractDbContext>();
            var data = JsonSerializer.Deserialize<Contract>(body);
            _logger.LogWarning($"Data {data}");
            var type = args.RoutingKey;
            if (type == _rmqSettings.EmployeeKey)
            {
                var contract = await dbContext.Contracts.FirstAsync(c => c.ContractId == data.ContractId);
                _logger.LogWarning($"Contract {contract.ContractId}");
                if (data.Quantity <= contract.Quantity)
                {
                    contract.Quantity = contract.Quantity - data.Quantity;
                }
                await dbContext.SaveChangesAsync(cancellationToken);
                var updated = JsonSerializer.Serialize<Contract>(contract);
                await PublishMessage(_rmqSettings.ContractKey, updated);
            }
        };
        channel.BasicConsume(_rmqSettings.EmployeeKey, true, consumer);
        await Task.CompletedTask;
    }
}