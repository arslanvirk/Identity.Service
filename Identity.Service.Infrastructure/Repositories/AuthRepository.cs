using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Identity.Service.Core.Entities;
using Identity.Service.Core.IRepositories;

namespace Identity.Service.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<User> _um;
        private readonly SignInManager<User> _signInManager;

        public AuthRepository(UserManager<User> um, SignInManager<User> signInManager)
        {
            _um = um;
            _signInManager = signInManager;
        }

        public async Task<bool> CheckPasswordAsync(User user, string pwd)
            => await _um.CheckPasswordAsync(user, pwd);

        public async Task<IList<Claim>> GetClaimsAsync(User user)
            => await _um.GetClaimsAsync(user);

        public async Task<string> GeneratePasswordResetTokenAsync(User user) =>
           await _um.GeneratePasswordResetTokenAsync(user);

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPwd, string newPwd) =>
           await _um.ChangePasswordAsync(user, currentPwd, newPwd);

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword)
            => await _um.ResetPasswordAsync(user, token, newPassword);

        public async Task<bool> AddPassword(User user, string password)
        {
            IdentityResult result = await _um.AddPasswordAsync(user, password);
            return result.Succeeded;
        }
        public async Task<bool> RemovePassword(User user)
        {
            IdentityResult result = await _um.RemovePasswordAsync(user);
            return result.Succeeded;
        }
        public async Task SignOut()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
