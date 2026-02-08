using Identity.Service.Application.Constants;
using Identity.Service.Application.DTOs.Shared;
using Identity.Service.Application.DTOs.Shared.Configurations;
using Identity.Service.Core.Entities;
using Identity.Service.Infrastructure.Context;
using Identity.Service.Web.Extensions;
using Identity.Service.Web.Helpers;
using Identity.Service.Web.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SchemaOptionsDto>(builder.Configuration.GetSection("SchemaOptions"));
AppSettingsDto.EnvironmentName = builder.Configuration["EnvironmentName"] ?? string.Empty;
SecretManagerDto.JwtSecret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
ParameterStoreDto.JwtValidIssuer = builder.Configuration["Jwt:Issuer"] ?? string.Empty;
ParameterStoreDto.JwtValidAudience = builder.Configuration["Jwt:Audience"] ?? string.Empty;
builder.Services.AddIdentityServices();
builder.Services.AddControllers(options =>
    options.Conventions.Add(new ControllerBasePathRegistration("identity/identity/api/v1")));

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

var keycloakAuthority = builder.Configuration["Keycloak:Authority"]!;
var keycloakClientId = builder.Configuration["Keycloak:ClientId"]!;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Identity Service API",
        Version = "v1",
        Description = "Identity Service API with Keycloak Authentication"
    });

    // OAuth2 definition for Keycloak
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/auth"),
                TokenUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID Connect" },
                    { "profile", "User profile" },
                    { "email", "Email address" }
                }
            }
        },
        Description = "Keycloak OAuth2 Authorization Code Flow"
    });

    // Global security requirement
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "oauth2"
                }
            },
            new[] { "openid", "profile", "email" }
        }
    });
});

builder.Services.AddHealthChecks();

builder.Services.AddDbContext<EFDataContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, UserRole>(opts =>
{
    opts.User.RequireUniqueEmail = true;
    opts.Password.RequiredLength = ConfigurationKey.RequiredLength;
    opts.Password.RequireUppercase = true;
    opts.Password.RequireDigit = true;
    opts.Lockout.MaxFailedAccessAttempts = ConfigurationKey.MaxFailedAccessAttempts;
})
.AddEntityFrameworkStores<EFDataContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var keycloakMetadataAddress = builder.Environment.IsDevelopment()
        ? builder.Configuration["Keycloak:MetadataAddress"]! // Use localhost for browser
        : builder.Configuration["Keycloak:MetadataAddress"]!.Replace("localhost", "keycloak"); // Use service name in Docker
    
    options.MetadataAddress = keycloakMetadataAddress;
    options.Audience = builder.Configuration["Keycloak:Audience"];

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Keycloak:Issuer"],
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(5)
    };

    // Required for HTTP in development (Keycloak uses HTTP by default in dev mode)
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully");
            return Task.CompletedTask;
        }
    };
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opts =>
{
    opts.Cookie.Name = AuthCookiesValue.AuthKey;
    opts.Cookie.HttpOnly = true;
    opts.LoginPath = "/identity/api/v1/login";
    opts.SlidingExpiration = true;
});

var app = builder.Build();

// Apply pending EF Core migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EFDataContext>();
   // await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity Service API v1");
    options.OAuthClientId(keycloakClientId);
    options.OAuthUsePkce(); // Use PKCE for security
    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
});

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.UseRouting();

app.UseMiddleware<RequestResponseMiddleware>();
app.UseMiddleware<AuthenticationMiddleware>();

app.MapControllers();
app.MapHealthChecks("/identity/identity/health", new HealthCheckOptions
{
    AllowCachingResponses = false
}).WithMetadata(new AllowAnonymousAttribute());

await app.RunAsync();
