using Microsoft.EntityFrameworkCore;
using UsersService.Models;
using UsersService.Models.DbModels;

namespace UsersService.Repositories;

public class UserUserServiceRepository(ApplicationDbContext dbContext, ILogger<UserUserServiceRepository> logger) : IUserServiceRepository<User>
{
    public async Task<List<User>?> GetAll()
    {
        return await dbContext.User
            .Include(e => e.CurrentPriceDraw)
            .Include(e => e.WonDraws)
            .Include(e => e.ContractHistoryRecords)
            .Include(e => e.DailyBonus)
            .Include(e => e.BlockStatus)
            .Include(e => e.UpgradeHistoryRecords)
            .Include(e => e.UserStatistics)
            .Include(e => e.UserInventory)
            .ThenInclude(e => e.InventoryRecords)
            .ToListAsync();
    }

    public Task<User?> Get(string id, bool isReadonly = false)
    {
        IQueryable<User> entities = isReadonly ? dbContext.User.AsNoTracking() : dbContext.User;
        var user = entities
            .Include(e => e.CurrentPriceDraw)
            .Include(e => e.WonDraws)
            .Include(e => e.ContractHistoryRecords)
            .Include(e => e.DailyBonus)
            .Include(e => e.BlockStatus)
            .Include(e => e.UpgradeHistoryRecords)
            .Include(e => e.UserStatistics)
            .Include(e => e.UserInventory)
            .ThenInclude(e => e.InventoryRecords)
            .FirstOrDefaultAsync(e => e.Id == id);
        return user;
    }

    public async Task<bool> Add(User entity, bool flush = false)
    {
        dbContext.User.Add(entity);
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

    public Task<bool> Update(User entity)
    {
        dbContext.User.Update(entity);
        try
        {
            dbContext.SaveChanges();
            return Task.FromResult(true);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return Task.FromResult(false);
        }
    }

    public Task<bool> Delete(string id)
    {
        dbContext.User.Remove(new User { Id = id });
        try
        {
            dbContext.SaveChanges();
            return Task.FromResult(true);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return Task.FromResult(false);
        }
    }


}