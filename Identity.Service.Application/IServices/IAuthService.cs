using System;
using Identity.Service.Application.DTOs;
using Identity.Service.Application.Wrappers;

namespace Identity.Service.Application.IServices
{
    /// <summary>
    /// Handles authentication processes, including user registration, login, 
    /// and password-related operations.
    /// </summary>
    public interface IAuthService
    {
        // Authentication methods
        Task<ApiResponse<string>> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);

        // Password management methods
        Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<ApiResponse<string>> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
        Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDto resetPassworddto);
        Task SignOut();
    }
}
