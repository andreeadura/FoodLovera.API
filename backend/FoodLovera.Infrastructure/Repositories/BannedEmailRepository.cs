#nullable enable
using FoodLovera.Core.Contracts;
using FoodLovera.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodLovera.Infrastructure.Repositories;

public sealed class BannedEmailRepository(FoodLoveraDbContext db) : IBannedEmailRepository
{
    public Task<bool> ExistsAsync(string emailNormalized, CancellationToken ct) =>
        db.BannedEmails.AnyAsync(x => x.EmailNormalized == emailNormalized, ct);

    public async Task AddAsync(string emailNormalized, int? bannedByUserId, string? reason, CancellationToken ct)
    {
        var exists = await ExistsAsync(emailNormalized, ct);
        if (exists) return;

        db.BannedEmails.Add(new BannedEmail
        {
            EmailNormalized = emailNormalized,
            BannedByUserId = bannedByUserId,
            Reason = reason,
            BannedAtUtc = DateTime.UtcNow
        });
    }

    public async Task RemoveAsync(string emailNormalized, CancellationToken ct)
    {
        var entity = await db.BannedEmails
            .FirstOrDefaultAsync(x => x.EmailNormalized == emailNormalized, ct);

        if (entity is null) return;

        db.BannedEmails.Remove(entity);
    }
}