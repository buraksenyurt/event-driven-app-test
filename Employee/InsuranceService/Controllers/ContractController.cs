using System.Text.Json;
using InsuranceService.Data;
using InsuranceService.Queue;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceService.Controllers;

[ApiController]
[Route("[controller]")]
public class ContractController
    : ControllerBase
{
    private readonly ContractDbContext _dbContext;
    private readonly ILogger<ContractController> _logger;
    private readonly IQueue _queue;

    public ContractController(ILogger<ContractController> logger, ContractDbContext dbContext, IQueue queue)
    {
        _logger = logger;
        _dbContext = dbContext;
        _queue = queue;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Contract>> GetContracts()
    {
        return _dbContext.Contracts.ToList();
    }

    [HttpPost]
    public async Task<ActionResult<Contract>> CreateContract(Contract payload)
    {
        _dbContext.Contracts.Add(payload);
        await _dbContext.SaveChangesAsync();

        var contract = JsonSerializer.Serialize(new
        {
            payload.Id,
            payload.ContractId,
            payload.Title,
            payload.Quantity
        });
        await _queue.PublishMessage("insurance.contract", contract);

        return CreatedAtAction("GetContracts", new { payload.Id }, payload);
    }

    [HttpPut]
    public async Task<ActionResult<Contract>> UpdateContract(Contract payload)
    {
        _dbContext.Contracts.Update(payload);
        await _dbContext.SaveChangesAsync();

        var contract = JsonSerializer.Serialize(new
        {
            payload.Id,
            NewTitle = payload.Title,
            payload.Quantity
        });

        return CreatedAtAction("GetContracts", new { payload.Id }, payload);
    }
}