namespace Employee.WebApp.ApiClient;

public class EmployeeApiHandler
    : IEmployeeApiHandler
{
    private readonly EmployeeApiClient _apiClient;

    public EmployeeApiHandler()
    {
        _apiClient = new EmployeeApiClient("http://localhost:5226", new HttpClient());
    }

    public async Task<ICollection<Contract>> GetContracts()
    {
        return await _apiClient.ContractsAsync();
    }

    public async Task<ICollection<Employee>> GetEmployees()
    {
        return await _apiClient.EmployeesAsync();
    }

    public IEnumerable<Contract> GetContractName()
    {
        return _apiClient.ContractsAsync().Result;
    }
    
    public async Task CreateEmployee(Employee employee, CancellationToken cancellationToken = default)
    {
        await _apiClient.EmployeeAsync(employee, cancellationToken);
    }
}