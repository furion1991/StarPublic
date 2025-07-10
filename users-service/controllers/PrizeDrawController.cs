using Microsoft.AspNetCore.Mvc;
using UsersService.Services;

namespace UsersService.controllers;


[ApiController]
[Route("prize-draw")]
public class PrizeDrawController(PrizeDrawService prizeDrawService, ILogger<PrizeDrawController> logger) : ControllerBase
{

    [HttpGet("is-subscribed")]
    public async Task<IActionResult> IsUserSubscribed([FromQuery] string userId)
    {
        var result = await prizeDrawService.IsUserParticipating(userId);
        return Ok(result);
    }

    [HttpGet("subscribe")]
    public async Task<IActionResult> SubscribeToCurrentPrizeDraw([FromQuery] string userId)
    {
        await prizeDrawService.SubscribeToCurrentPrizeDraw(userId);
        return Ok();
    }
}