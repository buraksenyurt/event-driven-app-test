using Microsoft.AspNetCore.Mvc;
using EmployeeService.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly ILogger<EmployeeController> _logger;
    private readonly EmployeeDbContext _dbContext;

    public EmployeeController(ILogger<EmployeeController> logger, EmployeeDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task CreateEmployee(Employee employee)
    {
        _dbContext.Employees.Add(employee);
        await _dbContext.SaveChangesAsync();
    }

    [HttpGet]
    [Route("/employees")]
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
    {

        return await _dbContext.Employees.ToListAsync();
    }

    [HttpGet]
    [Route("/contracts")]
    public ActionResult<IEnumerable<Contract>> GetContracts()
    {
        return _dbContext.Contracts.ToList();
    }
}
