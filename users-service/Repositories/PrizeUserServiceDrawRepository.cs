using Microsoft.EntityFrameworkCore;
using UsersService.Models;
using UsersService.Models.DbModels.MinorBonuses;

namespace UsersService.Repositories;

public class PrizeUserServiceDrawRepository(ApplicationDbContext dbContext, ILogger<PrizeUserServiceDrawRepository> logger) : IPrizeUserServiceRepository
{
    private DbSet<PrizeDraw> Entities { get; set; } = dbContext.PriceDraws;
    public async Task<List<PrizeDraw>?> GetAll()
    {
        return await Entities
            .Include(e => e.Participants)
            .ToListAsync();
    }

    public async Task<PrizeDraw?> Get(string id, bool isReadonly = false)
    {
        var entities = Entities;
        if (isReadonly)
        {
            entities = (DbSet<PrizeDraw>)Entities.AsNoTracking();
        }
        var prizeDraw = await entities
            .Include(e => e.Participants)
            .FirstOrDefaultAsync(e => e.Id == id);
        return prizeDraw;
    }

    public async Task<bool> Add(PrizeDraw entity, bool flush = true)
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

    public async Task<bool> Update(PrizeDraw entity)
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
            return true;
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

    private async Task Save()
    {
        await dbContext.SaveChangesAsync();
    }

    public async Task<PrizeDraw?> GetCurrentPrizeDraw()
    {
        var prizeDraw = await Entities
            .Include(e => e.Participants)
            .Where(e => e.IsFinished == false)
            .OrderBy(e => e.DrawDate).FirstOrDefaultAsync();
        return prizeDraw;
    }


}