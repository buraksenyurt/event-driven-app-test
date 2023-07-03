namespace EmployeeService.Queue;

using EmployeeService.Data;
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
    private IConnection GetConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = "127.0.0.1",
            UserName = "scoth",
            Password = "tiger"
        };

        var connection = factory.CreateConnection();
        return connection;
    }
    public async Task PublishMessage(string key, string data)
    {
        using var channel = GetConnection().CreateModel();

        var body = Encoding.UTF8.GetBytes(data);
        channel.BasicPublish(
            exchange: "sales.exchange",
            routingKey: key,
            basicProperties: null,
            body: body);

        await Task.CompletedTask;
    }

    public async Task ReadMessage(string key, CancellationToken cancellationToken)
    {
        using var channel = GetConnection().CreateModel();
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (model, args) =>
        {
            var body = Encoding.UTF8.GetString(args.Body.ToArray());
            using var scope = _serviceFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
            var data = JsonSerializer.Deserialize<Contract>(body);
            var type = args.RoutingKey;
            if (type == "insurance.contract")
            {
                var contract = await dbContext.Contracts.FirstOrDefaultAsync(c => c.ContractId == data.ContractId, cancellationToken);
                if (contract == null)
                {
                    await dbContext.Contracts.AddAsync(contract, cancellationToken);
                }
                else
                {
                    contract.Title = data.Title;
                    contract.Quantity = data.Quantity;
                }
                await dbContext.SaveChangesAsync(cancellationToken);
                await Task.Delay(new Random().Next(1, 3) * 1000, cancellationToken);
            }

        };
        channel.BasicConsume("insurance.contact", true, consumer);
        await Task.CompletedTask;
    }
}