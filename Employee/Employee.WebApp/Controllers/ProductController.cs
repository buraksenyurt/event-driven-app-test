using Employee.WebApp.ApiClient;
using Microsoft.AspNetCore.Mvc;

namespace Employee.WebApp.Controllers;

public class ContractController
    : Controller
{
    private readonly IEmployeeApiHandler _apiHandler;

    public ProductController(IEmployeeApiHandler apiHandler)
    {
        _apiHandler = apiHandler;
    }

    public async Task<IActionResult> List()
    {
        var contracts = await _apiHandler.GetContracts();
        return View(contracts);
    }
}