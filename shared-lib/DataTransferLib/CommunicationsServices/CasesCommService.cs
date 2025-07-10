using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using DataTransferLib.CacheServices;
using DataTransferLib.DataTransferObjects.CasesItems;
using DataTransferLib.DataTransferObjects.CasesItems.Models;
using DataTransferLib.DataTransferObjects.Common;
using DtoClassLibrary.DataTransferObjects.Audit;
using DtoClassLibrary.DataTransferObjects.Audit.Dashboard;
using DtoClassLibrary.DataTransferObjects.Bonus;
using DtoClassLibrary.DataTransferObjects.CasesItems;
using DtoClassLibrary.DataTransferObjects.CasesItems.Contracts;
using DtoClassLibrary.DataTransferObjects.CasesItems.Models;
using DtoClassLibrary.DataTransferObjects.CasesItems.Upgrade;
using DtoClassLibrary.DataTransferObjects.Common;
using DtoClassLibrary.DataTransferObjects.Users;
using DtoClassLibrary.DataTransferObjects.Users.Inventory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DataTransferLib.CommunicationsServices;

public class CasesCommService(IHttpClientFactory factory,
    ILogger<CasesCommService> logger,
    CasesCache casesCache,
    ItemsCache itemsCache,
    RabbitMqCacheService cacheService)
{
    private readonly HttpClient _client = factory.CreateClient(CommConfigure.CASES_CLIENT_NAME);

    public async Task<IResponse<bool>?> DeleteAllCasesAsync()
    {
        var response = await _client.DeleteAsync("cases/deleteall");
        return await response.ReadResponse<bool>();
    }


    public async Task<bool> SetAlphaForCase(AlphaSetRequest request)
    {

        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync("cases/setalpha", content);
        if (response.IsSuccessStatusCode)
        {
            await cacheService.SendCasesItemsReloadMessage();
        }
        return response.IsSuccessStatusCode;
    }

    public async Task<IResponse<MultiplierDto>?> GetMultipliers()
    {
        var response = await _client.GetAsync("cases/getmultipliers");
        return await response.ReadResponse<MultiplierDto>();
    }

    public async Task<bool> SetMultipliers(MultiplierDto request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("cases/setmultipliers", content);
        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        return false;
    }

    public async Task<IResponse<List<ItemDto>>?> GetItemsList(List<string> itemsList)
    {

        var existingItemList = await itemsCache.GetItemsFromCache(itemsList);
        if (existingItemList is { Count: > 0 })
        {
            return new DefaultResponse<List<ItemDto>>()
            {
                Message = "Items retrieved from cache.",
                Result = existingItemList,
                StatusCode = HttpStatusCode.OK
            };
        }
        var content = new StringContent(JsonSerializer.Serialize(itemsList), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("items/list", content);

        if (!response.IsSuccessStatusCode)
            return new ErrorResponse<List<ItemDto>>()
            {
                Message = response.ReasonPhrase,
                Result = null,
                ErrorDetails = response.ReasonPhrase,
                StatusCode = response.StatusCode
            };
        var result = await response.ReadResponse<List<ItemDto>>();
        await itemsCache.SetItemsToCache(result.Result);
        return result;

    }

    public async Task<int> GetAllCasesCount()
    {
        var response = await _client.GetAsync("cases/count");
        var count = await response.Content.ReadFromJsonAsync<int>();
        return count;
    }


    public async Task<HttpStatusCode> ApplyItemBonus(ItemBonusDto bonus, string userId)
    {
        var content = new StringContent(JsonConvert.SerializeObject(bonus), Encoding.UTF8, "application/json");


        var response = await _client.PostAsync($"items/apply/bonus_item/{userId}", content);

        return response.StatusCode;
    }


    public async Task<int> GetCasesCount()
    {
        var response = await _client.GetAsync($"cases/count");
        var result = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());

        return result;
    }


    public async Task<IResponse<List<CaseDto>>?> GetAllCases(DefaultRequest defaultRequest)
    {
        var cachedCases = await casesCache.GetAllCasesFromCache(query: defaultRequest.GetQueryParams());

        if (cachedCases is { Count: > 0 })
        {
            return new DefaultResponse<List<CaseDto>>()
            {
                Message = "Cases retrieved from cache.",
                Result = cachedCases,
                StatusCode = HttpStatusCode.OK
            };
        }
        var response = await _client.GetAsync($"cases{defaultRequest.GetQueryParams()}");

        if (response.IsSuccessStatusCode)
        {
            var result = await response.ReadResponse<List<CaseDto>>();
            if (result is not null)
            {
                await casesCache.SetCasesToCache(result.Result, defaultRequest.GetQueryParams());
                return result;
            }
        }

        return new ErrorResponse<List<CaseDto>>()
        {
            Message = "Failed to retrieve cases.",
            ErrorDetails = response.ReasonPhrase,
            Result = null,
            StatusCode = response.StatusCode
        };

    }

    public async Task<IResponse<bool>> TransferItemsFromOneCaseToAnother(string fromId, string toId)
    {
        var response = await _client.PostAsync($"cases/transfer?from={fromId}&=to{toId}", null);
        if (response.IsSuccessStatusCode)
        {
            await cacheService.SendCasesItemsReloadMessage();
            return new DefaultResponse<bool>()
            {
                Result = true,
                StatusCode = HttpStatusCode.OK,
                Message = "Items transferred successfully."
            };
        }
        return new ErrorResponse<bool>()
        {
            Result = false,
            StatusCode = response.StatusCode,
            Message = response.ReasonPhrase
        };
    }

    public async Task<IResponse<CaseDto>?> GetCase(string id)
    {
        var cachedCase = await casesCache.GetCaseFromCache(id);
        if (cachedCase is not null)
        {
            return new DefaultResponse<CaseDto>()
            {
                Message = "Case retrieved from cache.",
                Result = cachedCase,
                StatusCode = HttpStatusCode.OK
            };
        }

        var response = await _client.GetAsync($"cases/{id}");

        if (response.IsSuccessStatusCode)
        {
            var result = await response.ReadResponse<CaseDto>();
            await casesCache.SetCaseToCache(result.Result);
            return result;
        }

        return new ErrorResponse<CaseDto>()
        {
            Message = response.ReasonPhrase,
            ErrorDetails = response.ReasonPhrase,
            Result = null,
            StatusCode = response.StatusCode
        };
    }

    public async Task<IResponse<CaseDto>?> AddNewCase(CaseParams caseParams, StreamContent? imageFile = null)
    {
        using var form = new MultipartFormDataContent();

        if (!string.IsNullOrEmpty(caseParams.Name))
            form.Add(new StringContent(caseParams.Name, Encoding.UTF8), "Name");

        form.Add(new StringContent(caseParams.Type.ToString()), "Type");

        if (caseParams.Price.HasValue)
            form.Add(new StringContent(caseParams.Price.Value.ToString(), Encoding.UTF8), "Price");

        if (caseParams.OpenLimit.HasValue)
            form.Add(new StringContent(caseParams.OpenLimit.Value.ToString(), Encoding.UTF8), "OpenLimit");

        if (caseParams.Discount.HasValue)
            form.Add(new StringContent(caseParams.Discount.Value.ToString(), Encoding.UTF8), "Discount");

        if (caseParams.OldPrice.HasValue)
            form.Add(new StringContent(caseParams.OldPrice.Value.ToString(), Encoding.UTF8), "OldPrice");

        if (!string.IsNullOrEmpty(caseParams.CaseCategory))
            form.Add(new StringContent(caseParams.CaseCategory, Encoding.UTF8), "CaseCategory");


        if (imageFile != null)
        {
            logger.LogInformation("📂 Добавляем файл в запрос...");
            form.Add(imageFile, "imageFile", "case-image.png");
        }
        else
        {
            logger.LogWarning("⚠️ Файл отсутствует в запросе.");
        }

        var response = await _client.PostAsync("cases/add", form);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError(
                $"❌ Ошибка при загрузке: {response.StatusCode} {await response.Content.ReadAsStringAsync()}");
            return null;
        }

        await cacheService.SendCasesItemsReloadMessage();
        return await response.ReadResponse<CaseDto>();
    }


    public async Task<IResponse<bool>> CreateCategory(string name, IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            return new ErrorResponse<bool>
            {
                Result = false,
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Image file is required."
            };
        }
        var form = new MultipartFormDataContent
        {
            { new StringContent(name), "name" },
            { new StreamContent(image.OpenReadStream()), "imageFile", image.FileName }
        };
        var response = await _client.PostAsync($"cases/category/{name}", form);

        return response.IsSuccessStatusCode
            ? new DefaultResponse<bool> { Result = true, StatusCode = HttpStatusCode.OK, Message = "Category created successfully." }
            : new ErrorResponse<bool> { Result = false, StatusCode = response.StatusCode, Message = response.ReasonPhrase };
    }

    public async Task<IResponse<List<CaseCategoryDto>>> GetAllCategories()
    {
        var response = await _client.GetAsync("cases/categories");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.ReadResponse<List<CaseCategoryDto>>();
            return result ?? new ErrorResponse<List<CaseCategoryDto>>()
            {
                Message = "Failed to retrieve categories.",
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
        return new ErrorResponse<List<CaseCategoryDto>>()
        {
            Message = response.ReasonPhrase,
            StatusCode = response.StatusCode
        };
    }

    public async Task<IResponse<CaseCategoryDto>> GetCategoryByName(string name)
    {
        var response = await _client.GetAsync($"cases/category-name/{name}");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.ReadResponse<CaseCategoryDto>();
            return result ?? new ErrorResponse<CaseCategoryDto>()
            {
                Message = "Failed to retrieve category.",
                StatusCode = HttpStatusCode.InternalServerError
            };
        }

        return new ErrorResponse<CaseCategoryDto>()
        {
            Message = response.ReasonPhrase,
            Result = null,
            ErrorDetails = response.ReasonPhrase,
            StatusCode = response.StatusCode
        };
    }
    public async Task<IResponse<CaseCategoryDto>> GetCategoryById(string id)
    {
        var response = await _client.GetAsync($"cases/category-id/{id}");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.ReadResponse<CaseCategoryDto>();
            return result ?? new ErrorResponse<CaseCategoryDto>()
            {
                Message = "Failed to retrieve category.",
                StatusCode = HttpStatusCode.InternalServerError
            };
        }

        return new ErrorResponse<CaseCategoryDto>()
        {
            Message = response.ReasonPhrase,
            Result = null,
            ErrorDetails = response.ReasonPhrase,
            StatusCode = response.StatusCode
        };
    }

    public async Task<IResponse<bool>> DeleteCategory(string id)
    {
        var response = await _client.DeleteAsync($"cases/category/{id}");
        if (response.IsSuccessStatusCode)
        {
            await cacheService.SendCasesItemsReloadMessage();
            return new DefaultResponse<bool> { Result = true, StatusCode = HttpStatusCode.OK, Message = "Category deleted successfully." };
        }
        return new ErrorResponse<bool> { Result = false, StatusCode = response.StatusCode, Message = response.ReasonPhrase };
    }



    public async Task<IResponse<List<CaseDto>>> GetCasesOfCategory(string categoryId)
    {
        var response = await _client.GetAsync($"cases/from-category/{categoryId}");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.ReadResponse<List<CaseDto>>();
            return result ?? new ErrorResponse<List<CaseDto>>()
            {
                Message = "Failed to retrieve cases of category.",
                StatusCode = HttpStatusCode.InternalServerError
            };
        }

        return new ErrorResponse<List<CaseDto>>()
        {
            Message = response.ReasonPhrase,
            ErrorDetails = response.ReasonPhrase,
            Result = null,
            StatusCode = response.StatusCode
        };
    }

    public async Task<IResponse<bool>> AddCaseToCategory(AddCaseToCategoryRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("cases/category/addcase", content);
        if (response.IsSuccessStatusCode)
        {
            await cacheService.SendCasesItemsReloadMessage();
            return new DefaultResponse<bool> { Result = true, StatusCode = HttpStatusCode.OK, Message = "Case added to category successfully." };
        }
        return new ErrorResponse<bool> { Result = false, StatusCode = response.StatusCode, Message = response.ReasonPhrase };

    }
    public async Task<IResponse<bool>> ChangeCategory(string id, string name, IFormFile? image = null)
    {
        using var form = new MultipartFormDataContent
        {
            { new StringContent(name), "name" }
        };
        if (image != null && image.Length > 0)
        {
            form.Add(new StreamContent(image.OpenReadStream()), "imageFile", image.FileName);
        }
        var response = await _client.PutAsync($"cases/category/{id}", form);
        if (response.IsSuccessStatusCode)
        {
            await cacheService.SendCasesItemsReloadMessage();
            return new DefaultResponse<bool> { Result = true, StatusCode = HttpStatusCode.OK, Message = "Category updated successfully." };
        }
        return new ErrorResponse<bool> { Result = false, StatusCode = response.StatusCode, Message = response.ReasonPhrase };
    }

    public async Task<IResponse<CaseDto>?> ChangeCase(string id, CaseParams caseParams)
    {
        var content = new StringContent(JsonSerializer.Serialize(caseParams), Encoding.UTF8, "application/json");
        var response = await _client.PatchAsync($"cases/{id}", content);
        await cacheService.SendCasesItemsReloadMessage();
        return await response.ReadResponse<CaseDto>();
    }

    public async Task<IResponse<CaseDto>?> DeleteCase(string id)
    {
        var response = await _client.DeleteAsync($"cases/{id}");
        await cacheService.SendCasesItemsReloadMessage();
        return await response.ReadResponse<CaseDto>();
    }

    public async Task<IResponse<ItemCaseDto>?> AddItemToCase(AddDeleteItemsRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("cases/additem", content);

        await cacheService.SendCasesItemsReloadMessage();
        return await response.ReadResponse<ItemCaseDto>();
    }

    public async Task<IResponse<ItemCaseDto>?> RemoveItemFromCase(AddDeleteItemsRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("cases/removeitem", content);

        await cacheService.SendCasesItemsReloadMessage();
        return await response.ReadResponse<ItemCaseDto>();
    }

    public async Task<IResponse<List<ItemDto>>?> GetAllItems(DefaultRequest defaultRequest)
    {
        var cachedItems = await itemsCache.GetAllItemsFromCache(defaultRequest.GetQueryParams());
        if (cachedItems != null)
        {
            return new DefaultResponse<List<ItemDto>>()
            {
                Message = "Items retrieved from cache.",
                Result = cachedItems,
                StatusCode = HttpStatusCode.OK
            };
        }


        var response = await _client.GetAsync($"items{defaultRequest.GetQueryParams()}");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.ReadResponse<List<ItemDto>>();
            await itemsCache.SetAllItemsToCache(result.Result, defaultRequest.GetQueryParams());
            return result;
        }

        return new ErrorResponse<List<ItemDto>>()
        {
            Message = response.ReasonPhrase,
            Result = null,
            StatusCode = response.StatusCode
        };
    }

    public async Task<IResponse<GameResultDto>> UpgradeItem(UpgradeItemRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("items/upgrade", content);
        await cacheService.SendReloadUserCache(request.UserId);
        return await response.ReadResponse<GameResultDto>() ?? new ErrorResponse<GameResultDto>() { Message = "Error", StatusCode = HttpStatusCode.InternalServerError };
    }



    public async Task<IResponse<UpgradePreviewResponse>> GetUpgradePreview(UpgradePreviewRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("items/upgrade/preview", content);
        var result = await response.ReadResponse<UpgradePreviewResponse>();
        return result ?? new ErrorResponse<UpgradePreviewResponse>()
        {
            StatusCode = HttpStatusCode.InternalServerError
        };
    }



    public async Task<IResponse<ContractPreviewResponse>> GetPreview(ContractPreviewRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("contracts/preview", content);

        var result = await response.ReadResponse<ContractPreviewResponse>();

        return result ?? new ErrorResponse<ContractPreviewResponse>()
        {
            StatusCode = HttpStatusCode.InternalServerError
        };
    }

    public async Task<IResponse<GameResultDto>> ExecuteContract(ContractExecuteRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("contracts/execute", content);
        var result = await response.ReadResponse<GameResultDto>();
        await cacheService.SendReloadUserCache(request.UserId);
        return result ?? new ErrorResponse<GameResultDto>()
        {
            StatusCode = HttpStatusCode.InternalServerError
        };
    }


    public async Task<IResponse<ItemDto>> GetItem(string id)
    {
        var cachedItem = await itemsCache.GetItemFromCache(id);
        if (cachedItem != null)
        {
            return new DefaultResponse<ItemDto>()
            {
                Message = "Item retrieved from cache.",
                Result = cachedItem,
                StatusCode = HttpStatusCode.OK
            };
        }
        var response = await _client.GetAsync($"items/{id}");

        if (response.IsSuccessStatusCode)
        {
            var result = response.ReadResponse<ItemDto>();
            if (result.Result != null)
            {
                await itemsCache.SetItemToCache(result.Result.Result);
                return result.Result;
            }
        }

        return new ErrorResponse<ItemDto?>()
        {
            Message = "error getting item",
            ErrorDetails = response.ReasonPhrase,
            Result = new ItemDto() { Id = "" },
            StatusCode = HttpStatusCode.InternalServerError
        }!;
    }

    public async Task<IResponse<ItemDto>?> CreateItem(ItemParams itemParams, StreamContent? imageFile = null)
    {
        using var form = new MultipartFormDataContent();

        if (!string.IsNullOrEmpty(itemParams.Name))
            form.Add(new StringContent(itemParams.Name, Encoding.UTF8), "Name");

        form.Add(new StringContent(itemParams.Type.ToString()), "Type");
        form.Add(new StringContent(itemParams.Rarity.ToString()), "Rarity");

        if (itemParams.BaseCost.HasValue)
            form.Add(new StringContent(itemParams.BaseCost.Value.ToString(), Encoding.UTF8),
                "BaseCost"); // ✅ Убрали дублирование

        form.Add(new StringContent(itemParams.IsVisible.ToString(), Encoding.UTF8), "IsVisible");

        if (!string.IsNullOrEmpty(itemParams.Game))
            form.Add(new StringContent(itemParams.Game, Encoding.UTF8), "Game");

        // ✅ Проверяем, что файл передается корректно
        if (imageFile != null)
        {
            var fileName = "item-image.png"; // ✅ Дефолтное имя файла
            form.Add(imageFile, "imageFile", fileName);
            logger.LogInformation("📂 Добавляем файл в запрос: {FileName}", fileName);
        }
        else
        {
            logger.LogWarning("⚠️ Файл отсутствует в запросе.");
        }

        var response = await _client.PostAsync("items/add", form);

        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("✅ Предмет успешно добавлен.");
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync(); // ✅ Логируем ответ сервера
            logger.LogError("❌ Ошибка при добавлении предмета. Код: {StatusCode}, Ответ: {ErrorContent}",
                response.StatusCode, errorContent);
        }

        await cacheService.SendCasesItemsReloadMessage();

        return await response.ReadResponse<ItemDto>();
    }

    public async Task<IResponse<ItemDto>?> ChangeItem(string id, ItemParams itemParams)
    {
        var content = new StringContent(JsonSerializer.Serialize(itemParams), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"items/{id}", content);
        await cacheService.SendCasesItemsReloadMessage();
        return await response.ReadResponse<ItemDto>();
    }

    public async Task<IResponse<GameResultDto>> OpenCase(OpenCaseRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("cases/case/open", content);
        await cacheService.SendReloadUserCache(request.UserId);
        await cacheService.SendCasesItemsReloadMessage();
        return await response.ReadResponse<GameResultDto>() ?? new ErrorResponse<GameResultDto>() { StatusCode = HttpStatusCode.InternalServerError };
    }

    public async Task<IResponse<GameResultDto>> OpenMultipleCases(OpenCaseRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("cases/cases/open", content);
        await cacheService.SendReloadUserCache(request.UserId);
        await cacheService.SendCasesItemsReloadMessage();
        return await response.ReadResponse<GameResultDto>() ?? new ErrorResponse<GameResultDto>() { StatusCode = HttpStatusCode.InternalServerError };
    }


    public async Task<IResponse<ItemDto>?> DeleteItem(string id)
    {
        var response = await _client.DeleteAsync($"items/{id}");
        await cacheService.SendCasesItemsReloadMessage();
        return await response.ReadResponse<ItemDto>();
    }

    public async Task<IResponse<AddRemoveItemRequest>?> UpgradeItemTry(UpgradeItemRecordRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync($"items/upgrade", content);
        return await response.ReadResponse<AddRemoveItemRequest>();
    }


}