using Common.ApiClient.Insurance;
using Common.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common;
public static class DependencyInjection
    {
        public static IServiceCollection AddRabbitMqSettings(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<RabbitMqSettings>(config.GetSection("RabbitMqSettings"));
            return services;
        }
    }