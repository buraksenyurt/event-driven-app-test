namespace InsuranceService.Queue;

using InsuranceService.Data;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class RabbitQueue
    : IQueue
{
    private readonly IServiceScopeFactory _serviceFactory;
    public RabbitQueue(IServiceScopeFactory serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }
    public async Task PublishMessage(string key, string data)
    {
        var factory = new ConnectionFactory
        {
            HostName = "127.0.0.1",
            UserName = "scoth",
            Password = "tiger"
        };

        var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        var body = Encoding.UTF8.GetBytes(data);
        channel.BasicPublish(
            exchange: "sales.exchange",
            routingKey: key,
            basicProperties: null,
            body: body);

        await Task.CompletedTask;
    }

    public async Task ReadMessage(IModel channel, string key, CancellationToken cancellationToken)
    {
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (model, args) =>
        {
            var body = Encoding.UTF8.GetString(args.Body.ToArray());
            using var scope = _serviceFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ContractDbContext>();
            var data = JsonSerializer.Deserialize<Contract>(body);
            var type = args.RoutingKey;
            if (type == "insurance.contract")
            {
                var contract = await dbContext.Contracts.FirstAsync(c => c.ContractId == data.ContractId);
                if (data.Quantity <= contract.Quantity)
                {
                    contract.Quantity = contract.Quantity - data.Quantity;
                }
                await dbContext.SaveChangesAsync(cancellationToken);
                var updated = JsonSerializer.Serialize<Contract>(contract);
                await PublishMessage("insurance.contract", updated);
            }

        };
        channel.BasicConsume("insurance.contract", true, consumer);
        await Task.CompletedTask;
    }
}