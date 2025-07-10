using CasesService.Database;
using CasesService.Database.Models;
using DataTransferLib.CommunicationsServices;
using DataTransferLib.DataTransferObjects.CasesItems;
using DataTransferLib.DataTransferObjects.Common.Interfaces;
using DtoClassLibrary.DataTransferObjects.Users;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace CasesService.Services;

public class CaseOpenService(
    UsersCommService usersCommService,
    IRepository<Case> casesRepository)
{
    private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

    private static double NextSecureDouble()
    {
        var bytes = new byte[8];
        _rng.GetBytes(bytes);
        ulong uint64 = BitConverter.ToUInt64(bytes, 0);
        return (double)uint64 / ulong.MaxValue;
    }

    public async Task<Item?> OpenCase(OpenCaseRequest request)
    {
        var userData = await usersCommService.GetInternalUserData(request.UserId);
        if (userData?.Result == null) return null;

        var user = userData.Result;
        var userStats = user.PublicData.UserStatistics;

        var caseToOpen = await casesRepository.Get(request.CaseId!);
        if (caseToOpen is null || caseToOpen.Price is null) return null;

        var price = caseToOpen.Price.Value;
        var alpha = caseToOpen.Alpha;

        var items = caseToOpen.ItemsCases?
            .Select(ic => ic.Item)
            .Where(i => i != null && i.SellPrice > 0)
            .ToList();

        if (items == null || items.Count == 0) return null;

        const decimal targetRTP = 0.85m;

        var weights = new List<(Item item, decimal weight)>();
        decimal expectedTotal = 0;

        foreach (var item in items)
        {
            var priceRatio = item.SellPrice!.Value / price;
            var weight = (decimal)Math.Exp(-(double)((decimal)alpha * priceRatio));

            // failScore boost
            weight *= 1 + 0.3m * user.FailScore;

            // chanceBoost
            weight *= (decimal)user.ChanceBoost;

            // бонус новичка
            if (caseToOpen.BonusNewUserEnabled && userStats.CasesBought < 3)
                weight *= (decimal)1.3;

            weights.Add((item, weight));
            expectedTotal += item.SellPrice.Value * weight;
        }

        var totalWeight = weights.Sum(w => w.weight);
        if (totalWeight == 0) return items.OrderBy(_ => Guid.NewGuid()).First();

        var avgDropValue = expectedTotal / totalWeight;
        var actualRTP = avgDropValue / price;
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

        var isFail = selected.SellPrice < price;

        await usersCommService.UpdateStats(new UpdateStatisticsRequest()
        {
            UserId = request.UserId,
            AddFailScore = isFail ? 1 : 0,
            ResetFailScore = !isFail,
            AddCasesBought = 1,
            AddSpent = price,
            AddProfit = selected.SellPrice!.Value - price
        });

        caseToOpen.CurrentOpen += 1;
        caseToOpen.AddAccumulatedProfit();
        await casesRepository.Save();

        return selected;
    }


}
