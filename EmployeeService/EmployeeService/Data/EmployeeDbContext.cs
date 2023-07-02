namespace EmployeeService.Data;

using Microsoft.EntityFrameworkCore;

public class EmployeeDbContext
    : DbContext
{
    public EmployeeDbContext()
    {

    }

    public EmployeeDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Contract> Contracts { get; set; }
}