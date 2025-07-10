using CasesService.Database.Models;
using CasesService.Repositories;
using DataTransferLib.DataTransferObjects.Common.Interfaces;
using DtoClassLibrary.DataTransferObjects.CasesItems;
using DtoClassLibrary.DataTransferObjects.CasesItems.Contracts;
using Newtonsoft.Json;
using System.Security.Cryptography;
using DataTransferLib.CommunicationsServices;
using DataTransferLib.DataTransferObjects.Common;
using DataTransferLib.DataTransferObjects.Users;
using DtoClassLibrary.DataTransferObjects.Audit;
using DtoClassLibrary.DataTransferObjects.Users;
using DtoClassLibrary.DataTransferObjects.Users.Inventory;

namespace CasesService.Services;

public class ContractService(IRepository<Case> casesRepository,
    IRepository<Item> itemRepository,
    ILogger<ContractService> logger,
    UsersCommService usersCommService,
    RabbitMqService rabbitMqService)
{

    public async Task<ContractPreviewResponse> GetPreview(ContractPreviewRequest request)
    {
        logger.LogInformation($"Executing request: {JsonConvert.SerializeObject(request)}");

        if (request.ItemsList == null || request.ItemsList.Count < 3 || request.ItemsList.Count > 10)
        {
            throw new ArgumentException("Items list must contain from 3 to 10 items");
        }

        var itemRepo = itemRepository as ItemRepository;

        var distinctIds = request.ItemsList.Distinct().ToList();
        var items = await itemRepo!.GetItemsByIdsAsync(distinctIds);

        if (items.Count != distinctIds.Count)
        {
            throw new ArgumentException("Some items not found");
        }

        var itemCountMap = request.ItemsList
            .GroupBy(id => id)
            .ToDictionary(g => g.Key, g => g.Count());

        decimal total = itemCountMap.Sum(kvp =>
        {
            var item = items.First(i => i.Id == kvp.Key);
            return (item.SellPrice ?? 0) * kvp.Value;
        });

        decimal min = Math.Round(total * 0.25m, 2);
        decimal max = Math.Round(total * 4.0m, 2);

        return new ContractPreviewResponse()
        {
            MinValue = min,
            MaxValue = max
        };
    }


    private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

    private static double NextSecureDouble()
    {
        var bytes = new byte[8];
        _rng.GetBytes(bytes);
        ulong uint64 = BitConverter.ToUInt64(bytes, 0);
        return (double)uint64 / ulong.MaxValue;
    }

    public async Task<bool> IsItemsAvailableForContract(ContractExecuteRequest request)
    {
        var userData = await usersCommService.GetInternalUserData(request.UserId);
        var inventory = userData.Result.PublicData.UserInventory.ItemsUserInventory;

        foreach (var item in inventory)
        {
            if (request.ItemRecordIds.Contains(item.Id))
            {
                if (item.ItemRecordState is ItemRecordState.Sold
                    or ItemRecordState.UsedOnContract
                    or ItemRecordState.UsedOnUpgrade
                    or ItemRecordState.Withdrawn)
                {
                    return false;
                }
            }
        }

        return true;
    }



    public async Task<GameResultDto?> ExecuteContract(ContractExecuteRequest request)
    {
        var userData = await usersCommService.GetInternalUserData(request.UserId);
        if (userData?.Result == null) return null;

        var user = userData.Result;
        var stats = user.PublicData.UserStatistics;

        var itemsGet = new List<ItemRecordDto>();
        foreach (var itemRecordId in request.ItemRecordIds)
        {
            var itemRecord = userData.Result.PublicData.UserInventory.ItemsUserInventory
                .FirstOrDefault(i => i.Id == itemRecordId);
            itemRecord.ItemDto = (await itemRepository.Get(itemRecord.ItemId)).CreateDto();
            itemsGet.Add(itemRecord);

        }

        var itemRecords = itemsGet;
        logger.LogInformation($"item records {JsonConvert.SerializeObject(itemRecords)}");
        if (itemRecords.Count != request.ItemRecordIds.Count)
            throw new ArgumentException("Некоторые предметы не найдены");

        var items = itemRecords.Select(r => r.ItemDto).Where(i => i != null && i.SellPrice > 0).ToList();
        if (items.Count == 0) return null;

        decimal totalValue = items.Sum(i => i.SellPrice!.Value);
        const decimal targetRTP = 0.9m;
        decimal alpha = 1.1m;

        var allItems = (await itemRepository.GetAll()).Where(e => e.IsVisible == true);
        var candidates = allItems.Where(i => i.SellPrice >= totalValue * 0.25m && i.SellPrice <= totalValue * 4m).ToList();
        if (!candidates.Any()) return null;

        var weights = new List<(Item item, decimal weight)>();
        decimal expectedTotal = 0;

        foreach (var item in candidates)
        {
            var priceRatio = item.SellPrice!.Value / totalValue;
            var weight = (decimal)Math.Exp(-(double)(alpha * priceRatio));
            weight *= 1 + 0.3m * user.FailScore;
            weight *= (decimal)user.ChanceBoost;

            if (stats.ContractsPlaced < 3)
                weight *= 1.2m;

            weights.Add((item, weight));
            expectedTotal += item.SellPrice.Value * weight;
        }

        var totalWeight = weights.Sum(w => w.weight);
        if (totalWeight == 0) return null;

        var avgDropValue = expectedTotal / totalWeight;
        var actualRTP = avgDropValue / totalValue;
        var correction = Math.Clamp(targetRTP / actualRTP, 0.5m, 2.0m);

        weights = weights.Select(w => (w.item, w.weight * correction)).ToList();
        totalWeight = weights.Sum(w => w.weight);

        var roll = (decimal)NextSecureDouble() * totalWeight;
        decimal cumulative = 0;
        Item? selected = null;

        foreach (var (item, weight) in weights)
        {
            cumulative += weight;
            if (roll <= cumulative)
            {
                selected = item;
                break;
            }
        }

        if (selected == null) return null;

        bool isFail = selected.SellPrice < totalValue;
        var addItemResult = await usersCommService.AddItemToInventory(new AddRemoveItemRequest()
        {
            UserId = request.UserId,
            ItemRecordState = ItemRecordState.FromContract,
            ItemInventoryRecordId = selected.Id,
            ItemImageUrl = "",
            Quantity = 1

        });

        await usersCommService.UpdateStatsAfterContract(new UpdateContractsStatisticsRequest()
        {
            UserId = request.UserId,
            AddContractsPlaced = 1,
            AddFailScore = isFail ? 1 : 0,
            ResetFailScore = !isFail,
            AddSpent = totalValue,
            AddProfit = selected.SellPrice!.Value - totalValue,
            ItemsUsedInContract = items.Select(e => e.Id).ToList(),
            ResultItem = selected.Id

        });

        var changeStateRequest = new ChangeStateRequest()
        {
            ItemRecordState = ItemRecordState.UsedOnContract,
            InventoryRecordsIds = new List<string>()
        };
        changeStateRequest.InventoryRecordsIds.AddRange(request.ItemRecordIds);
        await usersCommService.SetItemsState(changeStateRequest);

        var droppedItem = new DroppedItemDto()
        {
            Item = selected.CreateDto(),
            UserId = request.UserId,
            SellPrice = selected.SellPrice!.Value,
            CaseId = "",
            OpenedTimeStamp = DateTime.UtcNow
        };


        await rabbitMqService.SendLog(
            "User №" + request.UserId + " contract executed " + 1 + " times", droppedItem,
            LTYPE.Case, objectId: "", type: 1);

        return addItemResult?.Result;
    }


}