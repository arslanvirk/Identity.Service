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
using Microsoft.IdentityModel.Tokens;
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
builder.Services.AddSwaggerGen();
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
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretManagerDto.JwtSecret))
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

// Configure the HTTP request pipeline.
app.UseSwagger(c =>
{
    c.RouteTemplate = "identity/swagger/{documentName}/swagger.json";
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/identity/swagger/v1/swagger.json", "identity v1");
    c.RoutePrefix = "identity/swagger";
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

app.Run();
