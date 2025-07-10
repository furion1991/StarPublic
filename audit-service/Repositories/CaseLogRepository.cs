using DataTransferLib.CommunicationsServices;
using AuditService.Database;
using AuditService.Database.Models;
using DataTransferLib.DataTransferObjects.Audit;
using DataTransferLib.DataTransferObjects.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using DataTransferLib.DataTransferObjects.Common;

namespace AuditService.Repositories;

public class CaseLogRepository(ApplicationDbContext context) : IDisposable, IRepository<CaseLog>
{
    private readonly ApplicationDbContext _context = context;
    private readonly DbSet<CaseLog> _entities = context.CaseLogs;
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

    public async Task<CaseLog?> Add(Log? caseLogParams, bool flush = false) 
    {
        if (caseLogParams == null || caseLogParams.ObjectId == null || caseLogParams.Type == null) 
            return null;

        CaseLog caseLog = new()
        {
            DateOfLog = DateTime.UtcNow,
            Message = caseLogParams.Message,
            PerformedById = caseLogParams.PerformedById,
            CaseId = caseLogParams.ObjectId,
            CaseLogType = (CaseLogDto.CTYPE)caseLogParams.Type
        };

        await Add(caseLog);
        if (flush)
            await Save();

        return caseLog;
    }

    public async Task Add(CaseLog caseLog, bool flush = false) 
    {
        await _entities.AddAsync(caseLog);
        if (flush)
            await Save();
    }

    public async Task<CaseLog?> Remove(string id, bool flush = false) 
    {
        CaseLog? caseLog = await Get(id);
        if (caseLog == null)
            return null;

        _entities.Remove(caseLog);
        if (flush)
            await Save();
        
        return caseLog;
    }

    public async Task Remove(CaseLog caseLog, bool flush = false) 
    {
        _entities.Remove(caseLog);
        if (flush)
            await Save();
    }

    public async Task Update(CaseLog caseLog, bool flush = false) {
        _context.Entry(caseLog).State = EntityState.Modified;
        if (flush)
            await Save();
    }

    public async Task<CaseLog?> Change(string id, Log caseLogParams, bool flush = false) {
        CaseLog? caseLog = await Get(id);
        if (caseLog == null)
            return null;

        caseLog.Message = caseLogParams.Message ?? caseLog.Message;
        caseLog.PerformedById = caseLogParams.PerformedById ?? caseLog.PerformedById;
        if (flush)
            await Save();

        return caseLog;
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<CaseLog>> GetAll() {
        return await _entities.ToListAsync();
    }

    public async Task<CaseLog?> Get(string id) {
        return await _entities.FindAsync(id);
    }

    public IQueryable<CaseLog>? Filter(IQueryable<CaseLog> objs, DefaultRequest defaultRequest)
    {
        if (defaultRequest.FilterBy == null || defaultRequest.FilterValue == null)
            return null;

        IQueryable<CaseLog> defaultWhere = objs.Where(caseLog => caseLog.Message == defaultRequest.FilterValue);
        switch(defaultRequest.FilterBy.ToLower()) {
            case "message":
                return objs.Where(caseLog => caseLog.Message == defaultRequest.FilterValue);
            case "performed_by_id":
                return objs.Where(caseLog => caseLog.PerformedById == defaultRequest.FilterValue);
            case "date":
                DateTime[]? dates = RequestService.GetValuesFromFilter<DateTime>(defaultRequest.FilterValue);
                if (dates != null) {
                    if (dates[0] > dates[1])
                        (dates[0], dates[1]) = (dates[1], dates[0]);
                    return objs.Where(caseLog => caseLog.DateOfLog >= dates[0] && caseLog.DateOfLog <= dates[1]);
                }
                else
                    return defaultWhere;
            case "case_id":
                return objs.Where(caseLog => caseLog.CaseId == defaultRequest.FilterValue);
            case "type":
                if (Enum.TryParse(defaultRequest.FilterValue, out CaseLogDto.CTYPE type))
                    return objs.Where(caseLog => caseLog.CaseLogType == type);
                else
                    return defaultWhere;
            default:
                return defaultWhere;
        }
    }

    public async Task<List<CaseLog>> GetOrderBy(DefaultRequest defaultRequest)
    {
        IOrderedQueryable<CaseLog> ordered = (defaultRequest.OrderBy.ToLower(), defaultRequest.OrderType.ToLower()) switch
        {
            ("date", "asc") => _entities.OrderBy(caseLog => caseLog.DateOfLog),
            ("date", "desc") => _entities.OrderByDescending(caseLog => caseLog.DateOfLog),
            ("message", "asc") => _entities.OrderBy(caseLog => caseLog.Message),
            ("message", "desc") => _entities.OrderByDescending(caseLog => caseLog.Message),
            ("performed_by_id", "asc") => _entities.OrderBy(caseLog => caseLog.PerformedById),
            ("performed_by_id", "desc") => _entities.OrderByDescending(caseLog => caseLog.PerformedById),
            ("case_id", "asc") => _entities.OrderBy(caseLog => caseLog.CaseId),
            ("case_id", "desc") => _entities.OrderByDescending(caseLog => caseLog.CaseId),
            ("type", "asc") => _entities.OrderBy(caseLog => caseLog.CaseLogType),
            ("type", "desc") => _entities.OrderByDescending(caseLog => caseLog.CaseLogType),
            _ => _entities.OrderBy(caseLog => caseLog.DateOfLog),
        };
        IQueryable<CaseLog>? where = Filter(ordered, defaultRequest);

        return await (where ?? ordered).Skip((defaultRequest.Page - 1) * defaultRequest.Count).Take(defaultRequest.Count).ToListAsync();
    }

    public async Task<int> GetCount(DefaultRequest defaultRequest)
    {
        IQueryable<CaseLog>? where = Filter(_entities, defaultRequest);
        return await (where != null ? where.CountAsync() : _entities.CountAsync());
    }

    public async Task<List<CaseLog>> GetHistory()
    {
        return await _entities.Where(caseLog => EF.Functions.Like(caseLog.Message, "%успешно открыл кейс")).Take(10).ToListAsync();
    }
}