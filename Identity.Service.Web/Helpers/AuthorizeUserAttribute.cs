using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using NLog;
using Identity.Service.Application.Constants;
using Identity.Service.Application.Wrappers;
using Identity.Service.Core.IRepositories;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using static Identity.Service.Application.Constants.Messages;

namespace Identity.Service.Web.Helpers
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizeUserAttribute : Attribute, IAuthorizationFilter
    {
        private readonly Logger logger;
        public AuthorizeUserAttribute()
        {
            logger = LogManager.GetCurrentClassLogger();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            IServiceProvider services = context.HttpContext.RequestServices;
            DataProtector? protector = services.GetService(typeof(DataProtector)) as DataProtector;

            if (protector == null)
            {
                logger.Error("Required services are not available.");
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Result = new JsonResult(new ApiResponse<dynamic>(false, string.Empty, "Internal server error"));
                return;
            }

            JwtSecurityTokenHandler tokenHandler = new();
            JwtSecurityToken token;
            if (context.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues headerToken))
            {
                token = tokenHandler.ReadJwtToken(headerToken.FirstOrDefault()?.Split(" ").LastOrDefault());
                string? apiUserId = token.Claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(apiUserId))
                {
                    logger.Error("userId not found from header");
                }
                else
                {
                   
                    context.HttpContext.Items["UserId"] = Guid.TryParse(apiUserId, out var userIdGuid) ? userIdGuid : null;
                    return;
                }
            }
            else
            {
                string? cookie = context.HttpContext.Request.Cookies[AuthCookiesValue.AuthKey];
                if (string.IsNullOrEmpty(cookie))
                {
                    logger.Error("cookie is null");
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Result = new JsonResult(new ApiResponse<dynamic>(false, string.Empty, InvalidToken));
                    return;
                }

                token = tokenHandler.ReadJwtToken(protector.Decrypt(cookie));
            }

            string? userId = token.Claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                logger.Error("userId not found from cookie");
                return;
            }
        }
    }
}
