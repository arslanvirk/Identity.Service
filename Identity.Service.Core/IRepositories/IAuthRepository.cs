using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Identity.Service.Core.Entities;

namespace Identity.Service.Core.IRepositories
{
    /// <summary>
    /// Encapsulates authentication-related operations such as password management, claims retrieval, and token operations.
    /// </summary>
    public interface IAuthRepository
    {
        Task<bool> CheckPasswordAsync(User user, string pwd);
        Task<IList<Claim>> GetClaimsAsync(User user);
        Task<string> GeneratePasswordResetTokenAsync(User user);
        Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword);
        Task<IdentityResult> ChangePasswordAsync(User user, string currentPwd, string newPwd);
        Task<bool> RemovePassword(User user);
        Task<bool> AddPassword(User user, string password);
        Task SignOut();
    }
}
