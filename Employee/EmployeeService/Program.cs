using Common;
using Common.Queue;
using EmployeeService.Data;
using EmployeeService.Queue;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        builder.Services.AddDbContext<EmployeeDbContext>(
            options => options.UseSqlite("Data Source = EmployeeService.db")
        );
        builder.Services
            .AddRabbitMqSettings(configuration)
            .AddSingleton<IQueueHandler, RabbitMqHandler>()
            .AddScoped<IRabbitMqScopedService, RabbitMqScopedService>()
            .AddHostedService<RabbitMqService>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
            dbContext.Database.EnsureCreated();

            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
