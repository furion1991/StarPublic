namespace FinancialService.Database.Models;

public class Promocode
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PromocodeText { get; set; }
    public string IssuedBy { get; set; }
    public string Beneficiar { get; set; }
    public int UsageLimit { get; set; } = 1;
}

