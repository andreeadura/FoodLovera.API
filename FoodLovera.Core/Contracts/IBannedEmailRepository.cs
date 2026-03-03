#nullable enable
namespace FoodLovera.Core.Contracts;

public interface IBannedEmailRepository
{
    Task<bool> ExistsAsync(string emailNormalized, CancellationToken ct);
    Task AddAsync(string emailNormalized, int? bannedByUserId, string? reason, CancellationToken ct);
    Task RemoveAsync(string emailNormalized, CancellationToken ct);
}