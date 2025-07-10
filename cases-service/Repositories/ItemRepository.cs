using CasesService.Database;
using CasesService.Database.Models;
using DataTransferLib.DataTransferObjects.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using DataTransferLib.DataTransferObjects.CasesItems.Models;
using DataTransferLib.DataTransferObjects.Common;
using DtoClassLibrary.DataTransferObjects.Bonus;
using DtoClassLibrary.DataTransferObjects.CasesItems.Models;

namespace CasesService.Repositories;

public class ItemRepository(ApplicationDbContext context) : IDisposable, IRepository<Item>
{
    private readonly ApplicationDbContext _context = context;
    private readonly DbSet<Item> _entities = context.Items;
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

    public async Task<List<Item>> GetItemsByIdsAsync(List<string> ids)
    {
        return await _context.Items
            .Include(i => i.ItemsCases)
            .Where(i => ids.Contains(i.Id)).ToListAsync();
    }
    public async Task<Item> Add(ItemParams itemParams, bool flush = false)
    {
        Item item = new()
        {
            Name = itemParams.Name,
            Type = itemParams.Type,
            Rarity = itemParams.Rarity,
            BaseCost = itemParams.BaseCost,
            SellPrice = itemParams.SellPrice,
            IsVisible = itemParams.IsVisible,
            Game = itemParams.Game
        };

        await Add(item);
        item.Image = $"https://{Environment.GetEnvironmentVariable("ENV")}.24cases.ru/v1/item/image/{item.Id}";
        if (flush)
            await Save();

        return item;
    }

    public async Task Add(Item item, bool flush = false)
    {
        await _entities.AddAsync(item);
        if (flush)
            await Save();
    }

    public async Task<Item?> Remove(string id, bool flush = false)
    {
        Item? item = await Get(id);
        if (item == null)
            return null;

        _entities.Remove(item);
        if (flush)
            await Save();

        return item;
    }

    public async Task Remove(Item item, bool flush = false)
    {
        _entities.Remove(item);
        if (flush)
            await Save();
    }

    public async Task Update(Item Item, bool flush = false)
    {
        _context.Entry(Item).State = EntityState.Modified;
        if (flush)
            await Save();
    }

    public async Task<Item?> Change(string id, ItemParams itemParams, bool flush = false)
    {
        Item? item = await Get(id);
        if (item == null)
            return null;

        item.Name = itemParams.Name ?? item.Name;
        item.Type = itemParams.Type;
        item.Rarity = itemParams.Rarity;
        item.BaseCost = itemParams.BaseCost ?? item.BaseCost;
        item.SellPrice = itemParams.SellPrice ?? item.SellPrice;
        item.IsVisible = itemParams.IsVisible;
        item.Game = itemParams.Game ?? item.Game;
        if (flush)
            await Save();

        return item;
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<Item>> GetAll()
    {
        return await _entities.ToListAsync();
    }

    public async Task<Item?> Get(string id)
    {
        return await _entities.Include(x => x.ItemsCases).FirstOrDefaultAsync(item => item.Id == id);
    }

    public IQueryable<Item>? Filter(IQueryable<Item> objs, DefaultRequest defaultRequest)
    {
        if (defaultRequest.FilterBy == null || defaultRequest.FilterValue == null)
            return null;

        IQueryable<Item> defaultWhere = objs.Where(item => item.Name == defaultRequest.FilterValue);
        switch (defaultRequest.FilterBy.ToLower())
        {
            case "name":
                return objs.Where(item => item.Name == defaultRequest.FilterValue);
            case "type":
                if (Enum.TryParse(defaultRequest.FilterValue, out ItemType type))
                    return objs.Where(item => item.Type == type);
                else
                    return defaultWhere;
            case "rarity":
                if (Enum.TryParse(defaultRequest.FilterValue, out EItemRarity rarity))
                    return objs.Where(item => item.Rarity == rarity);
                else
                    return defaultWhere;
            case "is_visible":
                if (bool.TryParse(defaultRequest.FilterValue, out bool isVisible))
                    return objs.Where(item => item.IsVisible == isVisible);
                else
                    return defaultWhere;
            default:
                return defaultWhere;
        }
    }

    public async Task<List<Item>> GetListOfCost(decimal cost)
    {
        var items = await _context.Items.Where(i => i.SellPrice < cost).ToListAsync();

        return items;
    }

    public async Task<List<Item>> GetFromBonusParams(ItemBonusDto bonus)
    {
        var items = await _context.Items
            .Where(i => i.SellPrice > bonus.MinimalItemCost && i.SellPrice <= bonus.MaximalItemCost).ToListAsync();
        return items;
    }

    public async Task<List<Item>> GetOrderBy(DefaultRequest defaultRequest)
    {
        if (defaultRequest.Count == -1)
        {
            defaultRequest.Count = DefaultRequest.MaximumCount;
        }

        IOrderedQueryable<Item> ordered = (defaultRequest.OrderBy.ToLower(), defaultRequest.OrderType.ToLower()) switch
        {
            ("name", "asc") => _entities.OrderBy(item => item.Name),
            ("name", "desc") => _entities.OrderByDescending(item => item.Name),
            ("type", "asc") => _entities.OrderBy(item => item.Type),
            ("type", "desc") => _entities.OrderByDescending(item => item.Type),
            ("rarity", "asc") => _entities.OrderBy(item => item.Rarity),
            ("rarity", "desc") => _entities.OrderByDescending(item => item.Rarity),
            ("base_cost", "asc") => _entities.OrderBy(item => item.BaseCost),
            ("base_cost", "desc") => _entities.OrderByDescending(item => item.BaseCost),
            ("sell_price", "asc") => _entities.OrderBy(item => item.SellPrice),
            ("sell_price", "desc") => _entities.OrderByDescending(item => item.SellPrice),
            ("is_visible", "asc") => _entities.OrderBy(item => item.IsVisible),
            ("is_visible", "desc") => _entities.OrderByDescending(item => item.IsVisible),
            ("game", "asc") => _entities.OrderBy(item => item.Game),
            ("game", "desc") => _entities.OrderByDescending(item => item.Game),
            _ => _entities.OrderBy(item => item.Name),
        };
        IQueryable<Item>? where = Filter(ordered, defaultRequest);

        return await (where ?? ordered).Skip((defaultRequest.Page - 1) * defaultRequest.Count)
            .Take(defaultRequest.Count).ToListAsync();
    }

    public async Task<int> GetCount(DefaultRequest defaultRequest)
    {
        IQueryable<Item>? where = Filter(_entities, defaultRequest);
        return await (where != null ? where.CountAsync() : _entities.CountAsync());
    }
}