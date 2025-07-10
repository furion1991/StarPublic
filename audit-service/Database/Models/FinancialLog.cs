using System.ComponentModel.DataAnnotations.Schema;
using DataTransferLib.DataTransferObjects.Audit;

namespace AuditService.Database.Models;

/// <summary>Финансовые логи</summary>
[Table("financial_log")]
public class FinancialLog : BaseLog
{

    [Column("financial_record_id")]
    public required string FinancialRecordId { get; set; }

    [Column("financial_log_type")]
    public required FinansialLogDto.FTYPE FinancialLogType { get; set; }
    [Column("amount")]
    public decimal Amount { get; set; }
    [Column("bonus_amount")]
    public decimal BonusAmount { get; set; }

    [Column("user_id")]
    public required string UserId { get; set; }
}