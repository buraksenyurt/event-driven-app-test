using Microsoft.AspNetCore.Mvc.Rendering;

namespace Employee.WebApp.Models;

public class NewEmployeeContract
{
    public int Id { get; set; }

    public string Fullname { get; set; }

    public Guid SelectedContract { get; set; }

    public string ContractTitle { get; set; }

    public List<SelectListItem> ContractId { get; set; }

    public int ContractInPortfolio { get; set; }
}