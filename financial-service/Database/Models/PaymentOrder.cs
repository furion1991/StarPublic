using DataTransferLib.DataTransferObjects.Financial.Models;

namespace FinancialService.Database.Models;

public class PaymentOrder
{
    public required string Id { get; set; } = Guid.NewGuid().ToString();
    public decimal Amount { get; set; }
    public PTYPE PaymentType { get; set; }
    public required string UserId { get; set; }
    public string? TransactionId { get; set; }
    public Transaction? Transaction { get; set; }
    public string Status { get; set; }
    public DateTime ExpirationDate { get; set; }
}