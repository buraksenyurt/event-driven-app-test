namespace Common.ApiClient.Insurance;

public interface IInsuranceApiHandler
{
    Task<ICollection<Contract>> GetContractsAsync();
    Task CreateContractAsync(Contract contract);
}