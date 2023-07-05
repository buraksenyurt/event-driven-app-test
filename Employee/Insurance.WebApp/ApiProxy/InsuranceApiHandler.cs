namespace Insurance.WebApp;

public class InsuranceApiHandler
    : IInsuranceApiHandler
{
    private readonly InsuranceApiClient _client;

    public InsuranceApiHandler()
    {
        _client = new InsuranceApiClient("http://localhost:5011", new HttpClient());
    }

    public async Task<ICollection<Contract>> GetContractsAsync()
    {
        return await _client.ContractAllAsync();
    }

    public async Task CreateContract(Contract contract)
    {
        await _client.ContractPOSTAsync(contract);
    }
}