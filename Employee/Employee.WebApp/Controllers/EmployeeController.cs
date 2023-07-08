using Employee.WebApp.ApiClient;
using Employee.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Employee.WebApp.Controllers;

public class EmployeeController
    : Controller
{
    private readonly IEmployeeApiHandler _apiHandler;
    private readonly ILogger<EmployeeController> _logger;
    public EmployeeController(ILogger<EmployeeController> logger, IEmployeeApiHandler apiHandler)
    {
        _apiHandler = apiHandler;
        _logger = logger;
    }

    public async Task<IActionResult> List()
    {
        var result = new List<NewEmployeeContract>();
        var employees = await _apiHandler.GetEmployees();

        var contracts = _apiHandler.GetContractName();

        foreach (var emp in employees)
            result.Add(
                new NewEmployeeContract
                {
                    Fullname = emp.Fullname,
                    ContractTitle = contracts.FirstOrDefault(c => c.ContractId == emp.ContractId).Title,
                    ContractInPortfolio = emp.ContractInPortfolio
                }
            );

        return View(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(NewEmployeeContract payload)
    {
        var selectedContract = payload.SelectedContract;

        var employee = new Employee
        {
            Fullname = payload.Fullname,
            ContractId = selectedContract,
            ContractInPortfolio = payload.ContractInPortfolio
        };

        await _apiHandler.CreateEmployee(payload);
        return RedirectToAction("List");
    }

    public IActionResult Create()
    {
        var payload = new NewEmployeeContract
        {
            ContractId = new List<SelectListItem>()
        };

        foreach (var item in _apiHandler.GetContractName())
            payload.ContractId.Add(new SelectListItem
            {
                Text = item.Title,
                Value = item.ContractId.ToString()
            });

        return View(payload);
    }
}