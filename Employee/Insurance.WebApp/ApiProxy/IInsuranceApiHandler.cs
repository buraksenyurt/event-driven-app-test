namespace Insurance.WebApp;

public interface IInsuranceApiHandler
{
    Task<ICollection<Contract>> GetContractsAsync();
    Task CreateContractAsync(Contract contract);
}