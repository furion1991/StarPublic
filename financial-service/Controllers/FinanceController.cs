using System.Net;
using DataTransferLib.CacheServices;
using FinancialService.Database.Models;
using Microsoft.AspNetCore.Mvc;
using DataTransferLib.CommunicationsServices;
using DataTransferLib.DataTransferObjects.Financial;
using FinancialService.Repositories;
using FinancialService.Converters;
using DataTransferLib.DataTransferObjects.Common;
using DataTransferLib.DataTransferObjects.Common.Interfaces;
using DataTransferLib.DataTransferObjects.Financial.Models;
using DataTransferLib.DataTransferObjects.Financial.Payments;
using DataTransferLib.DataTransferObjects.Users;
using DtoClassLibrary.DataTransferObjects.Audit.Dashboard;
using DtoClassLibrary.DataTransferObjects.Bonus;
using DtoClassLibrary.DataTransferObjects.Common.Logs;
using FinancialService.Database.Models.Bonuses;
using FinancialService.Services;
using DtoClassLibrary.DataTransferObjects.Financial.Models;
using DtoClassLibrary.DataTransferObjects.Users;
using Newtonsoft.Json;

namespace FinancialService.Controllers;

[Route("finance")]
[ApiController]
public class FinanceController(
    IRepository<FinancialData> financialRepo,
    IRepository<Transaction> transactionRepo,
    ILogger<FinanceController> logger,
    RabbitMqService rabbitMqService,
    CacheService cacheService,
    BonusService bonusService,
    CasesCommService casesCommService,
    AdminLogRabbitMqService adminLogService, PaymentService paymentService
    ) : ControllerBase
{
    private readonly FinancialDataRepository _financialRepo = (FinancialDataRepository)financialRepo;
    private readonly TransactionRepository _transactionRepo = (TransactionRepository)transactionRepo;


    [HttpPost("sell-item")]
    public async Task<IActionResult> SellItem([FromBody] TransactionParams transactionParams)
    {
        Transaction? transaction;
        try
        {
            logger.LogInformation($"Transaction params are: {JsonConvert.SerializeObject(transactionParams)}");
            transaction = await _transactionRepo.AddDeposit(transactionParams, _financialRepo, true);
            if (transaction == null || transaction.FinancialData == null)
                return BadRequest("Параметры для добавления не были переданы, " +
                                  "финансовые данные пользователя не найдены, сумма продажи отрицательна или больше текущего баланса");
            await rabbitMqService.SendLog("Продажа предмета №" + transaction.Id, transaction, LTYPE.Financial);
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }
        return new RequestService().GetResponse("Предмет продан:", transaction.FinancialData.CurrentBalance);
    }


    [HttpPost("")]
    public async Task<IActionResult> AddFinancialData([FromBody] FinancialDataParams? financialDataParams)
    {
        FinancialData? financialData;
        try
        {
            financialData = await _financialRepo.Add(financialDataParams, true);
            if (financialData == null)
                return BadRequest("Параметры для добавления не были переданы");

            await rabbitMqService.SendLog("Добавлены новые финансовые данные №" + financialData.Id, financialData,
                LTYPE.Financial);
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }

        return new RequestService().GetResponse(
            "Финансовые данные пользователя были успешно созданы:",
            new FinancialDataToDto(financialData).Convert()
        );
    }

    [HttpGet("balance/{userId}")]
    public async Task<IActionResult> GetBalance(string userId)
    {
        FinancialData? financialData = await _financialRepo.Get(userId);
        if (financialData == null)
            return NotFound("Финансовые данные пользователя не найдены");

        return new RequestService().GetResponse("Баланс пользователя:", financialData.CurrentBalance + financialData.BonusBalance);
    }

    [HttpPost("payment-link")]
    public async Task<IActionResult> GetPaymentLink([FromBody] CommonPaymentLinkRequest request)
    {
        var link = await paymentService.GetPaymentLink(request);
        return Ok(link);
    }

    [HttpGet("payment-providers")]
    public async Task<IActionResult> GetPaymentProviders()
    {
        var providers = await paymentService.GetPaymentProviders();
        return Ok(providers);
    }
    [HttpGet("bonus-balance/{userId}")]
    public async Task<IActionResult> GetBonusBalance(string userId)
    {
        FinancialData? financialData = await _financialRepo.Get(userId);
        if (financialData == null)
            return NotFound("Финансовые данные пользователя не найдены");
        return new RequestService().GetResponse("Бонусный баланс пользователя:", financialData.BonusBalance);
    }

    [HttpDelete("revert/transaction/{userId}")]
    public async Task<IActionResult> RevertLastTransaction(string userId)
    {
        try
        {
            await _transactionRepo.RevertLastTransaction(userId);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }


    [HttpGet("bonus-valid")]
    public async Task<IActionResult> GetUserBonusValidation([FromQuery] string userId)
    {
        var finData = await _financialRepo.Get(userId);
        if (finData == null)
            return NotFound("Финансовые данные пользователя не найдены");

        var valid = finData?.Transactions?
            .Where(e => e.Type == TTYPE.Deposit)
            .Any(e => e.Amount > 30);

        return valid == true ? Ok() : BadRequest();
    }


    [HttpPost("deposit")]
    public async Task<IActionResult> AddDeposit([FromBody] TransactionParams? transactionParams)
    {
        Transaction? transaction;
        logger.LogInformation($"Transaction params are: {JsonConvert.SerializeObject(transactionParams)}");
        if (transactionParams == null)
        {
            return BadRequest();
        }
        try
        {
            //if (await bonusService.CheckIfUserHasActiveBonus(transactionParams.UserId))
            //{
            //    await bonusService.ApplyBonusToTransaction(transactionParams);
            //}

            transaction = await _transactionRepo.AddDeposit(transactionParams, _financialRepo, true);
            if (transaction == null || transaction.FinancialData == null)
                return BadRequest("Параметры для добавления не были переданы, " +
                                  "финансовые данные пользователя не найдены, или сумма депозита указана равной 0 или отрицательной");
            await rabbitMqService.SendLog("Добавлен новый депозит №" + transaction.Id, transaction,
                LTYPE.Financial);

            await SendLogMessageAfterAction(transactionParams);


        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }

        return new RequestService().GetResponse("Баланс пополнен:", transaction.FinancialData.CurrentBalance);
    }



    [HttpPost("purchase")]
    public async Task<IActionResult> MakePurchase([FromBody] TransactionParams transactionParams)
    {
        Transaction? transaction;
        try
        {
            if (await bonusService.CheckIfUserHasActiveDiscountBonus(transactionParams.UserId))
            {
                await bonusService.ApplyDiscountBonusToTransaction(transactionParams);
            }

            transaction = await _transactionRepo.MakePurchase(transactionParams, _financialRepo, true);
            if (transaction == null || transaction.FinancialData == null)
                return BadRequest("Параметры для добавления не были переданы, " +
                                  "финансовые данные пользователя не найдены, сумма покупки отрицательна или больше текущего баланса");
            await rabbitMqService.SendLog("Совершена новая покупка № " + transaction.Id, transaction,
                LTYPE.Financial);

            var cashbackBonus = await bonusService.GetCashBackBonusIfAvailable(transactionParams);
            if (cashbackBonus != null)
            {
                var cashBackParams = new TransactionParams()
                {
                    Amount = transactionParams.Amount * cashbackBonus.CashbackPercentage,
                    UserId = transactionParams.UserId,
                    PaymentType = transactionParams.PaymentType
                };
                var cashbackTransaction = await _transactionRepo.AddCashback(cashBackParams, _financialRepo, true);
                await rabbitMqService.SendLog("Added cashback " + cashbackTransaction.Id, cashbackTransaction,
                    LTYPE.Financial);
                await SendLogMessageAfterAction(transactionParams);
            }
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }


        return new RequestService().GetResponse("Покупка совершена:", transaction.FinancialData.CurrentBalance);
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> MakeWithdraw([FromBody] TransactionParams transactionParams)
    {
        Transaction? transaction;
        try
        {
            transaction = await _transactionRepo.MakeWithdraw(transactionParams, _financialRepo, true);
            if (transaction == null || transaction.FinancialData == null)
                return BadRequest("Параметры для добавления не были переданы, " +
                                  "финансовые данные пользователя не найдены, сумма для вывода отрицательна или больше текущего баланса");
            await cacheService.DropCachedEntity<UserDto>(transactionParams.UserId);
            await rabbitMqService.SendLog("Выведены средства №" + transaction.Id, transaction, LTYPE.Financial);
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }

        return new RequestService().GetResponse("Средства выведены:", transaction.FinancialData.CurrentBalance);
    }

    [HttpGet("transactions/{userId}")]
    public async Task<IActionResult> GetTransactionsHistory(string userId, [FromQuery] DefaultRequest defaultRequest)
    {
        int count;
        List<Transaction> transactions;
        try
        {
            FinancialData? financialData = await _financialRepo.Get(userId);
            if (financialData == null)
                return NotFound("Финансовые данные пользователя не найдены");

            RequestService.CheckPaginationParams(ref defaultRequest);
            transactions = await _transactionRepo.GetOrderByWithFinancialData(financialData, defaultRequest);
            count = await _transactionRepo.GetCountWithFinancialData(financialData, defaultRequest);
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }

        return new RequestService().GetResponse(
            "История транзакций пользователя:",
            new TransactionToDto().Convert(transactions),
            page: defaultRequest.Page,
            count: count
        );
    }


    [HttpPost("balances")]
    public async Task<IActionResult> GetBalancesList([FromBody] List<string> userIds)
    {
        var finRecords = await _financialRepo.GetAll();

        var usersFinRecords = finRecords.Where(e => userIds.Contains(e.UserId)).ToList();

        var result = new Dictionary<string, decimal>();

        foreach (var user in usersFinRecords)
        {
            result[user.UserId] = user.CurrentBalance;
        }

        return new RequestService().GetResponse("balances", result);
    }
    [HttpPost("bonus-balances")]
    public async Task<IActionResult> GetBonusBalancesList([FromBody] List<string> userIds)
    {
        var finRecords = await _financialRepo.GetAll();

        var usersFinRecords = finRecords.Where(e => userIds.Contains(e.UserId)).ToList();

        var result = new Dictionary<string, decimal>();

        foreach (var user in usersFinRecords)
        {
            result[user.UserId] = user.BonusBalance;
        }

        return new RequestService().GetResponse("balances", result);
    }
    [HttpGet("transaction/{id}")]
    public async Task<IActionResult> GetTransaction(string id)
    {
        Transaction? transaction;
        try
        {
            transaction = await _transactionRepo.Get(id);
            if (transaction == null)
                return NotFound("Транзакция не найдена");
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }

        return new RequestService().GetResponse(
            "Данные транзакции:",
            new TransactionToDto(transaction).Convert()
        );
    }

    private async Task SendLogMessageAfterAction(TransactionParams transactionParams)
    {
        var allBalances = await financialRepo.GetAll();
        var transactions = await transactionRepo.GetAll();

        var balancesSum = allBalances.Sum(b => b.CurrentBalance);
        var bonusBalances = allBalances.Sum(b => b.BonusBalance);

        var deposits = transactions.Where(e => e.Type == TTYPE.Deposit).ToList();
        var withdrawals = transactions.Where(e => e.Type == TTYPE.Withdraw).ToList();

        var withdrawalsSum = withdrawals.Sum(e => e.Amount);
        var depositsSum = deposits.Sum(e => e.Amount);


        var payload = new DashboardBalanceDto()
        {
            AllUsersBalance = balancesSum,
            AllUsersBonusBalance = bonusBalances,
            AmountWithdrawn = withdrawalsSum,
            TotalProfit = depositsSum - withdrawalsSum,
        };

        var adminLog = new AdminLog<DashboardBalanceDto>()
        {
            Message = "Update balance",
            ActionPerformedBy = transactionParams.UserId,
            Payload = payload
        };

        await adminLogService.SendAdminLog(adminLog);
    }
}