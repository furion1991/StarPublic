using System.Net;
using System.Security.Claims;
using System.Text.Json;
using AuthService.Database.Context;
using AuthService.Database.Models;
using AuthService.DataTransfer;
using AuthService.Services;
using DataTransferLib.Auth;
using DataTransferLib.CommunicationsServices;
using DataTransferLib.DataTransferObjects.Auth;
using DataTransferLib.DataTransferObjects.Common;
using DataTransferLib.DataTransferObjects.Users;
using DtoClassLibrary.DataTransferObjects.Auth;
using DtoClassLibrary.DataTransferObjects.Auth.Telegram;
using DtoClassLibrary.DataTransferObjects.Auth.VkAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace AuthService.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(
    AuthContext authContext,
    JwtTokenService jwtTokenService,
    UserManager<StarDropUser> userManager,
    ILogger<AuthController> logger,
    RabbitMqService rabbitMqService,
    Services.AuthService authService)
    : ControllerBase
{
    private readonly string[] _roles = ["User", "Admin", "Manager"];
    private readonly ILogger<AuthController> _logger = logger;


    [HttpGet("exists/{email}")]
    public async Task<IActionResult> CheckExistingUser(string email)
    {
        var user = await authContext.Users.FirstOrDefaultAsync(u => u.UserName!.ToUpper() == email.ToUpper());
        var result = user != null;

        return new RequestService().GetResponse("Result", result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> FullDeleteUser(string id)
    {
        var userToDelete = await authContext.Users.FirstOrDefaultAsync(u => u.Id == id);

        try
        {
            authContext.Users.Remove(userToDelete);
            await authContext.SaveChangesAsync();
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user != null)
        {
            var result = await userManager.ConfirmEmailAsync(user, request.Token);
            if (result.Succeeded)
            {
                return new RequestService().GetResponse("Email Confirmed", result.Succeeded);
            }
        }

        return new RequestService().HandleError(new InvalidOperationException());
    }

    [HttpGet("resend-email/{email}")]
    public async Task<IActionResult> ResendConfirmationEmail(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return NotFound();
        }

        if (user.EmailConfirmed)
        {
            return StatusCode(405);
        }

        try
        {
            await authService.ResendConfirmationEmail(email);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("tg-subscription-validate")]
    public async Task<IActionResult> ValidateTgSubscription([FromBody] TelegramUserDataDto request,
        [FromQuery] string userId)
    {
        var result = await authService.ValidateTgSubscriptionStatus(request, userId);

        if (result)
        {
            return Ok(new DefaultResponse<bool>()
                { Message = "User subscription status OK", StatusCode = HttpStatusCode.OK, Result = true });
        }

        return BadRequest(new DefaultResponse<bool>()
            { Message = "User not subscribed", StatusCode = HttpStatusCode.BadRequest, Result = false });
    }

    [HttpPost("vk-subscription-validate")]
    public async Task<IActionResult> ValidateVkSubscription([FromBody] VkUseAuthRequest request,
        [FromQuery] string userId)
    {
        var result = await authService.ValidateVkSubscriptionStatus(request, userId);

        if (result)
        {
            return Ok(new DefaultResponse<bool>()
                { Message = "User subscription status OK", StatusCode = HttpStatusCode.OK, Result = true });
        }

        return BadRequest(new DefaultResponse<bool>()
            { Message = "User not subscribed", StatusCode = HttpStatusCode.BadRequest, Result = false });
    }

    [HttpGet("vk-subscription-status")]
    public async Task<IActionResult> CheckVkSubscriptionStatus([FromQuery] string userId)
    {
        var result = await authService.CheckVkSubscriptionStatus(userId);

        if (result)
        {
            return Ok(new DefaultResponse<bool>()
                { Message = "user subscription status OK", StatusCode = HttpStatusCode.OK, Result = true });
        }

        return BadRequest(new DefaultResponse<bool>()
            { Message = "User not subscribed", StatusCode = HttpStatusCode.BadRequest, Result = false });
    }

    [HttpGet("tg-subscription-status")]
    public async Task<IActionResult> CheckTgSubscriptionStatus([FromQuery] string userId)
    {
        var result = await authService.CheckTgSubscriptionStatus(userId);

        if (result)
        {
            return Ok(new DefaultResponse<bool>()
                { Message = "user subscription status OK", StatusCode = HttpStatusCode.OK, Result = true });
        }

        return BadRequest(new DefaultResponse<bool>()
            { Message = "User not subscribed", StatusCode = HttpStatusCode.BadRequest, Result = false });
    }

    [HttpPost("vk")]
    public async Task<IActionResult> VkAuth([FromBody] VkUseAuthRequest request)
    {
        var result = await authService.LoginOrRegisterWithVk(request);
        return new RequestService().GetResponse("Tokens", result);
    }

    [HttpGet("vk/data")]
    public async Task<IActionResult> GetVkUserData([FromQuery] string userId)
    {
        VkUserInfo user;
        var userFromDb = await authContext.VkUserData
            .Include(e => e.MainUser)
            .FirstOrDefaultAsync(e => e.MainUserId == userId);
        if (userFromDb == null)
        {
            return NotFound();
        }

        user = new VkUserInfo()
        {
            FirstName = userFromDb.FirstName,
            Id = userFromDb.Id,
            PhotoUrl = userFromDb.PhotoUrl,
            Email = userFromDb.MainUser.Email
        };

        return new RequestService().GetResponse("User vk", user);
    }

    [HttpPost("telegram")]
    public async Task<IActionResult> TelegramAuth([FromBody] TelegramUserDataDto dataDto)
    {
        var result = await authService.LoginOrRegisterTelegram(dataDto);

        return new RequestService().GetResponse("Tokens", result);
    }

    [HttpPost("register/email")]
    public async Task<IActionResult> RegisterEmail([FromBody] RegistrationEmailModel registrationEmailModel)
    {
        try
        {
            var result = await authService.RegisterWithEmail(registrationEmailModel);

            return new RequestService().GetResponse("Registration result", result);
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized();
            }

            if (!user.EmailConfirmed)
            {
                return StatusCode(403, "Email is not confirmed");
            }

            if (!authContext.ActiveUsers.Any(u => u.UserId == user.Id))
            {
                await authContext.ActiveUsers.AddAsync(new ActiveUser()
                    { UserId = user.Id, LoginTime = DateTime.UtcNow });
                await authContext.SaveChangesAsync();

                await rabbitMqService.SendLog("user logged in", await authContext.ActiveUsers.CountAsync(),
                    LTYPE.User);
            }

            var roles = await userManager.GetRolesAsync(user);

            var token = jwtTokenService.GenerateJwtToken(user, roles);

            var refreshToken = await jwtTokenService.GenerateNewRefreshToken(user);

            var tokenResponse = new TokensResponse()
            {
                RefreshToken = refreshToken.Token,
                Token = token
            };

            return new RequestService().GetResponse("Login successful", tokenResponse);
        }
        catch (Exception e)
        {
            return new RequestService().HandleError(e);
        }
    }

    [HttpPut("role/update")]
    public async Task<IActionResult> UpdateUserRole([FromBody] UserRoleUpdateRequest userRoleUpdateRequest)
    {
        var user = await userManager.FindByIdAsync(userRoleUpdateRequest.UserId);
        if (user == null)
        {
            return NotFound();
        }

        if (userRoleUpdateRequest.RoleName != "User" && userRoleUpdateRequest.RoleName !=
            "Manager" && userRoleUpdateRequest.RoleName != "Admin")
        {
            return BadRequest();
        }

        foreach (var role in _roles)
        {
            if (await userManager.IsInRoleAsync(user, role))
            {
                await userManager.RemoveFromRoleAsync(user, role);
            }
        }

        var result = await userManager.AddToRoleAsync(user, userRoleUpdateRequest.RoleName);
        if (result.Succeeded)
        {
            return new RequestService().GetResponse($"User {user.UserName} role updated",
                $"New role: {userRoleUpdateRequest.RoleName}");
        }

        return new RequestService().HandleError(result);
    }

    [HttpGet("active_users")]
    public async Task<IActionResult> GetActiveUsers()
    {
        var activeUsers = await authContext.ActiveUsers.ToListAsync();
        return Ok(activeUsers.Count);
    }


    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutModel dto)
    {
        var result = await jwtTokenService.RevokeRefreshToken(dto.RefreshToken);
        if (result.IsSuccessful)
        {
            var userId = await jwtTokenService.GetUserIdFromRefreshToken(dto.RefreshToken);
            var user = await authContext.ActiveUsers.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user != null)
            {
                authContext.ActiveUsers.Remove(user);
                await authContext.SaveChangesAsync();
                await rabbitMqService.SendLog("user logged out", await authContext.ActiveUsers.CountAsync(),
                    LTYPE.User);
            }

            return new RequestService().GetResponse("Logged out", result);
        }

        return new RequestService().HandleError(result);
    }


    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromQuery] string email)
    {
        try
        {
            var result = await authService.ResetPassword(email);
            return StatusCode((int)result.StatusCode, result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAccessToken([FromBody] RefreshAccessTokenRequest accessTokenRequest)
    {
        var result = await jwtTokenService.RefreshAccessToken(accessTokenRequest.RefreshToken);
        if (result is null)
        {
            return new RequestService().HandleError(new NullReferenceException("no tokens found"));
        }

        return new RequestService().GetResponse("New tokens", result);
    }

    [HttpDelete("delete/user/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is not null)
        {
            await userManager.DeleteAsync(user);
            return Ok();
        }

        return NotFound();
    }

    [HttpGet("validate/{token}")]
    public async Task<IActionResult> CheckAccessToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized();
        }

        var claims = jwtTokenService.ValidateAccessToken(token);
        if (claims is not null)
        {
            return new RequestService().GetResponse("Token valid", true);
        }

        return new RequestService().GetResponse("Token invalid", false);
    }
}