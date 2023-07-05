namespace Insurance.WebApp;

public interface IInsuranceApiWrapper
{
    Task<ICollection<Contract>> GetContractsAsync();
    Task CreateContract(Contract contract);
}