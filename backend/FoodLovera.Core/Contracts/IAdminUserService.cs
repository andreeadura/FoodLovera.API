#nullable enable
namespace FoodLovera.Core.Contracts;

public interface IAdminUserService
{
    Task DeleteUserAsync(int targetUserId, int adminUserId, CancellationToken ct);
}