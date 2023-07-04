namespace EmployeeService.Queue;

using Common.Config;
using Common.Queue;
using EmployeeService.Data;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class RabbitQueue
    : IQueue
{
    private readonly IServiceScopeFactory _serviceFactory;
    private readonly RabbitMqSettings _rmqSettings;
    private readonly ILogger _logger;
    public RabbitQueue(IServiceScopeFactory serviceFactory, ILogger<RabbitQueue> logger, IOptions<RabbitMqSettings> rmqSettings)
    {
        _serviceFactory = serviceFactory;
        _logger = logger;
        _rmqSettings = rmqSettings.Value;
    }
    public async Task ReadMessage(IModel channel, CancellationToken cancellationToken)
    {
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (model, args) =>
        {
            var body = Encoding.UTF8.GetString(args.Body.ToArray());
            _logger.LogWarning($"Gelen veri {body}");
            using var scope = _serviceFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
            var data = JsonSerializer.Deserialize<Contract>(body);
            var type = args.RoutingKey;
            _logger.LogWarning($"Routing Key {type}");
            if (type == _rmqSettings.ContractKey)
            {
                _logger.LogInformation($"Contract ID = {data.ContractId}");
                var contract = dbContext.Contracts.FirstOrDefault(c => c.ContractId == data.ContractId);
                _logger.LogInformation($"contract is null= {contract == null}");
                if (contract == null)
                {
                    _logger.LogInformation($"{data.ContractId} sistemde yok. Yeni eklenecek.");
                    var newContract = new Contract
                    {
                        ContractId = data.ContractId,
                        Title = data.Title,
                        Quantity = data.Quantity
                    };
                    await dbContext.Contracts.AddAsync(newContract, cancellationToken);
                }
                else
                {
                    _logger.LogInformation($"{data.ContractId} sistemde var. Bilgileri güncellenecek.");
                    contract.Title = data.Title;
                    contract.Quantity = data.Quantity;
                }
                await dbContext.SaveChangesAsync(cancellationToken);
                await Task.Delay(new Random().Next(1, 3) * 1000, cancellationToken);
            }

        };
        channel.BasicConsume(_rmqSettings.ContractKey, true, consumer);
        await Task.CompletedTask;
    }

    public async Task PublishMessage(string key, string data)
    {
        var factory = new ConnectionFactory
        {
            HostName = _rmqSettings.HostName,
            UserName = _rmqSettings.UserName,
            Password = _rmqSettings.Password
        };

        var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        var body = Encoding.UTF8.GetBytes(data);
        channel.BasicPublish(
            exchange: _rmqSettings.TopicExchange,
            routingKey: key,
            basicProperties: null,
            body: body);

        await Task.CompletedTask;
    }
}