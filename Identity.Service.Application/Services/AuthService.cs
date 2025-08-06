using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Identity.Service.Core.Entities;
using Identity.Service.Application.DTOs;
using Identity.Service.Core.IRepositories;
using Identity.Service.Application.Helpers;
using Identity.Service.Application.Wrappers;
using Identity.Service.Application.Constants;
using Identity.Service.Application.IServices;
using Identity.Service.Application.Exceptions;
using Identity.Service.Application.DTOs.Shared;
using static Identity.Service.Application.Constants.Messages;

namespace Identity.Service.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepo;
    private readonly ITokenService _tokenService;
    private readonly EmailTemplateService _emailService;
    private readonly IConfigRepository _configRepository;
    private readonly IUserRepository _userRepo;
    public AuthService(IAuthRepository authRepo, ITokenService tokenService, EmailTemplateService emailService,
        IConfigRepository configRepository, IUserRepository userRepo)
    {
        _authRepo = authRepo;
        _tokenService = tokenService;
        _emailService = emailService;
        _userRepo = userRepo;
        _configRepository = configRepository;
    }

    public async Task<ApiResponse<string>> RegisterAsync(RegisterDto dto)
    {

        var user = new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = dto.Email
        };

        var res = await _userRepo.CreateUserAsync(user, dto.Password);
        if (!res.Succeeded)
            HandleValidationErrors(res);

        return new ApiResponse<string>(true, "", UserCreatedSuccess);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var u = await _userRepo.FindByEmailAsync(dto.Email)
               ?? throw new KeyNotFoundException(InvalidEmail);
        if (!await _authRepo.CheckPasswordAsync(u, dto.Password))
            throw new KeyNotFoundException(InvalidCredentials);

        var claims = await _authRepo.GetClaimsAsync(u);
        var tokenConfig = (await _configRepository.GetByKeyAsync(ConfigurationKey.TokenExpiryTimeInMinutes)).ConfigurationValue;
        string token = _tokenService.GenerateToken(tokenConfig, u, claims);
        var exp = DateTime.UtcNow.AddMinutes(double.Parse(tokenConfig));
        return new AuthResponseDto(token, exp);
    }

    public async Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);

        var user = await _userRepo.FindByEmailAsync(dto.Email)
                   ?? throw new KeyNotFoundException($"User {dto.Email}");

        // Enforce rate limiting based on previous invite requests
        var previousLinks = await _userRepo.GetRecentInviteUsersByUserId(user.Id);
        if (previousLinks.Count >= 3)
        {
            throw new RatelimitingException(TooManyRequest);
        }

        // Proceed only if the user is active and the email is confirmed
        if (user.EmailConfirmed && user.IsActive)
        {
            // Deactivate any existing invite links
            await _userRepo.DeactivateInviteUsersByUserId(user.Id);

            // Retrieve configuration for link expiry time
            var linkExpiryConfig = await _configRepository.GetByKeyAsync(ConfigurationKey.LinkExpiryTimeInMinutes);
            var expiryMinutes = double.Parse(linkExpiryConfig.ConfigurationValue);
            var linkExpiryTime = DateTime.UtcNow.AddMinutes(expiryMinutes);

            // Create a new invite record
            var inviteUser = new InviteUser
            {
                UserId = user.Id,
                ExpireTime = linkExpiryTime,
                Status = InviteUserStatus.Pending,
                Type = InviteUserType.ForgetPassword,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var inviteUserModel = await _userRepo.CreateInviteUser(inviteUser);

            // Prepare email content and send password reset email
            if (string.IsNullOrEmpty(ParameterStoreDto.Email_Identity))
            {
                throw new KeyNotFoundException("Email_Rna configuration is missing.");
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                throw new KeyNotFoundException("User email is missing.");
            }
            string emailHtml = "";
            bool emailSent = await _emailService.SendForgetPasswordEmail(
                ParameterStoreDto.Email_Identity,
                user.Email,
                PasswordResetReq,
                inviteUserModel.InviteUserId,
                emailHtml
            );

            if (!emailSent)
            {
                throw new KeyNotFoundException(FailedToSendPasswordResetEmail);
            }

        }

        scope.Complete();

        return new ApiResponse<string>(true, string.Empty, $"{ResetPassword} {dto.Email}");
    }

    public async Task<ApiResponse<string>> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
    {
        var user = await _userRepo.FindByIdAsync(userId)
                   ?? throw new KeyNotFoundException($"User {userId}");

        var result = await _authRepo.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

        if (!result.Succeeded)
        {
            HandleValidationErrors(result);
        }

        return new ApiResponse<string>(true, "", ChangedPassword);
    }

    public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDto resetPassworddto)
    {
        // Validate invite user
        var inviteUser = await _userRepo.GetActiveInviteUserById(Guid.Parse(resetPassworddto.InviteUserId.Trim()));
        if (inviteUser == null)
        {
            throw new UnprocessableEntityException(ResendEmail);
        }

        // Validate user existence
        var user = await _userRepo.FindByIdAsync(inviteUser.UserId)
                   ?? throw new UnprocessableEntityException(InvalidUserId);

        // Check if the new password is the same as the current password
        if (await _authRepo.CheckPasswordAsync(user, resetPassworddto.Password))
        {
            throw new UnprocessableEntityException(PasswordCannotBeSame);
        }

        // Update user details
        user.EmailConfirmed = true;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = inviteUser.UserId.ToString();

        // Check if the invite link has expired
        if (inviteUser.ExpireTime < DateTime.UtcNow)
        {
            return new ApiResponse<string>(false, string.Empty, LinkExpired);
        }

        // Reset password
        if (!await _authRepo.RemovePassword(user) || !await _authRepo.AddPassword(user, resetPassworddto.Password))
        {
            throw new UnprocessableEntityException(AlphanumericPassword);
        }

        // Update user password time stamp
        user.PasswordUpdatedAt = DateTime.UtcNow;
        await _userRepo.UpdateAsync(user);

        // Update invite user status
        inviteUser.Status = InviteUserStatus.Used;
        inviteUser.IsActive = false;
        inviteUser.UpdatedAt = DateTime.UtcNow;
        inviteUser.UpdatedBy = inviteUser.UserId.ToString();
        await _userRepo.UpdateInviteUser(inviteUser);

        return new ApiResponse<string>(true, string.Empty, PasswordResetSuccessful);
    }

    public async Task SignOut()
    {
        await _authRepo.SignOut();
    }

    private static void HandleValidationErrors(IdentityResult result)
    {
        if (result.Errors == null || !result.Errors.Any())
        {
            throw new UnprocessableEntityException(new Dictionary<string, string[]>
            {
                { "General", new[] { UnknownValidationError } }
            });
        }

        // Group errors by code and map descriptions
        var errors = result.Errors
            .GroupBy(e => e.Code)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.Description).ToArray()
            );

        var firstError = errors.FirstOrDefault().Value?.FirstOrDefault();
        if (string.IsNullOrEmpty(firstError))
            throw new UnprocessableEntityException(UnknownValidationError);
        else if (errors.FirstOrDefault().Key == DuplicateUserName || errors.FirstOrDefault().Key ==  DuplicateEmail)
            throw new UnprocessableEntityException(DuplicateEmailaddress);
        else
            throw new UnprocessableEntityException(errors);
    }
}