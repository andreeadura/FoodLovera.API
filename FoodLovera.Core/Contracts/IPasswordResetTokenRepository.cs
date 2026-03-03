#nullable enable
using FoodLovera.Models.Entities;

namespace FoodLovera.Core.Contracts;

public interface IPasswordResetTokenRepository
{
    Task<PasswordResetToken?> GetByUserIdAsync(int userId, CancellationToken ct);
    Task AddAsync(PasswordResetToken token, CancellationToken ct);
    void Remove(PasswordResetToken token);
}