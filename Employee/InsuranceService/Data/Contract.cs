namespace InsuranceService.Data;

public class Contract
{
    public int Id { get; set; }
    public Guid ContractId { get; set; }
    public string Title { get; set; }
    public int Quantity { get; set; }
}