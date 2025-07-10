using System.ComponentModel.DataAnnotations.Schema;

namespace AuditService.Database.Models;

[Table("balances")]
public class BalanceStatisticsRecord
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public decimal AllUsersActualBalance { get; set; }
    public decimal AllUsersBonusBalance { get; set; }
    public decimal TotalProfit { get; set; }
    public decimal Withdrawn { get; set; }
    public DateTime RecordTimeStamp { get; set; }
}