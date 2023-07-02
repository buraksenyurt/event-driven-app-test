using System.Text.Json;
using InsuranceService.Data;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceService.Controllers;

[ApiController]
[Route("[controller]")]
public class ContractController : ControllerBase
{
    private readonly ContractDbContext _dbContext;
    private readonly ILogger<ContractController> _logger;

    public ContractController(ILogger<ContractController> logger, ContractDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Contract>> GetContracts()
    {
        return _dbContext.Contracts.ToList();
    }

    [HttpPost]
    public async Task<ActionResult<Contract>> PostContract(Contract payload)
    {
        _dbContext.Contracts.Add(payload);
        await _dbContext.SaveChangesAsync();

        //// Event bildirimleri için
        // var product = JsonSerializer.Serialize(new
        // {
        //     payload.Id,
        //     payload.ContractId,
        //     payload.Title,
        //     payload.Quantity
        // });

        return CreatedAtAction("GetContracts", new { payload.Id }, payload);
    }

    [HttpPut]
    public async Task<ActionResult<Contract>> UpdateContract(Contract payload)
    {
        _dbContext.Contracts.Update(payload);

        await _dbContext.SaveChangesAsync();

        //// Event bildirimleri için
        // var contract = JsonSerializer.Serialize(new
        // {
        //     payload.Id,
        //     NewTitle = payload.Title,
        //     payload.Quantity
        // });

        return CreatedAtAction("GetContracts", new { payload.Id }, payload);
    }
}