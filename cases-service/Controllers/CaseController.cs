using CasesService.Database.Models;
using Microsoft.AspNetCore.Mvc;
using DataTransferLib.CommunicationsServices;
using CasesService.Repositories;
using DataTransferLib.DataTransferObjects.Common.Interfaces;
using DataTransferLib.DataTransferObjects.Common;
using CasesService.Services.Converters;
using CasesService.Controllers.Models;
using CasesService.Services;
using CasesService.Utility;
using DataTransferLib.CacheServices;
using DataTransferLib.DataTransferObjects.CasesItems;
using DtoClassLibrary.DataTransferObjects.Audit;
using DtoClassLibrary.DataTransferObjects.Audit.Dashboard;
using DtoClassLibrary.DataTransferObjects.CasesItems;
using DtoClassLibrary.DataTransferObjects.CasesItems.Models;
using DtoClassLibrary.DataTransferObjects.Common.Logs;
using DtoClassLibrary.DataTransferObjects.Users;
using DtoClassLibrary.DataTransferObjects.Users.Inventory;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using InventoryRecordDto = DtoClassLibrary.DataTransferObjects.CasesItems.InventoryRecordDto;

namespace CasesService.Controllers;

[Route("cases")]
[ApiController]
public class CaseController(
    IRepository<Case> caseRepo,
    IRepository<Item> itemRepo,
    IRepository<ItemCase> itemCaseRepo,
    ILogger<CaseController> logger,
    FinancialCommService financialCommService,
    UsersCommService usersCommService,
    RabbitMqService rabbitMqService,
    CacheService cacheService,
    CaseOpenService caseOpenService,
    AdminLogRabbitMqService adminLogService) : ControllerBase
{
    private readonly CaseRepository _caseRepo = (CaseRepository)caseRepo;
    private readonly ItemRepository _itemRepo = (ItemRepository)itemRepo;
    private readonly ItemCaseRepository _itemCaseRepo = (ItemCaseRepository)itemCaseRepo;
    private readonly FinancialCommService _financialCommService = financialCommService;
    private readonly UsersCommService _usersCommService = usersCommService;
    [HttpGet("")]
    public async Task<IActionResult> CaseGetAll([FromQuery] DefaultRequest defaultRequest)
    {
        List<Case> cases;
        int count;
        try
        {
            RequestService.CheckPaginationParams(ref defaultRequest);
            cases = await _caseRepo.GetOrderBy(defaultRequest);
            count = await _caseRepo.GetCount(defaultRequest);
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }

        return new RequestService().GetResponse(
            "Список кейсов:",
            new CaseToDto().Convert(cases),
            page: defaultRequest.Page,
            count: count
        );
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetCasesCount()
    {
        var count = (await _caseRepo.GetAll()).Count;
        return Ok(count);
    }


    [HttpDelete("deleteall")]
    public async Task<IActionResult> DeleteAllCases()
    {
        var allCases = await _caseRepo.GetAll();
        foreach (var c in allCases)
        {
            await caseRepo.Remove(c);
        }

        return new RequestService().GetResponse("Все кейсы полностью удалены", true);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> CaseGet(string id)
    {
        Case? case_ = await _caseRepo.Get(id);
        if (case_ == null)
            return NotFound("Кейс не найден");

        return new RequestService().GetResponse("Кейс:", new CaseToDto(case_).Convert());
    }

    [HttpPost("add")]
    public async Task<IActionResult> CaseAdd([FromForm] CaseParams? caseParams, IFormFile? imageFile)
    {
        if (caseParams == null)
            return BadRequest("Параметры для добавления не были переданы");
        logger.LogInformation($"CaseAdd called with parameters: caseParams={JsonConvert.SerializeObject(caseParams)}");
        Case case_;
        try
        {
            case_ = await _caseRepo.Add(caseParams, true);

            // 📌 Проверяем, загружен ли файл изображения
            if (imageFile != null)
            {
                if (!(imageFile.ContentType == "image/png" || imageFile.ContentType == "image/jpeg" || imageFile.ContentType == "image/webp"))
                {
                    return BadRequest("Неверный формат изображения. Допустимы только PNG, JPEG и WebP.");
                }

                // 📌 Путь сохранения файла
                var filePath = Path.Combine("/app/resources", "case-images", $"{case_.Id}.webp");
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                // 📌 Конвертация в WebP (Lossless)
                using var image = await Image.LoadAsync<Rgba32>(imageFile.OpenReadStream());
                var encoder = new WebpEncoder
                {
                    Quality = 0, // 0 = Lossless, 100 = максимальное сжатие
                    FileFormat = WebpFileFormatType.Lossless
                };
                await image.SaveAsync(filePath, encoder);
            }
            else
            {
                logger.LogWarning("No image added");
            }

            await rabbitMqService.SendLog($"Добавлен новый кейс №{case_.Id}", case_, LTYPE.Case, objectId: case_.Id, type: 1);
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }

        await cacheService.DropCacheByTypeAsync(DataTransferLib.DataTransferObjects.Common.DefaultRequest.RequestType.Cases);
        return new RequestService().GetResponse("Кейс был успешно создан:", new CaseToDto(case_).Convert());
    }



    [HttpPut("{id}"), HttpPatch("{id}")]
    public async Task<IActionResult> CaseChange(string id, [FromBody] CaseParams? caseParams)
    {
        if (caseParams == null)
            return BadRequest("Параметры для изменения не были переданы");

        logger.LogInformation($"CaseChange called with parameters: id={id}, caseParams={JsonConvert.SerializeObject(caseParams)}");
        Case? case_;
        try
        {
            case_ = await _caseRepo.Change(id, caseParams, true);
            if (case_ == null)
                return NotFound("Кейс не найден");

            await rabbitMqService.SendLog("Updated case id: " + case_.Id, case_, LTYPE.Case,
                objectId: case_.Id, type: 1);
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }
        await cacheService.DropCachedEntity<CaseDto>(id);
        await cacheService.DropCacheByTypeAsync(DataTransferLib.DataTransferObjects.Common.DefaultRequest.RequestType
            .Cases);
        return new RequestService().GetResponse("Case updated", new CaseToDto(case_).Convert());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CaseDelete(string id)
    {
        Case? case_;
        try
        {
            case_ = await _caseRepo.Remove(id, true);
            if (case_ == null)
                return NotFound("Кейс не найден");


            await rabbitMqService.SendLog("Удалён кейс №" + case_.Id, case_, LTYPE.Case, objectId: case_.Id, type: 1);
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }
        await cacheService.DropCachedEntity<CaseDto>(id);
        await cacheService.DropCacheByTypeAsync(DataTransferLib.DataTransferObjects.Common.DefaultRequest.RequestType
            .Cases);
        return new RequestService().GetResponse("Кейс был успешно удалён:", new CaseToDto(case_).Convert());
    }




    [HttpPost("additem")]
    public async Task<IActionResult> ItemCaseAdd([FromBody] ItemCaseParams itemCaseParams)
    {
        logger.LogInformation(
            $"ItemCaseAdd called with parameters: CaseId={itemCaseParams.CaseId}, ItemId={itemCaseParams.ItemId}");

        if (itemCaseParams.ItemId == null || itemCaseParams.CaseId == null)
            return BadRequest("Параметры для изменения не были переданы");

        Case? case_ = await _caseRepo.Get(itemCaseParams.CaseId);
        if (case_ == null)
        {
            logger.LogWarning($"Case with ID {itemCaseParams.CaseId} not found");
            return NotFound("Кейс не найден");
        }

        logger.LogInformation($"Fetching item with ID: {itemCaseParams.ItemId}");
        Item? item = await _itemRepo.Get(itemCaseParams.ItemId);
        if (item == null)
        {
            logger.LogWarning($"Item with ID {itemCaseParams.ItemId} not found");
            return NotFound("Предмет не найден");
        }

        ItemCase itemCase;
        try
        {
            logger.LogInformation(
                $"Attempting to add item to case: CaseId={itemCaseParams.CaseId}, ItemId={itemCaseParams.ItemId}");
            itemCase = await _itemCaseRepo.Add(itemCaseParams, true);
            if (itemCase == null)
                return RequestService.InternalServerError("Ошибка получения результата");

            logger.LogInformation(
                $"Successfully added item to case. CaseId={itemCaseParams.CaseId}, ItemId={itemCaseParams.ItemId}");
            await rabbitMqService.SendLog("Добавлен новый предмет кейса №" + case_.Id, case_, LTYPE.Case,
                objectId: case_.Id, type: 1);
        }
        catch (Exception e)
        {
            logger.LogError(e,
                $"An error occurred while adding item to case. CaseId={itemCaseParams.CaseId}, ItemId={itemCaseParams.ItemId}");
            return new RequestService().HandleError(e);
        }
        await cacheService.DropCachedEntity<CaseDto>(itemCaseParams.CaseId);
        await cacheService.DropCacheByTypeAsync(DataTransferLib.DataTransferObjects.Common.DefaultRequest.RequestType
            .Cases);
        return new RequestService().GetResponse("Предмет кейса был успешно создан:",
            new ItemCaseToDto(itemCase).Convert());
    }









    [HttpPost("removeitem")]
    public async Task<IActionResult> ItemCaseDelete([FromBody] ItemCaseParams itemCaseParams)
    {
        if (itemCaseParams.ItemId == null || itemCaseParams.CaseId == null)
            return BadRequest("Идентификатор предмета или кейса не был передан");

        ItemCase? itemCase;
        try
        {
            itemCase = await _itemCaseRepo.Remove(itemCaseParams.CaseId, itemCaseParams.ItemId, true);
            if (itemCase == null)
                return NotFound("Предмет кейса не найден");

            await rabbitMqService.SendLog("Удалён предмет кейса №" + itemCase.CaseId, itemCase, LTYPE.Case);
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }
        await cacheService.DropCachedEntity<CaseDto>(itemCaseParams.CaseId);
        await cacheService.DropCacheByTypeAsync(DataTransferLib.DataTransferObjects.Common.DefaultRequest.RequestType
            .Cases);
        return new RequestService().GetResponse("Предмет кейса был успешно удалён:",
            new ItemCaseToDto(itemCase).Convert());
    }


    [HttpPut("setalpha")]
    public async Task<IActionResult> SetAlpha([FromBody] AlphaSetRequest setAlphaRequest)
    {
        if (setAlphaRequest.CaseId == null || setAlphaRequest.Alpha == null)
            return BadRequest("Идентификатор кейса или альфа не был передан");
        Case? case_;
        try
        {
            case_ = await _caseRepo.SetAlpha(setAlphaRequest.CaseId, setAlphaRequest.Alpha);
            if (case_ == null)
                return NotFound("Кейс не найден");
            await rabbitMqService.SendLog("Изменена альфа кейса №" + case_.Id, case_, LTYPE.Case,
                objectId: case_.Id, type: 1);
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }
        await cacheService.DropCachedEntity<CaseDto>(setAlphaRequest.CaseId);
        await cacheService.DropCacheByTypeAsync(DataTransferLib.DataTransferObjects.Common.DefaultRequest.RequestType
            .Cases);
        return new RequestService().GetResponse("Альфа была успешно изменена:", new CaseToDto(case_).Convert());
    }


    [HttpPost("case/open")]
    public async Task<IActionResult> OpenCase([FromBody] OpenCaseRequest openCaseRequest)
    {
        var caseFromBase = await _caseRepo.Get(openCaseRequest.CaseId);
        if (caseFromBase == null)
        {
            return NotFound("No such case");
        }

        if (caseFromBase?.CurrentOpen >= caseFromBase?.OpenLimit)
        {
            return BadRequest("Case open limit reached");
        }

        var itemsIds = caseFromBase.ItemsCases.Select(e => e.Item).ToList();

        var droppedItem = await caseOpenService.OpenCase(openCaseRequest);

        caseFromBase.CurrentOpen += 1;

        var result = await _usersCommService.AddItemToInventory(new AddRemoveItemRequest()
        {
            ItemInventoryRecordId = droppedItem.Id,
            Quantity = 1,
            UserId = openCaseRequest.UserId,
            ItemRecordState = ItemRecordState.FromCase,
            ItemImageUrl = droppedItem.Image
        });



        if (result?.Result is not null)
        {
            var openedCaseDto = new DroppedItemDto()
            {
                CaseId = caseFromBase.Id,
                UserId = openCaseRequest.UserId,
                Item = droppedItem.CreateDto(),
                OpenedTimeStamp = DateTime.UtcNow,
                SellPrice = droppedItem.SellPrice
            };
            await rabbitMqService.SendLog("User №" + openCaseRequest.UserId + " opened case", openedCaseDto,
                LTYPE.Case, objectId: openCaseRequest.CaseId, type: 1);
            caseFromBase.AddAccumulatedProfit();
            await _caseRepo.Save();
            await cacheService.DropCachedEntity<CaseDto>(openCaseRequest.CaseId);
            await cacheService.DropCacheByTypeAsync(DataTransferLib.DataTransferObjects.Common.DefaultRequest
                .RequestType
                .Cases);

            var openResult = new GameResultDto()
            {
                Items = new List<InventoryRecordDto>(),
                UserId = openCaseRequest.UserId,
            };
            openResult.Items.AddRange(result.Result.Items);

            
            return new RequestService().GetResponse("Кейс успешно открыт!", openResult);
        }
        else
        {
            return new RequestService().GetResponse("Произошла ошибка при попытке открытия кейса", new GameResultDto());
        }
    }


    [HttpPost("cases/open")]
    public async Task<IActionResult> OpenMultipleCases([FromBody] OpenCaseRequest request)
    {

        var caseFromBase = await _caseRepo.Get(request.CaseId);

        if (caseFromBase is null)
        {
            return NotFound("No such case");
        }

        bool caseExceeded = caseFromBase.CurrentOpen + request.Quantity > caseFromBase.OpenLimit;
        if (caseFromBase.CurrentOpen >= caseFromBase.OpenLimit || caseExceeded)
        {
            return BadRequest("Case open limit reached");
        }

        var itemsIds = caseFromBase.ItemsCases
            .Where(e => e.Item != null)
            .Select(e => e.Item)
            .ToList();
        logger.LogInformation("Available items to drop: {Count}", itemsIds.Count);

        var items = new List<string>();
        List<Item> dropedItems = new List<Item>();


        for (int i = 0; i < request.Quantity; i++)
        {
            logger.LogInformation("Iteration {i}, items count: {items}", i, items.Count);
            var droppedItem = await caseOpenService.OpenCase(request);
            if (droppedItem == null)
            {
                logger.LogWarning("Dropped item is null on iteration {i}", i);
                continue; // или throw
            }
            logger.LogInformation("Dropping item id = {ItemId}, name = {Name}", droppedItem.Id, droppedItem.Name);

            dropedItems.Add(droppedItem);

            items.Add(droppedItem.Id);

        }
        if (dropedItems.Count != request.Quantity)
        {
            logger.LogError("!! CRITICAL: drop count mismatch. Request = {r}, Actual = {a}", request.Quantity, dropedItems.Count);
            foreach (var item in dropedItems)
            {
                logger.LogError("Dropped item: {id}", item?.Id);
            }
        }

        caseFromBase.CurrentOpen += request.Quantity;

        var result = await _usersCommService.AddMultipleItemsToInventory(new AddRemoveMultipleItemsRequest()
        {
            Items = items,
            UserId = request.UserId
        });
        logger.LogInformation("Items dict: {@items}", items);
        logger.LogInformation("Dropped item count: {Count}", dropedItems.Count);
        logger.LogWarning("Requested {q}, dropped {d}, inventory added {i}", request.Quantity, dropedItems.Count, items.Count);


        if (result?.Result?.Items.Count != 0)
        {
            var itemsForRabbit = new List<DroppedItemDto>();
            foreach (var droppedItem in dropedItems)
            {
                itemsForRabbit.Add(new DroppedItemDto()
                {
                    UserId = request.UserId,
                    CaseId = request.CaseId,
                    Item = droppedItem.CreateDto(),
                    OpenedTimeStamp = DateTime.UtcNow,
                    SellPrice = droppedItem.SellPrice
                });
                caseFromBase.AddAccumulatedProfit();
            }
            if (dropedItems.Count != request.Quantity)
            {
                logger.LogError("Mismatch in drop count! Requested: {Requested}, Got: {Actual}", request.Quantity, dropedItems.Count);
            }

            await _caseRepo.Save();
            var itemsList = items;
            await rabbitMqService.SendLog(
                "User №" + request.UserId + " opened multiple cases " + request.Quantity + " times", itemsForRabbit,
                LTYPE.Case, objectId: request.CaseId, type: 1);
            await cacheService.DropCacheByTypeAsync(DataTransferLib.DataTransferObjects.Common.DefaultRequest
                .RequestType
                .Cases);
            logger.LogInformation("Case opened successfully");

            var openResult = new GameResultDto()
            {
                Items = new List<InventoryRecordDto>(),
                UserId = request.UserId,
            };
            openResult.Items.AddRange(result.Result.Items);


            return new RequestService().GetResponse("Кейс успешно открыт!", openResult);
        }
        else
        {
            return new RequestService().GetResponse("Произошла ошибка при попытке открытия кейса", new GameResultDto());
        }
    }
}