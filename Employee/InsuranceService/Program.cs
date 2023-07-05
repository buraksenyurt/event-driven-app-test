using Common;
using Common.Queue;
using InsuranceService.Data;
using InsuranceService.Queue;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddDbContext<ContractDbContext>(options =>
    options.UseSqlite("Data Source=ProductService.db"));

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
    var dbContext = scope.ServiceProvider.GetRequiredService<ContractDbContext>();
    dbContext.Database.EnsureCreated();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
