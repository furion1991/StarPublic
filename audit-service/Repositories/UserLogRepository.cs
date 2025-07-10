using DataTransferLib.CommunicationsServices;
using AuditService.Database;
using AuditService.Database.Models;
using DataTransferLib.DataTransferObjects.Audit;
using DataTransferLib.DataTransferObjects.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using DataTransferLib.DataTransferObjects.Common;

namespace AuditService.Repositories;

public class UserLogRepository(ApplicationDbContext context) : IDisposable, IRepository<UserLog>
{
    private readonly ApplicationDbContext _context = context;
    private readonly DbSet<UserLog> _entities = context.UserLogs;
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

    public async Task<UserLog?> Add(Log? userLogParams, bool flush = false) 
    {
        if (userLogParams == null || userLogParams.ObjectId == null || userLogParams.Type == null)
            return null;

        UserLog userLog = new()
        {
            DateOfLog = DateTime.UtcNow,
            Message = userLogParams.Message,
            PerformedById = userLogParams.PerformedById,
            UserId = userLogParams.ObjectId,
            UserLogType = (UserLogDto.UTYPE)userLogParams.Type
        };

        await Add(userLog);
        if (flush)
            await Save();

        return userLog;
    }

    public async Task Add(UserLog userLog, bool flush = false) 
    {
        await _entities.AddAsync(userLog);
        if (flush)
            await Save();
    }

    public async Task<UserLog?> Remove(string id, bool flush = false) 
    {
        UserLog? userLog = await Get(id);
        if (userLog == null)
            return null;

        _entities.Remove(userLog);
        if (flush)
            await Save();
        
        return userLog;
    }

    public async Task Remove(UserLog userLog, bool flush = false) 
    {
        _entities.Remove(userLog);
        if (flush)
            await Save();
    }

    public async Task Update(UserLog userLog, bool flush = false) {
        _context.Entry(userLog).State = EntityState.Modified;
        if (flush)
            await Save();
    }

    public async Task<UserLog?> Change(string id, Log userLogParams, bool flush = false) {
        UserLog? userLog = await Get(id);
        if (userLog == null)
            return null;

        userLog.Message = userLogParams.Message ?? userLog.Message;
        userLog.PerformedById = userLogParams.PerformedById ?? userLog.PerformedById;
        if (flush)
            await Save();

        return userLog;
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<UserLog>> GetAll() {
        return await _entities.ToListAsync();
    }

    public async Task<UserLog?> Get(string id) {
        return await _entities.FindAsync(id);
    }

    public IQueryable<UserLog>? Filter(IQueryable<UserLog> objs, DefaultRequest defaultRequest)
    {
        if (defaultRequest.FilterBy == null || defaultRequest.FilterValue == null)
            return null;

        IQueryable<UserLog> defaultWhere = objs.Where(x => x.Message == defaultRequest.FilterValue);
        switch(defaultRequest.FilterBy.ToLower()) {
            case "message":
                return objs.Where(x => x.Message == defaultRequest.FilterValue);
            case "performed_by_id":
                return objs.Where(x => x.PerformedById == defaultRequest.FilterValue);
            case "date":
                DateTime[]? dates = RequestService.GetValuesFromFilter<DateTime>(defaultRequest.FilterValue);
                if (dates != null) {
                    if (dates[0] > dates[1])
                        (dates[0], dates[1]) = (dates[1], dates[0]);
                    return objs.Where(x => x.DateOfLog >= dates[0] && x.DateOfLog <= dates[1]);
                }
                else
                    return defaultWhere;
            case "user_id":
                return objs.Where(x => x.UserId == defaultRequest.FilterValue);
            case "type":
                if (Enum.TryParse(defaultRequest.FilterValue, out UserLogDto.UTYPE type))
                    return objs.Where(x => x.UserLogType == type);
                else
                    return defaultWhere;
            default:
                return defaultWhere;
        }
    }

    public async Task<List<UserLog>> GetOrderBy(DefaultRequest defaultRequest)
    {
        IOrderedQueryable<UserLog> ordered = (defaultRequest.OrderBy.ToLower(), defaultRequest.OrderType.ToLower()) switch
        {
            ("date", "asc") => _entities.OrderBy(x => x.DateOfLog),
            ("date", "desc") => _entities.OrderByDescending(x => x.DateOfLog),
            ("message", "asc") => _entities.OrderBy(x => x.Message),
            ("message", "desc") => _entities.OrderByDescending(x => x.Message),
            ("performed_by_id", "asc") => _entities.OrderBy(x => x.PerformedById),
            ("performed_by_id", "desc") => _entities.OrderByDescending(x => x.PerformedById),
            ("user_id", "asc") => _entities.OrderBy(x => x.UserId),
            ("user_id", "desc") => _entities.OrderByDescending(x => x.UserId),
            ("type", "asc") => _entities.OrderBy(x => x.UserLogType),
            ("type", "desc") => _entities.OrderByDescending(x => x.UserLogType),
            _ => _entities.OrderBy(x => x.DateOfLog),
        };
        IQueryable<UserLog>? where = Filter(ordered, defaultRequest);

        return await (where ?? ordered).Skip((defaultRequest.Page - 1) * defaultRequest.Count).Take(defaultRequest.Count).ToListAsync();
    }

    public async Task<int> GetCount(DefaultRequest defaultRequest)
    {
        IQueryable<UserLog>? where = Filter(_entities, defaultRequest);
        return await (where != null ? where.CountAsync() : _entities.CountAsync());
    }
}