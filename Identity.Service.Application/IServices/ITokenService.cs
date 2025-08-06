using System;
using System.Security.Claims;
using Identity.Service.Core.Entities;

namespace Identity.Service.Application.IServices
{
    public interface ITokenService
    {
        string GenerateToken(string tokenConfiguration, User user, IList<Claim> claims);
    }
}
