using DataTransferLib.DataTransferObjects.Financial.Models;
using DataTransferLib.DataTransferObjects.Financial.Payments;
using FinancialService.Database;
using FinancialService.Database.Models;

namespace FinancialService.Services;

public class PaymentService(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
{
    public async Task<string> GetPaymentLink(CommonPaymentLinkRequest request)
    {
        var order = await CreatePaymentOrder(request);
        var wataPaymentService = serviceProvider.GetRequiredService<WataPaymentService>();
        var paymentLink = await wataPaymentService.GetPaymentLink(order, request.Email);
        await dbContext.SaveChangesAsync();
        return paymentLink;
    }

    public async Task<List<string>> GetPaymentProviders()
    {
        return dbContext.PaymentProviders.Select(e => e.ProviderName).ToList();
    }
    public async Task AcceptPayment(string orderId)
    {
        var order = await dbContext.PaymentOrders.FindAsync(orderId);
        if (order == null)
        {
            throw new Exception($"Payment order with ID {orderId} not found.");
        }
        order.Status = "Accepted";
        await dbContext.SaveChangesAsync();
    }

    public async Task RejectPayment(string orderId)
    {
        var order = await dbContext.PaymentOrders.FindAsync(orderId);
        if (order == null)
        {
            throw new Exception($"Payment order with ID {orderId} not found.");
        }
        order.Status = "Rejected";
        await dbContext.SaveChangesAsync();
    }

    public async Task<PaymentOrder> CreatePaymentOrder(CommonPaymentLinkRequest request)
    {
        var order = new PaymentOrder
        {
            UserId = request.UserId,
            Amount = Convert.ToDecimal(request.Amount),
            PaymentType = PTYPE.Bank,
            Id = Guid.NewGuid().ToString(),
            Status = "Pending"
        };

        await dbContext.PaymentOrders.AddAsync(order);
        await dbContext.SaveChangesAsync();
        return order;
    }

    public async Task<PaymentOrder?> GetPaymentOrder(string orderId)
    {
        return await dbContext.PaymentOrders.FindAsync(orderId);
    }
}