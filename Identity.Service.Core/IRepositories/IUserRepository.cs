using System;
using Microsoft.AspNetCore.Identity;
using Identity.Service.Core.Entities;

namespace Identity.Service.Core.IRepositories
{
    /// <summary>
    /// Provides operations for user management including creation, lookup by id or email, 
    /// and handling invitation-related functionality.
    /// </summary>
    public interface IUserRepository
    {
        Task<User?> FindByIdAsync(Guid id);
        Task<User?> FindByEmailAsync(string email);
        Task UpdateAsync(User user);
        Task<IdentityResult> CreateUserAsync(User user, string pwd);

        // Invitation operations:
        Task<InviteUser?> GetActiveInviteUserById(Guid inviteUserId);
        Task<List<InviteUser>> GetRecentInviteUsersByUserId(Guid userId);
        Task<List<InviteUser>> DeactivateInviteUsersByUserId(Guid userId);
        Task<InviteUser> CreateInviteUser(InviteUser inviteUser);
        Task<InviteUser> UpdateInviteUser(InviteUser inviteUser);
    }
}