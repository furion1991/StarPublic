using AuditService.Database.Models;
using Microsoft.AspNetCore.Mvc;
using DataTransferLib.DataTransferObjects.Common;
using DataTransferLib.CommunicationsServices;
using DataTransferLib.DataTransferObjects.Audit;
using System.Collections;

namespace AuditService.Controllers;

[Route("logs")]
[ApiController]
public class LogController(
    Services.LogService logService,
    ILogger<LogController> logger) : Controller
{
    private readonly ILogger<LogController> _logger = logger;
    private readonly Services.LogService _logService = logService;
    
    
    [HttpGet("")]
    public async Task<IActionResult> LogsGetAll([FromQuery] LogDefaultRequest defaultRequest)
    {
        int? count;
        IList? logs;
        string? message;
        try
        {
            DefaultRequest standardDefaultRequest = defaultRequest;
            RequestService.CheckPaginationParams(ref standardDefaultRequest);

            logs = await _logService.GetAllLogs(defaultRequest);
            count = await _logService.GetAllCount(defaultRequest);
            message = _logService.GetAllMessage(defaultRequest);

            if (logs == null || count == null || message == null)
                return BadRequest("Указаны некорректные данные для обработки");
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }

        return new RequestService().GetResponse(message, logs, page: defaultRequest.Page, count: count);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> LogsGet(string id)
    {
        BaseLog? baseLog = await _logService.Get(id);
        if (baseLog == null)
            return NotFound("Лог не найден");

        return new RequestService().GetResponse("Данные лога:", baseLog);
    }

    [HttpPost("")]
    public async Task<IActionResult> LogAdd([FromBody] Log? logParams)
    {
        BaseLog? baseLog;
        try
        {
            baseLog = await _logService.Add(logParams);
            if (baseLog == null)
                return BadRequest(
                    "Параметры для добавления не были переданы или количество идентификаторов для добавления неверно");
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }

        return new RequestService().GetResponse("Лог был успешно создан:", baseLog);
    }

    [HttpPut("{id}"), HttpPatch("{id}")]
    public async Task<IActionResult> LogChange(string id, [FromBody] Log? logParams)
    {
        BaseLog? baseLog;
        try
        {
            baseLog = await _logService.Change(id, logParams);
            if (baseLog == null)
                return BadRequest("Параметры для изменения не были переданы или лог не найден");
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }

        return new RequestService().GetResponse("Лог был успешно изменён:", baseLog);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> LogDelete(string id)
    {
        BaseLog? baseLog;
        try
        {
            baseLog = await _logService.Remove(id);
            if (baseLog == null)
                return BadRequest("Лог не найден");
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }

        return new RequestService().GetResponse("Лог был успешно удалён:", baseLog);
    }

    [HttpGet("history/get")]
    public async Task<IActionResult> GetHistory()
    {
        List<CaseLog>? logs;
        try
        {
            logs = await _logService.GetHistory();
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }

        return new RequestService().GetResponse("История открытия кейсов:", logs);
    }
}