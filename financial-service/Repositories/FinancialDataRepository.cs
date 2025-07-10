using DataTransferLib.CommunicationsServices;
using DataTransferLib.DataTransferObjects.Common;
using DataTransferLib.DataTransferObjects.Common.Interfaces;
using DataTransferLib.DataTransferObjects.Financial;
using FinancialService.Database;
using FinancialService.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancialService.Repositories;

public class FinancialDataRepository(ApplicationDbContext context) : IDisposable, IRepository<FinancialData>
{
    private readonly ApplicationDbContext _context = context;
    private readonly DbSet<FinancialData> _entities = context.FinancialDatas;
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

    public async Task<FinancialData?> Add(FinancialDataParams? financialDataParams, bool flush = false)
    {
        if (financialDataParams == null || financialDataParams.UserId == null)
            return null;

        FinancialData financialData = new()
        {
            UserId = financialDataParams.UserId,
            CurrentBalance = financialDataParams.CurrentBalance ?? 0
        };

        await Add(financialData);
        if (flush)
            await Save();

        return financialData;
    }

    public async Task Add(FinancialData financialData, bool flush = false)
    {
        await _entities.AddAsync(financialData);
        if (flush)
            await Save();
    }

    public async Task Remove(FinancialData financialData, bool flush = false)
    {
        _entities.Remove(financialData);
        if (flush)
            await Save();
    }

    public async Task Update(FinancialData financialData, bool flush = false)
    {
        _context.Entry(financialData).State = EntityState.Modified;
        if (flush)
        {
            await Save();
        }
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<FinancialData>> GetAll()
    {
        return await _entities.ToListAsync();
    }

    public async Task<FinancialData?> Get(string userId)
    {
        return await _entities.FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public IQueryable<FinancialData>? Filter(IQueryable<FinancialData> objs, DefaultRequest defaultRequest)
    {
        if (defaultRequest.FilterBy == null || defaultRequest.FilterValue == null)
            return null;

        IQueryable<FinancialData> defaultWhere = objs.Where(x => x.UserId == defaultRequest.FilterValue);
        switch (defaultRequest.FilterBy.ToLower())
        {
            case "current_balance":
                decimal[]? balances = RequestService.GetValuesFromFilter<decimal>(defaultRequest.FilterValue);
                if (balances != null)
                    return objs.Where(x => x.CurrentBalance >= balances[0] && x.CurrentBalance <= balances[1]);
                else
                    return defaultWhere;
            default:
                return defaultWhere;
        }
    }

    public async Task<List<FinancialData>> GetOrderBy(DefaultRequest defaultRequest)
    {
        IOrderedQueryable<FinancialData> ordered = (defaultRequest.OrderBy.ToLower(), defaultRequest.OrderType.ToLower()) switch
        {
            ("user_id", "asc") => _entities.OrderBy(x => x.UserId),
            ("user_id", "desc") => _entities.OrderByDescending(x => x.UserId),
            ("current_balance", "asc") => _entities.OrderBy(x => x.CurrentBalance),
            ("current_balance", "desc") => _entities.OrderByDescending(x => x.CurrentBalance),
            _ => _entities.OrderBy(x => x.UserId),
        };
        IQueryable<FinancialData>? where = Filter(ordered, defaultRequest);

        return await (where ?? ordered).Skip((defaultRequest.Page - 1) * defaultRequest.Count).Take(defaultRequest.Count).ToListAsync();
    }

    public async Task<int> GetCount(DefaultRequest defaultRequest)
    {
        IQueryable<FinancialData>? where = Filter(_entities, defaultRequest);
        return await (where != null ? where.CountAsync() : _entities.CountAsync());
    }
}