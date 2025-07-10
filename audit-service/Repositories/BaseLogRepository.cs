using DataTransferLib.CommunicationsServices;
using AuditService.Database;
using AuditService.Database.Models;
using DataTransferLib.DataTransferObjects.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using DataTransferLib.DataTransferObjects.Common;

namespace AuditService.Repositories;

public class BaseLogRepository(ApplicationDbContext context) : IDisposable, IRepository<BaseLog>
{
    private readonly ApplicationDbContext _context = context;
    private readonly DbSet<BaseLog> _entities = context.BaseLogs;
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

    public async Task<BaseLog?> Add(Log? baseLogParams, bool flush = false) 
    {
        if (baseLogParams == null)
            return null;
            
        BaseLog baseLog = new()
        {
            DateOfLog = DateTime.UtcNow,
            Message = baseLogParams.Message,
            PerformedById = baseLogParams.PerformedById
        };

        await Add(baseLog);
        if (flush)
            await Save();

        return baseLog;
    }

    public async Task Add(BaseLog baseLog, bool flush = false) 
    {
        await _entities.AddAsync(baseLog);
        if (flush)
            await Save();
    }

    public async Task<BaseLog?> Remove(string id, bool flush = false) 
    {
        BaseLog? baseLog = await Get(id);
        if (baseLog == null)
            return null;

        _entities.Remove(baseLog);
        if (flush)
            await Save();
        
        return baseLog;
    }

    public async Task Remove(BaseLog baseLog, bool flush = false) 
    {
        _entities.Remove(baseLog);
        if (flush)
            await Save();
    }

    public async Task Update(BaseLog baseLog, bool flush = false) {
        _context.Entry(baseLog).State = EntityState.Modified;
        if (flush)
            await Save();
    }

    public async Task<BaseLog?> Change(string id, Log baseLogParams, bool flush = false) {
        BaseLog? baseLog = await Get(id);
        if (baseLog == null)
            return null;

        baseLog.Message = baseLogParams.Message ?? baseLog.Message;
        baseLog.PerformedById = baseLogParams.PerformedById ?? baseLog.PerformedById;
        if (flush)
            await Save();

        return baseLog;
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<BaseLog>> GetAll() {
        return await _entities.ToListAsync();
    }

    public async Task<BaseLog?> Get(string id) {
        return await _entities.FindAsync(id);
    }

    public IQueryable<BaseLog>? Filter(IQueryable<BaseLog> objs, DefaultRequest defaultRequest)
    {
        if (defaultRequest.FilterBy == null || defaultRequest.FilterValue == null)
            return null;

        IQueryable<BaseLog> defaultWhere = objs.Where(baseLog => baseLog.Message == defaultRequest.FilterValue);
        switch(defaultRequest.FilterBy.ToLower()) {
            case "message":
                return objs.Where(baseLog => baseLog.Message == defaultRequest.FilterValue);
            case "performed_by_id":
                return objs.Where(baseLog => baseLog.PerformedById == defaultRequest.FilterValue);
            case "date":
                DateTime[]? dates = RequestService.GetValuesFromFilter<DateTime>(defaultRequest.FilterValue);
                if (dates != null) {
                    if (dates[0] > dates[1])
                        (dates[0], dates[1]) = (dates[1], dates[0]);
                    return objs.Where(baseLog => baseLog.DateOfLog >= dates[0] && baseLog.DateOfLog <= dates[1]);
                }
                else
                    return defaultWhere;
            default:
                return defaultWhere;
        }
    }

    public async Task<List<BaseLog>> GetOrderBy(DefaultRequest defaultRequest)
    {
        IOrderedQueryable<BaseLog> ordered = (defaultRequest.OrderBy.ToLower(), defaultRequest.OrderType.ToLower()) switch
        {
            ("date", "asc") => _entities.OrderBy(baseLog => baseLog.DateOfLog),
            ("date", "desc") => _entities.OrderByDescending(baseLog => baseLog.DateOfLog),
            ("message", "asc") => _entities.OrderBy(baseLog => baseLog.Message),
            ("message", "desc") => _entities.OrderByDescending(baseLog => baseLog.Message),
            ("performed_by_id", "asc") => _entities.OrderBy(baseLog => baseLog.PerformedById),
            ("performed_by_id", "desc") => _entities.OrderByDescending(baseLog => baseLog.PerformedById),
            _ => _entities.OrderBy(baseLog => baseLog.DateOfLog),
        };
        IQueryable<BaseLog>? where = Filter(ordered, defaultRequest);

        return await (where ?? ordered).Skip((defaultRequest.Page - 1) * defaultRequest.Count).Take(defaultRequest.Count).ToListAsync();
    }

    public async Task<int> GetCount(DefaultRequest defaultRequest)
    {
        IQueryable<BaseLog>? where = Filter(_entities, defaultRequest);
        return await (where != null ? where.CountAsync() : _entities.CountAsync());
    }
}