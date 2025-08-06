using Identity.Service.Web.Helpers;
using Identity.Service.Core.IRepositories;
using Identity.Service.Application.Helpers;
using Identity.Service.Application.Services;
using Identity.Service.Application.IServices;
using Identity.Service.Application.DTOs.Shared;
using Identity.Service.Infrastructure.Repositories;

namespace Identity.Service.Web.Extensions;

public static class ServiceConfig
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        // Application Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IConfigurationService, ConfigurationService>();

        // Helpers & Logging
        services.AddScoped<DataProtector>();
        services.AddScoped<NLogTrack, NLogTrack>();
        services.AddScoped<AuthHelper, AuthHelper>();

        services.AddSingleton<EmailTemplateService, EmailTemplateService>();

        // Infrastructure Services
        services.AddScoped<IConfigRepository, ConfigRepository>();
        services.AddScoped<IUserRepository, UserRepsository>();
        services.AddScoped<IAuthRepository, AuthRepository>();

        return services;
    }
}
