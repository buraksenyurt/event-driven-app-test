namespace InsuranceService.Queue;

public interface IQueue
{
    Task PublishMessage(string key, string data);
}