using System.ComponentModel.DataAnnotations.Schema;
using DataTransferLib.DataTransferObjects.Audit;

namespace AuditService.Database.Models;

/// <summary>Пользовательские логи</summary>
[Table("user_log")]
public class UserLog : BaseLog
{
    [Column("user_id")] public required string UserId { get; set; }

    [Column("user_log_type")] public required UserLogDto.UTYPE UserLogType { get; set; }
}