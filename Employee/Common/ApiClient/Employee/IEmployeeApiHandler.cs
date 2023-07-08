namespace Common.ApiClient.Employee;

public interface IEmployeeApiHandler
{
    Task<ICollection<Contract>> GetContracts();
    IEnumerable<Contract> GetContractName();
    Task<ICollection<Employee>> GetEmployees();
    Task CreateEmployee(Employee employee, CancellationToken cancellationToken = default);
}