using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditService.Database.Models;

/// <summary>Базовый тип логов</summary>
[Table("base_log")]
public class BaseLog
{
    [Key, Required]
    [Column("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Column("message")]
    public string? Message { get; set; }

    [Column("performed_by_id")]
    public string? PerformedById { get; set; }

    [Column("date_of_log")]
    public required DateTime DateOfLog { get; set; }
}