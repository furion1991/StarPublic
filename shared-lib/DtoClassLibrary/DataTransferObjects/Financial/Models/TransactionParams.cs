using DataTransferLib.DataTransferObjects.Financial.Models;
using DtoClassLibrary.Converters;
using Newtonsoft.Json;

namespace DtoClassLibrary.DataTransferObjects.Financial.Models;

/// <summary>Класс для получения корректного json для работы с транзакцией</summary>
public class TransactionParams
{
    public string? UserId { get; set; }
    public string? FinancialDataId { get; set; }
    public float? BalanceBefore { get; set; }
    public float? BalanceAfter { get; set; }
    public decimal? Amount { get; set; }
    public TTYPE? Type { get; set; }
    public PTYPE? PaymentType { get; set; }
    public string? OrderId { get; set; }
}