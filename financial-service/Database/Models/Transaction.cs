using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TTYPE = DataTransferLib.DataTransferObjects.Financial.Models.TTYPE;
using PTYPE = DataTransferLib.DataTransferObjects.Financial.Models.PTYPE;
using System.Text.Json.Serialization;

namespace FinancialService.Database.Models;

/// <summary>Транзация пользователя</summary>
[Table("transaction")]
public class Transaction
{
    [Key, Required]
    [Column("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Column("financial_data_id")]
    public string? FinancialDataId { get; set; }

    [JsonIgnore]
    public FinancialData? FinancialData { get; set; }

    [Column("balance_before")]
    public decimal BalanceBefore { get; set; }

    [Column("balance_after")]
    public decimal BalanceAfter { get; set; }

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("type")]
    public TTYPE Type { get; set; }

    [Column("payment_type")]
    public PTYPE PaymentType { get; set; }

    [Column("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public PaymentOrder? PaymentOrder { get; set; }
}

