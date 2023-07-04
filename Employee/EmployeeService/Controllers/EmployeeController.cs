using Microsoft.AspNetCore.Mvc;
using EmployeeService.Data;
using Microsoft.EntityFrameworkCore;
using EmployeeService.Queue;
using System.Text.Json;

namespace EmployeeService.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeeController
    : ControllerBase
{
    private readonly ILogger<EmployeeController> _logger;
    private readonly EmployeeDbContext _dbContext;
    private readonly IQueue _queue;

    public EmployeeController(
        ILogger<EmployeeController> logger
        , EmployeeDbContext dbContext
        , IQueue queue)
    {
        _logger = logger;
        _dbContext = dbContext;
        _queue = queue;
    }

    [HttpPost]
    public async Task TakeContratForEmployee(Employee employee)
    {
        _dbContext.Employees.Add(employee);
        await _dbContext.SaveChangesAsync();
        var payload = JsonSerializer.Serialize(new
        {
            employee.ContractId,
            employee.ContractInPortfolio
        });
        await _queue.PublishMessage("insurance.employee", payload);
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
