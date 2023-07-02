namespace InsuranceService.Data;

using Microsoft.EntityFrameworkCore;

public class ContractDbContext
    : DbContext
{
    public ContractDbContext()
    {

    }

    public ContractDbContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<Contract> Contracts { get; set; }
}