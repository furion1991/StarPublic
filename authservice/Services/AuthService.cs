using System.Net;
using AuthService.Controllers;
using AuthService.Database.Context;
using AuthService.Database.Models;
using DataTransferLib.CommunicationsServices;
using DataTransferLib.DataTransferObjects.Auth;
using DataTransferLib.DataTransferObjects.Common;
using DataTransferLib.DataTransferObjects.Financial.Models;
using DataTransferLib.DataTransferObjects.Users;
using DtoClassLibrary.DataTransferObjects.Auth;
using DtoClassLibrary.DataTransferObjects.Auth.Telegram;
using DtoClassLibrary.DataTransferObjects.Auth.VkAuth;
using DtoClassLibrary.DataTransferObjects.Common;
using DtoClassLibrary.DataTransferObjects.Financial.Models;
using DtoClassLibrary.DataTransferObjects.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VkNet;
using VkNet.Model;

namespace AuthService.Services
{
    public class AuthService(
        AuthContext authContext,
        JwtTokenService jwtTokenService,
        UserManager<StarDropUser> userManager,
        ILogger<AuthService> logger,
        VkService vkService,
        RabbitMqService rabbitMqService,
        TelegramSubscriptionChecker telegramSubscriptionChecker,
        FinancialCommService financialCommService
    )
    {
        public async Task ResendConfirmationEmail(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return;
            }

            var message = await GenerateConfirmationEmailMessage(user);

            await rabbitMqService.SendEmailConfirmationMessage(message);
        }

        public async Task<bool> CheckVkSubscriptionStatus(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            return user.VkSubscribed;
        }

        public async Task<bool> CheckTgSubscriptionStatus(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            return user.TgSubscribed;
        }

        public async Task<bool> ValidateTgSubscriptionStatus(TelegramUserDataDto tgDate, string userId)
        {
            var user = await authContext.Users
                .Include(e => e.TelegramUserData)
                .FirstOrDefaultAsync(e => e.Id == userId);
            if (user == null)
            {
                return false;
            }

            var result = await telegramSubscriptionChecker.CheckUserSubscribedAsync(tgDate.Id, "stardropgo");

            if (result)
            {
                if (!authContext.TelegramUsers.Any(e => e.Id == tgDate.Id))
                {
                    var tgUser = new TelegramUserData()
                    {
                        Id = tgDate.Id,
                        LastName = tgDate.LastName,
                        FirstName = tgDate.FirstName,
                        PhotoUrl = tgDate.PhotoUrl,
                        Username = tgDate.Username,
                        StarDropUser = user,
                        MainUserId = user.Id,
                    };

                    await authContext.TelegramUsers.AddAsync(tgUser);
                    user.TelegramUserData = tgUser;
                    user.TelegramUserDataId = tgUser.Id;
                }

                user.TgSubscribed = true;

                authContext.Users.Update(user);
                await authContext.SaveChangesAsync();

                await financialCommService.MakeTransaction(new TransactionParams()
                {
                    UserId = user.Id,
                    Amount = 20,
                    Type = TTYPE.Bonus,
                    BalanceAfter = 0,
                    BalanceBefore = 0,
                    PaymentType = PTYPE.Bank,
                    FinancialDataId = ""
                });
            }

            return user.TgSubscribed;
        }


        public async Task<bool> ValidateVkSubscriptionStatus(VkUseAuthRequest vkUserData, string userId)
        {
            var exchangeResult = await vkService.ExchangeCodeForVkToken(
                vkUserData.Code,
                vkUserData.CodeVerifier,
                vkUserData.DeviceId,
                vkUserData.State,
                "bonuses");

            var subscriptionStatus = await vkService.CheckGroupSubscriptionAsync(
                exchangeResult.AccessToken,
                exchangeResult.UserId,
                "stardrop");

            if (!subscriptionStatus) return false;

            var user = await userManager.Users.Include(e => e.VkUserData)
                .FirstOrDefaultAsync(e => e.Id == userId);

            if (user == null) return false;

            var data = await vkService.GetVkUserInfo(exchangeResult.AccessToken, exchangeResult.UserId);

            if (user.VkUserData == null || user.VkUserData.Id == 0)
            {
                var existing = await authContext.VkUserData.FirstOrDefaultAsync(e => e.Id == data.Id);

                if (existing == null)
                {
                    var vkData = new VkUserData
                    {
                        Id = data.Id,
                        PhotoUrl = data.PhotoUrl,
                        MainUserId = user.Id,
                        MainUser = user,
                        FirstName = data.FirstName
                    };

                    user.VkUserData = vkData;
                    user.VkUserDataId = vkData.Id;

                    await authContext.VkUserData.AddAsync(vkData);
                }
                else
                {
                    var vkMainUser = await authContext.Users.FirstOrDefaultAsync(e => e.Id == existing.MainUserId);
                    if (vkMainUser != null)
                    {
                        vkMainUser.VkSubscribed = true;
                        authContext.Users.Update(vkMainUser);
                    }

                    user.VkSubscribed = true;
                }
            }

            if (!user.VkSubscribed)
            {
                user.VkSubscribed = true;

                await financialCommService.MakeTransaction(new TransactionParams
                {
                    UserId = user.Id,
                    Amount = 20,
                    Type = TTYPE.Bonus,
                    BalanceAfter = 0,
                    BalanceBefore = 0,
                    PaymentType = PTYPE.Bank,
                    FinancialDataId = ""
                });
            }

            await userManager.UpdateAsync(user);
            await authContext.SaveChangesAsync();

            return true;
        }


        public async Task<TokensResponse?> LoginOrRegisterTelegram(TelegramUserDataDto tgUser)
        {
            var user = await userManager.Users
                .Include(e => e.TelegramUserData)
                .FirstOrDefaultAsync(u => u.TelegramUserDataId == tgUser.Id);
            if (user == null)
            {
                user = new StarDropUser()
                {
                    UserName = string.IsNullOrEmpty(tgUser.Username) ? tgUser.FirstName : tgUser.Username,
                    TelegramUserDataId = tgUser.Id,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    var role = "User";

                    await userManager.AddToRoleAsync(user, role);
                    await userManager.UpdateAsync(user);

                    var tgData = new TelegramUserData()
                    {
                        FirstName = tgUser.FirstName,
                        Id = tgUser.Id,
                        LastName = tgUser.LastName,
                        PhotoUrl = tgUser.PhotoUrl,
                        Username = tgUser.Username,
                        StarDropUser = user,
                        MainUserId = user.Id
                    };
                    await authContext.TelegramUsers.AddAsync(tgData);
                    await authContext.SaveChangesAsync();

                    var roles = await userManager.GetRolesAsync(user);

                    var token = jwtTokenService.GenerateJwtToken(user, roles);

                    var refreshToken = await jwtTokenService.GenerateNewRefreshToken(user);

                    var response = new TokensResponse()
                    {
                        RefreshToken = refreshToken.Token,
                        Token = token
                    };

                    return response;
                }
                else
                {
                    return new TokensResponse()
                    {
                        RefreshToken = string.Empty,
                        Token = string.Empty
                    };
                }
            }
            else
            {
                var roles = await userManager.GetRolesAsync(user);
                var token = jwtTokenService.GenerateJwtToken(user, roles);
                var refreshToken = await jwtTokenService.GenerateNewRefreshToken(user);
                var response = new TokensResponse()
                {
                    Token = token,
                    RefreshToken = refreshToken.Token,
                };
                return response;
            }
        }


        public async Task<TokensResponse> LoginOrRegisterWithVk(VkUseAuthRequest request)
        {
            logger.LogInformation("📥 VkAuthRequest: " + JsonConvert.SerializeObject(request));

            var vkResponse = await vkService.ExchangeCodeForVkToken(request.Code, request.CodeVerifier,
                request.DeviceId, request.State);
            logger.LogInformation("🔐 VkTokenResponse: " + JsonConvert.SerializeObject(vkResponse));

            var user = await authContext.VkUserData
                .Include(e => e.MainUser)
                .FirstOrDefaultAsync(e => e.Id == vkResponse.UserId);
            logger.LogInformation(user == null
                ? "🆕 VK user not found — proceeding to registration"
                : $"👤 VK user exists with ID {user.Id}");

            if (user == null)
            {
                var vkUserData = await vkService.GetVkUserInfo(vkResponse.AccessToken, vkResponse.UserId);
                logger.LogInformation("🧾 VK user info: " + JsonConvert.SerializeObject(vkUserData));

                var starDropUser = new StarDropUser()
                {
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    Email = vkResponse.Email,
                    UserName = string.IsNullOrEmpty(vkResponse.Email) ? vkResponse.UserId.ToString() : vkResponse.Email,
                };

                var result = await userManager.CreateAsync(starDropUser);
                logger.LogInformation("👷 CreateAsync result: " + JsonConvert.SerializeObject(result));

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(starDropUser, "User");
                    await userManager.UpdateAsync(starDropUser);

                    var vkDbData = new VkUserData()
                    {
                        FirstName = vkUserData.FirstName,
                        Id = vkResponse.UserId,
                        PhotoUrl = vkUserData.PhotoUrl,
                        VkRefReshToken = vkResponse.RefreshToken,
                        MainUser = starDropUser,
                        MainUserId = starDropUser.Id
                    };


                    await authContext.VkUserData.AddAsync(vkDbData);

                    starDropUser.VkUserData = vkDbData;
                    starDropUser.VkUserDataId = vkDbData.Id;

                    await userManager.UpdateAsync(starDropUser);

                    await authContext.SaveChangesAsync();

                    var roles = await userManager.GetRolesAsync(starDropUser);
                    logger.LogInformation("🧾 User roles (new): " + string.Join(", ", roles));

                    var token = jwtTokenService.GenerateJwtToken(starDropUser, roles);
                    var refreshToken = await jwtTokenService.GenerateNewRefreshToken(starDropUser);

                    logger.LogInformation("🪙 Tokens (new): " +
                                          JsonConvert.SerializeObject(new
                                          { token, refreshToken = refreshToken.Token }));

                    return new TokensResponse()
                    {
                        RefreshToken = refreshToken.Token,
                        Token = token
                    };
                }
            }
            else
            {
                var starDropUser = await authContext.Users.Include(e => e.VkUserData)
                    .FirstOrDefaultAsync(e => e.VkUserDataId == user.Id);

                logger.LogInformation(starDropUser == null
                    ? "⚠️ User from Users table not found"
                    : $"✅ Found user: {starDropUser.UserName} (ID: {starDropUser.Id})");

                if (starDropUser == null)
                    return new TokensResponse();

                var roles = await userManager.GetRolesAsync(starDropUser);
                logger.LogInformation("🧾 User roles (existing): " + string.Join(", ", roles));

                var token = jwtTokenService.GenerateJwtToken(starDropUser, roles);
                var refreshToken = await jwtTokenService.GenerateNewRefreshToken(starDropUser);

                logger.LogInformation("🪙 Tokens (existing): " +
                                      JsonConvert.SerializeObject(new { token, refreshToken = refreshToken?.Token }));

                return new TokensResponse()
                {
                    Token = token,
                    RefreshToken = refreshToken?.Token,
                };
            }

            logger.LogWarning("⚠️ Unexpected fallback — returning empty TokensResponse");
            return new TokensResponse();
        }


        public async Task<IResponse<bool>> ResetPassword(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new ErrorResponse<bool>()
                { Result = false, StatusCode = HttpStatusCode.NotFound, Message = "no user with this email" };
            }

            if (user.LastTimePasswordChanged > DateTime.UtcNow - TimeSpan.FromMinutes(15))
            {
                return new ErrorResponse<bool>()
                {
                    Result = false,
                    Message = "Not available reset password yet",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var newPassword = PasswordGenerator.GeneratePassword(8);

            try
            {
                var result = await userManager.ResetPasswordAsync(user, token, newPassword);
                if (result.Succeeded)
                {
                    user.LastTimePasswordChanged = DateTime.UtcNow;
                    var resetMessage = new ResetPasswordEmailMessage()
                    {
                        NewPassword = newPassword,
                        To = user.Email
                    };

                    authContext.Users.Update(user);

                    await authContext.SaveChangesAsync();
                    await rabbitMqService.SendResetPasswordMessage(resetMessage);
                    return new DefaultResponse<bool>()
                    { Message = "Password Reset complete", Result = true, StatusCode = HttpStatusCode.OK };
                }

                throw new InvalidOperationException();
            }
            catch (Exception e)
            {
                return new ErrorResponse<bool>()
                { Message = e.Message, Result = false, StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        public async Task<UserDto?> RegisterWithEmail(RegistrationEmailModel emailModel)
        {
            try
            {
                var user = new StarDropUser()
                {
                    Email = emailModel.Email,
                    UserName = emailModel.Email
                };

                var result = await userManager.CreateAsync(user, emailModel.Password);
                if (result.Succeeded)
                {
                    var role = string.IsNullOrEmpty(emailModel.Role) ? "User" : emailModel.Role;
                    await userManager.AddToRoleAsync(user, role);
                    await userManager.UpdateAsync(user);

                    var message = await GenerateConfirmationEmailMessage(user);

                    await rabbitMqService.SendEmailConfirmationMessage(message);
                    var userToReturn = new UserDto()
                    {
                        Id = user.Id
                    };
                    return userToReturn;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return null;
            }
        }


        private async Task<ConfirmationEmailMessage> GenerateConfirmationEmailMessage(StarDropUser user)
        {
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);


            var confirmationLink = $"https://front.stage.stardrop.app?email={user.Email}&token={Uri.EscapeDataString(token)}";
            var message = new ConfirmationEmailMessage()
            {
                ConfirmationLink = confirmationLink,
                To = user.Email
            };
            return message;
        }
    }
}