namespace FinancialService.Database.Models;

public class PaymentProvider
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string ProviderName { get; set; }
}