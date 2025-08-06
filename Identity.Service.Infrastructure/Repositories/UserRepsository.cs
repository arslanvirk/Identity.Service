using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Identity.Service.Core.Entities;
using Identity.Service.Core.IRepositories;
using Identity.Service.Application.Constants;
using Identity.Service.Application.Exceptions;
using Identity.Service.Infrastructure.Context;
using static Identity.Service.Application.Constants.Messages;

namespace Identity.Service.Infrastructure.Repositories;

public class UserRepsository : IUserRepository
{
    private readonly EFDataContext _ctx;
    private readonly UserManager<User> _um;
    private readonly int _pastdays = -5;
    public UserRepsository(EFDataContext ctx, UserManager<User> um)
    {
        _ctx = ctx;
        _um = um;
    }

    public async Task UpdateAsync(User user)
    {
        _ctx.Users.Update(user);
        await _ctx.SaveChangesAsync();
    }
    public async Task<User?> FindByEmailAsync(string email)
    {
        var dbProvider = _ctx.Database.ProviderName;
        if (dbProvider != null && dbProvider.Contains("Npgsql"))
        {
            return await _um.Users
                .Where(u => EF.Functions.ILike(u.Email!, email))
                .FirstOrDefaultAsync();
        }
        else
        {
            // SQLite and others: fallback to StringComparison.OrdinalIgnoreCase for case-insensitive search
            return _um.Users
                .AsEnumerable()
                .FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
        }
    }

    public async Task<User?> FindByIdAsync(Guid id) =>
    await _um.Users.FirstOrDefaultAsync(u => u.Id == id);

    public async Task<IdentityResult> CreateUserAsync(User user, string pwd)
    => await _um.CreateAsync(user, pwd);

    public async Task<InviteUser?> GetActiveInviteUserById(Guid inviteUserId)
    => await _ctx.InviteUsers.Where(e => e.InviteUserId.Equals(inviteUserId) && e.IsActive).FirstOrDefaultAsync();

    public async Task<List<InviteUser>> GetRecentInviteUsersByUserId(Guid userId)
          => await _ctx.InviteUsers.Where(e => e.UserId.Equals(userId) &&
          e.CreatedAt > DateTime.UtcNow.AddMinutes(_pastdays)).OrderByDescending(x => x.CreatedAt).ToListAsync();

    public async Task<List<InviteUser>> DeactivateInviteUsersByUserId(Guid userId)
    {
        List<InviteUser> result = await _ctx.InviteUsers.Where(e => e.UserId.Equals(userId) && e.IsActive).ToListAsync() ??
            throw new UnprocessableEntityException(InviteUserNotFound);
        result.ForEach(x => x.IsActive = false);
        _ctx.InviteUsers.UpdateRange(result);
        _ = await _ctx.SaveChangesAsync();
        return result;
    }

    public async Task<InviteUser> CreateInviteUser(InviteUser inviteUser)
    {
        _ = _ctx.InviteUsers.Add(inviteUser);
        _ = await _ctx.SaveChangesAsync();
        return inviteUser;
    }

    public async Task<InviteUser> UpdateInviteUser(InviteUser inviteUser)
    {
        _ = _ctx.InviteUsers.Update(inviteUser);
        _ = await _ctx.SaveChangesAsync();
        return inviteUser;
    }
}
