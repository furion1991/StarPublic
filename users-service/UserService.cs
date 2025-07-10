using Microsoft.EntityFrameworkCore;
using UsersService.Models;
using UsersService.Models.DbModels;

namespace UsersService;

public class UserService(ApplicationDbContext dbContext)
{
    public async Task<List<User>> GetAllUsers()
    {
        var users = await dbContext.User
            .Include(e => e.DailyBonus)
            .Include(e => e.CurrentPriceDraw)
            .Include(e => e.UpgradeHistoryRecords)
            .Include(e => e.BlockStatus)
            .Include(e => e.ContractHistoryRecords)
            .Include(e => e.UserStatistics)
            .Include(e => e.UserRole)
            .Include(e => e.UserInventory)
            .ThenInclude(e => e.InventoryRecords)
            .ToListAsync();
        return users;
    }
    public async Task<User?> GetUserById(string id)
    {
        var userBase = await dbContext.User.Where(p => p.Id == id)
            .Include(e => e.DailyBonus)
            .Include(e => e.CurrentPriceDraw)
            .Include(e => e.UpgradeHistoryRecords)
            .Include(e => e.BlockStatus)
            .Include(e => e.ContractHistoryRecords)
            .Include(e => e.UserStatistics)
            .Include(e => e.UserRole)
            .Include(e => e.UserInventory)
            .ThenInclude(e => e.InventoryRecords)
            .FirstOrDefaultAsync();

        return userBase;
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        var userBase = await dbContext.User.Where(p => p.Email == email)
            .Include(e => e.UpgradeHistoryRecords)
            .Include(e => e.DailyBonus)
            .Include(e => e.CurrentPriceDraw)
            .Include(e => e.ContractHistoryRecords)
            .Include(e => e.BlockStatus)
            .Include(e => e.UserStatistics)
            .Include(e => e.UserRole)
            .Include(e => e.UserInventory)
            .ThenInclude(e => e.InventoryRecords)
            .FirstOrDefaultAsync();
        return userBase;
    }
}