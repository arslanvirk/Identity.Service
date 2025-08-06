using System.Text;
using System.IdentityModel.Tokens.Jwt;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using NLog;
using Identity.Service.Web.Helpers;
using Identity.Service.Core.Entities;
using Identity.Service.Application.Constants;
using Identity.Service.Application.DTOs.Shared;
using static Identity.Service.Application.Constants.Messages;

namespace Identity.Service.Web.Middlewares
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, UserManager<User> userManager)
        {
            var services = context.RequestServices;
            var logTrack = services.GetService<NLogTrack>();
            var protector = services.GetService<DataProtector>();

            if (logTrack == null || protector == null)
            {
                _logger.Error("Required service(s) missing: {0} or {1}", nameof(NLogTrack), nameof(DataProtector));
                throw new UnauthorizedAccessException(InvalidToken);
            }

            // Allow anonymous endpoints
            if (context.GetEndpoint()?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                await _next(context);
                return;
            }

            // Pass through if Authorization header exists
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                await _next(context);
                return;
            }

            // Check for the authentication cookie
            if (!context.Request.Cookies.TryGetValue(AuthCookiesValue.AuthKey, out var cookie) || string.IsNullOrWhiteSpace(cookie))
            {
                _logger.Error(logTrack.GetLogMessage(AuthenticationCookieIsMissing));
                throw new UnauthorizedAccessException(InvalidToken);
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var decryptedToken = protector.Decrypt(cookie);

                if (string.IsNullOrWhiteSpace(SecretManagerDto.JwtSecret))
                {
                    _logger.Error(logTrack.GetLogMessage(JwtSecretMissing));
                    throw new UnauthorizedAccessException(InvalidToken);
                }

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = ParameterStoreDto.JwtValidIssuer,
                    ValidAudience = ParameterStoreDto.JwtValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretManagerDto.JwtSecret)),
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(decryptedToken, validationParameters, out _);
                context.User = principal;
            }
            catch (Exception ex)
            {
                _logger.Error(logTrack.GetLogMessage($"JWT validation failed: {ex.Message}"));
                throw new UnauthorizedAccessException(InvalidToken);
            }

            await _next(context);
        }
    }
}