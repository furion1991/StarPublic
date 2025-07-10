using DataTransferLib.CommunicationsServices;
using DataTransferLib.DataTransferObjects.CasesItems;
using CasesService.Database;
using CasesService.Database.Models;
using DataTransferLib.DataTransferObjects.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using DataTransferLib.DataTransferObjects.Common;
using DataTransferLib.DataTransferObjects.CasesItems.Models;
using DtoClassLibrary.DataTransferObjects.CasesItems;
using DtoClassLibrary.DataTransferObjects.CasesItems.Models;

namespace CasesService.Repositories;

public class CaseRepository(ApplicationDbContext context) : IDisposable, IRepository<Case>
{
    private readonly ApplicationDbContext _context = context;
    private readonly DbSet<Case> _entities = context.Cases;
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

    public async Task<Case> Add(CaseParams caseParams, bool flush = false)
    {
        Case case_ = new()
        {
            Name = caseParams.Name,
            Type = caseParams.Type,
            Price = caseParams.Price,
            OpenLimit = caseParams.OpenLimit,
            Discount = caseParams.Discount,
            OldPrice = caseParams.OldPrice
        };

        await Add(case_);
        case_.Image = $"https://{Environment.GetEnvironmentVariable("ENV")}.24cases.ru/v1/case/image/{case_.Id}";
        if (flush)
            await Save();

        return case_;
    }

    public async Task Add(Case case_, bool flush = false)
    {
        await _entities.AddAsync(case_);
        if (flush)
            await Save();
    }

    public async Task<Case?> Remove(string id, bool flush = false)
    {
        Case? case_ = await Get(id);
        if (case_ == null)
            return null;

        _entities.Remove(case_);
        if (flush)
            await Save();

        return case_;
    }

    public async Task Remove(Case case_, bool flush = false)
    {
        _entities.Remove(case_);
        if (flush)
            await Save();
    }

    public async Task Update(Case case_, bool flush = false)
    {
        _context.Entry(case_).State = EntityState.Modified;
        if (flush)
            await Save();
    }

    public async Task<Case?> Change(string id, CaseParams caseParams, bool flush = false)
    {
        Case? case_ = await Get(id);
        if (case_ == null)
            return null;

        case_.Name = caseParams.Name ?? case_.Name;
        case_.Type = caseParams.Type;
        case_.Image = caseParams.Image ?? case_.Image;
        case_.Price = caseParams.Price ?? case_.Price;
        case_.OpenLimit = caseParams.OpenLimit ?? case_.OpenLimit;
        case_.Discount = caseParams.Discount ?? case_.Discount;
        case_.OldPrice = caseParams.OldPrice ?? case_.OldPrice;
        if (flush)
            await Save();

        return case_;
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<Case>> GetAll()
    {
        return await _entities
            .Include(e => e.ItemsCases)
            .ThenInclude(e => e.Item)
            .ToListAsync();
    }

    public async Task<Case?> Get(string id)
    {
        return await _entities.Include(case_ => case_.ItemsCases)
            .ThenInclude(i => i.Item)
            .FirstOrDefaultAsync(case_ => case_.Id == id);
    }



    public IQueryable<Case>? Filter(IQueryable<Case> objs, DefaultRequest defaultRequest)
    {
        if (defaultRequest.FilterBy == null || defaultRequest.FilterValue == null)
            return null;

        IQueryable<Case> defaultWhere = objs.Where(case_ => case_.Name == defaultRequest.FilterValue);
        switch (defaultRequest.FilterBy.ToLower())
        {
            case "name":
                return objs.Where(case_ => case_.Name == defaultRequest.FilterValue);
            case "type":
                if (Enum.TryParse(defaultRequest.FilterValue, out ECaseType type))
                    return objs.Where(case_ => case_.Type == type);
                else
                    return defaultWhere;
            case "is_visible":
                if (bool.TryParse(defaultRequest.FilterValue, out bool isVisible))
                    return objs.Where(case_ => case_.IsVisible == isVisible);
                else
                    return defaultWhere;
            case "price":
                decimal[]? prices = RequestService.GetValuesFromFilter<decimal>(defaultRequest.FilterValue);
                if (prices != null)
                {
                    if (prices[0] > prices[1])
                        (prices[0], prices[1]) = (prices[1], prices[0]);
                    return objs.Where(case_ => case_.Price >= prices[0] && case_.Price <= prices[1]);
                }
                else
                    return defaultWhere;
            default:
                return defaultWhere;
        }
    }

    public async Task<List<Case>> GetOrderBy(DefaultRequest defaultRequest)
    {
        IQueryable<Case> query = _entities.Include(e => e.ItemsCases).ThenInclude(e => e.Item);

        // ��������� ���������� (����� ���� �������� Filter � IQueryable<Case>)
        query = Filter(query, defaultRequest) as IQueryable<Case> ?? query;

        // ����������
        query = defaultRequest.OrderBy.ToLower() switch
        {
            "name" => defaultRequest.OrderType.ToLower() == "asc" ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name),
            "price" => defaultRequest.OrderType.ToLower() == "asc" ? query.OrderBy(c => c.Price) : query.OrderByDescending(c => c.Price),
            "type" => defaultRequest.OrderType.ToLower() == "asc" ? query.OrderBy(c => c.Type) : query.OrderByDescending(c => c.Type),
            "image" => defaultRequest.OrderType.ToLower() == "asc" ? query.OrderBy(c => c.Image) : query.OrderByDescending(c => c.Image),
            "open_limit" => defaultRequest.OrderType.ToLower() == "asc" ? query.OrderBy(c => c.OpenLimit) : query.OrderByDescending(c => c.OpenLimit),
            "discount" => defaultRequest.OrderType.ToLower() == "asc" ? query.OrderBy(c => c.Discount) : query.OrderByDescending(c => c.Discount),
            "old_price" => defaultRequest.OrderType.ToLower() == "asc" ? query.OrderBy(c => c.OldPrice) : query.OrderByDescending(c => c.OldPrice),
            "accumulated_profit" => defaultRequest.OrderType.ToLower() == "asc" ? query.OrderBy(c => c.AccumulatedProfit) : query.OrderByDescending(c => c.AccumulatedProfit),
            "current_open" => defaultRequest.OrderType.ToLower() == "asc" ? query.OrderBy(c => c.CurrentOpen) : query.OrderByDescending(c => c.CurrentOpen),
            _ => query.OrderBy(c => c.Name)
        };

        // ���������� ���������
        return await query
            .Skip((defaultRequest.Page - 1) * defaultRequest.Count)
            .Take(defaultRequest.Count)
            .ToListAsync();
    }



    public async Task<int> GetCount(DefaultRequest defaultRequest)
    {
        IQueryable<Case>? where = Filter(_entities, defaultRequest);
        return await (where != null ? where.CountAsync() : _entities.CountAsync());
    }

    public async Task<Case?> SetAlpha(string caseId, float alpha)
    {
        var caseDb = await Get(caseId);
        if (caseDb == null)
        {
            return null;
        }
        caseDb.Alpha = alpha;

        try
        {

            await Save();
            return caseDb;
        }
        catch (Exception e)
        {
            return null;
        }
    }
}