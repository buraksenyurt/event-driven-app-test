namespace Common.Config;

public class RabbitMqSettings
{
    public string HostName { get; set; }
    public int Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string ContractKey { get; set; }
    public string EmployeeKey { get; set; }
    public string TopicExchange { get; set; }

}