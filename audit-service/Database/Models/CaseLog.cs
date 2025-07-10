using System.ComponentModel.DataAnnotations.Schema;
using DataTransferLib.DataTransferObjects.Audit;

namespace AuditService.Database.Models;

/// <summary>Логи кейсов</summary>
[Table("case_log")]
public class CaseLog : BaseLog
{
 
    [Column("case_id")]
    public required string CaseId { get; set; }

    [Column("case_log_type")]
    public required CaseLogDto.CTYPE CaseLogType { get; set; }
}