using FinancialService.Database.Models;

namespace FinancialService.Services;

public interface IPaymentService
{
    Task<string> GetPaymentLink(PaymentOrder order, string userEmail);
}