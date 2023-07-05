namespace Insurance.WebApp;

public class InsuranceApiWrapper
    : IInsuranceApiWrapper
{
    private readonly InsuranceApiClient _client;

    public InsuranceApiWrapper()
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