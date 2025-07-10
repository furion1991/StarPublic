using System.Text.Json;
using CasesService.Database.Models;
using DataTransferLib.DataTransferObjects.CasesItems;
using DataTransferLib.DataTransferObjects.Common;
using DtoClassLibrary.DataTransferObjects.CasesItems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CasesService.Services.Models;

/// <summary>Сервис для обслуживания всех запросов к БД сущности кейса</summary>
public class CaseService : ControllerBase
{
    /// <summary>Метод фильтрации списка кейсов по определённому условию</summary>
    private static IQueryable<Case>? Filter(IQueryable<Case> objs, DefaultRequest defaultRequest)
    {
        if (defaultRequest.FilterBy == null || defaultRequest.FilterValue == null)
            return null;

        IQueryable<Case> defaultWhere = objs.Where(x => x.Name == defaultRequest.FilterValue);
        switch(defaultRequest.FilterBy.ToLower()) {
            case "name":
                return objs.Where(x => x.Name == defaultRequest.FilterValue);
            case "type":
                if (Enum.TryParse(defaultRequest.FilterValue, out ECaseType type))
                    return objs.Where(x => x.Type == type);
                else
                    return defaultWhere;
            case "price":
                decimal[]? prices = JsonSerializer.Deserialize<decimal[]>(defaultRequest.FilterValue);
                if (prices != null) {
                    if (prices[0] > prices[1])
                        (prices[0], prices[1]) = (prices[1], prices[0]);
                    return objs.Where(x => x.Price >= prices[0] && x.Price <= prices[1]);
                }
                else
                    return defaultWhere;
            /*case ("is_visible", "equals"):
                if (bool.TryParse(request.FilterValue, out bool isVisible))
                    return objs.Where(x => x.IsVisible == isVisible);
                else
                    return defaultWhere;*/
            default:
                return defaultWhere;
        }
    }

    /// <summary>Метод пагинации, фильтрации и сортировки</summary>
    public static async Task<List<Case>> GetOrderBy(DbSet<Case> entity, DefaultRequest defaultRequest) 
    {
        //RequestService.CheckPaginationParams(ref request);
        IOrderedQueryable<Case> ordered = (defaultRequest.OrderBy.ToLower(), defaultRequest.OrderType.ToLower()) switch
        {
            ("name", "asc") => entity.OrderBy(x => x.Name),
            ("name", "desc") => entity.OrderByDescending(x => x.Name),
            ("type", "asc") => entity.OrderBy(x => x.Type),
            ("type", "desc") => entity.OrderByDescending(x => x.Type),
            ("image", "asc") => entity.OrderBy(x => x.Image),
            ("image", "desc") => entity.OrderByDescending(x => x.Image),
            ("price", "asc") => entity.OrderBy(x => x.Price),
            ("price", "desc") => entity.OrderByDescending(x => x.Price),
            ("open_limit", "asc") => entity.OrderBy(x => x.OpenLimit),
            ("open_limit", "desc") => entity.OrderByDescending(x => x.OpenLimit),
            ("discount", "asc") => entity.OrderBy(x => x.Discount),
            ("discount", "desc") => entity.OrderByDescending(x => x.Discount),
            ("old_price", "asc") => entity.OrderBy(x => x.OldPrice),
            ("old_price", "desc") => entity.OrderByDescending(x => x.OldPrice),
            _ => entity.OrderBy(x => x.Name),
        };
        IQueryable<Case>? where = Filter(ordered, defaultRequest);

        return await (where ?? ordered).Skip((defaultRequest.Page - 1) * defaultRequest.Count).Take(defaultRequest.Count).ToListAsync();
    }

    public static async Task<int> GetCount(DbSet<Case> entity, DefaultRequest defaultRequest) {
        IQueryable<Case>? where = Filter(entity, defaultRequest);
        return await (where != null ? where.CountAsync() : entity.CountAsync());
    }
}