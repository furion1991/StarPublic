using System.Net;
using System.Text;
using DataTransferLib.DataTransferObjects.Auth;
using DataTransferLib.DataTransferObjects.Common;
using DataTransferLib.DataTransferObjects.Users;
using DtoClassLibrary.DataTransferObjects.CasesItems;
using DtoClassLibrary.DataTransferObjects.CasesItems.Upgrade;
using DtoClassLibrary.DataTransferObjects.Common;
using DtoClassLibrary.DataTransferObjects.Users;
using DtoClassLibrary.DataTransferObjects.Users.Admin;
using DtoClassLibrary.DataTransferObjects.Users.Inventory;
using Newtonsoft.Json;
using InventoryRecordDto = DtoClassLibrary.DataTransferObjects.CasesItems.InventoryRecordDto;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DataTransferLib.CommunicationsServices;

public class UsersCommService(IHttpClientFactory clientFactory, FinancialCommService financialCommService)
{
    private readonly HttpClient _client = clientFactory.CreateClient(CommConfigure.USERS_CLIENT_NAME);


    public async Task<IResponse<List<UserDto>>> GetAllUsers()
    {
        var response = await _client.GetAsync("users/users-all");
        return await response.ReadResponse<List<UserDto>>() ?? new DefaultResponse<List<UserDto>?>()
        { Message = "no users found", Result = new List<UserDto>(), StatusCode = HttpStatusCode.NotFound };
    }

    public async Task<IResponse<UserDto>?> CreateUser(CreateUpdateUserRequest createUpdateUserRequest)
    {
        var content = new StringContent(JsonSerializer.Serialize(createUpdateUserRequest), Encoding.UTF8,
            "application/json");
        var response =
            await _client.PostAsync("users/create", content);
        if (response.IsSuccessStatusCode)
        {
            return await response.ReadResponse<UserDto>();
        }
        else
        {
            return new ErrorResponse<UserDto>()
            { StatusCode = response.StatusCode, Result = new UserDto() { Id = string.Empty } };
        }
    }

    public async Task<bool> SetChanceBoostForPlayer(ChanceBoostRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync("users/chanceboost", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<IResponse<List<UserDtoForAdminPanel>>> GetAdminUserData(DefaultRequest request)
    {
        var query = request.GetQueryParams();
        var response = await _client.GetAsync($"users/admin/users-data{query}");
        if (!response.IsSuccessStatusCode)
            return new ErrorResponse<List<UserDtoForAdminPanel>>()
            {
                StatusCode = response.StatusCode,
                Result = new List<UserDtoForAdminPanel>(),
                Message = "Failed to retrieve user data."
            };
        var result = await response.ReadResponse<List<UserDtoForAdminPanel>>();
        var userIds = result.Result.Select(e => e.Id).ToList();
        var financials = await financialCommService.GetAdminFinDataForUsers(userIds);

        foreach (var finDataDto in financials.Result)
        {
            var user = result.Result.FirstOrDefault(u => u.Id == finDataDto.UserId);
            if (user == null) continue;
            user.Deposited = finDataDto.AllDepositAmount;
            user.Profit = finDataDto.Profit;
            user.Balance = finDataDto.Balance;
        }

        return new DefaultResponse<List<UserDtoForAdminPanel>>()
        {
            Message = "",
            Result = result.Result,
            StatusCode = HttpStatusCode.OK
        };

    }


    public async Task<IResponse<List<string>>> GetAvailableRoles()
    {
        var response = await _client.GetAsync("users/available-roles");
        return await response.ReadResponse<List<string>>() ?? new ErrorResponse<List<string>>()
        {
            StatusCode = response.StatusCode,
            Result = new List<string>(),
            Message = "Failed to retrieve available roles."
        };
    }

    public async Task<IResponse<SingleUserForAdminDto>> GetSingleUserDataForAdmin(string userId)
    {
        var response = await _client.GetAsync($"users/admin/single-user-data/{userId}");

        return await response.ReadResponse<SingleUserForAdminDto>() ?? new ErrorResponse<SingleUserForAdminDto>()
        {
            StatusCode = response.StatusCode,
            Result = new SingleUserForAdminDto(),
            Message = "Failed to retrieve user data."
        };
    }

    public async Task<IResponse<ManagerUserDto>> GetManagerUserData(string id)
    {
        var response = await _client.GetAsync($"users/manager/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.ReadResponse<ManagerUserDto>();
        }
        else
        {
            return new ErrorResponse<ManagerUserDto>()
            { StatusCode = response.StatusCode, Result = new ManagerUserDto() };
        }
    }



    public async Task<bool> IsUserSubscribedToPrizeDraw(string userId)
    {
        var response = await _client.GetAsync($"prize-draw/is-subscribed?userId={userId}");
        var result = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
        return result;
    }

    public async Task<bool> SubscribeToPrizeDraw(string userId)
    {
        var response = await _client.GetAsync($"prize-draw/subscribe?userId={userId}");
        return response.IsSuccessStatusCode;
    }


    public async Task<IResponse<bool>> AddSmallBonus(string userId)
    {
        var response = await _client.GetAsync($"small-bonus/add?userId={userId}");
        return await response.ReadResponse<bool>() ?? new DefaultResponse<bool>()
        { Message = "Bonus not added", Result = false, StatusCode = HttpStatusCode.InternalServerError };
    }

    public async Task<IResponse<bool>> UpdateStatsAfterContract(UpdateContractsStatisticsRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync("users/stats/update/contract", content);
        return await response.ReadResponse<bool>() ?? new ErrorResponse<bool>()
        { StatusCode = HttpStatusCode.InternalServerError };
    }


    public async Task<IResponse<bool>> UpdateStatsAfterUpgrade(UpdateStatsAfterUpgradeRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync("users/stats/update/upgrade", content);
        return await response.ReadResponse<bool>() ?? new ErrorResponse<bool>()
        { StatusCode = HttpStatusCode.InternalServerError };
    }

    public async Task<IResponse<bool>> UpdateStats(UpdateStatisticsRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync("users/stats/update", content);

        return await response.ReadResponse<bool>() ?? new ErrorResponse<bool>()
        { StatusCode = HttpStatusCode.InternalServerError };
    }

    public async Task<IResponse<InternalUserDto>?> GetInternalUserData(string id)
    {
        var response = await _client.GetAsync($"users/internal/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.ReadResponse<InternalUserDto>();
        }
        else
        {
            return new ErrorResponse<InternalUserDto>()
            { StatusCode = response.StatusCode, Result = new InternalUserDto() { PublicData = new() } };
        }
    }

    public async Task<IResponse<UserDto>?> GetUserData(string data, bool isEmail = false)
    {
        HttpResponseMessage response;
        if (!isEmail)
        {
            response = await _client.GetAsync($"users/{data}");
            return await response.ReadResponse<UserDto>();
        }

        response = await _client.GetAsync($"users/mail/{data}");
        return await response.ReadResponse<UserDto>();
    }

    public async Task<IResponse<UserDto>?> UpdateUser(string id, CreateUpdateUserRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response =
            await _client.PutAsync($"users/{id}", content);

        return await response.ReadResponse<UserDto>();
    }


    public async Task<IResponse<UserRoleDto>?> UpdateUserRole(UserRoleUpdateRequest userRoleUpdateRequest)
    {
        var content = new StringContent(JsonSerializer.Serialize(userRoleUpdateRequest), Encoding.UTF8,
            "application/json");

        var responseFromUserService = await _client.PutAsync($"users/role/update", content);
        return await responseFromUserService.ReadResponse<UserRoleDto>();
    }

    public async Task<IResponse<BlockStatusDto>?> BlockUser(BlockUnblockRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"users/block", content);

        return await response.ReadResponse<BlockStatusDto>();
    }

    public async Task<IResponse<BlockStatusDto>?> Unblock(BlockUnblockRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"users/unblock", content);

        return await response.ReadResponse<BlockStatusDto>();
    }

    public async Task<bool> FullDeleteUser(string id)
    {
        var response = await _client.DeleteAsync($"users/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<IResponse<UserDto>?> DeleteUser(string id, string performedById)
    {
        var response = await _client.DeleteAsync($"users/{id}/{performedById}");
        return await response.ReadResponse<UserDto>();
    }

    public async Task<IResponse<GameResultDto>?> AddItemToInventory(AddRemoveItemRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync($"users/additem", content);
        return await response.ReadResponse<GameResultDto>() ?? new ErrorResponse<GameResultDto>()
        { StatusCode = HttpStatusCode.InternalServerError };
    }

    public async Task<IResponse<GameResultDto>?> AddMultipleItemsToInventory(AddRemoveMultipleItemsRequest request)
    {
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("users/add/items", content);
        return await response.ReadResponse<GameResultDto>() ?? new ErrorResponse<GameResultDto>()
        { StatusCode = HttpStatusCode.InternalServerError };
    }

    public async Task<HttpStatusCode> SetItemsState(ChangeStateRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"inventory/set_item_state", content);
        return response.StatusCode;
    }

    public async Task<HttpStatusCode> SellItem(string itemId)
    {
        var response = await _client.DeleteAsync($"users/sell-item?itemId={itemId}");
        return response.StatusCode;
    }

    public async Task<IResponse<bool>?> RemoveItemFromInvenory(AddRemoveItemRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync($"users/removeitem", content);
        return await response.ReadResponse<bool>();
    }

    public async Task<IResponse<bool>?> UncheckItemFromInvenory(AddRemoveItemRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PatchAsync($"api/inventorymanagement/uncheckitem", content);
        return await response.ReadResponse<bool>();
    }

    public async Task<IResponse<AddRemoveItemRequest>?> UpgradeItem(UpgradeItemRecordRequest request)
    {
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await _client.PatchAsync($"api/inventorymanagement/upgradeitem", content);
        return await response.ReadResponse<AddRemoveItemRequest>();
    }
}