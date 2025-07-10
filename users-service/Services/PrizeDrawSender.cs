using Microsoft.AspNetCore.SignalR;

namespace UsersService.Services;

public class PrizeDrawSender(PrizeDrawService prizeDrawService) : Hub
{
    public const string DrawHubName = "prize_draw";
    public async Task SendPrizeDrawUpdate()
    {
        var currentDraw = await prizeDrawService.GetCurrentPrizeDrawDto();
        if (currentDraw is null)
        {
            return;
        }
        await Clients.All.SendAsync(DrawHubName, currentDraw);
    }

    public override async Task OnConnectedAsync()
    {

        var currentDraw = await prizeDrawService.GetCurrentPrizeDrawDto();

        await Clients.All.SendAsync(DrawHubName, currentDraw);
        await base.OnConnectedAsync();
    }
}