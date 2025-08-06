using System;
using Identity.Service.Application.DTOs;
using Identity.Service.Core.IRepositories;
using Identity.Service.Application.Helpers;
using Identity.Service.Application.Wrappers;
using Identity.Service.Application.IServices;
using Identity.Service.Application.Exceptions;
using static Identity.Service.Application.Constants.Messages;

namespace Identity.Service.Application.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUserRepository _profileRepo;
    private readonly IAuthRepository _authRepo;
    private readonly AuthHelper _authHelper;
    public UserProfileService(IUserRepository repo, IAuthRepository authRepo, AuthHelper authHelper)
    {
        _profileRepo = repo;
        _authRepo = authRepo;
        _authHelper = authHelper;
    }

    public async Task<ApiResponse<UserProfileDto>> GetAsync(Guid userId)
    {
        var user = await _profileRepo.FindByIdAsync(userId);
        if (user is null)
            throw new KeyNotFoundException($"{UserNotFound} {userId}");

        var profile = new UserProfileDto
        (
            user.Id,
            user.Email!,
            user.FirstName!,
            user.LastName!
        );

        return new ApiResponse<UserProfileDto>(true, profile, StatusSuccess);
    }

    public async Task<ApiResponse<bool>> UpdateAsync(Guid id, UpdateProfileDto dto)
    {
        var user = await _profileRepo.FindByIdAsync(id);
        if (user is null)
            throw new KeyNotFoundException($"{UserNotFound} {id}");

        user.Email = dto.Email;
        user.NormalizedEmail = dto.Email?.ToUpper();
        user.UserName = dto.Email;
        user.NormalizedUserName = dto.Email;
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = id.ToString();
        bool isPasswordUpdated = false;
        if (dto.Password != null && dto.ConfirmPassword != null)
        {
            if (await _authRepo.CheckPasswordAsync(user, dto.Password))
                throw new KeyNotFoundException(PasswordCannotBeSame);

            if (dto.Password != dto.ConfirmPassword)
                throw new UnprocessableEntityException(PasswordsDoNotMatch);
            var token = await _authRepo.GeneratePasswordResetTokenAsync(user);
            var result = await _authRepo.ResetPasswordAsync(user, token, dto.Password);
            if (!result.Succeeded)
                throw new UnprocessableEntityException(AlphanumericPassword);

            user.PasswordUpdatedAt = DateTime.UtcNow;
            isPasswordUpdated = true;
        }
        await _profileRepo.UpdateAsync(user);
        if (isPasswordUpdated)
        {
            await _authRepo.SignOut();
            _authHelper.ExpireCookies();
        }
        return new ApiResponse<bool>(true, true, UserProfileUpdatedSuccessMessage);
    }
}
