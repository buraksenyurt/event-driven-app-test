using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Config;

public class RabbitMqSettings
{
    public string InstanceName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string ContractKey { get; set; }
    public string EmployeeKey { get; set; }
    public string TopicExchange { get; set; }

}
public static class SettingsExtension
{
    public static IServiceCollection UseRabbitMqSettings(
        this IServiceCollection services)
    {
        services.AddSingleton(ReadConfig());

        return services;
    }

    private static RabbitMqSettings? ReadConfig()
    {
        var configFile = File
                    .ReadAllText($"{Path.GetDirectoryName(
                             Assembly.GetExecutingAssembly().Location)}/appsettings.json");

        var jsonSerializeOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        jsonSerializeOptions.Converters.Add(new JsonStringEnumConverter());
        return JsonSerializer.Deserialize<RabbitMqSettings>(configFile, jsonSerializeOptions);
    }
}