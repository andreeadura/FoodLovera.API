#nullable enable
using FoodLovera.Models.Entities;

namespace FoodLovera.Core.Contracts;

public interface IEmailVerificationTokenRepository
{
    Task<EmailVerificationToken?> GetByUserIdAsync(int userId, CancellationToken ct);
    Task AddAsync(EmailVerificationToken token, CancellationToken ct);
    void Remove(EmailVerificationToken token);
}