#nullable enable
using FoodLovera.Core.Contracts;
using FoodLovera.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodLovera.Infrastructure.Repositories;

public sealed class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly FoodLoveraDbContext _db;

    public PasswordResetTokenRepository(FoodLoveraDbContext db) => _db = db;

    public Task<PasswordResetToken?> GetByUserIdAsync(int userId, CancellationToken ct) =>
        _db.PasswordResetTokens.FirstOrDefaultAsync(x => x.UserId == userId, ct);

    public async Task AddAsync(PasswordResetToken token, CancellationToken ct) =>
        await _db.PasswordResetTokens.AddAsync(token, ct);

    public void Remove(PasswordResetToken token) =>
        _db.PasswordResetTokens.Remove(token);
}