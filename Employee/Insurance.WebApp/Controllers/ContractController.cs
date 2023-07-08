using Common.ApiClient.Insurance;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.WebApp.Controllers;

public class ContractController
    : Controller
{
    private readonly ILogger<ContractController> _logger;
    private readonly IInsuranceApiHandler _apiHandler;

    public ContractController(ILogger<ContractController> logger, IInsuranceApiHandler apiHandler)
    {
        _logger = logger;
        _apiHandler = apiHandler;
    }
    public async Task<IActionResult> List()
    {
        var contracts = await _apiHandler.GetContractsAsync();
        return View(contracts);
    }

    public IActionResult Create() // Bunu eklemezsek View ekrana gelmez
    {
        var contract = new Contract();
        return View(contract);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Contract contract)
    {
        await _apiHandler.CreateContractAsync(contract);
        return RedirectToAction(nameof(List));
    }
}