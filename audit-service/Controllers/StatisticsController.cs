using AuditService.Database;
using AuditService.Services;
using DataTransferLib.CommunicationsServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuditService.Controllers;

[ApiController]
[Route("/audit")]
public class StatisticsController(OpenedCasesService openedCasesService) : ControllerBase
{
    [HttpGet("favcase/{userId}")]
    public async Task<IActionResult> GetFavouriteCaseForUser(string userId)
    {
        var favCase = await openedCasesService.GetFavouriteCaseForUser(userId);

        return favCase != null ? new RequestService().GetResponse("Favourite Case", favCase) : NotFound();
    }

    [HttpGet("max_cost/item/{userId}")]
    public async Task<IActionResult> GetMaxItemCostForUser(string userId)
    {
        var maxCostItem = await openedCasesService.GetMaxCostItem(userId);

        return new RequestService().GetResponse("Max Item", maxCostItem);
    }

}