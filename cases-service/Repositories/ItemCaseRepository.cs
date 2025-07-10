using CasesService.Database;
using CasesService.Database.Models;
using DataTransferLib.DataTransferObjects.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using DataTransferLib.DataTransferObjects.CasesItems.Models;
using DataTransferLib.DataTransferObjects.Common;
using CasesService.Controllers.Models;

namespace CasesService.Repositories;

public class ItemCaseRepository(ApplicationDbContext context) : IDisposable, IRepository<ItemCase>
{
    private readonly ApplicationDbContext _context = context;
    private readonly DbSet<ItemCase> _entities = context.ItemsCases;
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

    public async Task<ItemCase> Add(ItemCaseParams itemCaseParams, bool flush = false) 
    {
        ItemCase itemCase = new()
        {
            ItemId = itemCaseParams.ItemId,
            CaseId = itemCaseParams.CaseId
        };

        await Add(itemCase);
        if (flush)
            await Save();

        await _entities
            .Include(itemCase => itemCase.Case)
            .Include(itemCase => itemCase.Item)
            .FirstOrDefaultAsync(i => i.CaseId == itemCase.CaseId && i.ItemId == itemCase.ItemId);

        return itemCase;
    }

    public async Task Add(ItemCase itemCase, bool flush = false) 
    {
        await _entities.AddAsync(itemCase);
        if (flush)
            await Save();
    }

    public async Task<ItemCase?> Remove(string caseId, string itemId, bool flush = false) 
    {
        ItemCase? itemCase = await Get(caseId, itemId);
        if (itemCase == null)
            return null;

        _entities.Remove(itemCase);
        if (flush)
            await Save();
        
        return itemCase;
    }

    public async Task Remove(ItemCase itemCase, bool flush = false) 
    {
        _entities.Remove(itemCase);
        if (flush)
            await Save();
    }

    public async Task Update(ItemCase itemCase, bool flush = false) {
        _context.Entry(itemCase).State = EntityState.Modified;
        if (flush)
            await Save();
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<ItemCase>> GetAll() {
        return await _entities.ToListAsync();
    }

    public async Task<ItemCase?> Get(string id) {
        return await _entities.FirstOrDefaultAsync(itemCase => itemCase.CaseId == id);
    }

    public async Task<ItemCase?> Get(string caseId, string itemId) {
        return await _entities.FirstOrDefaultAsync(itemCase => itemCase.CaseId == caseId && itemCase.ItemId == itemId);
    }

    public IQueryable<ItemCase>? Filter(IQueryable<ItemCase> objs, DefaultRequest defaultRequest)
    {
        if (defaultRequest.FilterBy == null || defaultRequest.FilterValue == null)
            return null;

        return null;
    }

    public async Task<List<ItemCase>> GetOrderBy(DefaultRequest defaultRequest)
    {
        IOrderedQueryable<ItemCase> ordered = (defaultRequest.OrderBy.ToLower(), defaultRequest.OrderType.ToLower()) switch
        {
            ("case", "asc") => _entities.OrderBy(itemCase => itemCase.Case),
            ("case", "desc") => _entities.OrderByDescending(itemCase => itemCase.Case),
            ("item", "asc") => _entities.OrderBy(itemCase => itemCase.Item),
            ("item", "desc") => _entities.OrderByDescending(itemCase => itemCase.Item),
            _ => _entities.OrderBy(itemCase => itemCase.Case),
        };
        IQueryable<ItemCase>? where = Filter(ordered, defaultRequest);

        return await (where ?? ordered).Skip((defaultRequest.Page - 1) * defaultRequest.Count).Take(defaultRequest.Count).ToListAsync();
    }

    public async Task<int> GetCount(DefaultRequest defaultRequest)
    {
        IQueryable<ItemCase>? where = Filter(_entities, defaultRequest);
        return await (where != null ? where.CountAsync() : _entities.CountAsync());
    }
}