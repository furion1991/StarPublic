using AuditService.Database.Models;
using DataTransferLib.DataTransferObjects.Audit;

namespace AuditService.Converters;

public static class LogsConverter
{
    public static BaseLogDto ConvertToBaseLogDto(BaseLog baseLog)
    {
        return new BaseLogDto()
        {
            DateOfLog = baseLog.DateOfLog,
            Id = baseLog.Id,
            Message = baseLog.Message,
            PerformedById = baseLog.PerformedById,
        };
    }

    public static CaseLogDto ConvertToCaseLogDto(CaseLog caseLog)
    {
        return new CaseLogDto()
        {
            DateOfLog = caseLog.DateOfLog,
            CaseId = caseLog.CaseId,
            CaseLogType = caseLog.CaseLogType,
            Id = caseLog.Id,
            Message = caseLog.Message,
            PerformedById = caseLog.PerformedById,
        };
    }

    public static FinansialLogDto ConvertToFinansialLogDto(FinancialLog financialLog)
    {
        return new FinansialLogDto()
        {
            DateOfLog = financialLog.DateOfLog,
            UserId = financialLog.UserId,
            FinancialLogType = financialLog.FinancialLogType,
            FinancialRecordId = financialLog.FinancialRecordId,
            Id = financialLog.Id,
            Message = financialLog.Message,
            PerformedById = financialLog.PerformedById,
        };
    }

    public static ItemLogDto ConvertToItemLogDto(ItemLog itemLog)
    {
        return new ItemLogDto()
        {
            ItemId = itemLog.ItemId,
            DateOfLog = itemLog.DateOfLog,
            ItemLogType = itemLog.ItemLogType,
            Id = itemLog.Id,
            Message = itemLog.Message,
            PerformedById = itemLog.PerformedById,
        };
    }

    public static UserLogDto ConvertToUserLogDto(UserLog userLog)
    {
        return new UserLogDto()
        {
            Id = userLog.Id,
            UserId = userLog.UserId,
            DateOfLog = userLog.DateOfLog,
            UserLogType = userLog.UserLogType,
            Message = userLog.Message,
            PerformedById = userLog.PerformedById,
        };
    }
}