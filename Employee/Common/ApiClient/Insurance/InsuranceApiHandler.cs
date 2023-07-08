namespace Common.ApiClient.Insurance;

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
        return await _client.ContractAllAsync(new CancellationToken());
    }

    public async Task CreateContractAsync(Contract contract)
    {
        await _client.ContractPOSTAsync(contract, new CancellationToken());
    }
}