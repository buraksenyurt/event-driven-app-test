using Common;
using Common.Queue;
using EmployeeService.Data;
using EmployeeService.Queue;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddDbContext<EmployeeDbContext>(
    options => options.UseSqlite("Data Source = EmployeeService.db")
);
builder.Services
    .AddSingleton<IQueue, RabbitQueue>()
    .AddScoped<IRabbitMqScopedService, RabbitMqScopedService>()
    .AddHostedService<RabbitMqService>()
    .AddRabbitMqSettings(configuration);

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
