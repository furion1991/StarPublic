using System.Collections;
using AuditService.Converters;
using AuditService.Database;
using AuditService.Database.Models;
using AuditService.Repositories;
using DataTransferLib.CommunicationsServices;
using DataTransferLib.DataTransferObjects.Audit;
using DataTransferLib.DataTransferObjects.Common;

namespace AuditService.Services;

public class LogService(ApplicationDbContext context)
{
    private readonly BaseLogRepository _baseLogRepo = new(context);
    private readonly CaseLogRepository _caseLogRepo = new(context);
    private readonly ItemLogRepository _itemLogRepo = new(context);
    private readonly FinancialLogRepository _financialLogRepo = new(context);
    private readonly UserLogRepository _userLogRepo = new(context);

    public async Task<IList?> GetAllLogs(LogDefaultRequest? request)
    {
        if (request == null)
            return null;

        DefaultRequest standardDefaultRequest = request;
        RequestService.CheckPaginationParams(ref standardDefaultRequest);
        IList list = request.LogType switch
        {
            LTYPE.Case => (await _caseLogRepo.GetOrderBy(standardDefaultRequest)).Select(LogsConverter.ConvertToCaseLogDto)
                .ToList(),
            LTYPE.Item => (await _itemLogRepo.GetOrderBy(standardDefaultRequest)).Select(LogsConverter.ConvertToItemLogDto)
                .ToList(),
            LTYPE.Financial => (await _financialLogRepo.GetOrderBy(standardDefaultRequest))
                .Select(LogsConverter.ConvertToFinansialLogDto).ToList(),
            LTYPE.User => (await _userLogRepo.GetOrderBy(standardDefaultRequest)).Select(LogsConverter.ConvertToUserLogDto)
                .ToList(),
            _ => (await _baseLogRepo.GetOrderBy(standardDefaultRequest)).Select(LogsConverter.ConvertToBaseLogDto).ToList(),
        };

        return list;
    }

    public async Task<int?> GetAllCount(LogDefaultRequest? request)
    {
        if (request == null)
            return null;

        DefaultRequest standardDefaultRequest = request;
        RequestService.CheckPaginationParams(ref standardDefaultRequest);
        return request.LogType switch
        {
            LTYPE.Case => await _caseLogRepo.GetCount(standardDefaultRequest),
            LTYPE.Item => await _itemLogRepo.GetCount(standardDefaultRequest),
            LTYPE.Financial => await _financialLogRepo.GetCount(standardDefaultRequest),
            LTYPE.User => await _userLogRepo.GetCount(standardDefaultRequest),
            _ => await _baseLogRepo.GetCount(standardDefaultRequest),
        };
    }

    public string? GetAllMessage(LogDefaultRequest? request)
    {
        if (request == null)
            return null;

        DefaultRequest standardDefaultRequest = request;
        RequestService.CheckPaginationParams(ref standardDefaultRequest);
        return request.LogType switch
        {
            LTYPE.Case => "Список логов кейсов:",
            LTYPE.Item => "Список логов предметов:",
            LTYPE.Financial => "Список пользовательских логов:",
            LTYPE.User => "Список финансовых логов:",
            _ => "Список базовых логов:",
        };
    }

    public async Task<BaseLog?> Get(string? id)
    {
        if (id == null)
            return null;

        return await _baseLogRepo.Get(id);
    }

    public async Task<BaseLog?> Add(Log? logParams)
    {
        if (logParams == null)
            return null;

        return logParams.LogType switch
        {
            LTYPE.Case => await _caseLogRepo.Add(logParams, true),
            LTYPE.Item => await _itemLogRepo.Add(logParams, true),
            LTYPE.Financial => await _financialLogRepo.Add(logParams, true),
            LTYPE.User => await _userLogRepo.Add(logParams, true),
            _ => await _baseLogRepo.Add(logParams, true),
        };
    }

    public async Task<BaseLog?> Change(string? id, Log? logParams)
    {
        if (id == null || logParams == null)
            return null;

        return logParams.LogType switch
        {
            LTYPE.Case => await _caseLogRepo.Change(id, logParams, true),
            LTYPE.Item => await _itemLogRepo.Change(id, logParams, true),
            LTYPE.Financial => await _financialLogRepo.Change(id, logParams, true),
            LTYPE.User => await _userLogRepo.Change(id, logParams, true),
            _ => await _baseLogRepo.Change(id, logParams, true),
        };
    }

    public async Task<BaseLog?> Remove(string? id)
    {
        if (id == null)
            return null;

        return await _baseLogRepo.Remove(id, true);
    }

    public async Task<List<CaseLog>> GetHistory()
    {
        return await _caseLogRepo.GetHistory();
    }
}