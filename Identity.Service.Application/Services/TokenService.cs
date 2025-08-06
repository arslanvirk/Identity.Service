using System.Security.Claims;
using Identity.Service.Core.Entities;
using Identity.Service.Application.Helpers;
using Identity.Service.Application.IServices;
using Identity.Service.Application.DTOs.Shared;

namespace Identity.Service.Application.Services;

public class TokenService : ITokenService
{
    public string GenerateToken(string tokenConfiguration, User user, IList<Claim> claims)
    {
        if (string.IsNullOrEmpty(SecretManagerDto.JwtSecret))
        {
            throw new ArgumentNullException(nameof(tokenConfiguration), "JWT secret cannot be null or empty.");
        }

        return JwtTokenHelper.CreateJwtSecurityToken(
            tokenConfiguration,
            userId: user.Id.ToString(),
            firstName: user.FirstName ?? string.Empty,
            lastName: user.LastName ?? string.Empty,
            email: user.Email ?? string.Empty,
            jwtSecret: SecretManagerDto.JwtSecret ?? string.Empty
        );
    }
}