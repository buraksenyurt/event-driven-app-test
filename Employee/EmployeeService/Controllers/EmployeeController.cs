using Microsoft.AspNetCore.Mvc;
using EmployeeService.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Common.Queue;
using Common.Config;
using Microsoft.Extensions.Options;

namespace EmployeeService.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeeController
    : ControllerBase
{
    private readonly ILogger<EmployeeController> _logger;
    private readonly EmployeeDbContext _dbContext;
    private readonly IQueue _queue;
    private readonly RabbitMqSettings _rmqSettings;

    public EmployeeController(
        ILogger<EmployeeController> logger
        , EmployeeDbContext dbContext
        , IQueue queue
        , IOptions<RabbitMqSettings> rmqSettings)
    {
        _logger = logger;
        _dbContext = dbContext;
        _queue = queue;
        _rmqSettings = rmqSettings.Value;
    }

    [HttpPost]
    public async Task CreateEmployee(Employee employee)
    {
        _dbContext.Employees.Add(employee);
        await _dbContext.SaveChangesAsync();
        var payload = JsonSerializer.Serialize(new
        {
            employee.ContractId,
            employee.ContractInPortfolio
        });
        await _queue.PublishMessage(_rmqSettings.EmployeeKey, payload);
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
