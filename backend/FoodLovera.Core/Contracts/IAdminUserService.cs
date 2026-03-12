#nullable enable
using FoodLovera.Models.Models;

namespace FoodLovera.Core.Contracts;

public interface IAdminUserService
{
    Task<IReadOnlyList<AdminUserListItemDTO>> GetAllAsync(CancellationToken ct);
    Task DeleteUserAsync(int targetUserId, int adminUserId, CancellationToken ct);
}