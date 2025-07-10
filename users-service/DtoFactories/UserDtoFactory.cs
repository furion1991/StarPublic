using DataTransferLib.CacheServices;
using DataTransferLib.CommunicationsServices;
using DataTransferLib.DataTransferObjects.Common;
using DataTransferLib.DataTransferObjects.Users;
using DtoClassLibrary.DataTransferObjects.CasesItems.Models;
using DtoClassLibrary.DataTransferObjects.Users;
using UsersService.Models.DbModels;
using UsersService.Models.DbModels.MinorBonuses;
using Log = Serilog.Log;

namespace UsersService.DtoFactories;

public static class UserDtoFactory
{
    public static async Task<InternalUserDto> GetInternalUser(User user, CasesCommService casesCommService, CasesCache casesCache, ItemsCache itemsCache)
    {
        var newUser = await CreateUserGetDto(user, casesCommService, casesCache, itemsCache);

        var internalUser = new InternalUserDto()
        {
            PublicData = newUser,
            FailScore = user.UserStatistics.FailScore,
            TotalCasesSpent = user.UserStatistics.TotalCasesSpent,
            TotalCasesProfit = user.UserStatistics.TotalCasesProfit,
            ChanceBoost = user.ChanceBoost
        };
        return internalUser;
    }


    public static async Task<UserDto> CreateUserGetDto(User user, CasesCommService casesCommService, CasesCache casesCache, ItemsCache itemsCache)
    {
        var allItems = (await itemsCache.GetAllItemsFromCache())?.ToDictionary(e => e.Id, e => e)
                       ?? new Dictionary<string, ItemDto>();

        if (allItems.Count == 0)
        {
            var response = await casesCommService.GetAllItems(new DefaultRequest { Count = int.MaxValue });
            allItems = response?.Result?.ToDictionary(e => e.Id, e => e) ?? new Dictionary<string, ItemDto>();
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Phone = user.Phone,
            IsDeleted = user.IsDeleted,
            UserName = user.UserName,
            ProfileImagePath = user.ProfileImagePath,
            DateOfRegistration = user.DateOfRegistration,
            UserStatistics = new UserStatisticsResponse
            {
                Id = user.UserStatistics.Id ?? string.Empty,
                CasesBought = user.UserStatistics.CasesBought,
                LuckBaraban = user.UserStatistics.LuckBaraban,
                OrdersPlaced = user.UserStatistics.OrdersPlaced,
                PromocodesUsed = user.UserStatistics.PromocodesUsed,
                CrashRocketsPlayed = user.UserStatistics.CrashRocketsPlayed,
                ContractsPlaced = user.UserStatistics.ContractsPlaced,
                TotalCasesProfit = user.UserStatistics.TotalCasesProfit,
                TotalCasesSpent = user.UserStatistics.TotalCasesSpent,
                TotalContractsProfit = user.UserStatistics.TotalContractsProfit,
                TotalContractsSpent = user.UserStatistics.TotalContractsSpent,
                UpgradesPlayed = user.UserStatistics.UpgradesPlayed,
            },
            BlockStatus = new BlockStatusDto
            {
                Id = user.BlockStatus.Id,
                Reason = user.BlockStatus.Reason,
                IsBlocked = user.BlockStatus.IsBlocked,
                PerformedById = user.BlockStatus.PerformedById
            },
            UserRole = new()
            {
                Id = user.UserRole.Id,
                Name = user.UserRole.Name
            },
            UserInventory = new InventoryRecordDto
            {
                Id = user.UserInventory?.Id,
                ItemsUserInventory = new List<ItemRecordDto>()
            },
            ContractHistoryRecords = new List<ContractHistoryRecordDto>(),
            UpgradeHistoryRecords = new List<UpgradeHistoryRecordDto>()
        };

        if (user.UserInventory?.InventoryRecords != null)
        {
            foreach (var itemsUser in user.UserInventory.InventoryRecords)
            {
                if (itemsUser.ItemId == null || !allItems.ContainsKey(itemsUser.ItemId))
                {
                    Log.Warning("ItemId {ItemId} not found in cache for user {UserId}", itemsUser.ItemId, user.Id);
                    continue;
                }

                userDto.UserInventory.ItemsUserInventory.Add(new ItemRecordDto
                {
                    Id = itemsUser.Id,
                    ItemId = itemsUser.ItemId,
                    Userinventoryid = itemsUser.UserInventoryId,
                    ItemRecordState = itemsUser.ItemRecordState,
                    ItemDto = allItems[itemsUser.ItemId]
                });
            }
        }

        if (user.ContractHistoryRecords is { Count: > 0 })
        {
            foreach (var contract in user.ContractHistoryRecords)
            {
                var contractDto = new ContractHistoryRecordDto
                {
                    DateOfContract = contract.DateOfContract,
                    ItemsUsedOnThisContract = new List<ItemDto>()
                };

                foreach (var itemId in contract.ItemsFromIds)
                {
                    if (!allItems.ContainsKey(itemId))
                    {
                        Log.Warning("Contract itemId {ItemId} not found for user {UserId}", itemId, user.Id);
                        continue;
                    }
                    contractDto.ItemsUsedOnThisContract.Add(allItems[itemId]);
                }

                if (allItems.ContainsKey(contract.ResultItemId))
                {
                    contractDto.ResultItem = allItems[contract.ResultItemId];
                    userDto.ContractHistoryRecords.Add(contractDto);
                }
                else
                {
                    Log.Warning("Result item {ItemId} not found for contract of user {UserId}", contract.ResultItemId, user.Id);
                }
            }
        }

        if (user.UpgradeHistoryRecords is { Count: > 0 })
        {
            foreach (var upgrade in user.UpgradeHistoryRecords)
            {
                if (!allItems.ContainsKey(upgrade.ItemSpentId) || !allItems.ContainsKey(upgrade.ItemResultId))
                {
                    Log.Warning("Upgrade item missing: spent={SpentId}, result={ResultId}, user={UserId}",
                        upgrade.ItemSpentId, upgrade.ItemResultId, user.Id);
                    continue;
                }

                userDto.UpgradeHistoryRecords.Add(new UpgradeHistoryRecordDto
                {
                    IsSuccessful = upgrade.IsSuccessful,
                    DateOfUpgrade = upgrade.DateOfUpgrade,
                    Chance = 0,
                    ItemUsedOnThisUpgrade = allItems[upgrade.ItemSpentId],
                    ResultItem = allItems[upgrade.ItemResultId]
                });
            }
        }

        if (user.DailyBonus != null)
        {
            userDto.DailyBonus = new DailyBonusDto
            {
                UserId = user.Id,
                Id = user.DailyBonus.Id,
                Streak = user.DailyBonus.Streak,
                IsUsedToday = user.DailyBonus.IsUsedToday,
                Date = user.DailyBonus.TimeGotLastBonus,
                Amount = user.DailyBonus.Streak * 10
            };
        }

        return userDto;
    }

}