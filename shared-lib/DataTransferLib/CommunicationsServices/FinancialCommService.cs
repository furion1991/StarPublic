using System.Net;
using System.Text;
using DataTransferLib.DataTransferObjects.Common;
using DataTransferLib.DataTransferObjects.Financial;
using DataTransferLib.DataTransferObjects.Financial.Models;
using DataTransferLib.DataTransferObjects.Financial.Payments;
using DataTransferLib.DataTransferObjects.Financial.Promocodes;
using DtoClassLibrary.DataTransferObjects.Audit.Admin.Fin;
using DtoClassLibrary.DataTransferObjects.Bonus;
using DtoClassLibrary.DataTransferObjects.Common;
using DtoClassLibrary.DataTransferObjects.Financial.Models;
using DtoClassLibrary.DataTransferObjects.Financial.Withdrawals;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DataTransferLib.CommunicationsServices;

public class FinancialCommService(IHttpClientFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient(CommConfigure.FINANCE_CLIENT_NAME);



    public async Task<bool> RevertLastTransaction(string userId)
    {
        var response = await _client.DeleteAsync($"finance/revert/transaction/{userId}");
        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        return false;
    }


    public async Task<bool> SellItem(TransactionParams transactionParams)
    {
        var content = new StringContent(JsonSerializer.Serialize(transactionParams), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("finance/sell-item", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<IResponse<FinancialDataDto>?> CreateFinancialDataForUser(FinancialDataParams financialDataParams)
    {
        var content = new StringContent(JsonSerializer.Serialize(financialDataParams), Encoding.UTF8,
            "application/json");

        var response = await _client.PostAsync("finance", content);

        var result = await response.ReadResponse<FinancialDataDto>();

        if (result is null)
        {
            return new ErrorResponse<FinancialDataDto>()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Internal service error",
                ErrorDetails = response.ReasonPhrase
            };
        }

        return result;
    }

    public async Task<bool> DeleteBonus(string bonusId)
    {
        var response = await _client.DeleteAsync($"bonus/delete/{bonusId}");
        return response.IsSuccessStatusCode;
    }


    public async Task<IResponse<Dictionary<string, decimal>>> GetUsersBalance(List<string> userIds)
    {
        var content = new StringContent(JsonConvert.SerializeObject(userIds), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("finance/balances", content);

        if (response.IsSuccessStatusCode)
        {
            var balances = await response.ReadResponse<Dictionary<string, decimal>>();
            if (balances is not null)
            {
                return balances;
            }
        }

        return new ErrorResponse<Dictionary<string, decimal>>()
        {
            Message = response.ReasonPhrase,
            Result = null,
            ErrorDetails = "",
            StatusCode = response.StatusCode
        };
    }
    public async Task<IResponse<Dictionary<string, decimal>>> GetUsersBonusBalance(List<string> userIds)
    {
        var content = new StringContent(JsonConvert.SerializeObject(userIds), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("finance/bonus-balances", content);

        if (response.IsSuccessStatusCode)
        {
            var balances = await response.ReadResponse<Dictionary<string, decimal>>();
            if (balances is not null)
            {
                return balances;
            }
        }

        return new ErrorResponse<Dictionary<string, decimal>>()
        {
            Message = response.ReasonPhrase,
            Result = null,
            ErrorDetails = "",
            StatusCode = response.StatusCode
        };
    }
    public async Task<IResponse<decimal>> GetUserBonusBalance(string userId)
    {
        var response = await _client.GetAsync($"finance/bonus-balance/{userId}");
        if (response.IsSuccessStatusCode)
        {
            var balance = await response.ReadResponse<decimal>();
            if (balance is null)
            {
                return new ErrorResponse<decimal>()
                {
                    Message = "Произошла ошибка при получении бонусного баланса",
                    ErrorDetails = response.ReasonPhrase
                };
            }
            return balance; // Возвращаем уже считанный результат
        }
        return new ErrorResponse<decimal>()
        {
            Message = "Произошла ошибка при получении бонусного баланса",
            ErrorDetails = response.ReasonPhrase
        };
    }

    public async Task<IResponse<decimal>?> GetUserBalance(string userId)
    {
        var response = await _client.GetAsync($"finance/balance/{userId}");

        if (response.IsSuccessStatusCode)
        {
            var balance = await response.ReadResponse<decimal>();

            if (balance is null)
            {
                return new ErrorResponse<decimal>()
                {
                    Message = "Произошла ошибка при получении баланса",
                    ErrorDetails = response.ReasonPhrase
                };
            }

            return balance; // Возвращаем уже считанный результат
        }

        return new ErrorResponse<decimal>()
        {
            Message = "Произошла ошибка при получении баланса",
            ErrorDetails = response.ReasonPhrase
        };
    }


    public async Task<bool> GetUserBonusValidation(string userId)
    {
        var response = await _client.GetAsync($"finance/bonus-valid?userId={userId}");
        return response.IsSuccessStatusCode;
    }


    public async Task<IResponse<float>> MakeTransaction(TransactionParams transactionParams)
    {
        var content = new StringContent(JsonSerializer.Serialize(transactionParams), Encoding.UTF8, "application/json");

        string transactionUri = transactionParams.Type switch
        {
            TTYPE.Deposit => "finance/deposit",
            TTYPE.Purchase => "finance/purchase",
            TTYPE.Withdraw => "finance/withdraw",
            TTYPE.Bonus => "finance/deposit",
            _ => throw new ArgumentOutOfRangeException()
        };


        var response = await _client.PostAsync(transactionUri, content);
        var result = await response.ReadResponse<float>();

        if (result is not null)
        {
            return result;
        }

        return new ErrorResponse<float>()
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Message = "Error on transaction",
            ErrorDetails = response.ReasonPhrase
        };
    }

    public async Task<IResponse<List<TransactionDto>>> GetUserTransactions(string userId, DefaultRequest defaultRequest)
    {
        // Создаем строку запроса с параметрами
        var queryString = new StringBuilder();
        queryString.Append($"?page={defaultRequest.Page}");
        queryString.Append($"&count={defaultRequest.Count}");
        queryString.Append($"&orderBy={defaultRequest.OrderBy}");
        queryString.Append($"&orderType={defaultRequest.OrderType}");

        if (!string.IsNullOrEmpty(defaultRequest.FilterBy))
        {
            queryString.Append($"&filterBy={defaultRequest.FilterBy}");
        }

        if (!string.IsNullOrEmpty(defaultRequest.FilterValue))
        {
            queryString.Append($"&filterValue={defaultRequest.FilterValue}");
        }

        // Выполняем запрос с добавлением queryString
        var response = await _client.GetAsync($"finance/transactions/{userId}{queryString}");

        if (response.IsSuccessStatusCode)
        {
            var transactions = await response.ReadResponse<List<TransactionDto>>();
            if (transactions is not null)
            {
                return transactions;
            }
        }

        return new ErrorResponse<List<TransactionDto>>()
        {
            StatusCode = response.StatusCode,
            Message = "Ошибка при получении транзакций",
            ErrorDetails = response.ReasonPhrase
        };
    }

    public async Task<IResponse<TransactionDto>> GetTransactionById(string id)
    {
        var response = await _client.GetAsync($"finance/transaction/{id}");
        return await response.ReadResponse<TransactionDto>() ?? new ErrorResponse<TransactionDto>() { StatusCode = HttpStatusCode.InternalServerError };
    }


    public async Task<IResponse<string>> FullDeleteBonus(string id)
    {
        var response = await _client.DeleteAsync($"bonus/delete_full/{id}");

        if (response.IsSuccessStatusCode)
        {
            return await response.ReadResponse<string>() ?? new DefaultResponse<string>()
            { Message = "Deleted bonus", Result = id, StatusCode = HttpStatusCode.OK };

        }

        return new ErrorResponse<string>()
        {
            Message = "Error in deleting bonus",
            Result = await response.Content.ReadAsStringAsync(),
            StatusCode = HttpStatusCode.InternalServerError
        };
    }

    public async Task<IResponse<T>> AddBonus<T>(T bonus) where T : IBonusDto
    {
        var response = await _client.PostAsync(GetUrlForBonus(bonus), ContentCreator.CreateStringContent(JsonConvert.SerializeObject(bonus)));
        return await response.ReadResponse<T>() ?? new ErrorResponse<T>() { StatusCode = HttpStatusCode.InternalServerError };
    }

    public async Task<IResponse<UserBonusRecordDto>> GetLatestUserBonusRecord(string userId)
    {
        var response = await _client.GetAsync($"bonus/latest/{userId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return new ErrorResponse<UserBonusRecordDto>()
            {
                Message = "Not found",
                ErrorDetails = string.Empty,
                StatusCode = HttpStatusCode.NotFound
            };
        }

        return await response.ReadResponse<UserBonusRecordDto>() ??
               new ErrorResponse<UserBonusRecordDto>() { StatusCode = HttpStatusCode.InternalServerError };
    }


    public async Task<IResponse<string>> GetAllBonuses()
    {
        var response = await _client.GetAsync("bonus/get/all");

        var json = await response.Content.ReadAsStringAsync();
        return response.IsSuccessStatusCode
            ? new DefaultResponse<string>() { Message = "Bonuses", Result = json, StatusCode = response.StatusCode }
            : new ErrorResponse<string>() { StatusCode = HttpStatusCode.InternalServerError };
    }



    public async Task<IResponse<UserBonusRecordDto>> SpinWheelForUser(string userId)
    {
        var response = await _client.PostAsync($"bonus/wheel/add/{userId}", null);

        return await response.ReadResponse<UserBonusRecordDto>() ?? new ErrorResponse<UserBonusRecordDto>()
        { StatusCode = HttpStatusCode.InternalServerError };
    }


    private string GetUrlForBonus(IBonusDto bonus)
    {
        var url = string.Empty;
        switch (bonus.BonusType)
        {
            case BonusType.None:
                return url;
            case BonusType.BalanceBonus:
                url = "bonus/add/balance/bonus";
                break;
            case BonusType.CashbackBonus:
                url = "bonus/add/cashback/bonus";
                break;
            case BonusType.DepositBonus:
                url = "bonus/add/deposit/bonus";
                break;
            case BonusType.DiscountBonus:
                url = "bonus/add/discount/bonus";
                break;
            case BonusType.FreeCaseBonus:
                url = "bonus/add/freecase/bonus";
                break;
            case BonusType.ItemBonus:
                url = "bonus/add/item/bonus";
                break;
            case BonusType.RandomCaseBonus:
                url = "bonus/add/randomcase/bonus";
                break;
            case BonusType.FreeSpinBonus:
                url = "bonus/add/wheelspin/bonus";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return url;
    }


    public async Task<IResponse<List<FinancialDataDto>>> GetAllFinData()
    {
        var response = await _client.GetAsync("finance/all-data");
        if (response.IsSuccessStatusCode)
        {
            var data = await response.ReadResponse<List<FinancialDataDto>>();
            return data;
        }
        return new ErrorResponse<List<FinancialDataDto>>()
        {
            StatusCode = response.StatusCode,
            Message = "Failed to retrieve financial data",
            ErrorDetails = response.ReasonPhrase
        };
    }

    public async Task<IResponse<List<UserListAdminFinDataDto>>> GetAdminFinDataForUsers(List<string> userList)
    {
        var content = new StringContent(JsonConvert.SerializeObject(userList), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("finance/data-for-users", content);

        return await response.ReadResponse<List<UserListAdminFinDataDto>>() ?? new ErrorResponse<List<UserListAdminFinDataDto>>()
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Message = "Failed to retrieve financial data for users.",
            ErrorDetails = response.ReasonPhrase
        };
    }

    public async Task<IResponse<UserListAdminFinDataDto>> GetAdminFinDataForUser(string userId)
    {
        var response = await _client.GetAsync($"finance/data/{userId}");
        return await response.ReadResponse<UserListAdminFinDataDto>() ?? new ErrorResponse<UserListAdminFinDataDto>()
        { StatusCode = HttpStatusCode.InternalServerError };
    }

    #region Promocodes

    public async Task<IResponse<List<PromocodeDto>>> GetAllPromocodes(DateFilterRequest request)
    {
        var query = request.GetQueryParams();
        var response = await _client.GetAsync($"finance/promocode/all{query}");
        return await response.ReadResponse<List<PromocodeDto>>() ?? new ErrorResponse<List<PromocodeDto>>()
        { StatusCode = HttpStatusCode.InternalServerError };
    }

    public async Task<IResponse<PromocodeDto>> GetPromocodeById(string id)
    {
        var response = await _client.GetAsync($"finance/promocode/{id}");
        return await response.ReadResponse<PromocodeDto>() ?? new ErrorResponse<PromocodeDto>()
        { StatusCode = HttpStatusCode.InternalServerError };
    }

    public async Task<IResponse<bool>> DeletePromocode(string id)
    {
        var response = await _client.DeleteAsync($"finance/promocode/delete/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.ReadResponse<bool>() ?? new DefaultResponse<bool>()
            { Message = "Promocode deleted successfully", Result = true, StatusCode = HttpStatusCode.OK };
        }
        return new ErrorResponse<bool>()
        {
            StatusCode = response.StatusCode,
            Message = "Failed to delete promocode",
            ErrorDetails = response.ReasonPhrase
        };
    }

    public async Task<IResponse<bool>> UpdatePromocode(PromocodeDto promocodeDto)
    {
        var content = new StringContent(JsonConvert.SerializeObject(promocodeDto), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"finance/promocode/update/{promocodeDto.Id}", content);
        if (response.IsSuccessStatusCode)
        {
            return await response.ReadResponse<bool>() ?? new DefaultResponse<bool>()
            { Message = "Promocode updated successfully", Result = true, StatusCode = HttpStatusCode.OK };
        }
        return new ErrorResponse<bool>()
        {
            StatusCode = response.StatusCode,
            Message = "Failed to update promocode",
            ErrorDetails = response.ReasonPhrase
        };
    }

    public async Task<IResponse<bool>> CreatePromocode(PromocodeDto promocodeDto)
    {
        var content = new StringContent(JsonConvert.SerializeObject(promocodeDto), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("finance/promocode/create", content);

        if (response.IsSuccessStatusCode)
        {
            return await response.ReadResponse<bool>() ?? new DefaultResponse<bool>()
            { Message = "Promocode created successfully", Result = true, StatusCode = HttpStatusCode.OK };
        }
        return new ErrorResponse<bool>()
        {
            StatusCode = response.StatusCode,
            Message = "Failed to create promocode",
            ErrorDetails = response.ReasonPhrase
        };
    }

    #endregion


    #region Withdrawals

    public async Task<IResponse<List<WithdrawalListDto>>> GetWithdrawals(DateFilterRequest request)
    {
        var query = request.GetQueryParams();
        var response = await _client.GetAsync($"finance/withdrawals{query}");
        return await response.ReadResponse<List<WithdrawalListDto>>() ?? new ErrorResponse<List<WithdrawalListDto>>()
        { StatusCode = HttpStatusCode.InternalServerError };
    }

    public async Task<IResponse<WithdrawalDetails>> GetWithdrawalById(string id)
    {
        var response = await _client.GetAsync($"finance/withdrawal/{id}");
        return await response.ReadResponse<WithdrawalDetails>() ?? new ErrorResponse<WithdrawalDetails>()
        { StatusCode = HttpStatusCode.InternalServerError };
    }

    #endregion

    #region Payment

    public async Task<string> GetPaymentLink(CommonPaymentLinkRequest request)
    {
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("finance/payment-link", content);
        if (response.IsSuccessStatusCode)
        {
            var paymentLink = await response.Content.ReadAsStringAsync();
            return paymentLink ?? string.Empty;
        }
        return string.Empty;
    }

    public async Task<List<string>> GetPaymentProviders()
    {
        var response = await _client.GetAsync("finance/payment-providers");
        if (!response.IsSuccessStatusCode) return [];
        var providers = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<string>>(providers) ?? new List<string>();

    }

    #endregion
}

