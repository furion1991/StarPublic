using System.Net;
using CasesService.Database.Models;
using Microsoft.AspNetCore.Mvc;
using DataTransferLib.CommunicationsServices;
using DataTransferLib.DataTransferObjects.CasesItems;
using DataTransferLib.DataTransferObjects.CasesItems.Models;
using DataTransferLib.DataTransferObjects.Common;
using DataTransferLib.DataTransferObjects.Common.Interfaces;
using CasesService.Repositories;
using CasesService.Services.Converters;
using DtoClassLibrary.DataTransferObjects.Bonus;
using DtoClassLibrary.DataTransferObjects.CasesItems;
using DtoClassLibrary.DataTransferObjects.CasesItems.Models;
using CasesService.Services;
using DtoClassLibrary.DataTransferObjects.CasesItems.Upgrade;
using DtoClassLibrary.DataTransferObjects.Users.Inventory;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;

namespace CasesService.Controllers;

[Route("items")]
[ApiController]
public class ItemController(
    IRepository<Item> itemRepo,
    ILogger<ItemController> logger,
    RabbitMqService rabbitMqService,
    UsersCommService usersCommService,
    UpgradeService upgradeService) : Controller
{
    private readonly ItemRepository _itemRepo = (ItemRepository)itemRepo;


    [HttpPost("upgrade/preview")]
    public async Task<IActionResult> GetUpgradePreview([FromBody] UpgradePreviewRequest request)
    {
        var attemptedItem = await itemRepo.Get(request.AttemptedItemId);
        var userItem = await itemRepo.Get(request.UserItemId);

        if (attemptedItem == null || userItem == null)
        {
            return BadRequest("Items not found");
        }

        if (userItem.SellPrice == null || attemptedItem.SellPrice == null || userItem.SellPrice <= 0 ||
            attemptedItem.SellPrice <= 0)
        {
            return BadRequest("Invalid item prices");
        }

        decimal userPrice = userItem.SellPrice.Value;
        decimal attemptedPrice = attemptedItem.SellPrice.Value;

        decimal chance = UpgradeChanceCalculator.CalculateUpgradeChance(userPrice, attemptedPrice);

        var response = new UpgradePreviewResponse
        {
            Chance = Math.Round(chance * 100, 2), // в процентах, например 76.32%
            Coefficient = UpgradeChanceCalculator.CalculateCoefficient(userPrice, attemptedPrice)
        };

        return new RequestService().GetResponse("Preview result", response);
    }


    [HttpPost("upgrade")]
    public async Task<IActionResult> UpgradeItem([FromBody] UpgradeItemRequest request)
    {
        var result = await upgradeService.UpgradeItem(request);


        if (result == null)
        {
            return BadRequest(new DefaultResponse<GameResultDto>()
            { Message = "something went wrong", StatusCode = HttpStatusCode.BadRequest, Result = new GameResultDto() });
        }

        return new RequestService().GetResponse("Upgrade result", result);
    }

    [HttpGet("")]
    public async Task<IActionResult> ItemGetAll([FromQuery] DefaultRequest defaultRequest)
    {
        logger.LogInformation($"Request:  {JsonConvert.SerializeObject(defaultRequest)}\n\n\n\n\n\n\n\n\n");
        List<Item> items;
        int count;
        try
        {
            RequestService.CheckPaginationParams(ref defaultRequest);

            count = await _itemRepo.GetCount(defaultRequest);
            items = await _itemRepo.GetOrderBy(defaultRequest);
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }

        return new RequestService().GetResponse(
            "Список предметов:",
            new ItemToDto().Convert(items),
            page: defaultRequest.Page,
            count: count
        );
    }


    [HttpPost("apply/bonus_random_items/{userId}")]
    public async Task<IActionResult> ApplyRandomItemsBonus([FromBody] ItemBonusDto bonus, string userId)
    {
        var items = await _itemRepo.GetListOfCost(200);
        var request = new AddRemoveMultipleItemsRequest()
        {
            Items = new List<string>(),
            UserId = userId
        };
        var random = new Random();
        for (int i = 0; i < bonus.ItemCount; i++)
        {
            request.Items.Add(items[random.Next(0, items.Count)].Id);
        }

        var result = await usersCommService.AddMultipleItemsToInventory(request);

        if (result != null)
        {
            return Ok();
        }

        return StatusCode((int)result.StatusCode, result.Message);
    }


    [HttpPost("apply/bonus_item/{userId}")]
    public async Task<IActionResult> ApplyItemBonus([FromBody] ItemBonusDto bonus, string userId)
    {
        var item = await ((ItemRepository)itemRepo).GetFromBonusParams(bonus);
        var addedItem = item[new Random().Next(0, item.Count)];

        var result = await usersCommService.AddItemToInventory(new AddRemoveItemRequest()
        {
            ItemRecordState = ItemRecordState.FromCase,
            Quantity = 1,
            UserId = userId,
            ItemInventoryRecordId = addedItem.Id
        });

        if (result != null)
        {
            return Ok();
        }

        return StatusCode((int)result.StatusCode, result.Message);
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetItemsList([FromBody] List<string> itemsIdsList)
    {
        var listToSend = new List<ItemDto>();

        var converter = new ItemToDto();

        foreach (var item in itemsIdsList)
        {
            var itemFromDb = await _itemRepo.Get(item);
            if (itemFromDb == null)
            {
                continue;
            }

            listToSend.Add(converter.Convert(itemFromDb));
        }

        return new RequestService().GetResponse("Items list", listToSend);
    }


    [HttpDelete("deleteall")]
    public async Task<IActionResult> DeleteAllItems()
    {
        var items = await _itemRepo.GetAll();
        foreach (var item in items)
        {
            await _itemRepo.Remove(item);
        }

        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ItemGet(string id)
    {
        try
        {
            logger.LogInformation($"Getting item {id}");
            Item? item = await _itemRepo.Get(id);
            if (item == null)
                return NotFound("Предмет не найден");

            return new RequestService().GetResponse("Данные предмета:", new ItemToDto(item).Convert());
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return new RequestService().HandleError(e);
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> ItemAdd([FromForm] ItemParams? itemParams, IFormFile? imageFile)
    {
        if (itemParams == null)
            return BadRequest("Параметры для добавления не были переданы");

        logger.LogInformation($"Request:  {JsonConvert.SerializeObject(itemParams)}\n\n\n\n\n\n\n\n");
        Item item;
        try
        {
            item = await _itemRepo.Add(itemParams, true);

            if (imageFile != null)
            {
                logger.LogInformation("adding file");
                if (!(imageFile.ContentType == "image/png" || imageFile.ContentType == "image/jpeg" ||
                      imageFile.ContentType == "image/webp"))
                {
                    return BadRequest("Неверный формат изображения. Допустимы только PNG, JPEG и WebP.");
                }
            }

            var filePath = Path.Combine("/app/resources", "item-images", $"{item.Id}.webp");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            using var image = await Image.LoadAsync<Rgba32>(imageFile.OpenReadStream());
            var encoder = new WebpEncoder
            {
                Quality = 0,
                FileFormat = WebpFileFormatType.Lossless
            };
            await image.SaveAsync(filePath, encoder);
            await rabbitMqService.SendLog("Добавлен новый предмет №" + item.Id, item, LTYPE.Item);
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }

        return new RequestService().GetResponse("Предмет был успешно создан:", new ItemToDto(item).Convert());
    }

    [HttpPut("{id}"), HttpPatch("{id}")]
    public async Task<IActionResult> ItemChange(string id, [FromBody] ItemParams? itemParams)
    {
        if (itemParams == null)
            return BadRequest("Параметры для изменения не были переданы");

        Item? item;
        try
        {
            item = await _itemRepo.Change(id, itemParams, true);
            if (item == null)
                return NotFound("Предмет не найден");

            await rabbitMqService.SendLog("Изменён существующий новый предмет №" + item.Id, item, LTYPE.Item);
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }

        return new RequestService().GetResponse("Предмет был успешно изменён:", new ItemToDto(item).Convert());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> ItemDelete(string id)
    {
        Item? item;
        try
        {
            item = await _itemRepo.Remove(id, true);
            if (item == null)
                return NotFound("Предмет не найден");

            await rabbitMqService.SendLog("Удалён предмет №" + item.Id, item, LTYPE.Item);
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }

        return new RequestService().GetResponse("Предмет был успешно удалён:", new ItemToDto(item).Convert());
    }
}