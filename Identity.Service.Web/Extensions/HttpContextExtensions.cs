using System.Security.Claims;
using Identity.Service.Core.Entities;
using Identity.Service.Application.Wrappers;
using Identity.Service.Application.Constants;
using static Identity.Service.Application.Constants.Messages;

namespace Identity.Service.Application.Helpers;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this HttpContext context)
    {
        if (context?.User?.Identity is ClaimsIdentity identity)
        {
            var idClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(idClaim?.Value, out var userId))
                return userId;
        }
        throw new UnauthorizedAccessException(InvalidToken);
    }
}