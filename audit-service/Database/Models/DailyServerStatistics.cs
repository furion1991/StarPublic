using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditService.Database.Models;


[Table("daily_server_stats")]
public class DailyServerStatistics
{
    [Key, Required]
    [Column("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [Column("cases_opened")]
    public int CasesOpened { get; set; }
    [Column("statistics_date")]
    public DateTime StatisticsDate { get; set; }

    [Column("deposits_today")]
    public decimal DepositsToday { get; set; }
}

