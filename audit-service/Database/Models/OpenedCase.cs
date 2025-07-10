using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DtoClassLibrary.DataTransferObjects.Audit;

namespace AuditService.Database.Models;

[Table("opened_cases")]
public class OpenedCase
{
    [Key, Required]
    [Column("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [Column("case_id")]
    public string CaseId { get; set; }
    [Column("item_dropped_id")]
    public string ItemDroppedId { get; set; }
    [Column("time_opened")]
    public DateTime OpenedTimeStamp { get; set; }
    [Column("user_opened_id")]
    public string UserId { get; set; }

    [Column("item_cost")]
    public decimal? Cost { get; set; }
}

