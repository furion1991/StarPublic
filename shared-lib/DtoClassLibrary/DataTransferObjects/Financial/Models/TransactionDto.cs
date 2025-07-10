using DataTransferLib.DataTransferObjects.Common.Interfaces;

namespace DataTransferLib.DataTransferObjects.Financial.Models;

/// <summary>Класс для корректной работы с объектом транзакции и её сущностью в БД</summary>
public class TransactionDto : IDefaultDto
{
    public required string Id { get; set; }
    public FinancialDataDto? FinancialData { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public decimal Amount { get; set; }
    public TTYPE Type { get; set; }
    public PTYPE PaymentType { get; set; }
    public DateTime Timestamp { get; set; }
}