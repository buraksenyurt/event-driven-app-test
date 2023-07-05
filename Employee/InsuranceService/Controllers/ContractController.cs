using System.Text.Json;
using Common.Queue;
using InsuranceService.Data;
using Microsoft.AspNetCore.Mvc;
using Common.Config;
using Microsoft.Extensions.Options;

namespace InsuranceService.Controllers;

[ApiController]
[Route("[controller]")]
public class ContractController
    : ControllerBase
{
    private readonly ContractDbContext _dbContext;
    private readonly ILogger<ContractController> _logger;
    private readonly IQueueHandler _queue;
    private readonly RabbitMqSettings _rmqSettings;

    public ContractController(ILogger<ContractController> logger, ContractDbContext dbContext, IQueueHandler queue, IOptions<RabbitMqSettings> rmqSettings)
    {
        _logger = logger;
        _dbContext = dbContext;
        _queue = queue;
        _rmqSettings = rmqSettings.Value;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Contract>> GetContracts()
    {
        return _dbContext.Contracts.ToList();
    }

    [HttpPost]
    public async Task CreateContract(Contract payload)
    {
        payload.ContractId = Guid.NewGuid();
        _dbContext.Contracts.Add(payload);
        await _dbContext.SaveChangesAsync();

        var contract = JsonSerializer.Serialize(new
        {
            payload.Id,
            payload.ContractId,
            payload.Title,
            payload.Quantity
        });
        await _queue.PublishMessage(_rmqSettings.ContractKey, contract);
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
        await _queue.PublishMessage(_rmqSettings.ContractKey, contract);

        return CreatedAtAction("GetContracts", new { payload.Id }, payload);
    }
}