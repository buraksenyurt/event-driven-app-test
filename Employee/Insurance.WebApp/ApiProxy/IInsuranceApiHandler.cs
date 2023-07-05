namespace Insurance.WebApp;

public interface IInsuranceApiHandler
{
    Task<ICollection<Contract>> GetContractsAsync();
    Task CreateContract(Contract contract);
}