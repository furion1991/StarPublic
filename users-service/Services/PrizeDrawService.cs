using System.Net;
using DataTransferLib.CommunicationsServices;
using DataTransferLib.DataTransferObjects.Financial.Models;
using DtoClassLibrary.DataTransferObjects.Bonus.SmallBonuses;
using DtoClassLibrary.DataTransferObjects.Financial.Models;
using Microsoft.AspNetCore.SignalR;
using UsersService.Models.DbModels;
using UsersService.Models.DbModels.MinorBonuses;
using UsersService.Repositories;

namespace UsersService.Services;

public class PrizeDrawService(IUserServiceRepository<User> userRepository,
    ILogger<PrizeDrawService> logger,
    IPrizeDrawResultUserServiceRepository prizeDrawResultsUserServiceRepository,
    IPrizeUserServiceRepository prizeUserServiceRepository,
    FinancialCommService financialCommService,
    IHubContext<PrizeDrawSender> sender)
{


    public async Task<PrizeDrawDto?> GetCurrentPrizeDrawDto()
    {
        var current = await prizeUserServiceRepository.GetCurrentPrizeDraw();

        var last = await prizeDrawResultsUserServiceRepository.GetLastPrizeDrawResult();
        if (current is null)
        {
            logger.LogInformation("No current prize draw");
            await CreatePrizeDraw();
            current = await prizeUserServiceRepository.GetCurrentPrizeDraw();
        }

        var prizeDrawDto = new PrizeDrawDto()
        {
            CurrentPrizeAmount = current.PrizeAmount,
            LastWinnerImageUrl = last?.WinnerUser?.ProfileImagePath,
            LastWinnerUserId = last?.WinnerUser?.Id,
            LastWonPrizeAmount = last?.PrizeAmount ?? 0,
            NextPrizeAmount = current.PrizeAmount + 10,
            SecondsRemainingTillDraw = Math.Max(0, (int)(current.DrawDate - DateTime.UtcNow).TotalSeconds),
            SubscribedUsers = current.Participants?.Count ?? 0
        };

        return prizeDrawDto;
    }


    public async Task<bool> IsUserParticipating(string userId)
    {
        var current = await prizeUserServiceRepository.GetCurrentPrizeDraw();
        if (current is null)
        {
            logger.LogInformation("No current prize draw");
            return false;
        }

        if (current.Participants.Any(e => e.Id == userId))
        {
            return true;
        }

        return false;
    }
    public async Task<PrizeDraw?> GetCurrentPrizeDraw()
    {
        return await prizeUserServiceRepository.GetCurrentPrizeDraw();
    }

    public async Task FinishCurrentPriceDraw()
    {
        try
        {
            var currentPrizeDraw = await prizeUserServiceRepository.GetCurrentPrizeDraw();

            if (currentPrizeDraw is null)
            {
                logger.LogWarning("FinishCurrentPrizeDraw called, but no current draw found. Creating new one...");
                await CreatePrizeDraw();
                return;
            }

            if (currentPrizeDraw.Participants?.Count <= 0)
            {
                logger.LogInformation("Draw finished with no participants. Creating empty result.");

                currentPrizeDraw.IsFinished = true;
                await prizeUserServiceRepository.Update(currentPrizeDraw);

                var drawResult = new PrizeDrawResult
                {
                    Id = Guid.NewGuid().ToString(),
                    Winner = "no winner",
                    WinnerUser = null,
                    PrizeAmount = currentPrizeDraw.PrizeAmount,
                    DateDrawFinished = DateTime.UtcNow
                };

                await prizeDrawResultsUserServiceRepository.Add(drawResult, true);
                return;
            }

            var winner = currentPrizeDraw.Participants[Random.Shared.Next(currentPrizeDraw.Participants.Count)];
            logger.LogInformation("Prize winner selected: {UserId}, prize amount: {Amount}", winner.Id, currentPrizeDraw.PrizeAmount);

            var transaction = new TransactionParams
            {
                UserId = winner.Id,
                Type = TTYPE.Bonus,
                PaymentType = PTYPE.Bank,
                Amount = currentPrizeDraw.PrizeAmount,
                BalanceAfter = 0,
                BalanceBefore = 0,
                FinancialDataId = ""
            };

            logger.LogInformation("Sending transaction: {@Transaction}", transaction);

            var result = await financialCommService.MakeTransaction(transaction);

            if (result is { StatusCode: HttpStatusCode.OK })
            {
                logger.LogInformation("Transaction completed successfully for user {UserId}", winner.Id);

                currentPrizeDraw.IsFinished = true;
                await prizeUserServiceRepository.Update(currentPrizeDraw);

                var drawResult = new PrizeDrawResult
                {
                    Id = Guid.NewGuid().ToString(),
                    Winner = winner.Id,
                    WinnerUser = winner,
                    PrizeAmount = currentPrizeDraw.PrizeAmount
                };

                await prizeDrawResultsUserServiceRepository.Add(drawResult, true);

                foreach (var participant in currentPrizeDraw.Participants)
                {
                    participant.CurrentPriceDraw = null;
                    participant.PriceDrawId = string.Empty;
                }
            }
            else
            {
                logger.LogWarning("Transaction failed with status {Status}, response: {Response}", result?.StatusCode, result?.Message);
            }

            await sender.Clients.All.SendAsync(PrizeDrawSender.DrawHubName, await GetCurrentPrizeDrawDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during FinishCurrentPriceDraw");
            throw; // пусть пробрасывается дальше, или можно подавить
        }
    }


    public async Task SubscribeToCurrentPrizeDraw(string userId)
    {
        var currentPrizeDraw = await prizeUserServiceRepository.GetCurrentPrizeDraw();
        var user = await userRepository.Get(userId);
        if (user is null)
        {
            logger.LogError($"User with id {userId} not found");
            return;
        }

        if (currentPrizeDraw.Participants.Any(e => e.Id == userId))
        {
            logger.LogInformation($"User {userId} is already subscribed to the current prize draw");
            return;
        }
        currentPrizeDraw.Participants.Add(user);
        user.CurrentPriceDraw = currentPrizeDraw;
        user.PriceDrawId = currentPrizeDraw.Id;
        await prizeUserServiceRepository.Update(currentPrizeDraw);
        await userRepository.Update(user);

        await sender.Clients.All.SendAsync(PrizeDrawSender.DrawHubName, await GetCurrentPrizeDrawDto());
    }





    public async Task CreatePrizeDraw()
    {
        var prizeDraws = await prizeUserServiceRepository.GetAll();

        if (prizeDraws is { Count: > 0 })
        {
            var unfinishedDraws = prizeDraws.Where(e => e.IsFinished == false).ToList();
            if (unfinishedDraws.Count > 0)
            {
                logger.LogInformation("There is already an unfinished prize draw");
                return;
            }
        }

        var newPrizeDraw = new PrizeDraw()
        {
            Id = Guid.NewGuid().ToString(),
            IsFinished = false,
            Participants = new List<User>(),
            PrizeAmount = 120,
            DrawDate = DateTime.UtcNow + TimeSpan.FromHours(24)
        };

        await prizeUserServiceRepository.Add(newPrizeDraw, true);
        await sender.Clients.All.SendAsync(PrizeDrawSender.DrawHubName, await GetCurrentPrizeDrawDto());
    }


}