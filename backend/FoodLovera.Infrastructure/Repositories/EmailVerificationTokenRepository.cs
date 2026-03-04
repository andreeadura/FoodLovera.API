#nullable enable
using FoodLovera.Core.Contracts;
using FoodLovera.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodLovera.Infrastructure.Repositories;

public sealed class EmailVerificationTokenRepository : IEmailVerificationTokenRepository
{
    private readonly FoodLoveraDbContext _db;

    public EmailVerificationTokenRepository(FoodLoveraDbContext db) => _db = db;

    public Task<EmailVerificationToken?> GetByUserIdAsync(int userId, CancellationToken ct) =>
        _db.EmailVerificationTokens.FirstOrDefaultAsync(x => x.UserId == userId, ct);

    public async Task AddAsync(EmailVerificationToken token, CancellationToken ct) =>
        await _db.EmailVerificationTokens.AddAsync(token, ct);

    public void Remove(EmailVerificationToken token) =>
        _db.EmailVerificationTokens.Remove(token);
}