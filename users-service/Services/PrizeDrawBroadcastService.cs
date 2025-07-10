using DtoClassLibrary.DataTransferObjects.Bonus.SmallBonuses;
using Microsoft.AspNetCore.SignalR;

namespace UsersService.Services;

public class PrizeDrawBroadcastService(IHubContext<PrizeDrawSender> _hubContext, PrizeDrawService prizeDrawService) : BackgroundService
{


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return;
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                PrizeDrawDto? currentDraw = null;

                try
                {
                    currentDraw = await prizeDrawService.GetCurrentPrizeDrawDto();
                }
                catch (Exception ex)
                {
                    // не падаем если GetCurrentPrizeDrawDto сломался
                    Console.WriteLine("Failed to get current draw: " + ex.Message);
                }

                if (currentDraw is null)
                {
                    await Task.Delay(1000, stoppingToken);
                    await prizeDrawService.CreatePrizeDraw();
                    continue;
                }

                if (currentDraw.SecondsRemainingTillDraw <= 0)
                {
                    try
                    {
                        await prizeDrawService.FinishCurrentPriceDraw();
                        await prizeDrawService.CreatePrizeDraw();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error finishing draw: " + ex.Message);
                    }

                    continue;
                }

                await _hubContext.Clients.All.SendAsync(PrizeDrawSender.DrawHubName, currentDraw, cancellationToken: stoppingToken);
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("BroadcastService crashed: " + ex.Message);
        }
    }

}