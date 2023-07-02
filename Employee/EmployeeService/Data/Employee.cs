namespace EmployeeService.Data;

public class Employee
{
    public int Id { get; set; }
    public string Fullname { get; set; }
    public Guid ContractId { get; set; }
    public int ContractInPortfolio { get; set; }
}