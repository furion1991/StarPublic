using System.Net;
using CasesService.Database.Models;
using CasesService.Services;
using DataTransferLib.CommunicationsServices;
using DataTransferLib.DataTransferObjects.Common;
using DtoClassLibrary.DataTransferObjects.CasesItems;
using DtoClassLibrary.DataTransferObjects.CasesItems.Contracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CasesService.Controllers;


[ApiController]
[Route("contracts")]
public class ContractsController(ContractService contractService, ILogger<ContractsController> logger) : ControllerBase
{

    [HttpPost("preview")]
    public async Task<IActionResult> ContractPreview([FromBody] ContractPreviewRequest request)
    {
        logger.LogInformation($"Request:  {JsonConvert.SerializeObject(request)}\n\n\n\n\n\n\n\n\n");

        try
        {
            var result = await contractService.GetPreview(request);
            if (result == null)
            {
                return BadRequest("Invalid request");
            }

            return new RequestService().GetResponse("ContractPreview", result);
        }
        catch (Exception e)
        {

            return new RequestService().HandleError(e);
        }


    }

    [HttpPost("execute")]
    public async Task<IActionResult> ExecuteContract([FromBody] ContractExecuteRequest request)
    {
        var itemsAvailable = await contractService.IsItemsAvailableForContract(request);

        if (!itemsAvailable)
        {
            return BadRequest(new DefaultResponse<GameResultDto>()
            {
                Message = "One or more items is not valid",
                StatusCode = HttpStatusCode.BadRequest,
                Result = new GameResultDto()
            });
        }

        var item = await contractService.ExecuteContract(request);
        logger.LogInformation(JsonConvert.SerializeObject(item));
        return new RequestService().GetResponse("ContractExecute", item);

    }
}

