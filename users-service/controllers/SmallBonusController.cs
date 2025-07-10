using System.Net;
using DataTransferLib.DataTransferObjects.Common;
using Microsoft.AspNetCore.Mvc;
using UsersService.Services;

namespace UsersService.controllers;

[ApiController]
[Route("small-bonus")]
public class SmallBonusController(SmallBonusService smallBonusService, ILogger<SmallBonusController> logger) : ControllerBase
{
    [HttpGet("add")]
    public async Task<IActionResult> AddSmallBonus([FromQuery] string userId)
    {
        if (await smallBonusService.CheckIfUserAlreadyUsedBonusToday(userId))
        {
            logger.LogInformation("User already claimed bonus today");
            return BadRequest(new DefaultResponse<bool>() { Message = "User already claimed bonus today", StatusCode = HttpStatusCode.BadRequest, Result = false });
        }
        var result = await smallBonusService.AddSmallBonus(userId);
        if (result)
        {
            return Ok(new DefaultResponse<bool>() { Message = "Bonus added", StatusCode = HttpStatusCode.OK, Result = true });
        }
        return BadRequest("Failed to add small bonus");
    }
}