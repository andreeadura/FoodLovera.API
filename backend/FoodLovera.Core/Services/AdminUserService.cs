#nullable enable
using FoodLovera.Core.Contracts;
using FoodLovera.Models.Models;

namespace FoodLovera.Core.Services;

public sealed class AdminUserService(
    IUserRepository users,
    IBannedEmailRepository bannedEmails,
    IUnitOfWork uow)
    : IAdminUserService
{
    public async Task<IReadOnlyList<AdminUserListItemDTO>> GetAllAsync(CancellationToken ct)
    {
        var items = await users.GetAllAsync(ct);

        return items
            .Select(u => new AdminUserListItemDTO
            {
                Id = u.Id,
                Email = u.Email,
                Username = u.Username,
                IsEmailVerified = u.IsEmailVerified,
                Role = u.Role.ToString(),
                CreatedAt = u.CreatedAt
            })
            .ToList();
    }

    public async Task DeleteUserAsync(int targetUserId, int adminUserId, CancellationToken ct)
    {
        if (targetUserId <= 0)
            throw new ArgumentException("Invalid user id.", nameof(targetUserId));

        if (targetUserId == adminUserId)
            throw new InvalidOperationException("Admins cannot delete their own account.");

        var user = await users.GetByIdAsync(targetUserId, ct);
        if (user is null)
            throw new InvalidOperationException("User not found.");

        var emailNormalized = user.Email.Trim().ToLowerInvariant();

        await bannedEmails.AddAsync(
            emailNormalized: emailNormalized,
            bannedByUserId: adminUserId,
            reason: "Deleted by admin",
            ct: ct);

        users.Remove(user);

        await uow.SaveChangesAsync(ct);
    }
}