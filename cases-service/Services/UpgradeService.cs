using System.Net;
using CasesService.Database.Models;
using CasesService.Utility;
using DataTransferLib.CommunicationsServices;
using DataTransferLib.DataTransferObjects.Common.Interfaces;
using DtoClassLibrary.DataTransferObjects.CasesItems;
using DtoClassLibrary.DataTransferObjects.CasesItems.Upgrade;
using DtoClassLibrary.DataTransferObjects.Users.Inventory;

namespace CasesService.Services;

public class UpgradeService(IRepository<Item> itemRepo, IRepository<Case> casesRepo, UsersCommService usersCommService)
{
    public async Task<GameResultDto?> UpgradeItem(UpgradeItemRequest request)
    {
        Console.WriteLine(
            $"[UpgradeService] 🔍 Start upgrade for {request.UserId} with {request.UserInventoryRecordId} -> {request.AttemptedItemId}");

        var userData = await usersCommService.GetInternalUserData(request.UserId);
        if (userData?.Result == null)
        {
            Console.WriteLine("[UpgradeService] ❌ User data not found");
            return null;
        }

        var user = userData.Result;
        var userStats = user.PublicData.UserStatistics;

        var userItem = user.PublicData.UserInventory.ItemsUserInventory
            .FirstOrDefault(i => i.Id == request.UserInventoryRecordId);
        if (userItem == null)
        {
            Console.WriteLine("[UpgradeService] ❌ User item not found");
            return null;
        }

        var userItemDb = await itemRepo.Get(userItem.ItemId);
        if (userItemDb == null || userItemDb.SellPrice == null)
        {
            Console.WriteLine("[UpgradeService] ❌ User item DB not found or sell price is null");
            return null;
        }

        var attemptedItemDb = await itemRepo.Get(request.AttemptedItemId);
        if (attemptedItemDb == null || attemptedItemDb.SellPrice == null)
        {
            Console.WriteLine("[UpgradeService] ❌ Attempted item DB not found or sell price is null");
            return null;
        }

        decimal userItemPrice = userItemDb.SellPrice.Value;
        decimal attemptedItemPrice = attemptedItemDb.SellPrice.Value;

        if (attemptedItemPrice <= userItemPrice)
        {
            Console.WriteLine("[UpgradeService] ❌ Not allowed upgrade item lower cost");
            return null;
        }

        // логирование цен
        Console.WriteLine(
            $"[UpgradeService] 📊 Prices -> UserItem: {userItemPrice}, AttemptedItem: {attemptedItemPrice}");

        var chance = UpgradeChanceCalculator.CalculateChance(
            userItemPrice,
            attemptedItemPrice,
            user.FailScore,
            user.ChanceBoost
        );

        var roll = (decimal)RandomStardropNumberGenerator.NextSecureDouble();
        var success = roll <= chance;

        // логирование всех расчётов
        Console.WriteLine(
            $"[UpgradeService] 🎯 Chance: {Math.Round(chance * 100, 2)}% | FailScore: {user.FailScore} | Boost: {user.ChanceBoost}");
        Console.WriteLine($"[UpgradeService] 🎲 Roll: {Math.Round(roll, 4)} → {(success ? "SUCCESS ✅" : "FAIL ❌")}");

        await usersCommService.SetItemsState(new ChangeStateRequest()
        {
            InventoryRecordsIds = new List<string> { request.UserInventoryRecordId },
            ItemRecordState = ItemRecordState.UsedOnUpgrade
        });

        if (success)
        {
            var addItemResult = await usersCommService.AddItemToInventory(new AddRemoveItemRequest()
            {
                UserId = request.UserId,
                ItemInventoryRecordId = attemptedItemDb.Id,
                ItemRecordState = ItemRecordState.FromUpgrade,
                Quantity = 1,
                ItemImageUrl = attemptedItemDb.Image ?? ""
            });

            if (addItemResult?.Result == null)
            {
                Console.WriteLine("❌ Not added item to inventory");
                return null;
            }

            await usersCommService.UpdateStatsAfterUpgrade(new UpdateStatsAfterUpgradeRequest()
            {
                UserId = request.UserId,
                AddSpent = userItemPrice,
                AddProfit = attemptedItemPrice - userItemPrice,
                AddFailScore = 0,
                ResetFailScore = true,
                ItemSpent = userItemDb.CreateDto(),
                ItemGot = attemptedItemDb.CreateDto()
            });

            return addItemResult.Result;
        }
        else
        {
            var allItems = await itemRepo.GetAll();
            var fallbackCandidates = allItems
                .Where(i => i.SellPrice != null && i.SellPrice.Value < userItemPrice * 0.8m && i.IsVisible == true)
                .ToList();

            if (fallbackCandidates.Count == 0)
            {
                Console.WriteLine("⚠️ No fallback items found");
                return null;
            }

            var fallbackItem = fallbackCandidates.OrderBy(_ => Guid.NewGuid()).First();

            var addFallbackResult = await usersCommService.AddItemToInventory(new AddRemoveItemRequest()
            {
                UserId = request.UserId,
                ItemInventoryRecordId = fallbackItem.Id,
                ItemRecordState = ItemRecordState.FromUpgrade,
                Quantity = 1,
                ItemImageUrl = fallbackItem.Image ?? ""
            });

            if (addFallbackResult?.Result == null)
            {
                Console.WriteLine("❌ Failed to add fallback item");
                return null;
            }

            await usersCommService.UpdateStatsAfterUpgrade(new UpdateStatsAfterUpgradeRequest()
            {
                UserId = request.UserId,
                AddSpent = userItemPrice,
                AddProfit = fallbackItem.SellPrice!.Value - userItemPrice,
                AddFailScore = 1,
                ResetFailScore = false,
                ItemSpent = userItemDb.CreateDto(),
                ItemGot = fallbackItem.CreateDto()
            });

            return addFallbackResult.Result;
        }
    }
}