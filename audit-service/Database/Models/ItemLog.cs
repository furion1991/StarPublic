using System.ComponentModel.DataAnnotations.Schema;
using DataTransferLib.DataTransferObjects.Audit;

namespace AuditService.Database.Models;

/// <summary>Логи предметов кейсов</summary>
[Table("item_log")]
public class ItemLog : BaseLog
{
    [Column("item_id")] public required string ItemId { get; set; }

    [Column("item_log_type")] public required ItemLogDto.ITYPE ItemLogType { get; set; }
}