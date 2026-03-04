#nullable enable
using FoodLovera.Core.Contracts;

namespace FoodLovera.Core.Services;

public sealed class AdminUserService(
    IUserRepository users,
    IBannedEmailRepository bannedEmails,
    IUnitOfWork uow)
    : IAdminUserService
{
    public async Task DeleteUserAsync(int targetUserId, int adminUserId, CancellationToken ct)
    {
        if (targetUserId <= 0)
            throw new ArgumentException("Invalid user id.", nameof(targetUserId));

        // nu stergi propriul cont
        if (targetUserId == adminUserId)
            throw new InvalidOperationException("Admins cannot delete their own account.");

        var user = await users.GetByIdAsync(targetUserId, ct);
        if (user is null)
            throw new InvalidOperationException("User not found.");

        var emailNormalized = user.Email.Trim().ToLowerInvariant();

        // Ban email 
        await bannedEmails.AddAsync(
            emailNormalized: emailNormalized,
            bannedByUserId: adminUserId,
            reason: "Deleted by admin",
            ct: ct);

        // Delete user
        users.Remove(user);

        await uow.SaveChangesAsync(ct);
    }
}