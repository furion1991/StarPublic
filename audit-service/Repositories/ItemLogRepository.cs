using DataTransferLib.CommunicationsServices;
using AuditService.Database;
using AuditService.Database.Models;
using DataTransferLib.DataTransferObjects.Audit;
using DataTransferLib.DataTransferObjects.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using DataTransferLib.DataTransferObjects.Common;

namespace AuditService.Repositories;

public class ItemLogRepository(ApplicationDbContext context) : IDisposable, IRepository<ItemLog>
{
    private readonly ApplicationDbContext _context = context;
    private readonly DbSet<ItemLog> _entities = context.ItemLogs;
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

    public async Task<ItemLog?> Add(Log? itemLogParams, bool flush = false) 
    {
        if (itemLogParams == null || itemLogParams.ObjectId == null || itemLogParams.Type == null)
            return null;

        ItemLog itemLog = new()
        {
            DateOfLog = DateTime.UtcNow,
            Message = itemLogParams.Message,
            PerformedById = itemLogParams.PerformedById,
            ItemId = itemLogParams.ObjectId,
            ItemLogType = (ItemLogDto.ITYPE)itemLogParams.Type
        };

        await Add(itemLog);
        if (flush)
            await Save();

        return itemLog;
    }

    public async Task Add(ItemLog itemLog, bool flush = false) 
    {
        await _entities.AddAsync(itemLog);
        if (flush)
            await Save();
    }

    public async Task<ItemLog?> Remove(string id, bool flush = false) 
    {
        ItemLog? itemLog = await Get(id);
        if (itemLog == null)
            return null;

        _entities.Remove(itemLog);
        if (flush)
            await Save();
        
        return itemLog;
    }

    public async Task Remove(ItemLog itemLog, bool flush = false) 
    {
        _entities.Remove(itemLog);
        if (flush)
            await Save();
    }

    public async Task Update(ItemLog itemLog, bool flush = false) {
        _context.Entry(itemLog).State = EntityState.Modified;
        if (flush)
            await Save();
    }

    public async Task<ItemLog?> Change(string id, Log itemLogParams, bool flush = false) {
        ItemLog? itemLog = await Get(id);
        if (itemLog == null)
            return null;

        itemLog.Message = itemLogParams.Message ?? itemLog.Message;
        itemLog.PerformedById = itemLogParams.PerformedById ?? itemLog.PerformedById;
        if (flush)
            await Save();

        return itemLog;
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<ItemLog>> GetAll() {
        return await _entities.ToListAsync();
    }

    public async Task<ItemLog?> Get(string id) {
        return await _entities.FindAsync(id);
    }

    public IQueryable<ItemLog>? Filter(IQueryable<ItemLog> objs, DefaultRequest defaultRequest)
    {
        if (defaultRequest.FilterBy == null || defaultRequest.FilterValue == null)
            return null;

        IQueryable<ItemLog> defaultWhere = objs.Where(itemLog => itemLog.Message == defaultRequest.FilterValue);
        switch(defaultRequest.FilterBy.ToLower()) {
            case "message":
                return objs.Where(itemLog => itemLog.Message == defaultRequest.FilterValue);
            case "performed_by_id":
                return objs.Where(itemLog => itemLog.PerformedById == defaultRequest.FilterValue);
            case "date":
                DateTime[]? dates = RequestService.GetValuesFromFilter<DateTime>(defaultRequest.FilterValue);
                if (dates != null) {
                    if (dates[0] > dates[1])
                        (dates[0], dates[1]) = (dates[1], dates[0]);
                    return objs.Where(itemLog => itemLog.DateOfLog >= dates[0] && itemLog.DateOfLog <= dates[1]);
                }
                else
                    return defaultWhere;
            case "item_id":
                return objs.Where(itemLog => itemLog.ItemId == defaultRequest.FilterValue);
            case "type":
                if (Enum.TryParse(defaultRequest.FilterValue, out ItemLogDto.ITYPE type))
                    return objs.Where(itemLog => itemLog.ItemLogType == type);
                else
                    return defaultWhere;
            default:
                return defaultWhere;
        }
    }

    public async Task<List<ItemLog>> GetOrderBy(DefaultRequest defaultRequest)
    {
        IOrderedQueryable<ItemLog> ordered = (defaultRequest.OrderBy.ToLower(), defaultRequest.OrderType.ToLower()) switch
        {
            ("date", "asc") => _entities.OrderBy(itemLog => itemLog.DateOfLog),
            ("date", "desc") => _entities.OrderByDescending(itemLog => itemLog.DateOfLog),
            ("message", "asc") => _entities.OrderBy(itemLog => itemLog.Message),
            ("message", "desc") => _entities.OrderByDescending(itemLog => itemLog.Message),
            ("performed_by_id", "asc") => _entities.OrderBy(itemLog => itemLog.PerformedById),
            ("performed_by_id", "desc") => _entities.OrderByDescending(itemLog => itemLog.PerformedById),
            ("item_id", "asc") => _entities.OrderBy(itemLog => itemLog.ItemId),
            ("item_id", "desc") => _entities.OrderByDescending(itemLog => itemLog.ItemId),
            ("type", "asc") => _entities.OrderBy(itemLog => itemLog.ItemLogType),
            ("type", "desc") => _entities.OrderByDescending(itemLog => itemLog.ItemLogType),
            _ => _entities.OrderBy(itemLog => itemLog.DateOfLog),
        };
        IQueryable<ItemLog>? where = Filter(ordered, defaultRequest);

        return await (where ?? ordered).Skip((defaultRequest.Page - 1) * defaultRequest.Count).Take(defaultRequest.Count).ToListAsync();
    }

    public async Task<int> GetCount(DefaultRequest defaultRequest)
    {
        IQueryable<ItemLog>? where = Filter(_entities, defaultRequest);
        return await (where != null ? where.CountAsync() : _entities.CountAsync());
    }
}