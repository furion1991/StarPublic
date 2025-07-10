using System.Net;
using System.Security.Claims;
using System.Text;
using ApiGateway.Services;
using AuthService.DataTransfer;
using DataTransferLib.Auth;
using DataTransferLib.CacheServices;
using DataTransferLib.CommunicationsServices;
using DataTransferLib.DataTransferObjects.Auth;
using DataTransferLib.DataTransferObjects.CasesItems;
using DataTransferLib.DataTransferObjects.Common;
using DataTransferLib.DataTransferObjects.Financial.Models;
using DataTransferLib.DataTransferObjects.Financial.Payments;
using DataTransferLib.DataTransferObjects.Users;
using DtoClassLibrary.DataTransferObjects.Auth;
using DtoClassLibrary.DataTransferObjects.Auth.Telegram;
using DtoClassLibrary.DataTransferObjects.Auth.VkAuth;
using DtoClassLibrary.DataTransferObjects.Bonus;
using DtoClassLibrary.DataTransferObjects.CasesItems;
using DtoClassLibrary.DataTransferObjects.CasesItems.Contracts;
using DtoClassLibrary.DataTransferObjects.CasesItems.Models;
using DtoClassLibrary.DataTransferObjects.CasesItems.Upgrade;
using DtoClassLibrary.DataTransferObjects.Financial.Models;
using DtoClassLibrary.DataTransferObjects.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiGateway.Controllers;

[ApiController]
[Route("v1")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class RouteController(
    UsersCommService usersCommService,
    AuthCommService authService,
    CasesCommService casesCommService,
    FinancialCommService financialCommService,
    CacheService cacheService,
    RabbitMqCacheService rabbitMqCacheService,
    ItemsCache itemsCache,
    CasesCache casesCache,
    UsersCache usersCache,
    AuditCommService auditCommService,
    ILogger<RouteController> logger)
    : ControllerBase
{
    #region UsersMethods

    /// <summary>
    /// Получает по Id пользователя самый часто открываемый кейс
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [ApiExplorerSettings(GroupName = "User")]
    [HttpGet("audit/favcase/{userId}")]
    public async Task<ActionResult<CaseDto>> GetFavouriteCaseForUser(string userId)
    {
        var favCase = await auditCommService.GetFavouriteCaseForUser(userId);

        return favCase.StatusCode == HttpStatusCode.OK ? Ok(favCase.Result) : StatusCode((int)favCase.StatusCode);
    }

    /// <summary>
    /// Получение предмета максимальной стоимости у юзера в инвентаре
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("audit/max_cost_item/{userId}")]
    [ApiExplorerSettings(GroupName = "User")]
    public async Task<ActionResult<ItemDto>> GetMaxCostItemForUser(string userId)
    {
        var maxCostItem = await auditCommService.GetMaxCostItemForUserAsync(userId);
        return maxCostItem.StatusCode == HttpStatusCode.OK ? Ok(maxCostItem) : StatusCode((int)maxCostItem.StatusCode);
    }

    /// <summary>
    /// Проверка на существование пользователя в базе сайта
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    [HttpGet("exists/{email}")]
    [ApiExplorerSettings(GroupName = "User")]
    public async Task<ActionResult<bool>> CheckIfUserExists(string email)
    {
        var result = await authService.CheckIfUserExists(email);
        return StatusCode((int)result.StatusCode, result.Result);
    }


    /// <summary>
    /// Метод для получения данных о пользователе на основе AccessToken. Возвращает данные текущего пользователя на основе AccessToken
    /// </summary>
    /// <returns></returns>
    [HttpGet("users/me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ApiExplorerSettings(GroupName = "User")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized("User ID is missing in the token.");
        }

        var cachedUser = await usersCache.GetUserFromCache(userId);

        if (cachedUser != null)
        {
            logger.LogInformation($"User {userId} found in cache.\n\n\n\n\n\n\n\n");
            return new RequestService().GetResponse("User:", cachedUser);
        }

        // Получение данных пользователя по userId
        var user = await usersCommService.GetUserData(userId);

        await usersCache.SetUserToCache(user?.Result);


        return new RequestService().GetResponse("User:", user?.Result);
    }


    /// <summary>
    /// Получает данные пользователя по его Id в виде. Возвращает данные пользователя
    /// </summary>
    /// <param name="id">guid пользователя</param>
    /// <returns></returns>
    [HttpGet("users/id/{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ApiExplorerSettings(GroupName = "User")]
    public async Task<ActionResult<UserDto>> GetUserById(string id)
    {
        var cachedUser = await usersCache.GetUserFromCache(id);

        if (cachedUser != null)
        {
            return new RequestService().GetResponse("User:", cachedUser);
        }

        var result = await usersCommService.GetUserData(id);
        await cacheService.CacheEntity(result?.Result);

        return StatusCode((int)result!.StatusCode, result.Result);
    }

    /// <summary>
    /// Получение данных по электронной почте. Возвращает данные пользователя
    /// </summary>
    /// <param name="mail">Электронная почта пользователя</param>
    /// <returns></returns>
    [HttpGet("users/mail/{mail}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ApiExplorerSettings(GroupName = "User")]
    public async Task<ActionResult<UserDto>> GetUserByEmail(string mail)
    {
        var result = await usersCommService.GetUserData(mail, true);

        if (result != null)
        {
            await cacheService.CacheEntity(result.Result);
            return StatusCode((int)result.StatusCode, result.Result);
        }

        return NotFound();
    }

    /// <summary>
    /// Изменить учетные данные пользователя,  Возвращает обновленные данные юзера в случае успеха, а в случае неудачи статус 500
    /// </summary>
    /// <param name="id">GUID пользователя</param>
    /// <param name="request">В теле POST запроса передаем параметры которые необходимо поменять (Email, UserName, Phone) ID НЕ ПЕРЕДАВАТЬ.
    ///</param>
    /// <returns></returns>
    [HttpPut("users/update/{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse<UserDto>), StatusCodes.Status500InternalServerError)]
    [ApiExplorerSettings(GroupName = "User")]
    public async Task<ActionResult<UserDto>> UpdateUser(string id, [FromBody] CreateUpdateUserRequest request)
    {
        await usersCache.DropUserCache(id);
        var result = await usersCommService.UpdateUser(id, request);

        if (result != null)
            return StatusCode((int)result.StatusCode, result.Result);
        return StatusCode(500);
    }


    /// <summary>
    /// Изменение роли пользователя
    /// </summary>
    /// <param name="userRole">Передаем строкой роль пользователя (может быть "User", "Admin", "Manager").
    /// Возвращает объект записи о роли юзера в таблице в случае успеха, а в случае неудачи статус 500</param>
    /// <returns></returns>
    [HttpPut("users/update/role")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse<UserDto>), StatusCodes.Status500InternalServerError)]
    [ApiExplorerSettings(GroupName = "User")]
    public async Task<ActionResult<string>> UpdateUserRole([FromBody] UserRoleUpdateRequest userRole)
    {
        var resultFromAuth = await authService.UpdateUserRole(userRole);

        if (resultFromAuth is null)
        {
            return StatusCode(500, "Error while updating user role");
        }

        if (resultFromAuth.StatusCode != HttpStatusCode.OK)
        {
            return StatusCode((int)resultFromAuth.StatusCode, resultFromAuth);
        }

        var result = await usersCommService.UpdateUserRole(userRole);

        var cacheKey = $"User_{userRole.UserId}";
        await usersCache.DropUserCache(userRole.UserId);
        if (result != null)
            return StatusCode((int)result.StatusCode, result.Result);
        return StatusCode(500);
    }


    /// <summary>
    /// Регистрация пользователя в системе При помощи электронной почты Возвращает объект с результатом регистрации
    /// </summary>
    /// <param name="registrationEmailModel">В теле запроса передаем почту, пароль, РОЛЬ МОЖНО НЕ ПЕРЕДАВАТЬ ТАК КАК ПО ДЕФОЛТУ ИЗ ЭТОЙ АПИШКИ БУДЕТ СЕТИТЬ "User".
    /// </param>
    /// <returns></returns>
    [HttpPost("auth/register/email")]
    [ApiExplorerSettings(GroupName = "Auth")]
    public async Task<IActionResult> Register([FromBody] RegistrationEmailModel registrationEmailModel)
    {
        var result = await authService.Register(registrationEmailModel);
        if (result.StatusCode == HttpStatusCode.OK)
        {
            return Ok("Register successful");
        }

        return StatusCode((int)result.StatusCode, result);
    }


    /// <summary>
    /// Подтверждение электронной почты
    /// </summary>
    /// <returns></returns>
    [HttpGet("confirm-email")]
    [ApiExplorerSettings(GroupName = "Auth")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
    {
        var result = await authService.ConfirmEmail(new ConfirmEmailRequest() { Email = email, Token = token });
        return StatusCode((int)result.StatusCode, result.Result);
    }


    /// <summary>
    /// Повторная отправка сообщения для подтверждения электронной почты
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    [HttpGet("resend-email/{email}")]
    [ApiExplorerSettings(GroupName = "Auth")]
    public async Task<IActionResult> ResendConfirmationEmail(string email)
    {
        var result = await authService.ResendConfirmationEmail(email);
        return StatusCode((int)result.StatusCode, result.Message);
    }

    #endregion

    #region Cases

    /// <summary>
    /// Превью апгрейда, вызывать каждый раз как пользователь выбрал предмет свой и из списка предметов один для апгрейда, возвращает объект с шансом успеха
    /// Поле коэффициент возвращается рандомный, ни на что не влияет в этой версии
    /// Передавать id двух предметов - из инвентаря и целевого (ИМЕННО ID ПРЕДМЕТА, НЕ ЗАПИСЬ В ИНВЕНТАРЕ)
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("items/upgrade/preview")]
    [ApiExplorerSettings(GroupName = "Cases and Items")]
    public async Task<IActionResult> GetUpgradePreview([FromBody] UpgradePreviewRequest request)
    {
        var result = await casesCommService.GetUpgradePreview(request);
        return StatusCode((int)result.StatusCode, result.Result);
    }


    /// <summary>
    /// Выполнение апгрейда, возвращает gameresultdto как в контрактах и в кейсах с результатом апгрейда
    /// Если результат пришел не тот что юзер заказывал в апгрейде - считать провалом
    /// Передаем id записи юзера в инвентаре и id желаемого юзером предмета, а также id самого юзера
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("items/upgrade")]
    [ApiExplorerSettings(GroupName = "Cases and Items")]
    public async Task<IActionResult> UpgradeItem([FromBody] UpgradeItemRequest request)
    {
        var result = await casesCommService.UpgradeItem(request);

        if (result == null)
        {
            Console.WriteLine(
                $"[UpgradeItem] ❌ Upgrade failed for request: {System.Text.Json.JsonSerializer.Serialize(request)}");
            return BadRequest(new DefaultResponse<GameResultDto>()
            { Message = "something went wrong", StatusCode = HttpStatusCode.BadRequest });
        }

        await usersCache.DropUserCache(request.UserId);
        if (result.Result?.Items == null || result.Result?.Items.Count == 0)
        {
            return Conflict(new DefaultResponse<GameResultDto>()
            { Message = "Not allowed to upgrade item to less cost" });
        }
        logger.LogInformation(
            $"Upgrade result is: {((request.AttemptedItemId == result.Result?.Items.FirstOrDefault()?.Item.Id)
                    ? "success" : "failure")}");
        Console.WriteLine($"[UpgradeItem] ✅ Upgrade success: {System.Text.Json.JsonSerializer.Serialize(result)}");
        return new RequestService().GetResponse("Upgrade result", result.Result);
    }


    /// <summary>
    /// Получит данные о всех доступных кейсах
    ///
    /// 
    /// </summary>
    /// <param name="defaultRequest">Query параметры для фильтрации запроса, name, type(Standart || Premium), image, price, open_limit, discount, old_price, возвращает Список объектов кейсов, либо список ошибок, либо статус 500</param>
    /// <returns></returns>
    [HttpGet("cases/getall")]
    [ApiExplorerSettings(GroupName = "Cases and Items")]
    public async Task<ActionResult<CaseListDto>> GetAllCases([FromQuery] DefaultRequest defaultRequest)
    {
        var allCasesCount = await casesCommService.GetAllCasesCount();

        var cachedCases = await casesCache.GetAllCasesFromCache();


        if (cachedCases is not null)
        {
            logger.LogInformation($"Found {cachedCases.Count} cases in cache\n\n\n\n\n\n\n");
            cachedCases = cachedCases.Where(c => c.Items.Count > 0).ToList();
            var cachedResponse = new CaseListDto()
            {
                Count = allCasesCount,
                Cases = cachedCases
            };
            return Ok(cachedResponse);
        }

        if (defaultRequest.Count == 0)
        {
            defaultRequest.Count = DataTransferLib.DataTransferObjects.Common.DefaultRequest.MaximumCount;
        }


        var result = await casesCommService.GetAllCases(defaultRequest);

        if (result?.Result is null)
        {
            return NotFound();
        }

        result.Result = result.Result.Where(c => c.Items.Count > 0).ToList();

        var response = new CaseListDto()
        {
            Cases = result.Result,
            Count = allCasesCount
        };
        await casesCache.SetCasesToCache(result.Result);
        return StatusCode((int)result.StatusCode, response);

        return StatusCode(500);
    }


    /// <summary>
    /// Получение одного кейса возвращает Объект  кейса либо объект с ошибкой либо статус 500
    /// </summary>
    /// <param name="id">Id кейса в формате GUID</param>
    /// <returns></returns>
    [HttpGet("cases/get/{id}")]
    [ProducesResponseType(typeof(CaseDto), StatusCodes.Status200OK)]
    [ApiExplorerSettings(GroupName = "Cases and Items")]
    public async Task<ActionResult<CaseDto>?> GetCase(string id)
    {
        var cachedCase = await casesCache.GetCaseFromCache(id);

        if (cachedCase is not null)
        {
            return Ok(cachedCase);
        }

        var result = await casesCommService.GetCase(id);

        if (result != null)
        {
            await casesCache.SetCaseToCache(result.Result);
            return StatusCode((int)result.StatusCode, result.Result);
        }

        return StatusCode(500);
    }

    #endregion

    #region Items

    /// <summary>
    /// Получит данные о всех доступных предметах возвращает список объектов кейсов, либо список ошибок, либо статус 500
    /// </summary>
    /// <param name="defaultRequest">Query параметры для фильтрации запроса, name, type(Standart || Premium), image, price, open_limit, discount, old_price</param>
    /// <returns></returns>
    [HttpGet("items/getall")]
    [ApiExplorerSettings(GroupName = "Cases and Items")]
    public async Task<ActionResult<List<ItemDto>?>> GetAllItems([FromQuery] DefaultRequest defaultRequest)
    {
        var cachedItems = await itemsCache.GetAllItemsFromCache();
        if (cachedItems is not null)
        {
            return Ok(cachedItems);
        }

        logger.LogInformation($"\n\n\n\n\n\n data in requset: {JsonConvert.SerializeObject(defaultRequest)}");


        if (defaultRequest.Count == 0)
        {
            logger.LogInformation("No count in request\n\n\n\n\n\n\n\n\n\n");
            defaultRequest.Count = DataTransferLib.DataTransferObjects.Common.DefaultRequest.MaximumCount;
        }

        var result = await casesCommService.GetAllItems(defaultRequest);
        if (result != null)
        {
            await itemsCache.SetAllItemsToCache(result.Result);
            return StatusCode((int)result.StatusCode, result.Result);
        }

        return StatusCode(500);
    }

    /// <summary>
    /// Получение отдельного предмета возвращает объект  кейса либо объект с ошибкой либо статус 500
    /// </summary>
    /// <param name="id">Id предмета в формате GUID</param>
    /// <returns></returns>
    [HttpGet("items/get/{id}")]
    [ApiExplorerSettings(GroupName = "Cases and Items")]
    public async Task<ActionResult<ItemDto>?> GetItem(string id)
    {
        var cachedItem = await itemsCache.GetItemFromCache(id);

        if (cachedItem is not null)
        {
            return Ok(cachedItem);
        }

        var result = await casesCommService.GetItem(id);
        if (result != null)
        {
            await itemsCache.SetItemToCache(result.Result);
            return StatusCode((int)result.StatusCode, result.Result);
        }

        return StatusCode(500);
    }

    /// <summary>
    /// Метод для множественного открытия кейсов пользователем со списанием баланса, передаем id юзера и id кейса который юзер хочет открыть, а также их количество в поле Quantity
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("cases/open")]
    [ApiExplorerSettings(GroupName = "Cases and Items")]
    public async Task<IActionResult> OpenMultipleCases([FromBody] OpenCaseRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized("User Id is missing in the token");
        }

        await usersCache.DropUserCache(userId);
        var user = await usersCommService.GetUserData(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }


        var balance = await financialCommService.GetUserBalance(userId);
        var caseFromDb = await casesCommService.GetCase(request.CaseId);
        if (balance.Result < caseFromDb.Result.Price * request.Quantity)
        {
            return BadRequest("Недостаточно средств");
        }

        var financeResult = await financialCommService.MakeTransaction(new TransactionParams()
        {
            Amount = caseFromDb.Result.Price * request.Quantity,
            Type = TTYPE.Purchase,
            PaymentType = PTYPE.Bank,
            UserId = userId
        });
        if (financeResult.StatusCode != HttpStatusCode.OK)
        {
            return StatusCode((int)financeResult.StatusCode);
        }


        var result = await casesCommService.OpenMultipleCases(request);
        if (result.Result is not null && result.Result.Items.Count != 0)
        {
            return new RequestService().GetResponse("Кейс успешно открыт: ", result.Result);
        }
        else
        {
            await financialCommService.RevertLastTransaction(userId);

            return StatusCode(500, "Произошла ошибка при открытии кейса");
        }
    }

    /// <summary>
    /// Метод для открытия кейса пользователем со списанием баланса, передаем id юзера и id кейса который юзер хочет открыть !!!В этом методе Quantity не передаем, тут по умолчанию 1 !!!
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("case/open")]
    [ApiExplorerSettings(GroupName = "Cases and Items")]
    [Authorize]
    public async Task<IActionResult> OpenCase([FromBody] OpenCaseRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized("User ID is missing in the token.");
        }

        await usersCache.DropUserCache(userId);

        // Получение данных пользователя по userId
        var user = await usersCommService.GetUserData(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var balance = await financialCommService.GetUserBalance(userId);
        var caseFromDb = await casesCommService.GetCase(request.CaseId);
        if (balance.Result < caseFromDb.Result.Price)
        {
            return BadRequest("Недостаточно средств");
        }

        var financeResult = await financialCommService.MakeTransaction(new CasePurchaseTransactionParams()
        {
            Amount = caseFromDb.Result.Price,
            Type = TTYPE.Purchase,
            PaymentType = PTYPE.Bank,
            UserId = userId,
            CaseId = caseFromDb.Result.Id
        });

        if (financeResult.StatusCode != HttpStatusCode.OK)
        {
            return StatusCode((int)financeResult.StatusCode);
        }


        var result = await casesCommService.OpenCase(request);

        if (result?.Result?.Items.Count != 0)
        {
            return new RequestService().GetResponse("Кейс успешно открыт: ", result.Result);
        }
        else
        {
            await financialCommService.RevertLastTransaction(userId);

            return StatusCode(500, "Произошла ошибка при открытии кейса");
        }
    }

    /// <summary>
    /// Метод для продажи предметов, передаем список id предметов которые юзер хочет продать, id тут - не id предмета, а id его записи в инвентаре
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    [HttpPost("inventory/sell-item")]
    [ApiExplorerSettings(GroupName = "Cases and Items")]
    [Authorize]
    public async Task<IActionResult> SellItem([FromBody] List<string> inventoryRecordsIds)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


        foreach (var itemId in inventoryRecordsIds)
        {
            var response = await usersCommService.SellItem(itemId);
            if (response != HttpStatusCode.OK)
            {
                return StatusCode((int)response);
            }
        }

        await usersCache.DropUserCache(userId);
        return Ok();
    }

    /// <summary>
    /// Метод получения превью диапазона стоимости получаемого после контракта предмета, вызывать каждый раз при добавлении или удалении предмета в контракте
    /// Важно - тут присылаем именно id ПРЕДМЕТОВ, а не id их записей в инвентаре
    /// Предметов в списке должно быть от 3 до 10
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("contracts/preview")]
    [ApiExplorerSettings(GroupName = "Cases and Items")]
    public async Task<IActionResult> ContractPreview([FromBody] ContractPreviewRequest request)
    {
        if (request.ItemsList.Count is < 3 or > 10)
        {
            return BadRequest("Items count should be from 3 to 10");
        }

        var result = await casesCommService.GetPreview(request);

        logger.LogInformation($"Request: {JsonConvert.SerializeObject(request)}\n\n\n\n\n\n\n\n\n");

        return result.StatusCode != HttpStatusCode.OK ? StatusCode((int)result.StatusCode, result.Message) : Ok(result);
    }

    /// <summary>
    /// Выполнение контракта, сюда передается от 3 до 10 предметов (здесь уже именно id записи в инвентаре), которые юзер хочет обменять на один новый предмет, а также id юзера
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("contracts/execute")]
    [ApiExplorerSettings(GroupName = "Cases and Items")]
    [Authorize]
    public async Task<IActionResult> ExecuteContract([FromBody] ContractExecuteRequest request)
    {
        if (request.ItemRecordIds.Count < 3 || request.ItemRecordIds.Count > 10)
        {
            return BadRequest(new ErrorResponse<bool>() { StatusCode = HttpStatusCode.BadRequest, Message = "Items count are less than 3 or more than 10", Result = false });
        }
        var result = await casesCommService.ExecuteContract(request);

        if (result.StatusCode != HttpStatusCode.OK)
        {
            return StatusCode((int)result.StatusCode, result.Message);
        }

        await usersCache.DropUserCache(request.UserId);
        return new RequestService().GetResponse("Contract executed", result.Result);
    }

    #endregion

    #region Finance

    [ApiExplorerSettings(GroupName = "Finances")]
    [HttpPost("finance/payment-link")]
    public async Task<IActionResult> GetPaymentLink([FromBody] CommonPaymentLinkRequest request)
    {
        var result = await financialCommService.GetPaymentLink(request);
        return Ok(result);
    }

    [ApiExplorerSettings(GroupName = "Finances")]
    [HttpGet("finance/payment-providers")]
    public async Task<IActionResult> GetPaymentProviders()
    {
        var result = await financialCommService.GetPaymentProviders();
        return Ok(result);
    }


    /// <summary>
    /// Получить баланс пользователя возвращает Баланс пользователя, либо объект с ошибкой либо статус 500
    /// </summary>
    /// <param name="id">Id пользователя</param>
    /// <returns></returns>
    [Authorize]
    [HttpGet("finance/balance/{id}")]
    [ApiExplorerSettings(GroupName = "Finances")]
    [ProducesResponseType(typeof(float), StatusCodes.Status200OK)]
    public async Task<ActionResult<float>> GetUserBalance(string id)
    {
        var result = await financialCommService.GetUserBalance(id);
        if (result != null)
            return StatusCode((int)result.StatusCode, result.Result);
        return StatusCode(500);
    }

    /// <summary>
    /// Пополнение баланса возвращает Новый баланс пользователя
    /// </summary>
    /// <param name="transactionParams">параметры транзакции, передаем ТОЛЬКО id юзера, сумму и тип транзакции (Пополнение - 0, Покупка - 1, Вывод - 2(это энамы)), а также тип оплаты
    /// (Банк карта - 0, Крипта - 1)</param>
    /// <returns></returns>
    [HttpPost("finance/transaction")]
    [ApiExplorerSettings(GroupName = "Finances")]
    public async Task<ActionResult<float>> AddDeposit([FromBody] TransactionParams transactionParams)
    {
        if (string.IsNullOrEmpty(transactionParams.OrderId))
        {
            return Unauthorized();
        }
        var result = await financialCommService.MakeTransaction(transactionParams);
        if (result.StatusCode == HttpStatusCode.OK)
        {
            await usersCache.DropUserCache(transactionParams.UserId);
        }

        return StatusCode((int)result.StatusCode, result.Message);
    }

    [HttpGet("transactions/{userId}")]
    [ApiExplorerSettings(GroupName = "Finances")]
    public async Task<ActionResult<List<TransactionDto>>?> GetUserTransactions(string userId,
        [FromQuery] DefaultRequest defaultRequest)
    {
        var result = await financialCommService.GetUserTransactions(userId, defaultRequest);

        return StatusCode((int)result.StatusCode, result.Result);
    }

    /// <summary>
    /// получить отдельную транзакцию по Id возвращает объект транзакции
    /// </summary>
    /// <param name="id">id ТРАНЗАКЦИИ</param>
    /// <returns></returns>
    [HttpGet("transaction/id")]
    [ApiExplorerSettings(GroupName = "Finances")]
    public async Task<ActionResult<TransactionDto>?> GetTransaction(string id)
    {
        var result = await financialCommService.GetTransactionById(id);
        return StatusCode((int)result.StatusCode, result.Result);
    }

    #endregion

    #region Bonuses

    /// <summary>
    /// Проверка подписки на розыгрыш призов
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("draw/is-subscribed")]
    [ApiExplorerSettings(GroupName = "Bonus")]
    public async Task<IActionResult> IsSubscribed()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await usersCommService.IsUserSubscribedToPrizeDraw(userId);

        return result ? Ok(result) : BadRequest(result);
    }


    /// <summary>
    /// Подписка на розыгрыш призов
    /// </summary>
    /// <returns></returns>
    [HttpGet("draw/subscribe")]
    [Authorize]
    [ApiExplorerSettings(GroupName = "Bonus")]
    public async Task<IActionResult> SubscribeToPrizeDraw()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await usersCommService.SubscribeToPrizeDraw(userId);
        await usersCache.DropUserCache(userId);
        return result ? Ok(result) : BadRequest(result);
    }


    /// <summary>
    /// Метод для выполнения ежедневного бонуса
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [Authorize]
    [HttpGet("daily-bonus")]
    [ApiExplorerSettings(GroupName = "Bonus")]
    public async Task<IActionResult> AddSmallBonus()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest();
        }

        var result = await usersCommService.AddSmallBonus(userId);
        await usersCache.DropUserCache(userId);
        return StatusCode((int)result.StatusCode, result);
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <returns></returns>
    [HttpGet("bonus/get/all")]
    [ApiExplorerSettings(GroupName = "Bonus")]
    public async Task<IActionResult> GetAllBonuses()
    {
        var result = await financialCommService.GetAllBonuses();
        if (result.StatusCode == HttpStatusCode.OK)
        {
            return Ok(JsonConvert.DeserializeObject(result.Result));
        }

        return StatusCode((int)result.StatusCode, result.Result);
    }


    /// <summary>
    /// TODO
    /// </summary>
    /// <returns></returns>
    [HttpPost("bonus/wheel/spin")]
    [ApiExplorerSettings(GroupName = "Bonus")]
    public async Task<ActionResult<UserBonusRecordDto>> SpinWheelForUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized();
        }

        var result = await financialCommService.SpinWheelForUser(userId);
        await usersCache.DropUserCache(userId);
        return StatusCode((int)result.StatusCode, result);
    }

    /// <summary>
    /// Проверка и добавления бонуса за подписку
    /// </summary>
    /// <returns></returns>
    [HttpPost("subscription-bonus")]
    [Authorize]
    [ApiExplorerSettings(GroupName = "Bonus")]
    public async Task<IActionResult> AddSubscriptionBonus([FromBody] SocialLoginRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized("no user id found in token");
        }

        await usersCache.DropUserCache(userId);

        switch (request.Provider)
        {
            case "vk":
                {
                    var isVkSubscribed = await authService.CheckVkSubscriptionStatus(userId);
                    if (isVkSubscribed.Result)
                    {
                        return BadRequest(new DefaultResponse<bool>()
                        {
                            Message = "user is already subscribed",
                            StatusCode = HttpStatusCode.BadRequest,
                            Result = false
                        });
                    }

                    var vkrequest = new VkUseAuthRequest()
                    {
                        Code = request.VkData.Code,
                        CodeVerifier = request.VkData.CodeVerifier,
                        DeviceId = request.VkData.DeviceId,
                        State = request.VkData.State,
                    };
                    var vkResult = await authService.ValidateVkSubscription(vkrequest, userId);
                    if (vkResult.StatusCode != HttpStatusCode.OK) return StatusCode((int)vkResult.StatusCode, vkResult);
                    return StatusCode((int)vkResult.StatusCode, vkResult);
                }
            case "tg":
                {
                    var isTgSubscribed = await authService.CheckTgSubscriptionStatus(userId);
                    if (isTgSubscribed.Result)
                    {
                        return BadRequest(new DefaultResponse<bool>()
                        {
                            Message = "user is already subscribed",
                            StatusCode = HttpStatusCode.BadRequest,
                            Result = false
                        });
                    }

                    var data = ValidateTelegramData(request);
                    if (data == null)
                        return BadRequest(new DefaultResponse<bool>()
                        {
                            Message = "subscription validation failed due to wrong tg data",
                            StatusCode = HttpStatusCode.BadRequest,
                            Result = false
                        });

                    var result = await authService.ValidateTgSubscription(data, userId);
                    if (result.Result)
                    {
                        return Ok(result);
                    }

                    return BadRequest(result);
                }
        }

        return BadRequest(new DefaultResponse<bool>()
        {
            Result = false,
            StatusCode = HttpStatusCode.BadRequest,
            Message = "subscription validation failed due to unknown provider",
        });
    }

    #endregion

    #region Auth

    /// <summary>
    /// Метод для логина пользователя В случае успеха возвращает сообщение Login successful и accesstoken и refreshtoken в куках, в случае неудачи объект с информацией,
    /// что пошло не так, если все совсем поломалось - статус 500
    /// </summary>
    /// <param name="loginModel">Объект: Логин в виде электронной почты и пароль.
    ///  </param>
    /// <returns></returns>
    [HttpPost("auth/login")]
    [ApiExplorerSettings(GroupName = "Auth")]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        var result = await authService.Login(loginModel);
        if (result == null)
        {
            return StatusCode(500);
        }

        if (result.StatusCode == HttpStatusCode.OK)
        {
            AddCookiesWithTokens(result.Result);

            return Ok(new { message = "Login successful" });
        }

        if (result.StatusCode == HttpStatusCode.Unauthorized)
        {
            return Unauthorized(new
            {
                error = "Invalid Password",
                statusCode = (int)HttpStatusCode.Unauthorized
            });
        }

        if (result.StatusCode == HttpStatusCode.Forbidden)
        {
            return StatusCode(403, "email not confirmed");
        }

        if (result != null)
        {
            return StatusCode((int)result.StatusCode, new
            {
                error = "Unexpected error",
                details = result
            });
        }

        return StatusCode(500, new { error = "Internal Server Error" });
    }


    /// <summary>
    /// Метод для выхода юзера из системы Удаляет токены из кук, отзывает RefreshToken, в случае неудачи объект с информацией, что пошло не так, если все совсем поломалось - статус 500
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpPost("auth/logout")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ApiExplorerSettings(GroupName = "Auth")]
    public async Task<IActionResult> Logout()
    {
        var logoutModel = new LogoutModel();
        if (Request.Cookies.ContainsKey("RefreshToken") && Request.Cookies["RefreshToken"] is not null)
        {
            logoutModel.RefreshToken = Request.Cookies["RefreshToken"];
        }
        else
        {
            return BadRequest();
        }

        var result = await authService.Logout(logoutModel);
        DeleteAuthTokens();
        if (result != null)
            return StatusCode((int)result.StatusCode, result);
        return StatusCode(500);
    }

    [HttpGet("auth/vk/callback")]
    public async Task<IActionResult> VkCallback([FromQuery] string code, [FromQuery] string state)
    {
        if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
        {
            return BadRequest("Invalid VK auth token");
        }

        return Ok(code + " " + state);
    }

    private readonly List<string> _socialProvidersList = ["telegram", "vk"];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("auth/social")]
    [ApiExplorerSettings(GroupName = "Auth")]
    public async Task<IActionResult> LoginSocial([FromBody] SocialLoginRequest request)
    {
        if (!_socialProvidersList.Contains(request.Provider))
        {
            return BadRequest($"{request.Provider} provider is not supported");
        }

        var result = false;
        switch (request.Provider)
        {
            case "telegram":
                result = await HandleTelegramLogin(request);
                break;
            case "vk":
                result = await HandleVkLogin(request);
                break;
        }

        return result ? Ok("Login successful") : StatusCode(500);
    }


    [HttpPost("auth/reset-password")]
    [ApiExplorerSettings(GroupName = "Auth")]
    public async Task<IActionResult> ResetPassword([FromQuery] string email)
    {
        var result = await authService.ResetPassword(email);
        if (result.Result)
        {
            return Ok(result.Message);
        }

        return StatusCode((int)result.StatusCode, result);
    }

    /// <summary>
    /// Обновление AccessToken и RefreshToken при помощи RefreshToken из кук Новый AccessToken и новый RefreshToken
    /// </summary>
    /// <returns></returns>
    [HttpPost("auth/refresh")]
    [ApiExplorerSettings(GroupName = "Auth")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["RefreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized("No refresh token found");
        }

        var refreshResult = await authService.RefreshAccessToken(new RefreshAccessTokenRequest()
        {
            RefreshToken = refreshToken
        });
        if (!string.IsNullOrEmpty(refreshResult?.Result?.Token))
        {
            AddCookiesWithTokens(refreshResult.Result);
            return Ok("Token refreshed");
        }

        return Unauthorized("Invalid refresh token");
    }

    #endregion

    #region PrivateMethods

    private void DeleteAuthTokens()
    {
        Response.Cookies.Delete("AccessToken");
        Response.Cookies.Delete("RefreshToken");
    }

    private void AddCookiesWithTokens(TokensResponse tokens)
    {
        Response.Cookies.Append("AccessToken", tokens.Token, new CookieOptions
        {
            HttpOnly = false,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddMinutes(30)
        });
        Response.Cookies.Append("RefreshToken", tokens.RefreshToken, new CookieOptions()
        {
            HttpOnly = false,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
    }

    private TelegramUserDataDto? ValidateTelegramData(SocialLoginRequest request)
    {
        string cleanedBase64 = request.Data.Trim().Replace(" ", "").Replace("\n", "").Replace("\r", "");

        int mod4 = cleanedBase64.Length % 4;
        if (mod4 > 0)
        {
            cleanedBase64 += new string('=', 4 - mod4);
        }

        byte[] base64Bytes = Convert.FromBase64String(cleanedBase64);
        string jsonString = Encoding.UTF8.GetString(base64Bytes);


        var tgData = JsonConvert.DeserializeObject<TelegramUserDataDto>(jsonString);
        var isValid =
            TelegramHashValidator.ValidateTelegramAuth(tgData, "7339629113:AAEDkYjVHrremfyagzHtCm55BNi1O5Dwm5M");
        if (!isValid)
        {
            return null;
        }

        return tgData;
    }


    private async Task<bool> HandleTelegramLogin(SocialLoginRequest request)
    {
        try
        {
            string cleanedBase64 = request.Data.Trim().Replace(" ", "").Replace("\n", "").Replace("\r", "");

            int mod4 = cleanedBase64.Length % 4;
            if (mod4 > 0)
            {
                cleanedBase64 += new string('=', 4 - mod4);
            }

            byte[] base64Bytes = Convert.FromBase64String(cleanedBase64);
            string jsonString = Encoding.UTF8.GetString(base64Bytes);


            var tgData = ValidateTelegramData(request);

            if (tgData == null) throw new InvalidOperationException();

            var result = await authService.TelegramAuth(tgData);

            if (result?.Result != null)
            {
                AddCookiesWithTokens(result.Result);
                return true;
            }

            return false;
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Ошибка декодирования Base64: {ex.Message}");
            throw;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Общая ошибка: {e.Message}");
            throw;
        }
    }

    private async Task<bool> HandleVkLogin(SocialLoginRequest request)
    {
        var vkRequest = new VkUseAuthRequest()
        {
            Code = request.VkData.Code,
            CodeVerifier = request.VkData.CodeVerifier,
            DeviceId = request.VkData.DeviceId,
            State = request.VkData.State,
        };
        var result = await authService.VkAuth(vkRequest);
        if (result.Result != null && result.StatusCode == HttpStatusCode.OK)
        {
            AddCookiesWithTokens(result.Result);
            return true;
        }

        return false;
    }

    #endregion
}