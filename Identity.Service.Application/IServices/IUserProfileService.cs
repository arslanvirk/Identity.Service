using System;
using Identity.Service.Application.DTOs;
using Identity.Service.Application.Wrappers;

namespace Identity.Service.Application.IServices
{
    /// <summary>
    /// Handles user profile management.
    /// </summary>
    public interface IUserProfileService
    {
        Task<ApiResponse<UserProfileDto>> GetAsync(Guid userId);
        Task<ApiResponse<bool>> UpdateAsync(Guid id, UpdateProfileDto dto);
    }
}