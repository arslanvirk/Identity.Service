using System;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Identity.Service.Application.DTOs.Shared;

namespace Identity.Service.Application.Helpers;

public static class JwtTokenHelper
{
    public static string CreateJwtSecurityToken(
            string tokenConfiguration,
            string userId,
            string firstName,
            string lastName,
            string email,
            string jwtSecret)
    {
        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim("UserId", userId),
            new Claim("Name", firstName + " " + lastName),
            new Claim("Email", email)
        };

        SymmetricSecurityKey authSigningKey = new(Encoding.ASCII.GetBytes(jwtSecret));

        JwtSecurityToken token = new(
            issuer: ParameterStoreDto.JwtValidIssuer,
            audience: ParameterStoreDto.JwtValidAudience,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(tokenConfiguration)),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        var jwtTokenHandler = new JwtSecurityTokenHandler();
        return jwtTokenHandler.WriteToken(token);
    }
}