using System.Net;
using DataTransferLib.CommunicationsServices;
using DtoClassLibrary.DataTransferObjects.Bonus;
using DtoClassLibrary.DataTransferObjects.Financial.Models;
using FinancialService.Database;
using FinancialService.Database.Models.Bonuses;
using Microsoft.EntityFrameworkCore;

namespace FinancialService.Services;

public class BonusService(ApplicationDbContext context, ILogger<BonusService> logger, CasesCommService casesCommService)
{

    public async Task<UserBonusRecord?> GetLatestUserBonus(string userId)
    {
        var finRecord = await context.FinancialDatas
            .Include(f => f.Bonuses)
            .FirstOrDefaultAsync(f => f.UserId == userId);

        var lastBonus = finRecord?.Bonuses?
            .Where(b => b.IsWheelBonus)
            .Where(b => !b.IsUsed)
            .OrderByDescending(b => b.TimeGotBonus).FirstOrDefault();

        return lastBonus;

    }

    public async Task MarkUserBonusAsUsed(string bonusRecordId)
    {
        var userBonusRecord = await context.UserBonusRecords.FirstOrDefaultAsync(b => b.Id == bonusRecordId);

        if (userBonusRecord != null)
        {
            userBonusRecord.IsUsed = true;
            await context.SaveChangesAsync();
        }
    }

    public async Task ApplyBonusToTransaction(TransactionParams transactionParams)
    {
        var latestBonus = await GetLatestUserBonus(transactionParams.UserId);
        switch (latestBonus.Bonus.BonusType)
        {
            case BonusType.None:
                throw new ArgumentException("No base bonus allowed");
            case BonusType.DepositBonus:
                await ApplyDepositBonusToTransaction(latestBonus, transactionParams);
                break;
            case BonusType.FreeCaseBonus:
                break;
            case BonusType.ItemBonus:
                if (latestBonus.Bonus is ItemBonus itemBonus && itemBonus.IsDepositDependent)
                {
                    await ApplyFreeItemBonus(latestBonus, transactionParams);
                }
                break;
            case BonusType.RandomCaseBonus:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public async Task ApplyFreeItemBonus(UserBonusRecord bonusRecord, TransactionParams transactionParams)
    {
        var bonus = bonusRecord.Bonus as ItemBonus;
        if (bonus == null)
        {
            logger.LogError("Error in casting to item bonus");
            return;
        }

        var result = await casesCommService.ApplyItemBonus((ItemBonusDto)bonus.ConvertToBonusDto(), transactionParams.UserId);
        if (result == HttpStatusCode.OK)
        {
            try
            {
                bonusRecord.IsUsed = true;
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
            }
        }
    }


    public async Task ApplyFiveKBonus(string userId)
    {

    }

    public async Task<CashbackBonus?> GetCashBackBonusIfAvailable(TransactionParams transactionParams)
    {
        try
        {
            var bonus = await GetLatestUserBonus(transactionParams.UserId);
            if (bonus == null)
            {
                return null;
            }

            var cashback = (CashbackBonus)bonus.Bonus;

            if (bonus.TimeGotBonus + cashback.Duration > DateTime.UtcNow)
            {
                logger.LogInformation("Bonus expired");

                bonus.IsUsed = true;
                await context.SaveChangesAsync();
                return null;
            }

            return cashback;
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return null;
        }
    }

    public async Task ApplyDiscountBonusToTransaction(TransactionParams transactionParams)
    {
        var baseAmount = transactionParams.Amount;
        var bonus = await GetLatestUserBonus(transactionParams.UserId);
        var discount = (DiscountBonus)(bonus.Bonus);
        if (transactionParams is CasePurchaseTransactionParams casePurchase)
        {
            if (casePurchase.Quantity > 1)
            {
                var singleCost = casePurchase.Amount / casePurchase.Quantity;

                var discountedOneCase = singleCost - singleCost * discount.DiscountPercentage;

                transactionParams.Amount = discountedOneCase + (singleCost * (casePurchase.Quantity - 1));
            }
            else
            {
                transactionParams.Amount -= transactionParams.Amount * discount.DiscountPercentage;
            }

            try
            {
                bonus.IsUsed = true;
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                bonus.IsUsed = false;
                logger.LogError($"Failed to apply discount. Error: {e.Message}. Original amount: {baseAmount}, Calculated amount: {transactionParams.Amount}");
                transactionParams.Amount = baseAmount;
            }
        }
    }



    public async Task<bool> CheckIfUserHasActiveBonus(string userId)
    {
        var bonus = await GetLatestUserBonus(userId);
        return bonus != null;
    }

    public async Task<bool> CheckIfUserHasActiveDiscountBonus(string userId)
    {
        var bonus = await GetLatestUserBonus(userId);
        return bonus != null && bonus.Bonus.BonusType == BonusType.DiscountBonus;
    }


    public async Task ApplyDepositBonusToTransaction(UserBonusRecord bonusRecord, TransactionParams transactionParams)
    {
        var initialAmount = transactionParams.Amount;
        var latestBonus = await GetLatestUserBonus(transactionParams.UserId);
        var bonus = bonusRecord.Bonus as DepositBonus;
        if (bonus != null)
        {
            logger.LogError("error casting in deposit bonus");
            return;
        }

        switch (bonus.Mtype)
        {
            case MultiplierType.Percentage:
                transactionParams.Amount += transactionParams.Amount * bonus.BonusMultiplier;
                break;
            case MultiplierType.Multiply:
                var addedAmount = transactionParams.Amount * bonus.BonusMultiplier;
                if (addedAmount > bonus.DepositCap)
                {
                    addedAmount = bonus.DepositCap;
                }
                transactionParams.Amount += addedAmount;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        try
        {
            latestBonus.IsUsed = true;
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            latestBonus.IsUsed = false;
            transactionParams.Amount = initialAmount;
        }
    }
    public async Task SaveData()
    {
        await context.SaveChangesAsync();
    }
}