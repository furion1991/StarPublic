using System.Net;
using DataTransferLib.CommunicationsServices;
using DataTransferLib.DataTransferObjects.Financial.Models;
using DtoClassLibrary.DataTransferObjects.Financial.Models;
using UsersService.Models;
using UsersService.Models.DbModels;
using UsersService.Models.DbModels.MinorBonuses;
using UsersService.Repositories;

namespace UsersService.Services;

public class SmallBonusService(IUserServiceRepository<User> userUserServiceRepository, ILogger<SmallBonusService> logger, FinancialCommService financialCommService, ApplicationDbContext dbContext)
{

    public async Task<bool> CheckIfUserAlreadyUsedBonusToday(string userId)
    {
        var user = await userUserServiceRepository.Get(userId);
        if (user == null || user.DailyBonus == null)
        {
            logger.LogInformation("User already used daily bonus");
            return false;
        }
        return user.DailyBonus.TimeGotLastBonus.Date == DateTime.UtcNow.Date;
    }


    public async Task<bool> AddSmallBonus(string userId)
    {
        var user = await userUserServiceRepository.Get(userId);
        if (user == null)
        {
            logger.LogError($"User with id {userId} not found");
            return false;
        }

        if (user.DailyBonus == null)
        {
            user.DailyBonus = new DailyBonus
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Streak = 1,
                TimeGotLastBonus = DateTime.UtcNow
            };
        }
        else
        {
            user.DailyBonus.Streak++;
            user.DailyBonus.TimeGotLastBonus = DateTime.UtcNow;
        }
        if (user.DailyBonus.Streak <= 0)
        {
            user.DailyBonus.Streak = 1;
        }

        var transaction = new TransactionParams()
        {
            UserId = userId,
            Type = TTYPE.Deposit,
            PaymentType = PTYPE.Bank,
            Amount = 10 * user.DailyBonus.Streak,
            BalanceAfter = 0,
            BalanceBefore = 0,
        };

        var finResult = await financialCommService.MakeTransaction(transaction);
        if (finResult is not { StatusCode: HttpStatusCode.OK })
        {
            return false;
        }

        try
        {
            await userUserServiceRepository.Update(user);
            return true;
        }
        catch (Exception e)
        {

            logger.LogError(e.Message);
            return false;
        }
    }
}