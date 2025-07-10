using DataTransferLib.DataTransferObjects.Audit;
using DataTransferLib.DataTransferObjects.CasesItems.Models;
using DataTransferLib.DataTransferObjects.Common;
using DtoClassLibrary.DataTransferObjects.Audit;
using DtoClassLibrary.DataTransferObjects.Audit.Dashboard;
using DtoClassLibrary.DataTransferObjects.CasesItems;
using DtoClassLibrary.DataTransferObjects.CasesItems.Models;
using DtoClassLibrary.DataTransferObjects.Common;
using Newtonsoft.Json;
using System.Collections;
using System.Net;
using DtoClassLibrary.DataTransferObjects.Audit.Admin.Fin;

namespace DataTransferLib.CommunicationsServices;

public class AuditCommService(IHttpClientFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient(CommConfigure.AUDIT_CLIENT_NAME);

    public async Task<IResponse<List<CasesOpenDto>>> GetOpenedCases(DateFilterRequest request)
    {
        var query = request.GetQueryParams();
        var response = await _client.GetAsync($"audit/opened-cases{query}");

        return await response.ReadResponse<List<CasesOpenDto>>() ?? new ErrorResponse<List<CasesOpenDto>>()
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Message = "Failed to retrieve opened cases."
        };
    }

    public async Task<IResponse<List<BalanceDiversificationRecord>>> GetBalanceDiversification(DateFilterRequest request)
    {
        var query = request.GetQueryParams();
        var response = await _client.GetAsync($"audit/balance-diversification{query}");
        return await response.ReadResponse<List<BalanceDiversificationRecord>>() ?? new ErrorResponse<List<BalanceDiversificationRecord>>()
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Message = "Failed to retrieve balance diversification data."
        };
    }

    public async Task<IResponse<List<WithdrawnDto>>> GetWithdrawalsData(DateFilterRequest request)
    {
        var query = request.GetQueryParams();

        var response = await _client.GetAsync($"audit/withdrawals{query}");

        return await response.ReadResponse<List<WithdrawnDto>>() ?? new ErrorResponse<List<WithdrawnDto>>()
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Message = "Failed to retrieve withdrawals data."
        };
    }

    public async Task<IResponse<ItemDto>> GetMaxCostItemForUserAsync(string userId)
    {
        var response = await _client.GetAsync($"audit/max_cost/item/{userId}");

        return await response.ReadResponse<ItemDto>() ?? new ErrorResponse<ItemDto>()
        { StatusCode = HttpStatusCode.InternalServerError };
    }

    public async Task<IResponse<CaseDto>> GetFavouriteCaseForUser(string userId)
    {
        var response = await _client.GetAsync($"audit/favcase/{userId}");

        return await response.ReadResponse<CaseDto>() ?? new ErrorResponse<CaseDto>()
        { StatusCode = HttpStatusCode.InternalServerError };
    }

    public async Task<IResponse<IList>> GetAllLogsAsync(LogDefaultRequest defaultRequest)
    {
        // var queryParams = $"?logtype={request.LogType}";
        // var response = await _client.GetAsync($"logs{queryParams}");
        // var data = await response.Content.ReadAsStringAsync();
        // IList logs = request.LogType switch
        // {
        //     LTYPE.Base => JsonConvert.DeserializeObject<IList<BaseLogDto>>(data),
        //     LTYPE.Case => expr,
        //     LTYPE.Financial => expr,
        //     LTYPE.Item => expr,
        //     LTYPE.User => expr,
        //     _ => throw new ArgumentOutOfRangeException()
        // };
        IList list = null;
        return null;
    }
}