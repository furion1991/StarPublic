using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using AuthService.Database.Context;
using AuthService.Database.Models;

using DataTransferLib.DataTransferObjects.Auth;
using DataTransferLib.DataTransferObjects.Common;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services;

public class JwtTokenService
{
    private readonly AuthContext _authDbContext;
    private readonly UserManager<StarDropUser> _userManager;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _secret;

    [SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
    public JwtTokenService(AuthContext authDbContext, UserManager<StarDropUser> userManager)
    {
        _authDbContext = authDbContext;
        _userManager = userManager;
        _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")!;
        _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")!;
        _secret = Environment.GetEnvironmentVariable("JWT_SECRET")!;
    }

    public string GenerateJwtToken(StarDropUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id) // ✅ ID пользователя всегда есть
        };

        if (!string.IsNullOrEmpty(user.Email))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
        }

        if (!string.IsNullOrEmpty(user.UserName))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Nickname, user.UserName));
        }

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<RefreshToken> GenerateNewRefreshToken(StarDropUser user)
    {
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };
        await _authDbContext.AddAsync(refreshToken);
        refreshToken.UserId = user.Id;

        await _authDbContext.SaveChangesAsync();

        return refreshToken;
    }


    public async Task<TokensResponse?> RefreshAccessToken(string refreshToken)
    {
        var tokenFromDb = await _authDbContext.RefreshTokens.FirstOrDefaultAsync(e => e.Token == refreshToken);
        if (tokenFromDb is null || tokenFromDb.IsRevoked || tokenFromDb.ExpiryDate < DateTime.UtcNow)
        {
            return null;
        }

        tokenFromDb.IsRevoked = true;

        await _authDbContext.SaveChangesAsync();

        var user = await _userManager.FindByIdAsync(tokenFromDb.UserId);
        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = GenerateJwtToken(user, roles);
        var newRefreshToken = await GenerateNewRefreshToken(user);


        return new TokensResponse()
        {
            RefreshToken = newRefreshToken.Token,
            Token = newAccessToken
        };
    }

    public async Task<string> GetUserIdFromRefreshToken(string refreshToken)
    {
        var tokenFromDb = await _authDbContext.RefreshTokens.FirstOrDefaultAsync(e => e.Token == refreshToken);
        if (tokenFromDb == null)
        {
            return string.Empty;
        }
        return tokenFromDb.UserId;
    }

    public async Task<ActionResult> RevokeRefreshToken(string? token)
    {
        var tokenFromDb = await _authDbContext.RefreshTokens.FirstOrDefaultAsync(e => e.Token == token);
        if (tokenFromDb is null)
        {
            return new ActionResult()
            {
                IsSuccessful = false,
                Message = "No tokens found"
            };
        }

        try
        {
            tokenFromDb.IsRevoked = true;
            await _authDbContext.SaveChangesAsync();
            return new ActionResult()
            {
                IsSuccessful = true,
                Message = "Token revoked"
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ActionResult()
            {
                IsSuccessful = false,
                Message = e.Message
            };
        }
    }

    public ClaimsPrincipal? ValidateAccessToken(string token)
    {
        var _secretKey = _secret;
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_secretKey);

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),

                ValidateIssuer = true,
                ValidIssuer = _issuer,

                ValidateAudience = true,
                ValidAudience = _audience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };


            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            if (validatedToken is JwtSecurityToken jwtToken && jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return principal;
            }
        }
        catch (SecurityTokenException ex)
        {
            Console.WriteLine("Token validation failed: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }

        return null;
    }
}