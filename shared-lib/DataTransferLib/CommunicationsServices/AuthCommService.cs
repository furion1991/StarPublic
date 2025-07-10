using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using AuthService.DataTransfer;
using DataTransferLib.Auth;
using DataTransferLib.DataTransferObjects.Auth;
using DataTransferLib.DataTransferObjects.Common;
using DataTransferLib.DataTransferObjects.Financial;
using DataTransferLib.DataTransferObjects.Users;
using DtoClassLibrary.DataTransferObjects.Auth;
using DtoClassLibrary.DataTransferObjects.Auth.Telegram;
using DtoClassLibrary.DataTransferObjects.Auth.VkAuth;
using DtoClassLibrary.DataTransferObjects.Common;
using DtoClassLibrary.DataTransferObjects.Users;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DataTransferLib.CommunicationsServices;

public class AuthCommService
{
    private readonly HttpClient _client;
    private readonly UsersCommService _usersService;
    private readonly FinancialCommService _financialService;

    public AuthCommService(IHttpClientFactory factory, UsersCommService usersService,
        FinancialCommService financialService)
    {
        _client = factory.CreateClient(CommConfigure.AUTH_CLIENT_NAME);
        _usersService = usersService;
        _financialService = financialService;
    }


    public async Task<IResponse<bool>> ValidateTgSubscription(TelegramUserDataDto tgUserDataDto, string userId)
    {
        var content = new StringContent(JsonConvert.SerializeObject(tgUserDataDto), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync($"auth/tg-subscription-validate?userId={userId}", content);

        return await response.ReadResponse<bool>() ?? new DefaultResponse<bool>()
            { Message = "subscription validation failed", StatusCode = HttpStatusCode.BadRequest, Result = false };
    }

    public async Task<IResponse<bool>> ValidateVkSubscription(VkUseAuthRequest request, string userId)
    {
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync($"auth/vk-subscription-validate?userId={userId}", content);

        return await response.ReadResponse<bool>() ?? new DefaultResponse<bool>()
            { Message = "subscription validation failed", StatusCode = HttpStatusCode.BadRequest, Result = false };
    }

    public async Task<IResponse<bool>> CheckTgSubscriptionStatus(string userId)
    {
        var response = await _client.GetAsync($"auth/tg-subscription-status?userId={userId}");
        return await response.ReadResponse<bool>() ?? new DefaultResponse<bool>()
            { Message = "error", StatusCode = HttpStatusCode.InternalServerError, Result = false };
    }

    public async Task<IResponse<bool>> CheckVkSubscriptionStatus(string userId)
    {
        var response = await _client.GetAsync($"auth/vk-subscription-status?userId={userId}");
        return await response.ReadResponse<bool>() ?? new DefaultResponse<bool>()
            { Message = "error", StatusCode = HttpStatusCode.InternalServerError, Result = false };
    }

    public async Task<IResponse<bool>?> CheckIfUserExists(string email)
    {
        var response = await _client.GetAsync($"auth/exists/{email}");
        return await response.ReadResponse<bool>();
    }

    public async Task<IResponse<bool>?> ValidateToken(string token)
    {
        var response = await _client.GetAsync($"auth/validate/{token}");
        return await response.ReadResponse<bool>();
    }

    public async Task<int> GetActiveUsers()
    {
        var response = await _client.GetAsync("auth/active_users");
        return await response.Content.ReadFromJsonAsync<int>();
    }

    public async Task<IResponse<bool>> ConfirmEmail(ConfirmEmailRequest request)
    {
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("auth/confirm-email", content);
        return await response.ReadResponse<bool>() ?? new ErrorResponse<bool>()
            { Result = false, StatusCode = HttpStatusCode.InternalServerError };
    }

    public async Task<IResponse<UserDto>?> Register(RegistrationEmailModel registrationEmailModel)
    {
        var content = new StringContent(JsonSerializer.Serialize(registrationEmailModel), Encoding.UTF8,
            "application/json");

        var response = await _client.PostAsync("auth/register/email", content);

        if (response.IsSuccessStatusCode)
        {
            var userDto = await response.ReadResponse<UserDto>();

            var userId = userDto.Result.Id;

            var createBusinessUserRequest = new CreateUpdateUserRequest()
            {
                UserName = registrationEmailModel.Email,
                Email = registrationEmailModel.Email,
                Phone = "",
                Id = userId
            };

            var result = await _usersService.CreateUser(createBusinessUserRequest);
            if (result is not null && result.StatusCode == HttpStatusCode.OK)
            {
                var financeCreationResult = await _financialService.CreateFinancialDataForUser(new FinancialDataParams()
                    { CurrentBalance = 0, UserId = userId });
                if (financeCreationResult.StatusCode == HttpStatusCode.OK)
                {
                    return userDto;
                }
            }
        }

        return new ErrorResponse<UserDto>()
        {
            Result = null,
            ErrorDetails = "Error in registration",
            Message = "ERROR",
            StatusCode = HttpStatusCode.InternalServerError
        };
    }


    public async Task<IResponse<string>?> UpdateUserRole(UserRoleUpdateRequest userRoleUpdateRequest)
    {
        var content = new StringContent(JsonSerializer.Serialize(userRoleUpdateRequest), Encoding.UTF8,
            "application/json");
        var response = await _client.PutAsync($"auth/role/update/", content);

        return await response.ReadResponse<string>();
    }

    public async Task<bool> FullDeleteUser(string id)
    {
        var response = await _client.DeleteAsync($"auth/delete/user/{id}");
        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        return false;
    }

    public async Task<IResponse<bool>> ResendConfirmationEmail(string email)
    {
        var response = await _client.GetAsync($"auth/resend-email/{email}");
        return response.IsSuccessStatusCode
            ? new DefaultResponse<bool>() { Message = "email sent", StatusCode = response.StatusCode, Result = true }
            : new ErrorResponse<bool>() { StatusCode = response.StatusCode, Result = false };
    }

    public async Task<IResponse<TokensResponse>?> Login(LoginModel loginModel)
    {
        var content = new StringContent(JsonSerializer.Serialize(loginModel), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("auth/login", content);

        if (response.IsSuccessStatusCode)
        {
            return await response.ReadResponse<TokensResponse>();
        }
        else
        {
            return new ErrorResponse<TokensResponse>()
            {
                StatusCode = response.StatusCode,
                Result = null,
                Message = $"Error: {await response.Content.ReadAsStringAsync()}"
            };
        }
    }

    public async Task<IResponse<ActionResult>?> Logout(LogoutModel logoutModel)
    {
        var contentRes = new StringContent(JsonSerializer.Serialize(logoutModel), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("auth/logout", contentRes);

        return await response.ReadResponse<ActionResult>();
    }

    public async Task<IResponse<TokensResponse>?> TelegramAuth(TelegramUserDataDto dataDto)
    {
        var content = new StringContent(JsonConvert.SerializeObject(dataDto), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("auth/telegram", content);

        var result = await response.ReadResponse<TokensResponse>();

        if (response.IsSuccessStatusCode)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(result.Result.Token);

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            var createBusinessUserRequest = new CreateUpdateUserRequest()
            {
                UserName = dataDto.Username ?? dataDto.FirstName ?? dataDto.Id.ToString(),
                Email = "",
                Phone = "",
                Id = userId,
                ProfileImageUrl = dataDto.PhotoUrl ?? ""
            };

            var resultBusiness = await _usersService.CreateUser(createBusinessUserRequest);
            if (resultBusiness is not null && resultBusiness.StatusCode == HttpStatusCode.OK)
            {
                var financeCreationResult = await _financialService.CreateFinancialDataForUser(new FinancialDataParams()
                    { CurrentBalance = 0, UserId = userId });
                if (financeCreationResult.StatusCode == HttpStatusCode.OK)
                {
                    return result;
                }
            }
        }
        else
        {
            return null;
        }

        return result;
    }

    public async Task<IResponse<TokensResponse>> VkAuth(VkUseAuthRequest request)
    {
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("auth/vk", content);

        var result = await response.ReadResponse<TokensResponse>();
        if (result == null)
        {
            return new ErrorResponse<TokensResponse>() { StatusCode = HttpStatusCode.InternalServerError };
        }

        if (result.StatusCode == HttpStatusCode.OK)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(result.Result.Token);

            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            var vkUser = await GetVkUserInfo(userId);

            var createBusinessUserRequest = new CreateUpdateUserRequest()
            {
                UserName = vkUser.Result?.Email ?? vkUser.Result.Id.ToString(),
                Email = "",
                Phone = "",
                Id = userId,
                ProfileImageUrl = vkUser.Result.PhotoUrl ?? ""
            };

            var resultBusiness = await _usersService.CreateUser(createBusinessUserRequest);
            if (resultBusiness is not null && resultBusiness.StatusCode == HttpStatusCode.OK)
            {
                var financeCreationResult = await _financialService.CreateFinancialDataForUser(new FinancialDataParams()
                    { CurrentBalance = 0, UserId = userId });
                if (financeCreationResult.StatusCode == HttpStatusCode.OK)
                {
                    return result;
                }
            }
        }

        return result;
    }

    public async Task<IResponse<VkUserInfo>> GetVkUserInfo(string starDropUserId)
    {
        var response = await _client.GetAsync($"auth/vk/data?userId={starDropUserId}");

        return await response.ReadResponse<VkUserInfo>() ?? new ErrorResponse<VkUserInfo>()
            { StatusCode = HttpStatusCode.InternalServerError };
    }

    public async Task<IResponse<bool>> ResetPassword(string email)
    {
        var response = await _client.PostAsync($"auth/reset-password?email={email}", null);

        var result = await response.ReadResponse<bool>();

        return result ?? new ErrorResponse<bool>()
            { StatusCode = response.StatusCode, Message = await response.Content.ReadAsStringAsync() };
    }

    public async Task<IResponse<TokensResponse>?> RefreshAccessToken(RefreshAccessTokenRequest refreshToken)
    {
        var checkContent =
            new StringContent(JsonConvert.SerializeObject(refreshToken), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("auth/refresh", checkContent);
        return await response.ReadResponse<TokensResponse>();
    }

    private string? GetUserIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub");

        return userId?.Value;
    }
}