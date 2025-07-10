using DataTransferLib.CommunicationsServices;
using AuditService.Database;
using AuditService.Database.Models;
using DataTransferLib.DataTransferObjects.Audit;
using DataTransferLib.DataTransferObjects.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using DataTransferLib.DataTransferObjects.Common;

namespace AuditService.Repositories;

public class FinancialLogRepository(ApplicationDbContext context) : IDisposable, IRepository<FinancialLog>
{
    private readonly ApplicationDbContext _context = context;
    private readonly DbSet<FinancialLog> _entities = context.FinancialLogs;
    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task<FinancialLog?> Add(Log? financialLogParams, bool flush = false) 
    {
        if (financialLogParams == null || financialLogParams.ObjectId == null || financialLogParams.Type == null)
            return null;

        string[]? objectIds = RequestService.GetValuesFromFilter<string>(financialLogParams.ObjectId);
        if (objectIds == null || objectIds.Length < 2)
            return null;

        FinancialLog financialLog = new()
        {
            DateOfLog = DateTime.UtcNow,
            Message = financialLogParams.Message,
            PerformedById = financialLogParams.PerformedById,
            FinancialRecordId = objectIds[0],
            FinancialLogType = (FinansialLogDto.FTYPE)financialLogParams.Type,
            UserId = objectIds[1]
        };

        await Add(financialLog);
        if (flush)
            await Save();

        return financialLog;
    }

    public async Task Add(FinancialLog financialLog, bool flush = false) 
    {
        await _entities.AddAsync(financialLog);
        if (flush)
            await Save();
    }

    public async Task<FinancialLog?> Remove(string id, bool flush = false) 
    {
        FinancialLog? financialLog = await Get(id);
        if (financialLog == null)
            return null;

        _entities.Remove(financialLog);
        if (flush)
            await Save();
        
        return financialLog;
    }

    public async Task Remove(FinancialLog financialLog, bool flush = false) 
    {
        _entities.Remove(financialLog);
        if (flush)
            await Save();
    }

    public async Task Update(FinancialLog financialLog, bool flush = false) {
        _context.Entry(financialLog).State = EntityState.Modified;
        if (flush)
            await Save();
    }

    public async Task<FinancialLog?> Change(string id, Log financialLogParams, bool flush = false) {
        FinancialLog? financialLog = await Get(id);
        if (financialLog == null)
            return null;

        financialLog.Message = financialLogParams.Message ?? financialLog.Message;
        financialLog.PerformedById = financialLogParams.PerformedById ?? financialLog.PerformedById;
        if (flush)
            await Save();

        return financialLog;
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<FinancialLog>> GetAll() {
        return await _entities.ToListAsync();
    }

    public async Task<FinancialLog?> Get(string id) {
        return await _entities.FindAsync(id);
    }

    public IQueryable<FinancialLog>? Filter(IQueryable<FinancialLog> objs, DefaultRequest defaultRequest)
    {
        if (defaultRequest.FilterBy == null || defaultRequest.FilterValue == null)
            return null;

        IQueryable<FinancialLog> defaultWhere = objs.Where(financialLog => financialLog.Message == defaultRequest.FilterValue);
        switch(defaultRequest.FilterBy.ToLower()) {
            case "message":
                return objs.Where(financialLog => financialLog.Message == defaultRequest.FilterValue);
            case "performed_by_id":
                return objs.Where(financialLog => financialLog.PerformedById == defaultRequest.FilterValue);
            case "date":
                DateTime[]? dates = RequestService.GetValuesFromFilter<DateTime>(defaultRequest.FilterValue);
                if (dates != null) {
                    if (dates[0] > dates[1])
                        (dates[0], dates[1]) = (dates[1], dates[0]);
                    return objs.Where(financialLog => financialLog.DateOfLog >= dates[0] && financialLog.DateOfLog <= dates[1]);
                }
                else
                    return defaultWhere;
            case "financial_record_id":
                return objs.Where(financialLog => financialLog.FinancialRecordId == defaultRequest.FilterValue);
            case "user_id":
                return objs.Where(financialLog => financialLog.UserId == defaultRequest.FilterValue);
            case "type":
                if (Enum.TryParse(defaultRequest.FilterValue, out FinansialLogDto.FTYPE type))
                    return objs.Where(financialLog => financialLog.FinancialLogType == type);
                else
                    return defaultWhere;
            default:
                return defaultWhere;
        }
    }

    public async Task<List<FinancialLog>> GetOrderBy(DefaultRequest defaultRequest)
    {
        IOrderedQueryable<FinancialLog> ordered = (defaultRequest.OrderBy.ToLower(), defaultRequest.OrderType.ToLower()) switch
        {
            ("date", "asc") => _entities.OrderBy(financialLog => financialLog.DateOfLog),
            ("date", "desc") => _entities.OrderByDescending(financialLog => financialLog.DateOfLog),
            ("message", "asc") => _entities.OrderBy(financialLog => financialLog.Message),
            ("message", "desc") => _entities.OrderByDescending(financialLog => financialLog.Message),
            ("performed_by_id", "asc") => _entities.OrderBy(financialLog => financialLog.PerformedById),
            ("performed_by_id", "desc") => _entities.OrderByDescending(financialLog => financialLog.PerformedById),
            ("user_id", "asc") => _entities.OrderBy(financialLog => financialLog.UserId),
            ("user_id", "desc") => _entities.OrderByDescending(financialLog => financialLog.UserId),
            ("financial_record_id", "asc") => _entities.OrderBy(financialLog => financialLog.FinancialRecordId),
            ("financial_record_id", "desc") => _entities.OrderByDescending(financialLog => financialLog.FinancialRecordId),
            ("type", "asc") => _entities.OrderBy(financialLog => financialLog.FinancialLogType),
            ("type", "desc") => _entities.OrderByDescending(financialLog => financialLog.FinancialLogType),
            _ => _entities.OrderBy(financialLog => financialLog.DateOfLog),
        };
        IQueryable<FinancialLog>? where = Filter(ordered, defaultRequest);

        return await (where ?? ordered).Skip((defaultRequest.Page - 1) * defaultRequest.Count).Take(defaultRequest.Count).ToListAsync();
    }

    public async Task<int> GetCount(DefaultRequest defaultRequest)
    {
        IQueryable<FinancialLog>? where = Filter(_entities, defaultRequest);
        return await (where != null ? where.CountAsync() : _entities.CountAsync());
    }
}