using Microsoft.EntityFrameworkCore;
using UsersService.Models;
using UsersService.Models.DbModels.MinorBonuses;

namespace UsersService.Repositories;

public class PrizeDrawResultsUserServiceRepository(ApplicationDbContext dbContext, ILogger<PrizeDrawResultsUserServiceRepository> logger) : IPrizeDrawResultUserServiceRepository
{
    private DbSet<PrizeDrawResult> Entities { get; set; } = dbContext.PriceDrawResults;



    public async Task<PrizeDrawResult?> GetLastPrizeDrawResult()
    {
        var last = await Entities.Include(e => e.WinnerUser)
            .OrderByDescending(e => e.DateDrawFinished)
            .FirstOrDefaultAsync();
        return last;
    }
    public async Task<List<PrizeDrawResult>?> GetAll()
    {
        return await Entities
            .Include(e => e.WinnerUser)
            .ToListAsync();
    }

    public async Task<PrizeDrawResult?> Get(string id, bool isReadonly = false)
    {
        var entities = Entities;
        if (isReadonly)
        {
            entities = (DbSet<PrizeDrawResult>)Entities.AsNoTracking();
        }
        var prizeDrawResult = await entities
            .Include(e => e.WinnerUser)
            .FirstOrDefaultAsync(e => e.Id == id);
        return prizeDrawResult;
    }

    public async Task<bool> Add(PrizeDrawResult entity, bool flush = true)
    {
        await Entities.AddAsync(entity);
        if (flush)
        {
            try
            {
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return false;
            }
        }

        return true;
    }

    public async Task<bool> Update(PrizeDrawResult entity)
    {
        Entities.Update(entity);
        try
        {
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return false;
        }
    }

    public async Task<bool> Delete(string id)
    {
        var entity = await Get(id);
        if (entity == null)
        {
            return false;
        }
        Entities.Remove(entity);
        try
        {
            await dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return false;
        }
    }
}